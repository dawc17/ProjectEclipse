using System;
using System.Xml;
using System.Collections.Generic;
using Eclipse.Modding;
public class ItemInfo {public string Name="TEST_ITEM",MDPPNGIEJGD="";public ItemInfo ParentItem;public int OBJDGBBFJOO;public XmlNode NodeXML;}
public class UserItem {
 public ItemInfo Definition=new ItemInfo();public int PendingUpgrade,Upgrade;
 public long IJGAOHJNLAH()=>Delivery;public bool DBKKJGBJOEO()=>false;public ItemInfo BHKHOJPANHE()=>Definition;public int EIMMBNNMBCN()=>PendingUpgrade;public int DHNNCAEEMLL()=>Upgrade;
 public string Name;public int Count;public bool Equipped;public long Delivery;
 public UserItem(XmlNode n,string name,bool auto,int count,int upgrade,long delivery){Name=name;Count=count;Delivery=delivery;}
 public int OFOPFCJNEBL()=>Count;public void CHILOKHFALD(int value){Count=value;}
 public void KIGHKCOCJFJ(ItemInfo i){}public void CDFODJBJIPI(int level){}public void IJCEKDCPBAG(bool value){}
 public void PJEEGECBHMH(){}public void FMMDLMGHPIB(int upgrade){Upgrade=upgrade;}public void BAMLNLIDEBG(int upgrade){}
 public void set_DeliveryTime(long value){Delivery=value;}
}
public class Inventory {
 public Action<UserItem> HHGJMMHMEMP=i=>{};public List<UserItem> MPACCEAFDOH()=>new List<UserItem>();public List<UserItem> EHDCCPKOANN()=>new List<UserItem>();
 /* DELIVERY */

 public Dictionary<string,UserItem> Items=new Dictionary<string,UserItem>();public bool Fail;
 public UserItem GEFDJDIINND(UserItem item){if(Fail)throw new Exception("native failure");Items[item.Name]=item;return item;}
}
public class Roster {
 public static Action OnSave;public void GGGEHAGCLGC(){OnSave?.Invoke();}
 public bool ADKHNLAMDJP;public Inventory Inventory=new Inventory();public int PINDEKDNCNL()=>4;
 public Inventory KHCNHPCPFII()=>Inventory;public XmlNode BABKABBEFEL()=>new XmlDocument().CreateElement("Inventory");
}
public static class MenuController {public static void ADPMENDMMKJ(){}}
public class QuestParameters {public ItemInfo DLKPBAJDHBO;}
public class QuestEvent {public enum PMDPDMFLCIJ{QUEST_EVENT_DELIVERY}}
public class Quests {public Action OnDelivery;public bool Fail;public QuestParameters BNMLDPNCMLB()=>new QuestParameters();public bool FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ kind){if(Fail)throw new Exception("quest failure");OnDelivery?.Invoke();return false;}public void MHHNIPBJNAD(){}}
public static class ListSF {
 public static Quests Quests=new Quests();public static Quests ELEBLBJKDBI()=>Quests;

