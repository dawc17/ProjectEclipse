param([string]$Unity = 'F:\UnityInstalls\2022.3.62f3\Editor\Unity.exe', [switch]$WithPreview)
$ErrorActionPreference = 'Stop'
if (-not (Test-Path -LiteralPath $Unity)) { throw "Unity 2022.3.62f3 not found: $Unity" }
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root ('Temp/ModUiUnity-' + [Guid]::NewGuid().ToString('N'))
foreach ($directory in @('Assets/Editor','Assets/Plugins','Packages','ProjectSettings','Mods','FixtureData')) {
    New-Item -ItemType Directory -Force -Path (Join-Path $fixture $directory) | Out-Null
}
[IO.File]::WriteAllText((Join-Path $fixture 'Packages/manifest.json'), '{"dependencies":{"com.unity.ugui":"1.0.0","com.unity.modules.ui":"1.0.0","com.unity.modules.imgui":"1.0.0"}}')
[IO.File]::WriteAllText((Join-Path $fixture 'ProjectSettings/ProjectVersion.txt'), "m_EditorVersion: 2022.3.62f3`n")
Copy-Item -Path (Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding/*.cs') -Destination (Join-Path $fixture 'Assets')
Copy-Item -Path (Join-Path $root 'Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntime*.cs') -Destination (Join-Path $fixture 'Assets')
Copy-Item -LiteralPath (Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll') -Destination (Join-Path $fixture 'Assets/Plugins')
Copy-Item -LiteralPath (Join-Path $root 'Mods/example.charge-ui') -Destination (Join-Path $fixture 'Mods') -Recurse
Copy-Item -LiteralPath (Join-Path $root 'Assets/vanillaXml/stages.xml') -Destination (Join-Path $fixture 'FixtureData')
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/UI/Modding/ModUiView.cs') -Destination (Join-Path $fixture 'Assets')
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/UI/Modding/ModUiCoordinator.cs') -Destination (Join-Path $fixture 'Assets')
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/UI/Modding/ModUiGameBridge.cs') -Destination (Join-Path $fixture 'Assets')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ModUiBridgeFixtureStubs.cs') -Destination (Join-Path $fixture 'Assets')
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Nekki/SF2/GUI/ResolutionImage.cs') -Destination (Join-Path $fixture 'Assets')
foreach ($resource in @('ui/atlases/CommonButtons.png','ui/atlases/CommonButtons.BtnWhite.asset',
    'ui/atlases/DialogScroll.png','ui/atlases/DialogScroll.Background_Center.asset',
    'ui/atlases/FightUI.png','ui/atlases/FightUI.HealthBar_Full.asset','ui/atlases/FightUI.HealthBar_Empty.asset',
    'ui/atlases/MiscSprites.png','ui/atlases/MiscSprites.checkboxOff.asset','ui/atlases/MiscSprites.checkboxOn.asset',
    'ui/atlases/SlidersSettings.png','ui/atlases/SlidersSettings.SettingsEmpty.asset','ui/atlases/SlidersSettings.full.asset','ui/atlases/SlidersSettings.slider.asset',
    'ui/fonts/AGOpusBold.ttf')) {
    $destination = Join-Path $fixture ('Assets/Resources/' + $resource)
    New-Item -ItemType Directory -Force -Path (Split-Path -Parent $destination) | Out-Null
    Copy-Item -LiteralPath (Join-Path $root ('Assets/Resources/' + $resource)) -Destination $destination
    Copy-Item -LiteralPath (Join-Path $root ('Assets/Resources/' + $resource + '.meta')) -Destination ($destination + '.meta')
}
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ValidateModUiUnity.cs') -Destination (Join-Path $fixture 'Assets/Editor')
$log = Join-Path $fixture 'validation.log'
Write-Host "Unity UI fixture: $fixture"
$arguments = @('-batchmode','-projectPath',('"' + $fixture + '"'),'-executeMethod','ValidateModUiUnity.RunEditor','-logFile',('"' + $log + '"'))
if ($WithPreview) { $arguments += '-uiPreview' } else { $arguments += '-nographics' }
$process = Start-Process -FilePath $Unity -ArgumentList $arguments -WindowStyle Hidden -PassThru
Write-Host "Unity UI process: $($process.Id)"
$process.WaitForExit()
if ($process.ExitCode -ne 0) {
    Select-String -LiteralPath $log -Pattern 'error CS|Exception|\[ModUiUnity\]' -Context 0,2
    throw "Unity UI fixture failed: $log"
}
$passed = Select-String -LiteralPath $log -Pattern '\[ModUiUnity\] PASS:'
if (-not $passed) { throw "Unity UI fixture exited without pass evidence: $log" }
$passed | ForEach-Object { Write-Host $_.Line }
