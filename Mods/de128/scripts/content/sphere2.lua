local sf2 = require("sf2")
-- Complete archived Sphere2 graph, authored with typed Lua definitions.
local animation = {
    player = sf2.assets.binary("animations/fireball_player"),
    start = sf2.assets.binary("animations/fireball_start"),
    middle = sf2.assets.binary("animations/fireball_middle"),
}
local family = sf2.moves.register_template { id = "sphere2" }
local moves = {}
local function name(id) return "de128:moves/" .. id end

local function locks(player, shop)
    local result = {
        { item = player and "Magic" or "Weapon", subtype = "Sphere2" },
        { item = "Skeleton", subtype = player and "Skeleton" or "SkeletonMagic" },
    }
    if shop then table.insert(result, 1, { screen = "ShopMagic" }) end
    return result
end

local function attack(start, shroud)
    local ignores = { "Evade", "Recovery", "Dash" }
    if shroud then table.insert(ignores, "ShroudInterval") end
    return { type = "Attack", from = start, attack = {
        edges = { "Magic-Edge2_1", "lightningEdge2", "lightningEdge1" }, damage = 0.45,
        damage_terms = { MagicDamage = 0, UnarmedDamage = -25 },
        impulse = { x = 600 }, hit = "High",
        options = { no_effect = true, no_critical = true, body_part = "Body",
            defense_types = { "BodyDefense" }, ignores_block = true, ignores_invulnerable = ignores },
    } }
end

local function register(id, definition)
    definition.id = id
    moves[id] = sf2.moves.register(definition)
    return moves[id]
end

register("sphere2_player", {
    animation = animation.player, core_templates = { "1key", "MagicPlayer", "Controlled", "SoundStrike" },
    type = "ATTACK", mid_frames = 2, first_frame = 2, priority = 110, tactic_weapon = "Sphere2", mirror_node = "NHeel_1",
    tactic_distance = { distance = "X", min = 400, from = { pivot = "Me" }, to = { node = "NPivot", player = "Enemy" } },
    align = { axes = { "X", "Z" }, pivot = { node = "NHeel_2" }, position = { pivot = "Me" } },
    conditions = {
        { keys = { "Magic" } },
        { bullets = "MagicBullet", min = 1 },
        { not_interval = "SemiUninterrupt" },
        { not_mod = "Concussion" },
        { not_interval = "Uninterrupt" },
        { stage = "Fight" },
        { not_all = {
            { animation = "$Move" }, { interval = "SemiUninterrupt" },
        } }, { not_animation = "Physical" }, { not_mod = "Stun" },
    }, locks = locks(true), intervals = { { name = "Uninterrupt", to = 31 } },
    timeline = {
        [2] = { projectile = {
        name = "Sphere2", core_skeleton = "SkeletonMagic", copy_parent_type = "Magic",
    } },
        [7] = { add_bullets = "MagicBullet", amount = -1 },
        strike = { sound = { "snd_hit1", "snd_hit2", "snd_hit3", "snd_hit4", "snd_hit5", "snd_hit6" } },
    },
    events = { "key_pressed", { interval_end = "Uninterrupt" }, "animation_end" }, direction = { from = { node = "NPivot", player = "Me" }, to = { node = "NPivot", player = "Enemy" } },
})
local start_actions = {
        [3] = { effect = {
            name = "EnergyballStart", core_sequence = ("mgc_magic_mid_sphere_" .. string.lower("Start")),
            scale = 1, time_scale = 1.1, looped = false,
            position = { node = "Magic-Node2_1", player = "Me", x = 25, y = 0 }, follow = true,
        } },
        animation_end = { effect = {
            name = "EnergyballMiddle", core_sequence = ("mgc_magic_mid_sphere_" .. string.lower("Middle")),
            scale = 1, time_scale = 1, looped = true,
            position = { node = "Magic-Node2_1", player = "Me", x = 5, y = 0 }, follow = true,
        } },
        [2] = { sound = "snd_midsphere_start" },
        [12] = { sound = "snd_midsphere_middle" },
    }
