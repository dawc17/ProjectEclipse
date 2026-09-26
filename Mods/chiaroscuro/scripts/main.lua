-- Chiaroscuro: light, shadow and depth for Shadow Fight 2 fights.
-- Every effect is a switch under Options > Mod settings. The engine draws the
-- effects; this mod builds them from sf2.fx blocks where one exists and uses
-- sf2.visuals presets for the rest (depth, haze, rim light).
local sf2 = require("sf2")

local function toggle(id, label, description)
    return sf2.settings.toggle { id = id, label = label, description = description, default = true }
end

local depth = toggle("background_depth", "Background depth",
    "Far background layers move less, so the scene reads deeper.")
local trails = toggle("weapon_trails", "Weapon trails", "Short trails behind fast weapon swings.")
local haze = toggle("depth_haze", "Depth haze", "Distant layers fade toward the sky colour.")
local rim = toggle("rim_light", "Rim light", "A thin lit edge on each fighter; it turns to ink while casting magic.")
local shafts = toggle("light_shafts", "Light shafts", "Soft beams of light, with drifting dust, in lit interiors and forests.")
local knockout = toggle("knockout_fade", "Knockout fade", "The final hit slows time and drains the colour from everything but the blood.")
local film = toggle("film_look", "Film look", "Warm halation around highlights and a fine grain.")
local stains = toggle("stains", "Stains", "Hits leave blood on the floor until the round ends.")
local glow = toggle("weapon_light", "Weapon and magic light", "Fire and electric weapons and magic light the stage and the fighters near them.")
local dust = toggle("dust", "Dust", "Landings, knockdowns and skids kick up dust from the floor.")
local walls = toggle("wall_impacts", "Wall impacts", "Being driven into the arena wall throws off debris and jolts the picture.")

sf2.visuals.background_depth { strength = 0.6, setting = depth }

-- Blade trails in the fighter's own colour, in fights and menu previews.
sf2.fx.trail {
    id = "weapon_trails", setting = trails, weapon = true, scenes = "everywhere",
    lifetime = 0.11, min_speed = 900, full_speed = 2600, alpha = 0.55, start_alpha = 0.35,
}

sf2.visuals.depth_haze { strength = 0.4, setting = haze }

sf2.visuals.rim_light { offset = 2.5, alpha = 0.85, lighten = 0.35, ink = 0.85, ink_color = "#1A0C26", setting = rim }

-- Light shafts: three slanted beams from the upper left, on the nearest
-- background layer, where the art suggests a light source overhead.
local lit = { "dojo", "temple", "ruins", "forest", "grove", "cave", "statue", "village", "castle", "sakura", "waterfall", "gate" }
local unlit = { "night", "dark", "haloween", "hw", "moon", "spaceship", "neural", "vortex" }
for index, beam in ipairs {
    { x = -720, width = 190, angle = -16, alpha = 0.10 },
    { x = -120, width = 260, angle = -14, alpha = 0.08 },
    { x = 560, width = 170, angle = -18, alpha = 0.07 },
} do
    sf2.fx.overlay {
        id = "light_shaft_" .. index, setting = shafts, match = lit, exclude = unlit,
        placement = "background", depth = 0.05, shape = "shaft", blend = "additive", color = "#FFE3B8",
        x = beam.x, y = 70, width = beam.width, height = 760, angle = beam.angle, alpha = beam.alpha,
    }
end

-- Dust that catches the light, drifting through the middle of the stage.
sf2.fx.particles {
    id = "light_dust", setting = shafts, match = lit, exclude = unlit, placement = "front", blend = "additive",
    color = "#FFE9C4B0", count = 45, lifetime_min = 6, lifetime_max = 11, size_min = 1.2, size_max = 3.2,
    velocity_x_min = -6, velocity_x_max = 10, velocity_y_min = -4, velocity_y_max = 6, noise = 8,
    area_width = 0.6, area_height = 0.8, y = 60,
}

