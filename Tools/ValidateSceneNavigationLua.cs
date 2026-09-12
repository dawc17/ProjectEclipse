using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Eclipse.Modding;
static class Program {
 static int checks;
 static void Check(bool value,string text){checks++;if(!value)throw new Exception(text);}
 static void Main(string[] args){
  var dir=Path.Combine(args[0],"example.navigation");Directory.CreateDirectory(Path.Combine(dir,"scripts"));
  foreach(var scenario in new[]{"accepted","rejected","denied","invalid","nohost","cleanup","example"}){
   File.WriteAllText(Path.Combine(dir,"mod.toml"),("schema=1\nid='example.navigation'\nname='Navigation'\nversion='1.0.0'\napi='>=0.31 <1.0'\nauthors=['Eclipse']\nentrypoint='scripts/main.lua'\ncapabilities=['story.events','ui.create'"+(scenario=="denied"?"":",'presentation.navigate'")+"]\n").Replace("'","\""));
   string code="local sf2=require('sf2')\nassert(sf2.scenes.open('shop')=="+(scenario=="rejected"?"false":"true")+")";
   if(scenario=="invalid")code="local sf2=require('sf2');sf2.scenes.open('fight')";
   if(scenario=="cleanup")code=@"local sf2=require('sf2')
local view=sf2.ui.open{id='cleanup',mount='menu',root={id='text',kind='text',text='test',width=100,height=50},
 on_close=function() local ok=pcall(function() sf2.scenes.open('shop') end);assert(not ok) end}
sf2.ui.close(view)";
   if(scenario=="example")code=File.ReadAllText(Path.Combine(args[1],"Mods/example.scene-menu/scripts/main.lua"));
   File.WriteAllText(Path.Combine(dir,"scripts/main.lua"),code);
   var mod=ModDiscovery.DiscoverLoose(args[0]).Mods.Single();var catalog=new ModContentCatalog();
   var views=new List<ModUiSurface>();var errors=new List<string>();var bus=new ModStoryEvents((id,error)=>errors.Add(error));
   int calls=0;bool allowed=scenario!="rejected";
   ModSceneAccess.Clear();if(scenario!="nohost")ModSceneAccess.Open=name=>{calls++;Check(name=="shop","wrong destination");return allowed;};
   bool failed=false;
   using(var tx=catalog.BeginRegistration(mod))
   using(var context=new MoonSharpScriptRuntime(views.Add,null,null,bus).CreateContext(mod,new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx,new ModStateRuntime(),null))){
    try{context.ExecuteEntrypoint();tx.Commit();}catch(ModScriptException){failed=true;}
    Check(failed==(scenario=="denied"||scenario=="invalid"||scenario=="nohost"),"unexpected Lua result "+scenario);
    if(scenario=="example"){
     bus.BindProfile();bus.Publish(new ModStoryEvent(ModStoryEventKind.SceneEnter,null,scene:"map"));
     Check(errors.Count==0&&views.Count==1&&views[0].TryClick("shop")&&views[0].IsClosed&&calls==1,"example accepted button");
     allowed=false;bus.Publish(new ModStoryEvent(ModStoryEventKind.SceneEnter,null,scene:"shop"));
     Check(views.Count==2&&views[1].TryClick("shop")&&!views[1].IsClosed&&calls==2,"example rejected button");
     Check(views[1].Read("status").Text=="Unavailable right now","example rejection message");
     Check(views[1].TryClick("back")&&views[1].IsClosed&&calls==2,"back invoked navigation");
     bus.Publish(new ModStoryEvent(ModStoryEventKind.SceneEnter,null,scene:"fight"));
     Check(views.Count==2,"example opened in combat");
    } else Check(calls==((scenario=="accepted"||scenario=="rejected")?1:0),"rejected call reached host "+scenario);
   }
   Check(views.All(view=>view.IsClosed),"UI survived context disposal");
  }
  ModSceneAccess.Clear();Check(ModSceneAccess.Open==null,"host teardown retained navigation");
  Console.WriteLine("PASS: "+checks+" Lua navigation/capability/example checks with controlled host.");
 }
}
