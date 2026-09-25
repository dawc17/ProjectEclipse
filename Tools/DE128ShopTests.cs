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
    internal bool TryOverrideInitialProfile(int level, int upgradeLevel,
        System.Collections.Generic.IReadOnlyDictionary<string, int> stats, string upgradeTemplate,
        string legacyPaidItem, bool clearLocalUpgrades, out IDisposable lifetime)
    { lifetime = null; return false; }
    internal bool TryOverrideShopPrice(long coins, long gems, out IDisposable lifetime)
    { lifetime = null; return false; }
    internal bool TryOverridePresentation(string icon, string model, out IDisposable lifetime)
    { lifetime = null; return false; }
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
        check(policies.Length == 221, "Expected the complete DE shop availability cohort.");
        check(catalog.ItemInitialProfiles.Count == 221, "Expected initial DE progression for every restored shop item.");
        check(catalog.ItemShopPrices.Count == 99, "Expected archived shop prices to be restored.");
        check(catalog.ItemPresentations.Count == 24, "Expected archived item icons and models to be restored.");
        check(catalog.ItemDefaultEnchantments.Count(value => policies.Any(policy => policy.Item == value.Item)) == 217,
            "Expected archived default enchantments for every changed shop item.");
        check(catalog.ItemInitialProfiles.Count(value => value.LegacyPaidItem == "none") == 129,
            "Expected every archived paid marker to be cleared.");
        string[] Enchantments(XmlElement element) => element.SelectNodes("Enchantments/Perk").Cast<XmlElement>()
            .Select(perk => perk.GetAttribute("Name") + "|" + (perk["Set"]?.GetAttribute("Aspect") ?? "")).ToArray();
        var profile = new Roster();
        foreach (var policy in policies)
        {
            check(catalog.TryGetItem(policy.Item, out var definition) && definition.IsCore, "Shop must reuse core identity.");
            var original = new XmlDocument { XmlResolver = null };
            original.LoadXml(definition.LegacyItemXml);
            var item = original.DocumentElement;
            string name = item.GetAttribute("Name");
            var expected = (XmlElement)archive.SelectSingleNode("/List/Items/Item[@Name='" + name + "']");
            check(expected != null && expected.GetAttribute("Type") != "", "Availability target is not archived equipment.");
            check(item.GetAttribute("ShopHide") == "1" && expected.GetAttribute("ShopHide") != "1", "Archive does not expose this item.");
            check(policy.Owner == mod.Id && policy.Visibility == ModItemVisibility.ForceVisible &&
                policy.RequiredGroup == expected.GetAttribute("PackLabel"), "Shop policy lost its DE group or owner.");
            check(policy.MinimumLevel == int.Parse(expected.GetAttribute("Level")), "Shop gate differs from archive.");
            var profilePatch = catalog.ItemInitialProfiles.Single(value => value.Item == policy.Item);
            check(profilePatch.Owner == mod.Id && profilePatch.Level == policy.MinimumLevel &&
                profilePatch.UpgradeLevel == int.Parse(expected.GetAttribute("UpgradeLevel")),
                "Initial item progression differs from the archive.");
            check(profilePatch.UpgradeTemplate == expected["Upgrades"]?.Attributes?["Template"]?.Value,
                "Initial item upgrade template differs from the archive: " + name);
            check(profilePatch.ClearLocalUpgrades == (item["Upgrades"]?.ChildNodes.Count > 0) &&
                expected["Upgrades"]?.ChildNodes.Count == 0,
                "Local upgrade rows differ from the archive: " + name);
            check(!expected.HasAttribute("PaidItem") &&
                profilePatch.LegacyPaidItem == (item.HasAttribute("PaidItem") ? "none" : null),
                "Legacy paid marker differs from the archive: " + name);
            var archivedEnchantments = Enchantments(expected);
            var canonicalEnchantments = Enchantments(item);
            var enchantmentPatches = catalog.ItemDefaultEnchantments.Where(value => value.Item == policy.Item).ToArray();
            if (!archivedEnchantments.SequenceEqual(canonicalEnchantments))
            {
                var archivedPerks = expected.SelectNodes("Enchantments/Perk").Cast<XmlElement>().ToArray();
                check(enchantmentPatches.Length == 1 &&
                    enchantmentPatches[0].Entries.Count == archivedPerks.Length &&
                    enchantmentPatches[0].Entries.Zip(archivedPerks, (entry, perk) =>
                        entry.Perk == CoreContentImporter.PerkId(perk.GetAttribute("Name")) &&
                        entry.Aspect == int.Parse(((XmlElement)perk["Set"]).GetAttribute("Aspect"))).All(value => value),
                    "Default enchantments differ from DE archive: " + name);
            }
            else check(enchantmentPatches.Length == 0, "Redundant default enchantment patch: " + name);
            foreach (var stat in profilePatch.InitialStats.Values)
                check(expected.GetAttribute(stat.Key) == stat.Value.ToString(),
                    "Initial stat differs from the archive: " + name + "/" + stat.Key);
            check(profilePatch.InitialStats.Values.Count > 0 &&
                profilePatch.InitialStats.Values.Count == new[] { "WeaponDamage", "BodyDefense", "HeadDefense",
                    "UnarmedDamage", "RangedDamage", "MagicDamage" }.Count(expected.HasAttribute),
                "Initial profile omitted or added an archived stat: " + name);
            var pricePatches = catalog.ItemShopPrices.Where(value => value.Item == policy.Item).ToArray();
            long sourceCoins = long.TryParse(item.GetAttribute("Price"), out var sc) ? sc : 0;
            long sourceGems = long.TryParse(item.GetAttribute("BonusPrice"), out var sg) ? sg : 0;
            long targetCoins = long.TryParse(expected.GetAttribute("Price"), out var tc) ? tc : 0;
            long targetGems = long.TryParse(expected.GetAttribute("BonusPrice"), out var tg) ? tg : 0;
            if (sourceCoins != targetCoins || sourceGems != targetGems)
                check(pricePatches.Length == 1 &&
                    pricePatches[0].Price.Currency == ModPriceCurrency.Gems &&
                    pricePatches[0].Price.Amount == targetGems &&
                    (targetCoins == 0 ? !pricePatches[0].SecondaryPrice.HasValue :
                        pricePatches[0].SecondaryPrice.HasValue &&
                        pricePatches[0].SecondaryPrice.Value.Currency == ModPriceCurrency.Coins &&
                        pricePatches[0].SecondaryPrice.Value.Amount == targetCoins),
                    "Archived shop price was not restored: " + name);
            else check(pricePatches.Length == 0, "Redundant shop price patch: " + name);
            check(targetCoins > 0 || targetGems > 0, "Archived shop item has no purchase price.");
            string icon = item.GetAttribute("Image") == expected.GetAttribute("Image") ? null : expected.GetAttribute("Image");
            string model = item.GetAttribute("Model") == expected.GetAttribute("Model") ? null : expected.GetAttribute("Model");
            var presentation = catalog.ItemPresentations.Where(value => value.Item == policy.Item).ToArray();
            check(icon == null && model == null ? presentation.Length == 0 :
                presentation.Length == 1 &&
                (icon == null ? presentation[0].Icon == default(AssetId) :
                    presentation[0].Icon.ToString() == "core:ui/items/" + icon.ToLowerInvariant()) &&
                (model == null ? presentation[0].Model == default(AssetId) :
                    presentation[0].Model.ToString() == "core:gamedata/models/" + model.ToLowerInvariant()),
                "Archived icon/model presentation differs: " + name);
            if (item.GetAttribute("SubType") != expected.GetAttribute("SubType"))
                check(catalog.ItemCombatSubtypes.Any(patch => patch.Item == policy.Item &&
                    patch.Subtype == expected.GetAttribute("SubType")),
                    "DE combat family differs without an active subtype patch: " + name);
            check(File.Exists(Path.Combine(repository, "Assets/Resources/ui/items", item.GetAttribute("Image") + ".png")) ||
                icon != null, "Missing imported shop icon: " + name);
            var native = new ItemInfo { Name = name, NodeXML = item, DCHJDPCEODD = false, MMHIKEIDDNB = "old_offer" };
            ModRuntime.Scripts = new ModScriptSession { Content = catalog };
            profile.Group = policy.RequiredGroup;
            profile.Level = policy.MinimumLevel - 1;
            check(!ShopAvailabilityPolicy.IsAvailable(native, profile), "Item exposed below its level gate.");
            profile.Level++;
            check(ShopAvailabilityPolicy.IsAvailable(native, profile), "Item not exposed at its level gate.");
            profile.Level = 52;
            check(ShopAvailabilityPolicy.IsAvailable(native, profile), "Item relocked after its level gate.");
            if (policy.RequiredGroup != "")
            {
                profile.Group = "";
                check(!ShopAvailabilityPolicy.IsAvailable(native, profile), "Item bypassed its DE act group.");
                profile.Group = policy.RequiredGroup;
            }
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

        string beforeProfile = ModSaveData.ComputeContentSetFingerprint(new[] { mod }, baseCatalog);
        using (var tx = baseCatalog.BeginRegistration(mod))
        {
            foreach (var bounds in new[] { (0, 1500), (53, 1500), (15, -1), (15, 5201) })
            {
                bool rejected = false;
                try { tx.SetItemInitialProfile(target, bounds.Item1, bounds.Item2, new ModEquipmentInitialStats(342)); }
                catch (ModContentException) { rejected = true; }
                check(rejected && tx.RegistrationCount == 0, "Invalid initial profile range poisoned registration.");
            }
            bool wrongStat = false;
            try { tx.SetItemInitialProfile(target, 15, 1500, new ModEquipmentInitialStats(headDefense: 1)); }
            catch (ModContentException) { wrongStat = true; }
            check(wrongStat && tx.RegistrationCount == 0, "Wrong-category initial stat was accepted.");
            bool missingStats = false;
            try { tx.SetItemInitialProfile(target, 15, 1500, null); }
            catch (ModContentException) { missingStats = true; }
            check(missingStats && tx.RegistrationCount == 0, "Missing initial stat snapshot was accepted.");
            bool wrongPaid = false;
            try { tx.SetItemInitialProfile(target, 15, 1500, new ModEquipmentInitialStats(342), "Weapon_Bonus", "unknown"); }
            catch (ModContentException) { wrongPaid = true; }
            check(wrongPaid && tx.RegistrationCount == 0, "Invalid legacy paid marker was accepted.");
            bool wrongTemplate = false;
            try { tx.SetItemInitialProfile(target, 15, 1500, new ModEquipmentInitialStats(342), "Armor_Bonus"); }
            catch (ModContentException) { wrongTemplate = true; }
            check(wrongTemplate && tx.RegistrationCount == 0, "Wrong-category upgrade template was accepted.");
            tx.SetItemInitialProfile(target, 15, 1500, new ModEquipmentInitialStats(342), "Weapon_Bonus", "none");
            bool duplicate = false;
            try { tx.SetItemInitialProfile(target, 15, 1500, new ModEquipmentInitialStats(343)); }
            catch (ModContentException) { duplicate = true; }
            check(duplicate && tx.RegistrationCount == 1, "Duplicate initial profile was accepted in one registration.");
            tx.Commit();
        }
        check(beforeProfile != ModSaveData.ComputeContentSetFingerprint(new[] { mod }, baseCatalog),
            "Initial profile is missing from the content fingerprint.");
        check(baseCatalog.ItemInitialProfiles.Single().UpgradeTemplate == "Weapon_Bonus",
            "Upgrade template was lost during profile registration.");
        check(baseCatalog.ItemInitialProfiles.Single().LegacyPaidItem == "none",
            "Legacy paid marker was lost during profile registration.");
        var alternateTemplate = new ModContentCatalog();
        CoreContentImporter.ImportWeapons(alternateTemplate, new[] { xml.DocumentElement }, null);
        using (var tx = alternateTemplate.BeginRegistration(mod))
        {
            tx.SetItemAvailability(target, ModItemVisibility.Inherit, "club", 20);
            tx.SetItemInitialProfile(target, 15, 1500, new ModEquipmentInitialStats(342), "Paid_Weapon_Bonus", "none");
            tx.Commit();
        }
        check(ModSaveData.ComputeContentSetFingerprint(new[] { mod }, baseCatalog) !=
            ModSaveData.ComputeContentSetFingerprint(new[] { mod }, alternateTemplate),
            "Upgrade template selection is missing from the content fingerprint.");
        var alternatePaidMarker = new ModContentCatalog();
        CoreContentImporter.ImportWeapons(alternatePaidMarker, new[] { xml.DocumentElement }, null);
        using (var tx = alternatePaidMarker.BeginRegistration(mod))
        {
            tx.SetItemAvailability(target, ModItemVisibility.Inherit, "club", 20);
            tx.SetItemInitialProfile(target, 15, 1500, new ModEquipmentInitialStats(342), "Weapon_Bonus");
            tx.Commit();
        }
        check(ModSaveData.ComputeContentSetFingerprint(new[] { mod }, baseCatalog) !=
            ModSaveData.ComputeContentSetFingerprint(new[] { mod }, alternatePaidMarker),
            "Legacy paid marker is missing from the content fingerprint.");
        bool competing = false;
        using (var tx = baseCatalog.BeginRegistration(mod))
        {
            tx.SetItemInitialProfile(target, 15, 1500, new ModEquipmentInitialStats(344));
            try { tx.Commit(); }
            catch (ModContentException) { competing = true; }
        }
        check(competing && baseCatalog.ItemInitialProfiles.Count == 1, "Competing initial profile replaced a committed patch.");

        string beforePrice = ModSaveData.ComputeContentSetFingerprint(new[] { mod }, baseCatalog);
        using (var tx = baseCatalog.BeginRegistration(mod))
        {
            foreach (var price in new[] { new ModPrice(ModPriceCurrency.Gems, 0),
                new ModPrice(ModPriceCurrency.Gems, (long)int.MaxValue + 1) })
            {
                bool rejected = false;
                try { tx.SetItemShopPrice(target, price); }
                catch (ModContentException) { rejected = true; }
                check(rejected && tx.RegistrationCount == 0, "Invalid shop price poisoned registration.");
            }
            bool sameCurrency = false;
            try { tx.SetItemShopPrice(target, new ModPrice(ModPriceCurrency.Gems, 39),
                new ModPrice(ModPriceCurrency.Gems, 4)); }
            catch (ModContentException) { sameCurrency = true; }
            check(sameCurrency && tx.RegistrationCount == 0, "Two shop prices in the same currency were accepted.");
            tx.SetItemShopPrice(target, new ModPrice(ModPriceCurrency.Gems, 39));
            bool duplicatePrice = false;
            try { tx.SetItemShopPrice(target, new ModPrice(ModPriceCurrency.Coins, 7)); }
            catch (ModContentException) { duplicatePrice = true; }
            check(duplicatePrice && tx.RegistrationCount == 1, "Duplicate shop price accepted.");
            tx.Commit();
        }
        check(baseCatalog.ItemShopPrices.Single().Price.Amount == 39 &&
            beforePrice != ModSaveData.ComputeContentSetFingerprint(new[] { mod }, baseCatalog),
            "Shop price was lost or omitted from the content fingerprint.");
        bool competingPrice = false;
        using (var tx = baseCatalog.BeginRegistration(mod))
        {
            tx.SetItemShopPrice(target, new ModPrice(ModPriceCurrency.Coins, 39));
            try { tx.Commit(); }
            catch (ModContentException) { competingPrice = true; }
        }
        check(competingPrice && baseCatalog.ItemShopPrices.Count == 1,
            "Competing shop price replaced a committed patch.");
        string beforePresentation = ModSaveData.ComputeContentSetFingerprint(new[] { mod }, baseCatalog);
        var iconAsset = AssetId.Parse("core:ui/items/weapon_kunai");
        var modelAsset = AssetId.Parse("core:gamedata/models/mdl_weapon_cool_katana");
        using (var tx = baseCatalog.BeginRegistration(mod))
        {
            bool missingArt = false;
            try { tx.SetItemPresentation(target, default(AssetId), default(AssetId)); }
            catch (ModContentException) { missingArt = true; }
            check(missingArt && tx.RegistrationCount == 0, "Empty presentation was accepted.");
            tx.SetItemPresentation(target, iconAsset, modelAsset);
            bool duplicateArt = false;
            try { tx.SetItemPresentation(target, iconAsset, default(AssetId)); }
            catch (ModContentException) { duplicateArt = true; }
            check(duplicateArt && tx.RegistrationCount == 1, "Duplicate presentation was accepted.");
            tx.Commit();
        }
        check(baseCatalog.ItemPresentations.Single().Icon == iconAsset &&
            baseCatalog.ItemPresentations.Single().Model == modelAsset &&
            beforePresentation != ModSaveData.ComputeContentSetFingerprint(new[] { mod }, baseCatalog),
            "Item presentation was lost or omitted from the fingerprint.");
        bool competingArt = false;
        using (var tx = baseCatalog.BeginRegistration(mod))
        {
            tx.SetItemPresentation(target, iconAsset, default(AssetId));
            try { tx.Commit(); }
            catch (ModContentException) { competingArt = true; }
        }
        check(competingArt && baseCatalog.ItemPresentations.Count == 1,
            "Competing presentation replaced a committed patch.");
    }
}
