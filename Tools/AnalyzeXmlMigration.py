#!/usr/bin/env python3
"""Audit newer plaintext gamedata against the decompiled SF2 runtime.

The report is deliberately conservative: it inventories every quest/event/action
used by the custom XML and compares the quest vocabulary with the old parser and
the 2.41.9 APK enum recovered by Il2CppDumper.
"""

from __future__ import annotations

import json
import re
import xml.etree.ElementTree as ET
from collections import Counter, defaultdict
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
XML_ROOT = ROOT / "Assets" / "xml"
CS_ROOT = ROOT / "Assets" / "Scripts" / "Assembly-CSharp"
APK_DUMP = (
    ROOT
    / "ResearchSources"
    / "reversingsf2"
    / "analysis_bundle"
    / "il2cppdumper_out"
    / "dump.cs"
)
OUT_ROOT = ROOT / "ResearchSources" / "XMLMigration"

# These newer actions are deliberately translated at the XML boundary. UI-only
# tutorial actions become an immediate-completion action; gameplay visibility
# and shop-refresh actions map to their legacy equivalents.
ADAPTED_ACTIONS = {
    "BlockTouches",
    "ClickButton",
    "ForgeTutorialEnchantItem",
    "ForgeTutorialGiveRequiredMaterials",
    "ForgeTutorialOpenForge",
    "ForgeTutorialRevealPropertiesPanel",
    "HideArrow",
    "HideHint",
    "SetBattleVisibility",
    "ShowArrow",
    "ShowHint",
    "UnblockTouches",
    "UpdateEclipseBattles",
    "UpdateShopItems",
    "ValidatePacks",
}

RUNTIME_ITEM_ALIASES = {
    "ARMOR_IM_CEREMONIAL": "ARMOR_CEREMONIAL",
    "HELM_IM_CEREMONIAL": "HELM_CEREMONIAL",
    "MAGIC_DARK_WAVE": "MAGIC_C4_Z1_WARLOCK_DARK_WAVE",
    "RANGED_NEEDLES": "RANGED_NEEDLE",
    "WEAPON_SPEAR": "WEAPON_AE21_SPEAR",
}


def parse(path: Path) -> ET.Element:
    return ET.parse(path).getroot()


def csharp_cases(path: Path) -> set[str]:
    text = path.read_text(encoding="utf-8-sig", errors="replace")
    return set(re.findall(r'case\s+"([^"]+)"\s*:', text))


def apk_action_enum() -> set[str]:
    text = APK_DUMP.read_text(encoding="utf-8-sig", errors="replace")
    match = re.search(
        r"public enum BDOPLIGLAPK.*?\{(?P<body>.*?)\n\}", text, re.S
    )
    if not match:
        return set()
    return set(re.findall(r"public const BDOPLIGLAPK\s+(\w+)\s*=", match.group("body")))


def quest_files() -> list[Path]:
    paths = [XML_ROOT / "quests.xml"]
    paths.extend(sorted((XML_ROOT / "quest_extensions").rglob("*.xml")))
    return paths


