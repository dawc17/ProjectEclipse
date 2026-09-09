"""Inventory every archived DE XML delta against canonical Eclipse XML.

Preserves child order and values, ignores indentation/attribute ordering/comments.
Identity matching is structural, not proof of semantic equivalence or DE intent.
Run --write to regenerate the review ledger; otherwise check it for source drift.
"""
import argparse
from collections import Counter, defaultdict
import hashlib
import gzip
import json
from pathlib import Path
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]
OUTPUT = ROOT / "Mods/DE_XML_DELTA_LEDGER.json.gz"
KEYS = ("Name", "name", "ID", "Id", "id", "IDS", "Key", "key", "Code", "code", "Title", "Level", "File", "Type")


def canonical(node):
    return [node.tag, dict(sorted(node.attrib.items())), (node.text or "").strip(),
            [canonical(c) for c in node], (node.tail or "").strip()]


def children(node):
    result = {}
    counts = Counter()
    for child in node:
        key = next((f'{child.tag}[@{k}={json.dumps(child.get(k), ensure_ascii=False)}]' for k in KEYS if child.get(k) is not None), child.tag)
        counts[key] += 1
        result[f"{key}[{counts[key]}]"] = child
    return result


def diff(a, b, path):
    if a is None or b is None:
        return [dict(path=path, operation="add" if a is None else "remove",
                     before=canonical(a) if a is not None else None,
                     after=canonical(b) if b is not None else None)]
    rows = []
    if a.tag != b.tag:
        rows.append(dict(path=path, operation="tag", before=a.tag, after=b.tag))
    for field in sorted(a.attrib.keys() | b.attrib.keys()):
        if a.get(field) != b.get(field):
            rows.append(dict(path=path + "/@" + field, operation="attribute", before=a.get(field), after=b.get(field)))
    for field, av, bv in (("text()", a.text, b.text), ("tail()", a.tail, b.tail)):
        if (av or "").strip() != (bv or "").strip():
            rows.append(dict(path=path + "/" + field, operation="text", before=(av or "").strip(), after=(bv or "").strip()))
    ac, bc = children(a), children(b)
    common = ac.keys() & bc.keys()
    if [k for k in ac if k in common] != [k for k in bc if k in common]:
        rows.append(dict(path=path, operation="order", before=list(ac), after=list(bc)))
    for key in sorted(ac.keys() | bc.keys()):
        rows.extend(diff(ac.get(key), bc.get(key), path + "/" + key))
    return rows


def domain(path):
    if path.startswith("compat/"): return "archived_compatibility_overlay"
    if path.startswith("quest_extensions/") or path == "quests.xml": return "quests"
    if path.startswith("localizations/") or path == "localization.xml": return "localization"
    if path.startswith("locations/"): return "locations"
    if path.startswith("credits/"): return "credits"
    if path.startswith("animations/"): return "moves"
    return {"stages.xml":"stages", "raid_stages_default.xml":"raids", "list.xml":"items_sets",
            "perks.xml":"perks", "CharacterProgress.xml":"progression", "Achievements.xml":"achievements",
            "forge.xml":"forge", "tacticSettings.xml":"ai", "ComputerSettings.xml":"ai",
            "usersDefault.xml":"default_profile", "usersDefaultWarrior.xml":"default_profile"}.get(path, "configuration")


def inventory():
    roots = [ROOT / "Assets" / tree for tree in ("vanillaXml", "DExml")]
    files = [{p.relative_to(root).as_posix():p for p in root.rglob("*.xml")} for root in roots]
    entries = []
    for name in sorted(files[0].keys() | files[1].keys()):
        nodes, hashes = [], []
        for paths in files:
            p = paths.get(name)
            nodes.append(ET.parse(p).getroot() if p else None)
            hashes.append(hashlib.sha256(p.read_bytes()).hexdigest() if p else None)
        a, b = nodes
        changes = diff(a, b, "/" + (b.tag if b is not None else a.tag))
        status = "de_only" if a is None else "vanilla_only" if b is None else "changed" if changes else "equal"
        entries.append(dict(file=name, domain=domain(name), status=status, vanilla_sha256=hashes[0], de_sha256=hashes[1],
                            de_nodes=sum(1 for _ in b.iter()) if b is not None else 0, changes=changes))
    return dict(schema=1, scope="All XML in Assets/DExml and Assets/vanillaXml; current base includes canonical economy edits. Non-XML payloads are not inventoried here.",
                method="Attribute/text/order delta plus complete added/removed subtrees. Named siblings paired by identity and occurrence; numeric strings remain exact. No comment-only differences. File absence does not imply active content removal; trace includes. DE intent is unconfirmed unless separately evidenced.",
                counts=dict(vanilla=len(files[0]), de=len(files[1]), files=len(entries), statuses=dict(Counter(e['status'] for e in entries)), changes=sum(len(e['changes']) for e in entries)), entries=entries)


