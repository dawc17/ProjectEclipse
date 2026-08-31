# Loose mods

Place each mod in `Mods/<folder>/` with a `mod.toml` manifest. See `example.weapon`
for a working weapon registration, localization, and shop listing.

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

## Shop and logging API

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
