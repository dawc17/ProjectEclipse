"""Inventory and selectively extract recoverable assets from an SF2 UnityFS bundle."""

from __future__ import annotations

import argparse
import csv
import re
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

sys.path.insert(0, str(Path(__file__).with_name("python-packages")))

import UnityPy  # noqa: E402


SAFE_NAME = re.compile(r"[^A-Za-z0-9._ -]+")


def safe_name(value: str) -> str:
    value = SAFE_NAME.sub("_", value or "unnamed").strip(" .")
    return value or "unnamed"


def object_name(obj) -> str:
    try:
        data = obj.read()
        return getattr(data, "m_Name", "") or ""
    except Exception:
        return ""


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("bundle", type=Path)
    parser.add_argument("output", type=Path)
    parser.add_argument("--extract", action="store_true")
    parser.add_argument("--name", help="Regular expression used to limit extracted object names")
    parser.add_argument("--item-xml", type=Path, help="Only extract objects named by Item Image attributes")
    args = parser.parse_args()

    env = UnityPy.load(str(args.bundle))
    rows = []
    counts = {}
    for obj in env.objects:
        type_name = obj.type.name
        counts[type_name] = counts.get(type_name, 0) + 1
        rows.append((obj.path_id, type_name, object_name(obj), obj.assets_file.name))

    args.output.mkdir(parents=True, exist_ok=True)
    with (args.output / "inventory.csv").open("w", newline="", encoding="utf-8") as handle:
        writer = csv.writer(handle)
        writer.writerow(("path_id", "type", "name", "assets_file"))
        writer.writerows(rows)

    print(f"objects={len(rows)}")
    for key in sorted(counts):
        print(f"{key}={counts[key]}")

    if not args.extract:
        return 0

    extracted = 0
    name_filter = re.compile(args.name, re.IGNORECASE) if args.name else None
    wanted_names = None
    if args.item_xml:
        wanted_names = {
            value.lower()
            for node in ET.parse(args.item_xml).getroot().iter("Item")
            if (value := node.get("Image"))
        }
    for obj in env.objects:
        try:
            data = obj.read()
            raw_name = getattr(data, "m_Name", "") or ""
            if name_filter and not name_filter.search(raw_name):
                continue
            if wanted_names is not None and raw_name.lower() not in wanted_names:
                continue
            name = safe_name(raw_name)
            if obj.type.name == "Texture2D":
                target = args.output / "Texture2D" / f"{obj.path_id}_{name}.png"
                target.parent.mkdir(parents=True, exist_ok=True)
                data.image.save(target)
                extracted += 1
            elif obj.type.name == "Sprite":
                target = args.output / "Sprite" / f"{obj.path_id}_{name}.png"
                target.parent.mkdir(parents=True, exist_ok=True)
                data.image.save(target)
                extracted += 1
            elif obj.type.name == "TextAsset":
                target = args.output / "TextAsset" / f"{obj.path_id}_{name}.bin"
                target.parent.mkdir(parents=True, exist_ok=True)
                script = data.m_Script
                # UnityPy exposes arbitrary TextAsset bytes as a string using
                # surrogate escapes. Preserve those original non-UTF8 bytes;
                # animation .bytes payloads are binary, not text.
                target.write_bytes(
                    script.encode("utf-8", errors="surrogateescape")
                    if isinstance(script, str)
                    else bytes(script)
                )
                extracted += 1
            elif obj.type.name == "AudioClip":
                # AudioClip.samples preserves embedded OGG/WAV data and converts
                # supported FMOD payloads to WAV. A clip can expose more than one
                # sample, so retain each sample's own extension and name.
                for sample_name, sample_bytes in data.samples.items():
                    target = args.output / "AudioClip" / f"{obj.path_id}_{safe_name(sample_name)}"
                    target.parent.mkdir(parents=True, exist_ok=True)
                    target.write_bytes(sample_bytes)
                    extracted += 1
        except Exception as exc:
            print(f"skip path_id={obj.path_id} type={obj.type.name}: {exc}", file=sys.stderr)
    print(f"extracted={extracted}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
