---
title: Tune PvP balance
description: Edit JSON presets for versus damage, recoverable block damage and nonlethal blocking.
---

PvP balance profiles are JSON files. They affect local versus, online versus and
training. Replay and spectator playback use the rules captured for their match.
Campaign fights and title-screen sparring retain their existing combat rules.
These presets are separate from Lua mods: installing one does not register
equipment, grant a Lua capability or change campaign saves.

## Edit a preset

In Unity, open **SF2 → Multiplayer → PvP Balance**. Load a preset, duplicate it
with a new ID, then adjust damage and recovery. The editor supports category,
equipment and move overrides, searching, importing/exporting JSON and an
effective-value preview that shows which override supplied each multiplier.
The **Browse** button can list catalog IDs after entering multiplayer Play Mode.
You can also type exact IDs while the catalog is unloaded.

**Save** writes a project preset to
`Assets/Resources/EclipseVersus/Balance/<id>.json`. Enable **Save as local
override** to write to the current client's data folder instead. **Save and use
for new matches** selects the saved preset. **Save and restart training** saves,
selects and starts a new training fight with those rules; it is available during
a running training fight. Changes do not mutate an active fight.

Choose **BALANCE** in the local or training setup, or in the direct host's lobby.
Changing balance in the paused training menu restarts training when you resume.
Rematches retain the original match's rules; return to the lobby to choose a
different preset. The room setup's **YOUR BALANCE** selects the preset this
client will propose when it becomes the host of a paired fight. It is not a
server-wide room rule.

## Install a shared JSON file

Project presets are bundled at build time. Player-installed presets go in
`PvpBalance/` under Eclipse's persistent data folder. A local file with the same
profile `id` overrides the bundled preset. The selected ID is stored separately
in `PvpBalance/selected.txt`. At most 128 local JSON files are loaded, in filename
order; invalid files are ignored with a warning, and a default preset remains
available. Importing a file into the editor does not install it until you save.

Multiplayer Play Mode clients have their own data folders under
`MultiplayerPlayMode/<player tag>/`. Put the JSON in each client's `PvpBalance/`
folder when testing local overrides, or use a project preset shared by the
editor clients. Profile changes on disk are refreshed when opening the editor's
preset menu; changing rules also requires selecting the updated preset for a
new lobby or match.

Online, the fight host chooses the rules. Every participant, including
spectators, must have a profile with the same gameplay hash. Profile names and
IDs may differ between clients. The hash depends on gameplay values, not
whitespace, JSON property order or override array order. Changing host rules
clears the guest's readiness; readiness acknowledges that exact rules hash.
Missing profiles show an install message. JSON is not automatically downloaded
from an opponent.

This change uses netplay protocol **4**, room protocol **5** and replay format
**3**. Rebuild room servers and use matching clients together. Older replay
files can still be listed, but playback is refused because they predate these
combat rules. New recordings embed the complete immutable balance JSON and its
hash, so you can play them without reinstalling the original preset, provided
the game build and other gameplay content remain compatible.

## Profile fields

A file must be a JSON object, no larger than **32 KiB in UTF-8**. Duplicate keys,
unknown fields, invalid types and unsupported schema versions are rejected.
Numbers must be finite. Optional fields use the defaults below when omitted.

| Field | Required | Default | Meaning and limits |
| --- | --- | --- | --- |
| `schemaVersion` | Yes | — | Must be `1`. |
| `id` | Yes | — | Unique preset ID: 1–64 letters/digits, hyphens or underscores. Use simple ASCII IDs for portable filenames. |
| `name` | Yes | — | Display name: 1–64 characters, not blank. |
| `hitDamageScale` | No | `0.5` | Native unblocked strike multiplier, `0`–`10`. |
| `blockedDamageScale` | No | `0.25` | Native blocked strike multiplier, `0`–`10`. |
| `recoverableBlockedFraction` | No | `1` | Fraction of actual blocked damage added to grey health, `0`–`1`. |
| `recoverOnHit` | No | `0.5` | Grey health restored per unit of actual damage you inflict with an unblocked hit, `0`–`10`. |
| `recoverOnBlock` | No | `0.25` | Grey health restored per unit of actual damage you inflict when your opponent blocks, `0`–`10`. |
| `recoverableLossOnHit` | No | `0` | Grey health discarded per unit of incoming unblocked damage, `0`–`1`. This is additional pool loss, not additional current-health damage. |
| `minimumLifeOnBlock` | No | `0.0001` | Positive floor as a fraction of maximum life, `0.000001`–`0.1`. Default is 0.01% of maximum life. Blocking cannot kill; this cannot be set to zero. |
| `categories` | No | `[]` | Category damage overrides, at most 256 entries. |
| `equipment` | No | `[]` | Exact equipment ID damage overrides, at most 256 entries. |
| `moves` | No | `[]` | Exact attack animation name damage overrides, at most 256 entries. |

