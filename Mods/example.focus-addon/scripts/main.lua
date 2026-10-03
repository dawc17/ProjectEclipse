local sf2 = require("sf2")
local hit_service = sf2.extensions.get("example.focus-framework:extensions/charge_hit", 1)
local status_service = sf2.extensions.get("example.focus-framework:extensions/status", 1)
local hud

local focus = sf2.behaviors.register {
    id = "focus",
    on_round_begin = function()
        if hud and sf2.ui.is_open(hud) then sf2.ui.close(hud) end
        local status = sf2.extensions.call(status_service, {})
        hud = sf2.ui.open {
            id = "focus", mount = "hud",
            root = { id = "meter", kind = "text", width = 280, height = 48,
                text = "Focus: " .. status.focus .. "/3" },
        }
    end,
    on_damage_dealing = function(_, fighter, hit)
        local result = sf2.extensions.call(hit_service, { damage = hit.damage, blocked = hit.blocked })
        if result.bonus > 0 then fighter:add_outgoing_damage(result.bonus) end
        if hud and sf2.ui.is_open(hud) then
            sf2.ui.set_text(hud, "meter", "Focus: " .. result.focus .. "/3")
        end
    end,
    on_fight_end = function()
        if hud and sf2.ui.is_open(hud) then sf2.ui.close(hud) end
        hud = nil
    end,
}
local rule = sf2.rules.behavior { id = "focus", behavior = focus, target = sf2.rules.PLAYER }
for _, fight in ipairs {
    "core:fights/zone_1/tournament/3",
    "core:fights/zone_1/tournament_eclipsemode/3",
} do
    sf2.fights.patch { target = fight, append_rules = { rule } }
end
