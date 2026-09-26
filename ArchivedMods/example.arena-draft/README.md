# Arena Draft

Enable this mod in the mod manager, choose **Apply & Restart**, then enter the
map. Use the bottom page dots to find **Arena Draft**. Select its battle and press FIGHT.
This is a separate example map page, not Act 1 tournament battle 3. If another
example opens a menu automatically, disable that example while testing.

The panels, buttons, meters and text use the original game's UI assets and font.
No new art bundle is required; fighters reuse native character templates.
Use a test profile: mode progress is saved. The example awards no items/currency.

Each of three encounters pauses at an interactive draft. A saved RNG stream
shuffles three grid choices: a barehand fighter, sais fighter and nunchaku fighter.
Your selected fighter, difficulty and timer become the actual generated encounter.
This demonstrates grids, toggle/slider input, asynchronous mode resolution,
procedural ordering and saved encounter plans.

1. Select Needlehand or Storm Ronin: the Selected label must change.
2. Move the timer slider to its right end (90 seconds), enable Veteran and press
   FIGHT SELECTED OPPONENT. Confirm the portrait/name/weapon matches your choice
   and the battle timer starts at 90. Veteran adds 3 to the encounter's level.
3. Win: the next encounter opens another draft. Compare the shuffled cell order;
   random shuffles can legitimately repeat.
4. Press BACK/Escape on a draft: no fight starts. Open again and pick a different
   fighter. Cancelling does not rewind the saved random stream.
5. After an encounter has been prepared and the profile saved, restart before
   completing it: its saved plan should resume without rerolling its opponent.
6. Try mouse and keyboard/controller navigation; buttons and slider should remain
   usable, with no modal intercepting input after the draft closes.

The three candidates are a small authored pool, not generated meshes or animations.

Automated checks: `Tools/TestVisualExamples.ps1` runs these exact Lua scripts.
`Tools/TestModUiUnity.ps1` also mounts their UI in an isolated Unity fixture.
Those checks do not replace the full-game tests above.
