"""Move immutable location atlas/plist metadata into the TAR/LZ4 runtime catalog.

Location params stay loose. This tool only packages the TexturePacker-style XML TextAssets
living beside location sprites, because those describe how immutable art is laid out.
"""

from __future__ import annotations

from pathlib import Path
import argparse
import hashlib
import json
import shutil
import subprocess
import xml.etree.ElementTree as ET


ROOT = Path(__file__).resolve().parent.parent
RESOURCES = ROOT / "Assets/Resources"
SOURCES = (
    RESOURCES / "Textures/Locations",
    RESOURCES / "Textures/Location_effects/atlases",
)
CATALOG = ROOT / "Assets/Resources/SF2Content/Art/catalog.json"
STREAMING = ROOT / "Assets/StreamingAssets/SF2Content/ArtBundles"
WORK = ROOT / "Library/SF2DELocationDataMigration"
SOURCE = WORK / "source"
DRAFT_ARCHIVE = WORK / "LOCATION_DATA.tar.lz4"
DRAFT_CATALOG = WORK / "catalog.json"
PACKER_PROJECT = ROOT / "Tools/AssetPacker/AssetPacker.csproj"
PACKER = ROOT / "Tools/AssetPacker/bin/Release/net9.0/AssetPacker.dll"
ARCHIVE_NAME = "LOCATION_DATA.tar.lz4"
GROUP_NAME = "LOCATION_DATA"


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("command", choices=("generate", "verify", "commit"))
    args = parser.parse_args()
    if args.command == "generate":
        generate()
    elif args.command == "verify":
        verify()
    else:
        commit()


def ensure_packer() -> None:
    subprocess.run(["dotnet", "build", str(PACKER_PROJECT), "-c", "Release", "--nologo"],
                   cwd=ROOT, check=True, stdout=subprocess.DEVNULL)
    if not PACKER.is_file():
        raise RuntimeError("AssetPacker build did not produce " + str(PACKER))


def collect() -> list[Path]:
    files: list[Path] = []
    for source in SOURCES:
        if source.is_dir():
            files.extend(source.rglob("*.txt"))
    files.sort(key=lambda x: x.relative_to(RESOURCES).as_posix().casefold())
    if not files:
        raise RuntimeError("No loose location atlas metadata found")
    return files


def validate_xml(path: Path, payload: bytes) -> None:
    try:
        root = ET.fromstring(payload)
    except ET.ParseError as exc:
        raise RuntimeError(f"Invalid location atlas XML {path}: {exc}") from exc
    if root.tag.lower() not in {"plist", "dict"}:
        raise RuntimeError(f"Unexpected location atlas root {root.tag!r}: {path}")


def meta_text(address: str, name: str, file_name: str) -> str:
    return (
        "type=atlas\n"
        "namespace=core\n"
        f"address={address}\n"
        f"name={name}\n"
        f"file={file_name}\n"
    )


def generate() -> None:
    ensure_packer()
    files = collect()
    if WORK.exists():
        shutil.rmtree(WORK)
    (SOURCE / "assets").mkdir(parents=True)

    records: list[dict[str, str]] = []
    total = 0
    for index, source in enumerate(files):
        payload = source.read_bytes()
        validate_xml(source, payload)
        relative = source.relative_to(RESOURCES).as_posix()
        address = relative[:-4]  # strip Unity's .txt extension, like a Resources TextAsset lookup
        payload_path = "data/" + relative
        destination = SOURCE / Path(payload_path)
        destination.parent.mkdir(parents=True, exist_ok=True)
        destination.write_bytes(payload)
        descriptor = SOURCE / "assets" / f"{index:04d}_{source.stem}.meta"
        descriptor.write_text(meta_text(address, source.stem, payload_path), encoding="utf-8")
        records.append({"address": address, "texture": "", "sprites": "", "audio": "", "font": ""})
        total += len(payload)

    folded = [x["address"].casefold() for x in records]
    if len(folded) != len(set(folded)):
        raise RuntimeError("Location atlas metadata contains duplicate/case-colliding resource addresses")

    subprocess.run(["dotnet", str(PACKER), "pack", str(SOURCE), str(DRAFT_ARCHIVE)], cwd=ROOT, check=True)
    info = archive_info(DRAFT_ARCHIVE)
    catalog = json.loads(CATALOG.read_text(encoding="utf-8-sig"))
    if catalog.get("version") != 3:
        raise RuntimeError("Location-data migration requires the v3 TAR/LZ4 catalog")
    bundles = [x for x in catalog.get("bundles", []) if x.get("name", "").casefold() != GROUP_NAME.casefold()]
    bundles.append({
        "name": GROUP_NAME,
        "namespaceId": "core",
        "file": ARCHIVE_NAME,
        "sha256": info["sha256"],
        "size": info["size"],
        "unpackedSize": info["unpackedSize"],
        "assets": records,
    })
    catalog["bundles"] = bundles
    DRAFT_CATALOG.write_text(json.dumps(catalog, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    print(f"Generated {len(records)} location atlas records, {total} source bytes -> {DRAFT_ARCHIVE}")
    print(f"Compressed size: {info['size']} bytes")


def verify() -> None:
    ensure_packer()
    if not DRAFT_ARCHIVE.is_file() or not DRAFT_CATALOG.is_file():
        raise RuntimeError("Generate the location-data migration first")
    subprocess.run(["dotnet", str(PACKER), "verify", str(DRAFT_ARCHIVE)], cwd=ROOT, check=True)
    draft = json.loads(DRAFT_CATALOG.read_text(encoding="utf-8"))
    group = next((x for x in draft["bundles"] if x.get("name") == GROUP_NAME), None)
    if group is None or len(group.get("assets") or []) < 250:
        raise RuntimeError("Draft catalog does not contain the complete location-data group")
    info = archive_info(DRAFT_ARCHIVE)
    for key in ("size", "unpackedSize", "sha256"):
        if group.get(key) != info[key]:
            raise RuntimeError("Draft location-data catalog metadata mismatch: " + key)
    print(f"Verified {len(group['assets'])} location atlas assets in {ARCHIVE_NAME}")


def commit() -> None:
    verify()
    destination = STREAMING / ARCHIVE_NAME
    STREAMING.mkdir(parents=True, exist_ok=True)
    archive_tmp = destination.with_suffix(destination.suffix + ".tmp")
    catalog_tmp = CATALOG.with_suffix(".json.tmp")
    shutil.copy2(DRAFT_ARCHIVE, archive_tmp)
    shutil.copy2(DRAFT_CATALOG, catalog_tmp)
    archive_tmp.replace(destination)
    catalog_tmp.replace(CATALOG)
    print("Committed LOCATION_DATA.tar.lz4; loose source files are intentionally retained until Unity coverage passes")


def archive_info(path: Path) -> dict[str, int | str]:
    completed = subprocess.run(["dotnet", str(PACKER), "info", str(path)], cwd=ROOT,
                               check=True, capture_output=True, text=True)
    result: dict[str, int | str] = {}
    for line in completed.stdout.splitlines():
        key, sep, value = line.partition("=")
        if sep:
            result[key] = int(value) if key in {"size", "unpackedSize"} else value
    if result.get("sha256") != sha256(path) or result.get("size") != path.stat().st_size:
        raise RuntimeError("AssetPacker info mismatch for location-data archive")
    return result


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as source:
        for chunk in iter(lambda: source.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


if __name__ == "__main__":
    main()
