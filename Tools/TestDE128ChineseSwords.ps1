. (Join-Path $PSScriptRoot 'TestMovePresentationAuthoring.ps1')
Add-Content $manifest "`n[[dependencies]]`nid = `"core`"`nversion = `">=1.0 <2.0`""
$mod=[Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $fixture 'Mods')).Mods[0]
$null=New-Item -ItemType Directory -Force (Join-Path $package 'scripts/content')
Copy-Item (Join-Path $root 'Mods/de128/scripts/content/chinese_swords.lua') (Join-Path $package 'scripts/content/chinese_swords.lua')
Copy-Item (Join-Path $root 'Mods/de128/assets/animations/chinese_swords_super_slash_old.bytes') (Join-Path $package 'assets/animations/chinese_swords_super_slash_old.bytes')
$hash=(Get-FileHash (Join-Path $package 'assets/animations/chinese_swords_super_slash_old.bytes')).Hash
Check ($hash -ceq '6810E8BE5CE50A88DAD84EEB7660D9AB92552B69DBB209B94C3998278109E5FA') 'Packaged animation differs from archived binary.'
$seed={param($target)
    [xml]$items=Get-Content -Raw (Join-Path $root 'Assets/vanillaXml/list.xml')
    $null=[Eclipse.Modding.CoreContentImporter]::ImportWeapons($target,[Xml.XmlNode[]]@($items.SelectNodes('/List/Items/Item[@Name="WEAPON_CHNY21_JIAN"]')),[Collections.Generic.Dictionary[string,Xml.XmlDocument]]::new())
}
$catalog=Load-Lua 'require("content.chinese_swords")' $seed
Check ($catalog.Moves.Count -eq 2 -and $catalog.MoveItemLockExtensions.Count -eq 10 -and $catalog.ItemCombatSubtypes.Count -eq 1) 'Incomplete module registration.'
$doc=Project $catalog
foreach($pair in @(@('chinese_swords_super_slash','ChineseSwordsSuperSlash'),@('shop_chinese_swords_super_slash','ShopChineseSwordsSuperSlash'))) {
    $actual=$doc.SelectSingleNode('//Move[@Name="fixture.moves:moves/'+$pair[0]+'"]')
    $reference=$archive.SelectSingleNode('//Move[@Name="'+$pair[1]+'"]')
    Check ($null -ne $actual) 'Actual production move missing.'
    $normalized=$actual.CloneNode($true)
    $normalized.SetAttribute('Name',$reference.GetAttribute('Name'));$normalized.SetAttribute('FileName',$reference.GetAttribute('FileName'))
    if($normalized.Profile) {$normalized.Profile.RemoveAttribute('DisplayName')}
    # Top-level section ordering is not significant to the native named-node parser.
    Check ($normalized.ChildNodes.Count -eq $reference.ChildNodes.Count) 'Move has missing/extra sections.'
    foreach($section in $reference.ChildNodes) { $null=$normalized.AppendChild($normalized[$section.Name]) }
    Check ((Shape $normalized) -ceq (Shape $reference)) ('Complete archive move differs: '+$pair[1])
    foreach($templateName in $actual.GetAttribute('Template').Split('|')) {
        $canonical=$baseMoves.SelectSingleNode('/Movesxml/Templates/Template[@Name="'+$templateName+'"]')
        $archived=$archive.SelectSingleNode('/Movesxml/Templates/Template[@Name="'+$templateName+'"]')
        Check ($null -ne $canonical -and (Shape $canonical) -ceq (Shape $archived)) ('Inherited template differs: '+$templateName)
    }
}
$doc.Save((Join-Path $root 'Temp/DE128ChineseSwords.projected.xml'))
$fingerprint=Fingerprint $catalog
Check ($fingerprint -ceq (Fingerprint (Load-Lua 'require("content.chinese_swords")' $seed))) 'Actual module fingerprint changed on reload.'
$bad='local a=sf2.assets.binary("animations/chinese");sf2.moves.register {id="bad",animation=a,profile={rank=4,core_icon="Trick7.super_slash",display_name="not a handle"}}'
$failure=$null;try {$null=Load-Lua $bad}catch{$failure=$_};Check ($null -ne $failure) 'Raw profile title accepted as a localization handle.'
$titleLua=@'
local first=sf2.localization.register {id="first",language="eng",value="First"}
local second=sf2.localization.register {id="second",language="eng",value="Second"}
sf2.moves.register {id="title",animation=sf2.assets.binary("animations/chinese"),profile={rank=4,core_icon="Trick7.super_slash",display_name=first}}
'@
Check ((Fingerprint (Load-Lua $titleLua)) -cne (Fingerprint (Load-Lua $titleLua.Replace('display_name=first','display_name=second')))) 'Profile title handle omitted from fingerprint.'
$legacyMove=[InfoAnimation]::new();$legacyMove.Name='OriginalMove';[xml]$legacyProfile='<Profile Rank="4" Icon="Trick7.super_slash"/>'
Check ([Trick]::new($legacyProfile.DocumentElement,$legacyMove).DisplayName -ceq 'OriginalMove') 'Legacy profile name fallback changed.'
Write-Output "PASS $script:checks combined ChineseSwords checks: actual registration module, complete archived moves, inherited templates, profile localization; full native parser probe exported. No live fight."
