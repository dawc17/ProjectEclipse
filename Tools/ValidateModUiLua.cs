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
            context.Dispose();Check(views[1].IsClosed,"Script shutdown retained example HUD");
        });
        const string root="{id='root',kind='column',width=200,height=100,children={{id='label',kind='text',width=200,height=40,text='old'},{id='go',kind='button',width=200,height=40,text='Go'}}}";
        const string prefix="local sf2=require('sf2')\n";
        string open="sf2.ui.open{id='test',mount='menu',root="+root+"}";
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
    static void Run(string source,bool failure,Action<IModScriptContext,ModContentCatalog,List<ModUiSurface>> inspect=null,bool host=true,bool failMount=false)
    {
        File.WriteAllText(entry,source);
        var mod=ModDiscovery.DiscoverLoose(mods).Mods.Single();
        var catalog=new ModContentCatalog();var stages=new XmlDocument();stages.Load(Path.Combine(repo,"Assets/vanillaXml/stages.xml"));
        CoreContentImporter.ImportStages(catalog,stages.SelectSingleNode("Stages/Zones"));
        var assets=new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)});
        var views=new List<ModUiSurface>();
        Action<ModUiSurface> mount=host?(Action<ModUiSurface>)(view=>{views.Add(view);if(failMount)throw new InvalidOperationException("Fixture renderer failure");}):null;
        using(var tx=catalog.BeginRegistration(mod))
        using(var context=new MoonSharpScriptRuntime(mount).CreateContext(mod,new ModApiFacade(mod,assets,tx,new ModStateRuntime(),null)))
        {
            bool failed=false;string failureMessage="";
            try{context.ExecuteEntrypoint();tx.Commit();}catch(ModScriptException error){failed=true;failureMessage=error.Message;}
            Check(failed==failure,"Unexpected Lua initialization outcome: "+failureMessage);
            if(failed)context.Dispose();
            inspect?.Invoke(context,catalog,views);
        }
        Check(views.All(view=>view.IsClosed),"Context teardown left open UI");
    }
}
