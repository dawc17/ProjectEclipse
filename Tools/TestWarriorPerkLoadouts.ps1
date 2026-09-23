# Real Lua registration and production XML projection; no live fight or player save.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
dotnet build (Join-Path $root 'Assembly-CSharp.csproj') -v:q /clp:ErrorsOnly
if ($LASTEXITCODE -ne 0) { throw 'Managed build failed.' }
. (Join-Path $PSScriptRoot 'LoadUnityManagedAssemblies.ps1')
$null = [Reflection.Assembly]::LoadFrom((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
$null = [MoonSharp.Interpreter.Script]::DefaultOptions
$null = Import-SF2ManagedRuntime $root
$fixture = Join-Path $root ('Temp/WarriorPerks-' + [Guid]::NewGuid().ToString('N'))
$package = Join-Path $fixture 'Mods/fixture.warriors'
$null = New-Item -ItemType Directory -Force (Join-Path $package 'scripts/content')
Copy-Item -LiteralPath (Join-Path $root 'Mods/de128/scripts/content/sensei_act_one_opponents.lua') -Destination (Join-Path $package 'scripts/content/sensei_act_one_opponents.lua')
Copy-Item -LiteralPath (Join-Path $root 'Mods/de128/scripts/content/sensei_boss_opponents.lua') -Destination (Join-Path $package 'scripts/content/sensei_boss_opponents.lua')
Copy-Item -LiteralPath (Join-Path $root 'Mods/de128/scripts/content/sensei_art.lua') -Destination (Join-Path $package 'scripts/content/sensei_art.lua')
# Shipped DE128 Sensei sprites resolve under the fixture namespace through the real loose provider.
Copy-Item -Recurse -LiteralPath (Join-Path $root 'Mods/de128/assets') -Destination (Join-Path $package 'assets')
@'
schema = 1
id = "fixture.warriors"
name = "Warrior loadout checks"
version = "1.0.0"
authors = ["Eclipse tests"]
entrypoint = "scripts/main.lua"
capabilities = ["content.register"]
[[dependencies]]
id = "core"
version = ">=1.0 <2.0"
'@ | Set-Content (Join-Path $package 'mod.toml')
'return' | Set-Content (Join-Path $package 'scripts/main.lua')
$mod = [Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $fixture 'Mods')).Mods[0]
if ($null -eq $mod) { throw 'Fixture discovery failed.' }
[xml]$perks = Get-Content -Raw (Join-Path $root 'Assets/vanillaXml/perks.xml')
[xml]$items = Get-Content -Raw (Join-Path $root 'Assets/vanillaXml/list.xml')
[xml]$stages = Get-Content -Raw (Join-Path $root 'Assets/vanillaXml/stages.xml')
$script:checks = 0
function Check([bool]$ok, [string]$message) { if (!$ok) { throw $message }; $script:checks++ }
function Load-Lua([string]$scriptText, [Xml.XmlNode]$additionalTemplates = $null, [Xml.XmlNode[]]$additionalRanged = @(), [Xml.XmlNode]$zones = $null) {
    Set-Content -LiteralPath (Join-Path $package 'scripts/main.lua') -Value ('local sf2=require("sf2")' + "`n" + $scriptText)
    $catalog = [Eclipse.Modding.ModContentCatalog]::new()
    if ($null -ne $zones) { $null = [Eclipse.Modding.CoreContentImporter]::ImportStages($catalog, $zones) }
    $null = [Eclipse.Modding.CoreContentImporter]::ImportPerks($catalog, [Xml.XmlNode[]]@($perks.DocumentElement.ChildNodes))
    $null = [Eclipse.Modding.CoreContentImporter]::ImportWarriorTemplates($catalog, $stages.SelectSingleNode('Stages/Warriors/Templates'))
    if ($null -ne $additionalTemplates) { $null = [Eclipse.Modding.CoreContentImporter]::ImportWarriorTemplates($catalog, $additionalTemplates) }
    $null = [Eclipse.Modding.CoreContentImporter]::ImportWeapons($catalog, [Xml.XmlNode[]]@($items.SelectNodes('/List/Items/Item')), $null)
    $null = [Eclipse.Modding.CoreContentImporter]::ImportArmors($catalog, [Xml.XmlNode[]]@($items.SelectNodes('/List/Items/Item')), $null)
    $null = [Eclipse.Modding.CoreContentImporter]::ImportHelms($catalog, [Xml.XmlNode[]]@($items.SelectNodes('/List/Items/Item')), $null)
    $null = [Eclipse.Modding.CoreContentImporter]::ImportMagic($catalog, [Xml.XmlNode[]]@($items.SelectNodes('/List/Items/Item')), $null)
    $null = [Eclipse.Modding.CoreContentImporter]::ImportRanged($catalog, [Xml.XmlNode[]]@($items.SelectNodes('/List/Items/Item')), $null)
    if ($additionalRanged.Count -gt 0) { $null = [Eclipse.Modding.CoreContentImporter]::ImportRanged($catalog, $additionalRanged, $null) }
    $tx = $catalog.BeginRegistration($mod)
    $context = $null
    try {
        $assets = [Eclipse.Modding.AssetResolver]::new([Eclipse.Modding.IAssetProvider[]]@([Eclipse.Modding.LooseModProvider]::new($mod)))
        $api = [Eclipse.Modding.ModApiFacade]::new($mod,$assets,$tx,$null)
        $context = [Eclipse.Modding.MoonSharpScriptRuntime]::new().CreateContext($mod,$api)
        $context.ExecuteEntrypoint(); $tx.Commit()
    } catch {
        Check ($catalog.Warriors.Count -eq 0) 'Failed Lua registration leaked warriors.'
        throw
    } finally { if ($null -ne $context) {$context.Dispose()}; $tx.Dispose() }
    $catalog.Freeze()
    return $catalog
}
$flags = [Reflection.BindingFlags]'Instance,NonPublic'
function Project($catalog, $warrior) {
    $adapter = [Eclipse.Modding.LegacyContentAdapter]::new($catalog)
    return [Eclipse.Modding.LegacyContentAdapter].GetMethod('BuildWarriorNode',$flags).Invoke($adapter,@([Xml.XmlDocument]::new(),$warrior))
}
function Fingerprint($catalog) { return [Eclipse.Modding.ModSaveData]::ComputeContentSetFingerprint([Eclipse.Modding.ModDescriptor[]]@($mod),$catalog) }
function First-Warrior($catalog) { return @($catalog.Warriors)[0] }
$prefix = 'local p=sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_TIME_BOMB_WEAPON"); '
$bare = $prefix + 'sf2.warriors.register {id="test",perks={p}}'
$wrapped = $prefix + 'sf2.warriors.register {id="test",perks={{perk=p}}}'
$configured = $prefix + 'sf2.warriors.register {id="test",perks={{perk=p,aspect=100000,chance_factor=2.69}}}'
$plain = Load-Lua $bare
$config = Load-Lua $configured
Check ((Fingerprint $plain) -ceq (Fingerprint (Load-Lua $wrapped))) 'Empty settings changed the bare-handle fingerprint.'
Check ((Project $plain (First-Warrior $plain)).SelectNodes('Perks/Perk/Set').Count -eq 0) 'Bare handles should inherit native defaults.'
Check ((Fingerprint $config) -ceq (Fingerprint (Load-Lua $configured))) 'Non-deterministic settings fingerprint.'
foreach($changed in @($configured.Replace('100000','99999'),$configured.Replace('2.69','2.75'),$configured.Replace(',aspect=100000',''),$configured.Replace(',chance_factor=2.69',''))) {
    Check ((Fingerprint $config) -cne (Fingerprint (Load-Lua $changed))) 'A settings change was omitted from the content fingerprint.'
}
foreach($catalog in @($plain,$config)) {
    $baseline = @($catalog.Perks | Where-Object {$_.LegacyName -eq 'PERK_ITEM_SPECIAL_TIME_BOMB_WEAPON'})[0]
    Check ($baseline.LegacyPerkXml -ceq $perks.SelectSingleNode('/*/Perk[@Name="PERK_ITEM_SPECIAL_TIME_BOMB_WEAPON"]').OuterXml) 'Core perk definition was mutated.'
}
$node = Project $config (First-Warrior $config)
Check ((Project $plain (First-Warrior $plain)).OuterXml -ceq (Project (Load-Lua $wrapped) (First-Warrior (Load-Lua $wrapped))).OuterXml) 'Empty settings changed bare-handle projection.'
Check ($node.SelectSingleNode('Perks/Perk/Set').GetAttribute('Aspect') -ceq '100000') 'Aspect not projected.'
Check ($node.SelectSingleNode('Perks/Perk/Set').GetAttribute('ChanceFactor') -ceq '2.69') 'Chance factor not projected.'
$mixed=Load-Lua ($prefix+'local q=sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_INTOXICATION_WEAPON"); sf2.warriors.register{id="test",perks={p,{perk=q,aspect=0}}}')
$mixedNode=Project $mixed (First-Warrior $mixed)
Check ($null -eq $mixedNode.SelectSingleNode('Perks/Perk[1]/Set') -and $mixedNode.SelectSingleNode('Perks/Perk[2]/Set').GetAttribute('Aspect') -ceq '0' -and !$mixedNode.SelectSingleNode('Perks/Perk[2]/Set').HasAttribute('ChanceFactor')) 'Mixed handles/settings lost default inheritance or explicit zero.'
$culture = [Threading.Thread]::CurrentThread.CurrentCulture
try {
    [Threading.Thread]::CurrentThread.CurrentCulture = [Globalization.CultureInfo]::GetCultureInfo('nb-NO')
    Check ((Project $config (First-Warrior $config)).OuterXml -ceq $node.OuterXml) 'Locale changed native numbers.'
    Check ((Fingerprint $config) -ceq (Fingerprint (Load-Lua $configured))) 'Locale changed settings fingerprint.'
} finally { [Threading.Thread]::CurrentThread.CurrentCulture = $culture }
foreach($good in @('aspect=0','aspect=2147483647','aspect=0.5','chance_factor=0','chance_factor=10000','chance=0','chance=1','chance=0.42','frames=0','frames=300','frames=2147483647')) {
    $null = Load-Lua ($prefix + 'sf2.warriors.register {id="test",perks={{perk=p,'+$good+'}}}')
    Check $true ('Valid setting rejected: '+$good)
}
foreach($bad in @('aspect=-1','aspect=2147483648','aspect=1/0','aspect=0/0','aspect="12"','aspect=false',
    'chance_factor=-1','chance_factor=10001','chance_factor=1/0','chance_factor=0/0','chance_factor="2"','unknown=1',
    'chance=-0.1','chance=1.01','chance=1/0','chance=0/0','chance="0.42"','chance=false',
    'frames=-1','frames=0.5','frames=2147483648','frames=1/0','frames=0/0','frames="300"','frames=false')) {
    $failed=$false
    try { $null=Load-Lua ($prefix+'sf2.warriors.register {id="prior"}; sf2.warriors.register {id="test",perks={{perk=p,'+$bad+'}}}') } catch { $failed=$true }
    Check $failed ('Invalid setting accepted: '+$bad)
}
foreach($bad in @('{p,p}','{p,{perk=p,aspect=2}}','{false}','{{aspect=2}}','{{perk="fake"}}','{[1]=p,[3]=p}','{p,other=true}','false','"fake"')) {
    $failed=$false
    try { $null=Load-Lua ($prefix+'sf2.warriors.register {id="prior"}; sf2.warriors.register {id="test",perks='+$bad+'}') } catch { $failed=$true }
    Check $failed ('Invalid loadout accepted: '+$bad)
}
$failed=$false
try { $null=Load-Lua ($prefix+'local rows={}; for i=1,65 do rows[i]=p end; sf2.warriors.register{id="test",perks=rows}') } catch { $failed=$true }
Check $failed 'Oversized loadout accepted.'
$owned = @'
local name=sf2.localization.register{id="name",language="eng",value="Test"}
local behavior=sf2.behaviors.register{id="behavior",on_tick=function() end}
local p=sf2.perks.register{id="owned",display_name=name,description=name,behavior=behavior,kind=sf2.perks.SINGLE}
sf2.warriors.register{id="test",perks={p}}
'@
$null=Load-Lua $owned
$failed=$false
try { $null=Load-Lua $owned.Replace('perks={p}','perks={{perk=p,aspect=2}}') } catch { $failed=$true }
Check $failed 'Native overrides accepted on an owned Lua perk.'
foreach($setting in @('chance=0.42','frames=300')) {
    $failed=$false
    try { $null=Load-Lua $owned.Replace('perks={p}',('perks={{perk=p,'+$setting+'}}')) } catch { $failed=$true }
    Check $failed ('Native override accepted on an owned Lua perk: '+$setting)
}
$complete=$prefix+'sf2.warriors.register{id="test",perks={{perk=p,aspect=0,chance_factor=0,chance=0.42,frames=300}}}'
$completeCatalog=Load-Lua $complete
$completeNode=Project $completeCatalog (First-Warrior $completeCatalog)
Check ($completeNode.SelectSingleNode('Perks/Perk/Set').GetAttribute('Chance') -ceq '0.42') 'Explicit probability not projected.'
Check ($completeNode.SelectSingleNode('Perks/Perk/Set').GetAttribute('Frames') -ceq '300') 'Explicit duration not projected.'
foreach($variant in @($complete.Replace('chance=0.42','chance=0.43'),$complete.Replace('frames=300','frames=301'),$complete.Replace(',chance=0.42',''),$complete.Replace(',frames=300',''))) {
    Check ((Fingerprint $completeCatalog) -cne (Fingerprint (Load-Lua $variant))) 'Probability/duration missing from fingerprint.'
}
$zero=Load-Lua ($prefix+'sf2.warriors.register{id="test",perks={{perk=p,chance=0,frames=0}}}')
$zeroSet=(Project $zero (First-Warrior $zero)).SelectSingleNode('Perks/Perk/Set')
Check ($zeroSet.GetAttribute('Chance') -ceq '0' -and $zeroSet.GetAttribute('Frames') -ceq '0' -and !$zeroSet.HasAttribute('Aspect')) 'Explicit zero/default inheritance was lost.'

# Sprite-handle avatars and battle previews: qualified owned IDs, legacy strings kept.
$sprite='local art=sf2.assets.sprite("sprites/sensei/boss_hermit_young"); '
$handleAvatar=Load-Lua ($sprite+'sf2.warriors.register{id="test",avatar=art}')
Check ((Project $handleAvatar (First-Warrior $handleAvatar)).GetAttribute('Avatar') -ceq 'fixture.warriors:sprites/sensei/boss_hermit_young') 'Sprite avatar was not projected as its qualified ID.'
$stringAvatar=Load-Lua 'sf2.warriors.register{id="test",avatar="character_savage"}'
Check ((Project $stringAvatar (First-Warrior $stringAvatar)).GetAttribute('Avatar') -ceq 'character_savage') 'Legacy avatar string changed.'
Check ((Fingerprint $handleAvatar) -cne (Fingerprint (Load-Lua 'sf2.warriors.register{id="test",avatar="boss_hermit_young"}'))) 'Sprite and bare-name avatars share a fingerprint.'
$battleZone='local zone=sf2.zones.register{id="z"}; '
$handlePreview=Load-Lua ($sprite+$battleZone+'sf2.battles.register{id="b",zone=zone,type=sf2.battles.STORY,preview=art}')
Check (@($handlePreview.Battles)[0].Preview -ceq 'fixture.warriors:sprites/sensei/boss_hermit_young') 'Sprite preview was not stored as its qualified ID.'
$stringPreview=Load-Lua ($battleZone+'sf2.battles.register{id="b",zone=zone,type=sf2.battles.STORY,preview="preview_main.statue"}')
Check (@($stringPreview.Battles)[0].Preview -ceq 'preview_main.statue') 'Legacy preview string changed.'
foreach($bad in @('avatar=5','avatar={}','avatar=true','avatar=sf2.warriors.get_template("core:warrior-templates/lynx_claws")')) {
    $failed=$false
    try { $null=Load-Lua ('sf2.warriors.register{id="prior"}; sf2.warriors.register{id="test",'+$bad+'}') } catch { $failed=$_.ToString().Contains("must be a sprite handle or string") }
    Check $failed ('Invalid avatar accepted or wrongly reported: '+$bad)
}
foreach($bad in @('preview=5','preview={}','preview=zone')) {
    $failed=$false
    try { $null=Load-Lua ($battleZone+'sf2.battles.register{id="b",zone=zone,type=sf2.battles.STORY,'+$bad+'}') } catch { $failed=$_.ToString().Contains("must be a sprite handle or string") }
    Check $failed ('Invalid preview accepted or wrongly reported: '+$bad)
}

# Compare pending DE Lua opponents to the historical XML, not to a duplicated fixture.
$catalog = Load-Lua 'require("content.sensei_act_one_opponents")'
[xml]$archive = Get-Content -Raw (Join-Path $root 'Assets/DExml/stages.xml')
# Owned portraits project as qualified sprite IDs. Compare their archived name only
# after proving the ID is this package's shipped Sensei sprite, never a core name.
$script:ownedPortraits = @{}
function Owned-Portrait([string]$name, [string]$value) {
    $match = [regex]::Match($value, '^fixture\.[a-z]+:sprites/sensei/([a-z_]+)$')
    if ($name -ne 'Avatar' -or !$match.Success) { return $value }
    $bare = $match.Groups[1].Value
    Check (Test-Path (Join-Path $root ('Mods/de128/assets/textures/sensei/'+$bare+'.png'))) ('Owned avatar has no shipped texture: '+$value)
    $script:ownedPortraits[$bare] = $true
    return $bare
}
function Shape([Xml.XmlNode]$node) {
    $attrs = @($node.Attributes | Where-Object {$_.Name -ne 'EclipseCharacterId' -and !($_.Name -eq 'Level' -and $_.Value -eq '1')} | Sort-Object Name | ForEach-Object {$_.Name+'='+(Owned-Portrait $_.Name $_.Value)}) -join ';'
    $children = @($node.ChildNodes | Where-Object {$_.NodeType -eq 'Element'} | ForEach-Object {Shape $_}) -join ''
    return '<'+$node.LocalName+' '+$attrs+'>'+$children+'</'+$node.LocalName+'>'
}
Check ($catalog.Warriors.Count -eq 2) 'Missing normal/eclipse young Lynx.'
foreach($warrior in $catalog.Warriors) {
    $battle=if($warrior.Id.ToString().EndsWith('_eclipse')) {'SENSEI_MEMORIES_ECLIPSEMODE'} else {'SENSEI_MEMORIES'}
    $expected = $archive.SelectSingleNode('//Zone[@Name="ZONE_1"]/Battle[@Name="'+$battle+'"]//Warrior[@Template="Lynx_Claws"]')
    Check ($null -ne $expected) ('Missing archived opponent: '+$battle)
    $actual=Project $catalog $warrior
    $actual.OuterXml | Set-Content (Join-Path $fixture ($battle+'.xml'))
    Check ((Shape $actual) -ceq (Shape $expected)) ('Young Lynx differs from archive: '+$battle+"`n"+$actual.OuterXml+"`n"+$expected.OuterXml)
}

# All six bosses, both modes: compare complete native projection to source rows.
$catalog=Load-Lua 'require("content.sensei_boss_opponents")'
Check ($catalog.Warriors.Count -eq 12) 'Expected twelve normal/eclipse boss loadouts.'
foreach($warrior in $catalog.Warriors) {
    $local=$warrior.Id.LocalId
    $act=if($local.StartsWith('sensei_act_one_')){1}else{[int]([regex]::Match($local,'sensei_act_([2-6])_').Groups[1].Value)}
    $battle=if($local.EndsWith('_eclipse')){'SENSEI_MEMORIES_ECLIPSEMODE'}else{'SENSEI_MEMORIES'}
    $boss=@($archive.SelectNodes('//Zone[@Name="ZONE_'+$act+'"]/Battle[@Name="'+$battle+'"]//Warrior') | Where-Object {!$_.GetAttribute('Template').StartsWith('Guard_')})
    Check ($boss.Count -eq 1) ('Expected unique boss source in act '+$act)
    $actual=Project $catalog $warrior
    $actual.OuterXml | Set-Content (Join-Path $fixture ($local+'.xml'))
    Check ((Shape $actual) -ceq (Shape $boss[0])) ('Boss differs from archive: '+$local+"`n"+$actual.OuterXml+"`n"+$boss[0].OuterXml)
}
Check ((($script:ownedPortraits.Keys | Sort-Object) -join ',') -ceq 'boss_butcher_young,boss_hermit_young,boss_shogun_young,boss_wasp_young,boss_widow_young') ('Young boss avatars did not use shipped portraits: '+(($script:ownedPortraits.Keys | Sort-Object) -join ','))
Write-Output "PASS: $script:checks warrior loadout checks. Projection evidence: $fixture. Native combat is not exercised."
