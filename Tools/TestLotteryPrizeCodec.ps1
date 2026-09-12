$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$fixture=Join-Path $root ('Temp/LotteryPrizeCodec-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$codec=[Security.SecurityElement]::Escape((Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModLotteryPrizeCodec.cs'))
$game=[Security.SecurityElement]::Escape((Join-Path $root 'Temp/bin/Debug/Assembly-CSharp.dll'))
$firstpass=[Security.SecurityElement]::Escape((Join-Path $root 'Temp/bin/Debug/Assembly-CSharp-firstpass.dll'))
$runtime=[Security.SecurityElement]::Escape((Join-Path $root 'Temp/bin/Debug/Eclipse.Runtime.dll'))
$program=@'
using System;
using System.Xml;
using System.Linq;
using System.IO;
using Eclipse.Modding;
class Program {
 static int checks;
 static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
 static void Main(){
  // Avoid Unity-dependent item construction; only persisted identity fields are populated.
  var item=(ItemInfo)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(ItemInfo));
  item.Name="test_item";item.MHGODOLNDLE=40;item.OBJDGBBFJOO=4000;
  var doc=new XmlDocument();doc.LoadXml("<Item Name='test_item'><Enchantments><Perk Name='test_effect' ItemType='Weapon|Armor'><Set Power='12' Chance='0.3'/></Perk></Enchantments></Item>");
  var reward=new RewardItem(doc.DocumentElement);
  var original=new FightResult.ResultPrizeStruct{GBGNFPNCGED=500,PNDAIFALIKF=12,exp=30};
  original.HELFDCAIJNE.Add(new FightResult.LJFFIBFBGID{DLKPBAJDHBO=item,NAIEGGHELIH=reward,IDGKPLBKDIB=true});
  var saved=new XmlDocument();saved.AppendChild(ModLotteryPrizeCodec.Write(saved,original));
  var loaded=new XmlDocument();loaded.LoadXml(saved.OuterXml);
  var restored=ModLotteryPrizeCodec.Read(loaded.DocumentElement,(name,level,upgrade)=>item,name=>null,name=>null);
  Check(restored.GBGNFPNCGED==500&&restored.PNDAIFALIKF==12&&restored.exp==30,"Scalar prize changed");
  Check(restored.HELFDCAIJNE.Single().DLKPBAJDHBO.OBJDGBBFJOO==4000&&restored.HELFDCAIJNE[0].IDGKPLBKDIB,"Item upgrade/drop changed");
  var perk=restored.HELFDCAIJNE[0].NAIEGGHELIH.LDLPCOFHFKE.Single();
  Check(perk.get_Name()=="test_effect"&&perk.NMOKPAPJLCN.SequenceEqual(new[]{"Weapon","Armor"})&&perk.Pairs.Count==2,"Enchantment changed");
  bool rejected=false;try{ModLotteryPrizeCodec.Read(loaded.DocumentElement,(name,level,upgrade)=>null,name=>null,name=>null);}catch(InvalidDataException){rejected=true;}
  Check(rejected,"Missing item silently lost");
  loaded.DocumentElement.SetAttribute("Format","2");rejected=false;
  try{ModLotteryPrizeCodec.Read(loaded.DocumentElement,(name,level,upgrade)=>item,name=>null,name=>null);}catch(InvalidDataException){rejected=true;}
  Check(rejected,"Unknown version accepted");
  Console.WriteLine("PASS: "+checks+" prize codec checks with native reward/item/perk types; controlled content resolution, no grants.");
 }
}
'@
$program | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Program.cs')
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup><Compile Include="$codec"/><Reference Include="Assembly-CSharp"><HintPath>$game</HintPath></Reference><Reference Include="Assembly-CSharp-firstpass"><HintPath>$firstpass</HintPath></Reference><Reference Include="Eclipse.Runtime"><HintPath>$runtime</HintPath></Reference></ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')
dotnet run --project (Join-Path $fixture 'Fixture.csproj')
if($LASTEXITCODE -ne 0){throw 'Lottery prize codec tests failed. Compile Assembly-CSharp first.'}
