"""Move core SF2 model geometry XML into the TAR/LZ4 runtime-content catalog.

The model documents are XML only as a serialization format. They describe runtime model
geometry/physics (nodes, edges, capsules and triangles), so they are intentionally treated as
opaque runtime assets rather than gameplay/config XML. Location params, quests, items, settings,
and other mod-facing XML remain outside TAR.
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
VANILLA_MODELS = ROOT / "Assets/vanillaXml/models"
LEGACY_MODELS = ROOT / "Assets/Resources/gamedata/models"
CATALOG = ROOT / "Assets/Resources/SF2Content/Art/catalog.json"
STREAMING = ROOT / "Assets/StreamingAssets/SF2Content/ArtBundles"
WORK = ROOT / "Library/SF2DEModelMigration"
SOURCE = WORK / "source"
BACKUP = WORK / "backup"
DRAFT_ARCHIVE = WORK / "MODELS.tar.lz4"
DRAFT_CATALOG = WORK / "catalog.json"
PACKER_PROJECT = ROOT / "Tools/AssetPacker/AssetPacker.csproj"
PACKER = ROOT / "Tools/AssetPacker/bin/Release/net9.0/AssetPacker.dll"
ARCHIVE_NAME = "MODELS.tar.lz4"
GROUP_NAME = "MODELS"


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


def collect_models() -> dict[str, Path]:
    models: dict[str, Path] = {}
    if VANILLA_MODELS.is_dir():
        for path in sorted(VANILLA_MODELS.glob("*.xml"), key=lambda x: x.name.casefold()):
            models[path.stem.casefold()] = path
    if LEGACY_MODELS.is_dir():
        for path in sorted(LEGACY_MODELS.glob("*.txt"), key=lambda x: x.name.casefold()):
            models.setdefault(path.stem.casefold(), path)
    if not models:
        raise RuntimeError("No source model documents found")
    return models


def validate_model(path: Path, payload: bytes) -> None:
    try:
        root = ET.fromstring(payload)
    except ET.ParseError as exc:
        raise RuntimeError(f"Invalid model XML {path}: {exc}") from exc
    if root.tag != "Scene" or root.find("Figures") is None:
        raise RuntimeError(f"Model is not a Scene/Figures document: {path}")


def meta_text(stem: str, file_name: str) -> str:
    return (
        "type=model\n"
        "namespace=core\n"
        f"address=gamedata/models/{stem}\n"
        f"name={stem}\n"
        f"file=models/{file_name}\n"
    )


def generate() -> None:
    ensure_packer()
    models = collect_models()
    if WORK.exists():
        shutil.rmtree(WORK)
    (SOURCE / "models").mkdir(parents=True)
    (SOURCE / "assets").mkdir(parents=True)

    assets: list[dict[str, str]] = []
    total = 0
    for folded, source in sorted(models.items()):
        stem = source.stem
        payload = source.read_bytes()
        validate_model(source, payload)
        file_name = stem + ".xml"
        (SOURCE / "models" / file_name).write_bytes(payload)
        (SOURCE / "assets" / (stem + ".meta")).write_text(meta_text(stem, file_name), encoding="utf-8")
        assets.append({"address": f"gamedata/models/{stem}", "texture": "", "sprites": "",
                       "audio": "", "font": ""})
        total += len(payload)

    subprocess.run(["dotnet", str(PACKER), "pack", str(SOURCE), str(DRAFT_ARCHIVE)], cwd=ROOT, check=True)
    info = archive_info(DRAFT_ARCHIVE)

    catalog = json.loads(CATALOG.read_text(encoding="utf-8-sig"))
    if catalog.get("version") != 3:
        raise RuntimeError("Model migration requires the v3 TAR/LZ4 catalog")
    bundles = [x for x in catalog.get("bundles", []) if x.get("name", "").casefold() != GROUP_NAME.casefold()]
    bundles.append({
        "name": GROUP_NAME,
        "namespaceId": "core",
        "file": ARCHIVE_NAME,
        "sha256": info["sha256"],
        "size": info["size"],
        "unpackedSize": info["unpackedSize"],
        "assets": assets,
    })
    catalog["bundles"] = bundles
    DRAFT_CATALOG.write_text(json.dumps(catalog, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    print(f"Generated {len(assets)} models, {total} source bytes -> {DRAFT_ARCHIVE}")
    print(f"Compressed size: {info['size']} bytes")


def archive_info(path: Path) -> dict[str, int | str]:
    completed = subprocess.run(["dotnet", str(PACKER), "info", str(path)], cwd=ROOT,
                               check=True, capture_output=True, text=True)
    result: dict[str, int | str] = {}
    for line in completed.stdout.splitlines():
        key, sep, value = line.partition("=")
        if sep:
            result[key] = int(value) if key in {"size", "unpackedSize"} else value
    if result.get("sha256") != sha256(path) or result.get("size") != path.stat().st_size:
        raise RuntimeError("AssetPacker info mismatch for model archive")
    return result


def verify() -> None:
    ensure_packer()
    if not DRAFT_ARCHIVE.is_file() or not DRAFT_CATALOG.is_file():
        raise RuntimeError("Generate the model migration first")
    subprocess.run(["dotnet", str(PACKER), "verify", str(DRAFT_ARCHIVE)], cwd=ROOT, check=True)
    draft = json.loads(DRAFT_CATALOG.read_text(encoding="utf-8"))
    model_group = next((x for x in draft["bundles"] if x.get("name") == GROUP_NAME), None)
    if model_group is None or len(model_group.get("assets") or []) < 500:
        raise RuntimeError("Draft catalog does not contain the complete model group")
    info = archive_info(DRAFT_ARCHIVE)
    for key in ("size", "unpackedSize", "sha256"):
        if model_group.get(key) != info[key]:
            raise RuntimeError("Draft model catalog metadata mismatch: " + key)
    print(f"Verified {len(model_group['assets'])} model assets in {ARCHIVE_NAME}")


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

    # Destructive cleanup is deliberately last. The archive and catalog above have already
    # passed the independent packer validator and hash/size verification.
    if BACKUP.exists():
        shutil.rmtree(BACKUP)
    BACKUP.mkdir(parents=True)
    if VANILLA_MODELS.is_dir():
        shutil.copytree(VANILLA_MODELS, BACKUP / "vanillaXml-models")
    if LEGACY_MODELS.is_dir():
        shutil.copytree(LEGACY_MODELS, BACKUP / "Resources-models")
    safe_remove_tree(VANILLA_MODELS)
    safe_remove_file(Path(str(VANILLA_MODELS) + ".meta"))
    safe_remove_tree(LEGACY_MODELS)
    safe_remove_file(Path(str(LEGACY_MODELS) + ".meta"))
    print("Committed MODELS.tar.lz4 and removed imported/core loose model trees")


def safe_remove_tree(path: Path) -> None:
    if not path.exists():
        return
    resolved = path.resolve()
    assets = (ROOT / "Assets").resolve()
    if assets not in resolved.parents:
        raise RuntimeError("Refusing to remove path outside Assets: " + str(resolved))
    shutil.rmtree(resolved)


def safe_remove_file(path: Path) -> None:
    if path.is_file():
        path.unlink()


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as source:
        for chunk in iter(lambda: source.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


if __name__ == "__main__":
    main()
