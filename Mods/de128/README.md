# DE128 - Definitive Edition

The owner-supplied archive is now the source of truth for DE content. See
[SOURCE_CORPUS.md](SOURCE_CORPUS.md) for acquisition status and reconciliation requirements.
The designated 1.6 GB download is still blocked by Google Drive quota. A
separate local owner drop supplies the reviewed raid file and art used below;
older comparisons remain historical until the full corpus can be reconciled.

DE128 is an ordinary downstream Eclipse mod. Version `0.36.0` restores both
native phases of Widow Teleportation in twelve Underworld boss fights and
Widow's Demon survival wave. It preserves the archived RaidCharge control,
enemy safety gates, 100-unit finishing alignment, nine-edge strike, and
encounter-specific opening and recast timing. All thirteen encounters passed
native Unity acceptance; the Wind Wolf Power arena's early hot-ground rule
allows only the opening cast and strike in its unattended test. DE128 reads no
XML at runtime. Version `0.35.0` restores Arkhos's
Rat Wave and Tenebris's Fear Ray in normal and Power Mode Underworld fights.
Their original native casters use the archived RaidCharge input, mod-shipped
binary clips, 600-frame opening and recast gates, and native projectile attacks.
Tenebris no longer inherits the packaged core's extra invulnerability window.
The four fights passed native Unity casts, attacks, recasts and map return.
Version `0.34.0` restores
Hoaxen's Tentacles, Hunter's four Fly attacks and Berstuuk's full Root Potion
projectile, trigger and hitbox in both normal and Power Mode Underworld fights.
The bosses use archived RaidCharge inputs and opening/recast timing; Hunter
steers into the wall range required by his native Fly moves. Six fights passed
native Unity acceptance with two casts and a return to the map each. Berstuuk's
hidden Root Potion items use the archived model geometry. Version `0.33.0` restores
Dandy's complete native Underworld Lightning Chain in normal and Power Mode.
His RaidCharge cast creates the hidden chain actor, runs five linked phases
with four damage-capable attack intervals and effects, then cleans up. The
opponent tactics use the archived 300-frame opening and 600/500-frame recast
delays. Native Unity acceptance observes two complete casts and map return in each mode;
the test player moves to the caster's required 450-unit range. Version `0.32.0` restores
Saturn's native Underworld Blaster in normal and Power Mode. A guarded move
patch changes the original caster to RaidCharge priority 200 while retaining
its linked pistol and two damaging projectiles. Mode-specific tactics honor
the archived 300-frame opening delay and 660/550-frame recast delays. Native
Unity tests observe both casts, the projectile graph and map return in each
mode. Version `0.31.0` restores
Blackness's three-phase Underworld Grasp in normal and Power Mode: the
RaidCharge cast creates a named BlackHand, transitions it into its damaging
attack, shows the hand effect, and respects the 600-frame opening cooldown.
Native Unity tests observe both complete fights through map return. Version
`0.30.0` restores
Gatekeeper's archived RaidCharge Power Field in normal and Power Mode. The
reusable Eclipse effect renderer now follows two live model nodes and places
background electricity between the arena and fighters. Native Unity tests
observe the cast, hidden field actor, attack, visible effects and map return.
Version `0.29.0` restores War's
archived Whirl attack in both normal and Power Mode Underworld fights. Native
Unity tests observe the RaidCharge cast, attack interval, three scheduled effect
stages, sound cleanup actions and return to the map. Version `0.28.0` restores Hermit's
archived Storm caster, repeated hidden-item Storm spawns, idle continuation and
victory transition in the second Demon survival wave. Native Unity tests observe
the entire cast through map return and the authored victory move after a player
defeat. Version `0.27.0` restores Butcher's
archived Earthquake caster and hidden-item projectile in the Demon survival wave.
The native test reaches his third wave, observes the RaidCharge cast, projectile
attack interval and deletion, then returns to the Underworld map. The same
acceptance suite reaches Girl Fan in Mercenary survival's 23rd wave and verifies
her ceremonial gear in combat. Version `0.26.0` restores Wasp's
four archived Fly attacks in the Demon survival wave, with the native RaidCharge
input, cooldown, wall ranges, hit behavior, effects and a Fly-aware Aggressive
tactic. The native test advances through three opponents, observes the ability
in combat and returns to the Underworld map. Version `0.25.0` restores the
two archived Underworld spotlight encounters and three hidden core equipment
identities used by late raid opponents. Version `0.24.0` reconciles the
local owner Underworld raid file, restores twenty boss portraits and Haunted
Prince's map button, and verifies all 29 changed fights in native Unity.
Version `0.23.0` restores all
five archived final Eclipse Titan equipment rewards with their own models,
localized names and player-level enchantments. Version `0.22.0` restores
Berstuuk's archived body and mask geometry and verifies all 76 Underworld
encounters in a native Unity fixture. Version `0.21.1` includes a
verified Underworld boss-alignment repair in the Eclipse template resolver.
Version `0.21.0` restores
normal shop availability and archived starting profiles for all 221 hidden core
equipment entries, with 99 price, 20 icon and three model differences patched.
Version `0.20.0` added the complete DE Underworld (below), and
`0.19.0` activated the Sensei story. DE128 also restores ten missing weapon
listings and thirteen other equipment listings, and enables the archived
ChineseSwords combat/preview graph for Jian alongside four earlier equipment
combat-family corrections through Lua. The working tree offers 221 existing core
items at their archived gates and completes saved, already-paid forge orders
without waiting. Ascension remains disabled; Master of Style and Relentless
follow the archived DE XML.
Definitions, translations and behavior are authored through the public Lua API.

