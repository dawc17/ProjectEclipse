# Roadmap status and measurement

Last broad reconciliation: 2026-09-12. Scope remains **G01–G14 plus E1–E8**.

2026-09-20 continuation (Step 41): pending Lua assembly now connects twelve
normal/Eclipse Sensei battles, 23 fights, 34 loadouts, 57 reward slots and 56 map
translations. A generic native map fix prevents locked normal entries from
exposing unlocked Eclipse counterparts and preserves active intermission pairing.
Conditional RaidCharge behavior and missing guard/prince dependencies are still
explicit required inputs; controlled graph checks do not activate the story or
establish native model/AI/gameplay parity. Active DE128 remains 0.18.0.

2026-09-20 continuation (Step 40): the pending Sensei opponent factory now covers
all 22 normal/Eclipse guard and prince loadouts and returns complete ordered
rosters together with the twelve boss definitions. All 34 source rows across 23
encounters match through actual Lua registration/projection. Guard template and
Sphere1 identities are controlled fixtures only; actual missing dependencies,
conditional controls and complete story acceptance remain open. Active DE128 is
still 0.18.0. Purchase/upgrade delivery was inspected and is already immediate in
the base runtime; no redundant policy or behavioral change was made.

2026-09-20 continuation (Step 39): pending Lua rules cover the unconditional
portions of all 23 Sensei fights. Generic C# fixes enforce all five NoButton
actions through shared input and preserve normal/Eclipse rule modes in native
projection. Ordered source comparisons and isolated Unity checks pass. The
conditional RaidCharge rule remains unattached because the recovered conditional
wrapper ignores its test. Full encounter assembly, missing templates and story
acceptance remain open; active DE128 stays 0.18.0.

2026-09-20 continuation (Step 38): pending Sensei loadouts now cover six young
bosses in both modes. Generic warrior perk settings gain explicit probability
and frame-duration overrides with validation, projection and content fingerprint
support. Twelve Lua warrior definitions match historical rows; 50 native perk
clone checks pass. This does not activate the story or prove boss AI/trigger
gameplay. Guard templates, conditional encounter rules and full assembly remain
unfinished; active DE128 stays 0.18.0.

2026-09-20 continuation (Step 37): the pending DE Sensei coordinator now sequences
six-act notifications and saves pending/opened flags through Lua. Generic C#
runtime support adds guarded native Eclipse-mode requests and an optional UI
Back callback distinct from cleanup. Controlled notification/save roundtrips,
all 56 historical translations and isolated native mode-off acceptance pass.
The package remains 0.18.0 with the coordinator outside its entrypoint. Full
dialog rendering, encounter assembly, missing guard templates, complete story
acceptance and deferred corpus reconciliation remain open.
On 2026-09-18 the owner separately authorized incremental DE128 production, one
approved step at a time. Its policies, Desolator reward, XML-evidenced combat perks and gaps
are tracked in [the DE128 production record](de128/PRODUCTION.md). This does not
close the engine roadmap or reduce either source roadmap's requirements.

2026-09-19 continuation: DE128 0.6.0 also opts saved pending forge orders into
normal instant settlement through `sf2.timers.set.complete_pending`. Package and
controlled native lifecycle checks pass; live forge/save acceptance and non-forge
delivery timers remain open. See Step 6 in the production record.

DE128 0.7.0 adds the five archived battle-pass collections (25 existing items) to
shop availability at archived player-level gates. The generic `minimum_level`
availability field is implemented; canonical prices, item power and upgrade rules
remain untouched. All 25 native icons and model texts loaded successfully. Shop
purchase/equip/save acceptance and full equipment/set parity remain open (Step 7).

DE128 0.8.0 applies four archived combat-family corrections using the generic
`sf2.items.set_subtype` API. Native move eligibility, snapshot copying and adapter
rollback are tested. ChineseSwords exposed missing move-authoring fields and
remains pending; the existing animation binary is present, so this is an API/
content-graph gap, not a missing-art excuse. See Step 8 for precise dependencies.

