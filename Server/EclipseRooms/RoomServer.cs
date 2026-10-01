using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Eclipse.Multiplayer.Online;
using Eclipse.Multiplayer.Online.Rooms;

namespace Eclipse.RoomServer
{
    /// <summary>
    /// The room/rendezvous server. It keeps rooms, queues and results, tells paired
    /// players each other's addresses so they can hole-punch, and relays fight packets
    /// for pairs that cannot connect directly. It never simulates or inspects a fight.
    /// Single-threaded; call <see cref="Update"/> in a loop.
    /// </summary>
    public sealed class RoomServer : IDisposable
    {
        public const int ClientTimeoutMs = 20000;
        public const int KeepAliveMs = 1000;
        public const int MaxClients = 4000;
        public const int MaxRooms = 1000;
        public const int RelayBytesPerSecond = 48 * 1024;
        public const int MatchTimeoutMs = 30 * 60 * 1000;
        public const int ChampionWaitMs = 30000;
        public const int MaxClientsPerAddress = 8;
        /// <summary>After one player reports, the other has this long before the match resolves anyway.</summary>
        public const int SecondReportWaitMs = 45000;
        /// <summary>Relay keeps forwarding a finished match briefly, for the final result/disconnect packets.</summary>
        public const int RelayGraceMs = 10000;
        public const int MaxFailedJoins = 5;
        public const int JoinLockoutMs = 30000;
        /// <summary>How often each client's ping is measured.</summary>
        public const int PingIntervalMs = 1000;
        /// <summary>A ping change alone updates the room at most this often.</summary>
        public const int PingBroadcastMs = 2000;
        /// <summary>A ping has to move this much before the room hears about it.</summary>
        public const int PingChangeMs = 10;
        /// <summary>Silence after which a member shows as stale.</summary>
        public const int StaleMs = 3000;
        /// <summary>Chat: messages a member may send at once, and how fast the allowance refills.</summary>
        public const int ChatBurst = 4;
        public const int ChatRefillMs = 1500;

        private sealed class Client
        {
            public uint Id, Token;
            public IPEndPoint EndPoint;
            public NetIdentity Identity;
            public readonly List<IPEndPoint> Lan = new List<IPEndPoint>();
            public readonly ReliableChannel Reliable = new ReliableChannel();
            public long LastReceiveMs, LastSendMs = long.MinValue;
            public Room Room;
            public RoomMember Member;
            public Match Match;
            public long RelayWindowMs;
            public int RelayBytes;
            /// <summary>Asked to queue while still marked in a fight; applied when the fight resolves.</summary>
            public bool QueueAfterMatch;
            public Match LastMatch;
            public long LastMatchEndedMs;
            public int FailedJoins;
            public long JoinLockedUntilMs;
            public long LastPingSentMs = long.MinValue;
            /// <summary>Smoothed round trip, or -1 before the first answer.</summary>
            public float PingMs = -1;
            public float ChatTokens = ChatBurst;
            public long ChatRefilledMs;
            public Match Watching;
            public int WatchTick;
        }

        private sealed class Room
        {
            public uint Id;
            public string Code, Password;
            public RoomSettings Settings;
            public string Build, Content;
            public readonly List<Client> Members = new List<Client>();
            public readonly List<uint> Queue = new List<uint>();
            public uint HostId, ChampionId;
            public int Streak;
            public readonly List<Match> Matches = new List<Match>();
            public long ChampionAwaySinceMs = -1;
            public long LastBroadcastMs;
        }

        private sealed class Match
        {
            public uint Id, Secret;
            public Room Room;
            public Client Left, Right;
            public MatchOutcome? LeftReport, RightReport;
            public bool LeftGone, RightGone;
            public long StartedMs;
            public long FirstReportMs = -1;
            /// <summary>Some of this fight's packets went through the relay.</summary>
            public bool Relayed;
            public int Seed;
            public RoomSettings Settings;
            public LoadoutCode LeftLoadout, RightLoadout;
            public SpectatorStream Stream;
        }

        private readonly UdpTransport _socket;
        private readonly NetWriter _writer = new NetWriter(NetProtocol.MaxPacketSize + 32);
        private readonly Dictionary<IPEndPoint, Client> _byEndPoint = new Dictionary<IPEndPoint, Client>();
        private readonly Dictionary<uint, Room> _rooms = new Dictionary<uint, Room>();
        private readonly Dictionary<string, Room> _byCode = new Dictionary<string, Room>(StringComparer.OrdinalIgnoreCase);
        private readonly Random _random = new Random();
        private readonly List<Client> _pendingDrops = new List<Client>();
        private readonly List<Client> _scratchClients = new List<Client>();
        private readonly List<Room> _scratchRooms = new List<Room>();
        private readonly byte[] _cookieKey = new byte[32];
        private readonly Action<EndPoint, byte[], int> _handler;
        private uint _nextClientId = 1, _nextRoomId = 1, _nextMatchId = 1;
        private long _nowMs;

        public Action<string> Log { get; set; } = _ => { };
        public int Port => _socket.LocalPort;
        public int ClientCount => _byEndPoint.Count;
        public int RoomCount => _rooms.Count;

        public RoomServer(int port)
        {
            _socket = new UdpTransport(port);
            _handler = OnDatagram;
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create()) rng.GetBytes(_cookieKey);
        }

        public void Dispose() => _socket.Dispose();

