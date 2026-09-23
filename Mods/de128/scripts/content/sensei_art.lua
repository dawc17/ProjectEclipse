local sf2 = require("sf2")
-- Sensei presentation art. Core supplies most portraits; DE128 ships the seven
-- portraits and five previews that public core sprite IDs cannot resolve
-- (Tools/ExtractDE128SenseiArt.py records sources and hashes). character_sensei
-- is a native resource missing from the packaged-art catalog. Names stay archival.
local owned = {
    boss_hermit_young = true, boss_butcher_young = true, boss_wasp_young = true,
    boss_widow_young = true, boss_shogun_young = true, character_pirate = true,
    character_sensei = true,
}
local cache = {}

local function portrait(name)
    assert(type(name) == "string" and name ~= "", "Portrait name required")
    local handle = cache[name]
    if not handle then
        handle = owned[name] and sf2.assets.sprite("sprites/sensei/" .. name)
            or sf2.assets.sprite("core:ui/users/" .. name:lower())
        cache[name] = handle
    end
    return handle
end

-- Warrior avatars keep their native resource name when core ships the portrait.
local function avatar(name)
    if owned[name] then return portrait(name) end
    return name
end

-- Only the pvp arena previews are absent from core; preview_main.* stays native.
local function preview(name)
    if name:find("^preview_pvp_") then return sf2.assets.sprite("sprites/sensei/" .. name) end
    return name
end

-- Every portrait the dialogue, victory, defeat and notification modules need.
local function story_portraits()
    local result = { character_sensei = portrait("character_sensei") }
    for _, sequence in ipairs(require("content.sensei_entry_data")) do
        for _, card in ipairs(sequence.cards) do
            if card.portrait then result[card.portrait] = portrait(card.portrait) end
        end
    end
    for _, sequence in ipairs(require("content.sensei_victory_data")) do
        for _, card in ipairs(sequence) do result[card.portrait] = portrait(card.portrait) end
    end
    return result
end

return { owned = owned, portrait = portrait, avatar = avatar, preview = preview, story_portraits = story_portraits }
