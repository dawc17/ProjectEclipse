---
title: Activate an authored move
description: Build a Lua ability that starts a registered attack and observes its playback receipt.
---

A registered move defines animation data, attack windows, effects and transitions.
Use `fighter:play_move(move)` to start it from an active Lua combat callback.
Declare `combat.animation` in your manifest; opponent playback also requires
`combat.target`. The [fighter reference](../../api/fighter/#fighterplay_move)
documents exact timing, receipts and limits.

The complete [Active Strike source](https://github.com/dawc17/ProjectEclipse/tree/main/Mods/example.active-strike)
adds a HUD ability to Act I Tournament stage 3 in normal and Eclipse mode. Equip
fists, enable the mod, Apply & Restart, enter the fight and press **Active Strike**.
The example ships the base game's compatible punch binary as a mod-owned asset,
registers its own attack windows and uses Lua for activation and cooldown.

## Request from a fresh callback

The HUD click stores a boolean intent. A later `on_tick` receives a fresh fighter
handle, checks readiness and calls `play_move`. Do not retain the fighter table
in a UI callback: its methods expire when the combat callback returns.

```lua
-- strike was returned by sf2.moves.register during loading.
-- pending belongs to this behavior's transient Lua bookkeeping.
on_tick = function(self, fighter, tick)
    if pending and pending.status == "applied" then
        self.state.cooldown = 120
        pending = nil
    elseif pending and pending.status == "failed" then
        if pending.error then sf2.log.warn(pending.error) end
        pending = nil
    end
    -- Your own activation/readiness logic decides when to make this request.
    if requested and not pending and self.state.cooldown == 0 then
        requested = false
        pending = fighter:play_move(strike)
    end
end
```

This fragment assumes a declared integer `cooldown` state field, plus your own
cooldown decrement and intent handling. The complete example supplies those
pieces and HUD cleanup. Receipts can be retained between ticks; they are not
fighter handles and do not provide further engine operations. Never serialize
them into saved state. `applied` means the native animation started; it does not
confirm completion, a successful hit or damage. Observe animation callbacks and
normal combat events when your mechanic needs those outcomes.

## Choose the ability's rules

Explicit playback does not simulate a key press or ask native AI/input selection
to choose a move. Selection events, conditions, priority, cancel windows and
player control restrictions do not gate the request. It can interrupt an active
animation. The fighter must still be alive and eligible, the move must be present
in its current native animation cache, and native readiness/physics must allow
playback. Equipment/rig/locks determine that cache.

Decide costs, cooldown, current-state gating and cancellation policy in Lua.
For example, Active Strike declines activation while the snapshot's current
animation is an attack. Another ability can deliberately interrupt an attack.
Animation, effects, collision and damage still come from the typed move
definition. See [move authoring](../../api/moves-and-tactics/) for the binary,
rig, interval and transition contract.

## Combine abilities and verify them

There is one pending playback slot per main fighter per step, shared across mods.
The first accepted request wins; a competitor receives a failed receipt. Requests
for different bodies are independent. Motion applies before playback at the end
of the step, so an ability can combine both operations, but they are not atomic.
Pause retains queued playback. Teardown, stale sessions and replaced/dead bodies
fail it, without replaying it after the state changes.

Test the actual animation, contact windows, damage, interruption, failed requests,
cooldown, pause and round cleanup. Combine it with another ability and inspect
[callback diagnostics](../runtime-diagnostics/). Managed and editor checks
exercise contracts; they do not replace native combat acceptance. See the
example README and engineering acceptance record for verified scenarios and
remaining input/arena/export limits.
