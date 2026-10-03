using System;
using Eclipse.Multiplayer.Balance;

namespace Eclipse.Multiplayer
{
    public static class PvpBalanceCombat
    {
        internal static PvpBalanceSnapshot Rules(Fight fight) => (fight?.GetFightDefinition() as LocalVersusMatch)?.Settings.Balance;

        internal static float ScaleDamage(Fight fight, float damage, bool blocked, Model attacker, IntervalAttack attack, Model defender)
        {
            var rules = Rules(fight);
            if (rules == null) return damage;
            var parameters = attacker?.Parameters;
            ItemInfo item = parameters?.Weapon;
            string category = item?.SubType;
            // Select the equipment supplying this attack's rating, not the projectile's appearance.
            if (parameters != null && attack != null)
                foreach (var attribute in attack.GetDamageAttributes())
                {
                    if (attribute.First == "MagicDamage") { item = parameters.Magic; category = item?.SubType; break; }
                    if (attribute.First == "RangedDamage") { item = parameters.Ranged; category = item?.SubType; break; }
                    if (attribute.First == "UnarmedDamage") { category = "Unarmed"; break; }
                }
            var effective = rules.Resolve(category, item?.Name, attacker?.GetCurrentAnimation()?.Name);
            float result = damage * (blocked ? effective.BlockedDamageScale : effective.HitDamageScale);
            return blocked && defender != null ? ClampBlocked(fight, defender, result) : result;
        }

        internal static float ClampBlocked(Fight fight, Model defender, float damage)
        {
            var rules = Rules(fight);
            if (rules == null || defender?.Parameters == null) return damage;
            var parameters = defender.Parameters;
            return PvpRecoverableHealth.ClampBlockedDamage(damage, parameters.RemainingHealthInDamageUnits,
                parameters.MaxLife, rules.MinimumLifeOnBlock);
        }

        internal static void AfterStrike(Fight fight, Model defender, Model attacker, bool blocked, float lifeBefore)
        {
            var rules = Rules(fight);
            if (rules == null || defender?.Parameters == null) return;
            var target = defender.Parameters;
            float lifeAfter = target.RemainingHealthInDamageUnits;
            float actualDamage = Math.Max(0, lifeBefore - lifeAfter);
            target.RecoverableLife = PvpRecoverableHealth.PoolAfterDamage(target.RecoverableLife,
                lifeAfter, target.MaxLife, actualDamage, blocked, rules);
            if (attacker?.Parameters == null || attacker == defender) return;
            var source = attacker.Parameters;
            float current = source.RemainingHealthInDamageUnits;
            float poolBefore = source.RecoverableLife;
            float recovery = PvpRecoverableHealth.Recovery(source.RecoverableLife, current, source.MaxLife, actualDamage, blocked, rules);
            if (recovery <= 0) return;
            fight.UpdateLife(attacker, recovery);
            // Native immortality or full-health clamps must not spend more pool than was restored.
            float restored = Math.Max(0, source.RemainingHealthInDamageUnits - current);
            source.RecoverableLife = Math.Max(0, Math.Min(source.MaxLife - source.RemainingHealthInDamageUnits, poolBefore - restored));
        }
    }
}
