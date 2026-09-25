local sf2 = require("sf2")

-- The remaining archived choices need missing installed art or params. Keep the
-- order of the choices whose location dependencies have been audited.
local choices = {
    { location = "dojo" },
    { location = "new_year_24_china_dojo" },
    { location = "dojo_indian_event" },
    { location = "dojo_indian_event_22" },
    { location = "dojo_india24" },
    { location = "haloween_dojo" },
    { location = "haloween_dojo_2019" },
    { location = "dojo_hw21" },
    { location = "dojo_american_event_22" },
    { location = "dojo_hw22" },
}

for _, choice in ipairs(choices) do
    choice.preview = sf2.assets.sprite("sprites/dojo_changer/" .. choice.location)
end
local title = sf2.localization.key("dojo.DojoChangerTitle")
local back = sf2.localization.key("dojo.BACK")

local function open_selector()
    local selected = sf2.locations.selected_dojo()
    local cells = {}
    for i, choice in ipairs(choices) do
        local qualified = "core:locations/" .. choice.location
        cells[i] = { id = "tile_" .. i, kind = "stack", width = 243, height = 168,
            children = {
                { id = "choice_" .. i, kind = "button", width = 243, height = 168,
                  style = { background_color = selected == qualified and "#E9C56B" or "#FFFFFF00" } },
                { id = "preview_" .. i, kind = "image", width = 235, height = 160,
                  sprite = choice.preview },
            } }
    end
    sf2.ui.open {
        id = "dojo_changer", mount = "modal",
        root = { id = "panel", kind = "stack", width = 620, height = 590,
            style = { frame = "scroll" }, children = {
                { id = "content", kind = "column", width = 520, height = 480,
                  gap = 12, children = {
                    { id = "title", kind = "text", width = 520, height = 44,
                      text = sf2.localization.text(title) },
                    { id = "viewport", kind = "scroll", width = 520, height = 360,
                      children = {
                        { id = "choices", kind = "grid", width = 498, height = 888,
                          columns = 2, cell_width = 243, cell_height = 168,
                          gap = 12, children = cells },
                      } },
                    { id = "close", kind = "button", width = 520, height = 52,
                      text = sf2.localization.text(back) },
                  } },
            } },
        on_click = function(view, id)
            if id == "close" then sf2.ui.close(view); return end
            local index = tonumber(id:match("^choice_(%d+)$"))
            local choice = index and choices[index]
            if not choice then return end
            sf2.locations.select_dojo("core:locations/" .. choice.location)
            sf2.ui.close(view)
            sf2.scenes.open("dojo")
        end,
    }
end

sf2.quests.register {
    id = "dojo_changer_map_button", place = "map", events = { "session" },
    actions = { { type = "show_map_button", id = "dojo_changer",
        image = sf2.assets.sprite("sprites/dojo_changer/credits"), x = 240, y = -650,
        anchor_min_x = 0, anchor_max_x = 0, show_type = "both" } },
}

sf2.story.on("map_button", function(event)
    if event.button == "de128.dojo_changer" then open_selector() end
end)

return choices
