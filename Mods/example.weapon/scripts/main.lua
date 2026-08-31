local sf2 = require("sf2")

local weapon = sf2.items.register_weapon {
    id = "example_blade",
    display_name = sf2.localization.key("weapon.example_blade"),
    icon = sf2.assets.sprite("sprites/weapon"),
    model = sf2.assets.model("core:gamedata/models/mdl_weapon_katana_ritual"),
    subtype = "Katana",
    damage = 9999,
}

sf2.shop.add {
    section = sf2.shop.WEAPONS,
    item = weapon,
    level = 1,
    price = sf2.price.coins(1),
}

sf2.mod.log("registered Example Blade")

