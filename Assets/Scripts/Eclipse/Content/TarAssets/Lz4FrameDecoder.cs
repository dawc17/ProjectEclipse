using System;
using System.IO;

namespace Eclipse.Content.TarAssets
{
    // Small, dependency-free decoder for the standard LZ4 Frame format produced by AssetPacker.
    // It supports both independent and chained blocks and deliberately performs no unsafe memory
    // operations, which keeps it compatible with Unity 2022.3's existing managed dependency set.
    internal static class Lz4FrameDecoder
    {
        private const uint Magic = 0x184D2204;
        private const int HistorySize = 64 * 1024;

        public static void Decode(Stream source, Stream target, long expectedLength)
        {
            if (source == null || !source.CanRead) throw new ArgumentException("LZ4 source must be readable.", "source");
            if (target == null || !target.CanWrite) throw new ArgumentException("LZ4 target must be writable.", "target");

            uint magic = ReadUInt32(source);
            if (magic != Magic) throw new InvalidDataException("Invalid LZ4 Frame magic.");

            int flg = ReadByte(source);
            int bd = ReadByte(source);
            if ((flg & 0xC0) != 0x40) throw new InvalidDataException("Unsupported LZ4 Frame version.");
            if ((flg & 0x02) != 0) throw new InvalidDataException("Reserved LZ4 Frame flag is set.");

            bool independentBlocks = (flg & 0x20) != 0;
            bool blockChecksum = (flg & 0x10) != 0;
            bool hasContentSize = (flg & 0x08) != 0;
            bool contentChecksum = (flg & 0x04) != 0;
            bool hasDictionary = (flg & 0x01) != 0;
            int maximumBlockSize = DecodeMaximumBlockSize(bd);

            ulong declaredContentSize = 0;
            if (hasContentSize)
                declaredContentSize = ReadUInt64(source);
            if (hasDictionary)
                throw new InvalidDataException("Dictionary-backed LZ4 Frames are not supported.");
            ReadByte(source); // Header checksum. The catalog SHA-256 authenticates the complete frame.

            if (declaredContentSize != 0 && expectedLength > 0 && declaredContentSize != (ulong)expectedLength)
                throw new InvalidDataException("LZ4 Frame content size differs from catalog size.");

            byte[] history = new byte[HistorySize];
            int historyLength = 0;
            long total = 0;

            while (true)
            {
                uint blockHeader = ReadUInt32(source);
                if (blockHeader == 0) break;

                bool uncompressed = (blockHeader & 0x80000000u) != 0;
                int storedSize = checked((int)(blockHeader & 0x7FFFFFFFu));
                if (storedSize <= 0 || storedSize > maximumBlockSize)
                    throw new InvalidDataException("Invalid LZ4 Frame block size: " + storedSize);

                byte[] stored = ReadExact(source, storedSize);
                byte[] decoded;
                int decodedLength;
                if (uncompressed)
                {
                    decoded = stored;
                    decodedLength = stored.Length;
                }
                else
                {
                    decoded = new byte[maximumBlockSize];
                    decodedLength = DecodeBlock(stored, decoded, history, independentBlocks ? 0 : historyLength);
                }

                total += decodedLength;
                if (expectedLength > 0 && total > expectedLength)
                    throw new InvalidDataException("LZ4 Frame expands beyond catalog size.");
                target.Write(decoded, 0, decodedLength);

                if (independentBlocks)
                    historyLength = 0;
                else
                    UpdateHistory(history, ref historyLength, decoded, decodedLength);

                if (blockChecksum)
                    ReadUInt32(source); // Integrity is already covered by the catalog SHA-256.
            }

            if (contentChecksum)
                ReadUInt32(source); // Same rationale as block checksum above.
            if (expectedLength > 0 && total != expectedLength)
                throw new InvalidDataException("LZ4 Frame decoded size mismatch: " + total + " != " + expectedLength);
            if (hasContentSize && declaredContentSize != 0 && total != (long)declaredContentSize)
                throw new InvalidDataException("LZ4 Frame decoded size differs from its content-size field.");
        }

