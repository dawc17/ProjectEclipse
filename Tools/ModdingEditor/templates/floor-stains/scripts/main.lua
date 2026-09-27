local sf2 = require("sf2")
local enabled = sf2.settings.toggle {
    id = "stains", label = "Floor stains", default = true,
    description = "Directional droplets that land and accumulate during a round.",
}
sf2.fx.stain {
    id = "blood", setting = enabled, trigger = "hit", color = "#4A0606",
    count = 3, size_min = 8, size_max = 20, spread = 30, flatten = 0.35,
    speed_min = 90, speed_max = 210, gravity = 900, lift = 90,
    merge_radius = 16, max_pool_size = 70, limit = 50,
}
