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

Older asset/source patch scripts and manual compiler response files are retained
under [Legacy](Legacy/README.md). They are not steps in the current player build
workflow. Normal managed verification uses the root `.csproj` files, as described
in [AGENTS.md](../AGENTS.md#build-and-verification).
