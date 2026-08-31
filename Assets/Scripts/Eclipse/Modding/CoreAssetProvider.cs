using Eclipse.Content;

namespace Eclipse.Modding
{
    public sealed class CoreAssetProvider : IAssetProvider
    {
        private static readonly ModId Core = ModId.Parse("core");

        public ModId Namespace => Core;

        public bool TryDescribe(AssetId id, out AssetMetadata metadata)
        {
            metadata = null;
            if (id.Namespace != Core || !PackagedArtCatalog.ContainsExactAddress(id.Path)) return false;

            AssetKind kind = id.Path.StartsWith("gamedata/models/", System.StringComparison.Ordinal)
                ? AssetKind.Model
                : AssetKind.Unknown;
            metadata = new AssetMetadata(id, kind, AssetSourceKind.Core, string.Empty, -1,
                "PackagedArtCatalog:" + id.Path);
            return true;
        }
    }
}
