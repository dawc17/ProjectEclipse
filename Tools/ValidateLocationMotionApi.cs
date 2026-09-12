using System;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;
sealed class MotionCore : IAssetProvider
{
 public ModId Namespace=>ModId.Parse("core");
 public bool TryDescribe(AssetId id,out AssetMetadata value){value=new AssetMetadata(id,(id.Path.StartsWith("audio/") || id.Path.StartsWith("gamedata/music/")) ? AssetKind.Audio : AssetKind.Sprite,AssetSourceKind.Core,"",-1,"fixture");return true;}
}
static class Program
{
 static int checks;static string root;static string examplePath;static string musicFields="";
 static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
 static ModContentCatalog Load(string curve,out ModDescriptor mod,bool mask=false)
 {
  string dir=Path.Combine(root,"test.motion");Directory.CreateDirectory(Path.Combine(dir,"scripts"));
  File.WriteAllText(Path.Combine(dir,"mod.toml"),"schema=1\nid=\"test.motion\"\nname=\"Motion\"\nversion=\"1.0.0\"\napi=\">=0.24 <1.0\"\nauthors=[\"Eclipse\"]\nentrypoint=\"scripts/main.lua\"\ncapabilities=[\"content.register\"]\n[[dependencies]]\nid=\"core\"\nversion=\">=1.0 <2.0\"\n");
  File.WriteAllText(Path.Combine(dir,"scripts/main.lua"),"local sf2=require('sf2')\nlocal curve="+curve+"\nsf2.locations.register{id='arena',"+musicFields+"layers={{images={{sprite=sf2.assets.sprite('core:Textures/test'),width=64,height=32,flip_x=true,mask="+(mask?"true":"false")+",motion_x=curve,motion_y=curve,rotation=curve,opacity=curve}}}}}");
  mod=ModDiscovery.DiscoverLoose(root).Mods.Single();var catalog=new ModContentCatalog();
  var templates=new XmlDocument();templates.LoadXml("<Templates><Warrior Name='Default'/></Templates>");
  CoreContentImporter.ImportWarriorTemplates(catalog,templates.DocumentElement);
  if(examplePath!=null)
  {
   File.WriteAllText(Path.Combine(dir,"scripts/main.lua"),File.ReadAllText(Path.Combine(examplePath,"scripts/main.lua")));
   Directory.CreateDirectory(Path.Combine(dir,"localizations"));
   File.Copy(Path.Combine(examplePath,"localizations/eng.toml"),Path.Combine(dir,"localizations/eng.toml"),true);
  }
  using(var tx=catalog.BeginRegistration(mod))
  using(var context=new MoonSharpScriptRuntime().CreateContext(mod,new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new MotionCore(),new LooseModProvider(mod)}),tx,new ModStateRuntime(),null)))
  {ModLocalizationLoader.Load(mod,new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx);context.ExecuteEntrypoint();tx.Commit();}
  return catalog;
 }
 static void Reject(string curve,bool mask=false){bool failed=false;try{Load(curve,out var mod,mask);}catch(Exception){failed=true;}Check(failed,"Invalid curve accepted: "+curve);}
 static void Main(string[] args)
 {
  root=args[0];Directory.CreateDirectory(root);
  string curve="{offset=0.25,points={{period=1,value=0},{period=1,value=8,ease=1}}}";
  var catalog=Load(curve,out var mod);var image=catalog.Locations.Single().Layers[0].Images[0];
  Check(image.IsAnimated&&image.MotionY.Offset==0.25f&&image.Opacity.Points.Count==2,"Lua curve data missing");
  var type=typeof(ModContentCatalog).Assembly.GetType("Projection",true);var projection=Activator.CreateInstance(type,true);
  var xml=(XmlDocument)type.GetMethod("Location").Invoke(projection,new object[]{catalog.Locations.Single()});
  var effect=(XmlElement)xml.SelectSingleNode("Root/Layer/SimpleEffect");
  Check(effect!=null&&effect.GetAttribute("Type")=="Picture"&&effect.GetAttribute("PictureLocation")=="local","Native picture effect missing");
  foreach(string channel in new[]{"OscillationX","OscillationY","Rotation","Transparency"})
   Check(effect[channel]?.GetAttribute("Offset")=="0.25"&&effect.SelectNodes(channel+"/Point").Count==2,"Curve projection missing: "+channel);
  Check(effect.GetAttribute("FlipX")=="1"&&effect.GetAttribute("Width")=="64","Image presentation lost");
  string hash=ModSaveData.ComputeContentSetFingerprint(new[]{mod},catalog);
  var changed=Load(curve.Replace("0.25","0.5"),out var changedMod);
  Check(hash!=ModSaveData.ComputeContentSetFingerprint(new[]{changedMod},changed),"Curve offset absent from fingerprint");
  var staticCatalog=Load("nil",out var staticMod);Check(!staticCatalog.Locations.Single().Layers[0].Images[0].IsAnimated,"Static image changed");
  Reject(curve,true);
  foreach(string bad in new[]{"{}","{points={}}","{points={{period=1,value=1}}}",curve.Replace("period=1","period=0"),curve.Replace("period=1","period=-1"),curve.Replace("value=8","value=101"),curve.Replace("ease=1","ease=0.00000001"),curve.Replace("offset=0.25","offset=3"),curve.Replace("offset=0.25","offset=0/0"),curve.Replace("value=8","value=1/0"),curve.Replace("value=8","unknown=8"),curve.Replace("points=","extra=true,points="),"{points={[1]={period=1,value=0},[3]={period=1,value=1}}}"})Reject(bad);
  var points=new[]{new LocationCurvePoint(1,0),new LocationCurvePoint(1,2)};var immutable=new LocationCurveDefinition(0,points);points[0]=new LocationCurvePoint(1,5);
  Check(immutable.Points[0].Value==0,"Caller mutated stored curve");
  string a="sf2.assets.audio('core:audio/a')",b="sf2.assets.audio('core:audio/b')";
  musicFields="music_choices={"+a+","+b+"},";
  var music=Load("nil",out var musicMod);
  Check(music.Locations.Single().MusicChoices.Count==2,"Audio choices missing");
  var musicXml=(XmlDocument)type.GetMethod("Location").Invoke(projection,new object[]{music.Locations.Single()});
  Check(musicXml.DocumentElement.GetAttribute("Music")=="core:audio/a|core:audio/b","Music choice projection lost qualified IDs");
  var musicHash=ModSaveData.ComputeContentSetFingerprint(new[]{musicMod},music);
  musicFields="music_choices={"+b+","+a+"},";
  var reordered=Load("nil",out var reorderedMod);
  Check(musicHash!=ModSaveData.ComputeContentSetFingerprint(new[]{reorderedMod},reordered),"Music choice order absent from fingerprint");
  foreach(var invalid in new[]{"music="+a+",music_choices={"+b+"},","music_choices={"+a+","+a+"},","music_choices={[2]="+a+"},","music_choices={sf2.assets.sprite('core:Textures/test')},","music_choices={'core:audio/a'},","music_choices={"+string.Join(",",Enumerable.Range(1,17).Select(i=>"sf2.assets.audio('core:audio/"+i+"')"))+"},"})
  {musicFields=invalid;Reject("nil");}
  musicFields="dojo=true,";var opted=Load("nil",out var optedMod);
  Check(ModSaveData.ComputeContentSetFingerprint(new[]{optedMod},opted)!=ModSaveData.ComputeContentSetFingerprint(new[]{staticMod},staticCatalog),"Dojo opt-in missing from fingerprint");
  musicFields="";
  examplePath=args[1];var demo=Load("nil",out var demoMod);
  Check(demo.Locations.Single().Layers[0].Images[0].IsAnimated&&demo.Fights.Count==1&&demo.Modes.Single().Repeatable,"Shipped animated arena is not connected to a repeatable fight");
  examplePath=args[2];var dojo=Load("nil",out var dojoMod);
  Check(dojo.Locations.Single().IsDojo&&dojo.Modes.Single().Repeatable,"Shipped dojo selector does not register an eligible location and mode");
  Console.WriteLine("PASS: "+checks+" location motion/music Lua, validation, native projection and fingerprint assertions.");
 }
}
