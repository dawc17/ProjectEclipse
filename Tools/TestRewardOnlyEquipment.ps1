$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root ('Temp/RewardOnlyEquipment-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture -Force | Out-Null
$adapter = Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Modding/LegacyContentAdapter.cs')
$methods = foreach ($name in @('ApplyItems', 'ExternalEquipment', 'BuildItemNode', 'RemoveItems',
    'ApplyLocalization', 'RemoveLocalization', 'OnLanguageChanged', 'Set', 'ThrowIfDisposed')) {
    $match = [regex]::Match($adapter, '(?ms)^        (?:public|private) [^\r\n]*\b' + $name + '\(.*?^        \}')
    if (!$match.Success) { throw "Cannot extract production adapter method: $name" }
    $match.Value
}
$projection = 'using System; using System.Collections.Generic; using System.Globalization; using System.Xml;' +
    'namespace Eclipse.Modding { public sealed partial class LegacyContentAdapter {' + ($methods -join "`n") + '}}'
Set-Content -LiteralPath (Join-Path $fixture 'Projection.cs') -Encoding UTF8 -Value $projection
$sources = @(Get-ChildItem -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding') -Filter '*.cs' -File |
    Select-Object -ExpandProperty FullName)
$sources += @(Join-Path $PSScriptRoot 'RewardOnlyEquipmentTests.cs')
$sources += @(Join-Path $fixture 'Projection.cs')
$includes = $sources | ForEach-Object { '<Compile Include="' + [Security.SecurityElement]::Escape($_) + '" />' }
$project = '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType>' +
    '<TargetFramework>net10.0</TargetFramework><EnableDefaultCompileItems>false</EnableDefaultCompileItems>' +
    '</PropertyGroup><ItemGroup>' + ($includes -join "`n") + '</ItemGroup></Project>'
Set-Content -LiteralPath (Join-Path $fixture 'RewardOnlyEquipment.csproj') -Encoding UTF8 -Value $project
Write-Output "Reward-only equipment fixture: $fixture"
dotnet run --project (Join-Path $fixture 'RewardOnlyEquipment.csproj') -- $root
if ($LASTEXITCODE -ne 0) { throw 'Reward-only equipment adapter checks failed.' }
