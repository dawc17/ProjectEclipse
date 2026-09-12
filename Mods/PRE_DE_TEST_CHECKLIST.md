# Pre-DE manual test checklist

Current additions: API 0.22 generated encounters, deferred mode preparation,
programmable AI, character authoring and native toggle/slider controls. Use Unity 2022.3.62f3 and allow script compilation/import to
finish. These checks complement the automated fixtures; full-game acceptance
has not been claimed.

## Setup

Open **Mods** from the title screen, enable the example being tested and choose
**Apply & Restart**, then enter Campaign. Enable one example at a time so other
combat patches do not obscure the result. Examples already live in this
repository's `Mods` folder; do not install a second copy with the same ID.
Keep the Unity Console visible for errors. Record the enabled mods and the
profile's progress when reporting failures.

## 1. Charged Strike HUD — test this first

Enable `example.charge-ui`. Use a profile that has unlocked the third Act I
tournament fight, then enter that fight. Test both normal and eclipse replay
mode; the example now patches both native fight identities.

- The HUD appears near the top-right safe-area corner. It uses the game font,
  original beveled button and bar textures, with no white missing-asset blocks.
- After five seconds of active combat, the meter reaches 100% and **ARM NEXT
  STRIKE** becomes clickable. Pausing should not charge the meter.
- Click with the mouse. The label changes to **Next hit: double damage**.
  A blocked hit should not consume it; the next positive unblocked hit should
  consume it once and reset the meter. The modifier doubles pending outgoing
  damage before native defenses, so displayed final damage can also depend on
  armor, hit type and other native effects.
- Finish a round, retry or leave the fight and enter again. There must be one
  fresh HUD at zero charge, no old armed bonus and no duplicate buttons/bars.
- After leaving, normal map/menu/fight input should work; there must be no
  invisible overlay intercepting clicks or held controls replaying unexpectedly.
- Resize the game view/window and test another aspect ratio. The HUD should
  remain inside the safe area and keep its game styling.
- If available in the build, switch between English and Polish. Status/button
  strings should refresh without broken glyphs or a separate font. HUD buttons
  currently use pointer input; keyboard/controller HUD focus remains open work.

## 2. Branching Trial

Enable `example.branching-trial`; select its added map zone and battle.
After restarting, use the bottom map-page dots to find **Branching Trial**.
Other examples may initially focus their own page. The entry quest reveals
the trial on map-session start, and the footer should show its name.

- Start, win, lose, and re-enter fights. The mode should stay playable after
  completion and should charge no entry item or grant example rewards.
- Leave through normal game menus after an encounter, restart, and continue.
  Progress should resume instead of becoming locked or restarting unexpectedly.
- Disable the mod, restart, then enable it again. Owned progress should remain.
- Linear completion bricks are intentionally hidden for custom routes.

For a fresh mode save, the intended routes are **1 → 3**, then **1 → 2 → 3**;
losing returns to encounter 1. The fighters are **Gatekeeper** (kunai),
**Bulwark** (steel batons/heavy build), and **Night Warden** (ninja sword/robe).
The first run should skip Bulwark; the next should include him. Verify their
distinct portraits, names and weapons in actual combat. Exact step routing and
duplicate-result handling are also covered by the automated native-host fixture.

## 3. Seeded Trial

Enable `example.seeded-trial` and repeat the save/reload, loss, replay and
disable/re-enable checks above. The stream must survive normal saves/reloads;
changing the mod's default seed does not overwrite existing saved state.

With a fresh saved field and default seed 12345, the first two completed runs
take **Wayfarer → Needlehand → Storm Ronin**, then **Wayfarer → Storm Ronin**.
Wayfarer is unarmed, Needlehand uses sai, and Storm Ronin uses nunchaku with a
conical hat. A loss returns to encounter 1 without drawing
another random value. Exact seeded sequences and per-mod isolation are tested
automatically. A forced process kill before the next game save is not a promised
persistence boundary.

## 4. Original UI regressions to recheck

These are regression checks from the original report, not a claim that the
latest API changes repaired all of them:

- Enter/leave the map repeatedly with eclipse enabled: map appearance matches
  the current mode, and boss replay counts appear.
- Compare Easy/Normal/Hard difficulty bars for distorted ends/fills.
- Open move details: the damage icon appears and multi-hit values stay on one
  line in the intended multiplication format.
