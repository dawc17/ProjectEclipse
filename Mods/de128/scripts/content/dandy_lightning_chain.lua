local sf2 = require("sf2")

-- Preserve the core caster identity: its CreatePlayer action and five linked
-- native chain phases already match the reviewed DE graph.
sf2.moves.patch {
    move = "LightingChainPlayer",
    input = { expected = "Super", value = "RaidCharge" },
    priority = { expected = 9000, value = 200 },
    interval_start = { name = "Uninterrupt", expected = 9, value = 0 },
}

-- Eclipse's spawned magic actor has its hidden HERMIT_STORM item but no parent
-- perk slot. Keep the five linked native phases exclusive to LightningChain.
local chain_perk = sf2.perks.get("core:perks/PERK_LIGHTING_CHAIN")
for _, name in ipairs({ "LightingChainStart", "LightingChain50", "LightingChain150",
    "LightingChain300", "LightingChain400" }) do
    sf2.moves.remove_perk_lock { move = name, perk = chain_perk }
    sf2.moves.patch { move = name, conditions = { { type = "actor_name", name = "LightningChain" } } }
end

local function register_tactic(id, cooldown)
    return sf2.tactics.register {
        id = id, template = "Aggressive",
        on_decide = function(memory, event)
            local ready = event.frame >= 300 and event.frame >= (memory.next_chain_frame or 0)
            local retreat, attacks = nil, {}
            for _, action in ipairs(event.actions) do
                if ready and action.name == "LightingChainPlayer" then
                    memory.next_chain_frame = event.frame + cooldown
                    return action
                end
                if action.name == "StepBack" then retreat = action end
                if action.type == "attack" and action.name ~= "LightingChainPlayer" then
                    attacks[#attacks + 1] = action
                end
            end
            if not ready then
                if #attacks > 0 then
                    memory.other_attack = (memory.other_attack or 0) % #attacks + 1
                    return attacks[memory.other_attack]
                end
                return retreat or "wait"
            end
            -- The native caster has a 450-unit AI range gate. If the player
            -- stays close, let Aggressive keep fighting instead of repeating
            -- StepBack against the arena wall forever.
            return nil
        end,
    }
end

return {
    tactic = sf2.tactics.name(register_tactic("dandy_lightning_chain", 600)),
    power_tactic = sf2.tactics.name(register_tactic("dandy_lightning_chain_power", 500)),
}
