local sf2 = require("sf2")

-- Restore the archived RaidCharge cast and its hidden power-field actor.
-- Both animations are byte-identical copies of the packaged native data.

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
    direction = { from = { wall = "Back", player = "Parent" }, to = { wall = "Front", player = "Parent" } },
    align = { axes = { "X", "Y", "Z" }, pivot = { node = "Magic-Node3_1", player = "Me" },
        position = { node = "NNeck", player = "Parent", x = -80, y = 60 } },
    events = { "birth" },
    conditions = { { actor = "AbilityPowerField" } },
    intervals = { { type = "Attack", from = 2, to = 5, attack = {
        edges = { "Magic-Edge1_1", "Magic-Edge2_1", "Magic-Edge4_1", "Magic-Edge6_1" },
        damage = 0.3,
        damage_terms = { MagicDamage = 0, UnarmedDamage = -25 },
        impulse = { x = 175, y = 350 }, hit = "ElectrocutionPowerfield",
        options = { no_effect = true, no_critical = true, body_part = "Body",
            defense_types = { "BodyDefense" }, ignores_block = true,
            ignores_invulnerable = { "Evade", "Recovery", "Dash" } },
    } } },
    timeline = {
        [1] = { { effect = {
            name = "PowerFieldEffect", core_sequence = "effect_electricity",
            scale = 1.5, time_scale = 2, on_background = true,
            attach = { player = "Parent", root_point = "NStomach", attach_point = "NChest",
                offset_x = 15, offset_y = -15 },
        } }, { effect = {
            name = "PowerFieldEffect2", core_sequence = "effect_electricity",
            scale = 1.5, time_scale = 2, on_background = true,
            attach = { player = "Parent", root_point = "NStomach", attach_point = "NChest",
                offset_x = 15, offset_y = -15, start_rotation = 330 },
        } } },
        animation_end = { delete_actor = "Me" },
    },
    locks = {
        { item = "Weapon", subtype = "MagicFireAura" },
        { item = "Skeleton", subtype = "SkeletonMagic" },
    },
}

local cast = sf2.moves.register {
    id = "gatekeeper_power_field",
    animation = sf2.assets.binary("animations/ability_gatekeeper_power_field"),
    core_templates = { "1key", "BossAbility", "Controlled", "SoundStrike" },
    mid_frames = 2, no_wall_repulsion = true, first_frame = 1,
    priority = 1010, mirror_node = "NHeel_1",
    tactic_conditions = { { distance = "X", max = 350, from = { pivot = "Me" }, to = { node = "NPivot", player = "Enemy" } } },
    align = { axes = { "X", "Z" }, pivot = { node = "NHeel_1" },
        position = { pivot = "Me" } },
    conditions = {
        { keys = { "RaidCharge" } },
        { not_mod = "GateKeeperPowerFieldCD" },
        { not_interval = "SemiUninterrupt" },
        { not_interval = "Uninterrupt" },
        { stage = "Fight" },
        { not_all = {
            { animation = "$Move" },
            { interval = "SemiUninterrupt" },
        } },
        { not_animation = "Physical" },
    },
    locks = { { perk = field_perk } },
    intervals = { { name = "Uninterrupt", to = 26 }, { type = "Block", from = 26 } },
    timeline = {
        [13] = { effect = {
            name = "ElectroEffect", core_sequence = "mgc_effect_shocker",
            scale = 0.75, time_scale = 2, on_background = true,
            attach = { player = "Me", root_point = "MacroBodyGatekeeper-Node975",
                attach_point = "MacroBodyGatekeeper-Node445",
                offset_x = -54, offset_y = -12, start_rotation = -78 },
        } },
        [20] = { projectile = {
            name = "AbilityPowerField", core_skeleton = "SkeletonMagic",
            item = aura_item, start_move = surge,
        } },
        [8] = { sound = "snd_energy_burst" },
    },
    events = { "key_pressed", { interval_end = "Uninterrupt" }, "animation_end" },
    direction = { from = { node = "NPivot", player = "Me" }, to = { node = "NPivot", player = "Enemy" } },
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
