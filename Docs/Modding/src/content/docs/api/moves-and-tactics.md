---
title: Moves, triggers, and opponent tactics
description: Author playable animation moves and program opponent decisions in Lua.
---

These are advanced content APIs. They configure the native animation and AI systems; Lua combat callbacks are covered separately in [Combat callbacks](../combat-callbacks/). Begin with a working fight and change one move at a time.

Native equipment can use a different AI table group from its animation subtype. Eclipse respects the shipped `TacticSubtype` metadata (for example, a `TwoHandedBlunt` weapon can use `TwoHanded` AI tables), falling back to `SubType` when it is absent. Item cloning preserves this distinction, and weapon changes update the fighter's own AI group. Animation and item-condition matching still use the actual subtype. Owned weapon registration supports an optional `tactic_subtype`; see [weapon registration](../equipment-shop-logging/#sf2itemsregister_weapon). You can also [override weapon AI groups](../items-progression-forge/#sf2itemsset_tactic_subtype). Managed tests cover parsing and group updates; live combat acceptance remains separate.

## Writing moves: the short form

Use plain declarative tables to describe moves. A condition names its kind,
points name the rig object, and a timeline groups actions by their frame or event.
The C# binding validates these tables and projects them into native move data;
no Lua helper library is required. Long-form move tables remain temporarily
accepted while test fixtures are migrated. New content should use the short form.

```lua
local sf2 = require("sf2")
local move = sf2.moves.register {
    id = "spell", animation = "animations/spell",
    core_templates = { "MagicPlayer", "Controlled" },
    events = "controlled", direction = "face_enemy",
    conditions = {
        { key = "Magic" }, { bullets = "MagicBullet", min = 1 },
        { not_mod = "Concussion" }, { not_mod = "Stun" },
        { controllable = true },
    },
    locks = { { item = "Skeleton", subtype = "Skeleton" } },
    intervals = { { name = "Uninterrupt", to = 30 } },
    timeline = {
        [3] = { sound = "snd_midsphere_start" },
        [7] = { add_bullets = "MagicBullet", amount = -1 },
        strike = { sound = { "snd_hit1", "snd_hit2" } },
    },
}
```

This is an authoring fragment: provide a compatible animation and the rest of
your spell's attack/projectile graph before using it in a fight.

Each condition has one kind key. Supported keys are `key`, `keys`, `mod`,
`interval`, `animation`, `stage`, `round_result`, `screen`, `actor`, `character`,
`perk`, `item`, `bullets`, `distance`, `direction`, `all`, and `any`. Prefix any
with `not_` to negate it. Conditions within a list are ANDed; `any` groups use OR.
Nested plain lists are spliced into the enclosing list, so shared condition
arrays can be reused. Nesting is limited to eight levels.

`{ controllable = true }` expands to the native readiness conditions: outside
`SemiUninterrupt` and `Uninterrupt`, round stage `Fight`, not simultaneously in
`$Move` and `SemiUninterrupt`, and not in `Physical`. It does not include stun,
concussion, equipment, or charge checks; declare those as needed.

For points, use `{ node = "NPivot", player = "Enemy", x = 5, y = 10 }` or
`{ wall = "Front", player = "Me" }`. Partless objects take the player directly:
`{ pivot = "Me" }`, `{ animation = "Parent" }`, `{ floor = "Me" }`, or
`{ com = "Me" }`. Use `true` to retain native default player selection, such as
`{ animation = true }`. A player cannot be supplied twice. The positioning
section below explains which objects each operation supports.

Use a single action or an array of actions under each timeline key:

```lua
local timeline = {
    [2] = {
        { sound = "snd_midsphere_start" },
        { effect = { name = "Glow", core_sequence = "mgc_magic_mid_sphere_start" } },
    },
    animation_end = { stop_effect = "Glow" },
}
```

Frames are emitted in ascending order, followed by events in this order:
`birth`, `round_stage`, `round_start`, `key_pressed`, `key_released`,
`animation_start`, `interval_start`, `every_frame`, `strike`, `hit`, `wall_hit`,
`interval_end`, `animation_end`, `mod_expires`, `round_end`. Actions under one
key retain their written order. The native runtime dispatches each trigger
separately, so order between different triggers does not affect execution.

| Short action | Meaning |
| --- | --- |
| `{ sound = "snd_hit1" }` or `{ sound = { "snd_hit1", "snd_hit2" } }` | Play one native sound or randomly choose from the ordered list. |
| `{ play_sound = "snd_m_pl_attack6", voice = "Male" }` | Play a native sound with an optional voice filter. |
| `{ stop_sound = "snd_blade_fury" }` | Stop the named native sound. |
| `{ effect = { name = "Glow", core_sequence = "mgc_magic_mid_sphere_start" } }` | Start a native effect with the payload described below. |
| `{ stop_effect = "Glow" }`, `{ stop_follow_effect = "Glow" }` | Stop an effect or detach its following behavior. |
| `{ projectile = { name = "Sphere", core_skeleton = "SkeletonMagic", copy_parent_type = "Magic" } }` | Create a native child projectile actor. |
| `{ add_bullets = "MagicBullet", amount = -1 }` | Change the current model's charge. |
| `{ delete_actor = "Me" }` | Delete the selected native actor. |
| `{ play_animation = move, player = "Child", child_name = "Hand" }` | Start a registered move on the selected actor. A string instead of the handle names a core animation. |
| `{ shake = { effect_time = 30, amplitude_x = 7, frequency_x = 1 } }` | Schedule native camera shake. |
| `{ try_on_end = true }` | Complete a shop preview. |

Other conveniences preserve the same native values:

- `events = "controlled"` means key press, `Uninterrupt` end, and animation end.
- `direction = "face_enemy"` uses `NPivot` on `Me` and `Enemy`.
- Intervals use `from`/`to`, for example `{ type = "Block", from = 20 }`.
- Damage terms use a map, for example `{ WeaponDamage = 0, UnarmedDamage = -10 }`.
  Terms emit in `WeaponDamage`, `RangedDamage`, `MagicDamage`, `UnarmedDamage` order.
- `tactic_distance` uses `distance`, `min`, `max`, `from`, and `to`.
- `animation = "animations/spell"` resolves the mod's binary asset; an existing
  binary handle is also accepted.

Only the compact move syntax is accepted. Legacy condition/event `type` fields, point `object` fields, interval `start`/`end`, damage-term arrays, tactic-distance `axis`, and scheduled move `actions` are rejected.

The move short form does not change quest actions, owned-audio trigger actions,
or guarded native interval patch selectors. Those have their own field contracts.

## Shared move fields

Both move registration functions accept the following fields:

