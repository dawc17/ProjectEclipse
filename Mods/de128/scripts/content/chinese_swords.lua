local sf2 = require("sf2")
local data = require("content.chinese_swords_data")
local animation = sf2.assets.binary("animations/chinese_swords_super_slash_old")

data.profile.display_name = sf2.localization.register {
    id = "move.chinese_swords_super_slash", language = "eng", value = "Super Slash",
}

for _, move in ipairs(data.item_lock_extensions) do
    sf2.moves.extend_item_lock {
        move = move, item_type = "Weapon", source_subtype = "Sai", subtype = "ChineseSwords",
    }
end

local slash = sf2.moves.register {
    id = "chinese_swords_super_slash", animation = animation,
    core_templates = { "3key", "Forward", "Weapon", "Jump", "Controlled", "NotTitan", "SoundStrike" },
    type = "ATTACK", mid_frames = 2, first_frame = 4, priority = 130,
    tactic_weapon = "HermitSwords", mirror_node = "NHeel_1",
    profile = data.profile, tactic_distance = data.tactic_distance,
    conditions = data.conditions, locks = data.locks, intervals = data.intervals,
    transitions = data.transitions, align = data.align, direction = data.direction,
    actions = data.actions,
    events = { "key_pressed", { type = "interval_end", name = "Uninterrupt" }, "animation_end" },
}
local preview = sf2.moves.register {
    id = "shop_chinese_swords_super_slash", animation = animation,
    core_templates = { "ShopTryOn", "StanceShop", "StageStance", "Stance" },
    mid_frames = 2, first_frame = 2, priority = 1, mirror_node = "NHeel_1", ends_stage = true,
    no_wall_repulsion = data.preview.no_wall_repulsion,
    no_interpolation_frames = data.preview.no_interpolation_frames,
    locks = data.preview.locks, align = data.preview.align, direction = data.direction,
    actions = data.preview.actions, events = { { type = "round_stage_start", name = "TryOn" } },
}
sf2.items.set_subtype {
    item = sf2.items.get("core:items/weapon/WEAPON_CHNY21_JIAN"), subtype = "ChineseSwords",
}
return { slash = slash, preview = preview }
