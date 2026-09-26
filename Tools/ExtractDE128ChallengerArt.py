"""Copy DE128's Challenger presentation art and music from the owner's asset drop.

Source: ResearchSources/de128_assets (the owner's 2026-09-23 DE drop). Core has
none of these images or tracks: the seven opponent portraits and weapon-drop
images (Users/, 200 pixels per unit), the seven battle previews (Battles/) and
map-button base/active states (Atlases/, 300x300). Lock and pressed states are
not used; the game falls back to its native lock art. The archived music ids
(samurai_spirit, blade_dance, ...) are not native tracks, so the matching owner
PCM16 WAVs ship under assets/audio/challenger/<id>.wav.

Every file is copied byte-for-byte and gets a line-based mod sprite descriptor.
The tool never writes Unity YAML, .meta files or sprite vertex data.
--check verifies the shipped copies and descriptors against their sources.
"""

from __future__ import annotations

import argparse
import hashlib
import re
import re
import shutil
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
DROP = ROOT / "ResearchSources" / "de128_assets" / "assets"
ASSETS = ROOT / "Mods" / "de128" / "assets"
# Archive icon, opponent avatar, drop image, music id and owner WAV per Challenger.
CHALLENGERS = {
    "trickster": ("man_nunchaku_new", "drop_nunchaku_new", "samurai_spirit", "the_samurai_spirit"),
    "hawk": ("man_ninja_naginata_new", "drop_naginata_new", "blade_dance", "the_blade_dance"),
    "rose": ("girl_katana_new", "drop_katana_new", "ronin", "the_ronin"),
    "fisher": ("man_cool_staff_new", "drop_wanderer_staff_new", "heavenly_clouds", "the_heavenly_clouds"),
    "outcast": ("man_heavy_kusarigama_new", "drop_kusarigama_new", "fuji", "fuji"),
    "ronin": ("man_dadao_janissary_new", "drop_ring_sword_new", "sky_isles", "the_sky_isles"),
    "nova": ("girl_im_knuckles_new", "img_drop_im_knuckles_new", "the_monastery", "the_monastery"),
}


def sprites():
    for icon, (avatar, drop, _, _) in CHALLENGERS.items():
        atlas = "BattleBtn" + icon.capitalize()
        for state, prefix in (("Base", "base"), ("Active", "active")):
            folder = atlas + state
            yield DROP / "Atlases" / folder / (folder + "." + prefix + "_" + icon + ".png"), atlas.lower() + "_" + prefix, 100
        yield DROP / "Battles" / ("preview_" + icon + ".png"), "preview_" + icon, 100
        for name in (avatar, drop):
            yield DROP / "Users" / (name + ".png"), name, 200


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def pixels_per_unit(source: Path) -> int:
    meta = source.with_name(source.name + ".meta").read_text(encoding="utf-8")
    found = re.search(r"^\s*spritePixelsToUnits: (\d+)\s*$", meta, re.MULTILINE)
    if found and int(found.group(1)) in (100, 200):
        return int(found.group(1))
    raise ValueError(f"Unreviewed import settings: {source}")


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--check", action="store_true")
    args = parser.parse_args()
    textures = ASSETS / "textures" / "challenger"
    descriptors = ASSETS / "sprites" / "challenger"
    audio = ASSETS / "audio" / "challenger"
    if not args.check:
        for folder in (textures, descriptors, audio):
            folder.mkdir(parents=True, exist_ok=True)
    failures = []
    count = 0
    for source, name, expected_ppu in sprites():
        if pixels_per_unit(source) != expected_ppu:
            failures.append(f"source pixels per unit {name}")
        target = textures / (name + ".png")
        descriptor = descriptors / (name + ".asset")
        body = f"type=sprite\ntexture=textures/challenger/{name}.png\npixels_per_unit={expected_ppu}\n"
        if not args.check:
            shutil.copyfile(source, target)
            descriptor.write_text(body, encoding="utf-8", newline="\n")
        if not target.is_file() or sha256(target) != sha256(source):
            failures.append("texture " + name)
        if not descriptor.is_file() or descriptor.read_text(encoding="utf-8") != body:
            failures.append("descriptor " + name)
        count += 1
    for _, _, music, wav in CHALLENGERS.values():
        source = DROP / "Music" / (wav + ".wav")
        target = audio / (music + ".wav")
        if not args.check:
            shutil.copyfile(source, target)
        if not target.is_file() or sha256(target) != sha256(source):
            failures.append("audio " + music)
        count += 1
    for failure in failures:
        print("FAIL", failure)
    print(f"{count} Challenger art/audio entries {'verified' if not failures else 'FAILED'}")
    return 1 if failures else 0


if __name__ == "__main__":
    sys.exit(main())
