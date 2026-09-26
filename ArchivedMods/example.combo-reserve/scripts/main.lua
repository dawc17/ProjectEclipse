local sf2 = require("sf2")
local reserve = sf2.behaviors.register {
    id = "reserve",
    parameters = {
        frames = { type = sf2.behaviors.INTEGER, default = 300 },
        per_hit = { type = sf2.behaviors.NUMBER, default = 0.01 },
        cap = { type = sf2.behaviors.INTEGER, default = 15 },
    },
    state = { lifetime = "round", fields = {
        stacks = { type = sf2.behaviors.INTEGER, default = 0 },
        expires = { type = sf2.behaviors.INTEGER, default = 0 },
    } },
    on_combo_changed = function(self, fighter, event)
        local combat = fighter:snapshot()
        if not combat or combat.frame < self.state.expires then return end
        if event.combo == 0 and event.last_combo >= 3 then
            self.state.stacks = math.min(event.last_combo, self.params.cap)
            self.state.expires = combat.frame + self.params.frames
        end
    end,
    on_tick = function(self, fighter, event)
        if self.state.stacks > 0 and event.frame >= self.state.expires then
            self.state.stacks = 0
            self.state.expires = 0
        end
    end,
    on_damage_dealing = function(self, fighter)
        local combat = fighter:snapshot()
        if combat and combat.frame < self.state.expires then
            fighter:scale_outgoing_damage(1 + self.state.stacks * self.params.per_hit)
        end
    end,
    on_style_changed = function(_, _, event)
        sf2.log.info("Style changed to " .. event.style_name)
    end,
}
local rule = sf2.rules.behavior { id = "reserve", behavior = reserve, target = sf2.rules.PLAYER }
sf2.fights.patch { target = "core:fights/zone_1/tournament/2", append_rules = { rule } }
