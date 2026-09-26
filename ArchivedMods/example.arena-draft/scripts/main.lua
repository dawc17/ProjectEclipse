local sf2 = require("sf2")
sf2.state.register {
    version = 1,
    fields = { route_rng = { type = sf2.state.INTEGER, default = 12345 } },
}
local function alias(key) return "example.arena-draft:localization/" .. key end
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
    on_prepare = function(request, event)
        local names = { "Wayfarer - barehand", "Needlehand - sais", "Storm Ronin - nunchaku" }
        local choices = { 1, 2, 3 }
        -- Fisher-Yates, backed by saved mod RNG state. Reopening a cancelled draft advances it.
        for i = #choices, 2, -1 do
            local j = sf2.random.integer("route_rng", 1, i)
            choices[i], choices[j] = choices[j], choices[i]
        end
        local selected, challenge, duration = choices[1], false, 60
        local cells = {}
        for i, candidate in ipairs(choices) do
            cells[i] = { id = "pick_" .. candidate, kind = "button", text = names[candidate] }
        end
        sf2.ui.open {
            id = "draft", mount = "modal",
            root = { id = "root", kind = "column", width = 570, height = 432, gap = 8, children = {
                { id = "title", kind = "text", width = 570, height = 40, text = "ARENA DRAFT - encounter " .. event.step },
                { id = "help", kind = "text", width = 570, height = 32, text = "Pick an opponent, then set the rules." },
                { id = "choices", kind = "grid", width = 570, height = 68, columns = 3, cell_width = 184, cell_height = 68, gap = 9, children = cells },
                { id = "selection", kind = "text", width = 570, height = 32, text = "Selected: " .. names[selected] },
                { id = "challenge", kind = "toggle", width = 570, height = 40, text = "Veteran: opponent gains 3 levels" },
                { id = "time", kind = "text", width = 570, height = 28, text = "Round time: 60 seconds" },
                { id = "duration", kind = "slider", width = 570, height = 36, value = 0.5 },
                { id = "begin", kind = "button", width = 570, height = 44, text = "FIGHT SELECTED OPPONENT" },
                { id = "cancel", kind = "button", width = 570, height = 48, text = "BACK" },
            } },
            on_change = function(view, id, value)
                if id == "challenge" then challenge = value
                elseif id == "duration" then duration = math.floor(30 + value * 60); sf2.ui.set_text(view, "time", "Round time: " .. duration .. " seconds") end
            end,
            on_click = function(view, id)
                local candidate = tonumber(id:match("^pick_(%d+)$"))
                if candidate and fighters[candidate] then selected = candidate; sf2.ui.set_text(view, "selection", "Selected: " .. names[selected])
                elseif id == "cancel" then sf2.ui.close(view)
                elseif id == "begin" and sf2.modes.is_pending(request) then
                    sf2.modes.resolve(request, { warriors = { fighters[selected] }, level = event.step + (challenge and 3 or 0), rounds = 1, round_time = duration })
                    sf2.ui.close(view)
                end
            end,
            on_close = function() sf2.modes.cancel(request) end,
        }
    end,
}

-- Registered battles need a map-session reveal before their zone is selectable.
sf2.quests.register {
    id = "entry", place = "map", events = { "session" },
    actions = { { type = "show_battle", battle = battle, locked = false } },
}
