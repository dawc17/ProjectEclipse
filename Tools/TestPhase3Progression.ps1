$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$production = Join-Path $root 'Temp/Phase1ShowcaseRuntime/bin/Debug/net10.0/Phase1ShowcaseRuntime.dll'
if (!(Test-Path $production)) { & (Join-Path $PSScriptRoot 'TestPhase1ShowcaseRuntime.ps1') }
$fixture = Join-Path $root ('Temp/P3Progression-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path (Join-Path $fixture 'Mods') | Out-Null
Copy-Item -LiteralPath (Join-Path $root 'Mods/example.phase3') -Destination (Join-Path $fixture 'Mods') -Recurse
$sources = @('UserAchievements', 'RosterAchievement', 'RosterAchievCounter', 'RepostAchievement', 'Achievement', 'AchievCounter') | ForEach-Object { Join-Path $root "Assets/Scripts/Assembly-CSharp/$_.cs" }
$sources += Join-Path $root 'Assets/Scripts/Eclipse/Modding/LegacyContentAdapterP3.cs'
$sources += Join-Path $PSScriptRoot 'Phase3ProgressionTests.cs'
$compile = ($sources | ForEach-Object { '<Compile Include="' + [Security.SecurityElement]::Escape($_) + '"/>' }) -join "`n"
$production = [Security.SecurityElement]::Escape($production)
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
$compile
<Reference Include="Phase1ShowcaseRuntime"><HintPath>$production</HintPath></Reference>
</ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 (Join-Path $fixture 'P3Progression.csproj')
dotnet run --project (Join-Path $fixture 'P3Progression.csproj') -- (Join-Path $fixture 'Mods')
if ($LASTEXITCODE -ne 0) { throw 'Phase 3 native progression fixture failed.' }
