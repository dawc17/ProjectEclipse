# DE128 production record

Current package: **0.18.0**. Latest completed content is recorded in Step 31;
Steps 32–34 add encounter perk settings, reward economy, saved fight queries and
pending Sensei Story data/decisions.
The following overview describes earlier milestones. Step 12 activates both ChineseSwords moves, ten lock
extensions and Jian's subtype delta through Lua. Steps 9–11 supplied the generic
move APIs and verified graph data; Step 13 adds passing isolated live input/animation
acceptance. Step 8 adds four evidence-backed combat subtype changes
and a reversible native patch API. Step 7 adds archived shop availability for the first
five battle-pass collections through Lua and a generic level-gate API. Step 6 enables normal settlement of saved pending forge
orders through a generic opt-in timer policy. The owner rejected the Ascension prototype and directed
production back to actual DE XML content. Its modules are commented out. Step 5
implements the archived Master of Style and Relentless perks, exact rank/branch
placement, and associated move-lock removals. Earlier policies and the configured
Desolator reward remain. Step 4 below is historical, disabled work.

## Step 1: package and policies

Authorized 2026-09-18. Scope: P4.1 module foundation, a bounded P4.2 intent ledger
and the policy portion of P4.3. Uses existing P2B public capabilities. The package
identity is `de128`, version `0.1.0`. Further production steps require the owner's
approval after this step's report.

No assets are needed for this slice. No archived XML is shipped or read by the
mod, and no base engine or canonical data changes are part of it.

## Intent and implementation ledger

This is an incremental ledger, not a classification of the whole archive.

| Intent | Classification | Evidence | Implementation / acceptance |
| --- | --- | --- | --- |
| Remove forge waiting for new orders | `intentional_de` | `Mods/DE_PARITY_TARGET.md:24`; `RecipeItemInfo.cs:27-43` reads the timer policy when creating an order. | `scripts/content/timers.lua`, `sf2.timers.set`, `policy.timers`. Managed policy checks plus the README's new-order game check. |
| Suppress monetization/service content | `intentional_de` | `Mods/DE_PARITY_TARGET.md:25-26,39-43`; `QuestStage.Compare` at `QuestStage.cs:345-355` checks six group names and battle-pass source paths. | `scripts/content/services.lua`, `sf2.services.disable`, `policy.services`. Managed flags/composition checks; full UI coverage remains open. |
| Preserve shared prices, costs and economic rules | `canonical_eclipse_economy` | `Mods/DE_PARITY_TARGET.md:66-69,90-93`. | No economic definitions, patches or mutation capabilities. Pending-order skip prices remain the base prices. |
| Remove compiled payment/ad SDKs | `build_only` | `Mods/DE_PARITY_TARGET.md:39-43`. | Separate Eclipse build/dependency work; a Lua policy cannot remove binaries. |

Source paths above for recovered classes are under
`Assets/Scripts/Assembly-CSharp/`. Policy validation and consumption are in
`Assets/Scripts/Eclipse/Runtime/Modding/ModContentP2.cs:69-196`; Lua bindings are
in `Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeP2.cs:141-157`.

## Confirmed limits and next investigations

| ID | Finding | Consequence and next evidence needed |
| --- | --- | --- |
| DE128-01 | `ModTimerPolicy` accepts only `forge`. The initial implementation restored saved deadlines directly and shared the early-skip flag with delivery settlement. | Pending forge orders are addressed by Step 6 below, including lifecycle and save regression checks. Purchase/upgrade timer support remains open. Saved deadlines are not rewritten by the mod. |
| DE128-02 | The audited `ModPolicies.FeatureEnabled` consumer is `QuestStage.Compare`, filtering recognized groups/source paths. | Complete ad/offer/battle-pass UI and direct service suppression are unproven. Trace concrete surviving surfaces before adding generic host hooks; do not claim the six flags remove every surface. |
| DE128-03 | `RewardItemGrant` at `ModContent.cs:1364-1373` carries only item and upgrade. Archived `Assets/DExml/stages.xml:36420-36425` grants Titan's sword with a reward-specific Lifesteal aspect. | The public item-reward contract cannot reproduce that grant-specific enchantment. Existing `sf2.items.set_default_enchantments` accepts an optional fixed integer aspect on an item definition (`MoonSharpScriptRuntime.cs:1058-1083`), not a per-grant calculation using the player's level. A typed grant/enchantment operation is needed for exact parity; no raw XML or expression-string passthrough. |
| DE128-04 | Canonical `Assets/vanillaXml/list.xml:1674` defines a sparse hidden Titan sword. Archived `Assets/DExml/list.xml:22-28` adds its icon, upgrades and innate `PERK_TITAN` / `PERK_ANTI_SHOCK`. | Reconcile metadata, upgrade policy and innate behavior through actual public APIs before claiming the DE item is complete. Shared upgrade economy remains immutable. |

The existing policy API is sufficient for Step 1. No new API is implemented in
this step. An absent API, missing art, unclassified archive difference and pending
gameplay acceptance are tracked separately.

## Candidate for the next approved content step

Titan's Desolator has direct evidence in the acceptance target and archived final
Eclipse Titan fight. The projected references are:

- Item: `core:items/weapon/WEAPON_TITAN_GIANT_SWORD`.
- Fight: `core:fights/zone_7/c3_boss_titan_eclipsemode/6`.

Projection follows `CoreContentImporter.cs:41-44,71-84,186-196`. Existing
`Mods/example.eclipse-reward/scripts/main.lua` demonstrates a scoped core Eclipse
item-reward patch while preserving native currency rewards. The base definition
references `mdl_weapon_giant_sword`, and the repository has
`Assets/Resources/ui/items/Weapon17.img_weapon_boss_giant_sword.png`.

That supports investigating a basic grant without recovering new art. Packaged
asset resolution, player weapon behavior, reward display and save/reload still
need verification. Exact archived grant parity is blocked by DE128-03, and the
item needs the DE128-04 review. No Titan patch is installed by this version.

## Validation boundary

`Tools/TestDE128Foundation.ps1` compiles current production runtime/binding source
and executes the real package in an isolated fixture. It checks manifest and
dependency resolution, required module loading, public Lua registration, policy
queries used by native consumers, conflicts in both orders, additive service
disables, failed-registration rollback, capability enforcement, and rebuilding
without DE128. It loads no archive or player save and supplies no art provider.

Managed policy checks are not Unity rendering, actual forge settlement, service
UI acceptance or a full campaign playtest. Use the README's game checks for those
boundaries and record failures before extending the mod.

## Recorded verification: 2026-09-18

The first package passed **73 checks** in `Tools/TestDE128Foundation.ps1`.
Compilation of the current runtime and MoonSharp binding sources completed with
zero warnings and zero errors. The retained fixture is
`Temp/DE128Foundation-577e8683035e40af97bd6e90c333417e`.

The existing mod-editor indexer and contract analyzer accepted the manifest and
all three Lua files with no diagnostics; Lua 5.2 syntax parsing also passed.
`npm run build` in `Docs/Modding` passed its reference audit, Astro checks, build,
search indexing and validation of 3,972 local links/assets across 47 pages. Astro
reported a `/404` route conflict warning during the build; its type checks had
zero errors, warnings or hints. `git diff --check` passed for tracked changes.

An initial attempt to run `Tools/TestPhase1ShowcaseRuntime.ps1` was blocked by the
tool before execution. The dedicated DE128 fixture above ran successfully and
does not depend on that showcase runner. No Unity game playtest was performed.

## Step 2: equipment without a purchase listing

Authorized by the owner's continuation request on 2026-09-18. This step creates
`de128:items/weapon/titans_desolator` through `scripts/content/equipment.lua` and
stops before installing its acquisition path. It reuses the core model and icon,
the `TitanGiantSword` subtype, and the core Titan/anti-shock innate perks. Its
English translation is owned by DE128 and registered in Lua. No XML, art copies,
TOML localization files, initial inventory grants or profile migrations are added.

The item has a new mod-owned identity. The core NPC sword remains unchanged;
this avoids changing Titan's own equipment through a player-item definition.
The item uses the existing canonical weapon progression profile. Its initial
level is 1 until a supported grant operation supplies the final reward's level.
This is an equipment foundation, not archived damage/upgrade/grant parity.

### Diagnosed API/runtime requirements

| ID | Evidence and implementation | Remaining acceptance |
| --- | --- | --- |
| DE128-05 | `LegacyContentAdapter.ApplyItems` and its item-localization aliases formerly iterated only shop listings, although public rewards and warriors accept unlisted item definitions. They now materialize every mod-owned equipment definition. `BuildItemNode` uses normal category baselines (weapon 1, armor/helm 2, ranged/magic 6), existing upgrade templates, `ShopHide=1` and no purchase price when there is no listing. | Actual game acquisition, inventory presentation, upgrades and reload still need a playtest. Listed equipment retains its chosen starting level and price. |
| DE128-06 | Mod equipment requires a mod-owned display-name handle, but the public Lua API had no operation to create that translation. `sf2.localization.register { id, language, value }` now calls the existing transactional localization registration path and returns a handle. It shares ownership and duplicate-language rules with file-based localization. `sf2.localization.key` also now accepts qualified references with dependency/category checks, as its documentation already promised. | Editor and Lua contract checks are recorded below. No privileged DE branch or raw native expression is introduced. |
| DE128-03, refined | Archive `Assets/DExml/stages.xml:36413-36426` awards the sword in final Eclipse Titan fight 6, reward slot 1, with `Level='?Player[].Level'` and Lifesteal `Aspect='3639 / 100 * ?Player[].Level + 60'`. The public `RewardItemGrant` only carries item and upgrade number. | Add a typed grant contract for level and enchantment parameters, including a safe Lua calculation at the actual grant boundary. Do not substitute a fixed enchantment power or a shop listing. |

The future patch target remains
`core:fights/zone_7/c3_boss_titan_eclipsemode/6`, with `wins=1`, `mode="eclipse"`
and no level gate. `wins` selects the native reward slot. Canonical
`Assets/vanillaXml/stages.xml:29346-29355` supplies its existing currency rewards.
`GameUtils.cs:2126-2135` selects the win slot and `FightResult.cs:188-201` filters
already-owned equipment; a separate once-only save flag is not presently needed.
Native grant enchantment payloads exist (`ListSF.cs:1982-1997`,
`UserItem.cs:636-705`), but the public Lua reward builder cannot author them yet.

### Asset and verification boundary

`Assets/Resources/SF2Content/Art/catalog.json` lists the giant-sword model and
Weapon17 atlas; the recovered icon also exists under `Assets/Resources/ui/items/`.
These are evidence for reusing core art, not proof of sprite decoding, player rig
compatibility or rendered attacks. Desolator does not become obtainable in this
step, so enabling the mod still presents only the existing policy changes.

The next approved step should implement and exercise the generic grant-time
level/enchantment contract, then connect Desolator to the audited Eclipse reward.

### Recorded Step 2 verification: 2026-09-18

- `Tools/TestDE128Foundation.ps1`: **224 checks passed**, with zero compile
  warnings/errors. Runs current runtime/binding sources and the actual DE128
  package against canonical weapon/perk projections. Covers localization
  ownership, languages/fallback, malformed and duplicate registrations, qualified
  references, innate registration, capability enforcement, missing modules/art,
  composition, rollback and re-enabling. Fixture:
  `Temp/DE128Foundation-6a34f49ba5cf45adb05b89b681cc9aaf`.
- `Tools/TestRewardOnlyEquipment.ps1`: **63 checks passed** against extracted
  current production adapter methods. Covers all five unlisted equipment
  categories, coin/gem listing preservation, localization aliases/language changes,
  removal, preflight collisions, later-add failure rollback and retry. Item
  storage, stat resolution and localization services are controlled substitutes;
  this does not execute native `Items`/`ItemInfo` or prove native stat values.
  Fixture: `Temp/RewardOnlyEquipment-92e3598a1bdc49be892ed23f4f7d4538`.
- Mod editor generation/checks passed, **32/32 unit tests passed**, and actual
  DE128 indexing resolves `item.titans_desolator` from its required equipment
  module. All four package Lua files had zero editor diagnostics. The indexer
  follows literal reachable local `require` calls; it does not execute dynamic
  Lua. LuaLS checks also passed before the final module-indexing adjustment.
- Wiki build passed: **139** public reference entries, **47** pages and **3,978**
  local links/assets checked. Astro reported zero errors/warnings/hints in its
  source checks; the existing `/404` route conflict warning remains during build.
  The VS Code extension build passed, but the integration runner stopped because
  it needs an explicit absolute `Code.exe` argument. It was not retried.

A complete managed game build remains blocked by the generated project files'
environment references: Visual Studio MSBuild could not resolve its SDK, and
`dotnet build Assembly-CSharp.csproj` reported nine missing Unity 6000.6/Visual
Studio/pipeline analyzer files, including paths belonging to another machine.
No project files or environment settings were changed to work around them. The
isolated source fixtures above passed independently of those generated projects.
No Unity playtest, actual art decoding or player-save mutation was performed.

## Step 3: configured Eclipse Titan reward

Authorized on 2026-09-18 after the owner confirmed the manually added Desolator
worked. Package `0.3.0` adds `scripts/content/rewards.lua`. The mod still ships
no XML definitions and reads no archived XML at runtime. This step does not edit
the owner's save or change the manually granted item.

### Content and provenance

The final Eclipse Titan fight is
`core:fights/zone_7/c3_boss_titan_eclipsemode/6`. The Lua module patches only its
winning reward slot (`wins = 1`, `mode = "eclipse"`, no level gate), using the
existing scoped item-drop projection. Base money and forge-material rewards stay
in place. Ordinary ownership filtering suppresses another Desolator copy.

