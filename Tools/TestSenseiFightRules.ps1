# Offline source oracle + actual Lua registration/projection. No player profile.
$ErrorActionPreference='Stop'
$root=Split-Path -Parent $PSScriptRoot
dotnet build (Join-Path $root 'Assembly-CSharp.csproj') -v:q /clp:ErrorsOnly
if($LASTEXITCODE -ne 0){throw 'Managed build failed.'}
. (Join-Path $PSScriptRoot 'LoadUnityManagedAssemblies.ps1')
$null=[Reflection.Assembly]::LoadFrom((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
$null=[MoonSharp.Interpreter.Script]::DefaultOptions
$null=Import-SF2ManagedRuntime $root
$fixture=Join-Path $root ('Temp/SenseiRules-'+[Guid]::NewGuid().ToString('N'))
$package=Join-Path $fixture 'Mods/fixture.rules'
$null=New-Item -ItemType Directory -Force (Join-Path $package 'scripts/content')
Copy-Item (Join-Path $root 'Mods/de128/scripts/content/sensei_fight_rules.lua') (Join-Path $package 'scripts/content')
@'
schema = 1
id = "fixture.rules"
name = "Sensei rule checks"
version = "1.0.0"
authors = ["Eclipse tests"]
entrypoint = "scripts/main.lua"
capabilities = ["content.register"]
[[dependencies]]
id = "core"
version = ">=1.0 <2.0"
'@ | Set-Content (Join-Path $package 'mod.toml')
[xml]$archive=Get-Content -Raw (Join-Path $root 'Assets/DExml/stages.xml')
$source=[Collections.Generic.List[string]]::new()
$source.Add('local sf2=require("sf2"); local data=require("content.sensei_fight_rules"); local zone=sf2.zones.register{id="test"}; local dummy=sf2.warriors.register{id="test"}')
$expected=@{}
for($act=1;$act -le 6;$act++){
    foreach($mode in @('normal','eclipse')){
        $battleName=if($mode -eq 'normal'){'SENSEI_MEMORIES'}else{'SENSEI_MEMORIES_ECLIPSEMODE'}
        $fights=$archive.SelectNodes('//Zone[@Name="ZONE_'+$act+'"]/Battle[@Name="'+$battleName+'"]/Fight')
        $index=0
        foreach($fight in $fights){
            $index++
            $id="act_${act}_${mode}_${index}"
            $expected[$id]=$fight.Rules
            $array=if($mode -eq 'normal'){"data.acts[$act].normal[$index]"}else{"data.acts[$act].eclipse"}
            $position=1
            foreach($rule in $fight.Rules.ChildNodes){if($rule.NodeType -ne 'Element'){continue};if($rule.Name -eq 'RulesWithConditions'){break};$position++}
            $source.Add("do local entry=$array; assert(entry.charge_position==$position); local battle=sf2.battles.register{id='$id',zone=zone,type=sf2.battles.STORY}; sf2.fights.register{id='$id',battle=battle,warriors={dummy},rules=entry.unconditional} end")
        }
    }
}
$source.Add('local b=sf2.battles.register{id="charge",zone=zone,type=sf2.battles.STORY}; sf2.fights.register{id="charge",battle=b,warriors={dummy},rules={data.raid_charge}}')
$source.Add('for _,mode in ipairs({"normal","eclipse","all"}) do sf2.rules.no_button{id="mode_"..mode,name="Kick",mode=mode,rounds={2}} end')
$source -join "`n" | Set-Content (Join-Path $package 'scripts/main.lua')
$mod=[Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $fixture 'Mods')).Mods[0]
$catalog=[Eclipse.Modding.ModContentCatalog]::new()
[xml]$items=Get-Content -Raw (Join-Path $root 'Assets/vanillaXml/list.xml')
$rows=[Xml.XmlNode[]]@($items.SelectNodes('/List/Items/Item'))
$null=[Eclipse.Modding.CoreContentImporter]::ImportWeapons($catalog,$rows,$null)
$null=[Eclipse.Modding.CoreContentImporter]::ImportArmors($catalog,$rows,$null)
$null=[Eclipse.Modding.CoreContentImporter]::ImportHelms($catalog,$rows,$null)
$null=[Eclipse.Modding.CoreContentImporter]::ImportMagic($catalog,$rows,$null)
$null=[Eclipse.Modding.CoreContentImporter]::ImportRanged($catalog,$rows,$null)
[xml]$perks=Get-Content -Raw (Join-Path $root 'Assets/vanillaXml/perks.xml')
$null=[Eclipse.Modding.CoreContentImporter]::ImportPerks($catalog,[Xml.XmlNode[]]@($perks.DocumentElement.ChildNodes))
$assets=[Eclipse.Modding.AssetResolver]::new([Eclipse.Modding.IAssetProvider[]]@([Eclipse.Modding.LooseModProvider]::new($mod)))
$tx=$catalog.BeginRegistration($mod)
$context=[Eclipse.Modding.MoonSharpScriptRuntime]::new().CreateContext($mod,[Eclipse.Modding.ModApiFacade]::new($mod,$assets,$tx,$null))
try{$context.ExecuteEntrypoint();$tx.Commit()}finally{$context.Dispose();$tx.Dispose()}
$catalog.Freeze()
$adapter=[Eclipse.Modding.LegacyContentAdapter]::new($catalog)
$build=[Eclipse.Modding.LegacyContentAdapter].GetMethod('BuildFightNode',[Reflection.BindingFlags]'Instance,NonPublic')
$checks=0
function Check([bool]$ok,[string]$message){if(!$ok){throw $message};$script:checks++}
function Shape([Xml.XmlElement]$node){
    $copy=$node.CloneNode($true)
    if($copy.LocalName -in @('EquipItem','Avatar','Name','NoButton') -and !$copy.HasAttribute('ApplyTo')){$copy.SetAttribute('ApplyTo','Player')}
    # The named canonical NoRanged item already carries its Ranged type.
    if($copy.LocalName -eq 'EquipItem' -and $copy.GetAttribute('Name') -eq 'NoRanged'){$copy.RemoveAttribute('Type')}
    return $copy.LocalName+':'+(@($copy.Attributes | Sort-Object Name | ForEach-Object {$_.Name+'='+$_.Value}) -join ';')
}
Check ($catalog.Fights.Count -eq 24) 'Expected 23 encounter rule lists and the separate conditional body.'
$buildRule=[Eclipse.Modding.LegacyContentAdapter].GetMethod('BuildRuleNode',[Reflection.BindingFlags]'Instance,NonPublic')
foreach($definition in $catalog.FightRules) {
    if(!$definition.Id.LocalId.StartsWith('mode_')){continue}
    $node=$buildRule.Invoke($adapter,@([Xml.XmlDocument]::new(),$definition))
    $native=[NoButtonRule]::new($node)
    $wanted=switch($definition.Id.LocalId){'mode_normal'{'MODE_NORMAL'} 'mode_eclipse'{'MODE_ECLIPSE'} 'mode_all'{'MODE_ALL'}}
    Check ($native.PGOPBNMFAAG.ToString() -ceq $wanted) ('Native mode parsing differs: '+$node.OuterXml)
    Check (!$native.HAKHBAOJBON(1) -and $native.HAKHBAOJBON(2) -and !$native.HAKHBAOJBON(3)) 'Native round restriction changed.'
}
foreach($fight in $catalog.Fights){
    $node=$build.Invoke($adapter,@([Xml.XmlDocument]::new(),$fight))
    $node.OuterXml | Set-Content (Join-Path $fixture ($fight.Id.LocalId+'.xml'))
    if($fight.Id.LocalId -eq 'charge'){
        Check ($node.Rules.NoButton.Name -ceq 'RaidCharge') 'Wrong pending conditional body.'
        continue
    }
    $oracle=$expected[$fight.Id.LocalId]
    $actual=@($node.SelectNodes('Rules/*'))
    $static=@($oracle.ChildNodes | Where-Object {$_.NodeType -eq 'Element' -and $_.Name -ne 'RulesWithConditions'})
    Check ($actual.Count -eq $static.Count) ('Rule count differs: '+$fight.Id)
    for($i=0;$i -lt $actual.Count;$i++){Check ((Shape $actual[$i]) -ceq (Shape $static[$i])) ('Rule differs: '+$fight.Id+' row '+$i+': '+$actual[$i].OuterXml)}
    Check ($node.SelectNodes('Rules/NoButton').Count -eq 0) 'Conditional rule was attached unconditionally.'
    Check ($oracle.RulesWithConditions.Conditions.Equal.Value1 -ceq '_RaidChargeButton' -and $oracle.RulesWithConditions.Conditions.Equal.Value2 -ceq '0') 'Conditional source policy changed.'
}
Write-Output "PASS: $checks Sensei static rule projection checks across 23 fights. Conditional RaidCharge is deliberately unattached. Evidence: $fixture"
