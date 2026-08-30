# Archived service integrations

The `Assets/` tree here preserves the removed Google Play, Google/Android bridge,
Facebook, store SDK, fake store, SmartFox, P31RestKit and MobileNativePopups assets with their original
Unity `.meta` files and GUIDs. This directory is outside Unity's `Assets/`, so these
files do not compile, import or ship with the game.

The Assembly-CSharp cleanup also archives the CUDLR HTTP console, unused mobile
permission/licensing modules, unreachable cloud-save and remote-license helpers,
unsubscribed purchase callbacks, and confirmed unreferenced managed helpers.
`AssemblyCleanup/assembly-cleanup.json` records the reason, original path, GUID, line count and
SHA-256 hashes for each source and metadata file. Run
`python Tools/AuditAssemblyCleanup.py` from the project root to verify the archive,
project exclusions, and absence of removed names/GUIDs in active text assets.
This is a reviewed removal list, not a general-purpose dead-code detector.
The new removals live under `AssemblyCleanup/Assets/` and are tracked with their
manifest; older local-only SDK archives remain ignored.

`NetworkController` retains local session/reward/quest completion. `ServerProvider`
retains its configuration-facing API, local clock and unavailable-service
callbacks, without receipt/device payload construction or cloud upload code.
`Tools/TestAssemblyCleanup.ps1` checks complete retained source against minimal
managed fakes; `Tools/TestOfflineRuntime.ps1` also checks the compiled assembly.

Gameplay statistics, local resource loading, XML-selected quest adapters, and UI
components remain even when they have few direct C# references. Shared serializers,
save/anti-cheat value types, audio and UI dependencies are not disposable SDKs.
No scene, prefab, canonical gameplay XML or surviving script GUID was changed.

Validation of this cleanup (2026-08-30): all four managed assemblies compiled;
the archive/reference audit, 40 session/backend assertions, 1,611 offline runtime
assertions, 1,282 Underworld assertions, the Underworld content audit, and 28
tutorial assertions passed. The existing throw regression also passed all 36
playback scenarios. `TestOfflineRuntime.ps1 -SkipPackagedContent` was used for the
runtime pass: the default full test detects a pre-existing generated-resource
mismatch at `Achievements.xml`. Regenerate the gameplay resource through Unity
before running that full check. No Unity native validation or game playtest was
performed for this cleanup.

Runtime callers now use local facades or report service unavailability. Restoring
this archive alone will not re-enable services and can conflict with the offline
store facade. Any future service restoration must be an explicit, reviewed change.

Shared libraries needed by the recovered game, serialization, UI, audio or debug
code remain in `Assets/Plugins`. In particular, `TcpClientImplementation.dll` also
supplies types referenced by the retained HTTP utility code; the HTTP dispatch
entry point is disabled. This cleanup removes service integrations, not every
third-party utility library.

`MobileNativePopups.dll` contained unguarded iOS native imports that failed Android
ARM64 IL2CPP linking. Its callers now use `DialogsOpener.OpenLocalAlertDialog` and
the game's existing touch/mouse dialog. Legacy device-ID and license facades no
longer import `_GetID`, `_CheckLicense`, or `_Close`, or load the removed Android
device-ID bridge.
