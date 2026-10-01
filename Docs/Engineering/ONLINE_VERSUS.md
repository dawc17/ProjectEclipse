# Online Versus

Online play extends Local Versus: two players, one on each machine, fight on the
same detached loadouts. Both games simulate the whole fight themselves. Only
per-tick controller input crosses the network, over UDP. The default netcode is
**rollback**: each game runs ahead on a guess of the opponent's input and fixes
wrong guesses by restoring the fight state and simulating again. The host can
switch the lobby to **delay-based** lockstep instead.

## How it works

- **Tick-sampled input for every versus fight.** `VersusTickDriver`
  (`Assets/Scripts/Eclipse/Multiplayer/`) owns input while a versus fight runs.
  `Fight.Draw` asks it for both fighters' inputs before each simulation tick and
  applies the changes through `Fight.ApplyVersusControl`. Device events from
  `GameController` are ignored for the whole versus fight. Local, online and replay
  matches all use this one path. The fight drops presses outside the stages that
  accept them, so on a stage change or after a local pause the driver releases
  and re-presses whatever is still held (as the old controller restart did). A
  match result is reported only after its final tick is recorded.
- **Input sources.** `LocalInputSource` (two devices), `OnlineInputSource`
  (local device plus remote stream), `ReplayInputSource` (a recording) and
  `RollbackSelfTest` (a recording, with forced rollbacks).
- **Input schedule.** Input sampled while simulating tick T is scheduled for tick
  T + delay. The first `delay` ticks are neutral on both sides.
- **Delay-based lockstep.** A tick simulates only when both players' inputs for
  it are known. Otherwise the fixed step stalls. A peer more than two ticks
  behind runs two ticks per fixed step to catch up.
- **Rollback** (see the next section) keeps a small delay (1 tick by default)
  and lets a tick run up to 8 ticks past the opponent's last confirmed input.
- **Determinism fixes** (see the audit notes below):
  - `ScreenFight` banner timers (VS, "Round N", "Fight!") drive round flow, so in
    versus they count simulation ticks, not frame time.
  - `VersusDeterminism` re-seeds `NekkiMath` from the match seed at every tick.
    It also replaces the rules' clock-based reseed and the gameplay
    `UnityEngine.Random` calls: crit shock/stun rolls in `Model`, animation
    tie-breaks in `SelectAnimation`, and `RandomTactic` delays.
  - Critical hit-stop uses a fixed scale instead of the local accessibility
    setting.

  Outside versus, every one of these falls back to the original behaviour.
- **Desync detection.** `VersusStateHash` hashes stage, round, the timers and
  each fighter's position, facing, life, rounds won and animation state every
  tick. Peers exchange the hash of their latest *final* tick: one simulated on
  confirmed inputs that will never be re-simulated. Replays keep a hash every 30
  ticks and check it on playback. A mismatch ends the online match, saves a
  `-desync` replay and logs both states.
- **Netcode.** `Online/` holds plain C# with no Unity dependency:
  - `NetplayPeer`: UDP socket, handshake, keep-alive, RTT, jitter, packet loss,
    timeouts.
  - `ReliableChannel`: ordered, acknowledged lobby messages.
  - `InputTimeline`: input history, prediction, misprediction detection, final
    ticks, hash checkpoints and time sync.
  - `RollbackRunner`: restores and re-simulates after a wrong guess, runs new
    ticks on predicted input, and paces this peer against the opponent.
  - `ObjectGraphSnapshotter`: saves and restores a managed object graph in place.
  - `VersusReplay`: compressed replay format.
- **Session.** `OnlineVersusSession` runs the lobby: the host owns the arena,
  the format, the netcode and the input delay; each player picks their own
  weapon; the guest readies up. It also handles match start, rematch, return to lobby, forfeit,
  desync and lost connections. The handshake rejects different game versions
  (`Application.version`) or different enabled mods and versions.
