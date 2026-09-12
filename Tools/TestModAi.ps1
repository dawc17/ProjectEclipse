$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
& (Join-Path $PSScriptRoot 'TestModAiEligibility.ps1')
& (Join-Path $PSScriptRoot 'TestModAiSnapshots.ps1')
& (Join-Path $PSScriptRoot 'TestPhase1ShowcaseRuntime.ps1')
$fixture = Join-Path $root ('Temp/ModAi-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force -Path (Join-Path $fixture 'Mods') | Out-Null
$nativeActions = Join-Path $fixture 'native-actions.json'
# Keep real compiled native types separate from the controlled eligibility fixtures.
& (Get-Process -Id $PID).Path -NoProfile -File (Join-Path $PSScriptRoot 'TestModAiNativeContent.ps1') -OutputPath $nativeActions
if ($LASTEXITCODE -ne 0) { throw 'Native AI content fixture failed.' }
Copy-Item -LiteralPath (Join-Path $root 'Mods/example.charge-ui') -Destination (Join-Path $fixture 'Mods') -Recurse
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ValidateModAi.cs') -Destination (Join-Path $fixture 'Program.cs')
$production = [Security.SecurityElement]::Escape((Join-Path $root 'Temp/Phase1ShowcaseRuntime/bin/Debug/net10.0/Phase1ShowcaseRuntime.dll'))
$moon = [Security.SecurityElement]::Escape((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
<Reference Include="Phase1ShowcaseRuntime"><HintPath>$production</HintPath></Reference>
<Reference Include="MoonSharp.Interpreter"><HintPath>$moon</HintPath></Reference>
</ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 (Join-Path $fixture 'Ai.csproj')
dotnet run --project (Join-Path $fixture 'Ai.csproj') -- (Join-Path $fixture 'Mods') $root $nativeActions
if ($LASTEXITCODE -ne 0) { throw 'Lua AI fixture failed.' }
