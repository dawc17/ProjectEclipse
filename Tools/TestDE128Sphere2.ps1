# Exact production Lua module projection compared to the archive; native live test is separate.
. (Join-Path $PSScriptRoot 'TestMoveSpellAttacks.ps1')
(Get-Content $manifest -Raw).Replace('id = "fixture.moves"','id = "de128"') | Set-Content $manifest
$spherePackage=Join-Path $fixture 'Mods/de128'
Copy-Item -LiteralPath $package -Destination $spherePackage -Recurse
$package=$spherePackage
$manifest=Join-Path $package 'mod.toml'
$mod=[Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $fixture 'Mods')).Mods[0]
Copy-Item (Join-Path $root 'Mods/de128/scripts/content/sphere2.lua') (Join-Path $package 'scripts/content/sphere2.lua')
foreach($file in @('fireball_player','fireball_start','fireball_middle')) {
 Copy-Item (Join-Path $root ('Mods/de128/assets/animations/'+$file+'.bytes')) (Join-Path $package ('assets/animations/'+$file+'.bytes'))
 Check ((Get-FileHash (Join-Path $root ('Mods/de128/assets/animations/'+$file+'.bytes'))).Hash -ceq (Get-FileHash (Join-Path $root ('Assets/Resources/gamedata/animations/binary/'+$file+'.bytes'))).Hash) 'Sphere animation binary changed.'
}
$catalog=Load-Lua 'require("content.sphere2")'
Check ($catalog.Moves.Count -eq 9 -and $catalog.MoveTemplates.Count -eq 1) 'Incomplete Sphere2 graph.'
$doc=Project $catalog
$doc.Save((Join-Path $root 'Temp/DE128Sphere2.projected.xml'))
$mapping=@{
 sphere2_player='Sphere2Player'; sphere2_start='Sphere2Start'; sphere2_middle='Sphere2Middle'; sphere2_wall='Sphere2Wall';
 shop_magic_sphere2='ShopMagicSphere2'; shop_magic_sphere2_player='ShopMagicSphere2Player';
 shop_magic_try_on_sphere2_player='ShopMagicTryOnSphere2Player'; shop_magic_try_on_sphere2_start='ShopMagicTryOnSphere2Start';
 shop_magic_try_on_sphere2_end='ShopMagicTryOnSphere2End'
}
function Normalize-Sphere([Xml.XmlElement]$element) {
 foreach($attribute in @($element.Attributes)) {
  if($attribute.Name -eq 'PackName' -or ($attribute.Name -eq 'ID' -and $element.LocalName -eq 'Interval')) {$element.RemoveAttribute($attribute.Name);continue}
  foreach($key in $mapping.Keys) {
   if($attribute.Value -ceq ('de128:moves/'+$key)) {$attribute.Value=$mapping[$key]}
  }
  if($attribute.Name -eq 'Template') {$attribute.Value=(@($attribute.Value.Split('|') | Where-Object {$_ -ne 'Sphere2' -and $_ -ne 'de128:move-templates/sphere2'}) -join '|')}
  if($attribute.Name -eq 'FileName' -and $attribute.Value.StartsWith('de128:animations/')) {$attribute.Value=$attribute.Value.Substring('de128:animations/'.Length)+'.bytes'}
  if(($attribute.Name -in @('ShiftX','ShiftY') -and [double]$attribute.Value -eq 0) -or
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
  if($element.LocalName -eq 'Actions' -and $child.LocalName -eq 'Sound') {
   $random=$element.OwnerDocument.CreateElement('RandomSound')
   foreach($timing in @('Frame','Event')) {if($child.HasAttribute($timing)) {$random.SetAttribute($timing,$child.GetAttribute($timing));$child.RemoveAttribute($timing)}}
   $null=$element.ReplaceChild($random,$child);$null=$random.AppendChild($child);Normalize-Sphere $random
  } else {Normalize-Sphere $child}
 }
 # Direct sections are selected by name. Damage term order is retained.
 if($element.LocalName -eq 'Move' -or $element.LocalName -eq 'Interval') {
  foreach($child in @($element.ChildNodes | Where-Object {$_ -is [Xml.XmlElement]} | Sort-Object LocalName)) {$null=$element.AppendChild($child)}
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
}
foreach($key in $mapping.Keys) {
 $actual=$doc.SelectSingleNode('//Move[@Name="de128:moves/'+$key+'"]')
 $reference=$archive.SelectSingleNode('//Move[@Name="'+$mapping[$key]+'"]')
 Check ($null -ne $actual -and $null -ne $reference) ('Missing graph member: '+$key)
 $a=$actual.CloneNode($true);$b=$reference.CloneNode($true);Normalize-Sphere $a;Normalize-Sphere $b
 Check ((Shape $a) -ceq (Shape $b)) ('Complete Sphere archive mismatch: '+$key+"`nACTUAL "+(Shape $a)+"`nEXPECTED "+(Shape $b))
 foreach($template in $actual.GetAttribute('Template').Split('|')) {
  if($template -eq 'de128:move-templates/sphere2') {continue}
  $base=$baseMoves.SelectSingleNode('//Templates/Template[@Name="'+$template+'"]')
  $old=$archive.SelectSingleNode('//Templates/Template[@Name="'+$template+'"]')
  Check ($null -ne $base -and (Shape $base) -ceq (Shape $old)) ('Inherited Sphere template differs: '+$template)
 }
 foreach($attack in $actual.SelectNodes('./Intervals/Interval[@Type="Attack"]')) {
  $parsed=Parse-Attack $attack
  Check ($parsed.HPLOFLKCLHG() -and $parsed.NPHDDMAIGKN() -and !$parsed.PIKCMLIAFOI()) 'Sphere native attack options lost.'
 }
}
Check ($archive.SelectSingleNode('//Templates/Template[@Name="Sphere2"]').ChildNodes.Count -eq 0) 'Removed archive family template is no longer empty.'
Check ((Fingerprint $catalog) -ceq (Fingerprint (Load-Lua 'require("content.sphere2")'))) 'Sphere graph fingerprint changed on reload.'
Write-Output "PASS $script:checks combined checks: complete nine-move Sphere2 Lua graph, template equivalence, binary identity and native attacks. No live casting/contact claim."
