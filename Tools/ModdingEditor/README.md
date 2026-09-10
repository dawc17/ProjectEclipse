# Eclipse Modding for VS Code

Editor support for all 31 public Eclipse API modules: 96 functions, aliases, and
callbacks; 76 constants; and 134 typed structures. Version 0.1.0 retains the ID
`eclipse-modding.eclipse-modding-preview` so it upgrades the original prototype.

## Install

1. Install **Lua** by **sumneko** in VS Code. LuaLS **3.18.2** is the tested version.
2. Build the package below, then run **Extensions: Install from VSIX...** and
   select `dist/eclipse-modding-0.1.0.vsix`. Reload when prompted.
3. Open the folder containing `mod.toml` and run **Eclipse Modding: Enable in This Folder**.
4. Write `local sf2 = require("sf2")` in Lua, then type `sf2.` and press Ctrl+Space.

The package is not on the Marketplace. Relevant GitHub Actions runs also produce a
VSIX artifact. Project indexing runs locally and never executes your Lua scripts.

## Features

- API completion, typed argument tables, distinct handles, signatures, and hovers
  with requirements, timing, return values, and wiki links.
- Inferred inline callback arguments: fighter methods, event fields, stateful
  `self.params` / `self.state`, and declared parameter/state key completion.
- Local sprite/model/audio/binary and localization string completion, including
  aliases such as `local assets = sf2.assets`.
- F12 on local lookup strings opens their definition. Localization hovers show translations.
- Warnings for missing local references, asset kind mismatches, undeclared
  dependencies/capabilities, invalid literal prices, and incorrect damage-scaling timing.
- Manifest, entrypoint, sprite texture, unsupported audio extension, and localization checks.
- Capability lightbulb fixes that edit `mod.toml` while retaining its comment.
- **Create Mod** builds a complete Training Blade starter with manifest, script,
  localization, texture, sprite descriptor, and editor settings. It never overwrites
  an existing folder.
- **Validate Open Mod** refreshes diagnostics and opens Problems; **Open Documentation** opens the wiki.

Type `eclipse-` for import, weapon, sprite, localization, behavior, stateful behavior,
damage modifier, saved state, and perk snippets. Snippets are building blocks:
replace placeholders and supply referenced files/handles and capabilities.

## Configuration

Enable/Disable use the active editor's workspace folder, or prompt if several are
open. Other Lua libraries are preserved. Existing `.luarc.json` or `.luarc.jsonc`
files are updated too, preserving comments and unrelated settings. Fix invalid JSON
first. Previously enabled folders migrate their registered path after an extension
update; rerun Enable if needed. Select **Lua 5.2** as the Lua runtime; Create Mod
sets this automatically.

Project assistance finds the nearest `mod.toml` within the workspace folder and
reads unsaved text edits. Disable removes this extension's registered library and
turns off project assistance. `eclipseModding.enabled` controls the latter independently.
Never copy `library/sf2.d.lua` into mod scripts: it is editor metadata.

## Limits

LuaLS checks types and required fields. Eclipse diagnostics analyze literal
references and recognizable API calls. Dynamic names, arbitrary helper functions,
metatables, cross-file data flow, and dynamic schema values are not fully checked.
Syntax errors can suspend project analysis until corrected.

Asset completion/navigation is local to the current mod. External references are
checked for a dependency declaration, not existence. Files are indexed without
decoding images, models, or audio: WAV must still satisfy PCM16 requirements.
Numeric checks currently cover literal prices; other runtime limits remain authoritative.

Validate Open Mod checks open Lua documents and their mod's manifest/assets/localizations,
not every unopened script. Indexes allow up to 10,000 files per asset/localization
directory. Indexing failures appear in **Output > Eclipse Modding**.
There is no debugger, live reload, mod installation, or game launch. A clean Problems
panel is not a gameplay test.

## Build and maintain

From `Tools/ModdingEditor`, with Node 22:

```powershell
npm ci
npm run generate
npm run check
npm test
npm run package
```

Edit `scripts/api-schema.cjs` for contracts. `generate` reads runtime bindings and
wiki sections, verifies complete member/constant coverage, then writes tracked
`library/sf2.d.lua` and `data/api.json`. Do not edit these generated files by hand.
The coverage check catches absent members and stale outputs, but cannot prove C#
argument semantics. Contract changes need source review and tests.

The extension bundles its runtime dependencies into ignored `out/`. Users need no
Node installation. The VSIX contains metadata and starter assets; generated installers
are ignored. CI checks coverage, project tests, and packaging on relevant changes.

For actual LuaLS tests, obtain the official **3.18.2** binary:

```powershell
node test/lsp.cjs 'C:/path/to/lua-language-server.exe'
```

For actual VS Code integration tests, install Lua in an isolated profile:

```powershell
code --extensions-dir .test-runtime/extensions --user-data-dir .test-runtime/vscode-profile --install-extension sumneko.lua@3.18.2
npm run build
node test/run-vscode.cjs 'C:/path/to/Microsoft VS Code/Code.exe'
```

Tests use generated workspaces and preserve the normal VS Code profile. They cover
every API function/alias/constant completion, callback inference, hovers/signatures,
clean starter diagnostics, intentional type errors, settings preservation, project
navigation, and capability fixes with unsaved edits.

Verified with VS Code **1.137.0**, Lua extension/LuaLS **3.18.2**, and Node **22.23.1**.
Standalone LuaLS 3.19.1 failed to start on this Windows setup with
`Duplicate channel task:1` before metadata loading; that combination is not verified.
No Unity validation or game playtest was performed for this editor-only change.

[API wiki](https://dawc17.github.io/ProjectEclipse/).
Implementation uses the [VS Code language feature APIs](https://code.visualstudio.com/api/language-extensions/programmatic-language-features).
