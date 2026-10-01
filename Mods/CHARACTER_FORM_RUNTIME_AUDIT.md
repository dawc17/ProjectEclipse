# Character forms: runtime integration requirements

Originally inspected 2026-09-12; native checkpoint updated 2026-09-16. The experimental
public request now passes the baseline Shifting Guardian native fight test.
The sections below retain the integration history and its evidence; this does not
close the broader E5 requirements. DE content conversion remains excluded.

## Verified native boundaries

- `Model.Init` calls `FCPIDGIFNKE`, which calls `Clear` before loading the model
  and recreating physics, strike, animation, collision and AI objects.
- `Model.Clear` removes model and animation listeners, clears enemies and event
  data, and drops those subsystem references. Calling Init on a fighting model
  does not preserve the fight's registration automatically.
- `Model.GMFOJPHEHHI` rebuilds equipment/perk animation candidates. It is not a
  body/controller replacement operation and must not be advertised as one.
- `Model.Reset` also resets statistics, conditions, animation and temporary perk
  state. It is unsuitable for preserving an ongoing boss phase by itself.
- `Fight.SetModelOnListening` registers combat listeners; `OLINHHIJCDL` separately
  registers models with camera and perks. `_SelectAnimation.set_Models` receives
  the fight's model collection. These are independent integration obligations.
- `Fight.EclipseFighterOperations` holds a concrete model reference. Shields are
  keyed by model. Replacing a model without lifetime handling leaves old callback
  handles and state referring to the previous instance.
- `Fight.OnStyleChanged` handles the combat style meter; it is not a character
  transformation seam despite its name.

## Implementation direction

Use a prepared replacement at a simulation boundary, with an explicit fighter
identity retained across forms. Do not expose Init, raw model replacement or a
sequence of native operations to Lua. Form definitions should refer to typed
warrior definitions; phase decisions belong in ordinary Lua callbacks.

1. Resolve and prepare the destination warrior, composed model, moves and tactic
   without mutating the active fighter. Validate every required rig reference.
   Resource or validation failure must leave the current fighter usable.
2. Queue a transition request against the fight generation and fighter identity.
   Apply outside hit/animation/perk callback dispatch and model-list iteration.
   Define duplicate/conflicting requests and cancel them on unload or round end.
3. Capture position, facing and the chosen health policy before replacing the
   model. Health policy must be explicit (preserve fraction, preserve amount or
   reset); a transformation must not accidentally count as a death or reward.
4. Replace the model's registrations coherently: active model list, opponent
   references, animation selector, camera, HUD, control ownership and perk host.
   Preserve the logical fighter identity while invalidating old native handles.
5. Define carry/reset behavior for shields, status effects, magic, cooldowns,
   statistics and running animation. Rig-specific contacts/intervals cannot be
   carried blindly into a different rig. This needs an explicit contract before
   exposing the operation publicly.
6. Dispatch completion only after the new form is usable. Lua can choose the next
   behavior from that event; it must not receive a success result on mere queuing.

## Required evidence before publishing

- A two-form boss actually changes body, equipment, available moves and AI in one
  ongoing round, with both sides/mirroring verified.
- Invalid destination assets leave the old form operational and report the cause.
- A transition requested during a hit callback completes outside that callback;
  simultaneous lethal damage, round end and unload have deterministic outcomes.
- Held player inputs, opponent targeting, HUD, camera and perks remain connected.
- Health/state policy and repeated changes behave as documented; old callbacks
  cannot mutate a removed form. Mod removal/reload does not retain a queued swap.
- Wiki, editor contracts and an original-style playable example ship with the API.

## Simulation boundary implemented

`Fight.Render()` calls `RenderFight()` (model updates, collisions, AI, animation
selection, round resolution and hit-data reset), then drains host-only transition
requests before camera rendering. Pausing skips both simulation and draining.
Requests capture the current round and model identity; only the two active living
fighters may queue, with one pending request per fighter. Reentrant requests wait
for a later simulation step. Unload closes the queue and cancels outstanding work.
Completion receives an application exception or cancellation rather than treating
queue acceptance as successful replacement.

`Tools/Tests/TestModelTransitionBoundary.ps1` exercises the actual queue and Render
method with controlled model/simulation/camera services. It covers ordering, pause,
duplicate requests, stale rounds/models, death, reentrancy, application/completion
errors and unload during draining. This does not test actual native replacement.
There is deliberately no public Lua form API or production request producer yet.

## Prepared-model ownership implemented

