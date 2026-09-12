local sf2 = require("sf2")
local counter = sf2.counters.register { id = "butcher_with_katana", maximum = 1 }
sf2.achievements.register {
    id = "blade_discipline", counter = counter, threshold = 1,
    title = sf2.localization.key("title"),
    description = sf2.localization.key("description"),
    icon = sf2.assets.sprite("core:UI/Achievements/ach_boss_butcher"),
}

-- Fight 6 is Butcher; fights 1..5 in the normal/Eclipse groups are bodyguards.
-- The intermission group has one gauntlet, ending with Butcher.
local encounters = {
    ["core:fights/zone_3/boss_butcher/6"] = true,
    ["core:fights/zone_3/boss_butcher_eclipsemode/6"] = true,
    ["core:fights/zone_3/boss_butcher_intermission/1"] = true,
}
sf2.story.on("battle_result", function(event)
    if event.outcome ~= "win" or not encounters[event.fight] or not event.equipment then return end
    if sf2.counters.get(counter) ~= 0 then return end
    for _, item in ipairs(event.equipment) do
        if item.type == "Weapon" and item.subtype == "Katana" then
            sf2.counters.add(counter, 1)
            sf2.log.info("Blade Discipline unlocked")
            return
        end
    end
end)
