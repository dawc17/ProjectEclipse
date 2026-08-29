#!/usr/bin/env python3
"""Promote a verified isolated Unity-upgrade workspace into this checkout."""

from __future__ import annotations

import argparse
import hashlib
import json
import os
import re
import shutil
from datetime import datetime, timezone
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
PROMOTED_ROOTS = ("Assets", "Packages", "ProjectSettings", "UpgradeQuarantine")


def digest_bytes(data: bytes) -> str:
    return hashlib.sha256(data).hexdigest()


def digest_file(path: Path) -> str:
    value = hashlib.sha256()
    with path.open("rb") as stream:
        for block in iter(lambda: stream.read(1024 * 1024), b""):
            value.update(block)
    return value.hexdigest()


def atomic_copy(source: Path, destination: Path, data: bytes | None = None) -> None:
    destination.parent.mkdir(parents=True, exist_ok=True)
    temporary = destination.with_name(destination.name + ".upgrade-transition.tmp")
    if data is None:
        shutil.copy2(source, temporary)
    else:
        temporary.write_bytes(data)
    os.replace(temporary, destination)


def project_product(data: bytes) -> str:
    match = re.search(rb"(?m)^  productName: (.+)$", data)
    if match is None:
        raise RuntimeError("Could not read the current Unity product name")
    return match.group(1).decode("utf-8")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--name", required=True, help="Upgrade checkpoint/workspace name")
    parser.add_argument("--apply", action="store_true", help="Apply after a safe preview")
    args = parser.parse_args()

    backup = (ROOT / "UpgradeBackups" / args.name).resolve()
    manifest_path = backup / "manifest.json"
    manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
    workspace = Path(manifest["workspace"]).resolve()
    if not manifest.get("verified"):
        raise RuntimeError("Checkpoint manifest is not verified")
    if Path(manifest["source"]).resolve() != ROOT or not workspace.is_relative_to(ROOT / "UpgradeWorkspaces"):
        raise RuntimeError("Manifest source/workspace does not match this checkout")
    version = (workspace / "ProjectSettings/ProjectVersion.txt").read_text(encoding="utf-8")
    if "2022.3." not in version:
        raise RuntimeError("Workspace is not a Unity 2022.3 project")

    settings_relative = "ProjectSettings/ProjectSettings.asset"
    original_product = project_product((ROOT / settings_relative).read_bytes())
    isolated_product = manifest["isolatedProductName"]
    records = {record["clonePath"].casefold(): record for record in manifest["files"]}
    copies: list[tuple[Path, Path, bytes | None, str]] = []
    removals: list[Path] = []
    conflicts: list[str] = []

    for record in manifest["files"]:
        source_relative = record["path"]
        clone_relative = record["clonePath"]
        clone = workspace / clone_relative
        if not clone.is_file():
            conflicts.append(f"workspace file missing: {clone_relative}")
            continue
        clone_data: bytes | None = None
        clone_hash = digest_file(clone)
        desired_relative = clone_relative if clone_relative != source_relative else source_relative
        if source_relative == settings_relative:
            clone_data = clone.read_bytes()
            isolated = f"  productName: {isolated_product}".encode()
            restored = f"  productName: {original_product}".encode()
            if isolated not in clone_data:
                conflicts.append("isolated Unity product name is missing from upgraded ProjectSettings")
                continue
            clone_data = clone_data.replace(isolated, restored, 1)
        desired_hash = digest_bytes(clone_data) if clone_data is not None else clone_hash

        is_quarantined = clone_relative != source_relative
        changed_in_workspace = clone_hash != record["cloneSha256"]
        if not changed_in_workspace and not is_quarantined and source_relative != settings_relative:
            continue

        destination = ROOT / desired_relative
        if destination.is_file() and digest_file(destination) == desired_hash:
            pass
        else:
            original = ROOT / source_relative
            current_hash = digest_file(original) if original.is_file() else None
            if not is_quarantined and current_hash not in (record["sha256"], desired_hash):
                conflicts.append(f"independently changed in main checkout: {source_relative}")
                continue
            if is_quarantined and destination.exists():
                conflicts.append(f"quarantine destination differs: {desired_relative}")
                continue
            copies.append((clone, destination, clone_data, desired_hash))
        if is_quarantined:
            original = ROOT / source_relative
            if original.is_file():
                if digest_file(original) != record["sha256"]:
                    conflicts.append(f"quarantined importer changed in main checkout: {source_relative}")
                else:
                    removals.append(original)

    added: list[str] = []
    for top in PROMOTED_ROOTS:
        directory = workspace / top
        if not directory.is_dir():
            continue
        for clone in directory.rglob("*"):
            if not clone.is_file():
                continue
            relative = clone.relative_to(workspace).as_posix()
            if relative.casefold() in records:
                continue
            destination = ROOT / relative
            desired_hash = digest_file(clone)
            if destination.is_file():
                if digest_file(destination) != desired_hash:
                    conflicts.append(f"added workspace file conflicts with main checkout: {relative}")
            else:
                copies.append((clone, destination, None, desired_hash))
                added.append(relative)

    summary = {
        "checkpoint": args.name,
        "targetVersion": version.strip(),
        "restoredProductName": original_product,
        "copies": len(copies),
        "quarantinedSourceRemovals": len(removals),
        "added": added,
        "conflicts": conflicts,
        "applied": bool(args.apply and not conflicts),
    }
    if conflicts:
        print(json.dumps(summary, indent=2))
        return 1
    if not args.apply:
        print(json.dumps(summary, indent=2))
        print("SAFE PREVIEW: rerun with --apply to promote the workspace")
        return 0

    for source, destination, data, expected_hash in copies:
        atomic_copy(source, destination, data)
        if digest_file(destination) != expected_hash:
            raise RuntimeError(f"Promotion verification failed: {destination}")
    for source in removals:
        source.unlink()
    for source in removals:
        if source.exists():
            raise RuntimeError(f"Quarantined importer remained active: {source}")

    summary["completedUtc"] = datetime.now(timezone.utc).isoformat()
    report = backup / "promotion-2022.json"
    report.write_text(json.dumps(summary, indent=2), encoding="utf-8")
    print(json.dumps(summary, indent=2))
    print(f"PASS: Unity 2022 workspace promoted; report: {report}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
