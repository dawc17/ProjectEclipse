# Agent Guide

## Project overview

This repository is Eclipse, an open-source Shadow Fight 2 reconstruction/base
project built from an AssetRipper export. It targets **Unity 6.6**.
Treat recovered game code and assets as archival data: make narrow,
evidence-based changes and preserve serialized Unity identity.

Definitive Edition is a downstream mod/content target, not the identity of the
base project. Do not hard-code new DE policy into Eclipse when the behavior can
wait for or belong behind the modding/content API. See `Docs/DE_SCOPE_AUDIT.md`.

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
- `Assets/vanillaXml/` - canonical vanilla 2.41.9 gameplay/configuration XML.
- `Assets/DExml/` - archived Definitive Edition-era XML/model data. Do not use it
  as the base-game authority; it is provenance/reference material until DE becomes
  a downstream mod.
- `Assets/Resources/` - recovered resources and bundled legacy runtime data; XML
  and resource paths are often runtime contracts.
- `Deobfuscation/` - audited, repeatable identifier-recovery workflow. Read
  `Deobfuscation/README.md` before changing mappings or running its scripts.
- `Tools/` - [tool index](Tools/README.md); regression runners and fixtures in
  `Tools/Tests/` grouped by subsystem (see its [index](Tools/Tests/README.md)), audits in `Tools/Audits/`, repair/extraction in `Tools/Recovery/`,
  save utilities in `Tools/Saves/`, and historical upgrade tools in `Tools/UnityUpgrade/`.
- `BuildScripts/` - project-specific build and reference maintenance scripts.
- `Docs/Engineering/` - reconstruction notes, recovery procedures and verification history.
- `Docs/Engineering/Modding/` - mod API engineering plans, acceptance records and archived DE audit evidence.
- `Docs/Modding/` - Git-tracked Astro Starlight modding API wiki and GitHub
  Pages build configuration. Keep website tooling outside Unity's `Assets/`.
- `Tools/ModdingEditor/` - VS Code/LuaLS modding extension, generated API contracts,
  project validation, and mod starter templates. Editor-only Lua definitions must
  never ship as executable mod scripts.

## Modding API documentation

- Keep the modding wiki up to date at all times. Any change to the public API,
  capabilities, manifests, content or asset formats, callbacks, save behavior,
  compatibility, or example mods must update the relevant wiki documentation
  in the same change. Do not leave documentation updates as follow-up work.
- Read `Docs/Modding/README.md` before editing the wiki. Author public guides
  and reference pages in `Docs/Modding/src/content/docs/`. Engineering notes in
  `Docs/Engineering/Modding/` are supporting evidence, not text to copy into the public wiki.
- Keep the wiki thorough and approachable for first-time modders. Explain terms,
  mark required fields/defaults/limits, and include practical Lua examples. Do
  not expose internal sweep or milestone labels in titles, prose, or code samples.
- Every public Lua function, alias, fighter method, and combat callback needs its
  own reference section with Signature, Returns, When, Requires, and a Lua example.
  Keep the binding coverage audit and sidebar in sync as the API grows.
- Document the implemented Eclipse contract. Clearly distinguish supported
  behavior, legacy compatibility, and planned work; the DE parity roadmap is
  not evidence that a feature is available. Preserve verification limits.
- Keep all authored wiki files, configuration, scripts, and the npm lockfile
  Git-tracked. Never commit `node_modules/`, `.astro/`, the generated function
  index, or `dist/` output.
- For documentation/API changes, run `npm ci` in `Docs/Modding/` when installing
  dependencies, then `npm run build`. This checks types, builds the site and
  search index, and validates internal links, anchors, and GitHub Pages paths.
  Report any documentation checks that could not be run.
- When changing API members covered by `Tools/ModdingEditor/`, update its
  authored schema, generated definitions, templates, and editor guide in the same
  change. Run `npm run generate`, `npm run check`, `npm test`, and relevant LuaLS
  and VS Code integration tests; do not confuse editor diagnostics with a game playtest.

## Working conventions

- Keep changes scoped to the requested issue. Do not reformat, rename, or
  "clean up" unrelated recovered/decompiled code.
- Treat `Assets/Plugins/` as recovered/legacy plugin territory. Do not add new
  Eclipse-owned runtime source there.
- Prefer vanilla/base compatibility and reusable hooks over DE-specific policy.
  When behavior is a DE feature (monetization removal, unlimited energy,
  restored content, permanent events, etc.), keep it isolated so it can become
  a downstream mod/configuration once the modding API exists.
- Mod API design: static content stays typed/declarative; custom procedural
  behavior belongs in Lua handlers using safe typed capabilities. Do not build
  a generic operation DSL inside Lua for arithmetic, branching, or variable
  manipulation. Preserve shipped recovered-content compatibility adapters;
  extend programmable behavior through evidence-backed runtime hooks. See
  [the API design rule](Docs/Engineering/Modding/DE_API_IMPLEMENTATION_PLAN.md#37-static-definitions-and-programmable-behavior).
- Preserve every Unity `.meta` file and its GUID. When moving or renaming an
  asset or script, move its `.meta` file with it; never regenerate GUIDs unless
  the task explicitly requires a new asset.
- Prefer existing asset-recovery/import workflows over handwritten serialized
  Unity YAML. In particular, **never manually restore or patch sprite vertex
  layouts**. See `Docs/Engineering/SPRITE_NATIVE_REBUILD.md`.
- Do not commit generated Unity state (`Library/`, `Temp/`, `Logs/`, `obj/`,
  `.vs/`) or local research dumps under `ResearchSources/`.
- Keep XML, resource, prefab, and C# naming compatible with existing runtime
  lookups. Search call sites and serialized references before renaming fields,
  classes, assets, or resource paths.
- Deobfuscation mappings must be supported by recorded structural or behavioral
  evidence. Use the conservative scripts and their dry-run/idempotency checks;
  do not guess names from proximity or replace identifier substrings.
- For obfuscated identifiers used by newly authored Eclipse code, make a
  best-effort descriptive naming guess from the declaration, implementation and
  callers. Limit this cleanup to symbols our new code actually uses, including
  necessary updates to their existing callers. Mark each inferred declaration
  with the exact comment `// best guess for name`. Reuse an existing descriptive
  name when available. Check the owning type, name collisions, reflection and
  serialized references before renaming; a shared obfuscated token does not
  imply a shared meaning. Keep these guesses separate from confirmed recovery
  mappings in `Deobfuscation/`.

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
.\Tools\Tests\Progression\TestUnderworldRuntime.ps1
python .\Tools\Audits\AuditUnderworld.py
```

For name-recovery work, preview before applying and confirm the second preview
is idempotent:

```powershell
python .\Deobfuscation\apply_reviewed_maps.py --dry-run
python .\Deobfuscation\apply_reviewed_maps.py
python .\Deobfuscation\apply_reviewed_maps.py --dry-run
```

When Unity is available, use the matching editor and run the
relevant menu validator under `SF2` or `Tools > SF2` after importing modified
assets. Managed compilation and static audits cannot validate Unity native
sprite import or thumbnail rendering.

## Change reporting

State which source/assets changed and which checks were run. If Unity editor
validation or a game playtest was not possible, say so explicitly rather than
claiming complete runtime verification.
