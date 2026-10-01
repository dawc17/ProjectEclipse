using System;
using System.Collections.Generic;
using System.Net;

namespace Eclipse.Multiplayer.Online.Rooms
{
    public enum RoomClientState { Connecting, Connected, Closed }

    public enum RoomEventType { Error, RoomListUpdated, RoomChanged, LeftRoom, Paired, Chat, Spectating }

    public struct RoomEvent
    {
        public RoomEventType Type;
        public string Text;
        public RoomPairing Pairing;
        public ChatLine Chat;
    }

    /// <summary>
    /// A game client's connection to the room server. One UDP socket carries server
    /// traffic, hole-punch probes and the fight itself, so the NAT mapping the server
    /// observed is the one the opponent punches through. Single-threaded; call
    /// <see cref="Update"/> every frame.
    /// </summary>
    public sealed class RoomClient : IDisposable
    {
        public const int HelloRetryMs = 300;
        public const int ConnectTimeoutMs = 10000;
        public const int KeepAliveMs = 500;
        public const int TimeoutMs = 15000;
        /// <summary>Chat lines kept for the room page; older ones drop off.</summary>
        public const int ChatHistory = 60;

        private readonly UdpTransport _socket;
        private readonly NetWriter _writer = new NetWriter();
        private readonly NetIdentity _identity;
        private readonly List<IPEndPoint> _lanEndPoints;
        private readonly ReliableChannel _reliable = new ReliableChannel();
        private readonly Queue<RoomEvent> _events = new Queue<RoomEvent>();
        private readonly Action<EndPoint, byte[], int> _handler;
        private readonly uint _token;
        private long _startedMs, _lastHelloMs = long.MinValue, _lastSendMs = long.MinValue, _nowMs;
        private uint _cookie;
        private long _windowReceivedMs, _serverUnixMs;
        public PlaytestWindow Window { get; private set; }
        private long ServerUnixMs => ServerUnixMsAt(_nowMs);
        private long ServerUnixMsAt(long nowMs) => _serverUnixMs + Math.Max(0, nowMs - _windowReceivedMs);
        public bool CanPlay => CanPlayAt(_nowMs);
        public bool CanPlayAt(long nowMs) => State == RoomClientState.Connected && Window != null &&
            nowMs - _windowReceivedMs < PlaytestWindow.LeaseMs &&
            Window.BlockReason(ServerUnixMsAt(nowMs)) == null;
        public string PlaytestMessage => PlaytestMessageAt(_nowMs);
        public string PlaytestMessageAt(long nowMs) => State == RoomClientState.Closed ? CloseReason :
            Window == null ? "Connecting to the playtest server..." : Window.BlockReason(ServerUnixMsAt(nowMs)) ??
            (nowMs - _windowReceivedMs >= PlaytestWindow.LeaseMs ? "Lost connection to the playtest server. Reconnect to continue." : Window.Message(ServerUnixMsAt(nowMs)));

        public IPEndPoint Server { get; }
        public RoomClientState State { get; private set; } = RoomClientState.Connecting;
        public string CloseReason { get; private set; } = string.Empty;
        public uint ClientId { get; private set; }
        /// <summary>This client's address as the server sees it.</summary>
        public IPEndPoint PublicEndPoint { get; private set; }
        public long LastReceiveMs { get; private set; }
        public RoomState Room { get; private set; }
        public readonly List<RoomListing> Rooms = new List<RoomListing>();
        /// <summary>This room's chat, oldest first; cleared on leaving the room.</summary>
        public readonly List<ChatLine> Chat = new List<ChatLine>();
        public MatchLink Link { get; private set; }
        public SpectatorStream Spectating { get; private set; }
        private uint _wantedSpectate;
        /// <summary>Test aid: skip hole punching and always relay through the server.</summary>
        public bool ForceRelay { get; set; }

        /// <param name="lanEndPoints">This machine's LAN addresses (port is ignored); may be empty.</param>
        public RoomClient(IPEndPoint server, NetIdentity identity, IEnumerable<IPAddress> lanAddresses, long nowMs)
        {
            Server = server ?? throw new ArgumentNullException(nameof(server));
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            _socket = new UdpTransport(0);
            _lanEndPoints = new List<IPEndPoint>();
            foreach (var address in lanAddresses ?? Array.Empty<IPAddress>())
            {
                if (address == null || address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork || IPAddress.IsLoopback(address)) continue;
                if (_lanEndPoints.Count < RoomProtocol.MaxCandidates - 1) _lanEndPoints.Add(new IPEndPoint(address, _socket.LocalPort));
            }
            _token = (uint)new Random().Next(1, int.MaxValue);
            _handler = OnDatagram;
            _startedMs = LastReceiveMs = nowMs;
        }

