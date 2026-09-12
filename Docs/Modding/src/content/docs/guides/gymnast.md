---
title: Author with Gymnast Tool Suite
description: Use an SF2 IK rig and visible model in Blender, then export and package a playable Eclipse character preview.
---

[Gymnast Tool Suite](https://github.com/FlipThoseTitle/Gymnast-Tool-Suite) provides the Blender authoring scene, IK controls, model tools and native animation exporter. Eclipse supplies validation and packaging into a regular Lua mod with a map battle. Start here for visual character authoring.

The tested combination is **Gymnast 1.1.5**, repository revision `b44dea8ae549ff52ec8d08d7d1ad86f53db80702`, and **Blender 5.0.1**. The upstream add-on advertises Blender 4.4+, but its current SF2 scenes use file format 405.91: Blender 4.4.3 and 4.5.3 warn about possible data loss. Use Blender 5.0+ with the bridge. Your Blender 3.6 installation can remain installed separately.

Eclipse does not bundle or modify Gymnast. The bridge loads the checkout you supply and registers its add-on for that Blender session. The upstream repository includes its GPL license; retain attribution and check asset permissions when distributing authoring files or exported content.

## Prepare and open the scene

Run from the Eclipse repository root. Obtain Blender from its [official downloads](https://www.blender.org/download/). A portable build works.

```powershell
git clone https://github.com/FlipThoseTitle/Gymnast-Tool-Suite.git Temp/Gymnast-Tool-Suite
git -C Temp/Gymnast-Tool-Suite checkout b44dea8ae549ff52ec8d08d7d1ad86f53db80702
dotnet run --project Tools/AssetPacker -- extract Assets/StreamingAssets/SF2Content/ArtBundles/MODELS.tar.lz4 Temp/CharacterCore
$blender = 'C:\Program Files\Blender Foundation\Blender 5.0\blender.exe'
.\Tools\Animation\OpenGymnast.ps1 -Blender $blender -Suite Temp/Gymnast-Tool-Suite -Rig Temp/CharacterCore/models/mdl_skeleton.xml -Blend Temp/MyGymnast/character.blend
```

Set `$blender` to your actual executable. If the checkout or extracted assets already exist, reuse them. The open command creates the authoring file only when it is missing; subsequent runs open your existing work. It does not overwrite the earlier `Temp/MyCharacter/character.blend` point-rig experiment or change Blender preferences.

The prepared copy has the upstream **visible capsule body and IK armature**, framed in an orthographic viewport. The armature is selected in Pose Mode. Point labels are hidden and the underlying node objects are protected from accidental selection. They remain present for export. The animation panel is configured for the SF2 IK armature and canonical skeleton. Press **N** for Gymnast's sidebar if needed.

## Animate with IK controls

Use the armature's `HandIK_1`, `HandIK_2`, `HeelIK_1`, `HeelIK_2` and body controls. Insert location/rotation keyframes on the controls; the native point objects follow through the upstream rig constraints. Start with a visible arm motion and check both the body and its controls while scrubbing.

The prepared range is frames **1–60 at 60 fps**. Every frame exports as one native sample with `mid_frames = 0`. For existing 20 fps sample data, use `--mid-frames 2` and a 20 fps scene; the bridge verifies the relationship instead of silently retiming. Native end frame is `sample_count - 1`: frame bounds index stored samples. Interpolation changes playback timing, with nominal sample spacing `(mid_frames + 1) / 60` seconds.

**Keep Gymnast's scene scale.** Its native coordinate conversion is `(Blender X, Blender Z, -Blender Y)` without a ×100 multiplier. The earlier low-level importer used different Blender units; do not mix its point objects into a Gymnast scene. Keep every required object name and use the same dependency rig when importing and exporting animations.

The [upstream animation guide](https://github.com/FlipThoseTitle/Gymnast-Tool-Suite/wiki/Animation-Tool) explains animation import, mirroring, interpolation and weapon-node options.

## Author models and equipment

Use Gymnast's [Model Tool](https://github.com/FlipThoseTitle/Gymnast-Tool-Suite/wiki/Model-Tool) to import native XML geometry or export modeled geometry. Choose the appropriate model type and dependency rig. Its head/body/foot gear and weapon workflows handle the corresponding native attachments. Save optional geometric overlays as XML files before packaging.

Pass character overlays with `--skin`. They are appended to the body/equipment composition. Equipment replacements should use the matching typed equipment API; a skin overlay does not automatically replace the character's weapon or armor. Preserve original rig names/order when reusing native moves and equipment. Native rigs, attachment math and collision behavior still constrain custom models.

## Export an installable preview mod

Save your `.blend`, then run:

```powershell
& $blender --background --factory-startup Temp/MyGymnast/character.blend --python-exit-code 1 --python Tools/Animation/GymnastBridge.py -- export --suite Temp/Gymnast-Tool-Suite --rig Temp/CharacterCore/models/mdl_skeleton.xml --output Temp/MyGymnast/move.bin --package Temp/MyGymnast/local.character-preview --mod-id local.character-preview
```

Add `--skin Temp/MyGymnast/skin.xml` for an exported overlay; repeat for up to 16 files. Use fresh output paths for subsequent exports. Existing files/packages are refused to protect edits. The package folder's name must match `--mod-id`.

The bridge checks missing node objects **before** invoking Gymnast's exporter, verifies finite evaluated coordinates and compares every exported point against the evaluated Blender pose. It then validates the body, optional skins, node counts and binary payload. The upstream exporter can otherwise write zero coordinates for missing nodes. The standalone packager also rejects required bindings collapsed to zero for the entire clip, but cannot prove the names of an arbitrary binary's points because native binaries do not contain them.

If you already exported through Gymnast's UI, package that `.bin` directly:

```powershell
python Tools/Animation/PackageCharacter.py --rig Temp/CharacterCore/models/mdl_skeleton.xml --animation Temp/MyGymnast/move.bin --output Temp/MyGymnast/local.character-preview --mod-id local.character-preview
```

This path validates file structure; only the Blender bridge can compare the output with its source pose. Neither path rescales, reorders or rewrites the animation bytes. Eclipse stores the same payload as `assets/animations/authored.bytes`.

The package contains a manifest, native assets, localization, `scripts/character.lua`, `scripts/main.lua`, a rig fingerprint sidecar, and a point-motion `preview.html`. The preview respects sample spacing. Body and skin appearance should be inspected in Blender and the game, not inferred from the point preview.

## Test in Eclipse

Copy the complete `local.character-preview` folder into Eclipse's `Mods` directory, preserving any existing work under that ID. Enable it through **Mods → Apply & Restart** and find **Character Preview** using the bottom map-page dots.

The repeatable fight uses your authored warrior. Its Lua AI selects `authored_move` whenever the native conditions permit, with a pause between requests. This allows you to watch the export without creating another fight script. The preview move does no attack damage; entry costs and rewards are empty.

Edit `scripts/character.lua` to add the [typed attack intervals](../../api/moves-and-tactics/), change input conditions or supply your own tactic. `scripts/main.lua` is ordinary mod code and can be replaced by your own battles. Inspect both facing directions, rig/skin deformation, equipment attachment, movement, hit reactions and contact timing. A successful binary read does not establish all of those behaviors.

## Verification and limits

`Tools/Animation/TestGymnastPipeline.ps1 -Blender <executable> -Suite <checkout>` exercises the supplied IK/body scene, an authored hand motion, upstream model and animation exports, source-pose comparison, packaging, real Lua map/mode/AI bindings, and the unchanged Unity animation reader. It writes an inspection render and test artifacts under `Temp`.

The [low-level point-rig tools](../character-authoring/) remain available for format work. They are not the recommended visual authoring interface. Automatic retargeting from unrelated skeletons, arbitrary Blender shaders, complete custom character controllers/forms and full-game visual/combat acceptance remain separate requirements.
