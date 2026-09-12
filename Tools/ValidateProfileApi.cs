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
assert(first.type=='Weapon' and first.subtype=='Nunchaku')
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
   var perkSource=new XmlDocument();perkSource.LoadXml("<Perk Name='TEST_PERK'/>");CoreContentImporter.ImportPerks(catalog,new[]{perkSource.DocumentElement});
   if(scenario=="present")NativeProfileFixture.Run(catalog);
   int reads=0;ModProfileAccess.Clear();
   if(scenario!="unavailable"){
    ModProfileAccess.Level=()=>12;
    ModProfileAccess.Item=id=>{reads++;Check(id==CoreContentImporter.WeaponId("WEAPON_NUNCHAKU"),"Wrong handle identity");
     return scenario=="absent"?new ModProfileItemSnapshot(false,99,true,9,"Weapon","Nunchaku"):new ModProfileItemSnapshot(true,2,true,3,"Weapon","Nunchaku");};
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
  foreach(var scenario in new[]{"learned","absent","denied","unavailable","forged"}){
   var manifest=Path.Combine(dir,"mod.toml");
   var text=File.ReadAllText(manifest).Replace(",\"profile.read\"","");
   if(scenario!="denied")text=text.Replace("[\"content.register\"]","[\"content.register\",\"profile.read\"]");
   File.WriteAllText(manifest,text);
   string lua="local sf2=require('sf2');local p=sf2.perks.get('core:perks/test_perk');local a=sf2.profile.perk("+(scenario=="forged"?"{}":"p")+");";
   lua+=scenario=="absent"?"assert(not a.learned and a.upgrade==nil)":"assert(a.learned and a.upgrade==2);a.upgrade=99;local b=sf2.profile.perk(p);assert(b.upgrade==2 and b~=a)";
   File.WriteAllText(Path.Combine(dir,"scripts/main.lua"),lua);
   var mod=ModDiscovery.DiscoverLoose(args[0]).Mods.Single();var catalog=new ModContentCatalog();var xml=new XmlDocument();xml.LoadXml("<Perk Name='TEST_PERK'/>");CoreContentImporter.ImportPerks(catalog,new[]{xml.DocumentElement});
   int reads=0;ModProfileAccess.Clear();
   if(scenario!="unavailable")ModProfileAccess.Perk=id=>{reads++;Check(id==CoreContentImporter.PerkId("TEST_PERK"),"Perk handle mapping");return new ModProfilePerkSnapshot(scenario!="absent",2);};
   bool failed=false;
   using(var tx=catalog.BeginRegistration(mod))using(var context=new MoonSharpScriptRuntime().CreateContext(mod,new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx,new ModStateRuntime(),null))){
    try{context.ExecuteEntrypoint();tx.Commit();}catch(ModScriptException){failed=true;}
   }
   Check(failed==(scenario=="denied"||scenario=="unavailable"||scenario=="forged"),"Unexpected perk outcome: "+scenario);
   if(scenario=="denied"||scenario=="forged")Check(reads==0,"Invalid perk query reached host");
  }
  foreach(string category in new[]{"items","perks"}) foreach(string scenario in new[]{"core","own","foreign","malformed","category","denied"}){
   string reference=scenario=="own"?"example.profile:"+category+"/test":scenario=="foreign"?"other.mod:"+category+"/test":scenario=="malformed"?"test":scenario=="category"?"core:quests/test":"core:"+category+"/test";
   File.WriteAllText(Path.Combine(dir,"mod.toml"),("schema=1\nid='example.profile'\nname='Profile'\nversion='1.0.0'\napi='>=0.38 <1.0'\nauthors=['Eclipse']\nentrypoint='scripts/main.lua'\ncapabilities=['story.events'"+(scenario=="denied"?"":",'profile.read'")+"]\n[[dependencies]]\nid='core'\nversion='>=1.0 <2.0'\n").Replace("'","\""));
   string query=category=="items"?"item":"perk";
   File.WriteAllText(Path.Combine(dir,"scripts/main.lua"),"local sf2=require('sf2');sf2.story.on('scene_enter',function(e) local a=sf2.profile."+query+"('"+reference+"');assert("+(category=="items"?"a.count==2":"a.learned and a.upgrade==2")+");sf2.log.info('queried') end)");
   var mod=ModDiscovery.DiscoverLoose(args[0]).Mods.Single();var catalog=new ModContentCatalog();int reads=0,completed=0;
   ModProfileAccess.Item=id=>{reads++;Check(id.ToString()==reference,"String item identity");return new ModProfileItemSnapshot(true,2,false,0);};
   ModProfileAccess.Perk=id=>{reads++;Check(id.ToString()==reference,"String perk identity");return new ModProfilePerkSnapshot(true,2);};
   var bus=new ModStoryEvents((id,message)=>{});bus.BindProfile();
   using(var tx=catalog.BeginRegistration(mod))using(var context=new MoonSharpScriptRuntime(null,null,null,bus).CreateContext(mod,new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx,new ModStateRuntime(),entry=>{if(entry.Message=="queried")completed++;}))){
    context.ExecuteEntrypoint();tx.Commit();
    bus.Publish(new ModStoryEvent(ModStoryEventKind.SceneEnter,null,scene:"map"));
    bool allowed=scenario=="core"||scenario=="own";
    Check(reads==(allowed?1:0)&&completed==(allowed?1:0),"Post-registration string query: "+category+"/"+scenario);
   }
  }
  foreach(string scenario in new[]{"known","missing_item","missing_perk","unbound"}){
   File.WriteAllText(Path.Combine(dir,"mod.toml"),("schema=1\nid='example.profile'\nname='Profile'\nversion='1.0.0'\napi='>=0.38 <1.0'\nauthors=['Eclipse']\nentrypoint='scripts/main.lua'\ncapabilities=['story.events','profile.read']\n[[dependencies]]\nid='core'\nversion='>=1.0 <2.0'\n").Replace("'","\""));
   File.WriteAllText(Path.Combine(dir,"scripts/main.lua"),@"local sf2=require('sf2')
sf2.story.on('item_acquired',function(e)
 local item=sf2.profile.item(e.item)
 assert(item.count==2 and item.type=='Weapon' and item.subtype=='Nunchaku')
 local perk=sf2.profile.perk('core:perks/"+(scenario=="missing_perk"?"missing":"test_perk")+@"')
 assert(perk.learned and perk.upgrade==2)
 sf2.log.info('native queried')
end)
sf2.story.on('item_acquired',function(e) sf2.log.info('survivor') end)");
   var mod=ModDiscovery.DiscoverLoose(args[0]).Mods.Single();var catalog=new ModContentCatalog();
   CoreContentImporter.ImportWeapons(catalog,new[]{source.SelectSingleNode("/List/Items/Item[@Name='WEAPON_NUNCHAKU']")},null);
   var xml=new XmlDocument();xml.LoadXml("<Perk Name='TEST_PERK'/>");CoreContentImporter.ImportPerks(catalog,new[]{xml.DocumentElement});
   NativeProfileFixture.Bind(catalog);
   var errors=new System.Collections.Generic.List<string>();var logs=new System.Collections.Generic.List<string>();
   var bus=new ModStoryEvents((id,message)=>errors.Add(message));bus.BindProfile();
   using(var tx=catalog.BeginRegistration(mod))using(var context=new MoonSharpScriptRuntime(null,null,null,bus).CreateContext(mod,new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx,new ModStateRuntime(),entry=>logs.Add(entry.Message)))){
    context.ExecuteEntrypoint();tx.Commit();
    if(scenario=="unbound")NativeProfileFixture.Unbind();
    var id=DefinitionId.Parse(scenario=="missing_item"?"core:items/weapon/missing":"core:items/weapon/weapon_nunchaku");
    var notification=new ModStoryEvent(ModStoryEventKind.ItemAcquired,id,previousCount:0,count:2);
    bus.Publish(notification);bus.Publish(notification);
    Check(logs.Count(message=>message=="survivor")==2,"Query failure interrupted other listeners: "+scenario);
    Check(logs.Count(message=>message=="native queried")==(scenario=="known"?2:0),"Native callback query result: "+scenario);
    Check(errors.Count==(scenario=="known"?0:1),"Query failure did not cancel its listener: "+scenario);
    if(scenario!="known")Check(errors[0].Contains(scenario=="unbound"?"No active game profile":scenario=="missing_item"?"unavailable item":"unavailable perk"),"Native query error lost its cause: "+scenario);
   }
  }
  foreach(string scenario in new[]{"equipment","denied","unbound"}){
   var manifest=File.ReadAllText(Path.Combine(dir,"mod.toml"));
   if(scenario=="denied")manifest=manifest.Replace(",\"profile.read\"","");
   else if(!manifest.Contains("\"profile.read\""))manifest=manifest.Replace("\"story.events\"","\"story.events\",\"profile.read\"");
   File.WriteAllText(Path.Combine(dir,"mod.toml"),manifest);
   File.WriteAllText(Path.Combine(dir,"scripts/main.lua"),@"local sf2=require('sf2')
local items=sf2.profile.equipment()
assert(#items==2 and items[1].item=='core:items/weapon/weapon_nunchaku' and items[1].subtype=='Nunchaku')
assert(items[1].owned and items[1].count==2 and items[1].upgrade==3)
assert(items[2].item==nil and not items[2].owned and items[2].type==nil)
items[1].count=999;items[2]=nil
local next=sf2.profile.equipment()
assert(#next==2 and next[1].count==2 and next~=items and next[1]~=items[1])");
   var mod=ModDiscovery.DiscoverLoose(args[0]).Mods.Single();var catalog=new ModContentCatalog();
   CoreContentImporter.ImportWeapons(catalog,new[]{source.SelectSingleNode("/List/Items/Item[@Name='WEAPON_NUNCHAKU']")},null);
   NativeProfileFixture.Bind(catalog);if(scenario=="unbound")NativeProfileFixture.Unbind();
   bool failed=false;
   using(var tx=catalog.BeginRegistration(mod))using(var context=new MoonSharpScriptRuntime().CreateContext(mod,new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx,new ModStateRuntime(),null))){
    try{context.ExecuteEntrypoint();tx.Commit();}catch(ModScriptException){failed=true;}
   }
   Check(failed==(scenario!="equipment"),"Equipment query outcome: "+scenario);
  }
  ModProfileAccess.Clear();Check(ModProfileAccess.Level==null&&ModProfileAccess.Item==null&&ModProfileAccess.Perk==null&&ModProfileAccess.Equipment==null,"Shutdown retained query services");
  Check(!new ModProfileItemSnapshot(true,0,false,0).Owned,"Empty inventory record counted as owned");
  Console.WriteLine("PASS: "+checks+" Lua profile query/capability/snapshot checks; native roster playback is separate.");
 }
}
