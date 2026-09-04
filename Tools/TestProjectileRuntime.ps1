# Build Assembly-CSharp.csproj first. Uses real move XML and the compiled parser;
# the normalizer's Unity resource/log calls are faked and native animation-file
# loading is skipped. No saves or Unity UI.
$ErrorActionPreference = 'Stop'
$projectPath = Split-Path $PSScriptRoot -Parent
. (Join-Path $PSScriptRoot 'LoadUnityManagedAssemblies.ps1')
$assembly = Import-SF2ManagedRuntime $projectPath
$checks = 0
function Assert-True($condition, [string]$label) {
    if (!$condition) { throw $label }
    $script:checks++
}

$resourceSource = Get-Content -Raw (Join-Path $projectPath 'Assets/Scripts/Assembly-CSharp/ResourceManager.cs')
$normalize = [regex]::Match($resourceSource, '(?ms)^\t\tprivate static void NormalizeMoves\([^\r\n]*\)\r?\n\t\t\{.*?^\t\t\}')
Assert-True $normalize.Success 'Cannot find move normalization method'
$fixture = @'
using System;
using System.Collections.Generic;
using System.Xml;
using Eclipse.Content;
namespace ProjectileNormalizationTest {
    public class TextAsset { public string text; }
    public static class ResourcesAndBundles {
        public static TextAsset Baseline;
        public static T Load<T>(string path) where T : class { return Baseline as T; }
    }
    public static class Debug {
        public static void Log(object message) { }
        public static void LogWarning(object message) {
            if (message.ToString().Contains("could not merge")) throw new Exception(message.ToString());
        }
    }
    public static class XmlExtensions {
        public static string CIPOICEEIBK(this XmlAttribute attribute, string fallback) {
            return attribute == null ? fallback : attribute.Value;
        }
    }
    public static class Normalizer {
        private static HashSet<string> _devXmlLogged = new HashSet<string>();
        public static void Run(XmlDocument document, string baseline) {
            ResourcesAndBundles.Baseline = new TextAsset { text = baseline };
            NormalizeMoves(document);
        }
/* NORMALIZE */
    }
}
'@
$normalizerReferences = @(
    $assembly.Location,
    (Join-Path $PSHOME 'ref/mscorlib.dll'),
    (Join-Path $PSHOME 'ref/netstandard.dll'),
    (Join-Path $PSHOME 'ref/System.Collections.dll'),
    (Join-Path $PSHOME 'ref/System.Xml.dll'),
    (Join-Path $PSHOME 'ref/System.Xml.ReaderWriter.dll')
)
Add-Type -TypeDefinition $fixture.Replace('/* NORMALIZE */', $normalize.Value) -ReferencedAssemblies $normalizerReferences
[xml]$modern = Get-Content -Raw (Join-Path $projectPath 'Assets/vanillaXml/animations/moves.xml')
$baselineText = Get-Content -Raw (Join-Path $projectPath 'Assets/Resources/gamedata/animations/moves.txt')
[xml]$baseline = $baselineText
$adapted = [xml]$modern.OuterXml
[ProjectileNormalizationTest.Normalizer]::Run($adapted, $baselineText)
$once = $adapted.OuterXml
[ProjectileNormalizationTest.Normalizer]::Run($adapted, $baselineText)
Assert-True ($adapted.OuterXml -ceq $once) 'Move normalization is not idempotent'

# The migration must not add old inherited actions/locks to modern projectiles.
foreach ($name in @('RangedShurikenPlayer','RangedShurikenWeapon','ShurikenFly','RangedKunaiWeapon','KunaiFly','RangedHeavyWeapon','ChakramFly','FireballStart')) {
    $path = "/Movesxml/Moves/Move[@Name='$name']"
    Assert-True ($null -ne $modern.SelectSingleNode($path)) ('Missing modern fixture: ' + $name)
    Assert-True ($adapted.SelectSingleNode($path).OuterXml -ceq $modern.SelectSingleNode($path).OuterXml) ('Modern move changed: ' + $name)
}

