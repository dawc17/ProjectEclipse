---
title: Events and fight sequences
description: Build repeatable modes, schedule events, require entry tickets, and track sequence progress.
---

A mode starts with an ordered roster of already registered fights. It controls which
step is available, what happens after a win/loss, and whether entry costs a
mod-owned item. Events and raids share this format. Since API 0.19, an optional
Lua result callback can choose branches or finish a run.

## Sequence fields

The three registration functions below accept the same table:

| Field | Type | Default | Meaning |
| --- | --- | --- | --- |
| `id` | String | Required | Local mode/event/raid ID. |
| `fights` | Fight-handle array | Required | 1–100 distinct fights owned by this mod. |
| `repeatable` | Boolean | `false` | Restart at the first step after completion. |
| `reset_on_loss` | Boolean | `false` | Restart at the first step after a loss. |
| `minimum_level` | Integer, 1–1000 | `1` | Player entry requirement. |
| `starts_at` | UTC Unix timestamp | `0` | Inclusive start time. |
| `ends_at` | UTC Unix timestamp | `0` | Exclusive end time; zero means no end. |
| `entry_item` | Consumable item handle | Omitted | Entry ticket owned by this mod. |
| `entry_count` | Integer, 1–100000 | Required with entry item | Number consumed per fight attempt. |
| `hard_mode` | Boolean | `false` | Raid-only Power Mode filter setting. |
| `on_result` | Lua function | Omitted | Since API 0.19: choose the next fight after native result settlement. See below. |
| `on_prepare` | Lua function | Omitted | Since API 0.22: generate an encounter or wait for a player choice before entry. |

Timestamps are integer seconds since 1970-01-01 UTC, from 0 through 253402300799.
A nonzero end must be after the start. Omit both for permanent availability.
Supplying `entry_count` without `entry_item` is an error. Shared currencies cannot
be entry tickets. Non-raid modes cannot set `hard_mode=true`.

A fight belongs to exactly one sequence step. Its battle must use the compatible
category, and all referenced fights must exist. The examples below assume `first`
and `final` are two fight handles registered by this mod; use only the function
matching your intended content, not all three on the same fights.

## sf2.modes.register


Create a progression sequence, such as a repeatable trial.

**Signature:** `sf2.modes.register(definition)`

**Requires:** `content.register`.

**When:** Entrypoint, after registering its fights.

**Returns:** `nil`.

```lua
sf2.modes.register {
    id = "training_trial",
    fights = { first, final },
    repeatable = true,
    reset_on_loss = true,
}
```

Use the sequence fields above. Every owned map battle resolves to the sequence's
current fight. Without a callback override, a win advances it. A completed repeatable sequence starts again;
a nonrepeatable one remains complete. A loss keeps or resets progress according
to `reset_on_loss`.

## sf2.events.register

Create a scheduled or permanently available event using the sequence format.

**Signature:** `sf2.events.register(definition)`

**Requires:** `content.register`.

**When:** Entrypoint, after its fights.

**Returns:** `nil`.

```lua
sf2.events.register {
    id = "open_training",
    fights = { first, final },
    minimum_level = 6,
    repeatable = true,
    -- Add starts_at/ends_at only when a real UTC schedule is intended.
}
```

Use the same fields and limits as a mode. Eligibility is checked again when the
player actually enters, not only when the map was first shown. Event registration
does not add a general Lua event bus; combat callbacks are documented separately.

## sf2.raids.register

Register a local offline raid sequence.

**Signature:** `sf2.raids.register(definition)`

**Requires:** `content.register`.

**When:** Entrypoint, after its fights with battle type `"raid"`.

**Returns:** `nil`.

```lua
-- boss_fight belongs to this mod and to a battle registered with type="raid".
sf2.raids.register {
    id = "local_boss",
    fights = { boss_fight },
    repeatable = true,
    reset_on_loss = true,
    hard_mode = false,
}
```

A raid battle without a raid sequence registration is rejected. Raid victory
requires defeating the boss; surviving a timeout does not award a win. This is
an offline content API, not matchmaking, multiplayer, or server leaderboard access.
See [offline raids](../offline-raids/) for boss health and rewards.

