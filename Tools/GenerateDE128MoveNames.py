#!/usr/bin/env python3
"""Copy the archived names of DE128's restored unarmed moves into its localizations.

The profile move list shows these titles for the moves registered in
Mods/de128/scripts/content/unarmed_moves.lua. Values are copied byte for byte
from Assets/DExml/localizations/<language>.xml; languages DE128 does not ship
are ignored. Run with --check to verify the committed files.
"""

from __future__ import annotations

import argparse
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from DE128Localization import DIRECTORY, ROOT, section_outputs, write_outputs  # noqa: E402

SECTION = "GENERATED Unarmed move names (Tools/GenerateDE128MoveNames.py)"
TITLES = ["FrontJumpScissorsKick", "AxeKickOld", "WallRunUp", "AirPunch", "ThrowLegPushProfile"]
ARCHIVE = ROOT / "Assets/DExml/localizations"


def values() -> dict[str, dict[str, str]]:
    result: dict[str, dict[str, str]] = {}
    for toml in sorted(DIRECTORY.glob("*.toml")):
        source = ARCHIVE / (toml.stem + ".xml")
        if not source.exists():
            continue
        words = {word.get("Title"): (word.text or "") for word in ET.parse(source).getroot().iter("Word")}
        entries = {title: words[title] for title in TITLES if words.get(title)}
        missing = [title for title in TITLES if title not in entries]
        if missing:
            raise SystemExit(f"{source}: missing archived move names {missing}")
        result[toml.stem] = entries
    return result


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--check", action="store_true", help="fail if the committed files differ")
    args = parser.parse_args()
    outputs = section_outputs(SECTION, values(), "moves.")
    if args.check:
        stale = [path.name for path, text in outputs.items() if path.read_text(encoding="utf-8") != text]
        if stale:
            print("Out of date: " + ", ".join(stale))
            return 1
        print(f"OK: {len(outputs)} localization files carry {len(TITLES)} archived move names.")
        return 0
    write_outputs(outputs)
    print(f"Wrote {len(TITLES)} move names to {len(outputs)} localization files.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
