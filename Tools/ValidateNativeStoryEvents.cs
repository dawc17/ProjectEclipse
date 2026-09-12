using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Eclipse.Modding;

public static class QuestEvent { public enum PMDPDMFLCIJ { QUEST_EVENT_PURCHASE, QUEST_EVENT_ENCHANTMENT, Other } }
public class ItemInfo { public string Name; public XmlNode NodeXML; }
public class QuestParameters {
    public ItemInfo DLKPBAJDHBO;
    public Enchant DPLEGFCHOCE = new Enchant();
    public class Enchant { public string OHCGEEEKEJH; public string FHELNNCGCGC; }
}
namespace Eclipse.Modding {
    public static class ModRuntime {
        public static object _profileRoster;
        public class Scripts { public ModContentCatalog Content; }
        public static Scripts _scripts;
        public static readonly ModStoryEvents StoryEvents = new ModStoryEvents();
        public static class Debug { public static void LogWarning(string message) { throw new Exception(message); } }
        /* HOST METHODS */
    }
}
public class NativeList {
    public class Manager {
        public Func<QuestEvent.PMDPDMFLCIJ,bool> Evaluate;
        public bool ActionQuest(QuestEvent.PMDPDMFLCIJ kind) => Evaluate(kind);
    }
    public Manager _QuestsManager = new Manager();
    public QuestParameters Parameters = new QuestParameters();
    public QuestParameters BNMLDPNCMLB() => Parameters;
    /* NATIVE DISPATCH */
}
static class Program {
    static int checks;
    static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    static void Main(string[] args){
        var document=new XmlDocument();document.Load(Path.Combine(args[0],"Assets/vanillaXml/list.xml"));
        var node=document.SelectSingleNode("/List/Items/Item[@Name='WEAPON_NUNCHAKU']");
        var catalog=new ModContentCatalog();CoreContentImporter.ImportWeapons(catalog,new[]{node},null);
        CoreContentImporter.ImportForgeEconomicProfiles(catalog,new[]{"Simple"});
        ModRuntime._scripts=new ModRuntime.Scripts{Content=catalog};
        ModRuntime._profileRoster=new object();
        var bus=ModRuntime.StoryEvents;
        var scope=bus.CreateScope(ModId.Parse("example.story"));
        var events=new List<ModStoryEvent>();
        bool nativeFinished=false;
        scope.Subscribe(ModStoryEventKind.Purchase,e=>{Check(nativeFinished,"notification preceded native evaluation");events.Add(e);});
        scope.Subscribe(ModStoryEventKind.Enchantment,e=>events.Add(e));
        var list=new NativeList();
        list.Parameters.DLKPBAJDHBO=new ItemInfo{Name="WEAPON_NUNCHAKU",NodeXML=node};
        list._QuestsManager.Evaluate=kind=>{nativeFinished=true;return true;};
        Check(list.FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_PURCHASE)&&events.Count==0,"unbound native operation changed");
        bus.BindProfile();nativeFinished=false;
        list._QuestsManager.Evaluate=kind=>{
            list.Parameters.DLKPBAJDHBO.Name="MUTATED";nativeFinished=true;return false;
        };
        Check(!list.FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_PURCHASE),"native false changed");
        Check(events.Count==1&&events[0].Item==CoreContentImporter.WeaponId("WEAPON_NUNCHAKU"),"snapshot lost original identity");
        list._QuestsManager.Evaluate=kind=>true;
        Check(list.FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.Other)&&events.Count==1,"unrelated event changed");
        list.Parameters.DPLEGFCHOCE.OHCGEEEKEJH="WEAPON_NUNCHAKU";
        list.Parameters.DPLEGFCHOCE.FHELNNCGCGC="Simple";
        list._QuestsManager.Evaluate=kind=>{list.Parameters.DPLEGFCHOCE.FHELNNCGCGC="MUTATED";return true;};
        Check(list.FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ENCHANTMENT),"native true changed");
        Check(events.Count==2&&events[1].Recipe==DefinitionId.Parse("core:forge-profiles/simple")&&events[1].Item==events[0].Item,"enchantment snapshot mapping");
        list.Parameters.DPLEGFCHOCE.OHCGEEEKEJH="unregistered";
        list._QuestsManager.Evaluate=kind=>false;
        list.FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ENCHANTMENT);
        Check(events.Count==3&&events[2].Item==null&&events[2].Recipe==null,"unknown identities fabricated or event dropped");
        list._QuestsManager.Evaluate=kind=>{bus.BindProfile();return true;};
        list.FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ENCHANTMENT);
        Check(events.Count==3,"captured old-profile event reached new profile");
        list._QuestsManager.Evaluate=kind=>{throw new InvalidOperationException("native failure");};
        bool threw=false;try{list.FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ENCHANTMENT);}catch(InvalidOperationException){threw=true;}
        Check(threw&&events.Count==3,"native failure masked or delivered event");
        ModRuntime._profileRoster=null;
        Check(ModRuntime.CaptureStoryEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ENCHANTMENT,list.Parameters)==null,"inactive roster capture");
        ModRuntime._profileRoster=new object();scope.Dispose();
        Check(ModRuntime.CaptureStoryEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ENCHANTMENT,list.Parameters)==null,"no-subscriber capture");
        Check(ModRuntime.CaptureStoryEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_PURCHASE,null)==null,"null params capture");
        Console.WriteLine("PASS: "+checks+" production native dispatch/capture checks with controlled quest processing. No full-game or Lua delivery claimed.");
    }
}