| Field | Meaning/default |
| --- | --- |
| `id` | Required local identifier. |
| `templates` | Array of mod move-template handles, default empty. |
| `core_templates` | Array of existing core template names, default empty. |
| `events`, `conditions`, `intervals` | Arrays described below, default empty. |
| `locks` | Up to 64 native availability conditions, default empty. Same condition tables as `conditions`. |
| `align`, `direction` | Optional positioning/facing tables described below. |
| `type` | Native move category string, default empty. |
| `priority` | Integer priority, default `0`. |
| `mid_frames`, `first_frame`, `end_frame` | Nonnegative integers, default `0`. |
| `mirror_node`, `tactic_equivalent`, `tactic_weapon` | Native animation/tactic name strings, default empty. |
| `looped`, `ends_stage` | Booleans, default `false`. |

Use existing core definitions as a reference for native names and animation nodes. A valid Lua table does not guarantee that an animation will suit the fighter skeleton.

Events are strings such as `"animation_end"`, or tables such as
`{ interval_end = "Uninterrupt", player = "Me" }`. A table's kind key holds
the native name to match; `player` is optional. Use `events = "controlled"`
for key press, Uninterrupt end, and animation end together.

Supported event names are `animation_end`, `animation_start`, `interval_end`,
`interval_start`, `hit`, `strike`, `every_frame`, `birth`, `round_stage_start`,
`mod_expires`, and `key_pressed`.

Conditions name their kind directly. Prefix that key with `not_` to negate it.

- `{ perk = handle, player = "Me" }` tests a perk. Core handles resolve to the native perk name.
- `{ all = { ... } }` / `{ any = { ... } }` group a nonempty list of conditions.
- `{ animation = "Stance" }` and `{ interval = "Uninterrupt" }` test native state; optional `player` selects the model.
- `{ item = "Weapon", subtype = "Katana" }` tests equipped items. Optional `name` selects an exact item; `player` selects the model.
- `{ character = warrior }` matches a registered character, including copies of its model parameters.
- `{ key = "Kick" }` tests one input. `{ keys = { "Punch", "Punch", { "Forward", press = "Hold" } } }` tests an ordered sequence of 1–14 inputs. `press` defaults to `Tap`; alternatives are `Hold` and `Release`. Keys are `Up`, `Up-Forward`, `Forward`, `Down-Forward`, `Down`, `Down-Back`, `Back`, `Up-Back`, `Punch`, `Kick`, `Ranged`, `Magic`, `RaidCharge`, and `Super`.
- `{ direction = "Enemy", from = ..., to = ... }` tests the facing of `Me` or `Enemy` relative to two points with explicit players. It is separate from the move's top-level `direction` setting.

Character and key conditions use string literals, without constant aliases. Combine them with `key_pressed` to bind an authored move to a fighter's controls.

Spell and projectile selection also supports:

- `{ actor = "Sphere1" }` tests the native model's exact,
  case-sensitive actor name (for example, the name supplied by `create_projectile`).
  It does not test a warrior content ID; use `character` for that.
- `{ bullets = "MagicBullet", min = 1 }` tests the
  selected model's native charge count. `bullets` is required and accepts
  `MagicBullet` or `RaidChargeBullet`. Bounds are inclusive integers in
  0–2,147,483,647; minimum defaults to 0 and maximum to 2,147,483,647. Minimum must
  not exceed maximum. This is a condition only; use the scheduled `add_bullets`
  action to consume charge.

Both conditions accept a `not_` prefix and `player` (`Me`, `Enemy`, `Parent`, `Child`,
`EnemyChild`, or `Both`, default `Me`) through native condition targeting. The actor
name follows the scheduled-action symbol rules. Conditions can be nested in
`all`/`any` groups and used wherever typed move conditions are accepted. Native
charge overrides still apply; declaring a condition does not change recharge rules.

A `distance` condition uses `distance` for its axis (`X`, `Y`, or `Full`), `from` and `to`
move points, plus optional inclusive `min`/`max` bounds (defaults −1,000,000
and +1,000,000). Bounds must be finite within that range and ordered. Optional
`not_distance` negates the comparison. Point players are selected independently; there
is no outer `player` field. Native `X` distance is signed and facing-relative;
`Y` is signed vertical distance, and `Full` is planar Euclidean distance. `Animation`
is not a distance-point object. Omitted point players retain native current-model
selection. For example, a projectile can test when it has passed the front wall:

```lua
{ distance = "X", max = -250, from = { node = "Magic-Node2_1" }, to = { wall = "Front" } },
```

This condition does not delete an actor; attach a separate cleanup action to the
move selected by it. Named rig points must exist on that projectile's skeleton.

Arrays must have consecutive integer indices starting at 1. Building an array with `table.remove` or deleting unused fields with `nil` is supported; live holes, fractional/zero/negative indices, and extra named entries are rejected.

Repeated keys are preserved: two `"Punch"` entries inside `keys` require the native double-tap sequence. Entries are passed to the native Tap/Hold/Release groups in authored order; this is not a general timing or input-history scripting language.

Four additional named condition types are available in moves, templates and triggers:

| Kind key | Required value | Meaning |
| --- | --- | --- |
| `stage` | `StartStance`, `Fight`, `EndStance`, or `TryOn` | Matches the native round stage. |
| `round_result` | `Victory` or `Defeat` | Matches the selected fighter's completed-round result; useful with `stage = "EndStance"` for victory or loss moves. |
| `screen` | `ShopArmor`, `ShopWeapon`, `ShopHelm`, `ShopMissile`, `ShopMagic`, `ShopRuby`, `ShopFree`, `ShopRaidItemPack`, `Profile`, or `Fight` | Matches the native combat/preview scene. |
| `mod` | Native effect name, e.g. `MOD_TITAN` | Tests an active combat modification, **not** an installed Lua mod. |

Negate these conditions with `not_stage`, `not_round_result`, `not_screen`, or `not_mod`. They accept optional `player = "Me"`, `"Enemy"`, or `"Both"` (default `Me`). Player selection matters for `round_result` and `mod_exists`; round stage and screen are shared native state. Names must contain 1–128 characters without surrounding whitespace. Unknown stage/result/screen names and item-specific fields are rejected. Use the native name of an effect that actually exists; registration does not resolve effect names.

```lua
conditions = {
    { round_result = "Victory" },
    { stage = "EndStance" },
}
```

An interval accepts `type`, `name`, optional `from` and `to` frame indices, and optional `attack`. At least one of `type` or `name` must be nonempty. Frame indices are integers from 0 to 100,000; when both are supplied, end must not precede start. Omitted bounds retain the native interval behavior. Use stored animation sample indices within the move's frame range. `mid_frames` changes interpolation time between samples, not the indices used for these bounds.

`attack` requires `type = "Attack"` and the following fields:

| Field | Contract/default |
| --- | --- |
| `edges` | Array of 1–64 native rig edge names, each 1–128 characters; required unless `direct = true`. These are attacking body parts, not arbitrary mesh vertices. |
| `direct` | Boolean, default false. With true, omit `edges` or supply an empty array. Uses the native attack path without collision edges, once per active interval against the current opponent; native invulnerability/damage rules still apply. |
| `damage` | Finite multiplier 0–16, default 0. Applied through the selected native damage attribute. |
| `damage_type` | Single unshifted attribute: `UnarmedDamage` (default), `WeaponDamage`, `RangedDamage`, or `MagicDamage`. Mutually exclusive with `damage_terms`. |
| `damage_terms` | Optional map of 1–4 attributes to shifts, such as `{ WeaponDamage = 0, UnarmedDamage = -10 }`. Keys use the same four attribute names; shifts must be finite in −1,000…1,000. |
| `hit` | `High` (default), `Middle`, `Low`, `Spinning`, `HighHeavy`, `MiddleShortPlus`, `Physycal` (the native spelling for physical fall), `HighLong`, `NoReaction`, `WaspFly`, `Earthquake`, or `ElectrocutionPowerfield`. |
| `hit_move` | Optional move handle selecting an authored hit reaction. Mutually exclusive with `hit`. The referenced move must exist and be accessible when registration commits. |
| `id` | Integer 0–999, default 0; native attack identity. |
| `impulse` | Optional `{x=0,y=0,z=0}` in native physics axes; each component finite and within ±100,000. |

The optional `attack.options` table exposes native spell attack rules:

| Field | Contract/default |
| --- | --- |
| `no_effect` | Boolean, default false; suppresses the attack's normal hit effect. Separately scheduled effects remain independent. |
| `no_critical` | Boolean, default false; suppresses critical hits for this attack. |
| `ignores_block` | Boolean, default false; explicitly bypasses all block intervals. Native ranged/magic damage handling may also bypass block. |
| `ignores_all_invulnerable` | Boolean, default false; bypasses every invulnerability interval. Cannot be combined with `ignores_invulnerable`. Use only for attacks whose source move explicitly calls for this behavior. |
| `body_part` | Optional `Body` or `Head`; omitting it retains the native parser's default selection. |
| `defense_types` | Optional array of up to two unique `BodyDefense`/`HeadDefense` attribute names, default empty. Uses the native defense calculation; these are not literal damage reductions. |
| `ignores_invulnerable` | Optional array of up to 32 unique native invulnerability interval names, default empty. Names follow scheduled-action symbol rules. Only listed intervals are bypassed; names are not verified against loaded graphs at registration. |

For example, inside an attack table:

```lua
options = {
    no_effect = true, no_critical = true, ignores_block = true,
    body_part = "Body", defense_types = { "BodyDefense" },
    ignores_invulnerable = { "Evade", "Recovery", "Dash", "ShroudInterval" },
},
```

Options are copied and fingerprinted. Omitted/default/empty options preserve prior
attack declarations. Native parser comparisons verify these fields against sphere
attacks; actual damage/contact and visible effects require a fight test.

Damage terms use the existing native attribute comparison and alignment formula. `shift` offsets an attribute before that calculation; it is not an extra hit, damage percentage, or a sum of independent damage amounts. The native formula chooses the strongest adjusted attribute contribution. `damage` remains the attack's common multiplier. Ranged/magic terms retain their native block-bypass behavior even when mixed with another term.

```lua
local sf2 = require("sf2")
sf2.moves.register_template {
    id = "double-tap-attack",
    conditions = {
        { stage = "Fight" },
        { keys = { "Punch", "Punch", { "Forward", press = "Hold" } } },
    },
    intervals = {{ type = "Attack", from = 11, to = 14, attack = {
            edges = { "WEAPON_SAI-Edge30_2", "WEAPON_SAI-Edge31_2" },
            damage = 0.06,
            damage_terms = { WeaponDamage = 0, UnarmedDamage = -10 },
            hit = "Spinning", impulse = { x = 25, y = -100 },
        } }},
}
```

This registers a reusable template, not a complete selectable move. Attach it to a compatible animation and rig through `sf2.moves.register`. Extended reaction names select native hit reactions; they do not supply new reaction animations. All terms, shifts and repeated key entries participate in content fingerprints. Existing single-attribute definitions retain their previous fingerprint and native projection; an explicit one-term zero-shift map is equivalent to `damage_type`.

See [Character authoring](../../guides/character-authoring/) for a complete exported character module and input/attack example. The registration API validates structure and bounds; verify edge names, contact timing, mirroring and damage in a fight.

### Authored hit reactions

Use a move handle in `attack.hit_move` to select a reaction you registered, rather
than adding a mod-specific name to the engine's `hit` list. Define the victim's
move before the attack. Give that move a `hit` event whose `name` is its complete
namespaced ID. Normal victim locks, conditions and priorities still apply: the
reference does not force an ineligible animation. References to missing moves or
inaccessible dependencies reject the entire registration transaction.

```lua
local reaction = sf2.moves.register {
    id = "recoil", animation = sf2.assets.binary("animations/recoil"),
    core_templates = { "Recoil", "Hit" },
    events = {{ hit = sf2.mod.id .. ":moves/recoil" }},
    direction = { impulse = { reverse = true } },
}
local strike = sf2.moves.register {
    id = "strike", animation = sf2.assets.binary("animations/strike"),
    intervals = {{ type = "Attack", from = 10, to = 12, attack = { edges = { "Weapon-Edge1" }, damage = 0.3, hit_move = reaction } }},
}
```

Supply compatible animation binaries and rig edges, plus the input conditions,
locks and other move fields your combat design requires. This fragment only
shows the reaction link; it is not a complete playable weapon.

### Scheduled actions and move presentation

The following fields belong directly to `sf2.moves.register`; `register_template`
rejects them. They require the same `content.register` capability as the move.

| Field | Meaning/default |
| --- | --- |
| `timeline` | Frame/event table containing up to 64 scheduled actions in total, default empty. |
| `profile` | Optional `{ rank, core_icon }` entry shown in the native moves list. `rank` is a required integer in 0–100,000; `core_icon` is an existing native icon name such as `Trick7.super_slash`. Omit the table for no entry. |
| `tactic_distance` | Optional native AI distance requirement with required `distance`, `from`, and `to`. `distance` is `X`, `Y`, or `Full` (planar distance). `min`/`max` default to −1,000,000/+1,000,000, must be finite within those bounds, and minimum must not exceed maximum. Points use the table format below, require explicit players and cannot use `Animation`. This restricts native tactic eligibility; it does not itself configure an AI tactic table. |
| `tactic_conditions` | Optional dense array of 1–32 typed move conditions evaluated by native AI when considering this move. It supports the same condition shapes and `all`/`any` groups as `conditions`; it does not change player input eligibility. Use this for AI rules that combine distance with another state, such as allowing a cast when the opponent is falling. Mutually exclusive with `tactic_distance`; omit both for no authored AI gate. |
| `no_wall_repulsion` | Boolean, default false. Uses the native move flag to suppress wall repulsion. |
| `no_interpolation_frames` | Boolean, default false. Uses the native move flag to suppress interpolation frames. |
| `no_magic_recharge` | Boolean, default false. Sets the native per-move flag preventing magic recharge during that move; useful for projectile actors. |
| `velocity` | Optional native motion table: `x`, `y`, `z`, `ax`, `ay`, `az` default to 0; `save_velocity` defaults to false. Components are finite numbers in −100,000..100,000. The first three are velocity, the last three acceleration, in native simulation units. `save_velocity` preserves native existing velocity on move entry. Omit the table to retain existing motion parsing behavior. These fields belong on moves, not templates. |

