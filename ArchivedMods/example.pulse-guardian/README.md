# Pulse Guardian

Enable this mod in the mod manager, choose **Apply & Restart**, then enter the
map. Use the bottom page dots to find **Pulse Guardian**. Select its battle and press FIGHT.
This is a separate example map page, not Act 1 tournament battle 3. If another
example opens a menu automatically, disable that example while testing.

The panels, buttons, meters and text use the original game's UI assets and font.
No new art bundle is required; fighters reuse native character templates.
Use a test profile: mode progress is saved. The example awards no items/currency.

A staff guardian alternates between three seconds of damage immunity and three
seconds of normal vulnerability. The HUD announces SHIELD UP / SHIELD DOWN,
shows progress to the next transition and counts stopped/landed positive-damage
callbacks. This demonstrates frame-driven Lua rules, incoming damage control,
a live progress widget and fight/round UI lifetime.

1. During SHIELD UP, land a hit: the guardian's health must stay unchanged and
   Stopped must increase. Normal knockback/animation can still happen.
2. Wait for SHIELD DOWN and land another hit: health should fall and Landed rise.
3. Check that the shield returns after the next interval; pause/resume should
   pause combat time rather than skipping the phase.
4. Finish/leave the fight: the HUD must disappear. Reenter: counters start at zero.

Counts describe damage callbacks, not button presses or completed combos. The
meter is UI presentation, not a new glow or shader on the fighter.

Automated checks: `Tools/TestVisualExamples.ps1` runs these exact Lua scripts.
`Tools/TestModUiUnity.ps1` also mounts their UI in an isolated Unity fixture.
Those checks do not replace the full-game tests above.
