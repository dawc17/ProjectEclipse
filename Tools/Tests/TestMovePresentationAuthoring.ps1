# Production Lua binding/projection and native scheduled actions; no live audio or fight.
. (Join-Path $PSScriptRoot 'TestMoveGraphAuthoring.ps1')
(Get-Content $manifest -Raw).Replace('["content.register"]','["content.register", "content.patch"]') | Set-Content $manifest
$mod=[Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $fixture 'Mods')).Mods[0]
$presentationLua=$graphLua.Replace('conditions=data.conditions,','profile=data.profile,tactic_distance=data.tactic_distance,timeline=data.timeline,conditions=data.conditions,').Replace('locks=data.preview.locks,','timeline=data.preview.timeline,no_wall_repulsion=data.preview.no_wall_repulsion,no_interpolation_frames=data.preview.no_interpolation_frames,locks=data.preview.locks,')
$catalog=Load-Lua $presentationLua
$doc=Project $catalog
$slash=$doc.SelectSingleNode('//Move[contains(@Name,"slash")]')
$preview=$doc.SelectSingleNode('//Move[contains(@Name,"preview")]')
foreach($section in @('Profile','Tactics','Actions')) { Check ((Shape $slash[$section]) -ceq (Shape $expected[$section])) ('Archive presentation mismatch: '+$section) }
Check ((Shape $preview.Actions) -ceq (Shape $expectedPreview.Actions)) 'Archive preview actions mismatch.'
foreach($attribute in @('NoWallRepulsion','NoInterpolationFrames')) {Check ($preview.GetAttribute($attribute) -ceq $expectedPreview.GetAttribute($attribute)) ('Preview flag mismatch: '+$attribute)}
foreach($move in @($slash,$preview)) {
    foreach($entry in $move.Actions.ChildNodes) {
        $native=[ActionsParser]::Create($entry)
        Check ($null -ne $native) 'Native action parser rejected authored action.'
        if($entry.HasAttribute('Frame')) {
            $frame=[int]$entry.GetAttribute('Frame')
            Check ($native.NeedStart($frame) -and !$native.NeedStart($frame-1) -and !$native.NeedStart($frame+1)) 'Scheduled frame was not preserved.'
            Check (!$native.NeedStart([EventAnimation+EECEJKADLCK]::EVENT_STRIKE)) 'Frame action also starts on strike.'
        } else {
            $eventType=if($entry.GetAttribute('Event') -eq 'Strike'){[EventAnimation+EECEJKADLCK]::EVENT_STRIKE}else{[EventAnimation+EECEJKADLCK]::EVENT_ANIMATION_END}
            Check ($native.NeedStart($eventType) -and !$native.NeedStart(0)) 'Scheduled event was not preserved.'
        }
        if($entry.Name -eq 'RandomSound') {
            Check ($native -is [ActionRandomSound]) 'Sound action has wrong native type.'
            $names=[ActionRandomSound].GetField('_Names',$flags).GetValue($native)
            Check (($names -join '|') -ceq (($entry.Sound | ForEach-Object {$_.GetAttribute('Name')}) -join '|')) 'Random sound choices/order changed.'
            Check ($native.SameGender('Male') -and $native.SameGender('Female')) 'Unrequested voice filter introduced.'
        } else {Check ($native -is [ActionTryOnEnd]) 'Preview completion has wrong native type.'}
    }
}
$distance=[ConditionsParser]::Create($slash.Tactics.Conditions.Distance)
Check ($distance -is [ConditionDistance]) 'Tactic distance has wrong native type.'
$expectedDistance=[ConditionsParser]::Create($expected.Tactics.Conditions.Distance)
foreach($field in [ConditionDistance].GetFields($flags)) {
    $a=$field.GetValue($distance);$b=$field.GetValue($expectedDistance)
    Check (($a | ConvertTo-Json -Depth 6 -Compress) -ceq ($b | ConvertTo-Json -Depth 6 -Compress)) ('Native tactic distance differs: '+$field.Name)
}
$full=Project (Load-Lua $presentationLua.Replace('local animation=','data.tactic_distance.distance="Full"' + "`n" + 'local animation='))
Check (!$full.SelectSingleNode('//Move[contains(@Name,"slash")]/Tactics/Conditions/Distance').HasAttribute('Axis')) 'Full distance incorrectly became native Y distance.'
$tacticLua=@'
local animation=sf2.assets.binary("animations/chinese")
local checks={
 { not_animation = "Jump", player = "Enemy" },
 { any = {
  { distance = "X", min = 250, from = { pivot = "Me" }, to = { node = "NPivot", player = "Enemy" } },
  { animation = "Fall", player = "Enemy" }
 } },
}
sf2.moves.register {id="tactic",animation=animation,tactic_conditions=checks}
'@
$tacticCatalog=Load-Lua $tacticLua
$tacticNode=(Project $tacticCatalog).SelectSingleNode('//Move[contains(@Name,"tactic")]/Tactics')
Check ((Shape $tacticNode) -ceq (Shape $archive.SelectSingleNode('//Move[@Name="ButcherEarthquakePlayer"]/Tactics'))) 'Archived compound AI tactic conditions did not project exactly.'
$resultLua=@'
local animation=sf2.assets.binary("animations/chinese")
sf2.moves.register {id="result",animation=animation,
 conditions={{ round_result = "Victory" }},
 timeline = {
        [1] = { effect = {name="Storm",core_sequence="mgc_effect_levitation_middle",on_background=true} },
        hit = { stop_sound = "snd_blade_fury" },
    }}
