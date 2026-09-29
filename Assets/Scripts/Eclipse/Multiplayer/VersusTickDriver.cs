using Eclipse.Input;
using Eclipse.Multiplayer.Online;

namespace Eclipse.Multiplayer
{
    /// <summary>Supplies both fighters' inputs for each versus simulation tick.</summary>
    public interface IVersusInputSource
    {
        /// <summary>Called once per fixed step, including stalled steps, before any tick.</summary>
        void Pump();
        /// <summary>How many ticks to attempt this fixed step (1 normally, 2 to catch up).</summary>
        int StepsWanted { get; }
        /// <returns>False to stall: the simulation does not advance this fixed step.</returns>
        bool TryGetTick(int tick, out byte left, out byte right);
        /// <param name="hash">State hash after the tick, present every <see cref="NetProtocol.HashInterval"/> ticks.
        /// The matching snapshot is <see cref="VersusTickDriver.LastSnapshot"/>.</param>
        void OnTickSimulated(int tick, byte left, byte right, uint? hash);
        /// <summary>The match reached its result on <paramref name="finalTick"/>.</summary>
        void OnMatchEnded(int finalTick, int winner, int leftRounds, int rightRounds);
        void Stop();
    }

    /// <summary>
    /// A source that runs the fixed step itself, for rollback: it may restore state and
    /// simulate several ticks (<see cref="VersusTickDriver.SimulateTick"/>) in one step.
    /// </summary>
    public interface IVersusStepRunner
    {
        void RunStep(Fight fight);
    }

    /// <summary>
    /// Owns versus input. Instead of applying device events whenever Unity delivers
    /// them, every versus fight samples inputs per simulation tick and applies them
    /// immediately before that tick. Local, online and replayed matches share this
    /// path, so a replay or a remote peer reproduces the match exactly.
    /// </summary>
    public static class VersusTickDriver
    {
        /// <summary>
        /// Driver state that belongs to the simulation. Rollback snapshots include it, so
        /// a restored tick sees the same held controls the original run did.
        /// </summary>
        internal sealed class TickState
        {
            public byte Left, Right;
            public bool Resync;
            public StageType.FDBBPEGEGMK LastStage;
            public int Tick;
        }

        private static IVersusInputSource _source;
        private static Fight _fight;
        private static TickState _state = new TickState();
        private static bool _applying;
        private static bool _inTick;
        private static TickFlags _flags;
        private static (int winner, int left, int right)? _pendingEnd;

        public static int Tick => _state.Tick;
        internal static TickState State => _state;
        /// <summary>The state behind the most recent tick hash.</summary>
        public static VersusSnapshot LastSnapshot { get; private set; }
        public static bool IsStalled { get; private set; }
        public static IVersusInputSource Source => _source;

        /// <summary>Fight banners (VS, round, "fight") count simulation ticks instead of frame time.</summary>
        public static bool PacesFightScreens => _source != null;

        public static bool Owns(Fight fight) => _source != null && fight != null && ReferenceEquals(fight, _fight);

        /// <summary>True while the driver itself is delivering control events to the fight.</summary>
        public static bool IsApplyingInput => _applying;

        /// <summary>
        /// True while simulating a tick that may still be undone (it ran on a predicted
        /// opponent input). Anything that cannot be undone must go through <see cref="Barrier"/>.
        /// </summary>
        public static bool IsSpeculating => _inTick && (_flags & TickFlags.Speculative) != 0;

        /// <summary>
        /// True while re-simulating a tick after a rollback. Presentation that already
        /// played (sounds, effects, blood, combo labels) must not play again.
        /// </summary>
        public static bool IsResimulating => _inTick && (_flags & TickFlags.Resimulating) != 0;

        /// <summary>Set when a speculative tick reached a <see cref="Barrier"/>; the tick is then discarded.</summary>
        public static bool BarrierHit { get; private set; }

        /// <summary>
        /// Call before something that must never be undone (a round transition, the match
        /// result). Returns true when the current tick is speculative: skip the action; the
        /// tick is thrown away and runs again once the opponent's input is confirmed.
        /// </summary>
        public static bool Barrier()
        {
            if (!IsSpeculating) return false;
            BarrierHit = true;
            return true;
        }

        public static void Begin(Fight fight, IVersusInputSource source, int seed)
        {
            Stop();
            _fight = fight;
            _source = source;
            _state = new TickState { Left = NetInput.Neutral, Right = NetInput.Neutral, LastStage = fight != null ? fight.stageType : default };
            IsStalled = false;
            _inTick = false;
            _flags = TickFlags.None;
            BarrierHit = false;
            _pendingEnd = null;
            VersusDeterminism.Seed(seed);
            VersusDeterminism.BeginTick(0);
        }

