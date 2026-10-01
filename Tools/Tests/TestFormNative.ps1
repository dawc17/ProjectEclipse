param([string]$Unity='F:\UnityInstalls\2022.3.62f3\Editor\Unity.exe')
$ErrorActionPreference='Stop'
$root=Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$fixture=Join-Path $root ('Temp/FormNative-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
foreach($folder in @('Assets','Packages','ProjectSettings')) {
    & robocopy (Join-Path $root $folder) (Join-Path $fixture $folder) /E /COPY:DAT /R:1 /W:1 /NFL /NDL /NJH /NJS | Out-Null
    if($LASTEXITCODE -ge 8){throw "Fixture copy failed: $folder"}
}
New-Item -ItemType Directory -Path (Join-Path $fixture 'Mods') | Out-Null
Copy-Item -LiteralPath (Join-Path $root 'Mods/example.shifting-guardian') -Destination (Join-Path $fixture 'Mods') -Recurse
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ValidateFormNative.cs') -Destination (Join-Path $fixture 'Assets/Editor')
[IO.File]::WriteAllText((Join-Path $fixture 'form-native-fixture.marker'),'Isolated native form acceptance fixture')
$log=Join-Path $fixture 'validation.log'
Write-Host "Native form fixture: $fixture"
$arguments=@('-batchmode','-projectPath',('"'+$fixture+'"'),'-executeMethod','ValidateFormNative.RunEditor','-logFile',('"'+$log+'"'))
$process=Start-Process -FilePath $Unity -ArgumentList $arguments -WindowStyle Hidden -PassThru
Write-Host "Native form process: $($process.Id)"
$process.WaitForExit()
Select-String -LiteralPath $log -Pattern '\[FormNative\]|error CS' | ForEach-Object {Write-Host $_.Line}
if($process.ExitCode -ne 0 -or !(Select-String -LiteralPath $log -Pattern '\[FormNative\] PASS:')){throw "Native form acceptance failed: $log"}
