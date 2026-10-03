param([string]$Unity = '')
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
$versionFile = Join-Path $root 'ProjectSettings/ProjectVersion.txt'
$version = (Select-String -LiteralPath $versionFile -Pattern '^m_EditorVersion: (.+)$').Matches[0].Groups[1].Value
if ([string]::IsNullOrWhiteSpace($Unity)) {
    $Unity = @("F:\UnityInstalls\$version\Editor\Unity.exe", "$env:ProgramFiles\Unity\Hub\Editor\$version\Editor\Unity.exe") | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
}
if (!$Unity -or !(Test-Path -LiteralPath $Unity)) { throw "Unity $version not found; pass -Unity." }
$fixture = Join-Path $root ('Temp/Unity6Workflows-' + [Guid]::NewGuid().ToString('N'))
foreach ($directory in @('Assets/Editor', 'Packages', 'ProjectSettings', 'Assets/src/GUI/Scenes/GameLoaderScene')) {
    New-Item -ItemType Directory -Force -Path (Join-Path $fixture $directory) | Out-Null
}
$dependencies = (Get-Content -Raw -LiteralPath (Join-Path $root 'Packages/manifest.json') | ConvertFrom-Json).dependencies
@{ dependencies = @{
    'com.unity.inputsystem' = $dependencies.'com.unity.inputsystem'
    'com.unity.multiplayer.playmode' = $dependencies.'com.unity.multiplayer.playmode'
    'com.unity.ugui' = $dependencies.'com.unity.ugui'
    'com.unity.modules.ui' = '1.0.0'
    'com.unity.modules.imgui' = '1.0.0'
} } | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $fixture 'Packages/manifest.json')
Copy-Item -LiteralPath $versionFile -Destination (Join-Path $fixture 'ProjectSettings')
foreach ($source in @('Assets/Scripts/Eclipse/Runtime/EclipseInput.cs', 'Assets/Scripts/Eclipse/Runtime/EditorPlayModeContext.cs', 'Assets/Scripts/Eclipse/Input/EclipseUiInput.cs', 'Assets/Plugins/Assembly-CSharp-firstpass/GamePad.cs', 'Assets/Plugins/Assembly-CSharp-firstpass/GamepadState.cs')) {
    $destination = Join-Path $fixture 'Assets'
    if ($source -like 'Assets/Scripts/Eclipse/Runtime/*') {
        $destination = Join-Path $fixture 'Assets/Runtime'
        New-Item -ItemType Directory -Path $destination -Force | Out-Null
    }
    Copy-Item -LiteralPath (Join-Path $root $source) -Destination $destination
}
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Eclipse.Runtime.asmdef') -Destination (Join-Path $fixture 'Assets/Runtime')
foreach ($source in @('EclipseUnity6Workflows.cs', 'EclipsePlayerBuild.cs')) {
    Copy-Item -LiteralPath (Join-Path $root ('Assets/Editor/' + $source)) -Destination (Join-Path $fixture 'Assets/Editor')
}
# Compile the real build entry points without copying the full game's resource tree.
@'
namespace Eclipse.Content {
 public static class GameplayContentArchive {
  public const string EditorSourceDirectoryName="vanillaXml",EditableDirectoryName="xml",EditableMarkerFileName="eclipse-editable-xml.txt";
  public static string NormalizeSourceRoot(string root)=>root;
  public static string[] GetSourceFiles(string root)=>System.Array.Empty<string>();
 }
}
namespace Eclipse.UI { public static class ReleaseCheck { public const string StampFileName="eclipse-version.txt"; } }
'@ | Set-Content -LiteralPath (Join-Path $fixture 'Assets/BuildFixtureStubs.cs')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ValidateUnity6Workflows.cs') -Destination (Join-Path $fixture 'Assets/Editor')
foreach ($suffix in @('', '.meta')) {
    Copy-Item -LiteralPath (Join-Path $root ('Assets/src/GUI/Scenes/GameLoaderScene/GameLoader.unity' + $suffix)) -Destination (Join-Path $fixture 'Assets/src/GUI/Scenes/GameLoaderScene')
}
$log = Join-Path $fixture 'validation.log'
Write-Host "Unity 6 fixture: $fixture"
$arguments = @('-batchmode', '-nographics', '-projectPath', ('"' + $fixture + '"'), '-executeMethod', 'ValidateUnity6Workflows.Run', '-logFile', ('"' + $log + '"'))
$process = Start-Process -FilePath $Unity -ArgumentList $arguments -WindowStyle Hidden -PassThru
if (!$process.WaitForExit(420000)) { Stop-Process -Id $process.Id; throw "Unity fixture timed out: $log" }
$passed = Select-String -LiteralPath $log -Pattern '\[Unity6Workflows\] PASS:'
if ($process.ExitCode -ne 0 -or !$passed) {
    Select-String -LiteralPath $log -Pattern 'error CS|Exception|\[Unity6Workflows\]' -Context 0,2
    throw "Unity 6 fixture failed: $log"
}
$passed | ForEach-Object { Write-Host $_.Line }
$defineLog = Join-Path $fixture 'profile-defines.log'
$xmlProfile = 'Assets/Settings/Build Profiles/Eclipse Windows Editable XML.asset'
$arguments = @('-batchmode', '-nographics', '-quit', '-projectPath', ('"' + $fixture + '"'), '-activeBuildProfile', ('"' + $xmlProfile + '"'), '-executeMethod', 'ValidateUnity6Workflows.VerifyXmlProfileDefines', '-logFile', ('"' + $defineLog + '"'))
$process = Start-Process -FilePath $Unity -ArgumentList $arguments -WindowStyle Hidden -PassThru
if (!$process.WaitForExit(240000)) { Stop-Process -Id $process.Id; throw "Profile define check timed out: $defineLog" }
$passed = Select-String -LiteralPath $defineLog -Pattern '\[Unity6Workflows\] PASS:'
if ($process.ExitCode -ne 0 -or !$passed) { throw "Active Build Profile compilation failed: $defineLog" }
$passed | ForEach-Object { Write-Host $_.Line }