        private static int DecodeBlock(byte[] input, byte[] output, byte[] history, int historyLength)
        {
            int source = 0;
            int target = 0;
            while (source < input.Length)
            {
                int token = input[source++];
                int literalLength = token >> 4;
                if (literalLength == 15)
                    literalLength += ReadLength(input, ref source);
                EnsureAvailable(input.Length, source, literalLength, "LZ4 literal overruns compressed block.");
                EnsureOutput(output.Length, target, literalLength);
                Buffer.BlockCopy(input, source, output, target, literalLength);
                source += literalLength;
                target += literalLength;

                // A final literal run ends the block and has no match offset.
                if (source == input.Length) break;
                EnsureAvailable(input.Length, source, 2, "LZ4 match offset is truncated.");
                int offset = input[source] | (input[source + 1] << 8);
                source += 2;
                if (offset <= 0 || offset > historyLength + target)
                    throw new InvalidDataException("Invalid LZ4 match offset: " + offset);

                int matchLength = token & 0x0F;
                if (matchLength == 15)
                    matchLength += ReadLength(input, ref source);
                matchLength += 4;
                EnsureOutput(output.Length, target, matchLength);

                for (int i = 0; i < matchLength; i++)
                {
                    int virtualSource = historyLength + target - offset;
                    output[target++] = virtualSource < historyLength
                        ? history[virtualSource]
                        : output[virtualSource - historyLength];
                }
            }
            return target;
        }

        private static int ReadLength(byte[] input, ref int position)
        {
            int total = 0;
            while (true)
            {
                if (position >= input.Length) throw new InvalidDataException("Truncated LZ4 length extension.");
                int value = input[position++];
                total = checked(total + value);
                if (value != 255) return total;
            }
        }

        private static void UpdateHistory(byte[] history, ref int historyLength, byte[] block, int blockLength)
        {
            if (blockLength >= HistorySize)
            {
                Buffer.BlockCopy(block, blockLength - HistorySize, history, 0, HistorySize);
                historyLength = HistorySize;
                return;
            }

            int keep = Math.Min(historyLength, HistorySize - blockLength);
            if (keep > 0 && historyLength > keep)
                Buffer.BlockCopy(history, historyLength - keep, history, 0, keep);
            Buffer.BlockCopy(block, 0, history, keep, blockLength);
            historyLength = keep + blockLength;
        }

        private static int DecodeMaximumBlockSize(int bd)
        {
            if ((bd & 0x8F) != 0) throw new InvalidDataException("Reserved LZ4 Frame BD bits are set.");
            switch ((bd >> 4) & 0x07)
            {
                case 4: return 64 * 1024;
                case 5: return 256 * 1024;
                case 6: return 1024 * 1024;
                case 7: return 4 * 1024 * 1024;
                default: throw new InvalidDataException("Unsupported LZ4 Frame block-size code.");
            }
        }

        private static void EnsureAvailable(int length, int position, int count, string message)
        {
            if (count < 0 || position < 0 || position > length - count)
                throw new InvalidDataException(message);
        }

        private static void EnsureOutput(int length, int position, int count)
        {
            if (count < 0 || position < 0 || position > length - count)
                throw new InvalidDataException("LZ4 block expands beyond its declared maximum size.");
        }

        private static int ReadByte(Stream source)
        {
            int value = source.ReadByte();
            if (value < 0) throw new EndOfStreamException("Unexpected end of LZ4 Frame.");
            return value;
        }

        private static uint ReadUInt32(Stream source)
        {
            uint a = (uint)ReadByte(source);
            uint b = (uint)ReadByte(source);
            uint c = (uint)ReadByte(source);
            uint d = (uint)ReadByte(source);
            return a | (b << 8) | (c << 16) | (d << 24);
        }

        private static ulong ReadUInt64(Stream source)
        {
            ulong low = ReadUInt32(source);
            ulong high = ReadUInt32(source);
            return low | (high << 32);
        }

        private static byte[] ReadExact(Stream source, int count)
        {
            byte[] result = new byte[count];
            int offset = 0;
            while (offset < count)
            {
                int read = source.Read(result, offset, count - offset);
                if (read <= 0) throw new EndOfStreamException("Unexpected end of LZ4 Frame block.");
                offset += read;
            }
            return result;
        }
    }
}
