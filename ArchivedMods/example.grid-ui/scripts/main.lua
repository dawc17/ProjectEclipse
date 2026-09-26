local sf2 = require("sf2")
local names = { "Blade", "Spear", "Staff", "Claws", "Knives", "Axe",
    "Katana", "Tonfas", "Sais", "Hammer", "Kusarigama", "Nunchaku" }

sf2.story.on("scene_enter", function(event)
    if event.scene == "fight" then return end
    local hidden, disabled = false, false
    local cells = {}
    for i, name in ipairs(names) do
        cells[i] = { id = "item_" .. i, kind = "button", text = name }
    end
    sf2.ui.open {
        id = "grid_showcase", mount = "modal",
        root = { id = "root", kind = "column", width = 560, height = 396, gap = 8,
            children = {
                { id = "title", kind = "text", text = "Equipment selector", width = 560, height = 44 },
                { id = "help", kind = "text", text = "Choose a name. Scroll for more choices.", width = 560, height = 32 },
                { id = "viewport", kind = "scroll", width = 560, height = 114, children = {
                    { id = "choices", kind = "grid", width = 560, height = 222,
                        columns = 3, cell_width = 180, cell_height = 48, gap = 10, children = cells },
                } },
                { id = "selection", kind = "text", text = "Preview only: nothing is equipped.", width = 560, height = 40 },
                { id = "controls", kind = "row", width = 560, height = 48, gap = 10, children = {
                    { id = "hide", kind = "button", text = "HIDE BLADE", width = 275, height = 48 },
                    { id = "disable", kind = "button", text = "DISABLE SPEAR", width = 275, height = 48 },
                } },
                { id = "close", kind = "button", text = "BACK", width = 560, height = 48 },
            },
        },
        on_click = function(view, id)
            local index = tonumber(id:match("^item_(%d+)$"))
            if index then
                sf2.ui.set_text(view, "selection", "Selected: " .. names[index])
            elseif id == "hide" then
                hidden = not hidden
                sf2.ui.set_visible(view, "item_1", not hidden)
                sf2.ui.set_text(view, "hide", hidden and "SHOW BLADE" or "HIDE BLADE")
            elseif id == "disable" then
                disabled = not disabled
                sf2.ui.set_enabled(view, "item_2", not disabled)
                sf2.ui.set_text(view, "disable", disabled and "ENABLE SPEAR" or "DISABLE SPEAR")
            elseif id == "close" then
                sf2.ui.close(view)
            end
        end,
    }
end)
