# Agent Guide

## Project overview

This repository is Eclipse, an open-source Shadow Fight 2 reconstruction/base
project built from an AssetRipper export. It targets **Unity 2022.3.62f3**.
Treat recovered game code and assets as archival data: make narrow,
evidence-based changes and preserve serialized Unity identity.

Definitive Edition is a downstream mod/content target, not the identity of the
base project. Do not hard-code new DE policy into Eclipse when the behavior can
wait for or belong behind the modding/content API. See `DE_SCOPE_AUDIT.md`.

## Important locations

- `Assets/Scripts/Assembly-CSharp/` - main recovered game C# source.
- `Assets/Plugins/Assembly-CSharp-firstpass/` - recovered firstpass C# source.
- `Assets/Scripts/Eclipse/` - project-owned reconstruction, desktop,
  compatibility, and presentation code. Keep Eclipse-owned source here, not
  under `Assets/Plugins/`.
- `Assets/Scripts/Eclipse/Runtime/` - project-owned code that must be visible to
  both recovered predefined assemblies. It is compiled by
  `Eclipse.Runtime.asmdef`; keep this assembly independent of recovered
  `Assembly-CSharp` types to avoid circular assembly dependencies.
- `Assets/Editor/` - Unity editor validation/import tools.
- `Assets/xml/` and `Assets/Resources/` - game configuration and recovered
  resources; XML and resource paths are often runtime contracts.
- `Deobfuscation/` - audited, repeatable identifier-recovery workflow. Read
  `Deobfuscation/README.md` before changing mappings or running its scripts.
- `Tools/` - focused repair, extraction, audit, and runtime-check utilities.
- `BuildScripts/` - project-specific build and reference maintenance scripts.

## Working conventions

- Keep changes scoped to the requested issue. Do not reformat, rename, or
  "clean up" unrelated recovered/decompiled code.
- Treat `Assets/Plugins/` as recovered/legacy plugin territory. Do not add new
  Eclipse-owned runtime source there.
- Prefer vanilla/base compatibility and reusable hooks over DE-specific policy.
  When behavior is a DE feature (monetization removal, unlimited energy,
  restored content, permanent events, etc.), keep it isolated so it can become
  a downstream mod/configuration once the modding API exists.
- Preserve every Unity `.meta` file and its GUID. When moving or renaming an
  asset or script, move its `.meta` file with it; never regenerate GUIDs unless
  the task explicitly requires a new asset.
- Prefer existing asset-recovery/import workflows over handwritten serialized
  Unity YAML. In particular, **never manually restore or patch sprite vertex
  layouts**. See `Tools/SPRITE_NATIVE_REBUILD.md`.
- Do not commit generated Unity state (`Library/`, `Temp/`, `Logs/`, `obj/`,
  `.vs/`) or local research dumps under `ResearchSources/`.
- Keep XML, resource, prefab, and C# naming compatible with existing runtime
  lookups. Search call sites and serialized references before renaming fields,
  classes, assets, or resource paths.
- Deobfuscation mappings must be supported by recorded structural or behavioral
  evidence. Use the conservative scripts and their dry-run/idempotency checks;
  do not guess names from proximity or replace identifier substrings.

## Build and verification

Run the smallest relevant checks first. From the repository root, the normal
managed compile checks are:

```powershell
msbuild Eclipse.Runtime.csproj /nologo /v:quiet /clp:ErrorsOnly
msbuild Assembly-CSharp-firstpass.csproj /nologo /v:quiet /clp:ErrorsOnly
msbuild Assembly-CSharp.csproj /nologo /v:quiet /clp:ErrorsOnly
msbuild Assembly-CSharp-Editor.csproj /nologo /v:quiet /clp:ErrorsOnly
```

For Underworld, raid, health-bar, or Cocos frame-parser changes, also run:

```powershell
.\Tools\TestUnderworldRuntime.ps1
python .\Tools\AuditUnderworld.py
```

For name-recovery work, preview before applying and confirm the second preview
is idempotent:

```powershell
python .\Deobfuscation\apply_reviewed_maps.py --dry-run
python .\Deobfuscation\apply_reviewed_maps.py
python .\Deobfuscation\apply_reviewed_maps.py --dry-run
```

When Unity is available, use the matching 2022.3.62f3 editor and run the
relevant menu validator under `SF2` or `Tools > SF2` after importing modified
assets. Managed compilation and static audits cannot validate Unity native
sprite import or thumbnail rendering.

## Change reporting

State which source/assets changed and which checks were run. If Unity editor
validation or a game playtest was not possible, say so explicitly rather than
claiming complete runtime verification.
