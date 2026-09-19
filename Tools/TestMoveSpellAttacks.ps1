# Actual Lua projection, recovered attack/distance parsing and signed predicate checks.
. (Join-Path $PSScriptRoot 'TestMoveSpellSelection.ps1')
$spellLua=@'
local animation=sf2.assets.binary("animations/chinese")
local options={no_effect=true,no_critical=true,ignores_block=true,body_part="Body",
 defense_types={"BodyDefense"},ignores_invulnerable={"Evade","Recovery","Dash","ShroudInterval"}}
local distance={type="distance",axis="X",maximum=-250,
 from={object="Nodes",part="Magic-Node2_1"},to={object="Wall",part="Front"}}
sf2.moves.register {id="sphere",animation=animation,conditions={distance},intervals={{type="Attack",start=13,
 attack={edges={"Fireball-Edge1"},damage=0.45,damage_terms={{type="MagicDamage"},{type="UnarmedDamage",shift=-25}},
 impulse={x=500},hit="High",options=options}}}}
'@
$catalog=Load-Lua $spellLua
$doc=Project $catalog
$node=$doc.SelectSingleNode('//Move')
$expectedAttack=$archive.SelectSingleNode('//Move[@Name="Sphere1Start"]/Intervals/Interval[@Type="Attack"]')
$parsed=Parse-Attack $node.Intervals.Interval;$baseline=Parse-Attack $expectedAttack
foreach($field in [IntervalAttack].GetFields($flags)) {
 $left=$field.GetValue($parsed);$right=$field.GetValue($baseline)
 if($null -eq $left -or $field.FieldType.IsPrimitive -or $left -is [string] -or $left -is [Collections.IList]) {
  Check (($left | ConvertTo-Json -Depth 8 -Compress) -ceq ($right | ConvertTo-Json -Depth 8 -Compress)) ('Sphere native attack mismatch: '+$field.Name)
 }
}
$distanceNode=$node.Conditions.Distance
$expectedDistance=$archive.SelectSingleNode('//Move[@Name="Sphere1Wall"]/Conditions/Distance')
Check ((Shape $distanceNode) -ceq (Shape $expectedDistance)) 'Archived wall cleanup declaration differs.'
$actualDistance=Parse-Condition $distanceNode;$sourceDistance=Parse-Condition $expectedDistance
foreach($field in [ConditionDistance].GetFields($flags)) {
 Check (($field.GetValue($actualDistance) | ConvertTo-Json -Depth 8 -Compress) -ceq ($field.GetValue($sourceDistance) | ConvertTo-Json -Depth 8 -Compress)) ('Native distance mismatch: '+$field.Name)
}
# Real signed native comparison on distance points independent of loaded rig geometry.
foreach($axis in @('X','Y')) {
 foreach($negated in @($false,$true)) {
  foreach($value in @(-251,-250,-249)) {
   [xml]$fixtureNode='<Distance Axis="'+$axis+'" Max="-250" Not="'+([int]$negated)+'"><From Object="Floor"/><To Object="Floor" Shift'+$axis+'="'+$value+'"/></Distance>'
   $native=Parse-Condition $fixtureNode.DocumentElement
   $state=[ModelConditions]::new();$state.PCAOCHAIBJC=1
   Check ($native.IsEqual($state) -eq (($value -le -250) -xor $negated)) 'Native signed distance bound/negation mismatch.'
  }
 }
}
$full=Project (Load-Lua $spellLua.Replace('axis="X"','axis="Full"'))
Check (!$full.SelectSingleNode('//Move/Conditions/Distance').HasAttribute('Axis')) 'Full distance incorrectly projected as Y.'
$fingerprint=Fingerprint $catalog
foreach($mutation in @('options.no_effect=false','options.no_critical=false','options.ignores_block=false','options.body_part="Head"',
 'options.defense_types={"HeadDefense"}','options.ignores_invulnerable={"Evade"}','distance.axis="Y"',
 'distance.minimum=-500','distance.maximum=-251','distance["not"]=true','distance.from.player="Parent"',
 'distance.from.part="Other"','distance.to.part="Back"','distance.from.shift_x=1','distance.to.shift_y=1')) {
 Check ($fingerprint -cne (Fingerprint (Load-Lua $spellLua.Replace('sf2.moves.register',($mutation+"`n"+'sf2.moves.register'))))) ('Attack/distance fingerprint omitted: '+$mutation)
}
foreach($mutation in @('options.no_effect=1','options.no_critical="yes"','options.ignores_block={}','options.body_part="Foot"',
 'options.defense_types={"MagicDefense"}','options.defense_types={"BodyDefense","BodyDefense"}',
 'options.ignores_invulnerable={"Evade|Dash"}','options.ignores_invulnerable={"Evade","Evade"}',
 'options.ignores_invulnerable={[2]="Evade"}','options.extra=1','distance.axis="Z"','distance.from.object="Animation"',
 'distance.from=nil','distance.minimum=0','distance.maximum=1/0','distance.minimum=-1000001','distance.player="Enemy"',
 'distance.from.unknown=true','distance["not"]=1')) {
 $failure=$null;try {$null=Load-Lua $spellLua.Replace('sf2.moves.register',($mutation+"`n"+'sf2.moves.register'))}catch{$failure=$_}
 Check ($null -ne $failure) ('Invalid spell attack/distance accepted: '+$mutation)
}
$legacy='sf2.moves.register_template {id="legacy",intervals={{type="Attack",attack={edges={"Edge"},damage=0.06}}}}'
Check ((Fingerprint (Load-Lua $legacy)) -ceq (Fingerprint (Load-Lua $legacy.Replace('damage=0.06','damage=0.06,options={}')))) 'Empty options changed legacy fingerprint.'
Write-Output "PASS $script:checks combined move checks including native Sphere attack options and signed distance predicates. No live projectile/contact acceptance claim."
