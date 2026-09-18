---
title: Localization
description: Give items readable names in different languages and patch existing translations.
---

You can register translations directly from the Lua entrypoint or keep them in
`localizations/<language>.toml`. TOML remains useful for larger translation sets.
For example, `localizations/eng.toml` can contain:

```toml
item.training_blade = "Training Blade"
perk.opening_charge = "Opening Charge"
perk.opening_charge.description = "Start with extra magic charge."
```

The key on the left stays the same in each language file. The value on the right
is what the player reads. This is a simple key/value format; keep entries on
single lines and use double-quoted values. See the [manifest guide](../../guides/manifest/)
for the surrounding folder layout.

## sf2.localization.register

Register one translation owned by your mod and get its localization handle.

**Signature:** `sf2.localization.register { id, language, value }`

**Returns:** A localization handle for the registered key.

**When:** Entrypoint.

**Requires:** `content.register`.

| Field | Type | Required? | Meaning |
| --- | --- | --- | --- |
| `id` | String | Yes | Local localization key owned by this mod, such as `item.desolator`. Do not qualify it with a namespace. |
| `language` | String | Yes | Language code such as `eng` or `pol`. It is trimmed and lowercased and may contain only letters, digits, underscores, or hyphens. |
| `value` | Nonempty string | Yes | Text for that language. |

Call the function once per language for the same `id`. The new key is available
immediately through `sf2.localization.key` and `sf2.localization.text`, including
before the registration transaction commits. Registration is transactional, so a
failed or rolled-back mod load does not leave the translation behind.

```lua
local desolatorName = sf2.localization.register {
    id = "item.desolator",
    language = "eng",
    value = "Desolator",
}

sf2.localization.register {
    id = "item.desolator",
    language = "pol",
    value = "Desolator",
}

local sameKey = sf2.localization.key("item.desolator")
```

Lua registration and TOML loading use the same ownership and duplicate rules.
Registering the same key and normalized language twice fails, including when one
copy comes from Lua and the other from a TOML file. There is no separate English
requirement for this function beyond the host's existing localization fallback
rules.

## sf2.localization.key

Get the typed localization handle used by item names, perks, and achievements.

**Signature:** `sf2.localization.key(key)`

**Requires:** `content.register`; other owners must be declared dependencies.

**When:** Entrypoint, after the key has been registered by Lua or discovered from localization files.

**Returns:** A localization handle. Missing keys raise an error.

`key` is your local key or a qualified localization definition ID.
The return value is not a translated string and cannot be substituted for fields
that explicitly require a plain string.

```lua
local name = sf2.localization.key("item.training_blade")
```

## sf2.localization.text

**Signature:** `sf2.localization.text(key, language?)`

**Returns:** A plain translated string. Resolution uses the requested language,
then `eng`, then an empty string if neither exists. Invalid handles or language
values raise an error.

**When:** Entrypoint or later callbacks. Obtain the handle with `key` during
registration and retain it for later reads. Each call reads current content,
including committed localization patches; it does not freeze a translation at
registration time. A patch made in the current transaction is also readable
before commit.

**Requires:** A localization handle created by this script context; no additional
capability for reading. Creating the handle requires `content.register` and the
usual dependency declaration when referencing another owner.

Omit `language` to use the current game language. Tool hosts without a language
provider use `eng`. An explicit language string is trimmed and lowercased; it
must contain only letters, digits, underscores or hyphens and cannot be empty.
Use game language codes such as `eng` or `pol`, matching localization filenames.

The returned string is a snapshot, not a widget binding. Resolve it again when
refreshing your UI after a language change. Formatting stays ordinary Lua; no
format expression runs inside the localization service. A translation used with
`string.format` must have matching placeholders in every language.

```lua
-- localizations/eng.toml: charge.status = "Charge: %d%%"
-- During registration:
local chargeText = sf2.localization.key("charge.status")

-- Inside your later UI refresh function (view is an open UI handle):
local label = string.format(sf2.localization.text(chargeText), 75)
sf2.ui.set_text(view, "status", label)
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
