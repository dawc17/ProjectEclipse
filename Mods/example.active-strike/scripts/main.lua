local sf2 = require("sf2")
-- The bundled native binary is the base game's high_punch.bytes. This owned
-- move has no selection events: only the explicit ability starts it.
local strike = sf2.moves.register {
    id = "active_strike", animation = "animations/high_punch",
    type = "ATTACK", priority = 110, mid_frames = 2, first_frame = 1,
    mirror_node = "NHeel_1", direction = "face_enemy",
    locks = { { item = "Weapon", subtype = "Fists" }, { item = "Skeleton", subtype = "Skeleton" } },
    align = { axes = { "X", "Z" }, pivot = { node = "NHeel_2" }, position = { pivot = "Me" } },
    intervals = {
        { name = "Uninterrupt", to = 9 }, { type = "Block", from = 10 },
        { name = "Throwable", from = 10 },
        { type = "Attack", from = 4, to = 5, attack = {
            edges = { "EForearm_2", "EHand_2", "EFingers_2" },
            damage = 0.11, damage_terms = { UnarmedDamage = -10 },
            impulse = { x = 245 }, hit = "High", id = 21,
        } },
    },
    timeline = { [3] = { sound = "snd_swish7" }, strike = { sound = "snd_hit1" } },
}
local hud, requested, pending
local function close()
    if hud and sf2.ui.is_open(hud) then sf2.ui.close(hud) end
    hud, requested, pending = nil, false, nil
end
local behavior = sf2.behaviors.register {
    id = "ability",
    state = { lifetime = "round", fields = { cooldown = { type = "integer", default = 0 } } },
    on_round_begin = function()
        close()
        hud = sf2.ui.open { id = "ability", mount = "hud",
            placement = { anchor = "top_right", x = -24, y = 216 },
            root = { id = "root", kind = "column", width = 300, height = 96,
                style = { background_color = "#fff4dbf0" }, children = {
                    { id = "status", kind = "text", text = "Active Strike ready", height = 36,
                        style = { text_color = "#2b2119", font_size = 18 } },
                    { id = "cast", kind = "button", text = "Active Strike", height = 48 },
                } },
            on_click = function(_, widget) if widget == "cast" then requested = true end end,
        }
    end,
    on_tick = function(self, fighter, tick)
        self.state.cooldown = math.max(0, self.state.cooldown - tick.delta_frames)
        if pending and pending.status ~= "queued" then
            if pending.status == "applied" then
                self.state.cooldown = 120
                sf2.ui.set_text(hud, "status", "Strike started (2 seconds)")
            else
                sf2.ui.set_text(hud, "status", "Strike unavailable")
                if pending.error then sf2.log.warn(pending.error) end
                sf2.ui.set_enabled(hud, "cast", true)
            end
            pending = nil
        elseif not pending and self.state.cooldown == 0 then
            sf2.ui.set_text(hud, "status", "Active Strike ready")
            sf2.ui.set_enabled(hud, "cast", true)
        end
        if not requested then return end
        requested = false
        if pending or self.state.cooldown > 0 then return end
        local view = fighter:snapshot()
        if not view or not view.round_active then return end
        -- Explicit playback does not run input selection conditions. This ability
        -- chooses to wait for an idle move; define your own cancellation policy.
        if view.self.animation and view.self.animation.type == "attack" then return end
        pending = fighter:play_move(strike)
        sf2.ui.set_text(hud, "status", "Strike queued")
        sf2.ui.set_enabled(hud, "cast", false)
    end,
    on_animation_start = function(_, _, event)
        if event.target == "self" and event.animation_name == sf2.mod.id .. ":moves/active_strike" then
            sf2.log.info("Active Strike animation started")
        end
    end,
    on_round_end = close, on_fight_end = close,
}
local rule = sf2.rules.behavior { id = "ability", behavior = behavior, target = sf2.rules.PLAYER }
for _, fight in ipairs { "core:fights/zone_1/tournament/3", "core:fights/zone_1/tournament_eclipsemode/3" } do
    sf2.fights.patch { target = fight, append_rules = { rule } }
end
