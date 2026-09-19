---
title: Fight rules
description: Register equipment restrictions, perk rules, presentation overrides, and round-scoped fight settings.
---

Rule functions create definitions. Add the returned handles to a fight's `rules`
array; calling a rule function does not immediately change the current fight.

Every function below requires `content.register`, runs in the entrypoint, and
returns a **rule handle**. Referenced items/perks must exist and belong to your
mod or a declared dependency. Unknown fields are rejected.

## Shared rule fields

| Field | Default | Meaning |
| --- | --- | --- |
| `id` | Required string | Stable local rule ID. |
| `target` | `sf2.rules.ALL` | `PLAYER`, `OPPONENT`, or `ALL`, on functions that accept it. |
| `mode` | `sf2.rules.BOTH` | `NORMAL`, `ECLIPSE`, or `BOTH`. `BOTH` is the string `"all"`. |
| `rounds` | Empty integer array | Restrict the rule to listed rounds; omitted means no round filter. |

Examples assume `sf2` is loaded. `weapon` and `perk` below are previously
registered or looked-up handles. Each example creates a different local rule ID.

## sf2.rules.no_perks

Disable perks matching the rule's target and optional name filter.

**Signature:** `sf2.rules.no_perks { id, target?, mode?, rounds?, name? }`

**Requires:** `content.register`.

**When:** Entrypoint, before attaching the rule to a fight.

**Returns:** A rule handle.

`name` is an optional string, default `""`; omit it for an unfiltered no-perks rule. Common field defaults are listed above.

```lua
local rule = sf2.rules.no_perks {
    id = "no_perks", target = sf2.rules.OPPONENT,
}
```

## sf2.rules.require_item

Require an item as a fight-entry/equipment condition.

**Signature:** `sf2.rules.require_item { id, item, minimum_level?, mode?, rounds? }`

**Requires:** `content.register`.

**When:** Entrypoint, before attaching the rule to a fight.

**Returns:** A rule handle.

`item` is required; `minimum_level` is a nonnegative integer defaulting to `0`. This rule has no `target` field. Common field defaults are listed above.

```lua
local rule = sf2.rules.require_item {
    id = "require_blade", item = weapon, minimum_level = 1,
}
```

## sf2.rules.equip_item

Apply a specified item to the target for this fight.

**Signature:** `sf2.rules.equip_item { id, item, minimum_level?, target?, mode?, rounds? }`

**Requires:** `content.register`.

**When:** Entrypoint, before attaching the rule to a fight.

**Returns:** A rule handle.

`item` is required; `minimum_level` is a nonnegative integer defaulting to `0`. Fight equipment replacement is not a permanent inventory grant. Common field defaults are listed above.

```lua
local rule = sf2.rules.equip_item {
    id = "equip_blade", item = weapon, target = sf2.rules.PLAYER,
}
```

## sf2.rules.avatar

Override the target's fight portrait reference.

**Signature:** `sf2.rules.avatar { id, name, target?, mode?, rounds? }`

**Requires:** `content.register`.

**When:** Entrypoint, before attaching the rule to a fight.

**Returns:** A rule handle.

`name` is a required existing portrait identifier string. It is not a sprite handle; use a verified portrait name from core content. Common field defaults are listed above.

```lua
local rule = sf2.rules.avatar {
    id = "portrait", name = "avatar_hero", target = sf2.rules.PLAYER,
}
```

`avatar_hero` is an existing core avatar name.

## sf2.rules.name

Override the target's displayed name during the fight.

**Signature:** `sf2.rules.name { id, name, target?, mode?, rounds? }`

**Requires:** `content.register`.

**When:** Entrypoint, before attaching the rule to a fight.

**Returns:** A rule handle.

`name` is a required string, typically a qualified localization reference. Common field defaults are listed above.

```lua
local rule = sf2.rules.name {
    id = "fighter_name", name = "my.mod:localization/opponent.name", target = sf2.rules.OPPONENT,
}
```

## sf2.rules.no_button

Disable a named combat button through the native fight-rule system.

**Signature:** `sf2.rules.no_button { id, name, target?, mode?, rounds? }`

**Requires:** `content.register`.

**When:** Entrypoint, before attaching the rule to a fight.

**Returns:** A rule handle.

`name` is a required native button identifier. It is not arbitrary UI text; use an identifier verified for the intended button. Common field defaults are listed above.

```lua
local rule = sf2.rules.no_button {
    id = "restricted_button", name = "Punch", target = sf2.rules.PLAYER,
}
```

Supported native identifiers are `"Punch"`, `"Kick"`, `"Ranged"`, `"Magic"`, and
`"RaidCharge"`. Active rules hide the relevant control and reject player-one
touch, keyboard and gamepad presses through their shared input path. A held
control is released when blocked and must return to neutral before a fresh press
can work after the rule ends. Availability refreshes cannot show a blocked button;
removing a rule does not make unavailable equipment usable.

