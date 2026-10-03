# Adjustable PvP balance

The supported JSON contract and editing workflow are in the
[public profile guide](../Modding/src/content/docs/guides/pvp-balance.md).
This implementation is scoped to `LocalVersusMatch`, including online,
training, replay and spectator modes. It does not impose downstream DE policy
on recovered campaign data.

## Ownership and hooks

- `Eclipse.Runtime` owns the Unity-independent typed JSON parser, validation,
  immutable snapshots, SHA256 gameplay fingerprint, override resolution and
  recoverable-health math in `Runtime/PvpBalance.cs`. It references no recovered
  game types. The existing Newtonsoft package provides strict JSON parsing.
- `PvpBalanceProfiles` loads bundled Resources and local presets. Historical
  snapshots remain addressable by hash for the duration of the process, so
  editing a preset does not invalidate an already agreed lobby/match.
  `LocalVersusSettings.Balance` captures an immutable snapshot. Reseeding for a
  rematch retains it; changes require a new lobby match or training restart.
- `PvpBalanceCombat` resolves source category, source equipment and current
  attacking animation. `Model.GetTotalDamage` applies resolved damage scales
  before `ResolveStrikeDamage`; the early block clamp prevents the native
  overkill margin from classifying chip as lethal. `Fight.OnModelHit` clamps
  again immediately before health application, after defense. Pool creation
  and recovery use actual health lost, never attempted damage.
- `ModelParameters.RecoverableLife` is copied by the existing clone and included
  by the production rollback graph policy. The native health setter clamps it
  to missing life and clears it at death/full health. Recovery records the
  pool before calling that setter, then spends only health actually restored;
  otherwise the setter's own clamp could double-spend the pool.
- `VersusStateHash` includes the pool and reports its exact float bits on
  divergence. UI components only read simulation health in `LateUpdate`.
- `PvpRecoverableBar` clones the native skewed live bar behind it, preserving
  sprite, geometry and mirroring. Re-initialization reuses one clone and binds
  the new fighter. The clone has no raycast target or tick interpolation.
  The recovery clone retains a full-width mesh; the shader draws only the
  interval from the visible interpolated live edge to current plus recoverable
  health. It derives progress from the sprite atlas UV bounds, respecting
  right-origin fills and mirrored parents without depending on canvas position. Transparent live artwork therefore
  has no grey texture underneath it. The Resources UI shader removes the recovered texture hue while preserving
  alpha, stencil masking and clipping. Each bar owns and disposes its material.
  No serialized sprite mesh or existing asset GUID changes are needed.
- `PvpBalanceEditor` provides JSON authoring and preview under
  `SF2/Multiplayer/PvP Balance`, including local overrides and training restart.
  Local JSON can override a bundled ID; the editor refuses to select stale
  bundled edits hidden by that local override.

The newly used recovered identifiers `MaxLife`, `GetFightDefinition` and
`GetDamageAttributes` are marked `// best guess for name` at their declarations.
Necessary callers and fixture stubs use the same names. They are inferred names,
not confirmed deobfuscation mappings. No serialized references to the old tokens
were found in `.asset`, `.prefab` or `.unity` files.

## Network and recording compatibility

Netplay protocol 4 adds the profile hash/name to lobby state, the hash to guest
readiness and the hash to match starts. A stale acknowledgement cannot ready a
guest for new rules. Guests require locally available identical gameplay rules;
the opponent does not transfer JSON automatically. Identity/content fingerprints
retain the engine balance version separately from the selectable profile hash.

Room protocol 5 changes its nested streamed match start. Rebuild the room server
and use matching clients. Each paired fight's host selects its own local preset;
there is no server-wide room preset. Spectators require the matching rules and
capture them in their local simulation settings.

Replay format 3 stores full balance JSON and its hash in the bounded header.
Playback validates their agreement and captures those rules independently of
the current selection. Legacy formats 1/2 remain readable, but their playback is
refused because they predate this balance engine and grey-health hash state.

## Verification

Run the focused checks from the repository root:

```powershell
pwsh -NoProfile -File Tools/Tests/Combat/TestPvpBalance.ps1
pwsh -NoProfile -File Tools/Tests/Combat/TestLocalVersusRules.ps1
pwsh -NoProfile -File Tools/Tests/Combat/TestPvpHealthRuntime.ps1
dotnet run --project Tools/NetplayTests/NetplayTests.csproj -c Release --verbosity quiet
pwsh -NoProfile -File Tools/Tests/Presentation/TestPvpHealthBarNative.ps1
pwsh -NoProfile -File Tools/Tests/Progression/TestUnderworldRuntime.ps1
python Tools/Audits/AuditUnderworld.py
```

