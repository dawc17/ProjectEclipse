$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
& (Join-Path $PSScriptRoot 'TestPhase1ShowcaseRuntime.ps1')
$fixture=Join-Path $root ('Temp/DojoSelection-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ValidateDojoSelection.cs') -Destination (Join-Path $fixture 'Program.cs')
$production=[Security.SecurityElement]::Escape((Join-Path $root 'Temp/Phase1ShowcaseRuntime/bin/Debug/net10.0/Phase1ShowcaseRuntime.dll'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
<Reference Include="Phase1ShowcaseRuntime"><HintPath>$production</HintPath></Reference>
</ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')
dotnet run --project (Join-Path $fixture 'Fixture.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Dojo selection checks failed.' }
