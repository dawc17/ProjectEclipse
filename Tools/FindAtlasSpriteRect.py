#!/usr/bin/env python3
"""Locate a reference atlas sprite inside a recovered project atlas.

Usage:
  python Tools/FindAtlasSpriteRect.py <ourAtlas.png> <refAtlas.png> <x> <y> <w> <h>

Rects use the Unity sprite convention (origin at the bottom-left of the
texture). The reference rect is cropped from the reference atlas and located in
the recovered atlas, printing the matching Unity rect. This exists so recovered
sprite descriptors can be repaired against evidence instead of by hand.
"""
import sys
import os

sys.path.insert(0, os.path.join(os.path.dirname(os.path.abspath(__file__)), "python-packages"))

from PIL import Image  # noqa: E402


def crop_unity(image, x, y, w, h):
    top = image.height - (y + h)
    return image.crop((x, top, x + w, top + h))


def match(ours, tpl, tolerance=24):
    ow, oh = ours.size
    tw, th = tpl.size
    opx = ours.load()
    tpx = tpl.load()

    # Pick sample points that are opaque and reasonably distinctive.
    samples = []
    for sy in range(0, th, max(1, th // 7)):
        for sx in range(0, tw, max(1, tw // 7)):
            pixel = tpx[sx, sy]
            if pixel[3] > 200:
                samples.append((sx, sy, pixel))
    if not samples:
        return None
    samples = samples[:12]

    anchor_x, anchor_y, anchor_color = samples[0]
    candidates = []
    for y in range(oh):
        for x in range(ow):
            pixel = opx[x, y]
            if abs(pixel[0] - anchor_color[0]) <= tolerance and \
               abs(pixel[1] - anchor_color[1]) <= tolerance and \
               abs(pixel[2] - anchor_color[2]) <= tolerance and \
               abs(pixel[3] - anchor_color[3]) <= tolerance:
                ox = x - anchor_x
                oy = y - anchor_y
                if 0 <= ox <= ow - tw and 0 <= oy <= oh - th:
                    candidates.append((ox, oy))

    best = None
    for ox, oy in candidates:
        ok = True
        for sx, sy, color in samples[1:]:
            pixel = opx[ox + sx, oy + sy]
            if abs(pixel[0] - color[0]) > tolerance or abs(pixel[1] - color[1]) > tolerance or \
               abs(pixel[2] - color[2]) > tolerance or abs(pixel[3] - color[3]) > tolerance:
                ok = False
                break
        if not ok:
            continue
        # Full verification with a mean absolute error budget.
        total = 0
        count = 0
        for sy in range(0, th, 2):
            for sx in range(0, tw, 2):
                a = tpx[sx, sy]
                b = opx[ox + sx, oy + sy]
                total += abs(a[0] - b[0]) + abs(a[1] - b[1]) + abs(a[2] - b[2]) + abs(a[3] - b[3])
                count += 4
        error = total / float(count)
        if best is None or error < best[0]:
            best = (error, ox, oy)
    return best


def main():
    if len(sys.argv) < 7:
        print(__doc__)
        return 1
    ours_path, ref_path = sys.argv[1], sys.argv[2]
    x, y, w, h = (int(float(v)) for v in sys.argv[3:7])
    ours = Image.open(ours_path).convert("RGBA")
    ref = Image.open(ref_path).convert("RGBA")
    tpl = crop_unity(ref, x, y, w, h)
    result = match(ours, tpl)
    if result is None:
        print("no match")
        return 2
    error, ox, oy = result
    unity_y = ours.height - (oy + h)
    print("match error=%.2f  unity rect: x=%d y=%d width=%d height=%d" % (error, ox, unity_y, w, h))
    return 0


if __name__ == "__main__":
    sys.exit(main())
