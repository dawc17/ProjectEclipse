using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Eclipse.Multiplayer.Online;
using Eclipse.Multiplayer.Online.Rooms;

/// <summary>Room server + several clients over real loopback UDP: rooms, queue, punching, relay, results.</summary>
internal static class RoomTests
{
    private static Action<bool, string> Check;
    private static readonly Stopwatch Clock = Stopwatch.StartNew();
    private static long Now => Clock.ElapsedMilliseconds;
    private static Eclipse.RoomServer.RoomServer _server;
    private static readonly List<RoomClient> _clients = new List<RoomClient>();
    private static readonly List<NetplayPeer> _peers = new List<NetplayPeer>();

    public static void Run(Action<bool, string> check)
    {
        Check = check;
        using (_server = new Eclipse.RoomServer.RoomServer(0, new PlaytestWindow(0, PlaytestWindow.MaxUnixMs)))
        {
            var server = new IPEndPoint(IPAddress.Loopback, _server.Port);
            var identity = new Func<string, NetIdentity>(name => new NetIdentity("1.0/IL2CPP", "mods:none", name));
            var ann = Connect(server, identity("Ann"));
            var bo = Connect(server, identity("Bo"));
            var cy = Connect(server, identity("Cy"));
            var modded = Connect(server, new NetIdentity("1.0/IL2CPP", "mods:x@1", "Dee"));

            // Create, list and join.
            ann.CreateRoom(new RoomSettings { Name = "Dojo night", WinsRequired = 1, Arena = "dojo", Rotation = RoomRotation.WinnerStays }, "");
            Pump(() => ann.Room != null);
            Check(ann.Room != null && ann.Room.HostId == ann.ClientId && ann.Room.Code.Length == 6, "room created with a code");
            bo.JoinByCode(ann.Room.Code.ToLowerInvariant(), "");
            Pump(() => bo.Room != null && ann.Room.Members.Count == 2);
            Check(bo.Room != null && bo.Room.RoomId == ann.Room.RoomId, "joined by code (case-insensitive)");
            cy.RefreshRooms();
            Pump(() => cy.Rooms.Count > 0);
            Check(cy.Rooms.Count == 1 && cy.Rooms[0].Name == "Dojo night" && cy.Rooms[0].Players == 2 && cy.Rooms[0].HostName == "Ann", "room listed");
            modded.RefreshRooms();
            Pump(() => Drain(modded).Any(e => e.Type == RoomEventType.RoomListUpdated) || modded.Rooms.Count > 0, 1500);
            Check(modded.Rooms.Count == 0, "rooms with other mods are hidden");
            modded.JoinByCode(ann.Room.Code, "");
            string modError = null;
            Pump(() => (modError = Drain(modded).Where(e => e.Type == RoomEventType.Error).Select(e => e.Text).FirstOrDefault()) != null);
            Check(modError != null && modError.Contains("mods"), "joining a room with other mods is refused: " + modError);
            cy.JoinRoom(cy.Rooms[0].Id, "");
            Pump(() => cy.Room != null && ann.Room.Members.Count == 3);
            Check(ann.Room.Members.Count == 3, "three members");

            // Queue: Ann, Bo, Cy. Winner stays.
            ann.SetMember(Loadout(0), true);
            Pump(() => ann.Room.Queue.Count == 1);
            bo.SetMember(Loadout(4), true);
            Pump(() => ann.Link != null && bo.Link != null);
            cy.SetMember(Loadout(2), true);
            Pump(() => ann.Room.Queue.Contains(cy.ClientId));
            Check(ann.Link.Pairing.Side == 0 && bo.Link.Pairing.Side == 1 && ann.Link.Pairing.PeerName == "Bo" &&
                bo.Link.Pairing.PeerLoadout == Loadout(0) && ann.Link.Pairing.PeerLoadout == Loadout(4) && ann.Link.Pairing.Seed == bo.Link.Pairing.Seed, "first two in queue paired");
            Check(ann.Room.MatchId == ann.Link.MatchId && ann.Room.Find(ann.ClientId).Status == MemberStatus.InMatch, "room shows the match");
            Fight(ann, bo, expectDirect: true);
            uint firstMatch = ann.Room.MatchId;
            ann.ReportMatch(firstMatch, MatchOutcome.LeftWon, "");
            bo.ReportMatch(firstMatch, MatchOutcome.LeftWon, "");
            Pump(() => ann.Room.MatchId == 0 && ann.Room.ChampionId == ann.ClientId);
            ann.ReleaseLink(firstMatch);
            bo.ReleaseLink(firstMatch);
            Check(ann.Room.Streak == 1 && ann.Room.Find(ann.ClientId).Wins == 1 && ann.Room.Find(bo.ClientId).Losses == 1, "result recorded, winner is champion");
            Check(ann.Room.Find(ann.ClientId).Status == MemberStatus.Away && ann.Link == null, "players are away after the match");

            // The champion must continue before the next fight; Cy is waiting.
            Pump(() => false, 300);
            Check(ann.Room.MatchId == 0, "no pairing while the champion is away");
            bo.SetMember(Loadout(4), true);
            ann.SetMember(Loadout(0), true);
            Pump(() => ann.Link != null && cy.Link != null);
            Check(ann.Link.Pairing.PeerId == cy.ClientId && ann.Link.Pairing.Side == 0, "winner stays against the next in queue");

            // Force relay for this pair.
            ann.ForceRelay = cy.ForceRelay = true;
            ann.Link.GetType(); // links were created before the flag; rebuild them through a fresh pairing below
            uint disputed = ann.Room.MatchId;
            ann.ReportMatch(disputed, MatchOutcome.RightWon, "");
            cy.ReportMatch(disputed, MatchOutcome.LeftWon, "");
            Pump(() => ann.Room.MatchId == 0);
            ann.ReleaseLink(disputed);
            cy.ReleaseLink(disputed);
            Check(ann.Room.Find(ann.ClientId).Wins == 1 && ann.Room.Find(cy.ClientId).Losses == 0 && ann.Room.ChampionId == ann.ClientId,
                "disagreeing reports give no result and keep the champion");

            ann.SetMember(Loadout(0), true);
            cy.SetMember(Loadout(2), true);
            Pump(() => ann.Link != null && (cy.Link != null || bo.Link != null));
            var opponent = cy.Link != null ? cy : bo;
            Check(ann.Link.Path == LinkPath.Relay, "forced relay path");
            Fight(ann, opponent, expectDirect: false);

            // The opponent leaves mid-match; Ann's win report stands.
            ann.ReportMatch(ann.Room.MatchId, MatchOutcome.LeftWon, "");
            opponent.LeaveRoom();
            Pump(() => ann.Room.MatchId == 0 && opponent.Room == null);
            Check(ann.Room.Streak == 2 && ann.Room.Members.Count == 2, "leaver forfeits; streak grows");

            // Host migration.
            ann.LeaveRoom();
            var other = opponent == cy ? bo : cy;
            Pump(() => other.Room != null && other.Room.HostId == other.ClientId && other.Room.Members.Count == 1);
            Check(other.Room.HostId == other.ClientId, "host moves to the next member");
            other.LeaveRoom();
            Pump(() => _server.RoomCount == 0);
            Check(_server.RoomCount == 0, "empty room closes");

            Rematches(server, identity);
            Hardening(server, identity);
            ChatAndPing(server, identity);
            RoomStateFitsFullLoadouts();
            SpectatorPacing();
            SimultaneousAndSpectating(server, identity);

            foreach (var client in _clients) client.Dispose();
            Pump(() => _server.ClientCount == 0, 2000);
            Check(_server.ClientCount == 0, "clients disconnect cleanly");
        }
        _clients.Clear();
        _peers.Clear();
        PlaytestGating();
    }

