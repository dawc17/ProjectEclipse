---
title: Reusing core equipment
description: Reference existing game items without copying or redefining them.
---

The built-in content owner is `core`. Declare a `core` dependency in your manifest before referring to its items or assets.

## Look up an existing item

```lua
local katana = sf2.items.get("core:items/weapon/weapon_katana")
```

The handle can be used where an item handle is expected, such as a reward, equipment rule, or item-set member. Looking up an item does not copy it or change its price, stats, or ownership.

IDs follow `core:items/<category>/<item-name>`. Categories are `weapon`, `armor`, `helm`, `ranged`, and `magic`. The registry also contains hidden/default definitions; existence in the registry does not imply that an item is normally sold.

## Keep items and assets separate

| Reference | What it identifies |
| --- | --- |
| `core:items/weapon/weapon_katana` | A complete equipment definition. |
| `core:gamedata/models/mdl_weapon_katana_ritual` | A model usable by a compatible new weapon. |
| `core:UI/Items/Armor12.img_armor_mantle_of_night` | A sprite in an existing atlas, usable as an icon. |

Use `sf2.items.get` for items, `sf2.assets.model` for models, and `sf2.assets.sprite` for sprites. These handles are not interchangeable. See [Assets](../assets/) and [Equipment](../equipment-shop-logging/) for their requirements.

## Find suitable references

Start with the [equipment example](https://github.com/dawc17/ProjectEclipse/tree/main/Mods/example.loadout), which pairs armor, helm, ranged, and magic icons with models and supported subtypes. To research other core equipment, use the canonical [vanilla XML](https://github.com/dawc17/ProjectEclipse/tree/main/Assets/vanillaXml) and validate the exact reference in Eclipse. Display names are translated text and are not reliable IDs.

Duplicate legacy names can have disambiguated IDs. The two ranged definitions named `GlaivebowArrow` resolve to `core:items/ranged/glaivebowarrow` and `core:items/ranged/glaivebowarrow/riflebullet`. Do not assume every legacy name can be converted by lowercasing it alone.
