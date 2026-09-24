local sf2 = require("sf2")

-- Exact non-economic SubType differences in Assets/DExml/list.xml.
-- This module changes only combat subtype; shop profiles and default
-- enchantments are applied separately by content.shop.
for _, entry in ipairs({
    { "weapon", "WEAPON_CHNY22_SPEAR", "Naginata" },
    { "weapon", "WEAPON_RAID_KARCER_SET", "HunterClaws" },
    { "weapon", "WEAPON_BG_YARI", "MagariYari" },
    { "ranged", "RANGED_BP_S3_WIND_MAKER", "Kunai" },
}) do
    sf2.items.set_subtype {
        item = sf2.items.get("core:items/" .. entry[1] .. "/" .. entry[2]),
        subtype = entry[3],
    }
end

-- Jian's subtype and complete move graph are registered by content.chinese_swords.
-- Keep the canonical MonkKatars/Musket AI groups: archive omission alone does
-- not establish that removing reconstruction-era compatibility is intentional.
