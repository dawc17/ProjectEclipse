# Katana Achievement

A small example of a procedural achievement condition using captured battle
equipment. Requires Eclipse API 0.40. Enable the mod and defeat Butcher with a
Weapon whose native subtype is Katana. Normal, Eclipse replay and intermission
gauntlet completions count; ordinary bodyguard fights do not.

The owned Blade Discipline achievement appears in the Profile achievement list.
It has no currency reward or guaranteed popup. A successful first unlock logs
`Blade Discipline unlocked`. The counter uses the existing profile persistence
path; later qualifying wins do not increment or log again.

This adds no map node, equipment, fight patch or fake perk. It does not unlock
Butcher or supply a katana. Use an eligible test profile. The condition reads
captured player model equipment, including temporary rule equipment when supplied;
it never substitutes the profile loadout when result equipment is unavailable.

Tests cover the shipped Lua predicate and registration. The existing native
counter fixture separately covers save/reload and unlock persistence. Full-game
achievement rendering, combat-equipment capture and this example's save/reload
acceptance remain pending. This is a reusable engine example, not a DE port or a
claim that the archived achievement's economy and presentation are reproduced.