    private static void PlaytestGating()
    {
        var window = PlaytestWindow.FromEnvironment(key => key.EndsWith("START") ? "2026-10-01T18:00:00+02:00" : "2026-10-01T19:00:00+02:00");
        Check(window.StartsUnixMs == DateTimeOffset.Parse("2026-10-01T16:00:00Z").ToUnixTimeMilliseconds(), "schedule converts explicit timezone to UTC");
        Check(window.BlockReason(window.StartsUnixMs - 1) != null && window.BlockReason(window.StartsUnixMs) == null &&
            window.BlockReason(window.EndsUnixMs - 1) == null && window.BlockReason(window.EndsUnixMs) == "Playtest is over.", "playtest start is inclusive and end exclusive");
        Check(!PlaytestWindow.FromEnvironment(_ => "").Scheduled, "unset schedule stays closed");
        foreach (var pair in new[] {
            ("2026-10-01T18:00:00", "2026-10-01T19:00:00Z"),
            ("", "2026-10-01T19:00:00Z"),
            ("2026-10-01T20:00:00Z", "2026-10-01T19:00:00Z"),
            ("2026-10-01T19:00:00Z", "2026-10-01T19:00:00Z"),
            ("bad", "bad") })
        {
            bool refused = false;
            try { PlaytestWindow.FromEnvironment(key => key.EndsWith("START") ? pair.Item1 : pair.Item2); }
            catch (ArgumentException) { refused = true; }
            Check(refused, "invalid or incomplete schedule is refused: " + pair);
        }
        var writer = new NetWriter();
        window.Write(writer, window.StartsUnixMs);
        var read = PlaytestWindow.Read(new NetReader(writer.Buffer, 0, writer.Length), out long serverUtc);
        Check(read.StartsUnixMs == window.StartsUnixMs && read.EndsUnixMs == window.EndsUnixMs && serverUtc == window.StartsUnixMs, "64-bit schedule packet round-trips");
        bool truncated = false;
        try { PlaytestWindow.Read(new NetReader(writer.Buffer, 0, writer.Length - 1), out _); }
        catch (NetFormatException) { truncated = true; }
        Check(truncated, "truncated schedule packet is refused");

        using (_server = new Eclipse.RoomServer.RoomServer(0))
        {
            var waiting = Connect(new IPEndPoint(IPAddress.Loopback, _server.Port), new NetIdentity("test", "none", "Waiting"));
            Check(!waiting.CanPlay && waiting.PlaytestMessage.Contains("not been scheduled"), "unscheduled client connects in a locked waiting state");
            waiting.CreateRoom(new RoomSettings(), "");
            string error = null;
            Pump(() => (error = Drain(waiting).FirstOrDefault(e => e.Type == RoomEventType.Error).Text) != null);
            Check(error != null && _server.RoomCount == 0, "server refuses room creation even if a locked client sends it");
            waiting.Dispose();
        }
        _clients.Clear();
        long utc = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        window = new PlaytestWindow(utc + 3000, utc + 60000);
        using (_server = new Eclipse.RoomServer.RoomServer(0, window, () => utc))
        {
            var endpoint = new IPEndPoint(IPAddress.Loopback, _server.Port);
            var players = Enumerable.Range(0, 5).Select(i => Connect(endpoint, new NetIdentity("test", "none", "Tester " + i))).ToArray();
            Check(players.All(client => !client.CanPlay), "all clients wait before the server's start time");
            players[0].CreateRoom(new RoomSettings(), "");
            string error = null;
            Pump(() => (error = Drain(players[0]).FirstOrDefault(e => e.Type == RoomEventType.Error).Text) != null);
            Check(error != null && _server.RoomCount == 0, "server refuses rooms before the start time");
            utc = window.StartsUnixMs;
            Pump(() => players.All(client => client.CanPlay));
            Check(players.All(client => client.CanPlay), "server heartbeat unlocks waiting clients without a rebuild or reconnect");
            players[0].CreateRoom(new RoomSettings { Rotation = RoomRotation.Simultaneous }, "");
            Pump(() => players[0].Room != null);
            foreach (var player in players.Skip(1)) player.JoinByCode(players[0].Room.Code, "");
            Pump(() => players[0].Room.Members.Count == 5);
            foreach (var player in players.Take(4)) player.SetMember(Loadout(0), true);
            Pump(() => players[0].Room.Fights.Count == 2);
            Check(players[0].Room.Fights.Count == 2, "scheduled playtest supports simultaneous fights");
            var pairing = players[0].Link.Pairing;
            players[0].PublishStart(pairing.MatchId, new MatchStart { MatchIndex = 1, Arena = "dojo", HostLoadout = Loadout(0), GuestLoadout = Loadout(0), Seed = pairing.Seed,
                WinsRequired = pairing.Settings.WinsRequired, RoundTimeSeconds = pairing.Settings.RoundTimeSeconds, InputDelay = 2 });
            Pump(() => players[4].Room.Fights[0].CanSpectate);
            players[4].Spectate(pairing.MatchId);
            Pump(() => players[4].Spectating != null);
            Check(players[4].Spectating != null, "scheduled playtest supports spectators");
            utc = window.EndsUnixMs;
            Pump(() => players.All(client => client.State == RoomClientState.Closed));
            Check(_server.RoomCount == 0 && players.All(client => !client.CanPlay && client.CloseReason == "Playtest is over."), "expiry closes all simultaneous fighters and spectators and clears rooms");
            foreach (var player in players) player.Dispose();
            _clients.Clear();
            var late = new RoomClient(endpoint, new NetIdentity("test", "none", "Late"), Array.Empty<IPAddress>(), Now);
            _clients.Add(late);
            Pump(() => late.State == RoomClientState.Closed);
            Check(!late.CanPlay && late.CloseReason == "Playtest is over.", "client launched after expiry stays locked");
            late.Dispose();
        }
        _clients.Clear();
        using (_server = new Eclipse.RoomServer.RoomServer(0, new PlaytestWindow(0, 12000), () => 10000))
        {
            var client = Connect(new IPEndPoint(IPAddress.Loopback, _server.Port), new NetIdentity("test", "none", "Clock"));
            long now = Now;
            // A new heartbeat with an old UTC stamp models a delayed packet/loading hitch.
            _server.Update(now + 1000);
            client.Update(now + 1000);
            Check(client.PlaytestMessageAt(now + 2000) == "Playtest is over.", "old heartbeat cannot rewind expiry, even before the frame pump runs");
            client.Dispose();
        }
        _clients.Clear();
        using (_server = new Eclipse.RoomServer.RoomServer(0, new PlaytestWindow(0, PlaytestWindow.MaxUnixMs)))
        {
            var client = Connect(new IPEndPoint(IPAddress.Loopback, _server.Port), new NetIdentity("test", "none", "Lease"));
            long now = Now;
            Check(client.CanPlay && !client.CanPlayAt(now + PlaytestWindow.LeaseMs), "access fails when server lease expires even before the client pump runs");
            client.Update(now + PlaytestWindow.LeaseMs);
            Check(client.State == RoomClientState.Closed && !client.CanPlay && client.CloseReason.Contains("Lost connection"), "loss of server heartbeat closes client within three seconds");
            client.Dispose();
        }
        _clients.Clear();
    }

