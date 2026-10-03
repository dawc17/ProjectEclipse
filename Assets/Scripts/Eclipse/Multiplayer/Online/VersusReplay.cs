using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;

namespace Eclipse.Multiplayer.Online
{
    /// <summary>
    /// A versus match as data: the matchup, the seed, both fighters' per-tick inputs
    /// and periodic state hashes. Replaying it through the same build reproduces the
    /// match exactly, and a hash mismatch pinpoints where the simulation diverged.
    /// <para>
    /// Format 3 adds the immutable balance JSON and its hash to the uncompressed header.
    /// Format 2 keeps a small uncompressed header in front of the compressed inputs, so
    /// a replay browser can list matchups, results and lengths without inflating files.
    /// Format 1 files (weapon only, no result) still load.
    /// </para>
    /// </summary>
    public sealed class VersusReplay
    {
        private static readonly byte[] Magic = Encoding.ASCII.GetBytes("ECLRPL");
        public const byte FormatVersion = 3;
        public const int MaxTicks = 60 * 60 * 60;
        /// <summary>Loadout slots in order: weapon, armor, helm, ranged, magic.</summary>
        public const int LoadoutSlots = 5;
        public const int WinnerUnknown = -2;
        private const int MaxHeaderBytes = 64 * 1024;
        private const int MaxRoundEnds = 64;

        public string Build = string.Empty;
        public string Content = string.Empty;
        public string BalanceHash = string.Empty;
        public string BalanceJson = string.Empty;
        /// <summary>A name the player gave the replay, or empty.</summary>
        public string Title = string.Empty;
        public string LeftName = string.Empty;
        public string RightName = string.Empty;
        /// <summary>Item ids per <see cref="LoadoutSlots"/>; an empty slot means the game default.</summary>
        public readonly string[] LeftLoadout = NewLoadout();
        public readonly string[] RightLoadout = NewLoadout();
        public string Arena = string.Empty;
        public int WinsRequired = 2;
        public int RoundTimeSeconds = 99;
        public int Seed;
        public bool Online;
        public long RecordedUnixSeconds;
        /// <summary>0 left, 1 right, -1 no winner, <see cref="WinnerUnknown"/> when not recorded (format 1, or cut short).</summary>
        public int Winner = WinnerUnknown;
        public int LeftRounds, RightRounds;
        /// <summary>The tick each round ended on, for timeline markers.</summary>
        public readonly List<int> RoundEnds = new List<int>();
        /// <summary>Kept replays are never pruned.</summary>
        public bool Kept;
        /// <summary>Why the replay was saved when not a normal finish ("desync", "disconnect"), or empty.</summary>
        public string Tag = string.Empty;
        /// <summary>Set by <see cref="ReadHeader"/>: inputs and hashes were not loaded.</summary>
        public bool HeaderOnly { get; private set; }
        /// <summary>Tick count stored in the header (valid for header-only reads).</summary>
        public int StoredTicks { get; private set; }
        public int SourceFormat { get; private set; } = FormatVersion;

        public readonly List<byte> Left = new List<byte>();
        public readonly List<byte> Right = new List<byte>();
        /// <summary>State hash after simulating the tick, recorded every <see cref="NetProtocol.ReplayHashInterval"/> ticks.</summary>
        public readonly SortedDictionary<int, uint> Hashes = new SortedDictionary<int, uint>();

        public int TickCount => HeaderOnly ? StoredTicks : Math.Min(Left.Count, Right.Count);
        public string LeftWeapon => LeftLoadout[0];
        public string RightWeapon => RightLoadout[0];

        private static string[] NewLoadout()
        {
            var slots = new string[LoadoutSlots];
            for (int i = 0; i < slots.Length; i++) slots[i] = string.Empty;
            return slots;
        }

        public void Record(byte left, byte right)
        {
            if (Left.Count >= MaxTicks) return;
            Left.Add(left);
            Right.Add(right);
        }

        public void Write(Stream stream)
        {
            if (HeaderOnly) throw new InvalidOperationException("Load the whole replay before saving it.");
            stream.Write(Magic, 0, Magic.Length);
            stream.WriteByte(FormatVersion);
            byte[] header;
            using (var buffer = new MemoryStream())
            {
                using (var writer = new BinaryWriter(buffer, Encoding.UTF8, true)) WriteHeader(writer);
                header = buffer.ToArray();
            }
            if (header.Length > MaxHeaderBytes) throw new InvalidDataException("Replay header is too large.");
            var length = BitConverter.GetBytes(header.Length);
            if (!BitConverter.IsLittleEndian) Array.Reverse(length);
            stream.Write(length, 0, 4);
            stream.Write(header, 0, header.Length);
            using (var deflate = new DeflateStream(stream, CompressionLevel.Optimal, true))
            using (var writer = new BinaryWriter(deflate, Encoding.UTF8))
            {
                int ticks = TickCount;
                writer.Write(ticks);
                for (int i = 0; i < ticks; i++) { writer.Write(Left[i]); writer.Write(Right[i]); }
                writer.Write(Hashes.Count);
                foreach (var pair in Hashes) { writer.Write(pair.Key); writer.Write(pair.Value); }
            }
        }

