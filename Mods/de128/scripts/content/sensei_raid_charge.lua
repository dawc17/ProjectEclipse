local sf2 = require("sf2")

-- Pending integration: the caller must supply the recovered perk-state reader.
-- Charge inventory and the forced Sensei loadout are not substitutes for it.
local function register(is_available)
    assert(type(is_available) == "function", "Sensei RaidCharge requires a perk-state reader")
    local behavior = sf2.behaviors.register {
        id = "sensei_raid_charge",
        on_round_begin = function(_, fighter)
            local available = is_available()
            assert(type(available) == "boolean", "Sensei RaidCharge availability must be boolean")
            fighter:set_control_blocked("raid_charge", not available)
        end,
    }
    return sf2.rules.behavior {
        id = "sensei_raid_charge_conditional", behavior = behavior, target = sf2.rules.PLAYER,
    }
end

return { register = register }
