# Archived DE XML versus public API 0.7

Reviewed 2026-09-10. Plan tasks: **P4.2 intentional-delta inventory and P5.1/P5.4 coverage review**. This is an audit, not an implementation or a declaration of DE parity.

**Conclusion: significant API and host gaps remain.** The accepted Phase 1â€“3 showcases demonstrate useful working slices, not the complete DE XML vocabulary or all original phase exit criteria. A downstream DE conversion cannot currently reproduce the complete archived content through the supported public API alone.

## Scope and reproducible evidence

Compared every XML file beneath `Assets/DExml` and `Assets/vanillaXml`, including animations, locations, translations, quest extensions and compatibility overlays:

| Inventory | Count |
| --- | ---: |
| Archived DE XML files | 212 |
| Canonical base XML files | 164 |
| Union of XML paths | 219 |
| Shared files with differences | 83 |
| Shared files equal after ignoring formatting/comments | 74 |
| DE-only files | 55 |
| Base-only files | 7 |
| Structural delta records | 44,955 |

The delta count is **not a feature count**. A changed attribute, reordered collection and complete added subtree are different kinds of records. Numeric strings remain exact. Named children are matched by identity and occurrence; anonymous children by position. This preserves the evidence but can describe a moved/renamed record as removal plus addition. Translations and text dominate many records.

Artifacts:

- [File inventory](DE_XML_FILE_INVENTORY.json): all 219 paths, source hashes, domain, equality/add/remove status and delta count.
- [Complete delta ledger](DE_XML_DELTA_LEDGER.json.gz): compressed UTF-8 JSON with before/after attributes, text, order and complete added/removed subtrees. Compression avoids committing a very large expanded text dump.
- [Named feature index](DE_XML_FEATURE_INDEX.json): every added/changed/removed item, set, perk, move, move template, main-file quest and achievement counter name, plus event/action/condition vocabulary used by changed or added records. Quest extensions remain individually covered by the complete ledger.
- [File-by-file coverage assessment](DE_XML_FILE_COVERAGE.md): every path linked to the findings below. A domain-level partial assessment does not certify each child record as supported.
- [Audit generator](../Tools/AuditDEXmlApi.py): `python Tools/AuditDEXmlApi.py --write` regenerates evidence; run without `--write` to detect XML source drift.

The reference is the archived DE tree, **not the creator's forthcoming release**. Canonical vanilla already contains the owner's requested economy edits. Archive differences are evidence of required expressiveness, not proof that every difference was intentional DE design; source-version drift and compatibility repairs remain possible. Non-XML JSON, APK code and binary art/animations are outside this inventory. Their absence/semantics can still block a feature. Existing [P3 configuration audit](PHASE3_CONFIGURATION_AUDIT.json) separately covers selected JSON/configuration differences.

## What DE changes

These are identity-based counts within the indicated files, not net counts of distinct playable features:

| Domain | Archive differences | Assessment |
| --- | --- | --- |
| Equipment and other items | `list.xml`: 55 added, 617 changed, 36 removed named items | Simple owned equipment works; metadata, acquisition, built-in enchantments and core edits are incomplete. Many numeric changes need economy classification. |
| Sets/abilities | 13 added and all 12 existing set records changed | Membership exists; ability linkage does not. |
| Perks | 6 added, 68 changed, 32 removed | Existing-template reuse is useful, but cannot express arbitrary new trigger logic. |
| Move definitions | 93 added, 938 changed, 87 removed; templates: 3 added, 6 removed | Broad authoring gap beyond the demonstration step. |
| Main quests | 275 added, 16 changed, 54 removed in `quests.xml` | Story/UI/acquisition logic exceeds the current quest surface. Includes must be considered separately. |
| Main stage content | Named battle examples: Ascension, lotteries, ambush/challengers and two Sensei memories battles; widespread existing fight edits | Fight registration works; targeted core edits cover only three fields. |
| Raid content | 3 base battle records versus 76 DE battle records | Raid loop foundation works; individual boss moves/perks/art/rules still need conversion and verification. |
| Progression | Replaces move unlock choices with Master of Style/Relentless branches and adds per-upgrade parameter/description records | Branch overlay works; level-specific perk payload is missing. |
| Achievements | 5 added and 89 changed counter records | Most shared edits are platform IDs, not new gameplay. Story/equipment/forge predicates still need supported event/query access. |
| Locations | 23 new, 28 changed, 55 equal; 2 base-only | Static layers work; animated scenery and changing the dojo are incomplete. |
| Languages | 5 new translation files, 9 changed shared translations plus metadata changes; 3 base-only translations | Owned strings/new locale metadata work; complete core string replacement and asset/font validation remain necessary. |
| Other files | Defaults, credits, device/internal/logger configuration, compatibility overlays, removed pack/CDN files | Mixed presentation, platform, already-base-owned policy and unresolved intent; not blanket API requests. |