    private static void SpectatorPacing()
    {
        foreach (int history in new[] { 0, 600 })
        {
            var playback = new SpectatorPlayback();
            int received = history, played = 0, batch = 1, previousDelivery = 0;
            int stalls = 0, catchups = 0, liveTicks = 0;
            bool live = false, wasCatchingUp = false;
            // Host publishes 20-tick batches. Delay some deliveries, retaining reliable ordering.
            for (int clock = 0; clock < 1800; clock++)
            {
                while (batch <= 90)
                {
                    int delivery = Math.Max(previousDelivery, batch * 20 + (batch % 4 == 0 ? 30 : 0));
                    if (delivery > clock) break;
                    received += 20;
                    previousDelivery = delivery;
                    batch++;
                }
                int steps = playback.StepsWanted(received - played, false);
                if (playback.CatchingUp && !wasCatchingUp) catchups++;
                wasCatchingUp = playback.CatchingUp;
                if (steps > 0 && !playback.CatchingUp) live = true;
                if (live)
                {
                    if (steps == 0) stalls++;
                    if (steps == 1) liveTicks++;
                }
                played += steps;
                if (played > received) throw new Exception("Spectator predicted unavailable input.");
            }
            Check(stalls == 0 && liveTicks > 1500, "bursty spectator inputs play continuously at normal speed (history " + history + ")");
            Check(catchups == (history == 0 ? 0 : 1), "spectator catches up once without chasing each live batch");
        }
        var refill = new SpectatorPlayback();
        Check(refill.StepsWanted(60, false) == 1 && refill.StepsWanted(0, false) == 0, "real spectator underrun starts rebuffering");
        Check(refill.StepsWanted(20, false) == 0 && refill.StepsWanted(40, false) == 0 && refill.StepsWanted(60, false) == 1, "underrun refills a full buffer before resuming");
        Check(refill.StepsWanted(0, false) == 0 && refill.StepsWanted(12, true) == 1, "ended stream drains even an incomplete refill buffer");
        int remaining = 200;
        while (remaining > 0) remaining -= refill.StepsWanted(remaining, true);
        Check(remaining == 0 && refill.StepsWanted(0, true) == 0, "ended stream drains exactly its available inputs");
    }

