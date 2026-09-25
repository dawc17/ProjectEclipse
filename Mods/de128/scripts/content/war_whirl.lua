local sf2 = require("sf2")

-- The archived War boss replaces the older Super-button whirlwind with a
-- RaidCharge cast. Its animation is copied unchanged from packaged core data.
sf2.moves.patch { move = "MagicWarAbilityPlayer", disable = true }

local HITS = { "snd_hit1", "snd_hit2", "snd_hit3", "snd_hit4", "snd_hit5", "snd_hit6" }

local function whirl(name, sequence, looped, time_scale)
    return { effect = {
        name = name, core_sequence = sequence, scale = 1.5, time_scale = time_scale, looped = looped,
        position = { node = "NStomach", player = "Me", x = 3, y = 15 }, follow = true,
    } }
end

local move = sf2.moves.register {
    id = "war_whirl_player",
    animation = "animations/boss_war_ability",
    core_templates = { "1key", "MagicPlayer", "Controlled", "SoundStrike" },
    mid_frames = 2, first_frame = 1, end_frame = 81, priority = 110,
    mirror_node = "NHeel_1",
    events = "controlled",
    direction = "face_enemy",
    align = { axes = { "X", "Z" }, pivot = { node = "NHeel_1" }, position = { pivot = "Me" } },

    conditions = {
        { key = "RaidCharge" },
        { not_mod = "WarRecharge" }, { not_mod = "Concussion" }, { not_mod = "Stun" },
        { controllable = true },
    },
    locks = {
        { perk = sf2.perks.get("core:perks/PERK_WAR_WHIRL") },
        { item = "Skeleton", subtype = "Skeleton" },
    },
    tactic_conditions = {
        { distance = "X", max = 600, from = { pivot = "Me" }, to = { node = "NPivot", player = "Enemy" } },
        { distance = "X", min = 400, from = { node = "NPivot", player = "Me" }, to = { wall = "Front", player = "Me" } },
    },

    intervals = {
        { name = "Uninterrupt", to = 80 },
        { type = "Attack", from = 14, to = 70, attack = {
            edges = { "EHand_1", "EFingers_1", "EArm_1", "EArm_2", "EForearm_2", "EHand_2", "EFingers_2", "EChest" },
            damage = 0.40, damage_terms = { MagicDamage = 0, UnarmedDamage = -25 },
            impulse = { x = 150, y = -600 }, hit = "Physycal",
            options = { no_critical = true, body_part = "Body", defense_types = { "BodyDefense" },
                ignores_block = true, ignores_invulnerable = { "Evade", "Recovery", "Dash" } },
        } },
    },

    timeline = {
        [3] = { whirl("MagicWarWhirlffectStart", "mgc_war_ability_start", false, 3), { sound = "snd_blade_fury" } },
        [13] = whirl("MagicWarWhirlffectMiddle", "mgc_war_ability_middle", true, 1),
        [60] = { { stop_effect = "MagicWarWhirlffectMiddle" },
            whirl("MagicWarWhirlffectMiddle", "mgc_war_ability_end", false, 3) },
        hit = { { stop_effect = "MagicWarWhirlffectMiddle" }, { stop_sound = "snd_blade_fury" } },
        animation_end = { { stop_effect = "MagicWarWhirlffectMiddle" }, { stop_sound = "snd_blade_fury" } },
        strike = { sound = HITS },
    },
}

local tactic = sf2.tactics.register {
    id = "war_whirl", template = "Aggressive",
    on_decide = function(memory, event)
        if event.seconds < 2 or event.seconds < (memory.next_whirl or 0) then return nil end
        for _, action in ipairs(event.actions) do
            if action.name == "de128:moves/war_whirl_player" then
                memory.next_whirl = event.seconds + 6
                return action
            end
        end
        return nil
    end,
}

return { move = move, tactic = sf2.tactics.name(tactic) }
