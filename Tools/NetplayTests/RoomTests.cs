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
        using (_server = new Eclipse.RoomServer.RoomServer(0))
        {
            var server = new IPEndPoint(IPAddress.Loopback, _server.Port);
            var identity = new Func<string, NetIdentity>(name => new NetIdentity("1.0/IL2CPP", "mods:none", name));
            var ann = Connect(server, identity("Ann"));
            var bo = Connect(server, identity("Bo"));
            var cy = Connect(server, identity("Cy"));
            var modded = Connect(server, new NetIdentity("1.0/IL2CPP", "mods:x@1", "Dee"));

            // Create, list and join.
            ann.CreateRoom(new RoomSettings { Name = "Dojo night", WinsRequired = 1, Arena = "dojo" }, "");
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
            ann.SetMember("Fists", true);
            Pump(() => ann.Room.Queue.Count == 1);
            bo.SetMember("WEAPON_KATANA", true);
            Pump(() => ann.Link != null && bo.Link != null);
            cy.SetMember("WEAPON_STAFF", true);
            Pump(() => ann.Room.Queue.Contains(cy.ClientId));
            Check(ann.Link.Pairing.Side == 0 && bo.Link.Pairing.Side == 1 && ann.Link.Pairing.PeerName == "Bo" &&
                bo.Link.Pairing.PeerWeapon == "Fists" && ann.Link.Pairing.Seed == bo.Link.Pairing.Seed, "first two in queue paired");
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
            bo.SetMember("WEAPON_KATANA", true);
            ann.SetMember("Fists", true);
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

            ann.SetMember("Fists", true);
            cy.SetMember("WEAPON_STAFF", true);
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

            Hardening(server, identity);

            foreach (var client in _clients) client.Dispose();
            Pump(() => _server.ClientCount == 0, 2000);
            Check(_server.ClientCount == 0, "clients disconnect cleanly");
        }
        _clients.Clear();
        _peers.Clear();
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
        wide.SetMember("../../etc/passwd", false);
        Pump(() => false, 200);
        Check(wide.Room.Find(wide.ClientId).Weapon == "", "junk weapon ids are ignored");
        wide.RefreshRooms();
        Pump(() => wide.Rooms.Count > 0);
        Check(wide.Rooms.Count >= 1, "room list with multibyte names");

        // Asking to queue while still marked in a fight counts once the fight resolves.
        wide.SetMember("Fists", true);
        mate.SetMember("Fists", true);
        Pump(() => wide.Link != null && mate.Link != null);
        uint match = wide.Link.MatchId;
        mate.Kick(wide.ClientId);
        string kickError = null;
        wide.Kick(mate.ClientId);
        Pump(() => (kickError = Drain(wide).Where(e => e.Type == RoomEventType.Error).Select(e => e.Text).FirstOrDefault()) != null);
        Check(kickError != null && mate.Room != null, "the host cannot kick someone mid-fight");
        mate.ReportMatch(match, MatchOutcome.Aborted, "test");
        mate.SetMember("Fists", true);
        Pump(() => false, 200);
        wide.ReportMatch(match, MatchOutcome.Aborted, "test");
        Pump(() => wide.Room.MatchId == 0 && wide.Room.Find(mate.ClientId).Status == MemberStatus.Queued);
        Check(wide.Room.Find(mate.ClientId).Status == MemberStatus.Queued, "an early re-queue is applied when the fight resolves");
        Check(wide.Room.Find(wide.ClientId).Status == MemberStatus.Away, "the other player is still away");
        wide.LeaveRoom();
        mate.LeaveRoom();
        Pump(() => wide.Room == null && mate.Room == null);
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
