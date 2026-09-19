"""Compare archived equipment move consumers with canonical native moves.
Read-only evidence utility; not part of the Lua mod or its runtime.
"""
import copy
import json
from pathlib import Path
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]
FAMILIES = ("Chakram", "MassBomb", "LightningArrow", "Sphere1", "Sphere2", "Sphere3", "ComboSphere3", "MindThrowNormal")


def normalize(node):
    node.attrib.pop("PackName", None)
    # Recovered attack IDs are bookkeeping, not authored move behavior.
    if node.tag == "Interval":
        node.attrib.pop("ID", None)
    for child in list(node):
        normalize(child)
        # A one-choice random sound dispatches the same clip at the same time.
        if node.tag == "Actions" and child.tag == "Sound":
            attributes = dict(child.attrib)
            child.tag = "RandomSound"
            child.attrib.clear()
            for key in ("Frame", "Event"):
                if key in attributes:
                    child.set(key, attributes.pop(key))
            ET.SubElement(child, "Sound", attributes).text = ""
    node.text = (node.text or "").strip()
    node.tail = ""


def differences(base, archived, path=""):
    path += "/" + base.tag
    result = []
    if base.tag != archived.tag or base.attrib != archived.attrib or base.text != archived.text:
        result.append({"path": path, "base": {"tag": base.tag, **base.attrib, "text": base.text},
                       "archive": {"tag": archived.tag, **archived.attrib, "text": archived.text}})
    if len(base) != len(archived):
        result.append({"path": path, "base_children": len(base), "archive_children": len(archived)})
    for index, (left, right) in enumerate(zip(base, archived)):
        result.extend(differences(left, right, path + "[" + str(index) + "]"))
    return result


def main():
    base = ET.parse(ROOT / "Assets/vanillaXml/animations/moves.xml")
    archive = ET.parse(ROOT / "Assets/DExml/animations/moves.xml")
    canonical = {node.get("Name"): node for node in base.iter("Move")}
    result = {}
    for family in FAMILIES:
        consumers = []
        for node in archive.iter("Move"):
            if not any(item.get("SubType") == family for item in node.iter("Item")):
                continue
            source = canonical.get(node.get("Name"))
            row = {"move": node.get("Name"), "exists_in_base": source is not None}
            if source is not None:
                left, right = copy.deepcopy(source), copy.deepcopy(node)
                normalize(left)
                normalize(right)
                row["differences"] = differences(left, right)
            consumers.append(row)
        result[family] = consumers
    print(json.dumps(result, indent=2, ensure_ascii=True))


if __name__ == "__main__":
    main()
