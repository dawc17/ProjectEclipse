local sf2 = require("sf2")

-- The archived battle-level music applies to every fight in these four
-- existing campaign battles. Patch each fight so its native XML projection
-- keeps the base battle and campaign progression intact.
local tracks = {
    ninja_in_the_night_old = sf2.assets.audio("audio/campaign/ninja_in_the_night_old"),
    old_sensei_old = sf2.assets.audio("audio/campaign/old_sensei_old"),
    deadly_smoke_old = sf2.assets.audio("audio/campaign/deadly_smoke_old"),
    burning_town_old = sf2.assets.audio("audio/campaign/burning_town_old"),
}

local battles = {
    { zone = "zone_1", battle = "tournament_intermission", fights = 8, music = tracks.ninja_in_the_night_old },
    { zone = "zone_2", battle = "boss_hermit_intermission", fights = 1, music = tracks.old_sensei_old },
    { zone = "zone_3", battle = "tournament_intermission", fights = 8, music = tracks.deadly_smoke_old },
    { zone = "zone_6", battle = "questbattle", fights = 1, music = tracks.burning_town_old },
}

for _, battle in ipairs(battles) do
    for fight = 1, battle.fights do
        sf2.fights.patch {
            target = "core:fights/" .. battle.zone .. "/" .. battle.battle .. "/" .. fight,
            music = battle.music,
        }
    end
end
