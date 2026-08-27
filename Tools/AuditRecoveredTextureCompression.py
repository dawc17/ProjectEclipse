"""Audit source bundle texture formats and Unity import settings for recovered art."""

from __future__ import annotations

import argparse
import collections
import re
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).with_name("python-packages")))

import UnityPy  # noqa: E402
from UnityPy.enums.TextureFormat import TextureFormat  # noqa: E402


RECOVERED_ROOTS = (
    "Assets/Resources/UI/Items",
    "Assets/Resources/UI/Skills",
    "Assets/Resources/UI/Enchantments",
    "Assets/Resources/textures/effects",
    "Assets/Resources/ui/users",
)
RECOVERED_ATLAS_ROOT = "Assets/Resources/Textures"

# Block-compressed mobile/desktop formats permanently discard source detail.
LOSSY_MARKERS = ("DXT", "BC", "PVRTC", "ETC", "EAC", "ASTC", "ATC")


def texture_format_name(raw_value: object) -> str:
    try:
        return TextureFormat(int(raw_value)).name
    except (TypeError, ValueError):
        return f"Unknown({raw_value})"


def audit_bundles(bundle_root: Path) -> tuple[collections.Counter[str], int]:
    formats: collections.Counter[str] = collections.Counter()
    failures = 0
    for bundle in sorted(path for path in bundle_root.rglob("*") if path.is_file()):
        try:
            environment = UnityPy.load(str(bundle))
            for obj in environment.objects:
                if obj.type.name != "Texture2D":
                    continue
                data = obj.read()
                formats[texture_format_name(data.m_TextureFormat)] += 1
        except Exception as exc:
            failures += 1
            print(f"BUNDLE_READ_FAILURE\t{bundle}\t{exc}", file=sys.stderr)
    return formats, failures


def audit_project_imports(project_root: Path) -> tuple[int, int, int, int]:
    png_count = 0
    missing_meta = 0
    compressed_default = 0
    crunched = 0
    candidates: list[Path] = []
    for relative_root in RECOVERED_ROOTS:
        root = project_root / relative_root
        if not root.exists():
            continue
        candidates.extend(root.rglob("*.png"))

    # Only TexturePacker atlases with recovered metadata are managed by the
    # recovered-atlas importer. Assets/Resources/Textures also contains many
    # unrelated decompiled textures, so auditing the whole tree is misleading.
    atlas_root = project_root / RECOVERED_ATLAS_ROOT
    if atlas_root.exists():
        for metadata in atlas_root.rglob("*_xml.txt"):
            png = Path(str(metadata)[: -len("_xml.txt")] + ".png")
            if png.exists():
                candidates.append(png)

    for png in set(candidates):
        png_count += 1
        meta = Path(str(png) + ".meta")
        if not meta.exists():
            missing_meta += 1
            continue
        text = meta.read_text(encoding="utf-8", errors="replace")
        values = re.findall(r"^\s*textureCompression:\s*(-?\d+)\s*$", text, re.MULTILINE)
        if any(value != "0" for value in values):
            compressed_default += 1
        if re.search(r"^\s*crunchedCompression:\s*1\s*$", text, re.MULTILINE):
            crunched += 1
    return png_count, missing_meta, compressed_default, crunched


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "--bundle-root",
        type=Path,
        default=Path("ResearchSources/CDNBundles/downloads"),
    )
    parser.add_argument("--project-root", type=Path, default=Path("."))
    args = parser.parse_args()

    formats, failures = audit_bundles(args.bundle_root)
    total_source = sum(formats.values())
    lossy_source = sum(
        count
        for format_name, count in formats.items()
        if format_name.startswith(LOSSY_MARKERS)
    )
    pngs, missing_meta, compressed_imports, crunched = audit_project_imports(args.project_root)

    print("SOURCE_BUNDLE_TEXTURE_FORMATS")
    for format_name, count in formats.most_common():
        classification = "LOSSY_GPU" if format_name.startswith(LOSSY_MARKERS) else "UNCOMPRESSED_OR_OTHER"
        print(f"{format_name}\t{count}\t{classification}")
    print("SUMMARY")
    print(f"source_textures={total_source}")
    print(f"source_lossy_gpu_textures={lossy_source}")
    print(f"bundle_read_failures={failures}")
    print(f"recovered_project_pngs={pngs}")
    print(f"missing_meta={missing_meta}")
    print(f"unity_compressed_imports={compressed_imports}")
    print(f"unity_crunched_imports={crunched}")
    return 1 if failures or missing_meta or compressed_imports or crunched else 0


if __name__ == "__main__":
    raise SystemExit(main())