All **11 DE-only quest extension files are empty roots** in this archive, including `sensei_arc.xml` and event/battle-pass filenames. Do not advertise them as implemented event content merely because they exist. Substantial Sensei/Ascension/dojo content is in `quests.xml` and `stages.xml`. Many shared extension files are emptied or no longer included; replacing them with extra registered quests would leave old behavior active unless removal/suppression is addressed.

## Implementation follow-up (2026-09-12)

The findings below retain their API 0.7 audit baseline. API 0.8 added direct Lua
rule hosts; 0.9 added combat snapshots. API 0.10 now covers the G01 subset of
existing fight rule append/replacement and location/music replacement. Core
encounter identity, opponents, rewards and progress are preserved. G01 is still
open for other content domains and fight opponent/reward editing; G10's scenery
and music-selection semantics remain open. This is not DE parity certification.
See [the cumulative pre-DE work log](PRE_DE_WORK_LOG.md) for changes and evidence.

## Verified gaps and boundaries

### G01 â€” Targeted modification and removal of core content

**Missing public operations, high priority.** `MoonSharpScriptRuntime.PatchFight` accepts only `target`, `description`, `rounds`, `round_time`. There is no general supported patch/remove path for existing quest graphs, equipment/perk records, moves, battle metadata, fight opponents/rules/rewards, or forge candidate collections. Existing localization patches, shop availability, perk-branch overlays and asset redirects are real exceptions, not a generic patch API.

DE changes 706 `RoundTime` attributes (covered in principle), but also 693 `Power` and 109 `Music` attributes in `stages.xml`, removes/replaces quests, changes original perk triggers, and changes existing item enchantment templates. Adding a namespaced duplicate does not replace references to the original. Required follow-up: typed non-economic patches, explicit suppression/removal and conflict/restore behavior. Keep global economy fields excluded.

Evidence: [Lua binding](../Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntime.cs), `PatchFight`, `RegisterFight`, `RegisterQuest`; [DE stages](../Assets/DExml/stages.xml), [DE quests](../Assets/DExml/quests.xml), complete ledger.

### G02 â€” Programmable story events, queries and operations

**Programmable support exists; breadth and acceptance remain.** Lua story subscriptions and profile queries now expose level, item ownership/count/equipment/subtype/upgrade and equipped-item snapshots. The legacy quest operand set is not the full programmable API. Fresh TestStoryApi checks pass for 56 subscription cases. Historical purchase counts, tutorial-state coverage, full archive operations and live lifecycle ordering remain separate gaps.

Examples in changed/new main-file quests include `ChangeTab`, `ShopButtonPress`, `BuySpinGems`, `AscensionReset`, `Activate`, `AttachQuestFile`, `ClearQuestQueue`, `Wait`, `ChangeScene`, `OpenShop`, `GivePerk`, `GiveAchievement`, `ShowMapButton`, `ChangeDojoLocation`, tutorial hints/arrows and forge guidance. Similar event names are not automatically equivalent: e.g. the public Session hook does not prove both native application-start and session-start ordering.

Do **not** solve `If`, `Foreach`, arithmetic and dynamic strings by extending a generic Lua operation DSL. Expose lifecycle/event callbacks, safe queries and typed domain operations, then write the control flow in Lua. Keep legacy adapters supported. Economy actions such as `GiveCurrency`/`TakeCurrency` are not automatically candidates for exposure.

Evidence: [ModQuest enums](../Assets/Scripts/Eclipse/Runtime/Modding/ModContentP1B.cs), `ReadQuestEvents`, `ReadQuestOperand`, `ReadQuestActions` in the Lua binding; [QuestAction factory](../Assets/Scripts/Assembly-CSharp/QuestAction.cs), [compatibility adapter](../Assets/Scripts/Eclipse/Content/QuestCompatibility.cs). Core action existence does not mean Lua access exists. `BeforeQueue`, `CheckUserUpdate`, `ReplayButtonPress` and several compatibility actions remain deferred; determine relevance through actual DE flows rather than assuming all are required.

### G03 â€” Dojo selection and custom menu/dialogue flows

