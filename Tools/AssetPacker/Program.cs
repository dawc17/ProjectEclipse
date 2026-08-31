using System.Formats.Tar;
using System.Security.Cryptography;
using System.Text;
using K4os.Compression.LZ4.Streams;

namespace Eclipse.AssetPacker;

internal static class Program
{
    private static readonly StringComparer PathComparer = StringComparer.OrdinalIgnoreCase;

    private static int Main(string[] args)
    {
        try
        {
            if (args.Length < 2)
                return Usage();

            string command = args[0].ToLowerInvariant();
            switch (command)
            {
                case "pack" when args.Length == 3:
                    Pack(args[1], args[2]);
                    return 0;
                case "list" when args.Length == 2:
                    List(args[1]);
                    return 0;
                case "verify" when args.Length == 2:
                    VerifyArchive(args[1], verbose: true);
                    return 0;
                case "extract" when args.Length == 3:
                    Extract(args[1], args[2]);
                    return 0;
                case "info" when args.Length == 2:
                    Info(args[1]);
                    return 0;
                case "compress" when args.Length == 3:
                    CompressTar(args[1], args[2]);
                    return 0;
                default:
                    return Usage();
            }
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine("ERROR: " + exception.Message);
            return 1;
        }
    }

    private static int Usage()
    {
        Console.Error.WriteLine("SF2DE TAR/LZ4 asset tool");
        Console.Error.WriteLine("  pack <directory> <bundle.tar.lz4>");
        Console.Error.WriteLine("  list <bundle.tar.lz4>");
        Console.Error.WriteLine("  verify <bundle.tar.lz4>");
        Console.Error.WriteLine("  extract <bundle.tar.lz4> <directory>");
        Console.Error.WriteLine("  info <bundle.tar.lz4>");
        Console.Error.WriteLine("  compress <bundle.tar> <bundle.tar.lz4>");
        return 2;
    }

