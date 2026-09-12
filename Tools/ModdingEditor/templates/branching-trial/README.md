# Branching Trial

Enable and restart, then select the added map zone and its Branching Trial battle.
The first run takes encounters 1 → 3; the next takes 1 → 2 → 3. Routes alternate
using the saved completion count. Losing returns to encounter 1. The native game
settles each fight; Lua only chooses the next registered encounter or finishes
the run. The example grants no rewards and charges no entry item.

The selected encounter persists across save/load. Keep the three fight IDs and
their roster order stable when updating this mod. Linear completion bricks are
hidden because a skipped encounter is not a completed encounter. This demonstrates
result-driven branching, not generated fights, seeded randomness or a custom lobby.
The actual Lua example runs both routes through the native mode host fixture,
with save reloads between encounters, duplicate results and interrupted-entry
resume checks. Native fight simulation/scene loading are not part of that fixture;
full-game playtesting remains pending.

A map-session entry quest reveals this battle. Select the bottom map-page dots
to find its named zone; other enabled examples may initially select another page.

## Recognizing the route

| Encounter | Fighter | Appearance / weapon |
| --- | --- | --- |
| 1 | Gatekeeper | Kunai and a green mask |
| 2 | Bulwark | Steel batons and a heavy build |
| 3 | Night Warden | Ninja sword and a robe |

Each encounter uses a separate registered warrior with a localized name and an
existing native character template, including its portrait and loadout. Fight
IDs and roster order are unchanged, so saved routes and seeds are preserved.
