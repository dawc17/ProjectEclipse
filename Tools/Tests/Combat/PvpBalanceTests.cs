using System;
using System.Collections.Generic;
using System.Globalization;
using Eclipse.Multiplayer;
using Eclipse.Multiplayer.Balance;

// Controlled native dependencies; runs the production parser, snapshot, math and combat bridge.
public sealed class ItemInfo { public string Name, SubType; }
public sealed class InfoAnimation { public string Name; }
public sealed class Pair<A,B> { public A First; public B Second; }
public sealed class IntervalAttack {
    public List<Pair<string,float>> Attributes = new List<Pair<string,float>>();
    public List<Pair<string,float>> GetDamageAttributes() => Attributes;
}
public sealed class ModelParameters {
    public ItemInfo Weapon, Ranged, Magic;
    public float MaxLife = 1, RemainingHealthInDamageUnits = 1, RecoverableLife;
}
public sealed class Model {
    public ModelParameters Parameters = new ModelParameters();
    public InfoAnimation Animation = new InfoAnimation { Name = "jab" };
    public InfoAnimation GetCurrentAnimation() => Animation;
}
public sealed class Fight {
    public LocalVersusMatch Definition;
    public LocalVersusMatch GetFightDefinition() => Definition;
    public void UpdateLife(Model model, float amount) {
        var p = model.Parameters;
        p.RemainingHealthInDamageUnits = Math.Clamp(p.RemainingHealthInDamageUnits + amount, 0, p.MaxLife);
        p.RecoverableLife = Math.Clamp(p.RecoverableLife, 0, p.MaxLife - p.RemainingHealthInDamageUnits);
    }
}
namespace Eclipse.Multiplayer {
    public sealed class LocalVersusMatch { public Settings Settings = new Settings(); }
    public sealed class Settings { public PvpBalanceSnapshot Balance; }
}
static class PvpBalanceTests {
    static int checks;
    static void Check(bool ok, string label) { checks++; if (!ok) throw new Exception(label); }
    static void Near(float a, float b, string label) => Check(Math.Abs(a-b) < .000002f, label + ": " + a + " != " + b);
    static void Reject(Action action, string label) {
        try { action(); } catch (Exception) { checks++; return; }
        throw new Exception("Accepted invalid " + label);
    }
    static void Main() {
        var p = new PvpBalanceProfile();
        var defaults = p.Compile();
        Check(defaults.Hash.Length == 64, "SHA256 rules hash");
        p.Name = "Renamed"; p.Id = "copy";
        Check(p.Compile().Hash == defaults.Hash, "metadata does not change gameplay hash");
        p.Categories = new[] { new PvpDamageOverride { Id="Katana", HitDamageScale=.8f }, new PvpDamageOverride { Id="Unarmed", HitDamageScale=.6f } };
        p.Equipment = new[] { new PvpDamageOverride { Id="KATANA_1", BlockedDamageScale=.1f } };
        p.Moves = new[] { new PvpDamageOverride { Id="jab", HitDamageScale=.2f } };
        var rules = p.Compile(); var damage = rules.Resolve("Katana", "KATANA_1", "jab");
        Near(damage.HitDamageScale,.2f,"move replaces category hit");
        Near(damage.BlockedDamageScale,.1f,"equipment block inherited through move");
        Check(damage.HitSource == "Move: jab" && damage.BlockedSource == "Equipment: KATANA_1", "effective source labels");
        Near(rules.Resolve("Katana",null,null).HitDamageScale,.8f,"category lookup");
        Near(rules.Resolve("katana",null,null).HitDamageScale,.5f,"IDs case sensitive");
        Array.Reverse(p.Categories);
        Check(p.Compile().Hash == rules.Hash, "override order canonical");
        string reordered = "{\"moves\":[],\"name\":\"Other\",\"id\":\"other\",\"schemaVersion\":1}";
        Check(PvpBalanceProfile.Parse(reordered).Compile().Hash == defaults.Hash, "JSON order and omitted defaults canonical");
        var priorCulture=CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture=CultureInfo.GetCultureInfo("pl-PL");
        Check(p.Compile().Hash == rules.Hash,"culture independent hash");
        CultureInfo.CurrentCulture=priorCulture;
        p.Moves[0].HitDamageScale = 9;
        var exported = rules.Export(); exported.Moves[0].HitDamageScale=8;
        Near(rules.Resolve("Katana","KATANA_1","jab").HitDamageScale,.2f,"snapshot isolated from author and exported copy");
        Check(p.Compile().Hash != rules.Hash,"changed gameplay changes hash");
        foreach (string invalid in new[] {
            "{}", "{\"schemaVersion\":1,\"id\":\"bad/path\",\"name\":\"a\"}",
            defaults.ToJson().Replace("\"schemaVersion\":1", "\"schemaVersion\":2"),
            defaults.ToJson().Replace("\"schemaVersion\":1", "\"schemaVersion\":1,\"schemaVersion\":1"),
            defaults.ToJson().Replace("\"schemaVersion\":1", "\"schemaVersion\":1,\"typo\":1"),
            defaults.ToJson().Replace("\"categories\":[]", "\"categories\":null"),
            defaults.ToJson().Replace("\"categories\":[]", "\"categories\":[{\"id\":\"a\",\"hitDamageScale\":1},{\"id\":\"a\",\"hitDamageScale\":2}]"),
            defaults.ToJson().Replace("\"categories\":[]", "\"categories\":[{\"id\":\"a\"}]"),
            defaults.ToJson().Replace("\"hitDamageScale\":0.5", "\"hitDamageScale\":\"0.5\""),
            defaults.ToJson().Replace("\"schemaVersion\":1", "\"schemaVersion\":\"1\""),
            defaults.ToJson().Replace("\"id\":\"default\"", "\"id\":123"),
            new string(' ', PvpBalanceProfile.MaxJsonBytes+1)
        }) Reject(()=>PvpBalanceProfile.Parse(invalid),"JSON schema/size/duplicates");
        foreach (float invalid in new[] {float.NaN,float.PositiveInfinity,-.1f,11f}) {
            var bad=new PvpBalanceProfile {HitDamageScale=invalid}; Reject(()=>bad.Compile(),"hit multiplier");
        }
        Reject(()=>new PvpBalanceProfile {MinimumLifeOnBlock=0}.Compile(),"lethal block floor");
        Reject(()=>new PvpBalanceProfile {RecoverableBlockedFraction=1.1f}.Compile(),"recoverable fraction");

        float life=1, pool=0;
        for(int i=0;i<1000;i++) {
            float before=life;
            float capped=PvpRecoverableHealth.ClampBlockedDamage(.17f,life,1,defaults.MinimumLifeOnBlock);
            life-=capped;
            pool=PvpRecoverableHealth.PoolAfterDamage(pool,life,1,before-life,true,defaults);
            Check(life>0 && pool>=0 && pool<=1-life+.000001f,"repeated blocked hit nonlethal/pool bounded");
        }
        Near(life,.0001f,"configured block floor"); Near(pool,1-life,"only actual chip enters grey pool");
        Near(PvpRecoverableHealth.ClampBlockedDamage(1,.00001f,1,.1f),0,"block below floor neither damages nor heals");
        Near(PvpRecoverableHealth.Recovery(.2f,.5f,1,.2f,false,defaults),.1f,"on hit recovery");
        Near(PvpRecoverableHealth.Recovery(.2f,.5f,1,.2f,true,defaults),.05f,"on block recovery");
        Near(PvpRecoverableHealth.Recovery(0,.5f,1,1,false,defaults),0,"no grey pool no heal");
        Near(PvpRecoverableHealth.Recovery(.2f,0,1,1,false,defaults),0,"no revival");
        Near(PvpRecoverableHealth.Recovery(.2f,.95f,1,1,false,defaults),.05f,"max health cap");
        var partial=new PvpBalanceProfile {RecoverableBlockedFraction=.5f,RecoverOnHit=0,RecoverOnBlock=0,RecoverableLossOnHit=1}.Compile();
        Near(PvpRecoverableHealth.PoolAfterDamage(0,.8f,1,.2f,true,partial),.1f,"partial recoverable chip");
        Near(PvpRecoverableHealth.PoolAfterDamage(.2f,.5f,1,.1f,false,partial),.1f,"incoming hit grey loss");
        Near(PvpRecoverableHealth.PoolAfterDamage(.2f,0,1,.7f,false,defaults),0,"KO clears pool");
        Near(PvpRecoverableHealth.Recovery(.2f,.5f,1,1,false,partial),0,"recovery disabled by profile");

        var fight=new Fight {Definition=new LocalVersusMatch()}; fight.Definition.Settings.Balance=rules;
        var attacker=new Model(); attacker.Parameters.Weapon=new ItemInfo {Name="KATANA_1",SubType="Katana"};
        attacker.Parameters.Magic=new ItemInfo {Name="SPELL",SubType="Magic"};
        var defender=new Model(); var attack=new IntervalAttack();
        Near(PvpBalanceCombat.ScaleDamage(fight,1,false,attacker,attack,defender),.2f,"production move scaling");
        Near(PvpBalanceCombat.ScaleDamage(fight,1,true,attacker,attack,defender),.1f,"production equipment scaling");
        attacker.Animation.Name="fireball"; attack.Attributes.Add(new Pair<string,float> {First="MagicDamage"});
        Near(PvpBalanceCombat.ScaleDamage(fight,1,true,attacker,attack,defender),.25f,"magic does not inherit equipped weapon override");
        Near(PvpBalanceCombat.ScaleDamage(new Fight(),1,true,attacker,attack,defender),1,"campaign damage unchanged");
        fight.Definition.Settings.Balance=defaults;
        defender.Parameters.RemainingHealthInDamageUnits=.8f;
        attacker.Parameters.RemainingHealthInDamageUnits=.6f; attacker.Parameters.RecoverableLife=.15f;
        PvpBalanceCombat.AfterStrike(fight,defender,attacker,true,1);
        Near(defender.Parameters.RecoverableLife,.2f,"production blocked hit grey");
        Near(attacker.Parameters.RemainingHealthInDamageUnits,.65f,"production recovery uses actual damage");
        Near(attacker.Parameters.RecoverableLife,.1f,"production recovery consumes pool");
        // Native health setters themselves clamp the pool; avoid consuming recovery twice.
        attacker.Parameters.RemainingHealthInDamageUnits=.8f; attacker.Parameters.RecoverableLife=.2f;
        defender.Parameters.RemainingHealthInDamageUnits=.4f;
        PvpBalanceCombat.AfterStrike(fight,defender,attacker,false,.6f);
        Near(attacker.Parameters.RemainingHealthInDamageUnits,.9f,"native-like setter heal");
        Near(attacker.Parameters.RecoverableLife,.1f,"no double spending against native setter");
        Console.WriteLine("PASS: " + checks + " production PvP balance/recovery checks (controlled native dependencies; no game playtest).");
    }
}
