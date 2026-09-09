# P2A combat showcase: Measured Resolve

First P2A implementation slice; gameplay acceptance is pending. Phase 1 remains
accepted. This mod adds two weapon forge families using the base Simple economy:

- **P2A Measured Resolve:** every third unblocked damaging hit grants 20% magic.
- **P2A Quick Resolve:** every second unblocked damaging hit grants 10% magic.

Restart Play Mode, open the unlocked forge, apply one recipe to a weapon, equip
that weapon and a magic item, then enter an ordinary fight. Let the opponent hit
you without blocking. The qualifying hit should add charge on top of the normal
hit recharge. Use/spend magic so a full meter does not conceal the effect. The
console also records each activation. Test both variants, blocked hits, and a
new fight resetting a partly filled hit counter. Avoid the Phase 1 fight's
full-magic rule when checking the meter.

One behavior implements both variants using typed `hits_required` and `charge`
parameters. Lua performs hit filtering, counting, branching and activation.
Counters are local per source item/perk and reset at fight begin; the cumulative
activation total uses the existing durable mod-state contract. No generic action
tables, new XML perk template or raw engine object implement the procedure.

## Implemented callback contract

`on_damage_received(parameters, fighter, event)` receives an isolated snapshot:
`round` (number), `health_before`, `health_after`, `damage` (numbers), `blocked`
and `critical` (booleans). Health values use recovered current-life units;
`damage` is the observed decrease, not raw attack power or total multi-bar damage.

The authoritative seam is the resolved-hit path in `Fight`: `LogDamage`, capture
health, `UpdateLife`, snapshot/callback, hit statistics, then ordinary magic-charge
updates and subsequent recovered hit processing. Only a positive observed health
decrease on the player dispatches. Blocked hits with chip damage are observable;
the sample ignores them. Lethal hits are observable but cannot be reversed with
`change_health` during this callback; the sample ignores them too.

The hook requires the normal FightBegin lifecycle; raid-only flows without that
initialization do not dispatch it. It uses active saved equipment enchantments, forged behavior-backed perks,
and learned player perks, with the same source/rule filtering as FightBegin.
Fighter capabilities expire when each callback finishes. Script exceptions and
instruction limits remain isolated; recursive combat dispatch is suppressed.
Callbacks do not roll back mutations already performed before an error.

This slice does **not** expose opponent behaviors, damage-over-time updates,
before-hit mutation, round/end callbacks, timed effects, or temporary modifiers.
Those remain P2A work. Existing two-argument `on_fight_begin` handlers remain valid.
This development slice currently uses the existing 0.5 API version; declaring
`>=0.5` alone does not guarantee this new callback on older builds.

Checks: `Tools/TestP2ACombatRuntime.ps1` executes this real Lua sample and verifies
both parameter sets, hit filtering, counter reset, callback expiration and error
isolation. `Tools/TestModFightBeginRuntime.ps1` exercises the production dispatcher
and checks source ordering at the hit seam. Managed compilation and isolated Unity
compatibility checks supplement those tests; they do not replace the playtest above.
