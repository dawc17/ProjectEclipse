using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using Eclipse.Launcher;

// Build-time tool only; not included in the launcher executable.
internal static class PackageBuilder
{
    private static void Main(string[] args)
    {
        // game, output, version, channel, release tag, notes, previous manifest (or empty)
        Build(args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
    }
    public static void Build(string game, string output, string version, string channel,
        string tag, string notes, string previousPath)
    {
        var previous = new Dictionary<string, DownloadPart>();
        if (!string.IsNullOrEmpty(previousPath))
        {
            var prior = UpdateCore.Json.Deserialize<Manifest>(File.ReadAllText(previousPath));
            IncrementalUpdate.Validate(prior);
            foreach (var part in prior.parts) previous.Add(part.contentSha256, part);
        }
        var objects = new Dictionary<string, DownloadPart>();
        var files = new List<PackageFile>();
        var paths = new List<string>();
        Collect(game, paths);
        paths.Sort(StringComparer.Ordinal);
        paths.Add(Path.Combine(output, "launcher", "EclipseLauncher.exe"));
        long total = 0, uploaded = 0, reused = 0;
        foreach (string path in paths)
        {
            bool launcher = path == paths[paths.Count - 1];
            string relative = launcher ? "EclipseLauncher.exe" : path.Substring(game.TrimEnd('\\', '/').Length + 1).Replace('\\', '/');
            IncrementalUpdate.ValidatePath(relative);
            var chunks = new List<string>();
            long size;
            string fileHash;
            using (var input = File.OpenRead(path))
            {
                size = input.Length;
                fileHash = IncrementalUpdate.Hash(input);
                input.Position = 0;
                while (input.Position < input.Length)
                {
                    byte[] bytes = new byte[(int)Math.Min(IncrementalUpdate.BlockSize, input.Length - input.Position)];
                    int position = 0, read;
                    while (position < bytes.Length && (read = input.Read(bytes, position, bytes.Length - position)) != 0) position += read;
                    if (position != bytes.Length) throw new IOException("Build changed during packaging.");
                    string id = IncrementalUpdate.Hash(bytes);
                    chunks.Add(id);
                    if (objects.ContainsKey(id)) continue;
                    DownloadPart part;
                    if (previous.TryGetValue(id, out part))
                    {
                        if (part.unpackedSize != bytes.Length) throw new InvalidDataException("Previous block size mismatch.");
                        reused += part.size;
                    }
                    else
                    {
                        string name = id + ".gz";
                        string destination = Path.Combine(output, name);
                        using (var stream = File.Create(destination))
                        using (var gzip = new GZipStream(stream, CompressionLevel.Optimal))
                            gzip.Write(bytes, 0, bytes.Length);
                        using (var stream = File.OpenRead(destination))
                            part = new DownloadPart { url = UpdateCore.Repository + "/releases/download/" + tag + "/" + name,
                                contentSha256 = id, unpackedSize = bytes.Length, size = stream.Length, sha256 = IncrementalUpdate.Hash(stream) };
                        uploaded += part.size;
                    }
                    objects.Add(id, part);
                }
            }
            files.Add(new PackageFile { path = relative, size = size, sha256 = fileHash, chunks = chunks.ToArray() });
            total += size;
        }
        var manifest = new Manifest { format = 2, version = version, notes = notes, unpackedSize = total,
            files = files.ToArray(), parts = new List<DownloadPart>(objects.Values).ToArray() };
        IncrementalUpdate.Validate(manifest);
        string json = UpdateCore.Json.Serialize(manifest);
        if (Encoding.UTF8.GetByteCount(json) > UpdateCore.ManifestLimit) throw new InvalidDataException("Manifest exceeds launcher limit.");
        File.WriteAllText(Path.Combine(output, channel + "-v2.json"), json, new UTF8Encoding(false));
        Console.WriteLine("New upload: {0:N0} bytes; reused hosted data: {1:N0} bytes; {2} files, {3} unique blocks.",
            uploaded, reused, files.Count, objects.Count);
    }
    private static void Collect(string directory, List<string> paths)
    {
        if ((File.GetAttributes(directory) & FileAttributes.ReparsePoint) != 0) throw new IOException("Do not package links.");
        foreach (string entry in Directory.GetFileSystemEntries(directory))
        {
            string name = Path.GetFileName(entry);
            if (Regex.IsMatch(name, @"^(Mods|versions|staging|launcher|download-cache|launcher-state\..*|launcher\.lock|EclipseLauncher\.exe)$", RegexOptions.IgnoreCase) ||
                name.Contains("BackUpThisFolder_ButDontShipItWithYourGame") || name.Contains("BurstDebugInformation_DoNotShip")) continue;
            var attributes = File.GetAttributes(entry);
            if ((attributes & FileAttributes.ReparsePoint) != 0) throw new IOException("Do not package links: " + entry);
            if ((attributes & FileAttributes.Directory) != 0) Collect(entry, paths);
            else paths.Add(entry);
        }
    }
}
