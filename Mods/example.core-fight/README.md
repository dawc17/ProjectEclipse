# Campaign Guard Rule

API 0.10 example using only core assets. Enable at the title screen, apply/restart,
then fight the first Lynx bodyguard on a test profile where that encounter remains
available. The bodyguard takes half incoming damage at or below half health. The
fight uses the dojo and the existing Samurai Spirit track.

The patch appends a Lua rule to the existing encounter. It preserves its native
rules, opponents, rewards, ID, and recorded progress. It does not unlock or reset
completed campaign content. Disabling and restarting restores the base encounter
definition. Other mods editing this fight's rule list, location, or music conflict;
different supported fields can coexist.

For deliberate replacement, use `rules = { rule }` instead of `append_rules`.
That removes all native rules on this fight. `rules = {}` clears the list. These
forms are mutually exclusive. Rule counters, when used, are transient as documented
in the battle-rule reference.

`Tools/TestFightPatches.ps1` checks registration against the canonical stage catalog,
projection, and runtime rule selection. A full Unity encounter playtest is pending.
