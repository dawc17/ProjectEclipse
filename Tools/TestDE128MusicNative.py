#!/usr/bin/env python3
"""Decode every packaged DE128 Underworld and campaign track through Unity.

Creates an independent Unity project and removes that owned fixture after Unity
exits. No test files are written into the source project.
"""

from __future__ import annotations

import argparse
import os
import re
import shutil
import subprocess
import sys
import tempfile
from pathlib import Path

from TestCharacterForms import ROOT, native_fixture_root


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--unity-editor", type=Path,
                        default=Path(r"F:\UnityInstalls\6000.6.0f1\Editor\Unity.exe"))
    parser.add_argument("--timeout", type=int, default=1200)
    args = parser.parse_args()
    if not args.unity_editor.is_file() or args.timeout < 1:
        parser.error("An installed Unity editor and positive timeout are required.")

    fixture = None
    process = None
    try:
        fixture_root = native_fixture_root().resolve()
        fixture_root.mkdir(parents=True, exist_ok=True)
        fixture = Path(tempfile.mkdtemp(prefix="FormNative-", dir=fixture_root))
        (fixture / "de128-music-native-fixture.marker").write_text(
            "Owned DE128 music native fixture\n", encoding="utf-8")
        for name in ("Assets", "Packages", "ProjectSettings"):
            shutil.copytree(ROOT / name, fixture / name)
        cache = ROOT / "Library/PackageCache"
        if cache.is_dir():
            shutil.copytree(cache, fixture / "Library/PackageCache")
        shutil.copytree(ROOT / "Mods/de128", fixture / "Mods/de128")
        shutil.copy2(ROOT / "Tools/ValidateDE128MusicNative.cs",
                     fixture / "Assets/Editor/ValidateDE128MusicNative.cs")
        log = fixture / "de128-music-native.log"
        command = [str(args.unity_editor.resolve()), "-batchmode", "-projectPath", str(fixture),
                   "-executeMethod", "ValidateDE128MusicNative.RunEditor", "-logFile", str(log)]
        print("Music fixture: " + str(fixture), flush=True)
        process = subprocess.Popen(command, cwd=fixture, env=os.environ.copy())
        try:
            process.wait(timeout=args.timeout)
        except subprocess.TimeoutExpired:
            process.terminate()
            try:
                process.wait(timeout=20)
            except subprocess.TimeoutExpired:
                process.kill()
                process.wait()
            raise TimeoutError("Unity music validation exceeded its timeout")
        output = log.read_text(encoding="utf-8", errors="replace") if log.is_file() else ""
        evidence = [line for line in output.splitlines()
                    if "[DE128MusicNative]" in line or re.search(r"error CS\d+", line)]
        for line in evidence[-35:]:
            print(line, flush=True)
        if process.returncode or "[DE128MusicNative] PASS:" not in output or "[DE128MusicNative] FAIL:" in output:
            print("Unity music validation failed (exit " + str(process.returncode) + ")", file=sys.stderr)
            return 1
        return 0
    finally:
        if process is not None and process.poll() is None:
            process.terminate()
            process.wait(timeout=20)
        if fixture is not None:
            owned_root = native_fixture_root().resolve()
            target = fixture.resolve()
            marker = target / "de128-music-native-fixture.marker"
            if target.parent != owned_root or not target.name.startswith("FormNative-") or \
                    not marker.is_file() or marker.read_text(encoding="utf-8").strip() != "Owned DE128 music native fixture":
                raise RuntimeError("Refusing to clean an unverified native fixture: " + str(target))
            shutil.rmtree(target)
            print("Removed music fixture: " + str(target), flush=True)


if __name__ == "__main__":
    raise SystemExit(main())
