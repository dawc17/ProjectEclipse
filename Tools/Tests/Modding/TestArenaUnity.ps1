param([string]$Unity = '', [string]$ExistingFixture = '')
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
$versionFile = Join-Path $root 'ProjectSettings/ProjectVersion.txt'
$version = (Select-String -LiteralPath $versionFile -Pattern '^m_EditorVersion: (.+)$').Matches[0].Groups[1].Value
if (!$Unity) { $Unity = "F:\UnityInstalls\$version\Editor\Unity.exe" }
if (!(Test-Path -LiteralPath $Unity)) { throw 'Pass the matching Unity editor with -Unity.' }
$fixture = Join-Path $root ('Temp/ArenaUnity-' + [Guid]::NewGuid().ToString('N'))
if ($ExistingFixture) {
    $fixture = [IO.Path]::GetFullPath($ExistingFixture)
    $tempRoot = [IO.Path]::GetFullPath((Join-Path $root 'Temp')) + [IO.Path]::DirectorySeparatorChar
    if (!$fixture.StartsWith($tempRoot,[StringComparison]::OrdinalIgnoreCase) -or !((Test-Path -LiteralPath (Join-Path $fixture 'arena-fixture.marker')) -or (Test-Path -LiteralPath (Join-Path $fixture 'audio-fixture.marker')))) { throw 'Existing fixture must be a marked arena project inside repository Temp.' }
}
Write-Host "Full-game arena fixture: $fixture"
foreach ($folder in @('Assets','Packages','ProjectSettings','Library/PackageCache')) {
    New-Item -ItemType Directory -Force -Path (Join-Path $fixture $folder) | Out-Null
    & robocopy (Join-Path $root $folder) (Join-Path $fixture $folder) /E /COPY:DAT /R:1 /W:1 /NFL /NDL /NJH /NJS | Out-Null
    if ($LASTEXITCODE -ge 8) { throw "Fixture copy failed: $folder" }
}
New-Item -ItemType Directory -Force -Path (Join-Path $fixture 'Mods') | Out-Null
& robocopy (Join-Path $root 'Mods/example.pulse-arena') (Join-Path $fixture 'Mods/example.pulse-arena') /E /COPY:DAT /R:1 /W:1 /NFL /NDL /NJH /NJS | Out-Null
if ($LASTEXITCODE -ge 8) { throw 'Mod fixture copy failed.' }

Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ArenaUnity.cs') -Destination (Join-Path $fixture 'Assets/Editor') -Force
[IO.File]::WriteAllText((Join-Path $fixture 'arena-fixture.marker'),'Isolated full-game native arena acceptance')
$log = Join-Path $fixture ('validation-'+[Guid]::NewGuid().ToString('N')+'.log')
Write-Host "Native arena log: $log"
$runStarted = [DateTime]::UtcNow
# Fresh profiles share only immutable hash-addressed TAR data on the project
# drive, avoiding a gigabyte of system-drive extraction on each acceptance run.
$arenaProduct = 'ArenaUnity-' + [Guid]::NewGuid().ToString('N')
$arenaProfile = [IO.Path]::GetFullPath((Join-Path ([Environment]::GetFolderPath('LocalApplicationData')) ('../LocalLow/EclipseAcceptance/'+$arenaProduct)))
$arenaCache = Join-Path $arenaProfile 'SF2DE/TarAssetCache'
$arenaCacheTarget = Join-Path $fixture 'audio-tar-cache'
New-Item -ItemType Directory -Force -Path $arenaCacheTarget | Out-Null
New-Item -ItemType Directory -Force -Path (Split-Path -Parent $arenaCache) | Out-Null
New-Item -ItemType Junction -Path $arenaCache -Target $arenaCacheTarget | Out-Null
$process = Start-Process -FilePath $Unity -ArgumentList @('-projectPath',('"'+$fixture+'"'),'-executeMethod','ArenaUnity.Run','-arenaAcceptanceProductName',$arenaProduct,'-logFile',('"'+$log+'"')) -WindowStyle Hidden -PassThru
Write-Host "Native arena process: $($process.Id)"
$deadline = [DateTime]::UtcNow.AddMinutes(15)
while (!$process.WaitForExit(20000)) {
    if ([DateTime]::UtcNow -gt $deadline) { $process.Kill(); throw "Native arena timed out: $log" }
}
Select-String -LiteralPath $log -Pattern '\[ArenaUnity\]|error CS' | ForEach-Object { Write-Host $_.Line }
$result = Join-Path $fixture 'arena-result.txt'
if ($process.ExitCode -ne 0 -or !(Test-Path -LiteralPath $result) -or (Get-Item -LiteralPath $result).LastWriteTimeUtc -lt $runStarted) { throw "Native arena acceptance failed: $log" }
Get-Content -LiteralPath $result