        public void Update(long nowMs)
        {
            _nowMs = nowMs;
            _socket.Poll(_handler);
            DropPending();
            _scratchClients.Clear();
            _scratchClients.AddRange(_byEndPoint.Values);
            foreach (var client in _scratchClients)
            {
                if (nowMs - client.LastReceiveMs > ClientTimeoutMs) { Drop(client, "timed out"); continue; }
                PumpSpectator(client);
                if (client.LastSendMs == long.MinValue || nowMs - client.LastSendMs >= KeepAliveMs ||
                    client.Reliable.PendingCount > 0 && nowMs - client.LastSendMs >= ReliableChannel.ResendMs)
                    Flush(client);
                if (client.LastPingSentMs == long.MinValue || nowMs - client.LastPingSentMs >= PingIntervalMs) SendPing(client);
            }
            DropPending();
            _scratchRooms.Clear();
            _scratchRooms.AddRange(_rooms.Values);
            foreach (var room in _scratchRooms)
            {
                if (!_rooms.ContainsKey(room.Id)) continue;
                foreach (var match in room.Matches.ToArray())
                    if (nowMs - match.StartedMs > MatchTimeoutMs ||
                        match.FirstReportMs >= 0 && nowMs - match.FirstReportMs > SecondReportWaitMs)
                        Resolve(match, force: true);
                // Pings and stale flags change often; members hear about it only when it matters.
                if (nowMs - room.LastBroadcastMs >= PingBroadcastMs && LinkChanged(room)) Broadcast(room);
                if (room.ChampionAwaySinceMs >= 0 && nowMs - room.ChampionAwaySinceMs > ChampionWaitMs)
                {
                    // A champion who never came back gives up the spot.
                    room.ChampionId = 0;
                    room.Streak = 0;
                    room.ChampionAwaySinceMs = -1;
                    TryPair(room);
                    Broadcast(room);
                }
            }
            DropPending();
        }

        private void DropPending()
        {
            while (_pendingDrops.Count > 0)
            {
                var client = _pendingDrops[_pendingDrops.Count - 1];
                _pendingDrops.RemoveAt(_pendingDrops.Count - 1);
                Drop(client, "stopped acknowledging");
            }
        }

        // ---- Transport ----

        private void OnDatagram(EndPoint from, byte[] buffer, int length)
        {
            // One bad packet or client must never take the server down.
            try { HandleDatagram(from, buffer, length); }
            catch (NetFormatException) { }
            catch (Exception exception) { Log("error handling a packet from " + from + ": " + exception); }
        }

        private void HandleDatagram(EndPoint from, byte[] buffer, int length)
        {
            if (!RoomProtocol.IsServerPacket(buffer, length)) return;
            var endPoint = (IPEndPoint)from;
            var reader = new NetReader(buffer, 0, length);
            reader.U8(); reader.U8();
            byte version = reader.U8();
            byte kind = reader.U8();
            uint token = reader.U32();
            if (kind == RoomProtocol.KindHello) { HandleHello(endPoint, version, token, reader); return; }
            if (!_byEndPoint.TryGetValue(endPoint, out var client) || client.Token != token) return;
            client.LastReceiveMs = _nowMs;
            switch (kind)
            {
                case RoomProtocol.KindData:
                    client.Reliable.Read(reader);
                    while (client.Reliable.TryReceive(out var message) && _byEndPoint.ContainsKey(endPoint)) HandleMessage(client, message);
                    break;
                case RoomProtocol.KindRelay:
                    Relay(client, buffer, reader);
                    break;
                case RoomProtocol.KindPong:
                    ReadPong(client, reader.U32());
                    break;
                case RoomProtocol.KindBye:
                    Drop(client, "left");
                    break;
            }
        }

        private void HandleHello(IPEndPoint from, byte version, uint token, NetReader reader)
        {
            if (version != RoomProtocol.Version)
            {
                SendReject(from, token, "The room server runs a different online version. Update the game.");
                return;
            }
            if (_byEndPoint.TryGetValue(from, out var existing) && existing.Token == token) { SendWelcome(existing); return; }
            var identity = NetIdentity.Read(reader);
            var lanEndPoints = new List<IPEndPoint>();
            int lan = reader.U8();
            for (int i = 0; i < lan; i++)
            {
                var endPoint = RoomProtocol.ReadEndPoint(reader);
                if (lanEndPoints.Count < RoomProtocol.MaxCandidates - 1) lanEndPoints.Add(endPoint);
            }
            uint cookie = reader.Remaining >= 4 ? reader.U32() : 0;
            // No state and no reply larger than the request until the sender proves it receives at this address.
            if (!CookieValid(from, token, cookie))
            {
                RoomProtocol.WriteHeader(_writer, RoomProtocol.KindChallenge, token);
                _writer.U32(Cookie(from, token, _nowMs / 60000));
                _socket.Send(from, _writer.Buffer, _writer.Length);
                return;
            }
            if (existing != null) Drop(existing, "reconnected");
            if (_byEndPoint.Count >= MaxClients) { SendReject(from, token, "The room server is full. Try again later."); return; }
            int sameAddress = 0;
            foreach (var other in _byEndPoint.Values) if (other.EndPoint.Address.Equals(from.Address)) sameAddress++;
            if (sameAddress >= MaxClientsPerAddress) { SendReject(from, token, "Too many players from this address."); return; }
            var client = new Client { Id = _nextClientId++, Token = token, EndPoint = from, Identity = identity, LastReceiveMs = _nowMs };
            client.Lan.AddRange(lanEndPoints);
            _byEndPoint[from] = client;
            Log("client " + client.Id + " '" + identity.PlayerName + "' from " + from + " (" + identity.Build + ", " + identity.Content + ")");
            SendWelcome(client);
        }

