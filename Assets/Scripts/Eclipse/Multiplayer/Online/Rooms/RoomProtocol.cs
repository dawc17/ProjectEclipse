using System;
using System.Collections.Generic;
using System.Net;

namespace Eclipse.Multiplayer.Online.Rooms
{
    /// <summary>
    /// Wire format between game clients and the room server, and between two paired
    /// clients while they hole-punch. Server packets start 'E' 'R'; punch packets 'E' 'P'.
    /// Fights themselves use the ordinary netplay packets ('E' 'N'), sent directly or
    /// wrapped in <see cref="KindRelay"/> through the server.
    /// </summary>
    public static class RoomProtocol
    {
        public const byte Version = 2;
        public const int DefaultPort = 7300;
        public const int MaxMembers = 8;
        public const int MaxCandidates = 6;

        public const byte KindHello = 1;
        public const byte KindWelcome = 2;
        public const byte KindReject = 3;
        public const byte KindData = 4;
        public const byte KindBye = 5;
        public const byte KindRelay = 6;
        /// <summary>
        /// Server to client: repeat your hello with this cookie. Proves the client can
        /// receive at its claimed address before the server keeps any state or sends
        /// anything larger than the request (no spoofing or reflection).
        /// </summary>
        public const byte KindChallenge = 7;
        /// <summary>Server to client, unreliable: a clock stamp the client echoes at once, to measure its ping.</summary>
        public const byte KindPing = 8;
        /// <summary>Client to server: the stamp from a <see cref="KindPing"/>, unchanged.</summary>
        public const byte KindPong = 9;

        public const byte PunchKindPing = 1;
        public const byte PunchKindAck = 2;

        public static void WriteHeader(NetWriter writer, byte kind, uint token)
        {
            writer.Reset();
            writer.U8((byte)'E');
            writer.U8((byte)'R');
            writer.U8(Version);
            writer.U8(kind);
            writer.U32(token);
        }

        public static bool IsServerPacket(byte[] buffer, int length) => length >= 8 && buffer[0] == 'E' && buffer[1] == 'R';
        public static bool IsPunchPacket(byte[] buffer, int length) => length >= 16 && buffer[0] == 'E' && buffer[1] == 'P';
        public static bool IsNetplayPacket(byte[] buffer, int length) => IsNetplayPacketAt(buffer, 0, length);

        public static bool IsNetplayPacketAt(byte[] buffer, int offset, int length) =>
            length >= NetProtocol.HeaderSize && buffer[offset] == 'E' && buffer[offset + 1] == 'N';

        public static void WritePunch(NetWriter writer, byte kind, uint matchId, uint fromClient, uint secret)
        {
            writer.Reset();
            writer.U8((byte)'E');
            writer.U8((byte)'P');
            writer.U8(Version);
            writer.U8(kind);
            writer.U32(matchId);
            writer.U32(fromClient);
            writer.U32(secret);
        }

        public static void WriteEndPoint(NetWriter writer, IPEndPoint endPoint)
        {
            byte[] address = endPoint.Address.GetAddressBytes();
            if (address.Length != 4) throw new ArgumentException("Only IPv4 endpoints are supported.");
            writer.Bytes(address, 0, 4);
            writer.U16((ushort)endPoint.Port);
        }

        public static IPEndPoint ReadEndPoint(NetReader reader)
        {
            byte[] address = reader.Bytes(4);
            int port = reader.U16();
            return new IPEndPoint(new IPAddress(address), port);
        }
    }

    public enum RoomMessage : byte
    {
        // Client to server.
        ListRooms = 1,
        CreateRoom = 2,
        JoinRoom = 3,
        LeaveRoom = 4,
        SetMember = 5,
        UpdateSettings = 6,
        Kick = 7,
        MatchReport = 8,
        Chat = 9,
        // Server to client.
        Error = 20,
        RoomList = 21,
        RoomState = 22,
        Pairing = 23,
        LeftRoom = 24,
        ChatLine = 25,
    }

