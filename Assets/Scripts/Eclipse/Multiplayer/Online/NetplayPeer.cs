using System;
using System.Net;
using System.Net.Sockets;

namespace Eclipse.Multiplayer.Online
{
    public enum NetplayState
    {
        /// <summary>Host socket is open and no guest has been accepted yet.</summary>
        WaitingForGuest,
        /// <summary>Guest is sending connect requests to the host.</summary>
        Connecting,
        Connected,
        Closed,
    }

    /// <summary>
    /// One side of a two-player UDP session: handshake, keep-alive, round-trip time,
    /// reliable messages and the lockstep input stream. Single-threaded; call
    /// <see cref="Update"/> regularly from the main thread.
    /// </summary>
    public sealed class NetplayPeer : IDisposable
    {
        public const int ConnectRetryMs = 250;
        public const int ConnectTimeoutMs = 10000;
        public const int KeepAliveMs = 100;
        public const int TimeoutMs = 20000;

        private readonly System.Net.Sockets.Socket _socket;
        private readonly NetWriter _writer = new NetWriter();
        private readonly byte[] _receiveBuffer = new byte[2048];
        private readonly NetIdentity _identity;
        private ReliableChannel _reliable = new ReliableChannel();
        private EndPoint _remote;
        private uint _token;
        private long _startedMs;
        private long _lastSendMs = long.MinValue;
        private long _lastConnectMs = long.MinValue;
        private uint _lastPeerStamp;
        private long _peerStampReceivedMs;
        private bool _hasPeerStamp;
        private long _clockOriginMs = -1;

        public bool IsHost { get; }
        public NetplayState State { get; private set; }
        public string CloseReason { get; private set; } = string.Empty;
        public NetIdentity RemoteIdentity { get; private set; }
        public int LocalPort => ((IPEndPoint)_socket.LocalEndPoint).Port;
        public EndPoint RemoteEndPoint => _remote;
        /// <summary>Smoothed round-trip time in milliseconds, or -1 before the first sample.</summary>
        public int RttMs { get; private set; } = -1;
        public long LastReceiveMs { get; private set; }
        public LockstepTimeline Timeline { get; set; }
        /// <summary>Set once a well-formed packet from the peer has been processed this session.</summary>
        public bool HasHeardFromPeer { get; private set; }
        /// <summary>Test aid: extra one-way delay added to every outgoing datagram.</summary>
        public int SimulatedLatencyMs { get; set; }
        /// <summary>Test aid: percentage (0-100) of outgoing datagrams to drop.</summary>
        public int SimulatedLossPercent { get; set; }

        private readonly System.Collections.Generic.List<(long due, EndPoint to, byte[] data)> _delayed =
            new System.Collections.Generic.List<(long, EndPoint, byte[])>();
        private readonly Random _simulationRandom = new Random(7);
        private long _nowMs;

        private NetplayPeer(bool host, NetIdentity identity, int port, long nowMs)
        {
            IsHost = host;
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            _socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp) { Blocking = false };
            DisableConnectionReset(_socket);
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
            _startedMs = nowMs;
            LastReceiveMs = nowMs;
            State = host ? NetplayState.WaitingForGuest : NetplayState.Connecting;
        }

        public static NetplayPeer Host(int port, NetIdentity identity, long nowMs)
        {
            return new NetplayPeer(true, identity, port, nowMs);
        }

