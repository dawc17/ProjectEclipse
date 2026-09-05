using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace Eclipse.Launcher
{
    public sealed class DownloadPart
    {
        public string url;
        public string sha256;
        public long size;
    }
    public sealed class Manifest
    {
        public int format;
        public string version;
        public string notes;
        public long unpackedSize;
        public DownloadPart[] parts;
    }
    public sealed class InstallState
    {
        public string current;
        public string previous;
        public string rejected;
        public string channel = "stable";
        public bool autoCheck = true;
    }
    public static class UpdateCore
    {
        public const string Repository = "https://github.com/dawc17/ProjectEclipse";
        public static readonly JavaScriptSerializer Json = new JavaScriptSerializer { MaxJsonLength = 1024 * 1024 };

        public static void ValidateVersion(string value)
        {
            if (value == null || !Regex.IsMatch(value, @"^\d{1,6}\.\d{1,6}\.\d{1,6}$"))
                throw new InvalidDataException("Version must be major.minor.patch.");
        }
        public static bool IsNewer(string candidate, string current)
        {
            ValidateVersion(candidate);
            if (string.IsNullOrEmpty(current)) return true;
            ValidateVersion(current);
            return new Version(candidate) > new Version(current);
        }
        public static InstallState ReadState(string root)
        {
            string path = Path.Combine(root, "launcher-state.json");
            var state = File.Exists(path) ? Json.Deserialize<InstallState>(File.ReadAllText(path)) : new InstallState();
            if (state == null) throw new InvalidDataException("Invalid launcher state.");
            foreach (string version in new[] { state.current, state.previous, state.rejected })
                if (!string.IsNullOrEmpty(version)) ValidateVersion(version);
            if (state.channel != "stable" && state.channel != "beta") throw new InvalidDataException("Invalid channel.");
            return state;
        }
        public static void SaveState(string root, InstallState state)
        {
            string path = Path.Combine(root, "launcher-state.json");
            string temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            File.WriteAllText(temporary, Json.Serialize(state));
            if (File.Exists(path)) File.Replace(temporary, path, path + ".bak");
            else File.Move(temporary, path);
        }
        public static string VersionDirectory(string root, string version)
        {
            ValidateVersion(version);
            return Path.Combine(root, "versions", version);
        }
        public static string GameDirectory(string root, InstallState state)
        {
            return string.IsNullOrEmpty(state.current) ? root : VersionDirectory(root, state.current);
        }
        public static void ValidateManifest(Manifest manifest)
        {
            if (manifest == null || manifest.format != 1) throw new InvalidDataException("Unsupported update format.");
            ValidateVersion(manifest.version);
            if (manifest.parts == null || manifest.parts.Length == 0 || manifest.parts.Length > 128 ||
                manifest.unpackedSize <= 0 || manifest.unpackedSize > 200L * 1024 * 1024 * 1024)
                throw new InvalidDataException("Invalid package dimensions.");
            foreach (var part in manifest.parts)
            {
                if (part == null || part.size <= 0 || part.size > 2000000000L ||
                    part.sha256 == null || !Regex.IsMatch(part.sha256, "^[a-fA-F0-9]{64}$") ||
                    part.url == null || !part.url.StartsWith(Repository + "/releases/download/", StringComparison.Ordinal))
                    throw new InvalidDataException("Invalid release asset.");
            }
        }
        public static void VerifyPart(string path, DownloadPart part)
        {
            if (new FileInfo(path).Length != part.size) throw new InvalidDataException("Incomplete download.");
            using (var stream = File.OpenRead(path))
            using (var sha = SHA256.Create())
            {
                string actual = BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "");
                if (!actual.Equals(part.sha256, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("Download checksum mismatch.");
            }
        }
        public static void Download(string url, string path, long maximumBytes, Action<long> progress)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "EclipseLauncher/1";
            request.Timeout = 30000;
            request.ReadWriteTimeout = 30000;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.ResponseUri.Scheme != "https" || response.ContentLength > maximumBytes)
                    throw new InvalidDataException("Unexpected update response.");
                using (var input = response.GetResponseStream())
                using (var output = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                {
                    var buffer = new byte[65536];
                    long total = 0;
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        total += read;
                        if (total > maximumBytes) throw new InvalidDataException("Download exceeds declared size.");
                        output.Write(buffer, 0, read);
                        if (progress != null) progress(total);
                    }
                }
            }
        }
        public static void Extract(string archive, string destination, long expectedSize)
        {
            string root = Path.GetFullPath(destination) + Path.DirectorySeparatorChar;
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var zip = ZipFile.OpenRead(archive))
            {
                long total = 0;
                if (zip.Entries.Count > 200000) throw new InvalidDataException("Too many package entries.");
                foreach (var entry in zip.Entries)
                {
                    string name = entry.FullName;
                    string[] pieces = name.TrimEnd('/').Split('/');
                    if (name.Contains("\\") || name.StartsWith("/") || !names.Add(name))
                        throw new InvalidDataException("Unsafe or duplicate archive path.");
                    foreach (string piece in pieces)
                        if (piece.Length == 0 || piece == "." || piece == ".." || piece.EndsWith(".") || piece.EndsWith(" ") ||
                            piece.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
                            Regex.IsMatch(piece, @"^(CON|PRN|AUX|NUL|COM[1-9]|LPT[1-9])(\.|$)", RegexOptions.IgnoreCase))
                            throw new InvalidDataException("Unsafe archive path.");
                    if (pieces[0].Equals("Mods", StringComparison.OrdinalIgnoreCase))
                        throw new InvalidDataException("Release packages must not contain user mods.");
                    if (((entry.ExternalAttributes >> 16) & 0xF000) == 0xA000)
                        throw new InvalidDataException("Archive links are not supported.");
                    total = checked(total + entry.Length);
                    if (total > expectedSize) throw new InvalidDataException("Package exceeds declared size.");
                }
                if (total != expectedSize) throw new InvalidDataException("Package size mismatch.");
                Directory.CreateDirectory(destination);
                foreach (var entry in zip.Entries)
                {
                    string path = Path.GetFullPath(Path.Combine(destination, entry.FullName));
                    if (!path.StartsWith(root, StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("Unsafe path.");
                    if (entry.FullName.EndsWith("/")) Directory.CreateDirectory(path);
                    else { Directory.CreateDirectory(Path.GetDirectoryName(path)); entry.ExtractToFile(path, false); }
                }
            }
            foreach (string file in new[] { "Eclipse.exe", "UnityPlayer.dll", "EclipseLauncher.exe" })
                if (!File.Exists(Path.Combine(destination, file))) throw new InvalidDataException("Missing " + file);
            if (!Directory.Exists(Path.Combine(destination, "Eclipse_Data"))) throw new InvalidDataException("Missing game data.");
        }
        public static void Activate(string root, InstallState state, string staged, string version)
        {
            string destination = VersionDirectory(root, version);
            if (Directory.Exists(destination)) throw new IOException("This version already exists. Use rollback or a newer release.");
            Directory.CreateDirectory(Path.GetDirectoryName(destination));
            Directory.Move(staged, destination);
            var next = Json.Deserialize<InstallState>(Json.Serialize(state));
            next.previous = state.current;
            next.current = version;
            SaveState(root, next);
            state.previous = next.previous;
            state.current = next.current;
        }
    }
}
