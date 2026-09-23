# Real Lua and projection, with explicit identity-only fixtures for missing inputs.
# These fixtures are NOT restored templates/equipment and never enter Assets/Mods.
. (Join-Path $PSScriptRoot 'TestWarriorPerkLoadouts.ps1')
Copy-Item (Join-Path $root 'Mods/de128/scripts/content/sensei_guard_opponents.lua') (Join-Path $package 'scripts/content')
Copy-Item (Join-Path $root 'Mods/de128/scripts/content/sensei_guard_templates.lua') (Join-Path $package 'scripts/content')
$script:checks=0
$pending='local pending=require("content.sensei_guard_opponents"); '
Check ((Load-Lua $pending).Warriors.Count -eq 0) 'Requiring the pending factory registered content.'
foreach($argsText in @('nil','{}','{guard_girl={}}','{guard_girl={},guard_man={}}','{guard_girl={},guard_man={},sphere1={}}')) {
    $failed=$false
    try {$null=Load-Lua ($pending+'pending.register('+$argsText+')')} catch {
        $failed=$_.ToString().Contains('require Guard_Girl, Guard_Man template specs') -or $_.ToString().Contains('Guard template spec requires')
    }
    Check $failed 'Missing prerequisites did not fail before registering content.'
}
foreach($name in @('Guard_Girl','Guard_Man')) {
    Check ($null -eq $stages.SelectSingleNode('Stages/Warriors/Templates/Template[@Name="'+$name+'"]')) ('Core now has '+$name+'; reconcile the pending dependency.')
    Check ($null -eq $archive.SelectSingleNode('Stages/Warriors/Templates/Template[@Name="'+$name+'"]')) ('Archive now has '+$name+'; reconcile the pending dependency.')
    $failed=$false
    try {$null=Load-Lua ('sf2.warriors.get_template("core:warrior-templates/'+$name.ToLowerInvariant()+'")')} catch {$failed=$true}
    Check $failed 'Missing canonical template was silently substituted.'
}
Check ($null -eq $items.SelectSingleNode('/List/Items/Item[@Name="Sphere1"]')) 'Canonical Sphere1 appeared; reconcile its real category and behavior.'
[xml]$archiveItems = Get-Content -Raw (Join-Path $root 'Assets/DExml/list.xml')