# Parse a focused move set with the actual compiled MovesParser. Reflection
# avoids its resource-loading entry point, which requires Unity's native API.
$flags = [Reflection.BindingFlags]'NonPublic,Static'
$parser = $assembly.GetType('MovesParser')
function Parse-Moves([xml]$document, [string[]]$names) {
    [MovesMaps]::Clear()
    [MovesMaps]::Init()
    $templates = New-Object 'System.Collections.Generic.Dictionary[string,TemplateAnimation]'
    $parser.GetMethod('AKGCKOGKJBD', $flags).Invoke($null, @($document.Movesxml.Templates, $templates.PSObject.BaseObject)) | Out-Null
    $legacyTemplates = New-Object 'System.Collections.Generic.Dictionary[string,System.Xml.XmlNode]'
    foreach ($template in $document.SelectNodes('/Movesxml/LegacyTemplates/Template')) {
        $legacyTemplates.Add($template.GetAttribute('Name'), $template)
    }
    $parser.GetField('_LegacyTemplateTemp', $flags).SetValue($null, $legacyTemplates.PSObject.BaseObject)
    $selected = $document.CreateElement('Moves')
    foreach ($name in $names) {
        $move = $document.SelectSingleNode("/Movesxml/Moves/Move[@Name='$name']")
        if ($null -eq $move) { throw ('Move missing: ' + $name) }
        $copy = $move.CloneNode($true)
        # Test the move rules, not Unity's native binary-resource loader.
        $copy.SetAttribute('FileName', '')
        $selected.AppendChild($copy) | Out-Null
    }
    $moves = New-Object 'System.Collections.Generic.List[InfoAnimation]'
    $tricks = New-Object 'System.Collections.Generic.List[Trick]'
    $parser.GetMethod('MNCBOOGMKGB', $flags).Invoke($null, @($selected, $templates.PSObject.BaseObject, $moves.PSObject.BaseObject, $tricks.PSObject.BaseObject)) | Out-Null
    return ,$moves
}
$names = @('EnergyballStart','IceballStart','HermitStormStart','RangedShurikenWeapon','ShurikenFly','RangedKunaiWeapon','KunaiFly','RangedHeavyWeapon','ChakramFly')
$parsed = Parse-Moves $adapted $names
$byName = @{}
foreach ($move in $parsed) { $byName[$move.Name] = $move }
Assert-True ($byName.EnergyballStart.IsItemRequired('Weapon','EnergyBall')) 'Energyball lost its inherited weapon lock'
Assert-True ($byName.IceballStart.IsItemRequired('Weapon','IceBall')) 'Iceball lost its inherited weapon lock'
foreach ($name in @('EnergyballStart','IceballStart','HermitStormStart')) {
    $move = $byName[$name]
    Assert-True ($move.ODACDCDONJE.HIFPHBNGIPO.Count -gt 0) ($name + ' is unrestricted')
    Assert-True ($move.ODACDCDONJE.JIFAHHGNPFH.Count -gt 0) ($name + ' lost inherited conditions')
}
# Original template membership is a runtime contract for CurrentAnimation
# conditions and AI queries; do not replace it with renamed template tags.
$originalParsed = Parse-Moves $baseline @('EnergyballStart','IceballStart','HermitStormStart')
foreach ($original in $originalParsed) {
    $move = $byName[$original.Name]
    Assert-True (($move.FOLOOGCLPNE() -join '|') -ceq ($original.FOLOOGCLPNE() -join '|')) ($original.Name + ' template membership changed')
    foreach ($field in @('HIFPHBNGIPO','JIFAHHGNPFH','DJBAIAKOIHM','AJCMBMJGJEG')) {
        Assert-True ($move.ODACDCDONJE.$field.Count -eq $original.ODACDCDONJE.$field.Count) ($original.Name + ' inherited rule count changed: ' + $field)
    }
}

