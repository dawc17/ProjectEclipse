$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fightSource = Get-Content -Raw -Encoding UTF8 (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Fight.cs')
$hitStart = $fightSource.IndexOf('public void OnModelHit(')
$outgoing = $fightSource.IndexOf('ModEffectEvent.DamageDealing', $hitStart)
$invulnerable = $fightSource.IndexOf('if (EGHPHELLOGO.KJDFJPBIGJC.IJINDLLEGKA())', $outgoing)
$shield = $fightSource.IndexOf('_eclipseShields.TryGetValue', $invulnerable)
$incoming = $fightSource.IndexOf('ModEffectEvent.DamageResolving', $shield)
$apply = $fightSource.IndexOf('UpdateLife(EGHPHELLOGO.KJDFJPBIGJC', $incoming)
if ($hitStart -lt 0 -or $outgoing -le $hitStart -or $invulnerable -le $outgoing -or $shield -le $invulnerable -or $incoming -le $shield -or $apply -le $incoming) {
    throw 'Hit ordering must be outgoing modifiers, invulnerability, shields, incoming modifiers, health application.'
}
& (Join-Path $PSScriptRoot 'TestPhase1ShowcaseRuntime.ps1')
$fixture = Join-Path $root ('Temp/BattleRules-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force -Path (Join-Path $fixture 'Mods') | Out-Null
Copy-Item -LiteralPath (Join-Path $root 'Mods/example.battle-rules') -Destination (Join-Path $fixture 'Mods') -Recurse
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ValidateBattleRules.cs') -Destination (Join-Path $fixture 'Program.cs')
$production = [Security.SecurityElement]::Escape((Join-Path $root 'Temp/Phase1ShowcaseRuntime/bin/Debug/net10.0/Phase1ShowcaseRuntime.dll'))
$moon = [Security.SecurityElement]::Escape((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
<Reference Include="Phase1ShowcaseRuntime"><HintPath>$production</HintPath></Reference>
<Reference Include="MoonSharp.Interpreter"><HintPath>$moon</HintPath></Reference>
</ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 (Join-Path $fixture 'BattleRules.csproj')
dotnet run --project (Join-Path $fixture 'BattleRules.csproj') -- (Join-Path $fixture 'Mods')
if ($LASTEXITCODE -ne 0) { throw 'Battle rule runtime fixture failed.' }
