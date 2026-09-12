"""Validate, bake and preview authored SF2 rigs and point animation. Python stdlib only."""
import argparse
import hashlib
import json
import math
from pathlib import Path
import struct
import xml.etree.ElementTree as ET


def finite(value, where):
    number = float(value)
    if not math.isfinite(number) or abs(number) > 100000:
        raise ValueError(f'{where}: coordinate must be finite within +/-100000')
    return number


def model(path, base=None):
    raw = Path(path).read_bytes()
    if len(raw) > 16 * 1024 * 1024 or b'<!DOCTYPE' in raw.upper() or b'<!ENTITY' in raw.upper():
        raise ValueError('Model exceeds 16 MiB or contains DTD/entities')
    root = ET.fromstring(raw)
    if root.tag != 'Scene' or root.find('Figures') is None:
        raise ValueError('Model requires Scene and Figures')
    inherited = {} if base is None else {n.tag: n for n in base.find('Nodes')}
    nodes = list(root.find('Nodes')) if root.find('Nodes') is not None else []
    names = list(inherited)
    for node in nodes:
        if node.tag in names or node.get('Type') not in ('Node', 'MacroNode', 'CenterOfMass'):
            raise ValueError(f'Duplicate node or unsupported type: {node.tag}')
        names.append(node.tag)
        for axis in 'XYZ':
            finite(node.get(axis, '0'), node.tag + '.' + axis)
        if finite(node.get('Mass', '0'), node.tag + '.Mass') < 0:
            raise ValueError(node.tag + ': mass cannot be negative')
    if not names or len(names) > 4096:
        raise ValueError('A composed model requires 1..4096 nodes')
    for node in nodes:
        count = int(node.get('NodesCount', 0))
        if not 0 <= count <= 128:
            raise ValueError('Invalid helper node dependency count')
        for i in range(1, count + 1):
            child = node.get('ChildNode' + str(i))
            if child not in names or names.index(child) >= names.index(node.tag):
                raise ValueError(f'{node.tag}: helper dependency must exist earlier: {child}')
            if node.get('Type') == 'MacroNode':
                finite(node.get('LCC' + str(i), '0'), node.tag + '.weight')
    edges = {} if base is None else {e.tag: e for e in base.find('Edges')}
    for edge in (root.find('Edges') if root.find('Edges') is not None else []):
        if edge.tag in edges or edge.get('Type') not in ('Edge', 'Muscle'):
            raise ValueError(f'Duplicate edge or unsupported type: {edge.tag}')
        if edge.get('End1') not in names or edge.get('End2') not in names:
            raise ValueError(f'{edge.tag}: unresolved endpoint')
        for key in ('Length', 'Radius', 'Margin1', 'Margin2'):
            finite(edge.get(key, '0'), edge.tag + '.' + key)
        edges[edge.tag] = edge
    seen = set()
    for figure in root.find('Figures'):
        if figure.tag in seen:
            raise ValueError('Duplicate figure: ' + figure.tag)
        seen.add(figure.tag)
        if figure.get('Type') == 'Triangle':
            if any(figure.get('Node' + str(i)) not in names for i in (1, 2, 3)):
                raise ValueError(figure.tag + ': unresolved triangle node')
        elif figure.get('Type') == 'Capsule':
            if figure.get('Edge') not in edges:
                raise ValueError(figure.tag + ': unresolved capsule edge')
            for key in ('Radius1', 'Radius2', 'Margin1', 'Margin2'):
                finite(figure.get(key, '0'), figure.tag + '.' + key)
        else:
            raise ValueError('Unsupported figure type: ' + str(figure.get('Type')))
    return root


def helper_positions(rig, positions):
    """Mirror the native weighted helper/COM construction in file coordinates."""
    by_name = {node.tag: node for node in rig.find('Nodes')}
    for node in rig.find('Nodes'):
        kind = node.get('Type')
        count = int(node.get('NodesCount', 0))
        if kind not in ('MacroNode', 'CenterOfMass') or count == 0:
            continue
        children = [node.get('ChildNode' + str(i)) for i in range(1, count + 1)]
        weights = ([float(node.get('LCC' + str(i), 0)) for i in range(1, count + 1)]
                   if kind == 'MacroNode' else [float(by_name[n].get('Mass', 0)) for n in children])
        if kind == 'CenterOfMass':
            total = sum(weights)
            if total <= 0:
                raise ValueError('Center of mass has no positive mass')
            weights = [w / total for w in weights]
        positions[node.tag] = [finite(sum(positions[n][axis] * w for n, w in zip(children, weights)), node.tag) for axis in range(3)]
    return positions


