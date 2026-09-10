---
title: Equipment and item functions
description: Register every equipment category and non-equipment item, look up existing items, and preserve saved item IDs.
---

An **item handle** identifies an item definition. It does not represent an item
already owned by the player. Registering equipment does not give it to the
player or automatically put it in the shop; use a listing or a reward next.

All registration functions below run in the entrypoint and require
`content.register`. Unknown fields, duplicate IDs, wrong handle types, and
missing dependencies are errors. Examples assume `local sf2 = require("sf2")`.

## Equipment fields

| Field | Type | Required? | Meaning |
| --- | --- | --- | --- |
| `id` | String | Yes | Stable local ID such as `my_blade`; do not include your mod namespace. |
| `display_name` | Localization handle | Yes | Obtain with `sf2.localization.key`. |
| `icon` | Sprite handle | Yes | Shop/inventory icon from `sf2.assets.sprite`. |
| `model` | Model handle | Yes | Equipped appearance from `sf2.assets.model`. |
| `subtype` | String | Category-dependent | Combat family; see each function below. |

Normal equipment takes no raw damage or defense field. Its initial power is
calculated from the category and shop listing's starting level. See
[shops and prices](../shop/) for section constants and allowed levels.
Create the referenced localization key in `localizations/eng.toml` before using
these examples. Core model/icon references require the `core` dependency.

## sf2.items.register_weapon

Create a new weapon definition owned by your mod.

**Signature:** `sf2.items.register_weapon { id, display_name, icon, model, subtype? }`

**Requires:** `content.register` and dependencies for referenced content.

**When:** Entrypoint, before listing or referencing this item.

**Returns:** An item handle with the weapon category.

Use the equipment fields above. Optional `subtype` defaults to `"Katana"`. Match it to the chosen model and move family.

```lua
local weapon = sf2.items.register_weapon {
    id = "my_weapon",
    display_name = sf2.localization.key("item.my_weapon"),
    icon = sf2.assets.sprite("sprites/weapon"),
    model = sf2.assets.model("core:gamedata/models/mdl_weapon_katana_ritual"),
    subtype = "Katana",
}
```

The matching shop section is `sf2.shop.WEAPONS`. A normal listing starts
at level 1 or later, up to level 52.

## sf2.items.register_armor

Create a new armor definition owned by your mod.

**Signature:** `sf2.items.register_armor { id, display_name, icon, model }`

**Requires:** `content.register` and dependencies for referenced content.

**When:** Entrypoint, before listing or referencing this item.

**Returns:** An item handle with the armor category.

Use the equipment fields above. No `subtype` field is accepted.

```lua
local armor = sf2.items.register_armor {
    id = "my_armor",
    display_name = sf2.localization.key("item.my_armor"),
    icon = sf2.assets.sprite("core:UI/Items/Armor12.img_armor_mantle_of_night"),
    model = sf2.assets.model("core:gamedata/models/mdl_armor_mantle_of_night"),
}
```

The matching shop section is `sf2.shop.ARMOR`. A normal listing starts
at level 2 or later, up to level 52.

## sf2.items.register_helm

Create a new helm definition owned by your mod.

**Signature:** `sf2.items.register_helm { id, display_name, icon, model }`

**Requires:** `content.register` and dependencies for referenced content.

**When:** Entrypoint, before listing or referencing this item.

**Returns:** An item handle with the helm category.

Use the equipment fields above. No `subtype` field is accepted.

```lua
local helm = sf2.items.register_helm {
    id = "my_helm",
    display_name = sf2.localization.key("item.my_helm"),
    icon = sf2.assets.sprite("core:UI/Items/Helm31.img_helm_hw14_pumpkin"),
    model = sf2.assets.model("core:gamedata/models/mdl_helm_hw14_pumpkin"),
}
```

The matching shop section is `sf2.shop.HELMETS`. A normal listing starts
at level 2 or later, up to level 52.

## sf2.items.register_ranged

Create a new ranged definition owned by your mod.

**Signature:** `sf2.items.register_ranged { id, display_name, icon, model, subtype }`

**Requires:** `content.register` and dependencies for referenced content.

**When:** Entrypoint, before listing or referencing this item.

**Returns:** An item handle with the ranged category.

Use the equipment fields above. `subtype` is required. It selects the ranged combat family; this example uses `"Skull"`.

```lua
local ranged = sf2.items.register_ranged {
    id = "my_ranged",
    display_name = sf2.localization.key("item.my_ranged"),
    icon = sf2.assets.sprite("core:UI/Items/Ranged12.img_ranged_hw15_skull"),
    model = sf2.assets.model("core:gamedata/models/mdl_ranged_hw15_skull"),
    subtype = "Skull",
}
```

The matching shop section is `sf2.shop.RANGED`. A normal listing starts
at level 6 or later, up to level 52.

## sf2.items.register_magic

Create a new magic definition owned by your mod.

