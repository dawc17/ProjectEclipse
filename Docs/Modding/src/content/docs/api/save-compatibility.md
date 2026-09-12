---
title: Keeping player saves compatible
description: Preserve owned items and progress when a mod is disabled or updated.
---

A player's saved mod content depends on stable IDs. Decide your manifest ID and local content IDs before publishing, then preserve them across updates.

Forge candidate exclusions and deviation overrides participate in the content compatibility fingerprint. Changing an override's bounds is detected even when the mod version stays the same. These changes affect future rolls; existing equipment enchantments retain their saved values.

Default equipment enchantment loadouts also participate in that fingerprint, including entry order and optional aspects. Changing or disabling a loadout restores acquisition defaults without rewriting enchantments already saved on inventory items.

Innate equipment loadouts are definition-level effects, not saved enchantments. Their ordered perks and named numeric parameters affect the fingerprint. Disabling them restores the original effects for subsequently built fighters without modifying the inventory's saved enchantments.

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

The legacy top-level `DialogLottery` quest action now uses these helpers for one
draw from the fight named by its `FightName` expression. It shows the saved reward
in a parchment dialog using the game's UI renderer. **Claim** grants the reward and
advances the quest; **Later**, Back, or scene teardown preserves it without
advancing. Opening another non-combat scene reopens the outstanding dialog while
that quest action remains active. Replaying the quest after a profile reload uses
the saved receipt. This is not an automatic quest-resume scheduler for unresumable
quests: their triggering event must run again.

The fight must supply exactly one lottery with an eligible slot. Missing rewards
raise an error rather than completing the action without a prize. `SpinNumber`
(archived paid-spin continuation) and native `Lock` attributes are explicitly
unsupported. The preview resolves item and currency names through the current game
localization, retaining the definition name when no translation exists. Its title,
level suffix and Claim/Later buttons use the existing game strings. New status and
scalar-reward labels have English defaults: `eclipse.lottery.saved`,
`eclipse.lottery.unavailable`, `eclipse.lottery.failed`, `eclipse.lottery.empty`,
`eclipse.lottery.Money` (Coins), `eclipse.lottery.Bonus` (Gems), and
`eclipse.lottery.Experience`. These are host localization keys, not a new Lua API.
The preview shows the saved slot's sprite when available, resolving item names
through their normal equipment/seal icon paths and preserving qualified mod sprite
IDs. If the slot artwork is unavailable, it tries the first saved item's icon.
Missing or unreadable optional artwork leaves the text and Claim button available.
The image keeps its aspect ratio inside the parchment's scroll area; short reward
summaries no longer reserve a large blank text section. Full-game visual acceptance,
translations of the new fallback labels and paid rerolls remain unfinished.
There is no public Lua lottery API yet.
Fight-end lottery deferral checks the reward already composed for that result,
including its selected reward scope and mode. A lottery in another win-count
reward does not suppress the current fight's normal quest events. Detection does
not re-evaluate reward expressions after progression or level-up, and losses and
surrenders do not create a deferred winning claim.

An awarded battle lottery now saves its draw with a `BattleEnd` continuation before
presentation. The reward opens on a non-combat scene, including after profile
reload. Claiming saves the inventory change first; accepting the deferred FightEnd
or RaidFightEnd event then checkpoints matching quests and saves a dispatch marker
before requesting their execution. A reload between those steps resumes event
acceptance without rerolling or granting the saved prize again. Encounter identity
prevents repeated preparation of the same tracked encounter while its claim record
is retained. A pending claim or unsettled continuation blocks starting another
fight and replacing the draw. This applies to the single pending claim per profile;
it is not a multi-claim inventory or a paid-reroll system.

Automated fixtures exercise claim/continuation recovery with controlled native
quest queues and disk services. Full-game scene transitions, quest resume behavior
and end-to-end crash recovery still require playtesting; the dispatch marker does
not certify that every asynchronous quest action has finished.
Automated claim fixtures control reward grants and disk saves; full-game
settlement/restart and visual verification remain pending.

When a host supplies a quest invocation ledger, claim completion also records a
receipt for that action in the same save. Replaying an acknowledged action does
not create another prize. A new quest run uses a new ID; pending receipts prevent
replacement of an unfinished run. The native quest runner now exposes its start,
checkpoint-resume, and completion boundaries to this bookkeeping. Quest claims are
identified by quest file and name under `EclipseQuestClaims`; an unfinished run is
reused when reopened, while a completed run can start a new draw. Resuming after
the action definition changes is rejected so an old receipt cannot acknowledge a
different action. Concurrent instances of the same lottery quest and nested action
identities are not supported by this bridge yet.

Native quest checkpoints also preserve lottery condition context in an optional
`EclipseLotteryContext` child: `inLottery`, the last spin number, the awarded item
identity, and the raid identity. Restoring a checkpoint recovers those values
before evaluating later quest actions. Ordinary checkpoints with none of these
values do not acquire this metadata; overwriting a checkpoint with ordinary
context removes a previous lottery context. Older saves without it retain the
normal defaults. This preserves context only; it does not implement paid spins
or paid lottery continuation.

Each queued native quest captures its event parameters when it is accepted. Later
events cannot replace its fight identity, lottery item, spin or purchase payload
while it waits for another quest to finish. Fight IDs and purchase payload values
are copied; item definitions and language objects remain shared catalog references.
On profile reload, each quest restores its own checkpoint instead of applying the
first queued quest's parameters to the entire queue. The saved average frame rate
is restored along with the other native fight fields. Resumed quests retain their
saved action index; queue restoration does not rerun the initial checkpoint and
reset them to the first action. This preserves the fields
the checkpoint format supports; it does not add persistence for every transient
event field or make concurrent instances of one quest independently resumable.

## Before publishing an update

Test with a save from the previous version, including owned and equipped items and any saved behavior progress. Also test disabling and re-enabling the mod. Restart Eclipse after file changes; replacing files does not reload existing definitions.

The save records mod versions and content metadata for diagnostics. Changes to that metadata do not invalidate the player's save.

Owned weapon `tactic_subtype` (API 0.51) participates in the content fingerprint when specified. Changing the AI group is a content change; omitting the field preserves the previous weapon fingerprint format and subtype fallback.

Weapon AI group overrides through `items.set_tactic_subtype` (API 0.52) record the target, owner and group in the fingerprint. An empty group is an explicit subtype fallback and differs from no override.
