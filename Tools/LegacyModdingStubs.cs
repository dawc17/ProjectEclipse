// Test-only recovered-runtime stubs for the isolated mod/content smoke project.
// The real project compiles the adapter against the recovered Items/ListSF/LocalizationManager classes.
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public static class TestXmlAttributeExtensions
{
    public static string CIPOICEEIBK(this XmlAttribute attribute, string fallback = "")
    {
        return attribute == null ? fallback : attribute.Value;
    }
}

public static class SF2Paths
{
    public static string KKIDGPBOBNI() { return Eclipse.Content.GameplayContentArchive.GetXmlRoot(); }
    public static string ENFGGKMDICD() { return Path.Combine(KKIDGPBOBNI(), "localizations"); }
}

public static class XmlUtils
{
    public static XmlDocument OpenXMLDocument(string root, string file)
    {
        string path = string.IsNullOrEmpty(file) ? root : Path.Combine(root, file);
        if (!File.Exists(path)) return null;
        var document = new XmlDocument { XmlResolver = null };
        document.Load(path);
        return document;
    }
}

public static class AnimationData
{
    public static int AddExternalMoves(XmlDocument document)
    {
        if (document == null || document["Movesxml"] == null) return 0;
        int count = 0;
        foreach (XmlNode child in document["Movesxml"].ChildNodes)
            if (child.NodeType == XmlNodeType.Element) count++;
        return count;
    }
}

public sealed class Tactic
{
    private readonly string _name;
    public Tactic(XmlNode node) { _name = node?.Attributes?["Name"]?.Value ?? string.Empty; }
    public string get_Name() { return _name; }
}

public static class TacticsCompiler
{
    public static void CompileTacticsSettings(XmlDocument document) { }
}

public static class AiData
{
    private static readonly Dictionary<string, Tactic> Tactics = new Dictionary<string, Tactic>(StringComparer.Ordinal);
    private static readonly HashSet<string> External = new HashSet<string>(StringComparer.Ordinal);

    public static Tactic GetTacticByName(string name)
    {
        Tactic tactic;
        return name != null && Tactics.TryGetValue(name, out tactic) ? tactic : null;
    }

    public static void AddExternalTactic(XmlNode node)
    {
        var tactic = new Tactic(node);
        if (string.IsNullOrEmpty(tactic.get_Name()) || Tactics.ContainsKey(tactic.get_Name()))
            throw new InvalidOperationException("Invalid or duplicate external tactic.");
        Tactics.Add(tactic.get_Name(), tactic);
        External.Add(tactic.get_Name());
    }

    public static bool RemoveExternalTactic(string name)
    {
        if (!External.Remove(name)) return false;
        return Tactics.Remove(name);
    }
}

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
    private readonly ItemSets _itemSets = new ItemSets();

    public List<ItemInfo> MJKFCBMNNGJ() { return _weapons; }
    public List<ItemInfo> MCGKNJPLIIH() { return _armors; }
    public List<ItemInfo> EKKIBLDGNHH() { return _helms; }
    public List<ItemInfo> LKGPBHADANE() { return _ranged; }
    public List<ItemInfo> OGFOBKIEGKA() { return _magic; }
    public List<ItemInfo> HCDLKHKBEPF() { return _all; }
    public ItemSets DGKMILIPLLF() { return _itemSets; }

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

public sealed class ItemSet
{
    public string Name { get; private set; }
    public ItemSet(XmlNode node) { Name = node?.Attributes?["Name"]?.Value ?? string.Empty; }
}

public sealed class ItemSets
{
    private readonly Dictionary<string, ItemSet> _sets = new Dictionary<string, ItemSet>(StringComparer.Ordinal);

    public ItemSet AddExternalSet(XmlNode node)
    {
        var set = new ItemSet(node);
        if (string.IsNullOrEmpty(set.Name) || _sets.ContainsKey(set.Name))
            throw new InvalidOperationException("Invalid or duplicate external item set.");
        _sets.Add(set.Name, set);
        return set;
    }

