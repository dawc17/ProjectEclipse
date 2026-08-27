#!/usr/bin/env python3
"""Apply reviewed SF2 Android-to-Switch symbol mappings to this Unity export.

The bulk mapper in RE/decompilation intentionally accepts only generic,
high-confidence candidates.  Additional subsystem TSVs were reviewed against
Android method bodies and Switch native RVAs; this script consumes those maps
without weakening their evidence boundary.

Run with --dry-run to report the changes without writing them.
"""

from __future__ import annotations

import argparse
import csv
import re
import shutil
from collections import defaultdict
from pathlib import Path


PROJECT = Path(__file__).resolve().parents[1]
SCRIPTS = PROJECT / "Assets" / "Scripts"
FIRSTPASS = PROJECT / "Assets" / "Plugins" / "Assembly-CSharp-firstpass"
SOURCE_ROOTS = (SCRIPTS, FIRSTPASS)
RE_ROOT = Path("/home/czapla/RE")
MAP_ROOT = RE_ROOT / "decompilation" / "generated"
SYSTEM_MAPS = MAP_ROOT / "system_maps"
CROSS_MAP = MAP_ROOT / "cross_build_map"
INITIAL_MAP = MAP_ROOT / ".cross_build_map.pwIZKx"

OBFUSCATED = re.compile(r"^[A-P]{10,16}$")
IDENTIFIER = re.compile(r"^[A-Za-z_][A-Za-z0-9_]*$")


def simple_type(full_name: str) -> str:
    """Return the source-level leaf name used by the namespace-free export."""
    leaf = full_name.split("+")[-1].split("/")[-1].split(".")[-1]
    return leaf.split("`")[0]


def method_leaf(value: str) -> str | None:
    """Extract a single method identifier from a report's display signature."""
    value = value.strip()
    if not value or " / " in value:
        return None
    value = value.split("(", 1)[0].strip().split(".")[-1]
    if value in {".ctor", ".cctor"} or not IDENTIFIER.fullmatch(value):
        return None
    return value


def add_candidate(
    candidates: dict[str, set[str]], old: str | None, new: str | None
) -> None:
    if not old or not new:
        return
    old = old.strip()
    new = new.strip()
    if OBFUSCATED.fullmatch(old) and IDENTIFIER.fullmatch(new):
        candidates[old].add(new)


def read_rows(path: Path) -> list[dict[str, str]]:
    with path.open(newline="", encoding="utf-8") as handle:
        return list(csv.DictReader(handle, delimiter="\t"))


def load_firstpass_types() -> tuple[dict[str, str], list[str]]:
    """Load strong, top-level firstpass matches from the unpoisoned map.

    Android obfuscation moved these types into the global namespace.  A clean
    leaf name therefore cannot be used when two obfuscated types map to the
    same leaf (for example two different SevenZip ``Encoder`` classes).
    Those cases are retained for later namespace restoration instead.
    """
    obfuscated_files = {
        path.stem
        for path in FIRSTPASS.glob("*.cs")
        if OBFUSCATED.fullmatch(path.stem)
    }
    proposed: dict[str, str] = {}
    targets: dict[str, list[str]] = defaultdict(list)
    sources = (
        (INITIAL_MAP / "type_candidates.tsv", {"HIGH", "MEDIUM"}),
        # The regenerated map contains manually accepted firstpass anchors.
        # Require its exact rank-1/score-1 anchor boundary; weaker regenerated
        # candidates are intentionally not consumed here.
        (CROSS_MAP / "type_candidates.tsv", {"ANCHOR"}),
    )
    for path, confidences in sources:
        for row in read_rows(path):
            old = row.get("android_type", "")
            if (
                row.get("assembly") != "Assembly-CSharp-firstpass"
                or old not in obfuscated_files
                or row.get("rank") != "1"
                or row.get("confidence") not in confidences
                or (
                    row.get("confidence") == "ANCHOR"
                    and (
                        row.get("score") != "1.000000"
                        or row.get("accepted_seed_anchor") != "true"
                    )
                )
            ):
                continue
            new = simple_type(row.get("switch_type", ""))
            if not IDENTIFIER.fullmatch(new):
                continue
            previous = proposed.get(old)
            if previous is not None and previous != new:
                skipped_name = f"{old}: candidate sources disagree ({previous}, {new})"
                proposed.pop(old, None)
                targets[previous].remove(old)
                continue
            proposed[old] = new
            if old not in targets[new]:
                targets[new].append(old)

    skipped: list[str] = []
    accepted: dict[str, str] = {}
    for old, new in sorted(proposed.items()):
        if len(targets[new]) > 1:
            skipped.append(
                f"{old} -> {new}: clean leaf is shared by "
                + ", ".join(sorted(targets[new]))
            )
            continue
        destination = FIRSTPASS / f"{new}.cs"
        if destination.exists():
            skipped.append(f"{old} -> {new}: global destination already exists")
            continue
        accepted[old] = new
    return accepted, skipped


