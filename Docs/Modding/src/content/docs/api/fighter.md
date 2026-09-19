---
title: Fighter methods
description: Observe combat, change health and magic charge, reduce pending damage, and manage temporary shields in callbacks.
---

These methods are supplied as the `fighter` argument to supported
[combat callbacks](../combat-callbacks/). Use a colon (`:`), which passes the
fighter as the method's first argument. They are not global `sf2` functions.

The examples below run **inside a callback**. A fighter reference must not be
saved and used after that callback returns. Missing capability declarations,
expired references, invalid values, or an unavailable fighter raise Lua errors.
Most mutation methods return `nil` on success; exceptions are documented below.
Observation methods return detached data.

## fighter:snapshot

Read fresh combat observations, including both fighters and the engine's elapsed
fight clock. Use this when making a health or distance
decision; the older `fighter.health` field is captured at callback entry.

**Signature:** `fighter:snapshot()`

**Returns:** A `CombatSnapshot` table, or `nil` if the fighter cannot be observed.

**When:** Inside any supported combat behavior callback, including battle rules,
perks, enchantments, and warrior behaviors. Each call samples the current state.
The callable reference expires when that callback returns; the returned data may
be retained as an observation, but will not update itself.

**Requires:** No additional capability. Observing the opponent does not require
`combat.target`; changing the opponent still requires the normal capabilities.

| Field | Meaning |
| --- | --- |
| `self` | The fighter receiving this callback, even when it is the enemy. |
| `opponent` | The opposing fighter's snapshot, or `nil` when unavailable. |
| `frame` | Nonnegative elapsed active-fight frame count, shared by both sides. |
| `seconds` | `frame / 60`, using the recovered engine's simulation rate. |
| `round_active` | Whether the engine is processing the round at capture time. |

Both fighter snapshots contain `health`, `max_health`, `health_bars`, and
`position = { x, y, z }`. Health uses the same normalized pool units as
`fighter.health`; `max_health` is the maximum in those units. `health_bars` is
the total authored bar count (at least one), not the number remaining. For
multi-bar opponents, incoming damage operations use single-bar units; do not
equate the normalized pool with damage points. Divide health by max health for
a fraction, guarding against a zero maximum.

Each fighter snapshot also has an optional `animation` table.
It is `nil` when the native controller is absent, stopped, or cannot provide a
valid bounded observation. The rest of the fighter snapshot remains available.

| Animation field | Meaning |
| --- | --- |
| `name` | Native name of the currently playing animation, including authored namespaced moves. |
| `type` | Native authored classification: `"none"`, `"move"`, or `"attack"`. |
| `facing` | Native orientation sign: `1` along positive model X, `-1` along negative model X. |
| `intervals` | Array of currently active `{ name, type }` interval observations, in native order. May be empty; at most 256 entries. |

Interval `type` is one of `"none"`, `"unstable"`, `"uninterrupt"`,
`"self_uninterrupt"`, `"attack"`, `"block"`, `"invulnerable"`, or
`"invisible"`. `name` preserves the authored interval name, or is empty when
the native interval has no name. Custom named intervals may have type `"none"`.
These are observations from the controller's latest interval update, not the
animation's complete list of future windows. An active attack interval does not
guarantee contact; an interval label does not override other native combat rules.

For example, inside a callback you can detect an opponent's active attack window:

```lua
local combat = fighter:snapshot()
local animation = combat and combat.opponent and combat.opponent.animation
local attacking = false
if animation then
    for _, interval in ipairs(animation.intervals) do
        if interval.type == "attack" then attacking = true; break end
    end
end
-- Use attacking in your own Lua rule logic.
```

Animation and interval tables are detached
values: editing them cannot start/stop an animation, turn a fighter, or add/remove
an interval. They can be retained as historical observations, but never used as
an AI candidate or an engine operation handle.

Positions use arena model coordinates, not screen pixels or metres. The clock
counts active fight frames across rounds, excluding round transitions and frames
when fight processing is stopped; it is neither wall time nor the remaining round
timer. Capture a baseline in `on_round_begin` to measure a round-local duration.
This method does not schedule callbacks or grant control of the clock.

```lua
on_damage_resolving = function(self, fighter, event)
    local combat = fighter:snapshot()
    if not combat or combat.self.max_health <= 0 then return end
    local fraction = combat.self.health / combat.self.max_health
    if fraction < 0.25 then
        fighter:scale_incoming_damage(0.5) -- requires combat.modify_hit
    end
end
```

Each result is a detached Lua table. Editing it affects only your copy and cannot
move a fighter, change health, alter later snapshots, or change the engine clock.
Call again after a health operation to obtain a fresh observation. Query failure
does not fabricate zero health or a default position.

## fighter:scale_outgoing_damage

Scale the current attacker's pending hit.

**Signature:** `fighter:scale_outgoing_damage(multiplier)`

**Returns:** `nil` on success; invalid values, missing capability, unavailable hit
or expired references raise a Lua error.

**When:** Inside `on_damage_dealing`, and inside attacker-side `on_post_hit` where
`event.target == "opponent"`. It is absent from `on_hit_post_crit` and incoming
`on_post_hit` callbacks.

