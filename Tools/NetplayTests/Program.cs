using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using Eclipse.Multiplayer.Online;

internal static class Program
{
    private static int _checks;

    internal static void Check(bool condition, string message)
    {
        _checks++;
        if (!condition) throw new Exception("FAIL: " + message);
    }

    private static int Main()
    {
        try
        {
            InputPacking();
            BufferBounds();
            ReliableUnderLossAndReorder();
            LockstepInMemory(lossPercent: 0, seed: 1);
            LockstepInMemory(lossPercent: 35, seed: 2);
            DesyncDetection();
            ReplayRoundTrip();
            UdpHandshakeRejectsMismatch();
            UdpMatch(latencyMs: 0, lossPercent: 0, delay: 2);
            UdpMatch(latencyMs: 40, lossPercent: 10, delay: 4);
            UdpDisconnectNotifies();
            RoomTests.Run(Check);
            Console.WriteLine("PASS: " + _checks + " online versus core checks.");
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            return 1;
        }
    }

    private static void InputPacking()
    {
        byte input = NetInput.WithDirection(0, 7);
        input = NetInput.WithButton(input, NetInput.Punch, true);
        input = NetInput.WithButton(input, NetInput.Magic, true);
        Check(NetInput.Direction(input) == 7, "direction packing");
        input = NetInput.WithButton(input, NetInput.Punch, false);
        Check(input == (7 | NetInput.Magic), "button release packing");
        Check(NetInput.IsValid(0x88) && !NetInput.IsValid(0x09) && !NetInput.IsValid(0x0F), "direction validation");
        Check(NetInput.WithDirection(NetInput.Kick, 99) == NetInput.Kick, "out of range direction becomes neutral");
    }

    private static void BufferBounds()
    {
        var writer = new NetWriter(64);
        writer.U8(200); writer.U16(65000); writer.I32(-5); writer.Str("Katana é"); writer.Bool(true);
        var reader = new NetReader(writer.ToArray());
        Check(reader.U8() == 200 && reader.U16() == 65000 && reader.I32() == -5 && reader.Str() == "Katana é" && reader.Bool(), "round trip");
        bool threw = false;
        try { reader.U8(); } catch (NetFormatException) { threw = true; }
        Check(threw, "reading past the end throws a format error");
        threw = false;
        try { new NetReader(new byte[] { 50, 1, 2 }).Str(); } catch (NetFormatException) { threw = true; }
        Check(threw, "string length beyond packet throws");
        threw = false;
        try { var small = new NetWriter(3); small.U32(1); } catch (InvalidOperationException) { threw = true; }
        Check(threw, "writer enforces capacity");
    }

    private static void ReliableUnderLossAndReorder()
    {
        var random = new Random(11);
        var a = new ReliableChannel();
        var b = new ReliableChannel();
        var inFlightToB = new List<byte[]>();
        var inFlightToA = new List<byte[]>();
        const int total = 300;
        int sent = 0;
        var received = new List<int>();
        for (long now = 0; now < 200000 && received.Count < total; now += 16)
        {
            if (sent < total && a.PendingCount < 40) { a.Send(new[] { (byte)1, (byte)(sent & 0xFF), (byte)(sent >> 8) }); sent++; }
            var writer = new NetWriter();
            a.Write(writer, now, 0);
            if (random.Next(100) >= 30) inFlightToB.Insert(random.Next(inFlightToB.Count + 1), writer.ToArray());
            writer = new NetWriter();
            b.Write(writer, now, 0);
            if (random.Next(100) >= 30) inFlightToA.Insert(random.Next(inFlightToA.Count + 1), writer.ToArray());
            Deliver(inFlightToB, b, random);
            Deliver(inFlightToA, a, random);
            while (b.TryReceive(out var message)) received.Add(message[1] | (message[2] << 8));
        }
        Check(received.Count == total, "all reliable messages arrive (" + received.Count + ")");
        for (int i = 0; i < total; i++) Check(received[i] == i, "reliable order at " + i);
        bool threw = false;
        try { a.Send(new byte[ReliableChannel.MaxMessageSize + 1]); } catch (ArgumentException) { threw = true; }
        Check(threw, "oversized reliable message rejected");
    }

    private static void Deliver(List<byte[]> queue, ReliableChannel target, Random random)
    {
        int count = Math.Min(queue.Count, random.Next(3));
        for (int i = 0; i < count; i++)
        {
            target.Read(new NetReader(queue[0]));
            queue.RemoveAt(0);
        }
    }

    /// <summary>A stand-in for the fight: any divergence in applied inputs changes its hash.</summary>
    private sealed class ToySim
    {
        public int X, Y, Tick;
        public void Step(byte left, byte right)
        {
            X = X * 31 + left + 7;
            Y = Y * 17 + right * 3 + (X & 0xFF);
            Tick++;
        }
        public uint Hash()
        {
            var hasher = new StateHasher();
            hasher.Add(X); hasher.Add(Y); hasher.Add(Tick);
            return hasher.Value;
        }
    }

