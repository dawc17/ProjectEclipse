using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

internal static class RewardGrantNativeTests
{
    private static int _assertions;

    private static void Assert(bool condition, string message)
    {
        _assertions++;
        if (!condition) throw new Exception(message);
    }

    private static RewardItem ParseRewardItem(string xml)
    {
        var document = new XmlDocument { XmlResolver = null };
        document.LoadXml(xml);
        return new RewardItem(document.DocumentElement);
    }

    private static ModDescriptor Mod(string id, params string[] dependencies)
    {
        var deps = dependencies.Select(value => new ModDependency(new ModId(value))).ToArray();
        return new ModDescriptor(new ModManifest(new ModId(id), deps));
    }

    private static void TestProjectionMarkers()
    {
        var catalog = new ModContentCatalog();
        var owner = new ModId("fixture");
        var item = DefinitionId.Parse("fixture:items/desolator");
        catalog.AddItem(new ItemDefinition(item, null));
        var rewardId = DefinitionId.Parse("fixture:rewards/titan");
        var plain = new RewardItemGrant(item);
        var configuredDirect = new RewardItemGrant(item, 2, _ => new RewardGrantConfiguration());
        var configuredChoice = new RewardItemGrant(item, 0, _ => new RewardGrantConfiguration());
        var reward = new RewardDefinition(rewardId,
            new[] { plain, configuredDirect },
            new[] { new RewardChoiceDefinition(new[] { new RewardChoiceItem(plain, 2.5f), new RewardChoiceItem(configuredChoice, 3.5f) }) });
        catalog.AddReward(reward);

        var projection = new RewardProjection(catalog);
        XmlElement root = projection.Build(reward);
        var items = root.SelectNodes("Item|Choice/Item").Cast<XmlElement>().ToArray();
        Assert(items.Length == 4, "Projection lost direct or weighted reward candidates.");
        Assert(!items[0].HasAttribute("EclipseReward"), "Unconfigured direct reward received an internal marker.");
        Assert(items[1].GetAttribute("EclipseReward") == rewardId.ToString() &&
            items[1].GetAttribute("EclipseGrant") == "1", "Configured direct reward flat index is wrong.");
        Assert(!items[2].HasAttribute("EclipseReward") && items[2].GetAttribute("Weight") == "2.5",
            "Unconfigured weighted reward changed marker or weight behavior.");
        Assert(items[3].GetAttribute("EclipseReward") == rewardId.ToString() &&
            items[3].GetAttribute("EclipseGrant") == "3" && items[3].GetAttribute("Weight") == "3.5",
            "Configured weighted reward flat index/weight is wrong.");
        Assert(reward.TryGetGrant(3, out var resolved) && ReferenceEquals(resolved, configuredChoice),
            "RewardDefinition flat lookup disagrees with adapter marker ordering.");
    }

    private static void TestRewardClone()
    {
        RewardItem source = ParseRewardItem("<Item Name='fixture:items/desolator' Drop='1' ShowReward='1' UpgradeNumber='2' Weight='4.25' Custom='kept' EclipseReward='fixture:rewards/titan' EclipseGrant='0'><Enchantments><Perk Name='old'><Set Aspect='10'/></Perk></Enchantments></Item>");
        RewardItem clone = source.CloneForConfiguredGrant(52, new[]
        {
            new RewardItem.ConfiguredGrantEnchantment("FRENZY", "3639.75", null),
            new RewardItem.ConfiguredGrantEnchantment("dep:perks/aura", null, "Combo")
        });
        Assert(source.LDLPCOFHFKE.Count == 1 && source.CMEFKONFDKN() == 0,
            "Configured clone mutated the shared source reward.");
        Assert(clone.CMEFKONFDKN() == 52 && clone.UpgradeNumber == 2 && clone.IDGKPLBKDIB && clone.GOOBKHECJIF,
            "Configured clone lost level, upgrade, drop, or presentation state.");
        Assert(clone.HasEclipseGrantConfiguration && clone.EclipseGrantIndex == 0,
            "Configured clone lost its internal reward marker.");
        Assert(clone.LDLPCOFHFKE.Count == 2 && clone.LDLPCOFHFKE[0].get_Name() == "FRENZY" &&
            clone.LDLPCOFHFKE[0].Pairs.Single().Value == "3639.75",
            "Configured clone did not retain the decimal aspect literal.");
        Assert(clone.LDLPCOFHFKE[1].Pairs.Count == 0 && clone.LDLPCOFHFKE[1].EclipseKind == "Combo",
            "Aspect-less external enchantment lost its kind or gained a synthetic aspect.");
    }

