local sf2 = require("sf2")
local function text(key) return sf2.mod.id .. ":localization/" .. key end

-- This behavior belongs to the battle. No equipment or perk is required.
local cycle = sf2.behaviors.register {
    id = "third_strike",
    parameters = { every = { type = sf2.behaviors.INTEGER, required = true } },
    state = {
        lifetime = "round",
        fields = { hits = { type = sf2.behaviors.INTEGER, required = true, default = 0 } },
    },
    on_damage_resolving = function(self, fighter, event)
        if event.damage <= 0 then return end
        -- Below one third health, the guardian loses its guard entirely.
        local combat = fighter:snapshot()
        if combat and combat.self.max_health > 0
            and combat.self.health / combat.self.max_health <= 1 / 3 then return end
        self.state.hits = self.state.hits + 1
        if self.state.hits % self.params.every ~= 0 then
            fighter:scale_incoming_damage(0)
        end
    end,
}
local rule = sf2.rules.behavior {
    id = "guardian_guard", behavior = cycle, parameters = { every = 3 },
    target = sf2.rules.OPPONENT,
}
local guardian = sf2.warriors.register {
    id = "guardian", template = sf2.warriors.get_template("core:warrior-templates/default"),
    tactic = "Standard", first_name = text("opponent"), last_name = "", level = 1,
}
local zone = sf2.zones.register { id = "trial", file = "Map1.1", start = false }
local battle = sf2.battles.register {
    id = "trial", zone = zone, type = sf2.battles.STORY, x = 0, y = 0,
    alias = text("trial"), title = text("trial"), description = text("description"), location = "dojo",
}
local fight = sf2.fights.register {
    id = "trial", battle = battle, warriors = { guardian }, rules = { rule },
    rounds = 3, round_time = 99, location = "dojo",
}
sf2.modes.register { id = "trial", fights = { fight }, repeatable = true }
-- Use the existing map-entry compatibility adapter; the combat mechanic is Lua.
sf2.quests.register {
    id = "entry", place = "map", events = { "session" },
    actions = { { type = "show_battle", battle = battle, locked = false } },
}
