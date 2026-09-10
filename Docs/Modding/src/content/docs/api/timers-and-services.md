---
title: Timers and service settings
description: Set the supported forge delivery policy and disable optional service groups.
---

These functions declare mod policy during startup. They do not expose arbitrary
configuration files. Multiple mods must respect the ownership rules below.

## sf2.timers.set

Set the delivery duration and early-skip policy for new forge orders.

**Signature:** `sf2.timers.set { subsystem, seconds, skip_enabled? }`

**Requires:** `policy.timers`.

**When:** Entrypoint.

**Returns:** `nil`.

| Field | Type | Default | Meaning |
| --- | --- | --- | --- |
| `subsystem` | String | Required | Currently only `"forge"`. |
| `seconds` | Integer, 0–31536000 | Required | Delivery duration; zero is instant. |
| `skip_enabled` | Boolean | `true` | Whether early completion is allowed. |

```lua
sf2.timers.set { subsystem = "forge", seconds = 0, skip_enabled = false }
```

Only one mod may own a subsystem's timer policy. Competing declarations fail
rather than choosing the last-loaded mod. Existing saved deadlines retain their
original values. Material costs and skip prices are unchanged. Shop delivery
is not an additional supported timer target.

## sf2.services.disable

Disable a supported optional service feature group.

**Signature:** `sf2.services.disable(name)`

**Requires:** `policy.services`.

**When:** Entrypoint.

**Returns:** `nil`.

```lua
sf2.services.disable("battle_pass")
```

Accepted names are `paid_offers`, `battle_pass`, `ads`, `rewarded_video`,
`online_services`, and `payments`. Unknown names are rejected. Disables from
multiple mods combine. This is an opt-out: it cannot install or re-enable an
absent service, remove SDK code from a build, or mutate arbitrary UI objects.
If the offline build already lacks a service, disabling it may have no visible effect.
