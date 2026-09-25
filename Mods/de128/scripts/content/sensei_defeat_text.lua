local sf2 = require("sf2")
-- Text lives in localizations/<language>.toml.
local text = {}
for _, key in ipairs({
    "OK",
    "Sensei_defeat1",
    "Sensei_defeat2",
    "Sensei_defeat3",
    "Sensei_defeat4",
    "Sensei_defeat5",
    "characterSensei",
}) do
    text[key] = sf2.localization.key("sensei.defeat." .. key)
end
return text
