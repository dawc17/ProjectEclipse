# Unity 2022.3 upgrade preparation

Prepared 2026-08-29. **The main project still targets Unity 2019.4.41f2.**
No engine migration, package update, art extraction or bundle-loader change has
been applied. Do not open the original checkout with a newer editor.

## Checkpoint and isolated workspace

Run from the original repository with Python 3.12 or newer:

```powershell
python Tools/PrepareUnityUpgrade.py --name unity2022-preflight-20260829 --backup-userdata 'C:/Users/dawid.DAWCZA67/AppData/LocalLow/sf2/de128/userdata'
```

The tool creates two local, Git-ignored directories outside `Temp`:

- `UpgradeBackups/<name>/`: verified original project zip, SHA256 manifest,
  Git HEAD/status and uncommitted patch; optional private userdata zip.
- `UpgradeWorkspaces/<name>/`: separate files, never hardlinks or symlinks.
  Includes current uncommitted gameplay fixes and prefab edits.

Every archived project file and original `.meta` file is checked against the
source and copy. Existing destinations are rejected. If source files change
during preparation, verification fails; use a fresh checkpoint name after
saving/stopping edits. Only a manifest with `verified: true` is a valid checkpoint.

The clone has two deliberate differences, recorded in its manifest:

1. Its product name is suffixed with `_upgrade2022_<name>`. This gives it a
   separate Unity persistent-data and PlayerPrefs identity. Current userdata is
   archived only, not restored into any live profile. Existing registry
   PlayerPrefs are not included in the backup. Never copy the original product
   name back before testing is finished. Use a separate Android application ID
   before installing a test APK; product-name isolation alone does not isolate
   an installed Android application.
2. Five automatic recovery importers, including the postprocessors in those
   files, are copied with their `.meta` files to `UpgradeQuarantine/Editor`,
   outside `Assets`. This prevents them from reslicing assets during the first
   upgraded import. The baseline zip contains them at their original paths.
   Review and restore them individually after establishing native rendering.

Excluded: Git history, Unity caches/build output, vendored Python packages,
research dumps and original research bundles. The latter are SHA256-inventoried,
not moved or modified. Backups remain on the same disk: copy them to another
disk for protection against drive failure. Save archives are private.

## Installed editors and initial target

- Found `F:/UnityInstalls/2019.4.41f2/Editor/Unity.exe`.
- Found Unity `6000.4.7f1` under `C:/Program Files/Unity/Hub/Editor`; this is not
  the requested 2022 target. Do not let Hub choose it automatically.
- No Unity 2022 editor found in those installation roots. Installation and
  exact patch selection are still required.
- Use a compatible **2022.3 LTS** patch. The research bundles span
  `2022.3.11f1`, `15f1`, `37f1`, `62f2` and `62f3`; test against the newest bundle
  format before assuming all can load. Confirm the selected editor's actual
  availability and security updates at installation time.
- Preserve Built-in rendering (`m_CustomRenderPipeline: 0`), Gamma color
  (`m_ActiveColorSpace: 0`), legacy Input Manager and existing scene order.
  Do not combine the upgrade with URP, a new input system, or artwork migration.
- Current project settings specify Android IL2CPP; the build preprocessor
  requires ARM64 only. Generated compiler definitions currently target Android.
  Windows standalone is a separate verification target, not an assumed setting.

## Audit findings and decisions

| Area | Evidence in this checkout | Upgrade handling |
| --- | --- | --- |
| Older packages | TMP 2.1.4, Timeline 1.2.18, Test Framework 1.1.31, Rider 1.2.1, VS 2.0.15, VS Code 1.2.5, XR helpers 2.1.9 | Leave baseline pinned. Resolve supported package versions in the clone with the selected editor; record every manifest/lock change. |
| Multiplayer HLAPI | `com.unity.multiplayer-hlapi` 1.0.8; no gameplay `NetworkBehaviour`, `NetworkIdentity`, `NetworkClient` or `NetworkServer` usage found | Candidate for removal if resolution fails. Check package GUID references in prefabs/scenes before removing. `UnityEngine.Networking` imports also cover UnityWebRequest and do not alone indicate HLAPI use. |
| Managed plugins | 13 DLLs: DOTween and 43/46/50 modules, Vectrosity, protobuf-net, TcpClientImplementation, Mono.Data/SqliteClient, System.Configuration/EnterpriseServices/Runtime.InteropServices/Runtime.CompilerServices.Unsafe | Check reference validation and overlapping framework types in the new runtime. These are risks, not confirmed failures. Do not delete or bulk-update DLLs preemptively. |
| Older APIs | WWW in ResourceManager, InternetUtils, CUDLR, AudioManager and SceneLoader; Application.LoadLevel in DownloadObb | Let the new compiler/API Updater identify actual failures. Preserve local-file and offline-network behavior when replacing calls. |
| Recovery tools | Five `[InitializeOnLoad]` importers modify textures or generate sprites; several also run on import/Play Mode changes | Quarantined only in the clone. Re-enable one at a time with metadata diffs. |
| Native sprite validation | DevSpriteCrashValidator reflects internal SpriteUtility.GetSpriteUVs and expects a separate `Temp/raid-sprite-rebuild.json` | Internal signature and missing manifest must be checked in the new editor. A compiled validator is not proof of native UV/thumbnail correctness. Follow SPRITE_NATIVE_REBUILD.md; never patch sprite vertex YAML. |
| Build references | Tracked csproj files point at Unity 2019 assemblies; BuildScripts/gen_rsp.py still embeds Unity 5.6/.NET 2.0-era symbols and paths | Regenerate project files in 2022. Do not use the legacy response-file generator to certify migration. Compare native Unity compile output and actual players. |
| Offline content | GameplayContentBuildProcessor rebuilds `Assets/Resources/SF2Content/gameplay.bytes` from `Assets/xml` | Repackage in the clone and test it. Preserve offline services, ARM64 enforcement and XML resource paths. |
| Save location | SF2Paths.Init uses Application.persistentDataPath; current company/product are sf2/de128 | Clone product identity is isolated. Android test package ID must also be isolated before installing. |
| Research bundles | 96 Android Unity 2022 bundles; 159 repeated art groups, 47 with different stored texture/metadata data | Defer loader changes. First test loading and rendering; differing compressed data does not necessarily mean visibly different images. Never use a basename-only index for competing paths. |

