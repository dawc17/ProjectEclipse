# Real Lua binding, adapter and native condition/attack parser. No game save or live fight.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
dotnet build (Join-Path $root 'Assembly-CSharp.csproj') -v:q /clp:ErrorsOnly
if ($LASTEXITCODE -ne 0) { throw 'Managed build failed.' }
. (Join-Path $PSScriptRoot 'LoadUnityManagedAssemblies.ps1')
$null = [Reflection.Assembly]::LoadFrom((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
# Initialize MoonSharp's standalone defaults before loading Unity reference DLLs;
# the production runtime replaces this loader with its sandboxed module loader.
$null = [MoonSharp.Interpreter.Script]::DefaultOptions
$null = Import-SF2ManagedRuntime $root
$fixture = Join-Path $root ('Temp/MoveAttackAuthoring-' + [Guid]::NewGuid().ToString('N'))
$package = Join-Path $fixture 'Mods/fixture.moves'
$null = New-Item -ItemType Directory -Force (Join-Path $package 'scripts/content')
Copy-Item -LiteralPath (Join-Path $root 'Mods/de128/scripts/content/chinese_swords_data.lua') -Destination (Join-Path $package 'scripts/content/chinese_swords_data.lua')
@'
schema = 1
id = "fixture.moves"
name = "Move authoring checks"
version = "1.0.0"
authors = ["Eclipse tests"]
entrypoint = "scripts/main.lua"
capabilities = ["content.register"]
'@ | Set-Content (Join-Path $package 'mod.toml')
'return' | Set-Content (Join-Path $package 'scripts/main.lua')
$mod = [Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $fixture 'Mods')).Mods[0]
if ($null -eq $mod) { throw 'Fixture discovery failed.' }
$script:checks = 0
function Check([bool]$ok, [string]$message) { if (!$ok) { throw $message }; $script:checks++ }
function Load-Lua([string]$scriptText, [scriptblock]$seedCatalog = $null) {
    Set-Content -LiteralPath (Join-Path $package 'scripts/main.lua') -Value ('local sf2=require("sf2")' + "`n" + $scriptText)
    $catalog = [Eclipse.Modding.ModContentCatalog]::new()
    if ($null -ne $seedCatalog) { & $seedCatalog $catalog }
    $tx = $catalog.BeginRegistration($mod)
    $context = $null
    try {
        $assets = [Eclipse.Modding.AssetResolver]::new([Eclipse.Modding.IAssetProvider[]]@([Eclipse.Modding.LooseModProvider]::new($mod)))
        $api = [Eclipse.Modding.ModApiFacade]::new($mod,$assets,$tx,$null)
        $context = [Eclipse.Modding.MoonSharpScriptRuntime]::new().CreateContext($mod,$api)
        $context.ExecuteEntrypoint(); $tx.Commit()
    } catch {
        Check ($catalog.MoveTemplates.Count -eq 0 -and $catalog.Moves.Count -eq 0 -and $catalog.MoveItemLockExtensions.Count -eq 0) 'Failed Lua registration leaked move content.'
        throw
    } finally { if ($null -ne $context) {$context.Dispose()}; $tx.Dispose() }
    $catalog.Freeze()
    return $catalog
}
$flags = [Reflection.BindingFlags]'Instance,NonPublic'
function Project($catalog) {
    $adapter = [Eclipse.Modding.LegacyContentAdapter]::new($catalog)
    return [Eclipse.Modding.LegacyContentAdapter].GetMethod('BuildMovesDocument',$flags).Invoke($adapter,@())
}
function Fingerprint($catalog) { return [Eclipse.Modding.ModSaveData]::ComputeContentSetFingerprint([Eclipse.Modding.ModDescriptor[]]@($mod),$catalog) }
$lua = @'
local data=require("content.chinese_swords_data")
sf2.moves.register_template { id="slash",conditions=data.conditions,intervals=data.intervals }
sf2.moves.register_template { id="preview",conditions={data.preview_screen} }
'@
$catalog = Load-Lua $lua
$projected = Project $catalog
$projected.Save((Join-Path $fixture 'projected.xml'))
[xml]$archive = Get-Content -Raw (Join-Path $root 'Assets/DExml/animations/moves.xml')
$expected = $archive.SelectSingleNode('//Move[@Name="ChineseSwordsSuperSlash"]')
$actual = $projected.SelectSingleNode('//Template[contains(@Name,"slash")]')
Check ($null -ne $actual -and $null -ne $expected) 'Missing authored/archive move.'
# Compare every authored condition, interval, edge, term, impulse and reaction.
# ID=0 is the native default inserted by the adapter; attribute order is irrelevant.
function Shape([Xml.XmlNode]$node) {
    $attrs = @($node.Attributes | Where-Object { !($_.Name -eq 'ID' -and $_.Value -eq '0') } | Sort-Object Name | ForEach-Object {$_.Name+'='+$_.Value}) -join ';'
    $children = @($node.ChildNodes | Where-Object {$_.NodeType -eq 'Element'} | ForEach-Object {Shape $_}) -join ''
    return '<'+$node.LocalName+' '+$attrs+'>'+$children+'</'+$node.LocalName+'>'
}
foreach($section in @('Conditions','Intervals')) { Check ((Shape $actual[$section]) -ceq (Shape $expected[$section])) "$section differs from archived move." }
Check ((Shape $projected.SelectSingleNode('//Template[contains(@Name,"preview")]/Conditions/Screen')) -ceq (Shape $archive.SelectSingleNode('//Move[@Name="ShopChineseSwordsSuperSlash"]/Locks/Screen'))) 'Preview screen differs from archive.'
[MovesMaps]::Init()
function Parse-Attack($node) { $attack=[IntervalAttack]::new(); $attack.set_AnimationFinishFrame(10000); $attack.Parse($node); $attack.Init(); return $attack }
$actualAttacks = $actual.SelectNodes('Intervals/Interval[@Type="Attack"]')
$expectedAttacks = $expected.SelectNodes('Intervals/Interval[@Type="Attack"]')
Check ($actualAttacks.Count -eq 4) 'Lost multi-hit interval.'
for($i=0;$i -lt 4;$i++) {
    $parsed=Parse-Attack $actualAttacks[$i]; $baseline=Parse-Attack $expectedAttacks[$i]
    # Compare primitive and list fields of the actual native parser, including
    # private damage terms/reactions, without introducing recovered symbol aliases.
    foreach($field in [IntervalAttack].GetFields([Reflection.BindingFlags]'Instance,NonPublic,Public')) {
        $left=$field.GetValue($parsed); $right=$field.GetValue($baseline)
        if ($null -eq $left -or $field.FieldType.IsPrimitive -or $left -is [string] -or $left -is [Collections.IList]) {
            Check (($left | ConvertTo-Json -Depth 8 -Compress) -ceq ($right | ConvertTo-Json -Depth 8 -Compress)) ('Native attack field differs: '+$field.Name)
        }
    }
}
function Parse-Condition($node) { $value=[ConditionsParser]::Create($node); $value.Parse($node); return $value }
$keys=Parse-Condition $actual.SelectSingleNode('Conditions/Keys')
function Input-Keys([string]$body) {
    [xml]$doc='<Keys>'+$body+'</Keys>'
    $condition=[ConditionKeys]::new($doc.DocumentElement)
    $field=[ConditionKeys].GetFields() | Where-Object {$_.FieldType -eq [KeyData]} | Select-Object -First 1
    return $field.GetValue($condition)
}
$tap='<Key Type="Punch" PressType="Tap"/>'
$forward='<Key Type="Forward" PressType="Hold"/>'
Check ($keys.IsEqual((Input-Keys ($tap+$tap+$forward)),$true)) 'Native repeated tap sequence rejected.'
Check (!$keys.IsEqual((Input-Keys ($tap+$forward)),$true)) 'Single tap incorrectly satisfies double tap.'
Check (!$keys.IsEqual((Input-Keys ($tap+$tap)),$true)) 'Forward hold requirement lost.'
foreach($name in @('RoundStage','ModExists')) {
    $a=Parse-Condition $actual.SelectSingleNode('Conditions/'+$name)
    $b=Parse-Condition $expected.SelectSingleNode('Conditions/'+$name)
    Check ($a.GetType() -eq $b.GetType() -and $a.IsNot -eq $b.IsNot) ('Native condition changed: '+$name)
}
$screen=Parse-Condition $projected.SelectSingleNode('//Template[contains(@Name,"preview")]/Conditions/Screen')
Check ($screen.get_Type().ToString() -eq 'SceneShopWeapon') 'Native shop screen mapping changed.'
$legacy='sf2.moves.register_template {id="test",intervals={{type="Attack",attack={edges={"Edge"},damage=0.06,damage_type="WeaponDamage"}}}}'
$single=$legacy.Replace('damage_type="WeaponDamage"','damage_terms={{type="WeaponDamage",shift=0}}')
Check ((Fingerprint (Load-Lua $legacy)) -ceq (Fingerprint (Load-Lua $single))) 'Single unshifted shorthand fingerprint changed.'
Check ((Shape (Project (Load-Lua $legacy)).DocumentElement) -ceq (Shape (Project (Load-Lua $single)).DocumentElement)) 'Legacy shorthand projection changed.'
Check ((Fingerprint $catalog) -ceq (Fingerprint (Load-Lua $lua))) 'Repeat registration is not deterministic.'
$mixed=$legacy.Replace('damage_type="WeaponDamage"','damage_terms={{type="WeaponDamage"},{type="UnarmedDamage",shift=-10}}')
Check ((Fingerprint (Load-Lua $mixed)) -cne (Fingerprint (Load-Lua $mixed.Replace('shift=-10','shift=-9')))) 'Damage shift omitted from fingerprint.'
Check ((Fingerprint (Load-Lua $mixed)) -cne (Fingerprint (Load-Lua $single))) 'Additional damage term omitted from fingerprint.'
Check ((Fingerprint (Load-Lua $mixed)) -cne (Fingerprint (Load-Lua $mixed.Replace('UnarmedDamage','MagicDamage')))) 'Additional term type omitted from fingerprint.'
Check ((Fingerprint (Load-Lua $lua)) -cne (Fingerprint (Load-Lua ($lua.Replace('sf2.moves.register_template { id="slash"','table.remove(data.conditions[1].keys, 1)' + "`n" + 'sf2.moves.register_template { id="slash"'))))) 'Repeated key count omitted from fingerprint.'
Check ((Fingerprint (Load-Lua $lua)) -ceq (Fingerprint (Load-Lua ($lua.Replace('sf2.moves.register_template { id="slash"','data.conditions[1].keys.removed = true; data.conditions[1].keys.removed = nil' + "`n" + 'sf2.moves.register_template { id="slash"'))))) 'Deleted non-array key rejected a valid sequence.'
foreach($shift in @(-1000,0,1000)) {
    $bounded=Load-Lua $single.Replace('shift=0',('shift='+$shift))
    Check ($bounded.MoveTemplates.Count -eq 1) 'Valid shift boundary rejected.'
}
foreach($bad in @(
    'damage_terms={}', 'damage_terms={{type="Armor"}}',
    'damage_terms={{type="WeaponDamage",shift=0/0}}', 'damage_terms={{type="WeaponDamage",shift=1/0}}',
    'damage_terms={{type="WeaponDamage",shift=1001}}', 'damage_terms={{type="WeaponDamage",shift=-1001}}',
    'damage_terms={{type="WeaponDamage",shift="bad"}}', 'damage_terms={{type="WeaponDamage",extra=1}}',
    'damage_terms={{type="WeaponDamage"},{type="WeaponDamage"}}',
    'damage_terms={{type="WeaponDamage"}},damage_type="WeaponDamage"',
    'damage_terms={[1]={type="WeaponDamage"},[3]={type="MagicDamage"}}',
    'damage_terms={false}', 'damage_terms={{type="WeaponDamage"},unexpected=true}',
    'damage_terms={{type="WeaponDamage"},{type="UnarmedDamage"},{type="MagicDamage"},{type="RangedDamage"},{type="WeaponDamage"}}',
    'hit="invented"'
)) {
    $failure=$null
    try {$null=Load-Lua ('sf2.moves.register_template {id="prior"}' + "`n" + $legacy.Replace('damage_type="WeaponDamage"',$bad))}catch{$failure=$_}
    Check ($null -ne $failure) ('Invalid attack accepted: '+$bad)
}
foreach($bad in @(
    '{type="round_stage",name="invented"}', '{type="round_stage"}',
    '{type="screen",name="invented"}', '{type="screen",name="ShopWeapon",item_type="Weapon"}',
    '{type="mod_exists",name=""}', '{type="mod_exists",name="MOD_TITAN",player="Nobody"}',
    '{type="keys",keys={}}', '{type="keys",keys={{key="Punch",press="invented"}}}'
)) {
    $failure=$null
    try {$null=Load-Lua ('sf2.moves.register_template {id="prior"}' + "`n" + 'sf2.moves.register_template {id="bad",conditions={'+$bad+'}}')}catch{$failure=$_}
    Check ($null -ne $failure) ('Invalid condition accepted: '+$bad)
}
foreach($mutation in @('data.conditions[1].keys[2]=nil','data.conditions[1].keys[0]=data.conditions[1].keys[1]',
    'data.conditions[1].keys[1.5]=data.conditions[1].keys[1]','data.conditions[1].keys[-1]=data.conditions[1].keys[1]',
    'data.conditions[1].keys.extra=true')) {
    $failure=$null
    try {$null=Load-Lua $lua.Replace('sf2.moves.register_template { id="slash"',$mutation+"`n"+'sf2.moves.register_template { id="slash"')}catch{$failure=$_}
    Check ($null -ne $failure) ('Non-dense key array accepted: '+$mutation)
}
Check ((Fingerprint $catalog) -ceq (Fingerprint (Load-Lua $lua))) 'Failed registrations poisoned subsequent move loading.'
Write-Output "PASS $script:checks Lua/native move attack authoring checks; projected archive sections in $fixture. No live fight."
