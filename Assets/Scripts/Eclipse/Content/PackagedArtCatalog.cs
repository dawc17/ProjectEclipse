using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Eclipse.Content.TarAssets;
using UnityEngine;

namespace Eclipse.Content
{
    // Project-owned runtime-content compatibility facade. Version 2 catalogs use Unity Resources;
    // version 3 catalogs route runtime assets into standard TAR archives wrapped in LZ4 frames.
    // Generic gameplay/config TextAssets are deliberately excluded from this provider. Model XML is
    // exposed only through LoadModelText/HasModel because it is runtime geometry, not gameplay config.
    public static class PackagedArtCatalog
    {
        public const string ResourceRoot = "SF2Content/Art";
        public const string CatalogResourcePath = ResourceRoot + "/catalog";
        public const string TarStreamingRoot = "SF2Content/ArtBundles";
        public const string LooseFontRoot = "SF2Content/Fonts";

        [Serializable]
        public sealed class Catalog
        {
            public int version;
            public BundleRecord[] bundles;
            public FileRecord[] files;
        }

        [Serializable]
        public sealed class BundleRecord
        {
            public string name;
            public string namespaceId;
            public string file;
            public string sha256;
            public long size;
            public long unpackedSize;
            public ArtRecord[] assets;
        }

        [Serializable]
        public sealed class ArtRecord
        {
            public string address;
            public string texture;
            public string sprites;
            public string audio;
            public string font;
        }

        [Serializable]
        public sealed class FileRecord
        {
            public string path;
            public long size;
            public string sha256;
        }

        private sealed class BundleAsset
        {
            public readonly string BundleName;
            public readonly string AssetPath;

            public BundleAsset(string bundleName, string assetPath)
            {
                BundleName = bundleName;
                AssetPath = assetPath;
            }
        }

        private static readonly Dictionary<string, List<BundleAsset>> AssetsByPath =
            new Dictionary<string, List<BundleAsset>>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, List<BundleAsset>> AssetsByName =
            new Dictionary<string, List<BundleAsset>>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, Sprite> SpriteCache =
            new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, string> SpriteAliases =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // Modern raid bundles moved forge currencies out of the legacy
                // MiscSprites atlas while current gameplay data retains the old ids.
                { "miscsprites.forge_green", "ui/atlases/raidcurrencyforge.forge_green" },
                { "miscsprites.forge_red", "ui/atlases/raidcurrencyforge.forge_red" },
                { "miscsprites.forge_purple", "ui/atlases/raidcurrencyforge.forge_purple" }
            };

        private static bool _indexed;
        private static readonly Dictionary<string, NativeAssetGroup> OpenBundles =
            new Dictionary<string, NativeAssetGroup>(StringComparer.OrdinalIgnoreCase);

        public static bool IsSupportedAssetType(Type type)
        {
            return type == typeof(Sprite) || type == typeof(Texture2D) ||
                type == typeof(AudioClip) || type == typeof(Font) || type == typeof(Material);
        }

        public static T Load<T>(string resourcePath) where T : UnityEngine.Object
        {
            if (!IsSupportedAssetType(typeof(T)) || string.IsNullOrEmpty(resourcePath))
                return null;

            EnsureIndex();
            if (typeof(T) == typeof(Sprite))
                return LoadSprite(resourcePath) as T;

            foreach (BundleAsset entry in FindCandidates(resourcePath, typeof(T)))
            {
                NativeAssetGroup bundle = OpenBundle(entry.BundleName);
                if (bundle == null)
                    continue;
                T asset = bundle.LoadAsset<T>(entry.AssetPath);
                if (asset != null)
                    return asset;
            }
            return null;
        }

