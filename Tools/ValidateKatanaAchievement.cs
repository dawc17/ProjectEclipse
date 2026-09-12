using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;
static class Program {
 sealed class Core:IAssetProvider {
  public ModId Namespace=>ModId.Parse("core");
  public bool TryDescribe(AssetId id,out AssetMetadata metadata){metadata=new AssetMetadata(id,AssetKind.Sprite,AssetSourceKind.Core,"",-1,"fixture");return id==AssetId.Parse("core:UI/Achievements/ach_boss_butcher");}
 }
 static int checks;
 static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
 static void Main(string[] args){
  var mod=ModDiscovery.DiscoverLoose(args[0]).Mods.Single();
  Check(!DependencyResolver.Resolve(new[]{mod},ModPlatformVersions.Api,ModPlatformVersions.Core).HasErrors,"Manifest compatibility");
  Check(File.Exists(Path.Combine(args[1],"Assets/Resources/ui/achievements/ach_boss_butcher.asset")),"Core icon missing");
  var assets=new AssetResolver(new IAssetProvider[]{new Core(),new LooseModProvider(mod)});var catalog=new ModContentCatalog();var errors=new List<string>();var logs=new List<string>();
  var bus=new ModStoryEvents((id,message)=>errors.Add(message));bus.BindProfile();
  int count=0,writes=0;
  ModProgressionAccess.Read=id=>count;ModProgressionAccess.Advance=(id,n)=>{writes++;Check(id==DefinitionId.Parse("example.katana-achievement:counters/butcher_with_katana")&&n==1,"Wrong counter mutation");return count=Math.Min(1,count+n);};
  using(var tx=catalog.BeginRegistration(mod))using(var context=new MoonSharpScriptRuntime(null,null,null,bus).CreateContext(mod,new ModApiFacade(mod,assets,tx,new ModStateRuntime(),e=>logs.Add(e.Message)))){
   ModLocalizationLoader.Load(mod,assets,tx);context.ExecuteEntrypoint();tx.Commit();
   Check(catalog.Counters.Count==1&&catalog.Counters.Single().Maximum==1&&catalog.Achievements.Count==1&&catalog.Achievements.Single().Threshold==1,"Achievement definitions");
   var stages=new XmlDocument();stages.Load(Path.Combine(args[1],"Assets/vanillaXml/stages.xml"));
   void Emit(string battle,string number,string outcome,string type,string subtype,bool knownEquipment=true){
    var equipment=knownEquipment?new[]{new ModBattleEquipmentSnapshot(null,type,subtype)}:null;
    bus.Publish(new ModStoryEvent(ModStoryEventKind.BattleResult,null,battle:new ModBattleResultSnapshot(CoreContentImporter.FightId("ZONE_3",battle,number),outcome,battle.EndsWith("ECLIPSEMODE"),equipment)));
   }
   foreach(var battle in new[]{"BOSS_BUTCHER","BOSS_BUTCHER_ECLIPSEMODE","BOSS_BUTCHER_INTERMISSION"}){
    string number=battle.EndsWith("INTERMISSION")?"1":"6";
    Check(stages.SelectSingleNode("//Zone[@Name='ZONE_3']/Battle[@Name='"+battle+"']/Fight[@Name='"+number+"']/Warriors/Warrior[@Template='Butcher_Backswords']")!=null,"Encounter does not contain Butcher");
    count=0;writes=0;logs.Clear();Emit(battle,number,"win","Weapon","Katana");
    Check(count==1&&writes==1&&logs.SequenceEqual(new[]{"Blade Discipline unlocked"}),"Qualifying victory rejected: "+battle);
    Emit(battle,number,"win","Weapon","Katana");Check(writes==1&&logs.Count==1,"Repeat unlock");
   }
   count=0;writes=0;logs.Clear();
   foreach(var outcome in new[]{"loss","surrender","raid_timeout","raid_round_timeout"})Emit("BOSS_BUTCHER","6",outcome,"Weapon","Katana");
   foreach(var number in new[]{"1","2","3","4","5"})Emit("BOSS_BUTCHER",""+number,"win","Weapon","Katana");
   Emit("BOSS_HERMIT","6","win","Weapon","Katana");Emit("BOSS_BUTCHER","6","win","Weapon","Nunchaku");Emit("BOSS_BUTCHER","6","win","Armor","Katana");Emit("BOSS_BUTCHER","6","win","Weapon","Katana",false);
   Check(writes==0&&count==0&&logs.Count==0&&errors.Count==0,"Nonqualifying results advanced or failed");
  }
  Check(!bus.HasSubscribers(ModStoryEventKind.BattleResult),"Disposed sample retained listener");ModProgressionAccess.Clear();
  Console.WriteLine("PASS: "+checks+" shipped Katana Achievement registration/predicate checks; native persistence tested separately.");
 }
}