**Selection APIs implemented; selector-flow acceptance remains.** Public locations.select_dojo, reset_dojo and selected_dojo exist, together with owned UI and scene navigation. Native dojo entry honors selection without changing battle routing. Archived page/load/unlock flows still need explicit behavior and persistence acceptance.

QuestActionChangeDojoLocation sets the native selection, and Location.ResolveEntryLocation honors it at entry. Fresh TestDojoLocationRouting checks pass for 29 cases; selection services are controlled, without rendering or persistence proof. Rich tutorial presentation and the complete archived menu flow remain distinct from the supported selection API.

Evidence: [main DE quests](../Assets/DExml/quests.xml), `DojoChanger_LoadSelectedDojo`; [QuestAction.cs](../Assets/Scripts/Assembly-CSharp/QuestAction.cs), `QuestActionChangeDojoLocation`; `ReadQuestActions`.

### G04 â€” Cosmetic lotteries and purchased set chests

**Claim/UI/quest workflow implemented; purchased flows remain incomplete.** QuestActionDialogLottery now creates and shows ModQuestLotteryAction instead of completing as a stub. Evaluated claims, continuation and recovery metadata exist; 80 fresh production claim/recovery checks pass with controlled selection/grant/disk services. Paid SpinNumber continuation is explicitly rejected. This does not complete archived cosmetic purchases, set chests or crash acceptance in game.

`MonkSetChest_Give` grants level-dependent equipped items and attaches multiple specified perks/aspects to them. Public reward entries contain only item + fixed upgrade (and weight for choices); the public quest surface has no equivalent contextual grant-and-enchant operation. A weighted fight reward can cover some loot selection, but not the purchase flow, lottery presentation or exact chest loadouts.

Evidence: [lottery action](../Assets/Scripts/Assembly-CSharp/QuestActionDialogLottery.cs), [DE list](../Assets/DExml/list.xml), [DE quests](../Assets/DExml/quests.xml); `ReadRewardItems`, `ReadRewardChoices`, `ReadQuestActions`. Recover the intended lottery semantics before implementing them. Borrow base-owned economic policy for prices/costs.

### G05 â€” Set-to-ability binding and activated abilities

**Public and host-schema gaps.** Added set IDs are `ASSISTANTS`, `EARTHQUAKE`, `FEAR_RAY`, `GRASP_OF_DARKNESS`, `HERMITSTORM`, `HEX_SHIELD`, `LIGHTNING_CHAIN`, `POWER_FIELD`, `RAT_WAVE`, `SHADOW_CLOAK`, `TELEPORTATION`, `WALL_JUMP`, `WAR_WHIRL`. These are more than another list of five equipment pieces: examples contain `DefaultComboPerk` and a `SpecialRecipe` member.

`ItemSetDefinition` provides title/text/brief/members only. Recovered `ItemSet` also does **not read `DefaultComboPerk`**. A behavior with `kind="combo"` is not proof of the archive's set binding, activation button, cooldown, icon and animation contract. Existing native template-backed set perks may work for shipped mechanics; that does not solve arbitrary DE set/ability definitions.

Evidence: [DE item sets](../Assets/DExml/list.xml), `SHADOW_CLOAK`, `WALL_JUMP`; [ItemSet](../Assets/Scripts/Assembly-CSharp/ItemSet.cs), [public set model](../Assets/Scripts/Eclipse/Runtime/Modding/ModContentP1C.cs); `PERK_SHADOW_CLOAK` in [DE perks](../Assets/DExml/perks.xml). Recover authoritative activation semantics before adding a nominal field.

### G06 â€” Combat events, hit modification and effects

**Missing public behavior capabilities.** Added perks are `MindThrowNormal`, `PERK_ITEM_SPECIAL_BLOODRAGE_WEAPON_STRANGER`, `PERK_MASTER_OF_STYLE`, `PERK_RELENTLESS`, `PERK_REVIVAL`, `PERK_SHADOW_CLOAK`; 68 existing perk definitions also change.

The accepted API supplies fight/round lifecycle, damage resolution/observations, block and critical events, life/magic changes, bounded damage reduction and shields. DE's Master of Style needs a `Style` event/query and an offensive hit change. Relentless needs combo events/counts and timed stacks. Shadow Cloak uses animation timing, cooldowns, model visibility, flags, interval changes and buff icons. The archive also uses pre/post-crit boundaries, `MagicCharged`, `ModExpires`, area enter/exit, impulses, bullets, animation changes and model effects.

