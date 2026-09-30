using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Eclipse.Multiplayer.Online;
using Nekki.SF2.GUI.Fight;

namespace Eclipse.Multiplayer.Rollback
{
    /// <summary>
    /// What a versus rollback snapshot saves. The fight's managed object graph is saved
    /// in full, with three kinds of exceptions:
    /// <list type="bullet">
    /// <item>shared, load-time definitions (moves, triggers, fight setup), which a fight never changes;</item>
    /// <item>presentation (effects, blood, audio, mod visuals), which only runs on a tick's first simulation;</item>
    /// <item>Unity components, except the fight UI that holds gameplay state (round timer, style meter, banners).</item>
    /// </list>
    /// </summary>
    internal sealed class FightSnapshotPolicy : ISnapshotPolicy
    {
        private static readonly HashSet<Type> GameplayComponents = new HashSet<Type>
        {
            // Round timer and the style meter that feeds style perks.
            typeof(ViewerFight), typeof(StylePanel), typeof(StyleBar), typeof(StyleBarStrip),
            // Banner timers that drive round flow.
            typeof(PreFight), typeof(ScreenFight),
        };

        private static readonly Type[] OpaqueBases =
        {
            // Parsed move and trigger definitions, shared by every fight.
            typeof(InfoAnimation), typeof(Trigger), typeof(EventAnimation), typeof(ConditionAnimation), typeof(IntervalAnimation),
            typeof(TransitionAnimation), typeof(ActionAnimation), typeof(GroupTables), typeof(ModelShiftTable), typeof(CocosAnimationData),
            typeof(ModelLoader), typeof(AiData), typeof(GameUtils.HitEffect), typeof(GameUtils.HitEffects),
            // The matchup and arena.
            typeof(FightList), typeof(Location),
            // Presentation that a re-simulation deliberately does not repeat.
            typeof(EffectsContainer), typeof(EffectsRunning), typeof(CurrentEffect), typeof(BloodEffect), typeof(ChangingSprite),
        };

        private static readonly string[] OpaqueNamespaces =
        {
            "UnityEngine", "System.Threading", "System.IO", "System.Reflection", "System.Globalization", "System.Xml", "System.Net",
            "Eclipse.Modding", "Eclipse.Rendering", "Eclipse.UI", "Eclipse.Diagnostics", "Eclipse.Input", "Eclipse.Multiplayer.Online",
            "Nekki.Audio", "DG.Tweening", "MoonSharp", "Newtonsoft", "CodeStage.AntiCheat.Detectors",
        };

        // Fields that hold presentation: restoring them would orphan scene objects.
        private static readonly HashSet<(Type, string)> SkippedFields = new HashSet<(Type, string)>
        {
            (typeof(Model), "BJKJBIMPPAM"),   // Effects attached to the model.
            (typeof(Render), "FPLGNMICCPH"),  // Blood drops,
            (typeof(Render), "OBCAJAIBJHP"),  // and their lifetime counter.
            // The playing hit-effect animation. Only a tick's first run sets it, so a
            // restored null would strand the effect object switched on.
            (typeof(Render), "PHKBOGAICCI"),
        };

        private static readonly PropertyInfo FillAmount = typeof(UnityEngine.UI.Image).GetProperty("fillAmount");
        private static readonly SnapshotCodec Vectors = new VectorCodec();

        public bool IsOpaque(Type type)
        {
            // Arrays are judged by their elements (a Vector2[] is state, not engine data).
            if (type.IsArray) return false;
            if (typeof(UnityEngine.Object).IsAssignableFrom(type)) return !GameplayComponents.Contains(type);
            foreach (var definition in OpaqueBases)
                if (definition.IsAssignableFrom(type)) return true;
            // Nested helper types of the definitions (MoveInside, TriggerInside...).
            for (var outer = type.DeclaringType; outer != null; outer = outer.DeclaringType)
                if (outer == typeof(InfoAnimation) || outer == typeof(Trigger)) return true;
            string space = type.Namespace;
            if (space != null)
                foreach (var prefix in OpaqueNamespaces)
                    if (space == prefix || space.StartsWith(prefix + ".", StringComparison.Ordinal)) return true;
            return false;
        }

