local sf2 = require("sf2")
-- SYNTHESIZED, not recovered. Archive stages.xml references Guard_Girl and
-- Guard_Man in all 22 Sensei guard rows but defines neither; vanilla, the DE
-- 1.0.6 export and the owner's asset drop lack them too. (Natively a missing
-- template falls back to blank parameters without Default's skeleton, fists,
-- attributes, perks or alignment.) Every comparable story-character template
-- (Man_Haunted_Prince, Boss_Lynx_Young, Boss_Wasp_Young, Boss_Widow_Young, ...)
-- is Template="Default" plus a Voice, with its own equipment; the guard rows
-- already carry their equipment and names. Guard_Girl rows are all women and
-- Guard_Man rows all men. A warrior on Default with that voice is exactly what
-- such a template would produce, because native parsing layers template and
-- warrior attributes through the same function.
local function resolve()
    local default = sf2.warriors.get_template("core:warrior-templates/default")
    return {
        guard_girl = { template = default, voice = "Female" },
        guard_man = { template = default, voice = "Male" },
    }
end
return { resolve = resolve }
