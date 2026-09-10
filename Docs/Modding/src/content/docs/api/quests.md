---
title: Quests and map actions
description: React to story events, reveal battles, show dialogs, and use the supported declarative quest fields.
---

Quests connect game events to a supported list of actions. A simple quest can
reveal your map battle at session startup. The current interface is a typed
compatibility format: it does not take arbitrary Lua quest callbacks. Put custom
combat calculations in behavior functions instead.

## sf2.quests.register

Create a quest that responds to selected game events.

**Signature:** `sf2.quests.register(definition)`

**Requires:** `content.register`, plus dependencies for referenced content.

**When:** Entrypoint, after the battle/fight/item definitions used by the quest.

**Returns:** A quest handle.

| Field | Type | Default | Meaning |
| --- | --- | --- | --- |
| `id` | String | Required | Stable local quest ID. |
| `priority` | Integer | `0` | Native quest ordering priority. |
| `unresumable` | Boolean | `false` | Native non-resumable quest flag. |
| `allow_doubles` | Boolean | `false` | Allow duplicate active instances under native quest rules. |
| `place` | String | `"map"` | `"map"`, `"fight"`, or `"dojo"`. |
| `groups`, `marks` | String arrays | Empty | Native quest grouping/mark metadata. |
| `events` | Event-name array | Required | At least one supported event. |
| `conditions` | Condition array | Empty | Conditions that must hold. |
| `actions` | Action array | Required | Supported actions to perform. |

```lua
-- battle is the handle created by sf2.battles.register.
sf2.quests.register {
    id = "reveal_training", place = "map", events = { "session" },
    actions = {
        { type = "show_battle", battle = battle, locked = false },
        { type = "map_focus", battle = battle },
    },
}
```

Use dense arrays starting at index 1; do not leave holes. Unknown fields and
unsupported action/condition names are errors. Registration alone does not run
the quest immediately; its event and conditions control activation.

## Supported events

| Event | Context |
| --- | --- |
| `session`, `activate` | Session initialization or quest activation. |
| `fight_enter`, `fight_end` | Ordinary fight entry/result. |
| `raid_fight_enter`, `raid_fight_end` | Registered offline raid fight entry/result. |
| `raid_enter`, `raid_end` | Raid attempt entry or sequence completion. |
| `reset_mode` | Mode reset. |
| `raid_map_enter`, `raid_floor_changed`, `show_raid_loot` | Raid map/progress/loot events. |
| `level_up`, `got_item`, `set_item_acquired` | Player progression and acquisition. |
| `purchase`, `delivery`, `timer_end` | Purchase, delivery, or timer completion. |
| `enchantment`, `activate_perk`, `deactivate_perk` | Enchantment/perk activity. |
| `dialog`, `map_button`, `scene_loaded`, `shop_enter` | UI and scene events. |

These names select existing host events; they do not create new gameplay systems.
Filter a fight-end quest to your own fight so it does not run after every battle.

## Conditions and operands

A comparison is `{ op = "eq", left = ..., right = ... }`. Operators are `eq`,
`gt`, `gte`, `lt`, and `lte`. Optional `not = true` negates the result. Because
`not` is a Lua keyword, write the key as `["not"] = true` in actual Lua code.

Group conditions with `{ op = "all", conditions = { ... } }` or `op = "any"`.
A group also accepts `["not"]`; do not add `left`/`right` to a group.

Operands may be string, number, or boolean literals, or these tables:

| Operand | Fields | Meaning |
| --- | --- | --- |
| `{ kind = "variable", name = "..." }` | Required string `name` | Native user variable. |
| `{ kind = "event_fight" }` | None | Fight associated with the current event. |
| `{ kind = "fight_result" }` | None | Native fight result value. |
| `{ kind = "current_battle" }` | None | Current battle identity. |
| `{ kind = "fight_wins", fight = handle }` | Fight handle | Saved win count for that fight. |
| `{ kind = "fight_id", fight = handle }` | Fight handle | Identity of a specific registered fight. |

```lua
-- As the conditions field of a quest responding to fight_end:
conditions = {
    {
        op = "eq",
        left = { kind = "event_fight" },
        right = { kind = "fight_id", fight = training_fight },
    },
}
```

`sf2.state` fields are a separate typed save API; a quest `variable` operand does
not automatically read them. Use this compatibility format only for its supported
host semantics, not as a general arithmetic or programming language.

## Supported actions

| `type` | Additional fields | Effect |
| --- | --- | --- |
| `show_battle` | Required `battle` handle; `locked=false` | Reveal a map entry with a lock state. |
| `toggle_battle` | Required `battle`; `visible=true` | Show or hide an entry. |
| `map_focus` | Required `battle` | Focus its map entry. |
| `fight` | Required `fight` handle | Start that fight. |
| `current_fight` | None | Start the currently selected fight. |
| `eclipse` | `enabled=true` | Toggle Eclipse mode. |
| `update_eclipse_battles` | None | Refresh Eclipse battle presentation. |
| `give_item` | Required `item` handle | Grant an item through the host. |
| `set_variable` | Required strings `name`, `value` | Set a native user variable. |
| `dialog` | `title=""`, `image=""`, required `lines`; optional `button` | Display a dialog. |
| `story` | Required `lines` | Display a story screen. |

Dialog/story `lines` are strings or tables `{ text, button?, frames? }`.
`text` is required; `button` defaults to `""` and `frames` to `0`.
Presentation strings may be supported localization references.

A dialog's optional button is `{ text = "...", color = "Beige", actions = { ... } }`:
`text` and `actions` are required, while `color` defaults to `"Beige"`.
Button actions use the same supported action shapes.

```lua
-- As an action in a quest:
{
    type = "dialog",
    title = "my.mod:localization/battle.title",
    lines = { "my.mod:localization/battle.introduction" },
    button = {
        text = "my.mod:localization/battle.begin",
        actions = { { type = "fight", fight = training_fight } },
    },
}
```

Use either a fight reward or a quest item grant for an intended single reward;
using both awards twice.
