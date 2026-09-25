$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root 'Temp/TrialRules'
$modsRoot = Join-Path $fixture 'Mods'
$modRoot = Join-Path $modsRoot 'trial.fixture'
New-Item -ItemType Directory -Force (Join-Path $modRoot 'scripts') | Out-Null

@'
schema = 1
id = "trial.fixture"
name = "Trial Rules Fixture"
version = "1.0.0"
authors = ["Eclipse"]
entrypoint = "scripts/main.lua"
capabilities = ["content.register"]

[[dependencies]]
id = "core"
version = ">=1.0 <2.0"
'@ | Set-Content -Encoding UTF8 (Join-Path $modRoot 'mod.toml')

@'
local sf2 = require("sf2")
local perk = sf2.perks.get("core:perks/PERK_TEST_TRIAL")
sf2.rules.hot_ground {
    id = "hot", frames = 420, target = sf2.rules.PLAYER,
    nodes = {
        { name = "NToeTip_1", axis = "Y", max = 15 },
        { name = "NPivot", axis = "Y", min = -30, max = 30 },
    },
    animations = { "Jump", "ThrowFall" },
}
sf2.rules.ring_out { id = "ring", node = "NPivot", axis = "X", min = -600, max = 600, target = sf2.rules.PLAYER }
sf2.rules.regeneration { id = "regen", rate = 0.001, frames_after_hit = 180, target = sf2.rules.OPPONENT }
sf2.rules.no_animation { id = "no_jump", name = "Jump" }
sf2.rules.remove_interval { id = "no_block", type = "Block", target = sf2.rules.PLAYER }
sf2.rules.perk { id = "buff", perk = perk, aspect = 100000, target = sf2.rules.OPPONENT }
sf2.rules.light_in_the_darkness { id = "light", radius = 0.20, shape = 1, target = sf2.rules.PLAYER }
'@ | Set-Content -Encoding UTF8 (Join-Path $modRoot 'scripts/main.lua')

function Extract-Method([string]$source, [string]$signature) {
    $start = $source.IndexOf($signature, [StringComparison]::Ordinal)
    if ($start -lt 0) { throw "Cannot find production method: $signature" }
    $lineStart = $source.LastIndexOf("`n", $start); if ($lineStart -lt 0) { $lineStart = 0 } else { $lineStart++ }
    $brace = $source.IndexOf('{', $start); if ($brace -lt 0) { throw "Cannot find opening brace: $signature" }
    $depth = 0
    for ($i = $brace; $i -lt $source.Length; $i++) {
        if ($source[$i] -eq '{') { $depth++ }
        elseif ($source[$i] -eq '}') { $depth--; if ($depth -eq 0) { return $source.Substring($lineStart, $i - $lineStart + 1) } }
    }
    throw "Cannot find closing brace: $signature"
}

$adapter = Get-Content -Raw (Join-Path $root 'Assets/Scripts/Eclipse/Modding/LegacyContentAdapter.cs')
$buildRule = Extract-Method $adapter 'private XmlElement BuildRuleNode(XmlDocument document, FightRuleDefinition rule)'
$legacyItem = Extract-Method $adapter 'private string LegacyItemName(DefinitionId id)'
$legacyPerk = Extract-Method $adapter 'private string LegacyPerkName(DefinitionId id)'
$targetName = Extract-Method $adapter 'private static string RuleTargetName(ModRuleTarget target)'
$setIfNotEmpty = Extract-Method $adapter 'private static void SetIfNotEmpty(XmlElement node, string name, string value)'
$projection = Join-Path $fixture 'TrialRuleProjection.cs'
@"
using System; using System.Collections.Generic; using System.Globalization; using System.Xml; using Eclipse.Modding;
internal sealed class TrialRuleProjection {
    private readonly ModContentCatalog _content;
    internal TrialRuleProjection(ModContentCatalog content) { _content = content; }
    internal XmlElement Build(FightRuleDefinition rule) { return BuildRuleNode(new XmlDocument(), rule); }
$buildRule
$legacyItem
$legacyPerk
$targetName
$setIfNotEmpty
}
"@ | Set-Content -Encoding UTF8 $projection

