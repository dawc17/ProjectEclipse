$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$fixture=Join-Path $root ('Temp/RewardUpgradeLookup-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$source=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ItemInfo.cs')
$methods=@('public int FMHIKMNJHDL','public List<UpgradeData> DNFDAGFAANJ','public ItemInfo HIOBANJPMKF') | ForEach-Object {
 $match=[regex]::Match($source,'(?ms)^\t'+[regex]::Escape($_)+'\(.*?^\t\}')
 if(!$match.Success){throw "Missing native method $_"};$match.Value
}
$program=@'
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
class UpgradeData:IComparable<UpgradeData> {
 public struct Fields {public int AKKLOMFOLNO,Level;}
 public Fields OGLHOJNMEBD;
 public int CompareTo(UpgradeData other)=>OGLHOJNMEBD.AKKLOMFOLNO>=other.OGLHOJNMEBD.AKKLOMFOLNO?1:-1;
}
class UpgradeDataContainer {public List<UpgradeData> KPAPEBOAKIE=new List<UpgradeData>();}
class ListSF {
 public static readonly ListSF Catalog=new ListSF();public static ListSF DJBOFEEKJMP()=>Catalog;
 public Dictionary<string,UpgradeDataContainer> Templates=new Dictionary<string,UpgradeDataContainer>();
 public UpgradeDataContainer BKPOCLGODDM(string name)=>name!=null&&Templates.TryGetValue(name,out var value)?value:null;
}
class ItemInfo {
 public List<UpgradeData> KEFPALGDBOC=new List<UpgradeData>();public string PHDCGJOKBLH;public int OBJDGBBFJOO,Level;
 public ItemInfo MPADIPJLMLH(UpgradeData data)=>new ItemInfo{OBJDGBBFJOO=data.OGLHOJNMEBD.AKKLOMFOLNO,Level=data.OGLHOJNMEBD.Level};
 /* METHODS */
}
class Program {
 static int checks;static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
 static UpgradeData Row(int level,int encoded)=>new UpgradeData{OGLHOJNMEBD=new UpgradeData.Fields{Level=level,AKKLOMFOLNO=encoded}};
 static UpgradeData Parse(XElement e)=>Row((int?)e.Attribute("Level")??0,(int?)e.Attribute("UpgradeLevel")??0);
 static void Main(string[] args){
  var item=new ItemInfo{PHDCGJOKBLH="test",OBJDGBBFJOO=100};item.KEFPALGDBOC.Add(Row(1,100));
  ListSF.Catalog.Templates["test"]=new UpgradeDataContainer{KPAPEBOAKIE=new List<UpgradeData>{Row(5,500),Row(1,90),Row(3,300)}};
  Check(item.HIOBANJPMKF(100).OBJDGBBFJOO==100,"Local exact upgrade lost");
  Check(item.HIOBANJPMKF(200).OBJDGBBFJOO==300,"Native next-entry semantics changed");
  Check(item.HIOBANJPMKF(501)==null,"Above-table request silently clamped");
  Check(item.DNFDAGFAANJ().Select(r=>r.OGLHOJNMEBD.AKKLOMFOLNO).SequenceEqual(new[]{100,300,500}),"Local/template merge or ordering changed");
  Check(item.DNFDAGFAANJ(true,3).Single().OGLHOJNMEBD.AKKLOMFOLNO==300,"Ordinal filter changed");
  var list=XDocument.Load(args[0]);
  foreach(var template in list.Descendants("Upgrades").Where(e=>e.Attribute("Name")!=null))
   ListSF.Catalog.Templates[(string)template.Attribute("Name")]=new UpgradeDataContainer{KPAPEBOAKIE=template.Elements("Upgrade").Select(Parse).ToList()};
  var items=list.Root.Element("Items").Elements("Item").ToDictionary(e=>(string)e.Attribute("Name"));
  int rows=0,missing=0,exact=0,higher=0,unavailable=0,unsupported=0;
  var stages=XDocument.Load(args[1]);
  foreach(var reward in stages.Descendants("Slot").SelectMany(e=>e.Elements("Item")).Where(e=>e.Attribute("UpgradeLevel")!=null)){
   rows++;string name=(string)reward.Attribute("Name");
   if(!items.TryGetValue(name,out var definition)){missing++;continue;}
   if((string)reward.Attribute("UpgradeLevel")!="?Player[].Level*100"){unsupported++;continue;}
   var upgrades=definition.Element("Upgrades");var native=new ItemInfo{PHDCGJOKBLH=(string)upgrades?.Attribute("Template")};
   if(upgrades!=null)native.KEFPALGDBOC.AddRange(upgrades.Elements("Upgrade").Select(Parse));
   for(int level=1;level<=52;level++){
    var chosen=native.HIOBANJPMKF(level*100);
    if(chosen==null)unavailable++;else if(chosen.OBJDGBBFJOO==level*100)exact++;else higher++;
   }
  }
  Check(rows>0,"Archive reward inventory empty");
  Console.WriteLine("PASS: "+checks+" native lookup checks. Archive projection: rows="+rows+", missing items="+missing+", unsupported expressions="+unsupported+", exact="+exact+", next-higher="+higher+", unavailable="+unavailable);
  Console.WriteLine("Archive projection reads raw local/template upgrade rows, probes levels 1..52 regardless of slot eligibility, and controls item materialization. It is not a native catalog import or full-game parity test.");
 }
}
'@
$program.Replace('/* METHODS */',($methods -join "`n")) | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Program.cs')
'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>' | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')
dotnet run --project (Join-Path $fixture 'Fixture.csproj') -- (Join-Path $root 'Assets/DExml/list.xml') (Join-Path $root 'Assets/DExml/stages.xml')
if($LASTEXITCODE -ne 0){throw 'Reward upgrade lookup checks failed.'}
