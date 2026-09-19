local sf2 = require("sf2")
-- Complete archived Sphere1 graph, authored with typed Lua definitions.
local animation = {
    player = sf2.assets.binary("animations/fireball_player"),
    start = sf2.assets.binary("animations/fireball_start"),
    middle = sf2.assets.binary("animations/fireball_middle"),
}
local family = sf2.moves.register_template { id = "sphere1" }
local moves = {}
local function name(id) return "de128:moves/" .. id end
local function point(object, part, player, x, y)
    return { object = object, part = part, player = player, shift_x = x, shift_y = y }
end
local function locks(player, shop)
    local result = {
        { type = "item", item_type = player and "Magic" or "Weapon", item_subtype = "Sphere1" },
        { type = "item", item_type = "Skeleton", item_subtype = player and "Skeleton" or "SkeletonMagic" },
    }
    if shop then table.insert(result, 1, { type = "screen", name = "ShopMagic" }) end
    return result
end
local function actor() return { type = "actor_name", name = "Sphere1" } end
local function current(value, player, negate)
    return { type = "current_animation", name = value, player = player, ["not"] = negate }
end
local function sound(when, ...)
    return { type = "random_sound", frame = type(when) == "number" and when or nil,
        event = type(when) == "string" and when or nil, core_sounds = { ... } }
end
local function effect(stage, when, x, y, follow, scale, time_scale, looped)
    return { type = "effect", frame = type(when) == "number" and when or nil,
        event = type(when) == "string" and when or nil, effect = {
            name = "SmallSphere" .. stage, core_sequence = "mgc_magic_small_sphere_" .. string.lower(stage),
            scale = scale, time_scale = time_scale, looped = looped,
            position = point("Nodes", "Magic-Node2_1", "Me", x, y), follow = follow,
        } }
end
local function spawn(start_move)
    return { type = "create_projectile", frame = 2, projectile = {
        name = "Sphere1", core_skeleton = "SkeletonMagic", copy_parent_type = "Magic", start_move = start_move,
    } }
end
local function facing()
    return { from = point("Nodes", "NPivot", "Me"), to = point("Nodes", "NPivot", "Enemy") }
end
local function attack(start, shroud)
    local ignores = { "Evade", "Recovery", "Dash" }
    if shroud then table.insert(ignores, "ShroudInterval") end
    return { type = "Attack", start = start, attack = {
        edges = { "Fireball-Edge1" }, damage = 0.45,
        damage_terms = { { type = "MagicDamage" }, { type = "UnarmedDamage", shift = -25 } },
        impulse = { x = 500 }, hit = "High",
        options = { no_effect = true, no_critical = true, body_part = "Body",
            defense_types = { "BodyDefense" }, ignores_block = true, ignores_invulnerable = ignores },
    } }
end
local function strike_actions()
    return { sound("Strike", "snd_smallsphere_end"),
        { type = "delete_actor", player = "Me", event = "Strike" },
        effect("End", "Strike", 100, 80, false, 1.5, 1, false) }
end
local function register(id, definition)
    definition.id = id
    moves[id] = sf2.moves.register(definition)
    return moves[id]
end

register("sphere1_player", {
    animation = animation.player, core_templates = { "1key", "MagicPlayer", "Controlled", "SoundStrike" },
    type = "ATTACK", mid_frames = 2, first_frame = 2, priority = 110, tactic_weapon = "Sphere1", mirror_node = "NHeel_1",
    tactic_distance = { axis = "X", minimum = 400, from = point("Pivot", nil, "Me"), to = point("Nodes", "NPivot", "Enemy") },
    align = { axes = { "X", "Z" }, pivot = point("Nodes", "NHeel_2"), position = point("Pivot", nil, "Me") },
    conditions = {
        { type = "keys", keys = { { key = "Magic" } } },
        { type = "bullets", bullet_type = "MagicBullet", minimum = 1 },
        { type = "current_interval", name = "SemiUninterrupt", ["not"] = true },
        { type = "mod_exists", name = "Concussion", ["not"] = true },
        { type = "current_interval", name = "Uninterrupt", ["not"] = true },
        { type = "round_stage", name = "Fight" },
        { type = "all", ["not"] = true, conditions = {
            current("$Move"), { type = "current_interval", name = "SemiUninterrupt" },
        } }, current("Physical", nil, true),
    }, locks = locks(true), intervals = { { name = "Uninterrupt", ["end"] = 31 } },
    actions = { spawn(), { type = "add_bullets", frame = 7, bullets = { type = "MagicBullet", value = -1 } },
        sound("Strike", "snd_hit1", "snd_hit2", "snd_hit3", "snd_hit4", "snd_hit5", "snd_hit6") },
    events = { "key_pressed", { type = "interval_end", name = "Uninterrupt" }, "animation_end" }, direction = facing(),
})
local start_actions = { sound(2, "snd_smallsphere_start"), effect("Start", 2, 0, 80, true, 0.75, 1.45, false),
    sound("AnimationEnd", "snd_smallsphere_middle"), effect("Middle", "AnimationEnd", -70, 55, true, 0.75, 1, true),
    sound(2, "snd_magic_fireball_start") }