    private static void TestRuntimeBridge()
    {
        var owner = Mod("fixture", "dep");
        var dep = Mod("dep");
        var catalog = new ModContentCatalog();
        DefinitionId itemId = DefinitionId.Parse("fixture:items/desolator");
        catalog.AddItem(new ItemDefinition(itemId, null));
        DefinitionId corePerk = DefinitionId.Parse("core:perks/frenzy");
        DefinitionId depPerk = DefinitionId.Parse("dep:perks/aura");
        catalog.AddPerk(new PerkDefinition(corePerk, "FRENZY"));
        catalog.AddPerk(new PerkDefinition(depPerk, null, ModPerkKind.Combo));
        DefinitionId rewardId = DefinitionId.Parse("fixture:rewards/titan");
        int calls = 0, seenLevel = 0;
        var grant = new RewardItemGrant(itemId, 0, level =>
        {
            calls++; seenLevel = level;
            return new RewardGrantConfiguration(null, new[]
            {
                new RewardGrantEnchantment(corePerk, 3639.75),
                new RewardGrantEnchantment(depPerk)
            });
        });
        catalog.AddReward(new RewardDefinition(rewardId, new[] { grant }, Array.Empty<RewardChoiceDefinition>()));
        ModRuntime.SetFixtureScripts(new ModScriptSession(catalog, new[] { owner, dep }));
        RewardItem source = ParseRewardItem("<Item Name='fixture:items/desolator' Drop='1' EclipseReward='fixture:rewards/titan' EclipseGrant='0'/>");

        Assert(ModRuntime.TryConfigureRewardGrant(source, 52, out var configured, out var error), "Bridge failed: " + error);
        Assert(calls == 1 && seenLevel == 52 && configured.CMEFKONFDKN() == 52,
            "Bridge did not snapshot the pre-XP player level into a literal reward level.");
        Assert(configured.LDLPCOFHFKE.Count == 2 && configured.LDLPCOFHFKE[0].get_Name() == "FRENZY" &&
            configured.LDLPCOFHFKE[0].Pairs.Single().Value == "3639.75" &&
            configured.LDLPCOFHFKE[1].get_Name() == depPerk.ToString() && configured.LDLPCOFHFKE[1].EclipseKind == "Combo",
            "Bridge did not convert validated perk identities/aspects into native payloads.");
        Assert(source.CMEFKONFDKN() == 0 && source.LDLPCOFHFKE.Count == 0,
            "Bridge mutated the shared reward instance.");

        Assert(ModRuntime.TryConfigureRewardGrant(source, 53, out var second, out error) && second.CMEFKONFDKN() == 53 && calls == 2,
            "Configured reward cached stale callback state across grants.");

        var overrideGrant = new RewardItemGrant(itemId, 0, _ => new RewardGrantConfiguration(37));
        catalog.ReplaceReward(new RewardDefinition(rewardId, new[] { overrideGrant }, Array.Empty<RewardChoiceDefinition>()));
        Assert(ModRuntime.TryConfigureRewardGrant(source, 53, out var levelOverride, out error) && levelOverride.CMEFKONFDKN() == 37,
            "Explicit configured reward level did not override the snapshot.");

        var missing = DefinitionId.Parse("fixture:perks/missing");
        catalog.ReplaceReward(new RewardDefinition(rewardId, new[]
        {
            new RewardItemGrant(itemId, 0, _ => new RewardGrantConfiguration(null, new[] { new RewardGrantEnchantment(missing, 1.5) }))
        }, Array.Empty<RewardChoiceDefinition>()));
        Assert(!ModRuntime.TryConfigureRewardGrant(source, 52, out configured, out error) && configured == null && error.Contains("unavailable perk"),
            "Missing perk did not reject the entire configured grant.");

        DefinitionId foreignPerk = DefinitionId.Parse("other:perks/aura");
        catalog.AddPerk(new PerkDefinition(foreignPerk, null));
        catalog.ReplaceReward(new RewardDefinition(rewardId, new[]
        {
            new RewardItemGrant(itemId, 0, _ => new RewardGrantConfiguration(null, new[] { new RewardGrantEnchantment(foreignPerk) }))
        }, Array.Empty<RewardChoiceDefinition>()));
        Assert(!ModRuntime.TryConfigureRewardGrant(source, 52, out configured, out error) && error.Contains("active dependency"),
            "Undeclared cross-mod perk namespace was accepted.");

        catalog.ReplaceReward(new RewardDefinition(rewardId, new[]
        {
            new RewardItemGrant(itemId, 0, _ => throw new ObjectDisposedException("fixture callback"))
        }, Array.Empty<RewardChoiceDefinition>()));
        Assert(!ModRuntime.TryConfigureRewardGrant(source, 52, out configured, out error) && error.Contains("callback failed"),
            "Disposed/error callback did not reject the grant without materializing an item.");

        RewardItem mismatch = ParseRewardItem("<Item Name='wrong' EclipseReward='fixture:rewards/titan' EclipseGrant='0'/>");
        Assert(!ModRuntime.TryConfigureRewardGrant(mismatch, 52, out configured, out error) && error.Contains("does not match"),
            "Marker could configure a different native item identity.");
    }

