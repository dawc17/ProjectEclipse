#!/usr/bin/env python3
"""Pack the archived Titan reward geometry; harpoon differs in two edge flags."""

from __future__ import annotations

import argparse
import gzip
import hashlib
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "ResearchSources/de128_assets/gamedata/models"
TARGET = ROOT / "Mods/de128/assets/models/titan"
MODELS = ("mdl_body_titan", "mdl_head_titan", "mdl_ranged_titans_harpoon", "mdl_magic_fireball")


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--check", action="store_true")
    args = parser.parse_args()
    if not args.check:
        TARGET.mkdir(parents=True, exist_ok=True)
    for name in MODELS:
        source = (SOURCE / (name + ".xml")).read_bytes()
        packed = gzip.compress(source, mtime=0)
        target = TARGET / (name + ".modelz")
        if not args.check:
            target.write_bytes(packed)
        if not target.exists() or target.read_bytes() != packed:
            raise ValueError(f"Archived Titan reward geometry differs: {target}")
        print(f"Verified {name} {hashlib.sha256(source).hexdigest()} ({len(source)} bytes)")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
