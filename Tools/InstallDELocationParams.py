#!/usr/bin/env python3
"""Install archived DE location params for locations whose base params are missing or no
longer match the packaged artwork.

Evidence (2026-09-24 owner playtest and an atlas-aware resolution audit of CORE_LOCATIONS):

* MISSING: the Underworld uses these locations, the bundle has their art, but the base had no
  params, so the game fell back to the dojo. Assets/DExml/locations holds world-unit params
  identical to DE 1.0.6; they are installed unchanged.
* REBUILT: the vanilla params name atlases that are not in the bundle (the packaged art is the
  newer per-image set), which rendered the arena malformed. The DE params match the new image
  names but give sizes in texture pixels for upscaled images (double the world size used by
  Location.cs, which draws an Image at exactly Width x Height world units). Each element's
  world size is taken from, in order: the vanilla element at the same ClassName/X/Y with the
  same aspect; the spacing of a row of equal tiles in one layer; the vanilla width (keeping
  the new aspect) for a re-drawn element; the file's measured doubling factor. pixel_1 filler
  quads are world-sized already.
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
REBUILT = ["dojo_hw21", "dojo_indian_event", "dojo_indian_event_22", "haloween_dojo", "ritual_battle_raid"]
STRIPPED = {"dojo_india24": ["dojo_india24_main_layers", "dojo_india24_decorations"]}


def fnum(value: float) -> str:
    return str(int(round(value))) if abs(value - round(value)) < 1e-6 else f"{value:.3f}".rstrip("0").rstrip(".")


def sized(element: ET.Element) -> bool:
    return element.get("ClassName") is not None and element.get("Width") is not None and element.get("Height") is not None


def rebuild(name: str, report: list[str]) -> str:
    de_tree = ET.parse(DEXML / name / f"{name}_params.xml")
    vanilla = ET.parse(VANILLA / name / f"{name}_params.xml").getroot()
    peers = {(e.get("ClassName"), e.get("X"), e.get("Y")): e for e in vanilla.iter() if sized(e)}

    def aspect(e):
        return float(e.get("Width")) / float(e.get("Height")) if float(e.get("Height")) else 0

    ratios = []
    for e in de_tree.getroot().iter():
        if not sized(e) or e.get("ClassName") == "pixel_1":
            continue
        peer = peers.get((e.get("ClassName"), e.get("X"), e.get("Y")))
        if peer is not None and abs(aspect(e) - aspect(peer)) <= 0.02 * max(aspect(e), 1e-6) and float(e.get("Width")):
            ratios.append(float(peer.get("Width")) / float(e.get("Width")))
    factor = sorted(ratios)[len(ratios) // 2] if ratios else 1.0
    factor = 0.5 if abs(factor - 0.5) < 0.05 else 1.0 if abs(factor - 1) < 0.05 else factor

    for layer in de_tree.getroot().iter("Layer"):
        elements = [e for e in layer if sized(e)]
        # Rows of equal tiles: same ClassName pattern not required, same DE size and Y, uniform spacing.
        rows: dict[tuple[str, str, str], list[ET.Element]] = {}
        for e in elements:
            if e.get("ClassName") != "pixel_1":
                rows.setdefault((e.get("Width"), e.get("Height"), e.get("Y")), []).append(e)
        tile_scale: dict[int, float] = {}
        for (width, _, _), row in rows.items():
            if len(row) < 2:
                continue
            xs = sorted(float(e.get("X")) for e in row)
            gaps = {round(b - a, 3) for a, b in zip(xs, xs[1:])}
            if len(gaps) == 1 and float(width):
                for e in row:
                    tile_scale[id(e)] = gaps.pop() / float(width) if gaps else 1.0
                    gaps = {round(xs[1] - xs[0], 3)}
        for e in elements:
            cn = e.get("ClassName")
            w, h = float(e.get("Width")), float(e.get("Height"))
            peer = peers.get((cn, e.get("X"), e.get("Y")))
            if cn == "pixel_1":
                how, nw, nh = "filler", w, h
            elif peer is not None and abs(aspect(e) - aspect(peer)) <= 0.02 * max(aspect(e), 1e-6):
                how, nw, nh = "vanilla", float(peer.get("Width")), float(peer.get("Height"))
            elif id(e) in tile_scale:
                s = tile_scale[id(e)]
                how, nw, nh = f"tile x{s:g}", w * s, h * s
            elif peer is not None:
                s = float(peer.get("Width")) / w
                how, nw, nh = "vanilla width", w * s, h * s
            else:
                how, nw, nh = f"factor x{factor:g}", w * factor, h * factor
            e.set("Width", fnum(nw))
            e.set("Height", fnum(nh))
            if (nw, nh) != (w, h) or how != "vanilla":
                report.append(f"  {name}/{cn} ({e.get('X')},{e.get('Y')}): {fnum(w)}x{fnum(h)} -> {fnum(nw)}x{fnum(nh)} [{how}]")
    return "<?xml version='1.0' encoding='utf-8'?>\n" + ET.tostring(de_tree.getroot(), encoding="unicode") + "\n"


def stripped(name: str, atlases: list[str]) -> str:
    text = (VANILLA / name / f"{name}_params.xml").read_text(encoding="utf-8-sig")
    # Idempotent: re-running (or --check) sees the already stripped file.
    for atlas in atlases:
        text = text.replace(f' Atlas="{atlas}"', "")
    return text


def plan(report: list[str]) -> dict[str, str]:
    result = {name: (DEXML / name / f"{name}_params.xml").read_text(encoding="utf-8-sig") for name in MISSING}
    for name in REBUILT:
        result[name] = rebuild(name, report)
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
