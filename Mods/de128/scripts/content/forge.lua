local sf2 = require("sf2")

-- Historical forge.xml splits the twelve existing Complex candidates into
-- three equal pools. The two new pools use the unchanged core Complex prices.
-- Abilities recipes need DE-only perks and a separate price profile; neither is
-- silently substituted here.
local equipment = {
    sf2.forge.WEAPON, sf2.forge.ARMOR, sf2.forge.HELM,
    sf2.forge.RANGED, sf2.forge.MAGIC,
}
local complex = sf2.forge.profile("Complex")
local simple = sf2.forge.profile("Simple")
local pools = {
    { id = "complex_2", text = "forge.complex_2", perks = {
        "PERK_SKANDA_SET_KARMA", "PERK_GUST_SET_WIND_MAKER",
        "PERK_DIRECTOR_SET_PLOT_TWIST", "PERK_TIME_SHIFT_SET_TIME_SHIFTER",
    } },
    { id = "complex_3", text = "forge.complex_3", perks = {
        "PERK_ARCANE_MARTIAL_ART_SET_NEO_WANDERER", "PERK_CORDYCEPS_FUNGUS_SET",
        "PERK_MAGMA_VOLCANO_SET", "PERK_KARCER_HUNGER_SET",
    } },
}

for _, pool in ipairs(pools) do
    local items, candidates = {}, {}
    for _, category in ipairs(equipment) do
        items[#items + 1] = { equipment = category, enchantments = 1, bar_scale = "Enchantment" }
        for _, name in ipairs(pool.perks) do
            local perk = sf2.perks.get("core:perks/" .. name)
            sf2.forge.exclude_candidate { profile = complex, perk = perk, equipment = category }
            candidates[#candidates + 1] = { perk = perk, equipment = category }
        end
    end
    sf2.forge.register_recipe {
        id = pool.id, alias = sf2.mod.id .. ":localization/" .. pool.text,
        economic_profile = complex, items = items, candidates = candidates,
    }
end

-- Simple changes only its random-aspect deviation; base prices and base aspect
-- scale remain owned by Eclipse.
for _, category in ipairs(equipment) do
    sf2.forge.override_deviation {
        profile = simple, equipment = category, minimum = 15, maximum = 75,
    }
end