- **Replays.** Every versus match is saved to
  `Application.persistentDataPath/Replays/` (the newest 40 are kept, plus
  `last.eclreplay`). "Watch last replay" in the versus lobby plays one back and
  reports whether every recorded state checkpoint matched. This is the quickest
  determinism test and needs only one machine.

## Rollback

### Wire and timing

- **Prediction.** A missing opponent input is guessed as "still holding
  whatever they held last". Each speculative tick records the guess it used.
  When the real input arrives and differs, `InputTimeline.PendingRollback` names
  the first wrong tick.
- **Rollback.** Before every speculative tick the game saves the full fight
  state. `RollbackRunner.Resolve` restores the state from before the first wrong
  tick, then simulates every tick up to the current one again with the
  corrected inputs. This all happens inside one fixed step, so the player only
  sees the corrected result.
- **Prediction window.** At most 8 ticks may run on guesses. Beyond that the
  game waits for input, the same as lockstep.
- **Final ticks and desync checks.** A tick is final once it has been simulated
  on confirmed inputs and no earlier tick is waiting to be re-simulated. Only
  final ticks are hashed against the opponent, recorded into the replay, or
  allowed to finish the match.
- **Time sync.** Each packet carries the sender's current tick and its own
  estimate of how far ahead it is. Averaging both peers' views gives a smoothed
  frame advantage.
  - A peer more than one tick ahead skips a tick, at most once every 10 ticks,
    so the correction never reads as a stutter.
  - A peer three or more ticks behind runs two ticks per step.
  - Without this, the faster machine would run up against the prediction window
    and cause longer rollbacks for both players.
- **Input packets.** Every data packet resends all inputs the peer has not
  acknowledged yet, run-length coded: holding a direction costs two bytes, not
  one byte per tick. One lost packet never loses input.
- **Connection readout.** The peer measures jitter (RTT variation) and packet
  loss (gaps in a per-packet sequence number). The in-fight HUD shows ping, the
  input delay, how many ticks the game is running ahead on a guess, the deepest
  rollback so far, and loss.
- **Suggested delay.** `NetcodeModes.SuggestedDelay` picks the delay from ping
  and jitter.
  - Rollback: 1 tick up to about 130 ms of ping, growing to at most 4 at very
    high ping, so rollbacks stay short.
  - Delay-based: covers the whole one-way trip, plus one tick.
- **Protocol version.** It is now 2, because the input block, lobby and match
  start changed. Version 1 builds are refused at the handshake.

### What a snapshot holds (`Rollback/FightRollback.cs`)

`FightSnapshotPolicy` saves everything reachable from the `Fight` object, and
restores it in place: the same objects get their old field values back, so
event listeners and cached node references stay valid. Also saved:

- the tick driver's held-input state;
- the `PerksStage` static perk tables;
- the fight speed factor.

Deliberately left out:

- **Shared definitions:** moves, triggers, conditions, actions, fight setup and
  location data. They never change during a fight.
- **Presentation:** effects, blood, audio, mod visuals, the combo labels
  (`ComboModel`), the perk icons (`ActivePerkModel`) and the life bars.
- **Unity components.** The exceptions are the fight UI that holds gameplay
  state:
  - `ViewerFight`: the round timer;
  - `StylePanel`, `StyleBar`, `StyleBarStrip`: the style meter, including the
    strip's image fill, because style changes fire perks;
  - `PreFight`, `ScreenFight`: the banner timers.

`Vector2f`/`Vector3f` use a reflection-free codec, because fighter skeletons and
key frames hold thousands of them.

### What re-simulation does not repeat

A re-simulated tick sets `VersusTickDriver.IsResimulating`. While it is set, the
following already happened when the tick first ran and are skipped:

- one-shot sounds;
- effect spawns and effect animation;
- blood and the hit sprite;
- fighter hit particles;
- mod impact visuals;
- combo labels and perk icons.

The fight is left as the corrected run leaves it.

- Effects spawned by an undone tick are destroyed.
- Perk icons can still be removed during a re-simulation.
- If a wrong guess showed a hit that the correction removed, its combo label
  can stay on screen until it slides away.