    private static void TestFightResultHook()
    {
        var owner = Mod("fixture");
        var catalog = new ModContentCatalog();
        DefinitionId itemId = DefinitionId.Parse("fixture:items/desolator");
        DefinitionId perkId = DefinitionId.Parse("core:perks/frenzy");
        catalog.AddItem(new ItemDefinition(itemId, null));
        catalog.AddPerk(new PerkDefinition(perkId, "FRENZY"));
        DefinitionId rewardId = DefinitionId.Parse("fixture:rewards/titan");
        int calls = 0;
        catalog.AddReward(new RewardDefinition(rewardId, new[]
        {
            new RewardItemGrant(itemId, 0, level => { calls++; return new RewardGrantConfiguration(level, new[] { new RewardGrantEnchantment(perkId, 99.5) }); })
        }, Array.Empty<RewardChoiceDefinition>()));
        ModRuntime.SetFixtureScripts(new ModScriptSession(catalog, new[] { owner }));

        var nativeItem = new ItemInfo("fixture:items/desolator", "Weapon", 52);
        ListSF.Reset(52, nativeItem);
        var result = new FightResult.ResultPrizeStruct();
        RewardItem source = ParseRewardItem("<Item Name='fixture:items/desolator' Drop='1' EclipseReward='fixture:rewards/titan' EclipseGrant='0'/>");
        result.KFJABAMAKOD(source);
        Assert(calls == 1 && result.HELFDCAIJNE.Count == 1 && result.HELFDCAIJNE[0].NAIEGGHELIH.CMEFKONFDKN() == 52,
            "FightResult did not configure the reward before native level lookup.");
        Assert(result.HELFDCAIJNE[0].NAIEGGHELIH.LDLPCOFHFKE.Single().Pairs.Single().Value == "99.5",
            "FightResult prize lost configured enchantment payload.");

        ListSF.Current.Inventory.Owned["fixture:items/desolator"] = new UserItem();
        result = new FightResult.ResultPrizeStruct();
        result.KFJABAMAKOD(source);
        Assert(calls == 1 && result.HELFDCAIJNE.Count == 0, "Owned-item skip invoked configured callback or granted a duplicate.");

        ListSF.Current.Inventory.Owned.Clear();
        ListSF.CurrentItems.ByName.Clear();
        result = new FightResult.ResultPrizeStruct();
        result.KFJABAMAKOD(source);
        Assert(calls == 1 && result.HELFDCAIJNE.Count == 0, "Missing-item skip invoked configured callback.");

        ListSF.CurrentItems.ByName[nativeItem.Name] = nativeItem;
        catalog.ReplaceReward(new RewardDefinition(rewardId, new[]
        {
            new RewardItemGrant(itemId, 0, _ => throw new Exception("boom"))
        }, Array.Empty<RewardChoiceDefinition>()));
        UnityEngine.Debug.Warnings.Clear();
        result = new FightResult.ResultPrizeStruct();
        result.KFJABAMAKOD(source);
        Assert(result.HELFDCAIJNE.Count == 0 && UnityEngine.Debug.Warnings.Count == 1,
            "Callback failure partially granted a prize or failed to warn.");

        catalog.ReplaceReward(new RewardDefinition(rewardId, new[]
        {
            new RewardItemGrant(itemId, 0, _ => new RewardGrantConfiguration(53))
        }, Array.Empty<RewardChoiceDefinition>()));
        nativeItem.MissingUpdateLevel = 53;
        UnityEngine.Debug.Warnings.Clear();
        result = new FightResult.ResultPrizeStruct();
        result.KFJABAMAKOD(source);
        Assert(result.HELFDCAIJNE.Count == 0 && UnityEngine.Debug.Warnings.Count == 1 &&
            UnityEngine.Debug.Warnings[0].Contains("exact level 53 is unavailable"),
            "Configured level silently downgraded to the native base item.");
    }