        public bool Captures(FieldInfo field)
        {
            var owner = field.DeclaringType;
            if (owner == null) return false;
            // Engine-side fields of components (UnityEngine.Object, Image internals) are not simulation state.
            if (typeof(UnityEngine.Object).IsAssignableFrom(owner) && IsEngineType(owner)) return false;
            return !SkippedFields.Contains((owner, field.Name));
        }

        public bool NeedsFieldCopy(Type type) => typeof(UnityEngine.Object).IsAssignableFrom(type);

        public SnapshotCodec CodecFor(Type type) =>
            type == typeof(ModelNode) || type == typeof(ModelMacroNode) ? Nodes :
            type == typeof(Vector3f) || type == typeof(Vector2f) ? Vectors :
            type == typeof(Vector3f[]) ? VectorArrays : type == typeof(KeyFrames.Frame[]) ? Frames :
            type == typeof(Pair<ModelNode, float>[]) ? NodeWeights : type == typeof(Triangle[]) ? Triangles : null;

        private static readonly SnapshotCodec Frames = new KeyFrameArrayCodec();
        private static readonly SnapshotCodec VectorArrays = new VectorArrayCodec();
        private static readonly SnapshotCodec NodeWeights = new NodeWeightArrayCodec();
        private static readonly SnapshotCodec Triangles = new TriangleArrayCodec();
        private static readonly SnapshotCodec Nodes = new ModelNodeCodec();

        private sealed class ModelNodeCodec : SnapshotCodec
        {
            public override void Save(object target, SnapshotTape tape)
            {
                ((ModelNode)target).SaveRollbackNode(tape);
                if (target is ModelMacroNode macro) macro.SaveRollbackMacro(tape);
            }

            public override void Load(object target, SnapshotTape tape)
            {
                ((ModelNode)target).LoadRollbackNode(tape);
                if (target is ModelMacroNode macro) macro.LoadRollbackMacro(tape);
            }

            public override void Walk(object target, List<object> into)
            {
                ((ModelNode)target).WalkRollbackNode(into);
                if (target is ModelMacroNode macro) macro.WalkRollbackMacro(into);
            }
        }

        // These are mutable state, not opaque topology. Save every slot (including
        // spare capacity), original object/array identity and all fields; walk the
        // referenced nodes normally. A derived element also takes the generic path
        // so a mod/reconstruction extension cannot lose its extra fields.
        private sealed class NodeWeightArrayCodec : SnapshotCodec
        {
            public override void Save(object target, SnapshotTape tape)
            {
                foreach (var pair in (Pair<ModelNode, float>[])target)
                {
                    tape.Object(pair);
                    if (pair == null) continue;
                    tape.Object(pair.First);
                    tape.Float(pair.Second);
                }
            }

            public override void Load(object target, SnapshotTape tape)
            {
                var array = (Pair<ModelNode, float>[])target;
                for (int i = 0; i < array.Length; i++)
                {
                    var pair = (Pair<ModelNode, float>)tape.ReadObject();
                    array[i] = pair;
                    if (pair == null) continue;
                    pair.First = (ModelNode)tape.ReadObject();
                    pair.Second = tape.ReadFloat();
                }
            }

            public override void Walk(object target, List<object> into)
            {
                foreach (var pair in (Pair<ModelNode, float>[])target)
                {
                    if (pair == null) continue;
                    into.Add(pair.First);
                    if (pair.GetType() != typeof(Pair<ModelNode, float>)) into.Add(pair);
                }
            }
        }

        private sealed class TriangleArrayCodec : SnapshotCodec
        {
            public override void Save(object target, SnapshotTape tape)
            {
                foreach (var triangle in (Triangle[])target)
                {
                    tape.Object(triangle);
                    if (triangle == null) continue;
                    tape.Object(triangle.get_Name());
                    var nodes = triangle.Nodes;
                    tape.Object(nodes);
                    if (nodes != null)
                        for (int n = 0; n < nodes.Length; n++) tape.Object(nodes[n]);
                }
            }

