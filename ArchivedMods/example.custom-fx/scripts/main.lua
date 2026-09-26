-- Custom FX Showcase: builds new looks from sf2.fx building blocks instead of
-- the sf2.visuals presets. Each effect has its own Options > Mod settings switch.
local sf2 = require("sf2")

local function switch(id, label)
    return sf2.settings.toggle { id = id, label = label, default = true }
end

-- Glowing sparks streaming off the main-hand weapon tip, for both fighters.
sf2.fx.particles {
    id = "blade_sparks", setting = switch("blade_sparks", "Blade sparks"),
    placement = "node", node = "Weapon-Node2_1", fighters = "both", scenes = "everywhere",
    blend = "additive", color = "#FFC060", end_color = "#FF400000",
    count = 40, lifetime_min = 0.3, lifetime_max = 0.7, size_min = 2, size_max = 5,
    velocity_y_min = 10, velocity_y_max = 60, noise = 30, radius = 6,
}

-- A cool blue trail on the player's lower leg for kicks.
sf2.fx.trail {
    id = "kick_trail", setting = switch("kick_trail", "Kick trails"),
    nodes = { "NKnee_2", "NHeel_2" }, fighters = "player",
    color = "#6FB8FF", blend = "additive", lifetime = 0.15, alpha = 0.6, min_speed = 700,
}

-- Low fog drifting just behind the fighters.
sf2.fx.particles {
    id = "ground_fog", setting = switch("ground_fog", "Ground fog"),
    placement = "behind", color = "#FFFFFF30",
    count = 60, lifetime_min = 10, lifetime_max = 16, size_min = 80, size_max = 160,
    velocity_x_min = -12, velocity_x_max = 12, velocity_y_min = -2, velocity_y_max = 2,
    noise = 4, area_height = 0.35,
}

-- A warm light wash over the farthest background layers in the dojo.
sf2.fx.overlay {
    id = "dojo_glow", setting = switch("dojo_glow", "Dojo glow"),
    match = { "dojo" }, placement = "background", depth = 0.8,
    color = "#FFB060", blend = "additive", alpha = 0.18,
}

-- A slightly cooler, higher-contrast grade with a soft vignette.
sf2.fx.screen {
    id = "cinema_grade", setting = switch("cinema_grade", "Cinema grade"),
    saturation = 0.9, contrast = 1.08, brightness = -0.02,
    tint = "#E8F0FF", tint_strength = 0.3, vignette = 0.35,
}
