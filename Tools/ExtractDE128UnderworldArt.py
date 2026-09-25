"""Copy DE128's Underworld presentation art from the owner's asset drop.

Source: ResearchSources/de128_assets/assets/Atlases (owner-supplied DE art; these
eleven event-raid button atlases exist nowhere in core). Each 300x300 PNG is copied
byte-for-byte and gets a line-based mod sprite descriptor. Lock states are not in
the drop; the game falls back to its native lock art. Two story portraits (character_may_1,
character_may_4) are native Unity resources missing from the packaged-art catalog
that resolves public core sprite IDs, so they are copied from Assets/Resources.
Every music id in the reviewed raid stages ships as PCM16 WAV under
assets/audio/underworld/<id>.wav. Sources are the owner's DE Music drop and the
DE 1.0.6 reference. Reference OGGs are decoded with ffmpeg; no substitute track
is used when a battle requests its archived id.
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
import subprocess
import sys
import tempfile
import wave
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
MUSIC_REFERENCE = ROOT / "ResearchSources" / "ReferenceSF2DE106" / "ExportedProject" / "Assets" / "gamedata" / "music"
MUSIC = {
    "burning_town_old": MUSIC_DROP / "the_burning_town_old.wav",
    "crystal": MUSIC_REFERENCE / "raids_crystal.ogg",
    "dark_ritual": MUSIC_REFERENCE / "fight38_dark_ritual.ogg",
    "drakaina": MUSIC_REFERENCE / "raids_war.ogg",
    "fatum": MUSIC_REFERENCE / "raids_fatum.ogg",
    "fear": MUSIC_REFERENCE / "raids_fear.ogg",
    "fight_halloween2022": MUSIC_REFERENCE / "fight_halloween2022.wav",
    "fight_independence_day": MUSIC_REFERENCE / "fight_independence_day.ogg",
    "fight38_sakura_forest": MUSIC_REFERENCE / "fight38_sakura_forest.ogg",
    "fight43_bihu_india": MUSIC_REFERENCE / "fight43_bihu_india.ogg",
    "flying_rocks": MUSIC_DROP / "the_flying_rocks.wav",
    "fungus": MUSIC_REFERENCE / "raids_fungus.ogg",
    "halls_of_the_dead_heroes": MUSIC_DROP / "the_halls_of_the_dead_heroes.wav",
    "holyman7": MUSIC_REFERENCE / "raids_arkhos.ogg",
    "holyman8": MUSIC_REFERENCE / "raids_hoaxen.ogg",
    "hunger": MUSIC_REFERENCE / "raids_hunger.ogg",
    "ninja_in_the_night_old": MUSIC_DROP / "the_ninja_in_the_night_old.wav",
    "fight_halloween2019": MUSIC_REFERENCE / "fight_halloween2019.wav",
    "raid_hw24": MUSIC_REFERENCE / "raid_hw24.ogg",
    "raid_newyear18": MUSIC_REFERENCE / "raid_newyear18.wav",
    "raids_berstuuk": MUSIC_REFERENCE / "raids_berstuuk.ogg",
    "raids_blackness": MUSIC_REFERENCE / "raids_blackness.ogg",
    "raids_freeze": MUSIC_REFERENCE / "raids_freeze.ogg",
    "raids_gatekeeper": MUSIC_REFERENCE / "raids_gatekeeper.ogg",
    "raids_hunter": MUSIC_REFERENCE / "raids_hunter.ogg",
    "raids_saturn": MUSIC_REFERENCE / "raids_saturn.ogg",
    "raids_shurale": MUSIC_REFERENCE / "raids_shurale.ogg",
    "vortex": MUSIC_REFERENCE / "raids_vortex.ogg",
    "vulcan": MUSIC_REFERENCE / "raids_vulcan.ogg",
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


def render_audio(source: Path, target: Path) -> None:
    if source.suffix == ".wav":
        shutil.copyfile(source, target)
        return
    subprocess.run(["ffmpeg", "-hide_banner", "-loglevel", "error", "-nostdin", "-y",
                    "-i", str(source), "-map_metadata", "-1", "-bitexact",
                    "-c:a", "pcm_s16le", str(target)], check=True)


def valid_audio(path: Path) -> bool:
    try:
        with wave.open(str(path), "rb") as clip:
            return (clip.getcomptype() == "NONE" and clip.getsampwidth() == 2 and
                    1 <= clip.getnchannels() <= 2 and clip.getframerate() > 0 and
                    clip.getnframes() > 0)
    except (OSError, EOFError, wave.Error):
        return False


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--check", action="store_true")
    args = parser.parse_args()
    failures = []
    reviewed_music = {battle.get("Music") for battle in ET.parse(RAID).iter("Battle") if battle.get("Music")}
    if reviewed_music != set(MUSIC):
        raise ValueError("Music source map differs from reviewed raid stages: " +
                         str(sorted(reviewed_music ^ set(MUSIC))))
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
        if source.suffix == ".wav":
            if not args.check:
                render_audio(source, target)
            matches = target.is_file() and sha256(target) == sha256(source)
        elif args.check:
            with tempfile.TemporaryDirectory(prefix="de128-audio-check-") as temporary:
                rendered = Path(temporary) / target.name
                render_audio(source, rendered)
                matches = target.is_file() and sha256(target) == sha256(rendered)
        else:
            render_audio(source, target)
            matches = target.is_file()
        if not matches or not valid_audio(target):
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
