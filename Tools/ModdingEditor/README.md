# Eclipse Modding for VS Code

`sf2.battles.set_locked(battle, locked)` requires `story.progression` and an owned
battle handle. It changes a revealed entry on an initialized, unblocked map and
returns whether the request was accepted. The boolean argument is strict; see the
content graph reference for scene/profile guards and persistence limits.
`sf2.battles.reveal(battle, locked)` creates the entry once; repeated calls preserve
its saved progress. `sf2.battles.focus(battle)` selects an already visible entry.
Both use the same capability, ownership and map guards.

`sf2.profile.fight(fight)` reads a detached `{ present, wins, losses }` snapshot.
It accepts a fight handle or qualified ID and requires `profile.read` plus any
referenced namespace dependency. LuaLS completes the snapshot fields; diagnostics
check the capability. Unknown fights and unavailable profiles raise runtime errors.

Reward definitions include `experience` (integer 0–1,000,000, default 0) and
optional `prize_base` (finite 0–1,000,000). The latter feeds native performance
coin calculations and is not a fixed coin award. Nested field completion and
generated definitions reflect the same contract as the public reward guide.

Warrior loadouts accept a mix of perk handles and `WarriorPerk` settings rows.
Rows expose `perk`, optional `aspect` (0–2,147,483,647), and optional
`chance_factor` (0–10,000). Both numbers must be finite; overrides are supported
only for core perks and apply to that opponent. LuaLS provides nested field
completion; the game checks ranges, duplicate perks and ownership at registration.

`sf2.rules.no_button` supports Punch, Kick, Ranged, Magic and RaidCharge. Runtime
rules hide and suppress player-one controls across touch, keyboard and gamepad;
they do not restrict AI move selection. Use `target = sf2.rules.PLAYER`.

Warrior perk entries also support optional `chance` (finite probability 0–1) and
`frames` (integer 0–2,147,483,647). These override parameters on that warrior's
core perk clone; omitted values inherit and explicit zero remains meaningful.
Owned Lua perks use their behavior definitions instead.

`sf2.profile.set_eclipse_mode(enabled)` requests the native story-map switch with
`story.progression`. Its boolean result says whether the requested mode and map
are ready; native tutorials can defer completion. UI definitions now accept
`on_back(view)` to acknowledge or retain a foreground menu/modal on user Back.
Scene/profile cleanup continues to invoke only `on_close`.

Editor support for all 37 public Eclipse API modules: 182 functions, aliases, and
callbacks; 76 constants; and 242 typed structures. Version 0.1.0 retains the ID
`eclipse-modding.eclipse-modding-preview` so it upgrades the original prototype.

Move authoring now completes `damage_terms = { { type, shift } }`, the native
`Spinning`/`HighHeavy` reactions, and `round_stage`, `screen`, and `mod_exists`
conditions. Ordered repeated key entries support native double-tap sequences.
`damage_type` and `damage_terms` are mutually exclusive at runtime; LuaLS table
completion does not replace runtime validation or a combat playtest.

Move definitions also complete `locks`, `transitions`, `align`, and `direction`,
including nested points and axes. Templates support these fields except
`transitions`, which the native parser does not inherit. The scoped
`sf2.moves.extend_item_lock` patch adds an alternative item subtype while
preserving the move's other requirements; it requires `content.patch`.

Direct move registrations also complete scheduled `actions` (core random sounds
and shop completion), `profile`, `tactic_distance`, `tactic_conditions`, `no_wall_repulsion`, and
`no_interpolation_frames`. Action frame/event exclusivity, native name availability
and live animation behavior still require runtime validation and playtesting.
`profile.display_name` completes as an optional localization handle for a readable
move heading, independently of the move's namespaced runtime identity.

## Install

Adds `sf2.items.set_tactic_subtype { item, group }` for core or owned weapons; an empty group selects subtype fallback. Requires `content.patch`.

Adds optional `tactic_subtype` to weapon registration for an independent native AI table group; omission preserves the animation subtype fallback.

