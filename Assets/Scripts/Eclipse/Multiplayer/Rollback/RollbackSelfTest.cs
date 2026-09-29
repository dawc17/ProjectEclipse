using System;
using Eclipse.Multiplayer.Online;
using UnityEngine;

namespace Eclipse.Multiplayer.Rollback
{
    /// <summary>
    /// Plays a recorded match while rolling back constantly, the way a bad connection
    /// would: every mid-round tick runs speculatively, and every other tick the last
    /// <see cref="Depth"/> ticks are restored and simulated again. The full saved state after the
    /// re-simulation must equal the state after the first run; any difference names the
    /// field that rollback failed to restore. Needs only one machine.
    /// </summary>
    public sealed class RollbackSelfTest : IVersusInputSource, IVersusStepRunner
    {
        /// <summary>Ticks undone by every forced rollback.</summary>
        public const int Depth = 6;
        private const int MaxLoggedMismatches = 5;

        private readonly VersusReplay _replay;
        private readonly StateSnapshot _original = new StateSnapshot();
        private readonly StateSnapshot _redone = new StateSnapshot();
        private readonly uint[] _hashes = new uint[64];
        private FightRollback _rollback;
        private int _speculativeRun;
        private double _resimulationMs;

        public RollbackSelfTest(VersusReplay replay)
        {
            _replay = replay ?? throw new ArgumentNullException(nameof(replay));
        }

        public bool ReachedEnd { get; private set; }
        public int Rollbacks { get; private set; }
        public int Mismatches { get; private set; }
        public int HashMismatches { get; private set; }
        public int Barriers { get; private set; }
        public int CheckedHashes { get; private set; }
        public int DivergedTick { get; private set; } = -1;
        public string FirstMismatch { get; private set; }

        public void Pump() { }
        public int StepsWanted => 1;
        public bool TryGetTick(int tick, out byte left, out byte right) { left = right = 0; return false; }
        public void OnTickSimulated(int tick, byte left, byte right, uint? hash) { }
        public void OnMatchEnded(int finalTick, int winner, int leftRounds, int rightRounds) => ReachedEnd = true;
        public void Stop() => RollbackObjects.Clear();

        public void RunStep(Fight fight)
        {
            if (_rollback == null) _rollback = new FightRollback(fight, NetProtocol.DefaultRollbackWindow);
            int tick = VersusTickDriver.Tick;
            if (tick >= _replay.TickCount) { ReachedEnd = true; return; }
            byte left = _replay.Left[tick], right = _replay.Right[tick];
            bool speculate = _rollback.CanSpeculate;
            if (speculate) _rollback.SaveState(tick);
            if (!_rollback.Simulate(tick, left, right, speculate ? TickFlags.Speculative : TickFlags.None, out var hash))
            {
                // A barrier: undo the tick and run it as confirmed, as an online match would.
                Barriers++;
                if (!_rollback.LoadState(tick) || !_rollback.Simulate(tick, left, right, TickFlags.None, out hash))
                {
                    Fail(tick, "the tick could not run again after a barrier");
                    return;
                }
                speculate = false;
            }
            if (!VersusTickDriver.Owns(fight)) return;
            _hashes[tick & (_hashes.Length - 1)] = hash;
            CheckRecording(tick, hash);
            _speculativeRun = speculate ? _speculativeRun + 1 : 0;
            // Every other tick keeps the test near real time while still covering each tick twice.
            if (_speculativeRun >= Depth && tick % 2 == 0) RollBack(fight, tick);
            RollbackObjects.Finalized(tick - Depth + 2);
        }

        private void RollBack(Fight fight, int tick)
        {
            int from = tick - Depth + 1;
            _rollback.Capture(_original, tick + 1);
            if (!_rollback.LoadState(from)) { Fail(from, "its snapshot was missing"); return; }
            Rollbacks++;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            for (int t = from; t <= tick; t++)
            {
                if (!_rollback.Simulate(t, _replay.Left[t], _replay.Right[t], TickFlags.Speculative | TickFlags.Resimulating, out var again))
                {
                    Fail(t, "re-simulation hit a barrier the first run did not");
                    return;
                }
                if (again != _hashes[t & (_hashes.Length - 1)])
                {
                    HashMismatches++;
                    if (HashMismatches <= MaxLoggedMismatches)
                        Debug.LogError("[Rollback test] Tick " + t + " hashed differently when re-simulated. " + VersusStateHash.Describe(fight));
                }
            }
            watch.Stop();
            _resimulationMs = _resimulationMs <= 0 ? watch.Elapsed.TotalMilliseconds : _resimulationMs * 0.95 + watch.Elapsed.TotalMilliseconds * 0.05;
            _rollback.Capture(_redone, tick + 1);
            string difference = _rollback.Snapshotter.FirstDifference(_original, _redone);
            if (difference == null) return;
            Mismatches++;
            if (FirstMismatch == null) FirstMismatch = "ticks " + from + "-" + tick + ": " + difference;
            if (Mismatches <= MaxLoggedMismatches)
                Debug.LogError("[Rollback test] State after re-simulating ticks " + from + "-" + tick + " differs from the first run: " + difference);
        }

        private void CheckRecording(int tick, uint hash)
        {
            if (!_replay.Hashes.TryGetValue(tick, out var expected)) return;
            CheckedHashes++;
            if (expected == hash || DivergedTick >= 0) return;
            DivergedTick = tick;
            Debug.LogError("[Rollback test] The match diverged from its recording at tick " + tick + ".");
        }

        private void Fail(int tick, string reason)
        {
            Mismatches++;
            if (FirstMismatch == null) FirstMismatch = "tick " + tick + ": " + reason;
            Debug.LogError("[Rollback test] Stopped at tick " + tick + ": " + reason + ".");
            ReachedEnd = true;
        }

        public string Summary()
        {
            string cost = _rollback == null ? "" :
                " Snapshot " + _rollback.ObjectCount + " objects, save " + _rollback.AverageSaveMs.ToString("0.00") + " ms, restore " +
                _rollback.LastLoadMs.ToString("0.00") + " ms, " + Depth + "-tick re-simulation " + _resimulationMs.ToString("0.0") + " ms.";
            if (Rollbacks == 0) return "Rollback test: no mid-round ticks to roll back." + cost;
            if (Mismatches == 0 && HashMismatches == 0 && DivergedTick < 0)
                return "Rollback test passed: " + Rollbacks + " rollbacks restored every value (" + Barriers + " barriers)." + cost;
            return "Rollback test FAILED: " + Mismatches + " state mismatches, " + HashMismatches + " hash mismatches" +
                (DivergedTick >= 0 ? ", diverged from the recording at tick " + DivergedTick : "") + ". First: " + FirstMismatch +
                " Full details are in the log." + cost;
        }
    }
}
