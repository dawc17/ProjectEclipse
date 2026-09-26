local sf2 = require("sf2")

-- Every core battle whose map placement the owner stages.xml changes from vanilla.
-- The archive moves Act 6's Duel clear of Sensei's Old Wounds entry.
local moves = {
    { target = "core:battles/zone_6/duel", x = -300, y = 100 },
    { target = "core:battles/zone_6/duel_intermission", x = -300, y = 100 },
    { target = "core:battles/zone_7/boss_titan_locked", x = -120, y = -80 },
}

for _, move in ipairs(moves) do
    sf2.battles.patch(move)
end
