# SF2DE Modding API and Asset-Pack System Plan

Status: active architecture and staged implementation plan
Target project: Unity 2022.3.62f3; Windows x86_64 first, Android/IL2CPP retained as a supported target
Source concept: `ResearchSources/newsystem.md`

## 1. Decision summary

Build the system incrementally behind stable interfaces. Start with a loose-folder
development provider and one complete modded weapon. The base game's large file-backed
runtime assets are already centralized behind `PackagedArtCatalog` in standard USTAR
archives wrapped in standard LZ4 Frames (`.tar.lz4`). Reuse that work instead of building
the previously proposed custom `.sfpack` container.

Use MoonSharp as the initial and supported Lua runtime. Keep it behind an
`IModScriptRuntime` boundary so the public API never exposes MoonSharp types and a future
runtime change remains possible without changing mod manifests or Lua APIs. Do not convert
the base game into Lua; Lua is for external mod behavior and narrow extension points where
declarative data is insufficient.

Loose folders remain the canonical authoring form. Complete packaged mods may later use
`.sfmod` as a distribution wrapper, but packaging must not block the first external-mod
vertical slice.

### 1.1 Current implementation baseline

Work completed since the original proposal already covers part of the asset and core
migration layers:

- The project is on Unity 2022.3.62f3.
- `ResourcesAndBundles` remains the recovered loading seam while Eclipse-owned content
  behavior lives under `Assets/Scripts/Eclipse/`. Unqualified vanilla runtime-art requests
  are now implicitly qualified into `core:*` at that seam before falling back to Unity
  `Resources`, so recovered callers do not need thousands of mechanical namespace edits.
- `PackagedArtCatalog` routes core sprites, textures, audio, models, location art and
  immutable location-atlas data through `.tar.lz4` archives.
- TAR descriptors and catalog groups carry a `namespace`/`namespaceId`, currently `core`.
- `ModId`, `AssetId` and `DefinitionId` are implemented as strict immutable contracts.
- Loose `mod.toml` discovery, semantic-version/API ranges, deterministic dependency
  ordering and consolidated diagnostics are implemented. Invalid mods no longer disable
  unrelated valid mods.
- `AssetResolver` enforces one provider per namespace. `LooseModProvider` indexes safe
  files below `<mod>/assets` and `<mod>/scripts`, while `CoreAssetProvider` exposes exact
  `PackagedArtCatalog` addresses as `core:*`. Typed runtime loading now delegates through
  the owning provider, so `ModAssetLoader` no longer contains a hard-coded
  `core -> PackagedArtCatalog` storage special case.
- `ModHost` composes discovery, dependency resolution and providers, and the Unity editor
  has `SF2/Modding/Validate Loose Mods` for a consolidated report.
- The automated Unity packaged-art fixture proves one valid loose mod can stay mounted
  alongside `core:*` while a deliberately broken independent mod is disabled. This meets
  the Phase 1 diagnostic-resolution exit criterion.
- `ModAssetLoader` now provides the first typed external pipeline: PNG textures plus typed
  `.asset` sprite descriptors (with legacy PNG/`.sprite.toml` compatibility), strict UTF-8
  model/text payloads, and PCM16 WAV audio. Core requests delegate to the existing packaged
  catalog instead of duplicating decoders.
- Explicit qualified paths are routed through `ModRuntime`. Ordinary unqualified recovered
  runtime-art paths enter the same resolver as implicit `core:*` requests, then retain
  `Resources` as compatibility fallback for content not owned by the core art provider.
- MoonSharp 3.0.0-beta.1 is pinned from the official repository at commit
  `0fb8ba9106c44b140b8f56cb44cb1b50b358897c` using its `/interpreter` UPM package.
- `IModScriptRuntime`, `IModScriptContext` and `ModApiFacade` keep MoonSharp types out of the
  public runtime contracts. `MoonSharpScriptRuntime` uses `Preset_HardSandbox` and supplies a
  project-owned `require` instead of enabling MoonSharp filesystem load methods.
- Loose `scripts/*.lua` and `localizations/*.toml` are mounted through the same namespaced
  provider. `require("sf2")` exposes `sf2.mod`, `sf2.log`, typed
  `sf2.assets.sprite/model`, `sf2.localization.key`, transactional registration for weapon,
  armor, helm, ranged and magic, `sf2.price.coins/gems`, and `sf2.shop.addItem` with all five
  matching shop sections. The earlier `sf2.mod.log/warn/error` and `sf2.shop.add` names remain
  compatibility aliases. Local module `require` is restricted to the calling mod namespace.
- `ModScriptSession` executes entrypoints in resolved dependency order, retains successful
  contexts for future callbacks, and isolates a failing script without disabling independent
  mods. An attempted `require("../escape")` is rejected as `SCRIPT001`.
