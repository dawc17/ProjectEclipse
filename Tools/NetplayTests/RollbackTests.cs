using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading;
using Eclipse.Multiplayer.Online;

/// <summary>Rollback netcode: graph snapshots, prediction, re-simulation, barriers and time sync.</summary>
internal static class RollbackTests
{
    private static Action<bool, string> Check;

    public static void Run(Action<bool, string> check)
    {
        Check = check;
        // Every snapshot test runs through reflection, then through the raw field-copy path.
        foreach (var raw in new[] { null, new FakeRawMemory() })
        {
            Raw = raw;
            SnapshotRestoresInPlace();
            SnapshotReportsDifferences();
            SnapshotCollections();
            CodecFramesRestoreWhole();
            RollbackInMemory(latencySteps: 0, lossPercent: 0, delay: 1, window: 8, seed: 3);
            RollbackInMemory(latencySteps: 5, lossPercent: 0, delay: 1, window: 8, seed: 4);
            RollbackInMemory(latencySteps: 9, lossPercent: 25, delay: 2, window: 8, seed: 5);
            RollbackInMemory(latencySteps: 14, lossPercent: 10, delay: 1, window: 6, seed: 6);
            if (raw != null) Check(raw.Blocks > 0 && raw.References > 0 && raw.Reads > 0, "raw path copied blocks and references and walked references");
        }
        Raw = null;
        RawLayoutMismatchFallsBack();
        RunLengthInputs();
        PredictionAndRollbackBookkeeping();
        TimeSyncSlowsTheLeader();
        RollbackOverUdp(latencyMs: 50, lossPercent: 5);
        SuggestedDelays();
    }

    // ---- Object graph snapshots ----

    private sealed class Node
    {
        public float X;
        public int Count;
        public string Label = "a";
        public Node Next;
        public readonly List<Node> Children = new List<Node>();
        public Dictionary<string, int> Flags = new Dictionary<string, int>();
        public int[] Numbers = { 1, 2, 3 };
        public Pair Held;
        public object Boxed;
        public Action Callback;
        public Definition Shared;
    }

    private struct Pair
    {
        public int Value;
        public Node Target;
    }

    /// <summary>Immutable data the policy keeps by reference.</summary>
    private sealed class Definition
    {
        public int Id;
    }

    private sealed class Opaque
    {
        public int Id;
    }

    private sealed class TestPolicy : ISnapshotPolicy
    {
        public bool IsOpaque(Type type) => type == typeof(Definition) || type == typeof(Opaque);
        public bool Captures(FieldInfo field) => field.Name != "Label" || field.DeclaringType != typeof(Node);
        public bool NeedsFieldCopy(Type type) => false;
        public SnapshotCodec CodecFor(Type type) => null;
        public PropertyInfo[] ExtraProperties(Type type) => null;
    }

    private static IRawObjectMemory Raw;

    private static ObjectGraphSnapshotter NewSnapshotter() => new ObjectGraphSnapshotter(new TestPolicy(), Raw);

    /// <summary>
    /// Stands in for direct memory access: each field gets an 8-byte slot at a made-up
    /// offset, and copies are done by reflection on the fields those slots name. It checks
    /// that blocks never cover references and that reference slots name references.
    /// </summary>
    private sealed class FakeRawMemory : IRawObjectMemory
    {
        /// <summary>Simulates a wrong layout: block copies do nothing.</summary>
        public bool DropBlocks;
        /// <summary>Starts dropping block copies after this many, or never when negative.</summary>
        public int DropAfter = -1;
        public int Blocks, References, Reads;
        private const BindingFlags Declared = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        private readonly Dictionary<Type, Dictionary<int, FieldInfo>> _maps = new Dictionary<Type, Dictionary<int, FieldInfo>>();

        public int FieldOffset(FieldInfo field)
        {
            int slot = 0;
            for (var type = field.DeclaringType.BaseType; type != null; type = type.BaseType) slot += type.GetFields(Declared).Length;
            return 16 + 8 * (slot + Array.IndexOf(field.DeclaringType.GetFields(Declared), field));
        }

        public int SizeOf(Type valueType) => 8;

        private Dictionary<int, FieldInfo> Map(Type type)
        {
            if (_maps.TryGetValue(type, out var map)) return map;
            map = new Dictionary<int, FieldInfo>();
            for (var current = type; current != null; current = current.BaseType)
                foreach (var field in current.GetFields(Declared)) map[FieldOffset(field)] = field;
            return _maps[type] = map;
        }

        public void CopyBlocks(object from, object to, int[] offsets, int[] sizes)
        {
            var map = Map(from.GetType());
            for (int i = 0; i < offsets.Length; i++)
            {
                Blocks++;
                for (int at = offsets[i]; at < offsets[i] + sizes[i]; at += 8)
                {
                    if (!map.TryGetValue(at, out var field)) throw new Exception("block covers no field at " + at);
                    if (!field.FieldType.IsValueType) throw new Exception("block covers reference " + field.Name);
                    if (!DropBlocks && (DropAfter < 0 || Blocks <= DropAfter)) field.SetValue(to, field.GetValue(from));
                }
            }
        }

