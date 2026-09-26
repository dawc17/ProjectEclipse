local sf2 = require("sf2")
local text = require("content.challenger_text")
local shared = require("content.sensei_state")

-- The seven post-Titan Challengers (owner stages.xml ZONE_1..ZONE_6 Challenger and
-- ZONE_6 Challenger_2; quests UnlockChallengerBattles and DropWeapon_<Name>).
-- Art and music ship with DE128 (Tools/ExtractDE128ChallengerArt.py).
local CHALLENGERS = {
    {
        key = "trickster", zone = "zone_1", battle = "challenger_trickster", x = 160, y = -45,
        alias = "characterTrickster", title = "TRICKSTER_Title", description = "descTrickster",
        location = "mountain", music = "samurai_spirit", experience = 0,
        first_name = "NAME_TRICKSTER", avatar = "man_nunchaku_new", voice = "Male",
        items = { "weapon/WEAPON_NUNCHAKU", "armor/ARMOR_RONIN", "helm/HELM_CONICAL_HAT" }, skeleton = "Skeleton",
        perk = "PERK_ITEM_SPECIAL_OVERHEAT_WEAPON",
        weapon = "WEAPON_NUNCHAKU", drop = "Trickster_drop", drop_image = "drop_nunchaku_new",
        grant = { perk = "PERK_ITEM_SPECIAL_OVERHEAT_WEAPON", chance = 0.3 },
    },
    {
        key = "hawk", zone = "zone_2", battle = "challenger_hawk", x = 5, y = 155,
        alias = "NAME_HAWK", title = "HAWK_Title", description = "descHawk",
        location = "sakura", music = "blade_dance", experience = 1,
        first_name = "NAME_HAWK", avatar = "man_ninja_naginata_new", voice = "Male",
        items = { "weapon/WEAPON_NAGINATA", "armor/ARMOR_GREEN", "helm/HELM_GREEN_MASK" }, skeleton = "Skeleton",
        perk = "PERK_ITEM_SPECIAL_BLOODRAGE_WEAPON",
        weapon = "WEAPON_NAGINATA", drop = "Hawk_drop", drop_image = "drop_naginata_new",
        -- The archive grants PERK_ITEM_SPECIAL_BLOODRAGE_WEAPON_STRANGER, a DE-only
        -- child of the core Bloodrage perk whose Set repeats these exact values.
        grant = { perk = "PERK_ITEM_SPECIAL_BLOODRAGE_WEAPON", chance = 0.3,
            parameters = { Base = -1000, DamageFactor = 15850 } },
    },
    {
        key = "rose", zone = "zone_3", battle = "challenger_rose", x = 65, y = -180,
        alias = "NAME_ROSE", title = "ROSE_Title", description = "descRose",
        location = "pink_lake", music = "ronin", experience = 1,
        first_name = "NAME_ROSE", avatar = "girl_katana_new", voice = "Female",
        items = { "weapon/WEAPON_KATANA", "armor/ARMOR_WOMAN_GREEN", "helm/HELM_STEEL", "ranged/RANGED_HUNTERS_KNIVES" },
        skeleton = "Skeleton",
        perk = "PERK_ITEM_SPECIAL_OVERHEAT_WEAPON",
        weapon = "WEAPON_KATANA", drop = "Rose_drop", drop_image = "drop_katana_new",
        grant = { perk = "PERK_ITEM_SPECIAL_OVERHEAT_WEAPON", chance = 0.35, parameters = { DamageFactor = 15850 } },
    },
    {
        key = "fisher", zone = "zone_4", battle = "challenger_fisher", x = 90, y = -130,
        alias = "NAME_FISHER", title = "FISHER_Title", description = "descFisher",
        location = "heaven", music = "heavenly_clouds", experience = 1,
        first_name = "NAME_FISHER", avatar = "man_cool_staff_new", voice = "Male",
        items = { "weapon/WEAPON_WANDERER_STAFF", "armor/ARMOR_DRAGON", "helm/HELM_SILVER",
            "ranged/RANGED_HEAVY_SHURIKENS", "magic/MAGIC_BOMB" },
        skeleton = "Skeleton",
        perk = "PERK_ITEM_SPECIAL_WEAKNESS_WEAPON",
        weapon = "WEAPON_WANDERER_STAFF", drop = "Fisher_drop", drop_image = "drop_wanderer_staff_new",
        grant = { perk = "PERK_ITEM_SPECIAL_WEAKNESS_WEAPON", chance = 0.41, frames = 300, parameters = { Base = -20000 } },
    },
    {
        key = "outcast", zone = "zone_5", battle = "challenger_outcast", x = -90, y = 120,
        alias = "NAME_OUTCAST", title = "OUTCAST_Title", description = "descOutcast",
        location = "snowy_peak", music = "fuji", experience = 1,
        first_name = "NAME_OUTCAST", avatar = "man_heavy_kusarigama_new", voice = "Male",
        items = { "weapon/WEAPON_HEAVY_KUSARIGAMA", "armor/ARMOR_GILDED", "helm/HELM_TWO_FACED",
            "ranged/RANGED_GHOST_KUNAI", "magic/MAGIC_LIGHTNING_ARROW" },
        skeleton = "Skeleton",
        perk = "PERK_ITEM_SPECIAL_PRECISION_WEAPON",
        weapon = "WEAPON_HEAVY_KUSARIGAMA", drop = "Outcast_drop", drop_image = "drop_kusarigama_new",
        grant = { perk = "PERK_ITEM_SPECIAL_PRECISION_WEAPON", chance = 0.2, parameters = { DamageFactor = 10000 } },
    },
    {
        -- Ronin and Nova use the historical Assets/DExml positions. The owner drop's
        -- (-505, 100) and (295, -220) differ from the shipped game's map (owner screenshot).
        key = "ronin", zone = "zone_6", battle = "challenger_ronin", x = -455, y = 130,
        alias = "NAME_RONIN", title = "RONIN_Title", description = "descRonin",
        location = "waterfall", music = "sky_isles", experience = 1,
        first_name = "NAME_RONIN", avatar = "man_dadao_janissary_new", voice = "Male",
        items = { "weapon/WEAPON_DADAO_JANISSARY", "armor/ARMOR_BIG_QUILTED", "helm/HELM_TWO_FACED",
            "ranged/RANGED_KUNAI_OF_WIND", "magic/MAGIC_WATER_BALL" },
        skeleton = "SkeletonHeavy",
        perk = "PERK_ITEM_SPECIAL_BLOODRAGE_WEAPON",
        weapon = "WEAPON_DADAO_JANISSARY", drop = "Ronin_drop", drop_image = "drop_ring_sword_new",
        -- As for Hawk: the archived _STRANGER perk is core Bloodrage with these values.
        grant = { perk = "PERK_ITEM_SPECIAL_BLOODRAGE_WEAPON", chance = 0.3,
            parameters = { Base = -1000, DamageFactor = 15850 } },
    },
    {
        key = "nova", zone = "zone_6", battle = "challenger_nova", x = -225, y = -245,
        alias = "NAME_IM_STRANGER", title = "NOVA_Title", description = "descNova",
        location = "road", music = "the_monastery", experience = 1,
        first_name = "NAME_IM_STRANGER", avatar = "girl_im_knuckles_new", voice = "Female",
        items = { "weapon/WEAPON_STRANGER_KNUCKLES", "armor/ARMOR_WOMAN_GREEN", "helm/HELM_ASSASSIN",
            "ranged/RANGED_THROWING_AXE", "magic/MAGIC_FIRE_SPLASH" },
        skeleton = "Skeleton",
        perk = "PERK_ITEM_SPECIAL_INTOXICATION_WEAPON",
        weapon = "WEAPON_STRANGER_KNUCKLES", drop = "Nova_drop", drop_image = "img_drop_im_knuckles_new",
        grant = { perk = "PERK_ITEM_SPECIAL_INTOXICATION_WEAPON" },
    },
}
-- The first normal Titan win reveals all seven entries.
local UNLOCK_FIGHT = "core:fights/zone_7/c3_boss_titan/6"

