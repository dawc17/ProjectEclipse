# Eclipse

Eclipse is an open-source Shadow Fight 2 base project built from
an exported Unity project. It targets **Unity 2022.3.62f3**.

The base project is intentionally not the Definitive Edition mod. Project-owned
engine, compatibility, desktop, presentation, and future modding code lives under
`Assets/Scripts/Eclipse/`. Definitive Edition content should ultimately sit on top
of this base through data/mod APIs rather than define the base game's behavior.

## Layout

- `Assets/` - recovered game code, assets, resources, and editor tools.
- `Assets/Scripts/Eclipse/` - Eclipse-owned reconstruction and platform code.
- `Assets/vanillaXml/` - canonical vanilla 2.41.9 gameplay/configuration XML.
- `Assets/DExml/` - archived pre-pivot Definitive Edition XML/model data; not the active base.
- `Deobfuscation/` - reviewed identifier-recovery workflow.
- `Tools/` - validation, repair, and audit utilities.
- `BuildScripts/` - project build and reference scripts.

## Verify

```powershell
msbuild Eclipse.Runtime.csproj /nologo /v:quiet /clp:ErrorsOnly
msbuild Assembly-CSharp-firstpass.csproj /nologo /v:quiet /clp:ErrorsOnly
msbuild Assembly-CSharp.csproj /nologo /v:quiet /clp:ErrorsOnly
msbuild Assembly-CSharp-Editor.csproj /nologo /v:quiet /clp:ErrorsOnly
```

See `AGENTS.md` for project conventions and validation guidance.
See `DE_SCOPE_AUDIT.md` for the current separation between reusable Eclipse work
and behavior that overlaps with the Definitive Edition feature set.
