---
title: Player profile queries
description: Read the active player's level and inventory without changing game state.
---

Available since API **0.27**. These queries read the active player's profile.
They do not describe an opponent, temporary fight equipment, or an item preview.
Declare `profile.read` and call after profile loading, such as inside a UI or
combat callback. Calling during mod loading or without an active profile raises
an error.

## sf2.profile.level

**Signature:** `sf2.profile.level()`

**Returns:** The active player's level as an integer.

**When:** After a game profile has loaded.

**Requires:** `profile.read`.

```lua
-- Inside a UI callback; view contains a text widget named "level".
sf2.ui.set_text(view, "level", "Player level: " .. sf2.profile.level())
```

This is a fresh read of profile progression. It neither changes the level nor
reports a mode's generated opponent level.

## sf2.profile.item

**Signature:** `sf2.profile.item(item)`

**Returns:** A new snapshot table with these fields:

| Field | Meaning |
| --- | --- |
| `present` | Boolean: an inventory record exists for this item. |
| `owned` | Boolean: the record exists and its quantity is greater than zero. |
| `count` | Integer native quantity; zero if no record exists. |
| `equipped` | Boolean native equipped flag; false if no record exists. |
| `upgrade` | Integer native upgrade index, or `nil` if no record exists. The native unspecified sentinel may be `-1`. |

**When:** After a game profile has loaded.

**Requires:** `profile.read` and an item handle obtained by this mod context.
Declare dependencies when obtaining another mod's item handle.

```lua
local sf2 = require("sf2")
-- Obtain handles during mod loading.
local nunchaku = sf2.items.get("core:items/weapon/weapon_nunchaku")

-- Call this function from an existing UI or gameplay callback.
local function describe_equipment()
    local item = sf2.profile.item(nunchaku)
    if item.equipped then
        return "Nunchaku equipped"
    elseif item.owned then
        return "Nunchaku in inventory"
    end
    return "Nunchaku not owned"
end
```

An empty inventory record can have `present = true` and `owned = false`.
Ownership is current state, not evidence of a past purchase. Core item handles
resolve to their native names; supported item redirects use the resolved item.
Unknown or forged handles raise an error, rather than appearing to be unowned.

Editing the returned table does not change the inventory. Call again for current
values; snapshots are not live handles. These functions expose no inventory,
currency or progression mutations, and do not create event subscriptions.

Registration/binding and controlled native roster tests pass. Full-game profile
switching and comparisons against the inventory UI remain acceptance checks.

Profile reset temporarily makes queries unavailable. Constructing an internal
comparison roster does not change which player these queries read. A newly selected
profile becomes readable after native activation and inventory preparation.
