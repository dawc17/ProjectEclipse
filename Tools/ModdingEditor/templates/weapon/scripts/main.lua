local sf2 = require("sf2")

local weapon = sf2.items.register_weapon {
    id = "training_blade",
    display_name = sf2.localization.key("weapon.training_blade"),
    icon = sf2.assets.sprite("sprites/weapon"),
    model = sf2.assets.model("core:gamedata/models/mdl_weapon_katana_ritual"),
    subtype = "Katana",
    -- API >=0.51 also supports tactic_subtype for a different native AI table group.
}

sf2.shop.addItem {
    section = sf2.shop.WEAPONS,
    item = weapon,
    level = 1,
    price = sf2.price.coins(1),
}

sf2.log.info("Training Blade registered")