        private uint Cookie(IPEndPoint from, uint token, long minute)
        {
            var data = new byte[4 + 2 + 4 + 8];
            Array.Copy(from.Address.GetAddressBytes(), 0, data, 0, Math.Min(4, from.Address.GetAddressBytes().Length));
            data[4] = (byte)from.Port; data[5] = (byte)(from.Port >> 8);
            BitConverter.GetBytes(token).CopyTo(data, 6);
            BitConverter.GetBytes(minute).CopyTo(data, 10);
            using (var hmac = new System.Security.Cryptography.HMACSHA256(_cookieKey))
            {
                uint value = BitConverter.ToUInt32(hmac.ComputeHash(data), 0);
                return value == 0 ? 1u : value;
            }
        }

        private bool CookieValid(IPEndPoint from, uint token, uint cookie)
        {
            if (cookie == 0) return false;
            long minute = _nowMs / 60000;
            return cookie == Cookie(from, token, minute) || cookie == Cookie(from, token, minute - 1);
        }

        private void SendWelcome(Client client)
        {
            RoomProtocol.WriteHeader(_writer, RoomProtocol.KindWelcome, client.Token);
            _writer.U32(client.Id);
            RoomProtocol.WriteEndPoint(_writer, client.EndPoint);
            _socket.Send(client.EndPoint, _writer.Buffer, _writer.Length);
        }

        private void SendReject(IPEndPoint to, uint token, string reason)
        {
            RoomProtocol.WriteHeader(_writer, RoomProtocol.KindReject, token);
            _writer.Str(reason);
            _socket.Send(to, _writer.Buffer, _writer.Length);
        }

        private void Flush(Client client)
        {
            RoomProtocol.WriteHeader(_writer, RoomProtocol.KindData, client.Token);
            client.Reliable.Write(_writer, _nowMs, 0);
            _socket.Send(client.EndPoint, _writer.Buffer, _writer.Length);
            client.LastSendMs = _nowMs;
        }

        private void SendPing(Client client)
        {
            client.LastPingSentMs = _nowMs;
            RoomProtocol.WriteHeader(_writer, RoomProtocol.KindPing, client.Token);
            _writer.U32(unchecked((uint)_nowMs));
            _socket.Send(client.EndPoint, _writer.Buffer, _writer.Length);
        }

        private void ReadPong(Client client, uint stamp)
        {
            uint rtt = unchecked((uint)_nowMs - stamp);
            if (rtt > 10000) return;
            client.PingMs = client.PingMs < 0 ? rtt : client.PingMs * 0.75f + rtt * 0.25f;
        }

        /// <summary>The member's ping and link flags as the room should now see them.</summary>
        private void MeasureLink(Client client, out int pingMs, out MemberLink link)
        {
            pingMs = client.PingMs < 0 ? -1 : (int)Math.Round(client.PingMs);
            link = MemberLink.None;
            if (_nowMs - client.LastReceiveMs > StaleMs) link |= MemberLink.Stale;
            var match = client.Match ?? client.LastMatch;
            if (match != null && match.Relayed) link |= MemberLink.Relayed;
        }

        private bool LinkChanged(Room room)
        {
            foreach (var client in room.Members)
            {
                if (client.Member == null) continue;
                MeasureLink(client, out int ping, out var link);
                if (link != client.Member.Link) return true;
                if ((ping < 0) != (client.Member.PingMs < 0) || Math.Abs(ping - client.Member.PingMs) >= PingChangeMs) return true;
            }
            return false;
        }

        private void Send(Client client, byte[] message)
        {
            try { client.Reliable.Send(message); }
            catch (InvalidOperationException)
            {
                // Dropping here would change room lists mid-iteration; do it after this packet.
                if (!_pendingDrops.Contains(client)) _pendingDrops.Add(client);
            }
        }

        private void Relay(Client from, byte[] buffer, NetReader reader)
        {
            uint matchId = reader.U32();
            var match = from.Match;
            if ((match == null || match.Id != matchId) && from.LastMatch != null && from.LastMatch.Id == matchId &&
                _nowMs - from.LastMatchEndedMs < RelayGraceMs)
                match = from.LastMatch;
            int payload = reader.Remaining;
            if (match == null || match.Id != matchId || payload <= 0 || payload > NetProtocol.MaxPacketSize) return;
            if (_nowMs - from.RelayWindowMs >= 1000) { from.RelayWindowMs = _nowMs; from.RelayBytes = 0; }
            from.RelayBytes += payload;
            if (from.RelayBytes > RelayBytesPerSecond) return;
            var to = match.Left == from ? match.Right : match.Left;
            if (to == null || !_byEndPoint.ContainsKey(to.EndPoint)) return;
            match.Relayed = true;
            RoomProtocol.WriteHeader(_writer, RoomProtocol.KindRelay, to.Token);
            _writer.U32(matchId);
            _writer.Bytes(buffer, reader.Position, payload);
            _socket.Send(to.EndPoint, _writer.Buffer, _writer.Length);
        }

        // ---- Messages ----

