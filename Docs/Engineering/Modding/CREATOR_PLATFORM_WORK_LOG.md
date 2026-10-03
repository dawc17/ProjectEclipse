# Creator platform implementation record

The objective is Minecraft-style creative freedom for Shadow Fight 2: creators
should be able to introduce gameplay systems, share frameworks, compose packs,
and ship substantial new experiences through a documented, usable API. One
feature, a function count or a downstream content port does not meet that objective.

The existing G01–G14 and E1–E8 requirements in
[the roadmap status](ROADMAP_STATUS.md),
[engine extensibility](MOD_ENGINE_EXTENSIBILITY.md), and
[the implementation plan](DE_API_IMPLEMENTATION_PLAN.md) remain in scope. This
record adds evidence to those requirements; it does not replace their exit criteria.
Public documentation describes implemented contracts, with acceptance limits.

## 2026-10-03: typed framework services

Before this change, separate Lua mods could reference declared content but could
not publish reusable procedural functions to dependent mods. The new
`sf2.extensions` module allows a framework to publish a versioned request/response
contract and handler. Dependent mods call it during loading or supported runtime
callbacks. The framework retains its capabilities and state, while data is copied
between script contexts. This advances E7/E8 composition and ownership work.

### Source and routing

| Source | Responsibility |
| --- | --- |
| `Assets/Scripts/Eclipse/Runtime/Modding/ModExtensions.cs` | Declarative service definitions, transactional catalog registration and session-owned routing. Checks direct dependencies, exact version, active owner, re-entry and shared budgets. Independent of recovered assemblies. |
| `Assets/Scripts/Eclipse/Runtime/Modding/ModContent.cs` | Includes services in registration capacity, validation, commit and rollback. |
| `Assets/Scripts/Eclipse/Runtime/Modding/ModScripting.cs` | Supplies the session registry to each facade. Existing central capability guard also blocks reward-configuration calls. |
| `Assets/Scripts/Eclipse/Modding/ModScriptSession.cs` | Creates one registry, activates exports only after successful content commit, removes failed owners and disposes services with their session. |
| `Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeExtensions.cs` | Four Lua bindings, opaque script-owned handles, typed primitive records and bounded provider handler execution. `try_call` supplies recoverable errors in the hard sandbox. |
| `Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntime.cs` | Opens execution scopes for shared budgets, clears handles/exports on disposal and restricts service calls during state migration. |
| `Assets/Scripts/Eclipse/Runtime/Modding/ModSaveData.cs` | Appends sorted service contracts to fingerprints only when services exist. Existing content without services keeps its previous fingerprint. |

Schemas reuse the existing parameter contract: at most 64 fields, finite numbers,
exact safe integers, booleans and bounded strings. Native handles, nested tables
and closures do not cross contexts. Calls permit depth 8 and 32 routed attempts
per outermost Lua execution; each provider handler has a 200,000-instruction
budget. Providers can intentionally expose their own operations, so handlers
must validate domain constraints and authorize callers where necessary. Applied
changes are not rolled back across mods if a later call fails.

### Creator workflow

`Mods/example.focus-framework` owns a saved Focus resource and publishes status
and hit services. `Mods/example.focus-addon` declares its dependency, obtains the
two handles and uses the services in combat callbacks. It applies outgoing damage
with its own capability and updates a HUD. Every third positive, unblocked hit
receives half its pending damage as a bonus, capped at one normalized unit.
The framework can serve other add-ons without duplicating its state logic.

The public [service reference](../../Modding/src/content/docs/api/extensions.md)
documents every function and the handler, schema limits, dependency/version
rules, authority, failure behavior and saves. The manifest guide, examples,
sidebar, compatibility guide and VS Code guide are updated. The editor's
authored schema, generated definitions and copied framework/add-on templates
cover the same contract. Editor-only definitions remain outside executable mods.

### Verification