# Guard_Girl/Guard_Man are synthesized by sensei_guard_templates.lua (Default + voice);
# only the Sphere1 identity below is a controlled fixture (its mapping is checked later).
# The full body rig, equipment category
# and actual charge behavior remain unverified. All other imports are canonical.
[xml]$charge='<Item Name="Sphere1" Type="Ranged" SubType="FixtureOnly"/>'
$dependencies=@'
local deps={
 guard_girl=require("content.sensei_guard_templates").resolve().guard_girl,
 guard_man=require("content.sensei_guard_templates").resolve().guard_man,
 sphere1=sf2.items.get("core:items/ranged/Sphere1"),
}
'@
$entry=$pending+$dependencies+@'
local acts=pending.register(deps)
assert(#acts==6)
local zone=sf2.zones.register{id="test"}
for act=1,6 do
 local count=act==6 and 2 or 3
 assert(#acts[act].normal==count and #acts[act].eclipse==count)
 local normal=sf2.battles.register{id="normal_"..act,zone=zone,type=sf2.battles.STORY}
 local eclipse=sf2.battles.register{id="eclipse_"..act,zone=zone,type=sf2.battles.FINAL}
 for index,warrior in ipairs(acts[act].normal) do
  sf2.fights.register{id="act_"..act.."_normal_"..index,battle=normal,warriors={warrior}}
 end
 sf2.fights.register{id="act_"..act.."_eclipse",battle=eclipse,warriors=acts[act].eclipse}
end
'@
# Archive rows name the missing Guard_* templates. The synthesized equivalent is
# Template="Default" plus the voice below; compare against that explicit mapping.
function Synthesized-Guards([Xml.XmlNode]$warriors) {
    $copy=$warriors.CloneNode($true)
    foreach($warrior in @($copy.SelectNodes('Warrior'))) {
        $template=$warrior.GetAttribute('Template')
        if($template -eq 'Guard_Girl' -or $template -eq 'Guard_Man') {
            Check (!$warrior.HasAttribute('Voice')) 'Archived guard row now carries its own voice; revisit the synthesis.'
            $warrior.SetAttribute('Template','Default')
            $warrior.SetAttribute('Voice',$(if($template -eq 'Guard_Girl'){'Female'}else{'Male'}))
            $script:synthesizedGuards++
        }
    }
    return $copy
}
$script:synthesizedGuards=0
$catalog=Load-Lua $entry $null @($charge.DocumentElement)
Check ($catalog.Warriors.Count -eq 34) 'Expected 12 boss and 22 guard/prince definitions.'
Check ($catalog.Fights.Count -eq 23) 'Expected 17 normal fights and six Eclipse gauntlets.'
$adapter=[Eclipse.Modding.LegacyContentAdapter]::new($catalog)
$build=[Eclipse.Modding.LegacyContentAdapter].GetMethod('BuildFightNode',[Reflection.BindingFlags]'Instance,NonPublic')
$normal=0;$eclipse=0;$guards=0
foreach($fight in $catalog.Fights) {
    $id=$fight.Id.LocalId
    $match=[regex]::Match($id,'^act_([1-6])_(normal_([1-3])|eclipse)$')
    Check $match.Success ('Unexpected fight identity: '+$id)
    $act=$match.Groups[1].Value
    $isEclipse=$id.EndsWith('eclipse')
    $battle=if($isEclipse){'SENSEI_MEMORIES_ECLIPSEMODE'}else{'SENSEI_MEMORIES'}
    $number=if($isEclipse){'1'}else{$match.Groups[3].Value}
    $expected=$archive.SelectSingleNode('//Zone[@Name="ZONE_'+$act+'"]/Battle[@Name="'+$battle+'"]/Fight[@Name="'+$number+'"]/Warriors')
    $actual=$build.Invoke($adapter,@([Xml.XmlDocument]::new(),$fight))
    $actual.OuterXml | Set-Content (Join-Path $fixture ($id+'-opponents.xml'))
    Check ((Shape $actual.Warriors) -ceq (Shape (Synthesized-Guards $expected))) ('Ordered encounter opponents differ: '+$id+"`n"+$actual.Warriors.OuterXml+"`n"+$expected.OuterXml)
    foreach($warrior in $expected.Warrior) {
        if($isEclipse){$eclipse++}else{$normal++}
        if($warrior.GetAttribute('Template').StartsWith('Guard_')){$guards++}
    }
}
Check ($normal -eq 17 -and $eclipse -eq 17 -and $guards -eq 22) 'Encounter roster coverage differs.'
Check ($script:synthesizedGuards -eq 22) 'Not every guard row used the synthesized template mapping.'
# Invalid caller-supplied handle types must fail the actual typed API and roll back
# previously registered boss definitions, not leave a partial playable story.
foreach($bad in @('deps.guard_girl=deps.sphere1','deps.guard_man={}','deps.sphere1=deps.guard_man')) {
    $failed=$false
    try {$null=Load-Lua ($pending+$dependencies+$bad+'; pending.register(deps)') $null @($charge.DocumentElement)} catch {$failed=$true}
    Check $failed ('Invalid dependency accepted: '+$bad)
}
Check ($script:ownedPortraits.ContainsKey('character_pirate')) 'Pirate avatar did not use the shipped DE 1.0.6 portrait.'
# Sphere1 evidence: the archived prince item is exactly the restored Minor Charge row.
$sphere=$archiveItems.SelectSingleNode('/List/Items/Item[@Name="Sphere1"]')
Check ($null -ne $sphere) 'Archived Sphere1 item disappeared.'
$row=[regex]::Match((Get-Content -Raw (Join-Path $root 'Mods/de128/scripts/content/restored_equipment.lua')),'\{ "minor_charge_of_darkness", "Minor Charge of Darkness", "magic", "([^"]+)", "([^"]+)", "([^"]+)", (\d+), (\d+), "([^"]+)", "([^"]+)", (\d+) \}')
Check $row.Success 'Restored Minor Charge row changed shape.'
$enchant=$sphere.SelectSingleNode('Enchantments/Perk')
Check ($sphere.Image -ceq $row.Groups[1].Value -and $sphere.Model -ceq $row.Groups[2].Value -and $sphere.Type -ceq 'Magic' -and
    $sphere.SubType -ceq $row.Groups[3].Value -and $sphere.Level -ceq $row.Groups[4].Value -and $sphere.BonusPrice -ceq $row.Groups[5].Value -and
    $sphere.PackLabel -ceq $row.Groups[6].Value -and $enchant.Name -ceq $row.Groups[7].Value -and $enchant.Set.Aspect -ceq $row.Groups[8].Value) 'Sphere1 no longer matches restored Minor Charge of Darkness.'
Check ((Get-Content -Raw (Join-Path $root 'Mods/de128/scripts/content/sensei_dependencies.lua')) -match 'restored_equipment"\)\.minor_charge_of_darkness') 'Sphere1 dependency is not the restored Minor Charge handle.'
Write-Output "PASS: $script:checks Sensei opponent checks, all 34 ordered loadouts across 23 encounters. Evidence: $fixture. Guard templates use the synthesized Default+voice mapping; Sphere1 maps to restored Minor Charge by archive evidence (its projection uses a controlled identity). No model, AI or complete story playtest claim."
