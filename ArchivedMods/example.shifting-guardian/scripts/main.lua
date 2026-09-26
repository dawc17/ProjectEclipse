local sf2 = require("sf2")
local function alias(key) return "example.shifting-guardian:localization/" .. key end
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

local second_form = sf2.warriors.register {
    id = "baton", template = sf2.warriors.get_template("core:warrior-templates/man_batons"),
    tactic = "Standard", first_name = alias("fighter.second"), last_name = "", level = 1,
}
local view, frames, requested, pending
local function close()
    if view then sf2.ui.close(view) end
    view, pending = nil, nil
end
local behavior = sf2.behaviors.register {
    id = "shift",
    on_round_begin = function()
        close(); frames, requested = 0, false
        view = sf2.ui.open {
            id = "shift", mount = "hud", placement = { anchor = "top_right", x = -24, y = 104 },
            root = { id = "root", kind = "column", width = 420, height = 124, gap = 6, children = {
                { id = "phase", kind = "text", width = 420, height = 36, text = "STAFF FORM", style = { font_size = 22 } },
                { id = "clock", kind = "progress", width = 420, height = 20 },
                { id = "result", kind = "text", width = 420, height = 56, text = "Baton form in 3 seconds", style = { font_size = 16 } },
            } },
        }
    end,
    on_tick = function(_, fighter, event)
        frames = frames + event.delta_frames
        if not view or not sf2.ui.is_open(view) then return end
        sf2.ui.set_value(view, "clock", math.min(frames / 180, 1))
        if frames >= 180 and not requested then
            requested = true
            pending = fighter:change_form(second_form)
            sf2.ui.set_text(view, "result", "Form request queued")
        end
        if pending and pending.status == "applied" then
            sf2.ui.set_text(view, "phase", "BATON FORM")
            sf2.ui.set_text(view, "result", "Applied: same fight, health percentage retained")
            pending = nil
        elseif pending and pending.status == "failed" then
            sf2.ui.set_text(view, "result", "Failed: " .. pending.error)
            pending = nil
        end
    end,
    on_round_end = close, on_fight_end = close,
}
local rule = sf2.rules.behavior { id = "shift", behavior = behavior, target = sf2.rules.OPPONENT }
local first_form = sf2.warriors.register {
    id = "staff", template = sf2.warriors.get_template("core:warrior-templates/man_staff"),
    tactic = "Standard", first_name = alias("fighter"), last_name = "", level = 1,
}
local zone = sf2.zones.register { id = "trial", file = "Map1.1", start = false }
local battle = sf2.battles.register { id = "trial", zone = zone, type = sf2.battles.STORY, x = 0, y = 0,
    alias = alias("trial"), title = alias("trial"), description = alias("trial.description"), location = location }
local loss = sf2.rewards.register { id = "loss", items = {} }
local win = sf2.rewards.register { id = "win", items = {} }
local fight = sf2.fights.register { id = "guardian", battle = battle, location = location,
    warriors = { first_form }, rules = { rule }, rewards = { loss, win }, rounds = 1, round_time = 99 }
sf2.modes.register { id = "trial", fights = { fight }, repeatable = true }
sf2.quests.register { id = "entry", place = "map", events = { "session" },
    actions = { { type = "show_battle", battle = battle, locked = false } } }
