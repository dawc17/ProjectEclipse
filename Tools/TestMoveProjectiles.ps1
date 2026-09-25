# Actual Lua and native projectile action parsing; no live actor/contact claim.
. (Join-Path $PSScriptRoot 'TestMoveEffects.ps1')
$projectileLua=@'
local animation=sf2.assets.binary("animations/chinese")
local child=sf2.moves.register {id="child",animation=animation}
local projectile={name="Sphere1",core_skeleton="SkeletonMagic",copy_parent_type="Magic"}
local actions={
 {type="create_projectile",frame=2,projectile=projectile},
 {type="add_bullets",frame=7,bullets={type="MagicBullet",value=-1}},
 {type="delete_actor",event="Strike",player="Me"},
}
sf2.moves.register {id="cast",animation=animation,actions=actions}
'@
$catalog=Load-Lua $projectileLua
$doc=Project $catalog
$nodes=$doc.SelectNodes('//Move[contains(@Name,"cast")]/Actions/*')
$sourceNodes=@($archive.SelectSingleNode('//Move[@Name="Sphere1Player"]/Actions/CreatePlayer'),
 $archive.SelectSingleNode('//Move[@Name="Sphere1Player"]/Actions/AddBullets'),
 $archive.SelectSingleNode('//Move[@Name="Sphere1Start"]/Actions/Delete'))
