# Definitive Edition parity: Mod API implementation plan

Recorded 2026-09-08 from the DE parity audit. This is the canonical engineering
roadmap for making `Mods/DE_PARITY_TARGET.md` achievable through the public Mod
API.

**Any agent working on DE parity, Mod API expansion, quests, stages, raids,
Ascension, progression, content overrides, assets, combat hooks, save state, or
UI/service policy must read this document and `DE_PARITY_TARGET.md` before making
changes.** Do not start from a partial task description alone.

The target document defines *what must eventually be possible*. This document
defines *how we get there, in dependency order, without breaking the architecture*.

## Coverage correction, 2026-09-10

The [complete archived XML inventory and API gap audit](DE_XML_API_GAP_AUDIT.md)
compares all 212 DE XML files against 164 base XML files. Phase 1–3 showcase
acceptance below remains valid, but **does not mean every domain exit criterion
or every archived DE mechanic is implemented**. Historical baseline/in-progress
paragraphs below must not override the current 0.7 contract or this audit.

P4.2/P5.4 review identified gaps G01–G14: targeted core patch/removal, programmable
story queries/operations, dojo/UI selection, lotteries and enchanted chests,
set/ability linkage, richer combat mechanics, level-specific perk data, move/input/
projectile authoring, conditional AI, animated locations, item metadata, existing
forge edits, contextual achievements/core localization, and remaining service/
boot/presentation classification. Some require host recovery, not merely bindings.
Use the audit's concrete DE fixtures and source references before claiming parity.

## Engine extensibility review, 2026-09-11

The owner's goal now explicitly extends beyond the DE reference mod: custom
rules, modes, characters, animations, and UI. Phase 4 is deferred pending assets.
[The engine review and E1–E8 roadmap](MOD_ENGINE_EXTENSIBILITY.md) defines this
broader work without claiming those capabilities are implemented.

API 0.8 delivers a bounded P1A.5 + P2A.1/P2A.3 slice: fight-attached Lua behavior
rules with parameter validation, per-rule/per-side transient state, existing
combat callbacks, and target/mode/round filters. This does not close G06/G08,
custom outcomes, programmable AI, or general UI. It uses a core-art trial fixture;
no downstream DE port or missing asset reconstruction is part of this slice.

## 1. Source-of-truth hierarchy

When requirements appear to conflict, use this order:

1. The project owner's latest explicit instructions.
2. `Mods/DE_PARITY_TARGET.md` for parity requirements and acceptance criteria.
3. This implementation plan for sequencing and engineering constraints.
4. `Mods/README.md` for the currently shipped public API.
5. Recovered/runtime source and canonical `Assets/vanillaXml` for actual engine
   semantics.
6. `Assets/DExml` as the archived DE reference used to identify downstream needs.

Never invent engine behavior to fill a hole. If recovered/runtime semantics are
unclear, investigate authoritative local source first and record the uncertainty.

## 2. End state

Eclipse must provide generic public mechanisms powerful enough that Definitive
Edition can exist as an ordinary downstream mod which:

- depends on `core`;
- adds, references, overrides, or removes supported content through public,
  validated contracts;
- supplies its own assets where necessary;
- persists its own progression safely;
- composes with other mods deterministically;
- reproduces the intentional DE behavior/content set without editing Eclipse
  source or canonical base XML;
- leaves the base game functional when the DE mod is disabled.

DE is an acceptance fixture, not a privileged namespace. Do not add code paths
such as `if (modId == "sf2de")`, `EnableDefinitiveEdition()`, or other DE-specific
runtime shortcuts.

## 3. Non-negotiable architecture rules

### 3.1 Economy remains base-owned

The September 8 Eclipse economy is canonical and immutable to downstream mods.
The public API must not expose generic mutation of:

- core item prices;
- upgrade costs;
- forge/enchantment costs;
- skip costs;
- shared currency values or formulas;
- shared economic progression/balance tables.

Existing prices on **mod-owned items** are definition-local data and do not grant
permission to modify the shared/core economy.

Recovered actions such as `GiveCurrency`/`TakeCurrency`, currency cost rules, or
raw internal settings must not be exposed wholesale just because they exist in
the old engine. Higher-level modes may use narrow base-owned cost/policy handles
where needed.

### 3.2 No unrestricted XML replacement as the solution

`ResourceManager` already has dev XML override/adaptation infrastructure. It is
useful as an internal compatibility bridge, but unrestricted file replacement is
not the public parity architecture and is explicitly insufficient evidence in
`DE_PARITY_TARGET.md`.

Public content APIs should produce validated definitions/patches which may
internally adapt into recovered XML/runtime structures. Executable behavior uses
Lua handlers and typed capabilities as specified in section 3.7.

### 3.3 No raw engine objects in Lua

Do not expose `Model`, `Roster`, `ListSF`, `FightList`, arbitrary `GameObject`s,
or other recovered implementation objects directly to scripts.

Use narrow, capability-gated interfaces and typed handles. API 0.3's
`IModFighterOperations` direction is the model to extend.

### 3.4 Determinism over load-order magic

There must be no silent last-mod-wins behavior. Patches must have explicit
ownership, dependency validation, deterministic order, and actionable conflict
diagnostics.

### 3.5 Missing mods must not destroy state

If a mod is missing or disabled, its saved definitions/progression must remain
preserved and inert wherever safely possible. Reinstalling the mod must restore
access without silently rebinding saved IDs to unrelated content.

### 3.6 Public capabilities must be generic

If DE needs a feature, implement the underlying reusable capability rather than a
DE-only special case. The same API should support a third-party story mod, raid
mod, item set, event, location, or combat mechanic.

### 3.7 Static definitions and programmable behavior

Recorded 2026-09-09 from the project owner's API design direction.

Static game content remains typed and declarative. Custom procedural behavior
belongs in executable Lua handlers calling safe, typed domain capabilities.
Do not use Lua merely as a carrier for a generic operation DSL.

- Definitions describe what exists: item stats and metadata, quest identity and
  prerequisites, battles, warriors, locations, animation assets, and references.
- Handlers express what happens: branching, calculations, dynamic choices,
  reactions to runtime events, and changes to mod-owned state. Use Lua functions,
  control flow, and composition for this logic.
- Capabilities perform verified game operations, such as the existing health and
  magic-charge operations. Future quest/story or AI handlers must use similarly
  narrow interfaces with authoritative runtime semantics.
- Reuse one behavior implementation across multiple perk/enchantment definitions
  with different typed instance parameters. Parameters configure behavior; they
  must not become instructions for another interpreter.

Reject new generic instruction tables such as
`{ action_type = "add", op1 = 4, op2 = 2 }`, or an expanding vocabulary of
`SetVariable`/`Conditional`/arithmetic nodes as the way to write custom logic.
Typed domain definitions, content patch operations, and recovered animation or
quest compatibility data remain legitimate; the rule does not require every
quest, move, or tactic to be rewritten entirely in Lua.

The shipped Phase 1 quest condition/action adapters and ordinary tactic
definitions remain supported compatibility authoring paths. They are not the
design template for future procedural APIs. Quest callbacks and programmable AI
remain follow-up gaps until their event/query/operation contracts are implemented
and tested; this rule does not announce them as available or remove existing APIs.