Adds `sf2.items.set_innate_perks { item, entries }`; each entry has a
perk handle and optional named numeric parameters. Empty entries remove innate effects.
Adds `sf2.items.set_default_enchantments { item, entries }`; each entry
has a perk handle and optional integer aspect. An empty entries array removes defaults.
Adds `sf2.items.set_presentation { item, icon?, model? }` to patch an existing
equipment item's typed sprite or model; at least one asset is required. The
`sf2.shop.set_price` table also accepts `secondary_price` in the other currency
when an item has both coin and gem purchase prices.
Adds `sf2.forge.override_deviation { profile, equipment, minimum, maximum }`
for an existing random-aspect recipe category. Its typed table requires integer bounds.
Adds `sf2.forge.exclude_candidate { profile, perk, equipment }` with
typed core profile/perk handles and a `content.patch` capability diagnostic.
Adds optional `animation` observations to both sides of AI decisions
and `fighter:snapshot()`: current name/type, facing and active named intervals.
AI action completion respects authored native `tactic_conditions` and
`tactic_distance` gates. Move conditions also support
`{ type = "round_result", name = "Victory" }` or `"Defeat"` for end-of-round animations.
Scheduled move effects accept `on_background = true` for the native background
render layer; omission keeps the existing foreground behavior. They also accept
`attach = { player, root_point, attach_point, offset_x?, offset_y?,
start_rotation? }` to follow a pair of live model nodes. Use `attach` in place
of `position`; its offset uses the node-local frame with positive Y down.
Scheduled `stop_sound` actions accept `core_sound` and call the native sound
stop path at a frame or event, including hit and animation end.
Scheduled `play_animation` actions target a named actor at a frame or event.
Use a registered `move` handle or a native `core_animation` name, exactly one;
`player` is required and `child_name` can select a specific spawned child.
Adds AI candidate `timing` (sample bounds, spacing, nominal duration,
loop flag) and `inputs` (native controls and press types). Nested fields complete
inside `on_decide`; nominal duration is not a prediction of completion or hits.
Adds `type` (`none`, `move`, `attack`) and integer `priority` to AI
action candidates, with completions in `on_decide` callbacks.
AI decisions also expose optional `back_wall_distance` in arena units. Native
fights supply it on every decision; older host-only adapters may omit it.
The programmable AI starter uses it to avoid stepping into the back wall.
Adds `kind = "grid"` with required `columns`, `cell_width` and
`cell_height`, using the existing game-styled child widgets and `gap` spacing.
Adds `sf2.ui.set_sprite(view, widget_id, sprite)` for changing a live image
without rebuilding its panel. Completion requires a typed sprite handle.
Adds `kind = "image"` UI nodes with a typed `sprite` handle and explicit
positive width/height. The generated `UiNode` contract includes completion for
`sprite`; artwork preserves aspect ratio and uses the shared UI container styling.

1. Install **Lua** by **sumneko** in VS Code. LuaLS **3.18.2** is the tested version.
2. Build the package below, then run **Extensions: Install from VSIX...** and
   select `dist/eclipse-modding-0.1.0.vsix`. Reload when prompted.
3. Open the folder containing `mod.toml` and run **Eclipse Modding: Enable in This Folder**.
4. Write `local sf2 = require("sf2")` in Lua, then type `sf2.` and press Ctrl+Space.

The package is not on the Marketplace. Relevant GitHub Actions runs also produce a
VSIX artifact. Project indexing runs locally and never executes your Lua scripts.

## Features

Includes game-styled `toggle` and `slider` nodes, typed `on_change`
callbacks and `sf2.ui.set_checked`. Slider values are normalized to 0–1;
map them to your own units in Lua. Setters update presentation without calling
input callbacks. See the Custom UI reference for lifetime and input rules.

- API completion, typed argument tables, distinct handles, signatures, and hovers
  with requirements, timing, return values, and wiki links.
- Inferred inline callback arguments: fighter methods, event fields, stateful
  `self.params` / `self.state`, and declared parameter/state key completion.
- Local sprite/model/audio/binary and localization string completion, including
  aliases such as `local assets = sf2.assets`.
  Local model references accept `assets/models/*.xml` or gzip-compressed
  `assets/models/*.modelz`; the extension is omitted from Lua IDs.
- F12 on local lookup strings opens their definition. Localization hovers show translations.
- Warnings for missing local references, asset kind mismatches, undeclared
  dependencies/capabilities, invalid literal prices, and incorrect damage-scaling timing.
- Manifest, entrypoint, sprite texture, unsupported audio extension, and localization checks.
- Capability lightbulb fixes that edit `mod.toml` while retaining its comment.
- **Create Mod** builds a complete Training Blade starter with manifest, script,
  localization, texture, sprite descriptor, and editor settings. It never overwrites
  an existing folder.
