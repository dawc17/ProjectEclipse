# DE128 - Definitive Edition

The owner-supplied archive is now the source of truth for DE content. See
[SOURCE_CORPUS.md](SOURCE_CORPUS.md) for acquisition status and reconciliation requirements.
The download is currently blocked by Google Drive quota; prior archive comparisons
below refer to the historical repository XML until reconciliation is performed.

DE128 is an ordinary downstream Eclipse mod. Version `0.18.0` restores ten missing weapon listings, thirteen other equipment
listings, and enables the archived
ChineseSwords combat/preview graph for Jian alongside four earlier equipment
combat-family corrections through Lua. It includes shop availability
for 25 former battle-pass items at their archived level gates and completes
saved, already-paid forge orders without waiting. Ascension remains disabled; Master of
Style and Relentless follow the archived DE XML.
Definitions, translations and behavior are authored through the public Lua API.

Pending `scripts/content/sensei_act_one_opponents.lua` ports normal/eclipse young
Lynx, including encounter-specific enchantment strength/chance settings. It is
**not loaded by the entrypoint**. Act I references `Guard_Girl` and `Guard_Man`
templates absent from the historical XML available here; no substitutes have
been invented. Story gates, rewards, presentation and live encounter acceptance
also remain unfinished. See Step 32 in `PRODUCTION.md`.

Pending `scripts/content/sensei_boss_opponents.lua` extends those loadouts to all
six young bosses in normal and Eclipse variants (12 warriors, 50 perk instances).
It preserves archived equipment, tactics, attributes, alignment rows and perk
settings, including explicit activation probabilities and Hermit's storm duration.
All rows match historical XML through the production adapter; native perk clone
checks pass. It remains outside main.lua, with guard templates, encounter rules,
boss AI/trigger playtests and full story acceptance unfinished (Step 38).

Pending `scripts/content/sensei_guard_opponents.lua` supplies the other 22
normal/Eclipse loadouts, including the prince, and combines them with the bosses
in the archived encounter order. Call its `register` function only after resolving
verified `guard_girl`, `guard_man` template handles and the `sphere1` item handle.
It provides no substitute for those missing inputs and has no registration side
effects when merely required. Complete ordered projections cover 34 loadouts in
23 encounters, with identity-only test fixtures for the unresolved inputs; this
does not validate their models, AI or charge behavior (Step 40).

Pending `scripts/content/sensei_fight_rules.lua` provides the unconditional rule
lists for all 23 Sensei fights: player equipment/identity, opponent anti-shock and
Ronin's Eclipse-only damage modifiers. Historical ordered rows and native mode
parsing are checked. The conditional RaidCharge rule is separate and unattached;
the recovered conditional wrapper ignores its condition, so the encounter
assembler must wait for faithful runtime support. This module also stays outside
main.lua (Step 39).

Pending `scripts/content/sensei_rewards.lua` contains all 57 normal/eclipse
reward slots across the six acts, including experience, gems and performance
bonus bases. It also stays outside the active entrypoint until the story is ready.
Native parsing and detached result calculations are checked against historical
XML; profile settlement and the full story remain unverified (Step 33).

Pending `scripts/content/sensei_progression.lua` computes the six acts eligible
to unlock after a victory, using saved tournament/prior-act wins and supplied
opened flags. Notification and persistence are handled by the pending coordinator
below. All 2048 prerequisite histories are compared with archived quest
conditions; the generic `sf2.profile.fight` query supplies the saved counters.

The generic `sf2.battles.set_locked` API can now update an owned, already revealed
battle on an unblocked map without resetting replay counts or fight history.
See Step 35 in `PRODUCTION.md` for its native map acceptance.

Pending `scripts/content/sensei_map.lua` now sequences the six initial reveals,
per-act unlock and map focus through public APIs. It stops on refusal and can be
retried without resetting existing progress. These map effects are tested against
the historical notification actions (Step 36).

Pending `scripts/content/sensei_notifications.lua` now coordinates ordered map
notifications, all 56 historical translations, Back/OK acknowledgment, Act I's
native Eclipse-mode exit, retry on refused actions, and persisted pending/opened
flags. Scene/profile cleanup never acknowledges. Controlled Lua/save checks pass;
the mode switch passes isolated native Unity acceptance. Full dialog rendering,
encounter assembly, missing guard templates and complete story/save acceptance
remain open. The coordinator is **not loaded by the entrypoint** (Step 37).

