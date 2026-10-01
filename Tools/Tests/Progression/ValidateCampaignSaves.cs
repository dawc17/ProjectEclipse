using System;
using System.IO;
using System.Linq;
using Eclipse.Saves;

static class Program
{
    static int checks;
    static void Check(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static void Reject<T>(Action action) where T : Exception
    {
        try { action(); } catch (T) { checks++; return; }
        throw new Exception("Expected " + typeof(T).Name);
    }
    static void Main(string[] args)
    {
        string parent = Path.GetFullPath(args[0]);
        string fixture = Path.Combine(parent, "owned-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(fixture);
        File.WriteAllText(Path.Combine(fixture, ".test-owner"), "campaign-save-test");
        try
        {
            string legacy = Path.Combine(fixture, "userdata");
            Directory.CreateDirectory(Path.Combine(legacy, "assets"));
            Directory.CreateDirectory(Path.Combine(legacy, "SaveProfiles", "Safety"));
            string original = "<Root><Warriors><Warrior ID='1' Level='12' CurrentZone='ZONE_3'/></Warriors><UnknownMod Value='preserve'/></Root>";
            File.WriteAllText(Path.Combine(legacy, "users.xml"), original);
            File.WriteAllText(Path.Combine(legacy, "users_backup.xml"), original);
            File.WriteAllText(Path.Combine(legacy, "users.xml.hash"), "hash");
            File.WriteAllText(Path.Combine(legacy, "users.xml.eclipse-write"), "pending-journal");
            File.WriteAllText(Path.Combine(legacy, "assets", "localSettings.bin"), "settings");
            File.WriteAllText(Path.Combine(legacy, "assets", "packs.xml"), "packs");
            File.WriteAllText(Path.Combine(legacy, "SaveProfiles", "Safety", "users.xml"), "safety");
            var store = new CampaignSaveStore(legacy);
            store.Initialize(); store.Initialize();
            Check(store.List().Count == 1, "Legacy import duplicated");
            Check(store.Progress("legacy").Contains("Level 12") && store.Progress("legacy").Contains("Act 3"), "Progress summary");
            string imported = Path.Combine(store.Root, "legacy", "userdata");
            Check(File.ReadAllText(Path.Combine(imported, "users.xml")) == original, "Imported XML changed");
            Check(File.ReadAllText(Path.Combine(imported, "users.xml.hash")) == "hash", "Missing hash");
            Check(File.ReadAllText(Path.Combine(imported, "users.xml.eclipse-write")) == "pending-journal", "Missing journal");
            Check(File.ReadAllText(Path.Combine(imported, "assets", "localSettings.bin")) == "settings", "Missing profile companion");
            Check(!Directory.Exists(Path.Combine(imported, "SaveProfiles")), "Developer archives copied into campaign");
            Check(File.ReadAllText(Path.Combine(legacy, "users.xml")) == original, "Original save changed");

            // A crash between publishing the imported folder and publishing its
            // marker must not overwrite subsequent progress or duplicate it.
            File.Delete(Path.Combine(store.Root, ".legacy-imported"));
            File.WriteAllText(Path.Combine(imported, "users.xml"), original + "<!--newer-->");
            store.Initialize();
            Check(File.ReadAllText(Path.Combine(imported, "users.xml")).EndsWith("<!--newer-->"), "Retried import overwrote progress");

            var a = store.Create("First journey");
            var b = store.Create("First journey");
            Check(a.Id != b.Id, "Names are being used as identity");
            Check(store.Progress(a.Id) == "New campaign", "Fresh campaign copied existing progress");
            store.Rename(a.Id, "../A <new> & named journey");
            Check(store.List().Single(x => x.Id == a.Id).Name == "../A <new> & named journey", "Name XML escaping or display/path separation failed");
            Reject<ArgumentException>(() => store.Create("  "));
            Reject<ArgumentException>(() => store.Create(new string('x', 49)));
            Reject<ArgumentException>(() => store.Delete("../userdata"));
            Reject<ArgumentException>(() => store.Open("C:\\outside"));

            CampaignSaveSession.Select(store, a.Id);
            string active = CampaignSaveSession.UserDataDirectory;
            Check(active == Path.Combine(store.Root, a.Id, "userdata"), "Selected path");
            File.WriteAllText(Path.Combine(active, "users.xml"), original);
            Reject<IOException>(() => store.Delete(a.Id));
            Reject<IOException>(() => store.Rename(a.Id, "in use"));
            Reject<IOException>(() => new CampaignSaveStore(legacy).Open(a.Id));
            Reject<InvalidOperationException>(() => CampaignSaveSession.Select(store, b.Id));
            CampaignSaveSession.Clear();
            Check(CampaignSaveSession.UserDataDirectory == null, "Stale path after teardown");
            CampaignSaveSession.Select(store, b.Id);
            Check(!File.Exists(Path.Combine(CampaignSaveSession.UserDataDirectory, "users.xml")), "Cross-campaign save leak");
            CampaignSaveSession.Clear();
            Check(store.List().Single(x => x.Id == b.Id).LastPlayedUtc != default, "Missing last played metadata");

            for (int i = 0; i < 130; i++) store.Create("Campaign " + i);
            Check(store.List().Count == 133, "Fixed slot limit or lost campaigns");
            Directory.CreateDirectory(Path.Combine(store.Root, ".create-interrupted"));
            Check(store.List().Count == 133, "Partially published campaign appeared");
            var broken = store.Create("Broken metadata");
            File.WriteAllText(Path.Combine(store.Root, broken.Id, "campaign.xml"), "<broken");
            Check(store.List().Single(x => x.Id == broken.Id).Error != null && store.List().Count == 134, "One bad save hid the others");
            store.Delete(broken.Id);
            store.Delete(a.Id);
            Check(!Directory.Exists(Path.Combine(store.Root, a.Id)) && Directory.Exists(Path.Combine(store.Root, b.Id)), "Deletion crossed slot boundary");
            store.Delete("legacy"); store.Initialize();
            Check(store.List().All(x => x.Id != "legacy"), "Deleted legacy campaign reappeared");
            Check(File.ReadAllText(Path.Combine(legacy, "users.xml")) == original, "Deletion damaged original import source");

            // Link traversal is refused, including when deleting an otherwise valid slot.
            string linked = Path.Combine(store.Root, b.Id, "userdata", "outside");
            bool linkCreated = false;
            try { Directory.CreateSymbolicLink(linked, legacy); linkCreated = true; }
            catch (Exception e) when (e is UnauthorizedAccessException || e is IOException || e is PlatformNotSupportedException)
            { Console.WriteLine("SKIP: symbolic-link creation is unavailable on this host."); }
            if (linkCreated)
            {
                try { Reject<IOException>(() => store.Delete(b.Id)); Reject<IOException>(() => store.Open(b.Id)); }
                finally { Directory.Delete(linked); }
            }
            Console.WriteLine("PASS: " + checks + " campaign checks: legacy import/retry, independent profiles, 130+ slots, naming, corruption isolation, leases and safe deletion.");
        }
        finally
        {
            CampaignSaveSession.Clear();
            // Only this run's owned fixture is ever removed.
            if (Path.GetDirectoryName(Path.GetFullPath(fixture)) != parent || !Path.GetFileName(fixture).StartsWith("owned-", StringComparison.Ordinal) ||
                File.ReadAllText(Path.Combine(fixture, ".test-owner")) != "campaign-save-test")
                throw new Exception("Refusing to remove an unverified fixture.");
            Directory.Delete(fixture, true);
        }
    }
}