`scale_incoming_damage` is bounded to 0..1: it cannot stand in for added offensive damage. Health changes cannot revive a dead fighter. A template-backed perk only inherits an existing native trigger graph; parameters cannot create missing triggers. Match each required mechanic to a precisely timed hook and typed effect, not just similarly named events. Do not promise every generic event in the archive needs a public callback; native template reuse may cover unchanged behavior after verification.

Evidence: [DE perks](../Assets/DExml/perks.xml), [P2 public contract](P2_API.md), [Lua combat implementation](../Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeP2.cs). Gameplay fixtures required for each converted mechanic.

### G07 â€” Perk upgrade parameter tables

**Implemented API; gameplay acceptance remains.** Perk registration now accepts typed `upgrades` with per-level descriptions and parameters. `CharacterProgress.xml` supplies archived examples with `Drain`, `DamagePerStack` and `MaxStacks`; those are reference data, not a DE conversion. Current Lua and native upgrade fixtures verify overlays, cloning, descriptions and restoration.

Branch placement and upgrade payloads are separate supported operations. Fresh checks on 2026-09-12 passed 39 Lua/saved-parameter cases and 49 native-source cases. Full-game level-up UI, save and effect acceptance is still required; the branch showcase alone does not prove it. Shared XP/currency/global progression formulas remain separate.

Evidence: [DE CharacterProgress](../Assets/DExml/CharacterProgress.xml); `RegisterPerk`, `ReplaceProgressionBranch` in [Lua binding](../Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntime.cs), [P1C contract](P1C_API.md).

### G08 â€” Moves, projectiles, shop demonstrations and input

**Large public authoring gap, high priority.** `animations/moves.xml` contains 93 added and 938 changed move records, not just the showcase step. Added families include WallRunUp, ShadowCloakPlayer, MindThrow*Normal, Sphere1/2/3, ComboSphere3, ChineseSwordsSuperSlash and many shop magic/weapon previews. Six templates are removed and three added.

The public move action vocabulary is **Sound and HitEffect**; conditions are CurrentAnimation/CurrentInterval/Item/Perk/All/Any. DE moves use input (`KeyPressed`, `Keys`), screen locks, distances, bullets, round stages, spawn/delete players/projectiles, effects, alignment, velocity, rotations, transitions, profiles and more. Templates can inherit compatible existing behavior, but cannot faithfully add or change arbitrary missing children. P3 asset replacement explicitly excludes opaque animation replacement. Providing `.bytes` assets does not fill the definition/runtime capability gap.

Evidence: [DE moves](../Assets/DExml/animations/moves.xml); [public move model](../Assets/Scripts/Eclipse/Runtime/Modding/ModContentP1D.cs), `ReadMoveActions`, `ReadMoveCondition`, `ValidateMoveNodeFields` in [Lua binding](../Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeP1D.cs). Preserve declarative animation data while using safe runtime operations for custom procedures.

### G09 â€” Conditional AI reactions

**Programmable support implemented; scenario acceptance remains.** Lua AI decisions now receive perception, animation/interval snapshots and eligible move candidates with timing/input metadata, and can issue bounded actions with native fallback. The reactive example demonstrates responding to an opponent's active attack. This supplies programmable decisions; it does not prove every archived `HermitStormPlayer`, `HermitStormPlayerIdle` or `WallRunUp` scenario works in a live fight.

`ComputerSettings.xml` is equal to the base: no new global computer-settings API is justified by this archive. Add evidence-backed reaction/decision hooks or typed compatibility support only for the required semantics.

Evidence: [DE tacticSettings](../Assets/DExml/tacticSettings.xml), `RegisterTactic` / `ReadTactic...` in [Lua binding](../Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeP1D.cs), `Tools/TestModAi.ps1`, `Mods/example.programmable-ai`. Native conditional-tree import and programmable equivalent behavior are distinct contracts. Live scenario coverage remains open.

### G10 â€” Animated location layers and music selection

**Partial support with motion implemented.** Public location layers now support typed `motion_x`, `motion_y`, `rotation` and `opacity` tracks in addition to static images and placement. The API covers layer oscillation but not arbitrary particle control, collision or hazards. Archived `arena_new` and other scenery still require per-effect coverage and visual acceptance.

Location `music_choices` and native list selection are implemented alongside single-track music. Fresh checks on 2026-09-12 passed six native selection cases and 36 Lua motion/music/projection/fingerprint cases; these do not prove audio playback. Targeted location edits, broader effects and in-game acceptance remain separate. Supplying missing music files remains an asset task, not automatically an API gap.

