#!/usr/bin/env python3
"""One-time bridge from the current Unity-native art tree to TAR/LZ4 v3.

This intentionally reads the generated NativeSpriteAtlas YAML rather than the original research
AssetBundles. It therefore migrates the canonical project state exactly as it exists today.

Usage:
    python Tools/MigrateNativeArtToTar.py generate
    python Tools/MigrateNativeArtToTar.py commit

`generate` is non-destructive. It writes verified archives and a draft catalog under
Library/TarAssetMigration/python-generated. `commit` refuses to run unless that complete generated
set validates, then moves the archives/fonts into their v3 locations and removes the old imported
art groups.
"""

from __future__ import annotations

import argparse
import hashlib
import io
import json
import os
from pathlib import Path
import re
import shutil
import struct
import subprocess
import sys
import tarfile
from typing import Any, Iterable


ROOT = Path(__file__).resolve().parents[1]
OLD_ROOT = ROOT / "Assets/Resources/SF2Content/Art"
CATALOG = OLD_ROOT / "catalog.json"
GENERATED = ROOT / "Library/TarAssetMigration/python-generated"
GENERATED_BUNDLES = GENERATED / "bundles"
DRAFT_CATALOG = GENERATED / "catalog.json"
STATS_FILE = GENERATED / "stats.json"
STREAMING_ROOT = ROOT / "Assets/StreamingAssets/SF2Content/ArtBundles"
FONT_ROOT = ROOT / "Assets/Resources/SF2Content/Fonts"
PACKER_DLL = ROOT / "Tools/AssetPacker/bin/Release/net9.0/AssetPacker.dll"

FLOAT_RE = r"[-+]?(?:\d+(?:\.\d*)?|\.\d+)(?:[eE][-+]?\d+)?"


class MigrationError(RuntimeError):
    pass


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("command", choices=("generate", "commit", "verify-generated"))
    args = parser.parse_args()
    try:
        if args.command == "generate":
            generate()
        elif args.command == "commit":
            commit()
        else:
            verify_generated(verbose=True)
        return 0
    except Exception as exc:
        print(f"ERROR: {exc}", file=sys.stderr)
        return 1


