"""Copy DE128's Underworld presentation art from the owner's asset drop.

Source: ResearchSources/de128_assets/assets/Atlases (owner-supplied DE art; these
eleven event-raid button atlases exist nowhere in core). Each 300x300 PNG is copied
byte-for-byte and gets a line-based mod sprite descriptor. Lock states are not in
the drop; the game falls back to its native lock art. Two story portraits (character_may_1,
character_may_4) are native Unity resources missing from the packaged-art catalog
that resolves public core sprite IDs, so they are copied from Assets/Resources.
Four Underworld music ids resolve to no packaged track; they ship as PCM WAV under
assets/audio/underworld/<id>.wav: three from the drop's DE-named Music folder (newer) and
fight_halloween2019 from the DE 1.0.6 reference, the only copy.
Four Underworld opponent and hidden ability models from the drop ship as
reproducible gzip-compressed geometry under assets/models/underworld/*.modelz.
Two archived boss caster clips are named by the reviewed owner moves XML;
the matching packaged core binaries are copied byte-for-byte under
assets/animations/ so guarded move patches can select them.
DE128's Lua never opens XML.
The reviewed raid data selects twenty distinct upscaled hard-mode portraits from
the owner's Users directory. They retain the source 200 pixels-per-unit setting.
--check verifies the shipped copies against their sources.
"""

from __future__ import annotations

import argparse
import gzip
import hashlib
import shutil
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
DROP = ROOT / "ResearchSources" / "de128_assets" / "assets" / "Atlases"
ASSETS = ROOT / "Mods" / "de128" / "assets"
BUTTONS = {
    "BattleBtnArchitect": "architect_raid", "BattleBtnHalloween": "halloween_raid", "BattleBtnLamb": "lamb_raid",
    "BattleBtnNrityu": "nrityu_raid", "BattleBtnPuppeteer": "puppeteer_raid", "BattleBtnRakshasa": "rakshasa_raid",
    "BattleBtnRavana": "ravana_raid", "BattleBtnShurale": "shurale_raid", "BattleBtnSnowflake": "snowflake_raid",
    "BattleBtnWindWolf": "wind_wolf_raid", "BattleBtnPrince": "prince",
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
MODELS = ("mdl_body_berstuuk_early", "mdl_head_berstuuk",
          "mdl_vertical_trigger", "mdl_small_collision_box")
ANIMATION_SOURCE = ROOT / "Assets" / "Resources" / "gamedata" / "animations" / "binary"
ANIMATIONS = ("magic_water_wave_player", "chest_laser_ray_player")
RAID = ROOT / "ResearchSources" / "de128_assets" / "gamedata" / "raid_stages_default.xml"
RAID_SHA256 = "d012a1f47418def617d375743864e2256b00f4fd709f6785da45aa67a8c3fa7c"
USERS = ROOT / "ResearchSources" / "de128_assets" / "assets" / "Users"


def upscaled_avatars():
    if hashlib.sha256(RAID.read_bytes()).hexdigest() != RAID_SHA256:
        raise ValueError("Owner raid source changed; review its portrait references before extraction")
    raid = ET.parse(RAID).getroot()
    names = sorted({node.get("Avatar") for node in raid.iter("Warrior")
                    if (node.get("Avatar") or "").endswith("_new")})
    if len(names) != 20:
        raise ValueError(f"Reviewed raid source has {len(names)} upscaled avatars, expected 20")
    return names


def sources():
    for atlas, icon in BUTTONS.items():
        for state, prefix in (("Base", "base"), ("Active", "active")):
            folder = atlas + state
            yield DROP / folder / (folder + "." + prefix + "_" + icon + ".png"), atlas.lower() + "_" + prefix, 100
    for name in PORTRAITS:
        yield ROOT / "Assets" / "Resources" / "ui" / "users" / (name + ".png"), name, 100
    for name in upscaled_avatars():
        source = USERS / (name + ".png")
        meta = (USERS / (name + ".png.meta")).read_text(encoding="utf-8")
        if "spriteMode: 1" not in meta or "spritePixelsToUnits: 200" not in meta:
            raise ValueError(f"Reviewed raid portrait import settings changed: {name}")
        yield source, name, 200


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
    for source, name, pixels_per_unit in sources():
        target = textures / (name + ".png")
        descriptor = sprites / (name + ".asset")
        body = "type=sprite\ntexture=textures/underworld/" + name + f".png\npixels_per_unit={pixels_per_unit}\n"
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
    animations = ASSETS / "animations"
    if not args.check:
        animations.mkdir(parents=True, exist_ok=True)
    for name in ANIMATIONS:
        source = ANIMATION_SOURCE / (name + ".bytes")
        target = animations / source.name
        if not args.check:
            shutil.copyfile(source, target)
        if not target.is_file() or sha256(target) != sha256(source):
            failures.append("animation " + name)
        rows.append((source.name, sha256(source)))
    for failure in failures:
        print("FAIL", failure)
    for name, digest in rows:
        print(digest, name)
    print(f"{len(rows)} Underworld art and music files {'verified' if not failures else 'FAILED'}")
    return 1 if failures else 0


if __name__ == "__main__":
    sys.exit(main())
