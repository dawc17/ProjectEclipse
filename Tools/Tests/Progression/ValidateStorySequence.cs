using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

static class Program
{
    static int checks;
    static void Check(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }
    sealed class Lease : IDisposable { public bool Disposed; public Action Closed; public void Dispose() { if (Disposed) return; Disposed = true; Closed?.Invoke(); } }
    const string Lua = """
local sf2=require('sf2')
local text=sf2.localization.register{id='text',language='eng',value='Text'}
sf2.state.register{version=1,fields={next={type=sf2.state.INTEGER,default=1}}}
local function definition()
 return {position='next',steps={
  {dialog={button=text,lines={{text=text}}}},
  {act_screen={lines={{text=text,frames=1}}}},
  {dialog={button=text,lines={{text=text}}}},
 },on_complete=function() sf2.log.info('complete') end,
 on_cancel=function() sf2.log.info('cancel') end}
end
for _, selected in ipairs{'open','sparse','both','nested_callback','late_invalid','bad_position','bad_callback','failing','looping','cancel_open','veto'} do
sf2.story.on('item_acquired',function(event)
 local command=event.item:match('items/(.+)$')
 if command ~= selected then return end
 local d=definition()
 if command=='veto' then d.on_step=function() return false end
 elseif command=='sparse' then d.steps[2]=nil
 elseif command=='both' then d.steps[1].act_screen=d.steps[2].act_screen
 elseif command=='nested_callback' then d.steps[1].dialog.on_complete=function() end
 elseif command=='late_invalid' then d.steps[3].dialog.button='text'
 elseif command=='bad_position' then d.position='undeclared'
 elseif command=='bad_callback' then d.on_cancel=true
 elseif command=='failing' then d.on_step=function() error('step-failure') end
 elseif command=='looping' then d.on_step=function() while true do end end
 elseif command=='cancel_open' then d.on_cancel=function() sf2.story.play_sequence(definition()) end
 end
 sf2.log.info('accepted:'..tostring(sf2.story.play_sequence(d)))
end)
end
""";
    static void Main(string[] args)
    {
        var package=Path.Combine(args[0],"fixture.dialog"); Directory.CreateDirectory(Path.Combine(package,"scripts"));
        File.WriteAllText(Path.Combine(package,"mod.toml"), "schema=1\nid=\"fixture.dialog\"\nname=\"Sequences\"\nversion=\"1.0.0\"\nauthors=[\"Tests\"]\nentrypoint=\"scripts/main.lua\"\ncapabilities=[\"content.register\",\"story.events\",\"ui.create\",\"state.read\",\"state.write\"]\n");
        File.WriteAllText(Path.Combine(package,"scripts/main.lua"),Lua);
        var mod=ModDiscovery.DiscoverLoose(args[0]).Mods.Single();
        var logs=new List<string>(); var callbacks=new List<Action<bool>>(); var leases=new List<Lease>();
        bool sync=false, refuse=false, dishonest=false;
        IDisposable Open(Action<bool> done) {
            if (refuse) { if(dishonest) done(true); return null; }
            callbacks.Add(done); var lease=new Lease { Closed=()=>done(false) }; leases.Add(lease);
            if(sync) done(true); return lease;
        }
        ModStoryDialogAccess.Open=(request,done)=>Open(done);
        ModActScreenAccess.Open=(lines,done)=>Open(done);
        var state=new ModStateRuntime(); var bus=new ModStoryEvents((owner,message)=>logs.Add(message));
        var catalog=new ModContentCatalog(); using var tx=catalog.BeginRegistration(mod);
        var context=new MoonSharpScriptRuntime(null,null,null,bus).CreateContext(mod,new ModApiFacade(mod,
            new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx,state,entry=>logs.Add(entry.Message)));
        context.ExecuteEntrypoint();tx.Commit();state.FreezeDefinitions();
        XmlDocument Save(string xml=null) { var doc=new XmlDocument();doc.LoadXml(xml??"<Warrior><EclipseMods schema='1'><Mod id='fixture.dialog'/></EclipseMods></Warrior>");return doc; }
        void Bind(XmlDocument doc) { Check(state.Bind(doc.DocumentElement,new[]{context}).Count==0,"Bind failed");bus.BindProfile(); }
        long Position() { state.TryGetValue(mod.Id,"next",out var value);return value.Integer; }
        void Reset() { state.SetValues(mod.Id,new Dictionary<string,ModParameterValue>{{"next",ModParameterValue.FromInteger(1)}}); }
        void Run(string command) { logs.Clear();bus.Publish(new ModStoryEvent(ModStoryEventKind.ItemAcquired,DefinitionId.Parse("fixture.dialog:items/"+command),previousCount:0,count:1)); }
        var save=Save();Bind(save);
        foreach(var command in new[]{"sparse","both","nested_callback","late_invalid","bad_position","bad_callback","failing","looping","veto"}) {
            int before=callbacks.Count;Run(command);Check(callbacks.Count==before && !logs.Contains("accepted:true"),"Invalid sequence opened: "+command);Check(Position()==1,"Invalid input advanced state");
            if(command=="looping")Check(logs.Any(x=>x.Contains("instruction budget")),"Loop was not budget-limited");
        }
        Run("open");Check(logs.Contains("accepted:true"),"Sequence refused: "+string.Join("|",logs));
        int count=callbacks.Count;Run("open");Check(logs.Contains("accepted:false")&&callbacks.Count==count,"Duplicate open");
        var first=callbacks.Last();first(true);Check(Position()==2&&callbacks.Count==count+1,"First acknowledgement did not save/advance");
        first(true);Check(Position()==2&&callbacks.Count==count+1,"Duplicate acknowledgement advanced twice");
        callbacks.Last()(false);Check(Position()==2&&logs.Contains("cancel"),"Cancel did not preserve cursor");
        string saved=save.OuterXml;Bind(Save());first(true);Check(Position()==1,"Stale callback changed new profile");
        Bind(Save(saved));Run("open");Check(Position()==2,"Reload did not resume");callbacks.Last()(true);Check(Position()==3,"Act screen did not advance");callbacks.Last()(true);Check(Position()==4&&logs.Count(x=>x=="complete")==1,"Completion/cursor incorrect");
        count=callbacks.Count;Run("open");Check(callbacks.Count==count&&logs.Contains("complete"),"Completed cursor replayed cards");
        Reset();refuse=true;Run("open");Check(Position()==1&&logs.Contains("accepted:false"),"Refusal advanced cursor");dishonest=true;Run("open");Check(Position()==1&&logs.Contains("accepted:false"),"Refused synchronous callback advanced cursor");refuse=false;dishonest=false;
        Run("open");var pending=leases.Last();var stale=callbacks.Last();bus.Publish(new ModStoryEvent(ModStoryEventKind.SceneEnter,null,scene:"map"));Check(pending.Disposed&&logs.Contains("cancel")&&Position()==1,"Scene entry did not cancel");stale(true);Check(Position()==1,"Scene stale callback advanced");
        Run("open");pending=leases.Last();stale=callbacks.Last();Bind(Save());Check(pending.Disposed,"Profile rebind did not release UI");stale(true);Check(Position()==1,"Profile stale callback advanced");
        sync=true;Run("open");Check(Position()==4&&logs.Contains("complete")&&logs.Contains("accepted:true"),"Synchronous sequence failed: "+string.Join("|",logs));sync=false;
        Reset();Run("cancel_open");callbacks.Last()(false);Check(logs.Any(x=>x.Contains("cleanup")),"Cancellation allowed reopening UI");
        Run("open");pending=leases.Last();stale=callbacks.Last();context.Dispose();logs.Clear();stale(true);Check(pending.Disposed&&logs.Count==0,"Disposal did not silence callback");
        // Saved playback requires each runtime capability even when registration succeeded.
        string manifest=File.ReadAllText(Path.Combine(package,"mod.toml"));
        foreach(string capability in new[]{"ui.create","state.read"})
        {
            File.WriteAllText(Path.Combine(package,"mod.toml"),manifest.Replace("\""+capability+"\",",""));
            var restricted=ModDiscovery.DiscoverLoose(args[0]).Mods.Single();
            var restrictedState=new ModStateRuntime();var restrictedBus=new ModStoryEvents((owner,message)=>logs.Add(message));
            var restrictedCatalog=new ModContentCatalog();using var restrictedTx=restrictedCatalog.BeginRegistration(restricted);
            using var restrictedContext=new MoonSharpScriptRuntime(null,null,null,restrictedBus).CreateContext(restricted,
                new ModApiFacade(restricted,new AssetResolver(new IAssetProvider[]{new LooseModProvider(restricted)}),restrictedTx,restrictedState,entry=>logs.Add(entry.Message)));
            restrictedContext.ExecuteEntrypoint();restrictedTx.Commit();
            Check(restrictedState.Bind(Save().DocumentElement,new[]{restrictedContext}).Count==0,"Restricted state bind failed");restrictedBus.BindProfile();
            logs.Clear();int before=callbacks.Count;
            restrictedBus.Publish(new ModStoryEvent(ModStoryEventKind.ItemAcquired,DefinitionId.Parse("fixture.dialog:items/open"),previousCount:0,count:1));
            Check(callbacks.Count==before&&logs.Any(x=>x.Contains(capability)),"Missing capability reached host: "+capability);
        }
        ModStoryDialogAccess.Clear();ModActScreenAccess.Clear();
        Console.WriteLine("PASS: "+checks+" sequence checks: validation, saved cursor, refusal, synchronous completion, duplicates, scene/profile cancellation, callback limits and disposal.");
    }
}