Step 9 implements mixed/shifted attack attributes, repeated native input sequences,
`Spinning`/`HighHeavy` reactions and native stage/effect/screen conditions. Chinese
swords combat sections are now authored in a pending Lua module and compared to
the archive through the production adapter and native parser. The shared dense
array validator also accepts nil tombstones after Lua `table.remove`. Package
behavior remains 0.8.0 until the rest of the move graph is connected; this is not
completed ChineseSwords gameplay parity.

Step 10 adds scoped item-lock alternatives, move locks, transitions, alignment
and facing through generic C# APIs. The pending Lua module now matches the ten
archived Sai lock extensions and the ChineseSwords combat/preview graph sections.
Native eligibility, projection, rollback and parser checks pass (364 combined
authoring checks). Sounds, profile/tactics metadata, preview completion and
remaining animation flags still precede activation. Active package stays 0.8.0;
live combat and preview acceptance remain open.

Step 11 closes the remaining ChineseSwords presentation authoring gaps: scheduled
native sounds, shop completion, moves-list profile, tactic distance and preview
flags. The pending Lua declarations match archived sections, and Unity loaded all
nine sound clips plus the profile icon. Complete registrations and inherited
template/native integration checks precede activation; these checks do not prove
live combat, audible playback, AI or shop-preview acceptance.

Step 12 activates DE128 **0.9.0**: both complete ChineseSwords registrations,
ten item-lock extensions and Jian's subtype change. Eleven inherited templates
match the archive; native Unity parsing and unchanged binary decoding pass.
Generic optional profile localization preserves namespaced move identity.
The 38-sample binary contains all attacks/sounds; native recovery-end clamping
matches the archived overlong interval. Full fight/AI/preview/save acceptance and
the separate archived ChineseSwords equipment item remain open.

Step 13 adds a repeatable isolated Unity fight acceptance harness and passes
actual DE128 initialization, Jian equipment/subtype, native double-tap/Forward
selection, all four attack intervals with bound weapon edges, all four timed
sound actions, animation completion and continued simulation. It injects native
input data and disables opponent AI to isolate the case; physical device input,
AI choice, hit contact, audible output, shop completion and save continuity are
not covered. The package remains 0.9.0.

The owner rejected the Ascension prototype on 2026-09-18. It is commented out;
its earlier fixture results are historical and do not count as active DE content.

## Reporting rule

A public API, a managed test, a native fixture and a game playtest prove different
things. Track implementation and acceptance separately. Do not use function count, test count or elapsed
time as completion percent.

Do not publish an overall percentage until all rows have been decomposed into
explicit requirements and their evidence reconciled. A broad track is not closed
because one of its functions passes tests. Unknown acceptance is not completion.
The present audit has freshly reassessed six G tracks; the other rows
retain the source roadmap's requirements and still need that reconciliation.

## Freshly reassessed requirements

| Track | Implemented evidence | Fresh verification | Remaining acceptance/scope |
| --- | --- | --- | --- |
| G02 programmable story | Story subscriptions and profile level/item/equipment snapshots in `MoonSharpScriptRuntimeP3` | `TestStoryApi.ps1`: 56 | Historical purchase counts, full archive operation coverage and live lifecycle ordering |
| G03 dojo/menu flows | Selection/reset/query APIs and native entry routing | `TestDojoLocationRouting.ps1`: 29, with controlled selection services | Persistence, rendered selector/unlock flow, tutorial presentation |
| G04 lotteries/chests | Actual quest presentation and persisted claim/recovery path; paid continuation explicitly rejected | `TestLotteryClaim.ps1`: 80, with controlled grant/selection/disk services | Paid spins, multiple pending claims, contextual purchased chests and live crash/reload acceptance |
| G07 perk upgrades | `MoonSharpScriptRuntime.RegisterPerk` accepts typed upgrade levels/descriptions/parameters | `TestPerkUpgrades.ps1`: 39; `TestPerkUpgradeNative.ps1`: 49 | Full-game level-up UI, saved upgrades and combat behavior; no DE content conversion claimed |
| G09 conditional AI | Programmable AI decisions, perception/animation/interval and candidate timing/input snapshots; reactive example | Source and prior recorded 172-check AI run reviewed; no fresh live fight | Named archived reaction scenarios in game; distinguish native-tree import from equivalent programmable behavior |
| G10 location motion/music | `music_choices` and typed motion/rotation/opacity tracks | `TestLocationMusic.ps1`: 6; `TestLocationMotionApi.ps1`: 36 | Audio playback, visual scenery acceptance, arbitrary particles/hazards and remaining targeted edits |

