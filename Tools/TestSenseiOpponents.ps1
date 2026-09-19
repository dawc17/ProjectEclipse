# Real Lua and projection, with explicit identity-only fixtures for missing inputs.
# These fixtures are NOT restored templates/equipment and never enter Assets/Mods.
. (Join-Path $PSScriptRoot 'TestWarriorPerkLoadouts.ps1')
Copy-Item (Join-Path $root 'Mods/de128/scripts/content/sensei_guard_opponents.lua') (Join-Path $package 'scripts/content')
$script:checks=0
$pending='local pending=require("content.sensei_guard_opponents"); '
Check ((Load-Lua $pending).Warriors.Count -eq 0) 'Requiring the pending factory registered content.'
foreach($argsText in @('nil','{}','{guard_girl={}}','{guard_girl={},guard_man={}}')) {
    $failed=$false
    try {$null=Load-Lua ($pending+'pending.register('+$argsText+')')} catch {
        $failed=$_.ToString().Contains('require verified Guard_Girl, Guard_Man and Sphere1 handles')
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

# Only the identities below are fabricated. The full body rig, equipment category
# and actual charge behavior remain unverified. All other imports are canonical.
[xml]$identities='<Templates><Template Name="Guard_Girl"/><Template Name="Guard_Man"/></Templates>'
[xml]$charge='<Item Name="Sphere1" Type="Ranged" SubType="FixtureOnly"/>'
$dependencies=@'
local deps={
 guard_girl=sf2.warriors.get_template("core:warrior-templates/guard_girl"),
 guard_man=sf2.warriors.get_template("core:warrior-templates/guard_man"),
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
$catalog=Load-Lua $entry $identities.DocumentElement @($charge.DocumentElement)
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
    Check ((Shape $actual.Warriors) -ceq (Shape $expected)) ('Ordered encounter opponents differ: '+$id+"`n"+$actual.Warriors.OuterXml+"`n"+$expected.OuterXml)
    foreach($warrior in $expected.Warrior) {
        if($isEclipse){$eclipse++}else{$normal++}
        if($warrior.GetAttribute('Template').StartsWith('Guard_')){$guards++}
    }
}
Check ($normal -eq 17 -and $eclipse -eq 17 -and $guards -eq 22) 'Encounter roster coverage differs.'
# Invalid caller-supplied handle types must fail the actual typed API and roll back
# previously registered boss definitions, not leave a partial playable story.
foreach($bad in @('deps.guard_girl=deps.sphere1','deps.guard_man={}','deps.sphere1=deps.guard_man')) {
    $failed=$false
    try {$null=Load-Lua ($pending+$dependencies+$bad+'; pending.register(deps)') $identities.DocumentElement @($charge.DocumentElement)} catch {$failed=$true}
    Check $failed ('Invalid dependency accepted: '+$bad)
}
Write-Output "PASS: $script:checks Sensei opponent checks, all 34 ordered loadouts across 23 encounters. Evidence: $fixture. Guard templates and Sphere1 are identity-only test fixtures; no model, AI, charge mapping or complete story playtest claim."
