---
title: Framework mods and shared services
description: Publish typed Lua services and let dependent mods extend your gameplay systems.
---

A **framework mod** supplies reusable functionality for other mods. An **extension
service** is a named Lua function with a declared request and response format.
Eclipse routes calls between separate Lua environments. Each mod retains its own
capabilities and state.

The [Focus Framework and Focus Trial example](https://github.com/dawc17/ProjectEclipse/tree/main/Mods/example.focus-framework)
uses two installable mods. The framework owns a saved Focus resource and calculates
a third-hit bonus. The add-on supplies the fight rule, applies the bonus through
its own fighter capability and updates its HUD. Other add-ons can use the same services.

## Dependencies and versions

Providers declare `extensions.provide`. Consumers declare `extensions.call` and a
**direct** manifest dependency on the provider; a dependency of a dependency does
not grant access. For example, add these fields to the consumer's normal manifest:

```toml
capabilities = ["extensions.call"]

[[dependencies]]
id = "myname.focus"
version = ">=1.0.0 <2.0.0"
```

The manifest range selects the package. The integer passed to
`sf2.extensions.get` separately selects the **exact service contract version**.
Publish another service ID when old and new contracts need to coexist.

## Request and response schemas

Schemas map field names to a type token or field definition:

| Type | Values |
| --- | --- |
| `"integer"` | Exact whole Lua numbers from −9007199254740991 to 9007199254740991. |
| `"number"` | Finite numbers; NaN and infinity fail. |
| `"boolean"` | `true` or `false`, without conversion. |
| `"string"` | Up to 2048 UTF-16 code units; empty is allowed. |

Each schema allows 64 fields. Names have 1–64 ASCII letters, digits or underscores.
A type token is required by default. Use `{ type = "integer", required = false }`
for an optional field, or a matching `default` to supply omitted values. Defaults
apply even to required fields. Optional fields without defaults remain absent.
Omitted schemas describe empty records.

Requests/responses are plain tables with string keys. Unknown fields, nested
tables, functions and asset/fighter/UI/native handles fail. Qualified content IDs
may be strings; the receiving mod resolves them through its own API/dependencies.
Changing a copied request or response never mutates the other mod's original table.

## sf2.extensions.register

**Signature:** `sf2.extensions.register { id, version, request?, response?, handler } -> string`

**Returns:** `<mod-id>:extensions/<id>` as a string.

**When:** In the provider's entrypoint. Exports become available after its content
transaction commits, before dependents load. Duplicate IDs and late registration fail.

**Requires:** `extensions.provide`. `id` uses the normal local definition path
rules. `version` is an integer from 1–1000000. `handler` is a Lua function.
Request/response schemas are optional; unknown registration fields fail.

```lua
local sf2 = require("sf2")
sf2.extensions.register {
    id = "hit_bonus", version = 1,
    request = { damage = "number", combo = "integer" },
    response = { bonus = "number" },
    handler = function(hit, caller)
        assert(hit.damage >= 0 and hit.combo >= 0, "Invalid hit")
        return { bonus = hit.combo % 3 == 0 and math.min(1, hit.damage * 0.5) or 0 }
    end,
}
```

IDs, versions, field types, required flags and defaults contribute to the content
compatibility fingerprint. Schema declaration order does not change that identity.
Content sets without extensions retain their previous fingerprints.

## Extension handler

**Signature:** `handler = function(request, caller) ... return response end`

**Returns:** A table matching the response schema, including `{}` for an empty
schema. `nil` is an error. Omitted defaults are filled after the handler returns.

**When:** Synchronously inside a consumer call, during a dependent's entrypoint
or a later supported UI/story/mode/AI/combat callback. It cannot yield or wait for
another callback. Its execution budget is 200,000 instructions.

**Requires:** The provider's own capabilities for its operations. `caller` is the
immediate caller's actual mod ID, supplied by Eclipse. In A → B → C, C sees B.
No fighter handle is supplied; state operations still require a loaded profile.

```lua
-- Within a registration whose response is { accepted = "boolean" }:
handler = function(request, caller)
    return { accepted = caller == "myname.focus-addon" }
end
```

Providers may intentionally offer operations that use their capabilities, such as
updating their own resource. Validate domain limits as well as field types; use
`caller` if access needs authorization. Successful Lua changes/state writes remain
applied when a later handler error, invalid response or consumer failure occurs.
Calls are not transactions across mods.

Calling into a mod already executing fails, preventing cycles. Use local Lua
functions within one mod. A chain permits 8 active service calls and 32 attempted
routed calls per outermost Lua execution, shared across nested calls. Budgets reset
on the next independent callback/entrypoint. These limits complement the per-handler
budget; they do not promise a fixed frame cost.

## sf2.extensions.get

**Signature:** `sf2.extensions.get(reference, version) -> extension`

**Returns:** An opaque handle owned by this script. Unavailable services, wrong
categories, undeclared dependencies and incompatible versions raise errors.

**When:** After the provider loads, normally once in the consumer's entrypoint.
Keep the handle in local session memory for later callbacks.

**Requires:** `extensions.call`, a direct provider dependency, a qualified
`<mod-id>:extensions/<id>` reference and the exact integer service version.

```lua
local sf2 = require("sf2")
local bonus = sf2.extensions.get("myname.focus:extensions/hit_bonus", 1)
```

Handles/closures cannot be saved or transferred to another mod. Acquire fresh
handles after reload. Removed or failed providers lose their exports; dependent
mods cannot start without them. Registries belong to the script session, so exports
cannot leak into another session. Saved resources use normal
[mod state](../mod-state/) and [missing-mod preservation](../save-compatibility/).

## sf2.extensions.call

**Signature:** `sf2.extensions.call(extension, request) -> response`

**Returns:** A detached response. Invalid handles/requests, provider failures,
invalid responses and exceeded budgets raise an error in the consumer.

**When:** Loading or supported runtime callbacks. Keep per-frame calls small.
State migrations and reward-configuration callbacks cannot invoke services,
including through a captured handle.

**Requires:** `extensions.call` and a handle acquired by this script. The provider
must remain available with the same version. Requests must match its schema.

```lua
local sf2 = require("sf2")
local bonus = sf2.extensions.get("myname.focus:extensions/hit_bonus", 1)
-- Use inside on_damage_dealing; also requires combat.modify_outgoing_hit.
local function apply_bonus(fighter, hit, combo)
    local result = sf2.extensions.call(bonus, { damage = hit.damage, combo = combo })
    if result.bonus > 0 then fighter:add_outgoing_damage(result.bonus) end
end
```

Unhandled errors follow the consumer callback's normal error policy. Use
`try_call` when your consumer has a useful fallback.

## sf2.extensions.try_call

**Signature:** `sf2.extensions.try_call(extension, request) -> response?, error?`

**Returns:** `response, nil` on success; `nil, diagnostic_string` on rejection or
provider failure. Handler failure diagnostics identify the provider and caller.
Successful changes before a failure remain applied.

**When:** The same boundaries as `call`, when recovery is possible. It does not
reset budgets, retry or turn a failed operation into success.

**Requires:** `extensions.call`; a missing capability still raises an error.
All ownership, schema, lifecycle and restricted-callback rules still apply.

```lua
-- bonus was acquired earlier by this script.
local result, err = sf2.extensions.try_call(bonus, { damage = 0.1, combo = 3 })
if not result then sf2.log.warn(err) end
```

## Verification and boundaries

The managed extension runner executes separate production Lua environments,
production session startup/teardown, provider/add-on combat callbacks, HUD data
and saved resource reloads with controlled native sources. It checks contracts,
dependencies, versions, failed providers, detached data and execution budgets.
An isolated Unity 6.6 Play Mode runner executes the shipped pair, renders the HUD
with the recovered game font and checks changing pixels, teardown and saved Focus
after removing/reinstalling both mods. Its contact source and asset host are
controlled. Real fight-contact ordering, native menus/profile integration and a
full game playtest remain acceptance work.

Services do not add raw C# plugins, arbitrary scenes, custom result authority,
extra fighters, network messages, asynchronous RPC or a general event bus. They
share procedural functionality built from existing typed capabilities. New engine
operations outside those capabilities still require source changes.
