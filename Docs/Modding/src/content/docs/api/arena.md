---
title: Arena regions and markers
description: Query native fighter geometry and own rectangular warning art in combat.
---

Use a rectangle to make an environmental hazard, capture zone or proximity gate.
Ordinary Lua decides what happens when a fighter enters it. The sensor and marker
share arena model coordinates: **positive Y points down**, X points right, and
Z is ignored. These are the coordinates returned by `fighter:snapshot()`.

The rectangle is a plain table with four required numbers:

| Field | Meaning and limit |
| --- | --- |
| `x`, `y` | Minimum X/Y corner, each finite in −10000..10000. |
| `width`, `height` | Positive size, each at most 4000. |

Unknown fields, missing values, numeric strings, infinity and NaN raise an error.
The shape is axis-aligned in arena coordinates; it inherits the arena's rendering
transform, including mirrored Y. It does not use screen pixels or UI units.

Queries test the current XY capsules in the native collision-edge list, with
each edge's radius and endpoint margins. That list is the one used by native hit
contact, including any equipment edges marked collidable. This is **geometry**:
attack intervals, invulnerability, block, shields and collision-disable flags do
not change the sensor. Boundary contact counts. There is no swept test between
poses, solid collision, actor spawning or automatic damage. Native rigs must have
1..512 collision edges. A callback may make 32 queries, shared by its fighter and
opponent; excess calls raise an error. These are per-callback bounds, not a total
frame-time guarantee for an arbitrary mod pack.

Markers are filled rectangles drawn over arena art using the native render
transform. They are presentation only, without physics, an animation or a timer.
The color accepts `#RRGGBB` or `#RRGGBBAA`; the last two digits are opacity.
At most 16 markers per script and 64 across the session may be active. Rejection
does not evict another mod's art. Markers disappear by the next presentation frame
on round end, surrender, fight exit or arena destruction, and immediately on
script disposal. Form changes keep the marker in the arena's coordinate system.
Pause keeps the current art; scheduling in `on_tick` follows simulation pause.
Handles are opaque and transient: retain them in Lua memory, never saved state.

All creation and queries require a current main fighter in an active offline
round. Title sparring, local versus, native PVP and online raids are excluded;
mod-owned offline raids may use the same contract. Missing host/rig, inactive
round and budget rejection return an error string as described below. Calling an
expired fighter or passing invalid arguments raises a Lua error.

## fighter:overlaps_rect

Query the current native collision capsules against a rectangle.

**Signature:** `fighter:overlaps_rect(rectangle)`

**Returns:** `boolean, nil` for a valid query, or `nil, error` when geometry is
unavailable. `false` means the valid rig does not overlap; it is distinct from `nil`.

**When:** Inside a supported combat callback during an active offline round.
The callable fighter reference expires at callback exit.

**Requires:** No additional capability for the callback's own fighter.

```lua
local region = { x = -160, y = -420, width = 320, height = 440 }
local inside, failure = fighter:overlaps_rect(region)
if inside == nil then
    sf2.log.warn(failure)
elseif inside then
    sf2.log.info("Inside the column")
end
```

## fighter.opponent:overlaps_rect

Query the opposing main fighter's collision capsules in the same coordinate space.

**Signature:** `fighter.opponent:overlaps_rect(rectangle)`

**Returns:** `boolean, nil`, or `nil, error` when the native rig is unavailable.

**When:** In the current combat callback during an active offline round. Check
that `fighter.opponent` exists before using it; its callable reference expires
with the callback.

**Requires:** `combat.target`.

```lua
if fighter.opponent then
    local inside, failure = fighter.opponent:overlaps_rect {
        x = -160, y = -420, width = 320, height = 440,
    }
    if inside then sf2.log.info("Opponent is in the column")
    elseif inside == nil then sf2.log.warn(failure) end
end
```

## fighter:mark_rect

Create an owned filled rectangle in the arena. The art stays fixed as fighters
move. Keep the returned marker for later recoloring or removal.

**Signature:** `fighter:mark_rect(rectangle, color?)`

**Returns:** `ArenaMarkerHandle, nil` on creation, or `nil, error` for an unavailable
host/round/render transform/shader or exhausted marker budget. The default color
is translucent yellow `#ffcc3366`.

**When:** During an active simulation callback, such as `on_tick`. Fight/round
begin and end callbacks, registration and UI cleanup cannot create markers. The marker lasts
through the current round, including pause; the fighter callable expires at
callback exit. The handle may be kept until the marker closes.

**Requires:** `presentation.visuals`.

```lua
-- Inside on_tick; marker is a Lua variable outside the callback.
if not marker then
    marker = assert(fighter:mark_rect {
        x = -160, y = -420, width = 320, height = 440,
    })
end
```

## sf2.world.set_marker_color

Update owned warning art without replacing its geometry or extending its lifetime.

**Signature:** `sf2.world.set_marker_color(marker, color)`

**Returns:** `true` if the marker was active and updated, `false` if it has closed.
Invalid colors, forged/foreign handles and extra arguments raise an error.

**When:** After marker creation, from the owning script's runtime callbacks.
Updating a closed handle cannot restart it.

**Requires:** `presentation.visuals`; a marker created by this script context.

```lua
if marker then sf2.world.set_marker_color(marker, "#ff332299") end
```

## sf2.world.is_marker_active

Check marker lifetime. This does not test whether the camera currently sees it.

**Signature:** `sf2.world.is_marker_active(marker)`

**Returns:** `true` while the round-bound marker is active, including pause,
otherwise `false`. A forged or foreign handle raises an error.

**When:** After creation; safe for a retained closed handle in its owning script.

**Requires:** `presentation.visuals`; a marker created by this script context.

```lua
if marker and not sf2.world.is_marker_active(marker) then marker = nil end
```

## sf2.world.remove_marker

Release owned arena art. Repeated removal is safe.

**Signature:** `sf2.world.remove_marker(marker)`

**Returns:** `true` if the marker was active, otherwise `false`. A forged or
foreign handle raises an error. The visual is hidden immediately; native Unity
resources are destroyed using the normal deferred destruction path.

**When:** After creation, including round/fight cleanup callbacks. Automatic
round/script cleanup still runs if the mod forgets to remove a marker.

**Requires:** `presentation.visuals`; a marker created by this script context.

```lua
if marker then sf2.world.remove_marker(marker); marker = nil end
```

For a complete six-second warning/active/recovery cycle, see Pulse Arena in the
[examples](../../examples/). It uses `on_tick`, `overlaps_rect`, owned markers,
and direct `change_health` loss. Direct health loss bypasses attack/block/armor/
critical-hit calculations and does not produce a hit reaction; choose that policy
explicitly when building an environmental hazard.
