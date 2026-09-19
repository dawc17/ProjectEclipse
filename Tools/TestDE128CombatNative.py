#!/usr/bin/env python3
"""Boot the actual DE128 package and native input/animation in an independent Unity copy.

Does not alter the working editor or its saves. Evidence stays under Temp/FormNative-*.
Uses the existing independent-copy preparer; no hardlinks, shared writable caches,
or cleanup of prior evidence. Requires the project's exact Unity version.
"""
import argparse
import hashlib
import json
import os
import shutil
import subprocess
import time
from pathlib import Path
from TestCharacterForms import ROOT, prepare_native, owned_native_fixture


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--unity-editor', type=Path, required=True)
    parser.add_argument('--spell', choices=('Sphere1', 'Sphere2', 'Sphere3', 'ComboSphere3'), default='Sphere1', help='Native spell lifecycle to exercise after Jian.')
    parser.add_argument('--timeout', type=int, default=1200)
    parser.add_argument('--reuse-native', type=Path, help='Stopped marked DE128 clone; preserve prior logs and back up synced inputs.')
    parser.add_argument('--sync-native-source', action='append', default=[], help='Repository-relative Assets or Mods/de128 file to refresh in a reused clone.')
    args = parser.parse_args()
    if not args.unity_editor.is_file() or args.timeout < 1:
        parser.error('An installed Unity executable and positive timeout are required.')
    if args.sync_native_source and not args.reuse_native:
        parser.error('--sync-native-source requires --reuse-native')
    if args.reuse_native:
        fixture = owned_native_fixture(args.reuse_native)
        marker = fixture / 'de128-native-fixture.marker'
        if not marker.is_file() or marker.read_text(encoding='utf-8').strip() != 'Isolated DE128 native combat acceptance':
            raise ValueError('Not an owned DE128 native clone.')
        import tempfile
        runs = fixture / 'DE128Runs'
        runs.mkdir(exist_ok=True)
        evidence = Path(tempfile.mkdtemp(prefix='Run-', dir=runs))
    else:
        fixture, _ = prepare_native(args.unity_editor.resolve())
        shutil.copytree(ROOT / 'Mods/de128', fixture / 'Mods/de128')
        shutil.copytree(ROOT / 'Tools/Fixtures/de128-combat', fixture / 'Mods/fixture.de128-combat')
        (fixture / 'de128-native-fixture.marker').write_text('Isolated DE128 native combat acceptance\n', encoding='utf-8')
        evidence = fixture
    pairs = [(ROOT / 'Tools/ValidateDE128CombatNative.cs', Path('Assets/Editor/ValidateDE128CombatNative.cs'))]
    pairs += [(path, Path('Mods/fixture.de128-combat') / path.relative_to(ROOT / 'Tools/Fixtures/de128-combat'))
              for path in (ROOT / 'Tools/Fixtures/de128-combat').rglob('*') if path.is_file()]
    for name in args.sync_native_source:
        relative = Path(name)
        source = (ROOT / relative).resolve()
        if relative.is_absolute() or '..' in relative.parts or not source.is_relative_to(ROOT) or not source.is_file():
            raise ValueError(f'Invalid sync source: {name}')
        if relative.parts[0] != 'Assets' and relative.parts[:2] != ('Mods', 'de128'):
            raise ValueError(f'Unsupported sync source: {name}')
        pairs.append((source, relative))
    changes = []
    for source, relative in pairs:
        target = fixture / relative
        if not target.resolve().is_relative_to(fixture) or target.is_symlink():
            raise ValueError(f'Unsafe sync destination: {target}')
        if args.reuse_native and target.exists():
            backup = evidence / 'before' / relative
            backup.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(target, backup)
        target.parent.mkdir(parents=True, exist_ok=True)
        shutil.copy2(source, target)
        changes.append({'path':str(relative), 'sha256':hashlib.sha256(source.read_bytes()).hexdigest()})
    (evidence / 'inputs.json').write_text(json.dumps(changes, indent=2), encoding='utf-8')
    log = evidence / 'de128-validation.log'
    command = [str(args.unity_editor.resolve()), '-batchmode', '-projectPath', str(fixture),
               '-executeMethod', 'ValidateDE128CombatNative.RunEditor', '-logFile', str(log)]
    (evidence / 'spell.json').write_text(json.dumps({'spell': args.spell}), encoding='utf-8')
    (evidence / 'de128-command.json').write_text(json.dumps(command, indent=2), encoding='utf-8')
    with (evidence / 'de128-launcher.log').open('x', encoding='utf-8') as output:
        environment = dict(os.environ, ECLIPSE_DE128_TEST_SPELL=args.spell)
        process = subprocess.Popen(command, cwd=fixture, stdout=output, stderr=subprocess.STDOUT, env=environment,
                                   creationflags=getattr(subprocess, 'CREATE_NO_WINDOW', 0))
        print(f'DE128 native process {process.pid}; log: {log}', flush=True)
        start = time.monotonic()
        offset = 0
        try:
            while process.poll() is None:
                try:
                    process.wait(timeout=15)
                except subprocess.TimeoutExpired:
                    pass
                if log.exists():
                    with log.open(encoding='utf-8', errors='replace') as stream:
                        stream.seek(offset)
                        for line in stream:
                            if '[DE128Native]' in line or 'error CS' in line:
                                print(line.rstrip(), flush=True)
                        offset = stream.tell()
                if time.monotonic() - start > args.timeout:
                    raise TimeoutError(f'Native run exceeded {args.timeout}s; evidence: {log}')
                print(f'DE128 native process status={process.poll()}, elapsed={int(time.monotonic()-start)}s', flush=True)
        except BaseException:
            if process.poll() is None:
                process.terminate()
                try:
                    process.wait(timeout=10)
                except subprocess.TimeoutExpired:
                    process.kill()
                    process.wait()
            raise
    text = log.read_text(encoding='utf-8', errors='replace') if log.exists() else ''
    if process.returncode != 0 or '[DE128Native] PASS:' not in text or '[DE128Native] FAIL:' in text:
        raise RuntimeError(f'Native acceptance failed (exit {process.returncode}); inspect {log}')
    print(f'PASS DE128 native acceptance: {log}', flush=True)


if __name__ == '__main__':
    main()
