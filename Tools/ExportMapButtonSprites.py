#!/usr/bin/env python3
"""Export BattleBtn atlas sprites as loose PNGs grouped by map button.

Run with Python 3.12 (the repository's Pillow wheel targets 3.12):
    py -3.12 Tools/ExportMapButtonSprites.py

The classic pressed atlas has no individual Sprite assets. It shares the
BattleBtnBase layout, so its crops use the matching base Sprite rectangles.
"""

from __future__ import annotations

import argparse
import csv
import re
import struct
import sys
import zipfile
from collections import defaultdict
from dataclasses import dataclass
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(Path(__file__).with_name("python-packages")))

from PIL import Image, ImageChops, ImageDraw  # noqa: E402


@dataclass(frozen=True)
class Sprite:
    path: Path
    atlas: str
    texture_guid: str
    key: str
    state: str
    x: float
    y: float
    width: float
    height: float
    pivot_x: float
    pivot_y: float
    pixels_per_unit: float
    vertices: tuple[tuple[float, float], ...]
    indices: tuple[int, ...]


def required(pattern: str, data: str, path: Path) -> re.Match[str]:
    match = re.search(pattern, data, re.MULTILINE)
    if match is None:
        raise ValueError(f"Missing expected Sprite field in {path}: {pattern}")
    return match


def read_sprite(path: Path) -> Sprite:
    data = path.read_text(encoding="utf-8")
    name = required(r"^  m_Name: (.+)$", data, path).group(1)
    atlas, member = name.split(".", 1)
    texture_guid = required(r"    texture: \{fileID: 2800000, guid: ([0-9a-f]+)", data, path).group(1)
    state, key = member.split("_", 1)
    if state not in {"base", "active"}:
        raise ValueError(f"Unexpected map button state in {path}: {state}")
    numbers = r"([-+\d.eE]+)"
    rect = required(
        rf"  m_Rect:\s*serializedVersion: \d+\s*x: {numbers}\s*y: {numbers}"
        rf"\s*width: {numbers}\s*height: {numbers}", data, path
    )
    pivot = required(rf"  m_Pivot: \{{x: {numbers}, y: {numbers}\}}", data, path)
    pixels_per_unit = float(required(rf"  m_PixelsToUnits: {numbers}", data, path).group(1))
    vertex_count = int(required(r"  m_VertexCount: (\d+)", data, path).group(1))
    raw = bytes.fromhex(required(r"  _typelessdata:[ \t]*([0-9a-f]*)", data, path).group(1))
    index_bytes = bytes.fromhex(required(r"  m_IndexBuffer:[ \t]*([0-9a-f]*)", data, path).group(1))
    vertices: tuple[tuple[float, float], ...] = ()
    if vertex_count:
        if len(raw) % vertex_count:
            raise ValueError(f"Vertex data length is not divisible by vertex count: {path}")
        stride = len(raw) // vertex_count
        if stride < 8:
            raise ValueError(f"Vertex stride is too short: {path}")
        vertices = tuple(struct.unpack_from("<ff", raw, i * stride) for i in range(vertex_count))
    if len(index_bytes) % 2:
        raise ValueError(f"Odd index buffer length: {path}")
    indices = tuple(struct.unpack(f"<{len(index_bytes) // 2}H", index_bytes)) if index_bytes else ()
    if any(index >= len(vertices) for index in indices):
        raise ValueError(f"Sprite mesh index exceeds vertex count: {path}")
    return Sprite(path, atlas, texture_guid, key, state, *(float(v) for v in rect.groups()),
                  *(float(v) for v in pivot.groups()), pixels_per_unit, vertices, indices)


def folder_name(key: str) -> str:
    return "BattleBtn" + "".join(part[0].upper() + part[1:] for part in key.split("_") if part)