        public int LocalPort => _socket.LocalPort;

        public bool TryGetEvent(out RoomEvent roomEvent)
        {
            if (_events.Count == 0) { roomEvent = default; return false; }
            roomEvent = _events.Dequeue();
            return true;
        }

        // ---- Requests ----

        public void RefreshRooms() => Send(RoomMessages.Simple(RoomMessage.ListRooms));
        public void CreateRoom(RoomSettings settings, string password) => Send(RoomMessages.CreateRoom(settings, password));
        public void JoinRoom(uint roomId, string password) => Send(RoomMessages.JoinRoom(roomId, "", password));
        public void JoinByCode(string code, string password) => Send(RoomMessages.JoinRoom(0, code, password));
        public void LeaveRoom() => Send(RoomMessages.Simple(RoomMessage.LeaveRoom));
        public void SetMember(LoadoutCode loadout, bool queued) => Send(RoomMessages.SetMember(loadout, queued));
        public void SendChat(string text)
        {
            text = ChatLine.Clean(text);
            if (text.Length > 0) Send(RoomMessages.Chat(text));
        }
        public void UpdateSettings(RoomSettings settings) => Send(RoomMessages.UpdateSettings(settings));
        public void Kick(uint memberId) => Send(RoomMessages.Kick(memberId));

        public void Spectate(uint matchId)
        {
            _wantedSpectate = matchId;
            if (Spectating != null) Spectating.Ended = true;
            Spectating = null;
            Send(RoomMessages.Spectate(matchId));
        }

        public void PublishStart(uint matchId, MatchStart start) => Send(RoomMessages.PublishStart(matchId, start));

        public bool PublishFrames(uint matchId, VersusReplay replay, ref int sent)
        {
            if (State != RoomClientState.Connected || _reliable.PendingCount >= 32 || sent >= replay.TickCount) return false;
            // Share the existing recording without keeping a second copy of fighter inputs.
            byte[] data = SpectatorStream.EncodeFrames(matchId, replay, sent, out int count);
            _reliable.Send(data);
            sent += count;
            return true;
        }

        /// <summary>Reports the fight's outcome from this side. The link stays up for the result screen.</summary>
        /// <summary>Asks to fight the opponent of <paramref name="matchId"/> again, or withdraws that.</summary>
        public void Rematch(uint matchId, bool wanted) => Send(RoomMessages.Rematch(matchId, wanted));

        public void ReportMatch(uint matchId, MatchOutcome outcome, string reason)
        {
            Send(RoomMessages.MatchReport(matchId, outcome, reason));
        }

        /// <summary>Stops routing packets for a finished fight.</summary>
        public void ReleaseLink(uint matchId)
        {
            if (Link != null && Link.MatchId == matchId) Link = null;
        }

        private void Send(byte[] message)
        {
            if (State == RoomClientState.Closed) return;
            // A server that stopped acknowledging times out on its own; never throw into game code.
            try { _reliable.Send(message); }
            catch (InvalidOperationException) { }
        }

        // ---- Pump ----

        public void Update(long nowMs)
        {
            _nowMs = nowMs;
            if (State == RoomClientState.Closed) return;
            _socket.Poll(_handler);
            if (State == RoomClientState.Closed) return;
            if (State == RoomClientState.Connecting)
            {
                if (nowMs - _startedMs > ConnectTimeoutMs) { Close("Could not reach the room server.", false); return; }
                if (_lastHelloMs == long.MinValue || nowMs - _lastHelloMs >= HelloRetryMs)
                {
                    _lastHelloMs = nowMs;
                    RoomProtocol.WriteHeader(_writer, RoomProtocol.KindHello, _token);
                    _identity.Write(_writer);
                    _writer.U8((byte)_lanEndPoints.Count);
                    foreach (var endPoint in _lanEndPoints) RoomProtocol.WriteEndPoint(_writer, endPoint);
                    _writer.U32(_cookie);
                    _socket.Send(Server, _writer.Buffer, _writer.Length);
                }
                return;
            }
            if (nowMs - LastReceiveMs > TimeoutMs) { Close("Lost connection to the room server.", false); return; }
            if (nowMs - _windowReceivedMs >= PlaytestWindow.LeaseMs) { Close("Lost connection to the playtest server. Reconnect to continue.", false); return; }
            if (Window.Scheduled && ServerUnixMs >= Window.EndsUnixMs) { Close("Playtest is over."); return; }
            if (_lastSendMs == long.MinValue || nowMs - _lastSendMs >= KeepAliveMs ||
                _reliable.PendingCount > 0 && nowMs - _lastSendMs >= ReliableChannel.ResendMs)
                Flush(nowMs);
            Link?.Update(nowMs);
        }

