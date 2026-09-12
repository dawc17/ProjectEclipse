---
title: Scene navigation
description: Use native menu transitions from custom Lua UI.
---

Available since API **0.31**. Declare `presentation.navigate`. This API uses the
same native transition as the game's menu, including its quest and tab checks.
It does not expose Unity scene objects, raw scene indexes or arbitrary scene loading.

## sf2.scenes.open

**Signature:** `sf2.scenes.open(destination)`

**Returns:** `true` if the native transition accepted the request or that scene
is already current. `false` if navigation is currently unavailable or a native
quest/tab gate consumed the request. A `true` result is not loading completion;
observe `scene_enter` with [story subscriptions](../story/) for destination entry.

**When:** After a profile and a menu scene have initialized. Intended for custom
menu buttons. Navigation is rejected while a transition is underway, encounter
preparation is pending, native input is blocked, a lock screen is active, or the
source is a fight, loader, preloader or credits scene. Navigation during a UI
`on_close` callback raises an error: cleanup must not initiate another transition.

**Requires:** `presentation.navigate`. `destination` is exactly one of `map`,
`shop`, `profile` or `dojo`. Other values raise an error. Calling without a host
navigation service also raises an error. Opening UI separately requires `ui.create`.

```lua
local sf2 = require("sf2")
-- Use this callback in a UI containing a button with id="shop".
local function on_click(view, id)
    if id == "shop" and sf2.scenes.open("shop") then
        sf2.ui.close(view)
    end
end
```

Requests do not forcibly dismiss native dialogs, surrender fights, replace result
settlement, or cancel pending encounter preparation. Finish/cancel your mode request
through its existing workflow first. Native quest interception may perform its own
action even when this function returns `false`; do not retry in a loop.

The repository's `Mods/example.scene-menu` opens game-styled travel buttons on
menu-scene entry. Enable it, enter the map and select SHOP, PROFILE or DOJO. BACK
closes the example. Choosing the current destination also closes it without reloading.

Native method fixtures verify gating and preservation of quest/tab checks. Actual
Lua tests exercise the example's buttons and rejection message. An isolated Unity
play-mode fixture also runs the shipped example through the production renderer
and input bridge, checking original-game font/sprites, button bounds, native-dialog
blocking, directional selection/submit, rejection text and cleanup/remounting.
Its navigation host is controlled: full-game transitions, visual acceptance in the
native scenes and physical keyboard/controller testing remain to be verified.
