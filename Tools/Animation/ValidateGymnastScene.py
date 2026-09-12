"""Integration fixture: author an IK motion and export a skin with upstream Gymnast."""
import argparse
from pathlib import Path
import sys

import bpy
from mathutils import Vector

sys.path.insert(0, str(Path(__file__).resolve().parent))
import GymnastBridge as bridge

parser = argparse.ArgumentParser()
parser.add_argument('--suite', type=Path, required=True)
parser.add_argument('--output', type=Path, required=True)
args = parser.parse_args(sys.argv[sys.argv.index('--') + 1:])
bridge.addon(args.suite)
scene = bpy.context.scene
armature = bpy.data.objects['Armature']
assert bpy.context.object == armature and bpy.context.object.mode == 'POSE'
assert scene.gymnast_tool_props.use_armature_ik
assert sum(o.name.startswith('Capsule') for o in scene.objects) > 20
# Missing native nodes must fail before upstream export can silently write zeroes.
rig = bridge.pipeline.model(scene.gymnast_tool_props.dependencies_xml)
node = scene.objects['NWrist_1']
node.name = 'FixtureMissingWrist'
try:
    try:
        bridge.bindings(rig)
        raise AssertionError('Missing wrist binding was accepted')
    except ValueError as error:
        assert 'NWrist_1' in str(error)
finally:
    node.name = 'NWrist_1'
hand = armature.pose.bones['HandIK_1']
for frame, height in [(1, 0), (30, 35), (60, 0)]:
    scene.frame_set(frame)
    hand.location.z = height
    hand.keyframe_insert(data_path='location', frame=frame)
scene.frame_set(1)
before = bpy.data.objects['NWrist_1'].matrix_world.translation.copy()
scene.frame_set(30)
after = bpy.data.objects['NWrist_1'].matrix_world.translation.copy()
assert (after-before).length > 1, 'IK motion did not drive native export nodes'
scene.frame_set(1)
bpy.ops.object.mode_set(mode='OBJECT')
mesh = bpy.data.meshes.new('PreviewSkin')
top = bpy.data.objects['NTop'].matrix_world.translation.copy()
mesh.from_pydata([top+Vector((-10,0,0)),top+Vector((10,0,0)),top+Vector((0,-5,15))],[],[(0,1,2)])
skin = bpy.data.objects.new('PreviewSkin',mesh)
scene.collection.objects.link(skin)
settings = scene.gymnast_tool_model_props
settings.model_type_export = 'HEAD_GEAR'
settings.selected_object = skin
settings.model_string_name = 'EclipseFixture'
args.output.mkdir(parents=True, exist_ok=True)
result = bpy.ops.model.export_to_xml(filepath=str((args.output/'skin.xml').resolve()))
assert 'FINISHED' in result
# Render an inspection image without changing the camera in the saved authoring scene.
for obj in scene.objects:
    obj.select_set(False)
armature.select_set(True)
bpy.context.view_layer.objects.active = armature
bpy.ops.object.mode_set(mode='POSE')
bpy.ops.wm.save_as_mainfile(filepath=str((args.output/'authored.blend').resolve()))
camera_data = bpy.data.cameras.new('FixtureCamera')
camera = bpy.data.objects.new('FixtureCamera',camera_data)
scene.collection.objects.link(camera)
camera.location = (0,-700,140)
camera.rotation_euler = (Vector((0,0,140))-camera.location).to_track_quat('-Z','Y').to_euler()
camera_data.type = 'ORTHO'
camera_data.ortho_scale = 360
scene.camera = camera
scene.render.engine = 'BLENDER_WORKBENCH'
scene.render.resolution_x = 700
scene.render.resolution_y = 800
scene.render.resolution_percentage = 100
scene.render.image_settings.media_type = 'IMAGE'
scene.render.image_settings.file_format = 'PNG'
scene.render.filepath = str((args.output/'body-preview.png').resolve())
scene.display.shading.light = 'STUDIO'
scene.display.shading.color_type = 'SINGLE'
scene.display.shading.single_color = (0.32,0.22,0.13)
bpy.ops.render.render(write_still=True)
print('PASS: upstream visible body/IK scene, authored wrist motion and native skin export')
