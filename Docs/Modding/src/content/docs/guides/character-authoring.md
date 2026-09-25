---
title: Low-level point-rig tools
description: Import a native SF2 rig into Blender, author its motion and geometric skin, and install a character with playable controls.
---

For visual authoring, start with [Gymnast Tool Suite and Eclipse packaging](../gymnast/). It provides a visible body and IK controls. This page documents the earlier low-level point importer for format experiments; its point objects alone are not a complete character authoring interface.

Eclipse characters use SF2's point-based physics rig. An animation stores the positions of those points in a fixed order; body and equipment models attach geometry to them. These low-level tools require Python 3 and Blender 3.6 or newer. Blender 3.6.23 is the tested version for this importer only.

The pipeline supports body proportions, native geometry overlays, animation import, constrained or keyframed point motion, baking, validation, a local preview, and runtime registration. It does not automatically retarget an arbitrary FBX skeleton or turn Blender materials into game shaders. Preserve the native rig's names and point order when sharing the game's moves, equipment, and physics. Substantially different skeletons also need compatible moves and equipment.

## Create an authoring scene

Run these commands from the repository root. The extraction writes a separate working directory and leaves the shipped assets intact:

```powershell
dotnet run --project Tools/AssetPacker -- extract Assets/StreamingAssets/SF2Content/ArtBundles/MODELS.tar.lz4 Temp/CharacterCore
$blender = 'C:\Program Files\Blender Foundation\Blender 3.6\blender.exe'
& $blender --background --factory-startup --python-exit-code 1 --python Tools/Animation/BlenderCharacter.py -- create --rig Temp/CharacterCore/models/mdl_skeleton.xml --blend Temp/MyCharacter/character.blend
```

Open the resulting `.blend` in Blender. `SF2_Nodes` contains named point objects, and `SF2_Skins` is the collection for your optional geometry. Each point's `sf2_node` property preserves its binding; renaming the visible Blender object is harmless, but changing or removing the binding is rejected during export. `sf2_mass` controls the exported point mass. The original rig XML is stored on the scene.

To import a native animation too, add `--animation path/to/clip.bytes` to `create`. Add `--mid-frames N` if its move uses intermediate frames; the default is 0 and the supported import range is 0–8. This expands source keyframe spacing before baking. The scene runs at 60 frames per second. Blender frame 1 becomes game frame 0.

## Author the body and motion

Move the point objects or constrain them to your own Blender armature. The exporter samples their **evaluated world positions**, so constraints can drive the animation. Keep every rig binding. Author motions across the scene's start/end frame range; both endpoints are included.

The scene's first frame is also the exported body's rest pose. Changing proportions updates native edge lengths. Check the resulting collision shape and equipment fit in the game, especially after large proportion changes. Macro nodes and centers of mass are derived from their original dependencies during baking; animate their source points instead of trying to override a derived point directly.

The coordinate conversion is native `(x, y, z)` to Blender `(x, -z, y) / 100`. Native animation Y is up. Attack impulses use the separate native physics convention, so do not copy Blender coordinates directly into an impulse table.

## Retarget motion from another Blender armature

`Tools/Animation/RetargetCharacter.py` transfers an evaluated armature animation to the native point rig. Import your donor animation into Blender using its appropriate importer and save a `.blend` first. This tool reads that scene; it does not include FBX import, automatic bone matching, foot locking or a character controller.

Create a JSON mapping with `version: 1` and a `bindings` object. Map **every** native point whose XML `Type` is `Node` to a source pose-bone name. Do not map `MacroNode` or `CenterOfMass` helpers: their positions are calculated from their dependencies. Several points may use the same bone, allowing a rigid body segment to carry multiple landmarks. For example, this fragment shows the format; add the remaining native points before using it:

```json
{
  "version": 1,
  "bindings": {
    "NElbow_1": "forearm.L",
    "NElbow_2": "forearm.R"
  }
}
```

