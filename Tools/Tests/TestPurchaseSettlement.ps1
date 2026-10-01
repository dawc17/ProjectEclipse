$ErrorActionPreference='Stop'
$root=Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$source=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModRuntime.cs')
$settle=[regex]::Match($source,'(?ms)^        internal static bool SettlePurchase\(.*?^        \}').Value
$defer=[regex]::Match($source,'(?ms)^        internal static bool DeferProfileSave\(.*?^        \}').Value
if (!$settle -or !$defer) { throw 'Production purchase/save methods missing.' }
$route=[regex]::Match($source,'(?ms)^        internal static bool SettleItemPurchase\(.*?^        \}').Value
if (!$route) { throw 'Production item purchase route missing.' }
$fixture=Join-Path $root ('Temp/PurchaseSettlement-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force -Path $fixture | Out-Null
$code=@'
using System;
using System.Xml;
using Eclipse.Modding;
static class Program {
 static int _profileMutationState,checks,writes; static Roster _profileRoster; static XmlNode _lotteryProfileNode;
 static string saved; static bool failSave;
 public sealed class ItemInfo { public string Name="known"; public XmlNode NodeXML=null; }
 sealed class Catalog { public bool TryResolveRuntimeItem(string name,string xml,out DefinitionId id){id=item;return name=="known";} }
 sealed class Scripts { public Catalog Content=new Catalog(); }
 static Scripts _scripts=new Scripts();
 static class StoryEvents { public static int ProfileGeneration; public static void RunDeferred(Action action)=>action(); }
 sealed class Roster { public void GGGEHAGCLGC(bool force){} }
 sealed class ListSF {
  public static ListSF GetInstance()=>new ListSF();
  public void OnAuthenticate(bool force){if(DeferProfileSave())return; if(failSave)throw new Exception("disk failure"); saved=_lotteryProfileNode.OuterXml; writes++;}
 }
 static readonly DefinitionId item=DefinitionId.Parse("example.purchase:items/token");
 static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
 static void Reject(Action action){bool rejected=false;try{action();}catch{rejected=true;}Check(rejected,"Expected failure");}
 static void Reset(){var xml=new XmlDocument();xml.LoadXml("<User Balance='100' Count='0'/>");_lotteryProfileNode=xml.DocumentElement;_profileRoster=new Roster();_profileMutationState=0;StoryEvents.ProfileGeneration++;writes=0;failSave=false;saved=xml.OuterXml;}
__DEFER__
__SETTLE__
__ROUTE__
 static void Main(){
  Reset(); string before=saved;
  Check(SettlePurchase(item,3,1,3,()=>{
   ((XmlElement)_lotteryProfileNode).SetAttribute("Balance","70");((XmlElement)_lotteryProfileNode).SetAttribute("Count","3");
   ListSF.GetInstance().OnAuthenticate(true);
   Check(saved==before && writes==0,"Intermediate native save escaped deferral");
   Check(!SettlePurchase(item,1,null,null,()=>throw new Exception("reentered")),"Nested purchase accepted");
   return true;
  }),"Successful settlement failed");
  var disk=new XmlDocument();disk.LoadXml(saved);
  Check(writes==1 && disk.DocumentElement.GetAttribute("Balance")=="70" && disk.DocumentElement.GetAttribute("Count")=="3","Final snapshot missed inventory/balance");
  Check(new ModPurchaseLedger(disk.DocumentElement).Read(item).Units==3,"Final snapshot missed receipt");
  Check(!DeferProfileSave(),"Successful purchase left save gate closed");
  Check(!SettlePurchase(item,1,1,null,()=>throw new Exception("limit bypass")) && writes==1,"Limit invoked grant or saved");
  foreach(bool throwDuringGrant in new[]{false,true}){
   Reset(); before=saved;
   Reject(()=>SettlePurchase(item,1,null,null,()=>{
    ((XmlElement)_lotteryProfileNode).SetAttribute("Balance","0");
    if(throwDuringGrant)throw new Exception("grant failure");return false;
   }));
   Check(saved==before && writes==0,"Failed mutation saved partial changes");
   Check(new ModPurchaseLedger((XmlElement)_lotteryProfileNode).Read(item).Units==0,"Failed grant recorded receipt");
   Reject(()=>ListSF.GetInstance().OnAuthenticate(true));
   Check(!SettlePurchase(item,1,null,null,()=>true),"Failed live profile allowed retry");
  }
  Reset(); before=saved;failSave=true;
  Reject(()=>SettlePurchase(item,1,null,null,()=>true));
  Check(saved==before && writes==0,"Failed disk save changed committed snapshot");
  Reject(()=>DeferProfileSave());
  Reset();
  Reject(()=>SettlePurchase(item,1,null,null,()=>{StoryEvents.ProfileGeneration++;return true;}));
  Check(writes==0,"Changed profile was saved");Reject(()=>DeferProfileSave());
  Reset(); Check(SettlePurchase(item,1,null,null,()=>true),"New profile could not settle after reload");
  Reset();
  Check(SettleItemPurchase(new ItemInfo(),4,()=>true) && new ModPurchaseLedger((XmlElement)_lotteryProfileNode).Read(item).Units==4,"Native identity route lost receipt quantity");
  Reset(); bool applied=false;
  Check(SettleItemPurchase(new ItemInfo{Name="unknown"},1,()=>{applied=true;return true;}) && applied && writes==0,"Unresolved legacy item changed behavior or invented history");
  _profileMutationState=2; applied=false;
  Check(!SettleItemPurchase(new ItemInfo{Name="unknown"},1,()=>{applied=true;return true;}) && !applied,"Fallback bypassed failed profile gate");
  Reset(); _scripts=null;
  Check(SettleItemPurchase(new ItemInfo(),1,()=>true) && writes==0,"Bootstrap path required scripts");
  _scripts=new Scripts();
  Check(!SettleItemPurchase(null,1,()=>true) && !SettleItemPurchase(new ItemInfo(),0,()=>true),"Invalid route input accepted");
  Console.WriteLine("Purchase settlement: "+checks+" checks passed (production orchestration/ledger; grant, events and disk services controlled).");
 }
}
'@
$code.Replace('__SETTLE__',$settle).Replace('__DEFER__',$defer).Replace('__ROUTE__',$route) | Set-Content -Encoding utf8 (Join-Path $fixture 'Program.cs')
$runtime=[Security.SecurityElement]::Escape((Join-Path $root 'Temp/Bin/Debug/Eclipse.Runtime.dll'))
"<Project Sdk=`"Microsoft.NET.Sdk`"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup><Reference Include=`"Eclipse.Runtime`"><HintPath>$runtime</HintPath></Reference></ItemGroup></Project>" | Set-Content -Encoding utf8 (Join-Path $fixture 'Check.csproj')
dotnet run --project (Join-Path $fixture 'Check.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Purchase settlement regression failed.' }
