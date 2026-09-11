---
title: Write a custom battle rule
description: Create a fight mechanic using ordinary Lua and isolated per-round state.
---

A battle rule can run Lua whenever the engine dispatches a supported combat
event. Use a typed rule definition to attach a reusable function to your fight;
put decisions, arithmetic, and counters in that function.

The [rule reference](../../api/rules/#sf2rulesbehavior) implements a guardian that
only takes damage on every third hit. Its `hits` counter belongs to this battle,
not to a weapon or enchantment. You can reuse the behavior with `every = 5` in
another fight, without rewriting the callback.

1. Set `api = ">=0.9 <1.0"` in your manifest.
2. Declare `content.register` and `combat.modify_hit`.
3. Register the behavior and its parameter/state schema.
4. Register a rule referencing the behavior.
5. Add the rule handle to your fight's `rules` array.

The complete [Third Strike Trial source](https://github.com/dawc17/ProjectEclipse/tree/main/Mods/example.battle-rules)
includes the manifest, localization, map entry, opponent, and repeatable fight.
The tracked source in your checkout is authoritative while changes are unpublished.
Enable it, restart from the title screen, and select its map entry in Campaign.
Check that only the third positive incoming hit can damage the guardian and that
the counter resets on the next round. At or below one third health, every hit
can damage it: the example reads a fresh [combat snapshot](../../api/fighter/#fightersnapshot)
to decide when the guard ends. Blocking still follows normal game rules.

Use `on_damage_resolving` to prevent or reduce pending damage. By
`on_damage_received`, the hit has already happened and a lethal result cannot be
reversed. Read [combat callbacks](../../api/combat-callbacks/) and
[fighter methods](../../api/fighter/) before choosing a hook.

Attach two differently parameterized rules to exercise independent state. Try
`target = sf2.rules.ALL` to run the mechanic independently for both fighters.
For a later-round mechanic, use `rounds = { 2, 3 }`; initialize it in
`on_round_begin`, because its filters exclude the first-round fight-begin event.

Automated checks cover registration, callbacks, state isolation, filtering,
serialization fingerprints, and capabilities. Full encounter presentation still
needs an in-game playtest. Custom HUD widgets and arbitrary win conditions are
not part of this API yet.
