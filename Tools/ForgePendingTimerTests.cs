// Test services surround production recipe, save/clear, delivery and settlement methods.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Eclipse.Modding;
using CodeStage.AntiCheat.ObscuredTypes;
namespace CodeStage.AntiCheat.ObscuredTypes { public struct ObscuredLong { public static implicit operator ObscuredLong(long v) => new ObscuredLong(); } }
namespace Nekki.Utils { }
public class ItemInfo { public string Name, Type; public int ItemLevel=10; public ObscuredLong KLHOKKPALOK; }
public class RecipePrice { public int DeliveryTime=300; public ObscuredLong BonusDeliveryPrice; }
public class Recipe { public string Name="Simple"; public RecipePrice GetPriceByItemLevel(UserItem item,int level)=>new RecipePrice(); }
public static class GlobalTimer { public static long get_GetTime()=>1000; }
public static class XmlHelpers {
 public static string CIPOICEEIBK(this XmlAttribute a,string fallback)=>a?.Value??fallback;
 public static uint ParseUint(this XmlAttribute a)=>a==null?0:uint.Parse(a.Value);
 public static long ParseLong(this XmlAttribute a,long fallback)=>a==null?fallback:long.Parse(a.Value);
 public static XmlNode ACBPMPMPKJJ(this XmlNode n,string name){var e=n.OwnerDocument.CreateElement(name);n.AppendChild(e);return e;}
 public static XmlAttribute LLIKNHNLGJJ(this XmlNode n,string name){var a=n.OwnerDocument.CreateAttribute(name);n.Attributes.Append(a);return a;}
}
public class UserItem {
 public readonly XmlDocument Save=new XmlDocument(); private XmlNode _Node;
 private bool JGPEOEDJMHH=true; private RecipeItemInfo LIONCGGHKLO;
 public UserItem(){Save.LoadXml("<Item/>");_Node=Save.DocumentElement;}
 public string get_Name()=>"weapon";
 public ItemInfo DBLCMCEGJGI(bool v)=>new ItemInfo(); public ItemInfo BHKHOJPANHE()=>new ItemInfo();
 public void Reload(){LIONCGGHKLO=_Node["RecipeDelivery"]==null?null:new RecipeItemInfo(_Node["RecipeDelivery"],this);}
 // INSERT_USERITEM
}
public class UserItems {
 public UserItem Item=new UserItem(); private List<RecipeItemInfo> LFADKPKKFMP=new List<RecipeItemInfo>();
 public List<RecipeItemInfo> PHKEAPFEOLP()=>Item.PHDBCIHJKON()==null?new List<RecipeItemInfo>():new List<RecipeItemInfo>{Item.PHDBCIHJKON()};
 // INSERT_USERITEMS
}
public class Roster {
 public UserItems Items=new UserItems(); public int Saves,Events;
 public int PINDEKDNCNL()=>10; public UserItems KHCNHPCPFII()=>Items;
 public void GGGEHAGCLGC(bool force){Saves++;} public void CallEvent(int kind,object value){Events++;}
}
public class ListSF {
 public static Roster Current=new Roster(); private Roster ANEHEDFAPCH=>Current;
 public static long Clock=1000;
 public static Roster CCDKHLAMKKO()=>Current; public static long IDMJOMOMDOJ()=>Clock;
 public static bool ApplyRecipeToItem(RecipeItemInfo recipe)=>Current.Items.FinishDeliveryRecipe(recipe);
 public void Tick(long time){Clock=time;GHDNJMDEALP(time);}
 // INSERT_LISTSF
}
public static class Debug { public static void LogError(object value){} }
public class ForgeManager {
 static ForgeManager instance=new ForgeManager(); public static ForgeManager ELEBLBJKDBI()=>instance;
 public bool Fail; public int Grants; public Recipe GetRecipeByName(string name)=>new Recipe{Name=name};
 public bool EnchantItem(RecipeItemInfo item){if(Fail)return false;Grants++;return true;}
 // INSERT_FORGE
}
static class Program {
 static int checks;
 static void Check(bool ok,string message){if(!ok)throw new Exception(message);checks++;}
 static void Policy(ModDescriptor mod,bool complete,bool skip=false){var c=new ModContentCatalog();using(var t=c.BeginRegistration(mod)){t.SetTimer("forge",0,skip,complete);t.Commit();}ModPolicies.Content=c;}
 static void Main(string[] args){
  var mod=ModDiscovery.DiscoverLoose(args[0]).Mods.Single(m=>m.Id.Value=="de128");
  var host=new ListSF();var roster=ListSF.Current;var item=roster.Items.Item;var forge=ForgeManager.ELEBLBJKDBI();
  ModPolicies.Content=null;
  var pending=new RecipeItemInfo(new Recipe(),item,new RecipePrice());
  Check(pending.RecipeDeliveryTime==1300,"base new deadline");
  Check(item.SetRecipeDelivery(pending),"persist base order");
  string saved=item.Save.OuterXml;item.Reload();pending=item.PHDBCIHJKON();
  Check(pending.RecipeDeliveryTime==1300 && pending.TimeLeft==300,"load saved deadline");
  Policy(mod,false);Check(pending.IsStillInOrder,"legacy policy preserves wait");
  Check(!roster.Items.FinishDeliveryRecipe(pending),"no early settlement with skipping disabled");
  Policy(mod,true);Check(pending.TimeLeft==0 && !pending.IsStillInOrder,"pending timer removed");
  Check(pending.HGDELDFDFNH()==0,"UI deadline removed");
  Check(pending.IsReadyForDelivery(1000),"scheduler eligibility");
  Check(pending.RecipeDeliveryTime==1300 && item.Save.OuterXml==saved,"policy never rewrites saved deadline");
  ModPolicies.Content=null;Check(pending.TimeLeft==300 && !pending.IsReadyForDelivery(1000),"unload before settlement restores deadline");
  Policy(mod,true);forge.Fail=true;host.Tick(1000);
  Check(item.PHDBCIHJKON()==pending && item.Save.OuterXml==saved,"failed settlement preserves pending save");
  Check(roster.Saves==0 && roster.Events==0 && forge.Grants==0,"failure has no success effects");
  forge.Fail=false;host.Tick(1000);
  Check(forge.Grants==1 && item.PHDBCIHJKON()==null,"normal automatic settlement grants once");
  Check(item.Save.DocumentElement["RecipeDelivery"]==null,"completed order cleared from save");
  Check(roster.Saves==1 && roster.Events==1,"completion saves and notifies once");
  host.Tick(1000);Check(!roster.Items.FinishDeliveryRecipe(pending) && forge.Grants==1,"stale receipt and repeated timer do not grant again");
  item.Reload();ModPolicies.Content=null;host.Tick(1400);Check(forge.Grants==1,"reload/unload after completion stays complete");
  ListSF.Clock=1000;var basePending=new RecipeItemInfo(new Recipe(),item,new RecipePrice());item.SetRecipeDelivery(basePending);
  Policy(mod,false);Check(!basePending.IsReadyForDelivery(1299)&&basePending.IsReadyForDelivery(1300),"natural expiration preserved");
  host.Tick(1300);Check(forge.Grants==2,"naturally expired order completes with skipping disabled");
  Policy(mod,true);var instant=new RecipeItemInfo(new Recipe(),item,new RecipePrice());
  Check(instant.RecipeDeliveryTime==0 && !instant.IsReadyForDelivery(1000),"new instant order stays on immediate creation path");
  Console.WriteLine($"PASS {checks} pending-forge lifecycle checks; production methods, controlled clock/enchantment/save services.");
 }
}
