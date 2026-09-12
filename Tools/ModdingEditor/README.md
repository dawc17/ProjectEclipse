# Eclipse Modding for VS Code

Editor support for all 32 public Eclipse API modules: 112 functions, aliases, and
callbacks; 76 constants; and 149 typed structures. Version 0.1.0 retains the ID
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

API 0.22 includes game-styled `toggle` and `slider` nodes, typed `on_change`
callbacks and `sf2.ui.set_checked`. Slider values are normalized to 0–1;
map them to your own units in Lua. Setters update presentation without calling
input callbacks. See the Custom UI reference for lifetime and input rules.

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

Battle rules: completion supports `sf2.rules.behavior` and its typed behavior,
parameters, target, mode, and rounds fields. See the executable starter in
`Mods/example.battle-rules` and the wiki's programmable-rules guide. Requires API
0.8.0. IntelliSense reflects the implemented API; it does not replace a game playtest.

`templates/battle-rules/` is a complete manual starter (copy it into your Mods
folder, then change its manifest ID). The Create Mod wizard still defaults to
the weapon starter. Its Lua resolves localization through `sf2.mod.id`.

Combat callbacks now complete `fighter:snapshot()` and its typed health, position, and clock result (API 0.9). The battle-rules template includes a health-dependent guard transition. Generated metadata reads the API version from the runtime manifest.

API 0.10 fight-patch completion includes `rules`, `append_rules`, `location`, and `music`. The `core-fight` template demonstrates editing an existing encounter; copy it manually as described for the battle-rules template.

API 0.11 adds perk `upgrades` completion with level, description and parameter fields. Runtime/native upgrade acceptance is tracked in `Mods/PRE_DE_WORK_LOG.md`.

The manual `perk-upgrades` template demonstrates a learned guard with three upgrades; its matching mod and automated checks are under `Mods/example.perk-upgrades` and `Tools/TestPerkUpgrades.ps1`.

API 0.12 infers `OutgoingFighter` in `on_damage_dealing`, with `scale_outgoing_damage` requiring `combat.modify_outgoing_hit`. The manual `outgoing-rule` template demonstrates a per-round third-hit modifier.

API 0.13 completes native combo/style event fields in callbacks. The manual `combo-reserve` template combines those events with a timed outgoing bonus and ordinary Lua control flow.

API 0.15 adds `sf2.ui.open`, owned UI handles, targeted widget setters, close and
open-state queries. Recursive layout definitions and click callback arguments
are typed. The manual `charge-ui` template links a live HUD to a fresh combat
callback; HUD buttons currently require pointer input. Full-game UI verification
is separate from editor diagnostics.

API 0.14 adds typed `on_tick` event fields (`frame`, `seconds`, `delta_frames`,
`delta_seconds`). The combo-reserve template now expires its state on active
simulation ticks. Editor completion does not replace a Unity pause/round playtest.

API 0.16 adds typed `placement` completion for UI anchors and offsets. The Charged Strike starter places its HUD near the top-right safe-area corner.

API 0.17 adds `sf2.localization.text(key, language?)` for translated strings. Charged Strike includes English and Polish translation files and refreshes localized labels using Lua.

API 0.18 adds typed UI style fields. Defaults reuse the game font, parchment, beveled buttons and combat bar textures; styles provide limited explicit overrides.

API 0.19 adds mode/event/raid `on_result` completion and result types. The `templates/branching-trial` starter demonstrates saved alternating routes with original game assets.

API 0.20 adds `sf2.random.integer(field, minimum, maximum)` and
`sf2.random.number(field)`, backed by declared integer save fields. Diagnostics
report each missing capability separately: draws require both `state.read` and
`state.write`. The manual `templates/seeded-trial` starter uses a saved stream to
select a route; it retains the original game assets. LuaLS checks signatures and
the example, while runtime fixtures verify save/reload and stream behavior.


API 0.21 adds typed `on_close(view, reason)` notification to UI definitions.
The public reference and generated callback inventory cover both `on_click`
and `on_close`. Charged Strike demonstrates canceling pending gameplay state
when its native view closes, retaining the existing game skin. Shutdown does
not execute close callbacks; editor completion does not prove lifecycle timing.

API 0.22 adds typed encounter preparation (`on_prepare`, `sf2.modes.resolve`,
`cancel`, `is_pending`), generated encounter plans, programmable tactic
`on_decide` callbacks, and native-styled toggles/sliders with `on_change` and
`set_checked`. The manual `generated-expedition` and `programmable-ai` starters
demonstrate complete map entries and gameplay scripts. Copy a starter into a
folder matching its manifest ID; when renaming it, update its ID and localization
namespace references together. The wizard still creates the weapon starter.

Character definitions now complete `body_model` and `skin_models`. Move definitions
complete `character`/`keys` conditions, `key_pressed` events, frame bounds and typed
attack data. The Blender authoring workflow is documented in the wiki's
`guides/character-authoring` page; its exporter creates a `character.generated.lua`
module and native assets for your mod. Keep editor-only `sf2.d.lua` outside the mod's
executable scripts. LuaLS verifies the new example scripts and callback/field
completion; Blender, native animation reading, and game tests remain separate checks.


The primary visual character workflow is now the [Gymnast guide](../../Docs/Modding/src/content/docs/guides/gymnast.md).
The generated package includes ordinary `scripts/character.lua` and `scripts/main.lua`
using API 0.22; no new Lua binding or editor schema is introduced. Blender scene
preparation, node/pose validation and native export tests are separate from editor
completion and diagnostics. Keep source scenes outside the distributed mod.


API 0.23 adds `sf2.quests.suppress { target = "..." }` with `content.patch`.
Completion uses `QuestSuppression`; core IDs include the original XML source file
and quest name. Unknown runtime targets and cross-mod conflicts are checked by
registration, not inferred from editor diagnostics. See the public quests reference.

API 0.24 adds LocationCurve and LocationCurvePoint completion for image motion_x,
motion_y, rotation and opacity. Points specify period/value and optional ease;
curves specify points and optional offset. The public location reference documents
units and bounds. Mods/example.animated-arena is a complete registration example.

API 0.25 includes optional LocationDefinition.music_choices (AudioHandle[]).
Use it instead of music for native random track selection at fight entry. The
location guide documents the 16-track limit and mutually exclusive settings.

API 0.26 adds LocationDefinition.dojo and locations.select_dojo/selected_dojo/
reset_dojo, with presentation.dojo capability diagnostics and completion. The
Dojo Selector example is validated as a complete mod; callbacks run after profile
loading, and changes apply on next dojo entry. See the public location guide.

API 0.27 adds profile.level(), profile.item(ItemHandle) and ProfileItemSnapshot
completion, plus profile.read capability diagnostics. See Player profile queries
in the wiki for timing and the distinction between item presence and ownership.

API 0.28 adds story.on/off/is_active, typed StoryEvent callback payloads and opaque
StorySubscription handles, with story.events capability diagnostics. See Story
events in the wiki for native timing, cancellation and delivery limits.

API 0.29 adds the level_up story event and optional integer previous_level/level
payload fields. The observer example includes experience-driven level changes.

API 0.30 adds scene_enter with a typed scene field. It observes initialized
destinations after a deferred frame; it does not grant combat authority or bypass
native dialogs. Existing UI close callbacks handle scene teardown.
