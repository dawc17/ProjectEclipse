# Three-hit Objective

Enable this mod and enter Act I tournament battle 3 in normal or Eclipse mode.
Land three positive, unblocked hits within ten seconds of active simulation to
win the round. Failing the objective loses the round. Pause stops its clock;
each new round starts a fresh objective. Normal knockout, native timeout, native
end rules and surrender retain precedence. The usual rounds-to-win, opponent
sequence, result and reward flow remain in charge of the complete fight.

The behavior uses round-local typed state, tick/damage observations, a HUD and
`fighter:end_round`. Its attached rule explicitly claims `controls_outcome` and
the manifest declares `combat.round_outcome`. Conflicting controllers are
rejected; disable examples that patch the same fight when trying this mod.

See the [round outcome reference](../../Docs/Modding/src/content/docs/api/round-outcomes.md).
Managed production-Lua/native-method checks and isolated Unity 6.6 Play Mode HUD,
font, rendered pixels and teardown checks pass. Models, contacts, simulation clock,
end presentation and settlement are controlled. Full-game contacts, animations and reward/save acceptance
remain to be verified.
