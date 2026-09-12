using System;
using System.Collections.Generic;
using Eclipse.Modding;
using ObscuredInt = System.Int32;
using ObscuredUInt = System.UInt32;

public class Roster {
 public class Parameters {
  public int Current=1;
  public int PINDEKDNCNL()=>Current;
  public void DLDMOHEGENM(int value){Current=value;}
 }
 public class Inventory {
  public int Updated;
  public void NHJAHNDOLAE(){Updated++;}
  public void UpdateLockItems(int level){Updated++;}
 }
 public Parameters HEGIABHIPHA=new Parameters();
 public List<uint> _levelThresholds=new List<uint>{100,200,300,400};
 public uint _experience;
 public Inventory Items=new Inventory();
 public Dictionary<string,object> Saved=new Dictionary<string,object>();
 public Action OnSave;
 public int Level=>PINDEKDNCNL();
 public int PINDEKDNCNL()=>HEGIABHIPHA?.Current??0;
 public Inventory KHCNHPCPFII()=>Items;
 public void EMDLLIGKONG(string key,object value){Saved[key]=value;OnSave?.Invoke();}
 /* ROSTER METHODS */
}
public static class GameUtils { public const uint JNOGEPFLLDM=100000;public static void PIHNKCIDDJB(){} public static void OFOKPNFGDMD(string value){} }
public class ListSF { public static ListSF ELEBLBJKDBI()=>new ListSF();public void PLNBHLPHDJG(int level){} }
public static class StatisticsEvent {public enum JDNFFHILFAF{Level_Up}}
public static class StatisticsCollector {public static void BPDGOKGHDHB(StatisticsEvent.JDNFFHILFAF value){} }
namespace Eclipse.Modding {
 public static class ModRuntime {
  public static Roster _profileRoster;
  public static ModStoryEvents StoryEvents=new ModStoryEvents();
  /* HOST METHOD */
 }
}
static class Program {
 static int checks;
 static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
 static void Main(){
  var events=new List<ModStoryEvent>();var bus=ModRuntime.StoryEvents;
  var scope=bus.CreateScope(ModId.Parse("example.level"));
  var active=new Roster();ModRuntime._profileRoster=active;bus.BindProfile();
  scope.Subscribe(ModStoryEventKind.LevelUp,e=>{
   Check(active.Saved.ContainsKey("Experience"),"event before experience save update");
   Check(e.Level==active.Level,"callback sees intermediate level");events.Add(e);
  });
  Check(!active.DBPBGBNHAIP(99)&&events.Count==0&&active.Level==1,"below threshold changed result");
  Check(active.DBPBGBNHAIP(400),"multi-level native flag lost");
  Check(events.Count==1&&events[0].PreviousLevel==1&&events[0].Level==3&&active.Items.Updated==2,"multi-level notification or inventory timing");
  Check(!active.DBPBGBNHAIP(100)&&events.Count==1,"unchanged experience emitted event");
  var comparison=new Roster();comparison.DBPBGBNHAIP(100);
  Check(events.Count==1,"comparison roster emitted active-profile event");
  active=new Roster();ModRuntime._profileRoster=active;
  Check(!active.DBPBGBNHAIP(1000)&&active.Level==4,"native capped result changed");
  Check(events.Count==2&&events[1].PreviousLevel==1&&events[1].Level==4,"capped multi-level gain missed because native flag false");
  active.DBPBGBNHAIP(1000);Check(events.Count==2,"maximum level duplicate");
  active=new Roster();ModRuntime._profileRoster=active;bus.UnbindProfile();
  active.DBPBGBNHAIP(100);Check(events.Count==2,"unbound profile delivery");
  bus.BindProfile();active=new Roster();ModRuntime._profileRoster=active;
  active.OnSave=()=>bus.BindProfile();active.DBPBGBNHAIP(100);
  Check(events.Count==2,"profile changed during update received stale event");
  active=new Roster{HEGIABHIPHA=null};ModRuntime._profileRoster=active;
  Check(!active.DBPBGBNHAIP(100)&&events.Count==2,"missing parameters changed");
  scope.Dispose();active=new Roster();ModRuntime._profileRoster=active;
  active.DBPBGBNHAIP(100);Check(events.Count==2,"disposed subscriber delivery");
  Console.WriteLine("PASS: "+checks+" production experience/level notification checks with controlled inventory/save services.");
 }
}
