local sf2 = require("sf2")

-- Each translation belongs to DE128 and is authored through the public Lua API.
local title = sf2.localization.register {
    id = "item.titans_desolator",
    language = "eng",
    value = "Titan's Desolator",
}

local desolator = sf2.items.register_weapon {
    id = "titans_desolator",
    display_name = title,
    icon = sf2.assets.sprite("core:UI/Items/Weapon17.img_weapon_boss_giant_sword"),
    model = sf2.assets.model("core:gamedata/models/mdl_weapon_giant_sword"),
    subtype = "TitanGiantSword",
}

sf2.items.set_innate_perks {
    item = desolator,
    entries = {
        { perk = sf2.perks.get("core:perks/PERK_TITAN") },
        { perk = sf2.perks.get("core:perks/PERK_ANTI_SHOCK") },
    },
}

-- The final Eclipse Titan reward configures the level and Lifesteal on acquisition.
-- Equipment remains unlisted and has no purchase price.
return { titans_desolator = desolator }
