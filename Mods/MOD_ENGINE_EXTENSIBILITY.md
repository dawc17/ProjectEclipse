# Eclipse as a general Shadow Fight 2 mod engine

Review date: 2026-09-11. Baseline: API 0.7.0; this change adds API 0.8.0 battle
behaviors. Phase 4 (the downstream DE port) remains deferred pending its assets.
This is an engine-wide extension of the parity roadmap, not a claim that DE or
all engine domains are complete. Missing DE art does not block the work below:
prove mechanics with core assets and purpose-made minimal fixtures.

## Assessment

The foundation is useful: ownership, dependency ordering, transactional
registration, typed asset handles, a Lua sandbox, mod-owned persistence,
instance schemas/migrations, content fingerprints and recovered runtime adapters.
Keep it. The largest limitation is that the public vocabulary often describes
what the recovered XML can represent, while runtime control is concentrated in
perk/enchantment callbacks. Adding more registration tables alone will not make
fully custom modes, characters or UI possible.

The old phase showcases prove selected vertical slices. They do not establish
full field coverage, unrestricted behavior, or full-game acceptance. The existing
`DE_XML_API_GAP_AUDIT.md` G01–G14 remains applicable. This review expands beyond DE
and prioritizes author experience and reusable engine services.

## Coverage against creator requests at the original review

| Domain | Implemented foundation | Missing for genuinely custom content |
| --- | --- | --- |
| Battle rules | Typed equipment/attribute/native rules; 0.8 adds direct Lua behavior attachments | Explicit round outcomes, timer control, scoring, fight clock/tick, action restrictions, custom rule presentation |
| Modes | Ordered owned-fight sequences, availability, tickets, replay and progress | Lua-driven branching/selection, resumable runs, generated encounters, mode lifecycle and custom result screens |
| Characters | Warrior templates, equipment, levels, tactics, perks and assets | First-class reusable character identity/controller; playable selection/loadout; stance and form changes; authored rigs/skins |
| Combat | Round/fight, resolving/resolved damage, block/critical callbacks; health, charge, mitigation and shield methods | Rich immutable hit/contact snapshots, motion/grounding/range queries, outgoing hit policy, status effects, projectiles, grabs/throws, ability activation |
| Animations/moves | Native animation binaries, move/template/interval definitions and limited triggers | Published asset spec, authoring/export pipeline, input bindings/combos/cancel windows, hit/hurt volumes, playback capability and animation events |
| AI | Native tabular/random tactics and scoring factors | Bounded decision callbacks, perception snapshots, typed intent/action requests, telegraphs and fallback behavior |
| UI | Recovered quest dialogue, notifications, map entry and service visibility | General mod-owned screens/HUDs, layout, widgets, input/focus, reactive updates and lifecycle cleanup |
| Story | Typed quests and recovered condition/action adapters | Event subscriptions, progression/inventory/equipment queries, asynchronous dialogue/choice results and procedural quest logic |
| Worlds | Static layered locations, spawns, music and sprite assets | Animated scenery, hazards, trigger volumes and supported camera/lighting controls |
| Equipment/sets | Five equipment categories, forge families, perks/enchants, set registration | Non-economic metadata patching, default effects, activated set abilities and level-scaled parameter profiles |
| Assets/audio | Namespaced lookup, sprites/textures, models, binary/audio references, asset redirection | Rig compatibility validation, author previews, animation conversion tools, controllable audio instances and presentation effects |
| Progression | Owned state/migrations, achievements/counters, rewards, sequence progress | Context-rich progress events, owned inventory operations, resumable custom mode state and save version diagnostics |
| Interoperability | Dependencies, ownership and deterministic targeted conflict checks | Public mod-service exports, typed extension points, subscriptions/unsubscription, explicit composition of runtime policies |
| Developer experience | Wiki, LuaLS definitions, VS Code starter/validation, runtime test harnesses | Mod reload in safe contexts, in-game diagnostics/inspector, callback trace/profiler, replayable fixtures and asset pipeline errors |