            public override void Load(object target, SnapshotTape tape)
            {
                var array = (Triangle[])target;
                for (int i = 0; i < array.Length; i++)
                {
                    var triangle = (Triangle)tape.ReadObject();
                    array[i] = triangle;
                    if (triangle == null) continue;
                    triangle.set_Name((string)tape.ReadObject());
                    var nodes = (ModelNode[])tape.ReadObject();
                    triangle.Nodes = nodes;
                    if (nodes != null)
                        for (int n = 0; n < nodes.Length; n++) nodes[n] = (ModelNode)tape.ReadObject();
                }
            }

            public override void Walk(object target, List<object> into)
            {
                foreach (var triangle in (Triangle[])target)
                {
                    if (triangle == null) continue;
                    if (triangle.Nodes != null) into.AddRange(triangle.Nodes);
                    if (triangle.GetType() != typeof(Triangle)) into.Add(triangle);
                }
            }
        }

        /// <summary>
        /// Animation interpolation buffers contain thousands of points in vector lists.
        /// Save their backing arrays in one pass instead of hashing and visiting every
        /// point as a graph node. Lists themselves still retain their exact backing array,
        /// count and version through the ordinary snapshot path. Save the full capacity:
        /// unused slots can contain points reused by a later animation.
        /// </summary>
        private sealed class VectorArrayCodec : SnapshotCodec
        {
            public override void Save(object target, SnapshotTape tape)
            {
                var array = (Vector3f[])target;
                for (int i = 0; i < array.Length; i++)
                {
                    var vector = array[i];
                    tape.Object(vector);
                    if (vector == null) continue;
                    tape.Float3(vector.GetX(), vector.GetY(), vector.GetZ());
                }
            }

            public override void Load(object target, SnapshotTape tape)
            {
                var array = (Vector3f[])target;
                for (int i = 0; i < array.Length; i++)
                {
                    var vector = (Vector3f)tape.ReadObject();
                    array[i] = vector;
                    if (vector == null) continue;
                    tape.ReadFloat3(out float x, out float y, out float z);
                    vector.Set(x, y, z);
                }
            }
        }

        public PropertyInfo[] ExtraProperties(Type type)
        {
            // The style strips keep their value in the image fill.
            if (FillAmount != null && typeof(UnityEngine.UI.Image).IsAssignableFrom(type)) return new[] { FillAmount };
            return null;
        }

        private static bool IsEngineType(Type type)
        {
            string space = type.Namespace ?? string.Empty;
            return space == "UnityEngine" || space.StartsWith("UnityEngine.", StringComparison.Ordinal);
        }

        /// <summary>
        /// Batch frame headers, then visit their lists normally. The vector-array
        /// codec batches the points. Keeping the list's normal snapshot preserves its
        /// backing array, unused capacity and version; assigning through its indexer
        /// during restore used to change the version on every rollback.
        /// </summary>
        private sealed class KeyFrameArrayCodec : SnapshotCodec
        {
            public override void Save(object target, SnapshotTape tape)
            {
                foreach (var frame in (KeyFrames.Frame[])target)
                {
                    tape.Object(frame);
                    if (frame == null) continue;
                    tape.Int(frame.Size);
                    tape.Object(frame.Data);
                }
            }

            public override void Load(object target, SnapshotTape tape)
            {
                var array = (KeyFrames.Frame[])target;
                for (int i = 0; i < array.Length; i++)
                {
                    var frame = (KeyFrames.Frame)tape.ReadObject();
                    array[i] = frame;
                    if (frame == null) continue;
                    frame.Size = tape.ReadInt();
                    frame.Data = (List<Vector3f>)tape.ReadObject();
                }
            }

            public override void Walk(object target, List<object> into)
            {
                foreach (var frame in (KeyFrames.Frame[])target)
                {
                    if (frame == null) continue;
                    into.Add(frame.Data);
                    if (frame.GetType() != typeof(KeyFrames.Frame)) into.Add(frame);
                }
            }
        }

