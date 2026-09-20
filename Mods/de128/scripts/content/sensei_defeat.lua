local sf2 = require("sf2")
local shared = require("content.sensei_state")
local text = require("content.sensei_defeat_text")

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
        local function acknowledge(current)
            if view ~= current or scene ~= "map" then return end
            sf2.state.set { sensei_defeat_pending = false }
            sf2.ui.close(current)
            shared.wake_notifications()
        end
        view = sf2.ui.open {
            id = "sensei_defeat", mount = "modal",
            root = { id = "dialog", kind = "column", width = 760, height = 400, gap = 12, children = {
                { id = "speaker", kind = "text", width = 736, height = 48, text = sf2.localization.text(text.characterSensei), style = { font_size = 32 } },
                { id = "content", kind = "row", width = 736, height = 252, gap = 20, children = {
                    { id = "portrait", kind = "image", width = 192, height = 252, sprite = portrait },
                    { id = "body", kind = "text", width = 524, height = 252, text = sf2.localization.text(text["Sensei_defeat" .. choice]), style = { font_size = 26, text_align = "left" } },
                } },
                { id = "continue", kind = "button", width = 240, height = 60, text = sf2.localization.text(text.OK) },
            } },
            on_click = function(current, id) if id == "continue" then acknowledge(current) end end,
            on_back = acknowledge,
            on_close = function(current) if view == current then view = nil end end,
        }
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
