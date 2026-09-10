---
title: Your first battle
description: Connect a map page, opponent, arena, fight, and quest in one complete example.
---

This tutorial adds a one-round sparring fight to the mod from [Your first weapon](../first-weapon/). Finish that tutorial first so you know your manifest, localization, and script load correctly. No additional capabilities or asset downloads are required: the fight reuses core art and a core warrior template.

## What you are connecting

| Part | Purpose |
| --- | --- |
| Zone | A map page that contains your battle entry. |
| Battle | The entry the player selects. |
| Location | The arena background and fighter positions. |
| Warrior | The opponent's template, level, and tactic. |
| Rewards | What the player receives for each result. |
| Fight | The connection between the battle and those gameplay definitions. |
| Quest | Reveals the battle when a map session begins. |

Keep the registrations in this order where one definition depends on another. The variable names below hold handles returned by earlier calls.

## 1. Add the text

Append these lines to `localizations/eng.toml`:

```toml
battle.training = "Sparring Practice"
battle.description = "Win one round against a training opponent."
opponent.name = "Sparring Partner"
```

The battle and warrior presentation fields below expect qualified **strings**, while the weapon's `display_name` expects a localization **handle**. The reference states which form each field uses.

## 2. Add the arena

Append this to `scripts/main.lua`, after the existing weapon script. Keep its existing `local sf2 = require("sf2")` at the top.

```lua
local arena = sf2.locations.register {
    id = "training_arena",
    width = 1936, height = 512,
    floor = 80, wall = 200,
    layers = {
        {
            type = 1,
            images = { {
                sprite = sf2.assets.sprite(
                    "core:Textures/Locations/battlefield/battlefield_bg1.back_1"),
                width = 1936, height = 1024,
            } },
        },
        {
            type = 2,
            fighters = {
                player_x = 868, player_y = -94,
                enemy_x = 1068, enemy_y = -94,
            },
        },
    },
}
local arena_name = sf2.locations.name(arena)
```

The first layer draws the background. The second positions the fighters. These coordinates come from the matching core battlefield layout; when using different art, test the floor and camera framing in game.

## 3. Add the map entry and opponent

Append:

```lua
local zone = sf2.zones.register {
    id = "training_zone", file = "Map1.1", start = false,
}

local battle = sf2.battles.register {
    id = "training_battle", zone = zone, type = sf2.battles.STORY,
    x = 0, y = 0,
    alias = sf2.mod.id .. ":localization/battle.training",
    title = sf2.mod.id .. ":localization/battle.training",
    description = sf2.mod.id .. ":localization/battle.description",
    location = arena_name,
}

local opponent = sf2.warriors.register {
    id = "sparring_partner",
    template = sf2.warriors.get_template("core:warrior-templates/default"),
    first_name = sf2.mod.id .. ":localization/opponent.name",
    level = 1,
    tactic = "Standard",
}
```

`sf2.mod.id` reads your manifest ID, so the localization references follow your namespace if you change it. The core template supplies the opponent's starting definition. `Standard` selects an existing AI tactic. You can add compatible equipment through `items = { ... }` later.

## 4. Connect the fight

Append:

```lua
local no_reward = sf2.rewards.register { id = "training_no_reward" }

local fight = sf2.fights.register {
    id = "training_fight",
    battle = battle,
    warriors = { opponent },
    rounds = 1,
    round_time = 99,
    location = arena_name,
    rewards = { no_reward, no_reward },
}
```

This practice fight awards nothing on either result. The first reward slot is for **zero wins**. The second is for one win. To add a victory prize later, register a separate reward and put its handle in the second slot. See [Rewards and fights](../../api/content-graph/#sf2rewardsregister) for item grants and gem rewards.

## 5. Make the battle reachable

Append:

```lua
sf2.quests.register {
    id = "reveal_training",
    place = "map",
    events = { "session" },
    actions = {
        { type = "show_battle", battle = battle, locked = false },
        { type = "map_focus", battle = battle },
    },
}

sf2.log.info("Sparring Practice registered")
```

The quest reveals the battle and focuses the map when its session event runs. For a larger mod, use conditions and progression so the map does not repeatedly focus on your practice battle. Registering the battle alone would not supply that reveal step.

## 6. Play through it

Exit Eclipse completely after saving the files, launch it again, and enter Campaign with the mod enabled. On a profile that can open the map, look for **Sparring Practice**. Check that its panel opens, the arena and opponent appear, and winning or losing returns you cleanly to the game. Confirm the practice fight does not grant a reward.

If the entry is missing, inspect mod errors first, then check the quest event and its `battle` handle. If the fight fails to start, check the arena, template, and localization references. A successful registration is only the first check; this new combination still needs an in-game playtest in your build.

Next, try adding a [fight rule](../../api/rules/), an [opponent perk](../../api/perks-and-enchantments/), or a [mode](../../api/events-and-modes/) that tracks a sequence of fights.
