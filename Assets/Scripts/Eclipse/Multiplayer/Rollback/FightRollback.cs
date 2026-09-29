using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public SnapshotCodec CodecFor(Type type) => type == typeof(Vector3f) || type == typeof(Vector2f) ? Vectors : null;

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
        private readonly ObjectGraphSnapshotter _snapshotter = new ObjectGraphSnapshotter(new FightSnapshotPolicy());
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
        public double LastLoadMs { get; private set; }
        public int ObjectCount => _snapshotter.LastObjectCount;
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
            AverageSaveMs = Saves == 0 ? LastSaveMs : AverageSaveMs * 0.95 + LastSaveMs * 0.05;
            if (Saves++ == 0)
                UnityEngine.Debug.Log("[Rollback] First snapshot: " + ObjectCount + " objects in " + LastSaveMs.ToString("0.00") + " ms.");
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
