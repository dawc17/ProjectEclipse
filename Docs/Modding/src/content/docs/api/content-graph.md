---
title: Maps, opponents, fights, and rewards
description: Build a map entry, define its opponent and rules, and award the correct result.
---

A **zone** is a map page. A **battle** is a selectable entry on that page.
A **warrior** describes an opponent. A **fight** connects a battle to opponents,
rules, round settings, and rewards. Create these in that order so references exist
before you use them. See [Your first battle](../../guides/first-battle/) for a
complete connected example.

All registration and lookup functions here use `content.register` and run during
the entrypoint. Cross-mod content requires the corresponding dependency.
Tables reject unknown fields. Examples assume `local sf2 = require("sf2")`.

Some presentation fields in this part of the API still take **strings**, such as
`"my.mod:localization/battle.title"`, rather than localization handles. Respect
the field type: a handle cannot be substituted for a string.

## sf2.zones.register

Create a map page for your battles.

**Signature:** `sf2.zones.register { id, file?, start? }`

**Requires:** `content.register`.

**When:** Entrypoint, before its battles.

**Returns:** A zone handle.

`id` is a required local ID. `file` is a string naming existing map art, default
`""`; `start` is a boolean defaulting to `false`. Reuse a known map file when
starting out; a zone ID is not the map-art filename.

```lua
local zone = sf2.zones.register { id = "training", file = "Map1.1", start = false }
```

## sf2.zones.get

Obtain an existing zone's handle instead of creating a new page.

**Signature:** `sf2.zones.get(reference)`

**Requires:** `content.register` and a dependency on an external owner.

**When:** Entrypoint.

**Returns:** A zone handle; a missing or inaccessible ID raises an error.

```lua
-- After the registration above, retrieve this mod's zone by local ID:
local same_zone = sf2.zones.get("training")
```

For another owner, use its complete `owner:zones/id` definition ID.

## sf2.battles.register

Create the map entry that the player selects to open a fight.

**Signature:** `sf2.battles.register(definition)`

**Requires:** `content.register`.

**When:** Entrypoint, after the zone.

**Returns:** A battle handle.

| Field | Type | Default | Meaning |
| --- | --- | --- | --- |
| `id` | String | Required | Local battle ID. |
| `zone` | Zone handle | Required | Owning map page. |
| `type` | Battle constant/string | Required | Game battle category; start with `sf2.battles.STORY`. |
| `x`, `y` | Integers | `0` | Placement on the map page. |
| `alias`, `title`, `description` | Strings | `""` | Presentation/localization references. |
| `icon`, `icon_atlas`, `preview` | Strings | `""` | Existing map/panel art identifiers. |
| `eclipse_toggle_name` | String | `""` | Eclipse-mode toggle presentation name. |
| `location`, `music` | Strings | `""` | Arena and music references used by the panel/content. |
| `reward_image` | String | `""` | Reward presentation image reference. |
| `show_resistance` | Boolean | `false` | Whether to show resistance presentation. |

```lua
local battle = sf2.battles.register {
    id = "training_battle", zone = zone, type = sf2.battles.STORY,
    title = "my.mod:localization/battle.title", x = 0, y = 0,
}
```

Constants on `sf2.battles` are `DUMMY`, `TUTORIAL`, `CHALLENGE`, `BOSSES`,
`TOURNAMENT`, `STORY`, `SURVIVAL`, `FRIENDLY`, `AUTO`, `AI`, `HIDDEN`, `FAKE`,
`PVP`, `FINAL`, and `FINAL_TITAN`. The string `"raid"` is also accepted for
registered offline raids. A category constant does not implement an online
service or arbitrary game mode; use the supported offline mode APIs.

Create a fight for the battle and reveal it through a quest. Merely registering
an entry does not guarantee that an existing story save will display it.

## sf2.warriors.get_template

Get an existing opponent template as the starting point for a new warrior.

**Signature:** `sf2.warriors.get_template(reference)`

**Requires:** `content.register` and the template owner's dependency.

**When:** Entrypoint.

**Returns:** A warrior-template handle; missing IDs raise errors.

```lua
local template = sf2.warriors.get_template("core:warrior-templates/default")
```

Template handles differ from warrior handles; pass this one to the `template`
field of `sf2.warriors.register`.

## sf2.warriors.register

Define an opponent, optionally inheriting from a core template.

**Signature:** `sf2.warriors.register(definition)`

**Requires:** `content.register`.

**When:** Entrypoint, before registering the fight.

**Returns:** A warrior handle.

