# Multiplayer: loadouts, stages, rooms, replays and training

Plan for the next multiplayer milestone. It turns versus from "cycle one of four
weapons in a paper box" into a full mode: players build loadouts, pick any
arena from a picker, see each other's gear and connection in rooms, chat, browse
and scrub replays, and practise in a training room.

Status: **all phases (A–F) implemented, not yet playtested.** Automated checks
pass (NetplayTests, the room server build, a full compile of the game assembly).
None of the UI has been run in Unity yet; the owner builds and playtests. See
[Implementation notes](#implementation-notes) for what differs from the plan below.

## Implementation notes

- **Protocol:** `NetProtocol.Version` 3, `RoomProtocol.Version` 2. **Redeploy the
  room server** with this build; older games and servers refuse each other cleanly.
- **Roster:** `Assets/Resources/EclipseVersus/roster.json`, read by `VersusRoster.cs`.
  Loadouts are `VersusLoadout` (item ids) locally and `LoadoutCode` (roster indices)
  on the wire. Saved loadouts and presets are in `VersusLoadouts.cs`.
- **Menu files:** `LocalVersusMenu.Screens.cs` (page frame, home, backdrop, music),
  `.Armory.cs`, `.Lobby.cs` (local lobby, VS splash), `.Rooms.cs`, `.Replays.cs`,
  `.Training.cs`. Components: `VersusBackdrop`, `VersusFighterPreview`,
  `VersusStagePicker`, `VersusReplayPlayer`, `VersusTraining` (dummy, readouts, HUD),
  `VersusUiKit` (hover events, signal bars, scroll-to-selection).
- **Game-code hooks** (each gated so normal play is unchanged):
  - `ModelContainer.ShowParameters` (fighter previews).
  - A main-camera guard in `ModelContainer.SetModelOnListening`.
  - The AI exception in `Model.RenderAi` for a training dummy.
  - The training hit hook, `TrainingPlaceFighters` and `TrainingRefillTime` in `Fight`.
  - `ViewerFight.RefillTime`.
- **Differences from the plan:**
  - Home cards use ink-and-sun motifs rather than fighter art.
  - Training hotkeys are F9 (reset), F10 (swap sides), F11 (hitboxes), F12 (inputs),
    Page Down (speed) and Page Up (record/play), because F2–F8 are taken by debug tools.
  - Frame advantage is measured from the "uninterruptible" animation interval ending,
    which is an approximation.
  - "Random" in the local and training lobby resolves when picked, so players see the arena.
- **Not done yet:**
  - The in-fight ping widget redraw (the text readout is unchanged).
  - A chat overlay while watching or on the result screen, and an unread counter.
  - A replay-browser thumbnail for each saved round.
  - Replay files from before this change are refused by the version check anyway.

## Scope decisions (from the owner)

- **No premade characters.** Players choose equipment directly.
- **Allowed equipment** (`list-equipment.md`), all **without enchantments**:

  | Slot | Items (display name, item id) |
  |---|---|
  | Melee | Unarmed `Fists`, Knuckles `WEAPON_KNUCKLES`, Staff `WEAPON_STAFF`, Axes `WEAPON_AXES`, Katana `WEAPON_KATANA`, Machetes `WEAPON_MACHETE`, Oriental Sabers `WEAPON_CHINESE_SABERS`, Kusarigama `WEAPON_KUSARIGAMA`, Dadao `WEAPON_DADAO`, Two-Handed Mace `WEAPON_TWO_HANDED_MACE`, Battle Hammers `WEAPON_BATTLE_HAMMERS`, Blade Tonfas `WEAPON_SHARP_TONFA`, Harbinger Sai `WEAPON_SUPER_SAI`, Glaive `WEAPON_GLAIVE` |
  | Ranged | none `NoRanged`, Throwing Daggers `RANGED_THROWING_DAGGERS`, Chakram `RANGED_CHAKRAM`, Shuriken `RANGED_SHURIKENS` |
  | Magic | none `NoMagic`, Force Wave `MAGIC_WAVE`, Force Ray `MAGIC_DEATH_RAY`, Fire Pillar `MAGIC_FIRE_PILLAR`, Fire Ball `MAGIC_FIRE_BALL` |
  | Armor, Helm | **every** valid armor and helm, purely cosmetic (same stats for all) |

  "None" for ranged and magic is my addition (the game's own defaults); it keeps a
  pure-melee option. Drop it if unwanted.
- **Balancing is deferred.** See [Balancing notes](#balancing-notes-deferred).
- **Elo and friend codes are deferred.** See [Elo notes](#elo-notes-deferred).
- **In scope now:** loadout builder, visible loadouts, stage picker (the dojo-picker
  modal, for all fight locations), animated backdrop and music, room chat, ping and
  connection indicators, replay browser and player, training mode.

## Findings that shape the design

- **Stats.** `LocalVersusMatch.PrepareFighter` zeroes the six combat ratings on the
  warrior node, and `ModelParameters.NOBKKLBJFIL` then ignores item `WeaponDamage`,
  `BodyDefense`, etc. Armor and helm are therefore already cosmetic; damage comes from
  moves only. This is the baseline balance until the balancing pass.
- **Enchantments.** `ItemInfo.IgnoreInventoryEnchantments = true` already keeps
  enchantments out of a fight. The current `CopyItem` also clears `InnatePerks`, which
  none of the allowed items rely on today, but it would silently break items like
  `RANGED_SUPER_MINE` or `MAGIC_MIND_THROW` if they are ever allowed. `CopyItem` keeps
  innate perks from now on; enchantments stay stripped.
- **Broken models.** `ItemListCompatibility.HideUnavailableModelItems` swaps missing
  armor and helm models for the plain body, so a broken item does not fail, it just
  looks naked. The roster checks `PackagedArtCatalog.HasModel` itself.
- **Moves.** Weapon, ranged and magic move sets are chosen by item `SubType` locks in
  `moves.xml` against the default `Skeleton`; every allowed item has moves. No moves
  lock on armor or helm, so any combination works.
- **Locations.** 74 installed locations (`Resources/gamedata/locations/*/params.txt`).
  A location whose params are missing falls back to the dojo without an error. Raid
  arenas expect the raid layout, and a few event arenas have unusual widths. None has
  a localized name or a preview image.
- **Network size.** A full loadout as five item-name strings does not fit in
  `RoomState` (worst case already ~910 of 1000 bytes). Loadouts travel as compact
  roster indices instead (below).
- **Ping.** Peer-to-peer RTT exists only between two fighters during their match. The
  room server measures nothing today.
- **Replays.** Stored per match in `persistentDataPath/Replays`, newest 40 kept, no
  winner recorded, header only readable by inflating the file.
- **Training hooks.** Game AI can drive player 2 (`Model.RenderAi` blocks it in versus
  today). Immortality (`ModelParameters.set_IsImmortalityEnabled`), round-start
  snapshots, position setters and a hitbox/hurtbox line renderer
  (`EclipseFightDebugMenu.RefreshCollisionLines`) exist. Mod combat events are off in
  versus, so training gets its own C# hooks in `Fight.OnModelHit` and the model event
  listeners.

## 1. Foundations

### 1.1 Versus roster (`Eclipse/Multiplayer/VersusRoster.cs`)

- Data file `Assets/Resources/EclipseVersus/roster.json`: the allowed weapon, ranged
  and magic ids in display order, an armor and helm **exclusion** list (boss bodies such as
  `BODY_GATEKEEPER`, `PunchingBag`, `*_UNKNOWN`, `*_INVISIBLE`, `ABILITY_*`), and the
  arena list with display names and preview hints. Kept as data so mods can extend
  it later, when the modding API grows a versus section.
- At load, every entry is checked against the live item database: it must exist,
  have an `Image` and a `Model`, and `PackagedArtCatalog.HasModel` must be true. Armor
  and helm lists are all remaining `Armor`/`Helm` items plus the defaults `Body`/`Head`,
  sorted by level then name. Anything that fails is dropped with one log line.
- Per entry: id, localized name (`LocalizationManager.GetStringOrDefault`), slot,
  `SubType` (weapon class shown in the UI), icon sprite (lazy,
  `ResolutionImage.GetSprite("UI/Items/", item.FileName)`).
- **Roster fingerprint**: a hash of the ordered id lists, appended to the content
  fingerprint used by `NetIdentity` and rooms. Peers with different rosters never
  pair, so indices mean the same thing on both ends.

### 1.2 Loadout model (`VersusLoadout`)

- `struct VersusLoadout { Weapon, Armor, Helm, Ranged, Magic }` of item ids, with
  `Default`, `Random(seed)`, validation against the roster, and equality.
- **Wire form:** 10 bytes, five `U16` roster indices, opaque to the room server.
- **Saved form:** item ids, in `PlayerPrefs` per profile slot, plus up to 6
  **named presets** ("Katana rushdown", ...) the player can save and load.
- `LocalVersusSettings` gets `PlayerOneLoadout` / `PlayerTwoLoadout`, replacing the two
  weapon strings. `LocalVersusMatch.PrepareFighter` equips all five slots through
  `CopyItem` (innate perks kept, enchantments stripped).

### 1.3 Protocol changes

Both versions bump, so old builds and the old server refuse cleanly with the existing
"update the game" messages. **The room server must be redeployed** with this build.

- `NetProtocol.Version` 2 → 3: `LobbyState`, `MatchStart` and `GuestLobby` carry
  loadouts (10 bytes each) instead of weapon strings; buffer sizes grow to match.
- `RoomProtocol.Version` 1 → 2:
  - `RoomMember.Weapon` (up to 33 bytes) becomes `Loadout` (10 bytes), and gains
    `PingMs` (`U16`) and `Link` flags (`U8`: relayed, stale, in-fight). The worst-case
    `RoomState` shrinks from ~910 to ~760 bytes.
  - `SetMember(loadout, queued)`, `RoomPairing.PeerLoadout`.
  - **Server RTT:** the client stamps its keep-alive and the server echoes it
    (as `NetplayPeer` already does), measured every 500 ms and smoothed. The server
    stores each client's ping and reports it in `RoomState`. A member's ping changing
    by less than 10 ms does not trigger a broadcast; the state goes out at most every
    2 s for ping alone.
  - **Chat:** `Chat = 9` (client → server, text up to 140 characters) and
    `ChatLine = 25` (server → clients: sender id, sender name, text, kind). The
    server trims, drops control characters and rate-limits with a token bucket (burst 4,
    one message per 1.5 s), answering floods with an error line only to the sender.
    It also emits **system lines** (joined, left, won, champion streak, settings
    changed). No history is stored server-side; clients keep the last 60 lines.
  - Arena setting: any roster arena id, or `random`.
- Direct-connect lobby also gains the loadout exchange and the stage picker.

### 1.4 Replay format v2

- `VersusReplay.FormatVersion` 1 → 2. The header moves **in front of** the compressed
  inputs as a small uncompressed block, so the browser reads metadata without
  inflating: build, content, names, both loadouts (item ids, so a replay survives
  roster changes), arena, wins, round time, seed, online flag, timestamp, tick count,
  **winner, round score, round end ticks** (for timeline markers), and a `Kept` flag.
- v1 files still load (weapon only, default armor, helm, ranged and magic; no result).
- `VersusReplays`: `List()` (headers, newest first), `TryLoad(path)`, `Delete`,
  `SetKept` (kept replays are never pruned), `Rename` (sets a title in the header).
  Prune stays at the newest 40 unkept.

**Checks:** NetplayTests for loadout encode/decode, an 8-member `RoomState` with full
loadouts and ping inside 1000 bytes, chat relay and rate limit, server RTT on loopback,
replay v2 round-trip and v1 compatibility. Full game compile. Server builds.

## 2. Look and feel

The current menu is one paper card on flat ink. The new one keeps the paper-and-ink
language (PaperPanel, InkStroke, AGOpus) but becomes a full-screen scene.

### 2.1 Living backdrop (`VersusBackdrop`)

- Shows the **currently selected arena**, built from its own layers
  (`Textures/Locations/<loc>/<ClassName>` per its params, or its `StaticImage/BG`
  panorama), with slow parallax on mouse and a breathing zoom as on the title screen.
  Changing the stage cross-fades the backdrop to the new arena.
- Ink vignette and a warm wash keep text readable; drifting leaves or petals
  (`TitleLeaf`) chosen by arena mood; an occasional wind gust with sound.
- Falls back to the autumn gate if an arena's art is missing.

### 2.2 Music and sound

- Menu music through `EclipseUiAudio.StartTitleMusic`: a calm track in menus
  (`gamedata/music/menu`) and a tenser one on the VS splash (see 3.4), faded out before
  the fight starts its own music. Returns after a match.
- Existing UI sounds for focus, confirm, back and tabs, plus a gong on match start and
  a soft tick per chat line.

### 2.3 Layout kit

- Replace the single 760-px card with screen layouts: a header ribbon (InkStroke with
  the page title and breadcrumb), content panels, and a footer bar with key and
  gamepad hints (as on the title screen).
- New reusable pieces: **item tile** (icon on a paper disc, rarity-free, name on
  hover), **icon grid** with keyboard, gamepad and mouse navigation, **tabs**,
  **loadout strip** (five icons, weapon largest), **signal bars**, **toast**, and
  **modal** base shared with the pickers.
- Page transitions: ink-wipe between pages, staggered `UiReveal` on panels.

### 2.4 Multiplayer home

Three large panels instead of a button list: **LOCAL VERSUS**, **ONLINE**,
**TRAINING**, each with an ink illustration (fighter silhouettes in a pose matching
the mode) that lifts and brightens on focus. Below them: **REPLAYS**, **ROLLBACK TEST**
(development builds only), **RETURN TO TITLE**.

## 3. Loadouts and stages

### 3.1 Armory (loadout builder)

Full-screen page, used by local versus (one per player), online (your loadout),
rooms and training (you and the dummy).

- **Left:** a large **live fighter preview** wearing the loadout, idling, and playing
  a signature move of the weapon when the weapon changes (see 3.2).
- **Right:** slot tabs (MELEE, RANGED, MAGIC, ARMOR, HELM) over an icon grid. Melee
  tiles show the weapon class; armor and helm grids are large, so they get a search
  box and page scrolling. Hover or focus previews on the fighter instantly;
  confirm keeps it.
- **Bottom:** the loadout strip, **RANDOM** (whole loadout, or the focused slot), and
  **PRESETS** (save, load, rename, delete).
- Local versus: both players edit side by side in split view (P1 keyboard or pad
  1, P2 pad 2), each with a READY state; the match starts when both are ready.

### 3.2 Fighter preview renderer (`VersusFighterPreview`)

- Builds a `Model` from the same `ModelParameters` as `PrepareFighter`, on a hidden
  layer with its own orthographic camera, into a `RenderTexture` shown in a
  `RawImage`. Silhouette tinted to match the backdrop's model colour.
- One instance per visible fighter. Previews are cached by loadout; textures are
  released when the page closes.
- **Risk:** `ModelContainer` (used by the profile and shop scenes) only builds the
  saved player's model, so this is new code. If it proves unworkable, the
  fallback is a composed silhouette plus the loadout strip, and the plan is updated.

### 3.3 Stage picker (`VersusStagePicker`)

- The dojo picker modal (hero preview, gliding strip, arrows, ESC/ENTER hints, the
  same easing and sounds), generalized to take `(id, name, preview)` entries and a
  callback. The dojo picker itself is left untouched.
- Entries: every roster arena plus **RANDOM**. Wide previews instead of round
  medallions: the native battle preview art (`UI/battles/<loc>`) where it exists,
  otherwise a crop of the arena's panorama or layers.
- Initial arena list: all installed story and event arenas with the standard layout,
  excluding raid arenas, dojo variants that are not installed, and layouts known to
  misbehave (to be confirmed in play). Names from a hand-written table in the
  roster file.
- Opened from the local lobby, the direct-connect host lobby, room settings, and
  training.

### 3.4 Showing loadouts to others

- **Room roster:** each row shows the member's loadout strip (item icons, weapon
  first), a small live silhouette on hover, ping bars, and status. The local player is
  highlighted.
- **Match intro (VS splash):** before every fight, both fighters are shown full-size
  with their loadout strips and names, over the arena, with the `ProceduralVsStripe`
  divider, for about 2.5 s (skippable locally, fixed online so both sides line up).
- **Result screen:** both loadouts shown next to the score, with REMATCH and CHANGE
  LOADOUT.
- **In fight:** the HUD name plate gets the weapon icon.

## 4. Rooms

- **Room page redesign:**
  - Left: roster with loadout strips, ping bars, status, champion crown and streak.
  - Centre: "now fighting" card with both fighters' previews, and the queue as small
    cards.
  - Right: **chat**.
  - Bottom: YOUR LOADOUT (opens the armory), QUEUE toggle, ROOM SETTINGS (host), LEAVE.
- **Chat panel:** scrolling log of player lines (name in the player's colour) and
  system lines (grey italic), an input field (Enter focuses, Enter sends, Esc
  leaves), unread counter when hidden. Chat also appears as a small overlay while
  watching someone else's fight in the room, and on the result screen.
- **Ping and connection indicators:** ping bars and milliseconds to the room server
  per member (green under 80 ms, amber under 150 ms, red above, grey when stale), a
  relay icon when a fight had to be relayed, and in-fight the existing
  PING/DELAY/ROLLBACK readout redrawn as a small corner widget instead of plain text.
- **Browser:** room cards show player count, host, first-to, rotation, lock, and the
  arena preview.

## 5. Replays

### 5.1 Replay browser

- Grid of replay cards: arena thumbnail, both names with loadout strips, winner and
  score, online or local, date, length. Kept replays are pinned first.
- Actions: WATCH, KEEP/UNKEEP, RENAME, DELETE (with confirmation), OPEN FOLDER.
  Filter: all, online, local, kept. Replays from other builds are listed but marked
  "different version" and cannot be watched.

### 5.2 Replay player

- Transport bar overlay during playback: play/pause, speed (0.25×, 0.5×, 1×, 2×,
  4×), step one frame while paused, a timeline with round markers and click-to-seek,
  and a HUD toggle.
- Speed uses `Time.timeScale` (the fight runs in `FixedUpdate`). Seeking forward runs
  ticks without presentation until the target. Seeking backward restarts the replay
  and fast-forwards. Replays are deterministic, so seeking is exact.
- Both players' input displays (see 6.4) can be shown.

## 6. Training

Entered from the multiplayer home. Player 1 against a dummy on any arena, with
any loadout on both sides.

### 6.1 Dummy behaviours

- **Stand, crouch, jump, walk forward or back:** scripted input bytes.
- **Block:** always, never, after the first hit, or randomly; standing or low.
- **Record and play back:** take control of the dummy, record up to 10 s, then the
  dummy loops or plays it back on a key press.
- **CPU:** the game's own AI with a tactic chosen as difficulty (Beginner, Standard,
  Aggressive, Sensei). Needs the versus block in `Model.RenderAi` lifted for a
  training dummy, and `InitializeLocalVersusParameters` to leave that side AI
  controlled.

### 6.2 Rules

- Health: normal, **infinite** (immortality on), or refill after each combo ends.
- Round timer off; round never ends.
- **Reset positions** (key or pad button): centre, left corner, right corner; swap
  sides.
- Ranged ammo and magic charge: normal or always full.

### 6.3 Readouts

- Last hit damage, combo damage, combo hits, best combo, blocked or clean, critical.
- Current move name for both fighters, and **frame advantage** after a hit or block
  (ticks until each fighter can act again).
- A damage log panel with the last 10 hits.

### 6.4 Visual aids

- **Input display:** scrolling history of P1 (and dummy) inputs using the game's own
  button and stick icons, with the hold length in frames.
- **Hitboxes and hurtboxes:** the line renderer from `EclipseFightDebugMenu`, moved
  into a reusable component, toggled from the training menu.
- Slow motion (0.5×, 0.25×) and freeze-frame step, using the replay player's speed
  control.

### 6.5 Training menu

A side panel on pause (not a full page) with every setting live, a loadout button for
you and the dummy (restarts instantly with the new gear), and the stage picker.

## 7. Order of work and checks

| Phase | Content | Checks before moving on |
|---|---|---|
| A | Roster, loadouts, protocol v3/v2, replay v2, server chat and ping | NetplayTests extended as in 1.4; full compile; server builds |
| B | UI kit, backdrop, music, multiplayer home, page transitions | Compile; owner run: menus navigable by mouse, keyboard, pad |
| C | Armory, fighter preview, stage picker, local versus flow, VS splash | Owner: local match with custom loadouts; ROLLBACK TEST still passes |
| D | Room redesign, chat, ping indicators, loadout display, direct-connect loadouts | Online match with the owner and a tester; server redeployed |
| E | Replay browser and player | Owner: browse, keep, delete, seek, speeds; v1 replay loads |
| F | Training mode | Owner: each dummy mode, readouts, hitboxes, input display |

Every phase: `ROLLBACK TEST` still passes (loadouts change the fight state), and a
patch build for testers via `Tools/MakeBuildPatch.ps1`. If a phase changes the public
modding API (for example a mod-extensible roster), the wiki and editor tooling are
updated in the same change.

## Balancing notes (deferred)

- Today every fighter's six combat ratings are zero, so item stats are ignored and
  damage comes from move data alone. Heavy weapons with high base damage or long reach
  likely dominate; fists and knuckles likely lag.
- Proposed later: a versus balance table in the roster file per weapon, ranged and
  magic item: damage multiplier, defense, ranged ammo and magic charge rate. Tuned
  from **match statistics**: replays already hold full inputs, so a tool can replay
  every saved match and tally damage per weapon, hit rates and win rates without extra
  logging.
- Armor and helm stay cosmetic by rule; if they ever gain stats, it must be one
  shared value.

## Elo notes (deferred)

- **Compute and storage are cheap:** one rating per player, updated per room fight
  (the server already knows both players and the agreed result), stored in a small
  SQLite file or JSON on the VPS. Thousands of players cost kilobytes.
- **The real cost is identity and trust:**
  - Players have no accounts; names are free text. Ratings need a stable identity: a
    key pair generated on first launch, public key sent at Hello, server challenge
    signed by the client. That is about a day of work.
  - Results are self-reported by both players; disagreeing reports already count for
    nobody. This is fine for friendly ranking, but not cheat-proof. A cheat-resistant
    ladder would need the server (or a third client) to replay the inputs, which the
    deterministic replays make possible but is a larger project.
  - Moderation: renames, resets, decay, seasons.
- **Suggested first step when picked up:** identity keys plus a per-room-server
  leaderboard, no matchmaking changes.

## Friend codes (deferred)

Build on the Elo identity: the public key hash as a short code, a friends list stored
locally, and the server showing which friends are online and in which room.
