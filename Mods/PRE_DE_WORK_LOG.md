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

## API 0.13: native combo and style observations

- Added `on_combo_changed` and `on_style_changed` to existing behavior hosts.
  Native combo notifications retain the finished count on expiry; style events
  report rank transitions after the model update. Initial setup and same-rank
  style progress do not emit the latter callback.
- Added immutable activity payloads, wrapper forwarding, detached Lua event
  tables, and rejection of missing or mismatched event sources. Existing
  capability restrictions and callback lifetimes still apply.
- Added `example.combo-reserve` and its matching editor template. A completed
  combo grants a five-second outgoing damage bonus, capped at fifteen stacks;
  another combo cannot replace an active reserve. Round state resets it.
  Expiry is checked when another relevant event arrives, not by a background
  timer. This is a reusable example, not a complete DE perk implementation.
- Public callback sections, examples, editor schema/generated contracts and
  LuaLS inference cover both events. API version is 0.13.0.
- Checks pass: 195 battle-rule assertions; 90 fight-patch/example assertions,
  including actual combo-reserve Lua, exact expiry, renewal, cap and round reset;
  seven checks against production ComboCounter with fixture thresholds; native
  dispatch source-order checks; the existing combat/mode regression; all four
  managed builds; eleven editor project tests; LuaLS; seven isolated VS Code
  integration checks. The wiki builds 41 pages, documents 102 bindings and
  validates 3,078 local links/assets.

The complete native encounter still needs a Unity playtest. ComboCounter tests
execute production counter logic, but its event dispatcher and threshold config
are fixture substitutes. Style integration is source-checked and compiled.

Next runtime investigation: `Fight.RenderFight` advances `fightTimeInFrame`
only when `round.processing`, then updates models, collisions, AI, native rules
and round settlement. A future combat tick must specify its position relative
to those operations and avoid delivering after round settlement. Existing
player dispatch rebuilds active perk sets and resolves profile/item instances
per event; copying that path into every simulation frame needs an explicit
subscription/host-caching design and lifecycle validation first. No tick API is
claimed by this release.

## API 0.14: active combat simulation ticks

- Added `on_tick` with `frame`, `seconds`, `delta_frames` (1), and
  `delta_seconds` (1/60). Native dispatch follows the combat clock increment,
  before model movement/collisions/AI and round settlement. Only processing
  rounds tick after fight initialization; native pause stops RenderFight.
- Player callbacks precede opponent callbacks, with a round-processing recheck
  between sides. Existing recursion guards, rule filters, callback error
  isolation and round/fight state lifetimes apply. No persistent background
  scheduler or wall-clock timer is introduced.
- Lua rejects absent/inactive/pre-clock tick sources; event tables are detached
  and fighter capabilities expire after callbacks. Pending-hit modification
  methods remain restricted to their respective damage callbacks.
- Script sessions cache immutable handler subscriptions after registration.
  Fights without tick subscriptions skip tick dispatch. Rule/perk/enchantment
  hosts skip parameter/state resolution when the current callback is absent.
  Mutable native equipped/active state is deliberately not cached, so native
  suppression remains authoritative. Disposal clears subscriptions.
- Combo Reserve and its matching manual editor template now clear expired
  stacks on ticks. The example manifest requires API 0.14. Public references,
  generated editor types and completion tests include the tick payload.
- Passed 207 battle-rule checks (including real production-session subscription
  discovery/disposal and real Lua tick lifetime/state validation), 93 fight
  patch/example checks, seven production combo-counter checks plus source-order
  and pause guards, the existing combat/mode regression, all four managed
  builds, eleven editor project tests and LuaLS inference checks.
  Seven isolated VS Code integration checks also pass; the wiki builds 41 pages,
  covers 103 public bindings and validates 3,082 local links/assets.

Unity pause/resume, full round/encounter gameplay and per-frame performance
profiling remain unverified. Source-order checks are not runtime playtests.