def generate() -> None:
    catalog = load_catalog()
    if catalog["version"] == 3:
        raise MigrationError("Project already uses catalog v3")
    if catalog["version"] != 2:
        raise MigrationError(f"Expected catalog v2, got {catalog['version']}")
    ensure_packer()

    if GENERATED.exists():
        shutil.rmtree(GENERATED)
    GENERATED_BUNDLES.mkdir(parents=True)

    new_groups: list[dict[str, Any]] = []
    stats = {
        "groups": len(catalog["bundles"]),
        "archives": 0,
        "sprites": 0,
        "custom_mesh_sprites": 0,
        "audio": 0,
        "fonts": 0,
        "source_payload_bytes": 0,
        "tar_bytes": 0,
        "compressed_bytes": 0,
        "max_uv_error": 0.0,
    }

    for index, group in enumerate(catalog["bundles"], 1):
        print(f"[{index:02d}/{len(catalog['bundles'])}] {group['name']}", flush=True)
        new_group = generate_group(group, stats)
        new_groups.append(new_group)

    draft = {"version": 3, "bundles": new_groups, "files": []}
    DRAFT_CATALOG.write_text(json.dumps(draft, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    STATS_FILE.write_text(json.dumps(stats, indent=2) + "\n", encoding="utf-8")
    verify_generated(verbose=False)
    print(json.dumps(stats, indent=2))
    print(f"Generated catalog: {DRAFT_CATALOG}")


def generate_group(group: dict[str, Any], stats: dict[str, Any]) -> dict[str, Any]:
    bundle_name = safe_name(group["name"]) + ".tar.lz4"
    tar_path = GENERATED / (safe_name(group["name"]) + ".tar")
    compressed_path = GENERATED_BUNDLES / bundle_name
    next_assets: list[dict[str, str]] = []
    has_archive = False

    with tarfile.open(tar_path, "w", format=tarfile.USTAR_FORMAT) as archive:
        for asset_index, asset in enumerate(group["assets"]):
            next_asset = {
                "address": asset["address"],
                "texture": "",
                "sprites": "",
                "audio": "",
                "font": "",
            }
            texture_resource = asset.get("texture") or ""
            sprites_resource = asset.get("sprites") or ""
            audio_resource = asset.get("audio") or ""
            font_resource = asset.get("font") or ""

            if texture_resource or sprites_resource:
                if not (texture_resource and sprites_resource):
                    raise MigrationError(f"Texture/sprite pair incomplete: {group['name']}/{asset['address']}")
                add_sprite_asset(archive, group, asset, asset_index, stats)
                has_archive = True

            if audio_resource:
                add_audio_asset(archive, asset, asset_index, stats)
                has_archive = True

            if font_resource:
                source = resource_file(font_resource, ".ttf")
                target = FONT_ROOT / safe_name(group["name"]) / source.name
                next_asset["font"] = resource_path(target)
                stats["fonts"] += 1

            next_assets.append(next_asset)

    next_group: dict[str, Any] = {
        "name": group["name"],
        "namespaceId": "core",
        "file": "",
        "sha256": "",
        "size": 0,
        "unpackedSize": 0,
        "assets": next_assets,
    }

    if not has_archive:
        tar_path.unlink()
        return next_group

    subprocess.run(
        ["dotnet", str(PACKER_DLL), "compress", str(tar_path), str(compressed_path)],
        cwd=ROOT,
        check=True,
        stdout=subprocess.DEVNULL,
    )
    subprocess.run(
        ["dotnet", str(PACKER_DLL), "verify", str(compressed_path)],
        cwd=ROOT,
        check=True,
        stdout=subprocess.DEVNULL,
    )

    next_group["file"] = bundle_name
    next_group["sha256"] = sha256(compressed_path)
    next_group["size"] = compressed_path.stat().st_size
    next_group["unpackedSize"] = tar_path.stat().st_size
    stats["archives"] += 1
    stats["tar_bytes"] += tar_path.stat().st_size
    stats["compressed_bytes"] += compressed_path.stat().st_size
    tar_path.unlink()
    return next_group


def add_sprite_asset(
    archive: tarfile.TarFile,
    group: dict[str, Any],
    asset: dict[str, Any],
    asset_index: int,
    stats: dict[str, Any],
) -> None:
    texture_path = resource_file(asset["texture"], ".png")
    sprite_path = resource_file(asset["sprites"], ".asset")
    texture_entry = f"textures/{asset_index:04d}_{safe_name(texture_path.name)}"
    add_disk_file(archive, texture_entry, texture_path)
    stats["source_payload_bytes"] += texture_path.stat().st_size

    texture_settings = parse_texture_meta(Path(str(texture_path) + ".meta"))
    texture_width, texture_height = png_size(texture_path)
    sprites = parse_native_sprite_atlas(sprite_path)
    if not sprites:
        raise MigrationError(f"Sprite atlas is empty: {sprite_path}")

    for sprite_index, sprite in enumerate(sprites):
        verify_sprite_uv(sprite, texture_width, texture_height, stats)
        if len(sprite["vertices"]) != 4:
            stats["custom_mesh_sprites"] += 1
        meta_name = f"assets/{asset_index:04d}_{sprite_index:04d}_{safe_name(sprite['name'])}.meta"
        metadata = serialize_sprite_meta(
            asset["address"], texture_entry, sprite, texture_settings
        ).encode("utf-8")
        add_bytes(archive, meta_name, metadata)
        stats["sprites"] += 1


def add_audio_asset(
    archive: tarfile.TarFile,
    asset: dict[str, Any],
    asset_index: int,
    stats: dict[str, Any],
) -> None:
    audio_path = resource_file(asset["audio"], ".wav")
    validate_pcm16_wav(audio_path)
    entry = f"audio/{asset_index:04d}_{safe_name(audio_path.name)}"
    add_disk_file(archive, entry, audio_path)
    name = audio_path.stem
    meta = "".join(
        [
            "type=audio\n",
            "namespace=core\n",
            f"address={escape(asset['address'])}\n",
            f"name={escape(name)}\n",
            f"file={escape(entry)}\n",
        ]
    ).encode("utf-8")
    add_bytes(archive, f"assets/{asset_index:04d}_{safe_name(name)}.meta", meta)
    stats["audio"] += 1
    stats["source_payload_bytes"] += audio_path.stat().st_size


def parse_native_sprite_atlas(path: Path) -> list[dict[str, Any]]:
    text = path.read_text(encoding="utf-8")
    first_sprite = text.find("--- !u!213 &")
    if first_sprite < 0:
        raise MigrationError(f"No Sprite documents in {path}")
    header = text[:first_sprite]
    ordered_ids = re.findall(r"^\s*- \{fileID: (-?\d+)\}\s*$", header, re.MULTILINE)
    docs: dict[str, dict[str, Any]] = {}
    pieces = re.split(r"(?=^--- !u!213 &-?\d+\s*$)", text[first_sprite:], flags=re.MULTILINE)
    for piece in pieces:
        if not piece.strip():
            continue
        match = re.match(r"--- !u!213 &(-?\d+)\s*$", piece, re.MULTILINE)
        if not match:
            continue
        file_id = match.group(1)
        docs[file_id] = parse_sprite_document(piece, path)
    if not ordered_ids:
        ordered_ids = list(docs)
    missing = [file_id for file_id in ordered_ids if file_id not in docs]
    if missing:
        raise MigrationError(f"Atlas references missing Sprite documents in {path}: {missing[:4]}")
    return [docs[file_id] for file_id in ordered_ids]


def parse_sprite_document(doc: str, path: Path) -> dict[str, Any]:
    name = require_match(doc, r"^\s*m_Name:\s*(.*?)\s*$", path)
    rect_block = require_match(doc, r"^\s*m_Rect:\s*\n((?:\s{4,}.*\n){5})", path)
    rect = tuple(
        parse_number(require_match(rect_block, rf"^\s*{field}:\s*({FLOAT_RE})\s*$", path))
        for field in ("x", "y", "width", "height")
    )
    border = parse_inline_vector(
        require_match(doc, r"^\s*m_Border:\s*\{([^}]*)\}\s*$", path),
        ("x", "y", "z", "w"),
        path,
    )
    ppu = float(require_match(doc, rf"^\s*m_PixelsToUnits:\s*({FLOAT_RE})\s*$", path))
    pivot = parse_inline_vector(
        require_match(doc, r"^\s*m_Pivot:\s*\{([^}]*)\}\s*$", path), ("x", "y"), path
    )

    rd_match = re.search(r"^\s*m_RD:\s*$([\s\S]*?)^\s*m_AtlasRD:\s*$", doc, re.MULTILINE)
    if not rd_match:
        raise MigrationError(f"Cannot isolate m_RD in {path}/{name}")
    rd = rd_match.group(1)
    vertex_count = int(require_match(rd, r"^\s*m_VertexCount:\s*(\d+)\s*$", path))
    raw_hex = require_match(rd, r"^\s*_typelessdata:\s*([0-9a-fA-F]+)\s*$", path)
    raw = bytes.fromhex(raw_hex)
    # Unity stores each vertex stream at a 16-byte aligned offset. Positions are stream 0
    # (float3) and UV0 is stream 1 (float2) in the NativeSpriteAtlas files produced by our importer.
    uv_offset = align(vertex_count * 12, 16)
    expected_size = uv_offset + vertex_count * 8
    if len(raw) != expected_size:
        raise MigrationError(
            f"Unexpected vertex stream layout in {path}/{name}: {len(raw)} != {expected_size}"
        )
    position_bytes = raw[: vertex_count * 12]
    uv_bytes = raw[uv_offset:]
    positions3 = struct.unpack("<" + "f" * (vertex_count * 3), position_bytes)
    uv_values = struct.unpack("<" + "f" * (vertex_count * 2), uv_bytes)
    vertices = [(positions3[i], positions3[i + 1]) for i in range(0, len(positions3), 3)]
    uvs = [(uv_values[i], uv_values[i + 1]) for i in range(0, len(uv_values), 2)]

    index_hex = require_match(rd, r"^\s*m_IndexBuffer:\s*([0-9a-fA-F]+)\s*$", path)
    index_bytes = bytes.fromhex(index_hex)
    if len(index_bytes) % 2:
        raise MigrationError(f"Odd sprite index buffer in {path}/{name}")
    triangles = list(struct.unpack("<" + "H" * (len(index_bytes) // 2), index_bytes))

    return {
        "name": name,
        "rect": rect,
        "border": border,
        "pixels_per_unit": ppu,
        "pivot": pivot,
        "vertices": vertices,
        "triangles": triangles,
        "uv": uvs,
    }


def parse_texture_meta(path: Path) -> dict[str, Any]:
    text = path.read_text(encoding="utf-8")
    texture_settings = require_match(
        text,
        r"^\s*textureSettings:\s*$([\s\S]*?)^\s*nPOTScale:",
        path,
    )
    return {
        "filter": int(require_match(texture_settings, r"^\s*filterMode:\s*(-?\d+)\s*$", path)),
        "aniso": int(require_match(texture_settings, r"^\s*aniso:\s*(-?\d+)\s*$", path)),
        "wrap_u": int(require_match(texture_settings, r"^\s*wrapU:\s*(-?\d+)\s*$", path)),
        "wrap_v": int(require_match(texture_settings, r"^\s*wrapV:\s*(-?\d+)\s*$", path)),
        "mipmaps": require_match(text, r"^\s*enableMipMap:\s*(\d+)\s*$", path) == "1",
    }


def serialize_sprite_meta(
    address: str,
    texture: str,
    sprite: dict[str, Any],
    texture_settings: dict[str, Any],
) -> str:
    rect = sprite["rect"]
    pivot = sprite["pivot"]
    border = sprite["border"]
    vertices = ";".join(f"{f32(x)},{f32(y)}" for x, y in sprite["vertices"])
    triangles = ",".join(str(value) for value in sprite["triangles"])
    uvs = ";".join(f"{f32(x)},{f32(y)}" for x, y in sprite["uv"])
    return "".join(
        [
            "type=sprite\n",
            "namespace=core\n",
            f"address={escape(address)}\n",
            f"name={escape(sprite['name'])}\n",
            f"texture={escape(texture)}\n",
            f"rect={','.join(f32(v) for v in rect)}\n",
            f"pivot={','.join(f32(v) for v in pivot)}\n",
            f"border={','.join(f32(v) for v in border)}\n",
            f"pixels_per_unit={f32(sprite['pixels_per_unit'])}\n",
            f"filter={texture_settings['filter']}\n",
            f"aniso={texture_settings['aniso']}\n",
            f"wrap_u={texture_settings['wrap_u']}\n",
            f"wrap_v={texture_settings['wrap_v']}\n",
            f"mipmaps={'true' if texture_settings['mipmaps'] else 'false'}\n",
            f"vertices={vertices}\n",
            f"triangles={triangles}\n",
            f"uv={uvs}\n",
        ]
    )


def verify_sprite_uv(sprite: dict[str, Any], width: int, height: int, stats: dict[str, Any]) -> None:
    x, y, w, h = sprite["rect"]
    px, py = sprite["pivot"]
    ppu = sprite["pixels_per_unit"]
    maximum = 0.0
    for vertex, actual in zip(sprite["vertices"], sprite["uv"], strict=True):
        expected = (
            (x + px * w + vertex[0] * ppu) / width,
            (y + py * h + vertex[1] * ppu) / height,
        )
        maximum = max(maximum, abs(expected[0] - actual[0]), abs(expected[1] - actual[1]))
    stats["max_uv_error"] = max(stats["max_uv_error"], maximum)
    if maximum > 1e-5:
        raise MigrationError(f"Sprite UV cannot be derived by OverrideGeometry for {sprite['name']}: {maximum}")


def verify_generated(verbose: bool) -> None:
    if not DRAFT_CATALOG.exists() or not STATS_FILE.exists():
        raise MigrationError("Generated migration set is missing. Run generate first.")
    catalog = json.loads(DRAFT_CATALOG.read_text(encoding="utf-8"))
    if catalog.get("version") != 3:
        raise MigrationError("Generated catalog is not v3")
    archives = 0
    for group in catalog["bundles"]:
        file = group.get("file") or ""
        if not file:
            if any(not asset.get("font") for asset in group["assets"]):
                raise MigrationError(f"Loose-only group has non-font assets: {group['name']}")
            continue
        archive = GENERATED_BUNDLES / file
        if not archive.exists() or archive.stat().st_size != group["size"]:
            raise MigrationError(f"Generated archive missing/size mismatch: {archive}")
        if sha256(archive) != group["sha256"]:
            raise MigrationError(f"Generated archive hash mismatch: {archive}")
        subprocess.run(
            ["dotnet", str(PACKER_DLL), "verify", str(archive)],
            cwd=ROOT,
            check=True,
            stdout=None if verbose else subprocess.DEVNULL,
        )
        archives += 1
    stats = json.loads(STATS_FILE.read_text(encoding="utf-8"))
    if archives != stats["archives"]:
        raise MigrationError(f"Archive count mismatch: {archives} != {stats['archives']}")
    if verbose:
        print(f"Generated set OK: {archives} archives")


def commit() -> None:
    current = load_catalog()
    if current["version"] == 3:
        raise MigrationError("Project already uses v3; refusing a second commit")
    if current["version"] != 2:
        raise MigrationError("Current catalog is no longer the expected v2 source")
    verify_generated(verbose=False)
    draft = json.loads(DRAFT_CATALOG.read_text(encoding="utf-8"))

    # Confirm routing shape did not drift between generation and commit.
    if [g["name"] for g in current["bundles"]] != [g["name"] for g in draft["bundles"]]:
        raise MigrationError("Source catalog groups changed since generation")
    for old_group, new_group in zip(current["bundles"], draft["bundles"], strict=True):
        if [a["address"] for a in old_group["assets"]] != [a["address"] for a in new_group["assets"]]:
            raise MigrationError(f"Source catalog addresses changed since generation: {old_group['name']}")

    STREAMING_ROOT.mkdir(parents=True, exist_ok=True)
    wanted_archives = set()
    for group in draft["bundles"]:
        file = group.get("file") or ""
        if not file:
            continue
        wanted_archives.add(file.lower())
        shutil.copy2(GENERATED_BUNDLES / file, STREAMING_ROOT / file)

    # Move fonts and their Unity .meta files before removing the source groups. GUIDs survive.
    moved_fonts: set[Path] = set()
    for old_group, new_group in zip(current["bundles"], draft["bundles"], strict=True):
        for old_asset, new_asset in zip(old_group["assets"], new_group["assets"], strict=True):
            font = old_asset.get("font") or ""
            if not font:
                continue
            source = resource_file(font, ".ttf")
            if source in moved_fonts:
                continue
            target = ROOT / "Assets/Resources" / (new_asset["font"] + ".ttf")
            target.parent.mkdir(parents=True, exist_ok=True)
            move_with_meta(source, target)
            moved_fonts.add(source)

    CATALOG.write_text(json.dumps(draft, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")

    # Destructive step remains last.
    for group in current["bundles"]:
        directory = OLD_ROOT / group["name"]
        if directory.exists():
            shutil.rmtree(directory)
        folder_meta = Path(str(directory) + ".meta")
        if folder_meta.exists():
            folder_meta.unlink()

    print(
        f"Committed {len(wanted_archives)} TAR/LZ4 archives, moved {len(moved_fonts)} fonts, "
        "and removed the old imported art groups."
    )


def load_catalog() -> dict[str, Any]:
    if not CATALOG.exists():
        raise MigrationError(f"Missing catalog: {CATALOG}")
    return json.loads(CATALOG.read_text(encoding="utf-8"))


def ensure_packer() -> None:
    subprocess.run(
        ["dotnet", "build", str(ROOT / "Tools/AssetPacker/AssetPacker.csproj"), "-c", "Release", "--nologo"],
        cwd=ROOT,
        check=True,
        stdout=subprocess.DEVNULL,
    )
    if not PACKER_DLL.exists():
        raise MigrationError(f"AssetPacker did not build: {PACKER_DLL}")


def add_disk_file(archive: tarfile.TarFile, name: str, path: Path) -> None:
    with path.open("rb") as source:
        data = source.read()
    add_bytes(archive, name, data)


def add_bytes(archive: tarfile.TarFile, name: str, data: bytes) -> None:
    normalized = normalize_tar_path(name)
    info = tarfile.TarInfo(normalized)
    info.size = len(data)
    info.mtime = 0
    info.mode = 0o644
    info.uid = 0
    info.gid = 0
    info.uname = ""
    info.gname = ""
    archive.addfile(info, io.BytesIO(data))


def normalize_tar_path(value: str) -> str:
    value = value.replace("\\", "/").strip().lstrip("/")
    parts = value.split("/")
    if not value or ":" in value or any(part in ("", ".", "..") for part in parts):
        raise MigrationError(f"Unsafe TAR path: {value}")
    encoded = value.encode("utf-8")
    if len(encoded) > 255:
        raise MigrationError(f"USTAR path too long: {value}")
    return value


def resource_file(resource: str, extension: str) -> Path:
    path = ROOT / "Assets/Resources" / (resource.replace("\\", "/") + extension)
    if not path.exists():
        raise MigrationError(f"Missing resource payload: {path}")
    return path


def resource_path(path: Path) -> str:
    root = ROOT / "Assets/Resources"
    relative = path.relative_to(root).as_posix()
    return relative[: -len(path.suffix)] if path.suffix else relative


def move_with_meta(source: Path, target: Path) -> None:
    source_meta = Path(str(source) + ".meta")
    target_meta = Path(str(target) + ".meta")
    if not source_meta.exists():
        raise MigrationError(f"Cannot preserve Unity GUID; .meta missing: {source_meta}")
    if target.exists() or target_meta.exists():
        raise MigrationError(f"Font migration target already exists: {target}")
    shutil.move(source, target)
    shutil.move(source_meta, target_meta)


def validate_pcm16_wav(path: Path) -> None:
    data = path.read_bytes()
    if len(data) < 44 or data[:4] != b"RIFF" or data[8:12] != b"WAVE":
        raise MigrationError(f"Invalid WAV: {path}")
    position = 12
    fmt: tuple[int, int] | None = None
    found_data = False
    while position + 8 <= len(data):
        chunk = data[position : position + 4]
        size = struct.unpack_from("<I", data, position + 4)[0]
        payload = position + 8
        end = payload + size
        if end > len(data):
            raise MigrationError(f"Truncated WAV chunk: {path}")
        if chunk == b"fmt " and size >= 16:
            audio_format, channels, _, _, _, bits = struct.unpack_from("<HHIIHH", data, payload)
            fmt = (audio_format, bits)
            if channels <= 0:
                raise MigrationError(f"Invalid WAV channel count: {path}")
        elif chunk == b"data":
            found_data = True
        position = end + (size & 1)
    if fmt != (1, 16) or not found_data:
        raise MigrationError(f"v1 runtime only supports PCM16 WAV: {path}")


def png_size(path: Path) -> tuple[int, int]:
    with path.open("rb") as file:
        header = file.read(24)
    if len(header) < 24 or header[:8] != b"\x89PNG\r\n\x1a\n" or header[12:16] != b"IHDR":
        raise MigrationError(f"Not a PNG: {path}")
    return struct.unpack(">II", header[16:24])


def parse_inline_vector(text: str, names: Iterable[str], path: Path) -> tuple[float, ...]:
    values = {}
    for part in text.split(","):
        key, separator, value = part.strip().partition(":")
        if not separator:
            raise MigrationError(f"Malformed inline vector in {path}: {text}")
        values[key.strip()] = float(value.strip())
    try:
        return tuple(values[name] for name in names)
    except KeyError as exc:
        raise MigrationError(f"Missing vector component {exc} in {path}: {text}") from exc


def require_match(text: str, pattern: str, path: Path) -> str:
    match = re.search(pattern, text, re.MULTILINE)
    if not match:
        raise MigrationError(f"Cannot parse {path}; missing pattern: {pattern}")
    return match.group(1)


def parse_number(value: str) -> float:
    number = float(value)
    return int(number) if number.is_integer() else number


def f32(value: float | int) -> str:
    return format(float(value), ".9g")


def align(value: int, alignment: int) -> int:
    return (value + alignment - 1) // alignment * alignment


def escape(value: str) -> str:
    return str(value).replace("\\", "\\\\").replace("\r", "\\r").replace("\n", "\\n")


def safe_name(value: str) -> str:
    invalid = '<>:"/\\|?*'
    result = "".join("_" if ch in invalid or ord(ch) < 32 else ch for ch in str(value))
    return result[:120] or "unnamed"


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as source:
        for chunk in iter(lambda: source.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


if __name__ == "__main__":
    raise SystemExit(main())