    private static void TestFinalTitanProjection(string repositoryRoot)
    {
        var catalog = new ModContentCatalog();
        DefinitionId itemId = DefinitionId.Parse("de128:items/weapon/titans_desolator");
        catalog.AddItem(new ItemDefinition(itemId, null));
        DefinitionId rewardId = DefinitionId.Parse("de128:rewards/titans_desolator");
        var reward = new RewardDefinition(rewardId, new[]
        {
            new RewardItemGrant(itemId, 0, _ => new RewardGrantConfiguration(52))
        }, Array.Empty<RewardChoiceDefinition>());
        catalog.AddReward(reward);
        var adapter = new RewardProjection(catalog);
        var stages = new XmlDocument { XmlResolver = null };
        stages.Load(System.IO.Path.Combine(repositoryRoot, "Assets", "vanillaXml", "stages.xml"));
        var canonicalFight = (XmlElement)stages.SelectSingleNode(
            "/Stages/Zones/Zone[@Name='ZONE_7']/Battle[@Name='C3_BOSS_TITAN_ECLIPSEMODE']/Fight[@Name='6']");
        Assert(canonicalFight != null, "Canonical Eclipse Titan fight 6 was not found.");
        var document = new XmlDocument { XmlResolver = null };
        document.AppendChild(document.ImportNode(canonicalFight, true));
        string slot0Before = ((XmlElement)document.SelectSingleNode("/Fight/Rewards/Reward[1]")).OuterXml;
        var resultBefore = (XmlElement)document.SelectSingleNode("/Fight/Rewards/Reward[2]");
        string resultAttributesBefore = string.Join("|", resultBefore.Attributes.Cast<XmlAttribute>().Select(a => a.Name + "=" + a.Value));
        string currenciesBefore = string.Concat(resultBefore.SelectNodes("EclipseModeReward/Currency").Cast<XmlNode>().Select(n => n.OuterXml));
        ModRewardDropProjectionFixture.Apply(document.DocumentElement, 1, ModRuleMode.Eclipse, null, null, reward,
            value => adapter.Build(value));
        var result = (XmlElement)document.SelectSingleNode("/Fight/Rewards/Reward[2]");
        Assert(((XmlElement)document.SelectSingleNode("/Fight/Rewards/Reward[1]")).OuterXml == slot0Before,
            "Eclipse Titan zero-win reward slot changed.");
        Assert(string.Join("|", result.Attributes.Cast<XmlAttribute>().Select(a => a.Name + "=" + a.Value)) == resultAttributesBefore,
            "Eclipse Titan target reward attributes changed.");
        var eclipseMode = (XmlElement)result.SelectSingleNode("EclipseModeReward");
        string currenciesAfter = string.Concat(eclipseMode.SelectNodes("Currency").Cast<XmlNode>().Select(n => n.OuterXml));
        Assert(currenciesBefore == currenciesAfter && eclipseMode.SelectNodes("Currency").Count == 2 &&
            eclipseMode.SelectSingleNode("Currency[@Name='ForgeMaterial1'][@ExpectedValue='27216']") != null &&
            eclipseMode.SelectSingleNode("Currency[@Name='ForgeMaterial2'][@ExpectedValue='23862']") != null,
            "Eclipse Titan currency payload changed.");
        var item = (XmlElement)eclipseMode.SelectSingleNode("Item");
        Assert(item != null && item.GetAttribute("Name") == itemId.ToString() &&
            item.GetAttribute("EclipseReward") == rewardId.ToString() && item.GetAttribute("EclipseGrant") == "0",
            "Eclipse Titan projection did not add the configured DE128 grant under EclipseModeReward.");
    }

