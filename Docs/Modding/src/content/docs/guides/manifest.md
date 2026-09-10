---
title: The mod manifest
description: Define your mod's identity, entry script, dependencies, and permissions.
---

Every mod needs a UTF-8 `mod.toml` directly inside its folder. Eclipse reads this file before running Lua. Use the supported simple TOML format shown below: double-quoted strings, one-line arrays, and one `[[dependencies]]` block per dependency. Put all main fields before the first dependency block.

```toml
schema = 1
id = "yourname.training"
name = "Training Equipment"
version = "1.0.0"
api = ">=0.7 <1.0"
authors = ["Your Name"]
entrypoint = "scripts/main.lua"
capabilities = ["content.register"]

[[dependencies]]
id = "core"
version = ">=1.0 <2.0"
```

## Main fields

All main fields in the example are required.

| Field | Meaning |
| --- | --- |
| `schema` | Manifest format version. Use integer `1`. |
| `id` | Permanent unique identity, using lowercase ASCII letters, digits, `.`, `_`, or `-`. No spaces. `core` and `sf2de` are reserved. |
| `name` | Human-readable name shown in the mod menu. |
| `version` | Your mod's release version, such as `1.0.0`. Increase it when publishing changes. |
| `api` | Accepted Eclipse Mod API versions. It is independent of your mod's own version. |
| `authors` | Nonempty array of author names. |
| `entrypoint` | Relative Lua path inside `scripts/`, ending in `.lua`. No absolute paths or `..`. |
| `capabilities` | Array of permission names. Use `[]` if none are needed. Duplicate entries are rejected. |

Changing `name` changes the display label. Changing `id` creates a different content owner and can make existing saved items unavailable. Choose the ID before distributing your mod and keep it stable.

## Version ranges

`">=0.7 <1.0"` accepts versions at least `0.7.0` and below `1.0.0`. All space-separated comparisons must match. Supported comparison operators are `=`, `>`, `>=`, `<`, and `<=`; an unprefixed version is an exact match. Do not use npm-style `^`, `~`, or `*` ranges.

Set the minimum API version to one that provides every feature you use. Making the range broader does not add missing functions to older game builds.

## Dependencies

Each dependency requires `id` and `version`. A dependency declares which other content owner you use and ensures it is available before your mod loads. For example, a reference beginning with `core:` requires the `core` dependency above. You do not need a physical `Mods/core` folder; core is built into Eclipse.

To use another mod, add another block:

```toml
[[dependencies]]
id = "other.mod"
version = ">=1.0 <2.0"
```

Replace that illustrative ID with the actual installed mod's ID. Enabling your mod also enables its dependencies. Missing or incompatible dependencies are shown in **Details** in the mod menu. Do not list the same dependency twice.

## Capabilities

A capability permits an operation; a dependency permits references to another owner's content. They are separate requirements. Every function section lists its capabilities under **Requires**.

| Capability | Used for |
| --- | --- |
| `content.register` | Register content and use the documented content lookups. |
| `content.patch` | Supported changes to existing content, such as localization or perk choices. |
| `assets.replace` | Explicit replacement of an existing asset. |
| `state.read`, `state.write` | Read and write mod-owned profile state. |
| `combat.change_life`, `combat.magic_charge` | Supported health and magic-charge methods. |
| `combat.modify_hit`, `combat.effects` | Damage scaling and temporary shields. |
| `combat.target` | Use supported operations on the opposing fighter. |
| `progression.read`, `progression.write` | Read and update achievement counters. |
| `policy.timers`, `policy.services` | Supported timer settings and service switches. |

For example, a behavior that heals a fighter requires its registration capability and `combat.change_life`. Add the strings to the same one-line array, separated by commas. Permission names must match exactly; requesting a name does not make an unsupported API available.

## After editing

Save the manifest and restart Eclipse. Use **Details** to read validation problems before testing content. See [Troubleshooting](../troubleshooting/) and [Installing mods](../../api/installing-mods/).
