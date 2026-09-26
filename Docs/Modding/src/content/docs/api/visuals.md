---
title: Visuals and mod settings
description: Build your own fight effects from particles, overlays, trails, shadows, glints and screen grades, use the ready-made presets, and give players switches for them.
---

Mods can change how fights look in two ways:

- **Building blocks (`sf2.fx`)** let you design your own effects: particle
  emitters with your own sprites (including bursts at the point of each hit),
  overlays placed at a chosen depth (with built-in light-beam and glow
  shapes), trails on weapons or any two fighter nodes, contact shadows, blade
  glints, lights carried by weapons and magic, floor stains, and screen colour
  grading that can also fire on hits and knockouts.
  Any number of mods can add these, and they stack.
- **Presets (`sf2.visuals`)** are seven ready-made effects: deeper background
  parallax, weapon trails, depth haze, a rim light on the fighters, bloom,
  ambient particles and a short impact effect on heavy hits. You only tune
  their numbers, and each preset has one owner.

Everything is off until a mod asks for it. The engine does the drawing; a mod
describes effects with typed tables when it loads. No Lua runs per frame.

Mod settings are simple on/off switches that appear under
**Options > Mod settings**, labelled with your mod's name. Link an effect to a
switch and players can turn it off without disabling your mod.

Things to know first:

- **Capabilities.** `sf2.fx.*` and `sf2.visuals.*` require `presentation.visuals`.
  `sf2.settings.toggle` requires `ui.settings`. `sf2.settings.get` needs no
  extra capability.
- **When.** Every function except `sf2.settings.get` is a registration: call it
  while your mod loads (at the top level of your scripts), not from callbacks.
- **Stacking.** A mod may add up to 32 `sf2.fx` effects, and effects from
  different mods all run together. Each **preset** has one owner: if two enabled
  mods configure the same preset, the second fails to load with a conflict
  error.
- **Presentation only.** Effects and setting values are never written to the
  player's profile and are not part of the content fingerprint, so adding or
  removing a visuals mod never affects saves. Setting values are stored per
  installation.
- **Where they apply.** Fighter effects (trails, glints, node particles, the
  rim light) can also run on fighter previews in menus such as the shop and
  profile; for `sf2.fx` effects set `scenes = "everywhere"`. Location, hit and
  screen effects (overlays, location and hit particles, contact shadows, screen
  grades, background depth, depth haze, ambient particles, bloom and impact) run
  only in fights and the dojo.
- **Triggers.** Most effects run continuously. Hit particles, stains and screen
  grades with a `trigger` fire once per matching hit instead: `hit` is any unblocked
  hit, `critical` a critical hit, `block` a blocked hit (particles only) and
  `ko` the hit that empties a fighter's health.