def crop_sprite(atlas_image: Image.Image, sprite: Sprite, mesh: Sprite | None) -> Image.Image:
    # Unity's texture origin is at the lower left; Pillow's is at the upper left.
    left = round(sprite.x)
    top = round(atlas_image.height - sprite.y - sprite.height)
    right = round(sprite.x + sprite.width)
    bottom = round(atlas_image.height - sprite.y)
    if not (0 <= left < right <= atlas_image.width and 0 <= top < bottom <= atlas_image.height):
        raise ValueError(f"Sprite rectangle exceeds {sprite.atlas}.png: {sprite.path}")
    cropped = atlas_image.crop((left, top, right, bottom)).convert("RGBA")
    if mesh and mesh.indices:
        # The classic atlas keeps opaque packing bleed outside its tight mesh.
        # Draw the recovered triangles to preserve silhouettes beyond the disc.
        scale = 4
        mask = Image.new("L", (cropped.width * scale, cropped.height * scale), 0)
        draw = ImageDraw.Draw(mask)
        points = [
            ((vx * mesh.pixels_per_unit + mesh.width * mesh.pivot_x) * scale,
             (sprite.height - vy * mesh.pixels_per_unit - mesh.height * mesh.pivot_y) * scale)
            for vx, vy in mesh.vertices
        ]
        for i in range(0, len(mesh.indices), 3):
            draw.polygon([points[index] for index in mesh.indices[i:i + 3]], fill=255)
        mask = mask.resize(cropped.size, Image.Resampling.LANCZOS)
        cropped.putalpha(ImageChops.multiply(cropped.getchannel("A"), mask))
    return cropped


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--output", type=Path, default=ROOT / "Temp" / "MapButtonSprites-HighRes")
    args = parser.parse_args()
    output = args.output.resolve()
    atlas_dir = ROOT / "Assets" / "Resources" / "ui" / "atlases"
    textures_by_guid = {
        required(r"^guid: ([0-9a-f]+)$", meta.read_text(encoding="utf-8"), meta).group(1):
        meta.with_suffix("")
        for meta in atlas_dir.glob("BattleBtn*.png.meta")
    }
    groups: dict[str, dict[str, Sprite]] = defaultdict(dict)
    sprites: list[Sprite] = []
    for path in sorted(atlas_dir.glob("BattleBtn*.asset")):
        if re.search(r"\.(?:base|active)_", path.name) is None:
            continue
        sprites.append(read_sprite(path))
    raw_keys = {sprite.key for sprite in sprites}
    for sprite in sprites:
        # Recovered low/high atlas copies sometimes append _0 to the low name
        # and sometimes to the high name. Keep the larger source by dimensions.
        key = sprite.key[:-2] if sprite.key.endswith("_0") and sprite.key[:-2] in raw_keys else sprite.key
        prior = groups[key].get(sprite.state)
        if prior is None or sprite.width * sprite.height > prior.width * prior.height:
            groups[key][sprite.state] = sprite

    output.mkdir(parents=True, exist_ok=True)
    images: dict[Path, Image.Image] = {}
    rows: list[tuple[str, str, str, str]] = []
    for key, variants in sorted(groups.items()):
        folder = output / folder_name(key)
        folder.mkdir(exist_ok=True)
        mesh = variants.get("active") if variants.get("base", variants.get("active")).atlas == "BattleBtnBase" else None
        for state in ("base", "pressed", "active"):
            sprite = variants.get("base") if state == "pressed" and variants.get("base") and variants["base"].atlas == "BattleBtnBase" else variants.get(state)
            if sprite is None:
                continue
            texture_path = (atlas_dir / "BattleBtnPressed.png") if state == "pressed" else textures_by_guid.get(sprite.texture_guid)
            if texture_path is None:
                raise ValueError(f"No PNG matches texture GUID {sprite.texture_guid} in {sprite.path}")
            if texture_path not in images:
                images[texture_path] = Image.open(texture_path).convert("RGBA")
            image = crop_sprite(images[texture_path], sprite, mesh)
            if image.getchannel("A").getbbox() is None:
                raise ValueError(f"Exported image is empty: {sprite.path} using {texture_path}")
            name = f"{folder.name}.{key}_{state}.png"
            target = folder / name
            image.save(target)
            rows.append((key, state, target.relative_to(output).as_posix(),
                         ("BattleBtnPressed.png using " + sprite.path.name) if state == "pressed"
                         else texture_path.name + " / " + sprite.path.name))

    with (output / "manifest.csv").open("w", newline="", encoding="utf-8") as stream:
        writer = csv.writer(stream)
        writer.writerow(("button_id", "state", "file", "source"))
        writer.writerows(rows)
    archive = output.with_suffix(".zip")
    with zipfile.ZipFile(archive, "w", compression=zipfile.ZIP_DEFLATED) as zipped:
        zipped.write(output / "manifest.csv", "manifest.csv")
        for _, _, relative_path, _ in rows:
            zipped.write(output / relative_path, relative_path)
    counts = {state: sum(row[1] == state for row in rows) for state in ("base", "pressed", "active")}
    print(f"Exported {len(groups)} buttons, {len(rows)} PNGs ({counts}) to {output}")
    print(f"ZIP: {archive}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
