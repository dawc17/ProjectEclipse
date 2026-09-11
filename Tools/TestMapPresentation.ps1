param([string]$Unity = 'F:\UnityInstalls\2022.3.62f3\Editor\Unity.exe')
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root 'Temp/MapPresentationProject'
foreach ($dir in @('Assets/Resources/ui/atlases', 'Assets/Resources/ui/items', 'Assets/Resources/gamedata/video', 'Packages', 'ProjectSettings')) {
    New-Item -ItemType Directory -Force -Path (Join-Path $fixture $dir) | Out-Null
}
[IO.File]::WriteAllText((Join-Path $fixture 'Packages/manifest.json'), '{"dependencies":{"com.unity.ugui":"1.0.0","com.unity.modules.video":"1.0.0","com.unity.modules.audio":"1.0.0","com.unity.modules.imageconversion":"1.0.0"}}')
[IO.File]::WriteAllText((Join-Path $fixture 'ProjectSettings/ProjectVersion.txt'), "m_EditorVersion: 2022.3.62f3`n")
foreach ($file in @('Nekki/SF2/GUI/ResolutionImage.cs','Nekki/SF2/GUI/Map/DifficultyPanel.cs','Nekki/SF2/GUI/Menu/MenuMaterSprite.cs')) {
    Copy-Item -LiteralPath (Join-Path $root ('Assets/Scripts/Assembly-CSharp/' + $file)) -Destination (Join-Path $fixture 'Assets') -Force
}
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ValidateMapPresentation.cs') -Destination (Join-Path $fixture 'Assets') -Force
Copy-Item -Path (Join-Path $root 'Assets/Resources/ui/atlases/DifficultyBars*'), (Join-Path $root 'Assets/Resources/ui/atlases/ComboButtons*') -Destination (Join-Path $fixture 'Assets/Resources/ui/atlases') -Force
Copy-Item -Path (Join-Path $root 'Assets/Resources/ui/items/img_video.png*') -Destination (Join-Path $fixture 'Assets/Resources/ui/items') -Force
Copy-Item -Path (Join-Path $root 'Assets/Resources/gamedata/video/*.mp4*') -Destination (Join-Path $fixture 'Assets/Resources/gamedata/video') -Force
$log = Join-Path $root 'Temp/map-presentation.log'
$process = Start-Process -FilePath $Unity -ArgumentList @('-batchmode','-projectPath',('"' + $fixture + '"'),'-executeMethod','ValidateMapPresentation.Run','-logFile',('"' + $log + '"')) -WindowStyle Hidden -PassThru -Wait
if ($process.ExitCode -ne 0 -or !(Select-String -LiteralPath $log -Pattern '\[MapPresentation\] PASS')) {
    Get-Content -LiteralPath $log -Tail 65
    throw "Map presentation validation failed: $log"
}
(Select-String -LiteralPath $log -Pattern '\[MapPresentation\]').Line