        public static void Stop()
        {
            var source = _source;
            _source = null;
            _fight = null;
            IsStalled = false;
            _inTick = false;
            _flags = TickFlags.None;
            _pendingEnd = null;
            VersusDeterminism.End();
            source?.Stop();
        }

        /// <returns>How many ticks <see cref="Fight.Draw"/> should run; 0 when the source ran the step itself.</returns>
        internal static int StepsFor(Fight fight)
        {
            if (!Owns(fight)) return 1;
            // A local pause drops presses; held controls are re-delivered on resume.
            if (fight.IsPaused()) _state.Resync = true;
            _source.Pump();
            if (_source is IVersusStepRunner runner)
            {
                if (fight.PrepareVersusStep()) runner.RunStep(fight);
                return 0;
            }
            return _source != null && _source.StepsWanted > 1 ? 2 : 1;
        }

        /// <returns>False when this fixed step must not simulate.</returns>
        internal static bool BeforeTick(Fight fight)
        {
            if (!Owns(fight)) return true;
            if (!_source.TryGetTick(_state.Tick, out var left, out var right))
            {
                IsStalled = true;
                return false;
            }
            IsStalled = false;
            BeginTick(fight, left, right, TickFlags.None);
            return true;
        }

        internal static void AfterTick(Fight fight)
        {
            if (!Owns(fight)) { _inTick = false; return; }
            int tick = EndTick(fight, out var hash);
            if (tick < 0) return;
            _source.OnTickSimulated(tick, _state.Left, _state.Right, hash);
            DeliverPendingEnd(tick);
        }

        /// <summary>
        /// Runs exactly one tick with the given inputs, for sources that drive the step
        /// themselves (<see cref="IVersusStepRunner"/>).
        /// </summary>
        /// <returns>False when a speculative tick hit a <see cref="Barrier"/> and must be discarded.</returns>
        internal static bool SimulateTick(Fight fight, int tick, byte left, byte right, TickFlags flags, out uint hash)
        {
            hash = 0;
            if (!Owns(fight)) return false;
            if (tick != _state.Tick) throw new System.InvalidOperationException("Versus tick " + tick + " requested on state " + _state.Tick + ".");
            IsStalled = false;
            BarrierHit = false;
            BeginTick(fight, left, right, flags);
            Eclipse.Diagnostics.PerformanceOverlay.BeginFightSimulation();
            try { fight.Render(); }
            finally { Eclipse.Diagnostics.PerformanceOverlay.EndFightSimulation(); }
            if (!Owns(fight)) { _inTick = false; _flags = TickFlags.None; return false; }
            int done = EndTick(fight, out var tickHash);
            bool kept = !BarrierHit;
            BarrierHit = false;
            hash = tickHash ?? 0u;
            if (done >= 0 && kept) DeliverPendingEnd(done);
            else _pendingEnd = null;
            return kept && done >= 0;
        }

        /// <summary>The stepping source could not run a tick this step.</summary>
        internal static void MarkStalled(bool stalled) => IsStalled = stalled;

        private static void BeginTick(Fight fight, byte left, byte right, TickFlags flags)
        {
            VersusDeterminism.BeginTick(_state.Tick);
            _flags = flags;
            _applying = true;
            _inTick = true;
            try
            {
                // The fight ignores presses outside the stages that accept them, so a
                // control held across a stage change or pause is released and pressed
                // again, like the controller restart this path replaced.
                if (_state.Resync || fight.stageType != _state.LastStage)
                {
                    Apply(fight, 0, ref _state.Left, NetInput.Neutral);
                    Apply(fight, 1, ref _state.Right, NetInput.Neutral);
                    _state.Resync = false;
                    _state.LastStage = fight.stageType;
                }
                Apply(fight, 0, ref _state.Left, left);
                Apply(fight, 1, ref _state.Right, right);
            }
            finally { _applying = false; }
        }

        /// <returns>The tick just finished, or -1 when the fight stopped being driven.</returns>
        private static int EndTick(Fight fight, out uint? hash)
        {
            hash = null;
            fight.AdvanceVersusScreens(VersusDeterminism.TickSeconds);
            if (!Owns(fight)) { _inTick = false; _flags = TickFlags.None; return -1; }
            int tick = _state.Tick++;
            if (tick % NetProtocol.HashInterval == 0)
            {
                // A hashing failure must never skip recording this tick's inputs.
                try
                {
                    LastSnapshot = VersusStateHash.Capture(fight, tick);
                    hash = VersusStateHash.Hash(LastSnapshot);
                }
                catch (System.Exception exception)
                {
                    UnityEngine.Debug.LogException(exception);
                    hash = 0xDEADBEEFu ^ (uint)tick;
                }
            }
            _inTick = false;
            _flags = TickFlags.None;
            return tick;
        }

