local sf2 = require("sf2")
local function alias(key) return "example.pulse-guardian:localization/" .. key end
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

local view, frames, stopped, landed = nil, 0, 0, 0
local function close()
    if view then sf2.ui.close(view) end
    view = nil
end
local function refresh()
    if not view or not sf2.ui.is_open(view) then return end
    local phase = frames % 360
    sf2.ui.set_text(view, "phase", phase < 180 and "SHIELD UP - immune" or "SHIELD DOWN - strike!")
    sf2.ui.set_value(view, "clock", (phase % 180) / 180)
    sf2.ui.set_text(view, "hits", "Stopped: " .. stopped .. "   Landed: " .. landed)
end
local pulse = sf2.behaviors.register {
    id = "pulse",
    on_round_begin = function()
        close(); frames, stopped, landed = 0, 0, 0
        view = sf2.ui.open {
            id = "pulse", mount = "hud", placement = { anchor = "top_right", x = -24, y = 104 },
            root = { id = "root", kind = "column", width = 390, height = 104, gap = 6, children = {
                { id = "phase", kind = "text", width = 390, height = 38, style = { font_size = 22 }, text = "SHIELD UP" },
                { id = "clock", kind = "progress", width = 390, height = 20 },
                { id = "hits", kind = "text", width = 390, height = 32, text = "Stopped: 0   Landed: 0" },
            } },
        }
        refresh()
    end,
    on_tick = function(_, _, event)
        frames = frames + event.delta_frames
        if event.frame % 6 == 0 then refresh() end
    end,
    on_damage_resolving = function(_, fighter, event)
        if event.damage <= 0 then return end
        if frames % 360 < 180 then
            fighter:scale_incoming_damage(0); stopped = stopped + 1
        else landed = landed + 1 end
        refresh()
    end,
    on_round_end = close, on_fight_end = close,
}
local rule = sf2.rules.behavior { id = "pulse", behavior = pulse, target = sf2.rules.OPPONENT }
local fighter = sf2.warriors.register {
    id = "guardian", template = sf2.warriors.get_template("core:warrior-templates/man_staff"),
    tactic = "Standard", first_name = alias("fighter"), last_name = "", level = 1,
}
local zone = sf2.zones.register { id = "trial", file = "Map1.1", start = false }
local battle = sf2.battles.register { id = "trial", zone = zone, type = sf2.battles.STORY, x = 0, y = 0,
    alias = alias("trial"), title = alias("trial"), description = alias("trial.description"), location = location }
local loss = sf2.rewards.register { id = "loss", items = {} }
local win = sf2.rewards.register { id = "win", items = {} }
local fight = sf2.fights.register { id = "guardian", battle = battle, location = location,
    warriors = { fighter }, rules = { rule }, rewards = { loss, win }, rounds = 1, round_time = 99 }
sf2.modes.register { id = "trial", fights = { fight }, repeatable = true }
sf2.quests.register { id = "entry", place = "map", events = { "session" },
    actions = { { type = "show_battle", battle = battle, locked = false } } }
