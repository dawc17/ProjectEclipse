local sf2 = require("sf2")

local lifesteal = sf2.perks.register {
    id = "eclipse_lifesteal",
    template = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_LIFESTEAL_WEAPON"),
    display_name = sf2.localization.key("perk.eclipse_lifesteal"),
    description = sf2.localization.key("perk.eclipse_lifesteal.description"),
    parameters = {
        Chance = 1,
    },
}

sf2.enchantments.register {
    id = "eclipse_lifesteal_weapon",
    perk = lifesteal,
    recipe = sf2.enchantments.MEDIUM,
    item_types = { sf2.enchantments.WEAPON },
}

sf2.log.info("registered template-derived Eclipse Lifesteal enchantment")
