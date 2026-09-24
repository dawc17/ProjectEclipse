#!/usr/bin/env python3
"""Move core locations to the upscaled ("<name>_new") versions from the owner's asset drop.

Source: ResearchSources/de128_assets (owner-supplied 2026-09-23). For every location that has
``gamedata/locations/<name>_new/<name>_new_params.xml`` this tool does what the earlier manual
mountain transition did:

* every top-level PNG in ``assets/Locations/<name>_new/`` becomes a full-quad sprite descriptor
  (1 pixel per unit, point filter, centre pivot: the drop's own import settings) at the existing
  address ``Textures/Locations/<name>/<png>``. An existing descriptor at that address is
  replaced; old atlases and other addresses stay in the bundle for compatibility.
* the DE params replace both ``Assets/vanillaXml/locations/<name>/<name>_params.xml`` (dev XML,
  read first) and ``Assets/Resources/gamedata/locations/<name>/params.txt`` (fallback). Layer
  ``Path="Locations/<x>_new/"`` becomes ``Path="Locations/<x>/"``; nothing else is edited.
* every local Image/Picture reference in the new params must resolve to a sprite in the bundle.

It never writes Unity YAML or sprite vertex data from Unity assets; descriptors are the
runtime TAR format documented in Tools/AssetPacker/README.md.

Usage:
    python Tools/ImportUpscaledLocations.py plan      # report, change nothing
    python Tools/ImportUpscaledLocations.py apply     # rebuild bundle, params and catalog
    python Tools/ImportUpscaledLocations.py check     # verify the installed result
"""

from __future__ import annotations

import argparse
import hashlib
import json
import re
import shutil
import subprocess
import sys
import struct
import xml.etree.ElementTree as ET  # parses only the owner's local drop and repository files
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]

DROP = ROOT / "ResearchSources" / "de128_assets"
DROP_ART = DROP / "assets" / "Locations"
DROP_PARAMS = DROP / "gamedata" / "locations"
VANILLA = ROOT / "Assets" / "vanillaXml" / "locations"
RESOURCES = ROOT / "Assets" / "Resources" / "gamedata" / "locations"
CATALOG = ROOT / "Assets" / "Resources" / "SF2Content" / "Art" / "catalog.json"
BUNDLE = ROOT / "Assets" / "StreamingAssets" / "SF2Content" / "ArtBundles" / "CORE_LOCATIONS.tar.lz4"
WORK = ROOT / "Library" / "UpscaledLocations"
PACKER = ROOT / "Tools" / "AssetPacker" / "bin" / "Release" / "net9.0" / "AssetPacker.dll"
GROUP = "CORE_LOCATIONS"
# Pictures the DE params name but the drop does not contain anywhere (searched the whole of
# ResearchSources on 2026-09-24). Location.cs skips a missing picture with a warning, as the
# current base already does for these; any other unresolved reference fails the import.
KNOWN_GAPS = {
    ("fungus_raid", "fungus_raid", "layer_0_2"),   # DE 1.0.6 only has the old-resolution sprite
    ("moon", "moon", "layer_3"),                   # the drop has layer3 (already drawn)
    ("road", "road", "leafs_on"), ("road", "road", "leafs_s"),  # shared particles, not location art
    ("road", "road", "Green_leafs"), ("road", "road", "Pink_leafs"),
}
# The DE waterfall_small params still use names that exist nowhere; its parent waterfall_new
# draws the same pieces as waterfall_2 / waterfall_left_tile / waterfall_right_tile.
RENAMES = {
    "waterfall_small": {
        'ClassName="waterfall_new_2"': 'ClassName="waterfall_2"',
        'ClassName="waterfall_new_left_tile"': 'ClassName="waterfall_left_tile"',
        'ClassName="waterfall_new_right_tile"': 'ClassName="waterfall_right_tile"',
    },
}
PATH_NEW = re.compile(r'(Path="Locations/[A-Za-z0-9_]+?)_new/"')


class ImportError_(RuntimeError):
    pass


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for block in iter(lambda: handle.read(1 << 20), b""):
            digest.update(block)
    return digest.hexdigest()


def packer(*args: str) -> str:
    result = subprocess.run(["dotnet", str(PACKER), *args], capture_output=True, text=True)
    if result.returncode != 0:
        raise ImportError_("AssetPacker " + args[0] + " failed: " + result.stderr.strip())
    return result.stdout


def locations(addresses: set[str]) -> list[str]:
    """Base locations that have a DE _new params file and exist in the base game."""
    result = []
    for folder in sorted(DROP_PARAMS.iterdir()):
        if not folder.name.endswith("_new") or not (folder / (folder.name + "_params.xml")).is_file():
            continue
        name = folder.name[:-4]
        known = (VANILLA / name).is_dir() or any(a.startswith(f"textures/locations/{name}/") for a in addresses)
        if known:
            result.append(name)
    return result


