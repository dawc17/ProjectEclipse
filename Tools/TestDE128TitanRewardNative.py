#!/usr/bin/env python3
"""Settle, reload and win the five-item Titan reward in an isolated Unity project."""

from __future__ import annotations

import argparse
import os
import re
import shutil
import subprocess
from pathlib import Path

from TestCharacterForms import ROOT, owned_native_fixture, prepare_native


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--unity-editor", type=Path, default=Path(r"F:\UnityInstalls\6000.6.0f1\Editor\Unity.exe"))
    parser.add_argument("--reuse-native", type=Path, help="Stopped FormNative-* project owned by this repository")
    parser.add_argument("--timeout", type=int, default=1200)
    parser.add_argument("--phases", default="grant,reload", help="Comma-separated grant/reload/win phases")
    parser.add_argument("--profile-tag", help="Isolated player-profile suffix")
    args = parser.parse_args()
    phases = args.phases.split(",")
    if not phases or any(phase not in ("grant", "reload", "win") for phase in phases):
        parser.error("--phases accepts grant, reload and win.")
    if "win" in phases and len(phases) != 1:
        parser.error("--phases win requires its own fresh profile run.")
    profile_tag = args.profile_tag or ("titanwin" if "win" in phases else "titanreward")
    if (not args.unity_editor.is_file() or args.timeout < 1 or
            not re.fullmatch(r"[A-Za-z0-9_-]{1,32}", profile_tag)):
        parser.error("An installed Unity editor and positive timeout are required.")
    fixture = owned_native_fixture(args.reuse_native) if args.reuse_native else prepare_native(args.unity_editor.resolve())[0]
    if (fixture / "Temp/UnityLockfile").exists():
        raise RuntimeError("The isolated project is already open in Unity.")
    (fixture / "de128-titan-reward-fixture.marker").write_text("Isolated Titan reward acceptance\n", encoding="utf-8")
    shutil.copytree(ROOT / "Mods/de128", fixture / "Mods/de128", dirs_exist_ok=True)
    for relative in (
        "Assets/Scripts/Assembly-CSharp/ResourceManager.cs",
        "Assets/Scripts/Eclipse/Modding/ModAssetLoader.cs",
        "Assets/Scripts/Eclipse/Modding/ModRuntime.cs",
        "Assets/Scripts/Eclipse/Runtime/Modding/LooseModProvider.cs",
    ):
        shutil.copy2(ROOT / relative, fixture / relative)
    quests = fixture / "Assets/vanillaXml/quests.xml"
    tutorial = (
        '  <Include File="quest_extensions/tutorial_quests.xml">\n'
        '    <Conditions>\n'
        '      <Equal Value1="_$StoryTutorialStep" Value2="END" Not="1"/>\n'
        '    </Conditions>\n'
        '  </Include>\n'
    )
    stock = (ROOT / "Assets/vanillaXml/quests.xml").read_text(encoding="utf-8-sig")
    if stock.count(tutorial) != 1:
        raise RuntimeError("The stock tutorial include changed; review the native fixture profile.")
    quests.write_text(stock.replace(tutorial, "", 1), encoding="utf-8")
    cache = fixture / "Assets/Scripts/Eclipse/Content/TarAssets/Lz4BundleCache.cs"
    cache_text = (ROOT / "Assets/Scripts/Eclipse/Content/TarAssets/Lz4BundleCache.cs").read_text(encoding="utf-8-sig")
    default_root = "string cacheRoot = Path.Combine(Application.persistentDataPath, CacheDirectoryName);"
    fixture_root = ('string cacheRoot = Path.Combine(Directory.GetParent(Application.dataPath).FullName, '
                    '"Temp", "DE128TarAssetCache");')
    if cache_text.count(default_root) != 1:
        raise RuntimeError("The native TAR cache root changed; review fixture isolation.")
    cache.write_text(cache_text.replace(default_root, fixture_root, 1), encoding="utf-8")
    validator = fixture / "Assets/Editor/ValidateDE128TitanRewardNative.cs"
    validator.parent.mkdir(parents=True, exist_ok=True)
    shutil.copy2(ROOT / "Tools/ValidateDE128TitanRewardNative.cs", validator)
    if "win" in phases:
        shutil.copy2(ROOT / "Tools/ValidateDE128TitanWinNative.cs",
                     fixture / "Assets/Editor/ValidateDE128TitanWinNative.cs")

    for phase in phases:
        log = fixture / f"de128-titan-reward-{phase}.log"
        validator_class = "ValidateDE128TitanWinNative" if phase == "win" else "ValidateDE128TitanRewardNative"
        prefix = "[DE128TitanWinNative]" if phase == "win" else "[DE128TitanRewardNative]"
        command = [str(args.unity_editor.resolve()), "-batchmode", "-projectPath", str(fixture),
                   "-executeMethod", validator_class + ".RunEditor", "-logFile", str(log)]
        environment = dict(os.environ, ECLIPSE_DE128_TITAN_PHASE=phase,
                           ECLIPSE_DE128_TITAN_PROFILE_TAG=profile_tag)
        print(f"Titan {phase} fixture: {fixture}; log: {log}", flush=True)
        result = subprocess.run(command, cwd=fixture, env=environment, timeout=args.timeout)
        lines = log.read_text(encoding="utf-8", errors="replace").splitlines() if log.exists() else []
        evidence = [line.strip() for line in lines if prefix in line or "error CS" in line]
        for line in evidence[-30:]:
            print(line, flush=True)
        success = "PASS:" if phase == "win" else "PASS " + phase + ":"
        if result.returncode != 0 or not any(prefix + " " + success in line
                                                 for line in evidence):
            raise RuntimeError(f"Native Titan reward {phase} failed (exit {result.returncode}); inspect {log}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
