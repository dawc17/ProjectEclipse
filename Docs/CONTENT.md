# Runtime content

Normal builds and play sessions use project-owned inputs under **Assets/**. They do not require
ResearchSources, a sibling content directory beside the executable, downloaded packs in a save
directory, or the recovered Android AssetBundles.

Open **SF2 > Content Browser** for a searchable index of packaged runtime assets, loose Resources,
gameplay/config files, and referenced scenes/assets. TAR entries select the archive that owns the address.

| Content | Canonical location | How it ships |
| --- | --- | --- |
| Sprites, textures, music, sound, models and location art | `Assets/StreamingAssets/SF2Content/ArtBundles/*.tar.lz4` | Standard USTAR archive inside a standard LZ4 Frame |
| Art routing catalog | `Assets/Resources/SF2Content/Art/catalog.json` | Small Unity TextAsset |
| Fonts referenced by packaged art | `Assets/Resources/SF2Content/Fonts/` | Loose Unity Resources so font/material import stays native |
| Existing loose UI, prefabs and animations | `Assets/Resources/` | Existing Resources paths and GUIDs preserved |
| Active gameplay/config XML, location params and includes | `Assets/vanillaXml/` | Build processor creates `Resources/SF2Content/gameplay.bytes` |
| Scenes and referenced objects/shaders | `Assets/src/`, `Assets/GameObject/`, `Assets/Shader/` | Enabled scenes and serialized dependencies |
| Historical DE XML and research dumps | `Assets/DExml/`, `ResearchSources/` | Archival only |

## TAR/LZ4 runtime assets

The current catalog is version 3. It keeps the original 92 logical art groups, plus a core model
group, immutable location-atlas metadata group, and `CORE_LOCATIONS`, which preserves the former
loose `Resources/Textures/Locations` lookup surface. It contains 95 logical groups and routes 94 of
them to `.tar.lz4` files; the font-only group does not need an archive. The original migration
contains 19,318 sprite descriptors and the recovered core-location archive adds another 1,678
descriptors across 562 exact Resources addresses. The archive set also contains 330 PCM16 WAV clips,
582 model documents, 281 location atlas/plist documents, and 10 loose TrueType fonts.

The archives are intentionally opaque to Unity's asset importer. Instead of importing thousands of
PNGs, WAVs, and generated sprite `.asset` files, Unity sees one routing catalog plus the archive
files in StreamingAssets. This removes member-level AssetDatabase/import work while keeping the
content project-owned and distributable with the player.

Each archive is an ordinary USTAR file wrapped in an ordinary LZ4 Frame. Logical objects are
described by small `*.meta` files inside the TAR. Sprite descriptors store the original address,
name, texture path, rect, pivot, border, pixels-per-unit, texture sampling settings, triangle mesh,
and UV evidence. Audio descriptors reference a PCM16 WAV in the same archive. Model descriptors
reference an XML model document in the same archive. Atlas descriptors reference TexturePacker
plist/XML stored as `.txt`. Arbitrary gameplay/config XML remains rejected. References cannot
escape their archive.

The migrated sprite geometry was checked against the previous native Unity representation before
the old source tree was removed. This includes custom meshes, not just rectangular sprites. Model
documents are runtime geometry/physics data and are centralized in TAR. Gameplay XML, tactics,
location params, quests, item data and other configuration remain loose/moddable.

## Runtime loading

Recovered callers still go through `ResourcesAndBundles`. `PackagedArtCatalog` remains the
compatibility facade and preserves the existing path normalization, source ordering, basename
fallbacks, and compatibility aliases. Location paths are the deliberate exception: anything below
`Textures/Locations/` or `Textures/Location_effects/` is exact-path only. Names such as `background_1`,
`layer3`, `left`, `right`, and `pixel_1` are reused by many stages, so a global basename fallback can
silently substitute another stage's art when one member is missing.

For a TAR-backed lookup, the provider opens only the owning logical group. A `.tar.lz4` file is
verified by size and SHA-256, decompressed once to `Application.persistentDataPath/SF2DE/TarAssetCache`,
then the uncompressed TAR is indexed for random entry reads. Repeated lookups reuse both the cached
TAR and runtime-created Unity objects.

PNG payloads become runtime `Texture2D` objects. Sprites are recreated with their original rect,
pivot, border, pixels-per-unit, and mesh through `Sprite.OverrideGeometry`. PCM16 WAV payloads become
runtime `AudioClip` objects. Model and location-atlas text use dedicated, path-restricted APIs rather
than generic `TextAsset` loading. Fonts continue to use Unity's native font importer and Resources path.

On platforms where StreamingAssets is a normal directory, the compressed archive is read directly.
On Android, the synchronous provider opens the installed APK as a ZIP and copies the requested
`assets/SF2Content/ArtBundles/*.tar.lz4` entry into the persistent cache before LZ4 decode. This avoids
blocking the main thread on an asynchronous `UnityWebRequest`. Android is configured for IL2CPP ARM64
by the existing project settings and receives the same archive files as StreamingAssets.

Existing explicit overrides remain. Location textures, atlases and atlas metadata now use TAR like
other runtime assets, while `gamedata/locations/*/params.xml` stays loose. The small
`Textures/Location_effects/particles` subtree remains Unity-native because those effects are serialized
`GameObject` prefabs with GUID-linked materials and textures, not raw file assets. Shop enchantment
glyphs still prefer `UI/Enchantments` before similarly named perk cards. Direct scene/prefab references
keep their Unity GUIDs. The catalog never supplies generic gameplay `TextAsset` content.

`CORE_LOCATIONS.tar.lz4` is ordered before the versioned art groups because the removed loose
Resources location tree previously had the same priority. It contains 464 texture payloads and 1,678
Sprite descriptors. AssetRipper's exported tight meshes are preferred where available; importer-only
Sprite sub-assets fill the remaining atlases. The migration verifies that every stored vertex can
reproduce the original Unity UVs. A small number of recovered float rects are expanded by less than
0.15 pixel to contain their own mesh while keeping absolute pivot and UV positions unchanged, which
is required by `Sprite.OverrideGeometry`.

## Browsing and deliberate edits

`Tools/AssetPacker` is the archive browser/editor helper. It does not require Unity:

```powershell
dotnet run --project Tools/AssetPacker -- list Assets/StreamingAssets/SF2Content/ArtBundles/ITEMS.tar.lz4
dotnet run --project Tools/AssetPacker -- extract Assets/StreamingAssets/SF2Content/ArtBundles/ITEMS.tar.lz4 Temp/ITEMS-edit
dotnet run --project Tools/AssetPacker -- pack Temp/ITEMS-edit Assets/StreamingAssets/SF2Content/ArtBundles/ITEMS.tar.lz4
dotnet run --project Tools/AssetPacker -- verify Assets/StreamingAssets/SF2Content/ArtBundles/ITEMS.tar.lz4
dotnet run --project Tools/AssetPacker -- info Assets/StreamingAssets/SF2Content/ArtBundles/ITEMS.tar.lz4
```

The extracted directory is the editable form. Payloads and descriptor files can be inspected with
ordinary tools, then `pack` creates a deterministic USTAR/LZ4 archive. The validator rejects unsafe
paths, duplicate/case-colliding entries, links/devices, unsupported descriptor types, generic `.xml`
files, and missing or cross-archive payload references. `.xml` files are accepted only when referenced
by a `type=model` descriptor; immutable TexturePacker plist XML is stored as `.txt` behind `type=atlas`.

After intentionally repacking an archive, run:

```powershell
python Tools/AuditNativeContent.py --refresh --deep
```

`--refresh` records the new compressed size, decoded TAR size, and SHA-256 in `catalog.json`.
`--deep` also opens and validates every archive. Without `--refresh`, any changed archive fails.

## Verification

- **SF2 > Validate Packaged Runtime Content** checks catalog structure, every archive size/SHA-256, loose font
  presence, and rejects leftover legacy native art beside the v3 catalog. The build preprocessor
  runs this automatically.
- `python Tools/AuditNativeContent.py` performs the same repository-level integrity checks without
  Unity. Add `--deep` to validate every TAR descriptor and payload reference.
- `Tools/TestPackagedArt.ps1` creates an isolated Unity project containing only the v3 catalog,
  TAR runtime, archives, and loose fonts. It exercises representative sprite, texture, audio, font,
  model, location-art and atlas-data lookups. The editor pass checks the complete location address
  inventory and parses all 281 location atlas records. `-BuildPlayer` additionally builds and runs a
  Windows content smoke player that resolves all 562 `CORE_LOCATIONS` addresses inside the player
  loop and explicitly verifies Moon's 98-vertex `layer3` tight mesh.
- `Tools/MigrateNativeArtToTar.py` is the reproducible one-time bridge from the former native v2
  tree. `generate` is non-destructive, `verify-generated` checks the complete generated set, and
  `commit` removes the old imported groups only after validation.
- `Tools/MigrateModelsToTar.py` and `Tools/MigrateLocationDataToTar.py` reproduce the model and
  immutable location-metadata migrations. Gameplay/config XML is intentionally outside both tools.
- `Tools/MigrateCoreLocationsToTar.py` plus `Tools/CoreLocationExporter.cs` are the one-time recovery
  bridge from the safety copy of the former loose core-location Resources tree. Unity itself reads
  the exported Sprites during that bridge so custom geometry, normalized TexturePacker rotations,
  and texture-import settings are preserved rather than guessed. The installed TAR is the canonical
  editable form afterward.

The `.tar.lz4` files are tracked with Git LFS because several logical groups are larger than normal
Git hosting limits.

## Current experimental tradeoffs

The format deliberately optimizes project/import behavior first, not player memory or first-touch
latency. PNG and WAV data barely shrink under LZ4 because those formats are already compressed or
high-entropy, so total archive size is close to the source payload size. A group also has to be
decompressed once before its first asset can be read. Large groups such as music therefore have a
noticeable cold-load/cache cost.

Runtime PNG decoding also produces ordinary Unity textures rather than build-time platform-compressed
texture assets. If this experiment proves the import-time win is worth keeping, the next format
iteration should target independently compressed/indexed payload chunks or platform-ready texture
payloads while preserving the same `PackagedArtCatalog` compatibility seam.