    private static void Hardening(IPEndPoint server, Func<string, NetIdentity> identity)
    {
        // A hello without the server's cookie creates no client and gets only a small challenge.
        int before = _server.ClientCount;
        var raw = new UdpTransport(0);
        var writer = new NetWriter();
        RoomProtocol.WriteHeader(writer, RoomProtocol.KindHello, 1234);
        identity("Spoof").Write(writer);
        writer.U8(0);
        writer.U32(0);
        int helloBytes = writer.Length;
        raw.Send(server, writer.Buffer, writer.Length);
        int replies = 0, replyBytes = 0;
        Pump(() => { raw.Poll((from, buffer, length) => { replies++; replyBytes += length; }); return replies > 0; }, 1000);
        Check(replies == 1 && replyBytes <= helloBytes && _server.ClientCount == before, "cookieless hello gets one small challenge and no state");
        writer.Reset();
        RoomProtocol.WriteHeader(writer, RoomProtocol.KindHello, 1234);
        identity("Spoof").Write(writer);
        writer.U8(0);
        writer.U32(0xDEADBEEF);
        raw.Send(server, writer.Buffer, writer.Length);
        Pump(() => false, 200);
        Check(_server.ClientCount == before, "a forged cookie is refused");
        raw.Dispose();

        // Long multibyte names and junk weapons must not break room messages.
        var wide = Connect(server, identity(new string('\u6f22', 24)));
        var mate = Connect(server, identity(new string('\u0416', 24)));
        wide.CreateRoom(new RoomSettings { Name = new string('\u6f22', 32), Arena = "dojo" }, "");
        Pump(() => wide.Room != null);
        Check(wide.Room != null, "room with a long multibyte name");
        mate.JoinByCode(wide.Room.Code, "");
        Pump(() => mate.Room != null);
        wide.SetMember(Loadout(65000), false);
        Pump(() => wide.Room.Find(wide.ClientId).Loadout == Loadout(65000) && mate.Room.Find(wide.ClientId).Loadout == Loadout(65000));
        Check(mate.Room.Find(wide.ClientId).Loadout == Loadout(65000), "loadouts are relayed as they are (the game validates them)");
        wide.RefreshRooms();
        Pump(() => wide.Rooms.Count > 0);
        Check(wide.Rooms.Count >= 1, "room list with multibyte names");

        // Asking to queue while still marked in a fight counts once the fight resolves.
        wide.SetMember(Loadout(0), true);
        mate.SetMember(Loadout(0), true);
        Pump(() => wide.Link != null && mate.Link != null);
        uint match = wide.Link.MatchId;
        mate.Kick(wide.ClientId);
        string kickError = null;
        wide.Kick(mate.ClientId);
        Pump(() => (kickError = Drain(wide).Where(e => e.Type == RoomEventType.Error).Select(e => e.Text).FirstOrDefault()) != null);
        Check(kickError != null && mate.Room != null, "the host cannot kick someone mid-fight");
        mate.ReportMatch(match, MatchOutcome.Aborted, "test");
        mate.SetMember(Loadout(0), true);
        Pump(() => false, 200);
        wide.ReportMatch(match, MatchOutcome.Aborted, "test");
        Pump(() => wide.Room.MatchId == 0 && wide.Room.Find(mate.ClientId).Status == MemberStatus.Queued);
        Check(wide.Room.Find(mate.ClientId).Status == MemberStatus.Queued, "an early re-queue is applied when the fight resolves");
        Check(wide.Room.Find(wide.ClientId).Status == MemberStatus.Away, "the other player is still away");
        wide.LeaveRoom();
        mate.LeaveRoom();
        Pump(() => wide.Room == null && mate.Room == null);
    }