**Requires:** `combat.modify_outgoing_hit`.

```lua
on_damage_dealing = function(parameters, fighter, event)
    fighter:scale_outgoing_damage(1.25)
end
```

The multiplier must be finite and between 0 and 16 inclusive. The resulting
damage must remain finite, nonnegative, and representable as a single-precision
number. Invalid scaling leaves the pending value unchanged. Calls multiply the
current value; zero remains zero. Invulnerability, shields, incoming modifiers,
and the normal health path run afterward. This does not bypass defense or change
the shared weapon/stat economy. Earlier successful operations remain applied if
a later handler fails. Retained methods cannot modify a later hit.

## fighter:add_outgoing_damage

Add normalized health damage to the current attacker's pending hit.

**Signature:** `fighter:add_outgoing_damage(amount)`

**Requires:** `combat.modify_outgoing_hit`.

**When:** Inside `on_damage_dealing`, and inside attacker-side `on_post_hit` where
`event.target == "opponent"`. It is absent from `on_hit_post_crit` and incoming
post-hit callbacks.

**Returns:** `nil` on success; invalid values, missing capability, unavailable hit
or expired references raise a Lua error.

`amount` must be finite and from `0` through `1` normalized health units. It is
added to the current pending damage value; the resulting value must remain finite,
nonnegative and representable as a single-precision number. Defender rules and
normal health application still run afterward.

```lua
fighter:add_outgoing_damage(0.04)
```

## fighter:change_health

Add or subtract health through the game's normal health-change path.

**Signature:** `fighter:change_health(amount)`

**Requires:** `combat.change_life`.

**When:** A supported callback with a live fighter capability.

**Returns:** `nil`.

`amount` is a finite number representable by a 32-bit float. Positive values
heal; negative values damage. Values use the game's life units rather than a
separate percentage-conversion API. A fighter whose health has already reached
zero cannot be revived with this operation.

```lua
fighter:change_health(0.05)
```

Use small values and test them against the relevant opponent and health-bar
configuration. Healing after `on_damage_received` does not reverse a lethal hit.

## fighter:add_magic_charge

Add to the fighter's magic charge and let the game normalize it.

**Signature:** `fighter:add_magic_charge(amount)`

**Requires:** `combat.magic_charge`.

**When:** A supported callback with a fighter capability.

**Returns:** `nil`.

`amount` must be a finite 32-bit-float value. The example adds 0.35 charge through
the normal magic-charge path. The player needs a usable magic item to observe it.

```lua
fighter:add_magic_charge(0.35)
```

## fighter.opponent:change_health

Apply the health-change operation to the opposing fighter.

**Signature:** `fighter.opponent:change_health(amount)`

**Requires:** Both `combat.target` and `combat.change_life`.

**When:** A supported callback where `fighter.opponent` is available.

**Returns:** `nil`; the same numeric and no-revival rules as `change_health` apply.

```lua
if fighter.opponent then
    fighter.opponent:change_health(-0.05)
end
```

## fighter.opponent:add_magic_charge

Adjust the opposing fighter's magic charge.

**Signature:** `fighter.opponent:add_magic_charge(amount)`

**Requires:** Both `combat.target` and `combat.magic_charge`.

**When:** A supported callback where the opponent is available.

**Returns:** `nil`; accepts a finite 32-bit-float number.

```lua
if fighter.opponent then
    fighter.opponent:add_magic_charge(0.1)
end
```

There are no opponent shield or opponent pending-hit methods on this target
object. The explicitly exposed methods are the two operations above.

## fighter:scale_incoming_damage

Multiply this fighter's pending incoming damage before it is applied.

**Signature:** `fighter:scale_incoming_damage(multiplier)`

**Requires:** `combat.modify_hit`.

**When:** Only inside `on_damage_resolving`.

**Returns:** `nil`.

`multiplier` is finite and between 0 and 1 inclusive. `0` prevents this hit's
damage, `0.5` halves it, and `1` leaves it unchanged. Values above 1 are rejected.
Multiple active behaviors multiply in dispatch order.

```lua
fighter:scale_incoming_damage(0.5)
```

Calling this after damage has resolved is too late; the method is not supplied
in those callbacks.

## fighter:add_damage_shield

Apply temporary damage reduction to future hits.

**Signature:** `fighter:add_damage_shield(key, fraction, frames)`

**Requires:** `combat.effects`.

**When:** A supported callback with fighter effect operations.

**Returns:** `nil`.

| Argument | Type and bounds | Meaning |
| --- | --- | --- |
| `key` | 1–64 letters, digits, or underscores | Name used to refresh/remove this behavior's shield. |
| `fraction` | Finite number, 0–1 | Fraction of damage reduced; `0.25` means 25% reduction. |
| `frames` | Integer, 1–3600 | Duration in simulation frames; 60 frames is one combat second. |

```lua
fighter:add_damage_shield("opening_ward", 0.25, 180)
```

This requests 25% reduction for three combat seconds. Reusing a key refreshes
that behavior's shield; different keys can compound. At most 128 shields are
active per fighter. The timer pauses with combat and shields clear at round/fight
cleanup. They do not add native buff icons or extra boss health bars.