    private static void TestNativeEnchantmentSerialization()
    {
        ListSF.Reset(52, new ItemInfo("fixture:items/desolator", "Weapon", 52));
        GameUtils.FDEJIIDIPBI.Register("FRENZY", PerkInfoItem.DNPGIEGCGKH.SINGLE);
        RewardItem reward = ParseRewardItem("<Item Name='fixture:items/desolator'><Enchantments><Perk Name='FRENZY'><Set Aspect='3639.75'/></Perk></Enchantments></Item>");
        var user = new UserItem();
        user.GDBFNNLHPOB(reward.LDLPCOFHFKE, 52, 52);
        XmlNode saved = user.Node.SelectSingleNode("Enchantments/Perk[@Name='FRENZY']/Set");
        Assert(saved != null && saved.Attributes["Aspect"].Value == "3639.75",
            "Recovered UserItem enchantment serialization changed the decimal aspect literal.");
    }

    public static int Main(string[] args)
    {
        string repositoryRoot = args.Length == 0 ? throw new ArgumentException("Repository root is required.") : args[0];
        TestProjectionMarkers();
        TestRewardClone();
        TestRuntimeBridge();
        TestFightResultHook();
        TestFinalTitanProjection(repositoryRoot);
        TestNativeEnchantmentSerialization();
        Console.WriteLine("Reward grant native PASS: " + _assertions + " assertions; markers, direct/weighted lookup, fresh configuration, dependency/perk validation, native prize hook, failure atomicity, decimal aspect serialization.");
        return 0;
    }
}

public static class XmlFixtureExtensions
{
    public static string CIPOICEEIBK(this XmlAttribute attribute, string fallback) => attribute == null ? fallback : attribute.Value;
    public static uint ParseUint(this XmlAttribute attribute) => attribute == null || string.IsNullOrEmpty(attribute.Value) ? 0u : uint.Parse(attribute.Value, CultureInfo.InvariantCulture);
    public static bool ParseBool(this XmlAttribute attribute) => attribute != null && (attribute.Value == "1" || bool.TryParse(attribute.Value, out var value) && value);
    public static int ToInt(this string value) => string.IsNullOrEmpty(value) ? 0 : (int)double.Parse(value, CultureInfo.InvariantCulture);
    public static XmlNode ACBPMPMPKJJ(this XmlNode node, string name) { var child = node.OwnerDocument.CreateElement(name); node.AppendChild(child); return child; }
    public static XmlNode KDPLHGGPJHN(this XmlNode node, string name) { var child = node.OwnerDocument.CreateElement(name); node.AppendChild(child); return child; }
    public static XmlAttribute LLIKNHNLGJJ(this XmlNode node, string name) { var element = (XmlElement)node; var attr = element.GetAttributeNode(name) ?? element.OwnerDocument.CreateAttribute(name); if (attr.OwnerElement == null) element.Attributes.Append(attr); return attr; }
}

public class Rewardable
{
    public enum GADCOGHCGDP { REWARD_NOTHING, REWARD_ITEM }
    public GADCOGHCGDP CLOGJMBMMPI;
    public bool IDGKPLBKDIB;
    public bool GOOBKHECJIF;
    public virtual void Parse(XmlNode node) { IDGKPLBKDIB = node.Attributes["Drop"].ParseBool(); GOOBKHECJIF = node.Attributes["ShowReward"].ParseBool(); }
}

