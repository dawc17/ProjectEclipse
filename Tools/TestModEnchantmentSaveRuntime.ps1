$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$userItemSource = Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/UserItem.cs')
$perkStructPath = Join-Path $root 'Assets/Scripts/Assembly-CSharp/PerkStruct.cs'
$modIdPath = Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding/ModId.cs'
$definitionIdPath = Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding/DefinitionId.cs'

# Execute the real recovered enchantment parse/replacement/save methods with only unrelated
# combat, roster and expression services stubbed. Keeping these methods extracted from
# UserItem.cs makes this a regression test for the actual users.xml mutation path rather
# than a second implementation of the persistence rules.
$parseEnchantments = [regex]::Match($userItemSource,
    '(?ms)^\tprivate void CHBPLGEDGAC\(XmlNode node\).*?^\t\}').Value
$migrateKind = [regex]::Match($userItemSource,
    '(?ms)^\tprivate static void EnsureExternalEnchantmentKind\(XmlNode node, PerkInfoItem perk\).*?^\t\}').Value
$applyEnchantments = [regex]::Match($userItemSource,
    '(?ms)^\tpublic void GDBFNNLHPOB\(List<PerkStruct> HALHGEGADKA, int MPAGFAKIEJG, int MHNCENBCECJ\).*?^\t\}').Value
$removeSingle = [regex]::Match($userItemSource,
    '(?ms)^\tprivate void JNGJKFLJCML\(\).*?^\t\}').Value
$removeCombo = [regex]::Match($userItemSource,
    '(?ms)^\tprivate void AMCMLDINIOM\(\).*?^\t\}').Value
$removeSaved = [regex]::Match($userItemSource,
    '(?ms)^\tprivate void RemoveSavedExternalEnchantmentsByKind\(string kind\).*?^\t\}').Value
$isEclipseOwned = [regex]::Match($userItemSource,
    '(?ms)^\t+private static bool IsEclipseOwnedSavedEnchantment\(XmlNode node\).*?^\t+\}').Value

if (!$parseEnchantments -or !$migrateKind -or !$applyEnchantments -or !$removeSingle -or !$removeCombo -or
    !$removeSaved -or !$isEclipseOwned) {
    throw 'Could not extract recovered enchantment persistence methods from UserItem.cs.'
}

$testRoot = Join-Path $root 'Temp/ModEnchantmentSaveRuntime'
New-Item -ItemType Directory -Force -Path $testRoot | Out-Null

$code = @'
using System;
using System.Collections.Generic;
using System.Xml;

namespace Eclipse.Modding
{
    public static class ModEffectSaveData
    {
        public const string NodeName = "EclipseParams";
        public const string ParameterNodeName = "Param";
        public const string Format = "1";
    }
}

public static class XmlCompat
{
    public static string CIPOICEEIBK(this XmlAttribute attribute, string fallback = "")
    {
        return attribute == null ? fallback : attribute.Value;
    }

    public static XmlAttribute LLIKNHNLGJJ(this XmlNode node, string name)
    {
        XmlAttribute attribute = node.Attributes[name];
        if (attribute != null) return attribute;
        attribute = node.OwnerDocument.CreateAttribute(name);
        node.Attributes.Append(attribute);
        return attribute;
    }

    public static XmlNode ACBPMPMPKJJ(this XmlNode node, string name)
    {
        XmlElement child = node.OwnerDocument.CreateElement(name);
        node.AppendChild(child);
        return child;
    }

    public static XmlNode KDPLHGGPJHN(this XmlNode node, string name)
    {
        return node.ACBPMPMPKJJ(name);
    }

    public static XmlNode LJGLMGNAFHJ(this XmlNode node, string childName, string attributeName, string value)
    {
        if (node == null) return null;
        foreach (XmlNode child in node.ChildNodes)
            if (child.NodeType == XmlNodeType.Element && child.Name == childName &&
                child.Attributes?[attributeName]?.Value == value) return child;
        return null;
    }
}

public sealed class FunctionResult
{
    public string DCJLKCFKCOM;
}

public sealed class FunctionExtension
{
    public sealed class CallbackResult { public string DCJLKCFKCOM; }
    private string _value;
    public void Parse(string value) { _value = value; }
    public void PBPBNENGLPA(Action<CallbackResult> callback) { }
    public void DMPCFMACDJM(Action<CallbackResult> callback) { }
    public FunctionResult IBCPKBBAFNH() { return new FunctionResult { DCJLKCFKCOM = _value }; }
}