def quest_inventory() -> dict:
    old_actions = csharp_cases(CS_ROOT / "QuestAction.cs")
    old_events = csharp_cases(CS_ROOT / "QuestEvent.cs")
    new_actions = apk_action_enum()
    all_known_actions = old_actions | new_actions

    events: Counter[str] = Counter()
    actions: Counter[str] = Counter()
    action_files: dict[str, set[str]] = defaultdict(set)
    event_files: dict[str, set[str]] = defaultdict(set)
    quest_names: Counter[str] = Counter()
    expression_functions: Counter[str] = Counter()
    parse_errors: dict[str, str] = {}
    include_refs: list[dict] = []
    attach_refs: list[dict] = []

    for path in quest_files():
        rel = path.relative_to(XML_ROOT).as_posix()
        try:
            root = parse(path)
        except Exception as exc:  # report all malformed inputs together
            parse_errors[rel] = str(exc)
            continue

        for include in root.findall(".//Include"):
            ref = include.get("File", "")
            include_refs.append(
                {"source": rel, "file": ref, "exists": (XML_ROOT / ref).is_file()}
            )
        for attach in root.findall(".//AttachQuestFile"):
            ref = attach.get("File", "")
            attach_refs.append(
                {"source": rel, "file": ref, "exists": (XML_ROOT / ref).is_file()}
            )

        for quest in root.findall(".//Quest"):
            quest_names[quest.get("Name", "<unnamed>")] += 1
            for element in quest.iter():
                for value in element.attrib.values():
                    expression_functions.update(re.findall(r"\?([A-Za-z_]\w*)\[", value))
            events_node = quest.find("Events")
            if events_node is not None:
                for event in list(events_node):
                    events[event.tag] += 1
                    event_files[event.tag].add(rel)

            actions_node = quest.find("Actions")
            if actions_node is None:
                continue
            # An action can be nested in Dialog buttons, Error handlers, If/Then,
            # Switch/Case, etc. Intersecting descendants with the recovered enum
            # avoids misclassifying Line/Conditions/Then as action types.
            for element in actions_node.iter():
                if element is actions_node:
                    continue
                if element.tag in all_known_actions:
                    actions[element.tag] += 1
                    action_files[element.tag].add(rel)

    missing_old_actions = sorted(set(actions) - old_actions)
    missing_old_events = sorted(set(events) - old_events)
    unknown_actions = sorted(set(actions) - new_actions - old_actions)
    old_condition_vocabulary = csharp_cases(CS_ROOT / "QuestCondition.cs")

    return {
        "quest_file_count": len(quest_files()),
        "quest_count": sum(quest_names.values()),
        "duplicate_quest_names": sorted(k for k, v in quest_names.items() if v > 1),
        "parse_errors": parse_errors,
        "events": dict(events.most_common()),
        "actions": dict(actions.most_common()),
        "missing_old_events": missing_old_events,
        "missing_old_actions": missing_old_actions,
        "missing_actions_after_runtime_adapter": sorted(
            set(missing_old_actions) - ADAPTED_ACTIONS
        ),
        "unknown_actions": unknown_actions,
        "expression_functions": dict(expression_functions.most_common()),
        "expression_functions_missing_from_old_parser": sorted(
            set(expression_functions) - old_condition_vocabulary
        ),
        "missing_old_event_files": {
            k: sorted(event_files[k]) for k in missing_old_events
        },
        "missing_old_action_files": {
            k: sorted(action_files[k]) for k in missing_old_actions
        },
        "include_refs": include_refs,
        "attach_refs": attach_refs,
        "missing_include_refs": [x for x in include_refs if not x["exists"]],
        "missing_attach_refs": [x for x in attach_refs if not x["exists"]],
        "old_action_count": len(old_actions),
        "apk_2419_action_count": len(new_actions),
    }