- A hit that only the correction reveals plays no sound or effect, because it
  never ran for the first time.
- The end-of-fight combo statistics count the guessed run. Versus results do
  not use them.

These all affect presentation only.

### Speculation barriers

Some things cannot be undone by restoring managed state, because they change
scene objects or end the match:

- the round-end branch of `Fight.RenderRound`;
- `Fight.SetStage`;
- `ScreenFight.Start`/`Stop` (banners);
- `LocalVersusSession.Complete` (the match result).

Model form swaps (`Fight.DrainModelTransitions`) are a barrier too.

Each calls `VersusTickDriver.Barrier()`. On a speculative tick this skips the
action and marks the tick. The runner then restores the state from before the
tick and waits until the opponent's input for it is confirmed. So a round only
ever ends, and a match only ever finishes, on confirmed input.

A deciding hit usually lands on a speculative tick, so its sound and effects
already played before the round-end barrier discarded the tick. When the
confirmed input matches the guess, the tick runs again with presentation muted
(`TickFlags.BarrierReplay`) until it reaches the barrier. From there on,
everything plays normally, so the final hit is heard once.

`Fight.VersusCanSpeculate` also refuses prediction outside a running round
(intros, banners, round ends). Those moments play as lockstep.

### Scene objects

`RollbackObjects` keeps GameObjects in step with a rollback:

- **Projectiles and other models created on a speculative tick** are destroyed
  if that tick is rolled back.
- **Models removed on a speculative tick** are only hidden. They are destroyed
  once the tick is final, or shown again if it is rolled back.
- **Effects spawned on a speculative tick** are destroyed if it is rolled back.
- **Fighters shown or hidden on a speculative tick** (vanish moves) go back to
  their earlier visibility if it is rolled back.
- **Looped sounds started on a speculative tick** are stopped if it is rolled
  back.

## Rooms (no port forwarding)

The room server is `Server/EclipseRooms/`; see its README for deployment. Online
always connects to `rooms.projecteclipse.fyi:7300`, ignoring previously saved
custom addresses. Players browse, create, or join rooms by code.

- **Rooms.** Up to 8 members, including spectators. The host sets name, password,
  size, first-to, arena (or random), and match mode. New rooms default to
  simultaneous fights: the server pairs every two queued members, up to four
  independent fights. Winner-stays and rotation retain their single-fight queues.
  Each pairing snapshots the room settings, so changes apply to new fights only.
  Members pick a loadout and join the queue. In winner-stays the
  champion goes back to the front and the room waits up to 30 s for them to
  continue.
- **Spectators.** Each active fight has a Spectate button. Only members of the
  same room who are not fighting may watch; watching removes them from the queue.
  The left/host client publishes the agreed `MatchStart` and batches its existing
  replay's confirmed inputs and state checkpoints through the room connection.
  The server holds at most 30 minutes of input history per active fight, and sends
  it to each viewer in ordered chunks with backpressure. Viewers catch up from
  tick zero using `SpectatorInputSource`, then follow live inputs with a three-tick
  buffer. Playback checks the published hashes and reports divergence. It does
  not publish fighter input, save a replay, report results, or enter rollback.
  Ending or switching viewing unsubscribes; final history is released once the
  fight ends and all viewers have received it or left. Joining late may require
  substantial catch-up on slow devices. Portable fight snapshots are not used.
  Room protocol 4 requires the updated server and clients to be deployed together.
- **Connecting a pair.** `RoomClient` uses one UDP socket for the server,
  hole-punch probes and the fight, so the opponent punches into the NAT mapping
  the server observed.
  - The pairing lists the opponent's public address plus their LAN addresses.
  - A path counts as **direct** only after our probe is acknowledged, which
    proves both directions work.
  - After 2.5 s without that, the link relays through the server, and keeps
    probing for a while in case it can still upgrade.
  - `MatchLink` implements `INetTransport`, so `NetplayPeer` and the lockstep
    code are unchanged.
