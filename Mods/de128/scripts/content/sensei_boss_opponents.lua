local sf2 = require("sf2")
local art = require("content.sensei_art")
-- Pending: not required by main.lua. Historical DE stages.xml boss loadouts.
-- Guards and encounter assembly remain unresolved; never replace missing templates.
-- Reconcile with SOURCE_CORPUS.md before current-corpus parity claims.
local result = { require("content.sensei_act_one_opponents") }

do
    local function register(id, alignments)
        return sf2.warriors.register {
            id = id,
            template = sf2.warriors.get_template("core:warrior-templates/hermit_swords"),
            tactic = "Lynx_Ranged", first_name = "BOSS_HERMIT", avatar = art.avatar("boss_hermit_young"),
            attributes = { WeaponDamage = 10, UnarmedDamage = 0, BodyDefense = 8, HeadDefense = 2 },
            items = { sf2.items.get("core:items/ranged/RANGED_NEEDLE") },
            perks = {
                { perk = sf2.perks.get("core:perks/PERK_HERMITSTORM"), frames = 300 },
                { perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_MAGIC_PAIN_RECHARGE_ARMOR"), aspect = 100000, chance = 0.42 },
                { perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_MAGIC_PAIN_RECHARGE_HELM"), aspect = 100000, chance = 0.42 },
                { perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_ENFEEBLE_WEAPON"), aspect = 100000, chance = 0.26 },
                { perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_ENFEEBLE_RANGED"), aspect = 100000, chance = 0.26 },
            },
            attribute_alignments = alignments,
        }
    end
    result[2] = {
        normal = register("sensei_act_2_boss_normal", {
            { factor = 1, shift = 6, priority = 1 },
            { factor = 1, shift = 6, priority = 1 },
        }),
        eclipse = register("sensei_act_2_boss_eclipse", {
            { factor = 1, shift = 7, priority = 1 },
        }),
    }
end

do
    local function register(id, alignments)
        return sf2.warriors.register {
            id = id,
            template = sf2.warriors.get_template("core:warrior-templates/butcher_backswords"),
            tactic = "Standard", first_name = "BOSS_BUTCHER", avatar = art.avatar("boss_butcher_young"),
            attributes = { WeaponDamage = 10, UnarmedDamage = 0, BodyDefense = 8, HeadDefense = 2 },
            items = { sf2.items.get("core:items/ranged/RANGED_CHAKRAM") },
            perks = {
                { perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_FRENZY_DEFENSE_ARMOR"), aspect = 100000, chance = 0.21 },
                { perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_FRENZY_DEFENSE_HELM"), aspect = 100000, chance = 0.21 },
                { perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_BLEEDING_WEAPON"), aspect = 100000, chance_factor = 2.06 },
                { perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_BLEEDING_RANGED"), aspect = 100000, chance_factor = 2.06 },
            },
            attribute_alignments = alignments,
        }
    end
    result[3] = {
        normal = register("sensei_act_3_boss_normal", {
            { factor = 1, shift = 6, priority = 1 },
            { factor = 1, shift = 6, priority = 1 },
        }),
        eclipse = register("sensei_act_3_boss_eclipse", {
            { factor = 1, shift = 7, priority = 1 },
        }),
    }
end

do
    local function register(id, alignments)
        return sf2.warriors.register {
            id = id,
            template = sf2.warriors.get_template("core:warrior-templates/wasp_naginata"),
            tactic = "Standard", first_name = "BOSS_WASP", avatar = art.avatar("boss_wasp_young"),
            attributes = { WeaponDamage = 10, UnarmedDamage = 0, BodyDefense = 8, HeadDefense = 2 },
            items = { sf2.items.get("core:items/ranged/RANGED_SHURIKEN_OF_DARKNESS") },
            perks = {
                { perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_DAMAGE_RETURN_ARMOR"), aspect = 100000, chance = 0.2 },
                { perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_DAMAGE_RETURN_HELM"), aspect = 100000, chance = 0.2 },
                { perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_PRECISION_WEAPON"), aspect = 100000, chance = 0.167 },
                { perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_PRECISION_RANGED"), aspect = 100000, chance = 0.167 },
            },
            attribute_alignments = alignments,
        }
    end
    result[4] = {
        normal = register("sensei_act_4_boss_normal", {
            { factor = 1, shift = 6, priority = 1 },
            { factor = 1, shift = 6, priority = 1 },
        }),
        eclipse = register("sensei_act_4_boss_eclipse", {
            { factor = 1, shift = 7, priority = 1 },
        }),
    }
end

do
    local function register(id, alignments)
        return sf2.warriors.register {
            id = id,
            template = sf2.warriors.get_template("core:warrior-templates/huntress_fan"),
            tactic = "Standard", first_name = "BOSS_HUNTRESS", avatar = art.avatar("boss_widow_young"),
            attributes = { WeaponDamage = 10, UnarmedDamage = 0, BodyDefense = 8, HeadDefense = 2 },
            items = { sf2.items.get("core:items/ranged/RANGED_ASSASSINS_DAGGER") },
            perks = {
                { perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_REGENERATION_ARMOR"), aspect = 100000, chance_factor = 1.375 },
                { perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_REGENERATION_HELM"), aspect = 100000, chance_factor = 1.375 },
                { perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_LIFESTEAL_WEAPON"), aspect = 100000, chance = 0.25 },
                { perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_LIFESTEAL_RANGED"), aspect = 100000, chance = 0.25 },
            },
            attribute_alignments = alignments,
        }
    end
    result[5] = {
        normal = register("sensei_act_5_boss_normal", {
            { factor = 1, shift = 6, priority = 1 },
            { factor = 1, shift = 6, priority = 1 },
        }),
        eclipse = register("sensei_act_5_boss_eclipse", {
            { factor = 1, shift = 7, priority = 1 },
        }),
    }
end

do
    local function register(id, alignments)
        return sf2.warriors.register {
            id = id,
            template = sf2.warriors.get_template("core:warrior-templates/shogun_katana"),
            tactic = "Standard", first_name = "NAME_SHOGUN", avatar = art.avatar("boss_shogun_young"),
            attributes = { WeaponDamage = 10, UnarmedDamage = 0, BodyDefense = 8, HeadDefense = 2 },
            items = { sf2.items.get("core:items/ranged/RANGED_CHAKRAM_OF_MASTER") },
            perks = {
                { perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_DAMAGE_ABSORPTION_HEAD_HELM"), aspect = 100000, chance = 0.33 },
                { perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_DAMAGE_ABSORPTION_BODY_ARMOR"), aspect = 100000, chance = 0.33 },
                { perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_OVERHEAT_WEAPON"), aspect = 100000, chance_factor = 4.16 },
                { perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_OVERHEAT_RANGED"), aspect = 100000, chance_factor = 4.16 },
            },
            attribute_alignments = alignments,
        }
    end
    result[6] = {
        normal = register("sensei_act_6_boss_normal", {
            { factor = 1, shift = 6, priority = 1 },
            { factor = 1, shift = 6, priority = 1 },
        }),
        eclipse = register("sensei_act_6_boss_eclipse", {
            { factor = 1, shift = 7, priority = 1 },
        }),
    }
end

return result