- `TestModExtensions.ps1`: 126 checks using production MoonSharp and `ModScriptSession`, with
  controlled asset-host/combat/UI inputs. Covers schemas, capabilities, versions,
  direct dependencies, detached data, failed-provider rollback, re-entry, runaway
  handlers, shared call/depth budgets, state migration, fingerprints and the actual
  two-mod example across save/reload. Its showcase prerequisite also passes.
- All four managed assemblies compile using ignored temporary project copies
  with references remapped to installed Unity 6.6 (`6000.6.0f1`). The tracked
  project files retain their original reference paths; direct standard commands
  hit local SDK/analyzer path issues. The two new source Includes are tracked.
- Editor generation/check/45 unit tests, LuaLS 3.19.1 protocol integration and real
  VS Code integration pass. Local LuaLS 3.18.2 fails initialization with a duplicate
  worker channel; it is not claimed as passing for this change.
- Wiki build, binding coverage, types, search index and local link checks pass.
  Build output is ignored; authored files and generated editor definitions are tracked.

At the first feature commit, Unity editor validation and a full-game playtest
had not been performed. The controlled combat test proves routed callbacks and HUD data,
not real contact ordering, rendering, menu navigation or native save acceptance.

### Remaining work toward the vision

Services do not supply arbitrary engine operations. Custom fight outcomes,
broader actor creation/control, general custom scene workflows, additional
profile/inventory operations, cross-mod events, modpack conflict tooling and
multiplayer extension policy retain their existing roadmap requirements.
Framework composition needs further pack/lifecycle and native acceptance.
Creator workflows should be demonstrated by independent mods, including their
failure, removal/reinstallation and save cases. The broader objective remains
active; this feature establishes reusable procedural composition only.

## 2026-10-03: native framework acceptance follow-up

Added `Tools/Tests/Modding/TestModExtensionsUnity.ps1`,
`ModExtensionsUnity.cs` and `ModExtensionsUnityStubs.cs`. The runner creates an
ignored fixture using the repository's matching Unity 6.6 editor/package versions,
the production runtime APIs, MoonSharp context, script session, HUD view/fade and
the actual shipped Focus mods. It imports canonical vanilla stage definitions
and copies the recovered font with its existing GUID. It runs with graphics and
exits only after recording acceptance evidence.

33 Play Mode checks pass: separate provider/add-on activation, native font loading,
HUD text and visible changing pixels, blocked-hit behavior, third-hit damage,
fight-end cleanup, actual fade destruction, XML write/read and removal/reinstall
state preservation. The `Focus: 2/3` screenshot was visually inspected. Screenshots,
profiles and logs remain in `Temp/ModExtensionsUnity-*`; they are not committed.

This strengthens verification of the existing feature; no public function or
capability was added. Public reference/example verification statements and the
test index now reflect that evidence. Contact notifications, asset-host construction
and unused artwork/scroll paths are controlled. Full-game contact ordering,
native menu/profile lifecycle, physical input and durable crash/restart acceptance
remain open, as do the wider creator-platform requirements above.

## 2026-10-03: declared offline round objectives

Adds reusable objective/result authority toward E2. A behavior rule declares
`controls_outcome = true`, requiring `combat.round_outcome`; its active combat
callback can call `fighter:end_round("win" | "loss")`. Results are relative to
the player and are provisional until the existing native round boundary.
No DE-specific policy, generic Lua operation DSL or direct reward grant is added.

### Source and execution

- `Runtime/Modding/ModRoundOutcomes.cs` supplies a recovered-type-independent
  typed host interface, exclusive controller validation and per-round request
  state. `ModContent.cs` validates attached controllers before catalog mutation;
  disjoint mode/round scopes can coexist. `LegacyContentAdapter.cs` validates
  generated encounter lists before returning projected XML. Selection checks
  in `ModBattleRuleInstances` defend the combat boundary too.
- `ModScripting.cs` associates result authority with a declared rule on its
  instance wrapper; `ModRuntime.cs` supplies that rule only for fight-rule dispatch.
  Ordinary rules, equipment and perks do not gain authority by sharing a behavior.
  `MoonSharpScriptRuntime.cs` enforces capabilities, exact outcomes and callback
  lifetime/timing, returning acceptance or a refusal reason.
