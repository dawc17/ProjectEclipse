local sf2 = require("sf2")

local armor = sf2.items.register_armor {
    id = "eclipse_mantle",
    display_name = sf2.localization.key("armor.eclipse_mantle"),
    icon = sf2.assets.sprite("core:UI/Items/Armor12.img_armor_mantle_of_night"),
    model = sf2.assets.model("core:gamedata/models/mdl_armor_mantle_of_night"),
}

local helm = sf2.items.register_helm {
    id = "eclipse_pumpkin",
    display_name = sf2.localization.key("helm.eclipse_pumpkin"),
    icon = sf2.assets.sprite("core:UI/Items/Helm31.img_helm_hw14_pumpkin"),
    model = sf2.assets.model("core:gamedata/models/mdl_helm_hw14_pumpkin"),
}

local ranged = sf2.items.register_ranged {
    id = "eclipse_skull",
    display_name = sf2.localization.key("ranged.eclipse_skull"),
    icon = sf2.assets.sprite("core:UI/Items/Ranged12.img_ranged_hw15_skull"),
    model = sf2.assets.model("core:gamedata/models/mdl_ranged_hw15_skull"),
    subtype = "Skull",
}

local magic = sf2.items.register_magic {
    id = "eclipse_asteroid",
    display_name = sf2.localization.key("magic.eclipse_asteroid"),
    icon = sf2.assets.sprite("core:UI/Items/Magic4.img_magic_asteroid"),
    model = sf2.assets.model("core:gamedata/models/mdl_magic_asteroid"),
    subtype = "MagicAsteroid",
}

sf2.shop.addItem { section = sf2.shop.ARMOR, item = armor, level = 2, price = sf2.price.coins(1) }
sf2.shop.addItem { section = sf2.shop.HELMETS, item = helm, level = 2, price = sf2.price.coins(1) }
sf2.shop.addItem { section = sf2.shop.RANGED, item = ranged, level = 6, price = sf2.price.coins(1) }
sf2.shop.addItem { section = sf2.shop.MAGIC, item = magic, level = 6, price = sf2.price.coins(1) }

sf2.log.info("registered Eclipse loadout: armor, helm, ranged, magic")