        public static string LoadModelText(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath))
                return null;
            string normalized = Normalize(resourcePath);
            if (!normalized.StartsWith("gamedata/models/", StringComparison.OrdinalIgnoreCase))
                return null;
            normalized = StripXmlExtension(normalized);
            EnsureIndex();
            foreach (BundleAsset entry in FindCandidates(normalized, typeof(TextAsset)))
            {
                NativeAssetGroup bundle = OpenBundle(entry.BundleName);
                if (bundle == null)
                    continue;
                string text = bundle.LoadText(entry.AssetPath, "model");
                if (!string.IsNullOrEmpty(text))
                    return text;
            }
            return null;
        }

        public static bool HasModel(string modelName)
        {
            if (string.IsNullOrEmpty(modelName)) return false;
            string path = modelName.Replace('\\', '/').Trim();
            path = GetLastSegment(StripXmlExtension(path));
            return !string.IsNullOrEmpty(LoadModelText("gamedata/models/" + path));
        }

        public static string LoadLocationDataText(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath))
                return null;
            string normalized = Normalize(resourcePath);
            if (!normalized.StartsWith("textures/locations/", StringComparison.OrdinalIgnoreCase) &&
                !normalized.StartsWith("textures/location_effects/", StringComparison.OrdinalIgnoreCase))
                return null;
            normalized = StripXmlExtension(normalized);
            EnsureIndex();
            foreach (BundleAsset entry in FindCandidates(normalized, typeof(TextAsset)))
            {
                NativeAssetGroup bundle = OpenBundle(entry.BundleName);
                if (bundle == null)
                    continue;
                string text = bundle.LoadText(entry.AssetPath, "atlas");
                if (!string.IsNullOrEmpty(text))
                    return text;
            }
            return null;
        }

        private static Sprite LoadSprite(string resourcePath)
        {
            string normalized = Normalize(resourcePath);
            string alias;
            if (SpriteAliases.TryGetValue(normalized, out alias) ||
                SpriteAliases.TryGetValue(GetLastSegment(normalized), out alias))
                normalized = alias;

            Sprite cached;
            if (SpriteCache.TryGetValue(normalized, out cached) && cached != null)
                return cached;

            foreach (BundleAsset entry in FindCandidates(normalized, typeof(Sprite)))
            {
                NativeAssetGroup bundle = OpenBundle(entry.BundleName);
                if (bundle == null)
                    continue;
                Sprite asset = bundle.LoadAsset<Sprite>(entry.AssetPath);
                if (asset != null && SpriteNameMatches(asset.name, normalized))
                {
                    SpriteCache[normalized] = asset;
                    return asset;
                }
            }

            Sprite member = LoadSpriteMember(normalized);
            if (member != null)
                SpriteCache[normalized] = member;
            return member;
        }

        private static Sprite LoadSpriteMember(string resourcePath)
        {
            string normalized = Normalize(resourcePath);
            int slash = normalized.LastIndexOf('/');
            string requestedName = (slash < 0) ? normalized : normalized.Substring(slash + 1);
            int dot = requestedName.IndexOf('.');
            if (dot <= 0 || dot == requestedName.Length - 1)
                return null;

            string atlasName = requestedName.Substring(0, dot);
            string memberName = requestedName.Substring(dot + 1);
            string atlasPath = ((slash < 0) ? string.Empty : normalized.Substring(0, slash + 1)) + atlasName;
            foreach (BundleAsset entry in FindCandidates(atlasPath, typeof(Sprite)))
            {
                NativeAssetGroup bundle = OpenBundle(entry.BundleName);
                if (bundle == null)
                    continue;
                Sprite[] sprites = bundle.LoadAssetWithSubAssets<Sprite>(entry.AssetPath);
                if (sprites == null)
                    continue;
                foreach (Sprite sprite in sprites)
                {
                    if (sprite != null &&
                        (string.Equals(sprite.name, requestedName, StringComparison.OrdinalIgnoreCase) ||
                         string.Equals(sprite.name, memberName, StringComparison.OrdinalIgnoreCase)))
                        return sprite;
                }
            }
            return null;
        }

        private static bool SpriteNameMatches(string spriteName, string resourcePath)
        {
            if (string.IsNullOrEmpty(spriteName))
                return false;
            string requestedName = GetLastSegment(Normalize(resourcePath));
            return string.Equals(spriteName, requestedName, StringComparison.OrdinalIgnoreCase);
        }

        public static T[] LoadWithSubAssets<T>(string resourcePath) where T : UnityEngine.Object
        {
            if (!IsSupportedAssetType(typeof(T)) || string.IsNullOrEmpty(resourcePath))
                return null;

            EnsureIndex();
            foreach (BundleAsset entry in FindCandidates(resourcePath, typeof(T)))
            {
                NativeAssetGroup bundle = OpenBundle(entry.BundleName);
                if (bundle == null)
                    continue;
                T[] assets = bundle.LoadAssetWithSubAssets<T>(entry.AssetPath);
                if (assets != null && assets.Length != 0)
                    return assets;
            }
            return null;
        }

        private static void EnsureIndex()
        {
            if (_indexed)
                return;
            TextAsset resource = Resources.Load<TextAsset>(CatalogResourcePath);
            if (resource == null)
                throw new InvalidDataException("Packaged art catalog is missing: " + CatalogResourcePath);
            Catalog catalog;
            try { catalog = ReadCatalog(resource.text); }
            finally { Resources.UnloadAsset(resource); }

            int count = 0;
            foreach (BundleRecord bundle in catalog.bundles)
            {
                OpenBundles.Add(bundle.name, new NativeAssetGroup(bundle));
                foreach (ArtRecord asset in bundle.assets)
                {
                    IndexAsset(bundle.name, asset.address);
                    count++;
                }
            }
            _indexed = true;
            Debug.Log("[PackagedArt] indexed " + count + " addresses from " + catalog.bundles.Length +
                (catalog.version == 3 ? " TAR/LZ4 asset groups." : " native asset groups."));
        }

        public static Catalog ReadCatalog(string json)
        {
            Catalog catalog = JsonUtility.FromJson<Catalog>(json);
            if (catalog == null || (catalog.version != 2 && catalog.version != 3) ||
                catalog.bundles == null || catalog.bundles.Length == 0)
                throw new InvalidDataException("Invalid packaged art catalog.");
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (BundleRecord bundle in catalog.bundles)
            {
                if (bundle == null || string.IsNullOrEmpty(bundle.name) ||
                    bundle.name.IndexOfAny(new[] { '/', '\\', ':' }) >= 0 ||
                    bundle.name == "." || bundle.name == ".." || !names.Add(bundle.name) ||
                    bundle.assets == null || bundle.assets.Length == 0)
                    throw new InvalidDataException("Invalid or duplicate packaged art bundle record.");
                if (catalog.version == 3)
                {
                    if (string.IsNullOrEmpty(bundle.namespaceId)) bundle.namespaceId = "core";
                    bool hasArchive = !string.IsNullOrEmpty(bundle.file);
                    if (bundle.namespaceId.IndexOfAny(new[] { '/', '\\', ':' }) >= 0 ||
                        (hasArchive && (!SafePath(bundle.file) ||
                        !bundle.file.EndsWith(".tar.lz4", StringComparison.OrdinalIgnoreCase) ||
                        bundle.size <= 0 || bundle.unpackedSize <= 0 || !IsSha256(bundle.sha256))))
                        throw new InvalidDataException("Invalid TAR/LZ4 art bundle record: " + bundle.name);
                }
                foreach (ArtRecord asset in bundle.assets)
                {
                    if (asset == null || string.IsNullOrEmpty(asset.address))
                        throw new InvalidDataException("Empty native art address: " + bundle.name);
                    if (catalog.version == 2)
                    {
                        foreach (string path in new[] { asset.texture, asset.sprites, asset.audio, asset.font })
                            if (!string.IsNullOrEmpty(path) && (!SafePath(path) || !path.StartsWith(ResourceRoot + "/", StringComparison.Ordinal)))
                                throw new InvalidDataException("Asset outside canonical art root: " + path);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(asset.texture) || !string.IsNullOrEmpty(asset.sprites) || !string.IsNullOrEmpty(asset.audio))
                            throw new InvalidDataException("TAR assets must be described inside their archive: " + asset.address);
                        if (!string.IsNullOrEmpty(asset.font) && (!SafePath(asset.font) ||
                            !asset.font.StartsWith(LooseFontRoot + "/", StringComparison.Ordinal)))
                            throw new InvalidDataException("Loose font is outside canonical font root: " + asset.font);
                        if (string.IsNullOrEmpty(bundle.file) && string.IsNullOrEmpty(asset.font))
                            throw new InvalidDataException("TAR catalog group has neither archive nor loose font: " + bundle.name);
                    }
                }
            }
            return catalog;
        }

        // Used before builds, without consulting the original research dump.
        public static Catalog ValidateProjectFiles(string assetsDirectory)
        {
            string root = Path.Combine(assetsDirectory, "Resources", ResourceRoot);
            Catalog catalog = ReadCatalog(File.ReadAllText(Path.Combine(root, "catalog.json")));
            if (catalog.version == 3)
                return ValidateTarProjectFiles(assetsDirectory, root, catalog);
            if (catalog.files == null || catalog.files.Length == 0)
                throw new InvalidDataException("Native art file inventory is missing.");
            var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var resources = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (FileRecord file in catalog.files)
            {
                if (file == null || !SafePath(file.path) || !paths.Add(file.path))
                    throw new InvalidDataException("Invalid or duplicate native art file.");
                string path = Path.Combine(root, file.path);
                resources.Add(ResourceRoot + "/" + file.path.Substring(0, file.path.Length - Path.GetExtension(file.path).Length));
                if (!File.Exists(path) || new FileInfo(path).Length != file.size || !File.Exists(path + ".meta"))
                    throw new InvalidDataException("Missing/incomplete native art file: " + path);
                using (var input = File.OpenRead(path))
                using (var hash = SHA256.Create())
                {
                    string actual = BitConverter.ToString(hash.ComputeHash(input)).Replace("-", "").ToLowerInvariant();
                    if (!string.Equals(actual, file.sha256, StringComparison.Ordinal))
                        throw new InvalidDataException("Native art checksum mismatch: " + path);
                }
            }
            foreach (BundleRecord group in catalog.bundles)
                foreach (ArtRecord asset in group.assets)
                    foreach (string resource in new[] { asset.texture, asset.sprites, asset.audio, asset.font })
                        if (!string.IsNullOrEmpty(resource) && !resources.Contains(resource))
                            throw new InvalidDataException("Uncatalogued art resource: " + resource);
            foreach (string path in Directory.GetFiles(root, "*", SearchOption.AllDirectories))
            {
                string relative = path.Substring(root.Length + 1).Replace('\\', '/');
                if (!relative.EndsWith(".meta", StringComparison.OrdinalIgnoreCase) && relative != "catalog.json" && !paths.Contains(relative))
                    throw new InvalidDataException("Uncatalogued art file: " + path);
            }
            return catalog;
        }

        private static Catalog ValidateTarProjectFiles(string assetsDirectory, string catalogRoot, Catalog catalog)
        {
            string streamingRoot = Path.Combine(assetsDirectory, "StreamingAssets", TarStreamingRoot);
            var archives = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (BundleRecord bundle in catalog.bundles)
            {
                if (!string.IsNullOrEmpty(bundle.file))
                {
                    if (!archives.Add(bundle.file))
                        throw new InvalidDataException("Duplicate TAR/LZ4 archive in catalog: " + bundle.file);
                    string path = Path.Combine(streamingRoot, bundle.file.Replace('/', Path.DirectorySeparatorChar));
                    if (!File.Exists(path) || new FileInfo(path).Length != bundle.size)
                        throw new InvalidDataException("Missing/incomplete TAR/LZ4 art bundle: " + path);
                    using (FileStream input = File.OpenRead(path))
                    using (SHA256 hash = SHA256.Create())
                    {
                        string actual = BitConverter.ToString(hash.ComputeHash(input)).Replace("-", "").ToLowerInvariant();
                        if (!string.Equals(actual, bundle.sha256, StringComparison.OrdinalIgnoreCase))
                            throw new InvalidDataException("TAR/LZ4 art checksum mismatch: " + path);
                    }
                }

                foreach (ArtRecord asset in bundle.assets)
                {
                    if (string.IsNullOrEmpty(asset.font)) continue;
                    string font = Path.Combine(assetsDirectory, "Resources",
                        asset.font.Replace('/', Path.DirectorySeparatorChar) + ".ttf");
                    if (!File.Exists(font) || !File.Exists(font + ".meta"))
                        throw new InvalidDataException("Loose TAR-catalog font is missing: " + font);
                }
            }

            foreach (string path in Directory.GetFiles(catalogRoot, "*", SearchOption.AllDirectories))
            {
                string name = Path.GetFileName(path);
                if (!name.Equals("catalog.json", StringComparison.OrdinalIgnoreCase) &&
                    !name.Equals("catalog.json.meta", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("Legacy native art remains beside TAR catalog: " + path);
            }
            return catalog;
        }

        private static void IndexAsset(string bundleName, string assetPath)
        {
            BundleAsset entry = new BundleAsset(bundleName, assetPath);
            string normalized = Normalize(assetPath);
            AddIndex(AssetsByPath, normalized, entry);
            if (normalized.StartsWith("gamedata/", StringComparison.OrdinalIgnoreCase))
                AddIndex(AssetsByPath, normalized.Substring("gamedata/".Length), entry);

            string name = GetLastSegment(normalized);
            if (!string.IsNullOrEmpty(name))
                AddIndex(AssetsByName, name, entry);
        }

        private static void AddIndex(Dictionary<string, List<BundleAsset>> index, string key, BundleAsset entry)
        {
            if (string.IsNullOrEmpty(key))
                return;
            List<BundleAsset> entries;
            if (!index.TryGetValue(key, out entries))
            {
                entries = new List<BundleAsset>();
                index.Add(key, entries);
            }
            entries.Add(entry);
        }

        private static IEnumerable<BundleAsset> FindCandidates(string resourcePath, Type assetType)
        {
            List<BundleAsset> result = new List<BundleAsset>();
            HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string key in GetPathKeys(resourcePath, assetType))
                AddCandidates(AssetsByPath, key, result, seen);

            string normalized = Normalize(resourcePath);
            // Location XML names and sprite member names are heavily reused between stages
            // (background_1, layer3, left, right, pixel_1...).  The old loose Resources
            // lookup was path-scoped; falling back globally by basename can silently turn a
            // missing Moon layer into a Dojo/raid layer.  Location content must therefore be
            // exact-path only.  Other legacy callers retain the recovered basename fallback.
            if (!IsStrictLocationPath(normalized))
            {
                AddCandidates(AssetsByName, GetLastSegment(normalized), result, seen);
                if (assetType == typeof(AudioClip))
                {
                    string withoutExtension = StripAudioExtension(normalized);
                    AddCandidates(AssetsByName, GetLastSegment(withoutExtension), result, seen);
                }
            }
            return result;
        }

        private static bool IsStrictLocationPath(string normalized)
        {
            return normalized.StartsWith("textures/locations/", StringComparison.OrdinalIgnoreCase) ||
                normalized.StartsWith("textures/location_effects/", StringComparison.OrdinalIgnoreCase);
        }

        private static IEnumerable<string> GetPathKeys(string resourcePath, Type assetType)
        {
            List<string> keys = new List<string>();
            string normalized = Normalize(resourcePath);
            AddUnique(keys, normalized);

            int resources = normalized.IndexOf("/resources/", StringComparison.OrdinalIgnoreCase);
            if (resources >= 0)
                AddUnique(keys, normalized.Substring(resources + "/resources/".Length));
            else if (normalized.StartsWith("resources/", StringComparison.OrdinalIgnoreCase))
                AddUnique(keys, normalized.Substring("resources/".Length));

            if (normalized.StartsWith("gamedata/", StringComparison.OrdinalIgnoreCase))
                AddUnique(keys, normalized.Substring("gamedata/".Length));

            if (assetType == typeof(AudioClip))
            {
                string[] snapshot = keys.ToArray();
                foreach (string key in snapshot)
                    AddUnique(keys, StripAudioExtension(key));
            }
            return keys;
        }

        private static void AddUnique(List<string> keys, string value)
        {
            if (!string.IsNullOrEmpty(value) && !keys.Contains(value))
                keys.Add(value);
        }

        private static string Normalize(string path)
        {
            return (path ?? string.Empty).Replace('\\', '/').Trim().TrimStart('/').ToLowerInvariant();
        }

        private static string GetLastSegment(string path)
        {
            int slash = path.LastIndexOf('/');
            return slash < 0 ? path : path.Substring(slash + 1);
        }

        private static string StripAudioExtension(string path)
        {
            string[] extensions = { ".ogg", ".wav", ".mp3", ".m4a" };
            foreach (string extension in extensions)
            {
                if (path.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                    return path.Substring(0, path.Length - extension.Length);
            }
            return path;
        }

        private static string StripXmlExtension(string path)
        {
            return path.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)
                ? path.Substring(0, path.Length - 4)
                : path;
        }

        private static void AddCandidates(Dictionary<string, List<BundleAsset>> index, string key,
            List<BundleAsset> result, HashSet<string> seen)
        {
            if (string.IsNullOrEmpty(key))
                return;
            List<BundleAsset> entries;
            if (!index.TryGetValue(key, out entries))
                return;
            foreach (BundleAsset entry in entries)
            {
                string identity = entry.BundleName + "\n" + entry.AssetPath;
                if (seen.Add(identity))
                    result.Add(entry);
            }
        }

        private static bool SafePath(string path)
        {
            if (string.IsNullOrEmpty(path) || path.Contains("\\") || path.Contains(":")) return false;
            foreach (string part in path.Split('/'))
                if (part == "" || part == "." || part == "..") return false;
            return true;
        }

        private static bool IsSha256(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length != 64) return false;
            foreach (char c in value)
                if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')))
                    return false;
            return true;
        }

        private sealed class NativeAssetGroup
        {
            private readonly Dictionary<string, ArtRecord> assets = new Dictionary<string, ArtRecord>(StringComparer.Ordinal);
            private readonly BundleRecord record;
            private TarAssetBundle tarBundle;
            public NativeAssetGroup(BundleRecord group)
            {
                record = group;
                foreach (ArtRecord asset in group.assets) assets.Add(asset.address, asset);
            }
            public T LoadAsset<T>(string address) where T : UnityEngine.Object
            {
                T[] results = LoadAssetWithSubAssets<T>(address);
                return results == null || results.Length == 0 ? null : results[0];
            }
            public T[] LoadAssetWithSubAssets<T>(string address) where T : UnityEngine.Object
            {
                ArtRecord asset;
                if (!assets.TryGetValue(address, out asset)) return null;
                if (!string.IsNullOrEmpty(record.file) &&
                    typeof(T) != typeof(Font) && typeof(T) != typeof(Material))
                {
                    if (tarBundle == null) tarBundle = new TarAssetBundle(record);
                    T[] tarAssets = tarBundle.LoadAssetWithSubAssets<T>(address);
                    if (tarAssets != null && tarAssets.Length != 0) return tarAssets;
                }
                string path = typeof(T) == typeof(Sprite) ? asset.sprites :
                    typeof(T) == typeof(Texture2D) ? asset.texture :
                    typeof(T) == typeof(AudioClip) ? asset.audio : typeof(T) == typeof(Font) ? asset.font : null;
                if (!string.IsNullOrEmpty(path)) return Resources.LoadAll<T>(path);
                if (typeof(T) == typeof(Material) && !string.IsNullOrEmpty(asset.font))
                {
                    Font font = Resources.Load<Font>(asset.font);
                    if (font != null) return new[] { font.material as T };
                }
                return null;
            }

            public string LoadText(string address, string type)
            {
                ArtRecord asset;
                if (!assets.TryGetValue(address, out asset) || string.IsNullOrEmpty(record.file))
                    return null;
                if (tarBundle == null) tarBundle = new TarAssetBundle(record);
                return tarBundle.LoadText(address, type);
            }
        }

        private static NativeAssetGroup OpenBundle(string name)
        {
            NativeAssetGroup group;
            return OpenBundles.TryGetValue(name, out group) ? group : null;
        }
    }
}
