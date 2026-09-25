local sf2 = require("sf2")
-- DE Underworld: eight tiers of offline boss fights (archive raid_stages_default.xml)
-- registered as Underworld map pages, with Power Mode twins. Data is generated
-- offline by Tools/GenerateDE128Underworld.py; this module only registers it.
local data = require("content.underworld_data")
local text = require("content.underworld_text")
local wasp_fly = require("content.wasp_fly")

local ZONE_TITLES = { "ZONE_RAID", "ZONE_RAID1", "ZONE_RAID2", "ZONE_RAID3", "ZONE_RAID4", "ZONE_RAID5", "ZONE_RAID6", "ZONE_RAID7" }

-- Archive music ids name the packaged raid tracks under their "raids_" names.
-- Ids without a packaged counterpart keep the native id (resolved by Sound.cs).
local MUSIC = {
    vulcan = "raids_vulcan", crystal = "raids_crystal", fungus = "raids_fungus", vortex = "raids_vortex",
    fatum = "raids_fatum", hunger = "raids_hunger", drakaina = "raids_war", fear = "raids_fear",
    holyman7 = "raids_arkhos", holyman8 = "raids_hoaxen", dark_ritual = "fight38_dark_ritual",
    fight_halloween2022 = "hw22", raid_newyear18 = "new_year_18",
}

-- Event-raid map buttons absent from core art ship as DE128 sprites.
-- Archive music ids no packaged track provides; DE128 ships them (Tools/ExtractDE128UnderworldArt.py).
local OWNED_MUSIC = { fight_halloween2019 = true, flying_rocks = true, halls_of_the_dead_heroes = true, ninja_in_the_night_old = true }
local OWNED_BUTTONS = {
    BattleBtnArchitect = true, BattleBtnHalloween = true, BattleBtnLamb = true, BattleBtnNrityu = true,
    BattleBtnPuppeteer = true, BattleBtnRakshasa = true, BattleBtnRavana = true, BattleBtnShurale = true,
    BattleBtnSnowflake = true, BattleBtnWindWolf = true, BattleBtnPrince = true,
}
-- Only these twenty owner portraits are packaged by DE128. The older
-- boss_wind_wolf_new name is already a native core resource.
local OWNED_AVATARS = {
    boss_architect_hummer_new = true, boss_arkhos_hardmode_new = true,
    boss_bison_hard_new = true, boss_crystal_hardmode_new = true,
    boss_fatum_hardmode_new = true, boss_fire_hardmode_new = true,
    boss_hoaxen_hardmode_new = true, boss_hunger_hardmode_new = true,
    boss_lamb_fungus_hard_new = true, boss_lamb_hard_new = true,
    boss_lamb_hunger_hard_new = true, boss_mushroom_hardmode_new = true,
    boss_rakshasa_hardmode_new = true, boss_ravana_hard_new = true,
    boss_saturn_hard_new = true, boss_tenebris_hardmode_new = true,
    boss_vortex_hardmode_new = true, boss_war_hardmode_new = true,
    boss_whisper_hardmode_new = true, new_man_shuang_gou_hardmode_new = true,
}

local function avatar(name)
    if OWNED_AVATARS[name] then
        return sf2.assets.sprite("sprites/underworld/" .. name)
    end
    return name
end

local BATTLE_TYPES = { final = sf2.battles.FINAL, survival = sf2.battles.SURVIVAL }

local function lower_id(name) return (name:gsub("[^%w_]", "_")):lower() end

