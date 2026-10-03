param([string]$Unity = '')
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
$versionFile = Join-Path $root 'ProjectSettings/ProjectVersion.txt'
$version = (Select-String -LiteralPath $versionFile -Pattern '^m_EditorVersion: (.+)$').Matches[0].Groups[1].Value
if (!$Unity) { $Unity = "F:\UnityInstalls\$version\Editor\Unity.exe" }
if (!(Test-Path -LiteralPath $Unity)) { throw 'Pass the matching Unity editor with -Unity.' }
$fixture = Join-Path $root ('Temp/ModExtensionsUnity-' + [Guid]::NewGuid().ToString('N'))
foreach ($folder in @('Assets/Plugins','Assets/Resources/ui/fonts','Packages','ProjectSettings','Mods','FixtureData')) {
    New-Item -ItemType Directory -Force -Path (Join-Path $fixture $folder) | Out-Null
}
$dependencies = (Get-Content -Raw -LiteralPath (Join-Path $root 'Packages/manifest.json') | ConvertFrom-Json).dependencies
@{dependencies=@{'com.unity.ugui'=$dependencies.'com.unity.ugui';'com.unity.modules.ui'='1.0.0';'com.unity.modules.imgui'='1.0.0';'com.unity.modules.imageconversion'='1.0.0'}} | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $fixture 'Packages/manifest.json')
Copy-Item -LiteralPath $versionFile -Destination (Join-Path $fixture 'ProjectSettings')
Copy-Item -Path (Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding/*.cs') -Destination (Join-Path $fixture 'Assets')
Copy-Item -Path (Join-Path $root 'Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntime*.cs') -Destination (Join-Path $fixture 'Assets')
foreach ($source in @('Assets/Scripts/Eclipse/Modding/ModScriptSession.cs','Assets/Scripts/Eclipse/UI/Modding/ModUiView.cs','Assets/Scripts/Eclipse/UI/Modding/ModUiFade.cs','Assets/Scripts/Eclipse/UI/PressBounce.cs','Assets/Scripts/Eclipse/UI/ComponentUtility.cs','Tools/Tests/Modding/ModExtensionsUnity.cs','Tools/Tests/Modding/ModExtensionsUnityStubs.cs')) {
    Copy-Item -LiteralPath (Join-Path $root $source) -Destination (Join-Path $fixture 'Assets')
}
Copy-Item -LiteralPath (Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll') -Destination (Join-Path $fixture 'Assets/Plugins')
foreach ($suffix in @('','.meta')) {
    Copy-Item -LiteralPath (Join-Path $root ('Assets/Resources/ui/fonts/AGOpusBold.ttf'+$suffix)) -Destination (Join-Path $fixture 'Assets/Resources/ui/fonts')
}
foreach ($id in @('example.focus-framework','example.focus-addon')) {
    Copy-Item -LiteralPath (Join-Path $root ('Mods/'+$id)) -Destination (Join-Path $fixture 'Mods') -Recurse
}
Copy-Item -LiteralPath (Join-Path $root 'Assets/vanillaXml/stages.xml') -Destination (Join-Path $fixture 'FixtureData')
$log = Join-Path $fixture 'validation.log'
Write-Host "Native framework fixture: $fixture"
$process = Start-Process -FilePath $Unity -ArgumentList @('-batchmode','-projectPath',('"'+$fixture+'"'),'-executeMethod','ModExtensionsUnity.Run','-logFile',('"'+$log+'"')) -WindowStyle Hidden -PassThru
$deadline = [DateTime]::UtcNow.AddMinutes(5)
while (!$process.WaitForExit(20000)) {
    if ([DateTime]::UtcNow -gt $deadline) { $process.Kill(); throw "Native framework validation timed out: $log" }
}
if ($process.ExitCode -ne 0) {
    Select-String -LiteralPath $log -Pattern 'error CS|Exception|\[ModExtensionsUnity\]' -Context 0,2
    throw "Native framework validation failed: $log"
}
$result = Join-Path $fixture 'validation-result.txt'
if (!(Test-Path -LiteralPath $result)) { throw "Unity exited without acceptance evidence: $log" }
Get-Content -LiteralPath $result
