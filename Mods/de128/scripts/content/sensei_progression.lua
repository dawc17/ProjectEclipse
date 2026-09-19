local sf2 = require("sf2")
-- Pending progression decisions; not required by main.lua. The presentation/map
-- coordinator must mark an act opened only when its notification has completed.
-- Historical source: quests.xml SenseiZone1Notify .. SenseiZone6Notify.
local tournaments = {
    "core:fights/zone_1/tournament/3",
    "core:fights/zone_2/tournament/3",
    "core:fights/zone_3/tournament/3",
    "core:fights/zone_4/tournament/3",
    "core:fights/zone_5/tournament/3",
    "core:fights/zone_6/tournament/3",
}

local function find_unlocks(event, opened, finals)
    local eligible = {}
    -- The original checks saved prerequisites after ANY victory, not only a
    -- victory in the prerequisite fight. Loss/surrender must not notify.
    if event.outcome ~= "win" then return eligible end
    for act, tournament in ipairs(tournaments) do
        if not opened[act] and sf2.profile.fight(tournament).wins >= 1 then
            if act == 1 or sf2.profile.fight(finals[act - 1]).wins >= 1 then
                eligible[#eligible + 1] = act
            end
        end
    end
    return eligible
end

return { find_unlocks = find_unlocks }