**Signature:** `sf2.items.register_magic { id, display_name, icon, model, subtype }`

**Requires:** `content.register` and dependencies for referenced content.

**When:** Entrypoint, before listing or referencing this item.

**Returns:** An item handle with the magic category.

Use the equipment fields above. `subtype` is required. It selects the magic combat family; this example uses `"MagicAsteroid"`.

```lua
local magic = sf2.items.register_magic {
    id = "my_magic",
    display_name = sf2.localization.key("item.my_magic"),
    icon = sf2.assets.sprite("core:UI/Items/Magic4.img_magic_asteroid"),
    model = sf2.assets.model("core:gamedata/models/mdl_magic_asteroid"),
    subtype = "MagicAsteroid",
}
```

The matching shop section is `sf2.shop.MAGIC`. A normal listing starts
at level 6 or later, up to level 52.

## Non-equipment fields

The following three functions share this table shape. They create item
categories used by the game's content system; none adds an arbitrary Lua
`on_use` callback.

| Field | Type | Default | Meaning |
| --- | --- | --- | --- |
| `id` | String | Required | Local item ID. |
| `display_name` | Localization handle | Required | Display text. |
| `icon` | Sprite handle | Omitted | Optional icon. |
| `model` | Model handle | Omitted | Optional model. |
| `subtype` | String | `""` | Existing content subtype, when needed. |
| `pack_label` | String | `""` | Pack label passed to item presentation. |
| `silent_receive` | Boolean | `false` | Suppress ordinary receive presentation. |
| `spend_after_use` | Boolean | `false` | Request consumption through the item's native use path. |

## sf2.items.register_consumable

Create a consumable, such as a mod-owned entry ticket for a mode.

**Signature:** `sf2.items.register_consumable(definition)`

**Requires:** `content.register`.

**When:** Entrypoint.

**Returns:** An item handle for this non-equipment category.

`definition` uses the non-equipment fields above. There are no equipment power
or shop-price fields in this table.

```lua
local item = sf2.items.register_consumable {
    id = "my_consumable",
    display_name = sf2.localization.key("item.my_consumable"),
}
```

## sf2.items.register_free

Create a free-category item for content and reward definitions.

**Signature:** `sf2.items.register_free(definition)`

**Requires:** `content.register`.

**When:** Entrypoint.

**Returns:** An item handle for this non-equipment category.

`definition` uses the non-equipment fields above. There are no equipment power
or shop-price fields in this table.

```lua
local item = sf2.items.register_free {
    id = "my_free",
    display_name = sf2.localization.key("item.my_free"),
}
```

## sf2.items.register_seal

Create a seal-category item for content and reward definitions.

**Signature:** `sf2.items.register_seal(definition)`

**Requires:** `content.register`.

**When:** Entrypoint.

**Returns:** An item handle for this non-equipment category.

`definition` uses the non-equipment fields above. There are no equipment power
or shop-price fields in this table.

```lua
local item = sf2.items.register_seal {
    id = "my_seal",
    display_name = sf2.localization.key("item.my_seal"),
}
```

## sf2.items.get

Obtain a handle to an existing item definition, including a core item.

**Signature:** `sf2.items.get(reference)`

**Requires:** `content.register`; other owners must be declared dependencies.

**When:** Entrypoint, before using the handle in another definition.

**Returns:** An item handle. Missing IDs or references outside allowed ownership
raise errors; this function does not return `nil` for a missing item.

```lua
local katana = sf2.items.get("core:items/weapon/weapon_katana")
```

Use a complete definition path: `items/weapon/...`, not a model path or a raw
legacy item name. For your own item, retaining its registration return value is
usually simpler. See [finding core content](../core-equipment/).

## sf2.items.alias

Let an old saved item ID resolve to a current item definition.

**Signature:** `sf2.items.alias { from, to }`

**Requires:** `content.register`.

**When:** Entrypoint, after registering the replacement item.

**Returns:** `nil`.

`from` is the historical local item path **including its category**;
`to` is the current item handle. Both must belong to this mod and preserve the
equipment category. The alias does not rewrite the historical save ID.

```lua
-- weapon is the item handle registered above.
sf2.items.alias { from = "weapon/old_blade", to = weapon }
```

Do not point an old weapon at armor or another owner's definition. Aliases are
for deliberate published-ID changes, not for creating duplicate shop items.

## sf2.items.tombstone

Reserve an old item ID when it has been retired without a replacement.

**Signature:** `sf2.items.tombstone { id }`

**Requires:** `content.register`.

**When:** Entrypoint.

**Returns:** `nil`.

`id` is a historical local item path including its category. It remains reserved
so saved ownership cannot accidentally bind to unrelated future content.

```lua
sf2.items.tombstone { id = "weapon/retired_blade" }
```

The saved item record stays preserved and unavailable. A tombstone is not a
command to delete items from a player's save.
