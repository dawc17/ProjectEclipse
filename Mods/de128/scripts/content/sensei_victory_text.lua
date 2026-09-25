local sf2 = require("sf2")
-- Text lives in localizations/<language>.toml.
local text = {}
for _, key in ipairs({
    "Ancient_zone6_1",
    "Ancient_zone6_2",
    "Butcher_zone3_3",
    "Butcher_zone3_4",
    "Butcher_zone3_5",
    "Butcher_zone3_6",
    "Hermit_zone2_3",
    "Hermit_zone2_4",
    "NAME_BUTCHER",
    "NAME_HERMIT",
    "NAME_PRINCE",
    "NAME_WASP",
    "NAME_WIDOW",
    "OK",
    "Prince_zone1_1",
    "Prince_zone1_2",
    "Prince_zone1_3",
    "Prince_zone2_1",
    "Sensei_arc_outro",
    "Sensei_zone1_1",
    "Sensei_zone2_1",
    "Sensei_zone3_2",
    "Sensei_zone4_2",
    "Sensei_zone5_1",
    "Sensei_zone6_2",
    "Wasp_zone4_3",
    "Wasp_zone4_4",
    "Wasp_zone4_5",
    "Widow_zone5_3",
    "Widow_zone5_4",
    "characterAncient",
    "characterSensei",
}) do
    text[key] = sf2.localization.key("sensei.victory." .. key)
end
return text