public sealed class FunctionResult { public string DCJLKCFKCOM; }
public sealed class FunctionExtension
{
    private string _value;
    public sealed class CallbackResult { public object data; public FunctionResult NAGGNMIFFGK = new FunctionResult(); }
    public sealed class GLBAFLLMOOH { public string FJLOLCPJACB = ""; public string HBDLDIKHFEG = ""; }
    public void Parse(string value) { _value = value; }
    public void PBPBNENGLPA(Action<CallbackResult> callback) { }
    public void DMPCFMACDJM(Action<CallbackResult> callback) { }
    public FunctionResult IBCPKBBAFNH() => new FunctionResult { DCJLKCFKCOM = string.IsNullOrEmpty(_value) ? "0" : _value };
}

public sealed class PerkInfoItem
{
    public enum DNPGIEGCGKH { SINGLE, COMBO }
    public string Name;
    public DNPGIEGCGKH LELHEEDNMBP;
    public void HJFEFJIEINN(FunctionExtension.CallbackResult value) { }
    public void OKPFNCJFLDL(FunctionExtension.CallbackResult value) { }
}

public sealed class PerkItems
{
    private readonly Dictionary<string, PerkInfoItem> _items = new Dictionary<string, PerkInfoItem>(StringComparer.Ordinal);
    public void Register(string name, PerkInfoItem.DNPGIEGCGKH kind) => _items[name] = new PerkInfoItem { Name = name, LELHEEDNMBP = kind };
    public PerkInfoItem ABAGJKMKCBA(string name) => _items.TryGetValue(name, out var item) ? item : null;
}

public static class GameUtils { public static readonly PerkItems FDEJIIDIPBI = new PerkItems(); }

public partial class UserItem
{
    internal bool JGPEOEDJMHH = true;
    internal readonly List<PerkInfoItem> JCGBOOPPOLG = new List<PerkInfoItem>();
    internal readonly XmlElement _Node;
    public XmlElement Node => _Node;
    public UserItem() { var document = new XmlDocument { XmlResolver = null }; _Node = document.CreateElement("Item"); document.AppendChild(_Node); }
    private void JNGJKFLJCML() { }
    private void AMCMLDINIOM() { }
}

public sealed class ItemInfo
{
    public readonly string Name;
    public readonly string Type;
    public readonly int ItemLevel;
    public ItemInfo ParentItem;
    public int MissingUpdateLevel = -1;
    public ItemInfo(string name, string type, int level) { Name = name; Type = type; ItemLevel = level; }
    public ItemInfo GetUpdateItemByLevel(int level, bool ignored) => level == MissingUpdateLevel ? null : level == ItemLevel ? this : new ItemInfo(Name, Type, level);
    public ItemInfo HIOBANJPMKF(int level) => new ItemInfo(Name, Type, level);
    public List<UpgradeData> DNFDAGFAANJ(bool ignored, int level) => new List<UpgradeData>();
    public ItemInfo MPADIPJLMLH(UpgradeData data) => this;
    public static PerkInfoItem APPAODDDDKI(XmlNode node) => GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(node.Attributes?["Name"]?.Value);
}
public sealed class UpgradeData { }

public sealed class Items
{
    public readonly Dictionary<string, ItemInfo> ByName = new Dictionary<string, ItemInfo>(StringComparer.Ordinal);
    public ItemInfo GetItemByName(string name) => ByName.TryGetValue(name, out var item) ? item : null;
}
public sealed class UserItems
{
    public readonly Dictionary<string, UserItem> Owned = new Dictionary<string, UserItem>(StringComparer.Ordinal);
    public UserItem CMGOCLGHNLH(string name) => Owned.TryGetValue(name, out var item) ? item : null;
}
public sealed class Roster
{
    public readonly UserItems Inventory = new UserItems();
    public int Level { get; set; }
    public int MMIMAJCKFKL;
    public bool CLODDOOGDBB;
    public UserItems KHCNHPCPFII() => Inventory;
    public int PINDEKDNCNL() => Level;
}
public static class ListSF
{
    public static readonly Roster Current = new Roster();
    public static readonly Items CurrentItems = new Items();
    public static Roster CCDKHLAMKKO() => Current;
    public static Items GetItems() => CurrentItems;
    public static void Reset(int level, ItemInfo item) { Current.Level = level; Current.Inventory.Owned.Clear(); CurrentItems.ByName.Clear(); if (item != null) CurrentItems.ByName[item.Name] = item; }
}