start_actions.strike = {
    { sound = "snd_midsphere_end" }, { delete_actor = "Me" },
    { effect = { name = "EnergyballEnd", core_sequence = "mgc_magic_mid_sphere1_end",
        scale = 1.5, time_scale = 1, looped = false,
        position = { node = "Magic-Node2_1", player = "Me", x = 5, y = 0 }, follow = false } },
}

register("sphere2_start", {
    animation = animation.start, templates = { family },
    core_templates = { "MagicMissileStart", "MagicMissile", "MissileStart", "MagicMissileFly" },
    mid_frames = 2, first_frame = 2, priority = 500, no_wall_repulsion = true, no_interpolation_frames = true, no_magic_recharge = true,
    direction = { from = { wall = "Back", player = "Parent" }, to = { wall = "Front", player = "Parent" } },
    align = { axes = { "X", "Z" }, pivot = { animation = true }, position = { animation = "Parent" } },
    events = { "birth" }, intervals = { attack(13, true) }, timeline = start_actions,
    conditions = { { not_animation = "StanceShop", player = "Parent" }, { actor = "Sphere2" } }, locks = locks(false),
})
register("sphere2_middle", {
    animation = animation.middle, templates = { family }, core_templates = { "MagicMissileFly", "MagicMissile" },
    mid_frames = 2, first_frame = 1, priority = 500, no_magic_recharge = true, velocity = { x = 30 },
    align = { axes = { "X", "Y", "Z" }, pivot = { node = "Magic-Node2_1" }, position = { node = "Magic-Node2_1", player = "Me" } },
    events = { "animation_end" }, conditions = { { any = {
        { animation = name("sphere2_start") }, { animation = name("sphere2_middle") },
    } }, { actor = "Sphere2" } }, intervals = { attack(nil, true) }, timeline = {
        strike = { { sound = "snd_midsphere_end" }, { delete_actor = "Me" }, { effect = {
            name = "EnergyballEnd", core_sequence = ("mgc_magic_mid_sphere1_end"),
            scale = 1.5, time_scale = 1, looped = false,
            position = { node = "Magic-Node2_1", player = "Me", x = 5, y = 0 }, follow = false,
        } } },
    }, locks = locks(false),
})
register("sphere2_wall", {
    animation = animation.middle, templates = { family }, core_templates = { "MagicMissileFly", "MagicMissile" },
    mid_frames = 2, first_frame = 1, priority = 500, no_magic_recharge = true,
    align = { axes = { "X", "Y", "Z" }, pivot = { node = "Magic-Node2_1" }, position = { node = "Magic-Node2_1", player = "Me" } },
    events = { "every_frame" }, conditions = { { animation = name("sphere2_middle") },
        { distance = "X", max = -250, from = { node = "Magic-Node2_1" }, to = { wall = "Front" } }, { actor = "Sphere2" } },
    timeline = {
        [1] = { delete_actor = "Me" },
    }, locks = locks(false),
})
register("shop_magic_sphere2", {
    animation = animation.start, templates = { family }, core_templates = { "MagicShop", "MagicMissileFly", "MagicMissile" },
    mid_frames = 2, first_frame = 2, priority = 1, no_wall_repulsion = true, no_interpolation_frames = true, no_magic_recharge = true,
    timeline = {
        [3] = { effect = {
            name = "EnergyballStart", core_sequence = ("mgc_magic_mid_sphere_" .. string.lower("Start")),
            scale = 1, time_scale = 1.1, looped = false,
            position = { node = "Magic-Node2_1", player = "Me", x = 25, y = 0 }, follow = true,
        } },
        animation_end = { { effect = {
            name = "EnergyballMiddle", core_sequence = ("mgc_magic_mid_sphere_" .. string.lower("Middle")),
            scale = 1, time_scale = 1, looped = true,
            position = { node = "Magic-Node2_1", player = "Me", x = 5, y = 10 }, follow = true,
        } }, { delete_actor = "Me" } },
        [2] = { sound = "snd_midsphere_start" },
        [12] = { sound = "snd_midsphere_middle" },
    },
    events = { "birth" }, conditions = { { animation = "ShopPeacefulStart", player = "Parent" }, { actor = "Sphere2" } }, locks = locks(false),
    align = { axes = { "X", "Z" }, pivot = { animation = true }, position = { animation = "Parent" } },
})
for _, row in ipairs({ { "shop_magic_sphere2_player", "ShopPeacefulStart", "PeacefulStart" },
    { "shop_magic_try_on_sphere2_player", "ShopTryOn", "TryOn" } }) do
    register(row[1], {
        animation = animation.player, core_templates = { row[2], "StanceShop", "StageStance", "Stance" },
        mid_frames = 2, first_frame = 2, priority = 1, no_interpolation_frames = true, ends_stage = true, mirror_node = "NHeel_1",
        align = { axes = { "X", "Z" }, pivot = { node = "NHeel_2" }, position = { pivot = "Me", x = -80 } },
        locks = locks(true, true), timeline = {
        [2] = { projectile = {
        name = "Sphere2", core_skeleton = "SkeletonMagic", copy_parent_type = "Magic", start_move = row[4],
    } },
    }, events = { { round_stage_start = row[3] } }, direction = { from = { node = "NPivot", player = "Me" }, to = { node = "NPivot", player = "Enemy" } },
    })
