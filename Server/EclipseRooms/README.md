# Eclipse room server

A small program that makes online versus plug-and-play. Players browse, create and
join rooms of up to 8. The server pairs players from the room's queue (winner stays,
or everyone rotates). Each pair then connects **peer-to-peer**:

1. The server gives each player the other's address, as the server sees it, plus
   their LAN addresses.
2. Both games hole-punch. Most home routers let this through, and nobody has to
   forward a port.
3. If punching fails within 2.5 s (strict NAT, most mobile networks), the fight's
   packets are relayed through this server instead. That's about 3 KB/s per player.

The server never runs or inspects a fight. It only forwards the ~40-byte input
packets for relayed pairs. It keeps win/loss records per room while the room exists;
nothing is saved to disk.

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

- 8 players per room, 1000 rooms, 4000 connected players.
- Relay is capped at 48 KB/s per player. A fight needs about 3 KB/s.
- Only players with the same game build (version and IL2CPP/Mono) and the same
  enabled mods see and join each other's rooms.
- Clients that go silent for 20 s are dropped. A player who leaves mid-fight loses
  if the opponent reports a win.
- Results are self-reported by both players. If the two reports disagree, the fight
  counts for nobody. This is a friendly-rooms design, not anti-cheat.
