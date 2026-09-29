using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.Multiplayer.Rollback
{
    /// <summary>
    /// Scene objects that speculative ticks create or remove. A rollback restores managed
    /// state, but GameObjects live outside it: objects created by an undone tick are
    /// destroyed, and objects an undone tick removed are shown again. Removal only hides
    /// an object until its tick is final, then it is destroyed for real.
    /// Outside a speculative tick every call does nothing and returns false.
    /// </summary>
    public static class RollbackObjects
    {
        private struct Entry
        {
            public int Tick;
            public GameObject Target;
            public bool WasActive;
            public string Sound;
        }

        private static readonly List<Entry> CreatedObjects = new List<Entry>();
        private static readonly List<Entry> HiddenObjects = new List<Entry>();
        private static readonly List<Entry> StartedLoops = new List<Entry>();
        private static readonly List<Entry> Toggled = new List<Entry>();

        public static int Pending => CreatedObjects.Count + HiddenObjects.Count + StartedLoops.Count + Toggled.Count;

        /// <summary>
        /// Sets an object's active state; during a speculative tick the previous state is
        /// remembered so a rollback restores it (a vanish that never happened).
        /// </summary>
        public static void SetActive(GameObject target, bool value)
        {
            if (target == null) return;
            if (VersusTickDriver.IsSpeculating && target.activeSelf != value)
                Toggled.Add(new Entry { Tick = VersusTickDriver.Tick, Target = target, WasActive = target.activeSelf });
            target.SetActive(value);
        }

        /// <summary>A speculative tick created <paramref name="target"/>.</summary>
        public static void Created(GameObject target)
        {
            if (target == null || !VersusTickDriver.IsSpeculating) return;
            CreatedObjects.Add(new Entry { Tick = VersusTickDriver.Tick, Target = target });
        }

        /// <summary>
        /// Called instead of destroying <paramref name="target"/>. During a speculative tick it
        /// only deactivates the object and returns true; otherwise returns false and the
        /// caller destroys it as usual.
        /// </summary>
        public static bool Hide(GameObject target)
        {
            if (target == null || !VersusTickDriver.IsSpeculating) return false;
            HiddenObjects.Add(new Entry { Tick = VersusTickDriver.Tick, Target = target, WasActive = target.activeSelf });
            target.SetActive(false);
            return true;
        }

        /// <summary>A speculative tick started the looped sound <paramref name="path"/>.</summary>
        public static void LoopStarted(string path)
        {
            if (string.IsNullOrEmpty(path) || !VersusTickDriver.IsSpeculating) return;
            StartedLoops.Add(new Entry { Tick = VersusTickDriver.Tick, Sound = path });
        }

        /// <summary>The state before <paramref name="tick"/> was restored: undo what later ticks did.</summary>
        public static void Restored(int tick)
        {
            for (int i = CreatedObjects.Count - 1; i >= 0; i--)
            {
                if (CreatedObjects[i].Tick < tick) continue;
                if (CreatedObjects[i].Target != null) Object.Destroy(CreatedObjects[i].Target);
                CreatedObjects.RemoveAt(i);
            }
            for (int i = HiddenObjects.Count - 1; i >= 0; i--)
            {
                if (HiddenObjects[i].Tick < tick) continue;
                if (HiddenObjects[i].Target != null) HiddenObjects[i].Target.SetActive(HiddenObjects[i].WasActive);
                HiddenObjects.RemoveAt(i);
            }
            // Newest first, so the earliest recorded state wins.
            for (int i = Toggled.Count - 1; i >= 0; i--)
            {
                if (Toggled[i].Tick < tick) continue;
                if (Toggled[i].Target != null) Toggled[i].Target.SetActive(Toggled[i].WasActive);
                Toggled.RemoveAt(i);
            }
            for (int i = StartedLoops.Count - 1; i >= 0; i--)
            {
                if (StartedLoops[i].Tick < tick) continue;
                Sound.StopSound(StartedLoops[i].Sound);
                StartedLoops.RemoveAt(i);
            }
        }

        /// <summary>Ticks below <paramref name="finalTicks"/> can no longer be undone.</summary>
        public static void Finalized(int finalTicks)
        {
            CreatedObjects.RemoveAll(entry => entry.Tick < finalTicks);
            StartedLoops.RemoveAll(entry => entry.Tick < finalTicks);
            Toggled.RemoveAll(entry => entry.Tick < finalTicks);
            for (int i = HiddenObjects.Count - 1; i >= 0; i--)
            {
                if (HiddenObjects[i].Tick >= finalTicks) continue;
                if (HiddenObjects[i].Target != null) Object.Destroy(HiddenObjects[i].Target);
                HiddenObjects.RemoveAt(i);
            }
        }

        /// <summary>The match is over: keep what exists and destroy what was only hidden.</summary>
        public static void Clear() => Finalized(int.MaxValue);
    }
}
