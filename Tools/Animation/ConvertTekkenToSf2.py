"""Blender 3.6: evaluate a local Polaris importer, retarget, and write SF2 nodes.

Run with --background --factory-startup --python-exit-code 1 --python THIS -- ...
This is a grounded, unarmed humanoid prototype, not an attack/moveset converter.
No Tekken code or assets are vendored and no base-game files are overwritten.
"""
import argparse
import hashlib
import json
import math
from pathlib import Path
import struct
import sys
import types
import xml.etree.ElementTree as ET

import bpy
from mathutils import Matrix, Vector


def read_sf2(path):
    data = Path(path).read_bytes()
    offset = 4
    count = struct.unpack_from('<i', data)[0]
    if not 1 <= count <= 100000:
        raise ValueError('Invalid SF2 frame count')
    frames = []
    for _ in range(count):
        flag, nodes = struct.unpack_from('<Bi', data, offset)
        offset += 5
        # The recovered reader ignores the flag; vanilla stance contains 1 and 5.
        if not 1 <= nodes <= 10000:
            raise ValueError('Unsupported SF2 frame header')
        frame = [Vector(struct.unpack_from('<3f', data, offset + 12*i)) for i in range(nodes)]
        offset += 12*nodes
        if not all(math.isfinite(v) for p in frame for v in p):
            raise ValueError('Non-finite coordinate')
        frames.append(frame)
    if offset != len(data):
        raise ValueError('Trailing SF2 bytes')
    return frames


def write_sf2(path, frames):
    # Raw file space: X along the arena, Y up, Z depth. InfoAnimation negates Y.
    data = bytearray(struct.pack('<i', len(frames)))
    for frame in frames:
        data.extend(struct.pack('<Bi', 1, len(frame)))
        for point in frame:
            if not all(math.isfinite(v) for v in point):
                raise ValueError('Non-finite output coordinate')
            data.extend(struct.pack('<3f', *point))
    Path(path).write_bytes(data)
    decoded = read_sf2(path)
    if len(decoded) != len(frames) or any(len(f) != len(frames[0]) for f in decoded):
        raise ValueError('Round-trip size mismatch')
    return decoded


def unit(v):
    if v.length < 1e-6:
        raise ValueError('Degenerate bone or orientation axis')
    return v.normalized()


def basis(primary, secondary):
    a = unit(primary)
    b = unit(secondary - a*secondary.dot(a))
    return Matrix((a, b, a.cross(b))).transposed()


def sha(path):
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def tekken_header(path):
    data = Path(path).read_bytes()
    if data[4:8] != b'PANM':
        raise ValueError('Expected Tekken 8 PANM fullbody file')
    last, fps = struct.unpack_from('<If', data, 0x40)
    if not 1 <= last <= 100000 or not 1 <= fps <= 240:
        raise ValueError('Invalid Tekken duration/rate')
    n = struct.unpack_from('<I', data, 0x94)[0]
    if not 1 <= n <= 2000:
        raise ValueError('Invalid Tekken track count')
    tracks = []
    for i in range(n):
        offset = 0x98+4*i
        a = offset + struct.unpack_from('<I', data, offset)[0]
        size = struct.unpack_from('<I', data, a+0x14)[0]
        tracks.append(data[a+0x18:a+0x18+size].decode('utf-8').rstrip('\0'))
    return last, fps, tracks


def import_source(args):
    # Import only the decoding modules: do not register the add-on UI or downloads.
    package = types.ModuleType('eclipse_polaris_local')
    package.__path__ = [str(args.plugin.resolve())]
    sys.modules[package.__name__] = package
    from eclipse_polaris_local import core_tk8
    with bpy.data.libraries.load(str(args.rig.resolve()), link=False) as (src, dst):
        if args.armature not in src.objects:
            raise ValueError('Required non-IK armature absent: '+args.armature)
        dst.objects = [args.armature]
    obj = dst.objects[0]
    bpy.context.collection.objects.link(obj)
    bpy.context.view_layer.objects.active = obj
    obj.animation_data_clear()
    missing = core_tk8.import_tk8_anim(str(args.input.resolve()), obj, 'FULLBODY', True, False)
    unexpected = set(missing) - {'KOSI_NULL2', 'MUNE_jnt'}
    if unexpected:
        raise ValueError('Unsupported missing tracks: '+str(sorted(unexpected)))
    return obj, missing


