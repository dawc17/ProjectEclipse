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

## API 0.17: dynamic localized text

- Added `sf2.localization.text(key, language?)`, accepting an existing owned
  localization handle and returning a plain string. Default language comes from
  the game host on each call; tool hosts default to English. Explicit language
  codes are normalized/validated. Requested language falls back to `eng`, then
  empty text when neither exists. Forged handles and non-string codes fail.
- Added read-only catalog/transaction resolution: pending owned translations and
  pending patches are readable during registration, and retained handles read
  committed content during later callbacks. No new mutation capability, template
  expression language or implicit UI subscription was added. Lua owns formatting
  and refresh timing. Existing localization ownership/dependency checks remain.
- Preserved both existing MoonSharp runtime constructors and added an injectable
  language provider; ModRuntime reads the same native language source as the
  recovered localization adapter. The neutral runtime stays Unity-independent.
- Updated Charged Strike and its editor template to use English and Polish TOML
  translations. Percentage formatting uses Lua `string.format`; each refresh
  resolves both status and button labels. The example now requires API 0.17.
- Updated the localization/UI wiki references, example listing, engine version,
  editor schema/generated contracts and editor guide. Contract inventory is
  111 public bindings, 76 constants and 147 typed structures.
- Verification: all four managed builds and the Phase 1 showcase pass; 404 Lua
  assertions cover current/fallback/explicit language reads, bad handles/codes,
  pending and committed patches, plus existing UI behavior. The isolated Unity
  fixture passes 59 checks, including changing the provider language to Polish
  and then an unknown language while the actual Lua HUD updates native widgets.
  Editor generate/check, twelve project tests, LuaLS and seven isolated VS Code
  integration checks pass. The VS Code runner now creates a fresh profile for
  each run, preventing restored unsaved quick-fix edits from contaminating later
  fixture runs; the rerun passes after an initial missing-diagnostic failure.
  Wiki build passes with 42 pages, 111 bindings and 3,217 checked links/assets.

This does not establish game-font glyph coverage, live native language-menu
acceptance, automatic translation bindings or a complete custom UI system.

## API 0.18: game-consistent UI defaults and bounded styles

- Applied the user's explicit direction that custom UI should look consistent
  with original SF2. The renderer now uses the original parchment, beveled white
  button, combat-bar textures and AGOpusBold font. Label and button-state colors
  follow native prefabs. HUD roots remain transparent; menus/modals get parchment.
- Reused ResolutionImage's existing sprite resolution/compatibility path. Native
  sliced button borders scale with authored height. No asset files or Unity GUIDs
  were edited. Missing-art fixture fallbacks retain the game palette.
- Added immutable node `style` fields: font_size (integer 8..128), text_align,
  text_color, background_color and fill_color. Colors accept only RGB/RGBA hex.
  Wrong widget kinds, malformed styles and invalid values fail before mounting.
  Sprite color overrides tint existing art; they do not replace its shading.
- Kept root-background visibility coupled to the owned root; hiding a menu does
  not leave its parchment visible. Style updates do not alter input authority.
- Charged Strike retains native styling and uses a 24-unit status label; its
  minimum API and the engine API are now 0.18. Updated editor template, schema,
  generated contracts, inline LuaLS style completion, wiki reference and guides.
- Expanded the Unity fixture with the actual font/skin assets and production
  ResolutionImage code, retaining substitute bundle/atlas backends. Added an
  optional graphics preview capture and visually inspected the rendered result.
  The fixture initially caught an invalid Color32 equality test; it now compares
  values with Equals. New checks cover asset identity, native style application
  and root-paper hide/show behavior.
- Verification: 84 model checks, 434 real-Lua assertions, all four managed builds,
  editor generation/check, twelve project tests, LuaLS and seven isolated VS Code
  checks pass. The final Unity graphics run passes 66 checks; the wiki builds
  42 pages, documents 111 bindings and validates 3,220 links/assets.
  Full-game layout and physical-device acceptance remain open.

## API 0.19: result-driven mode branching

- Added optional `on_result` Lua callbacks to mode/event/raid registration.
  A detached result snapshot provides won, one-based roster position, roster
  size, saved completion count and fight definition ID. Callbacks return an
  owned roster fight, "complete", or nil for default progression. Invalid,
  foreign/non-roster and unbounded callback results are isolated and fall back.
- Added a typed mode-script context boundary and session dispatch, wired through
  ModRuntime into native ModModeRuntime settlement. Callbacks have no fighter or
  shared reward capability. They run after native fight outcome/reward handling;
  route choice does not replace fight settlement.
- Extended saved progression with validated selected steps while retaining the
  existing roster signature and save format. Complete transitions validate bounds
  and counter overflow before releasing the reservation or writing progress.
  Explicit completion works after either outcome and respects repeatability.
- Consumed the live native settlement guard before dispatch, preventing repeated
  callback execution/reward eligibility when a result has already been handled.
  Existing entry tickets, launch rollback and interrupted reservation paths remain.
- Marked definitions using custom routing. The native map resolves their saved
  selected encounter, while linear completion bricks/count suffixes are hidden:
  skipped roster entries must not appear to be wins. This map change is compiled
  and covered at the host-policy level, not visually playtested in the full game.
- Added the standalone `example.branching-trial` and matching editor template.
  It alternates 1→3 and 1→2→3 using saved completion counts, routes losses to 1,
  uses original game assets and grants no rewards. The actual shipped Lua executes
  in the fixture across both routes. Existing integrated showcase behavior stays
  unchanged; its copied fixture supplies additional malformed/failing callbacks.
- Updated the public mode reference with a dedicated on_result section, examples,
  callback timing/returns/fallback/save limitations, editor schema/generated data,
  LuaLS result inference and starter validation. Mode callbacks are inventoried
  separately from combat callbacks to avoid giving them fighter semantics.
- Verification: all four managed builds pass; the expanded combat/mode fixture
  passes Lua routing, invalid/foreign handles, instruction limits, native selection,
  loss routing, duplicate guards, reload, counter-overflow preservation and existing
  raid/state/policy tests. Underworld runtime passes 1,282 assertions. Its asset
  audit completes but reports missing scenery across all three raid locations;
  this is not complete asset acceptance. Editor generate/check, thirteen project
  tests, LuaLS and seven isolated VS Code checks pass. Wiki build covers 42 pages,
  112 public bindings and 3,224 local links/assets. Diff whitespace check passes.

No full-game playtest was performed for this mode expansion. Persistent seeded
RNG, generated encounters, pre-entry player choices, asynchronous lobby/results,
custom run-state migrations and complete one-time settlement across interruption
boundaries remain broader E3 work. The goal remains open.

## Mode replay and interruption verification

- Audited native entry from InfoBattle through GameUtils.StartFight. The map's
  mode path selects the saved encounter, and StartFight resolves it before native
  roster setup and reserves mode entry before scene launch. Existing equipment
  requirements remain in effect. No replay-limit bypass was added.
- Traced reward selection through Fight.GameOver and GameUtils.EndFight. The
  native reward index comes from the current fight's initialized opponent/result
  state, rather than the mode's saved route position. No new reward policy was
  introduced during this audit.
- Strengthened TestP2ACombatRuntime: the actual shipped Branching Trial now runs
  both routes through production ModModeRuntime, not just ModModeProgress.
  Every encounter settles, rejects a duplicate result without changing XML or
  callback count, serializes/reloads the save and resolves the next native entry.
  It also reloads an entered reservation, resumes it without mutating reservation
  data, and verifies that a loss returns to the first encounter without adding a
  completed run. All checks pass with the existing combat/mode regression.
- This replaces weaker example coverage with native-host/save evidence. Scene
  loading, RosterFight internals, native fight simulation and physical input are
  not executed by this fixture; full-game acceptance remains unproven. The source
  audit is evidence of routing order, not a substitute for a playtest.

No public API or save format changed in this verification pass. Remaining E3 work
still includes seeded persistent randomness, generated encounters and asynchronous
player choices/lobby/results.

## API 0.20: saved random streams and seeded encounter routes

- Added `ModApiFacade.RandomInteger` and `RandomNumber` in ModScripting and
  `sf2.random.integer(field, minimum, maximum)` / `sf2.random.number(field)` in
  MoonSharpScriptRuntime. Streams use declared signed 32-bit integer state
  fields, both state capabilities, the existing owned state save path and a
  published stable v1 sequence. Bounds/types are strict; inclusive integer
  ranges use rejection sampling with a 128-attempt native-work cap. A failed
  draw does not commit stream state. Successful writes are not rolled back by
  a later Lua error or grouped transactionally with mode results/rewards.
- Advanced the API version to 0.20. No new save format, Unity asset or GUID was
  introduced. Existing seeds may be reset using normal state writes; equal
  seeds/calls reproduce results without touching Lua/native global randomness.
- Added `Mods/example.seeded-trial` and matching manual editor template: a
  three-encounter repeatable mode with first-win seeded branching, loss reset,
  original core assets and no rewards/entry price. Seed defaults preserve
  existing saves. The original alternating Branching Trial remains available.
- Added `TestModRandomRuntime.ps1` / `ValidateModRandomRuntime.cs`: 123 passing
  checks for golden sequences, full-width/single-value bounds, rejection,
  negative/zero seeds, serialized reloads, independent profiles and mods in
  one profile, disable/reinstall, future-schema preservation, mixed actual Lua
  calls, invalid fields/arguments, unbound state and missing capabilities.
- Extended TestP2ACombatRuntime to run both shipped branching examples through
  production mode routing with native host stubs, serializing and rebinding
  state after every encounter. It verifies selected routes, exactly-once
  callbacks, duplicate results, two completed runs and resumed losses.
- Added the public Saved random streams reference and sidebar entry, linked
  state/editor guides, updated examples and Mods README. Updated editor schema,
  generated Lua definitions/metadata and editor README. The validator now
  supports functions needing multiple capabilities and emits separate fixes.
  Added seeded template parity/capability tests, LuaLS completion/diagnostics,
  and an actual VS Code test for both capability quick fixes.

Verification: random runtime and existing P2 combat/mode fixtures pass; all four
managed projects compile; editor generate/check/build, 14 project tests and
LuaLS integration pass. Wiki build passes with 43 pages, 114 documented bindings
and 3,340 checked links/assets. The existing duplicate-404 Astro warning remains.
All 8 isolated VS Code integration checks pass, including separate quick fixes
for both random-stream capabilities. `git diff --check` passes.
No full-game Unity playtest was performed for this addition. The RNG fixture
uses production Lua/state code; the mode fixture stubs native host objects.
Actual gameplay/save timing and full interruption/settlement acceptance remain
unproven. Generated fights, async choices and broader E1–E8/G01–G14 work remain.

## API 0.21: UI close notification and cancellation

- Added `ModUiCloseReason` and optional close notification to the neutral UI
  model. First close wins; widgets, scope/layer ownership, native views and input
  are released before notification. Script, Back, scene teardown, renderer/click
  error and external destruction are distinguished. Close observer errors are
  isolated. Scope shutdown uses its own host reason.
- Added Lua `on_close(view, reason)` to UI definitions. It runs with the existing
  200,000-instruction callback budget, once for a successfully mounted live view.
  Failed mounts, owner-scope shutdown and disposed scripts skip Lua notification.
  Opening UI from close callbacks is rejected, including nested callbacks, so
  cleanup cannot rebuild menus during scene exit. Reopening after close returns
  is supported. Widget setters reject the closed handle; querying/closing it is
  safe. No gameplay authority, save transaction or pause behavior was added.
- Updated ModUiView/ModUiCoordinator error and native destruction paths to carry
  reasons. Preserved original game font, parchment, buttons, bars and assets.
- Updated Charged Strike and matching editor template to require API 0.21 and
  clear local view/charge/armed state on closure. An armed bonus is canceled if
  its native HUD disappears. Renamed its unused tick parameter for clean LuaLS
  diagnostics. Updated both example READMEs.
