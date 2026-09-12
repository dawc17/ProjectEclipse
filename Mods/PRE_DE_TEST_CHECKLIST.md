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

Follow `Docs/Modding/src/content/docs/guides/character-authoring.md` to import the
native rig, edit a motion and optional geometric skin, export, validate and open
the local preview. Copy generated assets/module into a test mod and use its
warrior handle in a fight. This workflow is not an automatically installed map trial.

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