        public void CopyReferences(object from, object to, int[] offsets)
        {
            var map = Map(from.GetType());
            foreach (int offset in offsets)
            {
                References++;
                var field = map[offset];
                if (field.FieldType.IsValueType) throw new Exception("reference slot holds value " + field.Name);
                field.SetValue(to, field.GetValue(from));
            }
        }

        public object ReadReference(object target, int offset)
        {
            Reads++;
            var field = Map(target.GetType())[offset];
            if (field.FieldType.IsValueType) throw new Exception("reference read of value " + field.Name);
            return field.GetValue(target);
        }
    }

    /// <summary>Shaped like the game's key-frame buffer frame: a size and a list of reusable points.</summary>
    private sealed class Frame
    {
        public int Size;
        public List<Point> Data = new List<Point>();
    }

    private sealed class Point { public float X, Y; }

    private sealed class FramePolicy : ISnapshotPolicy
    {
        private static readonly SnapshotCodec Codec = new FrameCodec();
        public bool IsOpaque(Type type) => false;
        public bool Captures(FieldInfo field) => true;
        public bool NeedsFieldCopy(Type type) => false;
        public SnapshotCodec CodecFor(Type type) => type == typeof(Frame) ? Codec : null;
        public PropertyInfo[] ExtraProperties(Type type) => null;
    }

    /// <summary>Saves a frame whole: ints, object references and floats interleaved on the tape.</summary>
    private sealed class FrameCodec : SnapshotCodec
    {
        public override void Save(object target, SnapshotTape tape)
        {
            var frame = (Frame)target;
            tape.Int(frame.Size);
            tape.Object(frame.Data);
            tape.Int(frame.Data.Count);
            foreach (var point in frame.Data) { tape.Object(point); tape.Float(point.X); tape.Float(point.Y); }
        }

        public override void Load(object target, SnapshotTape tape)
        {
            var frame = (Frame)target;
            frame.Size = tape.ReadInt();
            frame.Data = (List<Point>)tape.ReadObject();
            int count = tape.ReadInt();
            if (frame.Data.Count > count) frame.Data.RemoveRange(count, frame.Data.Count - count);
            for (int i = 0; i < count; i++)
            {
                var point = (Point)tape.ReadObject();
                if (i < frame.Data.Count) frame.Data[i] = point; else frame.Data.Add(point);
                point.X = tape.ReadFloat();
                point.Y = tape.ReadFloat();
            }
        }
    }

    private static void CodecFramesRestoreWhole()
    {
        var frames = new List<Frame>();
        for (int f = 0; f < 3; f++)
        {
            var frame = new Frame { Size = f + 1 };
            for (int p = 0; p <= f; p++) frame.Data.Add(new Point { X = f, Y = p });
            frames.Add(frame);
        }
        var kept = frames[2].Data[1];
        var snapshotter = new ObjectGraphSnapshotter(new FramePolicy(), Raw);
        var a = new StateSnapshot();
        snapshotter.Capture(a, 0, new object[] { frames });
        // Grow, shrink, replace and edit, as a buffer reused by a longer or shorter move would.
        frames[0].Data.Add(new Point { X = 99 });
        frames[2].Data.RemoveAt(2);
        frames[2].Data[1] = new Point { X = -1 };
        frames[1].Size = 50;
        frames[1].Data[0].X = 42;
        frames.Add(new Frame());
        snapshotter.Restore(a);
        Check(frames.Count == 3 && frames[0].Data.Count == 1 && frames[2].Data.Count == 3, "frame buffer counts restored");
        Check(ReferenceEquals(frames[2].Data[1], kept) && kept.X == 2 && kept.Y == 1, "the same point objects restored in place");
        Check(frames[1].Size == 2 && frames[1].Data[0].X == 1, "frame sizes and values restored");
        var b = new StateSnapshot();
        snapshotter.Capture(b, 0, new object[] { frames });
        Check(snapshotter.FirstDifference(a, b) == null, "restored frames compare equal: " + snapshotter.FirstDifference(a, b));
        frames[1].Size = 7;
        snapshotter.Capture(b, 0, new object[] { frames });
        Check(snapshotter.FirstDifference(a, b)?.Contains("codec value") == true, "a changed codec integer is reported");
        Check(a.ObjectCount < 10, "points saved by the codec are not walked (" + a.ObjectCount + " objects)");
    }