        private void WriteHeader(BinaryWriter writer)
        {
            writer.Write(Build); writer.Write(Content); writer.Write(Title ?? string.Empty);
            writer.Write(LeftName); writer.Write(RightName);
            foreach (string slot in LeftLoadout) writer.Write(slot ?? string.Empty);
            foreach (string slot in RightLoadout) writer.Write(slot ?? string.Empty);
            writer.Write(Arena);
            writer.Write(WinsRequired); writer.Write(RoundTimeSeconds); writer.Write(Seed);
            writer.Write(Online); writer.Write(RecordedUnixSeconds);
            writer.Write(TickCount);
            writer.Write(Winner); writer.Write(LeftRounds); writer.Write(RightRounds);
            int ends = Math.Min(RoundEnds.Count, MaxRoundEnds);
            writer.Write(ends);
            for (int i = 0; i < ends; i++) writer.Write(RoundEnds[i]);
            writer.Write(Kept);
            writer.Write(Tag ?? string.Empty);
            writer.Write(BalanceHash ?? string.Empty);
            writer.Write(BalanceJson ?? string.Empty);
        }

        private void ReadHeaderFields(BinaryReader reader)
        {
            Build = reader.ReadString(); Content = reader.ReadString(); Title = reader.ReadString();
            LeftName = reader.ReadString(); RightName = reader.ReadString();
            for (int i = 0; i < LoadoutSlots; i++) LeftLoadout[i] = reader.ReadString();
            for (int i = 0; i < LoadoutSlots; i++) RightLoadout[i] = reader.ReadString();
            Arena = reader.ReadString();
            WinsRequired = reader.ReadInt32(); RoundTimeSeconds = reader.ReadInt32(); Seed = reader.ReadInt32();
            Online = reader.ReadBoolean(); RecordedUnixSeconds = reader.ReadInt64();
            StoredTicks = reader.ReadInt32();
            if (StoredTicks < 0 || StoredTicks > MaxTicks) throw new InvalidDataException("Replay length is out of range.");
            Winner = reader.ReadInt32(); LeftRounds = reader.ReadInt32(); RightRounds = reader.ReadInt32();
            int ends = reader.ReadInt32();
            if (ends < 0 || ends > MaxRoundEnds) throw new InvalidDataException("Replay round count is out of range.");
            for (int i = 0; i < ends; i++) RoundEnds.Add(reader.ReadInt32());
            Kept = reader.ReadBoolean();
            Tag = reader.ReadString();
            if (SourceFormat >= 3)
            {
                BalanceHash = reader.ReadString();
                BalanceJson = reader.ReadString();
                if (Encoding.UTF8.GetByteCount(BalanceJson) > 32768) throw new InvalidDataException("Replay balance profile exceeds 32 KiB.");
            }
        }

        /// <summary>A whole replay: header, inputs and hashes.</summary>
        public static VersusReplay Read(Stream stream) => Read(stream, headerOnly: false);

        /// <summary>
        /// Only the header: matchup, result, length and flags. Cheap for format 2 files;
        /// format 1 files inflate just their leading fields.
        /// </summary>
        public static VersusReplay ReadHeader(Stream stream) => Read(stream, headerOnly: true);

        private static VersusReplay Read(Stream stream, bool headerOnly)
        {
            var magic = new byte[Magic.Length];
            if (ReadFully(stream, magic) != magic.Length || !BytesEqual(magic, Magic)) throw new InvalidDataException("Not an Eclipse replay.");
            int version = stream.ReadByte();
            var replay = new VersusReplay { SourceFormat = version, HeaderOnly = headerOnly };
            if (version == 1) { ReadVersionOne(stream, replay, headerOnly); return replay; }
            if (version != 2 && version != FormatVersion) throw new InvalidDataException("Unsupported replay version " + version + ".");
            var lengthBytes = new byte[4];
            if (ReadFully(stream, lengthBytes) != 4) throw new InvalidDataException("Replay header is cut short.");
            if (!BitConverter.IsLittleEndian) Array.Reverse(lengthBytes);
            int length = BitConverter.ToInt32(lengthBytes, 0);
            if (length <= 0 || length > MaxHeaderBytes) throw new InvalidDataException("Replay header size is out of range.");
            var header = new byte[length];
            if (ReadFully(stream, header) != length) throw new InvalidDataException("Replay header is cut short.");
            using (var reader = new BinaryReader(new MemoryStream(header), Encoding.UTF8)) replay.ReadHeaderFields(reader);
            if (headerOnly) return replay;
            using (var deflate = new DeflateStream(stream, CompressionMode.Decompress, true))
            using (var reader = new BinaryReader(deflate, Encoding.UTF8))
            {
                ReadInputs(reader, replay);
                if (replay.TickCount != replay.StoredTicks) throw new InvalidDataException("Replay length does not match its header.");
            }
            return replay;
        }

