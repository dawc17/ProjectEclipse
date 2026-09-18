local sf2 = require("sf2")

-- Authority: Assets/DExml/perks.xml, PERK_MASTER_OF_STYLE / PERK_RELENTLESS;
-- rank payloads: Assets/DExml/CharacterProgress.xml, Progress/Perks.
-- The archive is research input only. This module executes no XML.
local function text(id, value)
    return sf2.localization.register { id = id, language = "eng", value = value }
end

local master_icon = sf2.assets.sprite("core:ui/skills/IconMasterOfStyle")
local master_buff = sf2.assets.sprite("core:ui/skills/IconMasterOfStyle_Blue")
local relentless_icon = sf2.assets.sprite("core:ui/skills/IconCrackedApple")
local relentless_buff = sf2.assets.sprite("core:ui/skills/IconCrackedApple_Blue")

local function frame(fighter)
    local snapshot = fighter:snapshot()
    assert(snapshot, "DE combat perks require a combat clock snapshot.")
    return snapshot.frame
end

local function clear_master(self, fighter)
    self.state.expires = 0
    fighter:clear_status_icon("MasterReward")
end

local master_behavior = sf2.behaviors.register {
    id = "master_of_style",
    parameters = {
        active_frames = { type = sf2.behaviors.INTEGER, default = 300 },
        drain = { type = sf2.behaviors.NUMBER, default = 0.1 },
    },
    state = { lifetime = "round", fields = {
        expires = { type = sf2.behaviors.INTEGER, default = 0 },
    } },
    on_style_changed = function(self, fighter, event)
        -- XML ORs Min=Brutal, Min=Aggressive, Min=Crazy. Brutal is native rank 2.
        -- The archived English text instead says Aggressive; do not change the trigger.
        if event.style_rank >= 2 then
            self.state.expires = frame(fighter) + self.params.active_frames
            fighter:show_status_icon("MasterReward", master_buff, self.params.active_frames)
        end
    end,
    on_hit_post_crit = function(self, fighter, event)
        if self.state.expires <= frame(fighter) then return end
        if (event.target == "self" and not event.blocked) or
            (event.target == "opponent" and (event.ranged or event.magic)) then
            clear_master(self, fighter)
        end
    end,
    on_post_hit = function(self, fighter, event)
        if self.state.expires > frame(fighter) and event.target == "opponent" and
            not event.blocked and (event.weapon or event.unarmed) then
            -- Native Root[Hit.Damage + Drain] is addition in normalized health units.
            fighter:add_outgoing_damage(self.params.drain)
            clear_master(self, fighter)
        end
    end,
    on_tick = function(self, fighter, event)
        if self.state.expires > 0 and event.frame >= self.state.expires then clear_master(self, fighter) end
    end,
    on_round_end = clear_master,
}

local function clear_relentless(self, fighter)
    self.state.expires = 0
    fighter:clear_status_icon("BuffRelentless")
end

local relentless_behavior = sf2.behaviors.register {
    id = "relentless",
    parameters = {
        active_time = { type = sf2.behaviors.INTEGER, default = 300 },
        combo_min_count = { type = sf2.behaviors.INTEGER, default = 3 },
        combo_max_count = { type = sf2.behaviors.INTEGER, default = 15 },
        damage_per_stack = { type = sf2.behaviors.NUMBER, default = 0.01 },
    },
    state = { lifetime = "round", fields = {
        combo_count = { type = sf2.behaviors.INTEGER, default = 0 },
        expires = { type = sf2.behaviors.INTEGER, default = 0 },
    } },
    on_round_begin = function(self, fighter)
        self.state.combo_count = 0
        clear_relentless(self, fighter)
    end,
    on_combo_changed = function(self, fighter, event)
        local now = frame(fighter)
        if now >= self.state.expires and event.combo >= self.params.combo_min_count then
            if event.combo == self.params.combo_min_count then
                self.state.combo_count = self.params.combo_min_count
            else
                self.state.combo_count = math.min(self.state.combo_count + 1, self.params.combo_max_count)
            end
        end
        if event.combo == 0 and self.state.combo_count >= self.params.combo_min_count then
            self.state.expires = now + self.params.active_time
            fighter:show_status_icon("BuffRelentless", relentless_buff, self.params.active_time, self.state.combo_count)
        end
    end,
    on_post_hit = function(self, fighter, event)
        if event.target == "opponent" and self.state.expires > frame(fighter) then
            -- XML has no Block/Animation filter: the next outgoing hit consumes it.
            fighter:scale_outgoing_damage(1 + self.params.damage_per_stack * self.state.combo_count)
            clear_relentless(self, fighter)
        end
    end,
    on_tick = function(self, fighter, event)
        if self.state.expires > 0 and event.frame >= self.state.expires then clear_relentless(self, fighter) end
    end,
    on_round_end = clear_relentless,
}

local master_upgrades, relentless_upgrades = {}, {}
for rank = 1, 5 do
    -- Original English descriptions, with native %% escapes resolved for Lua text.
    master_upgrades[rank] = {
        level = rank,
        description = text("perk.master_of_style.rank_" .. rank,
            "Upon reaching Aggressive or higher style, your next melee attack within 5 seconds will reduce enemy health by " ..
            (rank * 2) .. "% of their maximum health."),
        parameters = { drain = rank * 0.02 },
    }
    relentless_upgrades[rank] = {
        level = rank,
        description = text("perk.relentless.rank_" .. rank,
            "After landing a combo of 3 or more hits, your next attack within 5 seconds deals +" .. rank ..
            "% damage per combo hit, up to 15 stacks."),
        parameters = { damage_per_stack = rank * 0.01 },
    }
end

local master = sf2.perks.register {
    id = "master_of_style", behavior = master_behavior, kind = sf2.perks.SINGLE,
    display_name = text("perk.master_of_style.name", "Master of Style"),
    description = master_upgrades[5].description, icon = master_icon,
    upgrades = master_upgrades, initial_upgrade = 1,
}
local relentless = sf2.perks.register {
    id = "relentless", behavior = relentless_behavior, kind = sf2.perks.SINGLE,
    display_name = text("perk.relentless.name", "Relentless"),
    description = relentless_upgrades[1].description, icon = relentless_icon,
    upgrades = relentless_upgrades, initial_upgrade = 1,
}

return { master_of_style = master, relentless = relentless }