- Updated public UI reference with dedicated `on_click`/`on_close` sections and
  all timing/limits, examples, editor guide, editor README and Mods README.
  Added UI callbacks to coverage inventories and generated Lua/API metadata;
  editor definitions include the view handle and five public close reasons.
- Extended model tests (104 checks), actual Lua tests (781 assertions) and
  isolated Unity 2022.3.62f3 tests (69 checks). Covers notification order/reasons,
  once-only/reentrant closes, errors/budget, stale setters, opening restrictions,
  failed mount/shutdown suppression and armed-bonus cancellation after native
  destruction. All pass. Full-game playtesting remains outstanding.
- All four managed projects compile. Wiki build passes: 43 pages, 116 public
  bindings/callbacks and 3,349 local links/assets; existing duplicate-404 warning
  remains. Editor generate/check/build, 14 project tests, LuaLS integration and
  all 8 isolated VS Code checks pass. `git diff --check` passes.
- Added [the manual test checklist](PRE_DE_TEST_CHECKLIST.md) covering Charged
  Strike first, mode replay/persistence, game styling and the original UI
  regression reports. The original defects are listed for rechecking without
  claiming this API pass repaired them.

The user requested wrapping up after this work. No further feature expansion
should start as part of this wrap-up. The full pre-DE objective remains incomplete;
see the manual test checklist and open requirements rather than interpreting the
current API version as completion.

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
| E3 programmable modes | API 0.20 supplies saved random choices alongside result-driven roster branches; generated encounters, pre-entry choices, async lifecycle, lobbies/results and full interruption/settlement proof remain. |
| E4 custom UI | API 0.17 exposes owned UI, anchored placement, dynamic translated strings and a Charged Strike example; full-game acceptance, HUD focus, automatic language bindings/custom assets, full widgets and creator workflows remain. |
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


## Charged Strike eclipse tournament attachment correction

The user's live eclipse fight showed no HUD. Inspection of canonical stages.xml
found a separate ZONE_1/Tournament_ECLIPSEMODE/3 identity. The sample patched
only ZONE_1/Tournament/3; the rule's default all-mode filter does not attach it
to another fight. The earlier answer claiming eclipse compatibility from the
mode filter alone was insufficient and incorrect for this replay battle.

Added an explicit append-rules patch for the eclipse fight to Charged Strike
and its editor template. Updated the UI reference, both example READMEs and the
manual checklist. Added regression assertions using production
ModBattleRuleInstances.Applicable and canonical runtime fight IDs: both normal
and eclipse fight 3 have the player rule; opponents and adjacent fight 2 do not.
The Lua fixture now passes 787 assertions. Editor generate/check, all 14 project
tests, LuaLS and the wiki build pass (43 pages, 116 bindings, 3349 links).
No native source/assets or API version changed. Live replay remains for the
user to retest after restarting Play mode so mod scripts reload.


## Trial map visibility and footer correction

The user could see Third Strike Trial but not Branching Trial or Seeded Trial.
Both mode samples registered zones/battles without a map-session reveal quest.
Added the existing supported show_battle entry quest, unlocked, to each sample
and its editor template. Added zones/trial English localization to all three
trial examples/templates, fixing the missing footer title visible in the report.
Updated both mode READMEs, the public examples guide and manual checklist with
bottom-page-dot navigation and the effect of several examples focusing pages.

The P2 native-host fixture now checks each shipped mode's entry quest place,
session event, unlocked target battle and matching localized zone title before
its route/replay/save tests. The complete fixture passes. Editor project/LuaLS
checks and wiki build pass; no API/native asset or source change was needed.
Full-game map visibility still requires retesting after mod scripts reload.


## Distinct fighters for the two route trials

Replaced each mode's shared default opponent with three separately registered,
localized warriors. Branching Trial: Gatekeeper (Man_Kunai), Bulwark
(Man_Batons), Night Warden (Man_Night). Seeded Trial: Wayfarer (Man_Kungfu),
Needlehand (Girl_Sai), Storm Ronin (Man_Nunchaku). The native templates supply
original portraits, clothing, skeletons, voices and weapon loadouts. No generated
art, asset identity changes or new combat policy was introduced. The first
warrior retains its prior ID; encounter IDs/order and random state are unchanged
so existing mode progression remains usable.

Updated both example scripts/localizations/READMEs and their editor templates,
the public examples page and manual test checklist. The P2 fixture now imports
canonical warrior templates, verifies three distinct warrior bindings per mode,
expected localized names/templates, and distinct native portrait and weapon
references before running the existing routing/save/replay checks. It passes;
editor generate/check, 14 project tests and LuaLS also pass. Wiki build passes
with 43 pages, 116 bindings and 3349 checked links/assets. Full-game appearance
and combat remain for user testing after restarting Play mode.


## API 0.22: generated encounters, AI, character tools and native UI controls

Implemented the owner's five requested extensions without starting the DE port.
Static content remains typed; decisions and preparation use ordinary bounded Lua.

- Modes/events/offline raids accept `on_prepare(request,event)`. Return a typed
  encounter plan immediately or retain an owned request for a later UI callback.
  `sf2.modes.resolve/cancel/is_pending` provide explicit completion and cancellation.
  Plans override owned warrior rosters, level, rounds and round time over a registered
  blueprint; location, rules, rewards and native identity remain the blueprint's.
  The plan is validated and saved before native entry, reused on reload/retry,
  and consumed by existing once-only settlement. Pending continuations are never
  serialized. Scene/profile/context teardown invalidates requests.
- Tabular tactics accept `on_decide(memory,event)` with detached fighter snapshots
  and currently playable action handles. Return a current action, `"wait"`, or nil
  for native fallback. Decisions are limited to one per six active simulation frames;
  stale/forged choices and callback failures disable that fighter's handler and fall
  back. Memory is isolated by controller and tactic. Tactic changes reset throttling.
- Warriors accept typed `body_model` and `skin_models`. Narrow native parser,
  model-composition and condition hooks carry these assets and character identity.
  Moves add character/key conditions, key-pressed events, frame bounds, attacking
  edges, damage attribute/multiplier, hit height and impulses. Existing serialized
  enum values and Unity GUIDs are preserved.
- Blender authoring tools import a native point rig, sample evaluated motion,
  export rest-body geometry and attached triangle skins, bake 60 Hz animation,
  emit fingerprints and an interactive preview, and generate a scoped Lua module.
  Portable Python validation/baking is also available. This is the SF2 point-rig
  workflow, not automatic arbitrary-FBX retargeting or Blender shader conversion.
- UI toggles and sliders reuse original checkbox/settings-slider assets and the
  game font. `on_change` and `set_checked` preserve ownership, bounded callbacks,
  foreground input and silent programmatic updates. Menu focus supports keyboard/
  controller slider adjustment; HUD interaction remains pointer-based.

Added Generated Expedition (procedural opponent plus asynchronous difficulty/time
choices) and AI Dojo (three visually distinct fighters with separate decision
policies), with matching editor starters. Updated the public wiki, character guide,
binding audit, authored editor schema, generated definitions and editor tests.

Verification: all four managed assemblies compile. UI neutral runtime 119 checks,
actual Lua UI 803 assertions, isolated native Unity UI 76 checks plus visual preview;
AI 30 checks including shipped AI Dojo; mode workflow 33 checks including shipped
Lua, production XML projection, save/reload, cancellation, settlement, malformed
requests and instruction limits. Existing P2 branching/seeded/raid/settlement suite
passes. Underworld runtime passes 1282 assertions; its asset audit still reports
the known 40 missing scenery references and no malformed metadata.

Character validation passes Python format tests, actual Blender 3.6.23 body/skin/
motion export, the unchanged Unity animation reader (60 frames, 67 nodes; 16145
checks), and actual generated Lua registration/native warrior+move projection.
Editor generate/check, 16 project tests, LuaLS and eight real VS Code integration
checks pass. Fixed a VS Code test race by waiting for diagnostic publication after
a superseding refresh. The wiki builds 44 pages and covers 123 public functions/
aliases/callbacks; links and search index pass.

Full-game generated-fight construction/entry, physical input, AI behavior and
authored character deformation/contact timing still require manual acceptance.
Native UI and animation fixtures do not establish those outcomes. No DE assets,
Unity serialized identities or shipped core content were rewritten.


## Gymnast authoring integration and timing correction

The earlier point-rig export proved file compatibility but did not supply an
approachable visual authoring scene. The primary guide now uses the unchanged
Gymnast Tool Suite checkout (1.1.5, revision
b44dea8ae549ff52ec8d08d7d1ad86f53db80702) and its visible SF2 capsule body/IK rig.
Blender 5.0.1 reads the supplied 405.91 scene cleanly; tested 4.4.3/4.5.3 builds
warn about possible data loss. The bridge requires 5.0+ and registers the add-on
only in its process. No upstream source or scenes are vendored.

Added scene preparation/opening, required-node preflight, evaluated-pose export
comparison and native package generation. Generated Lua connects the warrior,
move and AI tactic to a repeatable visible map fight. Assets retain exact source
bytes. Existing outputs are refused. The lower-level point tools remain available.
Corrected sample-index bounds: interpolation changes sample spacing, not native
end_frame or interval indices. The HTML preview now respects sample FPS.

Verification: seven Python tests pass. The complete Blender integration authors
an IK hand motion and skin, rejects a missing wrist binding, compares 60 frames
of 67 nodes with upstream export, and loads both zero- and two-mid-frame packages
through production Lua registration and AI selection. The unchanged Unity reader
passes 16,145 checks. This is not full-game deformation/contact acceptance.
Updated wiki, tool/editor guides and manual checklist. Wiki build passes 45 pages,
123 documented bindings and 3,636 local links/assets (existing duplicate-404
warning remains). Editor generation/check, 16 project tests and LuaLS pass; generation
also synchronizes existing toggle-label guidance from the public UI reference.
No new public runtime binding, core asset or Unity GUID changed in this slice.
E5/E8 are advanced, not closed; broader G01-G14/E1-E8 requirements remain active.


## Quest suppression host groundwork

Added an initially empty, ordinal-name suppression policy to QuestsManager.
Configuration is atomic and rejected while its queue is nonempty or running.
Suppressed definitions remain discoverable; deleting them would make Roster's
missing-definition path clear saved parameters. Event dispatch now skips before
Compare, explicit queue requests skip before preparation, and queue restoration
filters without mutating the caller's saved list. Clearing the policy restores
eligibility using the same QuestStage instance.

Source tracing also found direct Run/Foreach execution and pre-queue roster scene
selection. These paths now consult the same policy before running children,
clearing delivery collections, resetting unresumable saved quests or selecting a
resume scene. ResumeQuests counts only eligible records. Default empty policy
preserves existing behavior; no production suppression is installed yet.

Tools/TestQuestSuppression.ps1 compiles the complete production manager with
scene/roster stubs and the native event enum. Its 567 assertions cover all 51
nonempty event types, object/name/saved queue paths, no comparison/preparation
for suppressed quests, retained definitions/progress, re-enabling, mixed saved
parameters, ordinal names and atomic/active-queue rejection. Direct action and
Roster call-site guards are source-inspected and managed-compiled; the fixture
does not prove a serialized full-game resume or native dialogue completion.
Underworld's 1,282 runtime assertions pass; its known missing scenery remains.

This is internal groundwork for G01/G02/G14, not a published Lua capability.
Next: catalog identity/import validation, typed suppression ownership/conflicts,
transactional registration, startup application before restore, fingerprints,
Lua/editor/wiki contracts and actual saved-resume/native-action fixtures.
Do not mark core quest replacement/removal complete on this evidence.


## Quest identity correction before public registration

The next public suppression step exposed duplicate names across core extension
files (including the mini-event families) and a duplicate within dynamic_discounts.
A name-only policy would silently suppress other sources. Internal policy keys
now use exact source-file#quest-name identities; repeated records within the same
source and name intentionally share that suppression identity.

