using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

sealed class PortraitMetadata : IAssetProvider
{
    public ModId Namespace => ModId.Parse("core");
    public bool TryDescribe(AssetId id, out AssetMetadata metadata)
    {
        metadata = null;
        if(id.Namespace != Namespace || id.Path != "ui/users/character_sensei")return false;
        metadata = new AssetMetadata(id,AssetKind.Sprite,AssetSourceKind.Core,"",-1,"controlled portrait metadata");
        return true;
    }
}
static class Program
{
    static int checks;
    static void Check(bool value,string message) { checks++; if(!value)throw new Exception(message); }
    static XmlDocument Save(string xml=null) { var doc=new XmlDocument();doc.LoadXml(xml??"<Warrior><EclipseMods schema='1'><Mod id='fixture.notify'/></EclipseMods></Warrior>");return doc; }
    static void Main(string[] args)
    {
        string package=Path.Combine(args[0],"fixture.notify");
        File.WriteAllText(Path.Combine(package,"mod.toml"),"""
schema = 1
id = "fixture.notify"
name = "Sensei notification checks"
version = "1.0.0"
authors = ["Eclipse tests"]
entrypoint = "scripts/main.lua"
capabilities = ["content.register", "story.events", "story.progression", "profile.read", "state.read", "state.write", "ui.create"]
[[dependencies]]
id = "core"
version = ">=1.0 <2.0"
""");
        // Final IDs are controlled core handles here: encounter assembly is deliberately pending.
        File.WriteAllText(Path.Combine(package,"scripts/main.lua"),"""
local sf2=require("sf2")
local zone=sf2.zones.register{id="test"}
local battles,finals={},{}
for act=1,6 do
    battles[act]=sf2.battles.register{id="act"..act,zone=zone,type=sf2.battles.STORY}
    if act<6 then finals[act]="core:fights/zone_"..act.."/tournament/1" end
end
require("content.sensei_notifications").install(battles,finals)
""");
        var mod=ModDiscovery.DiscoverLoose(args[0]).Mods.Single();
        var catalog=new ModContentCatalog();
        var stages=new XmlDocument();stages.Load(Path.Combine(args[1],"Assets/vanillaXml/stages.xml"));
        CoreContentImporter.ImportStages(catalog,stages.SelectSingleNode("Stages/Zones"));
        var assets=new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod),new PortraitMetadata()});
        var errors=new List<string>();var views=new List<ModUiSurface>();
        var bus=new ModStoryEvents((id,message)=>errors.Add(message));
        var state=new ModStateRuntime();
        var calls=new List<string>();
        bool modeReady=false, mapReady=false;
        int wins=1;
        ModProfileAccess.Fight=id=>new ModProfileFightSnapshot(true,wins,0);
        ModProfileAccess.SetEclipseMode=enabled=>{Check(!enabled,"Notification enabled Eclipse mode");calls.Add("mode");return modeReady;};
        ModBattleAccess.Reveal=(id,locked)=>{calls.Add("reveal:"+id.LocalId+":"+locked);return mapReady;};
        ModBattleAccess.SetLocked=(id,locked)=>{calls.Add("lock:"+id.LocalId+":"+locked);return mapReady;};
        ModBattleAccess.Focus=id=>{calls.Add("focus:"+id.LocalId);return mapReady;};
        using(var layers=new ModUiLayerStack())
        using(var tx=catalog.BeginRegistration(mod))
        using(var context=new MoonSharpScriptRuntime(view=>{views.Add(view);layers.Add(view);},null,null,bus)
            .CreateContext(mod,new ModApiFacade(mod,assets,tx,state,entry=>errors.Add(entry.Message))))
        {
            context.ExecuteEntrypoint();tx.Commit();
            int translations=0;
            foreach(var pair in new Dictionary<string,string>{{"title","characterSensei"},{"intro","Sensei_remembers0"},{"more","Sensei_remembers1"},{"ending","Sensei_remembers2"}})
            {
                var definition=catalog.Localizations.Single(value=>value.Id.LocalId=="sensei.notify."+pair.Key);
                foreach(var path in Directory.GetFiles(Path.Combine(args[1],"Assets/DExml/localizations"),"*.xml"))
                {
                    var archive=new XmlDocument();archive.Load(path);
                    var word=archive.SelectSingleNode("//Word[@Title='"+pair.Value+"']");
                    if(word==null)continue;
                    Check(definition.TryGet(Path.GetFileNameWithoutExtension(path),out var actual)&&actual==word.InnerText,"Historical translation differs: "+path+" "+pair.Value);
                    translations++;
                }
            }
            Check(translations==56,"Translation source coverage changed");
            var save=Save();
            void Bind(XmlDocument doc) { Check(state.Bind(doc.DocumentElement,new[]{context}).Count==0,"State bind diagnostics");bus.BindProfile(); }
            bool Flag(string kind,int act) => state.TryGetValue(mod.Id,"sensei_"+kind+"_"+act,out var value) && value.Boolean;
            void Scene(string scene) => bus.Publish(new ModStoryEvent(ModStoryEventKind.SceneEnter,null,scene:scene));
            void Result(string outcome) => bus.Publish(new ModStoryEvent(ModStoryEventKind.BattleResult,null,battle:new ModBattleResultSnapshot(null,outcome,true)));
            ModUiSurface Live() => views.Last(view=>!view.IsClosed);
            void CloseForScene() { foreach(var view in views.ToArray())view.Close(ModUiCloseReason.Scene); }
            Bind(save);Scene("fight");
            Result("loss");Result("surrender");
            Check(!Flag("pending",1) && views.Count==0,"Loss queued notification");
            wins=0;Result("win");Check(!Flag("pending",1),"Unmet prerequisites queued notification");
            wins=1;Result("win");
            for(int act=1;act<=6;act++)Check(Flag("pending",act)&&!Flag("opened",act),"Victory failed to queue act "+act);
            Check(views.Count==0,"Fight scene displayed map dialog");
            string queued=save.OuterXml;
            Scene("map");var first=Live();
            Check(first.Read("body").Text.Contains("many years ago"),"Wrong first notification text");
            Check(first.Read("portrait").Sprite==AssetId.Parse("core:ui/users/character_sensei"),"Wrong portrait identity");
            layers.SetBlocked(true);Check(!layers.Back() && !first.TryClick("continue") && calls.Count==0,"Blocked input progressed");layers.SetBlocked(false);
            Check(layers.Back() && !first.IsClosed && calls.SequenceEqual(new[]{"mode"}) && !Flag("opened",1),"Refused mode lost pending notification");
            modeReady=true;calls.Clear();first.TryClick("continue");
            Check(!first.IsClosed && calls.Count==2 && calls[0]=="mode" && calls[1].StartsWith("reveal:") && !Flag("opened",1),"Refused map action advanced notification");
            calls.Clear();CloseForScene();Scene("shop");
            Check(calls.Count==0 && Flag("pending",1)&&!Flag("opened",1),"Scene cleanup acknowledged notification");
            bus.UnbindProfile();state.Unbind();Bind(Save());Scene("map");
            Check(views.All(view=>view.IsClosed)&&!Flag("pending",1),"New profile inherited pending notification");
            bus.UnbindProfile();state.Unbind();var restored=Save(queued);Bind(restored);Scene("map");
            Check(!Live().IsClosed && Flag("pending",1),"Serialized pending notification did not resume");
            mapReady=true;
            for(int act=1;act<=6;act++)
            {
                var current=Live();
                string expected=act==1?"many years ago":act==6?"at the end":"something more";
                Check(current.Read("body").Text.Contains(expected),"Wrong act text/order "+act);
                calls.Clear();Check(layers.Back()&&current.IsClosed,"Back failed to acknowledge act "+act);
                Check(Flag("opened",act)&&!Flag("pending",act),"Acknowledgment flags wrong "+act);
                Check(calls.Count==(act==1?9:2) && calls.Last()=="focus:act"+act,"Acknowledgment actions/order wrong "+act+": "+string.Join(",",calls));
            }
            Check(views.All(view=>view.IsClosed),"Final acknowledgment left dialog open");
            int count=views.Count;Result("win");Check(views.Count==count,"Opened acts notified twice");
            string completed=restored.OuterXml;bus.UnbindProfile();state.Unbind();Bind(Save(completed));Scene("map");
            for(int act=1;act<=6;act++)Check(Flag("opened",act)&&!Flag("pending",act),"Completed flags lost on roundtrip");
            Check(errors.Count==0,string.Join("\n",errors));
        }
        ModProfileAccess.Clear();ModBattleAccess.Clear();
        Console.WriteLine("PASS: "+checks+" actual Lua Sensei notification input, refusal/retry, ordering, profile separation and serialized state checks; controlled host services, no native rendering claim.");
    }
}
