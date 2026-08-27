"""Recover exact XML dependencies and repair the previous raid atlas export.

Defaults to a read-only report. --write repairs generated metadata and adds
missing textures; existing art and its GUIDs are preserved.
"""
import argparse
import json
import re
import uuid
import zipfile
import struct
import xml.etree.ElementTree as ET
from pathlib import Path

import ExtractRaidArt as recovery
from AuditUnderworld import audit, plist_frames, ASSETS, RES


def recover_reference_art(write):
    """The DE mod ships loose location PNGs outside Unity's Resources tree."""
    added = []
    locations = {x['location'] for x in audit()['locations']}
    with zipfile.ZipFile(ASSETS.parent / 'ResearchSources/com.sf2.de_1.0.6.apk') as apk:
        prefix = 'assets/sfml_data/mod/Textures/Locations/'
        for entry in apk.namelist():
            if not entry.startswith(prefix) or not entry.endswith('.png'):
                continue
            relative = Path(entry[len(prefix):])
            if relative.parts[0] not in locations:
                continue
            target = RES / 'Textures/Locations' / relative
            if target.exists():
                continue
            added.append(str(target.relative_to(RES)))
            if write:
                recovery.write_file(target, apk.read(entry))
                meta = recovery.png_meta(uuid.uuid4().hex).replace('spriteMode: 2', 'spriteMode: 1').replace('textureCompression: 2', 'textureCompression: 0').replace('maxTextureSize: 2048', 'maxTextureSize: 4096')
                Path(str(target) + '.meta').write_text(meta, encoding='utf-8')
    reference = ASSETS.parent / 'ResearchSources/ReferenceSF2DE106/ExportedProject/Assets/Resources/textures/locations'
    # Newer dojo members coexist with the legacy atlas; never overwrite the
    # old atlas under the same name or its old story-scene sprite rectangles.
    for loc in ['dojo', 'dojo_india24']:
        source_folder = reference / loc
        texture_map = {recovery.read_guid(Path(str(p) + '.meta')): p for p in source_folder.glob('*.png')}
        for source in source_folder.glob('*.asset'):
            if source.stem.endswith('_0'):
                continue
            target = RES / 'Textures/locations' / loc / source.name
            if target.exists():
                continue
            body = source.read_text(encoding='utf-8')
            match = re.search(r'texture: \{fileID: \d+, guid: ([a-f0-9]+), type: 3\}', body)
            if not match or match[1] not in texture_map:
                continue
            png_source = texture_map[match[1]]
            png_target = target.parent / ('reference_' + png_source.name)
            if write:
                recovery.write_file(png_target, png_source.read_bytes())
                guid = recovery.read_guid(Path(str(png_target) + '.meta'))
                recovery.write_file(target, body.replace(match[1], guid))
            added.append(str(target.relative_to(RES)))
    return added


def repair_ui_geometry(write):
    """Reject old unsafe generated records instead of guessing Unity's schema."""
    unsafe = []
    for target in (RES / 'ui').rglob('*.asset'):
        body = target.read_text(encoding='utf-8')
        if '  m_WireWidth:' in body or ('m_Name: "' in body and
                'm_RenderDataKey:' not in body and 'm_SpriteID:' in body):
            unsafe.append(str(target.relative_to(RES)))
    if unsafe and write:
        raise RuntimeError("Unsafe sprites require Unity-native rebuilding, not text repair. "
                           "See Tools/SPRITE_NATIVE_REBUILD.md. First: " + unsafe[0])
    return unsafe


def recover_shared_floor_tiles(write):
    # These DE event layouts reuse the legacy dojo's black foreground tiles.
    added = []
    for loc, members in {'dojo_hw21': ['layer_3_1','layer_3_2'],
                         'dojo_american_event_22': ['layer_3_2']}.items():
        for member in members:
            source = RES / 'Textures/locations/dojo' / (member + '.asset')
            target = RES / 'Textures/locations' / loc / source.name
            if not target.exists():
                if write:
                    recovery.write_file(target, source.read_text(encoding='utf-8'))
                added.append(str(target.relative_to(RES)))
    return added


def recover_volcano_particles(write):
    source = ASSETS.parent / 'ResearchSources/ReferenceSF2DE106/ExportedProject/Assets/Resources/textures/location_effects/particles'
    target = RES / 'Textures/Location_effects/Particles'
    added = []
    texture = target / 'reference_spark_fire2_spot.png'
    material = target / 'spark_fire2.mat'
    prefab = target / 'spark_fire2.prefab'
    if not prefab.exists():
        if write:
            recovery.write_file(texture, (source / 'spot.png').read_bytes())
            texture_guid = recovery.read_guid(Path(str(texture) + '.meta'))
            mat_text = (source / 'spark_fire2.mat').read_text(encoding='utf-8')
            mat_text = mat_text.replace(recovery.read_guid(source / 'spot.png.meta'), texture_guid)
            recovery.write_file(material, mat_text)
            mat_guid = recovery.read_guid(Path(str(material) + '.meta'))
            prefab_text = (source / 'spark_fire2.prefab').read_text(encoding='utf-8')
            recovery.write_file(prefab, prefab_text.replace(recovery.read_guid(source / 'spark_fire2.mat.meta'), mat_guid))
        added.append(str(prefab.relative_to(RES)))
    return added


