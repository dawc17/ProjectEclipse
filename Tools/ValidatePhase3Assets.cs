using System;
using System.IO;
using System.Linq;
using Eclipse.Modding;
using Eclipse.Content;
using UnityEngine;
using UnityEditor;

public static class ValidatePhase3Assets
{
    public static void Run()
    {
        try
        {
            const string target = "UI/Skills/SkillsEnch02.EnchantmentFrenzy";
            Sprite original = PackagedArtCatalog.Load<Sprite>(target);
            if (original == null) throw new Exception("Core target missing.");
            ModRuntime.Initialize(Path.Combine(Application.dataPath, "Phase3Mods"));
            using (var scripts = ModRuntime.Host.StartScripts(new MoonSharpScriptRuntime(), null,
                c => CoreContentImporter.ImportForgeEconomicProfiles(c, new[] { "Simple" })))
            {
                if (scripts.HasErrors || scripts.ActiveMods.Count != 1) throw new Exception(scripts.FormatReport());
                var declaration = scripts.Content.AssetReplacements.Single();
                Sprite direct = ResourcesAndBundles.Load<Sprite>(target);
                Sprite qualified = ResourcesAndBundles.Load<Sprite>("core:" + target);
                Sprite owned = ModRuntime.Host.TypedAssets.LoadSprite(declaration.Replacement);
                if (owned == null || direct != owned || qualified != owned || owned == original)
                    throw new Exception("Core and qualified loads did not resolve to the mod sprite.");
                Sprite[] atlas = PackagedArtCatalog.LoadWithSubAssets<Sprite>("UI/Skills/SkillsEnch02");
                Sprite member = atlas.FirstOrDefault(s => s.name == "EnchantmentFrenzy" || s.name == "SkillsEnch02.EnchantmentFrenzy");
                if (member == null || member.texture != owned.texture)
                    throw new Exception("Atlas member replacement missing or lost its legacy name.");
                Sprite cached = AtlasCache.GetSpriteFromAtlas("UI/Skills/SkillsEnch02", "SkillsEnch02.EnchantmentFrenzy");
                if (cached == null || cached.texture != owned.texture) throw new Exception("Atlas cache bypassed replacement.");
                ModRuntime.Host.Assets.SetReplacements(null);
                if (PackagedArtCatalog.Load<Sprite>(target) != original) throw new Exception("Base sprite not restored after unmount.");
                Debug.Log("[Phase3Assets] PASS: actual Unity PNG import, core/qualified/atlas/cache replacement, member identity and restoration.");
            }
            ModRuntime.Shutdown();
            EditorApplication.Exit(0);
        }
        catch (Exception e) { Debug.LogException(e); EditorApplication.Exit(1); }
    }
}
