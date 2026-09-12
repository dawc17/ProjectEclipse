using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

sealed class Core : IAssetProvider
{
    public ModId Namespace => ModId.Parse("core");
    public bool TryDescribe(AssetId id,out AssetMetadata metadata)
    { metadata=new AssetMetadata(id,AssetKind.Sprite,AssetSourceKind.Core,"",-1,"fixture"); return true; }
}
static class Program
{
    static int checks;
    static void Check(bool value,string message) { checks++; if (!value) throw new Exception(message); }
    static void Main(string[] args)
    {
        var mod=ModDiscovery.DiscoverLoose(args[0]).Mods.Single();
        var catalog=new ModContentCatalog(); var state=new ModStateRuntime();
        var stages=new XmlDocument(); stages.Load(Path.Combine(args[1],"Assets/vanillaXml/stages.xml"));
        CoreContentImporter.ImportWarriorTemplates(catalog,stages.SelectSingleNode("Stages/Warriors/Templates"));
        var assets=new AssetResolver(new IAssetProvider[]{new Core(),new LooseModProvider(mod)});
        var views=new List<ModUiSurface>();
        using(var tx=catalog.BeginRegistration(mod))
        using(var script=new MoonSharpScriptRuntime(views.Add).CreateContext(mod,new ModApiFacade(mod,assets,tx,state,log=>Console.WriteLine(log.Message))))
        {
            ModLocalizationLoader.Load(mod,assets,tx); script.ExecuteEntrypoint(); tx.Commit();
            var mode=catalog.Modes.Single();
            var save=new XmlDocument(); save.LoadXml("<Warrior/>"); ModSaveData.RecordContext(save.DocumentElement,new[]{mod},catalog,state); state.Bind(save.DocumentElement,new[]{script});
            ModPolicies.Content=catalog; ModModeRuntime.Bind(save.DocumentElement);
            foreach(var id in mode.Fights)
            {
                catalog.TryGetFight(id,out var definition); catalog.TryGetBattle(definition.Battle,out var battle);
                string runtime=catalog.RuntimeFightId(id);
                ListSF.Fights[runtime]=new FightList { BCKFACGMOKC=new FightIDS(runtime),CNAOMDMIGLJ=new Battle {Name=battle.LegacyName} };
            }
            Action ready=null,cancel=null; ModModeRequest request=null;
            ModModeRuntime.SchedulePreparation=(r,onReady,onCancel)=>{ request=r; ready=onReady; cancel=onCancel; };
            ModModeRuntime.Prepare=(m,step,count,r)=>{
                Check(((IModModePrepareScriptContext)script).TryPrepareMode(m,step,count,r,out var error),error);
            };
            int builds=0;
            ModModeRuntime.BuildEncounter=(m,step,plan)=>{
                builds++; Check(plan.Warriors.Count==1 && plan.Level>=1 && plan.RoundTime>=30,"Generated plan lost values");
                catalog.TryGetFight(m.Fights[step],out var blueprint);
                var projectionType=typeof(ModContentCatalog).Assembly.GetType("Projection",true);
                var projection=Activator.CreateInstance(projectionType,true);
                var xml=(XmlElement)projectionType.GetMethod("Encounter").Invoke(projection,new object[]{catalog,blueprint,plan});
                Check(xml.GetAttribute("RoundTime")==plan.RoundTime.ToString() && xml.GetAttribute("Rounds")==plan.Rounds.ToString() &&
                    xml.SelectSingleNode("Warriors/Warrior")?.Attributes["EclipseCharacterId"]?.Value==plan.Warriors[0].ToString() &&
                    xml.SelectSingleNode("Warriors/Warrior")?.Attributes["Level"]?.Value==plan.Level.ToString(),"Native encounter projection lost generated choices");
                Check(xml.GetAttribute("Location")==blueprint.Location && xml.SelectNodes("Rewards/Reward").Count==blueprint.Rewards.Count && blueprint.RoundTime==99,"Encounter changed its blueprint or lost presentation/rewards");
                return ListSF.Fights[catalog.RuntimeFightId(m.Fights[step])];
            };
            var entry=ListSF.Fights[catalog.RuntimeFightId(mode.Fights[0])];
            int resumed=0;
            Check(!ModModeRuntime.PrepareEntry(entry,()=>resumed++),"Pending mode entered immediately");
            Check(request.IsPending && views.Count==1 && ListSF.Roster.Saves==0,"Preparation charged/saved before a choice");
            Check(!ModModeRuntime.PrepareEntry(entry,()=>resumed++) && views.Count==1,"Double click duplicated setup");
            views[0].Close(ModUiCloseReason.Back);
            Check(!request.IsPending && request.Plan==null,"Back did not cancel pending request"); cancel();
            Check(!ModModeRuntime.PrepareEntry(entry,()=>resumed++),"Second preparation skipped choice");
            var view=views[1]; Check(view.TryChange("challenge",1) && view.TryChange("duration",1),"Options did not change");
            Check(view.TryClick("begin") && view.IsClosed && request.Plan!=null && !request.IsPending,"Choice did not resolve request");
            Check(resumed==0,"Choice resumed game inside Lua callback");
            ready(); Check(resumed==1 && builds==1,"Deferred ready action failed");
            var plan=new ModModeProgress(save.DocumentElement,mode).ReadPlan();
            Check(plan.Level==4 && plan.RoundTime==90 && plan.Rounds==1,"Chosen difficulty/time not saved");
            var clone=new XmlDocument(); clone.LoadXml(save.OuterXml); ModModeRuntime.Bind(clone.DocumentElement); state.Bind(clone.DocumentElement,new[]{script});
            Check(ModModeRuntime.PrepareEntry(entry,()=>resumed++) && views.Count==2,"Reload rerolled or reopened completed choice");
            var restored=new ModModeProgress(clone.DocumentElement,mode).ReadPlan();
            Check(restored.Warriors[0]==plan.Warriors[0] && restored.Level==4,"Reload changed generated opponent");
            Check(ModModeRuntime.ResolveEntry(ref entry) && ModModeRuntime.Begin(entry),"Prepared native entry failed");
            ModModeRuntime.Complete(entry,true); ModModeRuntime.Complete(entry,true);
            var progress=new ModModeProgress(clone.DocumentElement,mode);
            Check(progress.Step==1 && progress.ReadPlan()==null,"Settlement did not consume plan once");
            Check(!ModModeRuntime.PrepareEntry(entry,()=>resumed++),"Next encounter skipped preparation");
            ModModeRuntime.Clear(); Check(!request.IsPending && request.Plan==null,"Scene/profile teardown retained request");
            script.Dispose(); Check(views.All(v=>v.IsClosed),"Script teardown retained setup UI");
            var invalid=new ModEncounterPlan(level:2); var p=new ModModeProgress(clone.DocumentElement,mode); p.SavePlan(invalid);
            clone.DocumentElement.SelectSingleNode("EclipseModes/Mode/Encounter").Attributes["Version"].Value="99";
            string before=clone.OuterXml; bool rejected=false;
            try { p.ReadPlan(); } catch(ModContentException) { rejected=true; }
            Check(rejected && clone.OuterXml==before,"Future saved plan was silently overwritten");
        }
        string entryFile=Path.Combine(mod.RootPath,"scripts/main.lua");
        string original=File.ReadAllText(entryFile);
        foreach(string body in new[]{"return {level=2,rounds=1,round_time=45}","return {rounds=-1}","return {warriors={{}}}","sf2.modes.resolve({},{}); return nil","sf2.modes.resolve(request,{}); return {}","while true do end"})
        {
            File.WriteAllText(entryFile,original.Replace("on_prepare = function(request, event)","on_prepare = function(request, event) do "+body+" end"));
            assets=new AssetResolver(new IAssetProvider[]{new Core(),new LooseModProvider(mod)});
            var fresh=new ModContentCatalog(); CoreContentImporter.ImportWarriorTemplates(fresh,stages.SelectSingleNode("Stages/Warriors/Templates"));
            using(var tx=fresh.BeginRegistration(mod))
            using(var script=new MoonSharpScriptRuntime().CreateContext(mod,new ModApiFacade(mod,assets,tx,new ModStateRuntime(),null)))
            {
                ModLocalizationLoader.Load(mod,assets,tx); script.ExecuteEntrypoint(); tx.Commit();
                var request=new ModModeRequest();
                bool ok=((IModModePrepareScriptContext)script).TryPrepareMode(fresh.Modes.Single(),0,0,request,out var error);
                if(body.StartsWith("return {level")) Check(ok && !request.IsPending && request.Plan.Level==2,"Synchronous generator failed: "+error);
                else Check(!ok && !request.IsPending && request.Plan==null && error!=null,"Malformed/unbounded preparation retained a live request");
            }
        }
        File.WriteAllText(entryFile,original);
        Console.WriteLine("PASS: "+checks+" shipped procedural/async Lua, native XML projection, cancellation, deferred entry, persistence, settlement and malformed callback checks. Full-game parsing/rendering remains unverified.");
    }
}
