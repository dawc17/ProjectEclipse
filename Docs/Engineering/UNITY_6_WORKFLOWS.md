# Unity 6 input, multiplayer testing and builds

Use Unity **6000.6.0f1**, matching `ProjectSettings/ProjectVersion.txt`.
The project pins Input System **1.20.0** and Multiplayer Play Mode **3.0.0**.
The scenarios run Eclipse's existing UDP versus and room protocols.

## Input System

Gameplay and project menus read keyboard, mouse and gamepads through
`Assets/Scripts/Eclipse/Runtime/EclipseInput.cs`. The recovered `GamePad` API
delegates to that backend, preserving its button enum and saved `KeyCode`
bindings. Controllers use semantic south/east/north/west buttons, shoulders,
sticks and triggers. Stick up is positive; legacy joystick axes are no longer
consulted. Four controller slots are supported. Removing controller one does
not transfer controller two into its slot.

Recovered EventSystems disable `StandaloneInputModule` and add
`InputSystemUIInputModule` at scene load. Newly created title, controls and
versus EventSystems use the modern module directly. Android dialog touches use
Enhanced Touch. Scene/prefab identities remain intact.

**Active Input Handling stays Both** for recovered IMGUI text fields and native
compatibility. Project polling uses the new backend; the archived
`InputManager.asset` and legacy axis authoring utility are retained. Restart the
editor if Unity requests it after changing input handling or installing packages.

After applying the Input System migration to an editor that was already open,
fully quit and reopen that editor before testing. Restarting Play Mode alone
does not initialize a previously disabled native input backend. A fresh guest
can have working input while the main editor still has no devices registered.
Check **Window > Analysis > Input Debugger**: `Devices (0)` in the main editor
requires an editor restart. Tagged startup also logs a warning for this state.

## Multiplayer Play Mode

Native scenario assets live in `Assets/Settings/Play Mode Scenarios`.
Use **SF2 > Unity 6 > Select Direct Versus Scenario**, then press Play. This
starts in GameLoader with two editor players:

| Player tag | Startup behavior |
| --- | --- |
| `EclipseHost` | Enters versus and hosts on UDP 7311 |
| `EclipseGuest` | Enters versus and joins `127.0.0.1:7311` |

Use the lobby to select loadouts, ready up and start a fight. Tags identify
processes; they do not assign a physical controller to a window. Focus the
desired window for input. Tagged clients run in the background to keep
networking/simulation active when another window has focus. Clone logs stream
to the main editor with distinct colors. Allow up to two minutes for initial
content loading. Startup errors are logged as `[EclipseMPPM]`.

For rooms and spectators, start the local server in a terminal:

```powershell
dotnet run --project Server/EclipseRooms -c Release -- --port 7300
```

Select **SF2 > Unity 6 > Select Rooms and Spectator Scenario**, then Play.
The three tags are `EclipseRoomHost`, `EclipseRoomGuest` and `EclipseObserver`.
All enter the online menu with default room server `127.0.0.1:7300`. Create a
room in the first window, join in the other two, queue the fighters, then choose
**Spectate** in the observer. These remain explicit UI actions; the server is
a separate process.

Tagged clients keep saves, controls, mod selection/user-installed mods, replays,
content caches, thumbnails and diagnostics under
`Application.persistentDataPath/MultiplayerPlayMode/<tag>`. Copy desired user
mods (including any desired project `Mods/` content) into each client's `Mods`
directory and enable matching selections. Tagged clients do not install ZIPs
into the main project's shared mod directory.
Normal editor sessions and players retain their existing data locations.
Display/audio and other legacy PlayerPrefs remain installation preferences.
Tagged data roots use forward slashes to match the recovered XML loader's disk
path prefix checks, including fresh profiles and the title preview sandbox.
Editor clients use Mono. Compare matching editor clients together; IL2CPP
players can have a different network build identity.

Scenario selection is a local editor preference. Choose the normal/default
scenario again for campaign testing. **Set Up Workflows** creates missing
assets through Unity APIs and preserves existing GUIDs and customization.
Scenario creation uses a narrow Unity 6.6 reflection adapter because the
built-in authoring types are internal. Revalidate it when upgrading Unity.

## Build Profiles

Open **File > Build Profiles** and activate the desired committed profile:

| Profile | Settings/output with SF2 build menu |
| --- | --- |
| Eclipse Windows | Release configuration, `Builds/Windows/Eclipse.exe` |
| Eclipse Windows Development | Development Build + Script Debugging, `Builds/WindowsDevelopment/Eclipse.exe` |
| Eclipse Windows Editable XML | Development + debugging, loose XML, `Builds/WindowsEditableXml/Eclipse.exe` |
| Eclipse Android | Android release configuration, `Builds/Android/Eclipse.apk` |

Profiles inherit the canonical scene list and global Player Settings. The XML
profile supplies `ECLIPSE_EDITABLE_XML`; its postprocessor writes loose XML and
the marker even when using the Build Profiles window directly. The Android
profile sets LZ4HC compression; its SF2 build entry point enforces ARM64
IL2CPP/APK. Install the
matching platform support modules. Setup skips creating a platform profile
when its module is unavailable.

For unattended builds, close this project's editor, then run:

```powershell
./BuildScripts/BuildPlayers.ps1 -Target WindowsDevelopment
./BuildScripts/BuildPlayers.ps1 -Target WindowsEditableXml
./BuildScripts/BuildPlayers.ps1 -Target All -Version 1.2.3
```

`All` builds Windows and Android release configurations. Development and XML
builds are opt-in. The runner resolves the versioned editor installation and
passes `-activeBuildProfile` before compilation, preserving profile defines.
Use `-Unity` for a custom installation. Content validation, release stamps and
launcher packaging remain in the build entry points.

## Verification

Run `pwsh -NoProfile -File Tools/Tests/Runtime/TestUnity6Workflows.ps1` for an
isolated native Unity fixture. It compiles the real input bridge/build entry
points, enters Play Mode, injects keyboard/mouse/gamepad state across frames,
checks ownership/disconnects and modern UI default actions, and validates
profile/scenario serialization and setup idempotency.
`Tools/Tests/Combat/TestLocalVersusInput.ps1` covers combat routing/restrictions.

These checks do not verify physical controllers, Android touch behavior,
rendered UI, a full player build or a completed multiplayer fight. Perform the
two-window lobby/fight and three-window room/spectator playtests above before
claiming those end-to-end results.
