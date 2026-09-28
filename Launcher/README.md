# Eclipse Windows launcher

Build with `BuildScripts/BuildLauncher.ps1`. Output is
`BuildScripts/out/Launcher/EclipseLauncher.exe`. Requires Windows x64 with .NET
Framework 4.8 (included in current Windows 10/11). This is independent of Unity.
The build embeds `Assets/icon.png` and `Assets/Resources/ui/fonts/AGOpusBold.ttf`,
so the launcher matches the game's icon and title font.
The normal `BuildScripts/BuildPlayers.ps1 -Target Windows` workflow also places
the launcher beside the Windows player. Unity's editor menu alone does not.

Players put the launcher in a writable folder, optionally alongside an existing
`Eclipse.exe`, and run it. Check/install buttons, stable/beta selection and an
automatic-check preference are provided. Updates download into unique staging
folders, verify every downloaded block and reconstructed file, then
switch a small state file atomically. The installed build is playable offline.
Failed downloads leave the active version untouched; staging files can be removed
manually while the launcher is closed. Old versions are retained for rollback.

Incremental packages reuse matching 16 MiB blocks from the installed build and
download missing GZip objects with up to three file workers. Partial downloads
and verified objects persist under `download-cache/`; retry resumes a partial
object when the server supports HTTP Range, or restarts that object if it does
not. A complete block and each assembled file must pass SHA-256 before activation.
The progress bar measures assembled game bytes, including reused bytes.

Cache and old version retention are currently manual. With the launcher closed,
`download-cache/` can be removed to reclaim space; the next update will fetch any
needed objects again. Keep the active and previous version directories for Play
and rollback. Incremental updates still create a complete new version directory,
so allow disk space for both installations plus cached compressed downloads.

The root launcher forwards to the launcher included in the active game package,
so launcher updates take effect at the next launch without overwriting a running
executable. Always retain the root bootstrap and use it as the shortcut target.
Only one launcher per installation runs at a time. It waits for its game process
to exit before allowing updates. Directly launched game instances may continue
running, since installation uses separate directories.

The Roll back button restores the previous version (or the original loose build).
A process-start failure or nonzero game exit also attempts rollback. This detects
process failures, not a frozen/loading-broken game; players can manually roll back
for those cases. A rolled-back release is skipped until a newer version appears.
Rollback does not undo save changes: keep Unity company/product identity stable
and use backward-compatible save migrations. The updater never edits saves.

Desktop mods are stored in `<launcher folder>/Mods`, passed through the
`ECLIPSE_MODS_ROOT` environment variable. Existing mods beside a loose build stay
there. Standalone game launches retain the original adjacent-Mods behavior.

## Outdated game builds

Versioned Windows builds check for updates themselves, so starting `Eclipse.exe`
directly cannot bypass an update. During the splash, the game reads its own version from
`Eclipse_Data/eclipse-version.txt` and downloads the few-byte
`<channel>-version.json` for its launcher channel (same URLs as the manifests
below). When that names a newer version, the title screen shows an "out of date"
page instead of the menu:

- Started from the launcher (`ECLIPSE_LAUNCHER_ROOT` is set): **Update in
  launcher** quits with exit code 3. The launcher treats that code as an update
  request, not a crash, so it checks for updates instead of rolling back.
- Started directly: **Open launcher** starts the root bootstrap. The game finds it
  beside a loose build or two levels above `versions/<version>/`. If no launcher
  is found, the button opens the releases page.

The channel and the version skipped by a rollback come from `launcher-state.json`
in that root. A version that was rolled back does not block, matching the
launcher, which does not offer it again. Only a confirmed newer version blocks:
offline starts, timeouts (about 6 seconds), HTTP errors, a missing version file
(any release published before this check) and malformed replies all allow play.
Unversioned builds (no stamp, e.g. editor-menu or dev builds without `-Version`)
never check. Android builds skip the check; there is no Android updater or
published APK yet. The stamp is a plain file, so this is an update prompt, not tamper
protection.

## Publishing

Build Windows with `BuildScripts/BuildPlayers.ps1 -Target Windows -Version <version>`.
The version is stamped into the player (`Application.version` and
`Eclipse_Data/eclipse-version.txt`) for that build only; `ProjectSettings` keeps its
committed value. `PackageUpdate.ps1` refuses builds without a stamp or with a stamp
that differs from its `-Version`. A wrong stamp would make every install report
itself as outdated. Versions have three numeric components and must increase per
channel. Packaging never contacts GitHub. Stop the build before packaging; source
files must not change.

### First release with the new updater

Create one bridge package containing both incremental assets and the legacy full
ZIP. Adjust these example versions to exceed all previously distributed builds:

```powershell
.\BuildScripts\PackageUpdate.ps1 -Version 1.0.6 -GameDirectory 'F:\path\to\Windows' -IncludeLegacy
.\BuildScripts\PublishUpdate.ps1 -PackageDirectory '.\BuildScripts\out\Releases\stable-1.0.6' -ReleaseTag v1.0.6 -Plan
```

`-Plan` checks local upload lengths/hashes and reports upload bytes without network
calls. Install and authenticate GitHub CLI (`gh auth login`), then create a
**draft** release tagged `v1.0.6` in `dawc17/ProjectEclipse`. The upload script
requires an existing draft and never changes the source tag:

```powershell
.\BuildScripts\PublishUpdate.ps1 -PackageDirectory '.\BuildScripts\out\Releases\stable-1.0.6' -ReleaseTag v1.0.6
# After reviewing the draft, repeat with -Publish to publish and mark latest.
```

