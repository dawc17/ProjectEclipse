local sf2 = require("sf2")
local function alias(key) return "example.programmable-ai:localization/" .. key end
local arena = sf2.locations.register {
    id = "arena", color = "0x1b2230", wall = 200, floor = 80,
    width = 1936, height = 512, min_width = 1936,
    layers = {
        { type = 1, factor = 1, images = {
            { sprite = sf2.assets.sprite("core:Textures/Locations/battlefield/battlefield_bg1.back_1"),
              x = 0, y = 0, width = 1936, height = 1024 },
        } },
        { type = 2, factor = 1, fighters = { player_x = 868, player_y = -94, enemy_x = 1068, enemy_y = -94 } },
    },
}
local location = sf2.locations.name(arena)
-- Native templates retain the original portraits, rigs, clothing and weapons.
local function action(event, name)
    for _, candidate in ipairs(event.actions) do
        if candidate.name == name or ((name == "StepBack" or name == "StepForward") and candidate.name:find(name, 1, true)) then return candidate end
    end
end
local brains = {}
brains[1] = sf2.tactics.register {
    id = "patient", template = "Standard",
    on_decide = function(memory, event)
        if event.seconds < (memory.ready or 0) then return "wait" end
        local kick = action(event, "HighKick")
        if kick then memory.ready = event.seconds + 1.5; return kick end
        return nil
    end,
}
brains[2] = sf2.tactics.register {
    id = "footwork", template = "Standard",
    on_decide = function(memory, event)
        if event.seconds < (memory.ready or 0) then return "wait" end
        if not event.opponent then return nil end
        local distance = math.abs(event.self.position.x - event.opponent.position.x)
        local step = action(event, distance < 140 and "StepBack" or "StepForward")
        if step then memory.ready = event.seconds + 0.4; return step end
        return nil
    end,
}
brains[3] = sf2.tactics.register {
    id = "alternating", template = "Standard",
    on_decide = function(memory, event)
        if event.seconds < (memory.ready or 0) then return "wait" end
        local strike = action(event, memory.low and "LowKick" or "HighKick")
        if strike then
            memory.low = not memory.low
            memory.ready = event.seconds + 0.6
            return strike
        end
        return nil
    end,
}
local fighter_templates = { "man_kunai", "man_batons", "man_night" }
local fighters = {}
for i, template in ipairs(fighter_templates) do
    fighters[i] = sf2.warriors.register {
        id = i == 1 and "fighter" or "fighter_" .. i,
        template = sf2.warriors.get_template("core:warrior-templates/" .. template),
        tactic = sf2.tactics.name(brains[i]), first_name = alias("fighter." .. i), last_name = "", level = 1,
    }
end
local loss = sf2.rewards.register { id = "loss", items = {} }
local win = sf2.rewards.register { id = "win", items = {} }
local zone = sf2.zones.register { id = "trial", file = "Map1.1", start = false }
local battle = sf2.battles.register {
    id = "trial", zone = zone, type = sf2.battles.STORY, x = 0, y = 0,
    alias = alias("trial"), title = alias("trial"), description = alias("trial.description"), location = location,
}
local fights = {}
for i = 1, 3 do
    fights[i] = sf2.fights.register {
        id = "encounter_" .. i, battle = battle, rounds = 1, round_time = 99,
        location = location, warriors = { fighters[i] }, rewards = { loss, win },
    }
end
sf2.modes.register { id = "trial", fights = fights, repeatable = true, reset_on_loss = false }

-- Registered battles need a map-session reveal before their zone is selectable.
sf2.quests.register {
    id = "entry", place = "map", events = { "session" },
    actions = { { type = "show_battle", battle = battle, locked = false } },
}