Archive `Assets/DExml/stages.xml:36420-36425` supplies the player-level sword and
Lifesteal formula. The registered reward's callback returns `level =
context.player_level` and a core Lifesteal perk with aspect
`3639 / 100 * context.player_level + 60`. The mod passes a number, never a native
expression string. For example, the Lua result at level 52 is `1952.28` before
the recovered game's numeric conversion. Weapon stats and upgrades still use
the canonical progression profile introduced in Step 2.

### Generic API added for this step

Guaranteed reward items and weighted candidates accept optional
`configure = function(context) ... end`. The context contains `player_level` and
the qualified `item_id`. The result is a typed table containing optional `level`
(integer 1..10000) and `enchantments` (dense array, at most 64 distinct perk
handles, each with an optional finite numeric `aspect` from 0 to 2147483647).
Configuration is restricted to equipment. Existing upgrade ordinals still apply
after level selection. An omitted/empty enchantment array leaves acquisition
defaults alone; a nonempty grant follows native enchantment-kind replacement.

The callback runs during result composition after owned/missing-item filtering.
The player-level snapshot precedes reward XP. It may also run for a preview, so
Lua must return the same result for the same context and must not mutate captured
state. Host capability calls and mutable game/profile operations are blocked in
this scope. Execution is bounded to 200,000 instructions, and disposing the Lua
context invalidates retained callbacks. Invalid results, exceptions or exhausted
budgets skip the affected item with a diagnostic, without an unconfigured fallback.

`RewardItemGrant` carries the callback within the registration transaction.
`RewardDefinition.TryGetGrant` supplies the adapter's flat direct/choice index.
The native bridge resolves the committed grant and makes a fresh configured
`RewardItem`; it does not cache a player's configuration on a shared definition.
The content fingerprint records the presence of configuration callbacks. Normal
inventory settlement and the existing enchantment serializer persist the result.

This closes DE128-03's missing typed level/enchantment calculation for reward
items. It does not add a generic imperative inventory-grant function, arbitrary
enchantment parameter overrides, or a migration for previously owned equipment.
The pre-XP snapshot is the public API's timing contract. The archived native
enchantment expression is evaluated later during settlement, so a reward that
also levels up the player can have different timing; this step does not claim
universal archived expression parity.

### Step 3 verification

- `Tools/TestDE128Foundation.ps1`: **337 checks passed**, zero compiler warnings
  or errors. The actual package executes against current runtime/MoonSharp
  sources and canonical item, perk and stage projections. Tests cover the exact
  Titan target/slot/mode, player levels 1/2/7/51/52, decimal Lifesteal results,
  repeat calculations, weighted candidates, ownership-handle validation, malformed
  arrays/numbers/returns, instruction limits, blocked host calls, recovery after
  failed callbacks, disposal, rejected non-equipment grants and fingerprints.
  Existing policy/registration rollback checks also pass. Fixture:
  `Temp/DE128Foundation-cfc30e1ea54c46589cc2a1a65e683d00`.
- Editor generation/check passed; **33/33 unit tests**, LuaLS callback/context
  completion and VS Code integration passed. All **five** actual DE128 Lua files
  have zero analyzer diagnostics. Generated contracts contain 139 functions,
  76 constants and 185 typed structures. Deep unannotated return-table completion
  is a LuaLS limitation, not runtime validation.
- Wiki build passed: **139** reference entries, **47** pages, **3,981** checked
  links/assets and zero Astro source errors/warnings/hints. The existing `/404`
  route conflict warning remains.
- `Tools/TestRewardGrantNative.ps1`: **33 assertions passed**, zero errors and
  eight existing `PerkStruct.string.Copy` warnings. Compiles actual `RewardItem`
  and `PerkStruct`, plus extracted current adapter reward builders, the runtime
  bridge, native result selector, `UserItem` enchantment serializer and reward
  projection. Catalog/configuration, inventory/stat and expression services are
  controlled substitutes; the separate 337-check fixture covers real runtime
  types and Lua execution. Fixture: `Temp/RewardGrantNative`.
- The native fixture loads the actual canonical
  `ZONE_7/C3_BOSS_TITAN_ECLIPSEMODE/6` fight. It verifies Eclipse-only insertion,
  unchanged slot 0 and fight/reward attributes, and preservation of the real
  forge-material rewards `27216` and `23862`. This target has no XP attribute.
  An earlier test used the wrong Titan data; it was replaced before the final run.
- `Tools/TestModConsumableRewards.ps1` passed after updating its extracted-method
  contract. `TestRewardExpressions.ps1` remains blocked by the preexisting missing
  `Temp/bin/Debug/Assembly-CSharp-firstpass.dll`; `TestRewardNative.ps1` was refused
  by the tool before execution because it invokes the broader showcase launcher.
  Their directly affected fixture contracts were updated. No project/environment
  changes or full-game builds were attempted to work around these blocks.

These source/editor fixtures do not decode sprites/models or demonstrate an
actual reward screen, Lifesteal activation, campaign settlement or game reload.
The owner's earlier manual item check is separate evidence. The existing manually
granted sword is unchanged, so it does not receive the new acquisition enchantment
and ownership filtering prevents another copy.

### Next approval boundary

Stop after reporting this step. A useful next content slice is the accompanying
Titan armor/helmet rewards shown beside the sword in archived `stages.xml`, first
checking their model/icon references and player compatibility. Pending forge
orders and service UI suppression remain separately tracked gaps.

## Step 4: Ascension combat and persisted trial selection (disabled prototype)

Authorized on 2026-09-18 when the owner requested larger-impact production steps.
This step selects a complete five-fight combat/progression loop rather than the
previous candidate equipment additions. Package `0.4.0` adds two ordinary Lua
modules: `content/ascension.lua` and `content/ascension_rules.lua`. The user save
was not edited. Earlier DE128 policies and the Desolator reward remain active.

### Archive evidence and implemented slice

The authority for this slice is `Assets/DExml/stages.xml`, XPath
`/Stages/Zones/Zone[@Name='ZONE_2']/Battle[@Name='Ascension']` (lines 8714-9134).
That battle is absent at the same XPath in canonical `Assets/vanillaXml/stages.xml`.
It defines five one-round/150-second trials and six randomly selected challenge
families: no weapons, hot ground, ring out, no blocking, enemy regeneration and
no jumping. The Lua definitions preserve their recovered node names, thresholds,
frame counts, target sides and damage-factor adjustments. Native `AttributesRule`
adds those adjustments, so the baseline and challenge factors remain separate.

The canonical stage archive retains all five `Ascension_2` opponent templates
(lines 32836-32840), plus the `Survival_Ranged_2` shuriken/kunai choices (33412-33423).
The mod registers ten warrior variants using those templates, the archived
Standard tactic, resistance and attribute alignments. It shuffles the five
opponents and six challenges, using five distinct challenges per run. Sixteen
archived opponent perks are registered as rules with aspect 100000. Random choices
and their stream are owned saved state; there is no native expression interpreter
or archived XML read in the mod.

The statue parameters, map icon/preview and opponent portraits remain in core
resources. `Sound.cs:81` maps `halls_of_the_dead_heroes` to the existing
`fight34_halls_of_the_dead_heroes.ogg`. The Lua battle and fights use that mapping.
This is source/file evidence, not proof of native decoding or rendered combat.

The preview has an additional map zone labelled **Ascension Trials**, using
`Map1.2`, with a map-session reveal quest. Win and loss dialogs use the Puppeteer
and are filtered to this mode. Five wins finish a run and increment the host's
completion counter; loss/surrender resets to the first trial. New runs shuffle
again. A prepared attempt retains its opponent/rules on reload or launch retry.

### Generic API work

- Five typed rule functions expose recovered classes: `sf2.rules.hot_ground`,
  `ring_out`, `regeneration`, `no_animation` and `remove_interval`. Existing
  `sf2.rules.perk` accepts an optional finite numeric aspect. The new immutable
  payloads and optional aspect are included in content fingerprints.
- `on_prepare` encounter plans accept optional `rules` (up to 100 distinct owned
  handles) and `description` (up to 1024 characters). Omission inherits blueprint
  values; an explicit empty rules list removes them. The adapter validates native
  projection before saving the plan. Selected behavior-backed rules dispatch at
  the combat boundary alongside selected native rules.
- Encounter save version 2 retains the selected rule IDs and description, while
  version 1 remains readable. Unknown versions, duplicate data and unavailable
  rule identities are rejected without rewriting the saved encounter. Active
  rule state is cleared on completion, cancellation and profile teardown.

Hot-ground rules accept one explicit side, defaulting to player. Native
`HotGroundRule` inherits a copy method that returns `AnimationListRule`, making
the native all-target expansion unsafe; the API rejects that combination.
Ascension uses the archived player-only rule. No-animation rules affect both
fighters because the recovered rule has no side selector.

### Explicit remaining gaps

This is an Ascension **combat preview**, not the complete archived service. Its
two result slots contain no configured loot, and there is no entry ticket cost.
Shared prices, currency formulas and ordinary engine settlement remain untouched.

| Gap | Evidence and required next work |
| --- | --- |
| Exact campaign unlock | Archived `quests.xml:3618-3648` unlocks after Zone 2 Tournament 12. Public core-fight lookup/dynamic battle visibility is insufficient for that rule; the preview uses a separate visible zone. |
| Ticketed admission | Archived `quests.xml:3436-3508` charges three core Ascension tickets for initial entry and allows continuation. Existing modes only accept a mod-owned consumable cost per fight. Add an explicit admission contract without exposing shared currency redefinition. |
| Mixed lottery and Monk acquisition | Archived trials use Bronze/Bronze/Silver/Silver/Gold lotteries mixing Monk pieces, money and materials. The public item-only weighted reward choice cannot preserve that distribution. No guaranteed Monk items or substitute economy were added. |
| Presentation and difficulty parity | The original launcher/tutorial sequence and rating-label overrides are not reproduced. The new descriptions and result dialogs make the combat slice testable. |

### Recorded Step 4 verification

- `Tools/TestDE128Foundation.ps1`: **396 checks passed**, zero compiler warnings
  or errors. Executes the actual seven-script package against current runtime and
  MoonSharp sources. Existing policies/reward checks remain, with Ascension
  registration, missing-module/capability rollback and changed rule-fingerprint
  sensitivity. Fixture: `Temp/DE128Foundation-ba4ae26e9cf24ca9be2966cde1429b13`.
- `Tools/TestTrialRules.ps1`: **43 checks passed**, zero compiler warnings/errors.
  Compiles actual runtime/Lua sources and recovered rule classes, using the actual
  extracted adapter rule builder. Checks payload parsing, target restrictions,
  node/animation limits, native copy behavior, hot-ground timer/reset and invalid
  input rejection. Unrelated native services are controlled substitutes.
- `Tools/TestDE128Ascension.ps1`: **1,284 checks passed**, zero compiler warnings
  or errors. Executes the actual package, production mode runner and current
  extracted encounter/fight/warrior/rule/reward projection. Covers five-win
  completion, duplicate settlement, loss reset, launch cancellation, prepared and
  between-trial reloads, all six challenge families, all sixteen buffs, both ranged
  variants, no-repeat selections, old/new save formats, invalid saved rules and
  selected behavior-rule dispatch. `Phase2HostStubs.cs` substitutes native
  launch/profile/UI services; this is not a Unity scene or economy simulation.
- Editor generation/check and **34/34 unit tests** passed. LuaLS and VS Code
  integration passed for the new rule and encounter fields. All seven actual Lua
  files had zero static analyzer diagnostics. Generated contracts contain 144
  public references, 76 constants and 191 structures.
- Final wiki build passed: **144** references, **47** pages and **4,001** local
  links/assets, with zero Astro source errors/warnings/hints. The existing `/404`
  route conflict warning remains. Final `git diff --check` passed.

Actual arena rendering, music playback, native collision/animation restrictions,
combat difficulty and campaign scene transitions still require an owner playtest.
Source tests do not establish those outcomes. No new assets or XML mod definitions
were shipped and no full Unity build or environment repair was performed.

### Superseded continuation

The owner subsequently rejected this direction. Ascension admission, trials and
rewards are not the current production plan. Both Lua prototypes and their test
launcher are commented out in Step 5.

## Step 5: XML-evidenced combat perks and move progression

Authorized 2026-09-18 with the instruction to comment out Ascension and implement
actual DE content using the XML evidence. Package version is `0.5.0`; the existing
`de128` namespace is retained for saved item compatibility. No separate release-64
identity was verified in the available files. The concrete authority is the
checked-in `Assets/DExml` archive, whose relevant snapshots are recorded below.

### Disabled content

`scripts/main.lua` no longer requires Ascension. The entire executable bodies of
`ascension.lua` and `ascension_rules.lua` are inside Lua block comments; each file
returns an empty table. The prototype runner `Tools/TestDE128Ascension.ps1` is also
commented and emits SKIP. The current manifest drops the Ascension persistent-state
capabilities. No prototype modes, fights, warriors, rules, quests, localization or
state schema are registered, even when tests explicitly require both disabled
modules. Generic reusable engine APIs remain available. No user save was edited.

### Exact XML mapping

| Archive node | Implemented Lua behavior |
| --- | --- |
| `perks.xml:/Perks/Perk[@Name='PERK_MASTER_OF_STYLE']` (6281-6324) | Style rank >=2 arms `MasterReward` for 300 simulation frames. Next unblocked outgoing Weapon/Unarmed PostHit adds Drain and consumes the bonus. Outgoing RangedMissile/MagicMissile or unblocked incoming HitPostCrit cancels it. |
| `perks.xml:/Perks/Perk[@Name='PERK_RELENTLESS']` (6325-6380) | Round start resets ComboCount. Combo >=3 starts/increments a retained counter, capped at15. Combo=0 arms a 300-frame reward; next outgoing PostHit multiplies damage by `1 + DamagePerStack * ComboCount` and consumes it. The source has no block/category filter on consumption. |
| `CharacterProgress.xml:/Progress/Perks/Perk[@Name='PERK_MASTER_OF_STYLE']/UpgradeLevel` (335-350) | Five ranks with Drain .02/.04/.06/.08/.10. |
| `CharacterProgress.xml:/Progress/Perks/Perk[@Name='PERK_RELENTLESS']/UpgradeLevel` (352-367) | Five ranks with DamagePerStack .01/.02/.03/.04/.05 and cap15. |
| `CharacterProgress.xml:/Progress/PerkTree/Level` Values4/8/11/14/17 | Level4 offers the two unlock choices; levels8/11/14/17 upgrade them in the same order. Other branches stay unchanged. |
| `animations/moves.xml:/Movesxml/Moves/Move/Locks/Perk` | Remove only the six direct locks absent from the corresponding DE moves, listed below. Preserve sibling and inherited requirements. |

The six move names are `DoubleJumpKick`, `ElbowStrike`, `TwoFootJumpKick`,
`BackFlipKick`, `ThrowSuplex` and `ThrowSuplexProfile`. The last two share
`PERK_SUPLEX`; the other four use their correspondingly named vanilla gate perks.
Simply replacing the level-up branches would leave these moves inaccessible under
canonical move data. The DE archive removes their locks instead of granting the
old perks, and the Lua module reproduces that distinction.

`combat_perks.lua` contains the executable behaviors, English text and upgrade
tables. `progression.lua` declares branch replacements and the six exact lock
removals. The mod reads no XML. It reuses the four core normal/blue
`IconMasterOfStyle` and `IconCrackedApple` sprites, present in the art catalog;
no new art is required.

### Source discrepancies and recovered host gaps

The original Master of Style English description says Aggressive-or-higher, but
the trigger ORs minimum styles Brutal/Aggressive/Crazy. The recovered condition
uses >=, making Brutal (rank2) the effective threshold. Lua follows the trigger
and preserves the original English description. `Root` in the SetHit expression
is the expression root, not a square-root operation.

Native DE unlocks begin at authored upgrade1. The existing scripted-perk bridge
inserted a rank0 baseline first. New generic `initial_upgrade` metadata selects
the first native progression variant; both DE perks set it to1. Default0 preserves
existing mods. First unlock is now rank1, and four upgrade opportunities reach
rank5. Existing learned-perk save ranks are never rewritten by registration.

Generic `on_hit_post_crit` and `on_post_hit` callbacks preserve the required phase
boundaries. Their snapshots include target side and the four actual native
animation-category matches. Only the outgoing PostHit side can mutate the current
hit through the bounded additive/scaling operations. Status icons use the existing
native icon and expiration path, with behavior/instance key isolation.

The archive requests `ApplyModEffect Type='Stack' StackCount=...`, while the
recovered C# effect parser/consumer only implements Pulse. The positive numeric
stack badge is explicitly new generic Eclipse presentation code, wired through
`ActionPerk.EclipseStackCount` to `ActivePerkItem`; it is not claimed recovered
Stack-effect execution. Zero hides the badge, and icon initialization updates it
on reuse. Its rendered appearance remains unverified.

`sf2.moves.remove_perk_lock` performs a targeted live removal after validating the
base XML and parsed condition identity. All requests are checked before mutation,
and adapter rollback/unload restores exact original lock lists. The runtime name
comes from the resolved perk's native identity, preserving its case independently
of normalized public IDs. Registration conflicts are explicit. Initial perk rank
and the ordered move-lock edits are included in content fingerprints.

### Verification

- `Tools/TestDE128Foundation.ps1`: **1,409 checks passed**, zero compiler warnings
  and errors, using current runtime/MoonSharp sources and the actual Lua package.
  Archive XML is read only as test evidence. Tests compare five rank values and
  exact English descriptions, all five branch slots, all six move-lock removals,
  Master cancellation/consumption, literal Relentless counter/consumption rules,
  300-frame expiry/refresh, round isolation, capability failures and inactive
  Ascension modules. Prior policy and Desolator regressions also pass. Fixture:
  `Temp/DE128Foundation-f918d5ef665447d18d37546f5c07d896`.
- `Tools/TestDECombatPerksNative.ps1`: **26 checks passed** using extracted current
  hit/status methods and controlled native services. Verifies exact phase order,
  category/side routing, shared pending damage, pre-init/local-versus guards,
  icon expiry/refresh/clear and stack data. The Unity badge is source-checked;
  the actual prefab is not rendered by this fixture.
- `Tools/TestPerkUpgradeNative.ps1`: **67 checks passed** using production
  `PerkItems` and extracted `PerkInfoItem.Clone`. Confirms initial0 compatibility,
  initial1 produces exactly ranks1..5, and a missing initial rank fails before
  publication.
- `Tools/TestMovePerkLocks.ps1`: **passed** for all six canonical/DE move pairs,
  exact-case matching, sibling/inherited condition preservation, rollback order,
  duplicate conflicts and capability routing. Uses actual new runtime metadata
  and extracted apply/remove methods with controlled animation/catalog services.
- Editor generation/check, **36/36 unit tests** and LuaLS passed. All **nine**
  on-disk Lua files have zero diagnostics, including two inert commented prototypes.
  Generated contracts contain 150 public references, 76 constants and 193 types.
- Wiki build passed: **150** references, **47** pages, **4,025** checked links/assets
  and zero Astro source errors/warnings/hints. The existing `/404` route warning
  remains. VS Code integration was not rerun for unchanged extension plumbing.
- `git diff --check -- Assets/Scripts Docs Mods Tools` passed. The full-tree check
  separately reports whitespace in the two preexisting generated project changes.

Full managed project builds stop before compilation at the preexisting MSBuild
`Microsoft.NET.Sdk` resolution failure (MSB4236/MSB4276). Two generated project files
were already dirty and contain whitespace errors; they were not edited as part
of this step. Source fixtures do not prove a full Unity build, rendered icon/badge,
live perk choice or campaign save/reload. Existing profiles receive no retroactive
perk grants. Check acquisition from the level4 choice in game.

### Archive snapshot hashes (SHA-256)

```text
perks.xml                 06D92F04C1362C0E6E5080C65B71E9854B35C73F79D0623B44E4F9EF51FE933B
CharacterProgress.xml     56404708018B10939910A1A8490355E9C5A33007E7C1E8C3E99114758ACB72B7
animations/moves.xml      0C79503439E07111AB178FE9DD6542E978F0A51EC959DAF7A1049D533D24051E
localizations/eng.xml      A143E643AA84E1806B1C11EA5E36892DFBF4F0EE1B8E9C4D6BC2407F27806081
```

Stop after reporting this XML-derived content slice. Further content starts from
explicit archive differences and requires the owner's continuation. Ascension
remains disabled.

## Step 6: saved pending forge orders

Authorized by the active continuation goal on 2026-09-19. Version **0.6.0** closes
the pending-forge portion of DE128-01. The no-wait requirement is recorded in
`Mods/DE_PARITY_TARGET.md`; no new art is required. Purchase/upgrade delivery
timers remain outside the supported `forge` subsystem and are still open work.

`content/timers.lua` now opts into `complete_pending = true`. This new public
`sf2.timers.set` field defaults to false and requires `seconds = 0`. Existing
mods keep their previous pending-order behavior. Registration remains capability
checked, transactional and exclusive per subsystem; opting in changes the content
fingerprint without changing fingerprints of old declarations that omit the field.

The C# host exposes zero effective remaining time while retaining the original
saved deadline. `ListSF`'s existing delivery update recognizes eligibility and
uses `UserItems.FinishDeliveryRecipe` / `ForgeManager.FinishEnchant` to settle.
This is normal completion, including the existing enchantment, clear, save and
notification path, not an early skip or a second material purchase. Failed
enchantment keeps the pending order. Disabling the policy before settlement
restores its original remaining time; already-settled enchantments stay settled.
No live player saves or archived/core XML were modified. The mod contains only
Lua declarations for this behavior; no DE-specific branch was added to C#.

### Verification

- `Tools/TestDE128Foundation.ps1`: **1,439 checks passed**, using the actual
  package and current public bindings. Includes default behavior, activation,
  disabling, invalid positive-duration opt-in, ownership conflicts, failed
  registration rollback, capabilities, fingerprints and earlier DE content.
- `Tools/TestForgePendingTimers.ps1`: **20 checks passed**. Compiles the complete
  production `RecipeItemInfo` and extracts current user-item save/clear, pending
  delivery, forge settlement and timer-update methods. Clock, enchantment and
  disk/profile services are controlled. Covers saved-order reload, retained raw
  timestamps, effective UI time, failure/retry, disabling before/after completion,
  stale receipt/repeated update, skip-disabled completion and natural expiry.
  This is not a live enchantment-randomization or disk-crash test.
- Managed Assembly-CSharp and editor builds plus native Unity recompile passed.
- Editor schema generation/check and **36** project tests passed. LuaLS 3.18.2
  passed, including completion of `complete_pending`. VS Code integration passed.
- Wiki build passed, including reference coverage and **4,025** local link/asset
  checks across 47 pages. The existing `/404` route conflict warning remains.

No full forge UI/gameplay or real-profile save/reload playtest was performed.
Use the updated README acceptance steps on a test profile before claiming full
in-game acceptance. DE parity remains incomplete; Ascension stays disabled.

## Step 7: former battle-pass equipment in the shop

Version **0.7.0** implements the `list.xml` shop-access delta for all five pieces
of each of these archived collections:

| Legacy name suffix (weapon, armor, helm, ranged, magic) | Minimum player level |
| --- | ---: |
| `BP_S1_GUARDIAN` | 15 |
| `BP_S2_SKANDA` | 20 |
| `BP_S3_WIND_MAKER` | 25 |
| `BP_S5_TIME_SHIFTER` | 45 |
| `BP_S4_SCRIPTWRITER` | 50 |

Classification: `intentional_de`, supported by the parity target's alternate
acquisition requirement and `/List/Items/Item` records with these exact names.
The base marks every item `ShopHide="1"`; the archive removes that attribute.
All 25 have unchanged, positive canonical `BonusPrice` values and identical icon
and model references. `scripts/content/shop.lua` uses the existing core IDs and
`sf2.shop.set_availability`, without copying items or embedding/reading XML.

The missing capability was a level gate on availability. The generic optional
`minimum_level` field accepts integers 0..52 (default 0). Native shop listing and
item-availability queries apply it together with the policy's required group.
Force-visible bypasses the original hidden/group gates but not the policy's new
requirements. Validation, ownership conflicts, atomic entrypoint rollback and
content fingerprints cover the field; omitted/zero retains old fingerprints.
Disabling the mod restores base availability without changing saved ownership.

The archived `Level` is used only as a shop access threshold. Canonical item
level, upgrade level, stats, earned-gem price, enchantments, model and identity
are untouched. Archive equipment-power/upgrade differences remain classified
separately against the immutable Eclipse economy requirement. Non-economic
combat differences also remain open: for example Wind Maker ranged uses `Kunai`
in DE and `Chakram` in base. This step is **shop-access coverage**, not complete
battle-pass set/ability/equipment parity. Other special-offer collections remain
unconverted; no existing rewards or newly acquired items are migrated.

### Verification

- `Tools/TestDE128Foundation.ps1`: **1,847 checks passed**. Executes actual Lua
  modules against canonical core projections. New shop coverage checks all 25
  exact archive identities, eligibility just below/at/above each threshold,
  inherited visibility, required-group composition, null guards and unloading.
  It compiles the real `ShopAvailabilityPolicy` with controlled item/profile
  services. Numeric/type bounds, default compatibility, distinct fingerprints,
  conflicting policies in both load orders, capability failures, missing modules
  and rollback are covered. These are controlled tests, not purchase gameplay.
- `Tools/TestP1CContracts.ps1`: passed (existing structural API checks).
- `Tools/TestForgePendingTimers.ps1`: **20 checks passed**, retaining prior saved
  forge settlement coverage. Its controlled fixture types overlap the new shop
  fixture assembly and produce CS0436 warnings; its own types are used.
- Managed editor build (including runtime and recovered dependencies) passed:
  zero errors, existing recovered-code warnings. Unity recompile completed with
  no errors.
- Native `Tools/VerifyDE128ShopAssets.cs` probe: all **25 model texts and 25
  imported icon sprites** loaded through `CoreAssetProvider`; all icons have
  four vertices. Run with `unity command eval_file --file
  Tools/VerifyDE128ShopAssets.cs --json`. This does not instantiate fighters or
  validate every referenced texture or attack animation.
- Editor generation/check, **36** project tests and LuaLS passed, including the
  new availability field completion. Public API docs, generated types and the
  connected-mod example describe the implemented scope.
- VS Code integration passed. Wiki build passed with **4,026** local link/asset
  checks across 47 pages and no Astro source diagnostics. The existing `/404`
  route conflict warning remains.

Native shop purchases, previews/equipping, live level changes and real-profile
save/reload acceptance remain outstanding. README contains the test sequence.
No user save or archived/core XML was changed. Ascension remains disabled.

### Archive snapshot hashes (SHA-256)

```text
Assets/DExml/list.xml       B9F2D71E25FC396281A469DFCE85C9CD1EB253A61A5329967BEEA6ED1BEFB76E
Assets/vanillaXml/list.xml  147AFCF311C2145E1A90140597D71370088192B067D84C2548D19A1C568C299C
```

The timer investigation also confirmed that the current base `ListSF` purchase
path already sets new delivery to zero and `UserItems.DINFNDFAJMB` settles saved
purchase/upgrade delivery on its next tick. A configurable non-forge timer API is
still absent; this absence alone does not prove DE currently waits for those
orders. No timing behavior was changed in this step.

## Step 8: equipment combat-family patches

Version **0.8.0** implements the following intentional, non-economic `SubType`
deltas from `/List/Items/Item` in the archived `list.xml`:

| Item | Canonical family | Archived family | Native move evidence |
| --- | --- | --- | --- |
| `WEAPON_CHNY22_SPEAR` | Spear | Naginata | Seven attack definitions including `NaginataSuperSlash` |
| `WEAPON_RAID_KARCER_SET` | Claws | HunterClaws | Seven HunterClaws attack definitions |
| `WEAPON_BG_YARI` | Spear | MagariYari | Seven attack definitions including `MagariYariSuperSlash` |
| `RANGED_BP_S3_WIND_MAKER` | Chakram | Kunai | `RangedKunaiPlayer` and its shop preview |

The new `scripts/content/combat_equipment.lua` module uses generic
`sf2.items.set_subtype { item, subtype }`. Eclipse provides typed, capability-
checked registration, per-item conflicts, rollback, fingerprints and scoped native
application. The subtype affects move conditions, projectile routing and AI
fallback; an explicit tactic group remains independent. Duplicate recovered names
are resolved using the original node identity, not just the first matching name.
New fighter copies retain the applied family; already-built copies keep their
snapshot. Teardown restores the original item. No inventory, price, power,
upgrade, model or enchantment data is rewritten, and no XML is shipped/read by
the Lua mod.

Following the naming convention, the existing native `ItemInfo.MDPPNGIEJGD` field
is now `SubType`, and the unused-name `Items.KLJFJJJPPJJ` list property is now
`AllItems`. Both declarations carry `// best guess for name`. Only their actual
callers/fixtures changed; the similarly named condition-class properties remain
untouched. These plain runtime classes are not Unity serialized components, the
XML attribute remains `SubType`, and no mapping/GUID changes were made.

