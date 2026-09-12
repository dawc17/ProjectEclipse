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
| `kind` | Required | `stack`, `row`, `column`, `scroll`, `text`, `button`, `progress`, `toggle`, `slider`, `image`, or `grid`. Toggle/slider require API 0.22; image requires API 0.41; grid requires API 0.43. |
| `width`, `height` | `0` | Finite 0–8192 reference units. Both must be positive on the root. Zero gives flexible size in a row/column; use explicit dimensions inside stacks. |
| `children` | Empty | Dense array of nodes. Only containers accept children; `scroll` requires exactly one content node. |
| `gap` | `0` | Row/column/grid spacing, finite 0–1024. Other kinds require zero. Grid uses this spacing on both axes. |
| `columns` | Required for grid | Integer 1–256, only on `grid`. Children fill each row from left to right. |
| `cell_width`, `cell_height` | Required for grid | Finite 1–8192 reference units, only on `grid`. The grid controls each direct child's size. |
| `text` | `""` | Text/button/toggle label, up to 8192 UTF-16 code units. Other kinds require empty text. Plain text, wrapped and clipped; rich text is disabled. |
| `value` | `0` | Progress/slider fraction, finite 0–1. Toggles reject this field; other kinds require zero. |
| `checked` | `false` | Boolean, toggles only. |
| `sprite` | Required for image | Typed handle from `sf2.assets.sprite`. Only image widgets accept this field. Images require positive width and height, preserve aspect ratio, and do not receive clicks. |
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
translation handles during UI refreshes. Custom fonts and virtualized lists are not
supported yet.

### Grid layouts

API **0.43** adds fixed-column grids for equipment, character and reward selectors.
A grid arranges existing widgets, so buttons and labels retain the original game
sprites and font. A cell may also be a column or stack containing artwork and a
button. Direct child widths/heights are overridden by the grid's cell dimensions;
image nodes still require positive authored dimensions.

```lua
local sf2 = require("sf2")
local names = { "Blade", "Spear", "Staff", "Claws", "Knives", "Axe" }
local cells = {}
for i, name in ipairs(names) do
    cells[i] = { id = "option_" .. i, kind = "button", text = name }
end
sf2.ui.open {
    id = "equipment_choices", mount = "modal",
    root = { id = "panel", kind = "column", width = 460, height = 220,
        gap = 12, children = {
            { id = "selection", kind = "text", width = 460, height = 40,
              text = "Choose equipment" },
            { id = "options", kind = "grid", width = 460, height = 108,
              columns = 3, cell_width = 148, cell_height = 48, gap = 6,
              children = cells },
        },
    },
    on_click = function(view, id)
        local index = tonumber(id:match("^option_(%d+)$"))
        if index then sf2.ui.set_text(view, "selection", names[index]) end
    end,
}
```

This example selects a label; it does not grant or equip an item. Use the selected
value in your own supported workflow. Children appear in their authored order,
starting at the upper left. Hidden children leave the layout and later cells move
forward; disabled children keep their place. Set enough grid height for all rows:
`rows * cell_height + math.max(0, rows - 1) * gap`, where
`rows = math.ceil(#cells / columns)`. Width follows the same formula with columns.
Grid dimensions do not grow automatically. Put a tall grid inside a `scroll` node
for clipping and scrolling; grids by themselves do not clip overflowing children.

Tab visits interactive descendants in row order; Shift+Tab reverses that order.
Arrows, the D-pad and the left stick navigate grids by their visible layout:
Left/Right stay on the current row and Up/Down prefer overlapping columns.
Hidden and disabled controls are skipped. Directional grid navigation does not
wrap at an edge; use Tab to reach controls outside that direction. Controls outside
the grid can be reached when they lie in the requested direction. Outside grids,
Up/Down retain ordered traversal. Left/Right adjust a selected slider, including
when it is inside a grid; reaching the slider's endpoint does not move focus away.
Focus reveals its control inside vertical scroll containers. Grids retain the
256-node limit for the entire view, including nested cell contents; virtualized
collections are not implemented.

### Image widgets

Requires API **0.41** and the usual `ui.create` capability. Put a PNG at
`assets/sprites/reward.png` in your mod, then obtain its sprite handle. Replace
`example.my-mod` with your manifest's ID:

```lua
local sf2 = require("sf2")
local artwork = sf2.assets.sprite("example.my-mod:sprites/reward")
local view = sf2.ui.open {
    id = "reward_preview", mount = "modal",
    root = { id = "paper", kind = "column", width = 400, height = 240,
        children = {
            { id = "art", kind = "image", width = 360, height = 160,
              sprite = artwork },
            { id = "caption", kind = "text", width = 360, height = 40,
              text = "Your reward" },
        },
    },
}
```