def dependency_inventory() -> dict:
    list_root = parse(XML_ROOT / "list.xml")
    stages_root = parse(XML_ROOT / "stages.xml")
    compat_stages_root = parse(XML_ROOT / "compat" / "stages.xml")
    raid_stages_root = parse(XML_ROOT / "raid_stages_default.xml")
    moves_root = parse(XML_ROOT / "animations" / "moves.xml")

    models = {path.stem.lower() for path in (XML_ROOT / "models").glob("*.xml")}
    model_items: dict[str, list[str]] = defaultdict(list)
    unresolved_item_names: set[str] = set()
    unresolved_visible_items: list[str] = []
    for item in list_root.findall(".//Item"):
        model = item.get("Model")
        name = item.get("Name")
        if not model or not name:
            continue
        model_items[model.lower()].append(name)
        if model.lower() not in models:
            unresolved_item_names.add(name)
            if item.get("ShopHide", "0") != "1":
                unresolved_visible_items.append(name)

    stage_uses_unresolved: set[str] = set()
    for element in stages_root.iter():
        name = element.get("Name")
        if name in unresolved_item_names:
            stage_uses_unresolved.add(name)

    available_items = {
        item.get("Name") for item in list_root.findall("./Items/Item") if item.get("Name")
    }
    stage_item_refs = {
        element.get("Name")
        for xpath in (".//Items/Item[@Name]", ".//EquipItem[@Name]", ".//RequireItem[@Name]")
        for element in stages_root.findall(xpath)
        if element.get("Name")
    }
    missing_stage_items = sorted(stage_item_refs - available_items)
    missing_stage_items_after_adapter = sorted(
        set(missing_stage_items) - set(RUNTIME_ITEM_ALIASES)
    )

    locations = {
        element.get("Location")
        for element in stages_root.findall(".//*[@Location]")
        if element.get("Location")
    }
    missing_locations = []
    for location in sorted(locations):
        directory = XML_ROOT / "locations" / location
        if not ((directory / "params.xml").is_file() or
                (directory / f"{location}_params.xml").is_file()):
            missing_locations.append(location)

    valid_stage_refs: set[str] = set()
    for stage_document in (stages_root, compat_stages_root, raid_stages_root):
        for zone in stage_document.findall("./Zones/Zone"):
            zone_name = zone.get("Name")
            if not zone_name:
                continue
            for battle in zone.findall("./Battle"):
                battle_name = battle.get("Name")
                if not battle_name:
                    continue
                battle_ref = f"{zone_name}|{battle_name}"
                valid_stage_refs.add(battle_ref)
                for fight in battle.findall("./Fight"):
                    if fight.get("Name"):
                        valid_stage_refs.add(f"{battle_ref}|{fight.get('Name')}")

    quest_stage_refs: set[str] = set()
    for quest_path in quest_files():
        quest_root = parse(quest_path)
        for element in quest_root.iter():
            values = []
            if element.tag in {
                "ShowBattle", "HideBattle", "ToggleBattle", "SetBattleVisibility", "Fight"
            } and element.get("Name"):
                values.append(element.get("Name"))
            if element.tag == "SetMapFocus" and element.get("Battle"):
                values.append(element.get("Battle"))
            for value in values:
                value = value.rstrip("|")
                if value and "|" in value and "$" not in value and "?" not in value:
                    quest_stage_refs.add(value)
    missing_quest_stage_refs = sorted(quest_stage_refs - valid_stage_refs)

    template_names = {
        element.get("Name")
        for element in moves_root.findall("./Templates/Template")
        if element.get("Name")
    }
    template_refs = {
        value
        for element in (
            moves_root.findall("./Templates/Template") +
            moves_root.findall("./Moves/Move")
        )
        for value in element.get("Template", "").split("|")
        if value
    }
    available_binary_names = {
        path.name.lower() for path in (ROOT / "Assets").rglob("*.bytes")
    }
    binary_refs = {
        element.get("FileName")
        for element in moves_root.findall(".//*[@FileName]")
        if element.get("FileName", "").lower().endswith(".bytes")
    }

    return {
        "available_model_files": len(models),
        "referenced_models": len(model_items),
        "unresolved_models": sorted(set(model_items) - models),
        "unresolved_visible_item_count_before_runtime_quarantine": len(
            unresolved_visible_items
        ),
        "stage_references_to_items_with_unresolved_models": sorted(
            stage_uses_unresolved
        ),
        "stage_item_references": len(stage_item_refs),
        "missing_stage_items_before_runtime_adapter": missing_stage_items,
        "missing_stage_items_after_runtime_adapter": missing_stage_items_after_adapter,
        "literal_quest_stage_references": len(quest_stage_refs),
        "missing_quest_stage_references_after_runtime_adapter": missing_quest_stage_refs,
        "referenced_locations": len(locations),
        "missing_locations": missing_locations,
        "move_templates": len(template_names),
        "undefined_move_templates_before_runtime_adapter": sorted(
            template_refs - template_names
        ),
        "referenced_animation_binaries": len(binary_refs),
        "missing_animation_binaries": sorted(
            name for name in binary_refs if name.lower() not in available_binary_names
        ),
    }


def roots_inventory() -> dict[str, str]:
    result = {}
    for path in sorted(XML_ROOT.glob("*.xml")):
        try:
            result[path.name] = parse(path).tag
        except Exception as exc:
            result[path.name] = f"ERROR: {exc}"
    return result