- **Validate Open Mod** refreshes diagnostics and opens Problems; **Open Documentation** opens the wiki.
- Move short-form tables are typed: one-key conditions such as `{ not_mod = "Stun" }`
  and `{ controllable = true }`, points such as `{ node = "NPivot", player = "Enemy" }`,
  `timeline` keyed by frame or event, `events = "controlled"`,
  `direction = "face_enemy"`, `from`/`to` interval bounds and `damage_terms` maps.
  The long-form tables still type-check while packaged content is migrated.
  LuaLS checks field names and value types; the game enforces the one-kind-per-table
  rule and limits when the mod loads.

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

Timer declarations include optional `complete_pending = true` for instant forge
policies (`seconds = 0`). Completions and generated `TimerPolicy` definitions
include the field. The runtime rejects it for nonzero durations; it never rewrites
saved deadlines and uses the normal forge delivery settlement path.

For mod-owned equipment, shop `required_group` also supplies the native pack
label used by quest unlock notifications. Core item labels are unchanged.

Shop availability also accepts `minimum_level` (integer 0–52, default 0).
Completions include this eligibility gate alongside `visibility` and
`required_group`. It does not change equipment stats, prices or upgrade rules.

`sf2.items.set_subtype { item, subtype }` has typed item-handle completion and
requires `content.patch`. The target family must have compatible native move
and projectile support; editor type checking cannot prove animation compatibility.

`sf2.items.set_initial_profile { item, level, upgrade_level, initial_stats, upgrade_template?, legacy_paid_item?, clear_local_upgrades? }`
completes the six allowed stat names, native upgrade templates and legacy paid
marker values for reversible
starting-profile patches. The runtime validates stat fields and the template
against the equipment category. Selecting a different template can change future
upgrade prices; the item's purchase price and saved identity remain unchanged.
The optional `legacy_paid_item` changes only the native `PaidItem` metadata, not
the shop price or currency.
`clear_local_upgrades` is an optional Boolean that removes the item's own
upgrade rows while retaining the selected shared template. For an existing
equipment price, `sf2.shop.set_price { item, price }` completes the item and
opaque price handles; the runtime accepts a positive coin or gem amount and
restores the previous native fields on unload.

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
`Mods/example.battle-rules` and the wiki's programmable-rules guide. IntelliSense reflects the implemented API; it does not replace a game playtest.

`templates/battle-rules/` is a complete manual starter (copy it into your Mods
folder, then change its manifest ID). The Create Mod wizard still defaults to
the weapon starter. Its Lua resolves localization through `sf2.mod.id`.

Combat callbacks now complete `fighter:snapshot()` and its typed health, position, and clock result. The battle-rules template includes a health-dependent guard transition.

Fight-patch completion includes `rules`, `append_rules`, `location`, and `music`.
The `music` field accepts an `AudioHandle` from `sf2.assets.audio` or a native
track name. The `core-fight` template demonstrates editing an existing
encounter; copy it manually as described for the battle-rules template.

Adds perk `upgrades` completion with level, description and parameter fields. Runtime/native upgrade acceptance is tracked in `Mods/PRE_DE_WORK_LOG.md`.

The manual `perk-upgrades` template demonstrates a learned guard with three upgrades; its matching mod and automated checks are under `Mods/example.perk-upgrades` and `Tools/TestPerkUpgrades.ps1`.

Infers `OutgoingFighter` in `on_damage_dealing`, with `scale_outgoing_damage` requiring `combat.modify_outgoing_hit`. The manual `outgoing-rule` template demonstrates a per-round third-hit modifier.

Completes native combo/style event fields in callbacks. The manual `combo-reserve` template combines those events with a timed outgoing bonus and ordinary Lua control flow.

Adds `sf2.ui.open`, owned UI handles, targeted widget setters, close and
open-state queries. Recursive layout definitions and click callback arguments
are typed. The manual `charge-ui` template links a live HUD to a fresh combat
callback; HUD buttons currently require pointer input. Full-game UI verification
is separate from editor diagnostics.

Adds typed `on_tick` event fields (`frame`, `seconds`, `delta_frames`,
`delta_seconds`). The combo-reserve template now expires its state on active
simulation ticks. Editor completion does not replace a Unity pause/round playtest.

Adds typed `placement` completion for UI anchors and offsets. The Charged Strike starter places its HUD near the top-right safe-area corner.

Adds `sf2.localization.text(key, language?)` for translated strings. Charged Strike includes English and Polish translation files and refreshes localized labels using Lua.

