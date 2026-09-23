local sf2 = require("sf2")
local sequences = require("content.sensei_entry_data")
local text = require("content.sensei_entry_text")
local shared = require("content.sensei_state")
local dialog = require("content.sensei_dialog")

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
            local position = 1
            local show_next = nil
            local function acknowledge(card)
                if not sf2.story.fight_pending(request) then return end
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
                -- Native story dialog; teardown before acknowledgement abandons the entry.
                if not dialog.open(card, text, portraits[card.portrait], function() acknowledge(card) end,
                    function() sf2.story.cancel_fight(request) end) then
                    sf2.story.cancel_fight(request)
                end
            end
            show_next()
            return nil -- The exact native entry remains held until the Fight button.
        end)
    end
end
return { install = install }