$nativeStubs = Join-Path $fixture 'NativeStubs.cs'
@'
using System; using System.Collections.Generic; using System.Xml;
public class EventDispatcher<T> { }
public sealed class DeflatedString { private XmlNode _node; public void Set(XmlNode node){_node=node.CloneNode(true);} public XmlNode IOJIGDNFCFL()=>_node.CloneNode(true); }
public static class NativeXmlExt {
 public static bool Empty(this XmlAttribute a)=>a==null||string.IsNullOrEmpty(a.Value);
 public static string CIPOICEEIBK(this XmlAttribute a,string f)=>a==null?f:a.Value;
 public static float ParseFloat(this XmlAttribute a,float f=0)=>a==null?f:float.Parse(a.Value,System.Globalization.CultureInfo.InvariantCulture);
 public static bool ParseBool(this XmlAttribute a)=>a!=null&&(a.Value=="1"||string.Equals(a.Value,"true",StringComparison.OrdinalIgnoreCase)||string.Equals(a.Value,"Eclipse",StringComparison.OrdinalIgnoreCase));
}
public enum RuleAppliance { ApplianceNone, AppliancePlayer, ApplianceOpponent, ApplianceAll }
public enum FightEvent { AnimationStartEvent, RenderEvent, HitEvent }
public sealed class InfoAnimation { public string Name; public bool LPPIKDGABOL(string n)=>false; }
public static class AnimationData { public static void NEBELEFIDMB(string n,List<InfoAnimation> l){l.Add(new InfoAnimation{Name=n});} }
public sealed class ModelNode { public Vector3f GetStart()=>new Vector3f(); }
public sealed class ModelNodes { public ModelNode EGHIDHMENEF(string n)=>new ModelNode(); }
public class Model { public ModelNodes CLDMEJKGLBA()=>new ModelNodes(); public void PONNDMHBGJK(IntervalAnimation.NGAJJDIEDGF t){} }
public struct Vector3f { float x,y,z; public Vector3f(float x,float y,float z){this.x=x;this.y=y;this.z=z;} public float GetX()=>x; public float GetY()=>y; public float GetZ()=>z; }
public sealed class NativeLocation { public float JMLAKAKDBBL; public float GBNPHCHGKDO; }
public sealed class RuleInitData { public NativeLocation LPJNEDFCBOI; public Model DLPKDAIDCBF; public Model OGBHDKKOIGH; }
public sealed class FightData { public FightEvent KOJNCHKPLLN; public bool CBLNOFELDOE; public InfoAnimation LKLHCEEMINM; }
public sealed class PlayersFightData { public int SlowMode=1; public FightData MPLPEMOFHGI=new FightData(); public FightData EKBMBILHBMC=new FightData(); }
public static class LLLOJBFMONN { public static void Error(string f,params object[] a){} }
public static class IntervalAnimation { public enum NGAJJDIEDGF { INTERVAL_NONE,INTERVAL_ATTACK,INTERVAL_BLOCK,INTERVAL_INVULNERABLE,INTERVAL_SELF_UNINTERRUPT,INTERVAL_UNINTERRUPT,INTERVAL_UNSTABLE } }
public sealed class NativeRoster { public int PINDEKDNCNL()=>52; }
public static class ListSF { public static NativeRoster CCDKHLAMKKO()=>new NativeRoster(); }
'@ | Set-Content -Encoding UTF8 $nativeStubs