Source evidence: `MoonSharpScriptRuntime.cs`, `MoonSharpScriptRuntimeP2.cs` and
`MoonSharpScriptRuntimeP3.cs` are the actual binding inventory;
`ModContent*.cs` and `ModScripting*.cs` define contracts; `LegacyContentAdapter.cs`
projects static definitions; `Fight.cs` supplies combat dispatch;
`ModModeRuntime.cs` consumes fixed sequences. The public rules, events-and-modes,
quests, moves-and-tactics, locations-and-locales and fighter pages describe their
limits. The original review had no general custom UI or programmable AI binding;
subsequent delivered API changes are recorded below and in PRE_DE_WORK_LOG.md.

## Design decisions

1. **Separate definitions, behavior, and live instances.** Definitions identify
   characters, rules, moves, modes, widgets and assets. Reusable Lua functions
   implement decisions. Instances own state, subscriptions and cleanup. Do not
   encode branching/arithmetic in action tables. Preserve existing XML adapters
   as compatibility paths, with no obligation to imitate them for new domains.
2. **Use domain capabilities, not raw engine objects.** A fighter may request an
   action or query its state; a screen may update its widgets; a run may select a
   validated encounter. None exposes `Model`, `Roster`, `GameObject`, reflection,
   filesystem access or arbitrary method names. Keep capabilities discoverable in
   completion and report missing capability at the actual operation.
3. **Make rules a first-class host.** A rule must not need a fake enchantment,
   player inventory entry or injected learned perk. Version 0.8 implements this
   part, reusing the tested combat dispatcher and behavior schema/lifecycle.
4. **Read snapshots, request changes.** Event snapshots are immutable observations.
   Intentional changes use named methods with documented timing and results.
   Do not let assignment to a Lua event field appear to mutate the engine.
5. **Publish composition semantics.** Read-only notifications run in stable order;
   bounded numeric modifiers use a documented pipeline; exclusive ownership of
   outcome/camera/input authority has a conflict diagnostic. A second mod must
   not silently win because it happened to load later. Explicit dependencies may
   order compatible overrides. Label order-dependent custom rule interactions.
6. **Keep lifetime explicit.** Callback fighter handles expire immediately;
   screen/run/effect handles last until their owning instance closes. State is
   declared as callback, round, fight, run or profile state. Only supported data
   is persisted. Never save closures, UI objects, native handles or pending
   coroutines. Specify cancellation and what resumes after reload.
7. **One clock contract per domain.** Combat timers follow simulation steps and
   pause correctly; UI animation may follow presentation time. Expose seconds to
   creators, while preserving the native step semantics internally. Seeded,
   instance-owned RNG is required before procedural modes promise reproducibility;
   dependency ordering alone is not deterministic simulation.
8. **Keep the existing economy policy.** No new raw currency, core price or shared
   upgrade mutation. Allow owned counters/resources for modes and abilities;
   distinguish them from base currency. New reward APIs use validated host-owned
   settlement paths. A broader engine does not silently repeal this constraint.
9. **Make errors useful.** Include mod, definition, callback, field path and
   applicable timing in errors. Reject unsupported options; never silently ignore
   them. Offer structured inspect/trace tools before adding large opaque surfaces.
10. **Budget the whole frame as well as each callback.** Current Lua bounds are a
    useful start. Tick handlers, UI updates, spawned effects/projectiles and many
    simultaneous rule instances need aggregate limits and visible diagnostics.
    Limits should be documented and measured, not arbitrary hidden truncation.

## Custom UI architecture

Build an Eclipse-owned UI runtime under `Assets/Scripts/Eclipse/UI/Modding`, with
Unity UI as the initial rendering backend. Avoid public prefab internals, CSS
emulation, a browser runtime, or unrestricted component construction.

A small typed layout tree is appropriate for static presentation: row, column,
stack, scroll, text, image, button, toggle, slider, progress and list/grid. This
is layout data, not a new procedural language. Supply Lua callbacks for clicks,
selection and value changes. A live screen handle updates individual widget
values/visibility and closes the screen. Stable widget IDs make updates cheap;
virtualize large lists instead of rebuilding the entire tree each frame.

