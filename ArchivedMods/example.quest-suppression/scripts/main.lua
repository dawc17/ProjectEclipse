local sf2 = require("sf2")

sf2.quests.register {
    id = "old_introduction", place = "map", events = { "session" },
    actions = { { type = "dialog", lines = { "Old introduction: suppression failed." } } },
}
sf2.quests.suppress { target = "example.quest-suppression:quests/old_introduction" }
sf2.quests.register {
    id = "new_introduction", place = "map", events = { "session" },
    actions = { { type = "dialog", lines = { "Replacement introduction: quest suppression is active." } } },
}