## Underworld (since 0.20.0)

`main.lua` installs all eight archived Underworld tiers (`underworld.lua`) and
their story (`underworld_story.lua`): 76 battles with normal/Power Mode pairs,
76 fights, 104 opponents on 66 templates, 180 rewards with forge-material drops,
and the archived fight rules. The Underworld toggle stays hidden until the player
has beaten Lynx 2; the next map visit then plays the archived intro, focuses the
Volcano boss and ends in the dojo. Each of the 32 bosses greets the player once
before the first fight and speaks once after the first loss and first win.

The fight graph is generated by `Tools/GenerateDE128Underworld.py` from the
reviewed local owner raid file, with historical DE templates and translations.
The owner raid file corrects seven perk references and 21 avatar rows, removes
five obsolete health parameters and one cooldown override, changes Invisible's
preview, and gives Haunted Prince his own map button. The generator pins that
source hash and refuses an unreviewed revision. Map buttons for eleven event
raids, twenty upscaled boss portraits and two May portraits ship under
`assets/sprites/underworld/`. Berstuuk's hidden armor and mask use archived
geometry packed as `.modelz` assets and load through the normal model handle
API. DE128 Lua never opens XML. The core item-list compatibility loader supplies
the hidden ceremonial armor, ceremonial helm and needles identities used by the
archive. The `LightInTheDarkness` rule now follows the player in Blackness and
Son of the Sun Power Mode through Eclipse's typed rule API. The
manifest adds `presentation.navigate` for the dojo
change. Automated checks compare the generated data with the archive. The
headless Unity 6 fixture covers native entry, arena and fighter rigs, 30 combat
frames and surrender/map return for every Underworld fight, plus the archived
boss story sequence matrix. The 29 fights changed by the reviewed owner raid
file passed the same native path, with all new portraits and Prince buttons
decoded. The two spotlight encounters passed native shader, fighter-position,
arena and cleanup checks. An interactive combat and presentation playtest
remains outstanding.

## Sensei story (active since 0.19.0)

`main.lua` calls `sensei_story.install_default()`. It registers the six-act young
Sensei story on the existing map pages: 12 normal/Eclipse battles, 23 fights,
34 opponents, 57 reward slots, first-entry dialogue, victory and defeat
dialogue, unlock notifications and the conditional RaidCharge restriction.

Two inputs are **synthesized**, not recovered, because no available source
contains them (Step 49 in `PRODUCTION.md` records the evidence):

- `Guard_Girl`/`Guard_Man` (`sensei_guard_templates.lua`): the core Default
  template plus a Female/Male voice, the shape of every comparable story
  character template. Guard rows keep their archived equipment and names.
- RaidCharge availability (`sensei_raid_charge_state.lua`): true while an equipped
  item carries one of the twelve forged ability enchantments, or the full Neo
  Wanderer set is worn. Uses the new `enchantments` field of
  `sf2.profile.equipment()`.

