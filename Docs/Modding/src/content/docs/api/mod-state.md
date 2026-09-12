---
title: Saved mod state
description: Store progress for your mod, update typed values, and migrate existing player data.
---

Use `sf2.state` for values owned by your whole mod, such as a completed tutorial
flag or a custom win count. For a shield that refreshes every round or an effect
attached to one item, use [behavior-instance state](../behavior-instances/) instead.

State is registered during startup but can only be read or changed after a player
profile loads. Put reads and writes inside a supported callback, not at the top
of your entrypoint.

For reproducible encounter choices, use [saved random streams](../random/)
backed by declared integer fields. They share this API's ownership, schema and
normal profile-save behavior.

## sf2.state.register

Define the names, types, initial values, and version of your saved data.

**Signature:** `sf2.state.register { version, fields?, aliases?, tombstones?, migrations? }`

**Requires:** `state.write`.

**When:** Entrypoint, once for the mod's schema.

**Returns:** `nil`.

| Field | Type | Default | Meaning |
| --- | --- | --- | --- |
| `version` | Positive integer | Required | Schema version; increase it when changing the saved shape. |
| `fields` | Schema table | Empty | Field names and type/default definitions. |
| `aliases` | String-to-string table | Empty | Historical field name → current field name. |
| `tombstones` | String array | Empty | Retired field names that must not be reused. |
| `migrations` | Integer-to-function table | Empty | Function for each old version that needs an upgrade step. |

```lua
sf2.state.register {
    version = 1,
    fields = {
        victories = { type = sf2.state.INTEGER, default = 0 },
        finished = { type = sf2.state.BOOLEAN, default = false },
        nickname = { type = sf2.state.STRING, required = false },
    },
}
```

`required` defaults to `true`. Required saved fields need defaults so a new player
has a valid starting state. A schema can contain up to 64 fields. Names are
1–64 ASCII letters, digits, or underscores; use names beginning with a letter
for convenient Lua dot syntax.

Available type constants are `sf2.state.NUMBER`, `INTEGER`, `BOOLEAN`, and
`STRING`. Numbers must be finite; integers must remain within Lua's exact integer
range, −9,007,199,254,740,991 through 9,007,199,254,740,991. Strings are limited
to 2,048 characters. Functions, nested tables, and asset handles cannot be saved
as field values.

## sf2.state.get

Read one value belonging to this mod.

**Signature:** `sf2.state.get(name)`

**Requires:** `state.read` and a registered, successfully loaded schema.

**When:** A supported callback after profile load.

**Returns:** A number, integer-valued number, boolean, or string; `nil` if no value
is present for that name. Invalid names or unavailable state raise errors.

```lua
-- Inside a callback, after the schema above has been registered:
local victories = sf2.state.get("victories")
sf2.log.info("Wins: " .. tostring(victories))
```

## sf2.state.set

Update one or more fields together. Unmentioned fields retain their values.

**Signature:** `sf2.state.set { field_name = value, ... }`

**Requires:** `state.write`.

**When:** A supported callback after profile load.

**Returns:** `nil`.

```lua
-- Inside a callback:
sf2.state.set { victories = 3, finished = true }
```

The entire batch is validated before it is applied. An unknown field or wrong
value type rejects the batch without partially applying its earlier entries.
Setting a Lua table entry to `nil` removes it from that table, so use `unset`
when you intend to clear a saved field.

## sf2.state.unset

Remove a field's explicit value and apply its schema's default, if it has one.

**Signature:** `sf2.state.unset(name)`

**Requires:** `state.write`.

**When:** A supported callback after profile load.

**Returns:** `nil`.

```lua
-- Inside a callback:
sf2.state.unset("nickname")
```

`name` must be declared in the schema. An optional field without a default
becomes absent. A default-backed field resets to its default. Clearing a required
field with no usable default is rejected.

## Upgrading saved data

For version 1 → 2, add a migration under key `[1]` and set `version = 2`:

```lua
sf2.state.register {
    version = 2,
    fields = {
        victories = { type = sf2.state.INTEGER, default = 0 },
        finished = { type = sf2.state.BOOLEAN, default = false },
    },
    aliases = { wins = "victories" },
    tombstones = { "old_hint" },
    migrations = {
        [1] = function(old)
            old.finished = old.finished or false
            -- Mutate old, or return a replacement primitive-value table.
        end,
    },
}
```

Migration functions run once per required version step, with bounded execution.
They receive a copy of saved primitive values. They may mutate that table and
return nothing, or return a replacement table. Every step and the final result
must validate before replacing saved data. A failure preserves the prior data;
a save written by a newer schema is not silently downgraded.

Normal game saves persist the state. Disabling or uninstalling a mod preserves
its saved values so reinstalling the same namespace can restore them.

During profile reset/loading, state is unbound and state operations are unavailable.
This does not delete saved values or registered field definitions. Activating the
selected profile binds its own saved values again; internal comparison copies do
not replace the active state binding.