- Check Gates of Shadows completion artwork and completion videos on a profile
  that reaches that transition; an already-completed profile may not replay it.
- Open enchanting: the recipe panel meets the Apply/Back panel, clicking a
  recipe previews candidates, and forge-orb totals show all digits in game font.

For any failure, report the enabled example, exact steps, screenshot/video,
whether it happened after reload/scene change, and the first relevant Console
error. No new DE asset import or full downstream port is included in this pass.


## 5. Generated Expedition: new procedural and asynchronous workflow

Enable `example.generated-expedition`, Apply & Restart, and use the bottom map
page dots to find **Generated Expedition**. This is a separate mod map entry;
the Act I third tournament fight does not demonstrate these features.

- Press Fight. A parchment preparation menu must appear with the game font,
  original checkbox, slider and beveled Begin button. Combat must not start yet.
- Toggle **Stronger opponent** and drag the round timer from 30 to 90 seconds.
  The displayed value must follow. Try keyboard/controller focus and horizontal
  slider adjustment. Resize the window and check for clipping or missing assets.
- Back/Escape cancels the setup and leaves the map usable. Reopen it and press
  Begin once; rapid double clicks must not start duplicate fights.
- The fight should use the selected timer. Stronger opponents are three levels
  above the ordinary encounter setting. The generator selects one of the unarmed,
  sai or nunchaku fighters. Their identity is a generated encounter choice, not
  just a route that skips an existing battle.
- Leave/reload after the encounter starts but before settlement, then re-enter.
  The prepared opponent/settings should be retained without another setup or roll.
- Complete the encounter. The next of three steps gets a fresh preparation screen.
  Complete the expedition and confirm it repeats; this example charges no entry
  items and grants no rewards. Stop/restart Play mode while setup is open and
  confirm no old menu/request resumes into a fight.

## 6. AI Dojo: new programmable opponents

Enable `example.programmable-ai`, Apply & Restart, and select **AI Dojo** using
the map-page dots. Fight all four opponents and observe decisions over several
seconds, at close and long distances:

- **Patient Gatekeeper** (kunai): prefers a high kick when playable, then waits
  about 1.5 seconds before another decision of that kind.
- **Footwork Sentinel** (batons): prefers stepping back when close and forward
  when farther away, with a short pause between choices.
- **Alternating Warden** (ninja sword): alternates playable high/low kicks with
  short pauses. A missing preferred move uses native tactics, so conditions,
  stun, equipment and current animation still affect what can happen.
- **Reactive Guardian** (staff, green armor): the fourth encounter. Attack at
  close range to trigger legal backward movement during an active attack window.
  Remain close without attacking to see quick-kick choices, then move far away
  to allow native approach behavior. Its voluntary attack pause must not block
  an otherwise legal defensive retreat. This opponent uses interval, control and
  timing metadata, so no Lua editing is needed to exercise those APIs.
- Pause/resume and restart a round: decisions must stop while simulation is
  paused and per-fighter decision memory must not leak to another opponent.
  Unmodified campaign opponents should retain their native AI.

For API 0.45 authoring acceptance, use the `choose_quick_kick` callback from
`Docs/Modding/src/content/docs/api/moves-and-tactics.md` in a test tactic and
set its manifest to `api = ">=0.45 <1.0"`:

- With both native and custom kick-tap moves eligible, it should choose the
  candidate with the shortest nominal clip length. It should ignore looping
  candidates and use native fallback if no kick tap is available.
- Inspect a custom move with a known nonzero first sample and `mid_frames`:
  `timing` should report the inclusive sample range and
  `(last_sample - first_sample + 1) * (mid_frames + 1)` nominal frames. Do not
  interpret this as a measured recovery/hit time.
- Confirm a directional hold plus kick tap appears as separate `inputs`
  entries. Turn the fighter around: Forward/Back metadata stays relative to
  the authored facing, while native dispatch mirrors the on-screen control.
- Editing a candidate's nested timing/control fields must not alter native
  playback or subsequent snapshots. The original candidate identity is selected.

The automated `Tools/TestModAi.ps1` checks the native adapter and Lua snapshot
isolation; these full-game observations remain a separate acceptance step.

For API 0.46, use `evade_active_attack` from the same guide in a test tactic:

