param([string]$Unity = '')
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
$version = (Select-String -LiteralPath (Join-Path $root 'ProjectSettings/ProjectVersion.txt') -Pattern '^m_EditorVersion: (.+)$').Matches[0].Groups[1].Value
if (!$Unity) { $Unity = "F:\UnityInstalls\$version\Editor\Unity.exe" }
if (!(Test-Path -LiteralPath $Unity)) { throw 'Pass the matching Unity editor with -Unity.' }
$fixture = Join-Path $root ('Temp/PvpHealthBarNative-' + [Guid]::NewGuid().ToString('N'))
foreach ($folder in @('Assets/Editor', 'Assets/Resources/shaders', 'Packages', 'ProjectSettings')) { New-Item -ItemType Directory -Force -Path (Join-Path $fixture $folder) | Out-Null }
Copy-Item -LiteralPath (Join-Path $root 'ProjectSettings/ProjectVersion.txt') -Destination (Join-Path $fixture 'ProjectSettings')
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Multiplayer/PvpRecoverableBar.cs') -Destination (Join-Path $fixture 'Assets')
Copy-Item -LiteralPath (Join-Path $root 'Assets/Resources/shaders/EclipseRecoverableHealth.shader') -Destination (Join-Path $fixture 'Assets/Resources/shaders')
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Nekki/SF2/GUI/ResolutionImageSkew.cs') -Destination (Join-Path $fixture 'Assets')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'PvpHealthBarNative.cs') -Destination (Join-Path $fixture 'Assets/Editor')
@{ dependencies = @{ 'com.unity.ugui' = '2.0.0'; 'com.unity.modules.ui' = '1.0.0'; 'com.unity.modules.imgui' = '1.0.0' } } | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $fixture 'Packages/manifest.json')
@'
using UnityEngine;
public class ModelParameters { public float MaxLife=1, RecoverableLife, CurrentHealthBarFraction=.5f; }
public class Fight { public bool IsLocalVersus; public static Fight Current=new Fight(); public static Fight GetCurrentFight()=>Current; }
namespace Nekki.SF2.GUI { public class ResolutionImage : UnityEngine.UI.Image { } }
namespace Eclipse.Rendering.Interpolation { public class TickPresentationSmoother : MonoBehaviour { } }
'@ | Set-Content -LiteralPath (Join-Path $fixture 'Assets/FixtureStubs.cs')
$log = Join-Path $fixture 'validation.log'
Write-Host "Native PvP health bar fixture: $fixture"
$process = Start-Process -FilePath $Unity -ArgumentList @('-batchmode', '-projectPath', ('"' + $fixture + '"'), '-executeMethod', 'PvpHealthBarNative.Run', '-logFile', ('"' + $log + '"')) -WindowStyle Hidden -PassThru
$deadline = [DateTime]::UtcNow.AddMinutes(5)
while (!$process.WaitForExit(20000)) { if ([DateTime]::UtcNow -gt $deadline) { $process.Kill(); throw "Validation timed out; see $log" } }
if ($process.ExitCode -ne 0) { throw "Health bar validation failed; see $log" }
Get-Content -LiteralPath (Join-Path $fixture 'validation-result.txt')
