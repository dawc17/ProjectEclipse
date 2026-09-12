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
the map-page dots. Fight all three opponents and observe decisions over several
seconds, at close and long distances:

- **Patient Gatekeeper** (kunai): prefers a high kick when playable, then waits
  about 1.5 seconds before another decision of that kind.
- **Footwork Sentinel** (batons): prefers stepping back when close and forward
  when farther away, with a short pause between choices.
- **Alternating Warden** (ninja sword): alternates playable high/low kicks with
  short pauses. A missing preferred move uses native tactics, so conditions,
  stun, equipment and current animation still affect what can happen.
- Pause/resume and restart a round: decisions must stop while simulation is
  paused and per-fighter decision memory must not leak to another opponent.
  Unmodified campaign opponents should retain their native AI.

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