for _, action in ipairs(strike_actions()) do table.insert(start_actions, action) end
register("sphere1_start", {
    animation = animation.start, templates = { family },
    core_templates = { "MagicMissileStart", "MagicMissile", "MissileStart", "MagicMissileFly" },
    mid_frames = 2, first_frame = 2, priority = 500, no_wall_repulsion = true, no_interpolation_frames = true, no_magic_recharge = true,
    direction = { from = point("Wall", "Back", "Parent"), to = point("Wall", "Front", "Parent") },
    align = { axes = { "X", "Y", "Z" }, pivot = point("Animation"), position = point("Animation", nil, "Parent", nil, 45) },
    events = { "birth" }, intervals = { attack(13, true) }, actions = start_actions,
    conditions = { current("StanceShop", "Parent", true), actor() }, locks = locks(false),
})
register("sphere1_middle", {
    animation = animation.middle, templates = { family }, core_templates = { "MagicMissileFly", "MagicMissile" },
    mid_frames = 2, first_frame = 1, priority = 500, no_magic_recharge = true, velocity = { x = 30 },
    align = { axes = { "X", "Y", "Z" }, pivot = point("Nodes", "Magic-Node2_1"), position = point("Nodes", "Magic-Node2_1", "Me") },
    events = { "animation_end" }, conditions = { { type = "any", conditions = {
        current(name("sphere1_start")), current(name("sphere1_middle")),
    } }, actor() }, intervals = { attack() }, actions = strike_actions(), locks = locks(false),
})
register("sphere1_wall", {
    animation = animation.middle, templates = { family }, core_templates = { "MagicMissileFly", "MagicMissile" },
    mid_frames = 2, first_frame = 1, priority = 500, no_magic_recharge = true,
    align = { axes = { "X", "Y", "Z" }, pivot = point("Nodes", "Magic-Node2_1"), position = point("Nodes", "Magic-Node2_1", "Me") },
    events = { "every_frame" }, conditions = { current(name("sphere1_middle")),
        { type = "distance", axis = "X", maximum = -250, from = point("Nodes", "Magic-Node2_1"), to = point("Wall", "Front") }, actor() },
    actions = { { type = "delete_actor", player = "Me", frame = 1 } }, locks = locks(false),
})
local preview = register("shop_magic_sphere1", {
    animation = animation.start, templates = { family }, core_templates = { "MagicShop", "MagicMissileFly", "MagicMissile" },
    mid_frames = 2, first_frame = 2, priority = 1, no_wall_repulsion = true, no_interpolation_frames = true, no_magic_recharge = true,
    actions = { effect("Start", 3, 15, 25, true, 0.75, 1.45, false),
        effect("Middle", "AnimationEnd", -60, 40, true, 0.75, 1, true), sound(4, "snd_smallsphere_start"),
        sound("AnimationEnd", "snd_smallsphere_middle"), { type = "delete_actor", player = "Me", event = "AnimationEnd" } },
    events = { "birth" }, conditions = { current("ShopPeacefulStart", "Parent"), actor() }, locks = locks(false),
    align = { axes = { "X", "Z" }, pivot = point("Animation"), position = point("Animation", nil, "Parent") },
})
for _, row in ipairs({ { "shop_magic_sphere1_player", "ShopPeacefulStart", "PeacefulStart", preview },
    { "shop_magic_try_on_sphere1_player", "ShopTryOn", "TryOn" } }) do
    register(row[1], {
        animation = animation.player, core_templates = { row[2], "StanceShop", "StageStance", "Stance" },
        mid_frames = 2, first_frame = 2, priority = 1, no_interpolation_frames = true, ends_stage = true, mirror_node = "NHeel_1",
        align = { axes = { "X", "Z" }, pivot = point("Nodes", "NHeel_2"), position = point("Pivot", nil, "Me", -80) },
        locks = locks(true, true), actions = { spawn(row[4]) }, events = { { type = "round_stage_start", name = row[3] } }, direction = facing(),
    })
end
register("shop_magic_try_on_sphere1_start", {
    animation = animation.start, templates = { family }, core_templates = { "MagicShopTryOn", "MagicMissileFly", "MagicMissile" },
    mid_frames = 2, first_frame = 2, priority = 1, no_wall_repulsion = true, no_interpolation_frames = true, no_magic_recharge = true,
    actions = { effect("Start", 2, 0, 25, true, 0.75, 1.45, false), effect("Middle", "AnimationEnd", -70, 0.5, true, 0.75, 1, true),
        sound(2, "snd_smallsphere_start"), sound("AnimationEnd", "snd_smallsphere_middle") },
    events = { "birth" }, conditions = { current("ShopTryOn", "Parent"), actor() }, locks = locks(false),
    align = { axes = { "X", "Z" }, pivot = point("Animation"), position = point("Animation", nil, "Parent") },
})
register("shop_magic_try_on_sphere1_end", {
    animation = animation.middle, templates = { family }, core_templates = { "MagicMissileEnd", "MagicMissileFly", "MagicMissile" },
    mid_frames = 2, first_frame = 1, priority = 500, no_magic_recharge = true, velocity = { x = 30 },
    align = { axes = { "X", "Y", "Z" }, pivot = point("Nodes", "Magic-Node2_1"), position = point("Nodes", "Magic-Node2_1", "Me") },
    events = { "animation_end" }, conditions = { current(name("shop_magic_try_on_sphere1_start")), actor() },
    actions = { { type = "stop_effect", effect_name = "SmallSphereMiddle", frame = 2 },
        effect("End", 2, 100, 0.5, false, 1.5, 1, false), sound(2, "snd_smallsphere_end") }, locks = locks(false),
})
return moves