    /// <summary>A raw plan that disagrees with reflection is retired, and the copy is still right.</summary>
    private static void RawLayoutMismatchFallsBack()
    {
        var warnings = new List<string>();
        var snapshotter = new ObjectGraphSnapshotter(new TestPolicy(), new FakeRawMemory { DropBlocks = true }) { Warn = warnings.Add };
        var root = new Node { X = 1.5f, Count = 2 };
        var snapshot = new StateSnapshot();
        snapshotter.Capture(snapshot, 0, new object[] { root });
        root.X = 5f; root.Count = 9;
        snapshotter.Restore(snapshot);
        Check(root.X == 1.5f && root.Count == 2, "a mismatched raw copy is repaired by reflection");
        Check(warnings.Count > 0 && warnings[0].Contains("Node"), "the mismatch is reported: " + (warnings.Count > 0 ? warnings[0] : "none"));
        root.X = 7f;
        snapshotter.Capture(snapshot, 1, new object[] { root });
        root.X = 8f;
        snapshotter.Restore(snapshot);
        Check(root.X == 7f, "the retired type keeps working through reflection");

        // A copy that goes wrong after the first one: only full verification can catch it.
        var late = new FakeRawMemory { DropAfter = 1 };
        var lateWarnings = new List<string>();
        var verifying = new ObjectGraphSnapshotter(new TestPolicy(), late) { Warn = lateWarnings.Add, VerifyEveryCopy = true };
        var node = new Node { X = 1f, Count = 1 };
        var slot = new StateSnapshot();
        verifying.Capture(slot, 0, new object[] { node });
        node.X = 2f; verifying.Restore(slot);
        node.X = 3f; verifying.Restore(slot);
        node.X = 4f; verifying.Restore(slot);
        Check(node.X == 1f && lateWarnings.Count > 0 && verifying.RawFallbacks > 0, "full verification catches a later bad copy and repairs it (x " + node.X + ")");
    }

    private static void SnapshotRestoresInPlace()
    {
        var shared = new Definition { Id = 7 };
        var root = new Node { X = 1.5f, Count = 2, Shared = shared };
        var child = new Node { X = 9f, Next = root };
        root.Children.Add(child);
        root.Next = child;
        root.Flags["hit"] = 3;
        root.Held = new Pair { Value = 4, Target = child };
        root.Boxed = new Pair { Value = 5, Target = child };
        Action callback = () => { };
        root.Callback = callback;
        var snapshotter = NewSnapshotter();
        var snapshot = new StateSnapshot();
        snapshotter.Capture(snapshot, 10, new object[] { root });
        Check(snapshot.Tick == 10 && snapshot.ObjectCount > 5, "snapshot saved the graph (" + snapshot.ObjectCount + " objects)");

        // Mutate everything a tick could touch.
        root.X = -3f; root.Count = 99; root.Label = "changed";
        child.X = 0f; child.Next = null;
        root.Children.Add(new Node { X = 42f });
        root.Children[0] = new Node();
        root.Flags["hit"] = 8; root.Flags["new"] = 1;
        root.Numbers[1] = 20;
        root.Numbers = new[] { 5 };
        root.Held = new Pair { Value = 6, Target = root };
        root.Callback = null;
        shared.Id = 8;
        root.Shared = new Definition();

        snapshotter.Restore(snapshot);
        Check(root.X == 1.5f && root.Count == 2, "scalars restored");
        Check(root.Label == "changed", "fields the policy skips are left alone");
        Check(ReferenceEquals(root.Next, child) && ReferenceEquals(child.Next, root) && child.X == 9f, "references and cycles restored in place");
        Check(root.Children.Count == 1 && ReferenceEquals(root.Children[0], child), "list contents restored");
        Check(root.Flags.Count == 1 && root.Flags["hit"] == 3 && !root.Flags.ContainsKey("new"), "dictionary restored");
        Check(root.Numbers.Length == 3 && root.Numbers[1] == 2, "array reference and contents restored");
        Check(root.Held.Value == 4 && ReferenceEquals(root.Held.Target, child), "struct field restored");
        Check(root.Boxed is Pair boxed && boxed.Value == 5, "boxed value restored");
        Check(ReferenceEquals(root.Callback, callback), "delegate reference restored");
        Check(ReferenceEquals(root.Shared, shared) && shared.Id == 8, "opaque objects keep identity and are not rolled back");

        // A second restore of the same snapshot is valid (rollbacks can repeat).
        root.Count = 50;
        snapshotter.Restore(snapshot);
        Check(root.Count == 2, "snapshot restores more than once");

        var statics = new[] { typeof(StaticHolder).GetField(nameof(StaticHolder.Value)), typeof(StaticHolder).GetField(nameof(StaticHolder.List)) };
        StaticHolder.Value = 1;
        StaticHolder.List.Clear();
        StaticHolder.List.Add(1);
        snapshotter.Capture(snapshot, 11, new object[0], statics);
        StaticHolder.Value = 2;
        StaticHolder.List.Add(2);
        snapshotter.Restore(snapshot);
        Check(StaticHolder.Value == 1 && StaticHolder.List.Count == 1, "static roots restored");
    }

