"""Replace game and DE128 music with the owner's lossless soundtrack FLACs.

Source: ResearchSources/sf2flacs (the owner's 2026-09-29 drop: the two Shadow Fight 2
soundtrack albums as FLAC, "NN - Title" and "NN. Title"). Every pairing below was chosen
by audio fingerprint (log band-energy correlation of the first 40 s, best of 56 FLACs;
score in the comment) and agrees with the track name and length. Tracks whose best
match was weak were left alone: fight32_starship, fight33_stone_forest, fight36_stardocks
and the DE128 raid themes with no album counterpart. DE128's deliberate *_old campaign
versions are also kept.

Core music keeps its file name, extension and .meta (GUID and import settings), so every
Resources lookup and serialized reference is unchanged: .ogg targets are encoded as
Vorbis q10 and .mp3 targets as 320 kbps MP3; Unity then imports them as before. DE128
targets are PCM16 WAV, as the mod loader requires, decoded losslessly from the FLAC.

Usage:
    python Tools/ImportSoundtrackFlacs.py          # render every mapped target
    python Tools/ImportSoundtrackFlacs.py --check  # verify the shipped files match a fresh render
"""

from __future__ import annotations

import argparse
import hashlib
import subprocess
import sys
import tempfile
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
DROP = ROOT / "ResearchSources" / "sf2flacs"
MUSIC = "Assets/Resources/gamedata/music/"
DE128 = "Mods/de128/assets/audio/"

TARGETS = {
    # Core fight, menu and screen music.
    MUSIC + "act.ogg": "01. A New Act.flac",                       # 0.55, the 8 s act sting
    MUSIC + "credits.ogg": "01 - The Ambience.flac",               # 0.92
    MUSIC + "menu.ogg": "02 - The Training Room.flac",             # 0.95
    MUSIC + "fight1_samurai_spirit.ogg": "03 - The Samurai Spirit.flac",   # 0.98
    MUSIC + "fight2_blade_dance.mp3": "04 - The Blade Dance.flac",         # 0.84
    MUSIC + "fight3_vengeance.mp3": "05 - The Vengeance.flac",             # 0.70
    MUSIC + "fight4_forest_of_death.mp3": "06 - The Forest of Death.flac", # 0.98
    MUSIC + "fight5_ninja_in_the_night.ogg": "07 - The Ninja in the Night.flac",  # 0.89
    MUSIC + "fight6_sparring.mp3": "08 - The Sparring.flac",               # 1.00
    MUSIC + "fight7_fat_boss.mp3": "09 - The Fat Boss.flac",               # 0.98
    MUSIC + "fight8_final_boss.mp3": "10 - The Final Boss.flac",           # 0.79
    MUSIC + "fight9_master_skills.mp3": "11 - The Master of the Skills.flac",  # 0.98
    MUSIC + "fight10_black_warrior.ogg": "12 - The Black Warrior.flac",    # 0.79
    MUSIC + "fight11_ronin.mp3": "13 - Ronin.flac",                        # 0.99
    MUSIC + "fight12_deadly_smoke.mp3": "14 - The Deadly Smoke.flac",      # 0.81
    MUSIC + "fight13_old_sensei.mp3": "15 - Old Sensei.flac",              # 0.99 (album cut is longer)
    MUSIC + "fight14_ship_battle.mp3": "16 - The Battle Ship.flac",        # 0.65
    MUSIC + "fight15_shadow_lady.mp3": "17 - The Shadow Lady.flac",        # 1.00
    MUSIC + "fight16_the_battlefield_flowers.mp3": "18 - The Flower Battlefield.flac",  # 0.87
    MUSIC + "fight17_cave.mp3": "19 - The Cave.flac",                      # 0.99
    MUSIC + "fight18_fuji.mp3": "20 - Fuji.flac",                          # 0.98
    MUSIC + "fight19_volcano.mp3": "21 - The Volcano.flac",                # 0.82
    MUSIC + "fight20_bridge_to_the_other_side.ogg": "22 - Bridge to the Other Side.flac",  # 0.97
    MUSIC + "fight21_lesson_in_the_dark_room.mp3": "23 - The Lesson in the Dark Room.flac",  # 0.93
    MUSIC + "fight22_heavenly_clouds.mp3": "24 - Clouds Heaven.flac",      # 0.88
    MUSIC + "fight23_burning_town.mp3": "25 - Burning Town.flac",          # 0.98
    MUSIC + "fight24_ruins_village.mp3": "26 - The Village Ruins.flac",    # 0.98
    MUSIC + "fight25_hive.mp3": "27 - The Hive.flac",                      # 0.72
    MUSIC + "fight27_factory.mp3": "29 - The Factory.flac",                # 0.95
    MUSIC + "fight28_flying_rocks.mp3": "30 - The Flying Rocks.flac",      # 0.94
    MUSIC + "fight30_gates_of_shadows.mp3": "02. The Gates of Shadows.flac",   # 0.91
    MUSIC + "fight31_graveyard_ships.mp3": "03. The Ships Graveyard.flac",     # 0.99
    MUSIC + "fight34_halls_of_the_dead_heroes.ogg": "04. The Halls of Dead Heroes.flac",  # 0.97
    MUSIC + "fight37_Titan_Epic_Fight.mp3": "13. The Epic Titan Fight.flac",   # 0.99
    # Eclipse title theme.
    "Assets/Resources/EclipseTitle/the_ambience.mp3": "01 - The Ambience.flac",  # 0.93
    # DE128 Challenger duels.
    DE128 + "challenger/samurai_spirit.wav": "03 - The Samurai Spirit.flac",   # 1.00
    DE128 + "challenger/blade_dance.wav": "04 - The Blade Dance.flac",         # 1.00
    DE128 + "challenger/ronin.wav": "13 - Ronin.flac",                         # 1.00
    DE128 + "challenger/fuji.wav": "20 - Fuji.flac",                           # 1.00
    DE128 + "challenger/heavenly_clouds.wav": "24 - Clouds Heaven.flac",       # 1.00
    DE128 + "challenger/the_monastery.wav": "05. The Monastery.flac",          # 1.00
    DE128 + "challenger/sky_isles.wav": "12. The Sky Isles.flac",              # 1.00
    # DE128 Underworld raids.
    DE128 + "underworld/halls_of_the_dead_heroes.wav": "04. The Halls of Dead Heroes.flac",  # 1.00
    DE128 + "underworld/fight38_sakura_forest.wav": "07. The Sakura Forest.flac",  # 0.99
    DE128 + "underworld/fungus.wav": "09. The Underworld Passage.flac",        # 0.97
    DE128 + "underworld/crystal.wav": "10. The Crystal Dungeon.flac",          # 0.92
    DE128 + "underworld/vortex.wav": "15. The Ancient Evil.flac",              # 0.87
    DE128 + "underworld/fatum.wav": "16. The Fatum.flac",                      # 0.99
    DE128 + "underworld/holyman7.wav": "17. The Rats Underworld.flac",         # 0.93
    DE128 + "underworld/holyman8.wav": "18. Lord of the Lies.flac",            # 0.85
    DE128 + "underworld/raids_freeze.wav": "20. The Christmas Ride.flac",      # 0.94
    DE128 + "underworld/dark_ritual.wav": "21. Halloween Rade.flac",           # 0.98
    DE128 + "underworld/fear.wav": "22. The Inevitability.flac",               # 0.96
    DE128 + "underworld/raids_hunter.wav": "23. Shadow Magic.flac",            # 0.86
    DE128 + "underworld/fight_independence_day.wav": "24. The Bearfoot Tribe.flac",  # 1.00
    DE128 + "underworld/drakaina.wav": "25. The Ethernal Battlefield.flac",    # 0.81
    DE128 + "underworld/vulcan.wav": "26. The Underworld Demon.flac",          # 0.97
    DE128 + "underworld/flying_rocks.wav": "30 - The Flying Rocks.flac",       # 1.00
}