These are implementation findings, **not six closed tracks**.

## Remaining reconciliation queue

| Track | Requirement scope to assess |
| --- | --- |
| G01 | Modification/removal of existing content across the specified categories |
| G05 | Set-to-ability binding and activated abilities |
| G06 | Combat events, hit modification and effects |
| G08 | Moves, projectiles, demonstrations and input |
| G11 | Equipment metadata, defaults, innate effects and acquisition policy |
| G12 | Forge families, candidate structure, conditions and restoration |
| G13 | Achievement predicates and core localization coverage |
| G14 | Service suppression, initialization and presentation policy |
| E1 | Behavior hosts and identities; isolation/filter/lifetime/unmodded acceptance |
| E2 | Combat control, objective/result authority and outcome ordering |
| E3 | Programmable persistent story/runs, branching, seeded choices and settlement |
| E4 | Original-style owned UI fixtures, input/focus and teardown |
| E5 | Full character/animation pipeline, moves/input/AI, boss forms and rig diagnostics |
| E6 | World presentation, timed hazards, telegraph/pause/unload |
| E7 | Breadth/patching, two-mod composition, restoration and missing-mod saves |
| E8 | Creator workflow, diagnostics, compatibility and source-free newcomer acceptance |

For each row, enumerate the original requirements first, identify shipped code
and tests, then record absent features and missing runtime acceptance. Keep
overlapping G/E requirements linked rather than adding them twice into an overall
percentage. Unknown archive intent is a separate classification, not silently
removed scope or an implemented feature.

## E5 checkpoint: 2026-09-16

The experimental `fighter:change_form` path passes the baseline Shifting Guardian
native fight test in an isolated Unity 6000.6.0f1 project: staff to steel batons at
frame 180, retained health/variables/control role, Applied Lua HUD state and 120
later animated combat frames with no timer reset or captured combat exception.
Evidence and the production regressions are in `CHARACTER_FORM_RUNTIME_AUDIT.md`
and `PRE_DE_WORK_LOG.md`; the repeatable launcher is `Tools/TestCharacterForms.py`.
This is one verified E5 case. Active stolen magic, current-side restriction and
rule-perk inheritance, player forms and broader rig/effect acceptance remain open.

Sources: `DE_XML_API_GAP_AUDIT.md`, `MOD_ENGINE_EXTENSIBILITY.md`,
`PRE_DE_TEST_CHECKLIST.md`, actual bindings and native adapters. Historical work-log
entries locate evidence; they do not by themselves prove current completion.

DE128 0.10.0 restores nine missing archived weapon listings through Lua, with
archive-matched act/level gates, prices and default enchantments. The generic
owned-equipment adapter now projects `required_group` into the native quest
notification pack label. Native boot/stat/enchantment/art checks and the existing
Jian combat acceptance pass. Moon Fans remains pending: the archive omits its
damage attribute, whereas current vanilla progression assigns 760. Initial-stat
authoring, purchases/equipping/save acceptance and full equipment parity remain
open. See Step 14 in the DE128 production record.

DE128 0.11.0 closes the initial-stat gap identified in Step 14 and restores Moon
Fans, completing the ten missing archived weapon definitions. All five equipment
registration functions now accept typed `initial_stats`: omitted derives normal
power, an empty table preserves absence, and populated tables supply exact values.
Native Moon Fans comparison preserves missing damage and its normal first upgrade
(766 damage). This does not close purchase/equip/save acceptance or full DE parity.
The next equipment audit covers 12 missing armor, 15 helms, two ranged items and
seven magic items, including hidden NPC equipment that must not become shop stock.