- Attack at close range. When a backward movement candidate is legal during
  an observed attack interval, the callback should select it. Native reaction
  throttling and current uninterruptible states still apply; this is not an
  automatic guarantee of dodging a hit.
- Observe `event.opponent.animation` through attack and recovery. Its intervals
  should reflect the current native window, not every interval authored on the
  move. Repeat after changing facing and with a custom named interval.
- Compare with `fighter:snapshot().opponent.animation` inside a combat callback.
  Both paths expose the same fields; different capture moments can see different
  native states. Stopped/unavailable animation controllers report `nil`.
- Retain an observation, then capture again after an interval ends. The old
  table remains a historical value; it must not change or alter the fresh result.

## 7. Character/animation authoring

Follow `Docs/Modding/src/content/docs/guides/gymnast.md` for the prepared Gymnast
IK body, model export and installable preview package. Use Blender 5.0+ for the
pinned upstream scenes. The launcher opens the body in Pose Mode with controls
and registers the supplied add-on only for that process.

- Confirm the prepared body is visible and hand/heel IK controls move it.
- Export with `--package` and enable the generated mod through Apply & Restart.
  Find Character Preview using the map-page dots. Its opponent should perform
  the authored motion when playable. The preview is repeatable and has no rewards.
- For `mid_frames = 2`, sample spacing is three simulation frames; animation and
  interval bounds still use stored sample indices. Check timing in combat.

- Confirm the exported body/skin loads without errors, equipment follows the
  rig, and the authored movement is available only to that character.
- Test both facing directions, movement, hit reactions and knockdown. Inspect
  skin attachment under the largest bends; the point preview cannot prove skin
  rendering or contact behavior.
- Add the documented attack interval to an appropriate authored motion and
  verify its input, damage, hit timing and impulse. The generated default module
  is a movement preview and intentionally has no damaging interval.
- Verify another fighter retains its normal controls. Keep the Console visible.
  Blender export and the native animation-reader fixture already pass, but this
  full-game visual/combat acceptance remains necessary.


## 8. Quest suppression

Enable `example.quest-suppression`, Apply & Restart, then enter the map. Only
"Replacement introduction" should appear. Remove the suppression call and restart:
both introductions should appear. Disable the example and restart: neither appears.
For actual story replacements, separately test an in-progress saved quest, lazy
extension loading, direct Run/Foreach references and re-enabling the dependency.
The automated manager fixture does not prove full-game serialized resume timing.

## 9. Animated Arena

- Enable example.animated-arena, Apply & Restart, and select its map-page dot.
- Enter the repeatable fight: the battlefield backdrop should fill the arena,
  drift vertically and fade through a four-second loop. Combat uses normal rules.
- Pause and resume; record whether the decorative backdrop moves while paused.
  No pause-safe combat/hazard timing is claimed by this scenery API.
- Exit/reenter several times: check scale, position, opacity and absence of duplicate
  layers. Retry after a loss and after a completed fight.
- Disable the mod and restart: its page should disappear and original arenas
  should keep their normal appearance.

Lua/projection and native curve checks pass. This checklist is the outstanding
rendered and full-game acceptance, not a report that it has passed.

For Animated Arena on API 0.25, also listen for Samurai Spirit or Blade Dance.
A new entry may repeat the same track. Check music volume/mute and menu return;
there should be no simultaneous leftover fight tracks. This is a random choice
per entry, not continuous playlist advancement.

## 10. Dojo Selector

- Enable example.dojo-selector, Apply & Restart, and find its map-page entry.
- Press FIGHT to open the chooser. Select BATTLEFIELD DOJO, then use the normal
  game menu to enter Dojo. The animated battlefield should replace the backdrop.
  The chooser must not start a fight or award rewards.
- Exit/reenter and restart after a normal save; the choice should persist.
- Disable the mod and restart: the normal dojo returns. Reenable and restart:
  the saved choice returns without having to choose again.
- RESTORE DEFAULT returns to the native dojo on next entry. BACK/Escape leaves
  the choice intact. Reset is disabled for another mod's saved preference.
- Check a second profile: it must not inherit the first profile's selection.
- Enter an ordinary story/tournament/raid fight: its own arena must remain.
- Check keyboard/controller navigation and repeated open/close for stuck input.

These are pending full-game checks. Automated save and Lua UI callback fixtures
pass, but do not prove visual rendering, actual disk saving or live scene behavior.

