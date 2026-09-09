// Unrelated engine/UI dependencies for the production ModModeRuntime fixture.
using System;
using System.Collections.Generic;
using System.Xml;
public sealed class FightIDS { private readonly string _id; public FightIDS(string id) { _id=id; } public override string ToString()=>_id; }
public sealed class FightList { public FightIDS BCKFACGMOKC; public Battle CNAOMDMIGLJ; }
public sealed class Battle { public string Name; public string get_Name()=>Name; }
public sealed class Zone { public string Name; public string get_Name()=>Name; }
public sealed class UserItem { public int Count; }
public sealed class UserItems {
    public readonly Dictionary<string,UserItem> Items=new Dictionary<string,UserItem>();
    public UserItem CMGOCLGHNLH(string name)=>Items.TryGetValue(name,out var item)?item:null;
}
public sealed class Roster {
    public readonly UserItems Items=new UserItems(); public int Level=40; public int Saves;
    public int PINDEKDNCNL()=>Level; public UserItems KHCNHPCPFII()=>Items; public void GGGEHAGCLGC(bool force) { Saves++; }
}
public sealed class QuestParameters {}
public class QuestAction {
    public virtual void Parse(XmlNode node) {} public virtual void DEJMHFMLKIC(QuestParameters parameters) {} protected void OGIJONMKABB() {}
}
public sealed class QuestEvent {
    public enum PMDPDMFLCIJ { QUEST_EVENT_RAID_ENTER, QUEST_EVENT_RAID_END, QUEST_EVENT_RESET_ASCENSION, QUEST_EVENT_SHOW_RAID_LOOT }
}
public sealed class QuestManager {
    public readonly List<QuestEvent.PMDPDMFLCIJ> Events=new List<QuestEvent.PMDPDMFLCIJ>();
    public bool FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ kind) { Events.Add(kind); return false; } public void MHHNIPBJNAD() {}
}
public static class ListSF {
    public static Roster Roster=new Roster(); public static QuestManager Quests=new QuestManager();
    public static readonly Dictionary<string,FightList> Fights=new Dictionary<string,FightList>();
    public static Roster CCDKHLAMKKO()=>Roster; public static QuestManager ELEBLBJKDBI()=>Quests;
    public static FightList CHMCKGCDGCM(FightIDS id)=>Fights.TryGetValue(id.ToString(),out var fight)?fight:null;
    public static List<Zone> FHAIJEAPFEA()=>new List<Zone>();
}
public sealed class FightResult {}
public sealed class Fight {
    public static Fight Instance=new Fight(); public int Presented;
    public static Fight OHNKFOHIAKG()=>Instance; public void BCFBHJOLGNL(FightResult result) { Presented++; }
}
public static class LocalizationManager { public static string GetString(string value)=>value; }
namespace Nekki.SF2.GUI {
    public static class Scene<T> where T:new() { public static T get_Current()=>new T(); }
}
namespace Nekki.SF2.GUI.Map {
    public sealed class MapScene { public void SetRaidToggleVisible(bool value) {} public void SwitchToRaidMap() {} public void GotoZoneByName(string value) {} }
}
namespace Eclipse.Underworld {
    public static class UnderworldZonePolicy { public static bool IsRaidZone(Zone zone)=>Eclipse.Modding.ModPolicies.IsRaidZone(zone.Name); }
}
