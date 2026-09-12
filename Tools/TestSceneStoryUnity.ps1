param([string]$Unity='F:\UnityInstalls\2022.3.62f3\Editor\Unity.exe')
$ErrorActionPreference='Stop'
if(!(Test-Path -LiteralPath $Unity)){throw "Unity not found: $Unity"}
$root=Split-Path $PSScriptRoot -Parent
$fixture=Join-Path $root ('Temp/SceneStoryUnity-'+[Guid]::NewGuid().ToString('N'))
foreach($dir in @('Assets/Editor','Packages','ProjectSettings')){New-Item -ItemType Directory -Path (Join-Path $fixture $dir) -Force | Out-Null}
Set-Content -LiteralPath (Join-Path $fixture 'Packages/manifest.json') -Value '{"dependencies":{}}' -Encoding UTF8
Set-Content -LiteralPath (Join-Path $fixture 'ProjectSettings/ProjectVersion.txt') -Value 'm_EditorVersion: 2022.3.62f3' -Encoding UTF8
foreach($file in @('ModId.cs','DefinitionId.cs','ModStoryEvents.cs')){
 Copy-Item -LiteralPath (Join-Path $root ('Assets/Scripts/Eclipse/Runtime/Modding/'+$file)) -Destination (Join-Path $fixture 'Assets')
}
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModSceneEntry.cs') -Destination (Join-Path $fixture 'Assets')
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ScreenType.cs') -Destination (Join-Path $fixture 'Assets')
$hostSource=Get-Content -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModRuntime.cs') -Raw -Encoding UTF8
$publish=[regex]::Match($hostSource,'(?ms)^        internal static void PublishSceneEntry\(.*?^        \}')
if(!$publish.Success){throw 'Production scene publisher not found.'}
$driver=Get-Content -LiteralPath (Join-Path $PSScriptRoot 'SceneStoryUnityDriver.cs') -Raw -Encoding UTF8
$driver.Replace('/* PUBLISH */',$publish.Value) | Set-Content -LiteralPath (Join-Path $fixture 'Assets/SceneStoryUnityDriver.cs') -Encoding UTF8
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ValidateSceneStoryUnity.cs') -Destination (Join-Path $fixture 'Assets/Editor')
$log=Join-Path $fixture 'validation.log'
Write-Host "Unity scene fixture: $fixture"
$arguments=@('-batchmode','-nographics','-projectPath',('"'+$fixture+'"'),'-executeMethod','ValidateSceneStoryUnity.RunEditor','-logFile',('"'+$log+'"'))
$sceneProcess=Start-Process -FilePath $Unity -ArgumentList $arguments -WindowStyle Hidden -PassThru
Write-Host "Unity scene process: $($sceneProcess.Id)"
$sceneProcess.WaitForExit()
if($sceneProcess.ExitCode -ne 0){Select-String -LiteralPath $log -Pattern 'error CS|\[SceneStoryUnity\]' -Context 0,4;throw "Scene fixture failed: $log"}
$passed=Select-String -LiteralPath $log -Pattern '\[SceneStoryUnity\] PASS:'
if(!$passed){throw "Unity exited without scene pass evidence: $log"}
$passed | ForEach-Object {Write-Host $_.Line}
