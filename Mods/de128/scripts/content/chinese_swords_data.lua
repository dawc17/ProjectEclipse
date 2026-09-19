-- Authored combat data of the archived ChineseSwordsSuperSlash.
-- Registered by content.chinese_swords; definitions remain separate for archive checks.
-- No XML is read at runtime. Tests compare these Lua declarations to the archive.
local function attack(first, last, edges, hit, x, y)
    return {
        type = "Attack", start = first, ["end"] = last,
        attack = {
            edges = edges, damage = 0.06,
            damage_terms = {
                { type = "WeaponDamage" },
                { type = "UnarmedDamage", shift = -10 },
            },
            impulse = { x = x, y = y, z = 0 }, hit = hit,
        },
    }
end

return {
    profile = { rank = 4, core_icon = "Trick7.super_slash" },
    tactic_distance = {
        axis = "X", minimum = 200, maximum = 800,
        from = { player = "Me", object = "Pivot" },
        to = { player = "Enemy", object = "Nodes", part = "NPivot" },
    },
    actions = {
        { type = "random_sound", frame = 8, core_sounds = { "snd_swish_sword1" } },
        { type = "random_sound", frame = 17, core_sounds = { "snd_swish_sword1" } },
        { type = "random_sound", frame = 28, core_sounds = { "snd_swish_sword3" } },
        { type = "random_sound", frame = 33, core_sounds = { "snd_swish_sword2" } },
        { type = "random_sound", event = "Strike", core_sounds = { "snd_hit1", "snd_hit2", "snd_hit3", "snd_hit4", "snd_hit5", "snd_hit6" } },
    },
    item_lock_extensions = {
        "SaiStartStanceIdle", "SaiStartStance-Left", "SaiStartStance-Right", "Win_Sai",
        "SaiSpit", "SaiDoubleSpit", "SaiHeavySpit", "SaiSpinningSpit", "SaiUpperSpit", "SaiLowSpit",
    },
    locks = {
        { type = "item", item_type = "Weapon", item_subtype = "ChineseSwords" },
        { type = "item", item_type = "Skeleton", item_subtype = "Skeleton" },
    },
    transitions = {{ frame_shift = 2, conditions = {
        { type = "current_animation", name = "SaiHeavySpit" },
        { type = "current_interval", name = "SemiUninterrupt" },
    } }},
    align = {
        axes = { "X", "Z" },
        pivot = { object = "Nodes", part = "NHeel_2" },
        position = { player = "Me", object = "Pivot" },
    },
    direction = {
        from = { player = "Me", object = "Nodes", part = "NPivot" },
        to = { player = "Enemy", object = "Nodes", part = "NPivot" },
    },
    preview = {
        no_wall_repulsion = true, no_interpolation_frames = true,
        actions = {
            { type = "random_sound", frame = 8, core_sounds = { "snd_swish_sword1" } },
            { type = "random_sound", frame = 17, core_sounds = { "snd_swish_sword1" } },
            { type = "random_sound", frame = 28, core_sounds = { "snd_swish_sword3" } },
            { type = "random_sound", frame = 33, core_sounds = { "snd_swish_sword2" } },
            { type = "try_on_end", event = "AnimationEnd" },
        },
        locks = {
            { type = "screen", name = "ShopWeapon" },
            { type = "item", item_type = "Weapon", item_subtype = "ChineseSwords" },
            { type = "item", item_type = "Skeleton", item_subtype = "Skeleton" },
        },
        align = {
            axes = { "X", "Z" },
            pivot = { object = "Nodes", part = "NHeel_1" },
            position = { player = "Me", object = "Pivot", shift_x = -57 },
        },
    },
    conditions = {
        { type = "keys", keys = {
            { key = "Punch", press = "Tap" },
            { key = "Punch", press = "Tap" },
            { key = "Forward", press = "Hold" },
        } },
        { type = "any", conditions = {
            { type = "current_animation", name = "1key" },
            { type = "current_animation", name = "2key" },
            { type = "current_interval", name = "SemiUninterrupt", ["not"] = true },
        } },
        { type = "any", conditions = {
            { type = "current_animation", name = "Forward" },
            { type = "current_interval", name = "SemiUninterrupt", ["not"] = true },
        } },
        { type = "current_interval", name = "Uninterrupt", ["not"] = true },
        { type = "round_stage", name = "Fight" },
        { type = "all", ["not"] = true, conditions = {
            { type = "current_animation", name = "$Move" },
            { type = "current_interval", name = "SemiUninterrupt" },
        } },
        { type = "current_animation", name = "Physical", ["not"] = true },
        { type = "mod_exists", name = "MOD_TITAN", ["not"] = true },
    },
    intervals = {
        { name = "Unstable", start = 24, ["end"] = 29 },
        { name = "Uninterrupt", ["end"] = 50 },
        { type = "Block", start = 51 },
        { name = "Throwable", start = 51 },
        attack(11, 14, { "WEAPON_SAI-Edge30_2", "WEAPON_SAI-Edge31_2" }, "Middle", 175, -100),
        attack(19, 23, { "WEAPON_SAI-Edge30_2", "WEAPON_SAI-Edge31_1", "WEAPON_SAI-Edge31_2" }, "Spinning", 25, -100),
        attack(27, 30, { "WEAPON_SAI-Edge30_1", "WEAPON_SAI-Edge31_1", "WEAPON_SAI-Edge31_2" }, "HighHeavy", 25, -120),
        attack(32, 35, { "WEAPON_SAI-Edge30_2", "WEAPON_SAI-Edge31_1", "WEAPON_SAI-Edge31_2" }, "HighHeavy", 25, -150),
    },
    preview_screen = { type = "screen", name = "ShopWeapon" },
}