`Fight.PreparedFormModel` copies the destination ModelParameters container, creates
a hidden Model and calls native initialization without registering it in Fight.
Before constructing that model, `ModelLoader.RequireModelDocuments` loads every
named document into the native loader cache and requires Scene/Figures. Missing
documents throw with their path; an empty composition is rejected. The native
empty-model sentinel is skipped. This matters because ordinary `ModelLoader.Load`
logs missing files and continues. Ordinary loading behavior remains unchanged.
It owns cleanup until `Take()` transfers the initialized model to its eventual
registration owner. Abandoned or failed preparation disposes the model. Shared
catalog item definitions remain shared according to the existing parameter copy
constructor; this is not a deep clone of the content catalog.

`Model.IMFOFFFLGOM` and `Clear` now tolerate absent model, condition and event
objects after partial initialization. `Tools/Tests/TestPreparedFormModel.ps1` exercises
the production preparation/cleanup methods with controlled loading and Unity
services, covering hidden preparation, parameter isolation, transfer-once,
disposal and failed loading. Managed compilation passes. Actual native resource
loading and its side effects remain an integration acceptance requirement.
The fixture also extracts the production document preflight, checking missing
files, empty paths/compositions and absent required structural elements with a
controlled document cache. This is not complete rig-reference validation.

## Animation detachment corrected

Native `SelectAnimation.RemoveModel` previously removed only active membership
and delayed events owned by the model. It left selector listeners attached and
retained delayed events targeting the removed model. Removal now detaches the
eight subscriptions added by AddModel, purges both event directions, removes
birth/created-model/trigger references and keeps cached condition indices aligned.
Unrelated listeners and queued events are preserved. The production-method fixture
`Tools/Tests/TestAnimationModelRemoval.ps1` verifies this with controlled dispatcher/model
services; managed compilation passes. Replacement must still run outside selector
iteration, using the established frame boundary.

## Camera/viewer replacement implemented

`Tools/Tests/TestAnimationNodeRebind.ps1` now exercises the actual DistancePoint cache
fields, update and lookup methods with controlled node lookup. It confirms main
player/opponent rebinding, reverse restoration including pivots, and independent
child identities. Native missing-node lookup writes null silently. The former
form-only `RequireCompleteNodeBindings` check was removed on 2026-09-16 after a
real fight rejected `Ranged-Node2_1` on a valid baton form. `Model.KMKOHGBJNBK` binds
both participants' candidate animation/trigger sets before selection, so a point
can belong to equipment or a child absent on the current body. Rebinding must clear
the retired node/pivot instead of rejecting or retaining that stale reference.
The regression now covers absent ranged points, present body nodes, other-side
and child isolation, and reverse restoration. Required model-document preflight
is retained. Validation of a selected animation's complete rig remains separate.

`SelectAnimation.ReplaceModel` now also preserves the selection slot and refreshes
the replacement's condition snapshot and eight subscriptions. It uses the removal
cleanup above for stale events. Native UpdateAnimationParameters updates shared
animation node bindings by fighter side; on a binding exception the method attempts
to restore the old side before propagating failure. That restoration can itself
fail and is not proof of complete native rollback. The expanded removal fixture
passes success/order/listener checks and an injected binding failure with controlled
binding services. Actual animation-definition cache behavior still needs native
integration verification before public form switching.

`ViewerModel.ReplaceModel` replaces the expected object in its existing slot and
updates primary-fighter pointers, after preparing replacement parenting/color.
`Camera.ReplaceModel` validates the corresponding camera slot and player-focus
nodes, transfers render-event subscriptions, retains the old index and updates
focus references for player replacements. It does not enable the hidden model
or dispose the old model; the eventual coordinator owns those decisions.

`Tools/Tests/TestCameraModelReplacement.ps1` extracts both production methods. Controlled
Unity/renderer checks cover stable slots, primary references, focus, subscriptions,
stale/duplicate identities, missing focus nodes, slot mismatch and parenting
failure. Managed compilation passes. This does not prove rendered visual continuity
or camera interpolation during a real form change.

## HUD refresh implemented

`ScreenModel.RefreshForm` checks the expected parameter identity, binds the new
parameters and refreshes portrait, health/shields and name through existing native
presentation helpers. It deliberately does not call Init: that resets style/combo
and adds button listeners. Round UI/control initialization remains intact.
The coordinator must invoke this only after deciding the new live parameters.
Managed compilation and 1,282 Underworld runtime assertions pass; those tests do
not prove visual HUD refresh. AuditUnderworld reports missing raid sprite paths,
so its execution is not evidence of complete asset availability.

