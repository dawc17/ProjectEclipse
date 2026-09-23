"""Rebuild DE128's owned Sensei portraits and battle previews from ResearchSources.

Source precedence follows the owner's 2026-09-23 instruction: the owner-supplied
``ResearchSources/de128_assets`` drop and the DE 1.0.6 reference export win over
the vanilla Nekki ``ResearchSources/bundles``. Bundles are used only for images
with no newer alternative (the five young-boss portraits).

The tool writes PNG textures plus line-based mod sprite descriptors; it never
writes Unity YAML, ``.meta`` files or sprite vertex data. ``--check`` rebuilds
into a temporary directory and compares hashes with the shipped files.
"""

from __future__ import annotations

import argparse
import hashlib
import shutil
import sys
import tempfile
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
RESEARCH = ROOT / "ResearchSources"
sys.path.insert(0, str(RESEARCH / "tools" / "unitypy"))
sys.path.insert(1, str(Path(__file__).with_name("python-packages")))

BUNDLE = RESEARCH / "bundles" / "USERS"
BUNDLE_SHA256 = "772ff7060df53749fb11e84ef0cc8619551b1ecc40c5c3c4ff0f95878ca14eb8"
BUNDLE_PORTRAITS = (
    "boss_hermit_young",
    "boss_butcher_young",
    "boss_wasp_young",
    "boss_widow_young",
    "boss_shogun_young",
)
COPIES = {
    # Present as a native Unity resource but absent from the packaged-art catalog
    # that resolves public core sprite IDs, so the mod API cannot reference it.
    "character_sensei": ROOT / "Assets/Resources/ui/users/character_sensei.png",
    "character_pirate": RESEARCH / "ReferenceSF2DE106/ExportedProject/Assets/UI/Users/character_pirate.png",
    **{
        f"preview_pvp_{name}": RESEARCH / f"de128_assets/assets/Battles/preview_pvp_{name}.png"
        for name in ("stone_dragon", "village", "ships", "flooded_village", "magic_rocks")
    },
}
EXPECTED = {
    "boss_butcher_young": "3181c7891f5de2f7a4a2410a04b318c1e22afa3d29abf820fc00bbb3991164e7",
    "boss_hermit_young": "1e45d4bf5a2ab1ef90fd41a3041702e9ffb80c7c81e4818d8c3791cc2bc95b6e",
    "boss_shogun_young": "6439adabe6d1e547b3d137b7476f847477a5cde71c0298f9d5639b34815a31a3",
    "boss_wasp_young": "c7a9d150491045bd3a04b5aa8b028a24aacdc45f800564e85b2fe9200e5148b0",
    "boss_widow_young": "a67aafb5d4c471a665e553cc91cb5b9b3f9953f3a46e2c82a0fa0dab488c3f13",
    "character_sensei": "d046d8f299768fc694adb5a86cb1e74b52050d2b44bdc549c6eaee3ea30e3e51",
    "character_pirate": "2ca6a6eea6ee7ab88145e659948e75cbc9e6b99c5d3ee06450d76b30072515dd",
    "preview_pvp_flooded_village": "803d405bdb37cb2d75f49fe252d7969b160ccb2fe6b23edb071a5d7b4032c941",
    "preview_pvp_magic_rocks": "cd1e1181dda7f42e1331c0c080299e43f1f4d81e1bdad70435ff538948064877",
    "preview_pvp_ships": "736249249eef3b4bc4f487583be3ae224eee08ae21bf8755839dd119e609eb14",
    "preview_pvp_stone_dragon": "27e4dba9ec984a84b5b859eccba2d70e8328b6d71cd986994f36cc150b38bbde",
    "preview_pvp_village": "861538024cf31c1a9a60595367cdd122adc691abe161f1e5936c43fef60b3a01",
}


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def extract_bundle(textures: Path) -> None:
    import UnityPy  # noqa: PLC0415

    if sha256(BUNDLE) != BUNDLE_SHA256:
        raise SystemExit(f"{BUNDLE} changed; re-audit the vanilla source before rebuilding.")
    sprites, images = {}, {}
    for obj in UnityPy.load(str(BUNDLE)).objects:
        if obj.type.name not in ("Sprite", "Texture2D"):
            continue
        data = obj.read()
        if data.m_Name in BUNDLE_PORTRAITS:
            (sprites if obj.type.name == "Sprite" else images)[data.m_Name] = data
    for name in BUNDLE_PORTRAITS:
        sprite, texture = sprites[name], images[name]
        rect = sprite.m_Rect
        # The sprite covers the whole texture. Exporting the texture keeps the
        # native 512x512 portrait canvas; UnityPy's sprite export crops tight meshes.
        if (rect.x, rect.y, rect.width, rect.height) != (0, 0, texture.m_Width, texture.m_Height):
            raise SystemExit(f"{name}: sprite no longer covers its texture; review the source.")
        if sprite.m_PixelsToUnits != 100:
            raise SystemExit(f"{name}: unexpected pixels-per-unit {sprite.m_PixelsToUnits}.")
        texture.image.save(textures / f"{name}.png")


def build(assets: Path) -> None:
    textures = assets / "textures" / "sensei"
    sprites = assets / "sprites" / "sensei"
    textures.mkdir(parents=True, exist_ok=True)
    sprites.mkdir(parents=True, exist_ok=True)
    extract_bundle(textures)
    for name, source in COPIES.items():
        shutil.copyfile(source, textures / f"{name}.png")
    for name in EXPECTED:
        (sprites / f"{name}.asset").write_text(
            f"type=sprite\ntexture=textures/sensei/{name}.png\npixels_per_unit=100\n",
            encoding="utf-8", newline="\n")


def verify(assets: Path) -> list[str]:
    failures = []
    for name, digest in EXPECTED.items():
        texture = assets / "textures" / "sensei" / f"{name}.png"
        if not texture.is_file() or sha256(texture) != digest:
            failures.append(f"texture {name}")
        if not (assets / "sprites" / "sensei" / f"{name}.asset").is_file():
            failures.append(f"descriptor {name}")
    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--check", action="store_true", help="rebuild in a temporary folder and compare")
    args = parser.parse_args()
    shipped = ROOT / "Mods" / "de128" / "assets"
    if args.check:
        with tempfile.TemporaryDirectory() as temp:
            build(Path(temp))
            failures = verify(Path(temp)) + verify(shipped)
            for name in EXPECTED:
                for kind in ("sprites", "textures"):
                    suffix = ".asset" if kind == "sprites" else ".png"
                    a = Path(temp) / kind / "sensei" / f"{name}{suffix}"
                    b = shipped / kind / "sensei" / f"{name}{suffix}"
                    if not b.is_file() or a.read_bytes() != b.read_bytes():
                        failures.append(f"rebuilt {kind} {name} differs")
    else:
        build(shipped)
        failures = verify(shipped)
    for failure in failures:
        print("FAIL", failure)
    print(f"{len(EXPECTED)} Sensei art entries {'verified' if not failures else 'FAILED'}")
    return 1 if failures else 0


if __name__ == "__main__":
    sys.exit(main())
