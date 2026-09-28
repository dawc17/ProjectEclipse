---
title: On-screen control texture packs
description: Replace the joystick and fight-button artwork with local PNG packs.
---

Control packs replace on-screen fight controls. Open Options → Accessibility and cycle
the control pack setting. The selection is stored locally and applies when the
next fight or dojo creates its controls. `Default` uses the original artwork.

Create a folder called `ControlPacks/My Pack/` beside the game's data folder,
inside Unity's persistent data directory, or under `StreamingAssets/`. Persistent
data packs take priority, followed by the game folder and then StreamingAssets.
Folders with the same name are matched without regard to letter case. At least
one PNG is needed for a pack to appear; reopen the option to rescan folders.
Loaded images are cached for the game session; restart after editing a pack's PNGs.

Use the original `FightButtons` sprite member name for each PNG. For example,
`Joystick_norm.png` or `FightButtons.Joystick_norm.png` replaces that joystick
sprite. Missing or unreadable members use the built-in artwork. Keep the original
aspect ratio; larger PNGs retain the original width, pivot and border proportions.
Only the `UI/Atlases/FightButtons` atlas is supported. A mod's explicit sprite
replacement takes priority over the selected local control pack.

These are image folders, with no manifest or executable scripts. They do not
change bindings or gameplay and are not part of campaign saves. No extra packs
ship with Eclipse. A missing selected folder falls back to `Default`.
