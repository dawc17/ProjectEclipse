local sf2 = require("sf2")

-- Pending content: deliberately not required by main.lua. Archived Act I also
-- references Guard_Girl/Guard_Man templates absent from the available sources.
-- Source: Assets/DExml/stages.xml, ZONE_1/SENSEI_MEMORIES fight 3 and
-- SENSEI_MEMORIES_ECLIPSEMODE. Reconcile with SOURCE_CORPUS.md when available.
local function lynx(id, alignments)
    local function enchantment(name, chance)
        return { perk = sf2.perks.get("core:perks/" .. name), aspect = 100000, chance_factor = chance }
    end
    return sf2.warriors.register {
        id = id,
        template = sf2.warriors.get_template("core:warrior-templates/lynx_claws"),
        tactic = "Lynx_Ranged", first_name = "BOSS_LYNX", avatar = "boss_lynx_young",
        attributes = { WeaponDamage = 10, UnarmedDamage = 0, BodyDefense = 8, HeadDefense = 2 },
        items = { sf2.items.get("core:items/ranged/RANGED_SHURIKENS") },
        perks = {
            enchantment("PERK_ITEM_SPECIAL_TIME_BOMB_WEAPON", 2.69),
            enchantment("PERK_ITEM_SPECIAL_TIME_BOMB_RANGED", 2.69),
            enchantment("PERK_ITEM_SPECIAL_INTOXICATION_WEAPON", 2.75),
            enchantment("PERK_ITEM_SPECIAL_INTOXICATION_RANGED", 2.75),
        },
        attribute_alignments = alignments,
    }
end

return {
    normal = lynx("sensei_act_one_lynx", {
        -- The source repeats this row; preserve its order and multiplicity.
        { factor = 1, shift = 6, priority = 1 },
        { factor = 1, shift = 6, priority = 1 },
    }),
    eclipse = lynx("sensei_act_one_lynx_eclipse", { { factor = 1, shift = 7, priority = 1 } }),
}