        /// <summary>Fighter skeletons and key frames hold thousands of vectors; save them without reflection.</summary>
        private sealed class VectorCodec : SnapshotCodec
        {
            public override void Save(object target, SnapshotTape tape)
            {
                var vector = (Vector2f)target;
                tape.Float(vector.GetX());
                tape.Float(vector.GetY());
                if (vector is Vector3f vector3) tape.Float(vector3.GetZ());
            }

            public override void Load(object target, SnapshotTape tape)
            {
                var vector = (Vector2f)target;
                vector.SetX(tape.ReadFloat());
                vector.SetY(tape.ReadFloat());
                if (vector is Vector3f vector3) vector3.SetZ(tape.ReadFloat());
            }
        }
    }

    /// <summary>
    /// Rollback for a versus <see cref="Fight"/>: a ring of full-state snapshots, one per
    /// speculative tick, and single-tick simulation through <see cref="VersusTickDriver"/>.
    /// </summary>
    internal sealed class FightRollback : IRollbackGame
    {
        private static readonly FieldInfo[] StaticState = FindStatics();
        private readonly Fight _fight;
        private readonly ObjectGraphSnapshotter _snapshotter = CreateSnapshotter();
        private readonly StateSnapshot[] _ring;
        private readonly object[] _roots = new object[2];
        private readonly Stopwatch _watch = new Stopwatch();

        public FightRollback(Fight fight, int window)
        {
            _fight = fight ?? throw new ArgumentNullException(nameof(fight));
            _ring = new StateSnapshot[Math.Max(1, window) + 2];
            for (int i = 0; i < _ring.Length; i++) _ring[i] = new StateSnapshot();
        }

        public int Saves { get; private set; }
        public int Loads { get; private set; }
        public double LastSaveMs { get; private set; }
        public double AverageSaveMs { get; private set; }
        public double MaxSaveMs { get; private set; }
        public double LastLoadMs { get; private set; }
        public double AverageLoadMs { get; private set; }
        public int ObjectCount => _snapshotter.LastObjectCount;
        private StateSnapshot _latest;
        private Dictionary<Type, int> _firstCensus;

        /// <summary>The object types the saved state gained most since the first snapshot.</summary>
        public string GrowthSinceFirstSave(int top = 8) =>
            _firstCensus == null || _latest == null || _latest.IsEmpty ? "no snapshots" :
                ObjectGraphSnapshotter.Growth(_firstCensus, ObjectGraphSnapshotter.Census(_latest), top);
        internal ObjectGraphSnapshotter Snapshotter => _snapshotter;

        public bool CanSpeculate => VersusTickDriver.Owns(_fight) && _fight.VersusCanSpeculate;

        public void SaveState(int tick) => Capture(_ring[tick % _ring.Length], tick);

        /// <summary>Saves the current state into <paramref name="into"/> (also used by the self-test).</summary>
        internal void Capture(StateSnapshot into, int tick)
        {
            _watch.Restart();
            _roots[0] = _fight;
            _roots[1] = VersusTickDriver.State;
            _snapshotter.Capture(into, tick, _roots, StaticState);
            _roots[0] = _roots[1] = null;
            _watch.Stop();
            LastSaveMs = _watch.Elapsed.TotalMilliseconds;
            Eclipse.Diagnostics.PerformanceOverlay.RecordRollbackSave(_watch.ElapsedTicks);
            AverageSaveMs += (LastSaveMs - AverageSaveMs) / (Saves + 1);
            // The first save includes JIT and pool warm-up, so it does not count toward the peak.
            if (Saves > 0 && LastSaveMs > MaxSaveMs) MaxSaveMs = LastSaveMs;
            _latest = into;
            if (Saves++ == 0)
            {
                _firstCensus = ObjectGraphSnapshotter.Census(into);
                UnityEngine.Debug.Log("[Rollback] First snapshot: " + ObjectCount + " objects in " + LastSaveMs.ToString("0.00") +
                    " ms. Largest groups: " + string.Join(", ", _firstCensus.OrderByDescending(entry => entry.Value).Take(8)
                        .Select(entry => entry.Key.Name + " " + entry.Value)) + ".");
            }
        }

