"""Validate packaged SF2DE runtime assets, or refresh archive hashes after deliberate edits."""

from __future__ import annotations

from pathlib import Path
import argparse
import hashlib
import json
import subprocess


ROOT = Path(__file__).resolve().parent.parent
ART = ROOT / "Assets/Resources/SF2Content/Art"
STREAMING = ROOT / "Assets/StreamingAssets/SF2Content/ArtBundles"
RESOURCES = ROOT / "Assets/Resources"
PACKER = ROOT / "Tools/AssetPacker/bin/Release/net9.0/AssetPacker.dll"


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=ART,
                        help="Catalog directory, normally Assets/Resources/SF2Content/Art")
    parser.add_argument("--refresh", action="store_true",
                        help="Record deliberate payload edits into the catalog")
    parser.add_argument("--deep", action="store_true",
                        help="Also parse and validate every TAR/LZ4 archive")
    args = parser.parse_args()

    art = args.root.resolve()
    catalog = art / "catalog.json"
    data = json.loads(catalog.read_text(encoding="utf-8-sig"))
    version = data.get("version")
    if version == 2:
        audit_v2(art, catalog, data, args.refresh)
    elif version == 3:
        audit_v3(art, catalog, data, args.refresh, args.deep)
    else:
        raise AssertionError(f"Unsupported packaged-art catalog version: {version}")


def audit_v2(art: Path, catalog: Path, data: dict, refresh: bool) -> None:
    assert data.get("bundles") and data.get("files"), "Invalid native art catalog"
    paths: set[str] = set()
    resource_paths: set[str] = set()
    for record in data["files"]:
        name = safe_relative(record["path"])
        assert name not in paths, f"Duplicate file: {name}"
        paths.add(name)
        path = art / name
        assert path.is_file() and Path(str(path) + ".meta").is_file(), f"Missing file/metadata: {name}"
        payload = path.read_bytes()
        sha = hashlib.sha256(payload).hexdigest()
        if refresh:
            record.update(size=len(payload), sha256=sha)
        else:
            assert record["size"] == len(payload) and record["sha256"] == sha, \
                f"Changed native content (refresh deliberate edits): {name}"
        resource_paths.add("SF2Content/Art/" + str(Path(name).with_suffix("")).replace("\\", "/"))
    for group in data["bundles"]:
        addresses: set[str] = set()
        for asset in group["assets"]:
            assert asset["address"] not in addresses, f"Duplicate address: {asset['address']}"
            addresses.add(asset["address"])
            for kind in ("texture", "sprites", "audio", "font"):
                resource = asset.get(kind)
                if resource:
                    assert resource in resource_paths, f"Uncatalogued native resource: {resource}"
    actual = {p.relative_to(art).as_posix() for p in art.rglob("*")
              if p.is_file() and p.suffix != ".meta" and p != catalog}
    assert actual == paths, f"Unexpected/missing art files: {actual ^ paths}"
    if refresh:
        write_catalog(catalog, data)
    print(f"PASS v2: {len(data['bundles'])} groups, "
          f"{sum(len(g['assets']) for g in data['bundles'])} addresses, "
          f"{len(paths)} files, {sum(f['size'] for f in data['files'])} bytes.")