## on_result

**Signature:** `on_result = function(result) ... end`

**Returns:** A fight handle from this mode's `fights` roster, `"complete"`, or
`nil`. A fight handle selects that encounter for the next attempt, including
retries, backward routes and skipped encounters. `"complete"` finishes the run
and increments its saved completion count, even when the last fight was lost.
Repeatable modes then restart at the first encounter. `nil` uses the normal
win/loss progression described above. Other return values are errors.

**When:** Once when the native host settles an entered mode fight, after its
fight outcome and reward handling. Also available on event and raid registrations.
This synchronous callback cannot wait for a UI choice. It runs with a bounded
instruction budget. Failure, invalid results or budget exhaustion log a warning
and use default progression; the same native result cannot invoke it twice.

**Requires:** `content.register` to register the mode. There is no fighter or
reward capability in this callback. Other APIs still require their own manifest
capabilities. Returning a route does not change the native fight's win/loss result
or replace its reward calculation.

`result` is a fresh snapshot table. Changing it does not change the native result.

| Field | Type | Meaning |
| --- | --- | --- |
| `won` | Boolean | Native settled fight outcome. |
| `step` | Integer | One-based roster position of the fight just played. |
| `total` | Integer | Number of registered roster entries, not a route length. |
| `completions` | Integer | Saved completed runs before this result. |
| `fight_id` | String | Qualified definition ID of the fight just played. |

```lua
-- first, middle and final are fight handles already registered by this mod.
sf2.modes.register {
    id = "branching_trial", fights = { first, middle, final }, repeatable = true,
    on_result = function(result)
        if not result.won then return first end
        if result.step == 3 then return "complete" end
        if result.step == 1 and result.completions % 2 == 0 then return final end
        return nil
    end,
}
```

This alternates a short route with a full route. The selected encounter and run
completion count persist through the existing mode save path. Local Lua variables
are session memory; use the owned state API when additional state must persist.
Callback side effects are not rolled back if it later fails, so validate decisions
before changing owned state. Keep callbacks small and avoid opening a menu that
implies settlement is waiting for its buttons.

