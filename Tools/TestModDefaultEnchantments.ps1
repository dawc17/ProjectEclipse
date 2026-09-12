# Actual ItemInfo parser/projection/lifetime; acquisition, profile writes and UI rendering are not exercised.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
msbuild (Join-Path $root 'Assembly-CSharp.csproj') /nologo /v:quiet /clp:ErrorsOnly
if ($LASTEXITCODE -ne 0) { throw 'Managed build failed.' }
. (Join-Path $PSScriptRoot 'LoadUnityManagedAssemblies.ps1')
$null = Import-SF2ManagedRuntime $root
$flags = [Reflection.BindingFlags]'Instance,NonPublic'
$method = [ItemInfo].GetMethod('TryOverrideDefaultEnchantments',$flags)
$script:checks = 0
function Check([bool]$condition,[string]$message) { $script:checks++; if (!$condition) { throw $message } }
function Apply([string]$text) {
    [xml]$xml = $text
    $arguments = [object[]]@($xml.DocumentElement,$null)
    $success = $method.Invoke($item,$arguments)
    return @{ Success=$success; Lifetime=$arguments[1]; Xml=$xml }
}
[xml]$base = '<Perk Name="fixture.default" PerkType="Single"/>'
[void][GameUtils]::FDEJIIDIPBI.AddExternalBasePerk($base.DocumentElement)
$item = [ItemInfo]::new($null)
$originalPreview = $item.LFIGBCDJHPG
$originalGrants = $item.APMJCGBNEDI
$result = Apply '<Enchantments><Perk Name="fixture.default"/></Enchantments>'
Check $result.Success 'Valid default loadout rejected.'
Check ($item.LFIGBCDJHPG.Count -eq 1 -and $item.APMJCGBNEDI.Count -eq 1) 'Preview and acquisition defaults disagree.'
Check ($item.APMJCGBNEDI[0].get_Name() -eq 'fixture.default') 'Grant identity changed.'
Check ($originalPreview.Count -eq 0 -and $originalGrants.Count -eq 0) 'Original lists mutated.'
$result.Xml.DocumentElement.FirstChild.SetAttribute('Name','edited')
Check ($item.APMJCGBNEDI[0].get_Name() -eq 'fixture.default') 'Input XML mutation leaked.'
Check (!(Apply '<Enchantments/>').Success) 'Conflicting override accepted.'
$first = $result.Lifetime
$first.Dispose()
Check ([object]::ReferenceEquals($originalPreview,$item.LFIGBCDJHPG) -and [object]::ReferenceEquals($originalGrants,$item.APMJCGBNEDI)) 'Original list identities not restored.'
foreach ($invalid in @('<Wrong/>','<Enchantments><Wrong/></Enchantments>','<Enchantments><Perk Name="fixture.default"/><Perk Name="absent"/></Enchantments>','<Enchantments><Perk Name="fixture.default"/><Perk Name="fixture.default"/></Enchantments>')) {
    $failed = Apply $invalid
    Check (!$failed.Success -and $null -eq $failed.Lifetime) 'Malformed/missing/duplicate loadout accepted.'
    Check ([object]::ReferenceEquals($originalPreview,$item.LFIGBCDJHPG) -and [object]::ReferenceEquals($originalGrants,$item.APMJCGBNEDI)) 'Failed application partially changed item.'
}
$next = Apply '<Enchantments><Perk Name="fixture.default"/></Enchantments>'
$first.Dispose()
Check ($item.APMJCGBNEDI.Count -eq 1) 'Stale lifetime removed newer defaults.'
$next.Lifetime.Dispose()
$empty = Apply '<Enchantments/>'
Check $empty.Success 'Explicit empty loadout rejected.'
$empty.Lifetime.Dispose()
$empty.Lifetime.Dispose()
Check ([object]::ReferenceEquals($originalGrants,$item.APMJCGBNEDI)) 'Repeated disposal failed.'
$parameterized = Apply '<Enchantments><Perk Name="fixture.default"><Set Aspect="123"/></Perk></Enchantments>'
Check $parameterized.Success 'Parameterized loadout rejected.'
Check (($item.APMJCGBNEDI[0].Pairs | Where-Object Key -eq 'Aspect').Value -eq '123') 'Acquisition parameters lost.'
Check (![object]::ReferenceEquals($item.LFIGBCDJHPG[0],[GameUtils]::FDEJIIDIPBI.ABAGJKMKCBA('fixture.default'))) 'Parameterized preview shares mutable core perk.'
$parameterized.Xml.DocumentElement.FirstChild.Set.SetAttribute('Aspect','999')
Check (($item.APMJCGBNEDI[0].Pairs | Where-Object Key -eq 'Aspect').Value -eq '123') 'Source parameter mutation leaked.'
$parameterized.Lifetime.Dispose()
$rows = [Collections.Generic.List[string]]::new()
for ($index=0; $index -lt 65; $index++) {
    [xml]$definition = "<Perk Name='fixture.limit$index' PerkType='Single'/>"
    [void][GameUtils]::FDEJIIDIPBI.AddExternalBasePerk($definition.DocumentElement)
    $rows.Add("<Perk Name='fixture.limit$index'/>")
}
$bounded = Apply ('<Enchantments>' + ($rows.GetRange(0,64) -join '') + '</Enchantments>')
Check ($bounded.Success -and $item.APMJCGBNEDI.Count -eq 64) 'Maximum supported loadout rejected.'
$bounded.Lifetime.Dispose()
$oversized = Apply ('<Enchantments>' + ($rows -join '') + '</Enchantments>')
Check (!$oversized.Success -and [object]::ReferenceEquals($originalGrants,$item.APMJCGBNEDI)) 'Oversized loadout changed defaults.'
$mod = [Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $root 'Mods')).Mods | Where-Object { $_.Id.Value -eq 'example.charge-ui' }
function Catalog([bool]$missing,[int]$aspect=123,[bool]$freeze=$true) {
    $catalog = [Eclipse.Modding.ModContentCatalog]::new()
    [xml]$weapons = '<Items><Item Name="fixture_weapon" Type="Weapon" WeaponDamage="5"/><Item Name="fixture_absent" Type="Weapon" WeaponDamage="5"/></Items>'
    [void][Eclipse.Modding.CoreContentImporter]::ImportWeapons($catalog,[Xml.XmlNode[]]@($weapons.Items.ChildNodes),$null)
    [xml]$perks = '<Perks><Perk Name="fixture.default" PerkType="Single"/></Perks>'
    [void][Eclipse.Modding.CoreContentImporter]::ImportPerks($catalog,[Xml.XmlNode[]]@($perks.Perks.ChildNodes))
    $transaction = $catalog.BeginRegistration($mod)
    try {
        $entry = [Eclipse.Modding.ModDefaultEnchantment]::new([Eclipse.Modding.DefinitionId]::Parse('core:perks/fixture.default'),$aspect)
        $transaction.SetDefaultEnchantments([Eclipse.Modding.CoreContentImporter]::WeaponId('fixture_weapon'),[Eclipse.Modding.ModDefaultEnchantment[]]@($entry))
        if ($missing) { $transaction.SetDefaultEnchantments([Eclipse.Modding.CoreContentImporter]::WeaponId('fixture_absent'),[Eclipse.Modding.ModDefaultEnchantment[]]@($entry)) }
        $transaction.Commit()
    } finally { $transaction.Dispose() }
    if ($freeze) { $catalog.Freeze() }
    return $catalog
}
$content = Catalog $false
$mutable = Catalog $false 123 $false
$tx = $mutable.BeginRegistration($mod)
try {
    $entry = [Eclipse.Modding.ModDefaultEnchantment]::new([Eclipse.Modding.DefinitionId]::Parse('core:perks/fixture.default'),999)
    $tx.SetDefaultEnchantments([Eclipse.Modding.CoreContentImporter]::WeaponId('fixture_absent'),[Eclipse.Modding.ModDefaultEnchantment[]]@($entry))
    $tx.SetDefaultEnchantments([Eclipse.Modding.CoreContentImporter]::WeaponId('fixture_weapon'),[Eclipse.Modding.ModDefaultEnchantment[]]@($entry))
    $failure = $null
    try { $tx.Commit() } catch { $failure = $_ }
    Check ($null -ne $failure -and $failure.ToString().Contains('already patched')) 'Committed loadout conflict accepted.'
} finally { $tx.Dispose() }
Check ($mutable.ItemDefaultEnchantments.Count -eq 1 -and $mutable.ItemDefaultEnchantments[0].Entries[0].Aspect -eq 123) 'Conflicting transaction partially published defaults.'
$tx = $mutable.BeginRegistration($mod)
try {
    $entries = [Eclipse.Modding.ModDefaultEnchantment[]]@($entry)
    $tx.SetDefaultEnchantments([Eclipse.Modding.CoreContentImporter]::WeaponId('fixture_absent'),$entries)
    $entries[0] = [Eclipse.Modding.ModDefaultEnchantment]::new([Eclipse.Modding.DefinitionId]::Parse('core:perks/fixture.default'),1)
    $tx.Commit()
} finally { $tx.Dispose() }
Check ($mutable.ItemDefaultEnchantments[1].Entries[0].Aspect -eq 999) 'Caller array mutation changed registered loadout.'
$changed = Catalog $false 124
$emptyMods = [Eclipse.Modding.ModDescriptor[]]@()
Check ([Eclipse.Modding.ModSaveData]::ComputeContentSetFingerprint($emptyMods,$content) -ne [Eclipse.Modding.ModSaveData]::ComputeContentSetFingerprint($emptyMods,$changed)) 'Default aspect missing from compatibility fingerprint.'
$items = [Items]::new()
$item.Name = 'fixture_weapon'
$items.HCDLKHKBEPF().Add($item)
$adapter = [Eclipse.Modding.LegacyContentAdapter]::new($content)
$adapter.ApplyItems($items)
$forge = [ForgeManager]::new()
$adapter.ApplyPerksAndEnchantments([GameUtils]::FDEJIIDIPBI,$forge)
Check ($item.APMJCGBNEDI.Count -eq 1 -and ($item.APMJCGBNEDI[0].Pairs | Where-Object Key -eq 'Aspect').Value -eq '123') 'Committed loadout did not reach native defaults.'
$remove = [Eclipse.Modding.LegacyContentAdapter].GetMethod('RemovePerksAndEnchantments',$flags)
$null = $remove.Invoke($adapter,@())
Check ([object]::ReferenceEquals($originalGrants,$item.APMJCGBNEDI)) 'Adapter did not restore native default identity.'
$broken = [Eclipse.Modding.LegacyContentAdapter]::new((Catalog $true))
$broken.ApplyItems($items)
$failure = $null
try { $broken.ApplyPerksAndEnchantments([GameUtils]::FDEJIIDIPBI,$forge) } catch { $failure = $_ }
Check ($null -ne $failure -and $failure.ToString().Contains('fixture_absent')) 'Missing second item not rejected.'
Check ([object]::ReferenceEquals($originalGrants,$item.APMJCGBNEDI)) 'Later missing item did not restore earlier override.'
$adapter.ApplyPerksAndEnchantments([GameUtils]::FDEJIIDIPBI,$forge)
Check ($item.APMJCGBNEDI.Count -eq 1) 'Failed adapter poisoned later application.'
$null = $remove.Invoke($adapter,@())
Write-Output "PASS: $script:checks compiled default-enchantment registration/projection/rollback checks; no grant or rendering claim."
