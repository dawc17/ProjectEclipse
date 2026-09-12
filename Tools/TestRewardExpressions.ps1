$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$fixture=Join-Path $root ('Temp/RewardExpressions-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$program=@'
using System;
using System.Xml;
public static class Scalars {
 public static string CIPOICEEIBK(this XmlNode n,string fallback)=>n?.Value??fallback;
 public static uint ParseUint(this XmlNode n)=>n==null?0:uint.Parse(n.Value);
 public static int ToInt(this string value)=>int.TryParse(value,out int result)?result:0;
}
public enum GADCOGHCGDP {REWARD_ITEM}
public class Rewardable {protected GADCOGHCGDP CLOGJMBMMPI;protected void Parse(XmlNode n){}}
public class PerkStruct {public PerkStruct(XmlNode n){}}
public class ListSF {public static int Level;public static ListSF CCDKHLAMKKO()=>new ListSF();public int PINDEKDNCNL()=>Level;}
class Program {
 static void Main(){int checks=0;
 foreach(int level in new[]{1,4,40,52}){
  ListSF.Level=level;var doc=new XmlDocument();doc.LoadXml("<Item Name='test' Level='?Player[].Level' UpgradeLevel='?Player[].Level*100'/>");
  var item=new RewardItem(doc.DocumentElement);
  if(item.CMEFKONFDKN()!=level||item.EvaluateUpgradeLevel()!=level*100)throw new Exception("Native expression mismatch at "+level);checks++;
 }
 var invalid=new XmlDocument();invalid.LoadXml("<Item Name='test' UpgradeLevel='1.5'/>");
 bool rejected=false;try{new RewardItem(invalid.DocumentElement).EvaluateUpgradeLevel();}catch(FormatException){rejected=true;}
 if(!rejected)throw new Exception("Fractional upgrade silently coerced");checks++;
 Console.WriteLine("PASS: "+checks+" production RewardItem/native FunctionExtension expression checks; controlled roster level.");
 }
}
'@
$program | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Program.cs')
$item=[Security.SecurityElement]::Escape((Join-Path $root 'Assets/Scripts/Assembly-CSharp/RewardItem.cs'))
$firstpass=[Security.SecurityElement]::Escape((Join-Path $root 'Temp/bin/Debug/Assembly-CSharp-firstpass.dll'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
<Compile Include="$item"/><Reference Include="Assembly-CSharp-firstpass"><HintPath>$firstpass</HintPath></Reference>
</ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')
dotnet run --project (Join-Path $fixture 'Fixture.csproj')
if($LASTEXITCODE -ne 0){throw 'Reward expression checks failed. Build Assembly-CSharp-firstpass first.'}
