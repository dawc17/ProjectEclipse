local sf2 = require("sf2")
local registered = false
local result = {
    dialogue_pending = function() return false end, wake_notifications = function() end,
    defeat_pending = function() return false end, wake_defeat = function() end,
}
function result.register()
    if registered then return end
    local fields = {}
    for act = 1, 6 do
        for _, kind in ipairs { "opened", "pending", "dialogue_pending", "complete" } do
            fields["sensei_" .. kind .. "_" .. act] = { type = sf2.state.BOOLEAN, default = false }
        end
        fields["sensei_dialogue_next_" .. act] = { type = sf2.state.INTEGER, default = 1 }
    end
    for act = 1, 6 do
        for index = 1, (act == 6 and 2 or 3) do
            fields["sensei_entered_" .. act .. "_" .. index] = { type = sf2.state.BOOLEAN, default = false }
        end
    end
    fields.sensei_shogun_greeted = { type = sf2.state.BOOLEAN, default = false }
    fields.sensei_defeat_pending = { type = sf2.state.BOOLEAN, default = false }
    fields.sensei_defeat_rng = { type = sf2.state.INTEGER, default = 12859 }
    -- DE128 keeps one state schema (at most 64 fields); the Underworld story shares it.
    -- uw_intro: 0 not shown, 1 intro shown (followup pending), 2 complete. The other
    -- fields are sets of lower-case archive boss names, stored as ",boss_1,boss_2,"
    -- (see underworld_story.lua) so later data changes cannot reinterpret saves.
    fields.uw_intro = { type = sf2.state.INTEGER, default = 0 }
    for _, set in ipairs { "entered", "win_pending", "win_shown", "loss_pending", "loss_shown" } do
        fields["uw_" .. set] = { type = sf2.state.STRING, default = "," }
    end
    sf2.state.register { version = 1, fields = fields }
    registered = true
end
return result
