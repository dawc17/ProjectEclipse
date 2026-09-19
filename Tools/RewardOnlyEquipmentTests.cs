using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Xml;
using Eclipse.Modding;

public static class RewardOnlyEquipmentTests
{
    private static int _checks;

    private static void Check(bool condition, string message)
    {
        _checks++;
        if (!condition) throw new Exception(message);
    }

    private static Items CreateNativeItems(string projectRoot)
    {
        return new Items();
    }

    private static ModContentCatalog CreateCatalog(string projectRoot, bool duplicateNativeName)
    {
        ModDescriptor mod = ModDiscovery.DiscoverLoose(Path.Combine(projectRoot, "Mods")).Mods
            .First(value => value.Id.Value == "example.charge-ui");
        var catalog = new ModContentCatalog();
        using (ModRegistrationTransaction transaction = catalog.BeginRegistration(mod))
        {
            DefinitionId weaponName = transaction.AddLocalization("reward_only_weapon", "eng", "Reward Weapon");
            DefinitionId armorName = transaction.AddLocalization("reward_only_armor", "eng", "Reward Armor");
            DefinitionId helmName = transaction.AddLocalization("reward_only_helm", "eng", "Reward Helm");
            DefinitionId rangedName = transaction.AddLocalization("reward_only_ranged", "eng", "Reward Ranged");
            DefinitionId magicName = transaction.AddLocalization("reward_only_magic", "eng", "Reward Magic");
            DefinitionId listedName = transaction.AddLocalization("listed_weapon", "eng", "Listed Weapon");

            string weaponId = duplicateNativeName ? "duplicate" : "reward_only_weapon";
            transaction.RegisterWeapon(weaponId, weaponName, default(AssetId), default(AssetId), "TitanGiantSword");
            transaction.RegisterArmor("reward_only_armor", armorName, default(AssetId), default(AssetId));
            transaction.RegisterHelm("reward_only_helm", helmName, default(AssetId), default(AssetId));
            transaction.RegisterRanged("reward_only_ranged", rangedName, default(AssetId), default(AssetId), "Needle");
            transaction.RegisterMagic("reward_only_magic", magicName, default(AssetId), default(AssetId), "MindThrow");
            WeaponDefinition listed = transaction.RegisterWeapon("listed_weapon", listedName, default(AssetId), default(AssetId), "OneHandedSword");
            transaction.RegisterShopListing(listed.Id, ModShopSection.Weapons, 3, new ModPrice(ModPriceCurrency.Coins, 77));
            WeaponDefinition gem = transaction.RegisterWeapon("gem_weapon", listedName, default(AssetId), default(AssetId), "Katana");
            transaction.RegisterShopListing(gem.Id, ModShopSection.Weapons, 1, new ModPrice(ModPriceCurrency.Gems, 9));
            transaction.AddLocalization("reward_only_weapon", "pol", "Polish Reward Weapon");
            transaction.Commit();
        }
        catalog.Freeze();
        return catalog;
    }

    private static void CheckUnlisted(ItemInfo item, string type, int level, string statName)
    {
        Check(item != null, type + " reward-only equipment was not materialized.");
        XmlElement node = (XmlElement)item.NodeXML;
        Check(node.GetAttribute("Type") == type, type + " native type changed.");
        Check(node.GetAttribute("ShopHide") == "1", type + " reward-only equipment is visible in the shop.");
        Check(!node.HasAttribute("Price") && !node.HasAttribute("BonusPrice"), type + " reward-only equipment invented a price.");
        Check(node.GetAttribute("Level") == level.ToString(), type + " reward-only equipment used the wrong baseline level.");
        Check(node.GetAttribute("UpgradeLevel") == (level * 100).ToString(), type + " reward-only equipment used the wrong baseline upgrade level.");
        Check(node.GetAttribute(statName) == (level * 100 + 7).ToString(CultureInfo.InvariantCulture),
            type + " reward-only equipment passed the wrong level to the controlled stat resolver.");
        Check(node["Upgrades"] != null && !string.IsNullOrEmpty(node["Upgrades"].GetAttribute("Template")),
            type + " reward-only equipment lost its vanilla upgrade template.");
    }

