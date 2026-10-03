local sf2 = require("sf2")
local beacon = sf2.assets.audio("audio/beacon")
local hud, sound
local function close()
    if hud and sf2.ui.is_open(hud) then sf2.ui.close(hud) end
    hud, sound = nil, nil -- The UI owner closes its sound automatically.
end
local behavior = sf2.behaviors.register {
    id = "lab",
    on_round_begin = function()
        close()
        hud = sf2.ui.open { id = "lab", mount = "hud",
            placement = { anchor = "bottom_right", x = -24, y = -24 },
            root = { id = "root", kind = "column", width = 300, height = 232,
                style = { background_color = "#fff4dbf0" }, children = {
                    { id = "status", kind = "text", text = "Audio Lab ready", height = 36,
                        style = { text_color = "#2b2119", font_size = 18 } },
                    { id = "game", kind = "button", text = "Loop (game clock)", height = 44 },
                    { id = "real", kind = "button", text = "Loop (real clock)", height = 44 },
                    { id = "quiet", kind = "button", text = "Volume 25%", height = 44 },
                    { id = "stop", kind = "button", text = "Stop", height = 44 },
                } },
            on_click = function(_, widget)
                if widget == "game" or widget == "real" then
                    if sound then sf2.audio.stop(sound) end
                    local error
                    sound, error = sf2.audio.play(beacon, { loop = true, clock = widget, owner = hud })
                    sf2.ui.set_text(hud, "status", sound and ("Loop: " .. widget) or "Audio unavailable")
                    if error then sf2.log.warn(error) end
                elseif widget == "quiet" and sound then
                    if sf2.audio.set_volume(sound, 0.25) then sf2.ui.set_text(hud, "status", "Volume 25%") end
                elseif widget == "stop" and sound then
                    sf2.audio.stop(sound)
                    sf2.ui.set_text(hud, "status", "Stopped")
                end
            end,
        }
    end,
    on_tick = function()
        -- Completed/stopped handles remain safe to query. Never save them.
        if sound and not sf2.audio.is_playing(sound) then sound = nil end
    end,
    on_round_end = close, on_fight_end = close,
}
local rule = sf2.rules.behavior { id = "lab", behavior = behavior, target = sf2.rules.PLAYER }
for _, fight in ipairs { "core:fights/zone_1/tournament/3", "core:fights/zone_1/tournament_eclipsemode/3" } do
    sf2.fights.patch { target = fight, append_rules = { rule } }
end
