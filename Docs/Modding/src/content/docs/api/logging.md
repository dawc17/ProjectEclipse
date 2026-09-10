---
title: Modules and logging
description: Load the Eclipse API and your own Lua modules, identify your mod, and write useful diagnostics.
---

## require

Load the Eclipse API or a Lua module belonging to your mod.

**Signature:** `require(module_name)`

**Requires:** No capability for loading a module. Functions inside it retain their
normal requirements.

**When:** Entrypoint or a callback. Prefer loading modules once at script startup.

**Returns:** The module's returned value, cached for subsequent calls. A module
that returns nothing is represented by `true`.

```lua
local sf2 = require("sf2")
local helpers = require("helpers") -- scripts/helpers.lua in this mod
```

For example, `scripts/helpers.lua` can contain:

```lua
local helpers = {}
function helpers.clamp(value, minimum, maximum)
    return math.max(minimum, math.min(value, maximum))
end
return helpers
```

Use `require("combat.helpers")` for `scripts/combat/helpers.lua`. Modules are
isolated to the owning mod. Circular imports, missing files, absolute paths, and
parent-directory traversal fail. Lua runs in a sandbox: do not depend on ordinary
desktop Lua file I/O, network libraries, shell commands, or arbitrary C# access.

## Mod information

`sf2.mod.id`, `sf2.mod.name`, and `sf2.mod.version` are strings populated from
your manifest. Treat them as information, not settings you can change at runtime.

## sf2.log.debug

Write a detailed diagnostic message tagged with your mod's ID.

**Signature:** `sf2.log.debug(message)`

**Requires:** No capability.

**When:** Entrypoint or a callback.

**Returns:** `nil`.

`message` must be a string. Convert numbers with `tostring()` or concatenate them.

```lua
sf2.log.debug("Selected icon: " .. sf2.assets.qualify("sprites/weapon"))
```

## sf2.log.info

Record a normal milestone, such as successful content registration.

**Signature:** `sf2.log.info(message)`

**Requires:** No capability.

**When:** Entrypoint or a callback.

**Returns:** `nil`. `message` must be a string.

```lua
sf2.log.info("Registered my weapon")
```

## sf2.log.warn

Record a recoverable problem that may help explain unexpected behavior.

**Signature:** `sf2.log.warn(message)`

**Requires:** No capability.

**When:** Entrypoint or a callback.

**Returns:** `nil`. `message` must be a string.

```lua
sf2.log.warn("Optional sound missing; using a silent effect")
```

## sf2.log.error

Record an error message. Logging alone does not stop execution or undo changes.

**Signature:** `sf2.log.error(message)`

**Requires:** No capability.

**When:** Entrypoint or a callback.

**Returns:** `nil`. `message` must be a string.

```lua
sf2.log.error("Expected optional content was unavailable")
```

Use Lua's `error("reason")` when you need to abort the current script instead.
An uncaught entrypoint error fails registration. A callback error is reported and
isolated, but gameplay actions already performed by it are not rolled back.

## sf2.mod.log

Legacy alias for `sf2.log.info`. Prefer the latter in new code.

**Signature:** `sf2.mod.log(message)`

**Requires:** No capability.

**When:** Entrypoint or a callback.

**Returns:** `nil`; accepts one string.

```lua
sf2.mod.log("Loaded") -- same as sf2.log.info("Loaded")
```

## sf2.mod.warn

Legacy alias for `sf2.log.warn`.

**Signature:** `sf2.mod.warn(message)`

**Requires:** No capability.

**When:** Entrypoint or a callback.

**Returns:** `nil`; accepts one string.

```lua
sf2.mod.warn("Optional content missing")
```

## sf2.mod.error

Legacy alias for `sf2.log.error`. It also logs without throwing.

**Signature:** `sf2.mod.error(message)`

**Requires:** No capability.

**When:** Entrypoint or a callback.

**Returns:** `nil`; accepts one string.

```lua
sf2.mod.error("Unexpected content")
```
