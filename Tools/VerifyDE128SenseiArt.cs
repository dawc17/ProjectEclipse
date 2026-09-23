// Run with: unity command eval_file --file Tools/VerifyDE128SenseiArt.cs --json
// Read-only native decode of DE128's shipped Sensei sprites plus every core
// portrait the pending story references. Does not open UI, fights or saves.
var mod = System.Linq.Enumerable.Single(Eclipse.Modding.ModDiscovery.DiscoverLoose("Mods").Mods, m => m.Id.ToString() == "de128");
var core = new Eclipse.Modding.CoreAssetProvider();
var resolver = new Eclipse.Modding.AssetResolver(new Eclipse.Modding.IAssetProvider[] { core, new Eclipse.Modding.LooseModProvider(mod) });
var rows = new System.Collections.Generic.List<object>();
var owned = new[] { "boss_hermit_young", "boss_butcher_young", "boss_wasp_young", "boss_widow_young", "boss_shogun_young", "character_pirate", "character_sensei" };
var previews = new[] { "preview_pvp_stone_dragon", "preview_pvp_village", "preview_pvp_ships", "preview_pvp_flooded_village", "preview_pvp_magic_rocks" };
using (var loader = new Eclipse.Modding.ModAssetLoader(resolver))
{
    foreach (var name in System.Linq.Enumerable.Concat(owned, previews))
    {
        var sprite = loader.LoadSprite(Eclipse.Modding.AssetId.Parse("de128:sprites/sensei/" + name));
        bool portrait = !name.StartsWith("preview_");
        if (sprite == null || sprite.vertices.Length < 3 || sprite.texture == null)
            throw new Exception("Shipped sprite did not decode: " + name);
        if (portrait ? (sprite.rect.width != 512 || sprite.rect.height != 512) : (sprite.rect.width != 484 || sprite.rect.height < 273 || sprite.rect.height > 274))
            throw new Exception("Unexpected shipped sprite size: " + name + " " + sprite.rect);
        var center = sprite.texture.GetPixel((int)sprite.rect.width / 2, (int)sprite.rect.height / 2);
        rows.Add(new { id = "de128:sprites/sensei/" + name, sprite.rect.width, sprite.rect.height, vertices = sprite.vertices.Length, centerAlpha = center.a });
    }
}
// Core portraits named by the pending Lua data (portrait/avatar fields).
var names = new System.Collections.Generic.SortedSet<string>();
foreach (var file in new[] { "sensei_entry_data", "sensei_victory_data", "sensei_guard_opponents", "sensei_boss_opponents", "sensei_act_one_opponents" })
    foreach (System.Text.RegularExpressions.Match match in System.Text.RegularExpressions.Regex.Matches(
        System.IO.File.ReadAllText("Mods/de128/scripts/content/" + file + ".lua"), "(?:portrait|avatar) = (?:art\\.avatar\\()?\"([A-Za-z_]+)\""))
        names.Add(match.Groups[1].Value);
names.Add("character_sensei");
int corePortraits = 0;
foreach (var name in names)
{
    if (System.Array.IndexOf(owned, name) >= 0) continue;
    UnityEngine.Sprite sprite;
    if (!core.TryLoadUnityAsset<UnityEngine.Sprite>(Eclipse.Modding.AssetId.Parse("core:ui/users/" + name.ToLowerInvariant()), out sprite) || sprite == null || sprite.vertices.Length < 3)
        throw new Exception("Core portrait missing: " + name);
    corePortraits++;
}
if (rows.Count != 12 || corePortraits < 14) throw new Exception("Unexpected coverage: " + rows.Count + " shipped, " + corePortraits + " core");
return new { shipped = rows, corePortraits, coreNames = names };
