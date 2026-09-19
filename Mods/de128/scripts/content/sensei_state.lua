local sf2 = require("sf2")
local registered = false
local result = { dialogue_pending = function() return false end, wake_notifications = function() end }
function result.register()
    if registered then return end
    local fields = {}
    for act = 1, 6 do
        for _, kind in ipairs { "opened", "pending", "dialogue_pending", "complete" } do
            fields["sensei_" .. kind .. "_" .. act] = { type = sf2.state.BOOLEAN, default = false }
        end
        fields["sensei_dialogue_next_" .. act] = { type = sf2.state.INTEGER, default = 1 }
    end
    sf2.state.register { version = 1, fields = fields }
    registered = true
end
return result