# Mod audio file names this tool owns; the DE128 extractors skip them.
OWNED_MOD_AUDIO = {Path(target).name for target in TARGETS if target.startswith(DE128)}
OWNED_MOD_PATHS = {target[len("Mods/de128/assets/"):] for target in TARGETS if target.startswith(DE128)}

CODECS = {
    ".ogg": ["-c:a", "libvorbis", "-q:a", "10"],
    ".mp3": ["-c:a", "libmp3lame", "-b:a", "320k"],
    ".wav": ["-c:a", "pcm_s16le"],
}


def render(source: Path, target: Path) -> None:
    subprocess.run(["ffmpeg", "-hide_banner", "-loglevel", "error", "-nostdin", "-y", "-i", str(source),
                    "-map", "0:a:0", "-map_metadata", "-1", "-ar", "44100", "-ac", "2", "-bitexact",
                    *CODECS[target.suffix], str(target)], check=True)


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--check", action="store_true")
    args = parser.parse_args()
    failures = []
    for relative, flac in TARGETS.items():
        source, target = DROP / flac, ROOT / relative
        if not source.is_file():
            failures.append("missing source " + flac)
            continue
        if not target.is_file():
            failures.append("missing target " + relative)
            continue
        if not target.with_name(target.name + ".meta").is_file() and relative.startswith("Assets/"):
            failures.append("missing .meta " + relative)
        if args.check:
            with tempfile.TemporaryDirectory(prefix="soundtrack-check-") as temporary:
                fresh = Path(temporary) / target.name
                render(source, fresh)
                if sha256(fresh) != sha256(target):
                    failures.append("differs " + relative)
        else:
            render(source, target)
    for failure in failures:
        print("FAIL", failure)
    print(f"{len(TARGETS)} soundtrack targets {'verified' if args.check else 'rendered'}"
          f"{'' if not failures else ' with ' + str(len(failures)) + ' failures'}.")
    return 1 if failures else 0


if __name__ == "__main__":
    sys.exit(main())
