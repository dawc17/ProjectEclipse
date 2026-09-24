"""Generate DE128's Underworld Lua data from the archived DE XML.

Reads Assets/DExml (raid_stages_default.xml, stages.xml templates, list.xml,
localizations) once, offline, and writes typed Lua tables under
Mods/de128/scripts/content/. The shipped mod never reads XML. Run with --check
to verify the committed Lua is current.

Deliberate normalizations (each recorded in the generated notes):
- Fight Rounds="0" becomes rounds = 1, matching Eclipse's offline raid adaptation
  (UnderworldStageCompatibility.AdaptOfflineRaidRounds).
- Perk, item and rule names the DE data itself cannot resolve are omitted, as the
  native loader would ignore them.
- LightInTheDarkness has no recovered rule class; it is reported, not emitted.
"""

from __future__ import annotations

import argparse
import re
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
DE = ROOT / "Assets" / "DExml"
VANILLA = ROOT / "Assets" / "vanillaXml"
OUT = ROOT / "Mods" / "de128" / "scripts" / "content"
# Owner decision (2026-09-24): dojo_india24 exists only as a 768 px atlas stretched over
# 1536 world units; Faradeya fights in Dandy's near-identical, high-resolution dojo_india25.
LOCATION_OVERRIDES = {"dojo_india24": "dojo_india25"}
LANGUAGES = ["cro", "eng", "fra", "ger", "hin", "hun", "ita", "kor", "por", "rom", "rus", "spa", "swe", "tur"]
CATEGORY = {"Weapon": "weapon", "Armor": "armor", "Helm": "helm", "Ranged": "ranged", "Magic": "magic"}
# Archive item names restored by DE128 under new identities (sensei_dependencies.lua evidence).
RESTORED_ITEMS = {"Sphere1": "de128:items/magic/minor_charge_of_darkness",
                  "Sphere2": "de128:items/magic/medium_charge_of_darkness"}
TARGET = {"Player": "player", "Bot": "opponent", "All": "all", None: "all"}
ZONE_IDS = ["ZONE_RAID", "ZONE_RAID1", "ZONE_RAID2", "ZONE_RAID3", "ZONE_RAID4", "ZONE_RAID5", "ZONE_RAID6", "ZONE_RAID7"]


def lua(value, indent=0):
    pad = "    " * indent
    if value is None:
        return "nil"
    if isinstance(value, bool):
        return "true" if value else "false"
    if isinstance(value, (int, float)):
        text = repr(value)
        return text[:-2] if text.endswith(".0") else text
    if isinstance(value, str):
        return '"' + value.replace("\\", "\\\\").replace('"', '\\"').replace("\n", "\\n") + '"'
    if isinstance(value, list):
        if not value:
            return "{}"
        simple = all(not isinstance(v, (dict, list)) for v in value)
        if simple:
            return "{ " + ", ".join(lua(v) for v in value) + " }"
        return "{\n" + "".join(pad + "    " + lua(v, indent + 1) + ",\n" for v in value) + pad + "}"
    if isinstance(value, dict):
        items = [(k, v) for k, v in value.items() if v is not None]
        if not items:
            return "{}"
        def key(k):
            return k if re.fullmatch(r"[A-Za-z_][A-Za-z0-9_]*", k) else "[" + lua(k) + "]"
        parts = [key(k) + " = " + lua(v, indent + 1) for k, v in items]
        one = "{ " + ", ".join(parts) + " }"
        if len(one) + len(pad) <= 118 and "\n" not in one:
            return one
        return "{\n" + "".join(pad + "    " + p + ",\n" for p in parts) + pad + "}"
    raise TypeError(value)


def number(text):
    value = float(text)
    return int(value) if value.is_integer() and "." not in text and "e" not in text.lower() else value