The native loader flattens includes into a root Quests document and historically
sets each QuestStage.FileName to the loading container. That field must retain
its saved-file semantics. Added separate EclipseSourceFile provenance: the loader
stamps each plain quest document before include expansion/promotion and the stage
retains it separately. Source XML files are unchanged. Native roster restoration
uses saved container identity to find the stage and then consults its source;
a second check after lazy file loading protects unresumable state and scene routing.

The complete manager plus actual provenance/condition-merge methods now pass 573
checks, including same-name/different-source quests, flattened include provenance,
and unchanged saved loader identity. All four managed builds and Underworld's
1,282 assertions pass. Public Lua binding, catalog import/conflict validation,
startup policy installation and full-game save/resume acceptance remain pending.
The initial name-only host design is superseded by this source-aware contract.


## API 0.23: source-aware quest suppression

Published `sf2.quests.suppress { target = "namespace:quests/id" }`, requiring
content.patch and a registered owned/dependency target. Core targets include
original source XML path and quest name. The importer indexes 865 source identities
from canonical XML without altering definitions. Exact duplicates within a source
share a target; normalized-identity collisions with distinct native keys reject.

Suppression uses the existing Remove patch ledger and transaction capacity,
dependency, duplicate/conflict and fingerprint contracts. Startup applies source
keys after owned quests register and before queue restoration. The host gates
introduced above preserve definitions and saved progress. Mod changes require
Apply & Restart; live policy replacement is not exposed. The example suppresses
an owned old introduction while retaining a new native dialog, without core edits.

Verification: 19 actual Lua/catalog assertions cover canonical import, host-key
projection, fingerprints, missing targets/capabilities/dependencies, duplicate and
two-mod conflict rollback, owned targets, disable/rebuild identity and the shipped
example. The 573 production-manager/provenance assertions pass. All four managed
projects compile. Editor generate/check, 17 project checks and LuaLS pass. Wiki
build passes 45 pages, 124 documented bindings and 3,640 links/assets; the existing
duplicate-404 warning remains. Full-game saved interruption/resume, direct native
action completion and example dialog appearance remain manual acceptance work.

This advances G01/G02/G14. Individual quest action patches, general procedural
story callbacks/queries, and other core content domains remain open. DE porting
is still deferred. The earlier work-log statements that no binding exists are
historical and superseded by this entry.


API 0.23 final teardown check: adapter rollback/disposal clears suppression without
clearing an active native queue, so an unrelated startup failure cannot leave base
quests disabled after mod shutdown. The manager fixture now passes 574 assertions.
Managed Assembly-CSharp recompilation passes. All eight real isolated VS Code
integration checks also pass (invoked directly with node to avoid npm's Windows
path quoting). Full-game acceptance remains unclaimed.


## Saved quest lookup and direct-action verification

Native roster restoration still looked up stages by name alone even after source
suppression was added. Saved lookup now matches the original loading container
as well, through FindEclipseSavedQuest; ordinary name-based Run behavior remains
unchanged. Both Roster.PBOFBNFALNN and ResumeQuests use this lookup. This prevents
binding a saved record to an earlier-loaded same-name stage from another file.
The original one-argument GetQuestByName remains available. Missing-file lookup
returns null rather than choosing an unrelated definition.

Added TestQuestResumeRouting.ps1, which executes the actual recovered Run class,
Foreach entry method and roster resume method against observable scene/roster
services. Fifteen assertions prove suppressed children complete without executing,
all six Foreach collection paths are skipped before side effects, unresumable
checkpoints are retained, no suppressed resume scene is selected, clearing the
policy restores execution, source lookup chooses the correct definition and lazy
loading cannot bypass the gate. The complete manager/provenance fixture now passes
576 checks. These are production-method tests with host services stubbed, not a
Unity story playthrough or a complete serialized-profile round trip.


## Animated scenery host investigation and vertical phase repair

The native SimpleEffect parser supports Picture and Sequention types, X/Y
oscillations, transparency and rotation curves plus velocity/wrap fields.
ChangingSprite.INPLHCAAJKP (vertical phase offset) was an empty method even though
Location.ParseSimpleEffect invokes it for OscillationY.Offset. It now advances
the Y interpolator, matching the X/rotation/transparency setters. The native
parser applies offsets before adding points; that ordering is preserved.

TestLocationOscillation.ps1 executes the unchanged production Interpolator and
IntervalSet classes plus actual ChangingSprite axis methods. It checks all 18
arena_new Y curves over 240 steps against horizontal and explicit phase-advanced
references, including three nonzero offsets, a simple numeric displacement and
loop continuity. All 13,039 assertions pass. All four managed projects compile.
This verifies numerical motion, not rendered scenery or a Unity encounter.

Inventory: canonical locations contain 389 Picture and 131 Sequention effects;
archived DE has 488 Picture and 166 Sequention effects. No effect Point in either
location tree has a nonpositive Period. The current Interpolator can loop forever
on an all-zero-period curve, so future public validation must reject those values.
Do not expose raw native points without finite/size/duration constraints.

Remaining G10/E6 work: typed effect/curve definitions and Lua authoring, asset
resolution/scale verification, projection/fingerprints/editor/wiki, native render
and lifetime acceptance, then animation atlases/audio selection/world hazards.
Scenery currently follows LocationSelector.Render's native clock; this is not
combat tick authority or proof of pause-safe hazard behavior. No new Lua location
fields are published by this repair; API remains 0.23.

## API 0.24: animated location pictures

Location images accept motion_x, motion_y, rotation and opacity curves with bounded
period/value/ease points and phase offsets. Lua validation, native SimpleEffect
projection and content fingerprints include all four channels. Static image
projection is retained. Animated masks/opaque images are rejected. Qualified
picture sprites use their own import density and preserve flip flags.

The Animated Arena example uses the original battlefield backdrop with a four-second
vertical drift and opacity loop. It is a repeatable normal fight, not a hazard.
The editor schema and public location guide document limits and native quadratic
interpolation. These changes advance G10/E6; atlas animations, particles, audio
instances/playlists, hazards and camera operations remain open.

Verification: 25 real Lua/validation/projection/fingerprint assertions and 13,039
native interpolation assertions pass. Rendered scale, appearance, pause/resume and
scene teardown still require Unity/game acceptance; these fixtures do not prove them.

API 0.24 final checks: editor generation/check, all 18 project tests and actual
LuaLS completion pass (including nested optional curve fields). Wiki build passes
45 pages, 124 binding sections and 3,643 local links/assets. Assembly-CSharp
rebuild passes after reviewing qualified sprite-directory routing. The existing
first-argument import-density check is correct because projection splits the asset
into qualified directory and leaf; a transient change to check the leaf was reverted.
No full-game or Unity render result is claimed.

## API 0.25: random location music

Locations accept music_choices: zero to 16 distinct typed audio handles, mutually
exclusive with nonempty music. Ownership/dependencies and dense arrays are checked;
choice order is fingerprinted. Projection emits the native Music choice list.
External choice lists have the same priority as existing external single tracks,
then normal fight/default fallback applies. Native selection occurs at fight entry
and loops one track; this is not saved seeded randomness or a sequential playlist.

Animated Arena now uses two existing core fight tracks. The public guide and editor
schema explain the supported behavior. All four managed builds pass. The extended
Lua/projection suite passes 34 assertions and actual Location selection code passes
six precedence/fallback checks. Audible playback, mute/volume and repeated scene
transitions remain game acceptance work. G10/E6 remains open for controllable audio
instances, sequential playlists, atlas effects, particles, hazards and camera intent.

API 0.25 editor/wiki acceptance: generation/check and all 18 project tests pass;
LuaLS also checks the actual Animated Arena script without diagnostics. All eight
isolated VS Code tests pass after correcting the random-capability test to wait
for diagnostic publication when a debounced refresh supersedes its explicit call.
Wiki build passes 45 pages, 124 binding sections and 3,646 local links/assets.
Both selected core audio files exist. Actual audible playback is still unverified.

## Dojo selection host routing repair

G03 investigation found that QuestActionChangeDojoLocation changes only
GameUtils.NIPABEEAMHJ. Canonical Training is DUMMY (mapped to FightNone) with
Location=dojo. DojoScene selects a preloaded Training FightList; Fight previously
constructed its Location from that cached field, ignoring the changed global.
The archive reapplies its saved DojoLoader variable at ApplicationStart, so simply
wrapping the existing action would neither implement persistence nor reliably
change the rendered dojo on reentry.

Location.ResolveEntryLocation now resolves a nonempty current dojo name for
FightNone when the fight is constructed. Other battle types retain their explicit
locations; an unset selection retains the definition fallback. No content/saved
fight definition is mutated. The normal location loader still handles unavailable
art. The native quest action remains unchanged. No new Lua API is published here.

TestDojoLocationRouting.ps1 executes the production resolver with the production
BattleType enum: 29 checks cover selected core/qualified locations, empty defaults,
all other encounter types and nonmutation. Assembly-CSharp and Editor compile.
This does not verify rendering, immediate in-place refresh, profile persistence,
missing-mod restore or the complete selector flow.

Next G03 requirements: register validated choices and explicit ownership/composition;
bind the selected key to owned profile state after ModRuntime.RecordSaveContext
and ModScriptSession.BindState; resolve unavailable choices without deleting saves;
expose safe selection/query operations and a game-themed menu entry; test profile
switch/restart/disable and repeated scene entry. General story subscriptions and
scene navigation remain separate G02 work. DE content remains deferred.

Dojo routing final checks: all four managed assemblies pass, Underworld regression
fixture passes 1,282 assertions, and wiki build passes 45 pages with 3,649 local
links/assets. No Unity render or full-game dojo acceptance was performed.

## Dojo preference store (internal; API remains 0.25)

Added ModDojoSelection beside existing save services. It maintains an atomic set
of qualified location choices (maximum 256), binds one profile at a time, records
explicit selection/reset, and resolves absent choices to a supplied base fallback.
The stored choice is not erased when a mod disappears. Reintroducing the choice
restores resolution. Clearing/unbinding never modifies profile XML.

The internal preference node is EclipseMods/DojoSelection with schema=1 and a
qualified location attribute. Fresh binding is read-only. Unknown schema, duplicate
nodes and invalid IDs reject without rewriting data, and failed binding cannot
retain a previous profile. Explicit reset removes only the location attribute;
unknown attributes and siblings survive. Normal RecordContext preserves the node.

TestDojoSelection.ps1 passes 37 production-runtime assertions including serialized
roundtrip, two profiles, removal/reinstall, all rejection paths, registration
rollback, reset and teardown. All four managed assemblies compile; the shared
Phase 1 runtime regression also passes.

This is internal groundwork, NOT a published save contract or functioning selector.
It is not yet instantiated by ModRuntime, catalog registration, Lua or a menu.
Remaining work is to connect validated catalog choices, capability/ownership checks,
profile binding and native entry resolution; then deliver original-style menu
interaction and full-game acceptance. Do not claim G03 closed or ask users to test
an unavailable selector. No DE content port or API version bump occurs here.

## API 0.26: saved dojo selection

Location registration accepts dojo=true to opt in (false by default). The flag
is fingerprinted; aggregate choice capacity is validated transactionally before
commit. ModRuntime rebuilds choices only from successful registrations, clears
bindings on restart/shutdown, and binds the selected profile after save-context
recording. Failed/unknown profile metadata unbinds the prior profile. Native dojo
entry resolves the active preference without changing GameUtils or fight data.

Published locations.select_dojo(handle), selected_dojo(), reset_dojo(), gated by
presentation.dojo. Selection requires the caller's own registered opted-in handle.
Reset cannot erase another mod's preference. The query reports the saved ID even
when unavailable. No operation forces disk saving or changes an open scene.

