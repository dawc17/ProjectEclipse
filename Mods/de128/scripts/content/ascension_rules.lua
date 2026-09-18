-- Ascension prototype disabled by the owner on 2026-09-18.
--[==[
local sf2 = require("sf2")

local function damage(id, target, amount)
    return sf2.rules.attributes {
        id = "ascension_" .. id, target = target, values = { DamageFactor = amount },
    }
end

local base = {
    damage("baseline", sf2.rules.PLAYER, -5000),
    sf2.rules.no_button { id = "ascension_no_raid_charge", name = "RaidCharge", target = sf2.rules.PLAYER },
}

-- These are the six challenge families in the archived Ascension battle.
-- Native rules own their timer, collision, animation and healing behavior.
local challenges = {
    {
        name = "No weapons", text = "Fight unarmed. Neither fighter may use a weapon.",
        rules = {
            sf2.rules.equip_item { id = "ascension_unarmed", item = sf2.items.get("core:items/weapon/Fists") },
            damage("unarmed_player", sf2.rules.PLAYER, 7000),
            damage("unarmed_opponent", sf2.rules.OPPONENT, -7000),
        },
    },
    {
        name = "Hot ground", text = "Jump regularly. The ground becomes lethal after seven seconds.",
        rules = {
            sf2.rules.hot_ground {
                id = "ascension_hot_ground", frames = 420, target = sf2.rules.PLAYER,
                nodes = {
                    { name = "NToeTip_1", axis = "Y", max = 15 },
                    { name = "NToeTip_2", axis = "Y", max = 15 },
                    { name = "NKnee_1", axis = "Y", max = 30 },
                    { name = "NKnee_2", axis = "Y", max = 30 },
                    { name = "NPivot", axis = "Y", max = 30 },
                    { name = "NNeck", axis = "Y", max = 30 },
                    { name = "NTop", axis = "Y", max = 30 },
                },
                animations = { "ThrowFall", "Jump", "PhysicalFall" },
            },
            damage("hot_ground_player", sf2.rules.PLAYER, 2000),
            damage("hot_ground_opponent", sf2.rules.OPPONENT, -2000),
        },
    },
    {
        name = "Ring out", text = "Stay inside the arena boundaries. Crossing either edge loses the round.",
        rules = {
            sf2.rules.ring_out {
                id = "ascension_ring_out", node = "NPivot", axis = "X", min = -600, max = 600,
                target = sf2.rules.PLAYER,
            },
            damage("ring_out_player", sf2.rules.PLAYER, 3000),
            damage("ring_out_opponent", sf2.rules.OPPONENT, -3000),
        },
    },
    {
        name = "No blocks", text = "You cannot block attacks. Use movement to avoid damage.",
        rules = {
            sf2.rules.remove_interval { id = "ascension_no_blocks", type = "Block", target = sf2.rules.PLAYER },
            damage("no_blocks_player", sf2.rules.PLAYER, 1000),
            damage("no_blocks_opponent", sf2.rules.OPPONENT, -1000),
        },
    },
    {
        name = "Enemy regenerates", text = "Keep attacking. The opponent regenerates after three seconds without a hit.",
        rules = {
            sf2.rules.regeneration {
                id = "ascension_regeneration", rate = 0.001, frames_after_hit = 180, target = sf2.rules.OPPONENT,
            },
            damage("regeneration_player", sf2.rules.PLAYER, 1000),
            damage("regeneration_opponent", sf2.rules.OPPONENT, -1000),
        },
    },
    {
        name = "No jumps", text = "Neither fighter may jump.",
        rules = { sf2.rules.no_animation { id = "ascension_no_jumps", name = "Jump" } },
    },
}

local perk_names = {
    "PRECISION_WEAPON", "OVERHEAT_WEAPON", "INTOXICATION_WEAPON", "WEAKNESS_WEAPON",
    "BLOODRAGE_WEAPON", "REJUVENATION_ARMOR", "DAMAGE_ABSORPTION_BODY_ARMOR", "LIFESTEAL_WEAPON",
    "BLEEDING_WEAPON", "ENFEEBLE_WEAPON", "FRENZY_WEAPON", "STUN_WEAPON",
    "TIME_BOMB_WEAPON", "REGENERATION_ARMOR", "DAMAGE_RETURN_ARMOR", "SHIELDING_ARMOR",
}
local buffs = {}
for i, name in ipairs(perk_names) do
    buffs[i] = sf2.rules.perk {
        id = "ascension_buff_" .. i,
        perk = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_" .. name),
        aspect = 100000, target = sf2.rules.OPPONENT,
    }
end

return { base = base, challenges = challenges, buffs = buffs }
]==]
return {}
