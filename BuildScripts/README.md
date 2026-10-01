# Build and release scripts

Run commands from the repository root. Current player builds target the Unity
version specified in `ProjectSettings/ProjectVersion.txt`.

| Script | Purpose |
| --- | --- |
| `BuildPlayers.ps1` | Windows/Android player builds; see the [project guide](../README.md#build-windows-and-android) |
| `BuildLauncher.ps1` | Windows launcher build |
| `PackageUpdate.ps1` | Prepare release packages and manifests |
| `PublishUpdate.ps1` | Plan or publish packaged updates |
| `TestLauncher.ps1`, `TestPublishing.ps1` | Launcher and packaging regression checks |
| `MakeBuildPatch.ps1` | Compare two builds and write a patch ZIP with removed-file information |
| `check_script_refs.py` | Audit serialized script GUID references |

The [launcher guide](../Launcher/README.md) documents release requirements and
publishing commands. Player outputs use ignored `Builds/`; launcher/release
outputs use ignored `BuildScripts/out/`.

## Historical reconstruction scripts

`decompress_lzma_assets.py`, `fix_apple_ext.py`, `fix_selectanim.py`,
`gut_purchaser.py`, `hook_devxml.py`, `offline_patch.py` and `remove_security.py`
are retained asset/source repair scripts. They are not steps in the current
player build workflow; review their original assumptions before running them.

`gen_rsp.py`, `roslyn_fp.rsp` and `roslyn_main.rsp` retain an older manual compiler
workflow with Unity 5.6 defines and machine-specific paths. Normal managed
verification uses the root `.csproj` files, as described in
[AGENTS.md](../AGENTS.md#build-and-verification).