    private static void Pack(string sourceDirectory, string outputPath)
    {
        string source = Path.GetFullPath(sourceDirectory);
        if (!Directory.Exists(source))
            throw new DirectoryNotFoundException(source);
        if (!outputPath.EndsWith(".tar.lz4", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Bundle filename must end in .tar.lz4");

        DirectoryBundle bundle = ScanDirectory(source);
        ValidateBundle(bundle);

        string output = Path.GetFullPath(outputPath);
        Directory.CreateDirectory(Path.GetDirectoryName(output)!);
        string temporary = output + ".tmp";
        File.Delete(temporary);
        try
        {
            using FileStream file = File.Create(temporary);
            using Stream compressed = LZ4Stream.Encode(file, leaveOpen: false);
            using var writer = new System.Formats.Tar.TarWriter(compressed, TarEntryFormat.Ustar, leaveOpen: false);
            foreach (BundleFile item in bundle.Files.OrderBy(x => x.Path, StringComparer.Ordinal))
            {
                var entry = new UstarTarEntry(TarEntryType.RegularFile, item.Path)
                {
                    DataStream = File.OpenRead(item.FullPath),
                    ModificationTime = DateTimeOffset.UnixEpoch,
                    Uid = 0,
                    Gid = 0,
                    Mode = UnixFileMode.UserRead | UnixFileMode.UserWrite |
                        UnixFileMode.GroupRead | UnixFileMode.OtherRead,
                    UserName = string.Empty,
                    GroupName = string.Empty,
                };
                try { writer.WriteEntry(entry); }
                finally { entry.DataStream.Dispose(); }
            }
            writer.Dispose();
            compressed.Dispose();
            file.Dispose();
            File.Move(temporary, output, true);
        }
        catch
        {
            File.Delete(temporary);
            throw;
        }

        VerifyArchive(output, verbose: false);
        Console.WriteLine($"Packed {bundle.Files.Count} files, {bundle.Metas.Count} assets -> {output}");
        PrintHash(output);
    }

    private static void List(string archivePath)
    {
        ArchiveBundle bundle = ReadArchive(archivePath, capturePayload: false);
        foreach (ArchiveEntryInfo entry in bundle.Entries)
        {
            string marker = entry.Path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase) ? "asset" : "file ";
            Console.WriteLine($"{marker} {entry.Size,10}  {entry.Path}");
        }
        Console.WriteLine($"{bundle.Entries.Count} files, {bundle.Metas.Count} asset descriptors");
    }

    private static void VerifyArchive(string archivePath, bool verbose)
    {
        ArchiveBundle bundle = ReadArchive(archivePath, capturePayload: true);
        ValidateBundle(bundle);
        if (verbose)
        {
            Console.WriteLine($"OK: {bundle.Entries.Count} files, {bundle.Metas.Count} assets");
            PrintHash(Path.GetFullPath(archivePath));
        }
    }

    private static void Extract(string archivePath, string outputDirectory)
    {
        ArchiveBundle bundle = ReadArchive(archivePath, capturePayload: true);
        ValidateBundle(bundle);
        string root = Path.GetFullPath(outputDirectory);
        Directory.CreateDirectory(root);
        foreach (ArchiveEntryInfo entry in bundle.Entries)
        {
            string destination = SafeDestination(root, entry.Path);
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            File.WriteAllBytes(destination, entry.Payload ?? throw new InvalidDataException("Missing captured payload."));
        }
        Console.WriteLine($"Extracted {bundle.Entries.Count} files -> {root}");
    }

    private static void Info(string archivePath)
    {
        string path = Path.GetFullPath(archivePath);
        if (!File.Exists(path)) throw new FileNotFoundException("Bundle not found.", path);
        FileInfo fileInfo = new(path);
        Console.WriteLine("size=" + fileInfo.Length);
        Console.WriteLine("unpackedSize=" + MeasureDecodedLength(path));
        Console.WriteLine("sha256=" + Hash(path));
    }

    private static long MeasureDecodedLength(string archivePath)
    {
        using FileStream file = File.OpenRead(archivePath);
        using Stream decoded = LZ4Stream.Decode(file, leaveOpen: false);
        byte[] buffer = new byte[1024 * 1024];
        long total = 0;
        int read;
        while ((read = decoded.Read(buffer, 0, buffer.Length)) > 0)
            total += read;
        return total;
    }

    private static void CompressTar(string tarPath, string outputPath)
    {
        string source = Path.GetFullPath(tarPath);
        if (!File.Exists(source)) throw new FileNotFoundException("TAR not found.", source);
        if (!outputPath.EndsWith(".tar.lz4", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Bundle filename must end in .tar.lz4");
        string output = Path.GetFullPath(outputPath);
        Directory.CreateDirectory(Path.GetDirectoryName(output)!);
        string temporary = output + ".tmp";
        File.Delete(temporary);
        try
        {
            using FileStream input = File.OpenRead(source);
            using FileStream file = File.Create(temporary);
            using Stream compressed = LZ4Stream.Encode(file, leaveOpen: false);
            input.CopyTo(compressed);
            compressed.Dispose();
            file.Dispose();
            File.Move(temporary, output, true);
        }
        catch
        {
            File.Delete(temporary);
            throw;
        }
        VerifyArchive(output, verbose: false);
        Console.WriteLine($"Compressed {source} -> {output}");
        PrintHash(output);
    }

    private static DirectoryBundle ScanDirectory(string root)
    {
        var files = new List<BundleFile>();
        foreach (string fullPath in Directory.GetFiles(root, "*", SearchOption.AllDirectories))
        {
            string relative = NormalizePath(Path.GetRelativePath(root, fullPath));
            files.Add(new BundleFile(relative, fullPath));
        }
        var bundle = new DirectoryBundle(files);
        foreach (BundleFile file in files.Where(x => x.Path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase)))
            bundle.Metas.Add(ParseMeta(file.Path, File.ReadAllText(file.FullPath, Encoding.UTF8)));
        return bundle;
    }

    private static ArchiveBundle ReadArchive(string archivePath, bool capturePayload)
    {
        string path = Path.GetFullPath(archivePath);
        if (!File.Exists(path)) throw new FileNotFoundException("Bundle not found.", path);
        var bundle = new ArchiveBundle();
        var seen = new HashSet<string>(PathComparer);
        using FileStream file = File.OpenRead(path);
        using Stream decoded = LZ4Stream.Decode(file, leaveOpen: false);
        using var reader = new TarReader(decoded, leaveOpen: false);
        TarEntry? entry;
        while ((entry = reader.GetNextEntry()) != null)
        {
            if (entry.EntryType == TarEntryType.Directory) continue;
            if (entry.EntryType != TarEntryType.RegularFile && entry.EntryType != TarEntryType.V7RegularFile)
                throw new InvalidDataException($"Unsupported TAR entry type {entry.EntryType}: {entry.Name}");
            string name = NormalizePath(entry.Name);
            if (!seen.Add(name)) throw new InvalidDataException("Duplicate/case-colliding TAR path: " + name);
            if (entry.DataStream == null) throw new InvalidDataException("Regular TAR entry has no data: " + name);
            using var memory = new MemoryStream();
            entry.DataStream.CopyTo(memory);
            byte[] bytes = memory.ToArray();
            bundle.Entries.Add(new ArchiveEntryInfo(name, bytes.LongLength, capturePayload ? bytes : null));
            if (name.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                bundle.Metas.Add(ParseMeta(name, Encoding.UTF8.GetString(bytes)));
        }
        return bundle;
    }

    private static void ValidateBundle(IBundleView bundle)
    {
        if (bundle.Paths.Count == 0) throw new InvalidDataException("Bundle is empty.");
        if (bundle.Metas.Count == 0) throw new InvalidDataException("Bundle contains no .meta asset descriptors.");
        var assets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (Meta meta in bundle.Metas)
        {
            if (!assets.Add(meta.Namespace + ":" + meta.Address + ":" + meta.Name + ":" + meta.Type))
                throw new InvalidDataException("Duplicate logical asset descriptor: " + meta.Path);
            foreach (string reference in meta.References)
                if (!bundle.Paths.Contains(reference))
                    throw new InvalidDataException($"{meta.Path} references missing/out-of-bundle file: {reference}");
        }
        var allowedXml = new HashSet<string>(PathComparer);
        foreach (Meta meta in bundle.Metas.Where(x => x.Type == "model"))
            foreach (string reference in meta.References)
                allowedXml.Add(reference);
        foreach (string path in bundle.Paths.Where(x => x.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)))
            if (!allowedXml.Contains(path))
                throw new InvalidDataException("Config/gameplay XML is deliberately excluded from TAR assets: " + path);
    }

    private static Meta ParseMeta(string path, string text)
    {
        Dictionary<string, string> values = ParseValues(text);
        string type = Required(values, "type").ToLowerInvariant();
        if (type is "sound" or "music") type = "audio";
        if (type is not ("sprite" or "audio" or "model" or "atlas"))
            throw new InvalidDataException($"Unsupported TAR asset type '{type}' in {path}");
        string namespaceId = Optional(values, "namespace", "core");
        if (string.IsNullOrWhiteSpace(namespaceId) || namespaceId.IndexOfAny(['/', '\\', ':']) >= 0)
            throw new InvalidDataException("Invalid namespace in " + path);
        string address = NormalizeAddress(Required(values, "address"));
        string name = Optional(values, "name", string.Empty);
        var references = new List<string>();
        if (type == "sprite")
        {
            references.Add(NormalizePath(Required(values, "texture")));
            Required(values, "rect");
            if (string.IsNullOrWhiteSpace(name)) throw new InvalidDataException("Sprite name is missing in " + path);
        }
        else
        {
            references.Add(NormalizePath(Required(values, "file")));
        }
        return new Meta(path, type, namespaceId, address, name, references);
    }

    private static Dictionary<string, string> ParseValues(string text)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        int lineNumber = 0;
        foreach (string raw in text.Replace("\r", string.Empty).Split('\n'))
        {
            lineNumber++;
            string line = raw.Trim();
            if (line.Length == 0 || line.StartsWith('#')) continue;
            int equals = line.IndexOf('=');
            if (equals <= 0) throw new InvalidDataException($"Malformed metadata line {lineNumber}: {line}");
            string key = line[..equals].Trim();
            if (!values.TryAdd(key, Unescape(line[(equals + 1)..].Trim())))
                throw new InvalidDataException("Duplicate metadata key: " + key);
        }
        return values;
    }

    private static string NormalizePath(string path)
    {
        string normalized = path.Replace('\\', '/').Trim().TrimStart('/');
        if (string.IsNullOrWhiteSpace(normalized) || normalized.Contains(':'))
            throw new InvalidDataException("Unsafe bundle path: " + path);
        foreach (string part in normalized.Split('/'))
            if (part is "" or "." or "..") throw new InvalidDataException("Unsafe bundle path: " + path);
        return normalized;
    }

    private static string NormalizeAddress(string address)
    {
        return NormalizePath(address);
    }

    private static string SafeDestination(string root, string relative)
    {
        string destination = Path.GetFullPath(Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar)));
        string prefix = root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
        if (!destination.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Archive path escapes extraction directory: " + relative);
        return destination;
    }

