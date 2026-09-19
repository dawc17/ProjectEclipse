local sf2 = require("sf2")
-- This isolated acceptance profile exercises map locks/combat, not the dojo tutorial.
sf2.quests.suppress { target = "core:quests/quest_extensions/tutorial_quests.xml/storytutorialwelcome" }
require("content.sensei_act_one_opponents") -- Copied from the authored pending DE module by the runner.
require("content.sensei_rewards")
local function animation_probe(_, fighter, event)
    if event.animation_name:sub(1, 12) ~= "de128:moves/" then return end
    if event.type == "AnimationStart" then
        if event.target == "self" and event.animation_name:sub(-7) == "_player" then
            assert(not fighter:has_flag("cast"))
            local flag = fighter:set_flag("cast")
            fighter:set_flag("cast") -- idempotent: no duplicate native modifier
            assert(fighter:has_flag("cast"))
            sf2.log.info("[DE128Flag] set|" .. flag)
        elseif event.target == "other" and fighter:has_flag("cast") then
            fighter:clear_flag("cast")
            assert(not fighter:has_flag("cast"))
            fighter:clear_flag("cast") -- absent clear must not emit another expiry
            sf2.log.info("[DE128Flag] cleared")
        end
    end
    sf2.log.info("[DE128Lifecycle] " .. event.type .. "|" .. fighter.side .. "|"
        .. event.target .. "|" .. event.animation_name .. "|" .. tostring(event.frame))
end
local lifecycle = sf2.behaviors.register {
    id = "animation_lifecycle", on_animation_start = animation_probe, on_animation_end = animation_probe,
}
local lifecycle_rule = sf2.rules.behavior {
    id = "animation_lifecycle", behavior = lifecycle, target = sf2.rules.BOTH,
}
local arena = sf2.locations.register {
    id = "arena", color = "0x1b2230", wall = 200, floor = 80,
    width = 1936, height = 512, min_width = 1936,
    layers = {
        { type = 1, factor = 1, images = {
            { sprite = sf2.assets.sprite("core:Textures/Locations/battlefield/battlefield_bg1.back_1"),
              x = 0, y = 0, width = 1936, height = 1024 },
        } },
        { type = 2, factor = 1, fighters = { player_x = 468, player_y = -94, enemy_x = 1268, enemy_y = -94 } },
    },
}
local location = sf2.locations.name(arena)
local opponent = sf2.warriors.register {
    id = "jian", template = sf2.warriors.get_template("core:warrior-templates/man_staff"),
    tactic = "Standard", first_name = "DE128 acceptance", last_name = "", level = 1,
    items = { sf2.items.get("core:items/weapon/WEAPON_CHNY21_JIAN"),
        sf2.items.get("de128:items/magic/minor_charge_of_darkness") },
}
local zone = sf2.zones.register { id = "trial", file = "Map1.1", start = false }
local battle = sf2.battles.register { id = "trial", zone = zone, type = sf2.battles.STORY,
    x = 0, y = 0, alias = "DE128 acceptance", title = "DE128 acceptance", description = "", location = location }
local loss = sf2.rewards.register { id = "loss", items = {} }
local win = sf2.rewards.register { id = "win", items = {} }
local fight = sf2.fights.register { id = "jian", battle = battle, location = location,
    warriors = { opponent }, rewards = { loss, win }, rules = { lifecycle_rule }, rounds = 1, round_time = 99 }
local locked_battle = sf2.battles.register { id = "lock_check", zone = zone, type = sf2.battles.STORY,
    icon = "tournament", icon_atlas = "BattleBtnStart", title = "Lock check", x = 200, y = 0, location = location }
sf2.fights.register { id = "lock_check", battle = locked_battle, location = location,
    warriors = { opponent }, rewards = { loss, win }, rounds = 1, round_time = 99 }
local sphere2_opponent = sf2.warriors.register {
    id = "sphere2", template = sf2.warriors.get_template("core:warrior-templates/man_staff"),
    tactic = "Standard", first_name = "Sphere2 acceptance", last_name = "", level = 1,
    items = { sf2.items.get("core:items/weapon/WEAPON_CHNY21_JIAN"),
        sf2.items.get("de128:items/magic/medium_charge_of_darkness") },
}
local sphere2_battle = sf2.battles.register { id = "sphere2", zone = zone, type = sf2.battles.STORY,
    x = 0, y = 0, alias = "Sphere2 acceptance", title = "Sphere2 acceptance", description = "", location = location }
