# Offline Android and Windows playtest

Use Unity **2019.4.41f2**. The project now packages gameplay XML and uses local
startup, saves and clock data. Google Play sign-in, Facebook, remote licensing,
store purchases, cloud saves, remote news/config/downloads and telemetry are
disabled. External links do not launch a browser. Google Play and soundtrack-store
buttons are hidden; ordinary in-game currency purchases remain gameplay features.

Android builds require **IL2CPP + ARM64 only**. Run **SF2 > Configure Android ARM64**
to apply these settings; the build preprocessor rejects other Android combinations
so an ARMv7-only APK cannot accidentally be distributed. The Unity Android NDK and
IL2CPP support must be installed. Verify the resulting APK contains `arm64-v8a`.

## Packaged content

Every Unity player build runs `GameplayContentBuildProcessor` before building.
It packs all non-`.meta` files from `Assets/xml` (including `compat` and relative
includes) into `Assets/Resources/SF2Content/gameplay.bytes`. Missing required files
or a failed resource import abort the build. The resource and its generated meta
are ignored by Git; the processor recreates their stable asset identity.

To package without building, run **SF2 > Package Offline Gameplay XML**.
Run **SF2 > Validate Offline Services** to check the HTTP failure callback and
native Unity resource loading without entering Play mode or touching saves.

The editor continues to read `Assets/xml`. Players extract the bundled resource
to `Application.persistentDataPath/Content/gameplay/<content-version>/`, then use
the same filesystem XML loaders. No download or external XML folder is needed.
Current content is about 19 MB compressed and 104 MiB extracted. Allow room and
time for extraction on first launch. Later launches reuse the completed copy.
Changing source content creates another version directory; old copies and edits
are retained. Content-version SHA256 only names the cache; it is not save hash
validation. A dedicated modding API has not been added by this cleanup.

## Saves and debugging

Save hash checking and hash-sidecar maintenance are off in editor, development
and release builds. Missing or stale hashes are accepted. Existing saves and
sidecars are not deleted by this change. The application/save identity is
unchanged: back up profiles before installing over an earlier build.

Debugging code remains. The optional CUDLR debug listener runs only in Development
Builds on supported mobile platforms and binds only to `127.0.0.1`, not the LAN.
It no longer decrypts the original device whitelist or queries DNS. Ordinary
Unity/Logcat logging remains available in release builds.
Remote gameplay clients reject requests locally;
filesystem and Android `jar:file:` resource loading remain allowed. This is an
offline application configuration, not an OS firewall rule. The editor/package
manager and development profiler are separate from game services.

Removed service assets and their original metadata are archived outside Unity in
`Tools/LegacyServices`. Shared game/UI/serialization utility libraries remain.

## Verification

Run from the project root, after packaging the resource in Unity:

```powershell
msbuild Assembly-CSharp.csproj /nologo /v:quiet /clp:ErrorsOnly
msbuild Assembly-CSharp-Editor.csproj /nologo /v:quiet /clp:ErrorsOnly
.\Tools\TestOfflineRuntime.ps1
.\Tools\TestTutorialRuntime.ps1
.\Tools\TestUnderworldRuntime.ps1
python .\Tools\AuditUnderworld.py
```

The offline test checks archive round trips and the actual Unity-produced resource
against all source files, unsafe/truncated archive rejection, stable cache reuse,
preservation of edited old versions, hashless/stale-hash saves, unavailable-store
callbacks, removed SDK assembly references and network URL rejection. It uses
unique fixtures under `Temp`, never live save files.

The locale checks compile the real implementation against a fake Unity language
property. Startup no longer requires the missing Kokosoft PreciseLocale Android
plugin. Device language comes from Unity; region/currency metadata comes from a
matching local managed culture, or stays empty if unavailable. Unknown languages
fall back to English. These tests do not replace an Android launch test.

The tutorial test compiles the real block-tutorial action and preview methods
against headless UI fakes. It checks completion, unrelated animation events,
missing assets, interruption and callback cleanup; it is not a native playtest.
In the player, replay double sweep -> Profile -> Show block -> return to Map.
Verify the preview plays fully and the menu works without Escape. Also leave
Profile during the preview and confirm the menu works and the block tutorial can
be retried on returning. No save reset is part of the test or fix.

Before distributing, build **Android** and **PC, Mac & Linux Standalone / Windows /
x86_64**, then smoke-test the installed players with networking disabled. Test a
first launch, restart, fresh and existing saves, one ordinary fight, an Underworld
fight, equipment/shop navigation and save persistence. Check logs for exceptions,
missing content and stalled service dialogs. Editor play mode and managed tests do
not establish that the native player builds or device graphics/imports work.
