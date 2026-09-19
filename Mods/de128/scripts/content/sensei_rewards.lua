local sf2 = require("sf2")
-- Pending: main.lua does not load this module until the complete story is ready.
-- Historical source: Assets/DExml/stages.xml, six Sensei normal/eclipse battles.
-- Reconcile with SOURCE_CORPUS.md before claiming current-corpus parity.
local acts = {
    { base = 1, normal = { 8, 8, 11 }, eclipse = 4 },
    { base = 5, normal = { 18, 18, 21 }, eclipse = 6 },
    { base = 100, normal = { 27, 27, 31 }, eclipse = 8 },
    { base = 100, normal = { 33, 33, 36 }, eclipse = 10 },
    { base = 500, normal = { 39, 39, 43 }, eclipse = 12 },
    { base = 2000, normal = { 45, 48 }, eclipse = 14 },
}
local result = {}
for act, data in ipairs(acts) do
    local prefix = "sensei_act_" .. act .. "_"
    local normal = {}
    for fight, gems in ipairs(data.normal) do
        normal[fight] = {
            sf2.rewards.register { id = prefix .. "normal_" .. fight .. "_0", prize_base = data.base },
            sf2.rewards.register { id = prefix .. "normal_" .. fight .. "_1", experience = 2, gems = gems, prize_base = data.base },
        }
    end
    local eclipse = {}
    -- Native slots count wins, including zero and partially completed gauntlets.
    for wins = 0, #data.normal do
        eclipse[wins + 1] = sf2.rewards.register {
            id = prefix .. "eclipse_" .. wins,
            gems = wins == #data.normal and data.eclipse or 0,
            prize_base = 1,
        }
    end
    result[act] = { normal = normal, eclipse = eclipse }
end
return result
