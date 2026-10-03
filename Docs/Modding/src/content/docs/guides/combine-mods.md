---
title: Combine mods
description: Share fights through additive rules, understand conflicts, and test a composed pack.
---

A **mod pack** is a set of mods installed and enabled together. Each mod retains
its identity, capabilities, state and HUD handles. A dependency means a mod
requires another mod and loads after it; it does not merge their permissions.

## Try three cooperating mods

Install and enable these repository examples, then use **Apply & Restart**:

1. [Focus Framework](https://github.com/dawc17/ProjectEclipse/tree/main/Mods/example.focus-framework)
   owns a saved Focus resource and exposes versioned services.
2. [Focus Trial](https://github.com/dawc17/ProjectEclipse/tree/main/Mods/example.focus-addon)
   depends on the framework, adds a bonus every third positive unblocked outgoing
   hit, and displays the resource meter.
3. [Three-hit Objective](https://github.com/dawc17/ProjectEclipse/tree/main/Mods/example.hit-objective)
   independently adds a round controller: land three positive unblocked hits
   within ten seconds of active simulation to win the round; expiry loses.

Enter Act I tournament battle 3 in normal or Eclipse mode. Both gameplay rules
apply to the same fight. Separate HUD lines appear beneath the upper left edge
of the screen. Native knockout, timeout and surrender take precedence over the
objective. No other mods are required; a rule-list replacement from another
enabled mod can still conflict.

## Append compatible rules

Each mod registers its own typed rule and appends it:

```lua
local sf2 = require("sf2")
local observer = sf2.behaviors.register {
    id = "observer",
    on_round_begin = function()
        sf2.log.info("My rule is active alongside the fight's other rules")
    end,
}
local rule = sf2.rules.behavior { id = "observer", behavior = observer }
sf2.fights.patch {
    target = "core:fights/zone_1/tournament/3",
    append_rules = { rule },
}
```

Declare `content.register`, `content.patch` and a compatible `core` dependency
in the manifest. This observer claims no result authority.

Native projection preserves recovered XML entries and then adds static rules once.
Lua behavior rules use the combined handle order at their own combat dispatch
boundary; they are not XML entries. Added handles follow resolved mod load
order and each author's list order. Dependencies load before dependents; ready
mods sort by ID using an ordinal comparison. Folder/discovery order is irrelevant.
If behavior needs a particular order, declare the required dependency. Callback
operations still execute sequentially: scaling and adding damage can produce
different results when their order changes.

## Understand a conflict

| Change | Result |
| --- | --- |
| Two mods append different compatible rules | Both contribute. |
| A mod uses `rules`, including `rules = {}` | Exclusive replacement; any other rule-list patch conflicts. |
| Two controllers overlap in mode and round | The later registering transaction fails, naming both rules. |
| Controllers use disjoint mode or round filters | Both register; only the applicable controller has authority. |
| An already attached handle appears again | Rejected; it is not silently duplicated or ignored. |
| The combined list exceeds 100 handles | The later registering transaction fails. |
| One mod patches the same fight rule field twice | Rejected within that transaction; combine its handles into one list. |

A commit conflict discards all definitions and fields from that mod's transaction.
An already active mod keeps its committed content. Later independent mods can
still load; a mod depending on the failed owner remains unavailable. Distinct
rule IDs avoid identity collisions, but do not guarantee compatible gameplay.
HUD placement also belongs to each author: the engine does not automatically
move overlapping decorative overlays.

Use one [round controller](../../api/round-outcomes/) and
[framework services](../../api/extensions/) to coordinate a shared objective.
Dependencies do not authorize replacing another mod's exclusive rule-list patch.
Other semantic fields retain their documented conflict contracts.

## Disable and reinstall

Change the enabled set through Mods and use **Apply & Restart**. Eclipse rebuilds
the catalog from base definitions and only enabled mods; this is not hot removal
during an active fight. The native adapter projects the final combined rule list
once and retains the original battle source for teardown.

Disabling Three-hit Objective leaves Focus's rule and saved resource available.
Re-enabling it rebuilds both rules without copies. Disabling Focus Trial removes
its combat rule while the framework can remain installed. Missing required
dependencies are reported explicitly. Owned save data for absent mods is retained
under the normal [save compatibility](../../api/save-compatibility/) contract.
The objective's pending result and round-local state are transient.

Append patches now record the append operation in the content fingerprint.
Older profiles using such patches can report changed content after upgrading.
Keep saved IDs and schemas stable and test any intended migration.

## Verification limits

Production Lua/session tests cover load order, controller/replacement conflicts,
transaction rollback, limits, dispatch, source projection/restoration and the
shipped three-mod save/removal/reinstallation workflow. Isolated Unity 6.6 Play
Mode checks cover both visible HUDs, the Focus bonus, objective win/loss, native
fonts/fades and extracted adapter/round methods. Contacts, models, clock, battle
source storage, end presentation and settlement are controlled.
These checks do not prove full-game contact ordering, complete native result/reward
or menu/profile lifecycle, durable crash recovery, or arbitrary pack compatibility.
