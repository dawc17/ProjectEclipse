# Agent Guide

## Project overview

This repository is an AssetRipper-exported Unity project for SF2 DE. It targets
**Unity 2022.3.62f3**. Treat recovered game code and assets as archival data:
make narrow, evidence-based changes and preserve serialized Unity identity.

## Important locations

- `Assets/Scripts/Assembly-CSharp/` — main recovered game C# source.
- `Assets/Plugins/Assembly-CSharp-firstpass/` — recovered firstpass C# source.
- `Assets/Editor/` — Unity editor validation/import tools.
- `Assets/xml/` and `Assets/Resources/` — game configuration and recovered
  resources; XML and resource paths are often runtime contracts.
- `Deobfuscation/` — audited, repeatable identifier-recovery workflow. Read
  `Deobfuscation/README.md` before changing mappings or running its scripts.
- `Tools/` — focused repair, extraction, audit, and runtime-check utilities.
- `BuildScripts/` — project-specific build and reference maintenance scripts.

## Working conventions

- Keep changes scoped to the requested issue. Do not reformat, rename, or
  "clean up" unrelated recovered/decompiled code.
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
