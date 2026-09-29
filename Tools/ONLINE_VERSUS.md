# Online Versus (v1)

Online play extends Local Versus: two players, one on each machine, fight on the
same detached loadouts. Both games simulate the whole fight themselves. Only
per-tick controller input crosses the network (delay-based lockstep over UDP).

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
  (local device plus remote stream) and `ReplayInputSource` (a recording).
- **Lockstep.** Input sampled while simulating tick T is scheduled for tick
  T + delay. A tick simulates only when both players' inputs for it are known.
  Otherwise the fixed step stalls. The first `delay` ticks are neutral on both
  sides. A peer more than two ticks behind runs two ticks per fixed step to catch
  up (`OnlineInputSource.StepsWanted`).
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
  each fighter's position, facing, life, rounds won and animation state every 30
  ticks. Peers exchange hashes, and so does each replay against its recording. A
  mismatch ends the online match, saves a `-desync` replay and logs both states.
- **Netcode.** `Online/` holds plain C# with no Unity dependency:
  - `NetplayPeer`: UDP socket, handshake, keep-alive, RTT, timeouts.
  - `ReliableChannel`: ordered, acknowledged lobby messages.
  - `LockstepTimeline`: redundant input stream and hash checkpoints.
  - `VersusReplay`: compressed replay format.
- **Session.** `OnlineVersusSession` runs the lobby: the host owns the arena,
  the format and the input delay; each player picks their own weapon; the guest
  readies up. It also handles match start, rematch, return to lobby, forfeit,
  desync and lost connections. The handshake rejects different game versions
  (`Application.version`) or different enabled mods and versions.
- **Replays.** Every versus match is saved to
  `Application.persistentDataPath/Replays/` (the newest 40 are kept, plus
  `last.eclreplay`). "Watch last replay" in the versus lobby plays one back and
  reports whether every recorded state checkpoint matched. This is the quickest
  determinism test and needs only one machine.

## Rooms (no port forwarding)

The room server is `Server/EclipseRooms/`; see its README for deployment. Players
enter its address once in Online, then browse, create, or join rooms by code.

- **Rooms.** Up to 8 members. The host sets name, password, size, first-to,
  arena (or random), and winner-stays vs. rotation. Members pick a weapon and join
  the queue. The server pairs the first two in the queue; in winner-stays the
  champion goes back to the front and the room waits up to 30 s for them to
  continue.
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
  host migration and cleanup.

## Testing

1. Core netcode, from the repository root. It runs real loopback UDP with
   simulated latency and loss:

   ```sh
   ~/Unity/Hub/Editor/<version>/Editor/Data/DotNetSdk/dotnet run --project Tools/NetplayTests
   ```

2. Determinism on one machine: play a local versus match, then choose
   **Watch last replay**. The result screen must say "Replay verified". A
   divergence prints the first mismatched tick and both states to the log.
3. Online on one machine: run two players (a build plus the editor, or two
   builds). Host on one and join `127.0.0.1:7291` on the other.
   `NetplayPeer.SimulatedLatencyMs` / `SimulatedLossPercent` can emulate a bad
   connection.
4. Two machines: host on one and join its LAN address. Over the internet, forward
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

## Known limits (v1)

- **No NAT traversal or relay, and no matchmaking.** Players connect directly by
  address.
- **Delay-based, not rollback.** Input delay (default 3 frames, suggested from
  ping in the lobby) is felt on both sides. A connection hiccup freezes both games
  until inputs arrive; the session gives up after 20 s of silence.
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