def bake(rig, source):
    names = [node.tag for node in rig.find('Nodes')]
    supplied = source['names']
    if len(set(supplied)) != len(supplied) or set(supplied) != set(names):
        raise ValueError('Animation names must match the complete ordered rig; duplicates/missing/extra nodes rejected')
    fps = finite(source['fps'], 'fps')
    frames = source['frames']
    if not 1 <= fps <= 240 or not 2 <= len(frames) <= 36000:
        raise ValueError('Animation requires 2..36000 samples at 1..240 fps')
    ordered = []
    for index, frame in enumerate(frames):
        if len(frame) != len(supplied) or any(len(p) != 3 for p in frame):
            raise ValueError(f'Frame {index}: wrong node/coordinate count')
        positions = {n: [finite(v, f'frame {index}/{n}') for v in p] for n, p in zip(supplied, frame)}
        helper_positions(rig, positions)
        ordered.append([positions[n] for n in names])
    count = round((len(ordered) - 1) * 60 / fps) + 1
    if count < 2 or count > 36000 or count * len(names) > 2000000:
        raise ValueError('Baked animation requires 2..36000 frames and at most two million node samples')
    result = []
    for i in range(count):
        time = min(i * fps / 60, len(ordered) - 1)
        first = int(time); second = min(first + 1, len(ordered) - 1); amount = time - first
        result.append([[a + (b - a) * amount for a, b in zip(p, q)] for p, q in zip(ordered[first], ordered[second])])
    return {'version': 1, 'fps': 60, 'names': names, 'frames': result}


def write_animation(path, clip):
    data = bytearray(struct.pack('<i', len(clip['frames'])))
    for frame in clip['frames']:
        data += struct.pack('<Bi', 1, len(frame))
        for point in frame:
            data += struct.pack('<3f', *point)
    Path(path).write_bytes(data)


def read_animation(path, rig):
    data = Path(path).read_bytes()
    names = [node.tag for node in rig.find('Nodes')]
    if len(data) < 4:
        raise ValueError('Truncated animation header')
    count, = struct.unpack_from('<i', data)
    expected = 4 + count * (5 + 12 * len(names))
    if not 1 <= count <= 36000 or expected != len(data) or count * len(names) > 2000000:
        raise ValueError('Invalid frame count, truncated/trailing data or incompatible rig')
    frames = []; offset = 4
    for i in range(count):
        _, size = struct.unpack_from('<Bi', data, offset); offset += 5
        if size != len(names):
            raise ValueError(f'Frame {i} has the wrong node count')
        frame = []
        for name in names:
            frame.append([finite(v, name) for v in struct.unpack_from('<3f', data, offset)])
            offset += 12
        frames.append(frame)
    return {'version': 1, 'fps': 60, 'names': names, 'frames': frames}


