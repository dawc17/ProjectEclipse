# Actual Lua bindings, projection, and recovered effect parsers; no rendered Unity effects.
. (Join-Path $PSScriptRoot 'TestMovePresentationAuthoring.ps1')
$effectLua=@'
local animation=sf2.assets.binary("animations/chinese")
local effect={name="SmallSphereStart",core_sequence="mgc_magic_small_sphere_start",scale=0.75,time_scale=1.45,
    position={player="Me",object="Nodes",part="Magic-Node2_1",shift_x=0,shift_y=80},follow=true}
local actions={
    {type="effect",frame=2,effect=effect},
    {type="effect",event="AnimationEnd",effect={name="SmallSphereMiddle",core_sequence="mgc_magic_small_sphere_middle",
        scale=0.75,time_scale=1,looped=true,position={player="Me",object="Nodes",part="Magic-Node2_1",shift_x=-70,shift_y=55},follow=true}},
    {type="stop_effect",frame=2,effect_name="SmallSphereMiddle"},
    {type="stop_follow_effect",event="Strike",effect_name="SmallSphereStart"},
}
sf2.moves.register {id="effects",animation=animation,actions=actions}
'@
$catalog=Load-Lua $effectLua
$doc=Project $catalog
$nodes=$doc.SelectNodes('//Move/Actions/*')
Check ($nodes.Count -eq 4) 'Scheduled effect actions lost.'
$sourceNodes=@($archive.SelectSingleNode('//Move[@Name="Sphere1Start"]/Actions/Effect[@Name="SmallSphereStart"]'),
    $archive.SelectSingleNode('//Move[@Name="Sphere1Start"]/Actions/Effect[@Name="SmallSphereMiddle"]'),
    $archive.SelectSingleNode('//Move[@Name="ShopMagicTryOnSphere1End"]/Actions/StopEffect'))
for($i=0;$i -lt 3;$i++) {
    $native=[ActionsParser]::Create($nodes[$i]);$source=[ActionsParser]::Create($sourceNodes[$i])
    Check ($native.GetType() -eq $source.GetType()) 'Effect parser type changed.'
    foreach($field in $native.GetType().GetFields($flags)) {
        $actual=$field.GetValue($native) | ConvertTo-Json -Depth 8 -Compress
        $expectedValue=$field.GetValue($source) | ConvertTo-Json -Depth 8 -Compress
        Check ($actual -ceq $expectedValue) ('Archive native effect mismatch: '+$field.Name)
    }
    if($i -eq 1) {Check ($native.NeedStart([EventAnimation+EECEJKADLCK]::EVENT_ANIMATION_END) -and !$native.NeedStart(2)) 'Effect event scheduling lost.'}
    else {Check ($native.NeedStart(2) -and !$native.NeedStart(1)) 'Effect frame scheduling lost.'}
}
$stopFollow=[ActionsParser]::Create($nodes[3])
Check ($stopFollow -is [ActionStopFollowEffect]) 'Wrong stop-follow native action.'
Check ($stopFollow.get_Name() -ceq 'SmallSphereStart') 'Stop-follow name lost.'
Check ($stopFollow.NeedStart([EventAnimation+EECEJKADLCK]::EVENT_STRIKE) -and !$stopFollow.NeedStart(2)) 'Stop-follow event lost.'
$defaultLua=$effectLua.Replace('actions=actions','actions={{type="effect",frame=0,effect={name="Default",core_sequence="sequence"}}}')
$defaultDoc=Project (Load-Lua $defaultLua)
$defaultNode=$defaultDoc.SelectSingleNode('//Move/Actions/Effect')
Check ($null -eq $defaultNode.Position -and $defaultNode.GetAttribute('Scale') -eq '1' -and $defaultNode.GetAttribute('TimeScale') -eq '1' -and $defaultNode.GetAttribute('Looped') -eq '0') 'Effect defaults differ.'
Check ($null -ne [ActionsParser]::Create($defaultNode)) 'Positionless native effect failed.'
$baseline=Fingerprint $catalog
foreach($mutation in @(
 'effect.name="Changed"','effect.core_sequence="changed_sequence"','effect.scale=0.5','effect.time_scale=0.5',
 'effect.looped=true','effect.follow=false','effect.position=nil;effect.follow=false','effect.position.player="Parent"',
 'effect.position.object="Pivot"','effect.position.part="Other"','effect.position.shift_x=1','effect.position.shift_y=81',
 'actions[3].effect_name="Other"','actions[4].effect_name="Other"','actions[3].type="stop_follow_effect"'
)) {
    $changed=$effectLua.Replace('sf2.moves.register',($mutation+"`n"+'sf2.moves.register'))
    Check ($baseline -cne (Fingerprint (Load-Lua $changed))) ('Effect fingerprint omitted: '+$mutation)
}
foreach($mutation in @(
 'effect.name=""','effect.name="../bad"','effect.core_sequence="bad/path"','effect.scale=0','effect.scale=-1','effect.scale=101',
 'effect.scale=0/0','effect.time_scale=1/0','effect.time_scale=0','effect.time_scale="1"','effect.looped=1','effect.follow="yes"',
 'effect.position=nil','effect.position.object="Animation"','effect.position={object="Nodes"}','effect.position.shift_x=1/0',
 'effect.extra=true','effect.position.extra=true','actions[1].effect=nil','actions[1].effect="bad"','actions[1].core_sounds={"sound"}',
 'actions[3].effect_name=nil','actions[3].effect_name="bad/path"','actions[3].effect=effect','actions[4].effect_name=""',
 'actions[1].event="Strike"','actions[1].frame=0.5','actions[3].effect_name=false'
)) {
    $failure=$null
    try {$null=Load-Lua $effectLua.Replace('sf2.moves.register',($mutation+"`n"+'sf2.moves.register'))}catch{$failure=$_}
    Check ($null -ne $failure) ('Invalid effect accepted: '+$mutation)
}
$effect=[Eclipse.Modding.ModMoveEffect]::new('test','sequence')
foreach($kind in @('random_sound','try_on_end','stop_effect','stop_follow_effect')) {
    $failure=$null
    try {$null=[Eclipse.Modding.ModMoveScheduledAction]::new($kind,1,$null,[string[]]@('sound'),$effect,'test')}catch{$failure=$_}
    Check ($null -ne $failure) ('Direct action accepted incompatible effect payload: '+$kind)
}
Write-Output "PASS $script:checks combined move checks including archived native effect parsing, scheduling, strict Lua validation and fingerprints. No live effect rendering claim."
