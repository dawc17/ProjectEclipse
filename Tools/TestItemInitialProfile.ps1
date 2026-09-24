# Native item profile override and adapter rollback; no Unity shop or player save.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
dotnet build (Join-Path $root 'Assembly-CSharp.csproj') -v:q /clp:ErrorsOnly
if ($LASTEXITCODE -ne 0) { throw 'Managed build failed.' }
. (Join-Path $PSScriptRoot 'LoadUnityManagedAssemblies.ps1')
$null = Import-SF2ManagedRuntime $root
$flags = [Reflection.BindingFlags]'Instance,NonPublic'
$script:checks = 0
function Check([bool]$ok, [string]$message) { $script:checks++; if (!$ok) { throw $message } }
function Item([string]$type) {
    [xml]$xml = '<Item Name="fixture" Type="'+$type+'" Level="1" UpgradeLevel="600" WeaponDamage="5" BonusPrice="30" ShopHide="1"/>'
    $item = [ItemInfo]::new($null)
    $item.Name = 'fixture'; $item.Type = $type; $item.NodeXML = $xml.DocumentElement
    $item.HasAuthoredLevel = $true; $item.ItemLevel = 1; $item.UpgradeLevel = 600
    $item.UpgradeTemplateName = 'Paid_Weapon_Bonus'
    $item.LegacyPaidItem = 'Paid'
    $item.CoinPrice = [CodeStage.AntiCheat.ObscuredTypes.ObscuredLong]::op_Implicit([long]7)
    $item.GemPrice = [CodeStage.AntiCheat.ObscuredTypes.ObscuredLong]::op_Implicit([long]30)
    $item.FileName = 'core:ui/items/old_icon'
    $item.ModelFileName = 'core:gamedata/models/old_model'
    $item.ItemAttributes.Set('WeaponDamage', 5)
    return $item
}
function RawStat($item, [string]$name) {
    $value = 0
    $found = $item.ItemAttributes.Get($name, [ref]$value, $false, $false)
    return @{ Found = $found; Value = $value }
}
$method = [ItemInfo].GetMethod('TryOverrideInitialProfile', $flags)
function Apply($item, [int]$level, [int]$upgrade, $stats, $template = $null, $paid = $null, [bool]$clear = $false) {
    $arguments = [object[]]@($level, $upgrade, $stats, $template, $paid, $clear, $null)
    return @{ Success = $method.Invoke($item, $arguments); Lifetime = $arguments[6] }
}
$stats = [Collections.Generic.Dictionary[string,int]]::new()
$stats.Add('WeaponDamage', 342)
$item = Item 'Weapon'; $originalXml = $item.NodeXML.OuterXml
$scope = Apply $item 15 1500 $stats 'Weapon_Bonus' 'None'
Check ($scope.Success -and $item.ItemLevel -eq 15 -and $item.UpgradeLevel -eq 1500) 'Native level/upgrade override failed.'
Check ((RawStat $item 'WeaponDamage').Value -eq 342 -and $item.HasAuthoredLevel) 'Native initial stat override failed.'
Check ($item.UpgradeTemplateName -eq 'Weapon_Bonus') 'Native upgrade template override failed.'
Check ($item.LegacyPaidItem -eq 'None') 'Native legacy paid marker override failed.'
$snapshot = $item.Clone()
Check ($snapshot.ItemLevel -eq 15 -and (RawStat $snapshot 'WeaponDamage').Value -eq 342 -and
    $snapshot.UpgradeTemplateName -eq 'Weapon_Bonus' -and $snapshot.LegacyPaidItem -eq 'None') 'New item copy missed the profile.'
Check (!(Apply $item 20 2000 $stats).Success) 'Concurrent native profile override accepted.'
$scope.Lifetime.Dispose()
Check ($item.ItemLevel -eq 1 -and $item.UpgradeLevel -eq 600 -and
    (RawStat $item 'WeaponDamage').Value -eq 5 -and $item.UpgradeTemplateName -eq 'Paid_Weapon_Bonus' -and
    $item.LegacyPaidItem -eq 'Paid') 'Native rollback did not restore profile.'
Check ($item.NodeXML.OuterXml -eq $originalXml -and $snapshot.ItemLevel -eq 15) 'Rollback mutated XML or a completed snapshot.'
$fresh = Apply $item 20 2000 $stats; $scope.Lifetime.Dispose()
Check ($fresh.Success -and $item.ItemLevel -eq 20 -and
    $item.UpgradeTemplateName -eq 'Paid_Weapon_Bonus' -and
    $item.LegacyPaidItem -eq 'Paid') 'Stale scope erased newer override or optional metadata changed.'
