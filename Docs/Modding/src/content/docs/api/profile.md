---
title: Player profile queries
description: Read the active player's level, inventory and learned perks without changing game state.
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
| `type` | Since 0.35: native item type, such as `"Weapon"` or `"Consumable"`; `nil` if the runtime item definition is unavailable. |
| `subtype` | Since 0.35: native subtype, such as `"Nunchaku"`; an empty string means unspecified, `nil` means the runtime definition is unavailable. |
| `present` | Boolean: an inventory record exists for this item. |
| `owned` | Boolean: the record exists and its quantity is greater than zero. |
| `count` | Integer native quantity; zero if no record exists. |
| `equipped` | Boolean native equipped flag; false if no record exists. |
| `upgrade` | Integer native upgrade index, or `nil` if no record exists. The native unspecified sentinel may be `-1`. |

**When:** After a game profile has loaded.

**Requires:** `profile.read` and either an item handle obtained by this mod context
or, since API 0.38, a qualified item ID string. Other namespaces, including `core`,
require a declared dependency. String queries do not require `content.register`.

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
Ownership is current state, not evidence of a past purchase. Type/subtype come from
the runtime item catalog and are available for unowned items too. They preserve
native spelling and case. They describe the resolved item, not a past purchase
event or temporary combat state. Missing catalog metadata does not imply that
an existing inventory record is unowned. Core item handles
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


## sf2.profile.perk

Available since API **0.34**. Read whether the active profile has learned a perk
and its stored upgrade number. This queries the learned-perk list, not temporary
combat effects, equipment enchantments or whether a trigger is currently active.

**Signature:** `sf2.profile.perk(perk)`

**Returns:** A fresh table with `learned` (boolean) and `upgrade` (integer when
learned; `nil` otherwise). Upgrade zero is a valid stored value; it does not mean
unlearned. The number is the native `UpgradeLevel`, not a count of purchases.

**When:** After the active game profile has loaded, for example in a UI or story
callback. A profile switch or perk upgrade is reflected on the next query.

**Requires:** `profile.read`. Pass a perk handle acquired in the same script
context, or, since API 0.38, a qualified perk ID string. Other namespaces, including
`core`, require a declared dependency. Strings need no `content.register` capability.
Malformed IDs, wrong categories, unavailable definitions, forged handles and queries
without an active profile raise errors.

```lua
local sf2 = require("sf2")
local cobra = sf2.perks.get("core:perks/PERK_COBRA")

-- Call after profile loading, for example from your menu's button callback.
local function describe_cobra()
    local perk = sf2.profile.perk(cobra)
    if perk.learned then
        return "Cobra upgrade: " .. perk.upgrade
    end
    return "Cobra has not been learned"
end
```

Changing this returned table changes no game state. Each call reads the active
profile and returns detached values. Disabling/resetting a profile does not leave
its old perk list available to subsequent queries.

Automated checks cover native host mapping/lifetime with controlled roster services,
Lua capability/handle rejection and snapshot isolation. Full-game learning,
upgrading, reset and save/reload acceptance remain pending.

### Querying an acquisition event

With API 0.38, `story.events`, `profile.read`, and a `core` dependency, an observer
can inspect core items discovered at runtime without obtaining handles at load time:

```lua
local sf2 = require("sf2")
sf2.story.on("item_acquired", function(event)
    -- Other mods need their own declared dependencies before querying their items.
    if event.item and event.item:match("^core:") then
        local item = sf2.profile.item(event.item)
        sf2.log.info(event.item .. " currently owned: " .. item.count)
    end
end)
```

The query reads current inventory; nested grants can make it newer than the event's
count snapshot. Unknown event items are `nil`; skip them. A qualified but unavailable
definition raises an error instead of reporting an unowned item.

## sf2.profile.equipment

Available since API **0.39**. Inspect the active profile's equipped records without
knowing their item IDs in advance.

**Signature:** `sf2.profile.equipment()`

**Returns:** A fresh contiguous array of snapshot tables. An empty array means no
records are marked equipped. Each record contains:

| Field | Meaning |
| --- | --- |
| `item` | Qualified definition ID, or `nil` when the API cannot identify the native item. |
| `type` | Native item type, or `nil` when metadata is unavailable. |
| `subtype` | Native subtype; empty string means unspecified, `nil` means metadata unavailable. |
| `count` | Native inventory quantity. |
| `owned` | Whether quantity is greater than zero. |
| `upgrade` | Stored upgrade index, possibly the native unspecified sentinel `-1`; `nil` if unavailable. |

**When:** After an active game profile has loaded, including UI and story callbacks.

**Requires:** `profile.read`. No item handles or `content.register` capability.
An unavailable profile raises an error. Enumeration includes all native equipped
records, including records from other mods. Passing a returned ID to a separate
`profile.item` query still requires that namespace's declared dependency.

```lua
local sf2 = require("sf2")
-- Call from a story condition or UI callback after profile loading.
local function wearing_katana()
    for _, item in ipairs(sf2.profile.equipment()) do
        if item.owned and item.type == "Weapon" and item.subtype == "Katana" then
            return true
        end
    end
    return false
end
```

This reads profile equipment, not a fight's temporary rule-imposed loadout or the
opponent. It includes every record marked equipped by the native inventory, rather
than assuming exactly five slots. Do not use array positions as slot identifiers;
inspect `type` and `subtype`. Records with zero quantity remain visible with
`owned = false`. Unknown items retain their available metadata and quantity.
Changing the array or its entries changes no game state; query again after an
equipment change. This function does not equip or grant anything.

Production-query/Lua fixture checks cover identity, missing metadata, empty lists,
snapshot isolation and profile unbinding using controlled inventory services.
Full-game comparisons against the equipment UI and temporary battle loadouts remain
acceptance checks.
