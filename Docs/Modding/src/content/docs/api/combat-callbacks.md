---
title: Combat callbacks
description: Know exactly when each behavior callback runs and what its event data contains.
---

A **callback** is a Lua function you supply in a behavior definition. Eclipse
calls it when the corresponding combat event happens. Registering a callback
name elsewhere, such as at the top of `sf2`, does not subscribe it.

The snippets below are fields inside the table passed to
[`sf2.behaviors.register`](../behavior-instances/#sf2behaviorsregister).
They use the stateless `(parameters, fighter, event)` form. If the behavior
has a `state` specification, the first argument is `self`, with `self.params`
and `self.state`, instead.

## Shared event and fighter data

| Value | Meaning |
| --- | --- |
| `event.type` | Event name such as `FightBegin`, `DamageReceived`, or `FightEnd`. |
| `event.round` | Current round number when available. |
| `fighter.health` | Health snapshot, when the current fighter capability supplies it. |
| `fighter.opponent` | Opponent capability, when available; check it before using it. |
| `fighter.rule_id` | Qualified rule identity for fight-attached behavior callbacks; absent on equipment/perk instances. |
| `fighter.source` | Host provenance, including `rule`, `perk`, `enchantment`, or `warrior`. |
| `fighter.side` | Fighter context string, such as `player` or `opponent`, when supplied. |

Resolved hit events supply `damage`, `health_before`, `health_after`, `blocked`,
and `critical`. These describe the **victim**, including in `on_damage_dealt`.
Damage uses the runtime's life units; do not assume it is a health percentage,
raw weapon damage, or the entire pool of a boss with multiple health bars.
Pending damage has not been applied yet.

Fight-attached [behavior rules](../rules/#sf2rulesbehavior) also receive these callbacks, before each side's equipment/perk callbacks. Rule state is independent of equipped items.

Player perk callbacks use active learned perks and equipped enchantments; opponent
callbacks use active behavior-backed warrior perks. Normal fight rules may
suppress a perk. The dojo punchbag does not use this normal fight lifecycle.

Keep callbacks short. Fighter and effect methods expire at callback return.
A failing callback is logged and isolated; successful gameplay operations that
ran before the error are not undone. A callback's return value is not a way to
change damage; use the explicit [fighter methods](../fighter/).

Local Versus uses detached standard loadouts and does not dispatch these combat
behavior callbacks. Mod content projection remains loaded, but the first local
multiplayer version keeps its match lifecycle isolated from campaign/mod combat
events. This is a documented implementation boundary, not a claim that all mods
are disabled during Local Versus.

Local bootstrap also uses a temporary cloned profile document. Any profile changes
or mod-state migrations made against that local profile are discarded and are not
written into the campaign save. Content mods and their projected definitions remain
loaded for the runtime.

## on_fight_begin

Once when the first round begins, after ordinary perk initialization.

**Signature:** `on_fight_begin = function(parameters, fighter, event)`; with state,
`on_fight_begin = function(self, fighter, event)`.

**Requires:** `content.register` to register the behavior. No additional capability for the callback itself. This example uses `combat.magic_charge`.

**When:** The first round starts. It runs before `on_round_begin` for that round.

**Returns:** Return nothing; returned values are ignored by the dispatcher.

```lua
on_fight_begin = function(parameters, fighter, event)
    fighter:add_magic_charge(0.35)
end,
```

Use it for a starting resource or effect. It is not a callback for opening the map or loading a profile.

## on_round_begin

At the beginning of every round after round initialization.

**Signature:** `on_round_begin = function(parameters, fighter, event)`; with state,
`on_round_begin = function(self, fighter, event)`.

**Requires:** `content.register` to register the behavior. No additional capability for the callback itself.

**When:** Each round begins; in the first round it follows `on_fight_begin`.

**Returns:** Return nothing; returned values are ignored by the dispatcher.

```lua
on_round_begin = function(parameters, fighter, event)
    sf2.log.info("Round " .. tostring(event.round))
end,
```

Round-lifetime behavior state is available for the new round. Use round state for effects that should recharge every round.

## on_tick

Update timed behavior even when neither fighter lands a hit.

**Signature:** `on_tick = function(parameters, fighter, event)`; stateful behaviors
receive `self` instead of `parameters`.

**Returns:** Nothing; returned values are ignored.

**When:** Once per active combat simulation frame, after the combat clock advances
and before model movement, collisions, AI and round settlement for that frame.
Player callbacks run before opponent callbacks; rules run before that side's
equipped behavior callbacks. It starts after the round-begin callbacks. Paused
combat, round transitions and finished fights do not tick. This is the engine's
60-frame combat clock, not wall time or a Unity display-frame callback.

**Requires:** `content.register` to register the behavior. Observing the tick
requires no extra capability; operations retain their own requirements.

| Event field | Meaning |
| --- | --- |
| `frame` | Active combat frame number, cumulative across rounds in this fight. |
| `seconds` | `frame / 60`. |
| `delta_frames` | `1` for each delivered tick. |
| `delta_seconds` | `1 / 60`; pause time is excluded. |

```lua
-- Declare elapsed as an integer in round-lifetime state.
on_tick = function(self, fighter, event)
    self.state.elapsed = self.state.elapsed + event.delta_frames
    if self.state.elapsed >= 120 then
        self.state.elapsed = self.state.elapsed - 120
        sf2.log.info("Two seconds of active combat elapsed")
    end
end
```

Keep this handler small: it runs frequently. Use a counter or deadline to run
expensive logic less often. State retains its declared round/fight/saved lifetime;
the tick does not introduce persistent background work. Fighter methods expire
when the handler returns, and pending-hit modifiers are unavailable. A handler
error is isolated using the normal callback policy; it does not unsubscribe the
handler. Mod authors should avoid repeated failing work or per-frame logging.

## on_animation_start

Observe an animation entering its native start notification.

**Signature:** `on_animation_start = function(parameters, fighter, event)`;
stateful behaviors receive `self` instead of `parameters`.

**Returns:** Nothing; return values are ignored.

**When:** During an active round, immediately after native animation-start perk
notification. Both sides receive the event: player callbacks first, then opponent
callbacks, with rules before equipment/perks on each side. Events raised inside
another Lua combat callback wait until that dispatch returns; nested animation
events are delivered in order after both sides of the current animation event.
Round transitions, completed fights and Local Versus do not deliver these events.

**Requires:** `content.register` to register the behavior; no extra capability
to observe. Fighter operations retain their own capability requirements.

| Event field | Meaning |
| --- | --- |
| `animation_name` | Exact native animation name at notification time; owned moves use their qualified move ID. This is not a template-name matcher. |
| `target` | `"self"` for this fighter, `"opponent"` for the opposing fighter, or `"other"` for another actor, including projectiles. Relative to the callback recipient. |
| `frame` | Combat simulation frame when the event was raised. |

```lua
on_animation_start = function(parameters, fighter, event)
    if event.target == "self"
        and event.animation_name == sf2.mod.id .. ":moves/cast" then
        sf2.log.info("Cast started")
    end
end,
```

The event is detached data; changing its fields cannot change combat. A current
`fighter:snapshot()` may describe a later animation if dispatch was deferred.
`other` does not identify a projectile's owner and does not expose an actor
mutation capability. Filter the exact move name and relationship you need.
The host bounds each pending queue and delivery cascade to 256 events; overflow
is logged and discarded. Events queued for a round that has ended are discarded.

## on_animation_end

Observe the animation supplied by the native end notification.

**Signature:** `on_animation_end = function(parameters, fighter, event)`;
stateful behaviors receive `self` instead of `parameters`.

**Returns:** Nothing; return values are ignored.

**When:** After native animation-end perk notification during active combat.
It uses the same dispatch ordering, nested-event queue, limits and round filtering
as `on_animation_start`. This reports native notifications, not a guarantee that
every interrupted, deleted or replaced actor emits a matching end event.

**Requires:** `content.register` to register the behavior; no extra capability
to observe. Fighter operations keep their normal requirements.

The event has `animation_name`, `target` and `frame` with the same meanings as
the start callback; `event.type` is `"AnimationEnd"`.

```lua
on_animation_end = function(parameters, fighter, event)
    if event.target == "other"
        and event.animation_name == sf2.mod.id .. ":moves/projectile_flight" then
        sf2.log.info("Projectile flight animation ended")
    end
end,
```

## on_combo_changed

Observe the native combo counter for the fighter receiving this callback.

**Signature:** `on_combo_changed = function(parameters, fighter, event)`; stateful
behaviors receive `self` instead of `parameters`.

**Returns:** Nothing; the return value is ignored.

**When:** After native combo bookkeeping and perk notification.
`event.combo` is the current reported count; `event.last_combo` is the native
last-reported count. On native combo expiry, `combo` is zero and `last_combo`
retains the completed combo count. Counts below the native display threshold do
not produce this callback. Direct counter resets are not all event-producing;
use round/fight state lifetimes to reset your own data reliably.

**Requires:** No additional capability to observe. Operations called in the
handler keep their own capability requirements.

```lua
on_combo_changed = function(self, fighter, event)
    self.state.combo = event.combo
end
```

Declare `combo` in the behavior's integer state schema. This reports the engine's
combo, not a synthetic count of `on_damage_dealt` calls. It does not let scripts
change the counter or its timeout. Player/opponent rule filters apply normally.

## on_style_changed

Observe a transition between native style ranks.

**Signature:** `on_style_changed = function(parameters, fighter, event)`; stateful
behaviors receive `self` instead of `parameters`.

**Returns:** Nothing; the return value is ignored.

**When:** After the model receives a new style rank, before the native style-rule
check. Fields are `style_rank` (native zero-based index),
`style_name` (native name), `style_gain` (native progress value within that rank),
and `is_hit` (the native event's hit-origin flag). Same-rank progress updates do
not trigger this hook. Initial setup before fight callbacks begin is omitted.

**Requires:** No additional capability to observe.

```lua
on_style_changed = function(self, fighter, event)
    self.state.style = event.style_rank
end
```

Declare `style` in the behavior's integer state schema. Rank names/order come
from current game content; do not treat localized display strings as stable IDs.
Style snapshots are detached data. This callback does not mutate the style meter
or expose a general animation/contact event API.

## on_damage_dealing

Modify an attacker's pending hit after native damage, block and critical
calculation, before defender invulnerability, shields, incoming modifiers and
health application on the existing behavior hosts.

**Signature:** `on_damage_dealing = function(parameters, fighter, event)`; with
state, `on_damage_dealing = function(self, fighter, event)`.

**Returns:** Nothing; the return value is ignored.

**When:** A supported fighter deals a native strike. `fighter` is the attacker;
`event.damage`, `event.blocked` and `event.critical` snapshot the pending hit at
entry to this handler. Zero-damage hits can still reach the callback. Direct
health changes do not synthesize a strike or recursively invoke this event.

**Requires:** No additional capability to observe. Scaling or adding normalized
damage requires `combat.modify_outgoing_hit`.

```lua
on_damage_dealing = function(parameters, fighter, event)
    if event.critical and not event.blocked then
        fighter:scale_outgoing_damage(1.5)
    end
end
```

The method operates on the current pending value, so multiple handlers compose
in dispatch order. Your event table remains a snapshot. Normal defender rules
still apply afterward. This hook cannot set critical/block flags, create a hit,
or supply style/combo events. It is distinct from `on_damage_dealt`, which observes
health already lost. Callback-scoped methods expire on return, including errors.

`fighter:add_outgoing_damage(amount)` is also available here. It adds normalized
health units to the current pending outgoing hit instead of multiplying it.

## on_hit_post_crit

Observe the recovered native hit immediately after block/critical classification.

**Signature:** `on_hit_post_crit = function(parameters, fighter, event)`; with
state, `on_hit_post_crit = function(self, fighter, event)`.

**Returns:** Nothing; returned values are ignored.

**When:** For a native hit-phase snapshot on either side. `event.target` is
`"self"` when this fighter is the hit target and `"opponent"` when this fighter
is the attacker. `weapon`, `unarmed`, `ranged` and `magic` reproduce the recovered
native animation-category predicates for `Weapon`, `Unarmed`, `RangedMissile` and
`MagicMissile`. The booleans are observations, not editable flags.

**Requires:** No additional capability to observe. This callback is read-only for
the current pending hit.

```lua
on_hit_post_crit = function(self, fighter, event)
    if event.target == "opponent" and (event.ranged or event.magic) then
        self.state.armed = false
    end
end
```

The event contains `damage`, `blocked`, `critical`, `target`, `weapon`, `unarmed`,
`ranged` and `magic`. Pending-hit mutation methods are not supplied here, including
for the fighter's own incoming hit.

## on_post_hit

Observe the later native post-hit phase and, for the attacking side only, modify
the still-pending outgoing hit.

**Signature:** `on_post_hit = function(parameters, fighter, event)`; with state,
`on_post_hit = function(self, fighter, event)`.

**Returns:** Nothing; returned values are ignored.

**When:** For the same detached hit-phase fields as `on_hit_post_crit`. When
`event.target == "opponent"`, this fighter is the attacker and receives
`scale_outgoing_damage` and `add_outgoing_damage`. When `event.target == "self"`,
those pending-outgoing methods are absent.

**Requires:** No capability to observe. Attacker-side pending-hit modification
requires `combat.modify_outgoing_hit`.

```lua
on_post_hit = function(self, fighter, event)
    if event.target == "opponent" and not event.blocked and
        (event.weapon or event.unarmed) then
        fighter:add_outgoing_damage(self.params.bonus)
    end
end
```

The attacker-side methods expire when the callback returns. This callback cannot
modify the fighter's own pending incoming hit.

## on_damage_resolving

Just before an incoming hit is applied to the fighter.

**Signature:** `on_damage_resolving = function(parameters, fighter, event)`; with state,
`on_damage_resolving = function(self, fighter, event)`.

**Requires:** `content.register` to register the behavior. No additional capability for observation; `combat.modify_hit` for this example.

**When:** The final incoming hit reaches the health-application boundary.

**Returns:** Return nothing; returned values are ignored by the dispatcher.

```lua
on_damage_resolving = function(parameters, fighter, event)
    if event.damage > 0 then
        fighter:scale_incoming_damage(0.5)
    end
end,
```

`event.damage` is pending incoming damage. This is the only callback where `scale_incoming_damage` is available. It reduces this hit; later damage notifications report the resulting observed decrease.

The pending event also includes `blocked` and `critical`, captured
from the native hit flags before health application.

## on_damage_received

After the fighter loses health.

**Signature:** `on_damage_received = function(parameters, fighter, event)`; with state,
`on_damage_received = function(self, fighter, event)`.

**Requires:** `content.register` to register the behavior. No additional capability for observation.

**When:** A positive health decrease has been applied.

**Returns:** Return nothing; returned values are ignored by the dispatcher.

```lua
on_damage_received = function(parameters, fighter, event)
    sf2.log.debug("Damage received: " .. tostring(event.damage))
end,
```

The event contains the victim health snapshots and hit flags. A hit that leaves health unchanged does not produce a positive damage-received notification. A lethal hit cannot be undone by healing here.

## on_damage_dealt

After this fighter deals observed health damage.

**Signature:** `on_damage_dealt = function(parameters, fighter, event)`; with state,
`on_damage_dealt = function(self, fighter, event)`.

**Requires:** `content.register` to register the behavior. No additional capability for observation.

**When:** A positive health decrease is attributed to this attacker.

**Returns:** Return nothing; returned values are ignored by the dispatcher.

```lua
on_damage_dealt = function(parameters, fighter, event)
    sf2.log.debug("Damage dealt: " .. tostring(event.damage))
end,
```

The health fields describe the opponent who was hit, not the attacker. Unrelated NPC hits are excluded. Do not treat this as a notification for every attempted attack.

## on_block

Notify the defending fighter about a resolved blocked hit.

**Signature:** `on_block = function(parameters, fighter, event)`; with state,
`on_block = function(self, fighter, event)`.

**Requires:** `content.register` to register the behavior. No additional capability for observation.

**When:** A blocked hit resolves, including one that causes zero damage.

**Returns:** Return nothing; returned values are ignored by the dispatcher.

```lua
on_block = function(parameters, fighter, event)
    sf2.log.debug("Blocked a hit")
end,
```

The callback belongs to the victim/defender. `blocked` is true; resolved hit health/damage fields describe that defender.

## on_critical

Notify the attacker about a resolved critical hit.

**Signature:** `on_critical = function(parameters, fighter, event)`; with state,
`on_critical = function(self, fighter, event)`.

**Requires:** `content.register` to register the behavior. No additional capability for observation.

**When:** A critical hit resolves.

**Returns:** Return nothing; returned values are ignored by the dispatcher.

```lua
on_critical = function(parameters, fighter, event)
    sf2.log.debug("Landed a critical hit")
end,
```

The callback belongs to the attacker. The health/damage fields still describe the victim; `critical` is true.

## on_round_end

Notify the behavior when the round finishes.

**Signature:** `on_round_end = function(parameters, fighter, event)`; with state,
`on_round_end = function(self, fighter, event)`.

**Requires:** `content.register` to register the behavior. No additional capability for observation.

**When:** End-of-round stance completion or explicit surrender, once per round.

**Returns:** Return nothing; returned values are ignored by the dispatcher.

```lua
on_round_end = function(parameters, fighter, event)
    sf2.log.info("Round finished")
end,
```

Use this for round cleanup or accounting. Do not assume `event.won` is supplied here; the documented win/result fields belong to `on_fight_end`.

## on_fight_end

Notify the behavior when the complete fight finishes.

**Signature:** `on_fight_end = function(parameters, fighter, event)`; with state,
`on_fight_end = function(self, fighter, event)`.

**Requires:** `content.register` to register the behavior. No additional capability for observation.

**When:** Before the result flow, once per fight, including surrender.

**Returns:** Return nothing; returned values are ignored by the dispatcher.

```lua
on_fight_end = function(parameters, fighter, event)
    if event.won then
        sf2.log.info("This fighter won")
    end
end,
```

`event.player_result` is `"win"`, `"loss"`, `"surrender"`, or `"timeout"`. `event.won` is relative to the callback's fighter. Use `event.won` to count that fighter's victories; do not equate every end event with a win.

## fighter:change_form

**Signature:** `local request = fighter:change_form(character)`

**Returns:** A live result table with `status = "queued"`. At the simulation
boundary, status becomes `"applied"` or `"failed"`; failures include an `error`
string. Preparation errors or duplicate requests return an already failed result.
Invalid handles or missing capabilities raise a Lua error. Treat result fields as game-owned
observations. Keep this table in temporary Lua memory, not a saved state schema.

**When:** Inside an active combat behavior callback. The change
applies after the current simulation step. Pause delays application. Round end,
death or unloading fails a pending request. Fighter handles still expire at the
end of their callback; retaining this result does not extend their lifetime.

**Requires:** `combat.transform` and a handle returned by this mod's
`sf2.warriors.register`. This changes the callback's fighter. It is not exposed on
`fighter.opponent`; use an opponent-targeted rule to transform an opponent.
Only one request can be pending per fighter.

The new character supplies the body, equipment and native tactic. The swap
preserves health percentage, round wins, position, input timing, cooldowns, magic charge and a ready cast,
combat statistics, numeric/text perk variables and supported ongoing effects. It selects
an eligible native idle or transition animation for the current round, without
restarting the fight/round or editing saved player equipment. The first animation
frame runs after the replacement commits.
It does not resume an in-progress attack across different rigs.

Perk cooldown flags and variable modifiers keep their existing action and timer.
Variables retain their current values, including changes made since the effect
started; the swap does not evaluate their initial expressions again. Clearing
the retired body cannot erase these values. The destination keeps its own
rig and animation conditions. Failed handover restores both forms' prior state.

Magic keeps both its partial charge and ready-cast state when equipment changes.
The handover does not grant or consume a cast. Charging and cast consumption
continue through the normal combat rules; failed handover restores the previous
charge on each body.

**Verified native case:** the Shifting Guardian encounter swaps its staff fighter
to steel batons at frame 180, retains 75% health and numeric/text variables, reports
`applied`, and continues animating for 120 further combat frames without restarting
the timer. The isolated Unity check fails on combat exceptions as well as missing
or inactive models. Other effect/loadout combinations still need native acceptance;
active stolen magic can fail the request before retirement. Current-side move/perk
restriction inheritance also remains unfinished. Check the result rather than
treating `queued` as success.

```lua
-- second_form is a warrior handle registered before these callbacks run.
local pending
local requested = false
local behavior = sf2.behaviors.register {
    id = "transform",
    on_round_begin = function() pending, requested = nil, false end,
    on_tick = function(_, fighter)
        if not requested then
            requested = true
            pending = fighter:change_form(second_form)
        elseif pending and pending.status == "failed" then
            -- Display pending.error in your HUD; avoid retrying every frame.
            pending = nil
        elseif pending and pending.status == "applied" then
            pending = nil
        end
    end,
}
```
