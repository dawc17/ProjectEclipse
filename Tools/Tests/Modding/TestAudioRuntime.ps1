$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
& (Join-Path $PSScriptRoot 'TestPhase1ShowcaseRuntime.ps1')
$fixture = Join-Path $root ('Temp/AudioRuntime-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force -Path $fixture | Out-Null
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'AudioRuntimeTests.cs') -Destination $fixture
$production = [Security.SecurityElement]::Escape((Join-Path $root 'Temp/Phase1ShowcaseRuntime/bin/Debug/net10.0/Phase1ShowcaseRuntime.dll'))
$moon = [Security.SecurityElement]::Escape((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
<Reference Include="Phase1ShowcaseRuntime"><HintPath>$production</HintPath></Reference>
<Reference Include="MoonSharp.Interpreter"><HintPath>$moon</HintPath></Reference>
</ItemGroup></Project>
"@ | Set-Content -LiteralPath (Join-Path $fixture 'AudioRuntime.csproj')
dotnet run --project (Join-Path $fixture 'AudioRuntime.csproj') -- $fixture $root
if ($LASTEXITCODE -ne 0) { throw 'Audio runtime acceptance failed.' }
