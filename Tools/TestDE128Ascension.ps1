Write-Host 'SKIP: obsolete DE128 Ascension prototype fixture is disabled; production work moved back to XML-evidenced DE64 content.'
<#
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root 'Temp/DE128Ascension'
New-Item -ItemType Directory -Force $fixture | Out-Null

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
$signatures = @(
    'public XmlElement BuildEncounterNode(FightDefinition fight, ModEncounterPlan plan)',
    'private XmlElement BuildFightNode(XmlDocument document, FightDefinition fight)',
    'private XmlElement BuildWarriorNode(XmlDocument document, WarriorDefinition warrior)',
    'private void AppendFightRules(XmlElement rules, FightDefinition fight)',
    'private XmlElement BuildRuleNode(XmlDocument document, FightRuleDefinition rule)',
    'private XmlElement BuildRewardNode(XmlDocument document, RewardDefinition reward)',
    'private XmlElement BuildRewardItemNode(XmlDocument document, RewardDefinition reward, RewardItemGrant grant,',
    'private string LegacyItemName(DefinitionId id)',
    'private string LegacyPerkName(DefinitionId id)',
    'private static void SetIfNotEmpty(XmlElement node, string name, string value)',
    'private static string RuleTargetName(ModRuleTarget target)'
)
$methods = foreach ($signature in $signatures) { Extract-Method $adapter $signature }
$projection = Join-Path $fixture 'DE128Projection.cs'
@"
using System; using System.Collections.Generic; using System.Globalization; using System.Xml; using Eclipse.Modding;
internal sealed class DE128Projection {
    private readonly ModContentCatalog _content;
    internal DE128Projection(ModContentCatalog content) { _content = content; }
    internal XmlElement Encounter(FightDefinition fight, ModEncounterPlan plan) => BuildEncounterNode(fight, plan);
$($methods -join "`n")
}
"@ | Set-Content -Encoding UTF8 $projection

$runtimeSources = Get-ChildItem (Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding') -Filter '*.cs' -File | Select-Object -ExpandProperty FullName
$moonSources = Get-ChildItem (Join-Path $root 'Assets/Scripts/Eclipse/Modding') -Filter 'MoonSharp*.cs' -File | Select-Object -ExpandProperty FullName
$extra = @(
    (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModModeRuntime.cs'),
    (Join-Path $root 'Tools/Phase2HostStubs.cs'),
    (Join-Path $root 'Tools/DE128AscensionTests.cs'),
    $projection
)
$allSources = @($runtimeSources) + @($moonSources) + $extra
$compileXml = ($allSources | ForEach-Object { '    <Compile Include="' + [Security.SecurityElement]::Escape($_) + '" />' }) -join "`n"
$moon = [Security.SecurityElement]::Escape((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
$project = Join-Path $fixture 'DE128Ascension.csproj'
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><EnableDefaultCompileItems>false</EnableDefaultCompileItems><Nullable>disable</Nullable><LangVersion>latest</LangVersion></PropertyGroup><ItemGroup>
$compileXml
    <Reference Include="MoonSharp.Interpreter"><HintPath>$moon</HintPath><Private>true</Private></Reference>
</ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 $project

dotnet build $project -nologo --verbosity quiet
if ($LASTEXITCODE -ne 0) { throw "DE128 Ascension fixture compile failed: $LASTEXITCODE" }
dotnet (Join-Path $fixture 'bin/Debug/net10.0/DE128Ascension.dll') $root (Join-Path $root 'Mods')
if ($LASTEXITCODE -ne 0) { throw "DE128 Ascension fixture failed: $LASTEXITCODE" }
#>
