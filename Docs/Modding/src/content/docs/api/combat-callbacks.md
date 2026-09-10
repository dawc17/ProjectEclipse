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
| `fighter.side` | Fighter context string, such as `player` or `opponent`, when supplied. |

Resolved hit events supply `damage`, `health_before`, `health_after`, `blocked`,
and `critical`. These describe the **victim**, including in `on_damage_dealt`.
Damage uses the runtime's life units; do not assume it is a health percentage,
raw weapon damage, or the entire pool of a boss with multiple health bars.
Pending damage has not been applied yet.

Player callbacks use active learned perks and equipped enchantments; opponent
callbacks use active behavior-backed warrior perks. Normal fight rules may
suppress a perk. The dojo punchbag does not use this normal fight lifecycle.

Keep callbacks short. Fighter and effect methods expire at callback return.
A failing callback is logged and isolated; successful gameplay operations that
ran before the error are not undone. A callback's return value is not a way to
change damage; use the explicit [fighter methods](../fighter/).

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