        private void Flush(long nowMs)
        {
            RoomProtocol.WriteHeader(_writer, RoomProtocol.KindData, _token);
            _reliable.Write(_writer, nowMs, 0);
            _socket.Send(Server, _writer.Buffer, _writer.Length);
            _lastSendMs = nowMs;
        }

        public void Close(string reason, bool notifyServer = true)
        {
            if (State == RoomClientState.Closed) return;
            if (notifyServer && State == RoomClientState.Connected)
            {
                RoomProtocol.WriteHeader(_writer, RoomProtocol.KindBye, _token);
                _writer.Str(reason ?? "");
                for (int i = 0; i < 2; i++) _socket.Send(Server, _writer.Buffer, _writer.Length);
            }
            CloseReason = reason ?? string.Empty;
            if (Spectating != null) { Spectating.Ended = true; Spectating.EndReason = CloseReason; }
            State = RoomClientState.Closed;
        }

        public void Dispose()
        {
            Close("Left online play.");
            _disposed = true;
            _socket.Dispose();
        }

        // ---- Receive ----

        private void OnDatagram(EndPoint from, byte[] buffer, int length)
        {
            if (RoomProtocol.IsServerPacket(buffer, length))
            {
                if (from.Equals(Server)) HandleServer(buffer, length);
                return;
            }
            if (RoomProtocol.IsPunchPacket(buffer, length)) { Link?.HandlePunch((IPEndPoint)from, buffer, length); return; }
            if (RoomProtocol.IsNetplayPacket(buffer, length)) Link?.DeliverDirect((IPEndPoint)from, buffer, length);
        }

        private void HandleServer(byte[] buffer, int length)
        {
            var reader = new NetReader(buffer, 0, length);
            reader.U8(); reader.U8();
            byte version = reader.U8();
            byte kind = reader.U8();
            uint token = reader.U32();
            if (token != _token) return;
            if (version != RoomProtocol.Version)
            {
                if (kind == RoomProtocol.KindReject) Close(reader.Str(), false);
                return;
            }
            switch (kind)
            {
                case RoomProtocol.KindChallenge:
                    if (State != RoomClientState.Connecting) return;
                    _cookie = reader.U32();
                    _lastHelloMs = long.MinValue; // answer straight away
                    break;
                case RoomProtocol.KindWelcome:
                    if (State != RoomClientState.Connecting) return;
                    ClientId = reader.U32();
                    PublicEndPoint = RoomProtocol.ReadEndPoint(reader);
                    if (!ReadWindow(reader)) return;
                    State = RoomClientState.Connected;
                    LastReceiveMs = _nowMs;
                    Flush(_nowMs);
                    break;
                case RoomProtocol.KindReject:
                    Close(reader.Str(), false);
                    break;
                case RoomProtocol.KindBye:
                    Close(reader.Str(), false);
                    break;
                case RoomProtocol.KindData:
                    if (State != RoomClientState.Connected) return;
                    LastReceiveMs = _nowMs;
                    _reliable.Read(reader);
                    while (_reliable.TryReceive(out var message)) HandleMessage(message);
                    break;
                case RoomProtocol.KindPing:
                    if (State != RoomClientState.Connected) return;
                    LastReceiveMs = _nowMs;
                    uint stamp = reader.U32();
                    if (!ReadWindow(reader)) return;
                    RoomProtocol.WriteHeader(_writer, RoomProtocol.KindPong, _token);
                    _writer.U32(stamp);
                    _socket.Send(Server, _writer.Buffer, _writer.Length);
                    break;
                case RoomProtocol.KindRelay:
                    if (State != RoomClientState.Connected) return;
                    LastReceiveMs = _nowMs;
                    uint matchId = reader.U32();
                    if (Link != null && Link.MatchId == matchId) Link.DeliverRelayed(buffer, reader.Position, reader.Remaining);
                    break;
            }
        }

        private bool ReadWindow(NetReader reader)
        {
            try
            {
                long previousUnixMs = Window == null ? 0 : ServerUnixMs;
                Window = PlaytestWindow.Read(reader, out long serverUnixMs);
                // Delayed or reordered heartbeats must not rewind the known expiry clock.
                _serverUnixMs = Math.Max(previousUnixMs, serverUnixMs);
                _windowReceivedMs = _nowMs;
                return true;
            }
            catch (NetFormatException) { Close("The playtest server sent an invalid schedule.", false); return false; }
        }

