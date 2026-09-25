local sf2 = require("sf2")

-- The four archived Wasp Fly ranges replace the old core Super-button moves.
-- All animation bytes are copied unchanged from recovered Resources. DE128
-- never loads move XML at runtime.
local point = function(object, part, player, x, y)
    return { object = object, part = part, player = player, shift_x = x, shift_y = y }
end
local function sound(frame, name, voice)
    return { type = "sound", frame = frame, sound = { core_sound = name, voice = voice } }
end
local function random(frame, ...)
    return { type = "random_sound", frame = frame, core_sounds = { ... } }
end
local function effect(name, sequence, frame, x, y)
    return { type = "effect", frame = frame, effect = {
        name = name, core_sequence = sequence, scale = 1.35, time_scale = 1.9,
        position = point("Nodes", "NPivot", "Me", x, y), follow = false,
    } }
end
local edges = {
    "EArm_1", "EArm_2", "EHead", "EChest", "EForearm_1", "EForearm_2",
    "EHand_2", "EFingers_2", "EForearm_1", "EHand_1", "EFingers_1",
}
local variants = {
    { suffix = "150", binary = "naginata_boss_1", wall_min = 1, wall_max = 150,
      align_x = -160, end_frame = 50, unstable_end = 45, block_start = 51,
      attack_start = 19, attack_end = 31, effect_end = 32 },
    { suffix = "200", binary = "naginata_boss", wall_min = 150, wall_max = 230,
      align_x = -70, end_frame = 50, unstable_end = 45, block_start = 51,
      attack_start = 19, attack_end = 31, effect_end = 32,
      effect_start = { "WaspSpeedSplitWingsStart", "mgc_wasp_speed_split_wings_start", -70, 65 } },
    { suffix = "300", binary = "naginata_boss_2", wall_min = 230, wall_max = 300,
      align_x = 0, end_frame = 50, unstable_end = 45, block_start = 51,
      attack_start = 19, attack_end = 31, effect_end = 32,
      effect_start = { "WaspSpeedSplitWingsStartTwo", "mgc_wasp_speed_split_wings_start_2", -75, 90 } },
    { suffix = "370", binary = "naginata_boss_3", wall_min = 300, wall_max = 370,
      align_x = 75, end_frame = 62, unstable_end = 55, block_start = 63,
      attack_start = 26, attack_end = 39, effect_end = 39 },
}
local moves = {}
for _, variant in ipairs(variants) do
    sf2.moves.patch { move = "WaspFly_" .. variant.suffix, disable = true }
    local actions = {}
    if variant.effect_start then
        local start = variant.effect_start
        actions[#actions + 1] = effect(start[1], start[2], 4, start[3], start[4])
    end
    actions[#actions + 1] = effect("WaspSpeedSplitWingsEnd", "mgc_wasp_speed_split_wings_end",
        variant.effect_end, 140, 14)
    actions[#actions + 1] = sound(4, "snd_m_pl_attack3", "Male")
    actions[#actions + 1] = sound(4, "snd_low_pl_attack3", "MaleLow")
    actions[#actions + 1] = sound(4, "snd_f_pl_attack3", "Female")
    actions[#actions + 1] = random(14, "snd_swish2")
    actions[#actions + 1] = random(1, "snd_wasp_fly_start")
    actions[#actions + 1] = random(14, "snd_wasp_fly_mid")
    actions[#actions + 1] = random(26, "snd_wasp_fly_end")
    actions[#actions + 1] = { type = "random_sound", event = "Strike",
        core_sounds = { "snd_hit1", "snd_hit2", "snd_hit3", "snd_hit4", "snd_hit5", "snd_hit6" } }
    moves[variant.suffix] = sf2.moves.register {
        id = "wasp_fly_" .. variant.suffix,
        animation = sf2.assets.binary("animations/" .. variant.binary),
        core_templates = { "1key", "WaspFly", "BossAbility", "Controlled", "SoundStrike" },
        mid_frames = 2, no_wall_repulsion = true, first_frame = 1, priority = 110,
        mirror_node = "NHeel_1",
        align = { axes = { "X", "Z" }, pivot = point("Animation"),
            position = point("Wall", "Back", "Me", variant.align_x) },
        conditions = {
            { type = "keys", keys = { { key = "RaidCharge" } } },
            { type = "mod_exists", name = "WaspFlyRecharge", ["not"] = true },
            { type = "distance", axis = "X", minimum = variant.wall_min, maximum = variant.wall_max,
                from = point("Wall", "Back", "Me"), to = point("Nodes", "NHeel_1", "Me") },
            { type = "current_interval", name = "SemiUninterrupt", ["not"] = true },
            { type = "mod_exists", name = "CurseBomb", player = "Enemy", ["not"] = true },
            { type = "current_interval", name = "Uninterrupt", ["not"] = true },
            { type = "round_stage", name = "Fight" },
            { type = "all", ["not"] = true, conditions = {
                { type = "current_animation", name = "$Move" },
                { type = "current_interval", name = "SemiUninterrupt" },
            } },
            { type = "current_animation", name = "Physical", ["not"] = true },
        },
        locks = {
            { type = "perk", perk = sf2.perks.get("core:perks/PERK_WASPFLY") },
            { type = "item", item_type = "Skeleton", item_subtype = "Skeleton" },
        },
        intervals = {
            { type = "Invulnerable", name = "Boss" },
            { name = "Unstable", ["end"] = variant.unstable_end },
            { name = "Uninterrupt", ["end"] = variant.end_frame },
            { type = "Block", start = variant.block_start },
            { name = "Throwable", start = variant.block_start },
            { type = "Attack", start = variant.attack_start, ["end"] = variant.attack_end,
                attack = { edges = edges, damage = 0.3,
                    damage_terms = { { type = "WeaponDamage" }, { type = "UnarmedDamage", shift = -10 } },
                    impulse = { x = 350 }, hit = "WaspFly",
                    options = { ignores_block = true, ignores_all_invulnerable = true } } },
        },
        actions = actions,
        events = { "key_pressed", { type = "interval_end", name = "Uninterrupt" }, "animation_end" },
        direction = { from = point("Nodes", "NPivot", "Me"), to = point("Nodes", "NPivot", "Enemy") },
    }
end

-- The recovered Aggressive tactic can leave Fly unused for an entire survival
-- wave. Keep its normal decisions, but choose an eligible archived Fly move once
-- the boss has fought for two seconds. Native cooldown and distance conditions
-- still decide whether the move appears in event.actions.
local tactic = sf2.tactics.register {
    id = "wasp_fly",
    template = "Aggressive",
    on_decide = function(memory, event)
        if event.seconds < 2 or event.seconds < (memory.next_fly or 0) then return nil end
        for _, action in ipairs(event.actions) do
            if action.name:sub(1, 21) == "de128:moves/wasp_fly_" then
                memory.next_fly = event.seconds + 5
                return action
            end
        end
        return nil
    end,
}

return { moves = moves, tactic = sf2.tactics.name(tactic) }