DE128 0.12.0 adds eight missing non-weapon shop definitions through Lua: three
armors, two helms, Dragon Boomerangs, Dragon's Breath and Lightning Arc. Native
stat-presence/listing/enchantment/asset checks pass, including Samurai Armour's
head-only stats. Shared Chakram/MassBomb/LightningArrow move changes remain open;
an authored audit records those differences. Five other spells have available
assets but missing move graphs, while hidden NPC equipment remains unregistered.
See Step 16 for the full acceptance boundary and evidence.


DE128 0.13.0 adds five Lua-authored shared move changes through the generic typed
`sf2.moves.patch` API: ranged uninterrupt duration, Chakram reaction, preview sound
timing and two not-Stun conditions. Native condition evaluation and patch rollback
before/after initialization pass, alongside existing equipment and Jian acceptance.
Remaining presentation differences and five absent spell graphs are still open;
this does not close combat, purchase or save-continuity acceptance. See Step 17.


The next DE128 production step audits all 32 missing spell moves and their
transitive template/resource sources. Generic scheduled effect start, stop and
stop-follow actions are now Lua-authorable with native parser tests, fingerprints
and editor/wiki support. Spell registrations remain pending projectile creation,
charge handling and graph requirements; package version remains 0.13.0. Source
file presence and parser checks do not prove rendered gameplay. See Step 18.


Step 19 adds generic scheduled projectile creation with inherited equipment,
optional owned start moves, charge changes and actor deletion. Actual Unity parsing
matches the archived Sphere1 action data; live projectile lifecycle acceptance and
full spell graph registration remain open. DE128 stays at 0.13.0 pending those graphs.


Step 20 adds native actor-name/charge selection conditions and move velocity,
acceleration, velocity preservation and magic-recharge suppression. Predicate tests
and real Unity move parsing pass; live projectile/spell acceptance is still open.
Wall-distance conditions and spell attack options remain before graph integration.


Step 21 adds signed distance conditions and native spell attack options for defense,
critical/effect suppression, block and named invulnerability bypass. Sphere1 parser
comparisons and signed predicate tests pass. The next content step is the complete
Sphere1 Lua graph and live acceptance; later spells' additional reaction/edge-free
attack requirements remain explicit. Package version is still 0.13.0.

DE128 0.14.0 Sphere1 now passes all nine move declaration comparisons and native
Magic-input casting, startup-to-flight selection, charge consumption and child
deletion. The earlier flight check failed because startup contact deleted the
projectile; restored test spacing resolved it without gameplay changes. Numerical
damage, wall-miss cleanup and presentation acceptance remain open; see the production
record. This does not close spell parity.

DE128 0.15.0 adds all nine Sphere2 moves and Medium Charge of Darkness through Lua.
Full graph comparisons and native casting/flight/charge/deletion pass. Three spell
graphs and broader presentation/damage/save acceptance remain open; see Step 23.

DE128 0.16.0 adds Sphere3 and Large Charge of Darkness in Lua. Generic native
Physycal reaction support, complete graph comparisons and native cast/middle/charge/
cleanup pass. ComboSphere3 and MindThrowNormal remain; Step 24 records limits.

DE128 0.17.0 restores ComboSphere3 and Blast of the Void through Lua with generic
HighLong support. Archive comparisons and native casting/attack-phase/charge/
cleanup pass. MindThrowNormal remains the missing spell graph; Step 25 records
remaining damage, presentation and save acceptance.

The owner-supplied complete DE corpus now supersedes repository DExml as content
authority; acquisition is blocked by Drive quota. See de128/SOURCE_CORPUS.md.
MindThrow direct attacks, NoReaction, voice sounds and camera-shake primitives
are implemented; its graph and corpus reconciliation remain open (Step 26).

DE128 0.18.0 adds the six-move MindThrowNormal graph, its innate Lua behavior,
five unchanged recovered animation binaries and Mind Throw shop listing. Full
graph/template comparisons and actual-package checks pass. An isolated Unity
fight passed grounded contact, the owned victim reaction, innate flag expiry,
caster follow-up and final direct attack interval. The initial crouched fists
stance can duck the recovered projectile; broader stance/presentation/damage and
purchase/save acceptance remain open. Source-corpus reconciliation is still
deferred at the owner's request. See Step 31 in de128/PRODUCTION.md.