Start with three owned mounting points: mod menu screen, modal overlay and
combat HUD overlay. Define their bounds, input priority and lifetimes. Use safe
areas and reference units, anchors, intrinsic/min/max size, clipping and text
wrapping. Default to the game's font/theme, but allow validated namespaced fonts,
sprites and colors. Dynamic text must support localization arguments. Keyboard,
controller and pointer navigation are part of the first implementation, including
focus restoration, Back/Escape and the distinction between decorative and
input-blocking overlays. Combat pause must be an explicit operation/authority,
not an accidental consequence of opening a panel.

Show a plain Lua view-model example: a mode resource changes, the HUD label and
bar update, and a button invokes an ability callback. Do not invent a string
expression language such as `visible_when = "state.energy > 0"`. The callback
can compute visibility and call a typed setter. Cleanup removes subscriptions,
widgets, input capture, sounds and scheduled work on close, scene exit, mod
shutdown or handler failure. No live UI handles are saved.

Acceptance: an interactive loadout chooser; a scrollable branching-mode lobby;
a combat meter with an ability button; localization and resizing; keyboard and
controller use; two mods with overlapping overlays; repeated open/close and scene
changes with no orphan widgets, blocked input or callback leaks. These are planned
contracts; no `sf2.ui` functions are announced by this change.

## Implementation order and exit tests

| Track | Work and dependencies | Required proof |
| --- | --- | --- |
| E1: behavior hosts | P1A.5 + P2A.1/P2A.3. Direct rule attachments now; then explicit effect/character/encounter host identity | Equipment-free rule, state isolated per rule and side, filter/lifetime tests, unchanged unmodded combat |
| E2: combat control | G06/G08. Hit/query snapshots, simulation clock, statuses, ability/action requests, result authority | A timed objective and custom win/loss rule; correct lethal/timeout/surrender order; no duplicated results/rewards; pause and recursion tests |
| E3: programmable story/run | G01/G02 + P2C. Events/queries/operations, then a persistent Lua mode controller | Branching roguelike trial with seeded choices, a loss route, safe save/disable/reinstall and one-time settlement |
| E4: owned UI | Extend P2B.3 beyond visibility flags; may proceed after lifecycle services | The three interactive UI fixtures above, focus/input and teardown checks |
| E5: character/animation pipeline | G05/G07/G08/G09 + P1D.4/P1D.5. Publish native format constraints before building exporters | A new move with timing/contact data, custom input/AI use, multi-stage boss/form, invalid rig rejected with actionable error |
| E6: world/presentation | G10 + P1D.2/P1D.3. Animated layers, audio instances, hazards, camera intents | Arena with timed hazard and telegraph, pausing correctly and unloading cleanly |
| E7: breadth and patching | G01/G04/G11/G12/G13/G14. Targeted content collections, metadata, conditions, services | Composition tests with two mods; disabling restores base behavior; missing mods preserve owned saves |
| E8: authoring and stabilization | Tooling, diagnostics, compatibility policy, versioned examples across E1–E7 | A newcomer builds a rule, mode, character and UI without editing recovered source or learning obfuscated names |

E2/E3/E4 should share cancellation, authority and instance lifetime foundations.
Do not create three unrelated callback/state schedulers. Deliver small complete
vertical slices in this order; keep later domain designs provisional until their
actual engine seams are recovered. Third-party skeletal formats, animation
retargeting, extra simultaneous fighters/team combat, rollback/network play and
arbitrary renderer replacement each need dedicated feasibility work. They are
not implied by accepting a new character or binary asset ID.

## First delivered slice and limits

API 0.8 exposes `sf2.rules.behavior`. Its state is isolated by rule and fighter;
mode/round/target filters apply at dispatch; rule parameters enter content
fingerprints; saved-lifetime behavior is rejected. Static projection omits these
rules from native XML rule execution. `Fight.DispatchEclipseCombatEvent` and
`DispatchEclipseOpponent` dispatch them before each side's existing perk handlers.
No fake inventory/perk entry is made. Lua operations keep existing capabilities
and scope expiration. A rule error is isolated; previous host operations remain
applied. Shield keys include the rule/fighter instance identity through `ModInstanceFighter`.

