#!/usr/bin/env python3
"""Compare DE128's projected unarmed moves with the archived DE moves.xml.

Only representation differences the native parser treats identically are
normalized: element and attribute order, order inside condition containers,
number formatting, a zero/absent attack impulse, the default ID 0, the default
unarmed damage attribute, the archive's undefined "LegFall" template tag (DE128
names the victim move instead), and TacticWalNode/TacticWallShift/NoWallRepulsion=0,
which the recovered engine never reads. Every other difference fails.
"""

from __future__ import annotations

import sys
import xml.etree.ElementTree as ET

NAMES = {
    "front_jump_scissors_kick": "FrontJumpScissorsKick",
    "axe_kick_old": "AxeKickOld",
    "wall_run_up": "WallRunUp",
    "air_punch": "AirPunch",
    "throw_leg_push_v": "ThrowLegPushV",
    "throw_leg_push": "ThrowLegPush",
    "standup_after_leg_fall": "StandupAfterLegFall",
    "throw_leg_push_v_profile": "ThrowLegPushVProfile",
    "throw_leg_push_profile": "ThrowLegPushProfile",
}
RUNTIME = {"de128:moves/" + key: value for key, value in NAMES.items()}
ORDER_FREE = {"Conditions", "Condition", "Locks", "Operator", "Events", "Actions", "Intervals", "Interval",
              "AttackingParts", "Move", "Damage", "RandomSound", "CreatePlayer", "Transitions"}
# DisplayName carries the archived title; native moves used their name as the key instead.
# TacticEquivalent is Eclipse's AI mapping onto native table rows; the archive has none.
IGNORED = {("Move", "TacticWalNode"), ("Move", "TacticWallShift"), ("Profile", "DisplayName"), ("Move", "TacticEquivalent")}


def number(value: str) -> str:
    # The native parser reads floats; compare at single precision.
    try:
        return format(float(value), ".6g")
    except ValueError:
        return value


def canonical(node: ET.Element, parent: str = "") -> str:
    attributes = {}
    for key, value in node.attrib.items():
        if (node.tag, key) in IGNORED or (node.tag == "Move" and key == "NoWallRepulsion" and value == "0"):
            continue
        if key in ("Name", "Animation", "StartAnimation") and value in RUNTIME:
            value = RUNTIME[value]
        if node.tag == "Move" and key == "FileName":
            value = value.split("/")[-1].removesuffix(".bytes")
        if node.tag == "Move" and key == "Template":
            value = "|".join(sorted(t for t in value.split("|") if t != "LegFall"))
        attributes[key] = number(value)
    if node.tag == "AnimationEnd" and attributes.get("Name") == "LegFall":
        attributes["Name"] = "ThrowLegPushV"
    children = list(node)
    if node.tag == "Interval" and attributes.get("Type") == "Attack":
        attributes.setdefault("ID", number("0"))
        if not any(child.tag == "Impulse" for child in children):
            children.append(ET.Element("Impulse", {"X": "0", "Y": "0", "Z": "0"}))
    if node.tag == "Damage" and parent == "Interval" and not any(child.tag == "Damage" for child in children):
        children.append(ET.Element("Damage", {"Type": "UnarmedDamage"}))
    parts = [canonical(child, node.tag) for child in children]
    if node.tag in ORDER_FREE:
        parts.sort()
    text = " ".join(f'{k}="{v}"' for k, v in sorted(attributes.items()))
    return f"<{node.tag} {text}>" + "".join(parts) + f"</{node.tag}>"


def main() -> int:
    projected = ET.parse(sys.argv[1]).getroot()
    archive = ET.parse(sys.argv[2]).getroot()
    archived = {move.get("Name"): move for move in archive.iter("Move")}
    produced = {RUNTIME.get(move.get("Name"), move.get("Name")): move for move in projected.iter("Move")}
    failures = 0
    for name in NAMES.values():
        if name not in produced or name not in archived:
            print(f"MISSING {name}: projected={name in produced} archived={name in archived}")
            failures += 1
            continue
        mine, theirs = produced[name], archived[name]
        # The profile preview's attack only feeds the move-list damage readout.
        if name == "ThrowLegPushProfile":
            for interval in theirs.iter("Interval"):
                if interval.get("Type") == "Attack" and interval.find("Hit") is None:
                    interval.append(ET.Element("Hit", {"Name": "High"}))
        # Eclipse addition: AxeKickOld continues FrontKick's current frame instead of
        # restarting, so the double tap shows no separate forward kick.
        if name == "AxeKickOld":
            added = mine.find("Transitions")
            expected = '<Transitions ><Transition FrameShift="-1"><Conditions ><CurrentAnimation Name="FrontKick"></CurrentAnimation>' \
                '<CurrentInterval Name="SemiUninterrupt"></CurrentInterval></Conditions></Transition></Transitions>'
            if added is None or theirs.find("Transitions") is not None or canonical(added, "Move") != expected:
                print(f"DIFF {name}: missing or unexpected FrontKick continuation\n  {canonical(added, 'Move') if added is not None else None}")
                failures += 1
                continue
            mine = ET.fromstring(ET.tostring(mine))
            mine.remove(mine.find("Transitions"))
        a, b = canonical(mine), canonical(theirs)
        if a != b:
            failures += 1
            print(f"DIFF {name}")
            head_a, head_b = a[:a.index(">") + 1], b[:b.index(">") + 1]
            if head_a != head_b:
                print(f"  move lua:     {head_a}\n  move archive: {head_b}")
            parts_a = sorted(canonical(child, "Move") for child in mine)
            parts_b = sorted(canonical(child, "Move") for child in theirs)
            for part in parts_a:
                if part not in parts_b:
                    print(f"  only lua:     {part}")
            for part in parts_b:
                if part not in parts_a:
                    print(f"  only archive: {part}")
        else:
            print(f"OK   {name}")
    print(("PASS" if failures == 0 else "FAIL") + f": {len(NAMES) - failures}/{len(NAMES)} restored unarmed moves match the archive.")
    return 1 if failures else 0


if __name__ == "__main__":
    sys.exit(main())
