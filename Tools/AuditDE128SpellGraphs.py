"""Read-only archived spell dependency audit. Never used by the shipped Lua mod."""
import argparse
import copy
import hashlib
import json
from collections import Counter
from pathlib import Path
import xml.etree.ElementTree as ET

from AuditDE128EquipmentMoves import normalize, differences

ROOT = Path(__file__).resolve().parents[1]
FAMILIES = ("Sphere1", "Sphere2", "Sphere3", "ComboSphere3", "MindThrowNormal")


def index(tree, tag):
    result = {}
    for node in tree.iter(tag):
        name = node.get("Name")
        if name in result:
            raise ValueError(f"Duplicate {tag}: {name}")
        result[name] = node
    return result


def audit():
    paths = [ROOT / "Assets" / directory / "animations/moves.xml"
             for directory in ("vanillaXml", "DExml")]
    base, archive = [ET.parse(path) for path in paths]
    base_moves = index(base, "Move")
    base_templates = index(base, "Template")
    templates = index(archive, "Template")
    # Existence is evidence of an available source file, not native decode/render success.
    resources = {}
    for path in (ROOT / "Assets/Resources").rglob("*"):
        if path.is_file() and path.suffix != ".meta":
            resources.setdefault(path.name, []).append(path.relative_to(ROOT).as_posix())

    def closure(node, found, stack=()):
        for name in filter(None, node.get("Template", "").split("|")):
            if name in stack:
                raise ValueError("Template cycle: " + " -> ".join(stack + (name,)))
            if name in found:
                continue
            target = templates.get(name)
            if target is None:
                raise ValueError("Missing archived template: " + name)
            found[name] = target
            closure(target, found, stack + (name,))

    families = {}
    for family in FAMILIES:
        moves = [node for node in archive.iter("Move")
                 if any(item.get("SubType") == family for item in node.iter("Item"))]
        dependencies = {}
        for move in moves:
            closure(move, dependencies)
        template_rows = []
        for name, node in sorted(dependencies.items()):
            source = base_templates.get(name)
            delta = None
            if source is not None:
                left, right = copy.deepcopy(source), copy.deepcopy(node)
                normalize(left)
                normalize(right)
                delta = differences(left, right)
            template_rows.append({"name": name, "exists_in_base": source is not None,
                                  "differences": delta})
        nodes = moves + list(dependencies.values())
        files = sorted({n.get("FileName") for n in nodes if n.get("FileName")})
        sequences = sorted({effect.get("Sequence") for n in nodes
                            for effect in n.findall("./Actions/Effect") if effect.get("Sequence")})
        sounds = sorted({sound.get("Name") for n in nodes
                         for sound in n.findall("./Actions//Sound") if sound.get("Name")})
        families[family] = {
            "moves": [{"name": n.get("Name"), "exists_in_base": n.get("Name") in base_moves}
                      for n in moves],
            "templates": template_rows,
            "actions_including_templates": dict(sorted(Counter(
                action.tag for n in nodes for action in n.findall("./Actions/*")).items())),
            "condition_tags_including_templates": sorted({c.tag for n in nodes
                for section in ("Conditions", "Locks") for parent in n.findall(section)
                for c in parent.iter() if c is not parent}),
            "animation_sources": {name: resources.get(name, []) for name in files},
            "effect_atlas_metadata_sources": {name: resources.get(name + "_xml.txt", []) for name in sequences},
            "sound_sources": {name: sorted(resources.get(name + ".wav", []) + resources.get(name + ".ogg", []))
                              for name in sounds},
        }
    return {"source_sha256": {p.relative_to(ROOT).as_posix(): hashlib.sha256(p.read_bytes()).hexdigest()
                              for p in paths}, "families": families,
            "limits": "File existence only; template equivalence is structural after documented normalization. "
                      "Does not validate decoded assets, native graph selection, contact or rendered effects."}


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--output", type=Path, help="Optional JSON evidence file (prefer ignored Temp/).")
    args = parser.parse_args()
    result = audit()
    if args.output:
        args.output.write_text(json.dumps(result, indent=2) + "\n", encoding="utf-8")
    for family, row in result["families"].items():
        missing_templates = [t["name"] for t in row["templates"] if not t["exists_in_base"]]
        changed_templates = [t["name"] for t in row["templates"] if t["differences"]]
        missing_sources = [name for key in ("animation_sources", "effect_atlas_metadata_sources", "sound_sources")
                           for name, sources in row[key].items() if not sources]
        print(f"{family}: {len(row['moves'])} moves, {len(row['templates'])} transitive templates; "
              f"missing templates={missing_templates}; changed templates={changed_templates}; "
              f"missing source files={missing_sources}")
