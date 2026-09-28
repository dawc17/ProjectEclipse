---
title: Timers and service settings
description: Set forge delivery and battle timer policies and disable optional service groups.
---

These functions declare mod policy during startup. They do not expose arbitrary
configuration files. Multiple mods must respect the ownership rules below.

## sf2.timers.set

Set the delivery duration and early-skip policy for new forge orders, optionally
making already-paid pending orders eligible for immediate normal completion, or
set a shared time limit for timed battles.

**Signature:** `sf2.timers.set { subsystem, seconds, skip_enabled?, complete_pending? }`

**Requires:** `policy.timers`.

**When:** Entrypoint.

**Returns:** `nil`.

| Field | Type | Default | Meaning |
| --- | --- | --- | --- |
| `subsystem` | String | Required | `"forge"` or `"battle"`. |
| `seconds` | Integer | Required | Forge: 0–31536000, zero is instant. Battle: 1–86400 seconds per round. |
| `skip_enabled` | Boolean | `true` | Forge early completion; leave at its default for battle timers. |
| `complete_pending` | Boolean | `false` | With `seconds = 0`, make saved, already-paid forge orders eligible for normal completion on the next delivery update. Rejected for nonzero durations. |

```lua
sf2.timers.set {
    subsystem = "forge", seconds = 0,
    skip_enabled = false, complete_pending = true,
}
```

Only one mod may own a subsystem's timer policy. Competing declarations fail
rather than choosing the last-loaded mod.

```lua
sf2.timers.set { subsystem = "battle", seconds = 150 }
```

The battle policy overrides positive round limits in core and mod fights when
each round is prepared. Training (`FightNone`) and untimed fights retain their
native behavior. It overrides a fight's declared `round_time` while enabled, but
does not rewrite its definition or save. Disabling the mod restores the original
limit. The policy does not change simulation speed, animation timing or raid
session deadlines. Battle policies reject `skip_enabled = false` and
`complete_pending = true`; those options belong to forge delivery.

For forge policies, with the default `complete_pending = false`,
existing saved deadlines retain their original behavior. With `true`, their displayed
remaining time is zero and the normal delivery update applies the enchantment,
clears the pending order, and saves the result. This is ordinary completion, so it
does not require skipping to be enabled and does not charge materials a second time.

Registration never edits a saved deadline or grants an enchantment. Disabling the
policy before settlement restores the original remaining time; an already-completed
enchantment stays completed. A failed enchantment keeps its pending order for retry.
Material costs and skip prices are unchanged. Shop delivery
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
