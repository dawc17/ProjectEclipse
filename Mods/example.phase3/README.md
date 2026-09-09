# Phase 3 integrated showcase

## Recorded user playtest

User playtest confirmed the showcase flow, persistence across restart, no
progress on loss or surrender, and the sword replacement icon. The user stated
that point 5 needs no further confirmation. Mod removal/reinstallation remains
covered by automated checks; it was not separately reported in this playtest.

## Test steps

This folder is discovered automatically alongside your existing samples. Restart Play mode. It adds no
map zone; use **Phase 2 Trials → Open Training**, or any other repeatable fight.

1. Open **Profile → Achievements**. Find **P3 First Record**, initially incomplete.
   The native list shows the next milestone; the three-win entry appears after
   the first is earned.
2. Forge **P3 Archivist** onto a weapon and equip that weapon. With Phase 2 enabled,
   its existing instant-forge policy still applies. Read the Archivist description.
3. Win one fight. The console should log `Archivist victories: 1`. Profile should
   show **P3 First Record** earned and **P3 Three Records** at **1/3**.
4. Restart Play mode. The first achievement and **1/3** progress should remain.
5. Win two more fights with Archivist equipped. **P3 Three Records** should be earned.
   Further victories stay replayable and do not duplicate achievements.
6. Lose or surrender a fight. The counter should not advance. Winning without
   Archivist equipped should not advance it either. Archivist has no combat buff.
7. Inspect the achievement/Archivist icon: it uses the familiar sword marker.
   This deliberately replaces the core Frenzy sprite identity, so other content
   loading that same identity also changes. It does not change other enchantment
   descriptions or unrelated icons.
8. Return to Title through the main Menu, open Mods, disable `example.phase3`, and
   Apply & Restart. Enter Campaign: its achievements disappear and the original
   Frenzy sprite returns. Re-enable it through Mods and apply again: earned progress should return.

There is no achievement gem reward or separate unlock popup in this sample. Check
the Profile list for completion. The sword marker reuses Phase 1's existing PNG;
it is intentionally a conspicuous replacement test, not finished achievement art.

Counters and unlocks use native profile persistence; custom win filtering is a Lua
handler. See [P3_API.md](../P3_API.md) for capabilities, bounds and replacement rules.
