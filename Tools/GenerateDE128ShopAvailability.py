"""Project DE shop visibility and prices onto existing core equipment.

The two list.xml files are build-time evidence only. DE128 loads the generated Lua
table and never parses archived gameplay XML at runtime. Archived prices, icons,
models and starting progression use typed catalog patches. Reviewed combat-family
differences are resolved by DE128 move/subtype modules.
"""

from __future__ import annotations

import argparse
import json
import sys
import xml.etree.ElementTree as ET
from pathlib import Path


ROOT = Path(__file__).resolve().parent.parent
TARGET = ROOT / "Mods/de128/scripts/content/shop_data.lua"
CATEGORIES = {"Weapon": "weapon", "Armor": "armor", "Helm": "helm", "Ranged": "ranged", "Magic": "magic"}
STATS = {
    "weapon": (("WeaponDamage", "weapon_damage"),),
    "armor": (("BodyDefense", "body_defense"), ("HeadDefense", "head_defense"),
              ("UnarmedDamage", "unarmed_damage")),
    "helm": (("HeadDefense", "head_defense"),),
    "ranged": (("RangedDamage", "ranged_damage"), ("WeaponDamage", "weapon_damage")),
    "magic": (("MagicDamage", "magic_damage"),),
}
SUPPORTED_SUBTYPE_PATCHES = {
    "RANGED_BP_S3_WIND_MAKER",  # combat_equipment.lua: Kunai
    "WEAPON_CHNY22_SPEAR",       # combat_equipment.lua: Naginata
    "WEAPON_CHNY21_JIAN",        # chinese_swords.lua: ChineseSwords
    "WEAPON_RAID_KARCER_SET",    # combat_equipment.lua: HunterClaws
    "WEAPON_BG_YARI",           # combat_equipment.lua: MagariYari
}
SUPPORTED_TACTIC_RETENTION = {"WEAPON_NY2024_MUSKET"}
PROFILE_ATTRIBUTES = {"ShopHide", "Level", "UpgradeLevel", "PackLabel", "PaidItem", "SubType", "BonusPrice", "Price", "Image", "Model",
                      *(field for fields in STATS.values() for field, _ in fields)}


def items(path: Path) -> dict[str, ET.Element]:
    result = {}
    for node in ET.parse(path).getroot().findall("./Items/Item"):
        if node.get("Type") in CATEGORIES:
            name = node.get("Name")
            # The recovered list repeats a few projectile-only names. Keep the
            # first native definition, matching the item's ordinary lookup.
            result.setdefault(name, node)
    return result


def upgrade_templates(path: Path) -> dict[str, ET.Element]:
    return {node.get("Name"): node for node in ET.parse(path).getroot().find("UpgradeList")}


def signature(node: ET.Element) -> tuple:
    return node.tag, tuple(sorted(node.attrib.items())), tuple(signature(child) for child in node)


