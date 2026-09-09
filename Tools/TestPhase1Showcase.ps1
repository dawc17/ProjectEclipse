param(
    [string]$ProjectPath = (Split-Path -Parent $PSScriptRoot)
)

$ErrorActionPreference = 'Stop'
$mod = Join-Path $ProjectPath 'Mods/example.phase1'
$lua = Join-Path $mod 'scripts/main.lua'
$manifest = Join-Path $mod 'mod.toml'

function Require-Text([string]$text, [string]$needle, [string]$label) {
    if (-not $text.Contains($needle)) { throw "Missing showcase contract: $label ($needle)" }
}

if (-not (Test-Path $lua) -or -not (Test-Path $manifest)) { throw 'Phase 1 showcase mod is incomplete.' }
$source = Get-Content -Raw $lua
$toml = Get-Content -Raw $manifest

foreach ($needle in @(
    'sf2.state.register', 'sf2.localization.key',
    'sf2.items.register_consumable', 'sf2.itemsets.register', 'sf2.shop.set_availability', 'sf2.forge.register_recipe',
    'sf2.locales.register', 'sf2.locations.register', 'sf2.moves.register_template', 'sf2.moves.register',
    'sf2.moves.register_trigger', 'sf2.tactics.register',
    'sf2.zones.register', 'sf2.battles.register', 'sf2.warriors.register', 'sf2.rules.recharge_magic_each_round',
    'sf2.rewards.register', 'sf2.fights.register', 'sf2.quests.register'
)) { Require-Text $source $needle $needle }

foreach ($capability in @('content.register', 'content.patch', 'state.read', 'state.write')) {
    Require-Text $toml ('"' + $capability + '"') $capability
}

foreach ($asset in @(
    'assets/sprites/showcase.png',
    'assets/audio/showcase.wav',
    'assets/animations/showcase_step.bytes',
    'localizations/eng.toml',
    'localizations/pol.toml'
)) {
    if (-not (Test-Path (Join-Path $mod $asset))) { throw "Missing showcase asset: $asset" }
}

if ($source -match 'System\.Xml|Xml(Document|Node|Element)|<\s*(Zone|Battle|Fight|Warrior|Quest|Item|Perk)\b') {
    throw 'Showcase must not use raw XML or XML bridge objects.'
}
if ($source -cmatch '\bDE[_A-Za-z]|Definitive') { throw 'Showcase must not contain DE-specific runtime logic.' }
if ($source -match 'sf2\.progression\.replace_perk_branch|sf2\.localization\.patch|sf2\.price\.(coins|gems)') {
    throw 'Showcase unexpectedly mutates shared progression/localization or declares economy values.'
}

Write-Host 'Phase 1 showcase static contract PASS'
