"""Blender 3.6+: import an SF2 point rig, edit/retarget it, export a character package.

blender --background --python-exit-code 1 --python BlenderCharacter.py -- create --rig body.xml --blend character.blend
blender --background character.blend --python-exit-code 1 --python BlenderCharacter.py -- export --output package
"""
import argparse
import hashlib
import itertools
import json
from pathlib import Path
import sys
import xml.etree.ElementTree as ET

import bpy
from mathutils import Matrix, Vector

sys.path.insert(0, str(Path(__file__).resolve().parent))
import CharacterPipeline as pipeline

SCALE = 100.0


def raw_position(value):
    return [value.x * SCALE, value.z * SCALE, -value.y * SCALE]


def blender_position(value):
    return Vector((value[0] / SCALE, -value[2] / SCALE, value[1] / SCALE))


def collection(name):
    found = bpy.data.collections.get(name)
    if found is None:
        found = bpy.data.collections.new(name)
        bpy.context.scene.collection.children.link(found)
    return found


def create(args):
    rig = pipeline.model(args.rig)
    if bpy.data.collections.get('SF2_Nodes'):
        raise ValueError('This scene already contains an SF2 rig')
    nodes = collection('SF2_Nodes'); collection('SF2_Skins')
    scene = bpy.context.scene
    scene['sf2_rig_xml'] = ET.tostring(rig, encoding='unicode')
    scene.render.fps = 60; scene.frame_start = 1; scene.frame_end = 60
    for node in rig.find('Nodes'):
        obj = bpy.data.objects.new(node.tag, None); nodes.objects.link(obj)
        obj['sf2_node'] = node.tag
        obj.location = blender_position([float(node.get(k, 0)) for k in 'XYZ'])
        obj.empty_display_type = 'SPHERE'; obj.empty_display_size = 0.025
        obj.show_name = True
        # These original runtime attributes are visible in Blender custom properties.
        obj['sf2_type'] = node.get('Type'); obj['sf2_mass'] = float(node.get('Mass', 0))
    if args.animation:
        clip = pipeline.read_animation(args.animation, rig)
        scene.frame_end = (len(clip['frames']) - 1) * (args.mid_frames + 1) + 1
        lookup = {o['sf2_node']: o for o in nodes.objects}
        for frame, points in enumerate(clip['frames'], 1):
            for name, point in zip(clip['names'], points):
                obj = lookup[name]; obj.location = blender_position(point)
                obj.keyframe_insert(data_path='location', frame=(frame - 1) * (args.mid_frames + 1) + 1)
        for obj in nodes.objects:
            for curve in obj.animation_data.action.fcurves:
                for key in curve.keyframe_points:
                    key.interpolation = 'LINEAR'
    scene.frame_set(scene.frame_start)
    args.blend.parent.mkdir(parents=True, exist_ok=True)
    bpy.ops.wm.save_as_mainfile(filepath=str(args.blend.resolve()))


def skin_model(rig, rest_positions):
    """Fit each mesh vertex to four point landmarks, matching native MacroNode math."""
    root = ET.Element('Scene'); nodes = ET.SubElement(root, 'Nodes'); ET.SubElement(root, 'Edges'); figures = ET.SubElement(root, 'Figures')
    meshes = sorted((o for o in collection('SF2_Skins').objects if o.type == 'MESH'), key=lambda o: o.name)
    body = [(n, Vector(p)) for n, p in rest_positions.items()]
    total = 0
    for obj in meshes:
        mesh = obj.data; mesh.calc_loop_triangles()
        prefix = 'Skin' + hashlib.sha256(obj.name.encode()).hexdigest()[:10]
        vertex_names = []
        for vertex in mesh.vertices:
            total += 1
            if total > 2048:
                raise ValueError('Skin export permits at most 2048 vertices')
            point = Vector(raw_position(obj.matrix_world @ vertex.co))
            explicit = [obj.vertex_groups[g.group].name for g in vertex.groups if g.weight > 0 and obj.vertex_groups[g.group].name in rest_positions]
            nearest = sorted(body, key=lambda pair: (pair[1] - point).length_squared)[:8]
            candidates = [(n, Vector(rest_positions[n])) for n in explicit] if explicit else nearest
            if explicit and len(explicit) != 4:
                raise ValueError(f'{obj.name} vertex {vertex.index}: use exactly four SF2 node vertex groups, or none for automatic fitting')
            best = None
            for basis in itertools.combinations(candidates, 4):
                matrix = Matrix([[p[axis] for _, p in basis] for axis in range(3)] + [[1, 1, 1, 1]])
                determinant = abs(matrix.determinant())
                if determinant > 0.00001 and (best is None or determinant > best[0]):
                    best = (determinant, basis, matrix)
            if best is None:
                raise ValueError(f'{obj.name} vertex {vertex.index}: four non-coplanar landmarks are required')
            _, basis, matrix = best; weights = matrix.inverted() @ Vector((*point, 1))
            if max(abs(w) for w in weights) > 100:
                raise ValueError(f'{obj.name} vertex {vertex.index}: unstable attachment; choose closer landmark groups')
            name = prefix + 'V' + str(vertex.index); vertex_names.append(name)
            node = ET.SubElement(nodes, name, Type='MacroNode', X='0', Y='0', Z='0', NodesCount='4')
            for i, ((landmark, _), weight) in enumerate(zip(basis, weights), 1):
                node.set('ChildNode' + str(i), landmark); node.set('LCC' + str(i), format(weight, '.9g'))
        for i, triangle in enumerate(mesh.loop_triangles):
            ET.SubElement(figures, prefix + 'T' + str(i), Type='Triangle',
                          **{'Node' + str(k + 1): vertex_names[v] for k, v in enumerate(triangle.vertices)})
    return root if total else None


