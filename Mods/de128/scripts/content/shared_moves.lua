local sf2 = require("sf2")

-- Scoped differences from archived moves.xml, expressed through typed patches.
-- Guards describe the canonical source so incompatible bases fail explicitly.
sf2.moves.patch {
    move = "RangedHeavyPlayer",
    interval_end = { name = "Uninterrupt", expected = 42, value = 40 },
}
sf2.moves.patch {
    move = "ChakramFly",
    hit = { expected = "High", value = "MiddleShortPlus" },
}
sf2.moves.patch {
    move = "ShopRangedTryOnHeavyPlayer",
    sound_frame = { name = "snd_disk", expected = 18, value = 16 },
}
for _, move in ipairs({ "MassBombPlayer", "LightningArrowPlayer" }) do
    sf2.moves.patch {
        move = move,
        conditions = { { type = "mod_exists", name = "Stun", ["not"] = true } },
    }
end
