# Compact moves preserve approved native XML through the real Lua binding.
# Legacy declarations and malformed compact tables are rejected.
# No game save or live fight.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
dotnet build (Join-Path $root 'Assembly-CSharp.csproj') -v:q /clp:ErrorsOnly
if ($LASTEXITCODE -ne 0) { throw 'Managed build failed.' }
. (Join-Path $PSScriptRoot 'LoadUnityManagedAssemblies.ps1')
$null = [Reflection.Assembly]::LoadFrom((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
$null = [MoonSharp.Interpreter.Script]::DefaultOptions
$null = Import-SF2ManagedRuntime $root
$fixture = Join-Path $root ('Temp/MoveShortForm-' + [Guid]::NewGuid().ToString('N'))
$package = Join-Path $fixture 'Mods/fixture.short'
$null = New-Item -ItemType Directory -Force (Join-Path $package 'assets/animations'), (Join-Path $package 'scripts')
Copy-Item -LiteralPath (Join-Path $root 'Mods/de128/assets/animations/boss_war_ability.bytes') -Destination (Join-Path $package 'assets/animations/clip.bytes')
@'
schema = 1
id = "fixture.short"
name = "Move short form checks"
version = "1.0.0"
authors = ["Eclipse tests"]
entrypoint = "scripts/main.lua"
capabilities = ["content.register", "content.patch"]
'@ | Set-Content (Join-Path $package 'mod.toml')
'return' | Set-Content (Join-Path $package 'scripts/main.lua')
$mod = [Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $fixture 'Mods')).Mods[0]
if ($null -eq $mod) { throw 'Fixture discovery failed.' }
$flags = [Reflection.BindingFlags]'Instance,NonPublic'
$script:checks = 0
function Check([bool]$ok, [string]$message) { if (!$ok) { throw $message }; $script:checks++ }
function Project([string]$lua) {
    Set-Content -LiteralPath (Join-Path $package 'scripts/main.lua') -Value ('local sf2=require("sf2")' + "`n" + $lua)
    $catalog = [Eclipse.Modding.ModContentCatalog]::new()
    $tx = $catalog.BeginRegistration($mod); $context = $null
    try {
        $assets = [Eclipse.Modding.AssetResolver]::new([Eclipse.Modding.IAssetProvider[]]@([Eclipse.Modding.LooseModProvider]::new($mod)))
        $api = [Eclipse.Modding.ModApiFacade]::new($mod, $assets, $tx, $null)
        $context = [Eclipse.Modding.MoonSharpScriptRuntime]::new().CreateContext($mod, $api)
        $null = $context.ExecuteEntrypoint(); $null = $tx.Commit()
    } finally { if ($null -ne $context) { $context.Dispose() }; $tx.Dispose() }
    $null = $catalog.Freeze()
    $adapter = [Eclipse.Modding.LegacyContentAdapter]::new($catalog)
    return [Eclipse.Modding.LegacyContentAdapter].GetMethod('BuildMovesDocument', $flags).Invoke($adapter, @()).OuterXml
}
function MoveLua([string]$fields) { return 'sf2.moves.register { id = "m", animation = sf2.assets.binary("animations/clip"), ' + $fields + ' }' }
function Rejects([string]$short, [string]$pattern, [string]$what) {
    $failure = $null
    try { $null = Project (MoveLua $short) } catch { $failure = $_.Exception.ToString() }
    Check ($null -ne $failure) "Accepted malformed short form: $what"
    Check ($failure -match $pattern) "Unexpected rejection for $what`: $failure"
}

try {
    # Approved native XML captured before removal of the old parser.
    foreach ($case in (Get-Content (Join-Path $PSScriptRoot 'MoveShortFormProjections.json') -Raw | ConvertFrom-Json)) {
        Check ((Project (MoveLua $case.lua)) -eq $case.xml) ("Native projection changed: " + $case.name)
    }
    Rejects 'conditions = { { type = "mod_exists", name = "Stun" } }' 'kind|type' 'legacy condition'
    Rejects 'events = { { type = "interval_end", name = "Uninterrupt" } }' 'kind|type' 'legacy event'
    Rejects 'align = { axes = { "X" }, pivot = { object = "Pivot" }, position = { pivot = true } }' 'kind|object' 'legacy point'
    Rejects 'intervals = { { name = "Uninterrupt", start = 1 } }' 'start|from' 'legacy interval'
    Rejects 'intervals = { { type = "Attack", attack = { edges = { "EChest" }, damage_terms = { { type = "WeaponDamage" } } } } }' 'field|damage' 'legacy damage terms'
    Rejects 'tactic_distance = { axis = "X", from = { pivot = true }, to = { pivot = true } }' 'axis|distance' 'legacy tactic distance'
    Rejects 'actions = { { type = "try_on_end", frame = 1 } }' 'actions|timeline' 'legacy scheduled actions'
    # Malformed short tables.
    Check ((Project 'local t = { [1] = { sound = "a" }, [3] = { sound = "b" } }; t[1] = nil; sf2.moves.register { id = "m", animation = "animations/clip", timeline = t }') -eq
        (Project (MoveLua 'timeline = { [3] = { sound = "b" } }'))) 'Deleted timeline keys must not leave actions behind.'
    Rejects 'conditions = { { keys = { [1] = "Punch", [3] = "Kick" } } }' 'dense|consecutive|array' 'hole in short key sequence'
    Rejects 'conditions = { { keys = { "Punch", extra = "Kick" } } }' 'dense|consecutive|array' 'named field in short key sequence'
    Rejects 'events = { [1] = "birth", [3] = "animation_end" }' 'dense|consecutive|array' 'hole in short event array'
    Rejects 'events = { "birth", extra = "animation_end" }' 'dense|consecutive|array' 'named field in short event array'
    Rejects 'conditions = { { not_mod = "Stun", stage = "Fight" } }' 'names two kinds' 'two condition kinds'
    Rejects 'conditions = { { shoe = "Stun" } }' 'needs one kind key' 'unknown condition kind'
    Rejects 'conditions = { { mod = "Stun", press = "Tap" } }' 'press' 'field from another kind'
    Rejects 'conditions = { { controllable = false } }' 'must be true' 'controllable false'
    Rejects 'align = { axes = { "X" }, pivot = { pivot = "Me", player = "Enemy" }, position = { pivot = "Me" } }' 'player twice' 'player given twice'
    Rejects 'align = { axes = { "X" }, pivot = { node = true }, position = { pivot = "Me" } }' 'non-empty string' 'node without a name'
    Rejects 'intervals = { { name = "Uninterrupt", from = 1, ["end"] = 4 } }' 'end|to' 'mixed interval bounds'
    Rejects 'timeline = { [1.5] = { sound = "a" } }' 'whole numbers' 'fractional frame'
    Rejects 'timeline = { strik = { sound = "a" } }' 'unsupported key' 'misspelled event'
    Rejects 'timeline = { [1] = { sound = "a", stop_sound = "b" } }' 'names two kinds' 'two action kinds'
    Rejects 'timeline = { [1] = { sound = "a" } }, actions = { { type = "try_on_end", frame = 1 } }' 'actions|timeline' 'timeline and actions'
    Rejects 'events = "player"' 'controlled' 'unknown event preset'
    Rejects 'direction = "face_away"' 'face_enemy' 'unknown direction preset'
    Rejects 'intervals = { { type = "Attack", from = 1, attack = { edges = { "EChest" }, damage = 0.1, damage_terms = { Poison = 1 } } } }' 'Poison|damage' 'unknown damage term'
    Write-Output "PASS: $script:checks move short-form checks. Compact declarations preserve approved native XML; malformed short tables are rejected. No live fight."
} finally {
    Remove-Item -LiteralPath $fixture -Recurse -Force -ErrorAction SilentlyContinue
}
