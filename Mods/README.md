# Loose mods

Place each mod in `Mods/<folder>/` with a `mod.toml` manifest. See `example.weapon`
for the minimal weapon slice and `example.loadout` for armor, helm, ranged, and magic
registered through the same transactional API.

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

Addressability does **not** mean replaceability. External mods cannot claim the reserved
`core` namespace, and there is no last-mod-wins override behavior. If controlled core
replacement is added later it will require an explicit replacement contract and dependency,
not filesystem ordering.

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
It also records a deterministic `contentHash` over active mod IDs/versions and the
actual committed localization/item/shop definitions. The hash therefore changes
when registered content changes even if a mod author forgets to bump their version.
This metadata is diagnostic, not a reason to reject or reset a save. Item aliases and
tombstones provide the first non-destructive ID-evolution contract. Automatic versioned mod
migrations and transactional save rewrites remain future work; keeping published IDs stable
is still preferable when possible.

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

External Lua registration now supports weapon, armor, helm, ranged, and magic. The
tracked `example.loadout` registers one of each non-weapon category using matching
core-owned atlas sprites and model assets, and the recovered shop consumes the same five
category lists that `LegacyContentAdapter` updates. Perks, enchantments, fights, and quests
remain later content slices.

Validation: `Tools/TestModdingContracts.ps1` checks all 740 vanilla equipment
rows, atomic registration, duplicate-name disambiguation, and save XML round
trips. `Tools/TestModSaveRuntime.ps1` executes the recovered inventory parse and
actual `UserItem` XML mutation methods for ownership, upgrade, delivery, and
equipment state. `Tools/TestPackagedArt.ps1` checks provider routing, MoonSharp
registration, all five equipment registries, and the registry/legacy bridge in isolated
Unity. The expanded editor fixture currently passes 124 checks, including vanilla-derived
starting stats. A real player build has also been manually started with save data containing
modded equipment without breaking save load/startup. These checks still do not replace a
complete purchase/upgrade/equip/fight/removal/reinstall playtest for every category.