def rows() -> list[tuple]:
    base = items(ROOT / "Assets/vanillaXml/list.xml")
    de = items(ROOT / "Assets/DExml/list.xml")
    base_templates = upgrade_templates(ROOT / "Assets/vanillaXml/list.xml")
    de_templates = upgrade_templates(ROOT / "Assets/DExml/list.xml")
    core_perks = {node.get("Name") for node in ET.parse(ROOT / "Assets/vanillaXml/perks.xml").getroot().iter("Perk")}
    art_catalog = json.loads((ROOT / "Assets/Resources/SF2Content/Art/catalog.json").read_text(encoding="utf-8"))
    art_addresses = {asset["address"].lower() for bundle in art_catalog["bundles"]
                     if (ROOT / "Assets/StreamingAssets/SF2Content/ArtBundles" / bundle["file"]).is_file()
                     for asset in bundle["assets"]}
    def has_icon(name: str | None) -> bool:
        if not name:
            return False
        address = "ui/items/" + name.lower()
        return address in art_addresses or address.split(".")[0] in art_addresses
    output = []
    hidden_to_visible = 0
    template_changes = 0
    enchantment_changes = 0
    enchantment_entries = 0
    changed_enchantment_entries = 0
    paid_marker_changes = 0
    tactic_retentions = 0
    price_changes = 0
    icon_changes = 0
    model_changes = 0
    local_upgrade_clears = 0
    for name, target in de.items():
        source = base.get(name)
        if source is None or source.get("ShopHide") != "1" or target.get("ShopHide") == "1":
            continue
        hidden_to_visible += 1
        if source.get("Type") != target.get("Type"):
            continue
        source_price = (int(source.get("Price") or 0), int(source.get("BonusPrice") or 0))
        target_price = (int(target.get("Price") or 0), int(target.get("BonusPrice") or 0))
        if any(amount < 0 or amount > 2_147_483_647 for amount in target_price) or not any(target_price):
            raise ValueError(f"Invalid DE shop price for {name}: {target_price}")
        price_changed = source_price != target_price
        if price_changed and target_price[1] == 0:
            raise ValueError(f"Unreviewed coin-only DE price for {name}")
        if source.get("SubType") != target.get("SubType") and name not in SUPPORTED_SUBTYPE_PATCHES:
            continue
        price_changes += price_changed
        icon = target.get("Image") if source.get("Image") != target.get("Image") else None
        model = target.get("Model") if source.get("Model") != target.get("Model") else None
        if icon and not has_icon(icon):
            raise ValueError(f"Missing packaged core icon for {name}: {icon}")
        if model and "gamedata/models/" + model.lower() not in art_addresses:
            raise ValueError(f"Missing packaged core model for {name}: {model}")
        icon_changes += icon is not None
        model_changes += model is not None
        level = int(target.get("Level") or 0)
        if not 1 <= level <= 52:
            raise ValueError(f"Invalid DE shop level for {name}: {level}")
        upgrade_level = int(target.get("UpgradeLevel") or -1)
        if upgrade_level != level * 100:
            raise ValueError(f"Unexpected DE shop upgrade level for {name}: {upgrade_level}")
        category = CATEGORIES[target.get("Type")]
        upgrades = target.find("Upgrades")
        template = upgrades.get("Template") if upgrades is not None else None
        if upgrades is None or len(upgrades) != 0 or template != target.get("Type") + "_Bonus":
            raise ValueError(f"Unexpected DE upgrade template for {name}: {template}")
        source_upgrades = source.find("Upgrades")
        source_template = source_upgrades.get("Template") if source_upgrades is not None else None
        if source_template not in (None, template, "Paid_" + template) or (source_upgrades is not None and len(source_upgrades) != 0 and name != "HELM_STARTER_PACK"):
            raise ValueError(f"Unexpected canonical upgrade template for {name}: {source_template}")
        clear_local_upgrades = source_upgrades is not None and len(source_upgrades) != 0
        if clear_local_upgrades:
            expected_local = {"HeadDefense": "22", "Level": "2", "UpgradeLevel": "230",
                              "DeliveryTime": "60", "BonusDeliveryPrice": "1"}
            if (name != "HELM_STARTER_PACK" or len(source_upgrades) != 1 or
                    source_upgrades[0].tag != "Upgrade" or source_upgrades[0].attrib != expected_local or
                    len(source_upgrades[0]) != 0):
                raise ValueError(f"Unreviewed local upgrade rows for {name}")
            local_upgrade_clears += 1
        template_changes += source_template != template
        if template not in base_templates or template not in de_templates or signature(base_templates[template]) != signature(de_templates[template]):
            raise ValueError(f"DE upgrade template differs from the installed core table: {template}")
        stats = tuple((field, int(target.get(xml_name))) for xml_name, field in STATS[category]
                      if target.get(xml_name) is not None)
        if not stats or any(value < 0 or value > 1_000_000 for _, value in stats):
            raise ValueError(f"Invalid DE initial stats for {name}")
        archived_enchantments = target.find("Enchantments")
        enchantments = []
        seen_perks = set()
        for perk in archived_enchantments if archived_enchantments is not None else ():
            set_node = perk.find("Set")
            perk_name = perk.get("Name")
            if (perk.tag != "Perk" or set(perk.attrib) != {"Name"} or perk_name not in core_perks or
                    perk_name in seen_perks or len(perk) != 1 or set_node is None or
                    set(set_node.attrib) != {"Aspect"} or len(set_node) != 0):
                raise ValueError(f"Unsupported DE default enchantment for {name}: {perk_name}")
            aspect = int(set_node.get("Aspect"))
            if not 0 <= aspect <= 1_000_000:
                raise ValueError(f"Invalid DE enchantment aspect for {name}: {aspect}")
            seen_perks.add(perk_name)
            enchantments.append((perk_name, aspect))
        enchantment_entries += len(enchantments)
        source_enchantments = source.find("Enchantments")
        changed_enchantments = (signature(source_enchantments) if source_enchantments is not None else None) != (signature(archived_enchantments) if archived_enchantments is not None else None)
        enchantment_changes += changed_enchantments
        if changed_enchantments:
            changed_enchantment_entries += len(enchantments)
        if target.get("PaidItem") is not None or source.get("PaidItem") not in (None, "Paid", "SuperPaid"):
            raise ValueError(f"Unexpected legacy paid marker for {name}")
        paid_marker = "none" if source.get("PaidItem") is not None else None
        paid_marker_changes += paid_marker is not None

        # Every archive difference in the selected cohort must have an explicit
        # implementation or a reviewed compatibility exception. The Musket's
        # canonical Rifle AI tactic is intentionally retained by DE128.
        changed_attributes = {field for field in set(source.attrib) | set(target.attrib)
                              if source.get(field) != target.get(field)}
        tactic_changed = source.get("TacticSubtype") != target.get("TacticSubtype")
        if tactic_changed:
            if (name not in SUPPORTED_TACTIC_RETENTION or source.get("TacticSubtype") != "Rifle" or
                    target.get("TacticSubtype") is not None):
                raise ValueError(f"Unreviewed DE tactic subtype difference for {name}")
            changed_attributes.remove("TacticSubtype")
            tactic_retentions += 1
        if changed_attributes - PROFILE_ATTRIBUTES:
            raise ValueError(f"Unimplemented DE shop attributes for {name}: {sorted(changed_attributes - PROFILE_ATTRIBUTES)}")
        changed_children = {tag for tag in {node.tag for node in source} | {node.tag for node in target}
                            if ((signature(source.find(tag)) if source.find(tag) is not None else None) !=
                                (signature(target.find(tag)) if target.find(tag) is not None else None))}
        if changed_children - {"Upgrades", "Enchantments"}:
            raise ValueError(f"Unimplemented DE shop children for {name}: {sorted(changed_children - {'Upgrades', 'Enchantments'})}")
        if not has_icon(target.get("Image")):
            raise ValueError(f"Missing packaged DE shop icon for {name}: {target.get('Image')}")
        output.append((category, name, level, target.get("PackLabel") or "", upgrade_level,
                       template, stats, tuple(enchantments) if changed_enchantments else None, paid_marker,
                       target_price[1] if price_changed else None, clear_local_upgrades,
                       target_price[0] if price_changed else None, icon, model))
    output.sort(key=lambda row: (row[0], row[1]))
    if (hidden_to_visible != 221 or len(output) != 221 or price_changes != 99 or icon_changes != 20 or model_changes != 3 or
            sum("_BP_S" in row[1] for row in output) != 25 or template_changes != 150 or
            enchantment_changes != 217 or enchantment_entries != 262 or changed_enchantment_entries != 258):
        raise ValueError(f"DE shop cohort changed: {hidden_to_visible} candidates / {len(output)} rows / {price_changes} price changes / {icon_changes} icons / {model_changes} models / {template_changes} template changes / {enchantment_changes} changed enchantments / {changed_enchantment_entries} patched entries / {enchantment_entries} archive entries; review the selection")
    if paid_marker_changes != 129 or tactic_retentions != 1 or local_upgrade_clears != 1:
        raise ValueError(f"DE shop marker/tactic/upgrade cohort changed: {paid_marker_changes} markers / {tactic_retentions} retained tactics / {local_upgrade_clears} cleared local upgrades; review the selection")
    return output


