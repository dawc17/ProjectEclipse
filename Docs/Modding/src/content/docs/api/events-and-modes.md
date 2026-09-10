---
title: Events and fight sequences
description: Build repeatable modes, schedule events, require entry tickets, and track sequence progress.
---

A mode is an ordered sequence of already registered fights. It controls which
step is available, what happens after a win/loss, and whether entry costs a
mod-owned item. Events and raids share this sequence format.

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
current fight. A win advances it. A completed repeatable sequence starts again;
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

## Entry, interruption, and save behavior

Entry cost applies per fight attempt. Re-entering an interrupted reserved attempt
does not charge again; a failed scene launch refunds a new reservation. Progress
and reservations survive save/load and temporary mod removal.

Keep published sequence identities stable. A saved sequence that no longer
matches its definition is rejected rather than silently reinterpreted. Test
updates on a separate save before shipping a changed sequence.
