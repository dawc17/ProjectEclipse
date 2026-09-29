using System;

namespace Eclipse.Multiplayer.Online
{
    /// <summary>
    /// Wire constants for online versus. Every datagram starts with an 8 byte header:
    /// 'E' 'N', protocol version, packet kind, then the 32-bit session token.
    /// </summary>
    public static class NetProtocol
    {
        public const byte Version = 1;
        public const int DefaultPort = 7291;
        public const int MaxPacketSize = 1200;
        public const int MaxStringBytes = 200;
        public const int HeaderSize = 8;
        public const int MaxInputDelay = 10;
        public const int DefaultInputDelay = 3;
        /// <summary>Peers compare state every tick, so a desync report names the first diverging tick.</summary>
        public const int HashInterval = 1;
        /// <summary>Replays keep a checkpoint every half second.</summary>
        public const int ReplayHashInterval = 30;

        public const byte KindConnect = 1;
        public const byte KindAccept = 2;
        public const byte KindReject = 3;
        public const byte KindData = 4;
        public const byte KindDisconnect = 5;

        public static void WriteHeader(NetWriter writer, byte kind, uint token)
        {
            writer.Reset();
            writer.U8((byte)'E');
            writer.U8((byte)'N');
            writer.U8(Version);
            writer.U8(kind);
            writer.U32(token);
        }

        /// <returns>False for datagrams that are not Eclipse netplay traffic.</returns>
        public static bool TryReadHeader(NetReader reader, out byte version, out byte kind, out uint token)
        {
            version = kind = 0; token = 0;
            if (reader.Remaining < HeaderSize) return false;
            if (reader.U8() != 'E' || reader.U8() != 'N') return false;
            version = reader.U8();
            kind = reader.U8();
            token = reader.U32();
            return true;
        }
    }

    /// <summary>What a peer announces about its build before a session is accepted.</summary>
    public sealed class NetIdentity
    {
        /// <summary>Game build; peers must match exactly because the simulation is not versioned.</summary>
        public string Build { get; }
        /// <summary>Fingerprint of gameplay content (enabled mods and versions).</summary>
        public string Content { get; }
        public string PlayerName { get; }

        public NetIdentity(string build, string content, string playerName)
        {
            Build = build ?? string.Empty;
            Content = content ?? string.Empty;
            PlayerName = string.IsNullOrWhiteSpace(playerName) ? "Player" : playerName.Trim();
            if (PlayerName.Length > 24) PlayerName = PlayerName.Substring(0, 24);
        }

        public void Write(NetWriter writer)
        {
            writer.Str(Build);
            writer.Str(Content);
            writer.Str(PlayerName);
        }

        public static NetIdentity Read(NetReader reader) => new NetIdentity(reader.Str(), reader.Str(), reader.Str());

        /// <returns>A player-facing reason, or null when the peers can play together.</returns>
        public string Incompatibility(NetIdentity other)
        {
            if (other == null) return "The other player sent no build information.";
            if (!string.Equals(Build, other.Build, StringComparison.Ordinal))
                return "Game versions differ (" + Build + " vs " + other.Build + ").";
            if (!string.Equals(Content, other.Content, StringComparison.Ordinal))
                return "Enabled mods differ. Both players need the same mods and versions.";
            return null;
        }
    }

    public enum NetMessageType : byte
    {
        /// <summary>Guest to host: the guest's chosen weapon and ready flag.</summary>
        GuestLobby = 1,
        /// <summary>Host to guest: the complete lobby, including both weapon choices.</summary>
        HostLobby = 2,
        StartMatch = 3,
        RequestRematch = 4,
        ReturnToLobby = 5,
        MatchResult = 6,
        Desync = 7,
        Forfeit = 8,
        /// <summary>One peer's exact state on the first tick whose hashes disagreed.</summary>
        DesyncReport = 9,
    }

    /// <summary>Lobby as seen by the host. The host owns arena, format and input delay.</summary>
    public sealed class LobbyState
    {
        public string HostWeapon = string.Empty;
        public string GuestWeapon = string.Empty;
        public string Arena = string.Empty;
        public int WinsRequired = 2;
        public int RoundTimeSeconds = 99;
        public int InputDelay = NetProtocol.DefaultInputDelay;
        public bool GuestReady;