class Generator:
    def __init__(self):
        self.raid = ET.parse(DE / "raid_stages_default.xml").getroot()
        stages = ET.parse(DE / "stages.xml").getroot()
        self.templates = {t.get("Name"): t for t in stages.iter("Template")}
        self.core_templates = {t.get("Name") for t in ET.parse(VANILLA / "stages.xml").getroot().iter("Template")}
        self.core_items = {}
        for item in ET.parse(VANILLA / "list.xml").getroot().iter("Item"):
            if item.get("Type") and item.get("Name") not in self.core_items:
                self.core_items[item.get("Name")] = item.get("Type")
        self.core_perks = {p.get("Name") for p in ET.parse(VANILLA / "perks.xml").getroot().iter("Perk")}
        self.keys = set()
        self.notes = []
        self.omitted = {"items": set(), "perks": set(), "rules": set()}

    # --- references -------------------------------------------------------
    def text_key(self, key):
        self.keys.add(key)
        return key

    def item(self, name):
        if name in RESTORED_ITEMS:
            return {"ref": RESTORED_ITEMS[name]}
        kind = self.core_items.get(name)
        if kind == "Skeleton":
            return {"skeleton": name}
        if kind in CATEGORY:
            return {"ref": "core:items/" + CATEGORY[kind] + "/" + name}
        self.omitted["items"].add(name)
        return None

    def perk(self, name):
        if name in self.core_perks:
            return "core:perks/" + name
        self.omitted["perks"].add(name)
        return None

    def items(self, node):
        refs, skeleton = [], None
        items = node.find("Items")
        if items is None:
            return None, None
        for entry in items.findall("Item"):
            resolved = self.item(entry.get("Name"))
            if resolved is None:
                continue
            if "skeleton" in resolved:
                skeleton = resolved["skeleton"]
            elif resolved["ref"] not in refs:
                refs.append(resolved["ref"])
        return refs, skeleton

    def perk_rows(self, node):
        perks = node.find("Perks")
        if perks is None:
            return None
        rows = []
        for entry in perks.findall("Perk"):
            ref = self.perk(entry.get("Name"))
            if ref is None:
                continue
            row = {"perk": ref}
            params = {}
            for setting in entry.findall("Set"):
                for attr, text in setting.attrib.items():
                    if attr == "Aspect": row["aspect"] = number(text)
                    elif attr == "Chance": row["chance"] = number(text)
                    elif attr == "ChanceFactor": row["chance_factor"] = number(text)
                    elif attr == "Frames": row["frames"] = int(float(text))
                    else: params[attr] = number(text)
            if params:
                row["parameters"] = params
            rows.append(row)
        return rows

    def alignments(self, node):
        align = node.find("AttributesAlign")
        if align is None:
            return None
        return [{"factor": number(d.get("Factor", "0")), "shift": number(d.get("Shift", "0")),
                 "priority": int(d.get("Priority", "0"))} for d in align.findall("Delta")]

    # --- templates ----------------------------------------------------------
    def build_templates(self):
        used = []
        def visit(name):
            if not name or name in self.core_templates or name in used:
                return
            template = self.templates[name]
            visit(template.get("Template"))
            used.append(name)
        for warrior in self.raid.iter("Warrior"):
            visit(warrior.get("Template"))
        result = []
        for name in used:
            t = self.templates[name]
            parent = t.get("Template")
            attributes = {}
            for attr, text in t.attrib.items():
                if attr in ("Name", "Template", "FirstName", "Avatar", "Voice", "ShieldTotal"):
                    continue
                attributes[attr] = number(text)
            items, skeleton = self.items(t)
            result.append({
                "name": name,
                "parent": (("core:" if parent in self.core_templates else "") + parent) if parent else None,
                "first_name": self.text_key(t.get("FirstName")) if t.get("FirstName") else None,
                "avatar": t.get("Avatar"), "voice": t.get("Voice"),
                "health_bars": int(t.get("ShieldTotal")) if t.get("ShieldTotal") else None,
                "attributes": attributes or None, "items": items, "skeleton": skeleton,
            })
        return result

    # --- rules --------------------------------------------------------------
    def rule(self, node):
        tag = node.tag
        target = TARGET.get(node.get("ApplyTo"), None)
        if node.get("ApplyTo") not in TARGET:
            raise ValueError("Unknown ApplyTo " + node.get("ApplyTo"))
        if tag == "Perk":
            ref = self.perk(node.get("Name"))
            if ref is None:
                return None
            row = {"kind": "perk", "perk": ref, "target": target}
            params = {}
            for setting in node.findall("Set"):
                for attr, text in setting.attrib.items():
                    if attr == "Aspect": row["aspect"] = number(text)
                    else: params[attr] = number(text)
            if params: row["parameters"] = params
            return row
        if tag == "RulesWithConditions":
            conditions = [(e.get("Value1"), e.get("Value2")) for e in node.iter("Equal")]
            body = [c.tag + ":" + (c.get("Name") or "") for c in node.find("RuleList")]
            if conditions != [("_RaidChargeButton", "0")] or body != ["NoButton:RaidCharge"]:
                raise ValueError("Unexpected conditional rule " + ET.tostring(node, encoding="unicode"))
            return {"kind": "raid_charge"}
        if tag == "ComplexRule":
            description = node.find("Description")
            children = [self.rule(c) for c in node if c.tag != "Description"]
            children = [c for c in children if c is not None]
            if not children:
                return None
            return {"kind": "group", "description": self.text_key(description.get("Alias")) if description is not None else None,
                    "rules": children}
        if tag == "RandomRule":
            children = [c for c in (self.rule(c) for c in node) if c is not None]
            refresh = {"EachRound": "each_round", "EachFight": "each_fight"}[node.get("Refresh", "EachFight")]
            return {"kind": "random", "refresh": refresh, "rules": children}
        if tag in ("EquipItem",):
            resolved = self.item(node.get("Name"))
            if resolved is None or "ref" not in resolved:
                return None
            return {"kind": "equip_item", "item": resolved["ref"], "target": target}
        if tag in ("Avatar", "Name", "NoButton"):
            return {"kind": tag.lower().replace("nobutton", "no_button"), "name": node.get("Name"), "target": target}
        if tag == "HotGround":
            return {"kind": "hot_ground", "frames": int(node.get("Frames")), "target": target,
                    "nodes": [{k: v for k, v in (("name", n.get("Name")), ("axis", n.get("Axis")),
                               ("min", number(n.get("Min")) if n.get("Min") else None),
                               ("max", number(n.get("Max")) if n.get("Max") else None)) if v is not None}
                              for n in node.findall("Node")],
                    "animations": [a.get("Name") for a in node.findall("Animation")]}
        if tag == "RandomArea":
            return {"kind": "random_area", "image": node.get("Image"), "icon": node.get("Icon"),
                    "width": number(node.get("Width")), "fade_in": int(node.get("FadeIn")),
                    "frames_on": int(node.get("FramesOn")), "fade_out": int(node.get("FadeOut")),
                    "frames_off": int(node.get("FramesOff")), "target": target}
        if tag == "NoAnimation":
            return {"kind": "no_animation", "name": node.get("Name")}
        if tag == "RemoveInterval":
            return {"kind": "remove_interval", "type": node.get("Type"), "target": target}
        if tag == "Regeneration":
            return {"kind": "regeneration", "rate": number(node.get("Rate")),
                    "frames_after_hit": int(node.get("FramesAfterHit")), "target": target}
        if tag == "Attributes":
            values = {k: number(v) for k, v in node.attrib.items() if k != "ApplyTo"}
            return {"kind": "attributes", "values": values, "target": target}
        if tag == "NoHealthBar":
            return {"kind": "no_health_bar", "target": target}
        if tag == "InvertJoystick":
            return {"kind": "invert_joystick", "target": target}
        if tag == "LightInTheDarkness":
            self.omitted["rules"].add("LightInTheDarkness")
            return None
        raise ValueError("Unsupported rule " + tag)

    # --- battles ------------------------------------------------------------
    def build_zones(self):
        zones = []
        for index, zone in enumerate(self.raid.iter("Zone")):
            assert zone.get("Name") == ZONE_IDS[index], zone.get("Name")
            self.text_key(zone.get("Name"))
            battles = []
            for battle in zone.findall("Battle"):
                name = battle.get("Name")
                twin = name.endswith("_HARDMODE")
                has_twin = any(b.get("Name") == name + "_HARDMODE" for b in zone.findall("Battle"))
                power = "power" if twin else ("normal" if has_twin else None)
                description = battle.get("Description")
                desc_key, desc_param = (description, None)
                match = re.fullmatch(r"([^{]+)\{(\d+)\}", description or "")
                if match:
                    desc_key, desc_param = match.group(1), int(match.group(2))
                fights = []
                for fight in battle.findall("Fight"):
                    rewards = []
                    for reward in fight.find("Rewards").findall("Reward"):
                        rewards.append({
                            "prize_base": number(reward.get("PrizeBase")) if reward.get("PrizeBase") else None,
                            "experience": int(reward.get("Exp")) if reward.get("Exp") else None,
                            "gems": int(reward.get("Bonus")) if reward.get("Bonus") else None,
                            "currencies": [{"currency": c.get("Name"), "expected": number(c.get("ExpectedValue")),
                                            "show": c.get("ShowReward") == "1"} for c in reward.findall("Currency")] or None,
                        })
                    warriors = []
                    for w in fight.find("Warriors").findall("Warrior"):
                        attributes = {}
                        for attr in ("MagicInitialCharge", "WarriorPower"):
                            if w.get(attr): attributes[attr] = number(w.get(attr))
                        items, skeleton = self.items(w)
                        warriors.append({
                            "template": ("core:" if w.get("Template") in self.core_templates else "") + w.get("Template"),
                            "tactic": w.get("Tactic"), "avatar": w.get("Avatar"),
                            "health_bars": int(w.get("ShieldTotal")) if w.get("ShieldTotal") else None,
                            "attributes": attributes or None, "alignments": self.alignments(w),
                            "perks": self.perk_rows(w), "items": items, "skeleton": skeleton,
                        })
                    rules = [r for r in (self.rule(r) for r in fight.find("Rules")) if r is not None]
                    rounds = int(fight.get("Rounds", "1"))
                    fights.append({
                        "name": fight.get("Name"), "power": int(fight.get("Power", "0")),
                        "rounds": rounds if rounds > 0 else 1, "round_time": int(fight.get("RoundTime")),
                        "replays": int(fight.get("Replays", "0")),
                        "health_recovery": number(fight.get("HealthRecovery")) if fight.get("HealthRecovery") else None,
                        "rewards": rewards, "warriors": warriors, "rules": rules,
                    })
                battles.append({
                    "name": name, "type": {"FINAL_BATTLE": "final", "SURVIVAL": "survival"}[battle.get("Type")],
                    "alias": self.text_key(battle.get("Alias")), "title": self.text_key(battle.get("Title")),
                    "description": self.text_key(desc_key) if desc_key else None, "description_level": desc_param,
                    "icon": battle.get("Icon"), "icon_atlas": battle.get("IconAtlas"), "preview": battle.get("Preview"),
                    "x": int(battle.get("X")), "y": int(battle.get("Y")),
                    "location": LOCATION_OVERRIDES.get(battle.get("Location"), battle.get("Location")), "music": battle.get("Music"), "power_mode": power,
                    "fights": fights,
                })
            zones.append({"name": zone.get("Name"), "file": zone.get("FileName"), "battles": battles})
        return zones

    # --- story ----------------------------------------------------------------
    def build_story(self):
        quests = ET.parse(DE / "quests.xml").getroot()
        by_name = {q.get("Name"): q for q in quests.iter("Quest")}
        # Users variables assigned exactly once (FirstGuardBeaten) are constants.
        assigned = {}
        for setter in quests.iter("SetVariable"):
            if setter.get("Scope") == "Users":
                assigned.setdefault(setter.get("Name"), set()).add(setter.get("Value"))
        def resolve(value):
            if value and value.startswith("_") and not value.startswith("_$"):
                values = assigned.get(value[1:], set())
                if len(values) != 1:
                    raise ValueError("Unresolvable story variable " + value)
                return next(iter(values))
            return value
        self.portraits = set()
        english = {w.get("Title"): (w.text or "") for w in ET.parse(DE / "localizations" / "eng.xml").getroot().iter("Word")}
        def title_key(key):
            # Announcement dialogs name a title whose archived text is empty: no title.
            key = resolve(key)
            return self.text_key(key) if key and english.get(key, "").strip() else None
        def steps(actions):
            result = []
            for action in actions:
                if action.tag == "ActScreen":
                    lines = [{"text": self.text_key(l.get("Text")), "frames": int(l.get("Frames"))} for l in action.findall("Line")]
                    if not lines:
                        # QuestActionAct's Text form: EnterScreen.Init(string) holds for the
                        # prefab's hideTime (Assets/Resources/prefabs/map/EnterScreen.prefab: 5 s).
                        lines = [{"text": self.text_key(action.get("Text")), "frames": 300}]
                    result.append({"lines": lines})
                elif action.tag == "Dialog":
                    # NoAvatar is the same native dialog with its portrait hidden.
                    assert action.get("Type") in ("Regular", "NoAvatar"), action.get("Type")
                    assert (action.get("Type") == "NoAvatar") == (action.get("Image") is None)
                    button = action.find("Button")
                    lines = action.findall("Line")
                    assert len(lines) == 1
                    portrait = resolve(action.get("Image"))
                    if portrait:
                        self.portraits.add(portrait)
                    scene = button.find("ChangeScene")
                    result.append({
                        "title": title_key(action.get("Title")), "text": self.text_key(lines[0].get("Text")),
                        "portrait": portrait, "mirrored": action.get("Mirrored") == "1" or None,
                        "ignore_back": action.get("IgnoreBack") == "1" or None,
                        "button": self.text_key(button.get("Text")),
                        "launch": (button.find("Fight") is not None) or None,
                        "scene": scene.get("Destination").lower() if scene is not None else None,
                    })
            return result
        intro = by_name["RaidIntro"]
        unlock = [e.get("Value1") for e in intro.iter("GreaterEqual")]
        assert unlock == ["?Fight[ZONE_1|BOSS_LYNX|2].WinCount"], unlock
        focus = intro.find("Actions/SetMapFocus").get("Battle").split("|")[1]
        story = {
            "unlock_fight": "core:fights/zone_1/boss_lynx/2", "focus": focus,
            "intro": steps(intro.find("Actions")), "followup": steps(by_name["RaidIntro2"].find("Actions")),
            "bosses": {},
        }
        for name, quest in by_name.items():
            match = re.fullmatch(r"Raid(Enter|WinAfter|LoseAfter)(.+)", name or "")
            if not match:
                continue
            fight = [e.get("Value2") for e in quest.iter("Equal") if e.get("Value1") == "_$Fight"]
            assert len(fight) == 1 and fight[0].endswith("|1"), (name, fight)
            battle = fight[0].split("|")[1]
            kind = {"Enter": "enter", "WinAfter": "win", "LoseAfter": "loss"}[match.group(1)]
            entry = story["bosses"].setdefault(battle, {})
            assert kind not in entry, name
            entry[kind] = steps(quest.find("Actions"))
        for battle, entry in story["bosses"].items():
            assert sorted(entry) == ["enter", "loss", "win"], (battle, sorted(entry))
            # Entry sequences end with the explicit Fight button, nowhere else.
            assert [bool(step.get("launch")) for step in entry["enter"]] == [False] * (len(entry["enter"]) - 1) + [True], battle
        return story

    # --- localization -------------------------------------------------------
    def build_text(self):
        values = {}
        missing = set()
        tables = {}
        for language in LANGUAGES:
            words = {}
            for word in ET.parse(DE / "localizations" / (language + ".xml")).getroot().iter("Word"):
                words.setdefault(word.get("Title"), word.text or "")
            tables[language] = words
        lowered = {}
        for key in sorted(self.keys):
            other = lowered.setdefault(key.lower(), key)
            if other != key:
                raise ValueError("Localization keys collide case-insensitively: %s / %s" % (other, key))
            if not tables["eng"].get(key, "").strip():
                missing.add(key)
                continue
            for language in LANGUAGES:
                # An empty archived translation falls back to English natively.
                if tables[language].get(key, "").strip():
                    values.setdefault(language, {})[key] = tables[language][key]
        return values, missing

    def render(self):
        templates = self.build_templates()
        zones = self.build_zones()
        story = self.build_story()
        values, missing = self.build_text()
        header = "-- GENERATED by Tools/GenerateDE128Underworld.py from Assets/DExml; do not edit by hand.\n"
        data = header + "return " + lua({"templates": templates, "zones": zones}) + "\n"
        story_text = header + "return " + lua(story) + "\n"
        text = header + "return " + lua(values) + "\n"
        notes = {
            "omitted_items": sorted(self.omitted["items"]), "omitted_perks": sorted(self.omitted["perks"]),
            "omitted_rules": sorted(self.omitted["rules"]), "missing_text": sorted(missing),
            "templates": len(templates), "zones": len(zones),
            "battles": sum(len(z["battles"]) for z in zones),
            "fights": sum(len(b["fights"]) for z in zones for b in z["battles"]),
            "keys": len(self.keys), "story_bosses": len(story["bosses"]),
            "story_portraits": sorted(self.portraits),
        }
        return data, text, story_text, notes


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--check", action="store_true")
    args = parser.parse_args()
    data, text, story, notes = Generator().render()
    outputs = {OUT / "underworld_data.lua": data, OUT / "underworld_text_values.lua": text,
               OUT / "underworld_story_data.lua": story}
    stale = [path for path, content in outputs.items() if not path.exists() or path.read_text(encoding="utf-8") != content]
    if args.check:
        for path in stale:
            print("STALE", path.relative_to(ROOT))
    else:
        for path, content in outputs.items():
            path.write_text(content, encoding="utf-8", newline="\n")
    for key, value in notes.items():
        print(key + ":", value)
    return 1 if args.check and stale else 0


if __name__ == "__main__":
    sys.exit(main())
