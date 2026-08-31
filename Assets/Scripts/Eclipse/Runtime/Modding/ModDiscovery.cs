using System;
using System.Collections.Generic;
using System.IO;

namespace Eclipse.Modding
{
    public static class ModDiscovery
    {
        public static ModDiscoveryResult DiscoverLoose(string modsRoot)
        {
            if (string.IsNullOrEmpty(modsRoot)) throw new ArgumentNullException(nameof(modsRoot));
            string root = Path.GetFullPath(modsRoot);
            var mods = new List<ModDescriptor>();
            var diagnostics = new List<ModDiagnostic>();
            if (!Directory.Exists(root))
                return new ModDiscoveryResult(Array.Empty<ModDescriptor>(), Array.Empty<ModDiagnostic>());

            string[] directories = Directory.GetDirectories(root);
            Array.Sort(directories, (a, b) => string.CompareOrdinal(Path.GetFileName(a), Path.GetFileName(b)));
            var ids = new HashSet<ModId>();
            foreach (string directory in directories)
            {
                string folder = Path.GetFileName(directory);
                string manifestPath = Path.Combine(directory, "mod.toml");
                if (!File.Exists(manifestPath))
                {
                    diagnostics.Add(new ModDiagnostic(ModDiagnosticSeverity.Warning, "MOD001", folder,
                        "Ignoring directory without mod.toml."));
                    continue;
                }

                try
                {
                    ModManifest manifest = ModManifestReader.ReadExternalFile(manifestPath);
                    if (!string.Equals(folder, manifest.Id.Value, StringComparison.Ordinal))
                    {
                        diagnostics.Add(new ModDiagnostic(ModDiagnosticSeverity.Error, "MOD002", manifest.Id.Value,
                            "Mod directory must exactly match manifest id. Found '" + folder + "'."));
                        continue;
                    }
                    if (!ids.Add(manifest.Id))
                    {
                        diagnostics.Add(new ModDiagnostic(ModDiagnosticSeverity.Error, "MOD003", manifest.Id.Value,
                            "Duplicate mod id discovered."));
                        continue;
                    }
                    mods.Add(new ModDescriptor(manifest, Path.GetFullPath(directory), ModSourceKind.Loose));
                }
                catch (Exception exception) when (exception is FormatException || exception is IOException ||
                    exception is UnauthorizedAccessException)
                {
                    diagnostics.Add(new ModDiagnostic(ModDiagnosticSeverity.Error, "MOD004", folder, exception.Message));
                }
            }

            return new ModDiscoveryResult(mods.ToArray(), diagnostics.ToArray());
        }
    }
}