| Field | Type | Default | Meaning |
| --- | --- | --- | --- |
| `id` | String | Required | Local warrior ID. |
| `template` | Warrior-template handle | Omitted | Template to inherit from. |
| `first_name`, `last_name` | Strings | `""` | Name/localization references. |
| `avatar`, `voice` | Strings | `""` | Existing portrait/voice identifiers. |
| `level` | Integer | `0` | Opponent level setting, 0–10,000; use an explicit level in a new opponent. |
| `tactic` | Tactic handle or string | `""` | Registered tactic or existing tactic name. |
| `group` | String | `""` | Opponent content group. |
| `random` | Integer | `0` | Native random-selection setting. |
| `items` | Item-handle array | Empty | Equipment/content loadout. |
| `perks` | Perk-handle array | Empty | Active opponent perks. |
| `attributes` | Name-to-number table | Empty | Finite native attribute values. |
| `attribute_alignments` | Alignment array | Empty | Rows described below. |
| `body_model` | Model handle | Inherit skeleton | Native body model, including its ordered point rig. |
| `skin_models` | Model handle array | Empty | Up to 16 native geometry overlays, appended after equipment. |
| `health_bars` | Integer | `0` | Additional health-pool configuration; `0` keeps the template setting; `1` explicitly selects one pool. |

```lua
local opponent = sf2.warriors.register {
    id = "sparring_partner",
    template = sf2.warriors.get_template("core:warrior-templates/default"),
    first_name = "my.mod:localization/opponent.name",
    level = 1,
    tactic = "Standard",
}
```

Alignment rows require numeric `factor` and `shift`; optional `priority` defaults
to `0` and `mode` to `"all"` (`"normal"` and `"eclipse"` are also accepted).
Use documented native attributes for the relevant content; adding a made-up
attribute name does not create a new mechanic. Template inheritance and native
attribute handling can affect the result, so test custom balance in a fight.

For a Blender import, animation bake, geometric skin export and playable move example, follow [Character authoring](../../guides/character-authoring/). Body and skin assets must satisfy the native point-rig contract; these fields do not load arbitrary FBX files. The warrior handle also scopes moves through a `character` condition. Changing model bindings changes the saved content fingerprint.

`health_bars` is bounded to 0–10,000. Zero inherits the template; 1 means one pool. Values above 1 set the **total** number of
bars, including the active bar. These are separate from temporary Lua damage
shields. Duplicate loadout items or perks are rejected.

## sf2.rewards.register

Create a reward that a fight can grant through its normal result/save flow.

**Signature:** `sf2.rewards.register { id, items?, choices?, gems? }`

**Requires:** `content.register`.

**When:** Entrypoint, before the fight uses the reward.

**Returns:** A reward handle.

| Field | Type | Default | Meaning |
| --- | --- | --- | --- |
| `id` | String | Required | Local reward ID. |
| `items` | Grant array | Empty | Guaranteed item grants: `{ item = handle, upgrade = 0 }`. |
| `choices` | Choice-group array | Empty | Each group has `items`, a weighted candidate array. |
| `gems` | Integer, 0–1,000,000 | `0` | Fixed gem reward through normal acquisition. |

A candidate row is `{ item = handle, upgrade = 0, weight = 1 }`. Upgrade is a
nonnegative integer. Weights must be finite and greater than zero. Each group
selects an item from its candidates. This is a content reward, not arbitrary
script access to a currency balance.

Legacy XML reward items also support `UpgradeLevel`, which is an encoded native
upgrade level rather than the ordinal `UpgradeNumber` used by Lua's `upgrade`.
For example, `UpgradeLevel="?Player[].Level*100"` evaluates through the native
player-level expression resolver when the reward is selected. Selection uses the
native quest-grant lookup: the first upgrade entry whose encoded level is at
least the requested value. Negative values and unavailable upgrades fail reward
selection; specifying both upgrade attributes fails parsing. Existing
`UpgradeNumber` selection and clamping are unchanged. This compatibility support
does not expose XML expressions through the Lua API or provide a complete
interactive lottery workflow.

```lua
local no_reward = sf2.rewards.register { id = "no_reward" }
local victory_reward = sf2.rewards.register { id = "victory_reward", gems = 5 }
```

## sf2.fights.register

Connect a battle to opponents, rounds, rules, and rewards.

**Signature:** `sf2.fights.register(definition)`

**Requires:** `content.register`.

**When:** Entrypoint, after the referenced definitions.

**Returns:** A fight handle.

