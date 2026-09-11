using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using Eclipse.Modding;

// Asset mounting is isolated; the session, registration and Lua contexts are production code.
namespace Eclipse.Modding {
    public sealed class ModHost {
        public IReadOnlyList<ModDescriptor> EnabledMods { get; set; }
        public AssetResolver Assets { get; set; }
    }
}

public sealed class Core : IAssetProvider {
    public ModId Namespace => ModId.Parse("core");
    public bool TryDescribe(AssetId id, out AssetMetadata metadata) {
        metadata = new AssetMetadata(id, AssetKind.Sprite, AssetSourceKind.Core, "", -1, "fixture"); return true;
    }
}
public sealed class Fighter : IModFighterOperations, IModIncomingHitSource, IModCombatSnapshotSource, IModCombatActivitySource {
    public ModCombatActivityEvent ActivityEvent { get; set; }
    public Func<ModCombatSnapshot> Capture;
    public ModCombatSnapshot CaptureCombatSnapshot() => Capture?.Invoke();
    public ModIncomingHit IncomingHit { get; set; }
    public bool TryChangeHealth(double n,out string error) { error=""; return true; }
    public bool TryAddMagicCharge(double n,out string error) { error=""; return true; }
}
public static class Program {
    static int checks;
    static void Check(bool value,string message) { checks++; if(!value) throw new Exception(message); }
    public static void Main(string[] args) {
        string entry=Path.Combine(args[0],"example.battle-rules/scripts/main.lua");
        string original=File.ReadAllText(entry);
        string testManifest=Path.Combine(args[0],"example.battle-rules/mod.toml");
        File.WriteAllText(testManifest,File.ReadAllText(testManifest).Replace("\"content.register\"","\"content.register\", \"combat.modify_outgoing_hit\""));
        // Public Lua validation, not direct construction of internal DTOs.
        string probes = @"
local tick_reader
sf2.behaviors.register {
    id='tick_probe',
    state={lifetime='round',fields={count={type=sf2.behaviors.INTEGER,default=0}}},
    on_tick=function(self,fighter,event)
        assert(event.type=='Tick' and event.delta_frames==1 and event.delta_seconds==1/60)
        assert(event.frame==120 or event.frame==121 or event.frame==180)
        assert(event.seconds==event.frame/60 and fighter:snapshot().frame==event.frame)
        assert(fighter.scale_outgoing_damage==nil and fighter.scale_incoming_damage==nil)
        self.state.count=self.state.count+1
        assert(self.state.count==(event.frame==121 and 2 or 1))
        tick_reader=fighter.snapshot
        event.frame=-99
    end,
    on_round_end=function() tick_reader() end,
}
local previous_combo
sf2.behaviors.register {
    id='activity_probe',
    on_combo_changed=function(_,fighter,event)
        assert(fighter.scale_outgoing_damage==nil and fighter.scale_incoming_damage==nil)
        assert(event.combo==3 or event.combo==0)
        assert(event.last_combo==3)
        if previous_combo then assert(previous_combo.combo==99 and event.combo==0) end
        previous_combo=event;event.combo=99
    end,
    on_style_changed=function(_,fighter,event)
        assert(event.style_rank==2 and event.style_name=='FixtureStyle')
        assert(event.style_gain==0.25 and event.is_hit)
    end,
}
local retained_outgoing
sf2.behaviors.register {
    id='outgoing_probe',
    on_damage_dealing=function(_,fighter,event)
        assert(event.damage==10 and event.blocked and not event.critical)
        assert(fighter.scale_incoming_damage==nil)
        fighter:scale_outgoing_damage(2)
        fighter:scale_outgoing_damage(1.5)
        assert(event.damage==10)
        retained_outgoing=fighter.scale_outgoing_damage
    end,
    on_round_end=function() retained_outgoing(2) end,
}
local upgrade_perk=sf2.perks.register {
    id='upgraded',behavior=cycle,kind='single',
    display_name=sf2.localization.key('trial'),description=sf2.localization.key('description'),
    parameters={every=3},
    upgrades={{level=1,parameters={every=2}},{level=2,parameters={every=1}}},
}
local previous_snapshot, previous_reader
sf2.behaviors.register {
    id = 'snapshot_probe',
    on_round_begin = function(_, fighter)
        local first = fighter:snapshot()
        assert(first.self.health == 80 and first.self.max_health == 100)
        assert(first.self.health_bars == 3 and first.self.position.x == -25)
        assert(first.self.position.y == 4 and first.self.position.z == 2)
        assert(first.opponent.health == 50 and first.opponent.health_bars == 1)
        assert(first.frame == 120 and first.seconds == 2 and first.round_active)
        first.self.position.x = 999
        first.self.health = -1
        first.opponent.health = -1
        local fresh = fighter:snapshot()
        assert(fresh.self.health == 70 and fresh.self.position.x == -25)
        assert(fresh.opponent.health == 50 and first.self.health == -1)
        previous_snapshot, previous_reader = fresh, fighter.snapshot
    end,
    on_round_end = function() previous_reader() end,
    on_fight_end = function(_, fighter)
        assert(previous_snapshot.self.health == 70)
        local final = fighter:snapshot()
        assert(final.self.health == 0 and final.opponent == nil)
        assert(final.frame == 180 and final.seconds == 3 and not final.round_active)
    end,
}
sf2.behaviors.register {
    id = 'snapshot_unavailable',
    on_round_begin = function(_, fighter) assert(fighter:snapshot() == nil) end,
}
";
        foreach (string invalid in new[] {
            "sf2.rules.behavior {id='missing',behavior=cycle}",
            "sf2.rules.behavior {id='wrong',behavior=cycle,parameters={every='bad'}}",
            "sf2.rules.behavior {id='unknown',behavior=cycle,parameters={every=1},operation='add'}",
            "sf2.rules.behavior {id='target',behavior=cycle,parameters={every=1},target='nobody'}",
            "sf2.rules.behavior {id='round',behavior=cycle,parameters={every=1},rounds={0}}",
            "local saved=sf2.behaviors.register {id='saved',on_round_begin=function() end,state={lifetime='saved',fields={n={type=sf2.behaviors.INTEGER,default=0}}}}; sf2.rules.behavior {id='saved_rule',behavior=saved}"
        }) {
            File.WriteAllText(entry,original+"\n"+invalid);
            Reject(args[0]);
        }
        File.WriteAllText(entry,original+probes);
        string baseline=Run(args[0],true);
        File.WriteAllText(entry,(original+probes).Replace("target = sf2.rules.OPPONENT,", "target = sf2.rules.ALL, mode = sf2.rules.ECLIPSE, rounds = { 2 },"));
        Run(args[0],false,3,false,true);
        File.WriteAllText(entry,(original+probes).Replace("every = 3","every = 4"));
        string changed=Run(args[0],false,4);
        Check(baseline!=changed,"Rule parameters absent from fingerprint");
        File.WriteAllText(entry,original+probes);
        string manifest=Path.Combine(args[0],"example.battle-rules/mod.toml");
        File.WriteAllText(manifest,File.ReadAllText(manifest).Replace(", \"combat.modify_hit\"",""));
        Run(args[0],false,3,true);
        File.WriteAllText(manifest,File.ReadAllText(manifest).Replace("\"combat.modify_outgoing_hit\"","\"combat.modify_hit\""));
        Run(args[0],false,outgoingDenied:true);
        Console.WriteLine("PASS: "+checks+" battle-rule checks; real Lua registration, filtering, per-rule/side/round/fight isolation, damage mutation, fingerprint, capability rejection.");
    }
    static void Reject(string mods) {
        var mod=ModDiscovery.DiscoverLoose(mods).Mods.Single();
        var catalog=new ModContentCatalog();
        var xml=new XmlDocument();xml.LoadXml("<Templates><Warrior Name='Default'/></Templates>");
        CoreContentImporter.ImportWarriorTemplates(catalog,xml.DocumentElement);
        var assets=new AssetResolver(new IAssetProvider[]{new Core(),new LooseModProvider(mod)});
        bool rejected=false;
        using(var tx=catalog.BeginRegistration(mod)) {
          ModLocalizationLoader.Load(mod,assets,tx);
          using(var script=new MoonSharpScriptRuntime().CreateContext(mod,new ModApiFacade(mod,assets,tx,new ModStateRuntime(),null))) {
            try { script.ExecuteEntrypoint(); tx.Commit(); }
            catch(ModScriptException) { rejected=true; }
          }
        }
        Check(rejected && catalog.FightRules.Count==0,"Invalid rule committed partial content");
    }
    static string Run(string mods,bool filters,int every=3,bool forbidden=false,bool filtersOnly=false,bool outgoingDenied=false) {
        var discovery=ModDiscovery.DiscoverLoose(mods);
        Check(!discovery.HasErrors && discovery.Mods.Count==1,"Discovery");
        var mod=discovery.Mods[0];var catalog=new ModContentCatalog();
        var xml=new XmlDocument();xml.LoadXml("<Templates><Warrior Name='Default'/></Templates>");
        CoreContentImporter.ImportWarriorTemplates(catalog,xml.DocumentElement);
        var assets=new AssetResolver(new IAssetProvider[]{new Core(),new LooseModProvider(mod)});
        using(var tx=catalog.BeginRegistration(mod)) {
          ModLocalizationLoader.Load(mod,assets,tx);
          using(var script=new MoonSharpScriptRuntime().CreateContext(mod,new ModApiFacade(mod,assets,tx,new ModStateRuntime(),null))) {
            script.ExecuteEntrypoint();tx.Commit();
            if (filters) {
                var ownedUi=((IModUiScriptContext)script).UiScope;
                Check(ownedUi.Owner==mod.Id && !ownedUi.IsClosed,"Script UI ownership missing");
            }
            Check(catalog.FightRules.Count==1,"Rejected registrations leaked rules");
            var rule=catalog.FightRules[0];var fight=catalog.Fights.Single();
            string runtimeId=catalog.RuntimeFightId(fight.Id);
            var instances=new ModBattleRuleInstances();
            if(filtersOnly) {
                Check(!instances.Applicable(catalog,runtimeId,false,1,true).Any(),"Rule ignored round filter");
                Check(!instances.Applicable(catalog,runtimeId,false,2,false).Any(),"Rule ignored Eclipse filter");
                Check(instances.Applicable(catalog,runtimeId,false,2,true).Single()==rule,"Filtered opponent missing");
                Check(instances.Applicable(catalog,runtimeId,true,2,true).Single()==rule,"ALL omitted player");
                return ModSaveData.ComputeContentSetFingerprint(new[]{mod},catalog);
            }
            Check(instances.Applicable(catalog,runtimeId,false,1,false).Single()==rule,"Attached rule missing");
            Check(!instances.Applicable(catalog,runtimeId,true,1,false).Any(),"Opponent target leaked to player");
            Check(new ModBattleRuleInstances().Applicable(catalog,"unrelated|fight|1",false,1,false).Count()==0,"Rule leaked to another fight");
            var interactive=(IModInteractiveBehaviorScriptContext)script;
            var upgraded=catalog.Perks.Single();
            var saved=new Dictionary<string,ModParameterValue>{{"every",ModParameterValue.FromInteger(9)}};
            Check(upgraded.ResolveUpgradeParameters(0,saved)["every"].Integer==9,"Base upgrade lost saved parameter");
            Check(upgraded.ResolveUpgradeParameters(1,saved)["every"].Integer==2,"Upgrade override missing");
            Check(upgraded.ResolveUpgradeParameters(2,saved)["every"].Integer==1 && saved["every"].Integer==9,"Upgrade modified saved parameter");
            bool futureRejected=false;
            try { upgraded.ResolveUpgradeParameters(3,saved); } catch(ModContentException) { futureRejected=true; }
            Check(futureRejected,"Unknown upgrade level silently accepted");
            var savedPerk=new XmlDocument();savedPerk.LoadXml("<Perk UpgradeLevel='1'><SavedRoll every='9'/></Perk>");
            string savedXml=savedPerk.OuterXml;
            Check(upgraded.ResolveSavedUpgradeParameters(savedPerk.DocumentElement,saved)["every"].Integer==2,"Saved level selection failed");
            Check(savedPerk.OuterXml==savedXml && saved["every"].Integer==9,"Saved upgrade query mutated source");
            var reloaded=new XmlDocument(); reloaded.LoadXml(savedXml);
            Check(upgraded.ResolveSavedUpgradeParameters(reloaded.DocumentElement,saved)["every"].Integer==2,"Reload lost upgrade");
            foreach (string bad in new[]{"-1","1.5","unknown","2147483648","3"}) {
                savedPerk.DocumentElement.SetAttribute("UpgradeLevel",bad); string before=savedPerk.OuterXml;
                bool rejected=false; try { upgraded.ResolveSavedUpgradeParameters(savedPerk.DocumentElement,saved); } catch(ModContentException) { rejected=true; }
                Check(rejected && before==savedPerk.OuterXml,"Malformed/future upgrade modified saved data: "+bad);
            }
            savedPerk.DocumentElement.RemoveAttribute("UpgradeLevel");
            Check(upgraded.ResolveSavedUpgradeParameters(savedPerk.DocumentElement,saved)["every"].Integer==9,"Missing level must use base saved parameters");
            if (filters) SnapshotChecks(interactive, mod.Id);
            if (filters) ActivityChecks(interactive,mod.Id);
            if (filters) TickChecks(interactive,mod.Id);
            if (filters) SubscriptionChecks(mod,assets);
            if (filters || outgoingDenied) OutgoingChecks(interactive, mod.Id,outgoingDenied);
            for(int hit=1;hit<=every*2;hit++) {
                double damage=Hit(interactive,rule,instances,false,1,forbidden);
                Check(forbidden ? damage==10 : damage==(hit%every==0?10:0),"Third-strike progression or capability isolation");
            }
            if(!forbidden) {
                Check(Hit(interactive,rule,instances,false,2,false)==0,"Round state failed to reset");
                Check(Hit(interactive,rule,new ModBattleRuleInstances(),false,1,false)==0,"New fight inherited state");
                Check(!ReferenceEquals(instances.Instance(rule.Id,true),instances.Instance(rule.Id,false)),"Side instances share state");
                Check(!ReferenceEquals(instances.Instance(rule.Id,false),instances.Instance(DefinitionId.Parse(mod.Id+":rules/other"),false)),"Rule instances share state");
            }
            if(filters) {
                var weakened=new ModCombatSnapshot(new ModFighterSnapshot(25,100,1,0,0,0),null,120,true);
                Check(Hit(interactive,rule,new ModBattleRuleInstances(),false,1,false,weakened)==10,"Low-health guard transition ignored snapshot");
                using(var extra=catalog.BeginRegistration(mod)) {
                    var filtered=extra.RegisterBehaviorRule("filtered",rule.Behavior,ModRuleTarget.All,ModRuleMode.Eclipse,new[]{2},rule.InitialParameters);
                    Check(filtered.Rounds.Count==1,"Round filter lost");
                    var separate = new ModBattleRuleInstances();
                    Check(Hit(interactive,rule,separate,false,1,false)==0,"First original-rule hit");
                    Check(Hit(interactive,rule,separate,false,1,false)==0,"Second original-rule hit");
                    Check(Hit(interactive,filtered,separate,false,1,false)==0,"Shared behavior leaked state across rules");
                    Check(Hit(interactive,rule,separate,false,1,false)==10,"Independent rule reset original state");
                    Check(Hit(interactive,rule,separate,true,1,false)==0,"Shared behavior leaked state across sides");
                    bool rejected=false;
                    try { extra.RegisterBehaviorRule("dependency",DefinitionId.Parse("other:behaviors/b"),ModRuleTarget.All,ModRuleMode.All,null,null); }
                    catch(ModContentException) { rejected=true; }
                    Check(rejected,"Undeclared behavior accepted");
                }
            }
            if (filters) {
                var ui=((IModUiScriptContext)script).UiScope;
                var panel=ui.Open("owned",ModUiMount.Menu,new ModUiNode("root",ModUiKind.Text,100,40,text:"Owned"));
                script.Dispose();
                Check(ui.IsClosed && panel.IsClosed && ui.Count==0,"Script disposal retained owned UI");
            }
            return ModSaveData.ComputeContentSetFingerprint(new[]{mod},catalog);
          }
        }
    }
    static void SubscriptionChecks(ModDescriptor mod,AssetResolver assets) {
        var host=new ModHost{EnabledMods=new[]{mod},Assets=assets};
        var session=ModScriptSession.Start(host,new MoonSharpScriptRuntime(),null,content=>{
            var xml=new XmlDocument();xml.LoadXml("<Templates><Warrior Name='Default'/></Templates>");
            CoreContentImporter.ImportWarriorTemplates(content,xml.DocumentElement);
        });
        Check(!session.HasErrors,"Session subscription fixture initialization failed: "+session.FormatReport());
        var id=DefinitionId.Parse(mod.Id+":behaviors/tick_probe");
        Check(session.HasHandlers(ModEffectEvent.Tick) && session.HasBehaviorHandler(id,ModEffectEvent.Tick),"Tick subscription missing");
        Check(!session.HasBehaviorHandler(id,ModEffectEvent.DamageDealing),"Unregistered callback subscribed");
        Check(!session.HasBehaviorHandler(DefinitionId.Parse("missing:behaviors/probe"),ModEffectEvent.Tick),"Inactive owner subscribed");
        session.Dispose();
        Check(!session.HasHandlers(ModEffectEvent.Tick) && !session.HasBehaviorHandler(id,ModEffectEvent.Tick),"Disposed subscription retained");
    }
    static void TickChecks(IModInteractiveBehaviorScriptContext context, ModId mod) {
        var id=DefinitionId.Parse(mod+":behaviors/tick_probe");
        var xml=new XmlDocument();xml.LoadXml("<Rule/>");
        int frame=120;bool active=true;
        var fighter=new Fighter{Capture=()=>new ModCombatSnapshot(new ModFighterSnapshot(1,1,1,0,0,0),null,frame,active)};
        var wrapped=new ModInstanceFighter(fighter,xml.DocumentElement);
        var fields=new Dictionary<string,string>{{"source","rule"},{"round","1"},{"fight_id","tick-fixture"}};
        Check(context.TryInvokeBehavior(id,ModEffectEvent.Tick,null,fields,wrapped,out var error),error);
        frame=121;Check(context.TryInvokeBehavior(id,ModEffectEvent.Tick,null,fields,wrapped,out error),error);
        Check(!context.TryInvokeBehavior(id,ModEffectEvent.RoundEnd,null,fields,wrapped,out error),"Tick query survived callback exit");
        frame=180;fields["round"]="2";
        Check(context.TryInvokeBehavior(id,ModEffectEvent.Tick,null,fields,wrapped,out error),error);
        active=false;Check(!context.TryInvokeBehavior(id,ModEffectEvent.Tick,null,fields,wrapped,out error),"Inactive tick accepted");
        active=true;frame=0;Check(!context.TryInvokeBehavior(id,ModEffectEvent.Tick,null,fields,wrapped,out error),"Pre-clock tick accepted");
        fighter.Capture=null;Check(!context.TryInvokeBehavior(id,ModEffectEvent.Tick,null,fields,wrapped,out error),"Missing tick clock accepted");
    }
    static void ActivityChecks(IModInteractiveBehaviorScriptContext context, ModId mod) {
        var id=DefinitionId.Parse(mod+":behaviors/activity_probe");
        var fighter=new Fighter{ActivityEvent=ModCombatActivityEvent.ComboChange(3,3)};
        var node=new XmlDocument();node.LoadXml("<Rule/>");var wrapped=new ModInstanceFighter(fighter,node.DocumentElement);
        Check(context.TryInvokeBehavior(id,ModEffectEvent.ComboChanged,null,null,wrapped,out var error),error);
        Check(fighter.ActivityEvent.Combo==3,"Lua mutated combo snapshot source");
        fighter.ActivityEvent=ModCombatActivityEvent.ComboChange(0,3);
        Check(context.TryInvokeBehavior(id,ModEffectEvent.ComboChanged,null,null,wrapped,out error),error);
        fighter.ActivityEvent=ModCombatActivityEvent.StyleChange(2,"FixtureStyle",0.25,true);
        Check(context.TryInvokeBehavior(id,ModEffectEvent.StyleChanged,null,null,wrapped,out error),error);
        Check(!context.TryInvokeBehavior(id,ModEffectEvent.ComboChanged,null,null,wrapped,out error),"Mismatched activity accepted");
        fighter.ActivityEvent=null;
        Check(!context.TryInvokeBehavior(id,ModEffectEvent.StyleChanged,null,null,wrapped,out error),"Missing activity accepted");
    }

