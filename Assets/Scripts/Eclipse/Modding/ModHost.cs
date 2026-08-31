using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Eclipse.Modding
{
    public sealed class ModHost : IDisposable
    {
        public string ModsRoot { get; }
        public IReadOnlyList<ModDescriptor> EnabledMods { get; }
        public IReadOnlyList<ModDiagnostic> Diagnostics { get; }
        public AssetResolver Assets { get; }
        public ModAssetLoader TypedAssets { get; }

        public bool HasErrors
        {
            get
            {
                foreach (ModDiagnostic diagnostic in Diagnostics)
                    if (diagnostic.Severity == ModDiagnosticSeverity.Error) return true;
                return false;
            }
        }

        private ModHost(string modsRoot, ModDescriptor[] enabledMods,
            ModDiagnostic[] diagnostics, AssetResolver assets)
        {
            ModsRoot = modsRoot;
            EnabledMods = Array.AsReadOnly(enabledMods ?? Array.Empty<ModDescriptor>());
            Diagnostics = Array.AsReadOnly(diagnostics ?? Array.Empty<ModDiagnostic>());
            Assets = assets ?? throw new ArgumentNullException(nameof(assets));
            TypedAssets = new ModAssetLoader(Assets);
        }

        public static ModHost Build(string modsRoot)
        {
            if (string.IsNullOrEmpty(modsRoot)) throw new ArgumentNullException(nameof(modsRoot));
            string root = Path.GetFullPath(modsRoot);
            ModDiscoveryResult discovery = ModDiscovery.DiscoverLoose(root);
            DependencyResolutionResult resolution = DependencyResolver.Resolve(discovery.Mods,
                ModPlatformVersions.Api, ModPlatformVersions.Core);

            var diagnostics = new List<ModDiagnostic>();
            diagnostics.AddRange(discovery.Diagnostics);
            diagnostics.AddRange(resolution.Diagnostics);

            var providers = new List<IAssetProvider> { new CoreAssetProvider() };
            var enabled = new List<ModDescriptor>();
            var mounted = new HashSet<ModId>();
            foreach (ModDescriptor mod in resolution.OrderedMods)
            {
                ModId unavailableDependency;
                if (TryFindUnavailableDependency(mod, mounted, out unavailableDependency))
                {
                    diagnostics.Add(new ModDiagnostic(ModDiagnosticSeverity.Error, "HOST002", mod.Id.Value,
                        "Dependency '" + unavailableDependency + "' failed to mount, so this mod is disabled."));
                    continue;
                }

                try
                {
                    var provider = new LooseModProvider(mod);
                    providers.Add(provider);
                    enabled.Add(mod);
                    mounted.Add(mod.Id);
                }
                catch (Exception exception) when (exception is IOException || exception is InvalidDataException ||
                    exception is UnauthorizedAccessException || exception is FormatException)
                {
                    diagnostics.Add(new ModDiagnostic(ModDiagnosticSeverity.Error, "HOST001", mod.Id.Value,
                        "Failed to mount loose assets: " + exception.Message));
                }
            }

            return new ModHost(root, enabled.ToArray(), diagnostics.ToArray(), new AssetResolver(providers));
        }

        public static ModHost BuildDefault()
        {
            return Build(GetDefaultModsRoot());
        }

        public static string GetDefaultModsRoot()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            string dataPath = Path.GetFullPath(Application.dataPath);
            DirectoryInfo parent = Directory.GetParent(dataPath);
            if (parent == null) throw new InvalidOperationException("Cannot determine game root from Application.dataPath.");
            return Path.Combine(parent.FullName, "Mods");
#else
            return Path.Combine(Application.persistentDataPath, "Mods");
#endif
        }

        public ModScriptSession StartScripts(IModScriptRuntime runtime, Action<ModLogEntry> logger = null)
        {
            return ModScriptSession.Start(this, runtime, logger);
        }

        public string FormatReport()
        {
            var builder = new StringBuilder();
            builder.Append("Mod API ").Append(ModPlatformVersions.Api)
                .Append(" | core ").Append(ModPlatformVersions.Core)
                .Append(" | enabled ").Append(EnabledMods.Count)
                .Append(" | diagnostics ").Append(Diagnostics.Count)
                .AppendLine();
            builder.Append("Mods root: ").AppendLine(ModsRoot);
            foreach (ModDescriptor mod in EnabledMods)
                builder.Append("+ ").Append(mod.Id).Append(' ').Append(mod.Version).AppendLine();
            foreach (ModDiagnostic diagnostic in Diagnostics)
                builder.Append("! ").AppendLine(diagnostic.ToString());
            return builder.ToString().TrimEnd();
        }

        public void Dispose()
        {
            TypedAssets.Dispose();
        }

        private static bool TryFindUnavailableDependency(ModDescriptor mod, HashSet<ModId> mounted,
            out ModId unavailable)
        {
            foreach (ModDependency dependency in mod.Manifest.Dependencies)
            {
                if (dependency.Id.Value == "core") continue;
                if (mounted.Contains(dependency.Id)) continue;
                unavailable = dependency.Id;
                return true;
            }
            unavailable = default;
            return false;
        }
    }
}