`Mods/example.battle-rules` provides an equipment-independent third-hit guardian
using core art. It is a generic engine fixture, not the DE port. This addresses
part of E1; it does not satisfy custom outcome, tick, AI, UI, or full rule-editing
exit criteria. At this API 0.8 baseline, core fight patching could not change rule lists; API 0.10 now adds append/replacement. Phase 4 remains
unstarted, and no absent collaborator assets have been replaced or invented.

Validation results belong in the task handoff; managed/native fixtures are not a
substitute for a full encounter playtest. Before stabilizing 1.0, every promised
public domain needs definition and runtime consumption, namespace/dependency
checks, deterministic composition, lifecycle/missing-mod/save semantics, tooling,
current wiki pages and an end-to-end creator fixture.

## Verification of this slice

- All four managed project builds passed (Unity 2022.3.62f3 references).
- `Tools/TestBattleRules.ps1`: 92 checks, including actual MoonSharp execution,
  transactional rejection, target/mode/round filtering, per-rule/side/round/fight
  state, pending damage, capability enforcement and parameter fingerprints.
- `Tools/TestP2ACombatRuntime.ps1`: existing behavior/migration/capability and
  mode/progression regression fixture passed; its prerequisite content showcase
  fixture also passed.
- `Tools/TestUnderworldRuntime.ps1`: 1,282 assertions passed. The separate
  `AuditUnderworld.py` still reports missing loose raid-location images; it is not
  a clean asset-coverage result.
- Editor definition generation/check and seven project tests passed. Actual
  LuaLS and isolated VS Code integration passed; these are authoring checks.
- Wiki build and function coverage/link/search checks passed.
- No full Unity encounter playtest was performed for the new battle rule.
  Native animation/rig, controller input and custom UI acceptance are future
  work; managed tests do not prove them.

## Follow-up: combat observations (API 0.9)

`fighter:snapshot()` now captures fresh health/max-health/bar count, arena model
position, the opposing fighter, and active-fight frames/seconds. The runtime
contract uses detached immutable C# values and copied Lua tables. Live queries
expire at callback exit; data already returned may be retained. Missing sources
return nil. Rule wrappers forward the observation source, so the same method
works through all existing combat behavior hosts. The Third Strike Trial now
loses its guard at one third health using this API.

Evidence: Model.KKMCHCNOHMB and ModelParameters.CIDCNCDFONA are the normalized
current/maximum pool; HealthBarCount supplies authored bars. Model.PLBNCDCFPML
returns model position. Fight.RenderFight increments fightTimeInFrame only while
round.processing; Fight initialization resets it, and existing elapsed-time
accounting divides by 60. The API reports this existing clock without changing it.

This is an E2 observation slice, not completion of combat control. No tick
subscription, animation state, action request, custom result authority or UI
binding is added. Those still require their own lifecycle and engine integration.

Verification for API 0.9: all four managed builds pass; the battle-rule fixture
passes 104 checks, including fresh snapshots, detached nested values, expired
queries, missing opponents/sources, clock conversion and the low-health rule
transition. The existing combat suite and 1,282 Underworld assertions pass.
Editor generation/check, seven project tests, actual LuaLS (including snapshot
return inference and the full updated example), and VS Code integration pass.
The wiki builds 41 pages with 98 public binding sections and 3,057 checked local
links/assets. The asset audit still reports missing loose raid images. No full
Unity encounter or pause/playtest was performed; the native observation adapter
is compile-checked and based on the engine sources cited above.

## Existing encounter editing (API 0.10)

Fight patches now append or replace rules and replace location/music. This closes
the direct-rule host gap for existing campaign content and part of G01. See
[the cumulative work log](PRE_DE_WORK_LOG.md) for verification and remaining work.

## Outgoing hit control (API 0.12)

Attacker-side callbacks and bounded scaling are now implemented before defensive
stages. Pending hit flags are observable. See PRE_DE_WORK_LOG.md for source-order,
Lua and example checks; native gameplay and the remaining E2 controls stay open.

## Native combo and style observations (API 0.13)

Behavior handlers can observe native combo changes/expiry and style rank
transitions. The combo-reserve example combines these observations with round
state, the combat clock and outgoing scaling for a timed bonus. Its expiry is
evaluated by subsequent callbacks; this does not supply tick subscriptions or
status UI. See PRE_DE_WORK_LOG.md for the complete checks and native playtest
limits. An eventual tick host needs explicit ordering around model updates and
round settlement, plus cached subscriptions rather than full profile scans on
each simulation frame.

