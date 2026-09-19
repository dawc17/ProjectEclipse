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
function Load-Lua([string]$scriptText) {
    Set-Content -LiteralPath (Join-Path $package 'scripts/main.lua') -Value ('local sf2=require("sf2")' + "`n" + $scriptText)
    $catalog = [Eclipse.Modding.ModContentCatalog]::new()
    $null = [Eclipse.Modding.CoreContentImporter]::ImportPerks($catalog, [Xml.XmlNode[]]@($perks.DocumentElement.ChildNodes))
    $null = [Eclipse.Modding.CoreContentImporter]::ImportWarriorTemplates($catalog, $stages.SelectSingleNode('Stages/Warriors/Templates'))
    $null = [Eclipse.Modding.CoreContentImporter]::ImportRanged($catalog, [Xml.XmlNode[]]@($items.SelectNodes('/List/Items/Item')), $null)
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
foreach($good in @('aspect=0','aspect=2147483647','aspect=0.5','chance_factor=0','chance_factor=10000')) {
    $null = Load-Lua ($prefix + 'sf2.warriors.register {id="test",perks={{perk=p,'+$good+'}}}')
    Check $true ('Valid setting rejected: '+$good)
}
foreach($bad in @('aspect=-1','aspect=2147483648','aspect=1/0','aspect=0/0','aspect="12"','aspect=false',
    'chance_factor=-1','chance_factor=10001','chance_factor=1/0','chance_factor=0/0','chance_factor="2"','unknown=1')) {
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

# Compare pending DE Lua opponents to the historical XML, not to a duplicated fixture.
$catalog = Load-Lua 'require("content.sensei_act_one_opponents")'
[xml]$archive = Get-Content -Raw (Join-Path $root 'Assets/DExml/stages.xml')
function Shape([Xml.XmlNode]$node) {
    $attrs = @($node.Attributes | Where-Object {$_.Name -ne 'EclipseCharacterId' -and !($_.Name -eq 'Level' -and $_.Value -eq '1')} | Sort-Object Name | ForEach-Object {$_.Name+'='+$_.Value}) -join ';'
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
Write-Output "PASS: $script:checks warrior loadout checks. Projection evidence: $fixture. Native combat is not exercised."
