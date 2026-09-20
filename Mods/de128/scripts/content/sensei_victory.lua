local sf2 = require("sf2")
local data = require("content.sensei_victory_data")
local text = require("content.sensei_victory_text")
local shared = require("content.sensei_state")

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
    local scene, view, outro
    local function pending_act()
        for act = 1, 6 do
            if sf2.state.get("sensei_dialogue_pending_" .. act) and not sf2.state.get("sensei_complete_" .. act) then return act end
        end
    end
    shared.dialogue_pending = function() return pending_act() ~= nil end
    local show_next
    local function complete(act)
        sf2.state.set { ["sensei_complete_" .. act] = true, ["sensei_dialogue_pending_" .. act] = false }
        show_next()
        if not pending_act() then
            shared.wake_defeat()
            shared.wake_notifications()
        end
    end
    show_next = function()
        if scene ~= "map" or view or outro then return end
        local act = pending_act()
        if not act then return end
        local index = sf2.state.get("sensei_dialogue_next_" .. act)
        assert(index >= 1 and index <= #data[act] + 1, "Invalid saved Sensei dialogue position")
        local line = data[act][index]
        if not line then
            if act ~= 6 then complete(act); return end
            local token = {}
            outro = token
            local accepted = sf2.ui.act_screen { lines = { { text = text.Sensei_arc_outro, frames = 180 } }, on_complete = function()
                if outro ~= token or scene ~= "map" then return end
                if not sf2.state.get("sensei_dialogue_pending_" .. act) then return end
                outro = nil
                complete(act)
            end }
            if not accepted and outro == token then outro = nil end
            return
        end
        local function advance(current)
            if scene ~= "map" or view ~= current then return end
            sf2.state.set { ["sensei_dialogue_next_" .. act] = index + 1 }
            sf2.ui.close(current)
            show_next()
        end
        view = sf2.ui.open {
            id = "sensei_victory", mount = "modal",
            root = { id = "dialog", kind = "column", width = 800, height = 460, gap = 12, children = {
                { id = "speaker", kind = "text", width = 776, height = 48, text = sf2.localization.text(text[line.title]), style = { font_size = 32 } },
                { id = "content", kind = "row", width = 776, height = 310, gap = 20, children = {
                    { id = "portrait", kind = "image", width = 210, height = 310, sprite = portraits[line.portrait], mirrored = line.mirrored },
                    { id = "body", kind = "text", width = 546, height = 310, text = sf2.localization.text(text[line.text]), style = { font_size = 24, text_align = "left" } },
                } },
                { id = "continue", kind = "button", width = 240, height = 60, text = sf2.localization.text(text.OK) },
            } },
            on_click = function(current, id) if id == "continue" then advance(current) end end,
            on_back = advance,
            on_close = function(current) if view == current then view = nil end end,
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
        outro = nil
        show_next()
    end)
end
return { install = install }
