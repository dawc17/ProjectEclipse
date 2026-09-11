$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root ('Temp/PerkUpgradeNative-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force -Path $fixture | Out-Null
$source = Get-Content -Raw -Encoding UTF8 (Join-Path $root 'Assets/Scripts/Assembly-CSharp/PerkInfoItem.cs')
$clone = [regex]::Match($source, '(?ms)^\tpublic PerkInfoItem Clone\(.*?^\t\}(?=\r?\n\r?\n\tpublic void OEBOFOCIPBH)').Value
if (!$clone) { throw 'Production PerkInfoItem.Clone method was not found.' }
$program = (Get-Content -Raw -Encoding UTF8 (Join-Path $PSScriptRoot 'ValidatePerkUpgradeNative.cs')).Replace('// CLONE_BODY', $clone)
Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Program.cs') -Value $program
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/PerkItems.cs') -Destination $fixture
'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>' | Set-Content -Encoding UTF8 (Join-Path $fixture 'Native.csproj')
dotnet run --project (Join-Path $fixture 'Native.csproj') -- $root
if ($LASTEXITCODE -ne 0) { throw 'Native perk upgrade fixture failed.' }
