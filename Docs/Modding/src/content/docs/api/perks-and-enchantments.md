---
title: Perks and enchantments
description: Attach reusable Lua behavior to learned perks or forgeable equipment enchantments.
---

A **perk** is an ability definition used by learned player abilities or opponent
loadouts. An **enchantment** is an equipment effect available through a forge
recipe. Both can reuse the same behavior with different parameter values.

For native template-backed attribute modifiers, expiry removes the normalized
amount originally applied. It does not reevaluate the attribute expression against
later combat state. For example, a temporary +12 modifier removes 12 even if its
expression would now evaluate to 30; unrelated attribute changes remain intact.
This bookkeeping does not provide runtime character-form switching.

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
loadouts, fight rules, and forge candidates.

## sf2.perks.register

Register a new perk backed by your own Lua behavior.

**Signature:** `sf2.perks.register(definition)`

**Requires:** `content.register`, plus the capabilities used by its behavior.

**When:** Entrypoint, after the behavior it uses is available.

**Returns:** A perk handle. Registration does not automatically teach it to the player.

| Field | Type | Requirement/default |
| --- | --- | --- |
| `id` | String | Required local ID. |
| `display_name` | Localization handle | Required. |
| `description` | Localization handle | Required. |
| `icon` | Sprite handle | Optional. |
| `behavior` | Behavior handle | Required. |
| `kind` | Perk constant | Required: `sf2.perks.SINGLE` or `sf2.perks.COMBO`. |
| `parameters` | Typed value table | Optional; must satisfy the behavior schema. |
| `upgrades` | Dense upgrade array | Optional; 1–100 contiguous levels starting at 1. |
| `initial_upgrade` | Integer | Optional, default `0`. Requires `upgrades`; must be from 0 through the declared upgrade count. |

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

### Upgrade entries

Perk registration accepts `upgrades`, a dense array of
1–100 tables: `{ level, description?, parameters? }`. Levels must be ordered and
contiguous starting at 1. Level 0 uses the base perk. Omitted descriptions use the
base description; supplied descriptions must be localization handles owned by
this mod. An upgrade's parameters override the base independently, not the
previous upgrade. Parameters use the behavior schema.

`initial_upgrade` controls which declared rank is projected as the native baseline
for a newly registered learned perk. `0` keeps the base perk plus all declared
upgrade variants. A value from `1` through the upgrade count selects that rank as
the initial native variant and omits the lower baseline/variants from the native
projection. It does not rewrite existing saved learned-perk ranks; saved
`UpgradeLevel` values keep their normal meaning.

```lua
-- Fields to include in sf2.perks.register; Drain must be in the behavior schema.
upgrades = {
    { level = 1, parameters = { Drain = 0.02 } },
    { level = 2, parameters = { Drain = 0.04 } },
}
```

Register unlock/upgrade choices separately with
`sf2.progression.replace_perk_branch`. These entries do not change XP, currency,
upgrade costs or the global progression curve. Native progression variants supply
the selected description. Learned Lua perks use the saved `UpgradeLevel` to
apply parameter overrides at callback time. Saved parameter records and behavior
state are preserved; the effective map is a fresh copy. Unknown levels beyond the
registered upgrades and malformed saved levels report an error without modifying
the save. Perks without upgrade entries retain their existing parameter behavior.

The complete `Mods/example.perk-upgrades` mod provides a learned guard with three
upgrades, localized descriptions, and level 2–5 branch choices. Its managed tests
exercise actual Lua damage callbacks across save/reload and context recreation;
full Unity profile selection and encounter validation remains pending.

Equipment and warrior callbacks continue to use base parameters; they do not
inherit the player's learned upgrade level. Disabling a mod removes its native
variants and leaves its saved learned perk data intact. Learned behaviors may use
the documented style, combo and hit-phase callbacks when their archived behavior
depends on those native events.

## sf2.enchantments.register

Create a forgeable enchantment for selected equipment categories.

**Signature:** `sf2.enchantments.register(definition)`

**Requires:** `content.register`, plus the capabilities used by its behavior.

**When:** Entrypoint, after registering its behavior.

**Returns:** An enchantment handle. The player still needs to forge and equip it.

| Field | Type | Requirement/default |
| --- | --- | --- |
| `id` | String | Required local ID. |
| `recipe` | Recipe constant | Required: `SIMPLE`, `MEDIUM`, or `COMPLEX` from `sf2.enchantments`. |
| `item_types` | Equipment constant array | Required nonempty list; no duplicates. |
| `behavior` | Behavior handle | Required. |
| `display_name`, `description` | Localization handles | Required. |
| `icon` | Sprite handle | Optional. |
| `parameters` | Typed value table | Optional; must satisfy its schema. |

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
namespace.

Recipe selection uses the host's forge economy. These fields do not customize
shared prices, currencies, or upgrade formulas.
