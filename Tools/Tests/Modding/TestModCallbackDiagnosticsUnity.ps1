param([string]$Unity = '')
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
$versionFile = Join-Path $root 'ProjectSettings/ProjectVersion.txt'
$version = (Select-String -LiteralPath $versionFile -Pattern '^m_EditorVersion: (.+)$').Matches[0].Groups[1].Value
if (!$Unity) { $Unity = "F:\UnityInstalls\$version\Editor\Unity.exe" }
if (!(Test-Path -LiteralPath $Unity)) { throw 'Pass the matching Unity editor with -Unity.' }
$fixture = Join-Path $root ('Temp/ModCallbackDiagnosticsUnity-' + [Guid]::NewGuid().ToString('N'))
foreach ($folder in @('Assets/Plugins','Packages','ProjectSettings')) {
    New-Item -ItemType Directory -Force -Path (Join-Path $fixture $folder) | Out-Null
}
@{dependencies=@{'com.unity.modules.imgui'='1.0.0';'com.unity.modules.imageconversion'='1.0.0';'com.unity.modules.screencapture'='1.0.0'}} | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $fixture 'Packages/manifest.json')
Copy-Item -LiteralPath $versionFile -Destination (Join-Path $fixture 'ProjectSettings')
Copy-Item -Path (Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding/*.cs') -Destination (Join-Path $fixture 'Assets')
Copy-Item -Path (Join-Path $root 'Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntime*.cs') -Destination (Join-Path $fixture 'Assets')
foreach ($source in @('Assets/Scripts/Eclipse/Modding/ModScriptSession.cs','Assets/Scripts/Eclipse/UI/PerformanceOverlay.cs','Tools/Tests/Modding/ModCallbackDiagnosticsUnity.cs','Tools/Tests/Modding/ModCallbackDiagnosticsUnityStubs.cs')) {
    Copy-Item -LiteralPath (Join-Path $root $source) -Destination (Join-Path $fixture 'Assets')
}
Copy-Item -LiteralPath (Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll') -Destination (Join-Path $fixture 'Assets/Plugins')
$log = Join-Path $fixture 'validation.log'
Write-Host "Native callback diagnostics fixture: $fixture"
# IMGUI screen capture needs a rendering Game View; batch mode has no screen
# presentation. This isolated editor is hidden and exits after its checks.
$process = Start-Process -FilePath $Unity -ArgumentList @('-projectPath',('"'+$fixture+'"'),'-executeMethod','ModCallbackDiagnosticsUnity.Run','-logFile',('"'+$log+'"'),'-screen-width','1280','-screen-height','720') -WindowStyle Hidden -PassThru
$deadline = [DateTime]::UtcNow.AddMinutes(5)
while (!$process.WaitForExit(20000)) {
    if ([DateTime]::UtcNow -gt $deadline) { $process.Kill(); throw "Native callback diagnostics timed out: $log" }
}
if ($process.ExitCode -ne 0) {
    Select-String -LiteralPath $log -Pattern 'error CS|Exception|\[ModCallbackDiagnosticsUnity\]' -Context 0,2
    throw "Native callback diagnostics failed: $log"
}
$result = Join-Path $fixture 'validation-result.txt'
if (!(Test-Path -LiteralPath $result)) { throw "Unity exited without acceptance evidence: $log" }
Get-Content -LiteralPath $result