- `ModContentCatalog` and per-mod `ModRegistrationTransaction` stage localization, all five
  primary equipment categories, and shop definitions until an entrypoint finishes successfully. Registry collisions, invalid
  references, missing English localization fallbacks and post-registration Lua failures roll
  back without leaking partial definitions. Successful registries are frozen after startup.
- `LegacyContentAdapter` adapts committed external equipment/shop definitions into the recovered
  `ItemInfo`/`Items` runtime without exposing recovered types to Lua. Its one narrow item seam
  now serves weapon, armor, helm, ranged, and magic and preserves the recovered parser as the
  authority for defaults and upgrade-template semantics. Normal external equipment no longer
  accepts arbitrary raw attack/defense values: the shop listing's starting level selects the
  canonical vanilla category milestone and the existing `*_Bonus` table drives later upgrades.
  The tracked `example.weapon` remains the minimal vertical slice, while `example.loadout`
  registers visible armor, helm, ranged, and magic examples using matching core-owned TAR-backed
  atlas sprites and models.
- Mod-aware save loading now preserves unavailable external item XML as opaque orphan data,
  excludes missing items from active inventory/delivery processing, uses default equipment only
  in a temporary model view, and restores ownership when the mod returns. The save also records
  additive `EclipseMods` mod/version activity metadata plus a deterministic SHA-256 content-set
  fingerprint over active mod IDs/versions and committed registry content. Item aliases and
  tombstones now provide a non-destructive ID-evolution layer: historical save IDs may resolve to
  a current same-mod/same-category item without rewriting the XML, while retired IDs remain
  unavailable and reserved. Versioned migrations remain future work.
- `CoreContentImporter` now projects all five primary vanilla equipment categories into the same
  registries used by external content while keeping `Assets/vanillaXml/list.xml` authoritative:
  210 weapons, 179 armor, 193 helms, 85 ranged items and 73 magic items (740 source rows total).
  Qualified `core:items/...` lookups resolve back to the exact existing legacy `ItemInfo`,
  including deterministic disambiguation of the duplicate vanilla `GlaivebowArrow` ranged rows.
- The expanded Unity editor fixture now passes 124 checks with MoonSharp, implicit and explicit
  core routing, TAR-backed core models, loose assets, transactional registration for all five
  equipment categories, vanilla-derived starting power, item aliases/tombstones, script isolation
  and legacy loading together. The earlier standalone
  fixture remains useful evidence, but the latest multi-category expansion has not yet been
  rerun as a standalone player and should not be claimed at the editor's 124-check count.
- Runtime decoders already exist for PNG-backed sprites (including custom geometry),
  PCM16 WAV audio, model XML and location atlas/plist data.
- Generic gameplay/config XML deliberately remains outside the TAR provider and stays
  loose/moddable. Location `params.xml` remains config as well.
- `Tools/AssetPacker` already provides deterministic pack/list/verify/extract/info tooling
  for asset archives.
- The current TAR/LZ4 implementation decompresses an owning archive to a persistent TAR
  cache on first use. That is acceptable for the current migration, but it does not meet
  the eventual on-demand/chunked performance goals in this document.

Phase 2 still needs proper asset handles/scopes and preload/lifetime policy, but provider-owned
runtime loading and the main legacy seam are functional. Phase 3 has a working editor/player
MoonSharp path, hard sandbox, virtual local `require`, bounded entrypoint execution and per-mod
failure isolation. Phase 4 now spans all five primary equipment categories through the recovered
runtime, and Phase 5 has its first orphan-safe save/load implementation plus content-set hashes.
Core gameplay migration has also begun in earnest: all primary vanilla equipment has stable
registry identity while the legacy XML/parser remains authoritative. The immediate priorities
are finishing real upgrade/equip/fight parity across the equipment examples, versioned save
migrations, then perks/enchantments and deeper combat content. A real player build has
already been manually started with save data containing modded equipment without breaking startup.

## 2. Goals

- Load mod content without rebuilding the Unity player.
- Give every mod a collision-free namespace.
- Support sprites, model XML, animation data, audio, localization, Lua, and arbitrary
  text/binary data; add video and fonts only when required.
- Permit loose development mods and deterministic packaged releases.
- Keep the original `Resources` and Unity AssetBundle paths working during migration.
- Make registration transactional: a broken mod is disabled without partially changing
  global game state.
- Provide actionable diagnostics that identify the mod, script, asset, and source line.
- Keep Unity object creation on the main thread while allowing file IO and decompression
  off-thread.
- Support old saves when mods are removed or upgraded.
- Make the base game representable as the reserved `core` mod eventually.

### 2.1 What "core as a mod" means

Treat three concerns as separate contracts:

1. **Ownership/identity**: every runtime asset or gameplay definition belongs to a namespace.
   Vanilla content belongs to reserved namespace `core`; external content belongs to its manifest ID.
2. **Storage/provider**: ownership does not prescribe a filesystem layout. Today `core` runtime art is
   read by `CoreAssetProvider` through `PackagedArtCatalog` and TAR/LZ4, while a loose external mod is
   read from `Mods/<id>`. A future packaged provider may use a different physical format without
   changing callers or IDs.
3. **Public replacement policy**: being addressable as `core:*` does not automatically make an asset a
   stable or replaceable Mod API surface. External mods cannot mount the `core` namespace. Any future
   replacement of core content must be explicit, dependency-gated, and validated rather than relying
   on load order or filesystem shadowing.

Therefore a physical `Mods/core` directory is not the definition of core-as-a-mod. The desired runtime
property is semantic equivalence at the resolver and definition-registry layers. A literal `core.sfmod`
remains an optional later packaging decision.

Recovered call sites also do not need bulk edits merely to spell `core:`. The central compatibility
seam interprets an unqualified vanilla runtime-art request as an implicit `core:*` request first. New
Eclipse-owned code and mod-facing APIs should use explicit qualified IDs.

## 3. Non-goals for version 1

- Arbitrary C# assemblies or access to raw Unity/.NET APIs.
- Runtime replacement of shaders, scenes, native plugins, or executable code.
- Network access from Lua.
- Guaranteed hot reload during an active fight.
- Loading all seven Acts at startup.
- Cryptographic DRM. Integrity checks are for corruption and diagnosis, not secrecy.

## 4. Package model

The earlier custom `.sfpack` proposal has been superseded. Runtime asset payloads use an
ordinary USTAR archive wrapped in an ordinary LZ4 Frame and are named `.tar.lz4`. TAR is
the semantic archive format; LZ4 is only the compression layer. This keeps packs
cross-platform, inspectable with standard tooling, deterministic, and independent of a
private container specification.

### 4.1 Development layout

```text
Mods/example.weapon/
├── mod.toml
├── assets/
│   ├── sprites/weapon.asset
│   ├── textures/weapon.png
│   └── models/mdl_weapon_example.xml
├── localizations/
│   ├── eng.toml
│   └── pol.toml
└── scripts/
    ├── main.lua
    └── weapons.lua
```

Loose folders are the canonical authoring representation. They make failures inspectable
and let metadata, namespace resolution, and Lua behavior be tested independently from the
container implementation.

### 4.2 Distribution layout

`example.weapon.sfmod` may eventually contain the same virtual paths as the development
directory, including one or more `.tar.lz4` asset archives. `.sfmod` is a complete-mod
distribution wrapper, not a replacement for TAR. Its exact wrapper format should remain
undefined until loose mods pass the first vertical slice.

The base game does not need to become a physical `core.sfmod` before external mods work.
Expose the existing packaged core assets through a `CoreAssetProvider` first. Essential
boot UI, the mod manager, error screens and fallback fonts remain embedded in Unity so a
broken external mod cannot prevent diagnostics from rendering.

### 4.3 Asset archive format v1

The current asset archive contract is:

```text
<group>.tar.lz4
  standard LZ4 Frame
    standard USTAR
      payload files
      descriptor .meta files
```

Descriptors identify loadable objects and reference payloads within the same archive.
The runtime currently accepts typed `sprite`, `audio`, `model` and `atlas` descriptors;
arbitrary files are not implicitly exposed as assets. Generic gameplay/config XML is not
served through this provider.

Requirements:

- Keep archives valid standard TAR wrapped in a valid standard LZ4 Frame.
- Normalize lookup addresses to lowercase forward-slash paths while retaining useful
  original names in diagnostics.
- Reject absolute/traversal paths, links/devices, duplicate or case-colliding entries,
  unsupported descriptor types, missing payloads and cross-archive references.
- Produce deterministic archives from identical extracted input.
- Record compressed size, unpacked size and SHA-256 in the owning catalog and provide a
  full verification command.
- A descriptor namespace must match the namespace of the catalog group that owns it.

Current known tradeoff: the whole LZ4 frame is decoded to a persistent TAR cache on first
access to an archive. A future format iteration may add seekable/chunked LZ4 or independently
compressed payload chunks while preserving the same logical TAR addresses and descriptors.
Do not replace the working TAR semantics merely to solve this performance issue.

## 5. Identity, namespaces and references

Every resource and definition uses an immutable ID:

```text
AssetId      = namespace:path/to/asset
DefinitionId = namespace:category/id
```

Examples:

```text
core:sprites/shop/katana
example.weapon:models/mdl_weapon_example
example.weapon:items/weapon/example_blade
```

Rules:

- Mod IDs are lowercase ASCII identifiers containing letters, digits, dots, `_` and `-`.
- An unqualified reference resolves inside the calling mod's namespace.
- Cross-mod references must be qualified and the target must be a declared dependency.
- `core` and `sf2de` are reserved. `core` owns base-game content addressable through the content
  system; `sf2de` contains bootstrap resources required to diagnose or recover from content failure.
  Core addressability alone does not grant external replacement permission.
- Definitions cannot silently overwrite one another. Replacement requires an explicit
  `replaces` declaration and a dependency on the replaced namespace.
- Load order only breaks ties after dependency ordering; it must not be the primary
  collision mechanism.

## 6. Manifest and dependency resolution

Minimum `mod.toml` fields:

```toml
schema = 1
id = "example.weapon"
name = "Example Weapon"
version = "1.0.0"
api = ">=0.1 <1.0"
authors = ["Author"]
entrypoint = "scripts/main.lua"
capabilities = ["content.register", "events.combat"]

[[dependencies]]
id = "core"
version = ">=1.0 <2.0"
```

The loader must validate manifests, resolve a deterministic dependency graph, reject
cycles and incompatible versions, then display a single consolidated report. Optional
dependencies and explicit conflicts can be added before public release.

## 7. Runtime architecture

```text
Game systems
    |
Definition registries and compatibility adapters
    |
ModHost -------- ScriptHost -------- LocalizationService
    |
AssetResolver
    ├── BootstrapResourcesProvider
    ├── LooseModProvider
    ├── CoreAssetProvider
    │   └── PackagedArtCatalog / TAR+LZ4
    ├── LegacyUnityBundleProvider
    └── LegacyResourcesProvider
```

Core interfaces separate logical asset resolution from providers that can expose a single
canonical byte payload. This matters for `core`, where one logical sprite address may be
backed by TAR metadata plus a shared atlas texture rather than one standalone file:

```csharp
public interface IAssetProvider
{
    ModId Namespace { get; }
    bool TryDescribe(AssetId id, out AssetMetadata metadata);
}

public interface IAssetByteProvider : IAssetProvider
{
    bool TryRead(AssetId id, out AssetBytes bytes);
}

public interface IRuntimeAssetProvider
{
    bool TryLoadUnityAsset<T>(AssetId id, out T asset) where T : UnityEngine.Object;
    bool TryLoadUnityAssets<T>(AssetId id, out T[] assets) where T : UnityEngine.Object;
    bool TryLoadModelText(AssetId id, out string text);
}

public interface IModScriptRuntime
{
    IModScriptContext CreateContext(ModDescriptor mod, ModApiFacade api);
}
```

`AssetResolver` owns namespace routing and eventually dependency checks, typed decoding,
caching, reference counts, fallbacks and diagnostics. There is no implicit "last provider
wins" rule: one mounted provider owns one namespace. `CoreAssetProvider` implements the
typed runtime-provider contract and wraps the existing `PackagedArtCatalog` instead of
reimplementing TAR loading, so `ModAssetLoader` no longer needs to know that core storage is
TAR-backed or expose fake raw bytes for atlas-backed logical assets. Lua receives
typed asset handles, never `Texture2D`, `GameObject`, file paths, streams, or arbitrary
CLR objects.

Core sprite identity includes real atlas members. A member such as
`core:ui/items/armor12.img_armor_mantle_of_night` is describable as a typed sprite only if
`ui/items/armor12` is an exact core atlas address and that named sprite exists inside it. This
promotes the legacy `Atlas.member` convention into the strict namespace model without allowing
arbitrary basename/fuzzy matches for explicit public IDs.

The current compatibility flow is:

```text
unqualified recovered request                    explicit external/core request
"Textures/..."                                   "core:ui/items/armor12.member"
       |                                                       |
       +-- implicit core qualification                         |
                           \                                   /
                            +------ AssetResolver -------------+
                                      /       \
                             core provider     mod provider
                                  |                 |
                         PackagedArtCatalog     loose files
                                  |
                                TAR/LZ4
```

The recovered game is heavily synchronous. Do not spread `Task<T>` through recovered
callers merely to satisfy the provider abstraction. Keep raw IO/decompression capable of
running asynchronously, add preload/scene-scope APIs, and retain a narrow synchronous
compatibility facade at `ResourcesAndBundles` for legacy call sites.

### 7.1 Unity threading and lifetime

- File reads, hashes and LZ4 decompression may run on worker threads.
- `Texture2D`, `Sprite`, `AudioClip` and other Unity objects are created/destroyed on the
  main thread.
- Use explicit `AssetHandle<T>` ownership and reference counts.
- Cache metadata separately from decoded Unity objects.
- Add configurable memory budgets and least-recently-used eviction for decoded assets.
- Pin assets used by the current scene/fight; release scene scopes on transition.
- Never unload an asset while a visible object references it.