## 11. Story notifications (API 0.28)

Enable `example.story-observer`. This example logs to Unity's Console/player log;
there is no on-screen overlay to look for.

- Make a normal shop purchase: expect one `Story Observer purchase:` message with
  its qualified item ID. Cancel a purchase: expect no completion message.
- Complete an enchantment: expect one `Story Observer enchantment:` message with
  item and recipe identities. Opening or canceling the forge must not emit it.
- Confirm normal native quests still react, and purchases/enchantments retain
  their ordinary results and costs.
- Switch profiles and repeat: no notification from the previous profile should
  arrive, and each new action should still produce only one observer message.
- Disable the observer and restart: no new observer messages should appear.

These full-game checks remain pending. The transport, production-method fixtures,
actual Lua subscriptions and shipped observer script pass automated checks.

### Level notifications (API 0.29)

- With Story Observer enabled, gain a level through normal experience. Expect
  `Story Observer level: old -> new` once, with the final visible player level.
- A single reward crossing several level thresholds should produce one message,
  not one per intermediate level. A gain that reaches the cap must still emit it.
- Rewards below the next threshold and experience received while already at the
  cap should not emit level messages.
- Loading/reloading a profile and opening equipment comparisons must not emit
  level messages. Disable the observer and confirm messages stop.

These remain full-game acceptance checks. Automated fixtures execute native
experience processing with controlled inventory/save dependencies and real Lua.

### Scene entry (API 0.30)

- With Story Observer enabled, enter map, shop, profile, dojo and a fight. Expect
  one `Story Observer scene: name` message after each destination initializes.
- Loader/preloader/credits should not emit messages. Returning to a previously
  visited scene should emit once again.
- Navigate away quickly during loading: no pending entry message should arrive
  for the abandoned destination. Profile switching must drop old pending entries.
- Use the scene-enter UI snippet in the public story reference (add `ui.create`).
  On entering the map, the normal game-styled Back button should appear. Click it,
  use Escape, and leave/reenter the map: input and UI cleanup should remain normal.
- Enter a fight with native dialogs/prefight UI: scene entry must not bypass them
  or grant early combat control.

Native hook/coroutine tests use controlled Unity lifecycle services. In addition,
an isolated Unity 2022.3.62f3 play-mode fixture now passes
15 checks covering actual scene unload and helper coroutine lifetime, including
deactivation/reactivation cancellation. Native full-game scenes, rendered menu
placement and input still need the manual checks above.

## 12. Native menu navigation (API 0.31)

Enable `example.scene-menu` and enter the map. The Travel menu should use the
normal game font, parchment/button styling and keyboard/controller navigation.

- Use SHOP, PROFILE, DOJO and MAP. Each accepted transition should load that
  native destination and open the example there. No duplicate transitions.
- Choose the current destination: close the menu without reloading the scene.
- BACK/Escape closes the example without navigation; normal input must resume.
- Native dialogs, lock screens and pending encounter preparation must prevent
  navigation. A rejected request displays `Unavailable right now`.
- Native quest/tab interceptions must retain their normal behavior, without a
  forced second transition. Actual fight exits still require normal surrender or
  result handling; this API must not provide a shortcut around either.
- Disable the example and restart: no Travel menu should remain.

These are pending full-game checks. Production navigation/native transition
fixtures and actual Lua example button tests pass with controlled host services.

Additional automated evidence: the expanded Unity UI play-mode fixture passes 99
checks, including the shipped Scene Menu in the production renderer/input bridge.
It verifies game font/sprites, button bounds, directional submit, dialog blocking,
rejection labels, teardown and remounting. Navigation results are controlled, so
actual native destinations and physical-device checks above remain pending.

## 13. Existing fight opponent replacement (API 0.32)

In a test mod declaring content.register and content.patch, register an opponent
using a known working template/loadout. Pass its warrior handle as the sole entry
in warriors to sf2.fights.patch targeting core:fights/zone_1/boss_lynx/1.

- Test the normal encounter at that ID. To test the replay in Eclipse mode, also
  patch core:fights/zone_1/boss_lynx_eclipsemode/1 with the warrior list. Confirm the
  replacement name, portrait, equipment, model and attacks.
- Confirm the encounter retains its original campaign identity, unlock/progress,
  rules and rewards; replacing opponents must not reset completion.
