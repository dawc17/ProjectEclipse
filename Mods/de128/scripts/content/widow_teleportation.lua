local sf2 = require("sf2")

-- Reviewed owner moves.xml: WidowTeleportationStart/End. Keep their native
-- identities so the perk trigger and the start-to-end transition still work.
-- This is typed content; DE128 never loads XML while the game runs.
local point = function(object, part, player, x, y)
    return { object = object, part = part, player = player, shift_x = x, shift_y = y }
end
local teleport_perk = sf2.perks.get("core:perks/PERK_TELEPORTATION")
local excluded_enemy_moves = {
    "WallRunUp", "WallJump_50", "WallJump_100", "WallJump_200", "WallJump_250",
    "WallHit", "WallHitFall", "WaspFly_150", "WaspFly_200", "WaspFly_300", "WaspFly_370",
    "HunterFly_150", "HunterFly_200", "HunterFly_300", "HunterFly_370",
}
local function enemy_exclusions(conditions)
    for _, name in ipairs(excluded_enemy_moves) do
        conditions[#conditions + 1] = { type = "current_animation", player = "Enemy", name = name, ["not"] = true }
    end
    return conditions
end
local function effect(name, sequence, frame, x, y, scale)
    return { type = "effect", frame = frame, effect = {
        name = name, core_sequence = sequence, scale = scale, time_scale = 1.5,
        position = point("Nodes", "NPivot", "Me", x, y), follow = false,
    } }
end
local hits = { "snd_hit1", "snd_hit2", "snd_hit3", "snd_hit4", "snd_hit5", "snd_hit6" }

local start_conditions = enemy_exclusions {
    { type = "keys", keys = { { key = "RaidCharge", press = "Tap" } } },
    { type = "mod_exists", name = "TeleportationRecharge", ["not"] = true },
}
start_conditions[#start_conditions + 1] = { type = "direction", player = "Enemy",
    from = point("Nodes", "NPivot", "Enemy"), to = point("Nodes", "NPivot", "Me") }
start_conditions[#start_conditions + 1] = { type = "current_interval", name = "SemiUninterrupt", ["not"] = true }
start_conditions[#start_conditions + 1] = { type = "current_interval", name = "Uninterrupt", ["not"] = true }
start_conditions[#start_conditions + 1] = { type = "round_stage", name = "Fight" }
start_conditions[#start_conditions + 1] = { type = "all", ["not"] = true, conditions = {
    { type = "current_animation", name = "$Move" },
    { type = "current_interval", name = "SemiUninterrupt" },
} }
start_conditions[#start_conditions + 1] = { type = "current_animation", name = "Physical", ["not"] = true }

local start = sf2.moves.replace {
    id = "widow_teleportation_start", target = "WidowTeleportationStart",
    expected_file = "widow_teleportation_start.bytes",
    animation = sf2.assets.binary("animations/widow_teleportation_start"),
    core_templates = { "1key", "BossAbility", "Controlled", "SoundStrike" },
    mid_frames = 2, no_wall_repulsion = true, first_frame = 1, priority = 110,
    mirror_node = "NHeel_1",
    align = { axes = { "X", "Z" }, pivot = point("Nodes", "NHeel_1"),
        position = point("Pivot", nil, "Me") },
    conditions = start_conditions,
    locks = { { type = "perk", perk = teleport_perk } },
    intervals = {
        { name = "Uninterrupt", ["end"] = 13 },
        { type = "Block", start = 14 },
        { name = "Throwable", start = 14 },
    },
    actions = {
        effect("WidowTeleportationStart", "mgc_widow_teleportation_start", 13, 54, 73, 1.3),
        { type = "random_sound", frame = 4, core_sounds = { "snd_widow_teleport_start" } },
        { type = "stop_sound", event = "Hit", core_sound = "snd_widow_teleport_start" },
        { type = "random_sound", event = "Strike", core_sounds = hits },
    },
    events = { "key_pressed", { type = "interval_end", name = "Uninterrupt" }, "animation_end" },
    direction = { from = point("Nodes", "NPivot", "Me"), to = point("Nodes", "NPivot", "Enemy") },
}

local end_conditions = enemy_exclusions {
    { type = "current_animation", name = "WidowTeleportationStart" },
}
local finish = sf2.moves.replace {
    id = "widow_teleportation_end", target = "WidowTeleportationEnd",
    expected_file = "widow_teleportation_end.bytes",
    animation = sf2.assets.binary("animations/widow_teleportation_end"),
    core_templates = { "ChangeDirection" },
    mid_frames = 2, no_wall_repulsion = true, first_frame = 1, priority = 450,
    mirror_node = "NHeel_1",
    direction = { from = point("Wall", "Front", "Enemy"), to = point("Wall", "Back", "Enemy") },
    align = { axes = { "X", "Z" }, shift_model_node = "NPivot",
        pivot = point("Nodes", "NHeel_1"), position = point("Pivot", nil, "Enemy", 100) },
    events = { "animation_end" },
    conditions = end_conditions,
    locks = { { type = "perk", perk = teleport_perk } },
    intervals = {
        { name = "Uninterrupt", ["end"] = 19 },
        { type = "Block", start = 20 },
        { name = "Throwable", start = 20 },
        { type = "Attack", start = 3, ["end"] = 4, attack = {
            edges = { "EForearm_1", "EHand_1", "EFingers_1", "EArm_1", "EArm_2",
                "EForearm_2", "EHand_2", "EFingers_2", "EChest" },
            damage = 0.28,
            damage_terms = { { type = "WeaponDamage" }, { type = "UnarmedDamage", shift = -10 } },
            impulse = { x = -245, y = -245 }, hit = "High",
            options = { ignores_block = true },
        } },
    },
    actions = {
        effect("WidowTeleportEnd", "mgc_widow_teleportation_end", 1, 65, 95, 1.5),
        { type = "random_sound", frame = 6, core_sounds = { "snd_swish2" } },
        { type = "random_sound", event = "Strike", core_sounds = hits },
        { type = "random_sound", frame = 6, core_sounds = { "snd_widow_teleport_end" } },
        { type = "stop_sound", event = "Hit", core_sound = "snd_widow_teleport_end" },
    },
}

local function tactic(id, initial, cooldown)
    return sf2.tactics.name(sf2.tactics.register {
        id = id, template = "Aggressive",
        on_decide = function(memory, event)
            local ready = event.frame >= initial and event.frame >= (memory.next_teleport_frame or 0)
            local retreat, attacks = nil, {}
            for _, action in ipairs(event.actions) do
                if ready and action.name == "WidowTeleportationStart" then
                    memory.next_teleport_frame = event.frame + cooldown
                    return action
                end
                if action.name == "StepBack" then retreat = action end
                if action.type == "attack" and action.name ~= "WidowTeleportationStart" then
                    attacks[#attacks + 1] = action
                end
            end
            if #attacks > 0 then
                memory.other_attack = (memory.other_attack or 0) % #attacks + 1
                return attacks[memory.other_attack]
            end
            return retreat or "wait"
        end,
    })
end

return {
    start = start, finish = finish,
    normal = tactic("widow_teleportation", 600, 600),
    fast = tactic("widow_teleportation_fast", 300, 300),
    power = tactic("widow_teleportation_power", 480, 480),
    slow = tactic("widow_teleportation_slow", 660, 660),
}
