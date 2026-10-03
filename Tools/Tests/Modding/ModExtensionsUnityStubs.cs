// Controlled asset-host and unused artwork/scroll paths. Actual text view, font,
// Lua, script sessions, render frames and state serialization are production/native.
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace Eclipse.Modding
{
    public sealed class ModHost
    {
        public IReadOnlyList<ModDescriptor> EnabledMods { get; }
        public AssetResolver Assets { get; }
        public ModHost(IEnumerable<ModDescriptor> mods)
        {
            var order=DependencyResolver.Resolve(mods.ToArray(),ModPlatformVersions.Core);
            if(order.Diagnostics.Any(d=>d.Severity==ModDiagnosticSeverity.Error)) throw new Exception(string.Join("; ",order.Diagnostics));
            EnabledMods=order.OrderedMods;
            Assets=new AssetResolver(EnabledMods.Select(mod=>(IAssetProvider)new LooseModProvider(mod)));
        }
    }
    public static partial class ModRuntime { public static bool IsInitialized; public static FixtureHost Host; }
    public sealed class FixtureHost { public FixtureAssets TypedAssets; }
    public sealed class FixtureAssets { public Sprite LoadSprite(AssetId id)=>null; }
}
namespace Nekki.SF2.GUI
{
    public static class ResolutionImage { public static Sprite GetSprite(string path,string name)=>null; }
    public sealed class SFScrollRect : ScrollRect
    {
        public enum MDMLKCMBBPA { Elastic }
        public void set_viewport(RectTransform value)=>viewport=value;
        public void set_content(RectTransform value)=>content=value;
        public void set_horizontal(bool value)=>horizontal=value;
        public void set_vertical(bool value)=>vertical=value;
        public void set_movementType(MDMLKCMBBPA value)=>movementType=MovementType.Elastic;
        public void set_elasticity(float value)=>elasticity=value;
        public void set_inertia(bool value)=>inertia=value;
        public void set_decelerationRate(float value)=>decelerationRate=value;
        public void set_scrollSensitivity(float value)=>scrollSensitivity=value;
        public RectTransform get_content()=>content;
        public RectTransform get_viewport()=>viewport;
        public bool get_vertical()=>vertical;
    }
}
namespace Eclipse.UI
{
    public static class DesktopScrollbars { public static void Attach(Nekki.SF2.GUI.SFScrollRect scroll,Action stop) { } }
}