### Dependency findings and next content work

The first archive test caught that `WEAPON_CHNY21_JIAN` cannot yet switch from
HermitSwords to ChineseSwords: the base has **zero** ChineseSwords item-condition
consumers. The archive extends ten Sai stance/attack definitions and adds
`ChineseSwordsSuperSlash` plus its shop preview. The animation binary is present
at `Assets/Resources/gamedata/animations/binary/chinese_swords_super_slash_old.bytes`;
this is an API/content-graph dependency, not an absent-art blocker.

The exact authored graph needs capabilities beyond today's move API: extending
existing alternative item locks, repeated key sequences (two Punch taps),
transitions/alignment, additional native conditions, mixed damage terms with
shifts, `Spinning`/`HighHeavy` hit reactions, timed/random sound actions and shop
completion/profile metadata. Current attack registration supports one damage
attribute and only High/Middle/Low; it cannot reproduce the archive by substituting
an approximate hit or family. Jian stays on its original working family until
that dependency chain is implemented. This is the next substantial move-authoring
slice, not a claim that Jian parity is complete.

The archive also omits Monk Katar's `Katars` and Musket's `Rifle` tactic overrides.
Omission alone does not establish intentional DE behavior: the recovered tactics
pack has no MonkKatars/Musket-named archives. These remain `unresolved` relative
to reconstruction compatibility; the mod does not remove their base AI groups.
The apparent GlaivebowArrow subtype change comes from duplicate legacy records
and is not treated as an ordinary unique-item delta.

Music reconnaissance found 109 shared battle-level music-name differences.
Several restored names currently map to substitute clips in `Sound.cs` (including
old-version names). Merely patching names would not prove cut-music restoration;
no such patch was added or counted as restored music.

### Verification

- `Tools/TestDE128Foundation.ps1`: **1,944 checks passed**. Actual Lua package,
  exact archived subtype deltas, canonical move-condition consumers, explicit
  API capability enforcement, rejected economic fields, invalid subtype bounds/
  types/characters/categories, conflicts in both orders, atomic failure, unload/
  rebuild and subtype-sensitive fingerprints.
- `Tools/TestItemCombatSubtype.ps1`: **60 native checks passed** using compiled
  production item parsing/copying, `ConditionItemInfo` and `LegacyContentAdapter`.
  Covers weapon/ranged/magic, explicit AI-group independence, concurrent/stale
  scopes, duplicate-name identity, partial application failure and restoration.
- Existing native tactic-subtype fixture: **51** passed; projectile runtime:
  **211** passed, including real move XML/parser and 136 attack intervals.
- Regression checks after descriptive field naming: battle-result capture **12**,
  profile query **18 native + 57 Lua**, immediate purchases **32**, form animation
  entry/readiness **71**, and item acquisition/publication **22** passed. Stale
  test stubs were updated to current member names; profile method extraction now
  accepts CRLF. These controlled fixtures do not simulate a live profile/fight.
- Managed editor build (including dependencies) passed with zero errors and
  recovered-code warnings. Unity recompile completed without errors.
- Editor schema generation/check, **36** project tests, LuaLS and VS Code
  integration passed. Public contract now has 151 functions and 194 structures.
- Wiki build passed: 47 pages, **4,031** local link/asset checks, no Astro source
  diagnostics. The existing `/404` route conflict warning remains.
- Scoped/full whitespace checks passed. Canonical/archived XML, project settings,
  Unity identities and player saves were not changed.

Live combat timing, rig/contact quality, NPC AI and equipment preview acceptance
remain unverified. README lists the in-game checks. ChineseSwords and full DE
parity remain incomplete; Ascension stays disabled.

Source hashes retained from Step 7 for `list.xml`; additional SHA-256 evidence:

```text
DExml/animations/moves.xml       0C79503439E07111AB178FE9DD6542E978F0A51EC959DAF7A1049D533D24051E
vanillaXml/animations/moves.xml  FD02CCA484BA593ACE270D50926899DCCB1864D5BFCD56AE8CB2E06551CDE241
chinese_swords_super_slash_old.bytes 6810E8BE5CE50A88DAD84EEB7660D9AB92552B69DBB209B94C3998278109E5FA
```

## Step 9: Chinese swords attack and input authoring

The current package stays **0.8.0** because this step builds the missing combat
API and Lua sections without activating an incomplete move. No XML is shipped,
read or patched by the mod. The C# adapter projects typed Lua declarations into
the recovered parser's existing in-memory representation.

### Implemented

- `ModMoveAttack` and `sf2.moves.register[_template]` intervals now accept
  `damage_terms`: 1–4 unique native attribute types with finite shifts in
  -1000..1000. The old `damage_type` shorthand remains; specifying both is an
  error. The native comparison/alignment formula is retained. Terms are not
  summed damage or extra hits. New definitions retain ordered, immutable terms.
- `Spinning` and `HighHeavy` native hit reactions are supported alongside
  High/Middle/Low. No new reaction assets or DE-specific C# rules were added.
- Key conditions preserve repeated entries, enabling Punch/Punch with a held
  Forward key. Native `ConditionKeys` accepts the double tap and rejects the
  single tap or missing Forward hold.
- Added typed `round_stage`, `screen` and `mod_exists` conditions. Stage/screen
  values follow the recovered parser's supported names. `mod_exists` tests a
  combat effect, not a loaded Lua package; effect names remain native strings.
- All additional terms/shifts and repeated inputs participate in deterministic
  fingerprints. Existing single unshifted definitions keep their prior format;
  explicit one-term zero-shift arrays project/fingerprint like the shorthand.
- A regression uncovered MoonSharp retaining nil tombstones after `table.remove`.
  The shared array validator now ignores absent Lua entries and validates every
  live key as an integer in 1..count. It still rejects true holes, extra named
  keys and fractional/zero/negative indices. This is generic C# binding behavior.

The authored `scripts/pending/chinese_swords.lua` returns the exact eight input
conditions and eight intervals (four attack intervals) of the archived
`ChineseSwordsSuperSlash`, plus its preview screen restriction. Tests load this
actual Lua file, register temporary templates through the real binding, project
through `LegacyContentAdapter`, and compare every authored element/attribute to
`Assets/DExml/animations/moves.xml`. Source XML is test evidence only. `main.lua`
does not load the pending module, and Jian's combat subtype remains HermitSwords.

### Verification

- `Tools/TestMoveAttackAuthoring.ps1`: **167 checks passed**. Compiled production
  Lua bindings, adapter, `ConditionKeys`, native condition and attack parsing;
  all archived authored sections, invalid terms/conditions, rollback after a
  prior registration, fingerprint differences, deterministic reload, and valid
  edited arrays versus real malformed arrays. The standalone fixture initializes
  MoonSharp before loading Unity reference DLLs to avoid calling unavailable
  Unity Resources native functions; production supplies its own sandbox loader.
- `Tools/TestDE128Foundation.ps1`: **1,944 checks passed** for the unchanged active
  package, including composition, rollback, capability boundaries and perk traces.
- `Tools/TestProjectileRuntime.ps1`: **211 checks passed**, including 136 native
  attack intervals. `Tools/TestP2ACombatRuntime.ps1` passed public Lua examples,
  state/migration, capability lifetimes, settlement and atomic rollback.
- Managed editor build including runtime/firstpass/game dependencies passed with
  zero errors (15 existing editor-build warnings). Unity recompile completed with
  `failed: false`, `compilationFailed: false` and an empty error list.
- Editor generation/check: **151 functions, 76 constants, 198 structures**;
  **36 project tests**, LuaLS field inference (including nested damage terms),
  and actual VS Code integration passed. The VS Code runner was invoked directly
  with its executable path after npm's Windows argument escaping failed.
- Wiki build passed: 47 pages and **4,031** local links/assets, no Astro source
  diagnostics; the existing `/404` route warning remains. `git diff --check` passed.

### Next graph slice and limits

Before activation, implement exact extension of the ten Sai alternative item
locks, the SaiHeavySpit transition with frame shift 2, alignment/direction,
timed/random sounds, profile/tactics metadata, and shop completion/no-wall/no-
interpolation fields. Then register both full moves via Lua and apply Jian's
subtype delta. The binary exists; these are API gaps rather than missing art.
The pending module is not a replacement move, a rough gameplay approximation,
or proof of combat parity. No live fight, damage balance, contact timing, AI,
preview or player-save acceptance was performed in this step. Ascension remains
disabled, and full DE128 parity remains incomplete.

