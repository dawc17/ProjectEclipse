# Exercise the compiled production adapter, reward parser and health pool.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
. (Join-Path $PSScriptRoot 'LoadUnityManagedAssemblies.ps1')
$null = Import-SF2ManagedRuntime $root
function New-Definition([Type]$type, [hashtable]$values) {
    $ctor = $type.GetConstructors([Reflection.BindingFlags]'Instance,NonPublic')[0]
    $args = @($ctor.GetParameters() | ForEach-Object {
        if ($values.ContainsKey($_.Name)) { $values[$_.Name] }
        elseif ($_.ParameterType.IsValueType) { [Activator]::CreateInstance($_.ParameterType) }
        else { $null }
    })
    return $ctor.Invoke($args)
}
$catalog = [Eclipse.Modding.ModContentCatalog]::new()
$catalog.Freeze()
$adapter = [Eclipse.Modding.LegacyContentAdapter]::new($catalog)
$warrior = New-Definition ([Eclipse.Modding.WarriorDefinition]) @{ healthBars = 10; tactic = 'Standard' }
$reward = New-Definition ([Eclipse.Modding.RewardDefinition]) @{ gems = 25 }
$flags = [Reflection.BindingFlags]'Instance,NonPublic'
$document = [Xml.XmlDocument]::new()
$node = $adapter.GetType().GetMethod('BuildWarriorNode', $flags).Invoke($adapter, @($document, $warrior))
if ($node.GetAttribute('ShieldTotal') -ne '10' -or $node.GetAttribute('Tactic') -ne 'Standard') { throw 'Raid warrior adapter lost pool/tactic.' }
$boss = [ModelParameters]::new()
$boss.ShieldTotal = [int]$node.GetAttribute('ShieldTotal')
$boss.CIDCNCDFONA = 1
$boss.GFNCMLFKBGP(1)
$boss.GEACPINOAAN(-1.25)
if ($boss.RemainingHealthBars -ne 9 -or [Math]::Abs($boss.CurrentHealthBarFraction - 0.75) -gt 0.0002 -or $boss.OJMIFOAHKBK()) { throw 'Cross-bar damage/count failed.' }
$boss.GEACPINOAAN(-8.75)
if ($boss.RemainingHealthBars -ne 0 -or !$boss.OJMIFOAHKBK()) { throw 'Exhausted boss did not die.' }
$node = $adapter.GetType().GetMethod('BuildRewardNode', $flags).Invoke($adapter, @($document, $reward))
if ($node.GetAttribute('Bonus') -ne '25') { throw 'Gem reward adapter failed.' }
$prize = [RewardPrize]::new()
$prize.Parse($node, 0, 0)
if ([long]$prize.PNDAIFALIKF -ne 25) { throw 'Recovered reward parser lost gems.' }
Write-Host 'PASS: production warrior/reward adapters, ten-bar pool carry/death, and recovered 25-gem reward parsing.'