### 7.2 Type decoders

- Sprite: PNG bytes plus `.sprite.toml` containing rect, pivot, pixels-per-unit, border,
  filter mode and wrap mode. Atlas metadata can contain named sub-sprites.
- Model: existing SF2 model XML parsed through a compatibility adapter, then through the
  existing model parser. Do not duplicate parsing rules in Lua.
- Animation: existing binary/XML formats with a versioned descriptor.
- Audio: OGG preferred; stream long music and fully load short SFX. Hide temporary-file or
  decoder details behind the audio decoder.
- Localization: TOML/UTF-8 text loaded per namespace with language fallback.
- Text/XML/Lua: return immutable bytes/text with maximum-size enforcement.

## 8. Startup and registration lifecycle

The complete startup sequence is:

1. Initialize bootstrap UI and logger.
2. Discover loose mods and `.sfmod` files.
3. Parse manifests without executing scripts.
4. Resolve dependencies, versions, capabilities and conflicts.
5. Mount virtual filesystems and build asset indexes.
6. Load localization tables.
7. Create one isolated script context per enabled mod.
8. Execute registration entrypoints in dependency order.
9. Validate all assets and cross-definition references.
10. Commit each mod's registration transaction or roll it back entirely.
11. Freeze definition registries.
12. Adapt committed definitions into the legacy SF2 runtime.
13. Enter the existing game loader flow.

No mod should leave half-registered items after a Lua exception. A disabled mod should
produce an error report and allow the rest of the game to start whenever safe.

## 9. Lua language boundary

The syntax in the original concept note resembles C# or Python rather than Lua. Public
Lua APIs should accept tables so fields can evolve compatibly:

```lua
local sf2 = require("sf2")

local weapon = sf2.items.register_weapon {
    id = "example_blade",
    display_name = sf2.localization.key("weapon.example_blade"),
    icon = sf2.assets.sprite("sprites/weapon"),
    model = sf2.assets.model("models/mdl_weapon_example"),
}

sf2.shop.addItem {
    section = sf2.shop.WEAPONS,
    item = weapon,
    level = 12,
    price = sf2.price.coins(1000),
}
```

The normal equipment API deliberately separates identity from balance. Item registration owns
the definition's ID, visuals, model and combat subtype; the shop listing owns its starting level.
Eclipse derives the corresponding current attack/defense attributes from SF2's canonical normal
progression and then attaches the existing category upgrade template. Current normal ranges are
weapon 1..52, armor/helm 2..52 and ranged/magic 6..52. A level-12 weapon therefore enters the
legacy runtime at the `Weapon_Bonus` level-12 milestone (`WeaponDamage=261`) instead of accepting
an unrelated raw number from Lua. Future custom balance must use an explicit multiplier/profile
or custom progression contract rather than silently bypassing the game's upgrade semantics.

### 9.1 API design rules

- Lua-facing handles are opaque and immutable after registration.
- Registrations return typed handles rather than raw string IDs.
- All input tables are copied and validated; retaining a Lua table cannot mutate a
  committed definition.
- APIs use semantic versions. Additive changes stay within a major version.
- Deprecated calls warn for at least one API version before removal.
- Every error includes mod ID, file, line, API call and rejected field where available.
- Expensive per-frame Lua callbacks are not part of API v1.

### 9.2 Planned API modules

Foundation:

- `sf2.mod`: metadata, dependency queries, feature detection and logging.
- `sf2.assets`: typed references and preload groups.
- `sf2.localization`: namespaced keys, formatting and language fallback.
- `sf2.events`: subscriptions with scoped handles and deterministic priority.
- `sf2.random`: deterministic seeded RNG for gameplay-affecting scripts.

Content:

- `sf2.items`: weapons, armor, helmets, ranged weapons and magic.
- `sf2.enchantments`: enchant definitions and declarative triggers/actions.
- `sf2.perks`: perk definitions and supported effect components.
- `sf2.shop`: listings, prices, unlock predicates and visibility rules.
- `sf2.fights`: opponents, rules, stages, rewards and survival/tournament entries.
- `sf2.quests`: quest graph, conditions, actions, dialogue and progression flags.
- `sf2.locations`: map nodes, backgrounds and encounter groups.
- `sf2.audio`: declared cues, not direct mixer access.
- `sf2.ui`: constrained dialogs and menu contributions; arbitrary Unity UI comes later.

Runtime events should expose read-only snapshots plus narrow commands. They must never
expose `Model`, `GameObject`, `Transform`, reflection, threads, sockets or the filesystem.

## 10. Sandbox and reliability model

Treat downloaded mods as untrusted input even if initial users install them manually.