    private static class StaticHolder
    {
        public static int Value;
        public static readonly List<int> List = new List<int>();
    }

    private sealed class Bag
    {
        public HashSet<string> Set = new HashSet<string> { "a" };
        public Queue<int> Queue = new Queue<int>(new[] { 1, 2 });
        public LinkedList<Node> Linked = new LinkedList<Node>();
        public Dictionary<string, List<Node>> Table = new Dictionary<string, List<Node>>();
        public KeyValuePair<string, Node>[] Pairs = new KeyValuePair<string, Node>[2];
        public List<Pair> Structs = new List<Pair>();
        public WeakReference Weak = new WeakReference(new object());
        public int? Maybe = 3;
    }

    private static void SnapshotCollections()
    {
        var shared = new Node { X = 1 };
        var bag = new Bag();
        bag.Linked.AddLast(shared);
        bag.Table["k"] = new List<Node> { shared };
        bag.Pairs[0] = new KeyValuePair<string, Node>("p", shared);
        bag.Structs.Add(new Pair { Value = 1, Target = shared });
        var weak = bag.Weak;
        var snapshotter = NewSnapshotter();
        var snapshot = new StateSnapshot();
        snapshotter.Capture(snapshot, 0, new object[] { bag });

        bag.Set.Add("b"); bag.Set.Remove("a");
        bag.Queue.Dequeue(); bag.Queue.Enqueue(9);
        bag.Linked.AddFirst(new Node());
        bag.Table["k"].Clear(); bag.Table["x"] = new List<Node>();
        bag.Pairs[0] = default;
        bag.Structs[0] = new Pair { Value = 2 };
        shared.X = 7;
        bag.Maybe = null;
        bag.Weak = null;

        snapshotter.Restore(snapshot);
        Check(bag.Set.Count == 1 && bag.Set.Contains("a"), "hash set restored");
        Check(bag.Queue.Count == 2 && bag.Queue.Peek() == 1, "queue restored");
        Check(bag.Linked.Count == 1 && ReferenceEquals(bag.Linked.First.Value, shared), "linked list restored");
        Check(bag.Table.Count == 1 && bag.Table["k"].Count == 1 && ReferenceEquals(bag.Table["k"][0], shared), "nested dictionary restored");
        Check(bag.Pairs[0].Key == "p" && ReferenceEquals(bag.Pairs[0].Value, shared), "struct array with references restored");
        Check(bag.Structs[0].Value == 1 && ReferenceEquals(bag.Structs[0].Target, shared), "list of structs restored");
        Check(shared.X == 1, "objects reached through collections restored");
        Check(bag.Maybe == 3, "nullable restored");
        Check(ReferenceEquals(bag.Weak, weak) && weak.IsAlive | !weak.IsAlive, "weak reference kept by reference, never cloned");
    }

    private static void SnapshotReportsDifferences()
    {
        var root = new Node { X = 1f, Count = 1 };
        var snapshotter = NewSnapshotter();
        var a = new StateSnapshot();
        var b = new StateSnapshot();
        snapshotter.Capture(a, 0, new object[] { root });
        snapshotter.Capture(b, 0, new object[] { root });
        Check(snapshotter.FirstDifference(a, b) == null, "identical snapshots compare equal");
        root.Count = 2;
        snapshotter.Capture(b, 0, new object[] { root });
        string difference = snapshotter.FirstDifference(a, b);
        Check(difference != null && difference.Contains("Node.Count"), "difference names the field: " + difference);
        // Fresh objects with the same contents (a tick that allocates) still compare equal.
        root.Count = 1;
        root.Next = new Node { X = 4f };
        snapshotter.Capture(a, 0, new object[] { root });
        root.Next = new Node { X = 4f };
        snapshotter.Capture(b, 0, new object[] { root });
        Check(snapshotter.FirstDifference(a, b) == null, "reallocated but equal state compares equal: " + snapshotter.FirstDifference(a, b));
        root.Next = new Node { X = 5f };
        snapshotter.Capture(b, 0, new object[] { root });
        Check(snapshotter.FirstDifference(a, b)?.Contains("Node.X") == true, "a changed value inside a new object is found");
        var other = new Node();
        root.Next = other;
        root.Children.Add(other);
        snapshotter.Capture(b, 0, new object[] { root });
        Check(snapshotter.FirstDifference(a, b) != null, "a changed reference structure is found");
    }

    // ---- Timeline ----

