---
title: Item sets, perk choices, and forging
description: Group equipment into sets, change perk choices, and register enchantment recipes.
---

These functions run in your entry script. Declare a dependency on `core` when using its items, perks, or forge profiles. A *handle* is the Lua value returned by a lookup or registration function; pass that value directly instead of its name.

## sf2.itemsets.register

**Signature:** `sf2.itemsets.register(definition)`

**Returns:** An item-set handle.

**When:** During mod loading.

**Requires:** `content.register`.

Groups items into a named set. It does not automatically add a set bonus or grant the items to the player.

| Field | Meaning |
| --- | --- |
| `id` | Required local identifier, such as `training_set`. |
| `title`, `text`, `brief` | Required localization handles from `sf2.localization.key`. |
| `members` | Required, nonempty array of member tables. |

Each member requires an item handle in `item`. Optional presentation fields are `scale` (default `1`), `rotate`, `x`, `y`, and `icons_y` (all default `0`). Values must be finite numbers. Each item can appear only once in this set.

```lua
local katana = sf2.items.get("core:items/weapon/weapon_katana")
local set = sf2.itemsets.register {
    id = "training_set",
    title = sf2.localization.key("set.title"),
    text = sf2.localization.key("set.description"),
    brief = sf2.localization.key("set.brief"),
    members = { { item = katana, scale = 1 } },
}
```

Add the three localization keys to your mod's language file. Registering a set does not create a shop listing; use the shop API for items you want players to buy.

## sf2.progression.replace_perk_branch

**Signature:** `sf2.progression.replace_perk_branch(definition)`

**Returns:** Nothing (`nil`).

**When:** During mod loading, before the perk tree is used.

**Requires:** `content.patch`; looking up or registering perks also requires `content.register`.

Replaces the available choices at one perk-tree level. It does not immediately teach a perk, add experience, or change upgrade prices. Two mods replacing the same level conflict.

| Field | Meaning |
| --- | --- |
| `level` | Required integer from `1` to `52`. |
| `entries` | Required, nonempty array of perk choices. |
| `entries[].perk` | Required perk handle. |
| `entries[].action` | `"unlock"` (default) or `"upgrade"`. |

```lua
-- opening_focus is a perk handle registered earlier in this script.
sf2.progression.replace_perk_branch {
    level = 2,
    entries = { { perk = opening_focus, action = "unlock" } },
}
```

An `upgrade` entry must refer to a perk that supports upgrades. Keep alternative choices appropriate for players who have already progressed through the tree.

## sf2.forge.profile

**Signature:** `sf2.forge.profile(reference)`

**Returns:** A forge economy-profile handle.

**When:** During mod loading.

**Requires:** `content.register`, and a declared dependency when reading another mod's content.

Looks up an existing profile that supplies the forge's costs and timing. A short name such as `"Simple"` is resolved as `"core:forge-profiles/Simple"`. `"Complex"` is another existing core profile. Names are case-sensitive; an unknown profile fails loading.

```lua
local economy = sf2.forge.profile("Simple")
```

The handle is an input for a recipe. It is not a table of editable prices.

## sf2.forge.register_recipe

**Signature:** `sf2.forge.register_recipe(definition)`

**Returns:** A forge-recipe handle.

**When:** During mod loading.

**Requires:** `content.register`.

Creates a recipe using an existing economy profile and a pool of eligible enchantments.

| Field | Meaning |
| --- | --- |
| `id` | Required local identifier. |
| `alias` | Optional compatibility alias; defaults to an empty string. |
| `economic_profile` | Required handle from `sf2.forge.profile`. |
| `items` | Required array of equipment-category settings. |
| `candidates` | Required array of eligible perks/enchantments. |

Equipment categories are `sf2.forge.WEAPON`, `ARMOR`, `HELM`, `RANGED`, and `MAGIC` (string values `"weapon"`, `"armor"`, `"helm"`, `"ranged"`, `"magic"`).

Each `items` entry requires `equipment`. Optional fields are `enchantments` (integer, default `1`), `bar_scale` (string, default empty), `min_deviation` and `max_deviation` (integers, default `0`), and `random_aspect` (boolean, default `false`). These describe the recipe's enchantment presentation and roll behavior; they are not currency prices.

Each `candidates` entry requires `perk` (a perk or enchantment handle) and `equipment`. Optional integer `min_level` and `max_level` restrict eligibility; omitting them leaves the candidate unrestricted by level. Use explicit sensible bounds for your content.

```lua
-- training_enchantment is a handle from sf2.enchantments.register.
local recipe = sf2.forge.register_recipe {
    id = "training_recipe",
    economic_profile = sf2.forge.profile("Simple"),
    items = { { equipment = sf2.forge.WEAPON, enchantments = 1 } },
    candidates = {
        { perk = training_enchantment, equipment = sf2.forge.WEAPON,
          min_level = 1, max_level = 52 },
    },
}
```

A registered enchantment must still be made obtainable through a recipe or another supported equipment/perk route before its combat behavior can run.