def new_params(name: str) -> str:
    text = (DROP_PARAMS / (name + "_new") / (name + "_new_params.xml")).read_text(encoding="utf-8-sig")
    for old, new in RENAMES.get(name, {}).items():
        if old not in text:
            raise ImportError_(f"{name}: expected {old} in the DE params")
        text = text.replace(old, new)
    return PATH_NEW.sub(r'\1/"', text)


def png_size(path: Path) -> tuple[int, int]:
    with path.open("rb") as handle:
        header = handle.read(24)
    if header[:8] != b"\x89PNG\r\n\x1a\n" or header[12:16] != b"IHDR":
        raise ImportError_(f"Not a PNG: {path}")
    return struct.unpack(">II", header[16:24])


def descriptor(address: str, name: str, texture: str, width: int, height: int) -> str:
    # Same shape as the mountain replacement descriptors already in the bundle.
    x, y = width / 2, height / 2
    fmt = lambda v: str(int(v)) if float(v).is_integer() else repr(v)
    return (
        "type=sprite\nnamespace=core\n"
        f"address={address}\nname={name}\ntexture={texture}\n"
        f"rect=0,0,{width},{height}\npivot=0.5,0.5\nborder=0,0,0,0\npixels_per_unit=1\n"
        "filter=0\naniso=1\nwrap_u=1\nwrap_v=1\nmipmaps=false\n"
        f"vertices=-{fmt(x)},{fmt(y)};{fmt(x)},{fmt(y)};-{fmt(x)},-{fmt(y)};{fmt(x)},-{fmt(y)}\n"
        "triangles=0,1,2,2,1,3\nuv=0,1;1,1;0,0;1,0\n"
    )


def read_descriptors(tree: Path) -> dict[str, Path]:
    result = {}
    for meta in sorted((tree / "assets").glob("*.meta")):
        for line in meta.read_text(encoding="utf-8").splitlines():
            if line.startswith("address="):
                result[line[8:].lower()] = meta
                break
    return result


def references(name: str, text: str) -> list[tuple[str, str]]:
    """(folder, ClassName) for every local image the params draw."""
    result = []
    root = ET.fromstring(text.encode("utf-8"))
    for layer in root.iter("Layer"):
        path = layer.get("Path")
        folder = path.strip("/").split("/")[-1] if path else name
        for element in layer.iter():
            cn = element.get("ClassName")
            if not cn:
                continue
            if element.tag == "Image" or element.tag == "SpriteMask" or (
                    element.tag == "SimpleEffect" and element.get("Type") == "Picture"
                    and element.get("PictureLocation", "local") == "local"):
                result.append((folder, cn))
    return result


def build(tree: Path, names: list[str], report: list[str]) -> dict[str, str]:
    """Add/replace descriptors in an extracted bundle tree. Returns address -> texture path."""
    descriptors = read_descriptors(tree)
    next_index = max((int(p.stem) for p in (tree / "assets").glob("*.meta") if p.stem.isdigit()), default=-1) + 1
    installed = {}
    for name in names:
        art = DROP_ART / (name + "_new")
        if not art.is_dir():
            continue
        replaced = added = 0
        for png in sorted(art.glob("*.png")):
            texture = f"textures/{name}_replacement/{png.name}"
            target = tree / texture
            target.parent.mkdir(parents=True, exist_ok=True)
            shutil.copyfile(png, target)
            width, height = png_size(png)
            address = f"Textures/Locations/{name}/{png.stem}"
            meta = descriptors.get(address.lower())
            if meta is None:
                meta = tree / "assets" / f"{next_index:06d}.meta"
                next_index += 1
                added += 1
            else:
                replaced += 1
            meta.write_text(descriptor(address, png.stem, texture, width, height), encoding="utf-8", newline="\n")
            descriptors[address.lower()] = meta
            installed[address] = texture
        report.append(f"{name}: {replaced} replaced, {added} added")
    return installed


def sequences(name: str, text: str) -> list[str]:
    """Atlas address prefixes of every animated (Sequention) effect the params draw."""
    result = []
    for layer in ET.fromstring(text.encode("utf-8")).iter("Layer"):
        for effect in layer.iter("SimpleEffect"):
            if effect.get("Type") != "Sequention":
                continue
            if effect.get("Path"):
                base = "textures/" + effect.get("Path").strip("/")
            elif effect.get("PictureLocation") == "global":
                base = "textures/location_effects"
            else:
                base = "textures/" + (layer.get("Path") or f"locations/{name}").strip("/")
            result.append(f"{base}/atlases/{effect.get('ClassName')}".lower())
    return result


def validate(tree: Path, names: list[str]) -> list[str]:
    addresses = set(read_descriptors(tree))
    # Shared effect atlases live in other bundles too; any catalog address may satisfy them.
    catalog, _ = catalog_group()
    everything = addresses | {a["address"].lower() for g in catalog["bundles"] for a in g["assets"]}
    failures = []
    for name in names:
        text = new_params(name)
        for folder, cn in references(name, text):
            if f"textures/locations/{folder}/{cn}".lower() not in addresses and (name, folder, cn) not in KNOWN_GAPS:
                failures.append(f"{name}: {folder}/{cn}")
        for prefix in sequences(name, text):
            if not any(a.startswith(prefix) for a in everything):
                failures.append(f"{name}: animation {prefix}")
    return failures


