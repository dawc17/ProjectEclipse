// Test-only recovered-runtime stubs for the isolated mod/content smoke project.
// The real project compiles the adapter against the recovered Items/ListSF/LocalizationManager classes.
using System;
using System.Collections.Generic;
using System.Xml;

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
    public string UpgradeTemplate;
}

public sealed class Items
{
    private readonly List<ItemInfo> _all = new List<ItemInfo>();
    private readonly List<ItemInfo> _weapons = new List<ItemInfo>();

    public List<ItemInfo> MJKFCBMNNGJ() { return _weapons; }
    public List<ItemInfo> HCDLKHKBEPF() { return _all; }

    public ItemInfo KCCDBEEKBCG(string name)
    {
        return _all.Find(item => item.Name == name);
    }

    public ItemInfo AddExternalWeapon(XmlNode node)
    {
        string name = node.Attributes["Name"].Value;
        if (KCCDBEEKBCG(name) != null) throw new InvalidOperationException("Item already exists: " + name);
        var item = new ItemInfo
        {
            Index = _all.Count,
            Name = name,
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
            UpgradeTemplate = node["Upgrades"] == null ? string.Empty : Attr(node["Upgrades"], "Template")
        };
        if (item.Type != "Weapon") throw new InvalidOperationException("Expected Weapon.");
        _all.Add(item);
        _weapons.Add(item);
        return item;
    }

    public bool RemoveExternalWeapon(string name)
    {
        ItemInfo item = KCCDBEEKBCG(name);
        if (item == null) return false;
        _weapons.Remove(item);
        _all.Remove(item);
        return true;
    }

    private static string Attr(XmlNode node, string name)
    {
        XmlAttribute attribute = node.Attributes[name];
        return attribute == null ? string.Empty : attribute.Value;
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