Choose a reference frame where the donor's pose is aligned with the native rig's reference pose. The tool calculates a bone-local offset for each native point at that frame. Sampling the reference frame therefore reproduces the native point positions; subsequent evaluated bone transforms carry those offsets. Bone rotation, constraints, object transforms and object animation participate. This preserves the initial SF2 proportions but does **not** guarantee constant edge lengths or planted feet during motion. Align the donor's facing, scale and pose before calibrating, and inspect bends and root travel afterward.

```powershell
& $blender --background Temp/Donor/donor.blend --python-exit-code 1 --python Tools/Animation/RetargetCharacter.py -- --rig Temp/CharacterCore/models/mdl_skeleton.xml --mapping Temp/Donor/bindings.json --armature Armature --reference-frame 1 --start 1 --end 60 --scale 100 --output Temp/Donor/retargeted
python Tools/Animation/CharacterPipeline.py validate --rig Temp/CharacterCore/models/mdl_skeleton.xml --animation Temp/Donor/retargeted/retargeted.bytes
python Tools/Animation/PackageCharacter.py --rig Temp/CharacterCore/models/mdl_skeleton.xml --animation Temp/Donor/retargeted/retargeted.bytes --mod-id local.retarget-preview --output Temp/Donor/local.retarget-preview
```

`--scale` defaults to **100 native units per Blender unit**. Use `1` for a donor already using Gymnast's native scene scale. The coordinate conversion is `(Blender X, Blender Z, -Blender Y) × scale`; source object/root movement is retained. Start/end frames are inclusive, and the source scene's effective frame rate must be 1–240 fps. The reference frame may be outside the sampled range. The output is baked at 60 fps with `mid_frames = 0`, subject to the same frame/sample limits as the point baker.

The output directory must not exist. It contains `retargeted.bytes`, its rig fingerprint sidecar, sampled `frames.json`, an interactive `preview.html`, and `retarget.json` recording the bindings and sampling settings. Keep the original donor scene separately. Missing bones, incomplete or derived-point bindings, invalid coordinates and singular calibration transforms are rejected. The tool restores the scene's original frame after sampling and does not save or modify the source file.

Blender 3.6.23 tests exercise evaluated bone constraints, translation, calibration offsets, helper nodes, frame-rate conversion and invalid mappings. `Tools/Animation/TestRetargetPipeline.ps1` additionally creates a synthetic donor, retargets the canonical 67-point rig, validates the exported fingerprints, packages the character, executes its real Lua registrations and reads its 61-frame output through the recovered animation reader in Unity 2022.3.62f3. This verifies integration, not humanoid motion quality: arbitrary donor skeletons, deformation and in-game contact quality still require creator validation. See the [Gymnast packaging guide](../gymnast/) for the generated preview mod and multi-clip controls.

## Add a geometric skin

Place mesh objects in `SF2_Skins`. Apply mesh modifiers before export. Mesh faces are triangulated, and each vertex is attached to four non-coplanar rig landmarks using the native weighted-point calculation. The exporter chooses nearby landmarks automatically. To control an attachment, assign that vertex to exactly four positive vertex groups whose names match rig points. Group membership chooses the landmarks; the exporter computes the attachment weights from the rest pose rather than using the Blender weight values.

Coplanar landmarks and unstable attachments are rejected with the mesh and vertex identified. Use nearby landmarks spanning all three dimensions, then preview the skin during strong twists and bends. The export limit is 2,048 skin vertices; the composed model permits at most 4,096 points. Geometry uses native triangle figures and the game's shadow rendering; Blender UVs, textures, materials, and arbitrary bone-weight skinning are not exported.

The original body and template equipment remain present. A skin is an additional geometric layer, not a replacement for every equipped item. Change the character's typed equipment bindings when a different outfit or weapon is required.

## Export, validate, and preview

```powershell
& $blender --background Temp/MyCharacter/character.blend --python-exit-code 1 --python Tools/Animation/BlenderCharacter.py -- export --output Temp/MyCharacter/package
python Tools/Animation/CharacterPipeline.py validate --rig Temp/MyCharacter/package/assets/models/body.xml --skin Temp/MyCharacter/package/assets/models/skin.xml --animation Temp/MyCharacter/package/assets/animations/authored.bytes
```

