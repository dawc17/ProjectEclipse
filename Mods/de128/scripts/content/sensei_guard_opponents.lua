local sf2 = require("sf2")
-- Pending historical Sensei loadouts, not loaded by main.lua. The archive's
-- Guard_Girl/Guard_Man templates are absent from every source; callers pass the
-- synthesized template specs from sensei_guard_templates.lua and the Sphere1
-- handle from sensei_dependencies.lua. This module invents no defaults.
-- Reconcile with SOURCE_CORPUS.md before claiming current-corpus parity.
local characters = {
    { act = 1, slot = 1, template = "guard_girl", avatar = "character_savage", name = "character_Savage", attributes = true,
      items = { "weapon/WEAPON_TRIANGLE_KNIVES", "armor/ARMOR_WOMAN_GREEN", "helm/HEAD_KENJI" } },
    { act = 1, slot = 2, template = "guard_man", avatar = "character_philosopher", name = "character_Philosopher", attributes = true,
      items = { "armor/ARMOR_OLD_LEATHER", "weapon/WEAPON_KNUCKLES", "helm/HELM_GREEN_MASK" } },
    { act = 2, slot = 1, template = "guard_girl", avatar = "character_asian", name = "character_Asian",
      items = { "weapon/WEAPON_SUPER_FANS", "armor/BODY_WOMAN", "helm/HEAD_NIGHT" } },
    { act = 2, slot = 2, template = "guard_man", avatar = "character_ronin", name = "character_Ronin", attributes = true,
      items = { "weapon/WEAPON_NINJA_SWORD", "armor/ARMOR_ROBE", "helm/HELM_CONICAL_HAT" } },
    { act = 3, slot = 1, template = "guard_girl", avatar = "character_sadist", name = "character_Sadist",
      items = { "weapon/WEAPON_SICKLES", "armor/BODY_WOMAN", "helm/HELM_SPIKE" } },
    { act = 3, slot = 2, template = "guard_man", avatar = "character_fanatic", name = "character_Fanatic", attributes = true,
      items = { "weapon/WEAPON_MACHETE", "armor/ARMOR_STRONG_BARBARIAN", "helm/HELM_LEGIONER" } },
    { act = 4, slot = 1, template = "guard_man", avatar = "character_pirate", name = "character_Pirate",
      items = { "weapon/WEAPON_CHINESE_SABERS", "helm/HELM_MANTIS", "armor/ARMOR_ROBE" } },
    { act = 4, slot = 2, template = "guard_girl", avatar = "character_indean", name = "character_Indean", attributes = true,
      items = { "weapon/WEAPON_KUSARIGAMA", "armor/ARMOR_WOMAN_BARBARIAN", "helm/HEAD_KENJI" } },
    { act = 5, slot = 1, template = "guard_girl", avatar = "character_sister", name = "character_Sister",
      items = { "weapon/WEAPON_YARI", "armor/BODY_WOMAN", "helm/HELM_ASSASSIN" } },
    { act = 5, slot = 2, template = "guard_man", avatar = "character_blind", name = "character_Blind", attributes = true,
      items = { "weapon/WEAPON_DADAO", "armor/ARMOR_MANTLE_OF_NIGHT", "helm/Head" } },
    { act = 6, slot = 2, template = "guard_man", avatar = "character_prince_evil", name = "NAME_PRINCE_EVIL", attributes = true,
      items = { "armor/ARMOR_SUPER_CLOAK", "weapon/WEAPON_KEEN_KATANA", "helm/HELM_CLOSED" }, prince = true },
}

local function register(dependencies)
    assert(type(dependencies) == "table" and dependencies.guard_girl and dependencies.guard_man and dependencies.sphere1,
        "Sensei opponents require Guard_Girl, Guard_Man template specs and a Sphere1 handle")
    for _, name in ipairs({ "guard_girl", "guard_man" }) do
        local spec = dependencies[name]
        assert(type(spec) == "table" and spec.template ~= nil and (spec.voice == "Female" or spec.voice == "Male"),
            "Guard template spec requires a template handle and a Female/Male voice: " .. name)
    end
    local bosses = require("content.sensei_boss_opponents")
    local result = {}
    for act = 1, 6 do result[act] = { normal = {}, eclipse = {} } end
    for _, character in ipairs(characters) do
        for _, mode in ipairs({ "normal", "eclipse" }) do
            local items = {}
            for _, reference in ipairs(character.items) do items[#items + 1] = sf2.items.get("core:items/" .. reference) end
            local perks = {}
            if character.prince then
                items[#items + 1] = dependencies.sphere1
                for _, name in ipairs({ "PERK_ITEM_SPECIAL_LIFESTEAL_WEAPON", "PERK_ITEM_SPECIAL_LIFESTEAL_RANGED",
                    "PERK_ITEM_SPECIAL_MAGIC_PAIN_RECHARGE_ARMOR", "PERK_ITEM_SPECIAL_MAGIC_PAIN_RECHARGE_HELM" }) do
                    perks[#perks + 1] = { perk = sf2.perks.get("core:perks/" .. name), aspect = 100000, chance = 0.25 }
                end
            end
            local shift = mode == "eclipse" and 7 or (character.prince and 8 or 6)
            local alignments = { { factor = 1, shift = shift, priority = 1 } }
            if mode == "normal" then alignments[2] = { factor = 1, shift = shift, priority = 1 } end
            local warrior = sf2.warriors.register {
                id = "sensei_act_" .. character.act .. "_guard_" .. character.slot .. "_" .. mode,
                template = dependencies[character.template].template,
                voice = dependencies[character.template].voice, tactic = "Standard",
                first_name = character.name, avatar = require("content.sensei_art").avatar(character.avatar), items = items, perks = perks,
                attributes = character.attributes and { WeaponDamage = 10, UnarmedDamage = 0, BodyDefense = 8, HeadDefense = 2 } or nil,
                attribute_alignments = alignments,
            }
            result[character.act][mode][character.slot] = warrior
        end
    end
    -- Preserve native gauntlet order: guards then boss in I–V, Shogun then
    -- the prince in VI. Normal encounters use the same slots as separate fights.
    for act = 1, 6 do
        local slot = act == 6 and 1 or 3
        result[act].normal[slot] = bosses[act].normal
        result[act].eclipse[slot] = bosses[act].eclipse
    end
    return result
end

return { register = register }
