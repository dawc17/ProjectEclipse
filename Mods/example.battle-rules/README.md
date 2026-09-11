# Third Strike Trial

Enable the mod at the title screen, apply/restart, then enter Campaign and select
Third Strike Trial on its added map page. Hit the guardian: only every third
positive incoming hit can reduce health. Each new round starts a fresh counter.
At or below one third health, the guardian loses its guard and every hit can
damage it. This uses API 0.9 combat snapshots. Normal blocking and other game rules still apply. No enchantment is required.

This example uses core art and the default warrior template. It changes no
inventory or shared economy. Rule counters are transient, separate for every
rule and fighter, and are discarded for the next fight. Disabling the mod leaves
its owned progression inert under the existing missing-mod save contract.

Automated verification: `Tools/TestBattleRules.ps1`. A full in-game playtest is
still required to validate presentation and the complete encounter flow.
