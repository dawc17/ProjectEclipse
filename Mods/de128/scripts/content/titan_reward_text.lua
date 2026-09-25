local sf2 = require("sf2")
local values = require("content.titan_reward_text_values")
local names = {}

for language, words in pairs(values) do
    for key, value in pairs(words) do
        names[key] = sf2.localization.register {
            id = "titan_reward." .. key,
            language = language,
            value = value,
        }
    end
end

return names
