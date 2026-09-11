local sf2 = require("sf2")
local guard = sf2.behaviors.register {
    id = "guard",
    parameters = { scale = { type = sf2.behaviors.NUMBER, default = 0.9 } },
    on_damage_resolving = function(parameters, fighter)
        fighter:scale_incoming_damage(parameters.scale)
    end,
}
local perk = sf2.perks.register {
    id = "guard", behavior = guard, kind = sf2.perks.SINGLE,
    display_name = sf2.localization.key("name"),
    description = sf2.localization.key("base"),
    icon = sf2.assets.sprite("sprites/guard"),
    upgrades = {
        { level = 1, description = sf2.localization.key("first"), parameters = { scale = 0.8 } },
        { level = 2, description = sf2.localization.key("second"), parameters = { scale = 0.7 } },
        { level = 3, description = sf2.localization.key("third"), parameters = { scale = 0.6 } },
    },
}
sf2.progression.replace_perk_branch { level = 2, entries = { { action = "unlock", perk = perk } } }
for level = 3, 5 do
    sf2.progression.replace_perk_branch { level = level, entries = { { action = "upgrade", perk = perk } } }
end
