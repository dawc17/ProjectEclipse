# Exact production Lua module projection compared to the archive; native live test is separate.
. (Join-Path $PSScriptRoot 'TestMoveSpellAttacks.ps1')
(Get-Content $manifest -Raw).Replace('id = "fixture.moves"','id = "de128"') | Set-Content $manifest
$spherePackage=Join-Path $fixture 'Mods/de128'
Copy-Item -LiteralPath $package -Destination $spherePackage -Recurse
$package=$spherePackage
$manifest=Join-Path $package 'mod.toml'
$mod=[Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $fixture 'Mods')).Mods[0]
Copy-Item (Join-Path $root 'Mods/de128/scripts/content/mind_throw.lua') (Join-Path $package 'scripts/content/mind_throw.lua')
foreach($file in @('mind_throw_1_normal','mind_throw_2_normal','mind_suffocation','mind_suffocation_start','mind_suffocation_middle')) {
 Copy-Item (Join-Path $root ('Mods/de128/assets/animations/'+$file+'.bytes')) (Join-Path $package ('assets/animations/'+$file+'.bytes'))
 Check ((Get-FileHash (Join-Path $root ('Mods/de128/assets/animations/'+$file+'.bytes'))).Hash -ceq (Get-FileHash (Join-Path $root ('Assets/Resources/gamedata/animations/binary/'+$file+'.bytes'))).Hash) 'MindThrow animation binary changed.'
}
$catalog=Load-Lua 'require("content.mind_throw")'
Check ($catalog.Moves.Count -eq 6 -and $catalog.MoveTemplates.Count -eq 0) 'Incomplete MindThrow graph.'
$doc=Project $catalog
$doc.Save((Join-Path $root 'Temp/DE128MindThrow.projected.xml'))
$mapping=@{
 mind_throw_player='MindThrowPlayerNormal'; mind_throw_player2='MindThrowPlayer2Normal';
 mind_throw_start='MindThrowStartNormal'; mind_throw_middle='MindThrowMiddleNormal';
 mind_throw_wall='MindThrowWallNormal'; mind_throw_hit='MindThrowHitNormal'
}
function Normalize-MindThrow([Xml.XmlElement]$element) {
 foreach($attribute in @($element.Attributes)) {
  if($attribute.Name -eq 'PackName' -or ($attribute.Name -eq 'ID' -and $element.LocalName -eq 'Interval')) {$element.RemoveAttribute($attribute.Name);continue}
  foreach($key in $mapping.Keys) {
   if($attribute.Value -ceq ('de128:moves/'+$key)) {$attribute.Value=$mapping[$key]}
  }
  if($attribute.Value -ceq 'de128:behaviors/mind_throw:pending') {$attribute.Value='MindThrowFlag'}
  if($attribute.Name -eq 'FileName' -and $attribute.Value.StartsWith('de128:animations/')) {$attribute.Value=$attribute.Value.Substring('de128:animations/'.Length)+'.bytes'}
  if(($attribute.Name -in @('ShiftX','ShiftY') -and [double]$attribute.Value -eq 0) -or
     ($element.LocalName -eq 'Effect' -and $attribute.Name -eq 'Looped' -and [double]$attribute.Value -eq 0) -or
     ($element.LocalName -eq 'Effect' -and $attribute.Name -eq 'TimeScale' -and [double]$attribute.Value -eq 1) -or
     ($element.LocalName -eq 'Distance' -and (($attribute.Name -eq 'Min' -and [double]$attribute.Value -eq -1000000) -or ($attribute.Name -eq 'Max' -and [double]$attribute.Value -eq 1000000)))) {
   $element.RemoveAttribute($attribute.Name);continue
  }
  $number=0.0
  if([double]::TryParse($attribute.Value,[Globalization.NumberStyles]::Float,[Globalization.CultureInfo]::InvariantCulture,[ref]$number)) {
   # Native fields are parsed as floats; compare their actual numeric precision.
   $attribute.Value=([single]$number).ToString('R',[Globalization.CultureInfo]::InvariantCulture)
  }
 }
 foreach($child in @($element.ChildNodes)) {
  if($child -isnot [Xml.XmlElement]) {continue}
  if($child.LocalName -eq 'Impulse' -and $child.HasAttribute('X') -and [double]$child.GetAttribute('X') -eq 0 -and [double]$child.GetAttribute('Y') -eq 0 -and [double]$child.GetAttribute('Z') -eq 0) {$null=$element.RemoveChild($child);continue}
  if($element.LocalName -eq 'Actions' -and $child.LocalName -eq 'Sound') {
   $random=$element.OwnerDocument.CreateElement('RandomSound')
   foreach($timing in @('Frame','Event')) {if($child.HasAttribute($timing)) {$random.SetAttribute($timing,$child.GetAttribute($timing));$child.RemoveAttribute($timing)}}
   $null=$element.ReplaceChild($random,$child);$null=$random.AppendChild($child);Normalize-MindThrow $random
  } else {Normalize-MindThrow $child}
 }
 # Native ConditionList evaluates these pure conditions as a short-circuit AND/OR,
 # so their order does not change selection. Key sequences keep their order.
 if($element.LocalName -in @('Conditions','Locks','Operator')) {
  foreach($child in @($element.ChildNodes | Where-Object {$_ -is [Xml.XmlElement]} | Sort-Object OuterXml)) {$null=$element.AppendChild($child)}
 }
 # Native ModelAnimation dispatches frame and event actions separately, each call
 # collecting only its own trigger in list order. Group by trigger, keeping order.
 if($element.LocalName -eq 'Actions') {
  $index=0
  foreach($entry in @($element.ChildNodes | Where-Object {$_ -is [Xml.XmlElement]} | ForEach-Object {
    [pscustomobject]@{Node=$_;Index=$index++;Key=$(if($_.HasAttribute('Frame')){'F'+([int]$_.GetAttribute('Frame')).ToString('D6')}else{'E'+$_.GetAttribute('Event')})}
   } | Sort-Object Key,Index)) {$null=$element.AppendChild($entry.Node)}
 }
 # Direct sections are selected by name. Damage term order is retained.
 if($element.LocalName -eq 'Move' -or $element.LocalName -eq 'Interval') {
  foreach($child in @($element.ChildNodes | Where-Object {$_ -is [Xml.XmlElement]} | Sort-Object LocalName)) {$null=$element.AppendChild($child)}
 }
}
$seenTemplates=[Collections.Generic.HashSet[string]]::new()
function Verify-Template([string]$name) {
 if(!$name -or !$seenTemplates.Add($name)) {return}
 $base=$baseMoves.SelectSingleNode('//Templates/Template[@Name="'+$name+'"]')
 $old=$archive.SelectSingleNode('//Templates/Template[@Name="'+$name+'"]')
 Check ($null -ne $base -and $null -ne $old -and (Shape $base) -ceq (Shape $old)) ('Inherited MindThrow template differs: '+$name)
 foreach($parent in $base.GetAttribute('Template').Split('|')) {Verify-Template $parent}
}
foreach($key in $mapping.Keys) {
 $actual=$doc.SelectSingleNode('//Move[@Name="de128:moves/'+$key+'"]')
 $reference=$archive.SelectSingleNode('//Move[@Name="'+$mapping[$key]+'"]')
 Check ($null -ne $actual -and $null -ne $reference) ('Missing graph member: '+$key)
 $a=$actual.CloneNode($true);$b=$reference.CloneNode($true);Normalize-MindThrow $a;Normalize-MindThrow $b
 Check ((Shape $a) -ceq (Shape $b)) ('Complete MindThrow archive mismatch: '+$key+"`nACTUAL "+(Shape $a)+"`nEXPECTED "+(Shape $b))
 foreach($template in $actual.GetAttribute('Template').Split('|')) { Verify-Template $template }
}
Check ((Fingerprint $catalog) -ceq (Fingerprint (Load-Lua 'require("content.mind_throw")'))) 'MindThrow graph fingerprint changed on reload.'
foreach($phase in @('start','middle')) {
 $attack=$doc.SelectSingleNode('//Move[@Name="de128:moves/mind_throw_'+$phase+'"]/Intervals/Interval[@Type="Attack"]')
 $parsed=Parse-Attack $attack
 Check ($parsed.GetReactionName(10) -ceq 'de128:moves/mind_throw_hit') 'Owned victim hit lost in native parser.'
 Check ($parsed.HPLOFLKCLHG() -and $parsed.NPHDDMAIGKN() -and !$parsed.PIKCMLIAFOI()) 'Native projectile options lost.'
}
$direct=$doc.SelectSingleNode('//Move[@Name="de128:moves/mind_throw_player2"]/Intervals/Interval[@Type="Attack"]')
Check ($null -eq $direct.AttackingParts -and (Parse-Attack $direct).GetReactionName(48) -ceq 'NoReaction') 'Direct final hit changed.'
Write-Output "PASS $script:checks combined checks: complete six-move MindThrow graph, template closure, binary identity and native reactions. No live full-spell claim."
