$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
& (Join-Path $PSScriptRoot 'TestPhase1ShowcaseRuntime.ps1')
$fixture = Join-Path $root ('Temp/Phase3-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path (Join-Path $fixture 'Mods') | Out-Null
Copy-Item -LiteralPath (Join-Path $root 'Mods/example.phase3') -Destination (Join-Path $fixture 'Mods') -Recurse
$production = [Security.SecurityElement]::Escape((Join-Path $root 'Temp/Phase1ShowcaseRuntime/bin/Debug/net10.0/Phase1ShowcaseRuntime.dll'))
$moon = [Security.SecurityElement]::Escape((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
$testSource = [Security.SecurityElement]::Escape((Join-Path $PSScriptRoot 'Phase3RuntimeTests.cs'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
<Compile Include="$testSource"/>
<Reference Include="Phase1ShowcaseRuntime"><HintPath>$production</HintPath></Reference>
<Reference Include="MoonSharp.Interpreter"><HintPath>$moon</HintPath></Reference>
</ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 (Join-Path $fixture 'Phase3.csproj')
dotnet run --project (Join-Path $fixture 'Phase3.csproj') -- (Join-Path $fixture 'Mods')
if ($LASTEXITCODE -ne 0) { throw 'Phase 3 runtime contracts failed.' }