public sealed class PerkInfoItem
{
    public enum DNPGIEGCGKH { COMBO = 0, SINGLE = 1 }
    public string Name;
    public DNPGIEGCGKH LELHEEDNMBP;
    public void HJFEFJIEINN(FunctionExtension.CallbackResult value) { }
    public void OKPFNCJFLDL(FunctionExtension.CallbackResult value) { }
}

public sealed class PerkItems
{
    private readonly Dictionary<string, PerkInfoItem> _perks =
        new Dictionary<string, PerkInfoItem>(StringComparer.Ordinal);

    public void Add(string name, PerkInfoItem.DNPGIEGCGKH kind)
    {
        _perks[name] = new PerkInfoItem { Name = name, LELHEEDNMBP = kind };
    }

    public void Remove(string name) { _perks.Remove(name); }

    public PerkInfoItem ABAGJKMKCBA(string name)
    {
        PerkInfoItem value;
        return name != null && _perks.TryGetValue(name, out value) ? value : null;
    }
}

public static class GameUtils
{
    public static readonly PerkItems FDEJIIDIPBI = new PerkItems();
}

public static class ItemInfo
{
    public static PerkInfoItem APPAODDDDKI(XmlNode node)
    {
        string name = node?.Attributes?["Name"].CIPOICEEIBK(string.Empty);
        PerkInfoItem source = GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(name);
        if (source == null) return null;
        return new PerkInfoItem { Name = source.Name, LELHEEDNMBP = source.LELHEEDNMBP };
    }
}

public sealed class Roster
{
    public int MMIMAJCKFKL;
    public bool CLODDOOGDBB;
}

public static class ListSF
{
    private static readonly Roster Roster = new Roster();
    public static Roster CCDKHLAMKKO() { return Roster; }
}

public sealed class UserItem
{
    private readonly XmlNode _Node;
    private readonly bool JGPEOEDJMHH = true;
    private readonly List<PerkInfoItem> JCGBOOPPOLG = new List<PerkInfoItem>();
    public IReadOnlyList<PerkInfoItem> RuntimeEnchantments => JCGBOOPPOLG.AsReadOnly();
    public XmlNode Node => _Node;

    public UserItem(XmlNode node)
    {
        _Node = node;
        if (node?["Enchantments"] != null) CHBPLGEDGAC(node["Enchantments"]);
    }

    private void LEFIBJHJAOD(bool includeCombo = true, bool removeNodes = false)
    {
        JCGBOOPPOLG.Clear();
    }

    __PARSE_ENCHANTMENTS__
    __MIGRATE_KIND__
    __APPLY_ENCHANTMENTS__
    __REMOVE_SINGLE__
    __REMOVE_COMBO__
    __REMOVE_SAVED__
    __IS_ECLIPSE_OWNED__
}

public static class Program
{
    private const string ModPerk = "example.enchantment:perks/eclipse_lifesteal";
    private const string ModEnchantment = "example.enchantment:enchantments/eclipse_lifesteal_weapon";
    private const string VanillaPerk = "PERK_ITEM_SPECIAL_FRENZY_WEAPON";

    private static void Assert(bool value, string message)
    {
        if (!value) throw new Exception(message);
    }

    private static XmlElement Perk(XmlDocument document, string name, string kind = null,
        string enchantment = null, string aspect = null)
    {
        XmlElement perk = document.CreateElement("Perk");
        perk.SetAttribute("Name", name);
        if (kind != null) perk.SetAttribute(PerkStruct.EclipseKindAttribute, kind);
        if (enchantment != null) perk.SetAttribute(PerkStruct.EclipseEnchantmentAttribute, enchantment);
        if (aspect != null)
        {
            XmlElement set = document.CreateElement("Set");
            set.SetAttribute("Aspect", aspect);
            perk.AppendChild(set);
        }
        return perk;
    }

    private static UserItem Load(string xml)
    {
        var document = new XmlDocument();
        document.LoadXml(xml);
        return new UserItem(document.DocumentElement);
    }

    private static int CountPerks(XmlNode enchantments, string name)
    {
        int count = 0;
        if (enchantments == null) return 0;
        foreach (XmlNode child in enchantments.ChildNodes)
            if (child.NodeType == XmlNodeType.Element && child.Name == "Perk" &&
                child.Attributes?["Name"]?.Value == name) count++;
        return count;
    }

