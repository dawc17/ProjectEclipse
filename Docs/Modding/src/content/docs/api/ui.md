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

API **0.21** adds optional [`on_close`](#on_close), a notification for canceling
pending choices and releasing Lua references after a live view closes.

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
| `style` | Game defaults | Optional style table, available since API 0.18; see below. |

Rows and columns lay out their children in order; stacks center their children.
Scroll is vertical with clipped content. Default labels use the game font with
a fallback. Menu/modal surfaces use the original parchment background; buttons
use the native white beveled sprite and native button tints. Progress bars use
the recovered combat bar textures. HUD roots stay transparent. Keep custom UI
consistent with the game: prefer these shared defaults and use overrides for
readability or a specific semantic emphasis. Unknown fields, duplicate IDs, malformed arrays and invalid values
are errors. Dynamic text accepts plain strings. Since API 0.17, use
[`sf2.localization.text`](../localization-patches/#sf2localizationtext) to resolve
translation handles during UI refreshes. Images, custom fonts, toggles/sliders and virtualized lists are not
part of this initial UI contract.

## Widget styles

API **0.18** accepts an optional `style` table on each node. Styles are immutable
for the view's lifetime, do not inherit, and preserve game defaults when omitted.
They affect presentation only; they do not enable rich text or change input rules.

| Field | Default | Applies to |
| --- | --- | --- |
| `font_size` | `22` | Text/buttons; integer 8–128 reference units. Does not enlarge the layout box. |
| `text_align` | `center` | Text/buttons; `left`, `center`, or `right`, vertically centered. |
| `text_color` | Native dark text on parchment/buttons; pale gold on HUD labels | Text/buttons. |
| `background_color` | Native sprite colors | Containers, buttons and progress tracks. Use a container behind text. |
| `fill_color` | Native combat bar colors | Progress widgets only. |

Colors must be `#RRGGBB` or `#RRGGBBAA` hex strings (case-insensitive); omitted
alpha means opaque. Sprite colors are multiplicative tints, so a color does not
replace the texture's shading. Buttons additionally apply their native hover,
pressed and disabled tints. Container backgrounds use parchment and do not add
pointer blocking. An explicit transparent color does not disable button input;
use `enabled` or `visible` for that. Unknown fields and styles on incompatible
widget kinds raise an error.

```lua
-- A title node inside a parchment menu; retain the native font and colors.
{ id = "title", kind = "text", width = 400, height = 48,
  text = "Battle rules", style = { font_size = 30, text_align = "left" } }
```

Text still wraps and clips within its authored dimensions. Provide enough width
and height for translations and larger text. These options do not expose custom
fonts, materials, arbitrary stylesheets or replacement of the shared game skin.

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


## on_click

Handle a button activation in the view that owns it.

**Signature:** `on_click = function(view, widget_id) ... end`

**Returns:** Ignored.

**When:** An enabled, visible button in the foreground view is activated.

**Requires:** `ui.create` to open the view; no fighter authority is supplied.

```lua
local sf2 = require("sf2")
sf2.ui.open {
    id = "choice", mount = "menu",
    root = { id = "cancel", kind = "button", width = 240, height = 48, text = "Back" },
    on_click = function(view, widget_id)
        if widget_id == "cancel" then sf2.ui.close(view) end
    end,
}
```

`widget_id` is the button's declared ID, not its label. Hiding/disabling an
ancestor prevents activation. The callback has a 200,000-instruction budget;
an error closes this view and is logged. Updating widgets and closing the view
inside the callback is supported. Use normal Lua state to pass a choice to a
later supported gameplay callback. This callback cannot itself launch fights
or acquire a fighter handle.

## on_close

Clear pending choices or Lua references when a view closes. Available since API
**0.21**. Set this optional function in the table passed to `sf2.ui.open`.

**Signature:** `on_close = function(view, reason) ... end`

**Returns:** Ignored.

**When:** Once after a successfully mounted view closes and its renderer and
input ownership have been released, while its script context is still alive.

**Requires:** `ui.create` to open the view. State operations retain their own
capability and loaded-profile requirements; no fighter authority is supplied.

```lua
local sf2 = require("sf2")
local chooser
local pending_choice
local function show_choice()
    chooser = sf2.ui.open {
        id = "choice", mount = "menu",
        root = { id = "cancel", kind = "button", width = 240, height = 48, text = "Back" },
        on_click = function(view) sf2.ui.close(view) end,
        on_close = function(view, reason)
            if chooser == view then chooser = nil end
            pending_choice = nil
            sf2.log.debug("Chooser closed: " .. reason)
        end,
    }
end
show_choice()
```

Default widgets retain the game font, parchment and native button styling.
For a complete HUD example, Charged Strike cancels its armed bonus when its
view closes. It explicitly patches both `core:fights/zone_1/tournament/3` and
`core:fights/zone_1/tournament_eclipsemode/3`: allowing a rule in both modes does
not attach it to a separate native battle automatically.

| `reason` | Meaning |
| --- | --- |
| `script` | Lua called `sf2.ui.close`, or the host explicitly closed/disposed this surface. |
| `back` | Back/Escape/controller Back dismissed the foreground menu or modal. |
| `scene` | The owning scene's UI layer stack was disposed. |
| `error` | A click or renderer update failed. |
| `destroyed` | The native view was destroyed externally while still open. |

The first close wins: repeated closes do not notify again. Scene teardown may
report `scene` or `destroyed`, depending on which native component closes first.
Within `on_close`, `sf2.ui.is_open(view)` is false. Closing it again is harmless;
widget setters fail because it no longer has live widgets.

Notification has a 200,000-instruction budget. Errors are logged and cannot
prevent the completed teardown. Opening another UI surface inside any
`on_close` callback is rejected, including after closing another view. This
prevents a cancellation callback from rebuilding a menu during scene teardown.
Open subsequent menus from a later supported callback, or after `sf2.ui.close`
returns to the caller. Other live views may be updated or closed.

No notification runs for failed mounting, script shutdown, failed entrypoint
cleanup or a disposed owner scope. Native cleanup still runs. Do not depend on
`on_close` to grant rewards, persist a final result or run shutdown logic.
Successful state writes before a later callback error are not rolled back.
