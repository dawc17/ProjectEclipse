---
title: Locations and languages
description: Build an arena from sprite layers and register a selectable language.
---

Use localization files to translate your mod's text. Register a *locale* only when adding a language choice to the game. A *location* is the arena background and fighter placement used by a fight.

## sf2.locales.register

**Signature:** `sf2.locales.register(definition)`

**Returns:** The qualified locale ID as a string.

**When:** During mod loading.

**Requires:** `content.register`.

| Field | Meaning |
| --- | --- |
| `id` | Required local identifier. |
| `name` | Required display name. |
| `locale` | Required locale code. |
| `alias` | Optional legacy identifier, default empty. |
| `is_asian` | Optional boolean, default `false`; selects the corresponding text layout behavior. |
| `file_icon`, `file_icon_selected` | Optional existing icon names, default empty. |
| `loader_image`, `preloader_image` | Optional existing image names, default empty. |
| `fonts` | Optional font configuration described below. |

If `fonts` is supplied, `content`, `title`, and `button` are required font-name strings. `size_scale`, `line_spacing`, and `custom_line_spacing_scale` default to `1` and must be positive finite numbers. Font and image names must resolve to supported game resources; this function does not import a font file.

```lua
local language_id = sf2.locales.register {
    id = "custom_english",
    name = "Custom English",
    locale = "eng",
}
```

This registers the language option. Put translated strings in localization files separately; see [Localization](../localization-patches/).

## sf2.locations.register

**Signature:** `sf2.locations.register(definition)`

**Returns:** A location handle.

**When:** During mod loading.

**Requires:** `content.register`; declare dependencies for external assets.

| Field | Default and meaning |
| --- | --- |
| `id` | Required local identifier. |
| `color` | `"0x000000"`; background color string. |
| `width`, `height` | `1936`, `512`; positive arena dimensions. |
| `min_width` | Same as `width`; positive minimum width. |
| `wall`, `floor` | `200`, `80`; arena boundaries. |
| `position_y` | `0`; vertical offset. |
| `friction_force`, `grid_size` | `0`; physics/layout settings. Grid size cannot be negative. |
| `music` | Optional audio handle. |
| `layers` | Required, nonempty array of layer tables. |

Each layer has `type` (integer, default `1`), `factor` (number, default `1`), and `scaling` (boolean, default `false`). It needs images or fighter placements. Use `type = 2` for a layer with `fighters`.

An image requires a `sprite` handle. `x` and `y` default to `0`; positive `width` and `height` default to `1`. `opaque`, `flip_x`, `flip_y`, and `mask` are booleans defaulting to `false`. A fighter placement requires all four numbers: `player_x`, `player_y`, `enemy_x`, `enemy_y`. Coordinates and numeric settings must be finite.

```lua
local arena = sf2.locations.register {
    id = "training_arena",
    width = 1936, height = 512, floor = 80, wall = 200,
    layers = {
        {
            type = 1,
            images = { {
                sprite = sf2.assets.sprite(
                    "core:Textures/Locations/battlefield/battlefield_bg1.back_1"),
                width = 1936, height = 1024,
            } },
        },
        {
            type = 2,
            fighters = {
                player_x = 868, player_y = -94,
                enemy_x = 1068, enemy_y = -94,
            },
        },
    },
}
```

These dimensions are a starting point for this particular background. Check both fighter positions and camera framing in game when changing the art.

## sf2.locations.name

**Signature:** `sf2.locations.name(location)`

**Returns:** The location's qualified ID as a string.

**When:** During mod loading, after obtaining the location handle.

**Requires:** A valid location handle belonging to the current scripting context; no additional capability.

Converts a location handle to the name expected by a fight's `location` field. Passing a plain string or a handle of another kind is an error.

```lua
local arena_name = sf2.locations.name(arena)
-- Use location = arena_name inside sf2.fights.register { ... }.
```
