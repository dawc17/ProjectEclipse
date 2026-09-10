---
title: Perks and enchantments
description: Attach reusable Lua behavior to learned perks or forgeable equipment enchantments.
---

A **perk** is an ability definition used by learned player abilities or opponent
loadouts. An **enchantment** is an equipment effect available through a forge
recipe. Both can reuse the same behavior with different parameter values.

The examples below assume `sf2` is loaded, the localization keys exist, and
`opening_charge` is the behavior created in [Reusable behaviors](../behavior-instances/).

## sf2.perks.get

Look up an existing perk definition.

**Signature:** `sf2.perks.get(reference)`

**Requires:** `content.register`; a dependency on another owner when applicable.

**When:** Entrypoint.

**Returns:** A perk handle. Missing or inaccessible definitions raise errors.

```lua
local lifesteal = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_LIFESTEAL_WEAPON")
```

Use the complete perk definition ID. The returned handle is useful in opponent
loadouts, fight rules, forge candidates, and the template compatibility form.

## sf2.perks.register

Register a new perk backed by your own Lua behavior or an existing perk template.

**Signature:** `sf2.perks.register(definition)`

**Requires:** `content.register`, plus the capabilities used by its behavior.

**When:** Entrypoint, after the behavior or template is available.

**Returns:** A perk handle. Registration does not automatically teach it to the player.

| Field | Type | Requirement/default |
| --- | --- | --- |
| `id` | String | Required local ID. |
| `display_name` | Localization handle | Required. |
| `description` | Localization handle | Required. |
| `icon` | Sprite handle | Optional. |
| `behavior` | Behavior handle | Required for a new Lua-backed perk; mutually exclusive with `template`. |
| `kind` | Perk constant | Required with `behavior`: `sf2.perks.SINGLE` or `sf2.perks.COMBO`. |
| `parameters` | Typed value table | Optional; must satisfy the behavior schema. |
| `template` | Perk handle | Legacy alternative to `behavior`; omit `kind` in this form. |

```lua
local focus = sf2.perks.register {
    id = "opening_focus",
    display_name = sf2.localization.key("perk.opening_charge"),
    description = sf2.localization.key("perk.opening_charge.description"),
    kind = sf2.perks.SINGLE,
    behavior = opening_charge,
    parameters = { amount = 0.35 },
}
```

To make it active, add it to an opponent's `perks`, a supported fight rule, or a
[perk-tree choice](../items-progression-forge/#sf2progressionreplace_perk_branch).
Player callbacks run only for learned, active perks after normal rule filtering.

In the legacy form, `template` copies an existing template and `parameters`
contains scalar template overrides. It is a compatibility mechanism, not a way
to expose an arbitrary C# object. For custom logic, use `behavior`.

## sf2.enchantments.register

Create a forgeable enchantment for selected equipment categories.

**Signature:** `sf2.enchantments.register(definition)`

**Requires:** `content.register`, plus the capabilities used by its behavior.

**When:** Entrypoint, after registering its behavior or looking up its legacy perk.

**Returns:** An enchantment handle. The player still needs to forge and equip it.

| Field | Type | Requirement/default |
| --- | --- | --- |
| `id` | String | Required local ID. |
| `recipe` | Recipe constant | Required: `SIMPLE`, `MEDIUM`, or `COMPLEX` from `sf2.enchantments`. |
| `item_types` | Equipment constant array | Required nonempty list; no duplicates. |
| `behavior` | Behavior handle | New Lua-backed form; mutually exclusive with `perk`. |
| `display_name`, `description` | Localization handles | Required with `behavior`. |
| `icon` | Sprite handle | Optional with `behavior`. |
| `parameters` | Typed value table | Optional with `behavior`; must satisfy its schema. |
| `perk` | Template-backed perk handle | Legacy alternative; direct presentation/parameter fields are forbidden. |

```lua
local enchantment = sf2.enchantments.register {
    id = "charged_weapon",
    display_name = sf2.localization.key("perk.opening_charge"),
    description = sf2.localization.key("perk.opening_charge.description"),
    behavior = opening_charge,
    parameters = { amount = 0.35 },
    recipe = sf2.enchantments.SIMPLE,
    item_types = { sf2.enchantments.WEAPON },
}
```

Other equipment constants are `ARMOR`, `HELM`, `RANGED`, and `MAGIC` in the same
namespace. The legacy `perk` form requires a template-backed perk; a Lua-backed
perk cannot be converted into that legacy enchantment form. Use `behavior`
directly to share programmable logic.

Recipe selection uses the host's forge economy. These fields do not customize
shared prices, currencies, or upgrade formulas.
