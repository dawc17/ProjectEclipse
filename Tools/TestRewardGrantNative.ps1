$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixtureRoot = Join-Path $root 'Temp'
$testRoot = Join-Path $fixtureRoot ('RewardGrantNative-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $testRoot | Out-Null
Set-Content -LiteralPath (Join-Path $testRoot '.reward-grant-fixture') -Value 'Owned reward grant native fixture'
try {

function Extract-Method([string]$source, [string]$signature) {
    $start = $source.IndexOf($signature, [StringComparison]::Ordinal)
    if ($start -lt 0) { throw "Cannot find production method: $signature" }
    $lineStart = $source.LastIndexOf("`n", $start)
    if ($lineStart -lt 0) { $lineStart = 0 } else { $lineStart++ }
    $brace = $source.IndexOf('{', $start)
    if ($brace -lt 0) { throw "Cannot find opening brace for: $signature" }
    $depth = 0
    for ($i = $brace; $i -lt $source.Length; $i++) {
        if ($source[$i] -eq '{') { $depth++ }
        elseif ($source[$i] -eq '}') {
            $depth--
            if ($depth -eq 0) { return $source.Substring($lineStart, $i - $lineStart + 1) }
        }
    }
    throw "Cannot find closing brace for: $signature"
}

$adapterSource = Get-Content -Raw (Join-Path $root 'Assets/Scripts/Eclipse/Modding/LegacyContentAdapter.cs')
$runtimeSource = Get-Content -Raw (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModRuntime.cs')
$fightResultSource = Get-Content -Raw (Join-Path $root 'Assets/Scripts/Assembly-CSharp/FightResult.cs')
$userItemSource = Get-Content -Raw (Join-Path $root 'Assets/Scripts/Assembly-CSharp/UserItem.cs')
$contentSource = Get-Content -Raw (Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding/ModContent.cs')

$buildRewardNode = Extract-Method $adapterSource 'private XmlElement BuildRewardNode(XmlDocument document, RewardDefinition reward)'
$buildRewardItemNode = Extract-Method $adapterSource 'private XmlElement BuildRewardItemNode(XmlDocument document, RewardDefinition reward, RewardItemGrant grant,'
$legacyItemName = Extract-Method $adapterSource 'private string LegacyItemName(DefinitionId id)'
$configureReward = Extract-Method $runtimeSource 'internal static bool TryConfigureRewardGrant(RewardItem source, int playerLevel,'
$activeDependency = Extract-Method $runtimeSource 'private static bool IsActiveRewardDependency(ModDescriptor owner, ModId dependencyId)'
$fightReward = Extract-Method $fightResultSource 'public void KFJABAMAKOD(RewardItem JJBPBGKBEED)'
$grantEnchantments = Extract-Method $userItemSource 'public void GDBFNNLHPOB(List<PerkStruct> HALHGEGADKA, int MPAGFAKIEJG, int MHNCENBCECJ)'
$rewardDropApply = Extract-Method $contentSource 'public static void Apply(System.Xml.XmlElement fight, int resultIndex, ModRuleMode mode,'
$rewardDropReadBound = Extract-Method $contentSource 'private static int? ReadBound(System.Xml.XmlElement element, string name)'
$rewardDropRequireChoice = Extract-Method $contentSource 'private static void RequireItemChoice(System.Xml.XmlElement choice)'

$generated = Join-Path $testRoot 'ExtractedProduction.cs'
@"
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Eclipse.Modding;

internal sealed class RewardProjection
{
    private readonly ModContentCatalog _content;
    internal RewardProjection(ModContentCatalog content) { _content = content; }
    internal XmlElement Build(RewardDefinition reward) { var document = new XmlDocument(); return BuildRewardNode(document, reward); }
$buildRewardNode
$buildRewardItemNode
$legacyItemName
}

namespace Eclipse.Modding
{
    public static partial class ModRuntime
    {
        private static ModScriptSession _scripts;
        internal static void SetFixtureScripts(ModScriptSession scripts) { _scripts = scripts; }
$configureReward
$activeDependency
    }
}

public partial class FightResult
{
    public partial class ResultPrizeStruct
    {
        public readonly List<LJFFIBFBGID> HELFDCAIJNE = new List<LJFFIBFBGID>();
$fightReward
    }
}

public partial class UserItem
{
$grantEnchantments
}

internal static class ModRewardDropProjectionFixture
{
$rewardDropApply
$rewardDropReadBound
$rewardDropRequireChoice
}
"@ | Set-Content -Encoding UTF8 $generated

$project = Join-Path $testRoot 'RewardGrantNative.csproj'
$tests = Join-Path $root 'Tools/RewardGrantNativeTests.cs'
$rewardItem = Join-Path $root 'Assets/Scripts/Assembly-CSharp/RewardItem.cs'
$perkStruct = Join-Path $root 'Assets/Scripts/Assembly-CSharp/PerkStruct.cs'
$compile = @($tests, $generated, $rewardItem, $perkStruct) | ForEach-Object {
    '    <Compile Include="' + [Security.SecurityElement]::Escape($_) + '" />'
}
$compileXml = $compile -join "`n"
@"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <Nullable>disable</Nullable>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>
  <ItemGroup>
$compileXml
  </ItemGroup>
</Project>
"@ | Set-Content -Encoding UTF8 $project

dotnet build $project -nologo --verbosity quiet
if ($LASTEXITCODE -ne 0) { throw "Reward grant native fixture compile failed: $LASTEXITCODE" }
$dll = Join-Path $testRoot 'bin/Debug/net10.0/RewardGrantNative.dll'
dotnet $dll $root
if ($LASTEXITCODE -ne 0) { throw "Reward grant native fixture failed: $LASTEXITCODE" }
} finally {
    $ownedRoot = [System.IO.Path]::TrimEndingDirectorySeparator([System.IO.Path]::GetFullPath($fixtureRoot))
    $ownedFixture = [System.IO.Path]::GetFullPath($testRoot)
    $marker = Join-Path $ownedFixture '.reward-grant-fixture'
    if ([System.IO.Path]::GetDirectoryName($ownedFixture) -ine $ownedRoot -or
        [System.IO.Path]::GetFileName($ownedFixture) -notmatch '^RewardGrantNative-[0-9a-f]{32}$' -or
        !(Test-Path -LiteralPath $marker) -or
        (Get-Content -Raw -LiteralPath $marker).Trim() -ne 'Owned reward grant native fixture') {
        throw "Refusing to clean unverified reward grant fixture: $ownedFixture"
    }
    Remove-Item -LiteralPath $ownedFixture -Recurse -Force
}