- Test a second ordered opponent using the encounter's native progression rules.
- Disable the patching mod and restart: original opponents should return, without
  losing campaign progress.

These full-game checks remain pending. The automated fixture checks Lua validation,
conflicts, order/fingerprints and XML collection replacement with a controlled
warrior builder; it does not replace gameplay acceptance.

## 14. Scoped fight item rewards (API 0.33)

Use the item-drop example in the public fight-patch reference on a test profile.
Its Eclipse target is core:fights/zone_1/boss_lynx_eclipsemode/1; vanilla uses a
separate battle from BOSS_LYNX. A patch does not follow EclipseToggleName automatically.

- Win the first Lynx bodyguard encounter in Eclipse mode: verify the configured
  item appears in rewards and reaches inventory through native settlement.
- Repeat in normal mode: the Eclipse-only addition must not appear.
- Verify original money, gems, experience and shared rewards remain unchanged.
- Add min_level/max_level bounds: test below, at and above each inclusive boundary.
  Other matching native level rows still add their rewards.
- Replay, save/reload and disable/restart: confirm native eligibility/progress rules
  remain intact and disabling restores base reward definitions.
- Test two mods targeting the same scope (conflict) and distinct scopes (coexistence).

These are pending full-game checks. Automated registration/projection coverage does
not yet verify the complete native item builder, parser and settlement path.

Reward verification follow-up: 49 checks now execute production reward builders,
item parsing, weighted selection and native mode/level composition with controlled
host services. They also verify repaired lottery null merging and independent slot
lists across repeated evaluations. Actual inventory settlement and full-game UI,
replay eligibility and save/reload checks above remain pending.

Result-selection follow-up: the fixture now passes 55 checks, including the actual
FightResult item handler with controlled catalog/ownership/upgrade services.
Already-owned equipment is skipped; use an unowned reward item or fresh test
profile. Repeatable mod consumables retain their native repeat-grant exception.
Actual inventory mutation and persistence remain unverified.

Runnable fixture: enable example.eclipse-reward, Apply & Restart, and follow its
README for section 14. Its actual manifest/Lua now passes canonical catalog checks;
this does not mark the full-game grant and persistence checks complete.

## 15. Learned-perk queries (API 0.34)

In a profile-loaded UI/story callback, query sf2.profile.perk with a registered perk
handle (for example sf2.perks.get("core:perks/PERK_COBRA") acquired at entrypoint).

- Before learning: learned=false, upgrade=nil.
- After learning/upgrading: learned=true and the stored native UpgradeLevel.
  Zero is valid and must not be treated as unlearned.
- Reset perks or switch profiles: the next query must reflect that profile's list.
- Save/reload and query again: learned state and upgrade should persist.
- Applying equipment enchantments or temporary combat effects must not make an
  otherwise unlearned perk appear learned.

These full-game checks remain pending. Host tests use controlled UserPerks entries;
Lua tests verify capability/handle rejection and detached snapshots.

## 16. Item classification (API 0.35)

After profile load, query sf2.profile.item with known weapon, armor and consumable
handles. Compare type/subtype to their native item definitions. Repeat before and
after acquisition: classification should not depend on ownership. WEAPON_NUNCHAKU
should report Weapon/Nunchaku. Empty native subtype stays an empty string; missing
runtime metadata produces nil. Earlier returned tables must remain unchanged.

Controlled host/Lua tests pass; full-game catalog comparisons remain pending.

## 17. Item acquisition events (API 0.36)

Subscribe to item_acquired under story.events and log item, previous_count and count.
Grant an unowned reward item and increase an existing consumable stack: each positive
native grant operation should report its before/after counts. Zero/removal operations
and pending purchases with no count increase should not report acquisition. Compare
purchase callbacks separately to avoid double-counting. Switch profiles during a
native dialog: stale notifications must not reach the new profile.

The hook does not cover separate delivery-completion or direct inventory-edit paths.
An outer reward flow can still add enchantments afterward. Full-game acceptance is
pending; native/Lua fixtures use controlled services.

API 0.37 delivery follow-up for section 17: complete a pending empty-item delivery.
Expect one item_acquired notification after completion, and none when checking it
again. Upgrade-only delivery must remain silent. A delivery quest that grants the
same item must not cause a second notification for that same increase. These
full-game checks remain pending; the extracted native fixture passes 20 checks.

