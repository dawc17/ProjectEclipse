# Real Lua registration and battle-lock binding; no live map or player save.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
dotnet build (Join-Path $root 'Assembly-CSharp.csproj') -v:q /clp:ErrorsOnly
if ($LASTEXITCODE -ne 0) { throw 'Managed build failed.' }
. (Join-Path $PSScriptRoot 'LoadUnityManagedAssemblies.ps1')
$null = [Reflection.Assembly]::LoadFrom((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
$null = [MoonSharp.Interpreter.Script]::DefaultOptions
$null = Import-SF2ManagedRuntime $root
$fixture = Join-Path $root ('Temp/BattleLocks-' + [Guid]::NewGuid().ToString('N'))
$package = Join-Path $fixture 'Mods/fixture.locks'
$null = New-Item -ItemType Directory -Force (Join-Path $package 'scripts/content')
@'
schema = 1
id = "fixture.locks"
name = "Battle lock checks"
version = "1.0.0"
authors = ["Eclipse tests"]
entrypoint = "scripts/main.lua"
capabilities = ["content.register", "story.progression"]
[[dependencies]]
id = "core"
version = ">=1.0 <2.0"
'@ | Set-Content (Join-Path $package 'mod.toml')
'return' | Set-Content (Join-Path $package 'scripts/main.lua')
$mod = [Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $fixture 'Mods')).Mods[0]
if ($null -eq $mod) { throw 'Fixture discovery failed.' }
$script:checks = 0
function Check([bool]$ok, [string]$message) { if (!$ok) { throw $message }; $script:checks++ }
function Load-Lua([string]$scriptText) {
    Set-Content -LiteralPath (Join-Path $package 'scripts/main.lua') -Value ('local sf2=require("sf2")' + "`n" + $scriptText)
    $catalog = [Eclipse.Modding.ModContentCatalog]::new()
    $tx = $catalog.BeginRegistration($mod)
    $context = $null
    try {
        $assets = [Eclipse.Modding.AssetResolver]::new([Eclipse.Modding.IAssetProvider[]]@([Eclipse.Modding.LooseModProvider]::new($mod)))
        $api = [Eclipse.Modding.ModApiFacade]::new($mod,$assets,$tx,$null)
        $context = [Eclipse.Modding.MoonSharpScriptRuntime]::new().CreateContext($mod,$api)
        $context.ExecuteEntrypoint(); $tx.Commit()
    } catch {
        Check ($catalog.Battles.Count -eq 0) 'Failed Lua registration leaked battles.'
        throw
    } finally { if ($null -ne $context) {$context.Dispose()}; $tx.Dispose() }
    $catalog.Freeze()
    return $catalog
}
$script:calls=0
$script:accept=$true
$script:locked=$true
[Eclipse.Modding.ModBattleAccess]::SetLocked=[Func[Eclipse.Modding.DefinitionId,bool,bool]] {
    param($id,$locked)
    Check ($id.ToString() -ceq 'fixture.locks:battles/test') 'Wrong battle identity.'
    $script:calls++
    if($script:accept){$script:locked=$locked}
    return $script:accept
}
$prefix=@'
local zone=sf2.zones.register{id="test"}
local battle=sf2.battles.register{id="test",zone=zone,type=sf2.battles.STORY}
'@
try {
    [Eclipse.Modding.ModBattleAccess]::Reveal=[Eclipse.Modding.ModBattleAccess]::SetLocked
    [Eclipse.Modding.ModBattleAccess]::Focus=[Func[Eclipse.Modding.DefinitionId,bool]] {
        param($id)
        Check ($id.ToString() -ceq 'fixture.locks:battles/test') 'Wrong focus identity.'
        $script:calls++
        return $script:accept
    }
    $null=Load-Lua ($prefix+'assert(sf2.battles.set_locked(battle,false))')
    Check (!$script:locked -and $script:calls -eq 1) 'Accepted unlock did not reach host.'
    $null=Load-Lua ($prefix+'assert(sf2.battles.set_locked(battle,true))')
    Check $script:locked 'Relock did not reach host.'
    $script:accept=$false
    $null=Load-Lua ($prefix+'assert(not sf2.battles.set_locked(battle,false))')
    Check $script:locked 'Rejected change mutated host state.'
    $null=Load-Lua ($prefix+'assert(not sf2.battles.reveal(battle,false)); assert(not sf2.battles.focus(battle))')
    Check $script:locked 'Rejected reveal mutated host state.'
    $script:accept=$true
    $null=Load-Lua ($prefix+'assert(sf2.battles.reveal(battle,false)); assert(sf2.battles.focus(battle))')
    Check (!$script:locked) 'Reveal initial lock did not reach host.'
    foreach($call in @('sf2.battles.set_locked({},false)','sf2.battles.set_locked("core:battles/zone_1/tournament",false)',
        'sf2.battles.set_locked(zone,false)','sf2.battles.set_locked(battle,0)','sf2.battles.set_locked(battle,"false")','sf2.battles.set_locked(battle)',
        'sf2.battles.reveal({},false)','sf2.battles.reveal("core:battles/zone_1/tournament",false)',
        'sf2.battles.reveal(zone,false)','sf2.battles.reveal(battle,0)','sf2.battles.reveal(battle,"false")','sf2.battles.reveal(battle)',
        'sf2.battles.focus({})','sf2.battles.focus("core:battles/zone_1/tournament")','sf2.battles.focus(zone)')) {
        $before=$script:calls; $failed=$false
        try {$null=Load-Lua ($prefix+$call)}catch{$failed=$true}
        Check ($failed -and $before -eq $script:calls) ('Invalid request reached host: '+$call)
    }
    $saved=$mod; $manifest=Join-Path $package 'mod.toml'; $original=Get-Content -Raw $manifest
    $original.Replace(', "story.progression"','') | Set-Content $manifest
    $mod=[Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $fixture 'Mods')).Mods[0]
    foreach($call in @('sf2.battles.set_locked(battle,false)','sf2.battles.reveal(battle,false)','sf2.battles.focus(battle)')) {
        $before=$script:calls; $failed=$false
        try {$null=Load-Lua ($prefix+$call)}catch{$failed=$true}
        Check ($failed -and $before -eq $script:calls) 'Missing capability reached host.'
    }
    $mod=$saved; $original | Set-Content $manifest
    [Eclipse.Modding.ModBattleAccess]::Clear()
    foreach($call in @('sf2.battles.set_locked(battle,false)','sf2.battles.reveal(battle,false)','sf2.battles.focus(battle)')) {
        $failed=$false
        try {$null=Load-Lua ($prefix+$call)}catch{$failed=$true}
        Check $failed 'Missing host service silently accepted request.'
    }
} finally {[Eclipse.Modding.ModBattleAccess]::Clear()}
Write-Output "PASS: $script:checks Lua battle progression checks. Controlled host; native map verification is separate. Evidence: $fixture"