    public bool RemoveExternalSet(string name) { return !string.IsNullOrEmpty(name) && _sets.Remove(name); }
}

public sealed class PerkInfoItem
{
    public string Name = string.Empty;
    public XmlNode HAAKMBKCMCO { get; private set; }

    public void Parse(XmlNode node)
    {
        if (node == null) throw new ArgumentNullException("node");
        Name = node.Attributes?["Name"]?.Value ?? string.Empty;
        var document = new XmlDocument();
        XmlNode imported = document.ImportNode(node, true);
        document.AppendChild(imported);
        HAAKMBKCMCO = imported;
    }
}

public static class PerkStruct
{
    public const string EclipseEnchantmentAttribute = "EclipseEnchantment";
    public const string EclipseKindAttribute = "EclipseKind";
}

public sealed class PerkItems
{
    private readonly List<PerkInfoItem> _base = new List<PerkInfoItem>();
    private readonly HashSet<string> _external = new HashSet<string>(StringComparer.Ordinal);

    public List<PerkInfoItem> CJJEPHDFOCJ() { return _base; }

    public PerkInfoItem ABAGJKMKCBA(string name)
    {
        return _base.Find(value => value.Name == name);
    }

    public PerkInfoItem AddExternalBasePerk(XmlNode node)
    {
        if (node == null) throw new ArgumentNullException("node");
        string name = node.Attributes?["Name"]?.Value ?? string.Empty;
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("External perk requires Name.", "node");
        if (ABAGJKMKCBA(name) != null) throw new InvalidOperationException("Perk already exists: " + name);
        var perk = new PerkInfoItem();
        perk.Parse(node);
        _base.Add(perk);
        _external.Add(name);
        return perk;
    }

    public bool RemoveExternalBasePerk(string name)
    {
        if (!_external.Remove(name)) return false;
        PerkInfoItem perk = ABAGJKMKCBA(name);
        return perk != null && _base.Remove(perk);
    }

    public void SeedCore(string path)
    {
        var document = new XmlDocument();
        document.Load(path);
        XmlNode root = document.SelectSingleNode("/Perks");
        var compiled = new HashSet<string>(StringComparer.Ordinal);
        foreach (XmlNode node in document.SelectNodes("/Perks/Perk"))
            CompileTemplate(node, root, compiled, new HashSet<string>(StringComparer.Ordinal));
        foreach (XmlNode node in document.SelectNodes("/Perks/Perk"))
        {
            var perk = new PerkInfoItem();
            perk.Parse(node);
            _base.Add(perk);
        }
    }

    private static void CompileTemplate(XmlNode node, XmlNode root, HashSet<string> compiled, HashSet<string> visiting)
    {
        string name = node.Attributes?["Name"]?.Value ?? string.Empty;
        if (compiled.Contains(name)) return;
        if (!visiting.Add(name)) throw new InvalidOperationException("Perk template cycle in test fixture: " + name);
        string templateText = node.Attributes?["Template"]?.Value;
        if (!string.IsNullOrEmpty(templateText))
        {
            foreach (string templateName in templateText.Split('|'))
            {
                XmlNode template = root.SelectSingleNode("Perk[@Name='" + templateName.Replace("'", "&apos;") + "']");
                if (template == null) continue;
                CompileTemplate(template, root, compiled, visiting);
                foreach (XmlAttribute attribute in template.Attributes)
                    if (node.Attributes[attribute.Name] == null)
                        node.Attributes.Append((XmlAttribute)node.OwnerDocument.ImportNode(attribute, true));

                XmlNode ownSet = node["Set"];
                XmlNode templateSet = template["Set"];
                if (templateSet != null)
                {
                    if (ownSet == null) node.AppendChild(node.OwnerDocument.ImportNode(templateSet, true));
                    else foreach (XmlAttribute attribute in templateSet.Attributes)
                        if (ownSet.Attributes[attribute.Name] == null)
                            ownSet.Attributes.Append((XmlAttribute)node.OwnerDocument.ImportNode(attribute, true));
                }
                foreach (XmlNode trigger in template.SelectNodes("Trigger"))
                    node.AppendChild(node.OwnerDocument.ImportNode(trigger, true));
            }
        }
        visiting.Remove(name);
        compiled.Add(name);
    }
}

