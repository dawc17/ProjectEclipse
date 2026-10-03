local sf2 = require("sf2")
local hud, requested
local function close_hud()
    if hud and sf2.ui.is_open(hud) then sf2.ui.close(hud) end
    hud, requested = nil, false
end
local pulse = sf2.behaviors.register {
    id = "repulse",
    state = { lifetime = "round", fields = {
        cooldown = { type = "integer", default = 0 },
    } },
    on_round_begin = function()
        close_hud()
        hud = sf2.ui.open {
            id = "repulse", mount = "hud",
            placement = { anchor = "top_left", x = 24, y = 216 },
            root = { id = "pulse", kind = "column", width = 280, height = 96,
                style = { background_color = "#fff4dbf0" },
                children = {
                    { id = "status", kind = "text", text = "Repulse ready", height = 36,
                        style = { text_color = "#2b2119", font_size = 18 } },
                    { id = "cast", kind = "button", text = "Repulse", height = 48 },
                } },
            -- Store intent, never a fighter handle from an earlier callback.
            on_click = function(_, widget)
                if widget == "cast" then requested = true end
            end,
        }
    end,
    on_tick = function(self, fighter, tick)
        local old = self.state.cooldown
        self.state.cooldown = math.max(0, old - tick.delta_frames)
        if old > 0 and self.state.cooldown == 0 and hud and sf2.ui.is_open(hud) then
            sf2.ui.set_text(hud, "status", "Repulse ready")
            sf2.ui.set_enabled(hud, "cast", true)
        end
        if not requested then return end
        requested = false
        if self.state.cooldown > 0 then return end
        local view = fighter:snapshot()
        if not view or not view.round_active or not view.opponent or not fighter.opponent then return end
        local away = view.self.position.x <= view.opponent.position.x and -1 or 1
        -- Requests from other behaviors add to these at the simulation boundary.
        -- These two calls are independent, not a transactional pair.
        local enemy_ok, enemy_error = fighter.opponent:move_by(-away * 100, 0)
        local self_ok, self_error = fighter:move_by(away * 40, 0)
        if enemy_ok or self_ok then
            self.state.cooldown = 180
            if hud and sf2.ui.is_open(hud) then
                sf2.ui.set_text(hud, "status", "Repulse queued (3 seconds)")
                sf2.ui.set_enabled(hud, "cast", false)
            end
        end
        if not enemy_ok and enemy_error then sf2.log.warn(enemy_error) end
        if not self_ok and self_error then sf2.log.warn(self_error) end
    end,
    on_round_end = close_hud,
    on_fight_end = close_hud,
}
local rule = sf2.rules.behavior { id = "repulse", behavior = pulse, target = sf2.rules.PLAYER }
for _, fight in ipairs {
    "core:fights/zone_1/tournament/3",
    "core:fights/zone_1/tournament_eclipsemode/3",
} do
    sf2.fights.patch { target = fight, append_rules = { rule } }
end