    private static byte ScriptedInput(int side, int tick)
    {
        int phase = (tick / (5 + side * 3)) % 11;
        byte input = NetInput.WithDirection(0, phase % 9);
        if (phase % 4 == 1) input |= NetInput.Punch;
        if (phase % 5 == 2) input |= NetInput.Kick;
        return input;
    }

    private sealed class Side
    {
        public LockstepTimeline Timeline;
        public ToySim Sim = new ToySim();
        public int Index;
        public List<(byte, byte)> Applied = new List<(byte, byte)>();

        public bool TryStep(Func<int, int, byte> inputFor)
        {
            int tick = Sim.Tick;
            if (Timeline.NeedsLocalInput(tick)) Timeline.AddLocal(inputFor(Index, tick + Timeline.Delay));
            if (!Timeline.TryGetInputs(tick, out var local, out var remote)) return false;
            byte left = Index == 0 ? local : remote;
            byte right = Index == 0 ? remote : local;
            Sim.Step(left, right);
            Applied.Add((left, right));
            if (tick % NetProtocol.HashInterval == 0) Timeline.RecordLocalHash(tick, Sim.Hash());
            return true;
        }
    }

    private static void LockstepInMemory(int lossPercent, int seed)
    {
        const int delay = 3, ticks = 900;
        var random = new Random(seed);
        var host = new Side { Index = 0, Timeline = new LockstepTimeline(4, delay) };
        var guest = new Side { Index = 1, Timeline = new LockstepTimeline(4, delay) };
        int stalls = 0;
        for (int step = 0; step < ticks * 20 && (host.Sim.Tick < ticks || guest.Sim.Tick < ticks); step++)
        {
            if (host.Sim.Tick < ticks && !host.TryStep(ScriptedInput)) stalls++;
            if (guest.Sim.Tick < ticks && random.Next(4) != 0 && !guest.TryStep(ScriptedInput)) stalls++;
            Exchange(host.Timeline, guest.Timeline, random, lossPercent);
            Exchange(guest.Timeline, host.Timeline, random, lossPercent);
        }
        Check(host.Sim.Tick == ticks && guest.Sim.Tick == ticks, "both sides finish (" + host.Sim.Tick + "/" + guest.Sim.Tick + ")");
        Check(host.Sim.Hash() == guest.Sim.Hash(), "final state identical");
        for (int i = 0; i < ticks; i++) Check(host.Applied[i] == guest.Applied[i], "applied inputs identical at tick " + i);
        for (int i = 0; i < delay; i++) Check(host.Applied[i] == (0, 0), "opening delay ticks are neutral");
        Check(host.Applied[delay + 10].Item1 == ScriptedInput(0, delay + 10) && host.Applied[delay + 10].Item2 == ScriptedInput(1, delay + 10), "inputs land on their scheduled tick");
        Check(host.Timeline.DesyncTick < 0 && guest.Timeline.DesyncTick < 0, "no false desync");
        Check(host.Timeline.VerifiedTick > ticks - 200, "state hashes verified late into the match (" + host.Timeline.VerifiedTick + ")");
        var other = new LockstepTimeline(5, delay);
        var writer = new NetWriter();
        host.Timeline.WriteSync(writer);
        Check(!other.ReadSync(new NetReader(writer.ToArray())) && other.RemoteFrames == delay, "stale match index ignored");
    }

    private static void Exchange(LockstepTimeline from, LockstepTimeline to, Random random, int lossPercent)
    {
        var writer = new NetWriter();
        from.WriteSync(writer);
        if (random.Next(100) < lossPercent) return;
        to.ReadSync(new NetReader(writer.ToArray()));
    }

    private static void DesyncDetection()
    {
        var a = new LockstepTimeline(0, 1);
        var b = new LockstepTimeline(0, 1);
        a.RecordLocalHash(0, 10); b.RecordLocalHash(0, 10);
        Exchange(a, b, new Random(1), 0); Exchange(b, a, new Random(1), 0);
        Check(a.VerifiedTick == 0 && a.DesyncTick < 0, "matching hash verifies");
        a.RecordLocalHash(30, 99); b.RecordLocalHash(30, 98);
        Exchange(a, b, new Random(1), 0);
        Check(b.DesyncTick == 30, "remote mismatch reported on arrival");
        var c = new LockstepTimeline(0, 1);
        var d = new LockstepTimeline(0, 1);
        d.RecordLocalHash(60, 5);
        Exchange(d, c, new Random(1), 0);
        c.RecordLocalHash(60, 6);
        Check(c.DesyncTick == 60, "mismatch detected when the remote hash arrived first");
    }

