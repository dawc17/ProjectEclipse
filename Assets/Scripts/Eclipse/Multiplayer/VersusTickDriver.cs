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
        /// <param name="hash">State hash after the tick, present every <see cref="NetProtocol.HashInterval"/> ticks.</param>
        void OnTickSimulated(int tick, byte left, byte right, uint? hash);
        /// <summary>The match reached its result on <paramref name="finalTick"/>.</summary>
        void OnMatchEnded(int finalTick, int winner, int leftRounds, int rightRounds);
        void Stop();
    }

    /// <summary>
    /// Owns versus input. Instead of applying device events whenever Unity delivers
    /// them, every versus fight samples inputs per simulation tick and applies them
    /// immediately before that tick. Local, online and replayed matches share this
    /// path, so a replay or a remote peer reproduces the match exactly.
    /// </summary>
    public static class VersusTickDriver
    {
        private static IVersusInputSource _source;
        private static Fight _fight;
        private static byte _left, _right;
        private static bool _applying;
        private static bool _inTick;
        private static bool _resync;
        private static StageType.FDBBPEGEGMK _lastStage;
        private static (int winner, int left, int right)? _pendingEnd;

        public static int Tick { get; private set; }
        public static bool IsStalled { get; private set; }
        public static IVersusInputSource Source => _source;

        /// <summary>Fight banners (VS, round, "fight") count simulation ticks instead of frame time.</summary>
        public static bool PacesFightScreens => _source != null;

        public static bool Owns(Fight fight) => _source != null && fight != null && ReferenceEquals(fight, _fight);

        /// <summary>True while the driver itself is delivering control events to the fight.</summary>
        public static bool IsApplyingInput => _applying;

        public static void Begin(Fight fight, IVersusInputSource source, int seed)
        {
            Stop();
            _fight = fight;
            _source = source;
            _left = _right = NetInput.Neutral;
            Tick = 0;
            IsStalled = false;
            _inTick = _resync = false;
            _pendingEnd = null;
            _lastStage = fight != null ? fight.stageType : default;
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
            _pendingEnd = null;
            VersusDeterminism.End();
            source?.Stop();
        }

        internal static int StepsFor(Fight fight)
        {
            if (!Owns(fight)) return 1;
            // A local pause drops presses; held controls are re-delivered on resume.
            if (fight.IsPaused()) _resync = true;
            _source.Pump();
            return _source != null && _source.StepsWanted > 1 ? 2 : 1;
        }

        /// <returns>False when this fixed step must not simulate.</returns>
        internal static bool BeforeTick(Fight fight)
        {
            if (!Owns(fight)) return true;
            if (!_source.TryGetTick(Tick, out var left, out var right))
            {
                IsStalled = true;
                return false;
            }
            IsStalled = false;
            VersusDeterminism.BeginTick(Tick);
            _applying = true;
            _inTick = true;
            try
            {
                // The fight ignores presses outside the stages that accept them, so a
                // control held across a stage change or pause is released and pressed
                // again, like the controller restart this path replaced.
                if (_resync || fight.stageType != _lastStage)
                {
                    Apply(fight, 0, ref _left, NetInput.Neutral);
                    Apply(fight, 1, ref _right, NetInput.Neutral);
                    _resync = false;
                    _lastStage = fight.stageType;
                }
                Apply(fight, 0, ref _left, left);
                Apply(fight, 1, ref _right, right);
            }
            finally { _applying = false; }
            return true;
        }

        internal static void AfterTick(Fight fight)
        {
            if (!Owns(fight)) { _inTick = false; return; }
            fight.AdvanceVersusScreens(VersusDeterminism.TickSeconds);
            if (!Owns(fight)) { _inTick = false; return; }
            int tick = Tick++;
            uint? hash = null;
            if (tick % NetProtocol.HashInterval == 0)
            {
                // A hashing failure must never skip recording this tick's inputs.
                try { hash = VersusStateHash.Compute(fight, tick); }
                catch (System.Exception exception)
                {
                    UnityEngine.Debug.LogException(exception);
                    hash = 0xDEADBEEFu ^ (uint)tick;
                }
            }
            _source.OnTickSimulated(tick, _left, _right, hash);
            _inTick = false;
            // Delivered only now, so the tick the match ended on is recorded first.
            if (_pendingEnd.HasValue && _source != null)
            {
                var end = _pendingEnd.Value;
                _pendingEnd = null;
                _source.OnMatchEnded(tick, end.winner, end.left, end.right);
            }
        }

        internal static void MatchEnded(Fight fight, int winner, int leftRounds, int rightRounds)
        {
            if (!Owns(fight)) return;
            if (_inTick) _pendingEnd = (winner, leftRounds, rightRounds);
            else _source.OnMatchEnded(Tick - 1, winner, leftRounds, rightRounds);
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
