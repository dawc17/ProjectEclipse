# Move short form and Lua simplification — migration complete

Status as of 2026-09-25. All four requested areas are implemented.

## Current checkpoint

- Packaged moves, wiki examples, editor snippets and the example mod use compact
  declarations. Legacy move conditions/events, points, interval bounds, damage
  arrays, tactic-distance fields and scheduled `actions` are rejected.
- Conditions, points, events, intervals and damage maps now read directly into
  typed definitions. Timeline ordering uses a private normalization stage;
  this is not an alternate public declaration format. Quest actions, owned-audio
  trigger actions and guarded `remove_interval` selectors keep their separate contracts.
- Long-form editor types are removed. Positive and malformed-value fixtures use
  compact declarations; explicit removed-syntax rejection tests remain.
- `MoveShortFormProjections.json` captures native XML before parser removal.
  `DE128MoveProjections.json` captures 16 approved package projections after the
  prior archive-equivalence checks. Both regression scripts now use fixed data,
  never execute old Lua, and cannot regenerate their own expected results.
- Generators emit compact rows; 6,960 translations remain in 14 localization
  TOML files, with generated Lua handle indexes. Generator checks remain available.
- `sf2.story.play_sequence` owns dialogue/act-screen advancement, cancellation,
  bounded callbacks, synchronous/refused hosts and stale-callback protection.
  Its optional `position` names a declared integer state field (default 1).
  Profile rebinding releases sequence UI before any old callback can write state.
- Sensei entry, victory and defeat plus Underworld entry/intro/followup/results
  use declarative steps. Victory reuses existing saved cursors. Underworld uses
  two defaulted fields for its stable active-story key and cursor; interrupted
  map stories resume at the next unacknowledged card. Fight-entry sequences are
  intentionally transient and restart when that held request is abandoned.
- `sensei_guard_opponents.lua` and `underworld.lua` register content rather than
  advance dialogue. They have no dialogue state loop to migrate. Story selection,
  random defeat choices, progression flags and fight resumption remain ordinary Lua.

Verification: 89 compact move checks, all 16 approved package projections,
1,159 combined native move authoring checks, 14,934 foundation checks (including
1,225 Underworld story checks), 46 sequence checks, 65 story-event transport checks, 28 dialog binding checks,
1,329 Sensei entry and 533 combined Sensei story checks,
editor generation/check and 41 unit tests, and the wiki build/link audit
(188 bindings, 4,360 links across 48 pages). All four managed projects compile.
Underworld runtime passed 1,282 assertions; the static audit retains the existing
`fungus_raid: missing /layer_0_2` finding. VS Code integration could not launch
because its updater holds the `vscode-updating` mutex. LuaLS integration passes, including all migrated story files and compact-only
completion probes.
No Unity editor validation or in-game playtest is claimed.

## Decisions (owner, 2026-09-25)

- The goal is a modding API that is easy to read and write by hand. Much of the
  DE128 Lua (moves, the Underworld and Sensei data, story flows) is hard to
  maintain in its current form.
- **Short form only.** No external mods exist yet, so the long form
  (`type = ...`, `["not"] = true`, `object = ...`, `start`/`["end"]`, `actions`)
  has been removed. Only compact move declarations are accepted.
- **Short forms live in the C# binding**, not in a Lua helper library. They are
  plain declarative tables (no builder functions and no Lua DSL), which keeps with
  the AGENTS.md rule that static content stays typed and declarative.

## What exists (phase 1, done)

| Piece | Where |
| --- | --- |
| Short-form expansion (conditions, points, timeline, events, direction, intervals, damage-term map, tactic distance, animation path) | `Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeShortForm.cs` |
| Hooks into the move readers | `MoonSharpScriptRuntimeP1D.cs`: `ReadMoveConditions`, `ReadMovePoint`, `ReadMoveEvents`, `ReadMoveIntervals`, `ReadMoveAttack` (damage terms), `ReadMoveGraph` (direction), `ReadMovePresentation` (timeline, tactic distance), `MoveAnimationAsset` |
| Public docs | `Docs/Modding/src/content/docs/api/moves-and-tactics.md`, section "Writing moves: the short form" |
| Editor types | `Tools/ModdingEditor/scripts/api-schema.cjs` (`MoveShortCondition`, `MoveShortPoint`, `MoveShortAction`, `MoveTimeline`, `MoveShortEvent`, `MoveShortTacticDistance`, `MoveDamageTermMap`), regenerated `sf2.d.lua`/`api.json` |
| Migrated DE128 files | `Mods/de128/scripts/content/war_whirl.lua`, `sphere1.lua` |

The original expansion layer has been folded into direct typed readers except
for private timeline scheduling normalization. Native move XML remains identical
under the two ordering rules below.

### Ordering rules (evidence)

- **Conditions** in a list are an AND (or an `any` OR). Native
  `ConditionList.OIBMEHKCPKB` evaluates them with short-circuit, and the
  conditions moves use are pure, so order never changes which moves are
  selected. The `{ controllable = true }` preset therefore expands as one block
  wherever it is written. Key sequences inside `keys` keep their order, because
  that order is the input sequence.
- **Actions** are dispatched per trigger. `ModelAnimation` collects only the
  actions matching the current frame, or only those matching one event, and runs
  them in list order. The timeline emits frames ascending, then events in a fixed
  order. Entries under one key keep their written order, which is the only order
  the runtime observes.

The three archive comparisons (`TestDE128Sphere1/2`, `TestDE128ComboSphere3`)
follow these two rules and still compare everything else strictly.

## Verification tools

