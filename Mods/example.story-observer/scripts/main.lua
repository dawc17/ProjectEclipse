local sf2 = require("sf2")

sf2.story.on("scene_enter", function(event)
    sf2.log.info("Story Observer scene: " .. event.scene)
end)

-- Observe native activity without changing purchases, forge results or quests.
sf2.story.on("purchase", function(event)
    sf2.log.info("Story Observer purchase: " .. (event.item or "unregistered item"))
end)

sf2.story.on("level_up", function(event)
    sf2.log.info("Story Observer level: " .. event.previous_level .. " -> " .. event.level)
end)

sf2.story.on("enchantment", function(event)
    sf2.log.info("Story Observer enchantment: " .. (event.item or "unregistered item")
        .. " using " .. (event.recipe or "unregistered recipe"))
end)


sf2.story.on("item_acquired", function(event)
    sf2.log.info("Story Observer acquired: " .. (event.item or "unregistered item")
        .. " " .. event.previous_count .. " -> " .. event.count
        .. " (gained " .. (event.count - event.previous_count) .. ")")
end)

sf2.story.on("battle_result", function(event)
    sf2.log.info("Story Observer battle: " .. (event.fight or "unknown encounter")
        .. " " .. event.outcome .. (event.eclipse and " eclipse" or " normal"))
end)