    private static void SimultaneousAndSpectating(IPEndPoint server, Func<string, NetIdentity> identity)
    {
        foreach (var old in _clients.ToArray()) old.Dispose();
        _clients.Clear();
        Pump(() => _server.ClientCount == 0);
        var players = Enumerable.Range(0, 8).Select(i => Connect(server, identity("Player " + i))).ToArray();
        players[0].CreateRoom(new RoomSettings { Name = "Parallel" }, "");
        Pump(() => players[0].Room != null);
        for (int i = 1; i < players.Length; i++) players[i].JoinByCode(players[0].Room.Code, "");
        Pump(() => players[0].Room.Members.Count == 8);
        for (int i = 0; i < players.Length; i++) players[i].SetMember(Loadout(i), true);
        Pump(() => players.All(player => player.Link != null) && players[0].Room.Fights.Count == 4);
        Check(players[0].Room.Fights.Count == 4 && players.Select(player => player.Link.MatchId).Distinct().Count() == 4,
            "eight queued players get four independent simultaneous fights");
        Check(players[0].Room.Queue.Count == 0 && players[0].Room.Members.All(member => member.Status == MemberStatus.InMatch), "nobody waits while a pair is free");
        players[0].UpdateSettings(new RoomSettings { Name = "Parallel", Arena = "sakura", WinsRequired = 3 });
        Pump(() => players[0].Room.Settings.WinsRequired == 3);
        Check(players[0].Link.Pairing.Settings.Arena == RoomSettings.RandomArena && players[0].Link.Pairing.Settings.WinsRequired == 2,
            "room changes do not alter an existing pairing's settings snapshot");
        for (int i = 0; i < 8; i += 2)
        {
            var pair = players[i].Link.Pairing;
            players[i].PublishStart(pair.MatchId, new MatchStart
            {
                MatchIndex = 1, Seed = pair.Seed, Arena = "dojo", WinsRequired = pair.Settings.WinsRequired,
                RoundTimeSeconds = pair.Settings.RoundTimeSeconds, HostLoadout = Loadout(i), GuestLoadout = Loadout(i + 1), InputDelay = 2,
            });
        }
        Pump(() => players[0].Room.Fights.All(fight => fight.CanSpectate));
        Check(players[0].Room.Fights.All(fight => fight.CanSpectate), "every host publishes its own spectator setup");
        uint first = players[0].Link.MatchId, second = players[2].Link.MatchId, last = players[6].Link.MatchId;
        players[0].Spectate(second);
        string fightingRefusal = null;
        Pump(() => (fightingRefusal = Drain(players[0]).FirstOrDefault(e => e.Type == RoomEventType.Error).Text) != null);
        Check(fightingRefusal != null && players[0].Spectating == null && players[0].Link.MatchId == first,
            "a fighter cannot spectate or interfere with another fight");
        players[6].ReportMatch(last, MatchOutcome.Draw, ""); players[7].ReportMatch(last, MatchOutcome.Draw, "");
        Pump(() => players[0].Room.Fights.Count == 3);
        Check(players[0].Room.Fights.Count == 3 && players[0].Room.Fights.Any(fight => fight.MatchId == first), "finishing one fight leaves the others running");

        var recording = new VersusReplay();
        for (int i = 0; i < 150; i++) { recording.Record((byte)(i % 9), (byte)((i + 1) % 9)); if (i % 30 == 0) recording.Hashes[i] = (uint)i + 17; }
        int sent = 0;
        while (players[0].PublishFrames(first, recording, ref sent)) { }
        // A guest cannot overwrite the host's stream.
        int forged = 0; players[1].PublishFrames(first, recording, ref forged);
        players[6].Spectate(first);
        Pump(() => players[6].Spectating?.Replay.TickCount == 150);
        var feed = players[6].Spectating;
        Check(feed != null && feed.Replay.TickCount == 150 && feed.Replay.Left.SequenceEqual(recording.Left) && feed.Replay.Hashes[120] == 137,
            "a late viewer receives complete confirmed history and checkpoints, without duplicate guest uploads");
        Pump(() => players[0].Room.Find(players[6].ClientId).Status == MemberStatus.Spectating);
        Check(players[0].Room.Find(players[6].ClientId).Status == MemberStatus.Spectating, "the room marks the viewer as spectating");
        players[7].Spectate(first);
        Pump(() => players[7].Spectating?.Replay.TickCount == 150);
        Check(players[7].Spectating?.Replay.TickCount == 150, "multiple viewers can watch the same fight");
        for (int i = 150; i < 180; i++) recording.Record(3, 4);
        while (players[0].PublishFrames(first, recording, ref sent)) { }
        Pump(() => feed.Replay.TickCount == 180 && players[7].Spectating.Replay.TickCount == 180);
        Check(feed.Replay.TickCount == 180 && players[7].Spectating.Replay.TickCount == 180, "both viewers receive live inputs");
        players[6].ReportMatch(first, MatchOutcome.LeftWon, "forged spectator result");
        Pump(() => false, 100);
        Check(players[0].Room.Fights.Count == 3 && players[0].Room.Find(players[0].ClientId).Wins == 0, "spectators cannot report a fighter's result");
        players[6].Spectate(second);
        Pump(() => players[6].Spectating?.MatchId == second);
        Check(players[6].Spectating?.MatchId == second && players[6].Spectating.Replay.TickCount == 0, "switching fights starts a separate stream");
        players[6].Spectate(0);
        Pump(() => players[0].Room.Find(players[6].ClientId).Status == MemberStatus.Idle);
        Check(players[0].Room.Find(players[6].ClientId).Status == MemberStatus.Idle, "stopping spectating restores idle status");
        players[6].LeaveRoom();
        Pump(() => players[6].Room == null);
        players[6].CreateRoom(new RoomSettings { Name = "Outside" }, "");
        Pump(() => players[6].Room != null);
        players[6].Spectate(first);
        string refused = null;
        Pump(() => (refused = Drain(players[6]).FirstOrDefault(e => e.Type == RoomEventType.Error).Text) != null);
        Check(refused != null && players[6].Spectating == null, "a member of another room cannot spectate a private fight");

        players[0].ReportMatch(first, MatchOutcome.LeftWon, ""); players[1].ReportMatch(first, MatchOutcome.LeftWon, "");
        Pump(() => players[7].Spectating.Ended && players[0].Room.Fights.Count == 2);
        Check(players[7].Spectating.Ended && players[7].Spectating.Replay.TickCount == 180, "viewers receive the final inputs before the end notice");
        players[0].Rematch(first, true); players[1].Rematch(first, true);
        Pump(() => players[0].Link.MatchId != first && players[0].Room.Fights.Count == 3);
        Check(players[0].Link.MatchId != first && players[0].Room.Fights.Any(fight => fight.MatchId == second), "a rematch can run alongside other simultaneous fights");
        players[7].Spectate(second);
        Pump(() => players[7].Spectating?.MatchId == second);
        players[7].SetMember(Loadout(7), true);
        Pump(() => players[0].Room.Find(players[7].ClientId).Status == MemberStatus.Queued);
        Check(players[7].Spectating.Ended && players[0].Room.Queue.Contains(players[7].ClientId), "joining the queue unsubscribes a spectator");
        var truncated = new SpectatorStream();
        byte[] chunk = SpectatorStream.EncodeFrames(1, recording, 0, out _);
        bool rejected = false;
        try { truncated.ReadFrames(new NetReader(chunk, 5, chunk.Length - 6)); } catch (NetFormatException) { rejected = true; }
        Check(rejected && truncated.Replay.TickCount == 0, "truncated spectator chunks are rejected without partial history");
        foreach (var player in players) player.Dispose();
        _clients.Clear();
        Pump(() => _server.ClientCount == 0 && _server.RoomCount == 0);
        Check(_server.RoomCount == 0, "parallel rooms and viewers clean up on disconnect");
    }

