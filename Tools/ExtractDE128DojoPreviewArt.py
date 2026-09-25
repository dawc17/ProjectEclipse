#!/usr/bin/env python3
"""Install or verify the owner-supplied dojo chooser previews in DE128.

Nine previews are exact copies of the owner drop's Users/dojo medallions. Their
sprite descriptors crop the transparent padding around each 612 px medallion.
The reviewed drop has no India 2024 medallion, so that preview is synthesized:
the India I medallion frame is kept and its inner art is replaced by the
India 2024 raid preview. The chooser's selection halo is also generated here.
Only the standard library is used; output is deterministic so --check can
regenerate and compare bytes.
"""

from __future__ import annotations

import argparse
import math
import struct
import zlib
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "ResearchSources/de128_assets/assets"
MOD_ASSETS = ROOT / "Mods/de128/assets"
PREVIEWS = {
    "dojo": "Users/dojo_new.png",
    "new_year_24_china_dojo": "Users/new_year_china_dojo_new.png",
    "dojo_indian_event": "Users/dojo_indian_event_new.png",
    "dojo_indian_event_22": "Users/dojo_indian_event_22_new.png",
    "haloween_dojo": "Users/haloween_dojo_new.png",
    "haloween_dojo_2019": "Users/haloween_dojo_2019_new.png",
    "dojo_hw21": "Users/dojo_hw21_new.png",
    "dojo_american_event_22": "Users/dojo_american_event_22_new.png",
    "dojo_hw22": "Users/dojo_hw22_new.png",
}
# Every owner medallion shares one 1024 px layout: the opaque disc spans
# x 201..812 and y 207..808 (top-down). Unity sprite rects are bottom-up.
MEDALLION_RECT = "[198, 208, 617, 617]"
INDIA24_FRAME = "Users/dojo_indian_event_new.png"
INDIA24_ART = "Battles/preview_raid_events_india_24.png"
# Inner art window of the shared medallion frame (top-down pixels).
INNER_CENTER = (507.0, 507.0)
INNER_RADIUS = 281.0
INNER_BOTTOM = 766
HALO_SIZE = 256


def read_png(data: bytes) -> tuple[int, int, bytearray]:
    if data[:8] != b"\x89PNG\r\n\x1a\n":
        raise ValueError("Dojo preview is not a PNG")
    pos, idat, header = 8, b"", None
    while pos < len(data):
        (length,) = struct.unpack(">I", data[pos:pos + 4])
        kind, chunk = data[pos + 4:pos + 8], data[pos + 8:pos + 8 + length]
        pos += 12 + length
        if kind == b"IHDR":
            header = struct.unpack(">IIBBBBB", chunk)
        elif kind == b"IDAT":
            idat += chunk
    width, height, depth, color, _, _, interlace = header
    if depth != 8 or color != 6 or interlace != 0:
        raise ValueError("Dojo preview must be 8-bit non-interlaced RGBA")
    raw, stride = zlib.decompress(idat), width * 4
    pixels, previous, offset = bytearray(height * stride), bytearray(stride), 0
    for y in range(height):
        kind, line = raw[offset], bytearray(raw[offset + 1:offset + 1 + stride])
        offset += 1 + stride
        for i in range(stride):
            a = line[i - 4] if i >= 4 else 0
            b = previous[i]
            c = previous[i - 4] if i >= 4 else 0
            if kind == 1:
                line[i] = (line[i] + a) & 255
            elif kind == 2:
                line[i] = (line[i] + b) & 255
            elif kind == 3:
                line[i] = (line[i] + ((a + b) >> 1)) & 255
            elif kind == 4:
                pa, pb, pc = abs(b - c), abs(a - c), abs(a + b - 2 * c)
                line[i] = (line[i] + (a if pa <= pb and pa <= pc else b if pb <= pc else c)) & 255
        pixels[y * stride:(y + 1) * stride] = line
        previous = line
    return width, height, pixels


def write_png(width: int, height: int, pixels: bytes) -> bytes:
    stride = width * 4
    raw = b"".join(b"\x00" + bytes(pixels[y * stride:(y + 1) * stride]) for y in range(height))

    def chunk(kind: bytes, body: bytes) -> bytes:
        return struct.pack(">I", len(body)) + kind + body + struct.pack(">I", zlib.crc32(kind + body))

    return (b"\x89PNG\r\n\x1a\n" + chunk(b"IHDR", struct.pack(">IIBBBBB", width, height, 8, 6, 0, 0, 0)) +
            chunk(b"IDAT", zlib.compress(raw, 9)) + chunk(b"IEND", b""))


