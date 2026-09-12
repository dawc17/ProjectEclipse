$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$fixture=Join-Path $root ('Temp/BattleResultCapture-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$source=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModRuntime.cs')
$method=[regex]::Match($source,'(?ms)^        internal static ModStoryEvent CaptureBattleResult\(.*?^        \}')
if(!$method.Success){throw 'Native result capture method not found.'}
$program=@'
using System;
using System.Collections.Generic;
using Eclipse.Modding;
class Program {
 public enum GameOverTypes {GAME_OVER_NONE,GAME_OVER_WIN,GAME_OVER_LOSS,GAME_OVER_SURRENDER,GAME_OVER_RAID_TIMEOUT,GAME_OVER_RAID_ROUND_TIMEOUT}
 public class Roster {public bool Eclipse=true;public bool JPMPIDFGCJL()=>Eclipse;}
 public class FightList {public string BCKFACGMOKC="zone|boss|1";}
 public class Item {public string Name="katana",Type="Weapon",MDPPNGIEJGD="Katana";public System.Xml.XmlNode NodeXML;}
 public class ModelParameters {public bool IsPlayer;public List<Item> Items=new List<Item>();public List<Item> PJNJIJIODHE()=>Items;}
 public class Definition {public DefinitionId Id=DefinitionId.Parse("core:fights/zone/boss/1");}
 public class Catalog {
  public List<Definition> Fights=new List<Definition>{new Definition()};
  public string RuntimeFightId(DefinitionId id)=>"zone|boss|1";
  public bool TryResolveRuntimeItem(string name,string xml,out DefinitionId id){id=DefinitionId.Parse("core:items/weapon/katana");return name=="katana";}
 }
 public class Scripts {public Catalog Content=new Catalog();}
 static Scripts _scripts=new Scripts();static Roster _profileRoster=new Roster();
 static ModStoryEvents StoryEvents=new ModStoryEvents();static int checks;
 static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
 /* CAPTURE */
 static void Main(){
  StoryEvents.BindProfile();StoryEvents.CreateScope(ModId.Parse("example.test")).Subscribe(ModStoryEventKind.BattleResult,e=>{});
  var fight=new FightList();var player=new ModelParameters{IsPlayer=true};player.Items.Add(new Item());
  var outcomes=new[]{GameOverTypes.GAME_OVER_WIN,GameOverTypes.GAME_OVER_LOSS,GameOverTypes.GAME_OVER_SURRENDER,GameOverTypes.GAME_OVER_RAID_TIMEOUT,GameOverTypes.GAME_OVER_RAID_ROUND_TIMEOUT};
  var names=new[]{"win","loss","surrender","raid_timeout","raid_round_timeout"};
  for(int i=0;i<outcomes.Length;i++){
   var captured=CaptureBattleResult(_profileRoster,fight,outcomes[i],new ModelParameters(),player).Battle;
   Check(captured.Outcome==names[i]&&captured.Eclipse&&captured.Fight==_scripts.Content.Fights[0].Id,"Outcome/identity mapping");
  }
  var first=CaptureBattleResult(_profileRoster,fight,outcomes[0],player,null).Battle;
  player.Items[0].Name="unknown";player.Items[0].MDPPNGIEJGD="Changed";_profileRoster.Eclipse=false;
  var next=CaptureBattleResult(_profileRoster,fight,outcomes[0],player,null).Battle;
  Check(first.Equipment[0].Subtype=="Katana"&&first.Eclipse&&first.Equipment[0].Item.HasValue,"Snapshot aliased native state");
  Check(next.Equipment[0].Item==null&&next.Equipment[0].Subtype=="Changed"&&!next.Eclipse,"Unknown item metadata lost");
  Check(CaptureBattleResult(_profileRoster,fight,outcomes[0],null,null).Battle.Equipment==null,"Missing player guessed");
  fight.BCKFACGMOKC="unknown";
  Check(CaptureBattleResult(_profileRoster,fight,outcomes[0],player,null).Battle.Fight==null,"Unknown fight guessed");
  Check(CaptureBattleResult(new Roster(),fight,outcomes[0],player,null)==null,"Foreign profile accepted");
  Check(CaptureBattleResult(_profileRoster,fight,GameOverTypes.GAME_OVER_NONE,player,null)==null,"Unknown outcome accepted");
  StoryEvents.UnbindProfile();Check(CaptureBattleResult(_profileRoster,fight,outcomes[0],player,null)==null,"Unbound event capture");
  Console.WriteLine("PASS: "+checks+" production battle-result capture checks with controlled native services.");
 }
}
'@
$program.Replace('/* CAPTURE */',$method.Value) | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Program.cs')
$sources=@('ModId.cs','DefinitionId.cs','ModStoryEvents.cs') | ForEach-Object {
 $path=[Security.SecurityElement]::Escape((Join-Path $root ('Assets/Scripts/Eclipse/Runtime/Modding/'+$_)))
 '<Compile Include="'+$path+'" />'
}
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
$($sources -join [Environment]::NewLine)
</ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')
dotnet run --project (Join-Path $fixture 'Fixture.csproj')
if($LASTEXITCODE -ne 0){throw 'Battle result capture checks failed.'}