    private static void CheckInitialStatProjection(string projectRoot)
    {
        var mod = ModDiscovery.DiscoverLoose(Path.Combine(projectRoot, "Mods")).Mods.First(value => value.Id.Value == "example.charge-ui");
        foreach (bool empty in new[] { true, false })
        {
            var catalog = new ModContentCatalog();
            using (var tx = catalog.BeginRegistration(mod))
            {
                var title = tx.AddLocalization("stats", "eng", "Stats");
                tx.RegisterWeapon("stats", title, default(AssetId), default(AssetId), "Katana", initialStats: empty ? new ModEquipmentInitialStats() : new ModEquipmentInitialStats(weaponDamage: 0));
                tx.RegisterArmor("stats", title, default(AssetId), default(AssetId), empty ? new ModEquipmentInitialStats() : new ModEquipmentInitialStats(bodyDefense: 17));
                tx.RegisterHelm("stats", title, default(AssetId), default(AssetId), empty ? new ModEquipmentInitialStats() : new ModEquipmentInitialStats(headDefense: 13));
                tx.RegisterRanged("stats", title, default(AssetId), default(AssetId), "Needle", empty ? new ModEquipmentInitialStats() : new ModEquipmentInitialStats(rangedDamage: 29, weaponDamage: 7));
                tx.RegisterMagic("stats", title, default(AssetId), default(AssetId), "MindThrow", empty ? new ModEquipmentInitialStats() : new ModEquipmentInitialStats(magicDamage: 31));
                tx.Commit();
            }
            catalog.Freeze(); var items = new Items(); var adapter = new LegacyContentAdapter(catalog); adapter.ApplyItems(items);
            foreach (string category in new[] { "weapon", "armor", "helm", "ranged", "magic" })
            {
                var id = DefinitionId.Parse(mod.Id + ":items/" + category + "/stats"); catalog.TryGetItem(id, out var definition);
                var node = (XmlElement)items.GetItemByName(id.ToString()).NodeXML;
                foreach (string stat in new[] { "WeaponDamage", "BodyDefense", "HeadDefense", "UnarmedDamage", "RangedDamage", "MagicDamage" })
                {
                    bool present = definition.InitialStats.Values.TryGetValue(stat, out int expected);
                    Check(node.HasAttribute(stat) == present && (!present || node.GetAttribute(stat) == expected.ToString()),
                        "Initial snapshot did not replace derived stat: " + category + "/" + stat);
                }
                Check(node["Upgrades"] != null, "Explicit initial stats removed vanilla upgrades.");
            }
            adapter.RemoveTestContent(); Check(items.Count == 0, "Explicit stats left native definitions after cleanup.");
        }
    }