Further references confirmed: ControlPress/ControlRelease resolve the current
fighter by side for each event, but held-input state still needs transfer/reset
policy. Fight separately retains NMNCKBPFCCP/AKBNKDBHCEO and opponent parameter
lists for round results; RuleInitData also contains model/parameter references.
These cannot be left pointing at the previous form by the coordinator.

## Environmental rule node rebinding implemented

RingOutRule, LoseFallRule and HotGroundRule prepare replacements for their cached
rig nodes without resetting timers or reinitializing rules. RulesInspector first
collects all assignments; a missing required node rejects preparation before any
assignment is committed. Matching uses the old node reference, leaving the other
fighter's bindings alone. TestRuleModelRebind passes with extracted production
methods and controlled node services, including a missing later hot-ground node,
all three rule types and reverse rebinding. Attribute and interval rule effects
still require policy when preparing the new form; they must not be blindly
reapplied to the existing fighter.

## Reversible registration stage implemented

`Fight.FormRenderBindings` composes rule-node preparation, camera replacement and
animation-selector replacement. Dispose reverses these registrations unless Commit
was called, and attempts remaining restoration steps if one fails. Constructor
failure reports both application and rollback exceptions when necessary.
`Tools/Tests/TestFormRenderBindings.ps1` verifies this production orchestration with
controlled camera/selector/rule services; the boundary regression and managed
compile pass. The stage captures both pending animation event queues and the
birth, created-model and trigger lists before exchange. After successfully reversing
selector registration, rollback restores their original record identities and order.
TestAnimationModelRemoval exercises the production capture and replacement methods;
TestFormRenderBindings checks restoration ordering, commit, rejection and failed
reverse registration. These snapshots are only valid for synchronous exchange
between simulation steps, with no intervening event processing. This is not full
transaction rollback: the stage does not change participant parameters, perks,
visibility or model ownership.

Next implementation: connect the prepared model to coordinated replacement of all
native registrations in a fixture before adding a public request producer.
Application failure reporting is implemented; rollback of a partially applied
replacement is not. The source inspection above rules out wrapping Model.Init as
a complete implementation; it does not prove replacement atomicity.

## Enemy targeting exchange implemented

`Model.ReplaceEnemyForm` replaces a registered opposing fighter and its weapon
children, preserving unrelated enemies. It updates matching animation, nearest-enemy
and delayed-event target references and the AI's enemy weapon category. Moving to an
unarmed form clears the previous category. The returned rollback restores the exact
original list (including order and duplicates), target references and category;
application exceptions restore that snapshot before propagating.

`Tools/Tests/TestEnemyFormReplacement.ps1` extracts this production method and the AI
snapshot method. Checks cover weapon children, direct references, unrelated targets,
unarmed replacement, stale identity and failure after category mutation. Managed
compilation passes. Animation services and AI category resolution are controlled;
this does not establish collision behavior in a running fight.

FormRenderBindings now calls this for surviving registered fighters and their
weapon models that target the old form. A set prevents double exchange of weapons
also present in the fight list; models owned by the retired fighter are excluded.
Target exchange precedes selector rebinding, and rollback restores targeting before
reversing selector registration. The orchestration fixture covers child targeting,
duplicate membership, retired children, a later observer rejecting the exchange,
selector rejection, commit and continued restoration after a target rollback error.

The coordinator still must initialize the prepared form's own opposing links.
There is no public producer. Existing projectiles, collision caches and perk action
ownership still need coordinated policy; the fixture is not a full-game playtest.

## Detached perk registration preparation implemented

PerksStage.PrepareModelRegistration builds a new PerkModelStruct and its trigger
lists before publishing it. AddModel now uses this path: a malformed later perk
cannot remove the existing registration or leave a partial replacement visible.
Successful registration keeps the recovered remove-then-append ordering. Source
inspection confirms OPACOCIKEOL only populates the new registration's trigger/data
lists; PerkInfoItem.EIKAGOOJOCN selects existing trigger definitions, and PerkData's
constructor only stores its definition and enabled state. No actions run here.

TestPerkModelPreparation extracts preparation/AddModel/RemoveModel and injects a
failure during a later trigger build. Detached preparation, preserved existing
registration, failed fresh registration, successful replacement and null rejection
pass; managed editor compilation passes. Trigger construction is controlled.

