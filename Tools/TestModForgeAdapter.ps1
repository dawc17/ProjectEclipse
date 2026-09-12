# Real compiled adapter/recipe lifecycle; Unity file discovery and profile services are bypassed.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
msbuild (Join-Path $root 'Assembly-CSharp.csproj') /nologo /v:quiet /clp:ErrorsOnly
if ($LASTEXITCODE -ne 0) { throw 'Managed build failed.' }
. (Join-Path $PSScriptRoot 'LoadUnityManagedAssemblies.ps1')
$null = Import-SF2ManagedRuntime $root
$flags = [Reflection.BindingFlags]'Instance,NonPublic'
$forge = [ForgeManager]::new()
[ForgeManager].GetField('_parsed',$flags).SetValue($forge,$true)
[xml]$xml = Get-Content -Raw (Join-Path $root 'Assets/vanillaXml/forge.xml')
$recipe = [Recipe]::new($xml.SelectSingleNode('//Recipe[@Name="Complex"]'))
[ForgeManager].GetField('_recipes',$flags).GetValue($forge).Add($recipe)
$nativeCount = $recipe.Variations.Count
$mod = [Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $root 'Mods')).Mods | Where-Object { $_.Id.Value -eq 'example.charge-ui' }
function Catalog([bool]$missing,[bool]$deviation=$false,[bool]$badDeviation=$false) {
    $catalog = [Eclipse.Modding.ModContentCatalog]::new()
    [void][Eclipse.Modding.CoreContentImporter]::ImportForgeEconomicProfiles($catalog,[string[]]@('Complex','Simple'))
    [xml]$perks = '<Perks><Perk Name="PERK_MONK_SET_WHIRL"/><Perk Name="MISSING_NATIVE"/></Perks>'
    [void][Eclipse.Modding.CoreContentImporter]::ImportPerks($catalog,[Xml.XmlNode[]]@($perks.Perks.ChildNodes))
    $tx = $catalog.BeginRegistration($mod)
    try {
        $profile = [Eclipse.Modding.DefinitionId]::Parse('core:forge-profiles/Complex')
        $perk = [Eclipse.Modding.DefinitionId]::Parse('core:perks/PERK_MONK_SET_WHIRL')
        $tx.ExcludeForgeCandidate($profile,$perk,[Eclipse.Modding.ModEquipmentKind]::Weapon)
        if ($missing) { $perk = [Eclipse.Modding.DefinitionId]::Parse('core:perks/MISSING_NATIVE') }
        $tx.ExcludeForgeCandidate($profile,$perk,[Eclipse.Modding.ModEquipmentKind]::Armor)
        if ($deviation) {
            $tx.OverrideForgeDeviation([Eclipse.Modding.DefinitionId]::Parse('core:forge-profiles/Simple'),[Eclipse.Modding.ModEquipmentKind]::Weapon,15,75)
            if ($badDeviation) { $tx.OverrideForgeDeviation($profile,[Eclipse.Modding.ModEquipmentKind]::Weapon,15,75) }
        }
        $tx.Commit()
    } finally { $tx.Dispose() }
    $catalog.Freeze()
    return $catalog
}
$script:checks = 0
function Check([bool]$condition,[string]$message) { $script:checks++; if (!$condition) { throw $message } }
$excluded = [Recipe].GetField('_excludedNativeCandidates',$flags).GetValue($recipe)
$remove = [Eclipse.Modding.LegacyContentAdapter].GetMethod('RemovePerksAndEnchantments',$flags)
$adapter = [Eclipse.Modding.LegacyContentAdapter]::new((Catalog $false))
$adapter.ApplyPerksAndEnchantments([PerkItems]::new(),$forge)
Check ($excluded.Count -eq 2) 'Native adapter did not apply both categories.'
Check ($excluded['Weapon'].Contains('PERK_MONK_SET_WHIRL')) 'Native perk name mapping changed.'
$null = $remove.Invoke($adapter,@())
Check ($excluded.Count -eq 0) 'Native teardown retained exclusions.'
$null = $remove.Invoke($adapter,@())
Check ($excluded.Count -eq 0) 'Repeated teardown changed restored state.'
$adapter.ApplyPerksAndEnchantments([PerkItems]::new(),$forge)
Check ($excluded.Count -eq 2) 'Reapplication after teardown failed.'
$null = $remove.Invoke($adapter,@())
$broken = [Eclipse.Modding.LegacyContentAdapter]::new((Catalog $true))
$failure = $null
try { $broken.ApplyPerksAndEnchantments([PerkItems]::new(),$forge) } catch { $failure = $_ }
Check ($null -ne $failure -and $failure.ToString().Contains('Candidate or equipment is missing')) 'Missing native candidate was not rejected at apply.'
Check ($excluded.Count -eq 0) 'Failure after first exclusion did not roll back.'
Check ($recipe.Variations.Count -eq $nativeCount) 'Adapter changed original variations.'
$adapter.ApplyPerksAndEnchantments([PerkItems]::new(),$forge)
Check ($excluded.Count -eq 2) 'Failed adapter poisoned subsequent application.'
$null = $remove.Invoke($adapter,@())
$simple = [Recipe]::new($xml.SelectSingleNode('//Recipe[@Name="Simple"]'))
[ForgeManager].GetField('_recipes',$flags).GetValue($forge).Add($simple)
$original = $simple.Items[0]
$combined = [Eclipse.Modding.LegacyContentAdapter]::new((Catalog $false $true))
$combined.ApplyPerksAndEnchantments([PerkItems]::new(),$forge)
Check ($simple.Items[0].MinDeviation -eq 15 -and $excluded.Count -eq 2) 'Deviation/exclusion composition failed.'
$null = $remove.Invoke($combined,@())
Check ([object]::ReferenceEquals($original,$simple.Items[0]) -and $excluded.Count -eq 0) 'Combined teardown failed.'
$bad = [Eclipse.Modding.LegacyContentAdapter]::new((Catalog $false $true $true))
$failure = $null
try { $bad.ApplyPerksAndEnchantments([PerkItems]::new(),$forge) } catch { $failure = $_ }
Check ($null -ne $failure -and $failure.ToString().Contains('random-aspect')) 'Fixed-aspect target not rejected.'
Check ([object]::ReferenceEquals($original,$simple.Items[0]) -and $excluded.Count -eq 0) 'Partial deviation failure left overlays active.'
$combined.ApplyPerksAndEnchantments([PerkItems]::new(),$forge)
Check ($simple.Items[0].MaxDeviation -eq 75) 'Deviation could not reapply after rollback.'
$null = $remove.Invoke($combined,@())
Write-Output "PASS: $script:checks compiled native forge adapter apply/teardown/rollback checks using canonical recipes; full game unload not exercised."
