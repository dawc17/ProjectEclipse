using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

static class Program
{
    static int checks; static string fixture,repo;
    static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    sealed class Marker : IModArenaMarker
    {
        public bool Active=true;public int Removed; public ModUiColor Color;
        public bool IsActive=>Active;
        public void SetColor(ModUiColor color){Color=color;}
        public void Dispose(){Active=false;Removed++;}
    }
    sealed class Fighter : IModFighterOperations, IModFighterRegions, IModCombatSnapshotSource, IModFighterTargets
    {
        public readonly List<Marker> Markers=new List<Marker>();public bool Inside=true,Reject,Throws;public int Queries,Changes;
        public double Health{get;private set;}=1;public Fighter Peer;public IModFighterOperations Opponent=>Peer;
        public ModCombatSnapshot CaptureCombatSnapshot()=>new ModCombatSnapshot(new ModFighterSnapshot(Health,1,1,0,0,0),null,1,true);
        public bool TryChangeHealth(double amount,out string error){Changes++;Health+=amount;error=null;return true;}
        public bool TryAddMagicCharge(double amount,out string error){error=null;return true;}
        public bool TryOverlapRect(ModArenaRect rect,out bool hit,out string error){Queries++;hit=Inside;error=Reject?"unavailable":null;return !Reject;}
        public bool TryMarkRect(ModArenaRect rect,ModUiColor color,out IModArenaMarker result,out string error)
        {
            result=null;error=null;if(Throws)throw new Exception("render failed");if(Reject){error="inactive round";return false;}
            var marker=new Marker{Color=color};Markers.Add(marker);result=marker;return true;
        }
    }
    sealed class Loaded : IDisposable
    {
        public IModScriptContext Context;public ModContentCatalog Content;public DefinitionId Behavior;public Fighter Fighter=new Fighter{Peer=new Fighter()};
        public void Dispose()=>Context.Dispose();
    }
    static Loaded Load(string body,string caps="content.register,presentation.visuals,combat.target",bool freeze=true)
    {
        var parent=Path.Combine(fixture,Guid.NewGuid().ToString("N"));var folder=Path.Combine(parent,"fixture.arena");Directory.CreateDirectory(Path.Combine(folder,"scripts"));
        File.WriteAllText(Path.Combine(folder,"mod.toml"),"schema=1\nid=\"fixture.arena\"\nname=\"Arena\"\nversion=\"1.0.0\"\nauthors=[\"Fixture\"]\nentrypoint=\"scripts/main.lua\"\ncapabilities=["+string.Join(",",caps.Split(',').Select(c=>"\""+c+"\""))+"]\n");
        File.WriteAllText(Path.Combine(folder,"scripts/main.lua"),"local sf2=require('sf2');local rect={x=0,y=0,width=20,height=40};local saved,marker;local calls=0;local function run(_,fighter) calls=calls+1;"+body+" end;sf2.behaviors.register{id='test',on_tick=run,on_round_begin=run,on_round_end=run,on_fight_begin=run,on_fight_end=run};");
        var discovery=ModDiscovery.DiscoverLoose(parent);Check(discovery.Diagnostics.Count==0,"Bad manifest");var mod=discovery.Mods.Single();var content=new ModContentCatalog();using var tx=content.BeginRegistration(mod);
        var api=new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx,new ModStateRuntime(),null);
        var context=new MoonSharpScriptRuntime(surface=>{}).CreateContext(mod,api);context.ExecuteEntrypoint();tx.Commit();if(freeze)content.Freeze();
        return new Loaded{Context=context,Content=content,Behavior=DefinitionId.Parse("fixture.arena:behaviors/test")};
    }
    static bool Invoke(Loaded loaded,out string error,ModEffectEvent kind=ModEffectEvent.Tick)=>
        ((IModInteractiveBehaviorScriptContext)loaded.Context).TryInvokeBehavior(loaded.Behavior,kind,null,null,loaded.Fighter,out error);
    static void Event(Loaded loaded){Check(Invoke(loaded,out var error),error);}
    static void Main(string[] args)
    {
        fixture=args[0];repo=args[1];Geometry();
        using(var l=Load("local yes,e=fighter:overlaps_rect(rect);assert(yes and e==nil);local no,f=fighter.opponent:overlaps_rect(rect);assert(no==false and f==nil);marker=assert(fighter:mark_rect(rect));assert(sf2.world.is_marker_active(marker));assert(sf2.world.set_marker_color(marker,'#12345678'));assert(sf2.world.remove_marker(marker));assert(not sf2.world.is_marker_active(marker));assert(not sf2.world.remove_marker(marker));assert(not sf2.world.set_marker_color(marker,'#ffffff'))"))
        {l.Fighter.Peer.Inside=false;Event(l);Check(l.Fighter.Markers[0].Color.A==0x78&&l.Fighter.Markers[0].Removed==1,"Color/remove idempotency");}
        using(var l=Load("marker=assert(fighter:mark_rect(rect))")){Event(l);l.Dispose();Check(!l.Fighter.Markers[0].Active,"Script teardown");}
        using(var l=Load("for i=1,16 do assert(fighter:mark_rect(rect)) end;local m,e=fighter:mark_rect(rect);assert(m==nil and e:find('16'))")){Event(l);Check(l.Fighter.Markers.Count==16,"Per-context marker budget");}
        using(var l=Load("if calls==1 then marker=assert(fighter:mark_rect(rect)) else assert(not sf2.world.is_marker_active(marker));assert(fighter:mark_rect(rect)) end")){Event(l);l.Fighter.Markers[0].Active=false;Event(l);Check(l.Fighter.Markers[0].Removed==1&&l.Fighter.Markers.Count==2,"Completed slot release");}
        using(var l=Load("local value,e=fighter:overlaps_rect(rect);assert(value==nil and e=='unavailable');local m,f=fighter:mark_rect(rect);assert(m==nil and f=='inactive round')")){l.Fighter.Reject=true;Event(l);}
        using(var l=Load("local m,e=fighter:mark_rect(rect);assert(m==nil and e:find('render failed'))")){l.Fighter.Throws=true;Event(l);}
        foreach(var rect in new[]{"{}","{x=0,y=0,width=0,height=1}","{x=0,y=0,width=1,height=-1}","{x=math.huge,y=0,width=1,height=1}","{x=0/0,y=0,width=1,height=1}","{x=10001,y=0,width=1,height=1}","{x=0,y=0,width=4001,height=1}","{x='0',y=0,width=1,height=1}","{x=0,y=0,width=1,height=1,angle=0}","nil","1"})
        foreach(var method in new[]{"overlaps_rect","mark_rect"})
        using(var l=Load("fighter:"+method+"("+rect+")")){Check(!Invoke(l,out var error)&&error.Length>0,"Invalid rectangle: "+method+rect);Check(l.Fighter.Queries==0&&l.Fighter.Markers.Count==0,"Invalid input reached backend");}
        foreach(var bad in new[]{"fighter:overlaps_rect()","fighter:overlaps_rect(rect,2)","fighter:mark_rect(rect,1)","fighter:mark_rect(rect,'bad')","fighter:mark_rect(rect,nil,2)","sf2.world.is_marker_active({})","sf2.world.remove_marker({})","sf2.world.set_marker_color({},'#ffffff')"})
        using(var l=Load(bad)){Check(!Invoke(l,out var error)&&error.Length>0,"Invalid arena call: "+bad);Check(l.Fighter.Markers.Count==0,"Invalid call allocated");}
        foreach(var bad in new[]{"sf2.world.remove_marker(marker,1)","sf2.world.is_marker_active(marker,1)","sf2.world.set_marker_color(marker)","sf2.world.set_marker_color(marker,1)","sf2.world.set_marker_color(marker,'bad')"})
        using(var l=Load("marker=assert(fighter:mark_rect(rect));"+bad)){Check(!Invoke(l,out var error),"Bad retained handle call accepted");Check(l.Fighter.Markers[0].Active,"Bad call removed marker");}
        foreach(var body in new[]{"fighter:mark_rect(rect)","fighter.opponent:overlaps_rect(rect)"})
        using(var l=Load(body,"content.register")){Check(!Invoke(l,out var error)&&error.Contains("capability"),"Capability missing");}
        using(var l=Load("assert(fighter:overlaps_rect(rect))","content.register"))Event(l);
        foreach(var kind in new[]{ModEffectEvent.FightBegin,ModEffectEvent.RoundBegin,ModEffectEvent.RoundEnd,ModEffectEvent.FightEnd})
        using(var l=Load("fighter:mark_rect(rect)")){Check(!Invoke(l,out var error,kind)&&error.Contains("simulation"),"Marker forbidden lifecycle");}
        foreach(var body in new[]{"if calls==1 then saved=fighter.overlaps_rect else saved(rect) end","if calls==1 then saved=fighter.mark_rect else saved(rect) end","if calls==1 then saved=fighter.opponent.overlaps_rect else saved(rect) end"})
        using(var l=Load(body)){Event(l);Check(!Invoke(l,out var error)&&error.Contains("expired"),"Escaped fighter remained active");}
        using(var l=Load("for i=1,16 do assert(fighter:overlaps_rect(rect));assert(fighter.opponent:overlaps_rect(rect)) end;fighter:overlaps_rect(rect)"))
        {Check(!Invoke(l,out var error)&&error.Contains("32"),"Shared query budget");Check(l.Fighter.Queries+l.Fighter.Peer.Queries==32,"Budget queried native source");}
        // Foreign handle identity: copying an actual table from another context
        // cannot make the recipient its owner, even when the shape is unchanged.
        using(var a=Load("foreign=assert(fighter:mark_rect(rect))"))using(var b=Load("sf2.world.remove_marker(foreign)"))
        {Event(a);var flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;var sa=(MoonSharp.Interpreter.Script)a.Context.GetType().GetField("_script",flags).GetValue(a.Context);var sb=(MoonSharp.Interpreter.Script)b.Context.GetType().GetField("_script",flags).GetValue(b.Context);bool blocked=false;try{sb.Globals.Set("foreign",sa.Globals.Get("foreign"));}catch(MoonSharp.Interpreter.ScriptRuntimeException e){blocked=e.Message.Contains("different scripts");}Check(blocked&&a.Fighter.Markers[0].Active,"MoonSharp foreign marker ownership");}
        using(var l=Load("marker=assert(fighter:mark_rect(rect))",freeze:false)){Check(!Invoke(l,out var error)&&error.Contains("registration")&&l.Fighter.Markers.Count==0,"Marker created while dependent registration open");l.Content.Freeze();Event(l);}
        using(var l=Load("local hud=sf2.ui.open{id='owner',mount='hud',root={id='text',kind='text',text='Owner',width=100,height=40},on_close=function() fighter:mark_rect(rect) end};sf2.ui.close(hud)","content.register,presentation.visuals,ui.create")){Event(l);Check(l.Fighter.Markers.Count==0,"UI cleanup created a marker");}
        Shipped();Console.WriteLine("Arena runtime PASS: "+checks+" checks; production geometry/Lua/scopes and shipped hazard with controlled native rig/markers/clock.");
    }
    static void Geometry()
    {
        var r=new ModArenaRect(0,0,10,10);
        foreach(var c in new[]{(-5d,5d,15d,5d,0d),(5d,-5d,5d,15d,0d),(-1d,-1d,11d,11d,0d),(0d,0d,0d,0d,0d),(5d,5d,5d,5d,0d),(-2d,5d,-2d,9d,2d),(-3d,-4d,-3d,-4d,5d)})
            Check(r.OverlapsCapsule(c.Item1,c.Item2,c.Item3,c.Item4,c.Item5),"Capsule contact/tangent");
        foreach(var c in new[]{(-2d,-2d,-2d,-2d,2d),(-2d,5d,-2d,9d,1.99d),(-3d,-4d,-3d,-4d,4.99d),(11d,-4d,11d,-2d,1d),(-5d,-1d,-1d,-5d,1d)})
            Check(!r.OverlapsCapsule(c.Item1,c.Item2,c.Item3,c.Item4,c.Item5),"Rounded corner/distant capsule false positive");
        Check(!r.OverlapsCapsule(double.NaN,0,0,0,1)&&!r.OverlapsCapsule(0,0,0,0,-1),"Invalid native capsule");
    }
    static void Shipped()
    {
        var mod=ModDiscovery.DiscoverLoose(Path.Combine(repo,"Mods")).Mods.Single(m=>m.Id.Value=="example.pulse-arena");var content=new ModContentCatalog();var stages=new XmlDocument();stages.Load(Path.Combine(repo,"Assets/vanillaXml/stages.xml"));CoreContentImporter.ImportStages(content,stages.SelectSingleNode("Stages/Zones"));
        var surfaces=new List<ModUiSurface>();using var tx=content.BeginRegistration(mod);var api=new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx,new ModStateRuntime(),null);using var context=new MoonSharpScriptRuntime(surfaces.Add).CreateContext(mod,api);context.ExecuteEntrypoint();tx.Commit();content.Freeze();
        var native=new Fighter{Peer=new Fighter{Inside=false}};var doc=new XmlDocument();doc.LoadXml("<Instance/>");var rule=content.FightRules.Single();var fighter=new ModInstanceFighter(native,doc.DocumentElement,rule);int round=1;
        void Event(ModEffectEvent kind){Check(((IModInteractiveBehaviorScriptContext)context).TryInvokeBehavior(rule.Behavior,kind,null,new Dictionary<string,string>{{"source","rule"},{"round",round.ToString()},{"fight_id","fixture"}},fighter,out var error),error);}
        Event(ModEffectEvent.RoundBegin);var hud=surfaces.Single();
        for(int i=0;i<120;i++)Event(ModEffectEvent.Tick);Check(native.Changes==0&&hud.Read("status").Text.Contains("Warning")&&native.Markers.Single().Active,"Warning phase");
        for(int i=0;i<120;i++)Event(ModEffectEvent.Tick);Check(native.Changes==4&&native.Peer.Changes==0&&Math.Abs(native.Health-.9)<.0001,"Active contact schedule");Check(native.Markers[0].Color.R==255&&native.Markers[0].Color.G==0x33,"Recolor");
        for(int i=0;i<120;i++)Event(ModEffectEvent.Tick);Check(!native.Markers[0].Active&&native.Changes==4&&hud.Read("status").Text.Contains("Safe"),"Safe phase");
        Event(ModEffectEvent.Tick);Check(native.Markers.Count==2,"Recurring phase");Event(ModEffectEvent.RoundEnd);Check(hud.IsClosed&&!native.Markers[1].Active,"Round cleanup");
        round++;Event(ModEffectEvent.RoundBegin);Event(ModEffectEvent.Tick);Check(native.Markers.Count==3&&surfaces.Last().Read("contacts").Text=="Contacts: 0","New round state");Event(ModEffectEvent.FightEnd);Check(surfaces.Last().IsClosed&&!native.Markers.Last().Active,"Fight cleanup");
    }
}
