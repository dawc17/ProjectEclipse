# Third Strike Trial

Enable the mod at the title screen, apply/restart, then enter Campaign and select
Third Strike Trial on its added map page. Hit the guardian: only every third
positive incoming hit can reduce health. Each new round starts a fresh counter.
At or below one third health, the guardian loses its guard and every hit can
damage it. This uses combat snapshots. Normal blocking and other game rules still apply. No enchantment is required.

This example uses core art and the default warrior template. It changes no
inventory or shared economy. Rule counters are transient, separate for every
rule and fighter, and are discarded for the next fight. Disabling the mod leaves
its owned progression inert under the existing missing-mod save contract.
If you add `attribute_alignments` to the guardian, those rows append once to
the default template's rows; leave the field out to inherit them unchanged.

For a visual challenge, add `sf2.rules.light_in_the_darkness {
id = "spotlight", radius = 0.2, shape = 1 }` to `scripts/main.lua` and include
the returned handle in the fight's `rules` array. The circle follows the player;
it changes visibility only and needs `content.register`.

Automated verification: `Tools/TestBattleRules.ps1`. A full in-game playtest is
still required to validate presentation and the complete encounter flow.