## Step 10: Chinese swords locks, transitions and positioning

The package remains **0.8.0**. This step extends the pending Lua module and the
generic C# move API; it does not activate an incomplete ChineseSwords family.
Archived XML remains comparison evidence, never a mod runtime dependency.

### Implemented

- `sf2.moves.extend_item_lock` adds an item subtype to one positive direct item
  clause or one direct OR group selected by move, item type and source subtype.
  Other skeleton, screen and perk requirements retain their native objects.
  Negated, nested, AND-only, named-item, missing and ambiguous matches fail.
  Selectors refer to original clauses, so one addition cannot depend on another
  pending addition. The adapter validates the whole batch before changing live
  lists and restores replaced clauses during teardown. Capability, conflict and
  fingerprint handling are integrated into the normal content transaction.
- Move definitions now support typed `locks`, `transitions`, `align` and
  `direction`. Transition conditions use exactly one relative frame shift or
  absolute first frame. Alignment uses validated axes and native points; facing
  requires explicit players. Unsupported native combinations are rejected.
  Templates reject transitions because the recovered parser does not inherit
  them. Existing definitions without graph data retain their fingerprints.
- The pending Lua module authors all ten Sai item-lock extensions, weapon and
  skeleton locks, the SaiHeavySpit/SemiUninterrupt transition at frame shift 2,
  combat alignment/facing, and shop screen locks with the archived -57 offset.
  `main.lua` still does not load it, and Jian remains HermitSwords.
- Native members newly used by the implementation received narrow descriptive
  names marked `// best guess for name`, with actual callers and fixtures updated.
  No deobfuscation mapping, serialized asset identity or archived XML changed.
- Public wiki, editor schema/generated definitions, nested completion checks
  and current production status describe the implemented contract.

### Verification

- `Tools/TestMoveGraphAuthoring.ps1`: **364 combined checks passed**, including
  the 167 attack/input checks. Actual pending Lua definitions are compared with
  archived combat and preview graph sections through production projection and
  native transition/alignment/direction parsers. All ten base move eligibility
  truth tables match the archive for Sai/HermitSwords/ChineseSwords/Katana and
  Skeleton/Titan. Tests cover screen restrictions, atomic failure, ambiguous
  clauses, composition, teardown, fingerprints and rejected input.
- Active DE foundation: **1,944 checks passed**. Native projectile checks: **211**;
  form animation entry: **71**; form parameter copying: **18**; AI eligibility:
  **14**. Move perk-lock and showcase regression fixtures passed. Stale fixture
  names/stubs were updated to current production declarations; these controlled
  checks do not constitute live gameplay or raid acceptance.
- Managed editor build passed with zero errors and 15 existing warnings. Unity
  recompile completed with `failed: false`, `compilationFailed: false` and no errors.
- Editor generation/check: **152 functions, 76 constants, 203 structures**;
  **36 project tests** and LuaLS completion/diagnostic checks passed. VS Code
  integration could not launch because its updater held `vscode-updating` beyond
  the launcher's timeout; no integration pass is claimed for this step.
- Wiki build passed with zero Astro diagnostics: 47 pages and 4,039 local
  links/assets checked. The existing `/404` route warning remains.
  `git diff --check` passed; Git reported only existing line-ending notices.

### Remaining before activation

Timed and random strike sounds, profile rank/icon, tactics distance metadata,
shop `TryOnEnd` completion, and no-wall/no-interpolation fields remain. After
those are supported, register both complete moves through Lua and apply Jian's
subtype delta. Live combat, timing/contact, AI, preview and player-save acceptance
remain unverified. Ascension stays disabled; full DE128 parity remains incomplete.

## Step 11: Chinese swords sound and presentation authoring

The remaining presentation fields are now available through generic C# runtime
APIs and authored in the pending Lua module. Active package behavior remains
**0.8.0** until complete registrations and native integration are checked.

### Implemented

- Direct move registration accepts up to 64 scheduled actions with exactly one
  native frame or event. `random_sound` retains an ordered list of 1–32 core sound
  names and uses the existing native random-sound action; `try_on_end` uses the
  existing shop completion action. These are typed declarations, not arbitrary
  XML or a procedural operation language. The mod reads no XML.
- `profile` supplies a native moves-list rank/core icon; `tactic_distance`
  supplies an axis, finite bounds and typed source/destination points. `Full`
  omits the native axis attribute because any non-X attribute otherwise means Y.
  `no_wall_repulsion` and `no_interpolation_frames` preserve native move flags.
  These fields are move-only; templates reject them explicitly.
- Ordered action choices, scheduling, profile, distance and flags participate in
  fingerprints. Empty/default presentation retains previous fingerprints.
  Unknown fields, invalid schedules, paths masquerading as core names, malformed
  arrays and invalid points/bounds fail registration without leaking content.
- The pending Lua module now contains swishes at frames 8, 17, 28 and 33;
  six random strike sounds; rank 4 and `Trick7.super_slash`; the 200–800 X tactic
  distance; preview swishes and AnimationEnd completion; both preview flags.
  No runtime policy specific to DE was added to the C# implementation.
- Public move documentation and editor schema/generated declarations now expose
  all fields, defaults, limits and native-resource/verification constraints.

### Verification

- `Tools/TestMovePresentationAuthoring.ps1`: **504 combined checks passed**,
  including the prior 364 attack/input/graph checks. The actual pending Lua module
  projects matching archived Profile/Tactics/Actions sections and preview flags.
  Native action types, exact start frames/events, random choice arrays and gender
  behavior, parsed distance fields, profile rank/icon, fingerprints and rejection
  rollback are checked. This does not exercise live sound playback or combat.
- `Tools/TestDE128Foundation.ps1`: **1,944 checks passed** for the active package.
- Managed editor build completed with zero errors and 2,739 warnings; subsequent
  game builds completed with zero errors/warnings. Unity recompile completed with
  `failed: false`, `compilationFailed: false` and an empty error list.
- `Tools/VerifyDE128MovePresentationAssets.cs` ran successfully inside Unity:
  all nine clips loaded with nonzero samples (swishes at 44,100 Hz; strikes at
  22,050 Hz), and the native UI resolver returned a four-vertex profile sprite.
  This is import/resolution evidence, not audible or rendered acceptance.
- Editor generate/check: **152 functions, 76 constants, 206 structures**;
  **36 project tests**, LuaLS including nested action/distance completions, and
  real VS Code integration passed. VS Code's updater no longer blocked launch.
- Wiki build passed: 47 pages, **4,042** local links/assets, zero Astro diagnostics;
  the existing `/404` route warning remains. `git diff --check` passed.

### Next integration step and limits

Register both complete moves through Lua using the existing binary and exact
attributes/events/templates, verify inherited native behavior and profile naming,
then apply the ten item-lock extensions and Jian's subtype delta. The pending
module remains absent from `main.lua`; the active package does not advertise
ChineseSwords gameplay yet. No fight, sound playback, AI, shop-preview completion
or player-save acceptance was performed. Ascension remains disabled and full
DE128 parity remains incomplete.

## Step 12: Activate Chinese swords combat and preview

Package **0.9.0** now loads `content.chinese_swords` from `main.lua`. The module
registers both complete moves, extends the ten archived Sai item-lock clauses,
and changes `WEAPON_CHNY21_JIAN` from HermitSwords to ChineseSwords in the same
normal transaction. The data module moved from `scripts/pending/chinese_swords.lua`
to `scripts/content/chinese_swords_data.lua`. No runtime XML is shipped or read.

### Integration and runtime changes

- Full declarations retain the archived attributes, input, four hit intervals,
  locks, transition, alignment/facing, sounds, profile/tactic metadata, preview
  completion and events. All eleven inherited core templates exist and match the
  archive; no additional template approximation was needed.
- The package owns a byte-identical copy of the recovered animation at
  `assets/animations/chinese_swords_super_slash_old.bytes`, resolved through
  `sf2.assets.binary`. SHA-256 remains
  `6810E8BE5CE50A88DAD84EEB7660D9AB92552B69DBB209B94C3998278109E5FA`.
- Native profile headings otherwise use the internal move name as a localization
  key. Added generic optional `profile.display_name`, a validated localization
  handle. The adapter supplies it to `Trick.DisplayName`, and the profile UI uses
  that heading while keeping `Trick.Name`/animation identity unchanged. Existing
  profiles retain their original name fallback. Lua registers the English title
  “Super Slash”; this title is an authored fallback, not a recovered translation.
  Title references participate in fingerprints and editor completion.
- Jian's prices, level, saved ownership and availability rules are not changed.
  The separate archived `WEAPON_CHINESE_SWORDS`, absent from the canonical item
  list, remains unimplemented. Monk Katar/Musket AI group intent remains open.

### Verification

- `Tools/TestDE128ChineseSwords.ps1`: **528 combined checks passed**, including
  the preceding attack, graph and presentation suites. Executes the actual
  registration module, compares both full moves with the archive (normalizing
  owned identity/asset references and irrelevant top-level section ordering),
  checks every inherited template, verifies the packaged binary hash, title
  handle validation/fingerprints and legacy profile fallback.
- `Tools/VerifyDE128ChineseSwordsNative.cs` passed inside Unity. It parsed two
  complete moves and one correctly titled rank-4 profile using temporary native
  catalogs, accepted ChineseSwords/Skeleton on the correct screens and rejected
  unrelated equipment. Parser lookup objects are restored in `finally`; the
  current live move list and saves are not changed by this probe.
- The same Unity probe decoded both moves' binary into **38 samples / 67 nodes**,
  verified consistent finite data and attack bounds. The archived Uninterrupt
  interval ends at 50 although the binary ends at 37; the native interval reader
  clamps it to 37 and reports its end at 38. This was explicitly checked rather
  than lengthening the binary or altering the archive. All attacks and scheduled
  sounds are inside the available samples. Live timing remains unverified.
- `Tools/TestDE128Foundation.ps1`: **2,027 checks passed** with the active package,
  including five subtype patches, both moves, ten extensions, localized title,
  disable/rebuild, and atomic failure for missing registration/data modules or
  the new animation binary. These are controlled runtime fixtures, not save or
  campaign acceptance.
- Managed editor build passed with zero errors and 15 warnings; Unity recompile
  completed successfully with no errors. Editor generation/check, 36 project
  tests, LuaLS including profile-title completion, and VS Code integration passed.
  Contract remains 152 functions, 76 constants and 206 structures.
- Wiki build passed with zero Astro diagnostics: 47 pages / 4,042 local links
  and assets. Existing `/404` route warning remains. `git diff --check` passed.

### Remaining acceptance and production

An actual fight must still verify double-tap input, rig/contact and reactions,
NPC tactic use, audible scheduling, and recovery timing; the shop must verify
preview completion and the profile heading visually. No player-save test or
running-game mod restart was performed. Next production work includes that
acceptance and the remaining archived equipment/content deltas. Ascension stays
disabled. This activates one complete authored move family; it does not establish
full DE128 gameplay parity or complete the engine roadmap.

## Step 13: Isolated live Chinese swords acceptance

The actual **0.9.0** package now passes one bounded live combat case in Unity
6000.6.0f1. This step adds repeatable engineering fixtures; it does not change
DE gameplay policy or claim that the remaining acceptance gates are complete.

### Harness and observed result

- `Tools/TestDE128CombatNative.py` prepares an independent project copy using the
  existing form-test copier, adds the real DE128 package and the Lua fixture under
  `Tools/Fixtures/de128-combat`, then runs `Tools/ValidateDE128CombatNative.cs`.
  The clone uses its own product/save identity and mod root. It never starts a
  fight in the working editor or touches the owner's saves. Reuse accepts a
  stopped marked clone, preserves prior logs, and backs up/hashes refreshed inputs.
- The Lua fixture registers an arena, Jian-equipped opponent and fight using the
  public APIs. Full game boot applies DE128 with no mod diagnostics. The live
  opponent receives `WEAPON_CHNY21_JIAN` / `ChineseSwords`.
- At simulation frame 120, the harness injects the authored double-tap/Forward
  keys through the native controller/event path, mirrored for the actor's facing.
  The native selector starts the namespaced ChineseSwords move at frame **122**.
  Keys are then released; the move is not selected directly by name.
- All four attack intervals start at samples **11, 19, 27, 32**. Each interval's
  authored weapon-edge count matches the actual bound rig edges; this catches the
  native binder silently skipping missing edges. All four random-sound actions
  fire at samples **8, 17, 28, 33**. Animation completion is observed, the fighter's
  object/rig remain active, and simulation continues beyond 180 frames after
  selection with no captured exception after body readiness.

Passing evidence:
`Temp/FormNative-qpsmkaqv/DE128Runs/Run-34hr23yy/de128-validation.log`
(Unity exit 0, explicit `[DE128Native] PASS`). The input-source hashes and previous
fixture versions are alongside that log. The initial fresh import/run took about
175 seconds; cached reruns took 35–39 seconds.

### Verification scope and earlier fixture failures

The first run supplied screen-right keys to a left-facing fighter and correctly
selected SaiSpinningSpit. The second selected ChineseSwords but held the synthetic
keys, causing a repeat. These were fixture input-lifecycle mistakes, corrected by
mirroring and releasing the injected sequence; no game-code workaround was added.
The subsequent run passed all assertions. Python syntax validation, fixture
manifest/Lua editor analysis, Unity compilation and `git diff --check` passed.

The log also contains a pre-game exception from UnityEditor.Search's startup
indexing, before body readiness. It did not prevent the fixture from running and
is not a gameplay exception; this step does not claim an entirely error-free
editor session. The copied form example was also discovered, but its separate
encounter/behavior is not attached to this fixture's Jian opponent.

### Still open

Both fighters are immortal and opponent AI is disabled after readiness. This
case does not establish physical keyboard/gamepad input, AI tactics, hit contact
or damage/reactions, audible output, visible profile rendering, shop completion,
player-facing recovery timing or save continuity. Those remain separate checks.
The archived standalone `WEAPON_CHINESE_SWORDS` item and broader equipment/content
deltas remain production work. Ascension is still disabled; full DE128 parity
and the engine roadmap remain incomplete.

## Step 14 - missing weapon listings and native group membership

DE128 0.10.0 adds `scripts/content/restored_weapons.lua`, loaded from `main.lua`.
It restores nine of the ten weapon definitions present in archived `DExml/list.xml`
but absent from the canonical vanilla item list. All declarations, localization,
listings, availability and enchantments are Lua. XML is used only by engineering
comparisons, never shipped/read by the mod.

| Weapon | Level | Gems | Act group | Default enchantment / aspect | Initial damage |
| --- | ---: | ---: | --- | --- | ---: |
| Super Knives | 9 | 50 | ACT_2 | Precision / 296 | 186 |
| Batons | 11 | 55 | ACT_2 | Weakness / 366 | 236 |
| Dragon Knives | 13 | 60 | ACT_3 | Overheat / 442 | 292 |
| Poleaxe | 19 | 78 | ACT_4 | Precision / 658 | 448 |
| Kelt Axes | 26 | 106 | ACT_5 | Weakness / 909 | 629 |
| Fans | 29 | 121 | ACT_5 | Bloodrage / 1014 | 704 |
| Imhotep Axes | 36 | 165 | ACT_6 | Stun / 1265 | 885 |
| Chinese Swords | 42 | 214 | INTERMISSION | Frenzy / 1511 | 1071 |
| Giant Sword | 50 | 305 | ACT_7_3 | Time Bomb / 1797 | 1283 |

Names, model/icon references, move families, prices, enchantments and group labels
come from the archive. Kelt Axes retains its archived unknown-item placeholder
icon. Initial damage derives from the existing vanilla Weapon_Bonus milestone;
the nine results match both explicit archived values and native parsed attributes.
Chinese Swords uses the family activated in Steps 12-13. Desolator remains reward
only; this batch does not add a purchasable Desolator listing.

**Runtime gap fixed:** owned equipment had no pack label. `required_group` gated
shop queries, but native quest unlock/new-item paths use ItemInfo pack membership.
The generic C# equipment adapter now projects an owned equipment policy's nonempty
required group into that native label. It does not rewrite core labels or existing
non-equipment pack metadata. The public shop reference, editor schema/help,
generated definitions and editor guide document this behavior. No DE IDs or act
policy were added to C#.

**Discovered gap, still open:** Moon Fans has no WeaponDamage attribute in the
archive. A real native comparison confirmed absent/zero there versus present/760
from current owned-equipment progression. It remains unregistered, with its
remaining row documented next to the Lua definitions. Its model and placeholder
icon are available. The next step must design and implement generic typed initial
stat authoring/semantics, then finish Moon Fans; do not silently normalize its
stats or claim all missing weapons restored. This is an API gap, not missing art.

Verification:

- Foundation suite: **2,152 checks passed**, including actual Lua registration,
  all nine archive comparisons, act/level gate behavior, exact enchantment aspects,
  missing module/art transaction rollback and rebuild/reenable checks. The
  pre-existing battle-pass and Desolator tests remain active.
