"""Reproduce the P3 semantic configuration inventory; --write updates its ledger."""
import argparse
import hashlib
import json
from pathlib import Path
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]
LEDGER = ROOT / "Mods/PHASE3_CONFIGURATION_AUDIT.json"


def canonical(node):
    return [node.tag, sorted(node.attrib.items()), (node.text or "").strip(),
            [canonical(child) for child in node]]


def digest(value):
    return hashlib.sha256(json.dumps(value, sort_keys=True).encode()).hexdigest()


INTERNAL = {
    "AchievementCounter": ("progression_content", "P3 owned counters and Lua callbacks; preserve shipped native counter semantics."),
    "AntiCheatSettings": ("compatibility", "Archived item exception list; do not expose anti-cheat internals or delete base validation."),
    "Benchmark": ("platform_configuration", "Benchmark cycles belong to device performance setup."),
    "Camera": ("presentation_content", "Camera width is a rendering parameter; no recovered mod-safe global camera contract yet."),
    "CounterPunches": ("combat_compatibility", "Native combat counter tuning; do not infer a universal mod policy from the DE value."),
    "DefaultServerResponseTimeout": ("platform_configuration", "Service transport timeout; P2 service opt-outs replace timeout tricks."),
    "GUI": ("presentation_content", "Hint timeout and credits scroll speed. No active external consumers of these BasicGUI getters were found; no nominal API."),
    "Hints": ("presentation_content", "Archived raid advice changes; use localized mod fight descriptions/dialogue."),
    "HitEffects": ("combat_compatibility", "Native hit pause timing; keep recovered timing authority rather than globally overwriting it."),
    "LotteryRerollPrices": ("private_economy", "Reroll cost schedule remains base-owned."),
    "PushRetantionTime": ("platform_configuration", "Notification retention belongs to platform integration."),
    "QualityOptions": ("platform_configuration", "Device quality flags are not gameplay mod capabilities."),
    "Resistances": ("combat_compatibility", "Removed legacy resistance descriptor is not evidence that the base mechanic should be removed."),
    "SlowMode": ("combat_compatibility", "Native slow-motion timing remains reconstruction-owned."),
    "VideoAdCounterReset": ("existing_p2_policy", "Use rewarded_video/ads service opt-out; do not change ad counter internals."),
}
BUILD = {
    "CheckNetworkLoginTimeoutSec": "platform_configuration", "ConfigResponseTimeout": "platform_configuration",
    "DebugStatistics": "build_only", "GPG": "platform_configuration", "HideLanguageFlags": "presentation_content",
    "Internet": "platform_configuration", "OpenAltUrlTimeout": "platform_configuration",
    "ServerResponseTimeout": "platform_configuration", "ShowIntro": "presentation_content",
    "SocialAuthorizeTimeout": "platform_configuration", "TacticsCaching": "platform_configuration",
}


def inventory():
    rows = []
    for filename in ("internalSettings.xml", "devices.xml", "BuildSettings.json", "loaderScreenSettings.json"):
        paths = [ROOT / "Assets" / tree / filename for tree in ("vanillaXml", "DExml")]
        if filename.endswith("xml"):
            groups = []
            for path in paths:
                group = {}
                for child in ET.parse(path).getroot():
                    group.setdefault(child.tag, []).append(canonical(child))
                groups.append(group)
        else:
            groups = [json.loads(p.read_text(encoding="utf-8-sig")) for p in paths]
        a, b = groups
        for key in sorted(a.keys() | b.keys()):
            if a.get(key) == b.get(key):
                continue
            if filename == "internalSettings.xml":
                category, decision = INTERNAL[key]  # New sections require review.
            elif filename == "devices.xml":
                category, decision = "platform_configuration", "Archived device/quality heuristics; retain reconstruction platform selection."
            elif filename == "BuildSettings.json":
                category = BUILD[key]
                decision = "Build/platform/presentation configuration, not a gameplay XML patch. P2 disables supported services semantically."
            else:
                category, decision = "presentation_content", "Regional loader/preloader branding. Boot-time assets load before gameplay mods; no unsupported hot replacement promise."
            rows.append(dict(file=filename, section=key, classification=category, decision=decision,
                             vanilla_sha256=digest(a.get(key)), archived_de_sha256=digest(b.get(key))))
    names = sorted({p.name for tree in ("vanillaXml", "DExml") for p in (ROOT / "Assets" / tree / "credits").glob("*.xml")})
    for name in names:
        values = []
        for tree in ("vanillaXml", "DExml"):
            p = ROOT / "Assets" / tree / "credits" / name
            values.append(canonical(ET.parse(p).getroot()) if p.exists() else None)
        if values[0] != values[1]:
            rows.append(dict(file="credits/" + name, section="document", classification="presentation_content",
                             decision="Credit text/localization and attribution; preserve base credits. No platform or gameplay API inferred.",
                             vanilla_sha256=digest(values[0]), archived_de_sha256=digest(values[1])))
    return dict(schema=1, scope="Semantic differences, ignoring XML indentation; archived DE is evidence, not base authority.", entries=rows)


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--write", action="store_true")
    args = parser.parse_args()
    result = inventory()
    if args.write:
        LEDGER.write_text(json.dumps(result, indent=2) + "\n", encoding="utf-8")
    elif json.loads(LEDGER.read_text(encoding="utf-8")) != result:
        raise SystemExit("P3 configuration sources changed: review classifications and regenerate the ledger.")
    print(f"PASS: {len(result['entries'])} classified configuration/credits differences; no unclassified internal/build settings.")
