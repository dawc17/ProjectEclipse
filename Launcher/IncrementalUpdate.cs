using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Eclipse.Launcher
{
    public sealed class PackageFile
    {
        public string path;
        public long size;
        public string sha256;
        public string[] chunks;
    }

    public static class IncrementalUpdate
    {
        public const int BlockSize = 16 * 1024 * 1024;
        public static bool IsHash(string value)
        {
            return value != null && Regex.IsMatch(value, "^[a-f0-9]{64}$");
        }
        public static string Hash(Stream stream)
        {
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
        }
        public static string Hash(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes, false)) return Hash(stream);
        }
        public static void ValidatePath(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length > 220 || name.Contains("\\"))
                throw new InvalidDataException("Invalid package path.");
            string[] pieces = name.Split('/');
            foreach (string piece in pieces)
                if (piece.Length == 0 || piece == "." || piece == ".." || piece.EndsWith(".") ||
                    piece.EndsWith(" ") || piece.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
                    Regex.IsMatch(piece, @"^(CON|PRN|AUX|NUL|COM[1-9]|LPT[1-9])(\.|$)", RegexOptions.IgnoreCase))
                    throw new InvalidDataException("Unsafe package path.");
            if (Regex.IsMatch(pieces[0], @"^(Mods|versions|staging|launcher|download-cache|launcher-state\..*|launcher\.lock)$", RegexOptions.IgnoreCase))
                throw new InvalidDataException("Reserved package path.");
        }
        public static void Validate(Manifest manifest)
        {
            UpdateCore.ValidateVersion(manifest.version);
            if (manifest.format != 2 || manifest.files == null || manifest.files.Length == 0 ||
                manifest.files.Length > 100000 || manifest.parts == null || manifest.parts.Length > 100000 ||
                manifest.unpackedSize <= 0 || manifest.unpackedSize > 200L * 1024 * 1024 * 1024)
                throw new InvalidDataException("Invalid incremental package dimensions.");
            var objects = new Dictionary<string, DownloadPart>(StringComparer.Ordinal);
            foreach (var part in manifest.parts)
            {
                if (part == null || !IsHash(part.sha256) || !IsHash(part.contentSha256) ||
                    part.size <= 0 || part.size > BlockSize + 65536 || part.unpackedSize <= 0 ||
                    part.unpackedSize > BlockSize || !UpdateCore.IsReleaseUrl(part.url) ||
                    objects.ContainsKey(part.contentSha256))
                    throw new InvalidDataException("Invalid incremental object.");
                objects.Add(part.contentSha256, part);
            }
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var used = new HashSet<string>();
            long total = 0;
            foreach (var file in manifest.files)
            {
                if (file == null) throw new InvalidDataException("Missing file.");
                ValidatePath(file.path);
                if (!names.Add(file.path) || file.size < 0 || !IsHash(file.sha256) || file.chunks == null)
                    throw new InvalidDataException("Invalid package file.");
                long size = 0;
                for (int i = 0; i < file.chunks.Length; i++)
                {
                    DownloadPart part;
                    if (file.chunks[i] == null || !objects.TryGetValue(file.chunks[i], out part) ||
                        (i < file.chunks.Length - 1 && part.unpackedSize != BlockSize))
                        throw new InvalidDataException("Invalid file block.");
                    size = checked(size + part.unpackedSize);
                    used.Add(file.chunks[i]);
                }
                if (size != file.size) throw new InvalidDataException("File size mismatch.");
                total = checked(total + size);
            }
            foreach (var name in names)
            {
                int slash = name.LastIndexOf('/');
                while (slash > 0)
                {
                    if (names.Contains(name.Substring(0, slash))) throw new InvalidDataException("File/directory collision.");
                    slash = name.LastIndexOf('/', slash - 1);
                }
            }
            if (total != manifest.unpackedSize || used.Count != objects.Count ||
                !names.Contains("Eclipse.exe") || !names.Contains("UnityPlayer.dll") || !names.Contains("EclipseLauncher.exe"))
                throw new InvalidDataException("Incomplete game package.");
            bool data = false;
            foreach (string name in names) data |= name.StartsWith("Eclipse_Data/", StringComparison.OrdinalIgnoreCase);
            if (!data) throw new InvalidDataException("Missing game data.");
        }

        // Refuse local junctions/symlinks before reading existing files or writing caches.
        public static void CheckLocalPath(string root, string path)
        {
            string boundary = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar);
            string full = Path.GetFullPath(path);
            if (!full.StartsWith(boundary + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Path outside installation.");
            for (string current = full; current != null; current = Path.GetDirectoryName(current))
            {
                if ((File.Exists(current) || Directory.Exists(current)) &&
                    (File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0)
                    throw new InvalidDataException("Installation paths must not contain links.");
            }
        }

        // A server may ignore Range. In that case discard the prefix and consume its full response.
        public static long ResponseOffset(int status, string contentRange, long offset, long expected)
        {
            if (status == 200) return 0;
            if (status != 206 || contentRange != "bytes " + offset + "-" + (expected - 1) + "/" + expected)
                throw new InvalidDataException("Unexpected download range.");
            return offset;
        }
        public static void Receive(Stream input, string path, long offset, long expected)
        {
            using (var output = new FileStream(path, offset == 0 ? FileMode.Create : FileMode.Open, FileAccess.Write))
            {
                if (offset > 0 && output.Length != offset) throw new InvalidDataException("Resume file changed.");
                output.Position = offset;
                var buffer = new byte[65536];
                long total = offset;
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) != 0)
                {
                    total += read;
                    if (total > expected) throw new InvalidDataException("Download exceeds declared size.");
                    output.Write(buffer, 0, read);
                }
                if (total != expected) throw new IOException("Incomplete download; retry to resume.");
            }
        }
        public static void Download(DownloadPart part, string path)
        {
            long offset = File.Exists(path) ? new FileInfo(path).Length : 0;
            if (offset >= part.size) { File.Delete(path); offset = 0; }
            var request = (HttpWebRequest)WebRequest.Create(part.url);
            request.UserAgent = "EclipseLauncher/2";
            request.Timeout = request.ReadWriteTimeout = 30000;
            if (offset > 0) request.AddRange(offset);
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.ResponseUri.Scheme != "https") throw new InvalidDataException("Insecure download redirect.");
                offset = ResponseOffset((int)response.StatusCode, response.Headers["Content-Range"], offset, part.size);
                if (response.ContentLength >= 0 && response.ContentLength != part.size - offset)
                    throw new InvalidDataException("Unexpected download length.");
                using (var input = response.GetResponseStream()) Receive(input, path, offset, part.size);
            }
        }

        public static byte[] Decode(string path, DownloadPart part)
        {
            byte[] bytes = new byte[(int)part.unpackedSize];
            using (var input = File.OpenRead(path))
            using (var gzip = new GZipStream(input, CompressionMode.Decompress))
            {
                int position = 0, read;
                while (position < bytes.Length && (read = gzip.Read(bytes, position, bytes.Length - position)) != 0) position += read;
                if (position != bytes.Length || gzip.ReadByte() != -1 || Hash(bytes) != part.contentSha256)
                    throw new InvalidDataException("Invalid uncompressed block.");
            }
            return bytes;
        }

        public static void Install(string root, string source, string destination, Manifest manifest,
            Action<DownloadPart, string> fetch, Action<long> progress)
        {
            Validate(manifest);
            CheckLocalPath(root, destination);
            if (Directory.Exists(destination)) throw new IOException("Staging directory already exists.");
            string cache = Path.Combine(root, "download-cache");
            CheckLocalPath(root, cache);
            Directory.CreateDirectory(cache);
            Directory.CreateDirectory(destination);
            var objects = new Dictionary<string, DownloadPart>();
            var locks = new Dictionary<string, object>();
            foreach (var part in manifest.parts) { objects.Add(part.contentSha256, part); locks.Add(part.contentSha256, new object()); }
            long completed = 0;
            object progressLock = new object();
            Parallel.ForEach(manifest.files, new ParallelOptions { MaxDegreeOfParallelism = 3 }, file =>
            {
                string target = Path.Combine(destination, file.path);
                CheckLocalPath(root, target);
                Directory.CreateDirectory(Path.GetDirectoryName(target));
                string existing = Path.Combine(source, file.path);
                CheckLocalPath(root, existing);
                using (var old = File.Exists(existing) ? File.OpenRead(existing) : null)
                using (var output = File.Create(target))
                {
                    foreach (string id in file.chunks)
                    {
                        var part = objects[id];
                        byte[] bytes = null;
                        if (old != null && old.Length >= output.Position + part.unpackedSize)
                        {
                            old.Position = output.Position;
                            bytes = new byte[(int)part.unpackedSize];
                            int position = 0, read;
                            while (position < bytes.Length && (read = old.Read(bytes, position, bytes.Length - position)) != 0) position += read;
                            if (position != bytes.Length || Hash(bytes) != id) bytes = null;
                        }
                        if (bytes == null)
                        {
                            lock (locks[id])
                            {
                                string cached = Path.Combine(cache, part.sha256 + ".gz");
                                string partial = cached + ".partial";
                                CheckLocalPath(root, cached); CheckLocalPath(root, partial);
                                if (File.Exists(cached))
                                {
                                    try { UpdateCore.VerifyPart(cached, part); }
                                    catch (InvalidDataException) { File.Delete(cached); }
                                }
                                if (!File.Exists(cached))
                                {
                                    bool complete = false;
                                    if (File.Exists(partial) && new FileInfo(partial).Length == part.size)
                                    {
                                        try { UpdateCore.VerifyPart(partial, part); complete = true; }
                                        catch (InvalidDataException) { File.Delete(partial); }
                                    }
                                    if (!complete) fetch(part, partial);
                                    try { UpdateCore.VerifyPart(partial, part); }
                                    catch (InvalidDataException) { File.Delete(partial); throw; }
                                    File.Move(partial, cached);
                                }
                                bytes = Decode(cached, part);
                            }
                        }
                        output.Write(bytes, 0, bytes.Length);
                        lock (progressLock) { completed += bytes.Length; if (progress != null) progress(completed); }
                    }
                }
                using (var input = File.OpenRead(target))
                    if (Hash(input) != file.sha256) throw new InvalidDataException("Installed file checksum mismatch.");
            });
        }
    }
}