Omit `--skin` when your scene contains no skin mesh. Repeat it for additional overlays. Export produces:

| File | Purpose |
| --- | --- |
| `assets/models/body.xml` or `body.modelz` | Native body rig and geometry. `.modelz` is gzip-compressed UTF-8 geometry; use the extensionless model ID in Lua. |
| `assets/models/skin.xml` or `skin.modelz` | Optional native geometric overlay. |
| `assets/animations/authored.bytes` | Baked native animation at 60 Hz, with `mid_frames = 0`. |
| `assets/animations/authored.rig.json` | Node order and rig/animation fingerprints for validation. |
| `frames.json` | Portable sampled animation source. |
| `preview.html` | Local interactive point-rig preview with playback and frame scrubbing. |
| `character.generated.lua` | Character and input-bound move registration module. |

Open `preview.html` locally. It previews motion and rig edges; native materials, skin deformation, contact physics, and move selection still require a game test. Re-exporting writes the named outputs. Use a fresh output directory if you removed a skin, so an older `skin.xml` is not mistaken for current output.

The validator checks model references, helper dependency order, finite coordinates, animation node counts, truncated payloads, and sidecar fingerprints. It rejects XML DTDs/entities. Model XML is limited to 16 MiB; animation export permits 2–36,000 frames and at most two million node samples. A changed rig or changed clip requires rebaking its sidecar.

Other authoring tools can produce `frames.json` with `version = 1`, `fps`, `names`, and `frames`. Each frame is an array of `[x,y,z]` points matching `names`. Names must match the rig exactly; the baker reorders them to native order and resamples input at 1–240 fps to 60 Hz:

```powershell
python Tools/Animation/CharacterPipeline.py bake --rig body.xml --source frames.json --output authored.bytes
python Tools/Animation/CharacterPipeline.py preview --rig body.xml --animation authored.bytes --output preview.html
```

## Install the character in a mod

Create a mod using the [editor starter](../vscode/) or an existing fight example. Its manifest needs `content.register` and a dependency on `core`. Copy the package's `assets` directory into the mod. Copy `character.generated.lua` to `scripts/character.lua`, then load it from `scripts/main.lua`:

```lua
local sf2 = require("sf2")
local authored = require("character")
-- In your existing fight registration:
-- warriors = { authored.warrior }
```

The generated module uses the core kung-fu template and binds the authored move to **Punch**, restricted to this character. It returns `warrior` and `move` handles. It is a movement preview without attack damage. Set a suitable tactic on the character if an AI opponent should choose it; a [programmable tactic](../../api/moves-and-tactics/#on_decide) receives it only when native conditions allow it.

To make the motion an attack, edit its existing registration: set `type = "ATTACK"`, choose a control, and add an attack interval. For example, this fragment assumes a clip long enough to include frame 12 and a leg motion that contacts during frames 6–8:

```lua
conditions = {
    { character = character },
    { keys = { { "Kick", press = "Tap" } } },
    { not_interval = "Uninterrupt" },
},
events = { "key_pressed" },
intervals = {
    { type = "Uninterrupt", from = 0, to = 12 },
    { type = "Attack", from = 6, to = 8, attack = {
        edges = { "ECalf_2" }, damage = 0.12,
        damage_type = "UnarmedDamage", hit = "High",
        impulse = { x = 245, z = 350 },
    } },
},

```

These are fields inside a move definition, not a standalone Lua script. Attack edges must exist on the composed fighter. [Moves and tactics](../../api/moves-and-tactics/) documents all field defaults and limits. Use ordinary [combat callbacks](../../api/combat-callbacks/) for procedural effects instead of adding an operation language to the move table.

## Verify in a fight

Check the character at idle, mirrored on each side, walking, attacking, hit, and knocked down. Confirm equipment follows the rig, the intended key selects the move only for this warrior, the attack deals damage during its intended frames, and unrelated fighters retain their controls. Test the skin under the largest bends. The repository's `Tools/Animation/TestCharacterPipeline.ps1` exercises Blender export, the unchanged native animation reader, and Lua/native-XML projection; those checks cannot establish visual or combat correctness for your authored content.
