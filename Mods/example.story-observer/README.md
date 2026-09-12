# Story Observer

Enable this mod with Eclipse API 0.30 or later, then make a normal shop purchase
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
