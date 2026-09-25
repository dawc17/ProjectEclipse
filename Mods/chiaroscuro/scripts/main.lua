-- Chiaroscuro: light, shadow and depth for Shadow Fight 2 fights.
-- Every effect is a switch under Options > Mod settings. The engine draws the
-- effects; this mod only chooses them and tunes their numbers.
local sf2 = require("sf2")

local function toggle(id, label, description)
    return sf2.settings.toggle { id = id, label = label, description = description, default = true }
end

local depth = toggle("background_depth", "Background depth",
    "Far background layers move less, so the scene reads deeper.")
local trails = toggle("weapon_trails", "Weapon trails", "Short trails behind fast weapon swings.")
local haze = toggle("depth_haze", "Depth haze", "Distant layers fade toward the sky colour.")
local rim = toggle("rim_light", "Rim light", "A thin lit edge on each fighter.")
local bloom = toggle("bloom", "Bloom", "Bright effects and art glow.")
local particles = toggle("ambient_particles", "Ambient particles", "Dust, snow, embers or petals per location.")
local impact = toggle("impact", "Impact effects", "Radial blur and colour split on heavy hits.")

sf2.visuals.background_depth { strength = 0.6, setting = depth }

sf2.visuals.weapon_trails { lifetime = 0.11, min_speed = 900, full_speed = 2600, alpha = 0.55, setting = trails }

sf2.visuals.depth_haze { strength = 0.4, setting = haze }

sf2.visuals.rim_light { offset = 2.5, alpha = 0.85, lighten = 0.35, setting = rim }

sf2.visuals.bloom { threshold = 0.82, knee = 0.12, intensity = 0.7, setting = bloom }

-- Styles are picked from words in the location's name; everything else gets dust.
sf2.visuals.ambient_particles {
    density = 1,
    default_style = "dust",
    locations = {
        { match = { "ny", "newyear", "xmas", "christmas", "winter", "snow", "ice", "frost" }, style = "snow" },
        { match = { "hw", "haloween", "halloween", "volcano", "fire", "burn", "burning", "magma",
                    "lava", "hell", "inferno", "uw", "underworld" }, style = "embers" },
        { match = { "china", "chinese", "sakura", "spring", "cherry", "india", "indian" }, style = "petals" },
    },
    setting = particles,
}

-- Critical hits are also scaled by the accessibility "Critical hit shake" slider.
sf2.visuals.impact { critical = 1, head = 0.6, shock = 0.4, duration = 0.3, setting = impact }