The sprite fits inside its rectangle without stretching or cropping. Use the
shared parchment container and game-font caption to keep the presentation
consistent. Images are decorative: they accept neither text, values, children,
nor style overrides. Put any background on their parent. API **0.42** adds
[`sf2.ui.set_sprite`](#sf2uiset_sprite) to replace artwork in place. Visibility and enabled
state use the existing UI functions. Closing a view does not destroy shared
loader-owned artwork. A missing or unloadable sprite fails mounting and closes
the surface rather than leaving a white missing-image rectangle.

Sprite handles retain the existing asset dependency and ownership checks. Raw
paths, strings in `sprite`, forged handles, and Unity objects are not accepted.
Automated Lua/runtime checks cover this contract; native rendering acceptance is
tracked separately from editor diagnostics.

API **0.22** adds toggles using the original checkbox sprites and sliders using
the original settings track, fill and handle. In a menu/modal, Up/Down or Tab
moves focus, Enter/Space (controller A) toggles the selected checkbox, and
Left/Right (D-pad or stick) adjusts the selected slider in steps of 0.05.
Pointer dragging is continuous. HUD controls accept pointer input.
An optional [`on_change`](#on_change) callback receives actual user changes;
programmatic setters never trigger it. Text styles also apply to toggle labels,
and `fill_color` applies to sliders. Map normalized slider values to meaningful
units in ordinary Lua, for example `seconds = 10 + value * 50`.

## sf2.ui.set_checked

**Signature:** `sf2.ui.set_checked(view, widget_id, checked)`

**Returns:** Nothing.

**When:** Set a toggle's boolean state without triggering `on_change`.

**Requires:** API 0.22, an open owned view and a toggle ID. No additional capability.

```lua
sf2.ui.set_checked(view, "challenge", true)
```

## on_change

**Signature:** `on_change = function(view, widget_id, value) ... end`

**Returns:** Ignored.

**When:** A visible, enabled toggle or slider in the foreground view changes
through user input. `value` is a boolean for toggles, a number from 0 to 1 for
sliders. The new value is committed before notification. Repeated identical
values and programmatic setters do not notify. Hidden/disabled ancestors and
native dialogs block input. A callback may update widgets or close its view.

**Requires:** API 0.22 and `ui.create` to open the view. No fighter authority is
supplied. The callback has a 200,000-instruction budget; failure closes the view
and logs an error. Already committed Lua or saved state is not rolled back.

```lua
local sf2 = require("sf2")
local challenge, duration = false, 30
sf2.ui.open {
    id = "options", mount = "menu",
    root = { id = "root", kind = "column", width = 400, height = 160,
        children = {
            { id = "challenge", kind = "toggle", width = 400, height = 48,
              text = "Challenge rules", checked = challenge },
            { id = "duration", kind = "slider", width = 400, height = 48, value = 0.4 },
        },
    },
    on_change = function(_, widget_id, value)
        if widget_id == "challenge" then challenge = value
        elseif widget_id == "duration" then duration = 10 + value * 50 end
    end,
}
```

## Widget styles

API **0.18** accepts an optional `style` table on each node. Styles are immutable
for the view's lifetime, do not inherit, and preserve game defaults when omitted.
They affect presentation only; they do not enable rich text or change input rules.

| Field | Default | Applies to |
| --- | --- | --- |
| `font_size` | `22` | Text/buttons/toggles; integer 8–128 reference units. Does not enlarge the layout box. |
| `text_align` | `center` | Text/buttons/toggles; `left`, `center`, or `right`, vertically centered. |
| `text_color` | Native dark text on parchment/buttons; pale gold on HUD labels | Text/buttons/toggles. |
| `background_color` | Native sprite colors | Containers, buttons, toggles, progress and slider tracks. Use a container behind text. |
| `fill_color` | Native combat bar colors | Progress widgets and sliders. |

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

**When:** Update a text, button or toggle label. The string may be empty and is limited
to 8192 UTF-16 code units. Updates do not rebuild the layout tree.

**Requires:** An open owned view and a text/button/toggle ID; no additional capability.

```lua
sf2.ui.set_text(view, "count", "Charge: " .. tostring(charge))
```

## sf2.ui.set_sprite

**Signature:** `sf2.ui.set_sprite(view, widget_id, sprite)`

**Returns:** `nil`.

**When:** After opening a surface, including from its click/change callbacks. Use
this to switch a character portrait, equipment icon or reward preview without
rebuilding the panel. Requires API **0.42**.

**Requires:** An open UI handle and a sprite handle created by the same script
context. The widget must be an `image`. Its size, aspect-preserving rendering,
visibility and place in the layout stay unchanged. `ui.create` is required to
create the surface; the setter grants no additional asset access.

```lua
local alternate = sf2.assets.sprite("example.selector:sprites/alternate")
-- view contains an image node whose id is "portrait".
sf2.ui.set_sprite(view, "portrait", alternate)
```

Strings, forged handles, other asset kinds, unknown widget IDs and closed views
are rejected. Assigning the current sprite again does nothing. The renderer loads
the replacement through the asset host; a missing or invalid image closes the
surface with reason `error` and reports the failure, following the normal UI
cleanup rules. Replacing an image never destroys loader-owned sprites used by
other views. To hide artwork, use `sf2.ui.set_visible`; `nil` is not a sprite.

## sf2.ui.set_value

**Signature:** `sf2.ui.set_value(view, widget_id, value)`

**Returns:** Nothing.

**When:** Change a progress widget's fill or a slider's position to a finite fraction from 0 to 1.
Invalid values are rejected before mutation.

**Requires:** An open owned view and a progress/slider ID; no additional capability. Sliders require API 0.22. Setters do not invoke `on_change`.

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
