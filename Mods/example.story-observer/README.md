# Story Observer

Enable this mod with Eclipse API 0.37 or later, then make a normal shop purchase
and complete an enchantment. Unity's Console/player log should contain one
`Story Observer purchase:` or `Story Observer enchantment:` message per native
notification, with qualified item/recipe identities where registered.

This is a diagnostic example with no visual overlay. Purchase cancellation and
merely opening the forge should not emit completion messages. Disable the mod and
repeat: no new observer messages should appear. No state, rewards or economic
behavior is changed. Full-game acceptance is still pending; the shipped Lua is
covered by the runtime fixture.

Gain a level through experience: expect `Story Observer level: old -> new` after
experience processing. A multi-level gain emits one message with the final level.
Loading a profile, gaining insufficient experience or staying at the level cap
must not emit a level-up message.

Enter map, shop, profile, dojo or fight: expect `Story Observer scene: name` after
native initialization and a deferred frame. Loader/preloader/credits are excluded.
Returning to a scene emits again; rapid navigation must not deliver a pending
notification for the scene that was abandoned.


Grant an unowned reward item or add to a consumable stack: expect
`Story Observer acquired: item before -> after (gained delta)`. A native delivery
completion that raises count from zero to one also reports acquisition. Upgrade-only
deliveries and repeated completion do not. An immediate purchase may log both
purchase and acquisition; these are distinct notifications, not two rewards.

Counts belong to the specific operation. If native callbacks perform nested grants,
the inner operation can log before the outer one, and callback-time inventory may
already have changed again. Direct inventory edits and profile loading are outside
the acquisition hooks. The log does not certify a completed disk save.

To combine the reward and event tests, enable `example.eclipse-reward` alongside
this mod and follow that example's README on a profile without Monk's Katars.
Expect its acquisition message if the native reward reaches the grant routine.
Full-game reward granting/persistence remains pending; both examples have automated
Lua checks, and native acquisition fixtures use controlled host services.

With API 0.40 the observer also logs battle_result (fight ID, outcome and Eclipse
state). Launch a normal or Eclipse encounter from the map and finish or surrender.
Expect one battle line per tracked result. This adds no UI and certifies neither
lottery settlement nor disk-save completion. The payload also supports captured
player equipment when native model parameters are available; see the story guide.
