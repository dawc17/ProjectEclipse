---
title: Replacing existing assets
description: Declare an explicit asset replacement and understand its scope, dependencies, and conflicts.
---

For a new item, simply point its icon/model field at your own asset. Use a global
replacement only when all supported loads of an existing asset should resolve
to your replacement after the game reloads.

## sf2.assets.replace

Redirect a supported asset ID to another asset owned by your mod.

**Signature:** `sf2.assets.replace { target, replacement }`

**Requires:** `assets.replace` and a declared dependency on the target's owner.

**When:** Entrypoint. Restart after enabling/disabling a replacement mod.

**Returns:** `nil`.

| Field | Type | Required? | Meaning |
| --- | --- | --- | --- |
| `target` | Asset ID string | Yes | Existing asset owned by a declared dependency. |
| `replacement` | Asset ID string | Yes | Existing asset owned by this mod. |

```lua
sf2.assets.replace {
    target = "core:UI/Skills/SkillsEnch02.EnchantmentFrenzy",
    replacement = "sprites/my_frenzy",
}
```

The strings omit file extensions. Both assets must exist and have the same
supported kind: **sprite, texture, model, or audio**. This function takes strings,
not the handles from `sf2.assets.sprite` or `sf2.assets.model`.

A second claim on the same target conflicts, even if it names the same replacement.
Cycles, wrong types, missing assets, or wrong ownership fail registration. The
base files are not overwritten. Disabling the replacement restores normal
resolution on reload; existing live objects are not retroactively rewritten.

The supported runtime loaders and core atlas members honor replacements.
Boot screens and arbitrary direct engine loads outside those loaders are not
covered. There is no configuration-text, arbitrary XML, deletion, or global
native-animation replacement escape hatch.

Exact existing `core:ui/...` Sprite resources are also supported when they are
stored outside the packaged art catalog. For example, menu art can target
`core:ui/atlases/MenuButtons.Dojo_normal`, and the two VS background panels can
target `core:ui/fullscreen/VS_Fon_left.img` and
`core:ui/fullscreen/VS_Fon_right.img`. These targets require a game build with
loose UI sprite support. Missing resource names still fail registration.
This does not add new menu states or replace arbitrary serialized sprite
references; the UI must load the sprite through its supported loaders.

A single replacement PNG can supply both VS panels through two sprite
descriptors with complementary `rect` crops. Their individual panel animations
continue to work. DE128 uses two 1280 × 1080 crops of its supplied 2560 × 1080
background. The same core panels are also used by the enemies screen.

## Keeping upscaled buttons the same size

An upscaled sprite needs a matching `pixels_per_unit` value. For example, when
replacing a 300 × 300 button at 100 pixels per unit with a 600 × 600 image, use
200 pixels per unit. This preserves its native UI size. Retain the original
normalized pivot, and check each resolution variant separately: a 150 × 150
variant at 100 pixels per unit needs 400 pixels per unit for that same 600 × 600
replacement. See [sprite descriptors](../sprites-and-textures/).

DE128 uses this approach for its supplied map buttons. Core atlas members use
`sf2.assets.replace`; DE128-owned raid and challenger icons retain their existing
sprite IDs and use updated textures and descriptors. Its `MAP_BUTTONS.json`
records the exact core targets and unused source images. Mod battle `icons`
support base, active, locked and locked-active states; there is currently no
separate pressed field for those mod-owned icons. Core pressed atlas members
can still be replaced through their existing asset IDs.