        private static void ReadVersionOne(Stream stream, VersusReplay replay, bool headerOnly)
        {
            using (var deflate = new DeflateStream(stream, CompressionMode.Decompress, true))
            using (var reader = new BinaryReader(deflate, Encoding.UTF8))
            {
                replay.Build = reader.ReadString(); replay.Content = reader.ReadString();
                replay.LeftName = reader.ReadString(); replay.RightName = reader.ReadString();
                replay.LeftLoadout[0] = reader.ReadString(); replay.RightLoadout[0] = reader.ReadString(); replay.Arena = reader.ReadString();
                replay.WinsRequired = reader.ReadInt32(); replay.RoundTimeSeconds = reader.ReadInt32(); replay.Seed = reader.ReadInt32();
                replay.Online = reader.ReadBoolean(); replay.RecordedUnixSeconds = reader.ReadInt64();
                if (headerOnly)
                {
                    int ticks = reader.ReadInt32();
                    if (ticks < 0 || ticks > MaxTicks) throw new InvalidDataException("Replay length is out of range.");
                    replay.StoredTicks = ticks;
                    return;
                }
                ReadInputs(reader, replay);
                replay.StoredTicks = replay.TickCount;
            }
        }

        private static void ReadInputs(BinaryReader reader, VersusReplay replay)
        {
            int ticks = reader.ReadInt32();
            if (ticks < 0 || ticks > MaxTicks) throw new InvalidDataException("Replay length is out of range.");
            for (int i = 0; i < ticks; i++)
            {
                byte left = reader.ReadByte(), right = reader.ReadByte();
                if (!NetInput.IsValid(left) || !NetInput.IsValid(right)) throw new InvalidDataException("Replay contains invalid input.");
                replay.Left.Add(left); replay.Right.Add(right);
            }
            int hashes = reader.ReadInt32();
            if (hashes < 0 || hashes > MaxTicks) throw new InvalidDataException("Replay hash count is out of range.");
            for (int i = 0; i < hashes; i++) replay.Hashes[reader.ReadInt32()] = reader.ReadUInt32();
        }

        private static int ReadFully(Stream stream, byte[] buffer)
        {
            int total = 0, read;
            while (total < buffer.Length && (read = stream.Read(buffer, total, buffer.Length - total)) > 0) total += read;
            return total;
        }

        public void Save(string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            // Unique, so two game instances sharing a data folder never collide.
            string temporary = path + "." + Guid.NewGuid().ToString("N").Substring(0, 8) + ".tmp";
            using (var file = File.Create(temporary)) Write(file);
            if (File.Exists(path)) File.Delete(path);
            File.Move(temporary, path);
        }

        public static VersusReplay Load(string path)
        {
            using (var file = File.OpenRead(path)) return Read(file);
        }

        public static VersusReplay LoadHeader(string path)
        {
            using (var file = File.OpenRead(path)) return ReadHeader(file);
        }

        private static bool BytesEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++) if (a[i] != b[i]) return false;
            return true;
        }
    }

    /// <summary>FNV-1a over exact bit patterns; floats hash by representation, not value.</summary>
    public struct StateHasher
    {
        private const uint Offset = 2166136261;
        private const uint Prime = 16777619;
        private uint _hash;
        private bool _started;

        public uint Value => _started ? _hash : Offset;

        public void Add(int value)
        {
            if (!_started) { _hash = Offset; _started = true; }
            unchecked
            {
                _hash = (_hash ^ (byte)value) * Prime;
                _hash = (_hash ^ (byte)(value >> 8)) * Prime;
                _hash = (_hash ^ (byte)(value >> 16)) * Prime;
                _hash = (_hash ^ (byte)(value >> 24)) * Prime;
            }
        }

        public void Add(bool value) => Add(value ? 1 : 0);
        public void Add(float value) => Add(new FloatBits { Float = value }.Int);

        /// <summary>The exact bit pattern of a float, for logs.</summary>
        public static uint Bits(float value) => unchecked((uint)new FloatBits { Float = value }.Int);

        public void Add(string value)
        {
            if (value == null) { Add(-1); return; }
            Add(value.Length);
            foreach (char c in value) Add(c);
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct FloatBits
        {
            [FieldOffset(0)] public float Float;
            [FieldOffset(0)] public int Int;
        }
    }
}
