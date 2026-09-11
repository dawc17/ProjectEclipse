# Third Hit Power

API 0.12 example: in the first tournament fight in Act I, every third unblocked,
positive-damage player hit receives a 2x outgoing multiplier. The counter resets
each round. This is a custom battle rule with no equipment or DE asset dependency.

Enable it at the title screen and restart. Use a separate test profile where that
encounter remains available; the mod does not reset or unlock campaign progress.
Other native and incoming defensive rules still apply. A multi-hit move can count
more than once because each native hit triggers the callback.

The modifier requires `combat.modify_outgoing_hit`. `event.damage` is a snapshot;
multiple modifier calls multiply the live pending value. The per-rule state is
transient. Disable and restart to restore the base encounter.

Focused Lua/runtime checks cover the outgoing operation and lifecycle, but the
complete example still requires a Unity encounter playtest.
