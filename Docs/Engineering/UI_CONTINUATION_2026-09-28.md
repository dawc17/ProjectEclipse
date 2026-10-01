> Historical capture, moved from `todo.md` during the October 1, 2026 cleanup.
> Checked items record implementation at that time, not current playtest results.
> Open work is tracked in the [current backlog](../../todo.md).

# UI continuation — 2026-09-28

Follow-up presentation repairs (native Unity 6000.6.0f1 checked):

- Win/lose results: layout-owned elements keep Unity's positions throughout
  entrance animations; original scales are preserved. Reward labels have measured
  widths, currency rows publish their widths, single-line amounts cannot be
  truncated by the old line boxes, OK is centered, and the shadow fills the viewport.
- Forge: dock the recipe drawer to the screen-space left of the current-enchantment
  paper after canvas layout. Its parent is rotated 180 degrees; bounds use all
  four corners. Drawer height follows the paper without inheriting the rotation.
- Shop: `ShopDojoBackdrop` loads the selected native/mod dojo through `Location`
  and `Render`, keeps animated scenery independent of item previews, and attaches
  the same location atmosphere and world-camera effects used in the dojo.
- DE128 loader: supplied backgrounds already contain the logo. The importer now
  emits transparent replacements for both extra logo halves (21 total targets).
  Exact loose `textures/logos/` sprite replacement is supported by the core provider.
- All four managed assemblies and native Unity compilation passed. Wiki build
  checked 50 pages / 4717 links; menu-art import `--check` passed. The native
  `Tools/Tests/Presentation/VerifyPresentationLayouts.cs` fixture passed 43 assertions with the shop
  forge open, including rendered reward glyphs, rotated-panel separation, selected
  dojo identity, screen effects, and transparent logo replacements.
- Visually inspected captured Play-mode shop/forge and synthetic result screens
  at 2400x1080. No rewards were granted or enchantment materials spent. Full fight
  completion, applying an enchantment, all dojo choices and transition timing are
  not end-to-end playtested. Repeated editor runs also logged UGUI Selectable
  index errors, a LightingChainStart preview error and BackKeyManager teardown
  warnings; these checks do not establish an otherwise error-free play session.

Checked items below mean implemented in source, **not Unity-playtested**. The
interrupted work has been integrated and compiler errors repaired. No commit was
created. Existing Unity asset GUIDs were preserved.

Implementation notes:

- Forge uses the standalone unknown-enchantment icon in place of the bad random
  atlas region, eases its drawer/panels and charge bars, fades recipe selection,
  animates previews/buttons and briefly reveals the enchantment result before closing.
- Fight banners now call `FightPopupCinematic`; result content/header reveal with
  animation. VS presentation and its countdown are both 1.5 times faster.
- Challenges initially show their available fight/battle description; clicking
  crossfades to difficulty and back. Missing descriptions do not show empty overlays.
- Scene fades capture before the outgoing scene unloads, expose the native loader
  artwork, and reveal the destination after readiness checks.
- `sf2.underworld.set_map_colors { normal, power, duration? }` provides scene-local
  Power Mode background tints, separate from the story map. Defaults are white;
  mods reapply settings on map entry. See the Underworld wiki reference.
- `sf2.timers.set { subsystem = "battle", seconds = 150 }` is applied by DE128;
  Eclipse retains its native limits without that policy. Training and untimed fights
  are excluded. The DE change lives in the separate linked `Mods/de128` repository.
- The DE art importer has run: brown `ResearchSources/DENew/bg.png` for fight entry,
  `ResearchSources/DENew/Output/Preloader.png` for menu loading, native split panels.
- Control packs are selectable under Options > Accessibility and apply to standalone
  control sprites as well as atlas lookups. Pack images are cached for the session.
- Three inferred recovered names used by new code were given descriptive names:
  `Fight.TogglePauseMenu`, `Model.GetCurrentAnimation`, `Module.GetCurrentScreenType`.
  Declarations carry `// best guess for name`; callers and fixtures were updated.
  QuestStage's unrelated same-spelling method was preserved. No confirmed recovery
  mapping was changed.

## Verification

- PASS: all four managed projects using `dotnet msbuild` with the **current Unity
  6000.6.0f1 generated references**. Local generated projects omit existing source
  files and reference a missing VS analyzer; the temporary
  `%TEMP%/Eclipse.LocalCompile.targets` override includes Eclipse source and removes
  only missing analyzer references. Plain `msbuild` could not locate the .NET SDK.
- PASS: `Tools/Tests/Progression/TestUnderworldRuntime.ps1` — 1,282 assertions.
- PASS: `Tools/Tests/Modding/TestUnderworldApi.ps1` — 112 API checks, including map color validation,
  capability/host behavior and battle timer boundaries; prerequisite 169 warrior checks.