        private void HandleMessage(Client client, byte[] message)
        {
            try
            {
                var reader = new NetReader(message, 1, message.Length - 1);
                switch ((RoomMessage)message[0])
                {
                    case RoomMessage.ListRooms: SendRoomList(client); break;
                    case RoomMessage.CreateRoom: CreateRoom(client, RoomSettings.Read(reader), reader.Str()); break;
                    case RoomMessage.JoinRoom: JoinRoom(client, reader.U32(), reader.Str(), reader.Str()); break;
                    case RoomMessage.LeaveRoom: LeaveRoom(client, "You left the room."); break;
                    case RoomMessage.SetMember: SetMember(client, LoadoutCode.Read(reader), reader.Bool()); break;
                    case RoomMessage.Chat: Chat(client, reader.Str()); break;
                    case RoomMessage.UpdateSettings: UpdateSettings(client, RoomSettings.Read(reader)); break;
                    case RoomMessage.Kick: Kick(client, reader.U32()); break;
                    case RoomMessage.MatchReport: Report(client, reader.U32(), (MatchOutcome)reader.U8(), reader.Str()); break;
                    case RoomMessage.Rematch: Rematch(client, reader.U32(), reader.Bool()); break;
                    case RoomMessage.Spectate: Spectate(client, reader.U32()); break;
                    case RoomMessage.PublishStart: PublishStart(client, reader.U32(), MatchStart.Decode(reader)); break;
                    case RoomMessage.SpectatorFrames: PublishFrames(client, reader.U32(), reader); break;
                }
            }
            catch (NetFormatException)
            {
                Send(client, RoomMessages.Error("The server could not read a request."));
            }
        }

        private static bool Compatible(Client client, Room room) =>
            client.Identity.Build == room.Build && client.Identity.Content == room.Content;

        private void SendRoomList(Client client)
        {
            var listings = _rooms.Values.Where(room => Compatible(client, room))
                .OrderByDescending(room => room.Members.Count).ThenBy(room => room.Id).Take(100)
                .Select(room => new RoomListing
                {
                    Id = room.Id,
                    Name = room.Settings.Name,
                    HostName = room.Members.FirstOrDefault(member => member.Id == room.HostId)?.Identity.PlayerName ?? "",
                    Players = room.Members.Count,
                    MaxPlayers = room.Settings.MaxPlayers,
                    Locked = !string.IsNullOrEmpty(room.Password),
                    WinsRequired = room.Settings.WinsRequired,
                    Rotation = room.Settings.Rotation,
                    Arena = room.Settings.Arena,
                }).ToList();
            int start = 0;
            do
            {
                Send(client, RoomMessages.RoomList(listings, start, out int written));
                start += written;
            } while (start < listings.Count);
        }

        private void CreateRoom(Client client, RoomSettings settings, string password)
        {
            string problem = settings.Validate();
            if (problem != null) { Send(client, RoomMessages.Error(problem)); return; }
            if (_rooms.Count >= MaxRooms) { Send(client, RoomMessages.Error("The server has too many rooms. Join one instead.")); return; }
            if (client.Room != null) LeaveRoom(client, null);
            var room = new Room
            {
                Id = _nextRoomId++,
                Code = NewCode(),
                Password = password ?? "",
                Settings = settings.Copy(),
                Build = client.Identity.Build,
                Content = client.Identity.Content,
                HostId = client.Id,
            };
            _rooms[room.Id] = room;
            _byCode[room.Code] = room;
            Log("room " + room.Id + " '" + settings.Name + "' code " + room.Code + " by client " + client.Id);
            AddMember(room, client);
        }

        private void JoinRoom(Client client, uint roomId, string code, string password)
        {
            if (_nowMs < client.JoinLockedUntilMs) { Send(client, RoomMessages.Error("Too many failed attempts. Wait a moment.")); return; }
            Room room = null;
            if (roomId != 0) _rooms.TryGetValue(roomId, out room);
            else if (!string.IsNullOrWhiteSpace(code)) _byCode.TryGetValue(code.Trim(), out room);
            if (room == null) { FailedJoin(client); Send(client, RoomMessages.Error("No room with that code.")); return; }
            if (client.Room == room) { Send(client, StateFor(room)); return; }
            if (client.Identity.Build != room.Build)
            { Send(client, RoomMessages.Error("That room runs a different game build (" + room.Build + ").")); return; }
            if (client.Identity.Content != room.Content)
            { Send(client, RoomMessages.Error("That room uses different mods. Everyone needs the same mods and versions.")); return; }
            if (!string.IsNullOrEmpty(room.Password) && room.Password != password) { FailedJoin(client); Send(client, RoomMessages.Error("Wrong room password.")); return; }
            client.FailedJoins = 0;
            if (room.Members.Count >= room.Settings.MaxPlayers) { Send(client, RoomMessages.Error("That room is full.")); return; }
            if (client.Room != null) LeaveRoom(client, null);
            AddMember(room, client);
        }

        private void FailedJoin(Client client)
        {
            if (++client.FailedJoins < MaxFailedJoins) return;
            client.FailedJoins = 0;
            client.JoinLockedUntilMs = _nowMs + JoinLockoutMs;
        }

        private void AddMember(Room room, Client client)
        {
            client.Room = room;
            client.Member = new RoomMember { Id = client.Id, Name = client.Identity.PlayerName, Status = MemberStatus.Idle };
            room.Members.Add(client);
            Broadcast(room);
            SystemLine(room, client.Identity.PlayerName + " joined.");
        }