`scripts/content/chinese_swords.lua` registers both moves and Jian's subtype;
`chinese_swords_data.lua` contains their typed combat/presentation data. The mod
bundles the unchanged recovered animation binary, with no runtime XML dependency.
Complete archive comparisons, native parsing and binary decoding pass. An isolated
live fight also passed native double-tap/Forward selection, four attack intervals
with bound weapon edges, four timed sound actions and animation completion. AI,
hit contact, audible output and shop-preview acceptance remain open.
See Steps 12–13 in `PRODUCTION.md`.

The restored listings are Super Knives, Batons, Dragon Knives, Poleaxe, Kelt Axes,
Fans, Moon Fans, Imhotep Axes, Chinese Swords and Giant Sword. Their Lua definitions retain
archived act/level gates, gem prices, default enchantments and art references.
Kelt Axes and Moon Fans retain the archive's placeholder icon. Moon Fans uses
`initial_stats = {}` to preserve its absent initial damage attribute. A native
check confirms its first normal upgrade supplies 766 damage, matching the archive.
Steps 14-15 record the restored definitions, generic API support and verification
limits. Purchase/equip/save/reload acceptance remains open.

The additional equipment is Dragon Carapace, Old Legionnaire Armour, Samurai
Armour, Gabled Helm, Dragon Helm, Dragon Boomerangs, Dragon's Breath and Lightning
Arc, plus Minor, Medium and Large Charge of Darkness, Blast of the Void and Mind
Throw. Stats, prices, group/level gates and default enchantments match archived
item definitions. Samurai Armour deliberately retains only head defense (914).
The three ranged/magic families use base moves. `shared_moves.lua` now applies
five guarded changes: the heavy ranged uninterrupt end, Chakram hit reaction,
heavy ranged preview sound timing, and not-Stun conditions for MassBomb and
LightningArrow. Sphere1 and Sphere2 each have nine Lua-authored combat and shop
moves and reuse three unchanged recovered animation binaries. Sphere3 adds five
more moves and two unchanged binaries, retaining enemy-centered placement,
five ordered attack edges (including the archived repetitions), downward impulse,
and the native `Physycal` hit reaction. Complete archive
comparisons cover their different attack edges, effects, alignments and conditions.
Sphere2 retains its Stun restriction and three attack edges; it is not a recolored
Sphere1. ComboSphere3 adds four moves and Blast of the Void, retaining its bounded attack
window, `HighLong` reaction and archived placeholder icon. MindThrowNormal adds six
moves, five unchanged recovered binaries and the Mind Throw shop listing. Its
innate Lua perk sets an instance-owned flag on casting and clears it on the victim
reaction or wall cleanup; native ModExpires selects the caster's follow-up.
Complete move/template comparisons and the three archived perk trigger traces pass.
An isolated Unity fight passed native contact in the normal idle stance, the owned
victim reaction, one innate flag expiry, caster follow-up and final attack interval.
The recovered projectile passes above the crouched fists idle pose in the native
fixture. Contact across other stances and reconciliation with the deferred
owner-supplied corpus remain open; see Step 31 in `PRODUCTION.md`.
Hidden NPC equipment remains unregistered. Numerical damage, wall-miss cleanup,
visual/audio output and shop-preview acceptance remain open. See `PRODUCTION.md`
for the native lifecycle evidence and its limits.

## What this version does

- Makes **new and saved pending forge/enchantment orders instant**, using the normal recipes and
  material costs.
- Disables the six supported service groups: paid offers, battle pass,
  advertising, rewarded video, online services and payments. The verified native
  consumer suppresses matching quest groups and battle-pass quest sources.
- Registers `de128:items/weapon/titans_desolator`, reusing the core sword model,
  icon and `TitanGiantSword` move family. Its English name is registered in Lua;
  its innate loadout references core `PERK_TITAN` and `PERK_ANTI_SHOCK`.
- Adds Desolator to the winning reward of final Eclipse Titan fight 6. The reward
  uses the player's level when the result is prepared, and its Lifesteal aspect
  is calculated in Lua as `3639 / 100 * player_level + 60`.
- Adds the XML's level-4 **Master of Style / Relentless** perk choice and paired
  upgrade opportunities at levels 8, 11, 14 and 17. Both start at rank 1 and have
  five ranks. Six move definitions receive the archive's exact perk-lock removals,
  retaining their other conditions.
- Gives Jian the archived ChineseSwords family, extending ten Sai stance/attack
  locks and adding the four-hit super slash, localized moves-list entry and shop
  preview. Existing purchase/availability rules are unchanged.

## Combat perks

Master of Style arms a 300-simulation-frame bonus at Brutal style or higher. The
next unblocked weapon or unarmed hit adds 2/4/6/8/10 percentage points of normalized
health damage, according to rank. An unblocked incoming hit or an outgoing ranged
or magic hit cancels the bonus at the native post-critical phase. The archived
English description says Aggressive; the trigger XML also includes Brutal. The
mod follows the XML trigger and retains the original description.

