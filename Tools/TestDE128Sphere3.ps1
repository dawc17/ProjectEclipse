# Exact production Lua module projection compared to the archive; native live test is separate.
. (Join-Path $PSScriptRoot 'TestMoveSpellAttacks.ps1')
(Get-Content $manifest -Raw).Replace('id = "fixture.moves"','id = "de128"') | Set-Content $manifest
$spherePackage=Join-Path $fixture 'Mods/de128'
Copy-Item -LiteralPath $package -Destination $spherePackage -Recurse
$package=$spherePackage
$manifest=Join-Path $package 'mod.toml'
$mod=[Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $fixture 'Mods')).Mods[0]
Copy-Item (Join-Path $root 'Mods/de128/scripts/content/sphere3.lua') (Join-Path $package 'scripts/content/sphere3.lua')
foreach($file in @('big_sphere_player','magic_fire_aura_bullet')) {
 Copy-Item (Join-Path $root ('Mods/de128/assets/animations/'+$file+'.bytes')) (Join-Path $package ('assets/animations/'+$file+'.bytes'))
 Check ((Get-FileHash (Join-Path $root ('Mods/de128/assets/animations/'+$file+'.bytes'))).Hash -ceq (Get-FileHash (Join-Path $root ('Assets/Resources/gamedata/animations/binary/'+$file+'.bytes'))).Hash) 'Sphere animation binary changed.'
}
$catalog=Load-Lua 'require("content.sphere3")'
Check ($catalog.Moves.Count -eq 5 -and $catalog.MoveTemplates.Count -eq 1) 'Incomplete Sphere3 graph.'
$doc=Project $catalog
$doc.Save((Join-Path $root 'Temp/DE128Sphere3.projected.xml'))
$mapping=@{
 sphere3_player='Sphere3Player'; sphere3_start='Sphere3Start'; sphere3_middle='Sphere3Middle';
 shop_magic_sphere3_player='ShopMagicSphere3Player'; shop_magic_try_on_sphere3_player='ShopMagicTryOnSphere3Player'
}
function Normalize-Sphere([Xml.XmlElement]$element) {
 foreach($attribute in @($element.Attributes)) {
  if($attribute.Name -eq 'PackName' -or ($attribute.Name -eq 'ID' -and $element.LocalName -eq 'Interval')) {$element.RemoveAttribute($attribute.Name);continue}
  foreach($key in $mapping.Keys) {
   if($attribute.Value -ceq ('de128:moves/'+$key)) {$attribute.Value=$mapping[$key]}
  }
  if($attribute.Name -eq 'Template') {$attribute.Value=(@($attribute.Value.Split('|') | Where-Object {$_ -ne 'Sphere3' -and $_ -ne 'de128:move-templates/sphere3'}) -join '|')}
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
  if($element.LocalName -eq 'Actions' -and $child.LocalName -eq 'Sound') {
   $random=$element.OwnerDocument.CreateElement('RandomSound')
   foreach($timing in @('Frame','Event')) {if($child.HasAttribute($timing)) {$random.SetAttribute($timing,$child.GetAttribute($timing));$child.RemoveAttribute($timing)}}
   $null=$element.ReplaceChild($random,$child);$null=$random.AppendChild($child);Normalize-Sphere $random
  } else {Normalize-Sphere $child}
 }
 # Direct sections are selected by name. Action, condition and term order is retained.
 if($element.LocalName -eq 'Move' -or $element.LocalName -eq 'Interval') {
  foreach($child in @($element.ChildNodes | Where-Object {$_ -is [Xml.XmlElement]} | Sort-Object LocalName)) {$null=$element.AppendChild($child)}
 }
}
foreach($key in $mapping.Keys) {
 $actual=$doc.SelectSingleNode('//Move[@Name="de128:moves/'+$key+'"]')
 $reference=$archive.SelectSingleNode('//Move[@Name="'+$mapping[$key]+'"]')
 Check ($null -ne $actual -and $null -ne $reference) ('Missing graph member: '+$key)
 $a=$actual.CloneNode($true);$b=$reference.CloneNode($true);Normalize-Sphere $a;Normalize-Sphere $b
 Check ((Shape $a) -ceq (Shape $b)) ('Complete Sphere archive mismatch: '+$key+"`nACTUAL "+(Shape $a)+"`nEXPECTED "+(Shape $b))
 foreach($template in $actual.GetAttribute('Template').Split('|')) {
  if($template -eq 'de128:move-templates/sphere3') {continue}
  $base=$baseMoves.SelectSingleNode('//Templates/Template[@Name="'+$template+'"]')
  $old=$archive.SelectSingleNode('//Templates/Template[@Name="'+$template+'"]')
  Check ($null -ne $base -and (Shape $base) -ceq (Shape $old)) ('Inherited Sphere template differs: '+$template)
 }
 foreach($attack in $actual.SelectNodes('./Intervals/Interval[@Type="Attack"]')) {
  $parsed=Parse-Attack $attack
  Check ($parsed.HPLOFLKCLHG() -and $parsed.NPHDDMAIGKN() -and !$parsed.PIKCMLIAFOI()) 'Sphere native attack options lost.'
 }
}
Check ($archive.SelectSingleNode('//Templates/Template[@Name="Sphere3"]').ChildNodes.Count -eq 0) 'Removed archive family template is no longer empty.'
Check ((Fingerprint $catalog) -ceq (Fingerprint (Load-Lua 'require("content.sphere3")'))) 'Sphere graph fingerprint changed on reload.'
Write-Output "PASS $script:checks combined checks: complete five-move Sphere3 Lua graph, template equivalence, binary identity and native attacks. No live casting/contact claim."

$attack=$doc.SelectSingleNode('//Move[@Name="de128:moves/sphere3_middle"]/Intervals/Interval[@Type="Attack"]')
$native=Parse-Attack $attack
Check ($native.GetReactionName(22) -ceq 'Physycal') 'Native physical-fall reaction was rewritten.'
Check ($native.IKPJJAEIOCG().Count -eq 5) 'Repeated archived attack edges were lost.'
$reactionLua='sf2.moves.register_template {id="reaction",intervals={{type="Attack",attack={edges={"Edge"},hit="Physycal"}}}}'
$reaction=Load-Lua $reactionLua
Check ((Fingerprint $reaction) -cne (Fingerprint (Load-Lua $reactionLua.Replace('Physycal','High')))) 'Reaction missing from fingerprint.'
foreach($bad in @('Physical','physycal','Unknown')) {
 $failure=$null;try {$null=Load-Lua $reactionLua.Replace('Physycal',$bad)}catch{$failure=$_}
 Check ($null -ne $failure) ('Unsupported reaction accepted: '+$bad)
}
Write-Output "PASS $script:checks combined checks including exact Physycal reaction and repeated edges."