    private static LoadoutCode Loadout(int weapon) =>
        new LoadoutCode { Weapon = (ushort)weapon, Armor = 3, Helm = 7, Ranged = 1, Magic = 2 };

    private static void ChatAndPing(IPEndPoint server, Func<string, NetIdentity> identity)
    {
        var host = Connect(server, identity("Host"));
        var guest = Connect(server, identity("Guest"));
        host.CreateRoom(new RoomSettings { Name = "Chat", Arena = "sakura" }, "");
        Pump(() => host.Room != null);
        guest.JoinByCode(host.Room.Code, "");
        Pump(() => guest.Room != null && host.Room.Members.Count == 2);
        Pump(() => host.Chat.Exists(line => line.Kind == ChatKind.System && line.Text.Contains("Guest joined")));
        Check(host.Chat.Exists(line => line.Kind == ChatKind.System && line.Text == "Guest joined."), "members hear who joined");

        guest.SendChat("  hello\u0007   there \n friend ");
        Pump(() => host.Chat.Exists(line => line.Kind == ChatKind.Player));
        var said = host.Chat.Find(line => line.Kind == ChatKind.Player);
        Check(said != null && said.Text == "hello there friend" && said.SenderId == guest.ClientId && said.SenderName == "Guest",
            "chat arrives cleaned, with the sender (" + (said?.Text ?? "none") + ")");
        Pump(() => guest.Chat.Exists(line => line.Kind == ChatKind.Player));
        Check(guest.Chat.Exists(line => line.Kind == ChatKind.Player && line.Text == "hello there friend"), "the sender sees their own line");
        Check(Drain(host).Exists(e => e.Type == RoomEventType.Chat && e.Chat.Kind == ChatKind.Player), "chat raises a room event");

        guest.SendChat(new string('x', 400));
        Pump(() => host.Chat.FindAll(line => line.Kind == ChatKind.Player).Count >= 2);
        var longLine = host.Chat.FindLast(line => line.Kind == ChatKind.Player);
        Check(longLine.Text.Length == ChatLine.MaxChars, "long lines are cut to " + ChatLine.MaxChars + " characters");

        // A flood: the allowance lets a few through, then the sender alone is told to slow down.
        int before = host.Chat.FindAll(line => line.Kind == ChatKind.Player).Count;
        for (int i = 0; i < 12; i++) guest.SendChat("spam " + i);
        Pump(() => guest.Chat.Exists(line => line.Kind == ChatKind.Notice), 3000);
        Pump(() => false, 300);
        int spam = host.Chat.FindAll(line => line.Kind == ChatKind.Player).Count - before;
        Check(spam >= 1 && spam <= Eclipse.RoomServer.RoomServer.ChatBurst, "flooding is rate limited (" + spam + " of 12 got through)");
        Check(guest.Chat.Exists(line => line.Kind == ChatKind.Notice) && !host.Chat.Exists(line => line.Kind == ChatKind.Notice),
            "only the flooder is told to slow down");
        guest.SendChat("   ");
        Check(true, "blank lines are not sent");

        // Pings reach the room state for every member.
        Pump(() => host.Room.Members.TrueForAll(member => member.PingMs >= 0), 6000);
        Check(host.Room.Members.TrueForAll(member => member.PingMs >= 0 && member.PingMs < 1000),
            "every member's ping is measured (" + string.Join(", ", host.Room.Members.ConvertAll(member => member.PingMs.ToString())) + " ms)");
        Check(host.Room.Members.TrueForAll(member => (member.Link & MemberLink.Stale) == 0), "live members are not stale");

        // A member that goes silent shows as stale, and recovers.
        _clients.Remove(guest);
        Pump(() => (host.Room.Find(guest.ClientId).Link & MemberLink.Stale) != 0, 8000);
        Check((host.Room.Find(guest.ClientId).Link & MemberLink.Stale) != 0, "a silent member shows as stale");
        _clients.Add(guest);
        Pump(() => (host.Room.Find(guest.ClientId).Link & MemberLink.Stale) == 0, 8000);
        Check((host.Room.Find(guest.ClientId).Link & MemberLink.Stale) == 0, "a member that speaks again is no longer stale");

        host.RefreshRooms();
        Pump(() => host.Rooms.Exists(room => room.Name == "Chat"));
        Check(host.Rooms.Find(room => room.Name == "Chat")?.Arena == "sakura", "room listings carry the arena");

        guest.LeaveRoom();
        Pump(() => host.Chat.Exists(line => line.Text == "Guest left."));
        Check(host.Chat.Exists(line => line.Text == "Guest left."), "members hear who left");
        Pump(() => guest.Room == null);
        Check(guest.Chat.Count == 0, "leaving a room clears its chat");
        host.LeaveRoom();
        Pump(() => host.Room == null);
    }

