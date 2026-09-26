local sf2 = require("sf2")

-- One reusable Lua behavior can power both a learned perk and an item enchantment
-- with different typed values. The fighter object is a sanitized capability table, not a
-- recovered C# Model.
local battle_charge = sf2.behaviors.register {
    id = "battle_charge",
    parameters = {
        magic_charge = sf2.behaviors.NUMBER,
        health_bonus = {
            type = sf2.behaviors.NUMBER,
            required = false,
            default = 0,
        },
    },
    on_fight_begin = function(parameters, fighter)
        fighter:add_magic_charge(parameters.magic_charge)
        if parameters.health_bonus ~= 0 then
            fighter:change_health(parameters.health_bonus)
        end

        local source_id = fighter.enchantment_id or fighter.perk_id or "unknown"
        sf2.log.info("battle_charge activated from " .. source_id)
    end,
}

sf2.perks.register {
    id = "battle_focus",
    behavior = battle_charge,
    kind = sf2.perks.SINGLE,
    display_name = sf2.localization.key("perk.battle_focus"),
    description = sf2.localization.key("perk.battle_focus.description"),
    icon = sf2.assets.sprite("core:UI/Skills/SkillsEnch02.EnchantmentFrenzy"),
    parameters = {
        magic_charge = 0.15,
        health_bonus = 0.02,
    },
}

sf2.enchantments.register {
    id = "battle_charge_weapon",
    behavior = battle_charge,
    display_name = sf2.localization.key("enchantment.battle_charge"),
    description = sf2.localization.key("enchantment.battle_charge.description"),
    icon = sf2.assets.sprite("core:UI/Skills/SkillsEnch02.EnchantmentFrenzy"),
    recipe = sf2.enchantments.MEDIUM,
    item_types = { sf2.enchantments.WEAPON },
    parameters = {
        magic_charge = 0.35,
        health_bonus = 0.05,
    },
}

sf2.log.info("registered the Battle Charge perk and weapon enchantment")