local sphere2_fight = sf2.fights.register { id = "sphere2", battle = sphere2_battle, location = location,
    warriors = { sphere2_opponent }, rewards = { loss, win }, rules = { lifecycle_rule }, rounds = 1, round_time = 99 }
sf2.modes.register { id = "trial", fights = { fight }, repeatable = true }
sf2.modes.register { id = "sphere2", fights = { sphere2_fight }, repeatable = true }

local sphere3_opponent = sf2.warriors.register {
    id = "sphere3", template = sf2.warriors.get_template("core:warrior-templates/man_staff"),
    tactic = "Standard", first_name = "Sphere3 acceptance", last_name = "", level = 1,
    items = { sf2.items.get("core:items/weapon/WEAPON_CHNY21_JIAN"),
        sf2.items.get("de128:items/magic/large_charge_of_darkness") },
}
local sphere3_battle = sf2.battles.register { id = "sphere3", zone = zone, type = sf2.battles.STORY,
    x = 0, y = 0, alias = "Sphere3 acceptance", title = "Sphere3 acceptance", description = "", location = location }
local sphere3_fight = sf2.fights.register { id = "sphere3", battle = sphere3_battle, location = location,
    warriors = { sphere3_opponent }, rewards = { loss, win }, rules = { lifecycle_rule }, rounds = 1, round_time = 99 }
sf2.modes.register { id = "sphere3", fights = { sphere3_fight }, repeatable = true }

local combosphere3_opponent = sf2.warriors.register {
    id = "combosphere3", template = sf2.warriors.get_template("core:warrior-templates/man_staff"),
    tactic = "Standard", first_name = "ComboSphere3 acceptance", last_name = "", level = 1,
    items = { sf2.items.get("core:items/weapon/WEAPON_CHNY21_JIAN"),
        sf2.items.get("de128:items/magic/blast_of_the_void") },
}
local combosphere3_battle = sf2.battles.register { id = "combosphere3", zone = zone, type = sf2.battles.STORY,
    x = 0, y = 0, alias = "ComboSphere3 acceptance", title = "ComboSphere3 acceptance", description = "", location = location }
local combosphere3_fight = sf2.fights.register { id = "combosphere3", battle = combosphere3_battle, location = location,
    warriors = { combosphere3_opponent }, rewards = { loss, win }, rules = { lifecycle_rule }, rounds = 1, round_time = 99 }
sf2.modes.register { id = "combosphere3", fights = { combosphere3_fight }, repeatable = true }

local mind_opponent = sf2.warriors.register {
    id = "mindthrownormal", template = sf2.warriors.get_template("core:warrior-templates/man_staff"),
    tactic = "Standard", first_name = "MindThrow acceptance", last_name = "", level = 1,
    items = { sf2.items.get("core:items/weapon/WEAPON_CHNY21_JIAN"), sf2.items.get("de128:items/magic/mind_throw") },
}
local mind_battle = sf2.battles.register { id = "mindthrownormal", zone = zone, type = sf2.battles.STORY,
    x = 0, y = 0, alias = "MindThrow acceptance", title = "MindThrow acceptance", description = "", location = location }
local mind_fight = sf2.fights.register { id = "mindthrownormal", battle = mind_battle, location = location,
    warriors = { mind_opponent }, rewards = { loss, win }, rules = { lifecycle_rule }, rounds = 1, round_time = 99 }
sf2.modes.register { id = "mindthrownormal", fights = { mind_fight }, repeatable = true }

-- Inert move definitions used to inspect actual native action parsing after boot.
local fixture_animation = sf2.assets.binary("de128:animations/chinese_swords_super_slash_old")
local child = sf2.moves.register { id = "projectile_child", animation = fixture_animation,
    conditions = {
        { type = "actor_name", name = "FixtureSphere" },
        { type = "bullets", bullet_type = "MagicBullet", minimum = 1, maximum = 2 },
    }, velocity = { x = 30, ay = -2, save_velocity = true }, no_magic_recharge = true,
}
sf2.moves.register { id = "projectile_actions", animation = fixture_animation, actions = {
    { type = "create_projectile", frame = 2, projectile = {
        name = "Sphere1", core_skeleton = "SkeletonMagic", copy_parent_type = "Magic",
    } },
    { type = "create_projectile", frame = 3, projectile = {
        name = "Preview", core_skeleton = "SkeletonMagic", copy_parent_type = "Magic", start_move = child,
    } },
    { type = "add_bullets", frame = 7, bullets = { type = "MagicBullet", value = -1 } },
    { type = "delete_actor", event = "Strike", player = "Me" },
} }