The prince's `Sphere1` is the restored Minor Charge of Darkness. Portraits and
previews missing from core ship under `assets/sprites/sensei/`. The manifest now
also requests `story.events`, `story.progression`, `profile.read`, `state.read`,
`state.write` and `ui.create`.

Automated checks cover registration, archive comparisons and the Lua flow with
controlled hosts. No native campaign playthrough of the activated story has been
done yet: play it on a test profile (reach each act's unlock, fight guards and
bosses in both modes, lose once, check portraits/previews and a forged ability)
before relying on it. The per-component notes below predate activation.

Pending `scripts/content/sensei_act_one_opponents.lua` ports normal/eclipse young
Lynx, including encounter-specific enchantment strength/chance settings. It is
**not loaded by the entrypoint**. Act I references `Guard_Girl` and `Guard_Man`
templates absent from the historical XML available here; no substitutes have
been invented. Story gates, rewards, presentation and live encounter acceptance
also remain unfinished. See Step 32 in `PRODUCTION.md`.

Pending `scripts/content/sensei_boss_opponents.lua` extends those loadouts to all
six young bosses in normal and Eclipse variants (12 warriors, 50 perk instances).
It preserves archived equipment, tactics, attributes, alignment rows and perk
settings, including explicit activation probabilities and Hermit's storm duration.
All rows match historical XML through the production adapter; native perk clone
checks pass. It remains outside main.lua, with guard templates, encounter rules,
boss AI/trigger playtests and full story acceptance unfinished (Step 38).

Pending `scripts/content/sensei_guard_opponents.lua` supplies the other 22
normal/Eclipse loadouts, including the prince, and combines them with the bosses
in the archived encounter order. Call its `register` function only after resolving
verified `guard_girl`, `guard_man` template handles and the `sphere1` item handle.
`sensei_dependencies.resolve()` supplies `sphere1`: the archived prince item is the
restored Minor Charge of Darkness (Step 48). The guard templates remain missing.
It provides no substitute for those missing inputs and has no registration side
effects when merely required. Complete ordered projections cover 34 loadouts in
23 encounters, with identity-only test fixtures for the unresolved inputs; this
does not validate their models, AI or charge behavior (Step 40).

Pending `scripts/content/sensei_fight_rules.lua` provides the unconditional rule
lists for all 23 Sensei fights: player equipment/identity, opponent anti-shock and
Ronin's Eclipse-only damage modifiers. Historical ordered rows and native mode
parsing are checked. The conditional RaidCharge rule is separate and unattached;
the recovered conditional wrapper ignores its condition, so the encounter
assembler uses a separate Lua behavior. This module also stays outside
main.lua (Step 39).

Pending `scripts/content/sensei_encounters.lua` assembles all twelve normal/Eclipse
battle entries and 23 fights from the opponent, rule and reward modules. It takes
complete opponent rosters and a separately implemented conditional RaidCharge
rule; it does not implement that condition. `sensei_battles.lua` pairs entries on
the six existing map pages, and `sensei_battle_text.lua` supplies 56 translated
labels. The returned `final_ids` contains all six qualified normal final-fight
IDs; `prior_final_ids` contains the first five for notifications. The original
five `finals` handles remain available. These factories remain outside main.lua;
source comparisons use controlled missing identities and a controlled availability
reader only in tests. See Step 41 for the native locked-pair fix and acceptance limits.

Pending `scripts/content/sensei_story.lua` connects the encounter graph, entry
scenes, victory sequences and notifications in the required subscription order.
Its `install(opponents, is_raid_charge_available, portraits)` takes verified
rosters, a faithful perk-state reader and all entry/victory portrait handles,
including `character_sensei` for notifications. It returns the encounter graph.
The caller must request `content.register`, `story.events`, `story.progression`,
`profile.read`, `state.read`, `state.write`, `ui.create` and `combat.effects`
(for the charge control callback). Do not load the component installers separately.
`Tools/TestSenseiStory.ps1` executes the combined Lua flow with controlled
opponents, availability and presentation hosts. All six unlocks, 17 entries,
23 victory cards, six defeat dialogues, loss/retry, save/profile separation and
outro cancellation are covered. `sensei_art.story_portraits()` supplies every
required portrait. Missing guard templates, the RaidCharge availability producer
and native campaign acceptance still prevent activation in main.lua. See Steps 46–48.

