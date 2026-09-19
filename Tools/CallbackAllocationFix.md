# Combat callback allocation fix

The capture `ProfilerCaptures/eclipse_2026-09-19_16-09-37.data` contains 2,000 frames over 9.584 seconds. Its median main-thread frame duration is 2.873 ms, but it contains 16 GC collections lasting 123–138 ms each, with a worst total frame of 178.238 ms. Recorded main-thread allocations total 1,173.53 MiB; 1,228,415,412 bytes are directly below `FightScene.FixedUpdate`. Two repeated allocation samples are exactly 1,048,608 bytes each.

`MoonSharpScriptRuntime.RunBounded` previously created a coroutine for every callback. Native inspection confirms that each coroutine owns two 131,072-element reference arrays, matching those two allocations. The capture did not include allocation call stacks, so it cannot independently name the allocating managed method below FixedUpdate.

## Change

The C# runtime now keeps private callback workers suspended between successful calls, reusing their interpreter stacks. Nested callbacks borrow separate workers. At most four idle workers are retained per script context, and disposal clears the pool. Callback function/argument references are cleared after every invocation. Workers are discarded after exceptions, unexpected yields, or instruction-budget exhaustion. Existing instruction-slice limits remain active. No mod Lua files, callback signatures, or public capabilities changed.

## Checks

- Managed Assembly-CSharp build: passed, zero errors.
- Unity editor compilation: passed.
- Native Unity invocation of the production runner: correct results across successive calls, one idle worker retained.
- `pwsh -NoProfile -File Tools/TestCallbackWorker.ps1`: 15 checks passed. Covers reuse, nested calls, multiple/nil returns, rejection of unexpected yields, error recovery, infinite-loop budget enforcement, and allocation regression. This harness extracts the worker and execution methods directly from production source and runs them against the project's MoonSharp assembly on .NET 10.
- Warm callback benchmark: fresh coroutine **2,097,893 bytes/call**, reused worker **968 bytes/call**, **99.95% lower allocation**. This measures callback machinery, not entire fights or editor frame rate.
- `Tools/TestP2ACombatRuntime.ps1`: passed, including public Lua execution, settlement/state/migration, capability lifetimes, and rollback. Its shared source list was repaired to include the existing trial-rule and move-perk-lock sources.
- `Tools/TestModUiLua.ps1`: blocked at a fixture asset classification mismatch (`example.charge-ui:sprites/ui-test` is classified as Texture, expected Sprite). This is not a passing UI-suite result.
- Scoped `git diff --check`: passed.

A new full-round profiler capture has not yet been measured. The observed per-callback allocation cause is fixed and regression-tested; a gameplay capture is still needed to quantify the resulting frame-time improvement and any remaining hotspots. Original capture files were preserved. Analysis scratch files are under ignored `Logs/`, and generated regression builds are under ignored `Temp/`.
