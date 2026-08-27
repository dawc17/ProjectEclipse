"""Recover named Unity sprite atlases as standalone frames plus Cocos plist metadata."""

from __future__ import annotations

import argparse
import re
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

sys.path.insert(0, str(Path(__file__).with_name("python-packages")))

import UnityPy  # noqa: E402


def render_data(sprite):
    atlas_ptr = getattr(sprite, "m_SpriteAtlas", None)
    if atlas_ptr:
        atlas = atlas_ptr.deref_parse_as_object()
        key = sprite.m_RenderDataKey
        return next(value for candidate, value in atlas.m_RenderDataMap if candidate == key)
    return sprite.m_RD


def object_key(assets_file, path_id: int) -> tuple[str, int]:
    """Unity path IDs are scoped to a SerializedFile, not the whole bundle."""
    return (assets_file.name, path_id)


def frame_sort_key(frame: tuple) -> tuple[int, str]:
    """Sort trailing frame numbers numerically instead of 1, 10, 11, 2."""
    name = frame[0]
    match = re.search(r"_(\d+)$", name)
    return (int(match.group(1)) if match else sys.maxsize, name)


def add_key_value(dictionary, key: str, tag: str, value: str | None = None):
    ET.SubElement(dictionary, "key").text = key
    element = ET.SubElement(dictionary, tag)
    if value is not None:
        element.text = value


def write_plist(target: Path, atlas_name: str, frames: list[tuple]):
    plist = ET.Element("plist", {"version": "1.0"})
    root = ET.SubElement(plist, "dict")
    ET.SubElement(root, "key").text = "frames"
    frame_dict = ET.SubElement(root, "dict")
    max_width = 1
    max_height = 1
    for name, width, height, source_width, source_height, offset_x, offset_y in frames:
        ET.SubElement(frame_dict, "key").text = name + ".png"
        values = ET.SubElement(frame_dict, "dict")
        add_key_value(values, "frame", "string", f"{{{{0,0}},{{{width},{height}}}}}")
        add_key_value(values, "offset", "string", f"{{{offset_x:g},{offset_y:g}}}")
        add_key_value(values, "rotated", "false")
        add_key_value(values, "sourceColorRect", "string", f"{{{{0,0}},{{{width},{height}}}}}")
        add_key_value(values, "sourceSize", "string", f"{{{source_width:g},{source_height:g}}}")
        max_width = max(max_width, width)
        max_height = max(max_height, height)
    ET.SubElement(root, "key").text = "metadata"
    metadata = ET.SubElement(root, "dict")
    add_key_value(metadata, "format", "integer", "2")
    add_key_value(metadata, "realTextureFileName", "string", atlas_name + ".png")
    add_key_value(metadata, "size", "string", f"{{{max_width},{max_height}}}")
    add_key_value(metadata, "textureFileName", "string", atlas_name + ".png")
    ET.indent(plist, space="  ")
    target.write_text('<?xml version="1.0" encoding="UTF-8"?>\n' + ET.tostring(plist, encoding="unicode"), encoding="utf-8")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("bundle", type=Path)
    parser.add_argument("output", type=Path)
    parser.add_argument("atlas", nargs="+", help="Exact Texture2D names to recover")
    args = parser.parse_args()

    requested = {name.lower(): name for name in args.atlas}
    env = UnityPy.load(str(args.bundle))
    # A bundle may contain several serialized files that reuse the same path ID
    # and may also contain low/high copies with the same Texture2D name.  Select
    # one highest-resolution texture per requested atlas, then associate sprites
    # by (serialized-file, path-id).  Keying by path-id alone mixed unrelated
    # magic atlases and produced apparently random frames at runtime.
    texture_candidates: dict[str, tuple[int, tuple[str, int], str]] = {}
    sprites = []
    for obj in env.objects:
        if obj.type.name == "Texture2D":
            data = obj.read()
            if data.m_Name.lower() in requested and not data.m_Name.lower().endswith("_low"):
                key = object_key(obj.assets_file, obj.path_id)
                score = int(getattr(data, "m_Width", 0)) * int(getattr(data, "m_Height", 0))
                current = texture_candidates.get(data.m_Name.lower())
                if current is None or score > current[0]:
                    texture_candidates[data.m_Name.lower()] = (score, key, data.m_Name)
        elif obj.type.name == "Sprite":
            sprites.append(obj)

    texture_names = {candidate[1]: candidate[2] for candidate in texture_candidates.values()}
    grouped: dict[str, list[tuple]] = {name: [] for name in texture_names.values()}
    args.output.mkdir(parents=True, exist_ok=True)
    for obj in sprites:
        try:
            sprite = obj.read()
            data = render_data(sprite)
            texture_id = object_key(data.texture.assetsfile, data.texture.path_id)
            atlas_name = texture_names.get(texture_id)
            if not atlas_name:
                continue
            image = sprite.image
            safe_name = sprite.m_Name.replace("/", "_").replace("\\", "_")
            image.save(args.output / f"{safe_name}.png")
            rect_width = max(float(sprite.m_Rect.width), 1.0)
            rect_height = max(float(sprite.m_Rect.height), 1.0)
            scale_x = image.width / rect_width
            scale_y = image.height / rect_height
            source_width = max(rect_width * scale_x, image.width)
            source_height = max(rect_height * scale_y, image.height)
            offset = sprite.m_Offset
            grouped[atlas_name].append(
                (safe_name, image.width, image.height, source_width, source_height,
                 float(offset.x) * scale_x, float(offset.y) * scale_y)
            )
        except Exception as exc:
            print(f"skip sprite path_id={obj.path_id}: {exc}", file=sys.stderr)

    for atlas_name, frames in grouped.items():
        frames.sort(key=frame_sort_key)
        write_plist(args.output / f"{atlas_name}_xml.txt", atlas_name, frames)
        print(f"{atlas_name}: frames={len(frames)}")
    missing = sorted(set(requested.values()) - set(grouped))
    for name in missing:
        print(f"missing texture: {name}", file=sys.stderr)
    return 0 if grouped else 1


if __name__ == "__main__":
    raise SystemExit(main())