Relentless tracks a combo of at least three hits, up to fifteen stacks. When that
combo ends, it arms a 300-frame bonus for the next outgoing hit only. Each stack
adds 1/2/3/4/5 percent damage according to rank. The XML has no block or attack-type
filter on consumption. Stack-counter details, including its retained value after
consumption, follow the trigger logic.

Both use the existing blue perk icons with expiration. The generic stack-count
badge is newly authored Eclipse presentation support: the archive requests stack
effects, but the recovered C# only implements the pulse effect. Its Unity rendering
still needs a game check.

Sources are `Assets/DExml/perks.xml`, `CharacterProgress.xml`,
`animations/moves.xml` and `localizations/eng.xml`. [Production notes](PRODUCTION.md)
record exact XML identities and hashes. No available metadata independently
identifies that archive as release 64. The existing `de128` namespace remains
stable so previously saved item references continue to resolve.

## Equipment combat families

`scripts/content/combat_equipment.lua` changes four existing core definitions:

| Equipment | Base family | DE family |
| --- | --- | --- |
| `WEAPON_CHNY22_SPEAR` | Spear | Naginata |
| `WEAPON_RAID_KARCER_SET` | Claws | HunterClaws |
| `WEAPON_BG_YARI` | Spear | MagariYari |
| `RANGED_BP_S3_WIND_MAKER` | Chakram | Kunai |

Those four target families have native moves in the base. Identity, prices, power,
art and saved ownership stay unchanged. New equipment/fighter copies use the
patched family; Apply & Restart with DE disabled restores the original family.
Existing explicit AI groups remain intact. Test attacks, ranged throws, shop
previews and NPC handling with each item before claiming full combat acceptance.

`content.chinese_swords` additionally changes `WEAPON_CHNY21_JIAN` from
HermitSwords to ChineseSwords and supplies its complete archived move graph.
The recovered binary has 38 samples/67 nodes. All attacks and sounds fit; the
archive's longer recovery interval follows native end-of-animation clamping,
which has been checked in Unity. Live execution/completion passes; contact and
player-facing timing still need acceptance.
The separate archived `WEAPON_CHINESE_SWORDS` item is not restored by this step.
Monk Katar and Musket
retain their canonical AI groups while archive intent is investigated.

## Former battle-pass equipment

`scripts/content/shop.lua` exposes the five pieces of each collection through
ordinary shop availability: Guardian at level 15, Skanda at 20, Wind Maker at 25,
Time Shifter at 45 and Scriptwriter at 50. No pass or event-group membership is
required. Existing core item IDs, earned-gem prices, stats, upgrade rules,
enchantments and ownership are preserved. These are availability gates, not
rewritten equipment levels. The mod does not grant equipment on activation.

This implements the archive's shop access for these collections. Different
archive stats, level/upgrade records and set abilities are not implemented by
this module. Wind Maker's ranged subtype is handled separately above. Other
special-offer collections remain open.

On a test profile, reopen the shop one level below and at each listed gate; check
all five categories, preview/equip, purchase at the displayed existing gem price,
and save/reload. Disable DE128 through Apply & Restart and verify base visibility
returns while owned items remain owned. Re-enable to restore the listings.
Automated eligibility tests and loaded native art are not a purchase playtest.

## Ascension disabled

The entrypoint call and both Ascension module bodies are commented out. Even
explicitly requiring either file produces no definitions. No Ascension zone,
battle, quest, rule, opponent, translation or state schema is registered. Existing
saved prototype state is retained as inactive data. Apply & Restart to unload
content already present in a running session. Generic engine APIs remain available
to other mods; the prototype test launcher now reports SKIP.

Saved pending forge orders complete on the next normal delivery update. Their
original timestamps are not rewritten; disabling DE128 before completion restores
their wait. Completed enchantments remain completed. Service gates do not prove that every
associated button, offer or direct service entry point is hidden.

Desolator has no purchase listing and is hidden before acquisition. The reward
patch preserves the base game's coins and forge materials. It does not unlock
Titan, grant equipment at startup, replace the NPC sword or migrate inventory.
The item uses normal core weapon progression. Already owning this mod's sword
suppresses another reward copy, including a sword manually added for testing;
that existing copy keeps its level and enchantments.

The generic reward callback uses a snapshot taken before reward experience is
awarded. The result is also safe to calculate for previews: it only returns
level/enchantment data and makes no profile changes. This is a defined API timing
contract, not a promise that every archived expression has the same timing when
a reward also levels up the player. Native settlement applies the prepared perk
through the ordinary inventory/save path.