    public enum RoomRotation : byte
    {
        /// <summary>The winner stays on; the loser goes to the back of the queue.</summary>
        WinnerStays = 0,
        /// <summary>Both players go to the back of the queue after every match.</summary>
        Rotate = 1,
    }

    public enum MemberStatus : byte
    {
        /// <summary>In the room but not queued (watching the scoreboard).</summary>
        Idle = 0,
        Queued = 1,
        InMatch = 2,
        /// <summary>Just finished a match and has not continued yet.</summary>
        Away = 3,
    }

    public enum MatchOutcome : byte
    {
        LeftWon = 0,
        RightWon = 1,
        Draw = 2,
        /// <summary>No result: the peers could not connect, desynced or someone left.</summary>
        Aborted = 3,
    }

    /// <summary>What the host controls.</summary>
    public sealed class RoomSettings
    {
        public const string RandomArena = "random";
        public string Name = "Room";
        public int MaxPlayers = RoomProtocol.MaxMembers;
        public string Arena = RandomArena;
        public int WinsRequired = 2;
        public int RoundTimeSeconds = 99;
        public RoomRotation Rotation = RoomRotation.WinnerStays;

        public void Write(NetWriter writer)
        {
            writer.StrClamped(Name, 64);
            writer.U8((byte)MaxPlayers);
            writer.StrClamped(Arena, 32);
            writer.U8((byte)WinsRequired);
            writer.U16((ushort)RoundTimeSeconds);
            writer.U8((byte)Rotation);
        }

        public static RoomSettings Read(NetReader reader)
        {
            var settings = new RoomSettings
            {
                Name = reader.Str(),
                MaxPlayers = reader.U8(),
                Arena = reader.Str(),
                WinsRequired = reader.U8(),
                RoundTimeSeconds = reader.U16(),
                Rotation = (RoomRotation)reader.U8(),
            };
            return settings;
        }

        /// <returns>A player-facing problem, or null.</returns>
        public string Validate()
        {
            if (string.IsNullOrWhiteSpace(Name) || Name.Length > 32) return "Room names are 1 to 32 characters.";
            if (MaxPlayers < 2 || MaxPlayers > RoomProtocol.MaxMembers) return "Rooms hold 2 to " + RoomProtocol.MaxMembers + " players.";
            if (string.IsNullOrWhiteSpace(Arena) || Arena.Length > 32) return "Choose an arena.";
            if (WinsRequired < 1 || WinsRequired > 5) return "First to 1 to 5 wins.";
            if (RoundTimeSeconds < 30 || RoundTimeSeconds > 300) return "Rounds last 30 to 300 seconds.";
            if (Rotation != RoomRotation.WinnerStays && Rotation != RoomRotation.Rotate) return "Unknown rotation.";
            return null;
        }

        public RoomSettings Copy() => (RoomSettings)MemberwiseClone();
    }

    /// <summary>What the server knows about a member's connection.</summary>
    [Flags]
    public enum MemberLink : byte
    {
        None = 0,
        /// <summary>Their last fight had to go through the server's relay.</summary>
        Relayed = 1,
        /// <summary>Nothing heard from them for a few seconds.</summary>
        Stale = 2,
    }

    public sealed class RoomMember
    {
        public uint Id;
        public string Name = string.Empty;
        public LoadoutCode Loadout = LoadoutCode.None;
        public MemberStatus Status;
        public int Wins, Losses;
        /// <summary>Round trip to the room server, or -1 before the first measurement.</summary>
        public int PingMs = -1;
        public MemberLink Link;
    }

    public sealed class RoomListing
    {
        public uint Id;
        public string Name = string.Empty;
        public string HostName = string.Empty;
        public int Players, MaxPlayers;
        public bool Locked;
        public int WinsRequired;
        public RoomRotation Rotation;
        public string Arena = RoomSettings.RandomArena;
    }