        private void LeaveRoom(Client client, string reason)
        {
            var room = client.Room;
            if (room == null) return;
            StopWatching(client, "You left the room.");
            var match = client.Match;
            if (match != null)
            {
                if (match.Left == client) match.LeftGone = true; else match.RightGone = true;
                client.Match = null;
                Resolve(match, force: false);
            }
            room.Members.Remove(client);
            room.Queue.Remove(client.Id);
            // Whoever was waiting to fight them again is not any more.
            foreach (var member in room.Members)
            {
                if (member.Member == null || !member.Member.WantsRematch || OpponentOf(member) != client) continue;
                member.Member.WantsRematch = false;
                Send(member, RoomMessages.Error(client.Identity.PlayerName + " left, so there is no rematch."));
            }
            client.Room = null;
            client.Member = null;
            if (room.ChampionId == client.Id) { room.ChampionId = 0; room.Streak = 0; room.ChampionAwaySinceMs = -1; }
            if (reason != null) Send(client, RoomMessages.LeftRoom(reason));
            if (room.Members.Count == 0)
            {
                _rooms.Remove(room.Id);
                _byCode.Remove(room.Code);
                Log("room " + room.Id + " closed");
                return;
            }
            bool hostLeft = room.HostId == client.Id;
            if (hostLeft) room.HostId = room.Members[0].Id;
            TryPair(room);
            Broadcast(room);
            SystemLine(room, client.Identity.PlayerName + " left.");
            if (hostLeft) SystemLine(room, room.Members[0].Identity.PlayerName + " is now the host.");
        }

        private void SetMember(Client client, LoadoutCode loadout, bool queued)
        {
            var room = client.Room;
            if (room == null || client.Member == null) return;
            if (client.Member.Status == MemberStatus.Spectating) StopWatching(client, "Stopped spectating.");
            // The server cannot read a loadout (only the game knows its roster); it stores and relays it.
            client.Member.Loadout = loadout;
            // Continuing to the room (queued or not) ends any rematch request.
            client.Member.WantsRematch = false;
            if (client.Member.Status == MemberStatus.InMatch) client.QueueAfterMatch = queued;
            else
            {
                if (queued)
                {
                    client.Member.Status = MemberStatus.Queued;
                    if (!room.Queue.Contains(client.Id))
                    {
                        // The champion goes straight back to the front.
                        if (room.ChampionId == client.Id) room.Queue.Insert(0, client.Id);
                        else room.Queue.Add(client.Id);
                    }
                    if (room.ChampionId == client.Id) room.ChampionAwaySinceMs = -1;
                }
                else
                {
                    client.Member.Status = MemberStatus.Idle;
                    room.Queue.Remove(client.Id);
                    if (room.ChampionId == client.Id) { room.ChampionId = 0; room.Streak = 0; room.ChampionAwaySinceMs = -1; }
                }
            }
            TryPair(room);
            Broadcast(room);
        }

        private void Chat(Client client, string text)
        {
            var room = client.Room;
            if (room == null || client.Member == null) return;
            text = ChatLine.Clean(text);
            if (text.Length == 0) return;
            client.ChatTokens = Math.Min(ChatBurst, client.ChatTokens + (_nowMs - client.ChatRefilledMs) / (float)ChatRefillMs);
            client.ChatRefilledMs = _nowMs;
            if (client.ChatTokens < 1f)
            {
                Send(client, new ChatLine { Kind = ChatKind.Notice, Text = "You are sending messages too quickly." }.Encode());
                return;
            }
            client.ChatTokens -= 1f;
            byte[] line = new ChatLine { Kind = ChatKind.Player, SenderId = client.Id, SenderName = client.Identity.PlayerName, Text = text }.Encode();
            foreach (var member in room.Members.ToArray()) Send(member, line);
        }

        /// <summary>A line from the room itself: someone joined, left, won, or the rules changed.</summary>
        private void SystemLine(Room room, string text)
        {
            if (!_rooms.ContainsKey(room.Id)) return;
            byte[] line = new ChatLine { Kind = ChatKind.System, Text = text }.Encode();
            foreach (var member in room.Members.ToArray()) Send(member, line);
        }

        private void UpdateSettings(Client client, RoomSettings settings)
        {
            var room = client.Room;
            if (room == null) return;
            if (room.HostId != client.Id) { Send(client, RoomMessages.Error("Only the room host can change settings.")); return; }
            string problem = settings.Validate();
            if (problem == null && settings.MaxPlayers < room.Members.Count) problem = "The room already has " + room.Members.Count + " players.";
            if (problem != null) { Send(client, RoomMessages.Error(problem)); return; }
            if (room.Settings.Rotation != settings.Rotation) { room.ChampionId = 0; room.Streak = 0; room.ChampionAwaySinceMs = -1; }
            room.Settings = settings.Copy();
            TryPair(room);
            Broadcast(room);
            SystemLine(room, "Room settings changed.");
        }

        private void Kick(Client client, uint memberId)
        {
            var room = client.Room;
            if (room == null || room.HostId != client.Id || memberId == client.Id) return;
            var target = room.Members.Find(member => member.Id == memberId);
            if (target == null) return;
            // Removing a fighter mid-match would hand the host a win by disconnect.
            if (target.Member.Status == MemberStatus.InMatch) { Send(client, RoomMessages.Error("Wait until their fight ends.")); return; }
            LeaveRoom(target, "The host removed you from the room.");
        }

