"""Prepare an isolated, minimal Unity project for native title sparring acceptance.

Does not launch Unity. Run the saved command separately. No user editor or saves
are touched. The source, catalog and TAR art are copied independently (no links).
"""
import argparse
import json
import shutil
import uuid
from pathlib import Path


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--unity-editor', type=Path, required=True)
    args = parser.parse_args()
    root = Path(__file__).resolve().parent.parent
    fixture = root / 'Temp' / ('TitleSparringNative-' + uuid.uuid4().hex)
    fixture.mkdir(parents=True)
    for name in ['Assets/Scripts', 'Assets/Plugins', 'Assets/vanillaXml',
                 'Assets/StreamingAssets', 'Packages', 'ProjectSettings', 'Library/PackageCache']:
        shutil.copytree(root / name, fixture / name)
    # Boot and native art use canonical XML and the catalog. Loose legacy image,
    # UI and audio dumps are unnecessary for this controlled, HUD-free encounter.
    for name in ['SF2Content', 'shaders', 'EclipseVersus', 'prefabs']:
        shutil.copytree(root / 'Assets/Resources' / name, fixture / 'Assets/Resources' / name)
    for source in (root / 'Assets/Resources/gamedata').rglob('*'):
        if not source.is_file():
            continue
        if source.suffix == '.meta':
            asset = source.with_suffix('')
            if asset.is_file() and asset.suffix not in ['.xml', '.json', '.txt', '.bytes']:
                continue
        elif source.suffix not in ['.xml', '.json', '.txt', '.bytes']:
            continue
        if source.is_file():
            target = fixture / source.relative_to(root)
            target.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(source, target)
    # Project-owned shaders outside Resources retain their serialized identities.
    if (root / 'Assets/Shader').is_dir():
        shutil.copytree(root / 'Assets/Shader', fixture / 'Assets/Shader')
    editor = fixture / 'Assets/Editor'
    editor.mkdir()
    shutil.copy2(root / 'Tools/ValidateTitleSparringNative.cs', editor / 'ValidateTitleSparringNative.cs')
    (fixture / 'Mods').mkdir()
    (fixture / 'title-sparring-fixture.marker').write_text('Isolated title sparring acceptance\n')
    command = [str(args.unity_editor.resolve()), '-batchmode', '-projectPath', str(fixture),
               '-executeMethod', 'ValidateTitleSparringNative.RunEditor', '-logFile', str(fixture / 'validation.log')]
    (fixture / 'command.json').write_text(json.dumps(command, indent=2))
    print(fixture, flush=True)


if __name__ == '__main__':
    main()
