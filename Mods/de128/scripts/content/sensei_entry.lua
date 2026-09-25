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
            local steps = {}
            for _, card in ipairs(sequence.cards) do
                if card.lines then
                    local lines = {}
                    for index, line in ipairs(card.lines) do lines[index] = { text = text[line.text], frames = line.frames } end
                    steps[#steps + 1] = { act_screen = { lines = lines } }
                else
                    steps[#steps + 1] = { dialog = dialog.definition(card, text, portraits[card.portrait]) }
                end
            end
            assert(sequence.cards[#sequence.cards].launch, "Sensei entry must end with its explicit Fight button")
            local opened = sf2.story.play_sequence {
                steps = steps,
                on_step = function() return sf2.story.fight_pending(request) end,
                on_complete = function()
                    if not sf2.story.fight_pending(request) then return end
                    if sequence.mark_shogun_greeted then sf2.state.set { sensei_shogun_greeted = true } end
                    if sequence.complete_before_launch then sf2.state.set { [flag] = true } end
                    if sf2.story.resume_fight(request) then
                        if not sequence.complete_before_launch then sf2.state.set { [flag] = true } end
                    else sf2.story.cancel_fight(request) end
                end,
                on_cancel = function() sf2.story.cancel_fight(request) end,
            }
            if not opened then sf2.story.cancel_fight(request) end
            return nil -- The exact native entry remains held until the Fight button.
        end)
    end
end
return { install = install }
