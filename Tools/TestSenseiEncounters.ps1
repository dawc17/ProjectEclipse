# Executes actual pending Lua. Missing templates/item use the explicit identity
# fixtures from TestSenseiOpponents; conditional behavior uses an explicit controlled availability reader.
. (Join-Path $PSScriptRoot 'TestSenseiOpponents.ps1')
foreach($name in @('sensei_battles','sensei_battle_text','sensei_encounters','sensei_rewards','sensei_fight_rules','sensei_raid_charge')) {
    Copy-Item (Join-Path $root "Mods/de128/scripts/content/$name.lua") (Join-Path $package 'scripts/content')
}
$script:checks=0
$script:ownedPreviews=0
$zoneDoc=[Xml.XmlDocument]::new();$zones=$zoneDoc.CreateElement('Zones');$null=$zoneDoc.AppendChild($zones)
for($act=1;$act -le 6;$act++) {
    $node=$stages.SelectSingleNode('//Zone[@Name="ZONE_'+$act+'"]')
    $null=$zones.AppendChild($zoneDoc.ImportNode($node,$false))
}
$prefix=$pending+$dependencies+@'
local factory=require("content.sensei_encounters")
local opponents=pending.register(deps)
local conditional=require("content.sensei_raid_charge").register(function() return false end)
'@
$entry=$prefix+@'
local graph=factory.register(opponents,conditional)
assert(#graph.battles.normal==6 and #graph.battles.eclipse==6 and #graph.normal==6 and #graph.eclipse==6 and #graph.finals==5)
for act=1,5 do assert(graph.finals[act]==graph.normal[act][3]) end
assert(#graph.normal[6]==2)
local original=require("content.sensei_fight_rules")
for act=1,6 do
 for _,entry in ipairs(original.acts[act].normal) do
  for _,rule in ipairs(entry.unconditional) do assert(rule~=conditional) end
 end
end
'@
$catalog=Load-Lua $entry $identities.DocumentElement @($charge.DocumentElement) $zones
Check ($catalog.Battles.Count -eq 12 -and $catalog.Fights.Count -eq 23 -and $catalog.Rewards.Count -eq 57) 'Incomplete encounter graph.'
Check ($catalog.Warriors.Count -eq 34 -and $catalog.Zones.Count -eq 6) 'Opponent or map-page graph differs.'
$adapter=[Eclipse.Modding.LegacyContentAdapter]::new($catalog)
$build=[Eclipse.Modding.LegacyContentAdapter].GetMethod('BuildBattleNode',[Reflection.BindingFlags]'Instance,NonPublic')
$textKeys=@{alias='Sensei_arc';title='Sensei_arc_Title';locked='Sensei_button_locked_desc';description='Sensei_arc_Desc'}
$translations=0
foreach($localization in $catalog.Localizations) {
    if($localization.Id.Namespace.Value -ne "fixture.warriors"){continue}
    $key=$localization.Id.LocalId.Replace('sensei.battle.','')
    Check ($textKeys.ContainsKey($key)) 'Unexpected localization key.'
    foreach($value in $localization.Values.GetEnumerator()) {
        [xml]$language=Get-Content -Raw (Join-Path $root ('Assets/DExml/localizations/'+$value.Key+'.xml'))
        $word=$language.SelectSingleNode('//Word[@Title="'+$textKeys[$key]+'"]')
        Check ($value.Value -ceq $word.InnerText) ('Battle translation differs: '+$key+'/'+$value.Key)
        $translations++
    }
}
Check ($translations -eq 56) 'Expected four labels in fourteen languages.'
function Rule-Shape([Xml.XmlElement]$node) {
    $copy=$node.CloneNode($true)
    if($copy.LocalName -in @('EquipItem','Avatar','Name','NoButton') -and !$copy.HasAttribute('ApplyTo')){$copy.SetAttribute('ApplyTo','Player')}
    if($copy.LocalName -eq 'EquipItem' -and $copy.GetAttribute('Name') -eq 'NoRanged'){$copy.RemoveAttribute('Type')}
    return Shape $copy
}
foreach($battle in $catalog.Battles) {
    $parts=[regex]::Match($battle.Id.LocalId,'^sensei_act_([1-6])_(normal|eclipse)$')
    Check $parts.Success 'Unexpected battle ID.'
    $act=$parts.Groups[1].Value;$mode=$parts.Groups[2].Value
    $name=if($mode -eq 'normal'){'SENSEI_MEMORIES'}else{'SENSEI_MEMORIES_ECLIPSEMODE'}
    $source=$archive.SelectSingleNode('//Zone[@Name="ZONE_'+$act+'"]/Battle[@Name="'+$name+'"]')
    $actual=$build.Invoke($adapter,@([Xml.XmlDocument]::new(),$battle))
    $actual.OuterXml | Set-Content (Join-Path $fixture ($battle.Id.LocalId+'-graph.xml'))
    Check ($battle.Zone.ToString() -ceq ('core:zones/zone_'+$act)) 'Battle moved to another map page.'
    foreach($field in @('Type','X','Y','Icon','Location','Music')) {
        Check ($actual.GetAttribute($field) -ceq $source.GetAttribute($field)) ('Battle metadata differs: '+$battle.Id+'/'+$field)
    }
    # Core has preview_main.statue; the five pvp arena previews are shipped DE128 sprites.
    $preview=$source.GetAttribute('Preview')
    if($preview.StartsWith('preview_pvp_')) {
        Check ($actual.GetAttribute('Preview') -ceq ('fixture.warriors:sprites/sensei/'+$preview)) ('Battle preview is not the shipped sprite: '+$battle.Id)
        Check (Test-Path (Join-Path $root ('Mods/de128/assets/textures/sensei/'+$preview+'.png'))) ('Shipped preview texture missing: '+$preview)
        $script:ownedPreviews++
    } else {
        Check ($actual.GetAttribute('Preview') -ceq $preview) ('Core battle preview changed: '+$battle.Id)
    }
    $description=if($mode -eq 'normal'){'locked'}else{'description'}
    foreach($field in @('Alias','Title','Description')) {
        $key=if($field -eq 'Description'){$description}else{$field.ToLowerInvariant()}
        Check ($actual.GetAttribute($field) -ceq ('fixture.warriors:localization/sensei.battle.'+$key)) 'Battle localization reference differs.'
    }
    if($mode -eq 'normal') {
        $target=$actual.GetAttribute('EclipseToggleName')
        $pair=@($catalog.Battles | Where-Object {$_.LegacyName -ceq $target})
        Check ($pair.Count -eq 1 -and $pair[0].Zone -eq $battle.Zone -and $pair[0].Id.LocalId -eq ('sensei_act_'+$act+'_eclipse')) 'Eclipse counterpart does not resolve in the same zone.'
    } else {Check (!$actual.HasAttribute('EclipseToggleName')) 'Eclipse counterpart points back to normal.'}
    $expectedFights=$source.SelectNodes('Fight');$actualFights=$actual.SelectNodes('Fight')
    Check ($actualFights.Count -eq $expectedFights.Count) 'Wrong encounter count.'
    for($index=0;$index -lt $actualFights.Count;$index++) {
        $a=$actualFights[$index];$e=$expectedFights[$index]
        foreach($field in @('Power','Rounds','RoundTime','Replays','EvaluatedRating')) {
            Check ([int]$a.GetAttribute($field) -eq [int]$e.GetAttribute($field)) ('Fight setting differs: '+$field)
        }
        Check ((Shape $a.Warriors) -ceq (Shape (Synthesized-Guards $e.Warriors))) 'Ordered assembled roster differs.'
        $actualRules=$a.SelectNodes('Rules/*')
        $expectedRules=@($e.SelectNodes('Rules/*') | Where-Object {$_.LocalName -ne 'RulesWithConditions'})
        Check ($actualRules.Count -eq $expectedRules.Count) 'Static rule count differs.'
        for($r=0;$r -lt $actualRules.Count;$r++) {Check ((Rule-Shape $actualRules[$r]) -ceq (Rule-Shape $expectedRules[$r])) 'Ordered static rule differs.'}
        # Behavior rules do not project into native XML; verify the actual catalog
        # attachment position against the source wrapper, not a hardcoded offset.
        $definition=@($catalog.Fights | Where-Object {$_.LegacyName -ceq $a.GetAttribute('Name')})[0]
        $position=0
        foreach($rule in $e.SelectNodes('Rules/*')){if($rule.LocalName -eq 'RulesWithConditions'){break};$position++}
        Check ($definition.Rules[$position].LocalId -ceq 'sensei_raid_charge_conditional') 'Conditional dependency attached at wrong position.'
        Check ($definition.Rules.Count -eq $expectedRules.Count+1) 'Conditional dependency duplicated or omitted.'
        $ar=$a.SelectNodes('Rewards/Reward');$er=$e.SelectNodes('Rewards/Reward')
        Check ($ar.Count -eq $er.Count) 'Reward slot count differs.'
        for($r=0;$r -lt $ar.Count;$r++) {
            foreach($field in @('Money','Exp','Bonus','PrizeBase')) {Check ([decimal]$ar[$r].GetAttribute($field) -eq [decimal]$er[$r].GetAttribute($field)) ('Reward slot differs: '+$field)}
        }
    }
}
foreach($bad in @('factory.register(opponents,nil)','opponents[4].normal[3]=nil; factory.register(opponents,conditional)',
    'factory.register(opponents,{})')) {
    $failed=$false
    try{$null=Load-Lua ($prefix+$bad) $identities.DocumentElement @($charge.DocumentElement) $zones}catch{$failed=$true}
    Check $failed ('Invalid assembler dependency accepted: '+$bad)
}
Check ($script:ownedPreviews -eq 10) ('Expected ten battle entries using shipped previews, found '+$script:ownedPreviews)
Write-Output "PASS: $script:checks Sensei graph checks: 12 paired battles, 23 fights, 34 loadouts, 57 reward slots and 56 translations. Evidence: $fixture. Conditional callback uses a controlled availability reader; guards use the synthesized Default+voice templates; the Sphere1 projection identity remains controlled. No complete story/asset/gameplay claim."
