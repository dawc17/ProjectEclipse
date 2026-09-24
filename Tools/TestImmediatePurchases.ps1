$ErrorActionPreference='Stop'
$root=Split-Path -Parent $PSScriptRoot
$source=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ItemBuyHelper.cs')
$methods=foreach($name in @('SettleImmediatePurchase','IHHKNBPKGHD','MGMAJHLAICA','NIEAANPCGLC','ApplyImmediateCoinPurchase','ApplyImmediateGemPurchase','ApplyImmediateConsumablePurchase')) {
    $match=[regex]::Match($source,"(?ms)^\t(?:public|private) static bool $name\(.*?^\t\}")
    if(!$match.Success){throw "Method not found: $name"}; $match.Value
}
$list=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ListSF.cs')
$increment=[regex]::Match($list,'(?ms)^\tinternal static bool CanIncrementItemCount\(.*?^\t\}').Value
if(!$increment){throw 'Inventory increment predicate missing.'}
$fixture=Join-Path $root ('Temp/ImmediatePurchases-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture -Force | Out-Null
$code=@'
using System;
class ItemInfo { public long CoinPrice=7,GemPrice=9; public int CPODJDDPJHB=2; public string SubType="",FAEGJAEEMGH="fixture"; }
struct ObscuredLong { long value; public static explicit operator ObscuredLong(long n)=>new ObscuredLong{value=n}; public static implicit operator long(ObscuredLong n)=>n.value; }
struct ObscuredInt { int value; public static explicit operator ObscuredInt(int n)=>new ObscuredInt{value=n}; public static implicit operator int(ObscuredInt n)=>n.value; }
class UserItem { public int Count; public int OFOPFCJNEBL()=>Count; }
class Inventory { public UserItem CMGOCLGHNLH(ItemInfo i)=>ListSF.Owner.Item; }
class Perks { public void LCDFOLAAEGM(){ListSF.Owner.Resets++;} }
class Roster {
 public enum HPOIJPGPOCF { CHANGE_BUY_ITEM }
 public UserItem Item=new UserItem(); public long Coins=100,Gems=100; public int Dirty,Resets,Currency;
 public Inventory KHCNHPCPFII()=>new Inventory(); public long BFBOEGMAMNF()=>Coins; public long EHFJHFDACMP()=>Gems;
 public void OIOOMAKNIOB(long n){Coins=n;} public void LLNELLFMMBB(long n,HPOIJPGPOCF kind){Gems=n;}
 public void GGGEHAGCLGC(bool value){Dirty++;} public Perks JLBDOBLHHAF()=>new Perks(); public void AddCurrencyCount(string name,int n){Currency+=n;}
}
static class ListSF { public static Roster Owner; public static Roster CCDKHLAMKKO()=>Owner;
__INCREMENT__
}
static class StatisticsCollector { public enum CNCDMFJLMFH { Money,Bonus } }
namespace Eclipse.Modding { static class ModRuntime {
 public static bool Deny; public static int Calls,Quantity; public static string Snapshot;
 public static bool SettleItemPurchase(ItemInfo item,int quantity,Func<bool> apply){Calls++;Quantity=quantity;if(Deny)return false;bool ok=apply();if(ok)Snapshot=$"{ListSF.Owner.Coins}/{ListSF.Owner.Gems}/{ListSF.Owner.Item.Count}";return ok;}
} }
static class Program {
 static int checks,grants,notifications;
 static bool KCBCGDFKNME(ItemInfo item){grants++;ListSF.Owner.Item.Count++;return true;}
 static void LMBHFAHHDKI(ItemInfo i,StatisticsCollector.CNCDMFJLMFH c,bool delivery){}
 static void CBADCGAEPGA(ItemInfo item){notifications++;}
__METHODS__
 static void Check(bool b,string text){checks++;if(!b)throw new Exception(text);}
 static void Reset(){ListSF.Owner=new Roster();grants=notifications=0;Eclipse.Modding.ModRuntime.Calls=0;Eclipse.Modding.ModRuntime.Deny=false;Eclipse.Modding.ModRuntime.Snapshot=null;}
 static void Main(){
  var methods=new Func<ItemInfo,bool>[] {IHHKNBPKGHD,MGMAJHLAICA,NIEAANPCGLC};
  for(int i=0;i<methods.Length;i++){
   var buy=methods[i]; Reset();
   Check(buy(new ItemInfo()),"Valid purchase rejected");
   Check(grants==1 && notifications==1 && ListSF.Owner.Item.Count==1,"Native grant/notification count changed");
   Check(Eclipse.Modding.ModRuntime.Calls==1 && Eclipse.Modding.ModRuntime.Quantity==1,"Settlement missing or duplicated");
   Check(Eclipse.Modding.ModRuntime.Snapshot==(i==0?"93/100/1":"100/91/1"),"Settlement did not include correct final currency/grant");
   Reset();Eclipse.Modding.ModRuntime.Deny=true;
   Check(!buy(new ItemInfo()) && grants==0 && ListSF.Owner.Coins==100 && ListSF.Owner.Gems==100,"Denied settlement changed currency or inventory");
   Reset();ListSF.Owner.Coins=ListSF.Owner.Gems=0;
   Check(!buy(new ItemInfo()) && Eclipse.Modding.ModRuntime.Calls==0,"Insufficient funds entered settlement");
   Reset();ListSF.Owner.Item.Count=int.MaxValue;
   Check(!buy(new ItemInfo()) && Eclipse.Modding.ModRuntime.Calls==0,"Inventory overflow entered settlement");
   Reset();
   Check(!buy(new ItemInfo{CoinPrice=-1,GemPrice=-1}) && Eclipse.Modding.ModRuntime.Calls==0,"Negative price entered settlement");
   Check(!buy(null) && Eclipse.Modding.ModRuntime.Calls==0,"Null item entered settlement");
   ListSF.Owner=null; Check(!buy(new ItemInfo()),"Missing profile accepted");
  }
  Reset();Check(NIEAANPCGLC(new ItemInfo{SubType="PerkReset"}) && ListSF.Owner.Resets==1,"Consumable perk reset lost");
  Reset();Check(NIEAANPCGLC(new ItemInfo{SubType="Currency"}) && ListSF.Owner.Currency==2,"Consumable currency effect lost");
  Console.WriteLine("Immediate purchases: "+checks+" checks passed (production methods; settlement/roster/grant services controlled).");
 }
}
'@
$code.Replace('__METHODS__',($methods -join "`n")).Replace('__INCREMENT__',$increment) | Set-Content -Encoding utf8 (Join-Path $fixture 'Program.cs')
'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>' | Set-Content -Encoding utf8 (Join-Path $fixture 'Check.csproj')
dotnet run --project (Join-Path $fixture 'Check.csproj')
if($LASTEXITCODE -ne 0){throw 'Immediate purchase regression failed.'}