## Baseline checks on 2019 (2026-08-29)

| Check | Result |
| --- | --- |
| Assembly-CSharp managed build | PASS |
| Assembly-CSharp-Editor managed build | PASS |
| Tools/TestEclipseRuntime.ps1 | PASS: 805 assertions, 21 replay segments |
| Tools/TestProjectileRuntime.ps1 | PASS: 204 assertions, 129 projectile/magic intervals |
| Tools/TestTutorialRuntime.ps1 | PASS: 28 assertions |
| Tools/TestUnderworldRuntime.ps1 | PASS: 1282 assertions |
| Tools/AuditUnderworld.py | PASS: 76 battles, 39 locations, no issues |
| Tools/TestOfflineRuntime.ps1 | **KNOWN FAIL before migration:** `Unity resource differs: stages.xml` |
| Checkpoint tool fixture | PASS: original preservation, metadata, private save archive, isolated identity, hash verification, overwrite/traversal refusal |
| Unity 2022 compile / native import / player tests | NOT RUN |

The offline failure is an existing mismatch between the packaged resource and
source XML. It is not an upgrade regression. Repackage with **SF2 > Package Offline
Gameplay XML** in the clone, then rerun the complete test; do not suppress it or
claim the full offline suite passes. The original resource was left unchanged.

## Migration gates

1. Verify the checkpoint manifest, save isolation and selected editor version.
   Save current Unity work first; the checkpoint includes files on disk, not
   unsaved scene edits. Install 2022.3 separately, with Android SDK/NDK/OpenJDK
   and Android IL2CPP support; keep 2019 available for rollback/comparison.
2. Review Unity's 2020, 2021 and 2022 upgrade guides in release order. This means
   reviewing intervening changes, not blindly rewriting engine versions in YAML.
   Open only the isolated workspace. Capture the full first-import log. If the
   direct jump fails, diagnose in that disposable copy before considering a
   staged 2020/2021 migration.
3. Resolve package and compiler failures narrowly. Keep assets' GUIDs, resource
   paths and serialized field names. Do not globally reserialize or change
   shaders/pipelines just to remove warnings. Regenerate IDE projects and build
   against the new editor's assemblies.
4. Verify all seven build scenes load without missing scripts, textures or
   materials. Review `.meta` changes. Validate native sprite UVs and render
   thumbnails, then compare map backgrounds, portraits, equipment icons,
   stretched UI, fonts, VFX and fight-location layers against 2019.
5. Restore recovery tools individually after reviewing their Unity 2022 API
   behavior. Repackage gameplay XML and run every baseline test above. These
   headless tests complement, but cannot replace, a native playtest.
6. On a disposable profile test startup/settings, shop/profile navigation,
   tutorial previews, ordinary melee blocks, unblocked ranged/magic attacks,
   correct projectile VFX/SFX, Eclipse bodyguards -> boss -> repeat, Underworld,
   save/reload and scene transitions. Repeat with a COPY of an existing save.
7. Build and run Android ARM64 and Windows x86_64 independently. Verify Android's
   test package ID before installation. Confirm offline startup and persistence.
8. Only then investigate direct art loading from the new bundles. Sample ITEMS,
   ZONE_2, ZONE_RAID, FONTS and an event bundle. Test actual Android and Windows
   rendering separately. Keep models, animation data, XML and audio on their
   current paths. Choose duplicate-art precedence only after examining the
   actual differences. Do not infer compatibility from an Editor-only success.

Rollback means using the untouched original project with 2019, or restoring the
verified zip into a NEW folder. Never downgrade upgraded assets in place or
overwrite current work with an old snapshot. This preparation makes no commits.

## Official references

- [2020 LTS upgrade guide](https://docs.unity3d.com/2022.3/Documentation/Manual/UpgradeGuide2020LTS.html)
- [2021 LTS upgrade guide: framework and managed-plugin conflicts](https://docs.unity3d.com/2022.3/Documentation/Manual/UpgradeGuide2021LTS.html#NET)
- [2022 LTS upgrade guide](https://docs.unity3d.com/2022.3/Documentation/Manual/UpgradeGuide2022LTS.html)
- [AssetBundle platform requirements](https://docs.unity3d.com/2023.2/Documentation/ScriptReference/BuildAssetBundlesParameters-targetPlatform.html)