    /// <summary>Everything a member sees about their room.</summary>
    public sealed class RoomState
    {
        public uint RoomId;
        public string Code = string.Empty;
        public RoomSettings Settings = new RoomSettings();
        public bool Locked;
        public uint HostId;
        public uint ChampionId;
        public int Streak;
        public uint MatchId, LeftId, RightId;
        public readonly List<RoomMember> Members = new List<RoomMember>();
        public readonly List<uint> Queue = new List<uint>();

        public RoomMember Find(uint id) => Members.Find(member => member.Id == id);

        public byte[] Encode()
        {
            var writer = new NetWriter(ReliableChannel.MaxMessageSize);
            writer.U8((byte)RoomMessage.RoomState);
            writer.U32(RoomId);
            writer.StrClamped(Code, 16);
            Settings.Write(writer);
            writer.Bool(Locked);
            writer.U32(HostId);
            writer.U32(ChampionId);
            writer.U16((ushort)Math.Min(Streak, ushort.MaxValue));
            writer.U32(MatchId);
            writer.U32(LeftId);
            writer.U32(RightId);
            writer.U8((byte)Members.Count);
            foreach (var member in Members)
            {
                writer.U32(member.Id);
                writer.StrClamped(member.Name, 48);
                member.Loadout.Write(writer);
                writer.U8((byte)member.Status);
                writer.U16((ushort)Math.Min(member.Wins, ushort.MaxValue));
                writer.U16((ushort)Math.Min(member.Losses, ushort.MaxValue));
                writer.U16((ushort)(member.PingMs < 0 ? ushort.MaxValue : Math.Min(member.PingMs, ushort.MaxValue - 1)));
                writer.U8((byte)member.Link);
            }
            writer.U8((byte)Queue.Count);
            foreach (uint id in Queue) writer.U32(id);
            return writer.ToArray();
        }

        public static RoomState Decode(NetReader reader)
        {
            var state = new RoomState
            {
                RoomId = reader.U32(),
                Code = reader.Str(),
                Settings = RoomSettings.Read(reader),
                Locked = reader.Bool(),
                HostId = reader.U32(),
                ChampionId = reader.U32(),
                Streak = reader.U16(),
                MatchId = reader.U32(),
                LeftId = reader.U32(),
                RightId = reader.U32(),
            };
            int members = reader.U8();
            if (members > RoomProtocol.MaxMembers) throw new NetFormatException("Too many members.");
            for (int i = 0; i < members; i++)
            {
                state.Members.Add(new RoomMember
                {
                    Id = reader.U32(),
                    Name = reader.Str(),
                    Loadout = LoadoutCode.Read(reader),
                    Status = (MemberStatus)reader.U8(),
                    Wins = reader.U16(),
                    Losses = reader.U16(),
                    PingMs = PingFromWire(reader.U16()),
                    Link = (MemberLink)reader.U8(),
                });
            }
            int queue = reader.U8();
            if (queue > RoomProtocol.MaxMembers) throw new NetFormatException("Queue too long.");
            for (int i = 0; i < queue; i++) state.Queue.Add(reader.U32());
            return state;
        }

        private static int PingFromWire(ushort value) => value == ushort.MaxValue ? -1 : value;
    }

    /// <summary>Sent to both players when the room pairs them for a fight.</summary>
    public sealed class RoomPairing
    {
        public uint MatchId;
        /// <summary>Shared by the two players only; authenticates punch packets.</summary>
        public uint Secret;
        /// <summary>0 = left fighter and netplay host, 1 = right fighter and guest.</summary>
        public int Side;
        public uint PeerId;
        public string PeerName = string.Empty;
        public LoadoutCode PeerLoadout = LoadoutCode.None;
        public int Seed;
        public readonly List<IPEndPoint> Candidates = new List<IPEndPoint>();

