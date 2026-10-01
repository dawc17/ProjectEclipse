$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
$fixture = Join-Path $root 'Temp/CampaignSaveChecks'
New-Item -ItemType Directory -Force $fixture | Out-Null
$source = [Security.SecurityElement]::Escape((Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Presentation/CampaignSaveStore.cs'))
$tests = [Security.SecurityElement]::Escape((Join-Path $root 'Tools/Tests/Progression/ValidateCampaignSaves.cs'))
@"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><EnableDefaultCompileItems>false</EnableDefaultCompileItems></PropertyGroup>
  <ItemGroup><Compile Include="$source" /><Compile Include="$tests" /></ItemGroup>
</Project>
"@ | Set-Content -Encoding utf8 (Join-Path $fixture 'Checks.csproj')
dotnet run --project (Join-Path $fixture 'Checks.csproj') -- $fixture
if ($LASTEXITCODE -ne 0) { throw 'Campaign save checks failed.' }

# The native loader must remain stopped until a title selection is complete;
# a path lease is released only after the outgoing profile has been stopped.
$loader = Get-Content -Raw (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Nekki/SF2/GUI/Scenes/GameLoaderScene.cs')
$stop = $loader.IndexOf('Stop(); HOCNNFGOMHL = false;')
$arrive = $loader.IndexOf('GameSessionRestart.ArrivedAtTitle()')
if ($stop -lt 0 -or $arrive -lt 0 -or $stop -gt $arrive -or
    !$loader.Contains('if (Eclipse.UI.TitleScreen.IsOpen) return;')) { throw 'Title profile handoff guard changed.' }
$paths = Get-Content -Raw (Join-Path $root 'Assets/Scripts/Assembly-CSharp/SF2Paths.cs')
if (!$paths.Contains('CampaignSaveSession.UserDataDirectory ?? GetLegacyUserDataDirectory()')) { throw 'Native profile path is not routed through the selected campaign.' }
Write-Output 'PASS: native title loading gate and profile path routing.'
