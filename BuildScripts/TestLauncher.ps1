param([string]$PackageDirectory = '')
$ErrorActionPreference = 'Stop'
& "$PSScriptRoot/BuildLauncher.ps1"
$compiler = Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
$source = Join-Path (Split-Path -Parent $PSScriptRoot) 'Launcher'
$outputFile = Join-Path $PSScriptRoot 'out/Launcher/UpdateTests.exe'
& $compiler /nologo /target:exe "/out:$outputFile" /r:System.Web.Extensions.dll /r:System.IO.Compression.dll /r:System.IO.Compression.FileSystem.dll (Join-Path $source 'UpdateTests.cs') (Join-Path $source 'UpdateCore.cs')
if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed.' }
$testRoot = Join-Path $PSScriptRoot ('out/Launcher/test-' + [Guid]::NewGuid().ToString('N'))
if ($PackageDirectory) { & $outputFile $testRoot $PackageDirectory } else { & $outputFile $testRoot }
if ($LASTEXITCODE -ne 0) { throw 'Launcher tests failed.' }
