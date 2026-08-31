using Eclipse.Content;

namespace Eclipse.Modding
{
    public sealed class CoreAssetProvider : IAssetProvider, IRuntimeAssetProvider
    {
        private static readonly ModId Core = ModId.Parse("core");

        public ModId Namespace => Core;

        public bool TryDescribe(AssetId id, out AssetMetadata metadata)
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
                kind = AssetKind.Unknown;
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
            return asset != null;
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