'@
$resultCatalog=Load-Lua $resultLua
$resultNode=(Project $resultCatalog).SelectSingleNode('//Move[contains(@Name,"result")]')
Check ($resultNode.Conditions.RoundResult.GetAttribute('Name') -ceq 'Victory' -and
    ([ConditionsParser]::Create($resultNode.Conditions.RoundResult) -is [ConditionRoundResult])) 'Typed round result did not reach the native condition parser.'
$background=[ActionsParser]::Create($resultNode.Actions.Effect)
Check ($resultNode.Actions.Effect.GetAttribute('OnBackground') -ceq '1' -and
    $background -is [ActionEffect] -and $background.JNAALMFCPCN()) 'Background effect did not reach the native renderer contract.'
Check ($resultNode.Actions.StopSound.GetAttribute('Name') -ceq 'snd_blade_fury' -and
    ([ActionsParser]::Create($resultNode.Actions.StopSound) -is [ActionStopSound])) 'Typed sound stop did not reach the native parser.'
Check ((Fingerprint $resultCatalog) -cne (Fingerprint (Load-Lua $resultLua.Replace('snd_blade_fury','snd_other')))) 'Sound stop is absent from the fingerprint.'
Check ((Fingerprint $resultCatalog) -cne (Fingerprint (Load-Lua $resultLua.Replace('on_background=true','on_background=false')))) 'Background effect is absent from the fingerprint.'
Check ((Fingerprint $resultCatalog) -cne (Fingerprint (Load-Lua $resultLua.Replace('round_result = "Victory"','round_result = "Defeat"')))) 'Round result is absent from the fingerprint.'
$playLua=@'
local animation=sf2.assets.binary("animations/chinese")
local child=sf2.moves.register {id="hand",animation=animation}
sf2.moves.register {id="caster",animation=animation,timeline = {
        [17] = { play_animation = child, player = "Child", child_name = "BlackHand" },
    }}
'@
$playCatalog=Load-Lua $playLua
$playNode=(Project $playCatalog).SelectSingleNode('//Move[contains(@Name,"caster")]/Actions/PlayAnimation')
$nativePlay=[ActionsParser]::Create($playNode)
Check ($nativePlay -is [ActionPlayAnimation] -and $nativePlay.AnimationName -ceq 'fixture.moves:moves/hand' -and
    $nativePlay.ChildName -ceq 'BlackHand' -and $playNode.GetAttribute('Player') -ceq 'Child' -and
    $nativePlay.NeedStart(17) -and !$nativePlay.NeedStart(16)) 'Typed child animation did not reach the native scheduled action.'