For example, a projectile flight move can declare:

```lua
-- Fields inside sf2.moves.register:
conditions = { { actor = "Sphere1" } },
velocity = { x = 30 },
no_magic_recharge = true,
```

Motion does not supply a rig, collider, attack interval or projectile lifecycle.
Its fields and the recharge flag participate in content fingerprints. Native
parser and predicate tests cover these declarations; live trajectory/contact
acceptance remains separate.

Place each action under a frame number (0–100,000) or an event key in `timeline`.
Frames are native animation sample indices, not elapsed milliseconds. Event keys
are `round_stage`, `key_pressed`, `key_released`, `round_start`, `round_end`,
`hit`, `strike`, `wall_hit`, `animation_start`, `animation_end`, `interval_start`,
`interval_end`, `every_frame`, `birth`, and `mod_expires`. Multiple actions under
one key use an array; their order is preserved.

- `sound` accepts one native sound name or a dense array of 1–32 existing
  native sound names. The native action chooses one entry when it runs. One entry
  gives a fixed sound. Entries retain order and repetition, allowing the native
  selection to weight repeated names. Voice filtering is not added.
- `{ play_sound = "name", voice = "Male" }` plays a native sound.
  `play_sound` is a required native sound symbol. Optional `voice` is exactly
  `Male`, `MaleLow`, or `Female`; omission allows any voice. The native action
  plays only when the actor's voice matches. Use separate actions for separate
  voice clips; `sound` remains unfiltered.
- `stop_sound` takes the exact name of a native sound
  already playing. Schedule it on `Hit` or `AnimationEnd` to end a long clip when
  a move is interrupted or finishes. This calls the native sound stop action;
  the name is validated at registration, while the sound's existence is resolved
  by the game.
- `play_animation` starts a move on `player` (`Me`, `Enemy`, `Parent`,
  `Child`, or `EnemyChild`) at the chosen frame or event. Its value is
  a handle returned by `sf2.moves.register` or a string naming an
  existing native move. Optional `child_name` picks a spawned actor by
  its exact name, useful when a caster has several children. The named child is
  checked first; if none matches, the native player target is used. The native
  action can fail when its target is absent or cannot select that animation;
  it does not force a transition. Register the target move before the caster.
- `shake` takes a table. All fields default to zero:
  `pause_time` and `effect_time` are integer native frame counts in 0–10,000;
  `amplitude_x`, `amplitude_y`, `frequency_x`, and `frequency_y` are finite native
  camera values in 0–1,000. Timing comes from the containing timeline key.
  This schedules the existing native camera action; it does not apply damage.
  Unknown fields and payloads on unrelated action types are rejected.

```lua
timeline = {
        [3] = { sound = "snd_blade_fury" },
        hit = { stop_sound = "snd_blade_fury" },
        animation_end = { stop_sound = "snd_blade_fury" },
}
```

For a spawned actor that changes phase after its caster has advanced eight
frames, retain the handle from its attacking move:

```lua
local hand_attack = sf2.moves.register {
    id = "hand_attack", animation = sf2.assets.binary("animations/hand_attack"),
    -- Add the child's locks, conditions, attack interval, and cleanup here.
}
local cast_timeline = {
        [17] = { play_animation = hand_attack, player = "Child", child_name = "BlackHand" },
    }
```

- `{ try_on_end = true }` signals native shop preview completion. Put it under
  `animation_end` on a shop-only move.

- `effect` takes a table. Required `name` identifies the
  active effect on the native model; required `core_sequence` selects an existing
  native effect sequence. Optional `scale` and `time_scale` default to 1 and must
  be finite numbers greater than 0 and at most 100. `looped` defaults to false.
  `on_background` defaults to false; true selects the native background effect
  layer for effects such as a boss's levitation aura.
  Optional `position` uses the move point format below, except `Animation` is not
  supported by native effect positions. `follow` defaults to false and requires
  `position` when true. Omit `position` for the native model-owned/default placement.
  Optional `attach` anchors and rotates the effect using two live model nodes. It
  requires `player` (`Me`, `Enemy`, `Parent`, `Child`, or `EnemyChild`),
  `root_point`, and `attach_point` (existing node names on that actor). Optional
  `offset_x`, `offset_y`, and `start_rotation` default to 0; each must be finite
  and within −10,000…10,000. The offset is in the two-node local frame, with
  positive Y down. `start_rotation` adds degrees to the node direction. The
  effect follows the nodes for its lifetime. Use either `attach` or `position`.
  Leave `follow` unset with `attach`; following is automatic.
  Missing live nodes cause the effect to be skipped and logged; the action does
  not alter combat damage or move selection.
- `stop_effect` takes the effect name and stops that effect on the
  model. `stop_follow_effect` takes the same name and detaches its
  following behavior while leaving the effect active. Names must match the
  corresponding effect's `name`; these actions do not select sequences globally.

For example, these actions start a following sphere effect and detach it at the
end of the move. Place them in a compatible move's `timeline` table:

```lua
timeline = {
        [2] = { effect = {
    name = "SmallSphereStart", core_sequence = "mgc_magic_small_sphere_start",
    scale = 0.75, time_scale = 1.45,
    position = { node = "Magic-Node2_1", player = "Me", y = 80 }, follow = true,
} },
        animation_end = { stop_follow_effect = "SmallSphereStart" },
        round_end = { stop_effect = "SmallSphereStart" },
}
```

For a moving boss aura, attach to two nodes instead of supplying a fixed point:

```lua
timeline = {
        [13] = { effect = {
    name = "ElectroEffect", core_sequence = "mgc_effect_shocker",
    scale = 0.75, time_scale = 2, on_background = true,
    attach = { player = "Me", root_point = "MacroBodyGatekeeper-Node975",
        attach_point = "MacroBodyGatekeeper-Node445",
        offset_x = -54, offset_y = -12, start_rotation = -78 },
} },
}
```

Effect names and sequences follow the same symbol rules as sounds. Registration
checks their syntax, not whether sequences or rig nodes exist. Looping effects
need appropriate stop/cleanup actions for the move's lifecycle. These operations
use native effect rendering; they do not load XML or define new effect resources.
Native parser/scheduling tests do not establish visible rendering acceptance.

