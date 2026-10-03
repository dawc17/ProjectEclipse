---
title: Create a movement ability
description: Build a repulse, dash or blink using Lua and queued fighter displacement.
---

Use `fighter:move_by(x, y, z?)` to request relative displacement of a living
fighter. Add `combat.motion` to your manifest. To move its opponent, also declare
`combat.target` and use `fighter.opponent:move_by(...)`.

The complete [Repulse Trial source](https://github.com/dawc17/ProjectEclipse/tree/main/Mods/example.repulse)
adds a HUD button to Act I Tournament stage 3 in normal and Eclipse mode. It
retreats the player and pushes the opponent away, with a three-second cooldown.
The example keeps button intent in Lua and consumes it in `on_tick` using fresh
fighter handles. Do not keep a fighter table for later UI or timer callbacks;
its operations expire when the combat callback returns.

Here is the displacement portion of an active combat callback:

```lua
local view = fighter:snapshot()
if view and view.round_active and view.opponent and fighter.opponent then
    local away = view.self.position.x <= view.opponent.position.x and -1 or 1
    local accepted, reason = fighter.opponent:move_by(-away * 100, 0)
    if not accepted and reason then sf2.log.warn(reason) end
end
```

This fragment needs an attached behavior and the two capabilities above. Read
the [fighter reference](../../api/fighter/#fightermove_by) for exact arguments,
returns and timing.

## Choose the movement you want

`x`, `y` and `z` are offsets in the native rig's coordinate system. Positive X
moves toward increasing X regardless of facing; positive Y moves downward (negative Y moves upward). Z
defaults to zero. These are neither screen pixels nor per-second velocities.
For a single dash or blink, make one request when the ability fires. Repeating
it every tick produces repeated displacement. This API does not create an
attack, damage, velocity impulse, invulnerability or a custom animation.

The API does not validate a safe destination. Movement applies after that step's
model, collision and animation processing. Its native constraint/friction solver
can correct rig positions and velocity, including configured wall/floor constraints;
further collisions continue on subsequent steps. It does not check every point
along the path. Use modest distances and test near walls, in the air, during
attacks and with other rules. A vertical displacement alone is not a complete
jump mechanic.

## Understand acceptance and composition

The method returns `true, nil` when the host accepts a request. Position snapshots
inside the same callback still describe the original position. The host adds
accepted offsets for each main fighter and applies one translation at the end of
that simulation step, before round results and interpolation capture. It moves
both the rig and running animation buffers to prevent immediate snapback.

Each individual offset and running combined offset must stay within
`-1000..1000` on each axis. At most 32 nonzero requests can be pending for one
fighter in one step, shared across mods and behaviors. Opposing offsets can
cancel. Requests are processed in callback order; a rejected request leaves
earlier accepted offsets intact. The limits apply to the running sum, so a large
intermediate sum can be rejected even if a later offset would cancel it. Zero
offsets consume no request slot but still require an eligible fighter.

Acceptance is not a completion receipt. If the fighter dies, is replaced or
detached, or the round/session ends before application, its request is discarded.
Pending requests survive a pause until simulation resumes. Two calls, including
one for each participant, are independent and do not form an atomic operation.
No callback runs inside native translation.

## Verify your ability

Use the example as a starting point and run a fight after each change. Check
direction, displacement, animation continuity, cooldown, pause/resume and round
cleanup. Combine it with other rules and inspect errors through
[callback diagnostics](../runtime-diagnostics/).

Managed checks exercise argument/capability/lifetime validation and the production
queue with controlled models. Native acceptance exercises real fighters and
animation translation in an isolated Unity fixture; consult the example README
and engineering acceptance record for the precise scenarios verified. Editor
completion and static validation alone do not establish game behavior. Custom
actors, physics systems, arena geometry and arbitrary attack animations remain
separate capabilities.
