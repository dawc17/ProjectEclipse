# Loose mods

The browsable modding wiki is maintained in [Docs/Modding](../Docs/Modding/README.md).
Its public reference is maintained against the Lua bindings; this document and
the linked API notes provide engineering detail.

The long-term acceptance target is [complete DE parity through the modding API](DE_PARITY_TARGET.md).
That target describes required coverage; the documentation below describes current support.
The dependency-ordered engineering roadmap is
[DE_API_IMPLEMENTATION_PLAN.md](DE_API_IMPLEMENTATION_PLAN.md). Agents working on
DE parity or Mod API expansion must read both parity documents before editing.

Current public Mod API version: **0.16.0**.

API 0.16 adds safe-area anchors and bounded offsets to custom UI views.

API 0.15 exposes owned custom UI through `sf2.ui`: layout creation, click handlers,
targeted updates and closure. `example.charge-ui` demonstrates a charged attack
with a live HUD. See the public Custom UI reference for supported widgets,
mount/input behavior, lifecycle and verification limits.

API 0.8 adds `sf2.rules.behavior`: directly attach reusable Lua combat behavior
to a fight, with isolated rule/fighter state and target/mode/round filtering.
See [Third Strike Trial](example.battle-rules/README.md), the
[rule reference](../Docs/Modding/src/content/docs/api/rules.md), and the
[engine extensibility review](MOD_ENGINE_EXTENSIBILITY.md).

The [archived DE XML coverage audit](DE_XML_API_GAP_AUDIT.md) records the remaining
gaps. Accepted phase showcases do not imply complete DE parity or support for
every field/event in the recovered XML.

See [Phase 3 API](P3_API.md) and its [numbered playtest guide](example.phase3/README.md).
The [Phase 2 API](P2_API.md) and [showcase](example.phase2/README.md) remain supported.

Place each mod in `Mods/<folder>/` with a `mod.toml` manifest. See `example.weapon`
for the minimal weapon slice, `example.loadout` for armor, helm, ranged, and magic,
and `example.enchantment` for the API 0.3 reusable behavior + typed perk/enchantment slice.
That sample also keeps its older template-derived Lifesteal definitions as an explicit API 0.2
compatibility example.

## Enabling and disabling mods

Open **Mods** directly from the title screen. Toggle the installed mods, then
choose **Apply & Restart**. The game saves and reloads to the title screen;
enter Campaign to load the new selection. **Back / Cancel** discards unapplied
changes. In game, open the main **Menu** and choose **Return to Title** to reach
the mod list again.

Enabling a mod also enables its dependencies. Disabling a dependency disables
the mods that require it. Core remains enabled. Unmet requirements appear under
**Details**, and must be resolved before applying. New mods default to enabled;
selections persist across launches without moving or deleting mod folders.
Mod-owned saved progress is retained while a mod is disabled.

See [mod menu verification](../Tools/MOD_MENU_TESTING.md) for the reload checklist.

## API design: definitions and behavior

Static content belongs in typed definitions; custom procedures belong in Lua
handlers using safe typed capabilities. Lua functions should express branching,
calculations, dynamic choices, and state changes. We should not recreate a generic
action/expression interpreter inside Lua to express that logic.

Phase 1 chiefly demonstrates content registration, validation, references, and
recovered-runtime integration. Its quest condition/action tables and ordinary
tactics are supported compatibility authoring paths. They do not define the
intended shape of future custom quest or AI logic, and the showcase alone does
not demonstrate the full programmable direction.

