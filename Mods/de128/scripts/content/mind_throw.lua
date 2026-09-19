local sf2 = require("sf2")
-- Historical MindThrowNormal graph and innate perk; no runtime XML loading.
local function name(id) return "de128:moves/mind_throw_" .. id end
local flag = "de128:behaviors/mind_throw:pending"
local function point(object, part, player, x, y)
    return { object = object, part = part, player = player, shift_x = x, shift_y = y }
end
local function current(id, player, negate)
    return { type = "current_animation", name = id, player = player, ["not"] = negate }
end
local function locks(caster)
    return {
        { type = "item", item_type = caster and "Magic" or "Weapon", item_subtype = "MindThrowNormal" },
        { type = "item", item_type = "Skeleton", item_subtype = caster and "Skeleton" or "SkeletonMagic" },
    }
end
local function random(frame, ...)
    return { type = "random_sound", frame = frame, core_sounds = { ... } }
end
local function effect(id, sequence, frame, scale, time, position, follow)
    return { type = "effect", frame = frame, effect = { name = id,
        core_sequence = "mgc_magic_mind_throw_" .. sequence, scale = scale,
        time_scale = time, position = position, follow = follow } }
end
local terms = { { type = "MagicDamage" }, { type = "UnarmedDamage", shift = -25 } }
local moves = {}
local function register(id, value)
    value.id = "mind_throw_" .. id
    moves[id] = sf2.moves.register(value)
    return moves[id]
