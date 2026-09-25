#!/usr/bin/env python3
"""Check the archived dojo choices against installed params and sprite bundles.

This is a read-only dependency check, not a Unity rendering test. Bundle extracts
live only inside a verified temporary directory and are removed on exit.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import subprocess
import tempfile
import xml.etree.ElementTree as ET
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
CATALOG = ROOT / "Assets/Resources/SF2Content/Art/catalog.json"
PACKER = ROOT / "Tools/AssetPacker/bin/Release/net9.0/AssetPacker.dll"
BUNDLES = ROOT / "Assets/StreamingAssets/SF2Content/ArtBundles"
RESOURCES = ROOT / "Assets/Resources"
CHOICES = (
    "dojo", "new_year_24_china_dojo", "dojo_indian_event",
    "dojo_indian_event_22", "dojo_india24", "haloween_dojo",
    "haloween_dojo_2019", "dojo_hw21", "dojo_american_event",
    "dojo_american_event_22", "dojo_new_year_22", "dojo_hw22",
)


def needed_layers(name: str) -> tuple[bool, list[tuple[str, str, str]], list[str]]:
    params = RESOURCES / "gamedata/locations" / name / "params.txt"
    installed = params.is_file()
    if not installed:
        # Still inspect the canonical definition so an absent resource does not
        # hide an independent art gap.
        params = ROOT / "Assets/vanillaXml/locations" / name / f"{name}_params.xml"
    if not params.is_file():
        return False, [], []
    layers = []
    sequences = []
    for layer in ET.parse(params).getroot().findall("Layer"):
        path = layer.get("Path", f"Locations/{name}/").strip("/")
        atlas = layer.get("Atlas", "")
        for image in (*layer.findall("Image"), *layer.findall("SpriteMask"),
                      *layer.findall("SimpleEffect[@Type='Picture']")):
            layers.append((path, atlas, image.get("ClassName", "")))
        for effect in layer.findall("SimpleEffect[@Type='Sequention']"):
            effect_path = effect.get("Path") or (
                "Location_effects" if effect.get("PictureLocation") == "global" else path)
            sequences.append(f"textures/{effect_path.strip('/')}/atlases/{effect.get('ClassName', '')}".lower())
    return installed, layers, sequences


def sprite_metadata(directory: Path) -> set[tuple[str, str]]:
    sprites = set()
    for meta in (directory / "assets").glob("*.meta"):
        fields = dict(line.split("=", 1) for line in meta.read_text(encoding="utf-8").splitlines()
                      if "=" in line)
        if fields.get("type") == "sprite":
            sprites.add((fields["address"].replace("\\", "/").lower(), fields["name"].lower()))
    return sprites


def audit() -> dict:
    catalog = json.loads(CATALOG.read_text(encoding="utf-8"))
    definitions = {name: needed_layers(name) for name in CHOICES}
    prefixes = {f"textures/{path}/".lower() for _, layers, _ in definitions.values()
                for path, _, _ in layers}
    sequences_needed = {sequence for _, _, sequences in definitions.values() for sequence in sequences}
    groups = [group for group in catalog["bundles"]
              if any(any(asset["address"].lower().startswith(prefix) for prefix in prefixes) or
                     asset["address"].lower() in sequences_needed
                     for asset in group["assets"])]
    temp_root = (ROOT / "Temp").resolve()
    temp_root.mkdir(exist_ok=True)
    with tempfile.TemporaryDirectory(prefix="DojoArtAudit-", dir=temp_root) as fixture:
        fixture_path = Path(fixture).resolve()
        if not fixture_path.is_relative_to(temp_root):
            raise RuntimeError("Bundle fixture escaped the workspace Temp directory")
        sprites = set()
        for group in groups:
            bundle = BUNDLES / group["file"]
            if not bundle.is_file():
                raise FileNotFoundError(f"Catalog bundle is missing: {bundle}")
            with bundle.open("rb") as stream:
                digest = hashlib.file_digest(stream, "sha256").hexdigest()
            if digest != group["sha256"]:
                raise ValueError(f"Bundle differs from the art catalog: {group['name']}")
            destination = fixture_path / group["name"]
            subprocess.run(["dotnet", str(PACKER), "extract", str(bundle), str(destination)],
                           check=True, capture_output=True, text=True)
            sprites.update(sprite_metadata(destination))

        locations = []
        addresses = {asset["address"].lower() for group in groups for asset in group["assets"]}
        for name, (installed, layers, sequences) in definitions.items():
            missing = []
            found = 0
            for path, atlas, image in layers:
                folder = RESOURCES / "textures" / path
                address = f"textures/{path}/{atlas}".lower()
                direct = f"textures/{path}/{image}".lower()
                sprite = image.lower()
                if ((folder / f"{image}.asset").is_file() or
                    (folder / f"{image}.png").is_file() or
                    (address, sprite) in sprites or (direct, sprite) in sprites):
                    found += 1
                else:
                    missing.append(f"{path}/{atlas}:{image}")
            for sequence in sequences:
                _, relative = sequence.split("/", 1)
                local = RESOURCES / "textures" / relative
                if not (local.with_name(local.name + "_xml.txt").is_file() or sequence in addresses):
                    missing.append("sequence:" + sequence)
            locations.append({"name": name, "params": installed,
                              "images": len(layers), "found": found,
                              "sequences": len(sequences),
                              "missing": sorted(set(missing))})
        return {"bundles_checked": [group["name"] for group in groups],
                "locations": locations}


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--require", action="append", choices=CHOICES, default=[],
                        help="Fail unless this dojo has params and all image sprites")
    args = parser.parse_args()
    report = audit()
    print(json.dumps(report, indent=2))
    ready = {entry["name"] for entry in report["locations"]
             if entry["params"] and not entry["missing"]}
    return 0 if all(name in ready for name in args.require) else 1


if __name__ == "__main__":
    raise SystemExit(main())
