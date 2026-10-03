param([string]$AssemblyDirectory = '', [string]$UnityManagedDirectory = '')
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
. (Join-Path $PSScriptRoot '../Shared/LoadUnityManagedAssemblies.ps1')
$assembly = Import-SF2ManagedRuntime $root $AssemblyDirectory $UnityManagedDirectory
$checks = 0
function Check($ok, $label) { if (!$ok) { throw $label }; $script:checks++ }
function Near($a, $b, $label) { Check ([Math]::Abs($a-$b) -lt .000002) $label }
$rules = (New-Object Eclipse.Multiplayer.Balance.PvpBalanceProfile).Compile()
$model = New-Object ModelParameters
$model.MaxLife = 1
$model.GFNCMLFKBGP(1)
for ($i=0; $i -lt 100; $i++) {
    $before = $model.RemainingHealthInDamageUnits
    $damage = [Eclipse.Multiplayer.Balance.PvpRecoverableHealth]::ClampBlockedDamage(.23, $before, $model.MaxLife, $rules.MinimumLifeOnBlock)
    $overkill = $false
    $resolved = $model.ResolveStrikeDamage($damage, [ref]$overkill)
    Check (!$overkill) 'Blocked hit passed native overkill margin'
    $model.GEACPINOAAN(-$resolved)
    Check (!$model.OJMIFOAHKBK()) 'Native health setter killed a blocking fighter'
    $model.RecoverableLife = [Eclipse.Multiplayer.Balance.PvpRecoverableHealth]::PoolAfterDamage($model.RecoverableLife, $model.RemainingHealthInDamageUnits, 1, $before-$model.RemainingHealthInDamageUnits, $true, $rules)
}
Near $model.RemainingHealthInDamageUnits .0001 'Native blocking floor'
$clone = $model.Clone()
Near $clone.RecoverableLife $model.RecoverableLife 'Model clone preserves grey pool'
[Reflection.Assembly]::LoadFrom((Join-Path $root 'Library/ScriptAssemblies/UnityEngine.UI.dll')) | Out-Null
$policy = [Activator]::CreateInstance($assembly.GetType('Eclipse.Multiplayer.Rollback.FightSnapshotPolicy'), $true)
$snapshotter = [Eclipse.Multiplayer.Online.ObjectGraphSnapshotter]::new($policy, $null)
$snapshot = New-Object Eclipse.Multiplayer.Online.StateSnapshot
$roots = [Collections.Generic.List[object]]::new()
$roots.Add($model)
$snapshotter.Capture($snapshot, 1, $roots, $null)
$beforePool = $model.RecoverableLife
$model.RecoverableLife = .1
$snapshotter.Restore($snapshot)
Near $model.RecoverableLife $beforePool 'Rollback graph restores native recoverable pool'
$state = New-Object Eclipse.Multiplayer.VersusSnapshot
$fighter = New-Object Eclipse.Multiplayer.FighterSnapshot
$fighter.Present = $true
$fighter.Life = .5
$state.Left = $fighter
$beforeHash = [Eclipse.Multiplayer.VersusStateHash]::Hash([ref]$state)
$fighter.RecoverableLife = .2
$state.Left = $fighter
Check ([Eclipse.Multiplayer.VersusStateHash]::Hash([ref]$state) -ne $beforeHash) 'Grey health missing from simulation hash'
$model.BCLGFKDDNKH()
Near $model.RecoverableLife 0 'Round reset clears grey pool'
$model.GFNCMLFKBGP(.5)
$model.RecoverableLife = .5
$model.GFNCMLFKBGP(.8)
Near $model.RecoverableLife .2 'External healing clamps grey pool'
$model.GEACPINOAAN(-1)
Check ($model.OJMIFOAHKBK()) 'Direct hit remains lethal'
Near $model.RecoverableLife 0 'Death clears grey pool'
Write-Output "PASS: $checks compiled native-health and rollback checks (no Unity editor or game playtest)."
