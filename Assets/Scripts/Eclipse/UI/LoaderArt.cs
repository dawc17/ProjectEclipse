using Nekki.SF2.GUI;
using UnityEngine;

namespace Eclipse.UI
{
    // Background art of the native module loader. Moving between the menu screens (dojo,
    // shop, profile, map) can show its own art through the replaceable core sprites
    // ui/fullscreen/menu_loading_left.img and _right.img. Without a replacement these alias
    // the normal loading panels, so the base game looks exactly as before.
    public static class LoaderArt
    {
        public const string MenuLeft = "menu_loading_left.img", MenuRight = "menu_loading_right.img";

        public static bool IsMenu(ScreenType screen)
        {
            return screen == ScreenType.ModuleDojo || screen == ScreenType.ModuleShop ||
                   screen == ScreenType.ModuleProfile || screen == ScreenType.ModuleMap;
        }

        // Core alias used by CoreAssetProvider: the menu panels default to the loading panels.
        public static string ResolveAlias(string path)
        {
            if (path == "ui/fullscreen/" + MenuLeft) return "ui/fullscreen/loading_left.img";
            if (path == "ui/fullscreen/" + MenuRight) return "ui/fullscreen/loading_right.img";
            return path;
        }

        // Called by LoaderScene after it chose the loading panels.
        public static void ApplyMenuSplash(GameObject panels, ScreenType previous, ScreenType next)
        {
            if (panels == null || !IsMenu(previous) || !IsMenu(next)) return;
            if (!Replaced(MenuLeft) && !Replaced(MenuRight)) return;
            foreach (var image in panels.GetComponentsInChildren<ResolutionImage>(true))
            {
                string name = image.get_SpriteName();
                if (name == "loading_left.img") image.set_SpriteName(MenuLeft);
                else if (name == "loading_right.img") image.set_SpriteName(MenuRight);
            }
        }

        public static bool Replaced(string spriteName)
        {
            try { return Eclipse.Modding.ModRuntime.TryResolveCoreReplacement("ui/fullscreen/" + spriteName.ToLowerInvariant(), out _); }
            catch (System.Exception) { return false; }
        }
    }
}