        public byte[] Encode()
        {
            var writer = new NetWriter(ReliableChannel.MaxMessageSize);
            writer.U8((byte)RoomMessage.Pairing);
            writer.U32(MatchId);
            writer.U32(Secret);
            writer.U8((byte)Side);
            writer.U32(PeerId);
            writer.StrClamped(PeerName, 48);
            PeerLoadout.Write(writer);
            writer.I32(Seed);
            int count = Math.Min(Candidates.Count, RoomProtocol.MaxCandidates);
            writer.U8((byte)count);
            for (int i = 0; i < count; i++) RoomProtocol.WriteEndPoint(writer, Candidates[i]);
            return writer.ToArray();
        }

        public static RoomPairing Decode(NetReader reader)
        {
            var pairing = new RoomPairing
            {
                MatchId = reader.U32(),
                Secret = reader.U32(),
                Side = reader.U8(),
                PeerId = reader.U32(),
                PeerName = reader.Str(),
                PeerLoadout = LoadoutCode.Read(reader),
                Seed = reader.I32(),
            };
            int count = reader.U8();
            if (count > RoomProtocol.MaxCandidates) throw new NetFormatException("Too many candidates.");
            for (int i = 0; i < count; i++) pairing.Candidates.Add(RoomProtocol.ReadEndPoint(reader));
            return pairing;
        }
    }

    public static class RoomMessages
    {
        public static byte[] Simple(RoomMessage type) => new[] { (byte)type };

        public static byte[] CreateRoom(RoomSettings settings, string password)
        {
            var writer = new NetWriter(256);
            writer.U8((byte)RoomMessage.CreateRoom);
            settings.Write(writer);
            writer.Str(password ?? string.Empty);
            return writer.ToArray();
        }

        public static byte[] JoinRoom(uint roomId, string code, string password)
        {
            var writer = new NetWriter(128);
            writer.U8((byte)RoomMessage.JoinRoom);
            writer.U32(roomId);
            writer.Str(code ?? string.Empty);
            writer.Str(password ?? string.Empty);
            return writer.ToArray();
        }

        public static byte[] SetMember(LoadoutCode loadout, bool queued)
        {
            var writer = new NetWriter(1 + LoadoutCode.Size + 1);
            writer.U8((byte)RoomMessage.SetMember);
            loadout.Write(writer);
            writer.Bool(queued);
            return writer.ToArray();
        }

        public static byte[] Chat(string text)
        {
            var writer = new NetWriter(8 + ChatLine.MaxTextBytes);
            writer.U8((byte)RoomMessage.Chat);
            writer.StrClamped(ChatLine.Clean(text), ChatLine.MaxTextBytes);
            return writer.ToArray();
        }

        public static byte[] UpdateSettings(RoomSettings settings)
        {
            var writer = new NetWriter(128);
            writer.U8((byte)RoomMessage.UpdateSettings);
            settings.Write(writer);
            return writer.ToArray();
        }

        public static byte[] Kick(uint memberId)
        {
            var writer = new NetWriter(8);
            writer.U8((byte)RoomMessage.Kick);
            writer.U32(memberId);
            return writer.ToArray();
        }

        public static byte[] MatchReport(uint matchId, MatchOutcome outcome, string reason)
        {
            var writer = new NetWriter(160);
            writer.U8((byte)RoomMessage.MatchReport);
            writer.U32(matchId);
            writer.U8((byte)outcome);
            writer.StrClamped(reason, 120);
            return writer.ToArray();
        }

        public static byte[] Error(string text)
        {
            var writer = new NetWriter(200);
            writer.U8((byte)RoomMessage.Error);
            writer.StrClamped(text, 180);
            return writer.ToArray();
        }

        public static byte[] LeftRoom(string reason)
        {
            var writer = new NetWriter(200);
            writer.U8((byte)RoomMessage.LeftRoom);
            writer.StrClamped(reason, 180);
            return writer.ToArray();
        }

        /// <summary>Upper bound on one encoded listing, used to page the list by size.</summary>
        public const int MaxListingBytes = 4 + 1 + 64 + 1 + 48 + 5 + 1 + 32;