Added example.dojo-selector: a map entry opens a native-themed UI through deferred
mode preparation. Select/reset/back closes the UI and cancels preparation without
starting a fight. Enter Dojo using the normal menu to see the chosen backdrop.
Native menu insertion and automatic scene navigation are not exposed by this slice.

Verification: 10 actual MoonSharp UI callback/registration/capability checks cover
selection/query/reset, missing capability, no profile, non-dojo handles and foreign
reset protection. The save service has 37 checks; the shared motion/music fixture
loads the actual selector script and confirms eligible location/mode registration.
All four managed assemblies compile. Full-game selector interaction, save flushing,
visuals, disable/reinstall and profile-switch acceptance remain outstanding. Earlier
entries describing an unconnected store are superseded by this integration.

API 0.26 final verification: 16 actual Lua checks now include the shipped selector's
on_prepare/on_click/on_close workflow, select/reset cancellation with no fight plan,
and ignored stale-request clicks. Location fixture has 36 checks including dojo
flag fingerprinting and shipped registration. The 29 entry-routing checks pass.
All four managed builds pass. Editor generation/check, 19 project tests, LuaLS
(including the selector) and eight isolated VS Code checks pass. Wiki build covers
127 binding sections, 45 pages and 3,658 local links/assets. Full-game and Unity
render acceptance is not claimed. Checklist section 10 records those pending checks.

## API 0.27: player profile queries

Published sf2.profile.level() and sf2.profile.item(handle), gated by profile.read.
Item queries return copied present/owned/count/equipped/upgrade values; ownership
matches native positive-quantity semantics and absent items use nil upgrade.
Core names and supported redirects resolve through the content catalog. Arbitrary
strings/forged handles are rejected; currency and mutations are not exposed.

ModRuntime receives the newly constructed Roster alongside RecordSaveContext.
It does not consult ListSF's potentially previous roster during profile loading.
Reads remain unavailable until save-context/state binding has completed. Startup,
failed profile binding and shutdown drop the reference and host services. Lua
receives no native objects and modifying a snapshot cannot change inventory.

Verification: 13 actual Lua checks cover current/absent inventory, permission,
unavailable host, wrong handles and detached tables. Six production host-method
checks use controlled roster services to verify native core-name mapping, fresh
values, retained snapshots, changed roster, unknown definition rejection and unbind.
All four managed builds compile. No full-game UI inventory comparison or live
profile-switch acceptance is claimed. Public wiki and typed editor schema updated.

This advances G02/G13 queries, not their complete event/predicate requirements.
Purchase history, item subtype queries, learned perks/tutorial/story state, runtime
subscriptions and typed story operations remain open. API adds no new events.

API 0.27 final checks: editor generation/check, all 20 project tests, profile
snapshot LuaLS field inference and all eight real isolated VS Code checks pass.
Wiki build passes 46 pages, 129 binding sections and 3,773 local links/assets.
No new game-facing inspector is shipped by this slice; the guide has callback
examples. Native roster methods were verified with controlled services, not a
Unity profile playthrough. Story subscriptions and broader queries remain open.

## Active-profile lifecycle correction before story subscriptions

Event-flow investigation found NHAMDLEDOHM is a shared roster constructor, also
called by JLEMHLLLCLD for a comparison copy. Recording/binding mod save context
inside it could redirect profile queries, dojo state and Lua state to a nonactive
roster. It also ran before the active inventory was fully prepared.

Moved RecordSaveContext to PBNNPBEDOOJ immediately after ANEHEDFAPCH is assigned
and HOMCPNCGPDB finishes inventory preparation. Generic roster construction no
longer binds or records mod metadata. New ModRuntime.UnbindProfile clears profile
queries, dojo binding, pending modes and bound Lua state before loading and on
ListSF.Reset. ModStateRuntime.Unbind preserves definitions and all serialized XML.

TestProfileActivation.ps1 executes the actual loader, constructor and reset methods
with controlled native services. Seven checks verify activation ordering, comparison
isolation, two-profile switching, missing-file handling and reset. The state/random
fixture now passes 129 checks including rejected access while unbound, idempotent
unbind, unchanged save bytes and restored values on rebind. All four managed
assemblies compile. No full-game profile switch or live UI verification claimed.

API remains 0.27. This corrects prior profile/dojo lifecycle assumptions and provides
a reliable activation boundary for forthcoming story subscriptions. No subscription
binding is published in this change; G02/G13 events remain outstanding.

Activation correction final regression: profile queries (6 host + 13 Lua checks),
dojo preference store (37 checks), and wiki build (46 pages, 3,773 links/assets)
pass. Documentation now states reset/loading unavailability and comparison-roster
isolation. Full-game verification remains pending.

## Story notification transport foundation

Added Runtime/Modding/ModStoryEvents.cs as a main-thread, engine-independent
transport for detached purchase/enchantment notifications. Scope disposal releases
callbacks and stops remaining callbacks from that scope during dispatch. Subscriber
additions become visible on the next notification; nested publication uses FIFO
delivery. Exceptions cancel only the failing subscription and retain owner-attributed
diagnostics. Diagnostic failures cannot propagate into a native caller.

Subscriptions are bounded to 64 per mod across scopes and 256 overall. Each root
dispatch accepts at most 128 notifications and invokes at most 1024 callbacks;
excess work is dropped with diagnostics. Profile unbinding discards the old queue
and interrupts the current notification. Subscriptions survive a profile switch,
and notifications explicitly published after rebinding can run for the new profile.
Clear cancels all subscriptions and invalidates old scopes, preventing stale owners
from registering callbacks after a runtime restart.

Tools/TestStoryEvents.ps1 compiles the production transport and identity types into
an isolated fixture: 30 checks pass, including callback mutation, cross-owner scope
disposal, recursive publication, both capacity budgets, profile changes, stale scopes,
failed handlers and failed logging. All four managed assemblies compile. This does
not yet connect native events, runtime profile binding or script contexts to the
transport. Lua on/off bindings, native identity projection, integration fixtures,
public docs/editor contracts and an example remain the next work. API stays 0.27;
no Unity/gameplay or public story subscription availability is claimed.

## API 0.28: native story subscriptions

Connected the transport to runtime start/shutdown and active-profile binding.
ListSF.FFBAJNGHGGD captures detached purchase/enchantment identities before native
quest evaluation, publishes afterward, and preserves the native boolean result
and exception behavior. A profile-generation check rejects stale captures if native
processing changes the profile. Unobserved events skip identity projection.
Core items resolve through the existing catalog adapter; native recipes use their
forge-profile ID and owned recipe families retain their forge-recipes ID. Unknown
identities become nil, without dropping the notification.

Published sf2.story.on/off/is_active under story.events. Each script owns a scope,
with callback instruction limits and opaque weak-key handles. Context disposal
cancels subscriptions before UI teardown. Registered listeners can wait for profile
activation; saved state is accessed through the existing profile binding, and no
event history is stored or replayed. Story Observer logs both supported events.

Verification: 30 transport checks; 13 extracted production native dispatch/capture
checks with controlled quest processing; 29 actual Lua checks including the shipped
example, denied capabilities, fabricated handles, unknown event names, no host,
detached callback tables, repeated cancellation, exception/infinite-loop isolation
and context teardown. Seven profile activation checks pass. All four managed builds
pass. Editor generation/check and 22 project tests pass; LuaLS verifies event-field
inference, and eight isolated VS Code checks pass. Wiki builds 47 pages with 132
binding sections and 3,894 local links/assets. Full-game purchase/forge playback
remains pending. This supersedes the unconnected foundation status above; broader
story events, query coverage and typed story operations still leave G02/E3 open.

## API 0.29: experience-driven level notifications

Native source inspection found no dispatch of QUEST_EVENT_LEVEL_UP. Roster's
DBPBGBNHAIP performs experience threshold processing, inventory updates, fight
level refresh and save-field updates. Added a notification after its final
Experience write, preserving its return value. Host filtering requires the same
active roster and profile generation as at entry. A single operation emits the
original/final level pair if it increases, even when native cap handling returns
false; repeated XP at the cap, comparison rosters and unloaded profiles are silent.

Added level_up to story.on, with optional previous_level/level integer payload
fields. Other event payloads retain nil level fields; level events have no item or
recipe. The Story Observer, public reference, manifest guide and editor contracts
are updated together. Direct level assignments are outside this event contract.

TestLevelUpStory.ps1 extracts the actual production experience, threshold and host
notification methods: 16 checks pass with controlled save/inventory services.
Actual Lua subscription checks now total 35, including level payloads and the
updated shipped observer. Transport validation has 36 checks; the 13 purchase/forge
native checks still pass. All four managed assemblies compile. Editor generation,
check, 22 project tests, LuaLS field inference and eight isolated VS Code checks
pass. Wiki builds 47 pages, 132 binding sections and 3,894 links/assets. Full-game
level gain and UI acceptance remain pending; the checklist records them.

Scene investigation: Module.OAAFAINKKMI dispatches QUEST_EVENT_SCENE_LOADED directly
after SceneManagerSF.Load starts LoadSceneAsync(1), before LoaderScene asynchronously
loads the target scene. This is not evidence of scene readiness, so no scene-loaded
subscription was exposed at that misleading native boundary. A later initialized
scene boundary remains needed for reliable custom menu/scene workflows.

## API 0.30: deferred initialized-scene entry

Scene<T>.Awake completes native Init, module registration and widescreen layout
before scheduling the new owned ModSceneEntry component. Its coroutine yields a
frame, checks that its object/scene is active and still the requested destination,
and publishes only for the captured profile generation. It is attached to the
destination object, so no persistent global coroutine keeps an unloaded scene
alive. Unobserved/unsupported scenes skip scheduling. Successful or rejected
delivery destroys the helper component. No legacy SCENE_LOADED semantics changed.

The story scene_enter event carries a typed scene string for map/shop/profile/
dojo/fight. It grants no fighter authority and does not bypass dialogs or wait
for every animation. Lua can open the existing game-styled UI here; its existing
scene-owned rendering and on_close callbacks handle teardown. Story Observer,
public reference/manifest guide and editor contracts are updated to API 0.30.

Verification: 20 checks execute the extracted native Awake and the production
helper coroutine with controlled Unity services, including initialization order,
deferred/once-only delivery, scene replacement, profile switch, inactive objects,
unsupported scenes and failed/rejected Init. Actual Lua checks total 43, including
scene payloads, shipped observer logging and UI creation/replacement/button close
from a scene callback. Transport validation has 41 checks. All four managed builds
pass. Editor generation/check, 22 project tests, LuaLS scene-field inference and
eight isolated VS Code checks pass. Wiki builds 47 pages, 132 binding sections and
3,894 links/assets. Real Unity scene unload/coroutine behavior and rendered menus
in a full game remain unverified, explicitly listed in the manual checklist.

## Real Unity scene coroutine verification and cancellation repair

Added TestSceneStoryUnity.ps1, ValidateSceneStoryUnity.cs and SceneStoryUnityDriver.cs.
The isolated Unity 2022.3.62f3 project runs production ModSceneEntry, story transport,
identity types and the extracted host publisher in play mode. Native Module/profile
services are controlled, while SceneManager, GameObject lifetime, deferred Start,
coroutines and component destruction are real Unity behavior.

The initial 12 checks passed unload, once-only deferred delivery, profile replacement,
superseded destinations and owner destruction. Extending the fixture to reactivate a
previously disabled owner produced a real failure: Start had never run, so pending
delivery revived after reactivation. Fixed ModSceneEntry.OnDisable to invalidate
and remove the helper, and skip scheduling initially inactive objects. Successful
completion clears configuration before destruction to avoid redundant cancellation.

The expanded fixture passes 15 checks, including reactivation, disabled helper
components and initially inactive owners. The failing pre-fix evidence is in
Temp/SceneStoryUnity-a84c6f1ae6b84f239d9592fdfc0cd63b/validation.log; post-fix pass is
Temp/SceneStoryUnity-b6a9b5fa42874d35990697cd5c8d47d0/validation.log. These generated
projects/logs remain untracked. This improves scene-entry acceptance without
claiming full-game native scene or custom UI rendering/input verification. Public
documentation now states cancellation on deactivation. API remains 0.30.

