local sf2 = require("sf2")
local hud, before, enemy_before, casts
local behavior = sf2.behaviors.register {
    id = "probe",
    on_round_begin = function()
        before, enemy_before, casts = nil, nil, 0
        hud = sf2.ui.open { id="probe", mount="hud",
            root={id="result",kind="text",text="waiting",width=500,height=40} }
    end,
    on_tick = function(_, fighter, tick)
        if tick.frame == 60 then
            local snapshot = fighter:snapshot()
            before, enemy_before = snapshot.self.position.x, snapshot.opponent.position.x
            assert(fighter:move_by(20, 0))
            assert(fighter:move_by(30, 0))
            assert(fighter.opponent:move_by(-25, 0))
            assert(fighter:snapshot().self.position.x == before, "recursive translation")
            casts = casts + 1
        elseif tick.frame == 61 then
            local view = fighter:snapshot()
            local dx, enemy_dx = view.self.position.x-before, view.opponent.position.x-enemy_before
            -- Peer rule adds +7 / -5. Native simulation runs between observations.
            assert(math.abs(dx-57)<1, "self additive boundary delta: "..dx)
            assert(math.abs(enemy_dx+30)<1, "opponent additive boundary delta: "..enemy_dx)
            assert(casts==1, "repeated cast")
            sf2.ui.set_text(hud,"result","composed: "..dx..", "..enemy_dx)
            sf2.ui.set_visible(hud,"result",false)
            sf2.log.info("[MotionProbe] composed delta self="..dx.." opponent="..enemy_dx)
        end
    end,
    on_round_end = function() if hud then sf2.ui.close(hud) end end,
    on_fight_end = function() if hud then sf2.ui.close(hud) end end,
}
local rule=sf2.rules.behavior{id="probe",behavior=behavior,target=sf2.rules.PLAYER}
sf2.fights.patch{target="core:fights/zone_1/tournament/3",append_rules={rule}}
