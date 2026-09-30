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
        public const string EditorSourceDirectoryName = "vanillaXml";
        // Developer builds only: a loose XML folder beside the executable replaces
        // the packaged archive so testers can edit gameplay data between launches.
        public const string EditableDirectoryName = "GameplayXml";
        public const string EditableMarkerFileName = "eclipse-editable-xml.txt";
        private const string Magic = "SF2XML1";
        private const int MaxFiles = 10000;
        private const int MaxFileBytes = 64 * 1024 * 1024;
        private const long MaxTotalBytes = 512L * 1024 * 1024;

        private static string _editableRoot;
        private static bool _editableRootInit;
        private static string _packagedRoot;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetRootCache()
        {
            _editableRoot = _packagedRoot = null;
            _editableRootInit = false;
        }

        public static string GetXmlRoot()
        {
            if (Application.isEditor)
                return Path.Combine(Application.dataPath, EditorSourceDirectoryName);

            string editable = GetEditableRoot();
            if (editable != null)
                return editable;

            // The packaged resource is immutable for this player run. Resolving its
            // directory again must not reload its bytes and hash the entire archive.
            // Loose/editor XML still gets read by callers; only the directory is cached.
            if (_packagedRoot != null)
                return _packagedRoot;

            TextAsset asset = Resources.Load<TextAsset>(ResourcePath);
            if (asset == null)
                throw new InvalidDataException("Packaged gameplay XML is missing. Rebuild using SF2's content build processor.");
            string root;
            try
            {
                root = ExtractArchive(asset.bytes, Path.Combine(Application.persistentDataPath, "Content/gameplay"));
            }
            finally
            {
                Resources.UnloadAsset(asset);
            }
            _packagedRoot = root;
            return root;
        }

        // Only honoured on desktop players, and only when the build step wrote the
        // marker, so a stray folder never silently replaces shipped content.
        private static string GetEditableRoot()
        {
            if (_editableRootInit)
                return _editableRoot;
            _editableRootInit = true;
            if (Application.platform != RuntimePlatform.WindowsPlayer &&
                Application.platform != RuntimePlatform.LinuxPlayer)
                return null;
            DirectoryInfo gameDirectory = Directory.GetParent(Application.dataPath);
            if (gameDirectory == null)
                return null;
            string root = Path.Combine(gameDirectory.FullName, EditableDirectoryName);
            if (!File.Exists(Path.Combine(root, EditableMarkerFileName)))
                return null;
            if (!File.Exists(Path.Combine(root, "stages.xml")))
            {
                Debug.LogWarning("[OfflineContent] Editable XML folder " + root +
                    " has no stages.xml; using packaged gameplay XML.");
                return null;
            }
            Debug.LogWarning("[OfflineContent] Using editable gameplay XML from " + root);
            _editableRoot = root;
            return root;
        }

        // Absolute paths of the files that make up packaged gameplay XML.
        public static string[] GetSourceFiles(string root)
        {
            return Directory.GetFiles(root, "*", SearchOption.AllDirectories)
                .Where(path => !path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                .Where(path => !path.Substring(root.Length).Replace('\\', '/').StartsWith("models/", StringComparison.OrdinalIgnoreCase))
                .OrderBy(path => path, StringComparer.Ordinal).ToArray();
        }

        public static string NormalizeSourceRoot(string sourceDirectory)
        {
            return Path.GetFullPath(sourceDirectory).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
        }

        public static byte[] CreateArchive(string sourceDirectory)
        {
            string root = NormalizeSourceRoot(sourceDirectory);
            string[] files = GetSourceFiles(root);
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