    static void OutgoingChecks(IModInteractiveBehaviorScriptContext context, ModId mod,bool denied) {
        double damage=10;
        var pending=new ModIncomingHit(()=>damage,n=>damage=n,true,false);
        var fighter=new Fighter { IncomingHit=pending };
        var id=DefinitionId.Parse(mod+":behaviors/outgoing_probe");
        bool success=context.TryInvokeBehavior(id,ModEffectEvent.DamageDealing,null,null,fighter,out var error);
        if (denied) { Check(!success && damage==10 && error.Contains("combat.modify_outgoing_hit"),"Missing outgoing capability changed hit"); return; }
        Check(success,"Outgoing callback: "+error);
        Check(damage==30,"Outgoing modifiers did not compose");
        Check(!context.TryInvokeBehavior(id,ModEffectEvent.RoundEnd,null,null,fighter,out error) && error.Contains("expired") && damage==30,"Retained outgoing operation modified hit");
        foreach(double invalid in new[]{-1d,17d,double.NaN,double.PositiveInfinity})
            Check(!pending.TryScaleOutgoing(invalid,out error) && damage==30,"Invalid outgoing multiplier changed hit");
        damage=float.MaxValue;
        Check(!pending.TryScaleOutgoing(2,out error) && damage==float.MaxValue,"Outgoing overflow changed hit");
        damage=0;Check(pending.TryScaleOutgoing(16,out error) && damage==0,"Outgoing resurrected zero damage");
    }

