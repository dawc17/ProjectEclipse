param([string]$Unity = '')
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
$version = (Select-String -LiteralPath (Join-Path $root 'ProjectSettings/ProjectVersion.txt') -Pattern '^m_EditorVersion: (.+)$').Matches[0].Groups[1].Value
if (!$Unity) { $Unity = "F:\UnityInstalls\$version\Editor\Unity.exe" }
if (!(Test-Path -LiteralPath $Unity)) { throw 'Pass the matching Unity editor with -Unity.' }
$fixture = Join-Path $root ('Temp/VersusPreviewNative-' + [Guid]::NewGuid().ToString('N'))
foreach ($folder in @('Assets/Editor', 'Packages', 'ProjectSettings')) {
    New-Item -ItemType Directory -Force -Path (Join-Path $fixture $folder) | Out-Null
}
Copy-Item -LiteralPath (Join-Path $root 'ProjectSettings/ProjectVersion.txt') -Destination (Join-Path $fixture 'ProjectSettings')
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Multiplayer/VersusFighterPreview.cs') -Destination (Join-Path $fixture 'Assets')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'VersusPreviewNative.cs') -Destination (Join-Path $fixture 'Assets/Editor')
@{ dependencies = @{ 'com.unity.ugui' = '2.0.0'; 'com.unity.modules.ui' = '1.0.0'; 'com.unity.modules.imgui' = '1.0.0' } } | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $fixture 'Packages/manifest.json')
@"
%YAML 1.1
--- !u!129 &1
PlayerSettings:
  m_ObjectHideFlags: 0
  companyName: EclipseTests
  productName: $(Split-Path $fixture -Leaf)
"@ | Set-Content -LiteralPath (Join-Path $fixture 'ProjectSettings/ProjectSettings.asset')
@'
using UnityEngine;
public class ModelParameters { }
public static class StageType { public enum FDBBPEGEGMK { STAGE_SHOP_START } }
namespace Eclipse.Multiplayer {
 public class VersusLoadout { }
 public static class LocalVersusMatch {
  public static ModelParameters PrepareFighter(VersusLoadout loadout,bool left,string name)=>new ModelParameters();
 }
}
namespace Nekki.SF2.Core.Fights {
 public class ModelContainer : MonoBehaviour {
  public int Ticks;
  Renderer bodyRenderer;
  Color baseColor;
  public void Init() { }
  public void ShowParameters(ModelParameters parameters,StageType.FDBBPEGEGMK stage,string screen,Color tint) {
   var body=GameObject.CreatePrimitive(PrimitiveType.Quad);
   body.transform.SetParent(transform,false);
   var material=new Material(Shader.Find("Unlit/Color"));
   material.color=tint;
   bodyRenderer=body.GetComponent<Renderer>();
   bodyRenderer.material=material;
   baseColor=tint;
  }
  void FixedUpdate() {
   Ticks++;
   if(bodyRenderer!=null)bodyRenderer.material.color=Color.Lerp(baseColor,Color.red,Mathf.PingPong(Ticks*.13f,1f));
  }
 }
}
'@ | Set-Content -LiteralPath (Join-Path $fixture 'Assets/PreviewFixtureStubs.cs')
$log = Join-Path $fixture 'validation.log'
Write-Host "Rendered preview fixture: $fixture"
# Rendering is required: do not use -nographics for pixel/alpha checks.
$process = Start-Process -FilePath $Unity -ArgumentList @('-batchmode', '-projectPath', ('"' + $fixture + '"'), '-executeMethod', 'VersusPreviewNative.Run', '-logFile', ('"' + $log + '"')) -WindowStyle Hidden -PassThru
$deadline = [DateTime]::UtcNow.AddMinutes(5)
while (!$process.WaitForExit(20000)) {
    if ([DateTime]::UtcNow -gt $deadline) { $process.Kill(); throw "Preview validation timed out; see $log" }
}
if ($process.ExitCode -ne 0) { throw "Preview validation failed; see $log" }
$result = Join-Path $fixture 'validation-result.txt'
if (!(Test-Path -LiteralPath $result)) { throw "Validation did not finish; see $log" }
Get-Content -LiteralPath $result
