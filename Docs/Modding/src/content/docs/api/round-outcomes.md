---
title: Custom round objectives
description: End an offline round through a declared Lua objective and the normal result flow.
---

A **round controller** is a fight-attached behavior rule that declares
`controls_outcome = true`. It can request a player win or loss without changing
either fighter's health. Use Lua state and existing combat observations for
objectives such as landing a combo, surviving a duration or protecting a resource.

Register the behavior and rule in your entrypoint, then attach the rule to an
owned fight or patch an existing fight:

```lua
local sf2 = require("sf2")
local objective = sf2.behaviors.register {
    id = "survive",
    state = { lifetime = "round", fields = { ticks = { type = "integer", default = 0 } } },
    on_tick = function(self, fighter, tick)
        self.state.ticks = self.state.ticks + tick.delta_frames
        if self.state.ticks >= 600 then fighter:end_round("win") end
    end,
}
local rule = sf2.rules.behavior {
    id = "survive", behavior = objective, target = sf2.rules.PLAYER,
    controls_outcome = true,
}
sf2.fights.patch {
    target = "core:fights/zone_1/tournament/3", append_rules = { rule },
}
```

This example requires `content.register`, `content.patch`, `combat.round_outcome`
and a `core` manifest dependency. Ten seconds here means 600 active simulation
ticks. Pause stops those ticks. It is a round win: ordinary rounds-to-win,
survival/opponent sequences and complete-fight settlement still apply.

## Ownership and conflicts

`controls_outcome` defaults to `false` and must be a boolean. Setting it to
`true` also requires `combat.round_outcome` during rule registration. The mod
owning the behavior needs the capability when it invokes the operation.

Only one distinct controller rule can apply to a fight in the same round/mode.
`target` chooses who receives callbacks; it does not divide result authority.
Different targets therefore still conflict. Controllers with disjoint `rounds`
or `mode` filters can coexist. Known conflicts reject the registering transaction
before definitions or patches commit, naming both rules. Generated encounter rule
lists are checked during encounter projection and again when selected for combat. Existing fight-patch conflict
rules also continue to apply.

For cooperating mods, use one controller and pass objective data through
[framework services](../extensions/). Equipment perks, enchantments and ordinary
rules do not acquire result authority by using the controller's behavior or by
declaring the same capability. Authority is tied to the attached rule instance.

## fighter:end_round

**Signature:** `fighter:end_round(outcome) -> boolean, string?`

**Returns:** `true, nil` when the host queues the request; `false, reason` when
the rule has no authority, the round is unavailable or another outcome is pending.
An accepted request is provisional until the simulation boundary. It does not
mean the round or complete fight has already ended.

**When:** An active simulation callback for the controller's attached rule,
normally `on_tick` or a combat observation. Unavailable in `on_fight_begin`,
`on_round_begin`, `on_round_end` and `on_fight_end`, from expired captured fighter
tables, while paused, or after a native result is pending.

**Requires:** `combat.round_outcome`, an applicable `controls_outcome = true`
rule and a main fighter in an offline fight. Supported offline mod raids use
the same round request; online raids, versus/PvP, training and title sparring
reject requests. `outcome` must be exactly `"win"` or `"loss"`, relative to the
**player**, even in an opponent callback. Missing capability, invalid argument
types/values and expired/wrong callback timing raise errors.

```lua
-- Inside the controller's on_tick callback:
if self.state.score >= 10 then
    local accepted, reason = fighter:end_round("win")
    if not accepted then sf2.log.warn(reason) end
end
```

The host queues rather than recursively ending combat from Lua. At the normal
round boundary, knockout, native timeout and native end rules take precedence
over a queued custom result. Surrender discards it. Otherwise the engine stops
round processing and uses its existing score update, end stance and later
result/reward paths. It does not kill, heal or directly grant rewards.

Repeated requests for the same result are idempotent while pending. A conflicting
request returns `false` and cannot replace the first. The boundary consumes the
request once; the resolved winner remains stable for end animations, then resets
for the next round. Requests are local to the fight and are not saved or replayed
after restart. A callback error after an accepted request does not roll it back.

## Example and verification

The [Three-hit Objective example](https://github.com/dawc17/ProjectEclipse/tree/main/Mods/example.hit-objective)
combines round-local state, outgoing damage observations, a ten-second simulation
clock, HUD and custom win/loss requests in Act I tournament battle 3. Copy both
its manifest and script, enable it through Mods and use Apply & Restart.
Disable examples that patch the same fight while testing it.

Managed tests execute the production Lua binding and native round arbitration,
score, winner and surrender methods with controlled clock/model/presentation and
settlement services. These checks cover conflicts, authority, pause, stale handles,
same-boundary precedence and repeated calls. The shipped example also passes an
isolated Unity 6.6 Play Mode check with the recovered font, visible changing HUD
pixels, native fade teardown, three-hit wins and ten-second losses. Its contacts,
models, clock, end presentation and settlement remain controlled. These checks
do not establish real contact timing, end-animation selection, native reward/save integration or full-game
acceptance; test those paths before distributing an objective mod.
