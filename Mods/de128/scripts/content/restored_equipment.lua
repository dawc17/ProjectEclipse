local sf2 = require("sf2")

-- Missing purchasable equipment whose base move families and art are available.
-- Preserve Samurai Armour's head-only initial stat snapshot from the archive.
-- Sphere3, ComboSphere3 and MindThrowNormal need their missing move graphs.
-- shared_moves.lua applies supported DE move deltas; remaining preview differences are pending.
local definitions = {
    { "medium_charge_of_darkness", "Medium Charge of Darkness", "magic", "RaidItems1.sphere2", "mdl_acid_cloud", "Sphere2", 37, 127, "GATES_OF_SHADOWS", "PERK_ITEM_SPECIAL_STUN_MAGIC", 1306 },
    { "minor_charge_of_darkness", "Minor Charge of Darkness", "magic", "RaidItems1.sphere1", "mdl_magic_fireball", "Sphere1", 23, 69, "ACT_4", "PERK_ITEM_SPECIAL_WEAKNESS_MAGIC", 798 },
    { "dragon_carapace", "Dragon Carapace", "armor", "Armor29.img_ARMOR_C2_Z5_DRAGON", "mdl_ARMOR_C2_Z5_DRAGON", nil, 13, 49, "ACT_3", "PERK_ITEM_SPECIAL_OVERHEAT_DEFENSE_ARMOR", 442 },
    { "old_legionnaire_armour", "Old Legionnaire Armour", "armor", "Armor31.img_armor_old_legioner", "mdl_armor_legioner", nil, 15, 53, "ACT_3", "PERK_ITEM_SPECIAL_DAMAGE_ABSORPTION_BODY_ARMOR", 512 },
    { "samurai_armour", "Samurai Armour", "armor", "UnknownItems.img_armor_unknown", "mdl_armor_big_shogun_old", nil, 37, 140, "GATES_OF_SHADOWS", "PERK_ITEM_SPECIAL_DAMAGE_ABSORPTION_BODY_ARMOR", 1306, { head_defense = 914 } },
    { "gabled_helm", "Gabled Helm", "helm", "UnknownItems.img_helm_unknown", "mdl_helm_gabled_old", nil, 11, 30, "ACT_2", "PERK_ITEM_SPECIAL_REJUVENATION_HELM", 366 },
    { "dragon_helm", "Dragon Helm", "helm", "Helm30.img_HELM_C2_Z5_DRAGON", "mdl_HELM_C2_Z5_DRAGON", nil, 13, 33, "ACT_3", "PERK_ITEM_SPECIAL_OVERHEAT_DEFENSE_HELM", 442 },
    { "dragon_boomerangs", "Dragon Boomerangs", "ranged", "Ranged10.img_RANGED_C2_Z5_DRAGON_BOOMERANG", "mdl_RANGED_C2_Z5_DRAGON_BOOMERANG", "Chakram", 13, 27, "ACT_3", "PERK_ITEM_SPECIAL_OVERHEAT_RANGED", 442 },
    { "dragons_breath", "Dragon’s Breath", "magic", "Magic9.img_MAGIC_C2_Z5_DRAGON_EARTHQUAKE", "mdl_magic_mass_bomb", "MassBomb", 13, 44, "ACT_3", "PERK_ITEM_SPECIAL_OVERHEAT_MAGIC", 442 },
    { "lightning_arc", "Lightning Arc", "magic", "Magic13.img_magic_lightning", "mdl_magic_fireball", "LightningArrow", 39, 139, "INTERMISSION", "PERK_ITEM_SPECIAL_ENFEEBLE_MAGIC", 1388 },
}
local categories = {
    armor = { sf2.items.register_armor, sf2.shop.ARMOR },
    helm = { sf2.items.register_helm, sf2.shop.HELMETS },
    ranged = { sf2.items.register_ranged, sf2.shop.RANGED },
    magic = { sf2.items.register_magic, sf2.shop.MAGIC },
}
local equipment = {}
for _, row in ipairs(definitions) do
    local category = categories[row[3]]
    local definition = {
        id = row[1],
        display_name = sf2.localization.register { id = "item." .. row[1], language = "eng", value = row[2] },
        icon = sf2.assets.sprite("core:UI/Items/" .. row[4]),
        model = sf2.assets.model("core:gamedata/models/" .. row[5]),
        initial_stats = row[12],
    }
    if row[6] then definition.subtype = row[6] end
    local item = category[1](definition)
    sf2.shop.addItem { section = category[2], item = item, level = row[7], price = sf2.price.gems(row[8]) }
    sf2.shop.set_availability { item = item, required_group = row[9], minimum_level = row[7] }
    sf2.items.set_default_enchantments {
        item = item, entries = { { perk = sf2.perks.get("core:perks/" .. row[10]), aspect = row[11] } },
    }
    equipment[row[1]] = item
end
return equipment