- `Fight.cs` queues requests while an offline round is active, consumes them
  after native KO/timeout/end-rule arbitration, and uses the existing `EndRound`
  score/end-stance path. Surrender cancels pending requests. `GetWinner` retains
  the consumed result through end presentation; next-round setup resets it.
  Same pending requests are idempotent, contradictory requests are refused.
  No health mutation or recursive result settlement occurs in the Lua callback.
  Requests are transient; accepted side effects are not rolled back by a later
  callback error. Online raid, PvP/versus, training and title sparring refuse them.
- `ModSaveData.cs` fingerprints declared authority without changing hashes for
  ordinary rules. The new runtime source and `.meta` are tracked; recovered
  asset identities and original project reference paths are preserved.

### Creator workflow and verification

`Mods/example.hit-objective` uses typed round state, damage/tick observations,
a HUD and one controller: three positive unblocked hits within ten active
simulation seconds win; expiry loses without killing either fighter. It patches
Act I tournament battle 3 in normal and Eclipse modes. The editor ships the same
manifest/script starter. Public round-outcome/rule references, manifest,
compatibility, examples, editor guide, generated definitions, capability checks
and sidebar cover the implemented contract.

- `TestRoundOutcomes.ps1`: 702 checks using production MoonSharp/runtime APIs
  and extracted current native arbitration/score/winner/surrender methods.
  Covers invalid inputs, capability/instance isolation, conflict rollback,
  disjoint scopes, expired callbacks, native-result precedence, lifecycle guards,
  offline raids, generated encounter projection, queued pause/resume, ordinary
  native results, zero-health/death-completion precedence, detached fights,
  fingerprint distinction and both paths of the shipped example.
  Models, clock, end presentation and settlement are controlled.
- `TestRoundOutcomesUnity.ps1`: 634 isolated Unity 6.6 Play Mode checks using
  production script sessions, shipped objective, actual font/view/fade and
  rendered changing text. The completed `3/3` screenshot was visually inspected.
  Round arbitration/score/winner methods are extracted from current `Fight.cs`;
  model/contact/clock/end-presentation/settlement inputs remain controlled.
  Unity's search-index startup logged an unrelated `ArgumentOutOfRangeException`;
  the objective completed and wrote explicit acceptance evidence. The managed
  fixture also prints MoonSharp's default Unity loader reflection warning under
  .NET; production mod asset loading and all acceptance checks still pass.
- All four managed assemblies compile with ignored temporary project copies
  remapped to installed Unity 6.6 (`6000.6.0f1`). Standard tracked project paths
  have the previously recorded local SDK/analyzer mismatch and are preserved.
- Editor generation/check/build, 46 unit tests, LuaLS 3.19.1 protocol tests and
  real VS Code integration pass; wiki coverage/types/build/search/link checks pass.
- Underworld runtime: 1282 assertions pass against freshly built game assemblies.
  `AuditUnderworld.py` still reports the previously recorded missing
  `fungus_raid` `/layer_0_2`; no location assets were changed.

This establishes a bounded round-objective contract. E2 stays open for broader
combat control and full-game acceptance. Actual contact ordering, complete native
end-animation selection, full result/reward/save lifecycle, removal/reinstallation
in a real profile and crash/restart acceptance are not proven by these fixtures.
The wider creator-platform goal remains active, including broader actor/scene
creation, pack conflicts/composition and independent creator workflows.

## 2026-10-03: additive fight rules across mods

Before this change, two independent `append_rules` patches to one fight conflicted
even when their rules were compatible. This prevented the shipped Focus add-on
and round-objective examples from running together. Additive contributions now
compose; replacements and conflicting result authority retain explicit rejection.
This advances E7/E8 without closing their broader requirements.

### Source and API contract

