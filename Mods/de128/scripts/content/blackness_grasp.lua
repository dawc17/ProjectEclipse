local sf2 = require("sf2")

-- The archived Grasp casts a hidden BlackHand, then tells that same child to
-- enter its attacking phase eight frames later. Keep the full native graph.
local function point(object, part, player, x, y)
    return { object = object, part = part, player = player, shift_x = x, shift_y = y }
end

local grasp_perk = sf2.perks.get("core:perks/PERK_GRASP_OF_DARKNESS")
local hand_item = sf2.items.get("core:items/magic/MAGIC_ACID_CLOUD")
-- Spawned Eclipse actors carry the two hidden items but not parent perk slots;
-- BlackHand's actor name and hidden item still keep both phases exclusive.
local hand_locks = {
    { type = "item", item_type = "Skeleton", name = "SkeletonMagic" },
    { type = "item", item_type = "Weapon", name = "MAGIC_ACID_CLOUD" },
}

for _, name in ipairs({ "BlacknessGraspAbilityPlayer", "BlacknessGraspAbilityHandStart",
    "BlacknessGraspAbilityHandMove" }) do
    sf2.moves.patch { move = name, disable = true }
end

local hand_attack = sf2.moves.register {
    id = "blackness_grasp_hand_attack",
    animation = sf2.assets.binary("animations/magic_empty"),
    core_templates = { "BossAbility", "MagicMissileFly", "MagicMissile" },
    mid_frames = 4, first_frame = 3, end_frame = 14, priority = 500,
    no_interpolation_frames = true, no_magic_recharge = true,
    align = { axes = { "X", "Z" }, pivot = point("Animation"), position = point("Animation", nil, "Me") },
    conditions = {
        { type = "current_animation", name = "de128:moves/blackness_grasp_hand_start" },
        { type = "actor_name", name = "BlackHand" },
    },
    locks = hand_locks,
    velocity = { ax = -0.4 },
    intervals = { { type = "Attack", start = 7, attack = {
        edges = { "lightningEdge1" }, damage = 0.4,
        damage_terms = { { type = "MagicDamage" }, { type = "UnarmedDamage", shift = -25 } },
        impulse = { x = -600 }, hit = "Physycal",
        options = { no_effect = true, no_critical = true, body_part = "Body",
            defense_types = { "BodyDefense" }, ignores_block = true,
            ignores_invulnerable = { "Evade", "Recovery", "Dash" } },
    } } },
    actions = { { type = "delete_actor", frame = 12, player = "Me" } },
}

local hand_start = sf2.moves.register {
    id = "blackness_grasp_hand_start",
    animation = sf2.assets.binary("animations/magic_empty"),
    core_templates = { "BossAbility", "MagicMissileFly", "MagicMissile" },
    mid_frames = 2, first_frame = 2, priority = 500, no_magic_recharge = true,
    align = { axes = { "X", "Z" }, pivot = point("Nodes", "Magic-Node2_1"),
        position = point("Nodes", "NPivot", "Enemy", 100) },
    direction = { from = point("Wall", "Back", "Parent"), to = point("Wall", "Front", "Parent") },
    conditions = { { type = "actor_name", name = "BlackHand" } },
    locks = hand_locks,
    actions = {
        { type = "effect", frame = 3, effect = {
            name = "BlackHandEFX", core_sequence = "mgc_effect_black_hand",
            time_scale = 3, scale = 1.5,
            position = point("Nodes", "Magic-Node2_1", "Me", 0, -10), follow = true,
        } },
        { type = "random_sound", frame = 6, core_sounds = { "snd_shadow_grasp" } },
    },
}

local cast = sf2.moves.register {
    id = "blackness_grasp_player",
    animation = sf2.assets.binary("animations/darkness_hug_player"),
    core_templates = { "1key", "BossAbility", "Controlled", "SoundStrike" },
    type = "ATTACK", mid_frames = 2, first_frame = 2, priority = 200,
    mirror_node = "NHeel_1", no_wall_repulsion = true,
    locks = { { type = "perk", perk = grasp_perk },
        { type = "item", item_type = "Skeleton", item_subtype = "Skeleton" } },
    direction = { from = point("Nodes", "NPivot", "Me"), to = point("Nodes", "NPivot", "Enemy") },
    align = { axes = { "X", "Z" }, pivot = point("Nodes", "NHeel_2"),
        position = point("Pivot", nil, "Me") },
    events = { "key_pressed" },
    conditions = {
        { type = "current_interval", name = "SemiUninterrupt", ["not"] = true },
        { type = "keys", keys = { { key = "RaidCharge" } } },
        { type = "mod_exists", name = "GraspOfDarknessCD", ["not"] = true },
        { type = "current_interval", name = "Uninterrupt", ["not"] = true },
        { type = "round_stage", name = "Fight" },
        { type = "all", ["not"] = true, conditions = {
            { type = "current_animation", name = "$Move" },
            { type = "current_interval", name = "SemiUninterrupt" },
        } },
        { type = "current_animation", name = "Physical", ["not"] = true },
    },
    tactic_conditions = { { type = "distance", axis = "X", minimum = 450,
        from = point("Pivot", nil, "Me"), to = point("Nodes", "NPivot", "Enemy") } },
    intervals = { { name = "Uninterrupt", ["end"] = 39 } },
    actions = {
        { type = "create_projectile", frame = 9, projectile = {
            name = "BlackHand", core_skeleton = "SkeletonMagic", item = hand_item,
            start_move = hand_start,
        } },
        { type = "play_animation", frame = 17, player = "Child", child_name = "BlackHand",
            move = hand_attack },
    },
}

local tactic = sf2.tactics.register {
    id = "blackness_grasp", template = "Aggressive",
    on_decide = function(memory, event)
        -- The DE perk starts Grasp on a 600-frame cooldown and renews it on
        -- cast. This opponent-only tactic preserves that combat timing.
        local ready = event.frame >= 600 and event.frame >= (memory.next_grasp_frame or 0)
        local retreat, attacks = nil, {}
        for _, action in ipairs(event.actions) do
            if ready and action.name == "de128:moves/blackness_grasp_player" then
                memory.next_grasp_frame = event.frame + 600
                return action
            end
            if action.name == "StepBack" then retreat = action end
            if action.type == "attack" and action.name ~= "de128:moves/blackness_grasp_player" then
                attacks[#attacks + 1] = action
            end
        end
        if not ready then
            if #attacks > 0 then
                memory.other_attack = (memory.other_attack or 0) % #attacks + 1
                return attacks[memory.other_attack]
            end
            return retreat or "wait"
        end
        -- Grasp's native AI gate requires 450 X units. Back away while it is
        -- ready instead of letting Aggressive close that window indefinitely.
        if retreat then return retreat end
        return nil
    end,
}

return { cast = cast, hand_start = hand_start, hand_attack = hand_attack,
    tactic = sf2.tactics.name(tactic) }