public static class GameUtils
{
    public static readonly PerkItems FDEJIIDIPBI = new PerkItems();
}

public sealed class ForgeManager
{
    private static readonly ForgeManager Instance = new ForgeManager();
    private readonly HashSet<string> _external = new HashSet<string>(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _metadata = new Dictionary<string, string>(StringComparer.Ordinal);
    private readonly List<Recipe> _recipes = new List<Recipe>();

    private ForgeManager()
    {
        _recipes.Add(new Recipe("Simple"));
        _recipes.Add(new Recipe("Medium"));
        _recipes.Add(new Recipe("Complex"));
    }

    public static ForgeManager ELEBLBJKDBI() { return Instance; }
    public IReadOnlyList<Recipe> Recipes { get { return _recipes.AsReadOnly(); } }

    public bool AddExternalRecipeFamily(string name, string alias, string economicProfileName,
        IReadOnlyList<ExternalRecipeItemSpec> items)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(economicProfileName) ||
            _recipes.Exists(value => string.Equals(value.Name, name, StringComparison.OrdinalIgnoreCase)) ||
            !_recipes.Exists(value => string.Equals(value.Name, economicProfileName, StringComparison.OrdinalIgnoreCase)))
            return false;
        _recipes.Add(new Recipe(name));
        return true;
    }

    public bool RemoveExternalRecipeFamily(string name)
    {
        Recipe recipe = _recipes.Find(value => string.Equals(value.Name, name, StringComparison.OrdinalIgnoreCase));
        if (recipe == null || recipe.Name == "Simple" || recipe.Name == "Medium" || recipe.Name == "Complex") return false;
        return _recipes.Remove(recipe);
    }

    public bool AddExternalPerkCandidate(string recipeName, string itemType, string perkName, string perkKind,
        int minLevel = int.MinValue, int maxLevel = int.MaxValue)
    {
        if (!_recipes.Exists(value => string.Equals(value.Name, recipeName, StringComparison.OrdinalIgnoreCase))) return false;
        return AddExternalEnchantmentCandidate(recipeName, itemType, perkName, string.Empty, perkKind);
    }

    public bool AddExternalEnchantmentCandidate(string recipeName, string itemType, string perkName,
        string enchantmentId, string perkKind, IReadOnlyDictionary<string, string> eclipseParameters = null)
    {
        string key = recipeName + "|" + itemType + "|" + perkName;
        if (!_external.Add(key)) return false;
        string parameters = string.Empty;
        if (eclipseParameters != null)
        {
            var names = new List<string>(eclipseParameters.Keys);
            names.Sort(StringComparer.Ordinal);
            foreach (string name in names) parameters += "|" + name + "=" + eclipseParameters[name];
        }
        _metadata[key] = enchantmentId + "|" + perkKind + parameters;
        return true;
    }

    public bool RemoveExternalEnchantmentCandidate(string recipeName, string itemType, string perkName)
    {
        string key = recipeName + "|" + itemType + "|" + perkName;
        _metadata.Remove(key);
        return _external.Remove(key);
    }

    public bool HasExternalEnchantmentCandidate(string recipeName, string itemType, string perkName)
    {
        return _external.Contains(recipeName + "|" + itemType + "|" + perkName);
    }

    public bool HasExternalEnchantmentMetadata(string recipeName, string itemType, string perkName,
        string enchantmentId, string perkKind)
    {
        string value;
        return _metadata.TryGetValue(recipeName + "|" + itemType + "|" + perkName, out value) &&
            value.StartsWith(enchantmentId + "|" + perkKind, StringComparison.Ordinal);
    }

    public bool HasExternalEnchantmentParameter(string recipeName, string itemType, string perkName,
        string parameterName, string parameterValue)
    {
        string value;
        return _metadata.TryGetValue(recipeName + "|" + itemType + "|" + perkName, out value) &&
            value.Contains("|" + parameterName + "=" + parameterValue);
    }
}

