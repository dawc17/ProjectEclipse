#!/usr/bin/env python3
"""Suggest clean quest-action type names from the unobfuscated factory enum."""

from __future__ import annotations

import csv
import re
from difflib import SequenceMatcher
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
FACTORY = ROOT / "Assets/Scripts/Assembly-CSharp/QuestAction.cs"
TYPES = Path("/home/czapla/RE/decompilation/generated/cross_build_map/types.tsv")
OBF = re.compile(r"^[A-P]{10,16}$")


def normalized(value: str) -> str:
    value = value.lower().replace("questaction", "")
    aliases = {
        "button": "btn",
        "application": "app",
        "achievements": "achievement",
        "quests": "quest",
        "tutorial": "tut",
    }
    for old, new in aliases.items():
        value = value.replace(old, new)
    return re.sub(r"[^a-z0-9]", "", value)


switch_actions: list[str] = []
with TYPES.open(newline="", encoding="utf-8") as handle:
    for row in csv.DictReader(handle, delimiter="\t"):
        if row["platform"] != "switch" or row["assembly"] != "Assembly-CSharp":
            continue
        leaf = row["type"].split("+")[-1].split(".")[-1]
        if leaf.startswith("QuestAction") and "<" not in leaf:
            switch_actions.append(leaf)

text = FACTORY.read_text(encoding="utf-8")
pattern = re.compile(
    r"case\s+[^.]+\.(QUEST_ACTION_[A-Z0-9_]+):\s*\n\s*return\s+new\s+([A-Za-z_][A-Za-z0-9_]*)\s*\(",
    re.MULTILINE,
)

print("android_type\tenum_case\tbest_switch_type\tscore\tsecond_switch_type\tmargin")
for enum_name, android_type in pattern.findall(text):
    if not OBF.fullmatch(android_type):
        continue
    needle = normalized(enum_name.removeprefix("QUEST_ACTION_"))
    ranked = sorted(
        (
            SequenceMatcher(None, needle, normalized(candidate)).ratio(),
            candidate,
        )
        for candidate in switch_actions
    )
    best_score, best = ranked[-1]
    second_score, second = ranked[-2]
    print(
        f"{android_type}\t{enum_name}\t{best}\t{best_score:.3f}\t"
        f"{second}\t{best_score - second_score:.3f}"
    )
