using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

public static class ValidateVisualExamples
{
    sealed class Core : IAssetProvider
    {
        public ModId Namespace => ModId.Parse("core");
        public bool TryDescribe(AssetId id, out AssetMetadata metadata)
        { metadata = new AssetMetadata(id, AssetKind.Sprite, AssetSourceKind.Core, "", -1, "fixture"); return true; }
    }
    sealed class Fighter : IModFighterOperations, IModCombatSnapshotSource, IModIncomingHitSource, IModFighterForms
    {
        public int Frame;
        public Action<bool,string> FormComplete;
        public DefinitionId Form;
        public bool RejectForm;
        public bool TryChangeForm(DefinitionId character, Action<bool,string> complete, out string error)
        { Form=character; FormComplete=complete; error=RejectForm?"fixture preparation rejected":""; return !RejectForm; }
        public ModIncomingHit IncomingHit { get; set; }
        public ModCombatSnapshot CaptureCombatSnapshot() => new ModCombatSnapshot(new ModFighterSnapshot(100,100,1,0,0,0),null,Frame,true);
        public bool TryChangeHealth(double value,out string error) { error=""; return true; }
        public bool TryAddMagicCharge(double value,out string error) { error=""; return true; }
    }
    static int checks;
    static void Check(bool value,string message) { checks++; if (!value) throw new Exception(message); }
    public static void Main(string[] args) { Run(args[0],args[1]); }
    public static void Run(string modsRoot,string stagesPath,Action<ModUiSurface> mount=null)
    {
        checks=0;
        foreach (var name in new[]{"example.pulse-guardian","example.tactic-gallery","example.arena-draft","example.shifting-guardian"})
        {
            var mod=ModDiscovery.DiscoverLoose(modsRoot).Mods.Single(m=>m.Id.Value==name);
            var catalog=new ModContentCatalog();var state=new ModStateRuntime();
            var stages=new XmlDocument();stages.Load(stagesPath);
            CoreContentImporter.ImportWarriorTemplates(catalog,stages.SelectSingleNode("Stages/Warriors/Templates"));
            var assets=new AssetResolver(new IAssetProvider[]{new Core(),new LooseModProvider(mod)});
            var views=new List<ModUiSurface>();var logs=new List<ModLogEntry>();
            using(var tx=catalog.BeginRegistration(mod))
            using(var script=new MoonSharpScriptRuntime(v=>{views.Add(v);mount?.Invoke(v);}).CreateContext(mod,new ModApiFacade(mod,assets,tx,state,logs.Add)))
            {
                ModLocalizationLoader.Load(mod,assets,tx);script.ExecuteEntrypoint();tx.Commit();
                Check(catalog.Modes.Count==1&&catalog.Quests.Count==1,name+" map mode/reveal missing");
                var save=new XmlDocument();save.LoadXml("<Warrior/>");
                ModSaveData.RecordContext(save.DocumentElement,new[]{mod},catalog,state);state.Bind(save.DocumentElement,new[]{script});
                if(name=="example.arena-draft")
                {
                    var prepare=(IModModePrepareScriptContext)script;var mode=catalog.Modes.Single();
                    for(int choice=1;choice<=3;choice++)
                    {
                        var request=new ModModeRequest();
                        Check(prepare.TryPrepareMode(mode,0,0,request,out var error),error);
                        var view=views.Last();view.SetInputAllowed(true);
                        Check(view.TryClick("pick_"+choice)&&view.Read("selection").Text.StartsWith("Selected:"),"Draft selection failed");
                        Check(view.TryChange("challenge",1)&&view.TryChange("duration",1),"Draft controls failed");
                        Check(view.TryClick("begin")&&view.IsClosed&&!request.IsPending&&request.Plan!=null,"Draft did not resolve and close");
                        string suffix=choice==1?"fighter":"fighter_"+choice;
                        Check(request.Plan.Warriors.Single().ToString()==name+":warriors/"+suffix&&request.Plan.Level==4&&request.Plan.RoundTime==90,"Draft plan differs from chosen fighter/settings");
                    }
                    var cancel=new ModModeRequest();Check(prepare.TryPrepareMode(mode,0,0,cancel,out var cancelError),cancelError);
                    views.Last().SetInputAllowed(true);views.Last().TryClick("cancel");
                    Check(!cancel.IsPending&&cancel.Plan==null&&views.Last().IsClosed,"Draft cancellation started an encounter");
                }
                else
                {
                    var fighter=new Fighter();var behavior=catalog.FightRules.Single().Behavior;
                    var fields=new Dictionary<string,string>{{"round","1"},{"source","rule"},{"fight_id",name}};
                    var interactive=(IModInteractiveBehaviorScriptContext)script;
                    Action<ModEffectEvent> invoke=kind=>{Check(interactive.TryInvokeBehavior(behavior,kind,null,fields,fighter,out var error),error);};
                    invoke(ModEffectEvent.RoundBegin);var view=views.Last();
                    if(name=="example.shifting-guardian")
                    {
                        for(int frame=1;frame<=180;frame++){fighter.Frame=frame;invoke(ModEffectEvent.Tick);}
                        Check(fighter.Form.ToString()==name+":warriors/baton"&&fighter.FormComplete!=null,"Form handle did not reach native host");
                        Check(view.Read("phase").Text=="STAFF FORM"&&view.Read("result").Text.Contains("queued"),"Queued form falsely reported as applied");
                        fighter.FormComplete(true,"");fighter.Frame++;invoke(ModEffectEvent.Tick);
                        Check(view.Read("phase").Text=="BATON FORM"&&view.Read("result").Text.StartsWith("Applied:"),"Completion receipt did not update Lua HUD");
                        invoke(ModEffectEvent.RoundEnd);invoke(ModEffectEvent.RoundBegin);view=views.Last();
                        for(int frame=1;frame<=180;frame++){fighter.Frame=frame;invoke(ModEffectEvent.Tick);}
                        fighter.FormComplete(false,"fixture cancellation");fighter.Frame++;invoke(ModEffectEvent.Tick);
                        Check(view.Read("phase").Text=="STAFF FORM"&&view.Read("result").Text=="Failed: fixture cancellation","Failed receipt was hidden or treated as successful");
                        invoke(ModEffectEvent.RoundEnd);invoke(ModEffectEvent.RoundBegin);view=views.Last();fighter.RejectForm=true;
                        for(int frame=1;frame<=180;frame++){fighter.Frame=frame;invoke(ModEffectEvent.Tick);}
                        Check(view.Read("result").Text=="Failed: fixture preparation rejected","Preparation rejection did not return a usable failed receipt");
                    }
                    else if(name=="example.pulse-guardian")
                    {
                        double damage=10;fighter.IncomingHit=new ModIncomingHit(()=>damage,n=>damage=n);
                        invoke(ModEffectEvent.DamageResolving);
                        Check(damage==0&&view.Read("hits").Text=="Stopped: 1   Landed: 0","Shield failed to stop actual incoming damage");
                        for(int frame=1;frame<=180;frame++){fighter.Frame=frame;invoke(ModEffectEvent.Tick);}
                        Check(view.Read("phase").Text.StartsWith("SHIELD DOWN"),"Vulnerable window not visible");
                        damage=10;invoke(ModEffectEvent.DamageResolving);
                        Check(damage==10&&view.Read("hits").Text=="Stopped: 1   Landed: 1","Vulnerable damage/counter failed");
                        for(int frame=181;frame<=360;frame++){fighter.Frame=frame;invoke(ModEffectEvent.Tick);}
                        Check(view.Read("phase").Text.StartsWith("SHIELD UP"),"Shield cycle did not restart");
                    }
                    else
                    {
                        var ai=(IModAiScriptContext)script;
                        var snapshot=new ModCombatSnapshot(new ModFighterSnapshot(40,50,1,10,0,0),new ModFighterSnapshot(30,50,1,60,0,0),60,true);
                        foreach(var brain in new[]{"patient","footwork","alternating"})
                        {
                            var actor=new object();
                            Check(ai.TryDecideAi(name+":tactics/"+brain,actor,snapshot,new[]{"HighKick","LowKick","KatanaStepBack","KatanaStepForward"},out var selected,out var error),error);
                            Check(selected==(brain=="footwork"?2:0)&&view.Read("decision").Text.Contains(brain=="footwork"?"KatanaStepBack":"HighKick"),"AI decision/HUD mismatch");
                            Check(ai.TryDecideAi(name+":tactics/"+brain,actor,snapshot,new[]{"HighKick"},out selected,out error)&&selected==-1&&view.Read("decision").Text=="AI choice: wait","AI pause not visible: "+error);
                        }
                    }
                    invoke(ModEffectEvent.RoundEnd);Check(view.IsClosed,"Combat HUD leaked past round end");
                    invoke(ModEffectEvent.RoundBegin);invoke(ModEffectEvent.FightEnd);Check(views.Last().IsClosed,"Combat HUD leaked past fight end");
                }
                Check(logs.All(l=>l.Level!=ModLogLevel.Error),name+" logged errors");
            }
        }
        Console.WriteLine("PASS: "+checks+" visual showcase Lua checks; guardian damage/HUD cycle, AI decisions/HUD and draft selections/cancellation. Combat operations and asset metadata controlled.");
    }
}
