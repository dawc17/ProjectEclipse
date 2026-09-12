---
title: Saved random streams
description: Make reproducible random choices that continue from a player's saved mod state.
---

API **0.20** provides random streams for encounter selection and other Lua
behavior. A stream is an ordinary, declared integer field in your mod's
[saved state](../mod-state/). Its default value is its seed. Each draw updates
that field in the loaded profile; normal game saves persist the updated value.
There is no separate random handle or global seed.

Declare the fields once in your entrypoint:

```lua
local sf2 = require("sf2")
sf2.state.register {
    version = 1,
    fields = {
        route_rng = { type = sf2.state.INTEGER, default = 12345 },
        reward_rng = { type = sf2.state.INTEGER, default = 67890 },
    },
}
```

Read or draw only after the profile loads, inside a supported callback such as
[a mode's `on_result`](../events-and-modes/#on_result). Declare both `state.read`
and `state.write` in `mod.toml`. Every stream value must be a signed 32-bit
integer (`-2147483648` through `2147483647`); zero and negative seeds are valid.
Optional fields must have a value before use. Unknown fields, other types and
out-of-range saved integers fail without advancing the stream.

## sf2.random.integer

Draw a whole number with equal probability for each value in an inclusive range.

**Signature:** `sf2.random.integer(field, minimum, maximum)`

**Returns:** Integer between `minimum` and `maximum`, inclusive.

**When:** After profile state is bound, in a supported callback.

**Requires:** `state.read` and `state.write`; an owned, declared integer state field.

`field` is a string naming your mod's field. Both bounds are required signed
32-bit integers and `minimum <= maximum`. Fractional values, numeric strings,
NaN and infinity are rejected. The full signed 32-bit range is supported. A
one-value range still advances the stream.

```lua
-- Inside on_result, with fights registered earlier and route_rng declared above:
if result.won and result.step == 1 then
    return fights[sf2.random.integer("route_rng", 2, 3)]
end
```

The implementation rejects words that would bias a remainder calculation.
Consequently one integer draw can advance the field more than once. Native work
is capped at 128 attempts; exceeding that cap raises an error and leaves the
field unchanged. Invalid arguments also leave it unchanged.

## sf2.random.number

Draw a fractional value from zero inclusive to one exclusive.

**Signature:** `sf2.random.number(field)`

**Returns:** Number in `[0, 1)`, in increments of `1 / 4294967296`.

**When:** After profile state is bound, in a supported callback.

**Requires:** `state.read` and `state.write`; an owned, declared integer state field.

```lua
-- Inside a supported callback, with reward_rng declared above:
local rare = sf2.random.number("reward_rng") < 0.1
sf2.log.info(rare and "Rare outcome" or "Ordinary outcome")
```

Each call advances the field once. Calling either function on the same field
shares its sequence; use different fields to keep unrelated choices independent.

## Reproducing a run

The same initial field value and ordered sequence of calls produce the same
results. Separate mods own separate fields even when their names match. Equal
seeds deliberately produce equal sequences; field names do not mix into a seed.
Reloading a saved profile resumes at its stored position. Disabling a mod
preserves its orphan state under the existing save compatibility rules.

To restart a sequence, call `sf2.state.set { route_rng = 12345 }` from a supported
callback. Do this deliberately at the start of a new run, rather than on every
result. Schema defaults do not replace an existing saved value. Renaming a
stream requires the same alias/migration treatment as any saved state field.

The stable sequence contract is **Eclipse RNG v1**: add `0x9e3779b9` to the
32-bit state, then mix a copy with xor-right-shift 16, multiply `0x85ebca6b`,
xor-right-shift 13, multiply `0xc2b2ae35`, and xor-right-shift 16. All arithmetic
wraps at 32 bits; the stored state is signed. `number` divides the unsigned
mixed word by `2^32`. `integer` accepts words below
`floor(2^32 / range) * range`, then returns `minimum + word % range`.

A successful draw updates profile state immediately. A later Lua error does
**not** roll back that draw or other state writes. This is not a transaction
covering mode progression and rewards, nor a guarantee that a crash before the
next game save preserves the draw. It does not seed Lua's `math.random`, native
combat randomness, or other mods. Use it for reproducible gameplay choices,
not secrets or security-sensitive tokens.
