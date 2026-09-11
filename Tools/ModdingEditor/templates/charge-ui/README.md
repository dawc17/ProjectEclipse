# Charged Strike UI

Enable and restart, then enter the third Act I tournament fight with a profile
that has unlocked it. A HUD near the top-right corner charges over five seconds of active combat.
Click **ARM NEXT STRIKE** when full; the next positive, unblocked player hit
receives 2x pending damage before native defensive modifiers. Each round resets
the charge and view. Campaign availability/rewards are unchanged.

The click changes ordinary Lua state. A fresh `on_damage_dealing` callback uses
the fighter capability to apply damage, so no expired fighter is retained. The
meter updates ten times per active combat second. Closing the view does not
cancel or change gameplay state; the round lifecycle does that explicitly.

Requires API 0.16 for anchored placement. The HUD uses pointer buttons. Menu/modal
mounts also support keyboard/controller navigation, but HUD focus controls,
additional layout controls, custom images/colors and localization helpers remain future work.
The view does not pause combat. This is an engine example, not a DE perk port.
Full-game visual and physical-input acceptance remains pending.
