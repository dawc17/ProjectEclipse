using System;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;
static class Program {
 static int checks;
 static void Check(bool ok,string text){checks++;if(!ok)throw new Exception(text);}
 static void Main(string[] args){
  string dir=Path.Combine(args[0],"example.profile");Directory.CreateDirectory(Path.Combine(dir,"scripts"));
  var source=new XmlDocument();source.Load(Path.Combine(args[1],"Assets/vanillaXml/list.xml"));
  foreach(var scenario in new[]{"present","absent","denied","unavailable","forged"}){
   File.WriteAllText(Path.Combine(dir,"mod.toml"),("schema=1\nid='example.profile'\nname='Profile'\nversion='1.0.0'\napi='>=0.27 <1.0'\nauthors=['Eclipse']\nentrypoint='scripts/main.lua'\ncapabilities=['content.register'"+(scenario=="denied"?"":",'profile.read'")+"]\n[[dependencies]]\nid='core'\nversion='>=1.0 <2.0'\n").Replace("'","\""));
   string script=@"local sf2=require('sf2')
local item=sf2.items.get('core:items/weapon/weapon_nunchaku')
assert(sf2.profile.level()==12)
local first=sf2.profile.item(item)
";
   if(scenario=="absent")script+="assert(not first.present and not first.owned and not first.equipped and first.count==0 and first.upgrade==nil)";
   else if(scenario=="forged")script+="sf2.profile.item({})";
   else script+=@"assert(first.present and first.owned and first.count==2 and first.equipped and first.upgrade==3)
first.count=999;first.equipped=false
local next=sf2.profile.item(item)
assert(next.count==2 and next.equipped and next~=first)";
   File.WriteAllText(Path.Combine(dir,"scripts/main.lua"),script);
   var mod=ModDiscovery.DiscoverLoose(args[0]).Mods.Single();var catalog=new ModContentCatalog();
   CoreContentImporter.ImportWeapons(catalog,new[]{source.SelectSingleNode("/List/Items/Item[@Name='WEAPON_NUNCHAKU']")},null);
   if(scenario=="present")NativeProfileFixture.Run(catalog);
   int reads=0;ModProfileAccess.Clear();
   if(scenario!="unavailable"){
    ModProfileAccess.Level=()=>12;
    ModProfileAccess.Item=id=>{reads++;Check(id==CoreContentImporter.WeaponId("WEAPON_NUNCHAKU"),"Wrong handle identity");
     return scenario=="absent"?new ModProfileItemSnapshot(false,99,true,9):new ModProfileItemSnapshot(true,2,true,3);};
   }
   bool failed=false;
   using(var tx=catalog.BeginRegistration(mod))
   using(var context=new MoonSharpScriptRuntime().CreateContext(mod,new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx,new ModStateRuntime(),null))){
    try{context.ExecuteEntrypoint();tx.Commit();}catch(ModScriptException){failed=true;}
   }
   bool expected=scenario=="denied"||scenario=="unavailable"||scenario=="forged";
   Check(failed==expected,"Unexpected query outcome: "+scenario);
   if(scenario=="denied"||scenario=="unavailable")Check(reads==0,"Rejected query reached host");
  }
  ModProfileAccess.Clear();Check(ModProfileAccess.Level==null&&ModProfileAccess.Item==null,"Shutdown retained query services");
  Check(!new ModProfileItemSnapshot(true,0,false,0).Owned,"Empty inventory record counted as owned");
  Console.WriteLine("PASS: "+checks+" Lua profile query/capability/snapshot checks; native roster playback is separate.");
 }
}
