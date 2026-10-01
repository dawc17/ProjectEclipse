# Eclipse playtest room server

A small program that makes online versus plug-and-play. Players browse, create and
join rooms of up to 8. The server pairs players from the room's queue (simultaneous,
winner stays, or everyone rotates). Each pair then connects **peer-to-peer**:

1. The server gives each player the other's address, as the server sees it, plus
   their LAN addresses.
2. Both games hole-punch. Most home routers let this through, and nobody has to
   forward a port.
3. If punching fails within 2.5 s (strict NAT, most mobile networks), the fight's
   packets are relayed through this server instead. That's about 3 KB/s per player.

The server never simulates a fight. It forwards fight packets for relayed pairs
and buffers the host's confirmed inputs for spectators. It keeps win/loss records
per room while the room exists; nothing is saved to disk.

## Simultaneous fights and spectators

New rooms default to **Simultaneous**: every two queued players are paired, so an
eight-player room can run four fights at once. Each fight has its own connection,
settings snapshot, results, and rematches. The room host can still choose
**Winner stays** or **Everyone rotates**, which run one queued fight at a time.
Changing the mode or rules affects new fights; active fights keep their settings.
Switching to a single-fight mode waits for existing simultaneous fights to finish.

Room members can click **Spectate** beside any ongoing fight once its host has
published the setup. Watching removes the member from the queue. Viewers receive
confirmed inputs and state checkpoints through the room server, independently of
the fighters' direct/relay connection. Late viewers replay from the beginning at
high speed with catch-up audio muted, then follow the live stream at normal speed
with a one-second input buffer. Catch-up restarts only when the backlog exceeds
three seconds and stops with one second still buffered. If incoming data runs
out, playback refills the buffer before resuming. This absorbs ordinary delivery
batches instead of repeatedly accelerating and stalling between them.
Catch-up can take time on slow machines or late in a long match.

Spectators send no fighter input or results. They can open the menu with Escape
and use **Back to room** to stop watching, then queue or select another fight.
The stream ends after the final buffered inputs when the server resolves the
fight. If playback diverges or the room connection fails, the viewer sees a
message and can return to the room. A viewer's pause, slow connection, or departure
does not pause the fighters.

Input history is limited to the existing 30-minute match timeout. It is released
after the fight ends and its viewers have received the final data or left.
Spectator sending leaves reliable-channel capacity for room control messages.
This remains a friendly-room design: the server validates membership and the
publishing host, but cannot prove that uploaded inputs represent an honest fight.

These features use **room protocol 5**. Deploy the updated server and distribute
matching clients together; protocol 4 and older clients are refused with an update message.

## Choose or change the schedule

This branch is `playtest/multiplayer-beta`. It serves the restricted build on
`rooms.projecteclipse.fyi:7301`, separately from normal main's UDP 7300 service.
The server uses its own UTC clock. Clients do not use the device's date/time.
Both schedule values empty means **closed until scheduled**. One missing value,
a missing timezone, or an end at/before the start prevents server startup.

From the repository root:

```sh
cp Server/EclipseRooms/playtest.env.example Server/EclipseRooms/playtest.env
```

Edit `playtest.env` with your chosen dates. These are examples, not a preset:

```dotenv
ECLIPSE_PLAYTEST_START=2026-10-01T18:00:00+02:00
ECLIPSE_PLAYTEST_END=2026-10-01T20:00:00+02:00
```

Use `Z` for UTC or an explicit offset such as `+02:00`. Players can launch early
and wait on the title screen; **Join playtest** unlocks at the start. Access ends
at the end timestamp, including ongoing fights and spectator playback, with
**Playtest is over.** A lost server heartbeat revokes access within three seconds,
even for directly connected fighters. Reconnect after restoring the service.

## Deploy

Allow **UDP 7301** through the VPS firewall, then use Docker Compose v2:

```sh
sudo ufw allow 7301/udp
sudo docker compose -f Server/EclipseRooms/compose.yaml up -d --build
sudo docker compose -f Server/EclipseRooms/compose.yaml logs -f
```

After editing the dates, recreate the service so Docker reloads the environment:

```sh
sudo docker compose -f Server/EclipseRooms/compose.yaml up -d --force-recreate
```

A plain `docker restart` does not reload the env file. Recreating the server
clears its rooms and makes active players reconnect; make schedule changes before
players join when possible. Updating dates needs no new game build.

Without Compose:

```sh
sudo docker build -f Server/EclipseRooms/Dockerfile -t eclipse-playtest-rooms .
sudo docker run -d --name eclipse-playtest-rooms --restart unless-stopped \
  --env-file Server/EclipseRooms/playtest.env -p 7301:7301/udp eclipse-playtest-rooms
```

Without Docker, export the two schedule variables in the shell before running:

```sh
dotnet run --project Server/EclipseRooms -c Release -- --port 7301
```

`ECLIPSE_ROOMS_PORT` overrides the server port; distributing clients for a different
endpoint also requires changing `RoomSession.DefaultServer`. Local testing uses a
local endpoint in that constant (loopback for the same PC, its LAN IP for phones).
The regression runner uses an isolated loopback server without changing it.
The log prints connections, rooms, matches, results and a status every minute.

## Limits

- 8 members per room (including spectators), 1000 rooms, 4000 connected players.
- Relay is capped at 48 KB/s per player. A fight needs about 3 KB/s.
- Only players with the same game build (version and IL2CPP/Mono) and the same
  enabled mods see and join each other's rooms.
- Clients that go silent for 20 s are dropped. A player who leaves mid-fight loses
  if the opponent reports a win.
- Results are self-reported by both players. If the two reports disagree, the fight
  counts for nobody. This is a friendly-rooms design, not anti-cheat.
