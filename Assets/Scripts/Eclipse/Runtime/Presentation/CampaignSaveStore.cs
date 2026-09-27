using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Eclipse.Saves
{
    public sealed class CampaignSaveInfo
    {
        public string Id { get; internal set; }
        public string Name { get; internal set; }
        public DateTime CreatedUtc { get; internal set; }
        public DateTime LastPlayedUtc { get; internal set; }
        public string Error { get; internal set; }
    }

    // Each campaign owns its entire userdata directory. No copying files back
    // and forth through a shared active users.xml, and no fixed slot count.
    public sealed class CampaignSaveStore
    {
        private readonly string _legacy;
        public string Root { get; }
        public const int MaximumNameLength = 48;

        public CampaignSaveStore(string legacyUserDataDirectory)
        {
            _legacy = Path.GetFullPath(legacyUserDataDirectory);
            Root = Path.Combine(Path.GetDirectoryName(_legacy), "CampaignSaves");
        }

        public void Initialize()
        {
            CheckPath(Root);
            Directory.CreateDirectory(Root);
            using (LockCatalog())
            {
                string marker = Path.Combine(Root, ".legacy-imported");
                if (File.Exists(marker)) return;
                string destination = SlotPath("legacy");
                if (!Directory.Exists(destination) && HasLegacyProgress())
                {
                    // Publish only a complete import. The old directory is left intact.
                    string staging = Path.Combine(Root, ".import-" + Guid.NewGuid().ToString("N"));
                    Directory.CreateDirectory(Path.Combine(staging, "userdata"));
                    try
                    {
                        CheckPath(_legacy);
                        foreach (string file in Directory.GetFiles(_legacy))
                        {
                            if (file.EndsWith(".lock", StringComparison.OrdinalIgnoreCase)) continue;
                            CopyFile(file, Path.Combine(staging, "userdata", Path.GetFileName(file)));
                        }
                        string assets = Path.Combine(_legacy, "assets");
                        if (Directory.Exists(assets)) CopyTree(assets, Path.Combine(staging, "userdata", "assets"));
                        var info = new CampaignSaveInfo { Id = "legacy", Name = "Existing campaign", CreatedUtc = DateTime.UtcNow };
                        WriteInfo(staging, info);
                        MoveDirectory(staging, destination);
                    }
                    finally { if (Directory.Exists(staging)) DeleteTree(staging); }
                }
                // Also records an empty first install, so deleting an imported
                // campaign never resurrects the preserved legacy copy.
                WriteAtomic(marker, Encoding.UTF8.GetBytes("1"));
            }
        }

        public List<CampaignSaveInfo> List()
        {
            var result = new List<CampaignSaveInfo>();
            CheckPath(Root);
            foreach (string directory in Directory.EnumerateDirectories(Root))
            {
                string id = Path.GetFileName(directory);
                if (!ValidId(id)) continue; // Ignore unpublished creates/imports/deletions.
                try { result.Add(ReadInfo(id)); }
                catch (Exception error) when (error is IOException || error is UnauthorizedAccessException || error is XmlException || error is FormatException || error is ArgumentException)
                {
                    result.Add(new CampaignSaveInfo { Id = id, Name = "Unreadable campaign", Error = error.Message });
                }
            }
            result.Sort((a, b) =>
            {
                int order = b.LastPlayedUtc.CompareTo(a.LastPlayedUtc);
                if (order == 0) order = b.CreatedUtc.CompareTo(a.CreatedUtc);
                return order == 0 ? string.CompareOrdinal(a.Id, b.Id) : order;
            });
            return result;
        }

        public CampaignSaveInfo Create(string name)
        {
            name = ValidateName(name);
            using (LockCatalog())
            {
                var info = new CampaignSaveInfo { Id = Guid.NewGuid().ToString("N"), Name = name, CreatedUtc = DateTime.UtcNow };
                string staging = Path.Combine(Root, ".create-" + info.Id);
                Directory.CreateDirectory(Path.Combine(staging, "userdata", "assets"));
                try { WriteInfo(staging, info); MoveDirectory(staging, SlotPath(info.Id)); }
                finally { if (Directory.Exists(staging)) DeleteTree(staging); }
                // No users.xml: the normal game loader creates a fresh profile
                // using the currently enabled content, not a copy of another save.
                return info;
            }
        }

        public void Rename(string id, string name)
        {
            name = ValidateName(name);
            using (LockCatalog())
            using (LockSlot(id))
            {
                CampaignSaveInfo info = ReadInfo(id);
                info.Name = name;
                WriteInfo(SlotPath(id), info);
            }
        }

        public void Delete(string id)
        {
            using (LockCatalog())
            {
                string source = SlotPath(id);
                CheckTree(source);
                // All activation/deletion operations hold the catalog lock, so
                // releasing this probe before the directory move is race-free.
                using (LockSlot(id)) { }
                string removed = Path.Combine(Root, ".delete-" + Guid.NewGuid().ToString("N"));
                MoveDirectory(source, removed);
                DeleteTree(removed);
            }
        }

        public CampaignSaveLease Open(string id)
        {
            using (LockCatalog())
            {
                FileStream lease = LockSlot(id);
                try
                {
                    CampaignSaveInfo info = ReadInfo(id);
                    string directory = Path.Combine(SlotPath(id), "userdata");
                    CheckTree(directory);
                    info.LastPlayedUtc = DateTime.UtcNow;
                    WriteInfo(SlotPath(id), info);
                    return new CampaignSaveLease(directory, lease);
                }
                catch { lease.Dispose(); throw; }
            }
        }

        // Called only for visible rows: listing many slots does not parse every profile.
        public string Progress(string id)
        {
            string directory = Path.Combine(SlotPath(id), "userdata");
            CheckPath(directory);
            bool found = false;
            foreach (string name in new[] { "users.xml", "users_backup.xml" })
            {
                string path = Path.Combine(directory, name);
                if (!File.Exists(path)) continue;
                found = true;
                try
                {
                    CheckPath(path);
                    using (XmlReader reader = Reader(path, 64L * 1024 * 1024))
                        while (reader.Read())
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Warrior" && reader.GetAttribute("ID") == "1")
                            {
                                string level = reader.GetAttribute("Level"), zone = reader.GetAttribute("CurrentZone");
                                int number;
                                string text = int.TryParse(level, out number) ? "Level " + number : "Campaign in progress";
                                if (zone != null && zone.StartsWith("ZONE_", StringComparison.Ordinal) && int.TryParse(zone.Substring(5), out number))
                                    text += "  ·  Act " + number;
                                return text;
                            }
                }
                catch (Exception error) when (error is XmlException || error is IOException || error is UnauthorizedAccessException) { }
            }
            if (File.Exists(Path.Combine(directory, "users.xml.eclipse-write")) ||
                File.Exists(Path.Combine(directory, "users_backup.xml.eclipse-write"))) return "Save recovery pending";
            return found ? "Progress unavailable" : "New campaign";
        }

        private CampaignSaveInfo ReadInfo(string id)
        {
            string directory = SlotPath(id);
            CheckPath(directory);
            CheckPath(Path.Combine(directory, "userdata"));
            string path = Path.Combine(directory, "campaign.xml");
            CheckPath(path);
            var document = new XmlDocument { XmlResolver = null };
            using (var reader = Reader(path, 32768)) document.Load(reader);
            XmlElement root = document.DocumentElement;
            if (root == null || root.Name != "Campaign" || root.GetAttribute("format") != "1" || root.GetAttribute("id") != id)
                throw new InvalidDataException("Unsupported campaign metadata.");
            var info = new CampaignSaveInfo { Id = id, Name = ValidateName(root.GetAttribute("name")),
                CreatedUtc = DateTime.Parse(root.GetAttribute("created"), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).ToUniversalTime() };
            string last = root.GetAttribute("played");
            if (last.Length != 0) info.LastPlayedUtc = DateTime.Parse(last, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).ToUniversalTime();
            foreach (string filename in new[] { "users.xml", "users_backup.xml" })
            {
                string file = Path.Combine(directory, "userdata", filename);
                if (File.Exists(file) && File.GetLastWriteTimeUtc(file) > info.LastPlayedUtc) info.LastPlayedUtc = File.GetLastWriteTimeUtc(file);
            }
            return info;
        }

        private static void WriteInfo(string directory, CampaignSaveInfo info)
        {
            var document = new XmlDocument();
            var element = document.CreateElement("Campaign"); document.AppendChild(element);
            element.SetAttribute("format", "1"); element.SetAttribute("id", info.Id); element.SetAttribute("name", info.Name);
            element.SetAttribute("created", info.CreatedUtc.ToString("O", CultureInfo.InvariantCulture));
            if (info.LastPlayedUtc != default) element.SetAttribute("played", info.LastPlayedUtc.ToString("O", CultureInfo.InvariantCulture));
            using (var stream = new MemoryStream()) { document.Save(stream); WriteAtomic(Path.Combine(directory, "campaign.xml"), stream.ToArray()); }
        }

        private bool HasLegacyProgress() => File.Exists(Path.Combine(_legacy, "users.xml")) ||
            File.Exists(Path.Combine(_legacy, "users_backup.xml")) || File.Exists(Path.Combine(_legacy, "users.xml.eclipse-write")) ||
            File.Exists(Path.Combine(_legacy, "users_backup.xml.eclipse-write"));

        private FileStream LockCatalog()
        {
            CheckPath(Root);
            return new FileStream(Path.Combine(Root, ".catalog.lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }

        private FileStream LockSlot(string id)
        {
            string directory = SlotPath(id);
            CheckPath(directory);
            return new FileStream(Path.Combine(directory, ".session.lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }

        private string SlotPath(string id)
        {
            if (!ValidId(id)) throw new ArgumentException("Invalid campaign ID.");
            CheckPath(Root);
            return Path.Combine(Root, id);
        }

        private static bool ValidId(string id) => id == "legacy" || (id != null && id.Length == 32 && Guid.TryParseExact(id, "N", out _));

        private static string ValidateName(string name)
        {
            name = (name ?? string.Empty).Trim();
            if (name.Length == 0 || name.Length > MaximumNameLength) throw new ArgumentException("Use a name between 1 and 48 characters.");
            foreach (char c in name) if (char.IsControl(c)) throw new ArgumentException("Campaign names cannot contain control characters.");
            return name;
        }

        private static XmlReader Reader(string path, long maximum) => XmlReader.Create(path,
            new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null, MaxCharactersInDocument = maximum });

        private static void CheckPath(string path)
        {
            // Reject links/junctions at every existing ancestor before any write,
            // copy or recursive delete. IDs and display names never become paths.
            for (string current = Path.GetFullPath(path); !string.IsNullOrEmpty(current); current = Path.GetDirectoryName(current))
                if ((Directory.Exists(current) || File.Exists(current)) && (File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0)
                    throw new IOException("Campaign storage cannot follow symbolic links or junctions.");
        }

        private static void CheckTree(string path)
        {
            CheckPath(path);
            foreach (string child in Directory.EnumerateFileSystemEntries(path))
            {
                CheckPath(child);
                if (Directory.Exists(child)) CheckTree(child);
            }
        }

        private static void CopyFile(string source, string destination)
        {
            CheckPath(source); File.Copy(source, destination, false);
        }

        private static void CopyTree(string source, string destination)
        {
            CheckPath(source); Directory.CreateDirectory(destination);
            foreach (string file in Directory.GetFiles(source))
                if (!file.EndsWith(".lock", StringComparison.OrdinalIgnoreCase)) CopyFile(file, Path.Combine(destination, Path.GetFileName(file)));
            foreach (string directory in Directory.GetDirectories(source)) CopyTree(directory, Path.Combine(destination, Path.GetFileName(directory)));
        }

        private static void DeleteTree(string path) { CheckTree(path); Directory.Delete(path, true); }

        private static void MoveDirectory(string source, string destination)
        {
            // Windows scanners can briefly hold a freshly written directory.
            // Retry only access/sharing errors, with a bounded total delay.
            for (int attempt = 0; ; attempt++)
            {
                try { Directory.Move(source, destination); return; }
                catch (IOException error) when (attempt < 4 &&
                    ((error.HResult & 0xffff) == 5 || (error.HResult & 0xffff) == 32 || (error.HResult & 0xffff) == 33))
                { System.Threading.Thread.Sleep(25 * (attempt + 1)); }
            }
        }

        private static void WriteAtomic(string path, byte[] bytes)
        {
            CheckPath(path);
            string temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                { stream.Write(bytes, 0, bytes.Length); stream.Flush(true); }
                if (File.Exists(path)) File.Replace(temporary, path, null); else File.Move(temporary, path);
            }
            finally { if (File.Exists(temporary)) File.Delete(temporary); }
        }
    }

    public sealed class CampaignSaveLease : IDisposable
    {
        private FileStream _lock;
        public string UserDataDirectory { get; }
        internal CampaignSaveLease(string directory, FileStream fileLock) { UserDataDirectory = directory; _lock = fileLock; }
        public void Dispose() { _lock?.Dispose(); _lock = null; }
    }

    // Cleared only after outgoing native scene/profile teardown. Keeping the
    // lease alive prevents another game process from opening or deleting it.
    public static class CampaignSaveSession
    {
        private static CampaignSaveLease _lease;
        public static string UserDataDirectory => _lease?.UserDataDirectory;
        public static void Select(CampaignSaveStore store, string id)
        {
            if (_lease != null) throw new InvalidOperationException("Return to the title before switching campaigns.");
            _lease = store.Open(id);
        }
        public static void Clear() { _lease?.Dispose(); _lease = null; }
    }
}