Keep registration validation, namespaces, dependencies, transactional commits,
save ownership, and lifecycle cleanup. Apply capability checks, bounded execution,
error isolation, defined dispatch order, and teardown to handlers as well. Lua
must still never receive raw XML, Model/Roster objects, or unrestricted engine
access, and executable logic does not bypass the economy firewall.

A mostly declarative Phase 1 example proves content integration and safety. It
does not by itself demonstrate the programmable API's expressive power. P2A must
deliver a runnable behavior example whose runtime decisions and state cannot be
preserved by mechanically translating registration tables to existing SF2 XML.
Evaluate the actual supported behavior, not whether Lua syntax is used.

## 4. Audited baseline

The current API 0.3 foundation already provides useful pieces and should not be
discarded:

- namespaced `ModId`, `AssetId`, and `DefinitionId` contracts;
- dependency/version resolution and deterministic topological ordering;
- MoonSharp sandboxing and declared capabilities;
- mod-owned localization additions;
- registration of weapons, armor, helms, ranged weapons, and magic;
- mod-owned shop listings;
- item aliases/tombstones;
- core equipment projection into the content catalog;
- template-backed compatibility perks;
- reusable behavior-backed perks and enchantments with typed parameters;
- saved `EclipseParams` for behavior-backed effects;
- missing-mod equipment preservation and mod provenance metadata;
- namespaced sprite/texture/model/text/audio loading infrastructure;
- additive recovered seams for external items, perks, and forge enchantment
  candidates.

The current catalog does **not** provide first-class public definitions for
quests, zones, battles, fights, warriors, rules, rewards, sets, modes, events,
raids, Ascension, general progression, locations, moves, tactics, achievements,
timers, or UI/service policy.

Current scripted combat behavior is also intentionally tiny:

- event: `FightBegin` only;
- fighter operations: health change and magic-charge change only.

The DE/vanilla audit found 224 non-meta files in `Assets/DExml` versus 167 in
`Assets/vanillaXml`, with 64 DE-only files and 83 shared XML files that differ
semantically. That inventory is a **superset of possible parity work**, not proof
that every delta is intentional DE behavior. Every delta still needs
classification as intentional DE content, upstream-version drift, or a
reconstruction repair before parity is claimed.

## 5. Dependency graph

Do not implement the roadmap as independent vertical features in arbitrary order.
The intended dependency chain is:

```text
P0 definition ownership/patch/conflict + durable mod state
            |
            v
P1 zone/battle/fight/warrior/rule/reward graph
            |
            v
P1 quest/event/condition/action graph
            |
            +-----------------------------+
            |                             |
            v                             v
P1 item/set/progression/forge       P1 assets/location/moves/tactics
            |                             |
            +--------------+--------------+
                           |
                           v
P2 expanded combat behavior + availability/timer/UI/service policy
                           |
                           v
P2/P3 events + Ascension + Underworld + achievements + remaining domains
                           |
                           v
DE reference mod conversion and full parity acceptance
```

Higher-level systems may be investigated earlier, but production API work should
not bypass the prerequisites above.

---

# PHASE P0: COMMON CONTENT AND SAVE FOUNDATION

P0 is the blocker for almost every later domain. Do this before proliferating
one-off registries.

## P0 implementation status, 2026-09-08

API 0.5.0 now contains the common ownership, patch/conflict, policy, fingerprint,
and durable state foundation. Generic collection child adapters are still proven
per-domain as those domains land rather than through raw XML mutation:

- P0.1: existing `DefinitionId`, namespace ownership, direct-dependency checks,
  and deterministic dependency order are retained as the common identity base;
- P0.2: typed replacement of a selected localization language field is live
  through `sf2.localization.patch`; generic collection child operations and
  public remove/tombstone operations for new domains remain open;
- P0.3: committed patches carry owner/target/field/operation provenance;
  same-field writes conflict explicitly while non-overlapping language fields
  compose;
- P0.4: the centralized semantic-field policy is deny-by-default and all
  `economy/*` semantic fields are base-only;
- P0.5: `sf2.state` now provides namespaced typed schemas, transactional batch
  writes, bounded N -> N+1 migrations, aliases/tombstones, opaque missing-mod
  preservation, reinstall restoration, and rollback on migration failure;
- P0.6: the deterministic content fingerprint is now `fingerprint-v6` and
  includes patch provenance, effective content, registered state schemas, and
  every committed Phase 1 semantic registry.

Authoritative implementation evidence for this slice:

- `Assets/Scripts/Eclipse/Runtime/Modding/ModContent.cs` owns patch policy,
  provenance, transaction validation, conflict detection, and atomic
  localization replacement;
- `Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntime.cs` exposes only the
  typed localization patch adapter, not generic XML/object mutation;
- `Assets/Scripts/Assembly-CSharp/LocalizationManager.cs` applies external
  strings as an overlay so removing a core localization patch reveals the base
  value instead of deleting it;
- `Tools/TestModdingContracts.ps1` covers dependency rejection, wrong-category
  rejection, same-field conflict, non-overlap composition, atomic rollback,
  capability gating, fingerprint changes, base restoration, and the economy
  firewall;
- `Tools/TestModStateRuntime.ps1` executes the exact P0.5 remove/save/reinstall/
  migrate/fail sequence, including transactional writes and future-schema
  preservation;
- `Tools/TestPackagedArt.ps1` / `ValidatePackagedArt.cs` exercise the public Lua
  patch path, a real two-mod overlap, state-backed behavior, successful Lua
  migration, and failed-migration rollback in isolated Unity. The editor fixture
  passes 160 checks before the API 0.5 version gate rerun;
- `dotnet build Assembly-CSharp.csproj --no-restore` builds the project with
  zero errors after this slice.

The common P0 machinery is now stable enough for P1 domains to consume. New
collection-like domains must still use the shared semantic patch-key/provenance
model and add explicit add/remove child policies rather than inventing private
last-wins logic.

## P0.1 Generic definition ownership and typed references

### Goal

Every public content domain uses the same ownership/reference rules.

### Required capabilities

- keep `DefinitionId` as the stable public identity;
- allow mods to register definitions in their own namespace;
- allow references to `core` and declared dependencies;
- reject undeclared cross-mod references;
- provide stable typed handles in Lua instead of strings where practical;
- preserve canonical legacy IDs internally when adapting recovered content.

### Acceptance

- two mods can safely reference content from a declared dependency;
- a mod cannot impersonate `core` or another namespace;
- saved IDs remain stable across load/reload.

## P0.2 Generic targeted patch model

### Goal

Support additions, overrides, and removals without replacing whole files.

### Required operation classes

- register/add definition;
- patch selected typed fields on an existing definition;
- add/remove child records in collection-like definitions;
- explicitly remove/tombstone supported content;
- redirect/alias identities where migrations require it;
- reference another definition without copying it.

The exact Lua syntax is not fixed by this plan, but the semantic model should be
equivalent to a validated `content.patch(target, operations)` rather than direct
object mutation.

### Patch policy metadata

Every patchable field/domain must declare whether it is:

- appendable;
- replaceable;
- removable;
- mergeable;
- read-only;
- base-only.

