local sf2 = require("sf2")

-- The remaining archived choices need missing installed art or params. Keep the
-- order of the choices whose location dependencies have been audited.
local choices = {
    { location = "dojo", label = "DefaultDojo" },
    { location = "new_year_24_china_dojo", label = "DojoChinese24" },
    { location = "dojo_indian_event", label = "DojoIndia" },
    { location = "dojo_indian_event_22", label = "DojoIndia22" },
    { location = "dojo_india24", label = "DojoIndia24" },
    { location = "haloween_dojo", label = "DojoHalloween" },
    { location = "haloween_dojo_2019", label = "DojoHalloween19" },
    { location = "dojo_hw21", label = "DojoHalloween21" },
    { location = "dojo_american_event_22", label = "DojoAmerican22" },
    { location = "dojo_hw22", label = "DojoStudio" },
}

local labels = {}
for _, choice in ipairs(choices) do
    labels[choice.label] = sf2.localization.key("dojo." .. choice.label)
end
labels.DojoChangerTitle = sf2.localization.key("dojo.DojoChangerTitle")
labels.BACK = sf2.localization.key("dojo.BACK")

local function text(key)
    return sf2.localization.text(labels[key])
end

local function open_selector()
    local selected = sf2.locations.selected_dojo()
    local cells = {}
    for i, choice in ipairs(choices) do
        local qualified = "core:locations/" .. choice.location
        local label = text(choice.label)
        if selected == qualified then label = "✓ " .. label end
        cells[i] = { id = "choice_" .. i, kind = "button", text = label }
    end
    sf2.ui.open {
        id = "dojo_changer", mount = "modal",
        root = { id = "panel", kind = "column", width = 548, height = 410,
            gap = 10, children = {
                { id = "title", kind = "text", width = 548, height = 42,
                  text = text("DojoChangerTitle") },
                { id = "choices", kind = "grid", width = 548, height = 300,
                  columns = 2, cell_width = 269, cell_height = 52, gap = 10,
                  children = cells },
                { id = "close", kind = "button", width = 548, height = 48,
                  text = text("BACK") },
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
        image = sf2.assets.sprite("sprites/dojo_changer/credits"), x = -3095, y = -645,
        anchor_min_x = 1, anchor_max_x = 1, show_type = "both" } },
}

sf2.story.on("map_button", function(event)
    if event.button == "de128.dojo_changer" then open_selector() end
end)

return choices