The managed health runners accept `-AssemblyDirectory` and
`-UnityManagedDirectory` when compiled DLLs/editor references are outside the
default `Temp/Bin/Debug` path. The pure rules runners require .NET 10 and the
project's imported Newtonsoft package. Netplay tests require .NET 8 or newer.
Native HUD validation uses the matching Unity editor in an isolated project
with graphics enabled; it does not control the open project editor.

On October 3, 2026, the pure rules/bridge suite passed 1051 checks, settings
passed 50, compiled native health/rollback passed 208, Underworld runtime passed
1282, and the network core passed 55234. The native HUD fixture passed 16 checks
and verifies pixels,
skew geometry, mirroring, pool updates, binding replacement and cleanup with
controlled health and atlas-base dependencies. These checks do not prove a
complete online fight, physical-device behavior or editor authoring ergonomics.
Unity's isolated editor also logged an unrelated SearchDatabase indexing
exception at shutdown; the fixture completed its assertions and exited zero.

The standard generated MSBuild projects still refer to unavailable SDK/reference
locations in this environment. All four production assemblies were compiled
using the matching Unity 6000.6.0f1 compiler and its Bee response files, with
new sources and dependency references included. Underworld's static audit still
reports the existing missing `fungus_raid` `/layer_0_2` asset; the balance change
does not modify those location assets. Full multiplayer gameplay acceptance
and tuning the default recovery rates remain playtest work.

## Recoverable segment presentation correction

The original grey underlay extended from zero to current plus recoverable life.
Translucent parts of the live sprite exposed that underlay, creating duplicate
artwork. The replacement clips both segment boundaries in the shader while
sampling the original full-width sprite. It starts at the displayed live fill,
after the native presentation smoother, so damage/healing animation keeps the
segments contiguous. No simulation, balance JSON or wire-format change is needed.
The native fixture checks translucent live pixels, both fill origins, mirrored
parents and delayed damage presentation as well as the original HUD checks.

The corrected presentation passed 19 native HUD checks, all four matching Unity
managed assembly compiles and 1282 Underworld runtime assertions. The wiki build
and internal link checks passed. The static asset audit still reports the
pre-existing missing `fungus_raid` `/layer_0_2`; no location assets were edited.
A full fight playtest with recovered HUD artwork was not performed.

## Positioned HUD and masking regression

The first segment shader used vertex positions as though they were local to
one image. UGUI canvas batching transforms those positions; at a translated
HUD location the shader clipped away every recoverable pixel. A new native
regression reproduced this before the fix: 4787 grey pixels at the canvas origin
became zero after moving both bars together.

The shader now derives horizontal progress from the same outer sprite UV bounds
that UGUI uses for horizontal fills. These coordinates survive canvas placement,
native skew and scaling. The component also updates UGUI's cached stencil
material when a parent mask creates one, so later health changes remain visible.
The fixture uses an atlas subrectangle, real ViewerFight bar dimensions/skew
and host/guest offsets, plus translated bars and changing masked segments.

The corrected shader/component passed 24 native HUD checks and all four Unity
managed assembly compiles. Underworld runtime passed 1282 assertions. These are
controlled renderer tests using the production skew, shader and component with
HUD layout values; a full multiplayer fight with recovered artwork was not run.
Restart Play Mode to rebuild active fight UI components after updating.

## Recoverable segment outline and fade

The recoverable-health shader now keeps a translucent center (32% opacity),
stronger top/bottom outlines with a smooth inward falloff, and a fade at both
segment ends. The outline occupies the outer 12% of bar height; the end fade
uses 1.2% of full bar width, capped at 20% of the current recovery segment so
small pools remain visible. These are presentation-only shader properties and
do not change balance JSON, simulation or network compatibility.

The expanded native fixture passed 30 checks, including center/outline contrast
and both end fades for left/right fills, in addition to the positioned/masked
HUD regressions. Underworld runtime passed 1282 assertions. The static asset
audit still reports the existing missing `fungus_raid` `/layer_0_2`; no location
assets changed. Native rendering used controlled artwork and the production
shader/skew; a full multiplayer game playtest was not performed.
