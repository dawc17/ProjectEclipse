# Generated Expedition

Requires API 0.22. Visit **Generated Expedition** using the map page dots.
Press Fight to open the original-style setup panel. Toggle a stronger opponent,
adjust round time, then press Begin Encounter. Each encounter chooses one of
three distinct fighters using the saved random stream and applies the chosen
level and time. Three victories finish a repeatable expedition.

Back cancels setup without charging entry or drawing an opponent. A completed
choice is saved before combat starts; reload or retry a failed scene launch
reuses it. Resolving a fight clears that plan and the next encounter gets a new
setup. This sample has no entry cost and no reward grant.

Test pointer, keyboard and controller changes, Back cancellation, repeated Fight
clicks, pause/resume and reload after choosing. An open setup dialog is not saved;
after leaving the map or restarting, press Fight again. Definitions remain fixed;
Lua generates the live encounter's roster and parameters without registering
new fights at runtime.