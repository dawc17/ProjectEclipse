local sf2 = require("sf2")

local weapon = sf2.items.register_weapon {
    id = "training_blade",
    display_name = sf2.localization.key("weapon.training_blade"),
    icon = sf2.assets.sprite("sprites/weapon"),
    model = sf2.assets.model("core:gamedata/models/mdl_weapon_katana_ritual"),
    subtype = "Katana",
    -- Omit initial_stats for normal power; { weapon_damage = 0 } stores zero,
    -- while {} leaves initial damage absent. Normal upgrades still apply.
    -- Optional tactic_subtype selects a different native AI table group.
}

sf2.shop.addItem {
    section = sf2.shop.WEAPONS,
    item = weapon,
    level = 1,
    price = sf2.price.coins(1),
}

-- To change an existing item's starting level, stats, upgrade template or legacy
-- paid marker, use
-- sf2.items.set_initial_profile in a mod with content.patch. The equipment
-- registration above already accepts initial_stats for new items.
-- Existing equipment can also use sf2.shop.set_price { item = handle,
-- price = sf2.price.gems(39) } with content.patch. Use shop.set_availability
-- separately if the base item is hidden.
-- To change an existing equipment icon or model, use
-- sf2.items.set_presentation { item = handle, icon = sf2.assets.sprite("sprites/weapon") }.
-- shop.set_price may include secondary_price in the other currency.

sf2.log.info("Training Blade registered")
