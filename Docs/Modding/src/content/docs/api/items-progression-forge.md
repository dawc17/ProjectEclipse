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

## sf2.items.set_tactic_subtype

Replace the AI table group of a core or owned weapon without changing its animation subtype. Available since **API 0.52**.

**Signature:** `sf2.items.set_tactic_subtype { item = ItemHandle, group = string }`

**Returns:** Nothing (`nil`).

**When:** During entrypoint registration. The override applies when content is loaded, before subsequent fights are constructed.

**Requires:** `content.patch`; item lookup also requires `content.register`. Declare dependencies for referenced namespaces and require `api = ">=0.52 <1.0"`.

Both fields are required. `item` must resolve to a weapon. `group` is a native AI table group containing at most 128 ASCII letters, digits or underscores. An empty string explicitly selects the weapon's actual subtype instead of an existing separate AI group. It does not create AI tables or animation moves; supply a group supported by your tactics.

```lua
local sf2 = require("sf2")
sf2.items.set_tactic_subtype {
    item = sf2.items.get("core:items/weapon/WEAPON_TWO_HANDED_MACE"),
    group = "TwoHanded",
}
```

Only one override may target a weapon; duplicate registrations or another mod's override fail instead of silently winning by load order. This can compose with default enchantments and innate perks on the same item. An owned weapon must have a supported shop listing so its native item exists when applying content. Prefer the `tactic_subtype` registration field for a new weapon unless a separate override is needed.

The group participates in the content compatibility fingerprint. Disabling the mod and applying/restarting restores the original native group, and partial application failures restore earlier targets. Existing fight copies retain their snapshot; this operation does not refresh a running fighter or edit inventory saves. Native projection, rollback and Lua validation have automated coverage; physical combat acceptance remains a separate playtest.

## sf2.items.set_innate_perks

**Signature:** `sf2.items.set_innate_perks(definition)`

**Returns:** Nothing (`nil`).

**When:** During mod loading, before fighters are built. Available since API `0.50`.

**Requires:** `content.patch`; item/perk lookup or registration also requires `content.register`. Declare dependencies for referenced core or other-mod content.

Replaces the permanent perks attached to an equipment definition. The normal combat equipment collector includes these perks whenever that equipment is used. This applies to already owned equipment in subsequently built fighters too. It does not write an enchantment to inventory or change acquisition defaults.

| Field | Meaning |
| --- | --- |
| `item` | Required weapon, armor, helm, ranged or magic handle. |
| `entries` | Required dense array of zero to 64 perk entries. Empty removes the item's innate perks. |
| `entries[].perk` | Required existing perk handle, once per loadout. Register owned perks first. |
| `entries[].parameters` | Optional table of up to 64 named finite numbers. Defaults to empty, preserving the perk's authored defaults. |

Parameter names must begin with an ASCII letter or underscore, contain only ASCII letters, digits or underscores, and be at most 128 characters. Values are stored as single-precision numbers; values that overflow that format are rejected. Strings, expressions, booleans, NaN and infinity are not accepted. Use names understood by the chosen perk: this API does not invent behavior for unknown parameters.

```lua
local sf2 = require("sf2")
-- Manifest: api = ">=0.50 <1.0"
-- capabilities = ["content.register", "content.patch"]
-- Also declare the core dependency in mod.toml.
sf2.items.set_innate_perks {
    item = sf2.items.get("core:items/weapon/WEAPON_KNIVES"),
    entries = {
        { perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_PRECISION_WEAPON"),
          parameters = { Aspect = 100 } },
    },
}
```

This attaches the native precision perk to Knives as an innate equipment effect. It is separate from the forge enchantment saved on a particular inventory item. For custom procedures, register a Lua-backed perk using the perk API and attach its handle with no loadout parameters. Lua-backed perks take their initial parameters at perk registration; nonempty loadout parameters for them are rejected. Keep conditional behavior in their callbacks rather than encoding expressions in this table.

The target must be present in the applied native item catalog. Owned items need their supported shop registration so they are materialized. Perk instances are cloned per item, preserving the registry and other equipment from the combat collector's mutable weapon/non-weapon marker.

Two innate loadouts targeting the same item conflict. An innate loadout and a default-enchantment loadout can coexist on that item. Their entry order and parameter values participate in the compatibility fingerprint. Failed application restores earlier changes from that adapter; unloading restores the original innate list. Use Apply & Restart when changing active mods. Already constructed fighters are not refreshed by this operation.

