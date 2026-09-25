local sf2 = require("sf2")
local data = require("content.sensei_victory_data")
local text = require("content.sensei_victory_text")
local shared = require("content.sensei_state")
local dialog = require("content.sensei_dialog")

-- Pending: install before notifications, with verified portrait handles.
local function install(final_ids, portraits)
    assert(type(final_ids) == "table" and #final_ids == 6 and type(portraits) == "table",
        "Sensei victory needs six final IDs and verified portraits")
    local acts = {}
    for act = 1, 6 do
        assert(type(final_ids[act]) == "string" and final_ids[act]:match("^[%w_.%-]+:fights/.+$")
            and not acts[final_ids[act]], "Unique qualified final fight IDs required")
        acts[final_ids[act]] = act
        for _, line in ipairs(data[act]) do assert(portraits[line.portrait], "Missing verified portrait: " .. line.portrait) end
    end
    shared.register()
    local scene
    local function pending_act()
        for act = 1, 6 do
            if sf2.state.get("sensei_dialogue_pending_" .. act) and not sf2.state.get("sensei_complete_" .. act) then return act end
        end
    end
    shared.dialogue_pending = function() return pending_act() ~= nil end
    local show_next = function() end
    local function complete(act)
        sf2.state.set { ["sensei_complete_" .. act] = true, ["sensei_dialogue_pending_" .. act] = false }
        show_next()
        if not pending_act() then
            shared.wake_defeat()
            shared.wake_notifications()
        end
    end
    show_next = function()
        if scene ~= "map" then return end
        local act = pending_act()
        if not act then return end
        local steps = {}
        for _, line in ipairs(data[act]) do
            steps[#steps + 1] = { dialog = dialog.definition(line, text, portraits[line.portrait]) }
        end
        if act == 6 then
            steps[#steps + 1] = { act_screen = { lines = { { text = text.Sensei_arc_outro, frames = 180 } } } }
        end
        sf2.story.play_sequence {
            position = "sensei_dialogue_next_" .. act, steps = steps,
            on_complete = function() complete(act) end,
        }
    end

    sf2.story.on("battle_result", function(event)
        local act = acts[event.fight]
        if not act or event.outcome ~= "win" or sf2.state.get("sensei_complete_" .. act) then return end
        sf2.state.set { ["sensei_dialogue_pending_" .. act] = true }
        show_next()
    end)
    sf2.story.on("scene_enter", function(event)
        scene = event.scene
        show_next()
    end)
end
return { install = install }
