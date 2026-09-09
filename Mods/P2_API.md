# Phase 2 — API 0.6

User runtime-tested and accepted, including the corrected replayable Volcano raid. The integrated external
sample is [example.phase2](example.phase2/README.md). Phase 1 remains accepted.
This document lists the supported contract, rather than promising every possible
recovered combat event or arbitrary engine access.

## Behavior instances

Existing callbacks without a `state` declaration retain `(parameters, fighter, event)`.
Declaring state opts into `(self, fighter, event)`:

```lua
local ward = sf2.behaviors.register {
    id = "ward",
    parameters = { multiplier = sf2.behaviors.NUMBER },
    state = {
        lifetime = "round",
        fields = { ready = { type = sf2.behaviors.BOOLEAN, required = true, default = true } },
    },
    on_damage_resolving = function(self, fighter, event)
        if self.state.ready and event.damage > 0 then
            fighter:scale_incoming_damage(self.params.multiplier)
            self.state.ready = false
        end
    end,
}
```

`self.params` is the resolved configuration for the attached perk/enchantment.
`self.state` is a copied, typed instance state table. Lifetimes are `fight`, `round`,
or `saved`; fight/round reset through the host's fight identity and round number.
Player instance identity comes from the saved equipped enchantment or learned perk.
NPC instances are isolated by runtime fighter and perk; they support fight/round state.
Saved NPC state is rejected because there is no persistent player-owned instance.

Saved state lives in an additive, behavior-qualified `EclipseBehaviorState` child
of the saved perk, alongside unchanged `EclipseParams`. It uses `version` (default 1)
and `migrations = { [oldVersion] = function(old) ... end }`. Migrations advance one
version at a time, have the existing P0 instruction budget, and return a primitive
table or mutate their copied input. See Veteran Guard for a version-1-to-2 example.
Invalid/future data is preserved and the callback reports an error. Missing mods do
not delete their saved instance data. State reaches disk through the normal save path.

State commits only after the callback succeeds and the entire output validates.
Gameplay operations already performed before a script error are not rolled back.
Retaining/mutating a previous `self.state` table does not mutate the next invocation.
Fighter, target, hit and effect methods expire when their callback returns.

The suggested instance/composition design informs this API: typed state, automatic
defaults/persistence, reusable functions, and a clear configuration/state split.
Ordinary Lua modules/functions provide composition; no `implements` hierarchy,
mandatory child-object tree, hand-written serializer, or generic operation DSL is
required. Static content and recovered compatibility adapters remain declarative.

## Verified combat callbacks and capabilities

| Callback | Boundary |
| --- | --- |
| `on_fight_begin` | First `NextRound`, after native perk initialization |
| `on_round_begin` | After native round initialization; follows fight-begin on round 1 |
| `on_damage_resolving` | Final incoming hit before `LogDamage` and health application |
| `on_damage_received` | Positive observed health decrease after `UpdateLife` |
| `on_damage_dealt` | Same boundary on the attacker; excludes unrelated NPC hits |
| `on_block` | Victim of a resolved blocked hit, including zero damage |
| `on_critical` | Attacker of a resolved critical hit |
| `on_round_end` | End-stance completion or explicit surrender, once per round |
| `on_fight_end` | Before the result flow, including surrender; once per fight |

Player dispatch uses active learned perks and saved equipped effects, retaining
native rule filtering. NPC dispatch uses active behavior-backed warrior perks.
The sample opponent has Opening Ward, so NPC behavior execution is observable.

Resolved hit events contain numeric `round`, `health_before`, `health_after`,
`damage`, and boolean `blocked`/`critical`. These are victim observations even for
`on_damage_dealt`; damage is the observed decrease in recovered life units, not an
assumed health percentage or total multi-bar damage. Pending hit events expose
`damage` before application. Lifecycle events have a numeric round when available.
Fight-end additionally supplies `player_result` (`win`, `loss`, `surrender`,
`timeout`) and `won` relative to the callback's fighter.