        // ---- Matches ----

        private void PublishStart(Client client, uint matchId, MatchStart start)
        {
            var match = client.Match;
            if (match == null || match.Id != matchId || match.Left != client || match.Stream != null) return;
            if (start.Seed != match.Seed || start.HostLoadout != match.LeftLoadout || start.GuestLoadout != match.RightLoadout ||
                string.IsNullOrWhiteSpace(start.Arena) || start.Arena.Length > 32 ||
                (match.Settings.Arena != RoomSettings.RandomArena && start.Arena != match.Settings.Arena) ||
                start.WinsRequired != match.Settings.WinsRequired ||
                start.RoundTimeSeconds != match.Settings.RoundTimeSeconds) return;
            match.Stream = new SpectatorStream { MatchId = matchId, Start = start };
            match.Stream.Replay.LeftName = match.Left.Identity.PlayerName;
            match.Stream.Replay.RightName = match.Right.Identity.PlayerName;
            Broadcast(match.Room);
        }

        private void PublishFrames(Client client, uint matchId, NetReader reader)
        {
            var match = client.Match;
            if (match == null || match.Id != matchId || match.Left != client || match.Stream == null) return;
            // Bound upload speed as well as total memory; a fighter cannot fill a half-hour buffer instantly.
            int available = (int)Math.Min(SpectatorStream.MaxTicks, (_nowMs - match.StartedMs) * 60 / 1000 + 600);
            if (match.Stream.Replay.TickCount > available) return;
            match.Stream.ReadFrames(reader);
        }

        private void Spectate(Client client, uint matchId)
        {
            var room = client.Room;
            if (room == null || client.Member == null) return;
            if (client.Match != null) { Send(client, RoomMessages.Error("Finish your own fight before spectating.")); return; }
            if (matchId == 0)
            {
                StopWatching(client, "Stopped spectating.");
                Broadcast(room);
                return;
            }
            var match = room.Matches.Find(fight => fight.Id == matchId);
            if (match?.Stream == null) { Send(client, RoomMessages.Error("That fight is still starting or has already ended.")); return; }
            StopWatching(client, "Switched fights.");
            room.Queue.Remove(client.Id);
            client.Member.Status = MemberStatus.Spectating;
            client.Member.WantsRematch = false;
            client.QueueAfterMatch = false;
            if (room.ChampionId == client.Id) { room.ChampionId = 0; room.Streak = 0; room.ChampionAwaySinceMs = -1; }
            client.Watching = match;
            client.WatchTick = 0;
            Send(client, match.Stream.EncodeStart());
            TryPair(room);
            Broadcast(room);
        }

        private void StopWatching(Client client, string reason)
        {
            var match = client.Watching;
            client.Watching = null;
            if (match?.Stream != null) Send(client, match.Stream.EncodeEnd(reason));
            if (client.Member?.Status == MemberStatus.Spectating) client.Member.Status = MemberStatus.Idle;
            ReleaseSpectatorHistory(match);
        }

        private void PumpSpectator(Client client)
        {
            var match = client.Watching;
            var stream = match?.Stream;
            if (stream == null) return;
            // Keep headroom for room control messages; slow viewers never stall the fighters.
            for (int i = 0; i < 4 && client.Reliable.PendingCount < 16 && client.WatchTick < stream.Replay.TickCount; i++)
            {
                Send(client, stream.EncodeFrames(client.WatchTick, out int count));
                client.WatchTick += count;
            }
            if (stream.Ended && client.WatchTick == stream.Replay.TickCount && client.Reliable.PendingCount < 16)
            {
                Send(client, stream.EncodeEnd());
                client.Watching = null;
                ReleaseSpectatorHistory(match);
            }
        }

        private void ReleaseSpectatorHistory(Match match)
        {
            if (match?.Stream?.Ended == true && !_byEndPoint.Values.Any(client => client.Watching == match)) match.Stream = null;
        }

        private void TryPair(Room room)
        {
            if (room.Matches.Count > 0 && room.Settings.Rotation != RoomRotation.Simultaneous) return;
            room.Queue.RemoveAll(id => room.Members.All(member => member.Id != id || member.Member.Status != MemberStatus.Queued));
            if (room.Settings.Rotation == RoomRotation.WinnerStays && room.ChampionId != 0)
            {
                var champion = room.Members.Find(member => member.Id == room.ChampionId);
                if (champion == null) { room.ChampionId = 0; room.Streak = 0; }
                else if (champion.Member.Status == MemberStatus.Away) return; // The winner stays: wait for them to continue.
            }
            while (room.Queue.Count >= 2)
            {
                var left = room.Members.Find(member => member.Id == room.Queue[0]);
                var right = room.Members.Find(member => member.Id == room.Queue[1]);
                room.Queue.RemoveRange(0, 2);
                StartMatch(room, left, right);
                if (room.Settings.Rotation != RoomRotation.Simultaneous) break;
            }
        }