def feature_index():
    """Named content and required vocabulary, not a claim that each tag needs an API."""
    result = []
    for filename, tags in (("list.xml", ("Item", "ItemSet")), ("perks.xml", ("Perk",)),
                           ("animations/moves.xml", ("Move", "Template")), ("Achievements.xml", ("Counter",)),
                           ("quests.xml", ("Quest",))):
        roots = [ET.parse(ROOT / "Assets" / d / filename).getroot() for d in ("vanillaXml", "DExml")]
        for tag in tags:
            maps = [{n.get("Name"):n for n in r.iter(tag) if n.get("Name")} for r in roots]
            a, b = maps
            added = sorted(b.keys() - a.keys())
            removed = sorted(a.keys() - b.keys())
            changed = sorted(k for k in a.keys() & b.keys() if canonical(a[k]) != canonical(b[k]))
            nodes = [b[k] for k in added + changed]
            vocab = {c:dict(sorted(Counter(n.tag for node in nodes for block in node.iter(c) for n in block).items()))
                     for c in ("Events", "Conditions", "Actions", "Locks")}
            result.append(dict(file=filename, tag=tag, vanilla=len(a), de=len(b), added=added, removed=removed, changed=changed,
                               changed_or_added_vocabulary=vocab))
    return result


def coverage(result):
    assessments = {
        "items_sets": "Partial: G01/G04/G05/G11. Owned items/membership; core edits, chests and ability linkage missing; classify economy separately.",
        "perks": "Partial: G01/G05/G06. Existing templates/basic Lua effects; full changed trigger graphs missing.",
        "moves": "Partial: G01/G08. Move assets/basic definitions; expanded input/action/projectile authoring missing.",
        "quests": "Partial: G01/G02/G03/G04/G14. Basic quest registration; broader story/UI/acquisition and suppression missing.",
        "stages": "Partial: G01/G02/G04/G11. Fight graph works; full core edit and lottery/story conversion missing.",
        "raids": "Partial: G01/G05/G06/G08/G10. Raid loop proven; each boss's mechanics/assets need conversion.",
        "progression": "Partial: G07. Branch overlays work; level-specific parameters/descriptions missing.",
        "achievements": "Partial: G02/G13. Owned counters work; contextual predicates/core edits missing. Platform-ID changes are separate.",
        "ai": "Missing: G09. Conditional reactions not exposed; do not infer global tuning requirements.",
        "locations": "Partial: G01/G03/G10. Static layers/audio supported; animated layers, existing edits and dojo selection need work.",
        "localization": "Partial: G13. Owned strings/locale metadata supported; broad core patching/font/presentation coverage not proven.",
        "forge": "Partial: G01/G12. New families/ranges/timing supported; core candidate editing/removal missing. Costs stay base-owned.",
        "credits": "Presentation: G14. Credits/localization attribution; gameplay API does not cover boot/credits content.",
        "configuration": "Mixed/unresolved: G14. Platform, build, existing service gates and presentation; not blanket API work.",
        "default_profile": "Mixed/unresolved: G14. Startup/profile defaults and existing base policy; no raw save-template replacement.",
        "archived_compatibility_overlay": "Reference only: G14. Compatibility overlay provenance; not automatically intentional DE content.",
    }
    lines = ["# Every XML path: coverage assessment", "", "Generated by `Tools/AuditDEXmlApi.py`. Findings G01–G14 are in [the gap audit](DE_XML_API_GAP_AUDIT.md).",
             "", "This maps every file to a reviewed domain boundary. Partial means at least some required contracts are missing/unverified, not that each node was independently implemented or playtested. Empty roots and absent files are not automatically active gameplay changes.", "",
             "| XML path | Comparison | Delta records | Coverage / follow-up |", "| --- | --- | ---: | --- |"]
    for e in result["entries"]:
        tree = "vanillaXml" if e["status"] == "vanilla_only" else "DExml"
        assessment = "No semantic file delta; no DE addition inferred." if e["status"] == "equal" else assessments[e["domain"]]
        if e["domain"] == "quests" and e["de_nodes"] == 1:
            assessment = "Empty DE root: no authored quests here. G01/G14 for removed/suppressed base flow; verify includes."
        lines.append(f"| [{e['file']}](../Assets/{tree}/{e['file']}) | {e['status']} | {len(e['changes'])} | {assessment} |")
    return "\n".join(lines) + "\n"


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--write", action="store_true")
    args = parser.parse_args()
    result = inventory()
    summary = {**result, "entries": [{k:v for k,v in e.items() if k != "changes"} | {"delta_records": len(e["changes"])} for e in result["entries"]]}
    artifacts = {"DE_XML_FILE_INVENTORY.json": json.dumps(summary, indent=2) + "\n",
                 "DE_XML_FEATURE_INDEX.json": json.dumps(feature_index(), indent=2) + "\n",
                 "DE_XML_FILE_COVERAGE.md": coverage(result)}
    if args.write:
        OUTPUT.write_bytes(gzip.compress(json.dumps(result, ensure_ascii=False, separators=(",", ":")).encode("utf-8"), mtime=0))
        for name, text in artifacts.items(): (OUTPUT.parent / name).write_text(text, encoding="utf-8")
    elif json.loads(gzip.decompress(OUTPUT.read_bytes())) != result:
        raise SystemExit("XML sources changed: review and regenerate the DE XML ledger.")
    elif any((OUTPUT.parent / name).read_text(encoding="utf-8") != text for name, text in artifacts.items()):
        raise SystemExit("Companion inventory/feature/coverage files differ: review and regenerate.")
    print(json.dumps(result["counts"], indent=2))
    groups = defaultdict(Counter)
    for entry in result["entries"]: groups[entry["domain"]][entry["status"]] += 1
    print(json.dumps(groups, indent=2))