## Owned UI foundation (internal; public API remains 0.14)

- Added engine-independent `ModUiScope`, `ModUiSurface`, immutable layout nodes
  and widget snapshots under Eclipse.Runtime. Scopes own bounded, independently
  named surfaces. Trees validate unique IDs, leaf/container shapes, finite sizes,
  bounded text/progress, depth and node count before registration.
- Added targeted text/progress/visibility/enabled updates and change-only
  notifications. Button callbacks reject hidden/disabled ancestors, stale
  controls and reentrancy. Callback/render errors close the affected surface;
  teardown errors cannot prevent remaining observers or scope cleanup.
- Added `Eclipse/UI/Modding/ModUiView`, a Unity UI renderer for stack/row/column,
  scroll, text, button and progress nodes. It uses the existing game-font lookup
  with fallback, plain wrapped labels, clipped scroll content and incremental
  updates. Input polling remains outside the view; explicit traversal/activation
  supports the future foreground coordinator. Close hides immediately and
  restores owned focus; native destruction closes the model.
- Added new Unity metadata without changing existing GUIDs. Updated local
  generated project compile lists so managed checks include the new source.
- `Tools/TestModUiRuntime.ps1` passes 41 production state/lifetime checks.
  `Tools/TestModUiUnity.ps1` passes 15 checks in an isolated Unity 2022.3.62f3
  play-mode project using the production renderer: hierarchy, font fallback,
  updates, scroll clipping components, guarded activation/focus and teardown.
  All four managed builds pass.
- Source investigation and the remaining integration sequence are recorded in
  [UI_RUNTIME_IMPLEMENTATION.md](UI_RUNTIME_IMPLEMENTATION.md). In particular,
  native DialogCanvasController currently disables/restores GraphicRaycasters;
  the future coordinator must cooperate with it and arbitrate keyboard input.

No `sf2.ui` binding, runtime mount coordinator, creator example or complete custom
UI capability is claimed yet. Full-game scene/input/dialog integration, actual
device events, screenshot/layout acceptance, localization, extended widgets and
the three planned creator workflows remain open. The Unity fixture is a real
native-component test, not a full-game test or proof of visual fidelity.

## UI layer and scene coordination (internal)

- Added `ModUiLayerStack`: modal/menu/HUD priority, newest-within-priority order,
  one stack per surface, a 64-surface scene budget, foreground input gating,
  native-block suspension and Back behavior. Scene disposal closes surfaces
  without disposing script scopes. Widget-only updates avoid ordering rebuilds.
- Added `ModUiCoordinator` under Eclipse-owned UI source with a new preserved
  meta/GUID. It creates scene-owned canvases and pointer-blocking exclusive
  backdrops, applies safe-area anchors and uniform root fitting, compacts canvas
  sorting ranks, and gates background interaction. Explicit focus/activation/
  Back methods own and restore EventSystem navigation without polling keys or
  granting pause authority. Native blocking hides views and yields input; native
  raycaster enabled flags are never overwritten.
- Added managed layer ordering/isolation and native coordinator coverage.
  The first native run exposed a fixture lookup treating a slash in an object
  name as a hierarchy path; internal canvas names now use owner:id labels.
  Final checks pass: 59 managed UI checks, 26 isolated Unity 2022.3.62f3 play-mode
  checks, all four managed builds, and whitespace validation.

This remains internal groundwork. Game entrypoints, native dialog/input bridge,
script-session cleanup wiring, bounded Lua bindings, creator tooling/examples,
and physical input/full-game acceptance still remain. The passive coordinator
does not yet instantiate itself during game startup.

## UI game-input bridge and script ownership (internal)

- Added `ModUiGameBridge` with a new Unity meta/GUID. It creates the scene
  coordinator on demand, routes keyboard/controller UI input before ordinary
  updates, waits for neutral menu controls after capture, repeats navigation in
  unscaled time, and consumes the closing frame. Duplicate Back delivery in one
  frame cannot close two overlays. Title/restart states suspend mod UI.