The mod now ships twelve Sensei sprites under `assets/sprites/sensei/`: five Act
II–VI battle previews, five young-boss portraits, the pirate portrait and a copy
of `character_sensei` (a native resource missing from the core art catalog that
public sprite IDs use). `Tools/ExtractDE128SenseiArt.py` rebuilds and hash-checks
them from `ResearchSources`; `Tools/VerifyDE128SenseiArt.cs` decodes them in
Unity. Battles and opponents pass these as sprite handles to `preview`/`avatar`.

Pending `scripts/content/sensei_raid_charge.lua` supplies that conditional behavior
through `register(is_available)`. Its caller must supply a verified boolean reader
for the archived perk-dependent availability; no inventory/loadout approximation
is supplied. It reapplies the block each round using the generic C# control hook,
which composes with native rules and other behavior instances. Lua and isolated
Unity checks exercise the condition, but the real perk-state producer and ability
button availability remain unresolved. It is not loaded by main.lua (Step 42).

Pending `scripts/content/sensei_rewards.lua` contains all 57 normal/eclipse
reward slots across the six acts, including experience, gems and performance
bonus bases. It also stays outside the active entrypoint until the story is ready.
Native parsing and detached result calculations are checked against historical
XML; profile settlement and the full story remain unverified (Step 33).

Pending `scripts/content/sensei_entry.lua` ports the 17 normal first-entry
sequences: 39 cards, seven timed lines and 938 translations. Call
`install(normal_fight_handles, portraits)` with all six verified normal fight
arrays and every portrait identity in `sensei_entry_data.lua`. The generic
`sf2.story.before_fight` continuation holds the original map launch until its
Fight button; interruptions restart unfinished intros, completed greetings do
not replay, and the Shogun acknowledgement is saved separately. Installation
requires `story.progression`, `ui.create`, `content.register`, `state.read` and
`state.write`. These modules remain outside main.lua; source/runtime checks
are described in Step 45 of the production record, not a full story playtest.

Pending `scripts/content/sensei_victory.lua` implements the six post-victory
sequences with 23 dialogue cards and 448 translations. Call
`install(final_ids, portraits)` before installing notifications:
`final_ids` contains the six qualified normal final-fight IDs; `portraits` maps
the seven names in `sensei_victory_data.lua` to verified sprite handles.
The final silent-lock ActScreen uses the reusable `sf2.ui.act_screen` runtime API.
Busy requests retry when the map is available; cancelled requests do not complete the act.
The shared `sensei_state.lua` registers both coordinators' saved fields once.
Cards resume after interruption, and pending dialogue delays unlock notifications.
The last act completes only after its 180-frame outro callback. Real portrait
acceptance remains open; none of this is loaded by main.lua.
See Steps 43–44 for tests and remaining story work.

Pending `scripts/content/sensei_progression.lua` computes the six acts eligible
to unlock after a victory, using saved tournament/prior-act wins and supplied
opened flags. Notification and persistence are handled by the pending coordinator
below. All 2048 prerequisite histories are compared with archived quest
conditions; the generic `sf2.profile.fight` query supplies the saved counters.

The generic `sf2.battles.set_locked` API can now update an owned, already revealed
battle on an unblocked map without resetting replay counts or fight history.
See Step 35 in `PRODUCTION.md` for its native map acceptance.

Pending `scripts/content/sensei_map.lua` now sequences the six initial reveals,
per-act unlock and map focus through public APIs. It stops on refusal and can be
retried without resetting existing progress. These map effects are tested against
the historical notification actions (Step 36).

Pending `scripts/content/sensei_notifications.lua` now coordinates ordered map
notifications, all 56 historical translations, Back/OK acknowledgment, Act I's
native Eclipse-mode exit, retry on refused actions, and persisted pending/opened
flags. Scene/profile cleanup never acknowledges. Controlled Lua/save checks pass;
the mode switch passes isolated native Unity acceptance. Full dialog rendering,
encounter assembly, missing guard templates and complete story/save acceptance
remain open. The coordinator is **not loaded by the entrypoint** (Step 37).