def retarget(args, obj, last, fps, xml):
    nodes = list(xml.find('Nodes'))
    names = [n.tag for n in nodes]
    reference = read_sf2(args.reference)[args.reference_frame]
    if len(reference) != len(names) or len(names) != 67:
        raise ValueError('This profile requires the 67-node mdl_skeleton and matching reference')
    ref = dict(zip(names, reference))
    expected = {'NTop','NNeck','NHead','NPivot','NStomach','NChest','NChestF','NStomachF','NPelvisF','NHeadF','COM'}
    for side in ('1','2'):
        expected.update(n+'_'+side for n in ['NShoulder','NElbow','NWrist','NHip','NKnee','NAnkle','NToe','NToeTip','NHeel','NToeS','NKnuckles','NKnucklesS','NFingertips','NFingertipsS','NFingertipsSS','NChestS','NStomachS','NHeadS'])
        expected.update('Weapon-Node'+str(i)+'_'+side for i in range(1,5))
        expected.update('MacroNode'+str(i)+'_'+side for i in range(1,7))
    if set(names) != expected:
        raise ValueError('Unknown target skeleton; refusing an inferred node order')

    # A proper basis change: Blender -Y => arena forward, Z => up, -X => depth.
    transform = Matrix(((0,-1,0),(0,0,1),(-1,0,0)))
    if args.facing == -1:
        transform = Matrix(((-1,0,0),(0,1,0),(0,0,-1))) @ transform
    required = ['Hip','Spine1','Spine2','Neck','Head']
    required += [s+'_'+b for s in ['L','R'] for b in ['UpperArm','LowerArm','Hand','UpperLeg','LowerLeg','Foot','Toe']]
    if not set(required).issubset(obj.pose.bones.keys()):
        raise ValueError('Required rig bones missing')
    action = obj.animation_data.action
    keyed = {curve.data_path.split('"')[1] for curve in action.fcurves if 'pose.bones["' in curve.data_path}
    if not (set(required)-{'L_Toe','R_Toe'}).issubset(keyed):
        raise ValueError('Importer silently skipped required body tracks')
    if int(action.frame_range.y) != last:
        raise ValueError('Decoded timeline does not match the PANM header')

    def source():
        points = {n: transform @ (obj.matrix_world @ obj.pose.bones[n].head) for n in required}
        rotations = {n: transform @ (obj.matrix_world @ obj.pose.bones[n].matrix).to_3x3().normalized() for n in required}
        return points, rotations

    bpy.context.scene.frame_set(0)
    first, first_rot = source()
    first_pivot = (first['L_UpperLeg']+first['R_UpperLeg'])/2
    scale = sum((ref['NHip_'+s]-ref['NKnee_'+s]).length+(ref['NKnee_'+s]-ref['NAnkle_'+s]).length for s in ('1','2')) / sum((first[s+'_UpperLeg']-first[s+'_LowerLeg']).length+(first[s+'_LowerLeg']-first[s+'_Foot']).length for s in ('L','R'))
    initial = Vector((0, ref['NPivot'].y, 0))
    body_ref = basis(ref['NNeck']-ref['NPivot'], ref['NHip_1']-ref['NHip_2'])
    foot_bind = {}
    hand_bind = {}
    for side, tk in [('1','L'),('2','R')]:
        foot_ref = basis(ref['NToe_'+side]-ref['NHeel_'+side], ref['NAnkle_'+side]-ref['NHeel_'+side])
        # Align the donor SF2 rigid foot to the source foot at frame zero, then
        # preserve the source foot's full rotation, including the support-foot roll.
        forward = first[tk+'_Toe']-first[tk+'_Foot']
        foot_src = basis(Vector((forward.x, 0, forward.z)), Vector((0,1,0)))
        foot_bind[side] = first_rot[tk+'_Foot'].inverted() @ foot_src @ foot_ref.inverted()
        donor_arm = ref['NWrist_'+side]-ref['NElbow_'+side]
        source_arm = first[tk+'_Hand']-first[tk+'_LowerArm']
        align = unit(donor_arm).rotation_difference(unit(source_arm)).to_matrix()
        hand_bind[side] = first_rot[tk+'_Hand'].inverted() @ align

    out = []
    # No invented last frame: header is the inclusive final index (59 => 60 samples).
    sample_count = int(round(last/fps*args.output_fps))+1
    for frame in range(sample_count):
        time = min(last, frame*fps/args.output_fps)
        bpy.context.scene.frame_set(int(time), subframe=time-int(time))
        src, rotations = source()
        p = {}
        pivot = (src['L_UpperLeg']+src['R_UpperLeg'])/2
        p['NPivot'] = initial+(pivot-first_pivot)*scale
        # Keep translational motion in the fighting plane; retain depth in poses.
        p['NPivot'].z = 0

        def chain(child, parent, direction):
            p[child] = p[parent]+unit(direction)*(ref[child]-ref[parent]).length

        for side, tk in [('1','L'),('2','R')]:
            chain('NHip_'+side, 'NPivot', src[tk+'_UpperLeg']-pivot)
            chain('NKnee_'+side, 'NHip_'+side, src[tk+'_LowerLeg']-src[tk+'_UpperLeg'])
            chain('NAnkle_'+side, 'NKnee_'+side, src[tk+'_Foot']-src[tk+'_LowerLeg'])
            foot_rot = rotations[tk+'_Foot'] @ foot_bind[side]
            for node in ['NToe','NHeel','NToeTip','NToeS']:
                n = node+'_'+side
                p[n] = p['NAnkle_'+side]+foot_rot @ (ref[n]-ref['NAnkle_'+side])

        chain('NStomach','NPivot', src['Spine1']-pivot)
        chain('NChest','NStomach', src['Spine2']-src['Spine1'])
        chain('NNeck','NChest', src['Neck']-src['Spine2'])
        chain('NHead','NNeck', src['Head']-src['Neck'])
        head_up = transform @ (obj.matrix_world.to_3x3() @ (obj.pose.bones['Head'].tail-obj.pose.bones['Head'].head))
        chain('NTop','NHead',head_up)

        for side, tk in [('1','L'),('2','R')]:
            chain('NShoulder_'+side,'NNeck',src[tk+'_UpperArm']-src['Neck'])
            chain('NElbow_'+side,'NShoulder_'+side,src[tk+'_LowerArm']-src[tk+'_UpperArm'])
            chain('NWrist_'+side,'NElbow_'+side,src[tk+'_Hand']-src[tk+'_LowerArm'])
            hand_rot = rotations[tk+'_Hand'] @ hand_bind[side]
            hand_nodes = [n for n in names if n.endswith('_'+side) and (n.startswith(('NKnuckles','NFingertips','Weapon-Node')))]
            for n in hand_nodes:
                p[n] = p['NWrist_'+side]+hand_rot @ (ref[n]-ref['NWrist_'+side])

        # SF2 helper nodes encode torso/head orientation for its mesh renderer.
        # Rotate their donor geometry with the corresponding evaluated body segment.
        for anchor, bone, helpers in [
            ('NPivot','Hip',['NPelvisF']),
            ('NStomach','Spine1',['NStomachS_1','NStomachS_2','NStomachF']),
            ('NChest','Spine2',['NChestS_1','NChestS_2','NChestF']),
            ('NHead','Head',['NHeadS_1','NHeadS_2','NHeadF'])]:
            source_body0 = basis(first['Neck']-first_pivot, first['L_UpperLeg']-first['R_UpperLeg'])
            rot = rotations[bone] @ first_rot[bone].inverted() @ source_body0 @ body_ref.inverted()
            for n in helpers:
                p[n] = p[anchor]+rot @ (ref[n]-ref[anchor])

        if args.ground == 'min-foot':
            # Deliberately restricted to grounded moves: this would erase jumps.
            floor = min(p[n+'_'+s].y for s in ('1','2') for n in ['NToe','NToeTip','NToeS','NHeel'])
            for point in p.values():
                point.y -= floor

        # Match ModelMacroNode.FPKMHOMMFKB and mass-weighted ModelObject COM.
        for node in nodes:
            if node.attrib['Type'] == 'MacroNode':
                p[node.tag] = sum((p[node.attrib['ChildNode'+str(i)]]*float(node.attrib['LCC'+str(i)]) for i in range(1,int(node.attrib['NodesCount'])+1)), Vector())
        com = next(n for n in nodes if n.tag == 'COM')
        masses = {n.tag:float(n.attrib.get('Mass',0)) for n in nodes}
        children = [com.attrib['ChildNode'+str(i)] for i in range(1,int(com.attrib['NodesCount'])+1)]
        p['COM'] = sum((p[n]*masses[n] for n in children),Vector())/sum(masses[n] for n in children)
        if set(p) != set(names):
            raise ValueError('Incomplete node mapping: '+str(set(names)-set(p)))
        out.append([p[n].copy() for n in names])
    return out, names, scale


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    for name in ('plugin','rig','input','model','reference','output'):
        parser.add_argument('--'+name, type=Path, required=True)
    parser.add_argument('--armature', default='SINGLE-P1-ARMATURE')
    parser.add_argument('--reference-frame', type=int, default=3)
    parser.add_argument('--output-fps', type=float, default=60)
    parser.add_argument('--facing', type=int, choices=[-1,1], default=1)
    parser.add_argument('--ground', choices=['min-foot','none'], default='min-foot')
    args = parser.parse_args(sys.argv[sys.argv.index('--')+1:])
    if not 1 <= args.output_fps <= 240:
        parser.error('output-fps must be between 1 and 240')
    if args.output.resolve() in {args.input.resolve(),args.reference.resolve(),args.model.resolve()}:
        parser.error('output must differ from every input asset')
    last, fps, tracks = tekken_header(args.input)
    xml = ET.parse(args.model).getroot()
    obj, missing = import_source(args)
    frames, names, scale = retarget(args,obj,last,fps,xml)
    args.output.parent.mkdir(parents=True,exist_ok=True)
    decoded = write_sf2(args.output,frames)
    report = {
        'status':'experimental retarget; not a combat-ready move',
        'source':str(args.input.resolve()),'source_sha256':sha(args.input),
        'source_last_frame':last,'source_fps':fps,'source_tracks':tracks,
        'missing_auxiliary_tracks':missing,'blender':bpy.app.version_string,
        'plugin_sha256':{n:sha(args.plugin/n) for n in ['core_tk8.py','profiles_tk8.py']},
        'rig_sha256':sha(args.rig),'model_sha256':sha(args.model),'reference_sha256':sha(args.reference),
        'reference_frame':args.reference_frame,'output_sha256':sha(args.output),
        'output_frames':len(frames),'output_fps':args.output_fps,'node_count':len(names),
        'node_order':names,'source_to_target_leg_scale':scale,'ground':args.ground,
        'facing':args.facing,'coordinates':'raw SF2: arena X, up Y, depth Z; runtime negates Y',
        'notes':['L maps to SF2 _1; R maps to _2.',
                 'SF2 donor hand/weapon shapes retained; no Tekken finger animation.',
                 'Ground min-foot is for grounded moves only; contact sliding is not solved.',
                 'No hitboxes, damage, input rules, interrupts, or combat balance inferred.',
                 'At 60 Hz playback use MidFrames=0; MidFrames=2 triples sample spacing.']}
    args.output.with_suffix('.report.json').write_text(json.dumps(report,indent=2)+'\n')
    payload = {'names':names,'fps':args.output_fps,'frames':[[list(p) for p in frame] for frame in decoded]}
    args.output.with_suffix('.frames.json').write_text(json.dumps(payload))
    print(json.dumps({k:report[k] for k in ['output_frames','node_count','output_fps','source_to_target_leg_scale','missing_auxiliary_tracks','output_sha256']},indent=2))
    print('OUTPUT',args.output.resolve())


if __name__ == '__main__':
    main()