        /// <summary>Pairs two members: each gets the other's address and a shared seed (which also picks a random arena).</summary>
        private void StartMatch(Room room, Client left, Client right)
        {
            var match = new Match
            {
                Id = _nextMatchId++,
                Secret = (uint)_random.Next(1, int.MaxValue),
                Room = room,
                Left = left,
                Right = right,
                StartedMs = _nowMs,
                Seed = _random.Next(),
                Settings = room.Settings.Copy(),
                LeftLoadout = left.Member.Loadout,
                RightLoadout = right.Member.Loadout,
            };
            room.Matches.Add(match);
            left.Match = right.Match = match;
            left.Member.Status = right.Member.Status = MemberStatus.InMatch;
            left.Member.WantsRematch = right.Member.WantsRematch = false;
            left.QueueAfterMatch = right.QueueAfterMatch = false;
            Send(left, PairingFor(match, left, right, 0, match.Seed).Encode());
            Send(right, PairingFor(match, right, left, 1, match.Seed).Encode());
            Log("match " + match.Id + " in room " + room.Id + ": " + left.Identity.PlayerName + " vs " + right.Identity.PlayerName);
        }

        /// <summary>The other player of this member's last fight, while both are still in its room.</summary>
        private static Client OpponentOf(Client client)
        {
            var last = client.LastMatch;
            if (last == null || client.Room == null || last.Room != client.Room) return null;
            var other = last.Left == client ? last.Right : last.Left;
            return other.Room == client.Room && other.Member != null ? other : null;
        }

        /// <summary>
        /// Both players of a finished fight asked to go again: pair them once more on the
        /// same sides, with a new seed (so a random arena is drawn afresh). The room's
        /// queue comes first: nobody may be waiting in line and no other fight running.
        /// </summary>
        private void Rematch(Client client, uint matchId, bool wanted)
        {
            var room = client.Room;
            if (room == null || client.Member == null) return;
            if (!wanted)
            {
                if (!client.Member.WantsRematch) return;
                client.Member.WantsRematch = false;
                Broadcast(room);
                return;
            }
            // The request may arrive before the second result report resolves the fight.
            var match = client.Match != null && client.Match.Id == matchId ? client.Match
                : client.LastMatch != null && client.LastMatch.Id == matchId && client.Member.Status == MemberStatus.Away ? client.LastMatch : null;
            if (match == null) { Send(client, RoomMessages.Error("That fight is over; there is nothing to rematch.")); return; }
            var opponent = match.Left == client ? match.Right : match.Left;
            if (opponent.Room != room || opponent.Member == null) { Send(client, RoomMessages.Error("Your opponent left the room.")); return; }
            bool opponentStill = opponent.Match == match || opponent.Match == null && opponent.LastMatch == match && opponent.Member.Status == MemberStatus.Away;
            if (!opponentStill) { Send(client, RoomMessages.Error(opponent.Identity.PlayerName + " has already moved on.")); return; }
            if (room.Queue.Count > 0 && room.Settings.Rotation != RoomRotation.Simultaneous) { Send(client, RoomMessages.Error("Others are waiting to fight, so no rematch this time.")); return; }
            client.Member.WantsRematch = true;
            TryRematch(match);
            Broadcast(room);
        }

        /// <summary>Starts the rematch once the fight has resolved and both players asked for it.</summary>
        private bool TryRematch(Match match)
        {
            var room = match.Room;
            var left = match.Left;
            var right = match.Right;
            bool bothBack = (room.Matches.Count == 0 || room.Settings.Rotation == RoomRotation.Simultaneous) && left.Room == room && right.Room == room && left.Member != null && right.Member != null &&
                left.Match == null && right.Match == null &&
                left.LastMatch == match && right.LastMatch == match &&
                left.Member.Status == MemberStatus.Away && right.Member.Status == MemberStatus.Away;
            if (!bothBack || !left.Member.WantsRematch || !right.Member.WantsRematch) return false;
            if (room.Queue.Count > 0 && room.Settings.Rotation != RoomRotation.Simultaneous)
            {
                // Someone joined the line while the fight was resolving; they go first.
                left.Member.WantsRematch = right.Member.WantsRematch = false;
                foreach (var player in new[] { left, right }) Send(player, RoomMessages.Error("Others are waiting to fight, so no rematch this time."));
                return false;
            }
            room.ChampionAwaySinceMs = -1;
            SystemLine(room, left.Identity.PlayerName + " and " + right.Identity.PlayerName + " go again.");
            StartMatch(room, left, right);
            return true;
        }

        private static RoomPairing PairingFor(Match match, Client self, Client peer, int side, int seed)
        {
            var pairing = new RoomPairing
            {
                MatchId = match.Id,
                Secret = match.Secret,
                Side = side,
                PeerId = peer.Id,
                PeerName = peer.Identity.PlayerName,
                PeerLoadout = peer.Member.Loadout,
                Seed = seed,
                Settings = match.Settings,
            };
            pairing.Candidates.Add(peer.EndPoint);
            foreach (var lan in peer.Lan)
                if (!lan.Equals(peer.EndPoint) && pairing.Candidates.Count < RoomProtocol.MaxCandidates) pairing.Candidates.Add(lan);
            return pairing;
        }

        private void Report(Client client, uint matchId, MatchOutcome outcome, string reason)
        {
            var match = client.Match;
            if (match == null || match.Id != matchId || outcome > MatchOutcome.Aborted) return;
            if (match.Left == client) match.LeftReport = outcome; else match.RightReport = outcome;
            if (match.FirstReportMs < 0) match.FirstReportMs = _nowMs;
            if (outcome == MatchOutcome.Aborted) Log("match " + match.Id + " aborted by " + client.Identity.PlayerName + ": " + reason);
            Resolve(match, force: false);
        }

