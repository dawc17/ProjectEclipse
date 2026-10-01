# Actual Lua bindings, projection, and recovered effect parsers; no rendered Unity effects.
. (Join-Path $PSScriptRoot 'TestMovePresentationAuthoring.ps1')
$effectLua=@'
local animation=sf2.assets.binary("animations/chinese")
local effect={name="SmallSphereStart",core_sequence="mgc_magic_small_sphere_start",scale=0.75,time_scale=1.45,
    position={ node = "Magic-Node2_1", player = "Me", x = 0, y = 80 },follow=true}
local actions={
        [2] = { { effect = effect }, { stop_effect = "SmallSphereMiddle" } },
        animation_end = { effect = {name="SmallSphereMiddle",core_sequence="mgc_magic_small_sphere_middle",
        scale=0.75,time_scale=1,looped=true,position={ node = "Magic-Node2_1", player = "Me", x = -70, y = 55 },follow=true} },
        strike = { stop_follow_effect = "SmallSphereStart" },
    }
sf2.moves.register {id="effects",animation=animation,timeline = actions}
'@
$catalog=Load-Lua $effectLua
$doc=Project $catalog
$nodes=@($doc.SelectSingleNode('//Move/Actions/Effect[@Name="SmallSphereStart"]'),$doc.SelectSingleNode('//Move/Actions/Effect[@Name="SmallSphereMiddle"]'),$doc.SelectSingleNode('//Move/Actions/StopEffect'),$doc.SelectSingleNode('//Move/Actions/StopFollowEffect'))
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
$defaultLua=$effectLua.Replace('timeline = actions','timeline = {[0]={effect={name="Default",core_sequence="sequence"}}}')
$defaultDoc=Project (Load-Lua $defaultLua)
$defaultNode=$defaultDoc.SelectSingleNode('//Move/Actions/Effect')
Check ($null -eq $defaultNode.Position -and $defaultNode.GetAttribute('Scale') -eq '1' -and $defaultNode.GetAttribute('TimeScale') -eq '1' -and $defaultNode.GetAttribute('Looped') -eq '0') 'Effect defaults differ.'
Check ($null -ne [ActionsParser]::Create($defaultNode)) 'Positionless native effect failed.'
$attachLua=@'
local sf2=require("sf2")
local attach={player="Parent",root_point="NStomach",attach_point="NChest",offset_x=15,offset_y=-15,start_rotation=330}
sf2.moves.register {id="attached",animation=sf2.assets.binary("animations/chinese"),timeline = {
        [1] = { effect = {name="Aura",core_sequence="effect_electricity",attach=attach} },
    }}
'@
$attachCatalog=Load-Lua $attachLua
$attachNode=(Project $attachCatalog).SelectSingleNode('//Move/Actions/Effect/Attach')
Check ($attachNode.GetAttribute('Player') -ceq 'Parent' -and $attachNode.GetAttribute('RootPoint') -ceq 'NStomach' -and
    $attachNode.GetAttribute('AttachPoint') -ceq 'NChest' -and $attachNode.GetAttribute('OffsetVector') -ceq '15;-15' -and
    $attachNode.GetAttribute('StartRotAngle') -ceq '330') 'Effect attachment native projection differs.'
$attached=[ActionsParser]::Create($attachNode.ParentNode)
Check ($null -ne $attached.Attachment -and $attached.DIGCODDLDAD()) 'Native parser did not retain following attachment.'
$attachBaseline=Fingerprint $attachCatalog
foreach($mutation in @('attach.player="Enemy"','attach.root_point="NNeck"','attach.attach_point="NHead"',
    'attach.offset_x=16','attach.offset_y=-16','attach.start_rotation=329')) {
    Check ($attachBaseline -cne (Fingerprint (Load-Lua $attachLua.Replace('sf2.moves.register',($mutation+"`n"+'sf2.moves.register'))))) ('Effect attachment fingerprint omitted: '+$mutation)
}
foreach($mutation in @('attach.player="Both"','attach.root_point=""','attach.attach_point="bad/path"',
    'attach.offset_x=0/0','attach.offset_y=10001','attach.start_rotation=1/0','attach.extra=true',
    'attach=nil;attach={player="Me"}','attach.offset_x="15"')) {
    $failure=$null
    try {$null=Load-Lua $attachLua.Replace('sf2.moves.register',($mutation+"`n"+'sf2.moves.register'))}catch{$failure=$_}
    Check ($null -ne $failure) ('Invalid effect attachment accepted: '+$mutation)
}
foreach($addition in @('position={player="Me",node="NPivot"}', 'follow=true')) {
    $failure=$null
    try {$null=Load-Lua $attachLua.Replace('attach=attach','attach=attach,'+$addition)}catch{$failure=$_}
    Check ($null -ne $failure) ('Attached effect accepted incompatible '+$addition)
}
$baseline=Fingerprint $catalog
foreach($mutation in @(
 'effect.name="Changed"','effect.core_sequence="changed_sequence"','effect.scale=0.5','effect.time_scale=0.5',
 'effect.looped=true','effect.follow=false','effect.position=nil;effect.follow=false','effect.position.player="Parent"',
 'effect.position.node=nil;effect.position.pivot=true','effect.position.node="Other"','effect.position.x=1','effect.position.y=81',
 'actions[2][2].stop_effect="Other"','actions.strike.stop_follow_effect="Other"','actions[2][2]={stop_follow_effect="SmallSphereMiddle"}'
)) {
    $changed=$effectLua.Replace('sf2.moves.register',($mutation+"`n"+'sf2.moves.register'))
    Check ($baseline -cne (Fingerprint (Load-Lua $changed))) ('Effect fingerprint omitted: '+$mutation)
}
foreach($mutation in @(
 'effect.name=""','effect.name="../bad"','effect.core_sequence="bad/path"','effect.scale=0','effect.scale=-1','effect.scale=101',
 'effect.scale=0/0','effect.time_scale=1/0','effect.time_scale=0','effect.time_scale="1"','effect.looped=1','effect.follow="yes"',
 'effect.position=nil','effect.position={animation=true}','effect.position={node=true}','effect.position.x=1/0',
 'effect.extra=true','effect.position.extra=true','actions[2][1].effect=nil','actions[2][1].effect="bad"','actions[2][1].core_sounds={"sound"}',
 'actions[2][2].stop_effect=nil','actions[2][2].stop_effect="bad/path"','actions[2][2].effect=effect','actions.strike.stop_follow_effect=""',
 'actions[2][1].event="Strike"','actions[0.5]=actions[2];actions[2]=nil','actions[2][2].stop_effect=false'
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