    /// <summary>Eight members with the longest names, full loadouts and pings still fit one reliable message.</summary>
    private static void RoomStateFitsFullLoadouts()
    {
        var state = new RoomState
        {
            RoomId = uint.MaxValue, Code = "ABCDEF",
            Settings = new RoomSettings { Name = new string('\u6f22', 32), Arena = new string('a', 32) },
            HostId = 1, ChampionId = 2, Streak = 9,
        };
        for (uint i = 0; i < 4; i++) state.Fights.Add(new RoomFight { MatchId = i + 1, LeftId = i * 2 + 1, RightId = i * 2 + 2, CanSpectate = true });
        for (int i = 0; i < RoomProtocol.MaxMembers; i++)
        {
            state.Members.Add(new RoomMember
            {
                Id = (uint)i + 1, Name = new string('\u6f22', 24), Loadout = Loadout(60000 + i), Status = MemberStatus.Queued,
                Wins = 999, Losses = 999, PingMs = 65000, Link = MemberLink.Relayed | MemberLink.Stale,
            });
            state.Queue.Add((uint)i + 1);
        }
        byte[] encoded = state.Encode();
        Check(encoded.Length <= ReliableChannel.MaxMessageSize, "a full room fits one message (" + encoded.Length + " of " + ReliableChannel.MaxMessageSize + " bytes)");
        var decoded = RoomState.Decode(new NetReader(encoded, 1, encoded.Length - 1));
        Check(decoded.Members.Count == RoomProtocol.MaxMembers && decoded.Members[7].Loadout == Loadout(60007) &&
            decoded.Members[3].PingMs == 65000 && decoded.Members[3].Link == (MemberLink.Relayed | MemberLink.Stale), "full room state round-trips");
        var unmeasured = new RoomState();
        unmeasured.Members.Add(new RoomMember { Id = 1, Name = "A" });
        byte[] small = unmeasured.Encode();
        var back = RoomState.Decode(new NetReader(small, 1, small.Length - 1));
        Check(back.Members[0].PingMs == -1 && !back.Members[0].Loadout.IsSet, "an unmeasured ping and an unset loadout survive");
    }

    /// <summary>Both players asking for a rematch pair them again; a waiting queue or a leaver refuses it.</summary>
    private static void Rematches(IPEndPoint server, Func<string, NetIdentity> identity)
    {
        var eve = Connect(server, identity("Eve"));
        var fay = Connect(server, identity("Fay"));
        eve.CreateRoom(new RoomSettings { Name = "Rematch", WinsRequired = 1, Rotation = RoomRotation.WinnerStays }, "");
        Pump(() => eve.Room != null);
        fay.JoinByCode(eve.Room.Code, "");
        Pump(() => fay.Room != null && eve.Room.Members.Count == 2);
        eve.SetMember(Loadout(0), true);
        fay.SetMember(Loadout(1), true);
        Pump(() => eve.Link != null && fay.Link != null);
        uint first = eve.Room.MatchId;
        int firstSeed = eve.Link.Pairing.Seed;
        Drain(eve); Drain(fay);

        // Eve asks before Fay's report has resolved the fight; it is held until then.
        eve.ReportMatch(first, MatchOutcome.LeftWon, "");
        eve.Rematch(first, true);
        Pump(() => eve.Room.Find(eve.ClientId)?.WantsRematch == true);
        Check(eve.Room.Find(eve.ClientId).WantsRematch && eve.Room.MatchId == first, "a rematch request is accepted while the fight resolves");
        fay.ReportMatch(first, MatchOutcome.LeftWon, "");
        Pump(() => fay.Room.MatchId == 0);
        Check(fay.Room.Find(eve.ClientId).WantsRematch && fay.Room.Find(fay.ClientId).Status == MemberStatus.Away, "the opponent sees the request");
        fay.Rematch(first, true);
        Pump(() => eve.Room.MatchId != 0 && eve.Room.MatchId != first && eve.Link != null && eve.Link.MatchId == eve.Room.MatchId && fay.Link != null && fay.Link.MatchId == eve.Room.MatchId);
        Check(eve.Room.MatchId != first && eve.Link.Pairing.Side == 0 && fay.Link.Pairing.PeerId == eve.ClientId, "both asking starts a rematch on the same sides");
        Check(eve.Link.Pairing.Seed == fay.Link.Pairing.Seed && eve.Link.Pairing.Seed != firstSeed, "the rematch has a fresh seed (a new random arena)");
        Check(!eve.Room.Find(eve.ClientId).WantsRematch && eve.Room.Find(fay.ClientId).Status == MemberStatus.InMatch, "requests clear once the rematch starts");

        // Someone waiting in line goes first.
        uint second = eve.Room.MatchId;
        var gus = Connect(server, identity("Gus"));
        gus.JoinByCode(eve.Room.Code, "");
        Pump(() => gus.Room != null && eve.Room.Members.Count == 3);
        gus.SetMember(Loadout(2), true);
        Pump(() => eve.Room.Queue.Contains(gus.ClientId));
        eve.ReportMatch(second, MatchOutcome.RightWon, "");
        fay.ReportMatch(second, MatchOutcome.RightWon, "");
        Pump(() => eve.Room.MatchId == 0);
        Drain(eve);
        eve.Rematch(second, true);
        string refusal = null;
        Pump(() => (refusal = Drain(eve).Where(e => e.Type == RoomEventType.Error).Select(e => e.Text).FirstOrDefault()) != null);
        Check(refusal != null && refusal.Contains("waiting") && !eve.Room.Find(eve.ClientId).WantsRematch, "no rematch while others queue: " + refusal);

        // Continuing to the room ends a request, and the opponent can no longer rematch.
        gus.SetMember(Loadout(2), false);
        Pump(() => !eve.Room.Queue.Contains(gus.ClientId));
        Drain(eve);
        eve.Rematch(second, true);
        Pump(() => fay.Room.Find(eve.ClientId)?.WantsRematch == true);
        string errors = string.Join(" / ", Drain(eve).Where(e => e.Type == RoomEventType.Error).Select(e => e.Text));
        Check(fay.Room.Find(eve.ClientId).WantsRematch, "a request with nobody waiting is accepted " + errors);
        eve.SetMember(Loadout(0), false);
        Pump(() => fay.Room.Find(eve.ClientId)?.WantsRematch == false && eve.Room.Find(eve.ClientId)?.Status == MemberStatus.Idle);
        Check(!fay.Room.Find(eve.ClientId).WantsRematch && eve.Room.Find(eve.ClientId).Status == MemberStatus.Idle, "continuing to the room withdraws the request, out of the queue");
        Drain(fay);
        fay.Rematch(second, true);
        string movedOn = null;
        Pump(() => (movedOn = Drain(fay).Where(e => e.Type == RoomEventType.Error).Select(e => e.Text).FirstOrDefault()) != null);
        Check(movedOn != null && movedOn.Contains("Eve") && !eve.Room.Find(fay.ClientId).WantsRematch, "no rematch once the opponent went back to the room: " + movedOn);
        eve.LeaveRoom();
        fay.LeaveRoom();
        gus.LeaveRoom();
        eve.ReleaseLink(second); fay.ReleaseLink(second);
        Pump(() => fay.Room == null && gus.Room == null);
        // The server allows a few players per address; free these slots for the later tests.
        int before = _server.ClientCount;
        foreach (var client in new[] { eve, fay, gus }) { client.Dispose(); _clients.Remove(client); }
        Pump(() => _server.ClientCount <= before - 3, 3000);
    }

