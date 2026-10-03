# Project Eclipse

Project Eclipse is a mod engine for Shadow Fight 2. Think of this as the SF2 equivalent of Forge for Minecraft. </br>
It targets **Unity 6.6**.

**The project is not meant to be played on it's own without mods, even if it is possible** </br>
It is intentionally bare, and it is strongly recommended to play with mods. (when that time comes lol)

Definitive Edition 128 will be bundled as a mod, and enabled by default on the first stable public release. </br>
This will be the """vanilla""" Project Eclipse experience, but modders will always have the option to disable DE128 for their modding purposes. </br>

The base project is intentionally not Definitive Edition. Project-owned
engine, compatibility, desktop, presentation, and future modding code lives under
`Assets/Scripts/Eclipse/`. </br>

## Layout

- `Assets/` - recovered game code, assets, resources, and editor tools.
- `Assets/Resources/SF2Content/Art/` - native runtime art and its catalog; no research folder is required to run a build.
- `Assets/Scripts/Eclipse/` - Eclipse-owned reconstruction and platform code.
- `Assets/vanillaXml/` - canonical vanilla 2.41.9 gameplay/configuration XML.
- `Assets/DExml/` - archived pre-pivot Definitive Edition XML/model data; not the active base.
- `Deobfuscation/` - reviewed identifier-recovery workflow.
- `Tools/` - [tool index](Tools/README.md), with tests, audits, recovery and save utilities grouped by purpose.
- `BuildScripts/` - [build, release and reference scripts](BuildScripts/README.md).
- `Docs/Engineering/` - [reconstruction notes and recovery procedures](Docs/Engineering/README.md).
- `Docs/Modding/` - Astro Starlight modding wiki; see its [setup and deployment guide](Docs/Modding/README.md).

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

Install Unity **6000.6.0f1** with Windows build support and **Android Build
Support**, including its SDK/NDK tools and OpenJDK. Use Unity's embedded Android
toolchain rather than an unrelated system Java installation.

In Unity, activate **Eclipse Windows** or **Eclipse Android** in **File > Build Profiles**, then use
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

Use `-Target Windows` or `-Target Android` to build just one target.
`WindowsDevelopment` and `WindowsEditableXml` select the corresponding tester
profiles. See [Unity 6 workflows](Docs/Engineering/UNITY_6_WORKFLOWS.md) for the
Input System migration and ready-to-use Multiplayer Play Mode scenarios. Pass
`-Version major.minor.patch` for a release build: packaging requires it, and the
player uses it to refuse to start once a newer release is published (see
`Launcher/README.md`). Builds without `-Version` are unversioned dev builds that
never check for updates. The script
starts a separate Unity process with an explicit Build Profile for each build; use
`-Unity`, `-ProjectPath`, and `-OutputDirectory` to override its paths. Each target
gets a build log alongside the output directories. The APK uses the
project's existing signing settings; configure a release keystore separately
before distributing a production release.

See `AGENTS.md` for project conventions and validation guidance.
Open verification and follow-up work is tracked in [the backlog](todo.md).
For IDE work, use `ProjectEclipse.sln` or `ProjectEclipse.slnx`; both include the
four managed projects. Installed Unity IDE integrations generate the solution
name from this project's directory. Build scripts continue to use `.csproj` files.
See [runtime content](Docs/CONTENT.md) for the content layout and validation. Use **SF2 > Content Browser** to search assets across the project from one window.
See [Mods](Mods/README.md) for mod installation and selection, and the
[modding wiki](Docs/Modding/README.md) for asset formats and the Lua API.
See [Local Multiplayer](Docs/LOCAL_MULTIPLAYER.md) for the current two-player Local Versus controls and limitations.
See [the scope audit](Docs/DE_SCOPE_AUDIT.md) for the current separation between reusable Eclipse work
and behavior that overlaps with the Definitive Edition feature set.
