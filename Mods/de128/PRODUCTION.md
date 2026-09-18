# DE128 production record

Current package: **0.5.0**. The owner rejected the Ascension prototype and directed
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
| DE128-01 | `ModTimerPolicy` accepts only `forge`. `RecipeItemInfo.cs:55-66` restores saved deadlines directly, and both `ForgeManager.FinishEnchant` and `UserItems.FinishDeliveryRecipe` consult the shared skip flag. | Pending orders are not instant. Keep skipping enabled in this version. A generic policy for settling existing orders needs lifecycle, materials, enchantment and save tests before implementation. Do not rewrite saved deadlines from the mod. |
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
