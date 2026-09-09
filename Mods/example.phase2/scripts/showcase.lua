local sf2 = require("sf2")
local function key(name) return sf2.localization.key(name) end
local function alias(name) return "example.phase2:localization/" .. name end

sf2.timers.set { subsystem = "forge", seconds = 0, skip_enabled = false }
for _, feature in ipairs({ "paid_offers", "battle_pass", "ads", "rewarded_video", "online_services", "payments" }) do
    sf2.services.disable(feature)
end

local icon = sf2.assets.sprite("core:UI/Skills/SkillsEnch02.EnchantmentFrenzy")
local ticket = sf2.items.register_consumable {
    id = "trial_ticket", display_name = key("ticket"), icon = icon, subtype = "TrialTicket",
    pack_label = "phase2", silent_receive = false, spend_after_use = false,
}
sf2.shop.set_availability { item = ticket, visibility = sf2.shop.FORCE_HIDDEN }

local arena = sf2.locations.register {
    id = "trial_arena", color = "0x1b2230", wall = 200, floor = 80,
    width = 1936, height = 512, min_width = 1936,
    layers = {
        { type = 1, factor = 1, images = {
            { sprite = sf2.assets.sprite("core:Textures/Locations/battlefield/battlefield_bg1.back_1"),
              x = 0, y = 0, width = 1936, height = 1024 },
        } },
        { type = 2, factor = 1, fighters = { player_x = 868, player_y = -94, enemy_x = 1068, enemy_y = -94 } },
    },
}
local location = sf2.locations.name(arena)
local ward = sf2.behaviors.register {
    id = "opening_ward",
    parameters = { multiplier = sf2.behaviors.NUMBER },
    state = { lifetime = "round", fields = { ready = { type = sf2.behaviors.BOOLEAN, required = true, default = true } } },
    on_damage_resolving = function(self, fighter, event)
        if self.state.ready and event.damage > 0 then
            fighter:scale_incoming_damage(self.params.multiplier)
            self.state.ready = false
            sf2.log.info("Opening Ward reduced the first hit this round for " .. fighter.side .. ".")
        end
    end,
}
local ward_perk = sf2.perks.register {
    id = "opening_ward", behavior = ward, kind = sf2.perks.SINGLE,
    display_name = key("ward"), description = key("ward.description"), icon = icon,
    parameters = { multiplier = 0.5 },
}
sf2.forge.register_recipe {
    id = "opening_ward", alias = alias("ward.forge"), economic_profile = sf2.forge.profile("Simple"),
    items = { { equipment = sf2.forge.WEAPON, enchantments = 1, bar_scale = "1", min_deviation = 0, max_deviation = 0, random_aspect = false } },
    candidates = { { perk = ward_perk, equipment = sf2.forge.WEAPON, min_level = 1, max_level = 52 } },
}