        private void HandleMessage(byte[] message)
        {
            var reader = new NetReader(message, 1, message.Length - 1);
            try
            {
                switch ((RoomMessage)message[0])
                {
                    case RoomMessage.Error:
                        _events.Enqueue(new RoomEvent { Type = RoomEventType.Error, Text = reader.Str() });
                        break;
                    case RoomMessage.RoomList:
                        RoomMessages.ReadRoomList(reader, Rooms, out _, out bool last);
                        if (last) _events.Enqueue(new RoomEvent { Type = RoomEventType.RoomListUpdated });
                        break;
                    case RoomMessage.RoomState:
                        Room = RoomState.Decode(reader);
                        _events.Enqueue(new RoomEvent { Type = RoomEventType.RoomChanged });
                        break;
                    case RoomMessage.LeftRoom:
                        if (Spectating != null) { Spectating.Ended = true; Spectating.EndReason = "Left the room."; }
                        _wantedSpectate = 0;
                        Room = null;
                        Link = null;
                        Chat.Clear();
                        _events.Enqueue(new RoomEvent { Type = RoomEventType.LeftRoom, Text = reader.Str() });
                        break;
                    case RoomMessage.ChatLine:
                        var line = ChatLine.Decode(reader);
                        if (line.Text.Length == 0) break;
                        Chat.Add(line);
                        if (Chat.Count > ChatHistory) Chat.RemoveRange(0, Chat.Count - ChatHistory);
                        _events.Enqueue(new RoomEvent { Type = RoomEventType.Chat, Chat = line });
                        break;
                    case RoomMessage.Pairing:
                        _wantedSpectate = 0;
                        if (Spectating != null) Spectating.Ended = true;
                        Spectating = null;
                        var pairing = RoomPairing.Decode(reader);
                        Link = new MatchLink(this, pairing, ForceRelay, _nowMs);
                        _events.Enqueue(new RoomEvent { Type = RoomEventType.Paired, Pairing = pairing });
                        break;
                    case RoomMessage.SpectatorStart:
                        var stream = SpectatorStream.DecodeStart(reader);
                        if (stream.MatchId != _wantedSpectate || Room == null) break;
                        Spectating = stream;
                        _events.Enqueue(new RoomEvent { Type = RoomEventType.Spectating });
                        break;
                    case RoomMessage.SpectatorFrames:
                        uint watched = reader.U32();
                        if (Spectating?.MatchId == watched) Spectating.ReadFrames(reader);
                        break;
                    case RoomMessage.SpectatorEnd:
                        uint ended = reader.U32();
                        string reason = reader.Str();
                        if (Spectating?.MatchId == ended) { Spectating.Ended = true; Spectating.EndReason = reason; }
                        break;
                }
            }
            catch (NetFormatException) { /* Ignore a malformed server message. */ }
        }

        // ---- Used by MatchLink ----

        internal void PollSocket()
        {
            if (!_disposed) _socket.Poll(_handler);
        }

        private bool _disposed;

        internal void SendRaw(IPEndPoint to, byte[] data, int length) => _socket.Send(to, data, length);

        internal void SendRelayed(uint matchId, byte[] data, int length)
        {
            var writer = new NetWriter(NetProtocol.MaxPacketSize + 16);
            RoomProtocol.WriteHeader(writer, RoomProtocol.KindRelay, _token);
            writer.U32(matchId);
            writer.Bytes(data, 0, length);
            _socket.Send(Server, writer.Buffer, writer.Length);
        }
    }

    public enum LinkPath { Punching, Direct, Relay }

    /// <summary>
    /// The path between two paired players. Starts by hole-punching every candidate
    /// address; if no probe gets through in time, traffic goes through the server's
    /// relay instead. Either way it is an <see cref="INetTransport"/> for the fight.
    /// </summary>
    public sealed class MatchLink : INetTransport
    {
        public const int PunchIntervalMs = 100;
        public const int PunchTimeoutMs = 2500;

        private readonly RoomClient _client;
        private readonly RoomPairing _pairing;
        private readonly NetWriter _writer = new NetWriter(64);
        private readonly Queue<byte[]> _inbox = new Queue<byte[]>();
        private readonly long _startedMs;
        private readonly bool _forceRelay;
        private long _lastPunchMs = long.MinValue;
        /// <summary>Confirmed both ways: our probe was acknowledged from this address.</summary>
        private IPEndPoint _direct;
        /// <summary>Every address the opponent's valid probes came from; their packets are accepted from any.</summary>
        private readonly List<IPEndPoint> _seen = new List<IPEndPoint>();