For example, these are fields inside a move definition with a valid animation:

```lua
intervals = {
  { type = "Attack", from = 48, to = 49, attack = { direct = true, damage = 0.15, damage_type = "MagicDamage",
               hit = "NoReaction" } },
},
timeline = {
        [12] = { play_sound = "snd_m_pl_attack6", voice = "Male" },
        [48] = { shake = { effect_time = 30, amplitude_x = 7, frequency_x = 1,
              amplitude_y = 9, frequency_y = 0.3 } },
    },
```

`NoReaction` keeps the native no-reaction name; it does not suppress damage.
An edge-free attack is explicit so accidentally omitting `edges` cannot create
an unavoidable attack. Existing edge-based definitions and fingerprints are unchanged.

Scheduled projectile lifecycle actions use the same timeline keys:

- `projectile` takes `{ name, core_skeleton,
  copy_parent_type }` or `projectile = { name, core_skeleton, item }`. The native
  runtime creates a child weapon actor with this exact model name and an existing
  skeleton such as `SkeletonMagic`. Supply exactly one equipment source:
  `copy_parent_type` copies the caster's `Weapon`, `Ranged`, or `Magic` equipment
  into the child's Weapon slot; `item` equips a resolved equipment handle there,
  including a hidden core projectile item. `item` accepts weapon, ranged, or
  magic equipment; missing and incompatible items fail registration.
  Names are symbolic, not paths. This API describes this native child-actor path;
  it does not register a warrior or load a new skeleton. Copying equipment retains
  native item properties instead of supplying a replacement damage value.
- Optional `projectile.start_move` is a registered move handle in this mod or an
  accessible dependency; register the child move first. Alternatively use
  `projectile.core_start_animation` for an exact existing native animation name.
  They are mutually exclusive. Omit both to let the native Birth event select a
  move. A start override does not prove the animation is compatible with the rig.
- `{ add_bullets = "MagicBullet", amount = -1 }` changes charge. The bullet name is
  `MagicBullet` or `RaidChargeBullet`; `amount` is a nonzero integer from −100,000 to
  100,000. Negative consumes charge, positive adds charge through native handling.
  It acts on the model running the move; no player override is accepted. Native
  charge rules still apply. It neither creates a projectile nor guards selection;
  author the cast's eligibility and spawn action separately.
- `delete_actor` takes a player name, one of `Me`, `Enemy`, `Parent`,
  `Child`, or `EnemyChild`. It invokes native deletion for that selected actor.
  Usually use `Me` in the projectile's own strike/expiry move. Do not place that
  action on the caster unless deleting the caster is intended.

For example, these entries in a caster's `timeline` reproduce native magic
spawn and charge timing. The projectile still needs its own complete move graph:

```lua
timeline = {
        [2] = { projectile = {
    name = "Sphere1", core_skeleton = "SkeletonMagic", copy_parent_type = "Magic",
} },
        [7] = { add_bullets = "MagicBullet", amount = -1 },
}
```

A boss ability can equip its own hidden core item instead of borrowing the
caster's equipment. Register a compatible child move before referencing it:

```lua
local quake_item = sf2.items.get("core:items/magic/MAGIC_BUTCHER_EARTHQUAKE")
local timeline = {
    [22] = { projectile = {
        name = "Earthquake", core_skeleton = "SkeletonMagic",
        item = quake_item, start_move = quake_child,
    } },
}
```

Registration validates payloads and owned start-move references. Core skeletons
and native starting animations resolve at runtime. Tests compare native parsing
and scheduling with archived actions; this alone does not prove live projectile
contact, cleanup or rendering. These optional action payloads participate in
fingerprints; older action declarations retain their representation.

Core sound/icon names are exact symbolic names of 1–128 letters, digits, `_`,
`-`, or `.`; they are not file paths or owned asset handles. Registration checks
syntax, not resource availability. Missing core assets still follow the native
resolver's behavior. Use the owned-audio trigger API for mod audio assets.
Actions are additional to inherited template actions, so check templates to avoid
duplicate effects. All these fields participate in content fingerprints; omitted
and empty/default presentation fields preserve previous fingerprints.

`profile.display_name` optionally accepts a localization handle from
`sf2.localization.register` or `sf2.localization.get`. It changes the profile
heading without changing the move's identity or animation lookups. The handle
must reference an available localization in your mod or a declared dependency.
If omitted, native behavior uses the move's runtime name as the translation key;
for namespaced mod moves, provide a title explicitly:

```lua
local title = sf2.localization.register {
    id = "move.super_slash", language = "eng", value = "Super Slash",
}
-- Inside sf2.moves.register:
-- profile = { rank = 4, core_icon = "Trick7.super_slash", display_name = title }
```

For example, these fields can be added to a move registration table with an
existing binary animation handle:

```lua
profile = { rank = 4, core_icon = "Trick7.super_slash" },
tactic_distance = { distance = "X", min = 200, max = 800, from = { pivot = "Me" }, to = { node = "NPivot", player = "Enemy" } },
timeline = {
        [8] = { sound = "snd_swish_sword1" },
        strike = { sound = { "snd_hit1", "snd_hit2" } },
    },
```

Managed parser/scheduling checks do not verify audible playback, rig compatibility,
rendered icons or shop completion in a running game. Playtest those behaviors.

### Availability, transitions and positioning

`locks` are checked when the native system builds a fighter or preview's eligible move list. They are separate from the conditions evaluated to start a move. Use locks for equipment/skeleton/screen requirements; put input sequences and current-animation tests in `conditions`. Sibling locks are ANDed; use `{ any = { ... } }` for alternatives. Existing template locks remain additional requirements.

`sf2.moves.register` also accepts up to 32 ordered `transitions`. Each row requires 1–64 `conditions` and **exactly one** of `frame_shift` or `first_frame`. `frame_shift` is an integer from −100,000 to 100,000 using the native relative-frame transition behavior. `first_frame` is an absolute starting sample in 0–100,000. The first matching transition wins. Check values against the actual animation. The recovered parser does not inherit template transitions, so **`register_template` rejects `transitions`**; declare them directly on the move.

```lua
-- Fields inside sf2.moves.register { ... }:
transitions = {{ frame_shift = 2, conditions = {
    { animation = "SaiHeavySpit" },
    { interval = "SemiUninterrupt" },
} }},
locks = {
    { item = "Weapon", subtype = "ChineseSwords" },
    { item = "Skeleton", subtype = "Skeleton" },
},
align = {
    axes = { "X", "Z" },
    pivot = { node = "NHeel_2" },
    position = { pivot = "Me" },
},
direction = {
    from = { node = "NPivot", player = "Me" },
    to = { node = "NPivot", player = "Enemy" },
},
```

