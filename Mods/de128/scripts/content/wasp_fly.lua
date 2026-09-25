local sf2 = require("sf2")

-- The four archived Wasp Fly ranges replace the old core Super-button moves.
-- All animation bytes are copied unchanged from recovered Resources. DE128
-- never loads move XML at runtime.

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
    local timeline = {
        [1] = { sound = "snd_wasp_fly_start" },
        [4] = {
            { play_sound = "snd_m_pl_attack3", voice = "Male" },
            { play_sound = "snd_low_pl_attack3", voice = "MaleLow" },
            { play_sound = "snd_f_pl_attack3", voice = "Female" },
        },
        [14] = { { sound = "snd_swish2" }, { sound = "snd_wasp_fly_mid" } },
        [26] = { sound = "snd_wasp_fly_end" },
        [variant.effect_end] = { effect = {
            name = "WaspSpeedSplitWingsEnd", core_sequence = "mgc_wasp_speed_split_wings_end",
            scale = 1.35, time_scale = 1.9,
            position = { node = "NPivot", player = "Me", x = 140, y = 14 }, follow = false,
        } },
        strike = { sound = { "snd_hit1", "snd_hit2", "snd_hit3", "snd_hit4", "snd_hit5", "snd_hit6" } },
    }
    if variant.effect_start then
        local start = variant.effect_start
        table.insert(timeline[4], 1, { effect = {
            name = start[1], core_sequence = start[2], scale = 1.35, time_scale = 1.9,
            position = { node = "NPivot", player = "Me", x = start[3], y = start[4] }, follow = false,
        } })
    end
    moves[variant.suffix] = sf2.moves.register {
        id = "wasp_fly_" .. variant.suffix,
        animation = sf2.assets.binary("animations/" .. variant.binary),
        core_templates = { "1key", "WaspFly", "BossAbility", "Controlled", "SoundStrike" },
        mid_frames = 2, no_wall_repulsion = true, first_frame = 1, priority = 110,
        mirror_node = "NHeel_1",
        align = { axes = { "X", "Z" }, pivot = { animation = true },
            position = { wall = "Back", player = "Me", x = variant.align_x } },
        conditions = {
            { keys = { "RaidCharge" } },
            { not_mod = "WaspFlyRecharge" },
            { distance = "X", min = variant.wall_min, max = variant.wall_max, from = { wall = "Back", player = "Me" }, to = { node = "NHeel_1", player = "Me" } },
            { not_interval = "SemiUninterrupt" },
            { not_mod = "CurseBomb", player = "Enemy" },
            { not_interval = "Uninterrupt" },
            { stage = "Fight" },
            { not_all = {
                { animation = "$Move" },
                { interval = "SemiUninterrupt" },
            } },
            { not_animation = "Physical" },
        },
        locks = {
            { perk = sf2.perks.get("core:perks/PERK_WASPFLY") },
            { item = "Skeleton", subtype = "Skeleton" },
        },
        intervals = {
            { type = "Invulnerable", name = "Boss" },
            { name = "Unstable", to = variant.unstable_end },
            { name = "Uninterrupt", to = variant.end_frame },
            { type = "Block", from = variant.block_start },
            { name = "Throwable", from = variant.block_start },
            { type = "Attack", from = variant.attack_start, to = variant.attack_end, attack = { edges = edges, damage = 0.3,
                    damage_terms = { WeaponDamage = 0, UnarmedDamage = -10 },
                    impulse = { x = 350 }, hit = "WaspFly",
                    options = { ignores_block = true, ignores_all_invulnerable = true } } },
        },
        timeline = timeline,
        events = { "key_pressed", { interval_end = "Uninterrupt" }, "animation_end" },
        direction = { from = { node = "NPivot", player = "Me" }, to = { node = "NPivot", player = "Enemy" } },
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
