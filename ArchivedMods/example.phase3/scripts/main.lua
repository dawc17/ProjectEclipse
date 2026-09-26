local sf2 = require("sf2")
local function key(name) return sf2.localization.key(name) end

-- Explicit global replacement: every load through this core sprite identity
-- uses our sword marker. Removing this mod restores the original art on restart.
sf2.assets.replace {
    target = "core:UI/Skills/SkillsEnch02.EnchantmentFrenzy",
    replacement = "sprites/archivist",
}
local icon = sf2.assets.sprite("core:UI/Skills/SkillsEnch02.EnchantmentFrenzy")
local wins = sf2.counters.register { id = "archivist_wins", maximum = 1000000 }
for _, milestone in ipairs({ { id = "first", count = 1 }, { id = "third", count = 3 } }) do
    sf2.achievements.register {
        id = milestone.id, counter = wins, threshold = milestone.count,
        title = key(milestone.id), description = key(milestone.id .. ".description"), icon = icon,
    }
end
local archivist = sf2.behaviors.register {
    id = "archivist",
    on_fight_end = function(self, fighter, event)
        if fighter.side ~= "player" or not event.won then return end
        local total = sf2.counters.add(wins, 1)
        sf2.log.info("Archivist victories: " .. total .. "; saved counter: " .. sf2.counters.get(wins))
    end,
}
local perk = sf2.perks.register {
    id = "archivist", behavior = archivist, kind = sf2.perks.SINGLE,
    display_name = key("archivist"), description = key("archivist.description"), icon = icon,
}
sf2.forge.register_recipe {
    id = "archivist", alias = "example.phase3:localization/forge",
    economic_profile = sf2.forge.profile("Simple"),
    items = { { equipment = sf2.forge.WEAPON, enchantments = 1, bar_scale = "1",
        min_deviation = 0, max_deviation = 0, random_aspect = false } },
    candidates = { { perk = perk, equipment = sf2.forge.WEAPON, min_level = 1, max_level = 52 } },
}
sf2.log.info("Phase 3 ready: forge Archivist, equip it, win one and three fights, check Profile achievements.")

