param(
    [Parameter(Mandatory=$true)][string]$Animation,
    [int]$ExpectedFrames = 60,
    [int]$ExpectedNodes = 67,
    [switch]$WithMovePreview,
    [int]$TimeoutSeconds = 180,
    [string]$Unity = 'F:\UnityInstalls\2022.3.62f3\Editor\Unity.exe'
)

$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$animationPath = (Resolve-Path -LiteralPath $Animation).Path
# A fresh isolated Unity project avoids touching any open editor/session/assets.
$fixture = Join-Path $root ('Temp/AnimationReader-' + [Guid]::NewGuid().ToString('N'))
foreach ($directory in @('Assets/Editor','Packages','ProjectSettings')) {
    New-Item -ItemType Directory -Path (Join-Path $fixture $directory) -Force | Out-Null
}
[IO.File]::WriteAllText((Join-Path $fixture 'Packages/manifest.json'), '{"dependencies":{}}')
[IO.File]::WriteAllText((Join-Path $fixture 'ProjectSettings/ProjectVersion.txt'), "m_EditorVersion: 2022.3.62f3`n")

# Compile the exact recovered reader method, extracted from the working tree.
# Only its containing class is reduced to the two fields the method uses.
$source = Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/InfoAnimation.cs')
$method = [regex]::Match($source, '(?s)\tprivate void ReadAnimation\(byte\[\] data\).*?(?=\r?\n\tprivate void BAIMGDMKILA)')
if (!$method.Success) { throw 'Recovered reader boundary changed; review the fixture extractor.' }
$reader = "using UnityEngine;`npublic class RecoveredAnimationReader {`npublic Vector3[][] _AnimationContainer;`npublic int LHHAGECFIOL;`npublic void Read(byte[] data) { ReadAnimation(data); }`n" + $method.Value + "`n}`n"
[IO.File]::WriteAllText((Join-Path $fixture 'Assets/Editor/RecoveredAnimationReader.cs'), $reader)
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/BinaryReaderNekki.cs') -Destination (Join-Path $fixture 'Assets/Editor/BinaryReaderNekki.cs')
Copy-Item -LiteralPath $animationPath -Destination (Join-Path $fixture 'Assets/sample.bytes')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ValidateSf2Animation.cs') -Destination (Join-Path $fixture 'Assets/Editor/ValidateSf2Animation.cs')
if ($WithMovePreview) {
    Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Content/LocalAnimationPreview.cs') -Destination (Join-Path $fixture 'Assets/Editor/LocalAnimationPreview.cs')
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ValidateMovePreview.cs') -Destination (Join-Path $fixture 'Assets/Editor/ValidateMovePreview.cs')
    Copy-Item -LiteralPath (Join-Path $root 'Assets/vanillaXml/animations/moves.xml') -Destination (Join-Path $fixture 'vanilla-moves.xml')
    $previewDirectory = Join-Path $fixture 'Library/EclipseAnimationPreview'
    New-Item -ItemType Directory -Path $previewDirectory -Force | Out-Null
    Copy-Item -LiteralPath (Join-Path $root 'Library/EclipseAnimationPreview/Move.xml') -Destination $previewDirectory
    Copy-Item -LiteralPath $animationPath -Destination (Join-Path $previewDirectory 'kazuya_ff3.bytes')
    [IO.File]::WriteAllText((Join-Path $previewDirectory 'enabled'), 'enabled')
}
[IO.File]::WriteAllText((Join-Path $fixture 'expected.txt'), "$ExpectedFrames`n$ExpectedNodes")
$log = Join-Path $fixture 'validation.log'
$entryPoint = if ($WithMovePreview) { 'ValidateMovePreview.Run' } else { 'ValidateSf2Animation.Run' }
$arguments = @('-batchmode','-nographics','-projectPath',('"'+$fixture+'"'),'-executeMethod',$entryPoint,'-logFile',('"'+$log+'"'))
$process = Start-Process -FilePath $Unity -ArgumentList $arguments -WindowStyle Hidden -PassThru
$timer = [Diagnostics.Stopwatch]::StartNew()
while (!$process.WaitForExit(1000)) {
    if ($timer.Elapsed.TotalSeconds -gt $TimeoutSeconds) {
        $process.Kill()
        throw "Isolated animation check timed out: $log"
    }
}
Get-Content -LiteralPath $log | Select-String -Pattern '\[AnimationReader\]|\[AnimationPreview\]|error CS|Exception' | ForEach-Object { $_.Line }
if ($process.ExitCode -ne 0 -or !(Select-String -LiteralPath $log -SimpleMatch '[AnimationReader] PASS' -Quiet)) {
    throw "Animation reader validation failed: $log"
}
Write-Output "Validation log: $log"
