local sf2 = require("sf2")
-- Availability reader for sensei_raid_charge, SYNTHESIZED from archive evidence.
-- quests.xml ShowRaidChargeButton/HideRaidChargeButton set the user variable
-- _RaidChargeButton on ActivatePerk/DeactivatePerk for thirteen ability perks;
-- the recovered C# has no producer for those events. perks.xml marks the perks
-- PerkType="Combo" with Button="RaidCharge". Twelve belong to single-item
-- "SpecialRecipe" item sets, i.e. they arrive as an item enchantment (forge.xml's
-- Abilities recipe grants four of them; "Charm ... is now on"). The thirteenth
-- is the NEO_WANDERER five-piece set bonus (list.xml ItemSet). The
-- variable is profile state, so this reads the profile's equipment, not a
-- fight's forced loadout. Set-chest purchases that also reset it are not
-- modelled: equipment is re-read every round instead of latched.
local ability_perks = {
    "PERK_SHADOW_CLOAK", "PERK_HERMITSTORM", "PERK_EARTHQUAKE", "PERK_WASPFLY",
    "PERK_TELEPORTATION", "PERK_ASSISTANTS", "PERK_RAT_WAVE", "PERK_WAR_WHIRL",
    "PERK_FEAR_RAY", "PERK_LIGHTING_CHAIN", "PERK_POWER_FIELD", "PERK_GRASP_OF_DARKNESS",
}
local neo_wanderer = {
    "weapon/WEAPON_C1_Z4_NEO_WANDERER", "armor/ARMOR_C1_Z4_NEO_WANDERER", "helm/HELM_C1_Z4_NEO_WANDERER",
    "ranged/RANGED_C1_Z4_NEO_WANDERER", "magic/MAGIC_C1_Z4_NEO_WANDERER",
}

-- Resolve every identity at registration so a missing core definition fails loudly.
-- Qualified definition IDs are normalized to lower case, so compare lowered IDs.
local function create()
    local abilities, set_items = {}, {}
    for _, name in ipairs(ability_perks) do
        -- PERK_SHADOW_CLOAK is DE-only and absent from core; no Eclipse item can
        -- carry it yet, so it is matched by ID without a registration lookup.
        if name ~= "PERK_SHADOW_CLOAK" then sf2.perks.get("core:perks/" .. name) end
        abilities[("core:perks/" .. name):lower()] = true
    end
    sf2.perks.get("core:perks/PERK_ARCANE_MARTIAL_ART_SET_NEO_WANDERER")
    for _, path in ipairs(neo_wanderer) do
        sf2.items.get("core:items/" .. path)
        set_items[#set_items + 1] = ("core:items/" .. path):lower()
    end
    return function()
        local worn = {}
        for _, record in ipairs(sf2.profile.equipment()) do
            if record.owned then
                if record.item then worn[record.item:lower()] = true end
                for _, perk in ipairs(record.enchantments) do
                    if abilities[perk:lower()] then return true end
                end
            end
        end
        for _, item in ipairs(set_items) do
            if not worn[item] then return false end
        end
        return true
    end
end

return { create = create }
