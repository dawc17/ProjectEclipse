local sf2 = require("sf2")
local behavior=sf2.behaviors.register {
    id="peer",
    on_tick=function(_,fighter,tick)
        if tick.frame==60 then
            assert(fighter:move_by(7,0))
            assert(fighter.opponent:move_by(-5,0))
        end
    end,
}
local rule=sf2.rules.behavior{id="peer",behavior=behavior,target=sf2.rules.PLAYER}
sf2.fights.patch{target="core:fights/zone_1/tournament/3",append_rules={rule}}