The current executable foundation is reusable perk/enchantment behavior with
typed instance parameters, `on_fight_begin`, the documented health/magic-charge
capabilities, and mod-owned state. Phase 2 adds combat lifecycle/damage callbacks; Phase 3 adds saved achievement
counters and explicit asset replacement. General quest callbacks and programmable
AI remain roadmap work. The
[design rule and acceptance criteria](DE_API_IMPLEMENTATION_PLAN.md#37-static-definitions-and-programmable-behavior)
guide that work; the API sections below describe what is available today.

## Sprites and textures

Keep image pixels separate from sprite definitions:

```text
assets/
  sprites/
    weapon.asset
  textures/
    weapon.png
```

`sprites/weapon.asset` is a UTF-8, line-based descriptor, not a Unity serialized
asset or Unity `.meta` file:

```ini
type=sprite
texture=textures/weapon.png
pixels_per_unit=100
```

`type` is required; currently `sprite` is the supported descriptor type. The type
comes from this field, not the folder name or an extra `.sprite` filename suffix.
`texture` is required and names a PNG file **relative to the owning mod's
`assets/` root**, including `.png`. Absolute paths, `..`, and namespace-qualified
texture references are rejected. Put PNG textures outside the legacy `sprites/`
folder (normally under `textures/`).

The namespace comes from `mod.toml`'s `id`. Logical asset IDs use the relative
file path without its final extension, normalized to lowercase. For mod
`example.weapon`, the descriptor above is `example.weapon:sprites/weapon`, and
its texture is `example.weapon:textures/weapon`. The sprite's runtime name is
`weapon`. Do not repeat `namespace`, `address`, or `name` in descriptors. Different
folders may contain the same basename, but duplicate logical IDs are rejected.

Optional sprite fields (defaults shown):

```ini
# Omit rect to use the entire image. Coordinates are pixels from the bottom left.
# rect=[0, 0, 128, 128]
pivot=[0.5, 0.5]
border=[0, 0, 0, 0]
pixels_per_unit=100
filter="bilinear"
wrap="clamp"
mipmaps=false
```

`pivot` uses normalized coordinates; `border` is left, bottom, right, top in
pixels. Rectangles and borders must fit within the image/crop. Filter options are
`point`, `bilinear`, and `trilinear`; wrap options are `clamp`, `repeat`, `mirror`,
and `mirror_once`. `type` and `texture` accept bare or double-quoted strings;
`filter` and `wrap` use double quotes. `#` starts a comment outside quotes.
Duplicate or unknown sprite fields are errors.

Multiple descriptors can reference one PNG with different crops, pivots, borders,
and pixels-per-unit values. They share a decoded texture when filter, wrap, and
mipmap settings match. Different texture settings create separate cached texture
instances, so loading one sprite cannot alter another's appearance.

Lua continues to reference the sprite by its logical ID:

```lua
local icon = sf2.assets.sprite("sprites/weapon")
```

For C# consumers, `ModAssetLoader.LoadTexture` and `LoadUnityAsset<Texture2D>` can
load a texture ID directly without creating a sprite.

## Equipment, shop, and logging API

The current external equipment API supports all five primary combat categories:

```lua
local sf2 = require("sf2")

local armor = sf2.items.register_armor {
    id = "eclipse_mantle",
    display_name = sf2.localization.key("armor.eclipse_mantle"),
    icon = sf2.assets.sprite("core:UI/Items/Armor12.img_armor_mantle_of_night"),
    model = sf2.assets.model("core:gamedata/models/mdl_armor_mantle_of_night"),
}

local helm = sf2.items.register_helm {
    id = "eclipse_pumpkin",
    display_name = sf2.localization.key("helm.eclipse_pumpkin"),
    icon = sf2.assets.sprite("core:UI/Items/Helm31.img_helm_hw14_pumpkin"),
    model = sf2.assets.model("core:gamedata/models/mdl_helm_hw14_pumpkin"),
}

sf2.shop.addItem { section = sf2.shop.ARMOR, item = armor, level = 2, price = sf2.price.coins(1) }
sf2.shop.addItem { section = sf2.shop.HELMETS, item = helm, level = 2, price = sf2.price.coins(1) }
```

`register_weapon`, `register_armor`, `register_helm`, `register_ranged`, and
`register_magic` all return opaque item handles. Weapon, ranged, and magic definitions
take a combat `subtype`; normal equipment definitions do **not** take raw damage or
defense values. Power is derived from the shop listing's starting level using the same
vanilla category progression that drives ordinary SF2 upgrades. This prevents a nominal
level-1 mod item from carrying late-game stats and then jumping backwards when the
recovered upgrade system applies its next `*_Bonus` milestone.

The normal vanilla progression profile currently supports levels 1..52 for weapons,
2..52 for armor and helms, and 6..52 for ranged and magic. Early values that predate
the shared upgrade tables use canonical normal-item baselines; later values come directly
from `Weapon_Bonus`, `Armor_Bonus`, `Helm_Bonus`, `Ranged_Bonus`, or `Magic_Bonus`.
For example, a level-12 weapon starts at `WeaponDamage=261`, while level-6 ranged and
magic items start at damage `105`. Custom balance multipliers or custom upgrade profiles
are intentionally deferred to an explicit future API rather than overloading normal
definitions with arbitrary raw stats.

Shop sections are `sf2.shop.WEAPONS`, `ARMOR`, `HELMETS`, `RANGED`, and `MAGIC`.
The registering mod can list only its own item handles and the section must match the
item category.

The tracked `example.loadout` intentionally reuses built-in content through qualified
`core:*` handles. Its armor, helm, ranged, and magic definitions are external, while their
matching vanilla shop sprites and models stay owned by `core` and physically remain in the
TAR/LZ4 provider. Its listings deliberately exercise the normal progression baseline:
armor/helm at level 2 and ranged/magic at level 6. `example.weapon` remains the smaller
proof for mod-owned loose assets.

```lua
sf2.shop.addItem {
    section = sf2.shop.WEAPONS,
    item = weapon, -- handle returned by sf2.items.register_weapon
    level = 1,
    price = sf2.price.coins(1000),
}

sf2.log.debug("debug details")
sf2.log.info("weapon registered")
sf2.log.warn("optional content unavailable")
sf2.log.error("operation failed")
```

Every log entry retains the originating mod ID and its severity. Logging an
error does not throw or roll back registration; use Lua `error(...)` for that.

## Targeted content patches (0.4)

API 0.4 adds the first typed proof of the shared content ownership/patch/conflict
framework. It is deliberately **not** a raw XML replacement API. The currently
shipped patch adapter is localization-only:

```lua
local sf2 = require("sf2")

sf2.localization.patch {
    target = "core:localization/weapon_nunchaku",
    language = "eng",
    value = "Nunchaku",
}
```

The manifest must declare `content.patch`, and the target namespace must be an
explicit dependency. Patching `core:*` therefore requires the normal `core`
dependency:

```toml
capabilities = ["content.patch"]

[[dependencies]]
id = "core"
version = ">=1.0 <2.0"
```

Each committed patch records its owner, target `DefinitionId`, semantic field,
and operation. Two mods replacing the same semantic field, such as
`values/eng` on the same localization definition, produce an explicit conflict
that names both owners. There is no filesystem-order or last-mod-wins fallback.
Non-overlapping fields, for example `eng` and `rus`, can compose.

Core localization projections retain the exact recovered lookup key, so a patch
is applied as a reversible runtime overlay. Removing or disabling the patching
mod reveals the unchanged base string again. The base XML is never rewritten.

The patch policy is deny-by-default: unsupported semantic fields are read-only.
Shared/core economy fields are centrally classified as base-only, including item
prices, upgrade costs, forge/enchantment costs, skip costs, shared currency
values/formulas, and global balance tables. `content.patch` does not grant any
way around that boundary. Prices declared on a new mod-owned item/listing remain
local definition data and do not mutate the shared economy.

This 0.4 slice proves the architecture on already projected core localization
definitions. It does not yet expose arbitrary core strings, generic item/stage/
quest patching, collection child operations, or public content removal. Those
remain roadmap work rather than hidden XML escape hatches.

## Mod-owned state and migrations (0.5)

API 0.5 adds durable typed state owned by one mod namespace. State schemas are
registered during the normal Lua entrypoint, then bound to the current player
only when the recovered roster save is loaded. Lua never receives the raw
`Warrior`, `Roster`, or save XML object.

```lua
local sf2 = require("sf2")

sf2.state.register {
    version = 2,
    fields = {
        victories = { type=sf2.state.INTEGER, required=true, default=0 },
        completed = { type=sf2.state.BOOLEAN, required=true, default=false },
        title = { type=sf2.state.STRING, required=false },
    },
    aliases = {
        wins = "victories",
    },
    tombstones = { "obsolete_flag" },
    migrations = {
        [1] = function(state)
            -- Runs once for schema 1 -> 2 under a bounded instruction budget.
            state.completed = state.completed or false
        end,
    },
}
```

The manifest must declare `state.read` to read state and `state.write` to
register or mutate it. State uses the same bounded `NUMBER`, `INTEGER`,
`BOOLEAN`, and `STRING` value contract as behavior parameters. Required fields
must have defaults so a new save has deterministic initial values.

Once a player save is bound, scripts can use:

```lua
local fights = sf2.state.get("victories")
sf2.state.set {
    victories = fights + 1,
    completed = true,
}
sf2.state.unset("title") -- optional/default-backed fields only
```

`sf2.state.set` validates the entire batch before replacing the saved state, so
one bad field cannot partially commit earlier fields. Migration also works on a
clone and replaces the real `<State>` node only after every N -> N+1 step and
the final schema validation succeed. A failed migration leaves the previous
state XML untouched and produces a `STATE003` diagnostic.

State is stored beneath that mod's existing `EclipseMods/Mod` ownership record.
Removing the mod marks the record inactive but leaves the complete state node
opaque. Saving and reloading without the mod does not delete it; reinstalling
the mod rebinds the preserved values. A saved state schema newer than the
installed mod understands is also left untouched rather than downgraded.
Unknown XML children remain opaque through normal binding/migration. Renames and
intentional removals should use explicit `aliases` and `tombstones`, not rely on
incidental table omission.

The deterministic `contentHash` is now `fingerprint-v6` and includes registered
state schema version, typed fields/defaults, aliases, and tombstones in addition
to the committed content/patch set. Phase 1 stage, quest, item/set/progression/
forge, locale/location/move/tactic definitions are also part of the fingerprint.
Runtime state values themselves are save progress and are not part of the content
fingerprint.

## Phase 1 content graph API (0.5)

API 0.5 now includes the first complete downstream content-authoring layer used by
`Mods/example.phase1`. The public surface is typed and validated. Lua never receives
raw `XmlNode`, stage XML, `Model`, `Roster`, `ListSF`, or arbitrary Unity objects.

### Zones, battles, fighters, rules, rewards, fights

```lua
local zone = sf2.zones.register {
    id = "arena_zone",
    file = "",
    start = false,
}

local battle = sf2.battles.register {
    id = "arena_battle",
    zone = zone,
    type = sf2.battles.STORY,
    x = 0,
    y = 0,
    title = "Arena",
    location = "mountain",
}

local fighter = sf2.warriors.register {
    id = "arena_fighter",
    template = sf2.warriors.get_template("core:warrior-templates/default"),
    first_name = "Arena",
    last_name = "Fighter",
    level = 1,
    tactic = "Standard", -- recovered/core tactic compatibility form
}

local rule = sf2.rules.recharge_magic_each_round {
    id = "arena_recharge",
    target = sf2.rules.ALL,
    mode = sf2.rules.BOTH,
}

local no_win_reward = sf2.rewards.register { id = "arena_no_win", items = {} }
local reward = sf2.rewards.register {
    id = "arena_reward",
    items = { { item = some_item } },
}

local fight = sf2.fights.register {
    id = "arena_fight",
    battle = battle,
    rounds = 1,
    round_time = 99,
    warriors = { fighter },
    rules = { rule },
    rewards = { no_win_reward, reward },
}
```

The stage graph preserves authored child order. A mod may add its own battle under
an existing `core`/dependency zone through the same typed battle registration path;
the child append is provenance-tracked and conflicts if another mod claims the same
semantic child identity. Supported core fight fields can be patched through
`sf2.fights.patch` and removal restores the original recovered source definition.

The currently exposed fight-rule helpers are the recovered, validated subset:
no-perks, require-item, equip-item, avatar/name, perk, recharge-magic-each-round,
attributes, and no-button. Shared currency/cost rule forms remain private.

Rewards currently expose non-economic item grants and weighted item choices. Shared
money/currency reward tuning is intentionally not public.
Reward entries preserve the recovered zero-based win-count slots: a one-round
fight needs an empty slot 0 followed by its victory reward in slot 1. Empty reward
definitions are supported for this purpose; Lua array positions are 1 and 2.

### Quests

`sf2.quests.register` adapts typed Lua data into the recovered quest engine.

This is the shipped Phase 1 compatibility surface. Future custom quest logic
should use Lua handlers and typed operations; quest callbacks are not exposed by
this registration API today.

```lua
sf2.quests.register {
    id = "arena_complete",
    priority = 20,
    place = "fight",
    events = { "fight_end" },
    conditions = {
        {
            op = "eq",
            left = { kind = "event_fight" },
            right = { kind = "fight_id", fight = fight },
        },
    },
    actions = {
        { type = "show_battle", battle = battle, locked = false },
        { type = "toggle_battle", battle = battle, visible = true },
        { type = "start_current_fight" },
        { type = "update_eclipse_battles" },
        { type = "give_item", item = some_item },
    },
}
```

Conditions support typed compare operands plus nested `all`/`any`/`not`. Verified
actions include dialog/story presentation, user-variable writes, battle show/toggle,
map focus, typed fight start/current-fight start, Eclipse-mode refresh/toggle, and
non-economic item grants. Dialog buttons may contain their own typed nested action
sequence. Unsupported service/network/currency actions are not exposed wholesale.

### Non-equipment items, sets, availability, progression, forge structure

```lua
local token = sf2.items.register_consumable {
    id = "token",
    display_name = sf2.localization.key("token.name"),
    icon = sf2.assets.sprite("sprites/token"),
    subtype = "Token",
}

sf2.shop.set_availability {
    item = token,
    visibility = sf2.shop.FORCE_VISIBLE,
}

sf2.itemsets.register {
    id = "token_set",
    title = sf2.localization.key("token_set.title"),
    text = sf2.localization.key("token_set.text"),
    brief = sf2.localization.key("token_set.brief"),
    members = { { item = token } },
}

sf2.forge.register_recipe {
    id = "token_recipe",
    economic_profile = sf2.forge.profile("Simple"),
    items = {
        { equipment = sf2.forge.WEAPON, enchantments = 1 },
    },
    candidates = {
        { perk = some_perk, equipment = sf2.forge.WEAPON, min_level = 1, max_level = 52 },
    },
}
```

Supported non-equipment kinds are consumable, free, and seal. Item availability is
shared by shop and quest availability checks. Progression overlays are targeted
branch replacements rather than arbitrary global progression mutation. Forge recipe
families are structural only: mods reference immutable host economic profiles and
cannot publish forge costs, skip costs, shared currency values, or shared formulas.

See `Mods/P1C_API.md` for the complete P1C authoring contract.

### Locales, audio, locations, moves, triggers, tactics

```lua
local music = sf2.assets.audio("audio/arena")
local anim = sf2.assets.binary("animations/arena_step")

sf2.locales.register {
    id = "arena_english",
    name = "eng_arena",
    locale = "en-x-arena",
}

local location = sf2.locations.register {
    id = "arena_location",
    music = music,
    layers = {
        { images = { { sprite = sf2.assets.sprite("sprites/background") } } },
        { type = 2, fighters = { player_x = 868, player_y = -94, enemy_x = 1068, enemy_y = -94 } },
    },
}

local template = sf2.moves.register_template {
    id = "arena_step_template",
    core_templates = { "ForwardStep" },
}

local move = sf2.moves.register {
    id = "arena_step",
    animation = anim,
    templates = { template },
}

sf2.moves.register_trigger {
    id = "arena_step_sound",
    events = { { type = sf2.moves.ANIMATION_START, name = "example.mod:moves/arena_step" } },
    actions = { { type = sf2.moves.SOUND, audio = music, volume = 0.25 } },
}

local tactic = sf2.tactics.register {
    id = "arena_tactic",
    type = sf2.tactics.TABULAR,
    template = "Standard",
    animation_weights = {
        { move = move, value = { base = 1 } },
    },
}

local fighter = sf2.warriors.register {
    id = "arena_tactician",
    template = sf2.warriors.get_template("core:warrior-templates/default"),
    first_name = "Arena",
    last_name = "Tactician",
    level = 1,
    tactic = tactic,
}
```

For recovered/core tactics, the compatibility string form such as
`tactic = "Standard"` remains valid. Prefer the opaque handle for mod-owned
tactics. `sf2.tactics.name(tactic)` is available as a narrow explicit runtime-name
adapter when another supported field genuinely requires the recovered tactic name.

Qualified runtime audio is currently PCM16 WAV only. `sf2.assets.binary` supports
namespaced move animation bytes. Location art supports typed single-sprite layers;
loose multi-sprite atlas subassets and recovered particle/prefab layer forms are not
yet a general public contract. Move/template/trigger registration is additive only
because no safe recovered reversible replace/remove seam has been proven. Ordinary
recovered tactics are supported; DE `ConditionalDecisions` remains unsupported
because no authoritative parser/evaluator exists in the recovered runtime.

Locale names and platform codes must both be unique against the shipped language
metadata. Use a private-use code such as `en-x-arena` for an English variant;
`en` already belongs to the base English entry. Additive locales inherit the
default language's base text and fonts unless explicit fonts are supplied. Mod
TOML translations still fall back to `eng` when the selected language has no file.

A fight location needs a gameplay layer (`type = 2`). Its optional `fighters`
record supplies all four spawn coordinates shown above; this layer may omit
`images`. These coordinates are recovered location units, not screen pixels.
The `color` field colors the fighters, not a full-screen backdrop.

`ForwardStep` is a classification template, not an input binding. Moves require
activation events and applicable conditions. The integrated showcase uses
`ROUND_STAGE_START` with `name = "Fight"` and a `PERK` condition whose `perk`
field is an opaque perk handle. This uses the recovered perk predicate to scope
the move to the showcase fighter. It does not add a player dash key.

For a mod zone's map footer, define the localization key `zones/<zone-local-id>`
in that mod's TOML files. The adapter publishes it under the zone's qualified
runtime name, which the recovered map uses for its title.

`Mods/example.phase1` is the integrated Phase 1 showcase. It combines durable
state, localization, P1C content, P1D location/move/tactic content, a P1A stage
graph, and P1B quest flow using only the public Lua API on an unchanged base install.

## Perks, enchantments, and reusable behaviors (0.3)

This is the executable side of the API: definitions bind typed parameters to
reusable Lua code, and that code calls supported runtime capabilities. The current
event surface includes fight/round lifecycle, resolved damage, block, critical,
and incoming-damage hooks. API 0.6 adds typed instance state and scoped target/effect
capabilities; see [the current contract](P2_API.md).

API 0.3 separates three concepts that the recovered engine historically represented with
the same `PerkInfoItem` machinery:

- a **behavior** is reusable executable Lua logic plus its typed instance schema;
- a **perk** is a public perk definition that may use a behavior;
- an **enchantment** is a public forge/save definition that may use the same behavior directly.

A perk and an enchantment no longer have to reference each other. They can independently use
the same behavior, so common mechanics live in one Lua function instead of being copied into
templates or XML.

Register reusable behavior first:

```lua
local absorption = sf2.behaviors.register {
    id = "damage_absorption",
    parameters = {
        chance = sf2.behaviors.NUMBER,
        protection = sf2.behaviors.STRING,
        stacks = {
            type = sf2.behaviors.INTEGER,
            required = false,
            default = 1,
        },
    },
    on_fight_begin = function(parameters, fighter)
        sf2.log.info("absorption initialized: " .. parameters.protection)
    end,
}
```

Behavior IDs use `<mod-id>:behaviors/<id>`. Supported schema types are `NUMBER`, `INTEGER`,
`BOOLEAN`, and `STRING`. A plain type token declares a required parameter. The expanded table
form can set `required = false` and a typed `default`. Schemas are limited to 64 fields, field
names use ASCII letters, digits, and `_`, numeric values must be finite, and strings are
bounded. `INTEGER` is limited to the exact Lua integer range
`-9007199254740991..9007199254740991`, so values survive XML, C#, and MoonSharp without
precision loss. The schema is part of the saved-content ABI, so an incompatible type/name
change should use a new published definition ID unless an explicit migration is added later.

The same behavior can back a perk without a template:

```lua
local head_absorption = sf2.perks.register {
    id = "head_absorption",
    behavior = absorption,
    kind = sf2.perks.SINGLE,
    display_name = sf2.localization.key("perk.head_absorption"),
    description = sf2.localization.key("perk.head_absorption.description"),
    parameters = {
        chance = 0.25,
        protection = "head",
    },
}
```

An enchantment may use that behavior directly, with no public perk dependency:

```lua
local body_absorption = sf2.enchantments.register {
    id = "body_absorption",
    behavior = absorption,
    display_name = sf2.localization.key("enchantment.body_absorption"),
    description = sf2.localization.key("enchantment.body_absorption.description"),
    recipe = sf2.enchantments.MEDIUM,
    item_types = { sf2.enchantments.ARMOR },
    parameters = {
        chance = 0.40,
        protection = "body",
    },
}
```

`parameters` on these behavior-backed definitions are validated immediately against the
registered schema. Missing optional values receive their declared defaults. Unknown fields,
wrong Lua types, non-finite numbers, and missing required fields fail registration atomically.

Recipe constants remain `SIMPLE`, `MEDIUM`, and `COMPLEX`. Equipment constants remain
`WEAPON`, `ARMOR`, `HELM`, `RANGED`, and `MAGIC`. A behavior-backed enchantment owns its own
display name, description, optional icon, public ID, typed state, and forge exposure. The
recovered runtime still receives a small internal compatibility `PerkInfoItem`, but that is
an engine implementation detail and is not the public relationship between perks and
enchantments anymore.

### Typed save state

Vanilla/recovered perk state stays in `<Set>`. Mod-authored typed state is stored separately:

```xml
<Perk
    Name="example.mod:enchantments/body_absorption"
    EclipseEnchantment="example.mod:enchantments/body_absorption"
    EclipseKind="Single">
  <Set Aspect="321" />
  <EclipseParams Format="1">
    <Param Name="chance" Value="0.4" />
    <Param Name="protection" Value="body" />
    <Param Name="stacks" Value="1" />
  </EclipseParams>
</Perk>
```

`Name` remains the recovered engine's runtime perk identity. `EclipseEnchantment` is the
public save identity, and `EclipseKind` preserves vanilla Single/Combo replacement semantics
even while the defining mod is temporarily missing. `EclipseParams` is owned by Eclipse and
is decoded using the behavior's registered schema. It is deliberately separate from `<Set>`
so arbitrary Lua fields never enter the recovered perk expression system.

Behavior-backed learned perks use the same typed payload directly on their saved roster perk:

```xml
<Perks>
  <Perk Name="example.mod:perks/battle_focus" Level="1">
    <EclipseParams Format="1">
      <Param Name="magic_charge" Value="0.15" />
      <Param Name="health_bonus" Value="0.02" />
    </EclipseParams>
  </Perk>
</Perks>
```

When a behavior-backed perk is newly learned or updated while its mod is active, Eclipse
snapshots the resolved typed values into that saved perk instance. Loading an older saved perk
that predates `EclipseParams` does not rewrite it; while the defining mod is available it falls
back to that perk definition's current initial values. For that reason, changing published
parameter defaults under the same perk ID is also a save-ABI change unless a migration is added.

Reads never normalize the XML. Unknown parameters and children survive round trips. If the
mod is missing, the raw node stays inert and untouched. If a known typed value is malformed,
the scripted instance is treated as unavailable instead of silently coercing or rewriting it.
Reinstalling the same published enchantment/behavior IDs makes valid preserved state usable
again unless the player already replaced that enchantment through normal forge rules.
Published behavior schemas, enchantment backends, recipe replacement class, and initial
parameter defaults should therefore be treated as stable save ABI for a given public ID.
In particular, an older saved node with no `EclipseParams` payload uses the currently
registered initial values, so changing those values under the same ID changes that old item.

### Behavior execution boundary

The first bounded event surface is `on_fight_begin`. Lua handlers are retained by the owning
MoonSharp context, not serialized into the catalog or save. Invocation uses fresh data tables,
does not expose recovered engine objects, forbids Lua yields, isolates script errors, and has
a separate 200,000-instruction per-call budget instead of the 5,000,000-instruction mod
entrypoint budget.

Direct behavior-backed saved enchantments and learned behavior-backed perks are connected to the
recovered normal-fight lifecycle. Eclipse dispatches `on_fight_begin` once, on round 1 after fight
rules have finalized the player's equipment, recovered perk state has been rebuilt,
`Model.NextRound` has completed, and `PerksStage` has re-enabled the active perks, but before
controllable combat begins. Enchantments must still be active in the player's post-rule runtime
perk set. Scripted perks must both be present in the learned/profile perk set and survive the same
post-rule/`NoPerks` filtering. Registration does not automatically grant or learn a perk.
Rule-created item clones, missing mods, API 0.2 template enchantments, and inactive definitions are
skipped.

The second argument is a sanitized capability table rather than a recovered fighter object. All
callbacks expose `side = "player"` plus `source = "perk"` or `source = "enchantment"`. Perk
callbacks expose the validated public `perk_id`. Enchantment callbacks expose `item_type`,
`item_id`, and the validated public `enchantment_id`. No `Model`, `ModelParameters`, `ItemInfo`,
`UserItem`, `XmlNode`, `PerkInfoItem`, or Unity object crosses into Lua. Each saved enchantment
instance is snapshotted and dispatched independently; a failing or malformed active handler is
logged and isolated so the recovered fight state machine and later mod effects continue.

Two explicit fighter operations are available in API 0.3:

```lua
on_fight_begin = function(parameters, fighter)
    fighter:add_magic_charge(0.35)
    fighter:change_health(0.05)
end
```

`fighter:change_health(amount)` requires the manifest capability `combat.change_life`. Positive
values heal and negative values damage through the recovered `Fight.UpdateLife` path already used
by vanilla Lifesteal and `ModHealthChange` actions. `fighter:add_magic_charge(amount)` requires
`combat.magic_charge` and uses the same recovered charge mutation/normalization path as
`PerkActionAddMagicCharge`. Both accept only finite single-precision numeric values. The host
operations are scoped to the current player fighter; they do not expose target handles or arbitrary
engine mutation.

A mod using both operations declares them explicitly:

```toml
capabilities = ["content.register", "combat.change_life", "combat.magic_charge"]
```

The tracked `example.enchantment` demonstrates one `battle_charge` behavior reused by a learned
`battle_focus` perk and the directly forgeable `battle_charge_weapon` enchantment with different
typed values. The enchantment starts a normal fight with 35% magic charge and a small heal; the
perk uses the same Lua function with its own values when that perk is actually learned/active.
The save-specific dispatcher is deliberately player-only: recovered AI loadouts do not have an
authoritative `UserItem` instance after fight-rule equipment replacement. `FightNone`/punchbag
also does not use the normal `NextRound` lifecycle and has no `on_fight_begin` policy yet.

API 0.6 expands these hooks to player equipment and registered opponent perks,
with round/fight/saved instance state, bounded migrations, damage shields and
incoming-hit scaling. Capability tables expire after each invocation. The
[integrated Phase 2 sample](example.phase2/README.md) also exercises timer policy,
feature gates, repeatable events, Ascension progression and offline raids.
See [P2_API.md](P2_API.md) for the exact supported surface and verification limits.

### API 0.2 template compatibility

The template form remains accepted as a compatibility lane for existing 0.2-style source,
provided the mod's manifest API range also accepts the current 0.3 runtime:

```lua
local lifesteal = sf2.perks.register {
    id = "lifesteal_legacy",
    template = sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_LIFESTEAL_WEAPON"),
    display_name = sf2.localization.key("perk.lifesteal"),
    description = sf2.localization.key("perk.lifesteal.description"),
    parameters = { Chance = 0.55 },
}

sf2.enchantments.register {
    id = "lifesteal_weapon_legacy",
    perk = lifesteal,
    recipe = sf2.enchantments.MEDIUM,
    item_types = { sf2.enchantments.WEAPON },
}
```

In this compatibility form, perk `parameters` are still definition-time overrides for the
recovered template `<Set>`. New behavior-backed content should use typed instance parameters
instead. A `perk = ...` enchantment must reference a template-backed compatibility perk;
behavior-backed perks cannot be routed back through that legacy enchantment form. Direct
behavior enchantments use `behavior = ...` and own their typed parameters themselves.
Registration remains transactional and behavior schemas, behavior references, typed initial
values, perks, and enchantments are all included in the deterministic `contentHash`.

### Renaming and retiring item IDs

Published item IDs should normally stay stable because ownership is stored in the save by
qualified definition ID. When a rename is unavoidable, register an alias from the historical
local item path to the current item handle:

```lua
local weapon = sf2.items.register_weapon {
    id = "example_blade",
    display_name = sf2.localization.key("weapon.example_blade"),
    icon = sf2.assets.sprite("sprites/weapon"),
    model = sf2.assets.model("core:gamedata/models/mdl_weapon_katana_ritual"),
    subtype = "Katana",
}

sf2.items.alias {
    from = "weapon/example_blade_legacy",
    to = weapon,
}
```

An existing save whose item node still says
`example.weapon:items/weapon/example_blade_legacy` resolves to the current definition and
participates in inventory/equipment logic normally. Eclipse deliberately leaves the historical
`Name` attribute unchanged. This keeps alias handling non-destructive until the future save
migration system can perform backed-up transactional rewrites.

Aliases stay inside the registering mod namespace and must preserve the equipment category.
For example, a historical weapon ID cannot alias to an armor definition. The alias points to
the current item definition, not to another mod's content.

If an ID is intentionally retired with no replacement, reserve it as a tombstone:

```lua
sf2.items.tombstone {
    id = "weapon/example_blade_retired",
}
```

A tombstoned save record remains preserved as unavailable/orphaned content rather than being
deleted or accidentally rebound to a future definition that reuses the same ID. The current
content-set fingerprint includes aliases and tombstones. Automatic record merging, arbitrary
save transformations, and versioned migration scripts remain future work.

## Compatibility

Early `sf2.shop.add` and `sf2.mod.log/warn/error` calls remain aliases for
`sf2.shop.addItem` and `sf2.log.info/warn/error`. New mods should use the new names.
Legacy `sprites/*.png` assets and optional sibling `*.sprite.toml` descriptors
still load. To migrate, move the PNG to `textures/` and replace its optional
sidecar with a `.asset` descriptor containing `type` and `texture`; the sprite's
logical ID can stay unchanged. Do not leave both the PNG and `.asset` at that
same logical ID.

Core TAR bundles retain their existing `.meta` descriptors and legacy addresses.
This loose-mod format does not modify Unity `.meta` files or core asset identity.

## Core ownership and storage

`core` is a semantic content owner, not a requirement to create a physical
`Mods/core/assets` tree. Vanilla runtime art is resolved through the same namespace
resolver as external assets, while `CoreAssetProvider` reads the current core storage
from `PackagedArtCatalog` and its TAR/LZ4 archives:

```text
legacy request "Textures/..." --implicit--> core:Textures/...
                                         |
                                         v
                                   AssetResolver
                                    /        \
                      CoreAssetProvider      LooseModProvider
                             |                       |
                      PackagedArtCatalog       Mods/<id>/assets
                             |
                           TAR/LZ4
```

An explicit external or core reference enters the same resolver directly. The caller does
not need to know whether a provider reads TAR/LZ4, loose files, or a future packaged mod.

Vanilla item art is frequently stored as atlas members rather than standalone catalog
addresses. Those members are valid first-class core sprite IDs when the exact atlas and
named member both exist, for example
`core:ui/items/armor12.img_armor_mantle_of_night`. A nonexistent member does not resolve.

API 0.7 adds [explicit typed asset replacement](P3_API.md#asset-replacement).
External mods still cannot claim the reserved `core` namespace. Replacement requires
`assets.replace`, a declared dependency, matching types and a unique target claim;
conflicts fail rather than using filesystem or last-mod-wins ordering.

## Save compatibility

Ownership continues to live in the existing `users.xml` item nodes, using the
qualified definition ID as `Name`. If a mod item definition is unavailable,
Eclipse leaves that entire XML node untouched and excludes it from active
inventory, equipment, and delivery processing. Counts, upgrade levels, pending
deliveries, enchantments, and unrecognized fields remain in the save. Reinstalling
the mod restores access on the next load; live mod unloading is not supported.

A missing equipped item uses the normal default item for the runtime model.
This fallback does not overwrite the saved equipment reference. If the player
explicitly equips another item while the mod is absent, restoring the mod
restores ownership without overriding that newer equipment choice.

`UserItems.MissingModItemIds` exposes unavailable item IDs for diagnostics. The
warrior's additive `EclipseMods` node records schema/API/core versions and each
successfully initialized mod's version and active status. Last-seen records for
absent mods are retained; unsupported future metadata schemas are left unchanged.
It also records a deterministic `contentHash` over active mod IDs/versions, the
actual committed definitions, committed patch provenance, and registered state
schemas. The hash therefore changes when registered semantic content changes
even if a mod author forgets to bump their version. This metadata is diagnostic,
not a reason to reject or reset a save. Item aliases/tombstones and API 0.5 state
aliases/tombstones provide non-destructive ID/state evolution; state migrations
are transactional and preserve the previous XML when a migration fails.

## Built-in core equipment registry

Game startup projects all five primary vanilla equipment categories and their
localized names into the same `ModContentCatalog` used by external mods, before
Lua registration: 210 weapons, 179 armor definitions, 193 helms, 85 ranged
definitions, and 73 magic definitions. For example, legacy `WEAPON_KATANA` is
exposed as `core:items/weapon/weapon_katana`, while `Body`, `Head`, `NoRanged`,
and `NoMagic` are exposed under their corresponding equipment categories.

Qualified lookups resolve back to the existing legacy `ItemInfo`; names, save
IDs, prices, upgrade templates, availability, and source XML are not rewritten.
`ItemDefinition.LegacyName` and `LegacyItemXml` retain the original identity and
complete source. This remains a read-only registry projection: vanilla XML and
the recovered item parser stay authoritative, including hidden definitions,
negative sentinel values, and fields not yet modeled by the public Mod API.

The vanilla data contains two distinct ranged rows named `GlaivebowArrow`.
Eclipse preserves both instead of applying a last-wins rule: the first is
`core:items/ranged/glaivebowarrow`, and the rifle-bullet variant is exposed as
`core:items/ranged/glaivebowarrow/riflebullet`. Shared localization identities
are reused only when their values match exactly.

No `Mods/core` package or bulk Lua conversion is required. Core assets remain behind
`CoreAssetProvider`, whose current storage implementation is TAR/LZ4. Unqualified legacy
asset calls routed through `ResourcesAndBundles` are now implicitly qualified to `core`
before the Unity `Resources` fallback, so vanilla and external assets share the same
namespace boundary without rewriting thousands of recovered call sites.

External Lua registration supports weapon, armor, helm, ranged, magic, Phase 1
non-equipment/set/progression/forge content, stages, quests, locales, locations,
moves, and ordinary tactics. The
tracked `example.loadout` registers one of each non-weapon category using matching
core-owned atlas sprites and model assets, and the recovered shop consumes the same five
category lists that `LegacyContentAdapter` updates. `example.phase1` is the integrated
public-API acceptance fixture spanning P0.5 plus P1A/P1B/P1C/P1D.

Validation: `Tools/TestModdingContracts.ps1` checks all 740 vanilla equipment
rows, atomic registration, duplicate-name disambiguation, and save XML round
trips. `Tools/TestModSaveRuntime.ps1` executes the recovered inventory parse and
actual `UserItem` XML mutation methods for ownership, upgrade, delivery, and
equipment state. `Tools/TestPackagedArt.ps1` checks provider routing, MoonSharp
registration, all five equipment registries, and the registry/legacy bridge in isolated
Unity. The expanded editor fixture currently passes 160 checks, including vanilla-derived
starting stats, the public Lua localization-patch path, reversible core localization binding,
and an intentional two-mod same-field conflict. A real player build has also been manually started with save data containing
modded equipment without breaking save load/startup. These checks still do not replace a
complete purchase/upgrade/equip/fight/removal/reinstall playtest for every category.

API 0.9 adds `fighter:snapshot()` for fresh, detached health, position, and fight-clock observations. See the fighter reference and the updated Third Strike Trial.

API 0.10 extends fight patches with location/music and rule replacement or append. [Campaign Guard Rule](example.core-fight/README.md) changes an existing encounter without duplicating it.

API 0.12 adds `on_damage_dealing` and capability-gated `fighter:scale_outgoing_damage`, before defensive modifiers. See `example.outgoing-rule`.

API 0.13 adds native `on_combo_changed` and `on_style_changed` snapshots. `example.combo-reserve` shows a timed bonus using the existing fight clock.

API 0.14 adds `on_tick` on active combat simulation frames. The combo-reserve
example uses it to clear timed state even without another hit; pause time is
excluded. See the public combat callback reference for ordering and lifetime.