    private static void RunLengthInputs()
    {
        var a = new InputTimeline(1, 0, 8);
        var b = new InputTimeline(1, 0, 8);
        for (int i = 0; i < 600; i++) a.AddLocal(i < 400 ? (byte)3 : (byte)(i % 2 == 0 ? NetInput.Punch : 0));
        var writer = new NetWriter();
        a.WriteSync(writer);
        Check(writer.Length < 40 + 2 * 60, "held input packs into runs (" + writer.Length + " bytes)");
        for (int round = 0; round < 10 && b.RemoteFrames < 600; round++)
        {
            writer = new NetWriter();
            a.WriteSync(writer);
            b.ReadSync(new NetReader(writer.ToArray()));
            writer = new NetWriter();
            b.WriteSync(writer);
            a.ReadSync(new NetReader(writer.ToArray()));
        }
        Check(b.RemoteFrames == 600, "every input arrives across packets (" + b.RemoteFrames + ")");
        bool same = true;
        for (int i = 0; i < 600; i++) same &= b.GetRemote(i) == a.GetLocal(i);
        Check(same, "run-length inputs decode exactly");
        bool threw = false;
        var bad = new NetWriter();
        bad.U8(1); bad.I32(0); bad.I32(0); bad.U16(0); bad.I32(0); bad.U8(1); bad.U8(3); bad.U8(0); bad.I32(-1); bad.U32(0);
        try { new InputTimeline(1, 0, 8).ReadSync(new NetReader(bad.ToArray())); } catch (NetFormatException) { threw = true; }
        Check(threw, "zero-length run rejected");
    }

    private static void PredictionAndRollbackBookkeeping()
    {
        var t = new InputTimeline(0, 1, 4);
        for (int i = 0; i < 8; i++) t.AddLocal(0);
        // Remote tick 0 is the neutral opening delay; ticks 1-4 are predicted, 5 exceeds the window.
        for (int tick = 0; tick < 5; tick++)
        {
            Check(t.TryGetInputs(tick, out _, out var remote, out var predicted), "tick " + tick + " may run");
            Check(predicted == (tick >= 1), "tick " + tick + " predicted " + predicted);
            t.MarkSimulated(tick, remote, predicted);
        }
        Check(!t.TryGetInputs(5, out _, out _, out _), "prediction window caps how far ahead a peer runs");
        Check(t.FinalTicks == 1, "only confirmed ticks are final (" + t.FinalTicks + ")");
        // The opponent really held neutral for ticks 1-2, then pressed punch on tick 3.
        var remoteSide = new InputTimeline(0, 1, 4);
        remoteSide.AddLocal(0); remoteSide.AddLocal(0); remoteSide.AddLocal(NetInput.Punch);
        var writer = new NetWriter();
        remoteSide.WriteSync(writer);
        t.ReadSync(new NetReader(writer.ToArray()));
        Check(t.PendingRollback == 3, "wrong guess found at tick 3 (" + t.PendingRollback + ")");
        Check(t.FinalTicks == 3, "ticks before the wrong guess are final (" + t.FinalTicks + ")");
        t.BeginResimulation(3);
        Check(t.IsResimulating && t.PendingRollback < 0, "re-simulation begins");
        Check(t.TryGetInputs(3, out _, out var fixedRemote, out var stillPredicted) && fixedRemote == NetInput.Punch && !stillPredicted, "re-simulation reads the real input");
        t.MarkSimulated(3, fixedRemote, false);
        Check(t.TryGetInputs(4, out _, out var guess, out var predicted4) && predicted4 && guess == NetInput.Punch, "new guesses repeat the latest real input");
        t.MarkSimulated(4, guess, true);
        Check(!t.IsResimulating && t.Rollbacks == 1 && t.LongestRollback == 2, "rollback statistics");
    }

    // ---- In-memory rollback matches ----

    /// <summary>
    /// A deterministic toy fight whose state lives in an object graph. Barrier ticks
    /// stand in for round transitions that must never run speculatively.
    /// </summary>
    private sealed class ToyFight : IRollbackGame
    {
        public sealed class Fighter { public int Position; public int Health = 1000; public List<int> Combo = new List<int>(); }
        public sealed class State { public Fighter Left = new Fighter(), Right = new Fighter(); public int Tick; public List<Fighter> Projectiles = new List<Fighter>(); }

        public readonly State World = new State();
        public readonly List<(int tick, byte left, byte right)> FinalInputs = new List<(int, byte, byte)>();
        public int BarrierEvery = 97;
        public int Speculated, Resimulated, BarrierReplays;
        private readonly ObjectGraphSnapshotter _snapshotter = NewSnapshotter();
        private readonly StateSnapshot[] _ring;
        private readonly InputTimeline _timeline;

        public ToyFight(InputTimeline timeline)
        {
            _timeline = timeline;
            _ring = new StateSnapshot[timeline.MaxPrediction + 2];
            for (int i = 0; i < _ring.Length; i++) _ring[i] = new StateSnapshot();
        }

        public bool CanSpeculate => World.Tick % BarrierEvery != BarrierEvery - 1;

