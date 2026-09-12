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
| `dojo` | `false`; opt this location into saved dojo selection (API 0.26). At most 256 choices across active mods. |
| `music` | Optional single audio handle; cannot be combined with nonempty `music_choices`. |
| `music_choices` | Optional dense array of up to 16 distinct audio handles (API 0.25). Empty/omitted means no choices. |
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


## Animated image curves

API 0.24 adds optional `motion_x`, `motion_y`, `rotation` and `opacity` tables to
location images. Each is `{ offset = 0, points = { ... } }`. Points require
`period` (seconds) and `value`; `ease` defaults to zero. Curves loop from the last
point back to the first. Each period is the time from that point to the next.
Offset advances the initial phase in seconds.

Position values are displacements in native arena units; positive Y motion uses
the native effect's upward axis (the image's base `y` coordinate is inverted by
the location loader). Rotation uses native degrees. Opacity values are percentages
from 0 to 100. `ease = 0` gives linear interpolation. Other values use the native
quadratic curvature coefficient, not a named easing preset, and can overshoot.

```lua
local drift = {
    offset = 0.25,
    points = { { period = 1, value = -8 }, { period = 1, value = 8 } },
}
-- Inside a location layer's images array:
local image = {
    sprite = sf2.assets.sprite("my.mod:sprites/scenery/banner"),
    width = 128, height = 256,
    motion_y = drift,
    rotation = { points = { { period = 1, value = -2 }, { period = 1, value = 2 } } },
}
```

Each curve requires 2–64 points. Periods must be between 1/60 and 120 seconds,
and offsets between zero and the sum of periods. Values must be finite and in
-10000..10000; opacity narrows this to 0..100. Ease must be zero or have magnitude
0.0001..1000 to avoid unstable native coefficients. Zero-duration segments are
rejected because the native loop cannot safely advance an all-zero curve.
Animated images cannot be masks or marked opaque; flipping remains supported.

These are decorative picture effects using the native scenery clock. They do
not grant collision, damage, hazard timing, atlas animation or particle control.
Pause behavior must be tested in the intended scene; scenery timing is not the
combat callback clock. Curves participate in content fingerprints. Existing static
images keep their original projection. Numerical and Lua/projection tests pass;
full-game visual, pause and lifetime acceptance remains outstanding.


## Random fight music

Since API 0.25, set `music_choices` on a location to let the native game choose
one track at fight entry. Each entry has equal probability; a track may repeat
on the next entry. The chosen track loops. This is not a sequential playlist,
and selection is not tied to a mode's saved random stream.

```lua
-- Add this field to your location definition alongside its id and layers.
music_choices = {
    sf2.assets.audio("core:gamedata/music/fight1_samurai_spirit"),
    sf2.assets.audio("core:gamedata/music/fight2_blade_dance"),
},
```

Use audio handles, not filenames or pipe-separated strings. Declare dependencies
for tracks from another mod. Duplicate assets, sparse arrays, more than 16 entries,
wrong handle kinds and combining a nonempty array with `music` are rejected.
Both a location's single track and its choices take priority over the fight's
music setting. Omitting both retains the existing fight/default fallback.

The ordered choices are part of the content fingerprint. They do not save a live
playback position. Apply & Restart is required after changing the definition.
Native selection/projection tests pass; audible playback, volume/mute behavior
and scene transitions still need a game test. See the Animated Arena example.

## sf2.locations.select_dojo

**Signature:** `sf2.locations.select_dojo(location)`

**Returns:** `nil`.

**When:** After a game profile has loaded, normally from a selector's UI callback.
The new backdrop applies on the next dojo entry; it does not refresh an open dojo.

**Requires:** `presentation.dojo` and this mod's registered location handle with
`dojo = true`. UI creation separately requires `ui.create`.

```lua
-- arena is a location handle registered earlier with dojo = true.
-- Call inside your selector's on_click callback.
sf2.locations.select_dojo(arena)
sf2.ui.close(view)
```

The host writes the preference into the current profile's existing save DOM.
Normal game saving persists it; this operation does not force a disk flush.
The preference takes priority over the native quest-selected dojo, and never
changes ordinary battle locations. No fighter or inventory definition is changed.

The latest explicit selection replaces the previous preference. A mod can select
only its own registered choices. No selection is made automatically when a mod
loads. Invalid handles, non-dojo locations, unavailable profiles and missing
capability raise an error before changing the preference.

## sf2.locations.selected_dojo

**Signature:** `sf2.locations.selected_dojo()`

**Returns:** Saved qualified location ID string, or `nil` for the native default.

**When:** After a game profile has loaded.

**Requires:** `presentation.dojo`.

```lua
local selected = sf2.locations.selected_dojo()
local using_this_arena = selected == sf2.locations.name(arena)
```

This reports the saved preference, including a currently unavailable location.
It does not report the temporary fallback or the currently rendered scene.
When its provider mod is absent, the game uses the native dojo and preserves the
preference. Reenabling the provider restores it at the next dojo entry.

## sf2.locations.reset_dojo

**Signature:** `sf2.locations.reset_dojo()`

**Returns:** `nil`.

**When:** After a game profile has loaded, normally from a reset button.

**Requires:** `presentation.dojo`. The saved choice must belong to this mod or
already be empty; another mod's preference cannot be cleared by this function.

```lua
local selected = sf2.locations.selected_dojo()
if selected == nil or selected == sf2.locations.name(arena) then
    sf2.locations.reset_dojo()
end
```

Reset explicitly clears the preference; it preserves unrelated save data. The
native/default dojo returns on the next entry. Switching profiles and shutting
down unload the active binding. Unknown or malformed preference data is retained
and selection operations remain unavailable for that profile.

The Dojo Selector example opens a game-themed chooser from a custom map entry
using deferred mode preparation. Closing it cancels preparation without entering
a fight. Native menu insertion and automatic scene navigation are not supplied
by these functions. Registration, Lua UI callbacks and save-store tests pass;
full-game persistence, art rendering and scene transitions still need acceptance.