 public static Roster Active;public static Action OnEquip;
 public static Roster CCDKHLAMKKO()=>Active;
 public static UserItem CMGOCLGHNLH(string name)=>Active.Inventory.Items.TryGetValue(name,out var item)?item:null;
 public static void AFGHCIDFAHB(UserItem item,bool force){item.Equipped=true;OnEquip?.Invoke();}
 /* ACQUIRE */
}
namespace Eclipse.Modding {
 public static class ModRuntime {
  public class Scripts {public ModContentCatalog Content;}
  public static Roster _profileRoster;public static Scripts _scripts;
  public static ModStoryEvents StoryEvents=new ModStoryEvents();
  /* PUBLISH */
 }
}
public static class Program {
 static int checks;static void Check(bool ok,string text){checks++;if(!ok)throw new Exception(text);}
 public static void Main(){
  var catalog=new ModContentCatalog();var xml=new XmlDocument();xml.LoadXml("<Item Type='Weapon' Name='TEST_ITEM' WeaponDamage='1'/>");CoreContentImporter.ImportWeapons(catalog,new[]{xml.DocumentElement},null);
  ModRuntime._scripts=new ModRuntime.Scripts{Content=catalog};ListSF.Active=new Roster();ModRuntime._profileRoster=ListSF.Active;ModRuntime.StoryEvents.BindProfile();
  bool nestedGrant=false;var seen=new List<ModStoryEvent>();using(var scope=ModRuntime.StoryEvents.CreateScope(ModId.Parse("example.acquisition"))){
   scope.Subscribe(ModStoryEventKind.ItemAcquired,e=>{if(!nestedGrant)Check(ListSF.CMGOCLGHNLH("TEST_ITEM").Count==e.Count,"Published before inventory update");seen.Add(e);});
   var item=new ItemInfo{NodeXML=xml.DocumentElement};var acquired=ListSF.GEFDJDIINND(item,2);
   Check(seen.Count==1&&seen[0].PreviousCount==0&&seen[0].Count==2&&seen[0].Item==CoreContentImporter.WeaponId("TEST_ITEM")&&acquired.Equipped,"First acquisition payload/timing");
   ListSF.GEFDJDIINND(item,3);Check(seen.Count==2&&seen[1].PreviousCount==2&&seen[1].Count==5,"Stack acquisition payload");
   ListSF.GEFDJDIINND(item,0);Check(seen.Count==2,"Zero delta emitted acquisition");
   ListSF.GEFDJDIINND(item,-1);Check(seen.Count==2,"Removal emitted acquisition");
   var child=new ItemInfo{ParentItem=item};ListSF.GEFDJDIINND(child);Check(seen.Count==2,"Parent-linked non-increment emitted acquisition");
   ListSF.Active.Inventory.Items.Clear();ListSF.GEFDJDIINND(item,1,100);Check(seen.Count==2,"Pending delivery emitted acquisition");
   ListSF.Active.Inventory.Items.Clear();ListSF.Active.Inventory.Fail=true;bool failed=false;try{ListSF.GEFDJDIINND(item);}catch(Exception){failed=true;}
   Check(failed&&seen.Count==2,"Failed native grant emitted acquisition");ListSF.Active.Inventory.Fail=false;
   ListSF.OnEquip=()=>ModRuntime.StoryEvents.BindProfile();ListSF.GEFDJDIINND(item);Check(seen.Count==2,"Stale generation published acquisition");ListSF.OnEquip=null;
   ModRuntime._profileRoster=new Roster();ListSF.GEFDJDIINND(item);Check(seen.Count==2,"Non-active roster published acquisition");
   ModRuntime._profileRoster=ListSF.Active;
   var delivery=new UserItem(null,"TEST_ITEM",false,0,-1,100){Definition=item};ListSF.Active.Inventory.Items[item.Name]=delivery;
   ListSF.Active.Inventory.GBLHFNGPIOF(delivery);
   Check(seen.Count==3&&seen[2].PreviousCount==0&&seen[2].Count==1&&delivery.Delivery==-1,"Delivery did not publish completed count increase");
   ListSF.Active.Inventory.GBLHFNGPIOF(delivery);Check(seen.Count==3,"Repeated delivery duplicated acquisition");
   delivery.Delivery=100;delivery.PendingUpgrade=2;ListSF.Active.Inventory.GBLHFNGPIOF(delivery);
   Check(seen.Count==3&&delivery.Upgrade==2,"Upgrade-only delivery emitted acquisition");
   delivery.Count=0;delivery.Delivery=100;Roster.OnSave=()=>ModRuntime.StoryEvents.BindProfile();ListSF.Active.Inventory.GBLHFNGPIOF(delivery);Roster.OnSave=null;
   Check(seen.Count==3,"Delivery crossed profile generation");
   delivery.Count=0;delivery.Delivery=100;ListSF.Quests.Fail=true;bool deliveryFailed=false;try{ListSF.Active.Inventory.GBLHFNGPIOF(delivery);}catch(Exception){deliveryFailed=true;}ListSF.Quests.Fail=false;
   Check(deliveryFailed&&seen.Count==3&&delivery.Count==0,"Failed delivery emitted acquisition");
   delivery.Count=0;delivery.Delivery=100;ListSF.Quests.OnDelivery=()=>ListSF.GEFDJDIINND(item,1,0,false);ListSF.Active.Inventory.GBLHFNGPIOF(delivery);ListSF.Quests.OnDelivery=null;
   Check(seen.Count==4,"Delivery duplicated a nested quest acquisition");
   delivery.Count=0;delivery.Delivery=100;
   var foreign=new Inventory();foreign.GBLHFNGPIOF(delivery);Check(seen.Count==4,"Foreign inventory delivery published acquisition");
   seen.Clear();ListSF.Active.Inventory.Items.Clear();nestedGrant=true;
   ListSF.OnEquip=()=>ListSF.GEFDJDIINND(item,2,0,false);
   ListSF.GEFDJDIINND(item,1);ListSF.OnEquip=null;
   int delta=0;foreach(var e in seen)delta+=e.Count.Value-e.PreviousCount.Value;
   Check(seen.Count==2&&delta==3&&ListSF.CMGOCLGHNLH(item.Name).Count==3,"Nested equip grant double-counted acquisition");
   Check(seen[0].PreviousCount==1&&seen[0].Count==3&&seen[1].PreviousCount==0&&seen[1].Count==1,"Nested return-order snapshots incorrect");
  }
  Console.WriteLine("PASS: "+checks+" native acquisition/publication checks with controlled inventory services; Lua delivery tested separately.");
 }
}
