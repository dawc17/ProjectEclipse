local sf2 = require("sf2")
-- Complete archived ComboSphere3 graph, authored with typed Lua definitions.
local animation = {
    player = sf2.assets.binary("animations/big_sphere_player"),
    start = sf2.assets.binary("animations/magic_fire_aura_bullet"),
    middle = sf2.assets.binary("animations/magic_fire_aura_bullet"),
}
local family = sf2.moves.register_template { id = "combo_sphere3" }
local moves = {}
local function name(id) return "de128:moves/" .. id end
local function point(object, part, player, x, y)
    return { object = object, part = part, player = player, shift_x = x, shift_y = y }
end
local function locks(player, shop)
    local result = {
        { type = "item", item_type = player and "Magic" or "Weapon", item_subtype = "ComboSphere3" },
        { type = "item", item_type = "Skeleton", item_subtype = player and "Skeleton" or "SkeletonMagic" },
    }
    if shop then table.insert(result, 1, { type = "screen", name = "ShopMagic" }) end
    return result
end
local function actor() return { type = "actor_name", name = "ComboSphere3" } end
local function current(value, player, negate)
    return { type = "current_animation", name = value, player = player, ["not"] = negate }
end
local function sound(when, ...)
    return { type = "random_sound", frame = type(when) == "number" and when or nil,
        event = type(when) == "string" and when or nil, core_sounds = { ... } }
end
local function effect(stage, frame, scale, time_scale, position)
    return { type = "effect", frame = frame, effect = {
        name = "ComboSphere3" .. stage, core_sequence = "mgc_magic_big_sphere_" .. string.lower(stage),
        scale = scale, time_scale = time_scale, position = position, follow = false,
    } }
end
local function spawn(start_move)
    return { type = "create_projectile", frame = 15, projectile = {
        name = "ComboSphere3", core_skeleton = "SkeletonMagic", copy_parent_type = "Magic", start_move = start_move,
    } }
end
local function facing()
    return { from = point("Nodes", "NPivot", "Me"), to = point("Nodes", "NPivot", "Enemy") }
end
local function register(id, definition)
    definition.id = id
    moves[id] = sf2.moves.register(definition)
    return moves[id]
end

register("combo_sphere3_player", {
    animation = animation.player, core_templates = { "1key", "MagicPlayer", "Controlled", "SoundStrike" },
    type = "ATTACK", mid_frames = 1, first_frame = 1, priority = 110, mirror_node = "NHeel_1",
    tactic_distance = { axis = "X", minimum = 400, maximum = 700, from = point("Pivot", nil, "Me"), to = point("Nodes", "NPivot", "Enemy") },
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
        } }, current("Physical", nil, true), { type = "mod_exists", name = "Stun", ["not"] = true },
    }, locks = locks(true), intervals = { { name = "Uninterrupt", ["end"] = 34 } },
    actions = { effect("Start", 22, 0.75, 4, point("Nodes", "NPivot", "Me", 95, 15)),
        spawn(), sound(15, "snd_bigsphere_end"), { type = "add_bullets", frame = 7, bullets = { type = "MagicBullet", value = -1 } },
        sound("Strike", "snd_hit1", "snd_hit2", "snd_hit3", "snd_hit4", "snd_hit5", "snd_hit6") },
    events = { "key_pressed", { type = "interval_end", name = "Uninterrupt" }, "animation_end" }, direction = facing(),
})
register("combo_sphere3_start", {
    animation = animation.start, templates = { family }, core_templates = { "MagicMissileStart", "MagicMissile", "MissileStart", "MagicMissileFly" },
    mid_frames = 3, first_frame = 9, priority = 600, no_magic_recharge = true,
    align = { axes = { "X", "Z" }, pivot = point("Nodes", "Magic-Node2_1"), position = point("Nodes", "NPivot", "Enemy") },
    direction = { from = point("Wall", "Back", "Parent"), to = point("Wall", "Front", "Parent") },
    events = { "birth" }, intervals = { { type = "Attack", start = 12, ["end"] = 22, attack = {
        -- Preserve the archive's repeated edge names and order.
        edges = { "Magic-Edge4_1", "Magic-Edge1_1", "Magic-Edge2_1", "Magic-Edge1_1", "Magic-Edge2_1" },
        damage = 0.3, damage_terms = { { type = "MagicDamage" }, { type = "UnarmedDamage", shift = -25 } },
        impulse = { x = -1, y = -600 }, hit = "HighLong",
        options = { no_effect = true, no_critical = true, body_part = "Body", defense_types = { "BodyDefense" },
            ignores_block = true, ignores_invulnerable = { "Evade", "Recovery", "Dash" } },
    } } }, actions = { effect("End", 10, 1.5, 2, point("Nodes", "Magic-Node2_1", "Me", 0, -150)),
        { type = "delete_actor", player = "Me", event = "Strike" },
        { type = "delete_actor", player = "Me", event = "AnimationEnd" } },
    conditions = { current("StanceShop", "Parent", true), actor() }, locks = locks(false),
})
for _, row in ipairs({ { "shop_magic_combo_sphere3_player", "ShopPeacefulStart", "PeacefulStart" },
    { "shop_magic_try_on_combo_sphere3_player", "ShopTryOn", "TryOn" } }) do
    register(row[1], {
        animation = animation.player, core_templates = { row[2], "StanceShop", "StageStance", "Stance" },
        mid_frames = 1, first_frame = 1, priority = 1, no_interpolation_frames = true, ends_stage = true, mirror_node = "NHeel_1",
        align = { axes = { "X", "Z" }, pivot = point("Nodes", "NHeel_2"), position = point("Pivot", nil, "Me", -80) },
        locks = locks(true, true), actions = {
            effect("Start", 22, 0.75, 4, point("Nodes", "NPivot", "Me", 95, 15)),
            effect("End", 22, 2, 1.5, point("Wall", "Back", "Me", 400, 150)), sound(15, "snd_bigsphere_end"),
            { type = "try_on_end", event = "AnimationEnd" },
        }, events = { { type = "round_stage_start", name = row[3] } }, direction = facing(),
    })
end
return moves
