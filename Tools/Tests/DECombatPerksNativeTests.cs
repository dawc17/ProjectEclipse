using System;
using System.Collections.Generic;
using System.Linq;
using Eclipse.Modding;

internal static class DECombatPerksNativeTests
{
    private static int _checks;
    private static void Check(bool value, string message) { _checks++; if (!value) throw new Exception(message); }

    public static int Main()
    {
        CheckHitClassificationAndLiveDamage();
        CheckReverseSidesAndLocalVersus();
        CheckOutgoingArithmetic();
        CheckStatusIcons();
        Console.WriteLine("DE combat perks native PASS: " + _checks + " checks; extracted current hit-phase/status-icon methods and current hit contract.");
        return 0;
    }

    private static void CheckHitClassificationAndLiveDamage()
    {
        var fight = new FightHarness();
        fight.Player = new Model { Name = "player" };
        fight.Opponent = new Model { Name = "opponent" };
        var animation = new InfoAnimation("Weapon", "Unarmed");
        var strike = new Model.StrikeResult {
            GAIBPAGPEGK = fight.Player, PBPDKJNKFCJ = animation, EEDJBBOCFNL = 0.20f,
            DFOHNJEBDED = false, DNGKOMPMPCD = true
        };
        fight.BeginDispatched = false;
        fight.Hit(new Model.EventModel { KJDFJPBIGJC = fight.Opponent, GAIBPAGPEGK = fight.Player }, strike, ModEffectEvent.PostHit);
        Check(fight.Events.Count == 0, "Hit phase dispatched before fight-begin initialization.");
        fight.BeginDispatched = true;
        fight.Hit(new Model.EventModel { KJDFJPBIGJC = fight.Opponent, GAIBPAGPEGK = fight.Player }, strike, ModEffectEvent.PostHit);
        Check(fight.Events.Count == 2, "PostHit must dispatch attacker and defender once.");
        Check(fight.Events[0].Side == "player" && !fight.Events[0].Hit.HitEvent.Incoming, "Attacker side/target snapshot wrong.");
        Check(fight.Events[1].Side == "opponent" && fight.Events[1].Hit.HitEvent.Incoming, "Defender side/target snapshot wrong.");
        Check(fight.Events.All(e => e.Hit.Blocked == false && e.Hit.Critical), "Block/critical snapshot changed between sides.");
        Check(fight.Events.All(e => e.Hit.HitEvent.Weapon && e.Hit.HitEvent.Unarmed && !e.Hit.HitEvent.Ranged && !e.Hit.HitEvent.Magic),
            "Recovered animation tags were not mapped independently.");
        Check(fight.Events[0].Hit.TryAddOutgoing(0.10, out var error) && error == "", "PostHit additive damage failed.");
        Check(Math.Abs(strike.EEDJBBOCFNL - 0.30f) < 0.000001 && Math.Abs(fight.Events[1].Hit.Damage - 0.30) < 0.000001,
            "Attacker/defender hit snapshots do not share the pending native strike.");

        fight.Events.Clear();
        animation = new InfoAnimation("RangedMissile", "MagicMissile");
        strike.PBPDKJNKFCJ = animation; strike.EEDJBBOCFNL = 0.4f;
        fight.Hit(new Model.EventModel { KJDFJPBIGJC = fight.Opponent, GAIBPAGPEGK = fight.Player }, strike, ModEffectEvent.HitPostCrit);
        Check(fight.Events.All(e => e.Hit.HitEvent.Ranged && e.Hit.HitEvent.Magic && !e.Hit.HitEvent.Weapon && !e.Hit.HitEvent.Unarmed),
            "Ranged/magic cancellation tags are not exact recovered predicates.");
    }

