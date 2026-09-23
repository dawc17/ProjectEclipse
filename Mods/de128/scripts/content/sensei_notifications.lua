local sf2 = require("sf2")
local progression = require("content.sensei_progression")
local map = require("content.sensei_map")
local text = require("content.sensei_notification_text")
local shared = require("content.sensei_state")

-- Pending until the six battle definitions and prior-act finals are assembled.
-- Install once during registration. No XML or native user-variable names escape
-- into the runtime behavior: all notification state belongs to this mod.
local function install(battles, finals, portrait)
    assert(#battles == 6 and #finals == 5, "Sensei notifications need six battles and five prior-act finals")
    portrait = portrait or require("content.sensei_art").portrait("character_sensei")
    shared.register()
    -- Text modules register localizations, so resolve them during installation.
    local captions = { OK = require("content.sensei_defeat_text").OK }
    for key, value in pairs(text) do captions[key] = value end
    local dialog = require("content.sensei_dialog")
    local scene, view
    local show_next
    show_next = function()
        if scene ~= "map" or view or shared.dialogue_pending() or shared.defeat_pending() then return end
        local act
        for index = 1, 6 do
            if sf2.state.get("sensei_pending_" .. index) and not sf2.state.get("sensei_opened_" .. index) then
                act = index
                break
            end
        end
        if not act then return end
        -- The native dialog is already closed when on_complete runs. A refused map
        -- action keeps the act pending; it is shown again on the next map wake.
        local token = {}
        local line = act == 1 and "intro" or (act == 6 and "ending" or "more")
        local card = { title = "title", text = line, button = "OK" }
        local opened = dialog.open(card, captions, portrait, function()
            if view ~= token then return end
            view = nil
            if act == 1 and not sf2.profile.set_eclipse_mode(false) then return end
            if not map.open_act(act, battles) then return end
            sf2.state.set { ["sensei_opened_" .. act] = true, ["sensei_pending_" .. act] = false }
            show_next()
        end, function()
            if view == token then view = nil end
        end)
        if opened then view = token end
    end
    shared.wake_notifications = show_next
    local function queue_unlocks(event)
        local opened, updates = {}, {}
        for act = 1, 6 do opened[act] = sf2.state.get("sensei_opened_" .. act) end
        for _, act in ipairs(progression.find_unlocks(event, opened, finals)) do
            if not sf2.state.get("sensei_pending_" .. act) then updates["sensei_pending_" .. act] = true end
        end
        if next(updates) then sf2.state.set(updates) end
    end
    sf2.story.on("battle_result", function(event)
        queue_unlocks(event)
        show_next()
    end)
    sf2.story.on("scene_enter", function(event)
        scene = event.scene
        -- The archive checks prerequisites after a victory. A save that already met
        -- them before DE128 was enabled would otherwise wait for its next win, so
        -- the map re-checks the same saved prerequisites (never a loss outcome).
        if scene == "map" then queue_unlocks({ outcome = "win" }) end
        show_next()
    end)
end

return { install = install }
