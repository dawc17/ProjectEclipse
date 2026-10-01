using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

sealed class PortraitMetadata : IAssetProvider
{
    public ModId Namespace=>ModId.Parse("core");
    public bool TryDescribe(AssetId id,out AssetMetadata metadata) {
        metadata=null;if(id.Namespace!=Namespace||!id.Path.StartsWith("ui/users/"))return false;
        metadata=new AssetMetadata(id,AssetKind.Sprite,AssetSourceKind.Core,"",-1,"controlled portrait metadata");return true;
    }
}
static class Program
{
    sealed class Lease:IDisposable {Action close;internal Lease(Action close){this.close=close;}public void Dispose(){var action=close;close=null;action?.Invoke();}}
    static int checks;
    static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    static XmlDocument Read(string path){var doc=new XmlDocument();doc.Load(path);return doc;}
    static ModUiNode Node(ModUiNode root,string id)=>root.Id==id?root:root.Children.Select(child=>Node(child,id)).FirstOrDefault(value=>value!=null);
    static XmlDocument Save(string xml=null){var doc=new XmlDocument();doc.LoadXml(xml??"<Warrior><EclipseMods schema='1'><Mod id='fixture.notify'/></EclipseMods></Warrior>");return doc;}
    static void RegistryChecks()
    {
        var registry=new ModFightEntries();var owner=ModId.Parse("fixture.notify");var id=DefinitionId.Parse("fixture.notify:fights/one");
        ModFightEntryRequest request=null;bool valid=true,ready=true;int launched=0;
        var binding=registry.Register(owner,id,r=>{request=r;return null;});
        Func<bool> launch=()=>{launched++;return true;};
        Check(registry.Begin(id,launch,()=>valid,()=>ready)==ModFightEntryDecision.Deferred,"Deferred entry failed");
        Check(registry.Begin(DefinitionId.Parse("fixture.notify:fights/other"),launch,()=>true,()=>true)==ModFightEntryDecision.Cancelled,"Concurrent entry passed");
        ready=false;Check(!request.Resume()&&request.IsPending&&launched==0,"Not-ready resume consumed entry");
        ready=true;Check(request.Resume()&&!request.Resume()&&launched==1&&!registry.HasPending,"Resume duplicated");
        registry.Begin(id,launch,()=>valid,()=>ready);valid=false;Check(!request.IsPending&&!request.Resume()&&launched==1,"Invalid profile/scene resumed");
        valid=true;registry.Begin(id,launch,()=>valid,()=>ready);binding.Dispose();Check(!request.Resume()&&!registry.Contains(id),"Scope removal retained request");
        registry.Register(owner,id,r=>{r.Resume();return true;});Check(registry.Begin(id,launch,()=>true,()=>true)==ModFightEntryDecision.Cancelled&&!registry.Contains(id),"Reentrant resume not rejected");
        registry.Register(owner,id,r=>false);Check(registry.Begin(id,launch,()=>true,()=>true)==ModFightEntryDecision.Cancelled&&!registry.HasPending,"False did not cancel");registry.Clear();
        registry.Register(owner,id,r=>true);Check(registry.Begin(id,launch,()=>true,()=>true)==ModFightEntryDecision.Continue&&!registry.HasPending&&launched==1,"Pass-through invoked continuation");registry.Clear();
    }
    static void BindingChecks(string mods)
    {
        string directory=Path.Combine(mods,"fixture.notify");string manifestPath=Path.Combine(directory,"mod.toml");string manifest=File.ReadAllText(manifestPath);
        const string prefix="local sf2=require('sf2');local zone=sf2.zones.register{id='test'};local battle=sf2.battles.register{id='test',zone=zone,type=sf2.battles.STORY};local warrior=sf2.warriors.register{id='dummy'};local fight=sf2.fights.register{id='test',battle=battle,warriors={warrior}};";
        void Run(string source,bool registrationFails,bool callbackFails,bool capability=true)
        {
            File.WriteAllText(manifestPath,capability?manifest:manifest.Replace("\"story.progression\", ",""));
            File.WriteAllText(Path.Combine(directory,"scripts/main.lua"),prefix+source);
            var mod=ModDiscovery.DiscoverLoose(mods).Mods.Single();var catalog=new ModContentCatalog();var logs=new List<string>();var bus=new ModStoryEvents((owner,error)=>logs.Add(error));
            using(var tx=catalog.BeginRegistration(mod))
            using(var context=new MoonSharpScriptRuntime(null,null,null,bus).CreateContext(mod,new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx,new ModStateRuntime(),entry=>logs.Add(entry.Message))))
            {
                bool failed=false;try{context.ExecuteEntrypoint();tx.Commit();}catch(ModScriptException){failed=true;}
                Check(failed==registrationFails,"Unexpected fight-entry registration outcome");
                var id=DefinitionId.Parse("fixture.notify:fights/test");
                if(callbackFails) {
                    Check(bus.FightEntries.Begin(id,()=>throw new Exception("Error callback launched"),()=>true,()=>true)==ModFightEntryDecision.Cancelled,"Invalid callback allowed entry");
                    Check(logs.Count==1&&!bus.FightEntries.Contains(id)&&!bus.FightEntries.HasPending,"Failed callback retained registration/request");
                }
                context.Dispose();Check(!bus.FightEntries.Contains(id),"Script teardown leaked registration");
            }
        }
        Run("sf2.story.before_fight({},function() return true end)",true,false);
        Run("sf2.story.before_fight(fight,42)",true,false);
        Run("sf2.story.before_fight(fight,function() return true end);sf2.story.before_fight(fight,function() return true end)",true,false);
        Run("sf2.story.before_fight(fight,function() return true end)",true,false,false);
        foreach(var body in new[]{"return 42","error('fixture callback error')","while true do end","return sf2.story.resume_fight(request)","return sf2.story.fight_pending({fight=request.fight})"})
            Run("sf2.story.before_fight(fight,function(request) "+body+" end)",false,true);
        File.WriteAllText(manifestPath,manifest);
    }
    static void Main(string[] args)
    {
        RegistryChecks();
        string package=Path.Combine(args[0],"fixture.notify");
        File.WriteAllText(Path.Combine(package,"mod.toml"),"""
schema = 1
id = "fixture.notify"
name = "Sensei entry checks"
version = "1.0.0"
authors = ["Eclipse tests"]
entrypoint = "scripts/main.lua"
capabilities = ["content.register", "story.events", "story.progression", "state.read", "state.write", "ui.create"]
[[dependencies]]
id = "core"
version = ">=1.0 <2.0"
""");
        File.WriteAllText(Path.Combine(package,"scripts/main.lua"),"""
local sf2=require("sf2")
local zone=sf2.zones.register{id="test"}
local warrior=sf2.warriors.register{id="dummy"}
local normal,portraits={},{}
for act=1,6 do
 local battle=sf2.battles.register{id="act"..act,zone=zone,type=sf2.battles.STORY}
 normal[act]={}
 for index=1,(act==6 and 2 or 3) do
  normal[act][index]=sf2.fights.register{id="act"..act.."_"..index,battle=battle,warriors={warrior}}
 end
end
for _,sequence in ipairs(require("content.sensei_entry_data")) do
 for _,card in ipairs(sequence.cards) do
  if card.portrait then portraits[card.portrait]=sf2.assets.sprite("core:ui/users/"..card.portrait) end
 end
end
require("content.sensei_entry").install(normal,portraits)
""");
        var mod=ModDiscovery.DiscoverLoose(args[0]).Mods.Single();var catalog=new ModContentCatalog();
        var stages=Read(Path.Combine(args[1],"Assets/vanillaXml/stages.xml"));CoreContentImporter.ImportStages(catalog,stages.SelectSingleNode("Stages/Zones"));
        var assets=new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod),new PortraitMetadata()});
        var errors=new List<string>();var dialogs=new FakeDialogHost(catalog);var views=dialogs.Views;var layers=dialogs;var bus=new ModStoryEvents((id,error)=>errors.Add(error));var state=new ModStateRuntime();
        Action<bool> finish=null;IReadOnlyList<ModActScreenLine> lines=null;
        ModActScreenAccess.Open=(value,done)=>{lines=value;finish=done;return new Lease(()=>done(false));};
        using(var tx=catalog.BeginRegistration(mod))
        using(var context=new MoonSharpScriptRuntime(null,null,null,bus).CreateContext(mod,new ModApiFacade(mod,assets,tx,state,entry=>errors.Add(entry.Message))))
        {
            ModLocalizationLoader.Load(mod, new AssetResolver(new IAssetProvider[] { new LooseModProvider(mod) }), tx);
            context.ExecuteEntrypoint();tx.Commit();var save=Save();Check(state.Bind(save.DocumentElement,new[]{context}).Count==0,"Bind failed");bus.BindProfile();
            var archive=Read(Path.Combine(args[1],"Assets/DExml/quests.xml"));var english=Read(Path.Combine(args[1],"Assets/DExml/localizations/eng.xml"));
            string Text(string key)=>english.SelectSingleNode("//Word[@Title='"+key+"']").InnerText;
            bool Flag(int act,int index)=>state.TryGetValue(mod.Id,"sensei_entered_"+act+"_"+index,out var value)&&value.Boolean;
            int translated=0;
            foreach(var definition in catalog.Localizations.Where(value=>value.Id.LocalId.StartsWith("sensei.entry.")))
            foreach(var file in Directory.GetFiles(Path.Combine(args[1],"Assets/DExml/localizations"),"*.xml")) {
                var key=definition.Id.LocalId.Substring("sensei.entry.".Length);
                var word=Read(file).SelectNodes("//Word").Cast<XmlNode>().SingleOrDefault(value=>string.Equals(value.Attributes["Title"].Value,key,StringComparison.OrdinalIgnoreCase));
                if(word==null)continue;
                Check(definition.TryGet(Path.GetFileNameWithoutExtension(file),out var value)&&value==word.InnerText,"Translation differs: "+key);translated++;
            }
            Check(translated==938,"Incomplete entry translations");int cards=0,timed=0,entries=0;
            for(int act=1;act<=6;act++)for(int index=1;index<=(act==6?2:3);index++) {
                var quest=archive.SelectSingleNode("//Quest[Events/FightEnter and Conditions/Equal[@Value1='_$Fight' and @Value2='ZONE_"+act+"|SENSEI_MEMORIES|"+index+"']]");
                Check(quest!=null,"Missing source entry");var id=DefinitionId.Parse("fixture.notify:fights/act"+act+"_"+index);int launched=0;
                var sourceFlag=quest.SelectSingleNode("Conditions/Equal[@Not='1']").Attributes["Value1"].Value.Substring(1);
                bool before=quest.SelectSingleNode("Actions/Dialog/Button/SetVariable[@Name='"+sourceFlag+"']")!=null;
                Check(bus.FightEntries.Begin(id,()=>{Check(Flag(act,index)==before,"Completion flag timing changed");
                    if(act==6&&index==1)Check(state.TryGetValue(mod.Id,"sensei_shogun_greeted",out var shogun)&&shogun.Boolean,"Separate Shogun acknowledgement missing before launch");launched++;return true;},()=>true,()=>true)==ModFightEntryDecision.Deferred,"Entry not held");entries++;
                foreach(XmlNode action in quest.SelectSingleNode("Actions").ChildNodes) {
                    if(action.Name=="ActScreen") {
                        var expected=action.SelectNodes("Line");Check(lines!=null&&lines.Count==expected.Count,"Timed lines missing");
                        for(int i=0;i<expected.Count;i++){Check(lines[i].Text==Text(expected[i].Attributes["Text"].Value)&&lines[i].Frames==int.Parse(expected[i].Attributes["Frames"].Value),"Timed line mismatch");timed++;}
                        finish(true);continue;
                    }
                    if(action.Name!="Dialog")continue;
                    var view=views.Last(value=>!value.IsClosed);var button=action.SelectSingleNode("Button");
                    Check(view.Read("speaker").Text==Text(action.Attributes["Title"].Value)&&view.Read("body").Text==Text(action.SelectSingleNode("Line").Attributes["Text"].Value),"Dialogue order/text mismatch");
                    Check(view.Read("continue").Text==Text(button.Attributes["Text"].Value),"Button text mismatch");
                    Check(view.Read("portrait").Sprite==AssetId.Parse("core:ui/users/"+action.Attributes["Image"].Value)&&view.Read("portrait").Mirrored==(action.Attributes["Mirrored"]?.Value=="1"),"Portrait mismatch");
                    Check(view.Request.IgnoreBack==(action.Attributes["IgnoreBack"]?.Value=="1")&&view.Request.Lines.Count==1,"Native IgnoreBack/line request differs");
                    layers.SetBlocked(true);Check(!view.TryClick("continue"),"Blocked entry advanced");layers.SetBlocked(false);
                    if(action.Attributes["IgnoreBack"]?.Value=="1"){layers.Back();Check(!view.IsClosed&&launched==0,"IgnoreBack launched fight");}
                    Check(view.TryClick("continue")&&view.IsClosed,"Card did not advance");cards++;
                }
                Check(launched==1&&Flag(act,index)&&!bus.FightEntries.HasPending,"Entry did not launch exactly once");
                Check(bus.FightEntries.Begin(id,()=>throw new Exception("Unexpected resume"),()=>true,()=>true)==ModFightEntryDecision.Continue,"Acknowledged entry replayed");
            }
            Check(entries==17&&cards==39&&timed==7,"Source coverage incomplete");
            var completed=save.OuterXml;bus.UnbindProfile();state.Unbind();Check(state.Bind(Save(completed).DocumentElement,new[]{context}).Count==0,"Reload failed");bus.BindProfile();
            Check(Flag(1,1)&&Flag(6,2),"Saved entry flags lost");
            bus.UnbindProfile();state.Unbind();Check(state.Bind(Save().DocumentElement,new[]{context}).Count==0,"New profile failed");bus.BindProfile();Check(!Flag(1,1),"New profile inherited entry");
            var first=DefinitionId.Parse("fixture.notify:fights/act1_1");bus.FightEntries.Begin(first,()=>throw new Exception("Cancelled entry launched"),()=>true,()=>true);bus.UnbindProfile();finish(true);Check(!bus.FightEntries.HasPending&&!Flag(1,1),"Stale timed callback advanced profile");
            bus.BindProfile();bus.FightEntries.Begin(first,()=>throw new Exception("Closed dialogue launched"),()=>true,()=>true);finish(true);
            var interrupted=views.Last(value=>!value.IsClosed);string firstText=interrupted.Read("body").Text;
            interrupted.Close(ModUiCloseReason.Scene);Check(!bus.FightEntries.HasPending&&!Flag(1,1),"Closed entry left a held fight or completion flag");
            bus.FightEntries.Begin(first,()=>throw new Exception("Repeated dialogue launched"),()=>true,()=>true);finish(true);
            Check(views.Last(value=>!value.IsClosed).Read("body").Text==firstText,"Interrupted intro did not restart at its first dialogue");
            context.Dispose();Check(!bus.FightEntries.Contains(first),"Context retained entry handlers");Check(errors.Count==0,string.Join("\n",errors));
        }
        ModActScreenAccess.Clear();BindingChecks(args[0]);Console.WriteLine("PASS: "+checks+" Sensei entry checks; 17 entries, 39 cards, 7 timed lines and 938 translations. Controlled portraits and launch host; no complete story playtest claim.");
    }
}