def preview(path, rig, clip):
    data = dict(clip)
    data['edges'] = [[e.get('End1'), e.get('End2')] for e in (rig.find('Edges') if rig.find('Edges') is not None else []) if e.get('Type') == 'Edge']
    payload = json.dumps(data, separators=(',', ':')).replace('<', '\\u003c')
    document = '''<!doctype html><meta charset="utf-8"><title>SF2 character preview</title>
<style>body{background:#28170e;color:#ecd1a1;font:18px Georgia;margin:24px}canvas{background:#b99b6c;width:100%;max-width:1100px}input{width:60%}button{font:inherit}</style>
<h1>Character and animation preview</h1><button id="play">Play</button> <input id="frame" type="range" min="0" value="0"> <span id="time"></span><p>Point rig preview; native collision, materials and move rules require a game playtest.</p><canvas id="canvas" width="1100" height="600"></canvas>
<script>const d=__DATA__,c=document.querySelector('canvas'),ctx=c.getContext('2d'),f=document.querySelector('#frame');f.max=d.frames.length-1;
let running=false,previous=0;document.querySelector('#play').onclick=()=>running=!running;
let loX=Infinity,hiX=-Infinity,loY=Infinity,hiY=-Infinity;for(const pose of d.frames)for(const p of pose){loX=Math.min(loX,p[0]);hiX=Math.max(hiX,p[0]);loY=Math.min(loY,p[1]);hiY=Math.max(hiY,p[1])}const scale=Math.min(1000/Math.max(1,hiX-loX),500/Math.max(1,hiY-loY));
function draw(){const pose=d.frames[+f.value],p=Object.fromEntries(d.names.map((n,i)=>[n,pose[i]]));ctx.clearRect(0,0,c.width,c.height);const xy=a=>[50+(a[0]-loX)*scale,550-(a[1]-loY)*scale];ctx.strokeStyle='#302013';ctx.lineWidth=5;for(const [a,b]of d.edges){ctx.beginPath();ctx.moveTo(...xy(p[a]));ctx.lineTo(...xy(p[b]));ctx.stroke()}ctx.fillStyle='#e5bc52';for(const a of pose){ctx.beginPath();ctx.arc(...xy(a),3,0,7);ctx.fill()}document.querySelector('#time').textContent=`Frame ${f.value} / ${f.max} · ${(+f.value/60).toFixed(2)}s`}
f.oninput=()=>{running=false;draw()};function tick(t){if(running&&t-previous>=1000/60){f.value=(+f.value+1)%d.frames.length;previous=t;draw()}requestAnimationFrame(tick)}draw();requestAnimationFrame(tick);</script>'''
    Path(path).write_text(document.replace('__DATA__', payload), encoding='utf-8')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('command', choices=['validate', 'bake', 'preview'])
    parser.add_argument('--rig', type=Path, required=True)
    parser.add_argument('--skin', type=Path, action='append', default=[])
    parser.add_argument('--source', type=Path)
    parser.add_argument('--animation', type=Path)
    parser.add_argument('--output', type=Path)
    args = parser.parse_args(); rig = model(args.rig)
    combined = ET.fromstring(ET.tostring(rig))
    for skin in args.skin:
        overlay = model(skin, combined)
        for section in ('Nodes', 'Edges', 'Figures'):
            if overlay.find(section) is not None:
                combined.find(section).extend(list(overlay.find(section)))
    if args.command == 'bake':
        if args.source is None or args.output is None:
            parser.error('bake requires --source frames.json and --output clip.bytes')
        clip = bake(rig, json.loads(args.source.read_text(encoding='utf-8')))
        args.output.parent.mkdir(parents=True, exist_ok=True)
        write_animation(args.output, clip)
        manifest = {'version': 1, 'fps': 60, 'mid_frames': 0, 'frames': len(clip['frames']), 'nodes': clip['names'],
                    'rig_sha256': hashlib.sha256(args.rig.read_bytes()).hexdigest(), 'animation_sha256': hashlib.sha256(args.output.read_bytes()).hexdigest()}
        args.output.with_suffix('.rig.json').write_text(json.dumps(manifest, indent=2), encoding='utf-8')
        preview(args.output.with_suffix('.html'), rig, clip)
    elif args.animation:
        clip = read_animation(args.animation, rig)
        sidecar = args.animation.with_suffix('.rig.json')
        if sidecar.exists():
            saved = json.loads(sidecar.read_text(encoding='utf-8'))
            if saved.get('version') != 1 or saved.get('nodes') != clip['names'] or saved.get('rig_sha256') != hashlib.sha256(args.rig.read_bytes()).hexdigest() or saved.get('animation_sha256') != hashlib.sha256(args.animation.read_bytes()).hexdigest():
                raise ValueError('Animation/rig fingerprint or node order changed; rebake the clip')
        if args.command == 'preview':
            preview(args.output or args.animation.with_suffix('.html'), rig, clip)
    elif args.command == 'preview':
        parser.error('preview requires --animation')
    print('PASS: model references and requested animation payload validated')


if __name__ == '__main__':
    main()
