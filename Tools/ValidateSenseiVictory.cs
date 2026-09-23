using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

sealed class PortraitMetadata : IAssetProvider
{
    internal static readonly string[] Names={"character_prince","character_sensei_young","boss_hermit_young","boss_butcher_young","boss_wasp_young","boss_widow_young","character_ancient","character_sensei"};
    public ModId Namespace=>ModId.Parse("core");
    public bool TryDescribe(AssetId id,out AssetMetadata metadata) {
        metadata=null;if(id.Namespace!=Namespace||!Names.Any(name=>id.Path=="ui/users/"+name))return false;
        metadata=new AssetMetadata(id,AssetKind.Sprite,AssetSourceKind.Core,"",-1,"controlled portrait metadata");return true;
    }
}
static class Program
{
    sealed class CancelLease : IDisposable {
        Action cancel; internal CancelLease(Action value){cancel=value;}
        public void Dispose(){var action=cancel;cancel=null;action?.Invoke();}
    }
    static int checks;
    static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    static XmlDocument Read(string path){var doc=new XmlDocument();doc.Load(path);return doc;}
    static XmlDocument Save(string xml=null){var doc=new XmlDocument();doc.LoadXml(xml??"<Warrior><EclipseMods schema='1'><Mod id='fixture.notify'/></EclipseMods></Warrior>");return doc;}
    static ModUiNode Node(ModUiNode root,string id)=>root.Id==id?root:root.Children.Select(child=>Node(child,id)).FirstOrDefault(value=>value!=null);
    static void Main(string[] args)
    {
        string package=Path.Combine(args[0],"fixture.notify");
        File.WriteAllText(Path.Combine(package,"mod.toml"),"""
schema = 1
id = "fixture.notify"
name = "Sensei victory checks"
version = "1.0.0"
authors = ["Eclipse tests"]
entrypoint = "scripts/main.lua"
capabilities = ["content.register", "story.events", "story.progression", "profile.read", "state.read", "state.write", "ui.create"]
[[dependencies]]
id = "core"
version = ">=1.0 <2.0"
""");
        File.WriteAllText(Path.Combine(package,"scripts/main.lua"),"""
local sf2=require("sf2")
local zone=sf2.zones.register{id="test"}
local battles,finals,ids,portraits={},{},{},{}
for act=1,6 do
 battles[act]=sf2.battles.register{id="act"..act,zone=zone,type=sf2.battles.STORY}
 ids[act]="fixture.notify:fights/final"..act
 if act<6 then finals[act]="core:fights/zone_"..act.."/tournament/1" end
end
for _,name in ipairs{"character_prince","character_sensei_young","boss_hermit_young","boss_butcher_young","boss_wasp_young","boss_widow_young","character_ancient"} do
 portraits[name]=sf2.assets.sprite("core:ui/users/"..name)
end
require("content.sensei_victory").install(ids,portraits)
require("content.sensei_notifications").install(battles,finals)
""");
        var mod=ModDiscovery.DiscoverLoose(args[0]).Mods.Single();var catalog=new ModContentCatalog();
        var stages=Read(Path.Combine(args[1],"Assets/vanillaXml/stages.xml"));
        CoreContentImporter.ImportStages(catalog,stages.SelectSingleNode("Stages/Zones"));
        var assets=new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod),new PortraitMetadata()});
        var errors=new List<string>();var logs=new List<string>();var dialogs=new FakeDialogHost(catalog);var views=dialogs.Views;var layers=dialogs;
        var bus=new ModStoryEvents((id,message)=>errors.Add(message));var state=new ModStateRuntime();
        int profileWins=1;ModProfileAccess.Fight=id=>new ModProfileFightSnapshot(true,profileWins,0);
        ModProfileAccess.SetEclipseMode=value=>true;ModBattleAccess.Reveal=(id,value)=>true;
        ModBattleAccess.SetLocked=(id,value)=>true;ModBattleAccess.Focus=id=>true;
        Action<bool> outro=null;
        IDisposable request=null;
        int attempts=0;
        ModActScreenAccess.Open=(lines,done)=>{
            Check(lines.Count==1&&lines[0].Frames==180&&lines[0].Text==Read(Path.Combine(args[1],"Assets/DExml/localizations/eng.xml")).SelectSingleNode("//Word[@Title='Sensei_arc_outro']").InnerText,"Native outro request differs");
            if(++attempts==1){logs.Add("OUTRO_REFUSED");return null;}
            logs.Add("OUTRO");outro=done;return request=new CancelLease(()=>done(false));
        };
        using(var tx=catalog.BeginRegistration(mod))
        using(var context=new MoonSharpScriptRuntime(null,null,null,bus)
            .CreateContext(mod,new ModApiFacade(mod,assets,tx,state,entry=>logs.Add(entry.Message))))
        {
            context.ExecuteEntrypoint();tx.Commit();
            var source=Read(Path.Combine(args[1],"Assets/DExml/quests.xml"));
            var english=Read(Path.Combine(args[1],"Assets/DExml/localizations/eng.xml"));
            int translations=0;
            foreach(var definition in catalog.Localizations.Where(value=>value.Id.LocalId.StartsWith("sensei.victory."))) {
                var localKey=definition.Id.LocalId.Substring("sensei.victory.".Length);
                var key=english.SelectNodes("//Word").Cast<XmlNode>().Single(word=>string.Equals(word.Attributes["Title"].Value,localKey,StringComparison.OrdinalIgnoreCase)).Attributes["Title"].Value;
                foreach(var file in Directory.GetFiles(Path.Combine(args[1],"Assets/DExml/localizations"),"*.xml")) {
                    var word=Read(file).SelectSingleNode("//Word[@Title='"+key+"']");if(word==null)continue;
                    Check(definition.TryGet(Path.GetFileNameWithoutExtension(file),out var value)&&value==word.InnerText,"Translation differs: "+key);translations++;
                }
            }
            Check(translations==448,"Incomplete victory translations");
            var save=Save();
            void Bind(XmlDocument doc){Check(state.Bind(doc.DocumentElement,new[]{context}).Count==0,"State bind failed");bus.BindProfile();}
            bool Flag(string kind,int act)=>state.TryGetValue(mod.Id,"sensei_"+kind+"_"+act,out var value)&&value.Boolean;
            void Scene(string scene){
                var stale=outro;
                if(scene!="map"){request?.Dispose();request=null;}
                bus.Publish(new ModStoryEvent(ModStoryEventKind.SceneEnter,null,scene:scene));
                if(scene!="map")stale?.Invoke(true); // A cancelled native callback cannot acknowledge the outro.
            }
            void Result(string outcome,string id)=>bus.Publish(new ModStoryEvent(ModStoryEventKind.BattleResult,null,battle:new ModBattleResultSnapshot(DefinitionId.Parse(id),outcome,false)));
            FakeDialog Live()=>views.LastOrDefault(value=>!value.IsClosed) ?? throw new Exception("No live story dialog: "+string.Join(" | ",errors.Concat(logs)));
            void Close(){foreach(var view in views.ToArray())view.Close(ModUiCloseReason.Scene);}
            Bind(save);Scene("fight");
            foreach(var outcome in new[]{"loss","surrender","raid_timeout"})Result(outcome,"fixture.notify:fights/final1");
            Result("win","fixture.notify:fights/not_final");
            Check(!Flag("dialogue_pending",1)&&views.Count==0,"Non-final/non-win queued victory dialogue");
            for(int act=1;act<=6;act++){Result("win","fixture.notify:fights/final"+act);Result("win","fixture.notify:fights/final"+act);}
            Check(views.Count==0,"Victory opened before map");Scene("map");
            int cards=0;
            for(int act=1;act<=6;act++) {
                var quest=source.SelectSingleNode("//Quest[@Name='Sensei_zone"+act+"_guards_beaten']");
                Check(quest.SelectSingleNode("Conditions/Equal[@Value1='_$FightResult']").Attributes["Value2"].Value=="Win","Source trigger changed");
                foreach(XmlNode dialog in quest.SelectNodes("Actions/Dialog")) {
                    var current=Live();Check(current.Id=="sensei_victory","Notification overtook victory sequence");
                    Check(current.Read("speaker").Text==english.SelectSingleNode("//Word[@Title='"+dialog.Attributes["Title"].Value+"']").InnerText,"Speaker differs");
                    Check(current.Read("body").Text==english.SelectSingleNode("//Word[@Title='"+dialog["Line"].GetAttribute("Text")+"']").InnerText,"Card order/text differs");
                    Check(current.Read("portrait").Sprite==AssetId.Parse("core:ui/users/"+dialog.Attributes["Image"].Value),"Portrait identity differs");
                    Check(current.Read("portrait").Mirrored==(dialog.Attributes["Mirrored"]?.Value=="1"),"Portrait mirroring differs");
                    Check(current.Read("continue").Text=="OK"&&!current.Request.IgnoreBack&&current.Request.Lines.Count==1,"Native dialog request differs from the archived Regular dialog");
                    Check(!Flag("complete",act),"Act completed before final card");
                    if(cards==1) {
                        layers.SetBlocked(true);Check(!current.TryClick("continue")&&!layers.Back(),"Blocked input advanced card");layers.SetBlocked(false);
                        string pending=save.OuterXml, previousText=current.Read("body").Text;Close();Scene("shop");
                        profileWins=0;bus.UnbindProfile();state.Unbind();Bind(Save());Scene("map");Check(views.All(value=>value.IsClosed),"New profile inherited dialogue");profileWins=1;
                        bus.UnbindProfile();state.Unbind();save=Save(pending);Bind(save);Scene("map");
                        Check(Live().Read("body").Text==previousText,"Saved cursor did not resume same card");current=Live();
                    }
                    Check(layers.Back()&&current.IsClosed,"Card acknowledgement failed");cards++;
                }
                if(act<6)Check(Flag("complete",act)&&!Flag("dialogue_pending",act),"Act completion flag missing");
            }
            Check(cards==23&&!Flag("complete",6)&&logs.Count(value=>value=="OUTRO_REFUSED")==1,"Refused outro prematurely completed act");
            Scene("map");Check(!Flag("complete",6)&&logs.Count(value=>value=="OUTRO")==1,"Deferred outro failed to retry");
            Scene("dojo");Check(!Flag("complete",6),"Cancelled outro callback completed act");
            Scene("map");Scene("profile");Check(!Flag("complete",6),"Stale outro callback completed act");
            Scene("map");outro(true);outro(true);
            Check(Flag("complete",6)&&!Flag("dialogue_pending",6),"Outro completion failed");
            for(int act=1;act<=6;act++){Check(Live().Id=="sensei_notification","Deferred notification missing");Check(layers.Back(),"Notification acknowledgement failed");}
            Check(views.All(value=>value.IsClosed),"Completed sequences left a view");
            int count=views.Count;for(int act=1;act<=6;act++)Result("win","fixture.notify:fights/final"+act);
            Check(views.Count==count,"Completed sequence replayed");
            string completed=save.OuterXml;bus.UnbindProfile();state.Unbind();Bind(Save(completed));Scene("map");
            for(int act=1;act<=6;act++)Check(Flag("complete",act)&&Flag("opened",act),"Serialized completion lost");
            Check(errors.Count==0&&!logs.Any(value=>value.Contains("failed")),string.Join("\n",errors.Concat(logs)));
        }
        ModProfileAccess.Clear();ModBattleAccess.Clear();ModActScreenAccess.Clear();
        Console.WriteLine("PASS: "+checks+" Sensei victory checks; 23 actual Lua cards, 448 translations, mirrored portraits, queued notifications, interruption/save/profile isolation and actual act-screen Lua binding with controlled host. No native story/art or timed-screen acceptance claim.");
    }
}
