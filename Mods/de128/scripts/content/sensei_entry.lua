local sf2 = require("sf2")
local sequences = require("content.sensei_entry_data")
local text = require("content.sensei_entry_text")
local shared = require("content.sensei_state")

-- Pending until the complete, verified normal roster and portraits are supplied.
local function install(normal, portraits)
    assert(type(normal) == "table" and #normal == 6 and type(portraits) == "table", "Six normal Sensei rosters and portraits required")
    for _, sequence in ipairs(sequences) do
        assert(normal[sequence.act] and normal[sequence.act][sequence.index], "Missing normal Sensei fight")
        for _, card in ipairs(sequence.cards) do
            if card.portrait then assert(portraits[card.portrait], "Missing verified portrait: " .. card.portrait) end
        end
    end
    shared.register()
    for _, sequence in ipairs(sequences) do
        local flag = "sensei_entered_" .. sequence.act .. "_" .. sequence.index
        sf2.story.before_fight(normal[sequence.act][sequence.index], function(request)
            if sf2.state.get(flag) then return true end
            local position, view, advancing = 1, nil, false
            local show_next
            local function acknowledge(current, card)
                if view ~= current or not sf2.story.fight_pending(request) then return end
                advancing = true
                sf2.ui.close(current)
                advancing = false
                if card.launch then
                    if sequence.mark_shogun_greeted then sf2.state.set { sensei_shogun_greeted = true } end
                    if sequence.complete_before_launch then sf2.state.set { [flag] = true } end
                    if sf2.story.resume_fight(request) then
                        if not sequence.complete_before_launch then sf2.state.set { [flag] = true } end
                    else
                        sf2.story.cancel_fight(request)
                    end
                    return
                end
                position = position + 1
                show_next()
            end
            show_next = function()
                if not sf2.story.fight_pending(request) then return end
                local card = sequence.cards[position]
                assert(card, "Sensei entry must end with its explicit Fight button")
                if card.lines then
                    local lines = {}
                    for index, line in ipairs(card.lines) do lines[index] = { text = text[line.text], frames = line.frames } end
                    if not sf2.ui.act_screen { lines = lines, on_complete = function()
                        position = position + 1
                        show_next()
                    end } then sf2.story.cancel_fight(request) end
                    return
                end
                view = sf2.ui.open {
                    id = "sensei_entry", mount = "modal",
                    root = { id = "dialog", kind = "column", width = 800, height = 460, gap = 12, children = {
                        { id = "speaker", kind = "text", width = 776, height = 48, text = sf2.localization.text(text[card.title]), style = { font_size = 32 } },
                        { id = "content", kind = "row", width = 776, height = 310, gap = 20, children = {
                            { id = "portrait", kind = "image", width = 210, height = 310, sprite = portraits[card.portrait], mirrored = card.mirrored },
                            { id = "body", kind = "text", width = 546, height = 310, text = sf2.localization.text(text[card.text]), style = { font_size = 24, text_align = "left" } },
                        } },
                        { id = "continue", kind = "button", width = 240, height = 60, text = sf2.localization.text(text[card.button]) },
                    } },
                    on_click = function(current, id) if id == "continue" then acknowledge(current, card) end end,
                    on_back = function(current) if not card.ignore_back then acknowledge(current, card) end end,
                    on_close = function(current)
                        if view == current then view = nil end
                        if not advancing then sf2.story.cancel_fight(request) end
                    end,
                }
            end
            show_next()
            return nil -- The exact native entry remains held until the Fight button.
        end)
    end
end
return { install = install }