def india24_medallion() -> bytes:
    width, height, frame = read_png((SOURCE / INDIA24_FRAME).read_bytes())
    art_w, art_h, art = read_png((SOURCE / INDIA24_ART).read_bytes())
    # Cover the inner window, centered between the two characters.
    scale = (INNER_BOTTOM - (INNER_CENTER[1] - INNER_RADIUS)) / (art_h - 4)
    focus_x, focus_y = 300.0, art_h / 2.0
    window_mid = (INNER_CENTER[1] - INNER_RADIUS + INNER_BOTTOM) / 2.0
    out = bytearray(frame)
    for y in range(height):
        if y >= INNER_BOTTOM:
            break
        for x in range(width):
            distance = math.hypot(x + 0.5 - INNER_CENTER[0], y + 0.5 - INNER_CENTER[1])
            cover = min(1.0, max(0.0, INNER_RADIUS - distance + 0.5))
            if cover <= 0.0:
                continue
            sx = min(max(focus_x + (x - INNER_CENTER[0]) / scale, 0.0), art_w - 1.001)
            sy = min(max(focus_y + (y - window_mid) / scale, 0.0), art_h - 1.001)
            x0, y0 = int(sx), int(sy)
            fx, fy = sx - x0, sy - y0
            index = (y * width + x) * 4
            for channel in range(3):
                def at(px: int, py: int) -> int:
                    return art[(py * art_w + px) * 4 + channel]
                top = at(x0, y0) * (1 - fx) + at(x0 + 1, y0) * fx
                bottom = at(x0, y0 + 1) * (1 - fx) + at(x0 + 1, y0 + 1) * fx
                sample = top * (1 - fy) + bottom * fy
                out[index + channel] = int(round(out[index + channel] * (1 - cover) + sample * cover))
            out[index + 3] = 255 if cover >= 1.0 else max(out[index + 3], int(round(255 * cover)))
    return write_png(width, height, out)


def selection_halo() -> bytes:
    # A gold rim hugging the medallion (drawn at 124/146 of the halo) with a
    # soft outer glow; the medallion covers the transparent center.
    size, center = HALO_SIZE, HALO_SIZE / 2.0
    rim_inner, rim_outer, glow_outer = 104.0, 113.0, 128.0
    out = bytearray(size * size * 4)
    for y in range(size):
        for x in range(size):
            r = math.hypot(x + 0.5 - center, y + 0.5 - center)
            if r < rim_inner - 1 or r > glow_outer:
                continue
            if r <= rim_outer:
                alpha = min(1.0, r - (rim_inner - 1.0))
                shade = 1.0 - abs((r - (rim_inner + rim_outer) / 2) / ((rim_outer - rim_inner) / 2)) * 0.35
                rgb = (int(255 * shade), int(214 * shade), int(102 * shade))
            else:
                t = (r - rim_outer) / (glow_outer - rim_outer)
                alpha, rgb = 0.75 * (1.0 - t) ** 2, (255, 222, 130)
            index = (y * size + x) * 4
            out[index:index + 4] = bytes((*rgb, int(round(255 * alpha))))
    return write_png(size, size, out)


def descriptor(name: str, rect: str | None) -> str:
    return ("type=sprite\n" f"texture=textures/dojo_changer/{name}.png\n" +
            (f"rect={rect}\n" if rect else "") + "pixels_per_unit=100\n")


def install(name: str, data: bytes, rect: str | None, check: bool, source: str) -> None:
    texture = MOD_ASSETS / "textures/dojo_changer" / f"{name}.png"
    sprite = MOD_ASSETS / "sprites/dojo_changer" / f"{name}.asset"
    text = descriptor(name, rect)
    if check:
        if texture.read_bytes() != data or sprite.read_text(encoding="utf-8") != text:
            raise ValueError(f"Packaged dojo preview differs from its source: {name}")
    else:
        texture.parent.mkdir(parents=True, exist_ok=True)
        sprite.parent.mkdir(parents=True, exist_ok=True)
        texture.write_bytes(data)
        sprite.write_text(text, encoding="utf-8", newline="\n")
    width, height = struct.unpack(">II", data[16:24])
    print(f"{name}: {width}x{height} ({source})")


def run(check: bool) -> None:
    for location, source_path in PREVIEWS.items():
        raw = (SOURCE / source_path).read_bytes()
        if struct.unpack(">II", raw[16:24]) != (1024, 1024):
            raise ValueError(f"Dojo medallion layout changed: {source_path}")
        install(location, raw, MEDALLION_RECT, check, source_path)
    install("dojo_india24", india24_medallion(), MEDALLION_RECT, check,
            f"synthesized: {INDIA24_FRAME} frame + {INDIA24_ART}")
    install("selected_halo", selection_halo(), None, check, "synthesized selection halo")


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--check", action="store_true", help="Compare existing mod art with the owner drop")
    run(parser.parse_args().check)