local fighter = sf2.warriors.register {
    id = "trial_fighter", template = sf2.warriors.get_template("core:warrior-templates/default"),
    -- The default equipment template does not provide an AI tactic.
    tactic = "Standard",
    first_name = alias("fighter"), last_name = "", level = 1, perks = { ward_perk },
}
local empty = sf2.rewards.register { id = "loss", items = {} }
local ticket_reward = sf2.rewards.register { id = "ticket", items = { { item = ticket } } }
local monk_items = {
    sf2.items.get("core:items/weapon/WEAPON_C2_Z2_MONK_KATAR"),
    sf2.items.get("core:items/armor/ARMOR_C2_Z2_MONK"),
    sf2.items.get("core:items/helm/HELM_C2_Z2_MONK"),
    sf2.items.get("core:items/ranged/RANGED_C2_Z2_MONK_SHURIKEN"),
    sf2.items.get("core:items/magic/MAGIC_C2_Z2_MONK_ROOT_STUN"),
}
local members, prizes = {}, {}
for _, item in ipairs(monk_items) do
    members[#members + 1] = { item = item, scale = 1 }
    prizes[#prizes + 1] = { item = item }
end
sf2.itemsets.register { id = "trial_monk", title = key("monk"), text = key("monk.text"), brief = key("monk.brief"), members = members }
local monk_reward = sf2.rewards.register { id = "monk", items = prizes }

local zone = sf2.zones.register { id = "trials", file = "Map1.1", start = false }
local raid_zone = sf2.zones.register { id = "offline_depths", file = "Map1.1", start = false }
local entries = {}
local function battle(id, owner_zone, kind, x, y)
    local entry = sf2.battles.register {
        id = id, zone = owner_zone, type = kind, x = x, y = y,
        alias = alias(id), title = alias(id), description = alias(id .. ".description"), location = location,
    }
    entries[#entries + 1] = entry
    return entry
end
local function fight(id, entry, reward)
    return sf2.fights.register {
        id = id, battle = entry, rounds = 1, round_time = 99, location = location,
        warriors = { fighter }, rewards = { empty, reward },
    }
end

local training = battle("training", zone, sf2.battles.STORY, -300, 0)
sf2.events.register {
    id = "open_training", repeatable = true,
    fights = { fight("training", training, ticket_reward) },
}
local ascension = battle("ascension", zone, sf2.battles.STORY, 250, 0)
sf2.modes.register {
    id = "ascension_trial", repeatable = true, reset_on_loss = true,
    fights = {
        fight("trial_1", ascension, ticket_reward),
        fight("trial_2", ascension, ticket_reward),
        fight("trial_3", ascension, monk_reward),
    },
}
local raid = sf2.battles.register {
    id = "raid", zone = raid_zone, type = "raid", x = 0, y = 0,
    alias = alias("raid"), title = alias("raid"), description = alias("raid.description"),
    icon = "vulcan_raid", icon_atlas = "BattleBtn_raid",
    preview = "preview_raid_base.vulcan_raid", location = "vulcan_raid",
}
local raid_boss = sf2.warriors.register {
    id = "raid_boss", template = sf2.warriors.get_template("core:warrior-templates/default"),
    first_name = alias("raid.boss"), last_name = "", avatar = "boss_fire", voice = "Male",
    level = 1, tactic = "Standard",
    items = {
        sf2.items.get("core:items/weapon/WEAPON_SUPER_SABERS"),
        sf2.items.get("core:items/armor/BODY_VULCAN"),
        sf2.items.get("core:items/helm/HEAD_VULCAN"),
        sf2.items.get("core:items/ranged/RANGED_SUPER_BLADE"),
        sf2.items.get("core:items/magic/MAGIC_FIRE_PILLAR"),
    },
    health_bars = 10,
}
local raid_reward = sf2.rewards.register { id = "raid_gems", gems = 25 }
local raid_first = sf2.fights.register {
    id = "raid_boss", battle = raid, rounds = 1, round_time = 300, location = "vulcan_raid",
    warriors = { raid_boss }, rewards = { empty, raid_reward },
}
sf2.raids.register {
    -- New identity preserves old two-boss test progress without reinterpreting it.
    id = "offline_raid_boss", repeatable = true, reset_on_loss = false,
    fights = { raid_first },
}
sf2.quests.register {
    id = "entries", place = "map", priority = 10, events = { "session" },
    actions = {
        { type = "show_battle", battle = training, locked = false },
        { type = "show_battle", battle = ascension, locked = false },
        { type = "show_battle", battle = raid, locked = false },
    },
}
sf2.quests.register {
    id = "raid_completion", place = "fight", priority = 20, events = { "raid_fight_end" },
    conditions = { { op = "any", conditions = {
        { op = "eq", left = { kind = "event_fight" }, right = { kind = "fight_id", fight = raid_first } },
    } } },
    actions = { { type = "dialog", title = alias("raid"), lines = { alias("raid.completed") } } },
}
local guard = sf2.behaviors.register {
    id = "veteran_guard",
    parameters = { healing = sf2.behaviors.NUMBER },
    state = {
        lifetime = "saved", version = 2,
        fields = { activations = { type = sf2.behaviors.INTEGER, required = true, default = 0 } },
        migrations = { [1] = function(old) return { activations = old.activations or 0 } end },
    },
    on_block = function(self, fighter, event)
        if fighter.health <= 0 then return end
        fighter:change_health(self.params.healing)
        fighter:add_damage_shield("guard", 0.25, 180)
        self.state.activations = self.state.activations + 1
        sf2.log.info("Veteran Guard saved activations: " .. self.state.activations)
    end,
}
local guard_perk = sf2.perks.register {
    id = "veteran_guard", behavior = guard, kind = sf2.perks.SINGLE,
    display_name = key("veteran"), description = key("veteran.description"), icon = icon,
    parameters = { healing = 0.02 },
}
sf2.forge.register_recipe {
    id = "veteran_guard", alias = alias("veteran.forge"), economic_profile = sf2.forge.profile("Simple"),
    items = { { equipment = sf2.forge.WEAPON, enchantments = 1, bar_scale = "1",
        min_deviation = 0, max_deviation = 0, random_aspect = false } },
    candidates = { { perk = guard_perk, equipment = sf2.forge.WEAPON, min_level = 1, max_level = 52 } },
}
sf2.log.info("Phase 2 registered: open training, three-step Ascension trial, two-step offline raid, instant forge.")

