using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Eclipse.Modding
{
    public sealed class LooseModProvider : IAssetByteProvider, IAssetEnumerableProvider
    {
        private sealed class LooseAsset
        {
            public AssetMetadata Metadata;
            public string FilePath;
        }

        private readonly Dictionary<AssetId, LooseAsset> _assets = new Dictionary<AssetId, LooseAsset>();
        private readonly List<AssetMetadata> _metadata = new List<AssetMetadata>();
        private readonly IReadOnlyList<AssetMetadata> _readOnlyMetadata;

        public ModDescriptor Mod { get; }
        public ModId Namespace => Mod.Id;
        public string AssetsRoot { get; }
        public string ScriptsRoot { get; }
        public string LocalizationsRoot { get; }
        public IReadOnlyList<AssetMetadata> Assets => _readOnlyMetadata;

        public LooseModProvider(ModDescriptor mod)
        {
            _readOnlyMetadata = _metadata.AsReadOnly();
            Mod = mod ?? throw new ArgumentNullException(nameof(mod));
            if (mod.SourceKind != ModSourceKind.Loose)
                throw new ArgumentException("LooseModProvider requires a loose mod descriptor.", nameof(mod));

            string modRoot = Path.GetFullPath(mod.RootPath);
            AssetsRoot = Path.GetFullPath(Path.Combine(modRoot, "assets"));
            ScriptsRoot = Path.GetFullPath(Path.Combine(modRoot, "scripts"));
            LocalizationsRoot = Path.GetFullPath(Path.Combine(modRoot, "localizations"));
            EnsureContained(modRoot, AssetsRoot, "Loose assets root escapes the mod root.");
            EnsureContained(modRoot, ScriptsRoot, "Loose scripts root escapes the mod root.");
            EnsureContained(modRoot, LocalizationsRoot, "Loose localizations root escapes the mod root.");
            if (Directory.Exists(AssetsRoot)) IndexDirectory(AssetsRoot, string.Empty);
            if (Directory.Exists(ScriptsRoot)) IndexDirectory(ScriptsRoot, "scripts/");
            if (Directory.Exists(LocalizationsRoot)) IndexDirectory(LocalizationsRoot, "localizations/");
        }

        public bool TryDescribe(AssetId id, out AssetMetadata metadata)
        {
            metadata = null;
            if (id.Namespace != Namespace) return false;
            LooseAsset asset;
            if (!_assets.TryGetValue(id, out asset)) return false;
            metadata = asset.Metadata;
            return true;
        }

        public bool TryRead(AssetId id, out AssetBytes bytes)
        {
            bytes = null;
            if (id.Namespace != Namespace) return false;
            LooseAsset asset;
            if (!_assets.TryGetValue(id, out asset)) return false;

            byte[] data = File.ReadAllBytes(asset.FilePath);
            if (data.LongLength != asset.Metadata.Size)
                throw new IOException("Loose mod asset changed after indexing: " + asset.Metadata.DiagnosticSource);
            bytes = new AssetBytes(asset.Metadata, data);
            return true;
        }

        private void IndexDirectory(string root, string logicalPrefix)
        {
            var pending = new Stack<string>();
            pending.Push(root);
            while (pending.Count != 0)
            {
                string directory = pending.Pop();
                RejectReparsePoint(directory);

                string[] directories = Directory.GetDirectories(directory);
                Array.Sort(directories, StringComparer.Ordinal);
                for (int i = directories.Length - 1; i >= 0; i--)
                {
                    RejectReparsePoint(directories[i]);
                    pending.Push(directories[i]);
                }

                string[] files = Directory.GetFiles(directory);
                Array.Sort(files, StringComparer.Ordinal);
                foreach (string file in files)
                {
                    RejectReparsePoint(file);
                    IndexFile(root, logicalPrefix, file);
                }
            }
        }

        private void IndexFile(string root, string logicalPrefix, string file)
        {
            string fullPath = Path.GetFullPath(file);
            EnsureContained(root, fullPath, "Loose file escapes its mod content root: " + file);
            string relative = fullPath.Substring(root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Length + 1)
                .Replace('\\', '/');

            string extension = Path.GetExtension(relative);
            if (string.IsNullOrEmpty(extension))
                throw new InvalidDataException("Loose mod asset has no file extension: " + relative);
            string logicalPath = logicalPrefix + relative.Substring(0, relative.Length - extension.Length);
            AssetId id = AssetId.Parse(Namespace.Value + ":" + logicalPath);
            AssetKind kind = GetKind(id.Path, extension);
            if (extension.Equals(".asset", StringComparison.OrdinalIgnoreCase))
            {
                string descriptor;
                try { descriptor = new UTF8Encoding(false, true).GetString(File.ReadAllBytes(fullPath)); }
                catch (DecoderFallbackException exception)
                {
                    throw new InvalidDataException("Asset descriptor is not valid UTF-8: " + relative, exception);
                }
                kind = AssetDescriptor.GetKind(AssetDescriptor.ParseFields(descriptor, relative), relative);
            }

            var metadata = new AssetMetadata(id, kind, AssetSourceKind.LooseMod,
                extension.ToLowerInvariant(), new FileInfo(fullPath).Length, relative);
            if (_assets.ContainsKey(id))
                throw new InvalidDataException("Loose mod assets collide at logical ID '" + id + "'.");
            _assets.Add(id, new LooseAsset { Metadata = metadata, FilePath = fullPath });
            _metadata.Add(metadata);
        }

        private static AssetKind GetKind(string logicalPath, string extension)
        {
            string ext = extension.ToLowerInvariant();
            if (ext == ".png" && logicalPath.StartsWith("sprites/", StringComparison.Ordinal)) return AssetKind.Sprite;
            if (ext == ".png") return AssetKind.Texture;
            if (ext == ".xml" && logicalPath.StartsWith("models/", StringComparison.Ordinal)) return AssetKind.Model;
            if (ext == ".wav" || ext == ".ogg" || ext == ".mp3") return AssetKind.Audio;
            if (ext == ".xml" || ext == ".toml" || ext == ".json" || ext == ".txt" || ext == ".lua")
                return AssetKind.Text;
            return AssetKind.Binary;
        }

        private static void RejectReparsePoint(string path)
        {
            if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
                throw new InvalidDataException("Loose mods may not contain symbolic links or reparse points: " + path);
        }

        private static void EnsureContained(string root, string path, string message)
        {
            string normalizedRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string normalizedPath = Path.GetFullPath(path);
            if (string.Equals(normalizedRoot, normalizedPath, StringComparison.OrdinalIgnoreCase)) return;
            string prefix = normalizedRoot + Path.DirectorySeparatorChar;
            if (!normalizedPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException(message);
        }
    }
}