This native rule controls the player's input, not AI move selection. The recovered
runtime does not interpret its `target` as an opponent-input restriction; use
`PLAYER` for clarity. Round changes clear and reapply the active restrictions.

## sf2.rules.perk

Apply a perk rule to the selected target.

**Signature:** `sf2.rules.perk { id, perk, aspect?, target?, mode?, rounds? }`

**Requires:** `content.register`.

**When:** Entrypoint, before attaching the rule to a fight.

**Returns:** A rule handle.

`perk` must be a perk handle, not a behavior or enchantment handle. `aspect` is an
optional finite number from `0` through `2147483647`; omit it to keep the native
perk rule's normal aspect/default behavior. Common field defaults are listed above.

```lua
local rule = sf2.rules.perk {
    id = "opponent_perk", perk = perk, aspect = 100000,
    target = sf2.rules.OPPONENT,
}
```

## sf2.rules.hot_ground

Create the native timed hot-ground rule used by challenge fights.

**Signature:** `sf2.rules.hot_ground { id, frames, nodes, animations?, target?, mode?, rounds? }`

**Requires:** `content.register`.

**When:** Entrypoint, before attaching the rule to a fight or encounter plan.

**Returns:** A rule handle.

`frames` is required and must be an integer from `1` through `216000`. `target`
defaults to `sf2.rules.PLAYER`; only `PLAYER` or `OPPONENT` are valid for this
rule. `ALL` is rejected because the recovered native copy path loses the
hot-ground runtime rule type when applying it to both sides. `nodes` is
a required dense array of 1–32 tables. Each node requires a nonempty native node
`name` of at most 128 characters and `axis = "X"` or `"Y"`. At least one of
`min` or `max` is required; supplied bounds are finite single-precision numbers,
and when both are present they must satisfy `min < max`. Duplicate `name` + `axis`
pairs are rejected. `animations` is optional and defaults to an empty list; when
supplied it is a dense array of at most 64 distinct nonempty native animation
names, each at most 128 characters.

```lua
local rule = sf2.rules.hot_ground {
    id = "burning_floor",
    frames = 300,
    nodes = {
        { name = "root", axis = "Y", max = 0 },
    },
    target = sf2.rules.PLAYER,
}
```

## sf2.rules.ring_out

Create a native ring-out boundary rule.

**Signature:** `sf2.rules.ring_out { id, node, axis, min, max, target?, mode?, rounds? }`

**Requires:** `content.register`.

**When:** Entrypoint, before attaching the rule to a fight or encounter plan.

**Returns:** A rule handle.

`node` is a required nonempty native node name of at most 128 characters. `axis`
is exactly `"X"` or `"Y"`. `min` and `max` are required finite single-precision
numbers and must satisfy `min < max`. Shared target/mode/round filters use the
defaults above.

```lua
local rule = sf2.rules.ring_out {
    id = "ring_out", node = "root", axis = "X", min = -180, max = 180,
}
```

## sf2.rules.regeneration

Create the native regeneration or degeneration rule.

**Signature:** `sf2.rules.regeneration { id, rate, frames_after_hit, target?, mode?, rounds? }`

**Requires:** `content.register`.

**When:** Entrypoint, before attaching the rule to a fight or encounter plan.

**Returns:** A rule handle.

`rate` is a required finite number from `-1` through `1`. Negative values retain
their native negative behavior rather than being clamped away. `frames_after_hit`
is a required integer from `0` through `216000`.

```lua
local rule = sf2.rules.regeneration {
    id = "enemy_regen", rate = 0.001, frames_after_hit = 180,
    target = sf2.rules.OPPONENT,
}
```

## sf2.rules.no_animation

Disable one named native animation for both fighters.

**Signature:** `sf2.rules.no_animation { id, name, mode?, rounds? }`

**Requires:** `content.register`.

**When:** Entrypoint, before attaching the rule to a fight or encounter plan.

**Returns:** A rule handle.

`name` is a required nonempty native animation name of at most 128 characters.
This rule intentionally has no `target` field: the native rule applies to both
fighters. `mode` and `rounds` use the shared defaults.

```lua
local rule = sf2.rules.no_animation { id = "no_jump", name = "Jump" }
```

## sf2.rules.remove_interval

Remove one native animation interval class from the selected target.

**Signature:** `sf2.rules.remove_interval { id, type, target?, mode?, rounds? }`

**Requires:** `content.register`.

**When:** Entrypoint, before attaching the rule to a fight or encounter plan.

**Returns:** A rule handle.

`type` is required and is exactly one of `Attack`, `Block`, `Invulnerable`,
`SelfUninterrupt`, `Uninterrupt`, or `Unstable`.

```lua
local rule = sf2.rules.remove_interval {
    id = "no_block", type = "Block", target = sf2.rules.PLAYER,
}
```

