# Eclipse

Eclipse is an open-source Shadow Fight 2 base project built from
an exported Unity project. It targets **Unity 2022.3.62f3**.

The base project is intentionally not the Definitive Edition mod. Project-owned
engine, compatibility, desktop, presentation, and future modding code lives under
`Assets/Scripts/Eclipse/`. Definitive Edition content should ultimately sit on top
of this base through data/mod APIs rather than define the base game's behavior.

## Layout

- `Assets/` - recovered game code, assets, resources, and editor tools.
- `Assets/Resources/SF2Content/Art/` - native runtime art and its catalog; no research folder is required to run a build.
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

Run managed runtime test scripts under **PowerShell 7 (`pwsh`)**, not Windows
PowerShell 5.1; Unity 2022's managed API is not compatible with the older host.

## Build Windows and Android

Install Unity **2022.3.62f3** with Windows build support and **Android Build
Support**, including its SDK/NDK tools and OpenJDK. Use Unity's embedded Android
toolchain rather than an unrelated system Java installation.

In Unity, select the target platform in Build Settings, then use
**SF2 > Build > Windows x86_64** or **Android ARM64 APK**.
Outputs go to the ignored `Builds/Windows/Eclipse.exe`
and `Builds/Android/Eclipse.apk` paths. Android uses IL2CPP, ARM64 only, and
LZ4 high-compression player data to keep the large content set packageable.
The configured enabled scenes are checked, packaged art is validated, and the
offline gameplay archive is regenerated before each player build.

For unattended builds, close the editor for this project first and run:

```powershell
& .\BuildScripts\BuildPlayers.ps1 -Target All
```

Use `-Target Windows` or `-Target Android` to build just one target. The script
starts a separate Unity process with an explicit platform for each build; use
`-Unity`, `-ProjectPath`, and `-OutputDirectory` to override its paths. Each target
gets a build log alongside the output directories. The APK uses the
project's existing signing settings; configure a release keystore separately
before distributing a production release.

See `AGENTS.md` for project conventions and validation guidance.
See `CONTENT.md` for the content layout and validation. Use **SF2 > Content Browser** to search assets across the project from one window.
See [Mods/README.md](Mods/README.md) for loose mod assets, sprite descriptors, and the Lua API.
See `DE_SCOPE_AUDIT.md` for the current separation between reusable Eclipse work
and behavior that overlaps with the Definitive Edition feature set.
