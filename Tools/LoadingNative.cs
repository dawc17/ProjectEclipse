using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Eclipse.Content.TarAssets;
using Eclipse.Modding;
using UnityEditor;
using UnityEngine;

// Only the unused compressed-cache constructor is stubbed. Native texture, sprite,
// disposal, metadata and FX code execute directly from the production sources.
namespace Eclipse.Content
{
    public static class PackagedArtCatalog
    {
        public sealed class BundleRecord { public string name, namespaceId; }
    }
}
namespace Eclipse.Content.TarAssets
{
    static class Lz4BundleCache
    {
        public static string OpenTar(Eclipse.Content.PackagedArtCatalog.BundleRecord record) => throw new NotSupportedException();
    }
}
public static class LoadingNative
{
    static int checks;
    static void Check(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }
    static int CacheCount(TarAssetBundle bundle, string field) => ((System.Collections.IDictionary)typeof(TarAssetBundle).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(bundle)).Count;
    static void Tar(string folder)
    {
        string path = Path.Combine(folder, "sprites.tar");
        var texture = new Texture2D(4, 4); byte[] png = texture.EncodeToPNG(); UnityEngine.Object.DestroyImmediate(texture);
        using (var output = File.Create(path)) using (var writer = new TarWriter(output))
        {
            writer.AddFile("image.png", png);
            foreach (string name in new[] { "a", "b", "c" }) writer.AddFile(name + ".meta", Encoding.UTF8.GetBytes("type=sprite\nnamespace=core\naddress=textures/atlas\nname=" + name + "\ntexture=image.png\nrect=0,0,4,4\n"));
            writer.AddFile("model.xml", Encoding.UTF8.GetBytes("<Model/>"));
            writer.AddFile("model.meta", Encoding.UTF8.GetBytes("type=model\naddress=gamedata/models/test\nfile=model.xml\n"));
            writer.AddFile("empty.xml", Array.Empty<byte>());
            writer.AddFile("empty.meta", Encoding.UTF8.GetBytes("type=model\naddress=gamedata/models/empty\nfile=empty.xml\n"));
        }
        using (var bundle = new TarAssetBundle(new Eclipse.Content.PackagedArtCatalog.BundleRecord { name = "fixture", namespaceId = "core" }, path))
        {
            Check(bundle.ContainsText("gamedata/models/test", "model") && !bundle.ContainsText("gamedata/models/empty", "model"), "Model presence changed");
            Check(CacheCount(bundle, "_sprites") == 0 && CacheCount(bundle, "_textures") == 0, "Presence query decoded artwork");
            Sprite first = bundle.LoadAsset<Sprite>("textures/atlas");
            Check(first != null && first.name == "a", "Single sprite selection changed");
            Check(CacheCount(bundle, "_sprites") == 1 && CacheCount(bundle, "_textures") == 1, "Single sprite request loaded atlas siblings");
            Check(ReferenceEquals(first, bundle.LoadAsset<Sprite>("textures/atlas")), "Sprite cache identity changed");
            Sprite[] all = bundle.LoadAssetWithSubAssets<Sprite>("textures/atlas");
            Check(all.Length == 3 && ReferenceEquals(first, all[0]) && all[1].name == "b" && all[2].name == "c", "Bulk sprite order or cache identity changed");
            Check(CacheCount(bundle, "_sprites") == 3 && CacheCount(bundle, "_textures") == 1, "Bulk request did not share atlas texture");
            Check(bundle.LoadAsset<AudioClip>("textures/atlas") == null && bundle.LoadAsset<Sprite>("missing") == null, "Wrong-type or missing asset selection changed");
        }
    }
    static ModFxDefinition Effect(string setting, string[] match = null, string[] exclude = null) => new ModFxDefinition("fixture.fx", ModId.Parse("fixture"), ModFxKind.Particles, setting,
        match, exclude, ModFxScenes.Everywhere, ModFxPlacement.Node, ModFxFighters.Both, ModFxBlend.Alpha, null, null, null, false, null, new Dictionary<string, float>());
    static int Count() { int count = 0; foreach (var effect in ModVisuals.EnumerateActiveFx(ModFxKind.Particles)) count++; return count; }
    static void Fx()
    {
        var catalog = new ModContentCatalog(); var view = catalog.Effects;
        var toggle = new ModSettingToggle("loading-fixture-toggle", ModId.Parse("fixture"), "Fixture", "", true);
        var toggles = (List<ModSettingToggle>)typeof(ModContentCatalog).GetField("_settings", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(catalog);
        toggles.Add(toggle); var fx = Effect(toggle.Name, new[] { "dojo" }, new[] { "night" }); catalog.CommitFx(new[] { fx });
        Check(view.Count == 1 && ReferenceEquals(view, catalog.Effects), "Cached effect view stopped reflecting registrations");
        ModVisuals.Bind(catalog);
        try
        {
            ModSettingsStore.Set(toggle, true); Check(Count() == 1, "Enabled effect disappeared");
            ModSettingsStore.Set(toggle, false); Check(Count() == 0, "Setting change did not disable effect immediately");
            ModSettingsStore.Set(toggle, true); Check(Count() == 1, "Setting change did not enable effect immediately");
            Check(fx.MatchesLocation("dojo_01") && !fx.MatchesLocation("dojo_night_02") && !fx.MatchesLocation(null) && fx.MatchesLocation("dojo_01"), "Cached location matching changed");
            Count(); fx.MatchesLocation("dojo_01"); long before = GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < 10000; i++) { Count(); fx.MatchesLocation("dojo_01"); }
            if (Program.AllocationCounterAvailable())
            {
                before = GC.GetAllocatedBytesForCurrentThread();
                for (int i = 0; i < 10000; i++) { Count(); fx.MatchesLocation("dojo_01"); }
                Check(GC.GetAllocatedBytesForCurrentThread() == before, "Steady FX queries allocate");
            }
            else Debug.Log("[LoadingNative] Allocation counter unavailable; steady FX allocation check skipped.");
            var replacement = new ModContentCatalog(); replacement.CommitFx(new[] { Effect(null) }); ModVisuals.Bind(replacement);
            Check(Count() == 1, "Catalog rebind changed selection");
            var iterator = ModVisuals.EnumerateActiveFx(ModFxKind.Particles).GetEnumerator(); iterator.MoveNext();
            Check(ReferenceEquals(iterator.Current, replacement.Effects[0]), "Catalog rebind retained old definition with same name");
        }
        finally { PlayerPrefs.DeleteKey("Eclipse.ModSetting." + toggle.Name); ModVisuals.Bind(null); }
    }
    public static void RunEditor()
    {
        try
        {
            string fixture = Directory.GetParent(Application.dataPath).FullName;
            string root = File.ReadAllText(Path.Combine(fixture, "source-root.txt"));
            typeof(Program).GetMethod("Main", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { new[] { root } });
            Tar(fixture); Fx(); Debug.Log("[LoadingNative] PASS: " + checks + " Unity TAR/FX checks; managed loading checks also passed."); EditorApplication.Exit(0);
        }
        catch (Exception exception) { Debug.LogError("[LoadingNative] FAIL: " + exception); EditorApplication.Exit(1); }
    }
}