Lua-only mods can use `sf2.localization.register { id, language, value }`. The
project indexer treats registrations in the entrypoint and its reachable literal
local `require("module.name")` chain like localization files, so
`sf2.localization.key` completion/validation resolves them without a TOML file.
The indexer follows the runtime's safe dotted module-name form and visits each
resolved module once. Dynamic requires and missing/unresolvable modules are not
guessed, and unrelated scripts that are never required are not indexed.

Adds typed UI style fields. Defaults reuse the game font, parchment, beveled buttons and combat bar textures; styles provide limited explicit overrides. A menu or modal root stack can set `style = { frame = "scroll" }` for the recovered rolled-paper frame and a visible scrollbar on its vertical scroll widget.

Adds mode/event/raid `on_result` completion and result types. The `templates/branching-trial` starter demonstrates saved alternating routes with original game assets.

Adds `sf2.random.integer(field, minimum, maximum)` and
`sf2.random.number(field)`, backed by declared integer save fields. Diagnostics
report each missing capability separately: draws require both `state.read` and
`state.write`. The manual `templates/seeded-trial` starter uses a saved stream to
select a route; it retains the original game assets. LuaLS checks signatures and
the example, while runtime fixtures verify save/reload and stream behavior.


Adds typed `on_close(view, reason)` notification to UI definitions.
The public reference and generated callback inventory cover both `on_click`
and `on_close`. Charged Strike demonstrates canceling pending gameplay state
when its native view closes, retaining the existing game skin. Shutdown does
not execute close callbacks; editor completion does not prove lifecycle timing.

