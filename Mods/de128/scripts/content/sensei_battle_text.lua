local sf2 = require("sf2")
-- Text lives in localizations/<language>.toml.
local text = {}
for _, key in ipairs({
    "alias",
    "description",
    "locked",
    "title",
}) do
    text[key] = (sf2.mod.id .. ":localization/sensei.battle." .. key):lower()
end
return text