def extract(target: Path) -> None:
    if target.exists():
        shutil.rmtree(target)
    packer("extract", str(BUNDLE), str(target))


def catalog_group() -> tuple[dict, dict]:
    catalog = json.loads(CATALOG.read_text(encoding="utf-8-sig"))
    group = next(g for g in catalog["bundles"] if g["name"] == GROUP)
    return catalog, group


def plan() -> None:
    catalog, group = catalog_group()
    if sha256(BUNDLE) != group["sha256"]:
        raise ImportError_("Installed CORE_LOCATIONS does not match the catalog; refusing to start.")
    names = locations({a["address"].lower() for a in group["assets"]})
    tree = WORK / "plan"
    extract(tree)
    report: list[str] = []
    build(tree, names, report)
    print("\n".join(report))
    failures = validate(tree, names)
    print(f"{len(names)} locations; unresolved local images: {len(failures)}")
    for failure in failures:
        print("  UNRESOLVED", failure)


def apply() -> None:
    catalog, group = catalog_group()
    if sha256(BUNDLE) != group["sha256"]:
        raise ImportError_("Installed CORE_LOCATIONS does not match the catalog; refusing to start.")
    names = locations({a["address"].lower() for a in group["assets"]})
    tree = WORK / "tree"
    extract(tree)
    report: list[str] = []
    installed = build(tree, names, report)
    failures = validate(tree, names)
    if failures:
        raise ImportError_("Unresolved local images:\n  " + "\n  ".join(failures))
    output = WORK / (GROUP + ".tar.lz4")
    output.unlink(missing_ok=True)
    packer("pack", str(tree), str(output))
    packer("verify", str(output))
    info = dict(line.split("=", 1) for line in packer("info", str(output)).split() if "=" in line)

    known = {a["address"].lower() for a in group["assets"]}
    for address in sorted(installed):
        if address.lower() not in known:
            group["assets"].append({"address": address, "texture": "", "sprites": "", "audio": "", "font": ""})
            known.add(address.lower())
    group["sha256"], group["size"], group["unpackedSize"] = info["sha256"], int(info["size"]), int(info["unpackedSize"])

    for name in names:
        text = new_params(name)
        for target in (VANILLA / name / (name + "_params.xml"), RESOURCES / name / "params.txt"):
            target.parent.mkdir(parents=True, exist_ok=True)
            target.write_text(text, encoding="utf-8", newline="\n")
    shutil.copyfile(output, BUNDLE)
    CATALOG.write_text(json.dumps(catalog, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    print("\n".join(report))
    print(f"Installed {len(names)} locations, {len(installed)} upscaled sprites; bundle {info['size']} bytes.")


def check() -> int:
    catalog, group = catalog_group()
    failures = []
    if sha256(BUNDLE) != group["sha256"] or BUNDLE.stat().st_size != group["size"]:
        failures.append("bundle hash/size differs from catalog")
    names = locations({a["address"].lower() for a in group["assets"]})
    known = {a["address"].lower() for a in group["assets"]}
    tree = WORK / "check"
    extract(tree)
    descriptors = read_descriptors(tree)
    for name in names:
        text = new_params(name)
        for target in (VANILLA / name / (name + "_params.xml"), RESOURCES / name / "params.txt"):
            if not target.is_file() or target.read_text(encoding="utf-8-sig") != text:
                failures.append(f"params differ: {target.relative_to(ROOT)}")
        art = DROP_ART / (name + "_new")
        for png in sorted(art.glob("*.png")) if art.is_dir() else []:
            address = f"textures/locations/{name}/{png.stem}".lower()
            meta = descriptors.get(address)
            texture = tree / f"textures/{name}_replacement/{png.name}"
            if address not in known:
                failures.append(f"catalog lacks {address}")
            if meta is None or f"texture=textures/{name}_replacement/{png.name}" not in meta.read_text(encoding="utf-8"):
                failures.append(f"descriptor missing/stale: {address}")
            elif not texture.is_file() or sha256(texture) != sha256(png):
                failures.append(f"texture differs: {texture.relative_to(tree)}")
    failures += ["unresolved " + f for f in validate(tree, names)]
    for failure in failures:
        print("FAIL", failure)
    print(f"{len(names)} upscaled locations {'verified' if not failures else 'FAILED'}")
    return 1 if failures else 0


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("command", choices=("plan", "apply", "check"))
    args = parser.parse_args()
    try:
        if args.command == "plan":
            plan()
        elif args.command == "apply":
            apply()
        else:
            return check()
    except ImportError_ as error:
        print("ERROR:", error, file=sys.stderr)
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
