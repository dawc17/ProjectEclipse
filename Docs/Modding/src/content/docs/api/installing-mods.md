---
title: "Installing and enabling mods"
description: "Installing and enabling mods in the Eclipse modding API."
---

## Install a ZIP

On Windows or Android, open **Mods** from the title screen and choose **Install
ZIP**. Select a `.zip` file. Eclipse shows the mod's name, version, and ID before
you confirm. If the ID is already installed, **Replace mod** replaces that mod's
files; saved mod progress is kept. After installation, choose **Apply & Restart**
and enter Campaign. This also works for a ZIP containing one enclosing folder
around `mod.toml`. Each ZIP must contain one mod, including its declared
`scripts/*.lua` entrypoint. The installer rejects unsafe paths and symbolic links,
and limits packages to 10,000 entries, 256 MiB per file, and 512 MiB unpacked.

On Android, the system file picker can select a ZIP from Downloads or another
document provider. You do not need to browse into `Android/data` or grant Eclipse
all-files access. Eclipse copies the chosen archive into its cache, validates it,
and installs its contents into its app-specific Mods directory. The ZIP is not
kept after installation. ZIP installation is currently available in the Unity
editor, Windows player, and Android player.

## Install a folder manually

You can still place a complete mod folder in your installation's `Mods`
directory, with `mod.toml` directly inside that folder. The folder name must
exactly match the manifest ID. A standalone Windows installation uses `Mods`
beside the executable; the Unity editor uses the project root `Mods` folder.
A launcher-managed installation may configure a shared Mods directory outside
its version folders. Android uses `Application.persistentDataPath/Mods`, normally
`Android/data/<package-id>/files/Mods`; the ZIP installer avoids the need to
access that location from a file manager. Restart Eclipse after manually adding
or updating mod files.

Toggle the installed mods, then choose **Apply & Restart**. The game saves and reloads to the title screen;
enter Campaign to load the new selection. **Back / Cancel** discards unapplied
changes. In game, open the main **Menu** and choose **Return to Title** to reach
the mod list again.

Enabling a mod also enables its dependencies. Disabling a dependency disables
the mods that require it. Core remains enabled. Unmet requirements appear under
**Details**, and must be resolved before applying. New mods default to enabled;
selections persist across launches without moving or deleting mod folders.
Mod-owned saved progress is retained while a mod is disabled.

If a mod does not appear or cannot load, follow [Troubleshooting](../../guides/troubleshooting/).
