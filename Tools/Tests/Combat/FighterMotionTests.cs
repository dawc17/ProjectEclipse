using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

static class Program
{
    static int checks; static string fixture,repo;
    static void Check(bool condition,string message){checks++;if(!condition)throw new Exception(message);}
    sealed class Loaded : IDisposable
    {
        public IModScriptContext Context; public DefinitionId Behavior;
        public void Dispose()=>Context.Dispose();
    }
    static Loaded Load(string body,string caps="content.register,combat.motion,combat.target",string suffix="")
    {
        var folder=Path.Combine(fixture,Guid.NewGuid().ToString("N"),"example.motion");Directory.CreateDirectory(Path.Combine(folder,"scripts"));
        File.WriteAllText(Path.Combine(folder,"mod.toml"),"schema=1\nid=\"example.motion\"\nname=\"Motion\"\nversion=\"1.0.0\"\nauthors=[\"Fixture\"]\nentrypoint=\"scripts/main.lua\"\ncapabilities=["+string.Join(",",caps.Split(',').Select(c=>"\""+c+"\""))+ "]\n");
        var source="local sf2=require('sf2');local saved;local function callback(_,fighter) "+body+" end;sf2.behaviors.register{id='move',on_tick=callback,on_fight_begin=callback,on_round_begin=callback,on_round_end=callback,on_fight_end=callback};"+suffix;
        File.WriteAllText(Path.Combine(folder,"scripts/main.lua"),source);
        var discovery=ModDiscovery.DiscoverLoose(Path.GetDirectoryName(folder));Check(discovery.Diagnostics.Count==0,"Fixture manifest rejected");
        var mod=discovery.Mods.Single();var content=new ModContentCatalog();
        using(var tx=content.BeginRegistration(mod))
        {
            var api=new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx,new ModStateRuntime(),null);
            var context=new MoonSharpScriptRuntime().CreateContext(mod,api);
            context.ExecuteEntrypoint();tx.Commit();return new Loaded{Context=context,Behavior=content.Behaviors.Single().Id};
        }
    }
    static bool Invoke(Loaded loaded,Fight fight,out string error,ModEffectEvent kind=ModEffectEvent.Tick,IModFighterOperations operations=null)
    {
        var document=new XmlDocument();document.LoadXml("<BehaviorInstance/>");
        var wrapped=new ModInstanceFighter(operations??fight.Operations(),document.DocumentElement);
        return ((IModInteractiveBehaviorScriptContext)loaded.Context).TryInvokeBehavior(loaded.Behavior,kind,null,new Dictionary<string,string>{{"side","player"}},wrapped,out error);
    }
    sealed class Unsupported : IModFighterOperations, IModCombatSnapshotSource
    {
        public double Health=>1;
        public ModCombatSnapshot CaptureCombatSnapshot()=>new ModCombatSnapshot(new ModFighterSnapshot(1,1,1,0,0,0),null,1,true);
        public bool TryChangeHealth(double amount,out string error){error=null;return true;}
        public bool TryAddMagicCharge(double amount,out string error){error=null;return true;}
    }
    static void Main(string[] args)
    {
        fixture=args[0];repo=args[1];
        using(var loaded=Load("local ok,reason=fighter:move_by(20,3,4);assert(ok and reason==nil);assert(fighter.opponent:move_by(-10,0))"))
        {
            var fight=new Fight();Check(Invoke(loaded,fight,out var error),error);
            Check(fight.Player.X==0&&fight.Enemy.X==0,"Motion ran recursively inside Lua");
            fight.Step();Check(fight.Player.X==20&&fight.Player.Y==3&&fight.Player.Z==4&&fight.Enemy.X==-10,"Self/opponent motion did not reach production queue");
            Check(fight.Player.Translations==1&&fight.Enemy.Translations==1&&fight.Player.AnimationShifted,"Motion did not coalesce with animation translation");
            fight.Step();Check(fight.Player.Translations==1,"Motion replayed next step");
        }
        using(var loaded=Load("assert(fighter.move_by(2,0,nil));assert(fighter:move_by(3,0))"))
        {var fight=new Fight();Check(Invoke(loaded,fight,out var error),error);fight.Step();Check(fight.Player.X==5&&fight.Player.Translations==1,"Dot/colon optional-z motion did not add");}
        foreach(var value in new[]{"", "1", "'1',0", "true,0", "{},0", "nil,0", "0,0,'bad'", "0,0,0,1", "1001,0", "0,-1001", "0,0,1001", "0/0,0", "math.huge,0", "0,-math.huge"})
        {
            using var loaded=Load("fighter:move_by("+value+")");var fight=new Fight();
            Check(!Invoke(loaded,fight,out var error),"Invalid displacement accepted: "+value);fight.Step();Check(fight.Player.Translations==0,"Invalid motion queued");
        }
        foreach(var item in new[]{("fighter:move_by(1,0)","content.register","combat.motion"),("fighter.opponent:move_by(1,0)","content.register,combat.motion","combat.target")})
        {using var loaded=Load(item.Item1,item.Item2);var fight=new Fight();Check(!Invoke(loaded,fight,out var error)&&error.Contains(item.Item3),"Capability not enforced: "+item.Item3);fight.Step();Check(fight.Player.Translations+fight.Enemy.Translations==0,"Denied capability queued motion");}
        using(var loaded=Load("assert(fighter:move_by(1,0))"))
            foreach(var kind in new[]{ModEffectEvent.FightBegin,ModEffectEvent.RoundBegin,ModEffectEvent.RoundEnd,ModEffectEvent.FightEnd})
            {var fight=new Fight();Check(!Invoke(loaded,fight,out var error,kind)&&error.Contains("active simulation"),"Boundary callback accepted movement: "+kind+" / "+error);fight.Step();Check(fight.Player.Translations==0,"Boundary queued movement");}
        using(var loaded=Load("if saved then saved:move_by(1,0) else saved=fighter end"))
        {var fight=new Fight();Check(Invoke(loaded,fight,out var error),error);Check(!Invoke(loaded,fight,out error)&&error.Contains("expired"),"Captured fighter could move later");}
        using(var loaded=Load("local ok,reason=fighter:move_by(1,0);assert(not ok and reason)"))
        {var fight=new Fight();Check(Invoke(loaded,fight,out var error,operations:new Unsupported()),error);}
        using(var a=Load("assert(fighter:move_by(20,0))"))
        using(var b=Load("assert(fighter:move_by(-5,0));assert(fighter.opponent:move_by(8,0))"))
        {var fight=new Fight();Check(Invoke(a,fight,out var error),error);Check(Invoke(b,fight,out error),error);fight.Step();Check(fight.Player.X==15&&fight.Enemy.X==8&&fight.Player.Translations==1,"Independent Lua contexts did not compose");}
        var capped=new Fight();Check(capped.Queue(capped.Player,900,0,0,out var reason),reason);
        Check(!capped.Queue(capped.Player,101,0,0,out reason),"Combined limit accepted");
        Check(capped.Queue(capped.Player,-200,0,0,out reason),reason);capped.Step();Check(capped.Player.X==700,"Rejected request corrupted earlier sum");
        var counted=new Fight();for(int i=0;i<32;i++)Check(counted.Queue(counted.Player,1,0,0,out reason),reason);
        Check(!counted.Queue(counted.Player,1,0,0,out reason),"Request limit accepted");
        for(int i=0;i<100;i++)Check(counted.Queue(counted.Player,0,0,0,out reason),"Zero displacement consumed a slot");
        Check(counted.Queue(counted.Enemy,-1000,1000,1000,out reason),reason);counted.Step();
        Check(counted.Player.X==32&&counted.Enemy.X==-1000&&counted.Enemy.Y==1000&&counted.Enemy.Z==1000,"Axis boundary or shared request count wrong");
        Check(counted.Queue(counted.Player,1,0,0,out reason),"New step did not reset limit");
        var neutral=new Fight();neutral.Queue(neutral.Player,10,2,3,out reason);neutral.Queue(neutral.Player,-10,-2,-3,out reason);
        neutral.Step();Check(neutral.Player.Translations==0,"Canceled offsets still ran native physics");
        foreach(var guard in new[]{"pause","title","versus","training","pvp","online-raid","stale","closed","end-stage","fight-end","ended-round","stopped","end-round","game-over","not-processing","dead","child","no-session"})
        {
            var fight=new Fight();Model body=fight.Player;
            if(guard=="pause")fight.Paused=true;if(guard=="title")fight.IsTitleSparring=true;if(guard=="versus")fight.IsLocalVersus=true;
            if(guard=="training")fight.FightDefinition.Type=BattleType.FightNone;if(guard=="pvp")fight.FightDefinition.Type=BattleType.FightPVP;if(guard=="online-raid")fight.FightDefinition.Type=BattleType.FightRaid;
            if(guard=="stale")Fight.Current=null;if(guard=="closed")fight._modelTransitionsClosed=true;if(guard=="end-stage")fight.stageType=StageType.FDBBPEGEGMK.STAGE_END_STANCE;
            if(guard=="fight-end")fight._eclipseFightEndDispatched=true;if(guard=="ended-round")fight._eclipseEndedRound=1;
            if(guard=="stopped")fight.isStopFight=true;if(guard=="end-round")fight.isEndRound=true;if(guard=="game-over")fight.isGameOver=true;if(guard=="not-processing")fight.round.processing=false;
            if(guard=="dead")fight.Player.Health=0;if(guard=="child")body=new Model();if(guard=="no-session")ModRuntime.Scripts=null;
            Check(!fight.Queue(body,1,0,0,out reason)&&reason!=null,"Guard failed: "+guard);Check(!fight.Queue(body,0,0,0,out reason),"Zero request bypassed guard: "+guard);
        }
        var raid=new Fight();raid.FightDefinition.Type=BattleType.FightRaid;ModModeRuntime.OfflineRaid=true;
        Check(raid.Queue(raid.Player,1,0,0,out reason),"Offline mod raid rejected");raid.Step();Check(raid.Player.X==1,"Offline mod raid did not move");
        foreach(var stale in new[]{"round","body","session","death","fight","cancel","ended"})
        {
            var fight=new Fight();var original=fight.Player;Check(fight.Queue(original,10,0,0,out reason),reason);
            if(stale=="round")fight.round.round++;if(stale=="body")fight.Player=new Model();if(stale=="session")ModRuntime.Scripts=new ModScriptSession();if(stale=="death")original.Health=0;
            if(stale=="fight")Fight.Current=null;if(stale=="cancel")fight.Cancel();if(stale=="ended")fight._eclipseEndedRound=1;
            fight.Step();Check(original.Translations==0,"Stale motion applied: "+stale);
        }
        var pause=new Fight();Check(pause.Queue(pause.Player,10,0,0,out reason),reason);pause.Paused=true;pause.Step();Check(pause.Player.Translations==0,"Paused motion applied");pause.Paused=false;pause.Step();pause.Step();Check(pause.Player.X==10&&pause.Player.Translations==1,"Resume lost or repeated motion");
        var failed=new Fight();failed.Player.Throw=true;failed.Queue(failed.Player,10,0,0,out reason);failed.Queue(failed.Enemy,20,0,0,out reason);failed.Step();failed.Player.Throw=false;failed.Step();Check(failed.Player.X==0&&failed.Enemy.X==20,"Native failure replayed or suppressed other participant");
        ShippedRepulse();
        var source=File.ReadAllText(Path.Combine(repo,"Assets/Scripts/Assembly-CSharp/Fight.cs")).Replace("\r\n","\n");
        Check(source.Contains("ApplyEclipseFighterMotion();\n\t\tRenderRound();"),"Native application boundary moved");
        Check(source.Split("CancelEclipseFighterMotion();").Length-1==4,"Native lifetime cancellation hook missing");
        Console.WriteLine("PASS: "+checks+" fighter-motion checks; real MoonSharp binding, independent Lua contexts and full production queue; model, clock/session and translation services controlled.");
    }
    static void ShippedRepulse()
    {
        var mod=ModDiscovery.DiscoverLoose(Path.Combine(repo,"Mods")).Mods.Single(m=>m.Manifest.Id.Value=="example.repulse");
        var content=new ModContentCatalog();var stages=new XmlDocument();stages.Load(Path.Combine(repo,"Assets/vanillaXml/stages.xml"));
        CoreContentImporter.ImportStages(content,stages.SelectSingleNode("Stages/Zones"));
        var surfaces=new List<ModUiSurface>();
        using var tx=content.BeginRegistration(mod);
        var api=new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx,new ModStateRuntime(),null);
        using var context=new MoonSharpScriptRuntime(surfaces.Add).CreateContext(mod,api);
        context.ExecuteEntrypoint();tx.Commit();
        var rule=content.FightRules.Single();var fight=new Fight();fight.Enemy.X=200;
        var document=new XmlDocument();document.LoadXml("<BattleRuleInstance/>");
        var fighter=new ModInstanceFighter(fight.Operations(),document.DocumentElement,rule);
        void Event(ModEffectEvent kind)
        {
            Check(((IModInteractiveBehaviorScriptContext)context).TryInvokeBehavior(rule.Behavior,kind,null,
                new Dictionary<string,string>{{"source","rule"},{"round",fight.round.round.ToString()},{"fight_id","fixture"}},fighter,out var error),error);
        }
        Event(ModEffectEvent.RoundBegin);var hud=surfaces.Single();
        Check(hud.Read("status").Text=="Repulse ready"&&hud.Read("cast").Enabled,"Shipped HUD did not initialize ready");
        Check(hud.TryClick("cast"),"Shipped button rejected click");Check(fight.Player.Translations+fight.Enemy.Translations==0,"Shipped UI moved recursively");
        Event(ModEffectEvent.Tick);Check(fight.Player.X==0&&fight.Enemy.X==200,"Shipped callback moved recursively");
        Check(!hud.Read("cast").Enabled&&hud.Read("status").Text.Contains("queued"),"Shipped cooldown/HUD not queued");
        fight.Step();Check(fight.Player.X==-40&&fight.Enemy.X==300,"Shipped ability offsets wrong");
        Check(!hud.TryClick("cast"),"Cooldown button accepted second click");
        for(int i=0;i<179;i++){fight.Clock++;Event(ModEffectEvent.Tick);}
        Check(!hud.Read("cast").Enabled,"Shipped cooldown ended early");
        fight.Clock++;Event(ModEffectEvent.Tick);Check(hud.Read("cast").Enabled&&hud.Read("status").Text=="Repulse ready","180-frame cooldown did not recharge");
        // A rejected pair leaves the ability ready, and consumes its UI intent once.
        for(int i=0;i<32;i++){fight.Queue(fight.Player,1,0,0,out _);fight.Queue(fight.Enemy,1,0,0,out _);}
        Check(hud.TryClick("cast"),"Ready click rejected");Event(ModEffectEvent.Tick);
        Check(hud.Read("cast").Enabled&&hud.Read("status").Text=="Repulse ready","Rejected movement started a cooldown");
        fight.Step();fight.Clock++;Event(ModEffectEvent.Tick);Check(fight.Player.X==-8&&fight.Enemy.X==332,"Rejected ability overwrote previous queue or retained intent");
        Event(ModEffectEvent.RoundEnd);Check(hud.IsClosed,"Shipped round end retained HUD");
        fight.round.round++;Event(ModEffectEvent.RoundBegin);var next=surfaces.Last();
        Check(next!=hud&&next.Read("status").Text=="Repulse ready"&&next.Read("cast").Enabled,"Next round did not reset cooldown and HUD");
        Event(ModEffectEvent.FightEnd);Check(next.IsClosed,"Shipped fight end retained HUD");
    }

}
