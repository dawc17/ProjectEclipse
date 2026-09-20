-- Pending composition root, deliberately absent from main.lua. Every dependency
-- must be verified before activation; fixtures are not production substitutes.
local installed = false
local function install(opponents, is_raid_charge_available, portraits)
    assert(not installed, "Sensei story is already installed")
    assert(type(is_raid_charge_available) == "function", "Verified perk-state reader required")
    assert(type(portraits) == "table" and portraits.character_sensei, "Verified Sensei portrait required")
    for _, sequence in ipairs(require("content.sensei_entry_data")) do
        for _, card in ipairs(sequence.cards) do
            if card.portrait then assert(portraits[card.portrait], "Missing verified portrait: " .. card.portrait) end
        end
    end
    for _, sequence in ipairs(require("content.sensei_victory_data")) do
        for _, card in ipairs(sequence) do
            assert(portraits[card.portrait], "Missing verified portrait: " .. card.portrait)
        end
    end
    local charge = require("content.sensei_raid_charge").register(is_raid_charge_available)
    local graph = require("content.sensei_encounters").register(opponents, charge)
    require("content.sensei_entry").install(graph.normal, portraits)
    -- The same final IDs drive narrative and the next act's prerequisites.
    -- Subscribe victory first so its pending flag gates the unlock notification.
    require("content.sensei_victory").install(graph.final_ids, portraits)
    require("content.sensei_defeat").install(graph.normal_ids, portraits.character_sensei)
    require("content.sensei_notifications").install(graph.battles.normal, graph.prior_final_ids, portraits.character_sensei)
    installed = true
    return graph
end
return { install = install }
