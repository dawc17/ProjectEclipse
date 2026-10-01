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
    focused = parser.add_mutually_exclusive_group()
    focused.add_argument('--mesh-culture', help='Run the focused title mesh regression with this numeric culture (for example hr-HR).')
    focused.add_argument('--rollback-benchmark', action='store_true', help='Measure real rig snapshot capture and verify restores against an independent reflection walk.')
    parser.add_argument('--build-player', action='store_true', help='Prepare an IL2CPP benchmark build command instead of an editor run (requires --rollback-benchmark).')
    args = parser.parse_args()
    if args.build_player and not args.rollback_benchmark:
        parser.error('--build-player requires --rollback-benchmark')
    root = Path(__file__).resolve().parents[3]
    fixture = root / 'Temp' / ('TitleSparringNative-' + uuid.uuid4().hex)
    fixture.mkdir(parents=True)
    for name in ['Assets/Scripts', 'Assets/Plugins', 'Assets/vanillaXml',
                 'Assets/StreamingAssets', 'Packages', 'ProjectSettings', 'Library/PackageCache']:
        shutil.copytree(root / name, fixture / name)
    # Boot and native art use canonical XML and the catalog. Loose legacy image,
    # UI and audio dumps are unnecessary for this controlled, HUD-free encounter.
    for name in ['SF2Content', 'shaders', 'EclipseVersus', 'prefabs']:
        shutil.copytree(root / 'Assets/Resources' / name, fixture / 'Assets/Resources' / name)
    if args.mesh_culture or args.rollback_benchmark:
        # Boot needs fonts; the real title CPUs also emit native hit effects.
        for name in ['UI/Fonts', 'textures/effects/fight']:
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
    # A focused showcase check uses real native animation audio on private sources.
    for name in ['snd_swish1', 'snd_swish2']:
        for suffix in ['.ogg', '.ogg.meta']:
            source = root / 'Assets/Resources/gamedata/sounds' / (name + suffix)
            target = fixture / source.relative_to(root)
            target.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(source, target)
    # Project-owned shaders outside Resources retain their serialized identities.
    if (root / 'Assets/Shader').is_dir():
        shutil.copytree(root / 'Assets/Shader', fixture / 'Assets/Shader')
    editor = fixture / 'Assets/Editor'
    editor.mkdir()
    validator = ('ValidateRollbackPerformanceNative' if args.rollback_benchmark else
                 'ValidateTitleMeshNative' if args.mesh_culture else 'ValidateTitleSparringNative')
    # The rollback fixture needs access to internal game policy and also runs in a player.
    destination = fixture / 'Assets/Scripts' if args.rollback_benchmark else editor
    shutil.copy2(Path(__file__).resolve().parent / (validator + '.cs'), destination / (validator + '.cs'))
    (fixture / 'Mods').mkdir()
    # Exercise the installed cinematic mod's hit/KO sounds as well as native
    # combat; preview fights must not activate its global screen triggers.
    if not args.mesh_culture and not args.rollback_benchmark:
        shutil.copytree(root / 'Mods/chiaroscuro', fixture / 'Mods/chiaroscuro')
    (fixture / 'title-sparring-fixture.marker').write_text('Isolated title sparring acceptance\n')
    command = [str(args.unity_editor.resolve()), '-batchmode', '-projectPath', str(fixture),
               '-executeMethod', validator + ('.BuildPlayer' if args.build_player else '.RunEditor'),
               '-logFile', str(fixture / 'validation.log')]
    if args.mesh_culture:
        command.extend(['-titleMeshCulture', args.mesh_culture])
    (fixture / 'command.json').write_text(json.dumps(command, indent=2))
    if args.build_player:
        player = [str(fixture / 'BenchmarkPlayer/RollbackBenchmark.exe'), '-batchmode',
                  '-rollback-benchmark', '-logFile', str(fixture / 'player-validation.log')]
        (fixture / 'player-command.json').write_text(json.dumps(player, indent=2))
    print(fixture, flush=True)


if __name__ == '__main__':
    main()
