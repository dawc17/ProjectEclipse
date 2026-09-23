local sf2 = require("sf2")
local story = require("content.underworld_story_data")
local text = require("content.underworld_text")
local shared = require("content.sensei_state")
local sensei_art = require("content.sensei_art")

-- The archived Underworld quests (RaidIntro, RaidIntro2 and the 32 RaidEnter/
-- RaidWinAfter/RaidLoseAfter bosses; see Tools/GenerateDE128Underworld.py) as mod
-- state and native story dialogs. No XML or native user variable is read.

-- Native resources absent from the packaged-art catalog ship with DE128.
local OWNED_PORTRAITS = { character_may_1 = "sprites/underworld/character_may_1", character_may_4 = "sprites/underworld/character_may_4" }
local portraits = {}
local function portrait(name)
    if name == nil then return nil end
    local handle = portraits[name]
    if handle == nil then
        if OWNED_PORTRAITS[name] then handle = sf2.assets.sprite(OWNED_PORTRAITS[name])
        elseif sensei_art.owned[name] then handle = sensei_art.portrait(name)
        else handle = sf2.assets.sprite("core:ui/users/" .. name:lower()) end
        portraits[name] = handle
    end
    return handle
end

local function open_card(card, on_complete, on_cancel)
    assert((card.title == nil or text[card.title]) and text[card.text] and text[card.button], "Underworld dialog text missing")
    return sf2.ui.story_dialog {
        title = card.title and text[card.title] or nil, portrait = portrait(card.portrait), mirrored = card.mirrored == true,
        lines = { { text = text[card.text] } }, button = text[card.button],
        ignore_back = card.ignore_back == true, on_complete = on_complete, on_cancel = on_cancel,
    }
end

local function act_lines(card)
    local lines = {}
    for index, line in ipairs(card.lines) do lines[index] = { text = text[line.text], frames = line.frames } end
    return lines
end

-- Every portrait the story names, resolved during installation.
local function story_portraits()
    local result = {}
    local function add(cards)
        for _, card in ipairs(cards) do
            if card.portrait then result[card.portrait] = portrait(card.portrait) end
        end
    end
    add(story.intro)
    add(story.followup)
    for _, boss in pairs(story.bosses) do
        add(boss.enter)
        add(boss.win)
        add(boss.loss)
    end
    return result
end

-- Saved boss sets (sensei_state.lua): ",name,name," with lower-case archive names.
local function has(set, key) return sf2.state.get("uw_" .. set):find("," .. key .. ",", 1, true) ~= nil end
local function add(set, key)
    if not has(set, key) then sf2.state.set { ["uw_" .. set] = sf2.state.get("uw_" .. set) .. key .. "," } end
end
local function remove(set, key)
    local value = sf2.state.get("uw_" .. set)
    local first, last = value:find("," .. key .. ",", 1, true)
    if first then sf2.state.set { ["uw_" .. set] = value:sub(1, first) .. value:sub(last + 1) } end
end