$sources = @(
 'Assets/Scripts/Eclipse/Runtime/Modding/ModId.cs','Assets/Scripts/Eclipse/Runtime/Modding/AssetId.cs','Assets/Scripts/Eclipse/Runtime/Modding/DefinitionId.cs',
 'Assets/Scripts/Eclipse/Runtime/Modding/SemanticVersion.cs','Assets/Scripts/Eclipse/Runtime/Modding/VersionRange.cs','Assets/Scripts/Eclipse/Runtime/Modding/ModManifest.cs',
 'Assets/Scripts/Eclipse/Runtime/Modding/ModManifestReader.cs','Assets/Scripts/Eclipse/Runtime/Modding/ModDiagnostics.cs','Assets/Scripts/Eclipse/Runtime/Modding/ModDiscovery.cs',
 'Assets/Scripts/Eclipse/Runtime/Modding/DependencyResolver.cs','Assets/Scripts/Eclipse/Runtime/Modding/AssetProvider.cs','Assets/Scripts/Eclipse/Runtime/Modding/AssetResolver.cs',
 'Assets/Scripts/Eclipse/Runtime/Modding/LooseModProvider.cs','Assets/Scripts/Eclipse/Runtime/Modding/ModScripting.cs','Assets/Scripts/Eclipse/Runtime/Modding/ModUiRuntime.cs',
 'Assets/Scripts/Eclipse/Runtime/Modding/ModStoryEvents.cs','Assets/Scripts/Eclipse/Runtime/Modding/ModFightEntry.cs','Assets/Scripts/Eclipse/Runtime/Modding/ModScriptingP1C.cs','Assets/Scripts/Eclipse/Runtime/Modding/ModScriptingP1D.cs',
 'Assets/Scripts/Eclipse/Runtime/Modding/ModWarriorTemplates.cs','Assets/Scripts/Eclipse/Runtime/Modding/ModSaveData.cs','Assets/Scripts/Eclipse/Runtime/Modding/CoreContentImporter.cs',
 'Assets/Scripts/Eclipse/Runtime/Modding/CoreContentImporterP1C.cs','Assets/Scripts/Eclipse/Runtime/Modding/ModContent.cs','Assets/Scripts/Eclipse/Runtime/Modding/ModContentP1B.cs',
 'Assets/Scripts/Eclipse/Runtime/Modding/ModContentP1C.cs','Assets/Scripts/Eclipse/Runtime/Modding/ModContentShopPrices.cs','Assets/Scripts/Eclipse/Runtime/Modding/ModContentP1D.cs','Assets/Scripts/Eclipse/Runtime/Modding/ModContentP2.cs',
 'Assets/Scripts/Eclipse/Runtime/Modding/ModContentP3.cs','Assets/Scripts/Eclipse/Runtime/Modding/ModDojoButtons.cs','Assets/Scripts/Eclipse/Runtime/Modding/ModVisuals.cs','Assets/Scripts/Eclipse/Runtime/Modding/ModFx.cs','Assets/Scripts/Eclipse/Runtime/Modding/ModSelection.cs','Assets/Scripts/Eclipse/Runtime/Modding/ModLocalizationLoader.cs',
 'Assets/Scripts/Eclipse/Runtime/Modding/ModMovePerkLocks.cs',
 'Assets/Scripts/Eclipse/Runtime/Modding/ModTrialRules.cs','Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntime.cs','Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeUi.cs','Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeVisuals.cs',
 'Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeP1D.cs','Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeShortForm.cs','Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeP2.cs','Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeP3.cs','Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeFightEntry.cs',
 'Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeP1D.cs','Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeSequence.cs','Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeP2.cs','Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeP3.cs','Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeFightEntry.cs',
 'Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeTrialRules.cs','Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeRuleGroups.cs','Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeUnderworld.cs','Assets/Scripts/Eclipse/Runtime/Modding/ModRuleGroups.cs',
 'Assets/Scripts/Assembly-CSharp/Rule.cs','Assets/Scripts/Assembly-CSharp/InFightRule.cs','Assets/Scripts/Assembly-CSharp/AnimationListRule.cs',
 'Assets/Scripts/Assembly-CSharp/HotGroundRule.cs','Assets/Scripts/Assembly-CSharp/RingOutRule.cs','Assets/Scripts/Assembly-CSharp/RegenerationRule.cs',
 'Assets/Scripts/Assembly-CSharp/NoAnimationRule.cs','Assets/Scripts/Assembly-CSharp/RemoveIntervalRule.cs',
 'Assets/Scripts/Eclipse/LightInTheDarknessRule.cs'
) | ForEach-Object { Join-Path $root $_ }
$program = Join-Path $root 'Tools/TrialRulesTests.cs'
$compile = @($program,$projection,$nativeStubs)+$sources
$compileXml = ($compile | ForEach-Object { '    <Compile Include="'+[Security.SecurityElement]::Escape($_)+'" />' }) -join "`n"
$moon = [Security.SecurityElement]::Escape((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
$project = Join-Path $fixture 'TrialRules.csproj'
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><EnableDefaultCompileItems>false</EnableDefaultCompileItems><Nullable>disable</Nullable><LangVersion>latest</LangVersion></PropertyGroup><ItemGroup>
$compileXml
    <Reference Include="MoonSharp.Interpreter"><HintPath>$moon</HintPath><Private>true</Private></Reference>
</ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 $project

dotnet build $project -nologo --verbosity quiet
if ($LASTEXITCODE -ne 0) { throw "Trial rule fixture compile failed: $LASTEXITCODE" }
dotnet (Join-Path $fixture 'bin/Debug/net10.0/TrialRules.dll') $modsRoot
if ($LASTEXITCODE -ne 0) { throw "Trial rule fixture failed: $LASTEXITCODE" }