For sections 17 and the delivery follow-up, enable the updated example.story-observer
(API 0.37+) to see acquisition identities, counts and deltas in the Console/player
log. Its README describes using example.eclipse-reward alongside it. Exact known/
unknown-item log messages pass automated Lua checks; full-game acceptance is pending.

### Runtime item IDs (API 0.38)

Use the acquisition callback example in the public profile reference with
`story.events`, `profile.read`, API >=0.38 and a core dependency. No
`content.register` capability is needed. Acquire a core item and confirm its
logged current quantity matches inventory. Unknown event items are skipped.
Nested grants can make current quantity newer than the event snapshot. Verify
profile switching reads the newly active inventory. Full-game checks pending.

### Equipped profile records (API 0.39)

From an after-load UI/story callback, log sf2.profile.equipment() entries with
profile.read enabled. Compare item IDs, type/subtype and upgrades to the equipment
screen. Swap weapons and query again; empty slots should have no equipped record.
Change profiles and ensure the new equipment appears. Enter a fight with temporary
rule-imposed equipment and verify the query still represents profile equipment.
The public profile reference contains a Katana condition example. These full-game
checks remain pending; no automatic popup is added by the API.

### Battle result observer (API 0.40)

Enable example.story-observer and launch a normal encounter from the map. Finish
it and inspect the Story Observer battle log: one line with fight ID and outcome.
Repeat in Eclipse and verify the eclipse marker; surrender should report surrender.
Try a repeatable owned mode and ensure successive launches each produce one line.
For equipment predicates, use the public story guide's Katana callback; temporary
rule equipment should be captured from model parameters when available. Surrender
may provide no equipment, and no profile-equipment fallback is implied. Native raid
timeout variants, instant-win path, profile interruption and deferred lottery wins
still need acceptance testing. This event is not proof that all loot/save work has
finished, and adds no UI by itself.

### Blade Discipline example

Enable example.katana-achievement with an eligible test profile. Defeat Butcher
with a katana in normal, Eclipse replay or the intermission gauntlet. Expect one
Blade Discipline unlocked log and the achievement in Profile; no popup or gems
are promised. Bodyguard wins, losses and non-katana weapons must not count. After
unlocking, repeat a qualifying win and confirm no second unlock log. Restart and
check the achievement persists. These full-game checks remain pending.

## Profile write recovery acceptance (pending)

Use a disposable test profile. Verify ordinary progress and mod-owned inventory
survive quit/restart with no pending .eclipse-write records after completed saves.
Verify intentional profile replacement/reset cannot restore an earlier pending
record. Disk fault/replay behavior is covered by TestProfileWriteJournal.ps1 and
TestProfileSaveBoundary.ps1; do not interrupt or corrupt a real player's save to
run these checks. Lottery claim recovery now has implementation and controlled
fixtures; full-game claim/restart acceptance remains pending.

### Custom UI artwork (API 0.41)

- Use an image node with a sprite handle from sf2.assets.sprite and explicit
  positive width/height; see the wiki UI image example. No DE assets are needed.
- Open it within the shared parchment modal alongside a text caption; verify the
  original game font and unchanged image aspect ratio in a square and wide layout.
- Click the artwork: it must not trigger a button callback. Close/reopen the view;
  other views using the same sprite must retain their artwork.
- Try a string or forged table in sprite, a missing sprite, or zero dimensions:
  initialization must fail without leaving a blank exclusive modal behind.
- Disable/reload the mod and change scenes: existing UI ownership/cleanup rules
  must still apply. Isolated Unity checks passed; these full-game checks are pending.
Lottery artwork: check the saved slot or first-item icon preserves aspect ratio, a short summary is visible without blank scrolling, missing optional art leaves Claim usable, and dismiss/reopen keeps the same reward. Full-game acceptance remains pending.

### Live custom UI artwork (API 0.42)

- In a menu/modal with an image node, call sf2.ui.set_sprite from a button callback
  using a second handle from sf2.assets.sprite. The image must change in place;
  panel size, scroll position, focus and the original game theme should remain.
- Switch between wide and square art, then hide/show the image. Check aspect ratio
  and the latest selected artwork; other views sharing the original sprite remain.
- Missing replacement artwork closes the surface through normal error cleanup.
  Wrong handle types and attempts to update a closed view must be rejected.
