local sf2 = require("sf2")
local power = sf2.behaviors.register {
    id = "third_hit",
    state = { lifetime = "round", fields = { hits = { type = sf2.behaviors.INTEGER, default = 0 } } },
    on_damage_dealing = function(self, fighter, event)
        if event.damage <= 0 or event.blocked then return end
        self.state.hits = self.state.hits + 1
        if self.state.hits % 3 == 0 then fighter:scale_outgoing_damage(2) end
    end,
}
local rule = sf2.rules.behavior { id = "power", behavior = power, target = sf2.rules.PLAYER }
sf2.fights.patch {
    target = "core:fights/zone_1/tournament/1",
    append_rules = { rule },
}