- One script state/context per mod.
- Do not load filesystem, process, environment, network, reflection or CLR interop APIs.
- `require` resolves only the calling mod and explicitly exported dependency modules.
- Provide a controlled logger and deterministic time/game-state services.
- Enforce source size, module count, recursion/stack, callback count and registration
  count limits.
- Add an execution/instruction budget or verified cancellation mechanism before allowing
  recurring callbacks. A wall-clock timeout alone cannot safely stop every interpreter.
- Catch exceptions at every mod boundary and disable only the offending callback/mod.
- Validate pack sizes before allocation and protect against decompression bombs.
- Never deserialize arbitrary CLR types from mod data.
- Capability declarations document and gate sensitive API modules.

## 11. MoonSharp runtime decision

MoonSharp is the initial scripting runtime for Mod API v1. This is a project decision,
not a temporary fallback pending an engine upgrade. The project is already on Unity
2022.3.62f3, but the first public API does not need to depend on a newer interpreter before
the mod lifecycle itself is proven.

Keep MoonSharp behind `IModScriptRuntime`. Public manifests, Lua modules and API functions
must not expose MoonSharp-specific types. A future interpreter replacement remains possible
only if it passes the same conformance suite without changing the public Mod API.

### MoonSharp acceptance and sandbox spike

Run this conformance suite before allowing general recurring callbacks:

- Unity Editor Mono and Windows x86_64 player compile and run.
- IL2CPP test if mobile support remains a goal.
- 10,000 declarative registrations without unacceptable startup or allocation cost.
- Strict virtual `require`; no host filesystem escape.
- Unicode localization and formatting tests.
- Syntax/runtime errors include usable chunk names and line numbers.
- Infinite loop, recursion explosion and memory growth can be contained.
- Disabled standard libraries cannot be recovered through interop.
- Callbacks and states are collected after mod unload.

MoonSharp remains the runtime for Mod API v1 if these checks pass. Re-evaluating another
interpreter is optional future work and is not a delivery dependency.

## 12. Legacy integration strategy

The project already has useful boundaries:

- `ResourcesAndBundles.Load<T>` now routes explicit qualified requests and implicit `core:*`
  vanilla runtime-art requests through `AssetResolver` before the `Resources` compatibility fallback.
- Route plaintext reads in `ResourceManager` through the virtual filesystem instead of
  binding mod logic directly to the canonical `Assets/vanillaXml`/packaged gameplay root.
- `CoreAssetProvider` owns the `PackagedArtCatalog` typed runtime backend through
  `IRuntimeAssetProvider`; upper loaders no longer special-case TAR/LZ4 storage.
- Preserve `BundleManager`, `Resources.Load` and packaged vanilla XML as fallback providers.
- `LegacyContentAdapter` now adapts weapon, armor, helm, ranged and magic definitions into the
  existing `ItemInfo`/`Items`/`ListSF` structures through one narrow recovered item seam.
- Adapt model assets through `ModelLoader` and the existing schema compatibility layer.
- Gradually replace remaining direct `Resources.Load` calls with the resolver.
- Give adapters readable names and tests; do not spread new behavior through obfuscated
  methods when one compatibility boundary can contain it.

Base-game migration sequence:

1. Expose the current packaged asset catalog as namespace `core` through
   `CoreAssetProvider` without changing storage or gameplay behavior. **Implemented.**
2. Make recovered unqualified runtime-art requests resolve as implicit `core:*` at central seams
   so vanilla itself uses the namespace abstraction without bulk call-site rewrites. **Implemented
   for `ResourcesAndBundles`; continue auditing remaining direct bypasses.**
3. Expose existing built-in gameplay definitions through qualified `core` IDs while keeping
   `Assets/vanillaXml` as canonical source of truth. **Implemented for all five primary equipment
   categories.**
4. Generate/author core registry snapshots and manifests from the vanilla source.
5. Move one definition category at a time behind registries: localization, items, shop,
   fights and quests.
6. Compare generated registries and full playthrough checkpoints against the legacy path.
7. Only consider a physical `core.sfmod` after external mods and parity tests are mature;
   it is not required for Mod API v1.
8. Retain a safe-mode bootstrap that disables external mods and can diagnose broken core
   content without depending on external packs.

## 13. Save-game integration

Current implementation:

- New external item records already use qualified definition IDs as their legacy `Name`.
- Enabled mod IDs/versions are recorded in additive `EclipseMods` metadata; absent mods retain
  their last-seen record with `active=false`.
- A deterministic `contentHash` fingerprints active mod IDs/versions plus the actual committed
  localization/item/shop registry. It changes on content edits even when a mod version is not
  bumped, but remains diagnostic rather than making old saves unloadable.