end
local victim = register("hit", {
    core_templates = { "Recoil", "Hit" }, animation = sf2.assets.binary("animations/mind_suffocation"),
    mid_frames = 2, first_frame = 2, priority = 500, mirror_node = "NHeel_1",
    align = { axes = { "X", "Z" }, pivot = point("Nodes", "NPivot"), position = point("Pivot", nil, "Me") },
    events = { { type = "hit", name = name("hit") } },
    conditions = { { type = "mod_exists", name = "MOD_TITAN", ["not"] = true },
        { type = "round_stage", name = "Fight" }, current("Physical", nil, true),
        { type = "current_interval", name = "TitanUnhittable", ["not"] = true } },
    intervals = { { name = "Uninterrupt", ["end"] = 84 }, { name = "Throwable", start = 7 } },
    locks = { { type = "item", item_type = "Skeleton", item_subtype = "Skeleton" } },
    direction = { impulse = { reverse = true } },
})
register("player", {
    core_templates = { "1key", "MagicPlayer", "Controlled", "SoundStrike" },
    animation = sf2.assets.binary("animations/mind_throw_1_normal"), type = "ATTACK",
    mid_frames = 2, first_frame = 1, priority = 110, tactic_weapon = "MindThrow", mirror_node = "NHeel_1",
    tactic_distance = { axis = "X", minimum = 400, from = point("Pivot", nil, "Me"), to = point("Nodes", "NPivot", "Enemy") },
    align = { axes = { "X", "Z" }, pivot = point("Nodes", "NHeel_1"), position = point("Pivot", nil, "Me") },
    conditions = { { type = "keys", keys = { { key = "Magic" } } },
        { type = "bullets", bullet_type = "MagicBullet", minimum = 1 },
        { type = "current_interval", name = "SemiUninterrupt", ["not"] = true },
        { type = "mod_exists", name = "Concussion", ["not"] = true },
        { type = "current_interval", name = "Uninterrupt", ["not"] = true },
        { type = "round_stage", name = "Fight" },
        { type = "all", ["not"] = true, conditions = { current("$Move"), { type = "current_interval", name = "SemiUninterrupt" } } },
        current("Physical", nil, true), { type = "mod_exists", name = "Stun", ["not"] = true } },
    locks = locks(true), intervals = { { name = "Uninterrupt" } },
    actions = {
        { type = "create_projectile", frame = 1, projectile = { name = "MindThrowNormal", core_skeleton = "SkeletonMagic", copy_parent_type = "Magic" } },
        { type = "add_bullets", frame = 6, bullets = { type = "MagicBullet", value = -1 } },
        { type = "sound", frame = 12, sound = { core_sound = "snd_m_pl_attack6", voice = "Male" } },
        { type = "sound", frame = 12, sound = { core_sound = "snd_low_pl_attack6", voice = "MaleLow" } },
        { type = "sound", frame = 12, sound = { core_sound = "snd_f_pl_attack6", voice = "Female" } },
        random(3, "snd_magic_bomb_start"),
        { type = "random_sound", event = "Strike", core_sounds = { "snd_hit1", "snd_hit2", "snd_hit3", "snd_hit4", "snd_hit5", "snd_hit6" } },
    }, events = { "key_pressed", { type = "interval_end", name = "Uninterrupt" }, "animation_end" },
    direction = { from = point("Nodes", "NPivot", "Me"), to = point("Nodes", "NPivot", "Enemy") },
})
register("player2", {
    core_templates = { "MagicMissile" }, animation = sf2.assets.binary("animations/mind_throw_2_normal"), type = "ATTACK",
    mid_frames = 2, first_frame = 1, priority = 110, tactic_weapon = "MindThrow",
    events = { { type = "mod_expires", player = "Me", name = flag } },
    align = { axes = { "X", "Z" }, pivot = point("Nodes", "NHeel_1"), position = point("Pivot", nil, "Me") },
    conditions = { current(name("hit"), "Enemy") },
    locks = { { type = "item", item_type = "Magic", item_subtype = "MindThrowNormal" } },
    intervals = { { name = "Uninterrupt" }, { type = "Attack", start = 48, ["end"] = 49,
        attack = { direct = true, damage = 0.15, damage_terms = terms, hit = "NoReaction",
            options = { no_critical = true, body_part = "Body", defense_types = { "BodyDefense" } } } } },
    actions = {
        effect("MindThrowStart", "start", 1, 0.6, 2, point("Nodes", "NFingertipsS_1", "Me"), true),
        effect("MindThrowMiddle", "middle", 2, 0.6, 3.5, point("Nodes", "NNeck", "Enemy", 5, 10), true),
        effect("MindThrowEnd", "end", 47, 1.5, 2, point("Nodes", "NPivot", "Enemy", -52, 63), false),
        { type = "shake_screen", frame = 48, shake = { pause_time = 0, effect_time = 30, amplitude_x = 7, frequency_x = 1, amplitude_y = 9, frequency_y = 0.3 } },
        random(13, "snd_magic_lightningarrow_start"), random(47, "snd_bucher_touchdown"),
        { type = "sound", event = "Strike", sound = { core_sound = "snd_magic_lightningarrow_end" } },
    },
})
for _, phase in ipairs({ "start", "middle" }) do
    local startup = phase == "start"
    register(phase, {
        core_templates = startup and { "MagicMissileStart", "MindThrow", "MissileStart", "MagicMissileFly" } or { "MindThrow", "MagicMissileFly" },
        animation = sf2.assets.binary("animations/mind_suffocation_" .. phase), mid_frames = 2, first_frame = 1, priority = 500,
        no_magic_recharge = true, no_wall_repulsion = startup, no_interpolation_frames = startup,
        velocity = not startup and { x = 40 } or nil,
        align = startup and { axes = { "X", "Z" }, pivot = point("Animation"), position = point("Animation", nil, "Parent") }
            or { axes = { "X", "Y", "Z" }, pivot = point("Nodes", "Magic-Node2_1"), position = point("Nodes", "Magic-Node2_1", "Me") },
        direction = startup and { from = point("Wall", "Back", "Parent"), to = point("Wall", "Front", "Parent") } or nil,
        events = { startup and "birth" or "animation_end" },
        conditions = { startup and current("StanceShop", "Parent", true) or
            { type = "any", conditions = { current(name("start")), current(name("middle")) } },
            { type = "actor_name", name = "MindThrowNormal" } }, locks = locks(false),
        intervals = { { type = "Attack", start = startup and 10 or nil, attack = {
            edges = startup and { "Fireball-Edge1" } or { "Fireball-Edge1", "Magic-Edge2_1" },
            damage = startup and 0.3 or 0.35, damage_terms = terms, impulse = { x = 450 }, hit_move = victim,
            options = { no_effect = true, no_critical = true, body_part = "Body", defense_types = { "BodyDefense" },
                ignores_block = true, ignores_invulnerable = { "Evade", "Recovery", "Dash" } },
        } } }, actions = startup and {
            effect("MindThrowStart", "start", 1, 0.6, 0.9, point("Nodes", "Magic-Node2_1", "Me", -75, -25), true),
            { type = "stop_follow_effect", event = "AnimationEnd", effect_name = "MindThrowStart" },
            random(13, "snd_magic_lightningarrow_start"),
            { type = "sound", event = "Strike", sound = { core_sound = "snd_magic_lightningarrow_end" } },
            { type = "delete_actor", event = "Strike", player = "Me" },
        } or {
            effect("MindThrowStart", "middle", 1, 0.6, 1, point("Nodes", "Magic-Node2_1", "Me", -75, -25), true),
            random(10, "snd_magic_lightningarrow_middle"), { type = "delete_actor", event = "Strike", player = "Me" },
            { type = "sound", event = "Strike", sound = { core_sound = "snd_magic_lightningarrow_end" } },
        },
    })
end
register("wall", {
    core_templates = { "MindThrow", "MagicMissileFly", "MagicMissile" },
    animation = sf2.assets.binary("animations/mind_suffocation_middle"), mid_frames = 2, first_frame = 1, priority = 500, no_magic_recharge = true,
    align = { axes = { "X", "Y", "Z" }, pivot = point("Nodes", "Magic-Node2_1"), position = point("Nodes", "Magic-Node2_1", "Me") },
    events = { "every_frame" }, conditions = { current(name("middle")),
        { type = "distance", axis = "X", maximum = -250, from = point("Nodes", "Magic-Node2_1"), to = point("Wall", "Front") },
        { type = "actor_name", name = "MindThrowNormal" } }, locks = locks(false),
    actions = { { type = "stop_effect", frame = 1, effect_name = "MindThrowStart" }, { type = "delete_actor", frame = 1, player = "Me" } },
})
return moves
