#!/usr/bin/env python3
"""Install archived DE location params for locations whose base params are missing or no
longer match the packaged artwork.

Evidence (2026-09-24 owner playtest and an atlas-aware resolution audit of CORE_LOCATIONS):

* MISSING: the Underworld uses these locations, the bundle has their art, but the base had no
  params, so the game fell back to the dojo. Assets/DExml/locations holds world-unit params
  identical to DE 1.0.6; they are installed unchanged.
* REPLACED: the vanilla params name atlases that are not in the bundle (the packaged art is
  the newer per-image set) and size the images for the old half-resolution atlas, so the arena
  drew at half size in the middle of the stage. The DE params were authored for exactly these
  images; an offscreen composition of both (2026-09-24) confirmed the DE files fill the stage
  like dojo_india25, so they are installed unchanged.
* STRIPPED: the vanilla params are right but name missing atlases whose members exist as
  per-image sprites; only those Atlas attributes are removed.

Writes both Assets/vanillaXml/locations/<n>/<n>_params.xml and
Assets/Resources/gamedata/locations/<n>/params.txt. ``--check`` verifies the installed files.
"""

from __future__ import annotations

import argparse
import sys
import xml.etree.ElementTree as ET  # repository files only
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
DEXML = ROOT / "Assets" / "DExml" / "locations"
VANILLA = ROOT / "Assets" / "vanillaXml" / "locations"
RESOURCES = ROOT / "Assets" / "Resources" / "gamedata" / "locations"
MISSING = ["american_event_24", "dojo_american_event_22", "dojo_india25", "fall_event_24",
           "hw24_ancient_temple", "mountains_ny_25"]
REPLACED = ["dojo_hw21", "dojo_indian_event", "dojo_indian_event_22", "haloween_dojo", "ritual_battle_raid"]
STRIPPED = {"dojo_india24": ["dojo_india24_main_layers", "dojo_india24_decorations"]}


def stripped(name: str, atlases: list[str]) -> str:
    text = (VANILLA / name / f"{name}_params.xml").read_text(encoding="utf-8-sig")
    # Idempotent: re-running (or --check) sees the already stripped file.
    for atlas in atlases:
        text = text.replace(f' Atlas="{atlas}"', "")
    return text


def plan(report: list[str]) -> dict[str, str]:
    result = {name: (DEXML / name / f"{name}_params.xml").read_text(encoding="utf-8-sig") for name in MISSING + REPLACED}
    for name, atlases in STRIPPED.items():
        result[name] = stripped(name, atlases)
    return result


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("command", choices=("plan", "apply", "check"))
    args = parser.parse_args()
    report: list[str] = []
    files = plan(report)
    if args.command == "plan":
        print("\n".join(report))
        return 0
    failures = []
    for name, text in files.items():
        for target in (VANILLA / name / f"{name}_params.xml", RESOURCES / name / "params.txt"):
            if args.command == "apply":
                target.parent.mkdir(parents=True, exist_ok=True)
                target.write_text(text, encoding="utf-8", newline="\n")
            elif not target.is_file() or target.read_text(encoding="utf-8-sig") != text:
                failures.append(str(target.relative_to(ROOT)))
    for failure in failures:
        print("FAIL params differ:", failure)
    print(f"{len(files)} DE location params {'installed' if args.command == 'apply' else 'verified' if not failures else 'FAILED'}")
    return 1 if failures else 0


if __name__ == "__main__":
    sys.exit(main())
