---
title: Custom UI
description: Open owned layouts, update their widgets from Lua, and handle clicks safely.
---

Available since API **0.15**. Layout tables describe presentation; ordinary Lua
functions perform calculations and handle clicks. No expression strings or
operation lists are needed. A **view handle** identifies one live surface owned
by the script that created it. It is not save data and cannot be forged by
constructing a similar-looking table.

## Layout and lifetime

An open definition requires `id`, `mount`, and `root`. Optional `on_click` is a
Lua function with signature `function(view, widget_id)`; its return is ignored.
It runs on a button click with a bounded instruction budget. An error closes
that view and logs a diagnostic. It receives no fighter capability: change Lua
state and act through a fresh combat callback when gameplay authority is needed.

Mounts are `menu`, `modal`, and `hud`. Modals have priority over menus, then HUDs;
the newest view wins within a priority. Only the foreground view accepts input.
Menu/modal backdrops block pointer input outside their content and capture
keyboard/controller navigation. Back closes their foreground view. HUDs do not
capture keyboard navigation automatically; their buttons currently use pointer
input. Opening any view **does not pause combat**.

Layouts use a 1280×720 reference canvas and default to the center of the screen's
safe area. API **0.16** adds optional `placement` to the open definition:

```lua
placement = { anchor = "top_right", x = -24, y = 104 },
```

`anchor` defaults to `center`; supported values are `top_left`, `top`,
`top_right`, `left`, `center`, `right`, `bottom_left`, `bottom`, and
`bottom_right`. The matching edge or corner of the root aligns with the safe
area. `x` and `y` default to zero and accept finite values from -8192 to 8192
in reference units. Positive `x` moves right; positive `y` moves down.
For example, the placement above leaves 24 units on the right and 104 at the top.

Oversized roots scale down uniformly. Offsets are clamped to keep the entire
root inside the safe area; resizing recalculates from the requested placement.
Placement is fixed for the lifetime of a view; close and reopen to change it.
Native
dialogs, the title screen and restart flow suspend mod UI. Closing a scene or
disposing its script closes the affected views. An ID can be reused after its
view closes, with fresh state. Limits: eight open views per script scope, 64
mounted views per scene, 256 nodes per view, and depth 16.

Each node is a table:

| Field | Required/default | Meaning |
| --- | --- | --- |
| `id` | Required | Unique within this view; 1–64 ASCII letters, digits, `_` or `-`. View IDs use the same syntax. |
| `kind` | Required | `stack`, `row`, `column`, `scroll`, `text`, `button`, or `progress`. |
| `width`, `height` | `0` | Finite 0–8192 reference units. Both must be positive on the root. Zero gives flexible size in a row/column; use explicit dimensions inside stacks. |
| `children` | Empty | Dense array of nodes. Only containers accept children; `scroll` requires exactly one content node. |
| `gap` | `0` | Row/column spacing, finite 0–1024. Other kinds require zero. |
| `text` | `""` | Text/button label, up to 8192 UTF-16 code units. Other kinds require empty text. Plain text, wrapped and clipped; rich text is disabled. |
| `value` | `0` | Progress fraction, finite 0–1. Other kinds require zero. |
| `visible`, `enabled` | `true` | Widget state; hidden/disabled ancestors also prevent button activation. |

Rows and columns lay out their children in order; stacks center their children.
Scroll is vertical with clipped content. Default labels use the game font with
a fallback. Unknown fields, duplicate IDs, malformed arrays and invalid values
are errors. Dynamic text currently accepts plain strings; localization helpers,
images, custom themes, toggles/sliders and virtualized lists are not
part of this initial UI contract.

## sf2.ui.open

**Signature:** `sf2.ui.open(definition)`

**Returns:** An owned view handle, or raises an error if validation/mounting fails.

**When:** During script execution or callbacks with an available game UI host.
Scene changes close the resulting view; create it from the relevant lifecycle
callback when it must appear in a particular scene. Opening on every tick is
unnecessary: keep a handle and update individual widgets.

**Requires:** `ui.create` in the manifest.

```lua
local count = 0
local view = sf2.ui.open {
    id = "counter", mount = "menu",
    root = { id = "root", kind = "column", width = 320, height = 96, gap = 8,
        children = {
            { id = "count", kind = "text", width = 320, height = 40, text = "0" },
            { id = "add", kind = "button", width = 320, height = 48, text = "Add one" },
        },
    },
    on_click = function(current, widget_id)
        if widget_id == "add" then
            count = count + 1
            sf2.ui.set_text(current, "count", tostring(count))
        end
    end,
}
```

## sf2.ui.close

**Signature:** `sf2.ui.close(view)`

**Returns:** Nothing.

**When:** When the view is no longer needed, including inside its click handler.
Closing an already closed valid handle is harmless. Close removes presentation
and input ownership; it does not reset your gameplay variables.

**Requires:** A view handle from this script context; no additional capability.

```lua
sf2.ui.close(view)
```

## sf2.ui.is_open

**Signature:** `sf2.ui.is_open(view)`

**Returns:** `true` while open, otherwise `false`.

**When:** Before an update when Back, scene exit or a callback error may have
closed the view. An invalid or foreign handle raises an error, rather than false.

**Requires:** A view handle from this script context; no additional capability.

```lua
if view and sf2.ui.is_open(view) then sf2.ui.set_text(view, "count", "Ready") end
```

## sf2.ui.set_text

**Signature:** `sf2.ui.set_text(view, widget_id, text)`

**Returns:** Nothing.

**When:** Update a text or button label. The string may be empty and is limited
to 8192 UTF-16 code units. Updates do not rebuild the layout tree.

**Requires:** An open owned view and a text/button ID; no additional capability.

```lua
sf2.ui.set_text(view, "count", "Charge: " .. tostring(charge))
```

## sf2.ui.set_value

**Signature:** `sf2.ui.set_value(view, widget_id, value)`

**Returns:** Nothing.

**When:** Change a progress widget's fill to a finite fraction from 0 to 1.
Invalid values are rejected before mutation.

**Requires:** An open owned view and a progress ID; no additional capability.

```lua
sf2.ui.set_value(view, "meter", math.min(1, charge / maximum))
```

## sf2.ui.set_visible

**Signature:** `sf2.ui.set_visible(view, widget_id, visible)`

**Returns:** Nothing.

**When:** Show/hide a widget or subtree using a boolean. Hiding the root releases
its foreground input priority; showing it restores its original ordering.

**Requires:** An open owned view and a valid widget ID; no additional capability.

```lua
sf2.ui.set_visible(view, "details", show_details)
```

## sf2.ui.set_enabled

**Signature:** `sf2.ui.set_enabled(view, widget_id, enabled)`

**Returns:** Nothing.

**When:** Enable/disable interaction using a boolean. A disabled ancestor prevents
its descendants from activating. Disabling a root does not close its modal backdrop.

**Requires:** An open owned view and a valid widget ID; no additional capability.

```lua
sf2.ui.set_enabled(view, "arm", charge >= 1 and not armed)
```

The [Charged Strike example](https://github.com/dawc17/ProjectEclipse/tree/main/Mods/example.charge-ui)
combines a live HUD, a click handler, simulation ticks and a fresh outgoing-hit
callback. Managed Lua and isolated Unity fixtures cover these components; full
gameplay, physical input and visual acceptance remain pending.