- Runtime/Lua and isolated Unity tests passed; full-game/device acceptance pending.

### Custom UI grids (API 0.43)

- Use the wiki's Grid layouts example with ui.create and API >=0.43. Check the
  six buttons form two rows of three with the original game font/button textures.
- Select cells by pointer, Tab/Shift+Tab, arrows and D-pad/stick. Arrows should move
  by grid rows/columns; Tab remains ordered. Disabled cells must be skipped;
  hiding a cell should close its layout gap. Directional movement must not wrap
  across a grid edge. Slider Left/Right input must stay on the slider at its limit.
- Put a taller grid inside a scroll node. Navigate to a lower button: the viewport
  must scroll to reveal it, and activation must reach that button's callback.
- Close/reopen and change scenes. Verify restored focus, no leftover input capture,
  and unchanged artwork shared by another view. Full-game acceptance is pending;
  isolated Unity verifies geometry, styling, ordered focus, scrolling and cleanup.
- This is fixed-column layout within the existing 256-node limit. Large virtualized
  collections remain open work; directional navigation now has isolated Unity
  coverage but still needs a physical keyboard/controller playtest.

### Character packages with multiple animations

- Export at least two motions against the same rig. Package the first using
  --animation and add another using --clip kick Kick 0 path/to/kick.bin.
  The Gymnast guide documents per-clip timing, valid controls and output files.
- Enable the generated mod, open Character Preview and watch the opponent cycle
  through eligible clips. Verify each motion retains its own timing and no damage
  is dealt by the generated preview moves.
- Test authored input bindings only on the intended warrior, both facing directions,
  equipment attachment and skin deformation. These require a full-game test.
- Check preview.html and preview-kick.html separately. Existing output folders,
  duplicate clip/control names and malformed clips must fail without overwriting
  authored files or publishing a partial package.
- This packages several validated native exports; it does not implement automatic
  retargeting, arbitrary character controllers/forms or attack authoring in Blender.

### Battle lottery continuation and queued quest context (pending game acceptance)

- Use a disposable profile and a winning fight whose composed reward contains a
  lottery. Return from combat: the original-style reward dialog should open.
- Choose Later, navigate away and return: the same reward must reopen. A new fight
  must not replace the pending reward. Restart before claiming and check the draw
  remains unchanged.
- Claim once: inventory should change once, and matching fight-end quests should
  run with the original fight, raid identity and lottery item. Reopening/restarting
  must not grant the same draw again.
- Queue two different quests while the first waits on a dialog. Trigger another
  event before the second starts. Each quest must retain its own event parameters.
- Save/reload two queued resumable quests with different fight or lottery contexts;
  the second must not inherit the first quest's fight/item/spin context.
- Put a checkpoint after an observable action, save while a later dialog waits,
  then reload. Resume at the saved checkpoint; do not replay the earlier action.
- Empty/invalid lottery pools still report an authoring error; no fallback prize
  or automatic skipped reward is defined. Paid spins and multiple pending draws
  are not implemented. Controlled queue/disk tests do not prove native crash recovery.

## Forge candidate exclusion (API 0.47)

- [ ] Use the complete sf2.forge.exclude_candidate example in the forge wiki with API >=0.47, content.register/content.patch and the core dependency. Enable and Apply & Restart.
- [ ] Open Complex enchantments for a weapon: Monk set enchantment must be absent from the recipe preview and eligible rolls. Existing enchanted equipment must retain its enchantment.
- [ ] Compare armor/helm previews and recipe costs, timers and power ranges against the disabled-mod baseline: unchanged.
- [ ] Disable the mod and Apply & Restart: the weapon candidate returns when its usual native eligibility conditions are met.
- [ ] Two enabled mods excluding that same target must report a conflict. A different equipment category may coexist.

Automated command: ./Tools/TestModForgeExclusions.ps1 (Lua registration, native filtering and compiled adapter lifecycle). These checks do not replace the game checks above.

## Forge deviation overrides (API 0.48)

