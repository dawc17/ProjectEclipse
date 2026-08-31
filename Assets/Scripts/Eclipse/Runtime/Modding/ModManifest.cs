using System;
using System.Collections.Generic;

namespace Eclipse.Modding
{
    public sealed class ModDependency
    {
        public ModId Id { get; }
        public VersionRange Version { get; }

        internal ModDependency(ModId id, VersionRange version)
        {
            Id = id;
            Version = version;
        }
    }

    public sealed class ModManifest
    {
        public int Schema { get; }
        public ModId Id { get; }
        public string Name { get; }
        public SemanticVersion Version { get; }
        public VersionRange Api { get; }
        public IReadOnlyList<string> Authors { get; }
        public string Entrypoint { get; }
        public IReadOnlyList<string> Capabilities { get; }
        public IReadOnlyList<ModDependency> Dependencies { get; }

        internal ModManifest(int schema, ModId id, string name, SemanticVersion version,
            VersionRange api, string[] authors, string entrypoint, string[] capabilities,
            ModDependency[] dependencies)
        {
            Schema = schema;
            Id = id;
            Name = name;
            Version = version;
            Api = api;
            Authors = Array.AsReadOnly(authors ?? Array.Empty<string>());
            Entrypoint = entrypoint;
            Capabilities = Array.AsReadOnly(capabilities ?? Array.Empty<string>());
            Dependencies = Array.AsReadOnly(dependencies ?? Array.Empty<ModDependency>());
        }
    }

    public enum ModSourceKind
    {
        Loose = 0
    }

    public sealed class ModDescriptor
    {
        public ModManifest Manifest { get; }
        public string RootPath { get; }
        public ModSourceKind SourceKind { get; }

        internal ModDescriptor(ModManifest manifest, string rootPath, ModSourceKind sourceKind)
        {
            Manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            RootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            SourceKind = sourceKind;
        }

        public ModId Id => Manifest.Id;
        public SemanticVersion Version => Manifest.Version;
    }

    public static class ModPlatformVersions
    {
        public static readonly SemanticVersion Api = SemanticVersion.Parse("0.1.0");
        public static readonly SemanticVersion Core = SemanticVersion.Parse("1.0.0");
    }
}
