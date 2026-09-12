$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
& (Join-Path $PSScriptRoot 'TestPhase1ShowcaseRuntime.ps1')
$fixture = Join-Path $root ('Temp/ModModeWorkflow-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force -Path (Join-Path $fixture 'Mods') | Out-Null
Copy-Item -LiteralPath (Join-Path $root 'Mods/example.generated-expedition') -Destination (Join-Path $fixture 'Mods') -Recurse
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ValidateModModeWorkflow.cs') -Destination (Join-Path $fixture 'Program.cs')
$production = [Security.SecurityElement]::Escape((Join-Path $root 'Temp/Phase1ShowcaseRuntime/bin/Debug/net10.0/Phase1ShowcaseRuntime.dll'))
$moon = [Security.SecurityElement]::Escape((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
$modeRuntime = [Security.SecurityElement]::Escape((Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModModeRuntime.cs'))
$hostStubs = [Security.SecurityElement]::Escape((Join-Path $root 'Tools/Phase2HostStubs.cs'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
<Compile Include="$modeRuntime"/><Compile Include="$hostStubs"/>
<Reference Include="Phase1ShowcaseRuntime"><HintPath>$production</HintPath></Reference>
<Reference Include="MoonSharp.Interpreter"><HintPath>$moon</HintPath></Reference>
</ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 (Join-Path $fixture 'ModeWorkflow.csproj')
dotnet run --project (Join-Path $fixture 'ModeWorkflow.csproj') -- (Join-Path $fixture 'Mods') $root
if ($LASTEXITCODE -ne 0) { throw 'Mode workflow fixture failed.' }
