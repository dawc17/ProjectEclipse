local sf2 = require("sf2")
-- Historical MindThrowNormal graph and innate perk; no runtime XML loading.
local function name(id) return "de128:moves/mind_throw_" .. id end
local flag = "de128:behaviors/mind_throw:pending"

local function locks(caster)
    return {
        { item = caster and "Magic" or "Weapon", subtype = "MindThrowNormal" },
        { item = "Skeleton", subtype = caster and "Skeleton" or "SkeletonMagic" },
    }
end

local terms = { MagicDamage = 0, UnarmedDamage = -25 }
local moves = {}
local function register(id, value)
    value.id = "mind_throw_" .. id
    moves[id] = sf2.moves.register(value)
    return moves[id]
end
local victim = register("hit", {
    core_templates = { "Recoil", "Hit" }, animation = sf2.assets.binary("animations/mind_suffocation"),
    mid_frames = 2, first_frame = 2, priority = 500, mirror_node = "NHeel_1",
    align = { axes = { "X", "Z" }, pivot = { node = "NPivot" }, position = { pivot = "Me" } },
    events = { { hit = name("hit") } },
    conditions = { { not_mod = "MOD_TITAN" },
        { stage = "Fight" }, { not_animation = "Physical" },
        { not_interval = "TitanUnhittable" } },
    intervals = { { name = "Uninterrupt", to = 84 }, { name = "Throwable", from = 7 } },
    locks = { { item = "Skeleton", subtype = "Skeleton" } },
    direction = { impulse = { reverse = true } },
})
register("player", {
    core_templates = { "1key", "MagicPlayer", "Controlled", "SoundStrike" },
    animation = sf2.assets.binary("animations/mind_throw_1_normal"), type = "ATTACK",
    mid_frames = 2, first_frame = 1, priority = 110, tactic_weapon = "MindThrow", mirror_node = "NHeel_1",
    tactic_distance = { distance = "X", min = 400, from = { pivot = "Me" }, to = { node = "NPivot", player = "Enemy" } },
    align = { axes = { "X", "Z" }, pivot = { node = "NHeel_1" }, position = { pivot = "Me" } },
    conditions = { { keys = { "Magic" } },
        { bullets = "MagicBullet", min = 1 },
        { not_interval = "SemiUninterrupt" },
        { not_mod = "Concussion" },
        { not_interval = "Uninterrupt" },
        { stage = "Fight" },
        { not_all = { { animation = "$Move" }, { interval = "SemiUninterrupt" } } },
        { not_animation = "Physical" }, { not_mod = "Stun" } },
    locks = locks(true), intervals = { { name = "Uninterrupt" } },
    timeline = {
        [1] = { projectile = { name = "MindThrowNormal", core_skeleton = "SkeletonMagic", copy_parent_type = "Magic" } },
        [6] = { add_bullets = "MagicBullet", amount = -1 },
        [12] = { { play_sound = "snd_m_pl_attack6", voice = "Male" }, { play_sound = "snd_low_pl_attack6", voice = "MaleLow" }, { play_sound = "snd_f_pl_attack6", voice = "Female" } },
        [3] = { sound = "snd_magic_bomb_start" },
        strike = { sound = { "snd_hit1", "snd_hit2", "snd_hit3", "snd_hit4", "snd_hit5", "snd_hit6" } },
    }, events = { "key_pressed", { interval_end = "Uninterrupt" }, "animation_end" },
    direction = { from = { node = "NPivot", player = "Me" }, to = { node = "NPivot", player = "Enemy" } },
})
register("player2", {
    core_templates = { "MagicMissile" }, animation = sf2.assets.binary("animations/mind_throw_2_normal"), type = "ATTACK",
    mid_frames = 2, first_frame = 1, priority = 110, tactic_weapon = "MindThrow",
    events = { { mod_expires = flag, player = "Me" } },
    align = { axes = { "X", "Z" }, pivot = { node = "NHeel_1" }, position = { pivot = "Me" } },
    conditions = { { animation = name("hit"), player = "Enemy" } },
    locks = { { item = "Magic", subtype = "MindThrowNormal" } },
    intervals = { { name = "Uninterrupt" }, { type = "Attack", from = 48, to = 49, attack = { direct = true, damage = 0.15, damage_terms = terms, hit = "NoReaction",
            options = { no_critical = true, body_part = "Body", defense_types = { "BodyDefense" } } } } },
    timeline = {
        [1] = { effect = { name = "MindThrowStart",
        core_sequence = "mgc_magic_mind_throw_start", scale = 0.6,
        time_scale = 2, position = { node = "NFingertipsS_1", player = "Me" }, follow = true } },
        [2] = { effect = { name = "MindThrowMiddle",
        core_sequence = "mgc_magic_mind_throw_middle", scale = 0.6,
        time_scale = 3.5, position = { node = "NNeck", player = "Enemy", x = 5, y = 10 }, follow = true } },
        [47] = { { effect = { name = "MindThrowEnd",
        core_sequence = "mgc_magic_mind_throw_end", scale = 1.5,
        time_scale = 2, position = { node = "NPivot", player = "Enemy", x = -52, y = 63 }, follow = false } }, { sound = "snd_bucher_touchdown" } },
        [48] = { shake = { pause_time = 0, effect_time = 30, amplitude_x = 7, frequency_x = 1, amplitude_y = 9, frequency_y = 0.3 } },
        [13] = { sound = "snd_magic_lightningarrow_start" },
        strike = { play_sound = "snd_magic_lightningarrow_end" },
    },
})
for _, phase in ipairs({ "start", "middle" }) do
    local startup = phase == "start"
    register(phase, {
        core_templates = startup and { "MagicMissileStart", "MindThrow", "MissileStart", "MagicMissileFly" } or { "MindThrow", "MagicMissileFly" },
        animation = sf2.assets.binary("animations/mind_suffocation_" .. phase), mid_frames = 2, first_frame = 1, priority = 500,
        no_magic_recharge = true, no_wall_repulsion = startup, no_interpolation_frames = startup,
        velocity = not startup and { x = 40 } or nil,
        align = startup and { axes = { "X", "Z" }, pivot = { animation = true }, position = { animation = "Parent" } }
            or { axes = { "X", "Y", "Z" }, pivot = { node = "Magic-Node2_1" }, position = { node = "Magic-Node2_1", player = "Me" } },
        direction = startup and { from = { wall = "Back", player = "Parent" }, to = { wall = "Front", player = "Parent" } } or nil,
        events = { startup and "birth" or "animation_end" },
        conditions = { startup and { not_animation = "StanceShop", player = "Parent" } or
            { any = { { animation = name("start") }, { animation = name("middle") } } },
            { actor = "MindThrowNormal" } }, locks = locks(false),
        intervals = { { type = "Attack", from = startup and 10 or nil, attack = {
            edges = startup and { "Fireball-Edge1" } or { "Fireball-Edge1", "Magic-Edge2_1" },
            damage = startup and 0.3 or 0.35, damage_terms = terms, impulse = { x = 450 }, hit_move = victim,
            options = { no_effect = true, no_critical = true, body_part = "Body", defense_types = { "BodyDefense" },
                ignores_block = true, ignores_invulnerable = { "Evade", "Recovery", "Dash" } },
        } } }, timeline = startup and {
        [1] = { effect = { name = "MindThrowStart",
        core_sequence = "mgc_magic_mind_throw_start", scale = 0.6,
        time_scale = 0.9, position = { node = "Magic-Node2_1", player = "Me", x = -75, y = -25 }, follow = true } },
        animation_end = { stop_follow_effect = "MindThrowStart" },
        [13] = { sound = "snd_magic_lightningarrow_start" },
        strike = { { play_sound = "snd_magic_lightningarrow_end" }, { delete_actor = "Me" } },
    } or {
        [1] = { effect = { name = "MindThrowStart",
        core_sequence = "mgc_magic_mind_throw_middle", scale = 0.6,
        time_scale = 1, position = { node = "Magic-Node2_1", player = "Me", x = -75, y = -25 }, follow = true } },
        [10] = { sound = "snd_magic_lightningarrow_middle" },
        strike = { { delete_actor = "Me" }, { play_sound = "snd_magic_lightningarrow_end" } },
    },
    })
end
register("wall", {
    core_templates = { "MindThrow", "MagicMissileFly", "MagicMissile" },
    animation = sf2.assets.binary("animations/mind_suffocation_middle"), mid_frames = 2, first_frame = 1, priority = 500, no_magic_recharge = true,
    align = { axes = { "X", "Y", "Z" }, pivot = { node = "Magic-Node2_1" }, position = { node = "Magic-Node2_1", player = "Me" } },
    events = { "every_frame" }, conditions = { { animation = name("middle") },
        { distance = "X", max = -250, from = { node = "Magic-Node2_1" }, to = { wall = "Front" } },
        { actor = "MindThrowNormal" } }, locks = locks(false),
    timeline = {
        [1] = { { stop_effect = "MindThrowStart" }, { delete_actor = "Me" } },
    },
})
return moves
