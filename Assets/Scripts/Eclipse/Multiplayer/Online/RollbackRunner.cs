using System;

namespace Eclipse.Multiplayer.Online
{
    /// <summary>How a tick is being simulated.</summary>
    [Flags]
    public enum TickFlags
    {
        None = 0,
        /// <summary>The tick uses a predicted opponent input (or follows one), so it may be undone.</summary>
        Speculative = 1,
        /// <summary>The tick already ran once; presentation (sound, effects) should not repeat.</summary>
        Resimulating = 2,
        /// <summary>
        /// Re-running a tick that was discarded at a barrier, with the same inputs: everything
        /// before the barrier already played, so presentation stays quiet until the game
        /// reaches the barrier again (<c>Resimulating</c> is cleared there).
        /// </summary>
        BarrierReplay = 4,
    }

    /// <summary>The game side of rollback: one simulation tick plus state save and restore.</summary>
    public interface IRollbackGame
    {
        /// <summary>Saves the complete simulation state as it is before <paramref name="tick"/> runs.</summary>
        void SaveState(int tick);
        /// <summary>Restores the state saved before <paramref name="tick"/>; false when it is not held.</summary>
        bool LoadState(int tick);
        /// <summary>False while the simulation is somewhere a tick must never be undone (round transitions).</summary>
        bool CanSpeculate { get; }
        /// <summary>
        /// Runs one tick. Returns false when a speculative tick reached something that
        /// cannot be undone (a speculation barrier); the runner then restores the state
        /// before it and waits for confirmed inputs.
        /// </summary>
        bool Simulate(int tick, byte left, byte right, TickFlags flags, out uint hash);
    }

    /// <summary>
    /// Drives an <see cref="IRollbackGame"/> from an <see cref="InputTimeline"/>: resolves
    /// mispredictions by restoring and re-simulating, runs new ticks on predicted input
    /// when allowed, and paces this peer against the opponent's clock.
    /// With a lockstep timeline it simply runs ticks whose inputs are confirmed.
    /// </summary>
    public sealed class RollbackRunner
    {
        /// <summary>A peer this many ticks ahead waits one tick for the opponent.</summary>
        public const float WaitThreshold = 1f;
        /// <summary>Waits are spread out so pacing never reads as a stutter.</summary>
        public const int MinTicksBetweenWaits = 10;
        /// <summary>A peer this far behind runs two ticks per step to catch up.</summary>
        public const float CatchUpThreshold = 3f;

        private readonly InputTimeline _timeline;
        private readonly IRollbackGame _game;
        private readonly int _localSide;
        private int _barrierTick = -1;
        private byte _barrierRemote;
        private int _ticksSinceWait;

        public RollbackRunner(InputTimeline timeline, IRollbackGame game, int localSide)
        {
            _timeline = timeline ?? throw new ArgumentNullException(nameof(timeline));
            _game = game ?? throw new ArgumentNullException(nameof(game));
            if (localSide != 0 && localSide != 1) throw new ArgumentOutOfRangeException(nameof(localSide));
            _localSide = localSide;
        }

        public InputTimeline Timeline => _timeline;
        /// <summary>The next tick to simulate.</summary>
        public int Tick => _timeline.SimulatedTicks;
        /// <summary>Set when a needed snapshot was missing; the match cannot continue safely.</summary>
        public string Failure { get; private set; }
        public int Barriers { get; private set; }
        public int Waits { get; private set; }
        /// <summary>Ticks re-simulated by the most recent <see cref="Resolve"/>.</summary>
        public int LastResimulated { get; private set; }
        /// <summary>Rollbacks resolved so far, and the ticks they re-simulated.</summary>
        public int Rollbacks { get; private set; }
        public int TotalResimulated { get; private set; }
        public int MaxResimulated { get; private set; }

        /// <summary>True when this peer is far enough ahead that it should skip this step.</summary>
        public bool ShouldWait()
        {
            _ticksSinceWait++;
            if (!_timeline.IsRollback || _timeline.FrameAdvantage < WaitThreshold || _ticksSinceWait < MinTicksBetweenWaits) return false;
            _ticksSinceWait = 0;
            _timeline.ConsumeAdvantage(1f);
            Waits++;
            return true;
        }

        /// <summary>How many ticks to attempt this step: two when this peer has fallen behind.</summary>
        public int StepsWanted
        {
            get
            {
                if (_timeline.IsRollback) return _timeline.FrameAdvantage <= -CatchUpThreshold ? 2 : 1;
                // Lockstep: the opponent's inputs are more than two ticks ahead of ours.
                return _timeline.RemoteFrames - _timeline.Delay - 1 > Tick + 2 ? 2 : 1;
            }
        }

        /// <summary>Undoes and re-simulates any ticks that ran on a wrong prediction.</summary>
        public void Resolve()
        {
            LastResimulated = 0;
            int rollback = _timeline.PendingRollback;
            if (rollback < 0 || Failure != null) return;
            if (!_game.LoadState(rollback))
            {
                Failure = "The saved state for tick " + rollback + " was missing.";
                return;
            }
            int end = _timeline.SimulatedTicks;
            _timeline.BeginResimulation(rollback);
            for (int tick = rollback; tick < end; tick++)
            {
                if (!RunTick(tick, true)) break;
                LastResimulated++;
            }
            Rollbacks++;
            TotalResimulated += LastResimulated;
            if (LastResimulated > MaxResimulated) MaxResimulated = LastResimulated;
        }

        /// <summary>Resolves rollbacks, then tries to simulate the next tick.</summary>
        /// <returns>False when the tick has to wait for the opponent's input.</returns>
        public bool Advance()
        {
            Resolve();
            if (Failure != null) return false;
            return RunTick(_timeline.SimulatedTicks, false);
        }

        private bool RunTick(int tick, bool resimulating)
        {
            bool mayPredict = _timeline.IsRollback && tick != _barrierTick && _game.CanSpeculate;
            if (!_timeline.TryGetInputs(tick, out var local, out var remote, out var predicted) || predicted && !mayPredict)
            {
                // A re-simulation that can no longer predict keeps what it has so far.
                if (resimulating) _timeline.Rewind(tick);
                return false;
            }
            var flags = TickFlags.None;
            if (predicted) flags |= TickFlags.Speculative;
            if (resimulating) flags |= TickFlags.Resimulating;
            // The discarded run guessed right: its presentation up to the barrier already played.
            if (tick == _barrierTick && !predicted && remote == _barrierRemote) flags |= TickFlags.Resimulating | TickFlags.BarrierReplay;
            if (predicted) _game.SaveState(tick);
            byte left = _localSide == 0 ? local : remote;
            byte right = _localSide == 0 ? remote : local;
            if (!_game.Simulate(tick, left, right, flags, out var hash))
            {
                // The tick reached something that cannot be undone: drop it and run it
                // again once the opponent's input for it is confirmed.
                Barriers++;
                _barrierTick = tick;
                _barrierRemote = remote;
                if (!_game.LoadState(tick)) Failure = "The saved state for tick " + tick + " was missing.";
                _timeline.Rewind(tick);
                return false;
            }
            _timeline.MarkSimulated(tick, remote, predicted);
            _timeline.RecordLocalHash(tick, hash);
            if (tick == _barrierTick && !predicted) _barrierTick = -1;
            return true;
        }
    }
}