- **Motion triggers.** Contact particles and screen grades can also fire from
  how a fighter moves: `land` when a fall ends on the floor, `knockdown` when
  the body hits the floor, `slide` repeatedly while the feet skid fast along the
  floor (contact particles only) and `wall` when a fighter plays a wall-hit
  recoil. See [contact particles](#contact-particles).
- **Accessibility.** The impact effect on critical hits is also scaled by the
  player's **Critical hit shake** accessibility slider.
- **Defaults.** Every numeric field is optional. Omitted fields use the
  defaults shown. Out-of-range values and unknown fields raise an error.

Two complete examples ship with Eclipse. **Custom FX Showcase**
(`Mods/example.custom-fx`) builds weapon sparks, kick trails, ground fog, a
dojo light wash and a screen grade from building blocks. **Chiaroscuro -
Cinematic Visuals** (`Mods/chiaroscuro`) builds weapon trails, light shafts
with drifting dust, a slow-motion knockout fade, film halation and grain, floor
stains, and light from weapons and magic from building blocks. It uses presets for background
depth, depth haze and the rim light, and gives each effect its own switch.

## Building blocks

Every `sf2.fx` function takes one table and returns the effect's qualified
name, `<mod-id>.<id>`. These fields work on every building block:

| Field | Required/default | Meaning |
| --- | --- | --- |
| `id` | Required | 1–64 lowercase ASCII letters, digits, `_` or `-`, unique among your mod's effects. |
| `setting` | `nil` | A handle from [`sf2.settings.toggle`](#sf2settingstoggle). The effect runs only while the switch is on. |
| `match` | Every location | Up to 32 lowercase words. The effect runs only in locations whose name contains one of them. Names are split on `_`, `-`, `:`, `/`, spaces and digits, so `new_year_24_china_dojo` gives `new`, `year`, `china` and `dojo`. |
| `exclude` | None | Up to 64 lowercase words. The effect never runs in a location whose name contains one of them, even if `match` also fits. Use it for "everywhere except" effects. |

Colours are `#RRGGBB` or `#RRGGBBAA` strings. `blend = "additive"` makes an
effect glow (it adds light); the default `"alpha"` paints over what is behind.
Distances and speeds are in location units, where a fighter is roughly 300
units tall; speeds are units per second.

**Fighter nodes.** Trails and node particles name points on a fighter. Useful
names, taken from the game's own move data:

| Node | Where |
| --- | --- |
| `NPivot` | Centre of the body |
| `NTop`, `NNeck`, `NChest`, `NStomach` | Head top, neck, chest, stomach |
| `NWrist_1`, `NWrist_2`, `NKnuckles_1`, `NKnuckles_2` | Wrists and fists (`_1` main side, `_2` other side) |
| `NKnee_1`, `NKnee_2`, `NHeel_1`, `NHeel_2`, `NToe_2` | Legs and feet |
| `Weapon-Node2_1`, `Weapon-Node2_2` | Near the main-hand and off-hand weapon |
| `Magic-Node2_1`, `Ranged-Node2_1` | Magic and ranged attachment points |

A node a fighter does not have is skipped for that fighter.

## sf2.fx.particles

**Signature:** `sf2.fx.particles { id = "...", placement = "behind", color = "#RRGGBBAA", count = 100, ... }`

**Returns:** The effect's name, `<mod-id>.<id>`.

**When:** During loading.

**Requires:** `presentation.visuals`. A `sprite` must be a handle from
`sf2.assets.sprite`.

Adds a particle emitter. With `placement = "background"`, `"behind"` or
`"front"` the particles fill the location; with `placement = "node"` (or just
`node = "..."`) they stream from one node of each selected fighter, and keep
flowing behind it as it moves. With `placement = "hit"` nothing plays until a
fighter is struck: then `count` particles burst from the contact point, fly
outward at `speed_min`–`speed_max`, fall with `gravity` and fade out. For hit
particles, `fighters` selects the fighter being struck. With
`placement = "contact"` the particles burst where a fighter meets the floor or
a wall; see [contact particles](#contact-particles).

| Field | Default | Meaning |
| --- | --- | --- |
| `placement` | `"behind"` (`"node"` when `node` is set) | `background`: on the background layer nearest `depth`. `behind`: just behind the fighters. `front`: in front of the fighters. `node`: at a fighter node. `hit`: a burst at each matching hit. `contact`: a burst at each matching landing, knockdown, skid or wall impact. |
| `node` | — | Required for node placement. See the node table. |
| `fighters` | `"both"` | Node placement only: `both`, `player` or `opponent`. In menu previews the fighter counts as the player. |
| `scenes` | `"fights"` | Node placement only: `everywhere` also runs on menu previews. |
| `sprite` | Soft dot | Your particle image. |
| `color`, `end_color` | White, `nil` | Start colour, and the colour each particle fades to by the end of its life. |
| `blend` | `"alpha"` | `alpha` or `additive`. |
| `count` | `100` | 1–1000 particles alive at once. |
| `lifetime_min`, `lifetime_max` | `6`, `10` | 0.05–60 seconds; min must not exceed max. |
| `size_min`, `size_max` | `3`, `6` | 0.1–400 units. |
| `velocity_x_min`, `velocity_x_max` | `-10`, `10` | Horizontal speed range, −2000 to 2000; positive is right. |
| `velocity_y_min`, `velocity_y_max` | `-10`, `10` | Vertical speed range; positive is up. |
| `noise` | `5` | 0–500 random wandering. |
| `spin` | `0` | 0–1: random start rotation, 1 is a full turn. |
| `depth` | `0.3` | 0–1 for background placement: 0 nearest the fighters, 1 the farthest layer. |
| `area_width`, `area_height` | `1.1`, `1.1` | 0–2: emission area as a fraction of the location size. |
| `radius` | `20` | 0–500: emission radius around a node, hit or contact point. |
| `x`, `y` | `0`, `0` | Location placements: centre of the emission area, as an offset from the location centre in location units; up is positive. A stage is about 2300 × 512 units, with the floor about 80 units above its bottom edge. |
| `trigger` | `"hit"` for hit placement, `"land"` for contact placement | Hit placement: `hit`, `critical`, `block` or `ko`. Contact placement: `land`, `knockdown`, `slide` or `wall`. Other placements run continuously and do not accept a trigger. |
| `speed_min`, `speed_max` | `0`, `0` | 0–5000: outward burst speed of hit and contact particles; min must not exceed max. The `velocity_*` ranges are added on top. |
| `gravity` | `0` | −5000 to 5000: downward pull on hit and contact particles in units per second squared; negative values rise. |

Particles appear only in fights (and on previews for node particles with
`scenes = "everywhere"`). Every particle fades in and out.

```lua
sf2.fx.particles {
    id = "blade_sparks", node = "Weapon-Node2_1", scenes = "everywhere",
    blend = "additive", color = "#FFC060", end_color = "#FF400000",
    count = 40, lifetime_min = 0.3, lifetime_max = 0.7,
    velocity_y_min = 10, velocity_y_max = 60, radius = 6,
}

-- Sparks thrown from the contact point of every unblocked hit.
sf2.fx.particles {
    id = "hit_sparks", placement = "hit", trigger = "hit", blend = "additive",
    color = "#FFD27A", end_color = "#FF5A1E00", count = 10,
    lifetime_min = 0.18, lifetime_max = 0.35, size_min = 2, size_max = 5,
    speed_min = 250, speed_max = 700, gravity = 900, radius = 4,
}
```

### Contact particles

Contact particles (`placement = "contact"`) are bursts that play where a fighter
meets the arena: dust from the floor, or grit knocked off a wall. Nothing plays
until the fighter's movement matches the `trigger`, then `count` particles burst
at the contact point, just in front of the fighter, and behave like hit
particles (`speed_*`, `gravity`, `velocity_*`, `radius`, fading out).

| Trigger | Fires when | Where |
| --- | --- | --- |
| `land` (default) | A fall from more than about 45 units above the floor ends with the feet on the floor. Small hops do not count. | On the floor under the lowest part of the body. |
| `knockdown` | The body's centre drops below about 60 units above the floor while falling fast. At most once every 0.6 seconds per fighter. | On the floor under the body's centre. |
| `slide` | The feet are on the floor while the body moves along it faster than about 420 units per second: skids, pushback and dashes. Repeats about every 0.07 seconds, so keep `count` small. | On the floor under the feet. |
| `wall` | The fighter starts a wall-hit recoil: the `WallHit` or `WallHitFall` move the game plays when a fighter is knocked into the arena wall. Walking or being pushed against a wall does not count. | At the edge of the body facing the nearer wall. |

A standing fighter is about 300 units tall, for scale. `fighters` selects the
fighter that is moving. Contact particles run in fights; with
`scenes = "everywhere"` landings, knockdowns and skids also play on menu
previews, but `wall` needs a fight's arena walls.

The triggers are read from the fighter's animated pose, not from the fight's
rules, so they only add presentation: they never change the fight. They use
game time, so nothing fires while the game is paused. A fighter placed at a new
position (such as at the start of a round) does not count as sliding.

```lua
-- Dust from heavy landings, and debris from wall impacts.
sf2.fx.particles {
    id = "landing_dust", placement = "contact", trigger = "land",
    color = "#C9B89A66", end_color = "#C9B89A00", count = 12, radius = 16,
    lifetime_min = 0.45, lifetime_max = 0.9, size_min = 16, size_max = 34,
    speed_min = 10, speed_max = 40, velocity_x_min = -140, velocity_x_max = 140,
    velocity_y_min = 15, velocity_y_max = 60, noise = 15,
}

sf2.fx.particles {
    id = "wall_debris", placement = "contact", trigger = "wall",
    color = "#5E4E3EFF", end_color = "#5E4E3E00", count = 16, radius = 20, spin = 1,
    lifetime_min = 0.5, lifetime_max = 0.9, size_min = 3, size_max = 8,
    speed_min = 200, speed_max = 480, gravity = 900,
}
```

## sf2.fx.overlay

**Signature:** `sf2.fx.overlay { id = "...", placement = "background", depth = 0.3, sprite = handle, color = "#RRGGBB", alpha = 1, ... }`

**Returns:** The effect's name, `<mod-id>.<id>`.

**When:** During loading.

**Requires:** `presentation.visuals`.

Places an image or a solid colour in the location. In the background it moves
with the background layer nearest `depth`, so it shares that layer's parallax;
in front it moves with the fighters' layer and sits in front of them.

| Field | Default | Meaning |
| --- | --- | --- |
| `placement` | `"background"` | `background` or `front`. |
| `depth` | `0.3` | 0–1 for background placement: 0 nearest the fighters, 1 the farthest layer. |
| `sprite` | Solid colour | Your image. Without one the overlay uses `shape`. |
| `shape` | `"rect"` | Built-in art used when there is no `sprite`: `rect` is a flat colour, `shaft` a soft vertical light beam that is brightest at the top, and `glow` a soft round light. A shaft or glow without a size is a quarter of the stage wide (a shaft is also the full stage height). |
| `color` | White | Multiplies the image, or is the flat colour. |
| `alpha` | `1` | 0–1 extra opacity. |
| `blend` | `"alpha"` | `alpha` or `additive`. |
| `x`, `y` | `0`, `0` | Offset from the location centre; up is positive. |
| `width`, `height` | `0` | Size in location units. `0` for either covers the whole view (see `shape` for beams and glows). |
| `angle` | `0` | −180 to 180 degrees; positive turns counter-clockwise. Tilt a `shaft` to slant the light. |
| `flicker` | `0` | 0–1: how far the opacity wavers, like a flame. |
| `flicker_speed` | `6` | 0.1–30: how fast the flicker moves. |

```lua
sf2.fx.overlay {
    id = "dojo_glow", match = { "dojo" }, placement = "background", depth = 0.8,
    color = "#FFB060", blend = "additive", alpha = 0.18,
}
```

## sf2.fx.trail

**Signature:** `sf2.fx.trail { id = "...", weapon = true, color = "#RRGGBB", ... }` or `sf2.fx.trail { id = "...", nodes = { "NKnee_2", "NHeel_2" }, ... }`

**Returns:** The effect's name, `<mod-id>.<id>`.

**When:** During loading.

**Requires:** `presentation.visuals`. Give exactly one of `weapon = true` or
two `nodes`.

Draws a fading ribbon swept by a line on the fighter. `weapon = true` follows
the weapon's blade (each hand separately). `nodes` gives the inner end first
and the moving end second. The ribbon only appears while the moving end is
faster than `min_speed`, at full strength from `full_speed`.

| Field | Default | Meaning |
| --- | --- | --- |
| `weapon` | `false` | Follow the weapon blade. |
| `nodes` | — | Two node names. See the node table. |
| `fighters` | `"both"` | `both`, `player` or `opponent`. |
| `scenes` | `"fights"` | `everywhere` also runs on menu previews. |
| `color` | Fighter colour | Ribbon colour. Without it the trail follows the fighter's colour, including perk tints. |
| `blend` | `"alpha"` | `alpha` or `additive`. |
| `lifetime` | `0.11` | 0.02–1 seconds. |
| `min_speed`, `full_speed` | `900`, `2600` | Speeds of the moving end; `full_speed` must be greater. |
| `alpha` | `0.55` | 0–1 at the moving end. |
| `start_alpha` | `0.35` | 0–1, fraction of `alpha` at the inner end. |

```lua
sf2.fx.trail {
    id = "kick_trail", nodes = { "NKnee_2", "NHeel_2" }, fighters = "player",
    color = "#6FB8FF", blend = "additive", lifetime = 0.15, min_speed = 700,
}
```

## sf2.fx.screen

**Signature:** `sf2.fx.screen { id = "...", trigger = "always", saturation = 1, contrast = 1, brightness = 0, tint = "#RRGGBB", tint_strength = 0, vignette = 0, ... }`

**Returns:** The effect's name, `<mod-id>.<id>`.

**When:** During loading.

**Requires:** `presentation.visuals`.

Grades the whole fight picture, in the locations its `match` and `exclude`
allow. When several grades are active they combine: saturation and contrast
multiply, brightness adds, tints layer in load order, halation adds, and the
strongest vignette, grain and accent win.

With a `trigger` other than `always`, the grade is off until a matching hit or,
for `land`, `knockdown` and `wall`, a matching movement by either fighter (see
[contact particles](#contact-particles) for when they fire).
It then applies fully for `hold` seconds and eases smoothly back to nothing
over `duration` seconds. Each new matching hit or movement restarts it. Hold and duration are
measured in real time, so they are not stretched by slow motion.

| Field | Default | Meaning |
| --- | --- | --- |
| `saturation` | `1` | 0–2. 0 is black and white. |
| `contrast` | `1` | 0–2. |
| `brightness` | `0` | −1 to 1. |
| `tint`, `tint_strength` | `nil`, `0` | A colour multiplied into the picture, and how strongly (0–1). |
| `vignette` | `0` | 0–1 darkening toward the edges. |
| `vignette_x`, `vignette_y` | `0`, `0` | −1 to 1: moves the vignette's centre; `-0.35, 0.25` centres it up and to the left, so the lower right is darkest. |
| `trigger` | `"always"` | `always`, `hit`, `critical`, `ko`, `land`, `knockdown` or `wall`. `slide` is not accepted because it repeats while a fighter skids. |
| `duration` | `0.25` | 0.02–10 seconds for a triggered grade to fade out. |
| `hold` | `0` | 0–10 seconds a triggered grade stays at full strength first. |
| `time_scale` | `1` | 0.05–1, triggered grades only: game speed while the grade is at full strength. Speed returns to normal as the grade fades, so slow motion lasts exactly as long as the grade. The slowest active grade wins. It never overrides a pause or another speed change already in effect. |
| `grain` | `0` | 0–1 fine moving film grain. |
| `halation`, `halation_threshold` | `0`, `0.75` | 0–2 glow strength around bright areas, and the brightness (0–2) where the glow starts. |
| `halation_color` | `"#FF9E6B"` | Colour of the halation glow. |
| `flicker`, `flicker_speed` | `0`, `6` | 0–1 irregular brightness wobble like firelight, and its speed (0.1–30). |
| `accent`, `accent_strength`, `accent_width` | `nil`, `0`, `0.08` | A colour whose hue keeps its saturation when the grade removes colour, how strongly (0–1), and how close a hue must be (0.01–0.5 of the colour wheel). |

```lua
sf2.fx.screen {
    id = "cinema_grade", saturation = 0.9, contrast = 1.08,
    tint = "#E8F0FF", tint_strength = 0.3, vignette = 0.35,
}

-- On the knockout hit, slow down and drain every colour but red, hold, then return.
sf2.fx.screen {
    id = "knockout_fade", trigger = "ko", saturation = 0, contrast = 1.15,
    accent = "#B01010", accent_strength = 1, accent_width = 0.06,
    hold = 1.2, duration = 1.6, time_scale = 0.3, -- slow motion while grey
}
```

## sf2.fx.shadow

**Signature:** `sf2.fx.shadow { id = "...", alpha = 0.45, width = 150, height = 28, fade_height = 350, min_scale = 0.35, color = "#000000", fighters = "both" }`

**Returns:** The effect's name, `<mod-id>.<id>`.

**When:** During loading.

**Requires:** `presentation.visuals`.

Draws a soft oval on the floor under each selected fighter, just behind the
fighter's body. As the fighter's lowest point rises above the floor, the shadow
shrinks toward `min_scale` and fades out, reaching nothing at `fade_height`.
The floor is the lowest point any fighter's skeleton has reached in the fight,
so it settles as soon as the fighters stand. Shadows run in fights only.

| Field | Default | Meaning |
| --- | --- | --- |
| `alpha` | `0.45` | 0–1 opacity on the ground. |
| `width`, `height` | `150`, `28` | Oval size in fighter units (1–2000 and 1–1000). A standing fighter is about 300 units tall. |
| `fade_height` | `350` | 1–5000: height above the floor at which the shadow has faded out. |
| `min_scale` | `0.35` | 0–1: shadow size at `fade_height`, as a fraction of full size. |
| `color` | Black | Shadow colour; its alpha multiplies `alpha`. |
| `fighters` | `"both"` | `both`, `player` or `opponent`. |

```lua
sf2.fx.shadow { id = "contact_shadow", alpha = 0.42, width = 150, height = 26, fade_height = 320 }
```

## sf2.fx.glint

**Signature:** `sf2.fx.glint { id = "...", interval = 3, duration = 0.35, size = 22, alpha = 0.9, max_speed = 250, color = "#FFF7E6", fighters = "both", scenes = "fights" }`

**Returns:** The effect's name, `<mod-id>.<id>`.

**When:** During loading.

**Requires:** `presentation.visuals`.

Every `interval` seconds or so, a small four-pointed star of light runs along
one of the fighter's weapon blades, from near the grip to the tip. It swells
and fades over `duration`. A glint starts only while the blade's tip is moving
slower than `max_speed`, so it appears on a held weapon, not during swings. It
uses the same blades as weapon trails; bare hands have none. It draws in front
of the fighter with additive light.

| Field | Default | Meaning |
| --- | --- | --- |
| `interval` | `3` | 0.2–60 seconds between glints (varied by ±30%). |
| `duration` | `0.35` | 0.05–3 seconds for one glint to travel and fade. |
| `size` | `22` | 1–400: length of the star's arms in fighter units. |
| `alpha` | `0.9` | 0–1 peak brightness. |
| `max_speed` | `250` | 0–20000: fastest tip speed at which a glint may start. |
| `color` | Warm white | Glint colour. |
| `fighters` | `"both"` | `both`, `player` or `opponent`. |
| `scenes` | `"fights"` | `everywhere` also runs on menu previews. |

```lua
sf2.fx.glint { id = "blade_glint", scenes = "everywhere", interval = 4, duration = 0.4, size = 26 }
```

## sf2.fx.light

**Signature:** `sf2.fx.light { id = "...", source = "weapon", weapons = { "fire" }, color = "#FFB061", radius = 320, intensity = 1, fighter_light = 0.8, glow = 0.3, glow_size = 380, flicker = 0.15, flicker_speed = 8, fighters = "both", scenes = "fights" }`

**Returns:** The effect's name, `<mod-id>.<id>`.

**When:** During loading.

**Requires:** `presentation.visuals`.

Makes something carry a light. With `source = "weapon"`, every fighter whose
equipped weapon matches one of the `weapons` words carries a light at each weapon
hand (`Weapon-Node2_1` and `Weapon-Node2_2`). A word matches when it appears
anywhere in the weapon's item name or subtype, ignoring case: `"fire"` matches
`WEAPON_FIRE_BATONS` and the `FireBatons` subtype. With `source = "magic"`, a
fighter casting magic (any move with the native `MagicPlayer` template) carries
a light at its magic hand, and every magic projectile in flight carries one at
its centre. A projectile's light goes out the moment it strikes something
(hit or blocked) or starts its end animation, and fighters it was lighting
lose that light at once.

Each light does two things:

- **It lights the stage.** A soft additive glow of `glow_size` is drawn just
  behind the fighter at the light.
- **It lights the fighters.** Every fighter within `radius`, including the one
  carrying it, gets a lit edge on the side facing the light, in the light's
  colour. The strength falls off with the square of the distance. Several lights
  blend by strength. This uses the rim-light edge, so it works with or without
  [`sf2.visuals.rim_light`](#sf2visualsrim_light); with the preset, the lit
  edge turns from its usual side toward the light.

Fighters are lit by the lights from the previous rendered frame, so every
fighter sees every light regardless of update order.

| Field | Default | Meaning |
| --- | --- | --- |
| `source` | `"weapon"` | `weapon` or `magic`. |
| `weapons` | — | Required for weapon lights, not allowed for magic: up to 32 lowercase words found in glowing weapons' names. |
| `color` | Warm orange | Light colour. |
| `radius` | `320` | 10–5000: how far the light reaches fighters, in fighter units (a standing fighter is about 300 tall). |
| `intensity` | `1` | 0–2 overall strength. |
| `fighter_light` | `0.8` | 0–1: how strongly it lights fighters. `0` only lights the stage. |
| `glow`, `glow_size` | `0.3`, `380` | 0–1 opacity and 1–5000 size of the stage glow. `glow = 0` draws none. |
| `flicker`, `flicker_speed` | `0.15`, `8` | 0–1 irregular waver of the strength, and its speed (0.1–30). |
| `fighters` | `"both"` | Whose weapons or magic carry the light. A projectile counts as the fighter who cast it. |
| `scenes` | `"fights"` | Weapon lights only: `everywhere` also lights menu previews. |

```lua
sf2.fx.light {
    id = "fire_weapons", source = "weapon", scenes = "everywhere",
    weapons = { "fire", "flame", "lava" }, color = "#FF8A3A",
    radius = 340, glow = 0.3, flicker = 0.25, flicker_speed = 9,
}
sf2.fx.light { id = "magic_light", source = "magic", color = "#C8B8FF", radius = 420, intensity = 1.2 }
```

## sf2.fx.stain

**Signature:** `sf2.fx.stain { id = "...", trigger = "hit", color = "#590808", alpha = 0.8, count = 3, size_min = 10, size_max = 26, spread = 40, flatten = 0.35, limit = 60, sprite = handle, blend = "alpha", fighters = "both" }`

**Returns:** The effect's name, `<mod-id>.<id>`.

**When:** During loading.

**Requires:** `presentation.visuals`. A `sprite` must be a handle from
`sf2.assets.sprite`.

Leaves splats on the floor under matching hits. Each hit drops `count` splats
on the floor below the contact point, scattered up to `spread` to either side,
flattened by `flatten` so they lie on the ground. They stay where they landed
until the next round begins. When more than `limit` of this effect's splats
exist, the oldest go first. Blocked hits never stain. Without a sprite, three
built-in splat shapes are used at random. The floor is found the same way as for
[`sf2.fx.shadow`](#sf2fxshadow). Stains run in fights only.

| Field | Default | Meaning |
| --- | --- | --- |
| `trigger` | `"hit"` | `hit`, `critical` or `ko`. |
| `fighters` | `"both"` | Which struck fighter leaves stains. |
| `color` | Dark red | Stain colour; its alpha multiplies `alpha`. |
| `alpha` | `0.8` | 0–1 opacity (each splat varies slightly). |
| `count` | `3` | 1–12 splats per hit. |
| `size_min`, `size_max` | `10`, `26` | 1–400 splat width in fighter units; min must not exceed max. |
| `spread` | `40` | 0–400: horizontal scatter around the hit. |
| `flatten` | `0.35` | 0.1–1: splat height as a fraction of its width. |
| `limit` | `60` | 1–200 splats of this effect kept at once. |
| `sprite`, `blend` | Built-in, `"alpha"` | Your own splat image, and `alpha` or `additive`. |

```lua
sf2.fx.stain { id = "hit_stains", color = "#4A0606", count = 2, size_min = 8, size_max = 20, limit = 50 }
sf2.fx.stain { id = "knockout_stain", trigger = "ko", count = 5, size_min = 20, size_max = 42 }
```

## Presets

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

The presets below are quicker to set up but fixed in shape. Use them as-is or
as a starting point, and build anything else from the blocks above.

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

**Signature:** `sf2.visuals.rim_light { offset = 2.5, alpha = 0.85, lighten = 0.35, ink = 0, ink_color = "#12081C", setting = handle }`

**Returns:** Nothing.

**When:** During loading.

**Requires:** `presentation.visuals`.

Draws a thin lit edge on each fighter's upper-left side. In fights its colour
is the location's background colour; in menu previews such as the shop and
profile it is a warm neutral light. Either way it is brightened toward white
by `lighten`. With `ink` above 0, a fighter's rim eases toward `ink_color`
while that fighter casts magic (any move with the native `MagicPlayer`
template), so the lit edge becomes a dark outline, then eases back.

| Field | Default | Range |
| --- | --- | --- |
| `offset` | `2.5` | 0–12 screen pixels |
| `alpha` | `0.85` | 0–1 |
| `lighten` | `0.35` | 0–1 |
| `ink` | `0` | 0–1: how far the rim turns to `ink_color` during magic casts. |
| `ink_color` | `"#12081C"` | `#RRGGBB` or `#RRGGBBAA` ink colour. |
| `setting` | `nil` | A setting handle. |

```lua
sf2.visuals.rim_light { offset = 3, ink = 0.85, ink_color = "#1A0C26", setting = rim }
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

The API contract (fields, defaults, ranges, triggers, shapes, capabilities,
conflicts and the shipped Chiaroscuro package) is checked headlessly. The
rendering itself, including hit timing, floor detection for shadows and glint
placement, and when the motion triggers (`land`, `knockdown`, `slide`, `wall`)
fire, is only verified by playing the game. The `land`, `knockdown` and `slide`
thresholds may be tuned.
