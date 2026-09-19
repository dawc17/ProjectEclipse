local sf2 = require("sf2")
local progression = require("content.sensei_progression")
local map = require("content.sensei_map")
local text = require("content.sensei_notification_text")
local portrait = sf2.assets.sprite("core:ui/users/character_sensei")

-- Pending until the six battle definitions and prior-act finals are assembled.
-- Install once during registration. No XML or native user-variable names escape
-- into the runtime behavior: all notification state belongs to this mod.
local function install(battles, finals)
    assert(#battles == 6 and #finals == 5, "Sensei notifications need six battles and five prior-act finals")
    local fields = {}
    for act = 1, 6 do
        fields["sensei_opened_" .. act] = { type = sf2.state.BOOLEAN, default = false }
        fields["sensei_pending_" .. act] = { type = sf2.state.BOOLEAN, default = false }
    end
    sf2.state.register { version = 1, fields = fields }
    local scene, view
    local show_next
    show_next = function()
        if scene ~= "map" or view then return end
        local act
        for index = 1, 6 do
            if sf2.state.get("sensei_pending_" .. index) and not sf2.state.get("sensei_opened_" .. index) then
                act = index
                break
            end
        end
        if not act then return end
        local function acknowledge(current)
            if act == 1 and not sf2.profile.set_eclipse_mode(false) then return end
            if not map.open_act(act, battles) then return end
            sf2.state.set { ["sensei_opened_" .. act] = true, ["sensei_pending_" .. act] = false }
            sf2.ui.close(current)
            show_next()
        end
        local line = act == 1 and text.intro or (act == 6 and text.ending or text.more)
        view = sf2.ui.open {
            id = "sensei_notification", mount = "modal",
            root = { id = "dialog", kind = "column", width = 760, height = 400, gap = 12, children = {
                { id = "speaker", kind = "text", width = 736, height = 48, text = sf2.localization.text(text.title), style = { font_size = 32 } },
                { id = "content", kind = "row", width = 736, height = 252, gap = 20, children = {
                    { id = "portrait", kind = "image", width = 192, height = 252, sprite = portrait },
                    { id = "body", kind = "text", width = 524, height = 252, text = sf2.localization.text(line), style = { font_size = 26, text_align = "left" } },
                } },
                { id = "continue", kind = "button", width = 240, height = 60, text = "OK" },
            } },
            on_click = function(current, id) if id == "continue" then acknowledge(current) end end,
            -- Native regular notifications accept Back as acknowledgement.
            -- Scene/profile teardown uses on_close only and never progresses.
            on_back = acknowledge,
            on_close = function(current) if view == current then view = nil end end,
        }
    end
    sf2.story.on("battle_result", function(event)
        local opened, updates = {}, {}
        for act = 1, 6 do opened[act] = sf2.state.get("sensei_opened_" .. act) end
        for _, act in ipairs(progression.find_unlocks(event, opened, finals)) do
            updates["sensei_pending_" .. act] = true
        end
        if next(updates) then sf2.state.set(updates) end
        show_next()
    end)
    sf2.story.on("scene_enter", function(event)
        scene = event.scene
        show_next()
    end)
end

return { install = install }
