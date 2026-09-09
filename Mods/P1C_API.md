# Eclipse Mod API Phase 1C

Phase 1C exposes progression and acquisition structure without exposing the recovered runtime object graph or allowing mods to redefine shared economy.

## Non-equipment items

`sf2.items.register_consumable`, `register_free`, and `register_seal` create ordinary typed definitions. They support display localization, optional sprite/model handles, subtype, pack label, silent receive, and spend-after-use. The recovered `ItemInfo` parser remains authoritative when the definitions are materialized.

`sf2.shop.set_availability` is the shared non-economic availability overlay. `INHERIT` keeps the recovered active/hidden/group result, `FORCE_VISIBLE` makes an item permanently visible, and `FORCE_HIDDEN` suppresses it. `required_group` can add a roster group prerequisite. Shop UI and quest `Availability` conditions use the same evaluator.

No price, currency, delivery cost, or shared progression formula is accepted by these APIs. Existing equipment shop listings retain their separate price API for mod-owned equipment.

## Item sets

`sf2.itemsets.register` accepts title/text/brief localization handles and an ordered member list. Completion remains the recovered ownership check, so save persistence is inherited from existing `UserItem` ownership. Empty sets are rejected because the recovered `ItemSet` class treats them as assembled and contains no authoritative ability/effect field.

## Perk-tree overlays

`sf2.progression.replace_perk_branch` replaces exactly one level branch before `PerkTree.LJHPGKAOIAE()` materializes profile choices. Entries use `UNLOCK` or `UPGRADE` and perk handles. The adapter records and restores the previous recovered branch. XP thresholds, currencies, and level formulas stay base-owned.

## Forge structural families

`sf2.forge.profile("Simple")` returns an opaque handle to a host-owned economic profile projected from the loaded base `forge.xml`. `sf2.forge.register_recipe` accepts structural item bindings and candidate eligibility, including item type, required enchantment count, UI bar scale, aspect deviation mode, and candidate level ranges.

External families borrow the selected profile's existing `RecipePrices` objects and price-block bindings. Lua cannot create or edit costs, currencies, delivery timers, skip prices, or scaling. A requested item type is rejected if the chosen host profile has no matching price binding.

Example shape:

```lua
local sf2 = require("sf2")
local complex = sf2.forge.profile("Complex")
local perk = sf2.perks.get("core:perks/PERK_EXAMPLE")

sf2.forge.register_recipe {
    id = "late_complex",
    economic_profile = complex,
    items = {
        { equipment = sf2.forge.WEAPON, enchantments = 1, random_aspect = false }
    },
    candidates = {
        { perk = perk, equipment = sf2.forge.WEAPON, min_level = 20, max_level = 52 }
    }
}
```

The profile handle is intentionally opaque. It is a policy boundary, not a shortcut to recovered `RecipePrice`, `Roster`, or XML objects.
