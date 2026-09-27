using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Threading;
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
        TestIncremental(Path.Combine(root, "incremental"));
        if (args.Length > 1)
        {
            string package = Path.GetFullPath(args[1]);
            string legacyPath = Path.Combine(package, "stable.json");
            if (File.Exists(legacyPath))
            {
                var manifest = UpdateCore.Json.Deserialize<Manifest>(File.ReadAllText(legacyPath));
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
            string incrementalPath = Path.Combine(package, "stable-v2.json");
            Assert(File.Exists(legacyPath) || File.Exists(incrementalPath), "package has a stable manifest");
            if (File.Exists(incrementalPath))
            {
                var incremental = UpdateCore.Json.Deserialize<Manifest>(File.ReadAllText(incrementalPath));
                string installRoot = Path.Combine(root, "packaged-incremental");
                Directory.CreateDirectory(installRoot);
                IncrementalUpdate.Install(installRoot, installRoot, Path.Combine(installRoot, "game"), incremental,
                    (item, target) => File.Copy(Path.Combine(package, Path.GetFileName(new Uri(item.url).AbsolutePath)), target), null);
                Assert(true, "self-contained incremental package hashes and reconstruction");
            }
        }
        Console.WriteLine(count + " tests passed; fixtures retained at " + root);
    }
    private static Manifest Clone(Manifest manifest)
    {
        return UpdateCore.Json.Deserialize<Manifest>(UpdateCore.Json.Serialize(manifest));
    }
    private static void TestIncremental(string root)
    {
        string game = Path.Combine(root, "loose");
        string package1 = Path.Combine(root, "package1");
        string package2 = Path.Combine(root, "package2");
        Directory.CreateDirectory(Path.Combine(game, "Eclipse_Data"));
        Directory.CreateDirectory(Path.Combine(game, "Mods"));
        File.WriteAllText(Path.Combine(game, "Mods", "private.txt"), "never ship");
        foreach (string name in new[] { "Eclipse.exe", "UnityPlayer.dll" }) File.WriteAllText(Path.Combine(game, name), name);
        File.WriteAllText(Path.Combine(game, "Eclipse_Data", "empty"), "");
        byte[] data = new byte[IncrementalUpdate.BlockSize + 4096];
        new Random(17).NextBytes(data);
        string dataPath = Path.Combine(game, "Eclipse_Data", "data");
        File.WriteAllBytes(dataPath, data);
        foreach (string package in new[] { package1, package2 })
        {
            Directory.CreateDirectory(Path.Combine(package, "launcher"));
            File.WriteAllText(Path.Combine(package, "launcher", "EclipseLauncher.exe"), "fixture launcher");
        }
        PackageBuilder.Build(game, package1, "2.0.0", "stable", "v2.0.0", "", "");
        var first = UpdateCore.Json.Deserialize<Manifest>(File.ReadAllText(Path.Combine(package1, "stable-v2.json")));
        Assert(first.format == 2 && first.files.Length == 5, "incremental packaging excludes mods and retains empty files");
        var locations = new Dictionary<string, string>();
        foreach (var part in first.parts) locations[part.contentSha256] = Path.Combine(package1, Path.GetFileName(new Uri(part.url).AbsolutePath));
        int requests = 0;
        Action<DownloadPart, string> fetch = (part, target) => {
            Interlocked.Increment(ref requests);
            File.Copy(locations[part.contentSha256], target, true);
        };
        string installed = Path.Combine(root, "installed");
        IncrementalUpdate.Install(root, game, installed, first, fetch, null);
        Assert(requests == 1, "loose install reuses all unchanged game blocks; only missing launcher downloads");
        using (var input = File.OpenRead(Path.Combine(installed, "Eclipse_Data", "data")))
            Assert(IncrementalUpdate.Hash(input) == IncrementalUpdate.Hash(data), "multi-block assembly is byte exact");
        Assert(new FileInfo(Path.Combine(installed, "Eclipse_Data", "empty")).Length == 0, "empty file installs");

        data[data.Length - 1] ^= 1;
        File.WriteAllBytes(dataPath, data);
        PackageBuilder.Build(game, package2, "2.0.1", "stable", "v2.0.1", "", Path.Combine(package1, "stable-v2.json"));
        var second = UpdateCore.Json.Deserialize<Manifest>(File.ReadAllText(Path.Combine(package2, "stable-v2.json")));
        Assert(Directory.GetFiles(package2, "*.gz").Length == 1, "one changed block produces only one new upload");
        long newBytes = 0;
        foreach (var part in second.parts)
        {
            string local = Path.Combine(package2, Path.GetFileName(new Uri(part.url).AbsolutePath));
            if (File.Exists(local)) { locations[part.contentSha256] = local; newBytes += part.size; }
        }
        Assert(newBytes < 8192, "small edit does not re-upload the 16 MiB unchanged block");
        requests = 0;
        string updated = Path.Combine(root, "updated");
        IncrementalUpdate.Install(root, installed, updated, second, fetch, null);
        Assert(requests == 1, "update downloads only changed block");
        using (var input = File.OpenRead(Path.Combine(updated, "Eclipse_Data", "data")))
            Assert(IncrementalUpdate.Hash(input) == IncrementalUpdate.Hash(data), "updated file hash");
        Assert(File.ReadAllBytes(Path.Combine(installed, "Eclipse_Data", "data"))[data.Length - 1] != data[data.Length - 1],
            "reused source remains unchanged for rollback");

        string freshRoot = Path.Combine(root, "fresh");
        Directory.CreateDirectory(freshRoot);
        requests = 0;
        IncrementalUpdate.Install(freshRoot, freshRoot, Path.Combine(freshRoot, "game"), second, fetch, null);
        Assert(requests == second.parts.Length, "fresh install resolves retained release objects");
        requests = 0;
        IncrementalUpdate.Install(freshRoot, freshRoot, Path.Combine(freshRoot, "cached"), second, fetch, null);
        Assert(requests == 0, "verified persistent cache supports offline reconstruction");
        string cacheFile = Directory.GetFiles(Path.Combine(freshRoot, "download-cache"), "*.gz")[0];
        File.Move(cacheFile, cacheFile + ".partial");
        IncrementalUpdate.Install(freshRoot, freshRoot, Path.Combine(freshRoot, "completed-partial"), second, fetch, null);
        Assert(requests == 0 && File.Exists(cacheFile), "complete verified partial promoted without refetch");
        File.WriteAllText(cacheFile, "corrupt");
        requests = 0;
        IncrementalUpdate.Install(freshRoot, freshRoot, Path.Combine(freshRoot, "repaired"), second, fetch, null);
        Assert(requests == 1, "corrupt cache block is fetched again");

        var bad = Clone(second); bad.files[0].path = "../escape";
        Reject(() => UpdateCore.ValidateManifest(bad), "incremental traversal");
        bad = Clone(second); bad.files[1].path = bad.files[0].path.ToUpperInvariant();
        Reject(() => UpdateCore.ValidateManifest(bad), "case-insensitive duplicate paths");
        bad = Clone(second); bad.files[1].path = bad.files[0].path + "/child";
        Reject(() => UpdateCore.ValidateManifest(bad), "file directory collision");
        bad = Clone(second); bad.parts[0].url = "https://github.com.evil.test/dawc17/ProjectEclipse/releases/download/v2/a.gz";
        Reject(() => UpdateCore.ValidateManifest(bad), "untrusted incremental URL");
        bad = Clone(second); bad.parts[0].unpackedSize++;
        Reject(() => UpdateCore.ValidateManifest(bad), "inconsistent block sizes");
        foreach (string path in new[] { "Mods/a", "download-cache/a", "a\\b", "/root", "CON.txt", "a/../b", "a:stream" })
            Reject(() => IncrementalUpdate.ValidatePath(path), "incremental reserved/unsafe " + path);

        string resumed = Path.Combine(root, "resumed");
        File.WriteAllBytes(resumed, new byte[] { 1, 2 });
        long offset = IncrementalUpdate.ResponseOffset(206, "bytes 2-3/4", 2, 4);
        using (var input = new MemoryStream(new byte[] { 3, 4 })) IncrementalUpdate.Receive(input, resumed, offset, 4);
        Assert(BitConverter.ToString(File.ReadAllBytes(resumed)) == "01-02-03-04", "206 appends only expected range");
        offset = IncrementalUpdate.ResponseOffset(200, null, 2, 4);
        using (var input = new MemoryStream(new byte[] { 5, 6, 7, 8 })) IncrementalUpdate.Receive(input, resumed, offset, 4);
        Assert(BitConverter.ToString(File.ReadAllBytes(resumed)) == "05-06-07-08", "ignored Range replaces rather than appends");
        Reject(() => IncrementalUpdate.ResponseOffset(206, "bytes 1-3/4", 2, 4), "wrong range rejected");
        Reject(() => { using (var input = new MemoryStream(new byte[5])) IncrementalUpdate.Receive(input, resumed, 0, 4); }, "oversized response rejected");
        bool failed = false;
        try { using (var input = new MemoryStream(new byte[] { 1, 2 })) IncrementalUpdate.Receive(input, resumed, 0, 4); }
        catch (IOException) { failed = true; }
        Assert(failed && new FileInfo(resumed).Length == 2, "interrupted transfer retains resumable prefix");

        string failureRoot = Path.Combine(root, "failure");
        Directory.CreateDirectory(failureRoot);
        var state = new InstallState { current = "2.0.0" };
        UpdateCore.SaveState(failureRoot, state);
        failed = false;
        try { IncrementalUpdate.Install(failureRoot, failureRoot, Path.Combine(failureRoot, "staged"), second,
            (part, target) => File.WriteAllText(target, "bad payload"), null); }
        catch (AggregateException ex) { failed = ex.Flatten().InnerException is InvalidDataException; }
        Assert(failed && UpdateCore.ReadState(failureRoot).current == "2.0.0", "bad network payload cannot activate or change state");
        Assert(Directory.GetFiles(Path.Combine(failureRoot, "download-cache"), "*.partial").Length == 0, "bad checksum partials discarded");
    }
}