Evidence: [arena_new](../Assets/DExml/locations/arena_new/arena_new_params.xml), [Location parser](../Assets/Scripts/Assembly-CSharp/Location.cs), `ReadLocationLayers` / `ReadLocationImages` in the Lua binding. Full 108-path location comparison is in the file inventory.

### G11 â€” Equipment metadata and default enchantment loadouts

**Partial support, mixed policy.** Added weapon/armor/helm/ranged/magic assets and non-equipment items have public registration. Shop visibility and mod-owned listing levels/prices also exist. New equipment registration itself is deliberately small: e.g. weapon fields are ID/display/icon/model/subtype; armor is ID/display/icon/model. The adapter derives combat values from the host's level templates.

DE item deltas include `TacticSubtype`, core item template changes, innate `<Perks>`/aspect changes, `SingleTimeBuy`, acquisition flags, package behavior and presentation metadata. There is no general field-complete registration/patch surface or contextual grant API for these. Do not count a visually matching weapon as the same item behavior.

API 0.49 now supports replacing acquisition-time default enchantments for core or owned equipment through `items.set_default_enchantments`, with typed perk handles and optional numeric aspects. Preview/acquisition projection, ownership conflicts, compatibility fingerprints and rollback are tested. Existing inventory is not migrated; full-game acquisition/save/render acceptance remains pending. This does not supply permanent innate `<Perks>`, tactic metadata or arbitrary grant operations.

API 0.50 adds `items.set_innate_perks` for the separate permanent equipment-perk list, with literal numeric native parameters and Lua-backed perk handles (their initial parameters belong at perk registration). Native model collection, cloned-instance isolation, composition with acquisition defaults, rollback and public Lua validation are tested. Live combat acceptance, activated ability mechanics, tactic metadata and general acquisition operations remain open.

Important boundary: prices, bonus prices, upgrade/stat scaling and level changes must first be checked against the canonical economy policy. This audit does **not** request arbitrary core stat/economy overrides. Non-economic subtype, identity, availability, display and supported enchantment loadout changes are the separate legitimate API work.

Evidence: [DE list](../Assets/DExml/list.xml); `RegisterWeapon`, `RegisterArmor`, `RegisterNonEquipmentItem`, `SetItemAvailability` in [Lua binding](../Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntime.cs); [LegacyContentAdapter](../Assets/Scripts/Eclipse/Modding/LegacyContentAdapter.cs).

### G12 â€” Forge family editing and candidate structure

**Implemented operations, gameplay acceptance pending.** Added families are `Abilities`, `Abilities2`, `Abilities3`, `Complex2`, `Complex3`. Registering a family using a host economic profile and candidate level ranges works. API 0.47 adds targeted native candidate exclusions for existing recipes. API 0.48 adds per-equipment deviation overrides for existing random-aspect recipes, covering both effective item settings and embedded native candidate ranges. These provide the operations needed for the cited Complex candidate removals and Simple deviation changes; the downstream DE records have not been ported or accepted in game. Arbitrary candidate conditions, full native family replacement and broader variation/availability editing remain outside these operations.

Exact prices/costs must remain base-owned. Check deviations/quality scaling against that boundary before deciding they should be configurable. New family registration is not evidence that the entire original forge is converted.

Evidence: [DE forge](../Assets/DExml/forge.xml), `RegisterForgeRecipeFamily`, `ReadForgeRecipeItems` in the Lua binding; [P1C](P1C_API.md). Forge timing policy itself is already supported.

### G13 â€” Achievement predicates and core localization

**Partial support.** The five added achievement counters are `DefeatButcherWithKatana`, `EnchantmentsQuest_1/2/3`, `SenseiStoryFinished`. P3 owned counters/threshold achievements work. Exact equipment/boss/story/forge predicates require the query/event support in G02/G06, and core-counter changes need G01. Of the 89 changed existing counters, platform GameCenter/GooglePlay IDs account for most attribute edits; replacing those IDs is not a needed offline gameplay API.

DE adds Croatian/Hindi/Hungarian/Romanian/Swedish translation files, changes shared strings, and omits three Asian translation files. New locales and owned strings are supported; do not assume every core Word is projected into the patchable catalog. Core string replacement must be verified for every intended key, including description-format placeholders. Fonts/glyphs and loader assets need validation; file absence alone does not justify removing a supported language.

Evidence: [DE achievements](../Assets/DExml/Achievements.xml), [DE localization metadata](../Assets/DExml/localization.xml), [P3 bindings](../Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeP3.cs), [CoreContentImporter](../Assets/Scripts/Eclipse/Runtime/Modding/CoreContentImporter.cs), current localization patch limitations in README.

