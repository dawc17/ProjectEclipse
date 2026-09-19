local sf2 = require("sf2")
-- Pending static portions of the 23 archived Sensei fights; not loaded by main.lua.
-- The assembler must insert a conditional RaidCharge rule at charge_position.
-- Do not attach raid_charge unconditionally: the legacy wrapper ignores its test.
local function equip(id, kind, name)
    return sf2.rules.equip_item {
        id = "sensei_" .. id, item = sf2.items.get("core:items/" .. kind .. "/" .. name),
        target = sf2.rules.PLAYER,
    }
end
local equipment = {
    equip("no_magic", "magic", "NoMagic"),
    equip("armour", "armor", "ARMOR_FOREIGN"),
    equip("helm", "helm", "HELM_LIGHT"),
    equip("weapon", "weapon", "WEAPON_NINJA_SWORD"),
}
local no_ranged = equip("no_ranged", "ranged", "NoRanged")
local ranged = equip("ranged", "ranged", "RANGED_SHURIKENS")
local anti_shock = sf2.rules.perk {
    id = "sensei_boss_anti_shock", perk = sf2.perks.get("core:perks/PERK_ANTI_SHOCK"), target = sf2.rules.OPPONENT,
}
local avatar = sf2.rules.avatar { id = "sensei_player_avatar", name = "character_sensei_young", target = sf2.rules.PLAYER }
local name = sf2.rules.name { id = "sensei_player_name", name = "characterSensei", target = sf2.rules.PLAYER }
local ronin_player = sf2.rules.attributes {
    id = "sensei_ronin_player", mode = sf2.rules.ECLIPSE, target = sf2.rules.PLAYER, values = { DamageFactor = -4000 },
}
local ronin_enemy = sf2.rules.attributes {
    id = "sensei_ronin_enemy", mode = sf2.rules.ECLIPSE, target = sf2.rules.OPPONENT, values = { DamageFactor = 4000 },
}
local raid_charge = sf2.rules.no_button { id = "sensei_raid_charge", name = "RaidCharge", target = sf2.rules.PLAYER }
local function sequence(act, protected, ronin)
    local rules = {}
    for _, item in ipairs(equipment) do rules[#rules + 1] = item end
    rules[#rules + 1] = act == 1 and no_ranged or ranged
    if protected then rules[#rules + 1] = anti_shock end
    rules[#rules + 1] = avatar
    rules[#rules + 1] = name
    local charge_position = #rules + 1
    if ronin then
        rules[#rules + 1] = ronin_player
        rules[#rules + 1] = ronin_enemy
    end
    return { unconditional = rules, charge_position = charge_position }
end
local acts = {}
for act = 1, 6 do
    local normal = {}
    local count = act == 6 and 2 or 3
    for fight = 1, count do
        normal[fight] = sequence(act, act == 6 or fight == 3, act == 2 and fight == 2)
    end
    acts[act] = { normal = normal, eclipse = sequence(act, act >= 2, false) }
end
return { acts = acts, raid_charge = raid_charge }