local function install(raid_charge_rule)
    assert(raid_charge_rule, "Underworld fights need the shared RaidCharge conditional rule")
    local items, perks = {}, {}
    local function item(ref) items[ref] = items[ref] or sf2.items.get(ref); return items[ref] end
    local function perk(ref) perks[ref] = perks[ref] or sf2.perks.get(ref); return perks[ref] end
    local function item_list(refs)
        if not refs then return nil end
        local result = {}
        for index, ref in ipairs(refs) do result[index] = item(ref) end
        return result
    end
    local function perk_rows(rows)
        if not rows then return nil end
        local result = {}
        for index, row in ipairs(rows) do
            result[index] = { perk = perk(row.perk), aspect = row.aspect, chance = row.chance,
                chance_factor = row.chance_factor, frames = row.frames, parameters = row.parameters }
        end
        return result
    end

    -- Templates, parents first (the generator orders them).
    local templates = {}
    local function template(ref)
        if ref:sub(1, 5) == "core:" then
            return sf2.warriors.get_template("core:warrior-templates/" .. ref:sub(6):lower())
        end
        return assert(templates[ref], "Unregistered Underworld template " .. ref)
    end
    for _, spec in ipairs(data.templates) do
        templates[spec.name] = sf2.warriors.register_template {
            id = "uw_" .. lower_id(spec.name), template = spec.parent and template(spec.parent) or nil,
            first_name = spec.first_name and text.key(spec.first_name) or nil,
            avatar = avatar(spec.avatar), voice = spec.voice, health_bars = spec.health_bars,
            attributes = spec.attributes, items = item_list(spec.items), skeleton = spec.skeleton,
        }
    end

    local function build_rule(spec, id)
        local kind = spec.kind
        if kind == "raid_charge" then return raid_charge_rule end
        if kind == "perk" then
            return sf2.rules.perk { id = id, perk = perk(spec.perk), target = spec.target, aspect = spec.aspect,
                parameters = spec.parameters }
        end
        if kind == "group" or kind == "random" then
            local children = {}
            for index, child in ipairs(spec.rules) do children[index] = build_rule(child, id .. "_" .. index) end
            if kind == "group" then
                return sf2.rules.group { id = id, rules = children,
                    description = spec.description and text[spec.description] or nil }
            end
            return sf2.rules.random { id = id, refresh = spec.refresh, rules = children }
        end
        if kind == "equip_item" then return sf2.rules.equip_item { id = id, item = item(spec.item), target = spec.target } end
        if kind == "avatar" then return sf2.rules.avatar { id = id, name = spec.name, target = spec.target } end
        if kind == "name" then return sf2.rules.name { id = id, name = spec.name, target = spec.target } end
        if kind == "no_button" then return sf2.rules.no_button { id = id, name = spec.name, target = spec.target } end
        if kind == "hot_ground" then
            return sf2.rules.hot_ground { id = id, frames = spec.frames, nodes = spec.nodes, animations = spec.animations,
                target = spec.target }
        end
        if kind == "random_area" then
            return sf2.rules.random_area { id = id, image = spec.image, icon = spec.icon, width = spec.width,
                fade_in = spec.fade_in, frames_on = spec.frames_on, fade_out = spec.fade_out,
                frames_off = spec.frames_off, target = spec.target }
        end
        if kind == "light_in_the_darkness" then
            return sf2.rules.light_in_the_darkness { id = id, radius = spec.radius,
                shape = spec.shape, target = spec.target }
        end
        if kind == "no_animation" then return sf2.rules.no_animation { id = id, name = spec.name } end
        if kind == "remove_interval" then return sf2.rules.remove_interval { id = id, type = spec.type, target = spec.target } end
        if kind == "regeneration" then
            return sf2.rules.regeneration { id = id, rate = spec.rate, frames_after_hit = spec.frames_after_hit,
                target = spec.target }
        end
        if kind == "attributes" then return sf2.rules.attributes { id = id, values = spec.values, target = spec.target } end
        if kind == "no_health_bar" then return sf2.rules.no_health_bar { id = id, target = spec.target } end
        if kind == "invert_joystick" then return sf2.rules.invert_joystick { id = id, target = spec.target } end
        error("Unsupported Underworld rule kind " .. tostring(kind))
    end

    local owned_music = {}
    local function music(name)
        if not OWNED_MUSIC[name] then return MUSIC[name] or name end
        owned_music[name] = owned_music[name] or sf2.assets.audio("audio/underworld/" .. name)
        return owned_music[name]
    end
    local zones, battles, fights = {}, {}, {}
    -- by_battle[archive battle][archive fight] = { handle, id } for story hooks.
    local by_battle = {}
    for tier, zone_spec in ipairs(data.zones) do
        local zone_id = "underworld_tier_" .. tier
        sf2.localization.register { id = "zones/" .. zone_id, language = "eng",
            value = text.value(ZONE_TITLES[tier], "eng") }
        for _, language in ipairs(text.languages) do
            local value = text.value(ZONE_TITLES[tier], language)
            if value and language ~= "eng" then
                sf2.localization.register { id = "zones/" .. zone_id, language = language, value = value }
            end
        end
        local zone = sf2.zones.register { id = zone_id, file = zone_spec.file, underworld = true }
        zones[tier] = zone
        for _, spec in ipairs(zone_spec.battles) do
            local battle_id = "uw_" .. lower_id(spec.name)
            local icons = nil
            if spec.icon_atlas and OWNED_BUTTONS[spec.icon_atlas] then
                icons = {
                    base = sf2.assets.sprite("sprites/underworld/" .. spec.icon_atlas:lower() .. "_base"),
                    active = sf2.assets.sprite("sprites/underworld/" .. spec.icon_atlas:lower() .. "_active"),
                }
            end
            local description = nil
            if spec.description then
                description = text.key(spec.description) .. (spec.description_level and ("{" .. spec.description_level .. "}") or "")
            end
            local battle = sf2.battles.register {
                id = battle_id, zone = zone, type = BATTLE_TYPES[spec.type], x = spec.x, y = spec.y,
                alias = text.key(spec.alias), title = text.key(spec.title), description = description,
                icon = spec.icon, icon_atlas = (not icons) and spec.icon_atlas or nil, icons = icons,
                preview = spec.preview, location = spec.location, music = music(spec.music),
                power_mode = spec.power_mode,
            }
            battles[spec.name] = battle
            by_battle[spec.name] = {}
            for _, fight_spec in ipairs(spec.fights) do
                local prefix = battle_id .. "_" .. lower_id(fight_spec.name)
                local warriors = {}
                for index, w in ipairs(fight_spec.warriors) do
                    local tactic = w.template == "Boss_Wasp_Young" and wasp_fly.tactic or w.tactic
                    warriors[index] = sf2.warriors.register {
                        id = prefix .. "_w" .. index, template = template(w.template), tactic = tactic,
                        avatar = avatar(w.avatar), health_bars = w.health_bars, attributes = w.attributes,
                        attribute_alignments = w.alignments, perks = perk_rows(w.perks),
                        items = item_list(w.items), skeleton = w.skeleton,
                    }
                end
                local rewards = {}
                for index, r in ipairs(fight_spec.rewards) do
                    rewards[index] = sf2.rewards.register {
                        id = prefix .. "_r" .. (index - 1), prize_base = r.prize_base, experience = r.experience,
                        gems = r.gems, currencies = r.currencies,
                    }
                end
                local rules = {}
                for index, rule in ipairs(fight_spec.rules) do rules[index] = build_rule(rule, prefix .. "_rule" .. index) end
                local fight = sf2.fights.register {
                    id = prefix, battle = battle, power = fight_spec.power, rounds = fight_spec.rounds,
                    round_time = fight_spec.round_time, replays = fight_spec.replays,
                    health_recovery = fight_spec.health_recovery, warriors = warriors, rules = rules, rewards = rewards,
                }
                fights[#fights + 1] = fight
                by_battle[spec.name][fight_spec.name] = { handle = fight, id = sf2.mod.id .. ":fights/" .. prefix }
            end
        end
    end
    return { zones = zones, battles = battles, fights = fights, templates = templates, by_battle = by_battle }
end

return { install = install, music = MUSIC }