- Reward-only equipment adapter regression: **63 checks passed**.
- Managed editor project build: **0 errors, 149 warnings**.
- Independent Unity 6000.6.0f1 run: **terminal exit 0**. The harness checks all nine
  native definitions, parsed damage, prices, levels, runtime group membership,
  exact default perk/aspect and real model-text/sprite loading, then repeats the
  live Jian four-hit selection/animation acceptance. Evidence is retained at
  `Temp/FormNative-qpsmkaqv/DE128Runs/Run-8bldlxd1/de128-validation.log`.
- Native diagnostic run `Run-f1tunqd_` caught Moon Fans' initial damage mismatch;
  an earlier run caught incorrect placement of the new pack projection. Both
  failures were investigated before the final passing run.
- Wiki build: 47 pages and 4,042 links/assets passed; existing duplicate `/404`
  route warning remains. Editor generation/check, unit tests, LuaLS and VS Code
  integration passed. VS Code was run directly through the Node launcher after
  correcting the required executable argument and npm/PowerShell quoting.
- `git diff --check` passed, with line-ending conversion warnings.

These checks do not prove purchase/equip/save/reload flows, visible notification
rendering, every restored weapon's combat behavior, or full equipment parity.
The isolated run leaves the working editor/player saves alone. Ascension remains
disabled. Full DE128 production is still active.

## Step 15 - typed initial equipment stats and Moon Fans

DE128 0.11.0 restores `de128:items/weapon/moon_fans` through Lua. It uses the
archived model, unknown-item icon, Fans subtype, level 31, 132-gem price, ACT_6
gate and Lifesteal aspect 1090. `initial_stats = {}` preserves the absence of
WeaponDamage instead of silently assigning the canonical level-31 value of 760.
This completes the ten archived weapon definitions absent from canonical vanilla;
it does not mean all equipment differences or weapon gameplay are complete.

The C# API now accepts optional `initial_stats` on all five equipment registration
functions. The immutable typed snapshot validates category-appropriate attributes
and integer values 0-1000000. Omitted/nil uses existing canonical initial power;
an explicit table replaces the complete initial snapshot, leaving unspecified
attributes absent. An empty table and an explicit zero are distinct. The native
adapter still supplies normal levels, prices and upgrade templates. The snapshot
is not a permanent stat override, does not rewrite existing inventory and does
not replace normal level-scaled acquisition or upgrade behavior. No DE identifiers
or policy entered the engine.

Changed engine sources: `ModContent.cs` (validated immutable definitions and
registration), `ModScripting.cs` (capability-checked facade),
`MoonSharpScriptRuntime.cs` (strict Lua parsing), `LegacyContentAdapter.cs`
(projection) and `ModSaveData.cs` (deterministic optional fingerprint data).
Legacy declarations without the field retain their fingerprint representation.
The equipment/shop wiki, editor schema, generated definitions, starter and editor
guide document the new contract and its limits.

Verification:

- **2,239 foundation checks passed**, including actual Lua registration for every
  equipment category, immutable snapshots, malformed/category/range/nonfinite
  value rejection, whole-transaction rollback, and distinct fingerprints for
  derived, absent, explicit-zero and positive snapshots. All ten restored weapons
  are compared with archived listing/gate/enchantment evidence.
- **135 extracted adapter checks passed**, including all five categories with
  empty and explicit snapshots, omitted-field behavior, zero preservation,
  upgrade-template retention, removal and previous default equipment regressions.
- Managed editor project: **0 errors, 149 warnings**.
- Isolated native Unity run **passed, terminal exit 0**. The ten definitions match
  archive damage values AND attribute presence, prices, levels, subtypes, group
  labels and exact enchantment aspects; all model texts/sprites load. Moon Fans
  is absent/zero like its archived native ItemInfo. Applying the first normal
  upgrade to isolated copies of each yields **766 damage** in both. The live Jian
  four-hit selection/interval/sound-action/completion acceptance still passes.
  Evidence: `Temp/FormNative-qpsmkaqv/DE128Runs/Run-7mr313t6/de128-validation.log`.
