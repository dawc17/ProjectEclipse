# Roadmap status and measurement

Last broad reconciliation: 2026-09-12. Scope remains **G01–G14 plus E1–E8**.
On 2026-09-18 the owner separately authorized incremental DE128 production, one
approved step at a time. Its policies, Desolator reward, XML-evidenced combat perks and gaps
are tracked in [the DE128 production record](de128/PRODUCTION.md). This does not
close the engine roadmap or reduce either source roadmap's requirements.

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
