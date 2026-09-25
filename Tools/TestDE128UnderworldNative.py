#!/usr/bin/env python3
"""Boot real DE128 Underworld encounters in an independent Unity project."""

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
    parser.add_argument("--tier-bosses", action="store_true", help="Play one native boss encounter from each Underworld tier")
    parser.add_argument("--story-bosses", action="store_true", help="Play the 32 boss fights with archived story sequences")
    parser.add_argument("--all-fights", action="store_true", help="Play all 76 native Underworld fights")
    parser.add_argument("--fight", help="Play one exact de128:fights/uw_* ID")
    parser.add_argument("--win", action="store_true", help="Complete --fight through the native victory, reward and map path")
    parser.add_argument("--wasp-wave", action="store_true", help="Reach the fourth Demon survival fighter and observe her Fly ability")
    parser.add_argument("--butcher-wave", action="store_true", help="Reach the third Demon survival fighter and observe Earthquake")
    parser.add_argument("--hermit-wave", action="store_true", help="Reach the second Demon survival fighter and observe Storm")
    parser.add_argument("--hermit-victory", action="store_true", help="Defeat the player after Hermit's Storm and observe the authored victory move")
    parser.add_argument("--mercenary-wave", action="store_true", help="Reach Girl Fan in Mercenary survival and validate her native equipment")
    parser.add_argument("--fights", help="Comma-separated exact de128:fights/uw_* IDs to play in one native run")
    parser.add_argument("--require-titan-equipment", action="store_true",
                        help="Assert the saved Titan reward set is equipped on the native fighter")
    parser.add_argument("--timeout", type=int, default=1200)
    args = parser.parse_args()
    if not args.unity_editor.is_file() or args.timeout < 1 or not re.fullmatch(r"[A-Za-z0-9_-]{0,32}", args.profile_tag):
        parser.error("An installed Unity editor and positive timeout are required.")
    if sum((args.tier_bosses, args.story_bosses, args.all_fights, bool(args.fight), bool(args.fights))) > 1:
        parser.error("Choose one encounter matrix at a time.")
    if args.fight and not re.fullmatch(r"de128:fights/uw_[a-z0-9_]+", args.fight):
        parser.error("--fight requires an exact de128:fights/uw_* ID.")
    if args.fights:
        selected = args.fights.split(",")
        if (len(selected) > 76 or len(set(selected)) != len(selected) or
                any(not re.fullmatch(r"de128:fights/uw_[a-z0-9_]+", value) for value in selected)):
            parser.error("--fights requires 1–76 distinct exact de128:fights/uw_* IDs.")
    if args.require_titan_equipment and not args.fight:
        parser.error("--require-titan-equipment requires one exact --fight ID.")
    if args.win and not args.fight:
        parser.error("--win requires one exact --fight ID.")
    if args.wasp_wave and (args.fight != "de128:fights/uw_survival_demon_1" or args.win):
        parser.error("--wasp-wave requires --fight de128:fights/uw_survival_demon_1 without --win.")
    if args.butcher_wave and (args.fight != "de128:fights/uw_survival_demon_1" or args.win):
        parser.error("--butcher-wave requires --fight de128:fights/uw_survival_demon_1 without --win.")
    if args.hermit_wave and (args.fight != "de128:fights/uw_survival_demon_1" or args.win):
        parser.error("--hermit-wave requires --fight de128:fights/uw_survival_demon_1 without --win.")
    if args.hermit_victory and (args.fight != "de128:fights/uw_survival_demon_1" or args.win):
        parser.error("--hermit-victory requires --fight de128:fights/uw_survival_demon_1 without --win.")
    if args.mercenary_wave and (args.fight != "de128:fights/uw_survival_mercenary_1" or args.win):
        parser.error("--mercenary-wave requires --fight de128:fights/uw_survival_mercenary_1 without --win.")
    if sum((args.wasp_wave, args.butcher_wave, args.hermit_wave, args.hermit_victory, args.mercenary_wave)) > 1:
        parser.error("Choose one survival wave acceptance at a time.")

    fixture = owned_native_fixture(args.reuse_native) if args.reuse_native else prepare_native(args.unity_editor.resolve())[0]
    if (fixture / "Temp/UnityLockfile").exists():
        raise RuntimeError("The isolated project is already open in Unity.")
    (fixture / "de128-underworld-fixture.marker").write_text("Isolated Underworld encounter acceptance\n", encoding="utf-8")
    shutil.copytree(ROOT / "Mods/de128", fixture / "Mods/de128", dirs_exist_ok=True)
    # Reused fixtures must exercise the current implementation of the resolver
    # and loose model loader, not source captured by an older fixture creation.
    for relative in (
        "Assets/Scripts/Assembly-CSharp/ListSF.cs",
        "Assets/Scripts/Assembly-CSharp/ResourceManager.cs",
        "Assets/Scripts/Assembly-CSharp/IntervalAttack.cs",
        "Assets/Scripts/Assembly-CSharp/ModelAi.cs",
        "Assets/Scripts/Eclipse/Modding/LegacyContentAdapterP1D.cs",
        "Assets/Scripts/Eclipse/Modding/ModAssetLoader.cs",
        "Assets/Scripts/Eclipse/Modding/ModRuntime.cs",
        "Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeP1D.cs",
        "Assets/Scripts/Eclipse/Runtime/Modding/ModContentP1D.cs",
        "Assets/Scripts/Eclipse/Runtime/Modding/ModMovePerkLocks.cs",
        "Assets/Scripts/Eclipse/Runtime/Modding/ModSaveData.cs",
        "Assets/Scripts/Eclipse/Runtime/Modding/LooseModProvider.cs",
    ):
        shutil.copy2(ROOT / relative, fixture / relative)
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
    # The test creates many isolated player profiles. Keep their decompressed
    # bundles in this disposable project copy instead of repeating them in
    # Unity's per-profile LocalLow cache on C:. Decoding and validation stay native.
    cache_source = ROOT / "Assets/Scripts/Eclipse/Content/TarAssets/Lz4BundleCache.cs"
    cache = fixture / "Assets/Scripts/Eclipse/Content/TarAssets/Lz4BundleCache.cs"
    cache_text = cache_source.read_text(encoding="utf-8-sig")
    default_root = "string cacheRoot = Path.Combine(Application.persistentDataPath, CacheDirectoryName);"
    fixture_root = (
        'string cacheRoot = Path.Combine(Directory.GetParent(Application.dataPath).FullName, '
        '"Temp", "DE128TarAssetCache");'
    )
    if cache_text.count(default_root) != 1:
        raise RuntimeError("The native TAR cache root changed; review fixture isolation.")
    cache.write_text(cache_text.replace(default_root, fixture_root, 1), encoding="utf-8")
    matrix = args.tier_bosses or args.story_bosses or args.all_fights or args.fight or args.fights
    validator_name = "ValidateDE128UnderworldWinNative" if args.win else (
        "ValidateDE128TierBossesNative" if matrix else "ValidateDE128UnderworldNative")
    prefix = "[DE128UnderworldWinNative]" if args.win else (
        "[DE128TierBossesNative]" if matrix else "[DE128UnderworldNative]")
    validator = fixture / f"Assets/Editor/{validator_name}.cs"
    validator.parent.mkdir(parents=True, exist_ok=True)
    shutil.copy2(ROOT / f"Tools/{validator_name}.cs", validator)

    log = fixture / "de128-underworld-native.log"
    command = [str(args.unity_editor.resolve()), "-batchmode", "-projectPath", str(fixture),
               "-executeMethod", f"{validator_name}.RunEditor", "-logFile", str(log)]
    print(f"Underworld fixture: {fixture}; log: {log}", flush=True)
    environment = dict(os.environ, ECLIPSE_DE128_UNDERWORLD_PROFILE_TAG=args.profile_tag)
    if args.all_fights:
        environment["ECLIPSE_DE128_UNDERWORLD_MATRIX"] = "all"
    if args.fight:
        environment["ECLIPSE_DE128_UNDERWORLD_MATRIX"] = "single"
        environment["ECLIPSE_DE128_UNDERWORLD_TARGETS"] = args.fight
    if args.fights:
        environment["ECLIPSE_DE128_UNDERWORLD_MATRIX"] = "subset"
        environment["ECLIPSE_DE128_UNDERWORLD_TARGETS"] = args.fights
    if args.require_titan_equipment:
        environment["ECLIPSE_DE128_TITAN_EQUIPMENT"] = "1"
    if args.wasp_wave:
        environment["ECLIPSE_DE128_WASP_WAVE"] = "1"
    if args.butcher_wave:
        environment["ECLIPSE_DE128_BUTCHER_WAVE"] = "1"
    if args.hermit_wave:
        environment["ECLIPSE_DE128_HERMIT_WAVE"] = "1"
    if args.hermit_victory:
        environment["ECLIPSE_DE128_HERMIT_WAVE"] = "1"
        environment["ECLIPSE_DE128_HERMIT_VICTORY"] = "1"
    if args.mercenary_wave:
        environment["ECLIPSE_DE128_MERCENARY_WAVE"] = "1"
    if args.story_bosses:
        environment["ECLIPSE_DE128_UNDERWORLD_MATRIX"] = "story"
        source = (ROOT / "Mods/de128/scripts/content/underworld_story_data.lua").read_text(encoding="utf-8")
        names = re.findall(r"^        (BOSS_[A-Z0-9_]+) = \{", source, re.MULTILINE)
        if len(names) != 32 or len(set(names)) != 32:
            raise RuntimeError("The authored Underworld story boss list changed; review the native matrix.")
        environment["ECLIPSE_DE128_UNDERWORLD_TARGETS"] = ",".join(
            "de128:fights/uw_" + name.lower() + "_1" for name in names)
    result = subprocess.run(command, cwd=fixture, env=environment, timeout=args.timeout)
    lines = log.read_text(encoding="utf-8", errors="replace").splitlines() if log.exists() else []
    evidence = [line.strip() for line in lines if prefix in line or "error CS" in line]
    for line in evidence[-45:]:
        print(line, flush=True)
    if result.returncode != 0 or not any(prefix + " PASS" in line for line in evidence):
        raise RuntimeError(f"Native Underworld acceptance failed (exit {result.returncode}); inspect {log}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