    public static void Main(string[] args)
    {
        string projectRoot = args[0];
        CheckInitialStatProjection(projectRoot);
        Items items = CreateNativeItems(projectRoot);
        ModContentCatalog catalog = CreateCatalog(projectRoot, false);
        var adapter = new LegacyContentAdapter(catalog);
        adapter.ApplyItems(items);

        CheckUnlisted(items.GetItemByName("example.charge-ui:items/weapon/reward_only_weapon"), "Weapon", 1, "WeaponDamage");
        CheckUnlisted(items.GetItemByName("example.charge-ui:items/armor/reward_only_armor"), "Armor", 2, "BodyDefense");
        CheckUnlisted(items.GetItemByName("example.charge-ui:items/helm/reward_only_helm"), "Helm", 2, "HeadDefense");
        CheckUnlisted(items.GetItemByName("example.charge-ui:items/ranged/reward_only_ranged"), "Ranged", 6, "RangedDamage");
        CheckUnlisted(items.GetItemByName("example.charge-ui:items/magic/reward_only_magic"), "Magic", 6, "MagicDamage");

        ItemInfo listed = items.GetItemByName("example.charge-ui:items/weapon/listed_weapon");
        Check(listed != null, "Listed equipment was not materialized.");
        XmlElement listedNode = (XmlElement)listed.NodeXML;
        Check(!listedNode.HasAttribute("ShopHide"), "Listed equipment was hidden from the shop.");
        Check(listedNode.GetAttribute("Price") == "77" && !listedNode.HasAttribute("BonusPrice"), "Listed coin price changed.");
        Check(listedNode.GetAttribute("Level") == "3" && listedNode.GetAttribute("UpgradeLevel") == "300", "Listed level changed.");
        var gemNode = (XmlElement)items.GetItemByName("example.charge-ui:items/weapon/gem_weapon").NodeXML;
        Check(gemNode.GetAttribute("BonusPrice") == "9" && !gemNode.HasAttribute("Price"), "Listed gem price changed.");

        string rewardKey = "example.charge-ui:items/weapon/reward_only_weapon";
        LocalizationManager.Base[rewardKey] = "Previous label";
        adapter.ApplyLocalization();
        Check(LocalizationManager.Get(rewardKey) == "Reward Weapon", "Unlisted equipment display name was not aliased.");
        LocalizationManager.ChangeLanguage("pol");
        Check(LocalizationManager.Get(rewardKey) == "Polish Reward Weapon", "Language change did not refresh unlisted equipment.");
        Check(LocalizationManager.Get("example.charge-ui:items/armor/reward_only_armor") == "Reward Armor", "English fallback was lost.");

        adapter.RemoveTestContent();
        Check(LocalizationManager.Get(rewardKey) == "Previous label", "Localization removal did not reveal the previous label.");
        Check(LocalizationManager.External.Count == 0, "Localization removal left external aliases.");
        foreach (string id in new[] {
            "example.charge-ui:items/weapon/reward_only_weapon",
            "example.charge-ui:items/armor/reward_only_armor",
            "example.charge-ui:items/helm/reward_only_helm",
            "example.charge-ui:items/ranged/reward_only_ranged",
            "example.charge-ui:items/magic/reward_only_magic",
            "example.charge-ui:items/weapon/listed_weapon",
            "example.charge-ui:items/weapon/gem_weapon"
        })
            Check(items.GetItemByName(id) == null, "Adapter disposal left external equipment behind: " + id);

        Items rollbackItems = CreateNativeItems(projectRoot);
        var duplicateDocument = new XmlDocument();
        duplicateDocument.LoadXml("<Item Name='example.charge-ui:items/weapon/duplicate' Type='Weapon' SubType='OneHandedSword' Level='1' WeaponDamage='5'/>");
        rollbackItems.AddExternalItem(duplicateDocument.DocumentElement);
        var broken = new LegacyContentAdapter(CreateCatalog(projectRoot, true));
        bool rejected = false;
        try { broken.ApplyItems(rollbackItems); }
        catch (InvalidOperationException) { rejected = true; }
        Check(rejected, "Existing native item collision was accepted.");
        Check(rollbackItems.GetItemByName("example.charge-ui:items/armor/reward_only_armor") == null,
            "Rejected equipment application partially added later categories.");
        Check(rollbackItems.Count == 1, "Collision removed the preexisting item.");

        var failingItems = new Items { FailOnAdd = 4 };
        var failingAdapter = new LegacyContentAdapter(catalog);
        rejected = false;
        try { failingAdapter.ApplyItems(failingItems); }
        catch (InvalidOperationException) { rejected = true; }
        Check(rejected && failingItems.Count == 0, "Late add failure left partially applied equipment.");
        failingItems.FailOnAdd = 0;
        failingAdapter.ApplyItems(failingItems);
        Check(failingItems.Count == 7, "Failed application could not be retried cleanly.");
        failingAdapter.RemoveTestContent();
        Check(failingItems.Count == 0, "Cleanup after retry left equipment behind.");

        Console.WriteLine("PASS: " + _checks + " checks of extracted production equipment/localization adapter methods. " +
            "Controlled item storage, stat resolver and localization services; no native Items/ItemInfo, art decoding or Unity playtest.");
    }
}