$fresh.Lifetime.Dispose()
foreach ($invalid in @(@(0, 1500), @(53, 1500), @(15, -1), @(15, 5201))) {
    Check (!(Apply $item $invalid[0] $invalid[1] $stats).Success) 'Invalid native profile accepted.'
}
Check (!(Apply (Item 'Consumable') 15 1500 $stats).Success) 'Non-equipment profile accepted.'
Check (!(Apply $item 15 1500 $stats 'Armor_Bonus').Success) 'Wrong-category native upgrade template accepted.'
Check (!(Apply $item 15 1500 $stats 'Weapon_Bonus' 'Invalid').Success) 'Invalid native paid marker accepted.'
foreach ($paid in @('Paid', 'SuperPaid')) {
    $markerScope = Apply $item 15 1500 $stats 'Weapon_Bonus' $paid
    Check ($markerScope.Success -and $item.LegacyPaidItem -eq $paid) 'Supported native paid marker was rejected.'
    $markerScope.Lifetime.Dispose()
    Check ($item.LegacyPaidItem -eq 'Paid') 'Native paid marker rollback failed.'
}
$localRows = $item.LocalUpgrades
$localRows.Add([System.Runtime.CompilerServices.RuntimeHelpers]::GetUninitializedObject([UpgradeData]))
$clearScope = Apply $item 15 1500 $stats 'Weapon_Bonus' $null $true
Check ($clearScope.Success -and $item.LocalUpgrades.Count -eq 0 -and
    $item.Clone().LocalUpgrades.Count -eq 0) 'Local upgrade rows survived a clearing profile.'
$clearScope.Lifetime.Dispose()
Check ([object]::ReferenceEquals($item.LocalUpgrades, $localRows) -and
    $item.LocalUpgrades.Count -eq 1) 'Local upgrade rows were not restored on unload.'
$priceMethod = [ItemInfo].GetMethod('TryOverrideShopPrice', $flags)
function Price($item, [long]$coins, [long]$gems) {
    $arguments = [object[]]@($coins, $gems, $null)
    return @{ Success = $priceMethod.Invoke($item, $arguments); Lifetime = $arguments[2] }
}
foreach ($invalid in @(@(0,0), @(-1,0), @(0,2147483648))) {
    Check (!(Price $item $invalid[0] $invalid[1]).Success) 'Invalid native shop price accepted.'
}
$priceScope = Price $item 0 39
Check ($priceScope.Success -and !$item.INCBGIDFIDN() -and $item.PLBFFNCCCGO() -and
    $item.OHBBLIMNIMJ() -eq 0 -and $item.MCNMMBCJADI() -eq 39) 'Native gem price/currency override failed.'
Check ($item.Clone().MCNMMBCJADI() -eq 39 -and !(Price $item 5 0).Success) 'Shop price copy or duplicate guard failed.'
$priceScope.Lifetime.Dispose()
Check ($item.OHBBLIMNIMJ() -eq 7 -and $item.MCNMMBCJADI() -eq 30) 'Native shop price rollback failed.'
$dualScope = Price $item 2550000 97
Check ($dualScope.Success -and $item.OHBBLIMNIMJ() -eq 2550000 -and
    $item.MCNMMBCJADI() -eq 97 -and $item.Clone().OHBBLIMNIMJ() -eq 2550000) 'Dual native price or copy failed.'
$dualScope.Lifetime.Dispose()
Check ($item.OHBBLIMNIMJ() -eq 7 -and $item.MCNMMBCJADI() -eq 30) 'Dual native price rollback failed.'
$presentationMethod = [ItemInfo].GetMethod('TryOverridePresentation', $flags)
function Presentation($item, [string]$icon, [string]$model) {
    $arguments = [object[]]@($icon, $model, $null)
    return @{ Success = $presentationMethod.Invoke($item, $arguments); Lifetime = $arguments[2] }
}
Check (!(Presentation $item $null $null).Success -and
    !(Presentation (Item 'Consumable') 'core:ui/items/new_icon' $null).Success) 'Invalid native item presentation accepted.'
$artScope = Presentation $item 'core:ui/items/new_icon' 'core:gamedata/models/new_model'
Check ($artScope.Success -and $item.FileName -eq 'core:ui/items/new_icon' -and
    $item.ModelFileName -eq 'core:gamedata/models/new_model' -and
    $item.Clone().ModelFileName -eq 'core:gamedata/models/new_model') 'Native item presentation or copy failed.'
$fighterModel = [ModelParameters]::new(); $fighterModel.Weapon = $item
$fighterModel.PPFDLIBLNDG()
Check ($fighterModel.ModelDocuments.Contains('core:gamedata/models/new_model.xml')) 'Fighter model preparation missed the patched model.'
Check (!(Presentation $item 'core:ui/items/other' $null).Success) 'Duplicate native presentation accepted.'
$artScope.Lifetime.Dispose()
Check ($item.FileName -eq 'core:ui/items/old_icon' -and
    $item.ModelFileName -eq 'core:gamedata/models/old_model') 'Native item presentation rollback failed.'

