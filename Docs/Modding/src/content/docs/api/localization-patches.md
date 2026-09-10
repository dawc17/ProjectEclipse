---
title: Localization
description: Give items readable names in different languages and patch existing translations.
---

Put your mod's translated text in `localizations/<language>.toml`. For example,
`localizations/eng.toml` can contain:

```toml
item.training_blade = "Training Blade"
perk.opening_charge = "Opening Charge"
perk.opening_charge.description = "Start with extra magic charge."
```

The key on the left stays the same in each language file. The value on the right
is what the player reads. This is a simple key/value format; keep entries on
single lines and use double-quoted values. See the [manifest guide](../../guides/manifest/)
for the surrounding folder layout.

## sf2.localization.key

Get the typed localization handle used by item names, perks, and achievements.

**Signature:** `sf2.localization.key(key)`

**Requires:** `content.register`; other owners must be declared dependencies.

**When:** Entrypoint, after localization files have been discovered by the host.

**Returns:** A localization handle. Missing keys raise an error.

`key` is your local key or a qualified localization definition ID.
The return value is not a translated string and cannot be substituted for fields
that explicitly require a plain string.

```lua
local name = sf2.localization.key("item.training_blade")
```

## sf2.localization.patch

Replace one language value of an existing localization definition.

**Signature:** `sf2.localization.patch { target, language, value }`

**Requires:** `content.patch`, plus a dependency on the target's owner.

**When:** Entrypoint.

**Returns:** `nil`.

| Field | Type | Required? | Meaning |
| --- | --- | --- | --- |
| `target` | String | Yes | Qualified localization definition ID. |
| `language` | String | Yes | Language code such as `eng`. |
| `value` | Nonempty string | Yes | Replacement text. |

```lua
sf2.localization.patch {
    target = "core:localization/weapon_nunchaku",
    language = "eng",
    value = "Nunchaku",
}
```

The target must exist in the exposed content registry. Two mods replacing the
same target and language conflict; changes to different languages can coexist.
Disabling the patching mod restores the original text after reload. This API
cannot replace arbitrary XML or modify prices and global balance settings.