## Active combat ticks (API 0.14)

`on_tick` runs before model/collision updates on each active combat frame, with
an explicit frame/seconds/delta payload. Native pause gates the simulation and
round processing gates clock advancement. Session handler subscriptions are
cached after registration freezes, and absent handlers skip parameter/state
resolution. Active equipment/perk eligibility is still read from the native
host to preserve in-fight suppression; this is not a cached mutable loadout.
The combo-reserve example now clears expired state on ticks. Native pause/round
gameplay and performance profiling remain pending. General asynchronous tasks,
UI lifetimes, status presentation and action/outcome authority remain separate.

## Owned UI foundation (internal)

An engine-independent scope/surface/tree model and Unity UI view now implement
bounded widgets, live updates, guarded button callbacks and deterministic
cleanup. Managed ownership tests and an isolated Unity play-mode fixture pass.
This is not a published Lua capability: mount/input coordination and script
lifetime integration are still required. See
[the implementation evidence](UI_RUNTIME_IMPLEMENTATION.md) for exact source,
supported internal primitives, native verification limits and remaining work.

The internal scene coordinator now implements modal/menu/HUD ordering, bounded
mounts, safe-area fitting, foreground input, Back, navigation ownership and scene
cleanup. Its isolated Unity fixture includes overlapping mod canvases. Game
entrypoints, native dialog/input routing and Lua lifetimes remain to be wired;
the public API version remains 0.14.

The game bridge now routes native dialog/Back/combat input ownership, preserves
closing-frame consumption, and supplies on-demand mounting. Script contexts own
UI scopes. Lua creation and handle methods remain the next integration boundary;
full-game physical input acceptance is still pending despite passing isolated
Unity bridge checks.

API 0.15 now publishes the initial owned UI creation/update/close contract and
a Charged Strike HUD with bounded click logic and fresh combat authority. The
public Custom UI reference and editor definitions describe the exact limits.
Full E4 remains open for native end-to-end acceptance, richer layout/assets/
localization/widgets, HUD controller focus and the broader creator workflows.

API 0.16 adds safe-area anchors/offsets to owned UI and connects the actual Charged Strike Lua example to the production Unity renderer in an isolated play-mode fixture. See [the UI implementation evidence](UI_RUNTIME_IMPLEMENTATION.md). Full-game acceptance and broader widget/mode workflows remain open.

API 0.17 adds dynamic localization reads for Lua/custom UI, with current-language selection, English fallback and pending/committed patch support. Charged Strike exercises English/Polish refreshes in the isolated Unity fixture.

API 0.18 establishes native game skin defaults for custom UI, per the user requirement, and adds bounded text/color styling. Future widget types must use the same original-game visual language by default. See the UI implementation evidence for native asset and preview coverage.


API 0.19 adds bounded Lua mode-result routing over registered fight rosters and
persists the selected step using existing mode storage. A standalone Branching
Trial demonstrates alternating short/full routes and loss routing. Native outcome,
reward and entry paths remain authoritative; malformed callbacks fall back.
Linear completion indicators are suppressed for these modes. This advances E3,
but does not deliver generated fights, seeded run services, asynchronous choices,
full custom lobbies/results or complete interruption acceptance. See the cumulative
work log for exact runtime/editor evidence and the remaining requirements.

API 0.20 adds deterministic random draws backed by ordinary declared integer
state fields. Integer ranges use bounded rejection sampling; both draw functions
require owned state read/write capabilities and resume from serialized state.
Seeded Trial connects this to actual Lua result routing over registered fights.
This supplies persistent seeded choices for E3; generated encounters, async
player choices, lobbies/results and transactional run settlement remain open.


API 0.21 adds bounded UI close notification after input/view teardown, with
explicit reasons and prevention of reopening UI during cancellation. Charged
Strike demonstrates canceling pending gameplay state on native HUD destruction.
This advances the E3/E4 lifetime foundation. Async mode entry, lobbies/results,
subscriptions and general cancellation services remain open.
