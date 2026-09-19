using System;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

// Compile the actual ShopAvailabilityPolicy with controlled profile/item services.
// Catalog resolution, Lua registration and archive assertions use production data.
public sealed class ItemInfo
{
    public string Name, MMHIKEIDDNB;
    public bool DCHJDPCEODD, Hidden;
    public XmlNode NodeXML;
    public bool GOKHJMOEGIJ() => Hidden;
}
public sealed class Roster
{
    public int Level;
    public string Group;
    public bool FLFKOIPCEPI(string group) => Group == group;
}
namespace Eclipse.Modding
{
    public sealed class ModScriptSession { public ModContentCatalog Content; }
    public static class ModRuntime { public static ModScriptSession Scripts; }
}

internal static class DE128ShopTests
{
    internal static void Run(ModDescriptor mod, ModContentCatalog catalog, string repository, Action<bool, string> check)
    {
        var archive = new XmlDocument { XmlResolver = null };
        archive.Load(Path.Combine(repository, "Assets/DExml/list.xml"));
        var policies = catalog.ItemAvailabilityPolicies.Where(value => value.Item.Namespace.Value == "core").ToArray();
        check(policies.Length == 25, "Expected five complete former battle-pass equipment collections.");
        var profile = new Roster();
        foreach (var policy in policies)
        {
            check(catalog.TryGetItem(policy.Item, out var definition) && definition.IsCore, "Shop must reuse core identity.");
            var original = new XmlDocument { XmlResolver = null };
            original.LoadXml(definition.LegacyItemXml);
            var item = original.DocumentElement;
            string name = item.GetAttribute("Name");
            var expected = (XmlElement)archive.SelectSingleNode("/List/Items/Item[@Name='" + name + "']");
            check(expected != null && name.Contains("_BP_S"), "Availability target is not archived battle-pass equipment.");
            check(item.GetAttribute("ShopHide") == "1" && expected.GetAttribute("ShopHide") != "1", "Archive does not expose this item.");
            check(policy.Owner == mod.Id && policy.Visibility == ModItemVisibility.ForceVisible && policy.RequiredGroup == "",
                "Shop policy retained a paid/event group or wrong owner.");
            check(policy.MinimumLevel == int.Parse(expected.GetAttribute("Level")), "Shop gate differs from archive.");
            check(item.GetAttribute("BonusPrice") == expected.GetAttribute("BonusPrice") && int.Parse(item.GetAttribute("BonusPrice")) > 0,
                "Battle-pass listing lost its existing earned-gem price.");
            check(item.GetAttribute("Image") == expected.GetAttribute("Image") && item.GetAttribute("Model") == expected.GetAttribute("Model"),
                "Core art differs from archive; this item needs separate asset work.");
            check(File.Exists(Path.Combine(repository, "Assets/Resources/ui/items", item.GetAttribute("Image") + ".png")),
                "Missing imported shop icon: " + name);
            var native = new ItemInfo { Name = name, NodeXML = item, DCHJDPCEODD = false, MMHIKEIDDNB = "old_offer" };
            ModRuntime.Scripts = new ModScriptSession { Content = catalog };
            profile.Level = policy.MinimumLevel - 1;
            check(!ShopAvailabilityPolicy.IsAvailable(native, profile), "Item exposed below its level gate.");
            profile.Level++;
            check(ShopAvailabilityPolicy.IsAvailable(native, profile), "Item not exposed at its level gate.");
            profile.Level = 52;
            check(ShopAvailabilityPolicy.IsAvailable(native, profile), "Item relocked after its level gate.");
            ModRuntime.Scripts = null;
            check(!ShopAvailabilityPolicy.IsAvailable(native, profile), "Unloading failed to restore base visibility.");
            check(definition.LegacyItemXml == original.DocumentElement.OuterXml, "Availability mutated core item data.");
        }

        var vanilla = new XmlDocument { XmlResolver = null };
        vanilla.Load(Path.Combine(repository, "Assets/vanillaXml/list.xml"));
        var missing = archive.SelectNodes("/List/Items/Item[@Type='Weapon']").Cast<XmlElement>()
            .Where(row => vanilla.SelectSingleNode("/List/Items/Item[@Name='" + row.GetAttribute("Name") + "']") == null).ToArray();
        check(missing.Length == 10, "Missing-weapon archive inventory changed.");
        foreach (var row in missing)
        {
            var weapon = catalog.Weapons.Single(value => !value.IsCore && value.Model.ToString() ==
                ("core:gamedata/models/" + row.GetAttribute("Model")).ToLowerInvariant());
            check(weapon.SubType == row.GetAttribute("SubType") && weapon.Icon.ToString() ==
                ("core:ui/items/" + row.GetAttribute("Image")).ToLowerInvariant(), "Restored weapon classification/art drift: " + row.GetAttribute("Name"));
            check(row.HasAttribute("WeaponDamage") ? weapon.InitialStats == null :
                weapon.InitialStats != null && weapon.InitialStats.Values.Count == 0,
                "Restored initial damage presence differs from archive.");
            var listing = catalog.ShopListings.Single(value => value.Item == weapon.Id);
            check(listing.Level == int.Parse(row.GetAttribute("Level")) && listing.Price.Currency == ModPriceCurrency.Gems &&
                listing.Price.Amount == long.Parse(row.GetAttribute("BonusPrice")), "Restored weapon price/level drift.");
            var availability = catalog.ItemAvailabilityPolicies.Single(value => value.Item == weapon.Id);
            check(availability.RequiredGroup == row.GetAttribute("PackLabel") && availability.MinimumLevel == listing.Level &&
                availability.Visibility == ModItemVisibility.Inherit, "Restored weapon group/level drift.");
            var enchantment = catalog.ItemDefaultEnchantments.Single(value => value.Item == weapon.Id).Entries.Single();
            var perk = (XmlElement)row.SelectSingleNode("Enchantments/Perk");
            check(enchantment.Perk == CoreContentImporter.PerkId(perk.GetAttribute("Name")) &&
                enchantment.Aspect == int.Parse(((XmlElement)perk["Set"]).GetAttribute("Aspect")), "Restored weapon default enchantment drift.");
            var native = new ItemInfo { Name = weapon.Id.ToString(), DCHJDPCEODD = true };
            ModRuntime.Scripts = new ModScriptSession { Content = catalog };
            profile.Level = listing.Level; profile.Group = "";
            check(!ShopAvailabilityPolicy.IsAvailable(native, profile), "Restored weapon bypassed act gate.");
            profile.Group = availability.RequiredGroup; profile.Level--;
            check(!ShopAvailabilityPolicy.IsAvailable(native, profile), "Restored weapon bypassed level gate.");
            profile.Level++;
            check(ShopAvailabilityPolicy.IsAvailable(native, profile), "Restored weapon remains locked after act/level gates.");
            ModRuntime.Scripts = null;
        }

        var target = policies[0].Item;
        var baseCatalog = new ModContentCatalog();
        var xml = new XmlDocument();
        xml.LoadXml(catalog.Weapons.First(x => x.Id == target || x.LegacyName == "WEAPON_BP_S1_GUARDIAN").LegacyItemXml);
        // Use one canonical weapon in a fresh catalog to test generic composition.
        CoreContentImporter.ImportWeapons(baseCatalog, new[] { xml.DocumentElement }, null);
        target = CoreContentImporter.WeaponId(xml.DocumentElement.GetAttribute("Name"));
        var probe = new ItemInfo { Name = xml.DocumentElement.GetAttribute("Name"), NodeXML = xml.DocumentElement, DCHJDPCEODD = true };
        check(!ShopAvailabilityPolicy.IsAvailable(null, profile) && !ShopAvailabilityPolicy.IsAvailable(probe, null), "Null guards changed.");
        string noPolicy = ModSaveData.ComputeContentSetFingerprint(new[] { mod }, baseCatalog);
        using (var tx = baseCatalog.BeginRegistration(mod))
        {
            foreach (int level in new[] { -1, 53 })
            {
                bool rejected = false;
                try { tx.SetItemAvailability(target, ModItemVisibility.ForceVisible, null, level); }
                catch (ModContentException) { rejected = true; }
                check(rejected && tx.RegistrationCount == 0, "Invalid level poisoned the registration transaction.");
            }
            tx.SetItemAvailability(target, ModItemVisibility.Inherit, "club", 20);
            tx.Commit();
        }
        check(noPolicy != ModSaveData.ComputeContentSetFingerprint(new[] { mod }, baseCatalog), "Availability missing from fingerprint.");
        ModRuntime.Scripts = new ModScriptSession { Content = baseCatalog };
        profile.Level = 20; profile.Group = "";
        check(!ShopAvailabilityPolicy.IsAvailable(probe, profile), "Level bypassed required group.");
        profile.Group = "club";
        check(ShopAvailabilityPolicy.IsAvailable(probe, profile), "Satisfied inherited policy rejected.");
        probe.Hidden = true;
        check(!ShopAvailabilityPolicy.IsAvailable(probe, profile), "Inherited policy bypassed hidden flag.");
        probe.Hidden = false; profile.Level = 19;
        check(!ShopAvailabilityPolicy.IsAvailable(probe, profile), "Group bypassed level.");
        ModRuntime.Scripts = null;
    }
}
