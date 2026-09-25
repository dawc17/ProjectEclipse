local sf2 = require("sf2")
local equipment = require("content.equipment")
local titan = require("content.titan_reward_equipment")

local function at_player_level(perk_name)
    local perk = sf2.perks.get("core:perks/" .. perk_name)
    return function(context)
        return {
            level = context.player_level,
            enchantments = {
                { perk = perk, aspect = 3639 / 100 * context.player_level + 60 },
            },
        }
    end
end

local desolator_reward = sf2.rewards.register {
    id = "titans_desolator",
    items = {
        {
            item = equipment.titans_desolator,
            configure = at_player_level("PERK_ITEM_SPECIAL_LIFESTEAL_WEAPON"),
        },
        {
            item = titan.form,
            configure = at_player_level("PERK_ITEM_SPECIAL_SHIELDING_ARMOR"),
        },
        {
            item = titan.helm,
            configure = at_player_level("PERK_ITEM_SPECIAL_DAMAGE_ABSORPTION_HEAD_HELM"),
        },
        {
            item = titan.harpoon,
            configure = at_player_level("PERK_ITEM_SPECIAL_PRECISION_RANGED"),
        },
        {
            item = titan.mind_throw,
            configure = at_player_level("PERK_ITEM_SPECIAL_FRENZY_MAGIC"),
        },
    },
}

-- Slot 1 is the final fight's winning reward. Native ownership filtering prevents
-- duplicate copies, and the scoped item-drop patch preserves the base economy.
sf2.fights.patch {
    target = "core:fights/zone_7/c3_boss_titan_eclipsemode/6",
    reward_drops = {
        { wins = 1, mode = "eclipse", reward = desolator_reward },
    },
}

return { titans_desolator = desolator_reward }
