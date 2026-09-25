local sf2 = require("sf2")
-- Complete archived Sphere3 graph, authored with typed Lua definitions.
local animation = {
    player = sf2.assets.binary("animations/big_sphere_player"),
    start = sf2.assets.binary("animations/magic_fire_aura_bullet"),
    middle = sf2.assets.binary("animations/magic_fire_aura_bullet"),
}
local family = sf2.moves.register_template { id = "sphere3" }
local moves = {}

local function locks(player, shop)
    local result = {
        { item = player and "Magic" or "Weapon", subtype = "Sphere3" },
        { item = "Skeleton", subtype = player and "Skeleton" or "SkeletonMagic" },
    }
    if shop then table.insert(result, 1, { screen = "ShopMagic" }) end
    return result
end

local function register(id, definition)
    definition.id = id
    moves[id] = sf2.moves.register(definition)
    return moves[id]
end

register("sphere3_player", {
    animation = animation.player, core_templates = { "1key", "MagicPlayer", "Controlled", "SoundStrike" },
    type = "ATTACK", mid_frames = 2, first_frame = 1, priority = 110, mirror_node = "NHeel_1",
    tactic_distance = { distance = "X", min = 400, max = 700, from = { pivot = "Me" }, to = { node = "NPivot", player = "Enemy" } },
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
    }, locks = locks(true), intervals = { { name = "Uninterrupt" } },
    timeline = {
        [5] = { projectile = {
        name = "Sphere3", core_skeleton = "SkeletonMagic", copy_parent_type = "Magic",
    } },
        [7] = { add_bullets = "MagicBullet", amount = -1 },
        strike = { sound = { "snd_hit1", "snd_hit2", "snd_hit3", "snd_hit4", "snd_hit5", "snd_hit6" } },
    },
    events = { "key_pressed", { interval_end = "Uninterrupt" }, "animation_end" }, direction = { from = { node = "NPivot", player = "Me" }, to = { node = "NPivot", player = "Enemy" } },
})
register("sphere3_start", {
    animation = animation.start, templates = { family }, core_templates = { "MagicMissileStart", "MagicMissile", "MissileStart", "MagicMissileFly" },
    mid_frames = 3, first_frame = 1, end_frame = 5, priority = 600, no_magic_recharge = true,
    align = { axes = { "X", "Z" }, pivot = { node = "Magic-Node2_1" }, position = { node = "NPivot", player = "Enemy" } },
    events = { "birth" }, timeline = {
        [2] = { effect = {
        name = "Sphere3Start", core_sequence = "mgc_magic_big_sphere_" .. string.lower("Start"),
        scale = 0.75, time_scale = 4, position = { node = "Magic-Node2_1", player = "Me", x = 0, y = -150 }, follow = false,
    } },
    },
    conditions = { { not_animation = "StanceShop", player = "Parent" }, { actor = "Sphere3" } }, locks = locks(false),
})
register("sphere3_middle", {
    animation = animation.middle, templates = { family }, core_templates = { "MagicMissile", "MagicMissileFly" },
    mid_frames = 3, first_frame = 9, priority = 600, no_magic_recharge = true,
    align = { axes = { "X", "Z" }, pivot = { node = "Magic-Node2_1" }, position = { node = "Magic-Node2_1", player = "Me" } },
    events = { "animation_end" }, intervals = { { type = "Attack", from = 22, attack = {
        -- Repeated edges are present in the archive and retain its ordering.
        edges = { "Magic-Edge4_1", "Magic-Edge1_1", "Magic-Edge2_1", "Magic-Edge1_1", "Magic-Edge2_1" },
        damage = 0.6, damage_terms = { MagicDamage = 0, UnarmedDamage = -25 },
        impulse = { y = -1000 }, hit = "Physycal",
        options = { no_effect = true, no_critical = true, body_part = "Body", defense_types = { "BodyDefense" },
            ignores_block = true, ignores_invulnerable = { "Evade", "Recovery", "Dash" } },
    } } }, timeline = {
        [10] = { effect = {
        name = "Sphere3End", core_sequence = "mgc_magic_big_sphere_" .. string.lower("End"),
        scale = 2, time_scale = 2, position = { node = "Magic-Node2_1", player = "Me", x = 0, y = -150 }, follow = false,
    } },
        animation_end = { delete_actor = "Me" },
        [9] = { sound = "snd_bigsphere_end" },
    },
    conditions = { { not_animation = "StanceShop", player = "Parent" }, { actor = "Sphere3" } }, locks = locks(false),
})
for _, row in ipairs({ { "shop_magic_sphere3_player", "ShopPeacefulStart", "PeacefulStart" },
    { "shop_magic_try_on_sphere3_player", "ShopTryOn", "TryOn" } }) do
    register(row[1], {
        animation = animation.player, core_templates = { row[2], "StanceShop", "StageStance", "Stance" },
        mid_frames = 2, first_frame = 1, priority = 1, no_interpolation_frames = true, ends_stage = true, mirror_node = "NHeel_1",
        align = { axes = { "X", "Z" }, pivot = { node = "NHeel_2" }, position = { pivot = "Me", x = -80 } },
        locks = locks(true, true), timeline = {
        [22] = { effect = {
        name = "Sphere3Start", core_sequence = "mgc_magic_big_sphere_" .. string.lower("Start"),
        scale = 0.75, time_scale = 4, position = { wall = "Back", player = "Me", x = 415, y = 160 }, follow = false,
    } },
        [27] = { effect = {
        name = "Sphere3End", core_sequence = "mgc_magic_big_sphere_" .. string.lower("End"),
        scale = 2, time_scale = 1.5, position = { wall = "Back", player = "Me", x = 400, y = 150 }, follow = false,
    } },
        [21] = { sound = "snd_bigsphere_end" },
        animation_end = { try_on_end = true },
    }, events = { { round_stage_start = row[3] } }, direction = { from = { node = "NPivot", player = "Me" }, to = { node = "NPivot", player = "Enemy" } },
    })
end
return moves
