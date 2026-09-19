$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root ('Temp/MoveCombatPatches-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$source = Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModAssetLoader.cs')
$marker = 'namespace Eclipse.Modding' + "`n" + '{' + "`n" + '    // Validate the complete batch first.'
$source = $source.Replace("`r`n", "`n")
$start = $source.IndexOf($marker)
if ($start -lt 0) { throw 'Production move patch runtime marker is missing.' }
$projection = 'using System; using System.Collections.Generic; using System.Xml;' + $source.Substring($start)
Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Runtime.cs') -Value $projection
$sources = @(Get-ChildItem -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding') -Filter '*.cs' -File | Select-Object -ExpandProperty FullName)
$sources += @(Join-Path $PSScriptRoot 'MoveCombatPatchTests.cs')
$sources += @(Join-Path $fixture 'Runtime.cs')
$includes = $sources | ForEach-Object { '<Compile Include="' + [Security.SecurityElement]::Escape($_) + '" />' }
$project = '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><EnableDefaultCompileItems>false</EnableDefaultCompileItems></PropertyGroup><ItemGroup>' + ($includes -join "`n") + '</ItemGroup></Project>'
Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj') -Value $project
Write-Output "Move combat patch fixture: $fixture"
dotnet run --project (Join-Path $fixture 'Fixture.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Move combat patch checks failed.' }