`scripts/content/chinese_swords.lua` registers both moves and Jian's subtype;
`chinese_swords_data.lua` contains their typed combat/presentation data. The mod
bundles the unchanged recovered animation binary, with no runtime XML dependency.
Complete archive comparisons, native parsing and binary decoding pass. An isolated
live fight also passed native double-tap/Forward selection, four attack intervals
with bound weapon edges, four timed sound actions and animation completion. AI,
hit contact, audible output and shop-preview acceptance remain open.
See Steps 12–13 in `PRODUCTION.md`.

The restored listings are Super Knives, Batons, Dragon Knives, Poleaxe, Kelt Axes,
Fans, Moon Fans, Imhotep Axes, Chinese Swords and Giant Sword. Their Lua definitions retain
archived act/level gates, gem prices, default enchantments and art references.
Kelt Axes and Moon Fans retain the archive's placeholder icon. Moon Fans uses
`initial_stats = {}` to preserve its absent initial damage attribute. A native
check confirms its first normal upgrade supplies 766 damage, matching the archive.
Steps 14-15 record the restored definitions, generic API support and verification
limits. Purchase/equip/save/reload acceptance remains open.

The additional equipment is Dragon Carapace, Old Legionnaire Armour, Samurai
Armour, Gabled Helm, Dragon Helm, Dragon Boomerangs, Dragon's Breath and Lightning
Arc, plus Minor, Medium and Large Charge of Darkness, Blast of the Void and Mind
Throw. Stats, prices, group/level gates and default enchantments match archived
item definitions. Samurai Armour deliberately retains only head defense (914).
The three ranged/magic families use base moves. `shared_moves.lua` now applies
five guarded changes: the heavy ranged uninterrupt end, Chakram hit reaction,
heavy ranged preview sound timing, and not-Stun conditions for MassBomb and
LightningArrow. Sphere1 and Sphere2 each have nine Lua-authored combat and shop
moves and reuse three unchanged recovered animation binaries. Sphere3 adds five
more moves and two unchanged binaries, retaining enemy-centered placement,
five ordered attack edges (including the archived repetitions), downward impulse,
and the native `Physycal` hit reaction. Complete archive
comparisons cover their different attack edges, effects, alignments and conditions.
Sphere2 retains its Stun restriction and three attack edges; it is not a recolored
Sphere1. ComboSphere3 adds four moves and Blast of the Void, retaining its bounded attack
window, `HighLong` reaction and archived placeholder icon. MindThrowNormal adds six
moves, five unchanged recovered binaries and the Mind Throw shop listing. Its
innate Lua perk sets an instance-owned flag on casting and clears it on the victim
reaction or wall cleanup; native ModExpires selects the caster's follow-up.
Complete move/template comparisons and the three archived perk trigger traces pass.
An isolated Unity fight passed native contact in the normal idle stance, the owned
victim reaction, one innate flag expiry, caster follow-up and final attack interval.
The recovered projectile passes above the crouched fists idle pose in the native
fixture. Contact across other stances and reconciliation with the deferred
owner-supplied corpus remain open; see Step 31 in `PRODUCTION.md`.
Hidden NPC equipment remains unregistered. Numerical damage, wall-miss cleanup,
visual/audio output and shop-preview acceptance remain open. See `PRODUCTION.md`
for the native lifecycle evidence and its limits.

## What this version does

- Makes **new and saved pending forge/enchantment orders instant**, using the normal recipes and
  material costs.
- Disables the six supported service groups: paid offers, battle pass,
  advertising, rewarded video, online services and payments. The verified native
  consumer suppresses matching quest groups and battle-pass quest sources.
- Registers `de128:items/weapon/titans_desolator`, reusing the core sword model,
  icon and `TitanGiantSword` move family. Its English name is registered in Lua;
  its innate loadout references core `PERK_TITAN` and `PERK_ANTI_SHOCK`.
- Restores the complete five-item winning reward of final Eclipse Titan fight 6:
  Desolator, Titan's Form, Titan's Helm, Titan's Harpoon and Mind Throw. Each
  uses the player's level when the result is prepared and its archived perk
  aspect `3639 / 100 * player_level + 60`.
- Adds the XML's level-4 **Master of Style / Relentless** perk choice and paired
  upgrade opportunities at levels 8, 11, 14 and 17. Both start at rank 1 and have
  five ranks. Six move definitions receive the archive's exact perk-lock removals,
  retaining their other conditions.