def audit_v3(art: Path, catalog: Path, data: dict, refresh: bool, deep: bool) -> None:
    groups = data.get("bundles") or []
    assert groups, "Invalid TAR/LZ4 runtime-content catalog"
    assert not data.get("files"), "v3 catalog must not enumerate Unity-imported payload files"

    group_names: set[str] = set()
    archives: set[str] = set()
    expected_archives: set[str] = set()
    address_count = 0
    font_refs = 0
    compressed_bytes = 0

    if deep:
        ensure_packer()

    for group in groups:
        group_name = group.get("name") or ""
        assert safe_token(group_name), f"Unsafe group name: {group_name!r}"
        folded_group = group_name.casefold()
        assert folded_group not in group_names, f"Duplicate group: {group_name}"
        group_names.add(folded_group)

        namespace = group.get("namespaceId") or "core"
        assert safe_token(namespace), f"Unsafe namespace: {namespace!r}"
        bundle_file = group.get("file") or ""
        if bundle_file:
            bundle_file = safe_relative(bundle_file)
            assert bundle_file.lower().endswith(".tar.lz4"), f"Not a TAR/LZ4 archive: {bundle_file}"
            folded_archive = bundle_file.casefold()
            assert folded_archive not in archives, f"Duplicate archive: {bundle_file}"
            archives.add(folded_archive)
            expected_archives.add(bundle_file)

            path = STREAMING / bundle_file
            assert path.is_file(), f"Missing TAR/LZ4 archive: {path}"
            size = path.stat().st_size
            sha = sha256(path)
            if refresh:
                info = packer_info(path)
                group["size"] = size
                group["sha256"] = sha
                group["unpackedSize"] = info["unpackedSize"]
            else:
                assert group.get("size") == size, f"TAR/LZ4 size mismatch: {bundle_file}"
                assert group.get("sha256") == sha, f"TAR/LZ4 hash mismatch: {bundle_file}"
                assert isinstance(group.get("unpackedSize"), int) and group["unpackedSize"] > 0, \
                    f"Missing decoded TAR size: {bundle_file}"
            compressed_bytes += size
            if deep:
                run_packer("verify", path)

        addresses: set[str] = set()
        for asset in group.get("assets") or []:
            address = asset.get("address") or ""
            assert address, f"Empty address in {group_name}"
            folded = address.casefold()
            assert folded not in addresses, f"Duplicate address in {group_name}: {address}"
            addresses.add(folded)
            address_count += 1

            assert not asset.get("texture") and not asset.get("sprites") and not asset.get("audio"), \
                f"v3 payload reference leaked into catalog: {group_name}/{address}"
            font = asset.get("font") or ""
            if font:
                font = safe_relative(font)
                assert font.startswith("SF2Content/Fonts/"), f"Font outside canonical root: {font}"
                font_path = RESOURCES / (font + ".ttf")
                assert font_path.is_file(), f"Missing loose font: {font_path}"
                assert Path(str(font_path) + ".meta").is_file(), f"Missing loose font .meta: {font_path}"
                font_refs += 1
            elif not bundle_file:
                raise AssertionError(f"Group has neither archive nor loose font: {group_name}/{address}")

    actual_archives = {p.relative_to(STREAMING).as_posix() for p in STREAMING.rglob("*")
                       if p.is_file() and p.suffix != ".meta"}
    assert {x.casefold() for x in actual_archives} == {x.casefold() for x in expected_archives}, \
        f"Unexpected/missing TAR/LZ4 archives: {actual_archives ^ expected_archives}"

    legacy_payloads = {p.relative_to(art).as_posix() for p in art.rglob("*")
                       if p.is_file() and p != catalog and p.suffix != ".meta"}
    assert not legacy_payloads, f"Legacy Unity-imported art remains: {sorted(legacy_payloads)[:10]}"

    if refresh:
        write_catalog(catalog, data)
    mode = " deep" if deep else ""
    print(f"PASS v3{mode}: {len(groups)} groups, {len(expected_archives)} TAR/LZ4 archives, "
          f"{address_count} addresses, {font_refs} loose fonts, {compressed_bytes} compressed bytes.")


def packer_info(path: Path) -> dict[str, int | str]:
    ensure_packer()
    completed = subprocess.run(["dotnet", str(PACKER), "info", str(path)], cwd=ROOT,
                               check=True, capture_output=True, text=True)
    values: dict[str, int | str] = {}
    for line in completed.stdout.splitlines():
        key, sep, value = line.partition("=")
        if not sep:
            continue
        values[key] = int(value) if key in {"size", "unpackedSize"} else value
    assert values.get("size") == path.stat().st_size, f"AssetPacker info size mismatch: {path}"
    assert isinstance(values.get("unpackedSize"), int) and values["unpackedSize"] > 0, \
        f"AssetPacker did not report decoded TAR size: {path}"
    assert values.get("sha256") == sha256(path), f"AssetPacker info hash mismatch: {path}"
    return values


def ensure_packer() -> None:
    if PACKER.is_file():
        return
    subprocess.run(["dotnet", "build", str(ROOT / "Tools/AssetPacker/AssetPacker.csproj"),
                    "-c", "Release", "--nologo"], cwd=ROOT, check=True)
    assert PACKER.is_file(), f"AssetPacker build did not produce {PACKER}"


def run_packer(command: str, path: Path) -> None:
    subprocess.run(["dotnet", str(PACKER), command, str(path)], cwd=ROOT, check=True,
                   stdout=subprocess.DEVNULL)


def write_catalog(path: Path, data: dict) -> None:
    path.write_text(json.dumps(data, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")


def safe_relative(value: str) -> str:
    value = str(value).replace("\\", "/").strip().lstrip("/")
    parts = value.split("/")
    assert value and ":" not in value and all(part not in {"", ".", ".."} for part in parts), \
        f"Unsafe relative path: {value!r}"
    return value


def safe_token(value: str) -> bool:
    return bool(value) and not any(ch in value for ch in "/\\:") and value not in {".", ".."}


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as source:
        for chunk in iter(lambda: source.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


if __name__ == "__main__":
    main()