def settings_inventory() -> dict:
    custom = parse(XML_ROOT / "internalSettings.xml")
    compat = parse(XML_ROOT / "compat" / "internalSettings.xml")
    custom_names = {x.tag for x in list(custom)}
    compat_names = {x.tag for x in list(compat)}
    required = {
        "AssemblySettings",
        "Internet",
        "Supports",
        "EULA",
        "Log",
        "ForcedLogConditions",
        "StarterPackTimer",
    }
    return {
        "custom_top_level_count": len(custom_names),
        "compat_top_level_count": len(compat_names),
        "required_missing_from_custom": sorted(required - custom_names),
        "legacy_sections_missing_from_custom": sorted(compat_names - custom_names),
        "new_sections_not_in_legacy": sorted(custom_names - compat_names),
    }


def write_markdown(report: dict) -> None:
    quests = report["quests"]
    settings = report["settings"]
    dependencies = report["dependencies"]
    lines = [
        "# Custom XML migration audit",
        "",
        "Generated from `Assets/xml`, the decompiled parser, and the recovered 2.41.9 action enum.",
        "",
        f"- Quest files scanned: {quests['quest_file_count']}",
        f"- Quests scanned: {quests['quest_count']}",
        f"- Old runtime action vocabulary: {quests['old_action_count']}",
        f"- 2.41.9 action vocabulary: {quests['apk_2419_action_count']}",
        f"- XML parse errors: {len(quests['parse_errors'])}",
        f"- Missing Include targets: {len(quests['missing_include_refs'])}",
        f"- Missing AttachQuestFile targets: {len(quests['missing_attach_refs'])}",
        "",
        "## Runtime features that must be ported",
        "",
        "Quest events: " + (", ".join(quests["missing_old_events"]) or "none"),
        "",
        "Quest actions: " + (", ".join(quests["missing_old_actions"]) or "none"),
        "",
        "Quest actions still unsupported after the runtime adapter: "
        + (", ".join(quests["missing_actions_after_runtime_adapter"]) or "none"),
        "",
        "Tags that are not known even to the recovered 2.41.9 action enum: "
        + (", ".join(quests["unknown_actions"]) or "none"),
        "",
        "## Settings bridge",
        "",
        "Required legacy sections absent from the custom file: "
        + (", ".join(settings["required_missing_from_custom"]) or "none"),
        "",
        "The loader should retain the custom settings and import only missing top-level legacy sections.",
        "",
        "## Content dependencies",
        "",
        f"- Plaintext model files available: {dependencies['available_model_files']}",
        f"- Models referenced by the custom list: {dependencies['referenced_models']}",
        f"- Optional/event models still unavailable: {len(dependencies['unresolved_models'])}",
        f"- Stage locations referenced/missing: {dependencies['referenced_locations']}/{len(dependencies['missing_locations'])}",
        f"- Animation binaries referenced/missing: {dependencies['referenced_animation_binaries']}/{len(dependencies['missing_animation_binaries'])}",
		f"- Stage item references missing after adaptation: {len(dependencies['missing_stage_items_after_runtime_adapter'])}",
		f"- Literal quest battle references missing after adaptation: {len(dependencies['missing_quest_stage_references_after_runtime_adapter'])}",
        "- Undefined move templates before adaptation: "
        + (", ".join(dependencies["undefined_move_templates_before_runtime_adapter"]) or "none"),
        "",
        "Unresolved models are quarantined from the shop at load time and owned items receive type-compatible fallbacks.",
        "",
        "## Duplicate quest names",
        "",
        ", ".join(quests["duplicate_quest_names"]) or "none",
        "",
    ]
    (OUT_ROOT / "report.md").write_text("\n".join(lines), encoding="utf-8")


def main() -> None:
    OUT_ROOT.mkdir(parents=True, exist_ok=True)
    report = {
        "roots": roots_inventory(),
        "settings": settings_inventory(),
        "quests": quest_inventory(),
        "dependencies": dependency_inventory(),
    }
    (OUT_ROOT / "report.json").write_text(
        json.dumps(report, indent=2, ensure_ascii=False), encoding="utf-8"
    )
    write_markdown(report)
    print(json.dumps(report["quests"], indent=2, ensure_ascii=False))


if __name__ == "__main__":
    main()