| Field | Type | Default | Meaning |
| --- | --- | --- | --- |
| `id` | String | Required | Local fight ID. |
| `battle` | Battle handle | Required | Map entry this fight belongs to. |
| `warriors` | Warrior-handle array | Empty | Opponent definitions. |
| `rules` | Rule-handle array | Empty | Fight rules from `sf2.rules`. |
| `rewards` | Reward-handle array | Empty | Reward slots indexed by wins; see below. |
| `rounds` | Integer, 1–100 | `3` | Number of rounds. |
| `round_time` | Integer, 1–86400 | `99` | Time limit in seconds. |
| `replays`, `replay_interval`, `power` | Nonnegative integers | `0` | Native replay, interval, and power settings. |
| `location`, `music` | Strings | `""` | Arena name and music reference. |
| `evaluated_rating` | Finite number | `-1` | Native rating setting. |
| `health_recovery` | Finite nonnegative number | `1` | Native health recovery setting. |
| `description`, `reward_image` | Strings | `""` | Presentation references. |
| `locked` | Boolean | `false` | Initial lock setting. |

```lua
local fight = sf2.fights.register {
    id = "training_fight", battle = battle,
    rounds = 1, round_time = 99,
    location = sf2.locations.name(arena), -- arena: registered location handle
    warriors = { opponent },
    rewards = { no_reward, victory_reward },
}
```

**Reward order matters.** The first Lua element is the zero-win result slot.
For a one-round fight, use `{ no_reward, victory_reward }` so a loss does not
accidentally receive the victory reward. Do not grant the same item again in a
fight-end quest unless you intend a second reward.

For scheduled or replayable content, use [events and modes](../events-and-modes/)
to manage sequence progress and entry requirements.

## sf2.fights.patch

Replace supported fields on an existing registered fight.

**Signature:** `sf2.fights.patch { target, description?, rounds?, round_time?, location?, music?, warriors?, reward_drops?, rules?, append_rules? }`

**Requires:** `content.patch`, and a dependency on the target owner.

**When:** Entrypoint, after the target mod has committed its definitions.

**Returns:** `nil`.

`target` is a fight definition ID string. Supply at least one changed field.
`description` is a string; `rounds` is 1–100; `round_time` is 1–86400 seconds.
Since API 0.10, `location` and `music` accept nonempty recovered runtime names,
as in fight registration. For a registered custom location, use
`sf2.locations.name(location)`. This changes the encounter's presentation; it does
not register or validate the existence of an asset named by that string.

