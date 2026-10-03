using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

// Production ModScriptSession is compiled into this fixture. Only asset-host
// construction is controlled; discovery, ordering, Lua and lifecycle are real.
namespace Eclipse.Modding
{
    public sealed class ModHost
    {
        public IReadOnlyList<ModDescriptor> EnabledMods { get; }
        public AssetResolver Assets { get; }
        public ModHost(IEnumerable<ModDescriptor> mods)
        {
            var order = DependencyResolver.Resolve(mods.ToArray(), ModPlatformVersions.Core);
            if (order.Diagnostics.Any(value => value.Severity == ModDiagnosticSeverity.Error))
                throw new Exception(string.Join("; ", order.Diagnostics));
            EnabledMods = order.OrderedMods;
            Assets = new AssetResolver(EnabledMods.Select(value => (IAssetProvider)new LooseModProvider(value)));
        }
    }
}

static class Program
{
    static int checks;
    static string root, repo;
    static void Check(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    const string ProviderId = "extension.provider";
    const string ConsumerId = "extension.consumer";
    const string Prefix = "local sf2=require('sf2')\n";
    const string Get = "local service=sf2.extensions.get('extension.provider:extensions/calculate',1)\n";
    const string Definition = "sf2.extensions.register{id='calculate',version=1,request={amount='integer'},response={answer='integer'},handler=function(request,caller) ";

    static ModDescriptor Write(string folder, string id, string source, string caps, params string[] dependencies)
    {
        var path = Path.Combine(folder, id);
        Directory.CreateDirectory(Path.Combine(path, "scripts"));
        File.WriteAllText(Path.Combine(path, "scripts/main.lua"), Prefix + source);
        File.WriteAllText(Path.Combine(path, "mod.toml"),
            "schema=1\nid=\"" + id + "\"\nname=\"Fixture\"\nversion=\"1.0.0\"\nauthors=[\"Fixture\"]\nentrypoint=\"scripts/main.lua\"\ncapabilities=[" + caps + "]\n" +
            string.Concat(dependencies.Select(dep => "[[dependencies]]\nid=\"" + dep + "\"\nversion=\">=1.0.0 <2.0.0\"\n")));
        var discovery = ModDiscovery.DiscoverLoose(folder);
        if (discovery.Diagnostics.Any(value => value.Severity == ModDiagnosticSeverity.Error))
            throw new Exception(string.Join("; ", discovery.Diagnostics));
        return discovery.Mods.Single(mod => mod.Id.Value == id);
    }

    static void Run(string provider, string consumer, Action<ModScriptSession, List<ModUiSurface>> inspect = null,
        bool expectError = false, string providerCaps = "\"extensions.provide\"", string consumerCaps = "\"extensions.call\",\"ui.create\"", bool dependency = true)
    {
        var folder = Path.Combine(root, Guid.NewGuid().ToString("N"));
        Write(folder, ProviderId, provider, providerCaps);
        Write(folder, ConsumerId, consumer, consumerCaps, dependency ? new[] { ProviderId } : Array.Empty<string>());
        var host = new ModHost(ModDiscovery.DiscoverLoose(folder).Mods);
        var views = new List<ModUiSurface>();
        var log = new List<ModLogEntry>();
        using var session = ModScriptSession.Start(host, new MoonSharpScriptRuntime(views.Add), log.Add, null);
        Check(session.HasErrors == expectError, "Unexpected startup result: " + session.FormatReport());
        try { inspect?.Invoke(session, views); }
        catch (Exception error) { throw new Exception(error.Message + "\nLua: " + consumer + "\nLog: " + string.Join("; ", log), error); }
        session.Dispose();
        Check(views.All(view => view.IsClosed), "Session teardown leaked a view");
    }

    static string Probe(string body) => Get + "sf2.ui.open{id='probe',mount='menu',root={id='go',kind='button',width=100,height=40,text='Go'},on_click=function() " + body + " end}";
    static void Click(ModScriptSession session, List<ModUiSurface> views) => Check(views.Single().TryClick("go"), "Runtime service probe failed");
    static void Main(string[] args)
    {
        root = args[0]; repo = args[1];
        var good = Definition + "assert(caller=='extension.consumer'); request.amount=request.amount+2; return {answer=request.amount} end}";
        Run(good, Get + "local input={amount=3}; local result=sf2.extensions.call(service,input); assert(input.amount==3 and result.answer==5); result.answer=999; assert(sf2.extensions.call(service,{amount=3}).answer==5)");
        Run(good, Probe("assert(sf2.extensions.call(service,{amount=4}).answer==6)"), Click);
        Run(good, Get, expectError:true, dependency:false);
        Run(good, Get, expectError:true, consumerCaps:"\"ui.create\"");
        Run(good, Get.Replace(",1)",",2)"), expectError:true);
        Run(good, Get.Replace("extensions/calculate","extensions/missing"), expectError:true);
        Run(good, Get.Replace("extensions/calculate","items/calculate"), expectError:true);
        Run(good, Get.Replace(",1)",",0/0)"), expectError:true);
        Run(good, Get, expectError:true, providerCaps:"\"content.register\"");
        Run(good + "\nerror('provider rejected')", "", (session, views) => {
            Check(session.ActiveMods.Count==0 && session.Content.Extensions.Count==0,"Failed provider retained exports/dependent");
            Check(session.Diagnostics.Count==2,"Dependent provider failure did not produce both diagnostics");
        }, expectError:true);
        Run(good + "\n" + good, "", (session, views) => Check(session.Content.Extensions.Count==0,"Duplicate export partially committed"), expectError:true);
        Run(good, "sf2.extensions.register{id='fake',version=1,handler=function() return {} end}", expectError:true);
        foreach(var request in new[]{"{}","{amount=1.5}","{amount=0/0}","{amount=math.huge}","{amount=9007199254740992}","{amount='3'}","{amount=3,caller='core'}","{amount={}}","{amount=function() end}"})
            Run(good, Probe("local ok=sf2.extensions.try_call(service,"+request+"); assert(not ok)"), Click);
        Run(good, Probe("assert(not sf2.extensions.try_call({}, {amount=3}))"), Click);
        foreach(var response in new[]{"nil","{answer='wrong'}","{extra=1}","{answer=math.huge}","{answer=1,native={}}"})
            Run(Definition+"return "+response+" end}", Probe("assert(not sf2.extensions.try_call(service,{amount=3}))"), Click);
        Run(Definition+"while true do end end}", Probe("assert(not sf2.extensions.try_call(service,{amount=3}))"), Click);
        Run(Definition+"error('broken provider') end}", Probe("local ok,message=sf2.extensions.try_call(service,{amount=3}); assert(not ok and string.find(message,'extension.provider') and string.find(message,'extension.consumer'))"), Click);
        Run(good, Probe("for i=1,32 do sf2.extensions.call(service,{amount=i}) end; assert(not sf2.extensions.try_call(service,{amount=3}))"), (session, views) => {
            Click(session,views); Click(session,views); // A fresh callback resets the chain budget.
        });
        Run("sf2.extensions.register{id='calculate',version=1,request={amount={type='integer',default=7}},response={answer={type='integer',default=9},label={type='string',required=false}},handler=function(request) assert(request.amount==7);return {} end}",
            Get+"local result=sf2.extensions.call(service,{}); assert(result.answer==9 and result.label==nil)");
        Run("sf2.extensions.register{id='calculate',version=1,request={flag='boolean',label='string',fraction='number'},response={flag='boolean',label='string',fraction='number'},handler=function(r) return r end}",
            Get+"local r=sf2.extensions.call(service,{flag=true,label='hello',fraction=0.25}); assert(r.flag and r.label=='hello' and r.fraction==0.25)");
        Run(Definition+"assert(not sf2.extensions.try_call(sf2.extensions.get('extension.provider:extensions/calculate',1),{amount=1}));return {answer=1} end}",
            Probe("assert(sf2.extensions.call(service,{amount=1}).answer==1)"),Click,providerCaps:"\"extensions.provide\",\"extensions.call\"");
        Run(good, Get+"sf2.state.register{version=2,fields={value={type='integer',default=0}},migrations={[1]=function(old) assert(not sf2.extensions.try_call(service,{amount=1}));return {value=old.value} end}}",
            (session, views) => {
                var save=new XmlDocument();save.LoadXml("<Warrior><EclipseMods schema='1'><Mod id='extension.consumer' version='1.0.0' active='true'><State format='1' version='1'><Value name='value' type='integer' value='7'/></State></Mod></EclipseMods></Warrior>");
                Check(session.BindState(save.DocumentElement).Count==0,"Migration service rejection prevented migration");
                Check(session.State.TryGetValue(ModId.Parse(ConsumerId),"value",out var value) && value.Integer==7,"Migration did not execute against preserved data");
            },consumerCaps:"\"extensions.call\",\"state.write\"");
        Fingerprints();
        DependencyChains();
        ShippedExample();
        Console.WriteLine("PASS: "+checks+" mod extension checks (real Lua, dependency order, lifecycle, schema, budgets, saved framework and combat/HUD add-on).");
    }

    static void Fingerprints()
    {
        var folder=Path.Combine(root,"fingerprint");
        var mod=Write(folder,ProviderId,"","\"extensions.provide\"");
        string Fingerprint(int version, bool required, ModParameterType type, string name, bool reverse=false)
        {
            var catalog=new ModContentCatalog();
            using(var tx=catalog.BeginRegistration(mod))
            {
                var fields=new[]{new ModParameterDefinition(name,type,required),new ModParameterDefinition("other",ModParameterType.Boolean)};
                tx.RegisterExtension("test",version,new ModParameterSchema(reverse?fields.Reverse():fields),new ModParameterSchema(Array.Empty<ModParameterDefinition>()));
                tx.Commit();
            }
            return ModSaveData.ComputeContentSetFingerprint(new[]{mod},catalog);
        }
        var initial=Fingerprint(1,true,ModParameterType.Integer,"amount");
        Check(initial==Fingerprint(1,true,ModParameterType.Integer,"amount",true),"Schema order changed fingerprint");
        Check(initial!=Fingerprint(2,true,ModParameterType.Integer,"amount"),"Version absent from fingerprint");
        Check(initial!=Fingerprint(1,false,ModParameterType.Integer,"amount"),"Required field absent from fingerprint");
        Check(initial!=Fingerprint(1,true,ModParameterType.Number,"amount"),"Field type absent from fingerprint");
        Check(initial!=Fingerprint(1,true,ModParameterType.Integer,"count"),"Field name absent from fingerprint");
    }

    static void DependencyChains()
    {
        foreach (var depth in new[] { 8, 9 })
        {
            var folder=Path.Combine(root,"chain-"+depth);
            for(var index=0;index<depth;index++)
            {
                var id="extension.chain"+index;
                var previous="extension.chain"+(index-1);
                var get=index==0 ? "" : "local previous=sf2.extensions.get('"+previous+":extensions/step',1)\n";
                var result=index==0 ? "return {answer=r.amount}" : "return sf2.extensions.call(previous,r)";
                Write(folder,id,get+"sf2.extensions.register{id='step',version=1,request={amount='integer'},response={answer='integer'},handler=function(r,caller) assert(caller=='extension.chain"+(index+1)+"'); "+result+" end}",
                    "\"extensions.provide\",\"extensions.call\"", index==0 ? Array.Empty<string>() : new[]{previous});
            }
            Write(folder,"extension.chain"+depth,
                "local service=sf2.extensions.get('extension.chain"+(depth-1)+":extensions/step',1)\n"+
                "sf2.ui.open{id='chain',mount='menu',root={id='go',kind='button',width=100,height=40,text='Go'},on_click=function() "+
                (depth==8 ? "assert(sf2.extensions.call(service,{amount=7}).answer==7)" : "local value,message=sf2.extensions.try_call(service,{amount=7});assert(not value and string.find(message,'budget exceeded'))")+" end}",
                "\"extensions.call\",\"ui.create\"","extension.chain"+(depth-1));
            var host=new ModHost(ModDiscovery.DiscoverLoose(folder).Mods);
            var views=new List<ModUiSurface>();
            using var session=ModScriptSession.Start(host,new MoonSharpScriptRuntime(views.Add),null,null);
            Check(!session.HasErrors,"Valid dependency chain failed to load: "+session.FormatReport());
            Click(session,views);
            Click(session,views); // Limits and execution scopes recover after either success or rejection.
            session.Dispose();
            Check(views.All(view=>view.IsClosed),"Dependency chain leaked views");
        }
    }

    sealed class Fighter : IModFighterOperations, IModIncomingHitSource
    {
        public double Damage;
        public bool Blocked;
        public ModIncomingHit IncomingHit => new ModIncomingHit(()=>Damage,value=>Damage=value,Blocked);
        public bool TryChangeHealth(double value,out string error){error="";return true;}
        public bool TryAddMagicCharge(double value,out string error){error="";return true;}
    }
    static void ShippedExample()
    {
        var folder=Path.Combine(root,"example");
        foreach(var id in new[]{"example.focus-framework","example.focus-addon"})
        {
            var source=Path.Combine(repo,"Mods",id);
            Write(folder,id,File.ReadAllText(Path.Combine(source,"scripts/main.lua")),"\"extensions.provide\"");
            File.Copy(Path.Combine(source,"mod.toml"),Path.Combine(folder,id,"mod.toml"),true);
        }
        var host=new ModHost(ModDiscovery.DiscoverLoose(folder).Mods);
        var views=new List<ModUiSurface>();
        void Import(ModContentCatalog catalog)
        {
            var stages=new XmlDocument();stages.Load(Path.Combine(repo,"Assets/vanillaXml/stages.xml"));
            CoreContentImporter.ImportStages(catalog,stages.SelectSingleNode("Stages/Zones"));
        }
        var save=new XmlDocument();save.LoadXml("<Warrior/>");
        using(var session=ModScriptSession.Start(host,new MoonSharpScriptRuntime(views.Add),null,Import))
        {
            Check(!session.HasErrors,session.FormatReport());
            Check(session.Content.Extensions.Count==2,"Framework definitions missing");
            ModSaveData.RecordContext(save.DocumentElement,session.ActiveMods,session.Content,session.State);
            var diagnostics=session.BindState(save.DocumentElement);
            Check(diagnostics.Count==0,"Framework profile bind failed: "+string.Join("; ",diagnostics));
            var fighter=new Fighter();
            var behavior=DefinitionId.Parse("example.focus-addon:behaviors/focus");
            var fields=new Dictionary<string,string>{{"round","1"},{"source","rule"}};
            void Invoke(ModEffectEvent kind) => Check(session.TryInvokeBehavior(behavior,kind,null,fields,fighter,out var error),error);
            Invoke(ModEffectEvent.RoundBegin);
            Check(views.Single().Read("meter").Text=="Focus: 0/3","HUD did not read framework resource");
            fighter.Damage=.1;Invoke(ModEffectEvent.DamageDealing);
            Check(fighter.Damage==.1 && views.Single().Read("meter").Text=="Focus: 1/3","First hit incorrectly amplified");
            fighter.Blocked=true;Invoke(ModEffectEvent.DamageDealing);
            Check(views.Single().Read("meter").Text=="Focus: 1/3","Blocked hit advanced Focus");
            fighter.Blocked=false;fighter.Damage=.1;Invoke(ModEffectEvent.DamageDealing);
            Check(views.Single().Read("meter").Text=="Focus: 2/3","Second hit missing");
            ModSaveData.RecordContext(save.DocumentElement,session.ActiveMods,session.Content,session.State);
            Invoke(ModEffectEvent.FightEnd);
            Check(views.Single().IsClosed,"Fight end did not close Focus HUD");
        }
        views.Clear();
        using(var session=ModScriptSession.Start(host,new MoonSharpScriptRuntime(views.Add),null,Import))
        {
            Check(session.BindState(save.DocumentElement).Count==0,"Reload lost framework profile");
            var fighter=new Fighter{Damage=.1};
            var id=DefinitionId.Parse("example.focus-addon:behaviors/focus");
            var fields=new Dictionary<string,string>{{"round","1"}};
            Check(session.TryInvokeBehavior(id,ModEffectEvent.RoundBegin,null,fields,fighter,out var error),error);
            Check(views.Single().Read("meter").Text=="Focus: 2/3","Focus did not survive reload");
            Check(session.TryInvokeBehavior(id,ModEffectEvent.DamageDealing,null,fields,fighter,out error),error);
            Check(Math.Abs(fighter.Damage-.15)<1e-10 && views.Single().Read("meter").Text=="Focus: 0/3","Framework/add-on third-hit bonus failed");
        }
        Check(views.All(view=>view.IsClosed),"Reload session teardown leaked HUD");
    }
}