- Missing external item nodes are preserved byte-for-XML-structure in the existing save DOM and
  skipped by active inventory/delivery processing.
- Missing equipped content falls back only in a temporary model input. The saved equipped ID is
  retained unless the player explicitly chooses another item.
- The recovered `UserItem` serialization path is regression-tested for count, upgrade, delivery,
  acquire-type and equip mutations across save-DOM reload.

Still required:

- Let mods register constrained versioned data migrations.
- Back up and transactionally replace saves while applying migrations.
- Prove the complete behavior in a real game restart/removal/reinstall cycle, not only the
  isolated recovered-runtime regression.
- The base game must remain loadable in safe mode after external mods are removed.

## 14. Tooling deliverables

- Keep `Tools/AssetPacker` as the low-level deterministic TAR/LZ4 asset archive tool
  (`pack`, `verify`, `list`, `extract`, `info`).
- Add `sfmod validate` first: validate loose mod manifests, schemas, references,
  dependency graph, asset archives and Lua syntax without packaging the mod.
- Add `sfmod pack`/`list`/`unpack` only after the loose-mod vertical slice is stable and
  the `.sfmod` distribution wrapper is defined.
- Unity editor mod console: status, dependency tree, errors, memory and loaded assets.
- Asset-reference report: missing, ambiguous, overridden and unused assets.
- Lua API stubs/documentation for an editor such as VS Code.
- Example mods: localization-only, one weapon, one enchantment, one fight and one quest.
- CI command that loads all test mods headlessly and validates registry snapshots.

## 15. Testing and quality gates

Unit tests:

- Path normalization, namespace parsing and dependency resolution.
- TAR path safety, malformed/truncated LZ4 frames, bad hashes, duplicate/case-colliding
  entries and cross-archive descriptor references.
- TAR/LZ4 round trips and deterministic pack output.
- Metadata schema validation and every asset decoder.
- Registry transactions, replacement rules and rollback.
- Lua argument validation and sandbox denial tests.

Integration tests:

- Loose and packed versions of the same mod produce identical registry snapshots.
- A modded weapon renders in profile/shop/dojo/fight and survives save/reload.
- Modded magic, ranged weapons and enchantments use their declared animation/VFX.
- Missing optional content degrades visibly without a black screen.
- Broken Lua disables its mod and reaches the main menu.
- Removing and reinstalling a mod preserves orphaned save data.
- Safe mode starts with corrupt external mods.

Performance gates should be measured on the target Windows build:

- Mod discovery/mount cost scales with manifest/index size, not total loose payload size.
- Lookup has no linear scan over packs or entries.
- No unrelated TAR archive is decompressed at startup. First-touch currently decodes the
  owning archive; future chunking should remove that cost without changing API semantics.
- No synchronous disk read or texture decoding during combat.
- Registration and decoded-asset memory are visible in diagnostics.
- Establish numeric budgets from the first vertical slice, then enforce regressions in CI.

## 16. Delivery phases and exit criteria

### Phase 0 — Identity, contracts and baseline

- Finish the loading-path inventory using the now-stable Eclipse content seams.
- Record clean startup, shop, dojo and representative fight baselines.
- Define immutable `AssetId` and `DefinitionId` value types.
- Define `ModDescriptor`, manifest schema and provider interfaces.
- Add architecture/unit tests without changing game behavior.

Exit: identity/path rules are executable tests and existing game behavior is unchanged.

### Phase 1 — Loose mod host and virtual filesystem

- Discover one loose mod under the external `Mods/` root.
- Parse/validate `mod.toml` without executing scripts.
- Implement deterministic namespace/dependency/version resolution and consolidated
  diagnostics.
- Implement `LooseModProvider`, `CoreAssetProvider` and `AssetResolver` composition.
- Resolve raw namespaced text/PNG/model assets through a diagnostic test/tool.

Exit: `example.weapon:*` and `core:*` assets resolve through the same namespace API while
legacy game behavior remains unchanged.

### Phase 2 — Resolver integration and typed asset lifecycle

- Reuse the existing core sprite/model/audio/TAR decoders behind `CoreAssetProvider`.
- Add external loose sprite/model/localization/audio decoding where required by the
  vertical slice.
- Add handles, caching, scene scopes and main-thread Unity object creation.
- Integrate `AssetResolver` before legacy `ResourcesAndBundles` providers.

Exit: one external replacement sprite/model/audio set works in the unchanged game and the
core TAR provider still passes its existing regression suite.

### Phase 3 — MoonSharp script runtime

- Implement `IModScriptRuntime`, isolated MoonSharp contexts, virtual module loading and
  structured logging.
- Disable filesystem, process, CLR/reflection and network escape paths.
- Build the MoonSharp conformance/sandbox suite.

