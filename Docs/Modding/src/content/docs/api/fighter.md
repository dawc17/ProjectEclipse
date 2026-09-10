---
title: Fighter methods
description: Change health and magic charge, reduce pending damage, and manage temporary shields in callbacks.
---

These methods are supplied as the `fighter` argument to supported
[combat callbacks](../combat-callbacks/). Use a colon (`:`), which passes the
fighter as the method's first argument. They are not global `sf2` functions.

The examples below run **inside a callback**. A fighter reference must not be
saved and used after that callback returns. Missing capability declarations,
expired references, invalid values, or an unavailable fighter raise Lua errors.
Every method returns `nil` on success; it does not return a success boolean.

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
