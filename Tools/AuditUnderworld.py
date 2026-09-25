"""Read-only dependency audit of every local Underworld battle/location."""
import json
import hashlib
import re
import struct
import subprocess
import xml.etree.ElementTree as ET
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
ASSETS = ROOT / 'Assets'
RES = ASSETS / 'Resources'
CATALOG = RES / 'SF2Content/Art/catalog.json'
BUNDLE = ASSETS / 'StreamingAssets/SF2Content/ArtBundles/CORE_LOCATIONS.tar.lz4'
PACKER = ROOT / 'Tools/AssetPacker/bin/Release/net9.0/AssetPacker.dll'


def packaged_sprites(catalog):
    """Read the installed bundle's real sprite names, including atlas members."""
    group = next(part for part in catalog['bundles'] if part['name'] == 'CORE_LOCATIONS')
    with BUNDLE.open('rb') as stream:
        digest = hashlib.file_digest(stream, 'sha256').hexdigest()
    if digest != group['sha256']:
        raise ValueError('CORE_LOCATIONS differs from the packaged-art catalog')
    cache = ROOT / 'Library/UnderworldAudit' / digest
    marker = cache / 'extracted.sha256'
    if not marker.is_file() or marker.read_text(encoding='ascii') != digest:
        cache.mkdir(parents=True, exist_ok=True)
        subprocess.run(['dotnet', str(PACKER), 'extract', str(BUNDLE), str(cache)],
                       check=True, capture_output=True, text=True)
        marker.write_text(digest, encoding='ascii')
    sprites = set()
    for path in (cache / 'assets').glob('*.meta'):
        fields = dict(line.split('=', 1) for line in path.read_text(encoding='utf-8').splitlines()
                      if '=' in line)
        if fields.get('type') == 'sprite':
            sprites.add((fields['address'].replace('\\', '/').lower(), fields['name'].lower()))
    return sprites


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
    # The active DE128 package is generated from this reviewed 76-battle source;
    # the canonical vanilla file contains only three ordinary raid battles.
    stages = ET.parse(ROOT / 'ResearchSources/de128_assets/gamedata/raid_stages_default.xml').getroot()
    # LocationSpriteCache resolves loose Resources first and then the packaged
    # art catalog. The upscaled DE locations are in CORE_LOCATIONS, not as loose
    # Resources files, so checking that directory alone invents missing art.
    catalog = json.loads(CATALOG.read_text(encoding='utf-8'))
    bundled = packaged_sprites(catalog)
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
        candidates = list((ASSETS / 'vanillaXml/locations' / location).glob('*params.xml'))
        if not candidates:
            candidates = list((RES / 'gamedata/locations' / location).glob('params.*'))
        if not candidates:
            issues.append(f'{location}: missing params')
            continue
        root = ET.parse(candidates[0]).getroot()
        missing, metadata = [], []
        images = loose_images = bundled_images = 0
        for layer in root.findall('Layer'):
            relative_folder = layer.get('Path', f'locations/{location}').strip('/')
            layer_folder = RES / 'textures' / relative_folder
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
                if ((layer_folder / (name + '.asset')).exists() or
                        (layer_folder / (name + '.png')).exists() or name in frames):
                    loose_images += 1
                elif (((f'textures/{relative_folder}/{atlas}'.lower(), name.lower()) in bundled
                       if atlas else False) or
                      (f'textures/{relative_folder}/{name}'.lower(), name.lower()) in bundled):
                    bundled_images += 1
                else:
                    missing.append(f'{atlas}/{name}')
            for effect in layer.findall("SimpleEffect[@Type='Sequention']"):
                path = effect.get('Path')
                effect_folder = RES / 'textures' / path if path else (
                    RES / 'textures/Location_effects' if effect.get('PictureLocation') == 'global' else layer_folder)
                effect_xml = effect_folder / 'Atlases' / (effect.get('ClassName', '') + '_xml.txt')
                effect_address = ('textures/' + (path or ('Location_effects' if effect.get('PictureLocation') == 'global'
                                                     else relative_folder)).strip('/') +
                                  '/atlases/' + effect.get('ClassName', '')).lower()
                if not effect_xml.exists() and not any(address == effect_address for address, _ in bundled):
                    missing.append('sequence:' + str(effect_xml.relative_to(RES)))
            for particle in list(layer.findall('ParticleEffect')) + list(layer.findall('NewParticleEffect')):
                prefab = RES / 'Textures/Location_effects/Particles' / (particle.get('ClassName') + '.prefab')
                if not prefab.exists():
                    missing.append('particle:' + particle.get('ClassName'))
        details.append({'location': location, 'params': str(candidates[0].relative_to(ROOT)),
                        'images': images, 'loose_images': loose_images,
                        'bundled_images': bundled_images, 'missing': sorted(set(missing)),
                        'malformed_metadata': len(metadata)})
        issues.extend(f'{location}: missing {m}' for m in sorted(set(missing)))
        issues.extend(f'{location}: {m}' for m in metadata)
    return {'battles': len(list(stages.iter('Battle'))), 'locations': details, 'issues': issues}


if __name__ == '__main__':
    print(json.dumps(audit(), indent=2))