        public bool LoadState(int tick)
        {
            var snapshot = _ring[tick % _ring.Length];
            if (snapshot.Tick != tick) return false;
            Restore(snapshot);
            return true;
        }

        internal void Restore(StateSnapshot snapshot)
        {
            _watch.Restart();
            _snapshotter.Restore(snapshot);
            RollbackObjects.Restored(snapshot.Tick);
            _watch.Stop();
            LastLoadMs = _watch.Elapsed.TotalMilliseconds;
            Eclipse.Diagnostics.PerformanceOverlay.RecordRollbackRestore(_watch.ElapsedTicks);
            AverageLoadMs += (LastLoadMs - AverageLoadMs) / (Loads + 1);
            Loads++;
        }

        /// <summary>Called after every kept tick, including re-simulated ones.</summary>
        public Action<int> TickSimulated;

        public bool Simulate(int tick, byte left, byte right, TickFlags flags, out uint hash)
        {
            if (!VersusTickDriver.SimulateTick(_fight, tick, left, right, flags, out hash)) return false;
            TickSimulated?.Invoke(tick);
            return true;
        }

        /// <summary>Uses direct field copies when this runtime supports them; reflection otherwise.</summary>
        internal static ObjectGraphSnapshotter CreateSnapshotter()
        {
            // -rollback-reflection turns direct copies off, to compare against the reflection path.
            bool forced = Array.IndexOf(Environment.GetCommandLineArgs(), "-rollback-reflection") >= 0;
            var raw = ManagedMemory.Available && !forced ? new ManagedMemoryAccess() : null;
            if (forced) UnityEngine.Debug.Log("[Rollback] -rollback-reflection: snapshots use reflection.");
            else if (raw == null) UnityEngine.Debug.LogWarning("[Rollback] Direct field copies unavailable on this runtime; snapshots use reflection.");
            return new ObjectGraphSnapshotter(new FightSnapshotPolicy(), raw) { Warn = message => UnityEngine.Debug.LogWarning("[Rollback] " + message) };
        }

        private sealed class ManagedMemoryAccess : IRawObjectMemory, IRawArrayMemory
        {
            public int FieldOffset(FieldInfo field) => ManagedMemory.FieldOffset(field);
            public int SizeOf(Type valueType) => ManagedMemory.SizeOf(valueType);
            public void CopyBlocks(object from, object to, int[] offsets, int[] sizes) => ManagedMemory.CopyBlocks(from, to, offsets, sizes);
            public void CopyReferences(object from, object to, int[] offsets) => ManagedMemory.CopyReferences(from, to, offsets);
            public object ReadReference(object target, int offset) => ManagedMemory.ReadReference(target, offset);
            public int StructFieldOffset(FieldInfo field) => ManagedMemory.StructFieldOffset(field);
            public void ReadArrayReferences(Array array, int stride, int[] offsets, List<object> into) =>
                ManagedMemory.ReadArrayReferences(array, stride, offsets, into);
        }

        /// <summary>Per-fight state kept in static fields that the tick changes.</summary>
        private static FieldInfo[] FindStatics()
        {
            var fields = new List<FieldInfo>();
            const BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            // Perk registrations and remaining uses, and the fight speed factor.
            foreach (var (owner, name) in new[] { (typeof(PerksStage), "PNAALKAHAKG"), (typeof(PerksStage), "PerkUsesLeft"), (typeof(GameUtils), "NJDEBAFKGID") })
            {
                var field = owner.GetField(name, flags);
                if (field != null) fields.Add(field);
                else UnityEngine.Debug.LogWarning("[Rollback] " + owner.Name + "." + name + " not found; it will not roll back.");
            }
            return fields.ToArray();
        }
    }
}
