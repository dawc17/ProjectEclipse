local sf2 = require("sf2")
-- Pending map effects for SenseiZone1Notify .. SenseiZone6Notify in historical
-- quests.xml. The coordinator must finish the dialog first; for Act I it must
-- also switch Eclipse mode off before invoking this sequence. Not an entrypoint.
local function open_act(act, battles)
    assert(type(act) == "number" and act % 1 == 0 and act >= 1 and act <= 6,
        "Sensei act must be an integer from 1 to 6")
    assert(type(battles) == "table" and #battles == 6, "Six Sensei battles required")
    if act == 1 then
        for index = 1, 6 do
            if not sf2.battles.reveal(battles[index], index ~= 1) then return false end
        end
    end
    if not sf2.battles.set_locked(battles[act], false) then return false end
    return sf2.battles.focus(battles[act])
end

-- The caller marks the opened flag only after true. A failed partial sequence
-- can be retried: reveal preserves existing progress and lock writes are idempotent.
return { open_act = open_act }