Point tables use `{ node = "NPivot", player = "Enemy" }`, `{ wall = "Front" }`,
or partless points such as `{ pivot = "Me" }`, `{ animation = true }`,
`{ floor = "Me" }`, and `{ com = "Me" }`. Optional finite `x`/`y` offsets are
within −100,000…100,000 and default to 0. `node` requires an exact nonempty name
of at most 128 characters. Supported players are `Me`, `Enemy`, `Parent`, `Child`,
and `EnemyChild`; model availability is a separate runtime requirement.

- `align` requires 1–3 unique `axes` (`X`, `Y`, `Z`) plus `pivot` and `position` points. Alignment supports `Nodes`, `Pivot`, `Animation`, and `Wall` objects; omitted players default to `Me` in the native parser. Optional `shift_model_node` selects the exact native rig node shifted during alignment, such as `NPivot`. Only the **position** may have nonzero offsets; pivot offsets and `z` are rejected because the recovered parser does not apply them.
- `direction` accepts either `from` and `to` points with explicit players, or `impulse = { reverse = false }`. The two forms are mutually exclusive. Impulse mode uses the native incoming-impulse facing calculation; `reverse` is a boolean and defaults to false. With true it faces against the incoming impulse. No impulse is applied. Point-based directions require both points. These support `Nodes`, `Pivot`, `Wall`, `Floor`, and `COM` objects. `Animation` is not a native direction point and is rejected. Direction uses the existing native facing calculation; it does not move the character.

Names and points are not asset lookups at registration. A valid declaration can still reference an absent rig node. Test contact, mirroring, position and transitions in a fight. Graph fields are fingerprinted; omitted/empty graph fields retain existing fingerprints. Lists are copied at registration, so later Lua table edits do not mutate registered content.

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

## sf2.moves.replace

**Signature:** `sf2.moves.replace(definition)`

**Returns:** A move handle. Its runtime name is the existing native `target`; its mod ID is the supplied local `id`.

**When:** During mod loading. The typed definition is applied after native animations load and before fights use them. Removing the mod restores the original move.

**Requires:** `content.patch`, a `core` dependency for core assets, and a binary animation shipped by the mod.

