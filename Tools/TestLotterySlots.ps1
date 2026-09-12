$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$fixture=Join-Path $root ('Temp/LotterySlots-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$runtime=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModRuntime.cs')
$selection=[regex]::Match($runtime,'(?ms)^        internal static bool TrySelectLotterySlot\(.*?^        \}')
if(!$selection.Success){throw 'Native lottery selector not found.'}
$program=@'
using System;
using System.Collections.Generic;
using System.Xml;
using System.Globalization;
public static class XmlValues {
 public static string CIPOICEEIBK(this XmlAttribute a,string fallback)=>a?.Value??fallback;
 public static int ParseInt(this XmlAttribute a,int fallback)=>int.TryParse(a?.Value,out var n)?n:fallback;
 public static float ParseFloat(this XmlAttribute a,float fallback)=>float.TryParse(a?.Value,NumberStyles.Float,CultureInfo.InvariantCulture,out var n)?n:fallback;
}
public class RewardPrize {public XmlNode Source;public int Level;}
public class Reward {readonly XmlNode node;public Reward(XmlNode n,ushort min,ushort max){node=n;}public RewardPrize KOBOIFJNPMO(int level)=>new RewardPrize{Source=node,Level=level};}
public class Rewardable {public enum GADCOGHCGDP {REWARD_LOTTERY}protected GADCOGHCGDP CLOGJMBMMPI;}
class Program {
 /* SELECTION */
 static int checks;static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
 static void Main(string[] args){
  var xml=new XmlDocument();xml.Load(args[0]);int slots=0;
  foreach(XmlNode lottery in xml.SelectNodes("//Lottery")){
   var parsed=new RewardLottery(lottery,0,0);Check(parsed.LotteryType==lottery.Attributes["Type"].Value,"Lottery type lost");
   var source=lottery.SelectNodes("Slot|Level/Slot");Check(parsed.EDCOGMLOEHE.Count==source.Count,"Slot count mismatch");
   for(int i=0;i<source.Count;i++){
    var slot=parsed.EDCOGMLOEHE[i];var node=source[i];slots++;
    Check(slot.Weight==node.Attributes["Weight"].ParseFloat(1)&&slot.Image==node.Attributes["Image"].Value&&slot.ViewType==node.Attributes["ViewType"].Value&&slot.CancellingItem==node.Attributes["CancellingItem"].CIPOICEEIBK(""),"Archived slot data lost");
   }
  }
  Check(slots==154,"Archived lottery inventory changed; review fixture expectations");
  xml.LoadXml("<Lottery Type='Gold'><Slot Image='plain'/><Level Min='3' Max='9'><Slot Weight='7'><Item Name='example'/></Slot></Level><Level Min='5'><Slot/></Level><Level Max='2'><Slot/></Level></Lottery>");
  var sample=new RewardLottery(xml.DocumentElement,0,0);
  Check(sample.EDCOGMLOEHE[0].Weight==1,"Missing weight default differs from RewardChoice");
  var bounded=sample.EDCOGMLOEHE[1];
  Check(!bounded.TryEvaluateAtLevel(2,out var ignored)&&!bounded.TryEvaluateAtLevel(10,out ignored),"Out-of-range reward evaluated");
  Check(bounded.TryEvaluateAtLevel(3,out var low)&&low.Level==3&&bounded.TryEvaluateAtLevel(9,out var high)&&high.Level==9,"Inclusive level limits lost");
  Check(low.Source.SelectSingleNode("Item").Attributes["Name"].Value=="example","Reward payload lost");
  Check(!sample.EDCOGMLOEHE[2].TryEvaluateAtLevel(4,out ignored)&&sample.EDCOGMLOEHE[2].TryEvaluateAtLevel(100,out ignored),"Open upper bound");
  Check(sample.EDCOGMLOEHE[3].TryEvaluateAtLevel(1,out ignored)&&!sample.EDCOGMLOEHE[3].TryEvaluateAtLevel(3,out ignored),"Open lower bound");
  Check(!default(MANJCIGJPMK).TryEvaluateAtLevel(1,out ignored),"Uninitialized slot evaluated");
  var clone=sample.CloneForRewardComposition();clone.EDCOGMLOEHE.RemoveAt(0);Check(sample.EDCOGMLOEHE.Count==4&&clone.EDCOGMLOEHE[0].Weight==7,"Composition lost slot metadata or aliased list");
  xml.LoadXml("<Lottery><Slot Image='zero' Weight='0'/><Slot Image='one' Weight='1'/><Slot Image='three' Weight='3'/></Lottery>");
  sample=new RewardLottery(xml.DocumentElement,0,0);
  Check(TrySelectLotterySlot(sample,1,0,null,out var chosen)&&chosen.Image=="one","Zero-weight slot selected");
  Check(TrySelectLotterySlot(sample,1,0.249999,null,out chosen)&&chosen.Image=="one","First weighted interval");
  Check(TrySelectLotterySlot(sample,1,0.25,null,out chosen)&&chosen.Image=="three","Boundary belongs to next interval");
  Check(TrySelectLotterySlot(sample,1,0.999999,null,out chosen)&&chosen.Image=="three","Last weighted interval");
  int calls=0;
  Check(TrySelectLotterySlot(sample,1,0.9,slot=>{calls++;return slot.Image=="one";},out chosen)&&chosen.Image=="one"&&calls==2,"Eligibility not cached or weights not renormalized");
  Check(!TrySelectLotterySlot(sample,1,0,slot=>false,out chosen),"Empty eligible set selected a prize");
  foreach(double invalid in new[]{-0.01,1,double.NaN,double.PositiveInfinity}){
   bool rejected=false;try{TrySelectLotterySlot(sample,1,invalid,null,out chosen);}catch(ArgumentOutOfRangeException){rejected=true;}Check(rejected,"Invalid random sample accepted");
  }
  xml.LoadXml("<Lottery><Level Min='3' Max='9'><Slot Weight='5'/></Level></Lottery>");sample=new RewardLottery(xml.DocumentElement,0,0);
  Check(!TrySelectLotterySlot(sample,2,0,null,out chosen)&&TrySelectLotterySlot(sample,3,0,null,out chosen)&&TrySelectLotterySlot(sample,9,0,null,out chosen)&&!TrySelectLotterySlot(sample,10,0,null,out chosen),"Selection ignored level limits");
  foreach(string weight in new[]{"-1","NaN","Infinity"}){
   xml.LoadXml("<Lottery><Slot Weight='"+weight+"'/></Lottery>");sample=new RewardLottery(xml.DocumentElement,0,0);
   bool rejected=false;try{TrySelectLotterySlot(sample,1,0,null,out chosen);}catch(InvalidOperationException){rejected=true;}Check(rejected,"Invalid weight accepted");
  }
  Console.WriteLine("PASS: "+checks+" native lottery slot checks, including 154 archived slots; reward evaluation service controlled.");
 }
}
'@
$program.Replace('/* SELECTION */',$selection.Value) | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Program.cs')
$sources=@('MANJCIGJPMK.cs','RewardLottery.cs') | ForEach-Object {
 $path=[Security.SecurityElement]::Escape((Join-Path $root ('Assets/Scripts/Assembly-CSharp/'+$_)))
 '<Compile Include="'+$path+'" />'
}
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
$($sources -join [Environment]::NewLine)
</ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')
dotnet run --project (Join-Path $fixture 'Fixture.csproj') -- (Join-Path $root 'Assets/DExml/stages.xml')
if($LASTEXITCODE -ne 0){throw 'Lottery slot checks failed.'}
