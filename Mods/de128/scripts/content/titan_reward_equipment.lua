local sf2 = require("sf2")
local names = require("content.titan_reward_text")

-- The final Eclipse Titan reward is a player-owned copy of these hidden
-- opponent items. Separate identities leave Titan's own loadout unchanged.
local form = sf2.items.register_armor {
    id = "titans_form",
    display_name = names.BODY_TITAN,
    icon = sf2.assets.sprite("core:UI/Items/UnknownItems.img_armor_unknown"),
    model = sf2.assets.model("models/titan/mdl_body_titan"),
    initial_stats = { body_defense = 1, unarmed_damage = 1 },
}
local helm = sf2.items.register_helm {
    id = "titans_helm",
    display_name = names.HEAD_TITAN,
    icon = sf2.assets.sprite("core:UI/Items/UnknownItems.img_helm_unknown"),
    model = sf2.assets.model("models/titan/mdl_head_titan"),
    initial_stats = { head_defense = 1 },
}
local harpoon = sf2.items.register_ranged {
    id = "titans_harpoon",
    display_name = names.RANGED_TITANS_HARPOON,
    icon = sf2.assets.sprite("core:UI/Items/UnknownItems.img_ranged_unknown"),
    model = sf2.assets.model("models/titan/mdl_ranged_titans_harpoon"),
    subtype = "TitansHarpoon",
    initial_stats = { ranged_damage = 1 },
}
local mind_throw = sf2.items.register_magic {
    id = "titans_mind_throw",
    display_name = names.MAGIC_MIND_THROW,
    icon = sf2.assets.sprite("core:UI/Items/UnknownItems.img_magic_unknown"),
    model = sf2.assets.model("models/titan/mdl_magic_fireball"),
    subtype = "MindThrow",
    initial_stats = { magic_damage = 1 },
}

return { form = form, helm = helm, harpoon = harpoon, mind_throw = mind_throw }
