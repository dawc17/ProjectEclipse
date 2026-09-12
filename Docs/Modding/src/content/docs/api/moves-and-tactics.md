---
title: Moves, triggers, and opponent tactics
description: Author playable animation moves and program opponent decisions in Lua.
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
| `key_pressed` | Use the string literal; no constant alias. |

Conditions use `type` and optional `["not"] = true` (default `false`). Available types:

- `"perk"` (`sf2.moves.PERK`): requires a `perk` handle; optional `player` string.
- `"all"` / `"any"` (`ALL` / `ANY`): require a nonempty `conditions` array.
- `"current_animation"`, `"current_interval"`, `"item"` (`CURRENT_ANIMATION`, `CURRENT_INTERVAL`, `ITEM`): accept native `name`, `player`, `item_type`, and `item_subtype` strings, default empty.

- `"character"`: requires a registered `warrior` handle; matches only that character, including copies of its model parameters.
- `"keys"`: requires 1–14 unique `keys`, each `{ key = "Kick", press = "Tap" }`. `press` defaults to `Tap`; alternatives are `Hold` and `Release`. Keys are `Up`, `Up-Forward`, `Forward`, `Down-Forward`, `Down`, `Down-Back`, `Back`, `Up-Back`, `Punch`, `Kick`, `Ranged`, `Magic`, `RaidCharge`, and `Super`.

Character and key conditions use string literals, without constant aliases. Combine them with `key_pressed` to bind an authored move to a fighter's controls.

An interval accepts `type`, `name`, optional `start` and `["end"]` frame indices, and optional `attack`. At least one of `type` or `name` must be nonempty. Frame indices are integers from 0 to 100,000; when both are supplied, end must not precede start. Omitted bounds retain the native interval behavior. Use stored animation sample indices within the move's frame range. `mid_frames` changes interpolation time between samples, not the indices used for these bounds.

`attack` requires `type = "Attack"` and the following fields:

| Field | Contract/default |
| --- | --- |
| `edges` | Required array of 1–64 native rig edge names, each 1–128 characters. These are attacking body parts, not arbitrary mesh vertices. |
| `damage` | Finite multiplier 0–16, default 0. Applied through the selected native damage attribute. |
| `damage_type` | `UnarmedDamage` (default), `WeaponDamage`, `RangedDamage`, or `MagicDamage`. |
| `hit` | `High` (default), `Middle`, or `Low`. |
| `id` | Integer 0–999, default 0; native attack identity. |
| `impulse` | Optional `{x=0,y=0,z=0}` in native physics axes; each component finite and within ±100,000. |

See [Character authoring](../../guides/character-authoring/) for a complete exported character module and input/attack example. The registration API validates structure and bounds; verify edge names, contact timing, mirroring and damage in a fight.

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

## on_decide

Available since API **0.22**. Attach this function to `sf2.tactics.register` to
program an opponent using ordinary Lua. Keep `type = "tabular"` (the default)
and a native `template`, usually `"Standard"`, for fallback behavior.

**Signature:** `on_decide = function(memory, event) ... end`

**Returns:** An action from this call's `event.actions`, `"wait"` to request no
new action until the next decision, or `nil` to let the native tactic decide.
Do not construct action tables or retain them for a later decision.

**When:** The native AI reaches an eligible move decision, at most once per six
active simulation frames (10 Hz). Native uninterruptible intervals and response
waits still apply. `event.self` and `event.opponent` contain detached health,
maximum health, health-bar count and position snapshots, as described in the
[fighter reference](../fighter/). `event.frame` and `event.seconds` are the
fighter controller's simulation clock. `event.actions` is an array of currently
legal input-driven actions, each with a native `name` string. It may be empty.
The host filters move conditions, equipment availability and priority before
Lua sees this list; a selected action still goes through normal input dispatch.

**Requires:** `content.register` when registering the tactic and a warrior whose
`tactic` is `sf2.tactics.name(your_tactic)`. Only the tabular input controller
supports this callback. No raw model, animation object, or fighter mutation
capability is exposed. Snapshots can be edited locally; edits do not change
combat. At most 1024 action candidates are passed to one decision.

```lua
local sf2 = require("sf2")
local patient = sf2.tactics.register {
    id = "patient", template = "Standard",
    on_decide = function(memory, event)
        memory.next_attack = memory.next_attack or 0
        if event.seconds < memory.next_attack then return "wait" end
        if #event.actions == 0 then return nil end
        memory.next_attack = event.seconds + 1
        return event.actions[1]
    end,
}
-- In an owned warrior definition:
-- tactic = sf2.tactics.name(patient)
```

`memory` is a plain Lua table private to this native fighter controller and this
tactic. It survives decisions on that controller, not save/reload or controller
replacement. Persist deliberate profile data through the owned state API.
Closures at script scope are shared across fighters; use `memory` for isolated
AI state. Callback errors, invalid/stale actions and instruction-budget overruns
(200,000 instructions) disable this callback for that controller/tactic and use
native fallback; the first failure is diagnosed. Other fighters keep their own
AI. This callback is not a coroutine.

The [programmable AI example](https://github.com/dawc17/ProjectEclipse/tree/main/Mods/example.programmable-ai)
uses different pacing for three opponents. Isolated Lua tests cover choices,
memory isolation, stale actions and failures. Native combat and physical input
acceptance require a game playtest.

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
