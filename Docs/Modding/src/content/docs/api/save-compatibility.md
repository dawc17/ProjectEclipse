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

## Before publishing an update

Test with a save from the previous version, including owned and equipped items and any saved behavior progress. Also test disabling and re-enabling the mod. Restart Eclipse after file changes; replacing files does not reload existing definitions.

The save records mod versions and content metadata for diagnostics. Changes to that metadata do not invalidate the player's save.
