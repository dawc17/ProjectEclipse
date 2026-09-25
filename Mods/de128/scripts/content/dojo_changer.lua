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

for _, choice in ipairs(choices) do
    choice.preview = sf2.assets.sprite("sprites/dojo_changer/" .. choice.location)
    choice.name = sf2.localization.key("dojo." .. choice.label)
end
local halo = sf2.assets.sprite("sprites/dojo_changer/selected_halo")
local title = sf2.localization.key("dojo.DojoChangerTitle")
local back = sf2.localization.key("dojo.BACK")

-- Three medallions per row with their names underneath. The whole cell is a
-- transparent button; the current dojo gets a gold halo and gold caption.
local COLUMNS, CELL_W, CELL_H, GAP = 3, 158, 184, 10

local function open_selector()
    local selected = sf2.locations.selected_dojo()
    local cells = {}
    for i, choice in ipairs(choices) do
        local current = selected == "core:locations/" .. choice.location
        cells[i] = { id = "tile_" .. i, kind = "stack", width = CELL_W, height = CELL_H,
            children = {
                { id = "choice_" .. i, kind = "button", width = CELL_W, height = CELL_H,
                  style = { background_color = "#FFFFFF00" } },
                { id = "card_" .. i, kind = "column", width = CELL_W, height = CELL_H, gap = 4,
                  children = {
                    { id = "art_" .. i, kind = "stack", width = CELL_W, height = 140, children = {
                        { id = "halo_" .. i, kind = "image", width = 140, height = 140,
                          sprite = halo, visible = current },
                        { id = "preview_" .. i, kind = "image", width = 118, height = 118,
                          sprite = choice.preview },
                    } },
                    { id = "name_" .. i, kind = "text", width = CELL_W, height = 40,
                      text = sf2.localization.text(choice.name),
                      style = { font_size = 17, text_color = current and "#9A6A12" or nil } },
                  } },
            } }
    end
    local rows = math.ceil(#choices / COLUMNS)
    sf2.ui.open {
        id = "dojo_changer", mount = "modal",
        root = { id = "panel", kind = "stack", width = 580, height = 630,
            style = { frame = "scroll" }, children = {
                { id = "content", kind = "column", width = 520, height = 480,
                  gap = 10, children = {
                    { id = "title", kind = "text", width = 520, height = 44,
                      text = sf2.localization.text(title), style = { font_size = 28 } },
                    { id = "viewport", kind = "scroll", width = 520, height = 366,
                      children = {
                        { id = "choices", kind = "grid", width = 494,
                          height = rows * CELL_H + (rows - 1) * GAP,
                          columns = COLUMNS, cell_width = CELL_W, cell_height = CELL_H,
                          gap = GAP, children = cells },
                      } },
                    { id = "close", kind = "button", width = 520, height = 50,
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

-- The chooser lives in the dojo menu, below the disciple toggle's slot. Earlier
-- versions saved a map button into profiles; remove it at each session start.
sf2.ui.dojo_button { id = "dojo_changer",
    image = sf2.assets.sprite("sprites/dojo_changer/credits") }

sf2.quests.register {
    id = "dojo_changer_map_button", place = "map", events = { "session" },
    actions = { { type = "hide_map_button", id = "dojo_changer" } },
}

sf2.story.on("dojo_button", function(event)
    if event.button == "de128.dojo_changer" then open_selector() end
end)

return choices
