local sf2 = require("sf2")
sf2.state.register {
    version = 1,
    fields = { route_rng = { type = sf2.state.INTEGER, default = 12345 } },
}
local function alias(key) return "example.seeded-trial:localization/" .. key end
local arena = sf2.locations.register {
    id = "arena", color = "0x1b2230", wall = 200, floor = 80,
    width = 1936, height = 512, min_width = 1936,
    layers = {
        { type = 1, factor = 1, images = {
            { sprite = sf2.assets.sprite("core:Textures/Locations/battlefield/battlefield_bg1.back_1"),
              x = 0, y = 0, width = 1936, height = 1024 },
        } },
        { type = 2, factor = 1, fighters = { player_x = 868, player_y = -94, enemy_x = 1068, enemy_y = -94 } },
    },
}
local location = sf2.locations.name(arena)
-- Native templates retain the original portraits, rigs, clothing and weapons.
local fighter_templates = { "man_kungfu", "girl_sai", "man_nunchaku" }
local fighters = {}
for i, template in ipairs(fighter_templates) do
    fighters[i] = sf2.warriors.register {
        id = i == 1 and "fighter" or "fighter_" .. i,
        template = sf2.warriors.get_template("core:warrior-templates/" .. template),
        tactic = "Standard", first_name = alias("fighter." .. i), last_name = "", level = 1,
    }
end
local loss = sf2.rewards.register { id = "loss", items = {} }
local win = sf2.rewards.register { id = "win", items = {} }
local zone = sf2.zones.register { id = "trial", file = "Map1.1", start = false }
local battle = sf2.battles.register {
    id = "trial", zone = zone, type = sf2.battles.STORY, x = 0, y = 0,
    alias = alias("trial"), title = alias("trial"), description = alias("trial.description"), location = location,
}
local fights = {}
for i = 1, 3 do
    fights[i] = sf2.fights.register {
        id = "encounter_" .. i, battle = battle, rounds = 1, round_time = 99,
        location = location, warriors = { fighters[i] }, rewards = { loss, win },
    }
end
sf2.modes.register {
    id = "trial", fights = fights, repeatable = true,
    on_result = function(result)
        if not result.won then return fights[1] end
        if result.step == 3 then return "complete" end
        if result.step == 1 then return fights[sf2.random.integer("route_rng", 2, 3)] end
        return nil -- Use the next roster entry on the full route.
    end,
}

-- Registered battles need a map-session reveal before their zone is selectable.
sf2.quests.register {
    id = "entry", place = "map", events = { "session" },
    actions = { { type = "show_battle", battle = battle, locked = false } },
}
