---
title: Visuals and mod settings
description: Turn on the engine's optional fight visuals and give players switches for them.
---

Eclipse can draw several optional effects in fights: deeper background
parallax, weapon trails, depth haze, a rim light on the fighters, bloom,
ambient particles and a short impact effect on heavy hits. They are all off
until a mod asks for them. The engine does the drawing; a mod only chooses
which effects run and tunes their numbers with a typed table. No Lua runs
per frame.

Mod settings are simple on/off switches that appear under
**Options > Mod settings**, labelled with your mod's name. Link an effect to a
switch and players can turn it off without disabling your mod.

Things to know first:

- **Capabilities.** `sf2.visuals.*` requires `presentation.visuals`.
  `sf2.settings.toggle` requires `ui.settings`. `sf2.settings.get` needs no
  extra capability.
- **When.** Every function except `sf2.settings.get` is a registration: call it
  while your mod loads (at the top level of your scripts), not from callbacks.
- **One owner per effect.** Each effect can be configured by one mod. If two
  enabled mods configure the same effect, the second one fails to load with a
  conflict error. A mod may configure each effect only once.
- **Presentation only.** Effects and setting values are never written to the
  player's profile and are not part of the content fingerprint, so adding or
  removing a visuals mod never affects saves. Setting values are stored per
  installation.
- **Accessibility.** The impact effect on critical hits is also scaled by the
  player's **Critical hit shake** accessibility slider.
- **Defaults.** Every numeric field is optional. Omitted fields use the
  defaults shown. Out-of-range values and unknown fields raise an error.

A complete example is the **Chiaroscuro - Cinematic Visuals** mod in
`Mods/chiaroscuro`, which turns on every effect with one switch each.

## sf2.settings.toggle

**Signature:** `sf2.settings.toggle { id = "...", label = "...", description = "...", default = false }`

