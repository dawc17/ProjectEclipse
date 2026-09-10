---
title: Counters and achievements
description: Count custom progress, save it to the player profile, and unlock achievements.
---

A **counter** is a saved, nondecreasing integer belonging to your mod. An
**achievement** watches one counter and unlocks when it reaches a threshold.
These are local profile achievements; they are not platform/Steam achievements.

Register definitions during the entrypoint. Read or increase counters only after
the player profile loads, such as from a supported combat callback.

## sf2.counters.register

Create a counter owned by this mod.

**Signature:** `sf2.counters.register { id, maximum? }`

**Requires:** `content.register`.

**When:** Entrypoint.

**Returns:** A counter handle.

`id` is required. `maximum` is an integer from 1 through 1,000,000,000, default
1,000,000,000. New counters start at zero. Duplicate or invalid IDs fail.

```lua
local wins = sf2.counters.register { id = "wins", maximum = 1000000 }
```

## sf2.counters.get

Read your counter's saved value.

**Signature:** `sf2.counters.get(counter)`

**Requires:** `progression.read`.

**When:** A supported callback after profile load.

**Returns:** The current integer value as a Lua number.

```lua
-- Inside a callback; wins is the handle registered above.
local total = sf2.counters.get(wins)
```

The argument must be a valid counter handle from this mod context. Wrong handles,
another mod's counter, or access before profile load raise errors.

## sf2.counters.add

Increase a counter and process achievements whose thresholds it reaches.

**Signature:** `sf2.counters.add(counter, amount)`

**Requires:** `progression.write`.

**When:** A supported callback after profile load.

**Returns:** The resulting counter value as a Lua number.

```lua
-- Inside a callback, after confirming the intended event happened:
local total = sf2.counters.add(wins, 1)
```

`amount` is an integer from 0 through 1,000,000,000. Negative values, fractions,
and invalid ownership fail. The result saturates at the counter's maximum.
Lowering a definition's maximum does not erase a larger previously saved value.
The normal progression path requests a profile save.

## sf2.achievements.register

Define an achievement tied to one of your counters.

**Signature:** `sf2.achievements.register { id, counter, title, description, icon, threshold, hidden? }`

**Requires:** `content.register`.

**When:** Entrypoint, after registering the counter.

**Returns:** `nil`.

| Field | Type | Requirement/default |
| --- | --- | --- |
| `id` | String | Required local ID. |
| `counter` | Counter handle | Required; owned by this mod. |
| `title`, `description` | Localization handles | Required. |
| `icon` | Sprite handle | Required. |
| `threshold` | Positive integer | Required; within the counter's maximum. |
| `hidden` | Boolean | `false`; native hidden-achievement flag. |

```lua
sf2.achievements.register {
    id = "first_win", counter = wins, threshold = 1,
    title = sf2.localization.key("achievement.first_win"),
    description = sf2.localization.key("achievement.first_win.description"),
    icon = sf2.assets.sprite("sprites/medal"),
}
```

Create those localization keys and the medal sprite first. The Profile list shows
earned records and progress toward the next milestone. These definitions have
no reward field; rewardless achievements are marked claimed without a fake gem
claim or a guaranteed unlock popup.

## Counting the right event

Counters do not automatically listen to native events. For example, inside a
behavior attached to a player's active perk or enchantment:

```lua
on_fight_end = function(parameters, fighter, event)
    if event.won then
        sf2.counters.add(wins, 1)
    end
end,
```

Do not attach the same counting logic to several simultaneously active effects
unless you intend multiple increments. `event.won` is relative to the fighter
receiving this callback; attach player-achievement logic accordingly.

Disabling the mod preserves its saved counters and achievements. Re-enabling it
with the same IDs restores them. Renaming an ID creates separate progress;
this counter API has no rename/migration function.
