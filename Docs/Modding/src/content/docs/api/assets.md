---
title: Asset functions
description: Find assets, create typed sprite, model, audio, and animation handles, and understand asset IDs.
---

An **asset** is a file your mod uses, such as an icon, model, sound, or animation.
All examples assume `local sf2 = require("sf2")` at the top of your script.
Paths below are logical IDs: omit the final file extension. A path without a
namespace belongs to your mod. To use another mod's assets, declare that mod as
a dependency and prefix the path with its ID, for example `core:`.

Asset handles identify existing content; they do not let Lua edit Unity objects.
An invalid path or undeclared dependency is an error, even in `exists`.

## sf2.assets.qualify

Turn a local asset path into its complete namespaced ID. This is useful in logs.

**Signature:** `sf2.assets.qualify(reference)`

**Requires:** No capability. Cross-mod references require a declared dependency.

**When:** Entrypoint or a callback.

**Returns:** A normalized string. It does not check whether the asset exists.

`reference` is a nonempty asset ID string. Absolute filesystem paths and traversal
segments such as `..` are not valid asset references.

```lua
local full_id = sf2.assets.qualify("sprites/weapon")
sf2.log.info(full_id) -- my.mod:sprites/weapon when the manifest ID is my.mod
```

## sf2.assets.exists

Check whether an asset ID is available before choosing it.

**Signature:** `sf2.assets.exists(reference)`

**Requires:** No capability; the referenced owner must be your mod or a dependency.

**When:** Entrypoint or a callback.

**Returns:** `true` if the asset is described by the resolver, otherwise `false`.
It does not guarantee successful image decoding or that the asset has the type
you want. Malformed or unauthorized references raise an error.

```lua
if sf2.assets.exists("sprites/optional_badge") then
    sf2.log.info("Optional badge found")
end
```

## sf2.assets.sprite

Get a sprite handle for an item icon, achievement, or location image.

**Signature:** `sf2.assets.sprite(reference)`

**Requires:** No capability; cross-mod references require a dependency.

**When:** Usually during registration, before passing the handle to a definition.

**Returns:** A sprite handle. Missing assets and assets of another type raise errors.

```lua
local icon = sf2.assets.sprite("sprites/weapon")
-- Requires assets/sprites/weapon.asset and its referenced PNG.
```

See the [sprite file format](../sprites-and-textures/) for crops, pivots, filtering,
and atlas references. A PNG texture ID is not interchangeable with a sprite ID.

## sf2.assets.model

Get a model handle for equipment.

**Signature:** `sf2.assets.model(reference)`

**Requires:** No capability; cross-mod references require a dependency.

**When:** Usually during registration.

**Returns:** A model handle. The asset must exist and be classified as a model.

```lua
local model = sf2.assets.model("core:gamedata/models/mdl_weapon_katana_ritual")
```

Using a known core model is the easiest starting point. The model, equipment
category, and combat subtype must make sense together; a valid ID alone does
not prove that a custom model will look or animate correctly.

## sf2.assets.audio

Get an audio handle for location music or a move's sound action.

**Signature:** `sf2.assets.audio(reference)`

**Requires:** No capability; cross-mod references require a dependency.

**When:** During registration before using the handle.

**Returns:** An audio handle. Missing or non-audio assets raise an error.

```lua
local music = sf2.assets.audio("audio/arena")
-- For example, a supported WAV file at assets/audio/arena.wav.
```

This only resolves the asset. To hear it, attach it to a location or a supported
sound action; there is no general `play()` method on the returned handle.

## sf2.assets.binary

Get a binary asset handle for an animation supplied to `sf2.moves.register`.

**Signature:** `sf2.assets.binary(reference)`

**Requires:** No capability; cross-mod references require a dependency.

**When:** During registration.

**Returns:** A binary handle. An asset of another kind is rejected.

```lua
local animation = sf2.assets.binary("animations/opening")
-- Supply an actual animation file in the runtime's supported binary format.
```

A binary handle does not decode, convert, or validate every animation structure.
Renaming an arbitrary file will not turn it into an animation. The
[move reference](../moves-and-tactics/) explains where this handle is used.

For global swaps, see [`sf2.assets.replace`](../asset-replacement/#sf2assetsreplace).
There is no public Lua texture constructor; sprite descriptors reference textures.
