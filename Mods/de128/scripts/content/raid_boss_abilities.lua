local sf2 = require("sf2")

-- Core's two hidden Root Potion items fall back to generic magic geometry.
-- Their archived attack edges live in these reviewed owner models.
sf2.items.set_presentation {
    item = sf2.items.get("core:items/magic/MAGIC_VERTICAL_TRIGGER"),
    model = sf2.assets.model("models/underworld/mdl_vertical_trigger"),
}
sf2.items.set_presentation {
    item = sf2.items.get("core:items/magic/SMALL_COLLISION_BOX"),
    model = sf2.assets.model("models/underworld/mdl_small_collision_box"),
}

-- Keep the original native move graphs; the reviewed boss casts use the
-- RaidCharge button, and Hunter's four wall-range variants have lower priority.
sf2.moves.patch {
    move = "HoaxenSpikeStrikePlayer",
    input = { expected = "Super", value = "RaidCharge" },
}
for _, range in ipairs({ "150", "200", "300", "370" }) do
    sf2.moves.patch {
        move = "HunterFly_" .. range,
        input = { expected = "Super", value = "RaidCharge" },
        priority = { expected = 1110, value = 200 },
    }
end
sf2.moves.patch {
    move = "AbilityRootPotionPlayer",
    input = { expected = "Super", value = "RaidCharge" },
}
-- The reviewed Arkhos and Tenebris casts name different packaged binaries.
-- Replace the loaded clip on the original move so its perk cooldown and
-- native projectile graph keep referring to the same caster identity.
sf2.moves.patch {
    move = "RatWavePlayer",
    input = { expected = "Up", value = "RaidCharge" },
    animation = { expected = "rats_wave.bytes",
        value = sf2.assets.binary("animations/magic_water_wave_player") },
    conditions = { { not_mod = "Stun" } },
}
sf2.moves.patch {
    move = "PerkFearRayPlayer",
    input = { expected = "Super", value = "RaidCharge" },
    animation = { expected = "boss_fear_ability.bytes",
        value = sf2.assets.binary("animations/chest_laser_ray_player") },
    remove_interval = { name = "Evade", type = "Invulnerable", start = 0, ["end"] = 47 },
    conditions = { { not_mod = "Stun" } },
}

local function tactic(id, moves, initial, cooldown, advance_for_range)
    return sf2.tactics.name(sf2.tactics.register {
        id = id, template = "Aggressive",
        on_decide = function(memory, event)
            local ready = event.frame >= initial and event.frame >= (memory.next_ability_frame or 0)
            local retreat, advance, attacks = nil, nil, {}
            for _, action in ipairs(event.actions) do
                if ready and moves[action.name] then
                    memory.next_ability_frame = event.frame + cooldown
                    return action
                end
                if action.name == "StepBack" then retreat = action end
                if action.name == "StepForward" then advance = action end
                if action.type == "attack" and not moves[action.name] then
                    attacks[#attacks + 1] = action
                end
            end
            if ready then
                if advance_for_range then
                    local back_wall = event.back_wall_distance
                    if (not back_wall or back_wall < 120) and advance then return advance end
                    if back_wall and back_wall > 300 and retreat then return retreat end
                elseif retreat then
                    -- Open the native minimum-range gate, but do not repeat
                    -- StepBack while the back wall leaves no room.
                    local back_wall = event.back_wall_distance
                    if not back_wall or back_wall > 120 then return retreat end
                end
            end
            if #attacks > 0 then
                memory.other_attack = (memory.other_attack or 0) % #attacks + 1
                return attacks[memory.other_attack]
            end
            if ready and advance_for_range then return nil end
            return retreat or "wait"
        end,
    })
end

local hunter_moves = {}
for _, range in ipairs({ "150", "200", "300", "370" }) do
    hunter_moves["HunterFly_" .. range] = true
end

return {
    hoaxen = tactic("hoaxen_tentacles", { HoaxenSpikeStrikePlayer = true }, 600, 600),
    hunter = tactic("hunter_fly", hunter_moves, 900, 900, true),
    hunter_power = tactic("hunter_fly_power", hunter_moves, 800, 800, true),
    berstuuk = tactic("berstuuk_root_potion", { AbilityRootPotionPlayer = true }, 300, 600),
    arkhos = tactic("arkhos_rat_wave", { RatWavePlayer = true }, 600, 600),
    tenebris = tactic("tenebris_fear_ray", { PerkFearRayPlayer = true }, 600, 600),
}