- **The fight** runs as `OnlineVersusSession.StartRoomFight`: both sides auto-ready,
  and the left player (host) starts once it has a ping sample, choosing the delay
  from it.
  - Each side reports its outcome: result, forfeit, desync (no result), or the
    opponent vanishing (a win).
  - Matching reports count; disagreeing ones count for nobody.
  - A pairing that fails to connect within 15 s reports no result and returns
    both players to the room.
- **Tests.** `Tools/NetplayTests` runs a real server with four clients: listing,
  mod-mismatch refusal, code joins, winner stays, the champion wait, a
  hole-punched fight, a forced-relay fight, disputed results, leaving mid-fight,
  host migration and cleanup. It also covers four simultaneous pairings, isolated
  completion/rematches, multiple and late spectators, live input/checkpoint
  delivery, switching/unsubscribing, spectator result rejection, room membership
  restrictions, bounded room-state encoding, and malformed stream chunks.

Native spectator validation on 2026-10-01 passed nine checks in Unity 6.6:
catch-up and live playback matched the original native fight's hashes, the final
buffer drained before completion, and the room rendered four usable spectate
buttons beside eight members. This used scripted input and a local UI fixture;
it does not verify internet latency or desktop/mobile spectator cross-play.
To repeat with a connected editor from the repository root:

```sh
unity command eval 'return ValidateLocalVersusNative.Prepare();' --caller plugin --skill unity-cli
unity command editor_play --caller plugin --skill unity-cli
unity command run_script --file Tools/NetplayTests/ValidateRoomSpectatingNative.cs --entry ValidateRoomSpectatingNative.Run --caller plugin --skill unity-cli
# Wait for Temp/RoomSpectatingNative/result.txt; room.png captures the layout.
unity command editor_stop --caller plugin --skill unity-cli
unity command eval 'return ValidateLocalVersusNative.Restore();' --caller plugin --skill unity-cli
```

## Testing

1. Core netcode, from the repository root. It runs real loopback UDP with
   simulated latency and loss:

   ```sh
   ~/Unity/Hub/Editor/<version>/Editor/Data/DotNetSdk/dotnet run --project Tools/NetplayTests
   ```

2. Determinism on one machine: play a local versus match, then choose
   **Watch last replay**. The result screen must say "Replay verified". A
   divergence prints the first mismatched tick and both states to the log.
3. Rollback on one machine: after a local versus match, choose **Test rollback
   on last replay**. `RollbackSelfTest` plays the recording with every
   mid-round tick treated as speculative.
   - Every other tick (once six have run), it restores the state from six ticks
     back, simulates them again, and compares the full saved state with the
     first run.
   - The result screen must say "Rollback test passed". A failure names the
     first field that differs, as `Type.field: first vs second`, and the log
     has up to five reports. That field is state the snapshot policy misses.
   - The summary also reports the snapshot size, save and restore times, and
     the cost of a 6-tick re-simulation. Run it on the slowest target device:
     rollback re-simulates up to 8 ticks inside one fixed step.
4. Online on one machine: run two players (a build plus the editor, or two
   builds). Host on one and join `127.0.0.1:7291` on the other.
   `NetplayPeer.SimulatedLatencyMs` / `SimulatedLossPercent` can emulate a bad
   connection.
5. Two machines: host on one and join its LAN address. Over the internet, forward
   UDP port 7291 on the host's router, or use a VPN such as Tailscale or ZeroTier.

### Phone and PC

- Touch controls feed player one's input in every versus mode. The on-screen
  stick and buttons are sampled per tick through
  `GameController.VersusTouchInput`. To check this path with a mouse on
  desktop, turn on Options → "Battle touch controls".
- Android builds need the internet permission; `ForceInternetPermission` is on,
  because raw sockets don't trigger Unity's automatic detection.
- Cross-play test: put both devices on the same Wi-Fi, host on the PC, and join
  from the phone. A PC (Mono x64) vs Android (IL2CPP ARM64) difference in float
  results shows up as a desync report.