public partial class FightResult
{
    public sealed class LJFFIBFBGID
    {
        public ItemInfo DLKPBAJDHBO;
        public RewardItem NAIEGGHELIH;
        public bool IDGKPLBKDIB;
    }
}

namespace UnityEngine
{
    public static class Debug
    {
        public static readonly List<string> Warnings = new List<string>();
        public static void LogWarning(object message) => Warnings.Add(message?.ToString() ?? string.Empty);
    }
}

namespace Eclipse.Modding
{
    public enum ModRuleMode { All, Normal, Eclipse }
    public enum ModPerkKind { Single, Combo }
    public sealed class ModContentException : Exception
    {
        public ModContentException(string message) : base(message) { }
    }

    public static class ModEffectSaveData { public const string NodeName = "EclipseParams"; public const string ParameterNodeName = "Param"; public const string Format = "1"; }

    public readonly struct ModId : IEquatable<ModId>
    {
        public string Value { get; }
        public ModId(string value) { Value = value ?? string.Empty; }
        public bool Equals(ModId other) => string.Equals(Value, other.Value, StringComparison.Ordinal);
        public override bool Equals(object obj) => obj is ModId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public static bool operator ==(ModId a, ModId b) => a.Equals(b);
        public static bool operator !=(ModId a, ModId b) => !a.Equals(b);
        public override string ToString() => Value;
    }

    public readonly struct DefinitionId : IEquatable<DefinitionId>
    {
        public ModId Namespace { get; }
        public string Category { get; }
        public string LocalId { get; }
        private DefinitionId(ModId ns, string category, string localId) { Namespace = ns; Category = category; LocalId = localId; }
        public static DefinitionId Parse(string text) { if (!TryParse(text, out var id)) throw new FormatException(text); return id; }
        public static bool TryParse(string text, out DefinitionId id)
        {
            id = default;
            if (string.IsNullOrEmpty(text)) return false;
            int colon = text.IndexOf(':'), slash = text.IndexOf('/', colon + 1);
            if (colon <= 0 || slash <= colon + 1 || slash == text.Length - 1) return false;
            id = new DefinitionId(new ModId(text.Substring(0, colon)), text.Substring(colon + 1, slash - colon - 1), text.Substring(slash + 1));
            return true;
        }
        public bool Equals(DefinitionId other) => Namespace == other.Namespace && Category == other.Category && LocalId == other.LocalId;
        public override bool Equals(object obj) => obj is DefinitionId other && Equals(other);
        public override int GetHashCode() => ToString().GetHashCode();
        public static bool operator ==(DefinitionId a, DefinitionId b) => a.Equals(b);
        public static bool operator !=(DefinitionId a, DefinitionId b) => !a.Equals(b);
        public override string ToString() => Namespace + ":" + Category + "/" + LocalId;
    }