Step 32 adds generic per-warrior native enchantment settings, documented Lua/editor
contracts and pending normal/eclipse young Lynx definitions. Actual Lua projection
matches historical Act I XML; isolated native perk cloning confirms the eight
instances, inheritance and isolation. The DE entrypoint remains 0.18.0 without
Sensei's Story: referenced Guard_Girl/Guard_Man templates are absent from available
XML, and story gates/rewards/presentation/encounter acceptance remain open.
The deferred complete corpus must still be reconciled. See de128/PRODUCTION.md.

Step 33 closes the reward representation gap with generic experience and native
performance-base fields. Pending Lua data covers all 57 Sensei reward slots over
six acts. Historical-source comparison passes both recovered parsing and native
FightResult calculations at two scales, alongside existing package/engine checks.
The story remains outside the entrypoint; guard templates, progression, encounter
assembly and settlement/save/reload acceptance remain open. See Step 33 in
de128/PRODUCTION.md for evidence and limits.

Step 34 adds read-only saved fight progress and pending six-act Lua unlock
decisions. All 2048 prerequisite histories are checked against archived quest
conditions; production native roster tests verify counters, freshness, no write
on read and profile unavailability. Notification ordering, map lock changes and
opened-state persistence remain to be connected before story activation. See
Step 34 in de128/PRODUCTION.md.

Step 35 implements owned battle lock updates through `story.progression` on an
unblocked map. It preserves other saved battle fields and refreshes native lock
buttons. Map reloads now retire old zone objects instead of leaving duplicate
buttons active. Lua, native map and existing combat checks cover this foundation;
initial revelation, notifications, persistent opened flags and full Sensei story
assembly remain unfinished. DE128 stays 0.18.0; see the production record.

Step 36 adds generic owned-battle reveal and focus APIs, including native saved
entry creation, preservation on repeated reveal and immediate map selection.
Pending Sensei Lua sequences all six first-act reveals and subsequent act unlocks
and focus; refusal/retry and archived action ordering are checked. Native Unity
acceptance covers the APIs and prior combat checks. Dialog ordering, Eclipse-mode
transition, opened-state persistence and story assembly remain open (0.18.0).

Step 42 adds per-behavior player control restrictions in the C# runtime and the
pending Sensei conditional RaidCharge Lua factory. Claims compose with native
rules, cannot grant unavailable actions, and clear at round setup/fight end.
Actual Lua, native Unity and editor contract checks pass. The real perk-state
reader and native ability availability remain unresolved, so this does not
activate the Sensei story; DE128 stays 0.18.0. See the production record.

Step 43 ports the six pending post-victory Sensei sequences (23 cards, 448
translations) into Lua with saved cursors, completion flags and notification
ordering. Reusable C# image mirroring preserves Widow's presentation. Actual Lua
and isolated Unity UI checks pass; portrait assets and the final timed ActScreen
remain explicit unverified dependencies, and intro/defeat/full-story integration
is unfinished. Active DE128 remains 0.18.0; see its production record.

Step 44 replaces the pending outro presenter with the C# `sf2.ui.act_screen` API:
localized timed lines, native fades/music, independent input leases, bounded
completion and cancellation on teardown. The pending victory factory uses it
directly; the story stays disabled while asset/identity, perk-state and remaining
narrative gaps are unresolved. Active DE128 remains 0.18.0. See its production
record for verification and remaining integration limits.

Step 45 adds owned-fight map-entry continuations in C# and ports the 17 pending
Sensei introductions/greetings into Lua (39 cards, seven timed lines, 938 translated
values). The normal native launch resumes after acknowledgement, with scoped
cancellation and saved one-time flags. Native continuation and source/Lua tests
pass; story activation still awaits verified assets/identities, faithful perk
availability and remaining defeat/campaign integration. Active DE128 is 0.18.0.