def export(args):
    scene = bpy.context.scene
    if 'sf2_rig_xml' not in scene:
        raise ValueError('Create/import an SF2 authoring scene first')
    rig = ET.fromstring(scene['sf2_rig_xml'])
    names = [node.tag for node in rig.find('Nodes')]
    lookup = {}
    for obj in collection('SF2_Nodes').objects:
        if 'sf2_node' in obj:
            name = obj['sf2_node']
            if name in lookup:
                raise ValueError('Duplicate node binding: ' + name)
            lookup[name] = obj
    if set(lookup) != set(names):
        raise ValueError('Do not remove/rename node bindings; the complete rig is required')
    previous = scene.frame_current
    output = args.output.resolve(); models = output / 'assets/models'; animations = output / 'assets/animations'
    models.mkdir(parents=True, exist_ok=True); animations.mkdir(parents=True, exist_ok=True)
    try:
        scene.frame_set(scene.frame_start)
        depsgraph = bpy.context.evaluated_depsgraph_get()
        rest = {n: raw_position(lookup[n].evaluated_get(depsgraph).matrix_world.translation) for n in names}
        pipeline.helper_positions(rig, rest)
        for node in rig.find('Nodes'):
            for axis, number in zip('XYZ', rest[node.tag]):
                node.set(axis, format(number, '.9g'))
            node.set('Mass', format(float(lookup[node.tag].get('sf2_mass', node.get('Mass', 0))), '.9g'))
        for edge in rig.find('Edges'):
            edge.set('Length', format((Vector(rest[edge.get('End1')]) - Vector(rest[edge.get('End2')])).length, '.9g'))
        body_path = models / 'body.xml'; ET.ElementTree(rig).write(body_path, encoding='utf-8', xml_declaration=True)
        pipeline.model(body_path)
        skin = skin_model(rig, rest)
        if skin is not None:
            skin_path = models / 'skin.xml'; ET.ElementTree(skin).write(skin_path, encoding='utf-8', xml_declaration=True)
            pipeline.model(skin_path, rig)
        frames = []
        for frame in range(scene.frame_start, scene.frame_end + 1):
            scene.frame_set(frame)
            depsgraph = bpy.context.evaluated_depsgraph_get()
            frames.append([raw_position(lookup[n].evaluated_get(depsgraph).matrix_world.translation) for n in names])
        source = {'version': 1, 'fps': scene.render.fps / scene.render.fps_base, 'names': names, 'frames': frames}
        clip = pipeline.bake(rig, source); clip_path = animations / 'authored.bytes'; pipeline.write_animation(clip_path, clip)
        (output / 'frames.json').write_text(json.dumps(source), encoding='utf-8')
        metadata = {'version': 1, 'fps': 60, 'mid_frames': 0, 'frames': len(clip['frames']), 'nodes': names,
                    'rig_sha256': hashlib.sha256(body_path.read_bytes()).hexdigest(), 'animation_sha256': hashlib.sha256(clip_path.read_bytes()).hexdigest()}
        clip_path.with_suffix('.rig.json').write_text(json.dumps(metadata, indent=2), encoding='utf-8')
        pipeline.preview(output / 'preview.html', rig, clip)
        # This is a module to require from an existing mod, not a hidden global patch.
        lua = '''local sf2 = require("sf2")
local character = sf2.warriors.register {
    id = "authored_character", level = 1,
    template = sf2.warriors.get_template("core:warrior-templates/man_kungfu"),
    body_model = sf2.assets.model("models/body"),
    SKINS
    tactic = "Standard",
}
local move = sf2.moves.register {
    id = "authored_move", animation = sf2.assets.binary("animations/authored"),
    core_templates = { "Controlled", "NotTitan" }, type = "MOVE", priority = 150,
    mid_frames = 0, first_frame = 0, end_frame = LAST, mirror_node = "NHeel_1",
    events = { "key_pressed" },
    conditions = {
        { type = "character", warrior = character },
        { type = "keys", keys = { { key = "Punch", press = "Tap" } } },
        { type = "current_interval", name = "Uninterrupt", ["not"] = true },
    },
    intervals = { { name = "Uninterrupt", start = 0, ["end"] = LAST } },
}
return { warrior = character, move = move }
'''.replace('SKINS', 'skin_models = { sf2.assets.model("models/skin") },' if skin is not None else '').replace('LAST', str(len(clip['frames']) - 1))
        (output / 'character.generated.lua').write_text(lua, encoding='utf-8')
        print('SF2 EXPORT: ' + str(output))
    finally:
        scene.frame_set(previous)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('command', choices=['create', 'export'])
    parser.add_argument('--rig', type=Path); parser.add_argument('--animation', type=Path)
    parser.add_argument('--blend', type=Path); parser.add_argument('--output', type=Path)
    parser.add_argument('--mid-frames', type=int, choices=range(9), default=0)
    args = parser.parse_args(sys.argv[sys.argv.index('--') + 1:])
    if args.command == 'create':
        if args.rig is None or args.blend is None:
            parser.error('create requires --rig and --blend')
        create(args)
    else:
        if args.output is None:
            parser.error('export requires --output')
        export(args)


if __name__ == '__main__':
    main()
