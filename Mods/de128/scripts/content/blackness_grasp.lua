local sf2 = require("sf2")

-- The archived Grasp casts a hidden BlackHand, then tells that same child to
-- enter its attacking phase eight frames later. Keep the full native graph.

local grasp_perk = sf2.perks.get("core:perks/PERK_GRASP_OF_DARKNESS")
local hand_item = sf2.items.get("core:items/magic/MAGIC_ACID_CLOUD")
-- Spawned Eclipse actors carry the two hidden items but not parent perk slots;
-- BlackHand's actor name and hidden item still keep both phases exclusive.
local hand_locks = {
    { item = "Skeleton", name = "SkeletonMagic" },
    { item = "Weapon", name = "MAGIC_ACID_CLOUD" },
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
    align = { axes = { "X", "Z" }, pivot = { animation = true }, position = { animation = "Me" } },
    conditions = {
        { animation = "de128:moves/blackness_grasp_hand_start" },
        { actor = "BlackHand" },
    },
    locks = hand_locks,
    velocity = { ax = -0.4 },
    intervals = { { type = "Attack", from = 7, attack = {
        edges = { "lightningEdge1" }, damage = 0.4,
        damage_terms = { MagicDamage = 0, UnarmedDamage = -25 },
        impulse = { x = -600 }, hit = "Physycal",
        options = { no_effect = true, no_critical = true, body_part = "Body",
            defense_types = { "BodyDefense" }, ignores_block = true,
            ignores_invulnerable = { "Evade", "Recovery", "Dash" } },
    } } },
    timeline = {
        [12] = { delete_actor = "Me" },
    },
}

local hand_start = sf2.moves.register {
    id = "blackness_grasp_hand_start",
    animation = sf2.assets.binary("animations/magic_empty"),
    core_templates = { "BossAbility", "MagicMissileFly", "MagicMissile" },
    mid_frames = 2, first_frame = 2, priority = 500, no_magic_recharge = true,
    align = { axes = { "X", "Z" }, pivot = { node = "Magic-Node2_1" },
        position = { node = "NPivot", player = "Enemy", x = 100 } },
    direction = { from = { wall = "Back", player = "Parent" }, to = { wall = "Front", player = "Parent" } },
    conditions = { { actor = "BlackHand" } },
    locks = hand_locks,
    timeline = {
        [3] = { effect = {
            name = "BlackHandEFX", core_sequence = "mgc_effect_black_hand",
            time_scale = 3, scale = 1.5,
            position = { node = "Magic-Node2_1", player = "Me", x = 0, y = -10 }, follow = true,
        } },
        [6] = { sound = "snd_shadow_grasp" },
    },
}

local cast = sf2.moves.register {
    id = "blackness_grasp_player",
    animation = sf2.assets.binary("animations/darkness_hug_player"),
    core_templates = { "1key", "BossAbility", "Controlled", "SoundStrike" },
    type = "ATTACK", mid_frames = 2, first_frame = 2, priority = 200,
    mirror_node = "NHeel_1", no_wall_repulsion = true,
    locks = { { perk = grasp_perk },
        { item = "Skeleton", subtype = "Skeleton" } },
    direction = { from = { node = "NPivot", player = "Me" }, to = { node = "NPivot", player = "Enemy" } },
    align = { axes = { "X", "Z" }, pivot = { node = "NHeel_2" },
        position = { pivot = "Me" } },
    events = { "key_pressed" },
    conditions = {
        { not_interval = "SemiUninterrupt" },
        { keys = { "RaidCharge" } },
        { not_mod = "GraspOfDarknessCD" },
        { not_interval = "Uninterrupt" },
        { stage = "Fight" },
        { not_all = {
            { animation = "$Move" },
            { interval = "SemiUninterrupt" },
        } },
        { not_animation = "Physical" },
    },
    tactic_conditions = { { distance = "X", min = 450, from = { pivot = "Me" }, to = { node = "NPivot", player = "Enemy" } } },
    intervals = { { name = "Uninterrupt", to = 39 } },
    timeline = {
        [9] = { projectile = {
            name = "BlackHand", core_skeleton = "SkeletonMagic", item = hand_item,
            start_move = hand_start,
        } },
        [17] = { play_animation = hand_attack, player = "Child", child_name = "BlackHand" },
    },
}

local function register_tactic(id, cooldown)
    return sf2.tactics.register {
        id = id, template = "Aggressive",
        on_decide = function(memory, event)
            -- The archived perk sets GraspOfDarknessCD for the warrior's _Frames
            -- at fight start and again on each cast (800 normal, 700 Power Mode).
            local ready = event.frame >= cooldown and event.frame >= (memory.next_grasp_frame or 0)
            local retreat, attacks = nil, {}
            for _, action in ipairs(event.actions) do
                if ready and action.name == "de128:moves/blackness_grasp_player" then
                    memory.next_grasp_frame = event.frame + cooldown
                    return action
                end
                if action.name == "StepBack" then retreat = action end
                if action.type == "attack" and action.name ~= "de128:moves/blackness_grasp_player" then
                    attacks[#attacks + 1] = action
                end
            end
            -- Grasp's native AI gate requires 450 X units. Back away while it is
            -- ready instead of letting Aggressive close that window, unless the
            -- back wall leaves no room to open it.
            local back_wall = event.back_wall_distance
            if ready and retreat and (not back_wall or back_wall > 120) then return retreat end
            if #attacks > 0 then
                memory.other_attack = (memory.other_attack or 0) % #attacks + 1
                return attacks[memory.other_attack]
            end
            if ready then return nil end
            return retreat or "wait"
        end,
    }
end

return { cast = cast, hand_start = hand_start, hand_attack = hand_attack,
    tactic = sf2.tactics.name(register_tactic("blackness_grasp", 800)),
    power_tactic = sf2.tactics.name(register_tactic("blackness_grasp_power", 700)) }
