$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
$fixture = Join-Path $root ('Temp/PvpBalance-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture -Force | Out-Null
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'PvpBalanceTests.cs') -Destination $fixture
$json = Get-ChildItem -Path (Join-Path $root 'Library/PackageCache/com.unity.nuget.newtonsoft-json*/Runtime/Newtonsoft.Json.dll') | Select-Object -First 1
if (!$json) { throw 'Import Unity packages first; Newtonsoft.Json is required.' }
$sources = @('Assets/Scripts/Eclipse/Runtime/PvpBalance.cs', 'Assets/Scripts/Eclipse/Multiplayer/PvpBalanceCombat.cs') | ForEach-Object {
    '<Compile Include="' + [Security.SecurityElement]::Escape((Join-Path $root $_)) + '" />'
}
$reference = [Security.SecurityElement]::Escape($json.FullName)
@"
<Project Sdk="Microsoft.NET.Sdk">
 <PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup>
 <ItemGroup>$sources<Reference Include="Newtonsoft.Json"><HintPath>$reference</HintPath></Reference></ItemGroup>
</Project>
"@ | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')
dotnet run --project (Join-Path $fixture 'Fixture.csproj')
if ($LASTEXITCODE -ne 0) { throw 'PvP balance regression failed.' }
