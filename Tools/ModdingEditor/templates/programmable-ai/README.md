# AI Dojo

Enable this mod and visit the **AI Dojo** map page using the
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
The candidate list also obeys native `tactic_conditions` and `tactic_distance`
gates, so a callback cannot choose a move outside its authored AI range.
For authored moves with long sounds, schedule `stop_sound` by exact `core_sound`
at `Hit` and `AnimationEnd` so native playback ends on interruption.
`event.frame` and `event.seconds` advance with the live fight even when a
fighter has not pressed a key; use them for decision delays and cooldowns.

When you register a complete replacement for an existing native move, call
`sf2.moves.patch { move = "ExactNativeName", disable = true }` during mod loading
and give the fighter a tactic whose `on_decide` returns the authored move from
`event.actions` when it is available. A disabled move remains in the native
catalog but cannot be selected. The replacement still needs its own animation,
conditions, attack intervals and assets. Use guarded
`input = { expected = "Super", value = "RaidCharge" }` and
`priority = { expected = 1000, value = 200 }` patches when the original move's
child animations must keep referring to its native name.
For an archived interval timing difference, use
`interval_start = { name = "Uninterrupt", expected = 9, value = 0 }` alongside
the input patch; the runtime checks the old bound and restores it on unload.
Use `animation = { expected = "old_clip.bytes", value = sf2.assets.binary("animations/new_clip") }`
when the original native move must play a packaged replacement clip. Ship
`assets/animations/new_clip.bytes` and verify its nodes and frame timings in a fight.
Use `remove_interval = { name = "Evade", type = "Invulnerable", start = 0, ["end"] = 47 }`
only after verifying that exact interval is absent from the intended move.
See the [move API reference](https://dawc17.github.io/ProjectEclipse/api/moves-and-tactics/)
for `WaspFly` and `Earthquake` hit reactions, native `tactic_conditions`,
projectiles equipped from an `item` handle, and the exclusive
`ignores_all_invulnerable`/`ignores_invulnerable` options.
For a timed child phase, register its move first and add a caster action such as
`timeline = { [17] = { play_animation = child_move, player = "Child", child_name = "BlackHand" } }`.
The child must exist and be able to select that move when the native action runs.

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