- Editor generation/check: 152 functions, 76 constants, **211 typed structures**;
  unit tests, LuaLS (including each initial-stat table's completion), and real
  VS Code integration passed.
- Wiki: 47 pages and **4,046 links/assets passed**, with the existing duplicate
  `/404` route warning. `git diff --check` passed with line-ending warnings.

No purchase/equip/save/reload UI acceptance is claimed, and the native upgrade
check does not spend player currency. Other weapons' live combat and notifications
remain open. The next archive inventory has 12 missing armor, 15 helms, two ranged
items, seven magic items and eight consumables; some armor/helms are hidden NPC
forms with level-1/no-upgrade metadata. These need asset/move/usage audits before
registration and may need further generic API support. Ascension stays disabled;
full DE128 production remains active.

## Step 16 - restored armor, helms, ranged and magic definitions

DE128 0.12.0 adds `scripts/content/restored_equipment.lua`, loaded explicitly from
`main.lua`. It uses existing public APIs; no new engine policy or API was needed
for these eight item definitions. All definitions, names, gem prices, group/level
gates and default enchantments are authored in Lua. The mod still ships no XML.

| Item | Category | Level | Gems | Group | Default enchantment / aspect |
| --- | --- | ---: | ---: | --- | --- |
| Dragon Carapace | Armor | 13 | 49 | ACT_3 | Overheat Defense / 442 |
| Old Legionnaire Armour | Armor | 15 | 53 | ACT_3 | Damage Absorption / 512 |
| Samurai Armour | Armor | 37 | 140 | GATES_OF_SHADOWS | Damage Absorption / 1306 |
| Gabled Helm | Helm | 11 | 30 | ACT_2 | Rejuvenation / 366 |
| Dragon Helm | Helm | 13 | 33 | ACT_3 | Overheat Defense / 442 |
| Dragon Boomerangs | Ranged | 13 | 27 | ACT_3 | Overheat / 442 |
| Dragon's Breath | Magic | 13 | 44 | ACT_3 | Overheat / 442 |
| Lightning Arc | Magic | 39 | 139 | INTERMISSION | Enfeeble / 1388 |

Samurai Armour preserves the archive's unusual **HeadDefense=914 only** initial
snapshot through `initial_stats`; it does not silently gain BodyDefense or
UnarmedDamage. All other new initial values derive from vanilla milestones and
match the archive in a native parsed-attribute comparison. Gabled Helm and Samurai
Armour retain their archived placeholder icons. English text is copied faithfully,
including the typographic apostrophe in Dragon's Breath.

**Scope:** these are item-definition restorations using existing base move families.
They are not completed DE combat/preview parity. `AuditDE128EquipmentMoves.py`
compares every archived consumer of Chakram, MassBomb and LightningArrow with
canonical moves. It ignores package metadata, recovered attack IDs and the
single-choice Sound/RandomSound spelling difference, but reports actual structure
and attribute changes. The audit found outstanding changes such as Chakram's hit
reaction (High versus MiddleShortPlus), uninterrupt end (42 versus 40), preview
sound timing (18 versus 16), and MassBomb/LightningArrow's additional not-Stun
condition. MassBomb preview effects also differ. These need explicit Lua-authored
move changes and, where necessary, scoped typed patch APIs; do not call them done
merely because the item definitions now exist.

Five more spell definitions remain unregistered: Sphere1, Sphere2, Sphere3,
ComboSphere3 and MindThrowNormal. Their assets are available, but their archived
move consumers are absent by name from the base (9, 9, 5, 4 and 5 consumers,
respectively). Hidden NPC armor/helms and RifleBullet also remain out of shop
registration. The new audit preserves this distinction rather than treating all
missing items as ordinary purchasable equipment.

Verification:

- **2,370 foundation checks passed.** The added equipment fixture compares all
  eight identities, English names, category counts, art references, listing data,
  group/level gates, default enchantments and Samurai's stat exception against
  the archive. Missing module and missing model/icon failures roll back the whole
  package, including all equipment categories. Existing weapon, policy, reward,
  perk, initial-stat and compatibility tests remain active.
- `AuditDE128MissingEquipmentAssets.cs` executed read-only in the working Unity
  editor: all 13 missing non-hidden armor/helm/ranged/magic item model texts and
  icon sprites loaded, including the five spells still pending move support.
- Isolated Unity 6000.6.0f1 acceptance **passed, terminal exit 0**. The eight native
  definitions match every stat's value AND presence, levels, upgrade levels,
  prices, group labels, default perk/aspect and upgrade template. Models/icons
  load through the real asset provider. The ten restored weapon checks and live
  Jian four-hit acceptance also still pass. Evidence:
  `Temp/FormNative-qpsmkaqv/DE128Runs/Run-mz1gbhdy/de128-validation.log`.
- That log also contains the previously observed UnityEditor.Search startup
  indexing exception, before combat readiness. It is not a new game exception;
  the harness does not claim the entire editor log is clean.
- The public examples page now describes current restored content and ChineseSwords
  validation accurately. Wiki build passed: **47 pages, 4,047 links/assets**;
  the existing duplicate `/404` route warning remains.
- `git diff --check` passed, with line-ending conversion warnings. No managed
  engine source changed this step; the test harness compiled and ran in Unity.

Purchase/equip/save/reload UI flows, the restored equipment's actual combat and
visible notification behavior remain unverified. Next work should address shared
move differences and missing spell graphs, rather than equating more registered
items with complete gameplay parity. Full DE128 production remains active.

## Step 17 - guarded native move patches (0.13.0)

The generic C# API `sf2.moves.patch` now supports typed additional selection
conditions, one named interval-end change, one whole-attack hit-reaction change,
and one directly scheduled sound-frame change. Selectors are exact and frame/hit
changes require expected source values. Conflicts reject two patches for the same
move; native application validates the whole batch before changing any objects.
Teardown restores owned changes, including deferred interval nodes consumed during
native initialization. No DE identity or archive loader is added to the engine.
The public API, editor types, completions and snippet are updated together.

`shared_moves.lua` authors five archived changes:

- RangedHeavyPlayer: Uninterrupt ends at 40 rather than 42.
- ChakramFly: High becomes MiddleShortPlus.
- ShopRangedTryOnHeavyPlayer: snd_disk moves from frame 18 to 16.
- MassBombPlayer and LightningArrowPlayer: add the not-Stun selection condition.

The stronger native predicate test exposed an adapter mistake: constructing a
condition with ConditionsParser.Create does not initialize its base Not/Player
fields. The adapter now also calls Parse, as the native parser does. A rerun
verified both absent-Stun and present-Stun behavior.

Verification:

- 2,482 foundation checks passed: actual packaged Lua, archive comparisons,
  conflicts in both load orders, package rollback, invalid declarations and
  fingerprint distinctions.
- 41 move-patch checks passed using the production patch implementation with
  controlled native containers; 167 existing attack-authoring checks passed.
- Managed editor assembly build passed with zero errors (149 warnings).
- Isolated Unity 6000.6.0f1 acceptance passed, terminal exit 0. Five native patches,
  actual Stun predicates, native apply/init/rollback in both initialization orders,
  all restored equipment checks and the live Jian four-hit case passed. Evidence:
  `Temp/FormNative-qpsmkaqv/DE128Runs/Run-n8uf5rmq/de128-validation.log`.
  The previously recorded UnityEditor.Search startup exception remains; this is
  not a claim that the whole editor log is clean.
- Editor generation/check, 36 tests, LuaLS completions and real VS Code integration
  passed. Contracts contain 153 functions, 76 constants and 215 typed structures.
- Wiki build passed: 47 pages and 4,052 links/assets; the existing duplicate
  `/404` route warning remains. `git diff --check` passed.

Remaining work includes MassBomb preview effects and other presentation/profile
changes, plus the absent Sphere1/Sphere2/Sphere3/ComboSphere3/MindThrowNormal move
graphs. Purchase/equip/save/reload flows and actual contact/preview acceptance
remain open. These five changes do not establish full DE combat parity.

## Step 18 - spell graph dependencies and native effect authoring

This step starts the missing spell graphs rather than registering items with no
working moves. `Tools/AuditDE128SpellGraphs.py` now traces each family's complete
archived template closure and compares templates with canonical base data. It
records source hashes, exact move names, actions, condition tags and resource
source paths; run it with `--output Temp/DE128SpellGraphs.json` for full evidence.
It is an engineering audit only. The mod never loads archive XML.

| Family | Moves absent from base | Transitive templates | Templates absent from base |
| --- | ---: | ---: | --- |
| Sphere1 | 9 | 17 | Sphere1 |
| Sphere2 | 9 | 17 | Sphere2 |
| Sphere3 | 5 | 14 | Sphere3 |
| ComboSphere3 | 4 | 14 | Sphere3 |
| MindThrowNormal | 5 | 9 | None |

All other referenced templates are structurally equivalent after the audit's
stated normalization. All directly referenced animation binaries, effect atlas
metadata and sound files were found in Resources. This proves file availability,
not decoded animation, complete texture dependencies or rendered-effect success.
The report must not be mistaken for full native graph acceptance.

The first API gap is now implemented in generic C#: scheduled `effect`,
`stop_effect` and `stop_follow_effect` actions on `sf2.moves.register`. An immutable
typed effect contains its model-local name, core sequence, scale/time scale,
loop flag and optional following position. Actions retain the existing frame/event
scheduler. The adapter projects native Effect/StopEffect/StopFollowEffect nodes;
Lua authors no XML. Strict field/type/range validation and optional fingerprint
extensions preserve existing declarations while detecting every new field.
There is no DE-specific engine branch. The public wiki, editor schema/generated
contracts, nested completion test and reusable action snippet are updated.

Verification:

- `Tools/TestMoveEffects.ps1`: **620 combined checks passed**, including the prior
  attack/graph/presentation cases. Actual Lua is projected through the production
  adapter and compared with recovered native effect objects parsed from archived
  Sphere1 actions. Tests cover defaults, positions, looping, both scheduling
  forms, stop/detach names, invalid data, transaction rollback and fingerprints.
- `Tools/TestDE128Foundation.ps1`: **2,482 checks passed**; the existing DE package
  still loads and rolls back normally. Package version remains 0.13.0 because
  this step does not yet enable another spell.
- Managed editor build passed with **0 errors**, 2,739 existing warnings on the
  rebuilt dependency graph. The focused test's subsequent managed build was clean.
- Editor generate/check, **36 tests**, LuaLS including nested effect completion,
  and real VS Code integration passed. Contracts have 153 functions, 76 constants
  and 216 typed structures.
- Wiki build passed: **47 pages, 4,052 links/assets**. The existing duplicate 404
  warning remains. `git diff --check` passed with line-ending warnings.

No Unity playtest or visible effect rendering was performed for these new actions.
The tests verify recovered native parsing/scheduling, not a rendered projectile.
Next work is typed projectile actor creation/equipment inheritance and deletion,
charge consumption, and the missing graph conditions (including actor names and
bullet counts). MindThrow also needs scheduled shake/voice behavior. The move
flags, velocity and damage/defense requirements must be carried through when
translating the complete graphs. These gaps remain explicit; no simplified spell
has been substituted for the archived behavior. Full production stays active.

## Step 19 - scheduled projectile creation, charge and deletion

Generic move actions now expose the existing native child-weapon actor path:
`create_projectile` takes a model name, core skeleton and source equipment slot
(Weapon/Ranged/Magic), copying that equipment into the child Weapon slot. Optional
owned `start_move` handles are validated at commit; alternatively a core animation
name can select the initial move. `add_bullets` changes MagicBullet or
RaidChargeBullet through native handling, and `delete_actor` selects a native actor
for removal. Typed payloads reject unrelated fields and invalid values. All added
fields affect fingerprints without altering prior action representations.

Lua bindings, C# definitions and the native adapter changed together. These remain
generic engine capabilities: no DE spell names or archive loading enter production
C#. Wiki, editor contracts/completion and a reusable projectile snippet are updated.
The five complete DE spell graphs remain pending; package version stays 0.13.0.

Verification:

- **697 combined move checks passed**, including actual Lua, archive projection,
  recovered parsers where usable outside Unity, default/invalid declarations,
  owned move handles, scheduling, rollback and fingerprint changes.
- **2,482 DE128 foundation checks passed**. Managed editor build passed with
  zero errors (2,739 warnings when rebuilding dependencies).
- The standalone CreatePlayer parser requires Unity native calls, so its actual
  ItemInfo comparison was moved to the isolated Unity acceptance harness instead
  of replacing native behavior with a successful stub.
- Unity 6000.6.0f1 run **passed, terminal exit 0** at
  `Temp/FormNative-qpsmkaqv/DE128Runs/Run-nq0s7b4v/de128-validation.log`.
  Inert Lua fixture moves reached the real parser. Skeleton/equipment-copy records,
  actor name, owned initial move, charge and deletion scheduling matched. Existing
  restored-equipment, shared-patch and live Jian acceptance also passed.
- Editor generation/check, 36 tests, LuaLS nested projectile/charge completion and
  real VS Code integration passed (153 functions, 76 constants, 218 structures).
- Wiki build passed: 47 pages, 4,052 links/assets. Existing duplicate 404 warning
  remains. Native logs retain the previously recorded editor indexing limitation.

No live projectile creation/contact/deletion acceptance is claimed: the new fixture
moves were inspected, not selected. Next work remains bullet/actor-name selection
conditions, remaining move flags/velocity/damage details, then complete Lua spell
graph translation and live acceptance. Full DE128 production remains active.

## Step 20 - charge/name selection and projectile motion

Generic Lua move conditions now support `actor_name` (exact, case-sensitive native
actor identity) and `bullets` (inclusive MagicBullet/RaidChargeBullet bounds).
Both support native target selection and negation, and can appear in typed groups,
locks or other existing condition consumers. New enum entries are appended to
preserve previous fingerprint identities. Optional range payloads are immutable
and reject malformed or unrelated fields.

Moves now accept native velocity and acceleration on all three axes, the native
velocity-preservation flag and `no_magic_recharge`. These fields are move-only;
the recovered parser does not correctly inherit velocity from templates, so the
API does not promise unsupported template behavior. Optional fingerprints preserve
old declarations. No DE content policy or runtime archive reading was added.

Verification:

- **857 combined move checks passed**, including actual Lua projection and a native
  actor-name/charge predicate matrix (case sensitivity, both charge types, inclusive
  bounds and negation). Native velocity/acceleration parsing matches Sphere1;
  individual axes, preservation, validation and fingerprint changes are checked.
- **2,482 DE128 foundation checks passed**; managed editor build passed with zero
  errors (2,739 warnings on rebuilt dependencies).
- Isolated Unity 6000.6.0f1 **passed, terminal exit 0**, including real parsed
  conditions, velocity/acceleration, preservation and no-recharge flags, prior
  projectile parsing, equipment comparisons, shared patches and live Jian checks.
  Evidence: `Temp/FormNative-qpsmkaqv/DE128Runs/Run-8zc147wi/de128-validation.log`.
- Editor generation/check, 36 tests, LuaLS completion and real VS Code integration
  passed. Contracts now
  contain 153 functions, 76 constants and 221 typed structures. The API reference,
  authoring snippet and native fixture guide are updated in this step.
- Wiki build passed: 47 pages, 4,052 links/assets, with the existing duplicate 404
  warning. Whitespace checks passed, with line-ending conversion warnings.

The new native fixture moves remain inert, so this is not live projectile
trajectory/contact/recharge acceptance. All five spell item registrations remain
pending their complete graphs; package version stays 0.13.0. Next work needs
wall-distance cleanup conditions and attack options (defense contribution,
critical suppression, block/invulnerability exceptions and attack-effect flags),
then exact Lua spell graph authoring and live verification. Production remains active.

## Step 21 - spell attack options and wall-distance conditions

Generic move conditions now include typed `distance` declarations with independent
from/to points, signed X/Y or planar Full distance, inclusive bounds and negation.
The adapter retains implicit native point players and omits Axis for Full (writing
Full literally would incorrectly select Y in the recovered parser). This allows
Lua to author the archived projectile-past-wall cleanup predicate.

`attack.options` now supports native critical/hit-effect suppression, explicit
block bypass, Body/Head targeting, BodyDefense/HeadDefense terms and a list of
named invulnerability exceptions. Payloads are immutable, strict and fingerprinted;
omitted or empty options preserve existing attack fingerprints. The generic C#
implementation contains no DE names or runtime archive loader. Wiki, generated
editor contracts, completion checks and a spell-options snippet changed together.

Verification:

- **949 combined move checks passed** using actual packaged Lua bindings and the
  production adapter. Sphere1 attack flags, defense lists and exception lists
  match recovered IntervalAttack fields parsed from the archive. Wall cleanup
  projection and native ConditionDistance fields also match. Actual native signed
  predicates cover the inclusive boundary and negation on both X and Y.
- Invalid option types, duplicate/unsupported defenses, malformed exception lists,
  distance bounds/points and fingerprint changes are tested. Empty attack options
  retain the legacy declaration's fingerprint.
- **2,482 DE128 foundation checks passed**; managed editor build passed with zero
  errors (149 warnings on the successful incremental build).
- Editor generation/check, 36 tests, LuaLS nested distance/attack completion and
  real VS Code integration passed. Contracts contain 153 functions, 76 constants
  and 223 typed structures.
- Wiki build passed: 47 pages, 4,052 links/assets; the existing duplicate 404 warning
  remains. Whitespace checks passed with line-ending conversion warnings.

No new Unity playtest was performed in this step: these tests run the recovered
parsers/predicates without a live projectile. Damage/contact, trajectory, cleanup
and visual acceptance remain open. The next content step is complete Sphere1 Lua
graph authoring and native live acceptance. Later spell graphs also need their
additional hit reactions and MindThrow's edge-free attack path; those requirements
were identified directly in the archive and are not silently approximated. Package
version remains 0.13.0 until new spell content is integrated. Production stays active.

## Sphere1 integration checkpoint (in progress)

Version 0.14.0 adds Minor Charge of Darkness and nine Lua-authored Sphere1 combat
and shop moves, with three unchanged recovered animation binaries. No runtime XML
loader is used. Complete archive comparisons passed (1,004 combined move checks),
and 2,509 foundation checks passed.

The latest isolated Unity run, Run-cdsq9ulc, exited 1: native casting, inherited
projectile equipment, charge consumption and deletion were observed, but the
harness did not observe the middle-flight phase. This is an unresolved acceptance
failure, not a completed spell integration. Numerical damage, visible effects,
shop preview and purchase/save acceptance remain open. Four other missing spell
graphs also remain pending. This checkpoint is committed at the user's request.

## Sphere1 native lifecycle follow-up

The failed flight expectation was caused by the preceding Jian attack closing the
fighters' gap: the projectile hit during its damaging startup and executed its
Strike deletion action before reaching flight. The harness now restores long-range
spacing before casting and listens to native animation-selection events rather
than relying solely on editor-update polling. No production graph or engine
behavior was changed to force a pass.

Isolated Unity run `Run-kejilqi4` exited 0. It observed native Magic-input selection,
exactly one inherited-equipment child, Sphere1Start then Sphere1Middle, one consumed
charge and child deletion/removal. The prior Jian and restored-equipment checks also
passed. This resolves the preceding failed middle-flight check. Numerical damage,
wall-miss cleanup, audible/visual output, shop previews and purchase/save acceptance
remain open; this is not full spell parity. The mod continues to author its graph
in Lua and does not load archived XML at runtime.

Follow-up verification: 1,004 combined move checks and 2,509 foundation checks passed; wiki build passed (47 pages, 4,052 links/assets), with the existing duplicate-404 warning. The move checks require pwsh; the initial Windows PowerShell invocation rejected that unsupported host before running assertions. Whitespace checks passed.

## Step 23 - Sphere2 and Medium Charge of Darkness (0.15.0)

Added all nine archived Sphere2 moves as Lua declarations: cast, startup, flight,
wall cleanup, shop preview and try-on. The graph shares the existing three
unchanged fireball animation binaries. It retains three ordered attack edges,
600 impulse, ShroudInterval exception in both attacks, the casting Stun exclusion,
X/Z startup alignment, and distinct Energyball effect sequences/timing/positions.
No new API or production C# changes were required; the mod loads no archived XML.

Medium Charge of Darkness retains level 37, 127 gems, GATES_OF_SHADOWS,
MagicDamage 921 through native progression, and the Stun enchantment at aspect
1306. The production package now registers 20 moves and 20 equipment listings.

Verification: 1,004 combined move checks passed, including full normalized
comparison of all nine declarations and inherited templates against the archive,
actual native attack parsing, unchanged binary hashes and reload fingerprints.
The actual package passed 2,536 foundation checks including the new item/gates.
The wiki built 47 pages and validated 4,052 links/assets (existing duplicate-404
warning). Native Unity run Run-yedgzocr exited 0: Jian input/attack checks, all
restored equipment definitions/art, Sphere2 Magic-input selection, one child with
inherited equipment, startup-to-flight selection, charge consumption and deletion.

The first native attempt was stopped after the fixture's second sequential fight
was rejected. Both spells now have independent battles/modes, and the harness
fails immediately on rejected entry. `--spell Sphere1|Sphere2` selects the native
fixture and records the choice in the run evidence.

Numerical damage, wall-miss cleanup, audible/visual effects, shop-preview and
purchase/save acceptance remain open. Sphere3, ComboSphere3 and MindThrowNormal
still need complete graphs; their distinct reactions/edge-free attacks must not
be approximated. Production remains active.

Sphere1 regression with the updated package/harness also passed: Run-8n0gxe37 exited 0, retaining native input selection, startup/flight, charge and deletion evidence. No Unity assets or engine source were changed in this step.

## Step 24 - Sphere3 and native physical-fall reaction (0.16.0)

Sphere3's five archived moves are authored in Lua, with unchanged recovered
big_sphere_player and magic_fire_aura_bullet binaries. The graph preserves its
spawn frame 5, charge frame 7, enemy-centered startup, short startup range,
frame-22 middle attack, five ordered edges (including repeated edge names),
0.6 mixed damage, downward impulse -1000, effects and both shop actions.
Large Charge of Darkness retains level 42, 159 gems, INTERMISSION, MagicDamage
1076 through native progression and Lifesteal enchantment aspect 1511.

Generic C# attack and guarded hit-patch validation now accept `Physycal`, the
existing native physical-fall reaction spelling. The recovered parser passes the
name to hit conditions; vanilla PhysicalFall already recognizes that contract.
No recovered game code or Unity identities changed. All spell behavior remains
in Lua; XML is used only by authoring verification, never loaded by the mod.
Wiki and editor schema/generated definitions/completion coverage changed together.

Verification:
- 992 combined move checks passed: all five complete archived graphs, template
  equivalence, binary identity, native attack parsing, exact Physycal spelling,
  repeated edges, fingerprint change and rejection of unsupported spellings.
- 2,563 actual-package foundation checks passed. 45 controlled production
  move-patch checks passed, including Physycal apply/rollback before and after
  native-container initialization.
- Isolated Unity run Run-3mi0pvne exited 0: Jian input/attack checks, restored
  equipment/art checks, Sphere3 Magic-input selection, one inherited-equipment
  child, startup and middle selection, charge consumption and child removal.
- Editor generation/check, 36 tests, LuaLS (including Physycal completion) and
  real VS Code integration passed. Contracts remain 153 functions, 76 constants,
  223 typed structures. Managed compile passed with no errors.

The native test does not prove numerical damage, the victim's fall animation,
audible/visual effects, shop previews or purchase/save continuity. ComboSphere3
and MindThrowNormal remain incomplete and require their exact reactions and
edge-free attack support. Production remains active.

Wiki build passed: 47 pages and 4,052 links/assets, with the existing duplicate-404 warning. Whitespace checks passed.

## Step 25 - ComboSphere3 / Blast of the Void (0.17.0)

The four ComboSphere3 moves are Lua-authored and reuse the two unchanged Sphere3
animation binaries. Exact archive comparison retains cast MidFrames 1,
Uninterrupt end 34, spawn frame 15, charge frame 7, enemy-centered projectile,
parent-facing direction, frame-12–22 attack, five ordered edges (including
repetitions), 0.3 mixed damage and impulse (-1,-600,0). Strike and animation-end
both retain their deletion actions. The two shop moves retain their own effects,
sound frame and TryOnEnd action. There is no middle-flight move to fabricate.

Generic C# attack and guarded hit-patch validation now accept the existing native
HighLong reaction (also used by vanilla HighBlockHeavy). Wiki, editor schema,
generated contracts and LuaLS completion changed together. Blast of the Void
retains level 27, 82 gems, ACT_5, MagicDamage 659 through native progression and
Enfeeble aspect 944. The archived unknown-magic icon is deliberately preserved.
All mod behavior is Lua; no runtime XML loading was introduced.

Verification:
- 988 combined move checks pass: complete four-move archive/template equivalence,
  binary identity, native attack/reaction parsing, repeated edges, fingerprint
  sensitivity and rejected unsupported reaction spellings.
- 2,590 actual-package foundation checks and 49 production patch/rollback checks
  pass, including HighLong before/after initialization.
- Isolated Unity Run-4scal437 exited 0: prior Jian acceptance, restored equipment
  and art, native Magic-input selection, one inherited-equipment child, the
  actual startup attack phase, charge consumption and child deletion/removal.
- Editor generation/check, 36 tests, LuaLS and real VS Code integration pass.
  Contracts remain 153 functions, 76 constants and 223 typed structures.

Numerical damage, victim hit reaction, visual/audio output, shop previews and
purchase/save continuity remain open. MindThrowNormal is the remaining missing
spell graph and requires additional reaction, action and edge-free attack support.
Production remains active; no full parity claim is made.

Wiki build passed (47 pages, 4,052 links/assets; existing duplicate-404 warning). Managed compilation and whitespace checks passed. No Unity assets changed.

## Step 26 - MindThrow runtime primitives and new source authority

Added generic typed support for explicit edge-free attacks (`direct = true`),
NoReaction, scheduled voice-filtered sound actions, and scheduled camera shakes.
Direct mode omits AttackingParts entirely; normal attacks still require 1–64
edges. The recovered ModelCollision path applies one direct collision per interval
and resets normally. Missing edges cannot silently opt into direct damage.
Voice names are strictly Male/MaleLow/Female, or omitted for unfiltered playback.
Shake payloads preserve native frame counts and bounded finite camera values.
New payloads are immutable and fingerprinted; omitted defaults preserve legacy
fingerprints. No DE policy was added to engine code.

The existing historical MindThrowPlayer2Normal declaration supplied the direct
attack and shake comparison, and MindThrowPlayerNormal supplied voice filtering.
1,046 combined checks pass against actual native parsers/collision logic, with
invalid payload, kind exclusivity, voice filtering, timing and fingerprint tests.
53 controlled patch/rollback checks and 2,590 package foundation checks pass.
Managed editor compilation passes (zero errors; existing warnings). Editor
contracts/docs include the new nested payloads and NoReaction completion.

MindThrow's full graph is not yet registered: its owned victim reaction, reversed
impulse direction and linked follow-up remain to be implemented/verified. No new
Unity fight, audible sound or visible shake acceptance is claimed in this step.
Package version remains 0.17.0 until the content is integrated.

During this step, the owner supplied a complete DE corpus and required all DE
content to come from it, including updates to Lua for changed XML. That archive
supersedes repository DExml as authority. Download confirmation returned Google
Drive Quota exceeded HTML, not archive bytes. Acquisition/reconciliation is pending
an accessible download or local path; see SOURCE_CORPUS.md. Previously missing
assets must be reassessed once the corpus is available. This also prevents claiming
that the earlier restored content already matches the new source.

Editor generation/check, 36 tests, LuaLS nested completion and real VS Code integration passed (153 functions, 76 constants, 225 structures). Wiki build passed: 47 pages, 4,052 links/assets, with the existing duplicate-404 warning. Whitespace checks passed.

## Step 27 - corpus reconciliation tooling (acquisition still pending)

Added a read-only full-file SHA-256 inventory and comparison tool for the newly
mandated corpus. Historical XML is compared by unambiguous relative suffix;
bundled mod assets are checked against unambiguous source filenames. Reports
include complete positional XML differences, added XML, missing/ambiguous matches
and binary mismatches. XML indentation/attribute order is ignored while meaningful
text, child order and repeated entries are preserved. The tool neither edits Lua
nor imports Unity assets; input links/junctions and overwriting reports are rejected.

Eight controlled-fixture tests passed, covering changed/formatting-only XML,
malformed/entity XML, repeated elements, binary identity/mismatch, ambiguity,
explicit subtree selection, deterministic inventory and report preservation.
The actual corpus has not been audited: a fresh confirmed Drive download still
returned 2,009-byte Quota exceeded HTML without a 7z signature. No new DE content
or parity assertion was made from the historical source. An accessible archive or
local path remains necessary for the owner's reconciliation and asset-expansion
request. Source instructions and the audit command are in SOURCE_CORPUS.md.

## Step 28 - owned hit reactions and impulse facing

The owner deferred corpus acquisition and authorized continued work with the
historical repository data. Reconciliation with the complete archive remains
pending; no newly available assets are assumed.

Added generic C# runtime support for `attack.hit_move`, a typed Move handle,
and `direction = { impulse = { reverse = true } }`. Owned reactions preserve
namespaced move identity through native attack and hit-event parsing. Commit
validation rejects missing/inaccessible targets atomically. Core `hit` and
`hit_move` are mutually exclusive, as are point-based and impulse-based facing.
Both additions participate in save fingerprints without changing the encoding
of existing definitions. Lua definitions, editor completion and the public wiki
describe the implemented contracts.

Verification: 1,074 combined reaction checks passed, including native impulse
direction for negative/zero/positive impulses, archive comparison, malformed
payloads, fingerprints and transaction rollback. All 2,590 package foundation
checks passed. Managed editor compilation passed with existing warnings.
Editor generation/check, 36 tests, LuaLS and real VS Code integration passed
(153 functions, 76 constants, 226 structures). Wiki build passed: 47 pages and
4,055 links/assets, with the existing duplicate-404 warning. Whitespace checks
passed. No Unity playtest or visible victim-animation acceptance was performed.

MindThrow also depends on its intrinsic perk's animation-event/ModFlag handoff,
in addition to five spell moves and the owned victim reaction. That procedural
handoff still needs generic runtime capabilities and Lua implementation before
the full spell can be registered faithfully. Package version remains 0.17.0;
this step does not claim complete MindThrow or DE parity.

## Step 29 - native animation lifecycle callbacks

Added `on_animation_start` and `on_animation_end` for Lua behaviors. Generic C#
hooks run after native perk notification during active combat. Immutable event
data records the exact animation name, simulation frame and actor relationship
(`self`, `opponent`, or `other`, including projectiles) for each recipient. This
avoids polling the current animation and missing transitions. No DE spell policy
was added to the engine. Nested events queue in order; round changes and bounded
queue/cascade limits prevent stale or unbounded dispatch. Public callback docs,
editor schemas and LuaLS completion were updated together.

Verification:
- 12 production-dispatch checks cover nested FIFO order, both perspectives,
  detached event data, subscription/lifecycle guards, round changes and limits.
- 221 battle-rule checks pass through actual MoonSharp bindings, including both
  callback payloads, instance forwarding, missing/mismatched observations and
  subscription disposal. The older showcase fixture now extracts the current
  move point/presentation helpers needed by its production projection methods.
- FightBegin source/provenance/reentry checks and all 2,590 DE128 foundation
  checks pass. Managed editor compilation passed with zero errors and existing
  warnings.
- Isolated Unity 6000.6 run `Run-8b7olbuw` exited 0. Actual Lua rules received
  Sphere1 caster start/end on both sides and projectile animation starts, with
  correct actor relationships. Jian and Sphere1 native combat acceptance also
  remained green. No new numerical damage, audio or shop-preview claim.
- Editor generation/check, 36 tests, LuaLS and real VS Code integration pass:
  155 functions/aliases/callbacks, 76 constants, 227 structures. Wiki build passed
  47 pages and 4,063 links/assets, with the existing duplicate-404 warning.

The native ModFlag/ModExpires handoff still needs an owner-scoped fighter
capability before MindThrow's innate Lua behavior and full spell can ship.
Package version remains 0.17.0. Corpus acquisition/reconciliation remains deferred
at the owner's request; production continues against historical repository data.

## Step 30 - instance-owned native combat flags

Added `fighter:set_flag`, `fighter:has_flag` and `fighter:clear_flag` behind
`combat.effects`. The typed C# bridge creates native modifier containers owned
by the behavior and its equipped/rule instance. Lua decides when to set/clear;
the recovered modifier path maintains ModExists observations, removal history,
ModExpires notification, round reset and form-transfer behavior. No mod XML,
native-trigger DSL or DE-specific engine policy was introduced.

Repeated sets are idempotent. Clearing an absent key emits no expiry. Qualified
names preserve behavior identity and fit native move selectors; one instance
cannot remove another's flags. Operations expire at callback return and require
an active registered fighter. Limits are 64 owners per fighter registration and
64 active flags per owner. Flags are combat state, not profile state. Documentation
and editor contracts explain names, limits, returns and selector behavior.

Verification:
- 268 battle-rule checks pass through current MoonSharp bindings, including
  flag instance isolation, repeated calls, malformed keys, capability denial and
  expired references. All 2,590 actual-package foundation checks pass.
- 160 existing production flag/variable lifetime and form-transfer checks pass.
  These exercise native modifier copying, reset/expiry, namespace tracking and
  transfer; they are controlled fixtures, not a new transformed-fighter playtest.
- Isolated Unity run `Run-5bgiel3n` exited 0: real Lua created exactly one native
  flag, ModExists observed it, clearing removed it and emitted/recorded exactly
  one ModExpires event. A repeated clear emitted none. Jian, Sphere1 and animation
  lifecycle acceptance remained green. The preceding failed run exposed a missing
  Parse call in the test's native condition probe; the corrected probe passed.
- Managed editor compilation passed. Editor generation/check, 36 tests, LuaLS
  and real VS Code integration passed (158 functions/aliases/callbacks, 76 constants,
  227 structures). Wiki build passed 47 pages and 4,075 links/assets, with the
  existing duplicate-404 warning. Whitespace checks passed.

MindThrow's six move definitions, innate Lua perk, item and full native spell
acceptance are still to be integrated. This step verifies the flag bridge, not
MindThrow damage or follow-up selection. Package version remains 0.17.0.

## Step 31 - MindThrowNormal Lua graph and innate behavior

Version 0.18.0 supplies all six historical moves in `content/mind_throw.lua`,
including the owned victim reaction and final direct attack. The three native
perk triggers are implemented as Lua decisions in `content/mind_throw_perk.lua`:
casting sets an instance-owned pending flag; an opposing victim reaction or the
wall move clears it. Native ModExpires selects the follow-up only while the
opponent is playing the owned victim reaction. The generic C# APIs from the prior
steps are reused unchanged; DE policy stays in Lua, with no runtime XML loading.

Mind Throw is a level-45, ACT_7_1 listing costing 181 gems. Its native projected
damage, upgrade level, Frenzy enchantment/aspect, icon and model match historical
`MAGIC_MIND_THROW_NORMAL`. Five binaries are copied unchanged from recovered
Resources: `mind_throw_1_normal`, `mind_throw_2_normal`, `mind_suffocation`,
`mind_suffocation_start` and `mind_suffocation_middle`. The designated complete
corpus remains deferred, so these sources still require reconciliation.

Verification:
- 984 combined Lua/native declaration checks compare all six moves and recursive
  core template dependencies to the historical XML, normalize only native-equivalent
  defaults/float precision, verify binary hashes and check deterministic fingerprints.
- 2,638 foundation checks execute the actual package, including equipment values,
  innate loadout, rollback/rebuild and MindThrow flag trigger traces.
- Unity 6000.6 isolated run `Run-g4ttg7td` exited 0. Native Magic input selected the
  cast, created one inherited-equipment projectile, consumed charge, hit a grounded
  target in the normal idle stance, selected the owned victim reaction, expired
  the innate flag exactly once, selected the caster follow-up and reached its
  final frame-48 direct attack interval. Existing Jian acceptance stayed green.
- Separate wall-miss run `Run-f91mb12c` exited 0: the projectile selected its wall
  move, deleted itself, cleared the innate flag exactly once and did not select
  a caster follow-up. Use `--spell MindThrowNormal --mindthrow-wall-miss` to repeat.
  Final harness contact rerun `Run-a346vpq7` also exited 0.
- The wiki builds 47 pages with 4,075 checked local links/assets, zero Astro
  diagnostics and the existing duplicate-404 warning. Whitespace checks pass.
  Unity compiled the native harness; no engine/API source changed in this step.

Earlier native attempts are retained as diagnostic evidence. The recovered
projectile passes above `FistsStartStanceIdle-Left`; it reaches wall cleanup with
one flag expiry. Jumping into it selects the native priority-600 PhysicalFall
because the jump is Unstable, overriding the priority-500 MindThrow reaction.
A short ordinary movement input leaves the initial crouched stance for normal
`StanceIdle`, where contact and follow-up pass. No production geometry, damage,
priority or timing was changed to satisfy the fixture. The initial cast is
interrupted by the successful follow-up, so no natural AnimationEnd is expected
for that cast (the lifecycle callback contract excludes interruptions).

This establishes the native graph/behavior handoff, not complete gameplay parity.
Numerical damage, other stances, visual/audio output, shop preview, purchase/equip
and save/reload still need acceptance. The user's running editor and save were
not used by the isolated checks.

## Step 32: opponent enchantment settings and pending young Lynx

The generic C# warrior API now accepts a mixed dense array of bare perk handles
and typed `{ perk, aspect?, chance_factor? }` rows. Settings are restricted to
core native perks, finite and bounded (Aspect 0–2147483647, ChanceFactor
0–10000). Each warrior projects its own native `<Set>` overrides. No shared
perk or item definition is changed. Owned Lua behaviors retain their own
configuration contract. Duplicate IDs, sparse arrays, malformed rows and
unknown fields fail registration. Configured settings enter the content
fingerprint; bare handles and empty settings retain the old representation.

The pending Lua module `scripts/content/sensei_act_one_opponents.lua` ports both
normal and eclipse young Lynx from ZONE_1. It preserves the four ordered innate
enchantments, Aspect 100000, ChanceFactor 2.69/2.75, attributes, shurikens,
portrait/name/tactic and the normal source's repeated alignment row. It is not
required by `main.lua`; the shipped package remains 0.18.0. Native test fixtures
copy this single authored module under fixture ownership, not into the active
DE entrypoint. Neither full story activation nor an extra shipped fight is claimed.

Evidence and checks:

- `pwsh -NoProfile -File Tools/TestWarriorPerkLoadouts.ps1`: 71 checks using actual
  MoonSharp and the production adapter. Complete historical opponent projections
  match (apart from the runtime character ID and explicit default perk Level 1).
  Checks cover invalid input/rollback, mixed rows, zero/default inheritance,
  core immutability, number formatting, determinism and fingerprint changes.
  Evidence: `Temp/WarriorPerks-44a2d76b59c446f5b2844511ffba125c`.
- `Tools/TestDE128Foundation.ps1`: all 2638 existing package checks pass.
- Managed editor build passes with zero errors. Isolated Unity native run
  `Temp/FormNative-qpsmkaqv/DE128Runs/Run-edhd8hcy` exits 0. The actual native
  `PerkInfoItem.Clone` path consumes all eight projected perk instances and
  preserves default inheritance, explicit zero and isolation between clones.
  Existing Jian and MindThrow contact/follow-up acceptance also passes.
- Editor schema/generated definitions, editor guide and public wiki are updated.
  Generation/check, project tests, LuaLS (including nested warrior perk
  completion) and VS Code integration pass. The VS Code runner was invoked
  directly with Code.exe after npm's Windows argument escaping mangled its path.
  Wiki builds 47 pages and checks 4075 local links/assets with zero Astro
  diagnostics; the pre-existing duplicate-404 warning remains.

Act I is still incomplete. Its Savage and Philosopher opponents reference
`Guard_Girl` and `Guard_Man`, with no matching template definitions found in
available archived DE or vanilla XML. No replacement templates or assets were
invented. Story availability/quest progression, rule conditions, native XP/reward
semantics, portraits, other opponents and live encounter/save acceptance need
implementation or verification before activation. The owner-supplied corpus is
still deferred; the historical XML comparison must be reconciled when it becomes
available. This step validates native perk consumption, not young Lynx combat
balance or complete Sensei's Story parity.

## Step 33: native reward economy and all Sensei reward slots

The generic C# reward definition/binding/adapter now supports `experience`
(integer 0–1000000, default 0) and nullable `prize_base` (finite 0–1000000).
Experience is an absolute count. PrizeBase is the recovered performance-bonus
base, not a fixed coin grant: denomination scaling and native performance
coefficients still apply. Positive values select the reward's base; zero and
omission retain their distinct native representations/fallback handling.
Existing item/choice/gem rewards retain their projection and fingerprint when
the new fields are unused. Configured values enter the content fingerprint.
No new API writes directly to profile currency or experience.

Pending `scripts/content/sensei_rewards.lua` authors all 57 slots across six
normal/eclipsed story acts: 17 two-slot normal fights and six gauntlets with
four slots each except Act VI's three slots. It preserves partial-gauntlet
results, the normal two-point experience awards, archived gem amounts and bases
1/5/100/100/500/2000 (normal) and 1 (eclipse). The module returns ordered reward
handle tables for future fight composition. It uses no XML at runtime and is
not loaded by the DE entrypoint; the package remains 0.18.0.

Verification:

- `Tools/TestSenseiRewards.ps1`: 727 checks through actual Lua registration,
  production projection and recovered Reward/RewardPrize parsing at two scales.
  All 57 slots match historical XML. Missing Money/Exp/Bonus are compared using
  their native zero defaults; unknown attributes, nested rewards or nonzero
  unsupported fixed money fail the oracle. Bounds, invalid values/rollback,
  locale formatting, default compatibility and fingerprints are also checked.
  Evidence: `Temp/SenseiRewards-6ecee72348bd469c8bb279fc721eea41`.
- Isolated Unity 6000.6.0f1 run
  `Temp/FormNative-qpsmkaqv/DE128Runs/Run-zz8iypk0` exits 0. All 57 projected
  rewards match historical-source rewards through actual native FightResult
  calculations at scales 0 and 2 with nonzero performance statistics. Experience,
  gems, performance coins and the selected bonus base match. The results are
  detached and are not awarded to a profile. Existing young Lynx perk, Jian
  input and MindThrow contact/follow-up checks remain green. The editor's Search
  index startup exception also occurs in the prior run; it is not a clean-log
  claim or an observed reward failure.
- All 2638 actual-package foundation and 71 warrior-loadout checks pass. Managed
  editor build succeeds with zero errors (existing warnings remain).
- Public wiki, editor schema/generated definitions and editor guide are updated.
  Generation/check, project tests, LuaLS nested-field completion and the VS Code
  integration runner pass. Wiki builds 47 pages with 4075 checked local
  links/assets and zero Astro diagnostics; existing duplicate-404 warning remains.

The reward representation gap is closed. Fight assembly, story progression,
missing guard templates, visuals and complete profile settlement/save/reload
acceptance remain open. Native result comparisons do not establish end-to-end
story rewards or exact asset-corpus parity. Acquisition/reconciliation of the
owner's complete archive remains deferred.

## Step 34: saved fight queries and six-act unlock decisions

Added generic `sf2.profile.fight(fight)` under `profile.read`. Qualified strings
respect dependencies; handles must belong to the script context. The host resolves
the active content graph, reads the bound profile's existing record and returns
detached `present`, `wins`, `losses` values. A known fight without a record returns
false/zero/zero without creating a record. Unknown fights and unavailable profiles
raise errors. Counts match the native WinCount/LossCount quest values, not the
separate eclipse counters or round wins. Host teardown clears the new service.

The required native methods were given narrowly scoped inferred names:
`Roster.FindSavedFightRecord`, `RosterFight.GetWinCount` and `GetLossCount`, with
the exact best-guess comment. Their implementations and save field names are
unchanged; existing call sites were updated. The evidence is the non-creating
roster lookup and getters/setters backed by CompletedCount/LossCount, plus the
quest-condition consumers. No matching serialized references or name collisions
were found. These guesses are not confirmed deobfuscation mappings.

Pending `scripts/content/sensei_progression.lua` computes acts eligible to unlock
after any victory. Each act requires its third tournament fight; acts II–VI also
require the previous act's final normal fight. Already opened acts and non-win
outcomes produce no notification candidates. Saved prerequisites earned before
the current event count, matching the archive. Callers supply boolean opened
flags and the five prior-act final fight identities. This is ordinary Lua logic,
not an XML interpreter or an operation DSL. The module is outside the entrypoint.

Verification:

- `Tools/TestFightProgress.ps1` passes 2119 checks through actual Lua bindings and the authored
  module against all 2048 combinations of six tournament/five previous-act wins.
  Expected gates come from historical quest conditions; the fixture also checks
  their event, outcome and opened-flag conditions. It verifies no duplicate
  candidates for opened acts, loss/surrender/timeouts, detached tables, missing
  records, ownership/capability errors and host cleanup.
  Evidence: `Temp/FightProgress-6fa7fbebbb9a44828cd0ab3fcb406a5a`.
- `Tools/TestProfileApi.ps1`: existing 18 production-method and 57 Lua profile
  tests pass; the Phase 1 showcase runtime also passes. All 2638 DE128 package
  foundation checks pass. Managed editor build succeeds (existing warnings remain).
- Isolated Unity 6000.6.0f1 run
  `Temp/FormNative-qpsmkaqv/DE128Runs/Run-siw3k4tu` exits 0. The production query
  reads an actual native roster, creates no absent records, distinguishes saved
  normal counts from different eclipse counts, does not mutate save XML, refreshes
  values between reads and rejects missing fights/unbound profiles. Fixture
  records and the original profile binding are restored in finally. Existing
  young Lynx perk, 57 reward-slot and Jian/MindThrow checks remain green.
- Wiki/API and generated editor contracts cover all 159 functions, 76 constants
  and 229 structures. Editor generation/check, project tests, LuaLS snapshot
  completion and VS Code integration pass. Wiki builds 47 pages and checks 4079
  links/assets; zero Astro diagnostics, existing duplicate-404 warning remains.

No map lock, notification or persistent opened flag is changed yet. Those need a
Lua presentation/progression coordinator that preserves native dialog ordering
and profile lifetime. Missing guard templates, fight assembly, full story
playback/save/reload and corpus reconciliation remain open. Package stays 0.18.0.

## Step 35: owned map battle lock updates

Added generic `sf2.battles.set_locked(battle, locked)` under `story.progression`.
The Lua binding accepts only this script's own registered battle handle and a
strict boolean. It rejects forged/foreign identities, missing capabilities and
UI cleanup calls. The C# host requires a bound profile and an active, unblocked
map; settlement, scene navigation and pending encounter preparation refuse the
change. Missing saved entries return false without creating them. Existing
entries change only Locked, refresh native map presentation and request a normal
save. An identical request returns true without rebuilding or saving again.
This is not an initial reveal/hide or replay-reset API.

Native map verification exposed a pre-existing refresh defect: MapPanel.Clear
cleared the scroll registry but left the old zone objects and buttons active.
It now uses BaseScrollContent.Clear to deactivate and destroy those objects as
well. The fix is shared C# runtime behavior, with no DE-specific branch.

The used roster methods have narrowly inferred names and the required best-guess
comments: GetSavedBattles, GetBattleId, IsLocked and SetLocked. Their existing
callers were updated; save attribute names and method behavior are unchanged.
The evidence is their roster list, FightIDS and Locked-backed implementations
and native quest/map consumers. These are not confirmed recovery mappings.

Verification:

- `Tools/TestBattleLocks.ps1`: 22 actual MoonSharp checks covering lock/unlock,
  refused changes, strict argument types, handle/capability rejection, registration
  rollback and cleared host service. Evidence:
  `Temp/BattleLocks-7bfe3e90cf45485fad355e0db25f0a66`.
- Isolated Unity 6000.6.0f1 run
  `Temp/FormNative-qpsmkaqv/DE128Runs/Run-ts3hf7dl` exits 0. It verifies native
  map unlock/relock and button state, unrelated saved fields preserved, identical
  requests unchanged, input/settlement guards, and obsolete zones inactive after
  refresh. Existing roster-query, young Lynx perk, 57 reward-slot, Jian input and
  MindThrow contact/follow-up checks also pass. This is not a full campaign or
  save/reload acceptance run, nor a clean Unity log claim.
- `Tools/TestEclipseRuntime.ps1`: 819 assertions and 21 replay segments pass;
  all 2638 DE128 foundation checks pass. Managed editor build succeeds with zero
  errors; existing warnings remain.
- Underworld runtime checks pass 1282 assertions. The asset audit exits 0 but
  still reports missing raid image references; this is not complete asset parity.
- Wiki and editor contracts cover 160 functions, 76 constants and 229 structures.
  Generation/check, project tests, LuaLS and VS Code integration pass. Wiki builds
  47 pages with 4083 checked links/assets and zero Astro diagnostics; the existing
  duplicate-404 warning remains.

The isolated native fixture suppresses the welcome tutorial using the existing
quest API and creates a controlled revealed battle with replay count 7. Initial
attempts encountered the tutorial's input block; forcing its continuation outside
the dojo is not a valid campaign test. The final fixture tests map lock changes
and existing combat acceptance, not tutorial completion or Lua initial revelation.
Only the isolated acceptance profile is written; owner saves are untouched.

The pending Sensei Lua coordinator has not been activated. Initial revelation,
notification ordering, persistent opened flags, missing guard templates, encounter
assembly and full story/save/reload acceptance remain open. Package remains
0.18.0; the owner's full asset archive remains deferred.

## Step 36: initial map revelation and six-act map effects

Historical notification actions show that the first act must reveal all six
entries, unlock Act I and focus it, after its dialog and Eclipse-mode transition.
Later acts unlock/focus their own entry. Lock mutation alone could not express
initial revelation, so the generic C# runtime now exposes `sf2.battles.reveal`
and `sf2.battles.focus` under `story.progression`. Both require an owned handle
and share the profile, scene, settlement, input and cleanup guards from lock
updates. Reveal's strict boolean sets the initial lock only; retries preserve
existing lock/hidden/replay fields and fight history. Focus requires a visible
entry in the current map mode and updates normal native selection/save focus.
Neither API starts fights, switches modes or overrides native fight conditions.

The native test found that immediate map selection after a rebuild read the old
selected zone until the following Update. Focus now flushes canvas layout and
MapPanel publishes selection synchronously for zero-duration scrolling, before
MapScene consumes it. Animated selection keeps its existing update path. Native
saved focus is written explicitly, including visible locked entries without an
eligible current fight. This is reusable C# behavior rather than DE policy.

Pending `scripts/content/sensei_map.lua` is ordinary Lua calling these APIs, with
no XML loading or operation interpreter. It reveals six entries only for Act I,
then unlocks/focuses the requested act. It returns false at the first refusal;
idempotent reveal allows retry after a partial sequence. The future coordinator
must finish presentation and Act I's Eclipse-mode transition before calling it,
and persist opened flags only on success. It remains outside the entrypoint.

New engine uses prompted narrowly inferred names: Roster.AddBattle (its existing
overloads), SetMapFocus and SetRaidMapFocus, and Battle.IsMapVisible. Evidence is
their native roster/MapFocus/RaidMapFocus backing and map display consumers.
Declarations have the required best-guess comments. Only Battle references to the
shared obfuscated visibility token changed; ItemInfo and other unrelated types
retain their symbols. No matching serialized asset references were found; Unity
GUIDs and save field names are unchanged. These are not confirmed recovery maps.

Verification:

- `Tools/TestBattleLocks.ps1`: 54 actual MoonSharp binding/capability/ownership,
  strict argument, rejection and host-clear checks.
- `Tools/TestSenseiMap.ps1`: 132 checks executing the authored module. All six
  acts match archived map action order/states; every first-act failure position
  stops and retries successfully. Later acts cannot fabricate initial entries.
  These use controlled host services, not story playback. Combined evidence:
  `Temp/BattleLocks-8700aefe2a374a5fa5833e2f3da2054d`.
- Isolated Unity 6000.6.0f1 run
  `Temp/FormNative-qpsmkaqv/DE128Runs/Run-wk2vo31i` exits 0. New native acceptance
  covers a fresh saved entry and zone/button, map/input guards, absent entry
  refusal, no duplicates or lock/replay reset, hidden-state preservation,
  immediate native selection and saved focus, and RosterBattle serialization
  roundtrip. Earlier lock, saved fight query, reward-slot, perk, Jian and MindThrow
  checks remain green. Only the isolated acceptance profile is used. No full
  profile reload, raid-focus playthrough, complete campaign or clean-log claim.
- Managed editor build passes; 819 Eclipse assertions across 21 replay segments,
  2638 DE128 foundation checks and 1282 Underworld checks pass. The Underworld
  audit exits 0 while retaining its missing raid-image findings.
- Wiki and generated editor contracts cover 162 functions, 76 constants and 229
  structures. Generation/check, project tests, LuaLS and VS Code integration pass.
  Wiki builds 47 pages and checks 4091 local links/assets, with zero Astro
  diagnostics and the existing duplicate-404 warning.

Package stays 0.18.0. Notification presentation, safe Eclipse-mode switching,
opened-state persistence, battle/encounter assembly, missing guard templates and
full story/save/reload acceptance remain open. Complete asset-corpus acquisition
and reconciliation remain deferred at the owner's request.

## Step 37 — Pending Sensei notification coordinator (2026-09-20)

`scripts/content/sensei_notifications.lua` now joins the pending eligibility and
map modules. It queues eligible acts after victories, presents the first pending
act on map entry, and saves twelve mod-owned boolean fields (pending/opened for
each act). Back and OK acknowledge the same sequence. Act I requests normal
mode before revealing/unlocking/focusing; refusal preserves the open dialog and
pending flag. An interrupted scene/profile leaves the notification pending.
Opened flags are written only after the map sequence succeeds. Repeated wins
do not duplicate completed notifications.

`sensei_notification_text.lua` authors the 56 existing translations of
characterSensei and Sensei_remembers0/1/2 from historical DE localization XML.
The coordinator references the existing shared core Sensei portrait. No XML is
loaded by the mod. This content still awaits reconciliation with the complete
owner-supplied corpus, whose acquisition remains deferred.

Reusable C# runtime support:

- `sf2.profile.set_eclipse_mode(boolean)`, requiring `story.progression`, requests
  the active native map-button quest, including tint/button/replay/tutorial policy.
  It refuses unavailable or blocked story maps and missing switches. A yielded
  native action returns false until the requested mode and map are ready; false
  does not imply that no part of the native transition ran.
- `sf2.ui.open { on_back = function(view) ... end }` replaces default Back close
  for the input-owning menu/modal. It can retain the dialog for retry, is bounded
  and non-reentrant, and never runs for scene/profile cleanup. `on_close` remains
  cleanup and cannot request the progression mutation. Views without on_back
  retain their previous behavior.

The new roster query uses inferred `Roster.IsEclipseMode`, backed by the same
boolean as existing SetEclipseMode and native Eclipse quest conditions. Its
declaration carries `// best guess for name`; owning callers and test stubs were
updated. No old-token serialized references remain, no save fields or GUIDs
changed, and no confirmed deobfuscation mapping was added.

Verification:

- `TestSenseiNotifications.ps1`: 114 actual Lua checks, including all 56 source
  translations, six-act order, win prerequisites, Back, blocked input, refusal and
  retry, scene cleanup, distinct profiles, pending and completed state XML
  roundtrips. Host progression and portrait metadata are controlled fixtures.
- `TestSenseiMap.ps1`: 67 binding checks plus 132 archived map-sequence checks.
- `TestModUiRuntime.ps1`: 152 UI model checks plus native bridge source contracts.
  `TestModUiLua.ps1`: 864 checks including real Lua Back lifetime/budget and cleanup guards. Its
  previously stale PNG fixture now uses explicit sprite descriptors.
- Isolated Unity 6000.6.0f1 `Temp/FormNative-qpsmkaqv/DE128Runs/Run-sa4746z5`
  exits 0: missing/blocked mode switch refusal, actual native mode-off quest,
  replacement switch, white map tint and repeat request. Existing map, rewards,
  saved-fight queries and Jian/MindThrow combat checks remain green. Only the
  isolated acceptance profile is touched; no clean-log or complete-story claim.
- Managed editor build passes; 819 Eclipse assertions/21 segments, 2638 DE128
  foundation checks and 1282 Underworld assertions pass. Underworld audit exits
  0 with its existing missing raid-image findings.
- Wiki/editor contracts cover 164 functions/aliases/callbacks, 76 constants and
  229 structures. Generate/check, 37 project tests, LuaLS and real VS Code
  integration pass. Wiki builds 47 pages with 4099 links/assets checked, no Astro
  diagnostics and the existing duplicate-404 warning.

Package remains 0.18.0; neither new module is loaded by main.lua. The eventual
encounter assembler must call install once with six battles and five prior-act
final fights, and merge its state fields if another DE schema is added first.
Native multilingual dialog layout, complete profile reload/story playback,
encounter assembly and missing guard templates remain acceptance/production work.

## Step 38 — Six young boss loadouts and missing perk parameters (2026-09-20)

Encounter assembly exposed settings unavailable through the existing warrior
API: explicit `Chance` values on several boss enchantments and Hermit's `Frames`
override. These cannot be replaced by ChanceFactor without changing semantics.
The generic C# `WarriorPerkDefinition` and Lua binding now accept optional `chance`
(finite 0..1) and `frames` (integer 0..2,147,483,647). The existing core-perk-only
constraint still applies. Owned procedural perks remain Lua behaviors.

The production adapter projects supplied fields into that warrior's native perk
clone; omission inherits defaults and zero is explicit. Fingerprinting includes
each new field's presence/value. Existing entries without these fields retain
their previous encoding, avoiding an unrelated saved-content identity change.

New pending `scripts/content/sensei_boss_opponents.lua` registers Hermit, Butcher,
Wasp, Widow and Shogun in normal/Eclipse variants and reuses the two existing Lynx
definitions. Its six-element result exposes normal/eclipse handles for the future
encounter assembler. All 12 warriors retain historical template identity, tactic,
name, avatar, native attributes, ranged item, complete perk order/settings and
alignment rows, including duplicate normal-mode alignments. There is no runtime
XML read and no invented substitute template.

Verification:

- `TestWarriorPerkLoadouts.ps1`: 139 checks. Full projected rows for all 12 bosses
  compare to independent historical stages.xml rows; setting validation rejects
  invalid probabilities, fractional/out-of-range durations, nonnumeric values and
  overrides on owned Lua perks. Explicit zero/default omission, transactional
  rejection and presence/value fingerprint changes pass. Evidence:
  `Temp/WarriorPerks-e4aae405b8b1417ba63dd15b59bd84d0`.
- Isolated Unity 6000.6.0f1
  `Temp/FormNative-qpsmkaqv/DE128Runs/Run-7q2drxnx` exits 0. All 50 native perk
  clones across the 12 pending bosses preserve Aspect, ChanceFactor, Chance and
  Frames, inherit omitted parameters, accept explicit zero and leave baseline/
  sibling clones unchanged. Previous map progression, reward-slot, saved-fight
  and Jian/MindThrow acceptance remains green. This tests native perk parameter
  binding, not boss AI, activation statistics, storm timing in combat or a full
  story playthrough. No owner profile/editor was touched.
- Managed editor build and 2638 active DE128 foundation checks pass. Wiki and
  editor schema/generated definitions document both optional fields; generation,
  check, 37 project tests, LuaLS field completion and real VS Code integration
  pass. Wiki builds 47 pages, checks 4099 links/assets and has zero Astro
  diagnostics (existing duplicate-404 warning remains).

Encounter diagnosis: Guard_Girl and Guard_Man are still absent from both current
vanilla and historical DE template collections. The final prince additionally
names an item `Sphere1` absent from the canonical item list; its relationship to
the restored charge equipment must be verified before mapping it. Every Sensei
fight has a conditional RaidCharge button rule based on native availability;
this needs a typed runtime route or faithful Lua condition, not arbitrary native
variable access. Battle pairing, player equipment/identity rules, narrative
sequences and full encounter assembly remain to be connected and tested.

Package remains 0.18.0 with the boss module outside main.lua. Complete-corpus
acquisition/reconciliation remains deferred; these comparisons use historical
repository evidence only. Native portrait/model rendering and complete story
save/reload remain acceptance work.

## Step 39 — Sensei fight rules and native rule enforcement (2026-09-20)

Pending `scripts/content/sensei_fight_rules.lua` authors the unconditional rule
lists for all 23 archived Sensei fights through the existing typed Lua API.
These cover player equipment, avatar/name, opponent anti-shock and the two
Eclipse-only Ronin damage modifiers. Each list records the insertion position of
its conditional RaidCharge rule, whose handle remains separate and unattached.
The recovered RulesWithConditions parser currently flattens its body without
checking the condition. This milestone does not use that behavior or replace the
condition with unconditional suppression. A faithful typed runtime/Lua route is
still needed before encounter activation.

Two generic C# runtime defects surfaced and are repaired:

- NoButton previously hid only Punch/Kick and allowed keyboard/gamepad input.
  All five native action identifiers now suppress shared player input and button
  presentation. Applying a rule releases accepted held input; a fresh press after
  neutral is required when the rule ends. Availability refresh cannot reshow a
  blocked button, and ending a rule preserves unavailable actions. Opponent AI
  and local-versus player-two input are not redirected through player rules.
- Typed normal/Eclipse rules projected words into the native numeric boolean
  field. The adapter now emits 0/1, preserving Eclipse-only rules as Eclipse-only.
  Both-mode omission and native round filtering remain intact.

Verification:

- `TestSenseiFightRules.ps1`: 252 checks through actual MoonSharp registration,
  production projection, ordered historical comparisons for all 23 lists and
  native mode/round parsing. Evidence: `Temp/SenseiRules-67d31279ea344cf39a8111ab6c353b94`.
- `TestLocalVersusInput.ps1`: 59 checks including all five rule gates and held-input
  recovery. Battle rules: 268; trial rules: 43; UI runtime: 152; local-versus rules:
  38. The trial test source list was repaired to include existing ModMovePerkLocks.
- Isolated Unity 6000.6.0f1
  `Temp/FormNative-9kwp94a2/DE128Runs/Run-4wt7w3i_` exits 0. Actual native rule
  application, synthetic release, shared input suppression, neutral recovery,
  visibility/availability and both Ronin attribute modes pass. Prior boss perk,
  reward, map, saved-fight and Jian/MindThrow checks also pass. This is controlled
  native acceptance, not physical-device or complete Sensei gameplay verification.
- Managed editor build passes; 2638 DE128 foundation checks and 1282 Underworld
  assertions pass. Underworld audit exits 0 with existing missing raid-image
  findings. Wiki builds 47 pages, validates 4099 links/assets and reports no Astro
  diagnostics; its existing duplicate-404 warning remains. Public rules reference
  and editor guide describe the corrected input behavior; API signatures and
  editor schemas are unchanged.

Package stays 0.18.0; the new module is not loaded by main.lua. Conditional
RaidCharge behavior, guard templates, prince charge mapping, encounter assembly,
player equipment/identity gameplay, story save/reload and deferred source-corpus
reconciliation remain outstanding. No owner profile/editor was modified. An old
fixture lockfile cleanup was rejected by automatic approval review; that fixture
was preserved and acceptance ran in a fresh isolated copy.