        private void Resolve(Match match, bool force)
        {
            var room = match.Room;
            if (!room.Matches.Contains(match)) return;
            bool leftDone = match.LeftReport.HasValue || match.LeftGone;
            bool rightDone = match.RightReport.HasValue || match.RightGone;
            if (!force && !(leftDone && rightDone)) return;
            MatchOutcome outcome;
            if (match.LeftReport.HasValue && match.RightReport.HasValue)
                outcome = match.LeftReport == match.RightReport ? match.LeftReport.Value : MatchOutcome.Aborted;
            else if (match.LeftReport.HasValue && match.RightGone)
                outcome = match.LeftReport == MatchOutcome.LeftWon ? MatchOutcome.LeftWon : MatchOutcome.Aborted;
            else if (match.RightReport.HasValue && match.LeftGone)
                outcome = match.RightReport == MatchOutcome.RightWon ? MatchOutcome.RightWon : MatchOutcome.Aborted;
            else outcome = MatchOutcome.Aborted;

            room.Matches.Remove(match);
            if (match.Stream != null) match.Stream.Ended = true;
            ReleaseSpectatorHistory(match);
            foreach (var player in new[] { match.Left, match.Right })
            {
                if (player.Match == match) player.Match = null;
                player.LastMatch = match;
                player.LastMatchEndedMs = _nowMs;
                if (player.Room == room && player.Member != null) player.Member.Status = MemberStatus.Away;
            }
            if (outcome == MatchOutcome.LeftWon || outcome == MatchOutcome.RightWon)
            {
                var winner = outcome == MatchOutcome.LeftWon ? match.Left : match.Right;
                var loser = outcome == MatchOutcome.LeftWon ? match.Right : match.Left;
                if (winner.Member != null) winner.Member.Wins++;
                if (loser.Member != null) loser.Member.Losses++;
                if (room.Settings.Rotation == RoomRotation.WinnerStays && winner.Room == room)
                {
                    room.Streak = room.ChampionId == winner.Id ? room.Streak + 1 : 1;
                    room.ChampionId = winner.Id;
                    room.ChampionAwaySinceMs = _nowMs;
                }
                else { room.ChampionId = 0; room.Streak = 0; }
            }
            else if (room.ChampionId != 0 && (room.ChampionId == match.Left.Id || room.ChampionId == match.Right.Id))
            {
                room.ChampionAwaySinceMs = _nowMs;
            }
            Log("match " + match.Id + " result " + outcome);
            if (outcome == MatchOutcome.LeftWon || outcome == MatchOutcome.RightWon)
            {
                var winner = outcome == MatchOutcome.LeftWon ? match.Left : match.Right;
                var loser = outcome == MatchOutcome.LeftWon ? match.Right : match.Left;
                SystemLine(room, winner.Identity.PlayerName + " beat " + loser.Identity.PlayerName +
                    (room.ChampionId == winner.Id && room.Streak > 1 ? " (" + room.Streak + " in a row)." : "."));
            }
            else SystemLine(room, match.Left.Identity.PlayerName + " vs " + match.Right.Identity.PlayerName + " ended without a result.");
            if (TryRematch(match)) { Broadcast(room); return; }
            // Anyone who already asked to queue again (continued before the other report) goes straight in.
            foreach (var player in new[] { match.Left, match.Right })
            {
                if (!player.QueueAfterMatch || player.Room != room || player.Member == null) continue;
                player.QueueAfterMatch = false;
                player.Member.Status = MemberStatus.Queued;
                if (!room.Queue.Contains(player.Id))
                {
                    if (room.ChampionId == player.Id) { room.Queue.Insert(0, player.Id); room.ChampionAwaySinceMs = -1; }
                    else room.Queue.Add(player.Id);
                }
            }
            TryPair(room);
            Broadcast(room);
        }

        // ---- Helpers ----

        private void Drop(Client client, string why)
        {
            if (!_byEndPoint.Remove(client.EndPoint)) return;
            LeaveRoom(client, null);
            Log("client " + client.Id + " " + why);
        }

        private byte[] StateFor(Room room)
        {
            var state = new RoomState
            {
                RoomId = room.Id,
                Code = room.Code,
                Settings = room.Settings,
                Locked = !string.IsNullOrEmpty(room.Password),
                HostId = room.HostId,
                ChampionId = room.ChampionId,
                Streak = room.Streak,
            };
            foreach (var match in room.Matches) state.Fights.Add(new RoomFight
            {
                MatchId = match.Id, LeftId = match.Left.Id, RightId = match.Right.Id, CanSpectate = match.Stream != null,
            });
            foreach (var member in room.Members)
            {
                MeasureLink(member, out member.Member.PingMs, out member.Member.Link);
                state.Members.Add(member.Member);
            }
            state.Queue.AddRange(room.Queue);
            return state.Encode();
        }

        private void Broadcast(Room room)
        {
            if (!_rooms.ContainsKey(room.Id)) return;
            room.LastBroadcastMs = _nowMs;
            byte[] state = StateFor(room);
            foreach (var member in room.Members.ToArray()) Send(member, state);
        }

        private string NewCode()
        {
            const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            for (;;)
            {
                var chars = new char[6];
                for (int i = 0; i < chars.Length; i++) chars[i] = alphabet[_random.Next(alphabet.Length)];
                string code = new string(chars);
                if (!_byCode.ContainsKey(code)) return code;
            }
        }
    }
}
