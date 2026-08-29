using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Eclipse.Content
{
    // Temporary art source while the modern 2.41.9 presentation assets are being
    // migrated into the project. Gameplay/config TextAssets are deliberately not
    // supported here: Assets/vanillaXml remains the only modern gameplay data source.
    public static class ResearchArtBundleOverride
    {
        public const string RootEnvironmentVariable = "SF2_RESEARCH_BUNDLES";

        private sealed class BundleAsset
        {
            public readonly string BundlePath;
            public readonly string AssetPath;

            public BundleAsset(string bundlePath, string assetPath)
            {
                BundlePath = bundlePath;
                AssetPath = assetPath;
            }
        }

        private static readonly Dictionary<string, List<BundleAsset>> AssetsByPath =
            new Dictionary<string, List<BundleAsset>>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, List<BundleAsset>> AssetsByName =
            new Dictionary<string, List<BundleAsset>>(StringComparer.OrdinalIgnoreCase);
        private static readonly HashSet<string> FailedBundles =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private static bool _indexed;
        private static AssetBundle _activeBundle;
        private static string _activeBundlePath;

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
            foreach (BundleAsset entry in FindCandidates(resourcePath, typeof(T)))
            {
                AssetBundle bundle = OpenBundle(entry.BundlePath);
                if (bundle == null)
                    continue;
                T asset = bundle.LoadAsset<T>(entry.AssetPath);
                if (asset != null)
                    return asset;
            }
            return null;
        }

        public static T[] LoadWithSubAssets<T>(string resourcePath) where T : UnityEngine.Object
        {
            if (!IsSupportedAssetType(typeof(T)) || string.IsNullOrEmpty(resourcePath))
                return null;

            EnsureIndex();
            foreach (BundleAsset entry in FindCandidates(resourcePath, typeof(T)))
            {
                AssetBundle bundle = OpenBundle(entry.BundlePath);
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
            _indexed = true;

            string root = FindBundleRoot();
            if (string.IsNullOrEmpty(root))
            {
                Debug.LogWarning("[ResearchArt] ResearchSources/bundles was not found; using project Resources fallbacks.");
                return;
            }

            int bundleCount = 0;
            int assetCount = 0;
            string[] files = Directory.GetFiles(root, "*", SearchOption.TopDirectoryOnly);
            Array.Sort(files, StringComparer.OrdinalIgnoreCase);
            foreach (string file in files)
            {
                if (string.Equals(Path.GetExtension(file), ".mp4", StringComparison.OrdinalIgnoreCase))
                    continue;

                AssetBundle bundle = AssetBundle.LoadFromFile(file);
                if (bundle == null)
                {
                    Debug.LogWarning("[ResearchArt] Could not inspect bundle '" + file + "'.");
                    continue;
                }
                try
                {
                    string[] assetNames = bundle.GetAllAssetNames();
                    foreach (string assetName in assetNames)
                    {
                        IndexAsset(file, assetName);
                        assetCount++;
                    }
                    bundleCount++;
                }
                finally
                {
                    bundle.Unload(true);
                }
            }

            Debug.Log("[ResearchArt] indexed " + assetCount + " asset address(es) from " + bundleCount +
                " research bundle(s); only sprites, textures, audio, fonts and materials may load from this source");
        }

        private static string FindBundleRoot()
        {
            string configured = Environment.GetEnvironmentVariable(RootEnvironmentVariable);
            if (!string.IsNullOrEmpty(configured))
            {
                string fullConfigured = Path.GetFullPath(configured);
                if (Directory.Exists(fullConfigured))
                    return fullConfigured;
            }

            string applicationRoot = Path.GetDirectoryName(Application.dataPath);
            if (string.IsNullOrEmpty(applicationRoot))
                return null;
            string defaultRoot = Path.GetFullPath(Path.Combine(applicationRoot, "ResearchSources", "bundles"));
            return Directory.Exists(defaultRoot) ? defaultRoot : null;
        }

        private static void IndexAsset(string bundlePath, string assetPath)
        {
            BundleAsset entry = new BundleAsset(bundlePath, assetPath);
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
            AddCandidates(AssetsByName, GetLastSegment(normalized), result, seen);
            if (assetType == typeof(AudioClip))
            {
                string withoutExtension = StripAudioExtension(normalized);
                AddCandidates(AssetsByName, GetLastSegment(withoutExtension), result, seen);
            }
            return result;
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
                string identity = entry.BundlePath + "\n" + entry.AssetPath;
                if (seen.Add(identity))
                    result.Add(entry);
            }
        }

        private static AssetBundle OpenBundle(string path)
        {
            if (_activeBundle != null && string.Equals(_activeBundlePath, path, StringComparison.OrdinalIgnoreCase))
                return _activeBundle;

            if (_activeBundle != null)
            {
                _activeBundle.Unload(false);
                _activeBundle = null;
                _activeBundlePath = null;
            }

            if (FailedBundles.Contains(path))
                return null;
            _activeBundle = AssetBundle.LoadFromFile(path);
            if (_activeBundle == null)
            {
                FailedBundles.Add(path);
                Debug.LogWarning("[ResearchArt] Could not load bundle '" + path + "'.");
                return null;
            }
            _activeBundlePath = path;
            return _activeBundle;
        }
    }
}
