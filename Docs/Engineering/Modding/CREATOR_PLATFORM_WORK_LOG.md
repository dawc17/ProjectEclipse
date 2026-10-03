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
