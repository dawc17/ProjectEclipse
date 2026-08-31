local sf2 = require("sf2")

local weapon = sf2.items.register_weapon {
    id = "example_blade",
    display_name = sf2.localization.key("weapon.example_blade"),
    icon = sf2.assets.sprite("sprites/weapon"),
    model = sf2.assets.model("core:gamedata/models/mdl_weapon_katana_ritual"),
    subtype = "Katana",
}

-- Save-compatible identity evolution examples. An old save that still owns
-- example.weapon:items/weapon/example_blade_legacy resolves to this current item
-- without rewriting the historical XML ID. A tombstone reserves an intentionally
-- retired ID while keeping any old ownership record preserved as unavailable.
sf2.items.alias { from = "weapon/example_blade_legacy", to = weapon }
sf2.items.tombstone { id = "weapon/example_blade_retired" }

sf2.shop.addItem {
    section = sf2.shop.WEAPONS,
    item = weapon,
    level = 1,
    price = sf2.price.coins(1),
}

sf2.log.info("registered Example Blade")

