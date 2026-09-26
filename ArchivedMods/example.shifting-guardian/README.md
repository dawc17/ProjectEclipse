# Shifting Guardian

Experimental character-form showcase. Enable this mod, Apply & Restart,
then select **Shifting Guardian** using the map's bottom zone dots and press FIGHT.
This is a separate encounter, not the third Act 1 tournament fight.

Wait three seconds: the staff fighter requests a baton fighter. The native-style
HUD distinguishes queued, applied and failed results. On success, check the weapon,
name, continued combat, unchanged timer and retained health percentage. Test pausing
before the change, losing/ending the round early, and replaying. The HUD should close
when the round ends. A rejected/failed message is a failed test, not proof of a swap.

Ongoing perk cooldown flags and numeric/text variables now follow the fighter
through a form change, keeping their current values and remaining timers.
Automated continuity checks run with `python3 Tools/TestCharacterForms.py` from
the repository root, or the matching PowerShell fixtures under `Tools/`.

The example has empty rewards. The isolated Unity acceptance check verifies the
staff-to-steel-baton swap at frame 180, retained health/variables, applied HUD state
and 120 further animated combat frames without a timer reset or combat exception.
Run it with `python3 Tools/TestCharacterForms.py --native` from the repository root.
Other transformation cases remain experimental. Active stolen magic and other
unresolved effect references may reject a form change. Report the displayed error.