def render(selected: list[tuple]) -> str:
    lines = [
        "-- Generated by Tools/GenerateDE128ShopAvailability.py from archived list.xml.",
        "-- This is static runtime data; DE128 never loads the source XML in game.",
        "return {",
    ]
    for category, name, level, group, upgrade_level, template, stats, enchantments, paid_marker, gem_price, clear_local, coin_price, icon, model in selected:
        stat_text = ", ".join(f"{field} = {value}" for field, value in stats)
        enchantment_text = "{ " + ", ".join(f'{{ "{perk}", {aspect} }}' for perk, aspect in enchantments) + " }" if enchantments is not None else "nil"
        paid_text = f'"{paid_marker}"' if paid_marker else "nil"
        price_text = str(gem_price) if gem_price is not None else "nil"
        clear_text = "true" if clear_local else "nil"
        coin_text = str(coin_price) if coin_price is not None else "nil"
        icon_text = f'"{icon}"' if icon else "nil"
        model_text = f'"{model}"' if model else "nil"
        lines.append(f'    {{ "{category}", "{name}", {level}, "{group}", {upgrade_level}, "{template}", {{ {stat_text} }}, {enchantment_text}, {paid_text}, {price_text}, {clear_text}, {coin_text}, {icon_text}, {model_text} }},')
    lines.append("}")
    return "\n".join(lines) + "\n"


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--check", action="store_true")
    args = parser.parse_args()
    selected = rows()
    content = render(selected)
    if args.check:
        if not TARGET.is_file() or TARGET.read_text(encoding="utf-8") != content:
            print("FAIL: generated shop availability differs from the archive")
            return 1
        print(f"Verified {len(selected)} priced DE shop entries")
    else:
        TARGET.write_text(content, encoding="utf-8", newline="\n")
        print(f"Generated {len(selected)} priced DE shop entries")
    return 0


if __name__ == "__main__":
    sys.exit(main())