## API 0.31: native menu scene navigation

Published sf2.scenes.open(destination), gated by presentation.navigate, over the
native Module.DLOKJOHNDID path with quest/tab checks enabled. Destination strings
are limited to map/shop/profile/dojo. Host checks require an active profile and
initialized matching menu scene, with no pending encounter preparation, native
input block, lock screen, combat/loader source or concurrent navigation. Same-scene
requests succeed without reloading. False preserves native quest interception;
exceptions release the reentry guard. Lua cannot navigate from UI on_close cleanup.
The host service is installed after script load and cleared at restart/shutdown.

Scene Menu opens game-styled travel buttons from scene_enter, closes on accepted
requests and reports rejection without looping. Native menu insertion is still
separate. Arrival remains asynchronous and observed through scene_enter.

TestSceneNavigation.ps1 extracts both the production gate and native transition
overload: 33 checks pass for destinations, source scenes, dialog/lock/preparation
states, native quest/tab interception, reentry and exceptions. Thirty actual Lua
checks cover capability/argument/host rejection, cleanup restrictions, boolean
results and the shipped menu's accepted/rejected/Back buttons. All four managed
assemblies compile. Editor generation/check, 23 project tests, LuaLS and eight
isolated VS Code checks pass. Public reference, manifest guide and editor schema
were updated together. Full-game transition, layout and input acceptance remain
pending in checklist section 12. This advances G02/G03/E3/E4 rather than closing them.

Navigation documentation final check: corrected the scene guide's relative story
link; wiki build now passes 48 pages, 133 binding sections and 4,010 links/assets.
Regenerated editor hover data and rechecked schema consistency after the link fix.

## Scene Menu in the production Unity renderer/input bridge

Extended TestModUiUnity.ps1 to copy example.scene-menu into a separate discovery
root, preserving the existing Charged Strike fixture. ValidateModUiUnity now executes
the shipped menu Lua with the real ModUiView, coordinator and input bridge in Unity
2022.3.62f3 play mode. Story events/navigation responses are controlled in this fixture.

Checks cover the original AGOpusBold font, parchment and button sprites, five button
bounds, native rejection text, dialog blocking, accepted close, closing-frame input
consumption, directional selection and submit, coordinator destruction, remounting,
combat exclusion and context disposal restoring native navigation ownership.
The initial bounds assertion used Rect.Contains, which excludes the upper/right
edges; corrected the fixture to inclusive bounds with 0.01-unit rounding tolerance.
No production layout repair was required.

The expanded complete fixture passes 99 Unity hierarchy/update/input/lifetime
checks. Latest evidence: Temp/ModUiUnity-51fa674d30f847d3b06c5244299ad430/validation.log.
This generated fixture remains untracked. It does not claim actual native menu
transitions, full-game visuals or physical-device acceptance. Public verification
notes, the example README and checklist now distinguish this evidence. API stays 0.31.

## API 0.32: existing encounter opponent replacement

Fight patches now accept warriors: 1–100 unique registered warrior handles,
replacing the complete list in order. Registration validates ownership and
references; the semantic fight/warriors field participates in conflict handling.
The patched definition preserves encounter identity and other fields. Native
projection builds the entire replacement collection before changing its cloned
source, using the existing production warrior builder. Rewards, rules and native
attributes are retained. This advances G01; reward and other-domain editing remain
open.

Verification: 121 fight patch checks pass, including actual Lua registration,
order-sensitive fingerprints, conflicting owners, invalid handles/lists, failed
projection rollback and unrelated-field preservation. Corrected the competing-mod
fixture to use a directory matching its manifest ID. All four managed builds pass.
Editor generation/check, 23 project tests, LuaLS and eight VS Code checks pass.
Wiki builds 48 pages and validates 4,010 local links/assets. Public reference and
editor schema/generated definitions are updated together.

Full-game opponent rendering, fight progression and disable/restart restoration
remain manual acceptance work. The projection test uses a controlled warrior
builder; it does not prove the complete native warrior construction path. A
separate remaining hardening issue is per-call rollback when Lua catches a later
field validation failure with pcall: existing multi-field patch staging needs an
atomic call boundary, beyond the tested entrypoint/commit transaction rollback.

## Fight patch call rollback and sandbox correction

Added a host-only atomic staging boundary around the Lua fight-patch binding.
Failed field validation removes only that call's staged fight records and conflict
keys; previous successful calls remain. Tests cover repeated failures, retry and a
duplicate field already owned by an earlier call. The fixture now passes 125 checks.
All four managed builds pass, as do editor generation/check and the wiki build
(48 pages, 4,010 links/assets).

Correction to the previous entry: production uses MoonSharp Preset_HardSandbox,
which does not install pcall. An actual Lua regression attempt failed because
pcall was nil; direct inspection confirmed this, while assert is present. The
partial-call recovery scenario was therefore not reachable by current mod Lua.
The new boundary is tested through the host facade, not advertised as working Lua
pcall recovery. Entrypoint failures still abort the whole registration. The public
fight reference and generated editor guidance now explain the distinction. No new
Lua binding or version bump was added.

Reward investigation: RewardPrize.Parse reads Money, Bonus, Exp, PrizeBase plus
Money/Currency/Resistance/Lottery children; Reward additionally combines matching
Level children with the base prize. RewardChoice permits mixed item and currency
choices, so removing an item changes the remaining currency probabilities. The
owned BuildRewardNode emits only Bonus, Item and item-only Choice entries. Replacing
an existing native Reward with that output would erase economic and conditional
semantics. Follow-up must target item grants with explicit result-slot and level
semantics, preserve economic rows/attributes and mixed choices, and reject unsupported
edits rather than silently narrowing native rewards. G01 reward editing stays open.

## Targeted reward drop projection prerequisite

Added ModRewardDropProjection in the existing owned runtime content source. This
is a host helper, not a new public Lua API. It targets an existing zero-based native
result row, shared/normal/Eclipse scope and optional exact level range. Missing
mode/range scopes can be created within that row; result slots cannot be invented.
It replaces direct Item and item-only Choice children while preserving the row's
other attributes/children, including currency, experience, lottery and other level
or mode scopes. Mixed choices reject editing because changing their item weights
would change economic probabilities. Rewards carrying gems are rejected.

Projection works on a detached clone and commits only after validation and builder
success. Tests cover all six mode/level combinations, new level scope creation,
ambiguous scopes, currency-bearing choices, nonexistent slots, reversed bounds,
currency-bearing input, invalid builder output, builder failure and clearing drops.
The fixture passes 152 checks, including the previous fight-patch coverage; all four
managed builds pass. Builders and native reward settlement are not exercised by
this helper fixture. Registration, conflicts, fingerprinting, Lua/editor/wiki,
production adapter connection and end-to-end native settlement remain necessary
before announcing supported reward patching. API remains 0.32.

## API 0.33: scoped encounter item reward editing

Connected reward_drops on fights.patch through capability-gated registration,
immutable fight edits, semantic scope conflicts, content fingerprinting and both
core/owned native fight adapters. Entries select an existing wins/result slot,
shared/normal/Eclipse addition and optional exact player-level bounds. Registered
reward handles provide items/choices; gems are rejected. Validation probes a cloned
native source before staging succeeds. Later fight-field copies retain the edits.
Missing mode/level scopes may be created; nonexistent result slots are rejected.

Actual Lua tests cover registration, scope retention through later patches,
fingerprints, committed projection, conflicts, independent mode edits, malformed
arrays, invalid handles/bounds, currency rejection and transaction rollback. The
fixture passes 174 checks. All four managed builds pass. Editor generation/check,
23 project tests, LuaLS and eight isolated VS Code checks pass. Wiki builds 48 pages
with 4,013 links/assets; it includes the typed schema, item-grant example, additive
mode/level semantics and current verification limits. API schema now has 166 types.

The production item builder/native reward parser/settlement combination still
requires direct verification, followed by full-game reward UI, inventory grants,
replays and disable/restart acceptance. The fixture's committed projection uses a
controlled builder and does not prove that complete path. G01 remains open beyond
this scoped item-drop capability; mixed economic choices, lotteries, enriched item
metadata and broader content editing remain separate gaps.

## Native item reward composition verification and lottery repairs

Added TestRewardNative.ps1/ValidateRewardNative.cs. The runner reads production
Reward, RewardStruct, RewardPrize, RewardChoice, RewardLottery and Rewardable,
plus the exact native RewardItem constructor and adapter reward/item/name builders.
It replaces anti-cheat numeric storage with primitive aliases and controls profile
mode, math/scalar conversion, item-level expressions and non-item grant services.
It does not execute inventory settlement or a full Unity fight.

Initial 45 checks verified normal/Eclipse and level boundaries (2,3,9,10), original
currency/experience/scaling, native item identity, upgrade/drop flags, item choices
and repeated evaluation. Adding a shared lottery plus non-lottery Eclipse addition
reproduced NullReferenceException in RewardPrize.HNJGHOKCDJF at
Temp/RewardNative-3029d8cd3d8e4024bf0c27ed38923cb0/RewardPrize.cs:101.

Fixed the native merge to inspect the incoming lottery before merging. Added an
internal RewardLottery.CloneForRewardComposition that copies its slot collection;
otherwise repeated evaluation mutates the shared source when mode lottery slots
are appended. Expanded tests cover two lotteries, repeated evaluation, subsequent
normal-mode evaluation and detached returned lists. All 49 checks pass. Lottery
slot reward execution remains controlled, and this is not a recovered lottery UI.

All four managed builds pass, editor generation/check passes, and wiki builds
48 pages with 4,013 links/assets. Public verification notes distinguish this native
builder/parser/composition evidence from remaining inventory, display, replay and
save acceptance. No API version change (still 0.33).

## Reward result selection verification

Extended TestRewardNative to extract the exact FightResult item-selection overload.
Production builder/parser output now reaches that method with controlled item
catalog, ownership and upgrade services. Six new checks cover upgraded drop flags,
retained grant metadata, owned equipment rejection, missing/null items, upgrade
clamping and repeatable owned mod consumables. Total: 55 checks pass. No production
change was needed. This verifies result selection, not inventory mutation/persistence.

Public guidance now explains that already-owned equipment is skipped by native
result handling, so acceptance needs an unowned item/test profile. Controlled
item-level expression/upgrade services are explicitly outside this fixture's
native coverage. Actual ListSF inventory granting and full-game acceptance remain.

Canonical example correction: stages.xml gives BOSS_LYNX an EclipseToggleName of
BOSS_LYNX_ECLIPSEMODE; both first fights have two reward rows. Changed the reward
guide to target core:fights/zone_1/boss_lynx_eclipsemode/1 and explained that reward
mode scopes do not follow battle links. Corrected opponent/manual instructions
that previously implied a normal-fight patch would also affect its Eclipse replay.
The guide's Monk weapon exists in canonical list.xml. This is a documentation and
acceptance-target correction, not implicit patch propagation.

## Runnable Eclipse reward acceptance example

Added example.eclipse-reward with its own API 0.33 manifest and actual Lua script.
It adds Monk's Katars to the one-win Eclipse scope of the first BOSS_LYNX_ECLIPSEMODE
fight. It uses existing assets, adds no UI/map entry and does not unlock encounters.
README explains owned-equipment suppression, restart restoration and pending
full-game acceptance. Public examples index links the mod.

Extended TestFightPatches to load the actual manifest and Lua in a separate discovery
root against canonical stages and weapon records. It verifies the exact replay
encounter, reward slot/mode, canonical item identity, unchanged other fights and
unchanged native source/progress fields. Initial test compared a normalized ID's
string to uppercase input; corrected it to compare parsed DefinitionId values.
The complete fixture passes 179 checks. Editor check, 24 project tests and LuaLS
pass; wiki builds 48 pages and validates 4,013 links/assets. No production code or
API version change. Full-game loot display/grant/persistence remains pending.