def load_reviewed_candidates() -> tuple[dict[str, str], dict[str, set[str]], list[str]]:
    candidates: dict[str, set[str]] = defaultdict(set)

    # The generic authoritative set is useful for idempotency and picks up any
    # map entries generated after the first rename pass.
    for row in read_rows(CROSS_MAP / "authoritative_members.tsv"):
        target = row.get("switch_name", "")
        backing = re.fullmatch(r"<(.+)>k__BackingField", target)
        if backing:
            target = "_" + backing.group(1)
        add_candidate(candidates, row.get("android_name"), target)

    derived_members = PROJECT / "Deobfuscation" / "derived_authoritative_members.tsv"
    if derived_members.exists():
        for row in read_rows(derived_members):
            target = row.get("switch_name", "")
            backing = re.fullmatch(r"<(.+)>k__BackingField", target)
            if backing:
                target = "_" + backing.group(1)
            add_candidate(candidates, row.get("android_name"), target)

    # Manually reviewed subsystem reports use several schemas.  Normalize all
    # of them to old-token -> clean-name while retaining conflicts for review.
    for path in sorted(SYSTEM_MAPS.glob("*.tsv")):
        for row in read_rows(path):
            old_type = row.get("android_type") or row.get("android_owner")
            new_type = (
                row.get("switch_type")
                or row.get("switch_owner")
                or row.get("clean_type")
            )
            if old_type and new_type:
                add_candidate(candidates, old_type.split("+")[-1].split(".")[-1], simple_type(new_type))

            old_member = (
                row.get("android_member")
                or row.get("android_method")
            )
            new_member = (
                row.get("switch_member")
                or row.get("switch_method")
                or row.get("clean_method")
            )
            add_candidate(candidates, method_leaf(old_member or ""), method_leaf(new_member or ""))

    manual_types = PROJECT / "Deobfuscation" / "manual_confirmed_types.tsv"
    for row in read_rows(manual_types):
        add_candidate(candidates, row.get("android_type"), row.get("clean_type"))

    firstpass_types, namespace_collisions = load_firstpass_types()
    for old, new in firstpass_types.items():
        add_candidate(candidates, old, new)

    resolved = {
        old: next(iter(targets))
        for old, targets in candidates.items()
        if len(targets) == 1
    }
    conflicts = {old: targets for old, targets in candidates.items() if len(targets) > 1}
    return resolved, conflicts, namespace_collisions


def load_manual_corrections() -> dict[str, str]:
    path = PROJECT / "Deobfuscation" / "manual_corrections.tsv"
    corrections: dict[str, str] = {}
    for row in read_rows(path):
        old = row.get("current_name", "").strip()
        new = row.get("clean_name", "").strip()
        if IDENTIFIER.fullmatch(old) and IDENTIFIER.fullmatch(new):
            corrections[old] = new
    return corrections