Core economy fields are always read-only/base-only.

### Acceptance

- DE can patch one non-economic field on a core definition without copying the
  entire source XML;
- unrelated patches from two mods compose;
- invalid or unsupported fields are rejected explicitly.

## P0.3 Deterministic conflict diagnostics

### Goal

Make overlapping modifications predictable.

### Required behavior

- retain current dependency topological order;
- track provenance for every committed definition/patch;
- detect incompatible writes to the same semantic field/child identity;
- allow non-overlapping patches to compose;
- include target definition, field/child, and both owners in diagnostics;
- never silently resolve an incompatible patch by filesystem order.

### Acceptance fixture

Create a second tiny test mod that intentionally overlaps one DE/core patch and
assert the expected diagnostic while a non-overlapping patch pair succeeds.

## P0.4 Public policy firewall

### Goal

Prevent future registries from accidentally exposing forbidden or unsafe engine
surfaces.

### Required categories

- economy: base-only;
- arbitrary network/billing/ad SDK calls: unavailable;
- filesystem/process/OS access: unavailable to sandboxed Lua;
- raw engine objects: unavailable;
- raw Unity hierarchy mutation: unavailable;
- explicitly modeled UI/service policy: allowed;
- explicitly modeled non-economic content changes: allowed.

Implement validation centrally rather than relying on every future subsystem to
remember these rules independently.

## P0.5 General mod-owned persistent state

### Goal

Support story/mode/event progression that is not naturally stored in an existing
item/perk node.

### Required contract

- per-mod state namespace;
- explicit schema/version;
- bounded typed values or typed records;
- transactional writes;
- versioned migrations;
- missing-mod opaque preservation;
- no automatic deletion when a mod is absent;
- rollback on failed migration;
- deterministic redirect/tombstone handling for renamed state/definitions.

Existing `EclipseMods` provenance and `EclipseParams` effect state should remain
compatible and can be reused rather than replaced.

### Acceptance

Test this exact sequence:

1. enable a test mod and create state;
2. save;
3. disable/remove the mod;
4. load and save without the mod;
5. reinstall the mod;
6. verify state is restored;
7. update the mod schema and perform a successful migration;
8. force a migration failure and verify the old state remains intact.

## P0.6 Content fingerprint expansion

As new public registries land, include their committed semantic content in the
deterministic content-set fingerprint. Do not fingerprint incidental filesystem
order or generated runtime object IDs.

### P0 exit criteria

- common definition ownership exists;
- targeted typed patches exist;
- deterministic conflicts exist;
- economy fields are centrally protected;
- durable versioned mod state exists;
- tests cover missing/reinstalled mods and overlapping patches.

---

# PHASE P1A: ZONES, BATTLES, FIGHTS, WARRIORS, RULES, REWARDS

This graph must exist before complete quest support because quests reference and
manipulate fights/battles.

## Phase 1 implementation status, 2026-09-09

**Phase 1's integrated showcase is accepted end to end following the user's
September 9 gameplay retests, including the final token reward panel.
P2A is now in progress.** The integrated acceptance fixture is `Mods/example.phase1`, which is
discovered and executed through the real public MoonSharp path by
`Tools/TestPhase1ShowcaseRuntime.ps1` on an unchanged base content catalog.

The September 9 playtest exposed a locale-code collision (`en`), repeated core
parsing/duplicated zones, an unavailable showcase map, missing arena fighter
layer/spawns, and an unbound custom move. Fixes use early locale validation and
guarded late binding, the unique code `en-x-phase1`, recovered map art, typed
fighter positions, and a perk-scoped round-start move event. The locale parser
regression is covered by `Tools/TestModLocaleRuntime.ps1`. The subsequent user
playtests closed the reported showcase blockers. See the sample README for the
accepted scope and limitations; this does not certify every API combination or
complete DE parity.

The follow-up playtest confirmed map/fighter/dialogue/forge presentation. Repairs
now clear reused tooltip labels, preserve Standard AI weights, remove the sample's
unbounded movement lock, scale qualified arena sprites by their import density,
and supply a verified packaged backdrop. Phase Token is hidden from equipment
shop listings; its set remains metadata without a collection UI. Opening Focus
now adds 25% magic charge through a Lua capability, and the player fight-begin
dispatcher supports behavior-backed perks saved by forge recipes. Focused Lua,
dispatch, label regressions and isolated Unity asset loading pass. User gameplay
confirmed movement/blocking, arena rendering, the visible effect, localization,
and the final reward display. The reward sample now supplies both the empty
zero-win slot and token victory slot required by the recovered runtime.

Shipped Phase 1 slices:

- **P1A:** typed zone/battle/fight/warrior/template/rule/reward definitions,
  authored child ordering, provenance-tracked mod battle append into existing
  dependency/core zones, reversible supported core fight field patches, and
  direct/weighted non-economic item rewards. `RewardChoice`'s recovered backing
  list initialization defect is fixed and covered by a runtime regression.
- **P1B:** typed quest events, compare/all/any/not conditions, stable fight/battle
  operands, dialog/story/variable/map/fight/battle/Eclipse/item actions, nested
  dialog-button action sequences, recovered quest injection, and reversible
  external quest teardown. The public surface remains an allowlist rather than
  exposing the recovered action factory wholesale.
- **P1C:** consumable/free/seal definitions, first-class item sets, shared
  shop/quest availability policy, targeted progression branch overlays, and
  structural forge recipe families that reference immutable host economic
  profiles. Shared/core costs and currency formulas remain base-owned.
- **P1D:** locale metadata, qualified PCM16 WAV audio and binary assets, typed
  location layers using namespaced single sprites/music, additive typed
  move/template/trigger registration, and ordinary recovered tactics. Opaque
  tactic handles can be passed directly to `sf2.warriors.register`.
- **Cross-slice save identity:** `fingerprint-v6` includes P1A/P1B/P1C/P1D
  semantic content so changing a shipped Phase 1 definition changes the
  deterministic content-set hash.

Authoritative limitations remain recorded rather than guessed:

- `ConditionalDecisions` is not public because no recovered parser/evaluator was
  found in live source or the recovered dump;
- mod audio playback is currently PCM16 WAV only;
- namespaced location art currently supports individual sprite assets, not a
  general loose multi-sprite atlas/particle/prefab contract;
- move registration is additive only because no authoritative reversible
  replace/remove runtime seam has been proven.

Focused gates are `Tools/TestP1ABContracts.ps1`, `Tools/TestP1CContracts.ps1`,
`Tools/TestPhase1Showcase.ps1`, `Tools/TestPhase1ShowcaseRuntime.ps1`, the existing
state/save/enchantment/fight-begin/RewardChoice suites, and
`Tools/TestPackagedArt.ps1`. The packaged-art editor fixture covers 160 checks with
95 groups, 94 TAR/LZ4 archives, and 10 loose fonts.

## P1A.1 ZoneDefinition

Expose the non-economic semantics represented by recovered `Zone` and stage XML:

- stable identity/name;
- stage/source membership;
- ordering/index;
- initial/open state policy;
- child battle references;
- location/presentation references where applicable.

Support register, targeted patch, and supported removal.

## P1A.2 BattleDefinition

Expose:

- battle type;
- zone membership;
- map position;
- icons/presentation references;
- location;
- child fight ordering;
- normal/Eclipse mode applicability;
- periodic/replay/Ascension/raid-specific metadata through typed extensions,
  rather than an unvalidated property bag.

Recovered subclasses should remain engine implementation details.

## P1A.3 FightDefinition

Expose at least:

- opponent/warrior references;
- round count/time;
- description;
- conditions;
- rules;
- item/equipment rules;
- repeatability where semantically part of the fight;
- non-economic rewards;
- location/presentation references;
- Eclipse/normal variants;
- entry requirements.

Do not expose shared money/currency tuning simply because `FightList` stores it.

## P1A.4 Warrior/opponent definitions

The DE stage graph contains warrior templates/groups and encounter-specific
fighters. Provide typed definitions for the non-economic fighter data required by
DE stages, including equipment, model/appearance, level/power-relevant references,
AI/tactic references, perks/effects, and boss-specific metadata where recovered
semantics justify it.

Avoid copying arbitrary recovered `ModelParameters` objects into Lua.

## P1A.5 Typed fight-rule grammar

Wrap the recovered rule system in a public typed model. Candidate recovered rules
include:

- item/equip/random acquired item;
- no button/no animation;
- ringout;
- darkness;
- hot ground;
- lose fall;
- regeneration;
- attributes;
- damage factor;
- remove interval;
- crazy;
- life steal;
- no health bar;
- combo;
- timeout win;
- points;
- recharge magic each round;
- no bullets replenishment;
- random/complex composition;
- description;
- perk/no perks;
- win style/combo/shock;
- change fight;
- tactic;
- invert joystick;
- random area;
- rating evaluation;
- invulnerability;
- resistance;
- avatar/name.

Currency-cost rule types remain private/base-owned unless a later narrow semantic
policy explicitly requires them.

Each exposed rule must have tests derived from the recovered parser/execution
semantics. Do not expose a rule token whose runtime semantics are still unknown.

## P1A.6 RewardDefinition

Create a generic reward/acquisition model reusable by fights, quests, events,
Ascension, and raids.

Public reward types should cover the non-economic DE needs, including:

- item grant;
- set item/set completion related reward;
- choice/loot selection where recovered semantics are validated;
- perk/unlock/progression grant where appropriate;
- mode/content unlocks;
- experience only if this can be exposed without giving mods shared economy
  mutation authority.

Shared money/currency amount/formula tuning remains base-owned.

### Known runtime checks before exposing rewards

- validate the recovered `RewardChoice` path. The audit found a possible backing
  list initialization issue; write a runtime regression before making it public;
- `FightRaid` currently bypasses the normal `<Rewards>` parse path and
  `BattleRaid` contains incomplete raid-data handling. Do not claim raid reward
  support until the raid-specific path is implemented and tested.

### P1A exit criteria

- a mod can add a new ordinary zone/battle/fight without raw XML replacement;
- a mod can target-patch a core fight;
- a fight can reference a mod-owned opponent, rules, and non-economic rewards;
- overlapping fight patches produce deterministic diagnostics;
- base story fights still parse/run with no mod enabled.

---

# PHASE P1B: QUEST GRAPH

The recovered quest system remains the execution engine for compatible quest
content. Phase 1 provides a validated, namespaced adapter over it. Future custom
quest procedures should use Lua handlers and typed quest capabilities under
section 3.7, while preserving recovered scheduling, progression, and save semantics.

## P1B.1 QuestDefinition and QuestStageDefinition

Expose:

- quest/stage identity;
- priority;
- group membership;
- marks;
- resumable/unresumable behavior;
- action placement (`Map`, `Fight`, `Dojo`) where recovered semantics support it;
- event list;
- condition tree;
- action sequence;
- references to fights/battles/items/modes/etc.

Saved quest progression must use stable mod-owned identities and survive a missing
mod according to P0 rules.

## P1B.2 Quest events

Build an allowlisted typed enum/schema from verified recovered `QuestEvent`
semantics. Existing recovered events include fight, raid, level, item, dialog,
session, purchase/delivery, timer, map, enchantment, perk, Ascension, set, duel,
raid lifecycle, season, daily window, scene and shop events.

Do not assume every token in DE XML is already functional.

### Deferred event blocker

`QuestCompatibility.DeferredQuestEvents` currently defers modern events including
at least:

- `BeforeQueue`;
- `CheckUserUpdate`;
- `RaidFloorChanged`;
- `RaidMapEnter`;
- `ReplayButtonPress`;
- `ShowRaidLoot`.

For each DE-relevant deferred event, either implement authoritative offline
semantics or classify it as unnecessary for intentional DE parity with evidence.
Silently mapping it to `QUEST_EVENT_NONE` is not parity.

## P1B.3 Quest conditions and expression system

For recovered-content compatibility, wrap verified `QuestCondition` capabilities
instead of exposing expression strings without validation. Required concepts include:

- equal/greater/greater-equal/less/less-equal;
- nested AND/OR/NOT;
- quest variables/marks;
- fight state;
- player state;
- item ownership/equipment/availability;
- battle state;
- timers;
- enchantment state;
- shop-open state;
- raid/mode state where supported;
- verified recovered math/string expressions only where required for compatibility.

Custom calculations and branching belong in Lua. Future programmable quest
support should expose typed queries/predicates instead of expanding this
compatibility grammar into a general-purpose expression interpreter.

Any property that reads shared economy may be query-only if needed for compatibility;
it must not imply mutation authority.

## P1B.4 Quest actions

Preserve an allowlist of compatibility actions whose semantics are verified and
safe. Important DE families include:

- dialog/story presentation;
- enter/start/show/hide fight or battle;
- map focus/goto zone;
- variable/mark changes;
- item/content visibility;
- give item/non-economic reward;
- timers;
- map buttons;
- Eclipse/mode transitions;
- raid map transitions where offline semantics exist.

Do **not** expose the entire recovered action factory. It contains currency,
billing, advertisement, network, platform, and legacy service actions outside the
public contract.

For future custom quest procedures, expose verified domain operations to Lua
handlers. A handler should perform its own branching and call an operation to
open content or update allowed state, rather than construct generic instruction
tables. Callback names, scheduling, resumability, and state access require a
separate verified contract; Phase 1 action registration does not supply one.

### Deferred/no-op action blocker

`QuestCompatibility.CreateRuntimeAction` currently substitutes no-op deferred
actions for modern actions including at least:

- `ChangeButtonState`;
- `ClickHint`;
- `ConnectToRaids`;
- `GiveGift`;
- `OpenRaidZone`;
- `RaidIndicateRaidBtn`;
- `SceneMenuScroll`;
- `ShowRaidLoot`;
- `UnlockCharacter`.

Any one required by intentional DE content must get real offline semantics before
the related DE feature can be marked complete.

## P1B.5 Quest composition and patching

Support:

- register a whole new quest chain;
- add a stage to an existing quest file/chain;
- target-patch a stage's events/conditions/actions;
- remove/disable a specific supported stage/action with conflict tracking;
- reference content from declared dependencies.

Do not require a mod to replace all of `quests.xml` to add one story arc.

### Old Wounds / Sensei acceptance fixture