- Offline check: copy a PC replay to the phone as `last.eclreplay` in the
  Replays folder, then watch it there. `adb logcat -s Unity` prints the phone's
  `[Versus] Replay saved: <path>` line, which gives the folder. For example:
  `adb push last.eclreplay /sdcard/Android/data/com.project.eclipse/files/Replays/`.
- A suspended mobile app sends nothing, so the opponent waits and the session
  ends after 20 s.

## Known limits

- **Direct connections have no NAT traversal.** Players connect by address.
  Rooms (above) add hole punching and a relay.
- **Rollback is not verified in the Unity editor yet.** The rollback loop,
  prediction, barriers, time sync and snapshot restore are covered by
  `Tools/NetplayTests`, using a toy fight and real UDP. What those tests cannot
  prove is that the snapshot policy covers every piece of recovered fight state.
  Run **Test rollback on last replay** on each platform before relying on it. If
  a match desyncs, the host can switch the lobby to delay-based netcode.
- **Rollback costs CPU.** Each speculative tick saves the full fight graph with
  reflection, and a correction re-simulates up to 8 ticks at once. On a slow
  phone, a larger input delay means fewer speculative ticks.
- **Transient perk flags now count simulation ticks in every versus mode.**
  They used to count render frames, which was not deterministic. A replay
  recorded before this change that depends on one of these flags could play
  back differently.
- **A long hiccup still freezes both games.** Past the 8-tick prediction window
  both games wait, and the session gives up after 20 s of silence.
- **Mono and IL2CPP builds can't play together.** Tested on 2026-09-29, a Mono
  build (the editor or a Mono Linux player) against the IL2CPP Android build
  desynced at tick 390, on the first moving frame of the start stance. Fighter
  positions differed from about the third decimal place. IL2CPP Linux x64
  against IL2CPP Android ARM64 stayed in sync, with IL2CPP built using
  `--compiler-flags=-ffp-contract=off` (no fused multiply-add). The build ID is
  `version/runtime`, so the handshake refuses mixed runtimes with a clear
  reason, and replays only play back in the same runtime. Ship desktop builds as
  IL2CPP; the editor only plays other Mono builds.
- **A desync reports itself.** Hashes are compared every tick. On the first
  mismatch each peer logs its exact state (floats include their bit patterns)
  and sends it to the other, so one log holds both sides.
- **Building IL2CPP for Linux on Arch-based systems** needs a
  `libncurses.so.6` alias for Unity's bundled clang:
  `sudo ln -s /usr/lib/libncursesw.so.6 /usr/lib/libncurses.so.6`.
- **The state hash covers the main fight state, not every field.** A divergence in
  unhashed state shows up later, once it affects hashed state.
- **Mod combat callbacks do not run in any versus mode** (as in Local Versus).
  Any future versus-enabled Lua must be tick-deterministic.

## Audit notes (evidence for the determinism fixes)

- `ScreenFight.Update` decremented `timer` by `Time.deltaTime`, and its `Stop()`
  triggers `Fight.OnStopPreFight`, which calls `NextRound`, `StartStance` and
  `PlayFight`, and `PlayFight` sets `round.processing`.
- `RulesInspector.ResetRandomRules` re-seeded `NekkiMath` from `DateTime.UtcNow`
  at fight construction and at every round.
- `Model` shock/stun rolls, `SelectAnimation` equal-priority tie-breaks and
  `RandomTactic.GetDelayByName` used the global `UnityEngine.Random`. Cosmetic
  effects (blood, particles, sounds, weapon trails) also consume that global.
- `Camera.FIEBIONJCCI` scaled critical hit-stop by the
  `Eclipse.CriticalPause` PlayerPref.
- The round timer (`ViewerFight`) and round end (`Fight.RenderRound`) were
  already tick-driven. `GameUtils.LDBMFAMEMPF` (two steps per tick) is never set.
