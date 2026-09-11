# Pre-DE engine expansion work log

Objective: implement as much of the general mod-engine roadmap and the known
DE-port capability gaps as possible before collaborator assets arrive. Keep DE
content/policy downstream. Report all delivered changes, tests, and limitations.
This objective remains active; a passing slice is not completion of the roadmap.

Authoritative requirements are the domain acceptance rules in
[the implementation plan](DE_API_IMPLEMENTATION_PLAN.md),
[the DE parity target](DE_PARITY_TARGET.md),
[G01–G14](DE_XML_API_GAP_AUDIT.md), and
[the engine extensibility review](MOD_ENGINE_EXTENSIBILITY.md).

## Delivered before this goal continuation

- API 0.8: `sf2.rules.behavior`, direct fight attachment, parameter validation,
  rule/side state isolation, target/mode/round filters, transient lifecycle,
  deterministic callback ordering, content fingerprints, and Third Strike Trial.
- API 0.9: `fighter:snapshot()` with detached health/max-health/bar count,
  position, opponent and active-fight clock observations; expired query guards;
  a health-dependent example; typed editor returns and generated API-version sync.
- Both include public guides/reference, templates, real Lua tests and editor checks.
- Earlier map/profile/forge/video presentation fixes in this worktree belong to
  the preceding UI request, not this mod-engine expansion.

## API 0.10: existing encounter patches

Changes made on 2026-09-12:

1. Extended `sf2.fights.patch` with `rules`, `append_rules`, `location`, and `music`.
   `rules` replaces all encounter rules (empty means clear); `append_rules`
   preserves native rules and adds registered rules in order. Both static and Lua
   rules are supported, including handles created earlier in the entrypoint.
2. Added typed transaction methods and centralized semantic policy entries.
   Rule lists reject duplicate/missing/undeclared handles, invalid arrays, empty
   appends, and lists over 100 entries. Replace/append are mutually exclusive and
   conflict on the same semantic field; independent fields can coexist.
3. Preserved encounter identity, opponents, rewards and saved progress. Definition
   copies preserve patch intent through subsequent field edits. Fingerprints
   distinguish append and replacement, including empty replacement.
4. Connected patches to the recovered adapter. Core source XML is cloned and
   patched, retaining the existing restoration path. Lua rules are dispatched
   through the existing fight behavior host and never emitted as native XML rules.
5. Extracted the core fight-field projection into a shared runtime helper used by
   the adapter and managed fixtures, so preservation and replacement are tested
   against the same projection implementation.
6. Added `Mods/example.core-fight` and the editor's matching manual template. It
   appends a health-dependent guard to the first Lynx bodyguard and uses existing
   dojo/music assets. It leaves campaign unlocks and progress alone.
7. Updated public fight/rule documentation, examples, editor schema/completion,
   generated definitions and metadata, and historical gap-audit follow-up notes.
8. Fixed the earlier behavior-rule parameter copy to compile under the legacy
   .NET Framework contract harness as well as Unity/.NET 10.

Verification for API 0.10:

- All four managed builds passed.
- `Tools/TestFightPatches.ps1`: 60 checks passed, including canonical stage import,
  Lua validation, atomic conflict rollback, stable identity and untouched encounters,
  core Lua rule dispatch selection, append/replace/clear XML projection, preservation
  of the original restoration source, and content fingerprints.
- `Tools/TestBattleRules.ps1`: 104 checks passed.
- `Tools/TestModdingContracts.ps1`: foundation and core/save contracts passed.
- `Tools/TestP2ACombatRuntime.ps1`: existing combat/state/mode regressions passed.
- Editor generation/check, eight project tests, LuaLS and isolated VS Code passed.
- Wiki: 41 pages, 98 binding sections, 3,059 local links/assets checked.
- `git diff --check` passed.

The projection fixture uses the production projection helper. Static rule XML
construction is delegated to the existing recovered adapter; its integration is
compile-checked, not a native Unity encounter test. Native asset appearance,
full encounter flow and restoration after an actual game restart require a Unity
playtest; managed fixtures do not establish those results.

## API 0.11 in progress: perk upgrades

- Added immutable upgrade entries with contiguous level validation, typed/native
  parameter checks, localized descriptions and content fingerprint coverage.
- Extended `sf2.perks.register` with `upgrades` and updated the wiki/editor schema.
- Added native external progression variants and owned removal through PerkItems.
- Learned Lua callbacks overlay the saved UpgradeLevel onto a detached parameter
  map; saved rolls and behavior state are not overwritten.
- Three runtime managed builds and editor generation/check/eight project tests
  pass. The battle-rule fixture now passes 116 checks, including public upgrade
  registration, effective level parameters and preserved saved values.

Follow-up verification and additions:

- Moved saved-level resolution into the shared production runtime method used by
  the dispatcher. Perks with no upgrade table retain their historical behavior.
- `Tools/TestPerkUpgradeNative.ps1` passes 49 checks using production PerkItems and
  the actual extracted PerkInfoItem.Clone method. It verifies all ten archived
  added-perk payloads, descriptions, base isolation, duplicates and owned removal.
  The unrelated native parser/presentation is stubbed; this is not a Unity test.
- `Tools/TestBattleRules.ps1` passes 143 checks, now covering malformed/future saved
  levels, reload, and preservation of XML and saved parameters.
- Added `example.perk-upgrades` and its matching manual editor template, reusing
  the existing showcase icon. It offers a guard at level 2 and upgrades at 3–5.
- `Tools/TestPerkUpgrades.ps1` passes 39 checks: real example entrypoint,
  four levels of actual Lua damage callbacks, reload/re-enable, saved roll
  preservation, and invalid registration rollback.
