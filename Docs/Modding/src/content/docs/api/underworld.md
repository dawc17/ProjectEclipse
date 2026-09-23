---
title: Underworld pages
description: Add boss pages to the Underworld map, pair normal and Power Mode entries, and control the map toggle and focus.
---

The **Underworld** is the game's second map: a column of boss pages the player
opens with the round toggle button on the story map. Its pages hold single boss
fights and survival gauntlets. A **Power Mode** checkbox on that map swaps each
boss for a harder variant.

Your mod can add its own Underworld pages. They appear alongside the game's raid
pages and use ordinary battles, fights, warriors and rewards. This page shows how
the pieces connect and documents the `sf2.underworld` functions. Examples assume
`local sf2 = require("sf2")`.

## Build an Underworld page

1. Register the page with `underworld = true` in
   [`sf2.zones.register`](../content-graph/#sf2zonesregister). Raid map art such
   as `file = "Raid1.1"` fits these pages.
2. Register each boss as a battle on that page, usually with
   `type = sf2.battles.FINAL` (or `SURVIVAL` for a gauntlet).
3. For a normal/Power Mode pair, register two battles at the same position with
   [`power_mode = "normal"` and `power_mode = "power"`](../content-graph/#power-mode-entries).
   Entries without `power_mode` are shown in both states.
4. Optionally give the button your own art with
   [`icons`](../content-graph/#map-button-sprites).
5. Register the boss (often from a shared
   [`sf2.warriors.register_template`](../content-graph/#sf2warriorsregister_template)),
   its rewards (forge materials use
   [`currencies`](../content-graph/#sf2rewardsregister)), and a fight connecting them.

```lua
local depths = sf2.zones.register { id = "depths", file = "Raid1.1", underworld = true }
local boss_template = sf2.warriors.register_template {
    id = "ember_boss", template = sf2.warriors.get_template("core:warrior-templates/default"),
    voice = "Male", health_bars = 10, skeleton = "SkeletonHeavy",
    items = { sf2.items.get("core:items/weapon/WEAPON_KATANA") },
}
local function boss(id, mode, level, bars)
    local battle = sf2.battles.register {
        id = id, zone = depths, type = sf2.battles.FINAL, x = 360, y = 820, power_mode = mode,
    }
    local warrior = sf2.warriors.register { id = id .. "_boss", template = boss_template, level = level, health_bars = bars }
    local loss = sf2.rewards.register { id = id .. "_loss" }
    local win = sf2.rewards.register {
        id = id .. "_win", gems = 5, currencies = { { currency = "ForgeMaterial1", expected = 40 } },
    }
    sf2.fights.register {
        id = id .. "_fight", battle = battle, rounds = 1, round_time = 300,
        warriors = { warrior }, rewards = { loss, win },
    }
    return battle
end
local ember = boss("ember", "normal", 40, 10)
local ember_power = boss("ember_power", "power", 60, 20)
```

Registering a battle does not reveal it in an existing save. Reveal entries with
[`sf2.battles.reveal`](../content-graph/#sf2battlesreveal) or a quest, exactly as
on the story map.

## Control the toggle and the focus

The story map shows the Underworld toggle by default. A mod that wants the
Underworld to be discovered later can hide the toggle until its story reveals
it, and can choose which boss the Underworld map opens on.

## sf2.underworld.set_toggle_visible

Show or hide the map's Underworld toggle button.

**Signature:** `sf2.underworld.set_toggle_visible(visible) -> boolean`

**Returns:** `true` when the current map applied the change; `false` when the
map is not ready: outside the map scene, during a scene or profile change, while
an encounter is being prepared, or while native input is blocked (for example by
a lock screen).

**When:** In a callback while the map scene is open, such as a `scene_enter`
event for `"map"` or a story dialog's `on_complete`. Calls from UI cleanup
callbacks raise an error.

**Requires:** `story.progression` and a strict boolean.

```lua
sf2.story.on("scene_enter", function(event)
    if event.scene == "map" and not sf2.state.get("depths_found") then
        sf2.underworld.set_toggle_visible(false)
    end
end)
```

The change lasts until the map is rebuilt: the next time the map scene opens,
the toggle is visible again unless you hide it again. Hide it on every
`scene_enter` for `"map"` while it should stay hidden, and store your own
"revealed" flag in [mod state](../mod-state/). Hiding the toggle does not lock
any battle, and it also hides the way to the game's own raid pages.

## sf2.underworld.set_focus

Choose the boss the Underworld map shows when the player next opens it.

**Signature:** `sf2.underworld.set_focus(battle) -> boolean`

**Returns:** `true` when the focus was stored; `false` when no profile is loaded,
a profile change is in progress, or the battle is not on one of your
Underworld pages.

**When:** A callback after the profile has loaded, typically right before you
show the toggle.

**Requires:** `story.progression` and a battle handle registered by this mod on a
zone with `underworld = true`. Strings, core battles and other mods' battles are
rejected.

```lua
-- ember was registered on an Underworld page during loading.
if sf2.underworld.set_toggle_visible(true) then
    sf2.underworld.set_focus(ember)
end
```

This stores the saved Underworld focus; it does not switch maps, reveal or unlock
the battle, or select it on the story map. Use
[`sf2.battles.focus`](../content-graph/#sf2battlesfocus) to select an entry that
is already visible on the current map.

## Tell the story

Boss introductions and after-fight lines use the regular story tools:

- [`sf2.story.before_fight`](../story/#sf2storybefore_fight) holds the fight while
  a sequence of [`sf2.ui.story_dialog`](../ui/#sf2uistory_dialog) cards plays, and
  starts it from the last card's button.
- The `battle_result` event reports `"win"` or `"loss"` (a surrender is reported
  separately as `"surrender"`), so a mod can queue a one-time line and show it
  on the next `scene_enter` for `"map"`.
- [`sf2.ui.act_screen`](../ui/#sf2uiact_screen) shows full-screen chapter text.

## Limits

- Only zones registered by your mod can carry `underworld = true`; core raid
  pages keep their own content.
- Power Mode filtering applies only on Underworld pages.
- Map buttons need their own sprites for both the normal and selected states;
  locked art falls back to the game's lock icon when `locked` is omitted.
- The Underworld functions change map presentation only. They do not grant
  rewards, reset attempts, or change the game's online raid services.