## API 0.34: learned-perk profile query

Added sf2.profile.perk(perk) under profile.read. The host resolves core legacy names
or owned IDs against the active roster's UserPerks list and reads stored UpgradeLevel.
Returns a detached learned/upgrade table; unlearned returns false/nil. This does not
report active temporary effects, equipment enchantments or trigger activation.
The profile service is cleared during runtime shutdown like other profile queries.

TestProfileApi now passes 11 extracted production host checks with controlled roster
services and 23 Lua capability/handle/snapshot checks, including the existing item
queries. All four managed builds pass. Editor schema/generated contracts, reference,
README and LuaLS field completion updated together: 134 public functions, 167 types.
Editor check, 24 project tests, LuaLS and eight isolated VS Code checks pass.
Full-game learning/upgrading/reset/save acceptance remains pending; G02 stays open.

Environment limitation: the first fixture build failed in Microsoft.Build.Tasks.Git
because repository metadata supplied a NUL-filled invalid reference. git rev-parse
--verify HEAD also failed. No repository metadata was changed. Managed checks ran
with EnableSourceControlManagerQueries=false to bypass optional source-link metadata.
This is not evidence of repaired Git history.

API 0.34 documentation final check: wiki build passed 48 pages and 4,017 local links/assets.

## Lottery result ownership repair

Followed lottery data beyond RewardPrize into FightResult's native lottery overload.
It retained the caller's RewardLottery, so a later merge appended slots into the
caller and any other result using that object. Extended TestRewardNative to extract
this exact production overload. Before the fix it failed with 'Lottery result merge
mutated source or another result' in
Temp/RewardNative-44a4f05cf30041f4ae1ddcf6b5b659f8/Program.cs:247.

Changed that single assignment to CloneForRewardComposition, matching the existing
RewardPrize ownership contract. Four new checks cover merging, independent source/
result lists, null input and clearing a returned collection. All 59 native reward
checks pass; Assembly-CSharp compiles. Optional source-control metadata queries
were disabled for these builds due to the previously recorded Git issue. No public
API or format change. Lottery slot execution, inventory mutation, UI and full-game
save/replay acceptance remain unverified; this does not close G04.

## API 0.35: item type/subtype profile metadata

Extended profile.item snapshots with optional native type/subtype strings. The
host resolves the same item identity used by inventory queries, then reads the
runtime catalog's Type/SubType independently of ownership. Missing metadata returns
nil fields without discarding inventory state; an unspecified native subtype stays
an empty string. No normalization or shared catalog mutation is introduced.

TestProfileApi passes 14 production-host checks with controlled services and 23
Lua checks. New cases cover unowned classification, native-name mapping, fresh reads,
snapshot isolation and missing runtime metadata. Canonical WEAPON_NUNCHAKU confirms
Type=Weapon and SubType=Nunchaku. All four managed builds pass with optional source
control metadata queries disabled as previously recorded. Editor schema, generated
contracts, README and public reference updated together; generation/check, 24
project tests, LuaLS field completion and eight VS Code checks pass. Full-game
catalog/inventory comparisons remain pending. G02/G11 remain open beyond this query.

## API 0.36: native item acquisition notifications

Added item_acquired to the owned story bus/Lua subscription surface, with detached
previous_count/count values. The native ListSF grant routine captures the profile
generation and prior inventory count, then publishes after its normal update and
optional auto-equip return successfully. The host requires the same active roster/
generation and an actual positive count increase. IDs resolve through the existing
catalog mapper; unknown items carry nil. This is not a universal inventory event:
separate delivery completion/direct edits remain outside the hook, and outer reward
flows can still apply enchantments after this notification.

TestItemAcquisition extracts the exact native grant routine and host publisher;
11 checks with controlled inventory services cover first/stack grants, zero/removal,
parent-linked upgrades, pending delivery, failure and stale/non-active profiles.
Actual Lua story tests pass 48 checks including acquisition payload and detached
callbacks; existing transport checks pass 41. All four managed builds pass with the
previously recorded optional source-control metadata query workaround. Public docs,
editor schema/definitions and completion fields updated together. Full-game grant,
delivery, callback reentrancy and inventory persistence acceptance remain pending.

API 0.36 final checks: editor generation/check, 24 project tests, LuaLS payload completion and eight VS Code checks pass. Wiki builds 48 pages with 4,020 valid local links/assets.

## API 0.37: delivery completion acquisition

Connected native UserItems.GBLHFNGPIOF to item_acquired when its own count mutation
raises an empty record to one. Publication follows native upgrade/level refresh
and save request, requires active inventory identity and the original profile
generation, and uses the count pair from that mutation. Upgrade-only/repeated
completion stays silent. Profile-load/direct inventory mutation remain outside scope.

The first extension captured count before native delivery quests. A regression where
the quest itself granted the same item reproduced duplicate acquisition in
Temp/ItemAcquisition-bdd6dc21c17a4b94bd900ba8d3ed7db8/Program.cs:181. Moved capture to
the delivery routine's actual count mutation and publish only when it performed
that mutation; nested quest grants retain their own notification.

The fixture now extracts both production grant and delivery methods and passes
20 checks with controlled inventory/quest/save services, including failure, stale
profiles, foreign inventories and nested quest grants. All four managed builds
pass with optional source-control metadata queries disabled as previously recorded.
Editor generation/check passes; wiki builds 48 pages with 4,020 links/assets.
Full-game migrated delivery, UI, inventory persistence and broader native callback
reentrancy remain acceptance work. Lua payload/bindings are unchanged.

## Nested grant count isolation

Added a native regression where auto-equip performs a second grant of the same
item. It reproduced double counting: the outer event read the final total after
the nested grant instead of its own mutation result. Failure evidence is
Temp/ItemAcquisition-e58d886cd06a4698909b3eed4aa67b72/Program.cs:189.

ListSF now captures acquiredCount immediately after insertion/count mutation,
while retaining publication after successful routine completion. The regression
verifies two notifications with total delta three (outer one, nested two) and
native return-order snapshots. All 22 acquisition checks pass; Assembly-CSharp
compiles with the existing optional Git metadata-query workaround. Public docs
explain snapshot counts versus current inventory and nested return order. API
remains 0.37. Full-game callback/save acceptance is still pending.

## Runnable acquisition observer

Story Observer now subscribes to item_acquired and logs the qualified/unknown item,
before/after counts and delta. Its manifest requires API 0.37 to include delivery
completion coverage. README and public story guide explain combined testing with
example.eclipse-reward, owned-item suppression, nested ordering and save limits.

Actual shipped Lua tests verify exact messages for known and unknown item identities;
the complete story fixture passes 50 checks. Editor generation/check, 24 project
tests and LuaLS pass. Wiki builds 48 pages with 4,020 valid links/assets. No runtime
code/API version change. Full-game observer + reward/delivery acceptance remains
pending; this example does not grant items or add UI.

## API 0.38: runtime profile references

Profile item/perk queries now accept qualified IDs alongside context-owned handles.
This lets story callbacks inspect dynamically discovered items without obtaining
registration handles in advance. The facade checks profile.read, category and
owner/dependency namespaces without accessing a closed registration transaction;
the existing native host validates catalog availability and reads active state.

TestProfileApi passes 14 native-host checks and 39 Lua checks. Added callbacks run
after commit with story.events/profile.read only, checking own/core IDs and rejection
of undeclared namespaces, malformed IDs, wrong categories and missing capability
before reaching the host. Host services in these Lua cases are controlled; this
is not a full-game acquisition test. Existing handle/snapshot checks still pass.

All four managed builds pass with the previously documented process-local optional
Git metadata-query workaround. Editor generation/check, 24 project tests, LuaLS
string-query snapshot completion and eight VS Code checks pass. Wiki builds 48 pages
with 4,023 valid local links/assets. Public reference, schema/generated contracts
and editor guide were updated together. No DE port work performed.

## Profile queries inside acquisition callbacks

Connected the profile fixture's extracted production query methods to actual Lua
item_acquired callbacks after registration commit. Four scenarios cover core
item/perk success, unavailable item, unavailable perk, and unbound native profile.
The callback uses event.item directly. Failure checks require the specific native
error, cancellation of only that listener, and continued delivery to another
listener on both publications. TestProfileApi now passes 54 Lua/integration checks
and 14 native-host checks. No runtime contract changed; API remains 0.38.

This closes a verification gap between separately tested Lua bindings and native
query methods. Inventory/catalog services remain controlled and event publication
is fixture-driven; full-game acquisition, redirects and profile switching remain
acceptance work. Existing item redirect resolution was inspected and remains
unchanged. No claim of full G02 completion is made.

## API 0.39: profile equipment enumeration

Added profile.equipment with detached item identity, native type/subtype, quantity,
owned flag and upgrade snapshots. Native host reads UserItems.JCMOHPFKPBO, whose
implementation selects each record's equipped flag. No five-slot assumption or
quantity filtering is introduced. Unknown identities remain nil with available
metadata, and no temporary fight equipment is represented. Host service unbinds
with the other profile queries. This advances G02/G13 equipment predicates.

TestProfileApi passes 18 production-method checks and 57 Lua/integration checks,
including empty lists, unknown records, metadata, detached nested tables, missing
capability and unbound profile. Inventory services remain controlled. All four
managed builds pass with the existing optional Git metadata-query workaround.
Editor generation/check, 24 project tests, LuaLS array field completion, eight
VS Code checks and the wiki build pass (135 references, 48 pages, 4,027 links).

Also corrected the authored editor schema for API 0.38 string item/perk arguments:
the earlier edit to generated api.json did not change the LuaLS schema. Both
unions now originate in api-schema.cjs and regenerate correctly. Public reference,
editor guide and generated contracts are synchronized. Full-game equipment UI,
profile-switch and rule-imposed loadout comparisons remain pending.

## Battle result event: recovered publication boundaries

Traced the next G02/G13 story requirement through production sources. A battle
outcome notification cannot be advertised as full reward settlement using any of
the current hooks:

- Fight.HCNDAFDHACI dispatches combat FightEnd before GameUtils.EndFight. The normal
  Fight.EndFight path also has combat-scope dispatch. These are fighter-lifetime
  callbacks, not completed inventory/progression transactions.
- GameUtils.EndFight captures the roster and resolves a fallback fight, then asks
  ModModeRuntime.CanResolve. That gate rejects duplicate completion for an active
  owned mode, but is not a universal encounter-instance guard for core battles.
- It creates FightResult, calculates rewards, changes battle progression, then
  calls ListSF.IMDGMNFHFCN only when its surrender/fallback flag is false. Therefore
  outcome alone does not imply rewards were granted.
- ListSF.IMDGMNFHFCN raises experience, currencies and item quantities, then applies
  each reward's enchantments with UserItem.GDBFNNLHPOB after GEFDJDIINND returns.
  Existing item_acquired notifications occur inside that grant call and precede
  those enchantments. A callback's equipment/profile queries read current state,
  not necessarily the fighter loadout that produced the result.
- ModModeRuntime.Complete runs before native quest result fields and NotifyResult.
  These callbacks can advance a mode before presentation and legacy fight-end
  events. They do not cover all core fights or establish all rewards settled.
- GameUtils.EndFight scans lottery rewards and stores HAOHNNFLOGK on wins. It skips
  the immediate legacy fight-end dispatch in that case. The repository's
  QuestActionDialogLottery.DEJMHFMLKIC only calls OGIJONMKABB; it does not implement
  drawing, prize presentation or deferred settlement. Other HAOHNNFLOGK references
  only retain the associated fight during Battle.MHMGONPIPKG cache cleanup.
- FightResult.IsWinner accepts only GAME_OVER_WIN. Raid timeout and raid-round
  timeout have separate enum values and must not be silently labeled surrender.

