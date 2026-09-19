# Real Lua registration and production XML projection; no live fight or player save.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
dotnet build (Join-Path $root 'Assembly-CSharp.csproj') -v:q /clp:ErrorsOnly
if ($LASTEXITCODE -ne 0) { throw 'Managed build failed.' }
. (Join-Path $PSScriptRoot 'LoadUnityManagedAssemblies.ps1')
$null = [Reflection.Assembly]::LoadFrom((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
$null = [MoonSharp.Interpreter.Script]::DefaultOptions
$null = Import-SF2ManagedRuntime $root
$fixture = Join-Path $root ('Temp/SenseiRewards-' + [Guid]::NewGuid().ToString('N'))
$package = Join-Path $fixture 'Mods/fixture.rewards'
$null = New-Item -ItemType Directory -Force (Join-Path $package 'scripts/content')
Copy-Item -LiteralPath (Join-Path $root 'Mods/de128/scripts/content/sensei_rewards.lua') -Destination (Join-Path $package 'scripts/content/sensei_rewards.lua')
@'
schema = 1
id = "fixture.rewards"
name = "Reward economy checks"
version = "1.0.0"
authors = ["Eclipse tests"]
entrypoint = "scripts/main.lua"
capabilities = ["content.register"]
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
$flags = [Reflection.BindingFlags]'Instance,NonPublic'
function Project($catalog, [Eclipse.Modding.RewardDefinition]$reward) {
    $adapter = [Eclipse.Modding.LegacyContentAdapter]::new($catalog)
    return [Eclipse.Modding.LegacyContentAdapter].GetMethod('BuildRewardNode',$flags).Invoke($adapter,@([Xml.XmlDocument]::new(),$reward))
}
function Fingerprint($catalog) { return [Eclipse.Modding.ModSaveData]::ComputeContentSetFingerprint([Eclipse.Modding.ModDescriptor[]]@($mod),$catalog) }
function First-Reward($catalog) { return @($catalog.Rewards)[0] }
$bare='sf2.rewards.register{id="test",gems=8}'
$configured='sf2.rewards.register{id="test",gems=8,experience=2,prize_base=1}'
$plain=Load-Lua $bare
$config=Load-Lua $configured
Check ((Fingerprint $plain) -ceq (Fingerprint (Load-Lua $bare.Replace('gems=8','gems=8,experience=0')))) 'Explicit zero experience changes old fingerprint.'
Check ((Project $plain (First-Reward $plain)).OuterXml -ceq '<Reward Bonus="8" />') 'Existing reward projection changed.'
Check ((Fingerprint $config) -ceq (Fingerprint (Load-Lua $configured))) 'Non-deterministic reward fingerprint.'
foreach($changed in @($configured.Replace('experience=2','experience=3'),$configured.Replace('prize_base=1','prize_base=2'),$configured.Replace(',prize_base=1',''))) {
    Check ((Fingerprint $config) -cne (Fingerprint (Load-Lua $changed))) 'Economy change missing from fingerprint.'
}
Check ((Fingerprint $plain) -cne (Fingerprint (Load-Lua $bare.Replace('gems=8','gems=8,prize_base=0')))) 'Explicit prize-base zero lost.'
foreach($good in @('experience=0','experience=1000000','prize_base=0','prize_base=1000000','prize_base=0.5')) {
    $null=Load-Lua ('sf2.rewards.register{id="test",'+$good+'}')
    Check $true ('Rejected valid economy boundary '+$good)
}
foreach($bad in @('experience=-1','experience=1000001','experience=0.5','experience=1/0','experience=0/0','experience="2"','experience=false',
    'prize_base=-1','prize_base=1000001','prize_base=1/0','prize_base=0/0','prize_base="2"','prize_base=false','xp=2')) {
    $failed=$false
    try {$null=Load-Lua ('sf2.rewards.register{id="prior"}; sf2.rewards.register{id="test",'+$bad+'}')} catch {$failed=$true}
    Check $failed ('Accepted invalid economy '+$bad)
}
$culture=[Threading.Thread]::CurrentThread.CurrentCulture
try {
    [Threading.Thread]::CurrentThread.CurrentCulture=[Globalization.CultureInfo]::GetCultureInfo('nb-NO')
    $fraction=Load-Lua $configured.Replace('prize_base=1','prize_base=0.5')
    Check ((Project $fraction (First-Reward $fraction)).GetAttribute('PrizeBase') -ceq '0.5') 'Locale changed reward projection.'
    Check ((Fingerprint $config) -ceq (Fingerprint (Load-Lua $configured))) 'Locale changed reward fingerprint.'
} finally {[Threading.Thread]::CurrentThread.CurrentCulture=$culture}

# The runtime package reads only Lua. XML below is an independent test oracle.
$catalog=Load-Lua 'require("content.sensei_rewards")'
[xml]$archive=Get-Content -Raw (Join-Path $root 'Assets/DExml/stages.xml')
$count=0
foreach($zone in $archive.SelectNodes('/Stages/Zones/Zone')) {
    foreach($battle in $zone.SelectNodes('Battle[@Name="SENSEI_MEMORIES" or @Name="SENSEI_MEMORIES_ECLIPSEMODE"]')) {
        $act=$zone.GetAttribute('Name').Replace('ZONE_','')
        foreach($fight in $battle.SelectNodes('Fight')) {
            $wins=0
            foreach($expected in $fight.SelectNodes('Rewards/Reward')) {
                $suffix=if($battle.GetAttribute('Name') -eq 'SENSEI_MEMORIES') {'normal_'+$fight.GetAttribute('Name')+'_'+$wins} else {'eclipse_'+$wins}
                $id='fixture.rewards:rewards/sensei_act_'+$act+'_'+$suffix
                $reward=@($catalog.Rewards | Where-Object {$_.Id.ToString() -ceq $id})
                Check ($reward.Count -eq 1) ('Missing slot '+$id)
                $actual=Project $catalog $reward[0]
                Check ($expected.ChildNodes.Count -eq 0) ('Unexpected nested reward source '+$id)
                foreach($attribute in $expected.Attributes) { Check ($attribute.Name -in @('Money','Exp','Bonus','PrizeBase')) ('Unhandled reward field '+$attribute.Name) }
                Check ([long]$expected.GetAttribute('Money') -eq 0) ('Unhandled fixed coin reward '+$id)
                # Missing Money/Exp/Bonus are native zero defaults, not lost rewards.
                foreach($attribute in @('Money','Exp','Bonus')) {
                    Check ([long]$actual.GetAttribute($attribute) -eq [long]$expected.GetAttribute($attribute)) ('Archive scalar mismatch '+$id+' '+$attribute)
                }
                Check ($actual.GetAttribute('PrizeBase') -ceq $expected.GetAttribute('PrizeBase')) ('Archive prize base mismatch '+$id)
                # Exercise recovered Reward/RewardPrize parsing and denomination scaling.
                foreach($exponent in @(0,2)) {
                    $native=[Reward]::new($actual,0,$exponent).KOBOIFJNPMO(1)
                    $baseline=[Reward]::new($expected,0,$exponent).KOBOIFJNPMO(1)
                    Check ([uint32]$native.exp -eq [uint32]$baseline.exp -and [long]$native.PNDAIFALIKF -eq [long]$baseline.PNDAIFALIKF -and [float]$native.prizeBase -eq [float]$baseline.prizeBase) ('Native reward parser mismatch '+$id)
                }
                $actual.OuterXml | Set-Content (Join-Path $fixture ($id.Split('/')[-1]+'.xml'))
                $wins++; $count++
            }
        }
    }
}
Check ($count -eq 57 -and $catalog.Rewards.Count -eq 57) 'Lost or extra Sensei reward slots.'
Write-Output "PASS: $script:checks reward economy checks; all $count historical Sensei slots match native parsing. Evidence: $fixture. No profile settlement/playtest claim."