$mod = [Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $root 'Mods')).Mods | Where-Object { $_.Id.Value -eq 'de128' }
$catalog = [Eclipse.Modding.ModContentCatalog]::new()
[xml]$xml = '<Items><Item Name="fixture" Type="Weapon" Level="1" UpgradeLevel="600" WeaponDamage="5" BonusPrice="30" ShopHide="1"/></Items>'
[void][Eclipse.Modding.CoreContentImporter]::ImportWeapons($catalog, [Xml.XmlNode[]]@($xml.Items.ChildNodes), $null)
$tx = $catalog.BeginRegistration($mod)
try {
    $id = [Eclipse.Modding.CoreContentImporter]::WeaponId('fixture')
    $tx.SetItemInitialProfile($id, 15, 1500, ([Eclipse.Modding.ModEquipmentInitialStats]::new(342, $null, $null, $null, $null, $null)), 'Weapon_Bonus', 'none')
    $tx.SetItemShopPrice($id, ([Eclipse.Modding.ModPrice]::new([Eclipse.Modding.ModPriceCurrency]::Gems, 39)))
    $tx.SetItemPresentation($id, [Eclipse.Modding.AssetId]::Parse('core:ui/items/weapon_kunai'),
        [Eclipse.Modding.AssetId]::Parse('core:gamedata/models/mdl_weapon_cool_katana'))
    $tx.Commit()
} finally { $tx.Dispose() }
$catalog.Freeze()
$items = [Items]::new(); $item = Item 'Weapon'; $items.AllItems.Add($item)
$adapter = [Eclipse.Modding.LegacyContentAdapter]::new($catalog)
$missingTemplate = $false
try { $adapter.ApplyItems($items) }
catch { $missingTemplate = $_.Exception.ToString().Contains('Initial profile upgrade template is unavailable') }
Check ($missingTemplate -and $item.UpgradeTemplateName -eq 'Paid_Weapon_Bonus') 'Unavailable template did not reject atomically.'
$items.CEKOFEFDMLJ.Add(([UpgradeDataContainer]::new()))
$items.CEKOFEFDMLJ[0].Type = 'Weapon_Bonus'
$adapter.ApplyItems($items)
Check ($item.ItemLevel -eq 15 -and (RawStat $item 'WeaponDamage').Value -eq 342 -and
    $item.UpgradeTemplateName -eq 'Weapon_Bonus' -and $item.LegacyPaidItem -eq 'None' -and
    $item.OHBBLIMNIMJ() -eq 0 -and $item.MCNMMBCJADI() -eq 39 -and
    $item.FileName -eq 'core:ui/items/weapon_kunai' -and
    $item.ModelFileName -eq 'core:gamedata/models/mdl_weapon_cool_katana') 'Adapter did not apply the profile, price and art.'
[xml]$newPurchaseNode = '<Item Name="fixture" Count="1" UpgradeLevel="-1" Equipped="0"/>'
$newPurchase = [UserItem]::new($newPurchaseNode.DocumentElement)
$newPurchase.KIGHKCOCJFJ($item)
Check ($newPurchase.DHNNCAEEMLL() -eq 1500 -and
    [object]::ReferenceEquals($newPurchase.BHKHOJPANHE(), $item)) 'New inventory item did not inherit the patched catalog profile.'
[xml]$savedNode = '<Item Name="fixture" Count="1" UpgradeLevel="600" Equipped="0"/>'
$savedItem = [UserItem]::new($savedNode.DocumentElement)
$savedItem.KIGHKCOCJFJ($item)
Check ($savedItem.DHNNCAEEMLL() -eq 600) 'Existing saved upgrade level was migrated unexpectedly.'
$null = [Eclipse.Modding.LegacyContentAdapter].GetMethod('RemoveItems', $flags).Invoke($adapter, @())
Check ($item.ItemLevel -eq 1 -and (RawStat $item 'WeaponDamage').Value -eq 5 -and
    $item.UpgradeTemplateName -eq 'Paid_Weapon_Bonus' -and $item.LegacyPaidItem -eq 'Paid' -and
    $item.OHBBLIMNIMJ() -eq 7 -and $item.MCNMMBCJADI() -eq 30 -and
    $item.FileName -eq 'core:ui/items/old_icon' -and
    $item.ModelFileName -eq 'core:gamedata/models/old_model') 'Adapter did not roll back the profile, price and art.'
Write-Output "PASS $script:checks native item initial-profile checks. Cloning, invalid input and adapter rollback; no Unity purchase or save."
