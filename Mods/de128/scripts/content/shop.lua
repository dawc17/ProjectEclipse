local sf2 = require("sf2")

-- Priced core equipment that the DE archive makes available in the normal shop.
-- The generated table contains static data; no XML is read by the mod in game.
-- Core identity stays base-owned. Prices, art, starting profiles, legacy paid
-- markers and changed enchantments are reversible catalog patches.
for _, row in ipairs(require("content.shop_data")) do
    local item = sf2.items.get("core:items/" .. row[1] .. "/" .. row[2])
    sf2.shop.set_availability {
        item = item,
        visibility = sf2.shop.FORCE_VISIBLE,
        minimum_level = row[3],
        required_group = row[4],
    }
    sf2.items.set_initial_profile {
        item = item,
        level = row[3], upgrade_level = row[5],
        upgrade_template = row[6], initial_stats = row[7],
        legacy_paid_item = row[9],
        clear_local_upgrades = row[11],
    }
    if row[10] then
        sf2.shop.set_price {
            item = item,
            price = sf2.price.gems(row[10]),
            secondary_price = row[12] > 0 and sf2.price.coins(row[12]) or nil,
        }
    end
    if row[13] or row[14] then
        sf2.items.set_presentation {
            item = item,
            icon = row[13] and sf2.assets.sprite("core:UI/Items/" .. row[13]) or nil,
            model = row[14] and sf2.assets.model("core:gamedata/models/" .. row[14]) or nil,
        }
    end
    if row[8] then
        local entries = {}
        for _, enchantment in ipairs(row[8]) do
            entries[#entries + 1] = {
                perk = sf2.perks.get("core:perks/" .. enchantment[1]),
                aspect = enchantment[2],
            }
        end
        sf2.items.set_default_enchantments { item = item, entries = entries }
    end
end
