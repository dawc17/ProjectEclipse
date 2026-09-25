#!/usr/bin/env python3
"""Install or verify the owner-supplied dojo chooser previews in DE128.

The India 2024 location has no matching Users/dojo medallion in the reviewed
owner drop, so its own raid preview is used until that asset is available.
"""

from __future__ import annotations

import argparse
import shutil
import struct
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "ResearchSources/de128_assets/assets"
MOD_ASSETS = ROOT / "Mods/de128/assets"
PREVIEWS = {
    "dojo": "Users/dojo_new.png",
    "new_year_24_china_dojo": "Users/new_year_china_dojo_new.png",
    "dojo_indian_event": "Users/dojo_indian_event_new.png",
    "dojo_indian_event_22": "Users/dojo_indian_event_22_new.png",
    "dojo_india24": "Battles/preview_raid_events_india_24.png",
    "haloween_dojo": "Users/haloween_dojo_new.png",
    "haloween_dojo_2019": "Users/haloween_dojo_2019_new.png",
    "dojo_hw21": "Users/dojo_hw21_new.png",
    "dojo_american_event_22": "Users/dojo_american_event_22_new.png",
    "dojo_hw22": "Users/dojo_hw22_new.png",
}


def dimensions(data: bytes) -> tuple[int, int]:
    if data[:16] != b"\x89PNG\r\n\x1a\n\x00\x00\x00\rIHDR":
        raise ValueError("Dojo preview is not a PNG")
    return struct.unpack(">II", data[16:24])


def run(check: bool) -> None:
    for location, source_path in PREVIEWS.items():
        source = SOURCE / source_path
        raw = source.read_bytes()
        width, height = dimensions(raw)
        if width < 200 or height < 200:
            raise ValueError(f"Dojo preview is unexpectedly small: {source}")
        texture = MOD_ASSETS / "textures/dojo_changer" / f"{location}.png"
        sprite = MOD_ASSETS / "sprites/dojo_changer" / f"{location}.asset"
        descriptor = (
            "type=sprite\n"
            f"texture=textures/dojo_changer/{location}.png\n"
            "pixels_per_unit=100\n"
        )
        if check:
            if texture.read_bytes() != raw or sprite.read_text(encoding="utf-8") != descriptor:
                raise ValueError(f"Packaged dojo preview differs from owner art: {location}")
        else:
            texture.parent.mkdir(parents=True, exist_ok=True)
            sprite.parent.mkdir(parents=True, exist_ok=True)
            shutil.copyfile(source, texture)
            sprite.write_text(descriptor, encoding="utf-8", newline="\n")
        print(f"{location}: {width}x{height} ({source_path})")


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--check", action="store_true", help="Compare existing mod art with the owner drop")
    run(parser.parse_args().check)
