# SF2DE Architecture Handoff

Date: 2026-08-29
Branch: `refactor/sf2de-architecture`

## Current state

The architecture/refactor pass is in a very good state and has been runtime-smoke-tested in Unity/gameplay. The latest batch was confirmed working by the user after importing and playing it.

The central rule of this branch is now mostly achieved:

- recovered/decompiled classes stay as thin integration hosts;
- project-owned behavior lives under `Assets/Scripts/SF2DE/`;
- Unity serialized identity stays in recovered components/prefabs;
- compatibility policy is separated from filesystem/Unity orchestration;
- fixed-step gameplay logic is not being rewritten just to make recovered code prettier.

Do not restart this work as a broad rewrite. Preserve the current seams.

## Important checkpoints

Recent architecture commits, newest first before this handoff:

- `63ad1f0e` Namespace gameplay content archive
- `eae7a769` Extract content override path policy
- `87a30c8e` Tidy Underworld fight adapter formatting
- `3ae079ff` Tidy desktop settings adapter formatting
- `161656c1` Move camera interpolation state into SF2DE
- `81889d60` Extract Underworld map battle presentation
- `77d03c3c` Move effect follow diagnostics into SF2DE
- `30c08d40` Move Underworld raid diagnostics into SF2DE
- `ab77456d` Extract Underworld raid life bar presentation
- `4f6d81d3` Extract content override validation
- `d538ade7` Extract item list compatibility transforms
- `b8f0b0b7` Extract move compatibility transforms
- `6cfa331f` Extract quest compatibility transforms
- `129db4e5` Extract internal settings compatibility
- `00e34849` Extract stage compatibility transforms
- `d657c382` Centralize Underworld compatibility policy
- `91a5c4f8` Move Underworld raid HUD into SF2DE
- `5c1aa91a` Centralize fight presentation interpolation
- `11d463ec` Extract desktop render settings controls
- `26d2b77d` Extract fight gamepad input
- `49ebf56e` Extract desktop top bar layout
- `322b7029` Extract Underworld map controls

There are also small project-regeneration commits between extraction batches. Unity reorders `<Compile>` items in generated `.csproj` files; compare compile-item sets before treating those diffs as meaningful.

## Content architecture

`ResourceManager` has reached the intended boundary. Keep it as the recovered integration/orchestration layer.

Project-owned content policy is now under `Assets/Scripts/SF2DE/Content/`:

- `ContentOverrideCompatibility.cs`
- `ContentOverridePaths.cs`
- `GameplayContentArchive.cs`
- `InternalSettingsCompatibility.cs`
- `ItemListCompatibility.cs`
- `MoveCompatibility.cs`
- `QuestCompatibility.cs`
- `StageCompatibility.cs`

Underworld-specific stage compatibility remains under `Assets/Scripts/SF2DE/Underworld/Content/`.

`ResourceManager` should continue to own things that actually belong to a loader:

- filesystem access and `File.Exists`/`Directory.Exists`;
- `ResourcesAndBundles` access;
- reading/parsing source files;
- one-time logging;
- runtime model-fallback dictionary storage;
- request routing and adaptation orchestration.

Do not move those out merely to reduce its line count.

## Other boundaries already established

### Underworld

Owned Underworld behavior is split under `Assets/Scripts/SF2DE/Underworld/`.

Recovered classes such as `Fight`, `MapPanel`, `MapScene`, `ZoneScrollItem`, `ScreenModel`, `PlayerLifeBar`, and `GameUtils` should contain small adapter calls only where practical.

Important extracted pieces include:

- zone/battle compatibility policy;
- map controls;
- map battle visibility/power-mode presentation;
- raid HUD/shield UI;
- raid life-bar style and segment-transition state;
- raid diagnostics;
- stage-content adaptation.

Do not extract the whole `MapScene` mode/selection state machine unless a real maintainability problem demands it. It is tightly coupled to recovered map containers and selection APIs and is currently a reasonable host.

### Rendering/interpolation

Owned interpolation code is under `Assets/Scripts/SF2DE/Rendering/Interpolation/`.

The recovered `Camera`, effect/render classes, etc. should keep fixed-step simulation and recovered rendering calls. Owned helpers may hold presentation interpolation state and sampling logic.

`FightCameraInterpolation` now owns camera interpolation scratch state and zoom history. Do not move camera simulation, quake lifetime, zoom progression, or recovered `Render` behavior into it.

Effect follow diagnostics are under `Assets/Scripts/SF2DE/Rendering/Diagnostics/`.

### Desktop/UI/input