-- underworld: the table returned by content.underworld's install.
local function install(underworld)
    assert(type(underworld) == "table" and type(underworld.by_battle) == "table", "Underworld registration required")
    assert(underworld.battles[story.focus], "Unknown Underworld focus battle " .. tostring(story.focus))
    shared.register()
    story_portraits()
    local results = {}
    for boss, sequences in pairs(story.bosses) do
        local fights = underworld.by_battle[boss]
        assert(fights and fights["1"], "Missing Underworld story fight " .. boss .. "|1")
        local key = boss:lower()
        results[fights["1"].id] = key
        -- RaidEnter<Boss>: once per profile, the exact entry is held until the Fight button.
        sf2.story.before_fight(fights["1"].handle, function(request)
            if has("entered", key) then return true end
            local position = 1
            local show_next = nil
            show_next = function()
                if not sf2.story.fight_pending(request) then return end
                local card = sequences.enter[position]
                assert(card, "Underworld entry must end with its Fight button")
                if card.lines then
                    if not sf2.ui.act_screen { lines = act_lines(card), on_complete = function()
                        position = position + 1
                        show_next()
                    end } then sf2.story.cancel_fight(request) end
                    return
                end
                local opened = open_card(card, function()
                    if not sf2.story.fight_pending(request) then return end
                    if not card.launch then
                        position = position + 1
                        show_next()
                        return
                    end
                    if sf2.story.resume_fight(request) then add("entered", key)
                    else sf2.story.cancel_fight(request) end
                end, function() sf2.story.cancel_fight(request) end)
                if not opened then sf2.story.cancel_fight(request) end
            end
            show_next()
            return nil
        end)
    end

    local scene, view = nil, nil
    local show_next = nil

    -- Runs cards in order; on_done runs after the last acknowledgement. A refused
    -- or cancelled card leaves the saved step unchanged for the next map entry.
    local function run_cards(cards, on_done)
        local position = 1
        local step = nil
        step = function()
            local card = cards[position]
            if not card then
                view = nil
                on_done()
                return
            end
            local token = {}
            local opened = open_card(card, function()
                if view ~= token then return end
                position = position + 1
                if scene == "map" then step() else view = nil end
            end, function()
                if view == token then view = nil end
            end)
            if opened then view = token else view = nil end
        end
        step()
    end

    local function run_intro()
        local token = {}
        view = token
        local intro_cards = {}
        for _, card in ipairs(story.intro) do
            if not card.lines then intro_cards[#intro_cards + 1] = card end
        end
        local accepted = sf2.ui.act_screen { lines = act_lines(story.intro[1]), on_complete = function()
            if view ~= token then return end
            if scene ~= "map" then view = nil return end
            -- ShowRaidsGag and SetMapFocus precede the archived intro dialogs.
            sf2.underworld.set_toggle_visible(true)
            sf2.underworld.set_focus(underworld.battles[story.focus])
            run_cards(intro_cards, function()
                sf2.state.set { uw_intro = 1 }
                show_next()
            end)
        end }
        if not accepted and view == token then view = nil end
    end

    local order = {}
    for boss in pairs(story.bosses) do order[#order + 1] = boss end
    table.sort(order)
    local function pending_result()
        for _, boss in ipairs(order) do
            local key = boss:lower()
            if has("win_pending", key) then return boss, "win" end
            if has("loss_pending", key) then return boss, "loss" end
        end
        return nil, nil
    end

    show_next = function()
        if scene ~= "map" or view or shared.dialogue_pending() or shared.defeat_pending() then return end
        local intro = sf2.state.get("uw_intro")
        if intro == 0 then
            -- RaidIntro: the archive checks a win over Lynx 2 when a fight ends. Saves that
            -- already have it (or gained it before DE128) are caught up on map entry.
            if sf2.profile.fight(story.unlock_fight).wins < 1 then return end
            run_intro()
            return
        end
        if intro == 1 then
            -- RaidIntro2 ends with ChangeScene Dojo on its last button.
            local cards = story.followup
            assert(cards[#cards].scene == "dojo", "Underworld followup must end in the dojo")
            run_cards(cards, function()
                sf2.state.set { uw_intro = 2 }
                sf2.scenes.open("dojo")
            end)
            return
        end
        local boss, kind = pending_result()
        if not boss then return end
        local key = boss:lower()
        run_cards(story.bosses[boss][kind], function()
            add(kind .. "_shown", key)
            remove(kind .. "_pending", key)
            show_next()
        end)
    end

    sf2.story.on("battle_result", function(event)
        local key = results[event.fight]
        -- The archive's FightResult is "Loss" for defeats; a surrender is distinct.
        if not key or (event.outcome ~= "win" and event.outcome ~= "loss") then return end
        if not has(event.outcome .. "_shown", key) then add(event.outcome .. "_pending", key) end
        show_next()
    end)
    sf2.story.on("scene_enter", function(event)
        scene = event.scene
        view = nil
        if scene ~= "map" then return end
        -- The toggle stays hidden until the intro's ShowRaidsGag step.
        if sf2.state.get("uw_intro") == 0 then sf2.underworld.set_toggle_visible(false) end
        show_next()
    end)
end

return { install = install, portrait = portrait, story_portraits = story_portraits }
