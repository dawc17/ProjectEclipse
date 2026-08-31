// Test-only recovered-runtime stubs for the isolated mod/content smoke project.
// The real project compiles the adapter against the recovered Items/ListSF/LocalizationManager classes.
using System;
using System.Collections.Generic;
using System.Xml;

public sealed class Attributes
{
    private readonly Dictionary<string, int> _values = new Dictionary<string, int>(StringComparer.Ordinal);

    public void Set(string name, int value) { _values[name] = value; }

    public bool Get(string name, ref int value, bool applyAspect = true, bool resolveAspect = false)
    {
        return _values.TryGetValue(name, out value);
    }
}

public sealed class UpgradeData
{
    public sealed class Values
    {
        public readonly Attributes IBLHIAHECLK = new Attributes();
        public int Level;
        public int AKKLOMFOLNO;
    }

    public readonly Values OGLHOJNMEBD = new Values();
}

public sealed class UpgradeDataContainer
{
    public string Type = string.Empty;
    public readonly List<UpgradeData> KPAPEBOAKIE = new List<UpgradeData>();
}

public sealed class ItemInfo
{
    public int Index;
    public string Name;
    public string FileName;
    public string Model;
    public string Type;
    public string SubType;
    public string Text;
    public long Price;
    public long BonusPrice;
    public int Level;
    public int UpgradeLevel;
    public int WeaponDamage;
    public int BodyDefense;
    public int HeadDefense;
    public int UnarmedDamage;
    public int RangedDamage;
    public int MagicDamage;
    public string UpgradeTemplate;
    public XmlNode NodeXML;
}

public sealed class Items
{
    private readonly List<ItemInfo> _all = new List<ItemInfo>();
    private readonly List<ItemInfo> _weapons = new List<ItemInfo>();
    private readonly List<ItemInfo> _armors = new List<ItemInfo>();
    private readonly List<ItemInfo> _helms = new List<ItemInfo>();
    private readonly List<ItemInfo> _ranged = new List<ItemInfo>();
    private readonly List<ItemInfo> _magic = new List<ItemInfo>();
    private readonly Dictionary<string, UpgradeDataContainer> _upgrades =
        new Dictionary<string, UpgradeDataContainer>(StringComparer.Ordinal);

    public List<ItemInfo> MJKFCBMNNGJ() { return _weapons; }
    public List<ItemInfo> MCGKNJPLIIH() { return _armors; }
    public List<ItemInfo> EKKIBLDGNHH() { return _helms; }
    public List<ItemInfo> LKGPBHADANE() { return _ranged; }
    public List<ItemInfo> OGFOBKIEGKA() { return _magic; }
    public List<ItemInfo> HCDLKHKBEPF() { return _all; }

    public UpgradeDataContainer BKPOCLGODDM(string name)
    {
        UpgradeDataContainer value;
        return _upgrades.TryGetValue(name, out value) ? value : null;
    }

    public void AddCoreUpgradeTemplates(XmlDocument document)
    {
        foreach (XmlNode containerNode in document.SelectNodes("/List/UpgradeList/Upgrades"))
        {
            string name = Attr(containerNode, "Name");
            if (string.IsNullOrEmpty(name)) continue;
            var container = new UpgradeDataContainer { Type = name };
            foreach (XmlNode node in containerNode.SelectNodes("Upgrade"))
            {
                var upgrade = new UpgradeData();
                upgrade.OGLHOJNMEBD.Level = IntAttr(node, "Level");
                upgrade.OGLHOJNMEBD.AKKLOMFOLNO = IntAttr(node, "UpgradeLevel");
                foreach (string attribute in new[]
                {
                    "WeaponDamage", "BodyDefense", "UnarmedDamage", "HeadDefense", "RangedDamage", "MagicDamage"
                })
                {
                    XmlAttribute value = node.Attributes[attribute];
                    int parsed;
                    if (value != null && int.TryParse(value.Value, out parsed))
                        upgrade.OGLHOJNMEBD.IBLHIAHECLK.Set(attribute, parsed);
                }
                container.KPAPEBOAKIE.Add(upgrade);
            }
            _upgrades[name] = container;
        }
    }

    public ItemInfo KCCDBEEKBCG(string name)
    {
        ItemInfo item = _all.Find(value => value.Name == name);
        if (item != null) return item;
        Eclipse.Modding.DefinitionId id;
        Eclipse.Modding.ItemDefinition definition;
        var scripts = Eclipse.Modding.ModRuntime.Scripts;
        if (scripts != null && Eclipse.Modding.DefinitionId.TryParse(name, out id) &&
            scripts.Content.TryResolveItem(id, out definition))
        {
            if (definition.IsCore)
            {
                ItemInfo exact = _all.Find(value => value.Name == definition.LegacyName && value.NodeXML != null &&
                    definition.LegacyItemXml != null && value.NodeXML.OuterXml == definition.LegacyItemXml);
                return exact ?? _all.Find(value => value.Name == definition.LegacyName);
            }
            return _all.Find(value => value.Name == definition.Id.ToString());
        }
        return null;
    }

