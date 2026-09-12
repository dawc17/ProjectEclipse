using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Eclipse.Modding;
static class Program {
 static int checks;
 static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
 static void Main(string[] args){
  string dir=Path.Combine(args[0],"example.story");Directory.CreateDirectory(Path.Combine(dir,"scripts"));
  foreach(string scenario in new[]{"good","denied","forged","unknown","error","loop","nohost","example","level","scene","acquired","battle"}){
   File.WriteAllText(Path.Combine(dir,"mod.toml"),("schema=1\nid='example.story'\nname='Story'\nversion='1.0.0'\napi='>=0.30 <1.0'\nauthors=['Eclipse']\nentrypoint='scripts/main.lua'\ncapabilities=["+(scenario=="denied"?"":"'story.events'")+(scenario=="scene"?",'ui.create'":"")+"]\n").Replace("'","\""));
   string script="local sf2=require('sf2')\n";
   if(scenario=="forged")script+="sf2.story.off({})";
   else if(scenario=="unknown")script+="sf2.story.on('made_up',function(e) end)";
   else if(scenario=="error"||scenario=="loop")script+=@"local bad=sf2.story.on('purchase',function(e) "+(scenario=="loop"?"while true do end":"error('deliberate')")+@" end)
sf2.story.on('purchase',function(e) assert(not sf2.story.is_active(bad)); sf2.log.info('survivor') end)";
   else script+=@"local count=0
local sub
sub=sf2.story.on('purchase',function(e)
 count=count+1
 assert(e.kind=='purchase' and e.item=='core:items/weapon/test' and e.recipe==nil)
 e.item='changed'
 assert(sf2.story.is_active(sub))
 if count==2 then sf2.story.off(sub);sf2.story.off(sub);assert(not sf2.story.is_active(sub)) end
 sf2.log.info('delivered')
end)
sf2.story.on('purchase',function(e) assert(e.item=='core:items/weapon/test') end)";
   if(scenario=="acquired")script="local sf2=require('sf2');sf2.story.on('item_acquired',function(e) assert(e.kind=='item_acquired' and e.previous_count==2 and e.count==5 and e.recipe==nil);e.count=999;sf2.log.info('acquired') end);sf2.story.on('item_acquired',function(e) assert(e.count==5) end)";
   if(scenario=="battle")script=@"local sf2=require('sf2');sf2.story.on('battle_result',function(e)
assert(e.kind=='battle_result' and e.fight=='core:fights/zone/boss/1' and e.eclipse and e.outcome=='win')
assert(e.item==nil and e.recipe==nil and #e.equipment==1 and e.equipment[1].subtype=='Katana')
e.equipment[1].subtype='changed';sf2.log.info('battle') end)
sf2.story.on('battle_result',function(e) assert(e.equipment[1].subtype=='Katana') end)";
   if(scenario=="example")script=File.ReadAllText(Path.Combine(args[1],"Mods/example.story-observer/scripts/main.lua"));
   if(scenario=="level")script=@"local sf2=require('sf2')
sf2.story.on('level_up',function(event)
 assert(event.kind=='level_up' and event.previous_level==2 and event.level==4)
 assert(event.item==nil and event.recipe==nil)
 sf2.log.info('level')
end)";
   File.WriteAllText(Path.Combine(dir,"scripts/main.lua"),script);
   if(scenario=="scene")File.WriteAllText(Path.Combine(dir,"scripts/main.lua"),@"local sf2=require('sf2')
local current
sf2.story.on('scene_enter',function(event)
 assert(event.kind=='scene_enter' and event.scene=='map')
 assert(event.item==nil and event.recipe==nil and event.previous_level==nil and event.level==nil)
 sf2.log.info('scene')
 if current and sf2.ui.is_open(current) then sf2.ui.close(current) end
 current=sf2.ui.open{id='scene_menu',mount='menu',root={id='close',kind='button',text='BACK',width=240,height=60},
  on_click=function(view,id) sf2.ui.close(view) end}
end)");
   var mod=ModDiscovery.DiscoverLoose(args[0]).Mods.Single();var catalog=new ModContentCatalog();
   var errors=new List<string>();var logs=new List<ModLogEntry>();
   var views=new List<ModUiSurface>();
   var bus=new ModStoryEvents((id,message)=>errors.Add(message));
   bool failed=false;
   using(var tx=catalog.BeginRegistration(mod))
   using(var context=new MoonSharpScriptRuntime(views.Add,null,null,scenario=="nohost"?null:bus).CreateContext(mod,
    new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx,new ModStateRuntime(),logs.Add))){
    try{context.ExecuteEntrypoint();tx.Commit();}catch(ModScriptException){failed=true;}
    bool expected=scenario=="denied"||scenario=="forged"||scenario=="unknown"||scenario=="nohost";
    Check(failed==expected,"Unexpected registration: "+scenario);
    if(!expected){
     var e=new ModStoryEvent(ModStoryEventKind.Purchase,DefinitionId.Parse("core:items/weapon/test"));
     if(scenario=="acquired")e=new ModStoryEvent(ModStoryEventKind.ItemAcquired,DefinitionId.Parse("core:items/weapon/test"),previousCount:2,count:5);
     if(scenario=="battle")e=new ModStoryEvent(ModStoryEventKind.BattleResult,null,battle:new ModBattleResultSnapshot(DefinitionId.Parse("core:fights/zone/boss/1"),"win",true,new[]{new ModBattleEquipmentSnapshot(null,"Weapon","Katana")}));
     if(scenario=="level")e=new ModStoryEvent(ModStoryEventKind.LevelUp,null,null,2,4);
     if(scenario=="scene")e=new ModStoryEvent(ModStoryEventKind.SceneEnter,null,scene:"map");
     Check(!bus.Publish(e),"delivered before profile");bus.BindProfile();
     bus.Publish(e);bus.Publish(e);bus.Publish(e);
     Check(logs.Count==(scenario=="good"?2:3),"callback count "+scenario);
     Check(errors.Count==(scenario=="good"||scenario=="example"||scenario=="level"||scenario=="scene"||scenario=="acquired"||scenario=="battle"?0:1),"callback errors "+scenario);
     if(scenario=="scene"){
      Check(views.Count==3&&views[0].IsClosed&&views[1].IsClosed&&!views[2].IsClosed,"scene callback UI replacement");
      Check(views[2].TryClick("close")&&views[2].IsClosed,"scene menu close button");
     }
     if(scenario=="example"){
      bus.Publish(new ModStoryEvent(ModStoryEventKind.Enchantment,null,DefinitionId.Parse("core:forge-profiles/simple")));
      Check(logs.Count==4&&errors.Count==0,"shipped enchantment observer");
      bus.Publish(new ModStoryEvent(ModStoryEventKind.LevelUp,null,null,1,3));
      Check(logs.Count==5&&errors.Count==0,"shipped level observer");
      bus.Publish(new ModStoryEvent(ModStoryEventKind.SceneEnter,null,scene:"shop"));
      Check(logs.Count==6&&errors.Count==0,"shipped scene observer");
      bus.Publish(new ModStoryEvent(ModStoryEventKind.ItemAcquired,DefinitionId.Parse("core:items/weapon/test"),previousCount:0,count:1));
      Check(logs.Count==7&&errors.Count==0&&logs[6].Message=="Story Observer acquired: core:items/weapon/test 0 -> 1 (gained 1)","shipped acquisition observer");
      bus.Publish(new ModStoryEvent(ModStoryEventKind.ItemAcquired,null,previousCount:2,count:5));
      Check(logs.Count==8&&errors.Count==0&&logs[7].Message=="Story Observer acquired: unregistered item 2 -> 5 (gained 3)","shipped unknown-item acquisition observer");

      bus.Publish(new ModStoryEvent(ModStoryEventKind.BattleResult,null,battle:new ModBattleResultSnapshot(DefinitionId.Parse("core:fights/zone/boss/1"),"win",true)));
      Check(logs.Count==9&&errors.Count==0&&logs[8].Message=="Story Observer battle: core:fights/zone/boss/1 win eclipse","shipped battle observer");
     }
    }
   }
   Check(!bus.HasSubscribers(ModStoryEventKind.ItemAcquired)&&!bus.HasSubscribers(ModStoryEventKind.Purchase)&&!bus.HasSubscribers(ModStoryEventKind.LevelUp)&&!bus.HasSubscribers(ModStoryEventKind.SceneEnter),"context disposal retained subscriptions");
  }
  Console.WriteLine("PASS: "+checks+" actual Lua story subscription checks.");
 }
}
