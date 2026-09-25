---
title: Story events
description: React to story events and present resumable dialogue sequences.
---

Declare `story.events` in your manifest.
Other operations performed by your callback still require their own capabilities.
See Battle results below for the `battle_result` event.

Local Versus does not dispatch campaign story callbacks, including battle-result
progression, and does not award campaign rewards. Its fighters use detached
standard loadouts. Mod content projection is still loaded, so this should not be
read as a general mod-disable mode. The public story API itself is unchanged.
Local bootstrap uses a temporary cloned profile document. Profile changes and mod
state migrations made while that local profile is active are discarded rather
than persisted to the campaign save, including delayed save/authentication work
that arrives after returning to the title screen.

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

`map_button` observes a native map-button press after native quest processing.
The event includes the button's native name. A mod's `show_map_button` action
names its button `<mod-id>.<id>`; check that exact name before opening UI or
navigating. It requires an active profile and follows the same subscription
lifetime and error rules as other story events.

`dojo_button` observes a press of a button registered with
[`sf2.ui.dojo_button`](../ui/#sf2uidojo_button). Its `button` field is the same
`<mod-id>.<id>` name that registration returned. It requires an active profile
and follows the same rules as `map_button`.

Each callback receives a fresh detached table:

| Field | Meaning |
| --- | --- |
| `kind` | `purchase`, `enchantment`, `level_up`, `scene_enter`, `map_button`, `dojo_button`, `item_acquired` or `battle_result`. |
| `item` | Qualified item ID, or `nil` when unavailable in the catalog. |
| `recipe` | Enchantment recipe ID: owned `namespace:forge-recipes/id` or native `core:forge-profiles/id`. `nil` for purchases or unknown recipes. |
| `previous_count` | Count before `item_acquired`; otherwise `nil`. |
| `count` | Count after `item_acquired`; otherwise `nil`. |
| `previous_level` | Original integer level for `level_up`; otherwise `nil`. |
| `level` | Final integer level for `level_up`; otherwise `nil`. |
| `scene` | For `scene_enter`: `map`, `shop`, `profile`, `dojo` or `fight`. Otherwise `nil`. |
| `button` | Button name for `map_button` and `dojo_button`; otherwise `nil`. |

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

**Requires:** `story.events`, an event name (`purchase`, `enchantment`, `level_up`, `scene_enter`, `map_button`, `dojo_button`, `item_acquired` or `battle_result`) and a Lua function.

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

```lua
sf2.story.on("map_button", function(event)
    if event.button == sf2.mod.id .. ".arena" then
        sf2.log.info("Arena map button pressed")
    end
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

The native delivery-completion routine also emits acquisition
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


The bundled `example.story-observer` mod logs acquisition identity,
before/after counts and the delta. Enable it alongside `example.eclipse-reward`
to observe that example's native grant path. It adds no UI and changes no rewards.
Its README explains expected messages and the remaining full-game checks.

### Battle results

`sf2.story.on("battle_result", callback)` observes successful native
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


## sf2.story.before_fight

**Signature:** `sf2.story.before_fight(fight, on_before_fight) -> nil`

**Returns:** Nothing. Duplicate registrations, foreign/core fight handles, missing capability or host support raise an error.

**When:** Register after creating an owned fight. The handler runs on the progression map before native fight-entry quest processing, encounter commitment and combat loading. Existing mode preparation/resolution runs first. Launches from other scenes retain their native path without invoking this handler (including in-fight retries).

**Requires:** `story.progression`. Creating the fight separately requires `content.register`.

Only a fight handle created by this script context is accepted. One handler may own each fight; limits are 64 handlers per mod and 256 per session. Handlers live until script unload or callback failure. They are not saved. They can also guard fights belonging to a custom mode; resuming retains normal mode entry processing and costs.

Return `true` to allow the original native entry immediately, `false` to cancel it, or `nil` to hold it while presenting UI. A held entry blocks competing fight launches. It grants no authority to select a different fight or skip native entry quests. The handler must eventually resume or cancel its request; it has no automatic timeout. Profile replacement, scene changes, restart and script unload invalidate held requests. Persist your own acknowledgement flags if the presentation should run only once.

```lua
-- first_fight is an owned handle registered earlier.
sf2.story.before_fight(first_fight, function(request)
    sf2.ui.open {
        id = "entry", mount = "modal",
        root = {id = "begin", kind = "button", width = 300, height = 80, text = "Begin"},
        on_click = function(view)
            sf2.ui.close(view)
            sf2.story.resume_fight(request)
        end,
        on_back = function(view)
            sf2.story.cancel_fight(request)
            sf2.ui.close(view)
        end,
    }
    return nil
end)
```

## on_before_fight

**Signature:** `on_before_fight(request) -> boolean|nil`

**Returns:** `true` continues immediately; `false` cancels; `nil` defers. Other return types raise an error. Callback errors and instruction-budget exhaustion cancel the entry, remove its handler and log a diagnostic.

**When:** When the registered owned fight is about to enter from the active progression map. `request.fight` is its qualified identity. Other request data is opaque; copying its table does not copy authority. Request functions only accept the original table in its owning script context.

**Requires:** A `sf2.story.before_fight` registration with `story.progression`.

Do not call `resume_fight` while this callback is running; return `true` for immediate entry. Deferred callbacks such as button clicks or act-screen completion may resume later. Each callback has the normal 200,000-instruction budget.

```lua
sf2.story.before_fight(first_fight, function(request)
    sf2.log.info("Entering " .. request.fight)
    return true
end)
```

## sf2.story.resume_fight

**Signature:** `sf2.story.resume_fight(request) -> boolean`

**Returns:** Whether the retained native entry was accepted. Invalidated/consumed requests return `false`. A temporarily blocked map returns `false` while leaving the request pending. Once native entry is attempted, the request is consumed even if native entry refuses it.

**When:** After the entry callback returns, while the same profile and map scene remain active. Unavailable during UI cleanup callbacks. Close your dialogue before resuming. Resume skips this handler once, then runs the ordinary native entry path with the original arguments; acceptance may include a native entry quest, not necessarily immediate combat.

**Requires:** `story.progression` and an original request from this context.

```lua
-- In an acknowledgement callback:
sf2.ui.close(dialogue)
local accepted = sf2.story.resume_fight(request)
if not accepted then sf2.story.cancel_fight(request) end
```

## sf2.story.cancel_fight

**Signature:** `sf2.story.cancel_fight(request) -> nil`

**Returns:** Nothing. Cancelling an already consumed/cancelled request is harmless.

**When:** Abandoning a held entry, including dialogue cancellation. This does not close UI or modify saved acknowledgement flags for you. It cannot cancel a fight that already launched.

**Requires:** `story.progression` and an original request from this context.

```lua
sf2.story.cancel_fight(request)
```

## sf2.story.fight_pending

**Signature:** `sf2.story.fight_pending(request) -> boolean`

**Returns:** Whether this request can still be resumed. A pending request may still be temporarily blocked by native presentation/input rules.

**When:** Before continuing asynchronous presentation. Scene/profile changes and script unload make it false. Foreign, copied or fabricated request tables raise an error.

**Requires:** `story.progression` and an original request from this context.

```lua
if sf2.story.fight_pending(request) then
    sf2.log.info("Entry is still waiting for acknowledgement")
end
```

## sf2.story.play_sequence

**Signature:** `sf2.story.play_sequence(definition)`

**Returns:** `boolean`: true when playback is accepted (possibly completed synchronously), false when another story sequence/dialog/act screen in this context is active or the host refuses the first step. Invalid definitions and unavailable hosts raise an error.

**When:** In a story or fight-entry callback after profile binding. Playback cancels on scene entry, native presentation cancellation, profile rebinding or context disposal. Call it again from the appropriate story event to resume.

**Requires:** `story.events` and `ui.create`. A saved `position` additionally requires `state.read`, `state.write` and a registered integer state field with default `1`.

A sequence is a dense array of **1–64 steps**. Each step has exactly one `dialog` or `act_screen` payload. These use the same fields and limits as [story dialogs and act screens](../ui/), but cannot contain their own callbacks. All payloads are validated and copied before the first screen opens. A dialog step advances after its final page is acknowledged; an act-screen step advances when it finishes.

| Field | Meaning |
| --- | --- |
| `steps` | Required ordered array of `{ dialog = {...} }` or `{ act_screen = { lines = {...} } }`. |
| `position` | Optional name of an owned integer state field. Its value must be in `1..#steps+1`. Omit for a transient sequence that starts at step 1 each call. |
| `on_complete` | Optional Lua function called after the final acknowledgement. |
| `on_cancel` | Optional Lua function called on cancellation or host refusal. Teardown and profile changes suppress this callback. |
| `on_step(index)` | Optional Lua function before opening each step, including resumed steps. Return `false` to cancel before showing the step, for example when `sf2.story.fight_pending(request)` is false. Use ordinary Lua here for presentation side effects. It may run again after interruption, so keep those effects safe to repeat. |

The runtime saves the next step **before** opening it. Cancellation leaves the cursor unchanged. Completion leaves it at `#steps+1`; update your pending/completed fields in `on_complete`, and reset the cursor to `1` yourself before replaying. Calling with a completed cursor invokes `on_complete` again without opening UI, allowing completion work to retry after an error. Save writes use the normal mod-state system and do not force an immediate disk flush. Keep step ordering stable or migrate the cursor with your state schema when changing a shipped sequence.

Callbacks use the normal bounded Lua execution budget. A failed callback stops playback and is reported; later event-driven calls can retry. Duplicate and stale native callbacks cannot advance a newer sequence or another profile. Cancellation callbacks run during cleanup and cannot open UI, navigate, or resume fights. Completion callbacks can start another sequence, with recursive completion limited to eight levels.

```lua
local next_card = "intro_next"
sf2.state.register {
    version = 1,
    fields = {
        intro_next = { type = sf2.state.INTEGER, default = 1 },
        intro_done = { type = sf2.state.BOOLEAN, default = false },
    },
}
-- Resolve these localization handles while registering your content.
local welcome = sf2.localization.key("welcome")
local ok = sf2.localization.key("ok")

sf2.story.on("scene_enter", function(event)
    if event.scene ~= "map" or sf2.state.get("intro_done") then return end
    sf2.story.play_sequence {
        position = next_card,
        steps = {
            { act_screen = { lines = { { text = welcome, frames = 180 } } } },
            { dialog = { lines = { { text = welcome } }, button = ok } },
        },
        on_complete = function() sf2.state.set { intro_done = true } end,
    }
end)
```

Keep branching, fight resumption, rewards and pending-story selection in Lua callbacks. The sequence API only presents the declared steps and tracks acknowledgement.
