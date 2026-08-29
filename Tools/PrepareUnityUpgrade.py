"""Create a verified Unity upgrade checkpoint and an isolated working copy.

Does not open Unity, install packages, or change the source project. Run with
Python 3.12+ from this project. Backups contain private saves; keep them local.
"""
from __future__ import annotations

import argparse
import hashlib
import json
import os
from pathlib import Path
import re
import shutil
import subprocess
from datetime import datetime, timezone
import zipfile

ROOT = Path(__file__).resolve().parents[1]
PROJECT_DIRS = ("Assets", "Packages", "ProjectSettings", "Tools", "BuildScripts", "Deobfuscation")
EXCLUDED = {"library", "temp", "obj", "logs", "build", "builds", ".vs", "__pycache__", "python-packages", "out"}
IMPORTERS = {
    "RecoveredUserSpriteImporter.cs", "RecoveredSkillIconImporter.cs",
    "RecoveredLocationAtlasImporter.cs", "RecoveredItemIconImporter.cs",
    "RecoveredEffectFrameImporter.cs",
}


def digest(data: bytes) -> str:
    return hashlib.sha256(data).hexdigest()


def project_files(root: Path) -> list[Path]:
    files = []
    for name in PROJECT_DIRS:
        folder = root / name
        if not folder.exists():
            continue
        for current, dirs, names in os.walk(folder, followlinks=False):
            # Never follow shared directories into the original project or a
            # different checkout. Both the zip and clone contain real copies.
            for child in dirs + names:
                p = Path(current) / child
                if p.is_symlink() or (hasattr(p, "is_junction") and p.is_junction()):
                    raise RuntimeError(f"Linked path needs manual review: {p}")
            dirs[:] = sorted(d for d in dirs if d.lower() not in EXCLUDED)
            files.extend(Path(current) / n for n in sorted(names)
                         if not n.endswith((".log", ".pyc", ".pdb", ".mdb")))
    files.extend(p for p in root.iterdir() if p.is_file() and
                 (p.suffix.lower() in (".csproj", ".sln", ".md") or p.name in (".gitignore", ".vsconfig")))
    return sorted(files)


def clone_path(relative: Path) -> Path:
    if relative.parent.as_posix() == "Assets/Editor" and relative.name.removesuffix(".meta") in IMPORTERS:
        return Path("UpgradeQuarantine/Editor") / relative.name
    return relative


