# Charged Strike UI

Enable and restart, then enter the third Act I tournament fight with a profile
that has unlocked it. Both the normal tournament and its separate eclipse
replay battle are patched at fight 3. A HUD near the top-right corner charges over five seconds of active combat.
Click **ARM NEXT STRIKE** when full; the next positive, unblocked player hit
receives 2x pending damage before native defensive modifiers. Each round resets
the charge and view. Campaign availability/rewards are unchanged.

The click changes ordinary Lua state. A fresh `on_damage_dealing` callback uses
the fighter capability to apply damage, so no expired fighter is retained. The
meter updates ten times per active combat second. The `on_close` callback cancels charge and any armed bonus if the HUD closes,
including native scene/view teardown. Round transitions also reset the view.

Requires API 0.21 for close notification. The HUD uses pointer buttons. Menu/modal
mounts also support keyboard/controller navigation, but HUD focus controls,
additional layout controls and custom images remain future work.
The view does not pause combat. This is an engine example, not a DE perk port.
Full-game visual and physical-input acceptance remains pending.

English and Polish strings live in `localizations/`. Each refresh resolves the current game language, falling back to English, and formats the percentage using ordinary Lua.

The HUD uses the shared game font, native button/bar assets and a 24-unit status label. It retains the game palette instead of supplying a separate theme.
