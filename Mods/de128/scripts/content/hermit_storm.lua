local sf2 = require("sf2")

-- The archived storm keeps the native child actor's four unchanged moves, but
-- changes the caster, idle continuation and victory transition.

local storm_item = sf2.items.get("core:items/magic/HERMIT_STORM")
local storm_perk = sf2.perks.get("core:perks/PERK_HERMITSTORM")

sf2.moves.patch { move = "HermitStormPlayer", disable = true }
sf2.moves.patch { move = "HermitStormPlayerIdle", disable = true }
sf2.moves.patch { move = "Win_HermitStorm", disable = true }

local win = sf2.moves.register {
    id = "hermit_storm_win", animation = sf2.assets.binary("animations/hermit_super_attack_idle"),
    core_templates = { "Win", "EndStance", "StageStance", "Stance" },
    mid_frames = 2, first_frame = 0, end_frame = 54, priority = 100,
    ends_stage = true, mirror_node = "NHeel_1",
    align = { axes = { "X", "Z" }, pivot = { node = "NHeel_2" }, position = { pivot = "Me" } },
    transitions = { { frame_shift = 0, conditions = { { animation = "de128:moves/hermit_storm_idle" }, { animation = "de128:moves/hermit_storm_win" } } } },
    locks = { { item = "Skeleton", subtype = "Skeleton" } },
    conditions = {
        { any = { { animation = "de128:moves/hermit_storm_win" }, { animation = "de128:moves/hermit_storm_idle" } } },
        { round_result = "Victory" },
        { stage = "EndStance" },
    },
    intervals = { { name = "Uninterrupt" }, { type = "Block" }, { name = "Throwable" } },
    events = { { round_stage_start = "EndStance" },
        { interval_end = "Uninterrupt" }, "animation_end" },
    direction = { from = { node = "NPivot", player = "Me" }, to = { node = "NPivot", player = "Enemy" } },
}

local idle_actions = {
        [25] = { projectile = {
        name = "HermitStorm", core_skeleton = "SkeletonMagic", item = storm_item,
    } },
        [40] = { projectile = {
        name = "HermitStorm", core_skeleton = "SkeletonMagic", item = storm_item,
    } },
        hit = { stop_effect = "HermitStormLevitation" },
    }
for _, frame in ipairs({ 3, 10, 18, 26, 34, 42, 50 }) do
    idle_actions[frame] = { sound = "snd_hermit_storm_idle" }
end
local idle = sf2.moves.register {
    id = "hermit_storm_idle", animation = sf2.assets.binary("animations/hermit_super_attack_idle"),
    mid_frames = 2, first_frame = 3, end_frame = 54, priority = 999,
    mirror_node = "NHeel_1", tactic_equivalent = "StanceIdle", ends_stage = true,
    align = { axes = { "X", "Z" }, pivot = { node = "NHeel_2" }, position = { pivot = "Me" } },
    events = { "animation_end" },
    conditions = { { any = { { animation = "de128:moves/hermit_storm_player" }, { animation = "de128:moves/hermit_storm_idle" }, { animation = "de128:moves/hermit_storm_win" } } } },
    locks = { { perk = storm_perk } },
    intervals = { { name = "Unstable" }, { name = "SemiUninterrupt", from = 36 },
        { name = "Uninterrupt", to = 35 } },
    timeline = idle_actions,
}

local edges = { "EArm_1", "EArm_2", "EChest", "EForearm_1", "EForearm_2",
    "EHand_2", "EFingers_2", "EForearm_1", "EHand_1", "EFingers_1" }
local terms = { WeaponDamage = 0, UnarmedDamage = -10 }
local function attack(start, finish, hit)
    return { type = "Attack", from = start, to = finish, attack = {
        edges = edges, damage = 0.05, damage_terms = terms,
        impulse = { x = 345, y = -205, z = -350 }, hit = hit,
    } }
end
local player = sf2.moves.register {
    id = "hermit_storm_player", animation = sf2.assets.binary("animations/hermit_super_attack"),
    core_templates = { "1key", "BossAbility", "Controlled", "SoundStrike" },
    mid_frames = 2, no_wall_repulsion = true, first_frame = 1,
    priority = 110, mirror_node = "NHeel_1",
    tactic_conditions = {
        { distance = "X", min = 100, from = { pivot = "Me" }, to = { node = "NPivot", player = "Enemy" } },
        { distance = "X", min = 200, from = { wall = "Back", player = "Me" }, to = { node = "NHeel_1", player = "Me" } },
        { distance = "X", min = 200, from = { node = "NHeel_1", player = "Me" }, to = { wall = "Front", player = "Me" } },
    },
    align = { axes = { "X", "Z" }, pivot = { node = "NHeel_1" }, position = { pivot = "Me" } },
    conditions = {
        { keys = { "RaidCharge" } },
        { not_mod = "HermitStormRecharge" },
        { not_interval = "SemiUninterrupt" },
        { not_interval = "Uninterrupt" },
        { stage = "Fight" },
        { not_all = {
            { animation = "$Move" },
            { interval = "SemiUninterrupt" },
        } },
        { not_animation = "Physical" },
    },
    locks = { { perk = storm_perk },
        { item = "Skeleton", subtype = "Skeleton" } },
    intervals = { { name = "Unstable" }, { name = "Uninterrupt", to = 36 },
        { name = "SemiUninterrupt", from = 36 },
        attack(8, 9, "High"), attack(11, 19, "Low"), attack(20, 27, "Low") },
    timeline = {
        [20] = { projectile = {
        name = "HermitStorm", core_skeleton = "SkeletonMagic", item = storm_item,
    } },
        [19] = { effect = {
            name = "HermitStormLevitation", core_sequence = "mgc_effect_levitation_middle",
            scale = 1.5, time_scale = 2, looped = true, on_background = true,
            position = { node = "NStomach", player = "Me", x = 3, y = 15 }, follow = true,
        } },
        hit = { stop_effect = "HermitStormLevitation" },
        [1] = { sound = "snd_hermit_storm_start" },
        [31] = { sound = "snd_hermit_storm_idle" },
        [39] = { sound = "snd_hermit_storm_idle" },
        strike = { sound = {
            "snd_hit1", "snd_hit2", "snd_hit3", "snd_hit4", "snd_hit5", "snd_hit6" } },
    },
    events = { "key_pressed", { interval_end = "Uninterrupt" }, "animation_end" },
    direction = { from = { node = "NPivot", player = "Me" }, to = { node = "NPivot", player = "Enemy" } },
}

local tactic = sf2.tactics.register {
    id = "hermit_storm", template = "Aggressive",
    on_decide = function(memory, event)
        if event.seconds < (memory.next_storm or 0) then return nil end
        for _, action in ipairs(event.actions) do
            if action.name == "de128:moves/hermit_storm_player" then
                memory.next_storm = event.seconds + 6
                return action
            end
        end
        return nil
    end,
}

return { player = player, idle = idle, win = win, tactic = sf2.tactics.name(tactic) }