Player-side Lua callbacks for active innate perks receive `fighter.source = "innate"`, `item_id`, `item_type` and `perk_id`. They do not require a saved inventory enchantment, including when a rule supplies the equipment. Native perk filtering still controls eligibility. One perk ID dispatches once per event across equipment: learned/profile perks with an available saved instance take precedence, then innate perks, then saved enchantments. A missing learned save node does not suppress an otherwise active innate perk. An innate instance retains state between events within its fight but has no persistent profile backing; use mod-owned saved state for progress that must survive fights. Opponent callbacks retain their existing `"warrior"` provenance.

This does not expose automatic activated abilities, arbitrary item metadata or permanent profile mutation. Native collection, ownership, rollback, player dispatch and Lua validation are tested; live combat behavior still needs acceptance testing with the chosen perk.

## sf2.items.set_default_enchantments

**Signature:** `sf2.items.set_default_enchantments(definition)`

**Returns:** Nothing (`nil`).

**When:** During mod loading. Available since API `0.49`.

**Requires:** `content.patch`; item/perk lookup or registration also requires `content.register`. Declare dependencies for referenced content from core or other mods.

Replaces an equipment item's default enchantment loadout. Shop previews and the native acquisition path use the replacement. This does not grant the item, re-enchant existing inventory, or attach permanent innate effects. Previously saved enchantments retain their values.

| Field | Meaning |
| --- | --- |
| `item` | Required equipment handle, from registration or `sf2.items.get`. Weapon, armor, helm, ranged and magic items are supported. |
| `entries` | Required dense array of zero to 64 entries. An empty array removes the acquisition defaults. |
| `entries[].perk` | Required existing perk handle. Each perk may appear only once. Register owned perks before this call. |
| `entries[].aspect` | Optional signed 32-bit integer, copied as the enchantment's explicit aspect. Omit it to preserve the perk's normal default. This is a value, not a formula or expression. |

```lua
local sf2 = require("sf2")
-- Manifest: api = ">=0.49 <1.0"
-- capabilities = ["content.register", "content.patch"]
-- Also declare the core dependency in mod.toml.
sf2.items.set_default_enchantments {
    item = sf2.items.get("core:items/weapon/WEAPON_KNIVES"),
    entries = {
        { perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_PRECISION_WEAPON"),
          aspect = 100 },
    },
}
```

Choose an aspect appropriate for your content; accepting an integer does not mean every value produces useful gameplay. Prices, equipment levels and shared stat scaling remain unchanged. The native acquisition path still determines when default enchantments are applied.

The target must exist in the applied native item catalog. For an owned item, use its supported shop registration so the adapter materializes it before applying the loadout. This call does not create a missing native item or substitute an acquisition route.

Two mods targeting the same item conflict, even if their loadouts match. Different items can coexist. Unknown fields, duplicate perks, sparse arrays and unresolved dependencies fail loading. A later native apply failure restores earlier loadouts from that adapter. Unloading restores the original preview and acquisition lists; use Apply & Restart after enabling or disabling the mod.

Loadout order, perk IDs and optional aspects participate in the content compatibility fingerprint. Changing defaults does not migrate existing equipment. Automated checks cover Lua registration, native projection and rollback; full-game acquisition, saves and preview rendering still need acceptance testing.

## sf2.forge.profile

**Signature:** `sf2.forge.profile(reference)`

**Returns:** A forge economy-profile handle.

**When:** During mod loading.

**Requires:** `content.register`, and a declared dependency when reading another mod's content.

Looks up an existing profile that supplies the forge's costs and timing. A short name such as `"Simple"` is resolved as `"core:forge-profiles/Simple"`. `"Complex"` is another existing core profile. Definition IDs normalize casing; an unknown profile fails loading.

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

Each `candidates` entry requires `perk` (a perk handle from `sf2.perks.register`) and `equipment`. Optional integer `min_level` and `max_level` restrict eligibility; omitting them leaves the candidate unrestricted by level. Use explicit sensible bounds for your content.

```lua
-- training_perk is a handle from sf2.perks.register.
local recipe = sf2.forge.register_recipe {
    id = "training_recipe",
    economic_profile = sf2.forge.profile("Simple"),
    items = { { equipment = sf2.forge.WEAPON, enchantments = 1 } },
    candidates = {
        { perk = training_perk, equipment = sf2.forge.WEAPON,
          min_level = 1, max_level = 52 },
    },
}
```

A registered enchantment must still be made obtainable through a recipe or another supported equipment/perk route before its combat behavior can run.

Registering a new family does not change the original family's candidate pool or costs.