| Check | Use |
| --- | --- |
| `Tools/TestDE128ShortFormEquivalence.ps1 -Files a.lua,b.lua` | Proof for a migrated file. Compares fixed approved native projections with the working-tree file through the real binding, including the projected native moves XML, move patches and tactics. Mutation-tested: catches changed frames, dropped conditions and reordered same-trigger actions. |
| `Tools/TestMoveShortForm.ps1` | Every compact kind matches fixed native XML; legacy syntax and malformed tables are rejected. |
| `Tools/ModdingEditor/test/lsp.cjs` | Opens the migrated DE128 files in LuaLS and requires zero diagnostics. Add each newly migrated file to the list in the "short-form DE128 moves" block. Run with `node test/lsp.cjs <path to lua-language-server.exe 3.18.2>`; the VS Code Lua extension ships one under `~/.vscode/extensions/sumneko.lua-3.18.2-win32-x64/server/bin/`. |
| `Tools/TestDE128Foundation.ps1` and the relevant `Tools/TestDE128*.ps1` | Full-package regression; needs PowerShell 7. |

## Next steps

### Phase 2 — migrate every DE128 move file (complete)

Completed using equivalence, LuaLS, foundation and the relevant archive suites.
The following is the original inventory; counts are pre-migration matches,
including the non-move false positives in `ascension.lua`.

`chinese_swords_data.lua` (43), `mind_throw.lua` (32), `sphere2.lua` (30),
`hermit_storm.lua` (30), `gatekeeper_power_field.lua` (23), `combo_sphere3.lua`
(23), `butcher_earthquake.lua` (23), `blackness_grasp.lua` (23),
`widow_teleportation.lua` (22), `sphere3.lua` (21), `wasp_fly.lua` (20),
`ascension.lua` (4), `raid_boss_abilities.lua` (3), `chinese_swords.lua` (2),
`shared_moves.lua` (1), `dandy_lightning_chain.lua` (1).

Watch for:
- Files with local `point()`, `current()`, `sound()` and `effect()` helpers:
  delete the helpers once the short form makes them pointless.
- A repeated hit-sound list (`snd_hit1`…`snd_hit6`) appears in many files; a
  shared `content/sounds.lua` module would remove the duplication.
- `sf2.moves.patch { conditions = ... }` also accepts the short form.
- `chinese_swords_data.lua` is loaded by `Tools/TestMoveAttackAuthoring.ps1` and
  compared to the archive; that test must keep passing.

Also convert the long-form Lua in `Mods/example.phase1/scripts/main.lua`,
`Tools/ModdingEditor/templates/programmable-ai/README.md`, the wiki examples in
`api/moves-and-tactics.md`, `api/quests.md` and `guides/character-authoring.md`,
and the snippets in `Tools/ModdingEditor/snippets/lua.json`.

### Phase 3 — remove the long form (complete)

Only after nothing packaged uses it:
1. In `MoonSharpScriptRuntimeShortForm.cs`, make every passthrough an error:
   entries with `type` in conditions and events, points with `object`, intervals
   with `start`/`end`, `damage_terms` arrays, `tactic_distance` with `axis`, and
   `actions` in move tables. Fold the expansion into the readers so errors
   name compact fields such as `min` and `max`. This is implemented in the direct
   typed readers; timeline scheduling retains private ordering normalization.
2. Rewrite the reference sections of `api/moves-and-tactics.md` to use short-form
   names only, and drop the long-form types from `api-schema.cjs`.
3. Convert the test fixtures that author long-form Lua:
   `TestMoveAttackAuthoring.ps1`, `TestMovePresentationAuthoring.ps1`,
   `TestMoveSpellAttacks.ps1`, `TestMoveSpellSelection.ps1`,
   `TestMindThrowPrimitives.ps1`, the long-form halves of `TestMoveShortForm.ps1`
   (keep them as expected-XML fixtures), and C# fixtures under `Tools/*.cs` that
   build moves.
4. Save fingerprints do not depend on syntax, so removal does not change them.

### Phase 4 — generated data files (complete)

The following was the original scope. Current generated rows, localization
sections, preserved translations and checks are described in the checkpoint above.
Sensei dialogue data was already authored as compact rows; no Sensei data
generator exists in this repository.

- `underworld_data.lua` (7.1k lines), `underworld_story_data.lua` (1.5k) and the
  Sensei data come from `Tools/GenerateDE128Underworld.py` and related
  generators. They are hard to read because the generator writes one key per line
  and repeats defaults. Make the generators write compact rows (one warrior, perk
  or dialogue line per line), omit values equal to documented defaults, and
  express Power Mode fights as a difference from their normal twin where the
  archive allows. Keep the `-- GENERATED` header and archive checks.
- `underworld_text_values.lua` (5.3k lines), `sensei_*_text.lua` and
  `titan_reward_text_values.lua` hold translations in Lua. Mods already support
  `localizations/` files (see `Mods/example.phase1` and
  `api/localization-patches.md`). Move the strings there. Check that the
  localization API covers every use first (dialogue titles, `{br}` markup, all
  languages).

### Phase 5 — story and Sensei flows (complete)

Implemented as `sf2.story.play_sequence { steps, position?, on_complete?,
on_cancel?, on_step? }`. See the current checkpoint and the public story reference.
This API presents typed dialogue/act-screen payloads and persists acknowledgements;
procedural choices remain Lua handlers. It does not introduce an operation DSL.

## Not verified

No Unity editor validation or in-game playtest was run for phase 1. Equivalence
is proven at the native-XML projection the game parses.