- Connected DialogCanvasController blocking/unblocking to the bridge without
  replacing its native raycaster operations. BackKeyManager gives exclusive
  mod UI first refusal. No combat pause or result authority was introduced.
- Added `ModUiControlGate<T>` and routed GameController's keyboard/gamepad/touch
  event emissions through it. Capture releases currently active controls in
  press order, captured presses stay suppressed until release, and physical
  release polling continues. Ordinary native delivery stays intact without
  capture; synthetic releases are not duplicated on the later physical release.
- MoonSharp contexts now implement `IModUiScriptContext`, own a mod-scoped UI
  lifetime, and close it during context disposal before clearing script tables.
  Updated the managed source fixture to include this runtime dependency.
- Checks pass: 71 UI/control tests plus native source contracts for control,
  Back and dialog routing; 209 battle-rule checks including real script-context
  UI ownership/disposal; existing combat/mode regression; all four managed
  builds. The isolated Unity fixture passes 34 checks including actual bridge
  routing, native/title/restart blocking signals, closing-frame input retention,
  duplicate Back suppression and bridge teardown. Its shell/controller signals
  are stubs, so physical device and full-game behavior are not claimed.

Public Lua creation/setters/click bindings, editor contracts and creator examples
are still pending. The bridge is callable by engine code but no `sf2.ui` module
is published; public API remains 0.14. Full-game scene/dialog/input acceptance,
localization, additional widgets and ability authority remain open.

## API 0.15: public owned Lua UI

- Added `sf2.ui.open`, `close`, `is_open`, `set_text`, `set_value`,
  `set_visible` and `set_enabled`. Creation requires `ui.create`; opaque handles
  are private to their originating script context. Weak-key handle storage
  avoids permanently retaining every closed handle. Close is idempotent;
  setters require an open view and the appropriate widget type.
- The Lua parser validates node kinds, dense arrays, unknown fields, duplicate
  IDs, cyclic/oversized/deep trees, finite dimensions/progress and text limits.
  Setter arguments reject implicit string/number/boolean coercion. Failed mounts
  close their surface and failed entrypoint disposal closes any mounted views.
- Optional click functions receive the view handle and widget ID through the
  existing bounded Lua runner. Failure/time-budget exhaustion closes their view.
  They never receive an expired or ambient fighter capability.
- Added an optional injected renderer callback to MoonSharpScriptRuntime while
  preserving its parameterless constructor. ModRuntime connects the game host
  to ModUiGameBridge; renderer-less test/tool hosts reject UI creation explicitly.
- Added `example.charge-ui` and matching manual editor template. The third Act I
  tournament fight gets a five-second charge HUD. Pointer activation arms one
  positive unblocked outgoing hit for 2x damage using fresh combat authority.
  It updates UI ten times per combat second, uses integer frames for exact
  charge boundaries, and resets/tears down each round.
- Added the public Custom UI reference, sidebar entry, capability documentation,
  example index, editor schema/generated definitions and recursive-node
  completion. The initial supported contract explicitly documents centered
  layouts, pointer-only HUD buttons and remaining widget/localization limits.
- `Tools/TestModUiLua.ps1` passes 369 assertions using production Lua code,
  including the complete example, stale/forged handles, strict types, tree
  validation, missing capability/renderer, failed mounts/entrypoints and an
  infinite click handler interrupted by its budget. A fixture originally used
  unavailable `pcall`; tests now validate each expected script failure through
  the host without changing the sandbox.
- All four managed builds, 209 battle-rule checks and the combat/mode regression
  pass. Editor generation/check, twelve project tests, LuaLS and seven isolated
  VS Code integration checks pass. Wiki verification builds 42 pages, covers
  110 public bindings and validates 3,212 local links/assets.

