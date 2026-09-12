param([string]$Blender='C:\Program Files\Blender Foundation\Blender 3.6\blender.exe')
$ErrorActionPreference='Stop'
$root=Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$fixture=Join-Path $root ('Temp/CharacterAuthoring-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
python (Join-Path $PSScriptRoot 'TestCharacterPipeline.py')
if ($LASTEXITCODE -ne 0) { throw 'Character format tests failed.' }
dotnet run --project (Join-Path $root 'Tools/AssetPacker') -- extract (Join-Path $root 'Assets/StreamingAssets/SF2Content/ArtBundles/MODELS.tar.lz4') (Join-Path $fixture 'Core')
if ($LASTEXITCODE -ne 0) { throw 'Canonical model extraction failed.' }
$rig=Join-Path $fixture 'Core/models/mdl_skeleton.xml'
if (-not (Test-Path -LiteralPath $rig)) { throw "Canonical rig not found at $rig" }
$blend=Join-Path $fixture 'character.blend'
& $Blender --background --factory-startup --python-exit-code 1 --python (Join-Path $PSScriptRoot 'BlenderCharacter.py') -- create --rig $rig --blend $blend
if ($LASTEXITCODE -ne 0) { throw 'Blender rig import failed.' }
$author=Join-Path $fixture 'author.py'
@'
import bpy
from mathutils import Vector
scene=bpy.context.scene
node=bpy.data.objects['NElbow_1']
node.keyframe_insert(data_path='location',frame=1)
node.location.z+=0.1
node.keyframe_insert(data_path='location',frame=30)
node.location.z-=0.1
node.keyframe_insert(data_path='location',frame=60)
mesh=bpy.data.meshes.new('AuthoredTriangle')
center=bpy.data.objects['NChest'].location
mesh.from_pydata([center+Vector((-.08,0,0)),center+Vector((.08,0,0)),center+Vector((0,-.02,.12))],[],[(0,1,2)])
obj=bpy.data.objects.new('AuthoredTriangle',mesh)
bpy.data.collections['SF2_Skins'].objects.link(obj)
scene.frame_set(1)
bpy.ops.wm.save_as_mainfile(filepath=bpy.data.filepath)
'@ | Set-Content -Encoding UTF8 -LiteralPath $author
& $Blender --background $blend --python-exit-code 1 --python $author
if ($LASTEXITCODE -ne 0) { throw 'Blender authored pose/skin fixture failed.' }
$package=Join-Path $fixture 'package'
& $Blender --background $blend --python-exit-code 1 --python (Join-Path $PSScriptRoot 'BlenderCharacter.py') -- export --output $package
if ($LASTEXITCODE -ne 0) { throw 'Blender character export failed.' }
python (Join-Path $PSScriptRoot 'CharacterPipeline.py') validate --rig (Join-Path $package 'assets/models/body.xml') --skin (Join-Path $package 'assets/models/skin.xml') --animation (Join-Path $package 'assets/animations/authored.bytes')
if ($LASTEXITCODE -ne 0) { throw 'Exported character contract failed.' }
& (Join-Path $PSScriptRoot 'TestSf2Animation.ps1') -Animation (Join-Path $package 'assets/animations/authored.bytes') -ExpectedFrames 60 -ExpectedNodes 67
& (Join-Path $PSScriptRoot 'TestCharacterLua.ps1') -Package $package
Write-Output "Character authoring fixture: $package"