-- Knockout: a bright pop on the final hit, then everything but the blood turns
-- grey in slow motion and eases back, colour and speed returning together.
sf2.fx.screen { id = "knockout_pop", setting = knockout, trigger = "ko", brightness = 0.4, contrast = 1.25, duration = 0.3 }
sf2.fx.screen {
    id = "knockout_fade", setting = knockout, trigger = "ko", saturation = 0, contrast = 1.35, brightness = -0.05,
    vignette = 0.6, accent = "#B01010", accent_strength = 1, accent_width = 0.05,
    hold = 0.25, duration = 2.8, time_scale = 0.25,
}

sf2.fx.screen { id = "film_look", setting = film, halation = 0.35, halation_threshold = 0.7, halation_color = "#FFA070", grain = 0.22 }

-- Blood on the floor: a few drops per hit, a pool under the knockout.
sf2.fx.stain { id = "hit_stains", setting = stains, trigger = "hit", color = "#4A0606", alpha = 0.8,
    count = 2, size_min = 8, size_max = 20, spread = 30, limit = 50 }
sf2.fx.stain { id = "knockout_stain", setting = stains, trigger = "ko", color = "#3C0404", alpha = 0.9,
    count = 5, size_min = 20, size_max = 42, spread = 45, flatten = 0.3, limit = 10 }

-- Light from magic.
sf2.fx.light { id = "magic_light", setting = glow, source = "magic", color = "#C8B8FF",
    radius = 420, intensity = 1.2, glow = 0.35, glow_size = 460, flicker = 0.1 }

-- Dust from the floor: soft puffs that spread low along the ground, rise a little
-- and fade. A heavy fall throws up more, and a skid leaves a trail of small puffs.
local dust_color, dust_clear = "#C9B89A66", "#C9B89A00"
sf2.fx.particles { id = "landing_dust", setting = dust, placement = "contact", trigger = "land",
    color = dust_color, end_color = dust_clear, count = 12, radius = 16,
    lifetime_min = 0.45, lifetime_max = 0.9, size_min = 16, size_max = 34, speed_min = 10, speed_max = 40,
    velocity_x_min = -140, velocity_x_max = 140, velocity_y_min = 15, velocity_y_max = 60, noise = 15 }
sf2.fx.particles { id = "knockdown_dust", setting = dust, placement = "contact", trigger = "knockdown",
    color = dust_color, end_color = dust_clear, count = 22, radius = 30,
    lifetime_min = 0.6, lifetime_max = 1.2, size_min = 22, size_max = 48, speed_min = 20, speed_max = 60,
    velocity_x_min = -220, velocity_x_max = 220, velocity_y_min = 20, velocity_y_max = 90, noise = 20 }
sf2.fx.particles { id = "slide_dust", setting = dust, placement = "contact", trigger = "slide",
    color = dust_color, end_color = dust_clear, count = 3, radius = 8,
    lifetime_min = 0.35, lifetime_max = 0.6, size_min = 10, size_max = 22, speed_min = 5, speed_max = 20,
    velocity_x_min = -60, velocity_x_max = 60, velocity_y_min = 10, velocity_y_max = 40, noise = 10 }

-- Wall impacts: grit and chips knocked off the wall fall away, a dust cloud rolls
-- off it, and the picture darkens for a moment with the thud.
sf2.fx.particles { id = "wall_debris", setting = walls, placement = "contact", trigger = "wall",
    color = "#5E4E3EFF", end_color = "#5E4E3E00", count = 16, radius = 20, spin = 1,
    lifetime_min = 0.5, lifetime_max = 0.9, size_min = 3, size_max = 8, speed_min = 200, speed_max = 480, gravity = 900 }
sf2.fx.particles { id = "wall_dust", setting = walls, placement = "contact", trigger = "wall",
    color = dust_color, end_color = dust_clear, count = 10, radius = 26,
    lifetime_min = 0.6, lifetime_max = 1.1, size_min = 22, size_max = 44, speed_min = 20, speed_max = 70,
    velocity_y_min = 10, velocity_y_max = 50, noise = 18 }
sf2.fx.screen { id = "wall_thud", setting = walls, trigger = "wall",
    brightness = -0.08, contrast = 1.12, vignette = 0.35, duration = 0.28 }
