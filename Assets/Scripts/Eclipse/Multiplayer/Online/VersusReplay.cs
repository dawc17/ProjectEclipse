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
    /// </summary>
    public sealed class VersusReplay
    {
        private static readonly byte[] Magic = Encoding.ASCII.GetBytes("ECLRPL");
        public const byte FormatVersion = 1;
        public const int MaxTicks = 60 * 60 * 60;

        public string Build = string.Empty;
        public string Content = string.Empty;
        public string LeftName = string.Empty;
        public string RightName = string.Empty;
        public string LeftWeapon = string.Empty;
        public string RightWeapon = string.Empty;
        public string Arena = string.Empty;
        public int WinsRequired = 2;
        public int RoundTimeSeconds = 99;
        public int Seed;
        public bool Online;
        public long RecordedUnixSeconds;
        public readonly List<byte> Left = new List<byte>();
        public readonly List<byte> Right = new List<byte>();
        /// <summary>State hash after simulating the tick, recorded every <see cref="NetProtocol.ReplayHashInterval"/> ticks.</summary>
        public readonly SortedDictionary<int, uint> Hashes = new SortedDictionary<int, uint>();

        public int TickCount => Math.Min(Left.Count, Right.Count);

        public void Record(byte left, byte right)
        {
            if (Left.Count >= MaxTicks) return;
            Left.Add(left);
            Right.Add(right);
        }

        public void Write(Stream stream)
        {
            stream.Write(Magic, 0, Magic.Length);
            stream.WriteByte(FormatVersion);
            using (var deflate = new DeflateStream(stream, CompressionLevel.Optimal, true))
            using (var writer = new BinaryWriter(deflate, Encoding.UTF8))
            {
                writer.Write(Build); writer.Write(Content);
                writer.Write(LeftName); writer.Write(RightName);
                writer.Write(LeftWeapon); writer.Write(RightWeapon); writer.Write(Arena);
                writer.Write(WinsRequired); writer.Write(RoundTimeSeconds); writer.Write(Seed);
                writer.Write(Online); writer.Write(RecordedUnixSeconds);
                int ticks = TickCount;
                writer.Write(ticks);
                for (int i = 0; i < ticks; i++) { writer.Write(Left[i]); writer.Write(Right[i]); }
                writer.Write(Hashes.Count);
                foreach (var pair in Hashes) { writer.Write(pair.Key); writer.Write(pair.Value); }
            }
        }

        public static VersusReplay Read(Stream stream)
        {
            var magic = new byte[Magic.Length];
            if (stream.Read(magic, 0, magic.Length) != magic.Length || !BytesEqual(magic, Magic)) throw new InvalidDataException("Not an Eclipse replay.");
            int version = stream.ReadByte();
            if (version != FormatVersion) throw new InvalidDataException("Unsupported replay version " + version + ".");
            var replay = new VersusReplay();
            using (var deflate = new DeflateStream(stream, CompressionMode.Decompress, true))
            using (var reader = new BinaryReader(deflate, Encoding.UTF8))
            {
                replay.Build = reader.ReadString(); replay.Content = reader.ReadString();
                replay.LeftName = reader.ReadString(); replay.RightName = reader.ReadString();
                replay.LeftWeapon = reader.ReadString(); replay.RightWeapon = reader.ReadString(); replay.Arena = reader.ReadString();
                replay.WinsRequired = reader.ReadInt32(); replay.RoundTimeSeconds = reader.ReadInt32(); replay.Seed = reader.ReadInt32();
                replay.Online = reader.ReadBoolean(); replay.RecordedUnixSeconds = reader.ReadInt64();
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
            return replay;
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
