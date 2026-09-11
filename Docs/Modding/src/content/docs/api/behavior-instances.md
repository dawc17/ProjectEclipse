---
title: Reusable behaviors
description: Register Lua callbacks, configure typed parameters, and keep state for a round, fight, or saved effect.
---

A **behavior** is reusable Lua logic. A perk, enchantment, or battle rule attaches
it to a fighter and supplies its configuration. Registering a behavior by itself
does not activate it; attach it using the [perk or enchantment functions](../perks-and-enchantments/)
or [a behavior rule](../rules/#sf2rulesbehavior).

## sf2.behaviors.register

Register a behavior with one or more supported combat callbacks.

**Signature:** `sf2.behaviors.register { id, parameters?, state?, on_event = function, ... }`

**Requires:** `content.register`. Operations performed by callbacks may require
additional capabilities such as `combat.magic_charge`.

**When:** Entrypoint, before registering perks, enchantments, or rules that use it.

**Returns:** A behavior handle.

| Field | Type | Default | Meaning |
| --- | --- | --- | --- |
| `id` | String | Required | Stable local behavior ID. |
| `parameters` | Schema table | Empty | Typed configuration supplied by each attachment. |
| `state` | State specification | Omitted | Opt in to instance state and the `self` callback form. |
| Supported `on_*` names | Lua functions | At least one required | See the [callback reference](../combat-callbacks/). |

```lua
local opening_charge = sf2.behaviors.register {
    id = "opening_charge",
    parameters = { amount = sf2.behaviors.NUMBER },
    on_fight_begin = function(parameters, fighter, event)
        fighter:add_magic_charge(parameters.amount)
    end,
}
```

This example needs `combat.magic_charge` when the callback runs. A perk or
enchantment using it supplies `parameters = { amount = 0.35 }`.

## Parameter schema

The constants `sf2.behaviors.NUMBER`, `INTEGER`, `BOOLEAN`, and `STRING` have
the same bounds as [saved state fields](../mod-state/). Up to 64 parameters are
allowed. The short form `amount = sf2.behaviors.NUMBER` means a required number
with no default. The long form makes defaults explicit:

```lua
parameters = {
    amount = { type = sf2.behaviors.NUMBER, required = true, default = 0.35 },
    label = { type = sf2.behaviors.STRING, required = false },
}
```

Missing required parameters, unknown parameter names, and wrong types are
rejected. The attached perk/enchantment supplies values; the behavior supplies
the schema and functions.

## Instance state and callback arguments

Without `state`, callbacks receive `(parameters, fighter, event)`.
Declaring `state` changes that to `(self, fighter, event)`:

```lua
local ward = sf2.behaviors.register {
    id = "opening_ward",
    parameters = { multiplier = sf2.behaviors.NUMBER },
    state = {
        lifetime = "round",
        fields = {
            ready = { type = sf2.behaviors.BOOLEAN, default = true },
        },
    },
    on_damage_resolving = function(self, fighter, event)
        if self.state.ready and event.damage > 0 then
            fighter:scale_incoming_damage(self.params.multiplier)
            self.state.ready = false
        end
    end,
}
```

Attach this behavior with `parameters = { multiplier = 0.5 }` and declare
`combat.modify_hit`. It halves the first positive incoming hit of each round.

| State field | Default | Meaning |
| --- | --- | --- |
| `fields` | Empty schema | Typed values; required fields must have defaults. |
| `lifetime` | `"fight"` | `"round"`, `"fight"`, or `"saved"`. |
| `version` | `1` | Positive schema version for saved instance state. |
| `migrations` | Empty | Old-version functions that upgrade primitive state values. |

`self.params` contains resolved configuration; `self.state` contains copied
state for this particular active effect. Round state resets for a new round;
fight state resets for a new fight. Saved state follows a player's individual
equipped enchantment or learned perk and is persisted by the normal save path.
NPC and battle-rule instances support round and fight state, not saved state.
Rule state is isolated per rule and fighter, including when multiple rules share
one behavior. It never modifies an equipment save.

Saved-instance migrations use the same old-version → next-version function
shape as mod state. Do not retain `self.state` tables to edit later: a successful
callback validates and commits its current copy. Failed callbacks do not commit
state, but gameplay operations already performed are not undone.

Fighter/target/effect methods expire as soon as the callback returns. Keep your
own primitive values, not a fighter object, for later callbacks.
