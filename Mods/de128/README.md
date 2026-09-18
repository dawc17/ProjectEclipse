# DE128 - Definitive Edition

DE128 is an ordinary downstream Eclipse mod. Version `0.5.0` disables the Ascension
prototype and implements Master of Style and Relentless from the archived DE XML.
Definitions, translations and behavior are authored through the public Lua API.

## What this version does

- Makes **new forge/enchantment orders instant**, using the normal recipes and
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

## Ascension disabled

The entrypoint call and both Ascension module bodies are commented out. Even
explicitly requiring either file produces no definitions. No Ascension zone,
battle, quest, rule, opponent, translation or state schema is registered. Existing
saved prototype state is retained as inactive data. Apply & Restart to unload
content already present in a running session. Generic engine APIs remain available
to other mods; the prototype test launcher now reports SKIP.

Existing saved forge deadlines retain their original time. Their normal skip
option stays available, with the existing base price. This version does not yet
make those pending orders instant. Service gates also do not prove that every
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
apply immediately and charge the usual materials. Compare an already-pending
order separately: it keeps its deadline and normal skip option. Disable DE128 and
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
next candidate. Development proceeds one approved step at a time. This foundation
does not claim complete DE parity or a completed game playtest.
