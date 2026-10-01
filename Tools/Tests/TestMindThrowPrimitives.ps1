# Actual Lua bindings, production projection and recovered native MindThrow primitives.
. (Join-Path $PSScriptRoot 'TestMoveSpellAttacks.ps1')
$mindLua=@'
local sound={core_sound="snd_m_pl_attack6",voice="Male"}
local shake={pause_time=0,effect_time=30,amplitude_x=7,frequency_x=1,amplitude_y=9,frequency_y=0.3}
local attack={direct=true,damage=0.15,hit="NoReaction",
 damage_terms={MagicDamage=0,UnarmedDamage=-25},
 options={no_critical=true,body_part="Body",defense_types={"BodyDefense"}}}
sf2.moves.register {id="mind",animation=sf2.assets.binary("animations/chinese"),
 intervals={{type="Attack",from=48,to=49,attack=attack}},timeline={
 [12]={play_sound=sound.core_sound,voice=sound.voice},[48]={shake=shake}}}
'@
$catalog=Load-Lua $mindLua
$doc=Project $catalog
$node=$doc.SelectSingleNode('//Move')
Check ($null -eq $node.SelectSingleNode('./Intervals/Interval/AttackingParts')) 'Direct attack still has collision geometry.'
$actual=Parse-Attack $node.Intervals.Interval
$expected=Parse-Attack $archive.SelectSingleNode('//Move[@Name="MindThrowPlayer2Normal"]/Intervals/Interval[@Type="Attack"]')
foreach($field in [IntervalAttack].GetFields($flags)) {
 $left=$field.GetValue($actual);$right=$field.GetValue($expected)
 if($null -eq $left -or $field.FieldType.IsPrimitive -or $left -is [string] -or $left -is [Collections.IList]) {
  Check (($left | ConvertTo-Json -Depth 8 -Compress) -ceq ($right | ConvertTo-Json -Depth 8 -Compress)) ('Direct native attack mismatch: '+$field.Name)
 }
}
# This branch must never inspect geometry: null models deliberately catch that.
$collision=[ModelCollision]::new($null)
$edges=[Collections.Generic.List[ModelEdge]]::new()
Check ($collision.Render($null,$edges,$actual)) 'Direct native collision did not fire.'
Check (!$collision.Render($null,$edges,$actual)) 'Direct native collision repeated within an interval.'
$collision.ResetInterval()
Check ($collision.Render($null,$edges,$actual)) 'Direct native collision did not reset.'
Check ($actual.GetReactionName(48) -ceq 'NoReaction') 'NoReaction changed during native parsing.'
$sourceSound=$archive.SelectSingleNode('//Move[@Name="MindThrowPlayerNormal"]/Actions/Sound[@Voice="Male"]')
Check ((Shape $node.Actions.Sound) -ceq (Shape $sourceSound)) 'Archived voice sound projection differs.'
foreach($voice in @('Male','MaleLow','Female')) {
 $soundDoc=Project (Load-Lua $mindLua.Replace('voice="Male"',('voice="'+$voice+'"')))
 $sound=[ActionSound]::new($soundDoc.SelectSingleNode('//Sound'))
 foreach($candidate in @('Male','MaleLow','Female','Other')) {
  Check ($sound.SameGender($candidate) -eq ($candidate -ceq $voice)) 'Native voice filtering changed.'
 }
 Check ($sound.NeedStart(12) -and !$sound.NeedStart(11)) 'Native voice sound timing changed.'
}
$unfiltered=Project (Load-Lua $mindLua.Replace(',voice="Male"',''))
Check ([ActionSound]::new($unfiltered.SelectSingleNode('//Sound')).SameGender('Other')) 'Omitted voice no longer allows any voice.'
$expectedShake=$archive.SelectSingleNode('//Move[@Name="MindThrowPlayer2Normal"]/Actions/ShakeScreen')
Check ((Shape $node.Actions.ShakeScreen) -ceq (Shape $expectedShake)) 'Archived shake projection differs.'
$actualShake=[ActionShakeScreen]::new($node.Actions.ShakeScreen)
$nativeShake=[ActionShakeScreen]::new($expectedShake)
Check ($actualShake.NeedStart(48) -and !$actualShake.NeedStart(47)) 'Native shake timing changed.'
foreach($field in $actualShake.CBNIELBJDAO().GetType().GetFields()) {
 Check ($field.GetValue($actualShake.CBNIELBJDAO()) -eq $field.GetValue($nativeShake.CBNIELBJDAO())) ('Shake native payload mismatch: '+$field.Name)
}
$fp=Fingerprint $catalog
foreach($mutation in @('sound.voice="Female"','sound.core_sound="snd_f_pl_attack6"','shake.pause_time=1','shake.effect_time=31',
 'shake.amplitude_x=8','shake.amplitude_y=10','shake.frequency_x=2','shake.frequency_y=0.5','attack.direct=false;attack.edges={"Edge"}')) {
 Check ($fp -cne (Fingerprint (Load-Lua $mindLua.Replace('sf2.moves.register',($mutation+"`n"+'sf2.moves.register'))))) ('Fingerprint missed '+$mutation)
}
foreach($mutation in @('attack.direct=false','attack.direct="yes"','attack.edges={"Edge"}','sound.voice="Unknown"','sound.voice=1',
 'sound.core_sound="bad/path"','shake.pause_time=-1','shake.effect_time=10001','shake.pause_time=0.5',
 'shake.amplitude_x=-1','shake.amplitude_y=1/0','shake.frequency_x=1001','shake.frequency_y="bad"','shake.extra=true')) {
 $failure=$null;try {$null=Load-Lua $mindLua.Replace('sf2.moves.register',($mutation+"`n"+'sf2.moves.register'))}catch{$failure=$_}
 Check ($null -ne $failure) ('Invalid payload accepted: '+$mutation)
}
foreach($bad in @($mindLua.Replace('play_sound=sound.core_sound','try_on_end=sound.core_sound'),
 $mindLua.Replace('shake=shake','play_sound=shake'),
 $mindLua.Replace('play_sound=sound.core_sound','play_sound=1'),$mindLua.Replace('shake=shake','shake=1'))) {
 $failure=$null;try {$null=Load-Lua $bad}catch{$failure=$_}
 Check ($null -ne $failure) 'Cross-kind/malformed payload accepted.'
}
$legacy='sf2.moves.register_template {id="legacy",intervals={{type="Attack",attack={edges={"Edge"},damage=0.06}}}}'
Check ((Fingerprint (Load-Lua $legacy)) -ceq (Fingerprint (Load-Lua $legacy.Replace('damage=0.06','damage=0.06,direct=false')))) 'Explicit direct=false changed legacy fingerprint.'
Write-Output "PASS $script:checks combined checks: MindThrow native direct collision, NoReaction, voice filtering and shake scheduling. No live fight/presentation claim."