    public static int Main()
    {
        GameUtils.FDEJIIDIPBI.Add(ModPerk, PerkInfoItem.DNPGIEGCGKH.SINGLE);
        GameUtils.FDEJIIDIPBI.Add(VanillaPerk, PerkInfoItem.DNPGIEGCGKH.SINGLE);

        // New forge candidates carry a stable public enchantment identity separately from
        // the legacy runtime perk Name. GDBFNNLHPOB must persist both plus the evaluated Set.
        var candidateDocument = new XmlDocument();
        XmlElement candidateNode = Perk(candidateDocument, ModPerk, "Single", ModEnchantment, "321");
        candidateNode.SetAttribute("ItemType", "Weapon");
        XmlElement parametersNode = candidateDocument.CreateElement(Eclipse.Modding.ModEffectSaveData.NodeName);
        parametersNode.SetAttribute("Format", Eclipse.Modding.ModEffectSaveData.Format);
        XmlElement chanceNode = candidateDocument.CreateElement(Eclipse.Modding.ModEffectSaveData.ParameterNodeName);
        chanceNode.SetAttribute("Name", "chance");
        chanceNode.SetAttribute("Value", "0.55");
        parametersNode.AppendChild(chanceNode);
        candidateNode.AppendChild(parametersNode);
        candidateDocument.AppendChild(candidateNode);
        var itemDocument = new XmlDocument();
        itemDocument.LoadXml("<Item Name='WEAPON_KATANA'/>");
        var item = new UserItem(itemDocument.DocumentElement);
        item.GDBFNNLHPOB(new List<PerkStruct> { new PerkStruct(candidateNode) }, 1200, 12);
        XmlElement saved = (XmlElement)item.Node["Enchantments"].FirstChild;
        Assert(saved.GetAttribute("Name") == ModPerk &&
            saved.GetAttribute(PerkStruct.EclipseEnchantmentAttribute) == ModEnchantment &&
            saved.GetAttribute(PerkStruct.EclipseKindAttribute) == "Single" &&
            saved["Set"]?.GetAttribute("Aspect") == "321" &&
            saved[Eclipse.Modding.ModEffectSaveData.NodeName]?
                .SelectSingleNode("Param[@Name='chance']")?.Attributes?["Value"]?.Value == "0.55",
            "External enchantment identity/kind/parameters were not persisted into users.xml.");

        string firstSavedPerk = saved.OuterXml;
        string firstSave = item.Node.OuterXml;
        var reloaded = Load(firstSave);
        Assert(reloaded.RuntimeEnchantments.Count == 1 && reloaded.RuntimeEnchantments[0].Name == ModPerk,
            "Installed mod enchantment did not resolve after a users.xml DOM reload.");

        // Removing the mod must hide the runtime effect while preserving its complete opaque node.
        GameUtils.FDEJIIDIPBI.Remove(ModPerk);
        var absent = Load(firstSave);
        string absentBeforeReplacement = absent.Node["Enchantments"].FirstChild.OuterXml;
        Assert(absent.RuntimeEnchantments.Count == 0 && absentBeforeReplacement == firstSavedPerk,
            "Missing mod enchantment was not kept as opaque save data.");

        // Preserve unrelated unknown data. Only Eclipse-tagged effects of the replaced kind
        // may be removed while the defining mod is absent.
        XmlElement opaque = absent.Node.OwnerDocument.CreateElement("Perk");
        opaque.SetAttribute("Name", "unknown.mod:perks/future");
        opaque.SetAttribute("Future", "preserve");
        absent.Node["Enchantments"].AppendChild(opaque);

        var vanillaCandidateDocument = new XmlDocument();
        XmlElement vanillaNode = Perk(vanillaCandidateDocument, VanillaPerk, null, null, "777");
        vanillaCandidateDocument.AppendChild(vanillaNode);
        absent.GDBFNNLHPOB(new List<PerkStruct> { new PerkStruct(vanillaNode) }, 1200, 12);
        XmlNode afterReplacement = absent.Node["Enchantments"];
        Assert(CountPerks(afterReplacement, ModPerk) == 0 && CountPerks(afterReplacement, VanillaPerk) == 1,
            "A missing external Single survived replacement by a later Single enchantment.");
        Assert(CountPerks(afterReplacement, "unknown.mod:perks/future") == 1 &&
            afterReplacement.SelectSingleNode("Perk[@Name='unknown.mod:perks/future']")?.Attributes?["Future"]?.Value == "preserve",
            "Replacement deleted or normalized unrelated opaque enchantment data.");

        // Reinstalling the old mod must not resurrect the replaced external enchantment.
        GameUtils.FDEJIIDIPBI.Add(ModPerk, PerkInfoItem.DNPGIEGCGKH.SINGLE);
        var restoredAfterReplacement = Load(absent.Node.OuterXml);
        Assert(CountPerks(restoredAfterReplacement.Node["Enchantments"], ModPerk) == 0 &&
            restoredAfterReplacement.RuntimeEnchantments.Count == 1 &&
            restoredAfterReplacement.RuntimeEnchantments[0].Name == VanillaPerk,
            "Reinstall resurrected an external enchantment that was replaced while its mod was absent.");

        // WIP 0.2 saves from before this metadata existed are migrated non-destructively when
        // the mod is available, so a later missing-mod replacement can still classify them.
        var legacy = Load("<Item><Enchantments><Perk Name='" + ModPerk + "'><Set Aspect='88'/></Perk>" +
            "<FutureNode Keep='yes'/></Enchantments></Item>");
        XmlElement legacyPerk = (XmlElement)legacy.Node["Enchantments"].FirstChild;
        Assert(legacyPerk.GetAttribute(PerkStruct.EclipseKindAttribute) == "Single" &&
            legacyPerk.GetAttribute(PerkStruct.EclipseEnchantmentAttribute) == string.Empty &&
            legacyPerk["Set"]?.GetAttribute("Aspect") == "88" &&
            legacy.Node["Enchantments"]["FutureNode"]?.Attributes?["Keep"]?.Value == "yes",
            "Legacy qualified perk migration changed data beyond adding its recoverable replacement kind.");

		var wrongKind = Load("<Item><Enchantments><Perk Name='" + ModPerk +
			"' EclipseKind='Combo'><Set Aspect='99'/></Perk></Enchantments></Item>");
		Assert(((XmlElement)wrongKind.Node["Enchantments"].FirstChild)
			.GetAttribute(PerkStruct.EclipseKindAttribute) == "Single",
			"Resolved external perk did not repair stale replacement-kind metadata.");

		const string NonPerkQualifiedName = "unrelated.mod:other/future";
		GameUtils.FDEJIIDIPBI.Add(NonPerkQualifiedName, PerkInfoItem.DNPGIEGCGKH.SINGLE);
		var nonPerk = Load("<Item><Enchantments><Perk Name='" + NonPerkQualifiedName +
			"'><Set Aspect='1'/></Perk></Enchantments></Item>");
		Assert(((XmlElement)nonPerk.Node["Enchantments"].FirstChild)
			.GetAttribute(PerkStruct.EclipseKindAttribute) == string.Empty,
			"Load migration tagged a namespaced runtime effect that is not an Eclipse perk definition ID.");

        GameUtils.FDEJIIDIPBI.Remove(ModPerk);
        var legacyAbsent = Load(legacy.Node.OuterXml);
        legacyAbsent.GDBFNNLHPOB(new List<PerkStruct> { new PerkStruct(vanillaNode) }, 1200, 12);
        Assert(CountPerks(legacyAbsent.Node["Enchantments"], ModPerk) == 0 &&
            legacyAbsent.Node["Enchantments"]["FutureNode"]?.Attributes?["Keep"]?.Value == "yes",
            "Migrated legacy external enchantment did not obey missing-mod replacement semantics.");

        Console.WriteLine("Mod enchantment save lifecycle: PASS (identity, reload, missing mod, replacement, reinstall, legacy migration, opaque preservation).");
        return 0;
    }
}
'@

