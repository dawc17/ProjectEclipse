---
title: Moves, triggers, and opponent tactics
description: Define animation moves and configure the game's existing opponent AI.
---

These are advanced content APIs. They configure the native animation and AI systems; Lua combat callbacks are covered separately in [Combat callbacks](../combat-callbacks/). Begin with a working fight and change one move at a time.

## Shared move fields

Both move registration functions accept the following fields:

| Field | Meaning/default |
| --- | --- |
| `id` | Required local identifier. |
| `templates` | Array of mod move-template handles, default empty. |
| `core_templates` | Array of existing core template names, default empty. |
| `events`, `conditions`, `intervals` | Arrays described below, default empty. |
| `type` | Native move category string, default empty. |
| `priority` | Integer priority, default `0`. |
| `mid_frames`, `first_frame`, `end_frame` | Nonnegative integers, default `0`. |
| `mirror_node`, `tactic_equivalent`, `tactic_weapon` | Native animation/tactic name strings, default empty. |
| `looped`, `ends_stage` | Booleans, default `false`. |

Use existing core definitions as a reference for native names and animation nodes. A valid Lua table does not guarantee that an animation will suit the fighter skeleton.

Events can be a type string or `{ type = ..., name = "...", player = "..." }`. Table form requires `type`; `name` and `player` default empty. Supported types and their constants under `sf2.moves` are:

| Type | Constant |
| --- | --- |
| `animation_end`, `animation_start` | `ANIMATION_END`, `ANIMATION_START` |
| `interval_end`, `interval_start` | `INTERVAL_END`, `INTERVAL_START` |
| `hit`, `strike` | `HIT`, `STRIKE` |
| `every_frame`, `birth` | `EVERY_FRAME`, `BIRTH` |
| `round_stage_start`, `mod_expires` | `ROUND_STAGE_START`, `MOD_EXPIRES` |

Conditions use `type` and optional `["not"] = true` (default `false`). Available types:

- `"perk"` (`sf2.moves.PERK`): requires a `perk` handle; optional `player` string.
- `"all"` / `"any"` (`ALL` / `ANY`): require a nonempty `conditions` array.
- `"current_animation"`, `"current_interval"`, `"item"` (`CURRENT_ANIMATION`, `CURRENT_INTERVAL`, `ITEM`): accept native `name`, `player`, `item_type`, and `item_subtype` strings, default empty.

An interval is `{ type = "...", name = "..." }`; at least one of `type` or `name` must be nonempty. These names describe native animation intervals, not seconds.

## sf2.moves.register_template

**Signature:** `sf2.moves.register_template(definition)`

**Returns:** A move-template handle.

**When:** During mod loading, before moves that use the template.

**Requires:** `content.register`.

Registers reusable move settings using the shared fields above. It does not require an animation asset. A reference to a core template must match an existing native name.

```lua
local step_template = sf2.moves.register_template {
    id = "forward_step",
    core_templates = { "ForwardStep" },
}
```

## sf2.moves.register

**Signature:** `sf2.moves.register(definition)`

**Returns:** A move handle.

**When:** During mod loading.

**Requires:** `content.register`, plus dependencies for external templates or assets.

Accepts the shared move fields and a required `animation` binary-asset handle. The file must contain a supported native animation; renaming an arbitrary file does not convert it.

```lua
-- Supply assets/animations/opening.bytes in your mod.
-- step_template is the template registered in the preceding example.
local opening_step = sf2.moves.register {
    id = "opening_step",
    animation = sf2.assets.binary("animations/opening"),
    templates = { step_template },
    core_templates = { "Step", "Forward", "SoundStrike" },
    type = "MOVE",
    priority = 10,
    mid_frames = 2,
    first_frame = 3,
    mirror_node = "NHeel_1",
    intervals = { { type = "Block" }, { name = "Throwable" } },
}
```

This demonstrates the definition's shape. Choose event and condition restrictions suitable for the equipment, fighter, and fight that should use your move. Unrestricted moves can affect more fighters than intended.

## sf2.moves.register_trigger

**Signature:** `sf2.moves.register_trigger(definition)`

**Returns:** A trigger handle.

**When:** During mod loading.

**Requires:** `content.register`.

Requires `id`. `events`, `conditions`, and `actions` are arrays (default empty). Events and conditions use the formats above. Actions support:

| Action | Fields |
| --- | --- |
| `type = "sound"` | Required `audio` handle, `volume` default `1` (finite and nonnegative), `looped` default `false`. |
| `type = "hit_effect"` | Required `name` of an existing native hit effect. |

```lua
-- Supply assets/audio/step.wav (PCM16 WAV).
local sound_trigger = sf2.moves.register_trigger {
    id = "opening_step_sound",
    events = { {
        type = sf2.moves.ANIMATION_START,
        name = sf2.mod.id .. ":moves/opening_step",
    } },
    actions = { {
        type = "sound",
        audio = sf2.assets.audio("audio/step"),
        volume = 0.7,
    } },
}
```

Triggers use these supported native actions. For procedural gameplay logic, use a supported combat callback and its typed fighter methods.

## sf2.tactics.register

**Signature:** `sf2.tactics.register(definition)`

**Returns:** A tactic handle.

**When:** During mod loading, before warriors that use it.

**Requires:** `content.register`.

Requires `id`. `type` defaults to `"tabular"` (`sf2.tactics.TABULAR`); `"random"` (`RANDOM`) is also supported. `template` optionally names an existing native tactic to inherit, such as `"Standard"`. Optional `memory` has `strikes` (integer, default `0`) and `round_factor` (number, default `0`).

```lua
local training_ai = sf2.tactics.register {
    id = "training_ai",
    type = sf2.tactics.TABULAR,
    template = "Standard",
    memory = { strikes = 2, round_factor = 0.25 },
}
```

Optional value-table fields are `counter_attack`, `dodge`, `block`, `safe_attack`, `table_attack`, `cautious_movement`, `dodge_missiles`, and `dodge_magic`.

Optional weighted-animation arrays are `animation_weights`, `quick_attacks`, `evades`, and `expected_wait`. Each row selects a registered `move` handle or an existing `animation` name string, and accepts a `value` table. A supplied section replaces that inherited section; replacing an attack list with one unsuitable move can leave an opponent unable to act.

A value table accepts the following finite numbers, all defaulting to `0`: `base`, `counter_factor`, `damage_factor`, `health_factor`, `enemy_health_factor`, `animation_frames_factor`, `child_frames_factor`, `magic_bullet_factor`, `missile_bullet_factor`, `hit_factor`, `distance_factor`, `shift`, `limit`, `anti_limit`. `factor_type` defaults to `"linear"` (`sf2.tactics.LINEAR`); `"exponential"` (`EXPONENTIAL`) is also supported. These are native scoring factors rather than probability percentages. Preserve a known working template until you have tested your custom weights in a fight.

## sf2.tactics.name

**Signature:** `sf2.tactics.name(tactic)`

**Returns:** The tactic's qualified ID as a string.

**When:** During mod loading, after obtaining the tactic handle.

**Requires:** A valid tactic handle from the current scripting context; no additional capability.

Use the result in a warrior definition's `tactic` field. A string or a different kind of handle is not accepted.

```lua
local tactic_name = sf2.tactics.name(training_ai)
-- Use tactic = tactic_name inside sf2.warriors.register { ... }.
```
