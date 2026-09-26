# Combo Reserve

In the second Act I tournament encounter, complete a native combo of at least
three hits and let its combo window expire. Each hit in that completed combo
provides 1% outgoing damage for five active-fight seconds, capped at 15%. New
combos do not replace a reserve that is still active. Round changes reset it.
Style transitions are logged so creators can inspect the native style names.

Enable and restart, then use a test profile where the encounter is available.
The mod uses core content only and does not unlock/reset campaign progress.
The bonus multiplies pending damage before normal defensive modifiers.

The Lua code uses ordinary conditions, arithmetic, per-round state and a fight
clock snapshot. `on_tick` clears an expired reserve even without another hit;
the hit handler also checks its deadline. Pause time does not count. There is
no custom buff icon. The native combo callback's zero
count marks expiration, and last_combo supplies the completed count.

This is a generic mechanics example, not a complete DE Relentless port: native
animation restrictions, buff presentation and the production content still need
separate work. Full Unity gameplay verification remains pending.