- Gives Jian the archived ChineseSwords family, extending ten Sai stance/attack
  locks and adding the four-hit super slash, localized moves-list entry and shop
  preview. Existing purchase/availability rules are unchanged.

## Combat perks

Master of Style arms a 300-simulation-frame bonus at Brutal style or higher. The
next unblocked weapon or unarmed hit adds 2/4/6/8/10 percentage points of normalized
health damage, according to rank. An unblocked incoming hit or an outgoing ranged
or magic hit cancels the bonus at the native post-critical phase. The archived
English description says Aggressive; the trigger XML also includes Brutal. The
mod follows the XML trigger and retains the original description.

Relentless tracks a combo of at least three hits, up to fifteen stacks. When that
combo ends, it arms a 300-frame bonus for the next outgoing hit only. Each stack
adds 1/2/3/4/5 percent damage according to rank. The XML has no block or attack-type
filter on consumption. Stack-counter details, including its retained value after
consumption, follow the trigger logic.

Both use the existing blue perk icons with expiration. The generic stack-count
badge is newly authored Eclipse presentation support: the archive requests stack
effects, but the recovered C# only implements the pulse effect. Its Unity rendering
still needs a game check.

Sources are `Assets/DExml/perks.xml`, `CharacterProgress.xml`,
`animations/moves.xml` and `localizations/eng.xml`. [Production notes](PRODUCTION.md)
record exact XML identities and hashes. No available metadata independently
identifies that archive as release 64. The existing `de128` namespace remains
stable so previously saved item references continue to resolve.

## Equipment combat families

`scripts/content/combat_equipment.lua` changes four existing core definitions:

| Equipment | Base family | DE family |
| --- | --- | --- |
| `WEAPON_CHNY22_SPEAR` | Spear | Naginata |
| `WEAPON_RAID_KARCER_SET` | Claws | HunterClaws |
| `WEAPON_BG_YARI` | Spear | MagariYari |
| `RANGED_BP_S3_WIND_MAKER` | Chakram | Kunai |

Those four target families have native moves in the base. Identity, prices, power,
art and saved ownership stay unchanged. New equipment/fighter copies use the
patched family; Apply & Restart with DE disabled restores the original family.
Existing explicit AI groups remain intact. Test attacks, ranged throws, shop
previews and NPC handling with each item before claiming full combat acceptance.

`content.chinese_swords` additionally changes `WEAPON_CHNY21_JIAN` from
HermitSwords to ChineseSwords and supplies its complete archived move graph.
The recovered binary has 38 samples/67 nodes. All attacks and sounds fit; the
archive's longer recovery interval follows native end-of-animation clamping,
which has been checked in Unity. Live execution/completion passes; contact and
player-facing timing still need acceptance.
The separate archived `WEAPON_CHINESE_SWORDS` item is not restored by this step.
Monk Katar and Musket
retain their canonical AI groups while archive intent is investigated.

## Archived shop availability

`scripts/content/shop.lua` exposes all 221 hidden existing core items that the DE archive
lists for normal purchase: the previous 25 battle-pass pieces plus 196 more
weapons, armor, helms, ranged items and magic. The complete static table in
`shop_data.lua` is generated by `Tools/GenerateDE128ShopAvailability.py` from
`Assets/vanillaXml/list.xml` and `Assets/DExml/list.xml`; the mod reads no gameplay
XML at runtime. `--check` verifies the authored table against the archive.

Each entry retains its core item identity. Ninety-nine price differences use
reversible `sf2.shop.set_price` patches, including the one item with both coin
and gem purchase prices. Twenty changed icons and three changed models use
`sf2.items.set_presentation` with typed packaged core assets. DE128 applies
the archive's starting level, upgrade level, exact stat
snapshot, native upgrade template and legacy paid marker to the catalog item, plus its level and
act-group shop gate: Guardian at level 15, Skanda at 20, Wind Maker at 25, Time Shifter at 45 and
Scriptwriter at 50. The five archive combat-family differences in this cohort
are supplied by `combat_equipment.lua` and `chinese_swords.lua`. The listing
does not grant equipment on activation. For 217 items, the mod also replaces
default enchantments with 258 of the archive's 262 perk/aspect entries; four
items' existing single-entry loadouts already match. It clears the canonical
legacy `PaidItem` marker on 129 items; this marker itself does not control price
or currency. `HELM_STARTER_PACK` also drops its canonical one-off upgrade row so
its archived shared template controls progression. Removing DE128 restores base
visibility, price, local upgrades, profile, paid marker and enchantments.