The script uploads only the named payloads, both manifests, the channel version
file and the standalone launcher. It excludes the intermediate `game.zip`. Reruns skip completed assets
only when GitHub reports matching size and SHA-256; different existing assets
cause an error instead of being overwritten. It verifies all retained references
before upload and all new assets before publication. Assets without a reported
GitHub digest fail verification and need separate operator investigation.

Old launchers read `stable.json` and install the full bridge build. On the next
launch, its bundled new updater reads `stable-v2.json`. The first bridge release
therefore uploads both representations once. Its version must exceed all
distributed legacy-launcher versions and it must contain this new updater.

### Subsequent releases

Supply the previous published incremental manifest to reuse objects already
hosted, and preserve the bridge manifest unchanged:

```powershell
.\BuildScripts\PackageUpdate.ps1 -Version 1.0.7 -GameDirectory 'F:\path\to\Windows' -PreviousManifest '.\BuildScripts\out\Releases\stable-1.0.6\stable-v2.json' -LegacyManifest '.\BuildScripts\out\Releases\stable-1.0.6\stable.json'
.\BuildScripts\PublishUpdate.ps1 -PackageDirectory '.\BuildScripts\out\Releases\stable-1.0.7' -ReleaseTag v1.0.7 -Plan
```

Create the new draft, upload and review it, then run the publisher with `-Publish`.
Only new `.gz` objects are emitted; unchanged objects keep their original release
URLs. Every latest stable release carries the old bridge `stable.json` so legacy
clients can still upgrade. **Retain every release referenced by a supported
manifest, including the bridge release.** Fresh installs need those objects.

Without `-PreviousManifest`, packaging creates a self-contained incremental
package and uploads all objects again. Without `-IncludeLegacy` or
`-LegacyManifest`, local packaging works but the publisher refuses distribution.
Fixed-size blocks do not realign after inserted bytes: rebuilding large Unity
files can still change many blocks. First installs still transfer the complete
compressed game. Actual savings depend on build changes.

### Channels and limits

New launchers check `releases/latest/download/stable-v2.json`, or
`releases/download/beta/beta-v2.json`. Games check `stable-version.json` or
`beta-version.json` at the same locations: `{"format":1,"version":"1.0.7"}`.
The publisher requires that version to match the manifest. Only HTTP 404 falls back to the corresponding
legacy manifest; corrupt manifests and other HTTP failures remain errors.
Changing channels only offers numerically newer versions, never silent downgrades.

For beta, use `-Channel beta` on both scripts. Payloads use immutable version tags
(default `v<version>-beta`) in draft **prereleases**. Create a public prerelease
tagged `beta` for the channel index first. On `-Publish`, the script publishes the
versioned payload before replacing the two manifests on `beta`, then replaces
`beta-version.json` last, so games only require an update the launcher can already
see. If channel upload fails after publication, manually upload the verified
`beta.json`, `beta-v2.json` and then `beta-version.json` to `beta` before
announcing the update. Do not reuse or move
version-specific tags.

GitHub permits at most 1000 assets per release and each must be below 2 GiB.
The publisher checks the asset count; larger packages need another layout or
host. Legacy ZIP parts remain 1.9 GB. See
[GitHub limits](https://docs.github.com/en/repositories/releasing-projects-on-github/about-releases).
Version directories are immutable. Never republish different files with the same
version. A failed activation can leave an unreferenced version directory; inspect
and move it aside before retrying that same version.

### Incremental manifest contract

Format 2 uses `version`, `notes`, `unpackedSize`, `files`, and `parts`.
Each file has a relative `path`, byte `size`, whole-file `sha256`, and ordered
`chunks` containing uncompressed SHA-256 IDs. Every nonfinal block is 16 MiB.
Empty files have zero chunks and the hash of empty bytes. Each unique part has
its `contentSha256` ID, `unpackedSize`, compressed `sha256`, compressed `size`,
and immutable GitHub release `url`. Compression is GZip; object filenames use
the content hash. Paths, dimensions, required files, duplicates and file/directory
collisions are checked before installation. Manifests are limited to 16 MiB.
Bootstrap state stays format 1; download format is independent.

Trust is HTTPS plus the release account and SHA-256 from its manifest. There is
no private token bundled in the launcher and no independent signing key yet.
Compromise of the GitHub release account can publish executable updates; protect
release credentials. Signing can be added as a subsequent format revision.
The fixed root bootstrap/state protocol is format 1; future incompatible launcher
changes must retain it or require a manually redistributed bootstrap.

Android updating is outside this Windows launcher. No hosted release or Unity
player build is created by compiling the launcher alone.

## Verification

Run `BuildScripts/TestLauncher.ps1` for core failure/installation tests, incremental
reuse, reconstruction, corrupt caches and resume response handling. Pass
`-PackageDirectory <self-contained stable packager output>` to verify packaged
hashes and reconstruct both formats present. That optional check requires every
referenced payload to be present locally; it does not fetch retained releases.
Run `BuildScripts/TestPublishing.ps1` for real packaging and simulated GitHub
upload, rerun, reference-retention, version-stamp and publication checks. GitHub is mocked.
Fixtures remain under ignored `BuildScripts/out/Launcher`. These tests do not
launch Unity, measure live network throughput, or publish a release.
Before distribution, exercise a draft/test-channel release with a real player:
fresh install, offline Play, update from a loose old build, shared mods,
manual rollback, and an older versioned build that is started directly and through the
launcher, which should show the out-of-date page. Keep the launcher in a user-writable directory.