        // Delivered only now, so the tick the match ended on is recorded first.
        private static void DeliverPendingEnd(int tick)
        {
            if (!_pendingEnd.HasValue || _source == null) return;
            var end = _pendingEnd.Value;
            _pendingEnd = null;
            _source.OnMatchEnded(tick, end.winner, end.left, end.right);
        }

        internal static void MatchEnded(Fight fight, int winner, int leftRounds, int rightRounds)
        {
            if (!Owns(fight)) return;
            if (_inTick) _pendingEnd = (winner, leftRounds, rightRounds);
            else _source.OnMatchEnded(_state.Tick - 1, winner, leftRounds, rightRounds);
        }

        // Releases before presses so a same-tick direction change never holds two quadrants.
        private static void Apply(Fight fight, int side, ref byte current, byte next)
        {
            if (current == next) return;
            int oldDirection = NetInput.Direction(current), newDirection = NetInput.Direction(next);
            if (oldDirection != newDirection && oldDirection != 0) fight.ApplyVersusControl(side, false, (FightCID)oldDirection);
            ApplyButton(fight, side, current, next, NetInput.Punch, FightCID.Punch, false);
            ApplyButton(fight, side, current, next, NetInput.Kick, FightCID.Kick, false);
            ApplyButton(fight, side, current, next, NetInput.Ranged, FightCID.MissileButton, false);
            ApplyButton(fight, side, current, next, NetInput.Magic, FightCID.MagicButton, false);
            if (oldDirection != newDirection && newDirection != 0) fight.ApplyVersusControl(side, true, (FightCID)newDirection);
            ApplyButton(fight, side, current, next, NetInput.Punch, FightCID.Punch, true);
            ApplyButton(fight, side, current, next, NetInput.Kick, FightCID.Kick, true);
            ApplyButton(fight, side, current, next, NetInput.Ranged, FightCID.MissileButton, true);
            ApplyButton(fight, side, current, next, NetInput.Magic, FightCID.MagicButton, true);
            current = next;
        }

        private static void ApplyButton(Fight fight, int side, byte current, byte next, byte bit, FightCID control, bool press)
        {
            bool was = (current & bit) != 0, now = (next & bit) != 0;
            if (was != now && now == press) fight.ApplyVersusControl(side, press, control);
        }
    }

    /// <summary>Turns one device's held controls into a <see cref="NetInput"/> byte.</summary>
    public sealed class VersusInputSampler
    {
        private readonly FightGamepadInput _input;
        private readonly bool _keyboardMovement, _keyboardActions, _gamepad;
        private byte _state;

        public VersusInputSampler(GamePad.Player player, bool keyboardMovement, bool keyboardActions, bool gamepad,
            FightKeyboardLayout layout = null)
        {
            _keyboardMovement = keyboardMovement;
            _keyboardActions = keyboardActions;
            _gamepad = gamepad;
            _input = new FightGamepadInput(control => true, OnControl, player, layout);
        }

        public byte Sample(bool enabled)
        {
            if (enabled) _input.Poll(_keyboardMovement, _keyboardActions, _gamepad);
            else _input.ReleaseAll();
            return _state;
        }

        /// <summary>Samples the device and merges the on-screen touch controls (player one's controls).</summary>
        public byte SampleWithTouch(bool enabled)
        {
            byte device = Sample(enabled);
            if (!enabled) return device;
            var controller = Nekki.SF2.Core.Fights.Controller.GameController.get_Current();
            byte touch = controller != null ? controller.VersusTouchInput : NetInput.Neutral;
            int direction = NetInput.Direction(touch) != 0 ? NetInput.Direction(touch) : NetInput.Direction(device);
            return NetInput.WithDirection((byte)((device | touch) & ~NetInput.DirectionMask), direction);
        }

        private void OnControl(int eventType, FightCID control) => _state = ApplyControl(_state, eventType, control);

        /// <summary>Folds one press (0) or release (1) event into a held-input byte.</summary>
        public static byte ApplyControl(byte state, int eventType, FightCID control)
        {
            bool press = eventType == 0;
            if (control >= FightCID.QuadrantUp && control <= FightCID.QuadrantUpBack)
            {
                if (press) return NetInput.WithDirection(state, (int)control);
                return NetInput.Direction(state) == (int)control ? NetInput.WithDirection(state, 0) : state;
            }
            switch (control)
            {
                case FightCID.Punch: return NetInput.WithButton(state, NetInput.Punch, press);
                case FightCID.Kick: return NetInput.WithButton(state, NetInput.Kick, press);
                case FightCID.MissileButton: return NetInput.WithButton(state, NetInput.Ranged, press);
                case FightCID.MagicButton: return NetInput.WithButton(state, NetInput.Magic, press);
                default: return state;
            }
        }
    }
}