        public void SaveState(int tick) => _snapshotter.Capture(_ring[tick % _ring.Length], tick, new object[] { World });

        public bool LoadState(int tick)
        {
            var snapshot = _ring[tick % _ring.Length];
            if (snapshot.Tick != tick) return false;
            _snapshotter.Restore(snapshot);
            return World.Tick == tick;
        }

        public bool Simulate(int tick, byte left, byte right, TickFlags flags, out uint hash)
        {
            hash = 0;
            if (World.Tick != tick) throw new Exception("simulated tick " + tick + " on state " + World.Tick);
            if ((flags & TickFlags.Speculative) != 0 && tick % 50 == 49) return false;
            if ((flags & TickFlags.Speculative) != 0) Speculated++;
            if ((flags & TickFlags.Resimulating) != 0) Resimulated++;
            if ((flags & TickFlags.BarrierReplay) != 0) BarrierReplays++;
            Step(World.Left, left, World.Right);
            Step(World.Right, right, World.Left);
            if ((left & NetInput.Ranged) != 0) World.Projectiles.Add(new Fighter { Position = World.Left.Position });
            if (World.Projectiles.Count > 3) World.Projectiles.RemoveAt(0);
            foreach (var projectile in World.Projectiles) projectile.Position += 3;
            World.Tick++;
            var hasher = new StateHasher();
            hasher.Add(World.Tick); hasher.Add(World.Left.Position); hasher.Add(World.Right.Position);
            hasher.Add(World.Left.Health); hasher.Add(World.Right.Health); hasher.Add(World.Left.Combo.Count); hasher.Add(World.Projectiles.Count);
            foreach (var projectile in World.Projectiles) hasher.Add(projectile.Position);
            hash = hasher.Value;
            return true;
        }

        private static void Step(Fighter self, byte input, Fighter other)
        {
            int direction = NetInput.Direction(input);
            if (direction == 2) self.Position += 2;
            if (direction == 6) self.Position -= 2;
            if ((input & NetInput.Punch) != 0) { other.Health -= 3 + self.Combo.Count; self.Combo.Add(1); }
            else if (self.Combo.Count > 0 && (input & NetInput.Kick) == 0) self.Combo.Clear();
        }

        /// <summary>Copies inputs of newly final ticks, as the replay recorder does.</summary>
        public void RecordFinal()
        {
            for (int tick = FinalInputs.Count; tick < _timeline.FinalTicks; tick++)
                FinalInputs.Add((tick, _timeline.GetLocal(tick), _timeline.GetRemote(tick)));
        }
    }

    private static byte BusyInput(int side, int tick)
    {
        // Changes often, so predictions are regularly wrong.
        int phase = (tick * (side + 3) / 4 + side * 5) % 13;
        byte input = NetInput.WithDirection(0, phase % 3 == 0 ? 2 : phase % 3 == 1 ? 6 : 0);
        if (phase % 4 == 1) input |= NetInput.Punch;
        if (phase % 6 == 2) input |= NetInput.Kick;
        if (phase == 7) input |= NetInput.Ranged;
        return input;
    }