- `Runtime/Modding/ModContent.cs` gives fight rules an append/replace field policy
  without granting removal, records `append_rules` as `Append`, and accepts only
  compatible append/append overlap. The final immutable fight definition retains
  contributor order and every patch record. Exclusive replacements report all
  existing contributors. Duplicate handles, one-field-per-transaction and the
  aggregate 100-handle limit still reject before any catalog mutation. Existing
  controller validation checks the combined list before commit. Failures discard
  the incoming mod's entire transaction and do not poison later independent mods.
- `LegacyContentAdapter.cs` projects each `(target, field)` once. This is essential:
  each patch record references the final combined list, so applying all records
  separately would multiply static native rules. Original battle clones and the
  existing restore path remain in use. No recovered XML/assets/GUIDs were edited.
- Existing `DependencyResolver` ordering and `ModBattleRuleInstances` dispatch
  supply the order: dependencies before dependents, ordinal ID selection among
  ready mods, and authored order within each appended list. Discovery/folder
  order does not control the combined definition. Order-dependent effects remain
  sequential rather than being described as commutative.
- Existing fingerprints include all patch owners and ordered fight rule handles.
  Recording append operations changes fingerprints for older configurations that
  used them; public docs state this compatibility implication. Owned state IDs and
  schemas are unchanged. Removal still means an enabled-set change plus restart,
  not hot unloading during a fight.

The actual Focus/objective scripts and their editor starters now use separate
upper-left HUD placements. The new public Combine mods guide explains setup,
authority, order, conflicts, the whole-transaction failure boundary, removal and
verification limits. The fight patch/round references, examples, compatibility
and editor guides are updated. Authored schema/generated Lua definitions include
field hover guidance for additive contributions versus exclusive replacement.

### Verification

- `TestModPackRules.ps1`: 86 checks with production Lua, script sessions, catalog,
  dependency resolution, state/save data and current extracted native adapter
  apply/remove and round methods. Covers input discovery order, dependency-before-ID
  order, all replacement/append combinations, contributor diagnostics, controller
  conflicts/disjoint scopes, failed dependencies, later independent recovery,
  aggregate bounds, duplicate handles, static projection/restoration/reinstall and
  the three shipped mods across objective/whole-pack removal and reinstallation.
  Battle source storage and combat models/clock/settlement remain controlled.
- `TestRoundOutcomesUnity.ps1 -WithFocusPack`: 651 isolated Unity 6.6 Play Mode
  checks. The shipped framework/add-on/objective run together with a static-rule
  fixture, actual HUD placement/fit, recovered font, pixels, fades, Focus's bonus
  and custom round win/loss. Current adapter methods project normal/Eclipse source
  clones once and restore them exactly around controlled battle source storage.
  The combined `Focus: 0/3` and completed `3/3` screenshot was visually inspected.
- Standalone objective native fixture: 636 checks. Standalone framework native
  fixture: 33 checks, including saved-state removal/reinstall. Existing managed
  outcome (702), extension (126) and fight-patch (179) checks pass. The fight-patch
  runner now reads its existing examples from `ArchivedMods`, their authoritative
  location, instead of failing before execution on stale `Mods` paths.
- All four managed assemblies compile with the previously documented ignored
  Unity 6.6 reference remapping. Original tracked project paths remain unchanged.
- Editor generate/check/build, 46 unit tests, LuaLS 3.19.1 (including both field
  hovers) and real VS Code field-hover integration pass. Wiki coverage/types,
  build, search and links pass. Generated build/runtime artifacts remain ignored.

Unity's search-index startup again logged an unrelated indexing exception; the
explicit acceptance checks completed successfully. The managed fixture's default
MoonSharp Unity-loader reflection warning under .NET is also unchanged. Neither
is presented as a game failure or hidden by a claim of a clean editor log.

Full native map/menu lifecycle, actual contacts/AI/end animations, result/reward
and real-profile disk/crash/restart acceptance remain unverified. Arbitrary pack
compatibility, automatic HUD conflict resolution, broader engine operations and
source-free independent creator acceptance remain open. The full vision stays active.