Use the actual DE source:

- the Old Wounds/Sensei story sequence is primarily in `Assets/DExml/quests.xml`
  around the `SENSEI_MEMORIES` flow and corresponding battles in
  `Assets/DExml/stages.xml`;
- `Assets/DExml/quest_extensions/sensei_arc.xml` is currently empty and must not
  be treated as the implementation of the story;
- the DE achievement `SenseiStoryFinished` should eventually become part of the
  achievement phase.

### P1B exit criteria

- an external mod can register and execute a multi-stage quest with dialog,
  conditions, actions, and a custom fight;
- Titan-style conditional item acquisition can be expressed through a public
  quest/fight reward path;
- the Old Wounds quest graph can be represented without raw file replacement;
- required deferred actions/events have real semantics or documented evidence
  that they are not part of intentional parity;
- quest save/reload and missing-mod behavior are tested.

---

# PHASE P1C: ITEMS, SETS, PROGRESSION, FORGE, SHOP AVAILABILITY

## P1C.1 Complete non-economic item semantics

The five current equipment definitions model only a subset of `ItemInfo`. Expand
them, or introduce shared typed components, for DE-required non-economic fields
such as:

- hidden/shop-hide state;
- pack/group/acquisition labels;
- tactic subtype;
- fixed perk/enchantment instances;
- set membership;
- upgrade-path references;
- availability/unlock references;
- special model/icon presentation;
- silent receive or other verified acquisition behavior;
- consumables/non-equipment item types required by DE modes and sets.

Do not expose core price/cost mutation while expanding item coverage.

## P1C.2 ItemSetDefinition

Provide first-class sets for:

- membership;
- completion state;
- set effect/ability reference;
- presentation;
- acquisition/progression hooks;
- saved ownership/completion behavior.

Acceptance fixtures include Monk, Sentinel, Neo-wanderer, and any ability pseudo-set
that is semantically part of DE rather than version drift.

## P1C.3 Character progression and unlocks

Expose non-economic progression required by `CharacterProgress.xml`, including
verified movement/perk/unlock relationships and content gates.

Do not expose arbitrary XP/economy formula replacement.

The DE audit specifically found progression differences involving
`PERK_MASTER_OF_STYLE` and `PERK_RELENTLESS`; use them as fixtures after the
underlying recovered semantics are traced.

## P1C.4 Forge recipe-family API

Current public enchantments only select `Simple`, `Medium`, or `Complex` and add
an external candidate. DE forge data contains more recipe families/relationships,
including additional complex/ability groups.

Expose the non-economic forge model required for:

- recipe identity/family;
- supported equipment types;
- candidate pools;
- eligibility/conditions;
- variation selection;
- aspect/deviation behavior where it is not shared economic balance;
- availability/unlock relationships.

Keep these base-owned:

- recipe costs;
- skip costs;
- shared economic scaling;
- other canonical economy amounts.

Timer duration is handled later by semantic timer policy, not by exposing cost
fields.

## P1C.5 Shop availability/acquisition policy

Separate **whether/how content is available** from **what the shared economy
costs**.

Expose:

- visibility/unlock conditions;
- availability labels/groups;
- alternate acquisition source;
- permanent availability independent of old paid offers/events;
- mode/event prerequisites.

This is the mechanism for special-offer items becoming normally obtainable in DE.

### P1C exit criteria

- DE-only/special equipment can be authored with all required non-economic
  semantics;
- sets are first-class and persist correctly;
- progression/unlock changes are expressible;
- forge recipe membership/eligibility is expressible without exposing shared
  costs;
- special-offer items can be permanently available through public policy.

---

# PHASE P1D: LOCALIZATION, AUDIO, LOCATIONS, MOVES, TACTICS

## P1D.1 Localization patching and locale metadata

Current localization is additive and mod-owned. DE also changes existing strings
and locale/font metadata.

Add controlled support for:

- overriding explicitly patchable core localization keys;
- adding languages/locales;
- locale metadata;
- font references/fallbacks required by supported languages;
- deterministic conflicts when two mods replace the same key.

DE fixtures include added locale entries such as Swedish, Romanian, Croatian,
Hungarian, and Hindi where those are confirmed intentional and runtime-capable.

## P1D.2 Namespaced audio handles and binding

The asset loader can classify/load audio, but public Lua currently lacks a full
audio/binding API and the recovered music path bypasses `ModAssetLoader`.

Required work:

- expose `sf2.assets.audio(...)` or equivalent typed handle;
- integrate namespaced clips with recovered music/SFX playback;
- allow battle/location/story definitions to reference audio handles;
- decide whether loose-mod audio is intentionally WAV-only or add validated
  OGG/MP3 decoding. Today `LooseModProvider` classifies OGG/MP3 as audio while
  `ModAssetLoader.LoadAudio` accepts only PCM16 WAV, so the contract must be made
  consistent;
- keep playback routing deterministic and dependency-aware.

Acceptance: a mod-owned cut/unused music track can be assigned to a modded or
patched battle/scene without placing it at a legacy core path.

## P1D.3 LocationDefinition and namespaced location assets

Recovered `Location` currently loads params/layers through hardcoded legacy paths.
Provide a public location definition with namespaced references for:

- location params;
- layer textures/sprites/atlases;
- environmental/effect references supported by recovered semantics;
- music/audio;
- other presentation metadata required by DE locations.

DE fixtures include `mountain_new`, `bamboo_grove_new`, `moon_new`, `sakura_new`,
and other confirmed DE-specific location parameter sets.

## P1D.4 Move/template/animation registry

`AnimationData` currently performs one whole `moves.xml` load with no additive
public registry. Add typed public support for:

- move templates;
- moves;
- triggers/conditions;
- animation/binary references;
- add/patch/remove operations;
- namespaced `.bytes` or equivalent required runtime assets.

DE fixtures include `AirPunch`, `MindThrow*Normal`, `ShadowCloakPlayer`,
`Sphere1/2/3`, `WallRunUp`, and confirmed shop-magic moves.

## P1D.5 Tactic/AI reaction registry

Tactic support depends on moves existing first. Expose typed reaction/tactic
definitions and references instead of raw tactic XML replacement.

These definitions cover ordinary recovered tactics. Custom decision-making
should eventually use Lua decision handlers with typed observations and supported
move/target capabilities, rather than an ever-growing declarative decision tree.
That programmable contract remains unimplemented until authoritative decision
seams, ordering, and execution bounds are established. Static move metadata and
recovered animation timelines can remain data.

DE fixtures include `HermitStorm` and `WallRunUp` reactions.

### P1D exit criteria

- a mod can patch an existing localization string and add a locale safely;
- namespaced mod audio can be assigned to actual game playback;
- a mod-owned location can load namespaced params/art/audio;
- moves/templates can be added without replacing the entire move file;
- tactic rules can reference mod-owned moves.

---

# PHASE P2A: EXPANDED COMBAT BEHAVIORS

**Implemented and user runtime-tested, 2026-09-09.** API 0.6 adds fight/round lifecycle,
resolved damage, block, critical and incoming-damage hooks; player and opponent
contexts; scoped target capabilities; temporary damage shields; and typed
round/fight/saved behavior state with migrations. Composition uses Lua functions
and modules. See [the supported contract](P2_API.md) and
[integrated sample](example.phase2/README.md). Gameplay acceptance was confirmed by the project owner;
the candidate event families below are not a claim that every hook is exposed.