The shared upgrade table definitions and saved ownership remain unchanged.
The restored template selection can change
future upgrade costs. Existing saved inventory is not migrated or re-leveled.
Musket keeps its canonical `Rifle` tactic tag: the archive omits the tag, but
that alone does not establish a safe replacement for its reconstruction-era AI
compatibility. The generator rejects any other unimplemented difference in the
selected item's attributes or child nodes.
No hidden-to-visible archive entries remain excluded from the normal shop.
The isolated Unity 6 art validator loaded every changed sprite and model.
A separate full Unity 6 fixture verified all 221 live visibility policies and
both price fields, native coin and gem purchases, two save/reload cycles,
equipment use, the starter helm's shared-template upgrade and the real shop
row for a changed icon. Interactive rendering and combat balance still need
a game playtest. Test items below and at their level and act gates, then
disable/re-enable DE128 through Apply & Restart to verify listing reversibility.

## Ascension disabled

The entrypoint call and both Ascension module bodies are commented out. Even
explicitly requiring either file produces no definitions. No Ascension zone,
battle, quest, rule, opponent, translation or state schema is registered. Existing
saved prototype state is retained as inactive data. Apply & Restart to unload
content already present in a running session. Generic engine APIs remain available
to other mods; the prototype test launcher now reports SKIP.

Saved pending forge orders complete on the next normal delivery update. Their
original timestamps are not rewritten; disabling DE128 before completion restores
their wait. Completed enchantments remain completed. Service gates do not prove that every
associated button, offer or direct service entry point is hidden.

The five Titan items have no purchase listings and are hidden before acquisition. The reward
patch preserves the base game's coins and forge materials. It does not unlock
Titan, grant equipment at startup, replace the NPC loadout or migrate inventory.
The sword uses normal core weapon progression. Already owning any of the five
mod items suppresses another copy of that item; an existing copy keeps its
level and enchantments. The four new items use archived hidden-item stats and
subtypes. Their models ship as packaged `.modelz` geometry, including the
archive's harpoon edge flags. DE128 Lua never reads XML.

The generic reward callback uses a snapshot taken before reward experience is
awarded. The result is also safe to calculate for previews: it only returns
level/enchantment data and makes no profile changes. This is a defined API timing
contract, not a promise that every archived expression has the same timing when
a reward also levels up the player. Native settlement applies the prepared perk
through the ordinary inventory/save path.

The four Titan models are rebuilt by `Tools/ExtractDE128TitanRewardArt.py` from
archived geometry; `Tools/GenerateDE128TitanRewardText.py` generates their names
in 14 languages. Core icon and perk references still require the installed game.
An isolated native Unity 6 run settled all five grants at level 52, equipped
them and reloaded their saved enchantments at aspect `1952.28`. A second native
fight rendered the Titan body and helm rigs for 30 frames and returned to the
Underworld map. Interactive reward presentation and ranged/magic attack visuals
remain for a game playtest.

## Enable and try it

Keep the directory named `de128`, matching `id = "de128"` in `mod.toml`.
Open **Mods** on the title screen, select **DE128 - Definitive Edition**, then
choose **Apply & Restart** and enter Campaign.

Another enabled mod must not own the forge timer. In particular, the integrated
`example.phase2` showcase also claims it; disable that showcase when testing DE128.
An overlapping timer declaration fails explicitly instead of silently replacing
the other mod's policy. Service-only disables from several mods can coexist.

Use a profile approaching the level-4 perk choice to test acquisition. Choose
Master of Style or Relentless, verify the rank-1 value, then check its four upgrade
opportunities. Already learned perks are preserved; enabling this version does
not retroactively award either perk. Check timed icons, one-hit consumption and
save/reload. Double jump kick, elbow strike, two-foot jump kick, backflip kick and
both Suplex move variants should no longer require their old perk unlocks, while
their skeleton/equipment/screen requirements still apply.

