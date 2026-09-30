using Eclipse.Content;

namespace Eclipse.Modding
{
    public sealed class CoreAssetProvider : IAssetProvider, IRuntimeAssetProvider
    {
        private static readonly ModId Core = ModId.Parse("core");

        public ModId Namespace => Core;

        // Load profiling (ModRuntime.StartGameContent reports and resets these): how many core
        // assets mods described, the time spent, and how many had to be loaded whole to learn
        // their kind.
        internal static int DescribeCount, DescribeLoads, DescribeLooseLoads;
        internal static readonly System.Diagnostics.Stopwatch DescribeTime = new System.Diagnostics.Stopwatch(),
            DescribeLoadTime = new System.Diagnostics.Stopwatch(), DescribeLooseTime = new System.Diagnostics.Stopwatch();

        public bool TryDescribe(AssetId id, out AssetMetadata metadata)
        {
            DescribeCount++;
            DescribeTime.Start();
            try { return Describe(id, out metadata); }
            finally { DescribeTime.Stop(); }
        }

        private static bool Describe(AssetId id, out AssetMetadata metadata)
        {
            metadata = null;
            if (id.Namespace != Core) return false;

            AssetKind kind;
            if (id.Path.StartsWith("gamedata/models/", System.StringComparison.Ordinal) &&
                PackagedArtCatalog.ContainsExactAddress(id.Path))
                kind = AssetKind.Model;
            else if (PackagedArtCatalog.ContainsSprite(id.Path))
                kind = AssetKind.Sprite;
            else if (PackagedArtCatalog.ContainsExactAddress(id.Path))
            {
                DescribeLoads++;
                DescribeLoadTime.Start();
                kind = PackagedArtCatalog.Load<UnityEngine.AudioClip>(id.Path) != null ? AssetKind.Audio :
                    PackagedArtCatalog.Load<UnityEngine.Texture2D>(id.Path) != null ? AssetKind.Texture : AssetKind.Unknown;
                DescribeLoadTime.Stop();
            }
            else if (LooseUiSpriteExists(id.Path))
                kind = AssetKind.Sprite;
            else
                return false;
            metadata = new AssetMetadata(id, kind, AssetSourceKind.Core, string.Empty, -1,
                "PackagedArtCatalog:" + id.Path);
            return true;
        }

        public bool TryLoadUnityAsset<T>(AssetId id, out T asset) where T : UnityEngine.Object
        {
            asset = null;
            if (id.Namespace != Core) return false;
            asset = PackagedArtCatalog.Load<T>(id.Path);
            if (asset == null && typeof(T) == typeof(UnityEngine.Sprite))
                asset = LoadLooseUiSprite(id.Path) as T;
            return asset != null;
        }

        private static bool LooseUiSpriteExists(string path)
        {
            DescribeLooseLoads++;
            DescribeLooseTime.Start();
            try { return LoadLooseUiSprite(path) != null; }
            finally { DescribeLooseTime.Stop(); }
        }

        // Some UI art remains in Resources rather than the packaged art catalog.
        // Resolve only an existing sprite at an exact UI resource path. The menu loader
        // panels are aliases of the loading panels (see Eclipse.UI.LoaderArt).
        private static UnityEngine.Sprite LoadLooseUiSprite(string path)
        {
            path = Eclipse.UI.LoaderArt.ResolveAlias(path);
            return (path.StartsWith("ui/", System.StringComparison.Ordinal) ||
                    path.StartsWith("textures/logos/", System.StringComparison.Ordinal))
                ? UnityEngine.Resources.Load<UnityEngine.Sprite>(path) : null;
        }

        public bool TryLoadUnityAssets<T>(AssetId id, out T[] assets) where T : UnityEngine.Object
        {
            assets = null;
            if (id.Namespace != Core) return false;
            assets = PackagedArtCatalog.LoadWithSubAssets<T>(id.Path);
            return assets != null;
        }

        public bool TryLoadModelText(AssetId id, out string text)
        {
            text = null;
            if (id.Namespace != Core || !id.Path.StartsWith("gamedata/models/", System.StringComparison.Ordinal))
                return false;
            text = PackagedArtCatalog.LoadModelText(id.Path);
            return text != null;
        }
    }
}