Damage multipliers apply after native block/critical calculations and before
the native lethal damage cap. They do not change block detection, hit reactions
or knockback. Separate scripted health changes do not receive these multipliers.

## Damage overrides and inheritance

Each override object requires `id` and at least one of `hitDamageScale` or
`blockedDamageScale`, with the same `0`–`10` limits as global values. An omitted
or `null` multiplier inherits the previous layer. IDs are case-sensitive, must
contain 1–128 nonblank characters and cannot repeat within the same array.

Each multiplier resolves independently in this order:
**global → category → equipment → move**. A later layer replaces the value;
the layers do not multiply one another. An equipment entry can replace blocked
damage while a move entry replaces unblocked damage.

Categories are the source equipment's `SubType`; unarmed attack intervals use
the category `Unarmed`. Weapon attacks use the equipped weapon ID, ranged
attacks use the ranged item ID and magic attacks use the magic item ID. Unarmed
intervals still allow an override for the equipped weapon. Move IDs are the
attacking model's current animation name, also used by the native strike path.

This complete example lowers unarmed hit damage and gives one move a different
blocked multiplier. `HighKick` is a shipped animation name; use **Browse**
or your loaded content's exact animation IDs for other moves.

```json
{
  "schemaVersion": 1,
  "id": "practice",
  "name": "Practice balance",
  "hitDamageScale": 0.5,
  "blockedDamageScale": 0.25,
  "recoverableBlockedFraction": 1,
  "recoverOnHit": 0.5,
  "recoverOnBlock": 0.25,
  "recoverableLossOnHit": 0,
  "minimumLifeOnBlock": 0.0001,
  "categories": [{ "id": "Unarmed", "hitDamageScale": 0.4 }],
  "equipment": [],
  "moves": [{ "id": "HighKick", "blockedDamageScale": 0.2 }]
}
```

## How grey health works

Suppose a blocked strike removes 10 health after defense and the nonlethal cap.
With `recoverableBlockedFraction: 1`, the defender gains 10 grey health. At
`0.5`, only 5 of those health points become recoverable. The grey segment appears
beside current health inside the same fight HUD bar, following its slanted edges
and fill direction. It starts at the visible live-health edge while that edge
animates; it does not tint or show through existing health. Recoverable health
never exceeds missing health.

If that defender later lands an unblocked attack that actually removes 20 health,
`recoverOnHit: 0.5` restores up to 10 from their existing grey pool. Making the
opponent block an attack that actually removes 8 restores up to 2 at the default
`recoverOnBlock: 0.25`. The recovery is based on damage inflicted, not on the
attacker's maximum health. No pool means no healing. Recovery cannot revive a
knocked-out fighter or exceed maximum health.

Blocked hits leave at least the configured positive health floor. A fighter
already below it takes no further blocked damage and is not healed up to it.
Unblocked attacks can still knock them out. At the floor, a blocked attack that
inflicts no actual damage creates no new grey health and grants no recovery.
Round resets and full-health refills clear the grey pool; death clears it too.
Training's infinite/refill settings still apply, so use **NORMAL** health when
checking chip and recovery. Timeout winners use current health, not grey health.

This is an Eclipse PvP recovery rule inspired by fighting-game grey health. It
does not implement Tekken's Heat system or animation-specific Tekken recovery.

## Verify a change

Use the effective-value preview for inheritance, then test your intended weapon,
move and blocked/unblocked outcomes in training. Try repeated blocks at low
health and attacks with an empty recovery pool. Finally run an online match
using matching JSON on both clients, and save/play its replay. Managed rule
checks and a rendered HUD fixture do not replace a full multiplayer playtest.
