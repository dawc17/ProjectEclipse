# Production Lua binding/projection and native scheduled actions; no live audio or fight.
. (Join-Path $PSScriptRoot 'TestMoveGraphAuthoring.ps1')
(Get-Content $manifest -Raw).Replace('["content.register"]','["content.register", "content.patch"]') | Set-Content $manifest
$mod=[Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $fixture 'Mods')).Mods[0]
$presentationLua=$graphLua.Replace('conditions=data.conditions,','profile=data.profile,tactic_distance=data.tactic_distance,actions=data.actions,conditions=data.conditions,').Replace('locks=data.preview.locks,','actions=data.preview.actions,no_wall_repulsion=data.preview.no_wall_repulsion,no_interpolation_frames=data.preview.no_interpolation_frames,locks=data.preview.locks,')
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
$full=Project (Load-Lua $presentationLua.Replace('local animation=','data.tactic_distance.axis="Full"' + "`n" + 'local animation='))
Check (!$full.SelectSingleNode('//Move[contains(@Name,"slash")]/Tactics/Conditions/Distance').HasAttribute('Axis')) 'Full distance incorrectly became native Y distance.'
$tacticLua=@'
local animation=sf2.assets.binary("animations/chinese")
local checks={
 {type="current_animation",player="Enemy",name="Jump",["not"]=true},
 {type="any",conditions={
  {type="distance",axis="X",minimum=250,from={object="Pivot",player="Me"},to={object="Nodes",part="NPivot",player="Enemy"}},
  {type="current_animation",player="Enemy",name="Fall"}
 }},
}
sf2.moves.register {id="tactic",animation=animation,tactic_conditions=checks}
'@
$tacticCatalog=Load-Lua $tacticLua
$tacticNode=(Project $tacticCatalog).SelectSingleNode('//Move[contains(@Name,"tactic")]/Tactics')
Check ((Shape $tacticNode) -ceq (Shape $archive.SelectSingleNode('//Move[@Name="ButcherEarthquakePlayer"]/Tactics'))) 'Archived compound AI tactic conditions did not project exactly.'
Check ((Fingerprint $tacticCatalog) -cne (Fingerprint (Load-Lua ($tacticLua.Replace('minimum=250','minimum=251'))))) 'AI tactic conditions are absent from the fingerprint.'
foreach($mutation in @('tactic_distance={axis="X",from={object="Pivot",player="Me"},to={object="Pivot",player="Enemy"}}','tactic_conditions={[2]=checks[1]}','tactic_conditions={}')) {
 $failure=$null
 try {$null=Load-Lua $tacticLua.Replace('tactic_conditions=checks',('tactic_conditions=checks,'+$mutation))}catch{$failure=$_}
 Check ($null -ne $failure) ('Invalid compound AI tactic accepted: '+$mutation)
}
$nativeProfile=[Trick]::new($slash.Profile,[InfoAnimation]::new())
Check ($nativeProfile.Rank -eq 4 -and $nativeProfile.NHKMCLPOMFK -ceq 'Trick7.super_slash') 'Profile rank/icon changed in native parser.'
$fingerprint=Fingerprint $catalog
Check ($fingerprint -ceq (Fingerprint (Load-Lua $presentationLua))) 'Presentation fingerprint changed on reload.'
foreach($mutation in @('data.profile.rank=5','data.profile.core_icon="Other.icon"','data.tactic_distance.minimum=201','data.tactic_distance.maximum=801','data.tactic_distance.axis="Y"','data.tactic_distance.to.part="OtherNode"','data.actions[1].frame=9','data.actions[5].event="Hit"','data.actions[5].core_sounds[1]="snd_other"','data.preview.no_wall_repulsion=false','data.preview.no_interpolation_frames=false')) {
    Check ($fingerprint -cne (Fingerprint (Load-Lua $presentationLua.Replace('local animation=',($mutation+"`n"+'local animation='))))) ('Presentation field missing from fingerprint: '+$mutation)
}
foreach($mutation in @(
    'data.actions[1].event="Strike"','data.actions[1].frame=nil','data.actions[1].frame=-1','data.actions[1].frame=100001','data.actions[1].frame=0.5',
    'data.actions[5].event="Unknown"','data.actions[1].core_sounds={}','data.actions[1].core_sounds={"../file"}','data.actions[1].core_sounds={[2]="snd_hit1"}',
    'data.actions[1].type="xml"','data.actions[1].voice="Male"','data.actions[2]=nil','data.preview.actions[5].core_sounds={"snd_hit1"}',
    'data.profile.rank=-1','data.profile.rank=0.5','data.profile.core_icon="../file"','data.profile.show=false',
    'data.tactic_distance.minimum=801','data.tactic_distance.maximum=1/0','data.tactic_distance.axis="Z"','data.tactic_distance.from.player=nil',
    'data.tactic_distance.to.object="Animation"','data.preview.no_wall_repulsion="true"'
)) {
    $failure=$null;try {$null=Load-Lua $presentationLua.Replace('local animation=',($mutation+"`n"+'local animation='))}catch{$failure=$_}
    Check ($null -ne $failure) ('Invalid presentation accepted: '+$mutation)
}
foreach($field in @('actions={}','profile={rank=4,core_icon="Trick7.super_slash"}','no_wall_repulsion=true','no_interpolation_frames=true','tactic_distance={}')) {
    $failure=$null;try {$null=Load-Lua ('sf2.moves.register_template {id="bad",'+$field+'}')}catch{$failure=$_}
    Check ($null -ne $failure) ('Move-only field accepted on template: '+$field)
}
$empty='local animation=sf2.assets.binary("animations/chinese");sf2.moves.register {id="empty",animation=animation}'
Check ((Fingerprint (Load-Lua $empty)) -ceq (Fingerprint (Load-Lua $empty.Replace('id="empty"','id="empty",actions={},no_wall_repulsion=false,no_interpolation_frames=false')))) 'Empty presentation changed previous fingerprint.'
Write-Output "PASS $script:checks combined attack, graph and presentation checks. Native scheduling and archived presentation matched; no live audio, fight or preview."
