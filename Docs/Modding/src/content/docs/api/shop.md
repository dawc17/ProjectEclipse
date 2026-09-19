---
title: Shops and prices
description: Put your equipment in the shop, choose its starting level and price, and control visibility.
---

Register an item first, then give its handle to the shop. Examples on this page
assume `sf2` is loaded and `weapon` is your registered weapon handle from
[Your first weapon](../../guides/first-weapon/).

## sf2.shop.addItem

Add a shop listing for equipment owned by your mod.

**Signature:** `sf2.shop.addItem { section, item, level, price }`

**Requires:** `content.register`.

**When:** Entrypoint, after registering the item.

**Returns:** The listing's qualified ID as a string.

| Field | Type | Required? | Meaning |
| --- | --- | --- | --- |
| `section` | Shop constant | Yes | Must match the equipment category. |
| `item` | Item handle | Yes | Equipment owned by this mod. |
| `level` | Integer | Yes | Starting level used for normal vanilla power progression. |
| `price` | Price handle | Yes | Returned by `sf2.price.coins` or `sf2.price.gems`. |

```lua
local listing_id = sf2.shop.addItem {
    section = sf2.shop.WEAPONS,
    item = weapon,
    level = 1,
    price = sf2.price.coins(1000),
}
```

| Equipment | Section | Supported starting levels |
| --- | --- | --- |
| Weapon | `sf2.shop.WEAPONS` | 1–52 |
| Armor | `sf2.shop.ARMOR` | 2–52 |
| Helm | `sf2.shop.HELMETS` | 2–52 |
| Ranged | `sf2.shop.RANGED` | 6–52 |
| Magic | `sf2.shop.MAGIC` | 6–52 |

Wrong categories, unsupported levels, duplicate listings, or another mod's item
are errors. This API does not change the prices of existing core items.
An equipment definition's explicit [`initial_stats`](../equipment-shop-logging/#explicit-initial-stats)
replaces its initial level-derived power; the listing still sets its level, price
and starting upgrade level.

## sf2.price.coins

Create a coin price to use in a shop listing. This does not spend or grant coins.

**Signature:** `sf2.price.coins(amount)`

**Requires:** No capability to construct the price; listing it requires `content.register`.

**When:** Usually during registration.

**Returns:** An opaque price handle.

`amount` must be an integer from 0 through 2,147,483,647. Negative values,
fractions, infinity, and NaN are rejected.

```lua
local price = sf2.price.coins(500)
```

## sf2.price.gems

Create a gem price to use in a shop listing.

**Signature:** `sf2.price.gems(amount)`

**Requires:** No capability to construct the price.

**When:** Usually during registration.

**Returns:** An opaque price handle. The same integer bounds as coin prices apply.

```lua
local price = sf2.price.gems(10)
```

This describes your new listing's cost. It is not a currency balance operation.

## sf2.shop.set_availability

Apply a non-economic visibility policy to an existing item.

**Signature:** `sf2.shop.set_availability { item, visibility?, required_group?, minimum_level? }`

**Requires:** `content.patch`; a declared dependency when targeting another owner.
Obtaining an item handle with `sf2.items.get` also requires `content.register`.

**When:** Entrypoint, after obtaining the item handle.

**Returns:** `nil`.

| Field | Type | Default | Meaning |
| --- | --- | --- | --- |
| `item` | Item handle | Required | Item whose visibility is controlled. |
| `visibility` | Constant | `sf2.shop.INHERIT` | Preserve existing visibility, force visible, or force hidden. |
| `required_group` | String | `""` | Optional player-group prerequisite. |
| `minimum_level` | Integer 0–52 | `0` | Player level needed for shop availability; zero adds no level restriction. |

```lua
sf2.shop.set_availability {
    item = weapon,
    visibility = sf2.shop.FORCE_VISIBLE,
    minimum_level = 15,
}
```

Other choices are `sf2.shop.FORCE_HIDDEN` and `sf2.shop.INHERIT`. The shop and
availability checks share this policy. Competing policies for the same item are
errors; the API cannot modify price, currency, or progression formulas.

The level requirement applies to all visibility modes. `FORCE_VISIBLE` bypasses
the item's original hidden and group restrictions, but still requires this
policy's `required_group` and `minimum_level`. `INHERIT` retains the original
restrictions and adds these requirements. `FORCE_HIDDEN` always hides the item.
The shop list and native item-availability queries use the same policy.

For mod-owned equipment, a nonempty `required_group` also supplies its native
shop pack label. Quest actions that unlock that group can therefore mark the
new equipment for shop notifications. Core items retain their recovered labels;
this policy does not rewrite them.

This controls listing eligibility and group-unlock notification membership only. It does not change the item's own level,
power, upgrade rules, price, ownership or equipment use, and does not grant an
item. An already-owned item stays owned. Removing the mod through Apply & Restart
restores the base visibility rules. The policy's level is included in the content
fingerprint; existing declarations that omit it retain their previous behavior.
