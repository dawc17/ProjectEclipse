# Actual Lua, native predicate evaluation and motion parser; no live spell claim.
. (Join-Path $PSScriptRoot 'TestMoveProjectiles.ps1')
$selectionLua=@'
local animation=sf2.assets.binary("animations/chinese")
local conditions={
 {type="actor_name",name="Sphere1"},
 {type="bullets",bullet_type="MagicBullet",minimum=1},
}
local motion={x=30}
sf2.moves.register {id="spell",animation=animation,conditions=conditions,velocity=motion,no_magic_recharge=true}
'@
$catalog=Load-Lua $selectionLua
$doc=Project $catalog
$node=$doc.SelectSingleNode('//Move')
Check ((Shape $node.Conditions.Name) -ceq (Shape $archive.SelectSingleNode('//Move[@Name="Sphere1Middle"]/Conditions/Name'))) 'Actor name differs from archive.'
Check ((Shape $node.Conditions.Bullets) -ceq (Shape $archive.SelectSingleNode('//Move[@Name="Sphere1Player"]/Conditions/Bullets'))) 'Charge condition differs from archive.'
Check ((Shape $node.Velocity) -ceq (Shape $archive.SelectSingleNode('//Move[@Name="Sphere1Middle"]/Velocity'))) 'Projectile motion differs from archive.'
Check ($node.GetAttribute('NoMagicRecharge') -ceq '1') 'No-recharge flag missing.'
foreach($negated in @($false,$true)) {
 foreach($bulletType in @('MagicBullet','RaidChargeBullet')) {
  $lua=$selectionLua.Replace('minimum=1','minimum=1,maximum=2').Replace('MagicBullet',$bulletType)
  if($negated) {$lua=$lua.Replace('name="Sphere1"','name="Sphere1",["not"]=true').Replace('minimum=1','["not"]=true,minimum=1')}
  $projection=Project (Load-Lua $lua)
  $native=[Collections.Generic.List[ConditionAnimation]]::new()
  [ConditionsParser]::ParseInside($native,$projection.SelectSingleNode('//Move/Conditions'))
  foreach($name in @('Sphere1','sphere1','Other')) {
   foreach($count in @(0,1,2,3)) {
    $state=[ModelConditions]::new();$state.ModelName=$name
    $state.JJDNDOLCMMN=if($bulletType -eq 'MagicBullet'){$count}else{999}
    $state.KHDBLNPFDPE=if($bulletType -eq 'RaidChargeBullet'){$count}else{999}
    Check ($native[0].IsEqual($state) -eq (($name -ceq 'Sphere1') -xor $negated)) 'Actor predicate/case/negation mismatch.'
    Check ($native[1].IsEqual($state) -eq (($count -ge 1 -and $count -le 2) -xor $negated)) 'Native charge range/type/negation mismatch.'
   }
  }
 }
}
$velocityParser=[MovesParser].GetMethod('JGLOLDJFFKC',$staticFlags)
$actualMove=[InfoAnimation]::new();$expectedMove=[InfoAnimation]::new()
$templates=[Collections.Generic.List[Xml.XmlNode]]::new()
$null=$velocityParser.Invoke($null,@($actualMove,$node,$templates))
$null=$velocityParser.Invoke($null,@($expectedMove,$archive.SelectSingleNode('//Move[@Name="Sphere1Middle"]'),$templates))
foreach($method in @('LBJFGCFGMDI','NCENGIOMKOF','HOPDDLNABCG')) {
 Check (($actualMove.$method() | ConvertTo-Json -Depth 4 -Compress) -ceq ($expectedMove.$method() | ConvertTo-Json -Depth 4 -Compress)) ('Native motion differs: '+$method)
}
$full=Project (Load-Lua $selectionLua.Replace('local motion={x=30}','local motion={x=-30,y=2,z=3,ax=4,ay=-5,az=6,save_velocity=true}'))
$fullMove=[InfoAnimation]::new();$null=$velocityParser.Invoke($null,@($fullMove,$full.SelectSingleNode('//Move'),$templates))
Check ($fullMove.LBJFGCFGMDI().GetX() -eq -30 -and $fullMove.LBJFGCFGMDI().GetY() -eq 2 -and $fullMove.LBJFGCFGMDI().GetZ() -eq 3) 'Native velocity axes lost.'
Check ($fullMove.NCENGIOMKOF().GetX() -eq 4 -and $fullMove.NCENGIOMKOF().GetY() -eq -5 -and $fullMove.NCENGIOMKOF().GetZ() -eq 6 -and $fullMove.HOPDDLNABCG()) 'Native acceleration/preserve flag lost.'
$fingerprint=Fingerprint $catalog
foreach($mutation in @('conditions[1].name="Other"','conditions[1].player="Parent"','conditions[1]["not"]=true',
 'conditions[2].minimum=2','conditions[2].maximum=3','conditions[2].bullet_type="RaidChargeBullet"','conditions[2].player="Enemy"',
 'motion.x=31','motion.y=1','motion.z=1','motion.ax=1','motion.ay=1','motion.az=1','motion.save_velocity=true')) {
 Check ($fingerprint -cne (Fingerprint (Load-Lua $selectionLua.Replace('sf2.moves.register',($mutation+"`n"+'sf2.moves.register'))))) ('Spell field absent from fingerprint: '+$mutation)
}
Check ($fingerprint -cne (Fingerprint (Load-Lua $selectionLua.Replace('no_magic_recharge=true','no_magic_recharge=false')))) 'Recharge flag absent from fingerprint.'
foreach($mutation in @('conditions[1].name=""','conditions[1].name="bad/path"','conditions[1].player="Everyone"','conditions[1].item_type="Magic"',
 'conditions[2].bullet_type="Unknown"','conditions[2].minimum=-1','conditions[2].minimum=0.5','conditions[2].maximum=0',
 'conditions[2].maximum=2147483648','conditions[2].maximum=1/0','conditions[2].name="bad"','conditions[2].player="Invalid"',
 'motion.x=1/0','motion.ax=0/0','motion.y=100001','motion.az=-100001','motion.save_velocity=1','motion.extra=1')) {
 $failure=$null;try {$null=Load-Lua $selectionLua.Replace('sf2.moves.register',($mutation+"`n"+'sf2.moves.register'))}catch{$failure=$_}
 Check ($null -ne $failure) ('Invalid spell declaration accepted: '+$mutation)
}
foreach($field in @('velocity={x=30}','no_magic_recharge=true')) {
 $failure=$null;try {$null=Load-Lua ('sf2.moves.register_template {id="bad",'+$field+'}')}catch{$failure=$_}
 Check ($null -ne $failure) ('Move-only spell field accepted on template: '+$field)
}
Write-Output "PASS $script:checks combined move checks, including native actor/charge predicate matrix and projectile velocity/acceleration parsing. No live spell acceptance claim."