def replace_sources(symbols: dict[str, str], dry_run: bool) -> tuple[int, int]:
    if not symbols:
        return 0, 0
    pattern = re.compile(
        r"\b(" + "|".join(sorted(map(re.escape, symbols), key=len, reverse=True)) + r")\b"
    )
    changed_files = replacements = 0
    for root in SOURCE_ROOTS:
        for path in sorted(root.rglob("*.cs")):
            original = path.read_text(encoding="utf-8", errors="surrogateescape")
            updated, count = pattern.subn(lambda match: symbols[match.group(0)], original)
            if not count:
                continue
            changed_files += 1
            replacements += count
            if not dry_run:
                path.write_text(updated, encoding="utf-8", errors="surrogateescape")
    return changed_files, replacements


def rename_script_files(symbols: dict[str, str], dry_run: bool) -> tuple[int, list[str]]:
    renamed = 0
    skipped: list[str] = []
    by_stem: dict[str, list[Path]] = defaultdict(list)
    for root in SOURCE_ROOTS:
        for path in root.rglob("*.cs"):
            by_stem[path.stem].append(path)
    for old, new in sorted(symbols.items()):
        sources = by_stem.get(old, [])
        if not sources:
            continue
        if len(sources) > 1:
            skipped.append(f"{old} -> {new}: source filename is ambiguous")
            continue
        source = sources[0]
        destination = source.with_name(f"{new}.cs")
        if destination.exists():
            skipped.append(f"{old} -> {new}: destination already exists")
            continue
        renamed += 1
        if dry_run:
            continue
        shutil.move(source, destination)
        source_meta = source.with_suffix(".cs.meta")
        if source_meta.exists():
            shutil.move(source_meta, destination.with_suffix(".cs.meta"))
    return renamed, skipped


def write_report(
    symbols: dict[str, str], conflicts: dict[str, set[str]], skipped: list[str]
) -> None:
    report = PROJECT / "Deobfuscation" / "reviewed_symbol_application.tsv"
    with report.open("w", encoding="utf-8", newline="") as handle:
        writer = csv.writer(handle, delimiter="\t", lineterminator="\n")
        writer.writerow(("old", "new", "status"))
        for old, new in sorted(symbols.items()):
            writer.writerow((old, new, "applied_or_already_applied"))
        for old, targets in sorted(conflicts.items()):
            writer.writerow((old, "|".join(sorted(targets)), "skipped_conflict"))
        for message in skipped:
            old, rest = message.split(" -> ", 1)
            writer.writerow((old, rest, "skipped_file_collision"))


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--dry-run", action="store_true")
    args = parser.parse_args()

    symbols, conflicts, namespace_collisions = load_reviewed_candidates()
    symbols.update(load_manual_corrections())
    changed_files, replacement_count = replace_sources(symbols, args.dry_run)
    renamed_files, skipped = rename_script_files(symbols, args.dry_run)

    print(f"resolved reviewed symbols : {len(symbols)}")
    print(f"ambiguous symbols skipped : {len(conflicts)}")
    print(f"source files changed       : {changed_files}")
    print(f"identifier replacements   : {replacement_count}")
    print(f"script files renamed       : {renamed_files}")
    print(f"file collisions skipped    : {len(skipped)}")
    print(f"namespace collisions held  : {len(namespace_collisions)}")
    if conflicts:
        print("\nConflicting reviewed targets:")
        for old, targets in sorted(conflicts.items()):
            print(f"  {old}: {', '.join(sorted(targets))}")
    if skipped:
        print("\nScript filename collisions:")
        for message in skipped:
            print(f"  {message}")
    if namespace_collisions:
        print("\nFirstpass namespace collisions held for review:")
        for message in namespace_collisions:
            print(f"  {message}")

    if not args.dry_run:
        write_report(symbols, conflicts, skipped + namespace_collisions)


if __name__ == "__main__":
    main()
