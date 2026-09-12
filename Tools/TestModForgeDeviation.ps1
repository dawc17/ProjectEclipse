# Run actual compiled Recipe projection against canonical XML, without rolling or Unity profile I/O.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
msbuild (Join-Path $root 'Assembly-CSharp.csproj') /nologo /v:quiet /clp:ErrorsOnly
if ($LASTEXITCODE -ne 0) { throw 'Managed build failed.' }
. (Join-Path $PSScriptRoot 'LoadUnityManagedAssemblies.ps1')
$null = Import-SF2ManagedRuntime $root
[xml]$xml = Get-Content -Raw (Join-Path $root 'Assets/vanillaXml/forge.xml')
$recipe = [Recipe]::new($xml.SelectSingleNode('//Recipe[@Name="Simple"]'))
$flags = [Reflection.BindingFlags]'Instance,NonPublic'
$apply = [Recipe].GetMethod('TryOverrideDeviation',$flags)
$copy = [Recipe].GetMethod('CopyCandidateForItem',$flags)
$script:checks = 0
function Check([bool]$condition,[string]$message) { $script:checks++; if (!$condition) { throw $message } }
function Override([string]$kind,[int]$minimum,[int]$maximum) {
    $args = [object[]]@($kind,$minimum,$maximum,$null)
    $success = $apply.Invoke($recipe,$args)
    return @{ Success=$success; Lifetime=$args[3] }
}
function Aspect($perk) { return ($perk.Pairs | Where-Object Key -eq 'Aspect').Value }
$original = $recipe.Items[0]
$prices = @($recipe.Prices)
$source = $recipe.Variations[0].Enchantments[0]
$baseline = Aspect $source
$result = Override 'Weapon' 15 75
Check $result.Success 'Valid override failed.'
$first = $result.Lifetime
Check ($recipe.Items[0].MinDeviation -eq 15 -and $recipe.Items[0].MaxDeviation -eq 75) 'Effective item settings unchanged.'
Check ($original.MinDeviation -eq -30 -and $original.MaxDeviation -eq 30) 'Original item mutated.'
Check ($recipe.Items[0].PricesBlockName -eq $original.PricesBlockName -and $recipe.Items[0].EnchantmentsNumber -eq $original.EnchantmentsNumber -and $recipe.Items[0].BarScale -eq $original.BarScale) 'Unrelated item settings changed.'
Check ([object]::ReferenceEquals($prices[0],$recipe.Prices[0])) 'Price objects replaced.'
$projected = $copy.Invoke($recipe,[object[]]@($source,'Weapon'))
Check ((Aspect $projected) -eq '?RandomAspect[15,75]') 'Native candidate still uses embedded old deviation.'
Check ((Aspect $source) -eq $baseline) 'Native perk source mutated.'
Check ((Aspect ($copy.Invoke($recipe,[object[]]@($source,'Armor')))) -eq $baseline) 'Override leaked to armor.'
Check (!(Override 'Weapon' 0 1).Success) 'Duplicate override accepted.'
Check (!(Override 'Missing' 0 1).Success) 'Missing equipment accepted.'
Check (!(Override 'Armor' 9 1).Success) 'Reversed bounds accepted.'
Check (!(Override 'Armor' -10001 0).Success) 'Lower bound accepted.'
Check (!(Override 'Armor' 0 10001).Success) 'Upper bound accepted.'
$complex = [Recipe]::new($xml.SelectSingleNode('//Recipe[@Name="Complex"]'))
$complexArgs = [object[]]@('Weapon',15,75,$null)
Check (!$apply.Invoke($complex,$complexArgs) -and $null -eq $complexArgs[3]) 'Fixed-aspect recipe was converted to random power.'
foreach ($value in @('123','?Aspect[1]','?RandomAspect[-30,30]+1')) {
    [xml]$perkXml = "<Perk Name='fixed'><Set Aspect='$value' Other='preserve'/></Perk>"
    $perk = [PerkStruct]::new($perkXml.DocumentElement)
    $observed = $copy.Invoke($recipe,[object[]]@($perk,'Weapon'))
    Check ((Aspect $observed) -eq $value -and ($observed.Pairs | Where-Object Key -eq 'Other').Value -eq 'preserve') 'Fixed/compound aspect changed.'
}
$first.Dispose()
Check ([object]::ReferenceEquals($original,$recipe.Items[0])) 'Original item identity not restored.'
Check ((Aspect ($copy.Invoke($recipe,[object[]]@($source,'Weapon')))) -eq $baseline) 'Original candidate expression not restored.'
$next = Override 'Weapon' -5 5
$first.Dispose()
Check ($recipe.Items[0].MinDeviation -eq -5) 'Old lifetime removed newer override.'
$next.Lifetime.Dispose()
$next.Lifetime.Dispose()
Check ([object]::ReferenceEquals($original,$recipe.Items[0])) 'Repeated disposal changed restoration.'
Write-Output "PASS: $script:checks compiled native deviation projection/restoration checks; no gameplay roll or UI rendering claimed."