    public class ItemDefinition
    {
        public DefinitionId Id { get; }
        public string LegacyName { get; }
        public bool IsCore => Id.Namespace.Value == "core";
        public ItemDefinition(DefinitionId id, string legacyName) { Id = id; LegacyName = legacyName; }
    }
    public class PerkDefinition
    {
        public DefinitionId Id { get; }
        public string LegacyName { get; }
        public ModPerkKind Kind { get; }
        public bool IsCore => Id.Namespace.Value == "core";
        public PerkDefinition(DefinitionId id, string legacyName, ModPerkKind kind = ModPerkKind.Single) { Id = id; LegacyName = legacyName; Kind = kind; }
    }
    public sealed class RewardGrantEnchantment
    {
        public DefinitionId Perk { get; }
        public double? Aspect { get; }
        public RewardGrantEnchantment(DefinitionId perk, double? aspect = null) { Perk = perk; Aspect = aspect; }
    }
    public sealed class RewardGrantConfiguration
    {
        public int? Level { get; }
        public IReadOnlyList<RewardGrantEnchantment> Enchantments { get; }
        public RewardGrantConfiguration(int? level = null, RewardGrantEnchantment[] enchantments = null) { Level = level; Enchantments = Array.AsReadOnly(enchantments ?? Array.Empty<RewardGrantEnchantment>()); }
    }
    public sealed class RewardItemGrant
    {
        public DefinitionId Item { get; }
        public uint UpgradeNumber { get; }
        public Func<int, RewardGrantConfiguration> Configure { get; }
        public bool UsesConfiguration => Configure != null;
        public RewardItemGrant(DefinitionId item, uint upgradeNumber = 0, Func<int, RewardGrantConfiguration> configure = null) { Item = item; UpgradeNumber = upgradeNumber; Configure = configure; }
    }
    public sealed class RewardChoiceItem { public RewardItemGrant Grant { get; } public float Weight { get; } public RewardChoiceItem(RewardItemGrant grant, float weight = 1) { Grant = grant; Weight = weight; } }
    public sealed class RewardChoiceDefinition { public IReadOnlyList<RewardChoiceItem> Items { get; } public RewardChoiceDefinition(RewardChoiceItem[] items) { Items = Array.AsReadOnly(items); } }
    public sealed class RewardDefinition
    {
        public DefinitionId Id { get; }
        public int Gems { get; }
        public IReadOnlyList<RewardItemGrant> Items { get; }
        public IReadOnlyList<RewardChoiceDefinition> Choices { get; }
        public RewardDefinition(DefinitionId id, RewardItemGrant[] items, RewardChoiceDefinition[] choices, int gems = 0) { Id = id; Items = Array.AsReadOnly(items); Choices = Array.AsReadOnly(choices); Gems = gems; }
        public bool TryGetGrant(int index, out RewardItemGrant grant) { grant = null; if (index < 0) return false; if (index < Items.Count) { grant = Items[index]; return true; } index -= Items.Count; foreach (var choice in Choices) { if (index < choice.Items.Count) { grant = choice.Items[index].Grant; return true; } index -= choice.Items.Count; } return false; }
    }
    public sealed class ModContentCatalog
    {
        private readonly Dictionary<string, ItemDefinition> _items = new Dictionary<string, ItemDefinition>(StringComparer.Ordinal);
        private readonly Dictionary<string, PerkDefinition> _perks = new Dictionary<string, PerkDefinition>(StringComparer.Ordinal);
        private readonly Dictionary<string, RewardDefinition> _rewards = new Dictionary<string, RewardDefinition>(StringComparer.Ordinal);
        public void AddItem(ItemDefinition value) => _items[value.Id.ToString()] = value;
        public void AddPerk(PerkDefinition value) => _perks[value.Id.ToString()] = value;
        public void AddReward(RewardDefinition value) => _rewards[value.Id.ToString()] = value;
        public void ReplaceReward(RewardDefinition value) => _rewards[value.Id.ToString()] = value;
        public bool TryResolveItem(DefinitionId id, out ItemDefinition value) => _items.TryGetValue(id.ToString(), out value);
        public bool TryGetPerk(DefinitionId id, out PerkDefinition value) => _perks.TryGetValue(id.ToString(), out value);
        public bool TryGetReward(DefinitionId id, out RewardDefinition value) => _rewards.TryGetValue(id.ToString(), out value);
    }
    public sealed class ModDependency { public ModId Id { get; } public ModDependency(ModId id) { Id = id; } }
    public sealed class ModManifest { public IReadOnlyList<ModDependency> Dependencies { get; } public ModManifest(ModId id, ModDependency[] dependencies) { Id = id; Dependencies = Array.AsReadOnly(dependencies); } public ModId Id { get; } }
    public sealed class ModDescriptor { public ModManifest Manifest { get; } public ModId Id => Manifest.Id; public ModDescriptor(ModManifest manifest) { Manifest = manifest; } }
    public sealed class ModScriptSession
    {
        public ModContentCatalog Content { get; }
        public IReadOnlyList<ModDescriptor> ActiveMods { get; }
        public ModScriptSession(ModContentCatalog content, ModDescriptor[] active) { Content = content; ActiveMods = Array.AsReadOnly(active); }
    }
}
