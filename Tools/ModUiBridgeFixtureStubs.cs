// The isolated Unity fixture controls native shell/device signals. UI bridge,
// coordinator, view and scope code are production sources, not copies.
using UnityEngine;
namespace Eclipse.UI {
    public static class TitleScreen { public static bool IsOpen; }
    public static class GameSessionRestart { public static bool IsRestarting; }
}
public static class GamePad {
    public enum LCNPGEANNDP { Dpad, LeftStick }
    public enum GGAKHLLMPMM { One }
    public enum PFENLAPGKFM { A, B }
    public static Vector2 CNNMBBLLGNE(LCNPGEANNDP source, GGAKHLLMPMM player, bool raw) => Vector2.zero;
    public static bool NFCGBMHPKMA(PFENLAPGKFM button, GGAKHLLMPMM player) => false;
    public static bool JAHEECFCLHN(PFENLAPGKFM button, GGAKHLLMPMM player) => false;
}
public static class ResourcesAndBundles
{
    public static T Load<T>(string path) where T : UnityEngine.Object => UnityEngine.Resources.Load<T>(path);
}
public static class AtlasCache
{
    public static UnityEngine.Sprite GetSpriteFromAtlas(string path,string name) => null;
}
