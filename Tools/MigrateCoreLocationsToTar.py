#!/usr/bin/env python3
"""Recover the former loose core location art into one exact-path TAR/LZ4 archive.

The source is the safety backup made when the loose Resources location tree was removed.
Unity itself opens those exported Sprite assets in a disposable project and emits v3 sprite
descriptors, so tight meshes/normalized TexturePacker rotations remain authoritative.

Usage:
    python Tools/MigrateCoreLocationsToTar.py generate
    python Tools/MigrateCoreLocationsToTar.py verify-generated
    python Tools/MigrateCoreLocationsToTar.py commit
"""

from __future__ import annotations

import argparse
import hashlib
import json
import os
from pathlib import Path
import shutil
import subprocess
import sys
import uuid


ROOT = Path(__file__).resolve().parents[1]
BACKUP = ROOT / "Library/SF2DELocationDataMigration/backup"
LOCATIONS = BACKUP / "Locations"
EFFECT_ATLASES = BACKUP / "Location_effects-atlases"
WORK = ROOT / "Library/CoreLocationMigration"
FIXTURE = WORK / "UnityProject"
EXPORT = WORK / "export"
META = WORK / "metadata"
ARCHIVE = WORK / "CORE_LOCATIONS.tar.lz4"
CATALOG = ROOT / "Assets/Resources/SF2Content/Art/catalog.json"
STREAMING = ROOT / "Assets/StreamingAssets/SF2Content/ArtBundles"
INSTALLED = STREAMING / "CORE_LOCATIONS.tar.lz4"
PACKER = ROOT / "Tools/AssetPacker/bin/Release/net9.0/AssetPacker.dll"
EXPORTER = ROOT / "Tools/CoreLocationExporter.cs"
DEFAULT_UNITY = Path(r"F:\UnityInstalls\2022.3.62f3\Editor\Unity.exe")
GROUP_NAME = "CORE_LOCATIONS"


class MigrationError(RuntimeError):
    pass


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("command", choices=("generate", "verify-generated", "commit"))
    parser.add_argument("--unity", type=Path, default=DEFAULT_UNITY)
    args = parser.parse_args()
    try:
        if args.command == "generate":
            generate(args.unity)
        elif args.command == "verify-generated":
            verify_generated(verbose=True)
        else:
            commit()
        return 0
    except Exception as exc:
        print(f"ERROR: {exc}", file=sys.stderr)
        return 1


