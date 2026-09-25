local sf2 = require("sf2")
-- Complete archived Sphere1 graph: the player cast, the flying sphere (start,
-- middle and wall hit) and the shop preview and try-on moves.
local family = sf2.moves.register_template { id = "sphere1" }
local moves = {}

local HITS = { "snd_hit1", "snd_hit2", "snd_hit3", "snd_hit4", "snd_hit5", "snd_hit6" }
local SPHERE = { actor = "Sphere1" }
local SPHERE_NODE = { node = "Magic-Node2_1", player = "Me" }

-- Equipment the fighter (player) or the spawned sphere actor must carry.
local function locks(player, shop)
    local result = {
        { item = player and "Magic" or "Weapon", subtype = "Sphere1" },
        { item = "Skeleton", subtype = player and "Skeleton" or "SkeletonMagic" },
    }
    if shop then table.insert(result, 1, { screen = "ShopMagic" }) end
    return result
end

-- SmallSphereStart / Middle / End effects on the sphere's node.
local function sphere_effect(stage, x, y, options)
    options = options or {}
    return { effect = {
        name = "SmallSphere" .. stage, core_sequence = "mgc_magic_small_sphere_" .. string.lower(stage),
        scale = options.scale or 0.75, time_scale = options.time_scale or 1, looped = options.looped,
        position = { node = "Magic-Node2_1", player = "Me", x = x, y = y }, follow = options.follow ~= false,
    } }
end

local function spawn(start_move)
    return { projectile = { name = "Sphere1", core_skeleton = "SkeletonMagic", copy_parent_type = "Magic", start_move = start_move } }
end

local function attack(from, shroud)
    local ignores = { "Evade", "Recovery", "Dash" }
    if shroud then table.insert(ignores, "ShroudInterval") end
    return { type = "Attack", from = from, attack = {
        edges = { "Fireball-Edge1" }, damage = 0.45,
        damage_terms = { MagicDamage = 0, UnarmedDamage = -25 },
        impulse = { x = 500 }, hit = "High",
        options = { no_effect = true, no_critical = true, body_part = "Body",
            defense_types = { "BodyDefense" }, ignores_block = true, ignores_invulnerable = ignores },
    } }
end

-- The sphere bursts and removes itself when it strikes.
local STRIKE = {
    { sound = "snd_smallsphere_end" },
    { delete_actor = "Me" },
    sphere_effect("End", 100, 80, { scale = 1.5, follow = false }),
}

local function register(id, definition)
    definition.id = id
    moves[id] = sf2.moves.register(definition)
    return moves[id]
end

register("sphere1_player", {
    animation = "animations/fireball_player",
    core_templates = { "1key", "MagicPlayer", "Controlled", "SoundStrike" },
    type = "ATTACK", mid_frames = 2, first_frame = 2, priority = 110, tactic_weapon = "Sphere1", mirror_node = "NHeel_1",
    events = "controlled", direction = "face_enemy",
    align = { axes = { "X", "Z" }, pivot = { node = "NHeel_2" }, position = { pivot = "Me" } },
    tactic_distance = { distance = "X", min = 400, from = { pivot = "Me" }, to = { node = "NPivot", player = "Enemy" } },
    conditions = {
        { key = "Magic" },
        { bullets = "MagicBullet", min = 1 },
        { not_mod = "Concussion" },
        { controllable = true },
    },
    locks = locks(true),
    intervals = { { name = "Uninterrupt", to = 31 } },
    timeline = {
        [2] = spawn(),
        [7] = { add_bullets = "MagicBullet", amount = -1 },
        strike = { sound = HITS },
    },
})

register("sphere1_start", {
    animation = "animations/fireball_start", templates = { family },
    core_templates = { "MagicMissileStart", "MagicMissile", "MissileStart", "MagicMissileFly" },
    mid_frames = 2, first_frame = 2, priority = 500, no_wall_repulsion = true, no_interpolation_frames = true, no_magic_recharge = true,
    events = { "birth" },
    direction = { from = { wall = "Back", player = "Parent" }, to = { wall = "Front", player = "Parent" } },
    align = { axes = { "X", "Y", "Z" }, pivot = { animation = true }, position = { animation = "Parent", y = 45 } },
    conditions = { { not_animation = "StanceShop", player = "Parent" }, SPHERE },
    locks = locks(false),
    intervals = { attack(13, true) },
    timeline = {
        [2] = { { sound = "snd_smallsphere_start" }, sphere_effect("Start", 0, 80, { time_scale = 1.45 }),
            { sound = "snd_magic_fireball_start" } },
        animation_end = { { sound = "snd_smallsphere_middle" }, sphere_effect("Middle", -70, 55, { looped = true }) },
        strike = STRIKE,
    },
})

