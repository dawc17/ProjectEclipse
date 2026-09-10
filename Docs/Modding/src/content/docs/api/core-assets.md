---
title: Core assets and ownership
description: Reuse built-in art and understand which files belong to your mod.
---

`core` is the owner of Eclipse's built-in content. You do not create a `Mods/core/assets` folder or unpack the game's art to use it. Declare a `core` dependency, then pass an existing qualified asset ID to the appropriate lookup.

```lua
local model = sf2.assets.model("core:gamedata/models/mdl_weapon_katana_ritual")
local icon = sf2.assets.sprite("core:UI/Items/Armor12.img_armor_mantle_of_night")
```

## Understand an asset reference

In the icon reference above, `core` is the owner and the rest identifies the sprite. Many core sprites are members of an *atlas*: one texture containing several named images. Both the atlas and the named member must exist.

For your own loose assets, paths start inside the mod's `assets/` directory and omit the last extension. `assets/sprites/weapon.asset` becomes `sprites/weapon`; the current mod's namespace is supplied automatically. See [Sprites and textures](../sprites-and-textures/) for the descriptor format.

## Reuse an asset

Reusing an asset means pointing a new definition at it. This does not change the original item or image. See [Reusing core equipment](../core-equipment/) for the difference between an item definition and its art.

## Replace an asset

[Explicit asset replacement](../asset-replacement/) changes the supported target for consumers that resolve it. It requires `assets.replace`, a dependency on the target owner, matching asset kinds, and a unique claim on the target. A file in your mod cannot take ownership of `core` by using the same filename.

Two mods claiming the same replacement target conflict. Changing their folder order is not a way to select a replacement.