Active effects still need separate handling. InfoPerk holds both queued and active
ActionPerk lists; ClearActions(true) only processes active actions and invokes
native expiration behavior. Reassigning targets alone would make expiration undo
effects on a new body where those effects were never applied. PerksStage also
retains actions in JLAKGOEOHMN and the static namespace registry PNAALKAHAKG. Neither
removing a PerkModelStruct nor rebuilding its triggers resolves these references.
The form coordinator must explicitly migrate or finish affected effects and queued
actions before retiring the old model; this preparation helper does not do so.

## Queued perk reference rebinding implemented

PerksStage.RebindQueuedFormActions snapshots and retargets source/target references
in pending InfoPerk actions, without moving containers or changing timing fields.
It deduplicates shared action records and excludes any record already present in
active, recent-action or namespace registries. FormRenderBindings now calls it and
restores those references on rollback. TestPerkModelPreparation covers both source
and target, shared identities, all active-alias exclusions and unchanged timing;
TestFormRenderBindings covers its orchestration. Both and managed compilation pass.
This does not migrate applied effects or preserve retired registration containers;
those remain required coordinator work.

## Applied attribute amounts retained

ActionPerk now records normalized attribute deltas when an attribute modifier
starts; copies preserve an independent dictionary. InfoPerk expiry subtracts those
recorded deltas instead of reevaluating expressions against changed fight state.
Actions without a record retain legacy expiry evaluation. All expressions resolve
before any attribute mutation, so failure in a later expression cannot apply an
incomplete modifier. This supplies applied amounts required for eventual body
transfer, but does not itself migrate effects or characters.

TestPerkAttributeLifetime passes original-delta expiry, unrelated changes, later
expression failure and unrecorded fallback with the production method and controlled
attribute services. Perk preparation and form registration regressions and managed
editor compilation pass. Full native effect/form playtesting remains pending.

## Attribute-effect transfer connected

InfoPerk.TransferAttributeEffect now moves one active attribute effect from the
old body's attribute container to the new body using its recorded deltas. It
preserves the action/timer and updates matching source/target references. Clone
calculations reject overflow before live mutation; rollback restores both original
containers in place, including absent keys. Missing applied records, wrong targets
and shared attribute containers are rejected.

PerksStage.TransferFormAttributeEffects composes active attribute effects and
reverses earlier transfers if a later one fails. FormRenderBindings invokes this
stage and retains its rollback. TestPerkAttributeLifetime exercises the production
per-effect and stage methods: two-body values, later expiry on the replacement,
overflow preflight, multi-effect composition and rollback after a later failure.
TestFormRenderBindings and managed editor compilation pass. Attribute normalization
is controlled in the fixture. Other effect types, registration ownership and
full-game form switching remain unfinished; this is not a public API addition.

## Ongoing health-effect transfer connected

InfoPerk.TransferHealthEffect retargets active ModHealthChange source/target
references without running the effect or modifying its timer. The native initial
health-effect handler is empty; Render applies health changes through DDOGCEKKDMK,
so the next scheduled tick can follow the new body without copying a persistent
attribute mutation. Source-only changes preserve the affected opponent.

The form effect dispatcher is now named TransferFormEffects and handles both
attribute and health effects. FormRenderBindings retains its combined rollback.
TestPerkHealthTransfer extracts transfer, Render, CAIPNAAJICO and DDOGCEKKDMK; damage,
healing, no extra transfer tick, original expiry timing, source-only changes,
rollback and expired-action rejection pass. Health application and expiration
dispatch are controlled. Attribute and form registration regressions and managed
compilation also pass. Remaining effect types and registration ownership still
prevent claiming complete form switching or full native expiry integration.

## Participant identity connected to registration transaction

Fight.BindFormParticipant now swaps the simulation-list slot, active model,
round parameters and opponent sequence/tactic together. It moves shield objects
and opponent/innate Lua instance nodes without recreating them, preserving their
identity and state. Both source and replacement identities are validated before
mutation. The registration transaction invokes it after camera, selector, perk
and rule registration; rollback restores participant identity before those
systems unwind. This remains a synchronous internal stage, not a public API.

TestFormParticipant executes the production binding with controlled model data
for player/opponent, round and sequence references, shield/behavior identity,
unrelated state, rollback and stale/colliding identity rejection. It passes,
as do TestFormRenderBindings, TestModelTransitionBoundary and managed editor
compilation. The interrupted perk-registration edit also compiles and passes
orchestration checks. Full active-effect migration, statistics/controller and
HUD handover, resource retirement, Lua requests and a transforming encounter
remain necessary before claiming a usable form workflow.