    private static void RollbackInMemory(int latencySteps, int lossPercent, int delay, int window, int seed)
    {
        const int ticks = 1200;
        var random = new Random(seed);
        var timelines = new[] { new InputTimeline(2, delay, window), new InputTimeline(2, delay, window) };
        var fights = new[] { new ToyFight(timelines[0]), new ToyFight(timelines[1]) };
        var runners = new[] { new RollbackRunner(timelines[0], fights[0], 0), new RollbackRunner(timelines[1], fights[1], 1) };
        var inFlight = new[] { new List<(int due, byte[] data)>(), new List<(int due, byte[] data)>() };
        int stalls = 0;
        for (int step = 0; step < ticks * 10 && (runners[0].Tick < ticks || runners[1].Tick < ticks || timelines[0].FinalTicks < ticks || timelines[1].FinalTicks < ticks); step++)
        {
            for (int side = 0; side < 2; side++)
            {
                var timeline = timelines[side];
                var runner = runners[side];
                // The guest's clock runs a little slow, so it keeps falling behind.
                if (side == 1 && random.Next(9) == 0) continue;
                if (runner.Tick < ticks)
                {
                    if (timeline.NeedsLocalInput(runner.Tick)) timeline.AddLocal(BusyInput(side, runner.Tick + delay));
                    if (!runner.Advance()) stalls++;
                }
                else runner.Resolve();
                Check(runner.Failure == null, "runner failure: " + runner.Failure);
                fights[side].RecordFinal();
                var writer = new NetWriter();
                timeline.WriteSync(writer, step * 16, latencySteps * 32);
                if (random.Next(100) >= lossPercent) inFlight[side].Add((step + latencySteps + random.Next(2), writer.ToArray()));
            }
            for (int side = 0; side < 2; side++)
            {
                var queue = inFlight[side];
                for (int i = 0; i < queue.Count; i++)
                {
                    if (queue[i].due > step) continue;
                    timelines[1 - side].ReadSync(new NetReader(queue[i].data), step * 16, latencySteps * 32);
                    queue.RemoveAt(i--);
                }
            }
        }
        string label = " (latency " + latencySteps + ", loss " + lossPercent + "%, delay " + delay + ")";
        Check(runners[0].Tick == ticks && runners[1].Tick == ticks, "rollback match finishes" + label + ": " + runners[0].Tick + "/" + runners[1].Tick);
        Check(timelines[0].FinalTicks == ticks && timelines[1].FinalTicks == ticks, "every tick becomes final" + label);
        Check(timelines[0].DesyncTick < 0 && timelines[1].DesyncTick < 0, "no desync after rollbacks" + label + " at " + timelines[0].DesyncTick + "/" + timelines[1].DesyncTick + " rollbacks " + timelines[0].Rollbacks + " barriers " + runners[0].Barriers + " final " + timelines[0].FinalTicks + " verified " + timelines[0].VerifiedTick);
        Check(timelines[0].VerifiedTick > ticks - 60, "hashes verified to the end" + label + " (" + timelines[0].VerifiedTick + ")");
        Check(fights[0].World.Left.Health == fights[1].World.Left.Health && fights[0].World.Right.Position == fights[1].World.Right.Position &&
            fights[0].World.Projectiles.Count == fights[1].World.Projectiles.Count, "final states identical" + label);
        for (int tick = 0; tick < ticks; tick++)
        {
            var a = fights[0].FinalInputs[tick];
            var b = fights[1].FinalInputs[tick];
            // Host local is left; guest local is right.
            Check(a.left == b.right && a.right == b.left, "final inputs agree at tick " + tick + label);
            if (tick >= delay) Check(a.left == BusyInput(0, tick) && b.left == BusyInput(1, tick), "inputs land on their tick " + tick + label);
        }
        if (latencySteps > delay + 1)
        {
            Check(timelines[0].Rollbacks > 20 && fights[0].Resimulated > 0, "mispredictions were rolled back" + label + " (" + timelines[0].Rollbacks + ")");
            Check(runners[0].Barriers > 0, "barriers held speculation back" + label);
            Check(fights[0].BarrierReplays > 0 && fights[0].BarrierReplays <= runners[0].Barriers, "correctly guessed barrier ticks replay quietly" + label);
        }
        Check(timelines[0].LongestRollback <= window && timelines[1].LongestRollback <= window, "rollbacks stay inside the window" + label);
    }

    private static void TimeSyncSlowsTheLeader()
    {
        // The host starts 20 ticks early; time sync must make it wait for the guest.
        var host = new InputTimeline(3, 1, 8);
        var guest = new InputTimeline(3, 1, 8);
        var hostFight = new ToyFight(host) { BarrierEvery = 100000 };
        var guestFight = new ToyFight(guest) { BarrierEvery = 100000 };
        var hostRunner = new RollbackRunner(host, hostFight, 0);
        var guestRunner = new RollbackRunner(guest, guestFight, 1);
        var toGuest = new Queue<(int due, byte[] data)>();
        var toHost = new Queue<(int due, byte[] data)>();
        const int latency = 3;
        for (int step = 0; step < 1500; step++)
        {
            long now = step * 16L;
            if (step >= 0) Step(host, hostRunner, 0);
            if (step >= 20) Step(guest, guestRunner, 1);
            var writer = new NetWriter();
            host.WriteSync(writer, now, latency * 2 * 16);
            toGuest.Enqueue((step + latency, writer.ToArray()));
            writer = new NetWriter();
            guest.WriteSync(writer, now, latency * 2 * 16);
            toHost.Enqueue((step + latency, writer.ToArray()));
            while (toGuest.Count > 0 && toGuest.Peek().due <= step) guest.ReadSync(new NetReader(toGuest.Dequeue().data), now, latency * 2 * 16);
            while (toHost.Count > 0 && toHost.Peek().due <= step) host.ReadSync(new NetReader(toHost.Dequeue().data), now, latency * 2 * 16);
        }
        int gap = hostRunner.Tick - guestRunner.Tick;
        Check(hostRunner.Waits >= 1 && hostRunner.Waits > guestRunner.Waits, "the leading peer waited (" + hostRunner.Waits + " waits)");
        Check(Math.Abs(gap) <= 3, "peers converge on the same tick (gap " + gap + ")");
        Check(host.DesyncTick < 0 && guest.DesyncTick < 0, "time sync keeps the match in sync");

        static void Step(InputTimeline timeline, RollbackRunner runner, int side)
        {
            if (runner.ShouldWait()) return;
            for (int i = 0; i < runner.StepsWanted; i++)
            {
                if (timeline.NeedsLocalInput(runner.Tick)) timeline.AddLocal(BusyInput(side, runner.Tick + timeline.Delay));
                if (!runner.Advance()) break;
            }
        }
    }

