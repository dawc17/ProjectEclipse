---
title: Lua basics for modders
description: The small amount of Lua you need to read examples and write your first mod.
---

Eclipse runs Lua scripts through its built-in scripting runtime. Start with `scripts/main.lua` in a mod folder; a separate Lua installation is not needed. This guide explains the notation used throughout the wiki.

## Load the API and keep a value

```lua
local sf2 = require("sf2")
local price = 5
sf2.log.info("My mod is loading")
```

`local` creates a variable, a name for a value. `require("sf2")` gives you the Eclipse API. `sf2.log.info(...)` calls a function with the text inside parentheses. Lines beginning with `--` are comments; Eclipse ignores them.

## Use the right value type

| Type | Example | Common mistake |
| --- | --- | --- |
| String (text) | `"training_blade"` | Unquoted text is treated as a variable. |
| Number | `5`, `0.25` | `"5"` is text, not a number. |
| Boolean | `true`, `false` | Use lowercase, without quotes. |
| Missing value | `nil` | This is different from `false` and `0`. |
| Table | `{ level = 1 }` | Separate fields with commas. |
| Function | `function() ... end` | Close the function with `end`. |

An *integer* is a whole number. A *finite* number excludes infinity and invalid arithmetic results. Each API field documents its accepted type and range.

## Tables describe content

A table groups named fields:

```lua
local settings = {
    id = "training_blade",
    level = 1,
}
```

Read a field with `settings.level`. Trailing commas are allowed. A function call with one table can omit parentheses, so `sf2.items.register_weapon { ... }` means the same as `sf2.items.register_weapon({ ... })`.

Tables also hold ordered lists, called arrays:

```lua
local names = { "Blade", "Staff", "Spear" }
local first = names[1] -- Lua arrays begin at 1.
```

Keep API arrays dense: do not skip an index or insert `nil` between entries. Order can matter, especially for fight reward slots and mode fight sequences.

## Keep handles as values

A *handle* is a value returned by the API that refers to one particular item, sprite, or other definition. Pass it directly to the next function:

```lua
local icon = sf2.assets.sprite("sprites/weapon")
-- Use icon = icon in your weapon definition.
```

The string `"sprites/weapon"` is an asset ID. The variable `icon` is a sprite handle. The text `"icon"` is neither that variable nor that handle. This distinction explains many registration errors.

## Functions and callbacks

A function holds code to run later. A callback is a function that Eclipse calls at a documented event.

```lua
local function describe_health(fighter)
    local health = fighter.health
    if health > 0 then
        sf2.log.info("Fighter health: " .. tostring(health))
    end
end
```

`..` joins strings. `if ... then ... end` runs code conditionally. Use `==` to compare values; `=` assigns one. Use `and`, `or`, and `not` for logical expressions. Only `false` and `nil` count as false in Lua; `0` and `""` count as true.

Use dots for API functions, such as `sf2.log.info(...)`. Use the colon shown in the reference for fighter methods, such as `fighter:change_health(0.05)`: it passes the fighter as the method's receiver. Do not substitute one spelling for the other.

The function above is an illustration, not a registered callback. See [Behavior instances](../../api/behavior-instances/) to connect a function to a perk or enchantment. Fighter methods only work during the callback that supplies the fighter.

## Split a larger script

The entrypoint can load a module inside your mod's `scripts` directory:

```lua
local settings = require("settings") -- scripts/settings.lua
sf2.log.info(settings.message)
```

```lua
-- scripts/settings.lua
return { message = "My mod is loading" }
```

The loader is restricted to supported mod modules. It does not grant arbitrary filesystem access or allow you to load another mod's private scripts. To use another mod's public content, declare a dependency and use qualified IDs.

## Read errors one at a time

If Lua reports a syntax error, check the named file and line for a missing comma, quote, brace, or `end`. If an API call reports a wrong type or missing field, compare the supplied table with that function's reference. Fix the first error, restart Eclipse, and try again.

Continue with [Your first weapon](../first-weapon/) for complete files you can copy.