# The exact reported shuriken, plus the other common ranged subtypes, must
# reject the magic birth moves while retaining their own launch/flight moves.
foreach ($subtype in @('Shuriken','Kunai','Chakram')) {
    $conditions = New-Object ModelConditions
    $conditions.OJIAKDDCGLB = New-Object 'System.Collections.Generic.List[ItemInfo]'
    foreach ($spec in @(@('Weapon',$subtype),@('Skeleton','SkeletonMissile'))) {
        $item = New-Object ItemInfo -ArgumentList @($null)
        $item.Type = $spec[0]
        $item.MDPPNGIEJGD = $spec[1]
        $item.Name = if ($spec[0] -eq 'Weapon') { 'RANGED_BP_S5_TIME_SHIFTER' } else { 'SkeletonMissile' }
        $conditions.OJIAKDDCGLB.Add($item)
    }
    foreach ($name in @('EnergyballStart','IceballStart','HermitStormStart')) {
        $move = $byName[$name]
        Assert-True (!$move.HPPGNJJCEGF($conditions, $move.ODACDCDONJE.HIFPHBNGIPO)) ($subtype + ' can select ' + $name)
    }
    $launch = if ($subtype -eq 'Chakram') { 'RangedHeavyWeapon' } else { 'Ranged' + $subtype + 'Weapon' }
    foreach ($name in @($launch, ($subtype + 'Fly'))) {
        $move = $byName[$name]
        Assert-True ($move.HPPGNJJCEGF($conditions, $move.ODACDCDONJE.HIFPHBNGIPO)) ($subtype + ' cannot select ' + $name)
    }
}
foreach ($case in @(@('EnergyballStart','EnergyBall','MAGIC_ENERGY_BALL'),@('IceballStart','IceBall','MAGIC_BP_S5_TIME_SHIFTER'),@('HermitStormStart','','HERMIT_STORM'))) {
    $conditions = New-Object ModelConditions
    $conditions.OJIAKDDCGLB = New-Object 'System.Collections.Generic.List[ItemInfo]'
    $item = New-Object ItemInfo -ArgumentList @($null)
    $item.Type = 'Weapon'
    $item.MDPPNGIEJGD = $case[1]
    $item.Name = $case[2]
    $conditions.OJIAKDDCGLB.Add($item)
    $skeleton = New-Object ItemInfo -ArgumentList @($null)
    $skeleton.Type = 'Skeleton'
    $skeleton.MDPPNGIEJGD = 'SkeletonMagic'
    $conditions.OJIAKDDCGLB.Add($skeleton)
    $move = $byName[$case[0]]
    Assert-True ($move.HPPGNJJCEGF($conditions, $move.ODACDCDONJE.HIFPHBNGIPO)) ($case[0] + ' rejects its intended magic item')
}
# Exercise the actual block-removal branch from Model.Strike with compiled
# IntervalAttack/Model/ModelAnimation objects. Only unrelated AI, collision,
# damage and event dispatch are excluded from this focused hit-path fixture.
$modelSource = Get-Content -Raw (Join-Path $projectPath 'Assets/Scripts/Assembly-CSharp/Model.cs')
$blockBranch = [regex]::Match($modelSource, '(?ms)^\t\tif \(hFIIPNLCIEE\.NPHDDMAIGKN\(\)\)\r?\n\t\t\{.*?^\t\t\}')
Assert-True $blockBranch.Success 'Cannot find strike block-bypass branch'
$blockSource = @'
public static class ProjectileBlockTest {
    public static bool Resolve(Model defender, IntervalAttack hFIIPNLCIEE) {
/* BLOCK */
        return defender.AMGHOKDANGN();
    }
}
'@
$blockBody = $blockBranch.Value.Replace('RemoveInterval(', 'defender.RemoveInterval(').Replace('RemoveIntervals(', 'defender.RemoveIntervals(')
$blockReferences = @($assembly.Location, (Join-Path $projectPath 'Temp/Bin/Debug/Assembly-CSharp-firstpass.dll'))
if ($PSVersionTable.PSEdition -eq 'Core') {
    # Unity's managed assemblies target .NET Framework; use Core's facade when
    # compiling the tiny caller in PowerShell 7.
    $blockReferences += Join-Path $PSHOME 'ref/mscorlib.dll'
    $blockReferences += Join-Path $PSHOME 'ref/System.Collections.dll'
}
Add-Type -TypeDefinition $blockSource.Replace('/* BLOCK */', $blockBody) -ReferencedAssemblies $blockReferences
$instanceFlags = [Reflection.BindingFlags]'NonPublic,Instance'
function New-BlockingDefender {
    $defender = [Runtime.Serialization.FormatterServices]::GetUninitializedObject([Model])
    $animation = [Runtime.Serialization.FormatterServices]::GetUninitializedObject([ModelAnimation])
    $intervals = New-Object 'System.Collections.Generic.List[IntervalAnimation]'
    foreach ($name in @('GuardHigh','GuardLow')) {
        $guard = New-Object IntervalAnimation ([IntervalAnimation+NGAJJDIEDGF]::INTERVAL_BLOCK)
        $guard.Name = $name
        $intervals.Add($guard)
    }
    $evade = New-Object IntervalAnimation ([IntervalAnimation+NGAJJDIEDGF]::INTERVAL_INVULNERABLE)
    $evade.Name = 'Evade'
    $intervals.Add($evade)
    [ModelAnimation].GetField('KKNKJMCFIJK', $instanceFlags).SetValue($animation, $intervals.PSObject.BaseObject)
    [Model].GetField('_Animation', $instanceFlags).SetValue($defender, $animation)
    return $defender
}
function Parse-Attack([System.Xml.XmlNode]$node) {
    $attack = New-Object IntervalAttack
    $attack.set_AnimationFinishFrame(10000)
    $attack.Parse($node)
    $attack.Init()
    return $attack
}
foreach ($case in @(
    @('<IgnoresBlock />', 'WeaponDamage', $false),
    @('<IgnoresBlock Name="" />', 'WeaponDamage', $false),
    @('', 'WeaponDamage', $true),
    @('<IgnoresBlock Name="GuardHigh" />', 'WeaponDamage', $true),
    @('<IgnoresBlock Name="GuardHigh|GuardLow" />', 'WeaponDamage', $false),
    @('', 'RangedDamage', $false),
    @('', 'MagicDamage', $false)
)) {
    [xml]$attackXml = '<Interval Type="Attack">' + $case[0] + '<Damage Value="0.1"><Damage Type="' + $case[1] + '" /></Damage></Interval>'
    $attack = Parse-Attack $attackXml.DocumentElement
    $defender = New-BlockingDefender
    Assert-True ([ProjectileBlockTest]::Resolve($defender, $attack) -eq $case[2]) ('Wrong blocked state for ' + $case[1] + ': ' + $case[0])
    Assert-True ($null -ne $defender.OCPMJKIEPIG().HDJBHPOGKNJ('Evade')) 'Block bypass removed dodge invulnerability'
    if ($case[0] -eq '<IgnoresBlock Name="GuardHigh" />') {
        Assert-True ($null -eq $defender.OCPMJKIEPIG().HDJBHPOGKNJ('GuardHigh')) 'Named block bypass did not remove its target'
    }
}
$projectileIntervals = $adapted.SelectNodes('/Movesxml/Moves/Move/Intervals/Interval[@Type="Attack"][Damage/Damage[@Type="RangedDamage" or @Type="MagicDamage"]]')
foreach ($node in $projectileIntervals) {
    $attack = Parse-Attack $node
    $defender = New-BlockingDefender
    $name = $node.ParentNode.ParentNode.GetAttribute('Name')
    Assert-True (![ProjectileBlockTest]::Resolve($defender, $attack)) ('Projectile is blockable: ' + $name)
}
Write-Output "PASS: $checks projectile assertions (real XML/parser, inherited locks, ranged eligibility, block resolution across $($projectileIntervals.Count) attack intervals)."
