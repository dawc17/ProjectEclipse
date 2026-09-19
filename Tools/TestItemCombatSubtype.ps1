# Native classification, move-condition routing and adapter rollback; no live save.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
dotnet build (Join-Path $root 'Assembly-CSharp.csproj') -v:q /clp:ErrorsOnly
if ($LASTEXITCODE -ne 0) { throw 'Managed build failed.' }
. (Join-Path $PSScriptRoot 'LoadUnityManagedAssemblies.ps1')
$null = Import-SF2ManagedRuntime $root
$flags = [Reflection.BindingFlags]'Instance,NonPublic'
$script:checks=0
function Check([bool]$ok,[string]$message) { $script:checks++; if (!$ok) {throw $message} }
function Parse([string]$text) {
    [xml]$xml=$text; $item=[ItemInfo]::new($null)
    [ItemInfo].GetMethod('ReadCombatClassification',$flags).Invoke($item,@($xml.DocumentElement)) | Out-Null
    $item.Name=$xml.DocumentElement.GetAttribute('Name'); $item.NodeXML=$xml.DocumentElement
    return $item
}
$override=[ItemInfo].GetMethod('TryOverrideCombatSubtype',$flags)
$effective=[ItemInfo].GetProperty('EffectiveTacticSubtype',$flags)
function ApplySubtype($item,$value) {
    $arguments=[object[]]@($value,$null)
    return @{Success=$override.Invoke($item,$arguments);Lifetime=$arguments[1]}
}
foreach ($case in @(@('Weapon','Spear','Naginata'),@('Weapon','Claws','HunterClaws'),@('Weapon','Spear','MagariYari'),@('Ranged','Chakram','Kunai'),@('Magic','FireBall','IceBall'))) {
    $item=Parse ('<Item Name="fixture" Type="'+$case[0]+'" SubType="'+$case[1]+'"/>')
    $xml=$item.NodeXML.OuterXml
    $scope=ApplySubtype $item $case[2]
    Check ($scope.Success -and $item.SubType -eq $case[2]) 'Subtype override failed.'
    Check ($effective.GetValue($item) -eq $case[2]) 'AI fallback did not follow subtype.'
    [xml]$conditionXml='<Item Type="'+$case[0]+'" SubType="'+$case[2]+'"/>'
    $condition=[ConditionItemInfo]::new($conditionXml.DocumentElement)
    $conditions=[ModelConditions]::new();$conditions.OJIAKDDCGLB=[Collections.Generic.List[ItemInfo]]::new();$conditions.OJIAKDDCGLB.Add($item)
    Check ($condition.IsEqual($conditions)) 'Native move condition did not see subtype.'
    $snapshot=$item.Clone()
    Check ($snapshot.SubType -eq $case[2]) 'New fighter snapshot lost subtype.'
    Check (!(ApplySubtype $item 'Other').Success) 'Concurrent native override accepted.'
    $scope.Lifetime.Dispose()
    Check ($item.SubType -eq $case[1] -and !$condition.IsEqual($conditions)) 'Teardown did not restore routing.'
    Check ($snapshot.SubType -eq $case[2] -and $item.NodeXML.OuterXml -eq $xml) 'Teardown changed snapshot or source XML.'
    $fresh=ApplySubtype $item 'NewFamily';$scope.Lifetime.Dispose()
    Check ($item.SubType -eq 'NewFamily') 'Stale scope erased newer override.'
    $fresh.Lifetime.Dispose()
}
$explicit=Parse '<Item Name="explicit" Type="Weapon" SubType="Spear" TacticSubtype="Staff"/>'
$scope=ApplySubtype $explicit 'Naginata'
Check ($scope.Success -and $effective.GetValue($explicit) -eq 'Staff') 'Combat subtype erased explicit AI grouping.'
$scope.Lifetime.Dispose()
foreach($invalid in @($null,'','bad value','a/b','a:b','ż',('x'*129))) {
    Check (!(ApplySubtype $explicit $invalid).Success -and $explicit.SubType -eq 'Spear') 'Invalid subtype changed item.'
}
foreach($type in @('Armor','Helm','Consumable')) {Check (!(ApplySubtype (Parse ('<Item Type="'+$type+'"/>')) 'Katana').Success) 'Invalid item category accepted.'}
$mod=[Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $root 'Mods')).Mods | Where-Object {$_.Id.Value -eq 'de128'}
function Catalog([bool]$broken=$false) {
    $catalog=[Eclipse.Modding.ModContentCatalog]::new()
    [xml]$xml='<Items><Item Name="fixture" Type="Weapon" SubType="Spear"/><Item Name="missing" Type="Weapon" SubType="Claws"/><Item Name="duplicate" Type="Ranged" SubType="Chakram"/><Item Name="duplicate" Type="Ranged" SubType="RifleBullet"/></Items>'
    [void][Eclipse.Modding.CoreContentImporter]::ImportWeapons($catalog,[Xml.XmlNode[]]@($xml.Items.ChildNodes),$null)
    [void][Eclipse.Modding.CoreContentImporter]::ImportRanged($catalog,[Xml.XmlNode[]]@($xml.Items.ChildNodes),$null)
    $tx=$catalog.BeginRegistration($mod)
    try {
        $tx.SetCombatSubtype([Eclipse.Modding.CoreContentImporter]::WeaponId('fixture'),'Naginata')
        $tx.SetCombatSubtype([Eclipse.Modding.DefinitionId]::Parse('core:items/ranged/duplicate/riflebullet'),'Kunai')
        if($broken){$tx.SetCombatSubtype([Eclipse.Modding.CoreContentImporter]::WeaponId('missing'),'HunterClaws')}
        $tx.Commit()
    } finally {$tx.Dispose()}
    $catalog.Freeze();return $catalog
}
$item=Parse '<Item Name="fixture" Type="Weapon" SubType="Spear"/>'
$first=Parse '<Item Name="duplicate" Type="Ranged" SubType="Chakram"/>'
$second=Parse '<Item Name="duplicate" Type="Ranged" SubType="RifleBullet"/>'
$items=[Items]::new();$items.AllItems.Add($item);$items.AllItems.Add($first);$items.AllItems.Add($second)
$remove=[Eclipse.Modding.LegacyContentAdapter].GetMethod('RemovePerksAndEnchantments',$flags)
foreach($broken in @($false,$true,$false)) {
    $adapter=[Eclipse.Modding.LegacyContentAdapter]::new((Catalog $broken));$adapter.ApplyItems($items)
    $failure=$null
    try {$adapter.ApplyPerksAndEnchantments([GameUtils]::FDEJIIDIPBI,[ForgeManager]::new())}catch{$failure=$_}
    if($broken) {
        Check ($null -ne $failure -and $failure.ToString().Contains('combat subtype')) 'Missing native target accepted.'
        Check ($item.SubType -eq 'Spear' -and $second.SubType -eq 'RifleBullet') 'Partial native application failed to roll back.'
    }else {
        Check ($null -eq $failure -and $item.SubType -eq 'Naginata') 'Adapter failed to apply subtype.'
        Check ($first.SubType -eq 'Chakram' -and $second.SubType -eq 'Kunai') 'Duplicate legacy name patched wrong item.'
    }
    $null=$remove.Invoke($adapter,@())
    Check ($item.SubType -eq 'Spear' -and $second.SubType -eq 'RifleBullet') 'Adapter teardown failed.'
}
Write-Output "PASS $script:checks native item subtype checks. Real item cloning, move conditions, adapter application and rollback; no Unity fight or save."
