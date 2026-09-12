# Actual native item overlay and ModelParameters equipment-perk collection; no live combat/playback.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
msbuild (Join-Path $root 'Assembly-CSharp.csproj') /nologo /v:quiet /clp:ErrorsOnly
if ($LASTEXITCODE -ne 0) { throw 'Managed build failed.' }
. (Join-Path $PSScriptRoot 'LoadUnityManagedAssemblies.ps1')
$null = Import-SF2ManagedRuntime $root
$flags = [Reflection.BindingFlags]'Instance,NonPublic'
$method = [ItemInfo].GetMethod('TryOverrideInnatePerks',$flags)
$script:checks=0
function Check([bool]$condition,[string]$message) { $script:checks++; if (!$condition) { throw $message } }
function Apply($item,[string]$text) {
    [xml]$xml=$text
    $arguments=[object[]]@($xml.DocumentElement,$null)
    $success=$method.Invoke($item,$arguments)
    return @{ Success=$success; Lifetime=$arguments[1]; Xml=$xml }
}
[xml]$xml='<Perk Name="fixture.innate" PerkType="Single"/>'
$definition=[GameUtils]::FDEJIIDIPBI.AddExternalBasePerk($xml.DocumentElement)
$weapon=[ItemInfo]::new($null); $weapon.Type='Weapon'
$armor=[ItemInfo]::new($null); $armor.Type='Armor'
$original=$weapon.NHBIJEEKALC
$grants=$weapon.APMJCGBNEDI
$preview=$weapon.LFIGBCDJHPG
$result=Apply $weapon '<Perks><Perk Name="fixture.innate"/></Perks>'
Check $result.Success 'Valid innate perk rejected.'
Check ($weapon.NHBIJEEKALC.Count -eq 1 -and $weapon.NHBIJEEKALC[0].Name -eq 'fixture.innate') 'Innate perk identity lost.'
Check (![object]::ReferenceEquals($definition,$weapon.NHBIJEEKALC[0])) 'Registry perk was not cloned.'
$other=Apply $armor '<Perks><Perk Name="fixture.innate"/></Perks>'
Check (![object]::ReferenceEquals($armor.NHBIJEEKALC[0],$weapon.NHBIJEEKALC[0])) 'Equipment objects share mutable perk.'
Check ([object]::ReferenceEquals($grants,$weapon.APMJCGBNEDI) -and [object]::ReferenceEquals($preview,$weapon.LFIGBCDJHPG)) 'Innate change affected acquisition enchantments.'
$model=[ModelParameters]::new()
$model.IsPlayer=$false
$model.JGMLKIPCFII=$weapon
Check ($model.JBIOECDAAKP().Exists([Predicate[PerkInfoItem]]{param($perk) $perk.Name -eq 'fixture.innate'})) 'Native model did not collect equipment perk.'
$marker=[PerkInfoItem].GetField('PGECFFBLKJL',$flags)
Check ($marker.GetValue($weapon.NHBIJEEKALC[0]) -and !$marker.GetValue($armor.NHBIJEEKALC[0]) -and !$marker.GetValue($definition)) 'Weapon marking leaked across item or registry boundaries.'
Check (!(Apply $weapon '<Perks/>').Success) 'Concurrent innate override accepted.'
$first=$result.Lifetime
$first.Dispose()
Check ([object]::ReferenceEquals($original,$weapon.NHBIJEEKALC)) 'Original innate list identity not restored.'
Check ($model.JBIOECDAAKP().Count -eq 0) 'Model still collects removed equipment perk.'
Check ($armor.NHBIJEEKALC.Count -eq 1) 'Weapon restoration affected armor.'
foreach ($invalid in @('<Wrong/>','<Perks><Perk Name="fixture.innate"/><Perk Name="missing"/></Perks>','<Perks><Perk Name="fixture.innate"/><Perk Name="fixture.innate"/></Perks>')) {
    $failed=Apply $weapon $invalid
    Check (!$failed.Success -and [object]::ReferenceEquals($original,$weapon.NHBIJEEKALC)) 'Invalid innate override partially applied.'
}
$next=Apply $weapon '<Perks><Perk Name="fixture.innate"><Set Aspect="123"/></Perk></Perks>'
$first.Dispose()
Check ($weapon.NHBIJEEKALC.Count -eq 1) 'Stale lifetime removed newer innate override.'
$next.Lifetime.Dispose(); $next.Lifetime.Dispose(); $other.Lifetime.Dispose()
Check ([object]::ReferenceEquals($original,$weapon.NHBIJEEKALC)) 'Repeated disposal changed restoration.'
$empty=Apply $weapon '<Perks/>'
Check $empty.Success 'Explicit empty innate loadout rejected.'
$empty.Lifetime.Dispose()
$rows=[Collections.Generic.List[string]]::new()
for ($index=0; $index -lt 65; $index++) {
    [xml]$node="<Perk Name='fixture.innate$index' PerkType='Single'/>"
    [void][GameUtils]::FDEJIIDIPBI.AddExternalBasePerk($node.DocumentElement)
    $rows.Add("<Perk Name='fixture.innate$index'/>")
}
$bounded=Apply $weapon ('<Perks>'+($rows.GetRange(0,64) -join '')+'</Perks>')
Check ($bounded.Success -and $weapon.NHBIJEEKALC.Count -eq 64) 'Bounded innate list rejected.'
$bounded.Lifetime.Dispose()
$oversized=Apply $weapon ('<Perks>'+($rows -join '')+'</Perks>')
Check (!$oversized.Success -and [object]::ReferenceEquals($original,$weapon.NHBIJEEKALC)) 'Oversized innate list changed equipment.'
$mod=[Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $root 'Mods')).Mods | Where-Object { $_.Id.Value -eq 'example.charge-ui' }
function Catalog([bool]$missing,[float]$aspect=123) {
    $catalog=[Eclipse.Modding.ModContentCatalog]::new()
    [xml]$itemsXml='<Items><Item Name="innate_weapon" Type="Weapon" WeaponDamage="5"/><Item Name="innate_absent" Type="Weapon" WeaponDamage="5"/></Items>'
    [void][Eclipse.Modding.CoreContentImporter]::ImportWeapons($catalog,[Xml.XmlNode[]]@($itemsXml.Items.ChildNodes),$null)
    [xml]$perksXml='<Perks><Perk Name="fixture.innate" PerkType="Single"/></Perks>'
    [void][Eclipse.Modding.CoreContentImporter]::ImportPerks($catalog,[Xml.XmlNode[]]@($perksXml.Perks.ChildNodes))
    $tx=$catalog.BeginRegistration($mod)
    try {
        $perk=[Eclipse.Modding.DefinitionId]::Parse('core:perks/fixture.innate')
        $target=[Eclipse.Modding.CoreContentImporter]::WeaponId('innate_weapon')
        $parameters=[Collections.Generic.Dictionary[string,float]]::new()
        $parameters.Add('Aspect',$aspect)
        $entry=[Eclipse.Modding.ModInnatePerk]::new($perk,$parameters)
        $parameters['Aspect']=999
        Check ($entry.Parameters['Aspect'] -eq $aspect) 'Caller parameter mutation changed innate definition.'
        $tx.SetInnatePerks($target,[Eclipse.Modding.ModInnatePerk[]]@($entry))
        $tx.SetDefaultEnchantments($target,[Eclipse.Modding.ModDefaultEnchantment[]]@([Eclipse.Modding.ModDefaultEnchantment]::new($perk,50)))
        if ($missing) { $tx.SetInnatePerks([Eclipse.Modding.CoreContentImporter]::WeaponId('innate_absent'),[Eclipse.Modding.ModInnatePerk[]]@($entry)) }
        $tx.Commit()
    } finally { $tx.Dispose() }
    $catalog.Freeze()
    return $catalog
}
$content=Catalog $false
$changed=Catalog $false 124
$none=[Eclipse.Modding.ModDescriptor[]]@()
Check ([Eclipse.Modding.ModSaveData]::ComputeContentSetFingerprint($none,$content) -ne [Eclipse.Modding.ModSaveData]::ComputeContentSetFingerprint($none,$changed)) 'Innate parameter missing from compatibility fingerprint.'
$items=[Items]::new(); $weapon.Name='innate_weapon'; $items.HCDLKHKBEPF().Add($weapon)
$adapter=[Eclipse.Modding.LegacyContentAdapter]::new($content)
$adapter.ApplyItems($items)
$forge=[ForgeManager]::new()
$adapter.ApplyPerksAndEnchantments([GameUtils]::FDEJIIDIPBI,$forge)
Check ($weapon.NHBIJEEKALC.Count -eq 1 -and $weapon.APMJCGBNEDI.Count -eq 1) 'Innate/default loadouts failed to compose.'
Check ($model.JBIOECDAAKP().Count -eq 1) 'Adapted innate perk absent from native model collection.'
$remove=[Eclipse.Modding.LegacyContentAdapter].GetMethod('RemovePerksAndEnchantments',$flags)
$null=$remove.Invoke($adapter,@())
Check ([object]::ReferenceEquals($original,$weapon.NHBIJEEKALC) -and [object]::ReferenceEquals($grants,$weapon.APMJCGBNEDI)) 'Combined loadout teardown failed.'
$broken=[Eclipse.Modding.LegacyContentAdapter]::new((Catalog $true))
$broken.ApplyItems($items)
$failure=$null
try { $broken.ApplyPerksAndEnchantments([GameUtils]::FDEJIIDIPBI,$forge) } catch { $failure=$_ }
Check ($null -ne $failure -and $failure.ToString().Contains('innate_absent')) 'Missing later innate target accepted.'
Check ([object]::ReferenceEquals($original,$weapon.NHBIJEEKALC) -and [object]::ReferenceEquals($grants,$weapon.APMJCGBNEDI)) 'Failed innate application left loadouts active.'
foreach ($key in @('bad key','Aspect')) {
    $parameters=[Collections.Generic.Dictionary[string,float]]::new()
    $parameters.Add($key,$(if ($key -eq 'Aspect') { [float]::NaN } else { [float]1 }))
    $failure=$null
    try { [void][Eclipse.Modding.ModInnatePerk]::new([Eclipse.Modding.DefinitionId]::Parse('core:perks/fixture.innate'),$parameters) } catch { $failure=$_ }
    Check ($null -ne $failure) 'Invalid innate parameter accepted.'
}
Write-Output "PASS: $script:checks innate registration/native projection/collection/restoration checks; no live fight acceptance."
