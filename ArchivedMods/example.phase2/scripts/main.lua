local sf2 = require("sf2")

sf2.state.register {
    version = 1,
    fields = { activations = { type = sf2.state.INTEGER, required = true, default = 0 } },
}

local resolve = sf2.behaviors.register {
    id = "measured_resolve",
    parameters = {
        hits_required = sf2.behaviors.INTEGER,
        charge = sf2.behaviors.NUMBER,
    },
    state = {
        lifetime = "round",
        fields = { hits = { type = sf2.behaviors.INTEGER, required = true, default = 0 } },
    },
    on_fight_begin = function(self, fighter) self.state.hits = 0 end,
    on_round_begin = function(self, fighter) self.state.hits = 0 end,
    on_damage_received = function(self, fighter, event)
        if event.blocked or event.damage <= 0 or event.health_after <= 0 then return end
        local parameters = self.params
        local count = self.state.hits + 1
        if count < parameters.hits_required then
            self.state.hits = count
            return
        end
        fighter:add_magic_charge(parameters.charge)
        self.state.hits = 0
        sf2.state.set { activations = (sf2.state.get("activations") or 0) + 1 }
        sf2.log.info("Measured Resolve activated on " .. (fighter.item_id or "profile"))
    end,
}

local icon = sf2.assets.sprite("core:UI/Skills/SkillsEnch02.EnchantmentFrenzy")
for _, variant in ipairs({
    { id = "resolve", forge = "forge", hits = 3, charge = 0.20 },
    { id = "resolve_quick", forge = "forge_quick", hits = 2, charge = 0.10 },
}) do
    local perk = sf2.perks.register {
        id = variant.id,
        behavior = resolve,
        kind = sf2.perks.SINGLE,
        display_name = sf2.localization.key(variant.id),
        description = sf2.localization.key(variant.id .. ".description"),
        icon = icon,
        parameters = { hits_required = variant.hits, charge = variant.charge },
    }
    sf2.forge.register_recipe {
        id = variant.id,
        alias = "example.phase2:localization/" .. variant.forge,
        economic_profile = sf2.forge.profile("Simple"),
        items = { { equipment = sf2.forge.WEAPON, enchantments = 1, bar_scale = "1",
            min_deviation = 0, max_deviation = 0, random_aspect = false } },
        candidates = { { perk = perk, equipment = sf2.forge.WEAPON, min_level = 1, max_level = 52 } },
    }
end

require("showcase")
