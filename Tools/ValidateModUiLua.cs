using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

sealed class UiFighter : IModFighterOperations, IModCombatSnapshotSource, IModIncomingHitSource
{
    public int Frame;
    public ModIncomingHit IncomingHit { get; set; }
    public ModCombatSnapshot CaptureCombatSnapshot() => new ModCombatSnapshot(new ModFighterSnapshot(1,1,1,0,0,0),null,Frame,true);
    public bool TryChangeHealth(double value,out string error) { error=""; return true; }
    public bool TryAddMagicCharge(double value,out string error) { error=""; return true; }
}
static class Program
{
    static int checks;
    static string mods, repo, manifest, entry, originalManifest;
    static void Check(bool value,string message) { checks++; if(!value)throw new Exception(message); }
    static void Main(string[] args)
    {
        mods=args[0];repo=args[1];
        manifest=Path.Combine(mods,"example.charge-ui/mod.toml");entry=Path.Combine(mods,"example.charge-ui/scripts/main.lua");
        originalManifest=File.ReadAllText(manifest);
        string example=File.ReadAllText(entry);
        Run(example,false,(context,catalog,views)=>{
            var behavior=catalog.FightRules.Single().Behavior;
            foreach(var battle in new[]{"tournament","tournament_eclipsemode"}) {
                var fightId=DefinitionId.Parse("core:fights/zone_1/"+battle+"/3");
                var rules=new ModBattleRuleInstances().Applicable(catalog,catalog.RuntimeFightId(fightId),true,1,battle.EndsWith("eclipsemode")).ToArray();
                Check(rules.Length==1 && rules[0].Behavior==behavior,"Charged Strike is not attached to the native "+battle+" fight");
                Check(!new ModBattleRuleInstances().Applicable(catalog,catalog.RuntimeFightId(fightId),false,1,true).Any(),"Player HUD rule reached opponent");
                var adjacent=DefinitionId.Parse("core:fights/zone_1/"+battle+"/2");
                Check(!new ModBattleRuleInstances().Applicable(catalog,catalog.RuntimeFightId(adjacent),true,1,true).Any(),"HUD leaked onto another fight");
            }
            var fighter=new UiFighter();
            var fields=new Dictionary<string,string>{{"round","1"},{"source","rule"},{"fight_id","fixture"}};
            var script=(IModInteractiveBehaviorScriptContext)context;
            Action<ModEffectEvent> invoke=kind=>Check(script.TryInvokeBehavior(behavior,kind,null,fields,fighter,out var error),error);
            invoke(ModEffectEvent.RoundBegin);
            var view=views.Single();
            Check(view.Placement.Anchor=="top_right" && view.Placement.X==-24 && view.Placement.Y==104,"Lua placement did not reach model");
            Check(!view.TryClick("arm"),"Uncharged button activated");
            for(int frame=1;frame<=300;frame++){fighter.Frame=frame;invoke(ModEffectEvent.Tick);}
            Check(view.Read("meter").Value==1 && view.Read("arm").Enabled,"Charge did not fill at exactly 300 active frames");
            Check(view.TryClick("arm") && !view.Read("arm").Enabled,"Arm click did not update UI");
            double damage=10;
            fighter.IncomingHit=new ModIncomingHit(()=>damage,n=>damage=n,true,false);invoke(ModEffectEvent.DamageDealing);
            Check(damage==10,"Blocked hit consumed bonus");
            fighter.IncomingHit=new ModIncomingHit(()=>damage,n=>damage=n);invoke(ModEffectEvent.DamageDealing);
            Check(damage==20 && view.Read("meter").Value==0,"Fresh combat callback did not consume armed bonus");
            damage=10;invoke(ModEffectEvent.DamageDealing);Check(damage==10,"Bonus applied twice");
            invoke(ModEffectEvent.RoundEnd);Check(view.IsClosed,"Round end retained HUD");
            fields["round"]="2";invoke(ModEffectEvent.RoundBegin);
            Check(views.Count==2 && views[1].Read("meter").Value==0,"Round restart reused stale UI/state");
            for(int frame=301;frame<=600;frame++){fighter.Frame=frame;invoke(ModEffectEvent.Tick);}
            Check(views[1].TryClick("arm"),"Second-round ability could not arm");
            views[1].Close(ModUiCloseReason.Scene);
            damage=10;invoke(ModEffectEvent.DamageDealing);
            Check(damage==10,"Closed HUD retained its armed gameplay bonus");
            context.Dispose();Check(views[1].IsClosed,"Script shutdown retained example HUD");
        });
        const string root="{id='root',kind='column',width=200,height=100,children={{id='label',kind='text',width=200,height=40,text='old'},{id='go',kind='button',width=200,height=40,text='Go'}}}";
        const string prefix="local sf2=require('sf2')\n";
        Run(prefix+@"local key=sf2.localization.key('charge.arm')
assert(sf2.localization.text(key)=='ARM NEXT STRIKE')
assert(sf2.localization.text(key,'eng')=='ARM NEXT STRIKE')
assert(sf2.localization.text(key,'missing')=='ARM NEXT STRIKE')
assert(sf2.localization.text(key,'POL')~=sf2.localization.text(key,'eng'))",false);
        foreach(string invalid in new[]{"{}","'charge.arm'","sf2.localization.key('charge.arm'),3","sf2.localization.key('charge.arm'),'../eng'"})
            Run(prefix+"sf2.localization.text("+invalid+")",true);
        string open="sf2.ui.open{id='test',mount='menu',root="+root+"}";
        foreach(var reason in new[]{ModUiCloseReason.Script,ModUiCloseReason.Back,ModUiCloseReason.Scene,ModUiCloseReason.Error,ModUiCloseReason.Destroyed})
        {
            var closeLogs=new List<ModLogEntry>();
            Run(prefix+"local marker="+open+@"
local calls=0
sf2.ui.open{id='closer',mount='modal',root="+root+@",on_close=function(view,reason)
    calls=calls+1; assert(calls==1 and not sf2.ui.is_open(view))
    sf2.ui.close(view)
    sf2.ui.set_text(marker,'label',reason)
end}",false,(ctx,cat,views)=>{
                views[1].Close(reason);views[1].Close();
                Check(views[0].Read("label").Text==reason.ToString().ToLowerInvariant(),"Lua close notification missing/wrong reason: "+string.Join(";",closeLogs));
            },captureLogs:closeLogs);
        }
        Run(prefix+@"local calls=0
local view=sf2.ui.open{id='closer',mount='menu',root="+root+@",on_click=function(view)sf2.ui.close(view)end,
on_close=function(view,reason) calls=calls+1;assert(reason=='script');assert(not sf2.ui.is_open(view)) end}
sf2.ui.close(view);assert(calls==1)
-- Opening after notification returns is supported, including the same ID.
sf2.ui.open{id='closer',mount='menu',root="+root+"}",false);
        Run(prefix+"sf2.ui.open{id='closer',mount='menu',root="+root+@",on_click=function(view)sf2.ui.close(view)end,
on_close=function(view,reason)assert(reason=='script' and not sf2.ui.is_open(view))end}",false,
            (ctx,cat,views)=>Check(views[0].TryClick("go") && views[0].IsClosed,"Nested close callback in click failed"));
        Run(prefix+"sf2.ui.open{id='bad',mount='menu',root="+root+",on_close=3}",true);
        foreach(string closeBody in new[]{"while true do end","error('close failed')","sf2.ui.set_text(view,'label','stale')","sf2.ui.open{id='escape',mount='menu',root="+root+"}"}) {
            var logs=new List<ModLogEntry>();
            Run(prefix+"sf2.ui.open{id='closer',mount='menu',root="+root+",on_close=function(view) "+closeBody+" end}",false,
                (ctx,cat,views)=>{
                    views[0].Close(ModUiCloseReason.Scene);
                    Check(views.Count==1 && views[0].IsClosed && ((IModUiScriptContext)ctx).UiScope.Count==0,"Close failure escaped teardown");
                    Check(logs.Any(log=>log.Level==ModLogLevel.Error),"Close failure was not diagnosed");
                },captureLogs:logs);
        }
        var shutdownLogs=new List<ModLogEntry>();
        Run(prefix+"sf2.ui.open{id='closer',mount='menu',root="+root+",on_close=function()error('shutdown notification')end}",false,
            (ctx,cat,views)=>{ctx.Dispose();Check(shutdownLogs.Count==0,"Disposed script executed Lua close callback");},captureLogs:shutdownLogs);
        var mountLogs=new List<ModLogEntry>();
        Run(prefix+"sf2.ui.open{id='closer',mount='menu',root="+root+",on_close=function()error('failed mount notification')end}",true,
            (ctx,cat,views)=>Check(mountLogs.Count==0,"Failed mount invoked Lua close callback"),failMount:true,captureLogs:mountLogs);
        Run(prefix+"sf2.ui.open{id='styled',mount='hud',root={id='root',kind='text',width=200,height=40,text='Styled',style={font_size=28,text_align='left',text_color='#aBcDeF80'}}}",false,
            (ctx,cat,views)=>Check(views.Single().Root.Style.FontSize==28 && views.Single().Root.Style.TextColor.A==128,"Lua style did not reach model"));
        foreach(string style in new[]{"3","{unknown=true}","{font_size=22.5}","{font_size=1/0}","{font_size='22'}","{text_color='white'}","{text_color=1}","{fill_color='#ffffff'}","{text_align='up'}"})
            Run(prefix+"sf2.ui.open{id='bad',mount='hud',root={id='root',kind='text',width=200,height=40,style="+style+"}}",true,
                (ctx,cat,views)=>Check(views.Count==0,"Invalid style reached renderer"));
        Run(prefix+@"local key=sf2.localization.key('fixture.patch')
assert(sf2.localization.text(key)=='base')
sf2.localization.patch{target='example.charge-ui:localization/fixture.patch',language='eng',value='patched'}
assert(sf2.localization.text(key,'missing')=='patched')
local view=sf2.ui.open{id='patch',mount='menu',root="+root+@",on_click=function()
    assert(sf2.localization.text(key)=='patched')
end}",false,(ctx,cat,views)=>Check(views.Single().TryClick("go"),"Committed localization patch was not readable from a click"));
        Run(prefix+"local view="+open+@"
assert(sf2.ui.is_open(view))
sf2.ui.set_text(view,'label','new')
sf2.ui.set_visible(view,'go',false)
sf2.ui.set_enabled(view,'root',false)
sf2.ui.close(view);sf2.ui.close(view)
assert(not sf2.ui.is_open(view))
",false,(ctx,cat,views)=>Check(views.Single().IsClosed,"Lua close did not release view"));
        foreach(string invalidOperation in new[]{"sf2.ui.set_value(view,'label',0.5)","sf2.ui.close({})",
            "sf2.ui.close(view);sf2.ui.set_text(view,'label','stale')","sf2.ui.set_visible(view,'go',1)",
            "sf2.ui.set_text(view,'label',4)","sf2.ui.set_enabled(view,'go','true')"})
            Run(prefix+"local view="+open+";"+invalidOperation,true);
        foreach(string invalid in new[]{
            "{id='bad',mount='menu',root="+root+",unknown=true}",
            "{id='bad',mount='unknown',root="+root+"}",
            "{id='bad',mount='menu',root="+root+",on_click=3}",
            "{id='bad',mount='menu',root={id='root',kind='text',width=0/0,height=1}}",
            "{id='bad',mount='menu',root={id='root',kind='text',width=1,height=1,text='x',children={"+root+"}}}",
            "{id='bad',mount='menu',root={id='root',kind='column',width=1,height=1,children={[2]="+root+"}}}",
            "{id='bad',mount='menu',root={id='root',kind='column',width=1,height=1,children={"+root+","+root+"}}}"
        }) Run(prefix+"sf2.ui.open"+invalid,true,(ctx,cat,views)=>Check(views.Count==0,"Invalid tree reached renderer"));
        foreach(string placement in new[]{"3","{anchor='unknown'}","{unknown=true}","{x='4'}","{y=1/0}","{x=8193}","{anchor=false}"})
            Run(prefix+"sf2.ui.open{id='bad',mount='hud',root="+root+",placement="+placement+"}",true,
                (ctx,cat,views)=>Check(views.Count==0,"Invalid placement reached renderer"));
        Run(prefix+"local cycle={id='cycle',kind='column',width=1,height=1};cycle.children={cycle};sf2.ui.open{id='cycle',mount='menu',root=cycle}",true);
        Run(prefix+"local view="+open+";error('registration fails')",true,(ctx,cat,views)=>Check(views.Single().IsClosed,"Failed entrypoint cleanup retained mounted UI"));
        Run(prefix+"local view=sf2.ui.open{id='bad',mount='menu',root="+root+",on_click=function() while true do end end}",false,
            (ctx,cat,views)=>Check(!views.Single().TryClick("go") && views.Single().IsClosed,"Unbounded click was not interrupted/closed"));
        Run(prefix+"local view="+open,true,null,host:false);
        Run(prefix+"local view="+open,true,(ctx,cat,views)=>Check(views.Single().IsClosed,"Failed mount retained view"),failMount:true);
        File.WriteAllText(manifest,originalManifest.Replace(", \"ui.create\"",""));
        Run(prefix+"local view="+open,true,(ctx,cat,views)=>Check(views.Count==0,"Missing UI capability reached renderer"));
        Console.WriteLine("PASS: "+checks+" actual Lua UI validation, capability, click budget, handle lifetime and Charged Strike checks.");
    }
    static void Run(string source,bool failure,Action<IModScriptContext,ModContentCatalog,List<ModUiSurface>> inspect=null,bool host=true,bool failMount=false,List<ModLogEntry> captureLogs=null)
    {
        File.WriteAllText(entry,source);
        var mod=ModDiscovery.DiscoverLoose(mods).Mods.Single();
        var catalog=new ModContentCatalog();var stages=new XmlDocument();stages.Load(Path.Combine(repo,"Assets/vanillaXml/stages.xml"));
        CoreContentImporter.ImportStages(catalog,stages.SelectSingleNode("Stages/Zones"));
        var assets=new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)});
        using(var seed=catalog.BeginRegistration(mod)) { seed.AddLocalization("fixture.patch","eng","base");seed.Commit(); }
        var views=new List<ModUiSurface>();
        Action<ModUiSurface> mount=host?(Action<ModUiSurface>)(view=>{views.Add(view);if(failMount)throw new InvalidOperationException("Fixture renderer failure");}):null;
        using(var tx=catalog.BeginRegistration(mod))
        using(var context=new MoonSharpScriptRuntime(mount).CreateContext(mod,new ModApiFacade(mod,assets,tx,new ModStateRuntime(),entry=>captureLogs?.Add(entry))))
        {
            ModLocalizationLoader.Load(mod,assets,tx);
            bool failed=false;string failureMessage="";
            try{context.ExecuteEntrypoint();tx.Commit();}catch(ModScriptException error){failed=true;failureMessage=error.Message;}
            Check(failed==failure,"Unexpected Lua initialization outcome: "+failureMessage);
            if(failed)context.Dispose();
            inspect?.Invoke(context,catalog,views);
        }
        Check(views.All(view=>view.IsClosed),"Context teardown left open UI");
    }
}