- PASS: `Tools/Tests/Progression/TestForgePendingTimers.ps1` — 16,219 DE foundation checks and 20 pending
  forge lifecycle checks. DE timer ownership expectations now cover both policies.
- PASS: `Tools/Tests/CharacterForms/TestFormAnimationEntry.ps1` — 71 checks; `Tools/Tests/Modding/TestSceneNavigation.ps1`
  — 35 checks, including resuming native loading after a deferred transition.
- PASS: ModdingEditor generate/check, 44 tests, LuaLS (including new fields/timer),
  and VS Code integration using the installed LuaLS/Code executables in isolated fixtures.
- PASS: wiki build, 50 pages / 4,717 links and assets. No dependency installation needed.
- PASS: `Tools/Recovery/ImportDE128MenuArt.py --check` — 19 replacements.
- `Tools/Audits/AuditUnderworld.py` completed but reports the existing missing
  `fungus_raid/layer_0_2` resource; not repaired by this UI change.
- Reviewed-map dry-run could not run: external `cross_build_map/authoritative_members.tsv`
  is unavailable at its configured path. No reviewed maps were applied.
- **Pending:** native sprite/thumbnail validation and a Unity game playtest. The guide's
  matching 2022.3.62f3 editor is not installed here; only 6000.6.0f1 was found. No
  engine migration or editor-version change was made.

## Visual acceptance still needed

- [ ] Shop: forge icon, open/close/preview/apply, requirement visibility, hint arrow,
      upgrade/buy inline icons, wheel scrolling and perk auto-scroll.
- [ ] Fight: Enter pause without immediately activating a pause button, charge/joystick
      easing, critical sound, banners/results, shortened VS sequence and raid bar counter.
- [ ] Map: challenge/difficulty crossfade, wheel scroll, press bounce, Eclipse tint,
      normal/Power Mode tint and returning to the story map without tint leakage.
- [ ] Title/settings: door transition, hover reset, mod-settings footer, globe,
      intro toggle/disclaimer/credit, upscaled scenes, control pack selection and quit.
- [ ] DE128: 150-second rounds and distinct fight/menu loader artwork.

## Engine wide
- [x] Add smooth increasing and decreasing of the magic button's charge.
- [x] Add a fading transition when switching dojos so that the scene doesn't pop in abruptly, same for switching to punching bag or kid.
- [x] Same as above but for switching to profile, shop and dojo.
- [x] Either delete or fix the current enchanting GUI.
- [x] Make scrolling across lists such as the shop items or achievements smooth.
- [x] Anchor the enchantment description to the icon of the enchantment itself.
- [x] Fix the item upgrade count displaying the font atlas.
- [x] Fix the buy screen for items so as the font atlas doesn't appear.
- [x] When learning a new perk, make the list automatically scroll down to select a perk at the following level.
- [x] Make the "Return to title" button use the door icon, and make it fade in, not appear abruptly.
- [x] Make challenges have descriptions, when clicked, switching smoothly to the difficulty and back.
- [x] Add bouncy map button animation from SE. (scale down when clicked down, back when released)
- [x] Make the size of the rewards numbers text for battles bigger.
- [x] Make pressing enter show the pause menu in a fight.
- [x] Add the snd_crit sound for critical hits.
- [x] Make the language icon that of a globe instead of specific flags.
- [x] Add a transition to the eclipse overlay color when eclipse mode is toggled.
- [x] Create a syntax similar to the one which sets the eclipse map color but for the Power Mode button in the underworld.
- [x] Add cinematic pop-in of "perfect", "great", "fight" screens in battle.
- [x] Add a cinematic fight result screen.
- [x] The back button in the mod settings menu overlaps with text.
- [x] Hovering over the back button in the options menu does not make it go back to black.
- [x] Make the overlay joystick nub track smoothly instead of abruptly when responding to keyboard or gamepad inputs
- [x] Cut back on the fight into cutscene time.
- [x] Add a setting to allow for the toggling of the intro video when selecting a save.
- [x] Add a disclaimer at the beginning of the intro sequence that the game is free, and under the white eclipse logo, the text "Team Definitive™".
- [x] Make the return to title screen animation less abrupt and more cinematic.
- [x] Use upscaled locations for title screen backgrounds.
- [x] Allow the selection of texture packs for on screen controls.
- [x] Add a quit option to the title screen.
- [x] Make the map smoothly scrollable with your mouse wheel.
- [x] Make the size of the number of health bars a raid boss has bigger, and place it in their health bar.

## DE128 wide
- [x] Make the battle timer 150 seconds.
- [x] Use the new brown loading screen after pressing fight, and the new splash screen for switching to shop and other tabs. located in F:\SF2DE\SF2DE\ExportedProject\ResearchSources\DENew\
