---
title: "Installing and enabling mods"
description: "Installing and enabling mods in the Eclipse modding API."
---

Place the complete mod folder in your installation's `Mods` directory, with
`mod.toml` directly inside that folder. Extract downloaded archives first. Do
not install two folders with the same manifest ID. Restart Eclipse after adding
or updating mod files.

Open **Mods** directly from the title screen. Toggle the installed mods, then
choose **Apply & Restart**. The game saves and reloads to the title screen;
enter Campaign to load the new selection. **Back / Cancel** discards unapplied
changes. In game, open the main **Menu** and choose **Return to Title** to reach
the mod list again.

Enabling a mod also enables its dependencies. Disabling a dependency disables
the mods that require it. Core remains enabled. Unmet requirements appear under
**Details**, and must be resolved before applying. New mods default to enabled;
selections persist across launches without moving or deleting mod folders.
Mod-owned saved progress is retained while a mod is disabled.

If a mod does not appear or cannot load, follow [Troubleshooting](../../guides/troubleshooting/).
