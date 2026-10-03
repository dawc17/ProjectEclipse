local sf2 = require("sf2")
local hud
local function close_hud()
    if hud and sf2.ui.is_open(hud) then sf2.ui.close(hud) end
    hud = nil
end
local goal = sf2.behaviors.register {
    id = "three_hits",
    state = {
        lifetime = "round",
        fields = {
            hits = { type = "integer", default = 0 },
            frames = { type = "integer", default = 0 },
            requested = { type = "boolean", default = false },
        },
    },
    on_round_begin = function()
        close_hud()
        hud = sf2.ui.open {
            id = "objective", mount = "hud",
            placement = { anchor = "top_left", x = 24, y = 144 },
            root = { id = "goal", kind = "text", width = 460, height = 48,
                text = "Land 3 hits in 10 seconds: 0/3" },
        }
    end,
    on_damage_dealt = function(self, _, hit)
        if hit.damage > 0 and not hit.blocked then
            self.state.hits = math.min(3, self.state.hits + 1)
            if hud and sf2.ui.is_open(hud) then
                sf2.ui.set_text(hud, "goal", "Land 3 hits in 10 seconds: " .. self.state.hits .. "/3")
            end
        end
    end,
    on_tick = function(self, fighter, tick)
        self.state.frames = self.state.frames + tick.delta_frames
        if self.state.requested then return end
        local outcome
        if self.state.hits >= 3 then outcome = "win"
        elseif self.state.frames >= 600 then outcome = "loss" end
        if outcome then
            -- Acceptance queues the result; a native KO/timeout still takes precedence.
            local accepted, reason = fighter:end_round(outcome)
            self.state.requested = accepted
            if not accepted then sf2.log.warn(reason) end
        end
    end,
    on_round_end = close_hud,
    on_fight_end = close_hud,
}
local rule = sf2.rules.behavior {
    id = "objective", behavior = goal, target = sf2.rules.PLAYER,
    controls_outcome = true,
}
for _, fight in ipairs {
    "core:fights/zone_1/tournament/3",
    "core:fights/zone_1/tournament_eclipsemode/3",
} do
    sf2.fights.patch { target = fight, append_rules = { rule } }
end
