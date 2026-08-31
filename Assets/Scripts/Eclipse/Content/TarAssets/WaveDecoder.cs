using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Eclipse.Content.TarAssets
{
    internal static class WaveDecoder
    {
        public static AudioClip Decode(byte[] bytes, string name)
        {
            if (bytes == null || bytes.Length < 44)
                throw new InvalidDataException("WAV is truncated: " + name);

            using (var input = new MemoryStream(bytes, false))
            using (var reader = new BinaryReader(input, Encoding.ASCII))
            {
                Require(ReadFourCc(reader) == "RIFF", "WAV is not RIFF: " + name);
                reader.ReadUInt32();
                Require(ReadFourCc(reader) == "WAVE", "WAV is not WAVE: " + name);

                ushort format = 0;
                ushort channels = 0;
                uint sampleRate = 0;
                ushort bits = 0;
                byte[] data = null;

                while (input.Position + 8 <= input.Length)
                {
                    string chunk = ReadFourCc(reader);
                    uint size = reader.ReadUInt32();
                    long next = input.Position + size + (size & 1u);
                    Require(next <= input.Length, "WAV chunk exceeds file bounds: " + name);
                    if (chunk == "fmt ")
                    {
                        Require(size >= 16, "WAV fmt chunk is truncated: " + name);
                        format = reader.ReadUInt16();
                        channels = reader.ReadUInt16();
                        sampleRate = reader.ReadUInt32();
                        reader.ReadUInt32(); // byte rate
                        reader.ReadUInt16(); // block align
                        bits = reader.ReadUInt16();
                    }
                    else if (chunk == "data")
                    {
                        if (size > int.MaxValue) throw new InvalidDataException("WAV data is too large: " + name);
                        data = reader.ReadBytes((int)size);
                        Require(data.Length == (int)size, "WAV data is truncated: " + name);
                    }
                    input.Position = next;
                }

                Require(format == 1, "Only PCM WAV is supported in TAR assets: " + name);
                Require(bits == 16, "Only 16-bit WAV is supported in TAR assets: " + name);
                Require(channels > 0 && sampleRate > 0 && data != null, "Incomplete WAV metadata: " + name);
                Require(data.Length % (channels * 2) == 0, "WAV sample data is misaligned: " + name);

                int sampleValues = data.Length / 2;
                float[] samples = new float[sampleValues];
                for (int i = 0, p = 0; i < sampleValues; i++, p += 2)
                {
                    short value = (short)(data[p] | (data[p + 1] << 8));
                    samples[i] = value / 32768f;
                }

                int frames = sampleValues / channels;
                AudioClip clip = AudioClip.Create(name, frames, channels, (int)sampleRate, false);
                Require(clip != null && clip.SetData(samples, 0), "Unity rejected WAV samples: " + name);
                return clip;
            }
        }

        private static string ReadFourCc(BinaryReader reader)
        {
            return Encoding.ASCII.GetString(reader.ReadBytes(4));
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidDataException(message);
        }
    }
}