    private static void ReplayRoundTrip()
    {
        var replay = new VersusReplay
        {
            Build = "0.9.1", Content = "mods:none", LeftName = "Ann", RightName = "Bo",
            LeftWeapon = "WEAPON_STAFF", RightWeapon = "Fists", Arena = "dojo",
            WinsRequired = 3, RoundTimeSeconds = 60, Seed = -12345, Online = true, RecordedUnixSeconds = 1790000000,
        };
        for (int i = 0; i < 5000; i++) replay.Record(ScriptedInput(0, i), ScriptedInput(1, i));
        replay.Hashes[0] = 1; replay.Hashes[30] = 0xFFFFFFFF;
        var stream = new MemoryStream();
        replay.Write(stream);
        Check(stream.Length < 5000, "replay compresses (" + stream.Length + " bytes)");
        stream.Position = 0;
        var loaded = VersusReplay.Read(stream);
        Check(loaded.TickCount == 5000 && loaded.Seed == -12345 && loaded.Online && loaded.RightWeapon == "Fists" &&
            loaded.WinsRequired == 3 && loaded.Hashes[30] == 0xFFFFFFFF && loaded.LeftName == "Ann", "replay fields survive");
        for (int i = 0; i < 5000; i++) Check(loaded.Left[i] == replay.Left[i] && loaded.Right[i] == replay.Right[i], "replay input " + i);
        bool threw = false;
        try { VersusReplay.Read(new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7 })); } catch (InvalidDataException) { threw = true; }
        Check(threw, "foreign file rejected");
    }

    private static readonly Stopwatch Clock = Stopwatch.StartNew();
    private static long Now => Clock.ElapsedMilliseconds;
    private static readonly NetIdentity HostId = new NetIdentity("1.0", "mods:a@1", "Host");
    private static readonly NetIdentity GuestId = new NetIdentity("1.0", "mods:a@1", "Guest");

    private static void Pump(NetplayPeer a, NetplayPeer b, Func<bool> done, int timeoutMs)
    {
        long end = Now + timeoutMs;
        while (!done() && Now < end)
        {
            a.Update(Now);
            b.Update(Now);
            Thread.Sleep(1);
        }
    }

    private static void UdpHandshakeRejectsMismatch()
    {
        using (var host = NetplayPeer.Host(0, HostId, Now))
        using (var guest = NetplayPeer.Join(new IPEndPoint(IPAddress.Loopback, host.LocalPort), new NetIdentity("1.0", "mods:a@2", "Guest"), Now))
        {
            Pump(host, guest, () => guest.State == NetplayState.Closed, 3000);
            Check(guest.State == NetplayState.Closed && guest.CloseReason.Contains("mods"), "mod mismatch rejected: " + guest.CloseReason);
            Check(host.State == NetplayState.WaitingForGuest, "host keeps waiting after rejecting");
        }
        var il2cpp = new NetIdentity("1.0/IL2CPP", "mods:none", "A");
        string runtimeProblem = il2cpp.Incompatibility(new NetIdentity("1.0/Mono", "mods:none", "B"));
        Check(runtimeProblem != null && runtimeProblem.Contains("Mono") && runtimeProblem.Contains("IL2CPP"), "Mono vs IL2CPP rejected with a runtime reason");
        Check(il2cpp.Incompatibility(new NetIdentity("1.1/IL2CPP", "mods:none", "B")).Contains("versions differ"), "version mismatch keeps its reason");
        Check(il2cpp.Incompatibility(new NetIdentity("1.0/IL2CPP", "mods:none", "B")) == null, "same version and runtime accepted");
        Check(NetplayPeer.TryParseAddress("127.0.0.1:9000", out var ep, out _) && ep.Port == 9000, "address with port");
        Check(NetplayPeer.TryParseAddress("127.0.0.1", out ep, out _) && ep.Port == NetProtocol.DefaultPort, "address default port");
        Check(!NetplayPeer.TryParseAddress("127.0.0.1:99999", out _, out var error) && error.Contains("port"), "bad port rejected");
    }

    private static void UdpMatch(int latencyMs, int lossPercent, int delay)
    {
        using (var host = NetplayPeer.Host(0, HostId, Now))
        using (var guest = NetplayPeer.Join(new IPEndPoint(IPAddress.Loopback, host.LocalPort), GuestId, Now))
        {
            host.SimulatedLatencyMs = guest.SimulatedLatencyMs = latencyMs;
            host.SimulatedLossPercent = guest.SimulatedLossPercent = lossPercent;
            Pump(host, guest, () => host.State == NetplayState.Connected && guest.State == NetplayState.Connected, 5000);
            Check(host.State == NetplayState.Connected && guest.State == NetplayState.Connected, "peers connect");
            Check(host.RemoteIdentity.PlayerName == "Guest" && guest.RemoteIdentity.PlayerName == "Host", "identities exchanged");

            // Lobby over the reliable channel, then the host starts the match.
            guest.SendReliable(NetMessages.GuestLobby("WEAPON_KATANA", true));
            LobbyState lobby = null;
            Pump(host, guest, () =>
            {
                if (host.TryReceiveReliable(out var message) && message[0] == (byte)NetMessageType.GuestLobby)
                {
                    var reader = new NetReader(message, 1, message.Length - 1);
                    lobby = new LobbyState { GuestWeapon = reader.Str(), GuestReady = reader.Bool(), HostWeapon = "Fists", Arena = "dojo", InputDelay = delay };
                }
                return lobby != null;
            }, 5000);
            Check(lobby != null && lobby.GuestWeapon == "WEAPON_KATANA" && lobby.GuestReady, "guest lobby arrives");
            var start = new MatchStart { MatchIndex = 1, HostWeapon = lobby.HostWeapon, GuestWeapon = lobby.GuestWeapon, Arena = lobby.Arena, WinsRequired = 2, RoundTimeSeconds = 99, InputDelay = delay, Seed = 4242 };
            host.SendReliable(start.Encode());
            MatchStart received = null;
            Pump(host, guest, () =>
            {
                if (guest.TryReceiveReliable(out var message) && message[0] == (byte)NetMessageType.StartMatch)
                    received = MatchStart.Decode(new NetReader(message, 1, message.Length - 1));
                return received != null;
            }, 5000);
            Check(received != null && received.Seed == 4242 && received.GuestWeapon == "WEAPON_KATANA" && received.InputDelay == delay, "match start arrives");

            const int ticks = 360;
            var a = new Side { Index = 0, Timeline = new LockstepTimeline(received.MatchIndex, delay) };
            var b = new Side { Index = 1, Timeline = new LockstepTimeline(received.MatchIndex, delay) };
            host.Timeline = a.Timeline;
            guest.Timeline = b.Timeline;
            long startMs = Now;
            var sides = new[] { (peer: host, side: a), (peer: guest, side: b) };
            long end = Now + 30000;
            while ((a.Sim.Tick < ticks || b.Sim.Tick < ticks) && Now < end)
            {
                foreach (var (peer, side) in sides)
                {
                    peer.Update(Now);
                    // Each peer runs its fixed step on its own 60 Hz clock.
                    long due = (Now - startMs) * 60 / 1000;
                    int steps = 0;
                    while (side.Sim.Tick < ticks && side.Sim.Tick < due && steps++ < 3 && side.TryStep(ScriptedInput)) { }
                    peer.Flush(Now);
                }
                Thread.Sleep(1);
            }
            Check(a.Sim.Tick == ticks && b.Sim.Tick == ticks, "UDP match finishes (latency " + latencyMs + ", loss " + lossPercent + "%): " + a.Sim.Tick + "/" + b.Sim.Tick);
            Check(a.Sim.Hash() == b.Sim.Hash(), "UDP match state identical");
            // Let the final checkpoint hashes cross.
            Pump(host, guest, () => a.Timeline.VerifiedTick >= ticks - NetProtocol.HashInterval && b.Timeline.VerifiedTick >= ticks - NetProtocol.HashInterval, 3000);
            Check(a.Timeline.DesyncTick < 0 && b.Timeline.DesyncTick < 0 && a.Timeline.VerifiedTick >= ticks - NetProtocol.HashInterval, "UDP hashes verified");
            Check(host.RttMs >= 0 && guest.RttMs >= 0, "round trip measured");
            if (latencyMs > 0) Check(host.RttMs >= latencyMs, "round trip includes simulated latency (" + host.RttMs + " ms)");
        }
    }

    private static void UdpDisconnectNotifies()
    {
        var host = NetplayPeer.Host(0, HostId, Now);
        var guest = NetplayPeer.Join(new IPEndPoint(IPAddress.Loopback, host.LocalPort), GuestId, Now);
        Pump(host, guest, () => host.State == NetplayState.Connected && guest.State == NetplayState.Connected, 5000);
        guest.Close("Returned to title.");
        Pump(host, guest, () => host.State == NetplayState.Closed, 3000);
        Check(host.State == NetplayState.Closed && host.CloseReason.Contains("left"), "host learns the guest left: " + host.CloseReason);
        host.Dispose();
        guest.Dispose();
        var lonely = NetplayPeer.Join(new IPEndPoint(IPAddress.Loopback, 1), GuestId, Now);
        lonely.Update(Now);
        lonely.Update(Now + NetplayPeer.ConnectTimeoutMs + 1);
        Check(lonely.State == NetplayState.Closed && lonely.CloseReason.Contains("reach"), "unreachable host times out");
        lonely.Dispose();
    }
}
