$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root ('Temp/ShowcaseEditorRegression-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
function Read-Method($source, $name) {
    $match = [regex]::Match($source, '(?ms)^\t(?:private|public) [^\r\n]*\b' + $name + '\(.*?^\t\}')
    if (!$match.Success) { throw "Cannot extract production method $name" }
    return $match.Value
}
$ai = Get-Content -Raw (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ModelAi.cs')
$quest = Get-Content -Raw (Join-Path $root 'Assets/Scripts/Assembly-CSharp/QuestCondition.cs')
$localization = Get-Content -Raw (Join-Path $root 'Assets/Scripts/Assembly-CSharp/LocalizationManager.cs')
$formatMethods = (Read-Method $localization 'CAKFDPGBLLG') + (Read-Method $localization 'GetWordEndSymbol')
$location = Get-Content -Raw (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Location.cs')
$locationPath = Read-Method $location 'LKDJCCIJFMD'
$labelSource = Get-Content -Raw (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Nekki/SF2/GUI/LabelAlias.cs')
$labelSetter = [regex]::Match($labelSource, '(?ms)^\t\tpublic void SetAlias\(.*?^\t\t\}').Value
if (!$labelSetter) { throw 'Cannot extract LabelAlias.SetAlias.' }
$aiMethods = (Read-Method $ai 'IsPlayableAnimations') + (Read-Method $ai 'IsTacticPlayableAnimations')
$questMethods = (Read-Method $quest 'KGCPIDICOJB') + (Read-Method $quest 'NumberCompare') +
    (Read-Method $quest 'StringCompare') + (Read-Method $quest 'OJHJEMKMDCP') + (Read-Method $quest 'LPBPJBCIGCO')
$enum = [regex]::Match($quest, '(?ms)^\tpublic enum NFFNINLIPJJ.*?^\t\}').Value
$code = @'
using System;
using System.Collections.Generic;
using System.Xml;
using Eclipse.Content;
public static class SystemProperties {
    public static bool DBBOCENKMGD() => false;
    public static VersionContainer DFJEJKJECBI() => new VersionContainer("2.41.9");
    public static VersionContainer KCJMMIEBLHL() => new VersionContainer("2.41.9");
}
namespace UnityEngine { public static class Time { public static int frameCount; } public static class Debug { public static void LogWarning(object message) {} } }
public static class LLLOJBFMONN { public static void Error(string message) { throw new Exception(message); } }
public sealed class QuestParameters {}
public class QuestAction { public string EFJMDEMAGIM; public virtual void DEJMHFMLKIC(QuestParameters p) {} public void OGIJONMKABB() {} }
public sealed class QuestActionOpenUrl : QuestAction {}
public sealed class QuestActionSwitchToRaidsMap : QuestAction {}
public static class GameCenterController { public static bool OBDJPKOJADA() => false; }
public sealed class ConditionKeys {}
public sealed class LocationPathFixture {
    /* LOCATION */
    public string Resolve(string path) => LKDJCCIJFMD(path);
}
public static class FormatFixture {
    /* FORMAT */
    public static string Format(string value) => CAKFDPGBLLG(value);
    static string GetString(string key) => key == "long" ? "a much longer translation" : key == "short" ? "x" : "%%ERROR%%";
}
public sealed class LabelFixture {
    string _Alias = ""; public string Text = "";
    void set_text(string value) { Text = value; }
    void OCLBJLPOKLB() { if (_Alias != "") Text = _Alias; }
    /* LABEL */
}
public sealed class EventAnimation { public enum EECEJKADLCK { EVENT_KEY_PRESSED } }
public sealed class ModelConditions { public bool IDCHHGHAENM; public int PDKPGKPBBIL, PCAOCHAIBJC, FOIHIKCEBJF; }
public sealed class ModelAnimation { public int KFCNPADAMHA() => 1; }
public sealed class Model {
    public List<InfoAnimation> Moves = new List<InfoAnimation>();
    public List<InfoAnimation> MCFPDHOLNGB() => Moves;
    public ModelConditions EBABHGHPLFK() => new ModelConditions();
}
public sealed class InfoAnimation {
    public sealed class Inside { public Inside ILOEBFFAEAN => this; public int OLBDPMKCJIF; public object NIDNJFOGBFO; }
    public sealed class CapabilityTable { public List<InfoAnimation> NINJLLDJLFI = new List<InfoAnimation>(); }
    public ConditionKeys Keys;
    public bool Allowed = true;
    public Inside ODACDCDONJE = new Inside();
    public CapabilityTable ICANLHJKKNE = new CapabilityTable();
    public ConditionKeys ILBCHANCOBP() => Keys;
    public int FOLOOGCLPNE() => 0;
    public int CEDEDCLGJDE(ModelConditions c, int direction) => 0;
    public EventAnimation OIGBIFNICBI(EventAnimation.EECEJKADLCK e) => null;
    public bool HPPGNJJCEGF(object model, object conditions, EventAnimation e) => Allowed;
}
public sealed class AiFixture {
    public Model _Model = new Model();
    public ModelAnimation _ModelAnimation = new ModelAnimation();
    public Model get_Model() => _Model;
    /* AI */
    public bool Normal(InfoAnimation move) => IsPlayableAnimations(move);
    public bool Tactical(InfoAnimation move) => IsTacticPlayableAnimations(move);
}
public sealed class QuestFixture {
    /* ENUM */
    public bool _compareVersions;
    public NFFNINLIPJJ LFLGCDNKNJI;
    public sealed class QuestFunctions { public string HBDLDIKHFEG; }
    public sealed class CompareResult {
        public string resultSTR = ""; public double resultNumber;
        public bool INCOIAANDCO() => resultSTR == "";
        public override string ToString() => INCOIAANDCO() ? resultNumber.ToString() : resultSTR;
    }
    /* QUEST */
    public bool Compare(string left, string right) => KGCPIDICOJB(new CompareResult { resultSTR = left }, new CompareResult { resultSTR = right });
    public string Version(bool data) {
        var result = new CompareResult(); var function = new QuestFunctions { HBDLDIKHFEG = "Version" };
        if (data) OJHJEMKMDCP(function, result); else LPBPJBCIGCO(function, result);
        return result.resultSTR;
    }
}
public static class Program {
    static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    public static void Main(string[] args) {
        Check(FormatFixture.Format("Grants 25% magic charge.") == "Grants 25% magic charge.", "Literal percentage crashed or changed.");
        Check(FormatFixture.Format("25%") == "25%", "Trailing percentage crashed.");
        Check(FormatFixture.Format("25%% and %short %long") == "25% and x a much longer translation", "Escaping/multiple aliases used stale offsets.");
        Check(FormatFixture.Format("%long %short") == "a much longer translation x", "Longer replacement broke subsequent alias.");
        var location = new LocationPathFixture();
        Check(location.Resolve("core:Textures/Locations/battlefield") == "core:Textures/Locations/battlefield", "Core arena path gained a Textures prefix.");
        Check(location.Resolve("example.phase1:sprites") == "example.phase1:sprites" && location.Resolve("Locations/dojo") == "Textures/Locations/dojo", "Location path compatibility changed.");
        var label = new LabelFixture(); label.SetAlias("Opening Focus description"); label.SetAlias("");
        Check(label.Text == "", "Empty description reused the previous perk's text.");
        label.SetAlias("Another enchantment"); label.SetAlias(null);
        Check(label.Text == "", "Null description retained stale text.");
        var ai = new AiFixture();
        var opening = new InfoAnimation(); var keyed = new InfoAnimation { Keys = new ConditionKeys() };
        ai._Model.Moves.Add(opening); ai._Model.Moves.Add(keyed);
        Check(!ai.Normal(opening) && !ai.Tactical(opening), "Event-only move entered the keyboard AI path.");
        Check(ai.Normal(keyed) && ai.Tactical(keyed), "Ordinary keyed movement was disabled.");
        keyed.ICANLHJKKNE.NINJLLDJLFI.Add(opening);
        Check(ai.Normal(keyed), "Event-only override suppressed ordinary keyed movement.");
        keyed.Allowed = false;
        Check(!ai.Normal(keyed) && !ai.Tactical(keyed), "Existing animation eligibility was bypassed.");
        var quest = new QuestFixture { _compareVersions = true, LFLGCDNKNJI = QuestFixture.NFFNINLIPJJ.QUEST_CONDITION_GREATER_EQUAL };
        Check(quest.Version(true) == "2.41.9" && quest.Version(false) == "2.41.9", "Full version property failed.");
        Check(quest.Compare("2.41.9", "2.0.0") && quest.Compare("2.10.0", "2.9.0") &&
            !quest.Compare("1.6.0", "2.0.0") && quest.Compare("2.0.0", "2.0.0"), "Version comparison is not numeric component order.");
        quest._compareVersions = false;
        Check(!quest.Compare("2.41.9", "2.0.0"), "Ordinary string comparison behavior changed.");
        var document = new XmlDocument(); document.Load(args[0]);
        QuestCompatibility.NormalizeActions(document);
        var dialog = document.SelectSingleNode("//Quest[@Name='EnergyRefillAdvertisingSupport']/Actions/Dialog");
        Check(dialog.SelectSingleNode("Button[@Type='Close']") == null &&
            dialog.SelectSingleNode("Button[@Type='Middle' and @Text='CANCEL']") != null &&
            dialog.SelectSingleNode("Button[@Type='Left']/Activate") != null &&
            dialog.SelectSingleNode("Button[@Type='Right']/BuyItem") != null, "Energy dialog lost dismissal or offer actions.");
        string once = document.OuterXml; QuestCompatibility.NormalizeActions(document);
        Check(document.OuterXml == once, "Dialog normalization is not idempotent.");
        Console.WriteLine("PASS: event-only AI exclusion, keyed move eligibility, version properties/order, energy dialog dismissal and idempotence.");
    }
}
'@
$code.Replace('/* AI */', $aiMethods).Replace('/* QUEST */', $questMethods).Replace('/* ENUM */', $enum).Replace('/* LABEL */', $labelSetter).Replace('/* FORMAT */', $formatMethods).Replace('/* LOCATION */', $locationPath) |
    Set-Content -Encoding UTF8 (Join-Path $fixture 'Program.cs')
Copy-Item -LiteralPath (Join-Path $root 'Assets/Plugins/Assembly-CSharp-firstpass/VersionContainer.cs') -Destination $fixture
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Content/QuestCompatibility.cs') -Destination $fixture
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding/AssetId.cs') -Destination $fixture
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding/ModId.cs') -Destination $fixture
@'
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><Nullable>disable</Nullable></PropertyGroup></Project>
'@ | Set-Content -Encoding UTF8 (Join-Path $fixture 'Regression.csproj')
dotnet run --project (Join-Path $fixture 'Regression.csproj') -- (Join-Path $root 'Assets/vanillaXml/quest_extensions/energy.xml')
if ($LASTEXITCODE -ne 0) { throw "Editor regression fixture failed: $LASTEXITCODE" }
