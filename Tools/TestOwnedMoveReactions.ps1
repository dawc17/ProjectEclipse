# Typed owned hit references and native impulse-facing behavior, no live fight.
. (Join-Path $PSScriptRoot 'TestMindThrowPrimitives.ps1')
$reactionLua=@'
local animation=sf2.assets.binary("animations/chinese")
local direction={impulse={reverse=true}}
local victim=sf2.moves.register {id="victim",animation=animation,
 events={{ hit = "fixture.moves:moves/victim" }},direction=direction}
local other=sf2.moves.register {id="other",animation=animation}
local attack={edges={"Edge"},damage=0.3,hit_move=victim}
sf2.moves.register {id="attack",animation=animation,intervals={{type="Attack",attack=attack}}}
'@
$catalog=Load-Lua $reactionLua
$doc=Project $catalog
$victim=$doc.SelectSingleNode('//Move[@Name="fixture.moves:moves/victim"]')
$attack=Parse-Attack $doc.SelectSingleNode('//Move[@Name="fixture.moves:moves/attack"]/Intervals/Interval')
Check ($attack.GetReactionName(0) -ceq 'fixture.moves:moves/victim') 'Owned reaction name lost in native attack.'
Check ($victim.Events.Hit.GetAttribute('Name') -ceq $attack.GetReactionName(0)) 'Owned hit selector no longer matches attack reaction.'
$reference=$archive.SelectSingleNode('//Move[@Name="MindThrowHitNormal"]/SetDirection')
Check ((Shape $victim.SetDirection) -ceq (Shape $reference)) 'Archived reverse-impulse direction differs.'
$parser=[MovesParser].GetMethod('JOLJIHDPADK',[Reflection.BindingFlags]'Static,Public')
foreach($reverse in @($true,$false)) {
 $projected=Project (Load-Lua $reactionLua.Replace('reverse=true',('reverse='+$reverse.ToString().ToLowerInvariant())))
 $direction=$parser.Invoke($null,@($projected.SelectSingleNode('//Move[@Name="fixture.moves:moves/victim"]/SetDirection')))
 foreach($impulse in @(-1,0,1)) {
  $state=[ModelConditions]::new();$state.BOECCPNHAII=$impulse
  $expected=if(($impulse * $(if($reverse){-1}else{1})) -ge 0){1}else{-1}
  Check ($direction.IMLFCBLAJGA($state) -eq $expected) 'Native impulse-facing sign/default differs.'
 }
}
$fp=Fingerprint $catalog
Check ($fp -cne (Fingerprint (Load-Lua $reactionLua.Replace('hit_move=victim','hit_move=other')))) 'Owned reaction absent from fingerprint.'
Check ($fp -cne (Fingerprint (Load-Lua $reactionLua.Replace('reverse=true','reverse=false')))) 'Reverse impulse absent from fingerprint.'
Check ((Fingerprint (Load-Lua $reactionLua.Replace('reverse=true',''))) -ceq (Fingerprint (Load-Lua $reactionLua.Replace('reverse=true','reverse=false')))) 'Default impulse direction changed fingerprint.'
foreach($bad in @($reactionLua.Replace('hit_move=victim','hit_move="fixture.moves:moves/victim"'),
 $reactionLua.Replace('hit_move=victim','hit_move=animation'),$reactionLua.Replace('hit_move=victim','hit_move=victim,hit="High"'),
 $reactionLua.Replace('impulse={reverse=true}','impulse=true'),$reactionLua.Replace('reverse=true','reverse=1'),
 $reactionLua.Replace('reverse=true','reverse=true,extra=1'),$reactionLua.Replace('impulse={reverse=true}','impulse={},from={pivot="Me"}'))) {
 $failure=$null;try {$null=Load-Lua $bad}catch{$failure=$_}
 Check ($null -ne $failure) 'Malformed/ambiguous reaction or direction accepted.'
}
# A C# producer cannot bypass commit-time resolution by constructing its own ID.
foreach($name in @('fixture.moves:moves/missing','foreign.mod:moves/missing')) {
 $target=[Eclipse.Modding.DefinitionId]::Parse($name)
 $data=[Eclipse.Modding.ModMoveAttack]::new([string[]]@('Edge'),0.3,'UnarmedDamage',[System.Management.Automation.Language.NullString]::Value,0,0,0,0,$null,$null,$false,$target)
 $interval=[Eclipse.Modding.ModMoveInterval]::new('Attack',$null,$null,$null,$data)
 $empty=[Eclipse.Modding.ModContentCatalog]::new();$tx=$empty.BeginRegistration($mod)
 try {
  $null=$tx.RegisterMove('attack',[Eclipse.Modding.AssetId]::Parse('fixture.moves:animations/chinese'),$null,$null,$null,$null,
   [Eclipse.Modding.ModMoveInterval[]]@($interval),$null,1,1,1,0,$null,$null,$null,$false,$false,$null)
  $failure=$null;try {$tx.Commit()}catch{$failure=$_}
  Check ($null -ne $failure -and $empty.Moves.Count -eq 0) 'Unresolved owned hit reference committed partial content.'
 } finally {$tx.Dispose()}
}
Write-Output "PASS $script:checks combined checks: owned hit references, transaction rejection and native impulse direction. No live victim animation claim."