foreach($pair in @(@('[17]','[18]'),@('player = "Child"','player = "Enemy"'),
    @('child_name = "BlackHand"','child_name = "OtherHand"'))) {
    $changed=(Fingerprint (Load-Lua $playLua.Replace($pair[0],$pair[1])))
    Check ((Fingerprint $playCatalog) -cne $changed) ('Play animation field missing from the fingerprint: '+$pair[1])
}
$corePlay=(Project (Load-Lua $playLua.Replace('play_animation = child,','play_animation = "StanceIdle",'))).SelectSingleNode('//Move[contains(@Name,"caster")]/Actions/PlayAnimation')
Check ($corePlay.GetAttribute('Animation') -ceq 'StanceIdle') 'Core animation name did not project.'
foreach($pair in @(@('play_animation = child','play_animation = nil'),@('play_animation = child','play_animation = animation'),
    @('play_animation = child','play_animation = "bad/path"'),
    @('play_animation = child','play_animation = child,core_animation="StanceIdle"'),
    @('player = "Child"','player = "Both"'),@('player = "Child"','player = nil'),
    @('child_name = "BlackHand"','child_name = "../bad"'),
    @('play_animation = child','play_animation = child,effect_name="Other"'))) {
    $failure=$null;try {$null=Load-Lua $playLua.Replace($pair[0],$pair[1])}catch{$failure=$_}
    Check ($null -ne $failure) ('Invalid play_animation accepted: '+$pair[1])
}
foreach($mutation in @('round_result = "Draw"','name=""','round_result = "Victory",item_type="Weapon"')) {
 $failure=$null;try {$null=Load-Lua $resultLua.Replace('round_result = "Victory"',$mutation)}catch{$failure=$_}
 Check ($null -ne $failure) ('Invalid round result accepted: '+$mutation)
}
$failure=$null;try {$null=Load-Lua $resultLua.Replace('on_background=true','on_background="yes"')}catch{$failure=$_}
Check ($null -ne $failure) 'Nonboolean background flag accepted.'
foreach($mutation in @('stop_sound = ""','stop_sound = "../bad"','effect_name="Storm"')) {
 $failure=$null;try {$null=Load-Lua $resultLua.Replace('stop_sound = "snd_blade_fury"',$mutation)}catch{$failure=$_}
 Check ($null -ne $failure) ('Invalid sound stop accepted: '+$mutation)
}
Check ((Fingerprint $tacticCatalog) -cne (Fingerprint (Load-Lua ($tacticLua.Replace('min = 250','min = 251'))))) 'AI tactic conditions are absent from the fingerprint.'
foreach($mutation in @('tactic_distance={distance="X",from={pivot="Me"},to={pivot="Enemy"}}','tactic_conditions={[2]=checks[1]}','tactic_conditions={}')) {
 $failure=$null
 try {$null=Load-Lua $tacticLua.Replace('tactic_conditions=checks',('tactic_conditions=checks,'+$mutation))}catch{$failure=$_}
 Check ($null -ne $failure) ('Invalid compound AI tactic accepted: '+$mutation)
}
$nativeProfile=[Trick]::new($slash.Profile,[InfoAnimation]::new())
Check ($nativeProfile.Rank -eq 4 -and $nativeProfile.NHKMCLPOMFK -ceq 'Trick7.super_slash') 'Profile rank/icon changed in native parser.'
$fingerprint=Fingerprint $catalog
Check ($fingerprint -ceq (Fingerprint (Load-Lua $presentationLua))) 'Presentation fingerprint changed on reload.'
foreach($mutation in @('data.profile.rank=5','data.profile.core_icon="Other.icon"','data.tactic_distance.min=201','data.tactic_distance.max=801','data.tactic_distance.distance="Y"','data.tactic_distance.to.node="OtherNode"','data.timeline[9]=data.timeline[8];data.timeline[8]=nil','data.timeline.hit=data.timeline.strike;data.timeline.strike=nil','data.timeline.strike.sound[1]="snd_other"','data.preview.no_wall_repulsion=false','data.preview.no_interpolation_frames=false')) {
    Check ($fingerprint -cne (Fingerprint (Load-Lua $presentationLua.Replace('local animation=',($mutation+"`n"+'local animation='))))) ('Presentation field missing from fingerprint: '+$mutation)
}
foreach($mutation in @(
    'data.timeline[8].event="Strike"','data.timeline[8]={}','data.timeline[-1]=data.timeline[8]','data.timeline[100001]=data.timeline[8]','data.timeline[0.5]=data.timeline[8]',
    'data.timeline.unknown=data.timeline.strike','data.timeline[8].sound={}','data.timeline[8].sound={"../file"}','data.timeline[8].sound={[2]="snd_hit1"}',
    'data.timeline[8].type="xml"','data.timeline[8].voice="Male"','data.timeline[8]={[2]={sound="snd_hit1"}}','data.preview.timeline.animation_end.sound={"snd_hit1"}',
    'data.profile.rank=-1','data.profile.rank=0.5','data.profile.core_icon="../file"','data.profile.show=false',
    'data.tactic_distance.min=801','data.tactic_distance.max=1/0','data.tactic_distance.distance="Z"','data.tactic_distance.from.pivot=true',
    'data.tactic_distance.to={animation="Enemy"}','data.preview.no_wall_repulsion="true"'
)) {
    $failure=$null;try {$null=Load-Lua $presentationLua.Replace('local animation=',($mutation+"`n"+'local animation='))}catch{$failure=$_}
    Check ($null -ne $failure) ('Invalid presentation accepted: '+$mutation)
}
foreach($field in @('actions={}','profile={rank=4,core_icon="Trick7.super_slash"}','no_wall_repulsion=true','no_interpolation_frames=true','tactic_distance={}')) {
    $failure=$null;try {$null=Load-Lua ('sf2.moves.register_template {id="bad",'+$field+'}')}catch{$failure=$_}
    Check ($null -ne $failure) ('Move-only field accepted on template: '+$field)
}
$empty='local animation=sf2.assets.binary("animations/chinese");sf2.moves.register {id="empty",animation=animation}'
Check ((Fingerprint (Load-Lua $empty)) -ceq (Fingerprint (Load-Lua $empty.Replace('id="empty"','id="empty",timeline={},no_wall_repulsion=false,no_interpolation_frames=false')))) 'Empty presentation changed previous fingerprint.'
Write-Output "PASS $script:checks combined attack, graph and presentation checks. Native scheduling and archived presentation matched; no live audio, fight or preview."
