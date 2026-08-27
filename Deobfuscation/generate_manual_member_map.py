#!/usr/bin/env python3
"""Generate member mappings for the manually confirmed Android type pairs.

This does not modify the RE workspace.  It stages the structural map in /tmp,
adds this project's reviewed type pairs, runs the existing member correlator,
and stores only its authoritative/high-confidence rows in Deobfuscation.
"""

from __future__ import annotations

import csv
import json
import shutil
import subprocess
import tempfile
from collections import defaultdict
from pathlib import Path


PROJECT = Path(__file__).resolve().parents[1]
SOURCE_MAP = Path("/home/czapla/RE/decompilation/generated/cross_build_map")
MEMBER_MAPPER = Path("/home/czapla/RE/decompilation/scripts/generate_member_map.py")
SWITCH_SCRIPT = Path(
    "/home/czapla/RE/decompilation/generated/switch_il2cpp/il2cppdumper/script.json"
)

# Resolve the few clean leaf names that legitimately occur more than once.
FULL_TYPE_OVERRIDES = {
    "FADHDBNEDHM": "Nekki.SF2.Core.Utils.Perks.ActionType",
    "IDDHECOKKJN": "ConditionType",
    "ENKGAHJKICL": "DelayedAnimation",
    "JDHKIJEIDCO": "Nekki.SF2.Core.Fights.Model.EventTriggers",
    "FHEOIDHGCBH": "Nekki.SF2.Core.Utils.ModelType",
}


def read_tsv(path: Path) -> list[dict[str, str]]:
    with path.open(encoding="utf-8", newline="") as handle:
        return list(csv.DictReader(handle, delimiter="\t"))


def main() -> None:
    types = json.loads((SOURCE_MAP / "types.json").read_text(encoding="utf-8"))
    by_leaf: dict[str, set[str]] = defaultdict(set)
    for row in types:
        if row["platform"] != "switch" or row["assembly"] != "Assembly-CSharp":
            continue
        full = row["type"]
        leaf = full.split("+")[-1].split(".")[-1].split("`")[0]
        by_leaf[leaf].add(full)

    confirmed = read_tsv(PROJECT / "Deobfuscation" / "manual_confirmed_types.tsv")
    pairs: list[tuple[str, str, str]] = []
    skipped: list[tuple[str, str, str]] = []
    for row in confirmed:
        old, clean = row["android_type"], row["clean_type"]
        override = FULL_TYPE_OVERRIDES.get(old)
        matches = sorted(by_leaf.get(clean, set()))
        if override:
            target = override
        elif len(matches) == 1:
            target = matches[0]
        else:
            skipped.append((old, clean, "missing" if not matches else "ambiguous"))
            continue
        pairs.append((old, target, row.get("evidence", "reviewed type mapping")))

    with tempfile.TemporaryDirectory(prefix="sf2-member-map-") as temp_name:
        staged = Path(temp_name) / "cross_build_map"
        shutil.copytree(SOURCE_MAP, staged)
        manual_path = staged / "manual_core_types.tsv"
        existing = read_tsv(manual_path) if manual_path.exists() else []
        with manual_path.open("w", encoding="utf-8", newline="") as handle:
            columns = (
                "entity_kind",
                "android_symbol",
                "switch_symbol",
                "subsystem",
                "confidence",
                "evidence",
            )
            writer = csv.DictWriter(handle, fieldnames=columns, delimiter="\t", lineterminator="\n")
            writer.writeheader()
            writer.writerows(existing)
            for old, target, evidence in pairs:
                writer.writerow(
                    {
                        "entity_kind": "type",
                        "android_symbol": old,
                        "switch_symbol": target,
                        "subsystem": "reviewed_export",
                        "confidence": "high",
                        "evidence": evidence,
                    }
                )

        subprocess.run(
            ["python3", str(MEMBER_MAPPER), str(staged), str(SWITCH_SCRIPT)],
            check=True,
        )
        authoritative = read_tsv(staged / "authoritative_members.tsv")

    confirmed_old = {old for old, _, _ in pairs}
    derived = [row for row in authoritative if row["android_type"] in confirmed_old]
    output = PROJECT / "Deobfuscation" / "derived_authoritative_members.tsv"
    columns = list(derived[0]) if derived else ["android_name", "switch_name"]
    with output.open("w", encoding="utf-8", newline="") as handle:
        writer = csv.DictWriter(handle, fieldnames=columns, delimiter="\t", lineterminator="\n")
        writer.writeheader()
        writer.writerows(derived)

    print(f"reviewed type pairs staged : {len(pairs)}")
    print(f"unresolved type pairs      : {len(skipped)}")
    print(f"derived member mappings    : {len(derived)}")
    print(f"output                     : {output}")
    if skipped:
        print("\nType pairs without a unique Switch owner:")
        for old, clean, reason in skipped:
            print(f"  {old} -> {clean}: {reason}")


if __name__ == "__main__":
    main()