def generate(unity: Path) -> None:
    validate_sources()
    ensure_packer()
    unity = unity.resolve()
    if not unity.is_file():
        raise MigrationError(f"Unity editor not found: {unity}")

    if WORK.exists():
        shutil.rmtree(WORK)
    (FIXTURE / "Assets/Resources/Textures/Locations").mkdir(parents=True)
    (FIXTURE / "Assets/Resources/Textures/Location_effects/atlases").mkdir(parents=True)
    (FIXTURE / "Assets/Editor").mkdir(parents=True)
    (FIXTURE / "Packages").mkdir(parents=True)
    (FIXTURE / "ProjectSettings").mkdir(parents=True)
    EXPORT.mkdir(parents=True)
    META.mkdir(parents=True)

    copy_tree_contents(LOCATIONS, FIXTURE / "Assets/Resources/Textures/Locations")
    copy_tree_contents(EFFECT_ATLASES, FIXTURE / "Assets/Resources/Textures/Location_effects/atlases")
    shutil.copy2(EXPORTER, FIXTURE / "Assets/Editor/CoreLocationExporter.cs")
    (FIXTURE / "Packages/manifest.json").write_text('{"dependencies":{}}\n', encoding="utf-8")
    (FIXTURE / "ProjectSettings/ProjectVersion.txt").write_text(
        "m_EditorVersion: 2022.3.62f3\n", encoding="utf-8"
    )

    log = WORK / "unity-export.log"
    environment = os.environ.copy()
    environment["SF2DE_CORE_LOCATION_OUTPUT"] = str(EXPORT.resolve())
    completed = subprocess.run(
        [
            str(unity),
            "-batchmode",
            "-nographics",
            "-quit",
            "-projectPath",
            str(FIXTURE.resolve()),
            "-executeMethod",
            "CoreLocationExporter.Run",
            "-logFile",
            str(log.resolve()),
        ],
        cwd=ROOT,
        env=environment,
    )
    if completed.returncode != 0:
        tail = log.read_text(encoding="utf-8", errors="ignore")[-12000:] if log.exists() else "<no log>"
        raise MigrationError(f"Unity core-location export failed ({completed.returncode}):\n{tail}")
    log_text = log.read_text(encoding="utf-8", errors="ignore")
    if "[CoreLocationExport] PASS:" not in log_text:
        raise MigrationError("Unity exited without the core-location PASS marker.")

    addresses = EXPORT / "addresses.txt"
    stats = EXPORT / "stats.txt"
    if not addresses.is_file() or not stats.is_file():
        raise MigrationError("Unity exporter did not produce address/stat manifests.")
    shutil.move(str(addresses), META / "addresses.txt")
    shutil.move(str(stats), META / "stats.txt")

    subprocess.run(
        ["dotnet", str(PACKER), "pack", str(EXPORT), str(ARCHIVE)], cwd=ROOT, check=True
    )
    info = packer_info(ARCHIVE)
    addresses_list = read_addresses()
    if not addresses_list:
        raise MigrationError("Core-location export contains no logical addresses.")

    record = make_group(addresses_list, info)
    (META / "group.json").write_text(
        json.dumps(record, indent=2, ensure_ascii=False) + "\n", encoding="utf-8"
    )
    verify_generated(verbose=False)
    print((META / "stats.txt").read_text(encoding="utf-8").strip())
    print(
        f"Generated {ARCHIVE.name}: {len(addresses_list)} exact resource addresses, "
        f"{info['size']} compressed bytes."
    )


def verify_generated(verbose: bool) -> None:
    if not ARCHIVE.is_file() or not (META / "group.json").is_file():
        raise MigrationError("Generated core-location migration set is missing. Run generate first.")
    ensure_packer()
    subprocess.run(
        ["dotnet", str(PACKER), "verify", str(ARCHIVE)],
        cwd=ROOT,
        check=True,
        stdout=None if verbose else subprocess.DEVNULL,
    )
    info = packer_info(ARCHIVE)
    group = json.loads((META / "group.json").read_text(encoding="utf-8"))
    if group.get("name") != GROUP_NAME or group.get("file") != ARCHIVE.name:
        raise MigrationError("Generated group identity is invalid.")
    if group.get("size") != info["size"] or group.get("unpackedSize") != info["unpackedSize"]:
        raise MigrationError("Generated group size metadata differs from archive.")
    if group.get("sha256") != info["sha256"]:
        raise MigrationError("Generated group SHA-256 differs from archive.")
    addresses = read_addresses()
    if [a.get("address") for a in group.get("assets", [])] != addresses:
        raise MigrationError("Generated group address inventory drifted.")
    if verbose:
        print(f"Generated core locations OK: {len(addresses)} addresses, sha256={info['sha256']}")