## fighter:remove_damage_shield

Remove a shield created under this behavior's key.

**Signature:** `fighter:remove_damage_shield(key)`

**Requires:** `combat.effects`.

**When:** A supported callback with fighter effect operations.

**Returns:** `nil`.

`key` uses the same naming rules as `add_damage_shield`. It is scoped by the
behavior identity; this does not remove arbitrary native effects.

```lua
fighter:remove_damage_shield("opening_ward")
```

## fighter:set_flag

Create a native combat flag owned by this behavior instance.

**Signature:** `fighter:set_flag(key)`

**Requires:** `combat.effects`.

**When:** A supported callback during an active round, with a currently registered
fighter. The method expires when the callback returns.

**Returns:** The qualified native flag name as a string. Setting an existing key
does nothing and returns the same name. Invalid input or unavailable support raises
a Lua error.

`key` is 1–64 ASCII letters, digits or underscores. The native name is
`<qualified behavior ID>:<key>` and must fit within 128 characters in total.
For behavior `example.spell:behaviors/cast`, key `pending` becomes
`example.spell:behaviors/cast:pending`.

```lua
on_animation_start = function(parameters, fighter, event)
    if event.target == "self"
        and event.animation_name == sf2.mod.id .. ":moves/cast" then
        fighter:set_flag("pending")
    end
end,
```

Flags have no timer. Clear them explicitly or let native round/fight cleanup
remove them; they are not saved in the profile. They participate in native
modifier lists, `mod_exists` conditions, expiry notifications and fighter form
transfers. There are at most 64 flag-owning instances per fighter registration
and 64 active flags per instance; exceeding either limit raises an error.

Ownership includes both behavior identity and equipped/rule instance. One instance
cannot clear another instance's flags, even if both use the same key. Native move
conditions see the qualified name, so they match any active instance using that
name. This method cannot create or overwrite a core flag such as `Stun`.

## fighter:has_flag

Check this behavior instance's flag without changing it.

**Signature:** `fighter:has_flag(key)`

**Requires:** `combat.effects`.

**When:** A supported callback during an active round with a registered fighter.

**Returns:** `true` if this instance owns the flag, otherwise `false`. Invalid
keys, expired callback references or unavailable support raise a Lua error.

```lua
if fighter:has_flag("pending") then
    sf2.log.info("This cast still owns its pending flag")
end
```

The key and length rules are the same as `set_flag`. This does not query arbitrary
native modifiers or another instance's state.

## fighter:clear_flag

Remove this behavior instance's flag through native modifier expiry handling.

**Signature:** `fighter:clear_flag(key)`

**Requires:** `combat.effects`.

**When:** A supported callback during an active round with a registered fighter.

**Returns:** `nil`. Clearing an absent key succeeds without emitting another
expiry event. Invalid keys, expired methods or unavailable support raise an error.

```lua
fighter:clear_flag("pending")
```

Clearing an existing flag removes it from `mod_exists` observations and emits the
native `ModExpires` event for its qualified name. A move can listen using
`events = { { type = "mod_expires", name = "example.spell:behaviors/cast:pending" } }`.
Normal move conditions, locks and priority still decide whether that move runs.
Clearing one instance emits its expiry even if another instance retains a flag
with the same qualified name. For purely Lua bookkeeping with no native move
interaction, prefer declared behavior state.

## fighter:show_status_icon

Show or refresh a transient status icon owned by this behavior instance.

**Signature:** `fighter:show_status_icon(key, sprite, frames, stacks?)`

**Requires:** `combat.effects`.

**When:** A supported behavior callback with a live fighter capability.

**Returns:** `nil`; invalid arguments, unavailable native support or an expired
fighter method raise a Lua error.

| Argument | Type and bounds | Meaning |
| --- | --- | --- |
| `key` | Valid behavior field name | Local key used to refresh or clear this behavior instance's icon. |
| `sprite` | Sprite handle | Captured sprite handle from the current mod context. |
| `frames` | Integer, 1–3600 | Native-frame lifetime. `0` is rejected. |
| `stacks` | Integer, 0–10000 | Optional stack count; defaults to `0`. |

```lua
local icon = sf2.assets.sprite("core:ui/skills/IconCrackedApple_Blue")
fighter:show_status_icon("relentless", icon, 300, 5)
```

The native key includes the behavior instance identity, so matching Lua keys from
different behavior instances are isolated. The method expires when the callback
returns. This documents the scripting contract only; it does not claim rendered
HUD acceptance for a particular sprite.

## fighter:clear_status_icon

Clear a status icon addressed by this behavior instance.

**Signature:** `fighter:clear_status_icon(key)`

**Requires:** `combat.effects`.

**When:** A supported behavior callback with a live fighter capability.

**Returns:** `nil`; invalid keys, unavailable native support or expired references
raise a Lua error.

```lua
fighter:clear_status_icon("relentless")
```

The key uses the same validation and behavior-instance isolation as
`show_status_icon`; it does not clear arbitrary status entries owned elsewhere.
