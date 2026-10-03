# Eclipse room server

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
matching clients together; older clients are refused with an update message.

Balance-enabled match starts carry the rules hash (netplay protocol 4).
Spectators need the same JSON gameplay rules as the fight host; the server
relays the hash rather than installing profiles. See
[the balance guide](../../Docs/Modding/src/content/docs/guides/pvp-balance.md).

## Run it

It needs a machine with a public IP (any small VPS) with **UDP port 7300** open.
This is the only port involved; players forward nothing.

Docker, from the repository root:

```sh
docker build -f Server/EclipseRooms/Dockerfile -t eclipse-rooms .
docker run -d --name eclipse-rooms --restart unless-stopped -p 7300:7300/udp eclipse-rooms
docker logs -f eclipse-rooms
```

Without Docker, using any .NET 8 SDK (Unity's bundled one works too):

```sh
dotnet run --project Server/EclipseRooms -c Release -- --port 7300
```

`ECLIPSE_ROOMS_PORT` overrides the port. The log prints one line per connection,
room, match and result, plus a status line every minute.

## Testing on your own machine

Run the server on your PC, then in the game enter `127.0.0.1:7300` as the room
server. Another device on the same Wi-Fi or hotspot uses your PC's LAN address,
for example `10.171.108.41:7300`. Allow UDP 7300 through your firewall
(`sudo ufw allow 7300/udp`).

## Limits

- 8 members per room (including spectators), 1000 rooms, 4000 connected players.
- Relay is capped at 48 KB/s per player. A fight needs about 3 KB/s.
- Only players with the same game build (version and IL2CPP/Mono) and the same
  enabled mods see and join each other's rooms.
- Clients that go silent for 20 s are dropped. A player who leaves mid-fight loses
  if the opponent reports a win.
- Results are self-reported by both players. If the two reports disagree, the fight
  counts for nobody. This is a friendly-rooms design, not anti-cheat.
