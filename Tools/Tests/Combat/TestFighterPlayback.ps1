$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
& (Join-Path $PSScriptRoot '../Modding/TestPhase1ShowcaseRuntime.ps1')
$fixture = Join-Path $root ('Temp/FighterPlayback-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force -Path $fixture | Out-Null
foreach ($file in @('FighterPlaybackTests.cs','FighterMotionNativeStubs.cs')) {
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot $file) -Destination $fixture
}
foreach ($file in @('FightFighterMotion.cs','FightFighterPlayback.cs')) {
    Copy-Item -LiteralPath (Join-Path $root ('Assets/Scripts/Eclipse/Modding/'+$file)) -Destination $fixture
}
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/BattleType.cs') -Destination $fixture
$production = [Security.SecurityElement]::Escape((Join-Path $root 'Temp/Phase1ShowcaseRuntime/bin/Debug/net10.0/Phase1ShowcaseRuntime.dll'))
$moon = [Security.SecurityElement]::Escape((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
<Reference Include="Phase1ShowcaseRuntime"><HintPath>$production</HintPath></Reference>
<Reference Include="MoonSharp.Interpreter"><HintPath>$moon</HintPath></Reference>
</ItemGroup></Project>
"@ | Set-Content -LiteralPath (Join-Path $fixture 'FighterPlayback.csproj')
dotnet run --project (Join-Path $fixture 'FighterPlayback.csproj') -- $fixture $root
if ($LASTEXITCODE -ne 0) { throw 'Fighter playback acceptance failed.' }