    private static string Required(Dictionary<string, string> values, string key)
    {
        if (!values.TryGetValue(key, out string? value) || string.IsNullOrWhiteSpace(value))
            throw new InvalidDataException("Required metadata key is missing: " + key);
        return value;
    }

    private static string Optional(Dictionary<string, string> values, string key, string fallback)
    {
        return values.TryGetValue(key, out string? value) ? value : fallback;
    }

    private static string Unescape(string value)
    {
        var output = new StringBuilder(value.Length);
        bool escaped = false;
        foreach (char c in value)
        {
            if (!escaped)
            {
                if (c == '\\') escaped = true;
                else output.Append(c);
                continue;
            }
            output.Append(c == 'n' ? '\n' : c == 'r' ? '\r' : c);
            escaped = false;
        }
        if (escaped) output.Append('\\');
        return output.ToString();
    }

    private static void PrintHash(string file)
    {
        using FileStream input = File.OpenRead(file);
        string sha = Convert.ToHexString(SHA256.HashData(input)).ToLowerInvariant();
        Console.WriteLine($"size={input.Length} sha256={sha}");
    }

    private static string Hash(string file)
    {
        using FileStream input = File.OpenRead(file);
        return Convert.ToHexString(SHA256.HashData(input)).ToLowerInvariant();
    }

    private interface IBundleView
    {
        HashSet<string> Paths { get; }
        List<Meta> Metas { get; }
    }

    private sealed record BundleFile(string Path, string FullPath);
    private sealed record ArchiveEntryInfo(string Path, long Size, byte[]? Payload);
    private sealed record Meta(string Path, string Type, string Namespace, string Address, string Name, List<string> References);

    private sealed class DirectoryBundle : IBundleView
    {
        public List<BundleFile> Files { get; }
        public HashSet<string> Paths { get; }
        public List<Meta> Metas { get; } = [];
        public DirectoryBundle(List<BundleFile> files)
        {
            Files = files;
            Paths = new HashSet<string>(files.Select(x => x.Path), PathComparer);
            if (Paths.Count != files.Count) throw new InvalidDataException("Directory contains case-colliding paths.");
        }
    }

    private sealed class ArchiveBundle : IBundleView
    {
        public List<ArchiveEntryInfo> Entries { get; } = [];
        public List<Meta> Metas { get; } = [];
        public HashSet<string> Paths => new(Entries.Select(x => x.Path), PathComparer);
    }
}