Replace one existing native move with a complete typed definition while keeping its name. Supply the fields of [`sf2.moves.register`](#sf2movesregister), plus required `target` (exact native move name) and `expected_file` (the original native `.bytes` filename). `templates` is unavailable; use existing `core_templates` and define all other behavior in the replacement. The target must exist exactly once and its loaded filename must match the guard. Duplicate targets and mismatched guards fail during native application. Its normal registration fields, conditions, locks, intervals, actions and positioning all come from the new definition; omitted fields are not inherited from the original move. Refer to the [shared field tables](#shared-move-fields) and validate rig nodes and attack contact in an actual fight.

```lua
local step = sf2.moves.replace {
    id = "guarded_step", target = "ExistingStep",
    expected_file = "existing_step.bytes",
    animation = sf2.assets.binary("animations/guarded_step"),
    core_templates = { "Step", "Controlled" },
    priority = 120,
    conditions = { { keys = { "RaidCharge" } } },
    intervals = { { name = "Uninterrupt", to = 12 } },
}
```

The original name stays available to native perk triggers and child transitions. A handle can also be used in a registered tactic or another mod move. Existing fighter availability still depends on the new locks and animation rig. The guard checks the loaded original filename, not the content hash of the replacement binary. Two mods cannot own the same native target. Native parsing and teardown are atomic; a failed replacement leaves the original move loaded. Save compatibility fingerprints include the target, guard and full new definition. The game receives typed Lua content; mods do not load XML at runtime.

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
local sound_trigger = sf2.moves.register_trigger {
    id = "opening_step_sound",
    events = { { animation_start = sf2.mod.id .. ":moves/opening_step" } },
    actions = { {
        type = "sound",
        audio = sf2.assets.audio("audio/step"),
        volume = 0.7,
    } },
}
```

## sf2.moves.extend_item_lock

**Signature:** `sf2.moves.extend_item_lock { move, item_type, source_subtype, subtype }`

**Returns:** Nothing (`nil`).

**When:** During registration. Applied to already-loaded native moves before external moves and fighters are built. Requires Apply & Restart when changing enabled content; it does not rebuild already-created fighter snapshots in place.

**Requires:** `content.patch`.

Add a subtype alternative to one existing equipment availability clause while preserving all other locks.

| Field | Required meaning |
| --- | --- |
| `move` | Exact, case-sensitive native move name, e.g. `SaiSpit`. |
| `item_type` | `Weapon`, `Ranged`, `Magic`, `Armor`, `Helm`, or `Skeleton`. |
| `source_subtype` | Existing subtype identifying the clause to extend. |
| `subtype` | Additional subtype; must differ from `source_subtype`. |

Each string must contain 1–128 characters without surrounding whitespace or line breaks. This does not alter an item's subtype; use [sf2.items.set_subtype](../items-progression-forge/#sf2itemsset_subtype) separately once its complete move graph exists.

```lua
local sf2 = require("sf2")
sf2.moves.extend_item_lock {
    move = "SaiSpit",
    item_type = "Weapon",
    source_subtype = "Sai",
    subtype = "ChineseSwords",
}
```

The runtime must find exactly one positive direct item lock, or one positive top-level OR group containing a positive direct item lock with the requested type/subtype and no item-name restriction. A direct item lock becomes an OR group; an existing OR group retains its children and gains the alternative. Nested/negated/AND-only/named-item matches are not widened. Missing or ambiguous matches and an already-present alternative fail the whole native batch before any lock changes. Other equipment, screen, perk and skeleton requirements remain intact.

Different additions to the same group compose. The source selector must exist before the batch; it cannot depend on a subtype another pending extension adds. The same move/type/additional-subtype combination is a registration conflict, including across mods, even if the source selector differs. Subtype matching is exact and case-sensitive. The selector and addition are fingerprinted. Native teardown restores the original condition objects; unrelated sibling edits are preserved. This endpoint supplies no missing animations, attacks or preview behavior by itself.

## sf2.moves.patch

**Signature:** `sf2.moves.patch { move, disable?, conditions?, interval_start?, interval_end?, hit?, sound_frame?, input?, priority?, animation?, remove_interval? }`

**Returns:** Nothing.

**When:** Entrypoint. Registration records the patch; native application validates
its target after base animations are available, before a fight starts.

**Requires:** `content.patch` and any dependencies required by referenced conditions.

Patch selected fields of an existing native move. `move` is an exact, case-sensitive native
name, not a move handle. Names contain 1-128 ASCII letters, digits, underscores,
dots or hyphens. At least one nonempty operation is required.

| Field | Meaning |
| --- | --- |
| `disable` | Boolean, default false. `true` adds a selection condition that always fails, so the native move stays registered but fighters cannot select it. Use when a complete replacement move is registered separately. |
| `conditions` | Up to 32 additional typed move conditions, using the same records as `moves.register`. They are appended as extra requirements; existing conditions remain. |
| `interval_start` | `{ name, expected, value }`. `name` is `Uninterrupt`, `SelfUninterrupt` or `Unstable`. Exactly one matching named interval must exist. Its start must equal `expected`; `value` becomes the start and cannot exceed its end. A missing native `Start` means zero. |
| `interval_end` | `{ name, expected, value }`. `name` is `Uninterrupt`, `SelfUninterrupt` or `Unstable`. Exactly one matching named interval must exist. Its end must equal `expected`; `value` becomes the end and cannot precede its start. |
| `hit` | `{ expected, value }`. Requires exactly one attack interval with exactly one full-interval reaction matching `expected`. Replaces only its reaction name. Supported names: `High`, `Middle`, `Low`, `Spinning`, `HighHeavy`, `MiddleShortPlus`, `Physycal`, `HighLong`, `NoReaction`. |
| `sound_frame` | `{ name, expected, value }`. Requires exactly one native direct Sound action with this clip name, scheduled at `expected`. Moves it to `value`. Event-driven and RandomSound actions are not supported by this selector. |
| `input` | `{ expected, value }` replaces one direct native Keys condition. Both values are supported control names, such as `Super` and `RaidCharge`, with `Tap` timing. The move must have exactly one direct Keys condition, and its authored key requirement must match `expected`. Native AI may temporarily invert the parsed key state while dispatching a move; that transient state is ignored by this guard. Other conditions remain intact. |
| `priority` | `{ expected, value }` replaces a native selection priority. Both are distinct integers in 0–100,000. The current priority must match `expected`. |
| `animation` | `{ expected, value }` replaces a parsed native move's clip. `expected` is its exact existing `.bytes` filename; `value` is a binary handle from `sf2.assets.binary` for a file shipped by the mod. The runtime loads the new clip and updates its frame count while keeping the move's name, conditions, actions and linked children. The clip must have usable frames; check its node layout and action timing against the fighter in combat. |
| `remove_interval` | `{ name, type, start, ["end"] }` removes exactly one native interval. All four fields are required guards: the interval's name, type, and inclusive sample bounds must match. Supported types are `Attack`, `Block`, `Invulnerable`, `Invisible`, `Uninterrupt`, `SelfUninterrupt`, and `Unstable`; bounds are integers from 0 through 100000 with end at least start. A removed interval cannot also receive a bounds patch. Use this for a verified native combat difference, since removing an attack or protection window can substantially change a fight. |

Frame values must be distinct integers from 0 through 100000. Reaction names
must also differ. Hit records with explicit start/end bounds in a deferred move,
multiple reactions, multiple attacks, missing targets and ambiguous selectors
are rejected. A patch does not supply a missing hit animation or sound asset.
`disable = true` must be the only operation in its patch table.
The entire native patch batch is validated before any of its edits apply.
`interval_start` and `interval_end` may target the same interval in one patch;
their combined bounds must remain valid. Both parsed and deferred native
intervals restore their original bounds when the patch is removed.
Removed intervals return to their original list position when the patch is removed.

```lua
local sf2 = require("sf2")
sf2.moves.patch {
    move = "MassBombPlayer",
    conditions = { { not_mod = "Stun" } },
}
sf2.moves.patch {
    move = "RangedHeavyPlayer",
    interval_end = { name = "Uninterrupt", expected = 42, value = 40 },
}
sf2.moves.patch {
    move = "ChakramFly",
    hit = { expected = "High", value = "MiddleShortPlus" },
}
sf2.moves.patch {
    move = "ShopRangedTryOnHeavyPlayer",
    sound_frame = { name = "snd_disk", expected = 18, value = 16 },
}
sf2.moves.patch { move = "WaspFly_150", disable = true }
sf2.moves.patch {
    move = "SaturnBlasterAbilityPlayer",
    input = { expected = "Super", value = "RaidCharge" },
    priority = { expected = 1000, value = 200 },
}
sf2.moves.patch {
    move = "LightingChainPlayer",
    input = { expected = "Super", value = "RaidCharge" },
    priority = { expected = 9000, value = 200 },
    interval_start = { name = "Uninterrupt", expected = 9, value = 0 },
}
-- Requires assets/animations/wave_cast.bytes in this mod.
sf2.moves.patch {
    move = "RatWavePlayer",
    input = { expected = "Up", value = "RaidCharge" },
    animation = { expected = "rats_wave.bytes",
        value = sf2.assets.binary("animations/wave_cast") },
}
sf2.moves.patch {
    move = "PerkFearRayPlayer",
    remove_interval = { name = "Evade", type = "Invulnerable", start = 0, ["end"] = 47 },
}
```

Only one `moves.patch` declaration may own a given move, including across mods;
combine operations in one table. Conflicts reject the registration transaction.
Expected source values make incompatible base data fail explicitly instead of
silently applying a different edit. The input patch preserves the native move
identity and its linked child animations. A clip replacement is loaded after the
native parser, so a matching filename alone is insufficient: missing or malformed
replacement bytes fail the native batch. Existing item/perk-lock APIs remain separate.

The runtime supports deferred and already-parsed intervals. Removing the content
restores its edited fields and removes its added condition objects, preserving
unrelated conditions and later field values that no longer equal the patch's
values. This is content teardown during Apply & Restart, not an API for changing
moves mid-fight. Patch owners, selectors, expected values, replacements and
conditions and `disable` participate in compatibility fingerprints. Mods without patches retain
their previous fingerprint representation.

## sf2.moves.remove_perk_lock

Remove one exact direct perk lock from an existing native move.

**Signature:** `sf2.moves.remove_perk_lock { move, perk }`

**Returns:** Nothing.

**When:** During mod registration. The patch is applied when the active content
set is projected into the recovered move runtime and is restored when that overlay
is removed or rolled back.

**Requires:** `content.patch`, plus access to the referenced perk through core or
a declared dependency.

```lua
local old_unlock = sf2.perks.get("core:perks/PERK_DOUBLE_JUMP_KICK")

sf2.moves.remove_perk_lock {
    move = "DoubleJumpKick",
    perk = old_unlock,
}
```

`move` is the canonical native move `Name` and is case-sensitive. `perk` is a
resolved perk handle; raw perk-name strings are not accepted. Duplicate patches
for the same move/perk pair are rejected.

This is deliberately narrower than a general move or XML patch. The host verifies
that the recovered base move contains that exact **direct**
`<Locks><Perk Name="...">` condition and removes only that one parsed lock. Other
locks on the move, including item, screen/profile, operator and inherited template
locks, remain untouched. Missing moves, missing direct perk locks, case mismatches,
or a live parser layout that does not match the recovered XML fail explicitly.

Use this when archival content replaces an old learned-perk gate but keeps the move
itself available. It does not grant the obsolete perk, remove arbitrary conditions,
rename moves or expose raw XML to Lua.

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

Attach this function to `sf2.tactics.register` to
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
[fighter reference](../fighter/). `event.frame` and `event.seconds` use the
fight's advancing simulation clock, including while a fighter has not pressed
a key. In a live fight, `event.back_wall_distance` is the nonnegative horizontal
distance in arena units from this fighter's pivot to the wall behind its current
facing. It is recomputed for each decision; older host-only adapters may omit it.
Native move conditions can measure from a named body node instead, so keep
some room inside a move's wall-distance limits. Inside `on_decide`, an opponent
that needs room for a wall attack can steer toward a 120–300 unit band:

```lua
local step_forward, step_back
for _, candidate in ipairs(event.actions) do
    if candidate.name:find("StepForward", 1, true) then step_forward = candidate end
    if candidate.name:find("StepBack", 1, true) then step_back = candidate end
end
local distance = event.back_wall_distance
if distance and distance < 120 then return step_forward or "wait" end
if distance and distance > 300 then return step_back or "wait" end
```

`event.actions` is an array of currently legal input-driven actions. It may be
empty. Each candidate has these fields:

| Field | Meaning |
| --- | --- |
| `name` | Native runtime action name, including namespaced authored moves. |
| `type` | Native classification `"none"`, `"move"`, or `"attack"`. This is authored animation metadata, not a prediction of contact or damage. |
| `priority` | Native integer move priority. Higher numbers take precedence among competing moves when native conditions apply; this is not an AI utility score. |
| `timing` | Detached nominal clip timing, described below. Older name-only host adapters leave this `nil`. |
| `inputs` | Array of `{ control, press }` entries from the native key combination used to dispatch this action. Empty if key metadata is unavailable. |

The host filters move conditions, native tactic conditions, equipment availability and priority before
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
        local best
        for _, action in ipairs(event.actions) do
            if action.type == "attack" and
               (not best or action.priority > best.priority) then
                best = action
            end
        end
        if not best then return nil end
        memory.next_attack = event.seconds + 1
        return best
    end,
}
```

Returning a
candidate selects its original identity even if Lua edits its fields. Field edits
cannot change the move, its priority, or a later decision's snapshot. An action
classified as `"move"` or `"none"` can still contain authored combat behavior;
use your own move knowledge when classification alone is insufficient.

### Clip timing and controls

Each native candidate's `timing` contains:

| Field | Meaning |
| --- | --- |
| `first_sample`, `last_sample` | Inclusive, zero-based sample indices in the animation's native storage. These are not simulation clock values. |
| `mid_frames` | Number of interpolation frames between stored samples. Sample spacing is `mid_frames + 1`. |
| `nominal_frames` | Native nominal clip length: `(last_sample - first_sample + 1) * (mid_frames + 1)`. |
| `nominal_seconds` | `nominal_frames / 60`, using the nominal simulation rate. |
| `looped` | Whether native playback is configured to loop. Nominal length describes one cycle. |

This is clip metadata, **not a guaranteed completion time, recovery time, or
hit window**. Transitions, interruption, looping, triggered actions and slow
motion affect playback. Native eligibility continues to decide when the AI may
select another action. For example, ten samples at `mid_frames = 2` have a
nominal length of 30 frames (0.5 seconds).

Each input's `press` is `"tap"`, `"hold"`, or `"release"`. `control` uses the
same names as move authoring: `Up`, `Up-Forward`, `Forward`, `Down-Forward`,
`Down`, `Down-Back`, `Back`, `Up-Back`, `Punch`, `Kick`, `Ranged`, `Magic`,
`RaidCharge`, or `Super`. Unexpected native control IDs become `Unknown`.
Forward and Back describe authored facing-relative directions, not screen-left
and screen-right. Inputs are grouped by tap, hold, then release, preserving
the native order within each group. They describe a combination, not a timed
sequence of commands. At most 64 entries are supported per candidate.

For example, this callback chooses the shortest non-looping clip dispatched by
a kick tap, without depending on any native or custom move name:

```lua
-- Place this function in a tactic definition's on_decide field.
local function choose_quick_kick(memory, event)
    local best
    for _, action in ipairs(event.actions) do
        local timing = action.timing
        if timing and not timing.looped then
            for _, input in ipairs(action.inputs) do
                if input.control == "Kick" and input.press == "tap" then
                    if not best or timing.nominal_frames < best.timing.nominal_frames then
                        best = action
                    end
                    break
                end
            end
        end
    end
    return best -- nil uses native tactics when no candidate matches
end
```

Editing `timing` or `inputs` only edits this Lua snapshot. It cannot change
playback, issue a different control, or affect the next decision. Return the
original candidate table to select it.

### Reacting to the current animation

`event.self.animation` and `event.opponent.animation` expose
the same detached [active animation observations](../fighter/#fightersnapshot)
as combat callbacks. Check for a missing opponent or `nil` animation. This lets
your AI react to live native intervals, rather than infer an attack from the
opponent's animation name or its nominal duration:

```lua
local function evade_active_attack(memory, event)
    local animation = event.opponent and event.opponent.animation
    if not animation then return nil end
    local attacking = false
    for _, interval in ipairs(animation.intervals) do
        if interval.type == "attack" then attacking = true; break end
    end
    if not attacking then return nil end
    for _, action in ipairs(event.actions) do
        if action.type == "move" then
            for _, input in ipairs(action.inputs) do
                if input.control == "Back" then return action end
            end
        end
    end
    return nil
end
```

Native eligibility and the six-frame decision throttle still apply. This is a
reaction policy, not a guarantee of evading a hit. The observation describes the
current animation state; it is not a candidate you may return from `on_decide`.

`memory` is a plain Lua table private to this native fighter controller and this
tactic. It survives decisions on that controller, not save/reload or controller
replacement. Persist deliberate profile data through the owned state API.
Closures at script scope are shared across fighters; use `memory` for isolated
AI state. Callback errors, invalid/stale actions and instruction-budget overruns
(200,000 instructions) disable this callback for that controller/tactic and use
native fallback; the first failure is diagnosed. Other fighters keep their own
AI. This callback is not a coroutine.

The [programmable AI example](https://github.com/dawc17/ProjectEclipse/tree/main/Mods/example.programmable-ai)
includes four opponents. The fourth, **Reactive Guardian**, uses active attack
intervals to choose backward movement and nominal clip timing to choose a quick
kick, without matching move names. Isolated Lua tests cover
those reactions, fallback, memory isolation, stale actions and failures. Native combat and physical input
acceptance require a game playtest.

The AI test suite also parses shipped retreat/kick definitions and their 67-node
clip bytes through the compiled native parser, reader and snapshot adapter. It
feeds those real control/timing snapshots to the Reactive Guardian callback.
The test bypasses Unity resource I/O by prewarming the native animation cache;
candidate eligibility and opponent observations remain controlled in that test.
It verifies the content-to-Lua connection, not in-fight reactions or rendering.

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
