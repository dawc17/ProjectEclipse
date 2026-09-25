local sf2 = require("sf2")

-- The archived storm keeps the native child actor's four unchanged moves, but
-- changes the caster, idle continuation and victory transition.
local function point(object, part, player, x, y)
    return { object = object, part = part, player = player, shift_x = x, shift_y = y }
end
local function current(name)
    return { type = "current_animation", name = "de128:moves/hermit_storm_" .. name }
end
local function spawn(frame, item)
    return { type = "create_projectile", frame = frame, projectile = {
        name = "HermitStorm", core_skeleton = "SkeletonMagic", item = item,
    } }
end
local function sound(frame, name)
    return { type = "random_sound", frame = frame, core_sounds = { name } }
end
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
    align = { axes = { "X", "Z" }, pivot = point("Nodes", "NHeel_2"), position = point("Pivot", nil, "Me") },
    transitions = { { frame_shift = 0, conditions = { current("idle"), current("win") } } },
    locks = { { type = "item", item_type = "Skeleton", item_subtype = "Skeleton" } },
    conditions = {
        { type = "any", conditions = { current("win"), current("idle") } },
        { type = "round_result", name = "Victory" },
        { type = "round_stage", name = "EndStance" },
    },
    intervals = { { name = "Uninterrupt" }, { type = "Block" }, { name = "Throwable" } },
    events = { { type = "round_stage_start", name = "EndStance" },
        { type = "interval_end", name = "Uninterrupt" }, "animation_end" },
    direction = { from = point("Nodes", "NPivot", "Me"), to = point("Nodes", "NPivot", "Enemy") },
}

local idle_actions = { spawn(25, storm_item), spawn(40, storm_item),
    { type = "stop_effect", event = "Hit", effect_name = "HermitStormLevitation" } }
for _, frame in ipairs({ 3, 10, 18, 26, 34, 42, 50 }) do
    idle_actions[#idle_actions + 1] = sound(frame, "snd_hermit_storm_idle")
end
local idle = sf2.moves.register {
    id = "hermit_storm_idle", animation = sf2.assets.binary("animations/hermit_super_attack_idle"),
    mid_frames = 2, first_frame = 3, end_frame = 54, priority = 999,
    mirror_node = "NHeel_1", tactic_equivalent = "StanceIdle", ends_stage = true,
    align = { axes = { "X", "Z" }, pivot = point("Nodes", "NHeel_2"), position = point("Pivot", nil, "Me") },
    events = { "animation_end" },
    conditions = { { type = "any", conditions = { current("player"), current("idle"), current("win") } } },
    locks = { { type = "perk", perk = storm_perk } },
    intervals = { { name = "Unstable" }, { name = "SemiUninterrupt", start = 36 },
        { name = "Uninterrupt", ["end"] = 35 } },
    actions = idle_actions,
}

local edges = { "EArm_1", "EArm_2", "EChest", "EForearm_1", "EForearm_2",
    "EHand_2", "EFingers_2", "EForearm_1", "EHand_1", "EFingers_1" }
local terms = { { type = "WeaponDamage" }, { type = "UnarmedDamage", shift = -10 } }
local function attack(start, finish, hit)
    return { type = "Attack", start = start, ["end"] = finish, attack = {
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
        { type = "distance", axis = "X", minimum = 100,
            from = point("Pivot", nil, "Me"), to = point("Nodes", "NPivot", "Enemy") },
        { type = "distance", axis = "X", minimum = 200,
            from = point("Wall", "Back", "Me"), to = point("Nodes", "NHeel_1", "Me") },
        { type = "distance", axis = "X", minimum = 200,
            from = point("Nodes", "NHeel_1", "Me"), to = point("Wall", "Front", "Me") },
    },
    align = { axes = { "X", "Z" }, pivot = point("Nodes", "NHeel_1"), position = point("Pivot", nil, "Me") },
    conditions = {
        { type = "keys", keys = { { key = "RaidCharge" } } },
        { type = "mod_exists", name = "HermitStormRecharge", ["not"] = true },
        { type = "current_interval", name = "SemiUninterrupt", ["not"] = true },
        { type = "current_interval", name = "Uninterrupt", ["not"] = true },
        { type = "round_stage", name = "Fight" },
        { type = "all", ["not"] = true, conditions = {
            { type = "current_animation", name = "$Move" },
            { type = "current_interval", name = "SemiUninterrupt" },
        } },
        { type = "current_animation", name = "Physical", ["not"] = true },
    },
    locks = { { type = "perk", perk = storm_perk },
        { type = "item", item_type = "Skeleton", item_subtype = "Skeleton" } },
    intervals = { { name = "Unstable" }, { name = "Uninterrupt", ["end"] = 36 },
        { name = "SemiUninterrupt", start = 36 },
        attack(8, 9, "High"), attack(11, 19, "Low"), attack(20, 27, "Low") },
    actions = {
        spawn(20, storm_item),
        { type = "effect", frame = 19, effect = {
            name = "HermitStormLevitation", core_sequence = "mgc_effect_levitation_middle",
            scale = 1.5, time_scale = 2, looped = true, on_background = true,
            position = point("Nodes", "NStomach", "Me", 3, 15), follow = true,
        } },
        { type = "stop_effect", event = "Hit", effect_name = "HermitStormLevitation" },
        sound(1, "snd_hermit_storm_start"), sound(31, "snd_hermit_storm_idle"),
        sound(39, "snd_hermit_storm_idle"),
        { type = "random_sound", event = "Strike", core_sounds = {
            "snd_hit1", "snd_hit2", "snd_hit3", "snd_hit4", "snd_hit5", "snd_hit6" } },
    },
    events = { "key_pressed", { type = "interval_end", name = "Uninterrupt" }, "animation_end" },
    direction = { from = point("Nodes", "NPivot", "Me"), to = point("Nodes", "NPivot", "Enemy") },
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
