local sf2 = require("sf2")

-- Assets/DExml/list.xml removes ShopHide from these former battle-pass items.
-- Use the archived Level as a shop eligibility gate, without rewriting the
-- canonical item's stats, upgrade level, prices, enchantments or saved identity.
local collections = {
    { suffix = "S1_GUARDIAN", level = 15 },
    { suffix = "S2_SKANDA", level = 20 },
    { suffix = "S3_WIND_MAKER", level = 25 },
    { suffix = "S4_SCRIPTWRITER", level = 50 },
    { suffix = "S5_TIME_SHIFTER", level = 45 },
}
for _, collection in ipairs(collections) do
    for _, category in ipairs({ "weapon", "armor", "helm", "ranged", "magic" }) do
        local name = string.upper(category) .. "_BP_" .. collection.suffix
        sf2.shop.set_availability {
            item = sf2.items.get("core:items/" .. category .. "/" .. name),
            visibility = sf2.shop.FORCE_VISIBLE,
            minimum_level = collection.level,
        }
    end
end
