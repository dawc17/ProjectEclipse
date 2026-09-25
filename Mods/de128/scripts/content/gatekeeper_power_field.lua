local sf2 = require("sf2")

-- Restore the archived RaidCharge cast and its hidden power-field actor.
-- Both animations are byte-identical copies of the packaged native data.
local function point(object, part, player, x, y)
    return { object = object, part = part, player = player, shift_x = x, shift_y = y }
end

local aura_item = sf2.items.get("core:items/magic/MAGIC_FIRE_AURA")
local field_perk = sf2.perks.get("core:perks/PERK_POWER_FIELD")

sf2.moves.patch { move = "GateKeeperPowerField", disable = true }
sf2.moves.patch { move = "AbilityGateKeeperPowerField", disable = true }

local surge = sf2.moves.register {
    id = "gatekeeper_power_surge",
    animation = sf2.assets.binary("animations/ability_power_surge"),
    core_templates = { "MagicFireAura", "MagicMissileFly", "MagicMissile" },
    mid_frames = 1, first_frame = 1, priority = 500,
    no_wall_repulsion = true, no_interpolation_frames = true, no_magic_recharge = true,
    direction = { from = point("Wall", "Back", "Parent"), to = point("Wall", "Front", "Parent") },
    align = { axes = { "X", "Y", "Z" }, pivot = point("Nodes", "Magic-Node3_1", "Me"),
        position = point("Nodes", "NNeck", "Parent", -80, 60) },
    events = { "birth" },
    conditions = { { type = "actor_name", name = "AbilityPowerField" } },
    intervals = { { type = "Attack", start = 2, ["end"] = 5, attack = {
        edges = { "Magic-Edge1_1", "Magic-Edge2_1", "Magic-Edge4_1", "Magic-Edge6_1" },
        damage = 0.3,
        damage_terms = { { type = "MagicDamage" }, { type = "UnarmedDamage", shift = -25 } },
        impulse = { x = 175, y = 350 }, hit = "ElectrocutionPowerfield",
        options = { no_effect = true, no_critical = true, body_part = "Body",
            defense_types = { "BodyDefense" }, ignores_block = true,
            ignores_invulnerable = { "Evade", "Recovery", "Dash" } },
    } } },
    actions = {
        { type = "effect", frame = 1, effect = {
            name = "PowerFieldEffect", core_sequence = "effect_electricity",
            scale = 1.5, time_scale = 2, on_background = true,
            attach = { player = "Parent", root_point = "NStomach", attach_point = "NChest",
                offset_x = 15, offset_y = -15 },
        } },
        { type = "effect", frame = 1, effect = {
            name = "PowerFieldEffect2", core_sequence = "effect_electricity",
            scale = 1.5, time_scale = 2, on_background = true,
            attach = { player = "Parent", root_point = "NStomach", attach_point = "NChest",
                offset_x = 15, offset_y = -15, start_rotation = 330 },
        } },
        { type = "delete_actor", event = "AnimationEnd", player = "Me" },
    },
    locks = {
        { type = "item", item_type = "Weapon", item_subtype = "MagicFireAura" },
        { type = "item", item_type = "Skeleton", item_subtype = "SkeletonMagic" },
    },
}

local cast = sf2.moves.register {
    id = "gatekeeper_power_field",
    animation = sf2.assets.binary("animations/ability_gatekeeper_power_field"),
    core_templates = { "1key", "BossAbility", "Controlled", "SoundStrike" },
    mid_frames = 2, no_wall_repulsion = true, first_frame = 1,
    priority = 1010, mirror_node = "NHeel_1",
    tactic_conditions = { { type = "distance", axis = "X", maximum = 350,
        from = point("Pivot", nil, "Me"), to = point("Nodes", "NPivot", "Enemy") } },
    align = { axes = { "X", "Z" }, pivot = point("Nodes", "NHeel_1"),
        position = point("Pivot", nil, "Me") },
    conditions = {
        { type = "keys", keys = { { key = "RaidCharge" } } },
        { type = "mod_exists", name = "GateKeeperPowerFieldCD", ["not"] = true },
        { type = "current_interval", name = "SemiUninterrupt", ["not"] = true },
        { type = "current_interval", name = "Uninterrupt", ["not"] = true },
        { type = "round_stage", name = "Fight" },
        { type = "all", ["not"] = true, conditions = {
            { type = "current_animation", name = "$Move" },
            { type = "current_interval", name = "SemiUninterrupt" },
        } },
        { type = "current_animation", name = "Physical", ["not"] = true },
    },
    locks = { { type = "perk", perk = field_perk } },
    intervals = { { name = "Uninterrupt", ["end"] = 26 }, { type = "Block", start = 26 } },
    actions = {
        { type = "effect", frame = 13, effect = {
            name = "ElectroEffect", core_sequence = "mgc_effect_shocker",
            scale = 0.75, time_scale = 2, on_background = true,
            attach = { player = "Me", root_point = "MacroBodyGatekeeper-Node975",
                attach_point = "MacroBodyGatekeeper-Node445",
                offset_x = -54, offset_y = -12, start_rotation = -78 },
        } },
        { type = "create_projectile", frame = 20, projectile = {
            name = "AbilityPowerField", core_skeleton = "SkeletonMagic",
            item = aura_item, start_move = surge,
        } },
        { type = "random_sound", frame = 8, core_sounds = { "snd_energy_burst" } },
    },
    events = { "key_pressed", { type = "interval_end", name = "Uninterrupt" }, "animation_end" },
    direction = { from = point("Nodes", "NPivot", "Me"), to = point("Nodes", "NPivot", "Enemy") },
}

local tactic = sf2.tactics.register {
    id = "gatekeeper_power_field", template = "Aggressive",
    on_decide = function(memory, event)
        if event.seconds < (memory.next_field or 0) then return nil end
        for _, action in ipairs(event.actions) do
            if action.name == "de128:moves/gatekeeper_power_field" then
                memory.next_field = event.seconds + 6
                return action
            end
        end
        return nil
    end,
}

return { cast = cast, surge = surge, tactic = sf2.tactics.name(tactic) }
