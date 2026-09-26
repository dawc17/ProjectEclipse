local sf2 = require("sf2")

local guard = sf2.behaviors.register {
    id = "last_stand",
    on_damage_resolving = function(_, fighter)
        local combat = fighter:snapshot()
        if combat and combat.self.max_health > 0
            and combat.self.health / combat.self.max_health <= 0.5 then
            fighter:scale_incoming_damage(0.5)
        end
    end,
}
local rule = sf2.rules.behavior {
    id = "last_stand", behavior = guard, target = sf2.rules.OPPONENT,
}

-- First Lynx bodyguard. Preserve the encounter identity and native rules.
sf2.fights.patch {
    target = "core:fights/zone_1/boss_lynx/1",
    append_rules = { rule },
    location = "dojo",
    music = "fight1_samurai_spirit",
}