At API 0.15, Lua/renderer components were checked separately; the prior 34-check isolated Unity
fixture covers the production view/coordinator/bridge, while the new Lua fixture
injects a recording renderer. Full-game Lua-to-native visual/device acceptance
remains unverified. Broader E4 workflows, HUD controller focus,
localization/assets/theme, extra widgets and complete custom modes still remain.

## API 0.16: anchored UI and an end-to-end Unity fixture

- Added optional `placement = { anchor, x, y }` to `sf2.ui.open`, with nine
  anchors, center/zero defaults and finite offsets bounded to -8192..8192.
  Positive X moves right; positive Y moves down. No operation DSL is involved.
- The production renderer aligns the matching root pivot with the safe-area
  anchor, uniformly scales oversized roots and clamps offsets to keep the entire
  root visible. Resizing restores the requested placement when space permits.
- Updated Charged Strike and its editor template to place the HUD near the
  top-right corner; bumped their minimum API and the engine API to 0.16.
- Extended editor schema/generated definitions with UiPlacement, documented
  defaults/limits/resizing, and tested inline anchor completion with real LuaLS.
- Extended the isolated Unity fixture to execute the actual mod Lua through
  production MoonSharp and the production view/coordinator/bridge. A real Unity
  Button invokes Lua; ticks update native UI; hit callbacks consume charge;
  round/context teardown removes the view. Anchor and resize checks cover all
  nine anchors, offsets, oversized roots and extreme clamping.
- Verification: 76 managed UI checks, 391 real-Lua assertions, 57 Unity play-mode
  checks and all four managed builds pass. Editor generate/check, twelve project
  tests, LuaLS and seven isolated VS Code integration checks pass. LuaLS's optional
  field completion labels include suffixes; the completion assertion now handles
  them. Direct Node invocation avoided npm's Windows executable-path escaping.
  Wiki build passes: 42 pages, 110 documented bindings, 3,212 checked links/assets.
  `git diff --check` passes.

Physical input, game-font appearance, full-game scene integration and broader
custom UI/mode/character workflows remain open. These changes require no missing
DE art assets and do not begin the DE content port.

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
| G03: dojo/custom menus | Static locations and basic owned interactive UI exist; persistent dojo selector and complete menu workflows remain. |
| G04: contextual item grants | Existing rewards/grants are partial; inspect complete archived enchanted chest payload and implement missing instance fields. |
| G05: activated set abilities | Set membership exists; activation, cooldowns, input and presentation need runtime contracts. |
| G06: combat control | Rules, snapshots, outgoing scaling, combo/style observations and active combat ticks exist; statuses and action/outcome authority remain. |
| G07: level-specific perk parameters | API 0.11 supplies native variants and learned Lua overlays; managed/native-source checks pass, Unity acceptance remains. |
| G08: moves/input/projectiles | Native binary/template foundation exists; full authoring pipeline and supported procedural operations remain. |
| G09: AI reactions | Native tactics exist; conditional decisions/programmable intent remain. |
| G10: animated scenery/music | Fight music/location patching exists; animated layers and playlist semantics still need implementation/evidence. |
| G11: item metadata/default effects | Registration is partial; supported non-economic patches and innate loadouts remain. |
| G12: forge candidates | Owned families exist; targeted core candidate editing remains, with costs base-owned. |
| G13: achievement predicates | Counters/core localization exist; event/query-driven predicates remain. |
| G14: service/boot/presentation | Named gates exist; targeted quest suppression and intent classification remain. |
| E2/E3/E4 shared runtime lifetimes | Combat query expiry and scoped ticks exist; general subscriptions, cancellation, clocked work and authority for modes/UI remain. |
| E3 programmable modes | Fixed sequences exist; branching persistent runs and settlement proof remain. |
| E4 custom UI | API 0.16 exposes owned UI, anchored placement and a Charged Strike example; full-game acceptance, HUD focus, localization/theme/assets, full widgets and creator workflows remain. |
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