Check ($nodes.Count -eq 3) 'Projectile actions lost.'
for($i=0;$i -lt 3;$i++) {
 Check ((Shape $nodes[$i]) -ceq (Shape $sourceNodes[$i])) 'Archive projectile action differs.'
 if($i -eq 0) {continue} # ItemInfo needs Unity native calls; covered by isolated native harness.
 $native=[ActionsParser]::Create($nodes[$i]);$source=[ActionsParser]::Create($sourceNodes[$i])
 Check ($native.GetType() -eq $source.GetType()) 'Native projectile action type differs.'
 foreach($field in $native.GetType().GetFields($flags)) {
  Check (($field.GetValue($native) | ConvertTo-Json -Depth 10 -Compress) -ceq ($field.GetValue($source) | ConvertTo-Json -Depth 10 -Compress)) ('Native projectile field differs: '+$field.Name)
 }
 if($i -eq 2) {
  Check ($native.NeedStart([EventAnimation+EECEJKADLCK]::EVENT_STRIKE) -and !$native.NeedStart(0)) 'Delete scheduling lost.'
  Check ($native.OJLDHGKPLNC() -eq $source.OJLDHGKPLNC()) 'Delete target differs.'
 } else {Check ($native.NeedStart([int]$nodes[$i].GetAttribute('Frame'))) 'Projectile frame scheduling lost.'}
}
foreach($choice in @('projectile.core_start_animation="ShopMagicSphere1"','projectile.start_move=child')) {
 $changed=Project (Load-Lua $projectileLua.Replace('sf2.moves.register {id="cast"',($choice+"`n"+'sf2.moves.register {id="cast"')))
 $native=$changed.SelectSingleNode('//Move[contains(@Name,"cast")]/Actions/CreatePlayer')
 $expectedName=if($choice.Contains('core_start')){'ShopMagicSphere1'}else{'fixture.moves:moves/child'}
 Check ($native.GetAttribute('StartAnimation') -ceq $expectedName) 'Explicit child starting move lost.'
}
$baseline=Fingerprint $catalog
$namedItemLua=$projectileLua.Replace('copy_parent_type="Magic"', 'item=sf2.items.get("core:items/magic/MAGIC_BUTCHER_EARTHQUAKE")')
$manifest=Join-Path $package 'mod.toml'
$oldManifest=Get-Content -LiteralPath $manifest -Raw
Add-Content -LiteralPath $manifest -Value "`n[[dependencies]]`nid = `"core`"`nversion = `">=1.0 <2.0`""
$mod=[Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $fixture 'Mods')).Mods[0]
$seedMagic={ param($target)
 $items=[Xml.XmlDocument]::new();$items.Load((Join-Path $root 'Assets/vanillaXml/list.xml'))
 [void][Eclipse.Modding.CoreContentImporter]::ImportMagic($target,
  [Xml.XmlNode[]]@($items.SelectSingleNode('/List/Items/Item[@Name="MAGIC_BUTCHER_EARTHQUAKE"]')),$null)
 [void][Eclipse.Modding.CoreContentImporter]::ImportArmors($target,
  [Xml.XmlNode[]]@($items.SelectSingleNode('/List/Items/Item[@Name="ARMOR_CEREMONIAL"]')),$null)
}
$namedItem=Load-Lua $namedItemLua $seedMagic
$namedNode=(Project $namedItem).SelectSingleNode('//Move[contains(@Name,"cast")]/Actions/CreatePlayer/Item[@Type="Weapon"]')
Check ($namedNode.GetAttribute('Name') -ceq 'MAGIC_BUTCHER_EARTHQUAKE' -and !$namedNode.HasAttribute('CopyParentType')) 'Named-item projectile did not emit the hidden native weapon source.'
Check ($baseline -cne (Fingerprint $namedItem)) 'Named-item projectile did not affect the content fingerprint.'
foreach($mutation in @('projectile.copy_parent_type="Magic"','projectile.item=nil','projectile.item=child','projectile.item="MAGIC_BUTCHER_EARTHQUAKE"',
 'projectile.item=sf2.items.get("core:items/armor/ARMOR_CEREMONIAL")')) {
 $failure=$null
 try {$null=Load-Lua $namedItemLua.Replace('sf2.moves.register {id="cast"',($mutation+"`n"+'sf2.moves.register {id="cast"')) $seedMagic}catch{$failure=$_}
 Check ($null -ne $failure) ('Invalid named-item projectile accepted: '+$mutation)
}
Set-Content -LiteralPath $manifest -Value $oldManifest
$mod=[Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $fixture 'Mods')).Mods[0]
foreach($mutation in @('projectile.name="Other"','projectile.core_skeleton="OtherSkeleton"','projectile.copy_parent_type="Ranged"',
 'projectile.core_start_animation="ShopMagicSphere1"','projectile.start_move=child','actions[2].bullets.type="RaidChargeBullet"',
 'actions[2].bullets.value=-2','actions[3].player="Child"')) {
 $changed=$projectileLua.Replace('sf2.moves.register {id="cast"',($mutation+"`n"+'sf2.moves.register {id="cast"'))
 Check ($baseline -cne (Fingerprint (Load-Lua $changed))) ('Projectile fingerprint omitted: '+$mutation)
}
foreach($mutation in @('projectile.name=""','projectile.name="bad/path"','projectile.core_skeleton="bad/path"',
 'projectile.copy_parent_type="Armor"','projectile.copy_parent_type=nil','projectile.start_move="fixture.moves:moves/child"',
 'projectile.start_move=animation','projectile.start_move=child;projectile.core_start_animation="ShopMagicSphere1"',
 'projectile.core_start_animation="../bad"','projectile.extra=true','actions[1].projectile=nil','actions[1].projectile="bad"',
 'actions[1].effect_name="bad"','actions[2].bullets={}','actions[2].bullets.type="RangedBullet"','actions[2].bullets.value=0',
 'actions[2].bullets.value=0.5','actions[2].bullets.value=1/0','actions[2].bullets.value=-100001','actions[2].bullets.value=100001',
 'actions[2].bullets.extra=true','actions[2].player="Enemy"','actions[3].player=nil','actions[3].player="Everybody"',
 'actions[3].projectile=projectile','actions[1].frame=-1','actions[1].frame=0.5','actions[1].event="Strike"')) {
 $failure=$null
 try {$null=Load-Lua $projectileLua.Replace('sf2.moves.register {id="cast"',($mutation+"`n"+'sf2.moves.register {id="cast"'))}catch{$failure=$_}
 Check ($null -ne $failure) ('Invalid projectile accepted: '+$mutation)
}
Write-Output "PASS $script:checks combined move checks including projectile inheritance projection, native parsers, start-move handles, scheduling, validation and fingerprints. No live projectile or contact claim."
