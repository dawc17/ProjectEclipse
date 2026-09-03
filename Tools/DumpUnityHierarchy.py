#!/usr/bin/env python3
"""Dump a readable hierarchy of a Unity scene/prefab (YAML) for research.

Usage: python Tools/DumpUnityHierarchy.py <file.unity|file.prefab> [rootNameFilter]

Prints GameObject tree with RectTransform geometry and component types.
Script GUIDs are resolved through .meta files when a --scripts dir is given.
"""
import re
import sys
import os

TYPE_NAMES = {
    "1": "GameObject", "4": "Transform", "224": "RectTransform",
    "222": "CanvasRenderer", "114": "MonoBehaviour", "223": "Canvas",
    "225": "CanvasGroup", "212": "SpriteRenderer", "20": "Camera",
    "82": "AudioSource", "95": "Animator", "111": "Animation",
    "33": "MeshFilter", "23": "MeshRenderer", "1001": "PrefabInstance",
}


def _reconfigure_stdout():
    try:
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    except AttributeError:
        pass


def parse(path):
    docs = {}
    cur_id = None
    cur_type = None
    lines = []
    with open(path, "r", encoding="utf-8", errors="replace") as fh:
        for line in fh:
            m = re.match(r"^--- !u!(\d+) &(\d+)", line)
            if m:
                if cur_id:
                    docs[cur_id] = (cur_type, lines)
                cur_type, cur_id = m.group(1), m.group(2)
                lines = []
            else:
                lines.append(line.rstrip("\n"))
    if cur_id:
        docs[cur_id] = (cur_type, lines)
    return docs


def field(lines, name):
    pat = re.compile(r"^\s*" + re.escape(name) + r":\s*(.*)$")
    for ln in lines:
        m = pat.match(ln)
        if m:
            return m.group(1).strip()
    return None


def fileid(value):
    if not value:
        return None
    m = re.search(r"fileID:\s*(-?\d+)", value)
    return m.group(1) if m else None


def guid(value):
    if not value:
        return None
    m = re.search(r"guid:\s*([0-9a-f]+)", value)
    return m.group(1) if m else None


def script_names(scripts_dir):
    names = {}
    if not scripts_dir or not os.path.isdir(scripts_dir):
        return names
    for root, _dirs, files in os.walk(scripts_dir):
        for f in files:
            if not f.endswith(".cs.meta"):
                continue
            p = os.path.join(root, f)
            try:
                with open(p, "r", encoding="utf-8", errors="replace") as fh:
                    text = fh.read()
            except OSError:
                continue
            m = re.search(r"guid:\s*([0-9a-f]+)", text)
            if m:
                names[m.group(1)] = f[:-8]
    return names


def main():
    if len(sys.argv) < 2:
        print(__doc__)
        return 1
    _reconfigure_stdout()
    path = sys.argv[1]
    name_filter = sys.argv[2] if len(sys.argv) > 2 and not sys.argv[2].startswith("--") else None
    scripts_dir = None
    if "--scripts" in sys.argv:
        scripts_dir = sys.argv[sys.argv.index("--scripts") + 1]
    guid_names = script_names(scripts_dir)

    docs = parse(path)

    # component owner map
    go_components = {}
    transforms = {}
    for oid, (otype, lines) in docs.items():
        if otype == "1":
            comps = re.findall(r"component:\s*\{fileID:\s*(-?\d+)\}", "\n".join(lines))
            go_components[oid] = comps
        if otype in ("4", "224"):
            transforms[oid] = lines

    children = {}
    parent_of = {}
    tr_go = {}
    for tid, lines in transforms.items():
        go = fileid(field(lines, "m_GameObject"))
        tr_go[tid] = go
        par = fileid(field(lines, "m_Father"))
        parent_of[tid] = par
        kids = re.findall(r"\{fileID:\s*(-?\d+)\}", field(lines, "m_Children") or "")
        block = []
        started = False
        for ln in lines:
            if ln.startswith("  m_Children:"):
                started = True
                continue
            if started:
                if ln.startswith("  - {fileID:"):
                    block.append(re.search(r"fileID:\s*(-?\d+)", ln).group(1))
                    continue
                break
        children[tid] = block or kids

    def describe(tid, depth):
        go = tr_go.get(tid)
        gtype, glines = docs.get(go, ("?", []))
        name = field(glines, "m_Name") or "?"
        active = field(glines, "m_IsActive")
        lines = transforms[tid]
        pad = "  " * depth
        geo = ""
        if docs[tid][0] == "224":
            def v(n):
                raw = field(lines, n) or ""
                m = re.search(r"x:\s*(-?[\d.eE+]+),\s*y:\s*(-?[\d.eE+]+)", raw)
                return "(%s,%s)" % (m.group(1), m.group(2)) if m else raw
            geo = " anchorMin=%s anchorMax=%s pivot=%s pos=%s size=%s" % (
                v("m_AnchorMin"), v("m_AnchorMax"), v("m_Pivot"),
                v("m_AnchoredPosition"), v("m_SizeDelta"))
        comps = []
        for cid in go_components.get(go, []):
            ctype = docs.get(cid, ("?", []))[0]
            cname = TYPE_NAMES.get(ctype, "Type" + ctype)
            if ctype == "114":
                g = guid(field(docs[cid][1], "m_Script"))
                cname = guid_names.get(g, "MonoBehaviour(%s)" % g)
            comps.append(cname)
        print("%s%s%s [%s]%s" % (pad, name, "" if active == "1" else " (inactive)",
                                 ", ".join(comps), geo))
        for cid in go_components.get(go, []):
            ctype, clines = docs.get(cid, ("?", []))
            if ctype == "114":
                for ln in clines:
                    if re.match(r"^  m_(Script|GameObject|ObjectHideFlags|Corresponding|Prefab|Enabled|EditorHideFlags|EditorClassIdentifier|Name)", ln):
                        continue
                    if ln.startswith("  "):
                        print("%s  . %s" % (pad, ln.strip()))
        for kid in children.get(tid, []):
            if kid in transforms:
                describe(kid, depth + 1)

    roots = [t for t in transforms if not parent_of.get(t) or parent_of[t] == "0"]
    for r in sorted(roots, key=lambda t: int(t)):
        go = tr_go.get(r)
        nm = field(docs.get(go, ("?", []))[1], "m_Name") or ""
        if name_filter and name_filter.lower() not in nm.lower():
            continue
        describe(r, 0)
    return 0


if __name__ == "__main__":
    sys.exit(main())