Linear completion bricks and the linear count suffix are hidden for modes with
this callback: jumping to roster entry three does not mean two fights were won.
The map still resolves each owned battle to the saved selected encounter.
See the complete [Branching Trial example](https://github.com/dawc17/ProjectEclipse/tree/main/Mods/example.branching-trial).
Use [on_prepare](#on_prepare) for generated encounters and pre-entry choices,
and [saved random streams](../random/) for repeatable draws. A complete custom
result/lobby lifecycle remains outside this contract.

## Entry, interruption, and save behavior

Entry cost applies per fight attempt. Re-entering an interrupted reserved attempt
does not charge again; a failed scene launch refunds a new reservation. Progress
and reservations survive save/load and temporary mod removal.

Keep published sequence identities stable. A saved sequence that no longer
matches its definition is rejected rather than silently reinterpreted. Test
updates on a separate save before shipping a changed sequence.

## on_prepare

**Signature:** `on_prepare = function(request, event) ... end`

**Returns:** An encounter plan to resolve immediately, or `nil` to leave the
request pending. An empty plan `{}` accepts the current fight unchanged.

**When:** The player attempts entry into a mode whose current encounter has no
saved plan. Runs before entry is charged or combat is launched. `event` contains
one-based `step`, `total`, `completions` before this attempt, and the current
blueprint's qualified `fight_id`. Return a generated plan directly, or open a
custom UI and call `sf2.modes.resolve` from its input callback later. Combat
launch is deferred until the Lua callback has returned. Repeated Fight clicks
while preparing do not create another request.

**Requires:** API 0.22 and `content.register` on mode registration. UI choices
also need `ui.create`; saved random draws need `state.read` and `state.write`.
The callback is bounded to 200,000 instructions. Invalid results/errors cancel
entry and report a diagnostic. It cannot yield a Lua coroutine.
Owned state writes and random draws are not rolled back if the callback later
fails or the player cancels. Draw only when committing a choice if cancellation
should leave the stream unchanged, as in Generated Expedition.

```lua
sf2.modes.register {
    id = "expedition", fights = { first, second, third }, repeatable = true,
    on_prepare = function(_, event)
        return {
            warriors = { opponents[sf2.random.integer("encounter_rng", 1, #opponents)] },
            level = event.step + 2,
            rounds = 1, round_time = 60,
        }
    end,
}
```

This example assumes registered fight/warrior handles and a declared integer
state field `encounter_rng`. Lua can calculate the roster, level and timing with
normal loops, conditions and functions. It does not register new global fights.

An **encounter plan** is a detached description applied to the current registered
fight blueprint. All fields are optional:

| Field | Meaning |
| --- | --- |
| `warriors` | Dense array of 1–64 owned warrior handles. Order and repetitions are retained. Omit to inherit the blueprint roster. |
| `level` | Integer 1–1000 applied to the encounter's warriors. Omit to retain their declared levels. |
| `rounds` | Integer 1–99; omit to inherit. |
| `round_time` | Integer 1–3600 seconds; omit to inherit. |

The blueprint supplies location, music, rules and rewards. Native reward
settlement and entry tickets remain under host control. Only the generated
instance changes; definitions and other fights keep their values. The host
validates native construction before saving the plan and entering combat.

Completed plans are stored with the mode's step in the player save. Reload and
failed scene-launch retries reuse the same plan without running `on_prepare` or
drawing random numbers again. Resolving a fight consumes its plan, including a
loss that retries the same step. Pending UI/closures are not saved: leaving the
scene, changing profile or disabling scripts cancels preparation. Press Fight
again to recreate a pending choice. Unknown plan save versions, changed rosters
or missing owned content are rejected with saved data preserved.

Random draws and other state writes made by Lua are not rolled back if a later
callback or scene launch fails. Draw after the player commits a choice when
canceling should not advance the random stream. Once the native save succeeds,
the chosen plan and the saved stream travel together. This is deterministic
encounter selection, not deterministic combat simulation.

## sf2.modes.resolve

**Signature:** `sf2.modes.resolve(request, plan)`

**Returns:** Nothing.

**When:** Complete a pending preparation, including from a UI callback. Reusing
a resolved/canceled request or supplying an invalid plan is an error. It marks
the request ready; native validation/save/entry occur after Lua returns.

**Requires:** API 0.22 and the pending request supplied to this script's
`on_prepare`. Forged or foreign request tables are rejected. No extra capability.

```lua
-- Inside the setup view's on_click callback:
sf2.modes.resolve(request, { warriors = { selected_opponent }, level = 4 })
sf2.ui.close(view)
```

## sf2.modes.cancel

**Signature:** `sf2.modes.cancel(request)`

**Returns:** Nothing.

**When:** Cancel an unfinished setup, for example in the view's `on_close`.
No ticket is charged and no fight starts. Repeated cancellation is harmless.
Cancellation after resolution is a no-op, so closing a successful choice view
cannot undo the selected plan. Scene/script teardown can still prevent launch.

**Requires:** API 0.22 and an owned request. No extra capability.

```lua
on_close = function() sf2.modes.cancel(request) end
```

## sf2.modes.is_pending

**Signature:** `sf2.modes.is_pending(request)`

**Returns:** `true` while awaiting a result, otherwise `false`.

**When:** Guard buttons or late callbacks against stale requests.

**Requires:** API 0.22 and an owned request. No extra capability.

```lua
if sf2.modes.is_pending(request) then
    sf2.modes.resolve(request, {})
end
```

The [Generated Expedition example](https://github.com/dawc17/ProjectEclipse/tree/main/Mods/example.generated-expedition)
combines a saved random stream, generated opponent roster and asynchronous game-
styled setup UI. Its runtime fixture tests the shipped Lua, cancellation,
deferred entry and save/reload. Full-game combat acceptance remains a playtest.