end
register("shop_magic_try_on_sphere2_start", {
    animation = animation.start, templates = { family }, core_templates = { "MagicShopTryOn", "MagicMissileFly", "MagicMissile" },
    mid_frames = 2, first_frame = 2, priority = 1, no_wall_repulsion = true, no_interpolation_frames = true, no_magic_recharge = true,
    timeline = {
        [3] = { effect = {
            name = "EnergyballStart", core_sequence = ("mgc_magic_mid_sphere_" .. string.lower("Start")),
            scale = 1, time_scale = 1.1, looped = false,
            position = { node = "Magic-Node2_1", player = "Me", x = 25, y = 0 }, follow = true,
        } },
        animation_end = { effect = {
            name = "EnergyballMiddle", core_sequence = ("mgc_magic_mid_sphere_" .. string.lower("Middle")),
            scale = 1, time_scale = 1, looped = true,
            position = { node = "Magic-Node2_1", player = "Me", x = 5, y = 10 }, follow = true,
        } },
        [2] = { sound = "snd_midsphere_start" },
        [12] = { sound = "snd_midsphere_middle" },
    },
    events = { "birth" }, conditions = { { animation = "ShopTryOn", player = "Parent" }, { actor = "Sphere2" } }, locks = locks(false),
    align = { axes = { "X", "Z" }, pivot = { animation = true }, position = { animation = "Parent" } },
})
register("shop_magic_try_on_sphere2_end", {
    animation = animation.middle, templates = { family }, core_templates = { "MagicMissileEnd", "MagicMissileFly", "MagicMissile" },
    mid_frames = 2, first_frame = 1, priority = 500, no_magic_recharge = true, velocity = { x = 30 },
    align = { axes = { "X", "Y", "Z" }, pivot = { node = "Magic-Node2_1" }, position = { node = "Magic-Node2_1", player = "Me" } },
    events = { "animation_end" }, conditions = { { animation = name("shop_magic_try_on_sphere2_start") }, { actor = "Sphere2" } },
    timeline = {
        [2] = { { stop_effect = "EnergyballMiddle" }, { effect = {
            name = "EnergyballEnd", core_sequence = ("mgc_magic_mid_sphere1_end"),
            scale = 1.5, time_scale = 1, looped = false,
            position = { node = "Magic-Node2_1", player = "Me", x = 5, y = 10 }, follow = false,
        } }, { sound = "snd_midsphere_end" } },
    }, locks = locks(false),
})
return moves
