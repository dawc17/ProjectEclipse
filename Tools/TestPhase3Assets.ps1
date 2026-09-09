param(
    [string]$Unity = 'F:\UnityInstalls\2022.3.62f3\Editor\Unity.exe',
    [ValidatePattern('^[A-Za-z0-9_-]+$')][string]$FixtureName = 'Phase3AssetsSmoke'
)
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixtureRoot = Join-Path $root "Temp/$FixtureName"
if (!(Test-Path (Join-Path $fixtureRoot 'ProjectSettings/ProjectVersion.txt'))) {
    & (Join-Path $PSScriptRoot 'TestPackagedArt.ps1') -Unity $Unity -FixtureName $FixtureName
}
Copy-Item (Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding/*.cs') "$fixtureRoot/Assets/Scripts/Eclipse/Runtime/Modding" -Force
Copy-Item (Join-Path $root 'Assets/Scripts/Eclipse/Modding/*.cs') "$fixtureRoot/Assets/Scripts/Eclipse/Modding" -Exclude ModModeRuntime.cs -Force
foreach ($name in @('LegacyModdingStubs', 'ValidatePhase3Assets')) {
    Copy-Item (Join-Path $PSScriptRoot "$name.cs") "$fixtureRoot/Assets/$name.cs" -Force
}
Copy-Item (Join-Path $root 'Assets/Scripts/Eclipse/Content/PackagedArtCatalog.cs') "$fixtureRoot/Assets/PackagedArtCatalog.cs" -Force
foreach ($name in @('AtlasCache', 'LocationSpriteCache', 'ResourcesAndBundles')) {
    Copy-Item (Join-Path $root "Assets/Scripts/Assembly-CSharp/$name.cs") "$fixtureRoot/Assets/$name.cs" -Force
}
New-Item -ItemType Directory "$fixtureRoot/Assets/Phase3Mods" -Force | Out-Null
Copy-Item (Join-Path $root 'Mods/example.phase3') "$fixtureRoot/Assets/Phase3Mods" -Recurse -Force
$log = Join-Path $root 'Temp/phase3-assets.log'
$process = Start-Process -FilePath $Unity -ArgumentList @('-batchmode', '-nographics', '-projectPath', ('"' + $fixtureRoot + '"'),
    '-executeMethod', 'ValidatePhase3Assets.Run', '-logFile', ('"' + $log + '"')) -WindowStyle Hidden -Wait -PassThru
if ($process.ExitCode -ne 0 -or !(Select-String $log -Pattern '\[Phase3Assets\] PASS')) {
    Select-String $log -Pattern 'error CS|Exception|Phase3Assets' -Context 0,2
    throw "Phase 3 native asset validation failed: $log"
}
(Select-String $log -Pattern '\[Phase3Assets\] PASS').Line
