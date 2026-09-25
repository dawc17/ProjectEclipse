local sf2 = require("sf2")

-- Reviewed owner moves.xml: WidowTeleportationStart/End. Keep their native
-- identities so the perk trigger and the start-to-end transition still work.
-- This is typed content; DE128 never loads XML while the game runs.

local teleport_perk = sf2.perks.get("core:perks/PERK_TELEPORTATION")
local excluded_enemy_moves = {
    "WallRunUp", "WallJump_50", "WallJump_100", "WallJump_200", "WallJump_250",
    "WallHit", "WallHitFall", "WaspFly_150", "WaspFly_200", "WaspFly_300", "WaspFly_370",
    "HunterFly_150", "HunterFly_200", "HunterFly_300", "HunterFly_370",
    -- wasp_fly.lua disables the native WaspFly selectors and plays these instead.
    "de128:moves/wasp_fly_150", "de128:moves/wasp_fly_200",
    "de128:moves/wasp_fly_300", "de128:moves/wasp_fly_370",
}
local function enemy_exclusions(conditions)
    for _, name in ipairs(excluded_enemy_moves) do
        conditions[#conditions + 1] = { not_animation = name, player = "Enemy" }
    end
    return conditions
end

local hits = { "snd_hit1", "snd_hit2", "snd_hit3", "snd_hit4", "snd_hit5", "snd_hit6" }

local start_conditions = enemy_exclusions {
    { keys = { { "RaidCharge", press = "Tap" } } },
    { not_mod = "TeleportationRecharge" },
}
start_conditions[#start_conditions + 1] = { direction = "Enemy", from = { node = "NPivot", player = "Enemy" }, to = { node = "NPivot", player = "Me" } }
start_conditions[#start_conditions + 1] = { not_interval = "SemiUninterrupt" }
start_conditions[#start_conditions + 1] = { not_interval = "Uninterrupt" }
start_conditions[#start_conditions + 1] = { stage = "Fight" }
start_conditions[#start_conditions + 1] = { not_all = {
    { animation = "$Move" },
    { interval = "SemiUninterrupt" },
} }
start_conditions[#start_conditions + 1] = { not_animation = "Physical" }

local start = sf2.moves.replace {
    id = "widow_teleportation_start", target = "WidowTeleportationStart",
    expected_file = "widow_teleportation_start.bytes",
    animation = sf2.assets.binary("animations/widow_teleportation_start"),
    core_templates = { "1key", "BossAbility", "Controlled", "SoundStrike" },
    mid_frames = 2, no_wall_repulsion = true, first_frame = 1, priority = 110,
    mirror_node = "NHeel_1",
    align = { axes = { "X", "Z" }, pivot = { node = "NHeel_1" },
        position = { pivot = "Me" } },
    conditions = start_conditions,
    locks = { { perk = teleport_perk } },
    intervals = {
        { name = "Uninterrupt", to = 13 },
        { type = "Block", from = 14 },
        { name = "Throwable", from = 14 },
    },
    timeline = {
        [13] = { effect = {
        name = "WidowTeleportationStart", core_sequence = "mgc_widow_teleportation_start", scale = 1.3, time_scale = 1.5,
        position = { node = "NPivot", player = "Me", x = 54, y = 73 }, follow = false,
    } },
        [4] = { sound = "snd_widow_teleport_start" },
        hit = { stop_sound = "snd_widow_teleport_start" },
        strike = { sound = hits },
    },
    events = { "key_pressed", { interval_end = "Uninterrupt" }, "animation_end" },
    direction = { from = { node = "NPivot", player = "Me" }, to = { node = "NPivot", player = "Enemy" } },
}

local end_conditions = enemy_exclusions {
    { animation = "WidowTeleportationStart" },
}
local finish = sf2.moves.replace {
    id = "widow_teleportation_end", target = "WidowTeleportationEnd",
    expected_file = "widow_teleportation_end.bytes",
    animation = sf2.assets.binary("animations/widow_teleportation_end"),
    core_templates = { "ChangeDirection" },
    mid_frames = 2, no_wall_repulsion = true, first_frame = 1, priority = 450,
    mirror_node = "NHeel_1",
    direction = { from = { wall = "Front", player = "Enemy" }, to = { wall = "Back", player = "Enemy" } },
    align = { axes = { "X", "Z" }, shift_model_node = "NPivot",
        pivot = { node = "NHeel_1" }, position = { pivot = "Enemy", x = 100 } },
    events = { "animation_end" },
    conditions = end_conditions,
    locks = { { perk = teleport_perk } },
    intervals = {
        { name = "Uninterrupt", to = 19 },
        { type = "Block", from = 20 },
        { name = "Throwable", from = 20 },
        { type = "Attack", from = 3, to = 4, attack = {
            edges = { "EForearm_1", "EHand_1", "EFingers_1", "EArm_1", "EArm_2",
                "EForearm_2", "EHand_2", "EFingers_2", "EChest" },
            damage = 0.28,
            damage_terms = { WeaponDamage = 0, UnarmedDamage = -10 },
            impulse = { x = -245, y = -245 }, hit = "High",
            options = { ignores_block = true },
        } },
    },
    timeline = {
        [1] = { effect = {
        name = "WidowTeleportEnd", core_sequence = "mgc_widow_teleportation_end", scale = 1.5, time_scale = 1.5,
        position = { node = "NPivot", player = "Me", x = 65, y = 95 }, follow = false,
    } },
        [6] = { { sound = "snd_swish2" }, { sound = "snd_widow_teleport_end" } },
        strike = { sound = hits },
        hit = { stop_sound = "snd_widow_teleport_end" },
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
