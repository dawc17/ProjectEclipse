using System;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;
sealed class DojoAssets:IAssetProvider {
 public ModId Namespace=>ModId.Parse("core");
 public bool TryDescribe(AssetId id,out AssetMetadata value){value=new AssetMetadata(id,id.Path.StartsWith("gamedata/music/")?AssetKind.Audio:AssetKind.Sprite,AssetSourceKind.Core,"",-1,"fixture");return true;}
}
static class Program {
 static int checks;
 static void Check(bool v,string s){checks++;if(!v)throw new Exception(s);}
 static void Main(string[] args){
  string dir=Path.Combine(args[0],"example.dojo");Directory.CreateDirectory(Path.Combine(dir,"scripts"));
  foreach(string scenario in new[]{"success","denied","foreign","unbound","core","missing_core"}){
   bool allowed=scenario!="denied";
   File.WriteAllText(Path.Combine(dir,"mod.toml"),("schema=1\nid='example.dojo'\nname='Dojo'\nversion='1.0.0'\nauthors=['Eclipse']\nentrypoint='scripts/main.lua'\ncapabilities=['content.register','ui.create'"+(allowed?",'presentation.dojo'":"")+"]\n[[dependencies]]\nid='core'\nversion='>=1.0 <2.0'\n").Replace("'", "\""));
   File.WriteAllText(Path.Combine(dir,"scripts/main.lua"),@"
local sf2=require('sf2')
local place=sf2.locations.register{id='garden',dojo=true,layers={{images={{sprite=sf2.assets.sprite('core:Textures/test')}}}}}
local ordinary=sf2.locations.register{id='ordinary',layers={{images={{sprite=sf2.assets.sprite('core:Textures/test')}}}}}
sf2.ui.open{id='chooser',mount='menu',root={id='root',kind='column',width=400,height=240,children={
 {id='select',kind='button',width=400,height=60,text='Select'}, {id='reset',kind='button',width=400,height=60,text='Reset'}, {id='invalid',kind='button',width=400,height=60,text='Invalid'},
 {id='core',kind='button',width=400,height=60,text='Core'}, {id='missing_core',kind='button',width=400,height=60,text='Missing core'} }},
 on_click=function(view,id)
  if id=='select' then sf2.locations.select_dojo(place);assert(sf2.locations.selected_dojo()==sf2.locations.name(place))
  elseif id=='core' then sf2.locations.select_dojo('core:locations/dojo_india24')
  elseif id=='missing_core' then sf2.locations.select_dojo('core:locations/not_installed')
  elseif id=='reset' then sf2.locations.reset_dojo();assert(sf2.locations.selected_dojo()==nil)
  else sf2.locations.select_dojo(ordinary) end
 end}
");
   var mod=ModDiscovery.DiscoverLoose(args[0]).Mods.Single();var catalog=new ModContentCatalog();var selection=new ModDojoSelection();selection.SetCoreLocationValidator(name=>name=="dojo_india24");int saveRequests=0;selection.SetSaveRequested(()=>saveRequests++);ModUiSurface surface=null;
   using(var tx=catalog.BeginRegistration(mod))
   using(var context=new MoonSharpScriptRuntime(s=>{surface=s;s.SetInputAllowed(true);},null,selection).CreateContext(mod,new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new DojoAssets(),new LooseModProvider(mod)}),tx,new ModStateRuntime(),null))){
    context.ExecuteEntrypoint();tx.Commit();
    Check(catalog.Locations.Count==2&&catalog.Locations.Count(l=>l.IsDojo)==1,"Dojo registration flag lost");
    selection.SetChoices(catalog.Locations.Where(l=>l.IsDojo).Select(l=>l.Id));
    var doc=new XmlDocument();doc.LoadXml("<Warrior><EclipseMods schema='1'/></Warrior>");selection.Bind(doc.DocumentElement);
    if(scenario=="foreign"){
     var foreign=DefinitionId.Parse("other.mod:locations/garden");selection.SetChoices(new[]{foreign});selection.Select(foreign);
     surface.TryClick("reset");Check(selection.SavedLocation==foreign.ToString()&&surface.IsClosed&&saveRequests==1,"Another mod reset a foreign preference");continue;
    }
    if(scenario=="unbound"){
     selection.Unbind();surface.TryClick("select");Check(!selection.IsBound&&surface.IsClosed,"Selection allowed without profile");continue;
    }
    if(scenario=="core"){
     surface.TryClick("core");Check(selection.SavedLocation=="core:locations/dojo_india24"&&selection.Resolve("dojo")=="dojo_india24"&&!surface.IsClosed,"Installed core dojo was not selected through Lua");
     surface.TryClick("reset");Check(selection.SavedLocation==""&&!surface.IsClosed&&saveRequests==2,"Core dojo reset did not request a profile save");continue;
    }
    if(scenario=="missing_core"){
     surface.TryClick("missing_core");Check(selection.SavedLocation==""&&surface.IsClosed&&saveRequests==0,"Missing core dojo was accepted");continue;
    }
    surface.TryClick("select");
    if(!allowed){Check(selection.SavedLocation==""&&surface.IsClosed,"Missing capability allowed selection");continue;}
    Check(selection.Resolve("dojo")=="example.dojo:locations/garden"&&!surface.IsClosed,"Real Lua UI callback failed to select/query");
    surface.TryClick("reset");Check(selection.SavedLocation==""&&!surface.IsClosed&&saveRequests==2,"Real Lua reset/query did not request saves");
    surface.TryClick("invalid");Check(selection.SavedLocation==""&&surface.IsClosed&&saveRequests==2,"Non-dojo selection accepted");
   }
  }
  {
   var mod=ModDiscovery.DiscoverLoose(args[1]).Mods.Single(m=>m.Id.Value=="example.dojo-selector");
   var catalog=new ModContentCatalog();var templates=new XmlDocument();templates.LoadXml("<Templates><Warrior Name='Default'/></Templates>");CoreContentImporter.ImportWarriorTemplates(catalog,templates.DocumentElement);
   var selection=new ModDojoSelection();ModUiSurface view=null;var assets=new AssetResolver(new IAssetProvider[]{new DojoAssets(),new LooseModProvider(mod)});
   using(var tx=catalog.BeginRegistration(mod))
   using(var script=new MoonSharpScriptRuntime(s=>{view=s;s.SetInputAllowed(true);},null,selection).CreateContext(mod,new ModApiFacade(mod,assets,tx,new ModStateRuntime(),null))){
    ModLocalizationLoader.Load(mod,assets,tx);script.ExecuteEntrypoint();tx.Commit();
    selection.SetChoices(catalog.Locations.Where(l=>l.IsDojo).Select(l=>l.Id));
    var save=new XmlDocument();save.LoadXml("<Warrior><EclipseMods schema='1'/></Warrior>");selection.Bind(save.DocumentElement);
    var prepare=(IModModePrepareScriptContext)script;var request=new ModModeRequest();
    Check(prepare.TryPrepareMode(catalog.Modes.Single(),0,0,request,out var error),error);
    view.TryClick("select");Check(selection.SavedLocation=="example.dojo-selector:locations/arena"&&view.IsClosed&&!request.IsPending&&request.Plan==null,"Shipped selector failed to choose and cancel preparation");
    request=new ModModeRequest();Check(prepare.TryPrepareMode(catalog.Modes.Single(),0,0,request,out error),error);request.Invalidate();
    view.TryClick("reset");Check(selection.SavedLocation!=""&&view.IsClosed,"Stale request changed preference");
    request=new ModModeRequest();Check(prepare.TryPrepareMode(catalog.Modes.Single(),0,0,request,out error),error);
    view.TryClick("reset");Check(selection.SavedLocation==""&&!request.IsPending&&request.Plan==null,"Shipped reset failed");
   }
  }
  {
   string mapDir=Path.Combine(args[0],"example.dojo.map");Directory.CreateDirectory(Path.Combine(mapDir,"scripts"));
   File.WriteAllText(Path.Combine(mapDir,"mod.toml"),"schema=1\nid=\"example.dojo.map\"\nname=\"Dojo map\"\nversion=\"1.0.0\"\nauthors=[\"Eclipse\"]\nentrypoint=\"scripts/main.lua\"\ncapabilities=[\"content.register\",\"story.events\",\"presentation.dojo\"]\n[[dependencies]]\nid=\"core\"\nversion=\">=1.0 <2.0\"\n");
   File.WriteAllText(Path.Combine(mapDir,"scripts/main.lua"),@"
local sf2=require('sf2')
sf2.quests.register{id='button',events={'session'},actions={{type='show_map_button',id='chooser',
 image='Textures/Buttons/Map/credits',x=-3095,y=-645,anchor_min_x=1,anchor_max_x=1,show_type='both'}}}
sf2.story.on('map_button',function(event)
 if event.button=='example.dojo.map.chooser' then sf2.locations.select_dojo('core:locations/dojo_india24') end
end)
");
   var mod=ModDiscovery.DiscoverLoose(args[0]).Mods.Single(m=>m.Id.Value=="example.dojo.map");
   var catalog=new ModContentCatalog();var selection=new ModDojoSelection();selection.SetCoreLocationValidator(name=>name=="dojo_india24");
   var events=new ModStoryEvents();
   using(var tx=catalog.BeginRegistration(mod))
   using(var script=new MoonSharpScriptRuntime(null,null,selection,events).CreateContext(mod,
    new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new DojoAssets(),new LooseModProvider(mod)}),tx,new ModStateRuntime(),null))){
    script.ExecuteEntrypoint();tx.Commit();
    var button=catalog.Quests.Single().Actions.Single().MapButton;
    Check(button!=null&&button.Name=="example.dojo.map.chooser"&&button.X==-3095&&button.AnchorMinX==1&&button.ShowType=="Both","Lua map button declaration lost native fields");
    var save=new XmlDocument();save.LoadXml("<Warrior><EclipseMods schema='1'/></Warrior>");selection.Bind(save.DocumentElement);events.BindProfile();
    events.Publish(new ModStoryEvent(ModStoryEventKind.MapButton,null,button:"other.button"));
    Check(selection.SavedLocation=="","Foreign button changed dojo");
    events.Publish(new ModStoryEvent(ModStoryEventKind.MapButton,null,button:"example.dojo.map.chooser"));
    Check(selection.SavedLocation=="core:locations/dojo_india24"&&selection.Resolve("dojo")=="dojo_india24","Map click did not select installed core dojo");
   }
  }
  Console.WriteLine("PASS: "+checks+" actual Lua dojo registration, UI callback and capability checks.");
 }
}