        /// <summary>
        /// One page of the room list, holding as many listings from <paramref name="start"/> as fit;
        /// <paramref name="written"/> says how many. The page is marked last when it reaches the end.
        /// </summary>
        public static byte[] RoomList(IList<RoomListing> rooms, int start, out int written)
        {
            var writer = new NetWriter(ReliableChannel.MaxMessageSize);
            int count = Math.Min(rooms.Count - start, (ReliableChannel.MaxMessageSize - 4) / MaxListingBytes);
            if (count < 0) count = 0;
            written = count;
            writer.U8((byte)RoomMessage.RoomList);
            writer.Bool(start == 0);
            writer.Bool(start + count >= rooms.Count);
            writer.U8((byte)count);
            for (int i = start; i < start + count; i++)
            {
                var room = rooms[i];
                writer.U32(room.Id);
                writer.StrClamped(room.Name, 64);
                writer.StrClamped(room.HostName, 48);
                writer.U8((byte)room.Players);
                writer.U8((byte)room.MaxPlayers);
                writer.Bool(room.Locked);
                writer.U8((byte)room.WinsRequired);
                writer.U8((byte)room.Rotation);
                writer.StrClamped(room.Arena, 32);
            }
            return writer.ToArray();
        }

        public static void ReadRoomList(NetReader reader, List<RoomListing> into, out bool first, out bool last)
        {
            first = reader.Bool();
            last = reader.Bool();
            int count = reader.U8();
            if (first) into.Clear();
            for (int i = 0; i < count; i++)
            {
                into.Add(new RoomListing
                {
                    Id = reader.U32(),
                    Name = reader.Str(),
                    HostName = reader.Str(),
                    Players = reader.U8(),
                    MaxPlayers = reader.U8(),
                    Locked = reader.Bool(),
                    WinsRequired = reader.U8(),
                    Rotation = (RoomRotation)reader.U8(),
                    Arena = reader.Str(),
                });
            }
        }

    }

    public enum ChatKind : byte
    {
        /// <summary>Something a member typed.</summary>
        Player = 0,
        /// <summary>The room itself: joins, leaves, results, settings.</summary>
        System = 1,
        /// <summary>Only for the recipient, such as "slow down".</summary>
        Notice = 2,
    }

    /// <summary>One line of room chat, as the server sends it to members.</summary>
    public sealed class ChatLine
    {
        public const int MaxChars = 140;
        public const int MaxTextBytes = 200;
        public ChatKind Kind;
        public uint SenderId;
        public string SenderName = string.Empty;
        public string Text = string.Empty;

        public byte[] Encode()
        {
            var writer = new NetWriter(16 + 48 + MaxTextBytes);
            writer.U8((byte)RoomMessage.ChatLine);
            writer.U8((byte)Kind);
            writer.U32(SenderId);
            writer.StrClamped(SenderName, 48);
            writer.StrClamped(Text, MaxTextBytes);
            return writer.ToArray();
        }

        public static ChatLine Decode(NetReader reader) => new ChatLine
        {
            Kind = (ChatKind)reader.U8(),
            SenderId = reader.U32(),
            SenderName = reader.Str(),
            Text = Clean(reader.Str()),
        };

        /// <summary>One line of printable text: control characters dropped, spaces collapsed, at most <see cref="MaxChars"/>.</summary>
        public static string Clean(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            var builder = new System.Text.StringBuilder(Math.Min(text.Length, MaxChars));
            bool space = false;
            foreach (char c in text)
            {
                if (char.IsControl(c) || c == '\u200B' || c == '\uFEFF') continue;
                if (char.IsWhiteSpace(c)) { space = builder.Length > 0; continue; }
                if (space) { builder.Append(' '); space = false; }
                builder.Append(c);
                if (builder.Length >= MaxChars) break;
            }
            return builder.ToString();
        }
    }
}
