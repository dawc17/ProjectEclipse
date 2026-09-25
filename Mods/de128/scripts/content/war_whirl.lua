local sf2 = require("sf2")

-- The archived War boss replaces the older Super-button whirlwind with a
-- RaidCharge cast. Its animation is copied unchanged from packaged core data.
local function point(object, part, player, x, y)
    return { object = object, part = part, player = player, shift_x = x, shift_y = y }
end

local function effect(name, sequence, frame, looped, time_scale)
    return { type = "effect", frame = frame, effect = {
        name = name, core_sequence = sequence, scale = 1.5,
        time_scale = time_scale, looped = looped,
        position = point("Nodes", "NStomach", "Me", 3, 15), follow = true,
    } }
end

sf2.moves.patch { move = "MagicWarAbilityPlayer", disable = true }

local move = sf2.moves.register {
    id = "war_whirl_player",
    animation = sf2.assets.binary("animations/boss_war_ability"),
    core_templates = { "1key", "MagicPlayer", "Controlled", "SoundStrike" },
    mid_frames = 2, first_frame = 1, end_frame = 81, priority = 110,
    mirror_node = "NHeel_1",
    tactic_conditions = {
        { type = "distance", axis = "X", maximum = 600,
            from = point("Pivot", nil, "Me"), to = point("Nodes", "NPivot", "Enemy") },
        { type = "distance", axis = "X", minimum = 400,
            from = point("Nodes", "NPivot", "Me"), to = point("Wall", "Front", "Me") },
    },
    align = { axes = { "X", "Z" }, pivot = point("Nodes", "NHeel_1"),
        position = point("Pivot", nil, "Me") },
    conditions = {
        { type = "keys", keys = { { key = "RaidCharge" } } },
        { type = "mod_exists", name = "WarRecharge", ["not"] = true },
        { type = "current_interval", name = "SemiUninterrupt", ["not"] = true },
        { type = "mod_exists", name = "Concussion", ["not"] = true },
        { type = "current_interval", name = "Uninterrupt", ["not"] = true },
        { type = "round_stage", name = "Fight" },
        { type = "all", ["not"] = true, conditions = {
            { type = "current_animation", name = "$Move" },
            { type = "current_interval", name = "SemiUninterrupt" },
        } },
        { type = "current_animation", name = "Physical", ["not"] = true },
        { type = "mod_exists", name = "Stun", ["not"] = true },
    },
    locks = {
        { type = "perk", perk = sf2.perks.get("core:perks/PERK_WAR_WHIRL") },
        { type = "item", item_type = "Skeleton", item_subtype = "Skeleton" },
    },
    intervals = {
        { name = "Uninterrupt", ["end"] = 80 },
        { type = "Attack", start = 14, ["end"] = 70, attack = {
            edges = { "EHand_1", "EFingers_1", "EArm_1", "EArm_2",
                "EForearm_2", "EHand_2", "EFingers_2", "EChest" },
            damage = 0.40,
            damage_terms = { { type = "MagicDamage" }, { type = "UnarmedDamage", shift = -25 } },
            impulse = { x = 150, y = -600 }, hit = "Physycal",
            options = { no_critical = true, body_part = "Body", defense_types = { "BodyDefense" },
                ignores_block = true, ignores_invulnerable = { "Evade", "Recovery", "Dash" } },
        } },
    },
    actions = {
        effect("MagicWarWhirlffectStart", "mgc_war_ability_start", 3, false, 3),
        effect("MagicWarWhirlffectMiddle", "mgc_war_ability_middle", 13, true, 1),
        { type = "stop_effect", frame = 60, effect_name = "MagicWarWhirlffectMiddle" },
        effect("MagicWarWhirlffectMiddle", "mgc_war_ability_end", 60, false, 3),
        { type = "random_sound", frame = 3, core_sounds = { "snd_blade_fury" } },
        { type = "stop_effect", event = "AnimationEnd", effect_name = "MagicWarWhirlffectMiddle" },
        { type = "stop_effect", event = "Hit", effect_name = "MagicWarWhirlffectMiddle" },
        { type = "stop_sound", event = "AnimationEnd", core_sound = "snd_blade_fury" },
        { type = "stop_sound", event = "Hit", core_sound = "snd_blade_fury" },
        { type = "random_sound", event = "Strike", core_sounds = {
            "snd_hit1", "snd_hit2", "snd_hit3", "snd_hit4", "snd_hit5", "snd_hit6" } },
    },
    events = { "key_pressed", { type = "interval_end", name = "Uninterrupt" }, "animation_end" },
    direction = { from = point("Nodes", "NPivot", "Me"), to = point("Nodes", "NPivot", "Enemy") },
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
