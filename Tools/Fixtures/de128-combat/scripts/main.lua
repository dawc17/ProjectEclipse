local sf2 = require("sf2")
local arena = sf2.locations.register {
    id = "arena", color = "0x1b2230", wall = 200, floor = 80,
    width = 1936, height = 512, min_width = 1936,
    layers = {
        { type = 1, factor = 1, images = {
            { sprite = sf2.assets.sprite("core:Textures/Locations/battlefield/battlefield_bg1.back_1"),
              x = 0, y = 0, width = 1936, height = 1024 },
        } },
        { type = 2, factor = 1, fighters = { player_x = 468, player_y = -94, enemy_x = 1268, enemy_y = -94 } },
    },
}
local location = sf2.locations.name(arena)
local opponent = sf2.warriors.register {
    id = "jian", template = sf2.warriors.get_template("core:warrior-templates/man_staff"),
    tactic = "Standard", first_name = "DE128 acceptance", last_name = "", level = 1,
    items = { sf2.items.get("core:items/weapon/WEAPON_CHNY21_JIAN"),
        sf2.items.get("de128:items/magic/minor_charge_of_darkness") },
}
local zone = sf2.zones.register { id = "trial", file = "Map1.1", start = false }
local battle = sf2.battles.register { id = "trial", zone = zone, type = sf2.battles.STORY,
    x = 0, y = 0, alias = "DE128 acceptance", title = "DE128 acceptance", description = "", location = location }
local loss = sf2.rewards.register { id = "loss", items = {} }
local win = sf2.rewards.register { id = "win", items = {} }
local fight = sf2.fights.register { id = "jian", battle = battle, location = location,
    warriors = { opponent }, rewards = { loss, win }, rounds = 1, round_time = 99 }
sf2.modes.register { id = "trial", fights = { fight }, repeatable = true }

-- Inert move definitions used to inspect actual native action parsing after boot.
local fixture_animation = sf2.assets.binary("de128:animations/chinese_swords_super_slash_old")
local child = sf2.moves.register { id = "projectile_child", animation = fixture_animation,
    conditions = {
        { type = "actor_name", name = "FixtureSphere" },
        { type = "bullets", bullet_type = "MagicBullet", minimum = 1, maximum = 2 },
    }, velocity = { x = 30, ay = -2, save_velocity = true }, no_magic_recharge = true,
}
sf2.moves.register { id = "projectile_actions", animation = fixture_animation, actions = {
    { type = "create_projectile", frame = 2, projectile = {
        name = "Sphere1", core_skeleton = "SkeletonMagic", copy_parent_type = "Magic",
    } },
    { type = "create_projectile", frame = 3, projectile = {
        name = "Preview", core_skeleton = "SkeletonMagic", copy_parent_type = "Magic", start_move = child,
    } },
    { type = "add_bullets", frame = 7, bullets = { type = "MagicBullet", value = -1 } },
    { type = "delete_actor", event = "Strike", player = "Me" },
} }