`fighter.health` and `fighter.opponent.health` are snapshots. Existing
`change_health(amount)` / `add_magic_charge(amount)` use `combat.change_life` /
`combat.magic_charge`. Opponent versions additionally require `combat.target`.
Health operations cannot revive a fighter whose resolved health is zero.

`fighter:scale_incoming_damage(multiplier)` requires `combat.modify_hit`, is only
available during `on_damage_resolving`, and accepts finite values in 0..1.
Multiple active callbacks compound in dispatch order. It never exposes the hit object.

`fighter:add_damage_shield(key, fraction, frames)` and `remove_damage_shield(key)`
require `combat.effects`. Fraction is 0..1; duration is an integer 1..3600 simulation
frames (60 frames per combat second). A repeated key refreshes that instance's
shield, other keys compound, and shields clear on round/fight cleanup. The host
bounds active shields to 128 per fighter. The clock advances with combat, not paused
wall time. Shields affect future resolved damage and do not add a native buff icon.

No raw Models, arbitrary flags/attributes, unbounded Lua update callbacks, or
unverified magic/miss/shock/animation hook families are exposed. `FightNone`/dojo
punchbags still have no normal fight lifecycle. These are explicit contract bounds.

## Timer and service policy

```lua
sf2.timers.set { subsystem = "forge", seconds = 0, skip_enabled = false }
sf2.services.disable("battle_pass")
```

Timer policy requires `policy.timers`; supported subsystem: `forge`. Zero means
instant; positive values up to one year create a timed delivery. New orders use
the policy; existing saved deadlines remain intact. Material prices and skip
prices are untouched. Disabling skip rejects early completion at the host boundary.
Only one mod may own a subsystem's timer policy; conflicts reject the transaction.
Shop delivery is already instant in this base and is not an additional timer target.

Service policy requires `policy.services`. Known keys: `paid_offers`, `battle_pass`,
`ads`, `rewarded_video`, `online_services`, `payments`. Disabled gates combine across
mods. The quest host gates the corresponding named groups (`Offers`, `BattlePass`,
`Advertising`, `RewardedVideo`, `OnlineServices`, `Payments`); battle-pass extension
source paths are also gated. Many service/SDK surfaces are already absent in the
offline base, so disabling them will not necessarily produce a visible change.
This API does not install/re-enable services or exclude SDK binaries from builds.

UI integration uses typed map battles as mode/event entry points, the existing
fight panel, progress labels, eligibility/missing-key messages, and P1 shop
acquisition presentation. There is no arbitrary GameObject mutation API.

## Events, mode sequences and Ascension

`sf2.events.register`, `sf2.modes.register`, and `sf2.raids.register` share:

```lua
sf2.modes.register {
    id = "trial",
    fights = { first_fight, second_fight, final_fight },
    minimum_level = 1,
    repeatable = true,
    reset_on_loss = true,
    -- Omit both timestamps for permanent availability.
    -- starts_at = UTC_UNIX_SECONDS, ends_at = UTC_UNIX_SECONDS,
    -- entry_item = my_consumable, entry_count = 1,
}
```

Requires `content.register`. Fights must be owned by this mod, registered through
P1, and belong to exactly one mode step. Sequences contain 1..100 distinct fights.
Mode registration is part of the same atomic content transaction as those fights.

Schedule start is inclusive, end is exclusive; omitted/zero end is permanent.
Eligibility is checked again at actual entry. Every owned map battle resolves to
the mode's current fight. The panel shows progress and entry-item requirements,
even when the old story roster considers a repeatable fight complete.

A win advances the sequence; a loss either keeps or resets it. Completing a
repeatable sequence resets to its first fight and increments a completion count.
Otherwise it remains complete. `EclipseModes Version="1"` preserves progress and
entry reservations across save/load and missing/reinstalled mods. A changed saved
sequence is rejected without silently reinterpreting its progress.

