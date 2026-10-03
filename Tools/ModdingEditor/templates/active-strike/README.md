# Active Strike Trial

Equip fists, enable this mod, Apply & Restart and enter Act I Tournament stage 3
in normal or Eclipse mode. Press **Active Strike** during combat. The button
stores intent, and the next tick queues its registered punch. A playback receipt
confirms startup before the 120-frame cooldown starts. Pause does not advance
cooldown. Round/fight teardown closes the HUD and discards transient intent.

The move is owned by this mod. `assets/animations/high_punch.bytes` is an unchanged
copy of the base game's `Assets/Resources/gamedata/animations/binary/high_punch.bytes`.
Its Lua definition declares compatible fists/Skeleton locks, contact edges,
attack timing, native damage terms, facing/alignment, impulse and sounds. It has
no selection events; the ability starts it explicitly. The API requires a move
handle registered by this script, not an arbitrary engine animation name.

`combat.animation` grants explicit playback. It does not run native input/AI
conditions, priority or cancel-window checks and can interrupt animations. This
example chooses to decline activation during another attack. Define your own
readiness, costs and cancellation policy in Lua; native model readiness/physics
and current move availability still apply. A receipt's `applied` status means
startup, not completion or contact. One queued playback per body/step is shared
across mods; competitors fail instead of overwriting it.

Managed checks exercise the real example's HUD, receipts, cooldown, rejection and
next-round cleanup with controlled models. Native acceptance uses real boot,
Campaign/core combat, the shipped move/button, animation events/contact damage,
pause retention and surrender cleanup. Input/AI and spacing are controlled; the
isolated profile starts after the tutorial. First-time onboarding, physical input,
all equipment/arenas/animations, Eclipse-mode combat and exported builds need
broader playtesting. Native artifacts are kept under ignored `Temp/`.

See [the ability guide](https://dawc17.github.io/ProjectEclipse/guides/move-abilities/)
and [fighter reference](https://dawc17.github.io/ProjectEclipse/api/fighter/#fighterplay_move).
