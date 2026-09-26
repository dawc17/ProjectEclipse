local sf2 = require("sf2")
-- Definitive Edition's additions to the default (unarmed) moveset, restored
-- from the archived moves.xml (Assets/DExml, identical in the DE128 drop):
--   FrontJumpScissorsKick  Up-Forward + Kick, Kick
--   AxeKickOld             Forward + Kick, Kick
--   WallRunUp              Back + Kick, Kick near the back wall (or after BackKick)
--   AirPunch               Down-Forward + Punch
--   ThrowLegPush           Back + Kick (or Forward + Punch facing a wall) in throw range
-- plus the throw's victim, stand-up and profile-preview moves. All animations
-- ship with the core game. No vanilla move is replaced or rebound.
--
-- The double-kick inputs start FrontKick or BackKick on the first Kick; the
-- second Kick cancels that starter during its early cancel window (patched
-- at the end of this file).
-- tactic_equivalent lets the AI use each move wherever its tables use the
-- named native move of similar timing and range; the tables have no rows of
-- their own for these moves (nor did the Definitive Edition's).
local function name(id) return "de128:moves/" .. id end
-- Profile titles copied from the archive by Tools/GenerateDE128MoveNames.py.
local function title(archived) return sf2.localization.key("moves." .. archived) end

local moves = {}
local function register(id, value)
    value.id = id
    moves[id] = sf2.moves.register(value)
    return moves[id]
end

local hit_sounds = { "snd_hit1", "snd_hit2", "snd_hit3", "snd_hit4", "snd_hit5", "snd_hit6" }
local function voices(sound)
    return { { play_sound = "snd_m_pl_" .. sound, voice = "Male" }, { play_sound = "snd_low_pl_" .. sound, voice = "MaleLow" },
        { play_sound = "snd_f_pl_" .. sound, voice = "Female" } }
end
local leg_2 = { "EThigh_2", "ECalf_2", "EInstep_2", "EToe_2", "EFoot_2" }
local leg_1 = { "EThigh_1", "ECalf_1", "EInstep_1", "EToe_1", "EFoot_1" }
local skeleton = { item = "Skeleton", subtype = "Skeleton" }
local not_gatekeeper = { any = { { not_item = "Armor", name = "BODY_GATEKEEPER" } } }
local unarmed_locks = { skeleton, not_gatekeeper }
local heel_align = { axes = { "X", "Z" }, pivot = { node = "NHeel_1" }, position = { pivot = "Me" } }
local near_enemy = { distance = "X", min = 50, max = 350, from = { pivot = "Me" }, to = { node = "NPivot", player = "Enemy" } }

-- The shared tail of every controlled attack: not mid-move, fight stage, not Titan.
local function controlled(direction, two_key)
    local state = { any = two_key and { { animation = "1key" }, { animation = "2key" }, { not_interval = "SemiUninterrupt" } }
        or { { animation = "1key" }, { not_interval = "SemiUninterrupt" } } }
    return { state, { any = { { animation = direction }, { not_interval = "SemiUninterrupt" } } },
        { not_interval = "Uninterrupt" }, { stage = "Fight" },
        { not_all = { { animation = "$Move" }, { interval = "SemiUninterrupt" } } },
        { not_animation = "Physical" }, { not_mod = "MOD_TITAN" } }
end

register("front_jump_scissors_kick", {
    core_templates = { "3key", "UpForward", "Unarmed", "Kick", "Jump", "Controlled", "NotTitan", "SoundStrike" },
    animation = sf2.assets.binary("animations/front_jump_scissors_kick"), type = "ATTACK",
    mid_frames = 2, first_frame = 1, priority = 130, style_factor = 1.1, mirror_node = "NHeel_1",
    tactic_equivalent = "FrontJumpKick",
    profile = { rank = 209, core_icon = "Trick2.double_jump_kick", display_name = title("FrontJumpScissorsKick") },
    tactic_distance = near_enemy, align = heel_align,
    transitions = { { frame_shift = 1, conditions = { { animation = "FrontJumpKick" }, { interval = "SemiUninterrupt" } } } },
    conditions = { { keys = { "Kick", "Kick", { "Up-Forward", press = "Hold" } } }, controlled("UpForward", true) },
    locks = unarmed_locks,
    intervals = { { name = "Unstable", from = 8, to = 21 }, { name = "Uninterrupt", to = 26 }, { type = "Block", from = 27 },
        { name = "Throwable", from = 27 },
        { type = "Attack", from = 9, to = 11, attack = { edges = leg_2, damage = 0.08, impulse = { x = 525, y = -175 }, hit = "HighShort" } },
        { type = "Attack", from = 14, to = 15, attack = { edges = leg_1, damage = 0.07, impulse = { x = 525, y = -175 }, hit = "HighPlus" } } },
    timeline = { [9] = { sound = "snd_swish3" }, [14] = { sound = "snd_swish3" }, strike = { sound = hit_sounds } },
    events = "controlled", direction = "face_enemy",
})

register("axe_kick_old", {
    core_templates = { "3key", "Forward", "Unarmed", "Kick", "Controlled", "NotTitan", "SoundStrike" },
    animation = sf2.assets.binary("animations/axe_kick_old"), type = "ATTACK",
    mid_frames = 2, first_frame = 1, priority = 135, style_factor = 1.1, mirror_node = "NHeel_1",
    tactic_equivalent = "FrontKick",
    profile = { rank = 211, core_icon = "Trick3.axe_kick", display_name = title("AxeKickOld") },
    tactic_distance = near_enemy, align = heel_align,
    -- Not in the archive: continue FrontKick's frame (native frame + 1 + shift) so
    -- the double tap reads as one axe kick, as DoublePunch does from HeavyPunch.
    transitions = { { frame_shift = -1, conditions = { { animation = "FrontKick" }, { interval = "SemiUninterrupt" } } } },
    conditions = { { keys = { "Kick", "Kick", { "Forward", press = "Hold" } } }, controlled("Forward", true) },
    locks = unarmed_locks,
    intervals = { { name = "Unstable", from = 5, to = 11 }, { name = "SemiUninterrupt", to = 5 }, { name = "Uninterrupt", to = 17 },
        { type = "Block", from = 18 }, { name = "Throwable", from = 18 },
        { type = "Attack", from = 7, to = 10, attack = { id = 431, edges = leg_2, damage = 0.09, impulse = { x = 245, y = 245 }, hit = "Overhead" } } },
    timeline = { [6] = voices("attack1"), [8] = { sound = "snd_swish3" }, strike = { sound = hit_sounds } },
    events = "controlled", direction = "face_enemy",
})

register("wall_run_up", {
    core_templates = { "2key", "Back", "Wall", "Jump", "Unarmed", "Kick", "Controlled", "NotTitan", "SoundStrike" },
    animation = sf2.assets.binary("animations/wall_run_up"), type = "ATTACK",
    mid_frames = 2, first_frame = 3, priority = 150, mirror_node = "NHeel_1",
    tactic_equivalent = "DoubleJumpKick",
    profile = { rank = 511, core_icon = "Trick5.high_punch", keys_description = "Wall_Keys", display_name = title("WallRunUp") },
    align = { axes = { "X" }, pivot = { animation = true }, position = { wall = "Back", player = "Me" } },
    transitions = { { frame_shift = 1, conditions = { { animation = "BackKick" }, { interval = "SemiUninterrupt" } } } },
    conditions = {
        { any = { { all = { { keys = { { "Back", press = "Hold" }, "Kick", "Kick" } } } },
            { all = { { animation = "BackKick" }, { interval = "SemiUninterrupt" } } } } },
        { distance = "X", min = 150, max = 350, from = { wall = "Back", player = "Me" }, to = { node = "NHeel_1", player = "Me" } },
        controlled("Back", true) },
    intervals = { { name = "Unstable", from = 3, to = 35 }, { name = "Uninterrupt", from = 2, to = 39 }, { type = "Block", from = 40 },
        { type = "Attack", from = 28, to = 34, attack = { edges = { leg_1[1], leg_1[2], leg_1[3], leg_1[4], leg_1[5],
            leg_2[1], leg_2[2], leg_2[3], leg_2[4], leg_2[5] }, damage = 0.30, impulse = { x = 175, y = 245 }, hit = "Overhead" } } },
    timeline = { [4] = voices("jump1"), [5] = { sound = "snd_swish7" }, [8] = { sound = "snd_swish5" },
        [13] = { sound = "snd_swish7" }, strike = { sound = hit_sounds } },
    events = "controlled", locks = unarmed_locks, direction = "face_enemy",
})

register("air_punch", {
    core_templates = { "2key", "DownForward", "Unarmed", "Jump", "Punch", "Controlled", "Arms", "NotTitan", "SoundStrike" },
    animation = sf2.assets.binary("animations/air_punch"), type = "ATTACK",
    mid_frames = 2, first_frame = 3, priority = 125, tactic_weapon = "Fists", mirror_node = "NHeel_1",
    tactic_equivalent = "TwoFootJumpKick",
    profile = { rank = 507, core_icon = "Trick5.high_punch", display_name = title("AirPunch") },
    tactic_distance = { distance = "X", min = 300, max = 600, from = { pivot = "Me" }, to = { node = "NPivot", player = "Enemy" } },
    align = { axes = { "X", "Z" }, pivot = { node = "NHeel_2" }, position = { pivot = "Me" } },
    conditions = { { keys = { "Punch", { "Down-Forward", press = "Hold" } } }, controlled("DownForward", false) },
    locks = { skeleton },
    intervals = { { name = "Unstable", from = 9, to = 22 }, { name = "Uninterrupt", to = 27 }, { type = "Block", from = 28 },
        { name = "Throwable", from = 28 },
        { type = "Attack", from = 13, to = 17, attack = { edges = { "EForearm_2", "EHand_2", "EFingers_2" }, damage = 0.16,
            impulse = { x = 525 }, hit = "HighHeavy" } } },
    timeline = { [8] = voices("attack5"), [13] = { sound = "snd_swish3" }, strike = { sound = hit_sounds } },
    events = "controlled", direction = "face_enemy",
})

-- Leg push throw: the victim move is registered first so the throw can name it.
register("throw_leg_push_v", {
    core_templates = { "NotTitan" }, animation = sf2.assets.binary("animations/throw_leg_push_v"),
    mid_frames = 2, first_frame = 7, priority = 9999, no_wall_repulsion = true, mirror_node = "NHeel_1",
    direction = { from = { wall = "Back", player = "Enemy" }, to = { wall = "Front", player = "Enemy" } },
    align = { axes = { "X", "Z" }, pivot = { animation = true }, position = { animation = "Enemy" } },
    events = { { animation_start = name("throw_leg_push"), player = "Enemy" } },
    timeline = { [9] = { sound = "snd_bodyfall1" } },
    conditions = { { not_actor = "Assistant" }, { any = { { not_animation = "Throw" }, { player_number = 2 } } },
        { not_mod = "MOD_TITAN" }, { not_animation = name("throw_leg_push_v") } },
    locks = unarmed_locks, intervals = { { name = "Uninterrupt" } },
})

register("throw_leg_push", {
    core_templates = { "2key", "Throw", "Back", "Unarmed", "ChangeDirection", "Controlled", "NotTitan", "SoundStrike" },
    animation = sf2.assets.binary("animations/throw_leg_push_a"), type = "ATTACK",
    mid_frames = 2, first_frame = 3, priority = 220, no_wall_repulsion = true, mirror_node = "NHeel_1", align = heel_align,
    conditions = {
        { direction = "Enemy", from = { node = "NPivot", player = "Enemy" }, to = { node = "NPivot", player = "Me" } },
        { distance = "X", max = 100, from = { pivot = "Me" }, to = { node = "NPivot", player = "Enemy" } },
        { any = {
            { all = { { keys = { "Kick", { "Back", press = "Hold" } } },
                { not_distance = "X", max = 200, from = { wall = "Back", player = "Me" }, to = { node = "NHeel_2", player = "Me" } } } },
            { all = { { keys = { "Punch", { "Forward", press = "Hold" } } },
                { distance = "X", max = 300, from = { node = "NHeel_1", player = "Me" }, to = { wall = "Front", player = "Me" } } } } } },
        { any = { { animation = "1key" }, { not_interval = "SemiUninterrupt" } } },
        { not_mod = "NoThrows" }, { not_mod = "CurseBomb", player = "Enemy" }, { interval = "Throwable", player = "Enemy" },
        { any = { { animation = "Back" }, { not_interval = "SemiUninterrupt" } } },
        { not_interval = "Uninterrupt" }, { stage = "Fight" },
        { not_all = { { animation = "$Move" }, { interval = "SemiUninterrupt" } } },
        { not_animation = "Physical" }, { not_mod = "MOD_TITAN" },
        { all = { { not_mod = "ThrowFallBlock", player = "Enemy" }, { not_mod = "ThrowBlock" } } },
        { not_mod = "MOD_TITAN", player = "Enemy" },
        { not_any = { { actor = "Assistant", player = "Enemy" }, { actor = "Assistant", player = "EnemyChild" } } },
        { any = { { not_animation = "Throw", player = "Enemy" }, { player_number = 2, player = "Enemy" } } },
        { not_mod = "Stun" } },
    intervals = { { name = "Uninterrupt", to = 48 }, { type = "Block", from = 49 },
        { type = "Attack", from = 24, to = 24, attack = { id = 428, direct = true, damage = 0.35, hit = "NoReaction",
            options = { defense_types = { "BodyDefense" } } } } },
    timeline = { [12] = voices("jump2"), [20] = { sound = "snd_swish5" }, strike = { sound = hit_sounds } },
    locks = { { screen = "Fight" }, skeleton }, events = "controlled", direction = "face_enemy",
})

-- The thrown fighter gets up with this move after the leg push lands.
register("standup_after_leg_fall", {
    core_templates = { "GetUp", "AfterThrowFall" }, animation = sf2.assets.binary("animations/throw_leg_push_v_standup"),
    mid_frames = 2, first_frame = 30, priority = 405, mirror_node = "NHeel_1",
    direction = { from = { node = "NNeck", player = "Me" }, to = { node = "NPivot", player = "Me" } },
    align = { axes = { "X", "Z" }, pivot = { node = "NPivot" }, position = { pivot = "Me" } },
    conditions = { { not_round_result = "Defeat" } },
    intervals = { { name = "Uninterrupt", to = 40 }, { type = "Block", from = 41 }, { type = "Invulnerable", name = "Recovery", to = 40 } },
    -- The archive matches the victim through its "LegFall" template tag; naming the move is equivalent.
    events = { { animation_end = name("throw_leg_push_v") } },
})

-- Profile screen preview: the throw performed on a spawned practice fighter.
register("throw_leg_push_v_profile", {
    animation = sf2.assets.binary("animations/throw_leg_push_v"),
    mid_frames = 2, first_frame = 3, priority = 555, no_wall_repulsion = true, no_interpolation_frames = true,
    direction = { from = { wall = "Front", player = "Parent" }, to = { wall = "Back", player = "Parent" } },
    align = { axes = { "X", "Z" }, shift_model_node = "NPivot", pivot = { animation = true }, position = { animation = "Parent" } },
    events = { "birth" }, conditions = { { animation = name("throw_leg_push_profile"), player = "Parent" } },
    timeline = { [26] = { sound = "snd_bodyfall1" }, [46] = { delete_actor = "Me" } },
})

register("throw_leg_push_profile", {
    core_templates = { "ThrowProfile" }, animation = sf2.assets.binary("animations/throw_leg_push_a"),
    mid_frames = 2, first_frame = 3, priority = 225, no_wall_repulsion = true, no_interpolation_frames = true,
    profile = { rank = 404, core_icon = "Trick9.throw_suplex", keys_description = "Throw_Keys", display_name = title("ThrowLegPushProfile") },
    align = heel_align,
    conditions = { { keys = { "Kick", { "Back", press = "Hold" } } } },
    intervals = { { type = "Attack", attack = { id = 428, direct = true, damage = 0.35 } } },
    timeline = { [3] = { create_player = { { "Skeleton", "Skeleton" }, { "Armor", "Body" }, { "Weapon", "Fists" }, { "Helm", "HEAD_KENJI" } } },
        [12] = voices("jump2"), [17] = { sound = "snd_swish5" } },
    locks = { { screen = "Profile" } },
})

-- Starter cancel windows. The second Kick only reaches Axe Kick or Wall Run
-- while the starter is outside Uninterrupt and inside SemiUninterrupt.
-- Vanilla FrontKick is Uninterrupt from frame 0 (the Definitive Edition gave
-- it 0-4); the owner chose a tighter 0-2 SemiUninterrupt after playtesting
-- 0-7 and 0-4. BackKick's 0-4 window is widened to 0-6 for a quick tap.
sf2.moves.patch {
    move = "FrontKick",
    add_interval = { name = "SemiUninterrupt", start = 0, ["end"] = 2 },
    interval_start = { name = "Uninterrupt", expected = 0, value = 3 },
}
sf2.moves.patch {
    move = "BackKick",
    interval_end = { name = "SemiUninterrupt", expected = 4, value = 6 },
    interval_start = { name = "Uninterrupt", expected = 5, value = 7 },
}

return moves
