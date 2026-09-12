# Pre-DE manual test checklist

Current additions: API 0.20 saved random streams and API 0.21 UI close
notifications. Use Unity 2022.3.62f3 and allow script compilation/import to
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
