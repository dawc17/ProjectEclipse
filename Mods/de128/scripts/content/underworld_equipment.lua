local sf2 = require("sf2")
local text = require("content.underworld_text")

-- Berstuuk's hidden opponent equipment. The archive defines both entries in
-- list.xml; their exact model geometry ships under assets/models/underworld/.
local form = sf2.items.register_armor {
    id = "berstuuk_form",
    display_name = text.BODY_BERSTUUK,
    icon = sf2.assets.sprite("core:UI/Items/UnknownItems.img_armor_unknown"),
    model = sf2.assets.model("models/underworld/mdl_body_berstuuk_early"),
    initial_stats = { body_defense = 1, unarmed_damage = 1 },
}
local mask = sf2.items.register_helm {
    id = "berstuuk_mask",
    display_name = text.HEAD_BERSTUUK,
    icon = sf2.assets.sprite("core:UI/Items/UnknownItems.img_helm_unknown"),
    model = sf2.assets.model("models/underworld/mdl_head_berstuuk"),
}

return { form = form, mask = mask }
