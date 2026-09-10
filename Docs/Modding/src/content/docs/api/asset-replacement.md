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
