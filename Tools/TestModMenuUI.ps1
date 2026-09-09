param(
    [string]$Unity = 'F:\UnityInstalls\2022.3.62f3\Editor\Unity.exe',
    [ValidatePattern('^[A-Za-z0-9_-]+$')][string]$FixtureName = 'ModMenuUISmoke'
)
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root ('Temp/' + $FixtureName)
# Reuse the established packaged-content fixture and its native boundary stubs.
if (!(Test-Path -LiteralPath (Join-Path $fixture 'Assets/LegacyModdingStubs.cs'))) {
    & (Join-Path $PSScriptRoot 'TestPackagedArt.ps1') -Unity $Unity -FixtureName $FixtureName
}
$manifestPath = Join-Path $fixture 'Packages/manifest.json'
$manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
$manifest.dependencies | Add-Member -NotePropertyName 'com.unity.ugui' -NotePropertyValue '1.0.0' -Force
$manifest | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath $manifestPath
Copy-Item -Path (Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding/*.cs') -Destination (Join-Path $fixture 'Assets/Scripts/Eclipse/Runtime/Modding') -Force
Copy-Item -Exclude ModModeRuntime.cs -Path (Join-Path $root 'Assets/Scripts/Eclipse/Modding/*.cs') -Destination (Join-Path $fixture 'Assets/Scripts/Eclipse/Modding') -Force
foreach ($name in @('TitleScreen', 'TitleScreenMods', 'TitleRibbon', 'GameSessionRestart', 'ReturnToTitleButton', 'BattleTouchControls')) {
    Copy-Item -LiteralPath (Join-Path $root "Assets/Scripts/Eclipse/UI/$name.cs") -Destination (Join-Path $fixture "Assets/$name.cs") -Force
}
foreach ($source in @('Assets/Scripts/Eclipse/Input/FightKeyBindings.cs', 'Assets/Scripts/Eclipse/Runtime/Presentation/SF2DisplayFrameRate.cs', 'Tools/ModMenuUIStubs.cs', 'Tools/ValidateModMenuUI.cs')) {
    Copy-Item -LiteralPath (Join-Path $root $source) -Destination (Join-Path $fixture 'Assets') -Force
}
$stubs = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'LegacyModdingStubs.cs') -Raw
$stubs.Replace('public static ListSF CCDKHLAMKKO() => Instance;', 'public static ListSF CCDKHLAMKKO() => Instance; public void GGGEHAGCLGC() { }') | Set-Content -LiteralPath (Join-Path $fixture 'Assets/LegacyModdingStubs.cs')
foreach ($name in @('example.phase1', 'example.phase2', 'example.phase3', 'example.weapon', 'example.loadout', 'example.enchantment')) {
    $destination = Join-Path $fixture "Assets/ModMenuFixtures/$name"
    New-Item -ItemType Directory -Path $destination -Force | Out-Null
    Copy-Item -Path (Join-Path $root "Mods/$name/*") -Destination $destination -Recurse -Force
}
$log = Join-Path $root 'Temp/mod-menu-ui.log'
$process = Start-Process -FilePath $Unity -ArgumentList @('-batchmode', '-projectPath', ('"' + $fixture + '"'), '-executeMethod', 'ValidateModMenuUI.Run', '-logFile', ('"' + $log + '"')) -WindowStyle Hidden -Wait -PassThru
$passed = Select-String -LiteralPath $log -Pattern '\[ModMenuUI\] PASS'
if ($process.ExitCode -ne 0 -or !$passed) {
    Select-String -LiteralPath $log -Pattern 'error CS|Exception|ModMenuUI' -Context 0,3
    throw "Mod menu UI validation failed: $log"
}
$passed.Line
