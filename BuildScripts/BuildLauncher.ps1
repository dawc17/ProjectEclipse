param([string]$OutputDirectory = (Join-Path $PSScriptRoot 'out/Launcher'))
$ErrorActionPreference = 'Stop'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
if (!(Test-Path -LiteralPath $compiler)) { throw 'Install .NET Framework 4.8 to build the launcher.' }
New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null
$source = Join-Path (Split-Path -Parent $PSScriptRoot) 'Launcher'
$outputFile = Join-Path $OutputDirectory 'EclipseLauncher.exe'
& $compiler /nologo /target:winexe /platform:x64 /optimize+ "/out:$outputFile" /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Web.Extensions.dll /r:System.IO.Compression.dll /r:System.IO.Compression.FileSystem.dll (Join-Path $source 'Program.cs') (Join-Path $source 'UpdateCore.cs')
if ($LASTEXITCODE -ne 0) { throw 'Launcher compilation failed.' }
Write-Output "Launcher built: $OutputDirectory/EclipseLauncher.exe"