## Combat history, input and HUD handover connected

The form registration transaction now transfers held-key/combo input state while
retaining each controller's body-specific event subscribers. Combat statistics,
model statistics ownership, action-button cooldowns, round/control state and
recorded combat counters transfer together with an idempotent rollback. The
replacement retains its own animation/physics/AI implementation. This does not
claim an arbitrary in-progress attack can resume across different rigs.

Fight presentation now transfers its 13 event listeners and refreshes the
appropriate native HUD panel. If refresh throws after assigning parameters,
rollback restores the previous panel and still restores listener ownership.
Neither transfer invokes a fight-begin or round-begin event.

TestFormCombatState runs the complete native ModelController and KeyData with the
production model transfer method, checking held release, subscriber isolation,
combo expiry against uninterrupted input, history/cooldown identity and rollback.
Statistics storage/event dispatch are controlled. TestFormPresentation exercises
the production presentation method against controlled panels including render
failure and stale identity, and audits symmetry of all 13 production listener
pairs. Both pass, alongside participant, queue, form-registration, attribute and
health-effect regressions. Managed editor compilation and 1,282 Underworld
runtime assertions pass. AuditUnderworld still reports existing missing raid
sprite assets; this is not a clean native asset or full-game acceptance result.

Resource retirement/visibility/start animation, remaining effect types, public
Lua requests and a playable transforming encounter remain unfinished. No public
form capability or roadmap milestone closure is claimed.

## Applied body modifiers follow replacement

TransferFormEffects now copies applied impulse, hit-effect scale, added damage,
color, slow factor plus its current frame phase, and collision suppression to the
replacement without evaluating perk expressions or restarting timers. Active
modifier actions retarget the replacement, preserving the action objects used by
normal expiration. Existing side-owned icons and fight-owned areas retain their
objects. Invisibility references also retarget; final visibility handover must
preserve the original object's active state. Source-only effect references move
without changing the affected opponent. Attribute and health transfer remain on
their dedicated paths.

TestFormModifierTransfer passes production copy/dispatch with controlled renderer
and action storage for the eight numeric/presentation types, attribution, timer
preservation, independent impulse vectors and rollback after later failure.
Invisibility visibility acceptance remains pending final resource handover.
Attribute regression and managed editor compilation pass. This is internal
integration, not a public capability or full native expiry playtest. Stolen magic,
remaining action/history references and resource retirement still require work.

## Queued host workflow and ownership commit connected

QueuePreparedFighterForm now composes the existing preparation, frame boundary,
registration/state/HUD transfer and commit. It captures health fraction, round
wins, position and facing at application time, builds replacement enemy links,
and prepares native idle/transition selection for the next model step. Accepted requests
own their prepared body; rejection leaves ownership with the caller. Cancellation
or application failure releases the unused preparation after registration rollback.

CommitPreparedForm validates transaction identity and checks active, pending,
recent and namespace perk references across the retired body and its helpers.
Untransferred references reject before visibility or ownership changes. Successful
commit preserves the original active state (including invisibility), transfers
preparation ownership, removes retired queue entries and disposes retired bodies.
Cleanup errors after ownership commits are logged rather than falsely reporting
that the swap failed and disposing the active replacement.

TestPreparedFormRequest and TestFormCommit pass extracted production orchestration
with controlled native/boundary services. They cover live state capture, ownership,
deferred entry animation, rejection, cancellation, rollback, nested helper retirement and
post-commit cleanup failure. TestFormModifierTransfer now also checks the actual
retirement-reference gate for active, queued, recent and namespace-only references.
Form registration, boundary regressions and managed editor compilation pass.

The host path is connected but is not yet exposed to Lua. Destination content
projection, remaining active/history effect references (including stolen magic),
native animation/rendering acceptance and the playable transforming example remain
required. A guarded rejection is not evidence those effect cases are supported.

## Registered character projection connected

TryQueueCharacterForm now resolves a registered warrior through ModRuntime and
LegacyContentAdapter, reusing the encounter warrior projection. ListSF creates
owned XML and uses the native template merge or direct warrior parser. Missing
templates reject explicitly. The fight applies native item rules for the current
side/round, computes model paths and attributes, prepares the body and queues the
host workflow. This internal path compiles; native loadout/render acceptance is
still required before exposing it as a verified public capability.

