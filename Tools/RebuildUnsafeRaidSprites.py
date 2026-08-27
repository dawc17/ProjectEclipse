"""Prepare/install Unity-native sprite regeneration; never invent sprite YAML.

prepare reads suspect records as text, outside Unity. install requires the
successful isolated Unity rebuild report and preserves all destination GUIDs.
"""
import argparse
import json
import re
import uuid
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
RES = ROOT / 'Assets/Resources'
MANIFEST = ROOT / 'Temp/raid-sprite-rebuild.json'


def prepare():
    textures = {}
    for p in (RES / 'ui').rglob('*.png'):
        guid = re.search(r'^guid: (\w+)', Path(str(p) + '.meta').read_text(), re.M)[1]
        textures[guid] = p
    entries = []
    for p in (RES / 'ui').rglob('*.asset'):
        text = p.read_text(encoding='utf-8')
        if not ('m_Name: "' in text and 'm_RenderDataKey:' not in text and 'm_Bindpose: []' in text and 'm_SpriteID:' in text):
            continue
        rect = text.split('  m_Rect:', 1)[1].split('  m_Offset:', 1)[0]
        coords = {k: float(v) for k,v in re.findall(r'\b(x|y|width|height): ([\d.e+-]+)', rect)}
        guid = re.search(r'texture: \{fileID: 2800000, guid: (\w+),', text)[1]
        entries.append(dict(assetPath=str(p), texturePath=str(textures[guid]), name=p.stem,
                            textureGuid=guid, pixelsPerUnit=100, **coords))
    if len(entries) != 149:
        raise RuntimeError(f'Expected the 149 previously generated records, found {len(entries)}; review scope')
    MANIFEST.parent.mkdir(exist_ok=True)
    MANIFEST.write_text(json.dumps({'entries': entries}, indent=2), encoding='utf-8')
    print(f'Prepared {len(entries)} sprites from {len(set(x["texturePath"] for x in entries))} original textures: {MANIFEST}')


def install():
    original = json.loads(MANIFEST.read_text())['entries']
    rebuilt = json.loads(Path(str(MANIFEST) + '.rebuilt.json').read_text())['entries']
    if len(original) != len(rebuilt):
        raise RuntimeError('Incomplete Unity rebuild')
    replacements = []
    for before, after in zip(original, rebuilt):
        if before['assetPath'] != after['assetPath']:
            raise RuntimeError('Manifest order changed')
        target = Path(before['assetPath']).resolve()
        target.relative_to(RES.resolve())
        text = Path(after['outputPath']).read_text(encoding='utf-8')
        if 'm_RenderDataKey:' not in text or 'm_VertexCount: 4' not in text:
            raise RuntimeError(f'Incomplete native sprite output: {target}')
        text = text.replace('guid: ' + after['generatedTextureGuid'] + ', type: 2',
                            'guid: ' + before['textureGuid'] + ', type: 3')
        text = re.sub(r'(?m)^  m_Name:.*$', '  m_Name: ' + before['name'], text)
        replacements.append((target, text))
    backup = ROOT / 'Temp/unsafe-raid-sprites-before-native-rebuild'
    for target, text in replacements:
        saved = backup / target.relative_to(RES)
        saved.parent.mkdir(parents=True, exist_ok=True)
        if target.exists() and not saved.exists():
            saved.write_bytes(target.read_bytes())
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_text(text, encoding='utf-8')
        meta = Path(str(target) + '.meta')
        if not meta.exists():
            meta.write_text('fileFormatVersion: 2\nguid: ' + uuid.uuid4().hex +
                '\nNativeFormatImporter:\n  externalObjects: {}\n  mainObjectFileID: 21300000\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
    print(f'Installed {len(replacements)} Unity-native sprites; original .meta GUIDs preserved. Backup: {backup}')


def prepare_pending():
    queue = json.loads((ROOT / 'Temp/raid-native-sprite-queue.json').read_text())
    entries = []
    for entry in queue['entries']:
        target = Path(entry['assetPath']).resolve()
        target.relative_to(RES.resolve())
        if not target.exists():
            entries.append(entry)
    if not entries:
        raise RuntimeError('No missing sprites queued; nothing to rebuild')
    MANIFEST.write_text(json.dumps({'entries': entries}, indent=2), encoding='utf-8')
    print(f'Prepared {len(entries)} missing sprites for native generation')


def prepare_assets(paths):
    """Rebuild explicitly selected legacy sprites, retaining their atlas crops."""
    textures = {}
    for p in (RES / 'ui').rglob('*.png'):
        guid = re.search(r'^guid: (\w+)', Path(str(p) + '.meta').read_text(), re.M)[1]
        textures[guid] = p
    entries = []
    for path in paths:
        p = Path(path).resolve()
        p.relative_to(RES.resolve())
        text = p.read_text(encoding='utf-8')
        rect = text.split('  m_Rect:', 1)[1].split('  m_Offset:', 1)[0]
        coords = {k: float(v) for k, v in re.findall(r'\b(x|y|width|height): ([\d.e+-]+)', rect)}
        guid = re.search(r'texture: \{fileID: 2800000, guid: (\w+),', text)[1]
        entries.append(dict(assetPath=str(p), texturePath=str(textures[guid]), name=p.stem,
                            textureGuid=guid, pixelsPerUnit=100, **coords))
    if not entries:
        raise RuntimeError('Explicit sprite asset paths required')
    MANIFEST.parent.mkdir(exist_ok=True)
    MANIFEST.write_text(json.dumps({'entries': entries}, indent=2), encoding='utf-8')
    print(f'Prepared {len(entries)} explicitly selected sprites: {MANIFEST}')


if __name__ == '__main__':
    ap = argparse.ArgumentParser()
    ap.add_argument('mode', choices=['prepare', 'prepare_pending', 'prepare_assets', 'install'])
    ap.add_argument('assets', nargs='*')
    ap.add_argument('--manifest', type=Path, default=MANIFEST)
    args = ap.parse_args()
    MANIFEST = args.manifest.resolve()
    if args.mode == 'prepare_assets':
        prepare_assets(args.assets)
    else:
        globals()[args.mode]()