        public byte[] Encode()
        {
            var writer = new NetWriter(256);
            writer.U8((byte)NetMessageType.HostLobby);
            writer.Str(HostWeapon);
            writer.Str(GuestWeapon);
            writer.Str(Arena);
            writer.U8((byte)WinsRequired);
            writer.U16((ushort)RoundTimeSeconds);
            writer.U8((byte)InputDelay);
            writer.Bool(GuestReady);
            return writer.ToArray();
        }

        public static LobbyState Decode(NetReader reader)
        {
            var state = new LobbyState
            {
                HostWeapon = reader.Str(),
                GuestWeapon = reader.Str(),
                Arena = reader.Str(),
                WinsRequired = reader.U8(),
                RoundTimeSeconds = reader.U16(),
                InputDelay = reader.U8(),
                GuestReady = reader.Bool(),
            };
            if (state.InputDelay > NetProtocol.MaxInputDelay) throw new NetFormatException("Input delay out of range.");
            return state;
        }
    }

    /// <summary>The host's authoritative description of one match. Both peers build the fight from it.</summary>
    public sealed class MatchStart
    {
        public int MatchIndex;
        public string HostWeapon = string.Empty;
        public string GuestWeapon = string.Empty;
        public string Arena = string.Empty;
        public int WinsRequired;
        public int RoundTimeSeconds;
        public int InputDelay;
        public int Seed;

        public byte[] Encode()
        {
            var writer = new NetWriter(256);
            writer.U8((byte)NetMessageType.StartMatch);
            writer.U8((byte)MatchIndex);
            writer.Str(HostWeapon);
            writer.Str(GuestWeapon);
            writer.Str(Arena);
            writer.U8((byte)WinsRequired);
            writer.U16((ushort)RoundTimeSeconds);
            writer.U8((byte)InputDelay);
            writer.I32(Seed);
            return writer.ToArray();
        }

        public static MatchStart Decode(NetReader reader)
        {
            var start = new MatchStart
            {
                MatchIndex = reader.U8(),
                HostWeapon = reader.Str(),
                GuestWeapon = reader.Str(),
                Arena = reader.Str(),
                WinsRequired = reader.U8(),
                RoundTimeSeconds = reader.U16(),
                InputDelay = reader.U8(),
                Seed = reader.I32(),
            };
            if (start.InputDelay > NetProtocol.MaxInputDelay) throw new NetFormatException("Input delay out of range.");
            return start;
        }
    }

    public static class NetMessages
    {
        public static byte[] GuestLobby(string weapon, bool ready)
        {
            var writer = new NetWriter(128);
            writer.U8((byte)NetMessageType.GuestLobby);
            writer.Str(weapon);
            writer.Bool(ready);
            return writer.ToArray();
        }

        public static byte[] Simple(NetMessageType type, int matchIndex)
        {
            return new[] { (byte)type, (byte)matchIndex };
        }

        /// <param name="winner">0 host/left, 1 guest/right, 255 none.</param>
        public static byte[] MatchResult(int matchIndex, int winner, int leftRounds, int rightRounds, int finalTick)
        {
            var writer = new NetWriter(32);
            writer.U8((byte)NetMessageType.MatchResult);
            writer.U8((byte)matchIndex);
            writer.U8((byte)winner);
            writer.U8((byte)leftRounds);
            writer.U8((byte)rightRounds);
            writer.I32(finalTick);
            return writer.ToArray();
        }

        public const int MaxReportBytes = 360;

        public static byte[] DesyncReport(int matchIndex, int tick, string platform, string state)
        {
            var writer = new NetWriter(ReliableChannel.MaxMessageSize);
            writer.U8((byte)NetMessageType.DesyncReport);
            writer.U8((byte)matchIndex);
            writer.I32(tick);
            writer.Str(platform.Length > 40 ? platform.Substring(0, 40) : platform);
            byte[] text = System.Text.Encoding.UTF8.GetBytes(state ?? string.Empty);
            int length = Math.Min(text.Length, MaxReportBytes - writer.Length);
            writer.U16((ushort)length);
            writer.Bytes(text, 0, length);
            return writer.ToArray();
        }

        public static byte[] Desync(int matchIndex, int tick)
        {
            var writer = new NetWriter(16);
            writer.U8((byte)NetMessageType.Desync);
            writer.U8((byte)matchIndex);
            writer.I32(tick);
            return writer.ToArray();
        }
    }
}