With the forge unlocked and enough materials, start a new enchantment. It should
apply immediately and charge the usual materials. An already-pending order should
complete once without charging materials again, clear its saved delivery entry,
and remain complete after save/reload. A failed enchantment should keep its order.
Disable DE128 before a pending order completes and
apply the selection again to restore the base policy, unless another mod owns it.
Already completed enchantments remain completed.

For the acquisition check, use a profile that has unlocked final Eclipse Titan
fight 6 and owns none of the five reward items. Win the Eclipse fight, check all
five items at the player's level and with their matching Lifesteal, Shielding,
Damage Absorption, Precision and Frenzy enchantments, then save and reload.
Compare a normal-mode result and an already-owned result: neither should grant
unearned or duplicate copies. Equip the body, helm, harpoon and magic in a fight
to check their rendered animations.

## Source layout

`scripts/main.lua` loads the content modules in an explicit order. Service policy
lives in `scripts/content/services.lua`; forge timing lives in
`scripts/content/timers.lua`; Desolator's definition, translation and innate perks
live in `scripts/content/equipment.lua`. The four other Titan items and their
translations live in `titan_reward_equipment.lua` and `titan_reward_text*.lua`.
The grant calculation and final Titan patch live in `scripts/content/rewards.lua`.
New perk behavior, icons,
translations and rank tables live in `scripts/content/combat_perks.lua`; exact
branch replacements and move-lock removals live in `scripts/content/progression.lua`.
`ascension.lua` and `ascension_rules.lua` are commented archival prototypes.

All DE gameplay definitions and behavior must be authored through Lua. The TOML
manifest is loader metadata. Archived `Assets/DExml` is research evidence, never
a runtime dependency or a mod definition source loaded by this package. DE128
uses the normal sandbox, ownership and transaction rules and has no privileged
engine branch. Core prices and shared economic rules stay base-owned.

The manifest requests `policy.services`, `policy.timers`, `content.register`,
`content.patch`, `combat.modify_outgoing_hit` and `combat.effects`. The new perks
use transient state per fighter/perk/round. They do not register or mutate an owned
persistent state schema. Native learned-perk saves retain selected ranks.

## Verification and next work

Run `./Tools/TestDE128Foundation.ps1` from the repository root. It executes the
actual packaged Lua through MoonSharp and checks policy consumption, all five Titan
item registrations, Lua localization, actual reward calculations, callback errors and
instruction budgets, module and asset failures, missing capabilities, conflicts,
composition and rebuilding without DE.
It uses an isolated temporary fixture and requires the project's MoonSharp DLL
plus the .NET 10 SDK. Its core definitions come from canonical game data, with
controlled metadata for the declared art references. It does not decode art or run
a Unity playtest. `Tools/TestRewardOnlyEquipment.ps1` checks the generic runtime
adapter responsible for equipment with no purchase listing; its output states
the native services substituted by that fixture.
The foundation runner removes its new fixture after the run, including on failure.
Pass `-KeepFixture` only when inspecting the generated case files or running a
dependent check that needs the compiled test assembly.

`Tools/TestRewardGrantNative.ps1` checks the configured reward bridge, actual
canonical Eclipse Titan reward projection and recovered enchantment serialization.
It extracts the changed production methods and supplies controlled engine/catalog
services. `Tools/TestDE128TitanRewardNative.py` separately runs native Unity 6
grant, inventory settlement and save/reload checks on an isolated profile. Its
`--phases win` mode plays the real final Eclipse Titan fight through the native
victory screen and settles all five items; the
Underworld native fight runner checks the equipped Titan body and helm rigs.
Neither runs a full campaign or loads the owner's save.

The foundation fixture also compares both perks' rank tables and progression
slots to the archive, executes hit/cancellation/expiry traces, and verifies that
both disabled Ascension modules remain inert when required directly.
`Tools/TestDECombatPerksNative.ps1` checks precise native hit and status paths;
`Tools/TestPerkUpgradeNative.ps1` checks native first-rank selection;
`Tools/TestMovePerkLocks.ps1` checks targeted live lock removal and restoration.

[Production notes](PRODUCTION.md) record source evidence, known API gaps and the
next candidate. Development proceeds in substantial steps with status reports. This foundation
does not claim complete DE parity or a completed game playtest.
