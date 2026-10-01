# Recovery and import tools

Run commands from the repository root. These tools recover specific source drops
or perform reviewed migrations; they are not a sequence to run on a fresh checkout.
Local `ResearchSources/` inputs and the separately linked `Mods/de128` repository
are not bundled with Eclipse.

Modes below distinguish repository writes from reports and staging output.
`plan`, `check`, `generate` and extraction commands may still create ignored
`Temp/` or `Library/` files. `--help` is available only for tools using argparse;
older positional tools and helper modules document their own interface.

## Inspect and extract

| Tool | Inputs and outputs | Behavior |
| --- | --- | --- |
| [DumpUnityHierarchy.py](DumpUnityHierarchy.py) | Scene/prefab YAML; prints hierarchy, geometry and component information | Read-only; positional file and optional root-name filter, `--scripts` resolves GUIDs |
| [FindAtlasSpriteRect.py](FindAtlasSpriteRect.py) | Two PNG atlases and a reference rectangle; prints a matching Unity rectangle | Read-only image comparison; positional inputs, requires Pillow |
| [ExtractUnityBundle.py](ExtractUnityBundle.py) | Bundle and output directory; inventories objects or exports recoverable payloads | Inventory by default; `--extract` writes output; `--name`/`--item-xml` filter objects |
| [DecryptSf2CdnXml.py](DecryptSf2CdnXml.py) | Recovered TextAsset directory and destination; writes plaintext XML | Writes destination; `--name` filters filenames; derived keys target the 2.41.9 Android client |
| [RecoverUnitySpriteAtlases.py](RecoverUnitySpriteAtlases.py) | Bundle, output directory and exact texture names; writes frames and Cocos plist metadata | Writes output; not a Unity sprite mesh authoring tool |
| [ExportMapButtonSprites.py](ExportMapButtonSprites.py) | Recovered battle-button atlases; writes grouped PNG crops | Default output `Temp/MapButtonSprites-HighRes`, overridden with `--output`; Python 3.12/Pillow |

## Native sprite and art recovery

Follow the [Unity-native sprite procedure](../../Docs/Engineering/SPRITE_NATIVE_REBUILD.md).
Preserve existing `.meta` GUIDs. Never restore or patch sprite vertex layouts by
hand. Fixture editor versions are independent of Eclipse's current Unity 6.6 target.

| Tool | Inputs and outputs | Behavior |
| --- | --- | --- |
| [ExtractRaidArt.py](ExtractRaidArt.py) | Reviewed bundles under `ResearchSources/NewFiles/bundles`; writes Resources PNG/plist metadata and a native sprite queue | `--dry-run` previews; otherwise writes assets and `Temp/raid-native-sprite-queue.json`; `--bundles` selects inputs |
| [RebuildUnsafeRaidSprites.py](RebuildUnsafeRaidSprites.py) | Suspect records or pending queue; writes a rebuild manifest, then installs Unity-generated sprites | `prepare`, `prepare_pending`, `prepare_assets` stage evidence; `install` rewrites destination sprites after a successful native report, preserving existing GUIDs |
| [RepairUnderworldAssets.py](RepairUnderworldAssets.py) | Recovered raid references and textures; reports dependencies and export defects | Default reports only, including a report file; `--write` repairs generated metadata/adds textures; `--reference-only` limits scope |
| [PrepareNativeArt.py](PrepareNativeArt.py) | `--source` research bundles and `--fixture` isolated project; stages texture/audio/font inputs and a manifest | Writes the fixture; requires the existing UnityPy installation, historically Python 3.12 |
| [ImportNativeArt.cs](ImportNativeArt.cs) | Staged native-art manifest inside the isolated Unity 2022.3 project | `ImportNativeArt.Run` creates/reimports native assets and catalog entries in the fixture; not a main-project setup script |

## Location, portrait and soundtrack imports

These tools retain reviewed owner-drop assumptions. A default invocation is not
necessarily a dry run: select the documented inspection mode explicitly.

| Tool | Inputs and outputs | Behavior |
| --- | --- | --- |
| [ImportUpscaledLocations.py](ImportUpscaledLocations.py) | `ResearchSources/de128_assets` location art/params; updates core location bundle, catalog and both base/fallback params | `plan` previews, `check` verifies, `apply` rewrites installed content; staging under `Library/UpscaledLocations` |
| [ImportUpscaledAvatars.py](ImportUpscaledAvatars.py) | Owner upscales and loose-only avatar metadata; updates USERS bundle and catalog | `plan`, `check`, `apply`; preserves displayed size through descriptor PPU; staging under `Library/UpscaledAvatars` |
| [InstallDELocationParams.py](InstallDELocationParams.py) | Reviewed archived location params; updates `Assets/vanillaXml/locations` and Resources fallback params | `plan` previews, `check` verifies, `apply` writes both trees; historical recovery decisions are recorded in the script |
| [ImportSoundtrackFlacs.py](ImportSoundtrackFlacs.py) | `ResearchSources/sf2flacs` and reviewed track pairings; overwrites selected base and DE128 audio files | Default writes via ffmpeg; `--check` renders temporary comparisons; existing Unity metadata is retained |