public sealed class Recipe
{
    public string Name { get; private set; }
    public Recipe(string name) { Name = name ?? string.Empty; }
}

public sealed class ExternalRecipeItemSpec
{
    public string ItemType { get; private set; }
    public int EnchantmentsNumber { get; private set; }
    public string BarScale { get; private set; }
    public int MinDeviation { get; private set; }
    public int MaxDeviation { get; private set; }
    public bool RandomAspect { get; private set; }

    public ExternalRecipeItemSpec(string itemType, int enchantmentsNumber, string barScale,
        int minDeviation, int maxDeviation, bool randomAspect)
    {
        ItemType = itemType ?? string.Empty;
        EnchantmentsNumber = enchantmentsNumber;
        BarScale = barScale ?? string.Empty;
        MinDeviation = minDeviation;
        MaxDeviation = maxDeviation;
        RandomAspect = randomAspect;
    }
}

public sealed class Battle
{
    private XmlNode _source;

    public XmlNode CloneSourceDefinitionForModding()
    {
        return _source == null ? null : _source.CloneNode(true);
    }

    public bool ReplaceSourceDefinitionForModding(XmlNode source, out string error)
    {
        error = string.Empty;
        _source = source == null ? null : source.CloneNode(true);
        return true;
    }

    public bool RestoreSourceDefinitionForModding(XmlNode source, out string error)
    {
        return ReplaceSourceDefinitionForModding(source, out error);
    }

    public void SetSourceForTest(XmlNode source) { _source = source == null ? null : source.CloneNode(true); }
}

public sealed class QuestStage
{
    public readonly XmlNode Node;
    public readonly string Owner;
    public QuestStage(XmlNode node, string owner)
    {
        Node = node == null ? null : node.CloneNode(true);
        Owner = owner ?? string.Empty;
    }
}

public sealed class PerkTree
{
    public enum AAAIBJGLPAI
    {
        TYPE_PERK,
        TYPE_UPGRADE,
    }

    public sealed class PerkItem
    {
        public readonly AAAIBJGLPAI Type;
        public readonly string Name;
        public readonly int Level;
        public PerkItem(AAAIBJGLPAI type, string name, int level)
        {
            Type = type;
            Name = name;
            Level = level;
        }
    }

    public sealed class PerkBranch
    {
        public readonly int Level;
        public readonly List<PerkItem> Items;
        public PerkBranch(int level, IEnumerable<PerkItem> items)
        {
            Level = level;
            Items = items == null ? new List<PerkItem>() : new List<PerkItem>(items);
        }
    }

    private static readonly PerkTree Instance = new PerkTree();
    private readonly Dictionary<int, PerkBranch> _branches = new Dictionary<int, PerkBranch>();

    public static PerkTree GBPBIPFIOJH() { return Instance; }

    public PerkBranch ReplaceExternalBranch(int level, IEnumerable<PerkItem> items)
    {
        PerkBranch previous;
        _branches.TryGetValue(level, out previous);
        _branches[level] = new PerkBranch(level, items);
        return previous;
    }

    public void RestoreExternalBranch(int level, PerkBranch previous)
    {
        if (previous == null) _branches.Remove(level);
        else _branches[level] = previous;
    }
}

public sealed class ListSF
{
    private static Items _items = new Items();
    private static readonly ListSF Instance = new ListSF();
    private readonly Dictionary<string, Battle> _battles = new Dictionary<string, Battle>(StringComparer.Ordinal);
    private readonly List<QuestStage> _quests = new List<QuestStage>();
    public static Items DJBOFEEKJMP() { return _items; }
    public static ListSF ELEBLBJKDBI() { return Instance; }
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