    static void SnapshotChecks(IModInteractiveBehaviorScriptContext context, ModId mod) {
        int captures=0;
        var opponent=new ModFighterSnapshot(50,100,1,25,4,2);
        var fighter=new Fighter { Capture=()=>new ModCombatSnapshot(
            new ModFighterSnapshot(++captures==1?80:70,100,3,-25,4,2),opponent,120,true) };
        var xml=new XmlDocument(); xml.LoadXml("<Instance/>");
        var wrapped=new ModInstanceFighter(fighter,xml.DocumentElement);
        var id=DefinitionId.Parse(mod+":behaviors/snapshot_probe");
        Check(context.TryInvokeBehavior(id,ModEffectEvent.RoundBegin,null,null,wrapped,out var error),"Snapshot observations: "+error);
        Check(captures==2 && opponent.Health==50,"Fresh read or native snapshot isolation failed");
        Check(!context.TryInvokeBehavior(id,ModEffectEvent.RoundEnd,null,null,wrapped,out error) && error.Contains("expired"),"Retained snapshot operation survived callback");
        Check(captures==2,"Expired observation reached native source");
        fighter.Capture=()=>new ModCombatSnapshot(new ModFighterSnapshot(0,100,3,-25,4,2),null,180,false);
        Check(context.TryInvokeBehavior(id,ModEffectEvent.FightEnd,null,null,wrapped,out error),"Retained value or final snapshot: "+error);
        fighter.Capture=null;
        Check(context.TryInvokeBehavior(DefinitionId.Parse(mod+":behaviors/snapshot_unavailable"),ModEffectEvent.RoundBegin,null,null,wrapped,out error),"Unavailable snapshot: "+error);
        foreach (double invalid in new[]{double.NaN,double.PositiveInfinity,double.NegativeInfinity}) {
            bool rejected=false;
            try { new ModFighterSnapshot(10,100,1,invalid,0,0); } catch(ArgumentOutOfRangeException) { rejected=true; }
            Check(rejected,"Non-finite snapshot coordinate accepted");
        }
        bool badFrame=false;
        try { new ModCombatSnapshot(opponent,null,-1,false); } catch(ArgumentOutOfRangeException) { badFrame=true; }
        Check(badFrame,"Negative simulation clock accepted");
    }

    static double Hit(IModInteractiveBehaviorScriptContext context,FightRuleDefinition rule,ModBattleRuleInstances instances,bool player,int round,bool forbidden,ModCombatSnapshot snapshot=null) {
        double damage=10;
        var fighter=new Fighter{IncomingHit=new ModIncomingHit(()=>damage,v=>damage=v),Capture=()=>snapshot};
        var fields=new Dictionary<string,string>{{"source","rule"},{"rule_id",rule.Id.ToString()},{"side",player?"player":"opponent"},{"fight_id","test-fight"},{"round",round.ToString()}};
        bool ok=context.TryInvokeBehavior(rule.Behavior,ModEffectEvent.DamageResolving,rule.InitialParameters,fields,new ModInstanceFighter(fighter,instances.Instance(rule.Id,player)),out var error);
        Check(forbidden?!ok:ok,"Callback: "+error);
        return damage;
    }
}
