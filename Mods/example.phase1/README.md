# Phase 1 integrated showcase

`example.phase1` is an acceptance/showcase mod for the public API surface available before P2A.
It deliberately uses only `require("sf2")` and public Lua functions. There is no raw XML,
test-only bridge, recovered C# object, or DE-specific branch.

The connected path is:

`state + localization -> P1C item/perk/forge -> P1D location -> P1A zone/battle/fight/reward -> P1B quests`

P1D move/template/trigger/tactic registration is also exercised. The tactic references the custom
move through the opaque move handle, and the P1A warrior consumes the opaque tactic handle directly.
`sf2.tactics.name(handle)` is available only when an explicit recovered runtime name is genuinely
needed; normal authoring does not need to manufacture qualified tactic IDs.

The forge fixture borrows `sf2.forge.profile("Simple")`; it does not define or mutate shared forge
prices, currencies, delivery timers, skip prices, upgrade tables, or any other core economy field.

Run `Tools/TestPhase1Showcase.ps1` for the focused static contract and
`Tools/TestPhase1ShowcaseRuntime.ps1` for real discovery, MoonSharp execution, transactional commit,
duplicate rollback, and teardown validation.
