# AI Dojo

Requires API 0.46. Enable this mod and visit the **AI Dojo** map page using the
bottom page dots. Four different native fighters demonstrate programmable AI:

1. Patient Gatekeeper prefers a high kick, then waits 1.5 simulation seconds.
2. Footwork Sentinel chooses a backward/forward step according to distance.
3. Alternating Warden alternates legal high/low kicks, spaced by 0.6 seconds.
4. Reactive Guardian carries a staff and wears green armor. At close range, it
   reacts to your active attack interval by choosing a legal backward movement.
   Otherwise it chooses the shortest nominal non-looping kick-tap candidate.
   It reads active intervals, input controls and timing, without matching move names.

If a preferred action is unavailable, each uses the Standard native tactic.
Win to advance; losing retries the same opponent. The complete run repeats.
These are Lua decisions over legal moves, not new animations or imported art.
Native move conditions and attack interruption rules still apply. Editing
`on_decide` changes behavior without authoring another XML table.

Test each distinct behavior in combat, including pause/resume, knockdowns and
weapon loss. The Lua fixture checks ownership, state, stale actions and bounded
execution; those checks do not establish full-game visual or combat acceptance.

To test the fourth opponent, win the first three encounters. Approach and attack:
backward movement should take priority when available, even during its voluntary
attack pause. Stop attacking and remain close to see quick-kick selection. Move
far away to allow the Standard tactic to approach. Native eligibility, reaction
throttling and interruptions still apply, so a hit is not guaranteed to be dodged.
The old three encounter IDs and order are retained; the fourth is appended.

`Tools/TestModAi.ps1` also runs the compiled native move parser, animation reader
and AI metadata adapter against shipped StepBack, StaffStepBack, HighKick and
LowKick XML/67-node clip bytes. Their real snapshots drive the Reactive Guardian
Lua checks. That test prewarms the native animation cache to bypass Unity resource
I/O; it does not exercise in-fight eligibility, physical playback or rendering.