These native rule constructors expose typed, evidence-backed challenge payloads.
They do not expose raw XML or arbitrary rule names. Use them in a fight's `rules`
array or in an encounter plan's dynamic `rules` replacement.

## sf2.rules.recharge_magic_each_round

Request normal magic recharge at round boundaries.

**Signature:** `sf2.rules.recharge_magic_each_round { id, target?, mode?, rounds? }`

**Requires:** `content.register`.

**When:** Entrypoint, before attaching the rule to a fight.

**Returns:** A rule handle.

The target needs a usable magic item for this rule to have a visible effect. Common field defaults are listed above.

```lua
local rule = sf2.rules.recharge_magic_each_round {
    id = "recharge", target = sf2.rules.ALL,
}
```

## sf2.rules.attributes

Apply a map of native fight attribute values.

**Signature:** `sf2.rules.attributes { id, values, target?, mode?, rounds? }`

**Requires:** `content.register`.

**When:** Entrypoint, before attaching the rule to a fight.

**Returns:** A rule handle.

`values` is a required string-to-finite-number table. Names are native attribute identifiers; unknown names do not implement new gameplay logic. Common field defaults are listed above.

```lua
local rule = sf2.rules.attributes {
    id = "attributes", values = { WeaponDamage = 100 }, target = sf2.rules.OPPONENT,
}
```

## sf2.rules.behavior

Attach executable Lua behavior directly to a fight, without creating a perk or
requiring an equipped item.

**Signature:** `sf2.rules.behavior { id, behavior, parameters?, target?, mode?, rounds? }`

**Returns:** A rule handle; put it in `sf2.fights.register { rules = { rule } }`.

**When:** Register in the entrypoint. Attached handlers run only at the supported
combat callback boundaries of the selected fight.

**Requires:** `content.register`. Each fighter operation still requires its own
combat capability, declared by the mod that owns the behavior.

`behavior` is a handle returned by `sf2.behaviors.register`. `parameters` is a
map matching that behavior's schema; required values and types are validated at
registration. `target`, `mode`, and `rounds` use the shared defaults above.
Unknown fields, missing behaviors, undeclared dependencies, and saved-lifetime
behaviors are rejected. The behavior must use `fight` or `round` state (or no
state); use `sf2.state` explicitly for longer-lived mod progression.

```lua
local guard = sf2.behaviors.register {
    id = "guard",
    parameters = { every = sf2.behaviors.INTEGER },
    state = {
        lifetime = "round",
        fields = { hits = { type = sf2.behaviors.INTEGER, default = 0 } },
    },
    on_damage_resolving = function(self, fighter, event)
        if event.damage <= 0 then return end
        self.state.hits = self.state.hits + 1
        if self.state.hits % self.params.every ~= 0 then
            fighter:scale_incoming_damage(0)
        end
    end,
}
local rule = sf2.rules.behavior {
    id = "third_strike", behavior = guard, parameters = { every = 3 },
    target = sf2.rules.OPPONENT,
}
-- Add rules = { rule } to your fight definition.
```

The example needs `combat.modify_hit`. See the complete
[programmable-rules guide](../../guides/programmable-rules/).

State is isolated per attached rule and fighter, even when two rules reuse one
behavior. An `ALL` rule gets separate player and opponent state. Fight state
survives rounds; round state resets on the first callback in each new round.
There is no mid-fight save/resume contract. Rule state never enters equipment or
profile XML. The rule ID is available as `fighter.rule_id`, with
`fighter.source == "rule"`.

Each side runs rules in the fight's declared `rules` order, before that side's
perk/enchantment callbacks. A duplicate rule handle runs once. These rules are
independent of `no_perks`. Filters are checked for every callback: a rule limited
to round two does not receive `on_fight_begin` in round one. It receives only the
callbacks that occur while its filters match. No omitted callback is synthesized.

Handlers share the existing execution budget, error isolation, and expiring
fighter handles. One rule's failure does not stop subsequent rules; already
performed gameplay operations are not rolled back. Nested combat notifications
from an operation inside a handler are suppressed, preventing recursive loops.
Shield keys are isolated by rule ID, so two rules reusing one behavior do not
replace each other's shield on the same fighter.

This API does **not** yet provide custom victory conditions, tick callbacks, animation control, additional fighters, or custom
HUDs. The available operations remain the documented fighter methods. Rules can be attached to new fights or appended to or replace
the rules of existing encounters through [fight patches](../content-graph/#sf2fightspatch).

Attacker-side scaling is available through `on_damage_dealing` and
`fighter:scale_outgoing_damage` with `combat.modify_outgoing_hit`.

For conditional action restrictions, attach a player-targeted behavior and call
[`fighter:set_control_blocked`](../fighter/#fighterset_control_blocked) from
`on_round_begin` after reading your mod's condition. This requires `combat.effects`.
The claim belongs to that rule instance; releasing it cannot undo native
`no_button` rules or another instance's restrictions. It does not grant equipment,
charges or an ability button, and must be reapplied each round.
