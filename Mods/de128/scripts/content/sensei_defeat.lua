local sf2 = require("sf2")
local shared = require("content.sensei_state")
local text = require("content.sensei_defeat_text")
local dialog = require("content.sensei_dialog")

-- SenseiLoss: ordinary normal-fight loss queues one resumable Map dialog.
-- The native expression draws when the dialog opens, not when the fight ends.
local function install(normal_ids, portrait)
    assert(type(normal_ids) == "table" and #normal_ids == 6 and portrait,
        "Six normal Sensei fight ID arrays and a verified portrait required")
    local normal = {}
    for act = 1, 6 do
        assert(type(normal_ids[act]) == "table" and #normal_ids[act] == (act == 6 and 2 or 3),
            "Incomplete normal Sensei fight IDs")
        for _, id in ipairs(normal_ids[act]) do
            assert(type(id) == "string" and id:match("^[%w_.%-]+:fights/.+$") and not normal[id],
                "Unique qualified normal fight IDs required")
            normal[id] = true
        end
    end
    shared.register()
    local scene, view
    shared.defeat_pending = function() return sf2.state.get("sensei_defeat_pending") end
    local function show_next()
        if scene ~= "map" or view or shared.dialogue_pending() or not shared.defeat_pending() then return end
        local choice = sf2.random.integer("sensei_defeat_rng", 1, 5)
        local token = {}
        local card = { title = "characterSensei", text = "Sensei_defeat" .. choice, button = "OK" }
        local opened = dialog.open(card, text, portrait, function()
            if view ~= token then return end
            view = nil
            sf2.state.set { sensei_defeat_pending = false }
            shared.wake_notifications()
        end, function()
            if view == token then view = nil end
        end)
        if opened then view = token end
    end
    shared.wake_defeat = show_next
    sf2.story.on("battle_result", function(event)
        if event.outcome ~= "loss" or not normal[event.fight] then return end
        -- Native AllowDoubles defaults false: another loss cannot stack copies.
        sf2.state.set { sensei_defeat_pending = true }
        show_next()
    end)
    sf2.story.on("scene_enter", function(event)
        scene = event.scene
        show_next()
    end)
end
return { install = install }