- Editor project tests now include the new example, and LuaLS verifies upgrade
  field completion. Public reference and example index are updated.

Still required for full G07 acceptance: Unity profile/selection/combat playtest.
The production dispatcher calls the tested resolver, but the complete Unity
save-loading and encounter flow is compile-checked rather than playtested.
Style/combo/offensive effects required by the DE perks remain separate G06 work.

## API 0.12: outgoing hit control

- Added `on_damage_dealing` for the attacker after native hit/critical/block
  calculation and before invulnerability, shields, incoming Lua modifiers and
  health application. Existing behavior hosts and rule filters dispatch it.
- Added `fighter:scale_outgoing_damage`, scoped to that callback and requiring
  `combat.modify_outgoing_hit`. Multipliers are finite 0..16, results must fit
  nonnegative single precision, and invalid calls do not change pending damage.
  Successful calls compose in order; methods expire after callback return/error.
- Pending outgoing and incoming events now include native blocked/critical flags.
- Added `example.outgoing-rule` and manual editor template: every third unblocked
  player hit receives 2x scaling in the first Act I tournament fight. Round state
  resets and native defensive rules remain in force.
- Added public callback/method/capability documentation, example indexing,
  OutgoingFighter inference and generated editor metadata.
- Battle-rule tests pass 189 checks, including actual outgoing Lua operations,
  capability denial, expiration, composition, zero damage and overflow rejection.
  A source-order contract checks the placement before defensive stages.
- Fight-patch tests pass 75 checks, now executing the full outgoing example's
  callbacks with blocked hits and round changes against its registered behavior.
- All four builds, existing P2 combat/mode regression, 1,282 Underworld assertions,
  ten editor project tests, LuaLS and isolated VS Code pass. Wiki builds 41 pages,
  100 binding sections and checks 3,070 local links/assets. The Underworld asset
  audit still reports missing loose raid images.

Native strike delivery/visuals and the complete example still need a Unity
encounter playtest. G06 remains open for style/combo hooks, simulation ticks,
statuses, action requests and explicit outcome authority. No economy policy or
DE-specific native action was added.

## Requirements still open

G07 source investigation is recorded in
[PERK_UPGRADE_IMPLEMENTATION.md](PERK_UPGRADE_IMPLEMENTATION.md). It identifies
the separate native progression and saved Lua-parameter paths that must both be
implemented. The canonical/archive counts were verified as 160/170 upgrade
records; no upgrade capability is claimed from this investigation alone.

| Requirement | Current evidence and remaining work |
| --- | --- |
| G01: targeted core modifications/removal | Fight rule/presentation slice implemented. Opponents/rewards, quests, moves, equipment/perks and forge collections remain. |
| G02: programmable story | Existing compatibility actions remain; general event subscriptions, queries and typed asynchronous operations are not implemented. |
| G03: dojo/custom menus | Static locations exist; persistent dojo selector and owned interactive UI remain. |
| G04: contextual item grants | Existing rewards/grants are partial; inspect complete archived enchanted chest payload and implement missing instance fields. |
| G05: activated set abilities | Set membership exists; activation, cooldowns, input and presentation need runtime contracts. |
| G06: combat control | Rules, snapshots and outgoing scaling exist; style/combo hooks, statuses, tick/outcome authority remain. |
| G07: level-specific perk parameters | API 0.11 supplies native variants and learned Lua overlays; managed/native-source checks pass, Unity acceptance remains. |
| G08: moves/input/projectiles | Native binary/template foundation exists; full authoring pipeline and supported procedural operations remain. |
| G09: AI reactions | Native tactics exist; conditional decisions/programmable intent remain. |
| G10: animated scenery/music | Fight music/location patching exists; animated layers and playlist semantics still need implementation/evidence. |
| G11: item metadata/default effects | Registration is partial; supported non-economic patches and innate loadouts remain. |
| G12: forge candidates | Owned families exist; targeted core candidate editing remains, with costs base-owned. |
| G13: achievement predicates | Counters/core localization exist; event/query-driven predicates remain. |
| G14: service/boot/presentation | Named gates exist; targeted quest suppression and intent classification remain. |
| E2/E3/E4 shared runtime lifetimes | Snapshot query expiry is implemented; shared subscriptions, cancellation, clocked work and authority for modes/UI remain. |
| E3 programmable modes | Fixed sequences exist; branching persistent runs and settlement proof remain. |
| E4 custom UI | Layout/widgets, menus/modals/HUDs, live updates, input/focus and teardown remain. |
| E5 character/animation pipeline | Custom controller/identity, moves, rigs, authored import/export validation remain. |
| E6 world/presentation | Dynamic hazards, audio/effects instances and camera operations remain. |
| E7 composition | Existing ownership/conflicts persist; public service exports and more extension points remain. |
| E8 tooling/stabilization | Typed editor/wiki continue; in-game diagnostics, safe reload, tracing and creator acceptance remain. |
| DE conversion/P5 | Production port, missing collaborator assets, record-level intent decisions and full gameplay matrix remain pending. |

## Completion rule

Do not mark this objective complete merely because the latest API version builds.
Revisit every open requirement against current source, runtime consumption, save
semantics, documentation/tooling and representative gameplay evidence. Missing
assets may defer affected content, but do not block unrelated engine work.

API 0.11 follow-up final checks: all four managed builds, foundation/core-save contracts, existing P2 combat/mode suite, nine editor project tests, LuaLS, isolated VS Code, and the wiki build pass. Wiki verification covers 41 pages, 98 bindings and 3,062 local links/assets. No Unity gameplay test was performed.
