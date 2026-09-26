local sf2 = require("sf2")
local statusText = sf2.localization.key("charge.status")
local armedText = sf2.localization.key("charge.armed")
local armText = sf2.localization.key("charge.arm")
local view, charge, armed, chargeFrames = nil, 0, false, 0

local function refresh()
    if view and sf2.ui.is_open(view) then
        sf2.ui.set_value(view, "meter", charge)
        sf2.ui.set_text(view, "status", armed and sf2.localization.text(armedText) or
            string.format(sf2.localization.text(statusText), math.floor(charge * 100)))
        sf2.ui.set_text(view, "arm", sf2.localization.text(armText))
        sf2.ui.set_enabled(view, "arm", charge >= 1 and not armed)
    end
end

local function close()
    if view then sf2.ui.close(view) end
    view = nil
end

local behavior = sf2.behaviors.register {
    id = "charge",
    on_round_begin = function()
        close()
        charge, armed, chargeFrames = 0, false, 0
        view = sf2.ui.open {
            id = "charge", mount = "hud",
            placement = { anchor = "top_right", x = -24, y = 104 },
            root = { id = "root", kind = "column", width = 320, height = 116, gap = 8,
                children = {
                    { id = "status", kind = "text", width = 320, height = 32, style = { font_size = 24 }, text = string.format(sf2.localization.text(statusText), 0) },
                    { id = "meter", kind = "progress", width = 320, height = 20 },
                    { id = "arm", kind = "button", width = 320, height = 48,
                      text = sf2.localization.text(armText), enabled = false },
                },
            },
            on_close = function(closed)
                if view == closed then
                    view = nil
                    charge, armed, chargeFrames = 0, false, 0
                end
            end,
            on_click = function(_, widget)
                if widget == "arm" and charge >= 1 and not armed then
                    armed = true
                    refresh()
                end
            end,
        }
    end,
    on_tick = function(_, _, event)
        chargeFrames = math.min(300, chargeFrames + event.delta_frames)
        charge = chargeFrames / 300
        if event.frame % 6 == 0 then refresh() end
    end,
    on_damage_dealing = function(_, fighter, event)
        if armed and not event.blocked and event.damage > 0 then
            fighter:scale_outgoing_damage(2)
            charge, armed, chargeFrames = 0, false, 0
            refresh()
        end
    end,
    on_round_end = close,
    on_fight_end = close,
}
local rule = sf2.rules.behavior { id = "charge", behavior = behavior, target = sf2.rules.PLAYER }
sf2.fights.patch { target = "core:fights/zone_1/tournament/3", append_rules = { rule } }
-- Eclipse replays use a separate native battle/fight identity.
sf2.fights.patch { target = "core:fights/zone_1/tournament_eclipsemode/3", append_rules = { rule } }
