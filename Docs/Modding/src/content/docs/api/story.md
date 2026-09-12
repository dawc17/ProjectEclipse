---
title: Story events
description: React to purchases and completed enchantments with Lua.
---

Available since API **0.28**. Declare `story.events` in your manifest.
Other operations performed by your callback still require their own capabilities.
The `level_up` event requires API **0.29**.
The `scene_enter` event requires API **0.30**.

`purchase` observes native purchase processing, not every grant or inventory change.
`enchantment` observes native forge completion. Callbacks run after native quest
evaluation returns. They cannot veto the action or replace its result; returning
a value has no effect. Delivery does not guarantee the save has reached disk.

`level_up` runs after native experience processing has updated the active roster
and its save fields. One operation crossing several thresholds produces one event
with the original and final level. A gain that reaches the cap still counts;
experience received while already at the cap does not. Loading a profile,
comparison rosters and direct level assignments do not emit this event. The event
does not depend on the unused legacy quest level-up event.

`scene_enter` observes entry into `map`, `shop`, `profile`, `dojo` or `fight`.
It is scheduled after the destination's native initialization, module registration
and widescreen setup, then delivered after a deferred frame. It requires that
the destination remains active, is still the requested scene, and the same profile
is bound. Loader/preloader/credits scenes are excluded. Returning to a scene emits
a new event; it is not replayed to subscriptions added after entry.
Unloading or disabling the destination cancels its pending entry. Re-enabling the
same object does not revive that canceled notification.

This is a scene initialization boundary, not a guarantee that native dialogs have
closed, every animation has finished, or a combat round has started. Use combat
callbacks for fighter authority. Custom UI opened here retains its normal native
dialog/input rules and closes with the scene; use its `on_close` callback for cleanup.

Each callback receives a fresh table copied before native quest evaluation:

| Field | Meaning |
| --- | --- |
| `kind` | `purchase`, `enchantment`, `level_up` or `scene_enter`. |
| `item` | Qualified item ID, or `nil` when unavailable in the catalog. |
| `recipe` | Enchantment recipe ID: owned `namespace:forge-recipes/id` or native `core:forge-profiles/id`. `nil` for purchases or unknown recipes. |
| `previous_level` | Original integer level for `level_up`; otherwise `nil`. |
| `level` | Final integer level for `level_up`; otherwise `nil`. |
| `scene` | For `scene_enter`: `map`, `shop`, `profile`, `dojo` or `fight`. Otherwise `nil`. |

Level-up events have `nil` item and recipe fields. Their values are captured at
the end of experience processing rather than at the purchase/forge quest boundary.
Scene-entry events have `nil` item, recipe and level fields.

IDs are strings, not handles. Obtain handles during loading for functions that
require them. No native objects or mutable inventory references reach Lua.

Subscriptions run in registration order. Cancellation takes effect immediately;
subscriptions added during delivery start with the next notification. Nested
notifications are queued. A callback error or its 200,000-instruction limit cancels
that subscription and logs a diagnostic; other subscriptions continue.

Limits are 64 active subscriptions per mod, 256 overall, 128 notifications and
1,024 callback invocations per root dispatch. Excess dispatch work is dropped with
a diagnostic. Registration beyond capacity raises an error.

Delivery requires an active profile. Switching profiles discards old pending events
and retains subscriptions for future events. Script unload, load failure and runtime
restart cancel subscriptions. They are not serialized or replayed. Use the mod state
API for persistent progress. Losing a handle does not cancel its subscription.

## sf2.story.on

**Signature:** `sf2.story.on(event, callback)`

**Returns:** An opaque subscription handle.

**When:** During mod loading or a callback while the script is active, including
before a profile loads.

**Requires:** `story.events`, an event name (`purchase`, `enchantment`, `level_up` or `scene_enter`) and a Lua function.

```lua
local sf2 = require("sf2")
local purchases = sf2.story.on("purchase", function(event)
    if event.item == "core:items/weapon/weapon_nunchaku" then
        -- Update declared mod state or an existing UI here.
    end
end)
```

```lua
sf2.story.on("level_up", function(event)
    sf2.log.info("Level " .. event.previous_level .. " -> " .. event.level)
end)
```

With `ui.create` also declared, a scene callback can open a menu using the normal
game-styled UI widgets. This fragment opens a Back button whenever the map is entered:

```lua
sf2.story.on("scene_enter", function(event)
    if event.scene ~= "map" then return end
    sf2.ui.open {
        id = "map_menu", mount = "menu",
        root = { id = "back", kind = "button", text = "BACK", width = 240, height = 60 },
        on_click = function(view, id) sf2.ui.close(view) end,
    }
end)
```

## sf2.story.off

**Signature:** `sf2.story.off(subscription)`

**Returns:** Nothing. Repeated cancellation is harmless.

**When:** While the script is active, including inside the subscription's callback.

**Requires:** `story.events` and a handle created by this script context. Copied,
fabricated or foreign handles are rejected.

```lua
local subscription
subscription = sf2.story.on("enchantment", function(event)
    sf2.story.off(subscription) -- Observe only the next enchantment.
end)
```

## sf2.story.is_active

**Signature:** `sf2.story.is_active(subscription)`

**Returns:** `true` while subscribed; `false` after cancellation or callback failure.
An active subscription can be waiting for a profile to load.

**When:** While the script is active.

**Requires:** `story.events` and this script context's subscription handle.

```lua
if sf2.story.is_active(purchases) then
    sf2.story.off(purchases)
end
```

Native method fixtures verify identity capture, ordering and profile boundaries.
Isolated Unity play-mode checks verify deferred scene delivery, actual scene unload,
profile changes, deactivation and reactivation. Full-game purchase/forge playback,
native scene integration and rendered menu/input behavior remain acceptance tests.

The repository's `Mods/example.story-observer` example logs all four notifications.
Enable it, make a shop purchase and finish an enchantment, then inspect Unity's
Console or the player log for `Story Observer` messages. It has no visual overlay.
Gain a level through experience to test the level notification.
Enter the map, shop, profile, dojo and fight scenes to check their entry messages.
