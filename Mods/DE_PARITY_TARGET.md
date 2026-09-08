# Modding API acceptance target: Definitive Edition parity

Recorded 2026-09-08 from the project owner's requirements.

> **Implementation work:** before changing the Mod API for this target, read
> [DE_API_IMPLEMENTATION_PLAN.md](DE_API_IMPLEMENTATION_PLAN.md). It is the
> canonical dependency-ordered roadmap and agent execution protocol for this
> acceptance target.

The API must be extensive enough for a downstream Definitive Edition mod to
reproduce every behavioral/content change represented by `Assets/DExml/`, plus
the complete DE feature list below, without editing Eclipse source or base XML.
This is an acceptance target, not a declaration that the current API supports it.
DE is the reference implementation used to expose gaps; the public capabilities
must also be usable by other mods.

## Required coverage

| Feature | Capability the mod must control |
| --- | --- |
 |
|  |
|  |
| No wait timers | Configure purchase delivery, upgrades, enchanting, forging, and other relevant timers and skip costs independently. |
| No battle passes | Disable pass systems, gating, offers, and UI; provide alternate item/reward acquisition. |
| No premium currency or offers | Control currency purpose, visibility, acquisition and spending, and offer UI. Keeping gems as an earned currency must be supported independently of paid purchases. |
| No FOMO; permanent events | Register events and control schedules, eligibility, repeatability, and permanent availability. |
| Underworld event bosses | Register raid zones, bosses, encounters, rules, rewards, keys, and progression. |
| All cut/unused music | Supply/reference audio assets and assign playback to scenes, battles, and story sequences. |
| Sensei's Story / Old Wounds | Register complete quest chains, dialogue, battles, conditions, actions, unlocks, and rewards. |
|  |
| Ascension and Monk set | Register the mode, entry rules, loot/reward selection, and set acquisition. |
| Every weapon, armor, helm, ranged weapon, and magic | Define equipment, assets, combat behavior, enchantments, unlocks, shop visibility, purchase prices, and upgrade paths. |
| Special-offer items in the shop | Make shop availability independent of the original paid offer or event. |
| Titan's Desolator earned in Eclipse mode | Attach conditional item rewards to the appropriate boss/mode completion. |
| Sentinel, Neo-wanderer, and every other set | Define set membership, effects, assets, and acquisition paths. |
 |

Removing ad/payment SDK binaries is a base build/dependency concern. A runtime
mod can disable services and all associated gameplay/UI, but cannot physically
remove code already compiled into the executable. Keep SDK exclusion available
at the build boundary. Assets absent from the base must be supplied or recovered;
an API alone cannot recreate missing models, music, or animations.

## Data and runtime contract

- Support additions, targeted overrides, removals, and references across all DE
  XML domains, including quests, stages, equipment, perks, character progression,
  forge recipes, settings, localization, locations, and animations.
- Preserve serialized Unity identity and existing runtime lookup contracts.
  Mods need supported asset loading and replacement, alongside data definitions.
- Provide runtime hooks for behavior that data alone cannot express, including
  combat effects, mode entry, reward calculation, and UI/service policy.
- Define deterministic dependency/load order and conflict diagnostics. Report
  unsupported fields or actions instead of silently dropping DE behavior.
- Keep mod-owned progression and state saveable, with versioned migrations and
  defined behavior when a mod is disabled or missing. Disabling a content mod
  must not silently erase its saved progression.
- Eclipse owns reconstruction and reusable mechanisms. DE owns content
  availability and policy. Avoid special cases keyed to DE.
- The current September 8 economy is base-owned and must remain unchanged.
  Economy values and rules are intentionally not part of the public mod API,
  because allowing multiple mods to redefine the shared economy would create
  ambiguous ordering and conflict behavior.

## Evidence required before claiming parity

1. Inventory semantic differences between the archived vanilla reference and DE
   XML, ignoring formatting. Distinguish intentional DE changes from version
   differences and reconstruction repairs; record unresolved differences.
2. Map every relevant difference and every feature above to a public capability,
   a runnable DE mod example, and a verification case. Unmapped changes are gaps.
3. Reproduce the DE profile through the mod using an unchanged base installation.
   An unrestricted file replacement facility alone is insufficient evidence:
   referenced schemas, assets, conditions, and actions must actually work.
4. Validate full story progression through intermission and Titan, restored
   modes, item acquisition/upgrading, and save/reload behavior in Unity.
5. Verify the base profile separately with the DE mod disabled and check defined
   conflict behavior with another mod changing an overlapping definition.

The economy changes directly applied to `Assets/vanillaXml/` during September 8
are explicitly requested changes. Preserve them as the canonical Eclipse economy.
They are the DE balancing, and will be the only real part of it made global.
Mods must not be able to override or replace this economy.

Current examples and API documentation live in [README.md](README.md). The
existing equipment, behavior, and save support is a foundation, not proof of
complete DE coverage. This document does not request an immediate wholesale
implementation or a rollback of working changes.
