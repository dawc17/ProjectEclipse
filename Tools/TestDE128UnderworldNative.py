#!/usr/bin/env python3
"""Boot the real DE128 Volcano encounter in an independent Unity project."""

from __future__ import annotations

import argparse
import os
import re
import shutil
import subprocess
import sys
from pathlib import Path

from TestCharacterForms import ROOT, owned_native_fixture, prepare_native


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--unity-editor", type=Path, default=Path(r"F:\UnityInstalls\6000.6.0f1\Editor\Unity.exe"))
    parser.add_argument("--reuse-native", type=Path, help="Stopped FormNative-* project owned by this repository")
    parser.add_argument("--profile-tag", default="", help="Optional isolated player-profile suffix for fixture reuse")
    parser.add_argument("--timeout", type=int, default=1200)
    args = parser.parse_args()
    if not args.unity_editor.is_file() or args.timeout < 1 or not re.fullmatch(r"[A-Za-z0-9_-]{0,32}", args.profile_tag):
        parser.error("An installed Unity editor and positive timeout are required.")

    fixture = owned_native_fixture(args.reuse_native) if args.reuse_native else prepare_native(args.unity_editor.resolve())[0]
    if (fixture / "Temp/UnityLockfile").exists():
        raise RuntimeError("The isolated project is already open in Unity.")
    (fixture / "de128-underworld-fixture.marker").write_text("Isolated Underworld encounter acceptance\n", encoding="utf-8")
    shutil.copytree(ROOT / "Mods/de128", fixture / "Mods/de128", dirs_exist_ok=True)
    # Model a profile past the stock movement tutorial. Its quest is included
    # only at load time, so calling the in-game skip later cannot remove it.
    quests = fixture / "Assets/vanillaXml/quests.xml"
    source_quests = (ROOT / "Assets/vanillaXml/quests.xml").read_text(encoding="utf-8-sig")
    tutorial_include = (
        '  <Include File="quest_extensions/tutorial_quests.xml">\n'
        '    <Conditions>\n'
        '      <Equal Value1="_$StoryTutorialStep" Value2="END" Not="1"/>\n'
        '    </Conditions>\n'
        '  </Include>\n'
    )
    if source_quests.count(tutorial_include) != 1:
        raise RuntimeError("The stock tutorial quest include changed; review the native fixture profile.")
    quests.write_text(source_quests.replace(tutorial_include, "", 1), encoding="utf-8")
    validator = fixture / "Assets/Editor/ValidateDE128UnderworldNative.cs"
    validator.parent.mkdir(parents=True, exist_ok=True)
    shutil.copy2(ROOT / "Tools/ValidateDE128UnderworldNative.cs", validator)

    log = fixture / "de128-underworld-native.log"
    command = [str(args.unity_editor.resolve()), "-batchmode", "-projectPath", str(fixture),
               "-executeMethod", "ValidateDE128UnderworldNative.RunEditor", "-logFile", str(log)]
    print(f"Underworld fixture: {fixture}; log: {log}", flush=True)
    environment = dict(os.environ, ECLIPSE_DE128_UNDERWORLD_PROFILE_TAG=args.profile_tag)
    result = subprocess.run(command, cwd=fixture, env=environment, timeout=args.timeout)
    lines = log.read_text(encoding="utf-8", errors="replace").splitlines() if log.exists() else []
    evidence = [line.strip() for line in lines if "[DE128UnderworldNative]" in line or "error CS" in line]
    for line in evidence[-45:]:
        print(line, flush=True)
    if result.returncode != 0 or not any("[DE128UnderworldNative] PASS" in line for line in evidence):
        raise RuntimeError(f"Native Underworld acceptance failed (exit {result.returncode}); inspect {log}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
