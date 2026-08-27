#!/usr/bin/env python3
"""Build small, reviewable compatibility fragments from recovered plaintext data."""

from __future__ import annotations

import copy
import xml.etree.ElementTree as ET
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
RECOVERED = (
    ROOT
    / "ResearchSources"
    / "reversingsf2"
    / "com.onerdna.sfml"
    / "files"
    / "mod"
    / "gamedata"
)
COMPAT = ROOT / "Assets" / "xml" / "compat"


def build_stage_compat() -> None:
    source = ET.parse(RECOVERED / "stages.xml").getroot()
    titan = source.find("./Zones/Zone[@Name='ZONE_7']/Battle[@Name='BOSS_TITAN']")
    if titan is None:
        raise RuntimeError("Recovered 2.41.9 stages.xml has no ZONE_7/BOSS_TITAN")

    root = ET.Element("Stages")
    zones = ET.SubElement(root, "Zones")
    zone = ET.SubElement(zones, "Zone", {"Name": "ZONE_7"})
    zone.append(copy.deepcopy(titan))
    ET.indent(root, space="  ")
    COMPAT.mkdir(parents=True, exist_ok=True)
    ET.ElementTree(root).write(
        COMPAT / "stages.xml", encoding="utf-8", xml_declaration=True
    )


if __name__ == "__main__":
    build_stage_compat()
    print("Built Assets/xml/compat/stages.xml with recovered ZONE_7/BOSS_TITAN")