### G14 â€” Core service suppression, boot/default profile and presentation

**Mixed supported policy, platform ownership and unresolved intent.** P2 gates named service groups and battle-pass extension paths. That does not prove every removed DE quest is suppressed: the archive empties utility, energy, raid, forge, offer and event extension files and radically changes the root include graph. Each intended suppression needs a semantic gate or G01 removal; avoid globally disabling unrelated story content.

Only forge is an exposed timer target; shop delivery is already instant in this base. Do not invent missing timer settings for an already-satisfied behavior. Default profile edits include intro suppression, dojo/raid UI defaults, save-slot and audio state; some are platform/initialization choices and some may need safe mod initialization. Do not replace `usersDefault` wholesale or ship archived Android filesystem paths.

Internal hit-pause/slow-motion/camera/resistance differences, device heuristics, logger configuration, credits translations and absent CDN/packs files are not proof of intentional mod policy. The P3 configuration ledger classifies these domains; recheck intent with the creator where behavior matters. Boot-time credits/branding are not covered by gameplay asset redirects. SDK removal is build work. Compatibility overlays are recorded separately and must not be mistaken for canonical desired DE content.

Evidence: [default profile](../Assets/DExml/usersDefault.xml), [internal settings](../Assets/DExml/internalSettings.xml), [P2 gates](P2_API.md), [P3 configuration ledger](PHASE3_CONFIGURATION_AUDIT.json).

## What remains supported

This audit does not invalidate accepted functionality: namespaced owned content, normal fight/warrior/rule registration, narrow fight patches, fixed/weighted item rewards, repeatable offline raid encounters, mode progression, dialogue, shop availability, forge families/timing, basic template-backed or Lua-backed effects, owned counters/state/migrations, static location layers, locale registration and typed asset redirects remain useful foundations.

For raids, use the owner's confirmed one-fight, long timer, blue multiple-health-bar, infinitely replayable, gem-reward semantics. Do not reintroduce obsolete server progression merely because archived raid XML contains it. The other 73 DE raid battle records still require boss-specific moves/perks/art and verification; registering their names alone is not parity. Ascension sequence/Monk acquisition is proven, while its exact archived lottery/dialogue/entry flow is not.

## Recommended next work before a complete wiki

1. Correct completion wording: phase showcase acceptance is not full domain parity. Mark partial/missing contracts explicitly in the wiki.
2. Close G01 plus a programmable story/query/operation slice from G02. Use a real archived DE flow as the fixture.
3. Prove dojo selection and an enchanted set chest (G03/G04), then an activated set ability and level-scaled perk (G05â€“G07).
4. Implement one demanding move/projectile family and its AI reaction, plus animated arena scenery (G08â€“G10).
5. Classify remaining metadata/forge/economy differences and map all selected DE records to actual mod code + tests. Obtain creator confirmation for ambiguous/archive-version differences instead of silently labelling them intentional.

No runtime API, economy, XML source, asset identity or mod behavior was changed by this audit. Validation here consists of complete XML parsing/differencing, deterministic regeneration/checks and source inspection. No DE conversion or gameplay playthrough was performed. This is a complete **source inventory** and a source-backed **gap assessment**, not a claim that every archived mechanic has been recovered or individually playtested.

G11/G09 native prerequisite update: shipped TacticSubtype was ignored by ItemInfo; it is now parsed independently from SubType, preserved by cloning/merge and consumed by Model's AI initialization and weapon changes. SetWeaponBot now updates the own-weapon table group used by native decision tables instead of the enemy field. Native metadata/AI checks and existing AI suite pass. A public typed metadata override and live combat acceptance remain open; this is not a DE port.

API 0.51 exposes optional tactic_subtype on owned weapon registration, independent of animation subtype. Literal native group names are validated, projected by the actual native item builder and included in compatibility fingerprints. Lua omission/invalid/rollback/hash checks and compiled projection checks pass. Core equipment metadata patching, other G11 metadata/acquisition gaps and live combat acceptance remain open.

API 0.52 adds items.set_tactic_subtype for core and owned weapons. Required group supports explicit empty fallback; weapon/dependency checks, conflict ledger, compatibility fingerprint, native scope restoration and adapter partial-failure rollback are implemented. 51 native/content/adapter tests and 123 Lua forge/loadout tests pass. Other equipment metadata/acquisition and full-game acceptance remain open.