    private static void CheckReverseSidesAndLocalVersus()
    {
        var fight = new FightHarness { Player = new Model { Name = "player" }, Opponent = new Model { Name = "opponent" } };
        var strike = new Model.StrikeResult { GAIBPAGPEGK = fight.Opponent, PBPDKJNKFCJ = new InfoAnimation("Weapon"), EEDJBBOCFNL = .2f };
        fight.Hit(new Model.EventModel { KJDFJPBIGJC = fight.Player, GAIBPAGPEGK = fight.Opponent }, strike, ModEffectEvent.PostHit);
        Check(fight.Events.Count == 2 && fight.Events[0].Side == "opponent" && !fight.Events[0].Hit.HitEvent.Incoming &&
            fight.Events[1].Side == "player" && fight.Events[1].Hit.HitEvent.Incoming, "Opponent attack side routing is wrong.");
        fight.Events.Clear(); fight.LocalVersus = true;
        fight.Hit(new Model.EventModel { KJDFJPBIGJC = fight.Player, GAIBPAGPEGK = fight.Opponent }, strike, ModEffectEvent.PostHit);
        Check(fight.Events.Count == 0, "Local versus must bypass scripted hit phases.");
    }

    private static void CheckOutgoingArithmetic()
    {
        double damage = .25;
        var hit = new ModIncomingHit(() => damage, value => damage = value, hitEvent: new ModHitEvent(false, true, false, false, false));
        Check(hit.TryScaleOutgoing(1.5, out _) && Math.Abs(damage - .375) < 0.000001, "Existing outgoing scale changed.");
        Check(hit.TryAddOutgoing(.10, out _) && Math.Abs(damage - .475) < 0.000001, "Additive normalized damage changed.");
        double before = damage;
        Check(!hit.TryAddOutgoing(-.01, out _) && damage == before, "Negative additive damage was accepted.");
        Check(!hit.TryAddOutgoing(1.01, out _) && damage == before, "Oversized additive damage was accepted.");
    }

    private static void CheckStatusIcons()
    {
        var fight = new FightHarness { Player = new Model { Name = "player" }, Opponent = new Model { Name = "opponent" } };
        object key = "master";
        Check(fight.Show(fight.Player, key, AssetId.Parse("de128:UI/Skills/IconMasterOfStyle_Blue"), 300, 0, out var error) && error == "",
            "Could not show timed status icon.");
        Check(fight.VisibleAdds == 1 && fight.VisibleRemoves == 0, "Status icon did not use recovered add path once.");
        var action = fight.IconAction(fight.Player, key);
        Check(action != null && action.NHKMCLPOMFK == "de128:UI/Skills/IconMasterOfStyle_Blue" && action.FLNCPBKBJBL && action.FLNLMIHEDCI == 300,
            "Status icon lost sprite/expiration metadata.");
        fight.Advance(299);
        Check(fight.HasIcon(fight.Player, key) && fight.IconAction(fight.Player, key).KGNDJOLBBJF == 299,
            "Status icon expired early or timer did not advance.");
        fight.Advance(1);
        Check(!fight.HasIcon(fight.Player, key) && fight.VisibleRemoves == 1, "300-frame status icon did not expire exactly.");

        key = "relentless";
        Check(fight.Show(fight.Player, key, AssetId.Parse("de128:UI/Skills/IconCrackedApple_Blue"), 300, 15, out _), "Relentless icon show failed.");
        Check(fight.IconAction(fight.Player, key).EclipseStackCount == 15, "Relentless stack count did not reach the native icon action.");
        Check(fight.Show(fight.Player, key, AssetId.Parse("de128:UI/Skills/IconCrackedApple_Blue"), 120, 7, out _), "Status icon refresh failed.");
        Check(fight.VisibleRemoves == 2 && fight.IconAction(fight.Player, key).FLNLMIHEDCI == 120 &&
            fight.IconAction(fight.Player, key).EclipseStackCount == 7,
            "Refresh must clear the old recovered icon and replace its timer/stack count.");
        Check(fight.Clear(fight.Player, key, out _) && !fight.HasIcon(fight.Player, key) && fight.VisibleRemoves == 3,
            "Explicit status-icon clear failed.");
        Check(fight.Clear(fight.Player, key, out _) && fight.VisibleRemoves == 3, "Clearing an absent icon must be idempotent.");
    }
}
