local sf2 = require("sf2")

local prize = sf2.rewards.register {
    id = "monk_katars",
    items = {{ item = sf2.items.get("core:items/weapon/WEAPON_C2_Z2_MONK_KATAR") }},
}

-- Vanilla uses a separate battle for the Eclipse replay. This does not unlock it.
sf2.fights.patch {
    target = "core:fights/zone_1/boss_lynx_eclipsemode/1",
    reward_drops = {{ wins = 1, mode = "eclipse", reward = prize }},
}
