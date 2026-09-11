# Perk upgrade implementation evidence

Initially inspected against API 0.10 on 2026-09-12. API 0.11 now implements the
contract below; see PRE_DE_WORK_LOG.md for managed/native-source verification.
Full Unity acceptance remains part of the active pre-DE objective.

## Actual content and engine paths

- Canonical `CharacterProgress.xml` contains 18 perk records with 160
  `UpgradeLevel` children. The archived DE file contains 20 records with 170
  children. The two extra records are Master of Style and Relentless, five levels
  each. Master of Style varies `Drain` from 0.02 to 0.10; Relentless varies
  `DamagePerStack` from 0.01 to 0.05 and retains `MaxStacks = 15`.
- `PerkItems.NLLMCPOPFCI` reads progression perk definitions, clones base perks
  with `Set`/`RatingEvaluation`, and adds each upgrade to `PAABAIILNEG`.
  `HDIPMKIGKDA` copies the upgrade's description and Value into its clone.
  `LAAJJBEEDKL(name, upgradeLevel)` performs exact level lookup. The parent
  Description can override child descriptions through `PJIBJPOKIOC`; an external
  projection must explicitly define description precedence rather than assume it.
- `PerkTree` holds the progression list from `GFPFNILGJML`, and falls back to
  `ABAGJKMKCBA` for base perks. Existing external base-perk registration and branch
  overlays do not install progression upgrade variants.
- `RosterPerk` persists separate `Level` and `UpgradeLevel` attributes. Upgrade
  selection must use `UpgradeLevel`, not player level or the branch's unlock level.
- `UserPerks` calls `ModRuntime.TryInitializeSavedPerkParameters`, which writes
  initial behavior parameters once. `TryReadSavedPerk` subsequently reads that
  instance record. Simply changing a definition or adding native clones cannot
  update the parameters used by learned Lua callbacks.
- `Fight` invokes learned behaviors through `TryInvokeSavedPerkFightBegin` for
  all supported events. That is the shared resolution point; do not implement
  separate upgrade logic in every event callback.

## Planned complete contract

Extend owned perk registration with typed upgrade entries. Each entry identifies
an upgrade level, optional localized description, and a parameter override map.
Keep static data declarative and actual perk logic in ordinary Lua callbacks.
Support template-backed parameters and scripted schema-typed parameters through
their existing distinct validation paths. No changes to global XP, currency,
level scaling, or upgrade costs belong in this contract.

Define base/unlocked level explicitly after verifying `UserPerks` selection and
upgrade mutation. Require unique, bounded, contiguous upgrade levels and reject
invalid parameter names/types/localization dependencies. Preserve immutable
definitions and include all level payloads in content fingerprints. Avoid
implicit cumulative inheritance between levels: each level overrides the base
parameter map, so authoring level 3 does not depend on level 2's incidental fields.

Install external progression variants through an owned PerkItems API that tracks
what it added and removes only those variants on adapter teardown. Do not call
the native whole-list parser, which clears vanilla progression. Preserve both
the base definition and native clone identity. A missing mod must not erase saved
learned perks, upgrade level, or behavior state.

For Lua, resolve a detached effective parameter map at callback time from the
validated saved instance and selected upgrade overrides. Do not overwrite saved
instance parameters merely to apply a level overlay. Keep persisted behavior
state independent from the parameter overlay. Define unknown future upgrade
levels and malformed saved levels conservatively: diagnose and preserve data,
never silently reinterpret them as an unrelated valid level.

Document scope for learned perks versus warrior/equipment hosts. Those other
hosts must not acquire a learned level accidentally; they continue at the base
parameter set unless a separately supported explicit level contract applies.

## Required proof before calling G07 delivered

1. Real Lua registration accepts all ten archived added-perk upgrade payloads and
   rejects duplicate/gapped levels, bad schemas and undeclared descriptions.
2. Production native progression projection exposes distinct descriptions and
   parameter values for base and upgraded variants without clearing vanilla rows.
3. Learned Lua callbacks actually receive the selected parameters; changing the
   saved upgrade level changes subsequent resolution, not historical snapshots.
4. Save/reload, disable/re-enable, invalid/future levels and behavior-state
   persistence preserve data; upgrade resolution does not rewrite saved rolls.
5. Registration rollback, content fingerprints and teardown restore behavior.
6. Public wiki, LuaLS fields, generated definitions, templates and a playable
   generic demonstration are updated together. Use core assets while DE art waits.
7. Managed builds and focused/native-source fixtures pass; a full Unity upgrade
   selection/profile/combat playtest is explicitly distinguished from those checks.

Completing G07 does not implement Master of Style/Relentless themselves: style
events, outgoing damage changes, combo/timed-stack behavior remain G06 work.
