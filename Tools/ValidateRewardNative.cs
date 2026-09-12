using System;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Eclipse.Modding;

// Controlled engine boundaries: numeric anti-cheat storage, profile mode, math,
// XML scalar conversion and non-item reward implementations. Native reward
// parsing/composition/choice and item constructor code are loaded by the runner.
namespace UnityEngine { public static class Mathf { public static float Pow(float x,float y)=>(float)Math.Pow(x,y); } }
public static class Scalars {
 public static int ParseInt(this XmlNode n,int fallback=0)=>n==null?fallback:int.Parse(n.Value,CultureInfo.InvariantCulture);
 public static uint ParseUint(this XmlNode n)=>n==null?0:uint.Parse(n.Value,CultureInfo.InvariantCulture);
 public static long ParseLong(this XmlNode n,long fallback=0)=>n==null?fallback:long.Parse(n.Value,CultureInfo.InvariantCulture);
 public static float ParseFloat(this XmlNode n,float fallback=0)=>n==null?fallback:float.Parse(n.Value,CultureInfo.InvariantCulture);
 public static bool ParseBool(this XmlNode n)=>n!=null&&n.Value=="1";
 public static string CIPOICEEIBK(this XmlNode n,string fallback)=>n?.Value??fallback;
 public static void GMCADPGOCHM<T>(this T n){}
}
public class ListSF {
 public static bool Eclipse; public static int Level=4;public static Inventory Inventory=new Inventory();public static ItemCatalog Catalog=new ItemCatalog();
 public static ListSF CCDKHLAMKKO()=>new ListSF();public bool JPMPIDFGCJL()=>Eclipse;
 public int PINDEKDNCNL()=>Level;public Inventory KHCNHPCPFII()=>Inventory;public static ItemCatalog DJBOFEEKJMP()=>Catalog;
}
public class UserItem {}
public class Inventory { public HashSet<string> Owned=new HashSet<string>(); public UserItem CMGOCLGHNLH(string name)=>Owned.Contains(name)?new UserItem():null; }
public class ItemCatalog { public Dictionary<string,ItemInfo> Items=new Dictionary<string,ItemInfo>();public ItemInfo KCCDBEEKBCG(string name)=>Items.TryGetValue(name,out var item)?item:null; }
public class UpgradeData { public int Number; }
public class ItemInfo {
 public string Name,Type="Weapon";public int MHGODOLNDLE=4;public int Upgrade;
 public List<UpgradeData> Upgrades=new List<UpgradeData>(); public ItemInfo LevelVariant;
 public ItemInfo GetUpdateItemByLevel(int level,bool flag)=>LevelVariant;
 public List<UpgradeData> DNFDAGFAANJ(bool flag,int level)=>Upgrades;
 public ItemInfo MPADIPJLMLH(UpgradeData data)=>new ItemInfo{Name=Name,Type=Type,MHGODOLNDLE=MHGODOLNDLE,Upgrade=data.Number};
}
public class Result {
 public struct LJFFIBFBGID { public ItemInfo DLKPBAJDHBO;public RewardItem NAIEGGHELIH;public bool IDGKPLBKDIB; }
 public List<LJFFIBFBGID> HELFDCAIJNE=new List<LJFFIBFBGID>();
 /* ITEM SELECTION */
}
public static class GameUtils { public static long GetDenominatedValue(long value,int n)=>value; }
public static class NekkiMath { public static float Position; public static float randomFloat(float a,float b)=>a+(b-a)*Position; }
public class PerkStruct { public PerkStruct(XmlNode n){} }
public class RewardItem:Rewardable {
 public string Name; public uint UpgradeNumber; protected string JNPPCEGFJLE;
 public List<PerkStruct> LDLPCOFHFKE=new List<PerkStruct>();
 public int CMEFKONFDKN()=>0;
 /* ITEM CONSTRUCTOR */
}
public class RewardMoney:Rewardable { public RewardMoney(XmlNode n){} }
public class RewardCurrency:Rewardable { public RewardCurrency(XmlNode n){} }
public class RewardResistance:Rewardable { public RewardResistance(XmlNode n){} }
public class MANJCIGJPMK { public int BDJKDCMHEBI, CIKLDJLOFDJ; public MANJCIGJPMK(XmlNode n,ushort a,ushort b){} }
public class Adapter {
 readonly ModContentCatalog _content; public Adapter(ModContentCatalog content){_content=content;}
 public XmlElement Build(XmlDocument doc,RewardDefinition r)=>BuildRewardNode(doc,r);
 /* BUILDERS */
}
public static class Program {
 static int checks;static void Check(bool ok,string message){checks++;if(!ok)throw new Exception(message);}
 public static void Main(string[] args){
  var catalog=new ModContentCatalog();var itemDoc=new XmlDocument();itemDoc.LoadXml("<Item Type='Weapon' Name='TEST_REWARD' WeaponDamage='1'/>");
  CoreContentImporter.ImportWeapons(catalog,new[]{itemDoc.DocumentElement},new Dictionary<string,XmlDocument>());
  var mod=ModDiscovery.DiscoverLoose(args[0]).Mods.Single(m=>m.Id.Value=="example.core-fight");
  RewardDefinition reward;
  using(var tx=catalog.BeginRegistration(mod)){
   var id=DefinitionId.Parse("core:items/weapon/TEST_REWARD");
   reward=tx.RegisterReward("native",new[]{new RewardItemGrant(id,2)},new[]{new RewardChoiceDefinition(new[]{new RewardChoiceItem(new RewardItemGrant(id,3),1)})});tx.Commit();
  }
  var adapter=new Adapter(catalog);
  const string source="<Fight><Rewards><Reward Money='5'/><Reward Money='77' Bonus='2' Exp='4' PrizeBase='1'><Item Name='shared'/><Level Min='3' Max='9' Money='10'><Item Name='shared-level'/></Level><NormalModeReward Exp='6'><Item Name='normal'/></NormalModeReward><EclipseModeReward Bonus='7'><Item Name='eclipse'/><Level Min='3' Max='9' Exp='8'><Item Name='old'/></Level></EclipseModeReward></Reward></Rewards></Fight>";
  var doc=new XmlDocument();doc.LoadXml(source);string original=doc.OuterXml;
  ModRewardDropProjection.Apply(doc.DocumentElement,1,ModRuleMode.Eclipse,3,9,reward,r=>adapter.Build(doc,r));
  Check(doc.SelectSingleNode("Fight/Rewards/Reward[1]").Attributes["Money"].Value=="5","Other result slot changed");
  foreach(bool eclipse in new[]{false,true})foreach(int level in new[]{2,3,9,10}){
   ListSF.Eclipse=eclipse;
   var before=new XmlDocument();before.LoadXml(original);
   var baseline=new RewardStruct(before.SelectSingleNode("Fight/Rewards/Reward[2]"),0,0).KOBOIFJNPMO(level);
   var native=new RewardStruct(doc.SelectSingleNode("Fight/Rewards/Reward[2]"),0,0);
   var actual=native.KOBOIFJNPMO(level);bool applies=eclipse&&level>=3&&level<=9;
   Check(actual.GBGNFPNCGED==baseline.GBGNFPNCGED&&actual.PNDAIFALIKF==baseline.PNDAIFALIKF&&actual.exp==baseline.exp&&actual.prizeBase==baseline.prizeBase,"Native composition changed economy");
   Check(actual.HELFDCAIJNE.Any(i=>i.Name=="TEST_REWARD")==applies,"Eclipse/level gating incorrect");
   Check(actual.HELFDCAIJNE.Any(i=>i.Name=="shared"),"Shared item lost");
   Check(actual.HELFDCAIJNE.Any(i=>i.Name=="old")==false,"Replaced level item remained");
   if(applies){
    var item=actual.HELFDCAIJNE.Single(i=>i.Name=="TEST_REWARD");
    Check(item.IDGKPLBKDIB&&item.UpgradeNumber==2,"Builder/native item identity, Drop or upgrade lost");
    NekkiMath.Position=0.5f;var choice=(RewardItem)actual.PNFMKMLLFHK.Single().OOOBLJIHBEP();
    Check(choice.Name=="TEST_REWARD"&&choice.UpgradeNumber==3&&choice.IDGKPLBKDIB,"Native weighted choice lost item data");
   }
   var again=native.KOBOIFJNPMO(level);
   Check(again.HELFDCAIJNE.Count==actual.HELFDCAIJNE.Count&&again.PNFMKMLLFHK.Count==actual.PNFMKMLLFHK.Count,"Repeated reward evaluation accumulated grants");
  }
  var lotteryDoc=new XmlDocument();lotteryDoc.LoadXml("<Reward><Lottery/><EclipseModeReward><Item Name='extra'/></EclipseModeReward></Reward>");
  ListSF.Eclipse=true;
  var withLottery=new RewardStruct(lotteryDoc.DocumentElement,0,0).KOBOIFJNPMO(4);
  Check(withLottery.FAPDEKOMOGH!=null&&withLottery.HELFDCAIJNE.Single().Name=="extra","Lottery plus non-lottery mode reward failed composition");
  lotteryDoc.LoadXml("<Reward><Lottery Type='Gold'><Slot/></Lottery><EclipseModeReward><Lottery Type='Gold'><Slot/></Lottery></EclipseModeReward></Reward>");
  var repeat=new RewardStruct(lotteryDoc.DocumentElement,0,0);
  var first=repeat.KOBOIFJNPMO(4);var second=repeat.KOBOIFJNPMO(4);
  Check(first.FAPDEKOMOGH.EDCOGMLOEHE.Count==2&&second.FAPDEKOMOGH.EDCOGMLOEHE.Count==2,"Repeated evaluation accumulated lottery slots");
  ListSF.Eclipse=false;var normal=repeat.KOBOIFJNPMO(4);
  Check(normal.FAPDEKOMOGH.EDCOGMLOEHE.Count==1,"Eclipse evaluation contaminated normal lottery");
  first.FAPDEKOMOGH.EDCOGMLOEHE.Clear();
  Check(second.FAPDEKOMOGH.EDCOGMLOEHE.Count==2&&repeat.KOBOIFJNPMO(4).FAPDEKOMOGH.EDCOGMLOEHE.Count==1,"Returned lottery collections alias source/other results");
  // Feed actual builder/parser output into the production result item selector.
  ListSF.Level=4;ListSF.Eclipse=true;
  var selectedPrize=new RewardStruct(doc.SelectSingleNode("Fight/Rewards/Reward[2]"),0,0).KOBOIFJNPMO(4);
  var grant=selectedPrize.HELFDCAIJNE.Single(i=>i.Name=="TEST_REWARD");
  var sourceItem=new ItemInfo{Name="TEST_REWARD",Upgrades=new List<UpgradeData>{new UpgradeData{Number=0},new UpgradeData{Number=1},new UpgradeData{Number=2}}};
  ListSF.Catalog.Items.Add(sourceItem.Name,sourceItem);
  var result=new Result();result.KFJABAMAKOD(grant);
  Check(result.HELFDCAIJNE.Count==1&&result.HELFDCAIJNE[0].DLKPBAJDHBO.Upgrade==2&&result.HELFDCAIJNE[0].IDGKPLBKDIB,"Native result lost projected upgrade/drop");
  Check(ReferenceEquals(result.HELFDCAIJNE[0].NAIEGGHELIH,grant),"Native result detached item grant metadata");
  ListSF.Inventory.Owned.Add(sourceItem.Name);var owned=new Result();owned.KFJABAMAKOD(grant);
  Check(owned.HELFDCAIJNE.Count==0,"Owned equipment was regranted");ListSF.Inventory.Owned.Clear();
  var missing=new Result();var missingDoc=new XmlDocument();missingDoc.LoadXml("<Item Name='missing'/>");missing.KFJABAMAKOD(new RewardItem(missingDoc.DocumentElement));missing.KFJABAMAKOD(null);
  Check(missing.HELFDCAIJNE.Count==0,"Missing/null item selected");
  grant.UpgradeNumber=99;var clamped=new Result();clamped.KFJABAMAKOD(grant);
  Check(clamped.HELFDCAIJNE.Single().DLKPBAJDHBO.Upgrade==2,"Native upgrade clamp failed");
  var consumeName="example.core-fight:items/consumable/token";ListSF.Catalog.Items.Add(consumeName,new ItemInfo{Name=consumeName,Type="Consumable"});ListSF.Inventory.Owned.Add(consumeName);
  missingDoc.LoadXml("<Item Name='"+consumeName+"' Drop='1'/>");var consumable=new Result();consumable.KFJABAMAKOD(new RewardItem(missingDoc.DocumentElement));
  Check(consumable.HELFDCAIJNE.Count==1,"Owned mod consumable blocked repeat grant");
  Console.WriteLine("PASS: "+checks+" native reward builder/parser/composition/result-selection checks; controlled host services, no inventory settlement.");
 }
}