    public ItemInfo AddExternalItem(XmlNode node)
    {
        string name = node.Attributes["Name"].Value;
        if (KCCDBEEKBCG(name) != null) throw new InvalidOperationException("Item already exists: " + name);
        ItemInfo item = CreateItem(node);
        List<ItemInfo> category = Category(item.Type);
        if (category == null) throw new InvalidOperationException("Unsupported external item type: " + item.Type);
        _all.Add(item);
        category.Add(item);
        return item;
    }

    public ItemInfo AddExternalWeapon(XmlNode node)
    {
        if (Attr(node, "Type") != "Weapon") throw new InvalidOperationException("Expected Weapon.");
        return AddExternalItem(node);
    }

    public ItemInfo AddCoreItem(XmlNode node)
    {
        string name = Attr(node, "Name");
        // The recovered vanilla parser preserves duplicate legacy Names in list.xml
        // (currently GlaivebowArrow). Qualified core identity disambiguates them later.
        if (string.IsNullOrEmpty(name)) throw new InvalidOperationException("Invalid core item name.");
        ItemInfo item = CreateItem(node);
        _all.Add(item);
        List<ItemInfo> category = Category(item.Type);
        if (category != null) category.Add(item);
        return item;
    }

    public bool RemoveExternalItem(string name)
    {
        ItemInfo item = KCCDBEEKBCG(name);
        if (item == null) return false;
        List<ItemInfo> category = Category(item.Type);
        if (category == null) return false;
        category.Remove(item);
        _all.Remove(item);
        return true;
    }

    public bool RemoveExternalWeapon(string name)
    {
        ItemInfo item = KCCDBEEKBCG(name);
        return item != null && item.Type == "Weapon" && RemoveExternalItem(name);
    }

    private List<ItemInfo> Category(string type)
    {
        switch (type)
        {
            case "Weapon": return _weapons;
            case "Armor": return _armors;
            case "Helm": return _helms;
            case "Ranged": return _ranged;
            case "Magic": return _magic;
            default: return null;
        }
    }

    private static string Attr(XmlNode node, string name)
    {
        XmlAttribute attribute = node.Attributes[name];
        return attribute == null ? string.Empty : attribute.Value;
    }

    private ItemInfo CreateItem(XmlNode node)
    {
        return new ItemInfo
        {
            Index = _all.Count,
            NodeXML = node.CloneNode(true),
            Name = Attr(node, "Name"),
            FileName = Attr(node, "Image"),
            Model = Attr(node, "Model"),
            Type = Attr(node, "Type"),
            SubType = Attr(node, "SubType"),
            Text = Attr(node, "Text"),
            Price = LongAttr(node, "Price"),
            BonusPrice = LongAttr(node, "BonusPrice"),
            Level = IntAttr(node, "Level"),
            UpgradeLevel = IntAttr(node, "UpgradeLevel"),
            WeaponDamage = IntAttr(node, "WeaponDamage"),
            BodyDefense = IntAttr(node, "BodyDefense"),
            HeadDefense = IntAttr(node, "HeadDefense"),
            UnarmedDamage = IntAttr(node, "UnarmedDamage"),
            RangedDamage = IntAttr(node, "RangedDamage"),
            MagicDamage = IntAttr(node, "MagicDamage"),
            UpgradeTemplate = node["Upgrades"] == null ? string.Empty : Attr(node["Upgrades"], "Template")
        };
    }

    private static int IntAttr(XmlNode node, string name)
    {
        int value;
        return int.TryParse(Attr(node, name), out value) ? value : 0;
    }

    private static long LongAttr(XmlNode node, string name)
    {
        long value;
        return long.TryParse(Attr(node, name), out value) ? value : 0L;
    }
}

public static class ListSF
{
    private static Items _items = new Items();
    public static Items DJBOFEEKJMP() { return _items; }
    public static void ResetModdingTestItems() { _items = new Items(); }

    public static void SeedModdingTestCoreItems()
    {
        var document = new XmlDocument();
        document.Load(System.IO.Path.Combine(Eclipse.Content.GameplayContentArchive.GetXmlRoot(), "list.xml"));
        _items.AddCoreUpgradeTemplates(document);
        foreach (XmlNode node in document.SelectNodes(
            "/List/Items/Item[@Type='Weapon' or @Type='Armor' or @Type='Helm' or @Type='Ranged' or @Type='Magic']"))
            _items.AddCoreItem(node);
    }
}

public sealed class Language
{
    public string name;
}

public static class LocalizationManager
{
    private static readonly Dictionary<string, string> Strings = new Dictionary<string, string>();
    public static string POIPGLLCCKC = "eng";
    public static Language ILAJKOBCHFH = new Language { name = "eng" };
    public static event Action OCLBJLPOKLB;

    public static void SetExternalString(string key, string value) { Strings[key] = value; }
    public static void RemoveExternalString(string key) { Strings.Remove(key); }

    public static string GetExternalStringForTest(string key)
    {
        string value;
        return Strings.TryGetValue(key, out value) ? value : null;
    }

    public static void ResetModdingTestLanguage(string language)
    {
        Strings.Clear();
        POIPGLLCCKC = language;
        ILAJKOBCHFH = new Language { name = language };
    }

    public static void ChangeModdingTestLanguage(string language)
    {
        Strings.Clear();
        ILAJKOBCHFH = new Language { name = language };
        Action changed = OCLBJLPOKLB;
        if (changed != null) changed();
    }
}
