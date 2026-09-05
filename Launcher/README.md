# Eclipse Windows launcher

Build with `BuildScripts/BuildLauncher.ps1`. Output is
`BuildScripts/out/Launcher/EclipseLauncher.exe`. Requires Windows x64 with .NET
Framework 4.8 (included in current Windows 10/11). This is independent of Unity.
The normal `BuildScripts/BuildPlayers.ps1 -Target Windows` workflow also places
the launcher beside the Windows player. Unity's editor menu alone does not.

Players put the launcher in a writable folder, optionally alongside an existing
`Eclipse.exe`, and run it. Check/install buttons, stable/beta selection and an
automatic-check preference are provided. Updates download into unique staging
folders, verify every part's SHA-256 and length, validate/extract the ZIP, then
switch a small state file atomically. The installed build is playable offline.
Failed downloads leave the active version untouched; staging files can be removed
manually while the launcher is closed. Old versions are retained for rollback.

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

## Publishing

1. Build Windows with the existing `BuildScripts/BuildPlayers.ps1 -Target Windows`.
2. Package a build (three numeric version components, increasing per channel):

   ```powershell
   .\BuildScripts\PackageUpdate.ps1 -Version 1.0.1 -GameDirectory 'F:\path\to\Windows' -Notes 'Instant purchases and reward fixes'
   ```

3. Create a **draft** release tagged `v1.0.1` in `dawc17/ProjectEclipse`. Upload
   `stable.json`, all `.partNNN` files and `launcher/EclipseLauncher.exe`, then
   publish and mark it latest. Do not upload the intermediate `game.zip`.
4. Users run the root launcher and install the offered version.

Stable checks `releases/latest/download/stable.json`. Beta checks
`releases/download/beta/beta.json`: use `-Channel beta` and publish the assets on
the `beta` prerelease. Upload all parts before replacing the beta manifest.
Retain version-specific parts while clients may still be downloading them.
Changing channels only offers numerically newer versions, never silent downgrades.
Publishing is explicit; packaging does not contact or modify GitHub.

The ZIP is split at 1.9 GB to fit GitHub's under-2-GiB asset limit:
https://docs.github.com/en/repositories/releasing-projects-on-github/about-releases
Expect disk space for the old installation, downloaded ZIP and extracted update.
Version directories are immutable; never republish different files with the same
version. A failed activation after directory placement can leave an unreferenced
version directory; inspect and move it aside before retrying that same version.

Trust is HTTPS plus the release account and SHA-256 from its manifest. There is
no private token bundled in the launcher and no independent signing key yet.
Compromise of the GitHub release account can publish executable updates; protect
release credentials. Signing can be added as a subsequent format revision.
The fixed root bootstrap/state protocol is format 1; future incompatible launcher
changes must retain it or require a manually redistributed bootstrap.

Android updating is outside this Windows launcher. No hosted release or Unity
player build is created by compiling the launcher alone.

## Verification

Run `BuildScripts/TestLauncher.ps1` for the core failure/installation tests. Pass
`-PackageDirectory <packager output>` to also verify a packaged manifest, every
part hash, ZIP reassembly, and extraction. Test fixtures remain under ignored
`BuildScripts/out/Launcher`. These tests do not launch Unity or publish a release.
Before distribution, exercise a draft/test-channel release with a real player:
fresh install, offline Play, update from a loose old build, shared mods, and
manual rollback. Keep the launcher in a user-writable directory.