**Returns:** A setting handle. Pass it as `setting` to a visuals function or to
[`sf2.settings.get`](#sf2settingsget).

**When:** During loading. Register switches before the effects that use them.

**Requires:** `ui.settings`.

| Field | Required/default | Meaning |
| --- | --- | --- |
| `id` | Required | 1–64 lowercase ASCII letters, digits, `_` or `-`, unique within your mod. It keeps the player's choice stable across versions, so do not rename it. |
| `label` | Required | 1–48 characters shown in the Options row, after your mod's name. |
| `description` | `nil` | Up to 160 characters describing the switch. |
| `default` | `false` | Value used until the player changes it. |

A mod may register up to 16 switches. The Options page shows six per page.

```lua
local sf2 = require("sf2")
local trails = sf2.settings.toggle {
    id = "weapon_trails", label = "Weapon trails",
    description = "Short trails behind fast weapon swings.", default = true,
}
```

## sf2.settings.get

**Signature:** `sf2.settings.get(setting)`

**Returns:** `true` or `false`, the switch's current value.

**When:** Any time after the switch was registered, including from callbacks.
The value can change while the game runs.

**Requires:** A handle from `sf2.settings.toggle` created by the same script
context. No extra capability.

```lua
if sf2.settings.get(trails) then
    -- The player has trails on.
end
```

## sf2.visuals.background_depth

**Signature:** `sf2.visuals.background_depth { strength = 0.6, setting = handle }`

**Returns:** Nothing.

**When:** During loading.

**Requires:** `presentation.visuals`.

Background layers already move at their own parallax factor from the location
file (for example `0.1` for far layers and `1` for the floor). This effect uses
`factor ^ (1 + strength)` instead, so far layers move proportionally less and
the scene reads deeper. A layer never moves further than its original factor
allows, so the art never runs out at the edges. Layers with factor `0` or `1`
and above, the fighters' own layer and everything in front of it keep their
original movement.

| Field | Default | Range |
| --- | --- | --- |
| `strength` | `0.6` | 0–2 |
| `setting` | `nil` | A setting handle; without one the effect is always on. |

```lua
sf2.visuals.background_depth { strength = 0.6, setting = depth }
```

## sf2.visuals.weapon_trails

**Signature:** `sf2.visuals.weapon_trails { lifetime = 0.11, min_speed = 900, full_speed = 2600, alpha = 0.55, color = "#RRGGBBAA", setting = handle }`

**Returns:** Nothing.

**When:** During loading.

**Requires:** `presentation.visuals`.

Draws a short ribbon from the main-hand weapon's grip to its tip. Each part of
the ribbon fades out over `lifetime` seconds, and only appears when the tip is
moving faster than `min_speed` (model units per second), at full strength from
`full_speed`. Fists and weapons shorter than a small blade length draw no trail.

| Field | Default | Range |
| --- | --- | --- |
| `lifetime` | `0.11` | 0.02–0.5 seconds |
| `min_speed` | `900` | 0–20000 |
| `full_speed` | `2600` | 1–40000, must be greater than `min_speed` |
| `alpha` | `0.55` | 0–1 |
| `color` | Fighter colour | `#RRGGBB` or `#RRGGBBAA`. Without it the trail follows the fighter's colour, including perk tints. |
| `setting` | `nil` | A setting handle. |

```lua
sf2.visuals.weapon_trails { lifetime = 0.12, alpha = 0.5, setting = trails }
```

## sf2.visuals.depth_haze

**Signature:** `sf2.visuals.depth_haze { strength = 0.4, setting = handle }`

**Returns:** Nothing.

**When:** During loading.

**Requires:** `presentation.visuals`.

Fades background layers toward the colour of the location's farthest layer.
The farther a layer is, the stronger its haze (`strength × (1 − factor)`). The
fighters' layer and the foreground are never hazed.

| Field | Default | Range |
| --- | --- | --- |
| `strength` | `0.4` | 0–1 |
| `setting` | `nil` | A setting handle. |

```lua
sf2.visuals.depth_haze { strength = 0.35, setting = haze }
```

## sf2.visuals.rim_light

**Signature:** `sf2.visuals.rim_light { offset = 2.5, alpha = 0.85, lighten = 0.35, setting = handle }`

**Returns:** Nothing.

**When:** During loading.

**Requires:** `presentation.visuals`.

Draws a thin lit edge on each fighter's upper-left side. Its colour is the
location's background colour, brightened toward white by `lighten`.

| Field | Default | Range |
| --- | --- | --- |
| `offset` | `2.5` | 0–12 screen pixels |
| `alpha` | `0.85` | 0–1 |
| `lighten` | `0.35` | 0–1 |
| `setting` | `nil` | A setting handle. |

```lua
sf2.visuals.rim_light { offset = 3, setting = rim }
```

## sf2.visuals.bloom

**Signature:** `sf2.visuals.bloom { threshold = 0.82, knee = 0.12, intensity = 0.7, setting = handle }`

**Returns:** Nothing.

**When:** During loading.

**Requires:** `presentation.visuals`.

Makes bright pixels glow. Pixels brighter than `threshold` bloom, with `knee`
softening the cut-off. This works on the whole picture, so very bright
background art also glows; raise `threshold` to limit it to effects.

| Field | Default | Range |
| --- | --- | --- |
| `threshold` | `0.82` | 0–2 |
| `knee` | `0.12` | 0–1 |
| `intensity` | `0.7` | 0–4 |
| `setting` | `nil` | A setting handle. |

```lua
sf2.visuals.bloom { threshold = 0.85, intensity = 0.6, setting = bloom }
```

## sf2.visuals.ambient_particles

**Signature:** `sf2.visuals.ambient_particles { density = 1, default_style = "dust", locations = { { match = { "word", ... }, style = "snow" } }, setting = handle }`

**Returns:** Nothing.

**When:** During loading.

**Requires:** `presentation.visuals`.

Adds drifting particles behind the fighters. The style is chosen from words in
the location's name: the name is lowercased and split on `_`, `-`, `:`, `/`,
spaces and digits, and the first rule with a matching word wins. For example
`new_year_24_china_dojo` gives the words `new`, `year`, `china` and `dojo`.
Locations that match no rule use `default_style`.

| Field | Default | Meaning |
| --- | --- | --- |
| `density` | `1` | 0–4, multiplies the number of particles. `0` draws none. |
| `default_style` | `"dust"` | `none`, `dust`, `snow`, `embers` or `petals`. |
| `locations` | `{}` | Up to 32 rules. Each has `match` (1–16 lowercase words using `a-z`, `0-9`, `_` or `-`) and `style`. |
| `setting` | `nil` | A setting handle. |

```lua
sf2.visuals.ambient_particles {
    default_style = "dust",
    locations = {
        { match = { "ny", "winter", "snow" }, style = "snow" },
        { match = { "volcano", "underworld" }, style = "embers" },
    },
    setting = particles,
}
```

## sf2.visuals.impact

**Signature:** `sf2.visuals.impact { critical = 1, head = 0.6, shock = 0.4, duration = 0.3, setting = handle }`

**Returns:** Nothing.

**When:** During loading.

**Requires:** `presentation.visuals`.

Adds a short radial blur and colour split when a native hit effect plays:
`critical` for critical hits (also scaled by the player's **Critical hit shake**
slider), `head` for head hits and `shock` for shocks. It fades out over
`duration` seconds.

| Field | Default | Range |
| --- | --- | --- |
| `critical` | `1` | 0–1 |
| `head` | `0.6` | 0–1 |
| `shock` | `0.4` | 0–1 |
| `duration` | `0.3` | 0.05–2 seconds |
| `setting` | `nil` | A setting handle. |

```lua
sf2.visuals.impact { critical = 1, head = 0, shock = 0, setting = impact }
```

## Verification limits

The API contract (fields, defaults, ranges, capabilities, conflicts and the
shipped Chiaroscuro package) is checked headlessly. The rendering itself is
only verified by playing the game.
