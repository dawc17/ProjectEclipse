# Follow-up hitch analysis — 2026-09-19

Capture: `ProfilerCaptures/eclipse_2026-09-19_16-30-07.data` (390,157,756 bytes). Unity exposes 2,000 frames, including after reloading with a larger history capacity. The file appears to contain the tail of the longer recording, not 22,000 saved frames.

## Findings

- The saved interval spans 7.915 seconds. Median frame: 2.809 ms; 95th percentile: 3.679 ms; 99th percentile: 14.059 ms.
- The previous repeated 2 MiB callback allocations and periodic combat-tick collections are absent. The callback regression benchmark still passes: 968 bytes per warm call versus roughly 2.1 MiB for a fresh coroutine.
- Frame 1998 takes 1,167.874 ms. `ScreenFight.Update` accounts for 1,092.166 ms, including eight `GC.Collect` samples totalling 860.971 ms. About 84.5 million bytes are allocated directly below that callback. Its screen-completion event enters `Fight.NextRound`, which forces collection before and after setup; tactics loading also forces collection after each archive.
- Other periodic frames take roughly 54–59 ms almost entirely under childless `EditorLoop` samples. These cannot be attributed to a particular editor subsystem from this capture. Frame 1999 is also dominated by EditorLoop. Do not mistake these samples for game simulation cost.
- Incremental GC was disabled in both project settings and the running editor.

## Changes

- Removed the two synchronous full collections in `Fight.NextRound`.
- Removed per-archive synchronous full collections in both tactics-loader overloads in `TacticsArchiver`.
- Enabled `gcIncremental` in `ProjectSettings/ProjectSettings.asset`. Automatic collection remains enabled. No game data, AI tables, or combat rules were changed.
- Kept the PlayerSettings file change scoped to that flag rather than including the current editor's unrelated format migration. The intermediate editor serialization is retained under ignored `Logs/`.

## Validation and limits

Managed Assembly-CSharp compilation and native Unity recompilation passed. The existing callback regression passed all 15 checks, including budget/error handling and allocation reduction. Scoped whitespace checks passed.

A four-second editor-only diagnostic was taken outside Play mode. Excluding the diagnostic save itself, it showed no non-idle frames over 8 ms, so it did not reproduce the periodic in-game EditorLoop stalls. The original saved capture and profiling mode were restored; diagnostic files are ignored under `Logs/`.

The running editor reported incremental GC inactive after changing the project setting. Restart Unity before evaluating that setting's effect. No post-fix full-round capture has been measured, and tactics decoding still performs work at round transitions. To resolve the separate EditorLoop stalls, record the hitch with **Profile Editor** enabled so those samples have child markers. Check the Profiler history/save range: the provided file contains only 2,000 frames.
