# Phase 3: profile achievements and controlled asset replacement

API **0.7.0**. The core showcase has now been user runtime-tested: achievement
flow, restart persistence, loss/surrender filtering and the replacement icon.
See the [recorded playtest](example.phase3/README.md#recorded-user-playtest) for scope.
The preceding Phase 2 showcase was user-tested,
including repeatable Volcano, its shield bars and reward presentation.

## Counters and achievements

`content.register` permits static definitions. `progression.read` and
`progression.write` separately permit reading and advancing the owning mod's
profile counters after profile load:

```lua
local wins = sf2.counters.register { id = "wins", maximum = 1000000 }
sf2.achievements.register {
    id = "first_win", counter = wins, threshold = 1,
    title = sf2.localization.key("first_win"),
    description = sf2.localization.key("first_win.description"),
    icon = sf2.assets.sprite("sprites/medal"),
    hidden = false,
}
-- Inside a supported Lua callback:
local total = sf2.counters.add(wins, 1)
local current = sf2.counters.get(wins)
```

Counter handles are opaque and owned by the registering mod. Maximum is an
integer from 1 to 1,000,000,000 (default 1,000,000,000). Add accepts nonnegative
integers up to 1,000,000,000 and saturates at the definition's maximum. Previously
saved values are never decreased when a mod lowers its maximum. Achievement
thresholds must be within that counter's bounds. IDs, localization references,
dependency ownership and duplicates are validated before transaction commit.

This uses the recovered `AchievCounter` / `Achievement` definitions and
`UserAchievements` roster records. Definitions are installed before roster parsing;
threshold crossings add native saved achievements and request a profile save.
The Profile achievement list displays earned records and the next milestone with
its native progress indicator. Rewardless achievements are immediately marked
claimed: there is no fake gem claim or separate unlock-popup promise.

These are local profile achievements. They have no platform achievement IDs and
are excluded from platform achievement reporting/retry queues.

Disabling a mod removes its definitions, leaving its saved counters and achievements
intact. Re-enabling it and reloading the profile restores them. Changing IDs creates
new progression; it does not migrate the old IDs. Unknown saved entries are retained.
These counters do not automatically subscribe to every native counter event: custom
conditions are Lua code in supported callbacks. Neither raw roster access nor a
generic counter-operation DSL is exposed.

The archived `SenseiStoryFinished` and enchantment quest entries demonstrate domain
intent, not base-game authority. Native vanilla counters remain untouched. A downstream
content mod must implement its own completion conditions through supported events.

## Asset replacement

With `assets.replace` and an explicit dependency:

```lua
sf2.assets.replace {
    target = "core:UI/Skills/SkillsEnch02.EnchantmentFrenzy",
    replacement = "sprites/my_frenzy",
}
```

Logical loose asset IDs omit file extensions. The replacement must be owned by the
caller and the target by a declared dependency. Both must exist and have matching
supported runtime kinds: sprite, texture, model or audio. Unsupported kinds,
including arbitrary config text, fail registration. Animation remains authored
through the existing typed move/animation API; opaque native animation blobs do
not gain a global replacement contract in this release.

Replacement declarations are part of the registration transaction and content
fingerprint. Their owner, target, replacement and kind remain available in catalog
provenance. A second claim on a target fails explicitly, including identical claims;
the failed mod contributes no partial registration. Cycles fail. Load order is not
a conflict-resolution mechanism. No deletion or filesystem shadowing is offered.

Resolvers activate the committed redirects after script initialization. Qualified
loads, core packaged runtime loads, atlas members and model text loading use them.
Native atlas members retain their expected names. Development model XML cannot
bypass an explicit runtime model replacement. Unmounting restores base resolution;
existing live objects are not retroactively rewritten. Restart the game after
enabling/disabling a replacement mod. Boot screens and arbitrary direct
`UnityEngine.Resources` loads outside the supported runtime loaders are not covered.
Prefer pointing a new item/location at a namespaced asset when a global swap is
unnecessary.

## Remaining configuration classification

The reproducible ledger is [PHASE3_CONFIGURATION_AUDIT.json](PHASE3_CONFIGURATION_AUDIT.json).
Run `python Tools/AuditPhase3Configuration.py` to detect source drift. The audit
compares semantic XML/JSON sections, ignoring indentation, and records hashes of
both sources plus a decision for every differing section.

| Domain | Decision |
| --- | --- |
| Achievement counters | New owned counters/achievements plus Lua behavior; preserve shipped counters. |
| Service timeouts, ad counter reset, social/Internet config | P2 semantic service opt-outs where supported; transport details stay private. |
| Lottery reroll costs | Base-owned economy; no raw settings override. |
| Hit pause, slow mode, counter-punch threshold, resistance deletion | Native combat/reconstruction compatibility; no unproven global tuning API. |
| Benchmark, device lists, quality heuristics, tactics caching | Platform/reconstruction configuration. |
| Debug statistics and dependency/SDK exclusions | Build concerns; runtime opt-out does not remove compiled SDK dependencies. |
| Camera width, hints, hint timeout, credits speed | Presentation differences. No nominal setter added without a supported consumer; `BasicGUI` getter usages were checked. |
| Credits translations/attribution | Presentation content; retain base credits. |
| Loader branding, intro, language flags | Boot/platform presentation; gameplay mod initialization is too late for a general boot API. |
| Anti-cheat item exceptions | Compatibility data; not a mod-facing anti-cheat toggle. |

No source difference alone is treated as proof of intentional DE policy or obsolete
upstream drift. The ledger classifies ownership; it does not silently apply DE values.

## Verification and playtest

See [example.phase3/README.md](example.phase3/README.md) for the numbered user test.
`TestPhase3Runtime.ps1` executes the public Lua sample and registry/redirect contracts.
`TestPhase3Progression.ps1` compiles the production native parser/save classes and P3
adapter with host-boundary stubs. `ValidatePhase3Assets` runs native sprite loading
in an isolated Unity project. These checks complement, rather than replace, an
in-game forge → equip → victory → Profile → restart playtest.

### Recorded validation, 2026-09-09

- Four managed assemblies compiled successfully.
- P3 public Lua/registry tests: 29 checks; native progression/save adapter: 14 checks.
- Isolated Unity 2022.3.62f3 asset test passed PNG import, direct/qualified loads,
  atlas-member names, cache lookup and restoration (`TestPhase3Assets.ps1`).
- Phase 1 showcase, Phase 2 combat/mode, core/save, P1A/P1B, startup and 1,282
  Underworld runtime assertions passed.
- Configuration audit: 45 classified semantic sections/documents; `git diff --check` passed.
- `AuditUnderworld.py` still reports the existing 40 missing loose-art references;
  it does not validate packaged asset resolution. No new raid content was added.
- Subsequent user playtest confirmed the core showcase flow, restart persistence,
  loss/surrender filtering and sword replacement. No additional confirmation was
  requested for point 5. Mod removal/reinstallation has automated coverage but
  was not separately reported in the user playtest.
