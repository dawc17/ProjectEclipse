using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Eclipse.Content
{
    // The legacy XML adapters require real files (including relative includes).
    // Package one local resource, then extract a versioned copy without touching saves.
    public static class GameplayContentArchive
    {
        public const string ResourcePath = "SF2Content/gameplay";
        private const string Magic = "SF2XML1";
        private const int MaxFiles = 10000;
        private const int MaxFileBytes = 64 * 1024 * 1024;
        private const long MaxTotalBytes = 512L * 1024 * 1024;

        public static string GetXmlRoot()
        {
            if (Application.isEditor)
                return Path.Combine(Application.dataPath, "xml");

            TextAsset asset = Resources.Load<TextAsset>(ResourcePath);
            if (asset == null)
                throw new InvalidDataException("Packaged gameplay XML is missing. Rebuild using SF2's content build processor.");
            try
            {
                return ExtractArchive(asset.bytes, Path.Combine(Application.persistentDataPath, "Content/gameplay"));
            }
            finally
            {
                Resources.UnloadAsset(asset);
            }
        }

        public static byte[] CreateArchive(string sourceDirectory)
        {
            string root = Path.GetFullPath(sourceDirectory).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
            string[] files = Directory.GetFiles(root, "*", SearchOption.AllDirectories)
                .Where(path => !path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                .OrderBy(path => path, StringComparer.Ordinal).ToArray();
            if (files.Length == 0 || files.Length > MaxFiles)
                throw new InvalidDataException("Invalid gameplay XML file count.");
            using (var output = new MemoryStream())
            {
                using (var gzip = new GZipStream(output, CompressionMode.Compress, true))
                using (var writer = new BinaryWriter(gzip, Encoding.UTF8))
                {
                    writer.Write(Magic);
                    writer.Write(files.Length);
                    long total = 0;
                    foreach (string file in files)
                    {
                        byte[] data = File.ReadAllBytes(file);
                        total += data.Length;
                        if (data.Length > MaxFileBytes || total > MaxTotalBytes)
                            throw new InvalidDataException("Gameplay XML archive exceeds size limit.");
                        writer.Write(file.Substring(root.Length).Replace('\\', '/'));
                        writer.Write(data.Length);
                        writer.Write(data);
                    }
                }
                return output.ToArray();
            }
        }

        public static string ExtractArchive(byte[] archive, string cacheDirectory)
        {
            string version;
            using (var sha = SHA256.Create())
                version = BitConverter.ToString(sha.ComputeHash(archive)).Replace("-", "").ToLowerInvariant();
            string root = Path.GetFullPath(Path.Combine(cacheDirectory, version));
            string complete = Path.Combine(root, ".complete");
            if (File.Exists(complete))
                return root;

            // Validate the entire archive before writing anything. Never accept rooted
            // paths, traversal, duplicate paths or truncated data from an archive.
            var files = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
            using (var input = new MemoryStream(archive, false))
            using (var gzip = new GZipStream(input, CompressionMode.Decompress))
            using (var reader = new BinaryReader(gzip, Encoding.UTF8))
            {
                if (reader.ReadString() != Magic)
                    throw new InvalidDataException("Unknown gameplay XML archive format.");
                int count = reader.ReadInt32();
                if (count < 1 || count > MaxFiles)
                    throw new InvalidDataException("Invalid gameplay XML file count.");
                long total = 0;
                for (int i = 0; i < count; i++)
                {
                    string relative = reader.ReadString();
                    string[] parts = relative.Split('/');
                    if (relative.Length > 1024 || relative.Contains("\\") || relative.Contains(":") ||
                        parts.Any(part => string.IsNullOrEmpty(part) || part == "." || part == ".." ||
                            part == ".complete" || part.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0))
                        throw new InvalidDataException("Invalid gameplay XML archive path: " + relative);
                    int length = reader.ReadInt32();
                    total += length;
                    if (length < 0 || length > MaxFileBytes || total > MaxTotalBytes)
                        throw new InvalidDataException("Invalid gameplay XML archive size.");
                    byte[] data = reader.ReadBytes(length);
                    if (data.Length != length || files.ContainsKey(relative))
                        throw new InvalidDataException("Truncated or duplicate gameplay XML archive entry.");
                    files.Add(relative, data);
                }
                if (reader.BaseStream.ReadByte() != -1)
                    throw new InvalidDataException("Trailing gameplay XML archive data.");
            }
            foreach (var file in files)
            {
                string path = Path.Combine(root, file.Key.Replace('/', Path.DirectorySeparatorChar));
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllBytes(path, file.Value);
            }
            File.WriteAllText(complete, version);
            return root;
        }
    }
}
