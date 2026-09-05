using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using Eclipse.Launcher;

internal static class UpdateTests
{
    private static int count;
    private static void Assert(bool condition, string label)
    {
        if (!condition) throw new Exception(label);
        count++; Console.WriteLine("PASS " + label);
    }
    private static void Reject(Action action, string label)
    {
        try { action(); } catch (InvalidDataException) { Assert(true, label); return; }
        throw new Exception("Accepted " + label);
    }
    private static void Main(string[] args)
    {
        string root = Path.GetFullPath(args[0]);
        Directory.CreateDirectory(root);
        Assert(UpdateCore.IsNewer("1.2.0", "1.1.9"), "version comparison");
        Assert(!UpdateCore.IsNewer("1.1.9", "1.2.0"), "no downgrade");
        Reject(() => UpdateCore.ValidateVersion("../escape"), "version path traversal");
        var state = new InstallState();
        UpdateCore.SaveState(root, state);
        state.current = "1.0.0";
        UpdateCore.SaveState(root, state);
        Assert(UpdateCore.ReadState(root).current == "1.0.0", "atomic state replacement");
        Assert(File.Exists(Path.Combine(root, "launcher-state.json.bak")), "state backup");
        string part = Path.Combine(root, "part");
        File.WriteAllText(part, "test bytes");
        string hash;
        using (var sha = SHA256.Create()) hash = BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(part))).Replace("-", "");
        var descriptor = new DownloadPart { size = new FileInfo(part).Length, sha256 = hash };
        UpdateCore.VerifyPart(part, descriptor); Assert(true, "valid download");
        File.WriteAllText(part, "evil bytes");
        Reject(() => UpdateCore.VerifyPart(part, descriptor), "checksum mismatch");
        descriptor.size++;
        Reject(() => UpdateCore.VerifyPart(part, descriptor), "truncated download");
        foreach (string name in new[] { "../escape", "C:/escape", "Mods/test", "NUL.txt", "test.", "a\\b" })
        {
            string bad = Path.Combine(root, Guid.NewGuid().ToString("N") + ".zip");
            using (var zip = ZipFile.Open(bad, ZipArchiveMode.Create)) zip.CreateEntry(name);
            Reject(() => UpdateCore.Extract(bad, Path.Combine(root, Guid.NewGuid().ToString("N")), 1), "reject " + name);
        }
        string valid = Path.Combine(root, "valid.zip");
        using (var zip = ZipFile.Open(valid, ZipArchiveMode.Create))
        {
            foreach (string name in new[] { "Eclipse.exe", "UnityPlayer.dll", "EclipseLauncher.exe", "Eclipse_Data/data" })
                using (var output = zip.CreateEntry(name).Open()) output.WriteByte(42);
        }
        string staged = Path.Combine(root, "staged");
        UpdateCore.Extract(valid, staged, 4);
        Assert(File.Exists(Path.Combine(staged, "Eclipse.exe")), "valid Unity layout");
        UpdateCore.Activate(root, state, staged, "1.1.0");
        Assert(UpdateCore.ReadState(root).previous == "1.0.0" && state.current == "1.1.0", "activate retains rollback version");
        Assert(File.Exists(Path.Combine(root, "versions", "1.1.0", "Eclipse.exe")), "staged version moved");
        if (args.Length > 1)
        {
            string package = Path.GetFullPath(args[1]);
            var manifest = UpdateCore.Json.Deserialize<Manifest>(File.ReadAllText(Path.Combine(package, "stable.json")));
            UpdateCore.ValidateManifest(manifest);
            string joined = Path.Combine(root, "joined.zip");
            using (var output = File.Create(joined))
                foreach (var item in manifest.parts)
                {
                    string path = Path.Combine(package, Path.GetFileName(new Uri(item.url).AbsolutePath));
                    UpdateCore.VerifyPart(path, item);
                    using (var input = File.OpenRead(path)) input.CopyTo(output);
                }
            UpdateCore.Extract(joined, Path.Combine(root, "packaged"), manifest.unpackedSize);
            Assert(true, "packaging manifest, part hashes, reassembly and extraction");
        }
        Console.WriteLine(count + " tests passed; fixtures retained at " + root);
    }
}
