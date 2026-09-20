$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
& (Join-Path $PSScriptRoot 'TestPhase1ShowcaseRuntime.ps1')
$fixture = Join-Path $root ('Temp/SenseiStory-' + [Guid]::NewGuid().ToString('N'))
$content = Join-Path $fixture 'Mods/fixture.story/scripts/content'
New-Item -ItemType Directory -Force $content | Out-Null
Get-ChildItem (Join-Path $root 'Mods/de128/scripts/content/sensei_*.lua') | Copy-Item -Destination $content
Copy-Item (Join-Path $PSScriptRoot 'ValidateSenseiStory.cs') (Join-Path $fixture 'Program.cs')
$production = [Security.SecurityElement]::Escape((Join-Path $root 'Temp/Phase1ShowcaseRuntime/bin/Debug/net10.0/Phase1ShowcaseRuntime.dll'))
$moon = [Security.SecurityElement]::Escape((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
<Reference Include="Phase1ShowcaseRuntime"><HintPath>$production</HintPath></Reference>
<Reference Include="MoonSharp.Interpreter"><HintPath>$moon</HintPath></Reference>
</ItemGroup></Project>
"@ | Set-Content (Join-Path $fixture 'Story.csproj')
dotnet run --project (Join-Path $fixture 'Story.csproj') -- (Join-Path $fixture 'Mods') $root
if ($LASTEXITCODE -ne 0) { throw 'Sensei story integration failed.' }
