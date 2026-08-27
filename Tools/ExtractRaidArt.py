"""Extract Underworld raid artwork into Assets/Resources using the two recovery
pipelines already proven inside this project:

1. Fight-scene location layers -> full-atlas PNG + ``<atlas>_xml.txt`` TexturePacker
   plist consumed by Editor/RecoveredLocationAtlasImporter.cs (same layout as the
   migrated bamboo_grove / american_event_* locations).
2. UI members (battle previews, RaidMisc icons, boss avatars) -> one PNG sheet plus
   standalone Sprite ``.asset`` files named ``<Atlas>.<Member>`` (same convention as
   BattleBtnBase_raid.active_vulcan_raid / MiscSprites.checkboxOn).

Only assets missing from Assets/ are written, so re-running the tool after new
bundles land under ResearchSources/NewFiles is safe and cheap.

Usage: py -3.12 Tools/ExtractRaidArt.py [--dry-run] [--bundles ...]
Without --bundles every *.bundle-like file under ResearchSources/NewFiles/bundles
is indexed (no extension there), which resolves event-location artwork too.
"""

from __future__ import annotations

import argparse
import io
import json
import struct
import sys
import uuid
import xml.etree.ElementTree as ET
from pathlib import Path

PROJECT_ROOT = Path(__file__).resolve().parent.parent
ASSETS = PROJECT_ROOT / "Assets"
RAID_XML = ASSETS / "xml" / "raid_stages_default.xml"
BUNDLE_DIR = PROJECT_ROOT / "ResearchSources" / "NewFiles" / "bundles"
DEFAULT_BUNDLES = [
    BUNDLE_DIR / "ZONE_RAID",
    BUNDLE_DIR / "VERSIONAL_ASSETS_HALLOWEEN_24",
    BUNDLE_DIR / "VERSIONAL_ASSETS_NY25",
    BUNDLE_DIR / "VERSIONAL_ASSETS_RITUAL",
    BUNDLE_DIR / "VERSIONAL_ASSETS_RITUAL_VULCAN",
    BUNDLE_DIR / "ASCENSION",
    BUNDLE_DIR / "BERSTUUK",
    BUNDLE_DIR / "CNY26",
]
LOCATIONS_ROOT = ASSETS / "Resources" / "Textures" / "locations"
UI_ATLASES = ASSETS / "Resources" / "ui" / "atlases"
UI_USERS = ASSETS / "Resources" / "ui" / "users"

sys.path.insert(0, str(Path(__file__).with_name("python-packages")))

import UnityPy  # noqa: E402


# ---------------------------------------------------------------------------
# meta templates


def png_meta(guid: str) -> str:
    return f"""fileFormatVersion: 2
guid: {guid}
timeCreated: 1787000000
licenseType: Free
TextureImporter:
  serializedVersion: 13
  internalIDToNameTable: []
  externalObjects: {{}}
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
    flipGreenChannel: 0
  isReadable: 0
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMipmapLimit: 0
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 48
  maxTextureSize: 2048
  textureSettings:
    serializedVersion: 2
    filterMode: 1
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
    wrapW: 1
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 50
  spriteMode: 2
  spriteExtrude: 1
  spriteMeshType: 0
  alignment: 9
  spritePivot: {{x: 0.5, y: 0.5}}
  spritePixelsToUnits: 1
  spriteBorder: {{x: 0, y: 0, z: 0, w: 0}}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationDetail: -1
  textureType: 8
  textureShape: 1
  singleChannelComponent: 0
  maxTextureSizeSet: 0
  compressionQualitySet: 0
  textureFormatSet: 0
  ignorePngGamma: 0
  applyGammaDecoding: 0
  cookieLightType: 0
  platformSettings:
  - serializedVersion: 3
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 2048
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 2
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    ignorePlatformSupport: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  spriteSheet:
    serializedVersion: 2
    sprites: []
    outline: []
    physicsShape: []
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def text_asset_meta(guid: str) -> str:
    return f"""fileFormatVersion: 2
