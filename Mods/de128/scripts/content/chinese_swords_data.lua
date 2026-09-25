-- Authored combat data of the archived ChineseSwordsSuperSlash.
-- Registered by content.chinese_swords; archive checks compare the native projection.
local function attack(first, last, edges, hit, x, y)
    return { type = "Attack", from = first, to = last, attack = {
            edges = edges, damage = 0.06,
            damage_terms = { WeaponDamage = 0, UnarmedDamage = -10 },
            impulse = { x = x, y = y, z = 0 }, hit = hit,
        } }
end

return {
    profile = { rank = 4, core_icon = "Trick7.super_slash" },
    tactic_distance = { distance = "X", min = 200, max = 800, from = { pivot = "Me" }, to = { node = "NPivot", player = "Enemy" } },
    timeline = {
        [8] = { sound = "snd_swish_sword1" },
        [17] = { sound = "snd_swish_sword1" },
        [28] = { sound = "snd_swish_sword3" },
        [33] = { sound = "snd_swish_sword2" },
        strike = { sound = { "snd_hit1", "snd_hit2", "snd_hit3", "snd_hit4", "snd_hit5", "snd_hit6" } },
    },
    item_lock_extensions = {
        "SaiStartStanceIdle", "SaiStartStance-Left", "SaiStartStance-Right", "Win_Sai",
        "SaiSpit", "SaiDoubleSpit", "SaiHeavySpit", "SaiSpinningSpit", "SaiUpperSpit", "SaiLowSpit",
    },
    locks = {
        { item = "Weapon", subtype = "ChineseSwords" },
        { item = "Skeleton", subtype = "Skeleton" },
    },
    transitions = {{ frame_shift = 2, conditions = {
        { animation = "SaiHeavySpit" },
        { interval = "SemiUninterrupt" },
    } }},
    align = {
        axes = { "X", "Z" },
        pivot = { node = "NHeel_2" },
        position = { pivot = "Me" },
    },
    direction = {
        from = { node = "NPivot", player = "Me" },
        to = { node = "NPivot", player = "Enemy" },
    },
    preview = {
        no_wall_repulsion = true, no_interpolation_frames = true,
        timeline = {
        [8] = { sound = "snd_swish_sword1" },
        [17] = { sound = "snd_swish_sword1" },
        [28] = { sound = "snd_swish_sword3" },
        [33] = { sound = "snd_swish_sword2" },
        animation_end = { try_on_end = true },
    },
        locks = {
            { screen = "ShopWeapon" },
            { item = "Weapon", subtype = "ChineseSwords" },
            { item = "Skeleton", subtype = "Skeleton" },
        },
        align = {
            axes = { "X", "Z" },
            pivot = { node = "NHeel_1" },
            position = { pivot = "Me", x = -57 },
        },
    },
    conditions = {
        { keys = { { "Punch", press = "Tap" }, { "Punch", press = "Tap" }, { "Forward", press = "Hold" } } },
        { any = {
            { animation = "1key" },
            { animation = "2key" },
            { not_interval = "SemiUninterrupt" },
        } },
        { any = {
            { animation = "Forward" },
            { not_interval = "SemiUninterrupt" },
        } },
        { not_interval = "Uninterrupt" },
        { stage = "Fight" },
        { not_all = {
            { animation = "$Move" },
            { interval = "SemiUninterrupt" },
        } },
        { not_animation = "Physical" },
        { not_mod = "MOD_TITAN" },
    },
    intervals = {
        { name = "Unstable", from = 24, to = 29 },
        { name = "Uninterrupt", to = 50 },
        { type = "Block", from = 51 },
        { name = "Throwable", from = 51 },
        attack(11, 14, { "WEAPON_SAI-Edge30_2", "WEAPON_SAI-Edge31_2" }, "Middle", 175, -100),
        attack(19, 23, { "WEAPON_SAI-Edge30_2", "WEAPON_SAI-Edge31_1", "WEAPON_SAI-Edge31_2" }, "Spinning", 25, -100),
        attack(27, 30, { "WEAPON_SAI-Edge30_1", "WEAPON_SAI-Edge31_1", "WEAPON_SAI-Edge31_2" }, "HighHeavy", 25, -120),
        attack(32, 35, { "WEAPON_SAI-Edge30_2", "WEAPON_SAI-Edge31_1", "WEAPON_SAI-Edge31_2" }, "HighHeavy", 25, -150),
    },
    preview_screen = { screen = "ShopWeapon" },
}
