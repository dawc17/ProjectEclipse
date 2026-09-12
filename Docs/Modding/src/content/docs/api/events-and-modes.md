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
This API does not yet provide generated encounters, persistent seeded RNG,
pre-entry player choices or a complete custom result/lobby lifecycle.

## Entry, interruption, and save behavior

Entry cost applies per fight attempt. Re-entering an interrupted reserved attempt
does not charge again; a failed scene launch refunds a new reservation. Progress
and reservations survive save/load and temporary mod removal.

Keep published sequence identities stable. A saved sequence that no longer
matches its definition is rejected rather than silently reinterpreted. Test
updates on a separate save before shipping a changed sequence.
