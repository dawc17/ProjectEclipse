param([string]$Unity = '', [string]$ExistingFixture = '')
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
$versionFile = Join-Path $root 'ProjectSettings/ProjectVersion.txt'
$version = (Select-String -LiteralPath $versionFile -Pattern '^m_EditorVersion: (.+)$').Matches[0].Groups[1].Value
if (!$Unity) { $Unity = "F:\UnityInstalls\$version\Editor\Unity.exe" }
if (!(Test-Path -LiteralPath $Unity)) { throw 'Pass the matching Unity editor with -Unity.' }
$fixture = Join-Path $root ('Temp/AudioUnity-' + [Guid]::NewGuid().ToString('N'))
if ($ExistingFixture) {
    $fixture = [IO.Path]::GetFullPath($ExistingFixture)
    $tempRoot = [IO.Path]::GetFullPath((Join-Path $root 'Temp')) + [IO.Path]::DirectorySeparatorChar
    if (!$fixture.StartsWith($tempRoot,[StringComparison]::OrdinalIgnoreCase) -or !(Test-Path -LiteralPath (Join-Path $fixture 'audio-fixture.marker'))) { throw 'Existing fixture must be a marked audio project inside repository Temp.' }
}
Write-Host "Full-game audio fixture: $fixture"
foreach ($folder in @('Assets','Packages','ProjectSettings','Library/PackageCache')) {
    New-Item -ItemType Directory -Force -Path (Join-Path $fixture $folder) | Out-Null
    & robocopy (Join-Path $root $folder) (Join-Path $fixture $folder) /E /COPY:DAT /R:1 /W:1 /NFL /NDL /NJH /NJS | Out-Null
    if ($LASTEXITCODE -ge 8) { throw "Fixture copy failed: $folder" }
}
New-Item -ItemType Directory -Force -Path (Join-Path $fixture 'Mods') | Out-Null
& robocopy (Join-Path $root 'Mods/example.audio-lab') (Join-Path $fixture 'Mods/example.audio-lab') /E /COPY:DAT /R:1 /W:1 /NFL /NDL /NJH /NJS | Out-Null
if ($LASTEXITCODE -ge 8) { throw 'Mod fixture copy failed.' }

Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'AudioUnity.cs') -Destination (Join-Path $fixture 'Assets/Editor') -Force
[IO.File]::WriteAllText((Join-Path $fixture 'audio-fixture.marker'),'Isolated full-game native audio acceptance')
$log = Join-Path $fixture ('validation-'+[Guid]::NewGuid().ToString('N')+'.log')
Write-Host "Native audio log: $log"
$runStarted = [DateTime]::UtcNow
# Fresh profiles share only immutable hash-addressed TAR data on the project
# drive, avoiding a gigabyte of system-drive extraction on each acceptance run.
$audioProduct = 'AudioUnity-' + [Guid]::NewGuid().ToString('N')
$audioProfile = [IO.Path]::GetFullPath((Join-Path ([Environment]::GetFolderPath('LocalApplicationData')) ('../LocalLow/EclipseAcceptance/'+$audioProduct)))
$audioCache = Join-Path $audioProfile 'SF2DE/TarAssetCache'
$audioCacheTarget = Join-Path $fixture 'audio-tar-cache'
New-Item -ItemType Directory -Force -Path $audioCacheTarget | Out-Null
New-Item -ItemType Directory -Force -Path (Split-Path -Parent $audioCache) | Out-Null
New-Item -ItemType Junction -Path $audioCache -Target $audioCacheTarget | Out-Null
$process = Start-Process -FilePath $Unity -ArgumentList @('-projectPath',('"'+$fixture+'"'),'-executeMethod','AudioUnity.Run','-audioAcceptanceProductName',$audioProduct,'-logFile',('"'+$log+'"')) -WindowStyle Hidden -PassThru
Write-Host "Native audio process: $($process.Id)"
$deadline = [DateTime]::UtcNow.AddMinutes(15)
while (!$process.WaitForExit(20000)) {
    if ([DateTime]::UtcNow -gt $deadline) { $process.Kill(); throw "Native audio timed out: $log" }
}
Select-String -LiteralPath $log -Pattern '\[AudioUnity\]|error CS' | ForEach-Object { Write-Host $_.Line }
$result = Join-Path $fixture 'audio-result.txt'
if ($process.ExitCode -ne 0 -or !(Test-Path -LiteralPath $result) -or (Get-Item -LiteralPath $result).LastWriteTimeUtc -lt $runStarted) { throw "Native audio acceptance failed: $log" }
Get-Content -LiteralPath $result
