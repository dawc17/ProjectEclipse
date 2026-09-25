"""Copy DE128's Underworld map-button sprites from the owner's asset drop.

Source: ResearchSources/de128_assets/assets/Atlases (owner-supplied DE art; these
ten event-raid button atlases exist nowhere in core). Each 300x300 PNG is copied
byte-for-byte and gets a line-based mod sprite descriptor. Lock states are not in
the drop; the game falls back to its native lock art. Two story portraits (character_may_1,
character_may_4) are native Unity resources missing from the packaged-art catalog
that resolves public core sprite IDs, so they are copied from Assets/Resources.
Four Underworld music ids resolve to no packaged track; they ship as PCM WAV under
assets/audio/underworld/<id>.wav: three from the drop's DE-named Music folder (newer) and
fight_halloween2019 from the DE 1.0.6 reference, the only copy.
Two Berstuuk opponent models from the drop ship as reproducible gzip-compressed
geometry under assets/models/underworld/*.modelz. DE128's Lua never opens XML.
--check verifies the shipped copies against their sources.
"""

from __future__ import annotations

import argparse
import gzip
import hashlib
import shutil
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
DROP = ROOT / "ResearchSources" / "de128_assets" / "assets" / "Atlases"
ASSETS = ROOT / "Mods" / "de128" / "assets"
BUTTONS = {
    "BattleBtnArchitect": "architect_raid", "BattleBtnHalloween": "halloween_raid", "BattleBtnLamb": "lamb_raid",
    "BattleBtnNrityu": "nrityu_raid", "BattleBtnPuppeteer": "puppeteer_raid", "BattleBtnRakshasa": "rakshasa_raid",
    "BattleBtnRavana": "ravana_raid", "BattleBtnShurale": "shurale_raid", "BattleBtnSnowflake": "snowflake_raid",
    "BattleBtnWindWolf": "wind_wolf_raid",
}
PORTRAITS = ("character_may_1", "character_may_4")
MUSIC_DROP = ROOT / "ResearchSources" / "de128_assets" / "assets" / "Music"
MUSIC = {
    "flying_rocks": MUSIC_DROP / "the_flying_rocks.wav",
    "halls_of_the_dead_heroes": MUSIC_DROP / "the_halls_of_the_dead_heroes.wav",
    "ninja_in_the_night_old": MUSIC_DROP / "the_ninja_in_the_night_old.wav",
    "fight_halloween2019": ROOT / "ResearchSources" / "ReferenceSF2DE106" / "ExportedProject" / "Assets" / "gamedata" / "music" / "fight_halloween2019.wav",
}
MODELS_DROP = ROOT / "ResearchSources" / "de128_assets" / "gamedata" / "models"
MODELS = ("mdl_body_berstuuk_early", "mdl_head_berstuuk")


def sources():
    for atlas, icon in BUTTONS.items():
        for state, prefix in (("Base", "base"), ("Active", "active")):
            folder = atlas + state
            yield DROP / folder / (folder + "." + prefix + "_" + icon + ".png"), atlas.lower() + "_" + prefix
    for name in PORTRAITS:
        yield ROOT / "Assets" / "Resources" / "ui" / "users" / (name + ".png"), name


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--check", action="store_true")
    args = parser.parse_args()
    failures = []
    textures = ASSETS / "textures" / "underworld"
    sprites = ASSETS / "sprites" / "underworld"
    if not args.check:
        textures.mkdir(parents=True, exist_ok=True)
        sprites.mkdir(parents=True, exist_ok=True)
    rows = []
    for source, name in sources():
        target = textures / (name + ".png")
        descriptor = sprites / (name + ".asset")
        body = "type=sprite\ntexture=textures/underworld/" + name + ".png\npixels_per_unit=100\n"
        if not args.check:
            shutil.copyfile(source, target)
            descriptor.write_text(body, encoding="utf-8", newline="\n")
        if not target.is_file() or sha256(target) != sha256(source):
            failures.append("texture " + name)
        if not descriptor.is_file() or descriptor.read_text(encoding="utf-8") != body:
            failures.append("descriptor " + name)
        rows.append((name, sha256(source)))
    audio = ASSETS / "audio" / "underworld"
    if not args.check:
        audio.mkdir(parents=True, exist_ok=True)
    for name, source in MUSIC.items():
        target = audio / (name + ".wav")
        if not args.check:
            shutil.copyfile(source, target)
        if not target.is_file() or sha256(target) != sha256(source):
            failures.append("audio " + name)
        rows.append((name + ".wav", sha256(source)))
    models = ASSETS / "models" / "underworld"
    if not args.check:
        models.mkdir(parents=True, exist_ok=True)
    for name in MODELS:
        source = MODELS_DROP / (name + ".xml")
        target = models / (name + ".modelz")
        packed = gzip.compress(source.read_bytes(), mtime=0)
        if not args.check:
            target.write_bytes(packed)
        if not target.is_file() or target.read_bytes() != packed:
            failures.append("model geometry " + name)
        rows.append((name + ".modelz <- " + name + ".xml", sha256(source)))
    for failure in failures:
        print("FAIL", failure)
    for name, digest in rows:
        print(digest, name)
    print(f"{len(rows)} Underworld art and music files {'verified' if not failures else 'FAILED'}")
    return 1 if failures else 0


if __name__ == "__main__":
    sys.exit(main())
