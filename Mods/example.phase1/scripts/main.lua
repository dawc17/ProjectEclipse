local sf2 = require("sf2")

-- P0.5 state. The callback below increments this only after a real save is bound.
sf2.state.register {
    version = 1,
    fields = {
        fight_begins = { type = sf2.state.INTEGER, required = true, default = 0 },
    },
}

-- Shared typed assets used by P1C and P1D. These are ordinary loose-mod assets.
local showcase_sprite = sf2.assets.sprite("sprites/showcase")
local showcase_audio = sf2.assets.audio("audio/showcase")
local showcase_animation = sf2.assets.binary("animations/showcase_step")

-- Localization definitions are consumed as opaque handles by typed content definitions.
local token_name = sf2.localization.key("showcase.token")
local set_title = sf2.localization.key("showcase.set.title")
local set_text = sf2.localization.key("showcase.set.text")
local set_brief = sf2.localization.key("showcase.set.brief")
local perk_name = sf2.localization.key("showcase.perk")
local perk_description = sf2.localization.key("showcase.perk.description")

-- P1D: additive locale metadata, a typed location, move/template/trigger content, and an ordinary tactic.
sf2.locales.register {
    id = "showcase_english",
    name = "eng_showcase",
    locale = "en",
    alias = "Phase1Showcase",
    file_icon = "usbr",
    file_icon_selected = "usbr_selected",
    loader_image = "Logo",
    preloader_image = "startLoading",
}

local showcase_location = sf2.locations.register {
    id = "showcase_arena",
    color = "0x1b2230",
    wall = 200,
    floor = 80,
    width = 1936,
    height = 512,
    min_width = 1936,
    music = showcase_audio,
    layers = {
        {
            type = 1,
            factor = 1,
            scaling = false,
            images = {
                { sprite = showcase_sprite, x = 0, y = 0, width = 256, height = 256 },
            },
        },
    },
}
local showcase_location_name = sf2.locations.name(showcase_location)

local forward_step_template = sf2.moves.register_template {
    id = "showcase_forward_step",
    core_templates = { "ForwardStep" },
}

local showcase_step = sf2.moves.register {
    id = "showcase_step",
    animation = showcase_animation,
    templates = { forward_step_template },
    core_templates = { "1key", "Step", "Forward", "Controlled", "SoundStrike" },
    type = "MOVE",
    priority = 10,
    mid_frames = 2,
    first_frame = 3,
    mirror_node = "NHeel_1",
}

sf2.moves.register_trigger {
    id = "showcase_step_sound",
    events = {
        { type = sf2.moves.ANIMATION_START, name = "example.phase1:moves/showcase_step" },
    },
    actions = {
        { type = sf2.moves.SOUND, audio = showcase_audio, volume = 0.25 },
    },
}

local showcase_tactic = sf2.tactics.register {
    id = "showcase_tactic",
    type = sf2.tactics.TABULAR,
    template = "Standard",
    memory = { strikes = 2, round_factor = 0.25 },
    safe_attack = { base = 1, distance_factor = 0.1 },
    animation_weights = {
        { move = showcase_step, value = { base = 1 } },
    },
}

-- P1C: a non-equipment item, availability overlay, item set, and forge family that borrows
-- the immutable host Simple economic profile instead of defining costs/currencies/timers.
local phase_token = sf2.items.register_consumable {
    id = "phase_token",
    display_name = token_name,
    icon = showcase_sprite,
    subtype = "PhaseToken",
    pack_label = "phase1",
    silent_receive = false,
    spend_after_use = false,
}

sf2.shop.set_availability {
    item = phase_token,
    visibility = sf2.shop.FORCE_VISIBLE,
}

sf2.itemsets.register {
    id = "phase_relics",
    title = set_title,
    text = set_text,
    brief = set_brief,
    members = {
        { item = phase_token, scale = 1 },
    },
}

local opening_focus_behavior = sf2.behaviors.register {
    id = "opening_focus_counter",
    parameters = {},
    on_fight_begin = function(parameters, fighter)
        local count = sf2.state.get("fight_begins") or 0
        sf2.state.set { fight_begins = count + 1 }
        sf2.log.info("Phase 1 showcase fight begin #" .. tostring(count + 1))
    end,
}

local opening_focus = sf2.perks.register {
    id = "opening_focus",
    behavior = opening_focus_behavior,
    kind = sf2.perks.SINGLE,
    display_name = perk_name,
    description = perk_description,
    icon = showcase_sprite,
    parameters = {},
}

local simple_profile = sf2.forge.profile("Simple")
sf2.forge.register_recipe {
    id = "showcase_simple",
    alias = "Phase1Showcase",
    economic_profile = simple_profile,
    items = {
        {
            equipment = sf2.forge.WEAPON,
            enchantments = 1,
            bar_scale = "1",
            min_deviation = 0,
            max_deviation = 0,
            random_aspect = false,
        },
    },
    candidates = {
        { perk = opening_focus, equipment = sf2.forge.WEAPON, min_level = 1, max_level = 52 },
    },
}

-- P1A: one complete Zone -> Battle -> Warrior/Rule/Reward -> Fight graph.
local showcase_zone = sf2.zones.register {
    id = "showcase_zone",
    file = "",
    start = false,
}

local showcase_battle = sf2.battles.register {
    id = "showcase_battle",
    zone = showcase_zone,
    type = sf2.battles.STORY,
    x = 0,
    y = 0,
    alias = "Phase1Showcase",
    title = "Phase 1 Showcase",
    description = "Integrated public API showcase",
    location = showcase_location_name,
}

local default_template = sf2.warriors.get_template("core:warrior-templates/default")
local showcase_warrior = sf2.warriors.register {
    id = "showcase_warrior",
    template = default_template,
    first_name = "Showcase",
    last_name = "Fighter",
    level = 1,
    tactic = showcase_tactic,
    perks = { opening_focus },
}

local recharge_rule = sf2.rules.recharge_magic_each_round {
    id = "showcase_recharge",
    target = sf2.rules.ALL,
    mode = sf2.rules.BOTH,
}

local token_reward = sf2.rewards.register {
    id = "showcase_reward",
    items = {
        { item = phase_token },
    },
}

local showcase_fight = sf2.fights.register {
    id = "showcase_fight",
    battle = showcase_battle,
    rounds = 1,
    round_time = 99,
    location = showcase_location_name,
    description = "Phase 1 showcase fight",
    warriors = { showcase_warrior },
    rules = { recharge_rule },
    rewards = { token_reward },
}

-- P1B: map/session discovery plus a fight-end condition that crosses back into the P1A graph
-- and grants the P1C item through a typed action.
sf2.quests.register {
    id = "showcase_intro",
    priority = 10,
    place = "map",
    events = { "session" },
    actions = {
        { type = "show_battle", battle = showcase_battle, locked = false },
        { type = "map_focus", battle = showcase_battle },
    },
}

sf2.quests.register {
    id = "showcase_completion",
    priority = 20,
    place = "fight",
    events = { "fight_end" },
    conditions = {
        {
            op = "eq",
            left = { kind = "event_fight" },
            right = { kind = "fight_id", fight = showcase_fight },
        },
    },
    actions = {
        {
            type = "dialog",
            title = "Phase 1 Showcase",
            lines = { "The integrated Phase 1 public API path completed." },
        },
        { type = "give_item", item = phase_token },
    },
}

sf2.log.info("registered integrated Phase 1 showcase")
