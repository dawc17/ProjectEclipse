#!/usr/bin/env python3
"""Report evidence for still-obfuscated top-level Assembly-CSharp scripts."""

from __future__ import annotations

import csv
import re
from collections import defaultdict
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
SCRIPTS = ROOT / "Assets/Scripts/Assembly-CSharp"
CANDIDATES = Path(
    "/home/czapla/RE/decompilation/generated/.cross_build_map.pwIZKx/type_candidates.tsv"
)
OBF = re.compile(r"^[A-P]{10,16}$")
DECL = re.compile(
    r"\b(?:class|struct|enum|interface)\s+([A-Za-z_][A-Za-z0-9_]*)"
    r"(?:\s*:\s*([^\n\{]+))?"
)
STRING = re.compile(r'"([^"\n]{3,80})"')


ranked: dict[str, list[dict[str, str]]] = defaultdict(list)
with CANDIDATES.open(newline="", encoding="utf-8") as handle:
    for row in csv.DictReader(handle, delimiter="\t"):
        segment = row["android_type"].split("+")[-1].split(".")[-1]
        ranked[segment].append(row)

print("android_type\tdeclaration\tstring_literals\ttop_candidates")
for path in sorted(SCRIPTS.glob("*.cs")):
    if not OBF.fullmatch(path.stem):
        continue
    text = path.read_text(encoding="utf-8", errors="replace")
    declaration = DECL.search(text)
    declaration_text = ""
    if declaration:
        declaration_text = declaration.group(1)
        if declaration.group(2):
            declaration_text += ":" + re.sub(r"\s+", " ", declaration.group(2)).strip()
    literals: list[str] = []
    for value in STRING.findall(text):
        if value not in literals and not value.startswith("http"):
            literals.append(value)
        if len(literals) == 8:
            break
    candidate_text = []
    for row in sorted(ranked.get(path.stem, []), key=lambda item: int(item["rank"]))[:5]:
        leaf = row["switch_type"].split("+")[-1].split(".")[-1]
        candidate_text.append(f"{row['rank']}:{leaf}@{row['score']}/{row['margin_to_second']}")
    print(
        f"{path.stem}\t{declaration_text}\t{' | '.join(literals)}\t"
        f"{'; '.join(candidate_text)}"
    )
