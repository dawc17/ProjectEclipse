using System;
using System.IO;
using System.IO.Compression;
using Eclipse.Modding;

internal static class Program
{
    private const string Manifest = "schema = 1\nid = \"example.zip\"\nname = \"ZIP test\"\n" +
        "version = \"1.0.0\"\nauthors = [\"Tester\"]\nentrypoint = \"scripts/main.lua\"\ncapabilities = []\n";

    private static int Main()
    {
        string root = Path.Combine(Path.GetTempPath(), "eclipse-mod-zip-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            string mods = Path.Combine(root, "Mods");
            string zip = Path.Combine(root, "first.zip");
            CreateZip(zip, ("mod.toml", Manifest), ("scripts/main.lua", "return 1"));
            Require(!ModZipInstaller.Inspect(zip, mods).IsUpdate, "New mod preview reported an update.");
            Require(!ModZipInstaller.Install(zip, mods, false).IsUpdate, "New mod reported an update.");
            string installed = Path.Combine(mods, "example.zip");
            Require(File.ReadAllText(Path.Combine(installed, "scripts/main.lua")) == "return 1", "Install lost Lua content.");

            string update = Path.Combine(root, "update.zip");
            CreateZip(update, ("wrapped/mod.toml", Manifest), ("wrapped/scripts/main.lua", "return 2"));
            Require(ModZipInstaller.Inspect(update, mods).IsUpdate, "Existing mod was not recognized.");
            ExpectFailure<IOException>(() => ModZipInstaller.Install(update, mods, false));
            Require(File.ReadAllText(Path.Combine(installed, "scripts/main.lua")) == "return 1", "Refused update changed files.");
            Require(ModZipInstaller.Install(update, mods, true).IsUpdate, "Replacement was not reported.");
            Require(File.ReadAllText(Path.Combine(installed, "scripts/main.lua")) == "return 2", "Replacement did not install.");

            string traversal = Path.Combine(root, "traversal.zip");
            CreateZip(traversal, ("mod.toml", Manifest), ("../escape.txt", "bad"));
            ExpectFailure<InvalidDataException>(() => ModZipInstaller.Install(traversal, mods, true));
            Require(!File.Exists(Path.Combine(root, "escape.txt")), "ZIP traversal escaped the Mods root.");
            Require(File.ReadAllText(Path.Combine(installed, "scripts/main.lua")) == "return 2", "Invalid ZIP changed installed mod.");

            string missingScript = Path.Combine(root, "missing-script.zip");
            CreateZip(missingScript, ("mod.toml", Manifest));
            ExpectFailure<InvalidDataException>(() => ModZipInstaller.Install(missingScript, mods, true));
            Require(File.ReadAllText(Path.Combine(installed, "scripts/main.lua")) == "return 2", "Invalid update changed installed mod.");

            string unsafeId = Path.Combine(root, "unsafe-id.zip");
            CreateZip(unsafeId, ("mod.toml", Manifest.Replace("example.zip", "..")),
                ("scripts/main.lua", "return 3"));
            ExpectFailure<InvalidDataException>(() => ModZipInstaller.Install(unsafeId, mods, true));
            Require(File.ReadAllText(Path.Combine(installed, "scripts/main.lua")) == "return 2", "Unsafe ID changed installed mod.");

            string outsideWrapper = Path.Combine(root, "outside-wrapper.zip");
            CreateZip(outsideWrapper, ("wrapped/mod.toml", Manifest),
                ("wrapped/scripts/main.lua", "return 3"), ("other/file.txt", "bad"));
            ExpectFailure<InvalidDataException>(() => ModZipInstaller.Inspect(outsideWrapper, mods));

            string symlink = Path.Combine(root, "symlink.zip");
            CreateZip(symlink, ("mod.toml", Manifest), ("scripts/main.lua", "return 3"));
            using (var archive = ZipFile.Open(symlink, ZipArchiveMode.Update))
                archive.CreateEntry("scripts/link.lua").ExternalAttributes = (0xA000 | 0x1FF) << 16;
            ExpectFailure<InvalidDataException>(() => ModZipInstaller.Inspect(symlink, mods));

            Console.WriteLine("ModZipInstaller PASS: root/wrapped ZIPs, replacement, refusal, unsafe paths/IDs and preservation of installed files.");
            return 0;
        }
        finally { Directory.Delete(root, true); }
    }

    private static void CreateZip(string path, params (string Name, string Content)[] files)
    {
        using var archive = ZipFile.Open(path, ZipArchiveMode.Create);
        foreach (var file in files)
        {
            using var writer = new StreamWriter(archive.CreateEntry(file.Name).Open());
            writer.Write(file.Content);
        }
    }

    private static void ExpectFailure<T>(Action action) where T : Exception
    {
        try { action(); }
        catch (T) { return; }
        throw new Exception("Expected " + typeof(T).Name + ".");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }
}

namespace Eclipse.Modding
{
    // The archive transaction test exercises the real parser and installer; Unity's asset
    // provider is compiled and validated by the managed Unity project checks.
    public sealed class LooseModProvider
    {
        public LooseModProvider(ModDescriptor mod) { }
    }
}
