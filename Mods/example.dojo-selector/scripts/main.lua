local sf2 = require("sf2")
local function text(key) return sf2.mod.id .. ":localization/" .. key end

local arena = sf2.locations.register {
    id = "arena", dojo = true, width = 1936, height = 512,
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
sf2.modes.register {
    id = "trial", fights = { fight }, repeatable = true,
    on_prepare = function(request)
        local selected = sf2.locations.selected_dojo()
        sf2.ui.open {
            id = "dojo_selector", mount = "menu",
            root = { id = "root", kind = "column", width = 460, height = 320, gap = 12, children = {
                { id = "title", kind = "text", width = 460, height = 70, text = "Choose your dojo" },
                { id = "select", kind = "button", width = 460, height = 60, text = "BATTLEFIELD DOJO" },
                { id = "reset", kind = "button", width = 460, height = 60, text = "RESTORE DEFAULT",
                  enabled = selected == nil or selected == arena_name },
                { id = "back", kind = "button", width = 460, height = 60, text = "BACK" },
            } },
            on_click = function(view, id)
                if not sf2.modes.is_pending(request) then sf2.ui.close(view); return end
                if id == "select" then sf2.locations.select_dojo(arena)
                elseif id == "reset" then sf2.locations.reset_dojo() end
                sf2.ui.close(view)
            end,
            on_close = function() sf2.modes.cancel(request) end,
        }
    end,
}
-- The map entry opens the selector; preparation is cancelled without starting combat.
sf2.quests.register {
    id = "entry", place = "map", events = { "session" },
    actions = { { type = "show_battle", battle = battle, locked = false } },
}
