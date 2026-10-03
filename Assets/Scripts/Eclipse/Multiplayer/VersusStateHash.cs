using System.Text;
using Eclipse.Multiplayer.Online;

namespace Eclipse.Multiplayer
{
    /// <summary>One fighter's hashed state after a tick. A plain struct, so a history costs no allocations.</summary>
    public struct FighterSnapshot
    {
        public bool Present;
        public float X, Y, Life, RecoverableLife;
        public int Facing, RoundsWon, Frame, Interval;
        public string Animation;
    }

    /// <summary>The versus simulation state that <see cref="VersusStateHash"/> fingerprints.</summary>
    public struct VersusSnapshot
    {
        public int Tick, Stage, Round, FightFrames, TimeLeft;
        public FighterSnapshot Left, Right;
    }

    /// <summary>
    /// Fingerprint of the versus simulation after a tick. Peers exchange it to detect
    /// desyncs, and replays store it to prove they reproduce the match.
    /// </summary>
    public static class VersusStateHash
    {
        public static VersusSnapshot Capture(Fight fight, int tick)
        {
            var snapshot = new VersusSnapshot { Tick = tick };
            if (fight == null) return snapshot;
            snapshot.Stage = (int)fight.stageType;
            snapshot.Round = fight.get_RoundNumber();
            snapshot.FightFrames = fight.get_FightTimeInFrames();
            snapshot.TimeLeft = fight.get_RoundTimeLeftFrames();
            snapshot.Left = CaptureModel(fight.GetPlayerModel());
            snapshot.Right = CaptureModel(fight.GetEnemyModel());
            return snapshot;
        }

        private static FighterSnapshot CaptureModel(Model model)
        {
            if (model == null) return default;
            var position = model.PLBNCDCFPML();
            var result = new FighterSnapshot
            {
                Present = true,
                X = position != null ? position.GetX() : float.NaN,
                Y = position != null ? position.GetY() : float.NaN,
                Facing = model.KFCNPADAMHA(),
                Life = model.KKMCHCNOHMB(),
                RecoverableLife = model.Parameters != null ? model.Parameters.RecoverableLife : 0f,
                RoundsWon = model.Parameters != null ? model.Parameters.RoundsWon : -1,
                Animation = model.GetCurrentAnimation()?.Name,
            };
            // Only counters that are safe without an active animation (fighters have none during the intro).
            var animation = model.OCPMJKIEPIG();
            if (animation != null)
            {
                result.Frame = animation.NEBJGKODIKP();
                result.Interval = animation.GetFrameInMove();
            }
            return result;
        }

        public static uint Hash(in VersusSnapshot snapshot)
        {
            var hasher = new StateHasher();
            hasher.Add(snapshot.Tick);
            hasher.Add(snapshot.Stage);
            hasher.Add(snapshot.Round);
            hasher.Add(snapshot.FightFrames);
            hasher.Add(snapshot.TimeLeft);
            Add(ref hasher, snapshot.Left);
            Add(ref hasher, snapshot.Right);
            return hasher.Value;
        }

        private static void Add(ref StateHasher hasher, in FighterSnapshot fighter)
        {
            if (!fighter.Present) { hasher.Add(-1); return; }
            hasher.Add(fighter.X);
            hasher.Add(fighter.Y);
            hasher.Add(fighter.Facing);
            hasher.Add(fighter.Life);
            hasher.Add(fighter.RecoverableLife);
            hasher.Add(fighter.RoundsWon);
            hasher.Add(fighter.Animation);
            hasher.Add(fighter.Frame);
            hasher.Add(fighter.Interval);
        }

        public static uint Compute(Fight fight, int tick) => Hash(Capture(fight, tick));

        /// <summary>Exact state for desync reports; floats include their bit patterns.</summary>
        public static string Format(in VersusSnapshot snapshot)
        {
            var text = new StringBuilder(320);
            text.Append("tick ").Append(snapshot.Tick).Append(" stage ").Append(snapshot.Stage).Append(" round ").Append(snapshot.Round)
                .Append(" frames ").Append(snapshot.FightFrames).Append(" left ").Append(snapshot.TimeLeft);
            Format(text, " | L ", snapshot.Left);
            Format(text, " | R ", snapshot.Right);
            return text.ToString();
        }

        private static void Format(StringBuilder text, string label, in FighterSnapshot fighter)
        {
            text.Append(label);
            if (!fighter.Present) { text.Append("none"); return; }
            text.Append(Exact(fighter.X)).Append(',').Append(Exact(fighter.Y)).Append(" f").Append(fighter.Facing)
                .Append(" hp ").Append(Exact(fighter.Life)).Append(" w").Append(fighter.RoundsWon)
                .Append(" grey ").Append(Exact(fighter.RecoverableLife))
                .Append(' ').Append(fighter.Animation ?? "-").Append(' ').Append(fighter.Frame).Append('/').Append(fighter.Interval);
        }

        private static string Exact(float value)
        {
            return value.ToString("R", System.Globalization.CultureInfo.InvariantCulture) + "#" + StateHasher.Bits(value).ToString("X8");
        }

        /// <summary>Human-readable current state for logs.</summary>
        public static string Describe(Fight fight)
        {
            if (fight == null) return "No fight.";
            try { return Format(Capture(fight, VersusTickDriver.Tick)); }
            catch (System.Exception exception) { return "State unavailable (" + exception.GetType().Name + ")."; }
        }
    }
}