local function sprite(name) return sf2.assets.sprite("sprites/challenger/" .. name:lower()) end

-- Saved drop set (sensei_state.lua): ",key,key," of Challenger keys.
local function pending() return sf2.state.get("challenger_drops_pending") end
local function set_pending(key, on)
    local value = pending()
    local marker = "," .. key .. ","
    local first, last = value:find(marker, 1, true)
    if on and not first then sf2.state.set { challenger_drops_pending = value .. key .. "," } end
    if not on and first then sf2.state.set { challenger_drops_pending = value:sub(1, first) .. value:sub(last + 1) } end
end

local function install(raid_charge_rule)
    assert(raid_charge_rule, "Challenger fights need the shared RaidCharge conditional rule")
    shared.register()
    local default = sf2.warriors.get_template("core:warrior-templates/default")
    local rules = {
        sf2.rules.perk { id = "challenger_anti_shock", perk = sf2.perks.get("core:perks/PERK_ANTI_SHOCK"),
            target = sf2.rules.OPPONENT },
        sf2.rules.attributes { id = "challenger_player_damage", target = sf2.rules.PLAYER, values = { DamageFactor = -8500 } },
        sf2.rules.attributes { id = "challenger_opponent_damage", target = sf2.rules.OPPONENT, values = { DamageFactor = 1000 } },
        raid_charge_rule,
    }
    local no_reward = nil
    local by_fight, by_weapon, battles = {}, {}, {}
    for _, c in ipairs(CHALLENGERS) do
        local items = {}
        for index, ref in ipairs(c.items) do items[index] = sf2.items.get("core:items/" .. ref) end
        local template = sf2.warriors.register_template {
            id = "challenger_" .. c.key, template = default, first_name = text.key(c.first_name),
            avatar = sprite(c.avatar), voice = c.voice, attributes = { EnchantmentResistance = 100 },
            items = items, skeleton = c.skeleton,
        }
        local warrior = sf2.warriors.register {
            id = "challenger_" .. c.key, template = template, tactic = "Aggressive",
            attributes = { MagicInitialCharge = 5000, WarriorPower = 1880 },
            attribute_alignments = {
                { factor = 1, shift = 0, priority = 1 },
                { factor = 0.965, shift = 7, priority = 1 },
                { factor = 0, shift = 7, priority = 1 },
            },
            perks = { { perk = sf2.perks.get("core:perks/" .. c.perk), aspect = 100000 } },
        }
        local battle = sf2.battles.register {
            id = c.battle, zone = sf2.zones.get("core:zones/" .. c.zone), type = sf2.battles.FINAL,
            x = c.x, y = c.y, alias = text.key(c.alias), title = text.key(c.title),
            description = text.key(c.description), icon = c.key,
            icons = { base = sprite("battlebtn" .. c.key .. "_base"), active = sprite("battlebtn" .. c.key .. "_active") },
            preview = sprite("preview_" .. c.key), location = c.location,
            music = sf2.assets.audio("audio/challenger/" .. c.music),
        }
        -- DropWeapon_<Name>: the first win grants the weapon at the player's level with
        -- the archived enchantment. Native ownership filtering skips an owned weapon.
        local weapon = sf2.items.get("core:items/weapon/" .. c.weapon)
        local grant_perk = sf2.perks.get("core:perks/" .. c.grant.perk)
        local grant = c.grant
        no_reward = no_reward or sf2.rewards.register { id = "challenger_loss", prize_base = 1 }
        local victory = sf2.rewards.register {
            id = "challenger_" .. c.key .. "_win", prize_base = 1, experience = c.experience, gems = 27,
            items = { {
                item = weapon,
                configure = function(context)
                    return {
                        level = context.player_level,
                        enchantments = { {
                            perk = grant_perk, aspect = 3639 / 100 * context.player_level + 60,
                            chance = grant.chance, frames = grant.frames, parameters = grant.parameters,
                        } },
                    }
                end,
            } },
        }
        sf2.fights.register {
            id = c.battle .. "_1", battle = battle, warriors = { warrior }, rules = rules,
            rewards = { no_reward, victory }, rounds = 3, round_time = 150, replays = 0, power = 0,
        }
        by_fight[sf2.mod.id .. ":fights/" .. c.battle .. "_1"] = c
        by_weapon[("core:items/weapon/" .. c.weapon):lower()] = c
        battles[#battles + 1] = battle
    end

    local scene, view = nil, false
    -- Weapons granted since the last fight began; only a Challenger win consumes them.
    local granted = {}

    local function reveal()
        if sf2.state.get("challengers_revealed") or sf2.profile.fight(UNLOCK_FIGHT).wins < 1 then return end
        for _, battle in ipairs(battles) do
            if not sf2.battles.reveal(battle, false) then return end
        end
        sf2.state.set { challengers_revealed = true }
    end

    local function show_next()
        if scene ~= "map" or view or shared.dialogue_pending() or shared.defeat_pending() then return end
        for _, c in ipairs(CHALLENGERS) do
            if pending():find("," .. c.key .. ",", 1, true) then
                view = sf2.ui.story_dialog {
                    title = text[c.weapon], portrait = sprite(c.drop_image),
                    lines = { { text = text[c.drop] } }, button = text.OK,
                    on_complete = function()
                        view = false
                        set_pending(c.key, false)
                        -- The archived OK button opens the shop's Weapon tab on this item.
                        -- The navigation API opens the shop; it cannot select the tab or item.
                        if pending() == "," then sf2.scenes.open("shop") else show_next() end
                    end,
                    on_cancel = function() view = false end,
                }
                return
            end
        end
    end

    -- The archive shows the drop dialog only when the win actually granted the
    -- weapon. Pair the grant with the win in whichever order they are reported.
    local won = nil
    sf2.story.on("item_acquired", function(event)
        local c = event.item and by_weapon[event.item:lower()]
        if not c or event.previous_count ~= 0 or event.count <= 0 then return end
        granted[c.key] = true
        if won == c then set_pending(c.key, true) end
    end)
    sf2.story.on("battle_result", function(event)
        local c = event.fight and by_fight[event.fight]
        if not c or event.outcome ~= "win" then return end
        won = c
        if granted[c.key] then set_pending(c.key, true) end
    end)
    sf2.story.on("scene_enter", function(event)
        scene = event.scene
        view = false
        if scene == "fight" then granted, won = {}, nil end
        if scene ~= "map" then return end
        reveal()
        show_next()
    end)
    return { battles = battles }
end

return { install = install }