Expired actions in JLAKGOEOHMN now retain attribution through a form swap without
replaying their effects. Active aliases remain on effect-specific transfer paths.
Namespace-only records are not assumed expired: native registration evidence does
not establish that, so the retirement gate continues to reject unresolved cases.
Tests cover history alias identity and rollback, modifier/attribute regression,
parameter routing/XML ownership/side/template rejection, and queued preparation.
All pass with the documented controlled services; managed editor compilation
passes. Public Lua binding, remaining effect cases and playable native acceptance
remain unfinished.

## Public experimental request and visual encounter

fighter:change_form accepts a warrior handle owned by the calling Lua context and
requires combat.transform. Its live receipt distinguishes queued, applied and
failed; expected preparation rejection returns a failed receipt without requiring
pcall (not exposed by the sandbox). Invalid handles/capabilities and expired
fighter operations remain errors. The instance wrapper forwards the native form
operation so opponent and item behavior contexts retain the capability.

example.shifting-guardian and the matching editor starter expose a three-second
staff-to-baton request with original-font HUD feedback, without claiming success
until the host completes. Round/fight teardown closes the view. A failed request
is displayed, not silently retried. The manual checklist and public reference
cover known limits and how to locate the separate map encounter.

Verification: 969 actual MoonSharp showcase assertions (including queued receipt,
completion, cancellation and preparation rejection), 247 isolated Unity UI checks,
27 editor contract tests, LuaLS result-type inference and VS Code integration all
pass. The wiki builds 48 pages, 142 reference entries and 4,101 local links/assets.
The new HUD preview was visually inspected. Managed editor compilation passes.

These checks do not execute the body swap in a complete native fight. Active
stolen magic and unresolved effect cases still require implementation/acceptance.
The E5 milestone remains open; the Lua request/example is now a usable test entry
point, not evidence of full character-form or overall roadmap completion.

## Magic charge and parameter copy verification (2026-09-16)

`Model.ExchangeFormCombatState` transfers the partial magic charge and ready-cast
count with the ongoing fighter. The reversible exchange does not normalize charge,
consume a cast or emit a magic-button event. `TestFormCombatState` reproduced the
lost charge before the fix at `Temp/CharacterForms-d85gx04c`; the updated fixture
executes native charge clamping, accumulation, readiness, cast consumption and UI
event production. Charging and casting match an uninterrupted fighter through two
swaps; rollback and retired-body clearing preserve both owners' state. The event
sink is controlled, so these checks do not establish rendered magic-button behavior.

`ModelParameters` copies now retain independently owned animation and perk exclusion
lists. `TestFormParameterCopy` executes the production constructor, native rule
application, animation filtering and equipment-perk aggregation for both sides.
Six of eighteen checks failed before the copy fix; all eighteen pass afterward,
including repeated copies and isolation from clearing the detached lists.

This establishes copy integrity, not complete fight-rule inheritance. Fresh form
projection still needs the current side's move/perk restrictions and rule-provided
perks applied before native preparation and registration. The ordinary fight paths
are `Fight`'s initial rule application and `GGJJDLNDFLF`; the new regression does not
claim those steps run through `TryQueueCharacterForm`.

## Baseline native form acceptance (2026-09-16)

The isolated Unity 6000.6.0f1 game now passes `Tools/Tests/ValidateFormNative.cs`:
registered Lua content starts the encounter, changes the enemy from staff to steel
batons at frame 180, retains health ratio 0.75 and numeric/text perk variables,
preserves participant control/AI eligibility, reports Applied in the Lua HUD state,
and remains active with a selected animation for 120 later combat frames. The
timer does not reset. The validator records any combat exception after initial
body readiness and fails from its next update, so an advancing clock cannot hide
aborted simulation. Run with `python3 Tools/Tests/TestCharacterForms.py --native`.

This required three repairs demonstrated by failed native runs: normal fighter
initialization for health/moves/default body; nullable rebinding of candidate move
nodes absent from the new equipment; and entry-animation/AI observation startup.
`PrepareFormAnimation` evaluates only the new body's animation-end candidates
through the existing conditions, priority and transition machinery. It schedules
the first frame before commit, without dispatching a round or trigger event.
`ModelAi.Render` initializes its missing enemy observation through the recovered
animation observer before reading statistics. The first frame executes normally
after ownership commits.

Passing evidence: `Temp/FormNative-oedb5l4p/NativeRuns/Run-q5834g5a/validation.log`
(exit 0), with the exact command/input hashes beside it. Earlier failing runs
remain in the same fixture. Native acceptance is limited to this enemy-form
encounter and interval; pause/replay/round teardown, player forms, broader rigs,
active stolen magic and current-side restriction/rule-perk inheritance remain open.
