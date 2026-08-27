"""Read-only dependency audit of every local Underworld battle/location."""
import json
import re
import struct
import xml.etree.ElementTree as ET
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
ASSETS = ROOT / 'Assets'
RES = ASSETS / 'Resources'


def plist_frames(path):
    result = {}
    root = ET.parse(path).getroot().find('dict')
    entries = list(root)
    for i, e in enumerate(entries[:-1]):
        if e.tag == 'key' and e.text == 'frames':
            values = list(entries[i + 1])
            for j in range(0, len(values) - 1, 2):
                pairs = list(values[j + 1])
                result[Path(values[j].text).stem] = {
                    pairs[k].text: (pairs[k + 1].text or pairs[k + 1].tag)
                    for k in range(0, len(pairs) - 1, 2)
                }
    return result


def audit():
    stages = ET.parse(ASSETS / 'xml/raid_stages_default.xml').getroot()
    locations = {}
    for battle in stages.iter('Battle'):
        for node in [battle, *battle.iter('Fight')]:
            location = node.get('Location')
            if location:
                locations.setdefault(location, []).append(battle.get('Name'))
    issues = []
    details = []
    for location, battles in sorted(locations.items()):
        folder = RES / 'textures/locations' / location
        candidates = list((ASSETS / 'xml/locations' / location).glob('*params.xml'))
        if not candidates:
            candidates = list((RES / 'gamedata/locations' / location).glob('params.*'))
        if not candidates:
            issues.append(f'{location}: missing params')
            continue
        root = ET.parse(candidates[0]).getroot()
        missing, metadata = [], []
        images = 0
        for layer in root.findall('Layer'):
            layer_folder = RES / 'textures' / layer.get('Path', f'locations/{location}').strip('/')
            atlas = layer.get('Atlas', '')
            frame_path = layer_folder / (atlas + '_xml.txt')
            frames = plist_frames(frame_path) if frame_path.exists() else {}
            if frame_path.exists():
                png = layer_folder / (atlas + '.png')
                texture_size = None
                if png.exists():
                    with png.open('rb') as stream:
                        texture_size = struct.unpack('>II', stream.read(24)[16:24])
                for name, frame in frames.items():
                    source = frame.get('sourceSize', '')
                    if not re.fullmatch(r'\{\s*[-+\d.eE]+\s*,\s*[-+\d.eE]+\s*\}', source):
                        metadata.append(f'{atlas}/{name}: sourceSize={source}')
                    rect = [int(v) for v in re.findall(r'-?\d+', frame.get('frame', ''))]
                    if texture_size and len(rect) == 4:
                        x, y, w, h = rect
                        if frame.get('rotated') == 'true':
                            w, h = h, w
                        if min(x, y) < 0 or min(w, h) <= 0 or x + w > texture_size[0] or y + h > texture_size[1]:
                            metadata.append(f'{atlas}/{name}: frame outside texture {texture_size}: {rect}')
            for image in list(layer.findall('Image')) + list(layer.findall('SpriteMask')) + list(layer.findall("SimpleEffect[@Type='Picture']")):
                images += 1
                name = image.get('ClassName', '')
                if not ((layer_folder / (name + '.asset')).exists() or
                        (layer_folder / (name + '.png')).exists() or name in frames):
                    missing.append(f'{atlas}/{name}')
            for effect in layer.findall("SimpleEffect[@Type='Sequention']"):
                path = effect.get('Path')
                effect_folder = RES / 'textures' / path if path else (
                    RES / 'textures/Location_effects' if effect.get('PictureLocation') == 'global' else layer_folder)
                effect_xml = effect_folder / 'Atlases' / (effect.get('ClassName', '') + '_xml.txt')
                if not effect_xml.exists():
                    missing.append('sequence:' + str(effect_xml.relative_to(RES)))
            for particle in list(layer.findall('ParticleEffect')) + list(layer.findall('NewParticleEffect')):
                prefab = RES / 'Textures/Location_effects/Particles' / (particle.get('ClassName') + '.prefab')
                if not prefab.exists():
                    missing.append('particle:' + particle.get('ClassName'))
        details.append({'location': location, 'params': str(candidates[0].relative_to(ROOT)),
                        'images': images, 'missing': sorted(set(missing)),
                        'malformed_metadata': len(metadata)})
        issues.extend(f'{location}: missing {m}' for m in sorted(set(missing)))
        issues.extend(f'{location}: {m}' for m in metadata)
    return {'battles': len(list(stages.iter('Battle'))), 'locations': details, 'issues': issues}


if __name__ == '__main__':
    print(json.dumps(audit(), indent=2))