def commit() -> None:
    verify_generated(verbose=False)
    catalog = json.loads(CATALOG.read_text(encoding="utf-8-sig"))
    if catalog.get("version") != 3 or not catalog.get("bundles"):
        raise MigrationError("Current packaged-art catalog is not v3.")
    generated = json.loads((META / "group.json").read_text(encoding="utf-8"))
    generated_addresses = {a["address"].lower() for a in generated["assets"]}
    # The former loose Resources tree had priority over all packaged art. Put this group first
    # so exact duplicate addresses preserve that same precedence.
    catalog["bundles"] = [
        group for group in catalog["bundles"]
        if group.get("name", "").lower() != GROUP_NAME.lower()
    ]
    catalog["bundles"].insert(0, generated)
    STREAMING.mkdir(parents=True, exist_ok=True)
    shutil.copy2(ARCHIVE, INSTALLED)
    CATALOG.write_text(json.dumps(catalog, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    write_unity_meta(INSTALLED)

    # Sanity-check the bug that motivated this migration before declaring success.
    moon_required = {
        "textures/locations/moon/moon_bg",
        "textures/locations/moon/moon_clouds",
        "textures/locations/moon/moon_atlas_layer1",
        "textures/locations/moon/moon_atlas_layer2",
        "textures/locations/moon/moon_atlas_layer3",
        "textures/locations/moon/moon_atlas_layer4",
        "textures/locations/moon/layer3",
    }
    missing = sorted(moon_required - generated_addresses)
    if missing:
        raise MigrationError("Installed core locations are missing Moon requirements: " + ", ".join(missing))
    print(f"Committed {INSTALLED.name} with {len(generated_addresses)} exact former-Resources addresses.")


def validate_sources() -> None:
    if not LOCATIONS.is_dir() or not any(LOCATIONS.rglob("*.png")):
        raise MigrationError(f"Core location backup is missing: {LOCATIONS}")
    if not EFFECT_ATLASES.is_dir() or not any(EFFECT_ATLASES.rglob("*.png")):
        raise MigrationError(f"Location-effect atlas backup is missing: {EFFECT_ATLASES}")
    if not EXPORTER.is_file():
        raise MigrationError(f"Unity exporter source is missing: {EXPORTER}")


def copy_tree_contents(source: Path, destination: Path) -> None:
    for child in source.iterdir():
        target = destination / child.name
        if child.is_dir():
            shutil.copytree(child, target)
        else:
            shutil.copy2(child, target)


def ensure_packer() -> None:
    subprocess.run(
        ["dotnet", "build", str(ROOT / "Tools/AssetPacker/AssetPacker.csproj"), "-c", "Release", "--nologo"],
        cwd=ROOT,
        check=True,
        stdout=subprocess.DEVNULL,
    )
    if not PACKER.is_file():
        raise MigrationError(f"AssetPacker did not build: {PACKER}")


def packer_info(path: Path) -> dict[str, int | str]:
    completed = subprocess.run(
        ["dotnet", str(PACKER), "info", str(path)], cwd=ROOT, check=True,
        capture_output=True, text=True
    )
    result: dict[str, int | str] = {}
    for line in completed.stdout.splitlines():
        key, sep, value = line.partition("=")
        if not sep:
            continue
        result[key] = int(value) if key in {"size", "unpackedSize"} else value
    if result.get("size") != path.stat().st_size or result.get("sha256") != sha256(path):
        raise MigrationError("AssetPacker archive info does not match the generated file.")
    return result


def read_addresses() -> list[str]:
    path = META / "addresses.txt"
    if not path.is_file():
        raise MigrationError("Core-location address manifest is missing.")
    values = [line.strip() for line in path.read_text(encoding="utf-8-sig").splitlines() if line.strip()]
    folded = [value.lower() for value in values]
    if len(folded) != len(set(folded)):
        raise MigrationError("Core-location exporter emitted duplicate/case-colliding addresses.")
    return values


def make_group(addresses: list[str], info: dict[str, int | str]) -> dict:
    return {
        "name": GROUP_NAME,
        "namespaceId": "core",
        "file": ARCHIVE.name,
        "sha256": info["sha256"],
        "size": info["size"],
        "unpackedSize": info["unpackedSize"],
        "assets": [
            {"address": address, "texture": "", "sprites": "", "audio": "", "font": ""}
            for address in addresses
        ],
    }


def write_unity_meta(path: Path) -> None:
    meta = Path(str(path) + ".meta")
    if meta.exists():
        return
    meta.write_text(
        "fileFormatVersion: 2\n"
        f"guid: {uuid.uuid4().hex}\n"
        "DefaultImporter:\n"
        "  externalObjects: {}\n"
        "  userData: \n"
        "  assetBundleName: \n"
        "  assetBundleVariant: \n",
        encoding="utf-8",
    )


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as source:
        for chunk in iter(lambda: source.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


if __name__ == "__main__":
    raise SystemExit(main())
