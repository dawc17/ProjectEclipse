using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

static class Program
{
    static int checks;
    static string root,repo;
    const string Target="core:fights/zone_1/tournament/3";
    static void Check(bool value,string message) {checks++;if(!value)throw new Exception(message);}
    static ModDescriptor Descriptor(string capabilities)
    {
        var folder=Path.Combine(root,Guid.NewGuid().ToString("N"),"example.objective");Directory.CreateDirectory(Path.Combine(folder,"scripts"));
        File.WriteAllText(Path.Combine(folder,"scripts/main.lua"),"");
        File.WriteAllText(Path.Combine(folder,"mod.toml"),"schema=1\nid=\"example.objective\"\nname=\"Objective\"\nversion=\"1.0.0\"\nauthors=[\"Fixture\"]\nentrypoint=\"scripts/main.lua\"\ncapabilities=["+capabilities.Replace("'","\"")+"]\n[[dependencies]]\nid=\"core\"\nversion=\">=1.0.0 <2.0.0\"\n");
        var discovery=ModDiscovery.DiscoverLoose(Path.GetDirectoryName(folder));
        if(discovery.Diagnostics.Count>0)throw new Exception(string.Join("; ",discovery.Diagnostics));
        return discovery.Mods.Single();
    }
    static ModContentCatalog Catalog()
    {
        var content=new ModContentCatalog();var xml=new XmlDocument();xml.Load(Path.Combine(repo,"Assets/vanillaXml/stages.xml"));
        CoreContentImporter.ImportStages(content,xml.SelectSingleNode("Stages/Zones"));return content;
    }
    sealed class Loaded : IDisposable
    {
        public ModContentCatalog Content;
        public IModScriptContext Context;
        public FightRuleDefinition Rule;
        public ModDescriptor Mod;
        public List<ModUiSurface> Surfaces=new List<ModUiSurface>();
        public void Dispose()=>Context.Dispose();
    }
    static Loaded Load(string body="assert(fighter:end_round('win'))",bool controls=true,string caps="'content.register','content.patch','combat.round_outcome'",string field=null)
    {
        var mod=Descriptor(caps);var content=Catalog();var tx=content.BeginRegistration(mod);
        IModScriptContext context=null;
        try
        {
            File.WriteAllText(Path.Combine(mod.RootPath,"scripts/main.lua"),"local sf2=require('sf2');local saved; local behavior=sf2.behaviors.register{id='goal',on_tick=function(_,fighter) "+body+" end,on_round_begin=function(_,fighter) "+body+" end}; local rule=sf2.rules.behavior{id='goal',behavior=behavior,target='player',controls_outcome="+(field??(controls?"true":"false"))+"};sf2.fights.patch{target='"+Target+"',append_rules={rule}}");
            var api=new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx,new ModStateRuntime(),null);
            context=new MoonSharpScriptRuntime().CreateContext(mod,api);
            context.ExecuteEntrypoint();
            tx.Commit();return new Loaded{Content=content,Context=context,Rule=content.FightRules.Single(),Mod=mod};
        }
        catch {context?.Dispose();throw;}
        finally {tx.Dispose();}
    }
    static Fight Native(Loaded loaded)
    {
        ModRuntime.Scripts=new FixtureScripts{Content=loaded.Content};ListSF.Instance.Eclipse=false;ModModeRuntime.OfflineRaid=false;
        var fight=new Fight();fight.FightDefinition.FightId=loaded.Content.RuntimeFightId(DefinitionId.Parse(Target));return fight;
    }
    static bool Invoke(Loaded loaded,Fight fight,out string error,ModEffectEvent kind=ModEffectEvent.Tick,bool authority=true)
    {
        var node=new XmlDocument();node.LoadXml("<BattleRuleInstance/>");
        var wrapped=new ModInstanceFighter(fight,node.DocumentElement,authority?loaded.Rule:null);
        return ((IModInteractiveBehaviorScriptContext)loaded.Context).TryInvokeBehavior(loaded.Rule.Behavior,kind,null,
            new Dictionary<string,string>{{"source","rule"},{"round","1"},{"side","player"}},wrapped,out error);
    }
    static void StartupRejected(Action action,string contains)
    {
        try {action();throw new Exception("Expected startup rejection: "+contains);}
        catch(ModScriptException error){Check(error.Message.Contains(contains),error.ToString());}
    }
    static void Main(string[] args)
    {
        root=args[0];repo=args[1];
        StartupRejected(()=>Load(caps:"'content.register','content.patch'"),"combat.round_outcome");
        StartupRejected(()=>Load(field:"'true'"),"boolean");
        foreach(var value in new[]{"'draw'","'WIN'","true","nil","{}"})
        {
            using var loaded=Load("fighter:end_round("+value+")");var fight=Native(loaded);
            Check(!Invoke(loaded,fight,out var error),"Invalid outcome accepted: "+value);
            Check(!fight._eclipseRoundOutcomes.PendingPlayerWins.HasValue,"Invalid outcome queued a result");
        }
        using(var loaded=Load())
        {
            var fight=Native(loaded);fight.Player.Health=.1f;fight.Enemy.Health=.9f;
            Check(Invoke(loaded,fight,out var error),error);
            Check(fight.Player.RoundsWon==0 && fight.round.processing,"Callback settled a round recursively");
            Check(fight.TryEndRound(loaded.Rule.Id,true,out error),"Identical request was not idempotent");
            Check(!fight.TryEndRound(loaded.Rule.Id,false,out error) && error.Contains("different"),"Conflicting pending result replaced first request");
            fight.Step();
            Check(fight.Player.IsWinner && fight.Player.RoundsWon==1 && !fight.Enemy.IsWinner,"Native score ignored explicit player win");
            Check(fight.Winner()==fight.Player,"End stance winner reverted to higher health");
            Check(fight.Player.Health==.1f && fight.Enemy.Health==.9f,"Custom result changed health");
            Check(!fight.round.processing && fight.preFight.ScoreUpdates==1 && fight._rulesInspector.Stops==1,"Native round boundary repeated or omitted");
            fight.Step();Check(fight.Player.RoundsWon==1,"Duplicate simulation step scored twice");
            fight.CompleteStance();fight.Step();fight.Step();
            Check(fight.Settlements==1 && fight.RoundEndEvents==1,"Result lifecycle repeated after a custom outcome");
            fight.ResetForNextRound();Check(!fight._eclipseRoundOutcomes.ResolvedPlayerWins.HasValue,"Next round retained custom winner");
            Check(fight.TryEndRound(loaded.Rule.Id,false,out error),error);fight.Step();
            Check(fight.Enemy.RoundsWon==1 && fight.Winner()==fight.Enemy,"Explicit loss not honored");
        }
        using(var loaded=Load())
        {
            foreach(var native in new[]{"lethal","timeout","rule","surrender"})
            {
                var fight=Native(loaded);Check(Invoke(loaded,fight,out var error),error);
                if(native=="lethal") {fight.Player.Health=0;fight.Player.PCALDKCJGCK=true;}
                if(native=="timeout") fight.preFight.Timeout=true;
                if(native=="rule") {fight._endFightRule=new Rule{Target=RuleAppliance.ApplianceOpponent};fight._endRoundType=EndRoundType.EndRoundTypeLose;}
                if(native=="surrender") fight.Surrender();else fight.Step();
                Check(!fight._eclipseRoundOutcomes.PendingPlayerWins.HasValue && !fight._eclipseRoundOutcomes.ResolvedPlayerWins.HasValue,"Custom outcome overrode "+native);
                Check(native=="surrender"?fight.Settlements==1: fight.Enemy.IsWinner,"Native "+native+" did not win precedence");
                Check(!fight.TryEndRound(loaded.Rule.Id,true,out error),"Outcome accepted after "+native);
            }
            foreach(var guard in new[]{"pause","versus","training","title","online-raid","pvp","stale","end-stage","fight-end"})
            {
                var fight=Native(loaded);
                if(guard=="pause")fight.Paused=true;if(guard=="versus")fight.IsLocalVersus=true;
                if(guard=="training")fight.FightDefinition.Type=BattleType.FightNone;if(guard=="title")fight.IsTitleSparring=true;
                if(guard=="online-raid")fight.FightDefinition.Type=BattleType.FightRaid;if(guard=="pvp")fight.FightDefinition.Type=BattleType.FightPVP;
                if(guard=="stale")Fight.Current=null;if(guard=="end-stage")fight.stageType=StageType.FDBBPEGEGMK.STAGE_END_STANCE;
                if(guard=="fight-end")fight._eclipseFightEndDispatched=true;
                Check(!fight.TryEndRound(loaded.Rule.Id,true,out var error),"Guard failed: "+guard);
            }
            var raid=Native(loaded);raid.FightDefinition.Type=BattleType.FightRaid;ModModeRuntime.OfflineRaid=true;
            Check(raid.TryEndRound(loaded.Rule.Id,true,out var failure),failure);raid.Step();
            Check(raid.Winner()==raid.Player,"Offline raid ignored custom winner");
            Check(!Invoke(loaded,Native(loaded),out var setupError,ModEffectEvent.RoundBegin),"Round setup accepted outcome control");
            var paused=Native(loaded);Check(Invoke(loaded,paused,out var pauseError),pauseError);paused.Paused=true;paused.Step();
            Check(paused.Player.RoundsWon==0 && paused._eclipseRoundOutcomes.PendingPlayerWins==true,"Paused boundary consumed objective request");
            paused.Paused=false;paused.Step();Check(paused.Player.RoundsWon==1,"Resume did not consume pending request once");
            var dying=Native(loaded);Check(Invoke(loaded,dying,out var dyingError),dyingError);
            dying.Player.Health=0;dying.Step();Check(dying.Player.RoundsWon==0,"Queued objective overrode zero health before native death was ready");
            dying.Player.PCALDKCJGCK=true;dying.Step();Check(dying.Enemy.RoundsWon==1 && !dying._eclipseRoundOutcomes.ResolvedPlayerWins.HasValue,"Native death completion lost precedence");
            var detached=Native(loaded);Check(Invoke(loaded,detached,out var detachedError),detachedError);Fight.Current=null;detached.Step();
            Check(detached.Player.RoundsWon==0,"Detached fight consumed pending objective");
        }
        using(var loaded=Load("local ok,reason=fighter:end_round('win');assert(not ok and reason)"))
            Check(Invoke(loaded,Native(loaded),out var error,authority:false),error);
        using(var loaded=Load(controls:false,caps:"'content.register','content.patch'"))
        {
            var native=Native(loaded);
            Check(!Invoke(loaded,native,out var error)&&error.Contains("combat.round_outcome"),"Invocation skipped capability guard");
            Check(!native.TryEndRound(loaded.Rule.Id,true,out error),"Ordinary rule acquired host authority");
            native.Player.Health=0;native.Player.PCALDKCJGCK=true;native.Step();
            Check(native.Enemy.RoundsWon==1 && native.Winner()==native.Enemy,"Ordinary native result changed without controller");
        }
        using(var loaded=Load("if saved then saved:end_round('win') else saved=fighter end"))
        {
            var fight=Native(loaded);Check(Invoke(loaded,fight,out var error),error);
            Check(!Invoke(loaded,fight,out error)&&error.Contains("expired"),"Captured fighter retained round authority");
        }
        using(var loaded=Load())
        using(var ordinary=Load(controls:false))
            Check(ModSaveData.ComputeContentSetFingerprint(new[]{loaded.Mod},loaded.Content)!=ModSaveData.ComputeContentSetFingerprint(new[]{ordinary.Mod},ordinary.Content),"Authority declaration absent from fingerprint");
        Conflicts();
        ShippedObjective();
        Console.WriteLine("PASS: "+checks+" round outcome checks (production Lua and extracted native arbitration/score/winner/surrender, controlled presentation and settlement).");
    }
    static void Conflicts()
    {
        foreach(var mode in new[]{"overlap","modes","rounds"})
        {
            var mod=Descriptor("'content.register','content.patch','combat.round_outcome'");var catalog=Catalog();
            using var tx=catalog.BeginRegistration(mod);
            var behavior=tx.RegisterBehavior("goal",new ModParameterSchema(Array.Empty<ModParameterDefinition>()));
            var first=tx.RegisterBehaviorRule("first",behavior.Id,ModRuleTarget.Player,mode=="modes"?ModRuleMode.Normal:ModRuleMode.All,mode=="rounds"?new[]{1}:null,null,true);
            var second=tx.RegisterBehaviorRule("second",behavior.Id,ModRuleTarget.Opponent,mode=="modes"?ModRuleMode.Eclipse:ModRuleMode.All,mode=="rounds"?new[]{2}:null,null,true);
            tx.PatchFightRules(Target,new[]{first.Id,second.Id},true);
            if(mode=="overlap")
            {
                try {tx.Commit();throw new Exception("Overlapping controllers committed");}
                catch(ModContentException error){Check(error.Message.Contains("first")&&error.Message.Contains("second"),"Conflict diagnostic omitted owners");}
                Check(catalog.FightRules.Count==0 && catalog.Behaviors.Count==0,"Outcome conflict partially committed");
            }
            else {tx.Commit();Check(catalog.FightRules.Count==2,"Disjoint controllers rejected");}
            var instances=new ModBattleRuleInstances();
            if(mode=="overlap")
            {
                // A generated rule list is checked again against committed definitions.
                var other=Catalog();using var separate=other.BeginRegistration(mod);
                var b=separate.RegisterBehavior("goal",new ModParameterSchema(Array.Empty<ModParameterDefinition>()));
                var a=separate.RegisterBehaviorRule("a",b.Id,ModRuleTarget.Player,ModRuleMode.All,null,null,true);
                var c=separate.RegisterBehaviorRule("b",b.Id,ModRuleTarget.Player,ModRuleMode.All,null,null,true);
                var zone=separate.RegisterZone("fixture");
                var battle=separate.RegisterBattle("fixture",zone.Id,ModBattleKind.Challenge);
                var warrior=separate.RegisterWarrior("fixture","Fixture","","","",1,"",null,null);
                var blueprint=separate.RegisterFight("fixture",battle.Id,0,0,0,1,99,"","",0,0,"",false,"",new[]{warrior.Id},null,null);
                separate.Commit();
                try {instances.Applicable(other,other.RuntimeFightId(DefinitionId.Parse(Target)),true,1,false,new[]{a.Id,c.Id}).ToArray();throw new Exception("Generated overlap accepted");}
                catch(ModContentException error){Check(error.Message.Contains("conflicting"),error.Message);}
                var type=typeof(MoonSharpScriptRuntime).Assembly.GetType("Projection",true);
                var projection=Activator.CreateInstance(type,true);var method=type.GetMethod("Encounter");
                try{method.Invoke(projection,new object[]{other,blueprint,new ModEncounterPlan(rules:new[]{a.Id,c.Id})});throw new Exception("Conflicting generated encounter projected");}
                catch(System.Reflection.TargetInvocationException error){Check(error.InnerException is ModContentException && error.InnerException.Message.Contains("conflicting"),error.ToString());}
                Check(method.Invoke(projection,new object[]{other,blueprint,new ModEncounterPlan(rules:new[]{a.Id})}) is XmlElement,"Single generated controller rejected");
            }
            else
            {
                var runtime=catalog.RuntimeFightId(DefinitionId.Parse(Target));
                Check(instances.OutcomeAuthority(catalog,runtime,1,false)==first.Id,"First controller not selected");
                Check(instances.OutcomeAuthority(catalog,runtime,mode=="rounds"?2:1,mode=="modes")==second.Id,"Second controller not selected");
            }
        }
    }
    static Loaded LoadExample()
    {
        var directory=Path.Combine(root,"shipped");Directory.CreateDirectory(directory);
        var source=Path.Combine(repo,"Mods/example.hit-objective");
        foreach(var path in Directory.GetFiles(source,"*",SearchOption.AllDirectories))
        {
            var target=Path.Combine(directory,"example.hit-objective",Path.GetRelativePath(source,path));
            Directory.CreateDirectory(Path.GetDirectoryName(target));File.Copy(path,target);
        }
        var mod=ModDiscovery.DiscoverLoose(directory).Mods.Single();
        var loaded=new Loaded{Mod=mod,Content=Catalog()};
        using var tx=loaded.Content.BeginRegistration(mod);
        var api=new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx,new ModStateRuntime(),null);
        loaded.Context=new MoonSharpScriptRuntime(loaded.Surfaces.Add).CreateContext(mod,api);
        loaded.Context.ExecuteEntrypoint();tx.Commit();loaded.Rule=loaded.Content.FightRules.Single();return loaded;
    }
    static void ShippedObjective()
    {
        using var loaded=LoadExample();var fight=Native(loaded);
        var xml=new XmlDocument();xml.LoadXml("<BattleRuleInstance/>");
        var fighter=new ModInstanceFighter(fight,xml.DocumentElement,loaded.Rule);
        void Event(ModEffectEvent kind)
        {
            Check(((IModInteractiveBehaviorScriptContext)loaded.Context).TryInvokeBehavior(loaded.Rule.Behavior,kind,null,
                new Dictionary<string,string>{{"source","rule"},{"round",fight.round.round.ToString()},{"side","player"},{"fight_id",fight.FightDefinition.FightId}},fighter,out var error),error);
        }
        Event(ModEffectEvent.RoundBegin);
        Check(loaded.Surfaces.Count==1 && loaded.Surfaces[0].Read("goal").Text.EndsWith("0/3"),"Shipped objective did not initialize HUD");
        fight.DamageEvent=new ModDamageEvent(1,1,.9,true,false);Event(ModEffectEvent.DamageDealt);
        fight.DamageEvent=new ModDamageEvent(1,1,1,false,false);Event(ModEffectEvent.DamageDealt);
        Check(loaded.Surfaces[0].Read("goal").Text.EndsWith("0/3"),"Blocked/zero damage advanced objective");
        for(int i=1;i<=3;i++)
        {
            fight.DamageEvent=new ModDamageEvent(1,1,.9,false,false);Event(ModEffectEvent.DamageDealt);
            Check(loaded.Surfaces[0].Read("goal").Text.EndsWith(i+"/3"),"Shipped HUD missed positive hit");
        }
        Event(ModEffectEvent.Tick);Check(fight._eclipseRoundOutcomes.PendingPlayerWins==true,"Three-hit Lua objective did not request victory");
        fight.Step();Check(fight.Player.RoundsWon==1 && fight.Player.Health==1 && fight.Enemy.Health==1,"Shipped objective did not score a nonlethal win");
        Event(ModEffectEvent.RoundEnd);Check(loaded.Surfaces[0].IsClosed,"Objective round end left HUD open");
        fight.ResetForNextRound();Event(ModEffectEvent.RoundBegin);
        Check(loaded.Surfaces.Count==2 && loaded.Surfaces[1].Read("goal").Text.EndsWith("0/3"),"Objective state did not reset next round");
        for(int i=0;i<599;i++){fight.Clock++;Event(ModEffectEvent.Tick);}
        Check(!fight._eclipseRoundOutcomes.PendingPlayerWins.HasValue,"Objective failed before ten simulated seconds");
        fight.Clock++;Event(ModEffectEvent.Tick);
        Check(fight._eclipseRoundOutcomes.PendingPlayerWins==false,"Ten-second Lua objective did not request loss");
        fight.Step();Check(fight.Enemy.RoundsWon==1 && fight.Player.Health==1 && fight.Enemy.Health==1,"Objective timeout did not score nonlethal loss");
        Event(ModEffectEvent.FightEnd);Check(loaded.Surfaces[1].IsClosed,"Fight end left objective HUD open");
    }
}