## sf2.forge.exclude_candidate

**Signature:** `sf2.forge.exclude_candidate(definition)`

**Returns:** Nothing (`nil`).

**When:** During mod loading. Available since API `0.47`.

**Requires:** `content.patch`, `content.register` for handle lookups, and a declared `core` dependency.

Removes a native enchantment from one core recipe's eligible pool for one equipment category while the mod configuration is applied. Both candidate previews and subsequent rolls use this pool. It does not remove enchantments already on equipment, delete the perk, or change prices, timers or power deviations.

| Field | Meaning |
| --- | --- |
| `profile` | Required core profile handle from `sf2.forge.profile`. Selects the native recipe with that profile's name. |
| `perk` | Required existing core perk handle from `sf2.perks.get`. |
| `equipment` | Required category: `sf2.forge.WEAPON`, `ARMOR`, `HELM`, `RANGED`, or `MAGIC`. |

```lua
local sf2 = require("sf2")
-- Manifest: api = ">=0.47 <1.0"
-- capabilities = ["content.register", "content.patch"]
-- Also declare the core dependency in mod.toml.
sf2.forge.exclude_candidate {
    profile = sf2.forge.profile("Complex"),
    perk = sf2.perks.get("core:perks/PERK_MONK_SET_WHIRL"),
    equipment = sf2.forge.WEAPON,
}
```

This example excludes the Monk set enchantment from Complex weapon rolls. Other equipment categories remain unchanged. All native occurrences of that candidate are filtered; independently added mod candidates are unaffected.

Duplicate exclusions for the same recipe, equipment and perk conflict, including duplicates from different mods. Different targets can coexist. Unknown fields, invalid categories and non-core targets are rejected. Registration validates the handles; the native adapter additionally checks that the recipe supports the equipment and contains the candidate when applying content. Failure aborts application and restores exclusions already applied by that adapter.

Unloading the applied content restores the original native candidate pool. Enable or disable the mod through Apply & Restart to change the active configuration. This function does not yet edit native candidate level conditions, replace entire families or modify native recipe item settings.

## sf2.forge.override_deviation

**Signature:** `sf2.forge.override_deviation(definition)`

**Returns:** Nothing (`nil`).

**When:** During mod loading. Available since API `0.48`.

**Requires:** `content.patch`, `content.register` for the profile lookup, and a declared `core` dependency.

Changes the random enchantment-power delta for one equipment category in an existing core recipe. The game still calculates the base aspect from its normal level curve, then adds a random integer from `minimum` through `maximum`, inclusive. Prices, timers and the base curve remain unchanged. Already enchanted equipment keeps its existing power.

| Field | Meaning |
| --- | --- |
| `profile` | Required core recipe profile handle from `sf2.forge.profile`. |
| `equipment` | Required `sf2.forge.WEAPON`, `ARMOR`, `HELM`, `RANGED`, or `MAGIC`. |
| `minimum` | Required integer, at least `-10000` and no greater than `maximum`. |
| `maximum` | Required integer, at most `10000` and no less than `minimum`. |

The limits are validation bounds, not recommended balance values. Start close to the original recipe's range and test the resulting power at several equipment levels.

```lua
local sf2 = require("sf2")
-- Manifest: api = ">=0.48 <1.0"
-- capabilities = ["content.register", "content.patch"]
-- Also declare the core dependency in mod.toml.
sf2.forge.override_deviation {
    profile = sf2.forge.profile("Simple"),
    equipment = sf2.forge.WEAPON,
    minimum = 15,
    maximum = 75,
}
```

This replaces the Simple weapon category's native `-30..30` range with `15..75`. Both the effective recipe item settings and copied roll candidates use the override. Other equipment categories retain their original settings. The source recipe and perk definitions remain intact.

Only categories already using random aspect are supported. Simple and Medium have such categories; Complex's fixed-aspect categories are rejected when content is applied. A candidate's complete native `RandomAspect` expression is updated; fixed values, missing aspect values and compound expressions are preserved. Independently added candidates with a complete random-aspect expression use the same category override.

Two overrides targeting the same recipe/category conflict, even with equal bounds. Different categories compose, and candidate exclusions can coexist with a deviation override. Applying an invalid native target rolls back earlier forge changes from that adapter. Unloading restores the original settings; use Apply & Restart when changing the active mods.

The bounds participate in the content compatibility fingerprint. Changing them is a content change even if the mod's version is unchanged. This function does not replace entire recipe families, change fixed-aspect recipes into random ones, or edit arbitrary candidate conditions.
