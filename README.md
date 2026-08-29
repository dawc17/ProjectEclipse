# SF2 DE Exported Unity Project

AssetRipper-exported Unity project for SF2 DE, targeting Unity **2019.4.41f2**.

## Layout

- `Assets/` — recovered game code, assets, resources, and editor tools.
- `Deobfuscation/` — reviewed identifier-recovery workflow.
- `Tools/` — validation, repair, and audit utilities.
- `BuildScripts/` — project build and reference scripts.

## Verify

```powershell
msbuild Assembly-CSharp.csproj /nologo /v:quiet /clp:ErrorsOnly
msbuild Assembly-CSharp-Editor.csproj /nologo /v:quiet /clp:ErrorsOnly
```

See `AGENTS.md` for project conventions and validation guidance.

## Engine upgrade preparation

The main project remains on Unity 2019.4.41f2. See
[`Tools/UNITY_2022_UPGRADE.md`](Tools/UNITY_2022_UPGRADE.md) for the audited risks,
baseline tests, verified checkpoint tool and isolated Unity 2022.3 migration steps.
