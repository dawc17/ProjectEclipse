local sf2 = require("sf2")

-- The archived raid-charge Earthquake graph uses the hidden native earthquake
-- item as its child actor's weapon. Its animation bytes are unchanged core data.

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
    direction = { from = { wall = "Back", player = "Parent" }, to = { wall = "Front", player = "Parent" } },
    align = { axes = { "X", "Z" }, pivot = { node = "Magic-Node2_1" },
        position = { node = "NPivot", player = "Enemy" } },
    events = { "birth" },
    conditions = { { animation = "de128:moves/butcher_earthquake_player", player = "Parent" } },
    locks = {
        { item = "Weapon", name = "MAGIC_BUTCHER_EARTHQUAKE" },
        { item = "Skeleton", subtype = "SkeletonMagic" },
    },
    intervals = { { type = "Attack", from = 2, attack = {
        edges = { "Edge1" }, damage = 0.41,
        damage_terms = { MagicDamage = 0, UnarmedDamage = -25 },
        impulse = { x = 150, y = -300 }, hit = "Earthquake",
        options = { no_effect = true, no_critical = true, body_part = "Body",
            defense_types = { "BodyDefense" }, ignores_block = true,
            ignores_invulnerable = { "Evade", "Recovery", "Dash" } },
    } } },
    timeline = {
        animation_end = { delete_actor = "Me" },
    },
}

local player = sf2.moves.register {
    id = "butcher_earthquake_player",
    animation = sf2.assets.binary("animations/butcher_super_attack"),
    core_templates = { "1key", "BossAbility", "Controlled", "SoundStrike" },
    mid_frames = 2, no_wall_repulsion = true, first_frame = 1,
    priority = 110, mirror_node = "NHeel_1",
    tactic_conditions = {
        { not_animation = "Jump", player = "Enemy" },
        { any = {
            { distance = "X", min = 250, from = { pivot = "Me" }, to = { node = "NPivot", player = "Enemy" } },
            { animation = "Fall", player = "Enemy" },
        } },
    },
    align = { axes = { "X", "Z" }, pivot = { node = "NHeel_1" }, position = { pivot = "Me" } },
    conditions = {
        { keys = { "RaidCharge" } },
        { not_mod = "EarthquakeRecharge" },
        { not_interval = "SemiUninterrupt" },
        { not_interval = "Uninterrupt" },
        { stage = "Fight" },
        { not_all = {
            { animation = "$Move" },
            { interval = "SemiUninterrupt" },
        } },
        { not_animation = "Physical" },
    },
    locks = { { perk = earthquake_perk } },
    intervals = {
        { name = "Unstable", to = 24 },
        { name = "Uninterrupt", to = 32 },
        { type = "Block", from = 33 },
        { name = "Throwable", from = 33 },
    },
    timeline = {
        [22] = { { projectile = {
            name = "Earthquake", core_skeleton = "SkeletonMagic", item = earthquake_item, start_move = start,
        } }, { effect = {
            name = "ButcherEarthquake", core_sequence = "mgc_effect_fall", scale = 1,
            time_scale = 2, looped = false, position = { node = "NPivot", player = "Me", x = 40, y = -40 }, follow = false,
        } } },
        [23] = { { shake = {
            pause_time = 0, effect_time = 60, amplitude_x = 7, frequency_x = 1,
            amplitude_y = 18, frequency_y = 0.5,
        } }, { play_sound = "snd_bodyfall1", voice = "Male" }, { play_sound = "snd_bodyfall1", voice = "MaleLow" }, { sound = "snd_bucher_touchdown" } },
        [4] = { { play_sound = "snd_bucher_jump_new", voice = "Male" }, { play_sound = "snd_bucher_jump_new", voice = "MaleLow" } },
        strike = { sound = { "snd_hit1", "snd_hit2", "snd_hit3", "snd_hit4", "snd_hit5", "snd_hit6" } },
    },
    events = { "key_pressed", { interval_end = "Uninterrupt" }, "animation_end" },
    direction = { from = { node = "NPivot", player = "Me" }, to = { node = "NPivot", player = "Enemy" } },
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
