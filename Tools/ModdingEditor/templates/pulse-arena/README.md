# Pulse Arena

An equipment-free timed hazard for Act I Tournament 3, normal and Eclipse. Install
this folder in `Mods/` alongside the core package, then enter that encounter.

The column is placed halfway between the fighters at the first tick, then stays
fixed for that round. It warns in yellow for two simulation seconds, becomes red
for two seconds, then disappears for two seconds. Every half second of the active
phase, either fighter whose native collision capsules touch the rectangle loses
0.025 normalized health. Leave the column to avoid it. Combat pause freezes the
schedule; round/fight exit removes the marker and HUD. No art files are required.

`fighter:mark_rect` creates arena-local warning art; `overlaps_rect` queries the
same rectangle against actual collision-rig edges with margins and radii.
`sf2.world.set_marker_color` and `remove_marker` change owned presentation.
Ordinary Lua and `on_tick` own the phase, cooldown and contact schedule.

Damage uses the existing `change_health` path. It is direct environmental health
loss, without attack/block/armor/critical-hit rules, shields or hit reactions. The
sensor ignores attack intervals and invulnerability: it observes geometry only.
It samples the current pose in XY, without swept movement, solid collision,
projectiles or new physical actors. Markers are round-bound, transient and capped.
