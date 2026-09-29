using System.Text;
using Eclipse.Multiplayer.Online;

namespace Eclipse.Multiplayer
{
    /// <summary>
    /// Fingerprint of the versus simulation after a tick. Peers exchange it to detect
    /// desyncs, and replays store it to prove they reproduce the match.
    /// </summary>
    public static class VersusStateHash
    {
        public static uint Compute(Fight fight, int tick)
        {
            var hasher = new StateHasher();
            hasher.Add(tick);
            if (fight == null) return hasher.Value;
            hasher.Add((int)fight.stageType);
            hasher.Add(fight.get_RoundNumber());
            hasher.Add(fight.get_FightTimeInFrames());
            hasher.Add(fight.get_RoundTimeLeftFrames());
            AddModel(ref hasher, fight.GetPlayerModel());
            AddModel(ref hasher, fight.GetEnemyModel());
            return hasher.Value;
        }

        private static void AddModel(ref StateHasher hasher, Model model)
        {
            if (model == null) { hasher.Add(-1); return; }
            var position = model.PLBNCDCFPML();
            hasher.Add(position != null ? position.GetX() : float.NaN);
            hasher.Add(position != null ? position.GetY() : float.NaN);
            hasher.Add(model.KFCNPADAMHA());
            hasher.Add(model.KKMCHCNOHMB());
            hasher.Add(model.Parameters != null ? model.Parameters.RoundsWon : -1);
            hasher.Add(model.GetCurrentAnimation()?.Name);
            var animation = model.OCPMJKIEPIG();
            // Only counters that are safe without an active animation (fighters have none during the intro).
            if (animation != null)
            {
                hasher.Add(animation.NEBJGKODIKP());
                hasher.Add(animation.HILLKPNMCIP());
            }
        }

        /// <summary>Human-readable state for desync logs.</summary>
        public static string Describe(Fight fight)
        {
            if (fight == null) return "No fight.";
            try { return DescribeUnchecked(fight); }
            catch (System.Exception exception) { return "State unavailable (" + exception.GetType().Name + ")."; }
        }

        private static string DescribeUnchecked(Fight fight)
        {
            var text = new StringBuilder();
            text.Append("tick ").Append(VersusTickDriver.Tick).Append(", stage ").Append(fight.stageType)
                .Append(", round ").Append(fight.get_RoundNumber()).Append(", fight frames ").Append(fight.get_FightTimeInFrames())
                .Append(", time left ").Append(fight.get_RoundTimeLeftFrames());
            Describe(text, "left", fight.GetPlayerModel());
            Describe(text, "right", fight.GetEnemyModel());
            return text.ToString();
        }

        private static void Describe(StringBuilder text, string side, Model model)
        {
            text.Append("; ").Append(side).Append(": ");
            if (model == null) { text.Append("none"); return; }
            var position = model.PLBNCDCFPML();
            var animation = model.OCPMJKIEPIG();
            text.Append("pos ").Append(position?.GetX().ToString("R")).Append(",").Append(position?.GetY().ToString("R"))
                .Append(" facing ").Append(model.KFCNPADAMHA())
                .Append(" life ").Append(model.KKMCHCNOHMB().ToString("R"))
                .Append(" anim ").Append(model.GetCurrentAnimation()?.Name ?? "-");
            if (animation != null)
                text.Append(" frames ").Append(animation.NEBJGKODIKP()).Append("/").Append(animation.HILLKPNMCIP());
        }
    }
}
