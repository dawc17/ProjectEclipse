// Only the game/session lookup, persistent report location and key events are
// controlled. Lua execution, native overlay Update/IMGUI, screenshot and report
// filesystem write are production/native.
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Eclipse.Modding
{
    public sealed class ModHost
    {
        public IReadOnlyList<ModDescriptor> EnabledMods { get; }
        public AssetResolver Assets { get; }
        public ModHost(IEnumerable<ModDescriptor> mods)
        {
            var order = DependencyResolver.Resolve(mods.ToArray(), ModPlatformVersions.Core);
            if (order.HasErrors) throw new Exception(string.Join("; ", order.Diagnostics));
            EnabledMods = order.OrderedMods;
            Assets = new AssetResolver(EnabledMods.Select(mod => (IAssetProvider)new LooseModProvider(mod)));
        }
    }
    public static class ModRuntime { public static ModScriptSession Scripts { get; set; } }
}
namespace Eclipse.Input
{
    public static class EclipseInput
    {
        public static readonly HashSet<KeyCode> Keys = new HashSet<KeyCode>();
        public static bool GetKeyDown(KeyCode key) => Keys.Remove(key);
    }
}
namespace Eclipse.Runtime
{
    public static class EditorPlayModeContext
    { public static string PersistentDataPath => System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.dataPath), "ReportData"); }
}
public static class Fight { public static object GetCurrentFight() => null; }
public static class SF2DisplayFrameRate
{
    public static int MaxFrameRate => 60;
    public static bool InterpolationEnabled => true;
    public static bool MotionBlurEnabled => false;
}
