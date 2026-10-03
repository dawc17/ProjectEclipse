param([string]$Unity = '', [string]$ExistingFixture = '')
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
$versionFile = Join-Path $root 'ProjectSettings/ProjectVersion.txt'
$version = (Select-String -LiteralPath $versionFile -Pattern '^m_EditorVersion: (.+)$').Matches[0].Groups[1].Value
if (!$Unity) { $Unity = "F:\UnityInstalls\$version\Editor\Unity.exe" }
if (!(Test-Path -LiteralPath $Unity)) { throw 'Pass the matching Unity editor with -Unity.' }
$fixture = Join-Path $root ('Temp/FighterMotionUnity-' + [Guid]::NewGuid().ToString('N'))
if ($ExistingFixture) {
    $fixture = [IO.Path]::GetFullPath($ExistingFixture)
    $tempRoot = [IO.Path]::GetFullPath((Join-Path $root 'Temp')) + [IO.Path]::DirectorySeparatorChar
    if (!$fixture.StartsWith($tempRoot,[StringComparison]::OrdinalIgnoreCase) -or !(Test-Path -LiteralPath (Join-Path $fixture 'fighter-motion-fixture.marker'))) { throw 'Existing fixture must be a marked motion project inside repository Temp.' }
}
Write-Host "Full-game fighter motion fixture: $fixture"
foreach ($folder in @('Assets','Packages','ProjectSettings','Library/PackageCache')) {
    New-Item -ItemType Directory -Force -Path (Join-Path $fixture $folder) | Out-Null
    & robocopy (Join-Path $root $folder) (Join-Path $fixture $folder) /E /COPY:DAT /R:1 /W:1 /NFL /NDL /NJH /NJS | Out-Null
    if ($LASTEXITCODE -ge 8) { throw "Fixture copy failed: $folder" }
}
New-Item -ItemType Directory -Force -Path (Join-Path $fixture 'Mods') | Out-Null
foreach ($entry in @(@{Source=(Join-Path $root 'Mods/example.repulse');Id='example.repulse'},@{Source=(Join-Path $PSScriptRoot 'FighterMotionProbeMods/fixture.motion-probe');Id='fixture.motion-probe'},@{Source=(Join-Path $PSScriptRoot 'FighterMotionProbeMods/fixture.motion-peer');Id='fixture.motion-peer'})) {
    & robocopy $entry.Source (Join-Path $fixture ('Mods/'+$entry.Id)) /E /COPY:DAT /R:1 /W:1 /NFL /NDL /NJH /NJS | Out-Null
    if ($LASTEXITCODE -ge 8) { throw 'Mod fixture copy failed.' }
}
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'FighterMotionUnity.cs') -Destination (Join-Path $fixture 'Assets/Editor') -Force
[IO.File]::WriteAllText((Join-Path $fixture 'fighter-motion-fixture.marker'),'Isolated full-game native fighter motion acceptance')
$log = Join-Path $fixture ('validation-'+[Guid]::NewGuid().ToString('N')+'.log')
Write-Host "Native motion log: $log"
$runStarted = [DateTime]::UtcNow
$process = Start-Process -FilePath $Unity -ArgumentList @('-projectPath',('"'+$fixture+'"'),'-executeMethod','FighterMotionUnity.Run','-logFile',('"'+$log+'"')) -WindowStyle Hidden -PassThru
Write-Host "Native fighter motion process: $($process.Id)"
$deadline = [DateTime]::UtcNow.AddMinutes(15)
while (!$process.WaitForExit(20000)) {
    if ([DateTime]::UtcNow -gt $deadline) { $process.Kill(); throw "Native fighter motion timed out: $log" }
}
Select-String -LiteralPath $log -Pattern '\[FighterMotionUnity\]|\[MotionProbe\]|error CS' | ForEach-Object { Write-Host $_.Line }
$result = Join-Path $fixture 'validation-result.txt'
if ($process.ExitCode -ne 0 -or !(Test-Path -LiteralPath $result) -or (Get-Item -LiteralPath $result).LastWriteTimeUtc -lt $runStarted) { throw "Native fighter motion acceptance failed: $log" }
Get-Content -LiteralPath $result
