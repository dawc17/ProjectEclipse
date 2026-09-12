$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$fixture=Join-Path $root ('Temp/LotteryArtwork-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$source=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModRuntime.cs')
$resolve=[regex]::Match($source,'(?ms)^        internal static AssetId\? ResolveLotteryArtwork\(.*?^        \}')
if(!$resolve.Success){throw 'Lottery artwork resolver missing.'}
$program=@'
using System;
using System.Collections.Generic;
using System.IO;
using Eclipse.Modding;
class Program {
 internal sealed class LotteryClaim {public string PreviewImage,PreviewItem;}
 sealed class Item {public string FileName,Type;}
 sealed class Catalog {public Dictionary<string,Item> Items=new Dictionary<string,Item>();public Item KCCDBEEKBCG(string name)=>Items.TryGetValue(name,out var item)?item:null;}
 static class ListSF {public static Catalog Catalog=new Catalog();public static Catalog DJBOFEEKJMP()=>Catalog;}
 static class SF2Paths {public static string LFIIMPEAMFG()=>"UI/Items/";public static string BHCPOOOJAAK()=>"UI/Users/";}
 static class Debug {public static int Warnings;public static void LogWarning(string text){Warnings++;}}
 sealed class Loader {public HashSet<string> Available=new HashSet<string>(StringComparer.OrdinalIgnoreCase);public string Broken;public object LoadSprite(AssetId id){if(id.ToString()==Broken)throw new InvalidDataException("malformed sprite");return Available.Contains(id.ToString())?this:null;}}
 sealed class HostState {public Loader TypedAssets=new Loader();}
 static HostState Host=new HostState();static bool IsInitialized=true;
 /* RESOLVE */
 static int checks;static void Check(bool pass,string message){checks++;if(!pass)throw new Exception(message);}
 static void Main(){
  Host.TypedAssets.Available.UnionWith(new[]{"example.art:sprites/prize","core:UI/Items/blade","core:UI/Users/seal"});
  var claim=new LotteryClaim{PreviewImage="example.art:sprites/prize"};
  Check(ResolveLotteryArtwork(claim)==AssetId.Parse("example.art:sprites/prize"),"Qualified sprite was prefixed");
  ListSF.Catalog.Items["SWORD"]=new Item{FileName="blade",Type="Weapon"};claim.PreviewImage="SWORD";
  Check(ResolveLotteryArtwork(claim)==AssetId.Parse("core:UI/Items/blade"),"Item identity did not resolve icon");
  ListSF.Catalog.Items["SEAL"]=new Item{FileName="seal",Type="Seal"};claim.PreviewImage="SEAL";
  Check(ResolveLotteryArtwork(claim)==AssetId.Parse("core:UI/Users/seal"),"Seal used equipment path");
  claim.PreviewImage="missing";claim.PreviewItem="SWORD";
  Check(ResolveLotteryArtwork(claim)==AssetId.Parse("core:UI/Items/blade"),"Missing slot art hid available item art");
  claim.PreviewImage="example.art:sprites/broken";Host.TypedAssets.Broken=claim.PreviewImage;
  Check(ResolveLotteryArtwork(claim)==AssetId.Parse("core:UI/Items/blade")&&Debug.Warnings==1,"Broken optional art prevented fallback");
  claim.PreviewImage="missing";claim.PreviewItem=null;Check(ResolveLotteryArtwork(claim)==null,"Missing art produced a placeholder asset");
  claim.PreviewImage="example.art:sprites/prize";IsInitialized=false;Check(ResolveLotteryArtwork(claim)==null,"Uninitialized asset host accessed");
  Console.WriteLine("PASS: "+checks+" production lottery artwork resolution checks; native item catalog and sprite loading controlled.");
 }
}
'@
$program.Replace('/* RESOLVE */',$resolve.Value) | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Program.cs')
foreach($name in @('AssetId.cs','ModId.cs')) {Copy-Item -LiteralPath (Join-Path $root ('Assets/Scripts/Eclipse/Runtime/Modding/'+$name)) -Destination $fixture}
'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>' | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')
dotnet run --project (Join-Path $fixture 'Fixture.csproj')
if($LASTEXITCODE -ne 0){throw 'Lottery artwork checks failed.'}