guid: {guid}
timeCreated: 1787000000
licenseType: Free
TextScriptImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def sprite_meta(guid: str) -> str:
    return f"""fileFormatVersion: 2
guid: {guid}
timeCreated: 1787000000
licenseType: Free
NativeFormatImporter:
  externalObjects: {{}}
  mainObjectFileID: 21300000
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def read_guid(meta_path: Path):
    if not meta_path.exists():
        return None
    for line in meta_path.read_text(errors="ignore").splitlines():
        line = line.strip()
        if line.startswith("guid:"):
            return line.split(":", 1)[1].strip()
    return None


def write_file(path: Path, content_bytes_or_str):
    path.parent.mkdir(parents=True, exist_ok=True)
    meta_path = Path(str(path) + ".meta")
    existed = path.exists()
    if not existed:
        if isinstance(content_bytes_or_str, bytes):
            path.write_bytes(content_bytes_or_str)
        else:
            path.write_text(content_bytes_or_str, encoding="utf-8", newline="\n")
        if not meta_path.exists():
            ext = path.suffix.lower()
            if ext == ".png":
                meta_path.write_text(png_meta(uuid.uuid4().hex), newline="\n")
            elif path.suffix == ".txt":
                meta_path.write_text(text_asset_meta(uuid.uuid4().hex), newline="\n")
            elif path.suffix == ".asset":
                meta_path.write_text(sprite_meta(uuid.uuid4().hex), newline="\n")
            else:
                meta_path.write_text(f"fileFormatVersion: 2\nguid: {uuid.uuid4().hex}\n", newline="\n")
    return existed


# ---------------------------------------------------------------------------
# plist writer


def add_kv(dictionary, key, tag, value=None):
    ET.SubElement(dictionary, "key").text = key
    element = ET.SubElement(dictionary, tag)
    if value is not None:
        element.text = value


def write_plist(target: Path, frames, texture_height: int):
    """frames: list of tuples
       (name, x, y_unity_topleft, w, h, off_x, off_y, src_w, src_h)
       RecoveredLocationAtlasImporter converts back with texH - cocosY - h."""
    plist = ET.Element("plist", {"version": "1.0"})
    root = ET.SubElement(plist, "dict")
    ET.SubElement(root, "key").text = "frames"
    frames_dict = ET.SubElement(root, "dict")
    for name, x, unity_y, w, h, off_x, off_y, _sw, _sh in frames:
        cocos_y = texture_height - unity_y - h
        ET.SubElement(frames_dict, "key").text = name + ".png"
        d = ET.SubElement(frames_dict, "dict")
        add_kv(d, "frame", "string", f"{{{{{x},{cocos_y}}},{{{w},{h}}}}}")
        add_kv(d, "offset", "string", f"{{{off_x:g},{off_y:g}}}")
        add_kv(d, "rotated", "false")
        add_kv(d, "sourceColorRect", "string", f"{{{{0,0}},{{{w},{h}}}}}")
        add_kv(d, "sourceSize", "string", f"{{{_sw:g},{_sh:g}}}")
    target.parent.mkdir(parents=True, exist_ok=True)
    target.write_text('<?xml version="1.0" encoding="UTF-8"?>\n' +
                      ET.tostring(plist, encoding="unicode"), encoding="utf-8")


# ---------------------------------------------------------------------------
# bundle access


def sprite_render_data(sprite):
    atlas_ptr = getattr(sprite, "m_SpriteAtlas", None)
    if atlas_ptr:
        try:
            atlas = atlas_ptr.deref_parse_as_object()
            key = sprite.m_RenderDataKey
            return next(v for cand, v in atlas.m_RenderDataMap if cand == key)
        except Exception:
            pass
    return getattr(sprite, "m_RD", None)


class BundleStore:
    def __init__(self, bundles):
        self.textures = {}          # lowered name -> {hi: rec, low: rec}
        self.sprites_by_texture = {}  # (assets_file, path_id) -> [SpriteData]
        for b in bundles:
            env = UnityPy.load(str(b))
            raw_sprites = []
            for obj in env.objects:
                try:
                    data = obj.read()
                except Exception:
                    continue
                tname = obj.type.name
                name = getattr(data, "m_Name", "") or ""
                if tname == "Texture2D":
                    bucket = self.textures.setdefault(name.lower(), {})
                    slot = "low" if name.endswith("_low") else "hi"
                    rec = {"obj": obj, "data": data, "name": name,
                           "key": (obj.assets_file.name, obj.path_id),
                           "bundle": b.name,
                           "width": int(getattr(data, "m_Width", 0)),
                           "height": int(getattr(data, "m_Height", 0))}
                    # keep the largest candidate per slot (bundles can repeat)
                    cur = bucket.get(slot)
                    if cur is None or rec["width"] * rec["height"] > cur["width"] * cur["height"]:
                        bucket[slot] = rec
                elif tname == "Sprite":
                    raw_sprites.append((b.name, obj, data))
            for bname, obj, sprite in raw_sprites:
                try:
                    rd = sprite_render_data(sprite)
                    tex_ptr = getattr(rd, "texture", None)
                    if tex_ptr is None:
                        continue
                    key = (tex_ptr.assetsfile.name, tex_ptr.path_id)
                    self.sprites_by_texture.setdefault(key, []).append(sprite)
                except Exception:
                    continue

    def match(self, lowered_names):
        """Return dict lowered_name -> {'hi':rec,'low':rec} whose name is present."""
        out = {}
        for name in lowered_names:
            rec = self.textures.get(name.lower())
            if rec:
                out[name] = rec
        return out

    def sprites_of(self, texture_key):
        return self.sprites_by_texture.get(texture_key, [])


# ---------------------------------------------------------------------------
# frame extraction


def frame_info(sprite):
    """Compute frame placement inside the owning texture."""
    img = sprite.image  # cropped to rect/textrureRect by UnityPy
    texture = sprite_render_data(sprite).texture.deref_parse_as_object()
    rect = sprite.m_Rect
    tr = getattr(sprite_render_data(sprite), "textureRect", None)
    if tr is not None:
        tx, ty, tw, th = float(tr.x), float(tr.y), float(tr.width), float(tr.height)
    else:
        tx, ty, tw, th = float(rect.x), float(rect.y), float(rect.width), float(rect.height)
    # textureRectOffset is the trimmed rectangle's bottom-left in the original
    # sprite. Do not multiply the original size by original/cropped size again.
    off = getattr(sprite_render_data(sprite), "textureRectOffset", None)
    offset_x = float(off.x) + img.width / 2 - float(rect.width) / 2 if off else float(sprite.m_Offset.x)
    offset_y = float(off.y) + img.height / 2 - float(rect.height) / 2 if off else float(sprite.m_Offset.y)
    return {
        "name": sprite.m_Name,
        "img": img,
        "x": round(tx), "y": round(ty), "w": round(tw), "h": round(th),
        "off_x": offset_x,
        "off_y": offset_y,
        "src_w": float(rect.width),
        "src_h": float(rect.height),
        "tex_w": int(texture.m_Width), "tex_h": int(texture.m_Height),
    }


# ---------------------------------------------------------------------------
# standalone Sprite .asset writer


def export_ui_member(out_dir: Path, asset_stem: str, frame, png_abs_path: Path) -> bool:
    """Queue a Unity-native rebuild; never hand-author serialized Sprite meshes."""
    target = out_dir / (asset_stem + ".asset")
    if target.exists():
        return False
    queue_path = PROJECT_ROOT / "Temp/raid-native-sprite-queue.json"
    queue_path.parent.mkdir(exist_ok=True)
    entries = json.loads(queue_path.read_text(encoding="utf-8"))["entries"] if queue_path.exists() else []
    entry = dict(assetPath=str(target.resolve()), texturePath=str(png_abs_path.resolve()),
                 textureGuid=read_guid(Path(str(png_abs_path) + ".meta")), name=asset_stem,
                 x=frame["x"], y=frame["y"], width=frame["w"], height=frame["h"], pixelsPerUnit=100)
    entries = [e for e in entries if e["assetPath"] != entry["assetPath"]]
    entries.append(entry)
    queue_path.write_text(json.dumps({"entries": entries}, indent=2), encoding="utf-8")
    return True


# ---------------------------------------------------------------------------
# targets


def parse_targets():
    root = ET.parse(RAID_XML).getroot()
    locations = set()
    previews = {"preview_raid_base"}
    avatars = set()
    icon_atlases = set()
    for battle in root.iter("Battle"):
        loc = battle.get("Location")
        if loc:
            locations.add(loc.strip())
        prev = battle.get("Preview")
        if prev and "." in prev:
            previews.add(prev.split(".", 1)[0].strip())
        ia = battle.get("IconAtlas")
        if ia:
            icon_atlases.add(ia.strip())
        for fight in battle.iter("Fight"):
            if fight.get("Location"):
                locations.add(fight.get("Location").strip())
    for warrior in root.iter("Warrior"):
        avatar = warrior.get("Avatar")
        if avatar:
            avatars.add(avatar.strip())
    previews.add("RaidMisc")
    return locations, previews, avatars, icon_atlases


def location_atlas_names(store: BundleStore, locations):
    """For every location, collect texture records belonging to it."""
    result = {}
    for loc in sorted(locations):
        l = loc.lower() + "_"
        found = {}
        for name_lower, bucket in store.textures.items():
            if name_lower == loc.lower() or name_lower.startswith(l):
                found[name_lower] = bucket
        if found:
            result[loc] = found
    return result


def ui_sheet_records(store: BundleStore, sheet_names):
    recs = store.match(sheet_names)
    return recs


# ---------------------------------------------------------------------------
# exporters


def texture_png(rec):
    buf = io.BytesIO()
    rec["data"].image.convert("RGBA").save(buf, format="PNG")
    return buf.getvalue()


def write_sheet(rec, out_path: Path):
    try:
        png_bytes = texture_png(rec)
    except Exception as exc:
        print(f"[ExtractRaidArt] FAILED png {out_path.name}: {exc}", file=sys.stderr)
        return False
    existed = write_file(out_path, png_bytes)
    return existed


def collect_frames(sprites):
    frames = []
    warned_rotated = False
    for s in sprites:
        try:
            settings_raw = int(getattr(s, "m_SettingsRaw", 0) or 0)
            if settings_raw & 0x3 and not warned_rotated:
                print(f"[ExtractRaidArt] NOTE rotated/packed frame '{s.m_Name}' - verify",
                      file=sys.stderr)
                warned_rotated = True
            fi = frame_info(s)
        except Exception:
            continue
        frames.append((fi["name"], fi["x"], fi["y"], fi["w"], fi["h"],
                       fi["off_x"], fi["off_y"], fi["src_w"], fi["src_h"]))
    return frames


def export_location(loc, buckets, dry_run):
    base = LOCATIONS_ROOT / loc
    created = []
    for _name_lower, bucket in sorted(buckets.items()):
        # High-res sheet carries the sprite metadata; low variants are dumped
        # as plain textures because the legacy loader never requests a
        # '<atlas>_low_xml' document - texture selection is handled by the
        # importer/runtime instead.
        rec = bucket.get("hi")
        if rec is not None:
            display = rec["name"]
            png_path = base / (display + ".png")
            plist_name = display + "_xml.txt"
            if not png_path.exists():
                if not dry_run:
                    write_sheet(rec, png_path)
                created.append(f"{loc}/{display}.png")
                sprites = [s for s in store_sprites_of(rec) if s.m_Name]
                frames = collect_frames(sprites)
                target_txt = base / plist_name
                if frames and not target_txt.exists() and not dry_run:
                    write_plist(target_txt, frames, rec["height"])
                    created.append(f"{loc}/{plist_name} ({len(frames)} frames)")
        low = bucket.get("low")
        if low is not None:
            low_path = base / (low["name"] + ".png")
            if not low_path.exists():
                if not dry_run:
                    write_sheet(low, low_path)
                created.append(f"{loc}/{low['name']}.png")
    return created


def export_ui_sheet(display_name, bucket, out_dir: Path, prefix_style: bool, dry_run):
    created = []
    hi = bucket.get("hi")
    if hi is None:
        print(f"[ExtractRaidArt] missing UI sheet '{display_name}'", file=sys.stderr)
        return created
    png_path = out_dir / (display_name + ".png")
    if not png_path.exists():
        if not dry_run:
            write_sheet(hi, png_path)
        created.append(str(png_path.relative_to(ASSETS)))
    for slot, tag in (("hi", ""), ("low", "_0")):
        rec = bucket.get(slot)
        if rec is None:
            continue
        for s in store_sprites_of(rec):
            member = s.m_Name
            if not member:
                continue
            # Bundle sprite names usually already carry "<Atlas>." - avoid doubling.
            short = member.split(".", 1)[1] if "." in member and member.lower().startswith(
                display_name.lower() + ".") else member
            stem = f"{display_name}.{short}{tag}" if prefix_style and "." not in member else (
                member + tag)
            try:
                fi = frame_info(s)
            except Exception:
                continue
            if not dry_run:
                export_ui_member(out_dir, stem, fi, png_path)
            created.append(stem + ".asset")
    return created


STORE = None


def store_sprites_of(rec):
    return STORE.sprites_of(rec["key"])


# ---------------------------------------------------------------------------


def main():
    global STORE
    ap = argparse.ArgumentParser()
    ap.add_argument("--bundles", nargs="*",
                    default=[str(p) for p in sorted(BUNDLE_DIR.iterdir())]
                    if BUNDLE_DIR.exists() else [str(p) for p in DEFAULT_BUNDLES])
    ap.add_argument("--dry-run", action="store_true")
    args = ap.parse_args()

    locations, previews, avatars, icon_atlases = parse_targets()
    print(f"[ExtractRaidArt] locations={len(locations)} previews={sorted(previews)} "
          f"avatars={len(avatars)} iconAtlasses={len(icon_atlases)}")

    STORE = BundleStore([Path(b) for b in args.bundles])
    print(f"[ExtractRaidArt] indexed textures={len(STORE.textures)} "
          f"sprites={sum(len(v) for v in STORE.sprites_by_texture.values())}")

    report = []

    # ---- 1. fight-location art -------------------------------------------
    per_loc = location_atlas_names(STORE, locations)
    missing_locs = sorted(set(locations) - set(per_loc.keys()))
    if missing_locs:
        print("[ExtractRaidArt] NOTE no bundle textures for locations:", ", ".join(missing_locs),
              file=sys.stderr)
    for loc, buckets in per_loc.items():
        if loc == "vortex_raid":
            continue  # shipped complete with the legacy-style recovery already
        report += export_location(loc, buckets, args.dry_run)

    # ---- 2. UI sheets ------------------------------------------------------
    sheet_names = set(previews) | ({n for n in icon_atlases} ) | {"RaidMisc"}
    for name, bucket in ui_sheet_records(STORE, sheet_names).items():
        if name.startswith("preview"):
            report += export_ui_sheet(name, bucket, UI_ATLASES, prefix_style=True, dry_run=args.dry_run)
        elif name == "raidmisc":
            report += export_ui_sheet("RaidMisc", bucket, UI_ATLASES, prefix_style=True, dry_run=args.dry_run)
        else:
            report += export_ui_sheet(name, bucket, UI_ATLASES, prefix_style=True, dry_run=args.dry_run)

    # ---- 3. boss avatars ----------------------------------------------------
    avatar_match = {}
    for avatar in avatars:
        rec = STORE.textures.get(avatar.lower())
        if rec and rec.get("hi"):
            avatar_match[avatar] = rec
    missing_avatars = sorted(set(avatars) - set(avatar_match))
    if missing_avatars:
        print("[ExtractRaidArt] NOTE avatars not in bundles:", ", ".join(missing_avatars), file=sys.stderr)
    for avatar, bucket in avatar_match.items():
        hi = bucket["hi"]
        png_path = UI_USERS / (avatar + ".png")
        if not png_path.exists():
            if not args.dry_run:
                write_sheet(hi, png_path)
            report.append(str(png_path.relative_to(ASSETS)))
            spr = STORE.sprites_of(hi["key"])
            if spr:
                fi = frame_info(spr[0])
                if not args.dry_run:
                    export_ui_member(UI_USERS, avatar, fi, png_path)
                report.append(avatar + ".asset")

    print(f"\n[ExtractRaidArt] wrote/queued {len(report)} file(s)")
    print("[ExtractRaidArt] New UI sprites are queued for Unity-native serialization; see Tools/SPRITE_NATIVE_REBUILD.md")
    for r in report[:60]:
        print("  " + r)
    if len(report) > 60:
        print(f"  ... and {len(report) - 60} more")
    if args.dry_run:
        print("[ExtractRaidArt] dry run only - nothing written")


if __name__ == "__main__":
    main()