$code = $code.Replace('__PARSE_ENCHANTMENTS__', $parseEnchantments)
$code = $code.Replace('__MIGRATE_KIND__', $migrateKind)
$code = $code.Replace('__APPLY_ENCHANTMENTS__', $applyEnchantments)
$code = $code.Replace('__REMOVE_SINGLE__', $removeSingle)
$code = $code.Replace('__REMOVE_COMBO__', $removeCombo)
$code = $code.Replace('__REMOVE_SAVED__', $removeSaved)
$code = $code.Replace('__IS_ECLIPSE_OWNED__', $isEclipseOwned)

$harness = Join-Path $testRoot 'Program.cs'
[IO.File]::WriteAllText($harness, $code, [Text.UTF8Encoding]::new($false))

$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$csc = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\Roslyn\csc.exe' |
    Select-Object -First 1
if (!$csc) { throw 'Could not locate the Visual Studio Roslyn compiler.' }

$exe = Join-Path $testRoot 'ModEnchantmentSaveRuntime.exe'
& $csc /nologo /langversion:9.0 /target:exe "/out:$exe" $modIdPath $definitionIdPath $perkStructPath $harness
if ($LASTEXITCODE -ne 0) { throw 'Mod enchantment save regression compilation failed.' }
& $exe
if ($LASTEXITCODE -ne 0) { throw 'Mod enchantment save regression failed.' }