def prepare(root: Path, stamp: str, save_root: Path | None = None) -> dict:
    root = root.resolve()
    if not re.fullmatch(r"[A-Za-z0-9_-]+", stamp):
        raise ValueError("Checkpoint name must contain only letters, digits, '_' or '-'")
    backup = root / "UpgradeBackups" / stamp
    clone = root / "UpgradeWorkspaces" / stamp
    for target in (backup, clone):
        target.resolve().relative_to(root)
        if target.exists():
            raise FileExistsError(f"Refusing to overwrite: {target}")
    version = (root / "ProjectSettings/ProjectVersion.txt").read_text()
    if "m_EditorVersion: 2019.4.41f2" not in version:
        raise RuntimeError("Expected the 2019.4.41f2 baseline; review before snapshotting a different engine")
    sources = project_files(root)
    size = sum(p.stat().st_size for p in sources)
    if shutil.disk_usage(root).free < size * 3 + 1024**3:
        raise RuntimeError("Insufficient free space for a clone, verified zip and import headroom")
    backup.mkdir(parents=True)
    clone.mkdir(parents=True)
    report = {"createdUtc": datetime.now(timezone.utc).isoformat(), "source": str(root),
              "backup": str(backup), "workspace": str(clone), "engine": "2019.4.41f2",
              "targetEngine": "2022.3 LTS (not installed or opened by this tool)",
              "files": [], "saveFiles": [], "bundles": [], "verified": False}
    archive = backup / "project-2019.zip"
    with zipfile.ZipFile(archive, "x", compression=zipfile.ZIP_DEFLATED, compresslevel=1) as z:
        for i, source in enumerate(sources):
            relative = source.relative_to(root)
            data = source.read_bytes()
            z.writestr(relative.as_posix(), data)
            destination = clone / clone_path(relative)
            destination.parent.mkdir(parents=True, exist_ok=True)
            destination.write_bytes(data)
            report["files"].append({"path": relative.as_posix(), "sha256": digest(data),
                                    "clonePath": clone_path(relative).as_posix()})
            if i and i % 5000 == 0:
                print(f"Copied and archived {i}/{len(sources)} files", flush=True)
    # Changing only the clone's product name isolates persistentDataPath and
    # PlayerPrefs. Never seed the original save location or launch a player here.
    settings = clone / "ProjectSettings/ProjectSettings.asset"
    text = settings.read_text(encoding="utf-8")
    match = re.search(r"(?m)^  productName: (.+)$", text)
    if not match:
        raise RuntimeError("Cannot identify save identity; clone must not be opened")
    original_product = match.group(1).strip()
    product = original_product + "_upgrade2022_" + stamp
    settings.write_text(text[:match.start(1)] + product + text[match.end(1):], encoding="utf-8")
    report["isolatedProductName"] = product
    report["quarantinedImporters"] = sorted(IMPORTERS)
    # All existing .meta files are copied byte for byte, including the ones
    # belonging to quarantined recovery tools. No asset GUIDs are regenerated.
    if save_root and save_root.is_dir():
        with zipfile.ZipFile(backup / "userdata-private.zip", "x", zipfile.ZIP_DEFLATED) as z:
            for source in sorted(save_root.rglob("*")):
                if source.is_file():
                    relative = source.relative_to(save_root).as_posix()
                    data = source.read_bytes()
                    z.writestr(relative, data)
                    report["saveFiles"].append({"path": relative, "sha256": digest(data)})
        with zipfile.ZipFile(backup / "userdata-private.zip") as z:
            for record in report["saveFiles"]:
                if digest(z.read(record["path"])) != record["sha256"]:
                    raise RuntimeError("Save backup verification failed")
    # Source bundles stay in ResearchSources; do not duplicate or load them.
    bundles = root / "ResearchSources/bundles"
    if bundles.is_dir():
        for source in sorted(bundles.iterdir()):
            if source.is_file():
                report["bundles"].append({"path": source.relative_to(root).as_posix(),
                                           "size": source.stat().st_size,
                                           "sha256": digest(source.read_bytes())})
    if (root / ".git").exists():
        for args, target in ((["status", "--porcelain=v1", "--untracked-files=all"], "git-status.txt"),
                             (["diff", "HEAD", "--binary"], "uncommitted.patch"),
                             (["rev-parse", "HEAD"], "git-head.txt")):
            result = subprocess.run(["git", "-C", str(root), *args], capture_output=True, check=True)
            (backup / target).write_bytes(result.stdout)
    print("Verifying archive, copy and unchanged source hashes...", flush=True)
    with zipfile.ZipFile(archive) as z:
        for record in report["files"]:
            path = record["path"]
            expected = record["sha256"]
            if digest(z.read(path)) != expected or digest((root / path).read_bytes()) != expected:
                raise RuntimeError(f"Archive mismatch or source changed during preparation: {path}")
            copied = (clone / record["clonePath"]).read_bytes()
            if path != "ProjectSettings/ProjectSettings.asset" and digest(copied) != expected:
                raise RuntimeError(f"Clone verification failed: {path}")
            record["cloneSha256"] = digest(copied)
    if project_files(root) != sources:
        raise RuntimeError("Source file set changed during preparation; take a fresh checkpoint")
    report["archiveSha256"] = digest(archive.read_bytes())
    report["verified"] = True
    (backup / "manifest.json").write_text(json.dumps(report, indent=2), encoding="utf-8")
    (clone / "UPGRADE_WORKSPACE.md").write_text(
        "# Isolated engine-upgrade workspace\n\n"
        f"Source: {root}\n\nVerified checkpoint: {backup}\n\n"
        f"Isolated product/save identity: {product}\n\n"
        "Not yet upgraded. Open ONLY this workspace with the selected Unity 2022.3 editor. "
        "Keep Built-in rendering, Gamma color and legacy Input Manager. Five recovery "
        "importers and their .meta files are in UpgradeQuarantine/Editor, outside Assets. "
        "Restore them only after reviewing their behavior in the new editor.\n\n"
        "Do not use the copied csproj files to claim a Unity 2022 compile: they still "
        "reference Unity 2019. Regenerate them in the new editor.\n\n"
        "See Tools/UNITY_2022_UPGRADE.md in the source project for the migration gates. "
        "Research bundles and existing PlayerPrefs were NOT copied. The private userdata "
        "backup was NOT restored to any live save location.\n", encoding="utf-8")
    return report


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--name", default="unity2022-" + datetime.now().strftime("%Y%m%d-%H%M%S"))
    parser.add_argument("--backup-userdata", type=Path, help="Optional explicit userdata folder to read and archive locally")
    args = parser.parse_args()
    report = prepare(ROOT, args.name, args.backup_userdata)
    print(f"PASS: {len(report['files'])} project files verified; {len(report['saveFiles'])} save files archived")
    print(f"Backup: {report['backup']}\nWorkspace: {report['workspace']}")


if __name__ == "__main__":
    main()