No additional art files are bundled. Core references still require their assets
to resolve in the installed game; a missing required reference rejects the whole
mod registration. The owner confirmed the manually added sword worked on
2026-09-18. That check does not establish reward-screen, Lifesteal or reload
acceptance for the newly connected reward.

## Enable and try it

Keep the directory named `de128`, matching `id = "de128"` in `mod.toml`.
Open **Mods** on the title screen, select **DE128 - Definitive Edition**, then
choose **Apply & Restart** and enter Campaign.

Another enabled mod must not own the forge timer. In particular, the integrated
`example.phase2` showcase also claims it; disable that showcase when testing DE128.
An overlapping timer declaration fails explicitly instead of silently replacing
the other mod's policy. Service-only disables from several mods can coexist.

Use a profile approaching the level-4 perk choice to test acquisition. Choose
Master of Style or Relentless, verify the rank-1 value, then check its four upgrade
opportunities. Already learned perks are preserved; enabling this version does
not retroactively award either perk. Check timed icons, one-hit consumption and
save/reload. Double jump kick, elbow strike, two-foot jump kick, backflip kick and
both Suplex move variants should no longer require their old perk unlocks, while
their skeleton/equipment/screen requirements still apply.

With the forge unlocked and enough materials, start a new enchantment. It should
apply immediately and charge the usual materials. An already-pending order should
complete once without charging materials again, clear its saved delivery entry,
and remain complete after save/reload. A failed enchantment should keep its order.
Disable DE128 before a pending order completes and
apply the selection again to restore the base policy, unless another mod owns it.
Already completed enchantments remain completed.

For the acquisition check, use a profile that has unlocked the final Eclipse
Titan fight and does not own `de128:items/weapon/titans_desolator`. Win fight 6,
check the sword's level and Lifesteal, then save and reload. Compare a normal-mode
result and an already-owned result: neither should grant a new copy. The manual
test copy in an existing profile is intentionally left alone.

## Source layout

`scripts/main.lua` loads the content modules in an explicit order. Service policy
lives in `scripts/content/services.lua`; forge timing lives in
`scripts/content/timers.lua`; Desolator's definition, translation and innate perks
live in `scripts/content/equipment.lua`. The grant calculation and final Titan
patch live in `scripts/content/rewards.lua`. New perk behavior, icons,
translations and rank tables live in `scripts/content/combat_perks.lua`; exact
branch replacements and move-lock removals live in `scripts/content/progression.lua`.
`ascension.lua` and `ascension_rules.lua` are commented archival prototypes.

All DE gameplay definitions and behavior must be authored through Lua. The TOML
manifest is loader metadata. Archived `Assets/DExml` is research evidence, never
a runtime dependency or a mod definition source loaded by this package. DE128
uses the normal sandbox, ownership and transaction rules and has no privileged
engine branch. Core prices and shared economic rules stay base-owned.

The manifest requests `policy.services`, `policy.timers`, `content.register`,
`content.patch`, `combat.modify_outgoing_hit` and `combat.effects`. The new perks
use transient state per fighter/perk/round. They do not register or mutate an owned
persistent state schema. Native learned-perk saves retain selected ranks.

## Verification and next work

Run `./Tools/TestDE128Foundation.ps1` from the repository root. It executes the
actual packaged Lua through MoonSharp and checks policy consumption, Desolator
registration, Lua localization, actual reward calculations, callback errors and
instruction budgets, module and asset failures, missing capabilities, conflicts,
composition and rebuilding without DE.
It uses an isolated temporary fixture and requires the project's MoonSharp DLL
plus the .NET 10 SDK. Its core definitions come from canonical game data, with
controlled metadata for the declared art references. It does not decode art or run
a Unity playtest. `Tools/TestRewardOnlyEquipment.ps1` checks the generic runtime
adapter responsible for equipment with no purchase listing; its output states
the native services substituted by that fixture.

`Tools/TestRewardGrantNative.ps1` checks the configured reward bridge, actual
canonical Eclipse Titan reward projection and recovered enchantment serialization.
It extracts the changed production methods and supplies controlled engine/catalog
services. It does not run a campaign or load the owner's save.

The foundation fixture also compares both perks' rank tables and progression
slots to the archive, executes hit/cancellation/expiry traces, and verifies that
both disabled Ascension modules remain inert when required directly.
`Tools/TestDECombatPerksNative.ps1` checks precise native hit and status paths;
`Tools/TestPerkUpgradeNative.ps1` checks native first-rank selection;
`Tools/TestMovePerkLocks.ps1` checks targeted live lock removal and restoration.

[Production notes](PRODUCTION.md) record source evidence, known API gaps and the
next candidate. Development proceeds in substantial steps with status reports. This foundation
does not claim complete DE parity or a completed game playtest.