Implementation consequence: capture an immutable encounter result before reward/
quest callbacks can change state, bind it to profile generation and encounter
identity, and publish an outcome observation only at a documented successful
native completion boundary. Preserve distinct timeout/surrender outcomes. A later
reward-settlement notification requires completing the deferred lottery path;
do not imply that outcome delivery proves all loot or save I/O has completed.
Equipment-sensitive achievements need a captured combat/loadout context rather
than a post-reward profile query masquerading as the historical loadout.

Required next verification: normal/Eclipse core fights, generated owned modes,
surrender, fallback resolution, both timeout kinds, repeated/reentrant completion,
profile replacement during grants, native grant failure, and deferred lottery wins.
This turn changes engineering evidence only. No new story event is exposed, API
stays 0.39, and G02/G04/G13 remain open. Source inspection is not a game playtest.

## Encounter outcome lifetime foundation

Added a host-only opaque encounter token to ModStoryEvents. BeginEncounter replaces
any previous attempt only while a profile is bound. Result reservation happens
once before native callbacks; completion succeeds once for that same current
attempt. Cancellation consumes a failed attempt instead of allowing a retry after
possible partial native mutations. Unbind/clear invalidate pending attempts, and
cancelling an old token cannot erase a newer encounter. Tokens are neither saved
nor exposed to Lua. This guards future observation, not native reward execution.

TestStoryEvents passes 56 checks, including 15 new attempt-lifetime checks for
unbound/null tokens, premature completion, repeated/reentrant results, supersession,
old cancellation, profile replacement, failure cancellation, foreign bus tokens,
clear and fresh attempts. Eclipse.Runtime compiles. No native completion hook or
public battle-result callback is connected yet; API remains 0.39. The next step
must wire actual encounter entry/result capture before this can prove runtime
exactly-once notification. Full G02/G13 and deferred lottery settlement remain open.

## Native encounter observation wiring

FightList now retains an internal transient ModStoryEncounter token. GameUtils
StartFight creates it after ModModeRuntime.Begin accepts entry and before scene
launch or the existing instant-win path. Failed ModuleFight launch cancels that
exact token; it cannot cancel a newer nested attempt. EndFight reserves the token
after the mode resolution gate and before reward construction/callbacks, then
completes it at the successful end of the native method. Exceptions cannot reach
completion; a reservation cannot be reused for a later duplicate observation.

This intentionally guards observation only: it does not skip native processing
or change reward grants when a token is absent/rejected. Direct result paths that
never went through StartFight currently have no token. Profile unbind clears the
bus identity, so a retained FightList token cannot complete in another profile.
No saved fields, Unity assets or GUIDs changed. All four managed projects compile
with the previously documented process-local optional Git metadata workaround.
The 56 transport checks cover token semantics; full native launch/EndFight execution
is not yet fixture- or game-tested. No public result event is exposed yet (0.39).
Next work remains capture of outcome/encounter/loadout identity and delivery through
Lua, including timeout distinctions and a clear deferred-lottery settlement limit.

## API 0.40: battle result story callbacks

Added battle_result to story.on. GameUtils.EndFight captures a detached outcome
snapshot after reserving its tracked launch token and before reward callbacks,
then publishes only after successful native completion and token consumption.
The snapshot resolves the catalog fight identity, roster Eclipse flag and available
player ModelParameters.PJNJIJIODHE equipment (including its native Skeleton slot).
Unknown IDs remain nil. Player parameters absent on surrender/fallback paths do
not trigger an invented profile-equipment substitute. Win, loss, surrender, raid
and raid-round timeout remain distinct. GAME_OVER_NONE is not published.

Each Lua subscriber receives new nested tables. Equipment records are immutable
host snapshots and list storage is copied. The event cannot change the outcome
and does not certify deferred lottery settlement or disk-save completion. Direct
result paths without tracked StartFight entry remain outside current delivery.
Native duplicate reward processing is not changed by an observation guard.

TestBattleResultCapture passes 12 extracted production capture-method checks with
controlled native/catalog services; TestStoryEvents passes 56 transport/lifetime
checks; TestStoryApi passes 56 actual Lua checks, including nested snapshot
isolation and the shipped Story Observer battle log. An initial test expectation
incorrectly classified the new successful scenario as an error case; corrected
that expectation after confirming all callbacks completed without errors.

All four managed builds, editor generation/check, 24 project tests, LuaLS payload
completion, eight VS Code checks and wiki build pass (135 reference entries,
169 typed structures, 48 pages, 4,030 links/assets). Public docs, editor contracts,
example manifest/script and guide updated together. Builds use the previously
recorded optional Git metadata-query workaround. Full native StartFight/EndFight
execution, Eclipse/timeout/instant-win and lottery acceptance remain unverified.
G02/G04/G13 and the broader goal remain open.

## Complete native battle-result flow fixture

Confirmed FightScene.Init -> GameUtils.ABAIHGFPHMO -> Fight construction retains
its FightList reference; normal Fight.EndFight forwards that same object to
GameUtils.EndFight. Added TestBattleResultFlow/ValidateBattleResultFlow, extracting
the entire production GameUtils.EndFight method rather than rebuilding its branch
sequence in the test. Reward, quest, presentation, mode and capture collaborators
are controlled; ModStoryEvents token/delivery implementation is production source.

All 12 flow checks pass: capture precedes calculation and delivery follows grants/
presentation; duplicate and reentrant native results emit once; grant/presentation
failure and profile replacement emit nothing; failed attempts cannot retry delivery;
surrender skips native grants; lottery wins defer legacy quest result while still
observing the outcome; mode gates prevent processing; untracked results retain
native grant behavior without a mod observation. Payload mapping itself remains
covered by the separate extracted capture fixture. No runtime code changed.

This improves verification of native integration but is not a Unity playtest or
proof of real reward/save/presentation services. StartFight launch execution,
full-game input and deferred lottery settlement remain open. API stays 0.40.

## Runnable equipment-conditioned achievement

Added example.katana-achievement using only public API 0.40. Its owned Blade
Discipline achievement advances once for a win against the exact normal/Eclipse
Butcher fight 6, or the intermission gauntlet fight 1, with captured Weapon/Katana
equipment. Canonical stages confirm those fights contain Butcher_Backswords and
normal/Eclipse fights 1�5 are bodyguards. Canonical WEAPON_KATANA confirms subtype
Katana. The existing core Butcher achievement sprite is referenced, not replaced.

TestKatanaAchievement executes the shipped manifest, localization and Lua and
passes 17 registration/predicate checks, including all three qualifying routes,
repeat suppression and nonqualifying results. The core sprite provider and counter
service are controlled; the test also confirms the referenced core asset exists.
TestPhase3Progression separately passes 14 production native parser/adapter/save/
reload checks. These are separate proofs, not full end-to-end persistence of the
new example. An initial fixture compile assumed collections were indexable; fixed
to use Single() for the single registered definitions.

Editor generation/check and all 25 project tests pass, including the new example.
Wiki builds 48 pages and validates 4,036 local links/assets. Added public achievement
condition guide and example listing. No engine API/runtime change; 0.40 remains.
The example has no currency reward, guaranteed popup, unlock bypass or fight patch.
It demonstrates G13's equipment predicate but does not port the archived DE reward/
presentation or close broader achievement/core-counter gaps. Full-game Profile
rendering, captured rule equipment and this mod's save/reload remain pending.

## Lottery slot recovery prerequisite

Traced RewardLottery and MANJCIGJPMK against all seven archived lotteries. The
154 slots all specify Weight (values 1,3,4,5,8,10,12,15), but the recovered slot
constructor did not read it. Slot reward, image, cancelling-item and view-type
fields had no usable consumer access; the level eligibility method was private.
QuestActionDialogLottery remains a no-op completion, not a recovered draw loop.

The slot now retains Weight using the same ParseFloat(1f) convention as the native
RewardChoice parser. Internal read-only image/cancelling-item/view-type properties
and TryEvaluateAtLevel expose existing data and inclusive/unbounded eligibility;
RewardLottery exposes its stored type internally. No selection algorithm, inventory
cancellation rule, UI, spin price or reward grant was invented or enabled. These
accessors are host-only and API remains 0.40. Canonical/shared economy is unchanged.

TestLotterySlots compiles the actual recovered slot/lottery classes and passes
177 checks, including all 154 archived slot metadata records, type/count retention,
bounded/open-ended eligibility, default weight, uninitialized slots and composition
copy metadata. Reward evaluation itself is controlled in that fixture; native
Reward parsing/selection is covered separately by TestRewardNative. Assembly-CSharp
compiles with the existing optional Git metadata-query workaround. No Unity import
or full-game lottery execution occurred. G04 still needs eligible draw policy,
settlement ownership/save handling, native-styled UI and deferred quest resumption.

## Weighted lottery selection foundation

Added host-only ModRuntime.TrySelectLotterySlot using recovered slot weights and
level eligibility. It accepts an explicit unit sample [0,1), snapshots the source
slot list, applies a caller eligibility predicate once per positive eligible slot,
then selects from renormalized weights with half-open boundaries. Zero weights
cannot win; negative/nonfinite weights and invalid samples are rejected. Empty
eligible sets return false. No unselected Reward is evaluated and no global RNG,
inventory, currency, save or presentation state is touched. CancellingItem remains
available to the future caller's ownership policy rather than being guessed here.

TestLotterySlots now passes 191 checks, adding interval boundaries, zero weights,
filtered renormalization, predicate call counts, empty sets, level bounds and
invalid weight/sample cases to the archived slot audit. The selector method is
extracted from production source; reward evaluation remains controlled. Main game
assembly compiles with the existing optional Git metadata-query workaround.

This adds no public API and changes no live lottery draw behavior yet. API remains
0.40. G04 still requires caller integration, guarded item/enchanted reward settlement,
profile/save lifetime, native-styled UI and deferred quest continuation. Unit-sample
selection is an implemented host mechanism, not proof of archived interactive
lottery timing, reroll economy or full-game parity.

## Selected lottery reward evaluation

Added host-only BuildLotteryPrize: evaluate one level-eligible selected slot and
construct a native FightResult.ResultPrizeStruct. It transfers the slot's direct
money/bonus/experience fields and delegates item, money, currency, resistance and
choice payloads to the existing native result methods. It does not reapply fight
performance bonuses. Nested lotteries are retained explicitly for a later workflow;
no inventory mutation, UI, save or settlement call is made by this builder.

TestRewardNative now compiles the actual MANJCIGJPMK instead of its former stub
and extracts the production lottery builder. All 65 checks pass. New assertions
cover scalar reward preservation, forwarding non-item reward types, retaining item
enchantment payloads, retaining nested lotteries, rejecting out-of-range slots and
no inventory grant. The fixture's currency/money services and PerkStruct parsing
remain controlled; native reward/item constructors and selection are extracted as
before. This is not proof of actual enchanted-item settlement. Main game assembly
compiles with the existing optional Git metadata-query workaround.

API stays 0.40. G04 still needs single-use settlement ownership, profile/save
semantics, cancelling-item policy, presentation and deferred quest continuation.
The archived cosmetic slots also use UpgradeLevel expressions whereas native
RewardItem explicitly reads UpgradeNumber; exact translation/level semantics need
further recovery before claiming cosmetic lottery parity.

## Live lottery claim ownership

Prepared lottery claims now capture the active roster/generation and build their
selected result once. An internal LotteryClaim rejects stale/repeated/reentrant
claims and refuses unresolved nested lotteries before granting. It consumes itself
before invoking native ListSF.IMDGMNFHFCN, then requests the originating roster's
save only if profile identity/generation still match. The native bool return is
level-up status, not success; false still completes a grant. Native exceptions or
mid-grant profile replacement leave the claim consumed to prevent blind retries.