API 0.3 proves the reusable behavior + typed instance parameter architecture. Do
not replace it with template copying. Expand it carefully at authoritative combat
boundaries.

## P2A.1 Behavior event lifecycle

Trace recovered perk/action semantics and add only events with clear authoritative
call sites. Expected families, subject to recovered-source confirmation, include:

- fight begin/end;
- round begin/end and round-stage transitions;
- before/after hit;
- before/after critical hit;
- damage dealt/received;
- block/miss/shock/knockdown;
- magic/ranged activation;
- perk/effect activate/deactivate;
- bounded timed/update callbacks only where a recovered mechanic genuinely
  requires them.

Do not invent callback ordering. Tests must assert recovered call order.

## P2A.2 Safe fighter/effect operations

Extend the narrow capability interface with only recovered operations required by
real mechanics, such as verified forms of:

- health/magic change;
- temporary attribute modifiers;
- effect/modifier add/remove;
- perk/effect state;
- hit mutation where the recovered engine has a safe semantic seam;
- temporary flags;
- timed effects;
- opponent/target operations through explicit target handles.

Each operation must:

- be capability-gated;
- validate bounds/types;
- avoid exposing raw `Model`/fight objects;
- define failure behavior;
- have a real combat integration test.

## P2A.3 Per-instance saved state

Continue using typed schemas and `EclipseParams`. Add migration/versioning only
through the P0 state contract. Preserve recovered `<Set>` compatibility where
engine internals still need it.

### P2A exit criteria

- a runnable public Lua example reacts to a verified combat event, makes a
  runtime decision using typed event/query data and mod-owned state, and invokes
  a supported gameplay capability; its custom procedure cannot be represented
  by translating registration tables into existing SF2 XML;
- the example reuses one behavior with different typed instance parameters and
  tests runtime effects, event order, state lifetime, error isolation, and cleanup;
- representative DE perk/enchantment/set mechanics that depend on hit/damage/
  round state can be implemented without copying recovered XML templates;
- callback ordering is tested;
- no raw engine object is script-visible;
- existing API 0.3 sample behavior remains compatible.

---

# PHASE P2B: TIMERS, SERVICES, UI POLICY, EVENTS

**Implemented and user runtime-tested, 2026-09-09.** API 0.6 implements forge duration/skip policy, named service/feature gates, and repeatable scheduled fight events with level/time eligibility. Arbitrary UI extensions and settings are not exposed; see [the bounded contract](P2_API.md). Gameplay acceptance was confirmed by the project owner.

## P2B.1 Semantic timer policy

The current base already forces some shop delivery paths instant while forge still
uses delivery queues. Public policy should express semantics such as:

- instant vs timed delivery for supported subsystems;
- timer duration policy where DE needs it;
- whether skipping is enabled.

Do not expose skip cost/economic tuning.

## P2B.2 Service policy

Provide typed feature/service gates rather than arbitrary internal settings:

- paid offer availability;
- battle-pass availability;
- advertisement/rewarded-video surfaces;
- online-only legacy service entry points;
- external storefront/payment surfaces.

The build system remains responsible for physically excluding SDK binaries.

## P2B.3 UI feature/extension policy

Expose semantic UI hooks/slots, not arbitrary GameObject access. Required families
may include:

- hide/show known feature surfaces;
- register a mode/event/map entry button;
- battle visibility/map focus integration;
- shop acquisition presentation;
- mode/event panels using stable extension points.

Generalize existing hardcoded desktop/Underworld UI behavior where needed.

## P2B.4 EventDefinition and schedule policy

Use the quest/fight/reward foundation to build reusable events with:

- event identity;
- content references;
- eligibility;
- schedule/start/end policy;
- repeatability;
- permanent availability;
- quest/battle chain references;
- rewards/acquisition;
- UI entry point.

This is the core mechanism for DE's no-FOMO permanent events.

### P2B exit criteria

- a downstream mod can make a supported subsystem instant without changing shared
  costs;
- battle pass/offer/ad surfaces can be disabled through semantic policy;
- an event can be made permanent and repeatable through public definitions;
- no arbitrary internalSettings/GameObject mutation is needed.

---

# PHASE P2C: ASCENSION

**Implemented and user runtime-tested, 2026-09-09.** API 0.6 implements typed multi-fight mode sequences, loss reset, persistent progress, entry items and native rewards. The sample exercises a three-fight Ascension sequence and Monk equipment rewards through this reusable mode host. Gameplay acceptance was confirmed by the project owner.

Ascension must be composed from generic mode/fight/reward/state primitives, not a
DE-specific toggle.

## Required capabilities

- mode definition/registration;
- entry eligibility;
- battle sequence/reference;
- reset/repeat policy;
- persisted current progression;
- loot/reward selection;
- Monk set acquisition;
- UI/map entry;
- quest events/actions required by the recovered flow;
- narrow base-owned handling for any ticket/cost concept without exposing generic
  shared currency mutation.

The audited DE implementation spans `quests.xml`, `stages.xml`, and `list.xml`, so
do not treat Ascension as a single battle type.

### P2C exit criteria

- Ascension can be registered by an ordinary external mod;
- progress survives save/reload and missing/reinstalled mod cycles;
- Monk rewards are acquired through the public reward/set API;
- the feature does not require generic currency mutation access.

---

# PHASE P2D/P3: UNDERWORLD / RAIDS

**Implemented and user runtime-tested, 2026-09-09.** The revised sample now uses one 300-second fight, a ten-bar boss with the native blue health display/counter, and a 25-gem victory reward. Timeout requires boss defeat; the project owner confirmed this correction. Included in this implementation pass: registered offline raid encounters, mod-owned entry tickets, persistent boss progression, native loot presentation, metadata-based raid/hard-mode classification and raid quest events. Native online services are outside this contract. Gameplay acceptance was confirmed by the project owner.

Underworld is a larger host gap than ordinary story content. Existing Eclipse
support is internal compatibility code and hardcoded naming/UI policy, not a
complete public offline raid API.

## P2D.1 Raid zone/boss/encounter definitions

Expose generic raid definitions for:

- raid zones;
- bosses;
- encounters/rounds;
- hard-mode metadata;
- fighter setup;
- raid-specific rules;
- entry/key requirements;
- lifecycle state.

## P2D.2 Offline raid progression and keys

Implement recovered-equivalent offline semantics for:

- raid entry;
- floor/zone progression;
- key acquisition/use where required;
- boss completion;
- repeat/reset policy;
- persistent state.

Do not simply preserve old server/network assumptions.

## P2D.3 Raid rewards/loot

Fix/complete the actual raid reward path first. The audit found that ordinary
fight reward parsing is skipped for `FightRaid` and `BattleRaid` has incomplete
raid-data handling. Public raid rewards are not done until real game result flow
awards and persists them.

## P2D.4 Raid quest semantics

Close the DE-relevant deferred raid events/actions listed in P1B. A raid API whose
story/UI quest hooks silently no-op is not parity.

