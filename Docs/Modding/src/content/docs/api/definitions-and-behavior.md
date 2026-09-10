---
title: Definitions and behavior
description: Understand which code runs during loading and which code runs during a fight.
---

A mod usually has two parts: definitions that describe its content, and functions that respond to gameplay.

## Definitions describe what exists

Your entry script runs when Eclipse loads the selected mods. Use it to register items, shop listings, battles, opponents, quests, perks, and other content. Lookups and registrations return handles you pass into later definitions. Define dependencies before the content that uses them.

Registration does not mean the player has received an item or that a fight is visible: a weapon needs a shop listing or reward, and a battle needs the appropriate quest/map and fight connections.

## Callbacks respond to gameplay

A callback is a function you give the API to call later, such as when a round starts. Register it as part of a behavior, then attach the behavior to a perk or enchantment that a fighter can actually have.

```lua
-- A callback field inside a behavior definition:
on_round_begin = function(params, fighter, event)
    if fighter.health > 0 then
        fighter:change_health(params.heal)
    end
end
```

This fragment belongs inside a behavior table, not alone at the top of a script. It requires a declared `heal` parameter and `combat.change_life`. See [Behavior instances](../behavior-instances/) for complete definitions and [Combat callbacks](../combat-callbacks/) for available events.

## Choose where state belongs

| Need | Use |
| --- | --- |
| A number that configures each perk or enchantment | Behavior `parameters`. |
| A value that changes during a round or fight | Behavior instance state with the corresponding lifetime. |
| Saved behavior state | Saved behavior state, where supported for player-owned instances. |
| General mod progress between sessions | The [mod state API](../mod-state/). |
| An achievement total | [Counters and achievements](../achievements/). |

A local Lua variable is useful for connecting definitions. It is not automatically a saved value. Use a callback's fighter handle only during the callback that supplied it.

## Choose the appropriate system

Quest tables describe supported events, conditions, and actions. Move triggers describe native animation actions. Tactics configure the existing opponent AI. These tables do not accept arbitrary new Lua callback names. For custom combat logic, use ordinary Lua functions and the documented fighter methods.