TestLotteryClaim extracts the production claim/preparation code and passes 17
lifetime checks with controlled selection, grant and save services. Cases cover
no preparation grant, fixed draw, repeated/reentrant claims, foreign/rebound profile,
failed grant, unresolved nested lottery, empty pool, profile changes during prepare/
grant and unbound profile. Assembly-CSharp compiles with the existing optional Git
metadata-query workaround. No public API or live quest/UI consumer is enabled.

Save tracing confirmed ListSF.EJANJEEGOOE marks GJEJCLBAPMP for later OnAuthenticate,
and MELBIBHDPCE.GGGEHAGCLGC invokes/queues a save-required event. Neither establishes
disk durability. This is an in-memory at-most-once guard, not crash-safe entitlement
settlement. Mid-grant failure may already have native partial effects; the guard
does not roll them back. Durable prepared results, atomic/recoverable grant state,
native callback isolation, quest resumption and original-style UI remain G04 work.
API remains 0.40 and no full-game lottery claim was executed.

## Lottery notification boundary

Added host-only ModStoryEvents.RunDeferred and applied it around a lottery claim's
native grant plus save request. Notifications buffer per nested operation and
flush in FIFO order only after success. Failed inner batches discard their own
notifications; a failed outer operation discards the whole buffered batch. Profile
unbind invalidates old batches without restoring stale parent state. The existing
128-event and callback dispatch budgets remain shared through deferred/nested
processing; no per-publication budget reset permits unbounded buffering.

TestLotteryClaim now uses production ModStoryEvents and passes 23 checks, including
acquisition callbacks seeing the finished save-request boundary, rejected reentrant
claim from a deferred callback and no acquisition delivery for a failing bundle.
TestStoryEvents passes 65 checks, adding nesting/FIFO, failed parent/child, profile
replacement, capacity and independent-dispatch reset checks. TestStoryApi passes
56 actual Lua checks. All four managed projects compile using the existing optional
Git metadata-query workaround.

RunDeferred is notification isolation, not native rollback or durable settlement.
A native exception can leave partial inventory effects, and the live claim remains
consumed. Save requests do not prove disk persistence. No live lottery quest/UI or
public API consumer is enabled yet; API remains 0.40. Durable draw/claim recovery,
partial-mutation recovery and native-styled UI remain open G04 requirements.

## Profile save boundary characterization

TestProfileSaveBoundary.ps1 passes 15 disk-backed checks using extracted production
ListSF.OnAuthenticate and XmlUtils.ONLDJNLKKAL plus the full UserDataValidator and
MD5Utils implementations. Roster serialization, device identity and the settings
switch are controlled; file locking forces real write failures under Temp only.
Both hash-disabled and hash-enabled runs show that a failed second-copy write
leaves a new primary and old backup, with the dirty flag retained. With hashes
enabled, a failed primary hash write leaves new XML with its old hash; validation
rejects it and the old backup remains valid. Explicit retry repairs these cases.
This characterizes existing behavior; it does not implement crash recovery.

Source tracing also found GameSettings.IIGOJINCIIF restores users_backup.xml only
when loading primary returns null. A CheckFileHash exception propagates instead
of returning null. GameSettings currently initializes/resets hash checks to false;
no enabling assignment was found. ListSF.PBNNPBEDOOJ subsequently loads primary.
The backup is a second sequential current-state copy, not a transaction journal.

Consequently durable lottery claims must commit their selected result/claim state
with the corresponding inventory snapshot through a recoverable profile boundary.
An independently saved consumed flag, or merely flushing GGGEHAGCLGC's save event,
cannot establish exactly-once settlement. XML/hash pair handling must preserve
validation if enabled, and recovery must distinguish incomplete writes from a
committed generation. Native partial-grant failure remains a separate problem.
No profile writer, settings or player save was changed by this investigation.

## Legacy reward encoded upgrade levels

RewardItem now retains UpgradeLevel independently of ordinal UpgradeNumber and
uses its existing FunctionExtension player callback to evaluate the expression.
Both attributes together are rejected, including an explicit UpgradeNumber=0.
Encoded results must be integers. FightResult evaluates the ordinary level once,
then resolves an explicit encoded upgrade through ItemInfo.HIOBANJPMKF, the same
lookup used by QuestActionGiveItem: first native upgrade whose encoded level is
at least the request. Negative/unavailable levels fail before adding that item
to the selected result. Ordinal clamping and owned-item suppression are unchanged.
This does not make the whole reward bundle transactional.

TestRewardNative passes 69 parser/composition/selection checks, with controlled
item catalog/upgrade lookup and expression values. TestRewardExpressions passes
5 checks using the full production RewardItem and compiled native FunctionExtension:
Player.Level*100 at levels 1, 4, 40 and 52, and fractional-result rejection. Roster
and scalar conversions are controlled. TestModConsumableRewards passes. Game and
editor assemblies compile; the wiki builds 48 pages and validates 4036 links.
No full-game reward settlement or cosmetic lottery playtest was performed.

The public Lua reward schema is unchanged (API 0.40); legacy XML compatibility is
documented in content-graph.md. This removes the previously ignored UpgradeLevel
payload gap, but does not prove every archived item's upgrade table has a matching
entry. Lottery durable recovery, UI, cancellation policy and quest resumption
remain open. No DE content was activated or ported.

## Native encoded-upgrade lookup and archive projection

TestRewardUpgradeLookup.ps1 extracts production ItemInfo.FMHIKMNJHDL,
DNFDAGFAANJ and HIOBANJPMKF. Six checks pass, covering exact local selection,
next-higher selection, above-table failure, sorted local/template merging, ordinal
level filtering and nonempty archive inventory. Item materialization and catalog
storage are controlled; this is stronger lookup evidence than the LINQ stand-in
in TestRewardNative, but is not full native item import.

A raw projection of archived local/template rows finds all 118 Slot/Item entries
with UpgradeLevel, all using ?Player[].Level*100, and no missing named items.
Probing levels 1..52 yields 5,894 exact encoded matches, 242 next-higher matches,
and zero unavailable lookups. This intentionally probes beyond each slot's level
eligibility. It does not prove effective item attributes, inheritance, presentation,
or full-game settlement. The next-higher results confirm that the public legacy
compatibility note must retain native >= semantics rather than promise exact
encoded matches. No archive or runtime source changed in this verification step.

## Recoverable profile snapshot writes

Added host-only ModProfileWriteJournal and connected it through XmlUtils for the
exact users.xml/users_backup.xml paths in the profile directory. Other XML writes
retain their prior path. A bounded, versioned, checksummed record stores snapshot
and optional hash bytes before replacements; flushed same-directory temporary
files install each destination. Pending records survive a failed install and are
validated/replayed by AIFIAKNJMHG before normal profile loading/hash validation.
The checksum detects corruption; the native UserDataValidator hash policy remains
the authenticity check. Its existing check was factored to accept a snapshot hash.

An exclusive sidecar lock coordinates journal access. Native reset and profile
replacement discard pending records before deleting the old profile, preventing
recovery from resurrecting it. Null hashes preserve the existing sidecar, matching
hash-disabled native saves. New source/meta identity is project-owned; existing
Unity GUIDs were preserved. No Lua filesystem API or API version change.

TestProfileWriteJournal passes 22 disk-backed checks, including failed first/hash
replacement, fresh-process replay, invalid-host-validation rejection, corruption,
concurrent access, discarded writes and temporary-file cleanup. The updated
TestProfileSaveBoundary passes 17 checks using extracted production ListSF save,
XmlUtils read/write, owned adapters and full native hash implementation. It verifies
recovery through the production reader before hash validation. Roster serialization,
settings and device identity remain controlled. All four managed assemblies pass.

This is now connected runtime behavior, not an unused helper. It does not make
primary/backup copies a single transaction, persist prepared lottery rewards, roll
back native mutations, or guarantee power-loss durability on every filesystem.
Full-game reset/restart acceptance remains pending. G04 stays open for durable
claim state, settlement failure handling, quest continuation and styled UI.

## Persisted evaluated lottery claims

Added ModLotteryPrizeCodec for evaluated result scalars, selected item identity/
level/encoded upgrade, native and mod enchantment payloads, currency and resistance
records. Resume resolves stored identities and rejects unavailable/changed item
upgrades; it does not execute reward selection or expressions again. Nested draws
must be resolved before encoding. Native resistance granting is an empty loop in
ListSF, so claim preparation/application rejects resistance prizes rather than
marking an ungranted reward complete. The codec retains their representation for
future native support.

PrepareLotteryClaim now writes one versioned EclipseLotteryClaim under the bound
warrior and forces the profile save before returning. An existing pending draw is
resumed rather than replaced. ResumeLotteryClaim rebuilds the selected result from
its persisted payload. Claim application defers ListSF.OnAuthenticate while native
grants run, writes the claimed marker, then forces the final inventory/claim save.
Profile bind/unbind is rejected during settlement. On failure the save gate remains
closed until a new profile binds; unloading alone does not reopen it. This avoids
later autosaving a partial native grant over the recoverable prepared snapshot.
It is fail-stop behavior, not in-memory rollback or a UI recovery experience.

TestLotteryClaim passes 32 production claim/preparation/resume/save-gate checks
with controlled payload codec, native grants and disk persistence. New cases cover
save-before-return, no reroll, XML reload, completed marker, stale handles, native
callback save deferral and failed-grant save blocking. TestLotteryPrizeCodec passes
5 checks with actual RewardItem/PerkStruct/native result types; ItemInfo is allocated
without its Unity constructor and only identity fields are populated. It covers
scalar/item/perk roundtrip, missing items and unknown versions. Currency/resistance
codec roundtrips and full native catalog import are not established by that fixture.
TestProfileSaveBoundary passes 17 checks after adding the save gate. Game/editor
assemblies compile; wiki builds 48 pages and validates 4039 links/assets.

No public Lua API/schema change (0.40). No live lottery UI/quest caller is enabled
yet. Quest continuation/source ownership, styled selection/claim UI, native grant
integration and full-game restart/failure acceptance remain necessary before G04
is closed. The persisted-helper tests do not prove those broader requirements.

## Lottery dialog presentation lifecycle

Added ModLotteryDialog, an owned modal surface using the existing ModUiView game
font, parchment and button skin. It provides a scrollable reward description,
Claim and Later, disables repeated claim input, and preserves an error/reload
message when settlement fails. It accepts host callbacks; only successful claims
invoke completion. Back/Later, scene teardown and disposal defer unclaimed work.
A scene close during a synchronous successful grant delays close notification
until the result is known, preventing loss of the completion callback.

TestLotteryDialog passes 14 production dialog/ModUiSurface checks with Unity
mounting controlled: success, repeated/reentrant claim, Later, unavailable claim,
exception, scene close before/during success and failure, and disposal. Game/editor
assemblies compile. No Unity screenshot/native input playtest was performed. The
layout inherits the existing skin but still needs actual visual acceptance and
reward art/localized labels before finished lottery presentation is claimed.

Quest tracing found a separate replay boundary: QuestActionsSequence advances its
in-memory index on completion, while QuestStage resumes from RosterQuest's saved
checkpoint index. A claim can already be saved as complete before that quest
checkpoint advances. Connecting DialogLottery without a persisted invocation key
would allow a resumed action to create another draw. The dialog is not wired to
that stub yet; quest invocation identity and completion acknowledgement must be
connected before enabling the live flow. No public API/schema changed.

2026-09-12: Connected lottery invocation bookkeeping to native QuestStage start,
resume and completion boundaries. ModRuntime lazily keys saved quest runs by file
and name, resumes unfinished runs, rejects changed action definitions and stale
profile handles, and prevents completion while a draw remains pending. Native
quest completion requests a save after recording completion. Concurrent and
nested lottery action identities remain unsupported; DialogLottery is still a
stub and no playable completion claim is made. TestLotteryClaim now passes 51
checks, including interrupted quest receipt replay, new completed runs, definition
changes and profile replacement. Assembly-CSharp managed compilation passed.
Public Lua API remains 0.40.0; save-compatibility documentation updated. Unity
playtest remains pending.
