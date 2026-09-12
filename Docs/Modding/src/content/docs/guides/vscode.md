---
title: VS Code IntelliSense
description: Explore the API, check your mod, and create a complete starter in VS Code.
---

**Eclipse Modding** adds autocomplete and error checking for the entire public Lua
API. Explore functions as you type, look up real assets, and catch common mistakes
before starting the game.

## Install and enable

The extension is installed from a `.vsix` package, not the Marketplace. Get it from
a successful **Modding editor** workflow artifact, or build it using the
[source README](https://github.com/dawc17/ProjectEclipse/tree/main/Tools/ModdingEditor).

1. Install **Lua** by **sumneko** in VS Code. **3.18.2** is the tested version;
   choose it with the gear menu > **Install Another Version** if needed.
2. Press **Ctrl+Shift+P**, run **Extensions: Install from VSIX...**, and select
   `eclipse-modding-0.1.0.vsix`. Reload when prompted.
3. Open the folder containing your `mod.toml`.
4. Run **Eclipse Modding: Enable in This Folder**.
5. Begin your Lua script with:

```lua
local sf2 = require("sf2")
```

Type `sf2.` and press **Ctrl+Space**. Choose a module, such as `items`, and type
another dot to see its functions. Hover a function for requirements and its full
reference link. Inside a call, **Ctrl+Shift+Space** opens parameter hints.

Version 0.1.0 upgrades the original preview using the same extension ID. It covers
combat, state, quests, equipment, progression, raids, and all other public modules.

## Create your first mod

Run **Eclipse Modding: Create Mod**. Enter a unique lowercase mod ID, display name,
and author name, then choose a parent folder. The command creates a new subfolder
and opens it in another VS Code window. Existing folders are never overwritten.

The starter includes a complete Training Blade: manifest, Lua script, localization,
texture, sprite descriptor, and editor settings. Follow [Your first weapon](../first-weapon/)
to understand the files and install it in Eclipse. The name entered in the command
names the mod. To rename the weapon, edit `weapon.training_blade` in `localizations/eng.toml`.

## Find assets and translated text

Inside a lookup string, press Ctrl+Space to choose a compatible file from your mod:

```lua
local icon = sf2.assets.sprite("sprites/weapon")
local name = sf2.localization.key("weapon.training_blade")
```

Press **F12** on the string to open its definition. Hover a localization reference
to read its translations. Suggestions update as you edit and also work with local
aliases such as `local assets = sf2.assets`.

The index contains your own mod's files. References to `core` or another mod are
checked for a dependency declaration, but their external files are not inspected.

## Write behaviors

Type `eclipse-behavior` or `eclipse-stateful` and accept a snippet. Inline callbacks
infer fighter and event types: `fighter:` suggests operations, and `event.` suggests
event fields. Stateful callbacks receive `self.params` and `self.state`, with
completion for declared parameter and state keys.

Attach the returned behavior handle to a perk or enchantment to use it in combat.
Damage multipliers belong in `on_damage_resolving`; other callbacks cannot change
the hit already being resolved. Follow the combat API reference when choosing events.

## Understand warnings

Open **View > Problems**. LuaLS checks names, argument types, required fields, and
handle kinds. Eclipse Modding adds checks for:

- Missing local references, wrong asset kinds, and undeclared dependencies.
- Missing capabilities for recognizable API calls, including fighter operations.
- Invalid literal coin/gem prices and damage scaling in the wrong callback.
- Manifest fields, duplicate IDs, unsafe or missing entrypoints, sprite textures,
  unsupported audio extensions, and malformed localization entries.

For missing capabilities, press **Ctrl+.** and choose **Declare ... in mod.toml**.
This edits the capability list; save your manifest before running the game.

**Eclipse Modding: Validate Open Mod** refreshes checks for open Lua files and their
mod's manifest/assets/localizations, then opens Problems. Open other scripts to check
them too. Syntax errors can pause project checks until the file parses again.

## Snippets and settings

Type `eclipse-` and press Ctrl+Space for weapon, behavior, stateful behavior, damage
modifier, saved state, perk, sprite, localization, and import snippets. These are
building blocks: replace placeholders, add files, and declare capabilities.

Enable preserves other Lua libraries and updates existing `.luarc.json` or
`.luarc.jsonc` while retaining comments and unrelated settings. Select **Lua 5.2**
as your Lua runtime; Create Mod sets it automatically. With multiple workspace
folders, commands use the active editor's folder or ask you to choose.
**Disable in This Folder** removes the registered library and turns off project assistance.

If suggestions disappear after an update, run Enable again. Check **Output > Eclipse
Modding** for indexing errors and **Output > Lua** for language-server errors.

## Keep testing in Eclipse

UI definitions complete `on_close` with a typed view handle and close reason.
The Charged Strike template demonstrates clearing local state after HUD closure.

Saved random stream calls complete under `sf2.random`. The editor reports
`state.read` and `state.write` requirements separately, so add both capabilities.
The repository's manual `seeded-trial` template demonstrates a saved encounter
choice; see [Saved random streams](../../api/random/) for bounds and lifetime rules.

The editor cannot prove every dynamically constructed reference, helper function,
schema value, numeric limit, or dependency compatibility rule. Asset checks do not
decode images, models, or sound: a `.wav` filename does not prove PCM16 audio.
A clean Problems panel does not confirm loading, appearance, or combat behavior.

The extension does not install mods, launch Eclipse, or provide live reload or a
debugger. Never copy its `sf2.d.lua` metadata into mod scripts: it is editor-only.