        public static NetplayPeer Join(IPEndPoint host, NetIdentity identity, long nowMs)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));
            var peer = new NetplayPeer(false, identity, 0, nowMs)
            {
                _remote = host,
                _token = (uint)new Random().Next(1, int.MaxValue),
            };
            return peer;
        }

        /// <summary>Parses "host", "host:port", "[v6]:port" is not supported; IPv4 and DNS names are.</summary>
        public static bool TryParseAddress(string text, out IPEndPoint endPoint, out string error)
        {
            endPoint = null;
            error = null;
            text = (text ?? string.Empty).Trim();
            if (text.Length == 0) { error = "Enter the host's address."; return false; }
            int port = NetProtocol.DefaultPort;
            string hostName = text;
            int colon = text.LastIndexOf(':');
            if (colon >= 0)
            {
                hostName = text.Substring(0, colon);
                if (!int.TryParse(text.Substring(colon + 1), out port) || port < 1 || port > 65535)
                { error = "The port must be a number from 1 to 65535."; return false; }
            }
            if (!IPAddress.TryParse(hostName, out var address))
            {
                try
                {
                    address = Array.Find(Dns.GetHostAddresses(hostName), a => a.AddressFamily == AddressFamily.InterNetwork);
                }
                catch (Exception) { address = null; }
                if (address == null) { error = "Could not find host \"" + hostName + "\"."; return false; }
            }
            endPoint = new IPEndPoint(address, port);
            return true;
        }

        public void SendReliable(byte[] message)
        {
            if (State == NetplayState.Closed) return;
            _reliable.Send(message);
        }

        public bool TryReceiveReliable(out byte[] message) => _reliable.TryReceive(out message);

        /// <summary>Receives everything pending, then sends a packet if one is due.</summary>
        public void Update(long nowMs)
        {
            _nowMs = nowMs;
            ReleaseDelayed(nowMs);
            if (State == NetplayState.Closed) return;
            Receive(nowMs);
            if (State == NetplayState.Closed) return;
            if (State == NetplayState.Connecting)
            {
                if (nowMs - _startedMs > ConnectTimeoutMs) { Close("Could not reach the host. Check the address, port and firewall.", false); return; }
                if (_lastConnectMs == long.MinValue || nowMs - _lastConnectMs >= ConnectRetryMs)
                {
                    _lastConnectMs = nowMs;
                    NetProtocol.WriteHeader(_writer, NetProtocol.KindConnect, _token);
                    _identity.Write(_writer);
                    SendRaw();
                }
                return;
            }
            if (State != NetplayState.Connected) return;
            if (nowMs - LastReceiveMs > TimeoutMs) { Close("The connection timed out.", false); return; }
            if (_lastSendMs == long.MinValue || nowMs - _lastSendMs >= KeepAliveMs || _reliable.PendingCount > 0 && nowMs - _lastSendMs >= ReliableChannel.ResendMs)
                Flush(nowMs);
        }

        /// <summary>Sends a data packet immediately (called every simulation tick during a match).</summary>
        public void Flush(long nowMs)
        {
            _nowMs = nowMs;
            if (State != NetplayState.Connected) return;
            NetProtocol.WriteHeader(_writer, NetProtocol.KindData, _token);
            // Timestamp echo: our clock, the peer's last clock, and how long we held it.
            _writer.U32(Stamp(nowMs));
            _writer.U32(_hasPeerStamp ? _lastPeerStamp : 0u);
            _writer.U16(_hasPeerStamp ? (ushort)Math.Min(ushort.MaxValue, nowMs - _peerStampReceivedMs) : ushort.MaxValue);
            var timeline = Timeline;
            _reliable.Write(_writer, nowMs, timeline != null ? 160 : 1);
            _writer.Bool(timeline != null);
            timeline?.WriteSync(_writer);
            SendRaw();
            _lastSendMs = nowMs;
        }

        public void Close(string reason, bool notifyPeer = true)
        {
            if (State == NetplayState.Closed) return;
            if (notifyPeer && _remote != null && State == NetplayState.Connected)
            {
                for (int i = 0; i < 3; i++)
                {
                    NetProtocol.WriteHeader(_writer, NetProtocol.KindDisconnect, _token);
                    _writer.Str(Truncate(reason));
                    SendRaw();
                }
            }
            CloseReason = reason ?? string.Empty;
            State = NetplayState.Closed;
        }

        public void Dispose()
        {
            Close("Left the session.");
            _socket.Close();
        }

        private void Receive(long nowMs)
        {
            for (int guard = 0; guard < 256; guard++)
            {
                EndPoint from = new IPEndPoint(IPAddress.Any, 0);
                int length;
                try
                {
                    if (_socket.Available <= 0) return;
                    length = _socket.ReceiveFrom(_receiveBuffer, ref from);
                }
                catch (SocketException exception) when (exception.SocketErrorCode == SocketError.WouldBlock ||
                    exception.SocketErrorCode == SocketError.ConnectionReset || exception.SocketErrorCode == SocketError.MessageSize)
                {
                    continue;
                }
                try { Handle(from, length, nowMs); }
                catch (NetFormatException) { /* Malformed datagram; drop it. */ }
                if (State == NetplayState.Closed) return;
            }
        }

        private void Handle(EndPoint from, int length, long nowMs)
        {
            var reader = new NetReader(_receiveBuffer, 0, length);
            if (!NetProtocol.TryReadHeader(reader, out var version, out var kind, out var token)) return;
            if (IsHost && kind == NetProtocol.KindConnect) { HandleConnect(from, version, token, reader, nowMs); return; }
            if (_remote == null || !from.Equals(_remote) || token != _token) return;
            if (version != NetProtocol.Version) return;
            switch (kind)
            {
                case NetProtocol.KindAccept when !IsHost:
                    RemoteIdentity = NetIdentity.Read(reader);
                    if (State == NetplayState.Connecting)
                    {
                        State = NetplayState.Connected;
                        MarkHeard(nowMs);
                        Flush(nowMs);
                    }
                    break;
                case NetProtocol.KindReject when !IsHost:
                    Close(reader.Str(), false);
                    break;
                case NetProtocol.KindDisconnect:
                    string reason = reader.Str();
                    Close("The other player left" + (string.IsNullOrEmpty(reason) ? "." : ": " + reason), false);
                    break;
                case NetProtocol.KindData:
                    if (State != NetplayState.Connected) return;
                    ReadData(reader, nowMs);
                    MarkHeard(nowMs);
                    break;
            }
        }

        private void HandleConnect(EndPoint from, byte version, uint token, NetReader reader, long nowMs)
        {
            if (State == NetplayState.Connected && _remote != null)
            {
                // A resend from our own guest whose accept was lost.
                if (from.Equals(_remote) && token == _token) SendAccept(from, token);
                else SendReject(from, token, "The host is already in a match.");
                return;
            }
            if (State != NetplayState.WaitingForGuest) return;
            if (version != NetProtocol.Version) { SendReject(from, token, "Online protocol versions differ. Update the game."); return; }
            var identity = NetIdentity.Read(reader);
            string problem = _identity.Incompatibility(identity);
            if (problem != null) { SendReject(from, token, problem); return; }
            _remote = from;
            _token = token;
            RemoteIdentity = identity;
            _reliable = new ReliableChannel();
            State = NetplayState.Connected;
            MarkHeard(nowMs);
            SendAccept(from, token);
        }

        private void ReadData(NetReader reader, long nowMs)
        {
            uint peerStamp = reader.U32();
            uint echo = reader.U32();
            ushort held = reader.U16();
            _lastPeerStamp = peerStamp;
            _peerStampReceivedMs = nowMs;
            _hasPeerStamp = true;
            if (held != ushort.MaxValue && echo != 0)
            {
                long sample = Stamp(nowMs) - (long)echo - held;
                if (sample >= 0 && sample < 5000)
                    RttMs = RttMs < 0 ? (int)sample : (int)(RttMs * 0.875 + sample * 0.125);
            }
            _reliable.Read(reader);
            if (reader.Bool()) Timeline?.ReadSync(reader);
        }

        private void MarkHeard(long nowMs)
        {
            LastReceiveMs = nowMs;
            HasHeardFromPeer = true;
        }

        private void SendAccept(EndPoint to, uint token)
        {
            NetProtocol.WriteHeader(_writer, NetProtocol.KindAccept, token);
            _identity.Write(_writer);
            SendRawTo(to);
        }

        private void SendReject(EndPoint to, uint token, string reason)
        {
            NetProtocol.WriteHeader(_writer, NetProtocol.KindReject, token);
            _writer.Str(Truncate(reason));
            SendRawTo(to);
        }

        private void SendRaw() => SendRawTo(_remote);

        private void SendRawTo(EndPoint to)
        {
            if (to == null) return;
            if (SimulatedLossPercent > 0 && _simulationRandom.Next(100) < SimulatedLossPercent) return;
            if (SimulatedLatencyMs > 0)
            {
                _delayed.Add((_nowMs + SimulatedLatencyMs, to, _writer.ToArray()));
                return;
            }
            SendBytes(to, _writer.Buffer, _writer.Length);
        }

        private void SendBytes(EndPoint to, byte[] data, int length)
        {
            try { _socket.SendTo(data, 0, length, SocketFlags.None, to); }
            catch (SocketException) { /* Transient network failures surface as timeouts. */ }
            catch (ObjectDisposedException) { }
        }

        private void ReleaseDelayed(long nowMs)
        {
            int released = 0;
            while (released < _delayed.Count && _delayed[released].due <= nowMs)
            {
                SendBytes(_delayed[released].to, _delayed[released].data, _delayed[released].data.Length);
                released++;
            }
            if (released > 0) _delayed.RemoveRange(0, released);
        }

        private uint Stamp(long nowMs)
        {
            if (_clockOriginMs < 0) _clockOriginMs = nowMs - 1;
            // Never zero, so zero can mean "no echo yet".
            return (uint)Math.Max(1, nowMs - _clockOriginMs);
        }

        private static string Truncate(string text)
        {
            text = text ?? string.Empty;
            return text.Length > 120 ? text.Substring(0, 120) : text;
        }

        private static void DisableConnectionReset(System.Net.Sockets.Socket socket)
        {
            // Windows reports ICMP port-unreachable as ConnectionReset on later receives.
            if (Environment.OSVersion.Platform != PlatformID.Win32NT) return;
            try
            {
                const int SioUdpConnReset = -1744830452;
                socket.IOControl(SioUdpConnReset, new byte[] { 0 }, null);
            }
            catch (Exception) { }
        }
    }
}
