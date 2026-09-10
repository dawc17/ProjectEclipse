---
title: Creating an offline raid
description: Connect a boss fight, multi-bar health, rewards, and a local raid mode.
---

An offline raid is a local encounter against a boss. The boss can have several health bars, and the player must defeat it to win; surviving until timeout is not a victory. Each retry begins a fresh health pool.

## Connect the content

Use the [content reference](../content-graph/) to register these definitions in order:

1. A zone and a battle with `type = "raid"`.
2. A warrior for the boss, using a valid template, equipment, and tactic.
3. An empty loss reward and a victory reward.
4. A fight that connects the battle, boss, arena, and rewards.
5. A mode registered with `sf2.raids.register` that includes the fight.
6. Quest/map actions that let the player reach the raid.

A raid battle without its offline raid-mode registration is rejected. A registered boss alone does not create a playable map entry.

## Give the boss multiple health bars

Set `health_bars` on the warrior definition. It accepts an integer from `0` to `10000`. `0` or omission inherits the template; `1` means one bar; `10` means ten total bars including the currently visible one.

Damage carries across bars and the boss dies when the total pool reaches zero. The HUD displays the bar and remaining-bar count. This is separate from the temporary damage-reduction shields supplied by fighter methods.

For a long boss fight, set `rounds = 1` and `round_time = 300` in the fight definition. Choose health, equipment, and time together so the fight is achievable with the intended player gear.

## Add rewards and entry rules

A reward definition can contain `gems` from `0` to `1000000`, default `0`. Put the empty loss reward first and the victory reward next in the fight's reward list. Rewards are granted through normal fight completion and save handling.

The [mode API](../events-and-modes/) supports free entry or an optional entry-item requirement. Set `hard_mode = true` to use the existing Power Mode map filter; special names are not required. Each mod's IDs keep its zones, bosses, keys, and progress distinct.

## Connect raid quests

The [quest API](../quests/) supports `raid_fight_enter`, `raid_fight_end`, `raid_enter`, `raid_end`, `reset_mode`, `raid_map_enter`, `raid_floor_changed`, and `show_raid_loot`. `raid_enter` means entering an attempt; `raid_end` means completing the sequence. Use the event that matches the progress you want to record.

## Test the player experience

Check map visibility, entry, boss health bars, victory, loss, timeout, rewards, and retry. If entry consumes an item, also check insufficient items and interrupted attempts. Verify that rewards appear once and remain correct after restarting. A script that loads successfully still needs these in-game checks.
