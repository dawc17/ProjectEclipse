$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
& (Join-Path $PSScriptRoot 'TestPhase1ShowcaseRuntime.ps1')
$fixture = Join-Path $root ('Temp/ModPackRules-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force -Path $fixture | Out-Null
. (Join-Path $PSScriptRoot '../Combat/ExportRoundOutcomeFixture.ps1')
Export-RoundOutcomeFixture $root $fixture
. (Join-Path $PSScriptRoot 'ExportModPackProjectionFixture.ps1')
Export-ModPackProjectionFixture $root $fixture
foreach ($source in @('Tools/Tests/Modding/ValidateModPackRules.cs','Tools/Tests/Modding/ModPackProjectionStubs.cs','Tools/Tests/Combat/RoundOutcomeNativeStubs.cs','Assets/Scripts/Eclipse/Modding/ModScriptSession.cs')) {
    Copy-Item -LiteralPath (Join-Path $root $source) -Destination $fixture
}
$production = [Security.SecurityElement]::Escape((Join-Path $root 'Temp/Phase1ShowcaseRuntime/bin/Debug/net10.0/Phase1ShowcaseRuntime.dll'))
$moon = [Security.SecurityElement]::Escape((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
<Reference Include="Phase1ShowcaseRuntime"><HintPath>$production</HintPath></Reference>
<Reference Include="MoonSharp.Interpreter"><HintPath>$moon</HintPath></Reference>
</ItemGroup></Project>
"@ | Set-Content -LiteralPath (Join-Path $fixture 'ModPackRules.csproj')
dotnet run --project (Join-Path $fixture 'ModPackRules.csproj') -- $fixture $root
if ($LASTEXITCODE -ne 0) { throw 'Mod pack rule acceptance failed.' }