Adds typed encounter preparation (`on_prepare`, `sf2.modes.resolve`,
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
using the public API; no new Lua binding or editor schema is introduced. Blender scene
preparation, node/pose validation and native export tests are separate from editor
completion and diagnostics. Keep source scenes outside the distributed mod.


Adds `sf2.quests.suppress { target = "..." }` with `content.patch`.
Completion uses `QuestSuppression`; core IDs include the original XML source file
and quest name. Unknown runtime targets and cross-mod conflicts are checked by
registration, not inferred from editor diagnostics. See the public quests reference.

Adds LocationCurve and LocationCurvePoint completion for image motion_x,
motion_y, rotation and opacity. Points specify period/value and optional ease;
curves specify points and optional offset. The public location reference documents
units and bounds. Mods/example.animated-arena is a complete registration example.

Includes optional LocationDefinition.music_choices (AudioHandle[]).
Use it instead of music for native random track selection at fight entry. The
location guide documents the 16-track limit and mutually exclusive settings.

Adds LocationDefinition.dojo and locations.select_dojo/selected_dojo/
reset_dojo, with presentation.dojo capability diagnostics and completion. The
Dojo Selector example is validated as a complete mod; callbacks run after profile
loading, and changes apply on next dojo entry. See the public location guide.
`select_dojo` also accepts an installed `core:locations/name` string. Quest
completion includes `show_map_button` placement fields, and `story.on` accepts
`map_button` with its `button` name in the event payload. These declarations
describe the Lua contract. `show_map_button.image` accepts a sprite handle for
mod artwork or an installed native `Textures/` path.
Running `show_map_button` again with the same ID updates a saved button's
presentation, so placement changes also apply to existing profiles.

Adds profile.level(), profile.item(ItemHandle) and ProfileItemSnapshot
completion, plus profile.read capability diagnostics. See Player profile queries
in the wiki for timing and the distinction between item presence and ownership.

Adds story.on/off/is_active, typed StoryEvent callback payloads and opaque
StorySubscription handles, with story.events capability diagnostics. See Story
events in the wiki for native timing, cancellation and delivery limits.

Adds the level_up story event and optional integer previous_level/level
payload fields. The observer example includes experience-driven level changes.

Adds scene_enter with a typed scene field. It observes initialized
destinations after a deferred frame; it does not grant combat authority or bypass
native dialogs. Existing UI close callbacks handle scene teardown.

Adds scenes.open with destination completion and presentation.navigate
capability diagnostics. Scene Menu demonstrates native navigation from game-styled
UI; native quests can consume a request, and loading completion is asynchronous.

Adds FightPatch.warriors: an optional array of 1–100 unique warrior handles for replacing an existing encounter's opponents. See the fight patch reference for preservation and conflict semantics.

Adds FightPatch.reward_drops and typed RewardDropPatch entries for scoped item rewards. Currency and other native reward scopes are preserved; mixed economic choices reject replacement. See the fight patch reference for additive mode and level semantics.

Reward grant configuration now completes enchantment chance, duration and
bounded native numeric `parameters` alongside perk and aspect. These settings
apply to the granted equipment copy, leaving the core item definition intact.

Adds profile.perk and a detached learned/upgrade snapshot under profile.read. This queries learned progression, not active combat effects.

Adds optional type/subtype fields to profile.item snapshots. Native catalog classification is available independently of ownership; nil indicates missing runtime metadata.

Adds item_acquired story notifications with previous_count/count. It observes positive increases through the native grant routine, not all inventory changes.

Extends item_acquired to native delivery completion that raises an empty record to count one; upgrade-only deliveries do not emit acquisition.

Profile item/perk queries support qualified ID strings, including IDs received by story callbacks. Completion retains the typed snapshot fields; declare dependencies for queried foreign namespaces.

Adds typed equipment-array completion for `sf2.profile.equipment()`.

Adds battle_result completion and typed outcome/equipment payload fields.

Adds `fighter:change_form(character)` and `Eclipse.FormRequest`
(`queued`, `applied`, `failed`, optional `error`). Requires `combat.transform`
and a warrior handle registered by the requesting mod. Native form acceptance
and remaining effect cases are still under verification; see the combat callback
reference before relying on this experimental workflow.
The complete Shifting Guardian starter is in `templates/shifting-guardian/`; copy it to your Mods directory to inspect the result-driven transformation HUD.

Form handover preserves perk cooldown flags and numeric/text variables, including
their current values and remaining modifier timers. The request signature and
result fields are unchanged. `python3 Tools/TestCharacterForms.py` runs the
existing production-method form fixtures on Linux using the installed Unity
compiler and .NET 10 runtime; it does not require PowerShell or start the game.

Equipment registration completes category-specific `initial_stats` fields.
Omitting the table keeps normal level-derived power; an empty table leaves all
initial attributes absent. A populated table replaces the complete initial
snapshot, with integer values 0-1000000 (zero is distinct from absence). Normal
upgrades and level-scaled acquisition still apply. The weapon starter explains
these choices without changing its normal stats. See the equipment API reference.

`sf2.moves.patch` completes extra conditions and guarded native interval-start/end,
full-interval hit-reaction, direct-sound-frame, single Tap input, selection
priority, binary animation-clip edits and guarded interval removal. The `animation` selector takes an exact
native `.bytes` filename plus a binary handle for a mod-shipped clip. One declaration owns each
native move; expected values must match at native application and the whole patch
batch validates before edits. Frame values are integers 0-100000. The shared
`MiddleShortPlus` native reaction is also available when authoring new attacks.
`remove_interval` requires the exact native name, type, start and end; it removes
one matching interval and restores it on unload.
See the moves reference for ambiguous-target rejection and teardown behavior.

`sf2.moves.replace` authors a complete typed definition for an existing native
move while preserving its name for perk and transition links. It requires
`content.patch`, an exact `target` and `expected_file` guard, plus a mod-shipped
binary animation. LuaLS offers the same move fields as `moves.register` except
mod-owned `templates`; use `core_templates`. Alignment now offers
`shift_model_node`, and move conditions can test a fighter's `direction` from
two points. The editor checks declaration shape; only a native fight confirms
the target filename, rig alignment and contact.

Move patches also accept `disable = true` to make an existing move unselectable
while a complete replacement is registered. Move attacks accept the `WaspFly` and `Earthquake`
reaction and `options.ignores_all_invulnerable = true`. The latter is exclusive
with the named `ignores_invulnerable` list. Core perk handles in move locks are
resolved to their native perk names when installed.

`tactic_conditions` describes native AI eligibility with the same typed condition
tables as move selection; it is exclusive with `tactic_distance`. A scheduled
projectile accepts exactly one of `copy_parent_type` and an `item` equipment
handle for its child Weapon slot. The named-item projectile snippet demonstrates
the hidden core-item path.

Scheduled move actions also support `effect`, `stop_effect`, and
`stop_follow_effect`. Effect tables describe an existing core sequence, model-local
name, scale/time scale, loop flag and optional following position. Completion is
not an asset-existence or rendered-effect check; see the move API reference.

Scheduled `create_projectile`, `add_bullets`, and `delete_actor` actions have typed
nested tables. Projectile completion includes copied parent equipment and optional
owned start-move handles. The `eclipse-move-projectile` snippet supplies cast actions;
it does not provide the required projectile animation/selection/cleanup graph.

Spell move declarations support typed `actor_name` and `bullets` conditions,
`velocity`/acceleration components and `no_magic_recharge`. LuaLS tests check nested
motion and charge-condition completion. Native simulation units and inclusive
charge bounds are documented in the move API reference.

Distance conditions and `attack.options` have nested completion for signed bounds,
point endpoints, native defense attributes and block/invulnerability exceptions.
The sphere attack snippet shows options only; it is not a complete projectile.

Attack and guarded hit-patch completion also accept `Physycal`, the native
physical-fall reaction spelling. Preserve that spelling; `Physical` is a different
name and is rejected. Existing reaction defaults are unchanged.

`HighLong` is also supported for new attack definitions and guarded hit patches.
It selects the native long high-hit reaction; it does not change interval duration.

Move completion includes explicit `attack.direct`, `NoReaction`, voice-filtered
`sound` actions and `shake_screen` payloads. Direct attacks require no edges;
normal attacks still require 1–64. See the move reference for camera bounds and
native frame timing. These fields expose native behavior, not arbitrary Lua execution.

`attack.hit_move` accepts a registered Move handle (exclusive with `hit`).
`direction.impulse.reverse` completes as a boolean, default false, and cannot be
combined with `from`/`to`. These use native hit selection and facing; they do not
force an animation past its conditions or apply an impulse.

## Animation lifecycle completion

Behavior definitions include `on_animation_start` and `on_animation_end` with
typed `animation_name`, `target` (`self`, `opponent`, `other`) and `frame` fields.
These observe native combat notifications; `other` includes projectiles and does
not imply projectile ownership. See the combat callback wiki for timing and limits.

Fighter completion includes `set_flag(key)`, `has_flag(key)` and `clear_flag(key)`.
These require `combat.effects` and address only the current behavior instance.
`set_flag` returns the qualified native name, and `has_flag` returns a boolean.

`fighter:set_control_blocked(control, blocked)` completes five control names:
`punch`, `kick`, `ranged`, `magic`, and `raid_charge`; `blocked` is a boolean.
It requires `combat.effects` and a player callback during an active round.
Restrictions belong to the attached behavior instance and clear at round setup
or fight end. Releasing one does not override other restrictions or grant an
unavailable action. Reapply persistent conditions in `on_round_begin`.

Image nodes accept `mirrored = true` to reflect a portrait horizontally. The
boolean defaults to false and is fixed for the view's lifetime. It changes
neither layout size nor the sprite asset; `sf2.ui.set_sprite` preserves it.

Timed map story screens use `sf2.ui.act_screen { lines = {{text = localized, frames = 180}}, on_complete = function() end }` (`ui.create`). IntelliSense provides typed localization handles, required line durations, and the optional completion callback. Refused/cancelled screens never acknowledge completion; see the Custom UI reference for bounds and lifecycle rules.

Underworld content completes `underworld = true` on zones, `power_mode` and
`icons = { base, active, locked?, locked_active? }` on battles,
`sf2.warriors.register_template` with `skeleton`, perk-row `parameters`, reward
`currencies = { { currency, expected, show? } }`, the `no_health_bar`,
`invert_joystick`, `random_area`, `light_in_the_darkness`, `group` and `random` rules, and
`sf2.underworld.set_toggle_visible` / `set_focus` (`story.progression`). See the
wiki's Underworld pages guide.
Move snippets now insert `timeline` entries with frame or lowercase event keys,
short condition keys, and `node`/`pivot` points. Put scheduled snippets inside
the move's `timeline` table. Quest and owned-audio trigger actions retain their
own `type`-based syntax. The LuaLS suite covers every packaged DE128 move module.
For a warrior or child template, `attribute_alignments` adds its rows once to
the inherited rows. Leave it out to keep the parent's alignment rows.

Owned fight intros use `sf2.story.before_fight(fight, function(request) ... end)` with `story.progression`. Typed request completion supports `resume_fight`, `cancel_fight` and `fight_pending`; return `true` for immediate entry or `nil` for deferred UI. See Story events for native entry, lifetime, and instruction limits.

Move definitions use compact condition keys, points, timelines and damage maps exclusively. Legacy move types are removed from completion. `sf2.story.play_sequence` provides typed dialog/act-screen steps and optional saved state cursors; see the story reference for callback and resume behavior.
