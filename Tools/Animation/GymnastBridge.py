"""Run inside Blender: prepare the upstream SF2 IK scene or validate/export it.

The caller supplies a Gymnast Tool Suite checkout. No upstream code is bundled
or modified; the addon is registered only in this Blender process.
"""
import argparse
import importlib.util
import json
import math
from pathlib import Path
import sys

import bpy
from mathutils import Quaternion, Vector

sys.path.insert(0, str(Path(__file__).resolve().parent))
import CharacterPipeline as pipeline
import PackageCharacter


def addon(suite):
    if bpy.app.version < (5, 0, 0):
        raise ValueError('Use Blender 5.0+ for the current Gymnast SF2 scenes (file format 405.91)')
    entry = suite / 'Addon/gymnast-tool-suite/__init__.py'
    spec = importlib.util.spec_from_file_location('eclipse_gymnast_session', entry,
                                                submodule_search_locations=[str(entry.parent)])
    module = importlib.util.module_from_spec(spec)
    sys.modules[spec.name] = module
    spec.loader.exec_module(module)
    module.register()
    return module


def bindings(rig):
    names = [node.tag for node in rig.find('Nodes')]
    missing = [name for name in names if name not in bpy.context.scene.objects]
    if missing:
        raise ValueError('Missing Gymnast node objects: ' + ', '.join(missing))
    return names


def prepare(args):
    if args.output.exists():
        raise ValueError('Prepared scene already exists; open it instead of overwriting your work')
    scene_path = args.suite / 'Blender Scenes/Shadow Fight 2/SF2Rig_ArmatureIK.blend'
    bpy.ops.wm.open_mainfile(filepath=str(scene_path.resolve()))
    addon(args.suite)
    rig = pipeline.model(args.rig)
    names = bindings(rig)
    scene = bpy.context.scene
    scene.gymnast_dependencies_xml = str(args.rig.resolve())
    scene.gymnast_normal_xml = ''
    scene.frame_start = 1
    scene.frame_end = 60
    scene.render.fps = 60
    scene.render.fps_base = 1
    scene.frame_set(1)
    armature = next((o for o in scene.objects if o.type == 'ARMATURE'), None)
    if armature is None:
        raise ValueError('The supplied Gymnast scene has no armature')
    settings = scene.gymnast_tool_props
    settings.dependencies_xml = str(args.rig.resolve())
    settings.model_xml = ''
    settings.use_armature = True
    settings.use_armature_ik = True
    settings.armature_object = armature
    settings.armature_rig_type = 'SHADOW FIGHT 2'
    # Keep the actual upstream constraints and capsule geometry. Only improve
    # selection, labels, bounds and viewport framing in this authoring copy.
    for obj in scene.objects:
        obj.select_set(False)
        if obj.name in names:
            obj.show_name = False
            obj.hide_select = True
    armature.hide_set(False)
    armature.show_in_front = True
    armature.select_set(True)
    bpy.context.view_layer.objects.active = armature
    points = [bpy.context.scene.objects[name].matrix_world.translation for name in names]
    low = Vector(tuple(min(p[i] for p in points) for i in range(3)))
    high = Vector(tuple(max(p[i] for p in points) for i in range(3)))
    for screen in bpy.data.screens:
        for area in screen.areas:
            if area.type == 'VIEW_3D':
                space = area.spaces.active
                space.clip_end = 10000
                space.shading.type = 'SOLID'
                space.region_3d.view_location = (low + high) / 2
                space.region_3d.view_distance = max(high - low) * 1.3
                space.region_3d.view_perspective = 'ORTHO'
                space.region_3d.view_rotation = Quaternion((1, 0, 0), math.pi / 2)
    bpy.ops.object.mode_set(mode='POSE')
    args.output.parent.mkdir(parents=True, exist_ok=True)
    bpy.ops.wm.save_as_mainfile(filepath=str(args.output.resolve()))
    print('PREPARED GYMNAST SCENE:', args.output.resolve())


def export(args):
    if args.package and not args.mod_id:
        raise ValueError('--package requires --mod-id')
    rig = pipeline.model(args.rig)
    names = bindings(rig)
    scene = bpy.context.scene
    frames = scene.frame_end - scene.frame_start + 1
    if frames < 2 or frames > 36000 or frames * len(names) > 2000000:
        raise ValueError('Animation needs 2..36000 frames and at most two million node samples')
    expected_fps = 60 / (args.mid_frames + 1)
    if abs(scene.render.fps / scene.render.fps_base - expected_fps) > 0.001:
        raise ValueError(f'Scene FPS must be {expected_fps:g} for mid_frames={args.mid_frames}; no implicit retiming is performed')
    if args.output.exists():
        raise ValueError('Export already exists; choose a fresh .bin path')
    module = addon(args.suite)
    previous = scene.frame_current
    sampled = []
    try:
        for frame in range(scene.frame_start, scene.frame_end + 1):
            scene.frame_set(frame)
            depsgraph = bpy.context.evaluated_depsgraph_get()
            pose = []
            for name in names:
                point = scene.objects[name].evaluated_get(depsgraph).matrix_world.translation
                pose.append([pipeline.finite(v, name) for v in (point.x, point.z, -point.y)])
            sampled.append(pose)
        args.output.parent.mkdir(parents=True, exist_ok=True)
        module.animationPanel.export_bin(str(args.output), str(args.rig), '')
        clip = pipeline.read_animation(args.output, rig)
        if len(clip['frames']) != len(sampled):
            raise ValueError('Gymnast output sample count differs from the evaluated scene range')
        for index, (expected, actual) in enumerate(zip(sampled, clip['frames'])):
            for name, a, b in zip(names, expected, actual):
                if any(abs(x - y) > 0.001 for x, y in zip(a, b)):
                    raise ValueError(f'Gymnast output differs from evaluated pose at frame {index}, node {name}')
        report = {'blender': bpy.app.version_string, 'gymnast_version': module.bl_info['version'],
                  'frames': frames, 'nodes': len(names), 'mid_frames': args.mid_frames,
                  'rig': str(args.rig.resolve()), 'animation': str(args.output.resolve()),
                  'evaluated_pose_comparison': 'passed'}
        args.output.with_suffix('.export.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
        if args.package:
            PackageCharacter.package(args.rig, args.output, args.skin, args.package,
                                     args.mod_id, args.title, args.mid_frames)
        print('GYMNAST EXPORT VERIFIED:', json.dumps(report))
    finally:
        scene.frame_set(previous)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('command', choices=['prepare', 'open', 'export'])
    parser.add_argument('--suite', type=Path, required=True)
    parser.add_argument('--rig', type=Path, required=True)
    parser.add_argument('--output', type=Path)
    parser.add_argument('--skin', type=Path, action='append', default=[])
    parser.add_argument('--package', type=Path)
    parser.add_argument('--mod-id')
    parser.add_argument('--title', default='Character Preview')
    parser.add_argument('--mid-frames', type=int, choices=range(9), default=0)
    args = parser.parse_args(sys.argv[sys.argv.index('--') + 1:])
    if args.command == 'open':
        addon(args.suite)
        bindings(pipeline.model(args.rig))
        bpy.context.scene.gymnast_dependencies_xml = str(args.rig.resolve())
        bpy.context.scene.gymnast_tool_props.dependencies_xml = str(args.rig.resolve())
        return
    if args.output is None:
        parser.error('prepare/export requires --output')
    if args.command == 'prepare':
        prepare(args)
    else:
        export(args)


if __name__ == '__main__':
    main()
