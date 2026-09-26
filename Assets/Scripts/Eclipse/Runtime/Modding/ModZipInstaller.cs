using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Eclipse.Modding
{
    public sealed class ModZipPreview
    {
        public ModManifest Manifest { get; }
        public bool IsUpdate { get; }

        internal ModZipPreview(ModManifest manifest, bool isUpdate)
        {
            Manifest = manifest;
            IsUpdate = isUpdate;
        }
    }

    // ZIPs are a transport format. The runtime still mounts validated loose mod folders.
    public static class ModZipInstaller
    {
        private const int MaxEntries = 10000;
        private const long MaxManifestBytes = 1024 * 1024;
        private const long MaxFileBytes = 256L * 1024 * 1024;
        private const long MaxTotalBytes = 512L * 1024 * 1024;

        private sealed class ArchivePlan
        {
            public ModManifest Manifest;
            public string Prefix;
            public List<ZipArchiveEntry> Files;
        }

        public static ModZipPreview Inspect(string zipPath, string modsRoot)
        {
            if (string.IsNullOrEmpty(zipPath)) throw new ArgumentNullException(nameof(zipPath));
            if (string.IsNullOrEmpty(modsRoot)) throw new ArgumentNullException(nameof(modsRoot));
            using (var archive = ZipFile.OpenRead(zipPath))
            {
                ArchivePlan plan = Plan(archive);
                return new ModZipPreview(plan.Manifest,
                    Directory.Exists(Path.Combine(Path.GetFullPath(modsRoot), plan.Manifest.Id.Value)));
            }
        }

        public static ModZipPreview Install(string zipPath, string modsRoot, bool replaceExisting)
        {
            return Install(zipPath, modsRoot, replaceExisting, default);
        }

        public static ModZipPreview Install(string zipPath, string modsRoot, bool replaceExisting, ModId approvedId)
        {
            if (string.IsNullOrEmpty(zipPath)) throw new ArgumentNullException(nameof(zipPath));
            if (string.IsNullOrEmpty(modsRoot)) throw new ArgumentNullException(nameof(modsRoot));
            string root = Path.GetFullPath(modsRoot);
            string parent = Directory.GetParent(root)?.FullName;
            if (parent == null) throw new InvalidOperationException("Mods root has no parent directory.");

            using (var archive = ZipFile.OpenRead(zipPath))
            {
                ArchivePlan plan = Plan(archive);
                if (approvedId.Value.Length != 0 && plan.Manifest.Id != approvedId)
                    throw new InvalidDataException("ZIP mod ID changed after preview; select it again.");
                string destination = Path.Combine(root, plan.Manifest.Id.Value);
                if (Directory.Exists(destination) &&
                    (File.GetAttributes(destination) & FileAttributes.ReparsePoint) != 0)
                    throw new InvalidDataException("Installed mod folder is a symbolic link; replace it manually.");
                if (Directory.Exists(destination) && !replaceExisting)
                    throw new IOException("Mod '" + plan.Manifest.Id + "' is already installed.");

                Directory.CreateDirectory(parent);
                string transaction = "." + Path.GetFileName(root) + "-install-" + Guid.NewGuid().ToString("N");
                string stageRoot = Path.Combine(parent, transaction);
                string stagedMod = Path.Combine(stageRoot, plan.Manifest.Id.Value);
                string backup = Path.Combine(parent, transaction + "-backup");
                bool movedOld = false;
                bool installed = false;
                try
                {
                    Directory.CreateDirectory(stagedMod);
                    long extracted = 0;
                    foreach (ZipArchiveEntry entry in plan.Files)
                    {
                        string relative = entry.FullName.Substring(plan.Prefix.Length);
                        string output = Path.Combine(stagedMod, relative.Replace('/', Path.DirectorySeparatorChar));
                        Directory.CreateDirectory(Path.GetDirectoryName(output));
                        using (Stream input = entry.Open())
                        using (var file = new FileStream(output, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                            CopyBounded(input, file, ref extracted, MaxFileBytes, MaxTotalBytes);
                    }

                    string manifestPath = Path.Combine(stagedMod, "mod.toml");
                    ModManifest manifest = ModManifestReader.ReadExternalFile(manifestPath);
                    if (manifest.Id != plan.Manifest.Id ||
                        !File.Exists(Path.Combine(stagedMod, manifest.Entrypoint.Replace('/', Path.DirectorySeparatorChar))))
                        throw new InvalidDataException("Installed files do not match the manifest or its Lua entrypoint.");
                    // Index assets before exposing a package to the game.
                    new LooseModProvider(new ModDescriptor(manifest, stagedMod, ModSourceKind.Loose));

                    Directory.CreateDirectory(root);
                    if (Directory.Exists(destination))
                    {
                        if (!replaceExisting) throw new IOException("Mod '" + manifest.Id + "' is already installed.");
                        Directory.Move(destination, backup);
                        movedOld = true;
                    }
                    Directory.Move(stagedMod, destination);
                    installed = true;
                    if (movedOld)
                    {
                        try { Directory.Delete(backup, true); }
                        catch (IOException) { /* The new installation is complete; leave the old backup for recovery. */ }
                        catch (UnauthorizedAccessException) { /* Same as above. */ }
                    }
                    return new ModZipPreview(manifest, movedOld);
                }
                catch
                {
                    if (movedOld && !installed && !Directory.Exists(destination))
                        Directory.Move(backup, destination);
                    throw;
                }
                finally
                {
                    try { if (Directory.Exists(stageRoot)) Directory.Delete(stageRoot, true); }
                    catch (IOException) { /* A stale empty staging directory must not mask the install result. */ }
                    catch (UnauthorizedAccessException) { /* Same as above. */ }
                }
            }
        }

        private static ArchivePlan Plan(ZipArchive archive)
        {
            if (archive.Entries.Count == 0 || archive.Entries.Count > MaxEntries)
                throw new InvalidDataException("Mod ZIP has no files or too many entries.");
            var files = new List<ZipArchiveEntry>();
            var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            ZipArchiveEntry manifestEntry = null;
            long declaredBytes = 0;
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string path = entry.FullName;
                ValidatePath(path);
                if (path == ".DS_Store" || path.StartsWith("__MACOSX/", StringComparison.Ordinal)) continue;
                if (((entry.ExternalAttributes >> 16) & 0xF000) == 0xA000)
                    throw new InvalidDataException("Mod ZIP cannot contain symbolic links: " + path);
                if (path.EndsWith("/", StringComparison.Ordinal)) continue;
                if (!paths.Add(path)) throw new InvalidDataException("Duplicate ZIP path: " + path);
                if (entry.Length > MaxFileBytes || declaredBytes > MaxTotalBytes - entry.Length)
                    throw new InvalidDataException("Mod ZIP exceeds the 512 MiB unpacked limit.");
                declaredBytes += entry.Length;
                files.Add(entry);
                if (path == "mod.toml" || (path.EndsWith("/mod.toml", StringComparison.Ordinal) &&
                    path.IndexOf('/') == path.LastIndexOf('/')))
                {
                    if (manifestEntry != null) throw new InvalidDataException("Mod ZIP contains multiple manifests.");
                    manifestEntry = entry;
                }
            }
            if (manifestEntry == null) throw new InvalidDataException("Mod ZIP needs mod.toml at its root or inside one folder.");
            string prefix = manifestEntry.FullName.Substring(0, manifestEntry.FullName.Length - "mod.toml".Length);
            foreach (ZipArchiveEntry entry in files)
                if (!entry.FullName.StartsWith(prefix, StringComparison.Ordinal) || entry.FullName == prefix)
                    throw new InvalidDataException("Mod ZIP contains files outside its mod folder.");
            if (manifestEntry.Length > MaxManifestBytes)
                throw new InvalidDataException("mod.toml is too large.");
            string manifestText;
            using (Stream stream = manifestEntry.Open())
            using (var copy = new MemoryStream())
            {
                long copied = 0;
                CopyBounded(stream, copy, ref copied, MaxManifestBytes, MaxManifestBytes);
                copy.Position = 0;
                using (var reader = new StreamReader(copy, new UTF8Encoding(false, true), true))
                    manifestText = reader.ReadToEnd();
            }
            ModManifest manifest = ModManifestReader.ParseExternal(manifestText);
            ValidatePath(manifest.Id.Value); // A valid ID must also be a safe folder name on every platform.
            return new ArchivePlan
            {
                Manifest = manifest,
                Prefix = prefix,
                Files = files
            };
        }

        private static void ValidatePath(string path)
        {
            if (string.IsNullOrEmpty(path) || path[0] == '/' || path.IndexOf('\\') >= 0 || path.IndexOf(':') >= 0)
                throw new InvalidDataException("Unsafe ZIP path: " + path);
            string trimmed = path.EndsWith("/", StringComparison.Ordinal) ? path.Substring(0, path.Length - 1) : path;
            foreach (string part in trimmed.Split('/'))
            {
                if (part.Length == 0 || part == "." || part == ".." || part.EndsWith(".", StringComparison.Ordinal) ||
                    part.EndsWith(" ", StringComparison.Ordinal) || part.IndexOfAny("<>\"|?*".ToCharArray()) >= 0)
                    throw new InvalidDataException("Unsafe ZIP path: " + path);
                foreach (char c in part)
                    if (char.IsControl(c)) throw new InvalidDataException("Unsafe ZIP path: " + path);
                string device = part.Split('.')[0].ToUpperInvariant();
                if (device == "CON" || device == "PRN" || device == "AUX" || device == "NUL" ||
                    (device.Length == 4 && (device.StartsWith("COM", StringComparison.Ordinal) ||
                    device.StartsWith("LPT", StringComparison.Ordinal)) && device[3] >= '1' && device[3] <= '9'))
                    throw new InvalidDataException("Reserved ZIP path: " + path);
            }
        }

        private static void CopyBounded(Stream input, Stream output, ref long total, long fileLimit, long totalLimit)
        {
            var buffer = new byte[81920];
            long fileBytes = 0;
            int count;
            while ((count = input.Read(buffer, 0, buffer.Length)) != 0)
            {
                if (count > fileLimit - fileBytes || count > totalLimit - total)
                    throw new InvalidDataException("Mod ZIP exceeds the unpacked size limit.");
                output.Write(buffer, 0, count);
                fileBytes += count;
                total += count;
            }
        }
    }
}
