---
title: Story events
description: React to purchases and completed enchantments with Lua.
---

Available since API **0.28**. Declare `story.events` in your manifest.
Other operations performed by your callback still require their own capabilities.
The `level_up` event requires API **0.29**.
The `scene_enter` event requires API **0.30**.
The `item_acquired` event requires API **0.36**.
The `battle_result` event requires API **0.40**; see Battle results below.

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

Each callback receives a fresh detached table:

| Field | Meaning |
| --- | --- |
| `kind` | `purchase`, `enchantment`, `level_up`, `scene_enter`, `item_acquired` or `battle_result`. |
| `item` | Qualified item ID, or `nil` when unavailable in the catalog. |
| `recipe` | Enchantment recipe ID: owned `namespace:forge-recipes/id` or native `core:forge-profiles/id`. `nil` for purchases or unknown recipes. |
| `previous_count` | Count before `item_acquired`; otherwise `nil`. |
| `count` | Count after `item_acquired`; otherwise `nil`. |
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

**Requires:** `story.events`, an event name (`purchase`, `enchantment`, `level_up`, `scene_enter`, `item_acquired` or `battle_result`) and a Lua function.

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


### Item acquisition timing

`item_acquired` observes a positive inventory count increase through the native
item-grant routine used by purchases and rewards. It runs after that routine's
inventory update and optional auto-equip. Counts describe that one operation;
`count - previous_count` is its increase. Counts are captured at that operation's
mutation, so a nested native grant is not included again in the outer event.
Notifications follow successful routine returns: a nested operation can notify
before its outer operation. The snapshot therefore need not equal the inventory
count at callback time; use a profile query when you need current ownership. A zero count, removal, unchanged parent
upgrade or pending delivery with no count increase produces no notification.
Native exceptions and stale profile generations produce no notification.

```lua
sf2.story.on("item_acquired", function(event)
    sf2.log.info((event.item or "Unknown item") .. ": gained "
        .. (event.count - event.previous_count))
end)
```

Since API **0.37**, the native delivery-completion routine also emits acquisition
when it changes an empty inventory record to count one. Notification follows its
upgrade/level refresh and native save request. Repeating completion or delivering
an upgrade without increasing count does not emit acquisition. A grant performed
by a nested delivery quest is reported by its own grant hook, not counted again
by delivery completion.

This is not a universal inventory-change event. Direct inventory edits and profile
loading are outside these hooks. It cannot
veto a grant. A surrounding reward flow may still apply enchantments or perform
other work after this routine returns; the event does not certify completion of
the entire reward transaction or disk save. A purchase may produce both acquisition
and purchase events: subscribe to the one matching your purpose to avoid counting
the same acquisition twice. Unknown catalog items carry `item = nil`.

Production-method tests use controlled inventory services and verify count/timing,
failed grants and profile boundaries. Lua tests cover payloads and detached tables.
Full-game purchase/reward delivery and inventory persistence remain pending.


The bundled `example.story-observer` mod (API 0.37+) logs acquisition identity,
before/after counts and the delta. Enable it alongside `example.eclipse-reward`
to observe that example's native grant path. It adds no UI and changes no rewards.
Its README explains expected messages and the remaining full-game checks.

### Battle results

Since API 0.40, `sf2.story.on("battle_result", callback)` observes successful native
result processing for encounters launched through `StartFight`. It uses the same
`story.events` capability, subscription limits, cancellation and profile lifetime
as other story events. No registration capability or fake equipment perk is needed.

The event has `kind = "battle_result"` and these additional fields:

| Field | Meaning |
| --- | --- |
| `fight` | Qualified fight definition ID, or `nil` if the encounter is not in the content catalog. |
| `outcome` | `"win"`, `"loss"`, `"surrender"`, `"raid_timeout"` or `"raid_round_timeout"`. |
| `eclipse` | Boolean captured from the active roster when result processing starts. |
| `equipment` | Array of player model equipment snapshots at result processing entry, or `nil` if no player model parameters were supplied. Each entry has `item` (qualified ID or `nil`), `type` and `subtype` (native strings, or `nil`). |

```lua
local sf2 = require("sf2")
sf2.story.on("battle_result", function(event)
    if event.outcome ~= "win" or not event.equipment then return end
    for _, item in ipairs(event.equipment) do
        if item.type == "Weapon" and item.subtype == "Katana" then
            sf2.log.info("Won with a katana: " .. (event.fight or "unknown encounter"))
            break
        end
    end
end)
```

Capture happens before reward/progression callbacks. Delivery happens at the end
of successful native result processing, after the native presentation calls. It
is an observation, not authority to change the outcome. A failed launch or a native
exception does not deliver a successful result notification. Duplicate/reentrant
processing of the same captured attempt cannot deliver twice; a new encounter or
profile boundary invalidates the old attempt. Direct native result calls without
a tracked launch currently do not emit this event.

This does **not** certify all rewards or save I/O have completed. Deferred lottery
settlement remains unsupported. Equipment can include the native Skeleton slot;
use types, not array positions. Surrender paths may lack player parameters, so
`equipment = nil` differs from an empty array. Neither means to substitute the
profile inventory as historical combat equipment. Each subscriber receives fresh
Lua tables, including nested equipment entries.

Lua payload, host transport and complete native `EndFight` method fixtures pass.
The native flow fixture uses controlled reward, quest, presentation and capture
services to check ordering, repeated/reentrant completion, failure and profile
replacement. It also verifies outcome delivery while lottery loot remains deferred.
Full-game launch, result, Eclipse, timeout and settlement acceptance remain pending.