Entry items must be mod-owned consumables, not shared currencies. Costs apply per
fight attempt. Re-entering a reserved attempt after interruption does not charge
again; a failed scene launch refunds a new reservation. A resolved loss/surrender
ends the attempt. Result duplication is rejected before the normal reward path.

Rewards and weighted loot remain the P1 fight/reward primitives. The Ascension
sample uses three ordinary story fights and grants the five canonical Monk items
on its final victory, with a public item-set definition describing them. It does
not depend on a DE toggle or enable the old native Ascension battle adapter.

## Offline raids and quest integration

A raid encounter is one fight against a boss with a multi-bar health pool, not a
sequence of replacement enemies. Set `health_bars` on its warrior definition:

```lua
sf2.warriors.register {
    id = "boss", tactic = "Standard", health_bars = 10,
    -- template, presentation and equipment as usual
}
sf2.rewards.register { id = "boss_prize", gems = 25 }
-- Attach that warrior and reward to a one-round fight with round_time = 300.
```

`health_bars` is an integer 0..10000: zero/omitted inherits the template; positive
values set the total number of bars, including the active bar. It maps to recovered
`ShieldTotal`, which is preserved through fighter cloning. It is distinct from
Lua temporary damage-reduction shields. The native HUD supplies the blue bar and
remaining-bar count; damage carries across bars, and death occurs at zero total
health. Registered offline raids require boss death to win, including on timeout.

`gems` is an integer 0..1000000 (default zero), mapped to the recovered reward's
`Bonus` field. It is a fixed content reward paid by the normal result/save path,
not a scripting capability to mutate a currency balance. Use an empty loss reward
and a gem victory reward. The integrated sample awards 25 gems for one ten-bar boss
with a 300-second timer. The current boss uses Volcano's recovered equipment, portrait and arena. The base no longer adds a legacy raid page; map raid pages come from mod registrations. The sample has free unlimited entry; retry starts a fresh full pool. Entry-item costs remain optional for other modes.

Register ordinary zone/warrior/rule/reward definitions, battles with `type = "raid"`,
then attach their fights through `sf2.raids.register`. Raid battles without their
offline mode registration are rejected. `hard_mode = true` opts a raid into the
existing Power Mode map filter; names do not need `ZONE_RAID` or `_HARDMODE`.
Different mods retain separate namespaced zones, bosses, progression and keys.

Registered offline raids parse normal rewards, enter through the local fight path,
complete the previously empty `EndFightRaid` path, award through normal saved item
acquisition, and show the normal result panel once. Legacy nonregistered server
raid result behavior is preserved.

P1 quest definitions additionally accept `raid_fight_enter`, `raid_fight_end`,
`raid_enter`, `raid_end`, `reset_mode`, `raid_map_enter`, `raid_floor_changed`, and
`show_raid_loot`. Raid-enter is an attempt entry; raid-end is sequence completion.
The recovered adapters for `OpenRaidZone`, `RaidIndicateRaidBtn`, and `ShowRaidLoot`
now open the local raid zone, reveal its map toggle, and present pending local loot
respectively. The indication adapter does not reproduce the legacy mobile arrow.

## Verification

`Tools/TestP2ACombatRuntime.ps1` now executes the full public Phase 2 sample plus
the production mode host with isolated engine/UI dependencies. It covers typed
state/migration, round reset, hit reduction, shield expiry, policy conflicts,
schedules, progression, key accounting/refund, result deduplication and orphan
save restoration. `TestModFightBeginRuntime.ps1` covers the production player
dispatcher and source ordering. Underworld runtime checks cover the retained
health-pool and damage machinery. Managed builds and the isolated Unity art/mod
compatibility validator supplement these checks; none constitutes a full playtest.

`Tools/TestRaidContentBridge.ps1` checks the production XML adapters, native gem
reward parser, and multi-bar damage/death. `TestModStartupRuntime.ps1` also checks
the production raid winner decision at timeout and boss death. HUD appearance
and the complete revised raid remain subject to manual playtesting.
