---
title: Keeping player saves compatible
description: Preserve owned items and progress when a mod is disabled or updated.
---

A player's saved mod content depends on stable IDs. Decide your manifest ID and local content IDs before publishing, then preserve them across updates.

## When a mod is disabled or missing

Eclipse preserves unavailable mod-owned inventory records, including counts, upgrades, deliveries, and enchantments. The missing items are excluded from active inventory and equipment processing. Re-enabling or reinstalling the mod restores access when the profile next loads.

If a missing item was equipped, the game uses its normal default item for the visible fighter. That fallback does not overwrite the saved equipment choice. If the player deliberately equips something else, restoring the mod does not override that newer choice.

Mod-owned saved progress is retained while the mod is disabled. Fix loading problems using the error details while preserving the player's save.

## When you change an item ID

Prefer keeping the published ID even if the display name changes. If an ID must change, register an alias from the old item ID to its replacement using [sf2.items.alias](../equipment-shop-logging/#sf2itemsalias). An old save can then resolve to the new item.

For intentionally retired content, a [tombstone](../equipment-shop-logging/#sf2itemstombstone) reserves the old ID while preserving its unavailable save record. It does not erase ownership or grant replacement content.

## When you change saved fields

Use the schema, aliases, tombstones, and migration rules documented in [Mod state](../mod-state/). A failed state migration preserves the previous saved XML. Avoid reusing an old field name for a different meaning or type.

## Interrupted profile writes

Eclipse records a pending profile snapshot before replacing `users.xml` or
`users_backup.xml` and its optional hash. The profile reader replays a valid
pending write before loading that file. This handles interruption between the
snapshot and hash replacements; it does not make the two profile copies one
transaction. Each replacement uses a flushed temporary file in the same directory.

Pending records use a `.eclipse-write` suffix and are removed after installation.
The `.eclipse-write.lock` sidecar remains for coordination. Recovery checks record
integrity and preserves the game's hash-validation policy when enabled. Invalid
records produce an error and remain available for diagnosis rather than being
silently discarded. Intentional profile reset or replacement discards pending
records so they cannot restore the old profile.

A save request is still not confirmation that writing finished. This mechanism
does not roll back partial gameplay mutations or guarantee durability against
every filesystem or power failure. Disk
failure and fresh-process recovery fixtures cover the write mechanism; full-game
restart acceptance remains pending.

The internal lottery workflow stores one pending `EclipseLotteryClaim` per profile,
with a versioned, evaluated prize. Resuming does not rerun reward selection. Claiming
defers native profile saves until reward application and the completion marker are
ready to save together. A failed settlement blocks later saves until profile reload,
so partial in-memory changes cannot overwrite the recoverable snapshot. Missing
item definitions or changed upgrade identity prevent restoration rather than
silently substituting a different reward. Nested draws and native resistance
grants are not supported by claiming yet.

These helpers are not yet connected to a complete lottery screen, quest resumption,
or public Lua lottery API. Automated claim fixtures control reward grants and disk
saves; full-game settlement/restart verification remains pending.

When a host supplies a quest invocation ledger, claim completion also records a
receipt for that action in the same save. Replaying an acknowledged action does
not create another prize. A new quest run uses a new ID; pending receipts prevent
replacement of an unfinished run. The native quest runner now exposes its start,
checkpoint-resume, and completion boundaries to this bookkeeping. Quest claims are
identified by quest file and name under `EclipseQuestClaims`; an unfinished run is
reused when reopened, while a completed run can start a new draw. Resuming after
the action definition changes is rejected so an old receipt cannot acknowledge a
different action. Concurrent instances of the same lottery quest and nested action
identities are not supported by this bridge yet. `DialogLottery` still needs its
presentation connection before this provides a complete playable workflow.

## Before publishing an update

Test with a save from the previous version, including owned and equipped items and any saved behavior progress. Also test disabling and re-enabling the mod. Restart Eclipse after file changes; replacing files does not reload existing definitions.

The save records mod versions and content metadata for diagnostics. Changes to that metadata do not invalidate the player's save.
