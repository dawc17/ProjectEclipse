# Compiled native item parsing/copying and AI weapon-group updates; no running fight.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
msbuild (Join-Path $root 'Assembly-CSharp.csproj') /nologo /v:quiet /clp:ErrorsOnly
if ($LASTEXITCODE -ne 0) { throw 'Managed build failed.' }
. (Join-Path $PSScriptRoot 'LoadUnityManagedAssemblies.ps1')
$null = Import-SF2ManagedRuntime $root
$flags = [Reflection.BindingFlags]'Instance,NonPublic'
$effective = [ItemInfo].GetProperty('EffectiveTacticSubtype',$flags)
$script:checks=0
function Check([bool]$condition,[string]$message) { $script:checks++; if (!$condition) { throw $message } }
function Parse([string]$text) { [xml]$xml=$text; $item=[ItemInfo]::new($null); [ItemInfo].GetMethod('ReadCombatClassification',$flags).Invoke($item,@($xml.DocumentElement)) | Out-Null; return $item }
$mace=Parse '<Item Name="test_mace" Type="Weapon" SubType="TwoHandedBlunt" TacticSubtype="TwoHanded"/>'
Check ($effective.GetValue($mace) -eq 'TwoHanded') 'Explicit tactic grouping was ignored.'
Check ($mace.MDPPNGIEJGD -eq 'TwoHandedBlunt') 'AI grouping changed animation/condition subtype.'
$copy=$mace.Clone()
Check ($effective.GetValue($copy) -eq 'TwoHanded' -and $copy.MDPPNGIEJGD -eq 'TwoHandedBlunt') 'Clone lost separate subtype/group.'
$normal=Parse '<Item Name="test_sword" Type="Weapon" SubType="OneHandedSword"/>'
Check ($effective.GetValue($normal) -eq 'OneHandedSword') 'Absent tactic subtype did not fall back.'
$normal.MDPPNGIEJGD='Barehand'
Check ($effective.GetValue($normal) -eq 'Barehand') 'Fallback retained a stale subtype.'
$normal.MergeWithItem($mace)
Check ($effective.GetValue($normal) -eq 'TwoHanded' -and $normal.MDPPNGIEJGD -eq 'TwoHandedBlunt') 'Item merge lost explicit grouping.'
$empty=Parse '<Item Name="empty" Type="Weapon"/>'
$normal.MergeWithItem($empty)
Check ($effective.GetValue($normal) -eq 'TwoHanded') 'Absent merge grouping erased template metadata.'
[xml]$vanilla=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/vanillaXml/list.xml')
$records=$vanilla.SelectNodes('//Item[@TacticSubtype]')
Check ($records.Count -gt 0) 'Canonical metadata fixtures missing.'
foreach ($record in $records) {
    # Project the exact canonical identity fields; unrelated price/resource parsing
    # is outside this test and may require a live Unity host.
    $item=Parse ('<Item Name="fixture" Type="Weapon" SubType="'+$record.GetAttribute('SubType')+'" TacticSubtype="'+$record.GetAttribute('TacticSubtype')+'"/>')
    Check ($effective.GetValue($item) -eq $record.GetAttribute('TacticSubtype')) ('Canonical grouping lost: '+$record.GetAttribute('Name'))
}
$ai=[Runtime.Serialization.FormatterServices]::GetUninitializedObject([ModelAi])
$own=[ModelAi].GetField('EIMKBOMDAAE',$flags)
$enemy=[ModelAi].GetField('HCJOIHLKOKJ',$flags)
$ai.SetWeaponEnemy('Katars')
$ai.SetWeaponBot($effective.GetValue($mace))
Check ($own.GetValue($ai) -eq 'TwoHanded' -and $enemy.GetValue($ai) -eq 'Katars') 'Own weapon update clobbered enemy grouping.'
$ai.SetWeaponBot('Barehand')
Check ($own.GetValue($ai) -eq 'Barehand' -and $enemy.GetValue($ai) -eq 'Katars') 'Disarmed weapon grouping remained stale.'
$ai.SetWeaponEnemy('Staff')
Check ($own.GetValue($ai) -eq 'Barehand' -and $enemy.GetValue($ai) -eq 'Staff') 'Enemy update changed own grouping.'
$override=[ItemInfo].GetMethod('TryOverrideTacticSubtype',$flags)
function ApplyGroup($target,$group) {
    $arguments=[object[]]@($group,$null)
    $success=$override.Invoke($target,$arguments)
    return @{ Success=$success; Lifetime=$arguments[1] }
}
$beforeXml=$mace.NodeXML
$scope=ApplyGroup $mace 'Staff'
Check $scope.Success 'Valid scoped AI group rejected.'
Check ($effective.GetValue($mace) -eq 'Staff' -and $mace.MDPPNGIEJGD -eq 'TwoHandedBlunt') 'Scoped group changed physical subtype.'
Check ([object]::ReferenceEquals($beforeXml,$mace.NodeXML)) 'Scoped group changed archival item XML.'
$snapshot=$mace.Clone()
Check ($effective.GetValue($snapshot) -eq 'Staff') 'Fight copy lost active group.'
Check (!(ApplyGroup $mace 'Katars').Success) 'Concurrent group override accepted.'
$scope.Lifetime.Dispose()
Check ($effective.GetValue($mace) -eq 'TwoHanded') 'Original group not restored.'
Check ($effective.GetValue($snapshot) -eq 'Staff') 'Teardown unexpectedly mutated an existing fight copy.'
$fresh=ApplyGroup $mace ''
Check ($fresh.Success -and $effective.GetValue($mace) -eq 'TwoHandedBlunt') 'Explicit fallback did not remove original group.'
$scope.Lifetime.Dispose()
Check ($effective.GetValue($mace) -eq 'TwoHandedBlunt') 'Stale scope removed a newer override.'
$fresh.Lifetime.Dispose()
Check ($effective.GetValue($mace) -eq 'TwoHanded') 'Fallback teardown did not restore explicit native group.'
foreach ($invalid in @($null,'a b','x/y','x:y',('x'*129))) {
    Check (!(ApplyGroup $mace $invalid).Success) 'Invalid AI group accepted.'
    Check ($effective.GetValue($mace) -eq 'TwoHanded') 'Invalid group changed native state.'
}
$armor=Parse '<Item Type="Armor"/>'
Check (!(ApplyGroup $armor 'Staff').Success) 'Nonweapon AI group override accepted.'
$maximum=ApplyGroup $mace ('x'*128)
Check $maximum.Success 'Maximum-length group rejected.'
$maximum.Lifetime.Dispose()
$mod=[Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $root 'Mods')).Mods | Where-Object { $_.Id.Value -eq 'example.charge-ui' }
function Catalog([string]$group,[bool]$missing=$false,[bool]$freeze=$true) {
    $catalog=[Eclipse.Modding.ModContentCatalog]::new()
    [xml]$xml='<Items><Item Name="fixture_weapon" Type="Weapon" SubType="TwoHandedBlunt" TacticSubtype="TwoHanded"/><Item Name="fixture_missing" Type="Weapon"/></Items>'
    [void][Eclipse.Modding.CoreContentImporter]::ImportWeapons($catalog,[Xml.XmlNode[]]@($xml.Items.ChildNodes),$null)
    $tx=$catalog.BeginRegistration($mod)
    try {
        $tx.SetTacticSubtype([Eclipse.Modding.CoreContentImporter]::WeaponId('fixture_weapon'),$group)
        if ($missing) { $tx.SetTacticSubtype([Eclipse.Modding.CoreContentImporter]::WeaponId('fixture_missing'),'Staff') }
        $tx.Commit()
    } finally { $tx.Dispose() }
    if ($freeze) { $catalog.Freeze() }
    return $catalog
}
$catalog=Catalog 'Katars'
$different=Catalog 'Staff'
$mods=[Eclipse.Modding.ModDescriptor[]]@()
Check ([Eclipse.Modding.ModSaveData]::ComputeContentSetFingerprint($mods,$catalog) -ne [Eclipse.Modding.ModSaveData]::ComputeContentSetFingerprint($mods,$different)) 'Changed core group missing from fingerprint.'
$mutable=Catalog 'Katars' $false $false
$tx=$mutable.BeginRegistration($mod)
try {
    $tx.SetTacticSubtype([Eclipse.Modding.CoreContentImporter]::WeaponId('fixture_missing'),'Staff')
    $tx.SetTacticSubtype([Eclipse.Modding.CoreContentImporter]::WeaponId('fixture_weapon'),'Staff')
    $failure=$null
    try { $tx.Commit() } catch { $failure=$_ }
    Check ($null -ne $failure -and $failure.ToString().Contains('already patched')) 'Committed group conflict accepted.'
} finally { $tx.Dispose() }
Check ($mutable.ItemTacticSubtypes.Count -eq 1 -and $mutable.ItemTacticSubtypes[0].Group -eq 'Katars') 'Failed transaction partially published grouping.'
$items=[Items]::new(); $mace.Name='fixture_weapon'; $items.HCDLKHKBEPF().Add($mace)
$adapter=[Eclipse.Modding.LegacyContentAdapter]::new($catalog)
$adapter.ApplyItems($items)
$adapter.ApplyPerksAndEnchantments([GameUtils]::FDEJIIDIPBI,[ForgeManager]::new())
Check ($effective.GetValue($mace) -eq 'Katars') 'Catalog group did not reach native item.'
$remove=[Eclipse.Modding.LegacyContentAdapter].GetMethod('RemovePerksAndEnchantments',$flags)
$null=$remove.Invoke($adapter,@())
Check ($effective.GetValue($mace) -eq 'TwoHanded') 'Adapter teardown did not restore group.'
$broken=[Eclipse.Modding.LegacyContentAdapter]::new((Catalog 'Staff' $true))
$broken.ApplyItems($items)
$failure=$null
try { $broken.ApplyPerksAndEnchantments([GameUtils]::FDEJIIDIPBI,[ForgeManager]::new()) } catch { $failure=$_ }
Check ($null -ne $failure -and $failure.ToString().Contains('tactic subtype')) 'Missing later native target did not fail.'
Check ($effective.GetValue($mace) -eq 'TwoHanded') 'Partial application did not restore earlier native target.'
$adapter.ApplyPerksAndEnchantments([GameUtils]::FDEJIIDIPBI,[ForgeManager]::new())
Check ($effective.GetValue($mace) -eq 'Katars') 'Rollback prevented later application.'
$null=$remove.Invoke($adapter,@())
Write-Output "Native item tactic subtype: $script:checks checks passed."
