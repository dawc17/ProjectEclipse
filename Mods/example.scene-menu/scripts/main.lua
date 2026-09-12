local sf2 = require("sf2")

sf2.story.on("scene_enter", function(event)
    if event.scene == "fight" then return end
    sf2.ui.open {
        id = "scene_menu", mount = "menu",
        root = { id = "root", kind = "column", width = 340, height = 392, gap = 8, children = {
            { id = "title", kind = "text", text = "Travel", width = 340, height = 48 },
            { id = "map", kind = "button", text = "MAP", width = 340, height = 52 },
            { id = "shop", kind = "button", text = "SHOP", width = 340, height = 52 },
            { id = "profile", kind = "button", text = "PROFILE", width = 340, height = 52 },
            { id = "dojo", kind = "button", text = "DOJO", width = 340, height = 52 },
            { id = "back", kind = "button", text = "BACK", width = 340, height = 52 },
            { id = "status", kind = "text", text = "", width = 340, height = 36 },
        } },
        on_click = function(view, id)
            if id == "back" or sf2.scenes.open(id) then
                sf2.ui.close(view)
            else
                sf2.ui.set_text(view, "status", "Unavailable right now")
            end
        end,
    }
end)
