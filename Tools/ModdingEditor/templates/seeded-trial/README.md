# Seeded Trial

Requires Eclipse API 0.20 or later. Install this folder as a loose mod. The trial
uses existing core assets and adds no item rewards or entry cost.

Winning the first encounter chooses encounter 2 or 3 using a saved random
stream. Losing returns to encounter 1. Winning encounter 3 completes the run;
the mode repeats. The default seed is 12345. Existing saved fields retain their
values: changing the default does not reset a player's stream.

The stream advances only on a first-encounter win. Its value belongs to this
mod's declared profile state, so other mods and other stream fields do not
advance it. Normal game saves store the stream position. A later callback error
does not roll back a completed draw.

See the public Saved random streams and Events and modes references for the
complete contract. The managed fixture exercises the shipped Lua callback,
serialized profile reloads, and native mode routing with host stubs. Full-game
map presentation, physical input, and native fight acceptance remain unverified.

A map-session entry quest reveals this battle. Select the bottom map-page dots
to find its named zone; other enabled examples may initially select another page.

## Recognizing the route

| Encounter | Fighter | Appearance / weapon |
| --- | --- | --- |
| 1 | Wayfarer | Unarmed kung fu fighter |
| 2 | Needlehand | Paired sai and armored fighter |
| 3 | Storm Ronin | Nunchaku and a conical hat |

Each encounter uses a separate registered warrior with a localized name and an
existing native character template, including its portrait and loadout. Fight
IDs and roster order are unchanged, so saved routes and seeds are preserved.
