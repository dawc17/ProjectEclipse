using System;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

internal static class DE128EquipmentTests
{
    internal static readonly string[] Ids = { "armor/dragon_carapace", "armor/old_legionnaire_armour", "armor/samurai_armour",
        "helm/gabled_helm", "helm/dragon_helm", "ranged/dragon_boomerangs", "magic/dragons_breath", "magic/lightning_arc" };
    internal static readonly string[] Names = { "ARMOR_C2_Z5_DRAGON", "ARMOR_OLD_LEGIONER", "ARMOR_BIG_SHOGUN_OLD",
        "HELM_GABLED_OLD", "HELM_C2_Z5_DRAGON", "RANGED_C2_Z5_DRAGON_BOOMERANG", "MAGIC_C2_Z5_DRAGON_EARTHQUAKE", "MAGIC_LIGHTNING" };

    internal static void Run(ModDescriptor mod, ModContentCatalog catalog, string repository, Action<bool, string> check)
    {
        var archive = new XmlDocument(); archive.Load(Path.Combine(repository, "Assets/DExml/list.xml"));
        var vanilla = new XmlDocument(); vanilla.Load(Path.Combine(repository, "Assets/vanillaXml/list.xml"));
        var language = new XmlDocument(); language.Load(Path.Combine(repository, "Assets/DExml/localizations/eng.xml"));
        check(catalog.Armors.Count(item => !item.IsCore) == 3 && catalog.Helms.Count(item => !item.IsCore) == 2 &&
            catalog.Ranged.Count(item => !item.IsCore) == 1 && catalog.Magic.Count(item => !item.IsCore) == 2,
            "Restored equipment inventory changed or unfinished/NPC equipment leaked.");
        for (int i = 0; i < Ids.Length; i++)
        {
            var row = (XmlElement)archive.SelectSingleNode("/List/Items/Item[@Name='" + Names[i] + "']");
            check(row != null && row.GetAttribute("ShopHide") != "1" &&
                vanilla.SelectSingleNode("/List/Items/Item[@Name='" + Names[i] + "']") == null, "Not a missing archived shop item.");
            check(catalog.TryGetItem(DefinitionId.Parse("de128:items/" + Ids[i]), out var item) && !item.IsCore,
                "Restored equipment missing: " + Ids[i]);
            check(item.Icon.ToString() == ("core:ui/items/" + row.GetAttribute("Image")).ToLowerInvariant() &&
                item.Model.ToString() == ("core:gamedata/models/" + row.GetAttribute("Model")).ToLowerInvariant(), "Restored art drift.");
            check(catalog.TryGetLocalization(item.DisplayName, out var title) && title.GetOrEnglish("eng") ==
                language.SelectSingleNode("//Word[@Title='" + Names[i] + "']").InnerText, "Archived English name drift.");
            if (item is RangedDefinition ranged) check(ranged.SubType == row.GetAttribute("SubType"), "Ranged subtype drift.");
            if (item is MagicDefinition magic) check(magic.SubType == row.GetAttribute("SubType"), "Magic subtype drift.");
            if (Names[i] == "ARMOR_BIG_SHOGUN_OLD")
                check(item.InitialStats != null && item.InitialStats.Values.Count == 1 && item.InitialStats.Values["HeadDefense"] == 914 &&
                    !row.HasAttribute("BodyDefense") && !row.HasAttribute("UnarmedDamage"), "Samurai Armour silently normalized to ordinary armor.");
            else check(item.InitialStats == null, "Unexpected initial stat override.");
            var listing = catalog.ShopListings.Single(value => value.Item == item.Id);
            check(listing.Level == int.Parse(row.GetAttribute("Level")) && listing.Price.Currency == ModPriceCurrency.Gems &&
                listing.Price.Amount == long.Parse(row.GetAttribute("BonusPrice")), "Restored equipment listing drift.");
            var policy = catalog.ItemAvailabilityPolicies.Single(value => value.Item == item.Id);
            check(policy.Owner == mod.Id && policy.Visibility == ModItemVisibility.Inherit && policy.RequiredGroup == row.GetAttribute("PackLabel") &&
                policy.MinimumLevel == listing.Level, "Restored equipment gate drift.");
            var expected = (XmlElement)row.SelectSingleNode("Enchantments/Perk");
            var actual = catalog.ItemDefaultEnchantments.Single(value => value.Item == item.Id).Entries.Single();
            check(actual.Perk == CoreContentImporter.PerkId(expected.GetAttribute("Name")) &&
                actual.Aspect == int.Parse(expected["Set"].GetAttribute("Aspect")), "Restored equipment enchantment drift.");
            ModRuntime.Scripts = new ModScriptSession { Content = catalog };
            var native = new ItemInfo { Name = item.Id.ToString(), DCHJDPCEODD = true };
            var player = new Roster { Level = listing.Level, Group = "" };
            check(!ShopAvailabilityPolicy.IsAvailable(native, player), "Restored equipment bypassed group gate.");
            player.Group = policy.RequiredGroup; player.Level--;
            check(!ShopAvailabilityPolicy.IsAvailable(native, player), "Restored equipment bypassed level gate.");
            player.Level++;
            check(ShopAvailabilityPolicy.IsAvailable(native, player), "Eligible restored equipment still locked.");
            ModRuntime.Scripts = null;
        }
    }
}