    // ---- Real UDP ----

    private static readonly Stopwatch Clock = Stopwatch.StartNew();
    private static long Now => Clock.ElapsedMilliseconds;

    private static void RollbackOverUdp(int latencyMs, int lossPercent)
    {
        var identity = new NetIdentity("1.0/IL2CPP", "mods:none", "A");
        using (var host = NetplayPeer.Host(0, identity, Now))
        using (var guest = NetplayPeer.Join(new IPEndPoint(IPAddress.Loopback, host.LocalPort), identity, Now))
        {
            host.SimulatedLatencyMs = guest.SimulatedLatencyMs = latencyMs;
            host.SimulatedLossPercent = guest.SimulatedLossPercent = lossPercent;
            long connectEnd = Now + 5000;
            while ((host.State != NetplayState.Connected || guest.State != NetplayState.Connected) && Now < connectEnd)
            {
                host.Update(Now); guest.Update(Now); Thread.Sleep(1);
            }
            Check(host.State == NetplayState.Connected && guest.State == NetplayState.Connected, "rollback peers connect");
            const int ticks = 600;
            var timelines = new[] { new InputTimeline(9, 1, 8), new InputTimeline(9, 1, 8) };
            var fights = new[] { new ToyFight(timelines[0]), new ToyFight(timelines[1]) };
            var runners = new[] { new RollbackRunner(timelines[0], fights[0], 0), new RollbackRunner(timelines[1], fights[1], 1) };
            host.Timeline = timelines[0];
            guest.Timeline = timelines[1];
            var peers = new[] { host, guest };
            long startMs = Now;
            var due = new long[2];
            long end = Now + 40000;
            while ((timelines[0].FinalTicks < ticks || timelines[1].FinalTicks < ticks) && Now < end)
            {
                for (int side = 0; side < 2; side++)
                {
                    peers[side].Update(Now);
                    long target = (Now - startMs) * 60 / 1000;
                    int steps = 0;
                    while (due[side] < target && steps++ < 4)
                    {
                        due[side]++;
                        var runner = runners[side];
                        if (runner.Tick >= ticks) { runner.Resolve(); continue; }
                        if (runner.ShouldWait()) continue;
                        for (int i = 0; i < runner.StepsWanted && runner.Tick < ticks; i++)
                        {
                            if (timelines[side].NeedsLocalInput(runner.Tick)) timelines[side].AddLocal(BusyInput(side, runner.Tick + 1));
                            if (!runner.Advance()) break;
                        }
                    }
                    peers[side].Flush(Now);
                }
                Thread.Sleep(1);
            }
            Check(timelines[0].FinalTicks >= ticks && timelines[1].FinalTicks >= ticks, "UDP rollback match finishes: " + timelines[0].FinalTicks + "/" + timelines[1].FinalTicks);
            Check(timelines[0].DesyncTick < 0 && timelines[1].DesyncTick < 0, "UDP rollback stays in sync");
            Check(fights[0].World.Left.Health == fights[1].World.Left.Health && fights[0].World.Right.Health == fights[1].World.Right.Health, "UDP rollback final state identical");
            Check(timelines[0].Rollbacks > 0, "UDP latency caused rollbacks (" + timelines[0].Rollbacks + ")");
            Check(host.RttMs >= latencyMs && host.JitterMs >= 0, "round trip and jitter measured (" + host.RttMs + " ms, jitter " + host.JitterMs + ")");
            Check(host.LossPercent <= lossPercent + 15, "loss estimate plausible (" + host.LossPercent + "%)");
        }
    }

    private static void SuggestedDelays()
    {
        Check(NetcodeModes.SuggestedDelay(NetcodeMode.Rollback, 20) == 1, "LAN rollback uses one tick");
        Check(NetcodeModes.SuggestedDelay(NetcodeMode.Rollback, 50) == 1, "nearby players keep one tick");
        Check(NetcodeModes.SuggestedDelay(NetcodeMode.Rollback, 80) == 2, "a cross-country trip hides all but a tick");
        Check(NetcodeModes.SuggestedDelay(NetcodeMode.Rollback, 250) >= 2, "long trips add a little delay under rollback");
        Check(NetcodeModes.SuggestedDelay(NetcodeMode.Rollback, 2000) <= 4, "rollback delay stays small");
        Check(NetcodeModes.SuggestedDelay(NetcodeMode.Delay, 100) == 4, "delay mode hides the one-way trip");
        Check(NetcodeModes.SuggestedDelay(NetcodeMode.Delay, -1) == NetProtocol.DefaultInputDelay, "delay mode default before a ping");
    }
}
