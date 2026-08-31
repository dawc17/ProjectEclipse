using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Eclipse.Content.TarAssets
{
    // Deliberately small USTAR implementation. Runtime bundles contain regular files only;
    // keeping the reader here avoids making TAR itself another player dependency.
    internal sealed class TarArchive
    {
        internal sealed class Entry
        {
            public string Name;
            public long Offset;
            public long Size;
        }

        private const int BlockSize = 512;
        private readonly string _path;
        private readonly Dictionary<string, Entry> _entries;

        private TarArchive(string path, Dictionary<string, Entry> entries)
        {
            _path = path;
            _entries = entries;
        }

        public IEnumerable<Entry> Entries { get { return _entries.Values; } }

        public static TarArchive Open(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                throw new FileNotFoundException("TAR asset cache is missing.", path);

            var entries = new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
            using (FileStream input = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] header = new byte[BlockSize];
                int zeroBlocks = 0;
                while (input.Position + BlockSize <= input.Length)
                {
                    ReadExact(input, header, 0, header.Length);
                    if (IsZeroBlock(header))
                    {
                        zeroBlocks++;
                        if (zeroBlocks >= 2)
                            break;
                        continue;
                    }
                    zeroBlocks = 0;

                    ValidateChecksum(header);
                    char type = (char)header[156];
                    string name = ReadString(header, 0, 100);
                    string prefix = ReadString(header, 345, 155);
                    if (!string.IsNullOrEmpty(prefix))
                        name = prefix + "/" + name;
                    name = NormalizeEntryPath(name);

                    long size = ReadOctal(header, 124, 12);
                    long dataOffset = input.Position;
                    long padded = RoundToBlock(size);
                    if (size < 0 || dataOffset + padded > input.Length)
                        throw new InvalidDataException("TAR entry exceeds archive bounds: " + name);

                    // USTAR regular file is '\0' or '0'. Directories are harmless and ignored.
                    if (type == '\0' || type == '0')
                    {
                        if (entries.ContainsKey(name))
                            throw new InvalidDataException("Duplicate TAR entry: " + name);
                        entries.Add(name, new Entry { Name = name, Offset = dataOffset, Size = size });
                    }
                    else if (type != '5')
                    {
                        throw new InvalidDataException("Unsupported TAR entry type '" + type + "': " + name);
                    }

                    input.Position = dataOffset + padded;
                }
            }
            return new TarArchive(path, entries);
        }

        public bool Contains(string path)
        {
            Entry ignored;
            return _entries.TryGetValue(NormalizeEntryPath(path), out ignored);
        }

        public byte[] ReadBytes(string path)
        {
            Entry entry;
            string normalized = NormalizeEntryPath(path);
            if (!_entries.TryGetValue(normalized, out entry))
                throw new FileNotFoundException("Entry is missing from TAR asset bundle: " + normalized, _path);
            if (entry.Size > int.MaxValue)
                throw new InvalidDataException("TAR entry is too large to load into memory: " + normalized);

            byte[] bytes = new byte[(int)entry.Size];
            using (FileStream input = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                input.Position = entry.Offset;
                ReadExact(input, bytes, 0, bytes.Length);
            }
            return bytes;
        }

        public string ReadText(string path)
        {
            return Encoding.UTF8.GetString(ReadBytes(path));
        }

        public static string NormalizeEntryPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new InvalidDataException("Empty TAR path.");
            string normalized = path.Replace('\\', '/').Trim().TrimStart('/');
            if (normalized.IndexOf(':') >= 0)
                throw new InvalidDataException("Drive-qualified TAR paths are not allowed: " + path);
            string[] parts = normalized.Split('/');
            foreach (string part in parts)
            {
                if (string.IsNullOrEmpty(part) || part == "." || part == "..")
                    throw new InvalidDataException("Unsafe TAR path: " + path);
            }
            return normalized;
        }

        private static long RoundToBlock(long value)
        {
            return (value + BlockSize - 1) / BlockSize * BlockSize;
        }

        private static bool IsZeroBlock(byte[] block)
        {
            for (int i = 0; i < block.Length; i++)
                if (block[i] != 0) return false;
            return true;
        }

        private static string ReadString(byte[] bytes, int offset, int length)
        {
            int end = offset;
            int limit = offset + length;
            while (end < limit && bytes[end] != 0)
                end++;
            return Encoding.UTF8.GetString(bytes, offset, end - offset).Trim();
        }

        private static long ReadOctal(byte[] bytes, int offset, int length)
        {
            string value = ReadString(bytes, offset, length).Trim();
            if (value.Length == 0) return 0;
            long result = 0;
            foreach (char c in value)
            {
                if (c < '0' || c > '7')
                    throw new InvalidDataException("Invalid TAR octal field: " + value);
                checked { result = result * 8 + (c - '0'); }
            }
            return result;
        }

        private static void ValidateChecksum(byte[] header)
        {
            long expected = ReadOctal(header, 148, 8);
            long actual = 0;
            for (int i = 0; i < header.Length; i++)
                actual += (i >= 148 && i < 156) ? 0x20 : header[i];
            if (expected != actual)
                throw new InvalidDataException("TAR header checksum mismatch.");
        }

        private static void ReadExact(Stream input, byte[] buffer, int offset, int count)
        {
            while (count > 0)
            {
                int read = input.Read(buffer, offset, count);
                if (read <= 0) throw new EndOfStreamException();
                offset += read;
                count -= read;
            }
        }
    }

    public sealed class TarWriter : IDisposable
    {
        private const int BlockSize = 512;
        private readonly Stream _output;
        private bool _finished;

        public TarWriter(Stream output)
        {
            _output = output ?? throw new ArgumentNullException("output");
            if (!output.CanWrite) throw new ArgumentException("Output stream is not writable.", "output");
        }

        public void AddFile(string path, byte[] bytes)
        {
            using (var input = new MemoryStream(bytes ?? Array.Empty<byte>(), false))
                AddFile(path, input, input.Length);
        }

        public void AddFile(string path, string sourceFile)
        {
            using (FileStream input = File.Open(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                AddFile(path, input, input.Length);
        }

        private void AddFile(string path, Stream input, long length)
        {
            if (_finished) throw new InvalidOperationException("TAR has already been finalized.");
            string normalized = TarArchive.NormalizeEntryPath(path);
            byte[] header = BuildHeader(normalized, length);
            _output.Write(header, 0, header.Length);
            input.CopyTo(_output);
            int padding = (int)((BlockSize - (length % BlockSize)) % BlockSize);
            if (padding != 0) _output.Write(new byte[padding], 0, padding);
        }

        public void Finish()
        {
            if (_finished) return;
            byte[] end = new byte[BlockSize * 2];
            _output.Write(end, 0, end.Length);
            _output.Flush();
            _finished = true;
        }

        public void Dispose()
        {
            Finish();
        }

        private static byte[] BuildHeader(string path, long size)
        {
            string name = path;
            string prefix = string.Empty;
            byte[] encoded = Encoding.UTF8.GetBytes(path);
            if (encoded.Length > 100)
            {
                int slash = path.LastIndexOf('/');
                while (slash > 0)
                {
                    prefix = path.Substring(0, slash);
                    name = path.Substring(slash + 1);
                    if (Encoding.UTF8.GetByteCount(name) <= 100 && Encoding.UTF8.GetByteCount(prefix) <= 155)
                        break;
                    slash = path.LastIndexOf('/', slash - 1);
                }
                if (Encoding.UTF8.GetByteCount(name) > 100 || Encoding.UTF8.GetByteCount(prefix) > 155)
                    throw new InvalidDataException("Path is too long for USTAR: " + path);
            }

            byte[] header = new byte[BlockSize];
            WriteString(header, 0, 100, name);
            WriteOctal(header, 100, 8, 420); // 0644
            WriteOctal(header, 108, 8, 0);
            WriteOctal(header, 116, 8, 0);
            WriteOctal(header, 124, 12, size);
            WriteOctal(header, 136, 12, 0); // deterministic mtime
            for (int i = 148; i < 156; i++) header[i] = 0x20;
            header[156] = (byte)'0';
            WriteString(header, 257, 6, "ustar");
            header[262] = 0;
            WriteString(header, 263, 2, "00");
            WriteString(header, 345, 155, prefix);

            long checksum = 0;
            foreach (byte value in header) checksum += value;
            string check = Convert.ToString(checksum, 8).PadLeft(6, '0');
            WriteRawAscii(header, 148, check);
            header[154] = 0;
            header[155] = 0x20;
            return header;
        }

        private static void WriteString(byte[] header, int offset, int length, string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value ?? string.Empty);
            if (bytes.Length > length) throw new InvalidDataException("USTAR field is too long: " + value);
            Buffer.BlockCopy(bytes, 0, header, offset, bytes.Length);
        }

        private static void WriteRawAscii(byte[] header, int offset, string value)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, header, offset, bytes.Length);
        }

        private static void WriteOctal(byte[] header, int offset, int length, long value)
        {
            string text = Convert.ToString(value, 8);
            if (text.Length > length - 1)
                throw new InvalidDataException("Value does not fit in USTAR field: " + value.ToString(CultureInfo.InvariantCulture));
            text = text.PadLeft(length - 1, '0');
            WriteRawAscii(header, offset, text);
            header[offset + length - 1] = 0;
        }
    }
}