Desktop settings controls, top-bar layout, and fight gamepad input have already been extracted into `SF2DE` ownership. Recovered UI/controller classes should stay thin hosts.

## Validation already performed

During this pass all four managed projects were repeatedly compiled:

```powershell
msbuild SF2DE.Runtime.csproj /nologo /v:quiet /clp:ErrorsOnly
msbuild Assembly-CSharp-firstpass.csproj /nologo /v:quiet /clp:ErrorsOnly
msbuild Assembly-CSharp.csproj /nologo /v:quiet /clp:ErrorsOnly
msbuild Assembly-CSharp-Editor.csproj /nologo /v:quiet /clp:ErrorsOnly
```

`git diff --check` was kept clean.

Underworld audit remained:

```text
battles: 76
issues: []
```

Focused smoke tests were also run for:

- move compatibility merge/pruning;
- item aliases and model fallbacks;
- content override validation;
- content override path classification/candidate ordering;
- raid life-bar segment transitions including multi-segment damage, healing, death, and carry-over damage;
- Underworld normal/power mode battle filtering;
- raid map sprite and icon fallbacks;
- camera interpolation zoom history and position sampling.

Most importantly, the user then performed an actual Unity/gameplay smoke test after the latest architecture batch and reported that it works correctly.

## What to do next

### 1. Stop broad extraction for now

This is the most important next decision. The major recovered-file ownership violations have been removed. Further extracting every remaining 5-15 line adapter would make the architecture worse, not better.

Treat the architecture pass as substantially complete unless a concrete subsystem still has project-owned implementation buried in recovered code.

### 2. Turn ad-hoc regression checks into repository tests

Highest-value next engineering task: persist the behavior tests that were run manually during this refactor.

Suggested target:

`Tools/TestCompatibility.ps1`

It should exercise pure helpers without launching Unity where possible:

- `MoveCompatibility`
- `ItemListCompatibility`
- `ContentOverrideCompatibility`
- `ContentOverridePaths`
- `UnderworldMapBattlePresentation`
- `UnderworldRaidLifeBarTransition`
- `FightCameraInterpolation` state behavior where practical

Keep this focused. Do not build a giant test framework just for these helpers.

### 3. Add one architecture note to AGENTS/README after tests exist

Document the final ownership rule succinctly:

> Recovered classes are integration hosts. New reconstruction, compatibility, desktop, Underworld, and presentation behavior belongs under `Assets/Scripts/SF2DE/` unless Unity serialization or assembly constraints require otherwise.

Also document the exceptions: `SF2DE.Runtime` cannot depend on recovered `Assembly-CSharp` types, and some global runtime types may intentionally remain global because of predefined-assembly or Unity component identity constraints.

### 4. Audit the few remaining global types under `Assets/Scripts/SF2DE/`

Current global/project-owned files include at least:

- `Runtime/Offline/OfflineServices.cs`
- `Runtime/Offline/OfflineStoreData.cs`
- `Runtime/Presentation/SF2DisplayFrameRate.cs`
- `Runtime/Presentation/MotionBlur/SF2MotionBlur.cs`

Do **not** namespace these automatically. First inspect callers, predefined-assembly dependencies, Unity serialization/component identity, and any reflection/string lookups. They may be intentionally global.

`GameplayContentArchive` was safe to namespace because it is a static utility with only direct code callers and no serialized identity.

### 5. Then return to reconstruction/fidelity work

Once repeatable regression tests exist, the project should move back toward actual game restoration rather than architecture cleanup.

Good categories to pick from based on current evidence:

- remaining recovered/deobfuscated-name work using the audited `Deobfuscation/` workflow;
- missing gameplay/content compatibility discovered by real playtesting;
- visual/UI fidelity issues visible in Unity rather than speculative cleanup;
- offline Underworld completeness and edge cases;
- desktop-specific polish/input/render behavior.

Pick the next task from an observed defect or missing feature. Avoid rewriting working recovered systems for aesthetics.

## Resume checklist

When resuming:

1. Read `AGENTS.md` and this file.
2. Confirm branch is `refactor/sf2de-architecture`.
3. Run `git status --short` before editing.
4. Run the four managed builds after C# changes.
5. Run `python Tools/AuditUnderworld.py` for Underworld/content work.
6. Use Unity 2022.3.62f3 for runtime validation.
7. Preserve `.meta` GUIDs and serialized recovered components.
8. Commit narrow rollback checkpoints.

## Current recommendation

Do **not** begin another architectural rewrite tomorrow. First make the compatibility/refactor smoke checks repeatable, then continue actual Shadow Fight 2 reconstruction from concrete runtime gaps.
