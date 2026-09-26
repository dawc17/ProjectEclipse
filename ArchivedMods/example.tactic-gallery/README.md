# Tactic Gallery

Enable this mod in the mod manager, choose **Apply & Restart**, then enter the
map. Use the bottom page dots to find **Tactic Gallery**. Select its battle and press FIGHT.
This is a separate example map page, not Act 1 tournament battle 3. If another
example opens a menu automatically, disable that example while testing.

The panels, buttons, meters and text use the original game's UI assets and font.
No new art bundle is required; fighters reuse native character templates.
Use a test profile: mode progress is saved. The example awards no items/currency.

Four fights pair visibly different native fighters with programmable brains and
a live AI-choice HUD. Win each fight to reach the next; the mode repeats.

1. Patient Gatekeeper (kunai): look for HighKick choices separated by `wait`.
2. Footwork Sentinel (batons): approach and retreat; look for StepBack / StepForward
   choices as distance changes.
3. Alternating Warden (ninja sword): look for HighKick / LowKick alternation.
4. Reactive Guardian (staff): attack nearby; it requests an eligible backward move
   during observed attack intervals, and otherwise favors a quick eligible kick.
5. Finish/leave any fight: the HUD disappears. It should return in the next fight.

The HUD reports the Lua decision, not a guaranteed animation or landed hit.
`native fallback` means the available candidates did not match the policy; the
native AI chooses instead. The reactive brain uses typed input, nominal timing
and the opponent's active animation intervals. Physical timing/interruptions
remain controlled by the game. All four retain their native portraits/equipment.

Automated checks: `Tools/TestVisualExamples.ps1` runs these exact Lua scripts.
`Tools/TestModUiUnity.ps1` also mounts their UI in an isolated Unity fixture.
Those checks do not replace the full-game tests above.
