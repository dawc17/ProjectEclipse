# Repulse Trial

Enable this mod, Apply & Restart, enter Campaign and fight Act I Tournament stage
3 (normal or Eclipse). Press the **Repulse** HUD button during combat: the next
simulation tick queues a 40-unit retreat and a 100-unit displacement of the
opponent away from you. The button recharges after 180 active simulation frames.
Pause does not advance its cooldown. Each round starts ready and closes its HUD
at round/fight end.

The example appends a rule to existing encounters. It does not replace their
opponents, equipment, rules or rewards. Use it alongside other examples to test
composition. Both movement calls independently return acceptance; they are not an
atomic pair. The HUD says **queued**, because an accepted request can be discarded
if its body, round or script session ends before application.

`combat.motion` permits movement, `combat.target` permits the opponent call, and
`ui.create` permits the button. The click callback stores intent; `on_tick` uses
its fresh fighter handles to act. Displacement is relative in native rig units,
not pixels or velocity. It neither creates a hit nor sweeps collisions along its
path. The native constraint/friction solver can correct rig positions and velocity,
and collision processing continues on subsequent steps. The API does not validate
a safe destination. Test your ability near arena boundaries and during
attacks. Change the distances or cooldown to create your own mechanic.

See [fighter motion](https://dawc17.github.io/ProjectEclipse/guides/fighter-motion/)
for limits, callback timing, composition and verification boundaries.

The native acceptance runner boots the real game and this normal-mode encounter,
checks additive movement from separate mods, the native HUD button/cooldown,
animation continuity, pause/resume and surrender cleanup. Its fighters are
controlled and its isolated profile starts after the tutorial. First-time
onboarding, physical input, every arena/animation and exported builds are not
covered. Managed checks separately exercise next-round HUD/state reset.