Use `append_rules = { rule, ... }` to keep existing rules and add your own in
array order. Use `rules = { rule, ... }` to replace the entire rule list, including
native rules on a core encounter. `rules = {}` explicitly clears it; an empty
`append_rules` is rejected. The two forms are mutually exclusive, accept at most
100 registered rule handles (also bounded to 100 after appending), and reject
duplicate handles. Handles may be registered earlier in this same entrypoint.
Both static rules and [Lua behavior rules](../rules/#sf2rulesbehavior) are supported.

Since API 0.32, `warriors = { opponent, ... }` replaces the entire opponent list.
Supply 1–100 unique registered warrior handles, in encounter order. An empty list
is rejected. These are warrior handles, not warrior-template handles or string IDs;
you may register them earlier in the same entrypoint. Omitting `warriors` preserves
the existing opponents. This field uses the normal warrior authoring contract,
including templates, equipment and custom character models.

```lua
-- opponent is a handle returned by sf2.warriors.register earlier in this script.
sf2.fights.patch {
    target = "core:fights/zone_1/boss_lynx/1",
    warriors = { opponent },
}
```

Opponent replacement leaves encounter identity, rewards and saved campaign
progress intact; it does not unlock or reset the fight. Two patches replacing
the same fight's opponents conflict. Opponent order contributes to the content
fingerprint. Full-game opponent presentation and campaign replay acceptance
remain pending; automated checks cover Lua registration, conflicts, preservation
of other fields and native XML projection.

Appending preserves native XML rules that have no public handle. Their native
execution remains unchanged; Lua rules run through the documented combat callback
dispatcher. Array order orders Lua rules relative to other Lua rules, not native
engine actions across different callback stages.
The target must already exist in the exposed catalog: this is not an arbitrary
XML patch and not a setter for a newly staged fight in the same transaction.

```lua
-- Replace the owner and ID with an existing fight from a declared dependency.
sf2.fights.patch {
    target = "other.mod:fights/trial",
    round_time = 120,
}
```

Competing changes to the same semantic field conflict. Different supported
fields can coexist. Unsupported fields, missing targets, or undeclared owners
fail registration.

Fight fields from one call are staged together: a validation failure discards
that call's fields, preserving earlier successful calls. A conflict discovered
when committing the mod still rejects the entire registration transaction.
The current Lua sandbox does not provide `pcall`/`xpcall`; an entrypoint error
aborts loading rather than letting the script recover and continue.

`rules` and `append_rules` address the same semantic field: competing rule-list
patches conflict, including two append requests. A conflict rolls back the whole
registration transaction. Encounter IDs and saved campaign
progress are preserved. The patch is reapplied from base definitions at startup;
disabling the mod and restarting restores base content. Content fingerprints
distinguish appended rules from replacement, including an empty replacement.

```lua
local guard = sf2.rules.behavior { id = "guard", behavior = my_behavior }
sf2.fights.patch {
    target = "core:fights/zone_1/boss_lynx/1",
    append_rules = { guard },
    location = "dojo",
    music = "fight1_samurai_spirit",
}
```

`my_behavior` above must be a registered behavior handle. The complete
`Mods/example.core-fight` mod demonstrates a health-dependent guard on an existing
campaign opponent. These fields do not provide a generic XML patch interface or
economy overrides. Reward editing is limited to the item-drop scopes below.


### Editing encounter item drops

Since API 0.33, `reward_drops` accepts 1–100 scoped edits. Each edit replaces the
selected scope's direct item grants and item-only weighted choices. It does not
replace the entire native reward row.

| Field | Type | Default | Meaning |
| --- | --- | --- | --- |
| `wins` | Integer | Required | Existing zero-based reward slot, 0–100. Slot 0 is the zero-win result; slot 1 is one win. The slot must exist on the target fight. |
| `reward` | Reward handle | Required | Registered reward supplying the replacement items/choices. Its `gems` must be zero. An empty reward clears direct drops. |
| `mode` | `"all"`, `"normal"`, `"eclipse"` | `"all"` | Select the shared row or a mode-specific addition. |
| `min_level` | Integer | Omitted | Inclusive lower player-level bound, 1–10,000. |
| `max_level` | Integer | Omitted | Inclusive upper player-level bound, 1–10,000; must be at least the minimum. |

With neither level bound, only the selected scope's direct drops change. With
one or both bounds, the edit selects a native level row with exactly those bounds,
creating it when absent. Omitted bounds stay unbounded. Other level rows remain
unchanged, including overlapping ranges: native settlement adds every matching
row. Duplicate matching scopes are rejected as ambiguous.

**Modes are additive.** `"all"` edits the shared reward, which applies in both
modes. `"eclipse"` edits only the extra Eclipse reward; shared items still apply.
A missing mode scope is created within the existing result slot.

A mode scope does not select a different battle. Some vanilla encounters use a
separate Eclipse battle through `EclipseToggleName`. Lynx, for example, links
`BOSS_LYNX` to `BOSS_LYNX_ECLIPSEMODE`: target the latter for the replay encounter,
as below. Patching one fight does not automatically patch its linked counterpart.

```lua
local sf2 = require("sf2")
local prize = sf2.rewards.register {
    id = "eclipse_prize",
    items = {{ item = sf2.items.get("core:items/weapon/WEAPON_C2_Z2_MONK_KATAR") }},
}
sf2.fights.patch {
    target = "core:fights/zone_1/boss_lynx_eclipsemode/1",
    reward_drops = {{ wins = 1, mode = "eclipse", reward = prize }},
}
```

The example requires `content.register` and `content.patch`, plus
a dependency on `core`. It changes a reward definition, not completion conditions:
it does not unlock the fight, reset progress or make an item a one-time grant.
Native repeat/reward settlement rules still apply. The native result handler skips
equipment the profile already owns; test with an unowned item or a separate test
profile. Mod-owned consumables retain their supported repeat-grant behavior.

Money, premium currency, experience, reward scaling, lotteries, resistance and
other scopes are preserved. A choice containing non-item outcomes is rejected:
changing its item weights would also change currency probabilities. This operation
does not edit lottery contents or nested item metadata/enchantments.

Edits to the same fight/result/mode/exact-level scope conflict, including repeated
edits by one mod. Distinct scopes coexist. All edits in one call share its rollback
boundary. Content fingerprints include the scopes and reward identities; disabling
the mod and restarting rebuilds the original definitions.

Lua registration, scoped projection and rollback have automated coverage. A native
fixture also exercises the production item builders, item parser, weighted choice,
mode/level reward composition, repeated evaluation and native result item selection,
with controlled profile, inventory lookup, item upgrade and numeric services. It verifies preserved currency values, item upgrade/drop flags
and independent lottery slot lists. Full-game reward display, inventory granting,
replay eligibility and save/reload acceptance remain pending; these fixtures alone
do not establish DE reward parity.
