using UnityEngine;

// The art-only Unity fixture has no gameplay/mod runtime. The production
// PackagedArtCatalog still runs; no core-asset replacement is installed.
namespace Eclipse.Modding
{
    public readonly struct AssetId { }

    public sealed class ArtFixtureTypedAssets
    {
        public T LoadUnityAsset<T>(AssetId id) where T : Object { return null; }
        public T[] LoadUnityAssets<T>(AssetId id) where T : Object { return null; }
        public string LoadModelText(AssetId id) { return null; }
    }

    public sealed class ArtFixtureHost
    {
        public ArtFixtureTypedAssets TypedAssets { get; } = new ArtFixtureTypedAssets();
    }

    public static class ModRuntime
    {
        public static ArtFixtureHost Host { get; } = new ArtFixtureHost();
        public static bool TryResolveCoreReplacement(string reference, out AssetId replacement)
        { replacement = default(AssetId); return false; }
        public static bool TryLoadCoreSpriteReplacement(string address, string name, out Sprite sprite)
        { sprite = null; return false; }
    }
}
