local sf2 = require("sf2")
-- Underworld names, titles, descriptions and rule labels in the archive's 14
-- languages (generated into underworld_text_values.lua). Keys keep their archive
-- spelling; registered ids are "uw.<key>", which native lookups see in lower case.
local values = require("content.underworld_text_values")
local text = { languages = {} }
for language in pairs(values) do text.languages[#text.languages + 1] = language end
table.sort(text.languages)

local handles = {}
for _, language in ipairs(text.languages) do
    for key, value in pairs(values[language]) do
        handles[key] = sf2.localization.register { id = "uw." .. key, language = language, value = value }
    end
end
for key, handle in pairs(handles) do text[key] = handle end

-- Native string fields (battle alias/title/description, warrior first names)
-- take the qualified key that the adapter registers as an external string.
function text.key(key)
    assert(handles[key], "Unknown Underworld text key " .. tostring(key))
    return (sf2.mod.id .. ":localization/uw." .. key):lower()
end

function text.value(key, language)
    return values[language] and values[language][key]
end

return text