register("sphere1_middle", {
    animation = "animations/fireball_middle", templates = { family }, core_templates = { "MagicMissileFly", "MagicMissile" },
    mid_frames = 2, first_frame = 1, priority = 500, no_magic_recharge = true, velocity = { x = 30 },
    events = { "animation_end" },
    align = { axes = { "X", "Y", "Z" }, pivot = { node = "Magic-Node2_1" }, position = SPHERE_NODE },
    conditions = {
        { any = { { animation = "de128:moves/sphere1_start" }, { animation = "de128:moves/sphere1_middle" } } },
        SPHERE,
    },
    locks = locks(false),
    intervals = { attack() },
    timeline = { strike = STRIKE },
})

register("sphere1_wall", {
    animation = "animations/fireball_middle", templates = { family }, core_templates = { "MagicMissileFly", "MagicMissile" },
    mid_frames = 2, first_frame = 1, priority = 500, no_magic_recharge = true,
    events = { "every_frame" },
    align = { axes = { "X", "Y", "Z" }, pivot = { node = "Magic-Node2_1" }, position = SPHERE_NODE },
    conditions = {
        { animation = "de128:moves/sphere1_middle" },
        { distance = "X", max = -250, from = { node = "Magic-Node2_1" }, to = { wall = "Front" } },
        SPHERE,
    },
    locks = locks(false),
    timeline = { [1] = { delete_actor = "Me" } },
})

-- Shop preview: the fighter casts a sphere that plays its effects and vanishes.
local preview = register("shop_magic_sphere1", {
    animation = "animations/fireball_start", templates = { family }, core_templates = { "MagicShop", "MagicMissileFly", "MagicMissile" },
    mid_frames = 2, first_frame = 2, priority = 1, no_wall_repulsion = true, no_interpolation_frames = true, no_magic_recharge = true,
    events = { "birth" },
    align = { axes = { "X", "Z" }, pivot = { animation = true }, position = { animation = "Parent" } },
    conditions = { { animation = "ShopPeacefulStart", player = "Parent" }, SPHERE },
    locks = locks(false),
    timeline = {
        [3] = sphere_effect("Start", 15, 25, { time_scale = 1.45 }),
        [4] = { sound = "snd_smallsphere_start" },
        animation_end = { sphere_effect("Middle", -60, 40, { looped = true }), { sound = "snd_smallsphere_middle" },
            { delete_actor = "Me" } },
    },
})

for _, shop in ipairs({
    { id = "shop_magic_sphere1_player", template = "ShopPeacefulStart", stage = "PeacefulStart", start = preview },
    { id = "shop_magic_try_on_sphere1_player", template = "ShopTryOn", stage = "TryOn" },
}) do
    register(shop.id, {
        animation = "animations/fireball_player", core_templates = { shop.template, "StanceShop", "StageStance", "Stance" },
        mid_frames = 2, first_frame = 2, priority = 1, no_interpolation_frames = true, ends_stage = true, mirror_node = "NHeel_1",
        events = { { round_stage_start = shop.stage } }, direction = "face_enemy",
        align = { axes = { "X", "Z" }, pivot = { node = "NHeel_2" }, position = { pivot = "Me", x = -80 } },
        locks = locks(true, true),
        timeline = { [2] = spawn(shop.start) },
    })
end

register("shop_magic_try_on_sphere1_start", {
    animation = "animations/fireball_start", templates = { family }, core_templates = { "MagicShopTryOn", "MagicMissileFly", "MagicMissile" },
    mid_frames = 2, first_frame = 2, priority = 1, no_wall_repulsion = true, no_interpolation_frames = true, no_magic_recharge = true,
    events = { "birth" },
    align = { axes = { "X", "Z" }, pivot = { animation = true }, position = { animation = "Parent" } },
    conditions = { { animation = "ShopTryOn", player = "Parent" }, SPHERE },
    locks = locks(false),
    timeline = {
        [2] = { sphere_effect("Start", 0, 25, { time_scale = 1.45 }), { sound = "snd_smallsphere_start" } },
        animation_end = { sphere_effect("Middle", -70, 0.5, { looped = true }), { sound = "snd_smallsphere_middle" } },
    },
})

register("shop_magic_try_on_sphere1_end", {
    animation = "animations/fireball_middle", templates = { family }, core_templates = { "MagicMissileEnd", "MagicMissileFly", "MagicMissile" },
    mid_frames = 2, first_frame = 1, priority = 500, no_magic_recharge = true, velocity = { x = 30 },
    events = { "animation_end" },
    align = { axes = { "X", "Y", "Z" }, pivot = { node = "Magic-Node2_1" }, position = SPHERE_NODE },
    conditions = { { animation = "de128:moves/shop_magic_try_on_sphere1_start" }, SPHERE },
    locks = locks(false),
    timeline = {
        [2] = { { stop_effect = "SmallSphereMiddle" }, sphere_effect("End", 100, 0.5, { scale = 1.5, follow = false }),
            { sound = "snd_smallsphere_end" } },
    },
})

return moves
