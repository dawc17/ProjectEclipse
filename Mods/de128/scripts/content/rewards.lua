local sf2 = require("sf2")
local equipment = require("content.equipment")

local lifesteal = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_LIFESTEAL_WEAPON")
local desolator_reward = sf2.rewards.register {
    id = "titans_desolator",
    items = {
        {
            item = equipment.titans_desolator,
            configure = function(context)
                return {
                    level = context.player_level,
                    enchantments = {
                        { perk = lifesteal, aspect = 3639 / 100 * context.player_level + 60 },
                    },
                }
            end,
        },
    },
}

-- Slot 1 is the final fight's winning reward. Native ownership filtering prevents
-- another copy, and the scoped item-drop patch preserves the base economy.
sf2.fights.patch {
    target = "core:fights/zone_7/c3_boss_titan_eclipsemode/6",
    reward_drops = {
        { wins = 1, mode = "eclipse", reward = desolator_reward },
    },
}

return { titans_desolator = desolator_reward }