## Downstream DE128 generation

These tools write the separately maintained `Mods/de128` content. They do not
make DE policy part of the base project. The common `--check` mode compares
expected output without replacing the mod's files; required research inputs and
dependencies must still exist. Default invocations generate or install output.

| Tool | Evidence inputs | Owned outputs |
| --- | --- | --- |
| [GenerateDE128Underworld.py](GenerateDE128Underworld.py) | Reviewed owner raid XML and archived stages/items/localizations | Typed Underworld Lua data under `scripts/content/` |
| [GenerateDE128ShopAvailability.py](GenerateDE128ShopAvailability.py) | Base and archived item lists | Shop/catalog Lua projections |
| [GenerateDE128ChallengerText.py](GenerateDE128ChallengerText.py) | Archived localization tables and owner English evidence | Challenger/opponent/reward localization and key module |
| [GenerateDE128MoveNames.py](GenerateDE128MoveNames.py) | Archived names of restored unarmed moves | Generator-owned move localization sections |
| [GenerateDE128TitanRewardText.py](GenerateDE128TitanRewardText.py) | Archived localization tables | Titan reward localization/key data |
| [ExtractDE128ChallengerArt.py](ExtractDE128ChallengerArt.py) | Owner portraits, battle previews, buttons and music | Challenger textures, descriptors and audio |
| [ExtractDE128DojoPreviewArt.py](ExtractDE128DojoPreviewArt.py) | Owner dojo medallions and reviewed India preview source | Dojo chooser textures/descriptors and generated selection halo |
| [ExtractDE128SenseiArt.py](ExtractDE128SenseiArt.py) | Owner drop, DE reference export and selected vanilla bundle fallbacks | Sensei portraits/battle previews as PNGs and mod descriptors |
| [ExtractDE128TitanRewardArt.py](ExtractDE128TitanRewardArt.py) | Archived reward geometry | Packed Titan reward presentation data |
| [ExtractDE128UnderworldArt.py](ExtractDE128UnderworldArt.py) | Owner raid/button art and selected native Resources inputs | Underworld textures, mod descriptors, geometry and audio |
| [ImportDE128MapButtons.py](ImportDE128MapButtons.py) | Owner DENew buttons and shipped TAR metadata | Exact-address mod button replacements; Python 3.12/Pillow/lz4 |
| [ImportDE128MenuArt.py](ImportDE128MenuArt.py) | Owner DENew menu, VS and loading artwork | Mod replacements; `--check` compares source bytes |
| [DE128Localization.py](DE128Localization.py) | Localization mappings supplied by the generators | Shared section/key-module writer; helper module, not a CLI |

## Historical archive migrations

These tools assume specific earlier layouts and safety backups. `generate`
rebuilds staging output; verification modes inspect generated archives. Their
`commit` command **installs content**, updates catalogs and, for migrations of
loose trees, removes migrated inputs. It is not a Git commit.

| Tool | Original inputs and outputs | Modes and limits |
| --- | --- | --- |
| [MigrateNativeArtToTar.py](MigrateNativeArtToTar.py) | Earlier Unity-native art tree to TAR/LZ4 v3 bundles/catalog | `generate`, `verify-generated`, `commit`; staging under `Library/TarAssetMigration`; commit removes migrated native trees |
| [MigrateLocationDataToTar.py](MigrateLocationDataToTar.py) | Immutable atlas/plist TextAssets to TAR/LZ4 catalog | `generate`, `verify`, `commit`; location params remain loose; commit installs the archive/catalog and removes migrated metadata |
| [MigrateModelsToTar.py](MigrateModelsToTar.py) | Model geometry XML to runtime TAR/LZ4 assets | `generate`, `verify`, `commit`; commit replaces installed content and removes migrated loose models; gameplay/config XML remains outside TAR |
| [MigrateCoreLocationsToTar.py](MigrateCoreLocationsToTar.py) | Earlier loose-location safety backup to native-authoritative descriptors and TAR/LZ4 | `generate`, `verify-generated`, `commit`; uses a disposable Unity fixture, defaults to historical 2022.3.62f3 (`--unity` overrides) |
| [CoreLocationExporter.cs](CoreLocationExporter.cs) | Unity-imported Sprite assets in that disposable fixture | `CoreLocationExporter.Run` writes descriptors/geometry, addresses and stats to the migration's configured output |
| [BuildXmlCompatAssets.py](BuildXmlCompatAssets.py) | Earlier plaintext `ResearchSources/reversingsf2` export | Historical direct writer to `Assets/xml/compat/stages.xml`; assumes the old XML tree and has no dry-run flag |

See the [runtime content guide](../../Docs/CONTENT.md) before changing installed
archives, and the [audit index](../Audits/README.md) for checks after deliberate edits.
