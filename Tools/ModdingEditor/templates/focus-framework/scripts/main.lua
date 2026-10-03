local sf2 = require("sf2")

-- A saved resource owned by this framework, independent of base currency.
sf2.state.register {
    version = 1,
    fields = { focus = { type = "integer", default = 0 } },
}

sf2.extensions.register {
    id = "charge_hit", version = 1,
    request = { damage = "number", blocked = "boolean" },
    response = { bonus = "number", focus = "integer" },
    handler = function(hit, caller)
        assert(hit.damage >= 0, "Damage must be nonnegative")
        local focus = sf2.state.get("focus")
        local bonus = 0
        if hit.damage > 0 and not hit.blocked then
            focus = focus + 1
            if focus >= 3 then
                focus = 0
                bonus = math.min(1, hit.damage * 0.5)
            end
            sf2.state.set { focus = focus }
        end
        -- caller is supplied by Eclipse, never by the request table.
        return { bonus = bonus, focus = focus }
    end,
}

sf2.extensions.register {
    id = "status", version = 1,
    response = { focus = "integer" },
    handler = function() return { focus = sf2.state.get("focus") } end,
}