Exit: a script can log, require its own module and resolve an asset while forbidden host
access and runaway behavior are contained.

### Phase 4 — Transactional content API v1

- Add localization, item and shop registries.
- Add typed Lua table validation and registration rollback.
- Adapt committed definitions into legacy runtime data.

Exit: MoonSharp-defined primary equipment appears in the correct recovered shop/profile/runtime
paths; representative buying, upgrading, equipping, preview and fight behavior is proven.

Status: weapon, armor, helm, ranged and magic registration now share the transactional registry and
generic legacy item seam. `example.weapon` has real rendering proof; `example.loadout` demonstrates
all four additional categories with matching core-owned atlas sprites and model assets. Normal mod
equipment now derives its starting attributes from the vanilla progression tables instead of taking
arbitrary raw power values, and the expanded Unity fixture verifies the resulting exact recovered
`ItemInfo` milestones. A full real-game buy/upgrade/equip/fight sequence still needs explicit
regression before treating the exit criterion as closed.

### Phase 5 — Mod-aware save integration

- Record enabled mod IDs/versions/content hash and qualified definition IDs for new data.
- Add aliases/tombstones, orphan preservation and constrained versioned migrations.
- Prove safe startup and save load with the example mod removed and restored.

Exit: the weapon vertical slice survives save/reload, removal and reinstall without
destroying orphaned mod data.

Status: orphan preservation, temporary equipment fallback, restoration, mod/version metadata and a
deterministic content-set fingerprint are implemented and regression-tested. A real player build has
also been manually started successfully with modded equipment already present in its save. Item
aliases/tombstones are implemented as non-destructive registry redirects and are covered by pure,
recovered-save, and Unity integration tests. Versioned migrations and a real remove/reinstall
process-cycle playtest remain open.

### Phase 6 — Deeper combat content

- Ranged and magic item registration is implemented; next add perks and enchantments.
- Prefer declarative triggers/actions; add narrow events only where necessary.
- Connect models, moves, VFX and SFX through qualified asset references.

Exit: representative content from every combat category passes previews and real fights.

### Phase 7 — Fights, locations and quests

- Add opponent/fight/stage/location definitions.
- Add versioned quest graph, conditions, actions, dialogue and progression flags.
- Extend save migrations/orphan handling to progression content.

Exit: an external mini-campaign can be completed, saved, removed and restored safely.

### Phase 8 — Packaged mods and authoring experience

- Define `.sfmod` only after loose-mod semantics are stable.
- Reuse `.tar.lz4` for asset archives inside packaged mods.
- Ship validator, examples, schema docs, API docs, editor definitions and diagnostics UI.
- Prove loose and packaged copies of the same mod produce identical registry snapshots.

Exit: a third party can build and distribute a mod without editing the Unity project.

### Phase 9 — Public API freeze and deeper core registry migration

- Run compatibility/security review and freeze Mod API 1.0.
- Expose current vanilla definitions consistently as `core` and add registry snapshots and
  progression checkpoints for all Acts.
- Implement zone-scoped preload/release groups after correctness is proven.
- A physical `core.sfmod` remains optional and requires full parity evidence.

Exit: external mods depend on stable `core` IDs and Mod API 1.0 without depending on
recovered implementation details.

## 17. Recommended immediate milestone

The original foundation milestone is complete enough to stop treating weapon registration as a
prototype. The next milestone is **equipment parity plus save closure**:

1. Use the corrected vanilla progression profile to verify an actual upgrade of `example.weapon`
   and representative `example.loadout` equipment, including the level/stat change in shop/profile/fight.
2. Repeat a real process cycle with the mod removed, verify safe default equipment and preserved orphan ownership,
   save again, reinstall and verify restoration.
3. Design constrained versioned mod migrations on top of the now-implemented alias/tombstone and
   content-fingerprint contracts, with backup + transactional save replacement before any mutation.
4. Keep the current vanilla registry projection authoritative only as a read-only view. All five
   primary equipment categories now have stable `core:items/...` identity; add parity snapshots
   for their important legacy fields before changing runtime authority.
5. Keep the now-implemented weapon/armor/helm/ranged/magic registration surface narrow and add
   parity tests for any new fields before exposing more recovered item state.
6. Use `example.loadout` to finish real armor/helm try-on and a representative ranged/magic fight,
   then move into perks/enchantments.
7. Continue auditing direct runtime-art loading bypasses and route them through explicit or implicit
   `core:*` ownership at narrow seams rather than bulk-editing recovered callers.
8. Keep `.sfmod` packaging deferred until these loose-mod semantics are stable.

This preserves the current direction: `core` behaves like a built-in content provider using the
same identities/registries as external mods, while TAR/LZ4 and canonical vanilla XML remain the
physical storage/authority until parity evidence justifies a later migration.