- [ ] Use sf2.forge.override_deviation from the forge wiki with Simple/weapon and minimum=15, maximum=75; enable and Apply & Restart.
- [ ] At the same equipment level, inspect recipe power range and complete weapon enchantments: random-aspect candidates use the new delta from the normal base aspect. Fixed values/compound expressions remain unchanged.
- [ ] Armor/helm/ranged/magic settings, prices, timers and existing enchanted equipment stay unchanged.
- [ ] Disable and Apply & Restart: Simple weapon range returns to its native -30..30; existing completed enchantments keep their saved power.
- [ ] A second mod overriding Simple/weapon conflicts. Overrides for different categories and candidate exclusions coexist.
- [ ] Targeting Complex/weapon must fail application and restore all earlier forge overlays from that failed application.

Automated verification: ./Tools/TestModForgeExclusions.ps1 and ./Tools/TestModForgeDeviation.ps1. Native fixtures verify projection and lifecycle, not the random draw, rendered recipe UI or full-game unload.

## Default equipment enchantments (API 0.49)

- [ ] Use the set_default_enchantments example in the forge/items wiki (WEAPON_KNIVES, precision weapon perk, aspect 100), with content.register/content.patch and core dependency. Enable and Apply & Restart.
- [ ] On a test profile where Knives have not been bought, inspect their shop enchantment preview and acquire them. Confirm precision is present at the specified aspect.
- [ ] Save/restart and confirm the acquired enchantment persists. Previously owned equipment must not be retroactively modified by enabling the loadout.
- [ ] Disable and Apply & Restart: original acquisition defaults return, while the already saved enchantment remains.
- [ ] Empty entries remove acquisition defaults; omitted aspect uses the perk default. Different items coexist; two mods targeting the same item conflict.

Automated checks: ./Tools/TestModDefaultEnchantments.ps1 and ./Tools/TestModForgeExclusions.ps1 (the latter also runs actual Lua loadout cases). Neither is a full-game acquisition/save/render test.

## Innate equipment perks (API 0.50)

- [ ] Apply the set_innate_perks wiki example to Knives, equip them and enter a new fight. Verify the chosen native precision effect follows its normal activation conditions.
- [ ] Unequip Knives and enter another fight: the added effect must be absent. Confirm existing saved forge enchantments are unchanged.
- [ ] Disable and Apply & Restart: original innate effects return for subsequent fights. The operation does not refresh already constructed fighters.
- [ ] Attach a registered Lua-backed perk with no loadout parameters and verify its callback in a new fight. Configure its initial values during perk registration; nonempty loadout parameters must be rejected.
- [ ] Default enchantments and innate perks can target the same item; two innate loadouts on that item conflict. Empty entries remove innate effects.

Automated commands: ./Tools/TestModInnatePerks.ps1 and ./Tools/TestModForgeExclusions.ps1. Native model collection is tested; real fight effect activation remains an acceptance check.

Automated innate execution check: run `pwsh -NoProfile -File Tools/TestModInnateLua.ps1` after the managed build. It uses the compiled game ModRuntime and real MoonSharp session to check initial parameters, state across rounds, isolated instances and missing definitions. Physical fighter operations are controlled; run TestModFightBeginRuntime separately for source selection. This does not replace a game playtest.

- [ ] Use a native TwoHandedBlunt weapon with TacticSubtype=TwoHanded in an AI fight, then disarm the fighter: verify weapon and barehand movement/attacks remain valid. Check the other fighter's weapon behavior stays independent. Automated prerequisite: `pwsh -NoProfile -File Tools/TestItemTacticSubtype.ps1` (native classification helper, clone/merge, canonical metadata and own/enemy updates; no physical combat).

- [ ] With manifest API >=0.51, register a custom mace with subtype=TwoHandedBlunt and tactic_subtype=TwoHanded, provide matching assets/moves and a shop listing, then equip it on an AI fighter. Verify the mace animation family is retained and AI uses the intended table group, including after disarm. Removing tactic_subtype falls back to subtype. Changing it should trigger the normal content compatibility handling.

- [ ] API >=0.52: use items.set_tactic_subtype on a core weapon; start a new fight and verify AI grouping independently of animation subtype. Try group empty to select subtype fallback. Disable and Apply & Restart to restore the original group. Two mods targeting the same weapon should report a conflict. Existing fight copies are not refreshed.

- [ ] Buy multiple units through a supported consumable/shop quantity flow: granted quantity must match the selected quantity and charge. Default single-unit purchases should behave as before. Automated dispatcher check: `pwsh -NoProfile -File Tools/TestPurchaseQuantity.ps1`; real inventory/save behavior still needs a playtest.
