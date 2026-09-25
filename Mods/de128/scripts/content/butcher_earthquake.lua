local sf2 = require("sf2")

-- The archived raid-charge Earthquake graph uses the hidden native earthquake
-- item as its child actor's weapon. Its animation bytes are unchanged core data.
local function point(object, part, player, x, y)
    return { object = object, part = part, player = player, shift_x = x, shift_y = y }
end
local function sound(frame, name, voice)
    return { type = "sound", frame = frame, sound = { core_sound = name, voice = voice } }
end
local function random(when, ...)
    return { type = "random_sound", frame = type(when) == "number" and when or nil,
        event = type(when) == "string" and when or nil, core_sounds = { ... } }
end
local earthquake_item = sf2.items.get("core:items/magic/MAGIC_BUTCHER_EARTHQUAKE")
local earthquake_perk = sf2.perks.get("core:perks/PERK_EARTHQUAKE")

sf2.moves.patch { move = "ButcherEarthquakePlayer", disable = true }
sf2.moves.patch { move = "ButcherEarthquakeStart", disable = true }

local start = sf2.moves.register {
    id = "butcher_earthquake_start",
    animation = sf2.assets.binary("animations/butcher_super_attack_activ"),
    core_templates = { "MagicMissileFly", "MagicMissile" },
    mid_frames = 2, no_wall_repulsion = true, no_magic_recharge = true,
    first_frame = 1, priority = 500,
    direction = { from = point("Wall", "Back", "Parent"), to = point("Wall", "Front", "Parent") },
    align = { axes = { "X", "Z" }, pivot = point("Nodes", "Magic-Node2_1"),
        position = point("Nodes", "NPivot", "Enemy") },
    events = { "birth" },
    conditions = { { type = "current_animation", player = "Parent", name = "de128:moves/butcher_earthquake_player" } },
    locks = {
        { type = "item", item_type = "Weapon", name = "MAGIC_BUTCHER_EARTHQUAKE" },
        { type = "item", item_type = "Skeleton", item_subtype = "SkeletonMagic" },
    },
    intervals = { { type = "Attack", start = 2, attack = {
        edges = { "Edge1" }, damage = 0.41,
        damage_terms = { { type = "MagicDamage" }, { type = "UnarmedDamage", shift = -25 } },
        impulse = { x = 150, y = -300 }, hit = "Earthquake",
        options = { no_effect = true, no_critical = true, body_part = "Body",
            defense_types = { "BodyDefense" }, ignores_block = true,
            ignores_invulnerable = { "Evade", "Recovery", "Dash" } },
    } } },
    actions = { { type = "delete_actor", event = "AnimationEnd", player = "Me" } },
}

local player = sf2.moves.register {
    id = "butcher_earthquake_player",
    animation = sf2.assets.binary("animations/butcher_super_attack"),
    core_templates = { "1key", "BossAbility", "Controlled", "SoundStrike" },
    mid_frames = 2, no_wall_repulsion = true, first_frame = 1,
    priority = 110, mirror_node = "NHeel_1",
    tactic_conditions = {
        { type = "current_animation", player = "Enemy", name = "Jump", ["not"] = true },
        { type = "any", conditions = {
            { type = "distance", axis = "X", minimum = 250,
                from = point("Pivot", nil, "Me"), to = point("Nodes", "NPivot", "Enemy") },
            { type = "current_animation", player = "Enemy", name = "Fall" },
        } },
    },
    align = { axes = { "X", "Z" }, pivot = point("Nodes", "NHeel_1"), position = point("Pivot", nil, "Me") },
    conditions = {
        { type = "keys", keys = { { key = "RaidCharge" } } },
        { type = "mod_exists", name = "EarthquakeRecharge", ["not"] = true },
        { type = "current_interval", name = "SemiUninterrupt", ["not"] = true },
        { type = "current_interval", name = "Uninterrupt", ["not"] = true },
        { type = "round_stage", name = "Fight" },
        { type = "all", ["not"] = true, conditions = {
            { type = "current_animation", name = "$Move" },
            { type = "current_interval", name = "SemiUninterrupt" },
        } },
        { type = "current_animation", name = "Physical", ["not"] = true },
    },
    locks = { { type = "perk", perk = earthquake_perk } },
    intervals = {
        { name = "Unstable", ["end"] = 24 },
        { name = "Uninterrupt", ["end"] = 32 },
        { type = "Block", start = 33 },
        { name = "Throwable", start = 33 },
    },
    actions = {
        { type = "create_projectile", frame = 22, projectile = {
            name = "Earthquake", core_skeleton = "SkeletonMagic", item = earthquake_item, start_move = start,
        } },
        { type = "shake_screen", frame = 23, shake = {
            pause_time = 0, effect_time = 60, amplitude_x = 7, frequency_x = 1,
            amplitude_y = 18, frequency_y = 0.5,
        } },
        { type = "effect", frame = 22, effect = {
            name = "ButcherEarthquake", core_sequence = "mgc_effect_fall", scale = 1,
            time_scale = 2, looped = false, position = point("Nodes", "NPivot", "Me", 40, -40), follow = false,
        } },
        sound(23, "snd_bodyfall1", "Male"),
        sound(4, "snd_bucher_jump_new", "Male"),
        sound(23, "snd_bodyfall1", "MaleLow"),
        sound(4, "snd_bucher_jump_new", "MaleLow"),
        random(23, "snd_bucher_touchdown"),
        random("Strike", "snd_hit1", "snd_hit2", "snd_hit3", "snd_hit4", "snd_hit5", "snd_hit6"),
    },
    events = { "key_pressed", { type = "interval_end", name = "Uninterrupt" }, "animation_end" },
    direction = { from = point("Nodes", "NPivot", "Me"), to = point("Nodes", "NPivot", "Enemy") },
}

local tactic = sf2.tactics.register {
    id = "butcher_earthquake", template = "Aggressive",
    on_decide = function(memory, event)
        if event.seconds < (memory.next_quake or 0) then return nil end
        for _, action in ipairs(event.actions) do
            if action.name == "de128:moves/butcher_earthquake_player" then
                memory.next_quake = event.seconds + 6
                return action
            end
        end
        return nil
    end,
}

return { player = player, start = start, tactic = sf2.tactics.name(tactic) }