        /// <summary>A stand-in address for the opponent; the link decides how to reach them.</summary>
        public static readonly IPEndPoint PeerEndPoint = new IPEndPoint(IPAddress.Parse("0.0.0.1"), 1);

        public uint MatchId => _pairing.MatchId;
        public RoomPairing Pairing => _pairing;
        public LinkPath Path { get; private set; } = LinkPath.Punching;
        /// <summary>The fight can start once the path is decided.</summary>
        public bool IsReady => Path != LinkPath.Punching;
        public int LocalPort => _client.LocalPort;

        internal MatchLink(RoomClient client, RoomPairing pairing, bool forceRelay, long nowMs)
        {
            _client = client;
            _pairing = pairing;
            _startedMs = nowMs;
            _forceRelay = forceRelay;
            if (forceRelay || pairing.Candidates.Count == 0) Path = LinkPath.Relay;
        }

        internal void Update(long nowMs)
        {
            if (_forceRelay) return;
            if (Path == LinkPath.Punching && nowMs - _startedMs >= PunchTimeoutMs) Path = LinkPath.Relay;
            // Keep probing a while after falling back, so a slow NAT can still upgrade to direct.
            bool probing = Path == LinkPath.Punching || (_direct == null && nowMs - _startedMs < PunchTimeoutMs * 4);
            if (probing)
            {
                if (_lastPunchMs != long.MinValue && nowMs - _lastPunchMs < PunchIntervalMs) return;
                _lastPunchMs = nowMs;
                RoomProtocol.WritePunch(_writer, RoomProtocol.PunchKindPing, _pairing.MatchId, _client.ClientId, _pairing.Secret);
                foreach (var candidate in _pairing.Candidates) _client.SendRaw(candidate, _writer.Buffer, _writer.Length);
            }
        }

        internal void HandlePunch(IPEndPoint from, byte[] buffer, int length)
        {
            if (_forceRelay) return;
            var reader = new NetReader(buffer, 0, length);
            reader.U8(); reader.U8();
            if (reader.U8() != RoomProtocol.Version) return;
            byte kind = reader.U8();
            if (reader.U32() != _pairing.MatchId || reader.U32() != _pairing.PeerId || reader.U32() != _pairing.Secret) return;
            if (!_seen.Contains(from) && _seen.Count < 8) _seen.Add(from);
            if (kind == RoomProtocol.PunchKindPing)
            {
                // Their probe reached us; answer so they learn their direction works too.
                RoomProtocol.WritePunch(_writer, RoomProtocol.PunchKindAck, _pairing.MatchId, _client.ClientId, _pairing.Secret);
                _client.SendRaw(from, _writer.Buffer, _writer.Length);
            }
            else if (_direct == null)
            {
                // An answer to our own probe: packets get through in both directions.
                _direct = from;
                Path = LinkPath.Direct;
            }
        }

        internal void DeliverDirect(IPEndPoint from, byte[] buffer, int length)
        {
            // Accept fight packets only from where the opponent's probes came from.
            if (!from.Equals(_direct) && !_seen.Contains(from)) return;
            Enqueue(buffer, 0, length);
        }

        internal void DeliverRelayed(byte[] buffer, int offset, int length)
        {
            if (RoomProtocol.IsNetplayPacketAt(buffer, offset, length)) Enqueue(buffer, offset, length);
        }

        private void Enqueue(byte[] buffer, int offset, int length)
        {
            if (_inbox.Count > 512) return;
            var copy = new byte[length];
            Array.Copy(buffer, offset, copy, 0, length);
            _inbox.Enqueue(copy);
        }

        public void Poll(Action<EndPoint, byte[], int> handler)
        {
            // The room client owns the socket; drain it first so fresh packets are included.
            _client.PollSocket();
            while (_inbox.Count > 0)
            {
                var packet = _inbox.Dequeue();
                try { handler(PeerEndPoint, packet, packet.Length); }
                catch (NetFormatException) { }
            }
        }

        public void Send(EndPoint to, byte[] data, int length)
        {
            if (Path == LinkPath.Direct && _direct != null) _client.SendRaw(_direct, data, length);
            else _client.SendRelayed(_pairing.MatchId, data, length);
        }

        /// <summary>The room client owns the socket; nothing to release.</summary>
        public void Dispose() { }
    }
}