def localize_preview_metadata(write):
    previews = recovery.parse_targets()[1]
    changed = 0
    for path in (RES / 'ui/battles').glob('*.meta'):
        if not any(path.name.lower().startswith(p.lower()) for p in previews):
            continue
        text = path.read_text(encoding='utf-8')
        updated = re.sub(r'(?m)^(  assetBundleName:)[^\r\n]*', r'\1 ', text)
        if updated != text:
            if write:
                path.write_text(updated, encoding='utf-8')
            changed += 1
    return changed


def dependencies():
    targets = set()
    atlasless = []
    for loc in audit()['locations']:
        root = ET.parse(ASSETS.parent / loc['params']).getroot()
        for layer in root.findall('Layer'):
            folder = RES / 'textures' / layer.get('Path', 'locations/' + loc['location']).strip('/')
            atlas = layer.get('Atlas')
            if atlas:
                targets.add((folder, atlas))
            else:
                for img in layer.findall('Image'):
                    name = img.get('ClassName')
                    if not (folder / (name + '.asset')).exists() and not (folder / (name + '.png')).exists():
                        atlasless.append((loc['location'], folder, name))
            for fx in layer.findall("SimpleEffect[@Type='Sequention']"):
                fx_folder = RES / 'textures' / fx.get('Path') if fx.get('Path') else (
                    RES / 'textures/Location_effects' if fx.get('PictureLocation') == 'global' else folder)
                targets.add((fx_folder / 'Atlases', fx.get('ClassName')))
    return sorted(targets), sorted(set(atlasless))


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument('--write', action='store_true')
    ap.add_argument('--reference-only', action='store_true')
    args = ap.parse_args()
    reference_art = recover_reference_art(args.write)
    reference_art += recover_shared_floor_tiles(args.write)
    reference_art += recover_volcano_particles(args.write)
    ui_repairs = repair_ui_geometry(args.write)
    localized = localize_preview_metadata(args.write)
    if args.reference_only:
        print(json.dumps({'reference_art':len(reference_art), 'ui_repairs':len(ui_repairs), 'localized_previews':localized}))
        return
    store = recovery.BundleStore(sorted(recovery.BUNDLE_DIR.iterdir()))
    recovery.STORE = store
    targets, atlasless = dependencies()
    report = {'reference_art': reference_art, 'repaired': [], 'missing': [], 'atlasless': []}
    for folder, atlas in targets:
        bucket = store.textures.get(atlas.lower(), {})
        rec = bucket.get('hi')
        target = folder / (atlas + '_xml.txt')
        png = folder / (atlas + '.png')
        # Only replace metadata produced by the broken exporter, never metadata
        # for an independently recovered/possibly repacked atlas.
        broken = target.exists() and any(v.get('sourceSize', '').startswith('{{') for v in plist_frames(target).values())
        if target.exists() and not broken:
            continue
        if not rec:
            report['missing'].append({'atlas': atlas, 'folder': str(folder.relative_to(RES))})
            continue
        sprites = store.sprites_of(rec['key'])
        rotated = [s.m_Name for s in sprites if int(recovery.sprite_render_data(s).settingsRaw) >> 2 & 15]
        if rotated:
            report['missing'].append({'atlas': atlas, 'unsupported_rotation': rotated})
            continue
        frames = recovery.collect_frames(sprites)
        if not frames:
            report['missing'].append({'atlas': atlas, 'reason': 'no sprite metadata'})
            continue
        if args.write:
            if not png.exists():
                recovery.write_sheet(rec, png)
            recovery.write_plist(target, frames, rec['height'])
        report['repaired'].append(str(target.relative_to(RES)))
    for loc, folder, name in atlasless:
        candidates = []
        for bucket in store.textures.values():
            rec = bucket.get('hi')
            if rec and any(s.m_Name == name for s in store.sprites_of(rec['key'])):
                candidates.append({'texture': rec['name'], 'bundle': rec['bundle']})
        report['atlasless'].append({'location': loc, 'name': name, 'candidates': candidates})
    print(json.dumps({'reference_art': len(reference_art), 'repaired': len(report['repaired']),
                      'missing': report['missing'], 'atlasless': len(report['atlasless'])}, indent=2))
    if args.write:
        report_path = ASSETS.parent / 'Temp/underworld-recovery-report.json'
        report_path.parent.mkdir(exist_ok=True)
        report_path.write_text(json.dumps(report, indent=2), encoding='utf-8')


if __name__ == '__main__':
    main()