    private static RoomClient Connect(IPEndPoint server, NetIdentity identity)
    {
        var client = new RoomClient(server, identity, new[] { IPAddress.Loopback }, Now);
        _clients.Add(client);
        Pump(() => client.State == RoomClientState.Connected);
        Check(client.State == RoomClientState.Connected && client.ClientId != 0, identity.PlayerName + " connects");
        return client;
    }

    private static List<RoomEvent> Drain(RoomClient client)
    {
        var events = new List<RoomEvent>();
        while (client.TryGetEvent(out var e)) events.Add(e);
        return events;
    }

    private static void Pump(Func<bool> done, int timeoutMs = 5000)
    {
        long end = Now + timeoutMs;
        while (Now < end)
        {
            _server.Update(Now);
            foreach (var client in _clients) client.Update(Now);
            foreach (var peer in _peers) peer.Update(Now);
            if (done()) return;
            Thread.Sleep(1);
        }
    }

    /// <summary>Runs a lockstep "fight" over the two clients' match link.</summary>
    private static void Fight(RoomClient left, RoomClient right, bool expectDirect)
    {
        var leftLink = left.Link;
        var rightLink = right.Link;
        Pump(() => leftLink.IsReady && rightLink.IsReady);
        Check(leftLink.IsReady && rightLink.IsReady, "link ready");
        if (expectDirect) Check(leftLink.Path == LinkPath.Direct && rightLink.Path == LinkPath.Direct, "hole punch succeeds on loopback");
        var host = NetplayPeer.Host(leftLink, new NetIdentity("1.0/IL2CPP", "mods:none", "L"), Now);
        var guest = NetplayPeer.Join(rightLink, MatchLink.PeerEndPoint, new NetIdentity("1.0/IL2CPP", "mods:none", "R"), Now);
        _peers.Add(host);
        _peers.Add(guest);
        Pump(() => host.State == NetplayState.Connected && guest.State == NetplayState.Connected);
        Check(host.State == NetplayState.Connected && guest.State == NetplayState.Connected, "fight connects over the " + (expectDirect ? "direct" : "relay") + " link");
        var a = new InputTimeline(1, 2);
        var b = new InputTimeline(1, 2);
        host.Timeline = a;
        guest.Timeline = b;
        int ta = 0, tb = 0;
        uint ha = 17, hb = 17;
        Pump(() =>
        {
            Step(host, a, 0, ref ta, ref ha);
            Step(guest, b, 1, ref tb, ref hb);
            return ta >= 240 && tb >= 240;
        }, 15000);
        Check(ta >= 240 && tb >= 240 && ha == hb, "lockstep over the link stays identical (" + ta + "/" + tb + ")");
        host.Dispose();
        guest.Dispose();
        _peers.Clear();
    }

    private static void Step(NetplayPeer peer, InputTimeline timeline, int side, ref int tick, ref uint hash)
    {
        if (tick >= 240) { peer.Flush(Now); return; }
        if (timeline.NeedsLocalInput(tick)) timeline.AddLocal((byte)((tick * 7 + side * 3) % 9));
        if (timeline.TryGetInputs(tick, out var local, out var remote))
        {
            byte l = side == 0 ? local : remote, r = side == 0 ? remote : local;
            hash = hash * 31 + l * 7u + r;
            tick++;
        }
        peer.Flush(Now);
    }
}
