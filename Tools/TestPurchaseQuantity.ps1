$ErrorActionPreference='Stop'
$root=Split-Path -Parent $PSScriptRoot
$source=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ListSF.cs')
$method=[regex]::Match($source,'(?ms)^\tpublic static bool KCBCGDFKNME\(ItemInfo item, ItemAction.*?^\t\}(?=\r?\n\r?\n\tpublic static bool IGLBLDKOMML)').Value
if (!$method) { throw 'Purchase dispatcher not found.' }
$affordability=[regex]::Match($source,'(?ms)^\tpublic static CheckItems CLKECIFEMNB\(.*?^\t\}(?=\r?\n\r?\n\tinternal static long CalculatePurchaseTotal)').Value
$total=[regex]::Match($source,'(?ms)^\tinternal static long CalculatePurchaseTotal\(.*?^\t\}').Value
if (!$affordability -or !$total) { throw 'Purchase affordability methods not found.' }
$fixture=Join-Path $root ('Temp/PurchaseQuantity-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture -Force | Out-Null
# Execute the production dispatcher; currency persistence, grant and UI are controlled.
$code=@'
using System;
enum ItemAction { Item_Buy_Gold,Item_Upgrade_Gold,Item_Buy_Ruby,Item_Upgrade_Ruby,Item_Buy_Real,Item_Free,Item_Consumable,Item_Delivery_Ruby,Item_Recipe_Delivery_Ruby,Item_Order_Ruby,Item_Recipe }
enum BKDHBIDPKLK { CHECK_ITEM_NONE,CHECK_ITEM_LEVEL,CHECK_ITEM_MONEY,CHECK_ITEM_BONUS,CHECK_ITEM_NO_NETWORK,CHECK_ITEM_MATERIALS }
class CheckItems { public BKDHBIDPKLK Type; public long Value; }
class ItemInfo { public string Type="Consumable"; public long Price=1; public long KLHOKKPALOK=0; public int MHGODOLNDLE=0; public long OHBBLIMNIMJ()=>Price; public long MCNMMBCJADI()=>Price; }
struct ObscuredLong { long n; public static explicit operator ObscuredLong(long n)=>new ObscuredLong{n=n}; public static implicit operator long(ObscuredLong n)=>n.n; }
class UserItem {}
class Recipe { public bool IHHJGMBGHEB(UserItem i)=>true; }
class RecipeItemInfo:ItemInfo { public Recipe OIMGNCLBPHD()=>new Recipe(); public UserItem MFEAIEJFDAM()=>new UserItem(); }
static class SystemProperties { public static bool PKLFCFBEIIG()=>true; }
class Quests { public void JLEMHLLLCLD(){} }
class Roster {
 public enum HPOIJPGPOCF { CHANGE_BUY_ITEM, CHANGE_BUY_DELIVERY }
 public void OIOOMAKNIOB(long n) { Program.Writes++; }
 public void LLNELLFMMBB(long n,HPOIJPGPOCF reason) { Program.Writes++; }
 public long BFBOEGMAMNF()=>Program.Balance; public long EHFJHFDACMP()=>Program.Balance; public int PINDEKDNCNL()=>52;
}
static class GameUtils { public static void OFOKPNFGDMD(string s) {} }
static class MenuController { public static void IAMGKKOINFC() {} }
static class LLLOJBFMONN { public static void Error(string s,object o) { throw new Exception(s); } }
static class Program {
 public static int Writes,Granted; public static long Balance=100; static int checks;
 static Quests ELEBLBJKDBI()=>new Quests();
 static Roster CCDKHLAMKKO()=>new Roster();
 static void MBBMOKFGABP(ItemInfo i){} static void BLNHEMCHIGF(ItemInfo i,bool b){}
 static bool EMEMDEAEMCB(ItemInfo i)=>true; static bool HJHCCBGILAJ(ItemInfo i)=>true;
 static void BDNBHBOJLDN(ItemInfo i,long n){Writes++;} static void ABKBFADGNBM(ItemAction a){}
 static bool PIFPAMKOPFK(RecipeItemInfo i,ItemAction a,long n,long d,int count=1)=>true;
 static bool IGLBLDKOMML(ItemInfo i,ItemAction a,long n,long d,int count=1){Granted+=count; return true;}
__METHOD__
__TOTAL__
__AFFORDABILITY__
 static void Check(bool b,string s){checks++;if(!b)throw new Exception(s);}
 static void Main(){
  foreach(var action in new[]{ItemAction.Item_Buy_Gold,ItemAction.Item_Buy_Ruby,ItemAction.Item_Consumable}){
   Writes=Granted=0;
   Check(KCBCGDFKNME(new ItemInfo(),action,50,7),"Valid purchase rejected");
   Check(Writes==1 && Granted==7,"Charged/granted quantity diverged for "+action);
  }
  foreach(int count in new[]{0,-1,int.MinValue}){
   Writes=Granted=0;
   Check(!KCBCGDFKNME(new ItemInfo(),ItemAction.Item_Buy_Gold,50,count),"Invalid quantity accepted");
   Check(Writes==0 && Granted==0,"Invalid quantity mutated purchase state");
  }
  Writes=Granted=0;
  Check(!KCBCGDFKNME(null,ItemAction.Item_Buy_Gold,50),"Missing item reported success");
  Check(Writes==0 && Granted==0,"Missing item mutated purchase state");
  Check(KCBCGDFKNME(new ItemInfo(),ItemAction.Item_Buy_Ruby,50) && Granted==1,"Default quantity changed");
  foreach(var action in new[]{ItemAction.Item_Buy_Gold,ItemAction.Item_Buy_Ruby,ItemAction.Item_Consumable}){
   Balance=100;
   Check(CLKECIFEMNB(new ItemInfo{Price=7},action,3).Value==79,"Ordinary total changed");
   var poor=CLKECIFEMNB(new ItemInfo{Price=51},action,2);
   Check(poor.Value==-1 && poor.Type==(action==ItemAction.Item_Buy_Gold?BKDHBIDPKLK.CHECK_ITEM_MONEY:BKDHBIDPKLK.CHECK_ITEM_BONUS),"Insufficient currency result changed");
   Balance=long.MaxValue;
   Check(CLKECIFEMNB(new ItemInfo{Price=long.MaxValue},action,1).Value==0,"Maximum representable price rejected");
   Check(CLKECIFEMNB(new ItemInfo{Price=long.MaxValue},action,2).Value==-1,"Negative overflow price accepted");
   Check(CLKECIFEMNB(new ItemInfo{Price=long.MaxValue/2+2},action,4).Value==-1,"Positive wraparound price accepted");
   Check(CLKECIFEMNB(new ItemInfo{Price=-1},action,1).Value==-1,"Negative unit price accepted");
   Check(CLKECIFEMNB(new ItemInfo{Price=0},action,int.MaxValue).Value==long.MaxValue,"Zero-price quantity rejected by cost math");
  }
  Check(CLKECIFEMNB(null,ItemAction.Item_Buy_Gold).Value==-1,"Missing item passed affordability");
  Check(CLKECIFEMNB(new ItemInfo(),ItemAction.Item_Buy_Gold,0).Value==-1,"Zero quantity passed affordability");
  Check(CLKECIFEMNB(new ItemInfo(),ItemAction.Item_Buy_Gold,-1).Value==-1,"Negative quantity passed affordability");
  Balance=long.MinValue;
  Check(CLKECIFEMNB(new ItemInfo{Price=1},ItemAction.Item_Buy_Gold).Value==-1,"Invalid balance overflowed into affordability");
  Console.WriteLine("Purchase quantity and affordability: "+checks+" checks passed (controlled currency/grant/UI services).");
 }
}
'@
$code.Replace('__METHOD__',$method).Replace('__TOTAL__',$total).Replace('__AFFORDABILITY__',$affordability) | Set-Content -Encoding utf8 (Join-Path $fixture 'Program.cs')
'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>' | Set-Content -Encoding utf8 (Join-Path $fixture 'Check.csproj')
dotnet run --project (Join-Path $fixture 'Check.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Purchase quantity regression failed.' }
