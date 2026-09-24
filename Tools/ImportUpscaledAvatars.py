#!/usr/bin/env python3
"""Give loose-only avatars their upscaled versions from the owner's asset drop.

The zone bundles already carry upscaled ``UI/Users`` portraits for acts two onwards, and packaged
core art is loaded before ``Assets/Resources``. Act one's bundle has none, so Lynx, his
bodyguards, Shadow (``avatar_hero``) and a few story portraits still fell back to the 512 px
loose files. This adds every avatar that exists only as a loose ``Assets/Resources/ui/users``
PNG and has ``ResearchSources/de128_assets/assets/Users/<name>_new.png`` to the USERS bundle.

Sprites are full-rect quads with the drop's import settings (centre pivot, trilinear filter).
Pixels per unit keep each loose portrait's native size (512 px at 100 -> 1024 px at 200), so
layouts that call SetNativeSize do not change. No Unity mesh data is copied or edited.

Usage:
    python Tools/ImportUpscaledAvatars.py plan | apply | check
"""

from __future__ import annotations

import argparse
import json
import re
import shutil
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from ImportUpscaledLocations import CATALOG, PACKER, ROOT, ImportError_, packer, png_size, read_descriptors, sha256  # noqa: E402

DROP = ROOT / "ResearchSources" / "de128_assets" / "assets" / "Users"
LOOSE = ROOT / "Assets" / "Resources" / "ui" / "users"
BUNDLE = ROOT / "Assets" / "StreamingAssets" / "SF2Content" / "ArtBundles" / "USERS.tar.lz4"
WORK = ROOT / "Library" / "UpscaledAvatars"
GROUP = "USERS"
FILTER = {"0": "0", "1": "1", "2": "2"}


def meta_value(path: Path, key: str) -> str:
    match = re.search(r"^\s*" + key + r":\s*(\S+)", path.read_text(encoding="utf-8"), re.M)
    if not match:
        raise ImportError_(f"{path.name} has no {key}")
    return match.group(1)


def catalog() -> tuple[dict, dict]:
    data = json.loads(CATALOG.read_text(encoding="utf-8-sig"))
    return data, next(g for g in data["bundles"] if g["name"] == GROUP)


def candidates(data: dict) -> list[str]:
    packaged = {a["address"].lower() for g in data["bundles"] for a in g["assets"]
                if a["address"].lower().startswith("ui/users/")}
    return [png.stem for png in sorted(LOOSE.glob("*.png"))
            if f"ui/users/{png.stem.lower()}" not in packaged and (DROP / (png.stem + "_new.png")).is_file()]


def sprite(name: str) -> tuple[str, bytes]:
    source = DROP / (name + "_new.png")
    width, height = png_size(source)
    loose = LOOSE / (name + ".png")
    loose_width, _ = png_size(loose)
    loose_ppu = float(meta_value(Path(str(loose) + ".meta"), "spritePixelsToUnits"))
    ppu = loose_ppu * width / loose_width
    meta = Path(str(source) + ".meta")
    if meta_value(meta, "spriteMode") != "1" or "x: 0.5, y: 0.5" not in meta.read_text(encoding="utf-8"):
        raise ImportError_(f"{source.name}: expected a single centred sprite")
    filt = FILTER[meta_value(meta, "filterMode")]
    x, y = width / 2 / ppu, height / 2 / ppu
    texture = f"textures/upscaled/{name}.png"
    text = (
        "type=sprite\nnamespace=core\n"
        f"address=UI/Users/{name}\nname={name}\ntexture={texture}\n"
        f"rect=0,0,{width},{height}\npivot=0.5,0.5\nborder=0,0,0,0\npixels_per_unit={ppu:g}\n"
        f"filter={filt}\naniso=1\nwrap_u=1\nwrap_v=1\nmipmaps=false\n"
        f"vertices=-{x:g},{y:g};{x:g},{y:g};-{x:g},-{y:g};{x:g},-{y:g}\n"
        "triangles=0,1,2,2,1,3\nuv=0,1;1,1;0,0;1,0\n"
    )
    return texture, text.encode("utf-8")


def extract(target: Path) -> None:
    if target.exists():
        shutil.rmtree(target)
    packer("extract", str(BUNDLE), str(target))


def run(command: str) -> int:
    data, group = catalog()
    if command != "check" and sha256(BUNDLE) != group["sha256"]:
        raise ImportError_("Installed USERS does not match the catalog; refusing to start.")
    names = [] if command == "check" else candidates(data)
    tree = WORK / command
    extract(tree)
    descriptors = read_descriptors(tree)
    failures = []
    if command == "check":
        if sha256(BUNDLE) != group["sha256"] or BUNDLE.stat().st_size != group["size"]:
            failures.append("bundle hash/size differs from catalog")
        failures += [f"still loose-only: {name}" for name in candidates(data)]
        names = [png.stem for png in sorted(LOOSE.glob("*.png")) if (DROP / (png.stem + "_new.png")).is_file()
                 and (m := descriptors.get(f"ui/users/{png.stem}".lower())) is not None
                 and f"texture=textures/upscaled/{png.stem}.png" in m.read_text(encoding="utf-8")]
        for name in names:
            texture, text = sprite(name)
            meta = descriptors.get(f"ui/users/{name}".lower())
            if meta is None or meta.read_bytes() != text:
                failures.append(f"descriptor missing/stale: {name}")
            elif sha256(tree / texture) != sha256(DROP / (name + "_new.png")):
                failures.append(f"texture differs: {name}")
        for failure in failures:
            print("FAIL", failure)
        print(f"{len(names)} upscaled avatars {'verified' if not failures else 'FAILED'}")
        return 1 if failures else 0

    print(f"{len(names)} loose-only avatars with upscales: {', '.join(names)}")
    if command == "plan":
        return 0
    next_index = max((int(p.stem) for p in (tree / "assets").glob("*.meta") if p.stem.isdigit()), default=-1) + 1
    for name in names:
        texture, text = sprite(name)
        (tree / texture).parent.mkdir(parents=True, exist_ok=True)
        shutil.copyfile(DROP / (name + "_new.png"), tree / texture)
        meta = descriptors.get(f"ui/users/{name}".lower())
        if meta is None:
            meta = tree / "assets" / f"{next_index:06d}.meta"
            next_index += 1
        meta.write_bytes(text)
        group["assets"].append({"address": f"UI/Users/{name}", "texture": "", "sprites": "", "audio": "", "font": ""})
    output = WORK / (GROUP + ".tar.lz4")
    output.unlink(missing_ok=True)
    packer("pack", str(tree), str(output))
    packer("verify", str(output))
    info = dict(line.split("=", 1) for line in packer("info", str(output)).split() if "=" in line)
    group["sha256"], group["size"], group["unpackedSize"] = info["sha256"], int(info["size"]), int(info["unpackedSize"])
    shutil.copyfile(output, BUNDLE)
    CATALOG.write_text(json.dumps(data, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    print(f"Installed {len(names)} upscaled avatars; USERS bundle {info['size']} bytes.")
    return 0


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("command", choices=("plan", "apply", "check"))
    try:
        return run(parser.parse_args().command)
    except ImportError_ as error:
        print("ERROR:", error, file=sys.stderr)
        return 1


if __name__ == "__main__":
    sys.exit(main())