## P2D.5 Generalize hardcoded Underworld policy

Existing code recognizes specific `ZONE_RAID*` patterns and contains hardcoded
raid UI/presentation conventions. Replace or wrap those assumptions with generic
definition metadata before declaring third-party raid support.

### P2D exit criteria

- an external mod can register an offline raid zone and boss without DE-specific
  names;
- entry, progression, key policy, rewards, save/reload, and UI entry work;
- required raid quest events/actions execute real semantics;
- another mod can add a separate raid without colliding with DE's namespace.

---

# PHASE P3A: ACHIEVEMENTS, COUNTERS, REMAINING PROGRESSION DOMAINS

**Implemented; core showcase user runtime-tested, API 0.7.** Owned achievement counters
integrate with the native profile/save model and advance from Lua callbacks.
The remaining configuration differences are classified in a reproducible ledger;
no arbitrary settings passthrough was added. See [P3_API.md](P3_API.md) and the
[showcase playtest record](example.phase3/README.md#recorded-user-playtest).
The user confirmed the achievement flow, restart persistence and no progress on
loss/surrender. Removal/reinstallation remains automated-test coverage.

## P3A.1 Achievement/counter registry

Expose typed achievement/counter definitions only after tracing the recovered
state model. DE fixtures include `SenseiStoryFinished` and enchantment-related
achievement changes.

## P3A.2 Remaining guarded settings

Classify `internalSettings.xml` deltas by semantic intent. Expose only stable
feature/policy concepts not already handled by P2B. Keep economy and platform/
service implementation details private.

## P3A.3 Credits/devices/build/loader/platform configuration

These DE-vs-vanilla differences must be classified before any API is added.

Likely categories:

- build-only dependency/SDK exclusion;
- reconstruction compatibility;
- platform configuration;
- presentation content;
- obsolete upstream-version drift.

Do not create Mod API surface merely because a file differs.

---

# PHASE P3B: CONTROLLED CORE ASSET REPLACEMENT

**Implemented, API 0.7; showcase sprite replacement user-confirmed.** Explicit typed sprite/texture/audio/model
redirects now enforce dependency ownership, atomic conflicts and provenance.
Native atlas-member identity and core model text loading are covered. Arbitrary
config, boot-time resources, opaque animation replacement and deletion remain
outside this supported contract; animation authoring uses the existing move API.
See [the precise boundaries](P3_API.md#asset-replacement).

Core assets are addressable and supported runtime kinds are replaceable. DE may need
to change existing art/model/audio/animation assets rather than only reference
new ones.

## Required contract

- explicit replacement/redirect declaration;
- target must belong to a declared dependency, usually `core`;
- replacement asset type must match the target contract;
- provenance must be tracked;
- multiple incompatible replacements must conflict explicitly;
- no filesystem shadowing or last-mod-wins behavior;
- removal only where the consuming system has defined missing behavior.

Use this for semantic asset replacement only after the higher-level definition
can point at namespaced assets. Prefer patching a battle/location/item to reference
a new asset instead of globally replacing a core asset when both achieve the same
intent.

---

# PHASE P4: DE REFERENCE MOD

Do not wait until the very end to create one giant DE mod. Build it incrementally
as each public slice lands, but only mark a feature complete when it no longer
depends on private Eclipse hooks.

## P4.1 Required DE module split

Keep the reference mod organized by generic domain so it doubles as API examples:

```text
Mods/definitive-edition/
  mod.toml
  scripts/
    main.lua
    content/
      localization.lua
      equipment.lua
      sets.lua
      forge.lua
      progression.lua
      fights.lua
      quests.lua
      events.lua
      ascension.lua
      raids.lua
      locations.lua
      moves.lua
      tactics.lua
      audio.lua
      ui_policy.lua
  assets/
    ...only DE-owned/recovered assets not already supplied by core...
```

The exact folder names are provisional, but keep systems separated enough that a
third-party author can copy a small example instead of reading one monolith.

## P4.2 Intentional-delta ledger

Maintain a machine-readable or reviewable ledger mapping every audited DE delta
to one classification:

- `intentional_de`;
- `canonical_eclipse_economy`;
- `upstream_version_drift`;
- `reconstruction_repair`;
- `build_only`;
- `unresolved`.

For every `intentional_de` entry record:

- source file/node or asset;
- public API capability used;
- DE mod definition/module implementing it;
- automated or manual verification case.

Unresolved or unmapped intentional entries remain parity blockers.

## P4.3 Feature fixtures

The DE reference mod must eventually demonstrate at least:

- Old Wounds / Sensei story;
- permanent/no-FOMO event availability;
- Ascension and Monk set acquisition;
- Sentinel, Neo-wanderer, and other confirmed sets;
- Titan's Desolator conditional Eclipse-mode acquisition;
- special-offer item alternate availability;
- Underworld event bosses/raid progression;
- cut/unused music assignment;
- DE locations;
- DE moves/tactics;
- required perk/enchantment/set combat mechanics;
- service/UI policy for battle pass/offers/ads;
- all intentional non-economic equipment/progression changes.

---

# PHASE P5: ACCEPTANCE AND REGRESSION GATE

Parity is not achieved when the API compiles or when DE XML parses. It is achieved
only when the reference mod works through unchanged public contracts.

## P5.1 Contract tests

Every public definition/patch/state type needs deterministic unit tests for:

- validation;
- namespace ownership;
- dependency references;
- duplicate IDs;
- unsupported fields;
- economy firewall;
- conflict detection;
- atomic registration/rollback;
- serialization/fingerprint stability.

## P5.2 Recovered-runtime integration tests

Where practical, execute the actual recovered parser/state mutation paths rather
than testing only DTOs. Continue the pattern used by the current save/enchantment
runtime scripts.

## P5.3 Unity runtime tests

At minimum verify:

- base boot with no mods;
- DE mod boot;
- quest progression;
- fight entry/result flow;
- item acquisition/equip/upgrade;
- forge/enchantment flow;
- perk/enchantment behavior hooks;
- location/audio/move loading;
- Ascension;
- Underworld;
- save/reload;
- disable/re-enable DE;
- conflicting second mod.

## P5.4 Full parity playthrough gate

Before saying "DE parity is achieved":

1. Re-run the semantic DE/vanilla inventory.
2. Classify every difference.
3. Map every intentional DE difference to public API + DE mod code + verification.
4. Run the full story through intermission and Titan.
5. Validate Old Wounds/Sensei progression.
6. Validate restored modes/events.
7. Validate Ascension and set acquisition.
8. Validate Underworld event bosses and raid progression.
9. Validate all required equipment acquisition/upgrading.
10. Save/reload at representative points.
11. Disable DE and verify the unchanged base profile remains healthy.
12. Re-enable DE and verify preserved mod state/content returns.
13. Enable an overlapping test mod and verify deterministic conflict behavior.
14. Confirm no DE implementation relies on private source edits or unrestricted
    raw XML/file replacement.

---

# 6. Known cross-domain DE fixtures and dependencies

These examples are here specifically so future agents do not accidentally treat
the features as isolated files.

## Old Wounds / Sensei story

Depends on:

```text
stages/fights
  -> quests/events/conditions/actions
  -> dialog/UI presentation
  -> rewards/unlocks
  -> achievement
  -> save state
```

The meaningful content is in DE `quests.xml` and `stages.xml`; the current
`quest_extensions/sensei_arc.xml` is empty.

## Ascension

Depends on:

```text
mode definition
  -> stages/fights
  -> quest flow
  -> state/reset/repeat policy
  -> reward/loot choice
  -> Monk set/items
  -> UI entry
```

It spans DE `quests.xml`, `stages.xml`, and `list.xml`.

## Underworld

Depends on:

```text
raid zone/boss/fight graph
  -> raid-specific state/key policy
  -> real raid reward flow
  -> deferred raid quest semantics
  -> UI/map presentation
  -> persistent progression
```

Do not mark Underworld complete based only on `raid_stages_default.xml` parsing.

## DE sets

Depend on:

```text
items
  -> set membership
  -> acquisition/rewards
  -> combat behaviors/effects
  -> save state
```

## Cut/unused music

Depends on:

```text
namespaced audio asset
  -> actual Sound integration
  -> stage/location/story binding
```

Loading an `AudioClip` in isolation is not enough.

## New moves/tactics

Depends on:

```text
move/template registry
  -> namespaced animation/binary assets
  -> tactic/reaction definitions
  -> fights/warriors that reference them
```

---

# 7. Agent execution protocol

This section is mandatory for coordinated work.

## Before an agent edits anything

The task context must tell the agent to read:

1. `Mods/DE_PARITY_TARGET.md`;
2. `Mods/DE_API_IMPLEMENTATION_PLAN.md`;
3. the relevant current API/runtime files for its assigned phase.

The agent must state which plan task IDs it is addressing, for example
`P1B.2 + P1B.4`, and must not silently broaden into another phase.

## Evidence rules

Every implementation decision involving recovered behavior must cite exact local
files/symbols in its report. `Assets/vanillaXml` and recovered source are
authoritative for engine semantics. `Assets/DExml` demonstrates desired DE
content but must not be used to invent unsupported runtime behavior.

No web guesses are acceptable for recovered SF2 semantics when local source exists.

## Scope rules

Agents must not:

- expose core economy mutation;
- add unrestricted raw XML replacement as a parity API;
- expose raw `Model`, `Roster`, `ListSF`, or arbitrary GameObjects to Lua;
- implement DE-specific conditionals in Eclipse;
- silently turn unsupported quest actions/events into success;
- claim a subsystem complete because registration works while runtime/save/UI
  consumption remains missing;
- overwrite unrelated dirty-tree work.

## Required worker handoff

Every worker report should include:

- **PLAN TASKS**: exact IDs from this document;
- **CURRENT SUPPORT**: what existed before;
- **CHANGES**: files/symbols changed;
- **RUNTIME SEAM**: authoritative recovered path used;
- **VALIDATION**: tests/build/runtime checks actually run;
- **REMAINING GAPS**: anything preventing that task's exit criteria;
- **ECONOMY CHECK**: explicit statement that no forbidden economy mutation was
  exposed;
- **DE FIXTURE**: which DE content/example proves the capability is useful.

If a worker discovers that the plan is wrong or incomplete, report the discrepancy
to the prime agent. Do not quietly invent a parallel architecture.

## Prime-agent responsibility

The prime agent owns cross-phase consistency. Before merging worker work, verify:

- it satisfies the assigned exit criteria;
- it uses shared definition/state/conflict infrastructure instead of creating a
  silo;
- it preserves the economy firewall;
- it adds/updates tests;
- it updates current API docs when public behavior changes;
- any newly discovered parity gap is recorded in this plan or the delta ledger.

---

# 8. Suggested implementation batches

These are the recommended work batches for multi-agent runs. Parallelize within a
batch only where the dependencies are already satisfied.

## Batch 0: foundation

- P0.1 ownership/references;
- P0.2 patch model;
- P0.3 conflicts;
- P0.4 policy firewall;
- P0.5 state/migrations;
- P0.6 fingerprint.

Do not start production quest/stage registries until the shared patch/state model
is stable enough to consume.

## Batch 1: content graph

Parallel lanes after P0:

- lane A: P1A zone/battle/fight/warrior definitions;
- lane B: P1A rule/reward definitions and regressions;
- lane C: P1C item/set/progression model foundations;
- lane D: P1D asset/audio/location/move feasibility/runtime seams.

## Batch 2: quests and authoring

- P1B quest graph;
- deferred quest semantics required by story content;
- Old Wounds fixture;
- Titan conditional reward fixture.

## Batch 3: content completeness

- complete P1C forge/shop availability;
- complete P1D localization/audio/location/moves/tactics;
- start incremental DE reference modules.

## Batch 4: runtime behavior and policies

- P2A combat lifecycle hooks;
- P2B timer/service/UI/event policy;
- representative DE perk/set/event fixtures.

## Batch 5: modes

- P2C Ascension;
- P2D/P3 Underworld/raids;
- resolve remaining deferred raid semantics;
- achievements/counters.

## Batch 6: parity closure

- controlled core asset replacement where still required;
- classify all remaining DE deltas;
- finish DE reference mod;
- full P5 acceptance matrix.

---

# 9. Definition of done for any new public domain

A domain is not done until **all** of these are true:

- public typed definition exists;
- static definitions and executable procedures follow section 3.7; domains
  claiming custom behavior expose tested Lua handlers and typed capabilities,
  with unsupported behavioral surfaces documented as gaps;
- namespace/dependency validation exists;
- add/reference behavior exists;
- required targeted patch/remove behavior exists;
- deterministic conflict behavior exists;
- recovered runtime actually consumes it;
- save behavior is defined where stateful;
- missing-mod behavior is defined where stateful;
- asset references are namespaced where applicable;
- Lua surface is sandbox-safe and capability-gated where executable;
- economy firewall is enforced;
- current API docs are updated;
- automated tests cover registration + runtime consumption;
- at least one real DE fixture uses it;
- base game without the mod still passes regression checks.

If only the DTO/registry exists, the task is **not complete**.

---

# 10. Immediate next milestone

API 0.7 and the Phase 1–3 showcase flows are implemented and have the recorded
user acceptance above. The current work is **P4.2 coverage/intent classification**
before a complete downstream DE conversion and wiki. Updated DE source is awaited.

The September 10 audit identifies work that is already demonstrably missing in
the archived XML; it does not authorize inventing behavior or adding DE-specific
shortcuts. Start any subsequent implementation from [G01–G14](DE_XML_API_GAP_AUDIT.md)
and the relevant original phase exit criteria. Keep current showcases green,
preserve the economy firewall, and use typed capabilities with real Lua handlers
for new procedural behavior. Full parity still requires the DE reference mod,
record-level intent decisions and P5 gameplay verification.


## Active pre-DE expansion (2026-09-12)

The owner requested continued implementation across the engine roadmap and DE
port blockers while production assets are pending. Track cumulative delivery and
open requirements in [PRE_DE_WORK_LOG.md](PRE_DE_WORK_LOG.md). API 0.10 adds
existing-fight rule append/replacement and presentation patches. This is partial
G01/E1 coverage; phase 4 and the full acceptance matrix remain incomplete.
