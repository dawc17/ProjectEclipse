param(
    [Parameter(Mandatory=$true)][string]$Blender,
    [Parameter(Mandatory=$true)][string]$Suite
)
$ErrorActionPreference='Stop'
$root=Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$fixture=Join-Path $root ('Temp/GymnastPipeline-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
python (Join-Path $PSScriptRoot 'TestPackageCharacter.py')
if ($LASTEXITCODE -ne 0) { throw 'Character packaging tests failed.' }
dotnet run --project (Join-Path $root 'Tools/AssetPacker') -- extract (Join-Path $root 'Assets/StreamingAssets/SF2Content/ArtBundles/MODELS.tar.lz4') (Join-Path $fixture 'Core')
if ($LASTEXITCODE -ne 0) { throw 'Canonical model extraction failed.' }
$rig=Join-Path $fixture 'Core/models/mdl_skeleton.xml'
$blend=Join-Path $fixture 'character.blend'
$bridge=Join-Path $PSScriptRoot 'GymnastBridge.py'
& $Blender --background --factory-startup --python-exit-code 1 --python $bridge -- prepare --suite $Suite --rig $rig --output $blend
if ($LASTEXITCODE -ne 0) { throw 'Gymnast preparation failed.' }
$authored=Join-Path $fixture 'Authored'
& $Blender --background --factory-startup $blend --python-exit-code 1 --python (Join-Path $PSScriptRoot 'ValidateGymnastScene.py') -- --suite $Suite --output $authored
if ($LASTEXITCODE -ne 0) { throw 'Gymnast IK/model fixture failed.' }
$clip=Join-Path $authored 'move.bin'
$package=Join-Path $fixture 'local.gymnast-preview'
& $Blender --background --factory-startup (Join-Path $authored 'authored.blend') --python-exit-code 1 --python $bridge -- export --suite $Suite --rig $rig --output $clip --skin (Join-Path $authored 'skin.xml') --package $package --mod-id local.gymnast-preview
if ($LASTEXITCODE -ne 0) { throw 'Gymnast export/package failed.' }
& (Join-Path $PSScriptRoot 'TestCharacterLua.ps1') -Package $package -Packaged
$spaced=Join-Path $fixture 'local.gymnast-spaced'
python (Join-Path $PSScriptRoot 'PackageCharacter.py') --rig $rig --animation $clip --skin (Join-Path $authored 'skin.xml') --output $spaced --mod-id local.gymnast-spaced --mid-frames 2
if ($LASTEXITCODE -ne 0) { throw 'Interpolated sample packaging failed.' }
& (Join-Path $PSScriptRoot 'TestCharacterLua.ps1') -Package $spaced -Packaged
& (Join-Path $PSScriptRoot 'TestSf2Animation.ps1') -Animation $clip -ExpectedFrames 60 -ExpectedNodes 67
Write-Output "Gymnast acceptance fixture: $fixture"