// Only native services are substituted. The runner extracts the actual production
// application, builder, enumeration, localization and removal methods on every run.
public sealed class ItemInfo
{
    public XmlNode NodeXML;
    public string Name => NodeXML.Attributes["Name"].Value;
}
public sealed class ItemSet { public string Name; }
public sealed class ItemSets
{
    public ItemSet AddExternalSet(XmlElement node) => throw new NotSupportedException("No sets in this fixture.");
    public void RemoveExternalSet(string name) => throw new NotSupportedException("No sets in this fixture.");
}
public sealed class Items
{
    private readonly Dictionary<string, ItemInfo> _items = new Dictionary<string, ItemInfo>();
    private int _adds;
    public int FailOnAdd;
    public int Count => _items.Count;
    public ItemInfo GetItemByName(string name) => _items.TryGetValue(name, out var item) ? item : null;
    public ItemInfo AddExternalItem(XmlElement node)
    {
        if (++_adds == FailOnAdd) throw new InvalidOperationException("Injected late add failure.");
        var item = new ItemInfo { NodeXML = node.CloneNode(true) };
        _items.Add(item.Name, item);
        return item;
    }
    public void RemoveExternalItem(string name) => _items.Remove(name);
    // best guess for name: GetItemSets. Signature retained for the extracted caller.
    public ItemSets DGKMILIPLLF() => new ItemSets();
}
public static class LocalizationManager
{
    public sealed class Language { public string name; }
    public static readonly Dictionary<string, string> Base = new Dictionary<string, string>();
    public static readonly Dictionary<string, string> External = new Dictionary<string, string>();
    // best guess for name: ActiveLanguage. Signature retained for the extracted caller.
    public static Language ILAJKOBCHFH = new Language { name = "eng" };
    // best guess for name: DefaultLanguage. Signature retained for the extracted caller.
    public static string POIPGLLCCKC = "eng";
    // best guess for name: LanguageChanged. Signature retained for the extracted caller.
    public static event Action OCLBJLPOKLB;
    public static void SetExternalString(string key, string value) => External[key] = value;
    public static void RemoveExternalString(string key) => External.Remove(key);
    public static string Get(string key) => External.TryGetValue(key, out var value) ? value :
        Base.TryGetValue(key, out value) ? value : string.Empty;
    public static void ChangeLanguage(string name) { ILAJKOBCHFH.name = name; OCLBJLPOKLB?.Invoke(); }
}
namespace Eclipse.Modding
{
    public sealed partial class LegacyContentAdapter
    {
        private readonly ModContentCatalog _content;
        private readonly List<string> _itemNames = new List<string>();
        private readonly List<string> _itemSetNames = new List<string>();
        private readonly List<string> _localizationKeys = new List<string>();
        private Items _items;
        private bool _itemsApplied, _languageSubscribed, _disposed;
        public LegacyContentAdapter(ModContentCatalog content) { _content = content; }
        private int ResolveVanillaStat(string template, int level, string attribute) => level * 100 + 7;
        private void ApplyP3Localization(string language) { }
        private XmlElement BuildNonEquipmentItemNode(NonEquipmentItemDefinition item) =>
            throw new NotSupportedException("No non-equipment in this fixture.");
        private XmlElement BuildItemSetNode(ItemSetDefinition item) =>
            throw new NotSupportedException("No sets in this fixture.");
        public void RemoveTestContent()
        {
            if (_languageSubscribed) LocalizationManager.OCLBJLPOKLB -= OnLanguageChanged;
            RemoveLocalization();
            RemoveItems();
            _disposed = true;
        }
    }
}
