$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
& (Join-Path $PSScriptRoot 'TestPhase1ShowcaseRuntime.ps1')
$fixture=Join-Path $root ('Temp/QuestSuppressionApi-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ValidateQuestSuppressionApi.cs') -Destination (Join-Path $fixture 'Program.cs')
$production=[Security.SecurityElement]::Escape((Join-Path $root 'Temp/Phase1ShowcaseRuntime/bin/Debug/net10.0/Phase1ShowcaseRuntime.dll'))
$moon=[Security.SecurityElement]::Escape((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
<Reference Include="Phase1ShowcaseRuntime"><HintPath>$production</HintPath></Reference>
<Reference Include="MoonSharp.Interpreter"><HintPath>$moon</HintPath></Reference>
</ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')
dotnet run --project (Join-Path $fixture 'Fixture.csproj') -- (Join-Path $fixture 'Mods') (Join-Path $root 'Assets/vanillaXml') (Join-Path $root 'Mods/example.quest-suppression/scripts/main.lua')
if ($LASTEXITCODE -ne 0) { throw 'Quest suppression API checks failed.' }
