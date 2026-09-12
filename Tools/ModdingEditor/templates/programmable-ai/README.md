# AI Dojo

Requires API 0.22. Enable this mod and visit the **AI Dojo** map page using the
bottom page dots. Three different native fighters demonstrate programmable AI:

1. Patient Gatekeeper prefers a high kick, then waits 1.5 simulation seconds.
2. Footwork Sentinel chooses a backward/forward step according to distance.
3. Alternating Warden alternates legal high/low kicks, spaced by 0.6 seconds.

If a preferred action is unavailable, each uses the Standard native tactic.
Win to advance; losing retries the same opponent. The complete run repeats.
These are Lua decisions over legal moves, not new animations or imported art.
Native move conditions and attack interruption rules still apply. Editing
`on_decide` changes behavior without authoring another XML table.

Test each distinct behavior in combat, including pause/resume, knockdowns and
weapon loss. The Lua fixture checks ownership, state, stale actions and bounded
execution; those checks do not establish full-game visual or combat acceptance.