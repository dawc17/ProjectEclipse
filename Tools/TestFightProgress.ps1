# Real Lua registration and production XML projection; no live fight or player save.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
dotnet build (Join-Path $root 'Assembly-CSharp.csproj') -v:q /clp:ErrorsOnly
if ($LASTEXITCODE -ne 0) { throw 'Managed build failed.' }
. (Join-Path $PSScriptRoot 'LoadUnityManagedAssemblies.ps1')
$null = [Reflection.Assembly]::LoadFrom((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
$null = [MoonSharp.Interpreter.Script]::DefaultOptions
$null = Import-SF2ManagedRuntime $root
$fixture = Join-Path $root ('Temp/FightProgress-' + [Guid]::NewGuid().ToString('N'))
$package = Join-Path $fixture 'Mods/fixture.progress'
$null = New-Item -ItemType Directory -Force (Join-Path $package 'scripts/content')
Copy-Item -LiteralPath (Join-Path $root 'Mods/de128/scripts/content/sensei_progression.lua') -Destination (Join-Path $package 'scripts/content/sensei_progression.lua')
@'
schema = 1
id = "fixture.progress"
name = "Fight progress checks"
version = "1.0.0"
authors = ["Eclipse tests"]
entrypoint = "scripts/main.lua"
capabilities = ["content.register", "profile.read"]
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
        Check ($catalog.Rewards.Count -eq 0) 'Failed Lua registration leaked rewards.'
        throw
    } finally { if ($null -ne $context) {$context.Dispose()}; $tx.Dispose() }
    $catalog.Freeze()
    return $catalog
}
$script:reads=0
$script:wins=@{}
$script:losses=3
$script:present=$true
[Eclipse.Modding.ModProfileAccess]::Clear()
[Eclipse.Modding.ModProfileAccess]::Fight = [Func[Eclipse.Modding.DefinitionId,Eclipse.Modding.ModProfileFightSnapshot]] {
    param($id)
    $script:reads++
    if ($id.ToString().Contains('missing')) {throw [Eclipse.Modding.ModContentException]::new('Unknown fight.')}
    $wins=if($script:wins.ContainsKey($id.ToString())) {$script:wins[$id.ToString()]} else {0}
    return [Eclipse.Modding.ModProfileFightSnapshot]::new($script:present,$wins,$script:losses)
}
try {
    $script:wins['core:fights/zone_1/tournament/3']=7
    $null=Load-Lua @'
local a=sf2.profile.fight("core:fights/zone_1/tournament/3")
assert(a.present and a.wins==7 and a.losses==3)
a.wins=999
local b=sf2.profile.fight("core:fights/zone_1/tournament/3")
assert(a~=b and b.wins==7)
local zone=sf2.zones.register{id="test"}
local battle=sf2.battles.register{id="test",zone=zone,type=sf2.battles.STORY}
local opponent=sf2.warriors.register{id="opponent"}
local fight=sf2.fights.register{id="test",battle=battle,warriors={opponent}}
assert(sf2.profile.fight(fight).wins==0)
'@
    Check ($script:reads -eq 3) 'String/owned-handle queries did not reach the host.'
    foreach($bad in @('{}','false','"core:items/weapon/test"','"other:fights/test"','"unqualified"')) {
        $before=$script:reads; $failed=$false
        try {$null=Load-Lua ('sf2.profile.fight('+$bad+')')} catch {$failed=$true}
        Check ($failed -and $script:reads -eq $before) ('Invalid reference reached host: '+$bad)
    }
    $script:present=$false
    $null=Load-Lua 'local r=sf2.profile.fight("core:fights/zone_1/tournament/3"); assert(not r.present and r.wins==0 and r.losses==0)'
    Check $true 'Absent record did not return zeros.'
    $script:present=$true
    $failed=$false
    try {$null=Load-Lua 'sf2.profile.fight("core:fights/missing")'} catch {$failed=$true}
    Check $failed 'Unknown host fight was silently treated as unwon.'
    $saved=$mod
    $manifest=Join-Path $package 'mod.toml'
    $original=Get-Content -Raw $manifest
    $original.Replace(', "profile.read"','') | Set-Content $manifest
    $mod=[Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $fixture 'Mods')).Mods[0]
    $before=$script:reads; $failed=$false
    try {$null=Load-Lua 'sf2.profile.fight("core:fights/zone_1/tournament/3")'} catch {$failed=$true}
    Check ($failed -and $script:reads -eq $before) 'Missing capability reached host.'
    $mod=$saved; $original | Set-Content $manifest

    # Exhaust every combination of six tournament and five prior-act victories.
    # Derive the expected prerequisites from the historical quest conditions.
    [xml]$archive=Get-Content -Raw (Join-Path $root 'Assets/DExml/quests.xml')
    $prerequisites=@()
    for($act=1;$act -le 6;$act++) {
        $quest=$archive.SelectSingleNode('/*/Quest[@Name="SenseiZone'+$act+'Notify"]')
        Check ($null -ne $quest) ('Missing source quest '+$act)
        Check ($quest.SelectNodes('Events/*').Count -eq 1 -and $null -ne $quest.SelectSingleNode('Events/FightEnd')) 'Changed source event gate.'
        $equals=$quest.SelectNodes('Conditions/Equal')
        Check ($equals.Count -eq 2 -and $quest.SelectNodes('Conditions/*').Count -eq $(if($act -eq 1){3}else{4})) 'Unhandled source condition.'
        Check ($equals[0].GetAttribute('Value1') -ceq '_$FightResult' -and $equals[0].GetAttribute('Value2') -ceq 'Win' -and !$equals[0].HasAttribute('Not')) 'Changed source outcome gate.'
        $flag=if($act -eq 1){'_ShownSenseiIntro'}else{'_Zone'+$act+'MemoriesOpened'}
        $value=if($act -eq 1){'0'}else{'1'}
        $negated=if($act -eq 1){''}else{'1'}
        Check ($equals[1].GetAttribute('Value1') -ceq $flag -and $equals[1].GetAttribute('Value2') -ceq $value -and $equals[1].GetAttribute('Not') -ceq $negated) 'Changed source opened gate.'
        $ids=@()
        foreach($condition in $quest.SelectNodes('Conditions/GreaterEqual')) {
            Check ($condition.GetAttribute('Value2') -eq '1') 'Changed source win threshold.'
            $match=[regex]::Match($condition.GetAttribute('Value1'),'\?Fight\[ZONE_(\d+)\|(Tournament|SENSEI_MEMORIES)\|3\]\.WinCount')
            Check $match.Success 'Unhandled source prerequisite.'
            $ids+=if($match.Groups[2].Value -eq 'Tournament') {'core:fights/zone_'+$match.Groups[1].Value+'/tournament/3'} else {'fixture.progress:fights/final_'+$match.Groups[1].Value}
        }
        $prerequisites+=,@($ids)
    }
    $lua=[Text.StringBuilder]::new()
    $null=$lua.AppendLine('local progression=require("content.sensei_progression"); local finals={}; for i=1,5 do finals[i]="fixture.progress:fights/final_"..i end')
    # One context per history state; each also checks all-open and non-win outcomes.
    for($mask=0;$mask -lt 2048;$mask++) {
        $script:wins=@{}
        for($bit=0;$bit -lt 11;$bit++) {
            $id=if($bit -lt 6) {'core:fights/zone_'+($bit+1)+'/tournament/3'} else {'fixture.progress:fights/final_'+($bit-5)}
            $script:wins[$id]=if(($mask -band (1 -shl $bit)) -ne 0) {1} else {0}
        }
        $expected=@()
        for($act=1;$act -le 6;$act++) {
            $ready=$true
            foreach($id in $prerequisites[$act-1]) {if($script:wins[$id] -lt 1){$ready=$false}}
            if($ready){$expected+=$act}
        }
        $scriptText=$lua.ToString()+'local r=progression.find_unlocks({outcome="win",fight="unrelated"},{},finals); assert(table.concat(r,",")=="'+($expected -join ',')+'")'+"`n"+@'
assert(#progression.find_unlocks({outcome="win"},{true,true,true,true,true,true},finals)==0)
for _,outcome in ipairs({"loss","surrender","raid_timeout","raid_round_timeout"}) do
    assert(#progression.find_unlocks({outcome=outcome},{},finals)==0)
end
'@
        $null=Load-Lua $scriptText
        Check $true ('Unlock decision mismatch for history mask '+$mask)
    }
    [Eclipse.Modding.ModProfileAccess]::Clear()
    Check ($null -eq [Eclipse.Modding.ModProfileAccess]::Fight) 'Host cleanup retained fight service.'
    $failed=$false
    try {$null=Load-Lua 'sf2.profile.fight("core:fights/zone_1/tournament/3")'} catch {$failed=$true}
    Check $failed 'Unavailable profile returned fabricated progress.'
} finally {[Eclipse.Modding.ModProfileAccess]::Clear()}
Write-Output "PASS: $script:checks fight-progress and archived Sensei unlock checks; 2048 prerequisite histories. Controlled host only. Evidence: $fixture"
