# Steady Guard Upgrades

Use a separate test profile. This mod replaces the level 2–5 perk choices:
learn Steady Guard at level 2, then upgrade it at levels 3, 4 and 5. Incoming damage
reduction changes from 10% to 20%, 30% and 40%, with matching descriptions.
Other normal combat modifiers still apply. The generic icon is reused from the
existing Phase 1 showcase; no DE art is required.

The example demonstrates ordinary Lua behavior with static upgrade parameters.
The base saved parameter remains 0.9. The selected learned UpgradeLevel overlays
0.8, 0.7 or 0.6 at callback time without rewriting that saved value. Equipment
and NPC copies use base parameters rather than the player's learned level.

Disable and restart to restore the original branches. Owned saved perk data is
retained for re-enabling. This example does not reset a profile's prior choices
or grant levels; test each choice as the profile earns the corresponding level.
A full Unity profile/upgrade/combat playtest is still pending.
