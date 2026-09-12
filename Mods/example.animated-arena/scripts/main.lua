local sf2 = require("sf2")
local function text(key) return sf2.mod.id .. ":localization/" .. key end

local arena = sf2.locations.register {
    id = "arena", width = 1936, height = 512,
    music_choices = {
        sf2.assets.audio("core:gamedata/music/fight1_samurai_spirit"),
        sf2.assets.audio("core:gamedata/music/fight2_blade_dance"),
    },
    layers = {
        { images = { {
            sprite = sf2.assets.sprite("core:Textures/Locations/battlefield/battlefield_bg1.back_1"),
            width = 1936, height = 1024,
            motion_y = { points = { { period = 2, value = -16 }, { period = 2, value = 16 } } },
            opacity = { points = { { period = 2, value = 65 }, { period = 2, value = 100 } } },
        } } },
        { type = 2, fighters = { player_x = 868, player_y = -94, enemy_x = 1068, enemy_y = -94 } },
    },
}
local arena_name = sf2.locations.name(arena)
local guardian = sf2.warriors.register {
    id = "guardian", template = sf2.warriors.get_template("core:warrior-templates/default"),
    tactic = "Standard", first_name = text("opponent"), last_name = "", level = 1,
}
local zone = sf2.zones.register { id = "trial", file = "Map1.1", start = false }
local battle = sf2.battles.register {
    id = "trial", zone = zone, type = sf2.battles.STORY, x = 0, y = 0,
    alias = text("trial"), title = text("trial"), description = text("description"), location = arena_name,
}
local fight = sf2.fights.register {
    id = "trial", battle = battle, warriors = { guardian },
    rounds = 3, round_time = 99, location = arena_name,
}
sf2.modes.register { id = "trial", fights = { fight }, repeatable = true }
-- Use the existing map-entry compatibility adapter; the combat mechanic is Lua.
sf2.quests.register {
    id = "entry", place = "map", events = { "session" },
    actions = { { type = "show_battle", battle = battle, locked = false } },
}
