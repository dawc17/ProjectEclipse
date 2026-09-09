# Phase 2 integrated showcase

API 0.6 implementation is ready for manual testing; gameplay acceptance is pending.
Restart Play Mode with `Mods/example.phase2` enabled. Phase 1 is not a dependency.

## What to test

1. **Forge:** four recipes should appear. New orders finish instantly; normal Simple recipe materials still apply.
2. **Measured Resolve / Quick Resolve:** after three / two unblocked damaging hits, receive 20% / 10% magic charge. Counters reset each round. Avoid testing with an already full magic meter.
3. **Opening Ward:** halves the first damaging hit each round. The showcase opponent also uses this perk; logs identify the fighter side.
4. **Veteran Guard:** blocking heals 0.02 life units and grants 25% damage reduction for 180 combat frames (three combat seconds). Repeated activations refresh the shield. Its activation count survives saving and restarting; check the log. There is no new buff icon.
5. **Phase 2 Trials / Open Training:** win the one-round, 99-second fight to receive a Trial Ticket. Repeat for more tickets. Tickets are hidden from the shop and demonstrate item rewards; Volcano does not require them.
6. **Ascension Trial:** complete three fights. Entry is free; this sample does not charge Trial Tickets for Ascension. The panel advances from 1/3 to 3/3 and progress survives saving/restarting between fights. Loss or surrender resets the sequence. The final reward grants the five Monk equipment pieces. The sequence is repeatable; item-set metadata does not create a separate shop tab.
7. **Underworld toggle / Phase 2 Offline Depths / Volcano:** one boss (Volcano), one round, **300 seconds**, and **ten health bars**. Entry is free and infinitely replayable. The raid panel shows the boss name, challenge acceptance text and gem reward, with no stage count or difficulty meter. The enemy uses the blue bar and starts with a counter of **x 10**; crossing a full bar decreases the counter without ending the battle. Excess damage carries into the next bar. Exhaust the entire pool to win **25 gems**; losing, surrendering or timing out gives no gem reward. A timeout is a loss even if you have more health remaining. Each new attempt starts with the full boss pool. Only mod-registered raid pages appear; the legacy base raid page is no longer loaded.
8. **Raid dialogue:** completion dialogue runs after this boss fight, including a loss. The new `offline_raid_boss` identity leaves old two-boss sample progress untouched; Trial Tickets are no longer required for this encounter. No save deletion is needed.
9. **Regressions:** open the shop/profile, run a Phase 1 or ordinary fight, and check forge/result navigation.

Training and Ascension use the recovered battlefield background. Volcano uses the recovered `vulcan_raid` arena, portrait, map artwork and equipment; its sample balance remains ten bars, 300 seconds and 25 gems.
The opponent uses ordinary combat plus Opening Ward; this sample does not add a custom movement routine.
Activation icons are not implemented for these Lua behaviors; use the logs and health/magic changes to verify activation.
Many external-service surfaces were already absent in the reconstruction, so the service gates may have no visible effect in this sample.

## API design

See [the API 0.6 contract](../P2_API.md) for supported hooks, state lifetimes,
migrations, temporary shields, timers, feature gates, schedules, and mode progress.
Stateful callbacks use `self.params` and `self.state` with automatic typed persistence.
Reusable Lua functions/modules provide composition; procedural behavior stays in Lua.

## Verification and bug reports

Managed builds and isolated runtime fixtures do not replace a game playthrough.
For failures, report the recipe or mode and step, win/loss, whether you restarted,
and the first complete Console exception plus a screenshot of the affected screen.