    public void AddExternalZone(XmlNode node) { }
    public bool RemoveExternalZone(string name) { return true; }
    public void AddExternalBattle(string zoneName, XmlNode node)
    {
        string battleName = node?.Attributes?["Name"]?.Value ?? string.Empty;
        var battle = new Battle();
        battle.SetSourceForTest(node);
        _battles[zoneName + "|" + battleName] = battle;
    }
    public bool RemoveExternalBattle(string zoneName, string battleName)
    {
        return _battles.Remove(zoneName + "|" + battleName);
    }
    public Battle FindBattleForModding(string zoneName, string battleName)
    {
        Battle battle;
        return _battles.TryGetValue(zoneName + "|" + battleName, out battle) ? battle : null;
    }
    public QuestStage AddExternalQuest(XmlNode node, string owner)
    {
        var quest = new QuestStage(node, owner);
        _quests.Add(quest);
        return quest;
    }
    public bool RemoveExternalQuest(QuestStage quest) { return quest != null && _quests.Remove(quest); }
}

public static class LocalizationManager
{
    public sealed class Language
    {
        public string name;
        public string EOMNCDDELLB;
        public int index;

        public Language(XmlNode node, int languageIndex)
        {
            name = node?.Attributes?["Name"]?.Value ?? string.Empty;
            EOMNCDDELLB = node?.Attributes?["Locale"]?.Value ?? string.Empty;
            index = languageIndex;
        }

        public Language(string languageName)
        {
            name = languageName ?? string.Empty;
            EOMNCDDELLB = languageName ?? string.Empty;
        }
    }

    private static readonly Dictionary<string, string> BaseStrings = new Dictionary<string, string>();
    private static readonly Dictionary<string, string> ExternalStrings = new Dictionary<string, string>();
    public static string POIPGLLCCKC = "eng";
    public static List<Language> MCLNNPPCFFL = new List<Language> { new Language("eng") };
    public static Language ILAJKOBCHFH = MCLNNPPCFFL[0];
    public static event Action OCLBJLPOKLB;

    public static Language NLFKNPBICED(string name)
    {
        return MCLNNPPCFFL.Find(value => string.Equals(value.name, name, StringComparison.Ordinal));
    }

    public static Language HHKANICOAAG(string locale)
    {
        return MCLNNPPCFFL.Find(value => string.Equals(value.EOMNCDDELLB, locale, StringComparison.OrdinalIgnoreCase));
    }

    public static void SetExternalString(string key, string value) { ExternalStrings[key] = value; }
    public static void RemoveExternalString(string key) { ExternalStrings.Remove(key); }

    public static string GetExternalStringForTest(string key)
    {
        string value;
        return ExternalStrings.TryGetValue(key, out value) ? value : null;
    }

    public static void SetBaseStringForTest(string key, string value)
    {
        BaseStrings[key] = value;
    }

    public static string GetStringForTest(string key)
    {
        string value;
        if (ExternalStrings.TryGetValue(key, out value)) return value;
        return BaseStrings.TryGetValue(key, out value) ? value : null;
    }

    public static void ResetModdingTestLanguage(string language)
    {
        BaseStrings.Clear();
        ExternalStrings.Clear();
        POIPGLLCCKC = language;
        MCLNNPPCFFL = new List<Language> { new Language(language) };
        ILAJKOBCHFH = MCLNNPPCFFL[0];
    }

    public static void ChangeModdingTestLanguage(string language)
    {
        BaseStrings.Clear();
        ExternalStrings.Clear();
        MCLNNPPCFFL = new List<Language> { new Language(language) };
        ILAJKOBCHFH = MCLNNPPCFFL[0];
        Action changed = OCLBJLPOKLB;
        if (changed != null) changed();
    }
}
