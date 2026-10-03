using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

static class Program
{
    static int checks; static string fixture,repo;
    static void Check(bool value,string error){checks++;if(!value)throw new Exception(error);}
    sealed class Loaded : IDisposable
    {
        public IModScriptContext Context; public ModContentCatalog Content; public DefinitionId Behavior,Move;
        public void Dispose()=>Context.Dispose();
    }
    static Loaded Load(string body,string caps="content.register,combat.animation,combat.target",ModContentCatalog content=null,string id="fixture.playback")
    {
        var parent=Path.Combine(fixture,Guid.NewGuid().ToString("N"));var folder=Path.Combine(parent,id);
        Directory.CreateDirectory(Path.Combine(folder,"scripts"));Directory.CreateDirectory(Path.Combine(folder,"assets/animations"));
        // Controlled asset metadata only; native acceptance uses a real binary.
        File.WriteAllBytes(Path.Combine(folder,"assets/animations/move.bytes"),new byte[]{1,2,3});
        File.WriteAllText(Path.Combine(folder,"mod.toml"),"schema=1\nid=\""+id+"\"\nname=\"Playback\"\nversion=\"1.0.0\"\nauthors=[\"Fixture\"]\nentrypoint=\"scripts/main.lua\"\ncapabilities=["+string.Join(",",caps.Split(',').Select(c=>"\""+c+"\""))+"]\n");
        File.WriteAllText(Path.Combine(folder,"scripts/main.lua"),"local sf2=require('sf2');local saved,request;local calls=0;local move=sf2.moves.register{id='move',animation='animations/move'};local function callback(_,fighter) calls=calls+1; "+body+" end;sf2.behaviors.register{id='play',on_tick=callback,on_fight_begin=callback,on_round_begin=callback,on_round_end=callback,on_fight_end=callback};");
        var discovery=ModDiscovery.DiscoverLoose(parent);Check(discovery.Diagnostics.Count==0,"Manifest rejected");
        var mod=discovery.Mods.Single();content=content??new ModContentCatalog();
        using var tx=content.BeginRegistration(mod);
        var api=new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx,new ModStateRuntime(),null);
        var context=new MoonSharpScriptRuntime().CreateContext(mod,api);context.ExecuteEntrypoint();tx.Commit();
        return new Loaded{Context=context,Content=content,Behavior=DefinitionId.Parse(id+":behaviors/play"),Move=DefinitionId.Parse(id+":moves/move")};
    }
    static Fight Native(Loaded loaded)
    {
        var fight=new Fight();ModRuntime.Scripts.Content=loaded.Content;
        foreach(var move in loaded.Content.Moves){fight.Player.Animations.Add(new InfoAnimation{Name=move.RuntimeName});fight.Enemy.Animations.Add(new InfoAnimation{Name=move.RuntimeName});}
        return fight;
    }
    static bool Invoke(Loaded loaded,Fight fight,out string error,ModEffectEvent kind=ModEffectEvent.Tick,IModFighterOperations operations=null)
    {
        var doc=new XmlDocument();doc.LoadXml("<BehaviorInstance/>");
        return ((IModInteractiveBehaviorScriptContext)loaded.Context).TryInvokeBehavior(loaded.Behavior,kind,null,new Dictionary<string,string>{{"side","player"}},new ModInstanceFighter(operations??fight.Operations(),doc.DocumentElement),out error);
    }
    static void Event(Loaded loaded,Fight fight){Check(Invoke(loaded,fight,out var error),error);}
    sealed class Unsupported : IModFighterOperations, IModCombatSnapshotSource
    {
        public ModCombatSnapshot CaptureCombatSnapshot()=>new ModCombatSnapshot(new ModFighterSnapshot(1,1,1,0,0,0),null,1,true);
        public bool TryChangeHealth(double amount,out string error){error=null;return true;}
        public bool TryAddMagicCharge(double amount,out string error){error=null;return true;}
    }
    static void Main(string[] args)
    {
        fixture=args[0];repo=args[1];
        using(var loaded=Load("if calls==1 then request=fighter:play_move(move);assert(request.status=='queued' and request.error==nil);assert(fighter:play_move(move).status=='failed');assert(fighter.opponent:play_move(move).status=='queued') else assert(request.status=='applied' and request.error==nil) end"))
        {
            var fight=Native(loaded);Event(loaded,fight);Check(fight.Player.Plays==0,"Playback ran inside Lua");fight.PlaybackStep();
            Check(fight.Player.Plays==1&&fight.Enemy.Plays==1,"Self/opponent did not play");Event(loaded,fight);fight.PlaybackStep();Check(fight.Player.Plays==1,"Playback repeated");
        }
        foreach(var value in new[]{"","nil","'CoreMove'","{}","1","true","move,0","{id=move.id}"})
        {using var loaded=Load("fighter:play_move("+value+")");var fight=Native(loaded);Check(!Invoke(loaded,fight,out var error)&&error.Contains("move handle"),"Invalid argument accepted: "+value);fight.PlaybackStep();Check(fight.Player.Plays==0,"Invalid arguments queued");}
        foreach(var callback in new[]{ModEffectEvent.FightBegin,ModEffectEvent.RoundBegin,ModEffectEvent.RoundEnd,ModEffectEvent.FightEnd})
        {using var loaded=Load("fighter:play_move(move)");var fight=Native(loaded);Check(!Invoke(loaded,fight,out var error,callback)&&error.Contains("simulation"),"Forbidden callback accepted");}
        foreach(var (body,caps) in new[]{("fighter:play_move(move)","content.register"),("fighter.opponent:play_move(move)","content.register,combat.animation")})
        {using var loaded=Load(body,caps);Check(!Invoke(loaded,Native(loaded),out var error)&&error.Contains("capability"),"Missing capability accepted");}
        using(var loaded=Load("if calls==1 then saved=fighter.play_move else saved(move) end"))
        {var fight=Native(loaded);Event(loaded,fight);Check(!Invoke(loaded,fight,out var error)&&error.Contains("expired"),"Retained operation remained live");}
        using(var loaded=Load("request=fighter:play_move(move);assert(request.status=='failed' and request.error)"))
        {var fight=Native(loaded);Check(Invoke(loaded,fight,out var error,operations:new Unsupported()),error);fight.Player.Animations.Clear();Event(loaded,fight);}
        using(var first=Load("assert(fighter:play_move(move).status=='queued')"))
        using(var second=Load("assert(fighter:play_move(move).status=='failed')",content:first.Content,id:"fixture.peer"))
        {var fight=Native(first);Event(first,fight);Event(second,fight);fight.PlaybackStep();Check(fight.Player.Playing==first.Move.ToString(),"Independent mod overwrote earlier request");}
        using(var loaded=Load("fighter:play_move(move);error('after request')"))
        {var fight=Native(loaded);Check(!Invoke(loaded,fight,out var error)&&error.Contains("after request"),"Handler did not fail");fight.PlaybackStep();Check(fight.Player.Plays==1,"Later handler error rolled back accepted playback");}
        foreach(var scenario in new[]{"pause","death","round","session","replacement","ended","cancel","missing","native-reject","native-throw"})
        {
            using var loaded=Load("if calls==1 then request=fighter:play_move(move);assert(request.status=='queued') else assert(request.status=='"+(scenario=="pause"?"applied":"failed")+"');"+(scenario=="pause"?"assert(request.error==nil)":"assert(request.error)")+" end");
            var fight=Native(loaded);Event(loaded,fight);var original=fight.Player;
            switch(scenario){case "pause":fight.Paused=true;fight.PlaybackStep();Check(original.Plays==0,"Pause consumed request");fight.Paused=false;break;case "death":original.Health=0;break;case "round":fight.round.round++;break;case "session":ModRuntime.Scripts=new ModScriptSession{Content=loaded.Content};break;case "replacement":fight.Player=new Model();break;case "ended":fight.isEndRound=true;break;case "cancel":fight.CancelPlayback();break;case "missing":original.Animations.Clear();break;case "native-reject":original.RejectPlayback=true;break;case "native-throw":original.Throw=true;break;}
            fight.PlaybackStep();Event(loaded,fight);fight.PlaybackStep();Check(original.Plays==(scenario=="pause"?1:0),"Bad lifetime application: "+scenario);
        }
        using(var loaded=Load("assert(fighter:play_move(move).status=='queued');assert(fighter.opponent:play_move(move).status=='queued')"))
        {var fight=Native(loaded);Event(loaded,fight);fight.Player.Throw=true;fight.PlaybackStep();Check(fight.Enemy.Plays==1,"Failure suppressed other body");}
        foreach(var guard in new Action<Fight>[]{f=>Fight.Current=null,f=>f.IsLocalVersus=true,f=>f.IsTitleSparring=true,f=>f.FightDefinition=null,f=>f.FightDefinition.Type=BattleType.FightPVP,f=>f.FightDefinition.Type=BattleType.FightRaid,f=>f.isGameOver=true,f=>f.isStopFight=true,f=>f.round.processing=false,f=>f.Paused=true,f=>f._modelTransitionsClosed=true})
        {using var loaded=Load("local r=fighter:play_move(move);assert(r.status=='failed' and r.error)");var fight=Native(loaded);guard(fight);if(fight.round.processing)Event(loaded,fight);else Check(!Invoke(loaded,fight,out var error)&&error.Contains("clock"),"Inactive tick accepted");fight.PlaybackStep();Check(fight.Player.Plays==0,"Forbidden fight played");}
        using(var loaded=Load("request=fighter.play_move(move);assert(request.status=='queued')"))
        {
            var fight=Native(loaded);Event(loaded,fight);
            fight.Player.OnPlay=()=>Check(!fight.QueuePlayback(fight.Player,loaded.Move,null,out var error)&&error.Contains("callback"),"Native playback allowed recursive queue");
            fight.PlaybackStep();Check(fight.Player.Plays==1,"Dot syntax failed");
            Check(fight.QueuePlayback(fight.Player,loaded.Move,(ok,error)=>throw new Exception("Controlled receipt failure"),out _),"Next step stayed locked");fight.PlaybackStep();Check(fight.Player.Plays==2,"Receipt failure repeated or suppressed playback");
        }
        var source=File.ReadAllText(Path.Combine(repo,"Assets/Scripts/Assembly-CSharp/Fight.cs"));
        Check(source.Split("CancelEclipseFighterPlayback();").Length-1==4,"Missing teardown hook");
        Check(source.IndexOf("ApplyEclipseFighterMotion();")<source.IndexOf("ApplyEclipseFighterPlayback();")&&source.IndexOf("ApplyEclipseFighterPlayback();")<source.IndexOf("RenderRound();"),"Wrong simulation boundary");
        Shipped();
        Console.WriteLine("PASS: "+checks+" fighter-playback checks; actual MoonSharp and production motion/playback queues; native model, asset contents, clock and session controlled.");
    }
    static void Shipped()
    {
        var mod=ModDiscovery.DiscoverLoose(Path.Combine(repo,"Mods")).Mods.Single(m=>m.Manifest.Id.Value=="example.active-strike");
        var content=new ModContentCatalog();var stages=new XmlDocument();stages.Load(Path.Combine(repo,"Assets/vanillaXml/stages.xml"));CoreContentImporter.ImportStages(content,stages.SelectSingleNode("Stages/Zones"));
        var surfaces=new List<ModUiSurface>();
        using var tx=content.BeginRegistration(mod);var api=new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx,new ModStateRuntime(),null);
        using var context=new MoonSharpScriptRuntime(surfaces.Add).CreateContext(mod,api);context.ExecuteEntrypoint();tx.Commit();
        var loaded=new Loaded{Content=content};var fight=Native(loaded);var doc=new XmlDocument();doc.LoadXml("<BehaviorInstance/>");
        var rule=content.FightRules.Single();var fighter=new ModInstanceFighter(fight.Operations(),doc.DocumentElement,rule);
        void Event(ModEffectEvent kind){Check(((IModInteractiveBehaviorScriptContext)context).TryInvokeBehavior(rule.Behavior,kind,null,new Dictionary<string,string>{{"source","rule"},{"round",fight.round.round.ToString()},{"fight_id","fixture"}},fighter,out var error),error);}
        Event(ModEffectEvent.RoundBegin);var hud=surfaces.Single();Check(hud.Read("cast").Enabled&&hud.Read("status").Text=="Active Strike ready","Initial HUD wrong");
        Check(hud.TryClick("cast"),"Button rejected intent");Check(fight.Player.Plays==0,"Button played recursively");Event(ModEffectEvent.Tick);Check(fight.Player.Plays==0&&hud.Read("status").Text=="Strike queued"&&!hud.Read("cast").Enabled,"Shipped ability did not defer");
        fight.PlaybackStep();fight.Clock++;Event(ModEffectEvent.Tick);Check(fight.Player.Plays==1&&hud.Read("status").Text.Contains("started"),"Applied receipt was not observed");
        for(int i=0;i<119;i++){fight.Clock++;Event(ModEffectEvent.Tick);}Check(!hud.Read("cast").Enabled,"Cooldown too short");fight.Clock++;Event(ModEffectEvent.Tick);Check(hud.Read("cast").Enabled&&hud.Read("status").Text=="Active Strike ready","Cooldown failed to recharge");
        fight.Player.Animations.Clear();Check(hud.TryClick("cast"),"Ready click rejected");Event(ModEffectEvent.Tick);fight.Clock++;Event(ModEffectEvent.Tick);Check(hud.Read("cast").Enabled&&hud.Read("status").Text=="Strike unavailable","Failed receipt did not reenable button");fight.PlaybackStep();Check(fight.Player.Plays==1,"Rejected cast played");
        Event(ModEffectEvent.RoundEnd);Check(hud.IsClosed,"Round end retained HUD");fight.round.round++;Event(ModEffectEvent.RoundBegin);var next=surfaces.Last();Check(next!=hud&&next.Read("cast").Enabled&&next.Read("status").Text=="Active Strike ready","Next round did not reset");Event(ModEffectEvent.FightEnd);Check(next.IsClosed,"Fight end retained HUD");
    }
}
