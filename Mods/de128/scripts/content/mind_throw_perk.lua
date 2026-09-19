local sf2 = require("sf2")
-- The three archived MindThrowNormal triggers, implemented as Lua decisions.
local behavior = sf2.behaviors.register {
    id = "mind_throw",
    on_animation_start = function(_, fighter, event)
        if event.target == "self" and event.animation_name == "de128:moves/mind_throw_player" then
            if not fighter:has_flag("pending") then fighter:set_flag("pending") end
        elseif event.animation_name == "de128:moves/mind_throw_wall" then
            fighter:clear_flag("pending")
        elseif event.target ~= "self" and event.animation_name == "de128:moves/mind_throw_hit"
            and fighter:has_flag("pending") then
            fighter:clear_flag("pending")
        end
    end,
}
return sf2.perks.register {
    id = "mind_throw", behavior = behavior, kind = sf2.perks.SINGLE,
    display_name = sf2.localization.register { id = "perk.mind_throw.name", language = "eng", value = "Mind Throw" },
    description = sf2.localization.register { id = "perk.mind_throw.description", language = "eng", value = "Lifts an opponent and slams them into the ground." },
}
